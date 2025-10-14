import request from '@/utils/request'

// 分页查询客户订单列表
export function listCustomerOrders(query) {
	return request({
		url: '/dfgx/customer-orders/list',
		method: 'get',
		params: query
	})
}

// 获取订单详情
export function getCustomerOrder(orderId) {
	return request({
		url: `/dfgx/customer-orders/${orderId}`,
		method: 'get'
	})
}

// 根据订单编号获取订单
export function getCustomerOrderByOrderNo(orderNo) {
	return request({
		url: `/dfgx/customer-orders/by-order-no/${orderNo}`,
		method: 'get'
	})
}

// 新增客户订单
export function addCustomerOrder(data) {
	return request({
		url: '/dfgx/customer-orders',
		method: 'post',
		data: data
	})
}

// 更新客户订单
export function updateCustomerOrder(data) {
	return request({
		url: '/dfgx/customer-orders',
		method: 'put',
		data: data
	})
}

// 删除客户订单
export function delCustomerOrder(orderId) {
	return request({
		url: `/dfgx/customer-orders/${orderId}`,
		method: 'delete'
	})
}

// 批量删除客户订单
export function delCustomerOrders(ids) {
	return request({
		url: '/dfgx/customer-orders/batch',
		method: 'delete',
		data: ids
	})
}

// 更新订单状态
export function updateOrderStatus(orderId, status, operator) {
	return request({
		url: `/dfgx/customer-orders/${orderId}/status`,
		method: 'put',
		params: {
			status: status,
			operator: operator
		}
	})
}

// 校验订单编号唯一性
export function checkOrderNoUnique(orderNo, orderId) {
	return request({
		url: '/dfgx/customer-orders/check-order-no',
		method: 'get',
		params: {
			orderNo: orderNo,
			orderId: orderId
		}
	})
}

// 导出客户订单
export function exportCustomerOrders(query) {
	return request({
		url: '/dfgx/customer-orders/export',
		method: 'get',
		params: query
	})
}
