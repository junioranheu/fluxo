using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        public readonly Context _context;

        public ReportRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetTodos()
        {
            var reportsBd = await _context.Reports.
                Include(u => u.Usuario).
                OrderBy(pi => pi.ReportId).AsNoTracking().ToListAsync();

            return reportsBd;
        }

        public async Task<Report> GetPorId(int id)
        {
            var reportBd = await _context.Reports.
                Include(u => u.Usuario).
                Where(pi => pi.ReportId == id).AsNoTracking().FirstOrDefaultAsync();

            return reportBd;
        }

        public async Task<int> PostCriar(Report post)
        {
            _context.Add(post);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }
    }
}
