namespace MainApp.Models;

public class ResponseMessage
{
    public string Message { get; set; }
    public ResponseStatus Status { get; set; }

    public static ResponseMessage Error(string message)
    {
        return new ResponseMessage
        {
            Message = message,
            Status = ResponseStatus.Error
        };
    }

    public static ResponseMessage Success(string message)
    {
        return new ResponseMessage
        {
            Message = message,
            Status = ResponseStatus.Success
        };
    }

    public override string ToString()
    {
        return $"Status: {Status.ToString()}, Message: {Message}";
    }
}

public enum ResponseStatus
{
    None,
    Success,
    Error
}