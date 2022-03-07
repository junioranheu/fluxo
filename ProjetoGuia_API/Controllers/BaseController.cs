using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjetoGuia_API.Services;
using ProjetoGuia_Biblioteca;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

// Como criar um BaseController: https://stackoverflow.com/questions/58735503/creating-base-controller-for-asp-net-core-to-do-logging-but-something-is-wrong-w;
// Como fazer os metódos da BaseController não bugar a API ([NonAction]): https://stackoverflow.com/questions/35788911/500-error-when-setting-up-swagger-in-asp-net-core-mvc-6-app
// Ou então usar "protected";
namespace ProjetoGuia_API.Controllers
{
    public abstract class BaseController<T> : Controller
    {
        protected async Task<bool> IsUsuarioSolicitadoMesmoDoToken(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token != null)
            {
                // var nomeUsuarioSistema = User.FindFirstValue(ClaimTypes.Name);          
                // var usuarioTipoid = User.FindFirstValue(ClaimTypes.Role);
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (usuarioId != id.ToString())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        protected static async Task<string> GetAPI(string caminho, string? token)
        {
            var resultado = null as dynamic;
            string urlApi = Biblioteca.CaminhoAPI();

            // https://www.c-sharpcorner.com/article/consuming-asp-net-web-api-rest-service-in-asp-net-mvc-using-http-client/
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Autorização com token;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

                // GET;
                HttpResponseMessage res = await client.GetAsync(caminho);

                // Resposta;
                if (res.IsSuccessStatusCode)
                {
                    resultado = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    string erro = Biblioteca.MensagemErroAPI(res, caminho);
                    throw new Exception(erro);
                }
            }

            return resultado;
        }

        protected static async Task<string> PostAPI(string caminho, dynamic? objeto, string? token)
        {
            var resultado = null as dynamic;
            string urlApi = Biblioteca.CaminhoAPI();

            // https://www.c-sharpcorner.com/article/consuming-asp-net-web-api-rest-service-in-asp-net-mvc-using-http-client/
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Autorização com token;
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }

                // Post;
                var objetoConvertido = JsonConvert.SerializeObject(objeto);
                var objetoConvertidoFinal = new StringContent(objetoConvertido, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync(caminho, objetoConvertidoFinal);

                // Resposta;
                if (res.IsSuccessStatusCode)
                {
                    resultado = res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    string erro = Biblioteca.MensagemErroAPI(res, caminho);
                    throw new Exception(erro);
                }
            }

            return resultado;
        }

        protected async Task<string> GerarToken(string? nomeUsuarioSistema, string? senha, ClaimsPrincipal? userClaim, HttpRequest? request, HttpResponse? response)
        {
            // Por via de regra, o método GerarToken deve receber um parâmetro preenchido e o outro nulo;
            // ("nomeUsuarioSistema" e "senha") preenchidos e "userClaim" nulo = caso #01;
            // ("nomeUsuarioSistema" e "senha") nulos e "userClaim" preenchido = caso #02 ou #03;

            // Verificação inicial (usuário deslogado);
            if (userClaim == null)
            {
                // Caso #01 - Usuário deslogado e está logando;
                // O idUsuario deve ser passado como parâmetro para que o token seja gerado;
                string? token = await Token(nomeUsuarioSistema, senha);

                // Salvar token no Cookies;
                SalvarCookies(response, token);

                return token;
            }
            else
            {
                // Nova verificação para ver se realmente o usuário está logado;
                if (userClaim.Identity.IsAuthenticated)
                {
                    string nomeUsuarioSistemaSemToken = userClaim.FindFirst(claim => claim.Type == ClaimTypes.UserData)?.Value;
                    string senhaUsuarioSemToken = userClaim.FindFirst(claim => claim.Type == ClaimTypes.Hash)?.Value;
                    string? tokenExistente = null as dynamic;

                    // Verificar se o usuário logado já tem token;
                    if (request != null)
                    {
                        if (request.Cookies.ContainsKey("X-Access-Token"))
                        {
                            tokenExistente = request.Cookies["X-Access-Token"];
                        }
                    }

                    if (!string.IsNullOrEmpty(tokenExistente))
                    {
                        bool isTokenValido = ValidarToken(tokenExistente);

                        // Verificar se o token ainda é valido (pelo tempo de expiração);
                        if (isTokenValido)
                        {
                            // Caso #02.1 - Se o usuário tiver token e estiver válido, retorne-o;
                            return tokenExistente;
                        }
                        else
                        {
                            // Caso #02.2 - Se o usuário tiver token e não estiver válido, retorne outro;
                            string? token = await Token(nomeUsuarioSistemaSemToken, senhaUsuarioSemToken);

                            // Salvar token no Cookies;
                            SalvarCookies(response, token);

                            return token;
                        }
                    }
                    else
                    {
                        // Caso #03 - Se o usuário estiver on-line e não tiver token, gere um novo;
                        string? token = await Token(nomeUsuarioSistemaSemToken, senhaUsuarioSemToken);

                        // Salvar token no Cookies;
                        SalvarCookies(response, token);

                        return token;
                    }
                }
                else
                {
                    // Caso #04 - Bug?
                    throw new Exception("Erro ao gerar token {GerarToken()}");
                }
            }
        }

        protected static async Task<string> Token(string nomeUsuarioSistema, string senha)
        {
            // Gerar o token usando o usuário;
            string caminho = String.Format("/api/UsuariosApi/autenticar/?nomeUsuarioSistema={0}&senha={1}", nomeUsuarioSistema, senha);
            string? tokenJson = await GetAPI(caminho, null);
            string? token = JsonConvert.DeserializeObject<string>(tokenJson);

            return token;
        }

        protected static bool ValidarToken(string token)
        {
            var chave = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Chave.chave));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = chave,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected static void SalvarCookies(HttpResponse? response, string token)
        {
            // Salvar o token em memória (in-memory);
            // Sobre ser safe, resposta de Big Pumpkin: https://stackoverflow.com/questions/27067251/where-to-store-jwt-in-browser-how-to-protect-against-csrf
            response.Cookies.Delete("fluxo_jwt");
            response.Cookies.Append("fluxo_jwt", token, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        // arquivo = o arquivo em si, a variável IFormFile;
        // id = o id do objeto em questão. Por exemplo, ao mudar a foto de perfil de um usuário, envie o id dele;
        // nomePasta = nome do caminho do arquivo, da pasta. Por exemplo: /upload/usuario/. "usuario" é o caminho;
        // arquivoAtual = o nome do arquivo atual, caso exista;
        // hostingEnvironment = o caminho até o wwwroot. Ele deve ser passado por parâmetro, já que não funcionaria aqui diretamente no BaseController;
        protected string UparImagem(IFormFile arquivo, int id, string nomePasta, string? arquivoAtual, IWebHostEnvironment hostingEnvironment)
        {
            // Procedimento de inicialização para salvar nova imagem;
            string webRootPath = hostingEnvironment.WebRootPath; // Vai até o wwwwroot;
            string restoCaminho = "/upload/" + nomePasta + "/"; // Acesso à pasta referente; 
            string nomeNovo = nomePasta + "-" + id.ToString() + ".webp"; // Nome novo do arquivo;
            string caminhoDestino = webRootPath + restoCaminho + nomeNovo; // Caminho de destino para upar;

            // Copiar o novo arquivo para o local de destino;
            if (arquivo.Length > 0)
            {
                // Verificar se já existe uma foto caso exista, delete-a;
                if (!String.IsNullOrEmpty(arquivoAtual))
                {
                    string caminhoArquivoAtual = webRootPath + restoCaminho + arquivoAtual;

                    // Verificar se o arquivo existe;
                    if (System.IO.File.Exists(caminhoArquivoAtual))
                    {
                        // Se existe, apague-o; 
                        System.IO.File.Delete(caminhoArquivoAtual);
                    }
                }

                // Então salve a imagem no servidor no formato WebP - https://blog.elmah.io/convert-images-to-webp-with-asp-net-core-better-than-png-jpg-files/;
                using (var webPFileStream = new FileStream(caminhoDestino, FileMode.Create))
                {
                    ImageFactory imageFactory = new(preserveExifData: false);
                    imageFactory.Load(arquivo.OpenReadStream())
                                .Format(new WebPFormat())
                                .Quality(10)
                                .Save(webPFileStream);
                }

                return nomeNovo;
            }
            else
            {
                return "";
            }
        }

        // https://app.sendgrid.com/
        protected static async Task<bool> EnviarEmail(string emailTo, string assunto, string conteudoEmailSemHtml, string conteudoEmailHtml)
        {
            if (String.IsNullOrEmpty(emailTo))
            {
                return false;
            }

            // https://app.sendgrid.com/guide/integrate/langs/csharp
            string apiKey = "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";
            string emailFrom = "accfake13@hotmail.com";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailFrom, "Adm do Fluxo");
            var to = new EmailAddress(emailTo);
            var msg = MailHelper.CreateSingleEmail(from, to, assunto, conteudoEmailSemHtml, conteudoEmailHtml);
            var resposta = await client.SendEmailAsync(msg);

            return resposta.IsSuccessStatusCode;
        }

        protected static async Task<bool> UparArquivoReact(string formPasta, string formId, IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                string webRootPath = Directory.GetCurrentDirectory() + "/Upload/" + formPasta + "/";
                string nomeNovo = formPasta + "-" + formId + System.IO.Path.GetExtension(formFile.FileName); // Nome novo do arquivo completo;
                string nomeSemExtensao = formPasta + "-" + formId; // Nome novo do arquivo sem extensão;
                string caminhoDestino = webRootPath + nomeNovo; // Caminho de destino para upar;

                // Verificar se já existe uma foto caso exista, delete-a;
                DirectoryInfo root = new(webRootPath);
                FileInfo[] listfiles = root.GetFiles(nomeSemExtensao + ".*");
                if (listfiles.Length > 0)
                {
                    foreach (FileInfo file in listfiles)
                    {
                        System.IO.File.Delete(file.ToString());
                    }
                }

                // Salvar imagem na pasta Upload na API;
                using (var fs = new FileStream(caminhoDestino, FileMode.Create))
                {
                    await formFile.CopyToAsync(fs);
                }

                return true;
            }

            return false;
        }
    }
}

