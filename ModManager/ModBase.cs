namespace ModManager;

public abstract class ModBase {
    public abstract void OnInitialize();
    public abstract void OnLoad();
    public abstract void OnUnload();
    public abstract bool IsEnabled();
}