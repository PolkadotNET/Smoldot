namespace PolkadotNET.Smoldot;

public interface ITimerMap
{
    void Add(double duration);
    bool HasElapsed();
}

public class TimerMap : ITimerMap
{
    private readonly List<double> _timers = new();
    
    public void Add(double duration)
    {
        _timers.Add(duration);
    }

    public bool HasElapsed()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var elapsedAmount =  _timers.RemoveAll(t => now >= t);
        return elapsedAmount > 0;
    }
}