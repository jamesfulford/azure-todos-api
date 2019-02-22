using System;
using System.Collections.Generic;
using System.Linq;
using TodosAPI.Models;

namespace TodosAPI.Data
{
    /// <summary>
    /// Seeds the database
    /// </summary>
    public class Initializer
    {
        /// <summary>
        /// Seeds the specified dbcontext with data
        /// </summary>
        /// <param name="context">The context to seed.</param>
        public static void Initialize(Context context)
        {
            if (context.Todos.Any())
            {
                return;
            }

            Todo[] todos = new Todo[]
            {
                new Todo() {
                    taskName = "Buy groceries",
                    isCompleted = false,
                    dueDate = new DateTime(2019, 2, 3),
                },

                new Todo() {
                    taskName = "Workout",
                    isCompleted = true,
                    dueDate = new DateTime(2019, 1, 1),
                },

                new Todo() {
                    taskName = "Paint fence",
                    isCompleted = false,
                    dueDate = new DateTime(2019, 3, 15),
                },

                new Todo() {
                    taskName = "Mow Lawn",
                    isCompleted = false,
                    dueDate = new DateTime(2019, 6, 11),
                },
            };

            // Add the data to the in memory model
            foreach (Todo todo in todos)
            {
                context.Todos.Add(todo);
            }

            // Commit the changes to the database
            context.SaveChanges();
        }
    }
}
