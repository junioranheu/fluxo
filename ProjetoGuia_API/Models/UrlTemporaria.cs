using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class UrlTemporaria
    {
        [Key]
        public int UrlTemporariaId { get; set; }
        public string? Url { get; set; }
        public string? ChaveDinamica { get; set; } // ChaveDinamica = id do usuário, nome do usuário, ou qualquer outro id, por exemplo;
        public DateTime DataGeracaoUrl { get; set; }
        public int IsAtivo { get; set; }

        // Fk (De lá pra cá);
        public int UrlTemporariaTipoId { get; set; }
        public UrlTemporariaTipo? UrlTemporariaTipos { get; set; }
    }
}
