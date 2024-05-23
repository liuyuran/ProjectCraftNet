using System.Net;
using System.Net.Sockets;
using ModManager.network;

namespace BlackBoxTest.utils;

public delegate void ReceiveEventHandler(int type, byte[] e);

public partial class TcpClient(string hostName, int port) {
    public event ReceiveEventHandler? ReceiveEvent;
    private Socket? _client;
    private Thread? _thread;
    private Thread? _keepAliveThread;
    private string HostName { get; } = hostName;
    private int Port { get; } = port;

    public async Task Connect(int port = -1) {
        var ipAddress = IPAddress.Parse(HostName);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        _client = new Socket(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
        var randomEndPoint = new IPEndPoint(IPAddress.Any, 0);
        _client.Bind(randomEndPoint);
        await _client.ConnectAsync(ipEndPoint);
        _thread = new Thread(Receive);
        _thread.Start();
        _keepAliveThread = new Thread(KeepAlive);
        _keepAliveThread.Start();
    }

    public async Task Disconnect() {
        if (_client is null)
            return;
        if (_client.Connected) await _client.DisconnectAsync(true);
        _thread?.Interrupt();
        _keepAliveThread?.Interrupt();
        ReceiveEvent = null;
    }

    private async Task Send(int type, byte[] message) {
        if (_client is null)
            return;
        var packLen = message.Length;
        var packType = BitConverter.GetBytes(type);
        var packLenBytes = BitConverter.GetBytes(packLen);
        packLenBytes = packLenBytes.Reverse().ToArray();
        packType = packType.Reverse().ToArray();
        var pack = new byte[packLen + 8];
        for (var i = 0; i < pack.Length; i++) {
            pack[i] = 0;
        }
        packLenBytes.CopyTo(pack, 0);
        packType.CopyTo(pack, 4);
        message.CopyTo(pack, 8);
        await _client.SendAsync(pack, SocketFlags.None);
    }

    private async void KeepAlive()
    {
        if (_client is null)
            return;
        while (true)
        {
            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception)
            {
                break;
            }
            if (!_client.Connected) return;
            try
            {
                var now = DateTimeOffset.Now;
                var nowTimestamp = now.ToUnixTimeMilliseconds();
                var timestamp = BitConverter.GetBytes((uint) nowTimestamp);
                await Send((int)PackType.Ping, timestamp);
            }
            catch (SocketException)
            {
                return;
            }
        }
    }

    private async void Receive() {
        if (_client is null)
            return;
        var tmp = new byte[4];
        var bytes = new byte[1024];
        var newPack = true;
        var packLen = 0;
        var packType = 0;
        var msgBuffer = new List<byte>();
        while (true) {
            // 置零
            for (var i = 0; i < bytes.Length; i++) {
                bytes[i] = 0;
            }
            
            int bytesRec;
            try
            {
                bytesRec = await _client.ReceiveAsync(bytes, SocketFlags.None).ConfigureAwait(false);
            } catch (SocketException)
            {
                break;
            }
            if (bytesRec == 0) {
                Thread.Sleep(1000);
                continue;
            }

            if (newPack) {
                // 读取包长度
                tmp[0] = bytes[0];
                tmp[1] = bytes[1];
                tmp[2] = bytes[2];
                tmp[3] = bytes[3];
                packLen = BitConverter.ToInt32(tmp, 0);
                // 读取包类型
                tmp[0] = bytes[4];
                tmp[1] = bytes[5];
                tmp[2] = bytes[6];
                tmp[3] = bytes[7];
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

                ReceiveEvent?.Invoke(packType, msgBuffer.ToArray());
                msgBuffer.Clear();
                // 把剩下的数据塞进去
                for (var j = i + 1; j < bytesRec; j++) {
                    msgBuffer.Add(bytes[j]);
                }

                break;
            }
        }
    }
}