namespace ModManager.game.block;

public abstract class Block
{
    public virtual string Id => "";
    public virtual string Name => "";
    public virtual string Material => "";
    public virtual BlockType BlockType => BlockType.Gas;
}