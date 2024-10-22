<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenAddSupperDialog">添加供应商</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="Searchsupplierselect" filterable placeholder="选择供应商（可输入查询）" style="width: 15%">
				<el-option v-for="item in supplierselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Searchsupplierselect" filterable placeholder="选择供应商等级" style="width: 15%">
				<el-option v-for="item in supplierselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Searchproductselect" filterable placeholder="选择业务范围" style="width: 15%">
				<el-option v-for="item in productselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchinquiryDate" type="date" placeholder="请选择最近交易日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchquotationDate" type="date" placeholder="请选择最近交易日期" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 5px;">
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;供应商信息表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="SupplierInfoTableData">
			<el-table-column prop="supplierId" label="供应商编号" width="150"></el-table-column>
			<el-table-column prop="shortName" label="供应商简称" width="150"></el-table-column>
			<el-table-column prop="fullName" label="供应商全称" width="150"></el-table-column>
			<el-table-column prop="cooperationLevel" label="供应商等级" width="150"></el-table-column>
			<el-table-column prop="businessScope" label="业务范围" width="150"></el-table-column>
			<el-table-column prop="province" label="所在省份" width="150"></el-table-column>
			<el-table-column prop="address" label="详细地址" width="150"></el-table-column>
			<el-table-column prop="developmentDate" label="开发时间" width="150"></el-table-column>
			<el-table-column prop="lastTransaction" label="最近交易" width="150"></el-table-column>
			<el-table-column prop="quoteSuccessRate" label="报价成交率" width="150"></el-table-column>
			<el-table-column fixed="right" label="详情" width="100">
				<template #default="scope">
					<el-button type="text" size="small" @click="checkSupplierDetails(scope.row)">查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="SupplierInfoTableDatahandlePageChange"
			:current-page="SupplierInfoTableDatacurrentPage" :page-size="SupplierInfoTableDatapageSize"
			:total="SupplierInfoTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="AddSupperDialog" title="添加供应商" :close-on-click-modal=false style="width: 70%;"
			@close="Closeaddsupperdialog">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="Addsupperinfoform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="厂商编号">
							<el-input v-model="Addsupperinfoform.supplierId" placeholder="请输入厂商编号" disabled
								style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商简称">
							<el-input v-model="Addsupperinfoform.shortName" placeholder="请输入厂商简称" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商名称">
							<el-input v-model="Addsupperinfoform.fullName" placeholder="请输入厂商名称" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="所在省份">
							<el-select v-model="Addsupperinfoform.province" placeholder="请选择所在省份" style="width: 300px"
								:disabled="isEditable">
								<el-option v-for="dict in optionss.hr_china_provinces" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>

							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="所在城市">
							<el-select v-model="Addsupperinfoform.city" placeholder="请选择所在城市" style="width: 300px"
								:disabled="isEditable">
								<el-option v-for="dict in optionss.hr_china_city" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="公司主页">
							<el-input v-model="Addsupperinfoform.website" placeholder="请输入公司主页" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="邮政编码">
							<el-input v-model="Addsupperinfoform.postalCode" placeholder="请输入邮政编码"
								:disabled="isEditable" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="详细地址">
							<el-input v-model="Addsupperinfoform.address" placeholder="请输入详细地址" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="业务范围">
							<el-select v-model="Addsupperinfoform.businessScope" filterable placeholder="请选择业务范围"
								:disabled="isEditable" style="width: 300px;">
								<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="信用等级">
							<el-input v-model="Addsupperinfoform.creditLevel" placeholder="请输入信用等级"
								:disabled="isEditable" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="合作等级">
							<el-select v-model="Addsupperinfoform.cooperationLevel" filterable placeholder="选择合作等级"
								:disabled="isEditable" style="width: 300px;">
								<el-option v-for="dict in optionss.hr_supplier_level" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="能否开票">
							<el-checkbox v-model="Addsupperinfoform.canInvoice" :disabled="isEditable" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="结算方式">
							<el-select v-model="Addsupperinfoform.paymentMethod" placeholder="请选择结算方式"
								:disabled="isEditable" style="width: 300px">
								<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="开户银行">
							<el-input v-model="Addsupperinfoform.bankName" placeholder="请输入开户银行" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="银行账号">
							<el-input v-model="Addsupperinfoform.bankAccount" placeholder="请输入银行账号"
								:disabled="isEditable" style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="公司税号">
							<el-input v-model="Addsupperinfoform.taxNumber" placeholder="请输入公司税号" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="开发时间">
							<el-date-picker v-model="Addsupperinfoform.developmentDate" type="date"
								:disabled="isEditable" placeholder="请选择开发时间" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="最近成交日期">
							<el-date-picker v-model="Addsupperinfoform.lastTransaction" type="date"
								:disabled="isEditable" placeholder="请选择最近成交日期" style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-form-item label="供应商图片">
						<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList" limit="5"
							:disabled="fileList.length >= 5" @change="handleChange" :action="UploadUrl"
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

				</el-row>
			</el-form>
			<el-tabs v-model="activeTab" class="demo-tabs">
				<el-button class="mt-4" type="primary" @click="handleAddContactRow"
					style="margin-bottom: 10px;">添加联系人</el-button>
				<el-tab-pane label="联系人" name="contacttabpane">
					<el-table :data="supperinfoContactsTableData" style="width: 100%">
						<el-table-column label="联系人姓名">
							<template #default="{ row }">
								<el-input v-model="row.name" placeholder="联系人姓名" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人部门">
							<template #default="{ row }">
								<el-input v-model="row.department" placeholder="联系人部门" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人性别">
							<template #default="{ row }">
								<el-select v-model="row.gender" placeholder="联系人性别" :disabled="isEditable">
									<el-option label="男" value="男" />
									<el-option label="女" value="女" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column label="联系人职务">
							<template #default="{ row }">
								<el-input v-model="row.position" placeholder="联系人职务" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人电话号码">
							<template #default="{ row }">
								<el-input v-model="row.phoneNumber" placeholder="电话号码" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人手机号码">
							<template #default="{ row }">
								<el-input v-model="row.mobileNumber" placeholder="手机号码" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人电子邮件">
							<template #default="{ row }">
								<el-input v-model="row.email" placeholder="电子邮件" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人备注">
							<template #default="{ row }">
								<el-input v-model="row.remark" placeholder="备注" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="small" :disabled="isEditable"
									@click="handleDeleteContactRow(scope.$index)">删除</el-button>
							</template>
						</el-table-column>
					</el-table></el-tab-pane>
				<el-tab-pane label="开户银行" name="second">开户银行</el-tab-pane>
				<el-tab-pane label="报价历史" name="third">报价历史</el-tab-pane>
				<el-tab-pane label="采购历史" name="fourth">采购历史</el-tab-pane>
				<el-tab-pane label="寄样历史" name="fifth">寄样历史</el-tab-pane>
				<el-tab-pane label="收样历史" name="sixth">收样历史</el-tab-pane>
				<el-tab-pane label="出货历史" name="seventh">出货历史</el-tab-pane>
				<el-tab-pane label="客诉历史" name="eighth">客诉历史</el-tab-pane>
				<el-tab-pane label="往来邮件" name="ninth">往来邮件</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isEditBtnVisible" type="primary" @click="IsEditBtnClick">
						编辑
					</el-button>
					<el-button v-show="isEditSaveBtnVisible" type="primary" @click="EditSaveSupperinfo">
						编辑保存
					</el-button>
					<el-button v-show="isSavebtnVisible" type="primary" @click="SaveSupperinfo">
						确定保存
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

const activeTab = ref('contacttabpane')
const isEditBtnVisible = ref(false)
const isEditSaveBtnVisible = ref(false)
const isSavebtnVisible = ref(true)

/*供应商联系人列表*/
const supperinfoContactsTableData = ref([]);
const handleAddContactRow = () => {
	const newRow = {
		name: '',
		department: '',
		gender: '',
		position: '',
		phoneNumber: '',
		mobileNumber: '',
		email: '',
		remarks: ''
	};
	supperinfoContactsTableData.value.push(newRow);
}

//获取供应商编号
const GetNextSupplierID = () => {
	request({
		url: 'Supplierinfo/GetNewSupplierID/GetSupplierID',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			Addsupperinfoform.supplierId = response;
		} else {
			console.error('获取下一个供应商编号出错');
		}
	}).catch(error => {
		console.error('获取下一个供应商编号出错！😔错误内容：', error);
	})
}

const OpenAddSupperDialog = () => {
	GetNextSupplierID();
	AddSupperDialog.value = true;
}

const handleDeleteContactRow = (index) => {
	supperinfoContactsTableData.value.splice(index, 1);
}
/*供应商联系人列表*/
const Addsupperinfoform = reactive({
	supplierId: '',
	shortName: '',
	fullName: '',
	province: '',
	city: '',
	website: '',
	postalCode: '',
	address: '',
	businessScope: '',
	creditLevel: '',
	cooperationLevel: '',
	canInvoice: '',
	paymentMethod: '',
	bankName: '',
	bankAccount: '',
	taxNumber: '',
	developmentDate: '',
	lastTransaction: '',
	factoryImageURL: ''
})

const SupplierRequest = reactive({
	Id: '',
	SupplierId: '',
	ShortName: '',
	FullName: '',
	Province: '',
	City: '',
	Website: '',
	PostalCode: '',
	Address: '',
	BusinessScope: '',
	CreditLevel: '',
	CooperationLevel: '',
	CanInvoice: '',
	PaymentMethod: '',
	BankName: '',
	BankAccount: '',
	TaxNumber: '',
	DevelopmentDate: '',
	LastTransaction: '',
	FactoryImageUrl: '',
	IsDelete: '',
	contactInfoItems: []
});

const AddSupperDialog = ref(false)

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_contract_status: [],
		sql_hr_customer: [],
		hr_customer_level: [],
		hr_settlement_way: [],
		hr_pricing_term: [],
		hr_nation: [],
		sql_hr_sale: [],
		hr_transport_port: [],
		hr_transportation_method: [],
		hr_ourcompany: [],
		hr_export_currency: [],
		hr_signing_place: [],
		sys_yes_no: [],
		hr_quotation_basis: [],
		hr_outerbox_unit: [],
		hr_calculate_unit: [],
		hr_supplier_level: [],
		hr_business_scope: [],
		hr_china_provinces: [],
		hr_china_city: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_supplier_level' }, { dictType: 'hr_business_scope' }, { dictType: 'hr_china_provinces' }, { dictType: 'hr_china_city' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})
/*动态下拉框end*/

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)
const fileList = ref<UploadUserFile[]>([])
const uploadedFiles = ref([]);  // 用于存储已上传的文件



const SaveSupperinfo = () => {
	ElMessageBox.confirm('确定保存供应商信息？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 保存供应商信息
		SupplierRequest.Id = Addsupperinfoform.supplierId
		SupplierRequest.SupplierId = Addsupperinfoform.supplierId
		SupplierRequest.ShortName = Addsupperinfoform.shortName
		SupplierRequest.FullName = Addsupperinfoform.fullName
		SupplierRequest.Province = Addsupperinfoform.province
		SupplierRequest.City = Addsupperinfoform.city
		SupplierRequest.Website = Addsupperinfoform.website
		SupplierRequest.PostalCode = Addsupperinfoform.postalCode
		SupplierRequest.Address = Addsupperinfoform.address
		SupplierRequest.BusinessScope = Addsupperinfoform.businessScope
		SupplierRequest.CreditLevel = Addsupperinfoform.creditLevel
		SupplierRequest.CooperationLevel = Addsupperinfoform.cooperationLevel
		SupplierRequest.CanInvoice = Addsupperinfoform.canInvoice
		SupplierRequest.PaymentMethod = Addsupperinfoform.paymentMethod
		SupplierRequest.BankName = Addsupperinfoform.bankName
		SupplierRequest.BankAccount = Addsupperinfoform.bankAccount
		SupplierRequest.TaxNumber = Addsupperinfoform.taxNumber
		SupplierRequest.DevelopmentDate = Addsupperinfoform.developmentDate
		SupplierRequest.LastTransaction = Addsupperinfoform.lastTransaction
		SupplierRequest.IsDelete = '0'
		SupplierRequest.contactInfoItems = supperinfoContactsTableData.value
		supperinfoContactsTableData.value.forEach((element) => {
			if (element.gender == '男') {
				element.gender = '1'
			} else {
				element.gender = '0'
			}
		})
		//上传供应商图片
		const uploadPromises = fileList.value.map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'Supplier/SupplierPhoto');
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
						message: "上传供应商图片出错！😔",
						type: 'error'
					})
				}
			});
			// 保存线索
			Addsupperinfoform.factoryImageURL = filelistUrlStr.value;
			SupplierRequest.FactoryImageUrl = Addsupperinfoform.factoryImageURL;
			request.post('Supplierinfo/AddSupplierinfo/Add', SupplierRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: response.msg,
						type: 'success'
					})

				} else {
					console.error('新增供应商资料出错');
				}
			}).catch(error => {
				console.error('新增供应商资料出错！😔错误内容：', error);
			})
		}).catch(error => {
			console.error('上传供应商图片出错！😔错误内容：', error);
		});
		AddSupperDialog.value = false
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}

// 上传文件
const handleChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 5) {
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

// 预览图片
const handlePictureCardPreview = (file: UploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}
// 删除图片
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
		if (Addsupperinfoform.factoryImageURL) {
			const urls = Addsupperinfoform.factoryImageURL.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				Addsupperinfoform.factoryImageURL = urls.join(',');
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

//查询条件
const Searchsupplierselect = ref('')
const Searchproductselect = ref('')
const SearchinquiryDate = ref('')
const SearchquotationDate = ref('')

// 供应商信息表
const SupplierInfoTableData = ref([]);
//分页组件
const SupplierInfoTableDatatotalItems = ref(0);
const SupplierInfoTableDatacurrentPage = ref(1);
const SupplierInfoTableDatapageSize = ref(10);
const SupplierInfoTableDatahandlePageChange = async (newPage) => {
	SupplierInfoTableDatacurrentPage.value = newPage;
	const start = newPage;
	const end = SupplierInfoTableDatapageSize.value;
	const newData = await GetSupplierInfoList(start, end);
};
GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
function GetSupplierInfoList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'Supplierinfo/GetSupplierList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				SupplierInfoTableData.value = response.data.result;
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetSupplierInfoList(start - 1, end);
				} else {
					SupplierInfoTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const isEditable = ref(false);
// 供应商详情
const SelctedSupplierId = ref('')
//查看供应商详情
const checkSupplierDetails = (row) => {
	isEditBtnVisible.value = true;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = false;
	// 获取供应商信息
	isEditable.value = true;
	SelctedSupplierId.value = row.id;
	Addsupperinfoform.supplierId = row.supplierId;
	Addsupperinfoform.shortName = row.shortName;
	Addsupperinfoform.fullName = row.fullName;
	Addsupperinfoform.province = row.province;
	Addsupperinfoform.city = row.city;
	Addsupperinfoform.website = row.website;
	Addsupperinfoform.postalCode = row.postalCode;
	Addsupperinfoform.address = row.address;
	Addsupperinfoform.businessScope = row.businessScope;
	Addsupperinfoform.creditLevel = row.creditLevel;
	Addsupperinfoform.cooperationLevel = row.cooperationLevel;
	Addsupperinfoform.canInvoice = row.canInvoice;
	Addsupperinfoform.paymentMethod = row.paymentMethod;
	Addsupperinfoform.bankName = row.bankName;
	Addsupperinfoform.bankAccount = row.bankAccount;
	Addsupperinfoform.taxNumber = row.taxNumber;
	Addsupperinfoform.developmentDate = row.developmentDate;
	Addsupperinfoform.lastTransaction = row.lastTransaction;
	if (row.factoryImageUrl != null && row.factoryImageUrl != '') {
		Addsupperinfoform.factoryImageURL = row.factoryImageUrl;
		row.factoryImageUrl.split(',').forEach((url, index) => {
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
	// 获取联系人信息
	request({
		url: 'Supplierinfo/GetSupplierContactBySUpplierID/GetSupplierContact',
		method: 'GET',
		params: {
			SupplierID: SelctedSupplierId.value
		}
	}).then(response => {
		if (response.data.length > 0) {
			supperinfoContactsTableData.value = response.data;
		} else {
			supperinfoContactsTableData.value = [];
		}
	}).catch(error => {
		console.error(error);
	});
	AddSupperDialog.value = true;
}
const IsEditBtnClick = () => {
	isEditable.value = false;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = true;
}

const EditSaveSupperinfo = () => {
	ElMessageBox.confirm('确定保存已完成编辑供应商信息？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 保存供应商信息
		SupplierRequest.Id = Addsupperinfoform.supplierId
		SupplierRequest.SupplierId = Addsupperinfoform.supplierId
		SupplierRequest.ShortName = Addsupperinfoform.shortName
		SupplierRequest.FullName = Addsupperinfoform.fullName
		SupplierRequest.Province = Addsupperinfoform.province
		SupplierRequest.City = Addsupperinfoform.city
		SupplierRequest.Website = Addsupperinfoform.website
		SupplierRequest.PostalCode = Addsupperinfoform.postalCode
		SupplierRequest.Address = Addsupperinfoform.address
		SupplierRequest.BusinessScope = Addsupperinfoform.businessScope
		SupplierRequest.CreditLevel = Addsupperinfoform.creditLevel
		SupplierRequest.CooperationLevel = Addsupperinfoform.cooperationLevel
		SupplierRequest.CanInvoice = Addsupperinfoform.canInvoice
		SupplierRequest.PaymentMethod = Addsupperinfoform.paymentMethod
		SupplierRequest.BankName = Addsupperinfoform.bankName
		SupplierRequest.BankAccount = Addsupperinfoform.bankAccount
		SupplierRequest.TaxNumber = Addsupperinfoform.taxNumber
		SupplierRequest.DevelopmentDate = Addsupperinfoform.developmentDate
		SupplierRequest.LastTransaction = Addsupperinfoform.lastTransaction
		SupplierRequest.IsDelete = '0'
		SupplierRequest.contactInfoItems = supperinfoContactsTableData.value
		// 上传供应商图片
		const uploadPromises = fileList.value.filter(file => file.isChanged).map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'Supplier/SupplierPhoto');
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
						message: "上传供应商图片出错！😔",
						type: 'error'
					})
				}
			});
			// 编辑保存线索
			SupplierRequest.FactoryImageUrl = filelistUrlStr.value;
			request.post('Supplierinfo/EditSupplierinfo/Edit', SupplierRequest).then(response => {
				if (response != null) {
					AddSupperDialog.value = false;
					GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
				} else {
					console.error('编辑供应商信息出错');
				}
			}).catch(error => {
				console.error('保存已编辑的供应商信息出错！😔错误内容：', error);
			})
		}).catch(error => {
			console.error('上传供应商图片出错！😔错误内容：', error);
		});
	}
	).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}

const Closeaddsupperdialog = () => {
	Addsupperinfoform.supplierId = '';
	Addsupperinfoform.shortName = '';
	Addsupperinfoform.fullName = '';
	Addsupperinfoform.province = '';
	Addsupperinfoform.city = '';
	Addsupperinfoform.website = '';
	Addsupperinfoform.postalCode = '';
	Addsupperinfoform.address = '';
	Addsupperinfoform.businessScope = '';
	Addsupperinfoform.creditLevel = '';
	Addsupperinfoform.cooperationLevel = '';
	Addsupperinfoform.canInvoice = '';
	Addsupperinfoform.paymentMethod = '';
	Addsupperinfoform.bankName = '';
	Addsupperinfoform.bankAccount = '';
	Addsupperinfoform.taxNumber = '';
	Addsupperinfoform.developmentDate = '';
	Addsupperinfoform.lastTransaction = '';
	Addsupperinfoform.factoryImageURL = '';
	supperinfoContactsTableData.value = [];
	fileList.value = [];
	uploadedFiles.value = [];
	isEditable.value = false;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = true;
}
</script>
