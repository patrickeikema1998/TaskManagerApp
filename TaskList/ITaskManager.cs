namespace TaskList.Core
{
    public interface ITaskManager
    {
        IDictionary<string, IList<Task>> GetAllTasks();
        TaskManagerResult AddTask(string project, string description);
        TaskManagerResult AddProject(string name);
        void ClearTasks();
        Task? GetTask(long taskId);
        TaskManagerResult CheckTask(long taskId);
        TaskManagerResult UncheckTask(long taskId);
        TaskManagerResult SetDone(long idString, bool done);
        TaskManagerResult SetDeadlineOfTask(long taskId, DateTime deadline);
        TaskManagerResult SetDeadlineOfTask(string projectId ,long taskId, DateTime deadline);
        List<Task> GetTasksOfToday();
        IDictionary<DateTime, List<Task>> GetTasksByDeadline();
        IDictionary<DateTime, IDictionary<string, List<Task>>> GetTasksByDeadlinePerProject();
        IDictionary<string, List<Task>> GroupTasksByProject(List<Task> tasks);
        string GetProjectNameOfTask(long taskId);
    }
}
