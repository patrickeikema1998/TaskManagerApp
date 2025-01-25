using System;

public class TaskManagerResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static TaskManagerResult SuccessResult() => new TaskManagerResult { Success = true };
    public static TaskManagerResult FailureResult(string message) => new TaskManagerResult { Success = false, Message = message };
}
