﻿using System.Net.Sockets;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.eventBus;
using ModManager.game.user;
using ModManager.logger;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.network;

/// <summary>
/// 网络服务抽象类，用于屏蔽服务器层的具体实现
/// </summary>
public static class NetworkEvents
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(NetworkEvents));

    public delegate void SendEventHandler(long socketId, PackType packType, byte[] data);

    // 发送数据请调用此事件
    public static event SendEventHandler? SendEvent;

    public static void FireSendEvent(long socketId, PackType packType, byte[] data)
    {
        try
        {
            SendEvent?.Invoke(socketId, packType, data);
        }
        catch (Exception e)
        {
            Logger.LogError("{}", Localize(ModId, "Error when sending PackType: {0}, {1}", packType, e.Message));
        }
    }

    public static void FireSendTextEvent(long socketId, string text)
    {
        try
        {
            var msg = new ChatAndBroadcast
            {
                Msg = text
            };
            var data = msg.ToByteArray();
            SendEvent?.Invoke(socketId, PackType.ChatPack, data);
        }
        catch (Exception e)
        {
            Logger.LogError("{}", Localize(ModId, "Error when sending PackType: {0}, {1}", PackType.ChatPack, e.Message));
        }
    }

    public static void FireReceiveEvent(long socketId, int packType, byte[] data, Socket socket)
    {
        if (!Enum.IsDefined(typeof(PackType), packType))
        {
            Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
            return;
        }

        if (packType == (int)PackType.PingPack)
        {
            // 心跳包不需要处理
            var now = DateTimeOffset.Now;
            var nowTimestamp = now.ToUnixTimeMilliseconds();
            var timestamp = BitConverter.ToUInt32(data, 0);
            var ping = (uint)(nowTimestamp - timestamp);
            UserManager.SetClientPing(socketId, ping);
            return;
        }

        var info = new ClientInfo
        {
            SocketId = socketId,
            Ip = socket.RemoteEndPoint?.ToString() ?? ""
        };
        Logger.LogDebug("{}", Localize(ModId, "Receive PackType: {0} from {1}", (PackType)packType, socketId));
        try
        {
            NetworkPackBus.Trigger(packType, info, data);
        }
        catch (Exception e)
        {
            Logger.LogError("{}", Localize(ModId, "Error when handling PackType: {0}, {1}", packType, e.Message));
        }
    }
}