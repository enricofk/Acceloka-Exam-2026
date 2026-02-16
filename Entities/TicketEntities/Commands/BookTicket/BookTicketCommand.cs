using Acceloka.Models;
using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.BookTicket
{
    public class BookTicketCommand : IRequest<IEnumerable<BookedTicket>>
    {
        public List<BookTicketRequest> Tickets { get; set; } = new();
    }

    public class BookTicketRequest
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}