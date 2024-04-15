using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Get the directory to serve files from, otherwise default to current directory /wwwroot
var pathArg = args.FirstOrDefault(i => i.ToLower().StartsWith("path"));

var path = pathArg is not null
    ? pathArg.Replace("path=", string.Empty)
    : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

if (!Directory.Exists(path))
{
    Console.WriteLine($"Directory does not exist: {path}");
    Console.WriteLine("Usage: StaticWebApp.exe path=PATH_TO_DIRECTORY");
    return;
}

Console.WriteLine($"Serving files from {path}");

var fileProvider = new PhysicalFileProvider(path);

app.UseDefaultFiles();
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileProvider,
});

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,

    OnPrepareResponseAsync = async ctx =>
    {
        ctx.Context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        ctx.Context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

        await Task.CompletedTask;
    },
    FileProvider = fileProvider
});

app.MapGet("/hello", () => $"Hello from ${Environment.GetEnvironmentVariable("HOSTNAME")}");

app.Run();
