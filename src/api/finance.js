import request from '@/utils/request'

/**
 * 获取财务总账数据
 * @param {string} customerID - 客户ID（可选）
 * @returns {Promise}
 */
export function getFinancialGeneralLedgerData(customerID) {
	return request({
		url: 'FinancialLedger/GetFinancialGeneralLedgerData/GetFinancialGeneralLedgerData',
		method: 'GET',
		params: {
			customerID: customerID || ''
		}
	})
}

