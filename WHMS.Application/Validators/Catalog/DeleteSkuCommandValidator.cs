using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using WHMS.Application.Features.Command.Catalog.DeleteSku;

namespace WHMS.Application.Validators.Catalog;

public class DeleteSkuCommandValidator : AbstractValidator<DeleteSkuCommandRequest>
{
    public DeleteSkuCommandValidator()
    {
        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");
    }
}