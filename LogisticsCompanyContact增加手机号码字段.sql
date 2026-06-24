-- 物流公司联系人表增加手机号码字段
-- 说明: 与 PhoneNumber 并列，用于存储联系人手机号码

ALTER TABLE `LogisticsCompanyContact`
ADD COLUMN `MobileNumber` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT '联系人手机号码' AFTER `PhoneNumber`;
