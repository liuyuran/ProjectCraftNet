using ModManager.state.world;

namespace ModManager.state;

public class ProjectCraftNet
{
    public static ProjectCraftNet Instance { get; } = new();
    public readonly GameWorld World = new();
}