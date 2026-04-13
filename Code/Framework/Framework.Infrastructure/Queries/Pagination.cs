namespace Framework.Infrastructure.Queries;

public abstract class PaginationQuery
{
    public string? SortBy { get; set; }
    public bool SortDirection { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
public class PaginatedList<TItem>
{
    public static PaginatedList<TItem>
        Create(IEnumerable<TItem> items, int pageNumber, int pageSize, long totalCount) => new()
        {
            Results = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };


    public IEnumerable<TItem> Results { get; set; } =[];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }


   
}

