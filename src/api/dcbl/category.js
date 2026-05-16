import request from '@/utils/request'

/** 零配件分类类型（与配件 Items.Type = 1 对应） */
export const SPARE_PART_CATEGORY_TYPE = 1

export function listCategory(query) {
  return request({
    url: '/Category/GetCategoryList/GetList',
    method: 'get',
    params: query
  })
}

export function listCategoryOptions(query) {
  return request({
    url: '/Category/GetCategoryOptions/GetOptions',
    method: 'get',
    params: query
  })
}

export function addCategory(data) {
  return request({
    url: '/Category/AddCategory/Add',
    method: 'post',
    data
  })
}

export function editCategory(data) {
  return request({
    url: '/Category/EditCategory/Edit',
    method: 'post',
    data
  })
}

export function deleteCategory(ids) {
  return request({
    url: '/Category/DeleteCategory/Delete',
    method: 'post',
    data: ids
  })
}
