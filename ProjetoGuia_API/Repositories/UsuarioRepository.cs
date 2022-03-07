using Microsoft.EntityFrameworkCore;
using ProjetoGuia_API.Data;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;
using ProjetoGuia_Biblioteca;

namespace ProjetoGuia_API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public readonly Context _context;

        public UsuarioRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetTodos()
        {
            var usuariosBd = await _context.Usuarios.
                Include(ut => ut.UsuarioTipos).
                Include(ui => ui.UsuariosInformacoes).ThenInclude(c => c.Cidades).ThenInclude(e => e.Estados).
                OrderBy(ui => ui.UsuarioId).AsNoTracking().ToListAsync();

            return usuariosBd;
        }

        public async Task<Usuario> GetPorId(int id)
        {
            var usuarioBd = await _context.Usuarios.
                Include(ut => ut.UsuarioTipos).
                Include(ui => ui.UsuariosInformacoes).ThenInclude(c => c.Cidades).ThenInclude(e => e.Estados).
                Where(ui => ui.UsuarioId == id).AsNoTracking().FirstOrDefaultAsync();

            return usuarioBd;
        }

        public async Task<int> PostCriar(Usuario usuario)
        {
            // Hora atual;
            DateTime horaAgora = Biblioteca.HorarioBrasilia();
            string senhaCriptografada = Biblioteca.Criptografar(usuario.Senha);

            usuario.Senha = senhaCriptografada;
            usuario.DataCriacao = horaAgora;

            _context.Add(usuario);
            var isOk = await _context.SaveChangesAsync();

            return usuario.UsuarioId;
        }

        public async Task<int> PostAtualizar(Usuario usuario)
        {
            if (String.IsNullOrEmpty(usuario.Senha))
            {
                var usuarioAntigo = await _context.Usuarios.Where(ui => ui.UsuarioId == usuario.UsuarioId).AsNoTracking().FirstOrDefaultAsync();
                usuario.Senha = usuarioAntigo.Senha;
            }

            _context.Update(usuario);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<int> PostAtualizarHoraOnline(Usuario usuario)
        {
            var u = await _context.Usuarios.Where(ui => ui.UsuarioId == usuario.UsuarioId).AsNoTracking().FirstOrDefaultAsync();
            u.DataOnline = usuario.DataOnline;

            _context.Update(u);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<Usuario> GetVerificarEmailSenha(string nomeUsuarioSistema, string senha)
        {
            string senhaCriptografada = Biblioteca.Criptografar(senha);

            var usuarioBd = await _context.Usuarios.
                Include(ui => ui.UsuariosInformacoes).ThenInclude(c => c.Cidades).
                AsNoTracking().
                FirstOrDefaultAsync(l => (l.NomeUsuarioSistema == nomeUsuarioSistema || l.Email == nomeUsuarioSistema) && l.Senha == senhaCriptografada);

            return usuarioBd;
        }

        public async Task<bool> IsExistePorEmail(string email)
        {
            return await _context.Usuarios.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> IsExistePorNomeUsuarioSistema(string nomeUsuarioSistema)
        {
            return await _context.Usuarios.AnyAsync(nus => nus.NomeUsuarioSistema == nomeUsuarioSistema);
        }

        public async Task<Usuario> GetPorNomeUsuarioSistema(string nomeUsuarioSistema)
        {
            var usuarioBd = await _context.Usuarios.
                Include(ut => ut.UsuarioTipos).
                Include(ui => ui.UsuariosInformacoes).ThenInclude(c => c.Cidades).ThenInclude(e => e.Estados).
                Where(nus => nus.NomeUsuarioSistema == nomeUsuarioSistema).AsNoTracking().FirstOrDefaultAsync();

            return usuarioBd;
        }

        public async Task<int> PostAtualizarFotoPerfil(string usuarioid, string fotoPerfil)
        {
            var usuario = await _context.Usuarios.Where(ui => ui.UsuarioId == Convert.ToInt32(usuarioid)).AsNoTracking().FirstOrDefaultAsync();
            usuario.Foto = fotoPerfil;

            _context.Update(usuario);
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<string> GetEmailPorEmailOuNomeUsuario(string emailOuNomeUsuario)
        {
            var usuarioBd = await _context.Usuarios.
                Where(ui => (ui.Email == emailOuNomeUsuario || ui.NomeUsuarioSistema == emailOuNomeUsuario)).AsNoTracking().FirstOrDefaultAsync();

            string email = usuarioBd != null ? usuarioBd.Email : "";
            return email;
        }

        public async Task<int> PostAtualizarSenha(string email, string senha)
        {
            // Verificar se esse e-mail tem uma solicitação de alteração de senha válida;
            var isSolicitavaoValida = await _context.UrlsTemporarias.
                Where(c => c.ChaveDinamica == email && c.IsAtivo == 1).
                OrderByDescending(u => u.UrlTemporariaId).
                Take(1).
                AsNoTracking().FirstOrDefaultAsync();

            if (isSolicitavaoValida == null)
            {
                return 0;
            }

            // Encontrar o usuário pelo e-mail;
            var usuario = await _context.Usuarios.Where(e => e.Email == email).AsNoTracking().FirstOrDefaultAsync();

            if (usuario == null)
            {
                return 0;
            }

            // Criptografar senha;
            string senhaCriptografa = Biblioteca.Criptografar(senha);

            // Atualizar senha;
            usuario.Senha = senhaCriptografa;
            _context.Update(usuario);

            // Atualizar o status da url em questão para desativado;
            isSolicitavaoValida.IsAtivo = 0;
            _context.Update(isSolicitavaoValida);

            // Verificar se as alterações estão ok;
            var isOk = await _context.SaveChangesAsync();

            return isOk;
        }

        public async Task<bool> PostVerificarConta(string usuarioId)
        {
            // Encontrar o usuário pelo e-mail;
            var usuario = await _context.Usuarios.Where(u => u.UsuarioId == Convert.ToInt32(usuarioId)).AsNoTracking().FirstOrDefaultAsync();

            if (usuario == null)
            {
                return false;
            }

            // Verificar se esse usuario tem uma solicitação de verificar conta;
            var isSolicitavaoValida = await _context.UrlsTemporarias.
                Where(c => c.ChaveDinamica == usuario.Email && c.IsAtivo == 1).
                OrderByDescending(u => u.UrlTemporariaId).
                Take(1).
                AsNoTracking().FirstOrDefaultAsync();

            if (isSolicitavaoValida == null)
            {
                return false;
            }

            // Alterar status;
            usuario.IsVerificado = 1;
            _context.Update(usuario);

            // Atualizar o status da url em questão para desativado;
            isSolicitavaoValida.IsAtivo = 0;
            _context.Update(isSolicitavaoValida);

            // Verificar se as alterações estão ok;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
