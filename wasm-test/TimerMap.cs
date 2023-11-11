namespace wasm_test;

public class TimerMap
{
    private readonly List<double> _timers = new();
    
    public void Add(double duration)
    {
        Console.WriteLine($"Added new timer that will elapse at {duration}");
        _timers.Add(duration);
    }

    public bool HasElapsed()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var elapsedAmount =  _timers.RemoveAll(t => now >= t);

        if (elapsedAmount > 0)
        {
            Console.WriteLine($"{elapsedAmount} timers have elapsed");
        }
        
        return elapsedAmount > 0;
    }
}