using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using teste.Entities;

namespace teste.LojaContext
{
    public class Context : IdentityDbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Analitica> Analiticas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Produto>();
                

            modelBuilder.Entity<Nota>()
                .HasOne(n => n.FkProdutosNavigation)
                .WithMany(p => p.Nota) // Corrigindo o nome da propriedade de navegação em Produto
                .HasForeignKey(n => n.FkProdutos) // Corrigindo o nome da chave estrangeira
                .IsRequired(false) // Define se é obrigatório ou não
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Banco>()
                .HasOne(n => n.FkNotasNavigation)
                .WithMany(p => p.Bancos) // Corrigindo o nome da propriedade de navegação em Produto
                .HasForeignKey(n => n.FkNotas) // Corrigindo o nome da chave estrangeira
                .IsRequired(false) // Define se é obrigatório ou não
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity(typeof(Analitica));
        }
    }
}