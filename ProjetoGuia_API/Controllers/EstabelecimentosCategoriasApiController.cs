using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentosCategoriasApiController : ControllerBase
    {
        private readonly IEstabelecimentoCategoriaRepository _estabelecimentosCategorias;

        public EstabelecimentosCategoriasApiController(IEstabelecimentoCategoriaRepository estabelecimentoCategoriaRepository)
        {
            _estabelecimentosCategorias = estabelecimentoCategoriaRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<EstabelecimentoCategoria>>> GetTodos()
        {
            var todos = await _estabelecimentosCategorias.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EstabelecimentoCategoria>> GetPorId(int id)
        {
            var porId = await _estabelecimentosCategorias.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }

        [HttpPost("criar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostCriar(EstabelecimentoCategoria estabelecimentoCategoria)
        {
            var isOk = await _estabelecimentosCategorias.PostCriar(estabelecimentoCategoria);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("atualizar")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<bool>> PostAtualizar(EstabelecimentoCategoria estabelecimentoCategoria)
        {
            var isOk = await _estabelecimentosCategorias.PostAtualizar(estabelecimentoCategoria);

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
            var isOk = await _estabelecimentosCategorias.PostDeletar(id);

            if (isOk < 1)
            {
                return NotFound();
            }

            return isOk;
        }
    }
}
