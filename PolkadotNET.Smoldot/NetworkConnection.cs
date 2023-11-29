using System.Net.Sockets;
using NLog;

namespace PolkadotNET.Smoldot;

public class NetworkConnection
{
    public const int MaxWriteBufferSize = 1024 * 1024;
    private const int MaxReadBufferSize = 1024 * 1024;

    public NetworkStream Stream { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }

    private readonly Queue<byte[]> _outgoingQueue = new();
    private int _occupied;
    private readonly int _id;
    private byte[] _readBuffer = new byte[MaxReadBufferSize];


    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public NetworkConnection(NetworkStream stream, CancellationTokenSource cancellationTokenSource, int id)
    {
        Stream = stream;
        CancellationTokenSource = cancellationTokenSource;
        _id = id;
        _occupied = 0;

        _ = ReadTask(cancellationTokenSource.Token);
        _ = WriteTask(cancellationTokenSource.Token);
    }

    public void Enqueue(byte[] data)
    {
        if (data.Length > RemainingCapacity())
            throw new Exception("capacity exceeded! (stream_writeable_bytes issue?)");

        _outgoingQueue.Enqueue(data);
        _occupied += data.Length;
    }

    public delegate void DataWritten(int id, int len);

    public delegate void ConnectionReset(int id);

    public delegate void DataReceived(int id, byte[] data);


    public event DataWritten OnDataWritten;
    public event ConnectionReset? OnConnectionReset;
    public event DataReceived? OnDataReceived;

    private Task WriteTask(CancellationToken ct)
    {
        return Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (_outgoingQueue.TryDequeue(out var data))
                {
                    _occupied -= data.Length;
                    await Stream.WriteAsync(data, ct);

                    // _logger.Debug($"{_id} (OUT, {data.Length}): {BitConverter.ToString(data)}");
                    OnDataWritten?.Invoke(_id, data.Length);
                }
                else
                {
                    await Task.Delay(1000, ct);
                }
            }
        }, ct);
    }


    private async Task ReadTask(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var len = await Stream.ReadAsync(_readBuffer, 0, MaxReadBufferSize, ct);
                if (len == 0)
                {
                    OnConnectionReset?.Invoke(_id);
                    _logger.Info($"network connection (id: {_id}) closed (EOF)");
                    // cancel so that the writeTask gets stopped as well.
                    CancellationTokenSource.Cancel();
                    return;
                }

                OnDataReceived?.Invoke(_id, _readBuffer[..len]);
            }
            catch (Exception ex) when (
                ex is IOException or ObjectDisposedException or InvalidOperationException
            )
            {
                OnConnectionReset?.Invoke(_id);
                _logger.Error($"network connection (id: {_id}) closed: {ex.Message}");

                // cancel so that the writeTask gets stopped as well.
                CancellationTokenSource.Cancel();
            }
        }
    }

    private int RemainingCapacity() => MaxWriteBufferSize - _occupied;
}