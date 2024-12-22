using Moq;
using ToDoAPI.Services;
using ToDoAPI.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace ToDoAPI.UnitTests;

public class TodoServiceTests {
    private ToDoDbContext CreateContext() {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ToDoDbContext(options);
    }

    [Fact]
    // Test CreateToDoTask method
    public void CreateToDoTask_ShouldAddToDoTask() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

        // Act
        service.CreateToDoTask(todo);

        // Assert
        var createdTodo = context.ToDoTasks.Find(todo.Id);
        Assert.NotNull(createdTodo);
        Assert.Equal(todo.Id, createdTodo.Id);
    }

    [Fact]
    // Test GetToDoTask method
    public void GetToDoTask_ShouldReturnToDo_WhenIdExists() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();
        var todo = new ToDoTask(
            id,
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

        context.ToDoTasks.Add(todo);
        context.SaveChanges();

        // Act
        var result = service.GetToDoTask(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todo.Id, result?.Id);
    }

    [Fact]
    // Test DeleteToDoTask method
    public void DeleteToDoTask_ShouldRemoveToDoTask_WhenIdExists() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();
        var todo = new ToDoTask(
            id,
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

        context.ToDoTasks.Add(todo);
        context.SaveChanges();

        // Act
        service.DeleteToDoTask(id);

        // Assert
        var deletedTodo = context.ToDoTasks.Find(id);
        Assert.Null(deletedTodo);
    }

    [Fact]
    // Test GetToDoTaskList method
    public void GetToDoTaskList_ShouldReturnAllToDoTasks() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo1 = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task 1",
            "Test Description 1",
            0);
        var todo2 = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task 2",
            "Test Description 2",
            0);
        context.ToDoTasks.Add(todo1);
        context.ToDoTasks.Add(todo2);
        context.SaveChanges();

        // Act
        var result = service.GetToDoTaskList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    // Test UpdateToDoTask method when expiry time is past
    public void TryCreateToDoTask_ShouldReturnNull_WhenExpiryTimeIsPast() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        // Act
        var result = service.UpdateToDoTask(todo.Id, DateTime.Now.AddMinutes(-1), "Test Task", "Test Description", 0);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    // Test UpdateToDoTask method when id does not exist
    public void TryCreateToDoTask_ShouldReturnNull_WhenIdDoesNotExist() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        // Act
        var result = service.UpdateToDoTask(Guid.NewGuid(), DateTime.Now, "Test Task", "Test Description", 0);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    // Test SetComplete method to set complete percent to 100 when complete is true
    public void SetCompleteToTrue_ShouldSetCompletePercentTo100_WhenCompleteIsTrue() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, true);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.Equal(100, result?.CompletePercent);
    }

    [Fact]
    // Test SetComplete method, set complete percent to 0 when complete is false
    public void SetCompleteTo0_ShouldSetCompletePercentTo0_WhenCompleteIsFalse() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            100);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, false);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.Equal(0, result?.CompletePercent);
    }

    [Fact]
    // Test SetComplete methodm, set IsCompleted to true when complete is true
    public void SetCompleteTrue_ShouldSetIsCompletedToTrue_WhenCompleteIsTrue() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, true);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.True(result?.IsCompleted);
    }

    [Fact]
    // Test SetComplete method, set IsCompleted to false when complete is false
    public void SetCompleteFalse_ShouldSetIsCompletedToFalse_WhenCompleteIsFalse() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, false);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.False(result?.IsCompleted);
    }

    [Fact]
    // Test SetComplete method to not set complete percent to 100 when complete is false
    public void CheckSetComplete_ShouldNotSetCompletePercentTo100_WhenCompleteIsFalse() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, false);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.Equal(0, result?.CompletePercent);
    }

    [Fact]
    // Test SetComplete method to not set IsCompleted to true when complete is false
    public void CheckSetComplete_ShouldNotSetIsCompletedToTrue_WhenCompleteIsFalse() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, false);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.False(result?.IsCompleted);
    }

    [Fact]
    // Test SetComplete method to not set complete percent to 0 when complete is true
    public void CheckSetComplete_ShouldNotSetCompletePercentTo0_WhenCompleteIsTrue() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            100);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, true);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.Equal(100, result?.CompletePercent);
    }

    [Fact]
    // Test SetComplete method to not set IsCompleted to false when complete is true
    public void CheckSetComplete_ShouldNotSetIsCompletedToFalse_WhenCompleteIsTrue() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            100);
        context.ToDoTasks.Add(todo);
        context.SaveChanges();
        // Act
        service.SetComplete(todo.Id, true);
        var result = context.ToDoTasks.Find(todo.Id);
        // Assert
        Assert.True(result?.IsCompleted);
    }
    [Fact]
    // Test UpdateToDoTask method when id exists
    public void UpdateToDoTask_ShouldUpdateToDoTask_WhenIdExists() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();
        var todo = new ToDoTask(
            id,
            DateTime.Now,
            "Original Task",
            "Original Description",
            0);

        context.ToDoTasks.Add(todo);
        context.SaveChanges();

        // Act
        var updatedTodo = service.UpdateToDoTask(id, DateTime.Now.AddDays(1), "Updated Task", "Updated Description", 50);

        // Assert
        Assert.NotNull(updatedTodo);
        Assert.Equal("Updated Task", updatedTodo.Title);
        Assert.Equal("Updated Description", updatedTodo.Description);
        Assert.Equal(50, updatedTodo.CompletePercent);
    }

    [Fact]
    // Test UpdateToDoTask method when id does not exist
    public void GetToDoTask_ShouldReturnNull_WhenIdDoesNotExist() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();

        // Act
        var result = service.GetToDoTask(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    // Test GetToDoTaskList method when list is empty
    public void CreateToDoTask_ShouldNotAddDuplicateToDoTask() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();
        var todo = new ToDoTask(
            id,
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

        context.ToDoTasks.Add(todo);
        context.SaveChanges();

        // Act
        var exception = Assert.Throws<ArgumentException>(() => service.CreateToDoTask(todo));
        var duplicateTodo = context.ToDoTasks.Find(id);

        // Assert
        Assert.NotNull(duplicateTodo);
        Assert.Equal(1, context.ToDoTasks.Count());
        Assert.Equal("A task with the same ID already exists.", exception.Message);
    }

    [Fact]
    // Test DeleteToDoTask method when id does not exist
    public void DeleteToDoTask_ShouldNotThrowException_WhenIdDoesNotExist() {
        // Arrange
        var context = CreateContext();
        var service = new ToDoService(context);
        var id = Guid.NewGuid();

        // Act & Assert
        var exception = Record.Exception(() => service.DeleteToDoTask(id));
        Assert.Null(exception);
    }



}
