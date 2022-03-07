using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjetoGuia_API.Controllers;
using ProjetoGuia_API.Models;
using ProjetoGuia_Biblioteca;
using System.Security.Claims;

namespace ProjetoGuia.Controllers
{
    public class UsuariosController : BaseController<UsuariosController>
    {
        private static ClaimsIdentity _identidade;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuariosController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> VerificarEmailSenha(string nomeUsuarioSistema, string senha)
        {
            // Verificar se o usuário existe;
            string caminho = String.Format("/api/UsuariosApi/verificarEmailSenha?nomeUsuarioSistema={0}&senha={1}", nomeUsuarioSistema, senha);
            var resultado = await GetAPI(caminho, null);
            Usuario? usuario = JsonConvert.DeserializeObject<Usuario>(resultado);

            bool isExiste = false;
            int idUsuario = 0;

            if (usuario != null)
            {
                isExiste = true;
                idUsuario = usuario.UsuarioId;
            }

            return Json(new { isExiste, idUsuario });
        }

        [HttpGet]
        public async Task<JsonResult> Login(string idUsuario, string senha)
        {
            DateTime horaAgora = Biblioteca.HorarioBrasilia();

            // Trazer todas as informações do usuário;
            string caminho = String.Format("/api/UsuariosApi/{0}", idUsuario);
            var resultado = await GetAPI(caminho, null);
            Usuario? usuario = JsonConvert.DeserializeObject<Usuario>(resultado);

            // Verificar se o usuário já está logado;
            // Se a diferença da última vez on-line da data atual for menor que 5 segundos, não deixe o usuário logar;
            if ((horaAgora - usuario.DataOnline).TotalSeconds < 5)
            {
                bool usuarioLogado = true;
                return Json(new { usuarioLogado });
            }

            // Verificar se o usuário está ativo;
            if (usuario.IsAtivo == 0)
            {
                bool isAtivo = false;
                return Json(new { isAtivo });
            }

            // Gerar token autenticado (JWT) e inseri-lo nos Cookies;
            string? token = await GerarToken(usuario.NomeUsuarioSistema, senha, null, Request, Response);

            // Cria a identidade pro usuário;
            _identidade = new(new[] {
                    new Claim(ClaimTypes.Name, usuario?.NomeCompleto), // Nome completo;
                    new Claim(ClaimTypes.Role, usuario?.UsuarioTipoId.ToString()), // Tipo de usuário id;
                    new Claim(ClaimTypes.NameIdentifier, usuario?.UsuarioId.ToString()), // ID;
                    new Claim(ClaimTypes.UserData, usuario?.NomeUsuarioSistema), // Nome de usuário;
                    new Claim(ClaimTypes.Thumbprint, usuario.Foto ?? ""), // Foto;
                    new Claim(ClaimTypes.Hash, senha) // Senha;
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            // Autenticar, de fato;
            var principal = new ClaimsPrincipal(_identidade);

            // Verificar se o usuário marcou o "Manter conectado";
            var props = new AuthenticationProperties
            {
                // IsPersistent = isManterConectado
                AllowRefresh = true,
                ExpiresUtc = horaAgora.AddDays(1),
                IssuedUtc = horaAgora
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).ConfigureAwait(true);

            return Json(new { });
        }

        [HttpGet]
        public async Task<JsonResult> Logout()
        {
            Response.Cookies.Delete("fluxo_jwt"); // Cookie do token JWT;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Cookie da sessão;

            string resposta = "Deslogado com sucesso";
            return Json(new { resposta });
        }

        [HttpGet]
        public JsonResult TrazerInformacoesUsuarioLogado()
        {
            Usuario usuario = new();
            usuario.UsuarioId = -1;
            usuario.NomeCompleto = "-1";
            usuario.UsuarioTipoId = -1;
            usuario.NomeUsuarioSistema = "-1";
            usuario.Foto = "-1";

            if (User.Identity.IsAuthenticated)
            {
                usuario.UsuarioId = Convert.ToInt32(User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value); // Id; 
                usuario.NomeCompleto = User.Identity.Name; // Nome completo;
                usuario.UsuarioTipoId = Convert.ToInt32(User.FindFirst(claim => claim.Type == ClaimTypes.Role)?.Value); // Tipo de usuário;
                usuario.NomeUsuarioSistema = User.FindFirst(claim => claim.Type == ClaimTypes.UserData)?.Value; // Nome de usuário;
                usuario.Foto = User.FindFirst(claim => claim.Type == ClaimTypes.Thumbprint)?.Value; // Foto de perfil;
            }

            return Json(new { usuario });
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> AtualizarDataOnline(string usuarioRequisitadoId)
        {
            string resposta = "", isOk = "0";

            // Atualizar data só se o usuário logado foi o requisitado;
            if (User.Identity.IsAuthenticated)
            {
                string usuarioLogadoId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (usuarioRequisitadoId == usuarioLogadoId)
                {
                    // Trazer todas as informações do usuário;
                    string caminho = String.Format("/api/UsuariosApi/{0}", usuarioLogadoId);
                    var resultado = await GetAPI(caminho, null);
                    Usuario? usuario = JsonConvert.DeserializeObject<Usuario>(resultado);

                    // Alterar a ultima data on-line;
                    string? token = await GerarToken(null, null, User, Request, Response);
                    usuario.DataOnline = Biblioteca.HorarioBrasilia();

                    resposta = await PostAPI("/api/UsuariosApi/atualizarHoraOnline/", usuario, token);
                    isOk = "1";
                }
                else
                {
                    resposta = "Usuário requisitado não é o mesmo do usuário logado";
                }
            }

            return Json(new { resposta, isOk });
        }

        [HttpGet]
        public async Task<JsonResult> ChecarEmailENomeUsuarioDisponivel(string email, string nomeUsuario, Usuario? usu)
        {
            string msg = "", erro = "";
            bool isDisponivel;

            // Verificar se o e-mail já está em uso;
            string caminho = String.Format("/api/UsuariosApi/IsExistePorEmail/?email={0}", email);
            bool isExiste = Convert.ToBoolean(await GetAPI(caminho, null));

            if (isExiste && (email != usu.Email))
            {
                isDisponivel = false;
                msg = "Parece que outra pessoa já está utilizando esse e-mail";
                erro = "1";
            }
            else
            {
                isDisponivel = true;
            }

            if (isDisponivel)
            {
                // Agora é a vez de verificar se o nome de usuário já está em uso;
                caminho = String.Format("/api/UsuariosApi/isExistePorNomeUsuarioSistema/?nomeUsuarioSistema={0}", nomeUsuario);
                isExiste = Convert.ToBoolean(await GetAPI(caminho, null));

                if (isExiste && (nomeUsuario != usu.NomeUsuarioSistema))
                {
                    isDisponivel = false;
                    msg = "Parece que outra pessoa já está utilizando esse nome de usuário";
                    erro = "2";
                }
                else
                {
                    isDisponivel = true;
                }
            }

            return Json(new { isDisponivel, msg, erro });
        }

        [HttpPost]
        public async Task<string> CriarConta(string nomeCompleto, string email, string nomeUsuario, string senha)
        {
            Usuario u = new()
            {
                NomeCompleto = nomeCompleto,
                Email = email.ToLowerInvariant(),
                NomeUsuarioSistema = nomeUsuario,
                Senha = senha,
                UsuarioTipoId = 2, // Usuário comum;      
                DataCriacao = Biblioteca.HorarioBrasilia(),
                Foto = "",
                IsAtivo = 1,
                IsPremium = 0
            };

            var idUsuarioCriado = await PostAPI("/api/UsuariosApi/criar/", u, null);
            return idUsuarioCriado;

        }

        [Route("perfil/@{nomeUsuarioSistema}")]
        public async Task<IActionResult> Perfil(string nomeUsuarioSistema)
        {
            if (nomeUsuarioSistema == null)
            {
                ViewBag.Erro = "Nome de usuário está nulo";
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            var resultado = await GetAPI(String.Format("/api/UsuariosApi/getPorNomeUsuarioSistema?nomeUsuarioSistema={0}", nomeUsuarioSistema), null);
            var usuarioBd = JsonConvert.DeserializeObject<Usuario>(resultado);

            if (usuarioBd == null)
            {
                ViewBag.Erro = "Nenhuma informação foi encontrada buscando pelo usuário @" + nomeUsuarioSistema +
                      "<br/>— Esse usuário existe?" +
                      "<br/>— Você escreveu o nome corretamente?";

                return View("~/Views/Home/SemAcesso.cshtml");
            }

            return View(usuarioBd);
        }

        [Route("perfil/atualizar")]
        [Authorize]
        public async Task<IActionResult> AtualizarPerfil()
        {
            string usuarioLogadoId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var resultado = await GetAPI(String.Format("/api/UsuariosApi/{0}", usuarioLogadoId), null);
            var usuarioBd = JsonConvert.DeserializeObject<Usuario>(resultado);

            return View(usuarioBd);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AtualizarPerfil(
            string cidadeNome, string estadoSigla,
            string nomeCompleto, string email,
            string nomeUsuarioSistema, string senha,
            string CPF, string telefone,
            string dataAniversario, string genero,
            string CEP, string numeroResidencia,
            string rua, string bairro,
            IFormFile arquivoFotoPerfil)
        {
            string resultado = "";

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Buscar as informações do usuário logado;
                    string usuarioLogadoId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                    resultado = await GetAPI(String.Format("/api/UsuariosApi/{0}", usuarioLogadoId), null);
                    Usuario? usu = JsonConvert.DeserializeObject<Usuario>(resultado);

                    // Verificar se o e-mail ou nome de usuário já está sendo utilizado em outra pessoa;
                    if (usu.Email.ToLowerInvariant() != email.ToLowerInvariant() || usu.NomeUsuarioSistema.ToLowerInvariant() != nomeUsuarioSistema.ToLowerInvariant())
                    {
                        var resultadoChecarDisponivel = await ChecarEmailENomeUsuarioDisponivel(email, nomeUsuarioSistema, usu);
                        string resultadoChecarDisponivelString = JsonConvert.SerializeObject(resultadoChecarDisponivel.Value);
                        dynamic resultadoChecarDisponivelDinamic = JObject.Parse(resultadoChecarDisponivelString);

                        bool isDisponivel = resultadoChecarDisponivelDinamic.isDisponivel;
                        string msg = resultadoChecarDisponivelDinamic.msg;

                        if (!isDisponivel)
                        {
                            resultado = msg;
                            return Json(new { resultado });
                        }
                    }

                    // Caso as outras informações (UsuariosInformacoes), cidade e estado estejam nulos, instâncie um novo; 
                    if (usu.UsuariosInformacoes == null)
                    {
                        usu.UsuariosInformacoes = new UsuarioInformacao();
                    }

                    usu.UsuariosInformacoes.Cidades = new Cidade
                    {
                        Estados = new Estado()
                    };

                    // Pegar o ID da cidade e do estado;
                    string nomeCidade = cidadeNome;
                    string siglaEstado = estadoSigla;
                    resultado = await GetAPI(String.Format("/api/CidadesApi/getPorNomeMaisSiglaEstado?nomeCidade={0}&siglaEstado={1}", nomeCidade, siglaEstado), null);
                    Cidade? cidade = JsonConvert.DeserializeObject<Cidade>(resultado);

                    // Preencher com as novas informações;
                    usu.NomeCompleto = nomeCompleto;
                    usu.Email = email;
                    usu.NomeUsuarioSistema = nomeUsuarioSistema;

                    if (!String.IsNullOrEmpty(senha))
                    {
                        usu.Senha = Biblioteca.Criptografar(senha);
                    }

                    usu.UsuariosInformacoes.CPF = CPF;
                    usu.UsuariosInformacoes.Telefone = telefone;
                    usu.UsuariosInformacoes.DataAniversario = Convert.ToDateTime(dataAniversario);
                    usu.UsuariosInformacoes.Genero = Convert.ToInt32(genero);
                    usu.UsuariosInformacoes.CEP = CEP;
                    usu.UsuariosInformacoes.NumeroResidencia = numeroResidencia;
                    usu.UsuariosInformacoes.Rua = rua;
                    usu.UsuariosInformacoes.Bairro = bairro;
                    usu.UsuariosInformacoes.Cidades = cidade;
                    usu.UsuariosInformacoes.DataUltimaAlteracao = Biblioteca.HorarioBrasilia();

                    // Upar a foto de perfil;
                    if (arquivoFotoPerfil != null)
                    {
                        string caminhoDestino = UparImagem(arquivoFotoPerfil, usu.UsuarioId, "usuario", usu.Foto, _webHostEnvironment);
                        usu.Foto = caminhoDestino;
                    }

                    // Salvar alterações;
                    string? token = await GerarToken(null, null, User, Request, Response);
                    resultado = await PostAPI("/api/UsuariosApi/atualizar/", usu, token);

                    // Para renovar a identidade que podem ter sido alteradas;
                    if (_identidade != null)
                    {
                        // Nome de usuário (Identificação/URL);
                        _identidade.RemoveClaim(_identidade.FindFirst(ClaimTypes.UserData));
                        _identidade.AddClaim(new Claim(ClaimTypes.UserData, usu.NomeUsuarioSistema));

                        // Senha;
                        if (!String.IsNullOrEmpty(senha))
                        {
                            _identidade.RemoveClaim(_identidade.FindFirst(ClaimTypes.Hash));
                            _identidade.AddClaim(new Claim(ClaimTypes.Hash, senha));
                        }

                        // Foto de perfil;
                        string fotoPerfilClaims = "";
                        if (!String.IsNullOrEmpty(usu.Foto))
                        {
                            fotoPerfilClaims = usu.Foto;
                        }

                        _identidade.RemoveClaim(_identidade.FindFirst(ClaimTypes.Thumbprint));
                        _identidade.AddClaim(new Claim(ClaimTypes.Thumbprint, fotoPerfilClaims));

                        var props = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = false
                        };

                        // Sair e entrar, para renovar as informações;
                        var principal = new ClaimsPrincipal(UsuariosController._identidade);
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).ConfigureAwait(true);
                    }

                    return Json(new { resultado });
                }
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
            }

            return Json(new { resultado });
        }

        [HttpPost]
        public async Task<string> Reportar(string report)
        {
            Report r = new()
            {
                Reclamacao = report,
                Data = Biblioteca.HorarioBrasilia(),
                UsuarioId = null
            };

            // Ajustar o idUsuario, caso esteja deslogado;
            if (User.Identity.IsAuthenticated)
            {
                string usuarioLogadoId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                r.UsuarioId = Convert.ToInt32(usuarioLogadoId);
            }

            var resultado = await PostAPI("/api/ReportsApi/criar/", r, null);
            return resultado;
        }
    }
}
