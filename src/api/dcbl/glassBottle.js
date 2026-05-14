import request from '@/utils/request'

/** 分页查询玻璃瓶身列表 */
export function listGlassBottles(query) {
  return request({
    url: '/GlassBottle/GetList/GetList',
    method: 'get',
    params: query
  })
}

/** 玻璃瓶身详情 */
export function getGlassBottle(id) {
  return request({
    url: '/GlassBottle/GetInfo/GetInfo',
    method: 'get',
    params: { id }
  })
}

/** 新增玻璃瓶身 */
export function addGlassBottle(data) {
  return request({
    url: '/GlassBottle/Add/Add',
    method: 'post',
    data
  })
}

/** 编辑玻璃瓶身 */
export function editGlassBottle(data) {
  return request({
    url: '/GlassBottle/Edit/Edit',
    method: 'post',
    data
  })
}

/** 删除玻璃瓶身（逻辑删除） */
export function deleteGlassBottles(ids) {
  return request({
    url: '/GlassBottle/Delete/Delete',
    method: 'post',
    data: ids
  })
}

/** 库存变更：opType = increase | decrease | adjust */
export function changeGlassBottleStock(data) {
  return request({
    url: '/GlassBottle/ChangeStock/ChangeStock',
    method: 'post',
    data
  })
}

/** 库存流水分页 */
export function listGlassBottleStockLogs(query) {
  return request({
    url: '/GlassBottle/GetStockLogs/GetStockLogs',
    method: 'get',
    params: query
  })
}
