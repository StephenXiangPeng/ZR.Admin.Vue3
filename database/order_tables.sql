-- 订单相关表结构 - MySQL 5.7

-- 客户订单主表
CREATE TABLE `dfgx_customer_orders` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '订单ID',
  `order_no` varchar(50) NOT NULL COMMENT '订单编号',
  `customer_name` varchar(100) DEFAULT NULL COMMENT '客户姓名',
  `customer_phone` varchar(20) DEFAULT NULL COMMENT '客户电话',
  `customer_email` varchar(100) DEFAULT NULL COMMENT '客户邮箱',
  `order_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '订单日期',
  `status` char(1) NOT NULL DEFAULT '0' COMMENT '订单状态（0待处理 1生产中 2已完成 3已取消）',
  `total_amount` decimal(10,2) DEFAULT '0.00' COMMENT '订单总金额',
  `remarks` text COMMENT '订单备注',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_deleted` int(11) NOT NULL DEFAULT '0' COMMENT '是否删除（0正常 1删除）',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_order_no` (`order_no`),
  KEY `idx_customer_name` (`customer_name`),
  KEY `idx_order_date` (`order_date`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户订单主表';

-- 订单详情表
CREATE TABLE `dfgx_order_details` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '详情ID',
  `order_id` bigint(20) NOT NULL COMMENT '订单ID',
  `lens_type` int(11) DEFAULT NULL COMMENT '镜片类型选项值',
  `lens_type_name` varchar(100) DEFAULT NULL COMMENT '镜片类型名称',
  `coating` int(11) DEFAULT NULL COMMENT '膜层选项值',
  `coating_name` varchar(100) DEFAULT NULL COMMENT '膜层名称',
  `design_name` int(11) DEFAULT NULL COMMENT '设计名称选项值',
  `design_name_text` varchar(100) DEFAULT NULL COMMENT '设计名称',
  `material` int(11) DEFAULT NULL COMMENT '材质选项值',
  `material_name` varchar(100) DEFAULT NULL COMMENT '材质名称',
  `refractive_index` int(11) DEFAULT NULL COMMENT '折射率选项值',
  `refractive_index_value` varchar(20) DEFAULT NULL COMMENT '折射率值',
  `frame_type` varchar(20) DEFAULT NULL COMMENT '镜框类型（full全框 half半框 none无框）',
  `quantity` int(11) NOT NULL DEFAULT '1' COMMENT '数量',
  `unit_price` decimal(10,2) DEFAULT '0.00' COMMENT '单价',
  `total_price` decimal(10,2) DEFAULT '0.00' COMMENT '小计',
  `left_eye_sph` varchar(20) DEFAULT NULL COMMENT '左眼球镜',
  `left_eye_cyl` varchar(20) DEFAULT NULL COMMENT '左眼柱镜',
  `left_eye_axis` varchar(20) DEFAULT NULL COMMENT '左眼轴位',
  `left_eye_add` varchar(20) DEFAULT NULL COMMENT '左眼下加光',
  `left_eye_prism` varchar(20) DEFAULT NULL COMMENT '左眼棱镜',
  `right_eye_sph` varchar(20) DEFAULT NULL COMMENT '右眼球镜',
  `right_eye_cyl` varchar(20) DEFAULT NULL COMMENT '右眼柱镜',
  `right_eye_axis` varchar(20) DEFAULT NULL COMMENT '右眼轴位',
  `right_eye_add` varchar(20) DEFAULT NULL COMMENT '右眼下加光',
  `right_eye_prism` varchar(20) DEFAULT NULL COMMENT '右眼棱镜',
  `auto_remarks` text COMMENT '自动备注',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_deleted` int(11) NOT NULL DEFAULT '0' COMMENT '是否删除（0正常 1删除）',
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_lens_type` (`lens_type`),
  KEY `idx_material` (`material`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`),
  CONSTRAINT `fk_order_details_order` FOREIGN KEY (`order_id`) REFERENCES `dfgx_customer_orders` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单详情表';

-- 订单状态历史表
CREATE TABLE `dfgx_order_status_history` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '历史ID',
  `order_id` bigint(20) NOT NULL COMMENT '订单ID',
  `status` char(1) NOT NULL COMMENT '状态',
  `status_name` varchar(50) NOT NULL COMMENT '状态名称',
  `remark` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '备注',
  `operator` varchar(64) DEFAULT NULL COMMENT '操作人',
  `create_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  CONSTRAINT `fk_status_history_order` FOREIGN KEY (`order_id`) REFERENCES `dfgx_customer_orders` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='订单状态历史表';

-- 插入示例订单数据
INSERT INTO `dfgx_customer_orders` (`order_no`, `customer_name`, `customer_phone`, `customer_email`, `order_date`, `status`, `total_amount`, `remarks`, `create_by`, `create_time`, `is_deleted`) VALUES
('ORD20241201001', '张三', '13800138001', 'zhangsan@example.com', '2024-12-01 10:30:00', '0', 850.00, '客户要求加急处理', 'admin', NOW(), 0),
('ORD20241201002', '李四', '13800138002', 'lisi@example.com', '2024-12-01 14:20:00', '1', 1200.00, '高端定制镜片', 'admin', NOW(), 0),
('ORD20241201003', '王五', '13800138003', 'wangwu@example.com', '2024-12-01 16:45:00', '2', 650.00, '已完成交付', 'admin', NOW(), 0);

-- 插入示例订单详情数据
INSERT INTO `dfgx_order_details` (`order_id`, `lens_type`, `lens_type_name`, `coating`, `coating_name`, `design_name`, `design_name_text`, `material`, `material_name`, `refractive_index`, `refractive_index_value`, `frame_type`, `quantity`, `unit_price`, `total_price`, `left_eye_sph`, `left_eye_cyl`, `left_eye_axis`, `left_eye_add`, `left_eye_prism`, `right_eye_sph`, `right_eye_cyl`, `right_eye_axis`, `right_eye_add`, `right_eye_prism`, `auto_remarks`, `create_by`, `create_time`, `is_deleted`) VALUES
(1, 1, '单焦点镜片', 1, '防蓝光膜', 1, '经典设计', 1, '树脂材质', 1, '1.50', 'full', 1, 850.00, 850.00, '-2.00', '-0.50', '90', '', '', '-2.25', '-0.75', '90', '', '', 'ST自动改成FT,需要不干胶标贴', 'admin', NOW(), 0),
(2, 2, '多焦点镜片', 2, '防紫外线膜', 2, '时尚设计', 2, '玻璃材质', 2, '1.56', 'half', 1, 1200.00, 1200.00, '-3.00', '-1.00', '180', '+2.00', '', '-3.25', '-1.25', '180', '+2.00', '', '需要不干胶标贴', 'admin', NOW(), 0),
(3, 3, '渐进镜片', 3, '防反射膜', 3, '运动设计', 3, 'PC材质', 3, '1.60', 'none', 1, 650.00, 650.00, '-1.50', '-0.25', '45', '+1.50', '', '-1.75', '-0.50', '45', '+1.50', '', '需要不干胶标贴', 'admin', NOW(), 0);

-- 插入示例状态历史数据
INSERT INTO `dfgx_order_status_history` (`order_id`, `status`, `status_name`, `remark`, `operator`, `create_by`, `create_time`) VALUES
(1, '0', '待处理', '订单已创建，等待生产', 'admin', 'admin', '2024-12-01 10:30:00'),
(2, '0', '待处理', '订单已创建，等待生产', 'admin', 'admin', '2024-12-01 14:20:00'),
(2, '1', '生产中', '开始生产镜片', 'admin', 'admin', '2024-12-01 15:00:00'),
(3, '0', '待处理', '订单已创建，等待生产', 'admin', 'admin', '2024-12-01 16:45:00'),
(3, '1', '生产中', '开始生产镜片', 'admin', 'admin', '2024-12-01 17:00:00'),
(3, '2', '已完成', '镜片生产完成，已交付', 'admin', 'admin', '2024-12-01 18:30:00');

-- 创建索引优化查询性能
CREATE INDEX `idx_order_customer_phone` ON `dfgx_customer_orders` (`customer_phone`);
CREATE INDEX `idx_order_customer_email` ON `dfgx_customer_orders` (`customer_email`);
CREATE INDEX `idx_order_status_date` ON `dfgx_customer_orders` (`status`, `order_date`);

-- 创建视图：订单汇总信息
CREATE VIEW `v_order_summary` AS
SELECT 
    o.id,
    o.order_no,
    o.customer_name,
    o.customer_phone,
    o.order_date,
    o.status,
    CASE o.status
        WHEN '0' THEN '待处理'
        WHEN '1' THEN '生产中'
        WHEN '2' THEN '已完成'
        WHEN '3' THEN '已取消'
        ELSE '未知状态'
    END as status_name,
    o.total_amount,
    o.remarks,
    COUNT(d.id) as detail_count,
    SUM(d.quantity) as total_quantity,
    o.create_time,
    o.update_time
FROM dfgx_customer_orders o
LEFT JOIN dfgx_order_details d ON o.id = d.order_id AND d.is_deleted = 0
WHERE o.is_deleted = 0
GROUP BY o.id, o.order_no, o.customer_name, o.customer_phone, o.order_date, o.status, o.total_amount, o.remarks, o.create_time, o.update_time;

-- 创建存储过程：生成订单编号
DELIMITER $$
CREATE PROCEDURE `sp_generate_order_no`(OUT order_no VARCHAR(50))
BEGIN
    DECLARE today_str VARCHAR(8);
    DECLARE max_num INT DEFAULT 0;
    DECLARE new_num INT DEFAULT 1;
    
    -- 获取今天的日期字符串
    SET today_str = DATE_FORMAT(NOW(), '%Y%m%d');
    
    -- 获取今天最大的订单编号
    SELECT COALESCE(MAX(CAST(SUBSTRING(order_no, 10) AS UNSIGNED)), 0) INTO max_num
    FROM dfgx_customer_orders 
    WHERE order_no LIKE CONCAT('ORD', today_str, '%')
    AND is_deleted = 0;
    
    -- 生成新的订单编号
    SET new_num = max_num + 1;
    SET order_no = CONCAT('ORD', today_str, LPAD(new_num, 3, '0'));
END$$
DELIMITER ;

-- 创建触发器：订单状态变更时自动记录历史
DELIMITER $$
CREATE TRIGGER `tr_order_status_change` 
AFTER UPDATE ON `dfgx_customer_orders`
FOR EACH ROW
BEGIN
    IF OLD.status != NEW.status THEN
        INSERT INTO dfgx_order_status_history (order_id, status, status_name, remark, operator, create_by, create_time)
        VALUES (
            NEW.id,
            NEW.status,
            CASE NEW.status
                WHEN '0' THEN '待处理'
                WHEN '1' THEN '生产中'
                WHEN '2' THEN '已完成'
                WHEN '3' THEN '已取消'
                ELSE '未知状态'
            END,
            CONCAT('状态从 ', 
                CASE OLD.status
                    WHEN '0' THEN '待处理'
                    WHEN '1' THEN '生产中'
                    WHEN '2' THEN '已完成'
                    WHEN '3' THEN '已取消'
                    ELSE '未知状态'
                END,
                ' 变更为 ',
                CASE NEW.status
                    WHEN '0' THEN '待处理'
                    WHEN '1' THEN '生产中'
                    WHEN '2' THEN '已完成'
                    WHEN '3' THEN '已取消'
                    ELSE '未知状态'
                END
            ),
            NEW.update_by,
            NEW.update_by,
            NOW()
        );
    END IF;
END$$
DELIMITER ;
