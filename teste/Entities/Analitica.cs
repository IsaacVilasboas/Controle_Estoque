using System.ComponentModel.DataAnnotations;

namespace teste.Entities
{
    public class Analitica
    {
        [Key]
        public int Id { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Final { get; set; }
        public string? UsuarioID { get; set; }
    }
}
