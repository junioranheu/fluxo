using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class CidadeRepository : ICidadeRepository
    {
        public readonly Context _context;

        public CidadeRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Cidade>> GetTodos()
        {
            var cidadesBd = await _context.Cidades.
                Include(e => e.Estados).
                OrderBy(t => t.Nome).AsNoTracking().ToListAsync();

            return cidadesBd;
        }

        public async Task<Cidade> GetPorId(int id)
        {
            var cidadeBd = await _context.Cidades.
                Include(e => e.Estados).
                Where(ei => ei.CidadeId == id).AsNoTracking().FirstOrDefaultAsync();

            return cidadeBd;
        }

        public async Task<Cidade> GetPorNomeMaisSiglaEstado(string nomeCidade, string siglaEstado)
        {
            var cidadeBd = await _context.Cidades.
                Include(e => e.Estados).
                Where(n => n.Nome == nomeCidade && n.Estados.Sigla == siglaEstado).AsNoTracking().FirstOrDefaultAsync();

            return cidadeBd;
        }
    }
}
