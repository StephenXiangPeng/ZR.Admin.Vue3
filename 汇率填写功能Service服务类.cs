using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZR.Admin.Vue3.Models.ExchangeRate;

namespace ZR.Admin.Vue3.Services.ExchangeRate
{
	/// <summary>
	/// 汇率填写服务接口
	/// </summary>
	public interface IExchangeRateService
	{
		/// <summary>
		/// 获取汇率更新任务配置
		/// </summary>
		/// <returns></returns>
		Task<ExchangeRateTaskConfig> GetTaskConfigAsync();

		/// <summary>
		/// 保存汇率更新任务配置
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<bool> SaveTaskConfigAsync(ExchangeRateTaskConfigRequest request);

		/// <summary>
		/// 检查用户是否需要填写汇率
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<UserExchangeRateCheckResponse> CheckUserNeedFillAsync(int userId);

		/// <summary>
		/// 提交汇率信息
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<bool> SubmitExchangeRateAsync(int userId, ExchangeRateSubmitRequest request);

		/// <summary>
		/// 批量提交多币种汇率信息
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		Task<bool> SubmitMultiCurrencyExchangeRateAsync(int userId, MultiCurrencyExchangeRateSubmitRequest request);

		/// <summary>
		/// 获取汇率填写记录
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<List<ExchangeRateRecord>> GetExchangeRateRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, int? userId = null);

		/// <summary>
		/// 获取汇率填写统计
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		Task<List<ExchangeRateStatsViewModel>> GetExchangeRateStatsAsync(DateTime startDate, DateTime endDate);

		/// <summary>
		/// 获取用户汇率填写状态
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		Task<List<UserExchangeRateStatusViewModel>> GetUserExchangeRateStatusAsync(DateTime? date = null);

		/// <summary>
		/// 记录通知日志
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="notificationType"></param>
		/// <returns></returns>
		Task<bool> LogNotificationAsync(int userId, string notificationType = "popup");
	}

	/// <summary>
	/// 汇率填写服务实现
	/// </summary>
	public class ExchangeRateService : IExchangeRateService
	{
		private readonly ISqlSugarClient _db;

		public ExchangeRateService(ISqlSugarClient db)
		{
			_db = db;
		}

		/// <summary>
		/// 获取汇率更新任务配置
		/// </summary>
		/// <returns></returns>
		public async Task<ExchangeRateTaskConfig> GetTaskConfigAsync()
		{
			var config = await _db.Queryable<ExchangeRateTaskConfig>()
				.FirstAsync();

			if (config == null)
			{
				// 如果没有配置，创建默认配置
				config = new ExchangeRateTaskConfig
				{
					TaskName = "每日汇率更新任务",
					Executors = "1", // 默认管理员ID
					NotificationInterval = 5,
					Status = 1,
					Description = "每日提醒指定人员填写当天汇率，如未填写则每隔指定时间弹窗提醒",
					CreateBy = "system",
					CreateTime = DateTime.Now,
					UpdateTime = DateTime.Now
				};

				await _db.Insertable(config).ExecuteCommandAsync();
			}

			return config;
		}

		/// <summary>
		/// 保存汇率更新任务配置
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<bool> SaveTaskConfigAsync(ExchangeRateTaskConfigRequest request)
		{
			try
			{
				var config = await _db.Queryable<ExchangeRateTaskConfig>().FirstAsync();

				if (config == null)
				{
					config = new ExchangeRateTaskConfig();
				}

				config.TaskName = request.TaskName;
				config.ExecutorList = request.Executors;
				config.NotificationInterval = request.NotificationInterval;
				config.Status = request.Status;
				config.Description = request.Description;
				config.UpdateTime = DateTime.Now;

				if (config.Id == 0)
				{
					config.CreateTime = DateTime.Now;
					await _db.Insertable(config).ExecuteCommandAsync();
				}
				else
				{
					await _db.Updateable(config).ExecuteCommandAsync();
				}

				return true;
			}
			catch (Exception ex)
			{
				// 记录日志
				Console.WriteLine($"保存汇率任务配置失败: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// 检查用户是否需要填写汇率
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<UserExchangeRateCheckResponse> CheckUserNeedFillAsync(int userId)
		{
			var response = new UserExchangeRateCheckResponse
			{
				UserId = userId,
				NeedFill = false,
				IsExecutor = false,
				HasFilledToday = false,
				TaskEnabled = false
			};

			try
			{
				// 获取任务配置
				var config = await GetTaskConfigAsync();
				response.TaskEnabled = config.Status == 1;

				if (!response.TaskEnabled)
				{
					return response;
				}

				// 检查用户是否在执行人员列表中
				var executorList = config.ExecutorList;
				response.IsExecutor = executorList.Contains(userId);

				if (!response.IsExecutor)
				{
					return response;
				}

				// 检查用户今天是否已经填写过
				var today = DateTime.Today;
				var hasFilled = await _db.Queryable<ExchangeRateRecord>()
					.Where(x => x.Executor == userId && x.Date.Date == today)
					.AnyAsync();

				response.HasFilledToday = hasFilled;
				response.NeedFill = !hasFilled;

				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"检查用户汇率填写需求失败: {ex.Message}");
				return response;
			}
		}

		/// <summary>
		/// 提交汇率信息
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<bool> SubmitExchangeRateAsync(int userId, ExchangeRateSubmitRequest request)
		{
			try
			{
				// 检查是否已经填写过
				var existingRecord = await _db.Queryable<ExchangeRateRecord>()
					.Where(x => x.Executor == userId && x.Date.Date == request.Date.Date && x.Currency == request.Currency)
					.FirstAsync();

				if (existingRecord != null)
				{
					// 更新现有记录
					existingRecord.ExchangeRate = request.ExchangeRate;
					existingRecord.Remark = request.Remark;
					existingRecord.UpdateTime = DateTime.Now;
					await _db.Updateable(existingRecord).ExecuteCommandAsync();
				}
				else
				{
					// 创建新记录
					var record = new ExchangeRateRecord
					{
						Date = request.Date.Date,
						Currency = request.Currency,
						ExchangeRate = request.ExchangeRate,
						Executor = userId,
						Remark = request.Remark,
						CreateTime = DateTime.Now,
						UpdateTime = DateTime.Now
					};

					await _db.Insertable(record).ExecuteCommandAsync();
				}

				// 更新通知日志
				await _db.Updateable<ExchangeRateNotificationLog>()
					.SetColumns(x => new ExchangeRateNotificationLog
					{
						IsFilled = true,
						FillTime = DateTime.Now
					})
					.Where(x => x.UserId == userId && x.CreateTime.Date == request.Date.Date)
					.ExecuteCommandAsync();

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"提交汇率信息失败: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// 批量提交多币种汇率信息
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<bool> SubmitMultiCurrencyExchangeRateAsync(int userId, MultiCurrencyExchangeRateSubmitRequest request)
		{
			try
			{
				// 使用事务处理批量提交
				await _db.Ado.BeginTranAsync();
				try
				{
					var successCount = 0;

					foreach (var rateItem in request.ExchangeRates)
					{
						// 检查是否已经填写过
						var existingRecord = await _db.Queryable<ExchangeRateRecord>()
							.Where(x => x.Executor == userId && x.Date.Date == request.Date.Date && x.Currency == rateItem.Currency)
							.FirstAsync();

						if (existingRecord != null)
						{
							// 更新现有记录
							existingRecord.ExchangeRate = rateItem.ExchangeRate;
							existingRecord.Remark = rateItem.Remark;
							existingRecord.UpdateTime = DateTime.Now;
							await _db.Updateable(existingRecord).ExecuteCommandAsync();
						}
						else
						{
							// 创建新记录
							var record = new ExchangeRateRecord
							{
								Date = request.Date.Date,
								Currency = rateItem.Currency,
								ExchangeRate = rateItem.ExchangeRate,
								Executor = userId,
								Remark = rateItem.Remark,
								CreateTime = DateTime.Now,
								UpdateTime = DateTime.Now
							};

							await _db.Insertable(record).ExecuteCommandAsync();
						}

						successCount++;
					}

					// 更新通知日志
					await _db.Updateable<ExchangeRateNotificationLog>()
						.SetColumns(x => new ExchangeRateNotificationLog
						{
							IsFilled = true,
							FillTime = DateTime.Now
						})
						.Where(x => x.UserId == userId && x.CreateTime.Date == request.Date.Date)
						.ExecuteCommandAsync();

					await _db.Ado.CommitTranAsync();
					Console.WriteLine($"用户 {userId} 成功批量提交 {successCount} 个币种汇率");
					return true;
				}
				catch
				{
					await _db.Ado.RollbackTranAsync();
					throw;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"批量提交汇率信息失败: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// 获取汇率填写记录
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<ExchangeRateRecord>> GetExchangeRateRecordsAsync(DateTime? startDate = null, DateTime? endDate = null, int? userId = null)
		{
			var query = _db.Queryable<ExchangeRateRecord>();

			if (startDate.HasValue)
			{
				query = query.Where(x => x.Date >= startDate.Value);
			}

			if (endDate.HasValue)
			{
				query = query.Where(x => x.Date <= endDate.Value);
			}

			if (userId.HasValue)
			{
				query = query.Where(x => x.Executor == userId.Value);
			}

			return await query.OrderByDescending(x => x.Date)
				.ThenByDescending(x => x.CreateTime)
				.ToListAsync();
		}

		/// <summary>
		/// 获取汇率填写统计
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public async Task<List<ExchangeRateStatsViewModel>> GetExchangeRateStatsAsync(DateTime startDate, DateTime endDate)
		{
			var stats = await _db.Queryable<ExchangeRateRecord>()
				.Where(x => x.Date >= startDate && x.Date <= endDate)
				.GroupBy(x => new { x.Date, x.Currency })
				.Select(x => new ExchangeRateStatsViewModel
				{
					Date = x.Date,
					Currency = x.Currency,
					FillCount = SqlFunc.AggregateCount(x.Executor),
					AvgRate = SqlFunc.AggregateAvg(x.ExchangeRate),
					MinRate = SqlFunc.AggregateMin(x.ExchangeRate),
					MaxRate = SqlFunc.AggregateMax(x.ExchangeRate),
					Executors = SqlFunc.AggregateGroupConcat(x.Executor)
				})
				.OrderByDescending(x => x.Date)
				.ThenBy(x => x.Currency)
				.ToListAsync();

			return stats;
		}

		/// <summary>
		/// 获取用户汇率填写状态
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public async Task<List<UserExchangeRateStatusViewModel>> GetUserExchangeRateStatusAsync(DateTime? date = null)
		{
			var checkDate = date ?? DateTime.Today;

			// 获取任务配置中的执行人员
			var config = await GetTaskConfigAsync();
			var executorIds = config.ExecutorList;

			if (executorIds == null || executorIds.Length == 0)
			{
				return new List<UserExchangeRateStatusViewModel>();
			}

			// 获取用户信息（这里需要根据实际的用户表结构调整）
			var users = await _db.Queryable<object>() // 替换为实际的用户表
				.Where($"user_id IN ({string.Join(",", executorIds)})")
				.Select("user_id as UserId, user_name as UserName, dept_id as DeptId, dept_name as DeptName")
				.ToListAsync();

			var result = new List<UserExchangeRateStatusViewModel>();

			foreach (var user in users)
			{
				var userId = (int)user.GetType().GetProperty("UserId").GetValue(user);
				var userName = user.GetType().GetProperty("UserName").GetValue(user)?.ToString();
				var deptId = (int)user.GetType().GetProperty("DeptId").GetValue(user);
				var deptName = user.GetType().GetProperty("DeptName").GetValue(user)?.ToString();

				// 检查是否已填写
				var record = await _db.Queryable<ExchangeRateRecord>()
					.Where(x => x.Executor == userId && x.Date.Date == checkDate.Date)
					.FirstAsync();

				result.Add(new UserExchangeRateStatusViewModel
				{
					UserId = userId,
					UserName = userName,
					DeptId = deptId,
					DeptName = deptName,
					FillStatus = record != null ? "已填写" : "未填写",
					ExchangeRate = record?.ExchangeRate,
					FillTime = record?.CreateTime,
					Remark = record?.Remark
				});
			}

			return result.OrderBy(x => x.UserName).ToList();
		}

		/// <summary>
		/// 记录通知日志
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="notificationType"></param>
		/// <returns></returns>
		public async Task<bool> LogNotificationAsync(int userId, string notificationType = "popup")
		{
			try
			{
				var log = new ExchangeRateNotificationLog
				{
					UserId = userId,
					NotificationType = notificationType,
					NotificationTime = DateTime.Now,
					IsRead = false,
					IsFilled = false,
					CreateTime = DateTime.Now
				};

				await _db.Insertable(log).ExecuteCommandAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"记录通知日志失败: {ex.Message}");
				return false;
			}
		}
	}
}
