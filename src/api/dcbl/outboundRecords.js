import request from '@/utils/request'

// 获取物料/产品下拉框数据
export function listOutboundItemOptions() {
  return request({
    url: '/OutboundRecords/GetItemOptions/GetItemOptions',
    method: 'get'
  })
}

// 获取出库记录列表
export function listOutboundRecords(query) {
  return request({
    url: '/OutboundRecords/GetOutboundList/GetList',
    method: 'get',
    params: query
  })
}

// 获取出库记录详情
export function getOutboundRecord(id) {
  return request({
    url: '/OutboundRecords/GetInfo/GetInfo',
    method: 'get',
    params: { id }
  })
}

// 新增出库记录
export function addOutboundRecord(data) {
  return request({
    url: '/OutboundRecords/AddOutbound/Add',
    method: 'post',
    data
  })
}

// 编辑出库记录
export function editOutboundRecord(data) {
  return request({
    url: '/OutboundRecords/EditOutbound/Edit',
    method: 'post',
    data
  })
}

// 删除出库记录
export function deleteOutboundRecords(ids) {
  return request({
    url: '/OutboundRecords/DeleteOutbound/Delete',
    method: 'post',
    data: ids
  })
}
