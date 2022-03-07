using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoGuia_API.Controllers;
using ProjetoGuia_API.Models;

namespace ProjetoGuia.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("fluxo")]
        public async Task<IActionResult> LandingPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Inicio");
            }

            List<Usuario>? listaUsuarios = null as dynamic;

            if (User.Identity.IsAuthenticated)
            {
                string? token = await GerarToken(null, null, User, Request, Response);
                var resultado = await GetAPI("/api/UsuariosApi/todos", token);
                listaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(resultado);
            }

            return View(listaUsuarios);
        }

        public async Task<IActionResult> Inicio()
        {
            Usuario? usuario = null as dynamic;

            if (User.Identity.IsAuthenticated)
            {
                int usuarioId = Convert.ToInt32(User.FindFirst(claim => claim.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value); // Id; 
                var resultado = await GetAPI(String.Format("/api/UsuariosApi/{0}", usuarioId), null);
                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);

                // Se o usuário não tiver preenchido o perfil, redirecione-o para a tela;
                if (usuario.UsuariosInformacoes == null)
                {
                    TempData["AvisoDadosFaltantes"] = "Preencha seus dados antes de sair por aí usando o sistema 😎";
                    return RedirectToAction("AtualizarPerfil", "Usuarios");
                }

                if (usuario.UsuarioTipoId == 3) // Estabelecimento;
                {
                    resultado = await GetAPI(String.Format("/api/EstabelecimentosApi/getPorQuery?id={0}&nome={1}&idUsuario={2}", 0, "", usuarioId), null);
                    Estabelecimento? estabelecimento = JsonConvert.DeserializeObject<List<Estabelecimento>>(resultado).FirstOrDefault();
                    ViewData["DadosEstabelecimentoUsuarioBd"] = estabelecimento;
                }
            }

            // Categorias dos tipos de estabelecimentos;
            var resultado2 = await GetAPI("/api/EstabelecimentosCategoriasApi/todos", null);
            List<EstabelecimentoCategoria>? estabelecimentosCategorias = JsonConvert.DeserializeObject<List<EstabelecimentoCategoria>>(resultado2);
            ViewData["EstabelecimentosCategoriasBd"] = estabelecimentosCategorias;

            // Tipos de estabelecimentos;
            resultado2 = await GetAPI("/api/EstabelecimentosTiposApi/todos", null);
            List<EstabelecimentoTipo>? estabelecimentosTipos = JsonConvert.DeserializeObject<List<EstabelecimentoTipo>>(resultado2);
            ViewData["EstabelecimentosTiposBd"] = estabelecimentosTipos;

            return View(usuario);
        }

        [Route("entrar")]
        public IActionResult Entrar()
        {
            // Se o usuário tiver logado, não permita ver a tela;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Erro = "Não é possível você entrar agora.<br/>Aparentemente você já está logado no sistema!";
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            return View();
        }

        [Route("sem-acesso")]
        public IActionResult SemAcesso()
        {
            return View();
        }

        [Route("politica-e-termos-de-uso")]
        public IActionResult TermosUso()
        {
            return View();
        }

        [Route("criar-conta")]
        public IActionResult CriarConta()
        {
            // Se o usuário tiver logado, não permita ver a tela;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Erro = "Não é possível registrar uma nova conta agora.<br/>Aparentemente você já está logado no sistema!";
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            return View();
        }

        [Route("reportar-problema")]
        public IActionResult ReportarProblema()
        {
            return View();
        }

        [Route("sobre")]
        public IActionResult Sobre()
        {
            return View();
        }
    }
}