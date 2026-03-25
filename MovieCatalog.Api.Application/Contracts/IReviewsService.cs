using MovieCatalog.Api.Application.DTOs.Review;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts
{
    public interface IReviewsService
    {
        Task<Result<GetReviewsDto>> AddReviewForMovieAsync(int movieId, CreateReviewDto reviewDto);
        Task<Result> DeleteReviewForMovieAsync(int movieId, int reviewId);
        Task<Result<GetMovieWithReviewsDto>> GetMovieWithReviewsAsync(int movieId, int pageNumber, int pageSize);
        Task<Result<GetReviewsDto>> UpdateReviewForMovieAsync(int movieId, int reviewId, UpdateReviewDto dto);
    }
}