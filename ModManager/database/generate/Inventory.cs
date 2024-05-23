using System;
using System.Collections.Generic;

namespace ModManager.database.generate;

/// <summary>
/// 背包
/// </summary>
public partial class Inventory
{
    /// <summary>
    /// 用户id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 背包类型
    /// </summary>
    public string SlotKey { get; set; } = null!;

    /// <summary>
    /// 背包内容
    /// </summary>
    public string? SlotValue { get; set; }
}
