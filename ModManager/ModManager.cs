using System.Reflection;
using Microsoft.Extensions.Logging;
using ModManager.localization;
using ModManager.logger;
using Tomlyn;

namespace ModManager;

public class ModManager
{
    private readonly ILogger _logger = SysLogger.GetLogger(typeof(ModManager));
    private static string ModId => "core-system";

    public List<ModBase> AllMods { get; } = [];

    public List<ModBase> EnabledMods => AllMods.Where(mod => mod.IsEnabled()).ToList();
    
    public void LoadMods(string modPath) {
        var modFiles = Directory.GetDirectories(modPath);
        foreach (var modFile in modFiles) {
            var fileContent = File.ReadAllText(Path.Combine(modFile, "mod.toml"));
            var modDictName = Path.GetFileName(modFile);
            var config = Toml.ToModel(fileContent);
            if (!config.TryGetValue("core-file", out var coreFile))
            {
                _logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} meta invalid", modDictName));
                continue;
            }
            if (!config.TryGetValue("core-class", out var coreClass))
            {
                _logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} meta invalid", modDictName));
                continue;
            }
            var modName = (string)coreFile;
            var modClass = (string)coreClass;
            var dllPath = Path.Combine(modFile, modName);
            // load all classes and scan for classes extend ModBase, like spring boot
            var assembly = Assembly.LoadFile(dllPath);
            var targetClass = assembly.GetType(modClass);
            if (targetClass == null)
            {
                _logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }

            if (!targetClass.IsSubclassOf(typeof(ModBase)))
            {
                _logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }
            // just load it
            var mod = (ModBase?)Activator.CreateInstance(targetClass);
            // maybe it will be null
            if (mod == null)
            {
                _logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }
            AllMods.Add(mod);
            mod.OnInitialize();
        }
    }
}