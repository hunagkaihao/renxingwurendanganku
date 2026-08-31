using System;
using System.Text.RegularExpressions;

namespace Wcs.ConfigTool;

public static class SelfCallEndpoint
{
    // A Kestrel listener list is not an HTTP client URL. Use the first binding,
    // replacing all-interface hosts with the corresponding loopback address.
    public static string Resolve(string listenerUrls)
    {
        var addresses = (listenerUrls ?? string.Empty).Split(',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (addresses.Length == 0)
            throw new ArgumentException("Wcs:BaseUrl must contain an HTTP listener address.", nameof(listenerUrls));

        var address = Regex.Replace(addresses[0], @"^(https?)://[+*](?=[:/]|$)",
            "$1://127.0.0.1", RegexOptions.IgnoreCase);
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || !string.IsNullOrEmpty(uri.UserInfo))
            throw new ArgumentException("Wcs:BaseUrl must contain a valid HTTP(S) listener address.", nameof(listenerUrls));

        var builder = new UriBuilder(uri);
        if (uri.Host == "0.0.0.0") builder.Host = "127.0.0.1";
        else if (uri.Host == "[::]" || uri.Host == "::") builder.Host = "::1";
        return builder.Uri.GetLeftPart(UriPartial.Authority);
    }
}
