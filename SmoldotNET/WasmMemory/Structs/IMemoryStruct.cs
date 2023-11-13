using Wasmtime;

namespace SmoldotNET.WasmMemory.Structs;

public interface IMemoryStruct
{
    void ReadFrom(Memory memory, int addrPointer, int addrLength);
}