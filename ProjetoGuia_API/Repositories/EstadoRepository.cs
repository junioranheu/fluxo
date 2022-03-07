using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        public readonly Context _context;

        public EstadoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Estado>> GetTodos()
        {
            var estadosBd = await _context.Estados.
                OrderBy(t => t.Nome).AsNoTracking().ToListAsync();

            return estadosBd;
        }

        public async Task<Estado> GetPorId(int id)
        {
            var estadoBd = await _context.Estados.
                Where(ei => ei.EstadoId == id).AsNoTracking().FirstOrDefaultAsync();

            return estadoBd;
        }
    }
}
