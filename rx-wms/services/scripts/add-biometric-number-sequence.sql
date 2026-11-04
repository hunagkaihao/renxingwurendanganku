-- MySQL 8.0.16+：在已创建 UserBiometricBinding 的 WMS 数据库内执行，可重复运行。
-- 人脸、指静脉各自的编号跨租户共用；序列独立于绑定，解除绑定和重启均不得清零或回退。
CREATE TABLE IF NOT EXISTS `BiometricNumberSequence` (
    `Name` varchar(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL COMMENT '编号类型，face 表示人脸人员，vein 表示指静脉设备模板',
    `LastNumber` int NOT NULL DEFAULT 0 COMMENT '已分配编号的最大值，0 表示尚未分配，禁止回退',
    PRIMARY KEY (`Name`),
    CONSTRAINT `CK_BiometricNumberSequence_LastNumber` CHECK (`LastNumber` >= 0)
) ENGINE=InnoDB COMMENT='生物特征编号序列，通过事务行锁保证跨进程分配唯一编号';

-- 初始化时越过全库已有编号；重复执行也保留更大的历史分配上限。
INSERT INTO `BiometricNumberSequence` (`Name`, `LastNumber`)
SELECT 'vein', COALESCE(MAX(`FingerId`), 0) FROM `UserBiometricBinding`
ON DUPLICATE KEY UPDATE `LastNumber` = GREATEST(`LastNumber`, VALUES(`LastNumber`));

-- 兼容已启用指静脉自动编号的数据库，补齐人脸序列，不改变指静脉已分配上限。
INSERT INTO `BiometricNumberSequence` (`Name`, `LastNumber`)
SELECT 'face', COALESCE(MAX(`FaceId`), 0) FROM `UserBiometricBinding`
ON DUPLICATE KEY UPDATE `LastNumber` = GREATEST(`LastNumber`, VALUES(`LastNumber`));
