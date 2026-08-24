namespace PlcServer.Defines
{
    public class PlcTagGroup
    {
        private string mGroupName;
        private Dictionary<string, PlcTag> mTags;
        private int mMinAddr;
        private int mMaxAddr;
        private int mMemoryArea; //组内所有tag所属存储区，如西门子PLC的DB区，M区，I区，O区等
        private int mMemAreaNo = 0; //存储区的编号，如西门子PLC的DB1，DB2，有些存储区没有编号，默认0

        public string GroupName => mGroupName;

        public Dictionary<string, PlcTag> Tags => mTags;

        public int MinAddr => mMinAddr;

        public int MaxAddr => mMaxAddr;

        public int MemoryArea => mMemoryArea;

        public int MemAreaNo => mMemAreaNo;

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="startAddr">变量的地址，如DB10.DBD100中的100，M5.1中的5</param>
        /// <param name="addrLen">变量地址包含字节或字的长度</param>
        /// <returns></returns>
        public bool AppendTag(PlcTag tag, int startAddr, int addrLen)
        {
            if(startAddr < 0 || addrLen < 1)
            {
                return false;
            }

            foreach (var item in mTags)
            {
                if (item.Key == tag.TagName) //已存在
                {
                    return false;
                }
            }

            mTags.Add(tag.TagName, tag);

            if (startAddr < mMinAddr)
            {
                mMinAddr = startAddr;
            }
            if (startAddr + addrLen - 1 > mMaxAddr)
            {
                mMaxAddr = startAddr + addrLen - 1;
            }
            return true;
        }

        public PlcTagGroup(string groupName, int memoryArea, int memAreaNo = 0)
        {
            mGroupName = groupName;
            mTags = new Dictionary<string, PlcTag>();
            mMinAddr = int.MaxValue;
            mMaxAddr = int.MinValue;
            mMemoryArea = memoryArea;
            mMemAreaNo = memAreaNo;
        }
    }
}
