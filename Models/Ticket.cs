namespace Acceloka.Models
{
    public class Ticket
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int Quota { get; set; }
    }
}
