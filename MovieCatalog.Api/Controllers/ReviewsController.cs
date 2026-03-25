using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Review;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Paging;

namespace MovieCatalog.Api.Controllers;

[Route("api/movies/{movieId:int}/reviews")]
[ApiController]
[Authorize]
public class ReviewsController(IReviewsService reviewsService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<GetMovieWithReviewsDto>> GetMovieWithReviews(
    [FromRoute] int movieId,
    [FromQuery] PaginationParameters pagination)
    {
        var result = await reviewsService.GetMovieWithReviewsAsync(
            movieId,
            pagination.PageNumber,
            pagination.PageSize);

        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<GetReviewsDto>> AddReviewForMovie
        (
          [FromRoute] int movieId,
          [FromBody] CreateReviewDto reviewDto
        )
    {
        var result = await reviewsService.AddReviewForMovieAsync(movieId, reviewDto);
        return ToActionResult(result);
    }

    [HttpPut("{reviewId:int}")]
    public async Task<ActionResult<GetReviewsDto>> UpdateReviewForMovie
        (
          [FromRoute] int movieId,
          [FromRoute] int reviewId,
          [FromBody] UpdateReviewDto reviewDto
        )
    {
        var result = await reviewsService.UpdateReviewForMovieAsync(movieId, reviewId, reviewDto);
        return ToActionResult(result);
    }

    [HttpDelete("{reviewId:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
    public async Task<ActionResult> DeleteReviewForMovie(
    [FromRoute] int movieId,
    [FromRoute] int reviewId)
    {
        var result = await reviewsService.DeleteReviewForMovieAsync(movieId, reviewId);
        return ToActionResult(result);
    }

}
