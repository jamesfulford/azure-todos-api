using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TodosAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        /// <summary>
        /// Get all tasks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DTO.TodoList), (int)HttpStatusCode.OK)]
        public ActionResult<DTO.TodoList> GetAllTasks()
        {
            return new DTO.TodoList(new List<DTO.TodoResponse> {
                new DTO.TodoResponse(1, "Hello World!", true, new DateTime(2019, 2, 14)),
            });
        }

        /// <summary>
        /// Get a task by id.
        /// </summary>
        /// <param name="id">Identifier to look up the task by.</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DTO.TodoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(DTO.ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public ActionResult<DTO.TodoResponse> GetTask(int id)
        {
            return new DTO.TodoResponse(1, "Hello World!", true, new DateTime(2019, 2, 14));
        }

        /// <summary>
        /// Create a task to do.
        /// </summary>
        /// <param name="todo">The todo item to create.</param>
        [HttpPost]
        public void CreateTask([FromBody] DTO.Todo todo) { }

        /// <summary>
        /// Update a pre-existing task.
        /// </summary>
        /// <param name="id">The identifier of the task to update.</param>
        /// <param name="todo">The new state of the task to update.</param>
        [HttpPatch("{id:int}")]
        public void UpdateTask(int id, [FromBody] DTO.Todo todo) { }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The identifier of the task to delete.</param>
        [HttpDelete("{id:int}")]
        public void DeleteTask(int id) { }
    }
}
