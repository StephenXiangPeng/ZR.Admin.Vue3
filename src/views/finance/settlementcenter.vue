<template>
	<div class="settlement-center">
		<el-card class="table-card">
			<template #header>
				<div class="card-header">
					<span>结算中心</span>
				</div>
			</template>

			<!-- 查询条件区域 -->
			<div class="search-area">
				<el-form :inline="true" :model="searchForm" class="search-form">
					<el-form-item label="出运发货单号">
						<el-select v-model="searchForm.shippingDeliveriesId" filterable placeholder="请选出运发货单号" clearable
							style="width: 200px" size="default">
							<el-option v-for="dict in optionss.sql_settlement_center_shipping" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-form-item>
					<el-form-item label="出运日期">
						<el-date-picker v-model="searchForm.shippingDateStart" type="date" placeholder="开始日期"
							style="width: 150px" size="default" value-format="YYYY-MM-DD" />
						<span style="margin: 0 10px;">至</span>
						<el-date-picker v-model="searchForm.shippingDateEnd" type="date" placeholder="结束日期"
							style="width: 150px" size="default" value-format="YYYY-MM-DD" />
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="handleSearch" size="default">查询</el-button>
						<el-button @click="handleReset" size="default">重置</el-button>
					</el-form-item>
				</el-form>
			</div>

			<!-- 表格区域 -->
			<el-table :data="tableData" border stripe v-loading="loading"
				:header-cell-style="{ background: '#f5f7fa', color: '#606266', fontWeight: 'bold' }"
				style="width: 100%; margin-top: 20px;">
				<el-table-column prop="shippingDate" label="出运日期" width="120" align="center" />
				<el-table-column prop="invoiceNumber" label="发票号码" width="150" align="center" />
				<el-table-column prop="salesContract" label="销售合同" width="150" align="center" />
				<el-table-column prop="customerName" label="客户名称" width="150" align="center" />
				<el-table-column prop="foreignCurrency" label="外销币种" width="100" align="center" />
				<el-table-column prop="priceTerms" label="价格条款" width="120" align="center" />
				<el-table-column prop="totalReceivable" label="应收货款" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalReceivable) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalReceived" label="已收货款" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalReceived) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalReceivedCNY" label="已收货款¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalReceivedCNY) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalUnreceived" label="未收货款" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalUnreceived) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalRefund" label="应退税¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalRefund) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalRefunded" label="已退税¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalRefunded) }}
					</template>
				</el-table-column>
				<el-table-column prop="totalPaid" label="已付运杂费¥" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalPaid) }}
					</template>
				</el-table-column>
				<el-table-column prop="expressFee" label="快件费用¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.expressFee) }}
					</template>
				</el-table-column>
				<el-table-column prop="otherDomesticCost" label="其它国内费用¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.otherDomesticCost) }}
					</template>
				</el-table-column>
				<el-table-column prop="commissionPaid" label="已付佣金¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.commissionPaid) }}
					</template>
				</el-table-column>
				<el-table-column prop="shippingPaidAmount" label="已付海运¥" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.shippingPaidAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="otherForeignCost" label="其它国外费用¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.otherForeignCost) }}
					</template>
				</el-table-column>
				<el-table-column prop="sampleFeePaid" label="已付样品费¥" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.sampleFeePaid) }}
					</template>
				</el-table-column>
				<el-table-column prop="otherFeePaid" label="已付其他费用¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.otherFeePaid) }}
					</template>
				</el-table-column>
				<el-table-column prop="factoryPayableAmount" label="工厂应付金额¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.factoryPayableAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="factoryPaidAmount" label="工厂已付金额¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.factoryPaidAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="factoryUnpaidAmount" label="工厂未付金额¥" width="150" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.factoryUnpaidAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="actualBusinessProfit" label="实际业务毛利¥" width="150" align="right">
					<template #default="scope">
						<span :style="{ color: scope.row.actualBusinessProfit >= 0 ? '#67C23A' : '#F56C6C' }">
							{{ formatAmount(scope.row.actualBusinessProfit) }}
						</span>
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="120" align="center">
					<template #default="scope">
						<el-button type="primary" link size="small" @click="handleView(scope.row)">查看详情</el-button>
					</template>
				</el-table-column>
			</el-table>

			<!-- 分页组件 -->
			<div class="pagination-container">
				<el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
					:page-sizes="[10, 20, 50, 100]" :total="totalItems" layout="total, sizes, prev, pager, next, jumper"
					@size-change="handleSizeChange" @current-change="handleCurrentChange" />
			</div>
		</el-card>
	</div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted, getCurrentInstance, toRefs } from 'vue'
import { ElMessage } from 'element-plus'
import request from '@/utils/request'
import { useRouter } from 'vue-router'

const router = useRouter()
const proxy = getCurrentInstance()?.proxy

// 查询表单
const searchForm = reactive({
	shippingDeliveriesId: '',
	shippingDateStart: '',
	shippingDateEnd: ''
})

// 表格数据
const tableData = ref([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(10)
const totalItems = ref(0)

// 字典数据
const state = reactive({
	optionss: {
		sql_settlement_center_shipping: []
	}
})
const { optionss } = toRefs(state)

// 格式化金额
const formatAmount = (amount: any) => {
	if (amount === null || amount === undefined || amount === '') {
		return '0.00'
	}
	const num = parseFloat(amount)
	return isNaN(num) ? '0.00' : num.toFixed(2)
}

// 加载字典数据
const loadDictData = async () => {
	try {
		const dictParams = [
			{ dictType: 'sql_settlement_center_shipping' }
		]
		if (proxy) {
			const response = await (proxy as any).getDicts(dictParams)
			response.data.forEach((element: any) => {
				state.optionss[element.dictType] = element.list
			})
		}
	} catch (error) {
		console.error('加载字典数据失败:', error)
	}
}

// 加载表格数据
const loadData = async () => {
	loading.value = true
	try {
		// TODO: 替换为实际的API接口
		const response: any = await request({
			url: 'ShippingDeliveries/GetSettlementList/GetList',
			method: 'GET',
			params: {
				PageNum: currentPage.value,
				PageSize: pageSize.value,
				ShippingDeliveriesId: searchForm.shippingDeliveriesId,
				ShippingDateStart: searchForm.shippingDateStart,
				ShippingDateEnd: searchForm.shippingDateEnd
			}
		})

		if (response && response.code === 200 && response.data) {
			tableData.value = response.data.result || []
			totalItems.value = response.data.total || 0

			// 处理数据格式化
			tableData.value.forEach((item: any) => {
				// 格式化日期
				if (item.shippingDate) {
					item.shippingDate = item.shippingDate.split(' ')[0]
				}
				// 确保金额字段为数字
				item.totalReceivable = parseFloat(item.totalReceivable) || 0
				item.totalReceived = parseFloat(item.totalReceived) || 0
				item.totalReceivedCNY = parseFloat(item.totalReceivedCNY) || 0
				item.totalUnreceived = parseFloat(item.totalUnreceived) || 0
				item.totalRefund = parseFloat(item.totalRefund) || 0
				item.totalRefunded = parseFloat(item.totalRefunded) || 0
				item.totalPaid = parseFloat(item.totalPaid) || 0
				item.expressFee = parseFloat(item.expressFee) || 0
				item.otherDomesticCost = parseFloat(item.otherDomesticCost) || 0
				item.commissionPaid = parseFloat(item.commissionPaid) || 0
				item.shippingPaidAmount = parseFloat(item.shippingPaidAmount) || 0
				item.otherForeignCost = parseFloat(item.otherForeignCost) || 0
				item.sampleFeePaid = parseFloat(item.sampleFeePaid) || 0
				item.otherFeePaid = parseFloat(item.otherFeePaid) || 0
				item.factoryPayableAmount = parseFloat(item.factoryPayableAmount) || 0
				item.factoryPaidAmount = parseFloat(item.factoryPaidAmount) || 0
				item.factoryUnpaidAmount = parseFloat(item.factoryUnpaidAmount) || 0
				item.actualBusinessProfit = parseFloat(item.actualBusinessProfit) || 0
			})
		} else {
			ElMessage.error((response && response.msg) || '获取数据失败')
			tableData.value = []
			totalItems.value = 0
		}
	} catch (error) {
		console.error('加载数据失败:', error)
		ElMessage.error('加载数据失败，请稍后重试')
		tableData.value = []
		totalItems.value = 0
	} finally {
		loading.value = false
	}
}

// 查询
const handleSearch = () => {
	currentPage.value = 1
	loadData()
}

// 重置
const handleReset = () => {
	searchForm.shippingDeliveriesId = ''
	searchForm.shippingDateStart = ''
	searchForm.shippingDateEnd = ''
	currentPage.value = 1
	loadData()
}

// 分页变化
const handleSizeChange = (size: number) => {
	pageSize.value = size
	currentPage.value = 1
	loadData()
}

const handleCurrentChange = (page: number) => {
	currentPage.value = page
	loadData()
}

// 查看详情
const handleView = (row: any) => {
	// 跳转到结算中心详情页面
	router.push({
		path: '/finance/settlementcenter',
		query: {
			contractId: row.shippingDeliveriesId || row.id,
			viewDetail: 'true'
		}
	})
}

// 初始化
onMounted(async () => {
	await loadDictData()
	loadData()
})
</script>

<style scoped>
.settlement-center {
	padding: 20px;
	background-color: #f5f5f5;
	min-height: 100vh;
}

.table-card {
	box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.card-header {
	font-size: 18px;
	font-weight: bold;
	color: #303133;
}

.search-area {
	background-color: #f8f9fa;
	padding: 15px;
	border-radius: 4px;
	margin-bottom: 20px;
}

.search-form {
	margin: 0;
}

.pagination-container {
	margin-top: 20px;
	display: flex;
	justify-content: flex-end;
}

:deep(.el-table) {
	font-size: 14px;
}

:deep(.el-table th) {
	background-color: #f5f7fa;
}

:deep(.el-table td) {
	padding: 8px 0;
}
</style>