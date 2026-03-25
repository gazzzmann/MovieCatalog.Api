using MovieCatalog.Api.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCatalog.Api.Application.DTOs.Transaction;

public class CreateTransactionDto
{
    public int MovieId { get; set; }
    public TransactionType Type { get; set; } 
}
