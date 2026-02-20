using System.Collections.Concurrent;

namespace NotesAPI.Middleware;

/// <summary>
/// Rate limiting middleware to prevent brute force attacks
/// Limits requests per IP address
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, RequestInfo> _requests = new();
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(RequestDelegate next, int maxRequests = 100, int timeWindowSeconds = 60)
    {
        _next = next;
        _maxRequests = maxRequests;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var endpoint = context.Request.Path.ToString();

        // Stricter limits for auth endpoints
        var maxAllowed = endpoint.Contains("/auth/") ? 10 : _maxRequests;

        var key = $"{ipAddress}:{endpoint}";

        // Clean up old entries
        CleanupOldEntries();

        if (_requests.TryGetValue(key, out var requestInfo))
        {
            if (DateTime.UtcNow - requestInfo.FirstRequest < _timeWindow)
            {
                if (requestInfo.Count >= maxAllowed)
                {
                    Console.WriteLine($"[RATE LIMIT] Blocked request from {ipAddress} to {endpoint} - Too many requests");
                    context.Response.StatusCode = 429; // Too Many Requests
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Too many requests. Please try again later.",
                        retryAfter = (int)(_timeWindow - (DateTime.UtcNow - requestInfo.FirstRequest)).TotalSeconds
                    });
                    return;
                }

                requestInfo.Count++;
                requestInfo.LastRequest = DateTime.UtcNow;
            }
            else
            {
                // Time window expired, reset
                _requests[key] = new RequestInfo
                {
                    FirstRequest = DateTime.UtcNow,
                    LastRequest = DateTime.UtcNow,
                    Count = 1
                };
            }
        }
        else
        {
            _requests[key] = new RequestInfo
            {
                FirstRequest = DateTime.UtcNow,
                LastRequest = DateTime.UtcNow,
                Count = 1
            };
        }

        await _next(context);
    }

    private void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow - _timeWindow;
        var keysToRemove = _requests
            .Where(kvp => kvp.Value.LastRequest < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requests.TryRemove(key, out _);
        }
    }

    private class RequestInfo
    {
        public DateTime FirstRequest { get; set; }
        public DateTime LastRequest { get; set; }
        public int Count { get; set; }
    }
}
