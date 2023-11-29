using System.Collections.Concurrent;
using System.Net.Sockets;
using NLog;

namespace PolkadotNET.Smoldot;

public class ConnectionManager : IConnectionManager
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly Queue<(int, string)> _connectionResets = new();
    private readonly Queue<(int, int)> _capacityUpdates = new();
    private readonly Queue<(int, byte[])> _inboundQueue = new();

    private readonly ConcurrentDictionary<int, NetworkConnection> _networkConnections = new();

    public void ResetConnection(int connectionId)
    {
        if (_networkConnections.TryRemove(connectionId, out var connection))
        {
            connection.CancellationTokenSource.Cancel();
            _logger.Info($"connection reset (by wasm) ({connectionId})");
        }
    }

    private readonly Queue<(int connectionId, string address, ushort port)> _newConnection = new();
    private async Task EnqueueConnection(int connectionId, string address, ushort port)
    {
        var tcpClient = new TcpClient();
        try
        {
            await tcpClient.ConnectAsync(address, port);

            var networkStream = tcpClient.GetStream();
            var tokenSource = new CancellationTokenSource();
            var connection = new NetworkConnection(networkStream, tokenSource, connectionId);

            connection.OnConnectionReset += (id) =>
            {
                if (_networkConnections.TryRemove(id, out var _))
                    _connectionResets.Enqueue((id, "generic reason"));
            };
            connection.OnDataWritten += (id, len) => _capacityUpdates.Enqueue((id, len));
            connection.OnDataReceived += (id, data) => _inboundQueue.Enqueue((id, data));
            
            _networkConnections.TryAdd(connectionId, connection);
            _capacityUpdates.Enqueue((connectionId, NetworkConnection.MaxWriteBufferSize));
            _logger.Info($"new network connection added: {address}:{port} ({connectionId})");
        }
        catch (Exception ex) when (
            ex is SocketException or ObjectDisposedException or InvalidOperationException
        )
        {
            _connectionResets.Enqueue((connectionId, "generic reason")); 
        }
    }

    public void CreateConnection(int connectionId, string address, ushort port)
    {
        _ = EnqueueConnection(connectionId, address, port);
        // var tcpClient = new TcpClient();
        // try
        // {
        //     tcpClient.Connect(address, port);
        //
        //     var networkStream = tcpClient.GetStream();
        //     var tokenSource = new CancellationTokenSource();
        //     var connection = new NetworkConnection(networkStream, tokenSource, connectionId);
        //
        //     connection.OnConnectionReset += (id) => _connectionResets.Enqueue((id, "generic reason"));
        //     connection.OnDataWritten += (id, len) => _capacityUpdates.Enqueue((id, len));
        //     connection.OnDataReceived += (id, data) => _inboundQueue.Enqueue((id, data));
        //     
        //     _networkConnections.Add(connectionId, connection);
        //     _capacityUpdates.Enqueue((connectionId, NetworkConnection.MaxWriteBufferSize));
        //     _logger.Info($"new network connection added: {address}:{port} ({connectionId})");
        // }
        // catch (Exception ex) when (
        //     ex is SocketException or ObjectDisposedException or InvalidOperationException
        // )
        // {
        //     _connectionResets.Enqueue((connectionId, "generic reason")); 
        // }
    }

    public void QueueOutbound(int connectionId, byte[] data)
    {
        if (!_networkConnections.TryGetValue(connectionId, out var connection))
            throw new Exception("connection not found");

        connection.Enqueue(data);
    }

    public IEnumerable<(int, string)> ResetConnections()
    {
        while (_connectionResets.TryDequeue(out var reset))
            yield return reset;
    }

    public IEnumerable<(int, int)> StreamCapacityUpdates() 
    {
        while (_capacityUpdates.TryDequeue(out var capacity))
            yield return capacity;
    }

    public IEnumerable<(int, byte[])> IncomingData()  
    {
        while (_inboundQueue.TryDequeue(out var inbound))
            yield return inbound;
    }
}