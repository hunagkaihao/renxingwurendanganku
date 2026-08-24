using AutoMapper;

using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
using WarehouseManagement.Archives.Aggregates;
using WarehouseManagement.Archives.Dto;
using WarehouseManagement.Cells;
using WarehouseManagement.Cells.Dto;
using WarehouseManagement.CheckHiss.Aggregates;
using WarehouseManagement.CheckHiss.Dto;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Checks.Dto;
using WarehouseManagement.Goodss.Aggregates;
using WarehouseManagement.Goodss.Dto;
using WarehouseManagement.LogFiles;
using WarehouseManagement.LogFiles.Dto;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.Plans.Dto;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.RfidCodes.Dto;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.TaskHiss.Dto;
using WarehouseManagement.Warehouses.Aggregates;
using WarehouseManagement.Warehouses.Dto;

namespace WarehouseManagement;

public class WarehouseManagementApplicationAutoMapperProfile : Profile
{
    public WarehouseManagementApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<CreateGoodsDto, Goods>(MemberList.None); ;
        CreateMap<Goods, CreateGoodsDto>();
        CreateMap<UpdateGoodsDto, Goods>(MemberList.None); ;
        CreateMap<Goods, UpdateGoodsDto>();
        CreateMap<Goods, GoodsDto>();


        CreateMap<ArchiveBox, ArchiveBoxDto>(MemberList.None); 
        CreateMap<CreateArchiveBoxDto, ArchiveBox>(MemberList.None); ;
        CreateMap<Archive, ArchiveDto>(MemberList.None); ;
        CreateMap<CreateArchiveDto, Archive>(MemberList.None); ;
        CreateMap<ArchiveBoxDetail, ArchiveBoxDetailDto>(MemberList.None); ;

        CreateMap<Rfid, RfidCodeDto>();
        CreateMap<CreateRfidCodeDto, Rfid>(MemberList.None); ;

        CreateMap<CreateCellDto, Cell>(MemberList.None); 
        CreateMap<Cell, CreateCellDto>();
        CreateMap<UpdateCellDto, Cell>(MemberList.None); 
        CreateMap<Cell, UpdateCellDto>();
        CreateMap<Cell, CellDto>();


        CreateMap<CreateStockTaskDto, StockTask>(MemberList.None); ;
        CreateMap<StockTask, CreateStockTaskDto>(MemberList.None);
        CreateMap<UpdateStockTaskDto, StockTask>(MemberList.None); ;
        CreateMap<StockTask, UpdateStockTaskDto>(MemberList.None);
        CreateMap<StockTask, StockTaskDto>(MemberList.None);
        CreateMap<StockTaskDto, StockTask>(MemberList.None);
        CreateMap<StockTaskDetail, PagingStockTaskDetailOutput>();
        CreateMap<StockTaskDetail, StockTaskDetailDto>(MemberList.None);

        CreateMap<TaskHis, TaskHisDto>(MemberList.None);
        CreateMap<TaskHisDetail, TaskHisDetailDto>(MemberList.None);


        CreateMap<UpdatePlanDto, Plan>(MemberList.None); ;
        CreateMap<Plan, UpdatePlanDto>();
        CreateMap<Plan, PlanDto>(MemberList.None);
        CreateMap<PlanDto, Plan>(MemberList.None);
        //CreateMap<UpdateCheckDto, Check>(MemberList.None); ;
        //CreateMap<Check, UpdateCheckDto>();
        CreateMap<Check, CheckDto>(MemberList.None);
        CreateMap<CheckDetail, CheckDetailDto>(MemberList.None);
        CreateMap<CheckDetailDto, CheckDetail>(MemberList.None);
        CreateMap<CheckHisDto, CheckHis>(MemberList.None);
        CreateMap<CheckHis, CheckHisDto>(MemberList.None);
        CreateMap<CheckDetailHisDto, CheckDetailHis>(MemberList.None);
        CreateMap<CheckDetailHis, CheckDetailHisDto>(MemberList.None);

        CreateMap<CreateWarehouseDto, Warehouse>(MemberList.None);
        CreateMap<Warehouse, CreateWarehouseDto>();
        CreateMap<UpdateWarehouseDto, Warehouse>(MemberList.None);
        CreateMap<Warehouse, UpdateWarehouseDto>();
        CreateMap<Warehouse, WarehouseDto>();

        CreateMap<CreateWarehouseAreaDto, WarehouseArea>(MemberList.None);
        CreateMap<WarehouseArea, CreateWarehouseAreaDto>();
        CreateMap<UpdateWarehouseAreaDto, WarehouseArea>(MemberList.None);
        CreateMap<WarehouseArea, UpdateWarehouseAreaDto>();
        CreateMap<WarehouseArea, WarehouseAreaDto>();


        CreateMap<LogFile, LogFileDto>();
    }
}
