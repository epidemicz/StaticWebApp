using System.Security.Cryptography.X509Certificates;

public class ConfigOptions
{
    public string? ServerPort { get; set; } = "8085";
    public string? ServeDirectory { get; set; } = Directory.GetCurrentDirectory();
    public bool UseDirectoryBrowser { get; set; } = true;
    public bool UseRequiredGodotHeaders { get; set; } = true;

}