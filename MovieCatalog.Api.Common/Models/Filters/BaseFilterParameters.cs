using System;
using System.Collections.Generic;
using System.Text;

namespace MovieCatalog.Api.Common.Models.Filters;

public abstract class BaseFilterParameters
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortOrder { get; set; } = false;
}
