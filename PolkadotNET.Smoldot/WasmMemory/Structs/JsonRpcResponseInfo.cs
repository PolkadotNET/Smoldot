﻿namespace PolkadotNET.Smoldot.WasmMemory.Structs;

public struct JsonRpcResponseInfo
{
    public uint Pointer { get; set; }
    public int Length { get; set; }
}