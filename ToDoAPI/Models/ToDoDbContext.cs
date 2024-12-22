using Microsoft.EntityFrameworkCore;

namespace ToDoAPI.Models {

    // Database context for ToDoTask
    public class ToDoDbContext(DbContextOptions<ToDoDbContext> options) : DbContext(options){

        // Table for ToDoTasks
        public virtual DbSet<ToDoTask> ToDoTasks { get; set; }
    }
}
