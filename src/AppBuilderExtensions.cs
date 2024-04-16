using System.Security.Cryptography;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic;

public static class AppBuilderExtensions
{
    public static bool RunningInContainer => 
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));

    public static ConfigOptions Options { get; private set; } = new ConfigOptions();

    public static IApplicationBuilder ConfigureFileServing(this IApplicationBuilder app, string[] args, IConfiguration? config = null)
    {
        config?.Bind("ConfigOptions", Options);

        // Was a directory passed in?  If so, use that, otherwise use current directory /wwwroot
        var pathArg = args.FirstOrDefault(i => i.ToLower().StartsWith("path"));
        var path = pathArg?.Replace("path=", string.Empty);
        var defaultServeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // If a path was passed in, use that, otherwise, use the configured path, but default to wwwroot if not configured
        Options.ServeDirectory = pathArg switch 
        {
            // override with the path from the command line
            not null => path,
            // use the configured path if it exists else use the default
            _ => Options.ServeDirectory ??= defaultServeDirectory
        };

        if (!Directory.Exists(Options.ServeDirectory))
        {
            Console.WriteLine($"Directory does not exist: {Options.ServeDirectory}");
            Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} path=PATH_TO_DIRECTORY");
            Environment.Exit(0);
        }

        var fileProvider = new PhysicalFileProvider(Options.ServeDirectory);
        
        app.UseDefaultFiles();

        // Add support for directory browsing
        if (Options.UseDirectoryBrowser)
        {
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
            });
        }

        // Add support for required headers for godot
        if (Options.UseRequiredGodotHeaders)
        {
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
        }

        return app;
    }
}