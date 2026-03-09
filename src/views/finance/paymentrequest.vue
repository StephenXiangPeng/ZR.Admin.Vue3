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
							style="width: 100%" clearable>
							<el-option v-for="dict in optionss.sql_payment_requests" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="SearchSupplierID" filterable placeholder="选择收款单位" size="default"
							style="width: 100%" clearable>
							<el-option v-for="supplier in filteredSupplierList" :key="supplier.dictvalue"
								:label="supplier.dictLabel" :value="supplier.dictvalue" />
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
									<el-tag :type="getStatusType(row.reviewStatusStr)"
										:style="{ ...getStatusStyle(row.reviewStatusStr), cursor: 'pointer' }"
										@click="getApprovalFlow(row.id)">
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
							<el-tag :type="getStatusType(row.contractReviewStatusStr)"
								:style="getStatusStyle(row.contractReviewStatusStr)">
								{{ row.contractReviewStatusStr }}
							</el-tag>
						</template>
					</template>
				</el-table-column>
				<el-table-column prop="paymentCategory" label="付款类别" width="100"></el-table-column>
				<el-table-column prop="paymentName" label="款项名称" width="100"></el-table-column>
				<el-table-column prop="payeeName" label="收款单位名称" width="200"></el-table-column>
				<el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
				<el-table-column prop="bankAccount" label="银行账号" width="200"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="110"></el-table-column>
				<el-table-column prop="currencyCode" label="货币代码" width="90"></el-table-column>
				<el-table-column prop="totalAmount" label="申请总额" width="90"></el-table-column>
				<el-table-column prop="paidAmount" label="已付金额" width="90" v-if="false"></el-table-column>
				<el-table-column prop="unpaidAmount" label="未付金额" width="90" v-if="false"></el-table-column>
				<el-table-column prop="applicant" label="申请人" width="120"></el-table-column>
				<el-table-column prop="applicationDepartment" label="申请部门" width="110"></el-table-column>
				<el-table-column prop="handler" label="经手人" width="150" v-if="false"></el-table-column>
				<el-table-column prop="applicationDate" label="申请日期" width="110"></el-table-column>
				<el-table-column fixed="right" label="操作" width="150">
					<template #default="scope">
						<el-button type="text" size="small" @click="CheckPaymentRequest(scope.row)">查看/编辑</el-button>
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
										size="default" clearable>
										<el-option v-for="dict in optionss.hr_payment_category" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="款项名称">
									<el-select v-model="addpaymentrequestform.paymentName" style="width: 300px"
										placeholder="请选择款项名称" :disabled="IsDisabled" size="default" clearable
										@change="paymentNameChange">
										<el-option v-for="dict in PaymentTypeOptions" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="收款单位" placeholder="请选择收款单位">
									<!-- 日常费用且为市场开发、产品开发、其它费用时，或客户事宜时，使用输入框 -->
									<el-input
										v-if="isDailyExpenseWithManualInput() || isCustomerMatterWithManualInput()"
										v-model="addpaymentrequestform.payeeName" style="width: 300px"
										:disabled="IsDisabled" size="default" placeholder="请输入收款单位名称"
										@input="handleManualPayeeInput" />
									<!-- 其他情况使用下拉选择 -->
									<el-select v-else filterable allow-create v-model="addpaymentrequestform.payeeCode"
										style="width: 300px" @change="payeeCodeChange()" :disabled="IsDisabled"
										size="default" clearable>
										<el-option v-for="supplier in filteredSupplierList" :key="supplier.dictvalue"
											:label="supplier.dictLabel" :value="supplier.dictvalue" />
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
									<!-- 需要手动输入银行信息时，使用可编辑输入框 -->
									<el-input
										v-if="isDailyExpenseWithManualInput() || isManualBankInput() || isCustomerMatterWithManualInput()"
										v-model="addpaymentrequestform.bankName" style="width: 300px"
										:disabled="IsDisabled" size="default" placeholder="请输入开户银行" />
									<!-- 其他情况使用只读输入框 -->
									<el-input v-else v-model="addpaymentrequestform.bankName" style="width: 300px"
										disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="银行账号">
									<!-- 需要手动输入银行信息时，使用输入框 -->
									<el-input
										v-if="isDailyExpenseWithManualInput() || isManualBankInput() || isCustomerMatterWithManualInput()"
										v-model="addpaymentrequestform.bankAccount" style="width: 300px"
										:disabled="IsDisabled" size="default" placeholder="请输入银行账号" />
									<!-- 其他情况使用下拉选择 -->
									<el-select v-else v-model="addpaymentrequestform.bankAccount" style="width: 300px"
										placeholder="请选择银行账号" :disabled="IsDisabled" @change="bankAccountChange"
										filterable clearable size="default">
										<el-option v-for="account in supplierBankAccounts" :key="account.id"
											:label="`${account.bank_account_number || account.bankAccountNumber}`"
											:value="account.bank_account_number || account.bankAccountNumber" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="我方公司">
									<el-select v-model="addpaymentrequestform.ourCompany" style="width: 300px"
										:disabled="IsDisabled" size="default" clearable>
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
										:disabled="IsDisabled" size="default" clearable>
										<el-option v-for="dict in optionss.hr_currency_code" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="申请金额">
									<el-input v-model="addpaymentrequestform.totalAmount" style="width: 300px"
										:disabled="IsDisabled || addpaymentrequestform.paymentCategory === '1' || addpaymentrequestform.paymentCategory === '5'"
										size="default" clearable></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6" v-if="false">
								<el-form-item label="已付金额">
									<el-input v-model="addpaymentrequestform.paidAmount" style="width: 300px"
										:disabled="IsDisabled" size="default" clearable></el-input>
								</el-form-item>
							</el-col>
							<!-- 关联合同列 - 仅在业务费用且为其它款项名称时显示 -->
							<el-col :span="6" v-if="isBusinessExpenseWithContract()">
								<el-form-item label="关联合同">
									<el-select v-model="addpaymentrequestform.relatedContract" style="width: 300px"
										:disabled="IsDisabled" size="default" clearable placeholder="请选择关联合同">
										<el-option v-for="contract in contractList" :key="contract.dictvalue"
											:label="contract.dictLabel" :value="contract.dictvalue" />
									</el-select>
								</el-form-item>
							</el-col>
							<!-- 相关客户列 - 仅在日常费用且为客户事宜时显示 -->
							<el-col :span="6" v-if="isDailyExpenseWithCustomerOptions()">
								<el-form-item label="相关客户">
									<el-select v-model="addpaymentrequestform.relatedCustomer" style="width: 300px"
										:disabled="IsDisabled" size="default" clearable placeholder="请选择相关客户">
										<el-option v-for="customer in customerOptions" :key="customer.dictvalue"
											:label="customer.dictLabel" :value="customer.dictvalue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="申请人">
									<el-select v-model="addpaymentrequestform.applicant" style="width: 300px"
										:disabled="IsDisabled" size="default" clearable>
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
										:disabled="IsDisabled" size="default" clearable></el-input>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="selectedSampleCollectionCollapseActive" style="margin-bottom: 20px;"
				v-show="showSelectedSampleCollection">
				<el-collapse-item title="已选择收寄样单据" name="selectedSampleCollection">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">已选择收寄样单据</span>
					</template>
					<!-- 筛选条件区域 -->
					<div style="margin-bottom: 10px; padding: 10px; background: #f5f7fa; border-radius: 4px;">
						<el-row :gutter="15">
							<el-col :span="6">
								<el-select v-model="filterSelectedCustomerSupplier" filterable placeholder="选择客户/供应商"
									size="default" style="width: 100%" clearable>
									<el-option label="客户" value="客户" />
									<el-option label="供应商" value="供应商" />
								</el-select>
							</el-col>
						</el-row>
					</div>
					<el-table :data="paginatedSelectedSampleCollectionData" style="width: 100%;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="type" label="寄样/收样" width="100">
							<template #default="{ row }">
								<span>{{ row.type || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_or_Supplier" label="客户/供应商" width="110">
							<template #default="{ row }">
								<span>{{ row.customer_or_Supplier || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_ID" label="寄样对象" width="150">
							<template #default="{ row }">
								<span>{{ row.customer_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="waybill_Number" label="运单号" width="130">
							<template #default="{ row }">
								<span>{{ row.waybill_Number || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="express_Company" label="快件公司" width="150">
							<template #default="{ row }">
								<span>{{ row.express_Company || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="sample_Date" label="登记日期" width="110">
							<template #default="{ row }">
								<span>{{ row.sample_Date || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="payment_Method" label="付费方式" width="90">
							<template #default="{ row }">
								<span>{{ row.payment_Method || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="company_ID" label="我方公司" width="130">
							<template #default="{ row }">
								<span>{{ row.company_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="paid_Express_Fee" label="快件费" width="110">
							<template #default="{ row }">
								<el-input v-model="row.paid_Express_Fee" :disabled="IsDisabled" size="small"
									@blur="handlePaidExpressFeeChange(row)"
									@input="(value) => handlePaidExpressFeeInput(row, value)" placeholder="0.00"
									style="width: 100%" />
							</template>
						</el-table-column>
						<el-table-column prop="relatedContractNumber" label="销售合同" width="150">
							<template #default="{ row }">
								<el-select v-model="row.relatedContractNumber"
									:disabled="IsDisabled || row.hasExistingContract === true" filterable clearable
									placeholder="请选择销售合同" size="small" style="width: 100%"
									@change="(value) => handleRelatedContractNumberChange(row, value)">
									<el-option v-for="contract in applicantSaleContracts" :key="contract.dictvalue"
										:label="contract.dictLabel" :value="contract.dictLabel" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="relatedShippingNumber" label="出运编号" width="150">
							<template #default="{ row }">
								<el-select v-model="row.relatedShippingNumber"
									:disabled="IsDisabled || row.hasExistingShipping === true" filterable clearable
									placeholder="请选择出运编号" size="small" style="width: 100%">
									<el-option v-for="shipping in (row.shippingContractOptions || [])"
										:key="shipping.dictvalue" :label="shipping.dictLabel"
										:value="shipping.dictLabel" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="100">
							<template #default="{ row }">
								<el-button type="text" size="large" @click="handleRemoveSelectedSampleCollection(row)"
									:disabled="IsDisabled">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination @current-change="selectedSampleCollectionHandlePageChange"
						:current-page="selectedSampleCollectionCurrentPage"
						:page-size="selectedSampleCollectionPageSize" :total="selectedSampleCollectionTotalItems"
						background layout="prev, pager, next" style="margin-top: 5px;" />
				</el-collapse-item>
			</el-collapse>
			<el-collapse v-model="sampleCollectionCollapseActive" style="margin-bottom: 20px;"
				v-show="showSampleCollection">
				<el-collapse-item title="待支付款项收寄样单据" name="sampleCollection">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">待支付款项收寄样单据</span>
					</template>
					<!-- 筛选条件区域 -->
					<div style="margin-bottom: 10px; padding: 10px; background: #f5f7fa; border-radius: 4px;">
						<el-row :gutter="15">
							<el-col :span="6">
								<el-select v-model="filterSampleCustomerSupplier" filterable placeholder="选择客户/供应商"
									size="default" style="width: 100%" clearable>
									<el-option label="客户" value="客户" />
									<el-option label="供应商" value="供应商" />
								</el-select>
							</el-col>
						</el-row>
					</div>
					<el-table :data="paginatedSampleCollectionData" style="width: 100%;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="type" label="寄样/收样" width="100">
							<template #default="{ row }">
								<span>{{ row.type || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_or_Supplier" label="客户/供应商" width="110">
							<template #default="{ row }">
								<span>{{ row.customer_or_Supplier || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="customer_ID" label="寄样对象" width="150">
							<template #default="{ row }">
								<span>{{ row.customer_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="waybill_Number" label="运单号" width="130">
							<template #default="{ row }">
								<span>{{ row.waybill_Number || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="express_Company" label="快件公司" width="150">
							<template #default="{ row }">
								<span>{{ row.express_Company || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="sample_Date" label="登记日期" width="110">
							<template #default="{ row }">
								<span>{{ row.sample_Date || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="payment_Method" label="付费方式" width="90">
							<template #default="{ row }">
								<span>{{ row.payment_Method || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="company_ID" label="我方公司" width="130">
							<template #default="{ row }">
								<span>{{ row.company_ID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="paid_Express_Fee" label="快件费" width="110">
							<template #default="{ row }">
								<span>{{ row.paid_Express_Fee || '0.00' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedContractID" label="销售合同" width="130">
							<template #default="{ row }">
								<span>{{ row.relatedContractID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedShippingContractsID" label="出运编号" width="130">
							<template #default="{ row }">
								<span>{{ row.relatedShippingContractsID || '' }}</span>
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="100">
							<template #default="{ row }">
								<el-button type="primary" size="small" @click="handleSelectSampleCollection(row)"
									:disabled="IsDisabled">
									选择
								</el-button>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination @current-change="sampleCollectionHandlePageChange"
						:current-page="sampleCollectionCurrentPage" :page-size="sampleCollectionPageSize"
						:total="sampleCollectionTotalItems" background layout="prev, pager, next"
						style="margin-top: 5px;" />
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
					<el-table :data="CostDetailsTbaleData" style="width: 100%;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="expenseName" label="费用名称" min-width="150">
							<template #default="{ row }">
								<span>{{ row.expenseName || '未设置费用名称' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedDocumentTypeName" label="单据类型" min-width="120">
							<template #default="{ row }">
								<span>{{ row.relatedDocumentTypeName || '未设置单据类型' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedDocumentsNo" label="单据号" min-width="150">
							<template #default="{ row }">
								<span>{{ row.relatedDocumentsNo || '未设置单据号' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="amount" label="金额" min-width="120">
							<template #default="{ row }">
								<span>{{ row.amount || row.totalGoodsValue || '0.00' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="appliedAmount" label="已申请金额" min-width="120">
							<template #default="{ row }">
								<span>{{ row.appliedAmount || '0.00' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="currentPaymentAmount" label="本次申请金额" min-width="140">
							<template #default="{ row }">
								<el-input v-model="row.currentPaymentAmount" :disabled="IsDisabled" size="small"
									placeholder="0.00" style="width: 100%"
									:class="{ 'error-input': isPaymentAmountExceeded(row) }"
									@blur="handleCurrentPaymentAmountInput(row)" />
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
					<el-table :data="UnpaidDetailsTbaleData" style="width: 100%;" stripe
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
						<el-table-column prop="expenseName" label="费用名称" min-width="150">
							<template #default="{ row }">
								<span>{{ row.expenseName || '未设置费用名称' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedDocumentTypeName" label="单据类型" min-width="120">
							<template #default="{ row }">
								<span>{{ row.relatedDocumentTypeName || '未设置单据类型' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="relatedDocumentsNo" label="单据号" min-width="150">
							<template #default="{ row }">
								<span>{{ row.relatedDocumentsNo || '未设置单据号' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="amount" label="金额" min-width="120">
							<template #default="{ row }">
								<span>{{ row.amount || '0.00' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="appliedAmount" label="已申请金额" min-width="120">
							<template #default="{ row }">
								<span>{{ row.appliedAmount || '0.00' }}</span>
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
						提交 ({{ addpaymentrequestform.paymentCategory === '1' ? '金额不匹配或超过金额' : '金额不匹配' }})
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
import { createApp, getCurrentInstance, reactive, toRefs, ref, computed, watch } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { number } from 'echarts';
import { getSupplierList, getContractListByCustomerId } from '@/api/supplier';
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
		case '已付款': return '' // 使用自定义样式，不设置type
		case '已归帐': return 'info'
		default: return 'info'
	}
}

// 获取状态标签的自定义颜色样式
const getStatusStyle = (status: string) => {
	switch (status) {
		case '已付款':
			return {
				color: '#13CE66',
				backgroundColor: '#E8F8F5',
				borderColor: '#A8E6CF'
			} // 青绿色系，与已批准区分
		case '已归帐':
			return {
				color: '#606266',
				backgroundColor: '#F4F4F5',
				borderColor: '#D3D4D6'
			} // 深灰色系
		default:
			return {} // 其他状态使用默认type样式
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
		ApproveDocumentRequest.ApproverID = userId;

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
const filteredSupplierList = ref([])

const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);
const activeTab = ref('CostDetailsTab')//费用信息Tab
const addpaymentrequestdialog = ref(false)//付款申请Dialog
const basicInfoCollapseActive = ref(['basicInfo']);//基本信息折叠面板
const paymentDetailsCollapseActive = ref(['paymentDetails']);//付款明细折叠面板
const unpaidDetailsCollapseActive = ref(['unpaidDetails']);//未支付款项详情折叠面板
const sampleCollectionCollapseActive = ref(['sampleCollection']);//收寄样列表折叠面板
const selectedSampleCollectionCollapseActive = ref(['selectedSampleCollection']);//已选择收寄样列表折叠面板
// 控制付款明细和未支付款项详情的显示
const showPaymentDetails = ref(true);
// 控制收寄样列表的显示
const showSampleCollection = ref(false);
// 控制已选择收寄样列表的显示
const showSelectedSampleCollection = ref(false);

// 筛选条件变量
const filterSampleCustomerSupplier = ref(''); // 待支付列表的客户/供应商筛选
const filterSampleOurCompany = ref(''); // 待支付列表的我方公司筛选
const filterSelectedCustomerSupplier = ref(''); // 已选择列表的客户/供应商筛选
const filterSelectedOurCompany = ref(''); // 已选择列表的我方公司筛选

// 是否为预付款（款项名称 dictValue == 1）
const isDepositType = computed(() => Number(addpaymentrequestform.value.paymentName) === 1);
const paymentrequesttableData = ref([])//付款申请列表Table
const CostDetailsTbaleData = ref([])//费用明细Table
const UnpaidDetailsTbaleData = ref([])//未支付款项详情Table
const sampleCollectionTableData = ref([])//收寄样列表Table（全部数据）
const selectedSampleCollectionTableData = ref([])//已选择收寄样列表Table（全部数据）

// 待支付款项收寄样单据分页相关
const sampleCollectionCurrentPage = ref(1);
const sampleCollectionPageSize = ref(10);
const sampleCollectionTotalItems = ref(0);

// 已选择收寄样单据分页相关
const selectedSampleCollectionCurrentPage = ref(1);
const selectedSampleCollectionPageSize = ref(10);
const selectedSampleCollectionTotalItems = ref(0);

// 计算属性：待支付款项收寄样单据筛选后的数据
const filteredSampleCollectionData = computed(() => {
	let filtered = sampleCollectionTableData.value;

	// 根据客户/供应商筛选
	if (filterSampleCustomerSupplier.value) {
		filtered = filtered.filter(item => item.customer_or_Supplier === filterSampleCustomerSupplier.value);
	}

	// 根据我方公司筛选
	if (filterSampleOurCompany.value) {
		filtered = filtered.filter(item => item.company_ID === filterSampleOurCompany.value);
	}

	return filtered;
});

// 计算属性：待支付款项收寄样单据分页后的数据
const paginatedSampleCollectionData = computed(() => {
	const start = (sampleCollectionCurrentPage.value - 1) * sampleCollectionPageSize.value;
	const end = start + sampleCollectionPageSize.value;
	return filteredSampleCollectionData.value.slice(start, end);
});

// 计算属性：已选择收寄样单据筛选后的数据
const filteredSelectedSampleCollectionData = computed(() => {
	let filtered = selectedSampleCollectionTableData.value;

	// 根据客户/供应商筛选
	if (filterSelectedCustomerSupplier.value) {
		filtered = filtered.filter(item => item.customer_or_Supplier === filterSelectedCustomerSupplier.value);
	}

	// 根据我方公司筛选
	if (filterSelectedOurCompany.value) {
		filtered = filtered.filter(item => item.company_ID === filterSelectedOurCompany.value);
	}

	return filtered;
});

// 计算属性：已选择收寄样单据分页后的数据
const paginatedSelectedSampleCollectionData = computed(() => {
	const start = (selectedSampleCollectionCurrentPage.value - 1) * selectedSampleCollectionPageSize.value;
	const end = start + selectedSampleCollectionPageSize.value;
	return filteredSelectedSampleCollectionData.value.slice(start, end);
});
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
	remarks: '',
	relatedContract: '', // 关联合同字段
	relatedCustomer: '' // 相关客户字段
})

// 供应商银行账号列表
const supplierBankAccounts = ref([])
// 关联合同列表
const contractList = ref([])
// 客户选项列表
const customerOptions = ref([])
// 与申请人关联的销售合同列表（用于关联合同号下拉框）
const applicantSaleContracts = ref([])
// 与申请人关联的出运合同列表（用于关联运编号下拉框）
const applicantShippingContracts = ref([])
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
		totalGoodsValue: 0,
		actualShippingAmount: 0,
		deposit: '',
		paidAmount: '',
		unpaidAmount: '',
		appliedAmount: 0, // 已申请金额字段
		currentPaymentAmount: '', // 新增本次付款金额字段
		unpaidAmountID: 0, // 未支付款项ID
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
		hr_contract_status: [],
		sql_hr_customer_abbreviation: [],
		hr_express_delivery_company: [],
		hr_express_payment_method: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_ourcompany' }, { dictType: 'hr_export_currency' }, { dictType: 'hr_bank' }, { dictType: 'sql_customercollections_no' },
{ dictType: 'hr_payment_category' }, { dictType: 'hr_factory_payment' }, { dictType: 'hr_domestic_charges' }, { dictType: 'hr_foreign_charges' },
{ dictType: 'hr_daily_expenses' }, { dictType: 'sql_supplier_info' }, { dictType: 'hr_currency_code' }, { dictType: 'sql_all_user' }, { dictType: 'sql_hr_sale' },
{ dictType: 'sql_hr_purchase' }, { dictType: 'sql_hr_finance' }, { dictType: 'sql_hr_dept' }, { dictType: 'hr_associated_modules' }, { dictType: 'sql_purchase_contract' },
{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_payment_requests' }, { dictType: 'hr_payment_contract_type' }, { dictType: 'sql_shippingdeliveries' }, { dictType: 'hr_business_expenses' }, { dictType: 'hr_contract_status' },
{ dictType: 'sql_hr_customer_abbreviation' }, { dictType: 'hr_express_payment_method' }]

// 统一的API请求处理工具函数
const ApiRequestHandler = {
	// 标准化数据结构
	standardizeData(data) {
		return data.map(item => ({
			dictvalue: item.dictvalue || item.dictValue || item.value,
			dictLabel: item.dictLabel || item.label || item.name
		}));
	},

	// 处理API响应
	handleResponse(response, successCallback, errorCallback) {
		if (response && response.code === 200) {
			const data = response.data || [];
			if (successCallback) {
				successCallback(data);
			}
		} else {
			console.error('API请求失败:', response?.msg);
			if (errorCallback) {
				errorCallback(response?.msg);
			}
		}
	},

	// 处理API错误
	handleError(error, errorCallback) {
		console.error('API请求出错:', error);
		if (errorCallback) {
			errorCallback(error);
		}
	}
};

// 加载过滤后的供应商列表
async function loadFilteredSuppliers() {
	try {
		const response = await getSupplierList();
		ApiRequestHandler.handleResponse(
			response,
			(data) => {
				filteredSupplierList.value = ApiRequestHandler.standardizeData(data);
			},
			() => {
				filteredSupplierList.value = [];
			}
		);
	} catch (error) {
		ApiRequestHandler.handleError(error, () => {
			filteredSupplierList.value = [];
		});
	}
}

// 加载合同列表
async function loadContractList(customerId) {
	try {
		const response = await getContractListByCustomerId(customerId);
		ApiRequestHandler.handleResponse(
			response,
			(data) => {
				contractList.value = ApiRequestHandler.standardizeData(data);
			},
			() => {
				contractList.value = [];
			}
		);
	} catch (error) {
		ApiRequestHandler.handleError(error, () => {
			contractList.value = [];
		});
	}
}

// 加载与申请人关联的销售合同列表
const loadApplicantSaleContracts = async (applicantId) => {
	if (!applicantId) {
		applicantSaleContracts.value = [];
		return;
	}
	try {
		const response = await request({
			url: 'Contracts/GetContractListByUser/GetContractList',
			method: 'GET'
		});
		if (response && response.code === 200) {
			applicantSaleContracts.value = (response.data || []).map(item => ({
				dictvalue: String(item.id || item.Id || item.ID),
				dictLabel: item.contractNumber || item.ContractNumber || item.contract_Number
			}));
		} else {
			applicantSaleContracts.value = [];
		}
	} catch (error) {
		console.error('获取销售合同列表失败:', error);
		applicantSaleContracts.value = [];
	}
};

// 加载与申请人关联的出运合同列表
const loadApplicantShippingContracts = async (applicantId) => {
	if (!applicantId) {
		applicantShippingContracts.value = [];
		return;
	}
	try {
		const response = await request({
			url: 'ShippingDeliveries/GetShippingContractSelectList/GetSelectList',
			method: 'GET',
			params: {
				SalespersonID: parseInt(applicantId)
			}
		});
		if (response && response.code === 200) {
			applicantShippingContracts.value = (response.data || []).map(item => ({
				dictvalue: String(item.dictValue || item.id || item.Id || item.ID),
				dictLabel: item.dictLabel || item.invoiceNumber || item.InvoiceNumber || item.shippingNumber || item.shipping_Number
			}));
		} else {
			applicantShippingContracts.value = [];
		}
	} catch (error) {
		console.error('获取出运编号列表失败:', error);
		applicantShippingContracts.value = [];
	}
};

// 加载快递公司下拉选项
const loadLogisticsCompanySelect = async () => {
	try {
		const response = await request({
			url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect',
			method: 'get',
			params: { companyType: 2 }
		});
		if (response.code === 200) {
			state.optionss.hr_express_delivery_company = (response.data || []).map(x => ({
				dictValue: String(x.dictValue),
				dictLabel: x.dictLabel
			}));
		}
	} catch (error) {
		console.error('加载快递公司下拉失败:', error);
	}
};

async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});

		// 加载过滤后的供应商列表
		await loadFilteredSuppliers();

		// 加载快递公司字典
		await loadLogisticsCompanySelect();

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

	// 若付款类别被清空，需同时清空款项名称选项列表并退出
	if (!addpaymentrequestform.value.paymentCategory) {
		PaymentTypeOptions.value = [];
		return;
	}

	// 清空收款单位相关字段
	addpaymentrequestform.value.payeeCode = '';
	addpaymentrequestform.value.payeeName = '';
	addpaymentrequestform.value.bankName = '';
	addpaymentrequestform.value.bankAccount = '';
	supplierBankAccounts.value = [];

	switch (addpaymentrequestform.value.paymentCategory) {
		case '1':
			PaymentTypeOptions.value = state.optionss.hr_factory_payment
			showPaymentDetails.value = true; // 工厂付款显示付款明细和未支付款项详情
			showSampleCollection.value = false; // 隐藏收寄样列表
			// 加载默认供应商选项
			await loadFilteredSuppliers();
			break;
		case '2':
			PaymentTypeOptions.value = state.optionss.hr_domestic_charges
			showPaymentDetails.value = true; // 国内费用显示付款明细和未支付款项详情
			showSampleCollection.value = false; // 隐藏收寄样列表
			// 加载默认供应商选项
			await loadFilteredSuppliers();
			break;
		case '3':
			PaymentTypeOptions.value = state.optionss.hr_foreign_charges
			showPaymentDetails.value = true; // 国外费用显示付款明细和未支付款项详情
			showSampleCollection.value = false; // 隐藏收寄样列表
			// 加载默认供应商选项
			await loadFilteredSuppliers();
			break;
		case '4':
			PaymentTypeOptions.value = state.optionss.hr_daily_expenses
			showPaymentDetails.value = false; // 日常费用隐藏付款明细和未支付款项详情
			showSampleCollection.value = false; // 隐藏收寄样列表
			// 加载默认供应商选项
			await loadFilteredSuppliers();
			break;
		case '5':
			PaymentTypeOptions.value = state.optionss.hr_business_expenses
			showPaymentDetails.value = false; // 业务费用隐藏付款明细和未支付款项详情
			showSampleCollection.value = false; // 默认隐藏收寄样列表，选择快递费后根据收款单位显示
			// 业务费用默认加载供应商选项，等选择款项名称后再动态更新
			// 在查看详情时，不要在这里加载供应商选项，而是在后面根据款项名称加载
			if (!isCheckAndEdit.value) {
				await loadFilteredSuppliers();
			}
			break;
		default:
			showPaymentDetails.value = true;
			showSampleCollection.value = false; // 隐藏收寄样列表
			// 加载默认供应商选项
			await loadFilteredSuppliers();
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
						supplierID: addpaymentrequestform.value.payeeCode,
						paymentType: Number(addpaymentrequestform.value.paymentName) || 0
					}
				});

				if (unpaidResponse.data && unpaidResponse.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
					// 保存原始未支付款项数据到Map，用于恢复时使用
					UnpaidDetailsTbaleData.value.forEach((element) => {
						const key = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
						originalUnpaidItemsMap.value.set(key, { ...element });
					});
					// 新API数据结构不需要处理contractStatus，直接使用返回的数据
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
	// 如果收款单位被清空，清空相关字段
	if (addpaymentrequestform.value.payeeCode === '' || addpaymentrequestform.value.payeeCode === null || addpaymentrequestform.value.payeeCode === undefined) {
		// 清空收款单位名称
		addpaymentrequestform.value.payeeName = '';
		// 清空开户银行
		addpaymentrequestform.value.bankName = '';
		// 清空银行账号
		addpaymentrequestform.value.bankAccount = '';
		// 清空供应商银行账号列表
		supplierBankAccounts.value = [];
		// 清空未支付款项详情
		UnpaidDetailsTbaleData.value = [];
		// 清空原始未支付款项数据Map
		originalUnpaidItemsMap.value.clear();
		// 清空已选择的未付款项
		selectedUnpaidItemIds.value.clear();
		// 清空关联合同
		addpaymentrequestform.value.relatedContract = '';
		contractList.value = [];
		// 清空收寄样列表
		sampleCollectionTableData.value = [];
		showSampleCollection.value = false;
		sampleCollectionTotalItems.value = 0;
		sampleCollectionCurrentPage.value = 1;
		selectedSampleCollectionTableData.value = [];
		showSelectedSampleCollection.value = false;
		selectedSampleCollectionTotalItems.value = 0;
		selectedSampleCollectionCurrentPage.value = 1;
		return;
	}
	try {
		// 1. 取下拉框label作为收款单位名称
		const selectedSupplier = filteredSupplierList.value.find(
			item => item.dictvalue == addpaymentrequestform.value.payeeCode ||
				item.dictvalue == addpaymentrequestform.value.payeeCode.toString() ||
				item.dictvalue == Number(addpaymentrequestform.value.payeeCode)
		);
		addpaymentrequestform.value.payeeName = selectedSupplier ? selectedSupplier.dictLabel : '';

		// 如果需要手动输入银行信息，清空银行信息让用户手动输入
		if (isManualBankInput()) {
			addpaymentrequestform.value.bankName = '';
			addpaymentrequestform.value.bankAccount = '';
			supplierBankAccounts.value = [];
		}
		// 判断是否为业务费用且款项名称为快递费、运杂费、海运费，使用物流公司银行账号接口
		else if (addpaymentrequestform.value.paymentCategory === '5' && isLogisticsPaymentName()) {
			// 业务费用（快递费、运杂费、海运费）：使用物流公司银行账号接口
			const bankAccountResponse = await request({
				url: 'LogisticsCompany/GetLogisticsCompanyBankAccountList/GetBankAccountList',
				method: 'GET',
				params: {
					logisticsCompanyId: addpaymentrequestform.value.payeeCode
				}
			});

			if (bankAccountResponse && bankAccountResponse.code === 200) {
				supplierBankAccounts.value = bankAccountResponse.data || [];

				// 如果有银行账号，默认选中第一个
				if (supplierBankAccounts.value.length > 0) {
					const firstAccount = supplierBankAccounts.value[0];
					addpaymentrequestform.value.bankName = firstAccount.bank || '';
					addpaymentrequestform.value.bankAccount = firstAccount.bankAccountNumber || '';
				} else {
					// 如果没有银行账号，清空相关字段
					addpaymentrequestform.value.bankName = '';
					addpaymentrequestform.value.bankAccount = '';
				}
			} else {
				// 如果获取银行账号失败，清空相关字段
				addpaymentrequestform.value.bankName = '';
				addpaymentrequestform.value.bankAccount = '';
				supplierBankAccounts.value = [];
			}
		} else {
			// 非业务费用：使用供应商银行账号接口
			// 获取供应商基本信息
			const supplierResponse = await request({
				url: 'Supplierinfo/GetSupplierInfoByID/GetSupplierInfo',
				method: 'GET',
				params: {
					SupplierID: addpaymentrequestform.value.payeeCode
				}
			});

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
		}

		// 只有在需要显示付款明细时才获取未付款详情列表
		if (showPaymentDetails.value) {
			try {
				const unpaidResponse = await request({
					//url: 'PaymentRequest/GetUnpaidPaymentListBySupplierID/GetUnpaidPaymentList',
					url: 'UnpaidAmount/GetUnpaidAmountList/GetList',
					method: 'GET',
					params: {
						supplierID: addpaymentrequestform.value.payeeCode
					}
				});

				if (unpaidResponse.data && unpaidResponse.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
					// 保存原始未支付款项数据到Map，用于恢复时使用
					UnpaidDetailsTbaleData.value.forEach((element) => {
						const key = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
						originalUnpaidItemsMap.value.set(key, { ...element });
					});
					// 新API数据结构不需要处理contractStatus，直接使用返回的数据
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

		// 如果是业务费用且为其它款项名称，加载合同列表
		if (isBusinessExpenseWithContract()) {
			// 清空之前的关联合同选择
			addpaymentrequestform.value.relatedContract = '';
			await loadContractList(addpaymentrequestform.value.payeeCode);
		} else {
			// 如果不是业务费用或不是其它款项名称，清空合同列表和关联合同
			contractList.value = [];
			addpaymentrequestform.value.relatedContract = '';
		}

		// 如果是业务费用且款项名称是快递费，获取收寄样列表
		if (isBusinessExpenseWithExpressFee()) {
			await loadSampleCollectionList(addpaymentrequestform.value.payeeCode);
		} else {
			// 如果不是业务费用或不是快递费，清空收寄样列表
			sampleCollectionTableData.value = [];
			showSampleCollection.value = false;
			sampleCollectionTotalItems.value = 0;
			sampleCollectionCurrentPage.value = 1;
			selectedSampleCollectionTableData.value = [];
			showSelectedSampleCollection.value = false;
			selectedSampleCollectionTotalItems.value = 0;
			selectedSampleCollectionCurrentPage.value = 1;
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
		addpaymentrequestform.value.relatedContract = '';
		supplierBankAccounts.value = [];
		UnpaidDetailsTbaleData.value = [];
		contractList.value = [];
		sampleCollectionTableData.value = [];
		showSampleCollection.value = false;
		sampleCollectionTotalItems.value = 0;
		sampleCollectionCurrentPage.value = 1;
		selectedSampleCollectionTableData.value = [];
		showSelectedSampleCollection.value = false;
		selectedSampleCollectionTotalItems.value = 0;
		selectedSampleCollectionCurrentPage.value = 1;
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
	CompanyType: 0, // 新增公司类型字段
	RelatedContract: '', // 关联合同
	RelatedCustomer: null, // 关联客户（可空整数）
	ActualApplicationAmount: 0, // 本次申请金额
	PaymentRequestDetails: [],
	SampleReceipts: [] // 已选择的样品收据
});

// 统一的表单处理工具函数
const FormHandler = {
	// 构建付款申请请求数据
	buildPaymentRequestData(isEdit = false, isSubmit = false) {
		const isFactoryPayment = addpaymentrequestform.value.paymentCategory === '1';
		let finalTotalAmount;

		if (isFactoryPayment && isSubmit) {
			// 工厂付款提交时：使用重新计算的总金额，确保与明细金额一致
			finalTotalAmount = CostDetailsTbaleData.value.reduce((sum, row) => {
				return sum + (parseFloat(row.currentPaymentAmount) || 0);
			}, 0);
		} else {
			// 其他情况：使用用户输入的申请金额
			finalTotalAmount = Number(addpaymentrequestform.value.totalAmount);
		}

		// 转换字段映射，使用新的请求体结构
		const processedDetails = CostDetailsTbaleData.value.map(detail => ({
			// 基础字段
			Id: Number(detail.id) || 0,
			SupplierID: Number(addpaymentrequestform.value.payeeCode) || 0,
			PaymentType: Number(detail.relatedDocumentType) || Number(detail.relatedmodules) || 0,
			Currency: Number(addpaymentrequestform.value.currencyCode) || 0,
			ExchangeRate: 1.0, // 默认汇率，可以根据需要调整
			Amount: Number(detail.currentPaymentAmount) || 0,
			IsPaid: 0, // 默认未支付
			RelatedDocumentType: Number(detail.relatedDocumentType) || Number(detail.relatedmodules) || 0,
			RelatedDocumentID: Number(detail.relatedDocumentID) || Number(detail.id) || 0,
			IsDeleted: 0, // 默认未删除
			ExpenseName: detail.expenseName || '',
			PaymentRequestID: PaymentRequestID.value || 0,
			UnpaidAmountID: Number(detail.unpaidAmountID) || 0 // 未支付款项ID
		}));

		// 构建SampleReceipts数组
		const sampleReceipts = selectedSampleCollectionTableData.value.map(row => {
			// 获取销售合同ID：如果有已有数据，使用原始ID；否则从下拉框选择的值转换
			let relatedContractIDValue = null;
			if (row.hasExistingContract && row.relatedContractIDOriginal) {
				// 如果有已有数据，使用原始ID
				relatedContractIDValue = Number(row.relatedContractIDOriginal);
			} else if (row.relatedContractNumber && row.relatedContractNumber !== '') {
				// 从下拉框选择的值（合同编号）转换为ID
				const contract = applicantSaleContracts.value.find(
					c => c.dictLabel === row.relatedContractNumber
				);
				relatedContractIDValue = contract ? Number(contract.dictvalue) : null;
			}

			// 获取出运合同ID：如果有已有数据，使用原始ID；否则从下拉框选择的值转换
			let relatedShippingContractsIDValue = null;
			if (row.hasExistingShipping && row.relatedShippingContractsIDOriginal) {
				// 如果有已有数据，使用原始ID
				relatedShippingContractsIDValue = Number(row.relatedShippingContractsIDOriginal);
			} else if (row.relatedShippingNumber && row.relatedShippingNumber !== '') {
				// 从下拉框选择的值（运编号）转换为ID
				const shipping = applicantShippingContracts.value.find(
					s => s.dictLabel === row.relatedShippingNumber
				);
				relatedShippingContractsIDValue = shipping ? Number(shipping.dictvalue) : null;
			}

			return {
				SampleID: Number(row.sampleID) || 0,
				RelatedContractID: relatedContractIDValue,
				RelatedShippingContractsID: relatedShippingContractsIDValue
			};
		});

		// 计算本次申请金额（ActualApplicationAmount）
		// 本次申请金额 = 付款明细中所有明细项的本次申请金额之和
		const actualApplicationAmount = CostDetailsTbaleData.value.reduce((sum, row) => {
			return sum + (parseFloat(row.currentPaymentAmount) || 0);
		}, 0);

		return {
			id: isEdit ? PaymentRequestID.value : 0,
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
			TotalAmount: finalTotalAmount,
			PaidAmount: Number(addpaymentrequestform.value.paidAmount),
			UnpaidAmount: Number(addpaymentrequestform.value.unpaidAmount),
			Applicant: Number(addpaymentrequestform.value.applicant),
			ApplicationDepartment: Number(addpaymentrequestform.value.applicationDepartment),
			FinancialApproval: Number(addpaymentrequestform.value.financialApproval),
			Handler: Number(addpaymentrequestform.value.handler),
			Remark: addpaymentrequestform.value.remarks,
			IsDelete: 0,
			CompanyType: getCurrentCompanyType(),
			RelatedContract: addpaymentrequestform.value.relatedContract || '', // 关联合同字段
			RelatedCustomer: addpaymentrequestform.value.relatedCustomer ? Number(addpaymentrequestform.value.relatedCustomer) : null, // 相关客户字段（可空整数）
			ActualApplicationAmount: actualApplicationAmount, // 本次申请金额
			PaymentRequestDetails: processedDetails,
			SampleReceipts: sampleReceipts
		};
	},

	// 处理保存响应
	handleSaveResponse(response, isEdit = false) {
		if (response != null) {
			ElMessage({
				message: isEdit ? '付款申请单编辑成功！' : '付款申请单添加成功！',
				type: 'success'
			});
			addpaymentrequestdialog.value = false;
			GetPaymentRequestList(paymentrequesttableDataCurrentPage.value, paymentrequesttableDataPageSize.value);
		} else {
			console.error(isEdit ? '编辑付款申请单出错' : '新增付款申请单出错');
		}
	},

	// 处理保存错误
	handleSaveError(error, isEdit = false) {
		console.error(isEdit ? '编辑付款申请单出错！😔错误内容：' : '新增付款申请单出错！😔错误内容：', error);
	}
};

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
		const requestData = FormHandler.buildPaymentRequestData(false, false);
		Object.assign(paymentRequestRequest, requestData);

		request.post('PaymentRequest/AddPaymentRequest/Add', paymentRequestRequest)
			.then(response => FormHandler.handleSaveResponse(response, false))
			.catch(error => FormHandler.handleSaveError(error, false));
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
	'3': '已拒绝',
	'4': '已付款',
	'5': '已归帐'
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

// 待支付款项收寄样单据分页处理
const sampleCollectionHandlePageChange = (newPage) => {
	sampleCollectionCurrentPage.value = newPage;
};

// 已选择收寄样单据分页处理
const selectedSampleCollectionHandlePageChange = (newPage) => {
	selectedSampleCollectionCurrentPage.value = newPage;
};

// 监听筛选后的数据变化，自动更新总数
watch(filteredSampleCollectionData, (newVal) => {
	sampleCollectionTotalItems.value = newVal.length;
	// 如果当前页超出范围，调整到最后一页
	const maxPage = Math.ceil(newVal.length / sampleCollectionPageSize.value);
	if (sampleCollectionCurrentPage.value > maxPage && maxPage > 0) {
		sampleCollectionCurrentPage.value = maxPage;
	} else if (newVal.length === 0) {
		sampleCollectionCurrentPage.value = 1;
	}
}, { deep: true });

watch(filteredSelectedSampleCollectionData, (newVal) => {
	selectedSampleCollectionTotalItems.value = newVal.length;
	// 如果当前页超出范围，调整到最后一页
	const maxPage = Math.ceil(newVal.length / selectedSampleCollectionPageSize.value);
	if (selectedSampleCollectionCurrentPage.value > maxPage && maxPage > 0) {
		selectedSampleCollectionCurrentPage.value = maxPage;
	} else if (newVal.length === 0) {
		selectedSampleCollectionCurrentPage.value = 1;
	}
}, { deep: true });

// 监听筛选条件变化，重置分页
watch([filterSampleCustomerSupplier, filterSampleOurCompany], () => {
	sampleCollectionCurrentPage.value = 1;
});

watch([filterSelectedCustomerSupplier, filterSelectedOurCompany], () => {
	selectedSampleCollectionCurrentPage.value = 1;
});

// 监听申请人变化，自动加载关联的合同列表
watch(() => addpaymentrequestform.value.applicant, async (newApplicantId) => {
	if (newApplicantId) {
		await loadApplicantSaleContracts(newApplicantId);
		await loadApplicantShippingContracts(newApplicantId);
	} else {
		applicantSaleContracts.value = [];
		applicantShippingContracts.value = [];
	}
});

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

		// 第一步：先加载付款类别和款项名称
		await paymentCategoryChange();
		addpaymentrequestform.value.paymentName = response.data.paymentRequest.paymentName.toString();
		// 第二步：根据付款类别和款项名称决定加载哪种收款单位选项
		// 如果是业务费用，根据款项名称加载对应的收款单位选项
		if (addpaymentrequestform.value.paymentCategory == '5') {
			// 等待一个微任务，确保PaymentTypeOptions已经设置
			await new Promise(resolve => setTimeout(resolve, 0));
			await loadBusinessExpensePayeeOptionsForViewByPaymentName(addpaymentrequestform.value.paymentName);
		} else if (addpaymentrequestform.value.paymentCategory == '4') {
			// 如果是日常费用，根据款项名称加载对应的收款单位选项
			// 等待一个微任务，确保PaymentTypeOptions已经设置
			await new Promise(resolve => setTimeout(resolve, 0));
			await loadDailyExpensePayeeOptionsForViewByPaymentName(addpaymentrequestform.value.paymentName);
			// 如果是客户事宜，加载客户选项
			if (isDailyExpenseWithCustomerOptions()) {
				await loadCustomerOptions();
			}
		} else {
			// 确保供应商列表已加载
			if (filteredSupplierList.value.length === 0) {
				await loadFilteredSuppliers();
			}
		}
		// 第三步：确保当前保存的收款单位在选项列表中
		const currentPayeeExists = filteredSupplierList.value.find(
			item => item.dictvalue == response.data.paymentRequest.payeeCode
		);

		if (!currentPayeeExists) {
			// 只有在非业务费用情况下，才从原始供应商列表中查找
			if (addpaymentrequestform.value.paymentCategory !== '5') {
				const originalSupplier = state.optionss.sql_supplier_info.find(
					item => item.dictValue == response.data.paymentRequest.payeeCode
				);
				if (originalSupplier) {
					// 将当前收款单位添加到选项列表的开头
					filteredSupplierList.value.unshift({
						dictvalue: originalSupplier.dictValue,
						dictLabel: originalSupplier.dictLabel
					});
				}
			} else {
				// 业务费用情况下，如果当前收款单位不在物流公司选项中，尝试从供应商列表中查找
				const originalSupplier = state.optionss.sql_supplier_info.find(
					item => item.dictValue == response.data.paymentRequest.payeeCode
				);
				if (originalSupplier) {
					// 检查是否已经存在相同的收款单位（避免重复）
					const duplicateExists = filteredSupplierList.value.find(
						item => item.dictvalue == originalSupplier.dictValue
					);
					if (!duplicateExists) {
						filteredSupplierList.value.unshift({
							dictvalue: originalSupplier.dictValue,
							dictLabel: originalSupplier.dictLabel
						});
					}
				}
			}
		}

		// 第四步：标准化和去重选项列表
		const standardizedOptions = filteredSupplierList.value.map(item => ({
			dictvalue: item.dictvalue || item.dictValue || item.value,
			dictLabel: item.dictLabel || item.label || item.name
		}));

		// 去重检查（基于标准化后的dictvalue）
		const uniqueOptions = standardizedOptions.filter((item, index, self) =>
			index === self.findIndex(t => t.dictvalue == item.dictvalue)
		);

		// 更新选项列表为去重后的标准化数据
		filteredSupplierList.value = uniqueOptions;

		// 第五步：收款单位选项列表准备好后，再设置收款单位的值
		const payeeCodeValue = response.data.paymentRequest.payeeCode;

		// 尝试在选项列表中查找匹配的收款单位
		const matchedPayee = filteredSupplierList.value.find(
			item => item.dictvalue == payeeCodeValue ||
				item.dictvalue == payeeCodeValue.toString() ||
				item.dictvalue == Number(payeeCodeValue)
		);

		if (matchedPayee) {
			// 使用匹配到的收款单位的dictvalue作为payeeCode
			addpaymentrequestform.value.payeeCode = matchedPayee.dictvalue;
			addpaymentrequestform.value.payeeName = matchedPayee.dictLabel;
		} else {
			// 如果没有找到匹配的，使用原始值
			addpaymentrequestform.value.payeeCode = payeeCodeValue.toString();
			addpaymentrequestform.value.payeeName = response.data.paymentRequest.payeeName;
		}

		addpaymentrequestform.value.bankName = response.data.paymentRequest.bankName;
		addpaymentrequestform.value.bankAccount = response.data.paymentRequest.bankAccount;

		// 收款单位选项和值已经在上面的逻辑中处理完成

		// 查看详情时不需要调用payeeCodeChange，直接根据付款类别和款项名称加载银行账号

		// 加载银行账号列表（根据付款类别和款项名称使用不同接口）
		try {
			if (addpaymentrequestform.value.paymentCategory === '5' && isLogisticsPaymentNameForView(addpaymentrequestform.value.paymentName)) {
				// 业务费用（快递费、运杂费、海运费）：使用物流公司银行账号接口
				const bankAccountResponse = await request({
					url: 'LogisticsCompany/GetLogisticsCompanyBankAccountList/GetBankAccountList',
					method: 'GET',
					params: {
						logisticsCompanyId: addpaymentrequestform.value.payeeCode
					}
				});

				if (bankAccountResponse && bankAccountResponse.code === 200) {
					supplierBankAccounts.value = bankAccountResponse.data || [];
				} else {
					supplierBankAccounts.value = [];
				}
			} else {
				// 非业务费用或业务费用的"其他"款项：使用供应商银行账号接口
				const bankAccountResponse = await request({
					url: 'Supplierinfo/GetSupplierBankAccountList/GetBankAccountList',
					method: 'GET',
					params: {
						supplierId: addpaymentrequestform.value.payeeCode
					}
				});

				if (bankAccountResponse.data && bankAccountResponse.data.code === 200) {
					supplierBankAccounts.value = bankAccountResponse.data.data || [];
				} else {
					supplierBankAccounts.value = [];
				}
			}
		} catch (error) {
			console.error('获取银行账号列表失败:', error);
			supplierBankAccounts.value = [];
		}

		// 只有在需要显示付款明细时才获取未付款详情列表（查看详情和编辑模式都需要）
		if (showPaymentDetails.value) {
			try {
				const unpaidResponse = await request({
					url: 'PaymentRequest/GetUnpaidPaymentListBySupplierID/GetUnpaidPaymentList',
					method: 'GET',
					params: {
						supplierID: response.data.paymentRequest.payeeCode,
						paymentType: Number(response.data.paymentRequest.paymentName) || 0
					}
				});

				if (unpaidResponse && unpaidResponse.code === 200) {
					UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
					// 处理contractStatus显示，并保存原始数据到Map
					UnpaidDetailsTbaleData.value.forEach((element) => {
						// 保存原始的contractStatus数值，同时添加显示用的contractStatusLabel
						element.contractStatusOriginal = element.contractStatus; // 保存原始数值
						element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
						// 保存原始未支付款项数据到Map，用于恢复时使用
						const key = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
						originalUnpaidItemsMap.value.set(key, { ...element });
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
		addpaymentrequestform.value.relatedContract = response.data.paymentRequest.relatedContracts || '';

		// 如果是日常费用且为客户事宜，确保客户选项已加载后再设置相关客户值
		// 直接检查条件，不依赖isDailyExpenseWithCustomerOptions()，因为PaymentTypeOptions可能还没设置好
		const isDailyExpense = addpaymentrequestform.value.paymentCategory === '4';
		const paymentNameValue = addpaymentrequestform.value.paymentName;
		// 检查是否为客户事宜：从字典中查找对应的标签
		let isCustomerMatter = false;
		if (isDailyExpense && paymentNameValue) {
			const paymentNameOption = state.optionss.hr_daily_expenses?.find(
				item => item.dictValue == paymentNameValue || item.dictValue === paymentNameValue.toString()
			);
			isCustomerMatter = paymentNameOption?.dictLabel?.includes('客户事宜') || false;
		}

		if (isDailyExpense && isCustomerMatter) {
			// 确保客户选项已加载（无论是否已加载，都重新加载以确保数据最新）
			await loadCustomerOptions();

			// 设置相关客户值，确保格式匹配
			const relatedCustomerValue = response.data.paymentRequest.relatedCustomer;

			if (relatedCustomerValue !== null && relatedCustomerValue !== undefined && relatedCustomerValue !== '') {
				// 尝试在客户选项列表中查找匹配的客户（支持多种格式匹配）
				const matchedCustomer = customerOptions.value.find(item => {
					// 使用宽松比较，支持字符串和数字类型匹配
					return String(item.dictvalue) === String(relatedCustomerValue) ||
						Number(item.dictvalue) === Number(relatedCustomerValue) ||
						item.dictvalue == relatedCustomerValue;
				});

				if (matchedCustomer) {
					// 使用匹配到的客户的dictvalue（保持原始格式，确保与el-select的value匹配）
					addpaymentrequestform.value.relatedCustomer = matchedCustomer.dictvalue;
				} else {
					// 如果没有找到匹配的，使用原始值
					addpaymentrequestform.value.relatedCustomer = relatedCustomerValue;
				}
			} else {
				addpaymentrequestform.value.relatedCustomer = '';
			}
		} else {
			addpaymentrequestform.value.relatedCustomer = response.data.paymentRequest.relatedCustomer || '';
		}

		// 加载与申请人关联的合同列表
		if (addpaymentrequestform.value.applicant) {
			await loadApplicantSaleContracts(addpaymentrequestform.value.applicant);
			await loadApplicantShippingContracts(addpaymentrequestform.value.applicant);
		}
		// 如果是业务费用且为其它款项名称，先加载合同列表，再设置关联合同值
		if (isBusinessExpenseWithContract()) {
			await loadContractList(addpaymentrequestform.value.payeeCode);
			// 合同列表加载完成后再设置关联合同值
			addpaymentrequestform.value.relatedContract = response.data.paymentRequest.relatedContracts || '';
		} else {
			// 如果不是业务费用或不是其它款项名称，清空合同列表
			contractList.value = [];
			addpaymentrequestform.value.relatedContract = response.data.paymentRequest.relatedContracts || '';
		}

		// 如果是业务费用且款项名称是快递费，加载收寄样列表
		if (isBusinessExpenseWithExpressFee() && addpaymentrequestform.value.payeeCode) {
			// 判断是否是待审批或已审批通过状态
			const isPendingOrApproved = (row.reviewStatus == "0" || row.reviewStatus == "3" || row.reviewStatusStr === '已批准' || row.reviewStatus == "2");

			// 如果不是待审批或已审批通过状态，才加载待支付收寄样列表
			if (!isPendingOrApproved) {
				await loadSampleCollectionList(addpaymentrequestform.value.payeeCode);
			}

			// 如果是待审批或已审批通过状态，隐藏待支付收寄样列表
			if (isPendingOrApproved) {
				showSampleCollection.value = false;
				sampleCollectionTableData.value = [];
				sampleCollectionTotalItems.value = 0;
				sampleCollectionCurrentPage.value = 1;
			}

			// 处理后端返回的已选择收寄样单据（sampleReceipt数组）
			if (response.data.sampleReceipt && response.data.sampleReceipt.length > 0) {
				selectedSampleCollectionTableData.value = response.data.sampleReceipt.map(item => {
					// 转换寄样/收样类型
					const type = item.type === 1 ? '寄样' : '收样';
					// 转换客户/供应商类型
					const customerOrSupplier = item.customer_or_Supplier === 1 ? '客户' : '供应商';
					// 转换客户ID为名称
					let customerIdLabel = '';
					if (item.customer_ID && item.customer_ID !== 0) {
						const customer = state.optionss.sql_hr_customer_abbreviation?.find(
							c => c.dictValue === item.customer_ID.toString()
						);
						customerIdLabel = customer ? customer.dictLabel : item.customer_ID.toString();
					}
					// 转换快递公司ID为名称
					let expressCompanyLabel = '';
					if (item.express_Company && item.express_Company !== 0) {
						const company = state.optionss.hr_express_delivery_company?.find(
							c => c.dictValue === item.express_Company.toString()
						);
						expressCompanyLabel = company ? company.dictLabel : item.express_Company.toString();
					}
					// 转换付费方式
					let paymentMethodLabel = '';
					if (item.payment_Method && item.payment_Method !== 0) {
						const method = state.optionss.hr_express_payment_method?.find(
							m => m.dictValue === item.payment_Method.toString()
						);
						paymentMethodLabel = method ? method.dictLabel : '';
					}
					// 转换我方公司
					let companyIdLabel = '';
					if (item.company_ID && item.company_ID !== 0) {
						const company = state.optionss.hr_ourcompany?.find(
							c => c.dictValue === item.company_ID.toString()
						);
						companyIdLabel = company ? company.dictLabel : '';
					}
					// 转换销售合同ID为合同编号
					let relatedContractNumber = '';
					if (item.relatedContractID && item.relatedContractID !== 0) {
						const contract = state.optionss.sql_sale_contracts?.find(
							c => c.dictValue === item.relatedContractID.toString()
						);
						relatedContractNumber = contract ? contract.dictLabel : '';
					}
					// 转换出运合同ID为合同编号
					let relatedShippingNumber = '';
					if (item.relatedShippingContractsID && item.relatedShippingContractsID !== 0) {
						const shipping = state.optionss.sql_shippingdeliveries?.find(
							s => s.dictValue === item.relatedShippingContractsID.toString()
						);
						relatedShippingNumber = shipping ? shipping.dictLabel : '';
					}

					return {
						sampleID: item.id || 0,
						type: type,
						customer_or_Supplier: customerOrSupplier,
						customer_ID: customerIdLabel,
						waybill_Number: item.waybill_Number || '',
						express_Company: expressCompanyLabel,
						sample_Date: formatDate(item.sample_Date),
						payment_Method: paymentMethodLabel,
						company_ID: companyIdLabel,
						paid_Express_Fee: (item.paid_Express_Fee || 0).toFixed(2),
						relatedContractID: relatedContractNumber, // 显示用合同编号
						relatedShippingContractsID: relatedShippingNumber, // 显示用运编号
						relatedContractIDOriginal: item.relatedContractID || null, // 保存原始合同ID
						relatedShippingContractsIDOriginal: item.relatedShippingContractsID || null, // 保存原始出运合同ID
						relatedContractNumber: relatedContractNumber || '', // 关联合同号下拉框值
						relatedShippingNumber: relatedShippingNumber || '', // 关联运编号下拉框值
						hasExistingContract: !!(item.relatedContractID && item.relatedContractID !== 0), // 是否有已有合同
						hasExistingShipping: !!(item.relatedShippingContractsID && item.relatedShippingContractsID !== 0), // 是否有已有出运合同
						shippingContractOptions: [] // 初始化出运合同选项列表
					};
				});

				// 为每个有关联合同号的行加载对应的出运合同列表（仅在非已有合同的情况下）
				for (const row of selectedSampleCollectionTableData.value) {
					if (row.relatedContractNumber && !row.hasExistingContract) {
						await loadShippingContractsByContractNumber(
							row,
							row.relatedContractNumber,
							row.customer_or_Supplier
						);
					}
				}

				// 显示已选择收寄样列表
				if (selectedSampleCollectionTableData.value.length > 0) {
					showSelectedSampleCollection.value = true;
				}

				// 如果不是待审批或已审批通过状态，从待支付收寄样列表中过滤掉已选择的记录
				if (!isPendingOrApproved && sampleCollectionTableData.value.length > 0) {
					sampleCollectionTableData.value = sampleCollectionTableData.value.filter(item => {
						return !selectedSampleCollectionTableData.value.some(selected =>
							selected.waybill_Number === item.waybill_Number &&
							selected.customer_ID === item.customer_ID &&
							selected.sample_Date === item.sample_Date
						);
					});
				}

				// 重新计算已付快件费总额
				calculateTotalPaidExpressFee();
			} else {
				// 如果没有已选择的收寄样单据数据，清空已选择收寄样列表
				selectedSampleCollectionTableData.value = [];
				showSelectedSampleCollection.value = false;
				selectedSampleCollectionTotalItems.value = 0;
				selectedSampleCollectionCurrentPage.value = 1;
			}
		} else {
			sampleCollectionTableData.value = [];
			showSampleCollection.value = false;
			sampleCollectionTotalItems.value = 0;
			sampleCollectionCurrentPage.value = 1;
			selectedSampleCollectionTableData.value = [];
			showSelectedSampleCollection.value = false;
			selectedSampleCollectionTotalItems.value = 0;
			selectedSampleCollectionCurrentPage.value = 1;
		}

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
			element.actualShippingAmount = detail.actualShippingAmount || 0;
			element.deposit = detail.deposit;
			element.paidAmount = detail.paidAmount;
			element.unpaidAmount = detail.unpaidAmount;
			// —— 列表展示所需的字段别名（与“未支付款项详情”一致） —— //
			element.depositPaidAmount = detail.deposit || 0;       // 已付预付款
			element.depositUnpaidAmount = detail.depositUnpaidAmount || 0; // 未付预付款（如果后端没有该字段则为0）
			element.goodsPaidAmount = detail.paidAmount || 0;      // 已付货款
			element.goodsUnpaidAmount = detail.unpaidAmount || 0;  // 未付货款
			element.contractStatusOriginal = detail.contractStatus; // 保存原始contractStatus数值
			element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == detail.contractStatus)?.dictLabel || ''; // 显示用的标签

			// 新增字段映射 - 使用现有字段或提供默认值
			element.expenseName = detail.expenseName || '未设置费用名称';
			element.relatedDocumentType = detail.relatedDocumentType || detail.relatedModules || 0;
			element.relatedDocumentTypeName = detail.relatedDocumentTypeName || '未设置单据类型';
			element.relatedDocumentID = detail.relatedDocumentID || detail.shippingOrderID || 0;
			element.relatedDocumentsNo = detail.relatedDocumentsNo || detail.invoiceNumber || detail.purchaseContractNumber || '';
			// 金额：使用接口返回的amount字段
			element.amount = detail.amount || detail.totalGoodsValue || detail.actualShippingAmount || 0;

			// 已申请金额：使用接口返回的appliedAmount字段
			element.appliedAmount = detail.appliedAmount || 0;

			// 本次申请金额：使用接口返回的amountAlreadyApplied字段
			element.currentPaymentAmount = detail.amountAlreadyApplied || 0;
			// 未支付款项ID：使用接口返回的UnpaidAmountID字段
			element.unpaidAmountID = detail.UnpaidAmountID || detail.unpaidAmountID || 0;
			element.remark = detail.remark;

			// 兼容旧字段
			element.relatedmodules = detail.relatedModules?.toString() || '0';
			element.associatedordernumber = detail.associatedOrderNumber?.toString() || '0';
			element.applicationamount = detail.applicationAmount || 0;
			element.relevantdates = detail.relevantDates || '';
			element.specificpaymentitems = detail.specificPaymentItems?.toString() || '0';

			// 如果是从未付款项添加的数据，标记为已选择（仅在编辑模式下）
			if (element.expenseName && element.relatedDocumentsNo && !IsDisabled.value) {
				const unpaidItem = UnpaidDetailsTbaleData.value.find(item =>
					item.expenseName === element.expenseName &&
					item.relatedDocumentsNo === element.relatedDocumentsNo &&
					item.relatedDocumentType === element.relatedDocumentType
				);
				if (unpaidItem) {
					selectedUnpaidItemIds.value.add(unpaidItem.id);
				}
			}

			// 保存付款明细对应的原始未支付款项数据到Map（用于恢复时使用）
			// 构建原始未支付款项数据结构，使用接口返回的appliedAmount字段（已申请金额）
			const originalUnpaidKey = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
			if (!originalUnpaidItemsMap.value.has(originalUnpaidKey)) {
				const originalUnpaidItem = {
					id: element.id || 0,
					expenseName: element.expenseName || '',
					relatedDocumentType: element.relatedDocumentType || 0,
					relatedDocumentTypeName: element.relatedDocumentTypeName || '',
					relatedDocumentsNo: element.relatedDocumentsNo || '',
					amount: element.amount || 0,
					appliedAmount: element.appliedAmount || 0, // 使用接口返回的appliedAmount（已申请金额）
					contractStatus: element.contractStatusOriginal || 0,
					contractStatusOriginal: element.contractStatusOriginal || 0
				};
				originalUnpaidItemsMap.value.set(originalUnpaidKey, originalUnpaidItem);
			}
		});

		// 查看详情时不需要重新计算总金额，直接使用接口返回的值
		// CalculatetotalAmount();

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
		const requestData = FormHandler.buildPaymentRequestData(true, false);
		Object.assign(paymentRequestRequest, requestData);

		request.post('PaymentRequest/EditPaymentRequest/Edit', paymentRequestRequest)
			.then(response => {
				FormHandler.handleSaveResponse(response, true);
				// 编辑成功后的额外处理
				IsDisabled.value = true;
				showEditBtn.value = true;
				isEditSaveBtnShow.value = false;
				showSubmitReviewBtn.value = true;
			})
			.catch(error => FormHandler.handleSaveError(error, true));
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


	// 编辑时重新加载未支付款项列表，过滤掉已选择的单据
	if (showPaymentDetails.value) {
		// 重新获取未支付款项列表，确保数据是最新的
		try {
			const unpaidResponse = await request({
				url: 'UnpaidAmount/GetUnpaidAmountList/GetList',
				method: 'GET',
				params: {
					supplierID: addpaymentrequestform.value.payeeCode
				}
			});
			if (unpaidResponse && unpaidResponse.code === 200) {
				const allUnpaidItems = unpaidResponse.data || [];
				// 过滤掉已经存在于付款明细中的单据
				const filteredUnpaidItems = allUnpaidItems.filter(unpaidItem => {
					return !CostDetailsTbaleData.value.some(selectedItem =>
						selectedItem.expenseName === unpaidItem.expenseName &&
						selectedItem.relatedDocumentsNo === unpaidItem.relatedDocumentsNo &&
						selectedItem.relatedDocumentType === unpaidItem.relatedDocumentType
					);
				});
				UnpaidDetailsTbaleData.value = filteredUnpaidItems;

				// 处理contractStatus显示，并保存原始数据到Map
				UnpaidDetailsTbaleData.value.forEach((element) => {
					element.contractStatusOriginal = element.contractStatus; // 保存原始数值
					element.contractStatus = state.optionss.hr_contract_status.find((item) => item.dictValue == element.contractStatus)?.dictLabel || '';
					// 保存原始未支付款项数据到Map，用于恢复时使用
					const key = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
					originalUnpaidItemsMap.value.set(key, { ...element });
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

	// 新增付款申请时隐藏审核按钮
	showApproveRejectBtn.value = false;
	showApprovePassBtn.value = false;

	// 设置默认申请日期为当天
	const today = new Date();
	const year = today.getFullYear();
	const month = String(today.getMonth() + 1).padStart(2, '0');
	const day = String(today.getDate()).padStart(2, '0');
	addpaymentrequestform.value.applicationDate = `${year}-${month}-${day}`;

	// 设置币种默认为人民币
	addpaymentrequestform.value.currencyCode = '2';

	// 设置部门和申请人
	if (userInfo.deptId.toString() == "0") {
		addpaymentrequestform.value.applicationDepartment = state.optionss.sql_hr_dept.find((item) => item.dictValue == "205").dictValue;
	} else {
		addpaymentrequestform.value.applicationDepartment = state.optionss.sql_hr_dept.find((item) => item.dictValue == userInfo.deptId.toString()).dictValue;
	}
	addpaymentrequestform.value.applicant = state.optionss.sql_all_user.find((item) => item.dictValue == userInfo.userId.toString()).dictValue;

	// 加载与申请人关联的合同列表
	if (addpaymentrequestform.value.applicant) {
		await loadApplicantSaleContracts(addpaymentrequestform.value.applicant);
		await loadApplicantShippingContracts(addpaymentrequestform.value.applicant);
	}

	// 如果是工厂付款，初始化申请金额为0
	if (addpaymentrequestform.value.paymentCategory === '1') {
		addpaymentrequestform.value.totalAmount = '0.00';
	}

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
				const requestData = FormHandler.buildPaymentRequestData(true, true);
				Object.assign(paymentRequestRequest, requestData);

				// 发送保存并提交审核请求
				request({
					url: 'PaymentRequest/SaveAndSubmitForReview/SaveAndSubmitForReview',
					method: 'POST',
					data: paymentRequestRequest
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
		currencyCode: '2', // 默认人民币
		totalAmount: '',
		paidAmount: '',
		unpaidAmount: '',
		applicant: '',
		financialApproval: '',
		handler: '',
		remarks: '',
		relatedContract: '', // 关联合同字段
		relatedCustomer: '' // 相关客户字段
	};

	// 清空费用明细表格数据
	CostDetailsTbaleData.value = [];

	// 清空供应商银行账号列表
	supplierBankAccounts.value = [];

	// 清空合同列表
	contractList.value = [];

	// 清空客户选项列表
	customerOptions.value = [];

	// 清空与申请人关联的合同列表
	applicantSaleContracts.value = [];
	applicantShippingContracts.value = [];

	// 清空未付款详情数据
	UnpaidDetailsTbaleData.value = [];

	// 清空已选择的未付款项
	selectedUnpaidItemIds.value.clear();

	// 清空原始未支付款项数据Map
	originalUnpaidItemsMap.value.clear();

	// 清空收寄样列表
	sampleCollectionTableData.value = [];
	showSampleCollection.value = false;
	sampleCollectionTotalItems.value = 0;
	sampleCollectionCurrentPage.value = 1;

	// 清空已选择收寄样列表
	selectedSampleCollectionTableData.value = [];
	showSelectedSampleCollection.value = false;
	selectedSampleCollectionTotalItems.value = 0;
	selectedSampleCollectionCurrentPage.value = 1;

	// 清空筛选条件
	filterSampleCustomerSupplier.value = '';
	filterSampleOurCompany.value = '';
	filterSelectedCustomerSupplier.value = '';
	filterSelectedOurCompany.value = '';
};

const CalculatetotalAmount = () => {
	// 初始化总金额为0
	let total = 0;

	// 遍历费用明细表格中的所有行
	CostDetailsTbaleData.value.forEach(row => {
		// 如果是工厂付款，自动设置本次付款金额为对应的金额 - 已申请金额
		if (addpaymentrequestform.value.paymentCategory === '1') {
			// 优先使用amount字段，如果没有则使用totalGoodsValue，最后使用actualShippingAmount
			const amount = parseFloat(row.amount) || parseFloat(row.totalGoodsValue) || parseFloat(row.actualShippingAmount) || 0;
			// 获取已申请金额
			const appliedAmount = parseFloat(row.appliedAmount || 0);
			// 本次申请金额 = 金额 - 已申请金额
			const defaultCurrentPaymentAmount = Math.max(0, amount - appliedAmount);
			// 如果当前值为空、0、或者等于金额（说明是旧逻辑设置的），则更新为计算后的值
			// 否则保持用户输入的值
			const currentValue = parseFloat(row.currentPaymentAmount || 0);
			if (!row.currentPaymentAmount || currentValue === 0 || currentValue === amount) {
				row.currentPaymentAmount = defaultCurrentPaymentAmount.toFixed(2);
			}
		}

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

	// 如果是工厂付款，自动更新申请金额为付款明细的总和
	if (addpaymentrequestform.value.paymentCategory === '1') {
		addpaymentrequestform.value.totalAmount = total.toFixed(2);
	}

	// 只更新未付金额字段（总金额 - 已付金额）
	const paidAmount = parseFloat(addpaymentrequestform.value.paidAmount) || 0;
	addpaymentrequestform.value.unpaidAmount = (total - paidAmount).toFixed(2);
}

// 存储已选择的未付款项ID
const selectedUnpaidItemIds = ref(new Set());
// 存储原始未支付款项数据（包含接口返回的appliedAmount），用于恢复时使用
const originalUnpaidItemsMap = ref(new Map());

// 检查项目是否已被选择
const isItemSelected = (row) => {
	return selectedUnpaidItemIds.value.has(row.id);
};

// 检查未付款项是否已被选择
const isUnpaidItemSelected = (row) => {
	// 如果项目在付款明细中，则认为已选择
	return CostDetailsTbaleData.value.some(item =>
		item.expenseName === row.expenseName &&
		item.relatedDocumentsNo === row.relatedDocumentsNo &&
		item.relatedDocumentType === row.relatedDocumentType
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

// 统一的付款明细处理工具函数
const PaymentDetailsHandler = {
	// 创建新的付款明细项
	createPaymentDetailItem(row, existingId = null) {
		// 计算今日日期字符串，作为默认的关联日期
		const today = new Date();
		const year = today.getFullYear();
		const month = String(today.getMonth() + 1).padStart(2, '0');
		const day = String(today.getDate()).padStart(2, '0');
		const todayStr = `${year}-${month}-${day}`;

		// 获取已申请金额（优先使用appliedAmount，如果没有则使用applicationamount）
		const appliedAmount = parseFloat(row.appliedAmount || row.applicationamount || 0);
		// 获取金额
		const amount = parseFloat(row.amount || 0);
		// 本次申请金额默认值 = 金额 - 已申请金额
		const defaultCurrentPaymentAmount = Math.max(0, amount - appliedAmount);

		return {
			id: existingId !== null && existingId !== undefined ? existingId : 0, // 如果提供了已存在的ID则使用，否则为0（新增）
			pcid: row.relatedDocumentID || 0,
			shippingOrderNumber: row.relatedDocumentsNo || '',
			ShippingOrderNumberOptions: [],
			purchaseContractNumber: row.relatedDocumentsNo || '',
			PurchaseContractNumberOptions: [],
			totalGoodsValue: row.amount || 0,
			actualShippingAmount: row.amount || 0,
			// —— 四个金额字段 —— //
			depositPaidAmount: 0,
			depositUnpaidAmount: 0,
			goodsPaidAmount: 0,
			goodsUnpaidAmount: row.amount || 0,
			// 已申请金额
			appliedAmount: appliedAmount,
			// 本次申请金额默认值 = 金额 - 已申请金额
			currentPaymentAmount: defaultCurrentPaymentAmount,
			// 未支付款项ID（用于保存时传递到服务端）：优先使用unpaidAmountID，如果没有则使用row.id
			unpaidAmountID: row.unpaidAmountID || row.id || 0,
			contractStatus: '',
			contractStatusOriginal: 0,
			relevantdates: todayStr,
			remark: '',
			// 新增字段
			expenseName: row.expenseName || '',
			relatedDocumentType: row.relatedDocumentType || 0,
			relatedDocumentTypeName: row.relatedDocumentTypeName || '',
			relatedDocumentsNo: row.relatedDocumentsNo || '',
			amount: row.amount || 0
		};
	},

	// 创建原始未付款项（仅在无法从Map获取原始数据时使用）
	createOriginalUnpaidItem(item) {
		return {
			id: item.id || Date.now() + Math.random(),
			pcid: item.pcid || 0,
			relatedDocumentID: item.id || 0,
			expenseName: item.expenseName || '',
			relatedDocumentType: item.relatedDocumentType || 0,
			relatedDocumentTypeName: item.relatedDocumentTypeName || '',
			relatedDocumentsNo: item.relatedDocumentsNo || '',
			amount: item.amount || 0,
			appliedAmount: item.appliedAmount || 0, // 已申请金额字段
			// 兼容旧字段
			invoiceNumbers: item.shippingOrderNumber,
			purchaseContractNumber: item.purchaseContractNumber,
			totalGoodsValue: item.totalGoodsValue,
			actualShippingAmount: item.actualShippingAmount || 0,
			depositPaidAmount: item.depositPaidAmount || 0,
			depositUnpaidAmount: item.depositUnpaidAmount || 0,
			goodsPaidAmount: item.goodsPaidAmount || 0,
			goodsUnpaidAmount: item.goodsUnpaidAmount || 0,
			contractStatus: item.contractStatus,
			contractStatusOriginal: item.contractStatusOriginal || item.contractStatus || 0,
			remark: item.remark
		};
	},

	// 从付款明细中移除项目
	removeFromPaymentDetails(row) {
		const index = CostDetailsTbaleData.value.findIndex(item =>
			item.expenseName === row.expenseName &&
			item.relatedDocumentsNo === row.relatedDocumentsNo &&
			item.relatedDocumentType === row.relatedDocumentType
		);
		if (index > -1) {
			CostDetailsTbaleData.value.splice(index, 1);
		}
	},

	// 从未支付款项详情中移除项目
	removeFromUnpaidDetails(row) {
		const index = UnpaidDetailsTbaleData.value.findIndex(item =>
			item.expenseName === row.expenseName &&
			item.relatedDocumentsNo === row.relatedDocumentsNo &&
			item.relatedDocumentType === row.relatedDocumentType
		);
		if (index > -1) {
			UnpaidDetailsTbaleData.value.splice(index, 1);
		}
	},

	// 添加项目到付款明细
	addToPaymentDetails(row) {
		// 保存原始未支付款项数据到Map，用于恢复时使用
		const key = `${row.expenseName}_${row.relatedDocumentsNo}_${row.relatedDocumentType}`;
		if (!originalUnpaidItemsMap.value.has(key)) {
			originalUnpaidItemsMap.value.set(key, { ...row });
		}

		// 检查是否已存在相同的付款明细（编辑模式下，可能已经存在）
		const existingDetail = CostDetailsTbaleData.value.find(item =>
			item.expenseName === row.expenseName &&
			item.relatedDocumentsNo === row.relatedDocumentsNo &&
			item.relatedDocumentType === row.relatedDocumentType
		);

		// 如果已存在，保留原有的id；否则创建新的明细（id为0）
		const newDetail = this.createPaymentDetailItem(row, existingDetail?.id);

		CostDetailsTbaleData.value.push(newDetail);
		selectedUnpaidItemIds.value.add(row.id);
		this.removeFromUnpaidDetails(row);
		ElMessage.success('已添加到付款明细');
	},

	// 从付款明细中移除项目并添加到未支付款项
	removeFromPaymentDetailsAndAddToUnpaid(row) {
		selectedUnpaidItemIds.value.delete(row.relatedDocumentID || row.id);
		this.removeFromPaymentDetails(row);
		// 从Map中查找原始未支付款项数据（包含接口返回的appliedAmount）
		const key = `${row.expenseName}_${row.relatedDocumentsNo}_${row.relatedDocumentType}`;
		const originalItem = originalUnpaidItemsMap.value.get(key);
		// 如果找到原始数据，使用原始数据；否则使用当前数据
		const itemToAdd = originalItem ? { ...originalItem } : { ...row };
		UnpaidDetailsTbaleData.value.push(itemToAdd);
		ElMessage.success('已从付款明细中移除');
	}
};

// 处理未付款项选择事件
const handleUnpaidItemSelect = (row) => {
	const isSelected = isUnpaidItemSelected(row);

	if (isSelected) {
		PaymentDetailsHandler.removeFromPaymentDetailsAndAddToUnpaid(row);
	} else {
		PaymentDetailsHandler.addToPaymentDetails(row);
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
				selectedItem.expenseName === row.expenseName &&
				selectedItem.relatedDocumentsNo === row.relatedDocumentsNo &&
				selectedItem.relatedDocumentType === row.relatedDocumentType
			);

			if (!alreadyExists) {
				// 保存原始未支付款项数据到Map，用于恢复时使用
				const key = `${row.expenseName}_${row.relatedDocumentsNo}_${row.relatedDocumentType}`;
				if (!originalUnpaidItemsMap.value.has(key)) {
					originalUnpaidItemsMap.value.set(key, { ...row });
				}
				const newDetail = PaymentDetailsHandler.createPaymentDetailItem(row);
				CostDetailsTbaleData.value.push(newDetail);
				selectedUnpaidItemIds.value.add(row.id);
			}
		});

		// 清空未支付款项详情列表
		UnpaidDetailsTbaleData.value = [];

		ElMessage.success('已全选所有未付款项');
	} else {
		// 取消全选：将所有已选择的项目移回未支付款项详情列表
		const itemsToMoveBack = CostDetailsTbaleData.value.map(item => {
			// 优先从Map中查找原始未支付款项数据（包含接口返回的appliedAmount）
			const key = `${item.expenseName}_${item.relatedDocumentsNo}_${item.relatedDocumentType}`;
			const originalItem = originalUnpaidItemsMap.value.get(key);
			// 如果找到原始数据，使用原始数据；否则使用createOriginalUnpaidItem创建
			return originalItem ? { ...originalItem } : PaymentDetailsHandler.createOriginalUnpaidItem(item);
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

// 统一的金额验证工具函数
const PaymentAmountValidator = {
	// 检查是否为工厂付款
	isFactoryPayment() {
		return addpaymentrequestform.value.paymentCategory === '1';
	},

	// 获取本次付款金额合计
	getTotalPaymentAmount() {
		return CostDetailsTbaleData.value.reduce((sum, row) => {
			return sum + (parseFloat(row.currentPaymentAmount) || 0);
		}, 0);
	},

	// 获取申请金额
	getApplicationAmount() {
		return parseFloat(addpaymentrequestform.value.totalAmount) || 0;
	},

	// 检查本次付款金额是否超过实际金额
	isPaymentAmountExceeded(row) {
		const currentPaymentAmount = parseFloat(row.currentPaymentAmount) || 0;
		const appliedAmount = parseFloat(row.appliedAmount) || 0;
		// 优先使用amount字段，如果没有则使用totalGoodsValue，最后使用actualShippingAmount
		const amount = parseFloat(row.amount) || parseFloat(row.totalGoodsValue) || parseFloat(row.actualShippingAmount) || 0;
		// 本次申请金额 + 已申请金额不能超过金额
		return (currentPaymentAmount + appliedAmount) > amount;
	},

	// 检查是否有超过金额的情况
	hasExceededAmount() {
		return CostDetailsTbaleData.value.some(row => this.isPaymentAmountExceeded(row));
	},

	// 验证付款金额（完整验证，包含错误提示）
	validate() {
		// 只有工厂付款才需要验证付款明细合计金额
		if (!this.isFactoryPayment()) {
			return true;
		}

		const totalPaymentAmount = this.getTotalPaymentAmount();
		const applicationAmount = this.getApplicationAmount();

		// 检查是否有超过金额的情况（本次申请金额 + 已申请金额 > 金额）
		if (this.hasExceededAmount()) {
			ElMessage.error('存在本次申请金额+已申请金额超过金额的情况，请检查');
			return false;
		}

		// 本次付款金额合计不能超过申请金额，但可以少于申请金额
		if (totalPaymentAmount > applicationAmount) {
			ElMessage.error(`付款明细总金额(${totalPaymentAmount.toFixed(2)})不能超过申请金额(${applicationAmount.toFixed(2)})`);
			return false;
		}

		return true;
	},

	// 检查金额是否匹配（用于控制提交按钮，不显示错误提示）
	isMatched() {
		// 只有工厂付款才需要验证付款明细合计金额
		if (!this.isFactoryPayment()) {
			return true;
		}

		const totalPayment = this.getTotalPaymentAmount();
		const applicationAmount = this.getApplicationAmount();

		// 检查是否有超过金额的情况
		const hasExceeded = this.hasExceededAmount();

		// 本次付款金额合计不能超过申请金额，但可以少于申请金额
		return totalPayment <= applicationAmount && !hasExceeded;
	},

	// 获取付款金额警告类型
	getAlertType() {
		// 只有工厂付款才需要验证付款明细合计金额
		if (!this.isFactoryPayment()) {
			return 'info';
		}

		const totalPayment = this.getTotalPaymentAmount();
		const applicationAmount = this.getApplicationAmount();

		if (totalPayment === 0) {
			return 'info';
		} else if (totalPayment > applicationAmount) {
			return 'error';
		} else if (totalPayment < applicationAmount) {
			return 'warning';
		} else {
			return 'success';
		}
	}
};

// 验证付款金额
const validatePaymentAmount = () => {
	return PaymentAmountValidator.validate();
};

// 检查本次付款金额是否超过金额
const isPaymentAmountExceeded = (row) => {
	return PaymentAmountValidator.isPaymentAmountExceeded(row);
};

// 检查金额是否匹配（用于控制提交按钮）
const isAmountMatched = () => {
	return PaymentAmountValidator.isMatched();
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
	return PaymentAmountValidator.getTotalPaymentAmount().toFixed(2);
};

// 获取付款金额警告类型
const getPaymentAmountAlertType = () => {
	return PaymentAmountValidator.getAlertType();
};



// 从付款明细中移除项目时，重新添加到未支付款项列表中
const CostDetailsTbaleDatahandleDelete = (index: number) => {
	const deletedItem = CostDetailsTbaleData.value[index];

	// 如果删除的项目来自未付款项，重新添加到未支付款项列表中
	if (deletedItem.expenseName && deletedItem.relatedDocumentsNo) {
		// 检查是否已经存在于未支付款项列表中
		const alreadyExists = UnpaidDetailsTbaleData.value.some(item =>
			item.expenseName === deletedItem.expenseName &&
			item.relatedDocumentsNo === deletedItem.relatedDocumentsNo &&
			item.relatedDocumentType === deletedItem.relatedDocumentType
		);

		if (!alreadyExists) {
			// 从Map中查找原始未支付款项数据（包含接口返回的appliedAmount）
			const key = `${deletedItem.expenseName}_${deletedItem.relatedDocumentsNo}_${deletedItem.relatedDocumentType}`;
			const originalItem = originalUnpaidItemsMap.value.get(key);
			// 如果找到原始数据，使用原始数据；否则使用createOriginalUnpaidItem创建
			const itemToAdd = originalItem ? { ...originalItem } : PaymentDetailsHandler.createOriginalUnpaidItem(deletedItem);
			UnpaidDetailsTbaleData.value.push(itemToAdd);
		}

		// 从已选择列表中移除
		selectedUnpaidItemIds.value.delete(deletedItem.relatedDocumentID || deletedItem.id);
	}

	CostDetailsTbaleData.value.splice(index, 1);
	CalculatetotalAmount(); // 重新计算总金额
};

// 款项名称变化时，若需展示付款明细，则按当前供应商和款项类型重新拉取未支付款项详情
const paymentNameChange = async () => {
	// 清空关联合同字段，因为款项名称变化可能导致合同列表变化
	addpaymentrequestform.value.relatedContract = '';
	contractList.value = [];

	// 清空收寄样列表
	sampleCollectionTableData.value = [];
	showSampleCollection.value = false;
	sampleCollectionTotalItems.value = 0;
	sampleCollectionCurrentPage.value = 1;

	// 清空已选择收寄样列表
	selectedSampleCollectionTableData.value = [];
	showSelectedSampleCollection.value = false;
	selectedSampleCollectionTotalItems.value = 0;
	selectedSampleCollectionCurrentPage.value = 1;

	// 业务费用收款单位选项动态获取逻辑
	if (addpaymentrequestform.value.paymentCategory === '5') { // 业务费用
		await loadBusinessExpensePayeeOptions();
		// 如果是业务费用且款项名称是快递费，且已选择收款单位，则加载收寄样列表
		if (isBusinessExpenseWithExpressFee() && addpaymentrequestform.value.payeeCode) {
			await loadSampleCollectionList(addpaymentrequestform.value.payeeCode);
		}
	}

	// 日常费用收款单位选项动态获取逻辑
	if (addpaymentrequestform.value.paymentCategory === '4') { // 日常费用
		await loadDailyExpensePayeeOptions();
		// 如果是客户事宜，加载客户选项
		if (isDailyExpenseWithCustomerOptions()) {
			await loadCustomerOptions();
		}
	}

	if (!showPaymentDetails.value) {
		return;
	}
	if (!addpaymentrequestform.value.payeeCode || addpaymentrequestform.value.payeeCode === '') {
		UnpaidDetailsTbaleData.value = [];
		return;
	}
	try {
		const unpaidResponse = await request({
			url: 'UnpaidAmount/GetUnpaidAmountList/GetList',
			method: 'GET',
			params: {
				supplierID: addpaymentrequestform.value.payeeCode
			}
		});
		if (unpaidResponse && unpaidResponse.code === 200) {
			UnpaidDetailsTbaleData.value = unpaidResponse.data || [];
			// 保存原始未支付款项数据到Map，用于恢复时使用
			UnpaidDetailsTbaleData.value.forEach((element) => {
				const key = `${element.expenseName}_${element.relatedDocumentsNo}_${element.relatedDocumentType}`;
				originalUnpaidItemsMap.value.set(key, { ...element });
			});
			// 新API数据结构不需要处理contractStatus，直接使用返回的数据
		} else {
			UnpaidDetailsTbaleData.value = [];
		}
	} catch (error) {
		console.error('获取未付款详情失败:', error);
		UnpaidDetailsTbaleData.value = [];
	}
};

// 统一的收款单位选项加载工具函数
const PayeeOptionsLoader = {
	// 根据付款类别和款项名称确定公司类型
	determineCompanyType(paymentCategory, paymentName) {
		// 获取当前选择的款项名称标签
		const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
			option.dictValue === paymentName
		)?.dictLabel || '';

		// 业务费用
		if (paymentCategory === '5') {
			if (currentPaymentNameLabel.includes('快递费') || currentPaymentNameLabel.includes('快递')) {
				return 2; // 快递公司
			} else if (currentPaymentNameLabel.includes('运费') && !currentPaymentNameLabel.includes('海运费')) {
				return 3; // 物流公司
			} else if (currentPaymentNameLabel.includes('海运费') || currentPaymentNameLabel.includes('运杂费')) {
				return 1; // 货代公司
			} else if (currentPaymentNameLabel.includes('佣金') || currentPaymentNameLabel.includes('其它') || currentPaymentNameLabel.includes('其他')) {
				return 4; // 客户
			}
		}
		// 日常费用
		else if (paymentCategory === '4') {
			if (currentPaymentNameLabel.includes('客户事宜')) {
				return 4; // 客户
			}
		}

		return 0; // 默认收款单位
	},

	// 加载收款单位选项
	async loadOptions(paymentCategory, paymentName, isViewMode = false) {
		const companyType = this.determineCompanyType(paymentCategory, paymentName);

		// 如果不需要特殊处理，加载默认供应商选项
		if (companyType === 0) {
			if (!isViewMode) {
				await loadFilteredSuppliers();
			}
			return;
		}

		try {
			// 调用API获取对应的收款单位选项
			const response = await request({
				url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect',
				method: 'GET',
				params: {
					companyType: companyType
				}
			});

			ApiRequestHandler.handleResponse(
				response,
				(data) => {
					// 更新收款单位选项列表，标准化数据结构
					filteredSupplierList.value = ApiRequestHandler.standardizeData(data);

					// 如果不是查看模式，清空当前选择的收款单位
					if (!isViewMode) {
						addpaymentrequestform.value.payeeCode = '';
						addpaymentrequestform.value.payeeName = '';
						addpaymentrequestform.value.bankName = '';
						addpaymentrequestform.value.bankAccount = '';
						supplierBankAccounts.value = [];

						ElMessage.success(`已加载${getCompanyTypeName(companyType)}选项`);
					}
				},
				() => {
					ElMessage.error('获取收款单位选项失败');
					// 如果获取失败，加载默认供应商选项
					if (!isViewMode) {
						loadFilteredSuppliers();
					}
				}
			);
		} catch (error) {
			ApiRequestHandler.handleError(error, () => {
				ElMessage.error('获取收款单位选项失败，请重试');
				// 如果获取失败，加载默认供应商选项
				if (!isViewMode) {
					loadFilteredSuppliers();
				}
			});
		}
	}
};

// 业务费用收款单位选项动态获取函数
const loadBusinessExpensePayeeOptions = async () => {
	await PayeeOptionsLoader.loadOptions('5', addpaymentrequestform.value.paymentName);
};

// 日常费用收款单位选项动态获取函数
const loadDailyExpensePayeeOptions = async () => {
	await PayeeOptionsLoader.loadOptions('4', addpaymentrequestform.value.paymentName);
};

// 加载客户选项
const loadCustomerOptions = async () => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerDataByUserID/GetSelectCustomerDataByUserID',
			method: 'GET'
		});

		// 返回Promise，确保数据设置完成后再resolve
		return new Promise((resolve, reject) => {
			if (response && response.code === 200) {
				const data = response.data || [];

				if (data.length > 0) {
					// 更新客户选项列表，标准化数据结构
					customerOptions.value = ApiRequestHandler.standardizeData(data);
					resolve(customerOptions.value);
				} else {
					// API 返回空数组，尝试使用字典数据作为备选
					if (state.optionss.sql_hr_customer_abbreviation && state.optionss.sql_hr_customer_abbreviation.length > 0) {
						customerOptions.value = ApiRequestHandler.standardizeData(state.optionss.sql_hr_customer_abbreviation);
						resolve(customerOptions.value);
					} else {
						customerOptions.value = [];
						resolve(customerOptions.value);
					}
				}
			} else {
				// API 失败时，尝试使用字典数据作为备选
				if (state.optionss.sql_hr_customer_abbreviation && state.optionss.sql_hr_customer_abbreviation.length > 0) {
					customerOptions.value = ApiRequestHandler.standardizeData(state.optionss.sql_hr_customer_abbreviation);
					resolve(customerOptions.value);
				} else {
					customerOptions.value = [];
					reject(response?.msg || 'API请求失败');
				}
			}
		});
	} catch (error) {
		// 出错时，尝试使用字典数据作为备选
		if (state.optionss.sql_hr_customer_abbreviation && state.optionss.sql_hr_customer_abbreviation.length > 0) {
			customerOptions.value = ApiRequestHandler.standardizeData(state.optionss.sql_hr_customer_abbreviation);
			return Promise.resolve(customerOptions.value);
		}
		customerOptions.value = [];
		throw error;
	}
};

// 格式化日期
const formatDate = (dateString) => {
	if (!dateString) return '';
	try {
		const date = new Date(dateString);
		const year = date.getFullYear();
		const month = String(date.getMonth() + 1).padStart(2, '0');
		const day = String(date.getDate()).padStart(2, '0');
		return `${year}-${month}-${day}`;
	} catch (error) {
		return dateString;
	}
};

// 加载收寄样列表
const loadSampleCollectionList = async (expressCompanyId) => {
	if (!expressCompanyId) {
		sampleCollectionTableData.value = [];
		showSampleCollection.value = false;
		return;
	}

	try {
		const response = await request({
			url: 'ProductSample/GetProductSampleSpecialList/GetList',
			method: 'GET',
			params: {
				PageNum: 1,
				PageSize: 1000, // 获取所有收寄样记录
				ExpressCompany: expressCompanyId // 根据快递公司ID过滤
			}
		});

		if (response && response.code === 200) {
			// 处理收寄样列表数据
			let data = response.data.result || [];

			// 如果后端不支持ExpressCompany参数，则在前端过滤
			if (data.length > 0 && data.some(item => item.express_Company != expressCompanyId)) {
				data = data.filter(item => item.express_Company == expressCompanyId);
			}
			sampleCollectionTableData.value = data.map(item => {
				// 转换寄样/收样类型
				const type = item.type === 1 ? '寄样' : '收样';
				// 转换客户/供应商类型
				const customerOrSupplier = item.customer_or_Supplier === 1 ? '客户' : '供应商';
				// 转换客户ID为名称（如果存在）
				let customerIdLabel = '';
				if (item.customer_ID && item.customer_ID !== 0) {
					const customer = state.optionss.sql_hr_customer_abbreviation?.find(
						c => c.dictValue === item.customer_ID.toString()
					);
					customerIdLabel = customer ? customer.dictLabel : item.customer_ID.toString();
				}
				// 转换快递公司ID为名称（如果存在）
				let expressCompanyLabel = '';
				if (item.express_Company && item.express_Company !== 0) {
					const company = state.optionss.hr_express_delivery_company?.find(
						c => c.dictValue === item.express_Company.toString()
					);
					expressCompanyLabel = company ? company.dictLabel : item.express_Company.toString();
				}
				// 转换付费方式
				let paymentMethodLabel = '';
				if (item.payment_Method && item.payment_Method !== 0) {
					const method = state.optionss.hr_express_payment_method?.find(
						m => m.dictValue === item.payment_Method.toString()
					);
					paymentMethodLabel = method ? method.dictLabel : '';
				}
				// 转换我方公司
				let companyIdLabel = '';
				if (item.company_ID && item.company_ID !== 0) {
					const company = state.optionss.hr_ourcompany?.find(
						c => c.dictValue === item.company_ID.toString()
					);
					companyIdLabel = company ? company.dictLabel : '';
				}
				// 转换销售合同ID为合同编号
				let relatedContractNumber = '';
				if (item.relatedContractID && item.relatedContractID !== 0) {
					const contract = state.optionss.sql_sale_contracts?.find(
						c => c.dictValue === item.relatedContractID.toString()
					);
					relatedContractNumber = contract ? contract.dictLabel : '';
				}
				// 转换出运合同ID为合同编号
				let relatedShippingNumber = '';
				if (item.relatedShippingContractsID && item.relatedShippingContractsID !== 0) {
					const shipping = state.optionss.sql_shippingdeliveries?.find(
						s => s.dictValue === item.relatedShippingContractsID.toString()
					);
					relatedShippingNumber = shipping ? shipping.dictLabel : '';
				}

				return {
					sampleID: item.id || item.ID || item.sampleID || 0, // 保存原始的样品ID
					type: type,
					customer_or_Supplier: customerOrSupplier,
					customer_ID: customerIdLabel,
					waybill_Number: item.waybill_Number || '',
					express_Company: expressCompanyLabel,
					sample_Date: formatDate(item.sample_Date),
					payment_Method: paymentMethodLabel,
					company_ID: companyIdLabel,
					paid_Express_Fee: (item.paid_Express_Fee || 0).toFixed(2),
					relatedContractID: relatedContractNumber, // 保存合同编号（用于显示）
					relatedShippingContractsID: relatedShippingNumber, // 保存运编号（用于显示）
					relatedContractIDOriginal: item.relatedContractID || item.relatedContractId || null, // 保存原始的合同ID
					relatedShippingContractsIDOriginal: item.relatedShippingContractsID || item.relatedShippingContractsId || null // 保存原始的出运合同ID
				};
			});

			// 过滤掉已选择的收寄样记录
			if (selectedSampleCollectionTableData.value.length > 0) {
				sampleCollectionTableData.value = sampleCollectionTableData.value.filter(item => {
					return !selectedSampleCollectionTableData.value.some(selected =>
						selected.waybill_Number === item.waybill_Number &&
						selected.customer_ID === item.customer_ID &&
						selected.sample_Date === item.sample_Date
					);
				});
			}

			// watch会自动更新总数和分页，这里只需要确保数据正确

			// 如果有数据，显示收寄样列表
			showSampleCollection.value = sampleCollectionTableData.value.length > 0;
		} else {
			sampleCollectionTableData.value = [];
			showSampleCollection.value = false;
			sampleCollectionTotalItems.value = 0;
			sampleCollectionCurrentPage.value = 1;
			selectedSampleCollectionTableData.value = [];
			showSelectedSampleCollection.value = false;
			selectedSampleCollectionTotalItems.value = 0;
			selectedSampleCollectionCurrentPage.value = 1;
		}
	} catch (error) {
		console.error('获取收寄样列表失败:', error);
		sampleCollectionTableData.value = [];
		showSampleCollection.value = false;
		sampleCollectionTotalItems.value = 0;
		sampleCollectionCurrentPage.value = 1;
		selectedSampleCollectionTableData.value = [];
		showSelectedSampleCollection.value = false;
		selectedSampleCollectionTotalItems.value = 0;
		selectedSampleCollectionCurrentPage.value = 1;
	}
};

// 获取公司类型名称
const getCompanyTypeName = (companyType) => {
	switch (companyType) {
		case 1: return '货代公司';
		case 2: return '快递公司';
		case 3: return '物流公司';
		case 4: return '客户';
		default: return '收款单位';
	}
};

// 获取当前付款申请的公司类型
const getCurrentCompanyType = () => {
	// 只有业务费用才需要设置公司类型
	if (addpaymentrequestform.value.paymentCategory !== '5') {
		return 0; // 默认收款单位
	}

	const paymentName = addpaymentrequestform.value.paymentName;

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === paymentName
	)?.dictLabel || '';

	// 根据款项名称标签确定公司类型
	if (currentPaymentNameLabel.includes('快递费') || currentPaymentNameLabel.includes('快递')) {
		return 2; // 快递公司
	} else if (currentPaymentNameLabel.includes('运费') && !currentPaymentNameLabel.includes('海运费')) {
		return 3; // 物流公司
	} else if (currentPaymentNameLabel.includes('海运费') || currentPaymentNameLabel.includes('运杂费')) {
		return 1; // 货代公司
	} else if (currentPaymentNameLabel.includes('佣金')) {
		return 4; // 客户
	} else {
		return 0; // 默认收款单位
	}
};

// 判断是否为物流相关款项名称（快递费、运费、运杂费、海运费）
const isLogisticsPaymentName = () => {
	const paymentName = addpaymentrequestform.value.paymentName;

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === paymentName
	)?.dictLabel || '';

	// 判断是否为快递费、运费、运杂费、海运费
	return currentPaymentNameLabel.includes('快递费') ||
		currentPaymentNameLabel.includes('运费') ||
		currentPaymentNameLabel.includes('运杂费') ||
		currentPaymentNameLabel.includes('海运费');
};

// 判断是否为"其他"款项名称
const isOtherPaymentName = () => {
	const paymentName = addpaymentrequestform.value.paymentName;

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === paymentName
	)?.dictLabel || '';

	// 判断是否为"其他"或类似的通用款项名称
	return currentPaymentNameLabel.includes('其他') ||
		currentPaymentNameLabel.includes('杂费') ||
		currentPaymentNameLabel.includes('费用') ||
		currentPaymentNameLabel === '';
};

// 查看详情时判断是否为物流相关款项名称（快递费、运费、运杂费、海运费）
const isLogisticsPaymentNameForView = (paymentName) => {
	// 获取款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === paymentName
	)?.dictLabel || '';

	// 判断是否为快递费、运费、运杂费、海运费
	return currentPaymentNameLabel.includes('快递费') ||
		currentPaymentNameLabel.includes('运费') ||
		currentPaymentNameLabel.includes('运杂费') ||
		currentPaymentNameLabel.includes('海运费');
};

// 查看详情时判断是否为"其他"款项名称
const isOtherPaymentNameForView = (paymentName) => {
	// 获取款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === paymentName
	)?.dictLabel || '';

	// 判断是否为"其他"或类似的通用款项名称
	return currentPaymentNameLabel.includes('其他') ||
		currentPaymentNameLabel.includes('杂费') ||
		currentPaymentNameLabel.includes('费用') ||
		currentPaymentNameLabel === '';
};

// 查看详情时根据款项名称加载业务费用收款单位选项
const loadBusinessExpensePayeeOptionsForViewByPaymentName = async (paymentName) => {
	await PayeeOptionsLoader.loadOptions('5', paymentName, true);
};

// 查看详情时根据款项名称加载日常费用收款单位选项
const loadDailyExpensePayeeOptionsForViewByPaymentName = async (paymentName) => {
	await PayeeOptionsLoader.loadOptions('4', paymentName, true);
};

// 查看详情时加载业务费用收款单位选项（保留原函数以兼容其他调用）
const loadBusinessExpensePayeeOptionsForView = async (companyType) => {
	try {
		// 调用API获取对应的收款单位选项
		const response = await request({
			url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect',
			method: 'GET',
			params: {
				companyType: companyType
			}
		});

		if (response && response.code === 200) {
			// 更新收款单位选项列表，标准化数据结构
			const data = response.data || [];
			filteredSupplierList.value = data.map(item => ({
				dictvalue: item.dictvalue || item.dictValue || item.value,
				dictLabel: item.dictLabel || item.label || item.name
			}));
		} else {
			// 如果获取失败，加载默认供应商选项
			await loadFilteredSuppliers();
		}
	} catch (error) {
		console.error('获取收款单位选项失败:', error);
		// 如果获取失败，加载默认供应商选项
		await loadFilteredSuppliers();
	}
};

// 判断是否为日常费用且需要手动输入的情况
const isDailyExpenseWithManualInput = () => {
	// 检查是否为日常费用类别
	if (addpaymentrequestform.value.paymentCategory !== '4') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 判断是否为市场开发、产品开发、其它费用
	return currentPaymentNameLabel.includes('市场开发') ||
		currentPaymentNameLabel.includes('产品开发') ||
		currentPaymentNameLabel.includes('其它费用') ||
		currentPaymentNameLabel.includes('其他费用');
};

// 判断是否为日常费用且需要客户选项的情况
const isDailyExpenseWithCustomerOptions = () => {
	// 检查是否为日常费用类别
	if (addpaymentrequestform.value.paymentCategory !== '4') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 判断是否为客户事宜
	return currentPaymentNameLabel.includes('客户事宜');
};

// 判断是否为客户事宜且需要收款单位输入框的情况
const isCustomerMatterWithManualInput = () => {
	// 检查是否为日常费用类别
	if (addpaymentrequestform.value.paymentCategory !== '4') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 判断是否为客户事宜
	return currentPaymentNameLabel.includes('客户事宜');
};

// 判断是否为业务费用且需要关联合同的情况
const isBusinessExpenseWithContract = () => {
	// 检查是否为业务费用类别
	if (addpaymentrequestform.value.paymentCategory !== '5') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 判断是否为其它款项名称
	return currentPaymentNameLabel.includes('其它') || currentPaymentNameLabel.includes('其他');
};

// 判断是否需要手动输入银行信息
const isManualBankInput = () => {
	// 检查是否为业务费用或日常费用类别
	if (addpaymentrequestform.value.paymentCategory !== '5' && addpaymentrequestform.value.paymentCategory !== '4') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 业务费用：其它款项名称或佣金
	if (addpaymentrequestform.value.paymentCategory === '5') {
		return currentPaymentNameLabel.includes('其它') ||
			currentPaymentNameLabel.includes('其他') ||
			currentPaymentNameLabel.includes('佣金');
	}

	// 日常费用：客户事宜
	if (addpaymentrequestform.value.paymentCategory === '4') {
		return currentPaymentNameLabel.includes('客户事宜');
	}

	return false;
};

// 判断是否为业务费用且款项名称是快递费
const isBusinessExpenseWithExpressFee = () => {
	// 检查是否为业务费用类别
	if (addpaymentrequestform.value.paymentCategory !== '5') {
		return false;
	}

	// 获取当前选择的款项名称标签
	const currentPaymentNameLabel = PaymentTypeOptions.value.find(option =>
		option.dictValue === addpaymentrequestform.value.paymentName
	)?.dictLabel || '';

	// 判断是否为快递费
	return currentPaymentNameLabel.includes('快递费') || currentPaymentNameLabel.includes('快递');
};

// 处理选择收寄样
const handleSelectSampleCollection = async (row) => {
	// 检查是否已经存在于已选择列表中
	const alreadyExists = selectedSampleCollectionTableData.value.some(item =>
		item.waybill_Number === row.waybill_Number &&
		item.customer_ID === row.customer_ID &&
		item.sample_Date === row.sample_Date
	);

	if (alreadyExists) {
		ElMessage.warning('该收寄样记录已存在，不能重复选择');
		return;
	}

	// 添加到已选择列表，确保已付快件费有默认值，并保存原始值
	const selectedRow = { ...row };
	// 保存原始的已付快件费值，用于删除时恢复（如果不存在才保存）
	if (selectedRow.originalPaidExpressFee === undefined) {
		selectedRow.originalPaidExpressFee = selectedRow.paid_Express_Fee || '0.00';
	}

	if (!selectedRow.paid_Express_Fee || selectedRow.paid_Express_Fee === '') {
		selectedRow.paid_Express_Fee = '0.00';
	} else {
		// 格式化已付快件费为两位小数
		const feeValue = parseFloat(selectedRow.paid_Express_Fee) || 0;
		selectedRow.paid_Express_Fee = feeValue.toFixed(2);
	}

	// 处理关联合同号和关联运编号：如果有已有数据，则赋值并标记为不可编辑
	if (selectedRow.relatedContractID && selectedRow.relatedContractID !== '') {
		selectedRow.relatedContractNumber = selectedRow.relatedContractID; // 使用合同编号显示
		selectedRow.hasExistingContract = true;
	} else {
		selectedRow.relatedContractNumber = '';
		selectedRow.hasExistingContract = false;
	}

	if (selectedRow.relatedShippingContractsID && selectedRow.relatedShippingContractsID !== '') {
		selectedRow.relatedShippingNumber = selectedRow.relatedShippingContractsID; // 使用运编号显示
		selectedRow.hasExistingShipping = true;
	} else {
		selectedRow.relatedShippingNumber = '';
		selectedRow.hasExistingShipping = false;
	}

	// 初始化出运合同选项列表
	selectedRow.shippingContractOptions = [];

	// 如果单据有销售合同（relatedContractIDOriginal存在），自动加载对应的出运编号列表
	if (selectedRow.relatedContractIDOriginal && selectedRow.relatedContractIDOriginal !== 0 && selectedRow.relatedContractIDOriginal !== null) {
		// 使用合同ID直接加载出运编号列表
		await loadShippingContractsByContractId(
			selectedRow,
			selectedRow.relatedContractIDOriginal,
			selectedRow.customer_or_Supplier
		);
	}
	// 如果有关联合同号但没有已有合同，加载对应的出运合同列表
	else if (selectedRow.relatedContractNumber && !selectedRow.hasExistingContract) {
		loadShippingContractsByContractNumber(
			selectedRow,
			selectedRow.relatedContractNumber,
			selectedRow.customer_or_Supplier
		);
	}

	selectedSampleCollectionTableData.value.push(selectedRow);

	// 从未支付收寄样列表中移除（watch会自动更新总数和分页）
	const index = sampleCollectionTableData.value.findIndex(item =>
		item.waybill_Number === row.waybill_Number &&
		item.customer_ID === row.customer_ID &&
		item.sample_Date === row.sample_Date
	);
	if (index > -1) {
		sampleCollectionTableData.value.splice(index, 1);
	}

	// 显示已选择收寄样列表
	if (selectedSampleCollectionTableData.value.length > 0) {
		showSelectedSampleCollection.value = true;
	}

	// 重新计算申请金额
	calculateTotalPaidExpressFee();

	ElMessage.success('已添加到已选择收寄样列表');
};

// 处理移除已选择的收寄样
const handleRemoveSelectedSampleCollection = (row) => {
	// 从已选择列表中移除
	const index = selectedSampleCollectionTableData.value.findIndex(item =>
		item.waybill_Number === row.waybill_Number &&
		item.customer_ID === row.customer_ID &&
		item.sample_Date === row.sample_Date
	);
	if (index > -1) {
		selectedSampleCollectionTableData.value.splice(index, 1);

		// 恢复记录到未支付收寄样列表，恢复原始的已付快件费值
		const restoredRow = { ...row };
		// 如果有保存的原始值，则使用原始值；否则使用当前值
		if (restoredRow.originalPaidExpressFee !== undefined) {
			restoredRow.paid_Express_Fee = restoredRow.originalPaidExpressFee;
		}
		// 移除临时保存的原始值字段
		delete restoredRow.originalPaidExpressFee;
		sampleCollectionTableData.value.push(restoredRow);

		// 如果已选择列表为空，隐藏折叠面板
		if (selectedSampleCollectionTableData.value.length === 0) {
			showSelectedSampleCollection.value = false;
		}

		// 重新计算申请金额
		calculateTotalPaidExpressFee();

		ElMessage.success('已从已选择收寄样列表中移除');
	}
};

// 处理已付快件费输入（只允许数字和小数点，不允许负数）
const handlePaidExpressFeeInput = (row, value) => {
	// 只允许数字和小数点（不允许负数）
	const regex = /^\d*\.?\d*$/;
	if (value === '' || regex.test(value)) {
		// 限制小数点后最多两位
		const parts = value.split('.');
		if (parts.length > 1 && parts[1].length > 2) {
			row.paid_Express_Fee = parts[0] + '.' + parts[1].substring(0, 2);
		} else {
			row.paid_Express_Fee = value;
		}
	} else {
		// 如果输入不合法，恢复为上一个合法值
		const lastValidValue = parseFloat(row.paid_Express_Fee);
		if (isNaN(lastValidValue) || lastValidValue < 0) {
			row.paid_Express_Fee = '0.00';
		} else {
			row.paid_Express_Fee = lastValidValue.toFixed(2);
		}
	}
};

// 处理已付快件费失焦（格式化并计算总和）
const handlePaidExpressFeeChange = (row) => {
	// 格式化数值，保留两位小数
	const value = parseFloat(row.paid_Express_Fee);
	if (isNaN(value) || value < 0) {
		row.paid_Express_Fee = '0.00';
	} else {
		row.paid_Express_Fee = value.toFixed(2);
	}
	// 重新计算申请金额
	calculateTotalPaidExpressFee();
};

// 计算已选择收寄样单据中已付快件费的总和并更新申请金额
const calculateTotalPaidExpressFee = () => {
	let total = 0;
	selectedSampleCollectionTableData.value.forEach(row => {
		// 确保字段值有效，如果无效则设为0
		let fee = parseFloat(row.paid_Express_Fee);
		if (isNaN(fee) || fee < 0) {
			fee = 0;
			row.paid_Express_Fee = '0.00';
		}
		total += fee;
	});
	// 更新申请金额，保留两位小数
	addpaymentrequestform.value.totalAmount = total.toFixed(2);
};

// 处理手动输入收款单位时的逻辑
const handleManualPayeeInput = () => {
	// 如果收款单位名称被清空，清空相关字段
	if (!addpaymentrequestform.value.payeeName || addpaymentrequestform.value.payeeName.trim() === '') {
		// 清空开户银行
		addpaymentrequestform.value.bankName = '';
		// 清空银行账号
		addpaymentrequestform.value.bankAccount = '';
		// 清空供应商银行账号列表
		supplierBankAccounts.value = [];
		// 清空未支付款项详情
		UnpaidDetailsTbaleData.value = [];
		// 清空原始未支付款项数据Map
		originalUnpaidItemsMap.value.clear();
		// 清空已选择的未付款项
		selectedUnpaidItemIds.value.clear();
		return;
	}

	// 当手动输入收款单位时，清空相关的银行信息，让用户手动输入
	if (isDailyExpenseWithManualInput() || isManualBankInput() || isCustomerMatterWithManualInput()) {
		// 清空银行账号下拉选项
		supplierBankAccounts.value = [];
		// 如果用户没有手动输入银行信息，则清空
		// 这里不自动清空，让用户自己决定是否输入
	}
};

// 根据合同ID和客户/供应商类型加载出运合同列表
const loadShippingContractsByContractId = async (row, contractId, customerOrSupplier) => {
	if (!contractId || contractId === 0) {
		// 如果合同ID为空，清空出运合同列表
		row.shippingContractOptions = [];
		row.relatedShippingNumber = '';
		return;
	}

	// 根据客户/供应商类型确定单据类型
	// 客户 -> 销售合同 (DocumentType = 1)
	// 供应商 -> 采购合同 (DocumentType = 2)
	const documentType = customerOrSupplier === '客户' ? 1 : 2;

	try {
		const response = await request({
			url: 'ShippingDeliveries/GetShippingContractSelectListByDocumentType/GetSelectListByDocumentType',
			method: 'GET',
			params: {
				DocumentType: documentType,
				DocumentID: Number(contractId)
			}
		});

		if (response && response.code === 200) {
			// 标准化数据结构
			row.shippingContractOptions = (response.data || []).map(item => ({
				dictvalue: String(item.dictValue || item.dictvalue || item.id || item.Id || item.ID),
				dictLabel: item.dictLabel || item.dictlabel || item.invoiceNumber || item.InvoiceNumber || ''
			}));
		} else {
			row.shippingContractOptions = [];
		}
	} catch (error) {
		console.error('获取出运编号列表失败:', error);
		row.shippingContractOptions = [];
	}
};

// 根据关联合同号和客户/供应商类型加载出运合同列表
const loadShippingContractsByContractNumber = async (row, contractNumber, customerOrSupplier) => {
	if (!contractNumber || contractNumber === '') {
		// 如果关联合同号为空，清空出运合同列表
		row.shippingContractOptions = [];
		row.relatedShippingNumber = '';
		return;
	}

	// 根据客户/供应商类型确定单据类型
	// 客户 -> 销售合同 (DocumentType = 1)
	// 供应商 -> 采购合同 (DocumentType = 2)
	const documentType = customerOrSupplier === '客户' ? 1 : 2;

	// 从合同编号找到对应的合同ID
	let contractId = null;
	if (documentType === 1) {
		// 销售合同：从applicantSaleContracts中查找
		const contract = applicantSaleContracts.value.find(c => c.dictLabel === contractNumber);
		contractId = contract ? Number(contract.dictvalue) : null;
	} else {
		// 采购合同：从采购合同字典中查找
		const contract = state.optionss.sql_purchase_contract?.find(c =>
			c.dictLabel === contractNumber || c.dictlabel === contractNumber
		);
		contractId = contract ? Number(contract.dictValue || contract.dictvalue) : null;

		// 如果从字典中找不到，尝试从applicantSaleContracts中查找（兼容处理）
		if (!contractId) {
			const contractFromApplicant = applicantSaleContracts.value.find(c => c.dictLabel === contractNumber);
			contractId = contractFromApplicant ? Number(contractFromApplicant.dictvalue) : null;
		}
	}

	if (!contractId) {
		ElMessage.warning('未找到对应的合同ID');
		row.shippingContractOptions = [];
		return;
	}

	try {
		const response = await request({
			url: 'ShippingDeliveries/GetShippingContractSelectListByDocumentType/GetSelectListByDocumentType',
			method: 'GET',
			params: {
				DocumentType: documentType,
				DocumentID: contractId
			}
		});

		if (response && response.code === 200) {
			// 标准化数据结构
			row.shippingContractOptions = (response.data || []).map(item => ({
				dictvalue: String(item.dictValue || item.dictvalue || item.id || item.Id || item.ID),
				dictLabel: item.dictLabel || item.dictlabel || item.invoiceNumber || item.InvoiceNumber || ''
			}));
		} else {
			row.shippingContractOptions = [];
			ElMessage.warning('获取出运编号列表失败');
		}
	} catch (error) {
		console.error('获取出运编号列表失败:', error);
		row.shippingContractOptions = [];
		ElMessage.error('获取出运编号列表失败，请重试');
	}
};

// 处理关联合同号变化
const handleRelatedContractNumberChange = async (row, contractNumber) => {
	// 获取客户/供应商类型
	const customerOrSupplier = row.customer_or_Supplier || '';

	// 清空关联运编号
	row.relatedShippingNumber = '';

	// 加载对应的出运合同列表
	await loadShippingContractsByContractNumber(row, contractNumber, customerOrSupplier);
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

//本次申请金额输入
const handleCurrentPaymentAmountInput = (row) => {
	const currentPaymentAmount = parseFloat(row.currentPaymentAmount) || 0;
	const appliedAmount = parseFloat(row.appliedAmount) || 0;
	// 优先使用amount字段，如果没有则使用totalGoodsValue，最后使用actualShippingAmount
	const amount = parseFloat(row.amount) || parseFloat(row.totalGoodsValue) || parseFloat(row.actualShippingAmount) || 0;

	// 验证：本次申请金额 + 已申请金额不能超过金额
	if ((currentPaymentAmount + appliedAmount) > amount) {
		ElMessage.warning(`本次申请金额(${currentPaymentAmount.toFixed(2)}) + 已申请金额(${appliedAmount.toFixed(2)}) = ${(currentPaymentAmount + appliedAmount).toFixed(2)}，超过了金额(${amount.toFixed(2)})，请调整`);
		// 自动调整为最大可申请金额
		const maxCurrentPaymentAmount = Math.max(0, amount - appliedAmount);
		row.currentPaymentAmount = maxCurrentPaymentAmount.toFixed(2);
	}

	// 重新计算总金额
	CalculatetotalAmount();
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
