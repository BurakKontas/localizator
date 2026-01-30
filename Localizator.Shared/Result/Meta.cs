using Localizator.Shared.Config;
using System;

namespace Localizator.Shared.Result;

public class Meta
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = AppConfig.ApiVersion;
    public string Duration { get; set; } = "00:00:00.000 000";
    public Pagination? Pagination { get; set; }
    public RateLimit? RateLimit { get; set; } = RateLimit.Empty();

    public static Meta Auto() => new();

    public static Meta AutoWithRateLimit(int remaining, DateTime resetAt)
    {
        return new Meta
        {
            RateLimit = new RateLimit(AppConfig.DefaultRateLimit, remaining, resetAt)
        };
    }

    public static Meta AutoWithPagination(Pagination pagination)
    {
        return new Meta { Pagination = pagination };
    }

    public Meta AddPagination(Pagination pagination)
    {
        Pagination = pagination;
        return this;
    }

    public Meta AddRateLimit(RateLimit rateLimit)
    {
        RateLimit = rateLimit;
        return this;
    }

    public Meta AddRateLimit(int remaining, DateTime resetAt)
    {
        RateLimit = RateLimit.Auto(remaining, resetAt);
        return this;
    }
}
