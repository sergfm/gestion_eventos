namespace gestion_eventos.Models
{
    public class CarritoItemViewModel
    {
        public string Title { get; set; }
        public int TicketsQuantity { get; set; }
        public decimal PricePerTicket { get; set; }
        public decimal TotalPrice { get; set; }
        public int CartId { get; set; }
    }
}
