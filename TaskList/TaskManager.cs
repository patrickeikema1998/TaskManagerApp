using TaskList.Core;

namespace TaskList
{
    public class TaskManager : ITaskManager
    {
        public IDictionary<string, IList<Task>> tasks;
        private long lastId = 0;


        public TaskManager()
        {
            tasks = new Dictionary<string, IList<Task>>();
        }

        public IDictionary<string, IList<Task>> GetAllTasks() => tasks;

        public TaskManagerResult AddProject(string name)
        {
            if (string.IsNullOrEmpty(name))
                return TaskManagerResult.FailureResult("Name is empty.");

            tasks[name] = new List<Task>();
            return TaskManagerResult.SuccessResult();
        } 

        public TaskManagerResult AddTask(string project, string description)
        {
            if (!tasks.TryGetValue(project, out IList<Task> projectTasks))
            {
                return TaskManagerResult.FailureResult("Project is not found.");
            }
            projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
            return TaskManagerResult.SuccessResult();
        }

        public void ClearTasks()
        {
            tasks.Clear();
            lastId = 0;
        }

        private long NextId() => ++lastId;

        public TaskManagerResult CheckTask(long taskId) => SetDone(taskId, true);


        public TaskManagerResult UncheckTask(long taskId) => SetDone(taskId, false);

        public Task? GetTask(long taskId)
        {
            Task? task = tasks
                .SelectMany(project => project.Value)
                .FirstOrDefault(task => task.Id == taskId);

            return task;
        }

        public IDictionary<DateTime, List<Task>> GetTasksByDeadline()
        {
            return tasks
                .SelectMany(project => project.Value)
                .GroupBy(task => task.Deadline.Date)
                .OrderBy(group => group.Key == DateTime.MinValue ? DateTime.MaxValue : group.Key) //if it the min-value (default datetime value), treat it as latest possible date. else treat is as normal.
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public List<Task> GetTasksOfToday()
        {
            DateTime today = DateTime.Today;

            return tasks
                .SelectMany(project => project.Value)
                .Where(task => task.Deadline.Date == today)
                .ToList();
        }

        public string GetProjectNameOfTask(long taskId)
        {
            return tasks
                   .FirstOrDefault(project => project.Value.Any(task => task.Id == taskId))
                   .Key;
        }

        public TaskManagerResult SetDone(long taskId, bool done)
        {
            var identifiedTask = tasks
                .Select(project => project.Value.FirstOrDefault(task => task.Id == taskId))
                .Where(task => task != null)
                .FirstOrDefault();

            if (identifiedTask == null)
            {
                return TaskManagerResult.FailureResult("Task with given id is not found.");
            }

            identifiedTask.Done = done;
            return TaskManagerResult.SuccessResult();
        }

        public TaskManagerResult SetDeadlineOfTask(long taskId, DateTime deadline)
        {
            Task? task = GetTask(taskId);

            if (task != null)
            {
                task.Deadline = deadline;
                return TaskManagerResult.SuccessResult();
            }
            else
            {
                return TaskManagerResult.FailureResult("Task with given id is not found.");
            }
        }

        public TaskManagerResult SetDeadlineOfTask(string projectId, long taskId, DateTime deadline)
        {
            if (!tasks.TryGetValue(projectId, out IList<Task> projectTasks))
            {
                return TaskManagerResult.FailureResult("Project is not found.");
            }

            Task? task = GetTask(taskId);

            if (task != null)
            {
                task.Deadline = deadline;
            }
            else
            {
                return TaskManagerResult.FailureResult("Task is not found.");
            }

            return TaskManagerResult.SuccessResult();
        }
    }
}
