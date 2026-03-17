<template>
	<div>
		<!-- 付款列表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div class="customer-search-area">
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-select v-model="searchForm.paymentCategory" filterable placeholder="选择付款类别" size="default"
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.hr_payment_category" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-input v-model="searchForm.paymentName" placeholder="款项名称" size="default" clearable />
					</el-col>
					<el-col :span="4">
						<el-input v-model="searchForm.payeeName" placeholder="收款单位" size="default" clearable />
					</el-col>
					<el-col :span="4">
						<el-select v-model="searchForm.ourCompany" filterable placeholder="选择我方公司" size="default"
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictLabel" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="searchForm.applicationDateStart" type="date" placeholder="申请日期起"
							size="default" style="width: 100%" />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="searchForm.applicationDateEnd" type="date" placeholder="申请日期止"
							size="default" style="width: 100%" />
					</el-col>
				</el-row>
				<el-row :gutter="15" class="search-row">
					<el-col :span="24" style="text-align: right;">
						<el-button type="primary" plain @click="handleSearch()" size="default">查询</el-button>
						<el-button @click="handleReset()" size="default">重置</el-button>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" :data="tableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="paymentCategory" label="付款类别" width="120" align="left"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="120" align="left"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位" width="180" align="left"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="120" align="left"></el-table-column>
				<el-table-column prop="pendingAmount" label="待付款金额" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.pendingAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="applicant" label="申请人" width="100" align="left"></el-table-column>
				<el-table-column prop="remark" label="备注" width="200" align="left"
					show-overflow-tooltip></el-table-column>
				<el-table-column prop="saleContract" label="销售合同" width="150" align="left"></el-table-column>
				<el-table-column prop="shippingContract" label="出运合同" width="150" align="left"></el-table-column>
				<el-table-column prop="applicationDate" label="申请日期" width="120" align="center">
					<template #default="scope">
						{{ formatDate(scope.row.applicationDate) }}
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="150" align="center">
					<template #default="scope">
						<el-button type="text" size="small" @click="handleView(scope.row)">查看</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :total="totalItems" :page-sizes="[10, 20, 30, 50]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, getCurrentInstance, toRefs } from 'vue'
import { ElMessage } from 'element-plus'

// 获取当前实例
const proxy = getCurrentInstance()?.proxy

// 字典数据
const state = reactive({
	optionss: {
		hr_payment_category: [], // 付款类别
		hr_ourcompany: [] // 我方公司
	}
})
const { optionss } = toRefs(state)

// 搜索表单
const searchForm = reactive({
	paymentCategory: '',
	paymentName: '',
	payeeName: '',
	ourCompany: '',
	applicationDateStart: '',
	applicationDateEnd: ''
})

// 表格数据
const tableData = ref<any[]>([])

// 分页相关
const currentPage = ref(1)
const pageSize = ref(30)
const totalItems = ref(0)


// 格式化金额
const formatAmount = (amount: number | string): string => {
	if (!amount && amount !== 0) return '0.00'
	const num = typeof amount === 'string' ? parseFloat(amount) : amount
	return num.toFixed(2)
}

// 格式化日期
const formatDate = (date: string | Date): string => {
	if (!date) return ''
	const d = typeof date === 'string' ? new Date(date) : date
	const year = d.getFullYear()
	const month = String(d.getMonth() + 1).padStart(2, '0')
	const day = String(d.getDate()).padStart(2, '0')
	return `${year}-${month}-${day}`
}

// 查询
const handleSearch = () => {
	currentPage.value = 1
	loadData()
}

// 重置
const handleReset = () => {
	searchForm.paymentCategory = ''
	searchForm.paymentName = ''
	searchForm.payeeName = ''
	searchForm.ourCompany = ''
	searchForm.applicationDateStart = ''
	searchForm.applicationDateEnd = ''
	currentPage.value = 1
	loadData()
}

// 加载数据
const loadData = async () => {
	try {
		// TODO: 调用API获取数据
		// 示例数据
		tableData.value = [
			{
				paymentCategory: '货款',
				paymentName: '预付款',
				payeeName: '供应商A',
				ourCompany: '公司A',
				pendingAmount: 100000.00,
				applicant: '张三',
				remark: '备注信息',
				saleContract: 'SC2024001',
				shippingContract: 'SH2024001',
				applicationDate: '2024-01-15'
			}
		]
		totalItems.value = tableData.value.length
	} catch (error) {
		ElMessage.error('加载数据失败')
		console.error(error)
	}
}

// 分页变化
const handlePageChange = (page: number) => {
	currentPage.value = page
	loadData()
}

// 每页条数变化
const handleSizeChange = (size: number) => {
	pageSize.value = size
	currentPage.value = 1
	loadData()
}

// 查看
const handleView = (row: any) => {
	console.log('查看', row)
	// TODO: 实现查看逻辑
}


// 加载字典数据
const loadDictData = async () => {
	try {
		const dictParams = [
			{ dictType: 'hr_payment_category' },
			{ dictType: 'hr_ourcompany' }
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

// 初始化
onMounted(async () => {
	await loadDictData()
	loadData()
})
</script>

<style scoped></style>
