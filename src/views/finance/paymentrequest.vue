<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="AddPaymentDialog">新增付款申请</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="SearchPaymentRequsetID" filterable placeholder="选择付款申请单号" style="width: 15%">
				<el-option v-for="dict in optionss.sql_payment_requests" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SearchSupplierID" filterable placeholder="选择收款单位" style="width: 15%">
				<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchPaymentDateStart" type="date" placeholder="请选择申请日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchPaymentDateEnd" type="date" placeholder="请选择申请日期止" size="Default"
				style="width: 15%" />
		</div>

		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchSubmitClick()">查询</el-button>
				<el-button @click="ResetClick()">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;付款申请列表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="paymentrequesttableData">
			<el-table-column prop="applicationNumber" label="申请单号" width="180">
				<template #default="scope">
					<span>{{ scope.row.applicationNumber }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
			<el-table-column prop="reviewStatusStr" label="审核状态" width="150" align="center">
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
			<el-table-column prop="paymentCategory" label="付款类别" width="150"></el-table-column>
			<el-table-column prop="paymentName" label="款项名称" width="150"></el-table-column>
			<el-table-column prop="payeeName" label="收款单位名称" width="150"></el-table-column>
			<el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
			<el-table-column prop="bankAccount" label="银行账号" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="currencyCode" label="货币代码" width="150"></el-table-column>
			<el-table-column prop="totalAmount" label="申请总额" width="150"></el-table-column>
			<el-table-column prop="paidAmount" label="已付金额" width="150"></el-table-column>
			<el-table-column prop="unpaidAmount" label="未付金额" width="150"></el-table-column>
			<el-table-column prop="applicant" label="申请人" width="150"></el-table-column>
			<el-table-column prop="applicationDepartment" label="申请部门" width="150"></el-table-column>
			<el-table-column prop="handler" label="经手人" width="150"></el-table-column>
			<el-table-column prop="applicationDate" label="申请日期" width="150"></el-table-column>
			<el-table-column fixed="right" label="操作" width="150">
				<template #default="scope">
					<el-button type="text" size="small" @click="CheckPaymentRequest(scope.row)">查看/编辑</el-button>
					<el-button v-if="scope.row.createBy === useUserStore().userId.toString() && scope.row.isDraft" link
						type="danger" size="small" @click="DeleteCustomerProfile(scope.row)">删除</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="paymentrequesttableDataHandlePageChange"
			:current-page="paymentrequesttableDataCurrentPage" :page-size="paymentrequesttableDataPageSize"
			:total="paymentrequesttableDataTotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="addpaymentrequestdialog" title="付款申请" :close-on-click-modal=false style="width: 70%;"
			@close="Closeaddpaymentrequestdialog()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="addpaymentrequestform" label-width="120px" ref="paymentFormRef">
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="申请单号">
							<el-input v-model="addpaymentrequestform.applicationNumber" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="申请日期">
							<el-date-picker v-model="addpaymentrequestform.applicationDate" type="date"
								style="width: 300px" :disabled="IsDisabled"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="付款类别">
							<el-select v-model="addpaymentrequestform.paymentCategory" style="width: 300px"
								placeholder="请选择付款类别" @change="paymentCategoryChange" :disabled="IsDisabled">
								<el-option v-for="dict in optionss.hr_payment_category" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="款项名称">
							<el-select v-model="addpaymentrequestform.paymentName" style="width: 300px"
								placeholder="请选择款项名称" :disabled="IsDisabled">
								<el-option v-for="dict in PaymentTypeOptions" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收款单位编号" placeholder="请选择收款单位编号">
							<el-select v-model="addpaymentrequestform.payeeCode" style="width: 300px"
								@change="payeeCodeChange()" :disabled="IsDisabled">
								<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收款单位名称">
							<el-input v-model="addpaymentrequestform.payeeName" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="开户银行">
							<el-input v-model="addpaymentrequestform.bankName" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="银行账号">
							<el-input v-model="addpaymentrequestform.bankAccount" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="addpaymentrequestform.ourCompany" style="width: 300px"
								:disabled="IsDisabled">
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="货币代码">
							<el-select v-model="addpaymentrequestform.currencyCode" style="width: 300px"
								:disabled="IsDisabled">
								<el-option v-for="dict in optionss.hr_currency_code" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="申请总额">
							<el-input v-model="addpaymentrequestform.totalAmount" style="width: 300px"
								:disabled="IsDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付金额">
							<el-input v-model="addpaymentrequestform.paidAmount" style="width: 300px"
								:disabled="IsDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="未付金额">
							<el-input v-model="addpaymentrequestform.unpaidAmount" style="width: 300px"
								:disabled="IsDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="申请人">
							<el-select v-model="addpaymentrequestform.applicant" style="width: 300px"
								:disabled="IsDisabled">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" :disabled="IsDisabled" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="申请部门">
							<el-select v-model="addpaymentrequestform.applicationDepartment" style="width: 300px"
								:disabled="IsDisabled">
								<el-option v-for="dict in optionss.sql_hr_dept" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="经手人">
							<el-select v-model="addpaymentrequestform.handler" style="width: 300px"
								:disabled="IsDisabled">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="26">
						<el-form-item label="备注说明">
							<el-input type="textarea" v-model="addpaymentrequestform.remarks"
								:autosize="{ minRows: 5, maxRows: 10 }" placeholder="输入备注内容" style="width: 743px"
								:disabled="IsDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">费用信息</span>
			<el-divider></el-divider>
			<el-button class="mt-4" type="primary" @click="handleAddRowCostDetails" style="margin-bottom: 10px;"
				:disabled="IsDisabled">添加费用明细</el-button>
			<el-tabs v-model="activeTab" tab-position="top" style="height: 350px; " class="demo-tabs">
				<el-tab-pane label="费用明细" name="CostDetailsTab">
					<el-table :data="CostDetailsTbaleData" style="width: 100%" height="280">
						<el-table-column prop="relatedmodules" label="关联模块" width="150">
							<template #default="{ row }">
								<el-select v-model="row.relatedmodules" placeholder="选择关联模块" size="large"
									@change="relatedmoduleshandleChange(row)" style="width: 130px;"
									:disabled="IsDisabled">
									<el-option v-for="dict in optionss.hr_associated_modules" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="associatedordernumber" label="关联单号" width="150">
							<template #default="{ row }">
								<el-select v-model="row.associatedordernumber" placeholder="选择关联单号" size="large"
									style="width: 130px;" :disabled="IsDisabled">
									<el-option v-for="dict in row.AssociatedOrderNumberOptions" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="applicationamount" label="申请金额" width="150">
							<template #default="{ row }">
								<el-input v-model="row.applicationamount" placeholder="输入申请金额" size="large"
									style="width: 130px" :disabled="IsDisabled"
									@input="CalculatetotalAmount(row)"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="relevantdates" label="关联日期" width="150">
							<template #default="{ row }">
								<el-date-picker v-model="row.relevantdates" type="date" size="large"
									style="width: 130px" :disabled="IsDisabled"></el-date-picker>
							</template>
						</el-table-column>
						<el-table-column prop="specificpaymentitems" label="具体款项" width="150">
							<template #default="{ row }">
								<el-select v-model="row.specificpaymentitems" placeholder="选择具体款项" size="large"
									:disabled="IsDisabled">
									<el-option v-for="dict in PaymentTypeOptions" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" style="width: 130px;" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" width="150">
							<template #default="{ row }"> <el-input v-model="row.remark" placeholder="输入备注内容"
									size="large" style="width: 130px" :disabled="IsDisabled"></el-input></template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="large"
									@click="CostDetailsTbaleDatahandleDelete(scope.$index)"
									:disabled="IsDisabled">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="未支付款项详情" name="UnpaidDetailsTab">
					<el-table :data="UnpaidDetailsTbaleData" style="width: 100%" height="280">
						<el-table-column prop="contractofpurchaseNo" label="采购合同" width="120"></el-table-column>
						<el-table-column prop="contractdate" label="合同日期" width="120"></el-table-column>
						<el-table-column prop="relatedmodules" label="关联模块" width="120"></el-table-column>
						<el-table-column prop="exportcurrency" label="外销币种" width="120"></el-table-column>
						<el-table-column prop="exchangerate" label="汇率" width="120"></el-table-column>
						<el-table-column prop="amountspayable" label="应支付金额" width="120"></el-table-column>
						<el-table-column prop="depositpaid" label="已付定金" width="120"></el-table-column>
						<el-table-column prop="paymentrequested" label="已申请付款" width="120"></el-table-column>
						<el-table-column prop="nopaymentrequested" label="未申请付款" width="120"></el-table-column>
						<el-table-column prop="paymentpaid" label="已付货款" width="120"></el-table-column>
						<el-table-column prop="unpaiditems" label="未付货款" width="120"></el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="客诉索赔" name="CustomerComplaintsTab"></el-tab-pane>
			</el-tabs>
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
					<el-button type="success" v-show="showSubmitReviewBtn" @click="submitForReview">
						提交
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
const CostDetailsTbaleDatahandleDelete = (index: number) => {
	CostDetailsTbaleData.value.splice(index, 1)
	CalculatetotalAmount() // 重新计算总金额
}
const handleAddRowCostDetails = () => {
	activeTab.value = 'CostDetailsTab'

	// 获取当前日期
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	const todayStr = `${year}-${month}-${day}`;

	CostDetailsTbaleData.value.push({
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
		specificpaymentitems: '',
		remark: ''
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
		hr_business_expenses: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_ourcompany' }, { dictType: 'hr_export_currency' }, { dictType: 'hr_bank' }, { dictType: 'sql_customercollections_no' },
{ dictType: 'hr_payment_category' }, { dictType: 'hr_factory_payment' }, { dictType: 'hr_domestic_charges' }, { dictType: 'hr_foreign_charges' },
{ dictType: 'hr_daily_expenses' }, { dictType: 'sql_supplier_info' }, { dictType: 'hr_currency_code' }, { dictType: 'sql_all_user' }, { dictType: 'sql_hr_sale' },
{ dictType: 'sql_hr_purchase' }, { dictType: 'sql_hr_finance' }, { dictType: 'sql_hr_dept' }, { dictType: 'hr_associated_modules' }, { dictType: 'sql_purchase_contract' },
{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_payment_requests' }, { dictType: 'hr_payment_contract_type' }, { dictType: 'sql_shippingdeliveries' }, { dictType: 'hr_business_expenses' }]

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
const paymentCategoryChange = () => {
	addpaymentrequestform.value.paymentName = '';
	switch (addpaymentrequestform.value.paymentCategory) {
		case '1':
			PaymentTypeOptions.value = state.optionss.hr_factory_payment
			break;
		case '2':
			PaymentTypeOptions.value = state.optionss.hr_domestic_charges
			break;
		case '3':
			PaymentTypeOptions.value = state.optionss.hr_foreign_charges
			break;
		case '4':
			PaymentTypeOptions.value = state.optionss.hr_daily_expenses
			break;
		case '5':
			PaymentTypeOptions.value = state.optionss.hr_business_expenses
			break;
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
const payeeCodeChange = () => {
	return new Promise((resolve, reject) => {
		request({
			url: 'Supplierinfo/GetSupplierInfoByID/GetSupplierInfo',
			method: 'GET',
			params: {
				SupplierID: addpaymentrequestform.value.payeeCode
			}
		}).then(response => {
			if (response.code == 200) {
				// 供应商简称
				addpaymentrequestform.value.payeeName = response.data.fullName;
				addpaymentrequestform.value.bankName = response.data.bankName;
				addpaymentrequestform.value.bankAccount = response.data.bankAccount;
				resolve(response);  // Resolve the promise with the response data
			} else {
				ElMessage({
					message: '未找到对应的厂商信息',
					type: 'error'
				});	// 提示错误信息
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
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
	Remarks: '',
	IsDelete: 0,
	PaymentRequestDetails: []
});

const SavePaymentRequest = () => {
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
		paymentRequestRequest.PayeeName = addpaymentrequestform.value.payeeName;
		paymentRequestRequest.BankName = addpaymentrequestform.value.bankName;
		paymentRequestRequest.BankAccount = addpaymentrequestform.value.bankAccount;
		paymentRequestRequest.OurCompany = Number(addpaymentrequestform.value.ourCompany);
		paymentRequestRequest.CurrencyCode = Number(addpaymentrequestform.value.currencyCode);
		paymentRequestRequest.TotalAmount = Number(addpaymentrequestform.value.totalAmount);
		paymentRequestRequest.PaidAmount = Number(addpaymentrequestform.value.paidAmount);
		paymentRequestRequest.UnpaidAmount = Number(addpaymentrequestform.value.unpaidAmount);
		paymentRequestRequest.Applicant = Number(addpaymentrequestform.value.applicant);
		paymentRequestRequest.ApplicationDepartment = Number(addpaymentrequestform.value.applicationDepartment);
		paymentRequestRequest.FinancialApproval = Number(addpaymentrequestform.value.financialApproval);
		paymentRequestRequest.Handler = Number(addpaymentrequestform.value.handler);
		paymentRequestRequest.Remarks = addpaymentrequestform.value.remarks;
		paymentRequestRequest.IsDelete = 0;
		paymentRequestRequest.PaymentRequestDetails = CostDetailsTbaleData.value;
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
		paymentCategoryChange();
		addpaymentrequestform.value.paymentName = response.data.paymentRequest.paymentName.toString();
		addpaymentrequestform.value.payeeCode = response.data.paymentRequest.payeeCode.toString();
		addpaymentrequestform.value.payeeName = response.data.paymentRequest.payeeName;
		addpaymentrequestform.value.bankName = response.data.paymentRequest.bankName;
		addpaymentrequestform.value.bankAccount = response.data.paymentRequest.bankAccount;
		addpaymentrequestform.value.ourCompany = response.data.paymentRequest.ourCompany.toString();
		addpaymentrequestform.value.currencyCode = response.data.paymentRequest.currencyCode.toString();
		addpaymentrequestform.value.totalAmount = response.data.paymentRequest.totalAmount;
		addpaymentrequestform.value.paidAmount = response.data.paymentRequest.paidAmount;
		addpaymentrequestform.value.unpaidAmount = response.data.paymentRequest.unpaidAmount;
		addpaymentrequestform.value.applicant = response.data.paymentRequest.applicant.toString();
		addpaymentrequestform.value.applicationDepartment = response.data.paymentRequest.applicationDepartment.toString();
		addpaymentrequestform.value.financialApproval = response.data.paymentRequest.financialApproval.toString();
		addpaymentrequestform.value.handler = response.data.paymentRequest.handler.toString();
		addpaymentrequestform.value.remarks = response.data.remarks;
		response.data.paymentRequestDetails.forEach((element) => {
			handleAddRowCostDetails();
		});
		CostDetailsTbaleData.value.forEach((element, index) => {
			element.id = response.data.paymentRequestDetails[index].id;
			element.relatedmodules = response.data.paymentRequestDetails[index].relatedModules.toString();
			relatedmoduleshandleChange(element);
			element.associatedordernumber = response.data.paymentRequestDetails[index].associatedOrderNumber.toString();
			element.applicationamount = response.data.paymentRequestDetails[index].applicationAmount;
			element.relevantdates = response.data.paymentRequestDetails[index].relevantDates;
			element.specificpaymentitems = response.data.paymentRequestDetails[index].specificPaymentItems.toString();
			element.remark = response.data.paymentRequestDetails[index].remark;
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
		paymentRequestRequest.PayeeName = addpaymentrequestform.value.payeeName;
		paymentRequestRequest.BankName = addpaymentrequestform.value.bankName;
		paymentRequestRequest.BankAccount = addpaymentrequestform.value.bankAccount;
		paymentRequestRequest.OurCompany = Number(addpaymentrequestform.value.ourCompany);
		paymentRequestRequest.CurrencyCode = Number(addpaymentrequestform.value.currencyCode);
		paymentRequestRequest.TotalAmount = Number(addpaymentrequestform.value.totalAmount);
		paymentRequestRequest.PaidAmount = Number(addpaymentrequestform.value.paidAmount);
		paymentRequestRequest.UnpaidAmount = Number(addpaymentrequestform.value.unpaidAmount);
		paymentRequestRequest.Applicant = Number(addpaymentrequestform.value.applicant);
		paymentRequestRequest.ApplicationDepartment = Number(addpaymentrequestform.value.applicationDepartment);
		paymentRequestRequest.FinancialApproval = Number(addpaymentrequestform.value.financialApproval);
		paymentRequestRequest.Handler = Number(addpaymentrequestform.value.handler);
		paymentRequestRequest.Remarks = addpaymentrequestform.value.remarks;
		paymentRequestRequest.IsDelete = 0;
		paymentRequestRequest.PaymentRequestDetails = CostDetailsTbaleData.value;
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
	addpaymentrequestdialog.value = false;
	addpaymentrequestform.value.applicant = '';
	addpaymentrequestform.value.applicationDate = '';
	addpaymentrequestform.value.applicationDepartment = '';
	addpaymentrequestform.value.applicationNumber = '';
	addpaymentrequestform.value.bankAccount = '';
	addpaymentrequestform.value.bankName = '';
	addpaymentrequestform.value.currencyCode = '';
	addpaymentrequestform.value.financialApproval = '';
	addpaymentrequestform.value.handler = '';
	addpaymentrequestform.value.ourCompany = '';
	addpaymentrequestform.value.paidAmount = '';
	addpaymentrequestform.value.paymentCategory = '';
	addpaymentrequestform.value.paymentName = '';
	addpaymentrequestform.value.payeeCode = '';
	addpaymentrequestform.value.payeeName = '';
	addpaymentrequestform.value.remarks = '';
	addpaymentrequestform.value.totalAmount = '';
	addpaymentrequestform.value.unpaidAmount = '';
	CostDetailsTbaleData.value = [];
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
const EditPayment = () => {
	showEditBtn.value = false;
	showSubmitReviewBtn.value = true;
	IsDisabled.value = false;
	isEditSaveBtnShow.value = true;
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
	paymentFormRef.value.validate((valid) => {
		if (valid) {
			ElMessageBox.confirm('确定提交审核吗?', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				// 构造请求数据（与保存草稿的数据结构保持一致）
				const requestData = {
					id: PaymentRequestID.value || 0, // 新增时为0，更新时为实际ID
					ApplicationNumber: addpaymentrequestform.value.applicationNumber,
					ApplicationDate: addpaymentrequestform.value.applicationDate,
					PaymentCategory: Number(addpaymentrequestform.value.paymentCategory),
					PaymentName: Number(addpaymentrequestform.value.paymentName),
					PayeeCode: Number(addpaymentrequestform.value.payeeCode),
					PayeeName: addpaymentrequestform.value.payeeName,
					BankName: addpaymentrequestform.value.bankName,
					BankAccount: addpaymentrequestform.value.bankAccount,
					OurCompany: Number(addpaymentrequestform.value.ourCompany),
					CurrencyCode: Number(addpaymentrequestform.value.currencyCode),
					TotalAmount: Number(addpaymentrequestform.value.totalAmount),
					PaidAmount: Number(addpaymentrequestform.value.paidAmount),
					UnpaidAmount: Number(addpaymentrequestform.value.unpaidAmount),
					Applicant: Number(addpaymentrequestform.value.applicant),
					ApplicationDepartment: Number(addpaymentrequestform.value.applicationDepartment),
					FinancialApproval: Number(addpaymentrequestform.value.financialApproval),
					Handler: Number(addpaymentrequestform.value.handler),
					Remark: addpaymentrequestform.value.remarks, // 注意字段名映射
					PaymentRequestDetails: CostDetailsTbaleData.value || []
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
};

const CalculatetotalAmount = () => {
	// 初始化总金额为0
	let total = 0;

	// 遍历费用明细表格中的所有行
	CostDetailsTbaleData.value.forEach(row => {
		// 将字符串转换为数字并累加
		// 如果applicationamount为空或非数字，使用0
		const amount = parseFloat(row.applicationamount) || 0;
		total += amount;
	});

	// 更新表单中的总金额字段
	addpaymentrequestform.value.totalAmount = total.toFixed(2);
	// 更新未付金额字段（总金额 - 已付金额）
	const paidAmount = parseFloat(addpaymentrequestform.value.paidAmount) || 0;
	addpaymentrequestform.value.unpaidAmount = (total - paidAmount).toFixed(2);
}

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
