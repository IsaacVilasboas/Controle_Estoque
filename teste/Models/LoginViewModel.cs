using System.ComponentModel.DataAnnotations;

namespace teste.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="O Usuario é obrigatório")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatária")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [Display(Name ="Lembrar-me")]
        public bool RememberMe { get; set; }
    }
}
