using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjetoGuia_API.Interfaces;
using ProjetoGuia_API.Models;

namespace ProjetoGuia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsApiController : BaseController<PostsApiController>
    {
        private readonly IPostRepository _posts;

        public PostsApiController(IPostRepository postRepository)
        {
            _posts = postRepository;
        }

        [HttpGet("todos")]
        public async Task<ActionResult<List<Post>>> GetTodos()
        {
            var todos = await _posts.GetTodos();

            // Esconder alguns atributos;
            foreach (var item in todos)
            {
                item.Usuarios.Senha = "";
            }

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPorId(int id)
        {
            var porId = await _posts.GetPorId(id);

            if (porId == null)
            {
                return NotFound();
            }

            // Esconder alguns atributos;
            porId.Usuarios.Senha = "";

            return porId;
        }

        [HttpPost("criar")]
        [Authorize]
        public async Task<ActionResult<bool>> PostCriar(Post post)
        {
            var isOk = await _posts.PostCriar(post);

            if (isOk < 1)
            {
                return NotFound();
            }

            return true;
        }

        [HttpPost("atualizar")]
        [Authorize]
        public async Task<ActionResult<bool>> PostAtualizar(Post post)
        {
            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(post.Usuarios.UsuarioId);

            if (isMesmoUsuario)
            {
                var isOk = await _posts.PostAtualizar(post);

                if (isOk < 1)
                {
                    return NotFound();
                }

                return true;
            }

            return Unauthorized();
        }

        [HttpPost("deletar")]
        [Authorize]
        public async Task<ActionResult<int>> PostDeletar(int id)
        {
            // Verificar se o usuário existe;
            string caminho = String.Format("/api/PostsApi/{0}", id);
            var resultado = await GetAPI(caminho, null);
            Post? p = JsonConvert.DeserializeObject<Post>(resultado);

            // Verifica se o post existe;
            if (p == null)
            {
                return NotFound("Post inválido");
            }

            var isMesmoUsuario = await IsUsuarioSolicitadoMesmoDoToken(p.Usuarios.UsuarioId);
            if (isMesmoUsuario)
            {
                var isOk = await _posts.PostDeletar(id);

                if (isOk < 1)
                {
                    return NotFound();
                }

                return isOk;
            }

            return Unauthorized();
        }

        [HttpGet("getTodosPorUsuarioIdTipoPostId")]
        public async Task<ActionResult<List<Post>>> GetTodosPorUsuarioId(int usuarioId, int tipoPostId)
        {
            var postsBd = await _posts.GetTodosPorUsuarioId(usuarioId, tipoPostId);

            if (postsBd == null)
            {
                return NotFound();
            }

            return postsBd;
        }
    }
}
