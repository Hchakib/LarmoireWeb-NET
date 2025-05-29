using System.ComponentModel.DataAnnotations;

namespace LarmoireWeb.ViewModels
{
    public class RegisterVm
    {
        [Required] public string Prenom { get; set; }
        [Required] public string Nom { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Adresse { get; set; }

        [Required, Phone(ErrorMessage = "Le format du téléphone est invalide.")]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
