using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.HttpService.Models;

namespace Beamable.StellarFederation.Features.HttpService;

public class HttpClientService : IService {

    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    public async Task<ApiResponse<T>> Get<T>(string url, object? content = null,
        IDictionary<string, string>? headers = null, string? bearerToken = null)
    {
        return await SendRequest<T>(HttpMethod.Get, url, content, headers, bearerToken);
    }

    public async Task<ApiResponse<T>> Post<T>(string url, object? content = null,
        IDictionary<string, string>? headers = null, string? bearerToken = null)
    {
        return await SendRequest<T>(HttpMethod.Post, url, content, headers, bearerToken);
    }

    // Convenience methods for when you don't expect response data (returns object as placeholder)
    public async Task<ApiResponse<object>> Get(string url, object? content = null,
        IDictionary<string, string>? headers = null, string? bearerToken = null)
    {
        return await SendRequest<object>(HttpMethod.Get, url, content, headers, bearerToken);
    }

    public async Task<ApiResponse<object>> Post(string url, object? content = null,
        IDictionary<string, string>? headers = null, string? bearerToken = null)
    {
        return await SendRequest<object>(HttpMethod.Post, url, content, headers, bearerToken);
    }

    private async Task<ApiResponse<T>> SendRequest<T>(
        HttpMethod method,
        string url,
        object? content = null,
        IDictionary<string, string>? headers = null,
        string? bearerToken = null,
        int maxRetries = 5,
        int initialDelayMilliseconds = 500,
        CancellationToken cancellationToken = default)
    {
        var retryCount = 0;
        var delay = initialDelayMilliseconds;

        if (string.IsNullOrWhiteSpace(url))
            return ApiResponse<T>.Failure(HttpStatusCode.BadRequest, "URL cannot be null or empty.");

        while (retryCount < maxRetries)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                // Add Bearer token if provided
                if (!string.IsNullOrWhiteSpace(bearerToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
                }

                if (content is not null)
                {
                    if (content is UrlRequestBody body)
                    {
                        request.Content = new FormUrlEncodedContent(body);
                    }
                    else
                    {
                        var json = JsonSerializer.Serialize(content);
                        request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    }
                }

                if (headers is not null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (TransientStatusCodes.Contains(response.StatusCode))
                    {
                        retryCount++;
                        BeamableLogger.LogWarning($"Transient error {response.StatusCode} from {url}. Retry {retryCount}/{maxRetries}");

                        if (retryCount >= maxRetries)
                        {
                            BeamableLogger.LogWarning($"Max retries reached for transient error {response.StatusCode}.");
                            return ApiResponse<T>.Failure(response.StatusCode, $"Max retries reached with status code {response.StatusCode}");
                        }

                        await Task.Delay(delay, cancellationToken);
                        delay *= 2;
                        continue;
                    }

                    // Non-retryable HTTP status
                    BeamableLogger.LogWarning($"Non-retryable status code {response.StatusCode} from {url}: {response.ReasonPhrase}");
                    return ApiResponse<T>.Failure(response.StatusCode, response.ReasonPhrase ?? "Unknown error");
                }

                // Handle successful response
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                // If no response content expected (like 202 Accepted, 204 No Content)
                if (string.IsNullOrWhiteSpace(responseContent) ||
                    response.StatusCode == HttpStatusCode.NoContent ||
                    response.StatusCode == HttpStatusCode.Accepted)
                {
                    return ApiResponse<T>.Success(response.StatusCode);
                }

                // Try to deserialize response content
                try
                {
                    var deserializedData = JsonSerializer.Deserialize<T>(responseContent);
                    return ApiResponse<T>.Success(response.StatusCode, deserializedData);
                }
                catch (JsonException ex)
                {
                    BeamableLogger.LogError($"Deserialization failed for response: {responseContent}. Error: {ex.Message}");

                    // Return success with raw content warning if deserialization fails but HTTP was successful
                    BeamableLogger.LogWarning("Returning successful response without deserialized data due to JSON parsing error.");
                    return ApiResponse<T>.Success(response.StatusCode);
                }
            }
            catch (HttpRequestException ex) when (retryCount < maxRetries)
            {
                retryCount++;
                BeamableLogger.LogWarning($"Transient network error calling {url}: {ex.Message}. Retry {retryCount}/{maxRetries}");
                await Task.Delay(delay, cancellationToken);
                delay *= 2;
            }
            catch (TaskCanceledException ex) when (retryCount < maxRetries)
            {
                retryCount++;
                BeamableLogger.LogWarning($"Timeout calling {url}: {ex.Message}. Retry {retryCount}/{maxRetries}");
                await Task.Delay(delay, cancellationToken);
                delay *= 2;
            }
            catch (Exception ex)
            {
                BeamableLogger.LogError($"Unexpected error calling {url}: {ex.Message}");
                return ApiResponse<T>.Failure(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        BeamableLogger.LogWarning($"Max retries reached calling {url}.");
        return ApiResponse<T>.Failure(HttpStatusCode.RequestTimeout, "Exceeded maximum retries without success.");
    }

    private static readonly HashSet<HttpStatusCode> TransientStatusCodes =
    [
        HttpStatusCode.RequestTimeout, // 408
        HttpStatusCode.InternalServerError, // 500
        HttpStatusCode.BadGateway, // 502
        HttpStatusCode.ServiceUnavailable, // 503
        HttpStatusCode.GatewayTimeout // 504
    ];
}