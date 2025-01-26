﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaskList.Core;

namespace TaskList
{
    /// <summary>
    /// The command-line interface for managing tasks and projects.
    /// </summary>
    public sealed class TaskListCommandLine
    {
        private const string QUIT = "quit"; // Command to exit the application.
        public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly ITaskManager taskManager;
        private readonly IConsole console;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            new TaskListCommandLine(new TaskManager(), new RealConsole()).Run();
        }

        /// <summary>
        /// Initializes the TaskListCommandLine with a task manager and console interface.
        /// </summary>
        public TaskListCommandLine(ITaskManager manager, IConsole console)
        {
            this.console = console;
            taskManager = manager;
        }

        /// <summary>
        /// Runs the command-line interface, accepting user commands in a loop.
        /// </summary>
        public void Run()
        {
            console.WriteLine(startupText);

            while (true)
            {
                console.Write("> ");
                var command = console.ReadLine();
                if (command == QUIT) // Exit loop if the command is 'quit'.
                {
                    break;
                }
                Execute(command); // Process the command.
            }
        }

        /// <summary>
        /// Parses and executes the given command line.
        /// </summary>
        private void Execute(string commandLine)
        {
            var commandRest = commandLine.Split(" ".ToCharArray(), 2);
            var command = commandRest[0]; // Extract the command.

            switch (command)
            {
                case "show": // Display all tasks and projects.
                    Show();
                    break;
                case "add": // Add a project or task.
                    Add(commandRest[1]);
                    break;
                case "check": // Mark a task as done.
                    taskManager.CheckTask(int.Parse(commandRest[1]));
                    break;
                case "uncheck": // Mark a task as not done.
                    taskManager.UncheckTask(int.Parse(commandRest[1]));
                    break;
                case "help": // Show available commands.
                    Help();
                    break;
                case "deadline": // Set a deadline for a task.
                    Deadline(commandRest[1]);
                    break;
                case "today": // Show tasks with today's deadline.
                    Today();
                    break;
                case "view-by-deadline": // Group and display tasks by deadline and project.
                    ViewByDeadline();
                    break;
                default: // Handle unknown commands.
                    Error(command);
                    break;
            }
        }

        /// <summary>
        /// Displays all tasks grouped by project.
        /// </summary>
        private void Show()
        {
            foreach (var project in taskManager.GetAllTasks())
            {
                console.WriteLine(project.Key); // Print project name.
                foreach (var task in project.Value)
                {
                    console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
                }
                console.WriteLine();
            }
        }

        /// <summary>
        /// Adds a new project or task based on the input command.
        /// </summary>
        private void Add(string commandLine)
        {
            var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
            var subcommand = subcommandRest[0];

            if (subcommand == "project") // Add a new project.
            {
                taskManager.AddProject(subcommandRest[1]);
            }
            else if (subcommand == "task") // Add a new task to a project.
            {
                var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
                taskManager.AddTask(projectTask[0], projectTask[1]);
            }
        }

        /// <summary>
        /// Displays help text for all available commands.
        /// </summary>
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

        /// <summary>
        /// Sets the deadline for a specific task.
        /// </summary>
        private void Deadline(string idString)
        {
            var subcommandRest = idString.Split(" ".ToCharArray(), 2);
            int taskId = int.Parse(subcommandRest[0]);
            string date = subcommandRest[1];

            DateTime datetime = DateTime.ParseExact(date, "dd-MM-yyyy", null);
            TaskManagerResult result = taskManager.SetDeadlineOfTask(taskId, datetime);

            if (result.Success)
            {
                console.WriteLine("Added deadline: \"{0}\" to task with id \"{1}\"", datetime.ToString("dd-MM-yyyy"), taskId);
            }
            else
            {
                console.WriteLine(result.Message);
            }
        }

        /// <summary>
        /// Displays tasks with today's deadline.
        /// </summary>
        private void Today()
        {
            foreach (Task task in taskManager.GetTasksOfToday())
            {
                Console.WriteLine("Project: \"{0}\", Task: \"{1}\"", taskManager.GetProjectNameOfTask(task.Id), task.Description);
            }
        }

        /// <summary>
        /// Displays tasks grouped by deadline and then by project.
        /// </summary>
        private void ViewByDeadline()
        {
            foreach (var group in taskManager.GetTasksByDeadline())
            {
                DateTime deadline = group.Key;

                var taskPerProject = taskManager.GroupTasksByProject(group.Value);

                /*writes the deadline to the console.
                  If the date is not set, we print no deadline.*/
                if (deadline == DateTime.MinValue)
                {
                    Console.WriteLine("No deadline:");
                }
                else
                {
                    Console.WriteLine($"{deadline.ToString("dd-MM-yyyy")}:");
                }

                // Prints the group and its task that comes with this group.
                foreach (var projectGroup in taskPerProject)
                {
                    Console.WriteLine($"\t{projectGroup.Key}:"); // Print project name.

                    foreach (var task in projectGroup.Value)
                    {
                        Console.WriteLine($"\t\t{task.Id}: {task.Description}");
                    }
                }
            }
        }

        /// <summary>
        /// Prints an error message for unknown commands.
        /// </summary>
        private void Error(string command)
        {
            console.WriteLine("I don't know what the command \"{0}\" is.", command);
        }
    }
}
