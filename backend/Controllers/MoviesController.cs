using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MyProject.Data;
using MyProject.Models;


[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MoviesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return movie;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        var movies = await _context.Movies.ToListAsync();

        var movieDTOs = movies.Select(movie => new MovieDTO
        {
            IdMovie = movie.IdMovie,
            Title = movie.Title,
            ReleaseDate = movie.ReleaseDate,
            GenreId = movie.GenreId,
            StreamingId = movie.StreamingId,
            ImageBase64 = movie.Image != null ?
            "data:image/jpeg;base64," + Convert.ToBase64String(movie.Image) : null,
            Rating = movie.Rating
        }).ToList();

        return Ok(movieDTOs);

    }

    [HttpPost]
    public async Task<ActionResult<Movie>> PostMovie([FromForm] Movie movie, IFormFile? image)
    {
        

        if (image != null)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                movie.Image = ms.ToArray();
            }
        }
        var existingMovie = await _context.Movies
        .FirstOrDefaultAsync(m => m.Title.ToLower() == movie.Title.ToLower());
        if (existingMovie != null)
        {
            return Conflict($"A movie with the title '{movie.Title}' already exists.");
        }
        // Verifica se o gênero existe
        var rating = await _context.Movies.FindAsync(movie.Rating);
        var genre = await _context.Genres.FindAsync(movie.GenreId);
        if (genre == null)
        {
            return NotFound($"Genre with ID {movie.GenreId} not found.");
        }
        var streaming = await _context.Streamings.FindAsync(movie.StreamingId);
        if (streaming == null)
        {
            return NotFound($"Streaming with ID {movie.StreamingId} not found.");
        }
        // Adiciona o filme ao contexto e salva as alterações
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMovie", new { id = movie.IdMovie }, movie);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<Movie>>> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        var updatedMovies = await _context.Movies.ToListAsync();
        return Ok(updatedMovies);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Movie>> PutMovie([FromForm] Movie movie, IFormFile? image)
    {
        if (image != null)
        {
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                movie.Image = ms.ToArray();
            }
        }
        _context.Entry(movie).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(movie);
    }

}