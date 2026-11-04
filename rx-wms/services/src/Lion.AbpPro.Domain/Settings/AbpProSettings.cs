namespace Lion.AbpPro.Settings
{
    public static class AbpProSettings
    {
        public const string Prefix = "setting_";

        /// <summary>
        /// 前端控件类型
        /// </summary>
        public static class ControlType
        {
            public const string Default = "Type";
            public const string TypeText = "Text";
            public const string TypeCheckBox = "CheckBox";
            public const string Number = "Number";
        }

        /// <summary>
        /// 系统控制分组
        /// </summary>
        public static class Group
        {
            public const string Default = "Setting.Group";
            public const string SystemManagement = Default + ".System";
            public const string OtherManagement = Default + ".Other";
            public const string AllocationCellManagement = Default + ".Cell";
            public const string VerificationManagement = Default + ".Verification";
        }

        /// <summary>
        /// 其他控制分组
        /// </summary>
        public static class Other
        {
            private const string Default = "Setting.Group.Other";
            public const string Github = Default + ".Github";
        }

        /// <summary>
        /// cell控制分组
        /// </summary>
        public static class Cell
        {
            //private const string Default = "Setting.Group.Cell";
            public const string X = "XBigToSmall";
            public const string Y = "YBigToSmall";
            public const string Z = "ZBigToSmall";
        }
        public static class Verification
        {
            //private const string Default = "Setting.Group.Cell";
            public const string Finger = "Finger";
            public const string Face = "Face";
        }
    }
}