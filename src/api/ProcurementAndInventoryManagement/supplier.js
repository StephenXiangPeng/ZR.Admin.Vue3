import request from '@/utils/request'

/**
 * 新增供应商信息
 * @param {Object} data 供应商信息
 */
export function addSupplierInfo(data) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo/Add',
		method: 'post',
		data: data
	})
}

/**
 * 查询供应商信息列表
 * @param {Object} query 查询条件
 */
export function listSupplierInfo(query) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo/list',
		method: 'get',
		params: query
	})
}

/**
 * 获取供应商信息详情
 * @param {Number} id 供应商ID
 */
export function getSupplierInfo(id) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo/' + id,
		method: 'get'
	})
}

/**
 * 修改供应商信息
 * @param {Object} data 供应商信息
 */
export function updateSupplierInfo(data) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo',
		method: 'put',
		data: data
	})
}

/**
 * 删除供应商信息
 * @param {Number} id 供应商ID
 */
export function delSupplierInfo(id) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo/' + id,
		method: 'delete'
	})
}

/**
 * 导出供应商信息
 * @param {Object} query 查询条件
 */
export function exportSupplierInfo(query) {
	return request({
		url: '/SupplierInfomation/AddSupplierInfo/export',
		method: 'get',
		params: query
	})
}
