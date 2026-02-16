using Acceloka.Abstractions;
using Acceloka.Models;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Queries.GetBookedTickets
{
    public class GetBookedTicketsHandler : IRequestHandler<GetBookedTicketsQuery, IEnumerable<BookedTicket>>
    {
        private readonly ITicketRepository _repo;

        public GetBookedTicketsHandler(ITicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BookedTicket>> Handle(GetBookedTicketsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetBookedTicketsAsync(request.BookedTicketId);
        }
    }
}