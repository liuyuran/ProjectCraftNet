using CoreMod.blocks;
using CoreMod.generator;
using ModManager.game.block;
using ModManager.game.generator;
using ModManager.mod;

namespace CoreMod;

public class CoreMod: ModBase
{
    public override string GetName()
    {
        return "失落大地";
    }

    public override string GetDescription()
    {
        return "游戏默认mod，如果没有其他mod明确表示可以替代它，务必不要取消加载";
    }

    public override string GetVersion()
    {
        return "1.0.0";
    }

    public override void OnInitialize()
    {
        OnLoad();
    }

    public override void OnLoad()
    {
        ChunkGeneratorManager.AddChunkGenerator(0, new DefaultChunkGenerator());
        BlockManager.RegisterBlock<Dirt>();
    }

    public override void OnUnload()
    {
        ChunkGeneratorManager.RemoveChunkGenerator(0);
        BlockManager.UnregisterBlock<Dirt>();
    }

    public override bool IsEnabled()
    {
        return true;
    }
}