namespace ModManager.game.item;

/// <summary>
/// 如果一个方块需要拥有对应的一对一的物品形态，则需要实现这个接口
/// </summary>
public interface IItemProvider
{
    Item AsItem();
}