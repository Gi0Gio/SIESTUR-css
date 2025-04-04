using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Turnero.Models;

namespace Turnero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendController : Controller
    {
        // --- Context to access the database ---
        private readonly TurneroDataContext _context;
        public AttendController(TurneroDataContext context)
        {
            _context = context;
        }

        // ----- Endpoints -----

        // api/attend/getAllAttentions - get all attends
        [HttpGet("getAllAttentions")]
        public async Task<ActionResult<IEnumerable<Attend>>> GetAllAttentions()
        {
            var attends = await _context.Attends.ToListAsync();
            return Ok(attends);
        }

        // api/attend/getAttendsByUserId/{windowId} - get last attend by windowId
        [HttpGet("getLastAttendByWindowId/{windowId}")]
        public async Task<ActionResult> GetAttendsByUserId(int windowId)
        {
            var attends = await _context.Attends.Where(a => a.WindowId == windowId).OrderByDescending(a => a.AttendanceDate).ToListAsync();
            if (attends == null)
                return NotFound(new { message = "No attended turns yet" }); // ✅ Send message instead of just 404
            return Ok(attends);
        }

        // api/attend/getAttendsByUserId/{windowId}/{userId} - Attent the last turn aveliable, by creating a new attend, and deleting the last turn from the list, and returning the attend.
        [HttpPost("attendLastTurn/{windowId}/{userId}")]
        public async Task<ActionResult> AttendLastTurn(int windowId, int userId)
        {
            var lastTurn = await _context.Turns.FirstOrDefaultAsync(); // Get the last turn
            if (lastTurn == null)
                return NotFound("No turns available");
            var attend = new Attend
            {
                TurnNumber = lastTurn.TurnNumber,
                UserId = userId,
                WindowId = windowId,
                AttendanceDate = DateTime.Now
            };
            try
            {
                _context.Attends.Add(attend);
                await _context.SaveChangesAsync();  // ✅ Save the Attend first

                _context.Turns.Remove(lastTurn); // Remove the last turn
                await _context.SaveChangesAsync(); // ✅ Save the Turn removal

                var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<TurnoHub>>();
                await hubContext.Clients.All.SendAsync("TurnUpdated");

                return Ok(attend);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Saving to Database: {ex.Message}");
                return BadRequest("Error Saving to Database" + ex);
            }
        }
        // api/attend/deleteAllAttentions - delete all attends
        [HttpDelete("deleteAllAttentions")]
        public async Task<ActionResult> DeleteAllAttentions()
        {
            _context.Attends.RemoveRange(_context.Attends);
            await _context.SaveChangesAsync();
            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<TurnoHub>>();
            await hubContext.Clients.All.SendAsync("AttentionsDeleted");
            return Ok();
        }
    }
}
