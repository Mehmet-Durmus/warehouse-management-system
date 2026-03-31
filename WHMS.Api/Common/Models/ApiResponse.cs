namespace WHMS.Api.Common.Models;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public List<string>? Errors { get; set; }
    public List<string>? Warnings { get; set; }

    public static ApiResponse SuccessWithWarnings(List<string> warnings)
        => new() { Warnings = warnings, IsSuccess = true };
    public static ApiResponse Failure(List<string> errors)
        => new() { Errors = errors, IsSuccess = false };
    public static ApiResponse Failure(string error)
        => Failure(new List<string> { error });
}