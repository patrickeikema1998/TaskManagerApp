using System;
using System.Threading.Tasks;
using TaskList.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskList
{
    public class TaskManager : ITaskManager
    {
        private readonly IDictionary<string, IList<Task>> tasks;
        private long lastId = 0;


        public TaskManager()
        {
            tasks = new Dictionary<string, IList<Task>>();
        }

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

        private long NextId() => ++lastId;

        public TaskManagerResult CheckTask(int taskId) => SetDone(taskId, true);


        public TaskManagerResult UncheckTask(int taskId) => SetDone(taskId, false);

        public Task? GetTask(int taskId)
        {
            Task? task = tasks
                .SelectMany(project => project.Value)
                .FirstOrDefault(task => task.Id == taskId);

            return task;
        }

        public IDictionary<DateTime, List<Task>> GetTasksByDeadline()
        {
            IDictionary<DateTime, List<Task>> tasksByDeadline = new Dictionary<DateTime, List<Task>>();

            tasksByDeadline = tasks
                .SelectMany(project => project.Value)
                .GroupBy(task => task.Deadline.Date)
                .OrderBy(group => group.Key == DateTime.MinValue ? DateTime.MaxValue : group.Key) //if it the min-value (default datetime value), treat it as latest possible date. else treat is as normal.
                .ToDictionary(group => group.Key, group => group.ToList());

            return tasksByDeadline;
        }

        public List<Task> GetTasksOfToday()
        {
            throw new NotImplementedException();
        }

        public TaskManagerResult SetDone(int taskId, bool done)
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

        public TaskManagerResult AddDeadlineToTask(int taskId, DateTime deadline)
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
    }
}
