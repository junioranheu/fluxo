using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosTiposApiController : ControllerBase
    {
        private readonly IUsuarioTipoRepository _usuariosTipos;

        public UsuariosTiposApiController(IUsuarioTipoRepository usuarioTipoRepository)
        {
            _usuariosTipos = usuarioTipoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<UsuarioTipo>>> GetTodos()
        {
            var todos = await _usuariosTipos.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioTipo>> GetPorId(int id)
        {
            var porId = await _usuariosTipos.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }
    }
}
