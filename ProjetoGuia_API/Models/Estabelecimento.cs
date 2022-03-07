using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class Estabelecimento 
    {
        [Key]
        public int EstabelecimentoId { get; set; }

        // Fk (De lá pra cá);
        public int EstabelecimentoTipoId { get; set; }
        public EstabelecimentoTipo? EstabelecimentoTipos { get; set; }

        // Fk (De lá pra cá);
        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; set; }

        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? Thumbnail { get; set; }

        public string? Rua { get; set; }
        public string? NumeroEndereco { get; set; }
        public string? CEP { get; set; }
        public string? Bairro { get; set; }

        // Fk (De lá pra cá);
        public int CidadeId { get; set; }
        public Cidade? Cidades { get; set; }

        public DateTime? DataCriacao { get; set; }
        public int IsAtivo { get; set; }
        public double? Avaliacao { get; set; }
    }
}
