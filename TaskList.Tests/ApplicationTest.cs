using System.Diagnostics;
using TaskList;
using TaskList.Core;

namespace Tasks
{
    /// <summary>
    /// A test class for the application that runs commands and verifies outputs.
    /// </summary>
    [TestFixture]
    public sealed class ApplicationTest
    {
        public const string PROMPT = "> ";

        private FakeConsole console;
        private System.Threading.Thread applicationThread;

        private readonly TaskManager taskManager = new TaskManager();
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Sets up the application for testing by starting the application thread and initializing necessary components.
        /// </summary>
        [SetUp]
        public void StartTheApplication()
        {
            console = new FakeConsole();
            cancellationTokenSource = new CancellationTokenSource();
            var taskList = new TaskList.TaskListCommandLine(taskManager, console);
            applicationThread = new System.Threading.Thread(() => taskList.Run());
            applicationThread.Start();
            ReadLines(TaskList.TaskListCommandLine.startupText);
        }

        /// <summary>
        /// Cleans up the application after each test by terminating the application thread and disposing resources.
        /// </summary>
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

        /// <summary>
        /// Tests the 'add project' command by adding a project and verifying it shows up.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
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

        /// <summary>
        /// Tests the 'add task' command by adding a task to a project and verifying it shows up.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
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

        /// <summary>
        /// Tests the 'check' command by marking a task as completed and verifying the task status.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
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

        /// <summary>
        /// Tests the 'deadline' command by assigning a deadline to a task and verifying the output.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
        public void TestDeadLineCommand()
        {
            taskManager.ClearTasks();
            Execute("add project secrets");
            Execute("add task secrets Laundry");
            Execute("deadline 1 25-01-2025");
            ReadLines("Added deadline: \"25-01-2025\" to task with id \"1\"");
        }

        /// <summary>
        /// Tests the 'today' command by assigning a deadline and verifying if the task is shown correctly.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
        public void TestTodayCommand()
        {
            string today = DateTime.Now.ToString("dd-MM-yyyy");

            taskManager.ClearTasks();
            Execute("add project secrets");
            Execute("add task secrets Laundry");
            Execute($"deadline 1 {today}");
            ReadLines($"Added deadline: \"{today}\" to task with id \"1\"");
            Execute("today");
            ReadLines("Project: \"secrets\", Task: \"Laundry\"");

        }

        /// <summary>
        /// Tests the 'view-by-deadline' command by adding multiple tasks with deadlines and verifying the output.
        /// </summary>
        [Test, Timeout(1000), NonParallelizable]
        public void TestViewByDeadlineCommand()
        {
            taskManager.ClearTasks();
            Execute("add project secrets");
            Execute("add task secrets Laundry");
            Execute("add task secrets Game");
            Execute("deadline 1 26-01-2025");
            Read($"Added deadline: \"26-01-2025\" to task with id \"1\"" + Environment.NewLine);
            Execute("view-by-deadline");
            ReadLines(
                "26-01-2025:",
                "        1: Laundry",
                "No deadline:",
                "        2: Game");
        }

        /// <summary>
        /// Executes a command and simulates user input/output.
        /// </summary>
        private void Execute(string command)
        {
            Read(PROMPT);
            Write(command);
        }

        /// <summary>
        /// Reads and asserts the expected output from the console.
        /// </summary>
        private void Read(string expectedOutput)
        {
            var length = expectedOutput.Length;
            var actualOutput = console.RetrieveOutput(expectedOutput.Length);
            Console.WriteLine(expectedOutput);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        /// <summary>
        /// Reads multiple lines of expected output.
        /// </summary>
        private void ReadLines(params string[] expectedOutput)
        {
            foreach (var line in expectedOutput)
            {
                Read(line + Environment.NewLine);
            }
        }

        /// <summary>
        /// Simulates user input to the console.
        /// </summary>
        private void Write(string input)
        {
            console.SendInput(input + Environment.NewLine);
        }
    }
}
