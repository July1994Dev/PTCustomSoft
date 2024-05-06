namespace MoviesApi.Domain.Models
{
    public class Director
    {
        public int DirectorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string BiographyPath { get; set; }
    }
}
