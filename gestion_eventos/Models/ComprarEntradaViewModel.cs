namespace gestion_eventos.Models
{
    public class ComprarEntradaViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int AvailableTickets { get; set; }
    }
}
