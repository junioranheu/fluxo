using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentosApiController : ControllerBase
    {
        private readonly IEstabelecimentoRepository _estabelecimentos;
        private readonly IEstabelecimentoAvaliacaoRepository _estabelecimentosAvaliacoes;

        public EstabelecimentosApiController(IEstabelecimentoRepository estabelecimentoRepository, IEstabelecimentoAvaliacaoRepository estabelecimentoAvaliacaoRepository)
        {
            _estabelecimentos = estabelecimentoRepository;
            _estabelecimentosAvaliacoes = estabelecimentoAvaliacaoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<Estabelecimento>>> GetTodos()
        {
            var todos = await _estabelecimentos.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                item.Usuarios.Senha = "";
            }

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Estabelecimento>> GetPorId(int id)
        {
            var porId = await _estabelecimentos.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            porId.Usuarios.Senha = "";

            // Resgatar a avaliação do estabelecimento;
            double avaliacao = await _estabelecimentosAvaliacoes.GetAvaliacaoPorEstabelecimentoId(porId.EstabelecimentoId);
            porId.Avaliacao = Math.Round(avaliacao, 1);

            return porId;
        }

        [HttpPost("criar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostCriar(Estabelecimento estabelecimento)
        {
            var isOk = await _estabelecimentos.PostCriar(estabelecimento);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("atualizar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostAtualizar(Estabelecimento estabelecimento)
        {
            var isOk = await _estabelecimentos.PostAtualizar(estabelecimento);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("deletar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<int>> PostDeletar(int id)
        {
            var isOk = await _estabelecimentos.PostDeletar(id);

            if (isOk < 1)
            {
                return NotFound();
            }

            return isOk;
        }

        [HttpGet("getPorEstabelecimentoTipoIdMaisCidadeIdUsuarioLogado")]
        public async Task<ActionResult<List<Estabelecimento>>> GetEstabelecimentosPorTipoCategoriaIdMaisSiglaEstadoUsuario(int id, int? cidadeIdUsuarioLogado)
        {
            var estabelecimentosBd = await _estabelecimentos.GetEstabelecimentosPorTipoCategoriaIdMaisSiglaEstadoUsuario(id, cidadeIdUsuarioLogado);

            if (estabelecimentosBd == null)
            {
                return NotFound();
            }

            // Resgatar a avaliação do estabelecimento;
            foreach (var e in estabelecimentosBd)
            {
                double avaliacao = await _estabelecimentosAvaliacoes.GetAvaliacaoPorEstabelecimentoId(e.EstabelecimentoId);
                e.Avaliacao = Math.Round(avaliacao, 1);
            }

            return estabelecimentosBd;
        }

        [HttpGet("getPorQuery")]
        public async Task<ActionResult<List<Estabelecimento>>> GetPorQuery(int id, string? nome, int idUsuario)
        {
            var todos = await _estabelecimentos.GetPorQuery(id, nome, idUsuario);

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                item.Usuarios.Senha = "";
            }

            return todos;
        }
    }
}
