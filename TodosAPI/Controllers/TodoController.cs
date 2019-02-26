using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TodosAPI.CustomSettings;
using TodosAPI.Data;
using TodosAPI.Models;

namespace TodosAPI.Controllers
{
    /// <summary>
    /// API for creating todo/task resources.
    /// </summary>
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        /// <summary>
        /// Identifier for the Get TodoById route
        /// </summary>
        private const string GetTodoByIdRoute = "GetTodoByIdRoute";

        /// <summary>
        /// The database context
        /// </summary>
        private readonly Context _context;

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The configuration instance
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The customer limits settings
        /// </summary>
        private readonly TodoLimits _todoLimits;

        /// <summary>
        /// Instantiate a TasksController with Dependency Injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        /// <param name="todoLimits"></param>
        public TasksController(
            ILogger<TasksController> logger,
            Context context,
            IConfiguration configuration,
            IOptions<TodoLimits> todoLimits
        )
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _todoLimits = todoLimits.Value;
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        /// <returns>A list of Todos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(DTO.TodoList), (int)HttpStatusCode.OK)]
        public ActionResult<DTO.TodoList> GetAllTasks()
        {
            return new DTO.TodoList((from todo in _context.Todos select todo).ToList());
        }

        private Todo GetTodoById(long id)
        {
            return (from t in _context.Todos where t.id == id select t).SingleOrDefault();
        }

        /// <summary>
        /// Get a task by id.
        /// </summary>
        /// <param name="id">Identifier to look up the task by.</param>
        /// <returns>The Todo requested.</returns>
        [HttpGet("{id:int}", Name = GetTodoByIdRoute)]
        [ProducesResponseType(typeof(Todo), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public ActionResult<Todo> GetTask(int id)
        {
            try
            {
                Todo todo = GetTodoById(id);
                if (todo == null)
                {
                    _logger.LogInformation($"Todo(id={id}) was not found.");
                    return NotFound(new DTO.ErrorResponse(DTO.ErrorNumber.NOTFOUND, "id", id.ToString()));
                }
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(Common.LoggingEvents.InternalError, ex, $"CustomerController Get Customer(id=[{id}]) caused an internal error.", id);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private Boolean TaskNameExists(string taskName)
        {
            return (from t in _context.Todos where t.taskName == taskName select t).FirstOrDefault() == null;
        }

        private DTO.ErrorNumber ErrorMessageToErrorNumber(string errorText)
        {
            // These strings are set in data attributes on input payload as ErrorMessages.
            switch (errorText)
            {
                case "2":
                    return DTO.ErrorNumber.TOOLARGE;
                case "3":
                    return DTO.ErrorNumber.REQUIRED;
                case "6":
                    return DTO.ErrorNumber.TOOSMALL;
                case "7":
                default:
                    return DTO.ErrorNumber.INVALID;
            }
        }

        private DTO.ErrorResponse MakeInvalidDataAttributeResponse ()
        {
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        return new DTO.ErrorResponse(
                            ErrorMessageToErrorNumber(error.ErrorMessage),
                            key,
                            ModelState[key].RawValue == null ? null : ModelState[key].RawValue.ToString()
                        );
                    }
                }
            }
            throw new Exception("ModelState is invalid, but has no erroring keys!");
        }

        /// <summary>
        /// Create a task to do.
        /// </summary>
        /// <param name="todo">The todo item to create.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Todo), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.Conflict)]
        public IActionResult CreateTask([FromBody] Todo todo) {
            try
            {
                // Required and various length constraints
                if (ModelState.IsValid)
                {

                    // Max todos constraint
                    if (!CanAddMoreTodos())
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, new DTO.ErrorResponse(DTO.ErrorNumber.MAXENTITIES));
                    }

                    // Unique constraint
                    if (!TaskNameExists(todo.taskName))
                    {
                        return new ConflictObjectResult(new DTO.ErrorResponse(DTO.ErrorNumber.EXISTS, "taskName", todo.taskName));
                    }

                    // Add the todo
                    _context.Todos.Add(todo);

                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest(MakeInvalidDataAttributeResponse());
                }
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return CreatedAtRoute(GetTodoByIdRoute, new { todo.id }, todo);
        }

        /// <summary>
        /// Update a pre-existing task.
        /// </summary>
        /// <param name="id">The identifier of the task to update.</param>
        /// <param name="todo">The new state of the task to update.</param>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.Conflict)]
        public IActionResult UpdateTask(int id, [FromBody] Todo todo)
        {
            try
            {
                // Required and various length constraints
                if (ModelState.IsValid)
                {
                    // Find todo to update
                    Todo targetTodo = GetTodoById(id);
                    if (targetTodo == null)
                    {
                        return NotFound(new DTO.ErrorResponse(DTO.ErrorNumber.NOTFOUND, "id", id.ToString()));
                    }

                    // Unique constraint
                    if (!TaskNameExists(todo.taskName))
                    {
                        return new ConflictObjectResult(new DTO.ErrorResponse(DTO.ErrorNumber.EXISTS, "taskName", todo.taskName));
                    }

                    // Update the todo
                    targetTodo.isCompleted = todo.isCompleted;
                    targetTodo.taskName = todo.taskName;
                    targetTodo.dueDate = todo.dueDate;

                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest(MakeInvalidDataAttributeResponse());
                }
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return NoContent();
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The identifier of the task to delete.</param>
        [HttpDelete("{id:int}")]
        public void DeleteTask(int id) { }

        private bool CanAddMoreTodos()
        {
            return _todoLimits.MaxTaskEntries > (from t in _context.Todos select t).Count();
        }
    }
}
