namespace WHMS.Api.Common.Models;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public List<string>? Errors { get; set; }
    public List<string>? Warnings { get; set; }
    public PaginationMeta? Pagination { get; set; }

    public static ApiResponse SuccessWithWarnings(List<string> warnings)
        => new() { Warnings = warnings, IsSuccess = true };
}