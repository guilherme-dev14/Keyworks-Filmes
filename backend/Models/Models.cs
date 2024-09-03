using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyProject.Models{
public class Movie
{ 
    [Key]
    public int IdMovie { get; set; }
    public required string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    public int StreamingId { get; set; }
    public Streaming? Streaming { get; set; }

    public byte[]? Image { get; set; }
    public int Rating { get; set; }
}

public class Genre
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Movie>? Movies { get; set; }
}

public class Review
{
    [Key]
    public int IdReview { get; set; }
    public int MovieId { get; set; }
    public required string Comment { get; set; }
    
}

public class Streaming
{
    [Key]
    public int IdStreaming { get; set; }
    public required string Name { get; set; }
    public ICollection<Movie>? Movie { get; set; }
}
public class MovieDTO
{
    public int IdMovie { get; set; }
    public required string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int GenreId { get; set; }
    public int StreamingId { get; set; }
    public string? ImageBase64 { get; set; }
    public int Rating { get; set; }
}
}