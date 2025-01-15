<!-- 关联单据模块 -->
<template>
	<div>
		<el-table :data="tableData" style="width: 100%">
			<el-table-column prop="date" label="收款单号" />
			<el-table-column prop="name" label="关联合同" />
		</el-table>
	</div>
</template>

<script lang="ts" setup>
const tableData = ref([])
</script>
<style scoped></style>
