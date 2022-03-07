using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IEstabelecimentoTipoRepository
    {
        Task<List<EstabelecimentoTipo>> GetTodos();
        Task<EstabelecimentoTipo> GetPorId(int id);
        Task<int> PostCriar(EstabelecimentoTipo estabelecimentoTipo);
        Task<int> PostAtualizar(EstabelecimentoTipo estabelecimentoTipo);
        Task<int> PostDeletar(int id);
    }
}
