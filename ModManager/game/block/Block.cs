namespace ModManager.game.block;

public abstract class Block
{
    public virtual string Id => throw new NotImplementedException();
    public virtual string Name => throw new NotImplementedException();
    public virtual string Material => "";
    public virtual BlockType BlockType => throw new NotImplementedException();
}