using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; } // ID del ítem en el carrito

        [Required]
        public string UserId { get; set; } // ID del usuario que agregó el ítem

        [Required]
        public int EventId { get; set; } // ID del evento asociado

        [Required]
        public int TicketsQuantity { get; set; } // Cantidad de boletos

        [Required]
        public decimal PricePerTicket { get; set; } // Precio por boleto

        [Required]
        public decimal TotalPrice { get; set; } // Precio total por el ítem

        public Event Event { get; set; } // Relación con el modelo Event
    }
}
