using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.Archives;
using WarehouseManagement.Boards.Dto;
using WarehouseManagement.Cells;
using WarehouseManagement.StockTasks;
using WarehouseManagement.TaskHiss;

namespace WarehouseManagement.Boards
{
    public class BoardAppService : WarehouseManagementAppService, IBoardAppService
    {
        public readonly TaskHisManager _taskHisManager;
        public readonly ArchiveBoxManager _archiveBoxManager;
        public readonly ArchiveManager _archiveManager;
        public readonly ArchiveBoxDetailManager _archiveBoxDetailManager;
        public readonly CellManager _cellManager;


        public BoardAppService(
            TaskHisManager taskHisManager,
            ArchiveBoxManager archiveBoxManager,
            ArchiveManager archiveManager,
            ArchiveBoxDetailManager archiveBoxDetailManager,
            CellManager cellManager
            )
        {
            _taskHisManager = taskHisManager;
            _archiveBoxManager = archiveBoxManager;
            _archiveBoxDetailManager = archiveBoxDetailManager;
            _cellManager = cellManager;
            _archiveManager = archiveManager;
        }

        public async Task<SevenDayTasksDto> GetSevenDayTasks()
        {
            SevenDayTasksDto sevenDayTasksDto = new();
            var entity = await _taskHisManager.GetSevenDayHisAsync();
            List<string> slist = new List<string>();
            List<int> ilist = new List<int>();
            List<int> inlist = new List<int>();
            List<int> outlist = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                slist.Add(DateTime.Now.AddDays(-i).Day.ToString() + "日");
                ilist.Add(entity.Count(x => x.CreationTime.Date == DateTime.Now.AddDays(-i).Date));
                inlist.Add(entity.Count(x => x.CreationTime.Date == DateTime.Now.AddDays(-i).Date & x.ManageTypeCode == ManageType.NPFullStockIn));
                outlist.Add(entity.Count(x => x.CreationTime.Date == DateTime.Now.AddDays(-i).Date & x.ManageTypeCode == ManageType.NPSortStockOut));
            }
            sevenDayTasksDto.TotalCount = entity.Count;
            sevenDayTasksDto.Keys = slist;
            sevenDayTasksDto.Value = ilist;
            sevenDayTasksDto.Invalue = inlist;
            sevenDayTasksDto.Outvalue = outlist;

            return sevenDayTasksDto;
        }
        public async Task<StockInfoDto> GetStockInfo()
        {
            try
            {
                StockInfoDto stockInfoDto = new();
                var archiveBoxs = await _archiveBoxManager.GetAll();
                var archiveBoxDetails = await _archiveBoxDetailManager.GetAll();
                var archives = await _archiveManager.GetAll();
                var iLists =await _cellManager.GetByCellTypeAsync(CellType.Station);
                List<int> cellIds = new();
                foreach (var list in iLists)
                {
                    cellIds.Add(list.Id);
                }

                stockInfoDto.BoxTotalCt = archiveBoxs.Count;
                stockInfoDto.BoxInTotalCt = archiveBoxs.Count(x => x.CellId != 0 || cellIds.Contains(x.CellId));
                stockInfoDto.ArchiveInTotalCt = archiveBoxDetails.Count;
                stockInfoDto.ArchiveTotalCt = archives.Count;
                return stockInfoDto;
            }
            catch(Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }


    }
}
