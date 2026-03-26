namespace WHMS.Api.Common.Models;

public record PaginationMeta
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPage { get; init; }

    public PaginationMeta(int page, int pageSize, int totlaPage)
    {
        Page = page;
        PageSize = pageSize;
        TotalPage = TotalPage;
    }
}