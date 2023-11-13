namespace SmoldotNET;

public interface IConnectionManager
{
    public IEnumerable<(int, string)> ResetConnections();
    public IEnumerable<(int, int)> StreamCapacityUpdates();
    public IEnumerable<(int, byte[])> IncomingData();

    public void CreateConnection(int connectionId, string address, ushort port);
    public void ResetConnection(int connectionId);
    public void QueueOutbound(int connectionId, byte[] data);
}