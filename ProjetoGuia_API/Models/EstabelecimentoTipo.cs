using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class EstabelecimentoTipo
    {
        [Key]
        public int EstabelecimentoTipoId { get; set; }
        public string Tipo { get; set; }

        // Fk (De lá pra cá);
        public int EstabelecimentoCategoriaId { get; set; }
        public EstabelecimentoCategoria? EstabelecimentoCategorias { get; set; }

        public string? Descricao { get; set; }
        public string? Thumbnail { get; set; }
        public string? Genero { get; set; }
        public int IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
