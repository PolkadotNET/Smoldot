namespace PolkadotNET.Smoldot;

public interface IMemoryTable
{
    byte[] GetBuffer(int index);
    void Release(int tableIndex);
    int Allocate(byte[] buffer);
}

public class MemoryTable : IMemoryTable
{
    private readonly List<byte[]> _table = new();
    private readonly List<int> _allocations = new();
    
    public byte[] GetBuffer(int index) => _table[index];

    public void Release(int tableIndex)
    {
        _allocations.RemoveAll(a => a == tableIndex);
    }
    
    public int Allocate(byte[] buffer)
    {
        // check for previous allocations and use them instead if there are any
        for (var i = 0; i < _allocations.Count; i++)
        {
            var allocation = _allocations[i];
            if (allocation > i)
            {
                // we have a previously released allocation, use that instead
                _allocations.Insert(i, i);
                return i;
            }
        }

        // create a brand-new allocation
        _table.Add(buffer);
        _allocations.Add(_table.Count - 1);
        return _allocations.Count - 1;
    }
}