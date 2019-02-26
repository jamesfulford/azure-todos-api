using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodosAPI.CustomSettings
{
    /// <summary>
    /// Defines the todo limit settings
    /// </summary>
    public class TodoLimits
    {
        /// <summary>
        /// Gets or sets the maximum number of todos.
        /// </summary>
        /// <value>
        /// The maximum number of todos.
        /// </value>
        public int MaxTaskEntries { get; set; } = 100;
    }


}
