namespace DeliverX.Results;
public class ResultFail
{
    public string Message { get; }
    public string Operation { get; }

    public ResultFail(string message, string operation)
    {
        Message = message;
        Operation = operation;
    }
}
