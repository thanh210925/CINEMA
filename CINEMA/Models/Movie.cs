﻿using System;
using System.Collections.Generic;

namespace CINEMA.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Duration { get; set; }

    public string? TrailerUrl { get; set; }

    public string? PosterUrl { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? Language { get; set; }

    public string? Country { get; set; }

    public string? AgeRating { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
