using MovieCatalog.Api.Common.Enum;

namespace MovieCatalog.Api.Application.DTOs.Transaction;

public class TransactionDto
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public decimal LateFee { get; set; }
}