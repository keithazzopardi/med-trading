namespace Med.Shared;

public class OperationResponse
{
    public bool IsSuccess { get; set; }
    public int? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public static OperationResponse CreateSuccess() => new OperationResponse
    {
        IsSuccess = true,
        ErrorCode = null,
    };
    
    public static OperationResponse CreateFailure(int errorCode, string? errorMessage = null) => new OperationResponse
    {
        IsSuccess = false,
        ErrorCode = errorCode, 
        ErrorMessage = errorMessage,
    };
}