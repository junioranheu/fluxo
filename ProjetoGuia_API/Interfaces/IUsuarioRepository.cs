using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetTodos();
        Task<Usuario> GetPorId(int id);
        Task<int> PostCriar(Usuario usuario);
        Task<int> PostAtualizar(Usuario usuario);
        Task<int> PostAtualizarHoraOnline(Usuario usuario);
        Task<Usuario> GetVerificarEmailSenha(string nomeUsuarioSistema, string senha);
        Task<bool> IsExistePorEmail(string email);
        Task<bool> IsExistePorNomeUsuarioSistema(string nomeUsuarioSistema);
        Task<Usuario> GetPorNomeUsuarioSistema(string nomeUsuarioSistema);
        Task<int> PostAtualizarFotoPerfil(string usuarioid, string fotoPerfil);
        Task<string> GetEmailPorEmailOuNomeUsuario(string emailOuNomeUsuario);
        Task<int> PostAtualizarSenha(string email, string senha);
        Task<bool> PostVerificarConta(string usuarioid);
    }
}
