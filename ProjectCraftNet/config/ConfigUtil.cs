using ModManager;
using Tomlyn;

namespace ProjectCraftNet.config;

/// <summary>
/// 读取全局配置文件的工具类
/// </summary>
public class ConfigUtil {
    public static ConfigUtil Instance { get; } = new();
    private Config? _config;
    
    /// <summary>
    /// 大驼峰命名法转中划线命名法
    /// </summary>
    /// <param name="str">大驼峰属性名</param>
    /// <returns>中划线属性名</returns>
    private static string CamelCaseToSnakeCase(string str) {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString())).ToLower();
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="configFilePath">配置文件路径，一般来自启动参数传入</param>
    public void Init(string configFilePath) {
        var fileContent = File.ReadAllText(configFilePath);
        var options = new TomlModelOptions
        {
            // 注意注意，这里是将类里的字段名转为文件里的字段名的函数，不是反过来
            ConvertPropertyName = CamelCaseToSnakeCase
        };
        _config = Toml.ToModel<Config>(fileContent, options: options);
    }
    
    /// <summary>
    /// 获取配置参数
    /// </summary>
    /// <returns>配置实例</returns>
    public Config GetConfig() {
        if (_config == null) {
            throw new Exception("Config is not initialized.");
        }
        return _config;
    }
}