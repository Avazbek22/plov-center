param(
    [string]$BaseUrl = "http://localhost:5288",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "Admin123!"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

$script:CreatedCategoryId = $null
$script:CreatedDishId = $null
$script:TempFilePath = $null

function Convert-ToJsonBody {
    param([object]$Value)

    return ($Value | ConvertTo-Json -Depth 10 -Compress)
}

function Get-StatusCode {
    param([System.Management.Automation.ErrorRecord]$ErrorRecord)

    if ($null -eq $ErrorRecord.Exception.Response) {
        return $null
    }

    return [int]$ErrorRecord.Exception.Response.StatusCode
}

function Invoke-ExpectedStatus {
    param(
        [scriptblock]$Action,
        [int]$ExpectedStatusCode
    )

    try {
        & $Action | Out-Null
        throw "Expected HTTP $ExpectedStatusCode, but the request succeeded."
    }
    catch {
        $actualStatusCode = Get-StatusCode $_

        if ($actualStatusCode -ne $ExpectedStatusCode) {
            throw
        }

        return $actualStatusCode
    }
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

    $invalidLoginStatus = Invoke-ExpectedStatus {
        Invoke-WebRequest `
            -Uri "$BaseUrl/api/admin/auth/login" `
            -Method Post `
            -ContentType "application/json" `
            -Body (Convert-ToJsonBody @{ username = $AdminUsername; password = "wrong-password" }) `
            -UseBasicParsing
    } 401

    $unauthorizedMeStatus = Invoke-ExpectedStatus {
        Invoke-WebRequest -Uri "$BaseUrl/api/admin/auth/me" -UseBasicParsing
    } 401

    $login = Invoke-RestMethod `
        -Uri "$BaseUrl/api/admin/auth/login" `
        -Method Post `
        -ContentType "application/json" `
        -Body (Convert-ToJsonBody @{ username = $AdminUsername; password = $AdminPassword })

    $headers = @{ Authorization = "Bearer $($login.token)" }

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
    $deleteCategoryWithDishStatus = Invoke-ExpectedStatus {
        Invoke-WebRequest -Uri "$BaseUrl/api/admin/categories/$($category.id)" -Method Delete -Headers $headers -UseBasicParsing
    } 409

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
        InvalidLogin = $invalidLoginStatus
        UnauthorizedMe = $unauthorizedMeStatus
        Admin = $me.username
        CategoryCreated = $category.id
        DishCreated = $dish.id
        AdminDishesCount = @($adminDishes).Count
        DeleteCategoryWithDish = $deleteCategoryWithDishStatus
        PublicMenuCategories = @($publicMenu.categories).Count
        PublicContentFetched = ($null -ne $publicContent.about -and $null -ne $publicContent.contacts)
        UploadStatus = [int]$uploadResponseMessage.StatusCode
        UploadedFileStatus = $uploadedFile.StatusCode
    } | ConvertTo-Json -Compress
}
finally {
    Remove-TestArtifacts -Headers $headers
}
