import request from '@/utils/request'

export function getFileList(query) {
	return request({
		url: '/api/file/list',
		method: 'get',
		params: query
	})
}

export function deleteFile(fileId) {
	return request({
		url: `/api/file/${fileId}`,
		method: 'delete'
	})
}
