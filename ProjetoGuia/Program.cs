using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using System.Globalization;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------- Services -----------------------------------------------------

// Autenticação - https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-mvc-using-cookie/;
string telaAcessoNegado = "/sem-acesso";
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.LoginPath = telaAcessoNegado;
                   options.AccessDeniedPath = telaAcessoNegado;

                   // https://forums.asp.net/t/2157350.aspx?How+does+cookie+authentication+in+identity+framework+work+;
                   options.Cookie.Name = "fluxo_autenticacao";
                   options.ExpireTimeSpan = TimeSpan.FromDays(1);
               });

// Comprimir as imagens (gzip) - https://gunnarpeipman.com/aspnet-core-compress-gzip-brotli-content-encoding/;
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Compressão;
builder.Services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    "text/plain", "text/html", "text/css", "font/woff2",
                    "application/javascript", "image/x-icon", "image/png"
                };

                options.EnableForHttps = true;
                options.MimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });

// Cors;
builder.Services.AddCors();

// Etc;
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Buildar;
var app = builder.Build();

// ----------------------------------------------------- APP -----------------------------------------------------

// Definindo a cultura padrão: pt-BR - https://imasters.com.br/dotnet/asp-net-core-solucionando-erros-de-conversao-formatacao-com-localization;
var supportedCultures = new[] { new CultureInfo("pt-BR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(telaAcessoNegado);
    app.UseHsts();
}

// Outros;
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRouting();

// Cache em arquivos estáticos: https://stackoverflow.com/questions/57254048/how-to-cache-static-files-in-asp-net-core
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache para arquivos estáticos (30 dias);
        int segundos = 2592000;
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=" + segundos + "");
        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
    }
});

// Cache de resposta - https://docs.microsoft.com/pt-br/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1;
app.UseResponseCaching();

// Cors;
app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

// Autenticação;
app.UseAuthentication();
app.UseAuthorization();

// Página inicial;
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Inicio}/{id?}");

app.Run();

