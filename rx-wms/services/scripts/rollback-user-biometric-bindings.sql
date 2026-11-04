-- 回退账号生物识别关联表：会删除全部绑定记录，执行前请先导出这些记录。
-- 不影响 AbpUsers 账号，也不删除识别服务中的人脸样本或指静脉模板。
DROP TABLE IF EXISTS `UserBiometricBinding`;
