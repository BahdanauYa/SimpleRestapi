using Microsoft.EntityFrameworkCore;
using SimpleRestapi;
using SimpleRestapi.Enums;
using SimpleRestapi.Models;
using SimpleRestapi.Services;

namespace SimpleRestApiTests.Services
{
    public class TodoServiceTests
    {
        private readonly TodoService _todoService;
        private readonly DataContext _context;

        public TodoServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _todoService = new TodoService(_context);
        }

        [Fact]
        public async Task GetAllTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var todo1 = new Todo { Title = "Task 1", ExpiryDate = DateTime.UtcNow };
            var todo2 = new Todo { Title = "Task 2", ExpiryDate = DateTime.UtcNow.AddDays(1) };

            _context.Todos.AddRange(todo1, todo2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetAllTodos();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTodoById_ShouldThrowException_WhenTodoNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _todoService.GetTodoById(99));
        }

        [Fact]
        public async Task GetTodoById_ShouldReturnTodo_WhenTodoExists()
        {
            // Arrange
            var todo = await _context.Todos.AddAsync(new Todo { Title = "Test Todo" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetTodoById(todo.Entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Todo", result.Title);
        }

        [Fact]
        public async Task CreateTodo_ShouldAddTodo()
        {
            // Arrange
            var todoName = "New Todo";
            var todo = new Todo
            {
                Title = todoName,
                Description = "Description",
                ExpiryDate = DateTime.Today.AddDays(3),
                PercentComplete = 0
            };

            // Act
            var result = await _todoService.CreateTodo(todo);
            var todos = _context.Todos.ToList();
            foreach (var todo1 in todos)
            {
                Console.WriteLine($"Todo: {todo1.Title}, ExpiryDate: {todo1.ExpiryDate}");
            }

            // Assert
            Assert.Equal(todoName, result.Title);
            Assert.True(_context.Todos.Any(i => i.Title == todoName));
        }

        [Fact]
        public async Task UpdateTodo_ShouldUpdateExistingTodo()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Task 1",
                Description = "Description 1",
                ExpiryDate = DateTime.UtcNow
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            var updatedTodo = new Todo
            {
                Title = "Updated Task",
                Description = "Updated Description",
                ExpiryDate = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var result = await _todoService.UpdateTodo(1, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Task", result.Title);
        }

        [Fact]
        public async Task DeleteTodo_ShouldRemoveTodo()
        {
            // Arrange
            var todo = await _context.Todos.AddAsync(new Todo
            {
                Title = "Task to Delete",
                ExpiryDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var isDeleted = await _todoService.DeleteTodo(1);

            // Assert
            Assert.True(isDeleted);
            Assert.Null(await _context.Todos.FindAsync(todo.Entity.Id));
        }

        [Fact]
        public async Task SetTodoPercentComplete_ShouldUpdatePercentComplete()
        {
            // Arrange
            var todo = new Todo
            {
                Title = "Task",
                Description = "Description",
                ExpiryDate = DateTime.UtcNow,
                PercentComplete = 20
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            await _todoService.SetTodoPercentComplete(1, 50);
            var updatedTodo = await _context.Todos.FindAsync(1);

            // Assert
            Assert.NotNull(updatedTodo);
            Assert.Equal(50, updatedTodo.PercentComplete);
        }

        [Fact]
        public async Task GetIncomingTodos_ShouldReturnTodayTodos()
        {
            // Arrange
            var today = DateTime.UtcNow;
            var todo1 = new Todo { Title = "Today Task", ExpiryDate = today };
            var todo2 = new Todo { Title = "Tomorrow Task", ExpiryDate = today.AddDays(1) };

            _context.Todos.AddRange(todo1, todo2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetIncomingTodos(TodoPeriod.Today);

            // Assert
            Assert.Single(result);
            Assert.Equal("Today Task", result.First().Title);
        }

        [Fact]
        public async Task GetIncomingTodos_ShouldReturnWeekTodos()
        {
            // Arrange
            var today = DateTime.UtcNow;
            var todo1 = new Todo { Title = "Today Task", ExpiryDate = today };
            var todo2 = new Todo { Title = "Task in 5 days", ExpiryDate = today.AddDays(5) };

            _context.Todos.AddRange(todo1, todo2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoService.GetIncomingTodos(TodoPeriod.CurrentWeek);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task MarkTodoAsDone_ShouldSetTodoAsDone_WhenTodoExists()
        {
            // Arrange
            var todo = await _context.Todos.AddAsync(new Todo
            {
                Title = "Test Todo",
                Description = "Test description",
                ExpiryDate = DateTime.Now.AddDays(1),
                PercentComplete = 50
            });
            await _context.SaveChangesAsync();

            // Act
            await _todoService.MarkTodoAsDone(todo.Entity.Id);

            // Assert
            var updatedTodo = await _todoService.GetTodoById(todo.Entity.Id);
            Assert.NotNull(updatedTodo);
            Assert.Equal(100, updatedTodo.PercentComplete);
            Assert.True(updatedTodo.IsDone);
        }
    }
}