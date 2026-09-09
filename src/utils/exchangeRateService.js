import request from '@/utils/request'

/**
 * 汇率服务类
 * 提供获取最新汇率的通用方法
 */
class ExchangeRateService {
	/**
	 * 获取指定币种的最新汇率
	 * @param {string} currency - 币种ID (1=美元, 2=欧元, 3=人民币)
	 * @param {Date} date - 指定日期，不传则使用今天
	 * @returns {Promise<number|null>} 汇率值，如果找不到则返回null
	 */
	async getLatestExchangeRate(currency, date = null) {
		try {
			const params = { currency }
			if (date) {
				params.date = date.toISOString().split('T')[0]
			}

			const response = await request.get('ExchangeRateTask/GetLatestExchangeRate/GetLatestExchangeRate', { params })

			if (response.code === 200 && response.data !== null) {
				return parseFloat(response.data)
			}

			return null
		} catch (error) {
			console.error('获取最新汇率失败:', error)
			return null
		}
	}

	/**
	 * 获取所有币种的最新汇率
	 * @param {Date} date - 指定日期，不传则使用今天
	 * @returns {Promise<Object>} 汇率字典 {currency: rate}
	 */
	async getAllLatestExchangeRates(date = null, includeDates = false) {
		try {
			const params = includeDates ? { includeDates: true } : {}
			if (date) {
				params.date = date.toISOString().split('T')[0]
			}

			const response = await request.get('ExchangeRateTask/GetAllLatestExchangeRates/GetAllLatestExchangeRates', { params })

			if (response.code === 200 && response.data) {
				return response.data
			}

			return {}
		} catch (error) {
			console.error('获取所有最新汇率失败:', error)
			return {}
		}
	}

	/**
	 * 根据币种ID获取币种名称
	 * @param {string} currencyId - 币种ID
	 * @param {Array} currencyOptions - 币种选项数组
	 * @returns {string} 币种名称
	 */
	getCurrencyName(currencyId, currencyOptions) {
		if (!currencyOptions || !currencyId) {
			return '未知币种'
		}

		const currency = currencyOptions.find(item => item.dictValue === currencyId)
		return currency ? currency.dictLabel : '未知币种'
	}

	/**
	 * 获取默认汇率（用于人民币等特殊币种）
	 * @param {string} currencyId - 币种ID
	 * @returns {number} 默认汇率
	 */
	getDefaultExchangeRate(currencyId) {
		switch (currencyId) {
			case '3': // 人民币
				return 1.000
			case '1': // 美元
				return 7.200
			case '2': // 欧元
				return 7.800
			default:
				return 1.000
		}
	}

	/**
	 * 格式化汇率显示
	 * @param {number} rate - 汇率值
	 * @param {number} precision - 小数位数，默认4位
	 * @returns {string} 格式化后的汇率字符串
	 */
	formatExchangeRate(rate, precision = 4) {
		if (rate === null || rate === undefined) {
			return ''
		}
		return parseFloat(rate).toFixed(precision)
	}
}

// 创建单例实例
const exchangeRateService = new ExchangeRateService()

export default exchangeRateService
