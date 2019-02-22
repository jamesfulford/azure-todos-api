using System.Collections.Generic;

namespace TodosAPI.DTO
{
    public class TodoList
    {
        /// <summary>
        /// List of tasks.
        /// </summary>
        public List<TodoResponse> tasks;

        /// <summary>
        /// A list of tasks to do.
        /// </summary>
        /// <param name="tasks">List of relevant tasks.</param>
        public TodoList(List<TodoResponse> tasks)
        {
            this.tasks = tasks;
        }
    }
}
