using System.Reflection;
using ProjectCraftNet.config;

namespace ProjectCraftNet.mod;

public class ModManager {
    public static ModManager Instance { get; } = new();
    
    public void LoadMods() {
        var modPath = ConfigUtil.Instance.modPath;
        var modFiles = Directory.GetFiles(modPath, "*.dll");
        foreach (var modFile in modFiles) {
            var assembly = Assembly.LoadFile(modFile);
            var types = assembly.GetTypes();
            foreach (var type in types) {
                if (!type.IsSubclassOf(typeof(ModBase))) continue;
                var mod = (ModBase?)Activator.CreateInstance(type);
                mod?.OnLoad();
            }
        }
    }
}