using MovieCatalog.Api.Common.Models.Paging;

namespace MovieCatalog.Api.Application.DTOs.Review
{
    public class GetMovieWithReviewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public List<GetReviewsDto> Reviews { get; set; } = new();
        public PaginationMetadata ReviewsPagination { get; set; } = new();
    }
}
