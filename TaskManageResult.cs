using System;

public class TaskManagerResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static Result SuccessResult() => new Result { Success = true };
    public static Result FailureResult(string message) => new Result { Success = false, Message = message };
}
