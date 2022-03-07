using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class Estado
    {
        [Key]
        public int EstadoId { get; set; }
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public int IsAtivo { get; set; }
    }
}
