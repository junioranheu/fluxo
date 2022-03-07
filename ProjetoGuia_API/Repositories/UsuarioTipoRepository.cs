using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class UsuarioTipoRepository : IUsuarioTipoRepository
    {
        public readonly Context _context;

        public UsuarioTipoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<UsuarioTipo>> GetTodos()
        {
            var usuariosTiposBd = await _context.UsuariosTipos.
                OrderBy(t => t.Tipo).AsNoTracking().ToListAsync();

            return usuariosTiposBd;
        }

        public async Task<UsuarioTipo> GetPorId(int id)
        {
            var usuarioTipoBd = await _context.UsuariosTipos.
                Where(uti => uti.UsuarioTipoId == id).AsNoTracking().FirstOrDefaultAsync();

            return usuarioTipoBd;
        }
    }
}
