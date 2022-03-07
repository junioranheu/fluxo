using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsTiposApiController : ControllerBase
    {
        private readonly IPostTipoRepository _PostsTipos;

        public PostsTiposApiController(IPostTipoRepository PostTipoRepository)
        {
            _PostsTipos = PostTipoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<PostTipo>>> GetTodos()
        {
            var todos = await _PostsTipos.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostTipo>> GetPorId(int id)
        {
            var porId = await _PostsTipos.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }
    }
}
