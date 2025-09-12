using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZR.Admin.Vue3.Models.ExchangeRate
{
	/// <summary>
	/// 汇率更新任务配置表
	/// </summary>
	[SugarTable("exchange_rate_task_config")]
	public class ExchangeRateTaskConfig
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[SugarColumn(ColumnName = "task_name", Length = 100, IsNullable = false)]
		[Required(ErrorMessage = "任务名称不能为空")]
		[StringLength(100, ErrorMessage = "任务名称长度不能超过100个字符")]
		public string TaskName { get; set; } = "每日汇率更新任务";

		/// <summary>
		/// 执行人员ID，逗号分隔，如：1,2,3
		/// </summary>
		[SugarColumn(ColumnName = "executors", ColumnDataType = "text", IsNullable = false)]
		[Required(ErrorMessage = "执行人员不能为空")]
		public string Executors { get; set; }

		/// <summary>
		/// 通知间隔（分钟），范围1-60
		/// </summary>
		[SugarColumn(ColumnName = "notification_interval", IsNullable = false)]
		[Range(1, 60, ErrorMessage = "通知间隔必须在1-60分钟之间")]
		public int NotificationInterval { get; set; } = 5;

		/// <summary>
		/// 任务状态：1-启用，0-禁用
		/// </summary>
		[SugarColumn(ColumnName = "status", IsNullable = false)]
		public int Status { get; set; } = 1;

		/// <summary>
		/// 任务描述
		/// </summary>
		[SugarColumn(ColumnName = "description", ColumnDataType = "text", IsNullable = true)]
		public string Description { get; set; }

		/// <summary>
		/// 创建者
		/// </summary>
		[SugarColumn(ColumnName = "create_by", Length = 64, IsNullable = true)]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 更新者
		/// </summary>
		[SugarColumn(ColumnName = "update_by", Length = 64, IsNullable = true)]
		public string UpdateBy { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = false)]
		public DateTime UpdateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
		public string Remark { get; set; }

		/// <summary>
		/// 执行人员列表（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public int[] ExecutorList
		{
			get
			{
				if (string.IsNullOrEmpty(Executors))
					return new int[0];
				return Array.ConvertAll(Executors.Split(','), int.Parse);
			}
			set
			{
				Executors = value != null ? string.Join(",", value) : string.Empty;
			}
		}

		/// <summary>
		/// 任务状态描述
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string StatusText => Status == 1 ? "启用" : "禁用";
	}

	/// <summary>
	/// 汇率填写记录表
	/// </summary>
	[SugarTable("exchange_rate_records")]
	public class ExchangeRateRecord
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 填写日期
		/// </summary>
		[SugarColumn(ColumnName = "date", IsNullable = false)]
		[Required(ErrorMessage = "填写日期不能为空")]
		public DateTime Date { get; set; }

		/// <summary>
		/// 币种ID（对应hr_export_currency字典）
		/// </summary>
		[SugarColumn(ColumnName = "currency", Length = 10, IsNullable = false)]
		[Required(ErrorMessage = "币种不能为空")]
		[StringLength(10, ErrorMessage = "币种长度不能超过10个字符")]
		public string Currency { get; set; }

		/// <summary>
		/// 汇率值，支持4位小数
		/// </summary>
		[SugarColumn(ColumnName = "exchange_rate", DecimalDigits = 4, IsNullable = false)]
		[Required(ErrorMessage = "汇率不能为空")]
		[Range(0.0001, double.MaxValue, ErrorMessage = "汇率必须大于0")]
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// 填写人用户ID
		/// </summary>
		[SugarColumn(ColumnName = "executor", IsNullable = false)]
		[Required(ErrorMessage = "填写人不能为空")]
		public int Executor { get; set; }

		/// <summary>
		/// 填写时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 备注信息
		/// </summary>
		[SugarColumn(ColumnName = "remark", ColumnDataType = "text", IsNullable = true)]
		public string Remark { get; set; }

		/// <summary>
		/// 创建者
		/// </summary>
		[SugarColumn(ColumnName = "create_by", Length = 64, IsNullable = true)]
		public string CreateBy { get; set; }

		/// <summary>
		/// 更新者
		/// </summary>
		[SugarColumn(ColumnName = "update_by", Length = 64, IsNullable = true)]
		public string UpdateBy { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = false)]
		public DateTime UpdateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 币种名称（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string CurrencyName { get; set; }

		/// <summary>
		/// 填写人姓名（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string ExecutorName { get; set; }
	}

	/// <summary>
	/// 汇率填写通知日志表
	/// </summary>
	[SugarTable("exchange_rate_notification_log")]
	public class ExchangeRateNotificationLog
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		[SugarColumn(ColumnName = "user_id", IsNullable = false)]
		[Required(ErrorMessage = "用户ID不能为空")]
		public int UserId { get; set; }

		/// <summary>
		/// 通知类型：popup-弹窗通知，email-邮件通知
		/// </summary>
		[SugarColumn(ColumnName = "notification_type", Length = 20, IsNullable = false)]
		[Required(ErrorMessage = "通知类型不能为空")]
		[StringLength(20, ErrorMessage = "通知类型长度不能超过20个字符")]
		public string NotificationType { get; set; } = "popup";

		/// <summary>
		/// 通知时间
		/// </summary>
		[SugarColumn(ColumnName = "notification_time", IsNullable = false)]
		public DateTime NotificationTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 是否已读：1-已读，0-未读
		/// </summary>
		[SugarColumn(ColumnName = "is_read", IsNullable = false)]
		public bool IsRead { get; set; } = false;

		/// <summary>
		/// 是否已填写：1-已填写，0-未填写
		/// </summary>
		[SugarColumn(ColumnName = "is_filled", IsNullable = false)]
		public bool IsFilled { get; set; } = false;

		/// <summary>
		/// 填写时间
		/// </summary>
		[SugarColumn(ColumnName = "fill_time", IsNullable = true)]
		public DateTime? FillTime { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 用户姓名（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string UserName { get; set; }

		/// <summary>
		/// 通知类型描述
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string NotificationTypeText => NotificationType switch
		{
			"popup" => "弹窗通知",
			"email" => "邮件通知",
			_ => "未知类型"
		};
	}

	/// <summary>
	/// 汇率历史记录表
	/// </summary>
	[SugarTable("exchange_rate_history")]
	public class ExchangeRateHistory
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 汇率日期
		/// </summary>
		[SugarColumn(ColumnName = "date", IsNullable = false)]
		[Required(ErrorMessage = "汇率日期不能为空")]
		public DateTime Date { get; set; }

		/// <summary>
		/// 币种ID
		/// </summary>
		[SugarColumn(ColumnName = "currency", Length = 10, IsNullable = false)]
		[Required(ErrorMessage = "币种不能为空")]
		[StringLength(10, ErrorMessage = "币种长度不能超过10个字符")]
		public string Currency { get; set; }

		/// <summary>
		/// 汇率值
		/// </summary>
		[SugarColumn(ColumnName = "exchange_rate", DecimalDigits = 4, IsNullable = false)]
		[Required(ErrorMessage = "汇率不能为空")]
		[Range(0.0001, double.MaxValue, ErrorMessage = "汇率必须大于0")]
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// 数据来源：manual-手动填写，api-接口获取
		/// </summary>
		[SugarColumn(ColumnName = "source", Length = 20, IsNullable = false)]
		[Required(ErrorMessage = "数据来源不能为空")]
		[StringLength(20, ErrorMessage = "数据来源长度不能超过20个字符")]
		public string Source { get; set; } = "manual";

		/// <summary>
		/// 是否有效：1-有效，0-无效
		/// </summary>
		[SugarColumn(ColumnName = "is_active", IsNullable = false)]
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// 创建者
		/// </summary>
		[SugarColumn(ColumnName = "create_by", Length = 64, IsNullable = true)]
		public string CreateBy { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "create_time", IsNullable = false)]
		public DateTime CreateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 更新者
		/// </summary>
		[SugarColumn(ColumnName = "update_by", Length = 64, IsNullable = true)]
		public string UpdateBy { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(ColumnName = "update_time", IsNullable = false)]
		public DateTime UpdateTime { get; set; } = DateTime.Now;

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "remark", Length = 500, IsNullable = true)]
		public string Remark { get; set; }

		/// <summary>
		/// 币种名称（非数据库字段）
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string CurrencyName { get; set; }

		/// <summary>
		/// 数据来源描述
		/// </summary>
		[SugarColumn(IsIgnore = true)]
		public string SourceText => Source switch
		{
			"manual" => "手动填写",
			"api" => "接口获取",
			_ => "未知来源"
		};
	}

	/// <summary>
	/// 汇率填写统计视图模型
	/// </summary>
	public class ExchangeRateStatsViewModel
	{
		/// <summary>
		/// 日期
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// 币种ID
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// 币种名称
		/// </summary>
		public string CurrencyName { get; set; }

		/// <summary>
		/// 填写人数
		/// </summary>
		public int FillCount { get; set; }

		/// <summary>
		/// 平均汇率
		/// </summary>
		public decimal AvgRate { get; set; }

		/// <summary>
		/// 最低汇率
		/// </summary>
		public decimal MinRate { get; set; }

		/// <summary>
		/// 最高汇率
		/// </summary>
		public decimal MaxRate { get; set; }

		/// <summary>
		/// 填写人ID列表
		/// </summary>
		public string Executors { get; set; }

		/// <summary>
		/// 填写人姓名列表
		/// </summary>
		public string ExecutorNames { get; set; }
	}

	/// <summary>
	/// 用户汇率填写状态视图模型
	/// </summary>
	public class UserExchangeRateStatusViewModel
	{
		/// <summary>
		/// 用户ID
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// 用户姓名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 部门ID
		/// </summary>
		public int DeptId { get; set; }

		/// <summary>
		/// 部门名称
		/// </summary>
		public string DeptName { get; set; }

		/// <summary>
		/// 填写状态
		/// </summary>
		public string FillStatus { get; set; }

		/// <summary>
		/// 汇率值
		/// </summary>
		public decimal? ExchangeRate { get; set; }

		/// <summary>
		/// 填写时间
		/// </summary>
		public DateTime? FillTime { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }
	}

	/// <summary>
	/// 汇率填写请求模型
	/// </summary>
	public class ExchangeRateSubmitRequest
	{
		/// <summary>
		/// 币种ID
		/// </summary>
		[Required(ErrorMessage = "币种不能为空")]
		public string Currency { get; set; }

		/// <summary>
		/// 汇率值
		/// </summary>
		[Required(ErrorMessage = "汇率不能为空")]
		[Range(0.0001, double.MaxValue, ErrorMessage = "汇率必须大于0")]
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 填写日期
		/// </summary>
		public DateTime Date { get; set; } = DateTime.Today;
	}

	/// <summary>
	/// 汇率任务配置请求模型
	/// </summary>
	public class ExchangeRateTaskConfigRequest
	{
		/// <summary>
		/// 任务名称
		/// </summary>
		[Required(ErrorMessage = "任务名称不能为空")]
		[StringLength(100, ErrorMessage = "任务名称长度不能超过100个字符")]
		public string TaskName { get; set; }

		/// <summary>
		/// 执行人员ID列表
		/// </summary>
		[Required(ErrorMessage = "执行人员不能为空")]
		public int[] Executors { get; set; }

		/// <summary>
		/// 通知间隔（分钟）
		/// </summary>
		[Required(ErrorMessage = "通知间隔不能为空")]
		[Range(1, 60, ErrorMessage = "通知间隔必须在1-60分钟之间")]
		public int NotificationInterval { get; set; }

		/// <summary>
		/// 任务状态
		/// </summary>
		[Required(ErrorMessage = "任务状态不能为空")]
		public int Status { get; set; }

		/// <summary>
		/// 任务描述
		/// </summary>
		public string Description { get; set; }
	}

	/// <summary>
	/// 用户汇率填写检查响应模型
	/// </summary>
	public class UserExchangeRateCheckResponse
	{
		/// <summary>
		/// 是否需要填写
		/// </summary>
		public bool NeedFill { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// 用户姓名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 是否在执行人员列表中
		/// </summary>
		public bool IsExecutor { get; set; }

		/// <summary>
		/// 今天是否已填写
		/// </summary>
		public bool HasFilledToday { get; set; }

		/// <summary>
		/// 任务状态
		/// </summary>
		public bool TaskEnabled { get; set; }
	}

	/// <summary>
	/// 多币种汇率填写请求模型
	/// </summary>
	public class MultiCurrencyExchangeRateSubmitRequest
	{
		/// <summary>
		/// 汇率列表
		/// </summary>
		[Required(ErrorMessage = "汇率列表不能为空")]
		public List<ExchangeRateItem> ExchangeRates { get; set; }

		/// <summary>
		/// 填写日期
		/// </summary>
		[Required(ErrorMessage = "填写日期不能为空")]
		public DateTime Date { get; set; }
	}

	/// <summary>
	/// 汇率项
	/// </summary>
	public class ExchangeRateItem
	{
		/// <summary>
		/// 币种ID
		/// </summary>
		[Required(ErrorMessage = "币种不能为空")]
		[StringLength(10, ErrorMessage = "币种长度不能超过10个字符")]
		public string Currency { get; set; }

		/// <summary>
		/// 汇率值
		/// </summary>
		[Required(ErrorMessage = "汇率不能为空")]
		[Range(0.0001, double.MaxValue, ErrorMessage = "汇率必须大于0")]
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// 备注信息
		/// </summary>
		[StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
		public string Remark { get; set; }
	}
}
