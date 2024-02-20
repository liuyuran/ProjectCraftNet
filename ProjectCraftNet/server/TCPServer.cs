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
                while (true)
                {
                    if (!socket.Connected)
                    {
                        break;
                    }
                    var buffer = new byte[1024];
                    var length = socket.Client.Receive(buffer);
                    if (length == 0)
                    {
                        break;
                    }
                    var data = Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine(data);
                    var response = Encoding.UTF8.GetBytes("Hello, World!");
                    socket.Client.Send(response);
                    socket.Close();
                }
            });
            thread.Start(socket);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}