using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZR.Admin.Vue3.Models.ExchangeRate;
using ZR.Admin.Vue3.Services.ExchangeRate;

namespace ZR.Admin.Vue3.Controllers.ExchangeRate
{
	/// <summary>
	/// 汇率填写控制器
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class ExchangeRateTaskController : ControllerBase
	{
		private readonly IExchangeRateService _exchangeRateService;
		private readonly ILogger<ExchangeRateTaskController> _logger;

		public ExchangeRateTaskController(IExchangeRateService exchangeRateService, ILogger<ExchangeRateTaskController> logger)
		{
			_exchangeRateService = exchangeRateService;
			_logger = logger;
		}

		/// <summary>
		/// 获取汇率更新任务配置
		/// </summary>
		/// <returns></returns>
		[HttpGet("GetConfig")]
		public async Task<IActionResult> GetConfig()
		{
			try
			{
				var config = await _exchangeRateService.GetTaskConfigAsync();
				return Ok(new
				{
					code = 200,
					msg = "获取配置成功",
					data = new
					{
						taskName = config.TaskName,
						executors = config.Executors,
						notificationInterval = config.NotificationInterval,
						status = config.Status.ToString(),
						description = config.Description
					}
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"获取配置失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 保存汇率更新任务配置
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("SaveConfig")]
		public async Task<IActionResult> SaveConfig([FromBody] ExchangeRateTaskConfigRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return Ok(new
					{
						code = 400,
						msg = "请求参数无效",
						data = ModelState
					});
				}

				var result = await _exchangeRateService.SaveTaskConfigAsync(request);

				if (result)
				{
					return Ok(new
					{
						code = 200,
						msg = "保存配置成功"
					});
				}
				else
				{
					return Ok(new
					{
						code = 500,
						msg = "保存配置失败"
					});
				}
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"保存配置失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 检查用户是否需要填写汇率
		/// </summary>
		/// <param name="userId">用户ID，如果不传则从当前登录用户获取</param>
		/// <returns></returns>
		[HttpGet("CheckUserNeedFill")]
		public async Task<IActionResult> CheckUserNeedFill(int? userId = null)
		{
			try
			{
				// 如果没有传入userId，从当前登录用户获取
				var currentUserId = userId ?? GetCurrentUserId();

				if (currentUserId <= 0)
				{
					return Ok(new
					{
						code = 401,
						msg = "用户未登录"
					});
				}

				var response = await _exchangeRateService.CheckUserNeedFillAsync(currentUserId);

				return Ok(new
				{
					code = 200,
					msg = "检查成功",
					data = new
					{
						needFill = response.NeedFill,
						userId = response.UserId,
						userName = response.UserName,
						isExecutor = response.IsExecutor,
						hasFilledToday = response.HasFilledToday,
						taskEnabled = response.TaskEnabled
					}
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"检查失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 提交汇率信息
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("SubmitExchangeRate")]
		public async Task<IActionResult> SubmitExchangeRate([FromBody] ExchangeRateSubmitRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return Ok(new
					{
						code = 400,
						msg = "请求参数无效",
						data = ModelState
					});
				}

				var currentUserId = GetCurrentUserId();
				if (currentUserId <= 0)
				{
					return Ok(new
					{
						code = 401,
						msg = "用户未登录"
					});
				}

				var result = await _exchangeRateService.SubmitExchangeRateAsync(currentUserId, request);

				if (result)
				{
					return Ok(new
					{
						code = 200,
						msg = "提交成功"
					});
				}
				else
				{
					return Ok(new
					{
						code = 500,
						msg = "提交失败"
					});
				}
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"提交失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 获取汇率填写记录
		/// </summary>
		/// <param name="startDate">开始日期</param>
		/// <param name="endDate">结束日期</param>
		/// <param name="userId">用户ID，不传则获取所有用户</param>
		/// <returns></returns>
		[HttpGet("GetRecords")]
		public async Task<IActionResult> GetRecords(DateTime? startDate = null, DateTime? endDate = null, int? userId = null)
		{
			try
			{
				var records = await _exchangeRateService.GetExchangeRateRecordsAsync(startDate, endDate, userId);

				return Ok(new
				{
					code = 200,
					msg = "获取记录成功",
					data = records
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"获取记录失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 批量提交多币种汇率信息
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("SubmitMultiCurrencyExchangeRate")]
		public async Task<IActionResult> SubmitMultiCurrencyExchangeRate([FromBody] MultiCurrencyExchangeRateSubmitRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return Ok(new
					{
						code = 400,
						msg = "请求参数无效",
						data = ModelState
					});
				}

				var currentUserId = GetCurrentUserId();
				if (currentUserId <= 0)
				{
					return Ok(new
					{
						code = 401,
						msg = "用户未登录"
					});
				}

				var result = await _exchangeRateService.SubmitMultiCurrencyExchangeRateAsync(currentUserId, request);

				if (result)
				{
					return Ok(new
					{
						code = 200,
						msg = "所有汇率提交成功"
					});
				}
				else
				{
					return Ok(new
					{
						code = 500,
						msg = "汇率提交失败"
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "批量提交汇率信息失败");
				return Ok(new
				{
					code = 500,
					msg = $"提交失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 获取汇率填写统计
		/// </summary>
		/// <param name="startDate">开始日期</param>
		/// <param name="endDate">结束日期</param>
		/// <returns></returns>
		[HttpGet("GetStats")]
		public async Task<IActionResult> GetStats(DateTime startDate, DateTime endDate)
		{
			try
			{
				var stats = await _exchangeRateService.GetExchangeRateStatsAsync(startDate, endDate);

				return Ok(new
				{
					code = 200,
					msg = "获取统计成功",
					data = stats
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"获取统计失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 获取用户汇率填写状态
		/// </summary>
		/// <param name="date">查询日期，不传则查询今天</param>
		/// <returns></returns>
		[HttpGet("GetUserStatus")]
		public async Task<IActionResult> GetUserStatus(DateTime? date = null)
		{
			try
			{
				var status = await _exchangeRateService.GetUserExchangeRateStatusAsync(date);

				return Ok(new
				{
					code = 200,
					msg = "获取状态成功",
					data = status
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"获取状态失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 记录通知日志
		/// </summary>
		/// <param name="userId">用户ID</param>
		/// <param name="notificationType">通知类型</param>
		/// <returns></returns>
		[HttpPost("LogNotification")]
		public async Task<IActionResult> LogNotification(int userId, string notificationType = "popup")
		{
			try
			{
				var result = await _exchangeRateService.LogNotificationAsync(userId, notificationType);

				if (result)
				{
					return Ok(new
					{
						code = 200,
						msg = "记录通知日志成功"
					});
				}
				else
				{
					return Ok(new
					{
						code = 500,
						msg = "记录通知日志失败"
					});
				}
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"记录通知日志失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 获取当前登录用户ID
		/// </summary>
		/// <returns></returns>
		private int GetCurrentUserId()
		{
			// 这里需要根据实际的认证方式获取当前用户ID
			// 例如从JWT Token中获取，或者从Session中获取
			// 以下是示例代码，需要根据实际情况调整

			var userIdClaim = User.FindFirst("user_id");
			if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
			{
				return userId;
			}

			// 如果无法获取，返回0表示未登录
			return 0;
		}
	}

	/// <summary>
	/// 汇率填写管理控制器（管理员专用）
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class ExchangeRateManageController : ControllerBase
	{
		private readonly IExchangeRateService _exchangeRateService;

		public ExchangeRateManageController(IExchangeRateService exchangeRateService)
		{
			_exchangeRateService = exchangeRateService;
		}

		/// <summary>
		/// 获取汇率填写统计报表
		/// </summary>
		/// <param name="startDate">开始日期</param>
		/// <param name="endDate">结束日期</param>
		/// <returns></returns>
		[HttpGet("GetReport")]
		public async Task<IActionResult> GetReport(DateTime startDate, DateTime endDate)
		{
			try
			{
				var stats = await _exchangeRateService.GetExchangeRateStatsAsync(startDate, endDate);
				var userStatus = await _exchangeRateService.GetUserExchangeRateStatusAsync();

				return Ok(new
				{
					code = 200,
					msg = "获取报表成功",
					data = new
					{
						stats = stats,
						userStatus = userStatus,
						summary = new
						{
							totalDays = (endDate - startDate).Days + 1,
							totalRecords = stats.Sum(x => x.FillCount),
							avgFillRate = stats.Count > 0 ? stats.Average(x => x.FillCount) : 0
						}
					}
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"获取报表失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 导出汇率填写记录
		/// </summary>
		/// <param name="startDate">开始日期</param>
		/// <param name="endDate">结束日期</param>
		/// <returns></returns>
		[HttpGet("ExportRecords")]
		public async Task<IActionResult> ExportRecords(DateTime startDate, DateTime endDate)
		{
			try
			{
				var records = await _exchangeRateService.GetExchangeRateRecordsAsync(startDate, endDate);

				// 这里可以添加Excel导出逻辑
				// 例如使用EPPlus或NPOI库

				return Ok(new
				{
					code = 200,
					msg = "导出成功",
					data = records
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"导出失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 批量导入汇率数据
		/// </summary>
		/// <param name="records">汇率记录列表</param>
		/// <returns></returns>
		[HttpPost("ImportRecords")]
		public async Task<IActionResult> ImportRecords([FromBody] List<ExchangeRateSubmitRequest> records)
		{
			try
			{
				var currentUserId = GetCurrentUserId();
				if (currentUserId <= 0)
				{
					return Ok(new
					{
						code = 401,
						msg = "用户未登录"
					});
				}

				var successCount = 0;
				var failCount = 0;

				foreach (var record in records)
				{
					var result = await _exchangeRateService.SubmitExchangeRateAsync(currentUserId, record);
					if (result)
					{
						successCount++;
					}
					else
					{
						failCount++;
					}
				}

				return Ok(new
				{
					code = 200,
					msg = "导入完成",
					data = new
					{
						successCount = successCount,
						failCount = failCount,
						totalCount = records.Count
					}
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					code = 500,
					msg = $"导入失败: {ex.Message}"
				});
			}
		}

		/// <summary>
		/// 获取当前登录用户ID
		/// </summary>
		/// <returns></returns>
		private int GetCurrentUserId()
		{
			// 这里需要根据实际的认证方式获取当前用户ID
			var userIdClaim = User.FindFirst("user_id");
			if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
			{
				return userId;
			}
			return 0;
		}
	}
}
