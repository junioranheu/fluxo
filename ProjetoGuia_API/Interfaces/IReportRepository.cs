using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IReportRepository
    {
        Task<List<Report>> GetTodos();
        Task<Report> GetPorId(int id);
        Task<int> PostCriar(Report report);
    }
}
