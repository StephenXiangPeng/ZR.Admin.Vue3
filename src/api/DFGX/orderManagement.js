import request from '@/utils/request'

// 获取订单完整信息
export function getCompleteOrder(orderId) {
	return request({
		url: `/dfgx/order-management/complete/${orderId}`,
		method: 'get'
	})
}

// 创建完整订单
export function createCompleteOrder(data) {
	return request({
		url: '/dfgx/order-management/create-complete',
		method: 'post',
		data: data,
		headers: {
			'Content-Type': 'application/json'
		}
	})
}

// 更新订单状态并记录历史
export function updateOrderStatusWithHistory(orderId, status, operator) {
	return request({
		url: `/dfgx/order-management/update-status/${orderId}`,
		method: 'put',
		params: {
			status: status,
			operator: operator
		}
	})
}

// 删除完整订单
export function delCompleteOrder(orderId) {
	return request({
		url: `/dfgx/order-management/complete/${orderId}`,
		method: 'delete'
	})
}

// 获取订单统计信息
export function getOrderStatistics(beginTime, endTime) {
	return request({
		url: '/dfgx/order-management/statistics',
		method: 'get',
		params: {
			beginTime: beginTime,
			endTime: endTime
		}
	})
}
