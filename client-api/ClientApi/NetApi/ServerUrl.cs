namespace ClientApi.NetApi;

/// <summary>为现有静态指静脉调用链提供当前档案库地址，不缓存地址或定义部署默认值。</summary>
public static class ServerUrl
{
    private static IConfiguration? _configuration;

    /// <summary>读取当前配置的档案库 HTTP(S) 地址；缺失或无效时明确失败。</summary>
    public static string AMSApiUrl => ReadApiUrl(_configuration
        ?? throw new InvalidOperationException("档案库配置尚未初始化。"));

    /// <summary>启动时关联应用配置并验证地址，后续读取自动使用重载后的值。</summary>
    /// <param name="configuration">包含 AMSApiUrl 的应用配置。</param>
    public static void Initialize(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ReadApiUrl(configuration);
        _configuration = configuration;
    }

    /// <summary>验证绝对 HTTP(S) 地址，保留配置原值，不替换为旧地址或静默修正。</summary>
    private static string ReadApiUrl(IConfiguration configuration)
    {
        var value = configuration["AMSApiUrl"];
        if (string.IsNullOrWhiteSpace(value) || !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new InvalidOperationException("请在配置文件中设置有效的 AMSApiUrl（绝对 HTTP 或 HTTPS 地址）。");
        return value;
    }
}
