<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="createNewLead">新建线索</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-input v-model="clueNoInput" clearable style="width: 15%"
				placeholder="输入线索编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="clueEmailInput" clearable style="width: 15%"
				placeholder="输入邮箱地址" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="clueCustomerNameInput" clearable style="width: 15%"
				placeholder="输入客户名称" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="contactNameInput" clearable style="width: 15%"
				placeholder="输入联系人" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="businessSelect" class="m-2" placeholder="选择涉及业务" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

		</div>
		<div style="width: 100%; margin-top: 5px;">
			<el-select filterable v-model="nationSelect" class="m-2" placeholder="选择国家" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="customerLevelSelect" class="m-2" placeholder="选择客户等级" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="customerSourceSelect" class="m-2" placeholder="选择客户来源" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="startDate" type="date" placeholder="起始日期" size="default"
				style="width: 15%;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;------&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="endDate" type="date" placeholder="结束日期" size="default" style="width: 15%;" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="search">查询</el-button>
				<el-button @click="reset">重置</el-button>
			</el-row>
		</div>
		<el-divider></el-divider>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;线索信息表</span>
		</div>
		<el-divider></el-divider>
		<el-table :data="CustomerLeadsTableData" align="center">
			<el-table-column fixed prop="id" label="线索编号" style="width: 12%;" />
			<el-table-column prop="contactEmail" label="邮箱" style="width: 12%;" />
			<el-table-column prop="customerName" label="客户名称" style="width: 12%;" />
			<el-table-column prop="customerLevel" label="客户等级" style="width: 12%;" />
			<el-table-column prop="customerNation" label="国家" style="width: 12%;" />
			<el-table-column prop="contactName" label="联系人" style="width: 12%;" />
			<el-table-column prop="customerSource" label="来源" style="width: 12%;" />
			<el-table-column prop="involvingBusiness" label="涉及业务" style="width: 12%;" />
			<el-table-column prop="state" label="最近联系" style="width: 12%;" />
			<el-table-column prop="state" label="原跟进人" style="width: 12%;" />
			<el-table-column prop="create_by" label="创建人" style="width: 12%;" />
			<el-table-column fixed="right" label="操作" style="width: 12%;">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small" @click="viewDetails(scope.row)">详情</el-button>
					<el-button link type="primary" size="small" @click="DelCustomerleads(scope.row)">删除</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />

		<el-dialog v-model="dialogFormVisible" title="新建线索" :close-on-click-modal=false :inline="true">
			<span style="font-size: 20px; font-weight: bold;">公司信息</span>
			<el-divider></el-divider>
			<el-form ref="NewCustomerleadsformRef" :rules="rules" :model="NewCustomerleadsform" label-width="auto"
				:inline="true">
				<el-form-item label="线索名称：" prop="clueName">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.clueName" placeholder="输入线索名称"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户名称：" prop="customerName">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.customerName" placeholder="输入客户名称"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户来源：" prop="customerSource">
					<el-select filterable :disabled="!isEditable" v-model="NewCustomerleadsform.customerSource"
						placeholder="请选择客户来源" style="width: 300px;">
						<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户等级：">
					<el-select filterable disabled v-model="NewCustomerleadsform.customerLevel" placeholder="请选择客户等级"
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="所属国家：" prop="customerNation">
					<el-select filterable :disabled="!isEditable" v-model="NewCustomerleadsform.customerNation"
						placeholder="请选择客户所属国家" style="width: 300px;">
						<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="涉及业务：" prop="involvingBusiness">
					<el-select filterable :disabled="!isEditable" v-model="NewCustomerleadsform.involvingBusiness"
						placeholder="请选择客户所涉及的业务" style="width: 300px;">
						<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="公司网址：">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.compantWebsite" placeholder="输入公司网址"
						style="width: 738px;" />
				</el-form-item>
				<el-form-item label="线索备注：" prop="clueRemark">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.clueRemark"
						:autosize="{ minRows: 5, maxRows: 10 }" type="textarea" placeholder="输入线索备注内容"
						style="width: 738px;" />
				</el-form-item>
				<el-form-item label="公司照片：">
					<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList" :limit="3"
						:disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl" :data="formData">
						<el-icon>
							<Plus />
						</el-icon>
						<template #file="{ file }">
							<div>
								<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
								<span class="el-upload-list__item-actions">
									<span class="el-upload-list__item-preview" @click="handlePictureCardPreview(file)">
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
						<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
							:src="dialogImageUrl" alt="Preview Image" />
					</el-dialog>
				</el-form-item>
				<br><span style="font-size: 20px; font-weight: bold;">联系人信息</span>
				<el-divider></el-divider>
				<el-form-item label="联系人名称：" prop="contactName">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactName" placeholder="输入联系人名称"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="联系人邮箱：" prop="contactEmail">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactEmail" placeholder="输入联系人邮箱"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="联系人电话：">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactPhone" placeholder="输入联系人电话"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="联系人职位：">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactPosition"
						placeholder="输入联系人职位" style="width: 300px;" />
				</el-form-item>
				<el-form-item label="联系人备注：">
					<el-input :disabled="!isEditable" v-model="NewCustomerleadsform.contactRemark"
						:autosize="{ minRows: 5, maxRows: 10 }" type="textarea" placeholder="输入联系人备注内容"
						style="width: 750px;" />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isEditBtnVisible" type="primary" @click="IsEditBtnClick">
						编辑
					</el-button>
					<el-button v-show="isEditSaveBtnVisible" type="primary"
						@click="EditSaveCustomerleads(NewCustomerleadsformRef)">
						编辑保存
					</el-button>
					<el-button v-show="isSavebtnVisible" type="primary"
						@click="SaveCustomerleads(NewCustomerleadsformRef)">
						确定保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref } from 'vue'
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
const pageSize = ref(10);
const CustomerLeadsTableData = ref([])
GetCustomeleadList(currentPage.value, pageSize.value);
const handlePageChange = async (newPage) => {
	// Fetch new data based on the new page and update currentTableData
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetCustomeleadList(start, end);  // Assume fetchData is a function that fetches data from the server
};

const dialogFormVisible = ref(false)


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

		// 从 NewCustomerleadsform.compantPhotoStr 中移除对应的 url
		if (NewCustomerleadsform.compantPhotoStr) {
			const urls = NewCustomerleadsform.compantPhotoStr.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				NewCustomerleadsform.compantPhotoStr = urls.join(',');
			}
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

const handleChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 3) {
		ElMessage({
			type: 'info',
			message: '最多上传3张图片！'
		});
		fileList.splice(3); // 保留前三个文件，移除其余文件
		return; // 不再继续执行后面的代码
	}
	const duplicate = uploadedFiles.value.findIndex(fileItem => fileItem.name === file.name);
	if (duplicate !== -1) {
		ElMessage({
			type: 'info',
			message: '不要上传重复的文件哦！😔'
		});
		const duplicatesInFileList = fileList.filter(fileItem => fileItem.name === file.name);
		if (duplicatesInFileList.length > 1) {
			for (let i = 1; i < duplicatesInFileList.length; i++) {
				const index = fileList.findIndex(fileItem => fileItem.uid === duplicatesInFileList[i].uid);
				if (index !== -1) {
					fileList.splice(index, 1); // 从fileList中移除重复文件，保留一个
				}
			}
		}
	} else {
		// 添加文件到uploadedFiles，确保不重复
		const newFiles = fileList.filter(file => !uploadedFiles.value.some(fileItem => fileItem.name === file.name));
		newFiles.forEach(file => {
			if (!file.isChanged) {
				file.isChanged = true;
				uploadedFiles.value.push(file);
			}
		});
	}
};

const handlePictureCardPreview = (file: UploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}

const resetForm = () => {
	NewCustomerleadsform.clueName = '';
	NewCustomerleadsform.customerName = '';
	NewCustomerleadsform.customerSource = '';
	NewCustomerleadsform.customerLevel = '';
	NewCustomerleadsform.customerNation = '';
	NewCustomerleadsform.involvingBusiness = '';
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
	customerSource: string,
	customerLevel: string,
	customerNation: string,
	involvingBusiness: string,
	compantWebsite: string,
	clueRemark: string,
	compantPhotoStr: string,
	contactName: string,
	contactEmail: string,
	contactPhone: string,
	contactPosition: string,
	contactRemark: string,
	isDelete: number,
	clueType: number
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
	customerSource: '',
	customerLevel: '',
	customerNation: '',
	involvingBusiness: '',
	compantWebsite: '',
	clueRemark: '',
	compantPhotoStr: '',
	contactName: '',
	contactEmail: '',
	contactPhone: '',
	contactPosition: '',
	contactRemark: '',
	isDelete: 0,
	clueType: 0
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
				//上传公司图片
				const uploadPromises = fileList.value.map(file => {
					const formData = new FormData();
					formData.append('FileName', file.name);
					formData.append('FileDir', 'CustomerLeads/CompanyPhoto');
					formData.append('FileNameType', '1');
					formData.append('File', file.raw);
					formData.append('storeType', '1');
					// 返回上传文件的 Promise
					return request.postForm(UploadUrl, formData);
				});
				Promise.all(uploadPromises).then(responses => {
					responses.forEach((response, index) => {
						if (response != null) {
							filelistUrlStr.value += (index > 0 ? ',' : '') + response.data.url;
						} else {
							ElMessage({
								message: "上传公司图片出错！😔",
								type: 'error'
							})
						}
					});
					// 保存线索
					NewCustomerleadsform.compantPhotoStr = filelistUrlStr.value;
					NewCustomerleadsform.customerLevel = '1';
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
				}).catch(error => {
					console.error('上传公司图片出错！😔错误内容：', error);
				});
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
					url: url,
					isChanged: false
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
					//上传公司图片
					const uploadPromises = fileList.value.filter(file => file.isChanged).map(file => {
						const formData = new FormData();
						formData.append('FileName', file.name);
						formData.append('FileDir', 'CustomerLeads/CompanyPhoto');
						formData.append('FileNameType', '1');
						formData.append('File', file.raw);
						formData.append('storeType', '1');
						// 返回上传文件的 Promise
						if (file.raw != null) {
							return request.postForm(UploadUrl, formData);
						}
					});
					Promise.all(uploadPromises).then(responses => {
						responses.forEach((response, index) => {
							if (response != null) {
								if (NewCustomerleadsform.compantPhotoStr != null && NewCustomerleadsform.compantPhotoStr != '') {
									NewCustomerleadsform.compantPhotoStr += ',' + response.data.url;
								} else {
									NewCustomerleadsform.compantPhotoStr = response.data.url;
								}
							} else {
								ElMessage({
									message: "上传公司图片出错！😔",
									type: 'error'
								})
							}
						});
						// 修改线索
						if (fileList.value.length == 0) {

							NewCustomerleadsform.compantPhotoStr = ''
						}
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
								console.error('修改线索出错');
							}
						}).catch(error => {
							console.error('修改线索出错', error);
						})
					}).catch(error => {
						console.error('上传公司图片出错！😔错误内容：', error);
					});
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
</style>
