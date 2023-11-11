using System.Runtime.InteropServices;
using Wasmtime;

namespace wasm_test;

public static class WasmtimeExtensions
{
    public const string MemoryName = "memory";

    public static string ReadStringFromMemory(this Caller caller, int ptr, int len) =>
        caller.GetMemory(MemoryName)!.ReadString(ptr, len);

    public static void WriteByteArray(this Caller caller, long target, byte[] data) =>
        data.CopyTo(caller.GetMemory(MemoryName)!.GetSpan(target, data.Length));
}