using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models; 

[Route("api/[controller]")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly AppDbContext _context;

    public GenresController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            return NotFound();
        }

        return genre;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    [HttpPost]

    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        var existingGenre = await _context.Genres
          .FirstOrDefaultAsync(g => g.Name.ToLower() == genre.Name.ToLower());
        if (existingGenre != null)  
        {
            return Conflict($"A genre with the name '{genre.Name}' already exists.");
        }

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<Genre>>> DeleteGenre(int id)
    {
    var genre = await _context.Genres.FindAsync(id);
    if (genre == null)
    {
        return NotFound();
    }
    
    _context.Genres.Remove(genre);
    await _context.SaveChangesAsync();
    
    var updatedGenres = await _context.Genres.ToListAsync();
    return Ok(updatedGenres);
    }
}