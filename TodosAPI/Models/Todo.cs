using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodosAPI.Models
{
    /// <summary>
    /// The Todo entity.
    /// </summary>
    public class Todo
    {

        /// <summary>
        /// Gets or sets the identifier of the todo.
        /// </summary>
        /// <value>The todo identifier.</value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }

        /// <summary>
        /// Gets or sets the content of the todo.
        /// </summary>
        /// <value>The content of the task.</value>
        [Required]
        [StringLength(200)]
        public string taskName { get; set; }

        /// <summary>
        /// Gets or sets the completion status of the todo.
        /// </summary>
        /// <value>The completion status of the todo.</value>
        public bool isCompleted { get; set; }

        /// <summary>
        /// Gets or sets the due date of the todo.
        /// </summary>
        /// <value>The due date of the todo.</value>
        public DateTime dueDate { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{{ \"id\": \"{id}\", \"taskName\": \"{taskName}\", \"isCompleted\": {(isCompleted ? "true" : "false" )}, \"dueDate\": \"{dueDate.ToShortDateString()}\"}}";
        }

    }
}
