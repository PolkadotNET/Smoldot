namespace wasm_test.WasmInstance;

public class MemoryTable
{
    // 1MB
    private const int BUFFER_SIZE = 1024 * 1024;
    private readonly List<byte[]> _table = new();
    private readonly List<int> _allocations = new();
    
    public byte[] GetBuffer(int index) => _table[index];

    public void Release(int tableIndex)
    {
        _allocations.RemoveAll(a => a == tableIndex);
    }
    
    public int AllocateBuffer(byte[] buffer)
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