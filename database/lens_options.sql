-- 镜片选项管理表
CREATE TABLE `dfgx_lens_options` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `option_type` int(11) NOT NULL COMMENT '选项类型（镜片类型、膜层、材质、设计名称等）',
  `option_name` varchar(100) NOT NULL COMMENT '选项名称',
  `option_value` int(11) NOT NULL COMMENT '选项值',
  `status` char(1) NOT NULL DEFAULT '0' COMMENT '状态（0正常 1停用）',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_deleted` int(11) NOT NULL DEFAULT '0' COMMENT '是否删除（0正常 1删除）',
  PRIMARY KEY (`id`),
  KEY `idx_option_type` (`option_type`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='镜片选项管理表';

-- 插入示例数据
INSERT INTO `dfgx_lens_options` (`option_type`, `option_name`, `option_value`, `status`, `remark`, `create_by`, `create_time`, `is_deleted`) VALUES
(1, '单焦点镜片', 1, '0', '单焦点镜片类型', 'admin', NOW(), 0),
(1, '多焦点镜片', 2, '0', '多焦点镜片类型', 'admin', NOW(), 0),
(1, '渐进镜片', 3, '0', '渐进镜片类型', 'admin', NOW(), 0),
(2, '防蓝光膜', 1, '0', '防蓝光保护膜层', 'admin', NOW(), 0),
(2, '防紫外线膜', 2, '0', '防紫外线保护膜层', 'admin', NOW(), 0),
(2, '防反射膜', 3, '0', '防反射膜层', 'admin', NOW(), 0),
(3, '树脂材质', 1, '0', '树脂镜片材质', 'admin', NOW(), 0),
(3, '玻璃材质', 2, '0', '玻璃镜片材质', 'admin', NOW(), 0),
(3, 'PC材质', 3, '0', '聚碳酸酯镜片材质', 'admin', NOW(), 0),
(4, '经典设计', 1, '0', '经典镜片设计', 'admin', NOW(), 0),
(4, '时尚设计', 2, '0', '时尚镜片设计', 'admin', NOW(), 0),
(4, '运动设计', 3, '0', '运动镜片设计', 'admin', NOW(), 0),
(5, '1.50', 1, '0', '折射率1.50', 'admin', NOW(), 0),
(5, '1.56', 2, '0', '折射率1.56', 'admin', NOW(), 0),
(5, '1.60', 3, '0', '折射率1.60', 'admin', NOW(), 0),
(5, '1.67', 4, '0', '折射率1.67', 'admin', NOW(), 0),
(5, '1.74', 5, '0', '折射率1.74', 'admin', NOW(), 0);

-- 创建索引优化查询性能
CREATE INDEX `idx_option_name` ON `dfgx_lens_options` (`option_name`);
CREATE INDEX `idx_option_value` ON `dfgx_lens_options` (`option_value`);
CREATE INDEX `idx_option_type_status` ON `dfgx_lens_options` (`option_type`, `status`);
