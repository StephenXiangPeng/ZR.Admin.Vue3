# 汇率填写功能API接口文档

## 概述
本文档描述了汇率填写通知功能所需的后端API接口。该功能允许管理员在plantask页面设置汇率填写人员，被指定的人员在登录系统后会自动收到汇率填写通知，如果未填写则每隔指定时间弹窗提醒。

## API接口列表

### 1. 获取汇率更新任务配置
**接口地址：** `GET /ExchangeRateTask/GetConfig`

**功能描述：** 获取汇率更新任务的配置信息

**响应数据：**
```json
{
  "code": 200,
  "msg": "success",
  "data": {
    "taskName": "每日汇率更新任务",
    "executors": "1,2,3", // 执行人员ID，逗号分隔
    "notificationInterval": 5, // 通知间隔（分钟）
    "status": "1", // 任务状态：1-启用，0-禁用
    "description": "每日提醒指定人员填写当天汇率，如未填写则每隔指定时间弹窗提醒"
  }
}
```

### 2. 保存汇率更新任务配置
**接口地址：** `POST /ExchangeRateTask/SaveConfig`

**功能描述：** 保存汇率更新任务的配置信息

**请求参数：**
```json
{
  "taskName": "每日汇率更新任务",
  "executors": "1,2,3", // 执行人员ID，逗号分隔
  "notificationInterval": 5, // 通知间隔（分钟）
  "status": "1", // 任务状态：1-启用，0-禁用
  "description": "每日提醒指定人员填写当天汇率，如未填写则每隔指定时间弹窗提醒"
}
```

**响应数据：**
```json
{
  "code": 200,
  "msg": "保存成功"
}
```

### 3. 检查用户是否需要填写汇率
**接口地址：** `GET /ExchangeRateTask/CheckUserNeedFill`

**功能描述：** 检查当前用户是否需要填写今日汇率

**响应数据：**
```json
{
  "code": 200,
  "msg": "success",
  "data": {
    "needFill": true // true-需要填写，false-不需要填写
  }
}
```

### 4. 提交汇率信息
**接口地址：** `POST /ExchangeRateTask/SubmitExchangeRate`

**功能描述：** 提交用户填写的汇率信息

**请求参数：**
```json
{
  "currency": "1", // 币种ID（来自hr_export_currency字典）
  "exchangeRate": 7.2345, // 汇率值
  "remark": "备注信息", // 备注（可选）
  "date": "2024-01-15" // 填写日期
}
```

**响应数据：**
```json
{
  "code": 200,
  "msg": "提交成功"
}
```

### 5. 获取汇率填写记录
**接口地址：** `GET /ExchangeRateTask/GetRecords`

**功能描述：** 获取汇率填写记录列表

**响应数据：**
```json
{
  "code": 200,
  "msg": "success",
  "data": [
    {
      "id": 1,
      "date": "2024-01-15",
      "currency": "1", // 币种ID
      "exchangeRate": 7.2345,
      "executor": "1", // 填写人ID
      "createTime": "2024-01-15 09:30:00",
      "remark": "备注信息"
    }
  ]
}
```

## 数据库表结构建议

### 汇率更新任务配置表 (exchange_rate_task_config)
```sql
CREATE TABLE exchange_rate_task_config (
  id INT PRIMARY KEY AUTO_INCREMENT,
  task_name VARCHAR(100) NOT NULL COMMENT '任务名称',
  executors TEXT NOT NULL COMMENT '执行人员ID，逗号分隔',
  notification_interval INT DEFAULT 5 COMMENT '通知间隔（分钟）',
  status TINYINT DEFAULT 1 COMMENT '任务状态：1-启用，0-禁用',
  description TEXT COMMENT '任务描述',
  create_time DATETIME DEFAULT CURRENT_TIMESTAMP,
  update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### 汇率填写记录表 (exchange_rate_records)
```sql
CREATE TABLE exchange_rate_records (
  id INT PRIMARY KEY AUTO_INCREMENT,
  date DATE NOT NULL COMMENT '填写日期',
  currency VARCHAR(10) NOT NULL COMMENT '币种ID',
  exchange_rate DECIMAL(10,4) NOT NULL COMMENT '汇率值',
  executor INT NOT NULL COMMENT '填写人ID',
  create_time DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '填写时间',
  remark TEXT COMMENT '备注',
  INDEX idx_date (date),
  INDEX idx_executor (executor),
  INDEX idx_currency (currency)
);
```

## 业务逻辑说明

1. **任务配置：** 管理员在plantask页面可以设置汇率更新任务的执行人员和通知间隔
2. **用户检查：** 用户登录系统后，系统会检查该用户是否在汇率填写人员列表中
3. **通知机制：** 如果需要填写且未填写，系统会弹出通知对话框
4. **定时提醒：** 如果用户未填写，系统会每隔指定时间（默认5分钟）再次弹出通知
5. **记录管理：** 系统会记录所有汇率填写历史，管理员可以在plantask页面查看

## 注意事项

1. 币种信息来自系统现有的`hr_export_currency`字典
2. 用户信息来自系统现有的用户表
3. 汇率值支持4位小数精度
4. 系统会自动记录填写时间和填写人信息
5. 通知间隔可配置，建议范围1-60分钟
