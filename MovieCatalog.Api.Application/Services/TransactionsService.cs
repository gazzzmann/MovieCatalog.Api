using Microsoft.EntityFrameworkCore;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Transaction;
using MovieCatalog.Api.Application.Projections;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Enum;
using MovieCatalog.Api.Common.Models.Extensions;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Services;
// The Transaction Service
public class TransactionsService(
    MovieCatalogDbContext context,
    IUsersService usersService) : ITransactionsService
{
    public async Task<Result<PagedResult<TransactionDto>>> GetUserTransactionsAsync(
    PaginationParameters paginationParameters)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<PagedResult<TransactionDto>>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var transactions = await context.MovieTransactions
           .AsNoTracking()
           .Where(t => t.UserId == userId)
           .OrderByDescending(t => t.TransactionDate)
           .Select(TransactionProjections.ToDto)
           .ToPagedResultAsync(paginationParameters);
          
        return Result<PagedResult<TransactionDto>>.Success(transactions);
    }

    public async Task<Result<TransactionDto>> GetTransactionByIdAsync(int id)
    {
        var userId = usersService.UserId;

        var transaction = await context.MovieTransactions
            .Include(t => t.Movie)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction is null)
        {
            return Result<TransactionDto>.NotFound(
                new Error(ErrorCodes.NotFound, "Transaction not found"));
        }

        return Result<TransactionDto>.Success(
            MapToDto(transaction, transaction.Movie.Title));
    }
    public async Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionDto dto)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var movie = await context.Movies.FindAsync(dto.MovieId);

        if (movie is null)
        {
            return Result<TransactionDto>.NotFound(
                new Error(ErrorCodes.NotFound, $"Movie with ID {dto.MovieId} was not found"));
        }

        var user = await context.Users.FindAsync(userId);

        if (user is null)
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "User not found"));
        }

        // Prevent duplicate BUY
        var alreadyBought = await context.MovieTransactions
            .AnyAsync(t =>
                t.MovieId == dto.MovieId &&
                t.UserId == userId &&
                t.Type == TransactionType.Buy);

        if (dto.Type == TransactionType.Buy && alreadyBought)
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "You already purchased this movie"));
        }

        //Prevent RENT if already bought
        if (dto.Type == TransactionType.Rent && alreadyBought)
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "You already own this movie"));
        }

        //Prevent duplicate RENT (active rental)
        if (dto.Type == TransactionType.Rent)
        {
            var alreadyRented = await context.MovieTransactions
                .AnyAsync(t =>
                    t.MovieId == dto.MovieId &&
                    t.UserId == userId &&
                    t.Type == TransactionType.Rent &&
                    t.ReturnedAt == null);

            if (alreadyRented)
            {
                return Result<TransactionDto>.Failure(
                    new Error(ErrorCodes.Validation, "You already rented this movie and haven't returned it"));
            }
        }

        // Determine price
        var amount = dto.Type == TransactionType.Buy
            ? movie.BuyPrice
            : movie.RentPrice;

        // Wallet check
        if (user.WalletBalance < amount)
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "Insufficient wallet balance"));
        }

        user.WalletBalance -= amount;

        var transaction = new MovieTransaction
        {
            MovieId = movie.Id,
            UserId = userId,
            Type = dto.Type,
            Amount = amount,
            TransactionDate = DateTime.UtcNow,
            DueDate = dto.Type == TransactionType.Rent
                ? DateTime.UtcNow.AddDays(3)
                : null,
            LateFee = 0
        };

        context.MovieTransactions.Add(transaction);
        await context.SaveChangesAsync();

        return Result<TransactionDto>.Success(
            MapToDto(transaction, movie.Title));
    }
    public async Task<Result<TransactionDto>> ReturnMovieAsync(int transactionId)
    {
        var transaction = await context.MovieTransactions
            .Include(t => t.Movie)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction is null)
        {
            return Result<TransactionDto>.NotFound(
                new Error(ErrorCodes.NotFound,
                $"Transaction with ID {transactionId} was not found"));
        }

        if (transaction.ReturnedAt != null)
        {
            return Result<TransactionDto>.Failure(
                new Error(ErrorCodes.Validation, "Movie already returned"));
        }

        transaction.ReturnedAt = DateTime.UtcNow;

        if (transaction.DueDate.HasValue &&
            transaction.ReturnedAt > transaction.DueDate)
        {
            var lateDays = (transaction.ReturnedAt.Value - transaction.DueDate.Value).Days;

            if (lateDays > 0)
            {
                transaction.LateFee = lateDays * 2;

                var user = await context.Users.FindAsync(transaction.UserId);

                if (user != null)
                {
                    user.WalletBalance -= transaction.LateFee;
                }
            }
        }

        await context.SaveChangesAsync();

        return Result<TransactionDto>.Success(
            MapToDto(transaction, transaction.Movie.Title));
    }

    public async Task<Result<decimal>> TopUpWalletAsync(decimal amount)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<decimal>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        if (amount <= 0)
        {
            return Result<decimal>.Failure(
                new Error(ErrorCodes.Validation, "Amount must be greater than zero"));
        }

        var user = await context.Users.FindAsync(userId);

        if (user is null)
        {
            return Result<decimal>.Failure(
                new Error(ErrorCodes.Validation, "User not found"));
        }

        user.WalletBalance += amount;

        await context.SaveChangesAsync();

        return Result<decimal>.Success(user.WalletBalance);
    }

    public async Task<Result<decimal>> GetWalletBalanceAsync()
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<decimal>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var user = await context.Users.FindAsync(userId);

        if (user is null)
        {
            return Result<decimal>.Failure(
                new Error(ErrorCodes.Validation, "User not found"));
        }

        return Result<decimal>.Success(user.WalletBalance);
    }


    private static TransactionDto MapToDto(MovieTransaction t, string movieTitle) =>
        new()
        {
            Id = t.Id,
            MovieId = t.MovieId,
            MovieTitle = movieTitle,
            Type = t.Type,
            Amount = t.Amount,
            TransactionDate = t.TransactionDate,
            DueDate = t.DueDate,
            ReturnedAt = t.ReturnedAt,
            LateFee = t.LateFee
        };

}