namespace ToDoAPI.Requests {

    // Request to Update a ToDoTask with new object without marking it as completed (as default)
    public record UpsertToDoRequest(
        Guid Id,
        DateTime ExpiryTime,
        string Title,
        string Description,
        int CompletePercent);

    
}

