namespace gestion_eventos.Models
{
    public class MisEventosViewModel
    {
        public int EventId { get; set; } // Agregado para evitar el error
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int TicketsBought { get; set; }
    }

}
