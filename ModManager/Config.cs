namespace ModManager;

public sealed class Config
{
    public Config(string modPath)
    {
        ModPath = modPath;
    }

    public string ModPath { get; private set; }
}