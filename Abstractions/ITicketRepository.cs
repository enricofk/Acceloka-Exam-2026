using Acceloka.Entities.TicketEntities.Commands.BookTicket;
using Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets;
using Acceloka.Entities.TicketEntities.Commands.BookTicket;
using Acceloka.Models;
using Acceloka.Models;
namespace Acceloka.Abstractions
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync(GetAvailableTicketsQuery request);
        Task<List<BookedTicket>> BookTicketsAsync(List<BookTicketRequest> tickets);
        Task<IEnumerable<BookedTicket>> GetBookedTicketsAsync(string bookedTicketId);
        Task RevokeTicketAsync(string bookedTicketId, string ticketCode, int quantity);
        Task EditBookedTicketAsync(string bookedTicketId, string ticketCode, int newQuantity);
    }
}
