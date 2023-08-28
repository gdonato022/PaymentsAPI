using FluentValidation;

namespace Plooto.Payments.Application.UseCases.Bill.Commands
{
    /// <summary>
    /// CreateBillCommand validator
    /// </summary>
    public class CreateBillCommandValidator : AbstractValidator<CreateBillCommand>
    {
        /// <summary>
        /// Constructor, performs validation over the command
        /// </summary>
        public CreateBillCommandValidator()
        {
            RuleFor(f => f.VendorName)
                .NotEmpty();

            RuleFor(f => f.Amount)
                .GreaterThan(0);
        }
    }
}
