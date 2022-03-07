using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IEstadoRepository
    {
        Task<List<Estado>> GetTodos();
        Task<Estado> GetPorId(int id);
    }
}
