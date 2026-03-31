using System.IO.Compression;

namespace WHMS.Api.Common.Models;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
    public PaginationMeta? Pagination { get; set; }
    
    public static ApiResponse<T> Success(T data) 
        => new() { Data = data, IsSuccess = true };

    public static ApiResponse<T> SuccessWithWarnings(T data, List<string> warnings)
        => new() { Data = data, Warnings = warnings, IsSuccess = true };
    
    public static ApiResponse<T> SuccessList(T data, PaginationMeta pagination)
        => new() { Data = data, Pagination = pagination, IsSuccess = true };
}