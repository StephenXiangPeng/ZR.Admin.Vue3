-- 订单相关表ALTER语句 - 简化版本
-- 先检查字段是否存在，再决定是否添加

-- 检查并添加字段的简单方法
-- 如果遇到 "Duplicate column name" 错误，说明字段已存在，可以忽略

-- 1. 客户订单主表 - 添加标准字段（如果不存在）
-- 如果字段已存在，会报错但不会影响数据，可以安全忽略
ALTER TABLE `dfgx_customer_orders` 
ADD COLUMN IF NOT EXISTS `remark` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '备注',
ADD COLUMN IF NOT EXISTS `create_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '创建者',
ADD COLUMN IF NOT EXISTS `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
ADD COLUMN IF NOT EXISTS `update_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '更新者',
ADD COLUMN IF NOT EXISTS `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间';

-- 2. 订单详情表 - 添加标准字段（如果不存在）
ALTER TABLE `dfgx_order_details` 
ADD COLUMN IF NOT EXISTS `remark` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '备注',
ADD COLUMN IF NOT EXISTS `create_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '创建者',
ADD COLUMN IF NOT EXISTS `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
ADD COLUMN IF NOT EXISTS `update_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '更新者',
ADD COLUMN IF NOT EXISTS `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间';

-- 3. 订单状态历史表 - 添加标准字段（如果不存在）
ALTER TABLE `dfgx_order_status_history` 
ADD COLUMN IF NOT EXISTS `remark` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '备注',
ADD COLUMN IF NOT EXISTS `create_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '创建者',
ADD COLUMN IF NOT EXISTS `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
ADD COLUMN IF NOT EXISTS `update_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '更新者',
ADD COLUMN IF NOT EXISTS `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间';

-- 添加索引优化查询性能（如果不存在）
CREATE INDEX IF NOT EXISTS `idx_customer_orders_create_time` ON `dfgx_customer_orders` (`create_time`);
CREATE INDEX IF NOT EXISTS `idx_customer_orders_update_time` ON `dfgx_customer_orders` (`update_time`);
CREATE INDEX IF NOT EXISTS `idx_customer_orders_create_by` ON `dfgx_customer_orders` (`create_by`);

CREATE INDEX IF NOT EXISTS `idx_order_details_create_time` ON `dfgx_order_details` (`create_time`);
CREATE INDEX IF NOT EXISTS `idx_order_details_update_time` ON `dfgx_order_details` (`update_time`);
CREATE INDEX IF NOT EXISTS `idx_order_details_create_by` ON `dfgx_order_details` (`create_by`);

CREATE INDEX IF NOT EXISTS `idx_status_history_create_time` ON `dfgx_order_status_history` (`create_time`);
CREATE INDEX IF NOT EXISTS `idx_status_history_update_time` ON `dfgx_order_status_history` (`update_time`);
CREATE INDEX IF NOT EXISTS `idx_status_history_create_by` ON `dfgx_order_status_history` (`create_by`);

-- 更新现有数据的标准字段（可选）
-- 为现有记录设置默认的创建者和创建时间
UPDATE `dfgx_customer_orders` 
SET `create_by` = 'system', `create_time` = NOW() 
WHERE `create_by` IS NULL;

UPDATE `dfgx_order_details` 
SET `create_by` = 'system', `create_time` = NOW() 
WHERE `create_by` IS NULL;

UPDATE `dfgx_order_status_history` 
SET `create_by` = 'system', `create_time` = NOW() 
WHERE `create_by` IS NULL;
