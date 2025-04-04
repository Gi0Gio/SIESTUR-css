using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.RegularExpressions;

[Route("api/[controller]")]
[ApiController]
public class VideoController : Controller
{
    private readonly string videoFile = "videos.json";
    private readonly IHubContext<TurnoHub> _hubContext;

    public VideoController(IHubContext<TurnoHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpGet("getVideos")]
    public async Task<ActionResult> GetVideos()
    {
        if (!System.IO.File.Exists(videoFile)) return Ok(new List<string>());

        var videos = await System.IO.File.ReadAllTextAsync(videoFile);
        return Ok(JsonSerializer.Deserialize<List<string>>(videos));
    }

    [HttpPost("addVideo")]
    public async Task<IActionResult> AddVideo([FromBody] VideoDTO videoDto)
    {
        if (videoDto == null || string.IsNullOrWhiteSpace(videoDto.Code))
        {
            return BadRequest(new { message = "Invalid video ID" });
        }

        // ✅ Convert ID to embed URL
        string embedUrl = $"https://www.youtube.com/embed/{videoDto.Code}";

        List<string> videos;
        if (System.IO.File.Exists(videoFile))
        {
            videos = JsonSerializer.Deserialize<List<string>>(await System.IO.File.ReadAllTextAsync(videoFile)) ?? new List<string>();
        }
        else
        {
            videos = new List<string>();
        }

        videos.Add(embedUrl);
        await System.IO.File.WriteAllTextAsync(videoFile, JsonSerializer.Serialize(videos));

        await _hubContext.Clients.All.SendAsync("VideoListUpdated");

        return Ok(new { url = embedUrl });
    }

    [HttpDelete("deleteVideo/{index}")]
    public async Task<ActionResult> DeleteVideo(int index)
    {
        if (!System.IO.File.Exists(videoFile)) return NotFound();

        var videos = JsonSerializer.Deserialize<List<string>>(await System.IO.File.ReadAllTextAsync(videoFile)) ?? new List<string>();

        if (index < 0 || index >= videos.Count) return BadRequest("Invalid index");

        videos.RemoveAt(index);
        await System.IO.File.WriteAllTextAsync(videoFile, JsonSerializer.Serialize(videos));

        await _hubContext.Clients.All.SendAsync("VideoListUpdated");

        return Ok(videos);
    }

    // ✅ DTO for Video Request
    public class VideoDTO
    {
        public string Code { get; set; }
    }
}