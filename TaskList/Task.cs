namespace TaskList
{
    /// <summary>
    /// Represents a task in the task list.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// The unique identifier for the task.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The description of the task.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A value indicating whether the task is completed.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// Tthe deadline for the task.
        /// </summary>
        public DateTime Deadline { get; set; }
    }
}
