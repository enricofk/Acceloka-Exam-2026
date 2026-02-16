using Acceloka.Models;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Queries.GetBookedTickets
{
    public class GetBookedTicketsQuery : IRequest<IEnumerable<BookedTicket>>
    {
        public string BookedTicketId { get; set; } = string.Empty;
    }
}