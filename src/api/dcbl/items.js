import request from '@/utils/request'

// 获取物料/产品列表
export function listItems(query) {
  return request({
    url: '/Items/GetItemsList/GetList',
    method: 'get',
    params: query
  })
}

// 添加物料/产品
export function addItem(data) {
  return request({
    url: '/Items/AddItem/Add',
    method: 'post',
    data
  })
}

// 编辑物料/产品
export function editItem(data) {
  return request({
    url: '/Items/EditItems/Edit',
    method: 'post',
    data
  })
}

// 删除物料/产品
export function deleteItems(ids) {
  return request({
    url: '/Items/DeleteItems/Delete',
    method: 'post',
    data: ids
  })
}
