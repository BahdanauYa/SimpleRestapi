using SimpleRestapi.Enums;
using SimpleRestapi.Models;

namespace SimpleRestapi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllTodos();
        Task<Todo> GetTodoById(int id);
        Task<IEnumerable<Todo>> GetIncomingTodos(TodoPeriod period);
        Task<Todo> CreateTodo(Todo todo);
        Task<Todo> UpdateTodo(int id, Todo todo);
        Task<Todo> SetTodoPercentComplete(int id, int percentComplete);
        Task<bool> DeleteTodo(int id);
        Task<Todo> MarkTodoAsDone(int id);
    }
}
