using FluentValidation;
using PlovCenter.Application.Abstractions.Persistence;
using PlovCenter.Application.Abstractions.Services;
using PlovCenter.Application.Common.Cqrs;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Dishes;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes;

public sealed record GetAdminDishesQuery(Guid? CategoryId) : IApplicationRequest<IReadOnlyCollection<DishResponse>>;

public sealed record GetDishByIdQuery(Guid DishId) : IApplicationRequest<DishResponse>;

public sealed record CreateDishCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder,
    bool IsVisible) : IApplicationRequest<DishResponse>;

public sealed record UpdateDishCommand(
    Guid DishId,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? PhotoPath,
    int SortOrder,
    bool IsVisible) : IApplicationRequest<DishResponse>;

public sealed record DeleteDishCommand(Guid DishId) : IApplicationRequest<bool>;

public sealed record SetDishVisibilityCommand(Guid DishId, bool IsVisible) : IApplicationRequest<DishResponse>;

public sealed class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.DishNameMaxLength);

        RuleFor(static command => command.Description)
            .MaximumLength(ValidationRules.DishDescriptionMaxLength);

        RuleFor(static command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.PhotoPath)
            .MaximumLength(512);
    }
}

public sealed class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.DishNameMaxLength);

        RuleFor(static command => command.Description)
            .MaximumLength(ValidationRules.DishDescriptionMaxLength);

        RuleFor(static command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.PhotoPath)
            .MaximumLength(512);
    }
}

public sealed class GetDishByIdQueryValidator : AbstractValidator<GetDishByIdQuery>
{
    public GetDishByIdQueryValidator()
    {
        RuleFor(static query => query.DishId).NotEmpty();
    }
}

public sealed class DeleteDishCommandValidator : AbstractValidator<DeleteDishCommand>
{
    public DeleteDishCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
    }
}

public sealed class SetDishVisibilityCommandValidator : AbstractValidator<SetDishVisibilityCommand>
{
    public SetDishVisibilityCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
    }
}

internal sealed class GetAdminDishesQueryHandler(IDishRepository dishRepository)
    : IApplicationRequestHandler<GetAdminDishesQuery, IReadOnlyCollection<DishResponse>>
{
    public async Task<IReadOnlyCollection<DishResponse>> HandleAsync(
        GetAdminDishesQuery request,
        CancellationToken cancellationToken)
    {
        var dishes = await dishRepository.GetAllAsync(request.CategoryId, cancellationToken);
        return dishes.Select(dish => DishMappings.ToResponse(dish)).ToArray();
    }
}

internal sealed class GetDishByIdQueryHandler(IDishRepository dishRepository)
    : IApplicationRequestHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<DishResponse> HandleAsync(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetByIdAsync(request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        return DishMappings.ToResponse(dish);
    }
}

internal sealed class CreateDishCommandHandler(
    IDishRepository dishRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<CreateDishCommand, DishResponse>
{
    public async Task<DishResponse> HandleAsync(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var dish = new Dish(
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.PhotoPath,
            request.SortOrder,
            request.IsVisible,
            dateTimeProvider.UtcNow);

        dishRepository.Add(dish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return DishMappings.ToResponse(dish, category.Name);
    }
}

internal sealed class UpdateDishCommandHandler(
    IDishRepository dishRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<UpdateDishCommand, DishResponse>
{
    public async Task<DishResponse> HandleAsync(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetByIdAsync(request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        dish.Update(
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.PhotoPath,
            request.SortOrder,
            request.IsVisible,
            dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return DishMappings.ToResponse(dish, category.Name);
    }
}

internal sealed class DeleteDishCommandHandler(IDishRepository dishRepository, IUnitOfWork unitOfWork)
    : IApplicationRequestHandler<DeleteDishCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetByIdAsync(request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        dishRepository.Remove(dish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

internal sealed class SetDishVisibilityCommandHandler(
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IApplicationRequestHandler<SetDishVisibilityCommand, DishResponse>
{
    public async Task<DishResponse> HandleAsync(SetDishVisibilityCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetByIdAsync(request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        dish.SetVisibility(request.IsVisible, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return DishMappings.ToResponse(dish);
    }
}

internal static class DishMappings
{
    public static DishResponse ToResponse(Dish dish, string? categoryName = null)
    {
        return new DishResponse(
            dish.Id,
            dish.CategoryId,
            categoryName ?? dish.Category?.Name ?? string.Empty,
            dish.Name,
            dish.Description,
            dish.Price,
            dish.PhotoPath,
            dish.SortOrder,
            dish.IsVisible,
            dish.CreatedUtc,
            dish.UpdatedUtc);
    }
}
