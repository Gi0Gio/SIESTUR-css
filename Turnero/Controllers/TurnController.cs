using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Turnero.Models;

namespace Turnero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnController : Controller
    {
        // --- Context to access the database ---
        private readonly TurneroDataContext _context;
        public TurnController(TurneroDataContext context)
        {
            _context = context;
        }

        // ----- Endpoints -----

        // api/turn/getAllTurns - get all turns
        [HttpGet("getAllTurns")]
        public async Task<ActionResult<IEnumerable<Turn>>> GetAllTurns()
        {
            var user = await _context.Turns.ToListAsync();
            return Ok(user);
        }

        // api/turn/getTurnsOrdered - get turns ordered by turnNumber First given First retrived
        [HttpGet("getTurnsOrdered")]
        public async Task<ActionResult> GetTurnsOrdered()
        {
            var turns = await _context.Turns.OrderBy(t => t.TurnNumber).ToListAsync();
            return Ok(turns);
        }

        // api/turn/getLastTurn - get the last turn
        [HttpGet("getLastTurn")]
        public async Task<ActionResult> GetLastTurn()
        {
            var turn = await _context.Turns.OrderByDescending(t => t.TurnNumber).FirstOrDefaultAsync();
            if (turn == null)
                return NotFound();
            return Ok(turn);
        }

        // api/turn/addTurn - create a new turn
        [HttpPost("addTurn")]
        public async Task<ActionResult> AddTurn()
        {
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

            // Get the last attended turn (highest attended number)
            var lastAttended = await _context.Attends
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => a.TurnNumber)
                .FirstOrDefaultAsync();

            // Get the last turn created today
            var lastTurnToday = await _context.Turns
                .Where(t => t.TurnDate.Date == today)
                .OrderByDescending(t => t.TurnNumber)
                .FirstOrDefaultAsync();

            int newTurnNumber;

            if (lastTurnToday != null)
            {
                // Continue from the last generated turn today
                newTurnNumber = lastTurnToday.TurnNumber + 1;
            }
            else if (lastAttended > 0)
            {
                // If no turn was generated today but there is an attended turn, continue from it
                newTurnNumber = lastAttended + 1;
            }
            else
            {
                // Start fresh if no history exists
                newTurnNumber = 1;
            }

            var newTurn = new Turn
            {
                TurnNumber = newTurnNumber,
                TurnDate = DateTime.UtcNow
            };

            _context.Turns.Add(newTurn);
            await _context.SaveChangesAsync();

            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<TurnoHub>>();
            await hubContext.Clients.All.SendAsync("NewTurnAdded");

            return Ok(newTurn);
        }

        // api/turn/deleteAllTurns/- delete all turns
        [HttpDelete("deleteAllTurns")]
        public async Task<ActionResult> DeleteTurnsAsync() {
            var turn = _context.Turns;
            if (turn == null)
                return NotFound();
            _context.Turns.RemoveRange(turn);
            _context.SaveChanges();

            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<TurnoHub>>();
            await hubContext.Clients.All.SendAsync("TurnsDeleted");

            return Ok("Turns deleted");
        }
    }
}
