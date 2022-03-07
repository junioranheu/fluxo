using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class EstabelecimentoAvaliacaoRepository : IEstabelecimentoAvaliacaoRepository
    {
        public readonly Context _context;

        public EstabelecimentoAvaliacaoRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<EstabelecimentoAvaliacao>> GetTodos()
        {
            var estabelecimentosAvaliacoesBd = await _context.EstabelecimentosAvaliacoes.
                Include(e => e.Estabelecimentos).
                Include(u => u.Usuarios).
                OrderBy(n => n.EstabelecimentoAvaliacaoId).AsNoTracking().ToListAsync();

            return estabelecimentosAvaliacoesBd;
        }

        public async Task<double> GetAvaliacaoPorEstabelecimentoId(int id)
        {
            var estabelecimentosAvaliacoesBd = await _context.EstabelecimentosAvaliacoes.
                Include(e => e.Estabelecimentos).
                Include(u => u.Usuarios).
                Where(ei => ei.EstabelecimentoId == id).AsNoTracking().ToListAsync();

            double avaliacao = Convert.ToDouble(estabelecimentosAvaliacoesBd.Sum(a => a.Avaliacao)) / estabelecimentosAvaliacoesBd.Count();

            return avaliacao;
        }

        public async Task<int> PostCriar(EstabelecimentoAvaliacao estabelecimentoAvaliacao)
        {
            _context.Add(estabelecimentoAvaliacao);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<List<EstabelecimentoAvaliacao>> GetAvaliacoesPorEstabelecimentoId(int id)
        {
            var estabelecimentosAvaliacoesBd = await _context.EstabelecimentosAvaliacoes.
                Include(e => e.Estabelecimentos).
                Include(u => u.Usuarios).
                Where(ei => ei.EstabelecimentoId == id).OrderBy(a => a.EstabelecimentoAvaliacaoId).AsNoTracking().ToListAsync();

            return estabelecimentosAvaliacoesBd;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.Estabelecimentos.AnyAsync(ei => ei.EstabelecimentoId == id);
        }

        public async Task<List<EstabelecimentoAvaliacao>> GetAvaliacoesPorUsuarioId(int id)
        {
            var estabelecimentosAvaliacoesBd = await _context.EstabelecimentosAvaliacoes.
                Include(e => e.Estabelecimentos).
                Include(u => u.Usuarios).
                Where(ei => ei.UsuarioId == id).OrderBy(a => a.EstabelecimentoAvaliacaoId).AsNoTracking().ToListAsync();

            return estabelecimentosAvaliacoesBd;
        }
    }
}
