using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IUsuarioTipoRepository
    {
        Task<List<UsuarioTipo>> GetTodos();
        Task<UsuarioTipo> GetPorId(int id);
    }
}
