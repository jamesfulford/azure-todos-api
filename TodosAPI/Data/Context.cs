using TodosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TodosAPI.Data
{
    /// <summary>
    /// Coordinates Entity Framework functionality for a given data model is the database context class
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    /// <remarks>Step 6</remarks>
    public class Context : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Represents the Todos table.
        /// </summary>
        /// <value>
        /// The current todos.
        /// </value>
        public DbSet<Todo> Todos { get; set; }
        
        /// <summary>
        /// Connects Model to Table
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Links Todos entity model to "Todos" table.
            modelBuilder.Entity<Todo>().ToTable("Todos");
        }
    }


}

