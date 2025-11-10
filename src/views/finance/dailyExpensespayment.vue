<template>
	<div>
		<!-- 付款列表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
					<el-col :span="4">
						<el-select v-model="searchForm.paymentName" filterable placeholder="选择款项名称" size="default"
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.hr_daily_expenses" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
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
				<el-row :gutter="15">
					<el-col :span="24" style="text-align: right;">
						<el-button type="primary" plain @click="handleSearch()" size="default">查询</el-button>
						<el-button @click="handleReset()" size="default">重置</el-button>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table :data="tableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="applicationDate" label="申请日期" width="120" align="center">
					<template #default="scope">
						{{ formatDate(scope.row.applicationDate) }}
					</template>
				</el-table-column>
				<el-table-column prop="saleContract" label="销售合同" width="150" align="left"></el-table-column>
				<el-table-column prop="shippingContract" label="出运编号" width="150" align="left"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位" width="180" align="left"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="120" align="left">
					<template #default="scope">
						{{ scope.row.paymentNameLabel || scope.row.paymentName }}
					</template>
				</el-table-column>
				<el-table-column prop="pendingAmount" label="付款金额" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.pendingAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="applicant" label="申请人" width="100" align="left"></el-table-column>
				<el-table-column fixed="right" label="详情/状态" width="150" align="center">
					<template #default="scope">
						<el-button type="text" size="small" @click="handleView(scope.row)">查看</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :total="totalItems" :page-sizes="[10, 20, 50, 100]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, getCurrentInstance, toRefs } from 'vue'
import { ElMessage } from 'element-plus'
import request from '@/utils/request'

// 获取当前实例
const proxy = getCurrentInstance()?.proxy

// 字典数据
const state = reactive({
	optionss: {
		hr_payment_category: [], // 付款类别
		hr_ourcompany: [], // 我方公司
		hr_daily_expenses: [] // 日常费用款项名称
	}
})
const { optionss } = toRefs(state)

// 搜索表单
const searchForm = reactive({
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
const pageSize = ref(10)
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
		const response = await request({
			url: 'PaymentRequest/GetPaymentRequestListByPaymentCategory/GetPaymentRequestList',
			method: 'GET',
			params: {
				PageNum: currentPage.value,
				PageSize: pageSize.value,
				PaymentCategory: 4 // 固定为4，表示日常费用付款
			}
		})

		const res = response as any
		if (res && res.code === 200) {
			// 处理返回的数据
			const result = res.data || {}
			tableData.value = result.result || []
			totalItems.value = result.totalNum || 0

			// 处理字典数据映射
			if (tableData.value.length > 0 && optionss.value.hr_payment_category && optionss.value.hr_ourcompany && optionss.value.hr_daily_expenses) {
				tableData.value.forEach((element: any) => {
					// 映射付款类别
					if (element.paymentCategory) {
						const category = optionss.value.hr_payment_category.find((item: any) => item.dictValue == element.paymentCategory)
						if (category) {
							element.paymentCategoryLabel = category.dictLabel
						}
					}
					// 映射款项名称
					if (element.paymentName) {
						const paymentName = optionss.value.hr_daily_expenses.find((item: any) => item.dictValue == element.paymentName.toString())
						if (paymentName) {
							element.paymentNameLabel = paymentName.dictLabel
						}
					}
					// 映射我方公司
					if (element.ourCompany) {
						const company = optionss.value.hr_ourcompany.find((item: any) => item.dictLabel == element.ourCompany)
						if (company) {
							element.ourCompanyLabel = company.dictLabel
						}
					}
				})
			}
		} else {
			tableData.value = []
			totalItems.value = 0
			const errorMsg = res?.msg || '加载数据失败'
			ElMessage.error(errorMsg)
		}
	} catch (error) {
		ElMessage.error('加载数据失败')
		console.error(error)
		tableData.value = []
		totalItems.value = 0
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
			{ dictType: 'hr_ourcompany' },
			{ dictType: 'hr_daily_expenses' }
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
