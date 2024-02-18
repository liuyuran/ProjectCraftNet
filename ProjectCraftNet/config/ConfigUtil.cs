using ModManager;
using Tomlyn;
using Tomlyn.Model;

namespace ProjectCraftNet.config;

public class ConfigUtil {
    public static ConfigUtil Instance { get; } = new();
    public Config Config { get; private set; }

    public void Init(string configFilePath) {
        var fileContent = File.ReadAllText(configFilePath);
        var model = Toml.ToModel(fileContent);
        var core = (TomlTable?)model["core"];
        if (core == null) throw new Exception("配置文件错误，缺少core段落");
        var modPath = (string)core["mod_path"];
        Config = new Config(modPath);
    }
}