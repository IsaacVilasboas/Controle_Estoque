using System.ComponentModel.DataAnnotations;

namespace teste.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [Required]
        [Display(Name ="Confirme a senha")]
        [Compare("Senha", ErrorMessage = "As senhas não são compativeis")]
        public string? ConfSenha { get; set; }

    }
}
