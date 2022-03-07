using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosApiController : ControllerBase
    {
        private readonly IEstadoRepository _estados;

        public EstadosApiController(IEstadoRepository estadoRepository)
        {
            _estados = estadoRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<Estado>>> GetTodos()
        {
            var todos = await _estados.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Estado>> GetPorId(int id)
        {
            var porId = await _estados.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }
    }
}
