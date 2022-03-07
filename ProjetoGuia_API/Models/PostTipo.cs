using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class PostTipo
    {
        [Key]
        public int PostTipoId { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public int IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
