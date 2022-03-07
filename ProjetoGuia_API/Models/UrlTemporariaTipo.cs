using System.ComponentModel.DataAnnotations;

namespace ProjetoGuia_API.Models
{
    public class UrlTemporariaTipo
    {
        [Key]
        public int UrlTemporariaTipoId { get; set; }
        public string? Tipo { get; set; }
        public int IsAtivo { get; set; }
    }
}
