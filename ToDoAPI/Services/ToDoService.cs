using ToDoAPI.Models;

namespace ToDoAPI.Services {
    public class ToDoService(ToDoDbContext context) : ITodoService {
        private readonly ToDoDbContext _context = context;

        // Create To Do Task
        public void CreateToDoTask(ToDoTask todo) {

            //if (_context.ToDoTasks.Any(t => t.Id == todo.Id)) {
            //    throw new ArgumentException("A task with the same ID already exists.");
            //}
            _context.ToDoTasks.Add(todo);
            _context.SaveChanges();

        }

        // Return To Do Task by Id
        public ToDoTask? GetToDoTask(Guid id) {

            return _context.ToDoTasks.Find(id);
        }

        // Delete To Do Task by Id
        public void DeleteToDoTask(Guid id) {
            var todo = _context.ToDoTasks.Find(id);
            if (todo != null) {
                _context.ToDoTasks.Remove(todo);
                _context.SaveChanges();
            }

        }

        // Return all To Do Tasks
        public Dictionary<Guid, ToDoTask> GetToDoTaskList() {
            return _context.ToDoTasks.ToDictionary(t => t.Id);
        }


        // Update To Do Task with new values
        public ToDoTask? UpdateToDoTask(Guid id, DateTime expiryTime, string title, string desc, int procent) {
            var todo = _context.ToDoTasks.Find(id);
            if (todo == null) return null;

            todo.ExpiryTime = expiryTime;
            todo.Title = title;
            todo.Description = desc;
            todo.CompletePercent = procent;

            _context.SaveChanges();
            return todo;

        }

        // Set Mark as done or not done
        public void SetComplete(Guid id,bool complete) {
            var todo = _context.ToDoTasks.Find(id);
            if (todo != null) {
                todo.IsCompleted = complete;
                // Also set Percentage to 100
                if(complete == true)
                    SetPercent(id, 100);
                if(complete == false)
                    SetPercent(id, 0);

            }
            
            
        }
        // Set complete percent value
        public void SetPercent(Guid id, int percent) {
            var todo = _context.ToDoTasks.Find(id);
            if (todo != null) {
                todo.CompletePercent = percent;

                // If Percentage equals 100, mark to do as done
                if (percent == 100 && todo.IsCompleted == false)
                    todo.IsCompleted = true;

            }
        }

        // Get Todos for today
        public IEnumerable<ToDoTask> GetToDosForToday() {
            var today = DateTime.UtcNow.Date;

            // get tasks for today, and turn it into list
            return [.. _context.ToDoTasks.Where(todo => todo.ExpiryTime.Date == today)];
        }

        // Get Todos for next day
        public IEnumerable<ToDoTask> GetToDosForNextDay() {
            var nextDay = DateTime.UtcNow.Date.AddDays(1);

            // get tasks for next day, and turn it into list
            return [.. _context.ToDoTasks.Where(todo => todo.ExpiryTime.Date == nextDay)];
        }

        // Get Todos for current week
        public IEnumerable<ToDoTask> GetToDosForCurrentWeek() {
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            // get tasks between first and last day of the week, and turn it into list
            return [.. _context.ToDoTasks.Where(todo => todo.ExpiryTime.Date >= startOfWeek && todo.ExpiryTime.Date <= endOfWeek)];
        }
    }
    }

