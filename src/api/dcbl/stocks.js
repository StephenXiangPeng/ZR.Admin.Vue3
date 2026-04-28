import request from '@/utils/request'

// 获取库存台账列表
export function listStocks(query) {
  return request({
    url: '/Stocks/GetStockList/GetList',
    method: 'get',
    params: query
  })
}

// 获取库存流水明细
export function listStockLogs(query) {
  return request({
    url: '/Stocks/GetStockLogs/GetStockLogs',
    method: 'get',
    params: query
  })
}
