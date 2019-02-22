using System;

namespace TodosAPI.DTO
{
    public class TodoResponse
    {
        /// <summary>
        /// Unique identifier of this task.
        /// </summary>
        public long id;

        /// <summary>
        /// Name of task.
        /// </summary>
        public string taskName;

        /// <summary>
        /// States whether the task is completed or not.
        /// </summary>
        public bool isCompleted;

        /// <summary>
        /// ISO8601 Datestring representing when the task is due.
        /// </summary>
        public string dueDate;

        /// <summary>
        /// Creates a Todo task.
        /// </summary>
        /// <param name="id">Unique integer identifier.</param>
        /// <param name="taskName">Unique task name/content.</param>
        /// <param name="isCompleted">Whether task has been completed.</param>
        /// <param name="dueDate">DateTime (converted to ISO8601 in constructor).</param>
        public TodoResponse(long id, string taskName, bool isCompleted, DateTime dueDate)
        {
            this.id = id;
            this.taskName = taskName;
            this.isCompleted = isCompleted;
            this.dueDate = dueDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
