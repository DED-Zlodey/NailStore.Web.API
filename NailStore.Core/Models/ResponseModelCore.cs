namespace NailStore.Core.Models;

public class ResponseModelCore<T>
{
    public ResponseHeaderCore Header { get; set; }
    public T Result { get; set; }
}
