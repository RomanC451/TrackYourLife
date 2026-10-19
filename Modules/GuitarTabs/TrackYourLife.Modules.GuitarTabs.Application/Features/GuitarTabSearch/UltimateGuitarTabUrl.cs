namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public static class UltimateGuitarTabUrl
{
    public static bool TryCreate(string? value, out Uri tabUrl)
    {
        tabUrl = null!;

        if (
            !Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps
            || !uri.Host.Equals("tabs.ultimate-guitar.com", StringComparison.OrdinalIgnoreCase)
            || !uri.AbsolutePath.StartsWith("/tab/", StringComparison.OrdinalIgnoreCase)
        )
        {
            return false;
        }

        tabUrl = new UriBuilder(uri) { Query = string.Empty, Fragment = string.Empty }.Uri;
        return true;
    }
}
