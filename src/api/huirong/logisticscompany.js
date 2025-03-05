import request from '@/utils/request'

// 查询物流公司列表
export function listLogisticsCompany(query) {
	return request({
		url: 'LogisticsCompany/GetList/GetList',
		method: 'get',
		params: query
	})
}

// 查询物流公司详细
export function getLogisticsCompany(id) {
	return request({
		url: 'LogisticsCompany/GetLogisticsCompany/GetLogisticsCompany',
		method: 'get',
		params: { id }
	})
}

// 新增物流公司
export function addLogisticsCompany(data) {
	return request({
		url: 'LogisticsCompany/AddLogisticsCompany/AddLogisticsCompany',
		method: 'post',
		data: data
	})
}

// 修改物流公司
export function updateLogisticsCompany(data) {
	return request({
		url: 'LogisticsCompany/UpdateLogisticsCompany/Update',
		method: 'post',
		data: data
	})
}

// 删除物流公司
export function delLogisticsCompany(id) {
	return request({
		url: 'LogisticsCompany/DelLogisticsCompany/Del',
		method: 'get',
		params: { id }
	})
}

// 导出物流公司
export function exportLogisticsCompany(query) {
	return request({
		url: '/huirong/logisticscompany/export',
		method: 'get',
		params: query
	})
}

// 获取物流公司联系人列表
export function getLogisticsCompanyContacts(logisticsCompanyId) {
	return request({
		url: 'LogisticsCompany/GetLogistictCompanyContactList/GetCompanyContact/',
		method: 'get',
		params: { logisticsCompanyId }
	})
}