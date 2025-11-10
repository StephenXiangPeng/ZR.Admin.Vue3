<template>
	<div>
		<!-- 付款列表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
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
					<el-col :span="4" style="text-align: right;">
						<el-button type="primary" plain @click="handleSearch()" size="default">查询</el-button>
						<el-button @click="handleReset()" size="default">重置</el-button>
					</el-col>
				</el-row>
				<el-row :gutter="15">

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
				<el-table-column prop="relatedContracts" label="销售合同" width="150" align="left"></el-table-column>
				<el-table-column prop="relatedShippingContracts" label="出运编号" width="150"
					align="left"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位" width="180" align="left"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="120" align="left">
					<template #default="scope">
						{{ scope.row.paymentNameLabel || scope.row.paymentName }}
					</template>
				</el-table-column>
				<el-table-column prop="totalAmount" label="付款金额" width="130" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.totalAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="120" align="left">
					<template #default="scope">
						{{ scope.row.ourCompanyLabel || scope.row.ourCompany }}
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
				:current-page="currentPage" :page-size="pageSize" :total="totalItems" :page-sizes="[10, 20, 50, 100]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>

		<!-- 查看详情 Dialog -->
		<el-dialog :modal="false" :modal-penetrable="true" v-model="viewDialogVisible" title="业务费用付款详情"
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
						<el-descriptions-item label="关联合同" v-if="viewForm.relatedContracts">
							<span>{{ viewForm.relatedContracts || '-' }}</span>
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

			<!-- 单据信息 -->
			<el-collapse v-model="sampleReceiptCollapseActive" style="margin-bottom: 20px;" v-show="showSampleReceipt">
				<el-collapse-item title="单据信息" name="sampleReceipt">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">单据信息</span>
					</template>
					<el-table :data="sampleReceiptData" style="width: 100%;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="type" label="寄样/收样" width="100" align="center">
							<template #default="{ row }">
								<span>{{ row.type || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_or_Supplier" label="客户/供应商" width="110" align="center">
							<template #default="{ row }">
								<span>{{ row.customer_or_Supplier || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_ID" label="寄样对象" width="150" align="left">
							<template #default="{ row }">
								<span>{{ row.customer_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="waybill_Number" label="运单号" width="130" align="left">
							<template #default="{ row }">
								<span>{{ row.waybill_Number || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="express_Company" label="快件公司" width="150" align="left">
							<template #default="{ row }">
								<span>{{ row.express_Company || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="sample_Date" label="登记日期" width="110" align="center">
							<template #default="{ row }">
								<span>{{ formatDate(row.sample_Date) || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="payment_Method" label="付费方式" width="90" align="center">
							<template #default="{ row }">
								<span>{{ row.payment_Method || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="company_ID" label="我方公司" width="130" align="left">
							<template #default="{ row }">
								<span>{{ row.company_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="paid_Express_Fee" label="快件费" width="110" align="right">
							<template #default="{ row }">
								<span>{{ formatAmount(row.paid_Express_Fee) || '0.00' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedContractNumber" label="销售合同" width="150" align="left">
							<template #default="{ row }">
								<span>{{ row.relatedContractNumber || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedShippingNumber" label="出运编号" width="150" align="left">
							<template #default="{ row }">
								<span>{{ row.relatedShippingNumber || '' }}</span>
							</template>
						</el-table-column>
					</el-table>
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
		hr_business_expenses: [], // 业务费用款项名称
		sql_all_user: [], // 申请人
		hr_currency_code: [], // 币种
		hr_express_delivery_company: [], // 快递公司
		hr_express_payment_method: [], // 快递付费方式
		sql_hr_customer_abbreviation: [], // 客户简称
		sql_sale_contracts: [], // 销售合同
		sql_shippingdeliveries: [] // 出运编号
	}
})
const { optionss } = toRefs(state)

// Dialog 相关
const viewDialogVisible = ref(false)
const basicInfoCollapseActive = ref(['basicInfo'])
const sampleReceiptCollapseActive = ref(['sampleReceipt'])
const currentPaymentRequestId = ref(0)
const showSampleReceipt = ref(false)
const sampleReceiptData = ref<any[]>([])

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
	relatedContracts: '',
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
				PaymentCategory: 5 // 固定为5，表示业务费用付款
			}
		})

		const res = response as any
		if (res && res.code === 200) {
			// 处理返回的数据
			const result = res.data || {}
			tableData.value = result.result || []
			totalItems.value = result.totalNum || 0

			// 处理字典数据映射
			if (tableData.value.length > 0 && optionss.value.hr_payment_category && optionss.value.hr_ourcompany && optionss.value.hr_business_expenses && optionss.value.sql_all_user) {
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
						const paymentName = optionss.value.hr_business_expenses.find((item: any) => item.dictValue == element.paymentName.toString())
						if (paymentName) {
							element.paymentNameLabel = paymentName.dictLabel
						}
					}
					// 映射我方公司
					if (element.ourCompany) {
						const company = optionss.value.hr_ourcompany.find((item: any) => item.dictValue == element.ourCompany)
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
			viewForm.relatedContracts = data.relatedContracts || ''
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
				const paymentName = optionss.value.hr_business_expenses.find(
					(item: any) => item.dictValue == viewForm.paymentName
				)
				viewForm.paymentNameLabel = paymentName?.dictLabel || ''
			}

			if (viewForm.ourCompany) {
				const company = optionss.value.hr_ourcompany.find(
					(item: any) => item.dictValue == viewForm.ourCompany
				)
				viewForm.ourCompanyLabel = company?.dictLabel || ''
			}

			if (viewForm.currencyCode) {
				const currency = optionss.value.hr_currency_code.find(
					(item: any) => item.dictValue == viewForm.currencyCode
				)
				viewForm.currencyCodeLabel = currency?.dictLabel || ''
			}

			if (viewForm.applicant) {
				const applicant = optionss.value.sql_all_user.find(
					(item: any) => item.dictValue == viewForm.applicant
				)
				viewForm.applicantLabel = applicant?.dictLabel || ''
			}

			// 处理收寄样单据数据
			if ((response as any).data.sampleReceipt && (response as any).data.sampleReceipt.length > 0) {
				sampleReceiptData.value = (response as any).data.sampleReceipt.map((item: any) => {
					// 转换寄样/收样类型
					const type = item.type === 1 ? '寄样' : '收样'
					// 转换客户/供应商类型
					const customerOrSupplier = item.customer_or_Supplier === 1 ? '客户' : '供应商'
					// 转换客户ID为名称
					let customerIdLabel = ''
					if (item.customer_ID && item.customer_ID !== 0) {
						const customer = optionss.value.sql_hr_customer_abbreviation?.find(
							(c: any) => c.dictValue === item.customer_ID.toString()
						)
						customerIdLabel = customer ? customer.dictLabel : item.customer_ID.toString()
					}
					// 转换快递公司ID为名称（使用字典标签）
					let expressCompanyLabel = ''
					if (item.express_Company && item.express_Company !== 0) {
						const company = optionss.value.hr_express_delivery_company?.find(
							(c: any) => c.dictValue === item.express_Company.toString()
						)
						expressCompanyLabel = company ? company.dictLabel : ''
					}
					// 转换付费方式
					let paymentMethodLabel = ''
					if (item.payment_Method && item.payment_Method !== 0) {
						const method = optionss.value.hr_express_payment_method?.find(
							(m: any) => m.dictValue === item.payment_Method.toString()
						)
						paymentMethodLabel = method ? method.dictLabel : ''
					}
					// 转换我方公司
					let companyIdLabel = ''
					if (item.company_ID && item.company_ID !== 0) {
						const company = optionss.value.hr_ourcompany?.find(
							(c: any) => c.dictValue === item.company_ID.toString()
						)
						companyIdLabel = company ? company.dictLabel : ''
					}
					// 转换销售合同ID为合同编号（使用字典标签）
					let relatedContractNumber = ''
					if (item.relatedContractID && item.relatedContractID !== 0) {
						const contract = optionss.value.sql_sale_contracts?.find(
							(c: any) => c.dictValue === item.relatedContractID.toString()
						)
						relatedContractNumber = contract ? contract.dictLabel : (item.relatedContractNumber || '')
					}
					// 转换出运合同ID为合同编号（使用字典标签）
					let relatedShippingNumber = ''
					if (item.relatedShippingContractsID && item.relatedShippingContractsID !== 0) {
						const shipping = optionss.value.sql_shippingdeliveries?.find(
							(s: any) => s.dictValue === item.relatedShippingContractsID.toString()
						)
						relatedShippingNumber = shipping ? shipping.dictLabel : (item.relatedShippingNumber || '')
					}

					return {
						type,
						customer_or_Supplier: customerOrSupplier,
						customer_ID: customerIdLabel,
						waybill_Number: item.waybill_Number || '',
						express_Company: expressCompanyLabel,
						sample_Date: item.sample_Date || '',
						payment_Method: paymentMethodLabel,
						company_ID: companyIdLabel,
						paid_Express_Fee: item.paid_Express_Fee || 0,
						relatedContractNumber,
						relatedShippingNumber
					}
				})
				showSampleReceipt.value = true
			} else {
				sampleReceiptData.value = []
				showSampleReceipt.value = false
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
	showSampleReceipt.value = false
	sampleReceiptData.value = []
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
			{ dictType: 'hr_business_expenses' },
			{ dictType: 'sql_all_user' },
			{ dictType: 'hr_currency_code' },
			{ dictType: 'hr_express_delivery_company' },
			{ dictType: 'hr_express_payment_method' },
			{ dictType: 'sql_hr_customer_abbreviation' },
			{ dictType: 'sql_sale_contracts' },
			{ dictType: 'sql_shippingdeliveries' }
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
