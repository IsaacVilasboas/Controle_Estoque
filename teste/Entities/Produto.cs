using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using teste.Areas.Admin.Models;

namespace teste.Entities
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20, ErrorMessage = "Código do produto")]
        public string? CodBarra { get; set; }
        [MaxLength(30)]
        public string? Marca { get; set; }
        [MaxLength(50)]
        public string? NomeProduto { get; set; }
        public decimal? Preco { get; set; }
        public decimal? Media { get; set; }
        public int? Qtd { get; set; }

        [Required, MaxLength(40, ErrorMessage = "Categoria do produto")]
        public string? Categoria { get; set; }
        [MaxLength(40)]
        public string? Sabor { get; set; }
        [MaxLength(20)]
        public string? Recipiente { get; set; }

        [MaxLength (30)]
        public string? Tipo { get; set; }
        [MaxLength(20)]
        public string? Peso { get; set; }

        public DateTime Validade { get; set; }
        public DateTime Cadastro { get; set; }
        public string? UsuarioID { get; set; }
        public virtual ICollection<Nota>? Nota { get; set; }

    }
}
