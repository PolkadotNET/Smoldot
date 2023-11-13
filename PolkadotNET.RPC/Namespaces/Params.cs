using ServiceStack.Text;

namespace PolkadotNET.RPC.Namespaces;

public record Params(string Subscription, JsonObject Result);