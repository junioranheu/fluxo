using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class EstabelecimentoTipoRepository : IEstabelecimentoTipoRepository
    {
        public readonly Context _context;

        public EstabelecimentoTipoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<EstabelecimentoTipo>> GetTodos()
        {
            var estabelecimentosTiposBd = await _context.EstabelecimentosTipos.
                Include(ec => ec.EstabelecimentoCategorias).
                // Include(c => c.Estabelecimentos).
                OrderBy(t => t.Tipo).AsNoTracking().ToListAsync();

            return estabelecimentosTiposBd;
        }

        public async Task<EstabelecimentoTipo> GetPorId(int id)
        {
            var estabelecimentoTipoBd = await _context.EstabelecimentosTipos.
                Include(ec => ec.EstabelecimentoCategorias).
                Where(eti => eti.EstabelecimentoTipoId == id).AsNoTracking().FirstOrDefaultAsync();

            return estabelecimentoTipoBd;
        }

        public async Task<int> PostCriar(EstabelecimentoTipo estabelecimentoTipoBd)
        {
            _context.Add(estabelecimentoTipoBd);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> PostAtualizar(EstabelecimentoTipo estabelecimentoTipoBd)
        {
            int isOk;

            try
            {
                _context.Update(estabelecimentoTipoBd);
                isOk = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return isOk;
        }

        public async Task<int> PostDeletar(int id)
        {
            var dados = await _context.EstabelecimentosTipos.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.EstabelecimentosTipos.Remove(dados);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.EstabelecimentosTipos.AnyAsync(et => et.EstabelecimentoTipoId == id);
        }
    }
}
