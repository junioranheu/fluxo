using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IEstabelecimentoCategoriaRepository
    {
        Task<List<EstabelecimentoCategoria>> GetTodos();
        Task<EstabelecimentoCategoria> GetPorId(int id);
        Task<int> PostCriar(EstabelecimentoCategoria entidade);
        Task<int> PostAtualizar(EstabelecimentoCategoria entidade);
        Task<int> PostDeletar(int id);
    }
}
