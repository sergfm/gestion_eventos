using System;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ActivityDescription { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; } // Asegúrate de tener esta propiedad

        // Relación con Event
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
