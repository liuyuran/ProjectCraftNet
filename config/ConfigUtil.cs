using Tomlyn;

namespace ProjectCraftNet.config;

public class ConfigUtil {
    public static ConfigUtil Instance { get; } = new();
    public string modPath { get; private set; } = "";

    public void Init(string configFilePath) {
        var fileContent = File.ReadAllText(configFilePath);
        var model = Toml.ToModel(fileContent);
        var core = model["core"];
        if (core == null) throw new Exception("配置文件错误，缺少core段落");
        modPath = (string)model["mod_path"];
    }
}