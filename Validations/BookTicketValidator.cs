using Acceloka.Entities.TicketEntities.Commands.BookTicket;
using FluentValidation;

namespace Acceloka.Validations
{
    public class BookTicketValidator : AbstractValidator<BookTicketCommand>
    {
        public BookTicketValidator()
        {
            RuleFor(x => x.Tickets)
                .NotEmpty().WithMessage("Ticket list cannot be empty.");

            RuleForEach(x => x.Tickets).SetValidator(new BookTicketItemValidator());
        }
    }

    public class BookTicketItemValidator : AbstractValidator<BookTicketRequest>
    {
        public BookTicketItemValidator()
        {
            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("Ticket Code is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
