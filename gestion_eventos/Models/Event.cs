using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models;

public class Event
{
    [Key]
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int AvailableTickets { get; set; }

    public ICollection<Schedule> Schedules { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<Attendance> Attendances { get; set; }
    public ICollection<Snack> Snacks { get; set; }
}
