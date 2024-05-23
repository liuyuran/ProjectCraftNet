using ModManager.state.world;

namespace ModManager.state;

public class CraftNet
{
    public static readonly AutoResetEvent MapInitEvent = new(false);
    public static readonly AutoResetEvent ServerInitEvent = new(false);
    public static readonly AutoResetEvent ClosingEvent = new(false);
    public static CraftNet Instance { get; } = new();
    public readonly GameWorld World = new();
    public readonly CancellationTokenSource CancelToken = new();
}