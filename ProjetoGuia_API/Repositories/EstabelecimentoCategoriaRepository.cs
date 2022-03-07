using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class EstabelecimentoCategoriaRepository : IEstabelecimentoCategoriaRepository
    {
        public readonly Context _context;

        public EstabelecimentoCategoriaRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<EstabelecimentoCategoria>> GetTodos()
        {
            var estabelecimentosCategoriasBd = await _context.EstabelecimentosCategorias.
                OrderBy(c => c.Categoria).AsNoTracking().ToListAsync();

            return estabelecimentosCategoriasBd;
        }

        public async Task<EstabelecimentoCategoria> GetPorId(int id)
        {
            var estabelecimentoCategoriaBd = await _context.EstabelecimentosCategorias.
                Where(eci => eci.EstabelecimentoCategoriaId == id).AsNoTracking().FirstOrDefaultAsync();

            return estabelecimentoCategoriaBd;
        }

        public async Task<int> PostCriar(EstabelecimentoCategoria estabelecimentoCategoria)
        {
            _context.Add(estabelecimentoCategoria);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> PostAtualizar(EstabelecimentoCategoria estabelecimentoCategoria)
        {
            int isOk;

            try
            {
                _context.Update(estabelecimentoCategoria);
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
            var dados = await _context.EstabelecimentosCategorias.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            // Verificar se esse dado já foi usado, se foi, não permite;
            var foiUsado = await _context.EstabelecimentosTipos.AnyAsync(et => et.EstabelecimentoCategoriaId == id);
            if (foiUsado)
            {
                throw new Exception("Esse registro não pode ser apagado pois já está sendo usado em outra tabela");
            }

            _context.EstabelecimentosCategorias.Remove(dados);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.EstabelecimentosCategorias.AnyAsync(ec => ec.EstabelecimentoCategoriaId == id);
        }
    }
}
