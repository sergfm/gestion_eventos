using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models;

public class Person
{
    [Key]
    public int PersonId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<Attendance> Attendances { get; set; }
    public ICollection<Snack> Snacks { get; set; }
}
