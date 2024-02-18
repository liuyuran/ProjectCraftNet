using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProjectCraftNet.server;

public class TcpServer
{
    private Dictionary<ulong, Socket> _clients = new();
    
    public void StartServer(string ip, int port)
    {
        using Socket listener = new(
            IPAddress.Parse(ip).AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        listener.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        listener.Listen(100);
        listener.BeginAccept(AcceptCallback, listener);
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request
        var listener = (Socket?) ar.AsyncState;
        if (listener == null) return;
        var handler = listener.EndAccept(ar);
        var state = new StateObject {WorkSocket = handler};
        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
        listener.BeginAccept(AcceptCallback, listener);
    }

    private void ReadCallback(IAsyncResult ar)
    {
        // Retrieve the state object and the handler socket
        var state = (StateObject?) ar.AsyncState;
        if (state == null) return;
        var handler = state.WorkSocket;
        // TODO 未完成用户映射
        _clients[(ulong)state.WorkSocket.Handle.ToInt64()] = handler;
        var bytesRead = handler.EndReceive(ar);
        if (bytesRead <= 0) return;
        state.Sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));
        // Check for end-of-file tag. If it is not there, read more data
        if (!handler.Connected) return;
        // Not all data received. Get more
        handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
    }
}