﻿using System;
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
            Todo todo = (from t in _context.Todos where t.id == id select t).SingleOrDefault();
            if (todo == null)
            {
                _logger.LogInformation($"Todo(id={id}) was not found.");
                // TODO(james): Return an error response
                return NotFound();
            }
            return todo;
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
                    if ((from t in _context.Todos where t.taskName == todo.taskName select t).FirstOrDefault() != null)
                    {
                        return new ConflictObjectResult(new DTO.ErrorResponse(DTO.ErrorNumber.EXISTS, "taskName", todo.taskName));
                    }

                    // Add the todo
                    _context.Todos.Add(todo);

                    _context.SaveChanges();
                }
                else
                {
                    foreach (var key in ModelState.Keys)
                    {
                        if (ModelState[key].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                        {
                            foreach (var error in ModelState[key].Errors)
                            {
                                DTO.ErrorNumber num;
                                switch (error.ErrorMessage)
                                {
                                    case "2":
                                        num = DTO.ErrorNumber.TOOLARGE;
                                        break;
                                    case "3":
                                        num = DTO.ErrorNumber.REQUIRED;
                                        break;
                                    case "6":
                                        num = DTO.ErrorNumber.TOOSMALL;
                                        break;
                                    case "7":
                                    default:
                                        num = DTO.ErrorNumber.INVALID;
                                        break;
                                }
                                return BadRequest(new DTO.ErrorResponse(
                                    num,
                                    key,
                                    ModelState[key].RawValue == null ? null : ModelState[key].RawValue.ToString()
                                ));
                            }
                        }
                    }
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
        public void UpdateTask(int id, [FromBody] Todo todo) { }

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
