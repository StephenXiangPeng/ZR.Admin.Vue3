# SqlSugar 字段映射解决方案

## 问题描述

当数据库字段名和C#属性名不一致时，SqlSugar无法正确映射数据。例如：
- 数据库字段：`task_name`（下划线命名）
- C#属性：`TaskName`（驼峰命名）

## 解决方案

### 1. 使用 `ColumnName` 属性

在 `SugarColumn` 特性中指定 `ColumnName` 参数：

```csharp
[SugarColumn(ColumnName = "task_name", Length = 100, IsNullable = false)]
public string TaskName { get; set; }
```

### 2. 完整的字段映射示例

```csharp
public class ExchangeRateTaskConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "task_name", Length = 100, IsNullable = false)]
    public string TaskName { get; set; }

    [SugarColumn(ColumnName = "executors", ColumnDataType = "text", IsNullable = false)]
    public string Executors { get; set; }

    [SugarColumn(ColumnName = "notification_interval", IsNullable = false)]
    public int NotificationInterval { get; set; }

    [SugarColumn(ColumnName = "status", IsNullable = false)]
    public int Status { get; set; }

    [SugarColumn(ColumnName = "description", ColumnDataType = "text", IsNullable = true)]
    public string Description { get; set; }

    [SugarColumn(ColumnName = "create_by", Length = 64, IsNullable = true)]
    public string CreateBy { get; set; }

    [SugarColumn(ColumnName = "create_time", IsNullable = false)]
    public DateTime CreateTime { get; set; }

    [SugarColumn(ColumnName = "update_by", Length = 64, IsNullable = true)]
    public string UpdateBy { get; set; }

    [SugarColumn(ColumnName = "update_time", IsNullable = false)]
    public DateTime UpdateTime { get; set; }

    [SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
    public string Remark { get; set; }
}
```

## 字段映射对照表

### ExchangeRateTaskConfig 表

| C# 属性名 | 数据库字段名 | 说明 |
|-----------|-------------|------|
| Id | id | 主键 |
| TaskName | task_name | 任务名称 |
| Executors | executors | 执行人员 |
| NotificationInterval | notification_interval | 通知间隔 |
| Status | status | 任务状态 |
| Description | description | 任务描述 |
| CreateBy | create_by | 创建者 |
| CreateTime | create_time | 创建时间 |
| UpdateBy | update_by | 更新者 |
| UpdateTime | update_time | 更新时间 |
| Remark | remark | 备注 |

### ExchangeRateRecord 表

| C# 属性名 | 数据库字段名 | 说明 |
|-----------|-------------|------|
| Id | id | 主键 |
| Date | date | 填写日期 |
| Currency | currency | 币种ID |
| ExchangeRate | exchange_rate | 汇率值 |
| Executor | executor | 填写人 |
| CreateTime | create_time | 填写时间 |
| Remark | remark | 备注 |
| CreateBy | create_by | 创建者 |
| UpdateBy | update_by | 更新者 |
| UpdateTime | update_time | 更新时间 |

### ExchangeRateNotificationLog 表

| C# 属性名 | 数据库字段名 | 说明 |
|-----------|-------------|------|
| Id | id | 主键 |
| UserId | user_id | 用户ID |
| NotificationType | notification_type | 通知类型 |
| NotificationTime | notification_time | 通知时间 |
| IsRead | is_read | 是否已读 |
| IsFilled | is_filled | 是否已填写 |
| FillTime | fill_time | 填写时间 |
| CreateTime | create_time | 创建时间 |

### ExchangeRateHistory 表

| C# 属性名 | 数据库字段名 | 说明 |
|-----------|-------------|------|
| Id | id | 主键 |
| Date | date | 汇率日期 |
| Currency | currency | 币种ID |
| ExchangeRate | exchange_rate | 汇率值 |
| Source | source | 数据来源 |
| IsActive | is_active | 是否有效 |
| CreateBy | create_by | 创建者 |
| CreateTime | create_time | 创建时间 |
| UpdateBy | update_by | 更新者 |
| UpdateTime | update_time | 更新时间 |
| Remark | remark | 备注 |

## 其他解决方案

### 1. 全局配置命名转换

在SqlSugar配置中设置全局命名转换：

```csharp
var config = new ConnectionConfig
{
    ConnectionString = connectionString,
    DbType = DbType.MySql,
    IsAutoCloseConnection = true,
    InitKeyType = InitKeyType.Attribute,
    // 设置命名转换
    MoreSettings = new ConnMoreSettings()
    {
        IsAutoRemoveDataCache = true,
        IsAutoUpdateQueryFilter = true
    }
};

// 设置实体和数据库字段的转换规则
config.MappingTables = new Dictionary<string, string>
{
    { "ExchangeRateTaskConfig", "exchange_rate_task_config" }
};

config.MappingColumns = new Dictionary<string, Dictionary<string, string>>
{
    {
        "ExchangeRateTaskConfig",
        new Dictionary<string, string>
        {
            { "TaskName", "task_name" },
            { "NotificationInterval", "notification_interval" },
            { "CreateTime", "create_time" },
            { "UpdateTime", "update_time" }
        }
    }
};
```

### 2. 使用特性配置

```csharp
[SugarTable("exchange_rate_task_config")]
public class ExchangeRateTaskConfig
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(ColumnName = "task_name")]
    public string TaskName { get; set; }

    [SugarColumn(ColumnName = "notification_interval")]
    public int NotificationInterval { get; set; }
}
```

## 注意事项

1. **一致性**：确保所有实体类都使用相同的命名转换规则
2. **性能**：字段映射不会影响查询性能
3. **维护性**：建议使用 `ColumnName` 属性，便于维护和理解
4. **数据库设计**：如果可能，建议数据库字段名使用下划线命名，C#属性使用驼峰命名

## 验证映射

可以通过以下方式验证字段映射是否正确：

```csharp
// 查看生成的SQL
db.Aop.OnLogExecuting = (sql, pars) =>
{
    Console.WriteLine($"SQL: {sql}");
};

// 测试查询
var config = await db.Queryable<ExchangeRateTaskConfig>().FirstAsync();
Console.WriteLine($"TaskName: {config.TaskName}");
```

## 常见问题

### 1. 字段映射不生效

**原因**：可能没有正确设置 `ColumnName` 属性
**解决**：检查 `SugarColumn` 特性配置

### 2. 查询结果为空

**原因**：字段名映射错误
**解决**：检查数据库字段名和映射配置

### 3. 插入数据失败

**原因**：必填字段映射错误
**解决**：检查 `IsNullable` 和字段映射配置
