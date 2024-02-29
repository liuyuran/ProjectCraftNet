using System.Reflection;
using Microsoft.Extensions.Logging;
using ModManager.localization;
using ModManager.logger;
using Tomlyn;

namespace ModManager;

public class ModManager
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(ModManager));
    public static string ModId => "core-system";

    public static List<ModBase> AllMods { get; } = [];

    public static List<ModBase> EnabledMods => AllMods.Where(mod => mod.IsEnabled()).ToList();
    
    public static void LoadMods(string modPath) {
        var modFiles = Directory.GetDirectories(modPath);
        foreach (var modFile in modFiles) {
            if (!File.Exists(Path.Combine(modFile, "mod.toml"))) {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} meta not found", Path.GetFileName(modFile)));
                continue;
            }
            var fileContent = File.ReadAllText(Path.Combine(modFile, "mod.toml"));
            var modDictName = Path.GetFileName(modFile);
            var config = Toml.ToModel(fileContent);
            if (!config.TryGetValue("core-file", out var coreFile))
            {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} meta invalid", modDictName));
                continue;
            }
            if (!config.TryGetValue("core-class", out var coreClass))
            {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} meta invalid", modDictName));
                continue;
            }
            var modName = (string)coreFile;
            var modClass = (string)coreClass;
            var dllPath = Path.Combine(modFile, modName);
            var absolutePath = Path.GetFullPath(dllPath);
            // load all classes and scan for classes extend ModBase, like spring boot
            var assembly = Assembly.LoadFile(absolutePath);
            var targetClass = assembly.GetType(modClass);
            if (targetClass == null)
            {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }

            if (!targetClass.IsSubclassOf(typeof(ModBase)))
            {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }
            // just load it
            var mod = (ModBase?)Activator.CreateInstance(targetClass);
            // maybe it will be null
            if (mod == null)
            {
                Logger.LogInformation("{}", LocalizationManager.Localize(ModId, "{0} main class invalid", modDictName));
                continue;
            }
            AllMods.Add(mod);
            mod.OnInitialize();
        }
    }
}