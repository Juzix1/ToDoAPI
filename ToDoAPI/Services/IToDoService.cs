using System.Security.Cryptography;
using ToDoAPI.Models;

namespace ToDoAPI.Services {
    public interface ITodoService {

        // Create To Do Task
        void CreateToDoTask(ToDoTask todo);

        // Return To Do Task by Id
        ToDoTask? GetToDoTask(Guid id);

        // Return all To Do Tasks
        Dictionary<Guid, ToDoTask> GetToDoTaskList();

        // Delete To Do Task by Id
        void DeleteToDoTask(Guid id);

        // Update To Do Task with new values
        ToDoTask? UpdateToDoTask(Guid id, DateTime expiryTime, string title, string desc, int procent);

        // Set Mark as done or not done
        void SetComplete(Guid id, bool complete);

        // Set complete percent value
        void SetPercent(Guid id, int percent);

        // Get Todos for today
        IEnumerable<ToDoTask> GetToDosForToday();
        // Get Todos for next day
        IEnumerable<ToDoTask> GetToDosForNextDay();
        // Get Todos for current week
        IEnumerable<ToDoTask> GetToDosForCurrentWeek();
    }
}
