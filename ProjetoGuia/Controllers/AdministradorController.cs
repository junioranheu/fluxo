using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoGuia_API.Controllers;
using ProjetoGuia_API.Models;
using ProjetoGuia_Biblioteca;

namespace ProjetoGuia.Controllers
{
    public class AdministradorController : BaseController<AdministradorController>
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdministradorController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment; // Pegar o caminho;
        }

        [Authorize(Roles = "1")]
        [Route("administrador/reports")]
        public async Task<IActionResult> Reports()
        {
            string? token = await GerarToken(null, null, User, Request, Response);
            var resultado = await GetAPI("/api/ReportsApi/todos", token);
            var reportsProblemasBd = JsonConvert.DeserializeObject<List<Report>>(resultado);
            ViewData["ReportsProblemasBd"] = reportsProblemasBd;

            return View();
        }

        [Authorize(Roles = "1")]
        [Route("administrador/gerenciar-armazenamento")]
        public IActionResult Armazenamento()
        {
            string webRootPath = _hostingEnvironment.WebRootPath; // Vai até o wwwwroot;

            // 01 - -=-=-=-=-=-=-=- Geral -=-=-=-=-=-=-=-
            string restoCaminho = "";
            string caminhoDestino = webRootPath + restoCaminho;

            // Realizar a soma do tamanho dos arquivos - https://stackoverflow.com/questions/21428843/how-can-i-get-total-size-of-particular-folder-in-c (Tim Schmelter);
            DirectoryInfo dir = new DirectoryInfo(caminhoDestino);
            FileInfo[] arquivos = dir.GetFiles("*.*", SearchOption.AllDirectories);
            long tamanhoBytes = arquivos.Sum(f => f.Length);

            ViewBag.GCaminho = caminhoDestino;
            ViewBag.GQuantidadeArquivos = arquivos.Count();
            ViewBag.GTamanhoArquivos = Biblioteca.FormatarBytes(tamanhoBytes);

            // 02 - -=-=-=-=-=-=-=- Usuários -=-=-=-=-=-=-=-
            restoCaminho = "/upload/usuario/";
            caminhoDestino = webRootPath + restoCaminho;

            // Realizar a soma do tamanho dos arquivos;
            if (Directory.Exists(caminhoDestino))
            {
                dir = new DirectoryInfo(caminhoDestino);
                arquivos = dir.GetFiles();
                tamanhoBytes = arquivos.Sum(f => f.Length);

                ViewBag.UCaminho = caminhoDestino;
                ViewBag.UQuantidadeArquivos = arquivos.Count();
                ViewBag.UTamanhoArquivos = Biblioteca.FormatarBytes(tamanhoBytes);
            }

            // 03 - -=-=-=-=-=-=-=- POSTS -=-=-=-=-=-=-=-
            restoCaminho = "/upload/post/";
            caminhoDestino = webRootPath + restoCaminho;

            // Realizar a soma do tamanho dos arquivos;
            if (Directory.Exists(caminhoDestino))
            {
                dir = new DirectoryInfo(caminhoDestino);
                arquivos = dir.GetFiles();
                tamanhoBytes = arquivos.Sum(f => f.Length);

                ViewBag.PCaminho = caminhoDestino;
                ViewBag.PQuantidadeArquivos = arquivos.Count();
                ViewBag.PTamanhoArquivos = Biblioteca.FormatarBytes(tamanhoBytes);
            }

            return View();
        }
    }
}
