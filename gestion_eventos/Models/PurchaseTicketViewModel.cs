using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class PurchaseTicketViewModel
    {
        [Required]
        public int EventId { get; set; } // ID del evento

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe comprar al menos un boleto.")]
        public int TicketsToBuy { get; set; } // Cantidad de boletos a comprar

        public string EventTitle { get; set; } // Título del evento

        public int AvailableTickets { get; set; } // Boletos disponibles

        public decimal Price { get; set; } // Precio por boleto
    }
}
