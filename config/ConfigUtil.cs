using Tomlyn;

namespace ProjectCraftNet.config;

public class ConfigUtil {
    public static ConfigUtil Instance { get; } = new();

    public void Init(string configFilePath) {
        var fileContent = File.ReadAllText(configFilePath);
        var model = Toml.ToModel(fileContent);
        var global = (string)model["global"]!;
        Console.WriteLine(global);
    }
}