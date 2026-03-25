namespace MovieCatalog.Api.Application.DTOs.Review
{
    public class GetReviewsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
