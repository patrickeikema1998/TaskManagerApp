using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace TaskList
{
    /// <summary>
    /// API Controller for managing projects and tasks.
    /// </summary>
    [Route("projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly TaskManager taskManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsController"/> class.
        /// </summary>
        /// <param name="taskManager">The task manager instance.</param>
        public ProjectsController(TaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="projectName">The name of the project to create.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult CreateProject([FromBody] string projectName)
        {
            var result = taskManager.AddProject(projectName);

            if (!result.Success)
            {
                return BadRequest(result.Message); // Return a 400 Bad Request with the error message.
            }

            return CreatedAtAction(nameof(CreateProject), new { projectName }); // Return a 201 Created response.
        }

        /// <summary>
        /// Request model for creating a task.
        /// </summary>
        public class CreateTaskRequest
        {
            public string ProjectId { get; set; } // The ID of the project.
            public string TaskName { get; set; } // The name of the task.
        }

        /// <summary>
        /// Creates a new task within a specified project.
        /// </summary>
        /// <param name="taskRequest">The task creation request.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost("{projectId}/tasks")]
        public IActionResult CreateTask(CreateTaskRequest taskRequest)
        {
            var result = taskManager.AddTask(taskRequest.ProjectId, taskRequest.TaskName);

            if (!result.Success)
            {
                return BadRequest(result.Message); // Return a 400 Bad Request with the error message.
            }
            return Ok(); // Return a 200 OK response.
        }

        /// <summary>
        /// Updates the deadline of a task.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="deadline">The new deadline in "dd-MM-yyyy" format.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPut("{projectId}/tasks/{taskId}")]
        public IActionResult UpdateTaskDeadline(string projectId, long taskId, string deadline)
        {
            TaskManagerResult result = taskManager.SetDeadlineOfTask(projectId, taskId, DateTime.ParseExact(deadline, "dd-MM-yyyy", null));

            if (!result.Success)
            {
                return BadRequest(result.Message); // Return a 400 Bad Request with the error message.
            }
            return Ok($"Deadline for task {taskId} updated to {deadline}."); // Return a 200 OK with a success message.
        }

        /// <summary>
        /// Retrieves tasks grouped by their deadline per project.
        /// </summary>
        /// <returns>A formatted string of tasks grouped by deadline per project.</returns>
        [HttpGet("view_by_deadline")]
        public IActionResult GetTasksGroupedByDeadline()
        {
            var tasksByDeadlinePerProject = taskManager.GetTasksByDeadlinePerProject();

            var result = new StringBuilder();

            foreach (var group in tasksByDeadlinePerProject)
            {
                DateTime deadline = group.Key;

                if (deadline == DateTime.MinValue)
                {
                    result.AppendLine("No deadline:"); // Append "No deadline" for tasks without a deadline.
                }
                else
                {
                    result.AppendLine($"{deadline.ToString("dd-MM-yyyy")}:"); // Append the formatted deadline date.
                }

                foreach (var projectGroup in group.Value)
                {
                    result.AppendLine($"{projectGroup.Key}:"); // Print project name.

                    foreach (var task in projectGroup.Value)
                    {
                        result.AppendLine($"{task.Id}: {task.Description}");
                    }
                }
            }

            return Ok(result.ToString()); // Returns the formatted string.
        }
    }
}
