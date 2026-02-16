using Acceloka.Abstractions;
using Acceloka.Models;
using MediatR;
namespace Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsQuery, IEnumerable<Ticket>>
    {
        private readonly ITicketRepository _ticketRepository;
        public GetAvailableTicketsHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public async Task<IEnumerable<Ticket>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            return await _ticketRepository.GetAvailableTicketsAsync(request);
        }
    }
}
