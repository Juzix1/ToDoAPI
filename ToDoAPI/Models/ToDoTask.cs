
using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models {
    public class ToDoTask(Guid id, DateTime expiryTime, string title, string description, int completePercent) {

        // Unique identifier for the task
        [Key]
        public Guid Id { get; set; } = id;

        // The time by which the task should be completed
        public DateTime ExpiryTime { get; set; } = expiryTime;

        // Title of the task
        public string Title { get; set; } = title;

        // Detailed description of the task
        public string Description { get; set; } = description;

        // Percentage of task completion
        public int CompletePercent { get; set; } = completePercent;

        // Indicates if the task is completed
        public bool IsCompleted { get; set; } = false;
    }
}
