<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;审批记录表</span>
			<el-divider></el-divider>
			<el-form-item>
				<el-radio-group v-model="DocumentSwitch">
					<el-radio-button label="1">我审批的单据</el-radio-button>
					<el-radio-button label="2">我提交的单据</el-radio-button>
				</el-radio-group>
			</el-form-item>
			<el-table :data="ApprovalRecordTableData" style="width: 100%">
				<el-table-column prop="documentType" label="单据类型" width="150"></el-table-column>
				<el-table-column prop="documentID" label="单据ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="documentID" label="单据编号" width="150"></el-table-column>
				<el-table-column label="审批进度" width="400" align="center">
					<template #default="{ row }">
						<el-steps :active="row.approvalSteps.length"
							:status="getStatus(row.approvalSteps[row.approvalSteps.length - 1]?.status)" size="large"
							align-center>
							<el-step v-for="step in row.approvalSteps" :key="step.StageID" :title="step.approvalUser"
								:description="getStatusText(step.status)" :status="getStatus(step.status)" />
						</el-steps>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
				:total="total" background layout="prev, pager, next" style="margin-top: 5px;" />

		</div>
	</div>
</template>
<script setup lang="ts">
import { ref } from 'vue'
import { ElDivider, ElTable, ElTableColumn, ElMessage } from 'element-plus'
import request from '@/utils/request';

const active = ref(2);
const DocumentSwitch = ref('1');//单据类型切换
const ApprovalRecordTableData = ref<ApprovalRecord[]>([]);//审批记录表数据
const currentPage = ref(1);//合同表当前页
const pageSize = ref(10);//合同表每页条数
const total = ref(0);//合同表总条数
// 定义接口返回的数据类型
interface ApprovalStep {
	StageID: number;
	Status: number;
	ApprovalUser: string;
	ApprovalDate: string;
	Comment: string;
}

interface ApprovalRecord {
	DocumentType: number;
	DocumentID: number;
	DocumentNumber: string;
	approvalSteps: ApprovalStep[];
}

// 获取审批状态文本
const getStatusText = (status: number) => {
	switch (status) {
		case 0: return '待审批';
		case 1: return '已通过';
		case 2: return '已拒绝';
		case 3: return '等待上一阶段';
		case 4: return '已终止';
		default: return '未知状态';
	}
}
// 获取步骤状态
const getStatus = (status: number) => {
	switch (status) {
		case 0: return 'wait';
		case 1: return 'success';
		case 2: return 'error';
		case 3: return 'wait';
		case 4: return 'error';
		default: return 'wait';
	}
}

// 获取阶段标题
const getStageTitle = (stageID: number) => {
	switch (stageID) {
		case 7: return '提交申请';
		case 8: return '部门审批';
		case 9: return '最终审批';
		default: return `阶段${stageID}`;
	}
}

// 获取审批记录
const getApprovalRecordData = async () => {
	try {
		const response = await request({
			url: 'ApprovalFlow/GetApprovalRecordByUser/GetApprovalRecord',
			method: 'get',
			params: {
				pageNum: currentPage.value,
				pageSize: pageSize.value,
				Type: DocumentSwitch.value
			}
		})

		if (response.code === 200) {
			ApprovalRecordTableData.value = response.data.rows
			total.value = response.data.total
		} else {
			ElMessage.error(response.msg || '获取审批记录失败')
		}
	} catch (error) {
		console.error('获取审批记录失败:', error)
		ElMessage.error('获取审批记录失败')
	}
}
getApprovalRecordData();
// 监听单据类型切换
watch(() => DocumentSwitch.value, () => {
	currentPage.value = 1  // 重置页码
	getApprovalRecordData()
})

// 页码变化处理
const handlePageChange = (page: number) => {
	currentPage.value = page
	getApprovalRecordData()
}

</script>
<style scoped></style>
