using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string Midia { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }
        public Usuario Usuarios { get; set; }

        // Fk (De lá pra cá);
        public int PostTipoId { get; set; }
        public PostTipo PostsTipos { get; set; }

        public int IsAtivo { get; set; }

        public DateTime DataPost { get; set; }
    }
}
