using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleRestapi.Enums;
using SimpleRestapi.Models;
using SimpleRestapi.Services;

namespace SimpleRestapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController(DataContext context, ITodoService todoService) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly ITodoService _todoService = todoService;

        /// <summary>
        /// Gets all Todos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> Get()
        {
            return await _context.Todos.ToListAsync();
        }

        /// <summary>
        /// Gets a Todo by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetById(int id)
        {
            var todo = await _todoService.GetTodoById(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        /// <summary>
        /// Get incoming for different periods
        /// </summary>
        /// <returns></returns>
        [HttpGet("incoming")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetIncomingTodos([FromQuery] TodoPeriod period)
        {
            var incomingTodos = await _todoService.GetIncomingTodos(period);
            return Ok(incomingTodos);
        }

        /// <summary>
        /// Creates a new Todo
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Todo>> Create(Todo todo)
        {
            var createdTodo = await _todoService.CreateTodo(todo);
            return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
        }

        /// <summary>
        /// Updates an existing Todo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todo"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Todo todo)
        {
            var updatedTodo = await _todoService.UpdateTodo(id, todo);
            if (updatedTodo == null) return NotFound();
            return Ok(updatedTodo);
        }

        /// <summary>
        /// Sets the PercentComplete for a Todo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [HttpPatch("{id}/percent")]
        public async Task<IActionResult> SetPercentComplete(int id, [FromQuery] int percentComplete)
        {
            var updatedTodo = await _todoService.SetTodoPercentComplete(id, percentComplete);
            if (updatedTodo == null) return NotFound();
            return Ok(updatedTodo);
        }

        /// <summary>
        /// Deletes a Todo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _todoService.DeleteTodo(id);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Marks a Todo as done
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/done")]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var updatedTodo = await _todoService.MarkTodoAsDone(id);
            if (updatedTodo == null) return NotFound();
            return Ok(updatedTodo);
        }
    }
}
