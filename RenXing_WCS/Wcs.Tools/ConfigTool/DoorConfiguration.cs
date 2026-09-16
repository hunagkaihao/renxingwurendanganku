using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Wcs.ConfigTool;

/// <summary>柜门清单是唯一启用来源；沿用 12 + 三位门号及原 PLC 命名规则。</summary>
public sealed class DoorConfiguration : ISingletonDependency
{
    private readonly HashSet<string> _codes;
    public IReadOnlyList<string> Codes { get; }

    public DoorConfiguration(IOptions<ConfigOptions> options)
    {
        var codes = options.Value.DoorCodes ?? throw new ArgumentException("Wcs:DoorCodes 必须是柜门编号列表。");
        _codes = new HashSet<string>(StringComparer.Ordinal);
        foreach (string code in codes)
        {
            if (!IsDoorCode(code))
                throw new ArgumentException($"柜门编号 {code} 无效，应为 12001～12999，例如 12009、12010。");
            if (!_codes.Add(code))
                throw new ArgumentException($"Wcs:DoorCodes 中柜门编号 {code} 重复。");
        }
        Codes = codes.OrderBy(c => c, StringComparer.Ordinal).ToArray();
    }

    public static bool IsDoorCode(string code) => code != null &&
        Regex.IsMatch(code, @"\A12[0-9]{3}\z") && code != "12000";
    public bool Contains(string code) => code != null && _codes.Contains(code);
    public void ValidateEndpoint(string code)
    {
        if (IsDoorCode(code) && !Contains(code))
            throw new ArgumentException($"柜门 {code} 未在 Wcs:DoorCodes 中启用。");
    }
    public string RequireDoor(string code)
    {
        if (!Contains(code)) throw new ArgumentException($"柜门 {code} 未在 Wcs:DoorCodes 中启用。");
        return code;
    }
    public static int Number(string code) => IsDoorCode(code)
        ? int.Parse(code.Substring(2)) : throw new ArgumentException($"无效柜门编号 {code}");
    public static string CommandTag(string code) => $"Door{Number(code)}_Cmd";
    public static string ResponseTag(string code) => $"Door{Number(code)}_Response";
    // 保持原 1～8 号门规格；新增门沿用原 4～8 号门的默认 _1cm，已存在节点不覆盖。
    public static string DefaultSpecs(string code) => Number(code) switch
    { 1 => "_5cm", 2 => "_3cm", 3 => "_2cm", _ => "_1cm" };
}
