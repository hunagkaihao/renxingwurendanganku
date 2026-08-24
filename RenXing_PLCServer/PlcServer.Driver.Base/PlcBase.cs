using PlcServer.Cache;
using PlcServer.Defines;

namespace PlcServer.Driver.Base
{
    public abstract class PlcBase
    {
        protected Dictionary<string, PlcTag> mTags;
        protected List<PlcTagGroup> mReadGroups;
        protected bool mIsConnected;
        protected string mPlcName;
        protected ICache mCache;
        protected string mConnParas;

        public PlcBase(ICache cache)
        {
            mTags = new Dictionary<string, PlcTag>();
            mReadGroups = new List<PlcTagGroup>();
            mIsConnected = false;
            mPlcName = "";
            mCache = cache;
            mConnParas = "";
        }

        public bool IsConnected => mIsConnected;

        public string PlcName
        {
            get { return mPlcName; }
            set { mPlcName = value; }
        }

        public string ConnParas
        {
            get { return mConnParas; }
            set { mConnParas = value; }
        }

        public int TagQuantity()
        {
            return mTags.Count;
        }

        public int GroupQuantity()
        {
            return mReadGroups.Count;
        }

        public abstract void LoadTags();
        public abstract void GroupTags();
        public abstract void InitCache();
        public abstract Task<bool> ConnectAsync();
        public abstract Task DisConnectAsync();
        public abstract Task ReadAllAsync();
        public abstract Task<PlcTagValue?> ReadTagAsync(string tagName);
        public abstract Task<bool> WriteTagAsync(string tagName, string tagValue);
    }
}