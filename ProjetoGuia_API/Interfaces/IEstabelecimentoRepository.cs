using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IEstabelecimentoRepository
    {
        Task<List<Estabelecimento>> GetTodos();
        Task<Estabelecimento> GetPorId(int id);
        Task<int> PostCriar(Estabelecimento estabelecimento);
        Task<int> PostAtualizar(Estabelecimento estabelecimento);
        Task<int> PostDeletar(int id);
        Task<List<Estabelecimento>> GetEstabelecimentosPorTipoCategoriaIdMaisSiglaEstadoUsuario(int id, int? cidadeIdUsuarioLogado);
        Task<List<Estabelecimento>> GetPorQuery(int id, string nome, int idUsuario);
    }
}
