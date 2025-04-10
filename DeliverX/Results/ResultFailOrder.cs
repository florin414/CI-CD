using FluentResults;

namespace DeliverX.Results;
public class ResultFailOrder : ResultFail, IError
{
    public enum OperationId
    {
        OrderNotFound,
    }

    private Guid OrderId { get; init; }

    private string Message { get; init; }

    private string Operation { get; init; }

    public List<IError> Reasons => [new Error(Message)];

    public Dictionary<string, object> Metadata => new()
    {
        { "OrderId", OrderId },
        { "Timestamp", DateTime.UtcNow },
        { "OperationId", Operation }
    };

    public ResultFailOrder(Guid orderId, string message, OperationId operationId)
        : base(message, operationId.ToString())
    {
        OrderId = orderId;
        Message = message;
        Operation = operationId.ToString();
    }
}
