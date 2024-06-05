using Microsoft.AspNetCore.Identity;
using teste.Entities;
using teste.Models;

namespace teste.Areas.Admin.Models
{
    public class RoleEdit
    {
        public IdentityRole? Role { get; set; }
        public IEnumerable<IdentityUser>? Members { get; set; }
        public IEnumerable<IdentityUser>? NonMembers { get; set; }

        public virtual ICollection<Produto>? Produtos { get; set; }
        public virtual ICollection<Nota>? Nota { get; set; }
        public virtual ICollection<Banco>? Bancos { get; set; }

    }

}
