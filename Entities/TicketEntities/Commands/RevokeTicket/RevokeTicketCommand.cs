using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.RevokeTicket
{
    public class RevokeTicketCommand : IRequest
    {
        public string BookedTicketId { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; } 
    }
}