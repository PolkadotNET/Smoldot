using Wasmtime;

namespace PolkadotNET.Smoldot.WasmMemory.Structs;

public interface IMemoryStruct
{
    void ReadFrom(Memory memory, int addrPointer, int addrLength);
}