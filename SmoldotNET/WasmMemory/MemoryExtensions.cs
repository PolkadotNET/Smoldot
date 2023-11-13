using SmoldotNET.WasmMemory.Structs;
using Wasmtime;

namespace SmoldotNET.WasmMemory;

public static class MemoryExtensions
{
    public const string MemoryName = "memory";

    public static T Read<T>(this Caller caller, int ptr, int len)
        where T: IMemoryStruct
    {
        var memory = caller.GetMemory(MemoryName)!;
        var instance = default(T);
        instance!.ReadFrom(memory, ptr, len);

        return instance;
    }

    public static Memory GetMemory(this Caller caller) => caller.GetMemory(MemoryName)!;
    
    public static string ReadStringFromMemory(this Caller caller, int ptr, int len) =>
        caller.GetMemory(MemoryName)!.ReadString(ptr, len);

    public static void WriteByteArray(this Caller caller, long target, byte[] data) =>
        data.CopyTo(caller.GetMemory(MemoryName)!.GetSpan(target, data.Length));
}