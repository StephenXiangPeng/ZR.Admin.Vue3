<template>
	<div>
		<!-- 退税管理表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="openaddctaxrefunddialog" size="default">新增退税</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
					<el-col :span="4">
						<el-date-picker v-model="SearchStartDate" type="date" placeholder="请选择退税日期起" style="width: 100%"
							size="default" />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchEndDate" type="date" placeholder="请选择退税日期止" style="width: 100%"
							size="default" />
					</el-col>
					<el-col :span="4">
						<el-select v-model="SearchInvoiceNumber" placeholder="选择发票号码" style="width: 100%" size="default"
							clearable>
							<el-option v-for="dict in optionss.sql_settlement_center_shipping" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="SearchCustomerID" filterable placeholder="选择客户简称" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchBtnClick()" size="default">查询</el-button>
							<el-button @click="ResetBtnClick()" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table :data="TaxrefundtableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="refundNumber" label="退税编号" width="130"></el-table-column>
				<el-table-column prop="refundDate" label="退税日期" width="110">
					<template #default="scope">
						{{ formatDate(scope.row.refundDate) }}
					</template>
				</el-table-column>
				<el-table-column prop="company" label="我方公司" width="110"></el-table-column>
				<el-table-column prop="totalRefundAmount" label="应退税总额" width="110"></el-table-column>
				<el-table-column prop="actualRefundAmount" label="实际退税额" width="110"></el-table-column>
				<el-table-column prop="unrefundedAmount" label="未退税额" width="110"></el-table-column>
				<el-table-column prop="remark" label="备注" width="200"></el-table-column>
				<el-table-column fixed="right" label="操作" width="200">
					<template #default="scope">
						<el-button type="text" size="small"
							@click="CheckTaxrefundtableData(scope.row)">查看/编辑</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="TaxrefundtableDataHandlePageChange"
				:current-page="TaxrefundtableDataCurrentPage" :page-size="TaxrefundtableDataPageSize"
				:total="TaxrefundtableDataTotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		</div>
		<el-dialog :modal="false" modal-penetrable v-model="addctaxrefunddialog" title="新增退税"
			:close-on-click-modal=false style="width: 75%;" @close="clearTaxrefundDialog()">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-form :model="addctaxrefundform" label-width="120px" :show-message="false">
						<el-row :gutter="15">
							<el-col :span="6">
								<el-form-item label="退税编号">
									<el-input v-model="addctaxrefundform.taxRefundNumber" placeholder="请输入退税编号"
										style="width: 300px;" disabled size="default" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="退税日期">
									<el-date-picker v-model="addctaxrefundform.taxRefundDate" type="date"
										placeholder="请选择退税日期" style="width: 300px;" :disabled=isDisable
										size="default"></el-date-picker>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="我方公司">
									<el-select v-model="addctaxrefundform.ourCompany" placeholder="请选择我方公司"
										style="width: 300px" :disabled=isDisable size="default" clearable>
										<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="">
									<!-- 占位列 -->
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<el-collapse v-model="detailInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="退税明细" name="detailInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">退税明细</span>
					</template>
					<el-button class="mt-4" type="primary" @click="AddTaxRefundDetailsClick"
						style="margin-bottom: 10px;" :disabled="isDisable" size="default">添加退税明细</el-button>
					<el-table :data="addtaxrefundtableData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }" :disable="true">
						<el-table-column prop="Id" label="Id" v-if="false">
							<template #default="{ row }">
								<el-input v-model="row.Id" size="small" :disabled=isDisable></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="invoiceNumber" label="发票号码">
							<template #default="{ row }">
								<el-select v-model="row.InvoiceNumber" placeholder="选择发票号码" clearable size="default"
									@change="invoiceNumberChange(row)" :disabled=isDisable>
									<el-option v-for="dict in optionss.sql_settlement_center_shipping"
										:key="dict.dictCode" :label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="CustomerAbbr" label="客户简称">
							<template #default="{ row }">
								<el-input v-model="row.CustomerAbbr" size="default" :disabled=isDisable></el-input>
							</template>
						</el-table-column>
						<el-table-column prop=" RefundAmount" label="应退税额">
							<template #default="{ row }">
								<el-input v-model="row.RefundAmount" placeholder="应退税额" size="default" :precision="3"
									@change="calculation" :disabled=isDisable></el-input>
							</template>
						</el-table-column>
						<el-table-column prop=" ActualRefundAmount" label="实际退税额">
							<template #default="{ row }">
								<el-input v-model="row.ActualRefundAmount" placeholder="输入实际退税额" size="default"
									:precision="3" @input="validateAndCalculate(row)" :disabled=isDisable>
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="IsRefunded" label="是否已退">
							<template #default="{ row }">
								<el-select v-model="row.IsRefunded" placeholder="选择是否" size="default" clearable
									:disabled=isDisable>
									<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="RefundDate" label="退税日期">
							<template #default="{ row }">
								<el-date-picker v-model="row.RefundDate" type="date" placeholder="请选择退税日期"
									size="default" style="width: 150px;" :disabled=isDisable></el-date-picker>
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作">
							<template #default="scope">
								<el-button type="text" size="small"
									@click="addtaxrefundtableDatahandleDelete(scope.$index)"
									:disabled=isDisable>删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="summaryInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="合计信息" name="summaryInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">合计信息</span>
					</template>
					<el-form :model="addtaxrefundform2" label-width="120px" :show-message="false">
						<el-row :gutter="15">
							<el-col :span="6">
								<el-form-item label="应退税总额">
									<el-input v-model="addtaxrefundform2.refundAmount" placeholder="请输入应退税总额"
										style="width: 300px;" :disabled=isDisable size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="实际退税额">
									<el-input v-model="addtaxrefundform2.actualRefundAmount" placeholder="请输入实际退税额"
										style="width: 300px;" :disabled=isDisable size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="未退税额">
									<el-input v-model="addtaxrefundform2.unrefundedAmount" placeholder="请输入未退税额"
										style="width: 300px;" :disabled=isDisable size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="">
									<!-- 占位列 -->
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="remarkInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="备注信息" name="remarkInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">备注信息</span>
					</template>
					<el-form :model="addtaxrefundform3" label-width="120px" :show-message="false">
						<el-row :gutter="15">
							<el-col :span="24">
								<el-form-item label="备注信息">
									<el-input type="textarea" :rows="10" placeholder="在此输入文字"
										v-model="addtaxrefundform3.remarks" :disabled=isDisable></el-input>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="SaveClick()" v-if="isSaveBtnShow">
						保存
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
import { createApp, ref, reactive, toRefs, getCurrentInstance } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile, ElCollapse, ElCollapseItem, ElRow, ElCol, ElSelect, ElOption, ElDatePicker, ElInput, ElPagination } from 'element-plus'
import request from '@/utils/request';

const isDisable = ref(false);
const isEdit = ref(false);
const isEditBtnShow = ref(false);
const isSaveBtnShow = ref(true);
const isEditTaxrefundID = ref(0);

// 折叠面板状态
const basicInfoCollapseActive = ref(['basicInfo']);
const detailInfoCollapseActive = ref(['detailInfo']);
const summaryInfoCollapseActive = ref(['summaryInfo']);
const remarkInfoCollapseActive = ref(['remarkInfo']);
const addctaxrefundform = ref({
	taxRefundNumber: '',
	taxRefundDate: '',
	ourCompany: ''
})
const addctaxrefunddialog = ref(false)
const currentDate = new Date();
const formattedDate = currentDate.toISOString().split('T')[0];
const openaddctaxrefunddialog = async () => {
	// 重置表单
	clearTaxrefundDialog();

	// 获取新的退税单编号
	await getNextTaxRefundNumber();

	// 设置默认日期为当天
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	addctaxrefundform.value.taxRefundDate = `${year}-${month}-${day}`;

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

// 验证并计算金额
const validateAndCalculate = (row) => {
	// 转换为数字进行比较
	const refundAmount = parseFloat(row.RefundAmount) || 0;
	const actualAmount = parseFloat(row.ActualRefundAmount) || 0;

	// 检查实际退税额是否大于应退税额
	if (actualAmount > refundAmount) {
		ElMessage.warning('实际退税额不能大于应退税额');
		// 重置为应退税额
		row.ActualRefundAmount = refundAmount.toString();
	}

	// 重新计算总额
	calculation();
}

// 修改原有的计算方法
const calculation = () => {
	let refundAmount = 0;
	let actualRefundAmount = 0;

	addtaxrefundtableData.value.forEach((element) => {
		// 将字符串转换为数字并累加
		refundAmount += parseFloat(element.RefundAmount) || 0;
		actualRefundAmount += parseFloat(element.ActualRefundAmount) || 0;
	});

	// 更新合计信息
	addtaxrefundform2.value.refundAmount = refundAmount.toFixed(2);
	addtaxrefundform2.value.actualRefundAmount = actualRefundAmount.toFixed(2);
	addtaxrefundform2.value.unrefundedAmount = (refundAmount - actualRefundAmount).toFixed(2);
}


// 在输入应退税额时也需要验证
const handleRefundAmountChange = (row) => {
	const refundAmount = parseFloat(row.RefundAmount) || 0;
	const actualAmount = parseFloat(row.ActualRefundAmount) || 0;

	// 如果修改应退税额后小于实际退税额，则调整实际退税额
	if (refundAmount < actualAmount) {
		row.ActualRefundAmount = refundAmount.toString();
		ElMessage.warning('已自动调整实际退税额以匹配应退税额');
	}

	calculation();
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
		sql_settlement_center_shipping: [],
		sys_yes_no: [],
		sql_hr_customer: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'sql_settlement_center_shipping' },
	{ dictType: 'sys_yes_no' },
	{ dictType: 'sql_hr_customer' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	GetTaxRefundInfoList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
})
/*动态下拉框end*/

// 添加日期格式化函数
const formatDate = (dateString) => {
	if (!dateString) return '';
	// 处理包含时间的日期字符串
	return dateString.split(' ')[0];
}

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
				TaxrefundtableData.value = response.data.result;
				TaxrefundtableData.value.forEach(item => {
					item.refundDate = formatDate(item.refundDate); // 格式化日期
					item.company = state.optionss.hr_ourcompany.find(x => x.dictValue === item.company).dictLabel;
				});
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
						Id: item.id,
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

// 定义获取退税单编号的方法
const getNextTaxRefundNumber = async () => {
	try {
		const response = await request({
			url: 'TaxRefund/GetNextTaxReFundNumber/GetNextNumber',
			method: 'GET'
		});

		if (response.code === 200) {
			// 将获取到的编号设置到表单中
			addctaxrefundform.value.taxRefundNumber = response.data;
		} else {
			ElMessage.error('获取退税单编号失败');
		}
	} catch (error) {
		console.error('Error:', error);
		ElMessage.error('获取退税单编号失败，请稍后重试');
	}
}
</script>