using PlcServer.Defines.Enum;

namespace PlcServer.Driver.Simulation
{
    internal class PlcAddress
    {
        private EnumMemoryType mMemoryType;
        private int mDbNumber;
        private int mStartByte;
        private int mBitNumber;
        private EnumPlcTagType mTagType;
        private int mByteLength; //包含字节数量

        public EnumMemoryType MemoryType
        {
            get => mMemoryType;
            set => mMemoryType = value;
        }

        public int DbNumber
        {
            get => mDbNumber;
            set => mDbNumber = value;
        }

        public int StartByte
        {
            get => mStartByte;
            set => mStartByte = value;
        }

        public int BitNumber
        {
            get => mBitNumber;
            set => mBitNumber = value;
        }

        public EnumPlcTagType TagType
        {
            get => mTagType;
            set => mTagType = value;
        }

        public int ByteLength
        {
            get => mByteLength;
            set => mByteLength = value;
        }

        public PlcAddress(string address)
        {
            Parse(address, 
                out mMemoryType, 
                out mDbNumber, 
                out mTagType, 
                out mStartByte, 
                out mBitNumber,
                out mByteLength);
        }

        public static void Parse(string input, 
            out EnumMemoryType memoryType, 
            out int dbNumber, 
            out EnumPlcTagType tagType, 
            out int startByte, 
            out int bitNumber, 
            out int byteLen)
        {
            bitNumber = -1;
            dbNumber = 0;

            switch (input.Substring(0, 2))
            {
                case "DB":
                    {
                        string[] strings = input.Split(new char[] { '.' });
                        if (strings.Length < 2)
                            throw new Exception("To few periods for DB address");

                        memoryType = EnumMemoryType.DataBlock;
                        dbNumber = int.Parse(strings[0].Substring(2));
                        startByte = int.Parse(strings[1].Substring(3));

                        string dbType = strings[1].Substring(0, 3);
                        switch (dbType)
                        {
                            case "DBB":
                                tagType = EnumPlcTagType.U8;
                                byteLen = 1;
                                return;
                            case "DBW":
                                tagType = EnumPlcTagType.U16;
                                byteLen = 2;
                                return;
                            case "DBD":
                                tagType = EnumPlcTagType.U32;//也可能时I32或F32，因无法识别，默认为U32
                                byteLen = 4;
                                return;
                            case "DBA":
                                if (strings.Length < 3)
                                    throw new Exception("Array length is not exist");
                                if (!int.TryParse(strings[2], out byteLen))
                                    throw new Exception("Array length is not a number");
                                if (byteLen < 1)
                                    throw new Exception("Array length must be larger than 0");
                                tagType = EnumPlcTagType.U8Array;
                                return;
                            case "DBX":
                                if (strings.Length < 3)
                                    throw new Exception("Bit is not exist");
                                if (!int.TryParse(strings[2], out bitNumber))
                                    throw new Exception("Bit is not a number");
                                if (bitNumber > 7 || bitNumber < 0)
                                    throw new Exception("Bit can only be 0-7");
                                tagType = EnumPlcTagType.Bit;
                                byteLen = 1;
                                return;
                            default:
                                throw new Exception("Invalid Address Exception");
                        }
                    }
                case "IB":
                case "EB":
                    {
                        // Input byte
                        memoryType = EnumMemoryType.Input;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U8;
                        byteLen = 1;
                        return;
                    }
                case "IW":
                case "EW":
                    {
                        // Input word
                        memoryType = EnumMemoryType.Input;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U16;
                        byteLen = 2;
                        return;
                    }
                case "ID":
                case "ED":
                    {
                        // Input double-word
                        memoryType = EnumMemoryType.Input;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U32; //也可能时I32或F32，因无法识别，默认为U32
                        byteLen = 4;
                        return;
                    }
                case "IA":
                case "EA":
                    {
                        memoryType = EnumMemoryType.Input;
                        dbNumber = 0;
                        string[] strings = input.Split(new char[] { '.' });
                        if (strings.Length < 2)
                        {
                            throw new Exception($"To few periods for address({input})");
                        }
                        if (!int.TryParse(strings[0].Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        if (!int.TryParse(strings[1], out byteLen))
                            throw new Exception("Array length is not a number");
                        if (byteLen < 1)
                            throw new Exception("Array length must be larger than 0");
                        tagType = EnumPlcTagType.U8Array;
                        return;
                    }
                case "QB":
                case "AB":
                case "OB":
                    {
                        // Output byte
                        memoryType = EnumMemoryType.Output;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U8;
                        byteLen = 1;
                        return;
                    }
                case "QW":
                case "AW":
                case "OW":
                    {
                        // Output word
                        memoryType = EnumMemoryType.Output;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U16;
                        byteLen = 2;
                        return;
                    }
                case "QD":
                case "AD":
                case "OD":
                    {
                        // Output double-word
                        memoryType = EnumMemoryType.Output;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U32; //也可能时I32或F32，因无法识别，默认为U32
                        byteLen = 4;
                        return;
                    }
                case "QA":
                case "AA":
                case "OA":
                    {
                        memoryType = EnumMemoryType.Output;
                        dbNumber = 0;
                        string[] strings = input.Split(new char[] { '.' });
                        if (strings.Length < 2)
                        {
                            throw new Exception($"To few periods for address({input})");
                        }
                        if (!int.TryParse(strings[0].Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        if (!int.TryParse(strings[1], out byteLen))
                            throw new Exception("Array length is not a number");
                        if (byteLen < 1)
                            throw new Exception("Array length must be larger than 0");
                        tagType = EnumPlcTagType.U8Array;
                        return;
                    }
                case "MB":
                    {
                        // Memory byte
                        memoryType = EnumMemoryType.Memory;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U8;
                        byteLen = 1;
                        return;
                    }
                case "MW":
                    {
                        // Memory word
                        memoryType = EnumMemoryType.Memory;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U16;
                        byteLen = 2;
                        return;
                    }
                case "MD":
                    {
                        // Memory double-word
                        memoryType = EnumMemoryType.Memory;
                        dbNumber = 0;
                        if (!int.TryParse(input.Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        tagType = EnumPlcTagType.U32; //也可能时I32或F32，因无法识别，默认为U32
                        byteLen = 4;
                        return;
                    }
                case "MA":
                    {
                        memoryType = EnumMemoryType.Memory;
                        dbNumber = 0;
                        string[] strings = input.Split(new char[] { '.' });
                        if (strings.Length < 2)
                        {
                            throw new Exception($"To few periods for address({input})");
                        }
                        if (!int.TryParse(strings[0].Substring(2), out startByte))
                        {
                            throw new Exception($"地址{input},格式错误");
                        }
                        if (!int.TryParse(strings[1], out byteLen))
                            throw new Exception("Array length is not a number");
                        if (byteLen < 1)
                            throw new Exception("Array length must be larger than 0");
                        tagType = EnumPlcTagType.U8Array;
                        return;
                    }
                default:
                    switch (input.Substring(0, 1))
                    {
                        case "E":
                        case "I":
                            // Input
                            memoryType = EnumMemoryType.Input;
                            tagType = EnumPlcTagType.Bit;
                            break;
                        case "Q":
                        case "A":
                        case "O":
                            // Output
                            memoryType = EnumMemoryType.Output;
                            tagType = EnumPlcTagType.Bit;
                            break;
                        case "M":
                            // Memory
                            memoryType = EnumMemoryType.Memory;
                            tagType = EnumPlcTagType.Bit;
                            break;
                        default:
                            throw new Exception(string.Format("{0} is not a valid address", input.Substring(0, 1)));
                    }

                    string txt2 = input.Substring(1);
                    if (txt2.IndexOf(".") == -1)
                        throw new Exception("To few periods for address");

                    startByte = int.Parse(txt2.Substring(0, txt2.IndexOf(".")));
                    bitNumber = int.Parse(txt2.Substring(txt2.IndexOf(".") + 1));
                    if (bitNumber > 7)
                        throw new Exception("Bit can only be 0-7");
                    byteLen = 1;
                    return;
            }
        }
    }
}
