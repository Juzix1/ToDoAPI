namespace ToDoAPI.Requests {

    // Request to Create a new ToDoTask, id is generated automatically, IsCompleted is false by default
    public record CreateToDoRequest(
        DateTime ExpiryTime,
        string Title,
        string Description,
        int CompletePercent);
    
}
