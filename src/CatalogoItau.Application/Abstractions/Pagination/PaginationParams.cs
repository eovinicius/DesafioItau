namespace CatalogoItau.Application.Abstractions.Pagination;

public sealed class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public PaginationParams()
    {
    }

    public PaginationParams(int page, int pageSize)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
    }

    public int GetSkip() => (Page - 1) * PageSize;
}