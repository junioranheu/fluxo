using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CidadesApiController : ControllerBase
    {
        private readonly ICidadeRepository _cidades;

        public CidadesApiController(ICidadeRepository cidadeRepository)
        {
            _cidades = cidadeRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<Cidade>>> GetTodos()
        {
            var todos = await _cidades.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cidade>> GetPorId(int id)
        {
            var porId = await _cidades.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }

        [HttpGet("getPorNomeMaisSiglaEstado")]
        public async Task<ActionResult<Cidade>> GetPorNomeMaisSiglaEstado(string nomeCidade, string siglaEstado)
        {
            var porNomeMaisSiglaEstado = await _cidades.GetPorNomeMaisSiglaEstado(nomeCidade, siglaEstado);

            if (porNomeMaisSiglaEstado == null)
            {
                return NotFound();
            }

            return porNomeMaisSiglaEstado;
        }
    }
}
