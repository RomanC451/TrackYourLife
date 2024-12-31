using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Common.Domain.Features.Cookies;

public sealed class Cookie : Entity<CookieId>
{
    public string Name { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public string Domain { get; private set; } = string.Empty;

    public string Path { get; private set; } = string.Empty;


    private Cookie()
        : base() { }

    private Cookie(CookieId id, string name, string value, string domain, string path)
        : base(id)
    {
        Name = name;
        Value = value;
        Domain = domain;
        Path = path;
    }

    public static Result<Cookie> Create(
        CookieId id,
        string name,
        string value,
        string domain,
        string path
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(id))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(name))
            ),
            Ensure.NotEmpty(
                value,
                DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(value))
            ),
            Ensure.NotEmpty(
                domain,
                DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(domain))
            ),
            Ensure.NotEmpty(
                path,
                DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(path))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Cookie>(result.Error);
        }

        var cookie = new Cookie(id, name, value, domain, path);

        return Result.Success(cookie);
    }

    public Result UpdateValue(string value)
    {
        var result = Ensure.NotEmpty(
            value,
            DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(value))
        );

        if (result.IsFailure)
        {
            return result;
        }

        Value = value;

        return Result.Success();
    }
}
