<template>
	<div class="app-container">
		<el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="88px">
			<el-form-item label="单据编号" prop="documentNo">
				<el-input v-model="queryParams.documentNo" placeholder="请输入单据编号" clearable @keyup.enter="handleQuery" />
			</el-form-item>
			<el-form-item label="单据类型" prop="documentType">
				<el-select v-model="queryParams.documentType" placeholder="请选择单据类型" clearable>
					<el-option v-for="dict in optionss.hr_business_documents" :key="dict.dictCode"
						:label="dict.dictLabel" :value="dict.dictValue" />
				</el-select>
			</el-form-item>
			<el-form-item label="申请人" prop="applyUser">
				<el-select v-model="queryParams.applyUser" placeholder="请选择申请人" clearable filterable>
					<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode" :label="dict.dictLabel"
						:value="dict.dictValue" />
				</el-select>
			</el-form-item>
			<el-form-item label="审批状态" prop="approveStatus">
				<el-select v-model="queryParams.approveStatus" placeholder="请选择审批状态" clearable>
					<el-option v-for="item in approveStatusOptions" :key="item.value" :label="item.label"
						:value="item.value" />
				</el-select>
			</el-form-item>
			<el-form-item label="执行状态" prop="executeStatus">
				<el-select v-model="queryParams.executeStatus" placeholder="请选择执行状态" clearable>
					<el-option v-for="item in executeStatusOptions" :key="item.value" :label="item.label"
						:value="item.value" />
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" icon="Search" @click="handleQuery">{{ $t('btn.search') }}</el-button>
				<el-button icon="Refresh" @click="resetQuery">{{ $t('btn.reset') }}</el-button>
			</el-form-item>
		</el-form>

		<el-row :gutter="10" class="mb8">
			<right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
		</el-row>

		<el-table v-loading="loading" :data="listData">
			<el-table-column label="ID" align="center" prop="id" width="80" v-if="false" />
			<el-table-column label="原表名" align="center" prop="sourceTable" width="120" :show-overflow-tooltip="true"
				v-if="false" />
			<el-table-column label="原单据ID" align="center" prop="sourceId" width="100" v-if="false" />
			<el-table-column label="单据类型" align="center" prop="documentType" width="160" :show-overflow-tooltip="true">
				<template #default="scope">
					<span>{{ getDocumentTypeLabel(scope.row.documentType) }}</span>
				</template>
			</el-table-column>
			<el-table-column label="单据编号" align="center" prop="documentNo" width="140" :show-overflow-tooltip="true" />
			<el-table-column label="申请原因" align="center" prop="applyReason" min-width="150"
				:show-overflow-tooltip="true" />
			<el-table-column label="申请人" align="center" prop="applyUser" width="120">
				<template #default="scope">
					<span>{{ getApplyUserLabel(scope.row.applyUser) }}</span>
				</template>
			</el-table-column>
			<el-table-column label="申请时间" align="center" prop="applyTime" width="170">
				<template #default="scope">
					<span>{{ parseTime(scope.row.applyTime) }}</span>
				</template>
			</el-table-column>
			<el-table-column label="审批状态" align="center" prop="approveStatus" width="100">
				<template #default="scope">
					<el-tag :type="getApproveStatusType(scope.row.approveStatus)">
						{{ getApproveStatusLabel(scope.row.approveStatus) }}
					</el-tag>
				</template>
			</el-table-column>
			<el-table-column label="审批人" align="center" prop="approveUser" width="100" />
			<el-table-column label="审批时间" align="center" prop="approveTime" width="170">
				<template #default="scope">
					<span>{{ scope.row.approveTime ? parseTime(scope.row.approveTime) : '—' }}</span>
				</template>
			</el-table-column>
			<el-table-column label="执行状态" align="center" prop="executeStatus" width="100">
				<template #default="scope">
					<el-tag :type="getExecuteStatusType(scope.row.executeStatus)">
						{{ getExecuteStatusLabel(scope.row.executeStatus) }}
					</el-tag>
				</template>
			</el-table-column>
			<el-table-column label="执行时间" align="center" prop="executeTime" width="170">
				<template #default="scope">
					<span>{{ scope.row.executeTime ? parseTime(scope.row.executeTime) : '—' }}</span>
				</template>
			</el-table-column>
			<el-table-column label="备注" align="center" prop="remark" min-width="120" :show-overflow-tooltip="true" />
			<el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="120" fixed="right">
				<template #default="scope">
					<el-button size="small" text icon="View" @click="handleViewDetail(scope.row)">查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>

		<pagination v-model:total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize"
			@pagination="getList" />

		<!-- 查看详情对话框 -->
		<el-dialog v-model="detailVisible" title="单据删除申请详情" width="680px" destroy-on-close>
			<el-descriptions :column="2" border v-if="detailData">
				<el-descriptions-item label="单据类型">{{ getDocumentTypeLabel(detailData.documentType)
				}}</el-descriptions-item>
				<el-descriptions-item label="单据编号">{{ detailData.documentNo || '—' }}</el-descriptions-item>
				<el-descriptions-item label="申请原因" :span="2">{{ detailData.applyReason || '—' }}</el-descriptions-item>
				<el-descriptions-item label="申请人">{{ getApplyUserLabel(detailData.applyUser) }}</el-descriptions-item>
				<el-descriptions-item label="申请时间">{{ detailData.applyTime ? parseTime(detailData.applyTime) : '—'
				}}</el-descriptions-item>
				<el-descriptions-item label="审批状态">{{ getApproveStatusLabel(detailData.approveStatus)
				}}</el-descriptions-item>
				<el-descriptions-item label="审批人">{{ detailData.approveUser || '—' }}</el-descriptions-item>
				<el-descriptions-item label="审批时间">{{ detailData.approveTime ? parseTime(detailData.approveTime) : '—'
				}}</el-descriptions-item>
				<el-descriptions-item label="审批备注" :span="2">{{ detailData.approveRemark || '—'
				}}</el-descriptions-item>
				<el-descriptions-item label="执行状态">{{ getExecuteStatusLabel(detailData.executeStatus)
				}}</el-descriptions-item>
				<el-descriptions-item label="执行时间">{{ detailData.executeTime ? parseTime(detailData.executeTime) : '—'
				}}</el-descriptions-item>
				<el-descriptions-item label="备注" :span="2">{{ detailData.remark || '—' }}</el-descriptions-item>
			</el-descriptions>
			<template #footer>
				<el-button @click="detailVisible = false">关闭</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script setup name="SysDocumentDeleteRequest">
import request from '@/utils/request'
import { getCurrentInstance } from 'vue'

const { proxy } = getCurrentInstance()
const state = reactive({
	optionss: {
		hr_business_documents: [],
		sql_all_user: []
	}
})
const optionss = state.optionss

const loading = ref(true)
const showSearch = ref(true)
const total = ref(0)
const listData = ref([])
const detailVisible = ref(false)
const detailData = ref(null)

const queryParams = reactive({
	pageNum: 1,
	pageSize: 10,
	documentNo: undefined,
	documentType: undefined,
	applyUser: undefined,
	approveStatus: undefined,
	executeStatus: undefined
})

// 审批状态 0待审批 1通过 2驳回
const approveStatusOptions = [
	{ value: 0, label: '待审批' },
	{ value: 1, label: '通过' },
	{ value: 2, label: '驳回' }
]

// 执行状态 0未执行 1已执行
const executeStatusOptions = [
	{ value: 0, label: '未执行' },
	{ value: 1, label: '已执行' }
]

const dictParams = [{ dictType: 'hr_business_documents' }, { dictType: 'sql_all_user' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})

function getDocumentTypeLabel(value) {
	if (value == null || value === undefined || value === '') return '—'
	const item = optionss.hr_business_documents.find((o) => o.dictValue == value)
	return item ? item.dictLabel : String(value)
}

function getApplyUserLabel(value) {
	if (value == null || value === undefined || value === '') return '—'
	const item = optionss.sql_all_user.find((o) => o.dictValue == value)
	return item ? item.dictLabel : String(value)
}

function getApproveStatusLabel(status) {
	if (status == null || status === undefined) return '—'
	const item = approveStatusOptions.find((o) => o.value === status)
	return item ? item.label : String(status)
}

function getApproveStatusType(status) {
	const map = { 0: 'warning', 1: 'success', 2: 'danger' }
	return map[status] ?? 'info'
}

function getExecuteStatusLabel(status) {
	if (status == null || status === undefined) return '—'
	const item = executeStatusOptions.find((o) => o.value === status)
	return item ? item.label : String(status)
}

function getExecuteStatusType(status) {
	const map = { 0: 'info', 1: 'success' }
	return map[status] ?? 'info'
}

/** 查询列表 */
function getList() {
	loading.value = true
	request({
		url: 'ApplyDeleteDocument/GetDocumentDeleteRequestList/GetList',
		method: 'get',
		params: {
			PageNum: queryParams.pageNum,
			PageSize: queryParams.pageSize,
			DocumentNo: queryParams.documentNo,
			DocumentType: queryParams.documentType,
			ApplyUser: queryParams.applyUser,
			ApproveStatus: queryParams.approveStatus,
			ExecuteStatus: queryParams.executeStatus
		}
	})
		.then((response) => {
			listData.value = response.data?.result ?? response.data ?? []
			total.value = response.data?.total ?? response.data?.totalNum ?? response.data?.totalCount ?? response.data?.page?.totalCount ?? 0
			loading.value = false
		})
		.catch(() => {
			listData.value = []
			loading.value = false
		})
}

/** 搜索 */
function handleQuery() {
	queryParams.pageNum = 1
	getList()
}

/** 重置 */
function resetQuery() {
	proxy.resetForm('queryRef')
	handleQuery()
}

/** 查看详情 */
function handleViewDetail(row) {
	detailData.value = { ...row }
	detailVisible.value = true
}

getList()
</script>
