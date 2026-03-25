using Microsoft.EntityFrameworkCore;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Review;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Services;

public class ReviewsService
    (MovieCatalogDbContext context, IUsersService usersService) : IReviewsService
{

    public async Task<Result<GetMovieWithReviewsDto>> GetMovieWithReviewsAsync(
    int movieId,
    int pageNumber = 1,
    int pageSize = 10)
    {
        var movie = await context.Movies
            .Where(m => m.Id == movieId)
            .Select(m => new GetMovieWithReviewsDto
            {
                Id = m.Id,
                Title = m.Title,

                AverageRating = m.Reviews
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0,

                ReviewsCount = m.Reviews.Count(),

                Reviews = m.Reviews
                    .OrderByDescending(r => r.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new GetReviewsDto
                    {
                        Id = r.Id,
                        UserName = r.User!.UserName!,
                        Rating = r.Rating,
                        Comment = r.Comment
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (movie is null)
            return Result<GetMovieWithReviewsDto>.Failure(
                new Error(ErrorCodes.NotFound, "Movie not found.")
            );

        var totalPages = (int)Math.Ceiling((double)movie.ReviewsCount / pageSize);

        movie.ReviewsPagination = new PaginationMetadata
        {
            TotalCount = movie.ReviewsCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            HasNext = pageNumber < totalPages,
            HasPrevious = pageNumber > 1
        };

        return Result<GetMovieWithReviewsDto>.Success(movie);
    }

    public async Task<Result<GetReviewsDto>> AddReviewForMovieAsync(int movieId, CreateReviewDto reviewDto)
    {
        var movieExists = await context.Movies
              .AnyAsync(m => m.Id == movieId);
        if (!movieExists)
            return Result<GetReviewsDto>.Failure
                (new Error(ErrorCodes.NotFound, $"Movie with id {movieId} not found."));

        var userId = usersService.UserId;


        if (string.IsNullOrEmpty(userId))
            return Result<GetReviewsDto>.Failure
                (new Error(ErrorCodes.Validation, "User is not authenticated."));

        var reviewExists = await context.Reviews
            .AnyAsync(r => r.MovieId == movieId && r.UserId == userId);

        if (reviewExists)
            return Result<GetReviewsDto>.Failure(
                new Error(ErrorCodes.Validation, "You have already reviewed this movie.")
            );

        var review = new Review
        {
            MovieId = movieId,
            UserId = userId,
            Rating = reviewDto.Rating,
            Comment = reviewDto.Comment
        };
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        var createdReview = await context.Reviews
           .Where(r => r.Id == review.Id)
           .Select(r => new GetReviewsDto
           {
               Id = r.Id,
               UserName = r.User!.UserName!,
               Rating = r.Rating,
               Comment = r.Comment
           })
              .FirstAsync();
        return Result<GetReviewsDto>.Success(createdReview);
    }

    public async Task<Result<GetReviewsDto>> UpdateReviewForMovieAsync(int movieId, int reviewId, UpdateReviewDto dto)
    {
        var movieExists = await context.Movies
            .AnyAsync(m => m.Id == movieId);

        if (!movieExists)
            return Result<GetReviewsDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Movie with id {movieId} not found.")
            );

        var userId = usersService.UserId;

        if (string.IsNullOrEmpty(userId))
            return Result<GetReviewsDto>.Failure(
                new Error(ErrorCodes.Validation, "User is not authenticated.")
            );

        var review = await context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.MovieId == movieId);

        if (review is null)
            return Result<GetReviewsDto>.Failure(
                new Error(ErrorCodes.NotFound, "Review not found.")
            );

        // Check if user is admin
        var isAdmin = usersService.IsInRole(AppRoles.Admin);

        // If not admin, ensure the user owns the review
        if (!isAdmin && review.UserId != userId)
            return Result<GetReviewsDto>.Failure(
                new Error(ErrorCodes.Validation, "You can only update your own review.")
            );

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;

        await context.SaveChangesAsync();

        var updatedReview = await context.Reviews
            .Where(r => r.Id == review.Id)
            .Select(r => new GetReviewsDto
            {
                Id = r.Id,
                UserName = r.User!.UserName!,
                Rating = r.Rating,
                Comment = r.Comment
            })
            .FirstAsync();

        return Result<GetReviewsDto>.Success(updatedReview);
    }

    public async Task<Result> DeleteReviewForMovieAsync(int movieId, int reviewId)
    {
        var movieExists = await context.Movies
            .AnyAsync(m => m.Id == movieId);

        if (!movieExists)
            return Result.Failure(
                new Error(ErrorCodes.NotFound, $"Movie with id {movieId} not found.")
            );

        var review = await context.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.MovieId == movieId);

        if (review is null)
            return Result.Failure(
                new Error(ErrorCodes.NotFound, "Review not found.")
            );

        context.Reviews.Remove(review);
        await context.SaveChangesAsync();

        return Result.Success();
    }

}
