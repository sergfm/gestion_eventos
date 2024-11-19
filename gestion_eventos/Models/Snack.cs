using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models;

public class Snack
{
    [Key]
    public int SnackId { get; set; }
    public string SnackType { get; set; }
    public DateTime ProvidedOn { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }
}
