<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="openAddCustomerCollection">新增客户收款</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="SearchReceiptNumber" filterable placeholder="选择收款单号" style="width: 15%">
				<el-option v-for="dict in optionss.sql_customercollections_no" :key="dict.dictCode"
					:label="dict.dictLabel" :value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SearchBank" filterable placeholder="选择收汇银行" style="width: 15%">
				<el-option v-for="dict in optionss.hr_bank" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchReceiptDateStart" type="date" placeholder="请选择收汇日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchReceiptDateEnd" type="date" placeholder="请选择收汇日期止" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchSubmit()">查询</el-button>
				<el-button @click="resetSearch()">重置</el-button>
			</el-row>
		</div>

		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;收款单据</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="customercollectiontableData">
			<el-table-column prop="receiptNumber" label="收款单号" width="180">
				<template #default="scope">
					<span>{{ scope.row.receiptNumber }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="receiptDate" label="收汇日期" width="150"
				:formatter="(row, column, cellValue) => formatDate(cellValue)"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
			<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
			<el-table-column prop="amount" label="金额" width="150"></el-table-column>
			<el-table-column prop="bank" label="收汇银行" width="150"></el-table-column>
			<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small"
						@click=CheckCustomerCollectionDetails(scope.row)>查看详情</el-button>
					<el-button link type="danger" size="small" @click="handleDelete(scope.row)">
						删除
					</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="addcustomercollectiondialog" title="新增收款单据" :close-on-click-modal=false style="width: 70%;"
			@close="Closeaddcustomercollectiondialog()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="addcustomercollectionform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="收款单号">
							<el-input v-model="addcustomercollectionform.receiptNumber" placeholder="自动生成"
								style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇日期">
							<el-date-picker v-model="addcustomercollectionform.receiptDate" type="date"
								placeholder="请选择收汇日期" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="addcustomercollectionform.ourCompany" placeholder="请选择我方公司"
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="外销币种">
							<el-select v-model="addcustomercollectionform.foreignCurrency" placeholder="请选择外销币种"
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="汇率">
							<el-input v-model="addcustomercollectionform.exchangeRate" placeholder="请输入汇率"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="金额">
							<el-input v-model="addcustomercollectionform.amount" placeholder="请输入金额"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="收汇银行">
							<el-select v-model="addcustomercollectionform.bank" placeholder="请选择收汇银行"
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_bank" :key="dict.dictCode" :label="dict.dictLabel"
									:value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户">
							<el-select v-model="addcustomercollectionform.Customer" filterable clearable
								placeholder="请选择客户" style="width: 300px">
								<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="款项类别">
							<el-select v-model="addcustomercollectionform.FundsClassification" filterable clearable
								placeholder="请选择款项类别" style="width: 300px">
								<el-option v-for="dict in optionss.hr_funds_classification" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="关联模块">
							<el-select v-model="addcustomercollectionform.AssociatedModules" filterable clearable
								placeholder="请选择关联模块" style="width: 300px">
								<el-option v-for="dict in optionss.hr_collection_associated_modules"
									:key="dict.dictCode" :label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="关联单号">
							<el-select v-model="addcustomercollectionform.AssociatedModulesDocumentID" filterable
								clearable placeholder="请选择关联单号" style="width: 300px"
								:disabled="isAssociatedDocumentDisabled">
								<el-option v-for="dict in associatedDocumentOptions" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="16">
						<el-form-item label="收款单据">
							<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList"
								limit="3" :disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl">
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
								<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
									:src="dialogImageUrl" alt="Preview Image" />
							</el-dialog>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" v-if="isSaveBtnShow" @click="SaveCustomerCollection()">
						保存草稿
					</el-button>
					<el-button type="primary" v-if="isEditSaveBtnShow" @click="SaveCustomerCollection()">
						保存草稿
					</el-button>
					<el-button type="success" v-if="isSubmitBtnShow" @click="SubmitCustomerCollection()">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref, computed, watch } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { getDicts } from '@/api/system/dict/data';
import { get } from 'sortablejs';

// 添加格式化日期函数
const formatDate = (dateString) => {
	if (!dateString) return '';
	const date = new Date(dateString);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	return `${year}-${month}-${day}`;
};

// 添加删除方法
const handleDelete = (row) => {
	ElMessageBox.confirm(
		'确定要删除该收款单据吗？此操作不可恢复！',
		'警告',
		{
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}
	)
		.then(async () => {
			try {
				const response = await request({
					url: `CustomerCollections/DelCustomerCollections/DelCustomerCollections`,
					method: 'get',
					params: {
						id: row.id
					}
				});

				if (response.code === 200) {
					ElMessage({
						type: 'success',
						message: response.msg,
					});
					// 重新加载列表数据
					GetCustomerCollectionsList(currentPage.value, pageSize.value);
				} else {
					ElMessage({
						type: 'error',
						message: response.msg || '删除失败',
					});
				}
			} catch (error) {
				console.error('删除收款单据时出错:', error);
				ElMessage({
					type: 'error',
					message: '删除收款单据时发生错误',
				});
			}
		})
		.catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消删除',
			});
		});
};

// 添加新方法
const openAddCustomerCollection = () => {
	if (EditID.value == 0) {
		isSaveBtnShow.value = true;
		isSubmitBtnShow.value = true;
	}
	addcustomercollectiondialog.value = true;
	addcustomercollectionform.value.receiptDate = new Date().toLocaleDateString();
	getNewReceiptNumber();
};
// 添加获取新收款单号的函数
const getNewReceiptNumber = async () => {
	try {
		const response = await request.get('CustomerCollections/GetReceiptNewNumber/GetReceiptNewNumber');
		if (response.code === 200) {
			addcustomercollectionform.value.receiptNumber = response.data;
		} else {
			ElMessage.error('获取收款单号失败');
		}
	} catch (error) {
		console.error('获取收款单号出错:', error);
		ElMessage.error('获取收款单号出错');
	}
};

//搜索条件
const SearchReceiptNumber = ref('')
const SearchBank = ref('')
const SearchReceiptDateStart = ref('')
const SearchReceiptDateEnd = ref('')
interface CustomUploadFile extends UploadUserFile {
	isChanged?: boolean;
}
// Then update the fileList ref type
const fileList = ref<CustomUploadFile[]>([]);
const isEdit = ref(false);
const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);
const isSubmitBtnShow = ref(false);

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_ourcompany: [],
		hr_export_currency: [],
		hr_bank: [],
		sql_customercollections_no: [],
		sql_hr_customer: [],
		hr_funds_classification: [],
		hr_collection_associated_modules: [],
		sql_sale_contracts: [],
		sql_shippingdeliveries: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'hr_export_currency' },
	{ dictType: 'hr_bank' },
	{ dictType: 'sql_customercollections_no' },
	{ dictType: 'sql_hr_customer' },
	{ dictType: 'hr_funds_classification' },
	{ dictType: 'hr_collection_associated_modules' },
	{ dictType: 'sql_sale_contracts' },
	{ dictType: 'sql_shippingdeliveries' }
]

async function fetchDataAndExecute() {
	try {
		const response = await getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		/*获取当前页面列表函数放在下方*/
		await GetCustomerCollectionsList(currentPage.value, pageSize.value);
		//await GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);;  // 现在可以安全执行
		//await GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
fetchDataAndExecute();

// 计算属性：根据关联模块动态获取关联单号选项
const associatedDocumentOptions = computed(() => {
	const selectedModule = addcustomercollectionform.value.AssociatedModules;
	// 如果关联模块为空或未选择，返回空数组
	if (!selectedModule || selectedModule === '') {
		return [];
	}
	// 根据关联模块的值返回对应的数据源
	if (selectedModule === '1') {
		return optionss.value.sql_sale_contracts || [];
	} else if (selectedModule === '2') {
		return optionss.value.sql_shippingdeliveries || [];
	}
	return [];
});

// 计算属性：控制关联单号下拉框是否禁用
const isAssociatedDocumentDisabled = computed(() => {
	return !addcustomercollectionform.value.AssociatedModules ||
		addcustomercollectionform.value.AssociatedModules === '';
});

// 监听关联模块变化，清空关联单号选择
watch(() => addcustomercollectionform.value.AssociatedModules, (newValue, oldValue) => {
	// 当关联模块发生变化时，清空关联单号
	if (newValue !== oldValue) {
		addcustomercollectionform.value.AssociatedModulesDocumentID = '';
	}
	// 特别处理当关联模块被清空的情况
	if (!newValue || newValue === '') {
		addcustomercollectionform.value.AssociatedModulesDocumentID = '';
	}
});

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'	// 上传图片地址
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)

// 检查上传客户图片数量
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

// 删除产品图片
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
		// 从 filelistUrlStr 中移除对应的 url
		if (filelistUrlStr.value != '') {
			const urls = filelistUrlStr.value.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				filelistUrlStr.value = urls.join(',');
			}
		}
		// 从 CustomerProfileform.customerPhoto 中移除对应的 url
		// if (Productform.ProductPhoto !== '' && Productform.ProductPhoto !== null) {
		// 	const urls = Productform.ProductPhoto.split(',');
		// 	const urlIndex = urls.indexOf(file.url);
		// 	if (urlIndex !== -1) {
		// 		urls.splice(urlIndex, 1);
		// 		Productform.ProductPhoto = urls.join(',');
		// 	}
		// }

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

const customerCollectionsRequest = reactive({
	Id: 0,
	ReceiptNumber: "",
	ReceiptDate: null,
	OurCompany: "",
	ForeignCurrency: "",
	ExchangeRate: 0,
	Amount: 0,
	Bank: "",
	ReceiptImageUrl: "",
	Remark: "",
	IsDelete: 0,
	isDraft: 1
})

//保存收款单据
const SaveCustomerCollection = async () => {
	try {
		const result = await ElMessageBox.confirm('确定保存该收款单据吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});
		// 准备基础数据
		customerCollectionsRequest.ReceiptNumber = addcustomercollectionform.value.receiptNumber;
		customerCollectionsRequest.ReceiptDate = addcustomercollectionform.value.receiptDate;
		customerCollectionsRequest.OurCompany = addcustomercollectionform.value.ourCompany;
		customerCollectionsRequest.ForeignCurrency = addcustomercollectionform.value.foreignCurrency;
		customerCollectionsRequest.ExchangeRate = Number(addcustomercollectionform.value.exchangeRate);
		customerCollectionsRequest.Amount = Number(addcustomercollectionform.value.amount);
		customerCollectionsRequest.Bank = addcustomercollectionform.value.bank;
		// 上传图片
		let receiptImageUrls = [];
		if (Array.isArray(fileList.value) && fileList.value.length > 0) {
			receiptImageUrls = await Promise.all(fileList.value
				.filter(file => file.isChanged)
				.map(async (file) => {
					const response = await uploadReceiptPhoto(file);
					if (response.code === 200 && response.data.url) {
						return response.data.url;
					}
					throw new Error('上传图片失败');
				}));
		}
		// 合并现有的和新上传的图片URL
		if (filelistUrlStr.value) {
			const existingUrls = filelistUrlStr.value.split(',');
			receiptImageUrls = [...existingUrls, ...receiptImageUrls];
		}
		customerCollectionsRequest.ReceiptImageUrl = receiptImageUrls.join(',');
		// 保存数据
		const response = await request.post(
			!isEdit.value
				? 'CustomerCollections/AddCustomerCollections/Add'
				: 'CustomerCollections/EditCustomerCollections/Edit',
			isEdit.value ? { ...customerCollectionsRequest, Id: EditID.value } : customerCollectionsRequest
		);
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			});
			Closeaddcustomercollectiondialog();
			GetCustomerCollectionsList(currentPage.value, pageSize.value);
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('保存收款单据出错！😔错误内容：', error);
			ElMessage.error('保存收款单据失败');
		}
	}
};
// 辅助函数：上传收款单据图片
const uploadReceiptPhoto = async (file) => {
	const formData = new FormData();
	formData.append('FileName', file.name || '未命名文件');
	formData.append('FileDir', 'CustomerCollection/CustomerCollectionPhoto');
	formData.append('FileNameType', '1');
	formData.append('File', file.raw || file);
	formData.append('storeType', '1');
	return await request.postForm(UploadUrl, formData);
};

const clearAll = () => {
	addcustomercollectionform.value.receiptNumber = ''
	addcustomercollectionform.value.receiptDate = ''
	addcustomercollectionform.value.ourCompany = ''
	addcustomercollectionform.value.foreignCurrency = ''
	addcustomercollectionform.value.exchangeRate = ''
	addcustomercollectionform.value.amount = ''
	addcustomercollectionform.value.bank = ''
	addcustomercollectionform.value.attachment = ''
	addcustomercollectionform.value.Customer = ''
	addcustomercollectionform.value.FundsClassification = ''
	addcustomercollectionform.value.AssociatedModules = ''
	addcustomercollectionform.value.AssociatedModulesDocumentID = ''
	fileList.value = []
	filelistUrlStr.value = ''
	isEdit.value = false
	EditID.value = 0
	filelistUrlStr.value = ''
}

//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
//收款单据表格
const customercollectiontableData = ref([])
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetCustomerCollectionsList(start, end);
};
//获取收款单据列表
function GetCustomerCollectionsList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'CustomerCollections/GetCustomerCollectionsList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				id: SearchReceiptNumber.value || 0,
				bank: SearchBank.value || '',
				startDatetime: SearchReceiptDateStart.value || null,
				endDatetime: SearchReceiptDateEnd.value || null
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				customercollectiontableData.value = response.data.result;
				customercollectiontableData.value.forEach((item) => {
					item.ourCompany = state.optionss.hr_ourcompany.find((dict) => dict.dictValue === item.ourCompany)?.dictLabel;
					item.foreignCurrency = state.optionss.hr_export_currency.find((dict) => dict.dictValue === item.foreignCurrency)?.dictLabel;
					item.bank = state.optionss.hr_bank.find((dict) => dict.dictValue === item.bank)?.dictLabel;
				})
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetCustomerCollectionsList(start - 1, end);
				} else {
					customercollectiontableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const EditID = ref(0);
const CheckCustomerCollectionDetails = (row) => {
	EditID.value = row.id;
	if (row.isDraft === 1) {
		isSubmitBtnShow.value = true;
		isEdit.value = true;
		isSaveBtnShow.value = false;
		isEditSaveBtnShow.value = true;
	} else {
		isSubmitBtnShow.value = false;
		isEdit.value = false;
		isSaveBtnShow.value = false;
		isEditSaveBtnShow.value = false;

	}
	// 基本信息赋值
	addcustomercollectionform.value.receiptNumber = row.receiptNumber;
	addcustomercollectionform.value.receiptDate = row.receiptDate;
	addcustomercollectionform.value.ourCompany = state.optionss.hr_ourcompany.find((dict) => dict.dictLabel === row.ourCompany)?.dictValue || '';
	addcustomercollectionform.value.foreignCurrency = state.optionss.hr_export_currency.find((dict) => dict.dictLabel === row.foreignCurrency)?.dictValue || '';
	addcustomercollectionform.value.exchangeRate = row.exchangeRate;
	addcustomercollectionform.value.amount = row.amount;
	addcustomercollectionform.value.bank = state.optionss.hr_bank.find((dict) => dict.dictLabel === row.bank)?.dictValue || '';

	// 关联模块相关字段赋值
	addcustomercollectionform.value.Customer = row.customer || '';
	addcustomercollectionform.value.FundsClassification = row.fundsClassification || '';
	addcustomercollectionform.value.AssociatedModules = row.associatedModules || '';
	addcustomercollectionform.value.AssociatedModulesDocumentID = row.associatedModulesDocumentID || '';

	// 清空现有图片列表
	fileList.value = [];
	filelistUrlStr.value = '';

	// 处理图片显示，添加完整的 URL 路径
	if (row.receiptImageUrl) {
		const imageUrls = row.receiptImageUrl.split(',');
		fileList.value = imageUrls.map((url, index) => ({
			name: `Image ${index + 1}`,
			url: url,
			isChanged: false,
			uid: Date.now() + index
		}));

		filelistUrlStr.value = row.receiptImageUrl;
	}

	// 更新已上传文件列表
	uploadedFiles.value = [...fileList.value];

	// 打开对话框
	addcustomercollectiondialog.value = true;
}

const Closeaddcustomercollectiondialog = () => {
	addcustomercollectiondialog.value = false
	clearAll();
}
const addcustomercollectiondialog = ref(false)

const SearchSubmit = () => {
	GetCustomerCollectionsList(currentPage.value, pageSize.value);
}

const resetSearch = () => {
	SearchReceiptNumber.value = ''
	SearchBank.value = ''
	SearchReceiptDateStart.value = ''
	SearchReceiptDateEnd.value = ''
	GetCustomerCollectionsList(currentPage.value, pageSize.value);
}

const addcustomercollectionform = ref({
	receiptNumber: '',
	receiptDate: '',
	ourCompany: '',
	foreignCurrency: '',
	exchangeRate: '',
	amount: '',
	bank: '',
	attachment: '',
	Customer: '',
	FundsClassification: '',
	AssociatedModules: '',
	AssociatedModulesDocumentID: ''
})

const SubmitCustomerCollection = async () => {
	try {
		await ElMessageBox.confirm('确定提交该收款单据吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});

		if (EditID.value === 0) {
			// First save the draft
			await SaveCustomerCollection();
		}

		// Then update the draft status
		const response = await request({
			url: 'CustomerCollections/UpdateCustomerCollectionsIsDraft/UpdateIsDraft',
			method: 'get',
			params: {
				ID: EditID.value
			}
		});

		if (response.code === 200) {
			ElMessage({
				type: 'success',
				message: response.msg
			});
			Closeaddcustomercollectiondialog();
			GetCustomerCollectionsList(currentPage.value, pageSize.value);
		} else {
			ElMessage({
				type: 'error',
				message: response.msg || '提交失败'
			});
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('提交收款单据出错：', error);
			ElMessage.error('提交收款单据失败');
		}
	}
}
</script>
