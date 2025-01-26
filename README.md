# Task Management Application

## Overview

This project provides a task management system that allows users to manage tasks and projects. The application supports adding tasks and projects, setting deadlines, marking tasks as completed, and viewing tasks grouped by their deadline. This repository contains both the backend API for managing the tasks and a test suite to ensure the functionality works as expected.

## File Structure

### 1. **API Controller (ProjectsController.cs)**

#### Location: `TaskList/ProjectsController.cs`

This file defines the API controller for managing projects and tasks through HTTP requests.

- **Endpoints**:
  - `POST /projects`: Creates a new project.
  - `POST /projects/{projectId}/tasks`: Creates a new task within a specified project.
  - `PUT /projects/{projectId}/tasks/{taskId}`: Updates the deadline of a task.
  - `GET /projects/view_by_deadline`: Retrieves tasks grouped by their deadline.

- **Main Functions**:
  - `CreateProject`: Handles project creation via the `POST /projects` endpoint.
  - `CreateTask`: Handles task creation via `POST /projects/{projectId}/tasks`.
  - `UpdateTaskDeadline`: Updates a task's deadline using the `PUT /projects/{projectId}/tasks/{taskId}` endpoint.
  - `GetTasksGroupedByDeadline`: Returns a list of tasks grouped by their deadline.

The controller interacts with the `TaskManager` class, which handles the business logic of managing projects and tasks.

### 2. **Task Manager Interface (ITaskManager.cs)**

This file defines the `ITaskManager` interface, which is used to abstract the task management operations in the application. It provides methods to add, get, update, and manage tasks and projects. The interface ensures that the task management logic can be easily tested and decoupled from other parts of the application.

#### Methods in the `ITaskManager` Interface:
- **`GetAllTasks()`**: Returns a dictionary of all tasks grouped by their project names.
- **`AddTask(string project, string description)`**: Adds a new task to a specified project with the provided description.
- **`AddProject(string name)`**: Adds a new project with the specified name.
- **`ClearTasks()`**: Clears all tasks from the system.
- **`GetTask(long taskId)`**: Retrieves a task by its ID.
- **`CheckTask(long taskId)`**: Marks a task as completed.
- **`UncheckTask(long taskId)`**: Marks a task as not completed.
- **`SetDone(long idString, bool done)`**: Sets a task's completion status.
- **`SetDeadlineOfTask(long taskId, DateTime deadline)`**: Sets a deadline for a task based on its task ID.
- **`SetDeadlineOfTask(string projectId, long taskId, DateTime deadline)`**: Sets a deadline for a task within a specific project.
- **`GetTasksOfToday()`**: Retrieves all tasks that are due today.
- **`GetTasksByDeadline()`**: Retrieves tasks grouped by their deadlines.
- **`GroupTasksByProject(List<Task> tasks)`**: Groups tasks by their project.
- **`GetProjectNameOfTask(long taskId)`**: Retrieves the project name of a specific task by its ID.

#### Purpose:
The purpose of the `ITaskManager` interface is to provide a set of common methods for managing tasks and projects. By defining the interface, it allows for flexibility in implementing the task management logic, making it easier to swap out different implementations or mock the functionality during testing.

This interface is typically implemented by the `TaskManager` class, which contains the actual logic for handling tasks and projects in the application.

### 3. ** Task Manager CommandLine(TaskListCommandLine.cs)**

This file defines the `TaskListCommandLine` class, which implements the command-line interface (CLI) for managing tasks and projects in the application. It processes user commands such as adding tasks, setting deadlines, viewing tasks, and displaying help. The CLI operates in a loop, accepting commands from the user and executing corresponding methods.

#### Methods:
- **`Execute(string commandLine)`**: Parses and executes the given command. Commands are processed using a switch statement, and the corresponding method is invoked.

  - **Supported Commands:**
    - **`show`**: Displays all tasks and projects, grouped by project name.
    - **`add`**: Adds a project or task. Subcommands include `add project <name>` and `add task <project name> <task description>`.
    - **`check`**: Marks a task as done by its ID.
    - **`uncheck`**: Marks a task as not done by its ID.
    - **`help`**: Displays a list of available commands.
    - **`deadline`**: Sets a deadline for a task.
    - **`today`**: Displays tasks with today’s deadline.
    - **`view-by-deadline`**: Displays tasks grouped by deadline and project.
      
- **`Show()`**: Displays all tasks grouped by project. Each task is displayed with its completion status (`[ ]` for incomplete, `[x]` for done).

- **`Add(string commandLine)`**: Adds a new project or task based on the input. It handles two subcommands: `project` (for adding a project) and `task` (for adding a task to a project).

- **`SetDone(string taskId, bool done)`**: Marks a task as done or undone, based on the provided `taskId` and the `done` status.

- **`Help()`**: Displays a list of all available commands and their syntax.

- **`Deadline(string idString)`**: Sets a deadline for a task. It accepts the task ID and a date in `dd-MM-yyyy` format. If the date format is incorrect, an error message is shown.

- **`Today()`**: Displays tasks that are due today, grouped by project.

- **`ViewByDeadline()`**: Displays tasks grouped by their deadlines, and further by project. If a task has no deadline, it is grouped under "No deadline."

- **`Error(string command)`**: Displays an error message for unknown or unsupported commands.

#### Purpose:
The `TaskListCommandLine` class serves as the command-line interface for interacting with the task management system. It allows the user to perform various operations like adding tasks, marking tasks as done, setting deadlines, and more. The CLI is interactive and provides real-time feedback based on the user’s input.

### 4. **Task Manager (TaskManager.cs)**

#### Location: `TaskList/Core/TaskManager.cs`

This file contains the `TaskManager` class, which is responsible for the core logic of task management, including adding projects and tasks, setting deadlines, and grouping tasks by deadline.

- **Main Functions**:
  - `AddProject`: Adds a new project to the system.
  - `AddTask`: Adds a new task to a specified project.
  - `SetDeadlineOfTask`: Sets a deadline for a specific task.
  - `GetTasksByDeadline`: Groups tasks based on their deadline.

This class is used by the API controller to manage data operations.

### 5. **Application Test (ApplicationTest.cs)**

#### Location: `Tasks/ApplicationTest.cs`

This file contains the unit tests for the task management application. It uses the NUnit testing framework to ensure that the application behaves as expected.

- **Test Cases**:
  - `TestAddProjectCommand`: Tests adding a project and verifying it appears in the task list.
  - `TestAddTaskCommand`: Tests adding a task to a project and verifying it appears.
  - `TestCheckCommand`: Tests marking a task as completed.
  - `TestDeadLineCommand`: Tests adding a deadline to a task.
  - `TestTodayCommand`: Verifies the task with today's deadline is correctly displayed.
  - `TestViewByDeadlineCommand`: Tests grouping tasks by their deadline.

- **Test Setup and Teardown**:
  - The `StartTheApplication` method sets up a mock console and starts a new application thread for each test.
  - The `KillTheApplication` method ensures the application is properly terminated after each test.

- **Execution and Assertions**:
  - The `Execute` method simulates user input for commands.
  - The `ReadLines` and `Read` methods validate that the console output matches the expected results.
