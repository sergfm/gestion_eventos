using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public int AvailableTickets { get; set; }

        [Required]
        [Range(15, 100, ErrorMessage = "El precio debe estar entre $15 y $100.")]
        public decimal Price { get; set; } // Precio del evento

        // Relaciones con otras entidades
        public ICollection<Schedule> Schedules { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<Snack> Snacks { get; set; }


        [Url(ErrorMessage = "Debe ser una URL válida.")]
        public string? ImagenURL { get; set; } // Nueva propiedad
    }
}
