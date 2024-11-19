using System;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class TicketPurchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public string UserId { get; set; } // Almacena el ID del usuario que realizó la compra

        [Required]
        [Range(1, int.MaxValue)]
        public int TicketsBought { get; set; }

        public DateTime PurchaseDate { get; set; }

        // Relación con el modelo Event
        public Event Event { get; set; }
    }
}
