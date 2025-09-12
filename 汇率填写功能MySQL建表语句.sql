-- 汇率填写功能 MySQL 5.7 建表语句
-- 创建时间: 2024年
-- 说明: 支持汇率填写通知功能的数据库表结构

-- 1. 汇率更新任务配置表
CREATE TABLE `exchange_rate_task_config` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `task_name` varchar(100) NOT NULL DEFAULT '每日汇率更新任务' COMMENT '任务名称',
  `executors` text NOT NULL COMMENT '执行人员ID，逗号分隔，如：1,2,3',
  `notification_interval` int(11) NOT NULL DEFAULT 5 COMMENT '通知间隔（分钟），范围1-60',
  `status` tinyint(1) NOT NULL DEFAULT 1 COMMENT '任务状态：1-启用，0-禁用',
  `description` text COMMENT '任务描述',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='汇率更新任务配置表';

-- 2. 汇率填写记录表
CREATE TABLE `exchange_rate_records` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `date` date NOT NULL COMMENT '填写日期',
  `currency` varchar(10) NOT NULL COMMENT '币种ID（对应hr_export_currency字典）',
  `exchange_rate` decimal(10,4) NOT NULL COMMENT '汇率值，支持4位小数',
  `executor` int(11) NOT NULL COMMENT '填写人用户ID',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '填写时间',
  `remark` text COMMENT '备注信息',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  KEY `idx_date` (`date`),
  KEY `idx_executor` (`executor`),
  KEY `idx_currency` (`currency`),
  KEY `idx_create_time` (`create_time`),
  UNIQUE KEY `uk_date_currency_executor` (`date`, `currency`, `executor`) COMMENT '同一天同一币种同一人只能填写一次'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='汇率填写记录表';

-- 3. 汇率填写通知日志表（可选，用于记录通知发送情况）
CREATE TABLE `exchange_rate_notification_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `user_id` int(11) NOT NULL COMMENT '用户ID',
  `notification_type` varchar(20) NOT NULL DEFAULT 'popup' COMMENT '通知类型：popup-弹窗通知，email-邮件通知',
  `notification_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '通知时间',
  `is_read` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否已读：1-已读，0-未读',
  `is_filled` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否已填写：1-已填写，0-未填写',
  `fill_time` datetime DEFAULT NULL COMMENT '填写时间',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_notification_time` (`notification_time`),
  KEY `idx_is_filled` (`is_filled`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='汇率填写通知日志表';

-- 4. 汇率历史记录表（可选，用于存储历史汇率数据）
CREATE TABLE `exchange_rate_history` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `date` date NOT NULL COMMENT '汇率日期',
  `currency` varchar(10) NOT NULL COMMENT '币种ID',
  `exchange_rate` decimal(10,4) NOT NULL COMMENT '汇率值',
  `source` varchar(20) NOT NULL DEFAULT 'manual' COMMENT '数据来源：manual-手动填写，api-接口获取',
  `is_active` tinyint(1) NOT NULL DEFAULT 1 COMMENT '是否有效：1-有效，0-无效',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `idx_date` (`date`),
  KEY `idx_currency` (`currency`),
  KEY `idx_is_active` (`is_active`),
  UNIQUE KEY `uk_date_currency` (`date`, `currency`) COMMENT '同一天同一币种只能有一条记录'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='汇率历史记录表';

-- 插入默认配置数据
INSERT INTO `exchange_rate_task_config` (
  `task_name`, 
  `executors`, 
  `notification_interval`, 
  `status`, 
  `description`,
  `create_by`,
  `remark`
) VALUES (
  '每日汇率更新任务',
  '1', -- 默认管理员ID，请根据实际情况修改
  5,
  1,
  '每日提醒指定人员填写当天汇率，如未填写则每隔指定时间弹窗提醒',
  'system',
  '系统默认配置'
);

-- 创建视图：汇率填写统计视图
CREATE VIEW `v_exchange_rate_stats` AS
SELECT 
  er.date,
  er.currency,
  COUNT(DISTINCT er.executor) as fill_count,
  AVG(er.exchange_rate) as avg_rate,
  MIN(er.exchange_rate) as min_rate,
  MAX(er.exchange_rate) as max_rate,
  GROUP_CONCAT(DISTINCT er.executor) as executors
FROM exchange_rate_records er
GROUP BY er.date, er.currency
ORDER BY er.date DESC, er.currency;

-- 创建视图：用户汇率填写情况视图
CREATE VIEW `v_user_exchange_rate_status` AS
SELECT 
  u.user_id,
  u.user_name,
  u.dept_id,
  u.dept_name,
  CASE 
    WHEN er.id IS NOT NULL THEN '已填写'
    ELSE '未填写'
  END as fill_status,
  er.exchange_rate,
  er.create_time as fill_time,
  er.remark
FROM (
  -- 这里需要根据实际的用户表结构调整
  SELECT 1 as user_id, 'admin' as user_name, 1 as dept_id, '管理部' as dept_name
  -- 请根据实际用户表结构添加更多用户
) u
LEFT JOIN exchange_rate_records er ON u.user_id = er.executor 
  AND er.date = CURDATE()
ORDER BY u.user_id;

-- 创建存储过程：检查用户是否需要填写汇率
DELIMITER //
CREATE PROCEDURE `sp_check_user_need_fill_exchange_rate`(
  IN p_user_id INT,
  OUT p_need_fill TINYINT(1)
)
BEGIN
  DECLARE v_is_executor TINYINT(1) DEFAULT 0;
  DECLARE v_has_filled TINYINT(1) DEFAULT 0;
  DECLARE v_task_status TINYINT(1) DEFAULT 0;
  
  -- 检查用户是否在执行人员列表中
  SELECT COUNT(1) INTO v_is_executor
  FROM exchange_rate_task_config 
  WHERE status = 1 
    AND FIND_IN_SET(p_user_id, executors) > 0;
  
  -- 检查用户今天是否已经填写过
  SELECT COUNT(1) INTO v_has_filled
  FROM exchange_rate_records 
  WHERE executor = p_user_id 
    AND date = CURDATE();
  
  -- 判断是否需要填写
  IF v_is_executor = 1 AND v_has_filled = 0 THEN
    SET p_need_fill = 1;
  ELSE
    SET p_need_fill = 0;
  END IF;
END //
DELIMITER ;

-- 创建存储过程：获取汇率填写统计
DELIMITER //
CREATE PROCEDURE `sp_get_exchange_rate_stats`(
  IN p_start_date DATE,
  IN p_end_date DATE
)
BEGIN
  SELECT 
    er.date,
    er.currency,
    COUNT(DISTINCT er.executor) as fill_count,
    AVG(er.exchange_rate) as avg_rate,
    MIN(er.exchange_rate) as min_rate,
    MAX(er.exchange_rate) as max_rate,
    GROUP_CONCAT(DISTINCT er.executor) as executors
  FROM exchange_rate_records er
  WHERE er.date BETWEEN p_start_date AND p_end_date
  GROUP BY er.date, er.currency
  ORDER BY er.date DESC, er.currency;
END //
DELIMITER ;

-- 创建触发器：汇率记录插入后自动更新通知日志
DELIMITER //
CREATE TRIGGER `tr_exchange_rate_record_insert` 
AFTER INSERT ON `exchange_rate_records`
FOR EACH ROW
BEGIN
  -- 更新该用户当天的通知日志为已填写状态
  UPDATE exchange_rate_notification_log 
  SET is_filled = 1, fill_time = NEW.create_time
  WHERE user_id = NEW.executor 
    AND DATE(notification_time) = NEW.date
    AND is_filled = 0;
END //
DELIMITER ;

-- 创建索引优化查询性能
CREATE INDEX `idx_exchange_rate_records_date_executor` ON `exchange_rate_records` (`date`, `executor`);
CREATE INDEX `idx_exchange_rate_records_currency_date` ON `exchange_rate_records` (`currency`, `date`);

-- 添加表注释
ALTER TABLE `exchange_rate_task_config` COMMENT = '汇率更新任务配置表 - 存储汇率填写任务的配置信息';
ALTER TABLE `exchange_rate_records` COMMENT = '汇率填写记录表 - 存储用户填写的汇率记录';
ALTER TABLE `exchange_rate_notification_log` COMMENT = '汇率填写通知日志表 - 记录通知发送和用户响应情况';
ALTER TABLE `exchange_rate_history` COMMENT = '汇率历史记录表 - 存储历史汇率数据';

-- 权限设置（根据实际需要调整）
-- GRANT SELECT, INSERT, UPDATE, DELETE ON exchange_rate_task_config TO 'app_user'@'%';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON exchange_rate_records TO 'app_user'@'%';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON exchange_rate_notification_log TO 'app_user'@'%';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON exchange_rate_history TO 'app_user'@'%';
-- GRANT EXECUTE ON PROCEDURE sp_check_user_need_fill_exchange_rate TO 'app_user'@'%';
-- GRANT EXECUTE ON PROCEDURE sp_get_exchange_rate_stats TO 'app_user'@'%';

