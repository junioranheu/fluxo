using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;
using ProjetoGuia_API.Services;
using ProjetoGuia_Biblioteca;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosApiController : BaseController<UsuariosApiController>
    {
        private readonly IUsuarioRepository _usuarios;

        public UsuariosApiController(IUsuarioRepository usuarioRepository)
        {
            _usuarios = usuarioRepository;
        }

        [HttpGet("autenticar")]
        public async Task<ActionResult<string>> Autenticar(string nomeUsuarioSistema, string senha)
        {
            // Verificar se o usuário existe;
            string caminho = String.Format("/api/UsuariosApi/verificarEmailSenha?nomeUsuarioSistema={0}&senha={1}", nomeUsuarioSistema, senha);
            var resultado = await GetAPI(caminho, null);
            Usuario? usu = JsonConvert.DeserializeObject<Usuario>(resultado);

            // Verifica se o usuário existe;
            if (usu == null)
            {
                return NotFound("Usuário ou senha inválidos");
            }

            // Gera o Token;
            var token = TokenService.ServicoGerarToken(usu.UsuarioId, usu.NomeUsuarioSistema, usu.UsuarioTipoId);

            return token;
        }

        [HttpGet("todos")]
        [Authorize]
        public async Task<ActionResult<List<Usuario>>> GetTodos()
        {
            var todos = await _usuarios.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                item.Senha = "";
            }

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetPorId(int id)
        {
            var usuarioBd = await _usuarios.GetPorId(id);

            if (usuarioBd == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            usuarioBd.Senha = "";

            return usuarioBd;
        }

        [HttpPost("criar")]
        public async Task<ActionResult<int>> PostCriar(Usuario usuario)
        {
            var idUsuarioCriado = await _usuarios.PostCriar(usuario);

            if (idUsuarioCriado < 1)
            {
                return NotFound();
            }

            return idUsuarioCriado;
        }

        [HttpPost("atualizar")]
        [Authorize]
        public async Task<ActionResult<bool>> PostAtualizar(Usuario usuario)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(usuario.UsuarioId);

            if (isMesmoUsuario)
            {
                var isOk = await _usuarios.PostAtualizar(usuario);

                if (isOk < 1)
                {
                    return NotFound();
                }

                return true;
            }

            return Unauthorized();
        }

        [HttpPost("atualizarHoraOnline")]
        [Authorize]
        public async Task<ActionResult<bool>> PostAtualizarHoraOnline(Usuario usuario)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(usuario.UsuarioId);

            if (isMesmoUsuario)
            {
                var isOk = await _usuarios.PostAtualizarHoraOnline(usuario);

                if (isOk < 1)
                {
                    return NotFound();
                }

                return true;
            }

            return Unauthorized();
        }

        [HttpGet("verificarEmailSenha")]
        public async Task<ActionResult<Usuario>> GetVerificarEmailSenha(string nomeUsuarioSistema, string senha)
        {
            var usuarioBd = await _usuarios.GetVerificarEmailSenha(nomeUsuarioSistema, senha);

            if (usuarioBd != null)
            {
                int cidadeId = usuarioBd.UsuariosInformacoes == null ? 0 : usuarioBd.UsuariosInformacoes.CidadeId;
                string cidade = usuarioBd.UsuariosInformacoes == null ? "" : usuarioBd.UsuariosInformacoes.Cidades.Nome;
                Cidade c = new()
                {
                    Nome = cidade
                };

                UsuarioInformacao ui = new()
                {
                    CidadeId = cidadeId,
                    Cidades = c
                };

                Usuario usu = new()
                {
                    UsuarioId = usuarioBd.UsuarioId,
                    NomeCompleto = usuarioBd.NomeCompleto,
                    NomeUsuarioSistema = usuarioBd.NomeUsuarioSistema,
                    Email = usuarioBd.Email,
                    UsuarioTipoId = usuarioBd.UsuarioTipoId,
                    Foto = usuarioBd.Foto,
                    DataOnline = usuarioBd.DataOnline,
                    UsuariosInformacoes = ui
                };

                return usu;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("isExistePorEmail")]
        public async Task<ActionResult<bool>> IsExistePorEmail(string email)
        {
            bool isExiste = await _usuarios.IsExistePorEmail(email);
            return isExiste;
        }

        [HttpGet("isExistePorNomeUsuarioSistema")]
        public async Task<ActionResult<bool>> IsExistePorNomeUsuarioSistema(string nomeUsuarioSistema)
        {
            bool isExiste = await _usuarios.IsExistePorNomeUsuarioSistema(nomeUsuarioSistema);
            return isExiste;
        }

        [HttpGet("getPorNomeUsuarioSistema")]
        public async Task<ActionResult<Usuario>> GetPorNomeUsuarioSistema(string nomeUsuarioSistema)
        {
            var usuarioBd = await _usuarios.GetPorNomeUsuarioSistema(nomeUsuarioSistema);

            if (usuarioBd == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            usuarioBd.Senha = "";

            return usuarioBd;
        }

        // Código copiado de Biblioteca.cs para ser usado no React;
        [HttpGet("criptografarSenha")]
        public string CriptografarSenha(string senha)
        {
            string senhaCriptografada = Biblioteca.Criptografar(senha);
            return senhaCriptografada;
        }

        // Código baseado em AtualizarPerfil() em UsuariosController e UparImagem() em BaseController para ser usado no React;
        // Tutorial completo para usar em React:
        // https://sankhadip.medium.com/how-to-upload-files-in-net-core-web-api-and-react-36a8fbf5c9e8
        // https://thewebdev.info/2021/11/07/how-to-read-and-upload-a-file-in-react-using-custom-button/
        [HttpPost("uparArquivoReact")]
        [Authorize]
        public async Task<bool> UparArquivoReact([FromForm] string formPasta, [FromForm] string formId, IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                string webRootPath = Directory.GetCurrentDirectory() + "/Upload/" + formPasta + "/";
                string nomeNovo = formPasta + "-" + formId + System.IO.Path.GetExtension(formFile.FileName); // Nome novo do arquivo completo;
                string nomeSemExtensao = formPasta + "-" + formId; // Nome novo do arquivo sem extensão;
                string caminhoDestino = webRootPath + nomeNovo; // Caminho de destino para upar;

                // Verificar se já existe uma foto caso exista, delete-a;
                DirectoryInfo root = new(webRootPath);
                FileInfo[] listfiles = root.GetFiles(nomeSemExtensao + ".*");
                if (listfiles.Length > 0)
                {
                    foreach (FileInfo file in listfiles)
                    {
                        System.IO.File.Delete(file.ToString());
                    }
                }

                // Salvar imagem na pasta Upload na API;
                using (var fs = new FileStream(caminhoDestino, FileMode.Create))
                {
                    await formFile.CopyToAsync(fs);
                }

                return true;
            }

            return false;
        }

        [HttpPost("atualizarFotoPerfil")]
        [Authorize]
        public async Task<ActionResult<bool>> PostAtualizarFotoPerfil(string usuarioId, string caminhoImagem)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(Convert.ToInt32(usuarioId));

            if (isMesmoUsuario)
            {
                var isOk = await _usuarios.PostAtualizarFotoPerfil(usuarioId, caminhoImagem);

                if (isOk < 1)
                {
                    return NotFound();
                }

                return true;
            }

            return Unauthorized();
        }

        [HttpGet("getEmailPorEmailOuNomeUsuario")]
        public async Task<ActionResult<string>> GetEmailPorEmailOuNomeUsuario(string emailOuNomeUsuario)
        {
            string email = await _usuarios.GetEmailPorEmailOuNomeUsuario(emailOuNomeUsuario);
            return email;
        }

        [HttpPost("enviarEmailRecuperacaoSenha")]
        public async Task<ActionResult<bool>> PostEnviarEmailRecuperacaoSenha(string email, string urlTemporaria)
        {
            string assunto = "Recuperação de senha";
            string conteudoEmailSemHtml = "";

            // Montar a url final;
            string dominio = Biblioteca.CaminhoReact();
            string urlAlterarSenha = (dominio + $"recuperar-senha/{urlTemporaria}");

            // Pegar o arquivo referente ao layout do e-mail de recuperação de senha;
            string conteudoEmailHtml = string.Empty;
            string root = Directory.GetCurrentDirectory() + "/Emails/";
            string arquivo = "RecuperarSenha.txt";
            string caminhoFinal = root + arquivo;
            using (var reader = new StreamReader(caminhoFinal))
            {
                // Ler arquivo;
                string readFile = reader.ReadToEnd();
                string strContent = readFile;

                // Remover tags desnecessárias;
                strContent = strContent.Replace("\r", string.Empty);
                strContent = strContent.Replace("\n", string.Empty);

                // Replaces;
                strContent = strContent.Replace("[Url]", urlAlterarSenha);
                conteudoEmailHtml = strContent.ToString();
            }

            bool resposta = await EnviarEmail(email, assunto, conteudoEmailSemHtml, conteudoEmailHtml);
            return resposta;
        }

        [HttpPost("atualizarSenha")]
        public async Task<ActionResult<bool>> PostAtualizarSenha(Usuario usuario)
        {
            var isOk = await _usuarios.PostAtualizarSenha(usuario.Email, usuario.Senha);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("enviarEmailBemVindo")]
        public async Task<ActionResult<bool>> PostEnviarEmailBemVindo(string email, string nomeUsuario, string urlTemporaria)
        {
            string assunto = "Bem-vindo ao Fluxo!";
            string conteudoEmailSemHtml = "";

            // Montar a url final;
            string dominio = Biblioteca.CaminhoReact();
            string urlAlterarSenha = (dominio + $"verificar-conta/{urlTemporaria}");

            // Pegar o arquivo referente ao layout do e-mail de recuperação de senha;
            string conteudoEmailHtml = string.Empty;
            string root = Directory.GetCurrentDirectory() + "/Emails/";
            string arquivo = "BemVindo.txt";
            string caminhoFinal = root + arquivo;
            using (var reader = new StreamReader(caminhoFinal))
            {
                // Ler arquivo;
                string readFile = reader.ReadToEnd();
                string strContent = readFile;

                // Remover tags desnecessárias;
                strContent = strContent.Replace("\r", string.Empty);
                strContent = strContent.Replace("\n", string.Empty);

                // Replaces;
                strContent = strContent.Replace("[Url]", urlAlterarSenha);
                strContent = strContent.Replace("[NomeUsuario]", nomeUsuario);
                conteudoEmailHtml = strContent.ToString();
            }

            bool resposta = await EnviarEmail(email, assunto, conteudoEmailSemHtml, conteudoEmailHtml);
            return resposta;
        }

        [HttpPost("verificarConta")]
        [Authorize]
        public async Task<ActionResult<bool>> PostVerificarConta(string usuarioId)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(Convert.ToInt32(usuarioId));

            if (isMesmoUsuario)
            {
                var isOk = await _usuarios.PostVerificarConta(usuarioId);

                if (!isOk)
                {
                    return NotFound();
                }

                return true;
            }

            return Unauthorized();
        }
    }
}
