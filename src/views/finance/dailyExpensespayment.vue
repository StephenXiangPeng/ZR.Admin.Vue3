<template>
	<div>
		<!-- 付款列表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div class="customer-search-area">
				<el-row :gutter="15" class="search-row">
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
					<el-col :span="4" style="text-align: left;">
						<el-button type="primary" plain @click="handleSearch()" size="default">查询</el-button>
						<el-button @click="handleReset()" size="default">重置</el-button>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" :data="tableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="applicationDate" label="申请日期" width="120" align="center">
					<template #default="scope">
						{{ formatDate(scope.row.applicationDate) }}
					</template>
				</el-table-column>
				<el-table-column prop="saleContract" label="销售合同" width="150" align="left"
					v-if="false"></el-table-column>
				<el-table-column prop="shippingContract" label="出运编号" width="150" align="left"
					v-if="false"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位" width="180" align="left"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="120" align="left">
					<template #default="scope">
						{{ scope.row.paymentNameLabel || scope.row.paymentName }}
					</template>
				</el-table-column>
				<el-table-column prop="relatedCustomer" label="相关客户" width="120" align="left">
					<template #default="scope">
						{{ scope.row.relatedCustomerLabel || scope.row.relatedCustomer }}
					</template>
				</el-table-column>
				<el-table-column prop="totalAmount" label="付款金额" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="applicant" label="申请人" width="120" align="left">
					<template #default="scope">
						{{ scope.row.applicantLabel || scope.row.applicant }}
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="备注" width="200" align="left"
					show-overflow-tooltip></el-table-column>
				<el-table-column fixed="right" label="详情/状态" width="150" align="center">
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

		<!-- 查看详情 Dialog -->
		<el-dialog :modal="false" :modal-penetrable="true" v-model="viewDialogVisible" title="日常费用付款详情"
			:close-on-click-modal="false" style="width: 75%;" @close="closeViewDialog">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-descriptions :column="2" border>
						<el-descriptions-item label="申请单号">
							<span>{{ viewForm.applicationNumber || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="申请日期">
							<span>{{ formatDate(viewForm.applicationDate) || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="付款类别">
							<span>{{ viewForm.paymentCategoryLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="款项名称">
							<span>{{ viewForm.paymentNameLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="收款单位">
							<span>{{ viewForm.payeeName || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="开户银行">
							<span>{{ viewForm.bankName || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="银行账号">
							<span>{{ viewForm.bankAccount || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="我方公司">
							<span>{{ viewForm.ourCompanyLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="币种">
							<span>{{ viewForm.currencyCodeLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="申请金额">
							<span>{{ formatAmount(viewForm.totalAmount) || '0.00' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="相关客户">
							<span>{{ viewForm.relatedCustomerLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="申请人">
							<span>{{ viewForm.applicantLabel || '-' }}</span>
						</el-descriptions-item>
						<el-descriptions-item label="备注说明" :span="2">
							<div style="white-space: pre-wrap; word-break: break-word;">
								{{ viewForm.remarks || '-' }}
							</div>
						</el-descriptions-item>
					</el-descriptions>
				</el-collapse-item>
			</el-collapse>

			<template #footer>
				<span class="dialog-footer">
					<el-button @click="closeViewDialog">关闭</el-button>
					<el-button type="success" v-if="canPay" @click="handlePay">
						付款
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, getCurrentInstance, toRefs, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import request from '@/utils/request'

// 获取当前实例
const proxy = getCurrentInstance()?.proxy

// 字典数据
const state = reactive({
	optionss: {
		hr_payment_category: [], // 付款类别
		hr_ourcompany: [], // 我方公司
		hr_daily_expenses: [], // 日常费用款项名称
		sql_all_user: [], // 申请人
		sql_hr_customer_abbreviation: [], // 客户简称
		hr_currency_code: [] // 币种
	}
})
const { optionss } = toRefs(state)

// Dialog 相关
const viewDialogVisible = ref(false)
const basicInfoCollapseActive = ref(['basicInfo'])
const currentPaymentRequestId = ref(0)

// 查看表单数据
const viewForm = reactive({
	applicationNumber: '',
	applicationDate: '',
	paymentCategory: '',
	paymentCategoryLabel: '',
	paymentName: '',
	paymentNameLabel: '',
	payeeName: '',
	bankName: '',
	bankAccount: '',
	ourCompany: '',
	ourCompanyLabel: '',
	currencyCode: '',
	currencyCodeLabel: '',
	totalAmount: '',
	relatedCustomer: '',
	relatedCustomerLabel: '',
	applicant: '',
	applicantLabel: '',
	remarks: '',
	reviewStatus: ''
})

// 判断是否可以付款（已审批通过且未付款）
const canPay = computed(() => {
	const status = viewForm.reviewStatus
	return String(status) === '2' || status === '已批准'
})

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
			if (tableData.value.length > 0 && optionss.value.hr_payment_category && optionss.value.hr_ourcompany && optionss.value.hr_daily_expenses && optionss.value.sql_all_user && optionss.value.sql_hr_customer_abbreviation) {
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
					// 映射申请人
					if (element.applicant) {
						const applicant = optionss.value.sql_all_user.find((item: any) => item.dictValue == element.applicant)
						if (applicant) {
							element.applicantLabel = applicant.dictLabel
						}
					}
					// 映射相关客户
					if (element.relatedCustomer) {
						const customer = optionss.value.sql_hr_customer_abbreviation.find((item: any) => item.dictValue == element.relatedCustomer.toString())
						if (customer) {
							element.relatedCustomerLabel = customer.dictLabel
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

// 查看详情
const handleView = async (row: any) => {
	currentPaymentRequestId.value = row.id
	try {
		const response = await request({
			url: 'PaymentRequest/GetPaymentRequestDetailsByID/GetDetails',
			method: 'GET',
			params: {
				id: row.id
			}
		})

		if (response && (response as any).code === 200) {
			const data = (response as any).data.paymentRequest

			// 填充表单数据
			viewForm.applicationNumber = data.applicationNumber || ''
			viewForm.applicationDate = data.applicationDate || ''
			viewForm.paymentCategory = data.paymentCategory?.toString() || ''
			viewForm.paymentName = data.paymentName?.toString() || ''
			viewForm.payeeName = data.payeeName || ''
			viewForm.bankName = data.bankName || ''
			viewForm.bankAccount = data.bankAccount || ''
			viewForm.ourCompany = data.ourCompany?.toString() || ''
			viewForm.currencyCode = data.currencyCode?.toString() || ''
			viewForm.totalAmount = data.totalAmount || ''
			viewForm.relatedCustomer = data.relatedCustomer?.toString() || ''
			viewForm.applicant = data.applicant?.toString() || ''
			viewForm.remarks = data.remark || ''
			viewForm.reviewStatus = data.reviewStatus || ''

			// 映射字典标签
			if (viewForm.paymentCategory) {
				const category = optionss.value.hr_payment_category.find(
					(item: any) => item.dictValue == viewForm.paymentCategory
				)
				viewForm.paymentCategoryLabel = category?.dictLabel || ''
			}

			if (viewForm.paymentName) {
				const paymentName = optionss.value.hr_daily_expenses.find(
					(item: any) => item.dictValue == viewForm.paymentName
				)
				viewForm.paymentNameLabel = paymentName?.dictLabel || ''
			}

			if (viewForm.ourCompany) {
				const company = optionss.value.hr_ourcompany.find(
					(item: any) => item.dictValue == viewForm.ourCompany || item.dictLabel == viewForm.ourCompany
				)
				viewForm.ourCompanyLabel = company?.dictLabel || viewForm.ourCompany
			}

			if (viewForm.currencyCode) {
				const currency = optionss.value.hr_currency_code.find(
					(item: any) => item.dictValue == viewForm.currencyCode
				)
				viewForm.currencyCodeLabel = currency?.dictLabel || ''
			}

			if (viewForm.relatedCustomer) {
				const customer = optionss.value.sql_hr_customer_abbreviation.find(
					(item: any) => item.dictValue == viewForm.relatedCustomer.toString()
				)
				viewForm.relatedCustomerLabel = customer?.dictLabel || ''
			}

			if (viewForm.applicant) {
				const applicant = optionss.value.sql_all_user.find(
					(item: any) => item.dictValue == viewForm.applicant
				)
				viewForm.applicantLabel = applicant?.dictLabel || ''
			}

			viewDialogVisible.value = true
		} else {
			ElMessage.error((response as any)?.msg || '加载详情失败')
		}
	} catch (error) {
		console.error('加载详情失败:', error)
		ElMessage.error('加载详情失败')
	}
}

// 关闭查看对话框
const closeViewDialog = () => {
	viewDialogVisible.value = false
	currentPaymentRequestId.value = 0
	// 重置表单
	Object.keys(viewForm).forEach(key => {
		if (typeof viewForm[key as keyof typeof viewForm] === 'string') {
			viewForm[key as keyof typeof viewForm] = '' as any
		} else {
			viewForm[key as keyof typeof viewForm] = '' as any
		}
	})
}

// 付款
const handlePay = async () => {
	try {
		await ElMessageBox.confirm('确定要付款吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		})

		// 调用付款 API
		const response = await request({
			url: 'PaymentRequest/ProcessPayment/ProcessPayment',
			method: 'GET',
			params: {
				paymentRequestId: currentPaymentRequestId.value
			}
		})

		if (response && (response as any).code === 200) {
			ElMessage.success('付款成功')
			closeViewDialog()
			loadData() // 刷新列表
		} else {
			ElMessage.error((response as any)?.msg || '付款失败')
		}
	} catch (error: any) {
		if (error !== 'cancel') {
			console.error('付款失败:', error)
			ElMessage.error('付款失败')
		}
	}
}


// 加载字典数据
const loadDictData = async () => {
	try {
		const dictParams = [
			{ dictType: 'hr_payment_category' },
			{ dictType: 'hr_ourcompany' },
			{ dictType: 'hr_daily_expenses' },
			{ dictType: 'sql_all_user' },
			{ dictType: 'sql_hr_customer_abbreviation' },
			{ dictType: 'hr_currency_code' }
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
