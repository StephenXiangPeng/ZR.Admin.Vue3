<template>
	<div class="taxrefund-page">
		<div class="page-title">
			<div class="title">退税管理</div>
			<div class="subtitle">管理退税信息，快速追踪已退税信息</div>
		</div>

		<div class="filter-card">
			<el-row :gutter="16">
				<el-col :span="7">
					<el-input v-model="SearchInvoiceNumber" placeholder="请输入发票号码" clearable size="default" />
				</el-col>
				<el-col :span="7">
					<el-input v-model="SearchCustomerID" placeholder="请输入客户简称" clearable size="default" />
				</el-col>
				<el-col :span="6">
					<el-select v-model="SearchTaxRefundStatus" placeholder="全部状态" style="width: 100%" size="default"
						clearable>
						<el-option v-for="dict in optionss.hr_taxrefundstatus" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-col>
				<el-col :span="4" class="filter-actions">
					<el-button type="primary" :icon="Search" @click="SearchBtnClick()" size="default">查询</el-button>
					<el-button :icon="Refresh" @click="ResetBtnClick()" size="default">重置</el-button>
				</el-col>
			</el-row>
		</div>

		<div class="table-card">
			<div class="table-actions">
				<div class="table-count">共 {{ tableCount }} 条记录</div>
			</div>
			<el-table :data="TaxrefundtableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#f8f9fb', color: '#374151', fontWeight: '600' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '6px 0' }">
				<el-table-column prop="invoiceNumber" label="发票号码" width="120">
					<template #default="scope">
						{{ scope.row.invoiceNumber || scope.row.refundNumber || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="salesContractNumber" label="销售合同号" width="120">
					<template #default="scope">
						{{ scope.row.salesContractNumber || scope.row.contractNumber || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="customerAbbr" label="客户简称" width="100">
					<template #default="scope">
						{{ scope.row.customerAbbr || scope.row.customerAbbreviation || scope.row.customer || '-' }}
					</template>
				</el-table-column>
				<el-table-column prop="contractAmount" label="合同金额" width="120" align="right">
					<template #default="scope">
						{{ formatAmount(scope.row.contractAmount ?? scope.row.totalRefundAmount) }}
					</template>
				</el-table-column>
				<el-table-column prop="refundAmount" label="应退税额" width="110" align="right">
					<template #default="scope">
						<span class="amount-warn">{{ formatAmount(scope.row.refundAmount ?? scope.row.totalRefundAmount,
							4)
						}}</span>
					</template>
				</el-table-column>
				<el-table-column prop="actualRefundAmount" label="已退税额" width="110" align="right">
					<template #default="scope">
						<span class="amount-success">{{ formatAmount(scope.row.actualRefundAmount ??
							scope.row.refundedAmount, 4) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="refundStatus" label="状态" width="100">
					<template #default="scope">
						<el-tag :type="getRefundStatus(scope.row).type" size="small" effect="light">
							{{ getRefundStatus(scope.row).label }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="refundDate" label="退税日期" width="110">
					<template #default="scope">
						{{ formatDate(scope.row.refundDate) || '-' }}
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="140">
					<template #default="scope">
						<el-button v-if="shouldShowRegister(scope.row)" type="success" link :icon="Check"
							@click="openRegisterTaxRefund(scope.row)">登记</el-button>
						<el-button v-if="isPartialRefund(scope.row)" type="primary" link
							@click="openTaxRefundForView(scope.row)">查看</el-button>
						<el-button v-if="isRefunded(scope.row)" type="primary" link
							@click="openTaxRefundForView(scope.row)">查看</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="TaxrefundtableDataHandlePageChange"
				:current-page="TaxrefundtableDataCurrentPage" :page-size="TaxrefundtableDataPageSize"
				:total="TaxrefundtableDataTotalItems" background layout="prev, pager, next" />
		</div>
		<el-dialog class="taxrefund-dialog" :modal="false" modal-penetrable v-model="addctaxrefunddialog"
			:close-on-click-modal=false width="560px" @close="clearTaxrefundDialog()">
			<template #header>
				<div class="dialog-header">
					<div class="dialog-title">{{ dialogTitle }}</div>
					<div class="dialog-subtitle">{{ dialogSubtitle }}</div>
				</div>
			</template>
			<div class="dialog-content">
				<div class="dialog-summary">
					<div class="summary-grid">
						<div class="summary-item">
							<div class="summary-label">发票号码</div>
							<div class="summary-value">
								<template v-if="dialogMode === 'create'">
									<el-select v-model="activeDetailRow.InvoiceNumber" placeholder="选择发票号码" clearable
										size="default" style="width: 100%;"
										@change="invoiceNumberChange(activeDetailRow)">
										<el-option v-for="dict in optionss.sql_settlement_center_shipping"
											:key="dict.dictCode" :label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
								<template v-else>
									{{ dialogSummary.invoiceNumber }}
								</template>
							</div>
						</div>
						<div class="summary-item">
							<div class="summary-label">销售合同号</div>
							<div class="summary-value">{{ dialogSummary.salesContractNumber }}</div>
						</div>
						<div class="summary-item">
							<div class="summary-label">客户简称</div>
							<div class="summary-value">{{ dialogSummary.customerAbbr }}</div>
						</div>
						<div class="detail-item">
							<div class="detail-label">合同金额</div>
							<div class="detail-value">¥{{ formatAmount(dialogSummary.contractAmount) }}</div>
						</div>
						<div class="summary-item">
							<div class="summary-label">应退税额</div>
							<div class="summary-value amount-warn">¥{{ formatAmount(dialogSummary.refundAmount, 4) }}
							</div>
						</div>
						<div class="summary-item">
							<div class="summary-label">已退税额</div>
							<div class="summary-value amount-success">¥{{ formatAmount(dialogSummary.refundedAmount, 4)
							}}</div>
						</div>
					</div>
					<div class="summary-divider"></div>
					<!-- <div class="summary-remaining">
						<div class="summary-label">剩余可退</div>
						<div class="summary-value amount-primary">¥{{ formatAmount(dialogSummary.remainingAmount, 4) }}</div>
					</div> -->
				</div>

				<div v-if="dialogMode !== 'view'" class="dialog-form">
					<el-form :model="addctaxrefundform" label-width="0" :show-message="false">
						<el-form-item class="form-item" label="">
							<div class="form-label">我方公司<span class="required">*</span></div>
							<el-select v-model="addctaxrefundform.ourCompany" placeholder="请选择我方公司" style="width: 100%"
								size="default" clearable>
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
						<el-form-item class="form-item" label="">
							<div class="form-label">本次实际退税额<span class="required">*</span></div>
							<el-input v-model="registerRefundAmount" placeholder="请输入退税金额" size="default">
								<template #prefix>¥</template>
							</el-input>
						</el-form-item>
						<el-form-item class="form-item" label="">
							<div class="form-label">退税日期<span class="required">*</span></div>
							<el-date-picker v-model="registerRefundDate" type="date" placeholder="请选择退税日期"
								size="default" style="width: 100%;" />
						</el-form-item>
						<el-form-item class="form-item" label="">
							<div class="form-label">备注</div>
							<el-input type="textarea" :rows="4" placeholder="请输入备注信息（可选）"
								v-model="addtaxrefundform3.remarks" />
						</el-form-item>
					</el-form>
				</div>

				<div v-else class="dialog-detail">
					<div class="detail-grid">
						<div class="detail-item">
							<div class="detail-label">状态</div>
							<el-tag :type="getRefundStatus(selectedRow || {}).type" size="small" effect="light">
								{{ getRefundStatus(selectedRow || {}).label }}
							</el-tag>
						</div>
					</div>
					<div class="summary-remaining summary-remaining--view">
						<div class="summary-label">剩余可退</div>
						<div class="summary-value amount-primary">¥{{ formatAmount(dialogSummary.remainingAmount, 4) }}
						</div>
					</div>

					<div class="refund-records">
						<div class="records-title">退税记录</div>
						<div v-if="refundRecords.length === 0" class="records-empty">暂无退税记录</div>
						<div v-else class="records-list">
							<div class="record-item" v-for="(record, index) in refundRecords" :key="record.key">
								<div class="record-info">
									<div class="record-index">第{{ index + 1 }}次</div>
									<div class="record-date">{{ record.date }}</div>
									<div class="record-remark">{{ record.remark }}</div>
								</div>
								<div class="record-amount">+¥{{ formatAmount(record.amount, 4) }}</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="SaveClick()" v-if="dialogMode !== 'view'">
						{{ dialogMode === 'register' ? '确认退税' : '保存' }}
					</el-button>
					<el-button @click="closeDialog()" v-else>关闭</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref, reactive, toRefs, getCurrentInstance, computed, watch } from 'vue'
import { Search, Refresh, Check, View } from '@element-plus/icons-vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile, ElCollapse, ElCollapseItem, ElRow, ElCol, ElSelect, ElOption, ElDatePicker, ElInput, ElPagination, ElTag } from 'element-plus'
import request from '@/utils/request';

const isDisable = ref(false);
const isEdit = ref(false);
const isEditBtnShow = ref(false);
const isSaveBtnShow = ref(true);
const isEditTaxrefundID = ref(0);
type DialogMode = 'create' | 'register' | 'view';
const dialogMode = ref<DialogMode>('create');
const dialogTitle = computed(() => {
	if (dialogMode.value === 'view') return '退税详情';
	if (dialogMode.value === 'register') return '登记退税';
	return '新增退税';
});
const dialogSubtitle = computed(() => {
	if (dialogMode.value === 'view') return '查看退税单据与记录';
	if (dialogMode.value === 'register') return '登记本次退税信息';
	return '新增退税记录并维护明细';
});
const selectedRow = ref<any>(null);

const applyDialogMode = (mode: DialogMode) => {
	dialogMode.value = mode;
	if (mode === 'view') {
		isDisable.value = true;
		isEdit.value = false;
		isEditBtnShow.value = false;
		isSaveBtnShow.value = false;
		return;
	}
	isDisable.value = false;
	isEdit.value = false;
	isEditBtnShow.value = mode !== 'create';
	isSaveBtnShow.value = true;
}

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
	selectedRow.value = null;
	applyDialogMode('create');

	// 获取新的退税单编号
	await getNextTaxRefundNumber();

	// 设置默认日期为当天
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	addctaxrefundform.value.taxRefundDate = `${year}-${month}-${day}`;

	addctaxrefunddialog.value = true
	ensureDetailRow();
	addctaxrefundform.value.taxRefundDate = formattedDate;
}

const closeDialog = () => {
	addctaxrefunddialog.value = false;
}
const addtaxrefundtableData = ref([])
const ensureDetailRow = () => {
	if (addtaxrefundtableData.value.length === 0) {
		AddTaxRefundDetailsClick();
	}
}
const activeDetailRow = computed(() => addtaxrefundtableData.value[0] || {});
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
			row.RefundAmount = response.data.taxRefundAmount;
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

const registerRefundAmount = ref('');
const registerRefundDate = ref(formattedDate);
const clampRegisterRefundAmount = () => {
	const totalRefund = Number(selectedRow.value?.refundAmount ?? selectedRow.value?.taxRefundTotal ?? 0);
	const refunded = Number(selectedRow.value?.refundedAmount ?? selectedRow.value?.refundAmountSum ?? 0);
	const remaining = Math.max(totalRefund - refunded, 0);
	const current = Number(registerRefundAmount.value || 0);

	if (current > remaining) {
		ElMessage.warning('本次实际退税额不能大于应退税额');
		registerRefundAmount.value = remaining ? remaining.toFixed(4) : '';
	}
};

watch(() => registerRefundAmount.value, () => {
	clampRegisterRefundAmount();
});

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
	if (dialogMode.value === 'register') {
		if (!addctaxrefundform.value.ourCompany) {
			ElMessage.warning('请选择我方公司');
			return;
		}
		if (!registerRefundAmount.value || Number(registerRefundAmount.value) <= 0) {
			ElMessage.warning('请输入本次实际退税额');
			return;
		}
		ElMessageBox.confirm('确定确认本次退税吗?', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			const payload = {
				InvoiceId: selectedRow.value?.id,
				SalesContractId: selectedRow.value?.contractId
					?? selectedRow.value?.salesContractId
					?? selectedRow.value?.salesContractID,
				RefundNo: addctaxrefundform.value.taxRefundNumber,
				RefundDate: registerRefundDate.value,
				RefundAmount: Number(registerRefundAmount.value || 0),
				Company: addctaxrefundform.value.ourCompany,
				Remark: addtaxrefundform3.value.remarks
			};

			request.post('TaxRefund/AddTaxRefund/AddTaxRefund', payload).then(response => {
				if (response != null) {
					ElMessage({
						message: '退税登记成功！',
						type: 'success'
					})
					clearTaxrefundDialog();
					addctaxrefunddialog.value = false;
					GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
				} else {
					console.error('退税登记出错');
				}
			}).catch(error => {
				console.error('退税登记出错！😔错误内容：', error);
			})
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消操作'
			});
		});
		return;
	}
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
					GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
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
					GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
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
		sql_hr_customer: [],
		hr_taxrefundstatus: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'sql_settlement_center_shipping' },
	{ dictType: 'sys_yes_no' },
	{ dictType: 'sql_hr_customer' },
	{ dictType: 'hr_taxrefundstatus' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
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
const SearchTaxRefundStatus = ref('')
const SearchStartDate = ref('')
const SearchEndDate = ref('')
const TaxrefundtableData = ref([]); //退税单据表格数据
const tableCount = computed(() => TaxrefundtableData.value.length)

const formatAmount = (value, digits = 2) => {
	const amount = Number(value);
	if (!Number.isFinite(amount)) return '-';
	return amount.toLocaleString('en-US', { minimumFractionDigits: digits, maximumFractionDigits: digits });
}

type RefundStatusTag = {
	label: string;
	type: 'success' | 'warning' | 'info' | 'danger' | 'primary';
};

const getRefundStatus = (row): RefundStatusTag => {
	const statusValue = row.taxRefundStatus ?? row.refundStatus ?? row.status ?? row.taxRefundStatusValue;
	if (statusValue !== undefined && statusValue !== null && statusValue !== '') {
		const matched = state.optionss.hr_taxrefundstatus.find(item =>
			item.dictValue === statusValue || item.dictValue === statusValue.toString()
		);
		if (matched) {
			const label = matched.dictLabel;
			if (label.includes('已退税')) return { label, type: 'success' };
			if (label.includes('部分')) return { label, type: 'info' };
			return { label, type: 'warning' };
		}
	}

	const refundAmount = Number(row.refundAmount ?? row.totalRefundAmount ?? 0);
	const actualAmount = Number(row.actualRefundAmount ?? row.refundedAmount ?? 0);
	if (refundAmount <= 0 && actualAmount <= 0) return { label: '待退税', type: 'warning' };
	if (actualAmount <= 0) return { label: '待退税', type: 'warning' };
	if (actualAmount >= refundAmount) return { label: '已退税', type: 'success' };
	return { label: '部分退税', type: 'info' };
}

const isPartialRefund = (row) => getRefundStatus(row).label.includes('部分');
const isPendingRefund = (row) => getRefundStatus(row).label.includes('待');
const isRefunded = (row) => getRefundStatus(row).label.includes('已退税');
const shouldShowRegister = (row) => isPendingRefund(row) || isPartialRefund(row);

const openTaxRefundForView = async (row) => {
	selectedRow.value = row;
	applyDialogMode('view');
	addctaxrefunddialog.value = true;
	await fetchTaxRefundRecords(row.id);
}
const openRegisterTaxRefund = async (row) => {
	selectedRow.value = row;
	applyDialogMode('register');
	addtaxrefundtableData.value = [];
	addtaxrefundform3.value.remarks = '';
	addctaxrefundform.value.ourCompany = '';
	registerRefundDate.value = formattedDate;

	const totalRefund = Number(row.refundAmount ?? row.taxRefundTotal ?? 0);
	const refunded = Number(row.refundedAmount ?? row.refundAmountSum ?? 0);
	const remaining = Math.max(totalRefund - refunded, 0);
	registerRefundAmount.value = remaining ? remaining.toFixed(4) : '';

	addtaxrefundtableData.value.push({
		InvoiceNumber: row.invoiceNumber,
		RefundAmount: totalRefund,
		ActualRefundAmount: refunded,
		CustomerAbbr: row.customerAbbr || row.customerAbbreviation,
		IsRefunded: refunded >= totalRefund ? 'Y' : 'N',
		RefundDate: registerRefundDate.value
	});
	ensureDetailRow();
	await getNextTaxRefundNumber();
	addctaxrefunddialog.value = true;
}

const TaxrefundtableDataTotalItems = ref(0);
const TaxrefundtableDataCurrentPage = ref(1);
const TaxrefundtableDataPageSize = ref(10);
const TaxrefundtableDataHandlePageChange = async (newPage) => {
	TaxrefundtableDataCurrentPage.value = newPage;
	const start = newPage;
	const end = TaxrefundtableDataPageSize.value;
	const newData = await GetTaxRefundList(start, end);
};
function GetTaxRefundList(start, end) {
	request({
		url: 'TaxRefund/GetTaxRefundList/GetTaxRefundList',
		method: 'GET',
		params: {
			PageNum: start,
			PageSize: end,
			InvoiceNumber: SearchInvoiceNumber.value,
			CustomerID: SearchCustomerID.value,
			TaxRefundStatus: SearchTaxRefundStatus.value,
			StartDate: SearchStartDate.value,
			EndDate: SearchEndDate.value
		}
	}).then(response => {
		const payload = response.data;
		const list = payload?.result ?? [];
		if (list.length > 0) {
			TaxrefundtableData.value = list.map(item => ({
				...item,
				contractAmount: item.amountTotal,
				refundAmount: item.taxRefundTotal,
				refundedAmount: item.refundAmountSum,
				actualRefundAmount: item.refundAmountSum,
				taxRefundStatus: item.taxRefundStatus,
				contractId: item.contractId,
				customerAbbr: item.customerAbbreviation,
				refundDate: item.lastRefundDateText && item.lastRefundDateText !== '无'
					? item.lastRefundDateText
					: formatDate(item.lastRefundDateRaw)
			}));
			TaxrefundtableDataTotalItems.value = payload?.totalNum ?? list.length;
		} else {
			TaxrefundtableData.value = [];
			TaxrefundtableDataTotalItems.value = payload?.totalNum ?? 0;
		}
	}).catch(error => {
		console.error(error);
	});
}

const CheckTaxrefundtableData = (row) => {
	isEditTaxrefundID.value = row.id;
	addtaxrefundtableData.value = [];
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
				ensureDetailRow();
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
	selectedRow.value = null;
	dialogMode.value = 'create';
	registerRefundAmount.value = '';
	registerRefundDate.value = formattedDate;
	taxRefundRecords.value = [];
}

const SearchBtnClick = () => {
	GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
}
const ResetBtnClick = () => {
	SearchInvoiceNumber.value = '';
	SearchCustomerID.value = '';
	SearchTaxRefundStatus.value = '';
	SearchStartDate.value = '';
	SearchEndDate.value = '';
	GetTaxRefundList(TaxrefundtableDataCurrentPage.value, TaxrefundtableDataPageSize.value);
}

const EditBtnClick = () => {
	isEdit.value = false;
	isEditBtnShow.value = true;
	isSaveBtnShow.value = false;
	isDisable.value = false;
}

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
const taxRefundRecords = ref([]);

const fetchTaxRefundRecords = async (invoiceId) => {
	if (!invoiceId) {
		taxRefundRecords.value = [];
		return;
	}
	try {
		const response = await request({
			url: 'TaxRefund/GetTaxRefundListByInvoiceID/GetTaxRefundList',
			method: 'GET',
			params: {
				InvoiceID: invoiceId
			}
		});
		if (response.code === 200) {
			taxRefundRecords.value = response.data || [];
		} else {
			taxRefundRecords.value = [];
		}
	} catch (error) {
		console.error(error);
		taxRefundRecords.value = [];
	}
};

const dialogSummary = computed(() => {
	const row = selectedRow.value || {};
	const detail = activeDetailRow.value || {};
	const totalRefund = Number(row.refundAmount ?? row.taxRefundTotal ?? detail.RefundAmount ?? addtaxrefundform2.value.refundAmount ?? row.totalRefundAmount ?? 0);
	const refunded = Number(row.refundedAmount ?? row.refundAmountSum ?? detail.ActualRefundAmount ?? addtaxrefundform2.value.actualRefundAmount ?? row.actualRefundAmount ?? 0);
	const currentRefund = dialogMode.value === 'register' ? Number(registerRefundAmount.value || 0) : 0;
	const remaining = totalRefund - refunded - currentRefund;

	return {
		invoiceNumber: row.invoiceNumber || detail.InvoiceNumber || row.refundNumber || '-',
		customerAbbr: detail.CustomerAbbr || row.customerAbbr || row.customerAbbreviation || row.customer || '-',
		salesContractNumber: row.salesContractNumber || row.contractNumber || '-',
		contractAmount: Number(row.contractAmount ?? row.amountTotal ?? row.totalRefundAmount ?? 0),
		refundAmount: totalRefund,
		refundedAmount: refunded,
		remainingAmount: remaining
	};
});

const refundRecords = computed(() => {
	return taxRefundRecords.value.map((item, index) => ({
		key: item.id ?? index,
		date: formatDate(item.refundDate) || '-',
		remark: item.remark || `第${index + 1}次退税`,
		amount: Number(item.refundAmount ?? 0)
	}));
});

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
<style scoped>
.taxrefund-page {
	background: #f5f6f8;
	padding: 16px;
	min-height: 100%;
}

.page-title .title {
	font-size: 18px;
	font-weight: 600;
	color: #111827;
}

.page-title .subtitle {
	margin-top: 4px;
	font-size: 12px;
	color: #6b7280;
}

.filter-card,
.table-card {
	background: #ffffff;
	border: 1px solid #e5e7eb;
	border-radius: 8px;
	padding: 16px;
	margin-top: 12px;
}

.filter-actions {
	display: flex;
	gap: 8px;
	align-items: center;
}

.table-actions {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 12px;
}

.table-action-left {
	display: flex;
	gap: 8px;
	align-items: center;
}

.table-count {
	font-size: 12px;
	color: #6b7280;
}

.amount-warn {
	color: #ef4444;
}

.amount-success {
	color: #16a34a;
}

.amount-primary {
	color: #3b82f6;
	font-weight: 600;
}

.dialog-content {
	display: flex;
	flex-direction: column;
	gap: 16px;
}

.dialog-summary {
	background: #f8f9fb;
	border: 1px solid #eef0f3;
	border-radius: 8px;
	padding: 16px;
}

.summary-grid {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	gap: 12px 20px;
}

.summary-item {
	display: flex;
	flex-direction: column;
	gap: 6px;
}

.summary-label {
	font-size: 12px;
	color: #6b7280;
}

.summary-value {
	font-size: 14px;
	color: #111827;
	font-weight: 600;
}

.summary-divider {
	height: 1px;
	background: #eef0f3;
	margin: 12px 0;
}

.summary-remaining {
	display: flex;
	flex-direction: column;
	gap: 6px;
}

.dialog-form .form-item {
	margin-bottom: 14px;
}

.form-label {
	font-size: 13px;
	color: #111827;
	margin-bottom: 6px;
}

.required {
	color: #ef4444;
	margin-left: 4px;
}

.dialog-detail {
	display: flex;
	flex-direction: column;
	gap: 12px;
}

.detail-grid {
	display: grid;
	grid-template-columns: repeat(2, minmax(0, 1fr));
	gap: 12px 20px;
}

.detail-item {
	display: flex;
	flex-direction: column;
	gap: 6px;
}

.detail-label {
	font-size: 12px;
	color: #6b7280;
}

.detail-value {
	font-size: 14px;
	font-weight: 600;
	color: #111827;
}

.amount-box {
	background: #ffffff;
	border: 1px solid #eef0f3;
	border-radius: 8px;
	padding: 10px 12px;
}

.summary-remaining--view {
	background: #eef4ff;
	border-radius: 8px;
	padding: 12px;
}

.refund-records {
	margin-top: 8px;
}

.records-title {
	font-size: 14px;
	font-weight: 600;
	color: #111827;
	margin-bottom: 8px;
}

.records-empty {
	font-size: 12px;
	color: #9ca3af;
	padding: 8px 0;
}

.records-list {
	display: flex;
	flex-direction: column;
	gap: 10px;
}

.record-item {
	display: flex;
	justify-content: space-between;
	align-items: center;
	border: 1px solid #eef0f3;
	border-radius: 8px;
	padding: 12px;
	background: #ffffff;
}

.record-info {
	display: flex;
	gap: 12px;
	align-items: center;
	font-size: 12px;
	color: #6b7280;
}

.record-index {
	color: #111827;
	font-weight: 600;
}

.record-amount {
	color: #16a34a;
	font-weight: 600;
}

:deep(.taxrefund-dialog .el-dialog) {
	border-radius: 10px;
	overflow: hidden;
	max-width: 92vw;
}

:deep(.taxrefund-dialog .el-dialog__header) {
	padding: 16px 20px 8px;
	border-bottom: 1px solid #eef0f3;
}

.dialog-header {
	display: flex;
	flex-direction: column;
	gap: 4px;
}

.dialog-title {
	font-size: 16px;
	font-weight: 600;
	color: #111827;
}

.dialog-subtitle {
	font-size: 12px;
	color: #6b7280;
}

:deep(.taxrefund-dialog .el-dialog__body) {
	padding: 16px 20px 4px;
	background: #fbfbfc;
}

:deep(.taxrefund-dialog .el-dialog__footer) {
	padding: 12px 20px 16px;
	border-top: 1px solid #eef0f3;
}
</style>