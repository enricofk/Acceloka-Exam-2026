using Acceloka.Abstractions;
using Acceloka.Models;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.BookTicket
{
    public class BookTicketHandler : IRequestHandler<BookTicketCommand, IEnumerable<BookedTicket>>
    {
        private readonly ITicketRepository _ticketRepository;

        public BookTicketHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<BookedTicket>> Handle(BookTicketCommand request, CancellationToken cancellationToken)
        {
            return await _ticketRepository.BookTicketsAsync(request.Tickets);
        }
    }
}