namespace PolkadotNET.Smoldot.Connection;

public enum IpType
{
    IpV4 = 0,
    IpV6 = 1,
    Domain = 1 << 1
}