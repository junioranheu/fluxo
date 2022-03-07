using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;
using ProjetoGuia_Biblioteca;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsTemporariasApiController : BaseController<UrlsTemporariasApiController>
    {
        private readonly IUrlTemporariaRepository _urlTemporaria;

        public UrlsTemporariasApiController(IUrlTemporariaRepository urlTemporariaRepository)
        {
            _urlTemporaria = urlTemporariaRepository;
        }

        [HttpGet("todos")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<List<UrlTemporaria>>> GetTodos()
        {
            var todos = await _urlTemporaria.GetTodos();
            return todos;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<UrlTemporaria>> GetPorId(int id)
        {
            var porId = await _urlTemporaria.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            return porId;
        }

        [HttpPost("criar")]
        public async Task<ActionResult<string>> PostCriar(UrlTemporaria urlTemporaria, string? urlTipo)
        {
            // Buscar tipo de url temporária;
            int urlTipoId = await _urlTemporaria.GetTipoUrlId(urlTipo);
            if (urlTipoId < 1)
            {
                return NotFound();
            }

            // Completar a variável "urlTemporaria";
            urlTemporaria.UrlTemporariaTipoId = urlTipoId;
            urlTemporaria.Url = Biblioteca.CodigoAleatorio(20); // Gerar código aleatório;

            // Criar url temporária;
            var isOk = await _urlTemporaria.PostCriar(urlTemporaria);

            if (isOk < 1)
            {
                return NotFound();
            }

            // Retornar a url gerada;
            return urlTemporaria.Url;
        }

        [HttpGet("getPorTipoUrlEIdDinamica")]
        public async Task<ActionResult<UrlTemporaria>> GetPorTipoUrlEIdDinamica(string urlTipo, string urlTemporaria)
        {
            // Buscar tipo de url temporária;
            int urlTipoId = await _urlTemporaria.GetTipoUrlId(urlTipo);
            if (urlTipoId < 1)
            {
                return NotFound();
            }

            var url = await _urlTemporaria.GetPorTipoUrlEIdDinamica(urlTipoId, urlTemporaria);

            if (url == null)
            {
                return NotFound();
            }

            return url;
        }
    }
}
