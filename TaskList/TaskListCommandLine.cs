using TaskList.Core;

namespace TaskList
{
	public sealed class TaskListCommandLine
	{
		private const string QUIT = "quit";
		public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

		private readonly ITaskManager taskManager;
		private readonly IConsole console;

		public static void Main(string[] args)
		{
			new TaskListCommandLine(new TaskManager(), new RealConsole()).Run();
		}

		public TaskListCommandLine( ITaskManager manager, IConsole console)
		{
			this.console = console;
			taskManager = manager;
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
				taskManager.CheckTask(int.Parse(commandRest[1]));
                break;
			case "uncheck":
				taskManager.UncheckTask(int.Parse(commandRest[1]));
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
			foreach (var project in taskManager.GetAllTasks()) {
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
                taskManager.AddProject(subcommandRest[1]);
            }
            else if (subcommand == "task") {
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
                taskManager.AddTask(projectTask[0], projectTask[1]);
            }
		}

		private void AddTask(string project, string description)
		{
			if (!taskManager.AddTask(project, description).Success) 
			{
                Console.WriteLine("Could not find a project with the name \"{0}\".", project);
            }
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

			TaskManagerResult result = taskManager.AddDeadlineToTask(taskId, datetime);

			if (result.Success)
			{
				console.WriteLine("Added dealine: \"{0}\" to task with id \"{1}\"", datetime, taskId);                
            }
			else
			{
				console.WriteLine(result.Message);
			}
		}
		
		private void Today()
		{
            foreach (Task task in taskManager.GetTasksOfToday())
            {
                Console.WriteLine("Project: \"{0}\", Task: \"{1}\"", taskManager.GetProjectNameOfTask(task.Id), task.Description);
            }
        }

        private void ViewByDeadline()
        {

            foreach (var group in taskManager.GetTasksByDeadline())
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
    }
}
