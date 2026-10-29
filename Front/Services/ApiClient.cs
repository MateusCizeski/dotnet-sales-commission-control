using Front.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Front.Services
{
    /// <summary>The API answered 401: the token is missing or expired. The page filter sends the user to the login.</summary>
    public class ApiUnauthorizedException : Exception
    {
        public ApiUnauthorizedException() : base("The session expired.") { }
    }

    /// <summary>Result of a command: success, or the message the API returned (ProblemDetails).</summary>
    public record ApiResult(bool Success, string? Error)
    {
        public static readonly ApiResult Ok = new(true, null);
    }

    /// <summary>
    /// Typed HttpClient for the Sales Commission API. The bearer token is added by <see cref="BearerTokenHandler"/>.
    /// </summary>
    public class ApiClient
    {
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        // ---------- Auth ----------

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", new { email, password }, JsonOptions);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        }

        // ---------- Queries ----------

        /// <summary>
        /// Calls an ApiBase list endpoint. Filters and sort are sent as JSON arrays in the query string.
        /// </summary>
        public Task<PageResult<T>> GetPageAsync<T>(string path, int page, int limit, object[]? filters = null, object[]? sort = null)
        {
            var query = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["limit"] = limit.ToString()
            };

            if (filters is { Length: > 0 })
            {
                query["filter"] = JsonSerializer.Serialize(filters, JsonOptions);
            }

            if (sort is { Length: > 0 })
            {
                query["sort"] = JsonSerializer.Serialize(sort, JsonOptions);
            }

            return GetAsync<PageResult<T>>(QueryHelpers.AddQueryString(path, query));
        }

        public async Task<T> GetAsync<T>(string path) where T : new()
        {
            var response = await _http.GetAsync(path);
            ThrowIfUnauthorized(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new T();
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions) ?? new T();
        }

        public async Task<T?> FindAsync<T>(string path) where T : class
        {
            var response = await _http.GetAsync(path);
            ThrowIfUnauthorized(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        }

        // ---------- Commands ----------

        public async Task<ApiResult> PostAsync(string path, object? body = null)
        {
            var response = body == null ? await _http.PostAsync(path, null) : await _http.PostAsJsonAsync(path, body, JsonOptions);
            return await ToResultAsync(response);
        }

        public async Task<ApiResult> PutAsync(string path, object body)
        {
            return await ToResultAsync(await _http.PutAsJsonAsync(path, body, JsonOptions));
        }

        public async Task<ApiResult> DeleteAsync(string path)
        {
            return await ToResultAsync(await _http.DeleteAsync(path));
        }

        private static async Task<ApiResult> ToResultAsync(HttpResponseMessage response)
        {
            ThrowIfUnauthorized(response);

            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Ok;
            }

            return new ApiResult(false, await ReadProblemAsync(response));
        }

        /// <summary>Reads "detail" (or "title", or the validation errors) of a ProblemDetails response.</summary>
        private static async Task<string> ReadProblemAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return "You do not have permission to perform this action.";
            }

            try
            {
                using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = document.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    return string.Join(" ", errors.EnumerateObject().SelectMany(e => e.Value.EnumerateArray()).Select(m => m.GetString()));
                }

                if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                {
                    return detail.GetString()!;
                }

                if (root.TryGetProperty("title", out var title))
                {
                    return title.GetString() ?? "The request failed.";
                }
            }
            catch (JsonException)
            {
                // Not a ProblemDetails body.
            }

            return "The request failed.";
        }

        private static void ThrowIfUnauthorized(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ApiUnauthorizedException();
            }
        }
    }
}
