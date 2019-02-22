using System;
using System.ComponentModel.DataAnnotations;

namespace TodosAPI.DTO
{
    public class Todo
    {
        /// <summary>
        /// Name of task.
        /// </summary>
        [Required]
        [StringLength(30)]
        public string taskName;

        /// <summary>
        /// States whether the task is completed or not.
        /// </summary>
        [Required]
        public bool isCompleted;

        /// <summary>
        /// ISO8601 Datestring representing when the task is due.
        /// </summary>
        [Required]
        public string dueDate;
    }
}
