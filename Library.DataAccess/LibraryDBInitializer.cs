using Library.Model;
using System.Data.Entity;

namespace Library.DataAccess
{
    public class LibraryDBInitializer : DropCreateDatabaseIfModelChanges<LibraryContext>
    {
        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected override void Seed(LibraryContext context)
        {
            RegisterUser testUser = new RegisterUser("TestUser", "filler_password", "filler_salt");

            SudokuPuzzle testPuzzle = new SudokuPuzzle();
            testPuzzle.CompletedPuzzle(true);

            context.RegisterUsers.Add(testUser);
            context.SudokuPuzzles.Add(testPuzzle);


            RegisterUser testUser2 = new RegisterUser("TestUser2", "filler_password", "filler_salt");
            SudokuPuzzle testPuzzle2 = new SudokuPuzzle(true);

            context.RegisterUsers.Add(testUser2);
            context.SudokuPuzzles.Add(testPuzzle2);
            base.Seed(context);
        }
    }
}