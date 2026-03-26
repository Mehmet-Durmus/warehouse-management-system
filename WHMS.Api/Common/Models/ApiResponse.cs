using System.IO.Compression;

namespace WHMS.Api.Common.Models;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public List<string>? Errors { get; set; }
    public List<string>? Warnings { get; set; }
    public PaginationMeta? Pagination { get; set; }

    public static ApiResponse<T> Success(T data) 
        => new() { Data = data, IsSuccess = true };

    public static ApiResponse<T> SuccessWithWarnings(T data, List<string> warnings)
        => new() { Data = data, Warnings = warnings, IsSuccess = true };
    
    public static ApiResponse<T> SuccessList(T data, PaginationMeta pagination)
        => new() { Data = data, Pagination = pagination, IsSuccess = true };

    public static ApiResponse<T> Failure(List<string> errors)
        => new() { Errors = errors, IsSuccess = false };
    public static ApiResponse<T> Failure(string error)
        => Failure(new List<string> { error });
}