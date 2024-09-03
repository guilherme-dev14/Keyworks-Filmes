using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{movieId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByMovie(int movieId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .ToListAsync();

        if (reviews == null || !reviews.Any())
        {
            return NotFound();
        }

        return reviews;
    }

    [HttpPost]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
        if (review == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid review data.");
        }

        var movie = await _context.Movies.FindAsync(review.MovieId);
        if (movie == null)
        {
            return BadRequest("Invalid MovieId.");
        }

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReviewsByMovie), new { movieId = review.MovieId }, review);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<Review>>> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        var updatedReviews = await _context.Reviews.ToListAsync();
        return Ok(updatedReviews);
    }
}
