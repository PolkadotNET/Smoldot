using System.Runtime.InteropServices;
using Wasmtime;

namespace wasm_test;

public static class WasmtimeExtensions
{
    private const string MemoryName = "memory";
    
    public static string ReadStringFromMemory(this Caller caller, int ptr, int len) =>
        caller.GetMemory(MemoryName)!.ReadString(ptr, len);

    public static void WriteByteArray(this Caller caller, long target, byte[] data)
    {
        GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
        IntPtr pointer = pinnedArray.AddrOfPinnedObject();
        caller.GetMemory(MemoryName)!.WriteIntPtr(target, pointer);
        pinnedArray.Free();
    }
}