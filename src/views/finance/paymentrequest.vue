<template>
	<div>
		<!-- 付款申请表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="AddPaymentDialog" size="default">新增付款申请</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" style="margin-bottom: 10px;">
					<el-col :span="4">
						<el-select v-model="SearchPaymentRequsetID" filterable placeholder="选择付款申请单号" size="default"
							style="width: 100%">
							<el-option v-for="dict in optionss.sql_payment_requests" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="SearchSupplierID" filterable placeholder="选择收款单位" size="default"
							style="width: 100%">
							<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchPaymentDateStart" type="date" placeholder="请选择申请日期起"
							size="default" style="width: 100%" />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchPaymentDateEnd" type="date" placeholder="请选择申请日期止" size="default"
							style="width: 100%" />
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchSubmitClick()" size="default">查询</el-button>
							<el-button @click="ResetClick()" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table :data="paymentrequesttableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="applicationNumber" label="申请单号" width="120">
					<template #default="scope">
						<span>{{ scope.row.applicationNumber }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
				<el-table-column prop="reviewStatusStr" label="审核状态" width="100" align="center">
					<template #default="{ row }">
						<template v-if="row.id"> <!-- 有ID才显示popover -->
							<el-popover placement="right" :width="400" trigger="click">
								<template #reference>
									<el-tag :type="getStatusType(row.reviewStatusStr)" @click="getApprovalFlow(row.id)"
										style="cursor: pointer">
										{{ row.reviewStatusStr }}
									</el-tag>
								</template>

								<!-- 有审批步骤才显示步骤条 -->
								<template #default>
									<div v-if="approvalSteps.length > 0" class="status-popover">
										<el-steps :active="approvalSteps.length" size="small">
											<el-step v-for="step in approvalSteps" :key="step.stageID"
												:title="step.approverUserName" :description="getStatusText(step.status)"
												:status="getStatus(step.status)" />
										</el-steps>
									</div>
									<div v-else>暂无审批流程</div>
								</template>
							</el-popover>
						</template>

						<!-- 没有ID时只显示tag -->
						<template v-else>
							<el-tag :type="getStatusType(row.contractReviewStatusStr)">
								{{ row.contractReviewStatusStr }}
							</el-tag>
						</template>
					</template>
				</el-table-column>
				<el-table-column prop="paymentCategory" label="付款类别" width="100"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="100"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位名称" width="200"></el-table-column>
				<el-table-column prop="bankName" label="开户银行" width="130"></el-table-column>
				<el-table-column prop="bankAccount" label="银行账号" width="200"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="110"></el-table-column>
				<el-table-column prop="currencyCode" label="货币代码" width="90"></el-table-column>
				<el-table-column prop="totalAmount" label="申请总额" width="90"></el-table-column>
				<el-table-column prop="paidAmount" label="已付金额" width="90"></el-table-column>
				<el-table-column prop="unpaidAmount" label="未付金额" width="90"></el-table-column>
				<el-table-column prop="applicant" label="申请人" width="90"></el-table-column>
				<el-table-column prop="applicationDepartment" label="申请部门" width="110"></el-table-column>
				<el-table-column prop="handler" label="经手人" width="150" v-if="false"></el-table-column>
				<el-table-column prop="applicationDate" label="申请日期" width="110"></el-table-column>
				<el-table-column fixed="right" label="操作" width="150">
					<template #default="scope">
						<el-button type="text" size="small" @click="CheckPaymentRequest(scope.row)">查看/编辑</el-button>
						<el-button v-if="scope.row.createBy === useUserStore().userId.toString() && scope.row.isDraft"
							link type="danger" size="small" @click="DeleteCustomerProfile(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="paymentrequesttableDataHandlePageChange"
				:current-page="paymentrequesttableDataCurrentPage" :page-size="paymentrequesttableDataPageSize"
				:total="paymentrequesttableDataTotalItems" background layout="prev, pager, next"
				style="margin-top: 5px;" />
		</div>
		<el-dialog :modal="false" :modal-penetrable="true" v-model="addpaymentrequestdialog" title="付款申请"
			:close-on-click-modal=false style="width: 75%;" @close="Closeaddpaymentrequestdialog()">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-form :model="addpaymentrequestform" label-width="120px" ref="paymentFormRef"
						:show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="申请单号">
									<el-input v-model="addpaymentrequestform.applicationNumber" style="width: 300px"
										disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="申请日期">
									<el-date-picker v-model="addpaymentrequestform.applicationDate" type="date"
										style="width: 300px" :disabled="IsDisabled" size="default"></el-date-picker>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="付款类别">
									<el-select v-model="addpaymentrequestform.paymentCategory" style="width: 300px"
										placeholder="请选择付款类别" @change="paymentCategoryChange" :disabled="IsDisabled"
										size="default">
										<el-option v-for="dict in optionss.hr_payment_category" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="款项名称">
									<el-select v-model="addpaymentrequestform.paymentName" style="width: 300px"
										placeholder="请选择款项名称" :disabled="IsDisabled" size="default">
										<el-option v-for="dict in PaymentTypeOptions" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="收款单位" placeholder="请选择收款单位">
									<el-select v-model="addpaymentrequestform.payeeCode" style="width: 300px"
										@change="payeeCodeChange()" :disabled="IsDisabled" filterable size="default">
										<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6" v-if="false">
								<el-form-item label="收款单位名称">
									<el-input v-model="addpaymentrequestform.payeeName" style="width: 300px" disabled
										size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="开户银行">
									<el-input v-model="addpaymentrequestform.bankName" style="width: 300px" disabled
										size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="银行账号">
									<el-select v-model="addpaymentrequestform.bankAccount" style="width: 300px"
										placeholder="请选择银行账号" :disabled="IsDisabled" @change="bankAccountChange"
										filterable clearable size="default">
										<el-option v-for="account in supplierBankAccounts" :key="account.id"
											:label="`${account.bank} - ${account.bank_account_number}`"
											:value="account.bank_account_number" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="我方公司">
									<el-select v-model="addpaymentrequestform.ourCompany" style="width: 300px"
										:disabled="IsDisabled" size="default">
										<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="币种">
									<el-select v-model="addpaymentrequestform.currencyCode" style="width: 300px"
										:disabled="IsDisabled" size="default">
										<el-option v-for="dict in optionss.hr_currency_code" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="申请金额">
									<el-input v-model="addpaymentrequestform.totalAmount" style="width: 300px"
										:disabled="IsDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6" v-if="false">
								<el-form-item label="已付金额">
									<el-input v-model="addpaymentrequestform.paidAmount" style="width: 300px"
										:disabled="IsDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="申请人">
									<el-select v-model="addpaymentrequestform.applicant" style="width: 300px"
										:disabled="IsDisabled" size="default">
										<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" :disabled="IsDisabled" />
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="备注说明">
									<el-input type="textarea" v-model="addpaymentrequestform.remarks"
										:autosize="{ minRows: 5, maxRows: 10 }" placeholder="输入备注内容" style="width: 100%"
										:disabled="IsDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="paymentDetailsCollapseActive" style="margin-bottom: 20px;"
				v-show="showPaymentDetails">
				<el-collapse-item title="付款明细" name="paymentDetails">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">付款明细</span>
					</template>
					<div style="margin-bottom: 10px;">
						<el-alert
							:title="`本次付款金额合计: ${getTotalCurrentPaymentAmount()} | 申请金额: ${addpaymentrequestform.totalAmount || '0.00'}`"
							:type="getPaymentAmountAlertType()" show-icon :closable="false">
						</el-alert>
						<div v-if="addpaymentrequestform.paymentCategory !== '1'"
							style="margin-top: 5px; color: #909399; font-size: 12px;">
							注：非工厂付款类别无需验证付款明细合计金额
						</div>
					</div>
					<el-table :data="CostDetailsTbaleData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="shippingOrderNumber" label="出运单号" width="150">
							<template #default="{ row }">
								<span>{{ row.shippingOrderNumber }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseContractNumber" label="采购合同号" width="150">
							<template #default="{ row }">
								<span>{{ row.purchaseContractNumber }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="currentPaymentAmount" label="本次付款金额" width="150">
							<template #default="{ row }">
								<el-input v-model="row.currentPaymentAmount" style="width: 130px" :disabled="IsDisabled"
									@input="onCurrentPaymentAmountChange" type="number" placeholder="请输入金额"
									:class="{ 'error-input': addpaymentrequestform.paymentCategory === '1' && isPaymentAmountExceeded(row) }"></el-input>
								<div v-if="addpaymentrequestform.paymentCategory === '1' && isPaymentAmountExceeded(row)"
									class="error-message" style="color: #f56c6c; font-size: 12px; margin-top: 2px;">
									本次付款金额不能超过实际发货金额
								</div>
							</template>
						</el-table-column>
						<el-table-column prop="totalGoodsValue" label="采购合同金额" width="150">
							<template #default="{ row }">
								<span>{{ row.totalGoodsValue }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="actualShippingAmount" label="实际发货金额" width="150">
							<template #default="{ row }">
								<span>{{ row.actualShippingAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="deposit" label="已付定金" width="150">
							<template #default="{ row }">
								<span>{{ row.deposit }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="paidAmount" label="已付货款" width="150">
							<template #default="{ row }">
								<span>{{ row.paidAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="unpaidAmount" label="未付货款" width="150">
							<template #default="{ row }">
								<span>{{ row.unpaidAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="contractStatus" label="采购合同状态" width="150">
							<template #default="{ row }">
								<span>{{ row.contractStatus }}</span>
							</template>
						</el-table-column>

						<el-table-column prop="remark" label="备注" width="150">
							<template #default="{ row }">
								<span>{{ row.remark }}</span>
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="large"
									@click="CostDetailsTbaleDatahandleDelete(scope.$index)"
									:disabled="IsDisabled">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="unpaidDetailsCollapseActive" style="margin-bottom: 20px;"
				v-show="showPaymentDetails && !IsDisabled">
				<el-collapse-item title="未支付款项详情" name="unpaidDetails">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">未支付款项详情</span>
					</template>
					<el-table :data="UnpaidDetailsTbaleData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column label="选择" width="85">
							<template #header>
								<el-checkbox :model-value="isAllUnpaidSelected" @change="handleSelectAllUnpaid"
									:indeterminate="isIndeterminateUnpaid">
									全选
								</el-checkbox>
							</template>
							<template #default="{ row }">
								<el-checkbox :model-value="isUnpaidItemSelected(row)"
									@change="handleUnpaidItemSelect(row)" />
							</template>
						</el-table-column>
						<el-table-column prop="id" label="ID" width="120" v-if="false">
							<template #default="{ row }">
								<span :class="{ 'selected-item': isItemSelected(row) }">{{ row.id }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="invoiceNumber" label="出运单号" width="120">
							<template #default="{ row }">
								<span>{{ row.invoiceNumber }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseContractNumber" label="采购合同号" width="120">
							<template #default="{ row }">
								<span>{{ row.purchaseContractNumber }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="totalGoodsValue" label="采购合同金额" width="120">
							<template #default="{ row }">
								<span>{{ row.totalGoodsValue }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="actualShippingAmount" label="实际发货金额" width="120">
							<template #default="{ row }">
								<span>{{ row.actualShippingAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="deposit" label="已付定金" width="120">
							<template #default="{ row }">
								<span>{{ row.deposit }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="paidAmount" label="已付货款" width="120">
							<template #default="{ row }">
								<span>{{ row.paidAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="unpaidAmount" label="未付货款" width="120">
							<template #default="{ row }">
								<span>{{ row.unpaidAmount }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="contractStatus" label="采购合同状态" width="120">
							<template #default="{ row }">
								<span>{{ row.contractStatus }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" width="120">
							<template #default="{ row }">
								<span>{{ row.remark }}</span>
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="100">
							<template #default="{ row }">
								<el-button type="primary" size="small" @click="handleUnpaidItemSelect(row)">
									选择
								</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-collapse-item>
			</el-collapse>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" v-show="isSaveBtnShow" @click="SavePaymentRequest()">
						保存草稿
					</el-button>
					<el-button type="warning" v-show="isEditSaveBtnShow" @click="EditSavePaymentRequest()">
						保存草稿
					</el-button>
					<el-button type="primary" v-show="showEditBtn" @click="EditPayment">
						编辑
					</el-button>
					<el-button type="success" v-show="showSubmitReviewBtn && isAmountMatched()"
						@click="submitForReview">
						提交
					</el-button>
					<el-button type="info" v-show="showSubmitReviewBtn && !isAmountMatched()" disabled>
						提交 ({{ addpaymentrequestform.paymentCategory === '1' ? '金额不匹配或超过实际发货金额' : '金额不匹配' }})
					</el-button>
					<el-button type="danger" v-show="showApproveRejectBtn" @click="ApproveReject">
						驳回
					</el-button>
					<el-button type="success" v-show="showApprovePassBtn" @click="Approvepass">
						通过
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { number } from 'echarts';
import { Edit } from '@element-plus/icons-vue/dist/types';
import { ca, el, id } from 'element-plus/es/locale';
import { dataScope } from '@/api/system/role';
import { JsonHubProtocol } from '@microsoft/signalr';
import { get } from 'sortablejs';
import { RefSymbol } from '@vue/reactivity';
import useUserStore from '@/store/modules/user'
import { useRoute } from 'vue-router'

const route = useRoute()
// 添加onMounted钩子
onMounted(() => {
	console.log('付款申请页面挂载，检查路由参数')
	autoLoadPaymentRequestDetail()
})

// 添加自动加载付款申请详情的函数
const autoLoadPaymentRequestDetail = async () => {
	// 检查URL参数
	const paymentRequestId = route.query.PaymentRequestID
	const viewDetail = route.query.viewDetail
	if (paymentRequestId && viewDetail === 'true') {
		console.log('自动加载付款申请详情, ID:', paymentRequestId)

		try {
			// 等待获取付款申请列表
			await GetPaymentRequestList(1, 100)

			// 查找匹配的付款申请
			const paymentrequest = paymentrequesttableData.value.find(item =>
				item.id.toString() === paymentRequestId.toString()
			)

			if (paymentrequest) {
				// 调用查看详情的函数
				await CheckPaymentRequest(paymentrequest)
				// 确保对话框显示
				addpaymentrequestdialog.value = true
			} else {
				console.error('未找到匹配的付款申请:', paymentRequestId)
				ElMessage.error('未找到匹配的付款申请')
			}
		} catch (error) {
			console.error('加载付款申请详情失败:', error)
			ElMessage.error('加载付款申请详情失败')
		}
	}
}

// 存储审批步骤数据
const approvalSteps = ref([])
// 获取审批流程
const getApprovalFlow = async (documentId: number) => {
	try {
		const res = await request({
			url: 'PaymentRequest/GetPaymentRequestApprovalFlowByPaymentRequestID/GetApprovalFlow',
			method: 'get',
			params: {
				DocumentID: documentId
			}
		})

		if (res.code === 200) {
			approvalSteps.value = res.data
		} else {
			ElMessage.error('获取审批流程失败')
		}
	} catch (error) {
		console.error('获取审批流程失败:', error)
		ElMessage.error('获取审批流程失败')
	}
}

// 获取状态文本
const getStatusText = (status: number) => {
	switch (status) {
		case 0: return '待审批'
		case 1: return '已通过'
		case 2: return '已拒绝'
		case 3: return '等待上一阶段'
		case 4: return '已终止'
		default: return '未知状态'
	}
}

// 获取状态类型
const getStatus = (status: number) => {
	switch (status) {
		case 0: return 'wait'
		case 1: return 'success'
		case 2: return 'error'
		case 3: return 'danger'
		case 4: return 'error'
		default: return 'wait'
	}
}

// 获取标签类型
const getStatusType = (status: string) => {
	switch (status) {
		case '待提审': return 'warning'
		case '审核中': return 'wait'
		case '已批准': return 'success'
		case '已拒绝': return 'error'
		default: return 'info'
	}
}


const showApproveRejectBtn = ref(false);
const showApprovePassBtn = ref(false);

// 审批文档请求对象
const ApproveDocumentRequest = reactive({
	ApprovalRecordID: 0,
	DocumentType: 0,
	DocumentID: 0,
	StageID: 0,
	ApproverID: 0,
	ApproveStatus: false
});

var userId = useUserStore().userId;
var CheckUser = ref(userId.toString()); // 初始化为当前用户ID
// 审核通过
const Approvepass = async () => {
	try {
		await ElMessageBox.confirm('确定通过该付款申请的审批吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 设置审批参数
		ApproveDocumentRequest.ApproveStatus = true;
		ApproveDocumentRequest.DocumentID = PaymentRequestID.value;
		ApproveDocumentRequest.ApproverID = userId;

		// 从审批流程中获取当前步骤信息
		const currentStep = approvalSteps.value.find(step => step.status === 0);
		if (currentStep) {
			ApproveDocumentRequest.StageID = currentStep.stageID;
			ApproveDocumentRequest.ApprovalRecordID = currentStep.recordID || 0;
		}

		request.post('ApprovalFlow/ApprovalDocument/ApprovalDocument', ApproveDocumentRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: response.data,
					type: 'success'
				});
				addpaymentrequestdialog.value = false;
				// 刷新合同列表
				GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
			} else {
				console.error('审批失败');
				ElMessage.error('审批失败');
			}
		}).catch(error => {
			console.error('审批失败', error);
			ElMessage.error('审批失败，请重试');
		});
	} catch (error) {
		if (error !== 'cancel') {
			console.error('审批确认失败:', error);
		}
	}
}

// 审核驳回
const ApproveReject = async () => {
	try {
		await ElMessageBox.confirm('确定驳回该付款申请的审批吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 设置审批参数
		ApproveDocumentRequest.ApproveStatus = false;
		ApproveDocumentRequest.DocumentID = 0;
		ApproveDocumentRequest.ApproverID = 0;

		// 从审批流程中获取当前步骤信息
		const currentStep = approvalSteps.value.find(step => step.status === 0);
		if (currentStep) {
			ApproveDocumentRequest.StageID = currentStep.stageID;
			ApproveDocumentRequest.ApprovalRecordID = currentStep.recordID;
		}

		request.post('ApprovalFlow/ApprovalDocument/ApprovalDocument', ApproveDocumentRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: response.data,
					type: 'success'
				});
				addpaymentrequestdialog.value = false;
				// 刷新合同列表
				GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
			} else {
				console.error('驳回失败');
				ElMessage.error('驳回失败');
			}
		}).catch(error => {
			console.error('驳回失败', error);
			ElMessage.error('驳回失败，请重试');
		});
	} catch (error) {
		if (error !== 'cancel') {
			console.error('驳回确认失败:', error);
		}
	}
}


// 检查当前用户是否是当前审批人
const checkIfCurrentUserIsApprover = () => {
	if (!approvalSteps.value || approvalSteps.value.length === 0) {
		return false;
	}

	// 查找状态为待审批(0)的步骤，这表示当前需要审批的步骤
	const currentStep = approvalSteps.value.find(step => step.status === 0);

	if (currentStep) {
		// 检查当前步骤的审批人是否是当前用户
		return currentStep.approverID.toString() === userId.toString();
	}

	return false;
}
// 获取用户信息
const userInfo = useUserStore().userInfo

/*查询条件*/
const SearchPaymentRequsetID = ref('');
const SearchSupplierID = ref('');
const SearchPaymentDateStart = ref('');
const SearchPaymentDateEnd = ref('');

const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);
const activeTab = ref('CostDetailsTab')//费用信息Tab
const addpaymentrequestdialog = ref(false)//付款申请Dialog
const basicInfoCollapseActive = ref(['basicInfo']);//基本信息折叠面板
const paymentDetailsCollapseActive = ref(['paymentDetails']);//付款明细折叠面板
const unpaidDetailsCollapseActive = ref(['unpaidDetails']);//未支付款项详情折叠面板
// 控制付款明细和未支付款项详情的显示
const showPaymentDetails = ref(true);
const paymentrequesttableData = ref([])//付款申请列表Table
const CostDetailsTbaleData = ref([])//费用明细Table
const UnpaidDetailsTbaleData = ref([])//未支付款项详情Table
const addpaymentrequestform = ref({
	paymentContractType: '',
	paymentContractID: 0,
	applicationNumber: '',
	applicationDate: '',
	paymentCategory: '',
	paymentName: '',
	payeeCode: '',
	payeeName: '',
	bankName: '',
	bankAccount: '',
	ourCompany: '',
	currencyCode: '',
	totalAmount: '',
	paidAmount: '',
	unpaidAmount: '',
	applicant: '',
	applicationDepartment: '',
	financialApproval: '',
	handler: '',
	remarks: ''
})

// 供应商银行账号列表
const supplierBankAccounts = ref([])
const handleAddRowCostDetails = () => {
	activeTab.value = 'CostDetailsTab'

	// 获取当前日期
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	const todayStr = `${year}-${month}-${day}`;

	CostDetailsTbaleData.value.push({
		shippingOrderNumber: '',
		ShippingOrderNumberOptions: [],
		purchaseContractNumber: '',
		PurchaseContractNumberOptions: [],
		totalGoodsValue: '',
		actualShippingAmount: '',
		deposit: '',
		paidAmount: '',
		unpaidAmount: '',
		currentPaymentAmount: '', // 新增本次付款金额字段
		contractStatus: '',
		remark: '',
		// 保留原有字段以兼容现有逻辑
		productCode: '',
		customerCode: '',
		chineseName: '',
		chineseSpec: '',
		englishName: '',
		englishSpec: '',
		associatedordernumber: '',
		AssociatedOrderNumberOptions: [],
		applicationamount: '',
		relevantdates: todayStr, // 设置关联日期默认值为当天
		relatedmodules: '',
		specificpaymentitems: ''
	})
}
/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_ourcompany: [],
		hr_export_currency: [],
		hr_bank: [],
		sql_customercollections_no: [],
		hr_payment_category: [],
		hr_factory_payment: [],
		hr_domestic_charges: [],
		hr_foreign_charges: [],
		hr_daily_expenses: [],
		sql_supplier_info: [],
		hr_currency_code: [],
		sql_all_user: [],
		sql_hr_sale: [],
		sql_hr_purchase: [],
		sql_hr_finance: [],
		sql_hr_dept: [],
		hr_associated_modules: [],
		sql_purchase_contract: [],
		sql_sale_contracts: [],
		sql_payment_requests: [],
		hr_payment_contract_type: [],
		sql_shippingdeliveries: [],
		hr_business_expenses: [],
		hr_contract_status: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_ourcompany' }, { dictType: 'hr_export_currency' }, { dictType: 'hr_bank' }, { dictType: 'sql_customercollections_no' },
{ dictType: 'hr_payment_category' }, { dictType: 'hr_factory_payment' }, { dictType: 'hr_domestic_charges' }, { dictType: 'hr_foreign_charges' },
{ dictType: 'hr_daily_expenses' }, { dictType: 'sql_supplier_info' }, { dictType: 'hr_currency_code' }, { dictType: 'sql_all_user' }, { dictType: 'sql_hr_sale' },
{ dictType: 'sql_hr_purchase' }, { dictType: 'sql_hr_finance' }, { dictType: 'sql_hr_dept' }, { dictType: 'hr_associated_modules' }, { dictType: 'sql_purchase_contract' },
{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_payment_requests' }, { dictType: 'hr_payment_contract_type' }, { dictType: 'sql_shippingdeliveries' }, { dictType: 'hr_business_expenses' }, { dictType: 'hr_contract_status' }]

async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		/*获取当前页面列表函数放在下方*/
		await GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);  // 现在可以安全执行
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
fetchDataAndExecute();
/*动态下拉框end*/
const PaymentTypeOptions = ref([]);
const paymentCategoryChange = async () => {
	addpaymentrequestform.value.paymentName = '';
	switch (addpaymentrequestform.value.paymentCategory) {
		case '1':
			PaymentTypeOptions.value = state.optionss.hr_factory_payment
			showPaymentDetails.value = true; // 工厂付款显示付款明细和未支付款项详情
			break;
		case '2':
			PaymentTypeOptions.value = state.optionss.hr_domestic_charges
			showPaymentDetails.value = true; // 国内费用显示付款明细和未支付款项详情
			break;
		case '3':
			PaymentTypeOptions.value = state.optionss.hr_foreign_charges
			showPaymentDetails.value = true; // 国外费用显示付款明细和未支付款项详情
			break;
		case '4':
			PaymentTypeOptions.value = state.optionss.hr_daily_expenses
			showPaymentDetails.value = false; // 日常费用隐藏付款明细和未支付款项详情
			break;
		case '5':
			PaymentTypeOptions.value = state.optionss.hr_business_expenses
			showPaymentDetails.value = false; // 业务费用隐藏付款明细和未支付款项详情
			break;
		default:
			showPaymentDetails.value = true;
			break;
	}

	// 当付款类别变化时，重新计算总金额以确保验证状态正确
	if (showPaymentDetails.value) {
		CalculatetotalAmount();

		// 如果切换到显示付款明细的类别且收款单位已有信息，获取未支付款项详情
		if (addpaymentrequestform.value.payeeCode && addpaymentrequestform.value.payeeCode !== '') {
			try {
				const unpaidResponse = await request({
					url: 'PaymentRequest/GetUnpaidPaymentListBySupplierID/GetUnpaidPaymentList',
					method: 'GET',
					params: {
						supplierID: addpaymentrequestform.value.payeeCode
					}
				});

				if (unpaidResponse.data && unpaidResponse.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
					UnpaidDetailsTbaleData.value.forEach((element) => {
						// 保存原始的contractStatus数值，同时添加显示用的contractStatusLabel
						element.contractStatusOriginal = element.contractStatus; // 保存原始数值
						element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
					});
				} else {
					UnpaidDetailsTbaleData.value = [];
				}
			} catch (unpaidError) {
				console.error('获取未付款详情失败:', unpaidError);
				UnpaidDetailsTbaleData.value = [];
			}
		} else {
			// 如果收款单位没有信息，清空未支付款项详情列表
			UnpaidDetailsTbaleData.value = [];
		}
	} else {
		// 如果隐藏付款明细，清空未支付款项详情列表
		UnpaidDetailsTbaleData.value = [];
	}
}

const relatedmoduleshandleChange = (row) => {
	row.associatedordernumber = '';
	switch (row.relatedmodules) {
		case '1':
			row.AssociatedOrderNumberOptions = state.optionss.sql_purchase_contract;
			break;
		case '2':
			row.AssociatedOrderNumberOptions = state.optionss.sql_sale_contracts;
			break;
		case '3':
			row.AssociatedOrderNumberOptions = state.optionss.sql_shippingdeliveries;
			break;
		default:
			row.AssociatedOrderNumberOptions = [];
			break;
	}
}
const payeeCodeChange = async () => {
	try {
		// 获取供应商基本信息
		const supplierResponse = await request({
			url: 'Supplierinfo/GetSupplierInfoByID/GetSupplierInfo',
			method: 'GET',
			params: {
				SupplierID: addpaymentrequestform.value.payeeCode
			}
		});

		// 1. 取下拉框label作为收款单位名称
		const selectedSupplier = state.optionss.sql_supplier_info.find(
			item => item.dictValue == addpaymentrequestform.value.payeeCode
		);
		addpaymentrequestform.value.payeeName = selectedSupplier ? selectedSupplier.dictLabel : '';

		// 获取供应商银行账号列表
		const bankAccountResponse = await request({
			url: 'Supplierinfo/GetSupplierBankAccountList/GetBankAccountList',
			method: 'GET',
			params: {
				supplierId: addpaymentrequestform.value.payeeCode
			}
		});

		if (bankAccountResponse.data && bankAccountResponse.code === 200) {
			supplierBankAccounts.value = bankAccountResponse.data || [];

			// 如果有银行账号，默认选中第一个
			if (supplierBankAccounts.value.length > 0) {
				const firstAccount = supplierBankAccounts.value[0];
				addpaymentrequestform.value.bankName = firstAccount.bank || '';
				addpaymentrequestform.value.bankAccount = firstAccount.bank_account_number || '';
			} else {
				// 如果没有银行账号，清空相关字段
				addpaymentrequestform.value.bankName = '';
				addpaymentrequestform.value.bankAccount = '';
			}
		} else {
			// 如果获取银行账号失败，使用供应商基本信息中的银行信息
			addpaymentrequestform.value.bankName = supplierResponse.bankName || '';
			addpaymentrequestform.value.bankAccount = supplierResponse.bankAccount || '';
			supplierBankAccounts.value = [];
		}

		// 只有在需要显示付款明细时才获取未付款详情列表
		if (showPaymentDetails.value) {
			try {
				const unpaidResponse = await request({
					url: 'PaymentRequest/GetUnpaidPaymentListBySupplierID/GetUnpaidPaymentList',
					method: 'GET',
					params: {
						supplierID: addpaymentrequestform.value.payeeCode
					}
				});

				if (unpaidResponse.data && unpaidResponse.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
					UnpaidDetailsTbaleData.value.forEach((element) => {
						// 保存原始的contractStatus数值，同时添加显示用的contractStatusLabel
						element.contractStatusOriginal = element.contractStatus; // 保存原始数值
						element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
					});
				} else {
					UnpaidDetailsTbaleData.value = [];
				}
			} catch (unpaidError) {
				console.error('获取未付款详情失败:', unpaidError);
				UnpaidDetailsTbaleData.value = [];
			}
		} else {
			// 如果不需要显示付款明细，清空未付款详情数据
			UnpaidDetailsTbaleData.value = [];
		}
	} catch (error) {
		console.error('获取供应商信息失败:', error);
		ElMessage({
			message: '获取供应商信息失败',
			type: 'error'
		});
		// 清空相关字段
		addpaymentrequestform.value.payeeName = '';
		addpaymentrequestform.value.bankName = '';
		addpaymentrequestform.value.bankAccount = '';
		supplierBankAccounts.value = [];
		UnpaidDetailsTbaleData.value = [];
	}
}

// 银行账号选择变化处理
const bankAccountChange = (selectedBankAccount) => {
	// 根据选择的银行账号找到对应的银行信息
	const selectedAccount = supplierBankAccounts.value.find(account => account.bank_account_number === selectedBankAccount);
	if (selectedAccount) {
		addpaymentrequestform.value.bankName = selectedAccount.bank || '';
	}
}

const paymentRequestRequest = reactive({
	id: 0,
	paymentContractType: 0,
	paymentContractID: 0,
	ApplicationNumber: '',
	ApplicationDate: '',
	PaymentCategory: 0,
	PaymentName: 0,
	PayeeCode: 0,
	PayeeName: '',
	BankName: '',
	BankAccount: '',
	OurCompany: 0,
	CurrencyCode: 0,
	TotalAmount: 0,
	PaidAmount: 0,
	UnpaidAmount: 0,
	Applicant: 0,
	ApplicationDepartment: 0,
	FinancialApproval: 0,
	Handler: 0,
	Remark: '',
	IsDelete: 0,
	PaymentRequestDetails: []
});

const SavePaymentRequest = () => {
	// 只有工厂付款才需要验证金额
	const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';

	// 如果是工厂付款且验证失败，提示用户
	if (isFactoryPayment && !validatePaymentAmount()) {
		ElMessage.error('工厂付款类别需要验证付款明细金额，请检查');
		return;
	}

	ElMessageBox.confirm('确定保存付款申请吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		paymentRequestRequest.ApplicationNumber = addpaymentrequestform.value.applicationNumber;
		paymentRequestRequest.ApplicationDate = addpaymentrequestform.value.applicationDate;
		paymentRequestRequest.PaymentCategory = Number(addpaymentrequestform.value.paymentCategory);
		paymentRequestRequest.PaymentName = Number(addpaymentrequestform.value.paymentName);
		paymentRequestRequest.PayeeCode = Number(addpaymentrequestform.value.payeeCode);
		paymentRequestRequest.PayeeName = addpaymentrequestform.value.payeeName || '';
		paymentRequestRequest.BankName = addpaymentrequestform.value.bankName;
		paymentRequestRequest.BankAccount = addpaymentrequestform.value.bankAccount;
		paymentRequestRequest.OurCompany = Number(addpaymentrequestform.value.ourCompany);
		paymentRequestRequest.CurrencyCode = Number(addpaymentrequestform.value.currencyCode);

		// 保存草稿时使用用户输入的申请金额，不重新计算
		paymentRequestRequest.TotalAmount = Number(addpaymentrequestform.value.totalAmount);
		paymentRequestRequest.PaidAmount = Number(addpaymentrequestform.value.paidAmount);
		paymentRequestRequest.UnpaidAmount = Number(addpaymentrequestform.value.unpaidAmount);
		paymentRequestRequest.Applicant = Number(addpaymentrequestform.value.applicant);
		paymentRequestRequest.ApplicationDepartment = Number(addpaymentrequestform.value.applicationDepartment);
		paymentRequestRequest.FinancialApproval = Number(addpaymentrequestform.value.financialApproval);
		paymentRequestRequest.Handler = Number(addpaymentrequestform.value.handler);
		paymentRequestRequest.Remark = addpaymentrequestform.value.remarks;
		paymentRequestRequest.IsDelete = 0;

		// 转换contractStatus为数字格式，并添加所有必需的字段，确保PaymentAmount被正确传递
		const processedDetails = CostDetailsTbaleData.value.map(detail => ({
			...detail,
			ShippingOrderID: Number(detail.id) || 0, // 关联出运发货单ID（来自未支付款项明细中的id）
			purchaseContracID: Number(detail.pcid) || 0, // 关联采购合同ID
			invoiceNumber: detail.shippingOrderNumber || '', // 关联出运发货单号
			purchaseContractNumber: detail.purchaseContractNumber || '', // 关联采购合同号
			PaymentAmount: Number(detail.currentPaymentAmount) || 0, // 映射到服务端期望的PaymentAmount字段
			contractStatus: Number(detail.contractStatusOriginal) || Number(detail.contractStatus) || 0 // 优先使用原始数值
		}));
		paymentRequestRequest.PaymentRequestDetails = processedDetails;
		request.post('PaymentRequest/AddPaymentRequest/Add', paymentRequestRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '付款申请单添加成功！',
					type: 'success'
				})
				addpaymentrequestdialog.value = false;
				GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
			} else {
				console.error('新增付款申请单出错');
			}
		}).catch(error => {
			console.error('新增付款申请单出错！😔错误内容：', error);
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}
//审核状态
const reviewStatusMap = {
	'0': '待提审',
	'1': '审核中',
	'2': '已批准',
	'3': '已拒绝'
}
///付款申请单表格数据
const paymentrequesttableDataTotalItems = ref(0);
const paymentrequesttableDataCurrentPage = ref(1);
const paymentrequesttableDataPageSize = ref(10);
const paymentrequesttableDataHandlePageChange = async (newPage) => {
	paymentrequesttableDataCurrentPage.value = newPage;
	const start = newPage;
	const end = paymentrequesttableDataPageSize.value;
	const newData = await GetPaymentRequestList(start, end);
};
function GetPaymentRequestList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'PaymentRequest/GetPaymentRequestList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				SearchPaymentRequsetID: SearchPaymentRequsetID.value,
				SearchSupplierID: SearchSupplierID.value,
				ApplicationDateStart: SearchPaymentDateStart.value,
				ApplicationDateEnd: SearchPaymentDateEnd.value
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				paymentrequesttableData.value = response.data.result;
				paymentrequesttableData.value.forEach((element) => {
					element.applicationDate = element.applicationDate.substring(0, 10);
					// 确保字典数据已加载
					if (state.optionss.hr_payment_category &&
						state.optionss.hr_ourcompany &&
						state.optionss.hr_currency_code &&
						state.optionss.sql_all_user &&
						state.optionss.sql_hr_dept) {

						element.paymentName = GetPaymentName(element.paymentCategory, element.paymentName);
						element.paymentCategory = state.optionss.hr_payment_category.find((item) => item.dictValue == element.paymentCategory)?.dictLabel || '';
						element.ourCompany = state.optionss.hr_ourcompany.find((item) => item.dictValue == element.ourCompany)?.dictLabel || '';
						element.currencyCode = state.optionss.hr_currency_code.find((item) => item.dictValue == element.currencyCode)?.dictLabel || '';
						element.applicant = state.optionss.sql_all_user.find((item) => item.dictValue == element.applicant)?.dictLabel || '';
						element.applicationDepartment = state.optionss.sql_hr_dept.find((item) => item.dictValue == element.applicationDepartment)?.dictLabel || '';
						element.handler = state.optionss.sql_all_user.find((item) => item.dictValue == element.handler)?.dictLabel || '';
					}
					element.reviewStatusStr = reviewStatusMap[element.reviewStatus.toString()] || '';
				});
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetPaymentRequestList(start - 1, end);
				} else {
					paymentrequesttableData.value = [];
				}
				reject(new Error('无数据'));
			}
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});
}
const isCheckAndEdit = ref(false);
const PaymentRequestID = ref(0);
const showEditBtn = ref(false);
const showSubmitReviewBtn = ref(false);
//查看详情
const CheckPaymentRequest = async (row) => {
	IsDisabled.value = true;
	isCheckAndEdit.value = true;
	if (row.reviewStatus == "0" || row.reviewStatus == "3") {
		showEditBtn.value = true;
		showSubmitReviewBtn.value = true;
	} else {
		showEditBtn.value = false;
		showSubmitReviewBtn.value = false;
	}
	isSaveBtnShow.value = false;
	isEditSaveBtnShow.value = false;

	PaymentRequestID.value = row.id;

	try {
		const response = await request({
			url: 'PaymentRequest/GetPaymentRequestDetailsByID/GetDetails',
			method: 'GET',
			params: {
				id: row.id
			}
		});

		addpaymentrequestform.value.applicationNumber = response.data.paymentRequest.applicationNumber;
		addpaymentrequestform.value.applicationDate = response.data.paymentRequest.applicationDate;
		addpaymentrequestform.value.paymentCategory = response.data.paymentRequest.paymentCategory.toString();
		// 根据付款类别设置显示状态
		switch (response.data.paymentRequest.paymentCategory.toString()) {
			case '1':
			case '2':
			case '3':
				showPaymentDetails.value = true; // 工厂付款、国内费用、国外费用显示付款明细
				break;
			case '4':
			case '5':
				showPaymentDetails.value = false; // 日常费用、业务费用隐藏付款明细
				break;
			default:
				showPaymentDetails.value = true;
				break;
		}
		paymentCategoryChange();
		addpaymentrequestform.value.paymentName = response.data.paymentRequest.paymentName.toString();
		addpaymentrequestform.value.payeeCode = response.data.paymentRequest.payeeCode.toString();
		addpaymentrequestform.value.payeeName = response.data.paymentRequest.payeeName;
		addpaymentrequestform.value.bankName = response.data.paymentRequest.bankName;
		addpaymentrequestform.value.bankAccount = response.data.paymentRequest.bankAccount;

		// 加载供应商银行账号列表
		try {
			const bankAccountResponse = await request({
				url: 'Supplierinfo/GetSupplierBankAccountList/GetBankAccountList',
				method: 'GET',
				params: {
					supplierId: response.data.paymentRequest.payeeCode
				}
			});

			if (bankAccountResponse.data && bankAccountResponse.data.code === 200) {
				supplierBankAccounts.value = bankAccountResponse.data.data || [];
			} else {
				supplierBankAccounts.value = [];
			}
		} catch (error) {
			console.error('获取供应商银行账号列表失败:', error);
			supplierBankAccounts.value = [];
		}

		// 只有在需要显示付款明细且处于编辑模式时才获取未付款详情列表
		if (showPaymentDetails.value && !IsDisabled.value) {
			try {
				const unpaidResponse = await request({
					url: 'PaymentRequest/GetUnpaidPaymentList/GetUnpaidPaymentList',
					method: 'GET',
					params: {
						supplierID: response.data.paymentRequest.payeeCode
					}
				});

				if (unpaidResponse.data && unpaidResponse.data.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data.data || [];
					// 处理contractStatus显示
					UnpaidDetailsTbaleData.value.forEach((element) => {
						// 保存原始的contractStatus数值，同时添加显示用的contractStatusLabel
						element.contractStatusOriginal = element.contractStatus; // 保存原始数值
						element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
					});
				} else {
					UnpaidDetailsTbaleData.value = [];
				}
			} catch (unpaidError) {
				console.error('获取未付款详情失败:', unpaidError);
				UnpaidDetailsTbaleData.value = [];
			}
		} else {
			// 如果不需要显示付款明细或处于查看模式，清空未付款详情数据
			UnpaidDetailsTbaleData.value = [];
		}
		addpaymentrequestform.value.ourCompany = response.data.paymentRequest.ourCompany.toString();
		addpaymentrequestform.value.currencyCode = response.data.paymentRequest.currencyCode.toString();
		addpaymentrequestform.value.totalAmount = response.data.paymentRequest.totalAmount;
		addpaymentrequestform.value.paidAmount = response.data.paymentRequest.paidAmount;
		addpaymentrequestform.value.unpaidAmount = response.data.paymentRequest.unpaidAmount;
		addpaymentrequestform.value.applicant = response.data.paymentRequest.applicant.toString();
		addpaymentrequestform.value.applicationDepartment = response.data.paymentRequest.applicationDepartment.toString();
		addpaymentrequestform.value.financialApproval = response.data.paymentRequest.financialApproval.toString();
		addpaymentrequestform.value.handler = response.data.paymentRequest.handler.toString();
		addpaymentrequestform.value.remarks = response.data.paymentRequest.remark || '';
		// 清空已选择的未付款项
		selectedUnpaidItemIds.value.clear();

		response.data.paymentRequestDetails.forEach((element) => {
			handleAddRowCostDetails();
		});
		CostDetailsTbaleData.value.forEach((element, index) => {
			const detail = response.data.paymentRequestDetails[index];
			element.id = detail.id;
			element.pcid = detail.purchaseContracID; // 保存pcid
			element.shippingOrderID = detail.shippingOrderID; // 保存shippingOrderID
			element.shippingOrderNumber = detail.invoiceNumber; // 出运单号
			element.purchaseContractNumber = detail.purchaseContractNumber; // 采购合同号
			element.totalGoodsValue = detail.totalGoodsValue;
			element.actualShippingAmount = detail.actualShippingAmount;
			element.deposit = detail.deposit;
			element.paidAmount = detail.paidAmount;
			element.unpaidAmount = detail.unpaidAmount;
			element.contractStatusOriginal = detail.contractStatus; // 保存原始contractStatus数值
			element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == detail.contractStatus)?.dictLabel || ''; // 显示用的标签
			element.currentPaymentAmount = detail.paymentAmount || ''; // 本次付款金额
			element.remark = detail.remark;

			// 兼容旧字段
			element.relatedmodules = detail.relatedModules?.toString() || '0';
			element.associatedordernumber = detail.associatedOrderNumber?.toString() || '0';
			element.applicationamount = detail.applicationAmount || 0;
			element.relevantdates = detail.relevantDates || '';
			element.specificpaymentitems = detail.specificPaymentItems?.toString() || '0';

			// 如果是从未付款项添加的数据，标记为已选择（仅在编辑模式下）
			if (element.shippingOrderNumber && element.purchaseContractNumber && !IsDisabled.value) {
				const unpaidItem = UnpaidDetailsTbaleData.value.find(item =>
					item.invoiceNumber === element.shippingOrderNumber &&
					item.purchaseContractNumber === element.purchaseContractNumber
				);
				if (unpaidItem) {
					selectedUnpaidItemIds.value.add(unpaidItem.id);
				}
			}
		});
		getApprovalFlow(row.id).then(() => {
			const isCurrentUserApprover = checkIfCurrentUserIsApprover();
			// 只有当前用户是审批人且合同在审核中时才显示审核按钮
			if (isCurrentUserApprover && row.reviewStatusStr === '审核中') {
				showApproveRejectBtn.value = true;
				showApprovePassBtn.value = true;
				// 设置文档类型（付款申请单）
				ApproveDocumentRequest.DocumentType = 5;//5付款申请单
			} else {
				// 如果不是审核中状态，隐藏审批按钮
				showApproveRejectBtn.value = false;
				showApprovePassBtn.value = false;
			}
		});
		addpaymentrequestdialog.value = true;
	} catch (error) {
		console.error(error);
	}
}
//获取付款申请单名称
const GetPaymentName = (paymentCategory, paymentName) => {
	if (!paymentCategory || !paymentName) {
		return '';
	}

	let dictList = [];
	switch (paymentCategory.toString()) {
		case '1':
			dictList = state.optionss.hr_factory_payment;
			break;
		case '2':
			dictList = state.optionss.hr_domestic_charges;
			break;
		case '3':
			dictList = state.optionss.hr_foreign_charges;
			break;
		case '4':
			dictList = state.optionss.hr_daily_expenses;
			break;
		case '5':
			dictList = state.optionss.hr_business_expenses;
			break;
		default:
			return '';
	}

	const foundItem = dictList?.find((item) => item.dictValue == paymentName);
	return foundItem?.dictLabel || '';
}
var AssociatedOrderNumberOptionsArrar = ref([]);
//获取关联单号
const GetAssociatedOrderNumberOptions = (relatedmodules) => {
	switch (relatedmodules) {
		case '1':
			AssociatedOrderNumberOptionsArrar.value = state.optionss.sql_purchase_contract;
			break;
		case '2':
			AssociatedOrderNumberOptionsArrar.value = state.optionss.sql_sale_contracts;
			break;
		default:
			AssociatedOrderNumberOptionsArrar.value = [];
			break;
	}
	return AssociatedOrderNumberOptionsArrar;
}
//编辑付款申请单
const EditSavePaymentRequest = () => {
	// 只有工厂付款才需要验证金额
	const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';

	// 如果是工厂付款且验证失败，提示用户
	if (isFactoryPayment && !validatePaymentAmount()) {
		ElMessage.error('工厂付款类别需要验证付款明细金额，请检查');
		return;
	}

	ElMessageBox.confirm('确定保存编辑后付款申请单吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		paymentRequestRequest.id = PaymentRequestID.value;
		paymentRequestRequest.ApplicationNumber = addpaymentrequestform.value.applicationNumber;
		paymentRequestRequest.ApplicationDate = addpaymentrequestform.value.applicationDate;
		paymentRequestRequest.PaymentCategory = Number(addpaymentrequestform.value.paymentCategory);
		paymentRequestRequest.PaymentName = Number(addpaymentrequestform.value.paymentName);
		paymentRequestRequest.PayeeCode = Number(addpaymentrequestform.value.payeeCode);
		paymentRequestRequest.PayeeName = addpaymentrequestform.value.payeeName || '';
		paymentRequestRequest.BankName = addpaymentrequestform.value.bankName;
		paymentRequestRequest.BankAccount = addpaymentrequestform.value.bankAccount;
		paymentRequestRequest.OurCompany = Number(addpaymentrequestform.value.ourCompany);
		paymentRequestRequest.CurrencyCode = Number(addpaymentrequestform.value.currencyCode);

		// 编辑时保持原有申请金额不变，不重新计算
		paymentRequestRequest.TotalAmount = Number(addpaymentrequestform.value.totalAmount);
		paymentRequestRequest.PaidAmount = Number(addpaymentrequestform.value.paidAmount);
		paymentRequestRequest.UnpaidAmount = Number(addpaymentrequestform.value.unpaidAmount);
		paymentRequestRequest.Applicant = Number(addpaymentrequestform.value.applicant);
		paymentRequestRequest.ApplicationDepartment = Number(addpaymentrequestform.value.applicationDepartment);
		paymentRequestRequest.FinancialApproval = Number(addpaymentrequestform.value.financialApproval);
		paymentRequestRequest.Handler = Number(addpaymentrequestform.value.handler);
		paymentRequestRequest.Remark = addpaymentrequestform.value.remarks;
		paymentRequestRequest.IsDelete = 0;

		// 转换contractStatus为数字格式，并添加所有必需的字段，确保PaymentAmount被正确传递
		const processedDetails = CostDetailsTbaleData.value.map(detail => ({
			...detail,
			ShippingOrderID: Number(detail.id) || 0, // 关联出运发货单ID（来自未支付款项明细中的id）
			purchaseContracID: Number(detail.pcid) || 0, // 关联采购合同ID
			invoiceNumber: detail.shippingOrderNumber || '', // 关联出运发货单号
			purchaseContractNumber: detail.purchaseContractNumber || '', // 关联采购合同号
			PaymentAmount: Number(detail.currentPaymentAmount) || 0, // 映射到服务端期望的PaymentAmount字段
			contractStatus: Number(detail.contractStatusOriginal) || Number(detail.contractStatus) || 0 // 优先使用原始数值
		}));
		paymentRequestRequest.PaymentRequestDetails = processedDetails;
		request.post('PaymentRequest/EditPaymentRequest/Edit', paymentRequestRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '付款申请单编辑成功！',
					type: 'success'
				})
				IsDisabled.value = true;
				showEditBtn.value = true;
				isEditSaveBtnShow.value = false;
				showSubmitReviewBtn.value = true;
				GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
			} else {
				console.error('编辑付款申请单出错');
			}
		}).catch(error => {
			console.error('编辑付款申请单出错！😔错误内容：', error);
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}
//关闭付款申请单对话框
const Closeaddpaymentrequestdialog = () => {
	resetForm();
	addpaymentrequestdialog.value = false;
	isEditSaveBtnShow.value = false;
	isSaveBtnShow.value = true;
	isCheckAndEdit.value = false;
	PaymentRequestID.value = 0;
}
//搜索
const SearchSubmitClick = () => {
	GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
}
//重置
const ResetClick = () => {
	SearchPaymentRequsetID.value = '';
	SearchSupplierID.value = '';
	SearchPaymentDateStart.value = '';
	SearchPaymentDateEnd.value = '';
	GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
}
//开启编辑
const IsDisabled = ref(true);
const EditPayment = async () => {
	showEditBtn.value = false;
	showSubmitReviewBtn.value = true;
	IsDisabled.value = false;
	isEditSaveBtnShow.value = true;

	// 编辑时获取未支付款项列表，过滤掉已选择的单据
	if (showPaymentDetails.value) {
		try {
			const unpaidResponse = await request({
				url: 'PaymentRequest/GetUnpaidPaymentListBySupplierID/GetUnpaidPaymentList',
				method: 'GET',
				params: {
					supplierID: addpaymentrequestform.value.payeeCode
				}
			});

			if (unpaidResponse.data && unpaidResponse.code === 200) {
				const allUnpaidItems = unpaidResponse.data || [];

				// 过滤掉已经存在于付款明细中的单据
				const filteredUnpaidItems = allUnpaidItems.filter(unpaidItem => {
					return !CostDetailsTbaleData.value.some(selectedItem =>
						selectedItem.shippingOrderNumber === unpaidItem.invoiceNumber &&
						selectedItem.purchaseContractNumber === unpaidItem.purchaseContractNumber
					);
				});

				UnpaidDetailsTbaleData.value = filteredUnpaidItems;

				// 处理contractStatus显示
				UnpaidDetailsTbaleData.value.forEach((element) => {
					element.contractStatusOriginal = element.contractStatus; // 保存原始数值
					element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
				});
			} else {
				UnpaidDetailsTbaleData.value = [];
			}
		} catch (unpaidError) {
			console.error('获取未付款详情失败:', unpaidError);
			UnpaidDetailsTbaleData.value = [];
		}
	}
}
//提交审核
const SubmitReview = () => {
	showEditBtn.value = false;
	showSubmitReviewBtn.value = true;
}


// 获取申请单号
const getNextPaymentNumber = async () => {
	try {
		const response = await request.get('PaymentRequest/GetPaymentNextNumber/GetNextNumber');
		if (response.code === 200) {
			addpaymentrequestform.value.applicationNumber = response.data;
		} else {
			ElMessage.error('获取申请单号失败');
		}
	} catch (error) {
		console.error('获取申请单号失败:', error);
		ElMessage.error('获取申请单号失败，请重试');
	}
};

const AddPaymentDialog = async () => {
	resetForm();
	await getNextPaymentNumber();
	IsDisabled.value = false;
	isSaveBtnShow.value = true;
	showSubmitReviewBtn.value = true;
	showEditBtn.value = false;
	isEditSaveBtnShow.value = false;

	// 设置默认申请日期为当天
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	addpaymentrequestform.value.applicationDate = `${year}-${month}-${day}`;

	// 设置部门和申请人
	if (userInfo.deptId.toString() == "0") {
		addpaymentrequestform.value.applicationDepartment = state.optionss.sql_hr_dept.find((item) => item.dictValue == "205").dictValue;
	} else {
		addpaymentrequestform.value.applicationDepartment = state.optionss.sql_hr_dept.find((item) => item.dictValue == userInfo.deptId.toString()).dictValue;
	}
	addpaymentrequestform.value.applicant = state.optionss.sql_all_user.find((item) => item.dictValue == userInfo.userId.toString()).dictValue;
	addpaymentrequestdialog.value = true;
}

// 添加表单ref
const paymentFormRef = ref();

// 表单验证规则
const rules = {
	paymentCategory: [{ required: true, message: '请选择付款类别', trigger: 'change' }],
	paymentName: [{ required: true, message: '请选择款项名称', trigger: 'change' }],
	payeeCode: [{ required: true, message: '请选择收款单位', trigger: 'change' }],
	ourCompany: [{ required: true, message: '请选择我方公司', trigger: 'change' }],
	currencyCode: [{ required: true, message: '请选择货币代码', trigger: 'change' }],
	applicant: [{ required: true, message: '请选择申请人', trigger: 'change' }],
	applicationDepartment: [{ required: true, message: '请选择申请部门', trigger: 'change' }],
	handler: [{ required: true, message: '请选择经手人', trigger: 'change' }]
};

// 提交审核方法
const submitForReview = () => {
	// 验证付款金额 - 只有工厂付款才需要验证
	if (!validatePaymentAmount()) {
		return;
	}

	paymentFormRef.value.validate((valid) => {
		if (valid) {
			ElMessageBox.confirm('确定提交审核吗?', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				// 根据付款类别决定使用哪个金额
				const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';
				let finalTotalAmount;

				if (isFactoryPayment) {
					// 工厂付款：使用重新计算的总金额，确保与明细金额一致
					finalTotalAmount = CostDetailsTbaleData.value.reduce((sum, row) => {
						return sum + (parseFloat(row.currentPaymentAmount) || 0);
					}, 0);
				} else {
					// 非工厂付款：使用用户输入的申请金额
					finalTotalAmount = Number(addpaymentrequestform.value.totalAmount) || 0;
				}

				// 构造请求数据（与保存草稿的数据结构保持一致）
				const requestData = {
					id: PaymentRequestID.value || 0, // 新增时为0，更新时为实际ID
					ApplicationNumber: addpaymentrequestform.value.applicationNumber,
					ApplicationDate: addpaymentrequestform.value.applicationDate,
					PaymentCategory: Number(addpaymentrequestform.value.paymentCategory),
					PaymentName: Number(addpaymentrequestform.value.paymentName),
					PayeeCode: Number(addpaymentrequestform.value.payeeCode),
					PayeeName: addpaymentrequestform.value.payeeName || '',
					BankName: addpaymentrequestform.value.bankName,
					BankAccount: addpaymentrequestform.value.bankAccount,
					OurCompany: Number(addpaymentrequestform.value.ourCompany),
					CurrencyCode: Number(addpaymentrequestform.value.currencyCode),
					TotalAmount: finalTotalAmount, // 根据付款类别使用相应的金额
					PaidAmount: Number(addpaymentrequestform.value.paidAmount),
					UnpaidAmount: Number(addpaymentrequestform.value.unpaidAmount),
					Applicant: Number(addpaymentrequestform.value.applicant),
					ApplicationDepartment: Number(addpaymentrequestform.value.applicationDepartment),
					FinancialApproval: Number(addpaymentrequestform.value.financialApproval),
					Handler: Number(addpaymentrequestform.value.handler),
					Remark: addpaymentrequestform.value.remarks, // 注意字段名映射
					PaymentRequestDetails: (CostDetailsTbaleData.value || []).map(detail => ({
						...detail,
						ShippingOrderID: Number(detail.id) || 0, // 关联出运发货单ID（来自未支付款项明细中的id）
						purchaseContracID: Number(detail.pcid) || 0, // 关联采购合同ID
						invoiceNumber: detail.shippingOrderNumber || '', // 关联出运发货单号
						purchaseContractNumber: detail.purchaseContractNumber || '', // 关联采购合同号
						PaymentAmount: Number(detail.currentPaymentAmount) || 0, // 映射到服务端期望的PaymentAmount字段
						contractStatus: Number(detail.contractStatusOriginal) || Number(detail.contractStatus) || 0 // 优先使用原始数值
					}))
				};

				// 发送保存并提交审核请求
				request({
					url: 'PaymentRequest/SaveAndSubmitForReview/SaveAndSubmitForReview',
					method: 'POST',
					data: requestData
				}).then(response => {
					if (response.code === 200) {
						ElMessage({
							message: response.msg || "付款申请单已保存并提交审核！",
							type: "success"
						});
						// 隐藏所有按钮
						showEditBtn.value = false;
						showSubmitReviewBtn.value = false;
						IsDisabled.value = true;
						isEditSaveBtnShow.value = false;
						isSaveBtnShow.value = false;
						isCheckAndEdit.value = false;
						PaymentRequestID.value = 0;
						// 关闭对话框
						addpaymentrequestdialog.value = false;
						// 刷新列表
						GetPaymentRequestList(
							paymentrequesttableDataCurrentPage.value,
							paymentrequesttableDataPageSize.value
						);
					} else {
						ElMessage.error(response.msg || '提交审核失败');
					}
				}).catch(error => {
					console.error('提交审核失败:', error);
					ElMessage.error('提交审核失败，请重试');
				});
			}).catch(() => {
				ElMessage({
					type: 'info',
					message: '已取消提交审核'
				});
			});
		} else {
			ElMessage.error('请填写必填项');
			return false;
		}
	});
};
// const submitForReview = () => {
// 	paymentFormRef.value.validate((valid) => {
// 		if (valid) {
// 			ElMessageBox.confirm('确定提交审核吗?', '提示', {
// 				confirmButtonText: '确定',
// 				cancelButtonText: '取消',
// 				type: 'warning'
// 			}).then(() => {
// 				// 发送提交审核请求
// 				request({
// 					url: 'PaymentRequest/SubmitForReview/SubmitPaymentReview',
// 					method: 'GET',
// 					params: {
// 						PaymentID: PaymentRequestID.value
// 					}
// 				}).then(response => {
// 					if (response.code === 200) {
// 						ElMessage({
// 							message: response.msg || "付款申请单已提交审核！",
// 							type: "success"
// 						});
// 						// 隐藏所有按钮
// 						showEditBtn.value = false;
// 						showSubmitReviewBtn.value = false;
// 						IsDisabled.value = true;
// 						isEditSaveBtnShow.value = false;
// 						isSaveBtnShow.value = false;
// 						isCheckAndEdit.value = false;
// 						PaymentRequestID.value = 0;

// 						// 关闭对话框
// 						addpaymentrequestdialog.value = false;

// 						// 刷新列表
// 						GetPaymentRequestList(
// 							paymentrequesttableDataCurrentPage.value,
// 							paymentrequesttableDataPageSize.value
// 						);
// 					} else {
// 						ElMessage.error(response.msg || '提交审核失败');
// 					}
// 				}).catch(error => {
// 					console.error('提交审核失败:', error);
// 					ElMessage.error('提交审核失败，请重试');
// 				});
// 			}).catch(() => {
// 				ElMessage({
// 					type: 'info',
// 					message: '已取消提交审核'
// 				});
// 			});
// 		} else {
// 			ElMessage.error('请填写必填项');
// 			return false;
// 		}
// 	});
// };

// 清空表单数据的方法
const resetForm = () => {
	showEditBtn.value = false;
	showSubmitReviewBtn.value = false;
	IsDisabled.value = false;
	isEditSaveBtnShow.value = false;
	isSaveBtnShow.value = true;
	isCheckAndEdit.value = false;
	PaymentRequestID.value = 0;
	// 重置显示状态为默认显示
	showPaymentDetails.value = true;

	if (paymentFormRef.value) {
		paymentFormRef.value.resetFields();
	}

	addpaymentrequestform.value = {
		paymentContractType: '',
		paymentContractID: 0,
		applicationNumber: '',
		applicationDate: '',
		applicationDepartment: '',
		paymentCategory: '',
		paymentName: '',
		payeeCode: '',
		payeeName: '',
		bankName: '',
		bankAccount: '',
		ourCompany: '',
		currencyCode: '',
		totalAmount: '',
		paidAmount: '',
		unpaidAmount: '',
		applicant: '',
		financialApproval: '',
		handler: '',
		remarks: ''
	};

	// 清空费用明细表格数据
	CostDetailsTbaleData.value = [];

	// 清空供应商银行账号列表
	supplierBankAccounts.value = [];

	// 清空未付款详情数据
	UnpaidDetailsTbaleData.value = [];

	// 清空已选择的未付款项
	selectedUnpaidItemIds.value.clear();
};

const CalculatetotalAmount = () => {
	// 初始化总金额为0
	let total = 0;

	// 遍历费用明细表格中的所有行
	CostDetailsTbaleData.value.forEach(row => {
		// 将字符串转换为数字并累加
		// 优先使用currentPaymentAmount，如果没有则使用unpaidAmount，最后使用applicationamount
		let amount = 0;
		if (row.currentPaymentAmount) {
			amount = parseFloat(row.currentPaymentAmount) || 0;
		} else if (row.unpaidAmount) {
			amount = parseFloat(row.unpaidAmount) || 0;
		} else if (row.applicationamount) {
			amount = parseFloat(row.applicationamount) || 0;
		}
		total += amount;
	});

	// 只更新未付金额字段（总金额 - 已付金额），不再自动赋值申请金额
	const paidAmount = parseFloat(addpaymentrequestform.value.paidAmount) || 0;
	addpaymentrequestform.value.unpaidAmount = (total - paidAmount).toFixed(2);
}

// 存储已选择的未付款项ID
const selectedUnpaidItemIds = ref(new Set());

// 检查项目是否已被选择
const isItemSelected = (row) => {
	return selectedUnpaidItemIds.value.has(row.id);
};

// 检查未付款项是否已被选择
const isUnpaidItemSelected = (row) => {
	// 如果项目在付款明细中，则认为已选择
	return CostDetailsTbaleData.value.some(item =>
		item.shippingOrderNumber === row.invoiceNumber &&
		item.purchaseContractNumber === row.purchaseContractNumber
	);
};

// 检查是否全选未付款项
const isAllUnpaidSelected = computed(() => {
	// 当所有未付款项都在付款明细中时，认为全选
	return UnpaidDetailsTbaleData.value.length === 0 && CostDetailsTbaleData.value.length > 0;
});

// 检查是否部分选择（用于显示半选状态）
const isIndeterminateUnpaid = computed(() => {
	// 当既有未付款项又有付款明细时，显示半选状态
	return UnpaidDetailsTbaleData.value.length > 0 && CostDetailsTbaleData.value.length > 0;
});

// 处理未付款项选择事件
const handleUnpaidItemSelect = (row) => {
	const isSelected = isUnpaidItemSelected(row);

	if (isSelected) {
		// 如果已选择，则取消选择
		selectedUnpaidItemIds.value.delete(row.id);

		// 从付款明细中移除
		const index = CostDetailsTbaleData.value.findIndex(item =>
			item.shippingOrderNumber === row.invoiceNumber &&
			item.purchaseContractNumber === row.purchaseContractNumber
		);
		if (index > -1) {
			CostDetailsTbaleData.value.splice(index, 1);
		}

		// 重新添加到未支付款项详情列表
		UnpaidDetailsTbaleData.value.push(row);

		ElMessage.success('已从付款明细中移除');
	} else {
		// 如果未选择，则添加到付款明细
		const newDetail = {
			id: row.id || 0, // 保存原始ID用于ShippingOrderID映射
			pcid: row.pcid || 0, // 保存pcid用于purchaseContracID映射
			shippingOrderNumber: row.invoiceNumber || '',
			ShippingOrderNumberOptions: [],
			purchaseContractNumber: row.purchaseContractNumber || '',
			PurchaseContractNumberOptions: [],
			totalGoodsValue: row.totalGoodsValue || '',
			actualShippingAmount: row.actualShippingAmount || '',
			deposit: row.deposit || 0,
			paidAmount: row.paidAmount || 0,
			unpaidAmount: row.unpaidAmount || 0,
			currentPaymentAmount: Math.min(row.unpaidAmount || 0, row.actualShippingAmount || 0), // 默认使用未付金额和实际发货金额的较小值
			contractStatus: row.contractStatus || '', // 显示用的标签
			contractStatusOriginal: row.contractStatusOriginal || row.contractStatus || 0, // 保存原始数值
			remark: row.remark || ''
		};

		CostDetailsTbaleData.value.push(newDetail);

		// 标记为已选择
		selectedUnpaidItemIds.value.add(row.id);

		// 从未支付款项详情列表中移除该行
		const index = UnpaidDetailsTbaleData.value.findIndex(item => item.id === row.id);
		if (index > -1) {
			UnpaidDetailsTbaleData.value.splice(index, 1);
		}

		ElMessage.success('已添加到付款明细');
	}

	// 重新计算总金额
	CalculatetotalAmount();
};

// 处理全选未付款项
const handleSelectAllUnpaid = (checked) => {
	if (checked) {
		// 全选：将所有未付款项添加到付款明细（保留现有数据）
		UnpaidDetailsTbaleData.value.forEach(row => {
			// 检查是否已经存在于付款明细中
			const alreadyExists = CostDetailsTbaleData.value.some(selectedItem =>
				selectedItem.shippingOrderNumber === row.invoiceNumber &&
				selectedItem.purchaseContractNumber === row.purchaseContractNumber
			);

			if (!alreadyExists) {
				const newDetail = {
					id: row.id || 0, // 保存原始ID用于ShippingOrderID映射
					pcid: row.pcid || 0, // 保存pcid用于purchaseContracID映射
					shippingOrderNumber: row.invoiceNumber || '',
					ShippingOrderNumberOptions: [],
					purchaseContractNumber: row.purchaseContractNumber || '',
					PurchaseContractNumberOptions: [],
					totalGoodsValue: row.totalGoodsValue || '',
					actualShippingAmount: row.actualShippingAmount || '',
					deposit: row.deposit || 0,
					paidAmount: row.paidAmount || 0,
					unpaidAmount: row.unpaidAmount || 0,
					currentPaymentAmount: Math.min(row.unpaidAmount || 0, row.actualShippingAmount || 0), // 使用未付金额和实际发货金额的较小值
					contractStatus: row.contractStatus || '', // 显示用的标签
					contractStatusOriginal: row.contractStatusOriginal || row.contractStatus || 0, // 保存原始数值
					remark: row.remark || ''
				};

				CostDetailsTbaleData.value.push(newDetail);
				selectedUnpaidItemIds.value.add(row.id);
			}
		});

		// 清空未支付款项详情列表
		UnpaidDetailsTbaleData.value = [];

		ElMessage.success('已全选所有未付款项');
	} else {
		// 取消全选：将所有已选择的项目移回未支付款项详情列表
		const itemsToMoveBack = [];

		CostDetailsTbaleData.value.forEach(item => {
			const originalItem = {
				id: item.id || Date.now() + Math.random(), // 保持原始ID，如果没有则生成新的
				pcid: item.pcid || 0, // 保持原始pcid
				invoiceNumber: item.shippingOrderNumber,
				purchaseContractNumber: item.purchaseContractNumber,
				totalGoodsValue: item.totalGoodsValue,
				actualShippingAmount: item.actualShippingAmount,
				deposit: item.deposit || 0,
				paidAmount: item.paidAmount || 0,
				unpaidAmount: item.unpaidAmount || 0,
				contractStatus: item.contractStatus,
				contractStatusOriginal: item.contractStatusOriginal || item.contractStatus || 0,
				remark: item.remark
			};
			itemsToMoveBack.push(originalItem);
		});

		// 清空付款明细
		CostDetailsTbaleData.value = [];
		selectedUnpaidItemIds.value.clear();

		// 将所有项目移回未支付款项详情列表
		UnpaidDetailsTbaleData.value = itemsToMoveBack;

		ElMessage.success('已取消全选');
	}

	// 重新计算总金额
	CalculatetotalAmount();
};

// 验证付款金额
const validatePaymentAmount = () => {
	// 只有工厂付款(1)才需要验证付款明细合计金额
	const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';

	if (!isFactoryPayment) {
		// 非工厂付款类别不需要验证合计金额，直接返回true
		return true;
	}

	const totalPaymentAmount = CostDetailsTbaleData.value.reduce((sum, row) => {
		return sum + (parseFloat(row.currentPaymentAmount) || 0);
	}, 0);

	const applicationAmount = parseFloat(addpaymentrequestform.value.totalAmount) || 0;

	// 检查是否有超过实际发货金额的情况
	const hasExceededAmount = CostDetailsTbaleData.value.some(row => isPaymentAmountExceeded(row));
	if (hasExceededAmount) {
		ElMessage.error('存在本次付款金额超过实际发货金额的情况，请检查');
		return false;
	}

	// 本次付款金额合计必须完全等于申请金额
	return totalPaymentAmount === applicationAmount; // 必须完全匹配
};

// 检查本次付款金额是否超过实际发货金额
const isPaymentAmountExceeded = (row) => {
	const currentPaymentAmount = parseFloat(row.currentPaymentAmount) || 0;
	const actualShippingAmount = parseFloat(row.actualShippingAmount) || 0;
	return currentPaymentAmount > actualShippingAmount;
};

// 检查金额是否匹配（用于控制提交按钮）
const isAmountMatched = () => {
	// 只有工厂付款(1)才需要验证付款明细合计金额
	const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';

	if (!isFactoryPayment) {
		// 非工厂付款类别不需要验证合计金额，直接返回true
		return true;
	}

	const totalPayment = parseFloat(getTotalCurrentPaymentAmount());
	const applicationAmount = parseFloat(addpaymentrequestform.value.totalAmount) || 0;

	// 检查是否有超过实际发货金额的情况
	const hasExceededAmount = CostDetailsTbaleData.value.some(row => isPaymentAmountExceeded(row));

	return totalPayment === applicationAmount && !hasExceededAmount; // 必须完全匹配且不超过实际发货金额
};



// 处理本次付款金额变化
const onCurrentPaymentAmountChange = () => {
	// 实时验证金额，但不阻止用户输入
	const totalPayment = parseFloat(getTotalCurrentPaymentAmount());
	const applicationAmount = parseFloat(addpaymentrequestform.value.totalAmount) || 0;

	// 检查是否有超过实际发货金额的情况
	const hasExceededAmount = CostDetailsTbaleData.value.some(row => isPaymentAmountExceeded(row));

	// 如果存在超过实际发货金额的情况，显示警告
	if (hasExceededAmount) {
		// 可以在这里添加实时提示，但为了避免过多的提示，只在提交时显示错误
	}
};

// 获取本次付款金额合计
const getTotalCurrentPaymentAmount = () => {
	const total = CostDetailsTbaleData.value.reduce((sum, row) => {
		return sum + (parseFloat(row.currentPaymentAmount) || 0);
	}, 0);
	return total.toFixed(2);
};

// 获取付款金额警告类型
const getPaymentAmountAlertType = () => {
	// 只有工厂付款(1)才需要验证付款明细合计金额
	const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';

	if (!isFactoryPayment) {
		// 非工厂付款类别不显示警告，返回info
		return 'info';
	}

	const totalPayment = parseFloat(getTotalCurrentPaymentAmount());
	const applicationAmount = parseFloat(addpaymentrequestform.value.totalAmount) || 0;

	if (totalPayment === 0) {
		return 'info';
	} else if (totalPayment > applicationAmount) {
		return 'error';
	} else if (totalPayment < applicationAmount) {
		return 'warning'; // 改为warning，表示需要匹配
	} else {
		return 'success';
	}
};



// 从付款明细中移除项目时，重新添加到未支付款项列表中
const CostDetailsTbaleDatahandleDelete = (index: number) => {
	const deletedItem = CostDetailsTbaleData.value[index];

	// 如果删除的项目来自未付款项，重新添加到未支付款项列表中
	if (deletedItem.shippingOrderNumber && deletedItem.purchaseContractNumber) {
		// 检查是否已经存在于未支付款项列表中
		const alreadyExists = UnpaidDetailsTbaleData.value.some(item =>
			item.invoiceNumber === deletedItem.shippingOrderNumber &&
			item.purchaseContractNumber === deletedItem.purchaseContractNumber
		);

		if (!alreadyExists) {
			// 重新添加到未支付款项列表中
			const originalUnpaidItem = {
				id: deletedItem.id || Date.now(), // 使用时间戳作为临时ID
				pcid: deletedItem.pcid || 0, // 保持原始pcid
				invoiceNumber: deletedItem.shippingOrderNumber,
				purchaseContractNumber: deletedItem.purchaseContractNumber,
				totalGoodsValue: deletedItem.totalGoodsValue,
				actualShippingAmount: deletedItem.actualShippingAmount,
				deposit: deletedItem.deposit || 0,
				paidAmount: deletedItem.paidAmount || 0,
				unpaidAmount: deletedItem.unpaidAmount || 0,
				contractStatus: deletedItem.contractStatus,
				contractStatusOriginal: deletedItem.contractStatusOriginal || deletedItem.contractStatus || 0,
				remark: deletedItem.remark
			};

			// 重新添加到未支付款项列表中
			UnpaidDetailsTbaleData.value.push(originalUnpaidItem);
		}

		// 从已选择列表中移除
		selectedUnpaidItemIds.value.delete(deletedItem.id);
	}

	CostDetailsTbaleData.value.splice(index, 1);
	CalculatetotalAmount(); // 重新计算总金额
};

const DeleteCustomerProfile = (row) => {
	ElMessageBox.confirm('确定要删除该付款申请单吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'PaymentRequest/DeletePaymentRequest/Delete',
			method: 'post',
			data: { PaymentRequestID: row.id }
		}).then(response => {
			if (response.code === 200) {
				ElMessage.success(response.msg || '删除成功');
				GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
			} else {
				ElMessage.error(response.msg || '删除失败');
			}
		}).catch(() => {
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage.info('已取消删除');
	});
};


</script>

<style scoped>
.selected-item {
	color: #f56c6c !important;
	font-weight: bold;
	text-decoration: line-through;
}

.error-input {
	border-color: #f56c6c !important;
}

.error-input:focus {
	border-color: #f56c6c !important;
	box-shadow: 0 0 0 2px rgba(245, 108, 108, 0.2) !important;
}

/* 付款申请dialog中的表单组件间距减少一半 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

.el-dialog .el-row {
	margin-bottom: 2.5px !important;
}
</style>
