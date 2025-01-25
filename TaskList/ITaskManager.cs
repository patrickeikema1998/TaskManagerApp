namespace TaskList.Core
{
    public interface ITaskManager
    {
        TaskManagerResult AddTask(string project, string description);
        TaskManagerResult AddProject(string name);
        Task? GetTask(int taskId);
        TaskManagerResult CheckTask(int taskId);
        TaskManagerResult UncheckTask(int taskId);
        TaskManagerResult SetDone(int idString, bool done);
        TaskManagerResult AddDeadlineToTask(int taskId, DateTime deadline);
        List<Task> GetTasksOfToday();
        IDictionary<DateTime, List<Task>> GetTasksByDeadline();
    }
}
