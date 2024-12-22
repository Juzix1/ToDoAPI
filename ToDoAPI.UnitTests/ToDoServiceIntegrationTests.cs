using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Models;
using ToDoAPI.Services;

namespace ToDoAPI.UnitTests {
    public class ToDoServiceIntegrationTests {

        private ToDoDbContext CreateContext() {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ToDoDbContext(options);
        }
        [Fact]
        public void Test_GetAllToDos_ReturnsToDoList() {
            // Arrange
            
            var service = new ToDoService(CreateContext());

            // Act
            var result =  service.GetToDoTaskList();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Dictionary<Guid,ToDoTask>>(result);
        }

        [Fact]
        public void Test_CreateToDo_CreatesToDoSuccessfully() {
            // Arrange
            var service = new ToDoService(CreateContext());

            var todoId = Guid.NewGuid();
            var todo = new ToDoTask(
            todoId,
            DateTime.Now,
            "Test ToDo",
            "Test Description",
            0);

            service.CreateToDoTask(todo);
            // Act
            var result = service.GetToDoTask(todoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test ToDo", result.Title);
        }

        [Fact]
        public void Test_UpdateToDo_UpdatesToDoSuccessfully() {
            // Arrange
            var service = new ToDoService(CreateContext());
            
            var todo = new ToDoTask(
            Guid.NewGuid(),
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

            // Act
            service.CreateToDoTask(todo);
            service.UpdateToDoTask(todo.Id, DateTime.Now, "Existing ToDo", "Existing Description", 0);
            var result = service.GetToDoTask(todo.Id);
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Existing ToDo", result.Title);
        }

        [Fact]
        public void Test_DeleteToDo_DeletesToDoSuccessfully() {
            // Arrange
            var service = new ToDoService(CreateContext());

            Guid todoId = new();
            var todo = new ToDoTask(
            todoId,
            DateTime.Now,
            "Test Task",
            "Test Description",
            0);

            // Act
            service.CreateToDoTask(todo);

            service.DeleteToDoTask(todoId);
            var result = service.GetToDoTask(todoId);

            // Assert
            Assert.Null(result);

        }
    }
}
