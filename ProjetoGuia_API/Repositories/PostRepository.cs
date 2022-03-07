using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Repositories
{
    public class PostRepository : IPostRepository
    {
        public readonly Context _context;

        public PostRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetTodos()
        {
            var postsBd = await _context.Posts.
                Include(pt => pt.PostsTipos).
                Include(u => u.Usuarios).
                OrderBy(pi => pi.PostId).AsNoTracking().ToListAsync();

            return postsBd;
        }

        public async Task<Post> GetPorId(int id)
        {
            var postBd = await _context.Posts.
                Include(pt => pt.PostsTipos).
                Include(u => u.Usuarios).
                Where(pi => pi.PostId == id).AsNoTracking().FirstOrDefaultAsync();

            return postBd;
        }

        public async Task<int> PostCriar(Post post)
        {
            _context.Add(post);
            var isOk = await _context.SaveChangesAsync();

            return post.PostId;
        }

        public async Task<int> PostAtualizar(Post post)
        {
            int isOk;

            try
            {
                _context.Update(post);
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
            var dados = await _context.Posts.FindAsync(id);

            if (dados == null)
            {
                throw new Exception("Registro com o id " + id + " não foi encontrado");
            }

            _context.Posts.Remove(dados);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        private async Task<bool> IsExiste(int id)
        {
            return await _context.Posts.AnyAsync(pi => pi.PostId == id);
        }

        public async Task<List<Post>> GetTodosPorUsuarioId(int usuarioId, int tipoPostId)
        {
            var postsBd = await _context.Posts.
                Include(u => u.Usuarios).
                Where(u => u.UsuarioId == usuarioId && u.PostTipoId == tipoPostId).AsNoTracking().ToListAsync();

            return postsBd;
        }
        public async Task<int> PostAtualizarImagemPost(string postId, string fotoPost)
        {
            var post = await _context.Posts.Where(p => p.PostId == Convert.ToInt32(postId)).FirstOrDefaultAsync();
            post.Midia = fotoPost;

            _context.Update(post);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }
    }
}
