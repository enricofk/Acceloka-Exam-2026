namespace Acceloka.Models
{
    public class BookedTicket
    {
        public int Id { get; set; }
        public string BookedTickedId { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
