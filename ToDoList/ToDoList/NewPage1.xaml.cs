using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace ToDoList;

public partial class NewPage1 : ContentPage
{
    public static ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
    public static ObservableCollection<TaskModel> newTasks = new ObservableCollection<TaskModel>();
    public NewPage1()
	{
		InitializeComponent();
        LoadTasks();
        TasksListView.ItemsSource = newTasks;
    }

    private async void LoadTasks()
    {
        tasks = await TaskStorage.LoadTasksAsync();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        string taskName = TaskNameEntry.Text;
        string taskDescription = TaskDescriptionEditor.Text;

        if (string.IsNullOrWhiteSpace(taskName))
        {
            DisplayAlert("Error", "Task name cannot be empty!", "OK");
            return;
        }

        TaskModel newTask = new TaskModel(taskName, string.IsNullOrWhiteSpace(taskDescription) ? null : taskDescription);
        newTasks.Add(newTask);

        TaskNameEntry.Text = string.Empty;
        TaskDescriptionEditor.Text = string.Empty;
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (tasks.Count > 0)
        {
            tasks.Clear();
            var loadedTasks = await TaskStorage.LoadTasksAsync();
            if (loadedTasks != null)
            {
                foreach (var task in loadedTasks)
                {
                    tasks.Add(task);
                }
            }
        }
        if (newTasks.Count > 0)
        {
            foreach (var task in newTasks)
            {
                tasks.Add(task);
            }
            await TaskStorage.SaveTasksAsync(tasks);
            await DisplayAlert("Success", $"You added {DisplayAddedTasks()} to list", "OK!");
            newTasks.Clear();
        }
        else
            await DisplayAlert("Fail", $"You have no new tasks to add", "OK!");
        
    }
    private string DisplayAddedTasks()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var task in newTasks)
        {
            sb.Append(task.Name);
            sb.Append(" ");
        }
        return sb.ToString();
    }

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (newTasks.Count > 0)
        {
            var taskToRemove = newTasks[newTasks.Count - 1];
            newTasks.RemoveAt(newTasks.Count -1);
        }
    }
}

public class TaskModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    public TaskModel(string name, string? description = null)
    {
        Name = name;
        Description = description;
        IsCompleted = false;
    }
}

public static class TaskStorage
{
    private static readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.json");
    public static async Task SaveTasksAsync(ObservableCollection<TaskModel> tasks)
    {
        try
        {
            string json = JsonSerializer.Serialize(tasks);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    public static async Task<ObservableCollection<TaskModel>> LoadTasksAsync()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<ObservableCollection<TaskModel>>(json) ?? new ObservableCollection<TaskModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
        }
        return new ObservableCollection<TaskModel>();
    }
}