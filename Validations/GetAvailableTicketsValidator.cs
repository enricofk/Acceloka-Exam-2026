using Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets;
using FluentValidation;

namespace Acceloka.Validations
{
    public class GetAvailableTicketsValidator : AbstractValidator<GetAvailableTicketsQuery>
    {
        public GetAvailableTicketsValidator()
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price cannot be negative.");

            RuleFor(x => x.MaxEventDate)
                .GreaterThanOrEqualTo(x => x.MinEventDate!.Value)
                .When(x => x.MaxEventDate.HasValue && x.MinEventDate.HasValue)
                .WithMessage("MaxEventDate must be greater than or equal to MinEventDate.");

            RuleFor(x => x.OrderState)
                .Must(x => string.IsNullOrEmpty(x) || x.ToLower() == "asc" || x.ToLower() == "desc")
                .WithMessage("OrderState must be 'asc' or 'desc'.");
        }
    }
}