using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class EstabelecimentoRepository : IEstabelecimentoRepository
    {
        public readonly Context _context;

        public EstabelecimentoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Estabelecimento>> GetTodos()
        {
            var estabelecimentosBd = await _context.Estabelecimentos.
                Include(et => et.EstabelecimentoTipos).
                Include(u => u.Usuarios).
                Include(c => c.Cidades).ThenInclude(e => e.Estados).
                OrderBy(n => n.Nome).AsNoTracking().ToListAsync();

            return estabelecimentosBd;
        }

        public async Task<Estabelecimento> GetPorId(int id)
        {
            var estabelecimentoBd = await _context.Estabelecimentos.
                Include(e => e.EstabelecimentoTipos).
                Include(u => u.Usuarios).
                Include(c => c.Cidades).ThenInclude(e => e.Estados).
                Where(ei => ei.EstabelecimentoId == id).AsNoTracking().FirstOrDefaultAsync();

            return estabelecimentoBd;
        }

        public async Task<int> PostCriar(Estabelecimento estabelecimento)
        {
            _context.Add(estabelecimento);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> PostAtualizar(Estabelecimento estabelecimento)
        {
            int isOk;

            try
            {
                _context.Update(estabelecimento);
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
            var dados = await _context.Estabelecimentos.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.Estabelecimentos.Remove(dados);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.Estabelecimentos.AnyAsync(ei => ei.EstabelecimentoId == id);
        }

        public async Task<List<Estabelecimento>> GetEstabelecimentosPorTipoCategoriaIdMaisSiglaEstadoUsuario(int id, int? cidadeIdUsuarioLogado)
        {
            var query = _context.Estabelecimentos.
                Include(et => et.EstabelecimentoTipos).
                Include(ec => ec.EstabelecimentoTipos.EstabelecimentoCategorias).
                Include(u => u.Usuarios).
                Include(c => c.Cidades).ThenInclude(e => e.Estados).
                OrderBy(n => n.Nome).AsNoTracking().AsQueryable();

            if (id > 0)
            {
                query = query.Where(e => e.EstabelecimentoTipoId == id);
            }

            if (cidadeIdUsuarioLogado > 0)
            {
                query = query.Where(c => c.CidadeId == cidadeIdUsuarioLogado);
            }

            var estabelecimentosBd = await query.ToListAsync();

            return estabelecimentosBd;
        }

        public async Task<List<Estabelecimento>> GetPorQuery(int id, string nome, int idUsuario)
        {
            var query = _context.Estabelecimentos.
                Include(et => et.EstabelecimentoTipos).
                Include(u => u.Usuarios).
                Include(c => c.Cidades).ThenInclude(e => e.Estados).
                OrderBy(n => n.Nome).AsNoTracking().AsQueryable();

            if (id > 0)
            {
                query = query.Where(e => e.EstabelecimentoId == id);
            }

            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(n => n.Nome.Contains(nome));
            }

            if (idUsuario > 0)
            {
                query = query.Where(u => u.UsuarioId == idUsuario);
            }

            var estabelecimentosBd = await query.ToListAsync();

            return estabelecimentosBd;
        }
    }
}
