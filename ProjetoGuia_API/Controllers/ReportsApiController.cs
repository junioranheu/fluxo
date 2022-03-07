using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsApiController : BaseController<ReportsApiController>
    {
        private readonly IReportRepository _reports;

        public ReportsApiController(IReportRepository reportRepository)
        {
            _reports = reportRepository;
        }

        [HttpGet("todos")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<List<Report>>> GetTodos()
        {
            var todos = await _reports.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                if (item.Usuario != null)
                {
                    item.Usuario.Senha = "";
                }
            }

            return todos;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<Report>> GetPorId(int id)
        {
            var porId = await _reports.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            porId.Usuario.Senha = "";

            return porId;
        }

        [HttpPost("criar")]
        public async Task<ActionResult<bool>> PostCriar(Report report)
        {
            var isOk = await _reports.PostCriar(report);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }
    }
}
