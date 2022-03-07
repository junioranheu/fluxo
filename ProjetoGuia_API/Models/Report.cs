using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public string Reclamacao { get; set; }

        // Fk (De lá pra cá);
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime Data { get; set; }
    }
}
