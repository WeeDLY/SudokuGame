using Library.DataAccess;
using Library.Model;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Library.Data.Api.Controllers
{
    public class SudokuPuzzlesController : ApiController
    {
        private LibraryContext db = new LibraryContext();

        // GET: api/SudokuPuzzles
        public IQueryable<SudokuPuzzle> GetSudokuPuzzles()
        {
            return db.SudokuPuzzles;
        }

        // GET: api/SudokuPuzzles/5
        [ResponseType(typeof(SudokuPuzzle))]
        public async Task<IHttpActionResult> GetSudokuPuzzle(int id)
        {
            SudokuPuzzle sudokuPuzzle = await db.SudokuPuzzles.FindAsync(id);
            if (sudokuPuzzle == null)
            {
                return NotFound();
            }

            return Ok(sudokuPuzzle);
        }

        // PUT: api/SudokuPuzzles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSudokuPuzzle(int id, SudokuPuzzle sudokuPuzzle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sudokuPuzzle.SudokuPuzzleId)
            {
                return BadRequest();
            }

            db.Entry(sudokuPuzzle).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SudokuPuzzleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SudokuPuzzleExists(int id)
        {
            return db.SudokuPuzzles.Count(e => e.SudokuPuzzleId == id) > 0;
        }

        // GET: api/SudokuPuzzles/users/userId
        /// <summary>
        /// Gets the sudoku puzzles by UserId
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SudokuPuzzles/users/{userId}")]
        [ResponseType(typeof(SudokuPuzzle))]
        public IQueryable GetSudokuPuzzlesByUser(int userId)
        {
            return from r in db.RegisterUsers
                   where r.UserId == userId
                   select r.SudokuPuzzles;
        }

        // DELETE: api/SudokuPuzzles/5
        /// <summary>
        /// Deletes the sudoku puzzle.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [ResponseType(typeof(SudokuPuzzle))]
        public async Task<IHttpActionResult> DeleteSudokuPuzzle(int id)
        {
            SudokuPuzzle sudokuPuzzle = await db.SudokuPuzzles.FindAsync(id);
            if (sudokuPuzzle == null)
            {
                return NotFound();
            }

            db.SudokuPuzzles.Remove(sudokuPuzzle);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(sudokuPuzzle);
        }

        // POST: api/SudokuPuzzles
        /// <summary>
        /// Posts the sudoku puzzle.
        /// </summary>
        /// <param name="sudokuPuzzle">The sudoku puzzle.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [Route("api/SudokuPuzzles/add/{userId}")]
        [ResponseType(typeof(SudokuPuzzle))]
        public async Task<IHttpActionResult> PostSudokuPuzzle(SudokuPuzzle sudokuPuzzle, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RegisterUser user = await db.RegisterUsers.FindAsync(userId);
            if (user == null)
                return StatusCode(HttpStatusCode.NotFound);

            sudokuPuzzle.RegisterUsers.Add(user);

            db.SudokuPuzzles.Add(sudokuPuzzle);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }
    }
}