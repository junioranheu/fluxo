using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IPostTipoRepository
    {
        Task<List<PostTipo>> GetTodos();
        Task<PostTipo> GetPorId(int id);
    }
}
