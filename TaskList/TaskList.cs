using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskList.Core;

namespace TaskList
{
	public sealed class TaskList
	{
		private const string QUIT = "quit";
		public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

		private readonly IDictionary<string, IList<Task>> tasks = new Dictionary<string, IList<Task>>();
		private readonly IConsole console;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this.console = console;
		}

		public void Run()
		{
			console.WriteLine(startupText);
			while (true) {
				console.Write("> ");
				var command = console.ReadLine();
				if (command == QUIT) {
					break;
				}
				Execute(command);
			}
		}

		private void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command) {
			case "show":
				Show();
				break;
			case "add":
				Add(commandRest[1]);
				break;
			case "check":
				Check(commandRest[1]);
				break;
			case "uncheck":
				Uncheck(commandRest[1]);
				break;
			case "help":
				Help();
				break;
			case "deadline":
				Deadline(commandRest[1]);
				break;
            case "today":
                Today();
                break;
			case "view-by-deadline":
				ViewByDeadline();
				break;
            default:
				Error(command);
				break;
			}
		}

        private void Show()
		{
			foreach (var project in tasks) {
				console.WriteLine(project.Key);
				foreach (var task in project.Value) {
					console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
				}
				console.WriteLine();
			}
		}

		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
			var subcommand = subcommandRest[0];
			if (subcommand == "project") {
				AddProject(subcommandRest[1]);
			} else if (subcommand == "task") {
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				AddTask(projectTask[0], projectTask[1]);
			}
		}

		private void AddProject(string name)
		{
			tasks[name] = new List<Task>();
		}

		private void AddTask(string project, string description)
		{
			if (!tasks.TryGetValue(project, out IList<Task> projectTasks))
			{
				Console.WriteLine("Could not find a project with the name \"{0}\".", project);
				return;
			}
			projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
		}

		private void Check(string idString)
		{
			SetDone(idString, true);
		}

		private void Uncheck(string idString)
		{
			SetDone(idString, false);
		}

		private void SetDone(string idString, bool done)
		{
			int id = int.Parse(idString);
			var identifiedTask = tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == id))
				.Where(task => task != null)
				.FirstOrDefault();
			if (identifiedTask == null) {
				console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void Help()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  add project <project name>");
			console.WriteLine("  add task <project name> <task description>");
			console.WriteLine("  check <task ID>");
			console.WriteLine("  uncheck <task ID>");
            console.WriteLine("  deadline <task ID> <date in dd-MM-yyyy>");
            console.WriteLine("  today");
            console.WriteLine("  view-by-deadline");

        }

        private void Deadline(string idString)
		{
			var subcommandRest = idString.Split(" ".ToCharArray(), 2);
			int taskId = int.Parse(subcommandRest[0]);
			string date = subcommandRest[1];

			DateTime datetime = DateTime.ParseExact(date, "dd-MM-yyyy", null);

			Task? task = GetTask(taskId);

			if (task != null)
			{
				task.Deadline = datetime;
				console.WriteLine("Added dealine: \"{0}\" to task with id \"{1}\"", datetime, taskId);                
            }
			else
			{
				console.WriteLine("There is no task with id \"{0}\"", taskId);
			}
		}
		
		private void Today()
		{
            DateTime today = DateTime.Today;

			var tasksWithTodayDeadline = tasks
				.SelectMany(project => project.Value, (project, task) => new { ProjectName = project.Key, Task = task })
				.Where(x => x.Task.Deadline.Date == today)
				.ToList();

            // Output the matching tasks with project name
            foreach (var item in tasksWithTodayDeadline)
            {
                Console.WriteLine("Project: \"{0}\", Task: \"{1}\"", item.ProjectName, item.Task.Description);
            }
        }

        private void ViewByDeadline()
        {
            IDictionary<DateTime, List<Task>> tasksByDeadline = new Dictionary<DateTime, List<Task>>();

            tasksByDeadline = tasks
                .SelectMany(project => project.Value)
                .GroupBy(task => task.Deadline.Date)
				.OrderBy(group => group.Key == DateTime.MinValue ? DateTime.MaxValue : group.Key) //if it the min-value (default datetime value), treat it as latest possible date. else treat is as normal.
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var group in tasksByDeadline)
            {
                DateTime deadline = group.Key;

                if (deadline == DateTime.MinValue)
                {
                    Console.WriteLine("No deadline:");
                }
                else
                {
                    Console.WriteLine($"{deadline.ToString("dd-MM-yyyy")}:");
                }

                foreach (var task in group.Value)
                {
                    Console.WriteLine($"\t{task.Id}: {task.Description}");
                }
            }
        }

        private void Error(string command)
		{
			console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private long NextId()
		{
			return ++lastId;
		}

		private Task? GetTask(int idTask)
		{
            //Gets the first task found with the given id.
            Task? task = tasks
				.SelectMany(project => project.Value)
				.FirstOrDefault(task => task.Id == idTask);

            return task;
        }
    }
}
