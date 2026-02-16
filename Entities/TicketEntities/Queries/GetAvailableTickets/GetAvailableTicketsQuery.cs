using Acceloka.Models;
using MediatR;
namespace Acceloka.Entities.TicketEntities.Queries.GetAvailableTickets
{
    public class GetAvailableTicketsQuery : IRequest<IEnumerable<Ticket>>
    {
        public string? CategoryName { get; set; }
        public string? TicketCode { get; set; }
        public string? TicketName { get; set; }
        public decimal? Price { get; set; } 
        public DateTime? MinEventDate { get; set; } 
        public DateTime? MaxEventDate { get; set; }

        public string? OrderBy { get; set; }
        public string? OrderState { get; set; }
    }
}
