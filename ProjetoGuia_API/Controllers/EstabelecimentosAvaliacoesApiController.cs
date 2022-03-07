using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentosAvaliacoesApiController : ControllerBase
    {
        private readonly IEstabelecimentoAvaliacaoRepository _estabelecimentosAvaliacoes;

        public EstabelecimentosAvaliacoesApiController(IEstabelecimentoAvaliacaoRepository estabelecimentoAvaliacaoRepository)
        {
            _estabelecimentosAvaliacoes = estabelecimentoAvaliacaoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<EstabelecimentoAvaliacao>>> GetTodos()
        {
            var todos = await _estabelecimentosAvaliacoes.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                item.Usuarios.Senha = "";
            }

            return todos;
        }

        [HttpGet("getAvaliacaoPorEstabalecimentoId")]
        public async Task<ActionResult<double>> GetAvaliacaoPorEstabelecimentoId(int id)
        {
            double avaliacao = await _estabelecimentosAvaliacoes.GetAvaliacaoPorEstabelecimentoId(id);

            return avaliacao;
        }

        [HttpPost("criar")]
        [Authorize]
        public async Task<ActionResult<bool>> PostCriar(EstabelecimentoAvaliacao estabelecimentoAvaliacao)
        {
            var isOk = await _estabelecimentosAvaliacoes.PostCriar(estabelecimentoAvaliacao);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpGet("getAvaliacoesPorEstabelecimentoId")]
        public async Task<ActionResult<List<EstabelecimentoAvaliacao>>> GetAvaliacoesPorEstabelecimentoId(int id)
        {
            var avaliacoes = await _estabelecimentosAvaliacoes.GetAvaliacoesPorEstabelecimentoId(id);

            // Pegar apenas as 5 últimas avaliações;
            avaliacoes = avaliacoes.OrderByDescending(e => e.EstabelecimentoAvaliacaoId).Take(5).ToList();

            // Esconder alguns atributos;
            foreach (var item in avaliacoes)
            {
                item.Usuarios.Senha = "";
            }

            return avaliacoes;
        }

        [HttpGet("getAvaliacoesPorUsuarioId")]
        public async Task<ActionResult<List<EstabelecimentoAvaliacao>>> GetAvaliacoesPorUsuarioId(int id)
        {
            var avaliacoes = await _estabelecimentosAvaliacoes.GetAvaliacoesPorUsuarioId(id);

            // Pegar apenas as 10 últimas avaliações;
            avaliacoes = avaliacoes.OrderByDescending(e => e.EstabelecimentoAvaliacaoId).Take(10).ToList();

            // Esconder alguns atributos;
            foreach (var item in avaliacoes)
            {
                item.Usuarios.Senha = "";
            }

            return avaliacoes;
        }
    }
}
