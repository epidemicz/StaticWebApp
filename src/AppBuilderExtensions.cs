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
        var path = GetArg(args, "PATH");
        var port = GetArg(args, "SERVER_PORT");
        var defaultServeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // If a path was passed in, use that, otherwise, use the configured path, but default to wwwroot if not configured
        Options.ServeDirectory = path switch
        {
            // override with the path from the command line
            not null => path,
            // use the configured path if it exists else use the default
            _ => Options.ServeDirectory ??= defaultServeDirectory
        };

        // When container, use server port environment variable
        // otherwise, use the SERVER_PORT arg and default to the config
        Options.ServerPort = RunningInContainer switch 
        {
            true => Environment.GetEnvironmentVariable("SERVER_PORT"),
            false => port ??= Options.ServerPort
        };

        // If a port was passed in, use that, otherwise, use the configured port
        Options.ServerPort = port ??= Options.ServerPort;

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

    /// <summary>
    /// Retrieves the value of an argument from an array of command line arguments.
    /// </summary>
    /// <param name="args">The array of command line arguments.</param>
    /// <param name="name">The name of the argument to retrieve.</param>
    /// <returns>The value of the argument, or null if the argument does not exist.</returns>
    private static string? GetArg(string[] args, string name)
    {
        var arg = args.FirstOrDefault(i => i.ToLower().StartsWith(name.ToLower()));

        if (arg is not null) 
        {
            return arg.Replace($"{name.ToLower()}=", "");
        }

        return null;
    }
}