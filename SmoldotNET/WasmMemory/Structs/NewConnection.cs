using SmoldotNET.Connection;
using Wasmtime;

namespace SmoldotNET.WasmMemory.Structs;

public struct NewConnection : IMemoryStruct
{
    public ConnectionType Type { get; set; }
    public ushort Port { get; set; }
    public string AddressOrDomain { get; set; }

    public void ReadFrom(Memory memory, int addrPointer, int addrLength)
    {
        var data = memory.GetSpan(addrPointer, addrLength);

        Type = new ConnectionType(data[0]);
        Port = BitConverter.ToUInt16(data[1..3].ToArray().Reverse().ToArray());
        AddressOrDomain = memory.ReadString(addrPointer + 3, addrLength - 3);
    }
}