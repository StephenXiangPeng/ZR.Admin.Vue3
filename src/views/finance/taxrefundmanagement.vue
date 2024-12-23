<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="openaddctaxrefunddialog">新增退税</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="SearchInvoiceNumber" placeholder="选择发票号码">
				<el-option v-for=" dict in optionss.sql_invoicenumber" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SearchCustomerID" filterable placeholder="选择客户简称" style="width: 15%">
				<el-option v-for="dict in optionss.sql_hr_customerabbr" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchStartDate" type="date" placeholder="请选择退税日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchEndDate" type="date" placeholder="请选择退税日期止" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchBtnClick()">查询</el-button>
				<el-button @click="ResetBtnClick()">重置</el-button>
			</el-row>
		</div>

		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;退税单据</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="TaxrefundtableData" style="width: 100%">
			<el-table-column prop="invoiceNumber" label="发票号码" width="150"></el-table-column>
			<el-table-column prop="refundAmount" label="应退税额" width="150"></el-table-column>
			<el-table-column prop="actualRefundAmount" label="实际退税额" width="150"></el-table-column>
			<el-table-column prop="customerAbbr" label="客户简称" width="150"></el-table-column>
			<el-table-column prop="isRefunded" label="是否已退" width="150"></el-table-column>
			<el-table-column prop="refundDate" label="退税日期" width="150"></el-table-column>
			<el-table-column fixed="right" label="操作" width="100">
				<template #default="scope">
					<el-button type="text" size="small" @click="CheckTaxrefundtableData(scope.row)">查看/编辑</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="TaxrefundtableDataHandlePageChange"
			:current-page="TaxrefundtableDataCurrentPage" :page-size="TaxrefundtableDataPageSize"
			:total="TaxrefundtableDataTotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="addctaxrefunddialog" title="新增退税" :close-on-click-modal=false style="width: 70%;"
			@close="clearTaxrefundDialog()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="addctaxrefundform" label-width="100px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="退税编号">
							<el-input v-model="addctaxrefundform.taxRefundNumber" placeholder="请输入退税编号"
								style="width: 300px;" :disabled=isDisable></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="退税日期">
							<el-date-picker v-model="addctaxrefundform.taxRefundDate" type="date" placeholder="请选择退税日期"
								style="width: 300px;" :disabled=isDisable></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="addctaxrefundform.ourCompany" placeholder="请选择我方公司" style="width: 300px"
								:disabled=isDisable>
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<div style="margin-top: 5px;"></div>
			<span style="font-size: 20px; font-weight: bold;">退税明细</span>
			<el-divider></el-divider>
			<el-button class="mt-4" type="primary" @click="AddTaxRefundDetailsClick" style="margin-bottom: 10px;"
				:disabled="isDisable">添加退税明细</el-button>
			<el-table :data="addtaxrefundtableData" style="width: 100%" :disable="true">
				<el-table-column prop="Id" label="Id" width="150" v-if="false">
					<template #default="{ row }">
						<el-input v-model="row.Id" size="small" :disabled=isDisable></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="invoiceNumber" label="发票号码" width="150">
					<template #default="{ row }">
						<el-select v-model="row.InvoiceNumber" placeholder="选择发票号码" size="small"
							@change="invoiceNumberChange(row)" :disabled=isDisable>
							<el-option v-for="dict in optionss.sql_invoicenumber" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop=" RefundAmount" label="应退税额" width="150">
					<template #default="{ row }">
						<el-input-number v-model="row.RefundAmount" placeholder="应退税额" size="small" :precision="3"
							:step="0.01" @change="calculation" :disabled=isDisable></el-input-number>
					</template>
				</el-table-column>
				<el-table-column prop=" ActualRefundAmount" label="实际退税额" width="150">
					<template #default="{ row }">
						<el-input-number v-model="row.ActualRefundAmount" placeholder="输入实际退税额" size="small"
							:precision="3" :step="0.01" @change="calculation" :disabled=isDisable></el-input-number>
					</template>
				</el-table-column>
				<el-table-column prop="CustomerAbbr" label="客户简称" width="150">
					<template #default="{ row }">
						<el-input v-model="row.CustomerAbbr" size="small" :disabled=isDisable></el-input>
					</template>
				</el-table-column>

				<el-table-column prop="IsRefunded" label="是否已退" width="150">
					<template #default="{ row }">
						<el-select v-model="row.IsRefunded" placeholder="选择是否" size="small" :disabled=isDisable>
							<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
								:value="dict.dictValue" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="RefundDate" label="退税日期" width="200">
					<template #default="{ row }">
						<el-date-picker v-model="row.RefundDate" type="date" placeholder="请选择退税日期" size="Default"
							style="width: 150px;" :disabled=isDisable></el-date-picker>
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="100">
					<template #default="scope">
						<el-button type="text" size="small" @click="addtaxrefundtableDatahandleDelete(scope.$index)"
							:disabled=isDisable>删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<span style="font-size: 20px; font-weight: bold;">合计信息</span>
			<el-divider></el-divider>
			<el-form :model="addtaxrefundform2" label-width="100px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="应退税总额">
							<el-input v-model="addtaxrefundform2.refundAmount" placeholder="请输入应退税总额"
								style="width: 300px;" :disabled=isDisable></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="实际退税额">
							<el-input v-model="addtaxrefundform2.actualRefundAmount" placeholder="请输入实际退税额"
								style="width: 300px;" :disabled=isDisable></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="未退税额">
							<el-input v-model="addtaxrefundform2.unrefundedAmount" placeholder="请输入未退税额"
								style="width: 300px;" :disabled=isDisable></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<div style="margin-top: 30px;"></div>
			<span style="font-size: 20px; font-weight: bold;">备注信息</span>
			<el-divider></el-divider>
			<el-form :model="addtaxrefundform3" label-width="100px">
				<el-row>
					<el-col :span="24">
						<el-form-item label="备注信息">
							<el-input type="textarea" :rows="10" placeholder="在此输入文字"
								v-model="addtaxrefundform3.remarks" :disabled=isDisable></el-input>
						</el-form-item>
					</el-col>

				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="SaveClick()" v-if="isSaveBtnShow">
						确定保存
					</el-button>
					<el-button type="primary" @click="EditBtnClick()" v-if="isEdit">
						编辑
					</el-button>
					<el-button type="primary" @click="SaveClick()" v-if="isEditBtnShow">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile } from 'element-plus'
import request from '@/utils/request';



const isDisable = ref(false);
const isEdit = ref(false);
const isEditBtnShow = ref(false);
const isSaveBtnShow = ref(true);
const isEditTaxrefundID = ref(0);
const addctaxrefundform = ref({
	taxRefundNumber: '',
	taxRefundDate: '',
	ourCompany: ''
})
const addctaxrefunddialog = ref(false)
const currentDate = new Date();
const formattedDate = currentDate.toISOString().split('T')[0];
const openaddctaxrefunddialog = () => {
	addctaxrefunddialog.value = true
	addctaxrefundform.value.taxRefundDate = formattedDate;
}
const addtaxrefundtableData = ref([])
const AddTaxRefundDetailsClick = () => {
	const newRow = {
		InvoiceNumber: '',
		RefundAmount: '',
		ActualRefundAmount: '',
		CustomerAbbr: '',
		IsRefunded: '',
		RefundDate: formattedDate
	};
	addtaxrefundtableData.value.push(newRow);
}

const addtaxrefundtableDatahandleDelete = (index) => {
	addtaxrefundtableData.value.splice(index, 1);
	calculation();
}

const invoiceNumberChange = (row) => {
	request({
		url: 'ShippingDeliveries/GetShippingDeliveriesDetailsByid/GetShippingDeliveriesDetails',
		method: 'GET',
		params: {
			ShippingDeliveriesId: row.InvoiceNumber
		}
	}).then(response => {
		if (response.data.shippingDeliveries != null) {
			row.CustomerAbbr = response.data.shippingDeliveries.customerAbbreviation
		} else {
			alert('未查询到发票号码对应的客户简称');
		}
	}).catch(error => {
		console.error(error);
	});
}

const calculation = () => {
	let refundAmount = 0;
	let actualRefundAmount = 0;
	addtaxrefundtableData.value.forEach((element) => {
		refundAmount += element.RefundAmount;
		actualRefundAmount += element.ActualRefundAmount;
	});
	addtaxrefundform2.value.refundAmount = refundAmount.toString();
	addtaxrefundform2.value.actualRefundAmount = actualRefundAmount.toString();
	addtaxrefundform2.value.unrefundedAmount = (refundAmount - actualRefundAmount).toString();
}

const SaveClick = () => {
	if (isEditBtnShow.value == false) {
		ElMessageBox.confirm('确定保存退税单据吗?', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			addTaxRefundInfoRequest.RefundNumber = addctaxrefundform.value.taxRefundNumber;
			addTaxRefundInfoRequest.RefundDate = addctaxrefundform.value.taxRefundDate;
			addTaxRefundInfoRequest.Company = addctaxrefundform.value.ourCompany;
			addTaxRefundInfoRequest.TotalRefundAmount = parseFloat(addtaxrefundform2.value.refundAmount);
			addTaxRefundInfoRequest.ActualRefundAmount = parseFloat(addtaxrefundform2.value.actualRefundAmount);
			addTaxRefundInfoRequest.UnrefundedAmount = parseFloat(addtaxrefundform2.value.unrefundedAmount);
			addTaxRefundInfoRequest.Remark = addtaxrefundform3.value.remarks;
			for (var i = 0; i < addtaxrefundtableData.value.length; i++) {
				if (addtaxrefundtableData.value[i].IsRefunded == 'Y') {
					addtaxrefundtableData.value[i].IsRefunded = true;
				} else {
					addtaxrefundtableData.value[i].IsRefunded = false;
				}
			}
			addTaxRefundInfoRequest.TaxRefundDetail = addtaxrefundtableData.value;
			request.post('TaxRefund/AddTaxRefundinfo/Add', addTaxRefundInfoRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: '退税单据保存成功！',
						type: 'success'
					})
					clearTaxrefundDialog();
					addctaxrefunddialog.value = false;
					GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
				} else {
					console.error('退税单据保存出错');
				}
			}).catch(error => {
				console.error('退税单据保存出错！😔错误内容：', error);
			})
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消保存'
			});
		});
	} else {
		ElMessageBox.confirm('确定保存编辑完成的退税单据吗?', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			addTaxRefundInfoRequest.Id = isEditTaxrefundID.value;
			addTaxRefundInfoRequest.RefundNumber = addctaxrefundform.value.taxRefundNumber;
			addTaxRefundInfoRequest.RefundDate = addctaxrefundform.value.taxRefundDate;
			addTaxRefundInfoRequest.Company = addctaxrefundform.value.ourCompany;
			addTaxRefundInfoRequest.TotalRefundAmount = parseFloat(addtaxrefundform2.value.refundAmount);
			addTaxRefundInfoRequest.ActualRefundAmount = parseFloat(addtaxrefundform2.value.actualRefundAmount);
			addTaxRefundInfoRequest.UnrefundedAmount = parseFloat(addtaxrefundform2.value.unrefundedAmount);
			addTaxRefundInfoRequest.Remark = addtaxrefundform3.value.remarks;
			for (var i = 0; i < addtaxrefundtableData.value.length; i++) {
				if (addtaxrefundtableData.value[i].IsRefunded == 'Y') {
					addtaxrefundtableData.value[i].IsRefunded = true;
				} else {
					addtaxrefundtableData.value[i].IsRefunded = false;
				}
			}

			addTaxRefundInfoRequest.TaxRefundDetail = addtaxrefundtableData.value;
			request.post('TaxRefund/EditTaxRefundInfo/Edit', addTaxRefundInfoRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: '退税单据保存成功！',
						type: 'success'
					})
					clearTaxrefundDialog();
					addctaxrefunddialog.value = false;
					GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
				} else {
					console.error('退税单据保存出错');
				}
			}).catch(error => {
				console.error('退税单据保存出错！😔错误内容：', error);
			})
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消保存'
			});
		});
	}
}

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_ourcompany: [],
		sql_invoicenumber: [],
		sys_yes_no: [],
		sql_hr_customerabbr: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_ourcompany' }, { dictType: 'sql_invoicenumber' }, { dictType: 'sys_yes_no' }, { dictType: 'sql_hr_customerabbr' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
})
/*动态下拉框end*/

///查询条件
const SearchInvoiceNumber = ref('')
const SearchCustomerID = ref('')
const SearchStartDate = ref('')
const SearchEndDate = ref('')

const TaxrefundtableDataTotalItems = ref(0);
const TaxrefundtableDataCurrentPage = ref(1);
const TaxrefundtableDataPageSize = ref(10);
const TaxrefundtableDataHandlePageChange = async (newPage) => {
	TaxrefundtableDataCurrentPage.value = newPage;
	const start = newPage;
	const end = TaxrefundtableDataPageSize.value;
	const newData = await GetTaxRefundInfoList(start, end);
};
function GetTaxRefundInfoList(start, end) {
	request({
		url: 'TaxRefund/GetTaxRefundInfoList/GetList',
		method: 'GET',
		params: {
			InvoiceNumber: SearchInvoiceNumber.value,
			CustomerID: SearchCustomerID.value,
			StartDate: SearchStartDate.value,
			EndDate: SearchEndDate.value
		}
	}).then(response => {
		if (response.data != null) {
			if (response.data.result.length > 0) {
				response.data.result.forEach(item => {
					item.isRefunded = item.isRefunded ? '是' : '否';
					const invoice = state.optionss.sql_invoicenumber.find(x => x.dictValue === item.invoiceNumber);
					item.invoiceNumber = invoice ? invoice.dictLabel : item.invoiceNumber;
				});
				TaxrefundtableData.value = response.data.result;
			} else {
				TaxrefundtableData.value = [];
			}
		}
	}).catch(error => {
		console.error(error);
	});
}

const CheckTaxrefundtableData = (row) => {
	isSaveBtnShow.value = false;
	isDisable.value = true;
	isEdit.value = true;
	isEditTaxrefundID.value = row.id;
	request({
		url: 'TaxRefund/GetTaxRefundDetailsByID/GetTaxRefundDetailsByID',
		method: 'GET',
		params: {
			TaxRefundID: row.id
		}
	}).then(response => {
		if (response.data != null) {
			if (response.data.data.taxRefundInfo != null) {
				addctaxrefundform.value.taxRefundNumber = response.data.data.taxRefundInfo.refundNumber;
				addctaxrefundform.value.taxRefundDate = response.data.data.taxRefundInfo.refundDate;
				addctaxrefundform.value.ourCompany = response.data.data.taxRefundInfo.company;
				addtaxrefundform2.value.refundAmount = response.data.data.taxRefundInfo.totalRefundAmount;
				addtaxrefundform2.value.actualRefundAmount = response.data.data.taxRefundInfo.actualRefundAmount;
				addtaxrefundform2.value.unrefundedAmount = response.data.data.taxRefundInfo.unrefundedAmount;
				addtaxrefundform3.value.remarks = response.data.data.taxRefundInfo.remark;
				response.data.data.taxRefundDetailList.forEach(item => {
					const newRow = {
						InvoiceNumber: item.invoiceNumber,
						RefundAmount: item.refundAmount,
						ActualRefundAmount: item.actualRefundAmount,
						CustomerAbbr: item.customerAbbr,
						IsRefunded: item.isRefunded ? 'Y' : 'N',
						RefundDate: item.refundDate
					};
					addtaxrefundtableData.value.push(newRow);
				});
				addctaxrefunddialog.value = true;
			} else {
				console.error('未查询到退税单据信息');
			}

		}
	}).catch(error => {
		console.error(error);
	});

}

const clearTaxrefundDialog = () => {
	addctaxrefundform.value.taxRefundNumber = '';
	addctaxrefundform.value.taxRefundDate = '';
	addctaxrefundform.value.ourCompany = '';
	addtaxrefundtableData.value = [];
	addtaxrefundform2.value.refundAmount = '';
	addtaxrefundform2.value.actualRefundAmount = '';
	addtaxrefundform2.value.unrefundedAmount = '';
	addtaxrefundform3.value.remarks = '';
	isEdit.value = false;
	isEditBtnShow.value = false;
	isSaveBtnShow.value = true;
	isDisable.value = false;
	isEditTaxrefundID.value = 0;
}

const SearchBtnClick = () => {
	GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
}
const ResetBtnClick = () => {
	SearchInvoiceNumber.value = '';
	SearchCustomerID.value = '';
	SearchStartDate.value = '';
	SearchEndDate.value = '';
	GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
}

const EditBtnClick = () => {
	isEdit.value = false;
	isEditBtnShow.value = true;
	isSaveBtnShow.value = false;
	isDisable.value = false;
}

const TaxrefundtableData = ref([]); //退税单据表格数据
const addTaxRefundInfoRequest = reactive({
	Id: 0,
	RefundNumber: '',
	RefundDate: '',
	Company: '',
	TotalRefundAmount: 0,
	ActualRefundAmount: 0,
	UnrefundedAmount: 0,
	Remark: '',
	IsDelete: 0,
	TaxRefundDetail: []
})

const addtaxrefundform2 = ref({
	refundAmount: '',
	actualRefundAmount: '',
	unrefundedAmount: ''
})

const addtaxrefundform3 = ref({
	remarks: ''
})
</script>