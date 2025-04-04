using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Turnero.Models;

namespace Turnero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WindowController : Controller
    {
        // --- Context to access the database ---
        private readonly TurneroDataContext _context;
        public WindowController(TurneroDataContext context)
        {
            _context = context;
        }
        // ----- Endpoints -----
        // Get: api/window/getWindows - get all windows
        [HttpGet("getWindows")]
        public async Task<ActionResult<IEnumerable<Window>>> GetAllWindows()
        {
            var windows = await _context.Windows.ToListAsync();
            return Ok(windows);
        }

        // Get: api/window/getWindowsById/{windowId} - get windows by id
        [HttpGet("getWindowsById/{windowId}")]
        public async Task<ActionResult> GetWindowsById(int windowId)
        {
            var windows = await _context.Windows.FindAsync(windowId);
            if (windows == null)
                return NotFound();
            return Ok(windows);
        }

        //Get: api/window/getLastWindowsAttentionByWindowId/{windowId} - get last attention by window id
        [HttpGet("getLastWindowsAttentionByWindowId/{windowId}")]
        public async Task<ActionResult> GetLastWindowsAttentionByWindowId(int windowId)
        {
            var attends = await _context.Attends.Where(a => a.WindowId == windowId).OrderByDescending(a => a.AttendanceDate).FirstOrDefaultAsync();
            if (attends == null)
                return NotFound();
            return Ok(attends);
        }


        //Get: api/window/getWindowAttentionHistory/{windowId} - get all attends by window id
        [HttpGet("getWindowAttentionHistory/{windowId}")]
        public async Task<ActionResult> GetWindowAttentionHistory(int windowId)
        {
            var attends = await _context.Attends.Where(a => a.WindowId == windowId).ToListAsync();
            if (attends == null)
                return NotFound();
            return Ok(attends);
        }

        // Post: api/window/addWindow - create a new window 
        [HttpPost("addWindow/{userId}")]
        public async Task<ActionResult> AddWindow(int userId, WindowDTOCreate windowDTOCreate)
        {
            var window = new Window
            {
                WindowName = windowDTOCreate.WindowName,
                UserId = userId,
            };
            _context.Windows.Add(window);
            await _context.SaveChangesAsync();
            return Ok(window);
        }

        // Put: api/window/updateWindowUser - udate windows user.
        [HttpPut("updateWindowUser/{windowId}/{userId}")]
        public async Task<ActionResult> UpdateWindowUser(int windowId, int userId)
        {
            var window = await _context.Windows.FindAsync(windowId);
            if(window == null)
                return NotFound();

            window.UserId = userId;
            await _context.SaveChangesAsync();
            return Ok(window);
        }

        // Delete: api/window/deleteWindow/{id} - delete a window by id
        [HttpDelete("deleteWindow/{windowId}")]
        public async Task<ActionResult> DeleteWindow(int windowId)
        {
            var window = await _context.Windows.FindAsync(windowId);
            if (window == null)
                return NotFound();
            _context.Windows.Remove(window);
            await _context.SaveChangesAsync();
            return Ok("Window Deleted");
        }
    }
}
