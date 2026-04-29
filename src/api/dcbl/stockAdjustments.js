import request from '@/utils/request'

// 获取配件/产品下拉框数据
export function listAdjustmentItemOptions() {
  return request({
    url: '/StockAdjustments/GetItemOptions/GetItemOptions',
    method: 'get'
  })
}

// 获取库存调整列表
export function listStockAdjustments(query) {
  return request({
    url: '/StockAdjustments/GetAdjustmentList/GetList',
    method: 'get',
    params: query
  })
}

// 获取库存调整详情
export function getStockAdjustment(id) {
  return request({
    url: '/StockAdjustments/GetInfo/GetInfo',
    method: 'get',
    params: { id }
  })
}

// 新增库存调整
export function addStockAdjustment(data) {
  return request({
    url: '/StockAdjustments/AddAdjustment/Add',
    method: 'post',
    data
  })
}

// 删除库存调整
export function deleteStockAdjustments(ids) {
  return request({
    url: '/StockAdjustments/DeleteAdjustment/Delete',
    method: 'post',
    data: ids
  })
}
