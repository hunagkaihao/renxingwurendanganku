using Volo.Abp.Reflection;

namespace WarehouseManagement.Permissions;

public class WarehouseManagementPermissions
{
    public const string GroupName = "WarehouseManagement";
    public static class GoodsManagement
    {
        public const string Default = GroupName + ".GoodsManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class CellManagement
    {
        public const string Default = GroupName + ".CellManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class StorageBoxManagement
    {
        public const string Default = GroupName + ".StorageBoxManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class StockTaskManagement
    {
        public const string Default = GroupName + ".StockTaskManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class TestSiteManagement
    {
        public const string Default = GroupName + ".TestSiteManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class PlanManagement
    {
        public const string Default = GroupName + ".PlanManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class WarehouseManagement
    {
        public const string Default = GroupName + ".WarehouseManagement";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(WarehouseManagementPermissions));
    }
}
