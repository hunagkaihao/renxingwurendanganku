-- MySQL 8.0.16+：创建账号—人脸编号—指静脉编号关联表。
-- 先选择 WMS 数据库；本脚本不插入示例绑定，可重复执行。
-- UserId 关联 AbpUsers.Id；录入照片和指静脉模板仍由识别服务保存。
CREATE TABLE IF NOT EXISTS `UserBiometricBinding` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL COMMENT '绑定记录 GUID',
    `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL COMMENT '关联 AbpUsers.Id，账号改名不影响绑定',
    `TenantId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NULL COMMENT '所属租户 GUID，NULL 表示宿主',
    `FaceId` int NULL COMMENT '人脸人员编号，正整数，未绑定时为 NULL',
    `FingerId` int NULL COMMENT '指静脉设备模板编号，正整数，未绑定时为 NULL',
    `IsEnabled` tinyint(1) NOT NULL DEFAULT 1 COMMENT '是否允许本组编号登录，停用仍保留编号归属',
    `CreationTime` datetime(6) NOT NULL COMMENT '绑定创建时间',
    `CreatorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NULL COMMENT '创建绑定的用户 GUID，系统操作可为 NULL',
    `LastModificationTime` datetime(6) NULL COMMENT '最后修改时间，未修改时为 NULL',
    `LastModifierId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NULL COMMENT '最后修改用户 GUID，未修改时为 NULL',
    `ConcurrencyStamp` varchar(40) NOT NULL COMMENT '并发更新标记，防止覆盖其他管理员的修改',
    `ExtraProperties` longtext NOT NULL COMMENT 'ABP 扩展属性 JSON',
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_UserBiometricBinding_FaceId` (`FaceId`),
    UNIQUE KEY `UX_UserBiometricBinding_FingerId` (`FingerId`),
    KEY `IX_UserBiometricBinding_UserId` (`UserId`),
    CONSTRAINT `FK_UserBiometricBinding_AbpUsers_UserId` FOREIGN KEY (`UserId`)
        REFERENCES `AbpUsers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `CK_UserBiometricBinding_Identifiers` CHECK (
        (`FaceId` IS NOT NULL OR `FingerId` IS NOT NULL)
        AND (`FaceId` IS NULL OR `FaceId` > 0)
        AND (`FingerId` IS NULL OR `FingerId` > 0)
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
  COMMENT='WMS 账号与人脸人员编号、指静脉模板编号的登录关联';
