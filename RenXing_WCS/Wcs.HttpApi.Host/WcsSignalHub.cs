using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wcs;

public class WcsSignalHub : Hub
{
    public async Task NotifyClients(string clientFunc, object data)
    {
        try
        {
            await Clients.All.SendAsync(clientFunc, data).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            Serilog.Log.Error(ex, "向客户端推送消息失败，客户端方法：{ClientMethod}", clientFunc);
        }
    }
}
