using System;
using System.Collections.Generic;

namespace ModManager.database.generate;

/// <summary>
/// 玩家注册表
/// </summary>
public partial class User
{
    /// <summary>
    /// 主键
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 登录所用的用户名
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 用户密码散列值
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 显示用的昵称
    /// </summary>
    public string Nickname { get; set; } = null!;

    /// <summary>
    /// 所在世界id
    /// </summary>
    public long WorldId { get; set; }

    /// <summary>
    /// x坐标
    /// </summary>
    public long PosX { get; set; }

    /// <summary>
    /// y坐标
    /// </summary>
    public long PosY { get; set; }

    /// <summary>
    /// z坐标
    /// </summary>
    public long PosZ { get; set; }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public int GameMode { get; set; }
}
