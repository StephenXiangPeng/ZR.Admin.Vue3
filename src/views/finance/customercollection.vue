<template>
	<div>
		<!-- 客户收款表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="openAddCustomerCollection"
								size="default">新增客户收款</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
					<el-col :span="4">
						<el-select v-model="SearchReceiptNumber" filterable placeholder="选择收款单号" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.sql_customercollections_no" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="SearchBank" filterable placeholder="选择收汇银行" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in searchBankOptions" :key="dict.dictCode" :label="dict.dictLabel"
								:value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchReceiptDateStart" type="date" placeholder="请选择收汇日期起"
							style="width: 100%" size="default" />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchReceiptDateEnd" type="date" placeholder="请选择收汇日期止"
							style="width: 100%" size="default" />
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchSubmit()" size="default">查询</el-button>
							<el-button @click="resetSearch()" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table :data="customercollectiontableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="receiptNumber" label="收款单号" width="120">
					<template #default="scope">
						<span>{{ scope.row.receiptNumber }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="receiptDate" label="收汇日期" width="110"
					:formatter="(row, column, cellValue) => formatDate(cellValue)"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="110"></el-table-column>
				<el-table-column prop="foreignCurrency" label="外销币种" width="90"></el-table-column>
				<el-table-column prop="exchangeRate" label="汇率" width="90"></el-table-column>
				<el-table-column prop="amount" label="金额" width="130">
					<template #default="scope">
						<span>{{ formatTableAmount(scope.row.amount, scope.row.foreignCurrency) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="bank" label="收汇银行" width="120"></el-table-column>
				<el-table-column prop="isCollected" label="是否领取" width="90">
					<template #default="scope">
						<el-tag v-if="scope.row.isCollected" type="success" size="small">已领取</el-tag>
						<el-tag v-else type="warning" size="small">待领取</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isConfirm" label="是否确认" width="90">
					<template #default="scope">
						<el-tag v-if="scope.row.isConfirm === 1" type="success" size="small">已确认</el-tag>
						<el-tag v-else type="warning" size="small">待确认</el-tag>
					</template>
				</el-table-column>
				<el-table-column fixed="right" prop="operate" label="操作" width="200">
					<template v-slot:default="scope">
						<el-button type="text" size="small"
							@click=CheckCustomerCollectionDetails(scope.row)>查看详情</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
				:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		</div>
		<el-dialog :modal="false" modal-penetrable v-model="addcustomercollectiondialog"
			:title="isReadOnly ? '查看收款单据' : (isEdit ? '编辑收款单据' : '新增收款单据')" :close-on-click-modal=false
			style="width: 75%;" @close="Closeaddcustomercollectiondialog()">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-form ref="addcustomercollectionformRef" :model="addcustomercollectionform" label-width="120px"
						:show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="收款单号">
									<el-input v-model="addcustomercollectionform.receiptNumber" placeholder="自动生成"
										style="width: 300px" disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="收汇日期">
									<el-date-picker v-model="addcustomercollectionform.receiptDate" type="date"
										placeholder="请选择收汇日期" style="width: 300px" :disabled="isReadOnly"
										size="default"></el-date-picker>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="我方公司">
									<el-select v-model="addcustomercollectionform.ourCompany" placeholder="请选择我方公司"
										style="width: 300px" @change="handleOurCompanyChange" :disabled="isReadOnly"
										size="default" clearable>
										<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="外销币种">
									<el-select v-model="addcustomercollectionform.foreignCurrency" placeholder="请选择外销币种"
										style="width: 300px" @change="handleForeignCurrencyChange"
										:disabled="isReadOnly" size="default" clearable>
										<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="汇率">
									<el-input v-model="addcustomercollectionform.exchangeRate" placeholder="请输入汇率"
										style="width: 300px" @input="handleExchangeRateInput"
										@blur="handleExchangeRateBlur" @change="calculateSettlementAmount"
										:disabled="true" size="default" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item :label="`收汇金额`">
									<el-input v-if="!isReadOnly" v-model="addcustomercollectionform.amount"
										placeholder="请输入金额" style="width: 300px" @input="handleAmountInput"
										@focus="handleAmountFocus" @blur="handleAmountBlur"
										@change="calculateSettlementAmount" size="default"></el-input>
									<el-input v-else :value="formattedAmount" placeholder="请输入金额" style="width: 300px"
										disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item :label="`结汇金额`">
									<el-input :value="formattedSettlementAmount" placeholder="自动计算" style="width: 300px"
										disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="收汇银行">
									<el-select v-model="addcustomercollectionform.bank" placeholder="请选择收汇银行"
										style="width: 300px" @focus="handleBankSelectFocus"
										:disabled="isReadOnly || !addcustomercollectionform.ourCompany" size="default"
										clearable>
										<el-option v-for="dict in receivingBankOptions" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row v-if="isCollectedRecord">
							<el-col :span="6">
								<el-form-item label="客户">
									<el-select v-model="addcustomercollectionform.customerID" style="width: 300px"
										:disabled="isReadOnly || !addcustomercollectionform.ourCompany" size="default"
										clearable>
										<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="领取人">
									<el-select v-model="addcustomercollectionform.receivingUser" style="width: 300px"
										:disabled="isReadOnly || !addcustomercollectionform.ourCompany" size="default"
										clearable>
										<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row v-if="isRemarkRequired">
							<el-col :span="24">
								<el-form-item label="备注" :required="isRemarkRequired">
									<el-input v-model="addcustomercollectionform.remark" placeholder="请输入备注"
										type="textarea" :rows="5" style="width: 100%" :maxlength="500" show-word-limit
										:disabled="isReadOnly" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="收款单据">
									<el-upload list-type="picture-card" :auto-upload="false"
										v-model:file-list="fileList" limit="3"
										:disabled="isReadOnly || fileList.length >= 3" @change="handleChange"
										:action="UploadUrl">
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
													<span v-if="!disabled && !isReadOnly"
														class="el-upload-list__item-delete" @click="handleRemove(file)">
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
						<el-row>
							<el-col :span="24">
								<el-form-item label="附件上传">
									<el-upload multiple :auto-upload="false" v-model:file-list="attachmentList"
										@change="handleAttachmentChange" :action="UploadUrl"
										:show-file-list="true"
										:file-list="attachmentList" class="attachment-upload">
										<el-button type="primary" :disabled="isReadOnly">
											<el-icon>
												<Upload />
											</el-icon>
											选择附件
										</el-button>
										<template #tip>
											<div class="el-upload__tip">
												支持上传文档、图片等文件，附件总大小不超过 50M
											</div>
										</template>
									</el-upload>
									<!-- 附件列表显示 -->
									<div v-if="attachmentList.length > 0" class="attachment-list">
										<div v-for="(file, index) in attachmentList" :key="file.uid || index"
											class="attachment-item">
											<div class="attachment-info">
												<el-icon class="attachment-icon">
													<Document />
												</el-icon>
												<span class="attachment-name">{{ file.name }}</span>
												<span class="attachment-size">{{ formatFileSize(file.size) }}</span>
											</div>
											<div class="attachment-actions">
												<el-button type="text" size="small" @click="previewAttachment(file)">
													<el-icon>
														<View />
													</el-icon>
													预览
												</el-button>
												<el-button type="text" size="small" @click="downloadAttachment(file)">
													<el-icon>
														<Download />
													</el-icon>
													下载
												</el-button>
												<el-button v-if="!isReadOnly" type="text" size="small"
													@click="removeAttachment(file, index)" class="delete-btn">
													<el-icon>
														<Delete />
													</el-icon>
													删除
												</el-button>
											</div>
										</div>
									</div>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-if="showReceivingPaymentsDetails" v-model="receivingDetailsCollapseActive"
				style="margin-bottom: 20px;">
				<el-collapse-item title="收款明细" name="receivingDetails">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">收款明细</span>
					</template>
					<el-table :data="ReceivingPaymentsDetailsTbaleData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="fundsClassification" label="款项类别">
							<template #default="{ row }">
								<span>{{ getFundsClassificationLabel(row.fundsClassification) }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="associatedModulesDocument" label="关联单号">
							<template #default="{ row }">
								<span>{{ getAssociatedDocumentLabel(row.associatedModulesDocument) }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="applicationAmount" label="金额">
							<template #default="{ row }">
								<span>{{ formatAmountWithCurrency(row.applicationAmount,
									addcustomercollectionform.foreignCurrency) }}</span>
							</template>
						</el-table-column>
					</el-table>
					<!-- 合计行 -->
					<div v-if="ReceivingPaymentsDetailsTbaleData.length > 0"
						style="margin-top: 10px; text-align: right; padding: 10px; background-color: #f5f7fa; border-radius: 4px;">
						<div style="display: flex; justify-content: space-between; align-items: center;">
							<div>
								<span style="font-weight: bold; font-size: 14px;">合计金额：</span>
								<span :style="{
									fontWeight: 'bold',
									fontSize: '16px',
									color: isAmountExceeded ? '#f56c6c' : isAmountEqual ? '#67c23a' : '#409eff'
								}">
									{{ formatAmountWithCurrency(totalAmount, addcustomercollectionform.foreignCurrency)
									}}
								</span>
								<div v-if="isAmountExceeded" style="margin-top: 5px;">
									<el-tag type="danger" size="small">
										<el-icon>
											<Warning />
										</el-icon>
										收款明细总金额已超过收汇金额
									</el-tag>
								</div>
								<div v-else-if="!isAmountEqual && totalAmount > 0" style="margin-top: 5px;">
									<el-tag type="warning" size="small">
										<el-icon>
											<Warning />
										</el-icon>
										收款明细总金额必须等于收汇金额
									</el-tag>
								</div>
								<div v-else-if="isAmountEqual" style="margin-top: 5px;">
									<el-tag type="success" size="small">
										<el-icon>
											<CircleCheck />
										</el-icon>
										金额匹配
									</el-tag>
								</div>
							</div>
							<div style="text-align: right;">
								<div style="font-size: 12px; color: #909399;">收汇金额：</div>
								<div style="font-size: 14px; color: #606266;">
									{{ formatAmountWithCurrency(addcustomercollectionform.amount,
										addcustomercollectionform.foreignCurrency)
									}}
								</div>
							</div>
						</div>
					</div>
				</el-collapse-item>
			</el-collapse>
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
					<el-button type="primary" v-if="isConfirmBtnShow" @click="ConfirmCustomerCollection()">
						确认
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref, computed, watch, nextTick } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import { useRoute } from 'vue-router'
import request from '@/utils/request';
import { getDicts } from '@/api/system/dict/data';
import { get } from 'sortablejs';
import exchangeRateService from '@/utils/exchangeRateService';

// 添加格式化日期函数
const formatDate = (dateString) => {
	if (!dateString) return '';
	const date = new Date(dateString);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	return `${year}-${month}-${day}`;
};

// 处理汇率输入格式化（保留3位小数）
const handleExchangeRateInput = (value) => {
	if (!value) return;

	// 移除非数字和小数点
	let cleanValue = value.replace(/[^\d.]/g, '');

	// 确保只有一个小数点
	const parts = cleanValue.split('.');
	if (parts.length > 2) {
		cleanValue = parts[0] + '.' + parts.slice(1).join('');
	}

	// 限制小数点后最多3位
	if (parts.length === 2 && parts[1].length > 3) {
		cleanValue = parts[0] + '.' + parts[1].substring(0, 3);
	}

	// 更新表单值
	addcustomercollectionform.value.exchangeRate = cleanValue;
};

// 处理汇率失焦格式化（补0到3位小数）
const handleExchangeRateBlur = (event) => {
	const value = event.target.value;
	if (!value) return;

	// 如果包含小数点，处理小数部分
	const parts = value.split('.');
	if (parts.length === 2) {
		const integerPart = parts[0];
		const decimalPart = parts[1];

		// 补0到3位小数
		const paddedDecimal = decimalPart.padEnd(3, '0');

		// 更新表单值
		addcustomercollectionform.value.exchangeRate = integerPart + '.' + paddedDecimal;
	} else if (parts.length === 1) {
		// 如果只有整数部分，添加.000
		const integerPart = parts[0];
		addcustomercollectionform.value.exchangeRate = integerPart + '.000';
	}
};

// 处理收汇金额输入格式化（保留2位小数）
const handleAmountInput = (value) => {
	if (!value) return;

	// 移除非数字和小数点
	let cleanValue = value.replace(/[^\d.]/g, '');

	// 确保只有一个小数点
	const parts = cleanValue.split('.');
	if (parts.length > 2) {
		cleanValue = parts[0] + '.' + parts.slice(1).join('');
	}

	// 限制小数点后最多2位
	if (parts.length === 2 && parts[1].length > 2) {
		cleanValue = parts[0] + '.' + parts[1].substring(0, 2);
	}

	// 更新表单值
	addcustomercollectionform.value.amount = cleanValue;
};

// 处理收汇金额获得焦点（清除货币符号，只显示数字）
const handleAmountFocus = (event) => {
	const value = event.target.value;
	if (!value) return;

	// 移除货币符号和空格，只保留数字和小数点
	const cleanValue = value.replace(/[^\d.]/g, '');
	event.target.value = cleanValue;
};

// 处理收汇金额失焦格式化（补0到2位小数）
const handleAmountBlur = (event) => {
	const value = event.target.value;
	if (!value) return;

	// 移除货币符号和空格，获取纯数字
	let cleanValue = value.replace(/[^\d.]/g, '');

	// 如果包含小数点，处理小数部分
	const parts = cleanValue.split('.');
	if (parts.length === 2) {
		const integerPart = parts[0];
		const decimalPart = parts[1];

		// 补0到2位小数
		const paddedDecimal = decimalPart.padEnd(2, '0');

		// 更新表单值
		addcustomercollectionform.value.amount = integerPart + '.' + paddedDecimal;
	} else if (parts.length === 1) {
		// 如果只有整数部分，添加.00
		const integerPart = parts[0];
		addcustomercollectionform.value.amount = integerPart + '.00';
	}

	// 重新计算结汇金额
	calculateSettlementAmount();

	// 在失去焦点时显示货币符号
	const currencySymbol = getCurrencySymbol(addcustomercollectionform.value.foreignCurrency);
	const formattedValue = currencySymbol + ' ' + addcustomercollectionform.value.amount;
	// 使用 nextTick 确保 DOM 更新后再设置显示值
	nextTick(() => {
		event.target.value = formattedValue;
	});
};

// 处理结汇金额输入格式化（保留2位小数）
const handleSettlementAmountInput = (value) => {
	if (!value) return;

	// 移除非数字和小数点
	let cleanValue = value.replace(/[^\d.]/g, '');

	// 确保只有一个小数点
	const parts = cleanValue.split('.');
	if (parts.length > 2) {
		cleanValue = parts[0] + '.' + parts.slice(1).join('');
	}

	// 限制小数点后最多2位
	if (parts.length === 2 && parts[1].length > 2) {
		cleanValue = parts[0] + '.' + parts[1].substring(0, 2);
	}

	// 更新表单值
	addcustomercollectionform.value.ExchangeSettlementAmount = cleanValue;
};

// 计算结汇金额（汇率 × 收汇金额）
const calculateSettlementAmount = () => {
	const exchangeRate = parseFloat(addcustomercollectionform.value.exchangeRate) || 0;
	const amount = parseFloat(addcustomercollectionform.value.amount) || 0;

	const settlementAmount = exchangeRate * amount;

	// 格式化为2位小数
	addcustomercollectionform.value.ExchangeSettlementAmount = settlementAmount.toFixed(2);
};

// 获取货币符号
const getCurrencySymbol = (currencyValue) => {
	if (!currencyValue) return '';
	if (currencyValue === '1') return 'USD';
	if (currencyValue === '2') return 'EUR';
	if (currencyValue === '3') return 'CNY';
	return '';
};

// 格式化金额显示（添加货币符号）
const formatAmountDisplay = (amount, currencyValue) => {
	if (!amount) return '';
	const symbol = getCurrencySymbol(currencyValue);
	return symbol ? `${symbol} ${amount}` : amount;
};

// 格式化表格中的金额显示
const formatTableAmount = (amount, currencyLabel) => {
	if (!amount) return '';
	// 确保金额格式化为2位小数
	const formattedAmount = parseFloat(amount).toFixed(2);
	// 根据货币标签获取货币符号
	let symbol = '';
	if (currencyLabel === '美元') symbol = 'USD';
	else if (currencyLabel === '欧元') symbol = 'EUR';
	else if (currencyLabel === '人民币') symbol = 'CNY';

	return symbol ? `${symbol} ${formattedAmount}` : formattedAmount;
};



// 计算属性：格式化结汇金额显示
const formattedSettlementAmount = computed(() => {
	const settlementAmount = addcustomercollectionform.value.ExchangeSettlementAmount;
	if (!settlementAmount) return '';
	// 确保结汇金额格式化为2位小数
	const formattedSettlementAmount = parseFloat(settlementAmount).toFixed(2);
	return formatAmountDisplay(formattedSettlementAmount, '3'); // 结汇金额始终使用CNY
});

// 计算属性：格式化收汇金额显示
const formattedAmount = computed(() => {
	const amount = addcustomercollectionform.value.amount;
	if (!amount) return '';
	// 确保金额格式化为2位小数
	const formattedAmount = parseFloat(amount).toFixed(2);
	return formatAmountDisplay(formattedAmount, addcustomercollectionform.value.foreignCurrency);
});

// 计算属性：收汇金额显示值（只读时显示格式化，编辑时显示原始值）
const displayAmount = computed(() => {
	if (isReadOnly.value) {
		return formattedAmount.value;
	}
	return addcustomercollectionform.value.amount;
});

// 处理外销币种变化事件
const handleForeignCurrencyChange = async (value) => {
	// 如果选择的是人民币（字典值为3），自动设置汇率为1.000
	if (value === '3') {
		addcustomercollectionform.value.exchangeRate = '1.000';
	} else {
		// 获取最新汇率
		try {
			const latestRate = await exchangeRateService.getLatestExchangeRate(value);
			if (latestRate !== null) {
				addcustomercollectionform.value.exchangeRate = exchangeRateService.formatExchangeRate(latestRate);
			} else {
				// 如果获取不到最新汇率，使用默认汇率
				const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
				addcustomercollectionform.value.exchangeRate = exchangeRateService.formatExchangeRate(defaultRate);
				ElMessage.warning(`未找到${exchangeRateService.getCurrencyName(value, optionss.hr_export_currency)}的最新汇率，已使用默认汇率`);
			}
		} catch (error) {
			console.error('获取汇率失败:', error);
			// 使用默认汇率
			const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
			addcustomercollectionform.value.exchangeRate = exchangeRateService.formatExchangeRate(defaultRate);
			ElMessage.warning(`获取汇率失败，已使用默认汇率`);
		}
	}

	// 重新计算结汇金额
	calculateSettlementAmount();
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
		isReadOnly.value = false; // 新增时设置为可编辑
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
const isReadOnly = ref(false); // 控制表单是否只读
const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);
const isSubmitBtnShow = ref(false);
const isConfirmBtnShow = ref(false);
const route = useRoute()

// 折叠面板控制变量
const basicInfoCollapseActive = ref(['basicInfo']);
const receivingDetailsCollapseActive = ref(['receivingDetails']);

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
		sql_shippingdeliveries: [],
		hr_receiving_bank: [],
		hr_rf_receiving_bank: [],
		sql_all_user: []
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
	{ dictType: 'sql_shippingdeliveries' },
	{ dictType: 'hr_receiving_bank' },
	{ dictType: 'hr_rf_receiving_bank' },
	{ dictType: 'sql_all_user' }
]

const mapReceiptDetailToRow = (details) => {
	const companyLabel = state.optionss.hr_ourcompany.find((dict) => dict.dictValue === details.ourCompany)?.dictLabel || details.ourCompany;
	const foreignCurrencyLabel = state.optionss.hr_export_currency.find((dict) => dict.dictValue === details.foreignCurrency)?.dictLabel || details.foreignCurrency;

	let bankOptions = [];
	if (companyLabel === '荣发塑料') {
		bankOptions = state.optionss.hr_rf_receiving_bank || [];
	} else if (companyLabel === '惠荣进出口') {
		bankOptions = state.optionss.hr_receiving_bank || [];
	}
	const bankLabel = bankOptions.find((dict) => dict.dictValue === details.bank)?.dictLabel || details.bank;

	const customerID = details.customerID ?? details.customer ?? details.CustomerID ?? details.Customer;
	const receivingUser = details.receivingUser ?? details.receiving_user ?? details.receivingUserId ?? details.receivinguserid;

	return {
		id: details.id ?? details.ID ?? details.customerCollectionId ?? details.customerCollectionID ?? details.collectionId,
		receiptNumber: details.receiptNumber,
		receiptDate: details.receiptDate,
		ourCompany: companyLabel,
		foreignCurrency: foreignCurrencyLabel,
		exchangeRate: details.exchangeRate,
		amount: details.amount,
		exchangeSettlementAmount: details.exchangeSettlementAmount ?? details.ExchangeSettlementAmount,
		bank: bankLabel,
		customerID,
		receivingUser,
		customer: details.customer ?? details.Customer,
		fundsClassification: details.fundsClassification,
		associatedModules: details.associatedModules,
		associatedModulesDocumentID: details.associatedModulesDocumentID,
		remark: details.remark,
		isConfirm: details.isConfirm ?? 0,
		isDraft: details.isDraft ?? 0,
		isCollected: !!(customerID && receivingUser),
		receiptImageUrl: details.receiptImageUrl,
		attachmentUrl: details.attachmentUrl
	};
};

const openReceiptFromRoute = async () => {
	const receiptId = route.query.receiptId;
	const receiptNumber = route.query.receiptNumber;
	if (!receiptId && !receiptNumber) return;

	await nextTick();
	let targetRow = null;
	if (receiptId) {
		targetRow = customercollectiontableData.value.find((item) => String(item.id) === String(receiptId));
	}
	if (!targetRow && receiptNumber) {
		targetRow = customercollectiontableData.value.find((item) => item.receiptNumber === receiptNumber);
	}
	if (!targetRow && receiptId) {
		try {
			const response = await request({
				url: 'CustomerCollections/GetCustomerCollectionDetails/GetDetails',
				method: 'GET',
				params: {
					id: receiptId
				}
			});
			if (response?.code === 200 && response.data) {
				targetRow = mapReceiptDetailToRow(response.data);
			}
		} catch (error) {
			console.error('获取收款单详情失败:', error);
		}
	}

	if (targetRow) {
		await CheckCustomerCollectionDetails(targetRow);
	} else {
		ElMessage.warning('未找到对应收款单');
	}
};

async function fetchDataAndExecute() {
	try {
		const response = await getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		/*获取当前页面列表函数放在下方*/
		await GetCustomerCollectionsList(currentPage.value, pageSize.value);
		await openReceiptFromRoute();
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

// 计算属性：根据我方公司动态获取收汇银行选项
const receivingBankOptions = computed(() => {
	const selectedCompany = addcustomercollectionform.value.ourCompany;
	// 如果我方公司为空或未选择，返回空数组
	if (!selectedCompany || selectedCompany === '') {
		return [];
	}
	// 根据我方公司的值返回对应的收汇银行数据源
	if (selectedCompany === '1') {
		// 荣发塑料，使用 hr_rf_receiving_bank
		return optionss.value.hr_rf_receiving_bank || [];
	} else if (selectedCompany === '2') {
		// 惠荣进出口，使用 hr_receiving_bank
		return optionss.value.hr_receiving_bank || [];
	}
	return [];
});

// 监听我方公司变化，清空收汇银行选择
watch(() => addcustomercollectionform.value.ourCompany, (newValue, oldValue) => {
	console.log('我方公司变化监听器触发:', { newValue, oldValue });

	// 当我方公司发生变化时，清空收汇银行和备注
	if (newValue !== oldValue) {
		// 使用 nextTick 确保在 DOM 更新后执行清空操作
		nextTick(() => {
			addcustomercollectionform.value.bank = '';
			addcustomercollectionform.value.remark = '';
			addcustomercollectionform.value.ExchangeSettlementAmount = '';
			console.log('我方公司已更改，已清空收汇银行、备注和结汇金额');
		});
	}

	// 特别处理当我方公司被清空的情况
	if (!newValue || newValue === '') {
		nextTick(() => {
			addcustomercollectionform.value.bank = '';
			addcustomercollectionform.value.remark = '';
			addcustomercollectionform.value.ExchangeSettlementAmount = '';
			console.log('我方公司已清空，已清空收汇银行、备注和结汇金额');
		});
	}
}, { immediate: true, deep: true });

// 监听收汇银行变化，当从"其他"改为其他选项时清空备注
watch(() => addcustomercollectionform.value.bank, (newValue, oldValue) => {
	// 如果之前选择的是"其他"（值为'5'），现在改为其他选项，则清空备注
	if (oldValue === '5' && newValue !== '5') {
		addcustomercollectionform.value.remark = '';
		console.log('收汇银行已从"其他"改为其他选项，已清空备注');
	}
});

// 去除货币符号的辅助函数
const removeCurrencySymbol = (value) => {
	if (!value) return '';
	// 移除货币符号（USD、EUR、CNY）和空格，只保留数字和小数点
	return value.replace(/[^\d.]/g, '');
};

// 处理我方公司变化事件
const handleOurCompanyChange = (value) => {
	console.log('我方公司变化事件触发:', value);
	// 直接清空收汇银行、备注和结汇金额
	addcustomercollectionform.value.bank = '';
	addcustomercollectionform.value.remark = '';
	addcustomercollectionform.value.ExchangeSettlementAmount = '';
	console.log('已清空收汇银行、备注和结汇金额');
};

// 处理收汇银行下拉框聚焦事件
const handleBankSelectFocus = () => {
	// 检查是否已选择我方公司
	if (!addcustomercollectionform.value.ourCompany || addcustomercollectionform.value.ourCompany === '') {
		ElMessage({
			type: 'warning',
			message: '请先选择我方公司'
		});
	}
};

// 计算属性：搜索条件中的收汇银行选项（包含所有可能的收汇银行）
const searchBankOptions = computed(() => {
	const allBanks = [];

	// 添加荣发塑料的收汇银行
	if (optionss.value.hr_rf_receiving_bank) {
		allBanks.push(...optionss.value.hr_rf_receiving_bank);
	}

	// 添加惠荣进出口的收汇银行
	if (optionss.value.hr_receiving_bank) {
		allBanks.push(...optionss.value.hr_receiving_bank);
	}

	return allBanks;
});

// 计算属性：判断是否显示备注输入框（当收汇银行选择"其他"时显示）
const isRemarkRequired = computed(() => {
	const selectedBank = addcustomercollectionform.value.bank;
	// 检查收汇银行是否选择的是"其他"
	// 这里假设"其他"的字典值为"999"或其他特定值，您需要根据实际情况调整
	return selectedBank === '5';
});

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'	// 上传图片地址
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)

// 附件上传相关变量
const attachmentList = ref([]);  // 附件列表
const attachmentUrlStr = ref('');  // 附件URL字符串
const uploadedAttachments = ref([]);  // 已上传的附件

const IMAGE_MAX_SIZE = 1 * 1024 * 1024; // 每张照片最大 1M
// 检查上传客户图片数量
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

const ATTACHMENT_TOTAL_MAX = 50 * 1024 * 1024; // 附件总量不超过 50M
const getAttachmentTotalSize = (list) => (list || []).reduce((sum, f) => sum + (f.raw ? f.raw.size : 0), 0);

// 附件变化处理（数量不限，总量不超过 50M）
const handleAttachmentChange = (file, fileList) => {
	const totalSize = getAttachmentTotalSize(fileList);
	if (totalSize > ATTACHMENT_TOTAL_MAX) {
		ElMessage.error('附件总大小不能超过 50M');
		attachmentList.value = fileList.filter(f => f.uid !== file.uid);
		return;
	}
	// 检查重复文件
	const duplicate = uploadedAttachments.value.findIndex(fileItem => fileItem.name === file.name);
	if (duplicate !== -1) {
		ElMessage.warning('请不要上传重复的文件!');
		const duplicatesInFileList = fileList.filter(fileItem => fileItem.name === file.name);
		if (duplicatesInFileList.length > 1) {
			for (let i = 1; i < duplicatesInFileList.length; i++) {
				const index = fileList.findIndex(fileItem => fileItem.uid === duplicatesInFileList[i].uid);
				if (index !== -1) {
					fileList.splice(index, 1);
				}
			}
		}
	} else {
		// 添加文件到已上传列表
		const newFiles = fileList.filter(file => !uploadedAttachments.value.some(fileItem => fileItem.name === file.name));
		newFiles.forEach(file => {
			if (!file.isChanged) {
				file.isChanged = true;
				uploadedAttachments.value.push(file);
			}
		});
	}
};

// 格式化文件大小
const formatFileSize = (size) => {
	if (!size) return '';
	const units = ['B', 'KB', 'MB', 'GB'];
	let unitIndex = 0;
	let fileSize = size;

	while (fileSize >= 1024 && unitIndex < units.length - 1) {
		fileSize /= 1024;
		unitIndex++;
	}

	return `${fileSize.toFixed(1)} ${units[unitIndex]}`;
};

// 预览附件
const previewAttachment = (file) => {
	if (file.url) {
		window.open(file.url, '_blank');
	} else {
		ElMessage.warning('文件还未上传，无法预览');
	}
};

// 下载附件
const downloadAttachment = (file) => {
	if (file.url) {
		const link = document.createElement('a');
		link.href = file.url;
		link.download = file.name;
		link.target = '_blank';
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
	} else {
		ElMessage.warning('文件还未上传，无法下载');
	}
};

// 删除附件
const removeAttachment = (file, index) => {
	ElMessageBox.confirm('确定删除该附件吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		// 从附件列表中移除
		attachmentList.value.splice(index, 1);

		// 从已上传列表中移除
		const uploadedIndex = uploadedAttachments.value.findIndex(f => f.uid === file.uid);
		if (uploadedIndex !== -1) {
			uploadedAttachments.value.splice(uploadedIndex, 1);
		}

		// 从URL字符串中移除
		if (attachmentUrlStr.value) {
			const urls = attachmentUrlStr.value.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				attachmentUrlStr.value = urls.join(',');
			}
		}

		ElMessage.success('删除成功!');
	}).catch(() => {
		ElMessage.info('已取消删除');
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
	AttachmentUrl: "",
	Remark: "",
	IsDelete: 0,
	isDraft: 1,
	ExchangeSettlementAmount: 0,
	isConfirm: 0,
	CustomerID: 0,
	ReceivingUser: 0
})

//保存收款单据
const SaveCustomerCollection = async () => {
	try {
		// 验证备注字段（当收汇银行选择"其他"时必填）
		if (isRemarkRequired.value && !addcustomercollectionform.value.remark.trim()) {
			ElMessage({
				type: 'error',
				message: '当收汇银行选择"其他"时，备注字段为必填项'
			});
			return;
		}

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
		// 去除收汇金额的货币符号后转换为数字
		const cleanAmount = removeCurrencySymbol(addcustomercollectionform.value.amount);
		customerCollectionsRequest.Amount = Number(cleanAmount);
		customerCollectionsRequest.Bank = addcustomercollectionform.value.bank;
		customerCollectionsRequest.Remark = addcustomercollectionform.value.remark;
		// 去除结汇金额的货币符号后转换为数字
		const cleanSettlementAmount = removeCurrencySymbol(addcustomercollectionform.value.ExchangeSettlementAmount);
		customerCollectionsRequest.ExchangeSettlementAmount = Number(cleanSettlementAmount);
		customerCollectionsRequest.isConfirm = addcustomercollectionform.value.isConfirm || 0;
		customerCollectionsRequest.CustomerID = Number(addcustomercollectionform.value.customerID) || 0;
		customerCollectionsRequest.ReceivingUser = Number(addcustomercollectionform.value.receivingUser) || 0;
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

		// 上传附件
		let attachmentUrls = [];
		if (Array.isArray(attachmentList.value) && attachmentList.value.length > 0) {
			attachmentUrls = await Promise.all(attachmentList.value
				.filter(file => file.isChanged)
				.map(async (file) => {
					const response = await uploadAttachment(file);
					if (response.code === 200 && response.data.url) {
						return response.data.url;
					}
					throw new Error('上传附件失败');
				}));
		}
		// 合并现有的和新上传的附件URL
		if (attachmentUrlStr.value) {
			const existingAttachmentUrls = attachmentUrlStr.value.split(',');
			attachmentUrls = [...existingAttachmentUrls, ...attachmentUrls];
		}
		customerCollectionsRequest.AttachmentUrl = attachmentUrls.join(',');
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

// 辅助函数：上传附件
const uploadAttachment = async (file) => {
	const formData = new FormData();
	formData.append('FileName', file.name || '未命名文件');
	formData.append('FileDir', 'CustomerCollection/Attachments');
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
	addcustomercollectionform.value.ExchangeSettlementAmount = ''
	addcustomercollectionform.value.remark = ''
	addcustomercollectionform.value.attachment = ''
	addcustomercollectionform.value.Customer = ''
	addcustomercollectionform.value.FundsClassification = ''
	addcustomercollectionform.value.AssociatedModules = ''
	addcustomercollectionform.value.AssociatedModulesDocumentID = ''
	fileList.value = []
	filelistUrlStr.value = ''
	// 清空附件相关数据
	attachmentList.value = []
	attachmentUrlStr.value = ''
	uploadedAttachments.value = []
	isEdit.value = false
	isReadOnly.value = false
	EditID.value = 0
	// 清空收款明细
	ReceivingPaymentsDetailsTbaleData.value = []
	totalAmount.value = 0
	// 清空客户ID和领取人
	addcustomercollectionform.value.customerID = ''
	addcustomercollectionform.value.receivingUser = ''
	// 清空isConfirm
	addcustomercollectionform.value.isConfirm = 0
	isConfirmBtnShow.value = false
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

					// 根据我方公司选择对应的收汇银行数据源
					let bankOptions = [];
					if (item.ourCompany === '荣发塑料') {
						bankOptions = state.optionss.hr_rf_receiving_bank || [];
					} else if (item.ourCompany === '惠荣进出口') {
						bankOptions = state.optionss.hr_receiving_bank || [];
					}

					// 在对应的收汇银行数据源中查找标签
					const bankDict = bankOptions.find((dict) => dict.dictValue === item.bank);
					item.bank = bankDict?.dictLabel || item.bank;

					// 转换客户ID为显示标签
					item.customerID = state.optionss.sql_hr_customer.find((dict) => dict.dictValue === item.customerID)?.dictLabel || item.customerID;
					// 计算是否领取状态：有领取人和客户则为已领取，否则为待领取
					item.isCollected = !!(item.customerID && item.receivingUser);
				})
				resolve(response.data);
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
const CheckCustomerCollectionDetails = async (row) => {
	EditID.value = row.id;
	if (row.isDraft === 1) {
		// 草稿状态：可以编辑和提交
		isSubmitBtnShow.value = true;
		isEdit.value = true;
		isSaveBtnShow.value = false;
		isEditSaveBtnShow.value = true;
		isReadOnly.value = false;
		isConfirmBtnShow.value = false;
	} else {
		// 已提交状态：只读模式
		isSubmitBtnShow.value = false;
		isEdit.value = false;
		isSaveBtnShow.value = false;
		isEditSaveBtnShow.value = false;
		isReadOnly.value = true;
		// 只有在已领取（isCollected为true）且待确认（isConfirm为0）的情况下，才显示确认按钮
		isConfirmBtnShow.value = row.isCollected && row.isConfirm === 0;
	}
	// 基本信息赋值
	addcustomercollectionform.value.receiptNumber = row.receiptNumber;
	addcustomercollectionform.value.receiptDate = row.receiptDate;
	addcustomercollectionform.value.ourCompany = state.optionss.hr_ourcompany.find((dict) => dict.dictLabel === row.ourCompany)?.dictValue || '';
	addcustomercollectionform.value.foreignCurrency = state.optionss.hr_export_currency.find((dict) => dict.dictLabel === row.foreignCurrency)?.dictValue || '';
	addcustomercollectionform.value.exchangeRate = row.exchangeRate ? parseFloat(row.exchangeRate).toFixed(3) : '';
	// 收汇金额赋值，存储纯数字，显示时通过计算属性格式化
	addcustomercollectionform.value.amount = row.amount ? parseFloat(row.amount).toFixed(2) : '';
	addcustomercollectionform.value.ExchangeSettlementAmount = row.exchangeSettlementAmount ? parseFloat(row.exchangeSettlementAmount).toFixed(2) : '';
	// 根据我方公司选择对应的收汇银行数据源，然后查找对应的值
	let bankOptions = [];
	if (row.ourCompany === '荣发塑料') {
		bankOptions = state.optionss.hr_rf_receiving_bank || [];
	} else if (row.ourCompany === '惠荣进出口') {
		bankOptions = state.optionss.hr_receiving_bank || [];
	}

	const bankDict = bankOptions.find((dict) => dict.dictLabel === row.bank);
	addcustomercollectionform.value.bank = bankDict?.dictValue || '';
	addcustomercollectionform.value.customerID = state.optionss.sql_hr_customer.find((dict) => dict.dictValue === row.customerID.toString())?.dictValue || '';
	addcustomercollectionform.value.receivingUser = state.optionss.sql_all_user.find((dict) => dict.dictValue === row.receivingUser.toString())?.dictValue || '';
	// 备注字段赋值
	addcustomercollectionform.value.remark = row.remark || '';
	// isConfirm字段赋值
	addcustomercollectionform.value.isConfirm = row.isConfirm || 0;

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

	// 清空现有附件列表
	attachmentList.value = [];
	attachmentUrlStr.value = '';

	// 处理附件显示，添加完整的 URL 路径
	if (row.attachmentUrl) {
		const attachmentUrls = row.attachmentUrl.split(',');
		attachmentList.value = attachmentUrls.map((url, index) => {
			const fileName = url.split('/').pop() || `Attachment ${index + 1}`;
			return {
				name: fileName,
				url: url,
				isChanged: false,
				uid: Date.now() + index + 1000
			};
		});

		attachmentUrlStr.value = row.attachmentUrl;
	}

	// 更新已上传附件列表
	uploadedAttachments.value = [...attachmentList.value];

	// 如果有客户ID和领取人，获取收款明细
	if (addcustomercollectionform.value.customerID && addcustomercollectionform.value.receivingUser) {
		await getCustomerCollectionsDetailsList(row.id);
	}

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
	remark: '',
	attachment: '',
	Customer: '',
	FundsClassification: '',
	AssociatedModules: '',
	AssociatedModulesDocumentID: '',
	ExchangeSettlementAmount: '',
	customerID: '',
	receivingUser: '',
	isConfirm: 0
})

// 保存收款单据（用于提交时的保存）
const SaveCustomerCollectionForSubmit = async () => {
	try {
		// 准备基础数据
		customerCollectionsRequest.ReceiptNumber = addcustomercollectionform.value.receiptNumber;
		customerCollectionsRequest.ReceiptDate = addcustomercollectionform.value.receiptDate;
		customerCollectionsRequest.OurCompany = addcustomercollectionform.value.ourCompany;
		customerCollectionsRequest.ForeignCurrency = addcustomercollectionform.value.foreignCurrency;
		customerCollectionsRequest.ExchangeRate = Number(addcustomercollectionform.value.exchangeRate);
		// 去除收汇金额的货币符号后转换为数字
		const cleanAmount = removeCurrencySymbol(addcustomercollectionform.value.amount);
		customerCollectionsRequest.Amount = Number(cleanAmount);
		customerCollectionsRequest.Bank = addcustomercollectionform.value.bank;
		customerCollectionsRequest.Remark = addcustomercollectionform.value.remark;
		// 去除结汇金额的货币符号后转换为数字
		const cleanSettlementAmount = removeCurrencySymbol(addcustomercollectionform.value.ExchangeSettlementAmount);
		customerCollectionsRequest.ExchangeSettlementAmount = Number(cleanSettlementAmount);
		customerCollectionsRequest.isConfirm = addcustomercollectionform.value.isConfirm || 0;
		customerCollectionsRequest.CustomerID = Number(addcustomercollectionform.value.customerID) || 0;
		customerCollectionsRequest.ReceivingUser = Number(addcustomercollectionform.value.receivingUser) || 0;

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

		// 上传附件
		let attachmentUrls = [];
		if (Array.isArray(attachmentList.value) && attachmentList.value.length > 0) {
			attachmentUrls = await Promise.all(attachmentList.value
				.filter(file => file.isChanged)
				.map(async (file) => {
					const response = await uploadAttachment(file);
					if (response.code === 200 && response.data.url) {
						return response.data.url;
					}
					throw new Error('上传附件失败');
				}));
		}
		// 合并现有的和新上传的附件URL
		if (attachmentUrlStr.value) {
			const existingAttachmentUrls = attachmentUrlStr.value.split(',');
			attachmentUrls = [...existingAttachmentUrls, ...attachmentUrls];
		}
		customerCollectionsRequest.AttachmentUrl = attachmentUrls.join(',');

		// 保存数据
		const response = await request.post(
			!isEdit.value
				? 'CustomerCollections/AddCustomerCollections/Add'
				: 'CustomerCollections/EditCustomerCollections/Edit',
			isEdit.value ? { ...customerCollectionsRequest, Id: EditID.value } : customerCollectionsRequest
		);

		if (response != null) {
			// 返回保存的ID，不关闭对话框
			return response.data || EditID.value;
		}
	} catch (error) {
		console.error('保存收款单据出错！😔错误内容：', error);
		throw error;
	}
};

const SubmitCustomerCollection = async () => {
	try {
		// 验证备注字段（当收汇银行选择"其他"时必填）
		if (isRemarkRequired.value && !addcustomercollectionform.value.remark.trim()) {
			ElMessage({
				type: 'error',
				message: '当收汇银行选择"其他"时，备注字段为必填项'
			});
			return;
		}

		await ElMessageBox.confirm('确定提交该收款单据吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		});

		let currentId = EditID.value;

		if (EditID.value === 0) {
			// 新增时先保存草稿，获取ID
			currentId = await SaveCustomerCollectionForSubmit();
		}

		// 更新草稿状态为已提交
		const response = await request({
			url: 'CustomerCollections/UpdateCustomerCollectionsIsDraft/UpdateIsDraft',
			method: 'get',
			params: {
				ID: currentId
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

// 确认收款单据
const ConfirmCustomerCollection = async () => {
	try {
		await ElMessageBox.confirm('确定确认该收款单据吗？', '提示', {
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
		// 去除收汇金额的货币符号后转换为数字
		const cleanAmount = removeCurrencySymbol(addcustomercollectionform.value.amount);
		customerCollectionsRequest.Amount = Number(cleanAmount);
		customerCollectionsRequest.Bank = addcustomercollectionform.value.bank;
		customerCollectionsRequest.Remark = addcustomercollectionform.value.remark;
		// 去除结汇金额的货币符号后转换为数字
		const cleanSettlementAmount = removeCurrencySymbol(addcustomercollectionform.value.ExchangeSettlementAmount);
		customerCollectionsRequest.ExchangeSettlementAmount = Number(cleanSettlementAmount);
		// 设置isConfirm为1（已确认）
		customerCollectionsRequest.isConfirm = 1;
		// 设置CustomerID和ReceivingUser
		customerCollectionsRequest.CustomerID = Number(addcustomercollectionform.value.customerID) || 0;
		customerCollectionsRequest.ReceivingUser = Number(addcustomercollectionform.value.receivingUser) || 0;

		// 处理图片URL（确认时不需要重新上传，使用现有URL）
		if (filelistUrlStr.value) {
			customerCollectionsRequest.ReceiptImageUrl = filelistUrlStr.value;
		} else {
			customerCollectionsRequest.ReceiptImageUrl = '';
		}

		// 处理附件URL（确认时不需要重新上传，使用现有URL）
		if (attachmentUrlStr.value) {
			customerCollectionsRequest.AttachmentUrl = attachmentUrlStr.value;
		} else {
			customerCollectionsRequest.AttachmentUrl = '';
		}

		// 调用edit接口更新数据
		const response = await request.post(
			'CustomerCollections/EditCustomerCollections/Edit',
			{ ...customerCollectionsRequest, Id: EditID.value }
		);

		if (response != null) {
			ElMessage({
				message: response.msg || '确认成功',
				type: 'success'
			});
			Closeaddcustomercollectiondialog();
			GetCustomerCollectionsList(currentPage.value, pageSize.value);
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('确认收款单据出错：', error);
			ElMessage.error('确认收款单据失败');
		}
	}
}

const ReceivingPaymentsDetailsTbaleData = ref([]);
// 格式化带货币符号的金额显示
const formatAmountWithCurrency = (amount, currencyValue) => {
	if (!amount) return '';
	const symbol = getCurrencySymbol(currencyValue);
	const formattedAmount = new Intl.NumberFormat('zh-CN', {
		minimumFractionDigits: 2,
		maximumFractionDigits: 2
	}).format(amount);
	return symbol ? `${symbol} ${formattedAmount}` : formattedAmount;
};


const totalAmount = ref(0);
// 检查是否超过收汇金额
const isAmountExceeded = computed(() => {
	const receiptAmount = parseFloat(addcustomercollectionform.value.amount) || 0;
	// 使用更精确的比较方式，避免浮点数精度问题
	return Math.round(totalAmount.value * 100) / 100 > Math.round(receiptAmount * 100) / 100;
});

// 检查金额是否完全相等
const isAmountEqual = computed(() => {
	const receiptAmount = parseFloat(addcustomercollectionform.value.amount) || 0;
	// 使用更精确的比较方式，避免浮点数精度问题
	return Math.round(totalAmount.value * 100) / 100 === Math.round(receiptAmount * 100) / 100;
});

// 判断是否为新增记录
const isNewRecord = computed(() => {
	return EditID.value === 0;
});

// 判断当前记录是否为已领取状态
const isCollectedRecord = computed(() => {
	const hasCustomer = addcustomercollectionform.value.customerID && addcustomercollectionform.value.customerID !== '';
	const hasReceiver = addcustomercollectionform.value.receivingUser && addcustomercollectionform.value.receivingUser !== '';
	return hasCustomer && hasReceiver;
});

// 判断是否显示收款明细（有领取人和客户的情况下才显示）
const showReceivingPaymentsDetails = computed(() => {
	const hasCustomer = addcustomercollectionform.value.customerID && addcustomercollectionform.value.customerID !== '';
	const hasReceiver = addcustomercollectionform.value.receivingUser && addcustomercollectionform.value.receivingUser !== '';
	console.log('收款明细显示条件检查:', {
		hasCustomer,
		hasReceiver,
		customerID: addcustomercollectionform.value.customerID,
		receiver: addcustomercollectionform.value.receivingUser,
	});
	return hasCustomer && hasReceiver;
});

// 获取款项类别标签
const getFundsClassificationLabel = (value) => {
	if (!value) return '';
	const dict = state.optionss.hr_funds_classification.find((dict) => dict.dictValue === value.toString());
	return dict ? dict.dictLabel : value;
};

// 获取关联单号标签
const getAssociatedDocumentLabel = (value) => {
	if (!value) return '';
	// 在所有关联文档选项中查找
	const allOptions = [
		...(optionss.value.sql_sale_contracts || []),
		...(optionss.value.sql_shippingdeliveries || [])
	];
	const dict = allOptions.find(item => item.dictValue === value.toString());
	return dict ? dict.dictLabel : value;
};

// 监听收款明细表格数据变化，计算总金额
watch(ReceivingPaymentsDetailsTbaleData, (newData) => {
	const total = newData.reduce((sum, item) => {
		const amount = parseFloat(item.applicationAmount) || 0;
		// 使用更精确的计算方式，避免浮点数精度问题
		return Math.round((sum + amount) * 100) / 100;
	}, 0);
	totalAmount.value = total;
}, { deep: true });

// 监听客户ID和领取人变化，获取收款明细
watch([() => addcustomercollectionform.value.customerID, () => addcustomercollectionform.value.receivingUser], async ([newCustomerID, newReceiver]) => {
	if (newCustomerID && newCustomerID !== '' && newReceiver && newReceiver !== '' && EditID.value > 0) {
		// 有客户ID和领取人且是编辑模式时，获取收款明细
		await getCustomerCollectionsDetailsList(EditID.value);
	} else {
		// 清空收款明细
		ReceivingPaymentsDetailsTbaleData.value = [];
		totalAmount.value = 0;
	}
});

// 获取收款明细列表
const getCustomerCollectionsDetailsList = async (customerCollectionID) => {
	try {
		console.log('开始获取收款明细，ID:', customerCollectionID);
		const response = await request({
			url: 'CustomerCollections/GetCustomerCollectionsDetailsList/GetCustomerCollectionsDetailsList',
			method: 'GET',
			params: {
				CustomerCollectionID: customerCollectionID
			}
		});

		console.log('收款明细接口响应:', response);

		if (response.data && response.code === 200) {
			ReceivingPaymentsDetailsTbaleData.value = response.data || [];
			console.log('收款明细数据已设置:', ReceivingPaymentsDetailsTbaleData.value);
		} else {
			ReceivingPaymentsDetailsTbaleData.value = [];
			console.log('收款明细接口返回错误或空数据');
		}
	} catch (error) {
		console.error('获取收款明细列表失败:', error);
		ReceivingPaymentsDetailsTbaleData.value = [];
	}
};
</script>

<style scoped>
/* 创建收款单据dialog中的表单组件间距减少一半 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

/* 附件上传样式 */
.attachment-upload {
	margin-bottom: 10px;
}

.attachment-list {
	margin-top: 10px;
	border: 1px solid #e5e7eb;
	border-radius: 6px;
	padding: 10px;
	background-color: #f9fafb;
}

.attachment-item {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 8px 12px;
	margin-bottom: 8px;
	background-color: white;
	border: 1px solid #e5e7eb;
	border-radius: 4px;
	transition: all 0.2s ease;
}

.attachment-item:hover {
	background-color: #f8f9fa;
	border-color: #d1d5db;
}

.attachment-item:last-child {
	margin-bottom: 0;
}

.attachment-info {
	display: flex;
	align-items: center;
	flex: 1;
}

.attachment-icon {
	margin-right: 8px;
	color: #6b7280;
	font-size: 16px;
}

.attachment-name {
	font-weight: 500;
	color: #374151;
	margin-right: 8px;
	word-break: break-all;
}

.attachment-size {
	color: #6b7280;
	font-size: 12px;
}

.attachment-actions {
	display: flex;
	gap: 4px;
}

.attachment-actions .el-button {
	padding: 4px 8px;
	font-size: 12px;
}

.delete-btn {
	color: #ef4444 !important;
}

.delete-btn:hover {
	background-color: #fef2f2 !important;
}

/* 附件上传提示样式 */
.el-upload__tip {
	color: #6b7280;
	font-size: 12px;
	margin-top: 5px;
}
</style>
