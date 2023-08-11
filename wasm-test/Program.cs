// See https://aka.ms/new-console-template for more information
//

using System.Runtime.InteropServices;
using wasm_test;
using wasm_test.WasmInstance;
using Wasmtime;

Console.WriteLine("Hello, World!");
using var engine = new Engine();
using var module = Module.FromFile(engine, "./smoldot_light_wasm.wasm");
using var linker = new Linker(engine);
using var store = new Store(engine);

const string FunctionName = "init";
const string ModuleName = "smoldot";
const string MemoryName = "memory";

var instance = new WasmInstance();
instance.Init(5);

await Task.Delay(5000);
instance.AdvanceExecution();

Console.ReadKey();