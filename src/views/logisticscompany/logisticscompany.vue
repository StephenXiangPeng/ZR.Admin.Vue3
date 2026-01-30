<template>
	<div>
		<!-- 物流公司表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="handleAdd" size="default"
								v-hasPermi="['huirong:logisticscompany:add']">新增物流公司</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
					<el-col :span="4">
						<el-input v-model="queryParams.simpleCompanyName" clearable placeholder="请输入公司简称"
							size="default" />
					</el-col>
					<el-col :span="4">
						<el-select v-model="queryParams.companyType" placeholder="请选择公司类型" clearable style="width: 100%"
							size="default">
							<el-option v-for="dict in state.optionss.hr_logisticscompany_type" :key="dict.dictValue"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="handleQuery" size="default">查询</el-button>
							<el-button @click="resetQuery" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table v-loading="loading" :data="logisticsCompanyList" @selection-change="handleSelectionChange"
				style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column label="ID" align="center" prop="id" v-if="false" />
				<el-table-column label="公司简称" align="center" prop="simpleCompanyName" />
				<el-table-column label="公司全称" align="center" prop="companyName" />
				<el-table-column label="公司类型" align="center" prop="companyType">
					<template #default="scope">
						<dict-tag :options="state.optionss.hr_logisticscompany_type" :value="scope.row.companyType" />
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="200">
					<template #default="scope">
						<el-button type="text" size="small" icon="View" @click="handleDetail(scope.row)">详情</el-button>
						<el-button type="text" size="small" icon="Edit" @click="handleUpdate(scope.row)"
							v-hasPermi="['huirong:logisticscompany:edit']">修改</el-button>
						<el-button type="text" size="small" icon="Delete" @click="handleDelete(scope.row)"
							v-hasPermi="['huirong:logisticscompany:remove']">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" :current-page="queryParams.pageNum"
				:page-size="queryParams.pageSize" :total="total" background layout="prev, pager, next"
				style="margin-top: 5px;" />
		</div>

		<!-- 添加或修改物流公司对话框 -->
		<el-dialog :modal="false" :modal-penetrable="true" :title="title" v-model="open" width="75%"
			:close-on-click-modal=false>
			<el-form ref="logisticsCompanyRef" :model="form" :rules="rules" label-width="120px">
				<!-- 基本信息 -->
				<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
					<el-collapse-item title="基本信息" name="basicInfo">
						<template #title>
							<span style="font-size: 18px; font-weight: bold; color: #000000;">基本信息</span>
						</template>
						<el-row>
							<el-col :span="6">
								<el-form-item label="公司简称" prop="simpleCompanyName">
									<el-input v-model="form.simpleCompanyName" placeholder="请输入公司简称"
										style="width: 300px" size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="公司全称" prop="companyName">
									<el-input v-model="form.companyName" placeholder="请输入公司全称" style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="公司类型" prop="companyType">
									<el-select v-model="form.companyType" placeholder="请选择公司类型" style="width: 300px"
										size="default" clearable>
										<el-option v-for="dict in state.optionss.hr_logisticscompany_type"
											:key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="详细地址" prop="address">
									<el-input v-model="form.address" placeholder="请输入详细地址" style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="公司税号" prop="companyTaxNumber">
									<el-input v-model="form.companyTaxNumber" placeholder="请输入公司税号" style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="备注" prop="remark">
									<el-input v-model="form.remark" type="textarea" placeholder="请输入备注"
										style="width: 300px" size="default" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-collapse-item>
				</el-collapse>

				<!-- 联系人信息 -->
				<el-collapse v-model="contactInfoCollapseActive" style="margin-bottom: 20px;">
					<el-collapse-item title="联系人信息" name="contactInfo">
						<template #title>
							<span style="font-size: 18px; font-weight: bold; color: #000000;">联系人信息</span>
						</template>
						<div class="mb10">
							<el-button type="primary" icon="Plus" @click="addContact" size="default">添加联系人</el-button>
						</div>
						<el-table :data="contactsTableData" style="width: 100%; table-layout: fixed;" stripe
							:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
							:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
							<el-table-column label="联系人姓名" prop="name">
								<template #default="scope">
									<el-input v-model="scope.row.name" placeholder="请输入联系人姓名" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="电话号码" prop="phone">
								<template #default="scope">
									<el-input v-model="scope.row.phone" placeholder="请输入电话号码" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="电子邮件" prop="email">
								<template #default="scope">
									<el-input v-model="scope.row.email" placeholder="请输入电子邮件" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="备注" prop="remark">
								<template #default="scope">
									<el-input v-model="scope.row.remark" placeholder="请输入备注" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="操作" width="100">
								<template #default="scope">
									<el-button type="danger" icon="Delete" circle size="small"
										@click="removeContact(scope.$index)" />
								</template>
							</el-table-column>
						</el-table>
					</el-collapse-item>
				</el-collapse>

				<!-- 银行账号信息 -->
				<el-collapse v-model="bankAccountCollapseActive" style="margin-bottom: 20px;">
					<el-collapse-item title="银行账号信息" name="bankAccountInfo">
						<template #title>
							<span style="font-size: 18px; font-weight: bold; color: #000000;">银行账号信息</span>
						</template>
						<div class="mb10">
							<el-button type="primary" icon="Plus" @click="addBankAccount"
								size="default">添加银行账号</el-button>
						</div>
						<el-table :data="bankAccountTableData" style="width: 100%; table-layout: fixed;" stripe
							:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
							:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
							<el-table-column label="开户名称" prop="bankAccountName">
								<template #default="scope">
									<el-input v-model="scope.row.bankAccountName" placeholder="请输入开户名称"
										size="default" />
								</template>
							</el-table-column>
							<el-table-column label="开户银行" prop="bank">
								<template #default="scope">
									<el-input v-model="scope.row.bank" placeholder="请输入开户银行" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="银行账号" prop="bankAccountNumber">
								<template #default="scope">
									<el-input v-model="scope.row.bankAccountNumber" placeholder="请输入银行账号"
										size="default" />
								</template>
							</el-table-column>
							<el-table-column label="银行地址" prop="bankAddress">
								<template #default="scope">
									<el-input v-model="scope.row.bankAddress" placeholder="请输入银行地址" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="备注" prop="remark">
								<template #default="scope">
									<el-input v-model="scope.row.remark" placeholder="请输入备注" size="default" />
								</template>
							</el-table-column>
							<el-table-column label="操作" width="100">
								<template #default="scope">
									<el-button type="danger" icon="Delete" circle size="small"
										@click="removeBankAccount(scope.$index)" />
								</template>
							</el-table-column>
						</el-table>
					</el-collapse-item>
				</el-collapse>
			</el-form>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="submitForm">确 定</el-button>
					<el-button @click="cancel">取 消</el-button>
				</div>
			</template>
		</el-dialog>

		<!-- 查看物流公司详情对话框 -->
		<el-dialog title="物流公司详情" v-model="detailOpen" width="75%" append-to-body>
			<el-descriptions :column="2" border>
				<el-descriptions-item label="公司简称">{{ detailForm.simpleCompanyName }}</el-descriptions-item>
				<el-descriptions-item label="公司全称">{{ detailForm.companyName }}</el-descriptions-item>
				<el-descriptions-item label="公司类型">
					<dict-tag :options="state.optionss.hr_logisticscompany_type" :value="detailForm.companyType" />
				</el-descriptions-item>
				<el-descriptions-item label="详细地址">{{ detailForm.address || '-' }}</el-descriptions-item>
				<el-descriptions-item label="公司税号">{{ detailForm.companyTaxNumber || '-' }}</el-descriptions-item>
				<el-descriptions-item label="备注">{{ detailForm.remark || '-' }}</el-descriptions-item>
			</el-descriptions>

			<div style="margin-top: 20px;">
				<div style="font-weight: bold; margin-bottom: 10px;">联系人信息</div>
				<el-table :data="detailContacts" style="width: 100%; table-layout: fixed;" stripe
					:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
					:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
					<el-table-column label="联系人姓名" prop="name" />
					<el-table-column label="电话号码" prop="phoneNumber" />
					<el-table-column label="电子邮件" prop="email" />
					<el-table-column label="备注" prop="remark" />
				</el-table>
			</div>

			<div style="margin-top: 20px;">
				<div style="font-weight: bold; margin-bottom: 10px;">银行账号信息</div>
				<el-table :data="detailBankAccounts" style="width: 100%; table-layout: fixed;" stripe
					:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
					:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
					<el-table-column label="开户名称" prop="bankAccountName" />
					<el-table-column label="开户银行" prop="bank" />
					<el-table-column label="银行账号" prop="bankAccountNumber" />
					<el-table-column label="银行地址" prop="bankAddress" />
					<el-table-column label="备注" prop="remark" />
				</el-table>
			</div>

			<div style="margin-top: 20px;">
				<div style="font-weight: bold; margin-bottom: 10px;">财务</div>
				<el-table v-loading="financeLedgerLoading" :data="financeLedgerPaginatedData"
					style="width: 100%; table-layout: fixed;" stripe
					:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
					:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
					<el-table-column prop="date" label="日期" width="130" align="center" />
					<el-table-column prop="summary" label="摘要/事项" width="150" align="center" />

					<el-table-column label="收支" align="center">
						<el-table-column prop="income.rmb" label="人民币" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.income.rmb > 0">{{ formatFinanceAmount(scope.row.income.rmb)
								}}</span>
							</template>
						</el-table-column>
						<el-table-column prop="income.usd" label="美元" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.income.usd > 0">{{ formatFinanceAmount(scope.row.income.usd)
								}}</span>
							</template>
						</el-table-column>
						<el-table-column prop="income.eur" label="欧元" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.income.eur > 0">{{ formatFinanceAmount(scope.row.income.eur)
								}}</span>
							</template>
						</el-table-column>
					</el-table-column>

					<el-table-column label="支出" align="center">
						<el-table-column prop="expenditure.rmb" label="人民币" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.expenditure.rmb > 0">{{
									formatFinanceAmount(scope.row.expenditure.rmb)
								}}</span>
							</template>
						</el-table-column>
						<el-table-column prop="expenditure.usd" label="美元" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.expenditure.usd > 0">{{
									formatFinanceAmount(scope.row.expenditure.usd)
								}}</span>
							</template>
						</el-table-column>
						<el-table-column prop="expenditure.eur" label="欧元" width="120" align="right">
							<template #default="scope">
								<span v-if="scope.row.expenditure.eur > 0">{{
									formatFinanceAmount(scope.row.expenditure.eur)
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
				<el-pagination @current-change="handleFinanceLedgerPageChange" :current-page="financeLedgerCurrentPage"
					:page-size="financeLedgerPageSize" :total="financeLedgerTotalRecords" background
					layout="prev, pager, next, total" style="margin-top: 5px;" />
			</div>

			<template #footer>
				<div class="dialog-footer">
					<el-button @click="detailOpen = false" size="default">关 闭</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, toRefs, onMounted, getCurrentInstance, nextTick, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance } from 'element-plus'
import {
	listLogisticsCompany,
	getLogisticsCompany,
	addLogisticsCompany,
	updateLogisticsCompany,
	delLogisticsCompany,
	exportLogisticsCompany,
	getLogisticsCompanyContacts,
	getLogisticsCompanyBankAccounts
} from '@/api/huirong/logisticscompany'
import { getFinancialGeneralLedgerData } from '@/api/finance'

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_logisticscompany_type: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_logisticscompany_type' }] as any
(proxy as any).getDicts(dictParams).then((response: any) => {
	response.data.forEach((element: any) => {
		state.optionss[element.dictType] = element.list
	});
	getList();
})
/*动态下拉框end*/


// 使用FormInstance类型
const logisticsCompanyRef = ref<FormInstance>()

// 物流公司表格数据
const logisticsCompanyList = ref([])
// 选中数组
const ids = ref([])
// 非单个禁用
const single = ref(true)
// 非多个禁用
const multiple = ref(true)
// 显示搜索条件
const showSearch = ref(true)
// 总条数
const total = ref(0)
// 弹出层标题
const title = ref('')
// 是否显示弹出层
const open = ref(false)
// 移除activeTab，不再需要tab切换
// 查询参数
const queryParams = ref({
	pageNum: 1,
	pageSize: 10,
	simpleCompanyName: undefined,
	companyName: undefined,
	companyType: undefined
})
// 表单参数
const form = ref({
	id: undefined,
	simpleCompanyName: undefined,
	companyName: undefined,
	companyType: undefined,
	address: undefined,
	openAccountBank: '', // 后端接口需要，但不在界面显示
	bankAccount: '', // 后端接口需要，但不在界面显示
	companyTaxNumber: undefined,
	remark: undefined,
	contacts: []
})

// 表单校验
const rules = ref({
	simpleCompanyName: [{ required: true, message: '公司简称不能为空', trigger: 'blur' }],
	companyName: [{ required: true, message: '公司全称不能为空', trigger: 'blur' }],
	companyType: [{ required: true, message: '公司类型不能为空', trigger: 'change' }]
})


// 加载状态
const loading = ref(false)

// 联系人表格数据
const contactsTableData = ref([]);

// 银行账号表格数据
const bankAccountTableData = ref([]);

// 折叠面板状态管理
const basicInfoCollapseActive = ref(['basicInfo'])
const contactInfoCollapseActive = ref(['contactInfo'])
const bankAccountCollapseActive = ref(['bankAccountInfo'])

// 是否显示详情弹出层
const detailOpen = ref(false)
// 详情表单参数
const detailForm = ref({
	id: undefined,
	simpleCompanyName: undefined,
	companyName: undefined,
	companyType: undefined,
	address: undefined,
	openAccountBank: undefined,
	bankAccount: undefined,
	companyTaxNumber: undefined,
	remark: undefined,
	contacts: []
})
// 详情联系人数据
const detailContacts = ref([])
// 详情银行账号数据
const detailBankAccounts = ref([])

interface FinanceAmount {
	rmb: number
	usd: number
	eur: number
}

interface FinanceRemarks {
	customerOrSupplier: string
	contractOrRemark: string
}

interface FinanceLedgerItem {
	date: string
	summary: string
	income: FinanceAmount
	expenditure: FinanceAmount
	remarks: FinanceRemarks
}

interface FinanceApiRow {
	date: string
	summary: string
	incomeCny: number
	incomeUsd: number
	incomeEur: number
	expenseCny: number
	expenseUsd: number
	expenseEur: number
	customerOrPayee: string
	contractOrRemark: string
}

const financeLedgerLoading = ref(false)
const financeLedgerData = ref<FinanceLedgerItem[]>([])
const financeLedgerCurrentPage = ref(1)
const financeLedgerPageSize = ref(10)
const financeLedgerTotalRecords = computed(() => financeLedgerData.value.length)
const financeLedgerPaginatedData = computed(() => {
	const start = (financeLedgerCurrentPage.value - 1) * financeLedgerPageSize.value
	const end = start + financeLedgerPageSize.value
	return financeLedgerData.value.slice(start, end)
})

/** 查询物流公司列表 */
function getList() {
	loading.value = true
	listLogisticsCompany(queryParams.value).then(response => {
		// 适配后端返回的数据结构
		logisticsCompanyList.value = response.data.result || []
		total.value = response.data.totalNum || 0
		loading.value = false
	})
}

/** 取消按钮 */
function cancel() {
	open.value = false
	reset()
}

/** 表单重置 */
function reset() {
	form.value = {
		id: undefined,
		simpleCompanyName: undefined,
		companyName: undefined,
		companyType: undefined,
		address: undefined,
		openAccountBank: '', // 后端接口需要，但不在界面显示
		bankAccount: '', // 后端接口需要，但不在界面显示
		companyTaxNumber: undefined,
		remark: undefined,
		contacts: []
	}
	// 清空联系人表格
	contactsTableData.value = [];
	// 清空银行账号表格
	bankAccountTableData.value = [];
}

/** 搜索按钮操作 */
function handleQuery() {
	queryParams.value.pageNum = 1
	getList()
}

/** 重置按钮操作 */
function resetQuery() {
	queryParams.value = {
		pageNum: 1,
		pageSize: 10,
		simpleCompanyName: undefined,
		companyName: undefined,
		companyType: undefined
	}
	handleQuery()
}

/** 分页处理 */
function handlePageChange(page) {
	queryParams.value.pageNum = page
	getList()
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
	ids.value = selection.map(item => item.id)
	single.value = selection.length !== 1
	multiple.value = !selection.length
}

/** 新增按钮操作 */
function handleAdd() {
	reset()
	open.value = true
	title.value = '添加物流公司'
}

/** 修改按钮操作 */
function handleUpdate(row) {
	reset()
	const id = row.id || ids.value[0]
	// 获取物流公司详情
	getLogisticsCompany(id).then(response => {
		if (response && response.data) {
			// 确保 companyType 是字符串类型
			if (response.data.companyType !== null && response.data.companyType !== undefined) {
				response.data.companyType = response.data.companyType.toString();
			}
			Object.assign(form.value, response.data)
			// 清空联系人表格
			contactsTableData.value = []
			// 清空银行账号表格
			bankAccountTableData.value = []
			// 获取联系人列表
			getLogisticsCompanyContacts(id).then(contactResponse => {
				if (contactResponse && contactResponse.data) {
					// 将联系人数据添加到表格
					contactsTableData.value = contactResponse.data.map(contact => ({
						id: contact.id,
						name: contact.name,
						phone: contact.phoneNumber, // 注意字段名可能不同
						email: contact.email || '',
						remark: contact.remark || ''
					})) || []
				} else {
					ElMessage.warning('获取联系人数据失败')
				}
			}).catch(() => {
				ElMessage.warning('获取联系人数据失败')
			})
			// 获取银行账号列表
			getLogisticsCompanyBankAccounts(id).then(bankResponse => {
				if (bankResponse && bankResponse.data) {
					// 将银行账号数据添加到表格
					bankAccountTableData.value = bankResponse.data.map(bank => ({
						id: bank.id,
						bankAccountName: bank.bankAccountName,
						bank: bank.bank,
						bankAccountNumber: bank.bankAccountNumber,
						bankAddress: bank.bankAddress || '',
						remark: bank.remark || ''
					})) || []
				} else {
					ElMessage.warning('获取银行账号数据失败')
				}
			}).catch(() => {
				ElMessage.warning('获取银行账号数据失败')
			})
			open.value = true
			title.value = '修改物流公司'
		} else {
			ElMessage.error('获取物流公司详情失败')
		}
	}).catch(() => {
		ElMessage.error('获取物流公司详情失败')
	})
}

/** 提交按钮 */
function submitForm() {
	logisticsCompanyRef.value.validate(valid => {
		if (valid) {
			// 检查联系人是否为空
			if (contactsTableData.value.length === 0) {
				ElMessage.warning('请至少添加一个联系人');
				return;
			}

			// 检查联系人必填字段
			for (let i = 0; i < contactsTableData.value.length; i++) {
				const contact = contactsTableData.value[i];
				if (!contact.name || !contact.phone) {
					ElMessage.warning(`第${i + 1}个联系人的姓名和电话不能为空`);
					return;
				}
			}

			// 准备提交的数据
			const submitData = {
				...form.value,
				// 联系人数据字段名与后端匹配
				Contacts: contactsTableData.value.map(contact => ({
					Id: contact.id,
					LogisticsCompanyId: form.value.id, // 如果是修改，需要保留关联ID
					Name: contact.name,
					Phone: contact.phone,
					Email: contact.email,
					Remark: contact.remark
				})),
				// 银行账号数据字段名与后端匹配
				BankAccounts: bankAccountTableData.value.map(bank => ({
					Id: bank.id,
					LogisticsCompanyID: form.value.id, // 如果是修改，需要保留关联ID
					Bank_account_name: bank.bankAccountName,
					Bank: bank.bank,
					Bank_account_number: bank.bankAccountNumber,
					Bank_address: bank.bankAddress || '', // 添加银行地址字段
					Remark: bank.remark
				}))
			};

			if (form.value.id) {
				// 修改操作
				updateLogisticsCompany(submitData).then(response => {
					if (response && response.data) {
						ElMessage.success('修改成功')
						open.value = false
						getList()
					} else {
						ElMessage.error('修改失败')
					}
				}).catch(() => {
					ElMessage.error('修改失败，请稍后重试')
				})
			} else {
				// 新增操作
				addLogisticsCompany(submitData).then(response => {
					if (response && response.data) {
						ElMessage.success('新增成功')
						open.value = false
						getList()
					} else {
						ElMessage.error('新增失败')
					}
				}).catch(() => {
					ElMessage.error('新增失败，请稍后重试')
				})
			}
		}
	})
}

/** 删除按钮操作 */
function handleDelete(row) {
	const idsToDelete = row.id || ids.value
	ElMessageBox.confirm('是否确认删除物流公司数据?', '警告', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		return delLogisticsCompany(idsToDelete)
	}).then(() => {
		getList()
		ElMessage.success('删除成功')
	}).catch(() => { })
}

/** 导出按钮操作 */
function handleExport() {
	exportLogisticsCompany(queryParams.value)
}

// 获取联系人提示信息
function getContactsTooltip(contacts) {
	if (!contacts || contacts.length === 0) return '';
	return contacts.map(contact => `${contact.name}: ${contact.phone}`).join('<br>');
}

// 添加联系人
function addContact() {
	contactsTableData.value.push({
		id: undefined,
		name: '',
		phone: '',
		email: '',
		remark: ''
	});
}

// 移除联系人
function removeContact(index) {
	contactsTableData.value.splice(index, 1);
}

// 添加银行账号
function addBankAccount() {
	bankAccountTableData.value.push({
		id: undefined,
		bankAccountName: '',
		bank: '',
		bankAccountNumber: '',
		bankAddress: '',
		remark: ''
	});
}

// 移除银行账号
function removeBankAccount(index) {
	bankAccountTableData.value.splice(index, 1);
}

const formatFinanceDate = (dateStr: string): string => {
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

const formatFinanceAmount = (amount: number): string => {
	if (!amount) return ''
	return Number(amount).toFixed(2)
}

const parseFinanceRemarks = (row: FinanceApiRow): FinanceRemarks => {
	return {
		customerOrSupplier: row.customerOrPayee || '-',
		contractOrRemark: row.contractOrRemark || '-'
	}
}

const transformFinanceLedgerData = (apiRows: FinanceApiRow[]): FinanceLedgerItem[] => {
	return apiRows.map(row => ({
		date: formatFinanceDate(row.date),
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
		remarks: parseFinanceRemarks(row)
	}))
}

const handleFinanceLedgerPageChange = (page: number) => {
	financeLedgerCurrentPage.value = page
}

const loadLogisticsFinanceLedger = async (companyId: number, companyLabel: string) => {
	financeLedgerLoading.value = true
	try {
		const response = await getFinancialGeneralLedgerData(null, companyId ? String(companyId) : null)
		if (response && response.code === 200 && response.data && response.data.rows) {
			const filteredRows = companyLabel
				? response.data.rows.filter((row: FinanceApiRow) =>
					(row.customerOrPayee || '').includes(companyLabel)
				)
				: response.data.rows
			financeLedgerData.value = transformFinanceLedgerData(filteredRows)
			financeLedgerCurrentPage.value = 1
		} else {
			financeLedgerData.value = []
		}
	} catch (error) {
		console.error('加载财务总账数据失败:', error)
		financeLedgerData.value = []
	} finally {
		financeLedgerLoading.value = false
	}
}

/** 查看详情按钮操作 */
function handleDetail(row) {
	detailForm.value = { ...row }
	detailContacts.value = [] // 先清空联系人数据
	detailBankAccounts.value = [] // 先清空银行账号数据
	loadLogisticsFinanceLedger(row.id, row.simpleCompanyName || row.companyName || '')
	// 获取联系人列表
	getLogisticsCompanyContacts(row.id).then(response => {
		if (response && response.data) {
			detailContacts.value = response.data || []
		} else {
			ElMessage.error('获取联系人数据失败')
		}
	}).catch(() => {
		ElMessage.error('获取联系人数据失败')
	})
	// 获取银行账号列表
	getLogisticsCompanyBankAccounts(row.id).then(response => {
		if (response && response.data) {
			detailBankAccounts.value = response.data || []
		} else {
			ElMessage.error('获取银行账号数据失败')
		}
		detailOpen.value = true
	}).catch(() => {
		detailOpen.value = true
		ElMessage.error('获取银行账号数据失败')
	})
}

onMounted(() => {
	getList()
})
</script>

<style scoped>
.mb10 {
	margin-bottom: 10px;
}

/* 创建合同和查看合同详情dialog中的表单组件间距减少一半 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

.el-dialog .el-row {
	margin-bottom: 2.5px !important;
}

/* 确保表单项标签宽度一致 */
.el-dialog .el-form-item__label {
	width: 120px !important;
	text-align: right;
	padding-right: 12px;
}

/* 表单项内容区域 */
.el-dialog .el-form-item__content {
	margin-left: 120px !important;
}
</style>