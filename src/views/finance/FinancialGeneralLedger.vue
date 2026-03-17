<template>
	<div>
		<!-- 财务总账列表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div class="customer-search-area">
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-select v-model="searchForm.customerId" filterable placeholder="选择客户" size="default"
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.sql_hr_customer_abbreviation" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="searchForm.supplierId" filterable placeholder="选择供应商" size="default"
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4" style="text-align: left;">
						<el-button type="primary" plain @click="handleSearch()" size="default">查询</el-button>
						<el-button @click="handleReset()" size="default">重置</el-button>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" v-loading="loading" :data="paginatedData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }" border :summary-method="getSummaries"
				show-summary>
				<el-table-column prop="date" label="日期" width="130" align="center" />
				<el-table-column prop="summary" label="摘要/事项" width="150" align="center" />

				<el-table-column label="收支" align="center">
					<el-table-column prop="income.rmb" label="人民币" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.rmb > 0">{{ formatAmount(scope.row.income.rmb) }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="income.usd" label="美元" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.usd > 0">{{ formatAmount(scope.row.income.usd) }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="income.eur" label="欧元" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.eur > 0">{{ formatAmount(scope.row.income.eur) }}</span>
						</template>
					</el-table-column>
				</el-table-column>

				<el-table-column label="支出" align="center">
					<el-table-column prop="expenditure.rmb" label="人民币" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.rmb > 0">{{ formatAmount(scope.row.expenditure.rmb)
							}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="expenditure.usd" label="美元" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.usd > 0">{{ formatAmount(scope.row.expenditure.usd)
							}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="expenditure.eur" label="欧元" width="120" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.eur > 0">{{ formatAmount(scope.row.expenditure.eur)
							}}</span>
						</template>
					</el-table-column>
				</el-table-column>

				<el-table-column label="备注" align="center">
					<el-table-column label="收款客户/付款单位" width="180" align="center">
						<template #default="scope">
							<span v-if="scope.row.remarks && scope.row.remarks.customerOrSupplier">
								{{ scope.row.remarks.customerOrSupplier }}
							</span>
							<span v-else>-</span>
						</template>
					</el-table-column>
					<el-table-column label="合同号/备注" width="200" align="center">
						<template #default="scope">
							<span v-if="scope.row.remarks && scope.row.remarks.contractOrRemark">
								{{ scope.row.remarks.contractOrRemark }}
							</span>
							<span v-else>-</span>
						</template>
					</el-table-column>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handleCurrentChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :total="totalRecords" :page-sizes="[10, 20, 30, 50]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>
	</div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted, reactive, getCurrentInstance, toRefs } from 'vue'
import { ElTable, ElTableColumn, ElPagination, ElMessage } from 'element-plus'
import { getFinancialGeneralLedgerData } from '@/api/finance'

// 获取当前实例
const proxy = getCurrentInstance()?.proxy

// 字典数据
const state = reactive({
	optionss: {
		sql_hr_customer_abbreviation: [], // 客户
		sql_supplier_info: [] // 供应商
	}
})
const { optionss } = toRefs(state)

// 定义数据类型
interface Amount {
	rmb: number
	usd: number
	eur: number
}

interface Remarks {
	customerOrSupplier: string // 收款客户或付款单位
	contractOrRemark: string // 销售合同号/出运合同号/采购合同号/备注
}

interface LedgerItem {
	date: string
	summary: string
	income: Amount
	expenditure: Amount
	remarks: Remarks
}

// API返回的数据类型
interface ApiRow {
	date: string
	summary: string
	incomeCny: number
	incomeUsd: number
	incomeEur: number
	expenseCny: number
	expenseUsd: number
	expenseEur: number
	customerOrPayee: string // 收款客户或付款单位
	contractOrRemark: string // 合同号或备注
}

interface ApiResponse {
	rows: ApiRow[]
	total: {
		incomeCny: number
		incomeUsd: number
		incomeEur: number
		expenseCny: number
		expenseUsd: number
		expenseEur: number
	}
}

// 分页相关变量
const currentPage = ref(1)
const pageSize = ref(30)

// 表格数据
const ledgerData = ref<LedgerItem[]>([])

// 加载状态
const loading = ref(false)

// 搜索表单
const searchForm = reactive({
	customerId: '',
	supplierId: ''
})

// 分页相关计算属性
const totalRecords = computed(() => ledgerData.value.length)

const paginatedData = computed(() => {
	const start = (currentPage.value - 1) * pageSize.value
	const end = start + pageSize.value
	return ledgerData.value.slice(start, end)
})

// 格式化金额显示
const formatAmount = (amount: number): string => {
	if (amount === 0) return ''
	return amount.toFixed(2)
}

// 分页事件处理
const handleSizeChange = (val: number) => {
	pageSize.value = val
	currentPage.value = 1 // 重置到第一页
}

const handleCurrentChange = (val: number) => {
	currentPage.value = val
}

// 自定义合计方法 - 基于所有数据计算
const getSummaries = (param: any) => {
	const { columns } = param
	const sums: string[] = []

	columns.forEach((column: any, index: number) => {
		if (index === 0) {
			sums[index] = '合计'
			return
		}
		if (index === 1) {
			sums[index] = ''
			return
		}
		// 备注列（第9列和第10列）不显示合计
		if (index === 8 || index === 9) {
			sums[index] = ''
			return
		}

		// 计算各列的总和 - 使用所有数据而不是当前页数据
		const values = ledgerData.value.map((item: any) => {
			if (index === 2) return item.income.rmb
			if (index === 3) return item.income.usd
			if (index === 4) return item.income.eur
			if (index === 5) return item.expenditure.rmb
			if (index === 6) return item.expenditure.usd
			if (index === 7) return item.expenditure.eur
			return 0
		})

		if (!values.every((value: any) => isNaN(value))) {
			const total = values.reduce((prev: number, curr: number) => {
				const value = Number(curr)
				if (!isNaN(value)) {
					return prev + curr
				} else {
					return prev
				}
			}, 0)
			sums[index] = total > 0 ? total.toFixed(2) : ''
		} else {
			sums[index] = ''
		}
	})

	return sums
}


// 格式化日期
const formatDate = (dateStr: string): string => {
	if (!dateStr) return ''
	try {
		const date = new Date(dateStr)
		const year = date.getFullYear()
		const month = String(date.getMonth() + 1).padStart(2, '0')
		const day = String(date.getDate()).padStart(2, '0')
		return `${year}-${month}-${day}`
	} catch (error) {
		return dateStr
	}
}

// 处理备注字段
const parseRemarks = (row: ApiRow): Remarks => {
	return {
		customerOrSupplier: row.customerOrPayee || '-',
		contractOrRemark: row.contractOrRemark || '-'
	}
}

// 转换API数据为前端格式
const transformApiData = (apiRows: ApiRow[]): LedgerItem[] => {
	return apiRows.map(row => ({
		date: formatDate(row.date),
		summary: row.summary || '',
		income: {
			rmb: row.incomeCny || 0,
			usd: row.incomeUsd || 0,
			eur: row.incomeEur || 0
		},
		expenditure: {
			rmb: row.expenseCny || 0,
			usd: row.expenseUsd || 0,
			eur: row.expenseEur || 0
		},
		remarks: parseRemarks(row)
	}))
}

// 查询
const handleSearch = () => {
	currentPage.value = 1
	loadData()
}

// 重置
const handleReset = () => {
	searchForm.customerId = ''
	searchForm.supplierId = ''
	currentPage.value = 1
	loadData()
}

// 加载财务总账数据
const loadData = async () => {
	loading.value = true
	try {
		// 分别传递客户ID和供应商ID
		const customerId = searchForm.customerId || null
		const supplierId = searchForm.supplierId || null
		const response = await getFinancialGeneralLedgerData(customerId, supplierId)
		if (response && response.code === 200 && response.data && response.data.rows) {
			ledgerData.value = transformApiData(response.data.rows)
			// 重置到第一页
			currentPage.value = 1
		} else {
			ElMessage.error(response?.msg || '获取财务总账数据失败')
			ledgerData.value = []
		}
	} catch (error: any) {
		console.error('加载财务总账数据失败:', error)
		ElMessage.error('加载财务总账数据失败，请稍后重试')
		ledgerData.value = []
	} finally {
		loading.value = false
	}
}

// 加载字典数据
const loadDictData = async () => {
	try {
		const dictParams = [
			{ dictType: 'sql_hr_customer_abbreviation' },
			{ dictType: 'sql_supplier_info' }
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

// 组件挂载时加载数据
onMounted(async () => {
	await loadDictData()
	loadData()
})
</script>

<style scoped>
/* 表头高度缩减 */
:deep(.el-table__header-wrapper) {
	height: auto;
}

:deep(.el-table__header th) {
	padding: 4px 0;
	height: auto;
	line-height: 1.2;
}

:deep(.el-table__header .cell) {
	padding: 0 8px;
	line-height: 1.2;
}

/* 合计行样式 */
:deep(.el-table__footer-wrapper) {
	background-color: #f5f7fa;
}

:deep(.el-table__footer-wrapper .el-table__footer) {
	background-color: #f5f7fa;
}

:deep(.el-table__footer-wrapper .el-table__footer td) {
	background-color: #f5f7fa;
	font-weight: bold;
	border-top: 2px solid #409eff;
	padding: 4px 0;
}
</style>