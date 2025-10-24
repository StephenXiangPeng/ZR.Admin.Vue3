import request from '@/utils/request'

/**
 * 获取收款单位列表（基于合同状态的供应商列表）
 * @returns {Promise} 返回符合条件的供应商列表
 */
export function getSupplierList() {
	return request({
		url: 'PaymentRequest/GetSupplierList/GetSupplierList',
		method: 'GET'
	})
}

/**
 * 获取所有供应商列表（原有功能）
 * @param {Object} query 查询参数
 * @returns {Promise} 返回供应商列表
 */
export function getAllSuppliers(query) {
	return request({
		url: 'Supplierinfo/GetSupplierList/GetList',
		method: 'GET',
		params: query
	})
}

/**
 * 根据供应商ID获取供应商信息
 * @param {String} supplierId 供应商ID
 * @returns {Promise} 返回供应商信息
 */
export function getSupplierById(supplierId) {
	return request({
		url: 'Supplierinfo/GetSupplierInfoByID/GetSupplierInfo',
		method: 'GET',
		params: {
			SupplierID: supplierId
		}
	})
}

/**
 * 获取供应商银行账号列表
 * @param {String} supplierId 供应商ID
 * @returns {Promise} 返回银行账号列表
 */
export function getSupplierBankAccounts(supplierId) {
	return request({
		url: 'Supplierinfo/GetSupplierBankAccountList/GetBankAccountList',
		method: 'GET',
		params: {
			supplierId: supplierId
		}
	})
}

/**
 * 根据客户ID获取合同列表（销售合同+出运合同）
 * @param {Number} customerId 客户ID
 * @returns {Promise} 返回合同列表
 */
export function getContractListByCustomerId(customerId) {
	return request({
		url: 'CustomerInfoMation/GetContractListByCustomerID/GetContractList',
		method: 'GET',
		params: {
			CustomerID: customerId
		}
	})
}
