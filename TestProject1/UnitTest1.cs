using SmoldotNET.Connection;

namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That(new ConnectionType(0).IpType, Is.EqualTo(IpType.IpV4));
        Assert.That(new ConnectionType(0).Protocol, Is.EqualTo(Protocol.TcpIp));
        Assert.That(new ConnectionType(1).IpType, Is.EqualTo(IpType.IpV6));
        Assert.That(new ConnectionType(1).Protocol, Is.EqualTo(Protocol.TcpIp));
        Assert.That(new ConnectionType(2).IpType, Is.EqualTo(IpType.Domain));
        Assert.That(new ConnectionType(2).Protocol, Is.EqualTo(Protocol.TcpIp));
        
        Assert.That(new ConnectionType(4).IpType, Is.EqualTo(IpType.IpV4));
        Assert.That(new ConnectionType(4).Protocol, Is.EqualTo(Protocol.WebSocket));
        Assert.That(new ConnectionType(5).IpType, Is.EqualTo(IpType.IpV6));
        Assert.That(new ConnectionType(5).Protocol, Is.EqualTo(Protocol.WebSocket));
        Assert.That(new ConnectionType(6).IpType, Is.EqualTo(IpType.Domain));
        Assert.That(new ConnectionType(6).Protocol, Is.EqualTo(Protocol.WebSocket));
        
        Assert.That(new ConnectionType(14).IpType, Is.EqualTo(IpType.Domain));
        Assert.That(new ConnectionType(14).Protocol, Is.EqualTo(Protocol.SecureWebSocket));
        Assert.That(new ConnectionType(16).IpType, Is.EqualTo(IpType.IpV4));
        Assert.That(new ConnectionType(16).Protocol, Is.EqualTo(Protocol.WebRTC));
        Assert.That(new ConnectionType(17).IpType, Is.EqualTo(IpType.IpV6));
        Assert.That(new ConnectionType(17).Protocol, Is.EqualTo(Protocol.WebRTC));
    }
}