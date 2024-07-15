namespace NailStore.Core.Models;

public class EmailServiceResultModel
{
    public bool IsSending { get; set; }
    public string MessageResultSending { get; set; }
    public short StatusCode { get; set; }
}
