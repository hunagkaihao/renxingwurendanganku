using System;
using System.Collections.Generic;

namespace Ecs.Dispatch;

public class OrderInfoDto
{
    public string orderCode { get; set; }
    public string orderType { get; set; }
    public string orderState { get; set; }
    public string plateCode { get; set; }
    public string startNode { get; set; }
    public string endNode { get; set; }
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
    public List<JobInfoDto> jobs { get; set; } = new List<JobInfoDto>();

    public OrderInfoDto()
    {
        orderCode = string.Empty;
        orderType = string.Empty;
        orderState = string.Empty;
        plateCode = string.Empty;
        startNode = string.Empty;
        endNode = string.Empty;
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
        jobs = new List<JobInfoDto>();
    }

    // public OrderInfoDto(DispatchOrder order)
    // {
    //     orderCode = order.OrderCode;
    //     orderType = order.OrderType.ToString();
    //     orderState = order.State.ToString();
    //     plateCode = order.PlateCode;
    //     startNode = order.StartNode;
    //     endNode = order.EndNode;
    //     openDoorImme = order.OpenDoorImme;
    //     priority = order.Priority;
    //     createTime = order.CreateTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
    //     execStep = order.ExecStep;
    //     execInfo = order.ExecInfo;
    //     hasError = order.HasError;
    //     execUpdateTime = order.ExecUpdateTime;

    //     pathId = 0;
    //     taskId = 0;
    //     taskState = string.Empty;
    //     this.jobs = new List<JobDto>();
    // }
}

public class JobInfoDto
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

    // public JobInfoDto(DispatchJob job)
    // {
    //     this.id = job.Id;
    //     this.pathStep = job.PathStep;
    //     this.nextTrueStep = job.NextTrueStep;
    //     this.nextFalseStep = job.NextFalseStep;
    //     this.state = job.State.ToString();
    //     this.priority = job.Priority;
    //     this.execInfo = string.Empty;
    //     this.createTime = job.CreateTime;
    //     this.cmdName = string.Empty;
    //     this.nodeName = job.NodeCode;
    // }

    public JobInfoDto()
    {
        
    }
}