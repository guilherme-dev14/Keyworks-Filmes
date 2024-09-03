using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MyProject.Data;
using MyProject.Models;

[Route("api/[controller]")]
[ApiController]
public class StreamingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StreamingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Streaming>> GetStreaming(int id)
    {
        var streaming = await _context.Streamings.FindAsync(id);

        if (streaming == null)
        {
            return NotFound();
        }

        return streaming;
    }
    [HttpPost]

    public async Task<ActionResult<Streaming>> PostStreaming(Streaming streaming)
    {
       var existingStreaming = await _context.Streamings
         .FirstOrDefaultAsync(s => s.Name.ToLower() == streaming.Name.ToLower());
        if (existingStreaming != null)
        {
            return Conflict($"A streaming service with the name '{streaming.Name}' already exists.");
        }
        
        _context.Streamings.Add(streaming);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStreaming), new { id = streaming.IdStreaming }, streaming);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<Streaming>>> DeleteStreaming(int id)
    {
        var streaming = await _context.Streamings.FindAsync(id);
        if (streaming == null)
        {
            return NotFound();
        }

        _context.Streamings.Remove(streaming);
        await _context.SaveChangesAsync();

        var updatedStreamings = await _context.Streamings.ToListAsync();
        return Ok(updatedStreamings);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Streaming>>> GetStreamings()
    {
        return await _context.Streamings.ToListAsync();
    }
}