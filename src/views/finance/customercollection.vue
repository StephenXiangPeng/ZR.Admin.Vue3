<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="addcustomercollectiondialog = true">新增客户收款</el-button>
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
			<el-table-column prop="receiptNumber" label="收款单号" width="150"></el-table-column>
			<el-table-column prop="receiptDate" label="收汇日期" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
			<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
			<el-table-column prop="amount" label="金额" width="150"></el-table-column>
			<el-table-column prop="bank" label="收汇银行" width="150"></el-table-column>
			<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small"
						@click=CheckCustomerCollectionDetails(scope.row)>查看详情</el-button>
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
							<el-input v-model="addcustomercollectionform.receiptNumber" placeholder="请输入收款单号"
								style="width: 300px"></el-input>
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
				</el-row>
				<el-row>
					<el-col :span="16">
						<el-form-item label="收款单据">
							<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList"
								limit="3" :disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl"
								:data="formData">
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
					<el-button type="primary" v-if="isSaveBtnShow" @click="SaveCustomerCollection()">
						确定保存
					</el-button>
					<el-button type="primary" v-if="isEditSaveBtnShow" @click="SaveCustomerCollection()">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { get } from 'sortablejs';
import { el } from 'element-plus/es/locale';

//搜索条件
const SearchReceiptNumber = ref('')
const SearchBank = ref('')
const SearchReceiptDateStart = ref('')
const SearchReceiptDateEnd = ref('')

const isEdit = ref(false);
const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);


/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_ourcompany: [],
		hr_export_currency: [],
		hr_bank: [],
		sql_customercollections_no: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_ourcompany' }, { dictType: 'hr_export_currency' }, { dictType: 'hr_bank' }, { dictType: 'sql_customercollections_no' }]

async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
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

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'	// 上传图片地址
const fileList = ref<UploadUserFile[]>([]);
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
	ExchangeRate: null,
	Amount: null,
	Bank: "",
	ReceiptImageUrl: "",
	Remark: "",
	IsDelete: 0
})

//保存收款单据
const SaveCustomerCollection = () => {
	ElMessageBox.confirm('确定保存该收款单据吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		customerCollectionsRequest.ReceiptNumber = addcustomercollectionform.value.receiptNumber
		customerCollectionsRequest.ReceiptDate = addcustomercollectionform.value.receiptDate
		customerCollectionsRequest.OurCompany = addcustomercollectionform.value.ourCompany
		customerCollectionsRequest.ForeignCurrency = addcustomercollectionform.value.foreignCurrency
		customerCollectionsRequest.ExchangeRate = addcustomercollectionform.value.exchangeRate
		customerCollectionsRequest.Amount = addcustomercollectionform.value.amount
		customerCollectionsRequest.Bank = addcustomercollectionform.value.bank
		//上传收款单据图片
		const uploadPromises = fileList.value.filter(file => file.isChanged).map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'CustomerCollection/CustomerCollectionPhoto');
			formData.append('FileNameType', '1');
			formData.append('File', file.raw);
			formData.append('storeType', '1');
			// 返回上传文件的 Promise
			return request.postForm(UploadUrl, formData);
		});
		Promise.all(uploadPromises).then(responses => {
			responses.forEach((response, index) => {
				if (response.code == 200 && response.data.url != null) {
					if (filelistUrlStr.value != '') {
						filelistUrlStr.value += ',' + response.data.url;
					} else {
						filelistUrlStr.value += (index > 0 ? ',' : '') + response.data.url;
					}
				} else {
					ElMessage({
						message: "上传收款单据图片出错！😔",
						type: 'error'
					})
				}
			});
			customerCollectionsRequest.ReceiptImageUrl = filelistUrlStr.value
			if (!isEdit.value) {
				request.post('CustomerCollections/AddCustomerCollections/Add', customerCollectionsRequest).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})
						Closeaddcustomercollectiondialog();
						GetCustomerCollectionsList(currentPage.value, pageSize.value);
					} else {
						console.error('保存收款单据出错');
					}
				}).catch(error => {
					console.error('保存收款单据出错！😔错误内容：', error);
				})
			} else {
				customerCollectionsRequest.Id = EditID.value;
				request.post('CustomerCollections/EditCustomerCollections/Edit', customerCollectionsRequest).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})
						Closeaddcustomercollectiondialog();
						GetCustomerCollectionsList(currentPage.value, pageSize.value);
					} else {
						console.error('编辑收款单据出错');
					}
				}).catch(error => {
					console.error('编辑收款单据出错！😔错误内容：', error);
				})
			}
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消保存',
			});
		}).catch(error => {
			console.error('上传收款单据图片出错！😔错误内容：', error);
		});
	});
}

const clearAll = () => {
	addcustomercollectionform.value.receiptNumber = ''
	addcustomercollectionform.value.receiptDate = ''
	addcustomercollectionform.value.ourCompany = ''
	addcustomercollectionform.value.foreignCurrency = ''
	addcustomercollectionform.value.exchangeRate = ''
	addcustomercollectionform.value.amount = ''
	addcustomercollectionform.value.bank = ''
	addcustomercollectionform.value.attachment = ''
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
	isEdit.value = true;
	isSaveBtnShow.value = false;
	isEditSaveBtnShow.value = true;
	EditID.value = row.id;
	addcustomercollectionform.value.receiptNumber = row.receiptNumber
	addcustomercollectionform.value.receiptDate = row.receiptDate
	addcustomercollectionform.value.ourCompany = state.optionss.hr_ourcompany.find((dict) => dict.dictLabel === row.ourCompany)?.dictValue;
	addcustomercollectionform.value.foreignCurrency = state.optionss.hr_export_currency.find((dict) => dict.dictLabel === row.foreignCurrency)?.dictValue;
	addcustomercollectionform.value.exchangeRate = row.exchangeRate
	addcustomercollectionform.value.amount = row.amount
	addcustomercollectionform.value.bank = state.optionss.hr_bank.find((dict) => dict.dictLabel === row.bank)?.dictValue;
	addcustomercollectionform.value.attachment = row.attachment
	addcustomercollectiondialog.value = true
	if (row.receiptImageUrl != null && row.receiptImageUrl != '') {
		row.receiptImageUrl.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false
				});
				filelistUrlStr.value += (index > 0 ? ',' : '') + url;
			}
		});
	}
	uploadedFiles.value = fileList.value;
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
	attachment: ''
})


</script>
