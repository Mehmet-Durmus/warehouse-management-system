namespace WHMS.Application.Common.DTOs;

public record PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }

    public PaginationDto(int page, int pageSize, int totalPage)
    {
        Page = page;
        PageSize = pageSize;
        TotalPage = totalPage;
    }
}