using Microsoft.EntityFrameworkCore;
using SimpleRestapi.Enums;
using SimpleRestapi.Models;

namespace SimpleRestapi.Services
{
    public class TodoService(DataContext context) : ITodoService
    {
        private readonly DataContext _context = context;

        public async Task<IEnumerable<Todo>> GetAllTodos()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo> GetTodoById(int id)
        {
            return await GetTodoOrThrow(id);
        }

        public async Task<IEnumerable<Todo>> GetIncomingTodos(TodoPeriod period)
        {
            var today = DateTime.UtcNow.Date;
            IQueryable<Todo> query = _context.Todos;

            switch (period)
            {
                case TodoPeriod.Today:
                    query = query.Where(t => t.ExpiryDate.Date == today);
                    break;
                case TodoPeriod.NextDay:
                    query = query.Where(t => t.ExpiryDate.Date == today.AddDays(1));
                    break;
                case TodoPeriod.CurrentWeek:
                    query = query.Where(t => t.ExpiryDate.Date >= today && t.ExpiryDate.Date <= today.AddDays(7));
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<Todo> CreateTodo(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo> UpdateTodo(int id, Todo todo)
        {
            var existingTodo = await GetTodoOrThrow(id);

            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.ExpiryDate = todo.ExpiryDate;
            existingTodo.PercentComplete = todo.PercentComplete;

            await _context.SaveChangesAsync();
            return existingTodo;
        }

        public async Task<Todo> SetTodoPercentComplete(int id, int percentComplete)
        {
            if (percentComplete < 0 || percentComplete > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percentComplete),
                    "Percent complete must be between 0 and 100.");
            }

            var todo = await GetTodoOrThrow(id);

            todo.PercentComplete = percentComplete;
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Todo> MarkTodoAsDone(int id)
        {
            var todo = await GetTodoOrThrow(id);

            todo.PercentComplete = 100;
            todo.IsDone = true;
            await _context.SaveChangesAsync();
            return todo;
        }

        private async Task<Todo> GetTodoOrThrow(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            return todo ?? throw new KeyNotFoundException($"Todo with ID {id} not found.");
        }
    }
}
