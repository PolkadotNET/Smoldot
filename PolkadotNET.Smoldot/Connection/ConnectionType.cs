namespace PolkadotNET.Smoldot.Connection;

public struct ConnectionType
{
    private const int IP_TYPE_SIZE = 2;
    private const int IP_TYPE_OFFSET = 0;
    private const ushort IP_TYPE_MASK = ((1 << IP_TYPE_SIZE) - 1) << IP_TYPE_OFFSET;

    private const int PROTOCOL_SIZE = 6;
    private const int PROTOCOL_OFFSET = IP_TYPE_OFFSET + IP_TYPE_SIZE;
    private const ushort PROTOCOL_MASK = ((1 << PROTOCOL_SIZE) - 1) << PROTOCOL_OFFSET;

    //(MSB)                                 (LSB)
    // | 07 | 06 | 05 | 04 | 03 | 02 | 01 | 00 |
    // |                   TYPE                |
    // |           PROTOCOL          |   IP    |

    private byte _value;

    public ConnectionType(byte connectionType)
    {
        _value = connectionType;
    }

    public IpType IpType
    {
        get => (IpType)((_value & IP_TYPE_MASK) >> IP_TYPE_OFFSET);
        set => _value = (byte)((_value & ~IP_TYPE_MASK) | (((int)value << IP_TYPE_OFFSET) & IP_TYPE_MASK));
    }

    public Protocol Protocol
    {
        get => (Protocol)((_value & PROTOCOL_MASK) >> PROTOCOL_OFFSET);
        set => _value = (byte)((_value & ~PROTOCOL_MASK) | (((int)value << PROTOCOL_OFFSET) & PROTOCOL_MASK));
    }
}