import request from '@/utils/request'

// 分页查询订单详情列表
export function listOrderDetails(query) {
	return request({
		url: '/dfgx/order-details/list',
		method: 'get',
		params: query
	})
}

// 获取订单详情
export function getOrderDetail(detailId) {
	return request({
		url: `/dfgx/order-details/${detailId}`,
		method: 'get'
	})
}

// 根据订单ID获取详情列表
export function getOrderDetailsByOrderId(orderId) {
	return request({
		url: `/dfgx/order-details/by-order/${orderId}`,
		method: 'get'
	})
}

// 新增订单详情
export function addOrderDetail(data) {
	return request({
		url: '/dfgx/order-details',
		method: 'post',
		data: data
	})
}

// 批量新增订单详情
export function addOrderDetails(data) {
	return request({
		url: '/dfgx/order-details/batch',
		method: 'post',
		data: data
	})
}

// 更新订单详情
export function updateOrderDetail(data) {
	return request({
		url: '/dfgx/order-details',
		method: 'put',
		data: data
	})
}

// 删除订单详情
export function delOrderDetail(detailId) {
	return request({
		url: `/dfgx/order-details/${detailId}`,
		method: 'delete'
	})
}

// 批量删除订单详情
export function delOrderDetails(ids) {
	return request({
		url: '/dfgx/order-details/batch',
		method: 'delete',
		data: ids
	})
}

// 根据订单ID删除所有详情
export function delOrderDetailsByOrderId(orderId) {
	return request({
		url: `/dfgx/order-details/by-order/${orderId}`,
		method: 'delete'
	})
}

// 计算订单详情总金额
export function calculateOrderTotal(orderId) {
	return request({
		url: `/dfgx/order-details/calculate-total/${orderId}`,
		method: 'get'
	})
}

// 导出订单详情
export function exportOrderDetails(query) {
	return request({
		url: '/dfgx/order-details/export',
		method: 'get',
		params: query
	})
}
