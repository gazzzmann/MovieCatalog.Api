using MovieCatalog.Api.Application.DTOs.Transaction;
using MovieCatalog.Api.Domain;
using System.Linq.Expressions;

namespace MovieCatalog.Api.Application.Projections;

public static class TransactionProjections
{
    public static Expression<Func<MovieTransaction, TransactionDto>> ToDto =>
        t => new TransactionDto
        {
            Id = t.Id,
            MovieId = t.MovieId,
            MovieTitle = t.Movie.Title,
            Type = t.Type,
            Amount = t.Amount,
            TransactionDate = t.TransactionDate,
            DueDate = t.DueDate,
            ReturnedAt = t.ReturnedAt,
            LateFee = t.LateFee
        };
}
