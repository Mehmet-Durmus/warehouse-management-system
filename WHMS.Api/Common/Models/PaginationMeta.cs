using WHMS.Application.Common.DTOs;

namespace WHMS.Api.Common.Models;

public record PaginationMeta
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPage { get; init; }

    public PaginationMeta(int page, int pageSize, int totalPage)
    {
        Page = page;
        PageSize = pageSize;
        TotalPage = totalPage;
    }

    public PaginationMeta(PaginationDto paginationDto)
    {
        Page = paginationDto.Page;
        PageSize = paginationDto.PageSize;
        TotalPage = paginationDto.TotalPage;
    }
}