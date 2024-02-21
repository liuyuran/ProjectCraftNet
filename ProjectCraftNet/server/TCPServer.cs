using System.Net;
using System.Net.Sockets;
using ModManager.network;
using TcpListener = System.Net.Sockets.TcpListener;

namespace ProjectCraftNet.server;

/// <summary>
/// 一个简易的tcp服务器实现
/// </summary>
public class TcpServer
{
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
        while (true)
        {
            var socket = await listener.AcceptTcpClientAsync();
            var thread = new Thread(() =>
            {
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
                        break;
                    }
                    for (var i = 0; i < bytes.Length; i++) {
                        bytes[i] = 0;
                    }

                    var bytesRec = client.Receive(bytes, SocketFlags.None);
                    if (bytesRec == 0) {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (newPack) {
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
                    }

                    // 读取字节直到抵达第一个字节所标注的长度，下一次读取需要剔除所有的0
                    for (var i = 0; i < bytesRec; i++) {
                        msgBuffer.Add(bytes[i]);
                        if (msgBuffer.Count < packLen + 8) continue;
                        newPack = true;
                
                        // 剔除前八个字节
                        for (var j = 0; j < 8; j++) {
                            msgBuffer.RemoveAt(0);
                        }
                        
                        NetworkEvents.FireReceiveEvent(socketId, packType, msgBuffer.ToArray());
                        msgBuffer.Clear();
                        // 把剩下的数据塞进去
                        for (var j = i + 1; j < bytesRec; j++) {
                            msgBuffer.Add(bytes[j]);
                        }

                        break;
                    }
                }
            });
            thread.Start(socket);
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socketId">连接标识符</param>
    /// <param name="packType">包类型</param>
    /// <param name="data">包内容</param>
    private async void SendMessage(ulong socketId, int packType, byte[] data)
    {
        if (!Sockets.TryGetValue(socketId, out var socket))
        {
            return;
        }

        var packLen = data.Length + 8;
        var lenBytes = BitConverter.GetBytes(packLen);
        var typeBytes = BitConverter.GetBytes(packType);
        var sendBytes = new byte[packLen];
        lenBytes.CopyTo(sendBytes, 0);
        typeBytes.CopyTo(sendBytes, 4);
        data.CopyTo(sendBytes, 8);
        await socket.SendAsync(sendBytes, SocketFlags.None);
    }
}