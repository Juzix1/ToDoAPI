using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using System.Diagnostics;
using ToDoAPI.Models;
using ToDoAPI.Requests;
using ToDoAPI.Services;

var builder = WebApplication.CreateBuilder(args); {

    // Add Sql and establish connection

    builder.Services.AddDbContext<ToDoDbContext>(options => {

        try {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Try setting up MySQL context
            options.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString),
                        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
                    );
        } catch (Exception e) {

            Debug.WriteLine(e.Message);
        }
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<ITodoService, ToDoService>();
    }


    // Build the application
    var app = builder.Build(); {



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Create To Do Object and add it to the database
    app.MapPost("/api/todos", (ITodoService service, CreateToDoRequest request) => {

        // Check if ExpiryTime is in the future
        if (request.ExpiryTime <= DateTime.UtcNow)
            return Results.BadRequest("ExpiryTime must be in the future.");

        // Check if Completed Percent value is properly set
        if (request.CompletePercent < 0 || request.CompletePercent > 100)
            return Results.BadRequest("Completion Percent must be between 0 and 100.");

        // Create new ToDoTask object and insert it to the database
        var todo = new ToDoTask(Guid.NewGuid(),
            request.ExpiryTime,
            request.Title,
            request.Description,
            request.CompletePercent);
        service.CreateToDoTask(todo);

        // Return 201 with callback to the MapGet with same id
        return Results.CreatedAtRoute("GetToDo", new { id = todo.Id }, todo);
    }).WithName("CreateToDo").Produces<ToDoTask>(201).Produces(400);

    // Get specific To Do Task
    app.MapGet("/api/todos/{id:guid}", (ITodoService service, Guid id) => {

        // Get To Do Task by id
        var todo = service.GetToDoTask(id);

        // If todo with id doesnt exist, return not found
        if (todo == null)
            return Results.NotFound();

        return Results.Ok(todo);
    }).WithName("GetToDo").Produces<ToDoTask>(404).Produces(200);

    // Get All To Do Tasks
    app.MapGet("/api/todos", (ITodoService service) => {
        // Get all To Do Tasks
        Dictionary<Guid, ToDoTask> todoList = service.GetToDoTaskList();

        // If list is empty, return not found
        if (todoList.Count == 0)
            return Results.NotFound("There are no To Do Tasks.");


        return Results.Ok(todoList);
    }).WithName("GetAllToDos").Produces(200);

    // Delete To Do Task
    app.MapDelete("/api/todos", (ITodoService service, Guid id) => {
        // if there are no To Do Tasks, return not found
        if (service.GetToDoTaskList().Count == 0)
            return Results.NotFound($"There is none of To Do Tasks.");
        
        if (service.GetToDoTask(id) == null)
            return Results.NotFound($"Todo with id {id} not found.");


        // Delete To Do Task
        service.DeleteToDoTask(id);
        return Results.Ok();
    }).WithName("Delete ToDo");

    // Update To Do Task, need to provide the same id in json format
    app.MapPut("/api/todos", (ITodoService service, UpsertToDoRequest request) => {
        var todoList = service.GetToDoTaskList();
        todoList.TryGetValue(request.Id, out var isNull);

        // Check if ExpiryTime is in the future
        if (request.ExpiryTime <= DateTime.UtcNow)
            return Results.BadRequest("ExpiryTime must be in the future.");

        // If todo with id doesnt exist, return not found
        if (isNull == null)
            return Results.NotFound($"Todo with id {request.Id} not found.");

        // Create new To do Task and pass it to the Update method
        var todo = service.UpdateToDoTask(request.Id,
            request.ExpiryTime,
            request.Title,
            request.Description,
            request.CompletePercent);

        return Results.Ok(todo);
    });

    // Mark to Do Task as completed
    app.MapPatch("/api/todos/{id:guid}/complete", (ITodoService service, Guid id, bool isCompleted) => {
        var todo = service.GetToDoTask(id);

        // If todo with id doesnt exist, return not found
        if (todo == null)
            return Results.NotFound($"Todo with id {id} not found.");
        service.SetComplete(id, isCompleted);
        return Results.Ok(todo);
    }).WithName("SetComplete");

    // Set Percentage of To Do Progress
    app.MapPatch("/api/todos/{id:guid}/percent", (ITodoService service, Guid id, int Percent) => {
        var todo = service.GetToDoTask(id);

        // If todo with id doesnt exist, return not found
        if (todo == null)
            return Results.NotFound($"Todo with id {id} not found.");
        service.SetPercent(id, Percent);
        return Results.Ok(todo);
    }).WithName("SetPercent");

    // Get todos for today
    app.MapPatch("/api/todos/today", (ITodoService service) => {
        var todos = service.GetToDosForToday();

        // If list is empty,return not found
        if (!todos.Any())
            return Results.NotFound("There are no To Do Tasks for today.");

        return Results.Ok(todos);
    }).WithName("GetTodoForToday");

    // Get todos for next day
    app.MapPatch("/api/todos/nextDay", (ITodoService service) => {
        var todos = service.GetToDosForNextDay();

        // If list is empty,return not found
        if (!todos.Any())
            return Results.NotFound("There are no To Do Tasks for next day.");

        return Results.Ok(todos);
    }).WithName("GetTodoForNextDay");

    // Get todos for current week
    app.MapPatch("/api/todos/week", (ITodoService service) => {
        var todos = service.GetToDosForCurrentWeek().ToList();

        // If list is empty, return not found
        if (todos.Count == 0)
            return Results.NotFound("There are no To Do Tasks for current week.");
        return Results.Ok(todos);
    }).WithName("GetTodoForCurrentWeek");

    // Redirect HTTP requests to HTTPS
    app.UseHttpsRedirection();

    app.Run();
}