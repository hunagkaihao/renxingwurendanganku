using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks
{
    public enum WcsOrderStatus
    {
        //	等待执行
        //	龙门入库取货
        //	龙门入库放货
        //	龙门出库取货
        //	龙门出库放货
        //	龙门读库存信息
        //	龙门回原点
        //	龙门在原点判断
        //	密集架入库打开
        //	密集架出库打开
        //	密集架闭合
        //	密集架是否闭合判断
        //	密集架是否在目标位判断
        //	取档口打开
        //	最后一个盘点订单判断
        //	已完成
        //	已强制完成
        //	已取消

        /// <summary>
        /// 等待执行
        /// </summary>
        WaitingExecuting = 1,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing = 2,
        /// <summary>
        /// 龙门入库抓取中
        /// </summary>
        RobotIncell = 3,
        /// <summary>
        /// 密集架打开
        /// </summary>
        CabinetOpen = 4,
        /// <summary>
        /// 龙门出库抓取中
        /// </summary>
        RobotOutcell = 5,
        /// <summary>
        /// 密集架关闭
        /// </summary>
        CabinetClose = 6,
        /// <summary>
        /// 任务完成
        /// </summary>
        Complete = 9,
        /// <summary>
        /// 调度删除任务
        /// </summary>
        Cancel =10,
        /// <summary>
        /// 异常完成
        /// </summary>
        ExceptionComplete =11,
    }
}
