using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Transaction;
using MovieCatalog.Api.Common.Models.Paging;

namespace MovieCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController(ITransactionsService transactionsService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto dto)
    {
        var result = await transactionsService.CreateTransactionAsync(dto);
        return ToActionResult(result);
    }

    [HttpPost("{transactionId}/return")]
    public async Task<ActionResult<TransactionDto>> ReturnMovie(int transactionId)
    {
        var result = await transactionsService.ReturnMovieAsync(transactionId);
        return ToActionResult(result);
    }

    [HttpGet("my-transactions")]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetMyTransactions(
        [FromQuery] PaginationParameters paginationParameters)
    {
        var result = await transactionsService
            .GetUserTransactionsAsync(paginationParameters);

        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(int id)
    {
        var result = await transactionsService.GetTransactionByIdAsync(id);
        return ToActionResult(result);
    }

    [HttpPost("top-up")]
    public async Task<ActionResult<decimal>> TopUpWallet([FromBody] TopUpWalletDto dto)
    {
        var result = await transactionsService.TopUpWalletAsync(dto.Amount);
        return ToActionResult(result);
    }

    [HttpGet("balance")]
    public async Task<ActionResult<decimal>> GetWalletBalance()
    {
        var result = await transactionsService.GetWalletBalanceAsync();
        return ToActionResult(result);
    }

}