using Microsoft.EntityFrameworkCore;
using PlcServer.Devices.DeviceServices.DeviceServiceByMySql.Repositories;
using PlcServer.Devices.IDeviceServices;
using PlcServer.Devices.Models;
using Shared.Logger.ILogger;

namespace PlcServer.Devices.DeviceServices.DeviceServiceByMySql
{
    public class DeviceServiceInMySql : IDeviceService
    {
        private PlcDbContext plcDbContext = new PlcDbContext();

        private readonly object mlocker = new object();

        private readonly ILog _logger;

        public DeviceServiceInMySql(ILog logger)
        {
            _logger = logger;
        }

        public List<PlcDevice> GetAllPlcDevices()
        {
            lock (mlocker)
            {
                try
                {
                    IQueryable<PlcDevice>? lstPlcDevice
                        = plcDbContext.PlcDevices?
                        .AsNoTracking()
                        .OrderBy(d => d.PlcId);

                    if (lstPlcDevice == null)
                    {
                        return new List<PlcDevice>();
                    }
                    else
                    {
                        return lstPlcDevice.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, GetType().FullName);
                    return new List<PlcDevice>();
                }
            }
        }

        public List<PlcNode> GetAllPlcNodes()
        {
            lock (mlocker)
            {
                try
                {
                    IQueryable<PlcNode>? lstPlcNode
                        = plcDbContext.PlcNodes?
                        .AsNoTracking()
                        .OrderBy(n => n.NodeId);

                    if (lstPlcNode == null)
                    {
                        return new List<PlcNode>();
                    }
                    else
                    {
                        return lstPlcNode.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, GetType().FullName);
                    return new List<PlcNode>();
                }
            }
        }

        public List<PlcNode> GetAllPlcNodesInPlc(string plcName)
        {
            lock (mlocker)
            {
                try
                {
                    IQueryable<PlcNode>? lstPlcNode
                        = plcDbContext.PlcNodes?
                        .AsNoTracking()
                        .Where(n => n.PlcName == plcName)
                        .OrderBy(n => n.NodeId);

                    if (lstPlcNode == null)
                    {
                        return new List<PlcNode>();
                    }
                    else
                    {
                        return lstPlcNode.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, GetType().FullName);
                    return new List<PlcNode>();
                }
            }
        }
    }
}