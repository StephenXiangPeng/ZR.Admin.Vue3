import request from '@/utils/request'

// 查询镜片选项列表
export function listLensOptions(query) {
	return request({
		url: 'ProductConfiguration/GetList/GetList',
		method: 'get',
		params: query
	})
}

// 查询镜片选项详细
export function getLensOption(id) {
	return request({
		url: 'ProductConfiguration/GetLensOptionByID/GetLensOptionByID',
		method: 'get',
		params: { id: id }
	})
}

// 新增镜片选项
export function addLensOption(data) {
	return request({
		url: 'ProductConfiguration/AddDFGXLensOption/Add',
		method: 'post',
		data: data
	})
}

// 修改镜片选项
export function updateLensOption(data) {
	return request({
		url: 'ProductConfiguration/EditLensOptionByID/EditLensOption',
		method: 'post',
		data: data
	})
}

// 删除镜片选项
export function delLensOption(id) {
	// 构建URL参数
	const idsParam = id.map((item, index) => `ids[${index}]=${item}`).join('&')
	return request({
		url: `ProductConfiguration/DeleteLensOptionByIDs/Delete?${idsParam}`,
		method: 'delete'
	})
}

// 批量删除镜片选项
export function delLensOptions(ids) {
	console.log('API接收到的IDs:', ids) // 调试信息
	// 构建URL参数
	const idsParam = ids.map((item, index) => `ids[${index}]=${item}`).join('&')
	console.log('构建的URL参数:', idsParam) // 调试信息
	return request({
		url: `ProductConfiguration/DeleteLensOptionByIDs/Delete?${idsParam}`,
		method: 'delete'
	})
}

// 修改镜片选项状态
export function changeLensOptionStatus(id, status) {
	return request({
		url: 'ProductConfiguration/UpdateStatusByID/UpdateStatus',
		method: 'put',
		params: {
			id: id,
			status: status
		}
	})
}

// 导出镜片选项
export function exportLensOptions(query) {
	return request({
		url: '/api/ProductConfiguration/Export',
		method: 'get',
		params: query
	})
}

// 获取镜片选项下拉列表
export function getLensOptionOptions(optionType) {
	return request({
		url: '/api/ProductConfiguration/Options/' + optionType,
		method: 'get'
	})
}

// 获取下一个选项值
export function getNextOptionValue(optionType) {
	return request({
		url: 'ProductConfiguration/GetNextValue/GetNextValue',
		method: 'get',
		params: { OptionType: optionType }
	})
}

// 获取所有选项值
export function getAllLensOptions() {
	return request({
		url: 'ProductConfiguration/GetLensOptions/GetAll',
		method: 'get'
	})
}
