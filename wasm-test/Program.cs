// See https://aka.ms/new-console-template for more information
//

using wasm_test.WasmInstance;
var cts = new CancellationTokenSource();

var instance = new WasmInstance();

instance.OnRpcResponse += Console.WriteLine;
var runTask = instance.Run(cts.Token);

// add chain etc :)
var chainId = instance.AddChain();

// await Task.Delay(10000);
instance.Send(chainId, "{\"id\":5,\"jsonrpc\":\"2.0\",\"method\":\"rpc_methods\",\"params\":[]}");

Console.ReadKey();
cts.Cancel();
await runTask;
Console.WriteLine("Bye!");


public class Chain
{
    public delegate void Received(string json);

    public event Received OnReceived;
    
    public void Send(string json)
    {
        
    }

    public void Remove()
    {
        
    }
}