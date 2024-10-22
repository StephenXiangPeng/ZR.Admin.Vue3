// fetchcontractdata.ts
import { ref, onMounted } from 'vue';
import request from '@/utils/request';

// 定义你获取的数据结构类型
interface Data {
	name: string;
	age: number;
	// 根据实际数据结构定义更多字段
}

export function useFetchData(id: number) {
	// 使用类型定义状态
	const data = ref<Data | null>(null);
	const loading = ref(true);
	const error = ref<Error | null>(null);
	const fetchData = async () => {
		try {
			loading.value = true;
			request({
				url: 'Contracts/GetContractDetailsById/GetContractDetails',
				method: 'GET',
				params: {
					contractId: id
				}
			}).then(response => {
				data.value = response.data;
			}).catch(error => {
				console.error(error);
			});
		} catch (err) {
			error.value = err;
		} finally {
			loading.value = false;
		}
	};

	// onMounted(() => {
	// 	fetchData();
	// });

	return { data, loading, error };
}
