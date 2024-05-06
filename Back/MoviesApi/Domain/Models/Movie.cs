namespace MoviesApi.Domain.Models
{
    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int DirectorID { get; set; }
        public int GenreID { get; set; }
        public string PosterPath { get; set; }
        public string TrailerPath { get; set; }
    }
}
