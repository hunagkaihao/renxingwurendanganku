using System;
using System.Collections.Generic;
using Wcs.Jobs.Models;
using Wcs.Orders.Models;

namespace Wcs.Backups;

public class OrderInRedis
{
    public string orderCode { get; set; }
    public string orderType { get; set; }
    public string orderState { get; set; }
    public string plateCode { get; set; }
    public string plateSpecs { get; set; }
    public string startNode { get; set; }
    public string endNode { get; set; }
    public int cachePos { get; set; }
    public int priority { get; set; }
    public bool openDoorImme { get; set; }
    public string createTime { get; set; }
    public string execStep { get; set; }
    public string execInfo { get; set; }
    public bool hasError { get; set; }
    public string execUpdateTime { get; set; }


    public int pathId { get; set; }
    public int taskId { get; set; }
    public string taskState { get; set; }
    public List<JobInfo> jobs { get; set; } = new List<JobInfo>();

    public OrderInRedis()
    {
        orderCode = string.Empty;
        orderType = string.Empty;
        orderState = string.Empty;
        plateCode = string.Empty;
        plateSpecs = string.Empty;
        startNode = string.Empty;
        endNode = string.Empty;
        cachePos = -1;
        priority = 0;
        openDoorImme = false;
        createTime = string.Empty;

        execStep = string.Empty;
        execInfo = string.Empty;
        hasError = false;
        execUpdateTime = string.Empty;

        pathId = 0;
        taskId = 0;
        taskState = string.Empty;
        jobs = new List<JobInfo>();
    }

    public OrderInRedis(DispatchOrder order)
    {
        orderCode = order.OrderCode;
        orderType = order.OrderType.ToString();
        orderState = order.State.ToString();
        plateCode = order.PlateCode;
        plateSpecs = order.PlateSpecs;
        startNode = order.StartNode;
        cachePos = -1;
        endNode = order.EndNode;
        priority = order.Priority;
        openDoorImme = order.CanOpenDoorImmediate;
        createTime = order.CreateTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        execStep = order.ExecStep;
        execInfo = order.ExecInfo;
        hasError = order.HasError;
        execUpdateTime = order.ExecUpdateTime;

        pathId = 0;
        taskId = 0;
        taskState = string.Empty;
        jobs = new List<JobInfo>();
    }
}

public class JobInfo
{
    public int id { get; set; }

    public int pathStep { get; set; }

    public int nextTrueStep { get; set; }

    public int nextFalseStep { get; set; }

    public string nodeName { get; set; } = string.Empty;

    public string cmdName { get; set; } = string.Empty;

    public string state { get; set; } = string.Empty;  //update

    public int priority { get; set; }

    public string execInfo { get; set; } = string.Empty;  //update

    public string createTime { get; set; } = string.Empty;

    public JobInfo(DispatchJob job)
    {
        id = job.Id;
        pathStep = job.ProcessSequence;
        nextTrueStep = job.NextTrueStep;
        nextFalseStep = job.NextFalseStep;
        state = job.State.ToString();
        priority = job.Priority;
        execInfo = string.Empty;
        createTime = job.CreateTime;
        cmdName = string.Empty;
        nodeName = job.NodeCode;
    }

    public JobInfo()
    {

    }
}