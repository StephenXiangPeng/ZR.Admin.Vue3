import request from '@/utils/request'

/**
 * 获取财务总账数据
 * @param {string|null} customerID - 客户ID（可选）
 * @param {string|null} supplierID - 供应商ID（可选）
 * @returns {Promise}
 */
export function getFinancialGeneralLedgerData(customerID, supplierID) {
	const params = {}
	if (customerID) {
		params.customerID = customerID
	}
	if (supplierID) {
		params.supplierID = supplierID
	}
	return request({
		url: 'FinancialLedger/GetFinancialGeneralLedgerData/GetFinancialGeneralLedgerData',
		method: 'GET',
		params
	})
}

