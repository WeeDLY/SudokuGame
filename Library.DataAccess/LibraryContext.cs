using Library.Model;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;

namespace Library.DataAccess
{
    /// <summary>
    /// LibraryContext
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class LibraryContext : DbContext
    {
        /// <summary>
        /// Gets or sets the register users.
        /// </summary>
        /// <value>
        /// The register users.
        /// </value>
        public virtual DbSet<RegisterUser> RegisterUsers { get; set; }

        /// <summary>
        /// Gets or sets the sudoku puzzles.
        /// </summary>
        /// <value>
        /// The sudoku puzzles.
        /// </value>
        public virtual DbSet<SudokuPuzzle> SudokuPuzzles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryContext" /> class.
        /// </summary>
        public LibraryContext()
        {
            Configuration.ProxyCreationEnabled = false;

            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder
            {
                DataSource = "", // TODO: need to fill this out
                UserID = "",
                Password = "",
            };

            Database.Connection.ConnectionString = connectionString.ToString();
            Database.SetInitializer(new LibraryDBInitializer());
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<RegisterUser>()
                .HasMany(a => a.SudokuPuzzles)
                .WithMany(b => b.RegisterUsers)
                .Map(m =>
                {
                    m.ToTable("UserPuzzles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("SudokuPuzzleId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}