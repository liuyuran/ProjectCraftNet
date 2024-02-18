using System.Reflection;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using Tomlyn;

namespace ModManager;

public class ModManager
{
    private readonly ILogger _logger = SysLogger.GetLogger(typeof(ModManager));
    private List<ModBase> _mods = new();
    
    public void LoadMods(string modPath) {
        var modFiles = Directory.GetDirectories(modPath)/*.SelectMany(Directory.GetFiles).Where(file => file.EndsWith(".dll"))*/;
        foreach (var modFile in modFiles) {
            var fileContent = File.ReadAllText(Path.Combine(modFile, "mod.toml"));
            var config = Toml.ToModel(fileContent);
            var modName = (string?)config["core-file"];
            var modClass = (string?)config["core-class"];
            if (modName == null)
            {
                _logger.LogInformation("{}文件错误，缺少core-file字段", Path.Combine(modFile, "mod.toml"));
                continue;
            }
            if (modClass == null)
            {
                _logger.LogInformation("{}文件错误，缺少core-class字段", Path.Combine(modFile, "mod.toml"));
                continue;
            }
            var dllPath = Path.Combine(modFile, modName);
            // load all classes and scan for classes extend ModBase, like spring boot
            var assembly = Assembly.LoadFile(dllPath);
            var targetClass = assembly.GetType(modClass);
            if (targetClass == null)
            {
                _logger.LogInformation("{}文件错误，找不到core-class字段指定的类", Path.Combine(modFile, "mod.toml"));
                continue;
            }

            if (!targetClass.IsSubclassOf(typeof(ModBase)))
            {
                _logger.LogInformation("{}文件错误，core-class字段指定的类不是ModBase的子类", Path.Combine(modFile, "mod.toml"));
                continue;
            };
            // just load it
            var mod = (ModBase?)Activator.CreateInstance(targetClass);
            // maybe it will be null
            if (mod == null)
            {
                _logger.LogInformation("{}文件错误，无法实例化core-class字段指定的类", Path.Combine(modFile, "mod.toml"));
                continue;
            }
            _mods.Add(mod);
            mod.OnLoad();
        }
    }
}