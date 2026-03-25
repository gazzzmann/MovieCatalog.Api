using MovieCatalog.Api.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCatalog.Api.Domain;

public class MovieTransaction
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public decimal LateFee { get; set; }
}
