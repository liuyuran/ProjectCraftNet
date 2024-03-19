using ModManager.user;

namespace ModManager.command;

public interface ICommand
{
    /// <summary>
    /// 获取命令名称
    ///
    /// 【/mod list】中，mod就是命令名称
    /// </summary>
    /// <returns>命令名称</returns>
    public string GetName();
    
    /// <summary>
    /// 获取命令简要说明
    ///
    /// 这里需要判断用户是否有权限使用命令
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    /// <returns>命令说明，如果无权使用，则返回null</returns>
    public string? GetShortDescription(UserInfo userInfo);
    
    /// <summary>
    /// 获取命令详细使用说明
    /// 
    /// 这里需要判断用户是否有权限使用命令
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    /// <param name="args">命令参数</param>
    /// <returns>命令使用说明，如果无权使用，则返回null</returns>
    public string? GetUsage(UserInfo userInfo, string[] args);
    
    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    /// <param name="args">命令参数</param>
    public void Execute(UserInfo userInfo, string[] args);
}