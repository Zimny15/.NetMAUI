using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace ToDoList;

public partial class NewPage2 : ContentPage
{
    public ObservableCollection<TaskModel>? tasks { get; set; } = new ObservableCollection<TaskModel>();
    public NewPage2()
	{
		InitializeComponent();
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshList();
    }
    private async Task RefreshList()
    {
        var loadedTasks = await TaskStorage.LoadTasksAsync();
        if (loadedTasks != null)
        {
            tasks.Clear();
            foreach (var task in loadedTasks)
            {
                tasks.Add(task);
            }
        }
    }
    private async void OnDeleteButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is TaskModel task && tasks is not null)
        {
            tasks.Remove(task);
            await TaskStorage.SaveTasksAsync(tasks);
            await RefreshList();
        }
    }

    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TaskModel task)
        {
            task.IsCompleted = e.Value;
            await TaskStorage.SaveTasksAsync(tasks);
        }
    }
}