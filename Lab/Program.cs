// See https://aka.ms/new-console-template for more information

using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.ChainHead;
using PolkadotNET.RPC.Services.ChainSpec;
using SmoldotNET.Smoldot;

try
{
    var cts = new CancellationTokenSource();

    var smoldot = new Smoldot("./smoldot_light_wasm.wasm");
    var runTask = smoldot.Run(cts.Token);

    var chain = smoldot.AddChain(File.ReadAllText("./polkadot.json"));
    var rpcClient = new SmoldotJsonRpcClient(chain);

    var rpc = new ChainSpecService(rpcClient);
    var name = await rpc.ChainName();
    
    Console.WriteLine(name);
    await runTask;
    Console.ReadKey();
    cts.Cancel();
    Console.WriteLine("Bye!");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}