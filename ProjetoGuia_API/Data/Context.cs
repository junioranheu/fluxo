using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            //
        }

        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTipo> UsuariosTipos { get; set; }
        public DbSet<UsuarioInformacao> UsuariosInformacoes { get; set; }
        public DbSet<EstabelecimentoCategoria> EstabelecimentosCategorias { get; set; }
        public DbSet<EstabelecimentoTipo> EstabelecimentosTipos { get; set; }
        public DbSet<Estabelecimento> Estabelecimentos { get; set; }
        public DbSet<EstabelecimentoAvaliacao> EstabelecimentosAvaliacoes { get; set; }
        public DbSet<PostTipo> PostsTipos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UrlTemporariaTipo> UrlsTemporariasTipos { get; set; }
        public DbSet<UrlTemporaria> UrlsTemporarias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
