using Turnero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Turnero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        // --- Context to access the database ---

        private readonly TurneroDataContext _context;

        public UserController(TurneroDataContext context)
        {
            _context = context;
        }

        // ----- Endpoints -----

        // api/user/getAllUsers - get all users
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersDTO = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            });
            return Ok(
               usersDTO 
                );
        }


        // api/user/{userId} - get a user by id 

        [HttpGet("getUserById/{userId}")]
        public async Task<ActionResult<UserDTO>> GetUserbyId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
;           }

            var userDto = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
            return userDto;
        }

        // api/user/{userId} - delete a user by id
        [HttpDelete("deleteUserById/{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("User Deleted");
        }

        // api/user/makeAdmin/{userId} - make user an admin by uid
        [HttpPut("makeAdmin/{userId}")]
        public async Task<ActionResult> MakeAdmin(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Role = "Admin";
            await _context.SaveChangesAsync();
            return Ok("User is now an Admin");
        }
    }
}
