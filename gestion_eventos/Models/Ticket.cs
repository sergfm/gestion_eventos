using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_eventos.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }



    }
}
