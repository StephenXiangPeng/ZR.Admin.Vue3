import request from '@/utils/request'

function serializeProductIDs(productIDs) {
	const params = new URLSearchParams()
	;(productIDs || []).forEach(id => {
		if (id === null || id === undefined || id === '') return
		params.append('productIDs', String(id))
	})
	return params
}

/**
 * 工厂报价历史（询价产品历史）
 * GET /ProductInformation/GetInquiryProductHistoryListByProductIDs/GetInquiryProductHistoryList?productIDs=1&productIDs=2
 */
export function getFactoryQuotationHistory(productIDs) {
	return request({
		url: '/ProductInformation/GetInquiryProductHistoryListByProductIDs/GetInquiryProductHistoryList',
		method: 'get',
		params: serializeProductIDs(productIDs)
	})
}

/**
 * 销售历史
 * GET /ProductInformation/GetSalesHistoryList?productIDs=1&productIDs=2
 */
export function getSalesHistory(productIDs) {
	return request({
		url: '/ProductInformation/GetSalesHistoryList',
		method: 'get',
		params: serializeProductIDs(productIDs)
	})
}

/**
 * 采购历史
 * GET /ProductInformation/GetPurchaseHistoryList?productIDs=1&productIDs=2
 */
export function getPurchaseHistory(productIDs) {
	return request({
		url: '/ProductInformation/GetPurchaseHistoryList',
		method: 'get',
		params: serializeProductIDs(productIDs)
	})
}

export const getInquiryProductHistoryList = getFactoryQuotationHistory
export const getSalesHistoryList = getSalesHistory
export const getPurchaseHistoryList = getPurchaseHistory
