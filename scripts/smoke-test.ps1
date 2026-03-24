param(
    [string]$BaseUrl = "http://localhost:5288",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "Admin123!"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

$script:WebApiProjectPath = (Resolve-Path (Join-Path $PSScriptRoot "..\src\WebApi")).Path
$script:WebApiBinPath = Join-Path $script:WebApiProjectPath "bin\Debug\net10.0"
$script:CreatedCategoryId = $null
$script:CreatedDishId = $null
$script:TempFilePath = $null

function Convert-ToJsonBody {
    param([object]$Value)

    return ($Value | ConvertTo-Json -Depth 10 -Compress)
}

function New-HttpResponse {
    param(
        [string]$Method,
        [string]$Uri,
        [hashtable]$Headers = @{},
        [string]$Body = ""
    )

    $client = [System.Net.Http.HttpClient]::new()

    try {
        foreach ($header in $Headers.GetEnumerator()) {
            if ($header.Key -ieq "Authorization") {
                $parts = $header.Value -split " ", 2

                if ($parts.Length -eq 2) {
                    $client.DefaultRequestHeaders.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new($parts[0], $parts[1])
                }

                continue
            }

            $null = $client.DefaultRequestHeaders.TryAddWithoutValidation($header.Key, [string]$header.Value)
        }

        $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), $Uri)

        if ($Body -ne "") {
            $request.Content = [System.Net.Http.StringContent]::new($Body, [System.Text.Encoding]::UTF8, "application/json")
        }

        $response = $client.SendAsync($request).Result
        $content = $response.Content.ReadAsStringAsync().Result
        $json = $null

        if (-not [string]::IsNullOrWhiteSpace($content)) {
            $json = $content | ConvertFrom-Json
        }

        return [pscustomobject]@{
            StatusCode = [int]$response.StatusCode
            Content = $content
            Json = $json
        }
    }
    finally {
        $client.Dispose()
    }
}

function Assert-ApiErrorResponse {
    param(
        [object]$Response,
        [int]$ExpectedStatusCode,
        [string]$ExpectedCode
    )

    if ($Response.StatusCode -ne $ExpectedStatusCode) {
        throw "Expected HTTP $ExpectedStatusCode, but got $($Response.StatusCode)."
    }

    if ($null -eq $Response.Json) {
        throw "Expected JSON error payload for HTTP $ExpectedStatusCode."
    }

    if ($Response.Json.code -ne $ExpectedCode) {
        throw "Expected error code '$ExpectedCode', but got '$($Response.Json.code)'."
    }

    if ([string]::IsNullOrWhiteSpace($Response.Json.message)) {
        throw "Error payload is missing the 'message' field."
    }

    if ([string]::IsNullOrWhiteSpace($Response.Json.traceId)) {
        throw "Error payload is missing the 'traceId' field."
    }

    if ($null -ne $Response.Json.errors) {
        throw "Expected 'errors' to be null for auth error payloads."
    }
}

function Assert-StatusCode {
    param(
        [object]$Response,
        [int]$ExpectedStatusCode
    )

    if ($Response.StatusCode -ne $ExpectedStatusCode) {
        throw "Expected HTTP $ExpectedStatusCode, but got $($Response.StatusCode)."
    }
}

function Convert-ToBase64Url {
    param([byte[]]$Bytes)

    return [Convert]::ToBase64String($Bytes).TrimEnd("=").Replace("+", "-").Replace("/", "_")
}

function New-ForbiddenScenarioToken {
    $appSettings = Get-Content (Join-Path $script:WebApiProjectPath "appsettings.json") | ConvertFrom-Json
    $jwtSettings = $appSettings.Jwt
    $userId = [Guid]::NewGuid().ToString()
    $now = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
    $expiresAtUtc = [DateTimeOffset]::UtcNow.AddMinutes(10).ToUnixTimeSeconds()

    $header = Convert-ToJsonBody @{
        alg = "HS256"
        typ = "JWT"
    }

    $payload = Convert-ToJsonBody @{
        sub = $userId
        unique_name = "smoke-forbidden"
        name = "smoke-forbidden"
        iss = $jwtSettings.Issuer
        aud = $jwtSettings.Audience
        nbf = $now
        exp = $expiresAtUtc
    }

    $unsignedToken = "{0}.{1}" -f `
        (Convert-ToBase64Url ([System.Text.Encoding]::UTF8.GetBytes($header))), `
        (Convert-ToBase64Url ([System.Text.Encoding]::UTF8.GetBytes($payload)))

    $hmac = [System.Security.Cryptography.HMACSHA256]::new([System.Text.Encoding]::UTF8.GetBytes($jwtSettings.SigningKey))

    try {
        $signature = Convert-ToBase64Url ($hmac.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($unsignedToken)))
    }
    finally {
        $hmac.Dispose()
    }

    return "$unsignedToken.$signature"
}

function Remove-TestArtifacts {
    param([hashtable]$Headers)

    if ($script:CreatedDishId) {
        try {
            Invoke-WebRequest -Uri "$BaseUrl/api/admin/dishes/$script:CreatedDishId" -Method Delete -Headers $Headers -UseBasicParsing | Out-Null
        }
        catch {
        }
    }

    if ($script:CreatedCategoryId) {
        try {
            Invoke-WebRequest -Uri "$BaseUrl/api/admin/categories/$script:CreatedCategoryId" -Method Delete -Headers $Headers -UseBasicParsing | Out-Null
        }
        catch {
        }
    }

    if ($script:TempFilePath -and (Test-Path $script:TempFilePath)) {
        Remove-Item $script:TempFilePath -Force
    }
}

$headers = @{}

try {
    $swagger = Invoke-WebRequest -Uri "$BaseUrl/swagger/index.html" -UseBasicParsing

    $invalidLoginResponse = New-HttpResponse `
        -Method "POST" `
        -Uri "$BaseUrl/api/admin/auth/login" `
        -Body (Convert-ToJsonBody @{ username = $AdminUsername; password = "wrong-password" })
    Assert-ApiErrorResponse -Response $invalidLoginResponse -ExpectedStatusCode 401 -ExpectedCode "unauthorized"

    $unauthorizedMeResponse = New-HttpResponse `
        -Method "GET" `
        -Uri "$BaseUrl/api/admin/auth/me"
    Assert-ApiErrorResponse -Response $unauthorizedMeResponse -ExpectedStatusCode 401 -ExpectedCode "unauthorized"

    $login = Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/auth/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{ username = $AdminUsername; password = $AdminPassword })

    $headers = @{ Authorization = "Bearer $($login.token)" }
    $forbiddenHeaders = @{ Authorization = "Bearer $(New-ForbiddenScenarioToken)" }

    $forbiddenResponse = New-HttpResponse `
        -Method "GET" `
        -Uri "$BaseUrl/api/admin/categories" `
        -Headers $forbiddenHeaders
    Assert-ApiErrorResponse -Response $forbiddenResponse -ExpectedStatusCode 403 -ExpectedCode "forbidden"

    $me = Invoke-RestMethod -Uri "$BaseUrl/api/admin/auth/me" -Headers $headers
    $adminContent = Invoke-RestMethod -Uri "$BaseUrl/api/admin/content" -Headers $headers

    Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/content/about" `
        -Method Put `
        -Headers $headers `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{
            text = $adminContent.about.text
            photoPath = $adminContent.about.photoPath
        }) | Out-Null

    Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/content/contacts" `
        -Method Put `
        -Headers $headers `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{
            address = $adminContent.contacts.address
            phone = $adminContent.contacts.phone
            hours = $adminContent.contacts.hours
            mapEmbed = $adminContent.contacts.mapEmbed
        }) | Out-Null

    $suffix = [Guid]::NewGuid().ToString("N").Substring(0, 8)

    $category = Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/categories" `
        -Method Post `
        -Headers $headers `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{
            name = "Smoke Category $suffix"
            sortOrder = 777
            isVisible = $true
        })

    $script:CreatedCategoryId = $category.id

    $dish = Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/dishes" `
        -Method Post `
        -Headers $headers `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{
            categoryId = $category.id
            name = "Smoke Dish $suffix"
            description = "smoke"
            price = 123.45
            photoPath = $null
            sortOrder = 1
            isVisible = $true
        })

    $script:CreatedDishId = $dish.id

    $adminDishes = Invoke-RestMethod -Uri "$BaseUrl/api/admin/dishes?categoryId=$($category.id)" -Headers $headers
    $deleteCategoryWithDishResponse = New-HttpResponse `
        -Method "DELETE" `
        -Uri "$BaseUrl/api/admin/categories/$($category.id)" `
        -Headers $headers
    Assert-StatusCode -Response $deleteCategoryWithDishResponse -ExpectedStatusCode 409

    $publicMenu = Invoke-RestMethod -Uri "$BaseUrl/api/public/menu"
    $publicContent = Invoke-RestMethod -Uri "$BaseUrl/api/public/content"

    $script:TempFilePath = Join-Path $env:TEMP "plov-center-smoke-$suffix.png"
    [System.IO.File]::WriteAllBytes(
        $script:TempFilePath,
        [Convert]::FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO2pQ9sAAAAASUVORK5CYII="))

    $client = [System.Net.Http.HttpClient]::new()
    try {
        $client.DefaultRequestHeaders.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $login.token)

        $form = [System.Net.Http.MultipartFormDataContent]::new()
        $form.Add([System.Net.Http.StringContent]::new("Dish"), "Area")

        $fileBytes = [System.IO.File]::ReadAllBytes($script:TempFilePath)
        $fileContent = [System.Net.Http.ByteArrayContent]::new($fileBytes)
        $fileContent.Headers.ContentType = [System.Net.Http.Headers.MediaTypeHeaderValue]::Parse("image/png")
        $form.Add($fileContent, "File", "smoke.png")

        $uploadResponseMessage = $client.PostAsync("$BaseUrl/api/admin/uploads/image", $form).Result

        if (-not $uploadResponseMessage.IsSuccessStatusCode) {
            throw "Upload request failed with status code $([int]$uploadResponseMessage.StatusCode)."
        }

        $upload = $uploadResponseMessage.Content.ReadAsStringAsync().Result | ConvertFrom-Json
        $uploadedFile = Invoke-WebRequest -Uri $upload.url -UseBasicParsing
    }
    finally {
        $client.Dispose()
    }

    Remove-TestArtifacts -Headers $headers
    $script:CreatedDishId = $null
    $script:CreatedCategoryId = $null

    [pscustomobject]@{
        Swagger = $swagger.StatusCode
        InvalidLogin = $invalidLoginResponse.StatusCode
        InvalidLoginCode = $invalidLoginResponse.Json.code
        UnauthorizedMe = $unauthorizedMeResponse.StatusCode
        UnauthorizedMeCode = $unauthorizedMeResponse.Json.code
        ForbiddenStatus = $forbiddenResponse.StatusCode
        ForbiddenCode = $forbiddenResponse.Json.code
        Admin = $me.username
        CategoryCreated = $category.id
        DishCreated = $dish.id
        AdminDishesCount = @($adminDishes).Count
        DeleteCategoryWithDish = $deleteCategoryWithDishResponse.StatusCode
        PublicMenuCategories = @($publicMenu.categories).Count
        PublicContentFetched = ($null -ne $publicContent.about -and $null -ne $publicContent.contacts)
        UploadStatus = [int]$uploadResponseMessage.StatusCode
        UploadedFileStatus = $uploadedFile.StatusCode
    } | ConvertTo-Json -Compress
}
finally {
    Remove-TestArtifacts -Headers $headers
}
