using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using teste.Areas.Admin.Models;
using teste.Entities;

namespace teste.Entities
{
    public partial class Banco
    {
         [Key]
        public int Id { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Caixa { get; set; }
        [MaxLength(100)]
        public string? Razao { get; set; }
        public DateTime? Dia { get; set; }

        [MaxLength(100)]
        public string? Pagamento { get; set; }
        public decimal? Saida { get; set; }
        public int? FkNotas { get; set; }
        public string? UsuarioID { get; set; }
        public virtual Nota? FkNotasNavigation { get; set; } = null!;
    }
}
