using Microsoft.Extensions.DependencyInjection;
using ZR.Admin.Vue3.Services.ExchangeRate;

namespace ZR.Admin.Vue3.Extensions
{
	/// <summary>
	/// 服务注册扩展
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 注册汇率填写相关服务
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddExchangeRateServices(this IServiceCollection services)
		{
			// 注册汇率填写服务
			services.AddScoped<IExchangeRateService, ExchangeRateService>();

			return services;
		}
	}
}

// 在 Startup.cs 或 Program.cs 中添加以下代码：

/*
// 在 ConfigureServices 方法中添加
services.AddExchangeRateServices();

// 或者如果使用 .NET 6+ 的 Program.cs
builder.Services.AddExchangeRateServices();
*/
