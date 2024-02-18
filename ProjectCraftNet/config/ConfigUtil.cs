using ModManager;
using Tomlyn;
using Tomlyn.Model;

namespace ProjectCraftNet.config;

public class ConfigUtil {
    public static ConfigUtil Instance { get; } = new();
    private Config? _config;

    public void Init(string configFilePath) {
        var fileContent = File.ReadAllText(configFilePath);
        _config = Toml.ToModel<Config>(fileContent);
    }
    
    public Config GetConfig() {
        if (_config == null) {
            throw new Exception("Config is not initialized.");
        }
        return _config;
    }
}