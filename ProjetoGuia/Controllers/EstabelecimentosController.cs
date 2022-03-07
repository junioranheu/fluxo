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
    public class EstabelecimentosController : BaseController<EstabelecimentosController>
    {
        [Route("estabelecimento/tipo/{estabelecimentoTipoId}")]
        public async Task<IActionResult> Estabelecimentos(string estabelecimentoTipoId)
        {
            var resultado = (dynamic)null;
            int cidadeIdUsuarioLogado = 0;
            string cidadeNomeUsuarioLogado = null;

            if (estabelecimentoTipoId == null)
            {
                ViewBag.Erro = "O id do tipo de estabelecimento está nulo";
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            if (User.Identity.IsAuthenticated)
            {
                string usuarioLogadoId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                resultado = await GetAPI(String.Format("/api/UsuariosApi/{0}", usuarioLogadoId), null);
                Usuario? usu = JsonConvert.DeserializeObject<Usuario>(resultado);

                if (usu.UsuariosInformacoes != null)
                {
                    cidadeIdUsuarioLogado = usu.UsuariosInformacoes.CidadeId;
                    cidadeNomeUsuarioLogado = usu.UsuariosInformacoes.Cidades.Nome;
                    ViewData["CidadeNomeUsuarioLogado"] = cidadeNomeUsuarioLogado;
                }
            }

            // Buscar pelos estabelecimentos com base no tipo de estabelecimento (estabelecimentoTipoId);
            string caminho = String.Format("/api/EstabelecimentosApi/getPorEstabelecimentoTipoIdMaisCidadeIdUsuarioLogado?id={0}&cidadeIdUsuarioLogado={1}", estabelecimentoTipoId, cidadeIdUsuarioLogado);
            resultado = await GetAPI(caminho, null);
            var estabelecimentoBd = JsonConvert.DeserializeObject<List<Estabelecimento>>(resultado);

            if (estabelecimentoBd == null)
            {
                ViewBag.Erro = "Nenhuma informação foi encontrada buscando pelo tipo de estabelecimento #" + estabelecimentoTipoId +
                      "<br/>— Esse tipo de estabelecimento realmente existe?";

                return View("~/Views/Home/SemAcesso.cshtml");
            }

            if (estabelecimentoBd.Count < 1)
            {
                string msg = "Parece que nenhuma loja se encaixa nesse tipo de estabelecimento ainda";
                if (!String.IsNullOrEmpty(cidadeNomeUsuarioLogado))
                {
                    msg = "Parece que nenhuma loja se encaixa nesse tipo de estabelecimento em " + cidadeNomeUsuarioLogado + " ainda";
                }

                ViewBag.Erro = msg;
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            return View(estabelecimentoBd);
        }

        [Route("estabelecimento/{estabelecimentoId}")]
        public async Task<IActionResult> Estabelecimento(string estabelecimentoId)
        {
            if (estabelecimentoId == null)
            {
                ViewBag.Erro = "O id do estabelecimento está nulo";
                return View("~/Views/Home/SemAcesso.cshtml");
            }

            var resultado = await GetAPI(String.Format("/api/EstabelecimentosApi/{0}", estabelecimentoId), null);
            var estabelecimentoBd = JsonConvert.DeserializeObject<Estabelecimento>(resultado);

            if (estabelecimentoBd == null)
            {
                ViewBag.Erro = "Nenhuma informação foi encontrada buscando pelo estabelecimento #" + estabelecimentoId +
                      "<br/>— Esse estabelecimento realmente existe?";

                return View("~/Views/Home/SemAcesso.cshtml");
            }

            // Posts;
            int usuarioDonoEstabelecimentoId = estabelecimentoBd.UsuarioId;
            int tipoPostId = 2; // 2 = Estabelecimento;
            var caminho = String.Format("/api/PostsApi/getTodosPorUsuarioIdTipoPostId?usuarioId={0}&tipoPostId={1}", usuarioDonoEstabelecimentoId, tipoPostId);
            var resultado2 = await GetAPI(caminho, null);
            List<Post>? posts = JsonConvert.DeserializeObject<List<Post>>(resultado2);
            ViewData["PostsBd"] = posts;

            return View(estabelecimentoBd);
        }

        [HttpPost]
        public IActionResult ModalPost()
        {
            return PartialView("ModalPost");
        }

        [HttpPost]
        [Authorize]
        public async Task<string> AvaliarEstabelecimento(EstabelecimentoAvaliacao estabelecimentoAvaliacao)
        {
            int usuarioLogadoId = Convert.ToInt32(User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

            EstabelecimentoAvaliacao e = new()
            {
                EstabelecimentoId = estabelecimentoAvaliacao.EstabelecimentoId,
                UsuarioId = usuarioLogadoId,
                Comentario = estabelecimentoAvaliacao.Comentario,
                Avaliacao = estabelecimentoAvaliacao.Avaliacao,
                Data = Biblioteca.HorarioBrasilia()
            };

            string? token = await GerarToken(null, null, User, Request, Response);
            var resultado = await PostAPI("/api/EstabelecimentosAvaliacoesApi/criar/", e, token);
            return resultado;
        }

        public async Task<JsonResult> EstabelecimentoAvaliacoes(string estabelecimentoId)
        {
            // Avaliações;
            string caminho = String.Format("/api/EstabelecimentosAvaliacoesApi/getAvaliacoesPorEstabelecimentoId?id={0}", estabelecimentoId);
            var resultado2 = await GetAPI(caminho, null);
            var avaliacoes = JsonConvert.DeserializeObject<List<EstabelecimentoAvaliacao>>(resultado2);


            return Json(new { avaliacoes });
        }
    }
}
