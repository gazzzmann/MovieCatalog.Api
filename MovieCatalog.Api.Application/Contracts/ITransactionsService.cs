using MovieCatalog.Api.Application.DTOs.Transaction;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts;

public interface ITransactionsService
{
    Task<Result<TransactionDto>> CreateTransactionAsync(CreateTransactionDto dto);
    Task<Result<TransactionDto>> GetTransactionByIdAsync(int id);
    Task<Result<PagedResult<TransactionDto>>> GetUserTransactionsAsync(PaginationParameters paginationParameters);
    Task<Result<decimal>> GetWalletBalanceAsync();
    Task<Result<TransactionDto>> ReturnMovieAsync(int transactionId);
    Task<Result<decimal>> TopUpWalletAsync(decimal amount);
}