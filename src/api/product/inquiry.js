import request from '@/utils/request'

/**
 * 根据产品ID集合获取询价产品历史记录
 * @param {Array} productIDs 产品ID集合
 * @returns {Promise}
 */
export function getInquiryProductHistoryList(productIDs) {
	// 构建查询字符串，避免axios自动添加[]
	const queryString = productIDs.map(id => `productIDs=${id}`).join('&');
	return request({
		url: `/ProductInformation/GetInquiryProductHistoryListByProductIDs/GetInquiryProductHistoryList?${queryString}`,
		method: 'get'
	})
}
