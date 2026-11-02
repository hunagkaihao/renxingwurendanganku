namespace WarehouseManagement;

public static class WarehouseManagementDbProperties
{
    /// <summary>
    /// 未单独规定业务长度的普通字符串上限，单位为字符；图片等长内容不适用。
    /// </summary>
    public const int DefaultStringMaxLength = 200;

    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "WarehouseManagement";
}
