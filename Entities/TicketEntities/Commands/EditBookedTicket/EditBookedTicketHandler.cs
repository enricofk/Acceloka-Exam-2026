using Acceloka.Abstractions;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.EditBookedTicket
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketCommand>
    {
        private readonly ITicketRepository _ticketRepository;

        public EditBookedTicketHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task Handle(EditBookedTicketCommand request, CancellationToken cancellationToken)
        {
            await _ticketRepository.EditBookedTicketAsync(request.BookedTicketId, request.TicketCode, request.NewQuantity);
        }
    }
}