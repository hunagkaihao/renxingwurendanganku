using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcServer.Cache
{
    public interface ICache
    {
        bool InitCache();
        bool AddTag(string plcName, string tagName, string initValue = "");
        Task<bool> AddTagAsync(string plcName, string tagName, string initValue = "");
        bool SetTagLifeCycle(string plcName, string tagName, int lifeCycle = -1);
        long RemoveTag(string plcName, string[] tagNames);
        Task<long> RemoveTagAsync(string plcName, string[] tagNames);
        string? ReadTag(string plcName, string tagName);
        Task<string?> ReadTagAsync(string plcName, string tagName);
        bool WriteTag(string plcName, string tagName, string tagValue);
        Task<bool> WriteTagAsync(string plcName, string tagName, string tagValue);
        bool WriteAndPublishTag(string plcName, string tagName, string tagValue);
        Task<bool> WriteAndPublishTagAsync(string plcName, string tagName, string tagValue);
        
        //客户端相关
        bool AddRegisterChannel();
        void SubscribeRegisterChannel(Action<string, string> handle);
        bool AddClientChannel(string clientChannel);
        void SubscribeClientChannel(string clientChannel, Action<string, string> handle);
        bool RegisterClient(string clientChannel);
        bool SendClientMessage(string clientChannel, string message);
    }
}
