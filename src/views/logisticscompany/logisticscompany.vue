<template>
	<div>
		<el-form :model="queryParams" ref="queryForm" :inline="true" v-show="showSearch" label-width="68px">
			<el-form-item label="公司简称" prop="simpleCompanyName">
				<el-input v-model="queryParams.simpleCompanyName" placeholder="请输入公司简称" clearable style="width: 240px"
					@keyup.enter="handleQuery" />
			</el-form-item>
			<el-form-item label="公司类型" prop="companyType">
				<el-select v-model="queryParams.companyType" placeholder="请选择公司类型" clearable style="width: 240px">
					<el-option v-for="dict in state.optionss.hr_logisticscompany_type" :key="dict.dictValue"
						:label="dict.dictLabel" :value="dict.dictValue" />
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
				<el-button icon="Refresh" @click="resetQuery">重置</el-button>
			</el-form-item>
		</el-form>

		<el-row :gutter="10" class="mb8">
			<el-col :span="1.5">
				<el-button type="primary" plain icon="Plus" @click="handleAdd"
					v-hasPermi="['huirong:logisticscompany:add']">新增</el-button>
			</el-col>
			<!-- <el-col :span="1.5">
				<el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate"
					v-hasPermi="['huirong:logisticscompany:edit']">修改</el-button>
			</el-col>
			<el-col :span="1.5">
				<el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete"
					v-hasPermi="['huirong:logisticscompany:remove']">删除</el-button>
			</el-col> -->
			<right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
		</el-row>

		<el-table v-loading="loading" :data="logisticsCompanyList" @selection-change="handleSelectionChange">
			<!-- <el-table-column type="selection" width="55" align="center" /> -->
			<el-table-column label="ID" align="center" prop="id" v-if="false" />
			<el-table-column label="公司简称" align="center" prop="simpleCompanyName" />
			<el-table-column label="公司全称" align="center" prop="companyName" />
			<el-table-column label="公司类型" align="center" prop="companyType">
				<template #default="scope">
					<dict-tag :options="state.optionss.hr_logisticscompany_type" :value="scope.row.companyType" />
				</template>
			</el-table-column>
			<el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="400">
				<template #default="scope">
					<el-button type="text" icon="View" @click="handleDetail(scope.row)">详情</el-button>
					<el-button type="text" icon="Edit" @click="handleUpdate(scope.row)"
						v-hasPermi="['huirong:logisticscompany:edit']">修改</el-button>
					<el-button type="text" icon="Delete" @click="handleDelete(scope.row)"
						v-hasPermi="['huirong:logisticscompany:remove']">删除</el-button>
				</template>
			</el-table-column>
		</el-table>

		<pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
			v-model:limit="queryParams.pageSize" @pagination="getList" />

		<!-- 添加或修改物流公司对话框 -->
		<el-dialog :title="title" v-model="open" width="800px" append-to-body>
			<el-form ref="logisticsCompanyRef" :model="form" :rules="rules" label-width="120px">
				<el-tabs v-model="activeTab">
					<el-tab-pane label="基本信息" name="basic">
						<el-row>
							<el-col :span="12">
								<el-form-item label="公司简称" prop="simpleCompanyName">
									<el-input v-model="form.simpleCompanyName" placeholder="请输入公司简称" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="公司全称" prop="companyName">
									<el-input v-model="form.companyName" placeholder="请输入公司全称" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="12">
								<el-form-item label="公司类型" prop="companyType">
									<el-select v-model="form.companyType" placeholder="请选择公司类型">
										<el-option v-for="dict in state.optionss.hr_logisticscompany_type"
											:key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="详细地址" prop="address">
									<el-input v-model="form.address" placeholder="请输入详细地址" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="12">
								<el-form-item label="开户银行" prop="openAccountBank">
									<el-input v-model="form.openAccountBank" placeholder="请输入开户银行" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="银行账号" prop="bankAccount">
									<el-input v-model="form.bankAccount" placeholder="请输入银行账号" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="12">
								<el-form-item label="公司税号" prop="companyTaxNumber">
									<el-input v-model="form.companyTaxNumber" placeholder="请输入公司税号" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="备注" prop="remark">
									<el-input v-model="form.remark" type="textarea" placeholder="请输入备注" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="联系人信息" name="contact">
						<div class="mb10">
							<el-button type="primary" icon="Plus" @click="addContact">添加联系人</el-button>
						</div>
						<el-table :data="contactsTableData" style="width: 100%">
							<el-table-column label="联系人姓名" prop="name">
								<template #default="scope">
									<el-input v-model="scope.row.name" placeholder="请输入联系人姓名" />
								</template>
							</el-table-column>
							<el-table-column label="电话号码" prop="phone">
								<template #default="scope">
									<el-input v-model="scope.row.phone" placeholder="请输入电话号码" />
								</template>
							</el-table-column>
							<el-table-column label="电子邮件" prop="email">
								<template #default="scope">
									<el-input v-model="scope.row.email" placeholder="请输入电子邮件" />
								</template>
							</el-table-column>
							<el-table-column label="备注" prop="remark">
								<template #default="scope">
									<el-input v-model="scope.row.remark" placeholder="请输入备注" />
								</template>
							</el-table-column>
							<el-table-column label="操作" width="100">
								<template #default="scope">
									<el-button type="danger" icon="Delete" circle
										@click="removeContact(scope.$index)" />
								</template>
							</el-table-column>
						</el-table>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" @click="submitForm">确 定</el-button>
					<el-button @click="cancel">取 消</el-button>
				</div>
			</template>
		</el-dialog>

		<!-- 查看物流公司详情对话框 -->
		<el-dialog title="物流公司详情" v-model="detailOpen" width="800px" append-to-body>
			<el-descriptions :column="2" border>
				<el-descriptions-item label="公司简称">{{ detailForm.simpleCompanyName }}</el-descriptions-item>
				<el-descriptions-item label="公司全称">{{ detailForm.companyName }}</el-descriptions-item>
				<el-descriptions-item label="公司类型">
					<dict-tag :options="state.optionss.hr_logisticscompany_type" :value="detailForm.companyType" />
				</el-descriptions-item>
				<el-descriptions-item label="详细地址">{{ detailForm.address || '-' }}</el-descriptions-item>
				<el-descriptions-item label="开户银行">{{ detailForm.openAccountBank || '-' }}</el-descriptions-item>
				<el-descriptions-item label="银行账号">{{ detailForm.bankAccount || '-' }}</el-descriptions-item>
				<el-descriptions-item label="公司税号">{{ detailForm.companyTaxNumber || '-' }}</el-descriptions-item>
				<el-descriptions-item label="备注">{{ detailForm.remark || '-' }}</el-descriptions-item>
			</el-descriptions>

			<div style="margin-top: 20px;">
				<div style="font-weight: bold; margin-bottom: 10px;">联系人信息</div>
				<el-table :data="detailContacts" style="width: 100%">
					<el-table-column label="联系人姓名" prop="name" />
					<el-table-column label="电话号码" prop="phoneNumber" />
					<el-table-column label="电子邮件" prop="email" />
					<el-table-column label="备注" prop="remark" />
				</el-table>
			</div>

			<template #footer>
				<div class="dialog-footer">
					<el-button @click="detailOpen = false">关 闭</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, toRefs, onMounted, getCurrentInstance, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance } from 'element-plus'
import {
	listLogisticsCompany,
	getLogisticsCompany,
	addLogisticsCompany,
	updateLogisticsCompany,
	delLogisticsCompany,
	exportLogisticsCompany,
	getLogisticsCompanyContacts
} from '@/api/huirong/logisticscompany'

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_logisticscompany_type: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_logisticscompany_type' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
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
// 当前激活的标签页
const activeTab = ref('basic')
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
	openAccountBank: undefined,
	bankAccount: undefined,
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
		openAccountBank: undefined,
		bankAccount: undefined,
		companyTaxNumber: undefined,
		remark: undefined,
		contacts: []
	}
	// 清空联系人表格
	contactsTableData.value = [];
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
	activeTab.value = 'basic'
}

/** 修改按钮操作 */
function handleUpdate(row) {
	reset()
	const id = row.id || ids.value[0]
	// 获取物流公司详情
	getLogisticsCompany(id).then(response => {
		if (response.code === 200 && response.data) {
			// 确保 companyType 是字符串类型
			if (response.data.companyType !== null && response.data.companyType !== undefined) {
				response.data.companyType = response.data.companyType.toString();
			}
			Object.assign(form.value, response.data)
			// 清空联系人表格
			contactsTableData.value = []
			// 获取联系人列表
			getLogisticsCompanyContacts(id).then(contactResponse => {
				if (contactResponse.code === 200) {
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
			open.value = true
			title.value = '修改物流公司'
			activeTab.value = 'basic'
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
				activeTab.value = 'contact';
				return;
			}

			// 检查联系人必填字段
			for (let i = 0; i < contactsTableData.value.length; i++) {
				const contact = contactsTableData.value[i];
				if (!contact.name || !contact.phone) {
					ElMessage.warning(`第${i + 1}个联系人的姓名和电话不能为空`);
					activeTab.value = 'contact';
					return;
				}
			}

			// 准备提交的数据
			const submitData = {
				...form.value,
				// 确保联系人数据字段名与后端匹配
				contacts: contactsTableData.value.map(contact => ({
					id: contact.id,
					logisticsCompanyId: form.value.id, // 如果是修改，需要保留关联ID
					name: contact.name,
					Phone: contact.phone, // 注意字段名可能需要转换
					email: contact.email,
					remark: contact.remark
				}))
			};

			if (form.value.id) {
				// 修改操作
				updateLogisticsCompany(submitData).then(response => {
					if (response.code === 200) {
						ElMessage.success('修改成功')
						open.value = false
						getList()
					} else {
						ElMessage.error(response.msg || '修改失败')
					}
				}).catch(() => {
					ElMessage.error('修改失败，请稍后重试')
				})
			} else {
				// 新增操作
				addLogisticsCompany(submitData).then(response => {
					if (response.code === 200) {
						ElMessage.success('新增成功')
						open.value = false
						getList()
					} else {
						ElMessage.error(response.msg || '新增失败')
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

/** 查看详情按钮操作 */
function handleDetail(row) {
	detailForm.value = { ...row }
	detailContacts.value = [] // 先清空联系人数据
	// 获取联系人列表
	getLogisticsCompanyContacts(row.id).then(response => {
		if (response.code === 200) {
			detailContacts.value = response.data || []
		} else {
			ElMessage.error('获取联系人数据失败')
		}
		detailOpen.value = true
	}).catch(() => {
		detailOpen.value = true
		ElMessage.error('获取联系人数据失败')
	})
	detailOpen.value = true
}

onMounted(() => {
	getList()
})
</script>

<style scoped>
.mb10 {
	margin-bottom: 10px;
}
</style>