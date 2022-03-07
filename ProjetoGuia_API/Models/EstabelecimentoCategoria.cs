using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class EstabelecimentoCategoria
    {
        [Key]
        public int EstabelecimentoCategoriaId { get; set; }
        public string Categoria { get; set; }
        public string Icone { get; set; }
        public int IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
