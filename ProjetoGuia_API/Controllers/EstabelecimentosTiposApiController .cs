using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentosTiposApiController : ControllerBase
    {
        private readonly IEstabelecimentoTipoRepository _estabelecimentosTipos;

        public EstabelecimentosTiposApiController(IEstabelecimentoTipoRepository estabelecimentoTipoRepository)
        {
            _estabelecimentosTipos = estabelecimentoTipoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<EstabelecimentoTipo>>> GetTodos()
        {
            var todos = await _estabelecimentosTipos.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EstabelecimentoTipo>> GetPorId(int id)
        {
            var porId = await _estabelecimentosTipos.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }

        [HttpPost("criar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostCriar(EstabelecimentoTipo estabelecimentoTipo)
        {
            var isOk = await _estabelecimentosTipos.PostCriar(estabelecimentoTipo);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("atualizar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostAtualizar(EstabelecimentoTipo estabelecimentoTipo)
        {
            var isOk = await _estabelecimentosTipos.PostAtualizar(estabelecimentoTipo);

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
            var isOk = await _estabelecimentosTipos.PostDeletar(id);

            if (isOk < 1)
            {
                return NotFound();
            }

            return isOk;
        }
    }
}
