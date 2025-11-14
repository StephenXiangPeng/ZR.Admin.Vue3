import request from '@/utils/request'

// 获取指定类型的选项列表
export function getLensOptionsByType(optionType) {
	return request({
		url: 'ProductConfiguration/GetLensOptionsByTypeList/GetLensOptionsByType',
		method: 'get',
		params: { optionType }
	})
}

// 获取折射率选项的详细信息（包含关联材质）
export function getLensOptionById(id) {
	return request({
		url: 'ProductConfiguration/GetLensOptionByID/GetLensOptionByID',
		method: 'get',
		params: { id }
	})
}

// 获取折射率选项
export function getRefractionOptions() {
	return getLensOptionsByType(5) // optionType = 5 对应折射率
}

// 获取材质选项
export function getMaterialOptions() {
	return getLensOptionsByType(3) // optionType = 3 对应材质
}

// 获取设计选项
export function getDesignOptions() {
	return getLensOptionsByType(4) // optionType = 4 对应设计名称
}

// 获取膜层选项
export function getCoatingOptions() {
	return getLensOptionsByType(2) // optionType = 2 对应膜层
}

// 获取额外处理选项
export function getExtraOptions() {
	return getLensOptionsByType(17) // optionType = 6 对应额外处理
}

// 获取指定折射率关联的材质列表
export function getRelatedMaterials(relatedId) {
	return request({
		url: 'ProductConfiguration/GetRelatedMaterialsList/GetRelatedMaterials',
		method: 'get',
		params: { RelatedID: relatedId }
	})
}

// 根据折射率获取关联材质
export async function getMaterialsByRefraction(refractionId) {
	try {
		// 使用专门的关联材质接口
		const relatedResponse = await getRelatedMaterials(refractionId)
		const relatedMaterials = relatedResponse.data || []

		if (relatedMaterials.length === 0) {
			// 如果没有关联材质，返回所有材质选项
			const materialsResponse = await getMaterialOptions()
			return materialsResponse.data.map(item => ({
				id: item.id,
				option_name: item.optionName,
				is_default: 0
			}))
		}

		// 获取所有材质选项用于匹配
		const allMaterials = await getMaterialOptions()
		const materialMap = new Map(allMaterials.data.map(item => [item.id, item]))

		// 根据关联材质数据构建结果
		return relatedMaterials.map(related => {
			const material = materialMap.get(related.material_id)
			return {
				id: related.material_id,
				option_name: material ? material.optionName : `材质ID: ${related.material_id}`,
				is_default: related.is_default || 0
			}
		})
	} catch (error) {
		console.error('获取关联材质失败:', error)
		throw error
	}
}

// ========== 品种价格管理 ==========

// 获取品种价格列表
export function getVarietyPriceList(params) {
	return request({
		url: 'DFGXPriceManager/GetlensvarietypriceList/GetList',
		method: 'get',
		params: {
			pageIndex: params.pageIndex || 1,
			pageSize: params.pageSize || 10,
			RefractiveIndex: params.RefractiveIndex || 0
		}
	})
}

// 新增品种价格
export function addVarietyPrice(data) {
	return request({
		url: 'DFGXPriceManager/AddVarietyPrice/Add',
		method: 'post',
		data: data
	})
}

// 编辑品种价格
export function updateVarietyPrice(data) {
	return request({
		url: 'DFGXPriceManager/UpdateVarietyPrice/Edit',
		method: 'put',
		data: data
	})
}

// 删除品种价格
export function deleteVarietyPrice(id) {
	return request({
		url: `DFGXPriceManager/DeleteVarietyPrice/${id}`,
		method: 'delete'
	})
}

// 批量删除品种价格
export function batchDeleteVarietyPrice(ids) {
	return request({
		url: 'DFGXPriceManager/BatchDeleteVarietyPrice',
		method: 'delete',
		data: { ids }
	})
}

// 获取品种价格详情
export function getVarietyPriceById(id) {
	return request({
		url: `DFGXPriceManager/GetVarietyPriceById/${id}`,
		method: 'get'
	})
}

// ========== 附加价管理 ==========

// 获取附加价列表
export function getSurchargeList(type, pageIndex = 1, pageSize = 10) {
	return request({
		url: 'DFGXPriceManager/GetlenssurchargeList/GetList',
		method: 'get',
		params: {
			type,
			pageIndex,
			pageSize
		}
	})
}

// 新增附加价
export function addSurcharge(data) {
	return request({
		url: 'DFGXPriceManager/AddSurcharge/Add',
		method: 'post',
		data: data
	})
}

// 编辑附加价
export function updateSurcharge(data) {
	return request({
		url: 'DFGXPriceManager/UpdateSurcharge/Edit',
		method: 'put',
		data: data
	})
}

// 删除附加价
export function deleteSurcharge(id) {
	return request({
		url: `DFGXPriceManager/DeleteSurcharge/${id}`,
		method: 'delete'
	})
}

// 批量删除附加价
export function batchDeleteSurcharge(ids) {
	return request({
		url: 'DFGXPriceManager/BatchDeleteSurcharge',
		method: 'delete',
		data: { ids }
	})
}

// 获取附加价详情
export function getSurchargeById(id) {
	return request({
		url: `DFGXPriceManager/GetSurchargeById/${id}`,
		method: 'get'
	})
}

// 更新附加价状态
export function updateSurchargeStatus(id, status) {
	return request({
		url: `DFGXPriceManager/UpdateSurchargeStatus/${id}`,
		method: 'put',
		data: { status }
	})
}

// ========== 客户管理 ==========

// 获取客户用户列表
export function getCustomerUserList() {
	return request({
		url: 'system/user/GetCustomerUserList',
		method: 'get'
	})
}

// ========== Excel导入相关 ==========

// 品种价格Excel导入
export function importVarietyPrice(file, customerId = null) {
	const formData = new FormData()
	formData.append('file', file)

	// 构建查询参数
	let url = 'DFGXPriceManager/ImportVarietyPrice/ImportVarietyPrice'
	return request({
		url: url,
		method: 'post',
		data: formData,
		headers: {
			'Content-Type': 'multipart/form-data'
		}
	})
}

// ========== 下拉框联动相关 ==========

// 根据折射率ID获取材质选项
export function getMaterialByIndexID(IndexID) {
	return request({
		url: 'ProductConfiguration/GetMaterialByIndexID',
		method: 'get',
		params: { IndexID }
	})
}

// 根据材质ID获取设计选项
export function getDesignByMaterialID(MaterialID) {
	return request({
		url: 'ProductConfiguration/GetDesignByMaterialID',
		method: 'get',
		params: { MaterialID }
	})
}

// 根据设计ID获取镜片型号选项（膜层）
export function getModelByDesignID(designID) {
	return request({
		url: 'ProductConfiguration/GetModelByDesignID',
		method: 'get',
		params: { designID }
	})
}


