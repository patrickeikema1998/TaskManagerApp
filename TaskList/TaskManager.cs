using TaskList.Core;

namespace TaskList
{
    /// <summary>
    /// Manages tasks and projects, providing functionality to add, update, retrieve, and group tasks by deadlines or projects.
    /// </summary>
    public class TaskManager : ITaskManager
    {
        /// <summary>
        /// Stores tasks grouped by their project names.
        /// </summary>
        public IDictionary<string, IList<Task>> tasks;

        /// <summary>
        /// Keeps track of the last used task ID to generate unique IDs.
        /// </summary>
        private long lastId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManager"/> class.
        /// </summary>
        public TaskManager()
        {
            tasks = new Dictionary<string, IList<Task>>();
        }

        /// <summary>
        /// Gets all tasks grouped by projects.
        /// </summary>
        /// <returns>A dictionary of project names and their associated tasks.</returns>
        public IDictionary<string, IList<Task>> GetAllTasks() => tasks;

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="name">The name of the project to add.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public TaskManagerResult AddProject(string name)
        {
            if (string.IsNullOrEmpty(name))
                return TaskManagerResult.FailureResult("Name is empty.");

            tasks[name] = new List<Task>();
            return TaskManagerResult.SuccessResult();
        }

        /// <summary>
        /// Adds a new task to a project.
        /// </summary>
        /// <param name="project">The name of the project.</param>
        /// <param name="description">The description of the task.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public TaskManagerResult AddTask(string project, string description)
        {
            if (!tasks.TryGetValue(project, out IList<Task> projectTasks))
            {
                return TaskManagerResult.FailureResult("Project is not found.");
            }

            projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
            return TaskManagerResult.SuccessResult();
        }

        /// <summary>
        /// Clears all tasks and resets the task ID counter.
        /// </summary>
        public void ClearTasks()
        {
            tasks.Clear();
            lastId = 0;
        }

        /// <summary>
        /// Generates the next unique task ID.
        /// </summary>
        /// <returns>The next task ID.</returns>
        private long NextId() => ++lastId;

        /// <summary>
        /// Marks a task as done.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public TaskManagerResult CheckTask(long taskId) => SetDone(taskId, true);

        /// <summary>
        /// Marks a task as not done.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public TaskManagerResult UncheckTask(long taskId) => SetDone(taskId, false);

        /// <summary>
        /// Retrieves a task by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <returns>The task if found; otherwise, null.</returns>
        public Task? GetTask(long taskId)
        {
            Task? task = tasks
                .SelectMany(project => project.Value)
                .FirstOrDefault(task => task.Id == taskId);

            return task;
        }

        /// <summary>
        /// Groups tasks by their deadlines. If a deadline is not set, its treated as the latest possible date so that its latest in the list.
        /// </summary>
        /// <returns>A dictionary where the key is the deadline date and the value is a list of tasks with that deadline.</returns>
        public IDictionary<DateTime, List<Task>> GetTasksByDeadline()
        {
            return tasks
                .SelectMany(project => project.Value)
                .GroupBy(task => task.Deadline.Date)
                .OrderBy(group => group.Key == DateTime.MinValue ? DateTime.MaxValue : group.Key) // Treat default date as the latest possible date.
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        /// <summary>
        /// Retrieves tasks that have today's date as their deadline.
        /// </summary>
        /// <returns>A list of tasks due today.</returns>
        public List<Task> GetTasksOfToday()
        {
            DateTime today = DateTime.Today;

            return tasks
                .SelectMany(project => project.Value)
                .Where(task => task.Deadline.Date == today)
                .ToList();
        }

        /// <summary>
        /// Retrieves the name of the project associated with a given task ID.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>The name of the project if found; otherwise, null.</returns>
        public string GetProjectNameOfTask(long taskId)
        {
            return tasks
                   .FirstOrDefault(project => project.Value.Any(task => task.Id == taskId))
                   .Key;
        }

        /// <summary>
        /// Sets the done status of a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="done">The desired done status.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
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

        /// <summary>
        /// Sets the deadline of a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="deadline">The deadline to set.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
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

        /// <summary>
        /// Sets the deadline of a task in a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="deadline">The deadline to set.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
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

        /// <summary>
        /// Groups a list of tasks by their associated project name.
        /// </summary>
        /// <param name="tasks">The list of tasks to group.</param>
        /// <returns>A dictionary where the key is the project name, and the value is a list of tasks belonging to that project.</returns>
        public IDictionary<string, List<Task>> GroupTasksByProject(List<Task> tasks)
        {
            return tasks
                .GroupBy(task => GetProjectNameOfTask(task.Id)) // Group tasks by project
                .OrderBy(group => group.Key) // Orders by project name
                .ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}
