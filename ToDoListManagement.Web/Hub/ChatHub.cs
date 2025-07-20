namespace ToDoListManagement.Web.Hub;

using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task NotifyStatusChange(int taskId, string newStatus)
    {
        await Clients.All.SendAsync("TaskStatusChanged", taskId, newStatus);
    }

    public async Task NewTaskAdded()
    {
        await Clients.All.SendAsync("NewTaskAdded");
    }

    public async Task TaskUpdated()
    {
        await Clients.All.SendAsync("TaskUpdated");
    }

    public async Task TaskDeleted()
    {
        await Clients.All.SendAsync("TaskDeleted");
    }

}
