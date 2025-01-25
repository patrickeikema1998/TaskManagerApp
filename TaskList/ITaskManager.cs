namespace TaskList.Core
{
    public interface ITaskManager
    {
        IDictionary<string, IList<Task>> GetAllTasks();
        TaskManagerResult AddTask(string project, string description);
        TaskManagerResult AddProject(string name);
        Task? GetTask(long taskId);
        TaskManagerResult CheckTask(long taskId);
        TaskManagerResult UncheckTask(long taskId);
        TaskManagerResult SetDone(long idString, bool done);
        TaskManagerResult AddDeadlineToTask(long taskId, DateTime deadline);
        List<Task> GetTasksOfToday();
        IDictionary<DateTime, List<Task>> GetTasksByDeadline();
        string GetProjectNameOfTask(long taskId);
    }
}
