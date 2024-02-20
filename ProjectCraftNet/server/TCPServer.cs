using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpListener = System.Net.Sockets.TcpListener;

namespace ProjectCraftNet.server;

public class TcpServer
{
    private readonly Dictionary<ulong, Socket> _clients = new();
    
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
                        
                        // TODO 这里需要处理消息
                        // ReceiveEvent?.Invoke(this, packType, msgBuffer.ToArray());
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
}