<template>
	<div>
		<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="settlement_center_shipping" filterable placeholder="请选出运发货单号" clearable
				style="width: 15%">
				<el-option v-for="dict in optionss.sql_settlement_center_shipping" :key="dict.dictCode"
					:label="dict.dictLabel" :value="dict.dictValue" />
			</el-select>

			<el-button style="margin-left: 20px;" type="warning" @click="CloseSettlement"
				:disabled="isCloseButtonDisabled">关账</el-button>
			<!-- 状态显示 -->
			<el-tag size="large" style="margin-left: 20px;" v-if="hasQueried"
				:type="getSettlementStatusType(SettlementStatus)">
				{{ getSettlementStatusText(SettlementStatus) }}
			</el-tag>
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="handleSearch">查询</el-button>
				<el-button @click="handleReset">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;主要信息</span>
		</div>
		<el-divider> </el-divider>
		<el-form :model="shippingdeliveryform" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="发票号码">
						<el-input v-model="shippingdeliveryform.invoiceNumber" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="出运日期">
						<el-date-picker v-model="shippingdeliveryform.shippingDate" type="date" placeholder="请选择"
							disabled :style="{ width: '300px' }"></el-date-picker>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="出运状态">
						<el-input v-model="shippingdeliveryform.shippingStatus" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="销售合同">
						<el-input v-model="shippingdeliveryform.salesContract" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="客户合同">
						<el-input v-model="shippingdeliveryform.customerContract" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="客户名称">
						<el-input v-model="shippingdeliveryform.customerName" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="我方公司">
						<el-select v-model="shippingdeliveryform.ourCompany" filterable placeholder="请选择" disabled
							:style="{ width: '300px' }">
							<el-option v-for="item in optionss.ourCompany" :key="item.dictValue" :label="item.dictLabel"
								:value="item.dictValue" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="外销币种">
						<el-input v-model="shippingdeliveryform.foreignCurrency" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="汇率">
						<el-input v-model="shippingdeliveryform.exchangeRate" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="价格条款">
						<el-input v-model="shippingdeliveryform.priceTerms" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="结汇方式">
						<el-input v-model="shippingdeliveryform.settlementMethod" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应收汇日">
						<el-date-picker v-model="shippingdeliveryform.receivableDate" type="date" placeholder="请选择"
							disabled :style="{ width: '300px' }"></el-date-picker>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="销售员">
						<el-input v-model="shippingdeliveryform.salesperson" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="单证员">
						<el-input v-model="shippingdeliveryform.documentOfficer" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="是否结账">
						<el-checkbox v-model="shippingdeliveryform.isSettled" :style="{ width: '300px' }"></el-checkbox>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;客户收汇</span>
		</div>
		<el-divider></el-divider>
		<el-form :model="CustomerReceiptForm" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="货值合计">
						<el-input v-model="CustomerReceiptForm.totalValue" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="费用合计">
						<el-input v-model="CustomerReceiptForm.totalCost" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="索赔金额">
						<el-input v-model="CustomerReceiptForm.claimAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="应收货款">
						<el-input v-model="CustomerReceiptForm.totalReceivable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已收货款">
						<el-input v-model="CustomerReceiptForm.totalReceived" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已收货款￥">
						<el-input v-model="CustomerReceiptForm.totalReceivedCNY" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未收货款">
						<el-input v-model="CustomerReceiptForm.totalUnreceived" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;退税情况</span>
		</div>
		<el-divider> </el-divider>

		<el-form :model="TaxRefundForm" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="应退税￥">
						<el-input v-model="TaxRefundForm.totalRefund" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已退税￥">
						<el-input v-model="TaxRefundForm.totalRefunded" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="未退税￥">
						<el-input v-model="TaxRefundForm.totalUnrefunded" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="是否已退">
						<el-checkbox v-model="TaxRefundForm.isRefunded" disabled
							:style="{ width: '300px' }"></el-checkbox>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;国内费用</span>
		</div>
		<el-divider></el-divider>

		<el-form :model="DomesticFeesForm" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="前程运输">
						<el-input v-model="DomesticFeesForm.transportation" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应付运杂费￥">
						<el-input v-model="DomesticFeesForm.totalPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付运杂费">
						<el-input v-model="DomesticFeesForm.totalPaid" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未付运杂费">
						<el-input v-model="DomesticFeesForm.totalUnpaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="快件费用￥">
						<el-input v-model="DomesticFeesForm.expressFee" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="其它国内费用￥">
						<el-input v-model="DomesticFeesForm.otherDomesticCost" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;国外费用</span>
		</div>
		<el-divider> </el-divider>
		<el-form :model="ForeignExpensesForm" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="应付佣金">
						<el-input v-model="ForeignExpensesForm.commissionPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付佣金">
						<el-input v-model="ForeignExpensesForm.commissionPaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付佣金￥">
						<el-input v-model="ForeignExpensesForm.commissionPaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未付佣金">
						<el-input v-model="ForeignExpensesForm.commissionUnpaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="船代公司">
						<el-input v-model="ForeignExpensesForm.shippingAgentCompany" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="海运费币种">
						<el-input v-model="ForeignExpensesForm.shippingCurrency" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="海运费汇率">
						<el-input v-model="ForeignExpensesForm.shippingExchangeRate" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应付海运">
						<el-input v-model="ForeignExpensesForm.shippingPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付海运">
						<el-input v-model="ForeignExpensesForm.shippingPaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="已付海运￥">
						<el-input v-model="ForeignExpensesForm.shippingPaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="未付海运￥">
						<el-input v-model="ForeignExpensesForm.shippingUnpaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="其它国外费用￥">
						<el-input v-model="ForeignExpensesForm.otherForeignCost" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;其它信息</span>
		</div>
		<el-divider> </el-divider>
		<el-tabs v-model="activeName" class="demo-tabs" @tab-click="handleClick">
			<el-tab-pane label="工厂付款" name="factorypayment">
				<el-table :data="factorypaymenttableData" height="500">
					<el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
					<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
					<el-table-column prop="reviewStatusStr" label="审核状态" width="150">
						<template #default="scope">
							<el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
								{{ scope.row.reviewStatusStr }}
							</el-tag>
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
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="采购其它费用" name="PurchaseOtherTab">
				<el-table :data="PurchaseOtherCostsTableData">
					<el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
					<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
					<el-table-column prop="reviewStatusStr" label="审核状态" width="150">
						<template #default="scope">
							<el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
								{{ scope.row.reviewStatusStr }}
							</el-tag>
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
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="客户已收汇详情" name="customerreceipts">
				<el-table :data="CustomerReceiptsTableData">
					<el-table-column prop="ReceiptNumber" label="收汇单号" width="150"></el-table-column>
					<el-table-column prop="ReceiptBank" label="收汇银行" width="150"></el-table-column>
					<el-table-column prop="ReceiptDate" label="收汇日期" width="150"></el-table-column>
					<el-table-column prop="OurCompany" label="我方公司" width="150"></el-table-column>
					<el-table-column prop="FundsType" label="款项类型" width="150"></el-table-column>
					<el-table-column prop="ReceiptAmount" label="收汇金额" width="150"></el-table-column>
					<el-table-column prop="ReceiptCurrency" label="收汇币种" width="150"></el-table-column>
					<el-table-column prop="ExchangeRate" label="汇率" width="150"></el-table-column>
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="退税详情" name="taxrefund">
				<el-table :data="TaxRefundTableData">
					<el-table-column prop="ReceiptNumber" label="退税单编号" width="150"></el-table-column>
					<el-table-column prop="InvoiceNumber" label="发票号" width="150"></el-table-column>
					<el-table-column prop="RefundAmount" label="应退税额" width="150"></el-table-column>
					<el-table-column prop="ActualRefundAmount" label="实际退税额" width="150"></el-table-column>
					<el-table-column prop="IsRefunded" label="是否已退" width="150"></el-table-column>
					<el-table-column prop="RefundDate" label="退税日期" width="150"></el-table-column>
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="国内已付费用详情" name="Domesticpayments">
				<el-table :data="DomesticPaymentsDataTable">
					<el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
					<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
					<el-table-column prop="reviewStatusStr" label="审核状态" width="150">
						<template #default="scope">
							<el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
								{{ scope.row.reviewStatusStr }}
							</el-tag>
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
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="国外已付费用详情" name="internationalpayments">
				<el-table :data="ForeignExpensesDataTable">
					<el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
					<el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
					<el-table-column prop="reviewStatusStr" label="审核状态" width="150">
						<template #default="scope">
							<el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
								{{ scope.row.reviewStatusStr }}
							</el-tag>
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
				</el-table>
			</el-tab-pane>
		</el-tabs>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElTabs, UploadUserFile, UploadProps, ElMessage } from 'element-plus'
import request from '@/utils/request';

const hasQueried = ref(false)
// 计算属性：关账按钮是否禁用
const isCloseButtonDisabled = computed(() => {
	// 基本条件检查
	const hasSelectedShipping = Boolean(settlement_center_shipping.value);
	const hasQueriedData = hasQueried.value;
	const isOpen = SettlementStatus.value === SETTLEMENT_STATUS.OPEN;

	// 条件1：必须选择出运单且已查询数据
	if (!hasSelectedShipping || !hasQueriedData) {
		return true;
	}

	// 条件2：必须是未关账状态
	return !isOpen;
});

const settlement_center_shipping = ref('')
const activeName = ref('productinfo')
const handleClick = (tab: any, event: any) => {
	console.log(tab, event)
}
/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		sql_settlement_center_shipping: [],
		ourCompany: [],
		hr_shipping_status: [],
		hr_export_currency: [],
		hr_pricing_term: [],
		hr_settlement_way: [],
		sql_all_user: [],
		hr_payment_category: [],
		hr_factory_payment: [],
		hr_ourcompany: [],
		hr_currency_code: [],
		sql_hr_dept: [],
		hr_bank: [],
		funds_type: [],
		hr_domestic_charges: [],
		hr_foreign_charges: [],
		hr_freight_forwarding_company: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'sql_settlement_center_shipping' },
	{ dictType: 'ourCompany' },
	{ dictType: 'hr_shipping_status' },
	{ dictType: 'hr_export_currency' },
	{ dictType: 'hr_pricing_term' },
	{ dictType: 'hr_settlement_way' },
	{ dictType: 'sql_all_user' },
	{ dictType: 'hr_payment_category' },
	{ dictType: 'hr_factory_payment' },
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'hr_currency_code' },
	{ dictType: 'sql_hr_dept' },
	{ dictType: 'hr_bank' },
	{ dictType: 'funds_type' },
	{ dictType: 'hr_domestic_charges' },
	{ dictType: 'hr_foreign_charges' },
	{ dictType: 'hr_freight_forwarding_company' }
]
// 在组件加载时获取数据
onMounted(async () => {
	await fetchDataAndExecute()
})
async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
/*动态下拉框end*/

//出运发货单数据
const shippingdeliveryform = ref({
	invoiceNumber: '',
	shippingDate: '',
	shippingStatus: '',
	salesContract: '',
	customerContract: '',
	customerNumber: '',
	customerAbbreviation: '',
	customerName: '',
	ourCompany: '',
	settlementType: '',
	foreignCurrency: '',
	exchangeRate: '',
	priceTerms: '',
	settlementMethod: '',
	receivableDate: '',
	salesperson: '',
	documentOfficer: '',
	isSettled: false
})

//客户收汇
const CustomerReceiptForm = ref({
	totalValue: '',
	totalCost: '',
	claimAmount: '',
	totalReceivable: '',
	totalReceived: '',
	totalReceivedCNY: '',
	totalUnreceived: ''
})

//退税情况
const TaxRefundForm = ref({
	totalRefund: '',
	totalRefunded: '',
	totalUnrefunded: '',
	isRefunded: false
})

//国内费用
const DomesticFeesForm = ref({
	transportation: '',
	totalPayable: '',
	totalPaid: '',
	totalUnpaid: '',
	expressFee: '',
	otherDomesticCost: ''
})

//国外费用
const ForeignExpensesForm = ref({
	commissionPayable: '',
	commissionPaid: '',
	commissionPaidAmount: '',
	commissionUnpaid: '',
	insurancePayable: '',
	insurancePaid: '',
	insurancePaidAmount: '',
	insuranceUnpaid: '',
	shippingAgentCompany: '',  // 确保这个属性在初始化时就存在
	shippingCurrency: '',
	shippingExchangeRate: '',
	shippingPayable: '',
	shippingPaid: '',
	shippingPaidAmount: '',
	shippingUnpaidAmount: '',
	otherForeignCost: ''
})

const factorypaymenttableData = ref([]); //工厂付款表格数据
const PurchaseOtherCostsTableData = ref([]); //采购其它费用表格数据
const CustomerReceiptsTableData = ref([]); //客户收汇表格数据
const TaxRefundTableData = ref([]); //退税表格数据
const DomesticPaymentsDataTable = ref([]); //国内已付费用表格数据
const ForeignExpensesDataTable = ref([]); //国外已付费用表格数据
//重置
const handleReset = () => {
	// 重置查询状态
	hasQueried.value = false;
	// Reset the filter value
	settlement_center_shipping.value = ''
	// Reset all form values to empty state
	shippingdeliveryform.value = {
		invoiceNumber: '',
		shippingDate: '',
		shippingStatus: '',
		salesContract: '',
		customerContract: '',
		customerNumber: '',
		customerAbbreviation: '',
		customerName: '',
		ourCompany: '',
		settlementType: '',
		foreignCurrency: '',
		exchangeRate: '',
		priceTerms: '',
		settlementMethod: '',
		receivableDate: '',
		salesperson: '',
		documentOfficer: '',
		isSettled: ''
	}

	CustomerReceiptForm.value = {
		totalValue: '',
		totalCost: '',
		claimAmount: '',
		totalReceivable: '',
		totalReceived: '',
		totalReceivedCNY: '',
		totalUnreceived: ''
	}

	TaxRefundForm.value = {
		totalRefund: '',
		totalRefunded: '',
		totalUnrefunded: '',
		isRefunded: false
	}

	DomesticFeesForm.value = {
		transportation: '',
		totalPayable: '',
		totalPaid: '',
		totalUnpaid: '',
		expressFee: '',
		otherDomesticCost: ''
	}

	ForeignExpensesForm.value = {
		commissionPayable: '',
		commissionPaid: '',
		commissionPaidAmount: '',
		commissionUnpaid: '',
		insurancePayable: '',
		insurancePaid: '',
		insurancePaidAmount: '',
		insuranceUnpaid: '',
		shippingAgentCompany: '',
		shippingCurrency: '',
		shippingExchangeRate: '',
		shippingPayable: '',
		shippingPaid: '',
		shippingPaidAmount: '',
		shippingUnpaidAmount: '',
		otherForeignCost: ''
	}

	// Reset table data
	factorypaymenttableData.value = []//工厂付款
}
activeName.value = 'factorypayment'
//查询
const handleSearch = () => {
	// 重置查询状态
	hasQueried.value = false;
	factorypaymenttableData.value = [];
	PurchaseOtherCostsTableData.value = [];
	CustomerReceiptsTableData.value = [];
	TaxRefundTableData.value = [];
	if (settlement_center_shipping.value != '') {
		//#region 获取出运发货单数据
		request({
			url: 'ShippingDeliveries/GetShippingDeliveriesDetailsByid/GetShippingDeliveriesDetails',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200 && response.data.shippingDeliveries) {
				const data = response.data.shippingDeliveries;
				ForeignExpensesForm.value.shippingAgentCompany = state.optionss.hr_freight_forwarding_company.find(item => item.dictValue === data.shippingAgent.toString())?.dictLabel || '';
				shippingdeliveryform.value = {
					invoiceNumber: data.invoiceNumber || '',
					shippingDate: data.shippingDate ? data.shippingDate.split(' ')[0] : '', // Extract date part only
					shippingStatus: state.optionss.hr_shipping_status.find(item => item.dictValue === data.shippingStatus.toString())?.dictLabel || '',
					salesContract: data.salesContractNumber || '',
					customerContract: data.customerContractNumber || '',
					customerNumber: data.customerNumber || '',
					customerAbbreviation: data.customerAbbreviation || '',
					customerName: data.customerAbbreviation || '', // Using abbreviation as name since name isn't in response
					ourCompany: state.optionss.ourCompany.find(item => item.dictValue === data.ourCompany.toString())?.dictValue || '',
					settlementType: '', // Not in response
					foreignCurrency: state.optionss.hr_export_currency.find(item => item.dictValue === data.exportCurrency.toString())?.dictLabel || '',
					exchangeRate: data.exchangeRate || '',
					priceTerms: state.optionss.hr_pricing_term.find(item => item.dictValue === data.priceTerms.toString())?.dictLabel || '',
					settlementMethod: state.optionss.hr_settlement_way.find(item => item.dictValue === data.settlementMethod.toString())?.dictLabel || '',
					receivableDate: data.receivableDate ? data.receivableDate.split(' ')[0] : '',
					salesperson: '',
					documentOfficer: state.optionss.sql_all_user.find(item => item.dictValue === data.documentClerk.toString())?.dictLabel || '',
					isSettled: data.isSettled ? '是' : '否'
				}

			} else {
				ElMessage.error('获取出运发货单数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取出运发货单数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取工厂付款数据
		request({
			url: 'ShippingDeliveries/GetFactoryPaymentData/GetFactoryPaymentDataList',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				factorypaymenttableData.value = response.data.map(item => ({
					applicationNumber: item.applicationNumber,
					reviewStatus: item.reviewStatus,
					reviewStatusStr: getReviewStatusText(item.reviewStatus),
					paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
					paymentName: state.optionss["hr_factory_payment"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
					payeeName: item.payeeName,
					bankName: item.bankName,
					bankAccount: item.bankAccount,
					ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
					currencyCode: state.optionss["hr_currency_code"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
					totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
					paidAmount: item.paidAmount?.toFixed(2) || '0.00',
					unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
					applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
					applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
					handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
					applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
				}))
			} else {
				ElMessage.error('获取工厂付款数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取工厂付款数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取采购其它费用数据
		request({
			url: 'ShippingDeliveries/GetPurchaseOtherFeeDataByShippingDeliveriesID/GetPurchaseOtherFeeData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				PurchaseOtherCostsTableData.value = response.data.map(item => ({
					applicationNumber: item.applicationNumber,
					reviewStatus: item.reviewStatus,
					reviewStatusStr: getReviewStatusText(item.reviewStatus),
					paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
					paymentName: state.optionss["hr_factory_payment"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
					payeeName: item.payeeName,
					bankName: item.bankName,
					bankAccount: item.bankAccount,
					ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
					currencyCode: state.optionss["hr_currency_code"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
					totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
					paidAmount: item.paidAmount?.toFixed(2) || '0.00',
					unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
					applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
					applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
					handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
					applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
				}))
			} else {
				ElMessage.error('获取采购其它费用数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取采购其它费用数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取客户收汇数据
		request({
			url: 'ShippingDeliveries/GetCustomerControllerDataByShippingDeliveriesID/GetCustomerControllerData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				CustomerReceiptsTableData.value = response.data.map(item => ({
					ReceiptNumber: item.receiptNumber,
					ReceiptDate: item.receiptDate ? item.receiptDate.split(' ')[0] : '',
					ReceiptAmount: item.amount,
					ReceiptCurrency: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.foreignCurrency.toString())?.dictLabel || '',
					ExchangeRate: item.exchangeRate,
					ReceiptBank: state.optionss["hr_bank"].find(bank => bank.dictValue == item.bank.toString())?.dictLabel || '',
					OurCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
					FundsType: state.optionss["funds_type"].find(ft => ft.dictValue == item.fundsType.toString())?.dictLabel || '',
				}))
			} else {
				ElMessage.error('获取客户收汇数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取客户收汇数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取退税数据
		request({
			url: 'ShippingDeliveries/GetTaxRefundDataByShippingDeliveriesID/GetTaxRefundData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				TaxRefundTableData.value = response.data.map(item => ({
					ReceiptNumber: item.refundNumber,
					InvoiceNumber: item.invoiceNumber,
					RefundAmount: item.refundAmount,
					ActualRefundAmount: item.actualRefundAmount,
					IsRefunded: item.isRefunded ? '是' : '否',
					RefundDate: item.refundDate ? item.refundDate.split(' ')[0] : '',
				}))
				calculationTaxRefund();
			} else {
				ElMessage.error('获取退税数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取退税数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取国内已付费用数据
		request({
			url: 'ShippingDeliveries/GetDomesticFeesDataByShippingDeliveriesID/GetDomesticFeesData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				DomesticPaymentsDataTable.value = response.data.map(item => ({
					applicationNumber: item.applicationNumber,
					reviewStatus: item.reviewStatus,
					reviewStatusStr: getReviewStatusText(item.reviewStatus),
					paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
					paymentName: state.optionss["hr_domestic_charges"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
					payeeName: item.payeeName,
					bankName: item.bankName,
					bankAccount: item.bankAccount,
					ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
					currencyCode: state.optionss["hr_currency_code"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
					totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
					paidAmount: item.paidAmount?.toFixed(2) || '0.00',
					unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
					applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
					applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
					handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
					applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
				}))
				calculateDomesticFees();
			} else {
				ElMessage.error('获取国内已付费用数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取国内已付费用数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取国外已付费用数据
		request({
			url: 'ShippingDeliveries/GetForeignExpensesByShippingDeliveriesID/GetForeignExpensesData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				ForeignExpensesDataTable.value = response.data.map(item => ({
					applicationNumber: item.applicationNumber,
					reviewStatus: item.reviewStatus,
					reviewStatusStr: getReviewStatusText(item.reviewStatus),
					paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
					paymentName: state.optionss["hr_foreign_charges"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
					payeeName: item.payeeName,
					bankName: item.bankName,
					bankAccount: item.bankAccount,
					ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
					currencyCode: state.optionss["hr_currency_code"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
					totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
					paidAmount: item.paidAmount?.toFixed(2) || '0.00',
					unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
					applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
					applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
					handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
					applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
				}))
				calculateForeignExpenses();
			} else {
				ElMessage.error('获取国内已付费用数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取国内已付费用数据失败，请稍后重试');
		});
		//#endregion
		//#region 获取客户收汇综合数据
		request({
			url: 'ShippingDeliveries/GetCustomerReceiptDataByShippingDeliveriesID/GetCustomerReceiptData',
			method: 'GET',
			params: {
				ShippingDeliveriesId: settlement_center_shipping.value
			}
		}).then(response => {
			if (response.code === 200) {
				CustomerReceiptForm.value.totalValue = response.data.totalValue;
				CustomerReceiptForm.value.totalCost = response.data.totalCost;
				CustomerReceiptForm.value.totalReceivable = response.data.accountsReceivable;
				CustomerReceiptForm.value.totalReceived = response.data.totalReceivable;
				CustomerReceiptForm.value.totalReceived = response.data.paymentReceived;
				CustomerReceiptForm.value.totalReceivedCNY = response.data.paymentReceived;
				CustomerReceiptForm.value.totalUnreceived = response.data.uncollectedPayment;
			} else {
				ElMessage.error('获取国内已付费用数据失败：无效的响应数据');
			}
		}).catch(error => {
			ElMessage.error('获取国内已付费用数据失败，请稍后重试');
		});
		//#endregion
		hasQueried.value = true;
	}
	else {
		ElMessage.warning('请先选择出运发货单');
		return;
	}
	request({
		url: 'ShippingDeliveries/GetSettlementByShippingDeliveriesID/GetSettlement',
		method: 'GET',
		params: {
			ShippingDeliveriesID: settlement_center_shipping.value
		}
	}).then(response => {
		if (response.code === 200 && response.data) {
			const settlementData = response.data;
			// 显示结算单状态
			SettlementStatus.value = settlementData.status;
			SettlementApprovalStatus.value = settlementData.approvalStatus;
			const statusText = getSettlementStatusText(settlementData.status);
			const statusType = getSettlementStatusType(settlementData.status);

		} else {
			ElMessage.warning('获取结算单数据失败：无效的响应数据');
		}
	}).catch(error => {
		isCloseButtonDisabled.value = false;
		console.error('获取结算单数据失败：', error);
		ElMessage.error('获取结算单数据失败，请稍后重试');
	});
}
const SettlementStatus = ref(0);
const SettlementApprovalStatus = ref('');

// 状态常量定义
const SETTLEMENT_STATUS = {
	OPEN: 0,    // 未关账
	CLOSED: 1   // 已关账
};

// 获取结算单状态文本
const getSettlementStatusText = (status) => {
	switch (status) {
		case SETTLEMENT_STATUS.OPEN:
			return '未关账';
		case SETTLEMENT_STATUS.CLOSED:
			return '已关账';
		default:
			return '未关账';
	}
};

// 获取结算单状态对应的标签类型
const getSettlementStatusType = (status) => {
	switch (status) {
		case SETTLEMENT_STATUS.OPEN:
			return 'warning';  // 未关账 - 黄色警告
		case SETTLEMENT_STATUS.CLOSED:
			return 'success';  // 已关账 - 绿色成功
		default:
			return 'info';     // 未知状态 - 灰色信息
	}
};

// 审核状态转换函数
const getReviewStatusText = (status) => {
	const statusMap = {
		0: '待提审',
		1: '审核中',
		2: '已批准',
		3: '已拒绝'
	}
	return statusMap[status] || '未知状态'
}
// 获取状态对应的标签类型
const getStatusType = (status) => {
	const typeMap = {
		0: 'info',    // 待提审 - 灰色
		1: 'warning', // 审核中 - 黄色
		2: 'success', // 已批准 - 绿色
		3: 'danger'   // 已拒绝 - 红色
	}
	return typeMap[status] || 'info'
}
// 计算退税总额和状态
const calculationTaxRefund = () => {
	let totalRefundAmount = 0;    // 应退税总额
	let totalActualRefund = 0;    // 已退税总额

	TaxRefundTableData.value.forEach((row) => {
		// 计算应退税总额（所有行的应退税额之和）
		totalRefundAmount += parseFloat(row.RefundAmount) || 0;

		// 只有在 IsRefunded 为 true 时才计入已退税总额
		if (row.IsRefunded === '是') {
			totalActualRefund += parseFloat(row.ActualRefundAmount) || 0;
		}
	});

	// 计算未退税额（应退税总额 - 已退税总额）
	const totalUnrefunded = totalRefundAmount - totalActualRefund;

	// 更新表单数据，保留两位小数
	TaxRefundForm.value = {
		totalRefund: totalRefundAmount.toFixed(2),      // 应退税总额
		totalRefunded: totalActualRefund.toFixed(2),    // 已退税总额
		totalUnrefunded: totalUnrefunded.toFixed(2),    // 未退税总额
	}
}

// 计算国内费用
const calculateDomesticFees = () => {
	let totalTransportation = 0;    // 前程运输费用
	let totalPayable = 0;          // 应付运杂费
	let totalPaid = 0;             // 已付运杂费
	let totalExpress = 0;          // 快件费用
	let totalOther = 0;            // 其它国内费用

	DomesticPaymentsDataTable.value.forEach(item => {
		// 判断付款类别是否为运杂费
		if (item.paymentName === '运杂费') {
			totalPayable += parseFloat(item.totalAmount) || 0;
			totalTransportation += parseFloat(item.totalAmount) || 0;
			// 只有审核通过的才计入已付金额
			if (item.reviewStatus === 2) { // 2 表示已批准
				totalPaid += parseFloat(item.paidAmount) || 0;
			}
		}
		if (item.paymentName === '其它') {
			totalOther += parseFloat(item.totalAmount) || 0;
		}

		// 判断付款类别
		switch (item.paymentName) {
			case '前程运输':
				totalTransportation += parseFloat(item.totalAmount) || 0;
				break;
			case '快递费':
				totalExpress += parseFloat(item.totalAmount) || 0;
				break;
			case '其它费用':
				totalOther += parseFloat(item.totalAmount) || 0;
				break;
		}
	});

	// 计算未付运杂费
	const totalUnpaid = totalPayable - totalPaid;

	// 更新表单数据
	DomesticFeesForm.value = {
		transportation: totalTransportation.toFixed(2),    // 前程运输
		totalPayable: totalPayable.toFixed(2),            // 应付运杂费
		totalPaid: totalPaid.toFixed(2),                  // 已付运杂费
		totalUnpaid: totalUnpaid.toFixed(2),              // 未付运杂费
		expressFee: totalExpress.toFixed(2),              // 快件费用
		otherDomesticCost: totalOther.toFixed(2)          // 其它国内费用
	}
}

// 计算国外费用
const calculateForeignExpenses = () => {
	let commissionPayable = 0;    // 应付佣金
	let commissionPaid = 0;       // 已付佣金
	let shippingPayable = 0;      // 应付海运费
	let shippingPaid = 0;         // 已付海运费
	let otherForeignCost = 0;     // 其它国外费用

	ForeignExpensesDataTable.value.forEach(item => {
		const amount = parseFloat(item.totalAmount) || 0;
		const isApproved = item.reviewStatus === 2; // 2 表示已批准

		// 根据款项名称分类计算
		switch (item.paymentName) {
			case '佣金':
				commissionPayable += amount;
				if (isApproved) {
					commissionPaid += amount;
				}
				break;
			case '海运费':
				shippingPayable += amount;
				if (isApproved) {
					shippingPaid += amount;
				}
				break;
			case '其它':
				otherForeignCost += amount;
				break;
		}
	});

	// 计算未付金额
	const commissionUnpaid = commissionPayable - commissionPaid;
	const shippingUnpaid = shippingPayable - shippingPaid;

	// 更新表单数据
	ForeignExpensesForm.value.commissionPayable = commissionPayable.toFixed(2);       // 应付佣金
	ForeignExpensesForm.value.commissionPaid = commissionPaid.toFixed(2);            // 已付佣金
	ForeignExpensesForm.value.commissionUnpaid = commissionUnpaid.toFixed(2);        // 未付佣金
	ForeignExpensesForm.value.shippingPayable = shippingPayable.toFixed(2);          // 应付海运费
	ForeignExpensesForm.value.shippingPaid = shippingPaid.toFixed(2);               // 已付海运费
	ForeignExpensesForm.value.shippingUnpaidAmount = shippingUnpaid.toFixed(2);     // 未付海运费
	ForeignExpensesForm.value.otherForeignCost = otherForeignCost.toFixed(2);       // 其它国外费用
}

const CloseSettlement = async () => {
	try {
		// 辅助函数：将字符串转换为数值，处理 null、undefined 和空字符串
		const toDecimal = (value) => {
			if (value === null || value === undefined || value === '') {
				return null;
			}
			const num = parseFloat(value);
			return isNaN(num) ? null : num;
		};

		const settlementRequest = {
			// 主要信息
			ShippingDeliveriesID: settlement_center_shipping.value,
			InvoiceNumber: shippingdeliveryform.value.invoiceNumber || null,
			ShippingDate: shippingdeliveryform.value.shippingDate || null,
			ShippingStatus: shippingdeliveryform.value.shippingStatus || null,
			SalesContract: shippingdeliveryform.value.salesContract || null,
			CustomerContract: shippingdeliveryform.value.customerContract || null,
			CustomerName: shippingdeliveryform.value.customerName || null,
			OurCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictLabel == shippingdeliveryform.value.ourCompany.toString())?.dictValue || 0,
			ForeignCurrency: state.optionss["hr_export_currency"].find(cc => cc.dictLabel == shippingdeliveryform.value.foreignCurrency.toString())?.dictValue || 0,
			ExchangeRate: toDecimal(shippingdeliveryform.value.exchangeRate),
			PriceTerms: shippingdeliveryform.value.priceTerms || null,
			SettlementMethod: shippingdeliveryform.value.settlementMethod || null,
			ReceivableDate: shippingdeliveryform.value.receivableDate || null,
			Salesperson: state.optionss["sql_all_user"].find(user => user.dictLabel == shippingdeliveryform.value.salesperson.toString())?.dictValue || 0,
			DocumentOfficer: shippingdeliveryform.value.documentOfficer || null,
			IsSettled: shippingdeliveryform.value.isSettled === true,

			// 客户收汇
			TotalValue: toDecimal(CustomerReceiptForm.value.totalValue),
			TotalCost: toDecimal(CustomerReceiptForm.value.totalCost),
			ClaimAmount: toDecimal(CustomerReceiptForm.value.claimAmount),
			TotalReceivable: toDecimal(CustomerReceiptForm.value.totalReceivable),
			TotalReceived: toDecimal(CustomerReceiptForm.value.totalReceived),
			TotalReceivedCNY: toDecimal(CustomerReceiptForm.value.totalReceivedCNY),
			TotalUnreceived: toDecimal(CustomerReceiptForm.value.totalUnreceived),

			// 退税情况
			TotalRefund: toDecimal(TaxRefundForm.value.totalRefund),
			TotalRefunded: toDecimal(TaxRefundForm.value.totalRefunded),
			TotalUnrefunded: toDecimal(TaxRefundForm.value.totalUnrefunded),
			IsRefunded: TaxRefundForm.value.isRefunded === true,

			// 国内费用
			Transportation: toDecimal(DomesticFeesForm.value.transportation),
			TotalPayable: toDecimal(DomesticFeesForm.value.totalPayable),
			TotalPaid: toDecimal(DomesticFeesForm.value.totalPaid),
			TotalUnpaid: toDecimal(DomesticFeesForm.value.totalUnpaid),
			ExpressFee: toDecimal(DomesticFeesForm.value.expressFee),
			OtherDomesticCost: toDecimal(DomesticFeesForm.value.otherDomesticCost),

			// 国外费用
			CommissionPayable: toDecimal(ForeignExpensesForm.value.commissionPayable),
			CommissionPaid: toDecimal(ForeignExpensesForm.value.commissionPaid),
			CommissionPaidAmount: toDecimal(ForeignExpensesForm.value.commissionPaidAmount),
			CommissionUnpaid: toDecimal(ForeignExpensesForm.value.commissionUnpaid),
			ShippingAgentCompany: ForeignExpensesForm.value.shippingAgentCompany || null,
			ShippingCurrency: state.optionss["hr_export_currency"].find(cc => cc.dictLabel == ForeignExpensesForm.value.shippingCurrency.toString())?.dictValue || 0,
			ShippingExchangeRate: toDecimal(ForeignExpensesForm.value.shippingExchangeRate),
			ShippingPayable: toDecimal(ForeignExpensesForm.value.shippingPayable),
			ShippingPaid: toDecimal(ForeignExpensesForm.value.shippingPaid),
			ShippingPaidAmount: toDecimal(ForeignExpensesForm.value.shippingPaidAmount),
			ShippingUnpaidAmount: toDecimal(ForeignExpensesForm.value.shippingUnpaidAmount),
			OtherForeignCost: toDecimal(ForeignExpensesForm.value.otherForeignCost),

			// 其他必要字段
			ApprovalStatus: 0,
			Status: 0,
			IsDelete: 0,
			Remark: ''
		};

		// 调试输出
		console.log('Settlement Request:', settlementRequest);

		const response = await request({
			url: 'ShippingDeliveries/ClosingSettlement/ClosingSettlement',
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			data: settlementRequest
		});

		if (response.code === 200) {
			ElMessage.success('关账成功');
			handleSearch();
		} else {
			ElMessage.error('关账失败：' + response.msg);
		}
	} catch (error) {
		console.error('关账出错：', error);
		if (error.response) {
			console.error('Error response:', error.response.data);
		}
		ElMessage.error('关账失败，请稍后重试');
	}
}
</script>
