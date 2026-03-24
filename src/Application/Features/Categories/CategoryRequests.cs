using FluentValidation;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Categories;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Categories;

public sealed record GetAdminCategoriesQuery() : IApplicationRequest<IReadOnlyCollection<CategoryResponse>>;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IApplicationRequest<CategoryResponse>;

public sealed record CreateCategoryCommand(string Name, int SortOrder, bool IsVisible) : IApplicationRequest<CategoryResponse>;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name, int SortOrder, bool IsVisible)
    : IApplicationRequest<CategoryResponse>;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IApplicationRequest<bool>;

public sealed record SetCategoryVisibilityCommand(Guid CategoryId, bool IsVisible) : IApplicationRequest<CategoryResponse>;

public sealed record ReorderCategoriesCommand(IReadOnlyCollection<ReorderCategoryItemRequest> Items) : IApplicationRequest<bool>;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.CategoryNameMaxLength);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.CategoryNameMaxLength);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}

public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(static query => query.CategoryId).NotEmpty();
    }
}

public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();
    }
}

public sealed class SetCategoryVisibilityCommandValidator : AbstractValidator<SetCategoryVisibilityCommand>
{
    public SetCategoryVisibilityCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();
    }
}

public sealed class ReorderCategoriesCommandValidator : AbstractValidator<ReorderCategoriesCommand>
{
    public ReorderCategoriesCommandValidator()
    {
        RuleFor(static command => command.Items).NotEmpty();

        RuleForEach(static command => command.Items)
            .ChildRules(item =>
            {
                item.RuleFor(static value => value.CategoryId).NotEmpty();
                item.RuleFor(static value => value.SortOrder).GreaterThanOrEqualTo(0);
            });
    }
}

internal sealed class GetAdminCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IApplicationRequestHandler<GetAdminCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<IReadOnlyCollection<CategoryResponse>> HandleAsync(
        GetAdminCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(CategoryMappings.ToResponse).ToArray();
    }
}

internal sealed class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    : IApplicationRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> HandleAsync(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdWithDishesAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        return CategoryMappings.ToResponse(category);
    }
}

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> HandleAsync(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.SortOrder, request.IsVisible, dateTimeProvider.UtcNow);

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryMappings.ToResponse(category);
    }
}

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> HandleAsync(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdWithDishesAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        category.Update(request.Name, request.SortOrder, request.IsVisible, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryMappings.ToResponse(category);
    }
}

internal sealed class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IApplicationRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdWithDishesAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        if (category.Dishes.Count > 0)
        {
            throw new ConflictException("A category with dishes cannot be deleted.");
        }

        categoryRepository.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

internal sealed class SetCategoryVisibilityCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<SetCategoryVisibilityCommand, CategoryResponse>
{
    public async Task<CategoryResponse> HandleAsync(
        SetCategoryVisibilityCommand request,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdWithDishesAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        category.SetVisibility(request.IsVisible, dateTimeProvider.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CategoryMappings.ToResponse(category);
    }
}

internal sealed class ReorderCategoriesCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<ReorderCategoriesCommand, bool>
{
    public async Task<bool> HandleAsync(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var requestedIds = request.Items.Select(static item => item.CategoryId).ToArray();

        if (requestedIds.Distinct().Count() != requestedIds.Length)
        {
            throw new ConflictException("Category reorder payload contains duplicate identifiers.");
        }

        var categories = await categoryRepository.GetByIdsAsync(requestedIds, cancellationToken);

        if (categories.Count != requestedIds.Length)
        {
            throw new NotFoundException("One or more categories were not found.");
        }

        var sortOrderById = request.Items.ToDictionary(static item => item.CategoryId, static item => item.SortOrder);

        foreach (var category in categories)
        {
            category.SetSortOrder(sortOrderById[category.Id], dateTimeProvider.UtcNow);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

internal static class CategoryMappings
{
    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.SortOrder,
            category.IsVisible,
            category.Dishes.Count,
            category.CreatedUtc,
            category.UpdatedUtc);
    }
}
