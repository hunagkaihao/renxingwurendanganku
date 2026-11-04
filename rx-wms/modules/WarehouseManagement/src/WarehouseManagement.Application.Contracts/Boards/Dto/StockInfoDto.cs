using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Boards.Dto
{
    public class StockInfoDto
    {
        public StockInfoDto()
        { }
        //档案盒总数
        public int BoxTotalCt { get; set; }
        //在库档案盒总数
        public int BoxInTotalCt { get; set; }
        //档案文件总数
        public int ArchiveTotalCt { get; set; }
        //在库档案文件总数
        public int ArchiveInTotalCt { get; set; }
        //借出档案数
        public int BorrowedTotalCt { get; set; }
    }
}
