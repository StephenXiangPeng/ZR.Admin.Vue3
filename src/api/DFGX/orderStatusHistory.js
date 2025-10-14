import request from '@/utils/request'

// 分页查询订单状态历史列表
export function listOrderStatusHistory(query) {
	return request({
		url: '/dfgx/order-status-history/list',
		method: 'get',
		params: query
	})
}

// 获取状态历史详情
export function getOrderStatusHistory(historyId) {
	return request({
		url: `/dfgx/order-status-history/${historyId}`,
		method: 'get'
	})
}

// 根据订单ID获取状态历史列表
export function getOrderStatusHistoryByOrderId(orderId) {
	return request({
		url: `/dfgx/order-status-history/by-order/${orderId}`,
		method: 'get'
	})
}

// 获取订单最新状态历史
export function getLatestOrderStatusHistory(orderId) {
	return request({
		url: `/dfgx/order-status-history/latest/${orderId}`,
		method: 'get'
	})
}

// 新增订单状态历史
export function addOrderStatusHistory(data) {
	return request({
		url: '/dfgx/order-status-history',
		method: 'post',
		data: data
	})
}

// 批量新增订单状态历史
export function addOrderStatusHistoryBatch(data) {
	return request({
		url: '/dfgx/order-status-history/batch',
		method: 'post',
		data: data
	})
}

// 更新订单状态历史
export function updateOrderStatusHistory(data) {
	return request({
		url: '/dfgx/order-status-history',
		method: 'put',
		data: data
	})
}

// 删除订单状态历史
export function delOrderStatusHistory(historyId) {
	return request({
		url: `/dfgx/order-status-history/${historyId}`,
		method: 'delete'
	})
}

// 批量删除订单状态历史
export function delOrderStatusHistoryBatch(ids) {
	return request({
		url: '/dfgx/order-status-history/batch',
		method: 'delete',
		data: ids
	})
}

// 根据订单ID删除所有状态历史
export function delOrderStatusHistoryByOrderId(orderId) {
	return request({
		url: `/dfgx/order-status-history/by-order/${orderId}`,
		method: 'delete'
	})
}

// 记录订单状态变更
export function recordOrderStatusChange(orderId, status, statusName, operator) {
	return request({
		url: '/dfgx/order-status-history/record-change',
		method: 'post',
		params: {
			orderId: orderId,
			status: status,
			statusName: statusName,
			operator: operator
		}
	})
}

// 导出订单状态历史
export function exportOrderStatusHistory(query) {
	return request({
		url: '/dfgx/order-status-history/export',
		method: 'get',
		params: query
	})
}
