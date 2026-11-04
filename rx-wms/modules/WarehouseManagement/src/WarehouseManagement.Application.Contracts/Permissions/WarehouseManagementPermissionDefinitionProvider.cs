using WarehouseManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace WarehouseManagement.Permissions;

public class WarehouseManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WarehouseManagementPermissions.GroupName, L("Permission:WarehouseManagement"));
        var abpIdentityGroup = context.GetGroup("AbpIdentity");

        var goodsManagement = abpIdentityGroup.AddPermission(WarehouseManagementPermissions.GoodsManagement.Default,
            L("Permission:GoodsManagement"));
        goodsManagement.AddChild(WarehouseManagementPermissions.GoodsManagement.Create, L("Permission:Create"));
        goodsManagement.AddChild(WarehouseManagementPermissions.GoodsManagement.Update, L("Permission:Update"));
        goodsManagement.AddChild(WarehouseManagementPermissions.GoodsManagement.Delete, L("Permission:Delete"));

        var cellManagement = abpIdentityGroup.AddPermission(WarehouseManagementPermissions.CellManagement.Default,
    L("Permission:CellManagement"));
        cellManagement.AddChild(WarehouseManagementPermissions.CellManagement.Create, L("Permission:Create"));
        cellManagement.AddChild(WarehouseManagementPermissions.CellManagement.Update, L("Permission:Update"));
        cellManagement.AddChild(WarehouseManagementPermissions.CellManagement.Delete, L("Permission:Delete"));

        var storageBoxManagement = abpIdentityGroup.AddPermission(WarehouseManagementPermissions.StorageBoxManagement.Default,
     L("Permission:StorageBoxManagement"));
        storageBoxManagement.AddChild(WarehouseManagementPermissions.StorageBoxManagement.Create, L("Permission:Create"));
        storageBoxManagement.AddChild(WarehouseManagementPermissions.StorageBoxManagement.Update, L("Permission:Update"));
        storageBoxManagement.AddChild(WarehouseManagementPermissions.StorageBoxManagement.Delete, L("Permission:Delete"));


        var stockTaskManagement = abpIdentityGroup.AddPermission(WarehouseManagementPermissions.StockTaskManagement.Default,
    L("Permission:StockTaskManagement"));
        stockTaskManagement.AddChild(WarehouseManagementPermissions.StockTaskManagement.Create, L("Permission:Create"));
        stockTaskManagement.AddChild(WarehouseManagementPermissions.StockTaskManagement.Update, L("Permission:Update"));
        stockTaskManagement.AddChild(WarehouseManagementPermissions.StockTaskManagement.Delete, L("Permission:Delete"));

        var warehouseManagement = abpIdentityGroup.AddPermission(WarehouseManagementPermissions.WarehouseManagement.Default,
     L("Permission:WarehouseManagement"));
        warehouseManagement.AddChild(WarehouseManagementPermissions.WarehouseManagement.Create, L("Permission:Create"));
        warehouseManagement.AddChild(WarehouseManagementPermissions.WarehouseManagement.Update, L("Permission:Update"));
        warehouseManagement.AddChild(WarehouseManagementPermissions.WarehouseManagement.Delete, L("Permission:Delete"));
    }

    


private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WarehouseManagementResource>(name);
    }
}
