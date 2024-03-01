using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using ModManager.network;
using static ModManager.localization.LocalizationManager;
using static ProjectCraftNet.Program;
using TcpListener = System.Net.Sockets.TcpListener;

namespace ProjectCraftNet.server;

/// <summary>
/// 一个简易的tcp服务器实现
/// </summary>
public class TcpServer
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(TcpServer));
    // 心跳包超时时间
    private const int SocketTimeout = 5;

    // 连接标识符与Socket实例的映射
    private Dictionary<ulong, Socket> Sockets { get; } = new();

    // 连接标识符生成器
    private ulong _socketId;

    public TcpServer()
    {
        NetworkEvents.SendEvent += SendMessage;
    }

    /// <summary>
    /// 启动服务监听，目前每个连接都会单独开一个线程，此函数会阻塞运行，所以请在新线程中调用
    /// </summary>
    /// <param name="ip">绑定ip</param>
    /// <param name="port">绑定端口</param>
    public async void StartServer(string ip, int port)
    {
        var ipAddress = IPAddress.Parse(ip);
        var listener = new TcpListener(ipAddress, port);
        listener.Start();
        Logger.LogInformation("{}", Localize(ModId, "Server started at {0}:{1}", ip, port));
        while (true)
        {
            var socket = await listener.AcceptTcpClientAsync();
            var thread = new Thread(() =>
            {
                var lastTime = DateTime.Now.Ticks;
                socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                var client = socket.Client;
                var socketId = Interlocked.Increment(ref _socketId);
                Sockets.Add(socketId, client);
                var tmp = new byte[4];
                var bytes = new byte[1024];
                var newPack = true;
                var packLen = 0;
                var packType = 0;
                var msgBuffer = new List<byte>();
                while (true)
                {
                    if (!socket.Connected)
                    {
                        // 连接断开
                        break;
                    }

                    if (DateTime.Now.Ticks - lastTime > SocketTimeout * 10000000)
                    {
                        // 心跳包超时
                        break;
                    }

                    for (var i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = 0;
                    }

                    int bytesRec;
                    try {
                        bytesRec = client.Receive(bytes, SocketFlags.None);
                        if (bytesRec == 0) {
                            Thread.Sleep(1000);
                            continue;
                        }
                    } catch (Exception) {
                        break;
                    }

                    if (newPack)
                    {
                        // 读取包长度
                        tmp[3] = bytes[0];
                        tmp[2] = bytes[1];
                        tmp[1] = bytes[2];
                        tmp[0] = bytes[3];
                        packLen = BitConverter.ToInt32(tmp, 0);
                        // 读取包类型
                        tmp[3] = bytes[4];
                        tmp[2] = bytes[5];
                        tmp[1] = bytes[6];
                        tmp[0] = bytes[7];
                        packType = BitConverter.ToInt32(tmp, 0);
                        newPack = false;
                        msgBuffer.Clear();
                    }

                    // 读取字节直到抵达第一个字节所标注的长度，下一次读取需要剔除所有的0
                    for (var i = 0; i < bytesRec; i++)
                    {
                        msgBuffer.Add(bytes[i]);
                        if (msgBuffer.Count < packLen + 8) continue;
                        newPack = true;

                        // 剔除前八个字节
                        for (var j = 0; j < 8; j++)
                        {
                            msgBuffer.RemoveAt(0);
                        }

                        if (packType == (int)PackType.Ping)
                        {
                            lastTime = DateTime.Now.Ticks;
                        }

                        NetworkEvents.FireReceiveEvent(socketId, packType, msgBuffer.ToArray(), socket.Client);
                        msgBuffer.Clear();
                        // 把剩下的数据塞进去
                        for (var j = i + 1; j < bytesRec; j++)
                        {
                            msgBuffer.Add(bytes[j]);
                        }

                        break;
                    }
                }
                socket.Close();
            });
            thread.Start();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socketId">连接标识符，当其为0时会触发广播</param>
    /// <param name="packType">包类型</param>
    /// <param name="data">包内容</param>
    private void SendMessage(ulong socketId, PackType packType, byte[] data)
    {
        if (socketId == 0)
        {
            foreach (var socketItem in Sockets.Where(socketItem => socketItem.Key != 0))
            {
                SendMessage(socketItem.Key, packType, data);
            }
        }
        if (!Sockets.TryGetValue(socketId, out var socket))
        {
            return;
        }

        var packLen = data.Length;
        var lenBytes = BitConverter.GetBytes(packLen);
        var typeBytes = BitConverter.GetBytes((int)packType);
        var sendBytes = new byte[packLen + 8];
        lenBytes.CopyTo(sendBytes, 0);
        typeBytes.CopyTo(sendBytes, 4);
        data.CopyTo(sendBytes, 8);
        try {
            socket.Send(sendBytes, SocketFlags.None);
        } catch (Exception e) {
            Sockets.Remove(socketId);
            Logger.LogError("{}", Localize(ModId, "Error when sending PackType: {0}, {1}", packType, e.Message));
        }
    }
}