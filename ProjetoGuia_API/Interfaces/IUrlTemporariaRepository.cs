using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IUrlTemporariaRepository
    {
        Task<List<UrlTemporaria>> GetTodos();
        Task<UrlTemporaria> GetPorId(int id);
        Task<int> PostCriar(UrlTemporaria urlTemporaria);
        Task<UrlTemporaria> GetPorTipoUrlEIdDinamica(int urlTipoId, string urlTemporaria);
        Task<int> GetTipoUrlId(string tipo);
    }
}
