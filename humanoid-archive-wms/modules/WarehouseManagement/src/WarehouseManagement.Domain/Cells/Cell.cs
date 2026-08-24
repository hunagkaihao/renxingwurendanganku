using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

namespace WarehouseManagement.Cells
{
    public class Cell : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        private Cell()
        {
            //SetIntProperties();

        }
        public Cell(string cellCode, string cellType, string cellName, int warehouseId = 0)
        {
            SetIntProperties();
            SetProperties(cellCode, cellType,cellName, warehouseId); 

        }
        public Cell(string cellCode, string cellType, string cellName, string cellGroup
     , int cell_z, int cell_x, int cell_y, string cellStorageType, string deviceCode
     , string customCode, string cellModel, int warehouseId = 0)
        {
            SetIntProperties();
            CellCode = cellCode;
            CellType = Enum.Parse<CellType>(cellType);
            CellName = cellName;
            CellGroup = cellGroup;
            Cell_z = cell_z;
            Cell_x = cell_x;
            Cell_y = cell_y;
            CellStorageType = cellStorageType;
            DeviceCode = deviceCode;
            CustomCode = customCode;
            CellModel = cellModel;
        }
        public void Update(string cellCode, string cellType, string cellName,int warehouseId = 0)
        {
            SetProperties(cellCode, cellType, cellName, warehouseId);
        }
        public void Update(string cellName, string cellCode, string cellType)
        {
            CellName = cellName;
            CellCode = cellCode;
            CellType = Enum.Parse<CellType>(cellType);
        }
        public void SetProperties(string cellCode, string cellType, string cellName, int warehouseId = 0)
        {
            CellCode = cellCode;
            CellType = Enum.Parse<CellType>(cellType); 
            WarehouseId= warehouseId;
            if (CellType==CellType.Cell|| CellType == CellType.CTUCell|| CellType == CellType.WallCell)
            {
                var cellXYZ =cellCode.Split('-');
                if (cellXYZ.Length == 3)
                {
                    try
                    {
                        Cell_z = Convert.ToInt32(cellXYZ[0]);
                        Cell_x = Convert.ToInt32(cellXYZ[1]);
                        Cell_y = Convert.ToInt32(cellXYZ[2]);
                    }
                    catch (Exception)
                    {

                        throw new UserFriendlyException("库位录入数据不符合规范，包含非数字");
                    }

                    CellGroup = CellCode;
                    CellName = cellName;
                }
                else
                {
                    throw new UserFriendlyException("库位录入数据不符合规范，格式为：00-00-00");
                }

            }
            else if (CellType==CellType.Station)
            {
                if (CellCode.Length == 5)//站台 以12开头  12001-
                {
                    //CellType = "Station";
                    Cell_z = 0;
                    Cell_x = 1;
                    Cell_y = 1;
                    CellGroup = "1";
                    CellName = cellName;
                    DeviceCode = CellCode;
                }
                else
                {
                    throw new UserFriendlyException("站台录入数据不符合规范，格式为：");
                }
            }
            else
            {
                throw new UserFriendlyException("库位类型错误");
            }

        }
        /// <summary>
        /// 设置初始属性
        /// </summary>
        public void SetIntProperties()
        {
            //WarehouseId = 1;
            //CellType = "Cell";
            CellInout = "InOut";
            CellModel = "Inch3";
            //CellStatus = "Nohave";
            //RunStatus = "Enable";
            CellForkType = "Normal";
            ShelfType = "Single";
            CellStorageType = "SinglePallet";
            LaneWay = "1";
            CellGroup = CellCode;
            DeviceCode = "18001";
            SetCellStatus("Nohave");
            SetRunStatus("Enable");
        }

        /// <summary>
        /// 设置运行状态
        /// </summary>
        /// <param name="runStatus"></param>
        public void SetRunStatus(string runStatus)
        {
            RunStatus = Enum.Parse<CellRunStatus>(runStatus);
            Log.Debug($"Cell:{CellCode} RunStatus is set {runStatus} Method:{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        public void SetCellStatus(string cellStatus)
        {
            CellStatus = Enum.Parse<CellStatus>(cellStatus);
            Log.Debug($"Cell:{CellCode} CellStatus is set {cellStatus} Method:{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            //Log.Warning($"库位:{CellCode}的库位状态设置为{cellStatus}。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        public int? WarehouseId { get; set; }
        public int? AreaId { get; set; }
        public int? LogicId { get; set; }
        public string CellName { get; set; }
        public string CellCode { get; set; }
        public CellType CellType { get; set; }
        public string DeviceCode { get; set; }
        public int Cell_z { get; set; }
        public int Cell_x { get; set; }
        public int Cell_y { get; set; }
        /// <summary>
        /// CTU库一般为InOut  入库分拨墙设置为In  出库分拨墙设置为Out
        /// </summary>
        public string CellInout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CellModel { get; set; }
        public CellStatus CellStatus { get; private set; }
        public CellRunStatus RunStatus { get; private set; }
        public string CellForkType { get; set; }
        public string CellLogicalName { get; set; }
        public string LaneWay { get; set; }
        public string CellGroup { get; set; }
        public string CellFlag { get; set; }
        public string ShelfType { get; set; }
        public string ShelfNeighbour { get; set; }
        /// <summary>
        /// 对应料箱容器类型 CtnrCode
        /// </summary>
        public string CellStorageType { get; set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
        public string LockCellId { get; set; }
        public string BelongArea { get; set; }
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 客户自定义编码
        /// </summary>
        public string CustomCode { get; set; }
        /// <summary>
        /// 分拨墙控制器IP
        /// </summary>
        public string ControllerIP { get; set; }
        /// <summary>
        /// 分拨墙控制器端口
        /// </summary>
        public string ChannelPort { get; set; }
        /// <summary>
        /// 标签灯位置ID
        /// </summary>
        public int LightPosition { get; set; }
        /// <summary>
        /// 是否需要出库确认当位CellInout出库位，OutConfirm为1  出库需要确认
        /// </summary>
        public int OutConfirm { get; set; }
    }
}
