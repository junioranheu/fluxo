using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class UrlTemporariaRepository : IUrlTemporariaRepository
    {
        public readonly Context _context;

        public UrlTemporariaRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<UrlTemporaria>> GetTodos()
        {
            var reportsBd = await _context.UrlsTemporarias.AsNoTracking().ToListAsync();

            return reportsBd;
        }

        public async Task<UrlTemporaria> GetPorId(int id)
        {
            var reportBd = await _context.UrlsTemporarias.
                Where(u => u.UrlTemporariaTipoId == id).AsNoTracking().FirstOrDefaultAsync();

            return reportBd;
        }

        public async Task<int> PostCriar(UrlTemporaria urlTemporaria)
        {
            _context.Add(urlTemporaria);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> GetTipoUrlId(string tipo)
        {
            var urlTemporariaTipo = await _context.UrlsTemporariasTipos.
                Where(t => t.Tipo == tipo).AsNoTracking().FirstOrDefaultAsync();

            if (urlTemporariaTipo == null)
            {
                return 0;
            }

            int tipoId = urlTemporariaTipo.UrlTemporariaTipoId;
            return tipoId;
        }

        public async Task<UrlTemporaria> GetPorTipoUrlEIdDinamica(int urlTipoId, string urlTemporaria)
        {
            var urlBd = await _context.UrlsTemporarias.
                Where(u => u.UrlTemporariaTipoId == urlTipoId && u.Url == urlTemporaria && u.IsAtivo == 1)
                .OrderByDescending(u => u.UrlTemporariaId)
                .Take(1)
                .AsNoTracking().FirstOrDefaultAsync();

            return urlBd;
        }
    }
}
