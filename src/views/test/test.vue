<template>
	<div>
		<h1>测试获取到的数量:{{ testCount }}</h1>
		<el-button @click="test1">测试</el-button>
	</div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { onMounted } from 'vue'
import useSocketStore from '@/store/modules/socket'
import request from '@/utils/request';
import { HubConnectionState } from '@microsoft/signalr'
import signalR from '@/signalr/signalr'

const socketStore = useSocketStore()
const testCount = ref(0)
// 监听任务数量变化
onMounted(() => {
	watch(() => socketStore.taskStats, (val) => {
		console.log('接收到任务数量更新', val)
		testCount.value = val.todo // 或者你想显示的字段
	}, { immediate: true })
})
const refTestCount = ref(0)
const test1 = async () => {
	if (signalR.SR.state !== HubConnectionState.Connected) {
		console.warn('[SignalR] 未连接，尝试连接...')
		await signalR.start()
		console.log('[SignalR]连接状态', signalR.SR.state)
	}
	refTestCount.value++;
	const res = await request({
		url: '/Test/SumbitTest/Test',
		method: 'get',
		params: {
			TestCount: refTestCount.value
		}
	})
	console.log('获取到的任务数量', res)
}
</script>