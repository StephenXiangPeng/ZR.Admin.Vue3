import request from '@/utils/request'

/**
 * 运行MRP
 * @param {Object} data MRP运行参数
 */
export function runMrp(data) {
	return request({
		url: '/Mrp/Run/Run',
		method: 'post'
	})
}

/**
 * 获取MRP结果
 * @param {Object} query 查询条件
 */
export function getMrpResult(query) {
	return request({
		url: '/Mrp/Result/Result',
		method: 'get',
		params: query
	})
}

/**
 * 获取MRP详情
 * @param {Number} id MRP记录ID
 */
export function getMrpDetail(id) {
	return request({
		url: '/Mrp/Result/' + id,
		method: 'get'
	})
}

/**
 * 删除MRP记录
 * @param {Array} ids MRP记录ID集合
 */
export function delMrpRecord(ids) {
	return request({
		url: '/Mrp/Result/Delete',
		method: 'delete',
		data: ids
	})
}

/**
 * 导出MRP结果
 * @param {Object} query 查询条件
 */
export function exportMrpResult(query) {
	return request({
		url: '/Mrp/Result/export',
		method: 'get',
		params: query
	})
}
