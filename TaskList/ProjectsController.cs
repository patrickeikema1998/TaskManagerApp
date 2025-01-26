using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TaskList.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskList
{
    [Route("projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly TaskManager taskManager;

        public ProjectsController(TaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        [HttpPost]
        public IActionResult CreateProject([FromBody] string projectName)
        {
            var result = taskManager.AddProject(projectName);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(CreateProject), new { projectName });
        }

        // A model for the CreateTask action
        public class CreateTaskRequest
        {
            public string ProjectId { get; set; }
            public string TaskName { get; set; }
        }

        [HttpPost("{projectId}/tasks")]
        public IActionResult CreateTask(CreateTaskRequest taskRequest)
        {
            var result = taskManager.AddTask(taskRequest.ProjectId, taskRequest.TaskName);

            if (!result.Success)
            {
                return BadRequest(result.Message);

            }
            return Ok();
        }

        [HttpPut("{projectId}/tasks/{taskId}")]
        public IActionResult UpdateTaskDeadline(
            string projectId,
        long taskId,
            string deadline)
        {
            TaskManagerResult result = taskManager.SetDeadlineOfTask(projectId, taskId, DateTime.ParseExact(deadline, "dd-MM-yyyy", null));

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok($"Deadline for task {taskId} updated to {deadline}.");
        }

        [HttpGet("view_by_deadline")]
        public IActionResult GetTasksGroupedByDeadline()
        {
            var tasksByDeadline = taskManager.GetTasksByDeadline();

            var result = new StringBuilder();

            foreach (var group in tasksByDeadline)
            {
                DateTime deadline = group.Key;

                if (deadline == DateTime.MinValue)
                {
                    result.AppendLine("No deadline:");
                }
                else
                {
                    result.AppendLine($"{deadline.ToString("dd-MM-yyyy")}:");
                }

                foreach (var task in group.Value)
                {
                    result.AppendLine($"{task.Id}: {task.Description}");
                }
            }

            return Ok(result.ToString()); // Return the formatted string
        }
    }
}

