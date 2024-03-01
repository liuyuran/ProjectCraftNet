namespace ModManager;

public abstract class ModBase {
    public abstract string GetName();
    public abstract string GetDescription();
    public abstract string GetVersion();
    public abstract void OnInitialize();
    public abstract void OnLoad();
    public abstract void OnUnload();
    public abstract bool IsEnabled();
}