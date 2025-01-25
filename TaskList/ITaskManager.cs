namespace TaskList.Core
{
    public interface ITaskManager
    {
        void AddTask(string project, string description);
        Task GetTask(int taskId);
        void CheckTask(int taskId);
        void UncheckTask(int taskId);
        List<Task> GetTasksOfToday();
        Dictionary<DateTime, List<Task>> GetTasksByDeadline();
    }
}
