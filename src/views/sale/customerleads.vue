<template>
	<div>
		<!-- 客户线索表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="createNewLead" size="default">新建线索</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div class="customer-search-area">
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-input v-model="clueNoInput" clearable placeholder="请输入线索编号" size="default" />
					</el-col>
					<el-col :span="4">
						<el-input v-model="clueEmailInput" clearable placeholder="请输入邮箱地址" size="default" />
					</el-col>
					<el-col :span="4">
						<el-input v-model="clueCustomerNameInput" clearable placeholder="请输入客户名称" size="default" />
					</el-col>
					<el-col :span="4">
						<el-input v-model="contactNameInput" clearable placeholder="请输入联系人" size="default" />
					</el-col>
					<el-col :span="4">
						<el-select filterable v-model="businessSelect" placeholder="请选择涉及业务" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select filterable v-model="nationSelect" placeholder="请选择国家" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
								:value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
				</el-row>
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-select filterable v-model="customerLevelSelect" placeholder="请选择客户等级" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select filterable v-model="customerSourceSelect" placeholder="请选择客户来源" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="startDate" type="date" placeholder="请选择起始日期" style="width: 100%"
							size="default" clearable />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="endDate" type="date" placeholder="请选择结束日期" style="width: 100%"
							size="default" clearable />
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="search" size="default">查询</el-button>
							<el-button @click="reset" size="default">重置</el-button>
						</div>
					</el-col>
					<!-- <el-col :span="4">
						<div style="text-align: right;">
							<el-button type="primary" @click="createNewLead" size="default">新建线索</el-button>
						</div>
					</el-col> -->
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" :data="CustomerLeadsTableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="id" label="线索编号" width="100">
					<template #default="scope">
						<span>{{ scope.row.id }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="contactEmail" label="邮箱" width="180" />
				<el-table-column prop="customerName" label="客户名称" width="150" />
				<el-table-column prop="customerLevel" label="客户等级" width="100" />
				<el-table-column prop="customerNation" label="国家" width="100" />
				<el-table-column prop="contactName" label="联系人" width="130" />
				<el-table-column prop="customerSource" label="来源" width="100" />
				<el-table-column prop="involvingBusiness" label="涉及业务" width="120" />
				<el-table-column prop="state" label="最近联系" width="100" />
				<el-table-column prop="state" label="原跟进人" width="100" />
				<el-table-column prop="create_by" label="创建人" width="100" />
				<el-table-column fixed="right" label="操作" width="260">
					<template #default="scope">
						<el-button type="text" size="small" @click="viewDetails(scope.row)">查看详情</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :total="totalItems" :page-sizes="[10, 20, 30, 50]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>

		<el-dialog :modal="false" modal-penetrable v-model="dialogFormVisible" title="新建线索" :close-on-click-modal=false
			style="width: 75%;" @close="handleDialogClose">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="公司信息" name="companyInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">公司信息</span>
					</template>
					<el-form ref="NewCustomerleadsformRef" :rules="rules" :model="NewCustomerleadsform"
						label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="线索名称" prop="clueName">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.clueName"
										placeholder="请输入线索名称" style="width: 300px;" size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户名称" prop="customerName">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.customerName"
										placeholder="请输入客户名称" style="width: 300px;" size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户来源" prop="customerSource">
									<el-select filterable :disabled="!isEditable"
										v-model="NewCustomerleadsform.customerSource" placeholder="请选择客户来源"
										style="width: 300px;" size="default" clearable>
										<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户等级">
									<el-select filterable disabled v-model="NewCustomerleadsform.customerLevel"
										placeholder="请选择客户等级" style="width: 300px;" size="default" clearable>
										<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="所属国家" prop="customerNation">
									<el-select filterable :disabled="!isEditable"
										v-model="NewCustomerleadsform.customerNation" placeholder="请选择客户所属国家"
										style="width: 300px;" size="default" clearable>
										<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="涉及业务" prop="involvingBusiness">
									<el-select filterable :disabled="!isEditable"
										v-model="NewCustomerleadsform.involvingBusiness" placeholder="请选择客户所涉及的业务"
										style="width: 300px;" size="default" clearable>
										<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="公司网址">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.compantWebsite"
										placeholder="请输入公司网址" style="width: 100%;" size="default" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="线索备注" prop="clueRemark">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.clueRemark"
										:autosize="{ minRows: 5, maxRows: 10 }" type="textarea" placeholder="请输入线索备注内容"
										style="width: 100%;" size="default" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="公司照片">
									<el-upload list-type="picture-card" :auto-upload="false"
										v-model:file-list="fileList" :limit="3" :disabled="fileList.length >= 3"
										@change="handleChange" :action="UploadUrl" :data="formData">
										<el-icon>
											<Plus />
										</el-icon>
										<template #file="{ file }">
											<div>
												<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
												<span class="el-upload-list__item-actions">
													<span class="el-upload-list__item-preview"
														@click="handlePictureCardPreview(file)">
														<el-icon><zoom-in /></el-icon>
													</span>
													<span v-if="!disabled" class="el-upload-list__item-delete"
														@click="handleRemove(file)">
														<el-icon>
															<Delete />
														</el-icon>
													</span>
												</span>
											</div>
										</template>
									</el-upload>
									<el-dialog v-model="dialogVisible">
										<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;"
											w-full :src="dialogImageUrl" alt="Preview Image" />
									</el-dialog>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="contactInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="联系人信息" name="contactInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">联系人信息</span>
					</template>
					<el-form ref="NewCustomerleadsformRef" :rules="rules" :model="NewCustomerleadsform"
						label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="联系人名称" prop="contactName">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactName"
										placeholder="请输入联系人名称" style="width: 300px;" size="default" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="联系人邮箱" prop="contactEmail">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactEmail"
										placeholder="请输入联系人邮箱" style="width: 300px;" size="default" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="联系人电话">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactPhone"
										placeholder="请输入联系人电话" style="width: 300px;" size="default" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="联系人职位">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactPosition"
										placeholder="请输入联系人职位" style="width: 300px;" size="default" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="联系人备注">
									<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactRemark"
										:autosize="{ minRows: 5, maxRows: 10 }" type="textarea" placeholder="请输入联系人备注内容"
										style="width: 100%;" size="default" clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isEditBtnVisible" type="warning" @click="IsEditBtnClick">
						编辑
					</el-button>
					<el-button
						v-show="(!isEditBtnVisible && !EditCustomerLeadsID) || (EditCustomerLeadsID && NewCustomerleadsform.IsDraft === 1)"
						type="primary" @click="SaveCustomerleadsDraft(NewCustomerleadsformRef)">
						保存草稿
					</el-button>
					<el-button v-show="isEditSaveBtnVisible" type="success"
						@click="EditSaveCustomerleads(NewCustomerleadsformRef)">
						提交
					</el-button>
					<el-button v-show="isSavebtnVisible" type="success"
						@click="SaveCustomerleads(NewCustomerleadsformRef)">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref, onBeforeUnmount } from 'vue'
import { ElMessageBox, UploadUserFile, ElMessage, UploadFile, FormInstance, FormRules } from 'element-plus'
import request from '@/utils/request';

const formData = ref(new FormData());
const clueNoInput = ref('')
const clueEmailInput = ref('')
const clueCustomerNameInput = ref('')
const contactNameInput = ref('')
const businessSelect = ref([])
const nationSelect = ref([])
const customerLevelSelect = ref([])
const customerSourceSelect = ref([])
const startDate = ref('')
const endDate = ref('')
const proxy = getCurrentInstance().proxy
const isEditable = ref(false)
const isSavebtnVisible = ref(true)
const isEditBtnVisible = ref(false)
const isEditSaveBtnVisible = ref(false)
const EditCustomerLeadsID = ref(0)
const state = reactive({
	optionss: {
		// 显示状态选项列表(动态字典将会从后台获取数据)
		sys_customer_source: [],
		hr_customer_level: [],
		hr_nation: [],
		hr_business_scope: []
	}
})
const { optionss } = toRefs(state)

//从后台读取多个字典数据
var dictParams = [{ dictType: 'sys_customer_source' }, { dictType: 'hr_customer_level' }, { dictType: 'hr_nation' }, { dictType: 'hr_business_scope' }]

proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})

const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(30);
const CustomerLeadsTableData = ref([])
GetCustomeleadList(currentPage.value, pageSize.value);
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	await GetCustomeleadList(newPage, pageSize.value);
};
const handleSizeChange = async (size) => {
	pageSize.value = size;
	currentPage.value = 1;
	await GetCustomeleadList(1, size);
};

const dialogFormVisible = ref(false)
const basicInfoCollapseActive = ref(['companyInfo'])
const contactInfoCollapseActive = ref(['contactInfo'])


// 上传文件
const filelistUrlStr = ref('')
const UploadUrl = 'CustomerLeads/UploadCompanyPhoto/UploadFile'
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)
const fileList = ref<UploadUserFile[]>([])
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const handleRemove = (file: UploadFile) => {
	ElMessageBox.confirm('确定删除该图片吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		// 从 fileList 中移除
		const index = fileList.value.findIndex(f => f.uid === file.uid);
		if (index !== -1) {
			fileList.value.splice(index, 1);
		}

		// 从 uploadedFiles 中移除
		const uploadedIndex = uploadedFiles.value.findIndex(f => f.uid === file.uid);
		if (uploadedIndex !== -1) {
			uploadedFiles.value.splice(uploadedIndex, 1);
		}

		// 更新 compantPhotoStr，只移除被删除的图片URL
		if (NewCustomerleadsform.compantPhotoStr) {
			const urls = NewCustomerleadsform.compantPhotoStr.split(',').filter(url => url.trim());
			// 只移除匹配的URL
			const updatedUrls = urls.filter(url => url !== file.url);
			NewCustomerleadsform.compantPhotoStr = updatedUrls.join(',');
			// 同时更新 filelistUrlStr
			filelistUrlStr.value = NewCustomerleadsform.compantPhotoStr;
		}

		ElMessage({
			type: 'success',
			message: '删除成功!',
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除',
		});
	});
};

const IMAGE_MAX_SIZE = 1 * 1024 * 1024; // 每张照片最大 1M
const handleChange = (file, fileList) => {
	if (file.raw && file.raw.size > IMAGE_MAX_SIZE) {
		ElMessage.error('每张图片不能超过 1M');
		fileList.splice(fileList.findIndex(f => f.uid === file.uid), 1);
		return;
	}
	if (fileList.length > 3) {
		ElMessage({ type: 'info', message: '最多上传3张图片！' });
		fileList.splice(3);
		return;
	}
	// 检查是否是新文件
	if (!file.isExisting) {
		const isDuplicate = uploadedFiles.value.some(f => f.name === file.name);
		if (isDuplicate) {
			ElMessage({
				type: 'info',
				message: '不要上传重复的文件！'
			});
			const index = fileList.findIndex(f => f.uid === file.uid);
			if (index !== -1) {
				fileList.splice(index, 1);
			}
		} else {
			// 标记为新文件
			file.isExisting = false;
			// 为新文件创建预览URL
			if (file.raw) {
				file.url = URL.createObjectURL(file.raw);
			}
			uploadedFiles.value.push(file);
		}
	}
};

const handlePictureCardPreview = (file: UploadFile) => {
	// 如果是新上传的文件，使用 blob URL
	if (file.url && file.url.startsWith('blob:')) {
		dialogImageUrl.value = file.url;
	} else {
		// 如果是已有文件，使用服务器URL
		dialogImageUrl.value = file.url;
	}
	dialogVisible.value = true;
};

// 在组件卸载时清理 blob URLs
onBeforeUnmount(() => {
	fileList.value.forEach(file => {
		if (file.url && file.url.startsWith('blob:')) {
			URL.revokeObjectURL(file.url);
		}
	});
});

const resetForm = () => {
	NewCustomerleadsform.clueName = '';
	NewCustomerleadsform.customerName = '';
	NewCustomerleadsform.customerSource = null;
	NewCustomerleadsform.customerLevel = null;
	NewCustomerleadsform.customerNation = null;
	NewCustomerleadsform.involvingBusiness = null;
	NewCustomerleadsform.compantWebsite = '';
	NewCustomerleadsform.clueRemark = '';
	NewCustomerleadsform.compantPhotoStr = '';
	NewCustomerleadsform.contactName = '';
	NewCustomerleadsform.contactEmail = '';
	NewCustomerleadsform.contactPhone = '';
	NewCustomerleadsform.contactPosition = '';
	NewCustomerleadsform.contactRemark = '';
	fileList.value = [];
	uploadedFiles.value = [];
}

const createNewLead = () => {
	resetForm();
	dialogFormVisible.value = true;
	isEditable.value = true;
	isSavebtnVisible.value = true;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = false;
}

const handleDialogClose = () => {
	// 重置折叠面板状态
	basicInfoCollapseActive.value = ['companyInfo']
	contactInfoCollapseActive.value = ['contactInfo']
}

// 定义表单数据
interface NewCustomerleadsform {
	id: number,
	create_by: string,
	create_time: string,
	update_by: string,
	update_time: string,
	remark: string,
	clueName: string,
	customerName: string,
	customerSource: number,
	customerLevel: number,
	customerNation: number,
	involvingBusiness: number,
	compantWebsite: string,
	clueRemark: string,
	compantPhotoStr: string,
	contactName: string,
	contactEmail: string,
	contactPhone: string,
	contactPosition: string,
	contactRemark: string,
	isDelete: number,
	clueType: number,
	IsDraft: number
}
const NewCustomerleadsformRef = ref<FormInstance>()
const NewCustomerleadsform = reactive<NewCustomerleadsform>({
	id: 0,
	create_by: '',
	create_time: '',
	update_by: '',
	update_time: '',
	remark: '',
	clueName: '',
	customerName: '',
	customerSource: null,
	customerLevel: null,
	customerNation: null,
	involvingBusiness: null,
	compantWebsite: '',
	clueRemark: '',
	compantPhotoStr: '',
	contactName: '',
	contactEmail: '',
	contactPhone: '',
	contactPosition: '',
	contactRemark: '',
	isDelete: 0,
	clueType: 0,
	IsDraft: 0
});

// 客户线索表单验证规则
const rules = reactive<FormRules<NewCustomerleadsform>>({
	clueName: [
		{
			required: true, message: '请输入线索名称', trigger: ['blur', 'change']
		}
	],
	customerName: [
		{
			required: true, message: '请输入客户名称', trigger: ['blur', 'change']
		}
	],
	customerSource: [
		{
			required: true, message: '请选择客户来源', trigger: ['blur', 'change']
		}
	],
	customerNation: [
		{
			required: true, message: '请选择所属国家', trigger: ['blur', 'change']
		}
	],
	involvingBusiness: [
		{
			required: true, message: '请选择涉及业务', trigger: ['blur', 'change']
		}
	],
	contactName: [
		{
			required: true, message: '请输入联系人名称', trigger: ['blur', 'change']
		}
	],
	contactEmail: [
		{
			required: true, message: '请输入联系人邮箱', trigger: ['blur', 'change']
		}],
	clueRemark: [
		{
			required: true, message: '请输入线索备注', trigger: ['blur', 'change']
		}
	]
});


//保存草稿
const SaveCustomerleadsDraft = async (formEl: FormInstance | undefined) => {
	// 验证线索名称是否填写
	if (!NewCustomerleadsform.clueName || NewCustomerleadsform.clueName.trim() === '') {
		ElMessage({
			message: "线索名称必须填写！",
			type: 'error'
		});
		return; // 阻止保存操作
	}

	if (hasFileListChanged()) {
		await uploadFiles();
	}

	// #region 保存线索草稿
	// 保存线索草稿
	NewCustomerleadsform.compantPhotoStr = filelistUrlStr.value;
	NewCustomerleadsform.customerLevel = 1;
	NewCustomerleadsform.IsDraft = 1;
	// 判断是新增还是修改
	if (EditCustomerLeadsID.value != 0) {
		// 修改草稿
		NewCustomerleadsform.id = EditCustomerLeadsID.value;
		request.post('CustomerLeads/EditCustomerLeads/Edit', NewCustomerleadsform).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg,
					type: 'success'
				})
				dialogFormVisible.value = false;
				GetCustomeleadList(currentPage.value, pageSize.value);
			} else {
				console.error('修改草稿出错');
			}
		}).catch(error => {
			console.error('修改草稿出错！😔错误内容：', error);
		})
	} else {
		// 新增草稿
		request.post('CustomerLeads/AddCustomerLeads/Add', NewCustomerleadsform).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg,
					type: 'success'
				})
				dialogFormVisible.value = false;
				GetCustomeleadList(currentPage.value, pageSize.value);
			} else {
				console.error('保存草稿出错');
			}
		}).catch(error => {
			console.error('保存草稿出错！😔错误内容：', error);
		})
	}
	// #endregion 保存线索草稿
}
//保存线索
const SaveCustomerleads = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			// #region 保存线索
			ElMessageBox.confirm('确定保存该线索吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				if (hasFileListChanged()) {
					uploadFiles();
				}
				// 保存线索
				NewCustomerleadsform.compantPhotoStr = filelistUrlStr.value;
				NewCustomerleadsform.customerLevel = 1;
				NewCustomerleadsform.IsDraft = 0;
				request.post('CustomerLeads/AddCustomerLeads/Add', NewCustomerleadsform).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})
						dialogFormVisible.value = false;
						GetCustomeleadList(currentPage.value, pageSize.value);
					} else {
						console.error('保存线索出错');
					}
				}).catch(error => {
					console.error('保存线索出错！😔错误内容：', error);
				})
			}).catch(() => {
				ElMessage({
					type: 'info',
					message: '已取消保存'
				})
			})
			filelistUrlStr.value = '';
			// #endregion 保存线索
		} else {
			console.log('error submit!', fields)
		}
	})
}

//获取线索列表
function GetCustomeleadList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'CustomerLeads/GetCustomerLeadsListByUserID/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				clueNo: clueNoInput.value || 0,
				clueEmail: clueEmailInput.value || null,
				clueCustomerName: clueCustomerNameInput.value,
				contactName: contactNameInput.value || null,
				business: businessSelect.value || 0,
				nation: nationSelect.value || 0,
				customerLevel: customerLevelSelect.value || 0,
				customerSource: customerSourceSelect.value || 0,
				startDate: startDate.value || null,
				endDate: endDate.value || null
			}
		}).then(response => {
			if (response.data.length > 0) {
				CustomerLeadsTableData.value = response.data;
				resolve(response.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetCustomeleadList(start - 1, end);
				} else {
					CustomerLeadsTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});

}

//重置查询条件
const reset = () => {
	clueNoInput.value = ''
	clueEmailInput.value = ''
	clueCustomerNameInput.value = ''
	contactNameInput.value = ''
	businessSelect.value = []
	nationSelect.value = []
	customerLevelSelect.value = []
	customerSourceSelect.value = []
	startDate.value = ''
	endDate.value = ''
	GetCustomeleadList(currentPage.value, pageSize.value);
}

///查询线索
const search = () => {
	GetCustomeleadList(currentPage.value, pageSize.value);
}

///查看线索详情
const viewDetails = (row) => {
	uploadedFiles.value = [];
	fileList.value = [];
	EditCustomerLeadsID.value = row.id;
	dialogFormVisible.value = true;
	isEditable.value = false;
	isSavebtnVisible.value = false;
	isEditBtnVisible.value = true;
	isEditSaveBtnVisible.value = false;
	NewCustomerleadsform.clueName = row.clueName;
	NewCustomerleadsform.customerName = row.customerName;
	NewCustomerleadsform.customerSource = state.optionss['sys_customer_source'].filter(item => item.dictLabel == row.customerSource).map(item => item.dictValue).values().next().value;
	NewCustomerleadsform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictLabel == row.customerLevel).map(item => item.dictValue).values().next().value;
	NewCustomerleadsform.customerNation = state.optionss['hr_nation'].filter(item => item.dictLabel == row.customerNation).map(item => item.dictValue).values().next().value;
	NewCustomerleadsform.involvingBusiness = state.optionss['hr_business_scope'].filter(item => item.dictLabel == row.involvingBusiness).map(item => item.dictValue).values().next().value;
	NewCustomerleadsform.compantWebsite = row.compantWebsite;
	NewCustomerleadsform.clueRemark = row.clueRemark;
	NewCustomerleadsform.contactName = row.contactName;
	NewCustomerleadsform.contactEmail = row.contactEmail;
	NewCustomerleadsform.contactPhone = row.contactPhone;
	NewCustomerleadsform.contactPosition = row.contactPosition;
	NewCustomerleadsform.contactRemark = row.contactRemark;
	NewCustomerleadsform.create_time = row.create_time;
	NewCustomerleadsform.create_by = row.create_by;
	if (row.compantPhotoStr != null && row.compantPhotoStr != '') {
		NewCustomerleadsform.compantPhotoStr = row.compantPhotoStr;
		row.compantPhotoStr.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url
				});
			}
		});
	}
	uploadedFiles.value = fileList.value;
	//}
}
///删除线索
const DelCustomerleads = (row) => {
	ElMessageBox.confirm('确定删除该线索吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'CustomerLeads/DelCustomerLeads/Del',
			method: 'GET',
			params: {
				ID: row.id
			}
		}).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg,
					type: 'success'
				})
				GetCustomeleadList(currentPage.value, pageSize.value);
			} else {
				console.error('删除线索出错');
			}
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除'
		})
	})
}

///编辑线索
const IsEditBtnClick = () => {
	isEditable.value = true;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = true;
}

///编辑保存线索
const EditSaveCustomerleads = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			if (EditCustomerLeadsID.value != 0) {
				ElMessageBox.confirm('确定修改该线索吗？', '提示', {
					confirmButtonText: '确定',
					cancelButtonText: '取消',
					type: 'warning'
				}).then(() => {
					if (hasFileListChanged()) {
						uploadFiles();
					}
					// 修改线索
					NewCustomerleadsform.id = EditCustomerLeadsID.value;
					NewCustomerleadsform.IsDraft = 0;
					request.post('CustomerLeads/EditCustomerLeads/Edit', NewCustomerleadsform).then(response => {
						if (response != null) {
							ElMessage({
								message: response.msg,
								type: 'success'
							})
							dialogFormVisible.value = false;
							GetCustomeleadList(currentPage.value, pageSize.value);
						} else {
							console.error('修改线索出错');
						}
					}).catch(error => {
						console.error('修改线索出错', error);
					})
				}).catch(() => {
					ElMessage({
						type: 'info',
						message: '已取消保存'
					})
				})
			} else {
				ElMessage({
					type: 'info',
					message: '未选择要修改的线索！'
				})
			}
		} else {
			var errormessage = '';
			for (let prop in fields) {
				if (fields[prop] && fields[prop].length > 0) {
					fields[prop].forEach(error => {
						errormessage += `<strong><br>${error.message}</strong>`;
					});
				}
			}
			ElMessage({
				type: 'info',
				dangerouslyUseHTMLString: true,
				message: errormessage
			})
		}
	})
}

const initializeFileList = () => {
	if (!NewCustomerleadsform.compantPhotoStr) return;

	const existingFiles = NewCustomerleadsform.compantPhotoStr.split(',').filter(url => url);
	fileList.value = existingFiles.map(url => ({
		name: url.split('/').pop(),
		url: url,
		raw: null,
		isExisting: true  // 标记为已存在的文件
	}));
	uploadedFiles.value = [...fileList.value];
};

const hasFileListChanged = () => {
	// 检查是否有新添加的文件（没有 url 但有 raw 的文件）
	const hasNewFiles = fileList.value.some(file => !file.url && file.raw);

	// 检查已有文件是否有改变（通过比较当前显示的文件URL和原始的文件URL）
	const currentUrls = fileList.value
		.filter(file => file.url)
		.map(file => file.url)
		.sort();

	const originalUrls = NewCustomerleadsform.compantPhotoStr ?
		NewCustomerleadsform.compantPhotoStr.split(',').filter(url => url).sort() : [];

	const hasUrlsChanged = JSON.stringify(currentUrls) !== JSON.stringify(originalUrls);

	return hasNewFiles || hasUrlsChanged;
};

const uploadFiles = async () => {
	// 重置文件URL字符串
	filelistUrlStr.value = '';

	// 获取当前显示的已有文件URL（排除 blob URL）
	const currentFileUrls = fileList.value
		.filter(file => file.url && !file.url.startsWith('blob:'))
		.map(file => file.url);

	// 只上传新添加的文件
	const uploadPromises = fileList.value
		.filter(file => file.raw && (!file.url || file.url.startsWith('blob:')))  // 处理新文件
		.map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'CustomerLeads/CompanyPhoto');
			formData.append('FileNameType', '1');
			formData.append('File', file.raw);
			formData.append('storeType', '1');
			return request.postForm(UploadUrl, formData);
		});

	try {
		if (uploadPromises.length > 0) {
			const responses = await Promise.all(uploadPromises);
			const newUrls = responses
				.filter(response => response != null)
				.map(response => response.data.url);

			// 合并已有URL和新URL
			const allUrls = [...currentFileUrls, ...newUrls];
			filelistUrlStr.value = allUrls.join(',');

			// 更新文件列表中的URL
			fileList.value = fileList.value.map(file => {
				if (file.raw && (!file.url || file.url.startsWith('blob:'))) {
					// 找到对应的新URL
					const newUrl = responses.find(response =>
						response.data.originalName === file.name
					)?.data.url;
					if (newUrl) {
						// 清理旧的 blob URL
						if (file.url && file.url.startsWith('blob:')) {
							URL.revokeObjectURL(file.url);
						}
						file.url = newUrl;
					}
				}
				return file;
			});
		} else {
			// 如果没有新文件上传，使用当前的文件URL列表
			filelistUrlStr.value = currentFileUrls.join(',');
		}

		// 更新表单中的文件字符串
		NewCustomerleadsform.compantPhotoStr = filelistUrlStr.value;

	} catch (error) {
		console.error('上传文件出错：', error);
		ElMessage({
			message: "上传文件出错！",
			type: 'error'
		});
		throw error;
	}
};
</script>

<style>
.demo-border .text {
	width: 15%;
}

.demo-border .line {
	width: 70%;
}

.demo-border .line div {
	width: 100%;
	height: 0;
	border-top: 1px solid var(--el-border-color);
}

.demo-border .line .dashed {
	border-top: 2px dashed var(--el-border-color);
}

.el-upload-list__item-thumbnail {
	width: 100%;
	/* 图片宽度填充容器 */
	height: 150px;
	/* 固定高度 */
	object-fit: cover;
	/* 调整图片大小以覆盖容器 */
}

/* 表格样式优化 */
.el-table .el-table__row td,
.el-table .el-table__body tr td,
.el-table .el-table__body .el-table__row td {
	padding: 2px 1px !important;
	line-height: 12px !important;
	overflow: hidden !important;
	text-overflow: ellipsis !important;
	white-space: nowrap !important;
}

/* 更具体的表格行高度控制 */
.el-table tbody tr {
	height: 20px !important;
}

.el-table tbody tr td {
	padding: 2px 1px !important;
	line-height: 12px !important;
	overflow: hidden !important;
	text-overflow: ellipsis !important;
	white-space: nowrap !important;
}

/* 创建合同和查看合同详情dialog中的表单组件间距减少一半 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

/* 隐藏组件外部的验证信息显示 */
.el-dialog .el-form-item__error,
.el-dialog .el-form-item .el-form-item__error,
.el-dialog .el-form-item.is-error .el-form-item__error {
	display: none !important;
	visibility: hidden !important;
	height: 0 !important;
	overflow: hidden !important;
}
</style>
