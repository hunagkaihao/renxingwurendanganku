using Lion.AbpPro.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Lion.AbpPro.Settings
{
    public class AbpProSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(AbpProSettings.MySetting1));
            OverrideDefalutSettings(context);
        }

        /// <summary>
        /// 重写默认setting添加自定义属性
        /// </summary>
        private static void OverrideDefalutSettings(ISettingDefinitionContext context)
        {
            context.GetOrNull("Abp.Localization.DefaultLanguage")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeText);

            context.GetOrNull("Abp.Identity.Password.RequiredLength")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.Number);

            context.GetOrNull("Abp.Identity.Password.RequiredUniqueChars")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.Number);

            context.GetOrNull("Abp.Identity.Password.RequireNonAlphanumeric")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox);

            context.GetOrNull("Abp.Identity.Password.RequireLowercase")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox);

            context.GetOrNull("Abp.Identity.Password.RequireUppercase")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox);

            context.GetOrNull("Abp.Identity.Password.RequireDigit")
                .WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.SystemManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox);

            //设置配置项
            //排
            context.Add(new SettingDefinition(
                    AbpProSettings.Cell.Z,
                    "true",
                    L("DisplayName:" + AbpProSettings.Cell.Z),
                    L("Description:" + AbpProSettings.Cell.Z)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.AllocationCellManagement)
                 .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox));
            //层
            context.Add(new SettingDefinition(
                    AbpProSettings.Cell.Y,
                    "true",
                    L("DisplayName:" + AbpProSettings.Cell.Y),
                    L("Description:" + AbpProSettings.Cell.Y)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.AllocationCellManagement)
                 .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox));
            //列
            context.Add(new SettingDefinition(
                    AbpProSettings.Cell.X,
                    "true",
                    L("DisplayName:" + AbpProSettings.Cell.X),
                    L("Description:" + AbpProSettings.Cell.X)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.AllocationCellManagement)
                 .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox));

            //验证方式
            context.Add(new SettingDefinition(
                    AbpProSettings.Verification.Face,
                    "true",
                    L("DisplayName:" + AbpProSettings.Verification.Face),
                    L("Description:" + AbpProSettings.Verification.Face)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.VerificationManagement)
                 .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox));

            context.Add(new SettingDefinition(
                    AbpProSettings.Verification.Finger,
                    "true",
                    L("DisplayName:" + AbpProSettings.Verification.Finger),
                    L("Description:" + AbpProSettings.Verification.Finger)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.VerificationManagement)
                 .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeCheckBox));


            context.Add(new SettingDefinition(
                    AbpProSettings.Other.Github,
                    "https://gitee.com/",
                    L("DisplayName:" + AbpProSettings.Other.Github),
                    L("Description:" + AbpProSettings.Other.Github)
                ).WithProperty(AbpProSettings.Group.Default,
                    AbpProSettings.Group.OtherManagement)
                .WithProperty(AbpProSettings.ControlType.Default,
                    AbpProSettings.ControlType.TypeText));


        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AbpProResource>(name);
        }
    }
}