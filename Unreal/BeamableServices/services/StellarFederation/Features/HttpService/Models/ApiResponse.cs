using System.Net;

namespace Beamable.StellarFederation.Features.HttpService.Models;

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public static ApiResponse<T> Success(HttpStatusCode statusCode, T? data = default)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Data = data,
            IsSuccess = true
        };
    }

    public static ApiResponse<T> Failure(HttpStatusCode statusCode, string errorMessage)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            IsSuccess = false
        };
    }
}