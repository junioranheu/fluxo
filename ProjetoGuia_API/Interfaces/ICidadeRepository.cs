using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface ICidadeRepository
    {
        Task<List<Cidade>> GetTodos();
        Task<Cidade> GetPorId(int id);
        Task<Cidade> GetPorNomeMaisSiglaEstado(string nomeCidade, string siglaEstado);
    }
}
