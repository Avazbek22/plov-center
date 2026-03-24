using FluentValidation;
using PlovCenter.Application.Contract.Uploads.Commands;

namespace PlovCenter.Application.Features.Uploads.Commands;

public sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(static command => command.Area).IsInEnum();
        RuleFor(static command => command.FileName).NotEmpty();
        RuleFor(static command => command.Size).GreaterThan(0);
    }
}
