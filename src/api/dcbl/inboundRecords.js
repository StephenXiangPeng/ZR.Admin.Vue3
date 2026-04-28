import request from '@/utils/request'

// 获取物料/产品下拉框数据
export function listInboundItemOptions() {
  return request({
    url: '/InboundRecords/GetItemOptions/GetItemOptions',
    method: 'get'
  })
}

// 获取入库记录列表
export function listInboundRecords(query) {
  return request({
    url: '/InboundRecords/GetInboundList/GetList',
    method: 'get',
    params: query
  })
}

// 获取入库记录详情
export function getInboundRecord(id) {
  return request({
    url: '/InboundRecords/GetInfo/GetInfo',
    method: 'get',
    params: { id }
  })
}

// 新增入库记录
export function addInboundRecord(data) {
  return request({
    url: '/InboundRecords/AddInbound/Add',
    method: 'post',
    data
  })
}

// 编辑入库记录
export function editInboundRecord(data) {
  return request({
    url: '/InboundRecords/EditInbound/Edit',
    method: 'post',
    data
  })
}

// 删除入库记录
export function deleteInboundRecords(ids) {
  return request({
    url: '/InboundRecords/DeleteInbound/Delete',
    method: 'post',
    data: ids
  })
}
