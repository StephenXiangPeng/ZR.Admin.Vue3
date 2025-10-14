-- 订单相关表ALTER语句 - 安全添加标准字段
-- 使用存储过程安全地添加字段，避免重复字段错误

DELIMITER $$

-- 创建存储过程：安全添加字段
CREATE PROCEDURE `sp_add_column_if_not_exists`(
    IN table_name VARCHAR(128),
    IN column_name VARCHAR(128),
    IN column_definition TEXT
)
BEGIN
    DECLARE column_exists INT DEFAULT 0;
    
    -- 检查字段是否存在
    SELECT COUNT(*) INTO column_exists
    FROM information_schema.COLUMNS 
    WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = table_name
    AND COLUMN_NAME = column_name;
    
    -- 如果字段不存在，则添加
    IF column_exists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', table_name, '` ADD COLUMN `', column_name, '` ', column_definition);
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

-- 1. 客户订单主表 - 安全添加标准字段
CALL sp_add_column_if_not_exists('dfgx_customer_orders', 'remark', 'varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''备注''');
CALL sp_add_column_if_not_exists('dfgx_customer_orders', 'create_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''创建者''');
CALL sp_add_column_if_not_exists('dfgx_customer_orders', 'create_time', 'datetime DEFAULT CURRENT_TIMESTAMP COMMENT ''创建时间''');
CALL sp_add_column_if_not_exists('dfgx_customer_orders', 'update_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''更新者''');
CALL sp_add_column_if_not_exists('dfgx_customer_orders', 'update_time', 'datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT ''更新时间''');

-- 2. 订单详情表 - 安全添加标准字段
CALL sp_add_column_if_not_exists('dfgx_order_details', 'remark', 'varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''备注''');
CALL sp_add_column_if_not_exists('dfgx_order_details', 'create_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''创建者''');
CALL sp_add_column_if_not_exists('dfgx_order_details', 'create_time', 'datetime DEFAULT CURRENT_TIMESTAMP COMMENT ''创建时间''');
CALL sp_add_column_if_not_exists('dfgx_order_details', 'update_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''更新者''');
CALL sp_add_column_if_not_exists('dfgx_order_details', 'update_time', 'datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT ''更新时间''');

-- 3. 订单状态历史表 - 安全添加标准字段
CALL sp_add_column_if_not_exists('dfgx_order_status_history', 'remark', 'varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''备注''');
CALL sp_add_column_if_not_exists('dfgx_order_status_history', 'create_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''创建者''');
CALL sp_add_column_if_not_exists('dfgx_order_status_history', 'create_time', 'datetime DEFAULT CURRENT_TIMESTAMP COMMENT ''创建时间''');
CALL sp_add_column_if_not_exists('dfgx_order_status_history', 'update_by', 'varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT ''更新者''');
CALL sp_add_column_if_not_exists('dfgx_order_status_history', 'update_time', 'datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT ''更新时间''');

-- 删除临时存储过程
DROP PROCEDURE IF EXISTS `sp_add_column_if_not_exists`;

DELIMITER ;

-- 添加索引优化查询性能
CREATE INDEX `idx_customer_orders_create_time` ON `dfgx_customer_orders` (`create_time`);
CREATE INDEX `idx_customer_orders_update_time` ON `dfgx_customer_orders` (`update_time`);
CREATE INDEX `idx_customer_orders_create_by` ON `dfgx_customer_orders` (`create_by`);

CREATE INDEX `idx_order_details_create_time` ON `dfgx_order_details` (`create_time`);
CREATE INDEX `idx_order_details_update_time` ON `dfgx_order_details` (`update_time`);
CREATE INDEX `idx_order_details_create_by` ON `dfgx_order_details` (`create_by`);

CREATE INDEX `idx_status_history_create_time` ON `dfgx_order_status_history` (`create_time`);
CREATE INDEX `idx_status_history_update_time` ON `dfgx_order_status_history` (`update_time`);
CREATE INDEX `idx_status_history_create_by` ON `dfgx_order_status_history` (`create_by`);

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
