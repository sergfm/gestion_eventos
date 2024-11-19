using System.ComponentModel.DataAnnotations;

namespace gestion_eventos.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "El nombre del titular es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "El número de tarjeta es obligatorio.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta solo debe contener dígitos.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "La fecha debe tener el formato MM/YY.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/[0-9]{2}$", ErrorMessage = "Formato inválido. Use MM/YY.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "El CVV es obligatorio.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El CVV debe tener 3 dígitos.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "El CVV solo debe contener dígitos.")]
        public string CVV { get; set; }
    }
}
