using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IEstabelecimentoAvaliacaoRepository
    {
        Task<List<EstabelecimentoAvaliacao>> GetTodos();
        Task<double> GetAvaliacaoPorEstabelecimentoId(int id);
        Task<int> PostCriar(EstabelecimentoAvaliacao estabelecimentoAvaliacao);
        Task<List<EstabelecimentoAvaliacao>> GetAvaliacoesPorEstabelecimentoId(int id);
        Task<List<EstabelecimentoAvaliacao>> GetAvaliacoesPorUsuarioId(int id);
    }
}
