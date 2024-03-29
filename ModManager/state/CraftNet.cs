using ModManager.state.world;

namespace ModManager.state;

public class CraftNet
{
    public static CraftNet Instance { get; } = new();
    public readonly GameWorld World = new();
    public readonly CancellationTokenSource CancelToken = new();
}