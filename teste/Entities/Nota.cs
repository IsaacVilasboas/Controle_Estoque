using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using teste.Areas.Admin.Models;

namespace teste.Entities
{
    public partial class Nota
    {
        [Key]
        public int Id { get; set; }

        public int? FkProdutos { get; set; }

        [MaxLength(50)]
        public string? Clientes { get; set; }
        [MaxLength(50)]
        public string? Cpf { get; set; }
        [MaxLength(50)]
        public string? Cod { get; set; }
        [MaxLength(30)]
        public string? Pagamento { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Gastos { get; set; }
        public DateTime DataSaida { get; set; }
        public int? Uni { get; set; }
        
        public string? UsuarioID { get; set; }
        public virtual Produto? FkProdutosNavigation { get; set; }
        public virtual ICollection<Banco>? Bancos { get; set; }
    }
}
