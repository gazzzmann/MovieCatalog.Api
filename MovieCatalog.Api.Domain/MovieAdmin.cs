namespace MovieCatalog.Api.Domain;

    public class MovieAdmin
    {
       public int Id { get; set; }

        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
