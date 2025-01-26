using System.Diagnostics;
using TaskList;
using TaskList.Core;

namespace Tasks
{
	[TestFixture]
	public sealed class ApplicationTest
	{
		public const string PROMPT = "> ";

		private FakeConsole console;
		private System.Threading.Thread applicationThread;

		private readonly TaskManager taskManager = new TaskManager();
        private CancellationTokenSource cancellationTokenSource;


        [SetUp]
		public void StartTheApplication()
		{
			this.console = new FakeConsole();
            this.cancellationTokenSource = new CancellationTokenSource();
            var taskList = new TaskList.TaskListCommandLine(taskManager, console);
			this.applicationThread = new System.Threading.Thread(() => taskList.Run());
			applicationThread.Start();
			ReadLines(TaskList.TaskListCommandLine.startupText);
		}

		[TearDown]
		public void KillTheApplication()
		{
			if (applicationThread == null || !applicationThread.IsAlive)
			{
				return;
			}

            Execute("quit");

            // Signal the thread to stop by canceling the token
            cancellationTokenSource.Cancel();

            // Wait for the thread to finish
            applicationThread.Join();

            // Optionally, check if the thread has terminated successfully
            if (applicationThread.IsAlive)
            {
                throw new Exception("The application is still running.");
            }

            cancellationTokenSource.Dispose();
        }

        [Test, Timeout(1000)]
		public void TestAddProjectCommand()
		{
            taskManager.ClearTasks();
            Execute("add project secrets");
            Execute("show");
            ReadLines(
                "secrets",
				""
            );
        }

        [Test, Timeout(1000)]
        public void TestAddTaskCommand()
        {
            taskManager.ClearTasks();
            Execute("add project secrets");
			Execute("add task secrets Laundry");
            Execute("show");
            ReadLines(
                 "secrets",
                "    [ ] 1: Laundry",
                ""
            );
        }

        [Test, Timeout(1000)]
		public void TestCheckCommand()
		{
            taskManager.ClearTasks();
            Execute("add project secrets");
            Execute("add task secrets Laundry");
            Execute("check 1");
            Execute("show");
            ReadLines(
                 "secrets",
                "    [x] 1: Laundry",
                ""
            );
        }


		[Test, Timeout(1000)]
		public void TestDeadLineCommand()
		{
			taskManager.ClearTasks();
			Execute("add project secrets");
			Execute("add task secrets Laundry");
			Execute("deadline 1 25-01-2025");
			ReadLines("Added deadline: \"25-01-2025\" to task with id \"1\"");
		}

		[Test, Timeout(1000)]
		public void TestTodayCommand()
		{
			string today = DateTime.Now.ToString("dd-MM-yyyy");

			taskManager.ClearTasks();
			Execute("add project secrets");
			Execute("add task secrets Laundry");
			Execute($"deadline 1 {today}");
			Execute("today");
			ReadLines("Project: \"secrets\", Task: \"Laundry\"");
		}

		[Test, Timeout(1000)]
		public void TestViewByDeadlineCommand()
		{
			taskManager.ClearTasks();

			Execute("add project secrets");
			Execute("add task secrets Laundry");
			Execute("add task secrets Game");
			Execute("deadline 1 26-01-2025");
			Execute("view-by-deadline");
			ReadLines(
				"26-01-2025:",
                "        1: Laundry",
				"No deadline:",
                "        2: Game");
		}


		private void Execute(string command)
		{
			Read(PROMPT);
			Write(command);
		}

		private void Read(string expectedOutput)
		{
			var length = expectedOutput.Length;
			var actualOutput = console.RetrieveOutput(expectedOutput.Length);
			Assert.AreEqual(expectedOutput, actualOutput);
		}

		private void ReadLines(params string[] expectedOutput)
		{
			foreach (var line in expectedOutput)
			{
				Read(line + Environment.NewLine);
			}
		}

		private void Write(string input)
		{
			console.SendInput(input + Environment.NewLine);
		}
	}
}
