// See https://aka.ms/new-console-template for more information

using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.ChainHead;
using SmoldotNET.Smoldot;

try
{
    var cts = new CancellationTokenSource();

    var smoldot = new Smoldot("./smoldot_light_wasm.wasm");
    var runTask = smoldot.Run(cts.Token);

    var westend = smoldot.AddChain(File.ReadAllText("./polkadot.json"));
    var rpcClient = new SmoldotJsonRpcClient(westend);
    rpcClient.OnNotification += Console.WriteLine;

    var chainHeadService = new ChainHeadService(rpcClient);
    var sId = await chainHeadService.FollowSubscription(new FollowParameters(false));


    Console.WriteLine(sId);
    await runTask;
    Console.ReadKey();
    cts.Cancel();
    Console.WriteLine("Bye!");

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}