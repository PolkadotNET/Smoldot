// See https://aka.ms/new-console-template for more information

using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.ChainHead;
using SmoldotNET.Smoldot;

var cts = new CancellationTokenSource();

var smoldot = new Smoldot("./smoldot_light_wasm.wasm");
_ = smoldot.Run(cts.Token);

var westend = smoldot.AddChain(File.ReadAllText("./westend.json"));

var rpcClient = new SmoldotJsonRpcClient(westend);
rpcClient.OnNotification += Console.WriteLine;

var chainHeadService = new ChainHeadService(rpcClient);
var sId = await chainHeadService.FollowSubscription(new FollowParameters(false));


Console.WriteLine(sId);

Console.ReadKey();
cts.Cancel();
Console.WriteLine("Bye!");
