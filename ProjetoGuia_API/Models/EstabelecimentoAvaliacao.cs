using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class EstabelecimentoAvaliacao
    {
        [Key]
        public int EstabelecimentoAvaliacaoId { get; set; }

        // Fk (De lá pra cá);
        public int EstabelecimentoId { get; set; }
        public Estabelecimento? Estabelecimentos { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; set; }

        public string? Comentario { get; set; }
        public double? Avaliacao { get; set; }
        public DateTime? Data { get; set; }
    }
}
