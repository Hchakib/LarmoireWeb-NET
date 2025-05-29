// ViewModels/LoginVm.cs
using System.ComponentModel.DataAnnotations;

namespace LarmoireWeb.ViewModels
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}
