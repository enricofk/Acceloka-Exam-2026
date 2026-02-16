using MediatR;

namespace Acceloka.Entities.TicketEntities.Commands.EditBookedTicket
{
    public class EditBookedTicketCommand : IRequest
    {
        public string BookedTicketId { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }
}