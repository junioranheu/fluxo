using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class PostTipoRepository : IPostTipoRepository
    {
        public readonly Context _context;

        public PostTipoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<PostTipo>> GetTodos()
        {
            var postsTiposBd = await _context.PostsTipos.
                OrderBy(t => t.Tipo).AsNoTracking().ToListAsync();

            return postsTiposBd;
        }

        public async Task<PostTipo> GetPorId(int id)
        {
            var postTipoBd = await _context.PostsTipos.
                Where(pti => pti.PostTipoId == id).AsNoTracking().FirstOrDefaultAsync();

            return postTipoBd;
        }
    }
}
