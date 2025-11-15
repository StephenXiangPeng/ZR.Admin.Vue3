<template>
	<div class="financial-ledger">
		<el-card class="table-card">
			<template #header>
				<div class="card-header">
					<span>财务总账</span>
				</div>
			</template>

			<el-table v-loading="loading" :data="paginatedData" border stripe :summary-method="getSummaries"
				show-summary class="ledger-table" :cell-style="cellStyle" :header-cell-style="headerCellStyle">
				<el-table-column prop="date" label="日期" width="130" align="left" />
				<el-table-column prop="summary" label="摘要/事项" width="350" align="left" />

				<el-table-column label="收支" align="center">
					<el-table-column prop="income.rmb" label="人民币" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.rmb > 0">{{ formatAmount(scope.row.income.rmb) }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="income.usd" label="美元" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.usd > 0">{{ formatAmount(scope.row.income.usd) }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="income.eur" label="欧元" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.income.eur > 0">{{ formatAmount(scope.row.income.eur) }}</span>
						</template>
					</el-table-column>
				</el-table-column>

				<el-table-column label="支出" align="center">
					<el-table-column prop="expenditure.rmb" label="人民币" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.rmb > 0">{{ formatAmount(scope.row.expenditure.rmb)
							}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="expenditure.usd" label="美元" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.usd > 0">{{ formatAmount(scope.row.expenditure.usd)
							}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="expenditure.eur" label="欧元" width="150" align="right">
						<template #default="scope">
							<span v-if="scope.row.expenditure.eur > 0">{{ formatAmount(scope.row.expenditure.eur)
							}}</span>
						</template>
					</el-table-column>
				</el-table-column>

				<el-table-column label="备注" width="200" align="left">
					<template #default="scope">
						<div v-if="scope.row.remarks && scope.row.remarks.primary">{{ scope.row.remarks.primary }}</div>
						<div v-if="scope.row.remarks && scope.row.remarks.secondary">{{ scope.row.remarks.secondary }}
						</div>
						<span
							v-if="!scope.row.remarks || (!scope.row.remarks.primary && !scope.row.remarks.secondary)">-</span>
					</template>
				</el-table-column>
			</el-table>

			<!-- 分页组件 -->
			<div class="pagination-container">
				<el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
					:page-sizes="[5, 10, 20, 50]" :total="totalRecords" layout="total, sizes, prev, pager, next, jumper"
					@size-change="handleSizeChange" @current-change="handleCurrentChange" />
			</div>
		</el-card>
	</div>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue'
import { ElTable, ElTableColumn, ElCard, ElPagination, ElMessage } from 'element-plus'
import { getFinancialGeneralLedgerData } from '@/api/finance'

// 定义数据类型
interface Amount {
	rmb: number
	usd: number
	eur: number
}

interface Remarks {
	primary: string
	secondary: string
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
	remark: string | null
	incomeCny: number
	incomeUsd: number
	incomeEur: number
	expenseCny: number
	expenseUsd: number
	expenseEur: number
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
const pageSize = ref(10)

// 表格数据
const ledgerData = ref<LedgerItem[]>([])

// 加载状态
const loading = ref(false)

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
		if (index === 8) {
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

// 单元格样式
const cellStyle = ({ row, column, rowIndex, columnIndex }: any) => {
	// 金额列右对齐
	if (columnIndex >= 2 && columnIndex <= 7) {
		return {
			textAlign: 'right' as const
		}
	}
	return {}
}

// 表头样式
const headerCellStyle = ({ row, column, rowIndex, columnIndex }: any) => {
	return {
		textAlign: 'center' as const,
		fontWeight: 'bold' as const
	}
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

// 处理备注字段（将单个字符串拆分为 primary 和 secondary）
const parseRemarks = (remark: string | null): Remarks => {
	if (!remark) {
		return { primary: '', secondary: '' }
	}
	// 如果备注包含换行符，第一行作为 primary，其余作为 secondary
	const lines = remark.split('\n').filter(line => line.trim())
	if (lines.length === 0) {
		return { primary: '', secondary: '' }
	}
	if (lines.length === 1) {
		return { primary: lines[0], secondary: '' }
	}
	return {
		primary: lines[0],
		secondary: lines.slice(1).join('\n')
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
		remarks: parseRemarks(row.remark)
	}))
}

// 加载财务总账数据
const loadData = async (customerID?: string) => {
	loading.value = true
	try {
		const response = await getFinancialGeneralLedgerData(customerID)
		if (response && response.code === 200 && response.data && response.data.rows) {
			ledgerData.value = transformApiData(response.data.rows || [])
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

// 组件挂载时加载数据
onMounted(() => {
	loadData()
})
</script>

<style scoped>
.financial-ledger {
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

.ledger-table {
	width: 100%;
}

/* 自定义表格样式以保持原有布局 */
.ledger-table :deep(.el-table__header) {
	background-color: #f5f7fa;
}

.ledger-table :deep(.el-table__header th) {
	background-color: #f5f7fa;
	color: #606266;
	font-weight: bold;
	border-bottom: 1px solid #ebeef5;
}

.ledger-table :deep(.el-table__body tr:hover > td) {
	background-color: #f5f7fa;
}

.ledger-table :deep(.el-table__footer-wrapper) {
	background-color: #f5f7fa;
}

.ledger-table :deep(.el-table__footer-wrapper .el-table__footer) {
	background-color: #f5f7fa;
}

.ledger-table :deep(.el-table__footer-wrapper .el-table__footer td) {
	background-color: #f5f7fa;
	font-weight: bold;
	border-top: 2px solid #409eff;
}

/* 金额列样式 - 只设置对齐方式，字体保持默认 */
.ledger-table :deep(.el-table__body td:nth-child(3)),
.ledger-table :deep(.el-table__body td:nth-child(4)),
.ledger-table :deep(.el-table__body td:nth-child(5)),
.ledger-table :deep(.el-table__body td:nth-child(6)),
.ledger-table :deep(.el-table__body td:nth-child(7)),
.ledger-table :deep(.el-table__body td:nth-child(8)) {
	text-align: right;
}

.ledger-table :deep(.el-table__footer td:nth-child(3)),
.ledger-table :deep(.el-table__footer td:nth-child(4)),
.ledger-table :deep(.el-table__footer td:nth-child(5)),
.ledger-table :deep(.el-table__footer td:nth-child(6)),
.ledger-table :deep(.el-table__footer td:nth-child(7)),
.ledger-table :deep(.el-table__footer td:nth-child(8)) {
	text-align: right;
}

/* 备注列样式 */
.ledger-table :deep(.el-table__body td:last-child) {
	line-height: 1.4;
}

.ledger-table :deep(.el-table__body td:last-child div) {
	margin: 2px 0;
}

/* 分页样式 */
.pagination-container {
	margin-top: 20px;
	display: flex;
	justify-content: center;
}

/* 响应式设计 */
@media (max-width: 1200px) {
	.ledger-table {
		min-width: 1000px;
	}
}
</style>