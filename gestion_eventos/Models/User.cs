using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }  // Considera encriptar las contraseñas para mayor seguridad

        [Required]
        [StringLength(50)]
        public string Role { get; set; }  // Admin, Cliente, Vendedor
        public string Email { get; set; } // Esta propiedad debe existir
    }
}
