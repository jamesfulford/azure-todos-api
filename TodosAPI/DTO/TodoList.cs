using System.Collections.Generic;
using TodosAPI.Models;

namespace TodosAPI.DTO
{
    /// <summary>
    /// DTO for transferring a list of Todos.
    /// </summary>
    public class TodoList
    {
        /// <summary>
        /// List of tasks.
        /// </summary>
        public List<Todo> tasks;

        /// <summary>
        /// A list of tasks to do.
        /// </summary>
        /// <param name="tasks">List of relevant tasks.</param>
        public TodoList(List<Todo> tasks)
        {
            this.tasks = tasks;
        }
    }
}
