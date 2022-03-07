using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetTodos();
        Task<Post> GetPorId(int id);
        Task<int> PostCriar(Post post);
        Task<int> PostAtualizar(Post post);
        Task<int> PostDeletar(int id);
        Task<List<Post>> GetTodosPorUsuarioId(int usuarioId, int tipoPostId);
        Task<int> PostAtualizarImagemPost(string postId, string fotoPost);
    }
}
