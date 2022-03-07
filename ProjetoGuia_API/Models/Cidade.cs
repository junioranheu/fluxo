using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class Cidade
    {
        [Key]
        public int CidadeId { get; set; }
        public string? Nome { get; set; }

        // Fk (De lá pra cá);
        public int EstadoId { get; set; }
        public Estado? Estados { get; set; }

        public int IsAtivo { get; set; }
    }
}
