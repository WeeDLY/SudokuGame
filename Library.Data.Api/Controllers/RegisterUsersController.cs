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
    public class RegisterUsersController : ApiController
    {
        private LibraryContext db = new LibraryContext();

        // GET: api/RegisterUsers
        public IQueryable<RegisterUser> GetRegisterUsers()
        {
            return db.RegisterUsers;
        }

        // GET: api/RegisterUsers/5
        [ResponseType(typeof(RegisterUser))]
        public async Task<IHttpActionResult> GetRegisterUser(int id)
        {
            RegisterUser registerUser = await db.RegisterUsers.FindAsync(id);
            if (registerUser == null)
            {
                return NotFound();
            }

            return Ok(registerUser);
        }

        // PUT: api/RegisterUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRegisterUser(int id, RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != registerUser.UserId)
            {
                return BadRequest();
            }

            db.Entry(registerUser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisterUserExists(id))
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

        // POST: api/RegisterUsers
        [ResponseType(typeof(RegisterUser))]
        public async Task<IHttpActionResult> PostRegisterUser(RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RegisterUsers.Add(registerUser);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = registerUser.UserId }, registerUser);
        }

        // DELETE: api/RegisterUsers/5
        [ResponseType(typeof(RegisterUser))]
        public async Task<IHttpActionResult> DeleteRegisterUser(int id)
        {
            RegisterUser registerUser = await db.RegisterUsers.FindAsync(id);
            if (registerUser == null)
            {
                return NotFound();
            }

            db.RegisterUsers.Remove(registerUser);
            await db.SaveChangesAsync();

            return Ok(registerUser);
        }

        // GET: api/RegisterUsers/{username}
        /// <summary>
        /// Gets the register user exists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [Route("api/RegisterUsers/exists/{username}")]
        [HttpGet]
        [ResponseType(typeof(RegisterUser))]
        public async Task<IHttpActionResult> GetRegisterUserExists(string username)
        {
            RegisterUser user = await (from u in db.RegisterUsers
                                        where u.Username == username
                                        select u).FirstOrDefaultAsync();
            if(user == default(RegisterUser))
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return Ok(user);
        }

        // PUT: api/RegisterUsers/{id}/{newUsername}
        /// <summary>
        /// Change Username
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="newUsername">The new username.</param>
        /// <returns></returns>
        [Route("api/RegisterUsers/{id}/{newUsername}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutChangeUsername(int id, string newUsername)
        {
            RegisterUser registerUser = await db.RegisterUsers.FindAsync(id);
            registerUser.Username = newUsername;
            db.Entry(registerUser).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RegisterUserExists(int id)
        {
            return db.RegisterUsers.Count(e => e.UserId == id) > 0;
        }
    }
}