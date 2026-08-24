using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.StockTasks
{
    public enum ManageStatus
    {   
        /// <summary>
        /// 等待执行
        /// </summary>
        WaitingExecute,
        /// <summary>
        /// 监控任务获取
        /// </summary>
        OrderCatched,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing,
        /// <summary>
        /// 调度删除任务
        /// </summary>
        Cancel,
        /// <summary>
        /// 设备错误
        /// </summary>
        Error,
        /// <summary>
        /// 输送完成
        /// </summary>
        Complete,
        /// <summary>
        /// 异常完成
        /// </summary>
        ExceptionComplete,
        /// <summary>
        /// 等待确认
        /// </summary>
        WaitingConfirm,
        /// <summary>
        /// 任务完成
        /// </summary>
        LogicFinish,
        /// <summary>
        /// 逻辑完成异常
        /// </summary>
        LogicError,
        /// <summary>
        /// 柜门打开
        /// </summary>
        StationOpen,
        /// <summary>
        /// 柜门关闭
        /// </summary>
        StationClose,
        /// <summary>
        /// 密集柜OPEN任务下达等待
        /// </summary>
        CabinetWait,
        /// <summary>
        /// 密集柜Open任务完成
        /// </summary>
        CabinetComplete,
        /// <summary>
        /// 机械手取放货命令已下达
        /// </summary>
        RobotWait,
        /// <summary>
        /// 机械手取放货命令已完成
        /// </summary>
        RobotComplete,
        /// <summary>
        /// 机械手抓
        /// </summary>
        RobotPick,
        /// <summary>
        /// 机械手放
        /// </summary>
        RobotPlace,
        /// <summary>
        /// 强制完成请求
        /// </summary>
        CompeleteRequest,
        /// <summary>
        /// 强制取消申请
        /// </summary>
        TaskDeleteRequest,
    }
}
