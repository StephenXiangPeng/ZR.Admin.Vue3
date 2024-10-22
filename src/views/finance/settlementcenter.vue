<template>
	<div>
		<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="receiptNumber" filterable placeholder="请选择销售合同" style="width: 15%">
				<el-option v-for="item in receiptNumberOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;主要信息</span>
		</div>
		<el-divider> </el-divider>
		<el-form :model="settlementcenterform" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="发票号码">
						<el-select v-model="settlementcenterform.invoiceNumber" filterable placeholder="请选择" disabled
							:style="{ width: '300px' }">
							<el-option v-for="item in invoiceOptions" :key="item.value" :label="item.label"
								:value="item.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="出运日期">
						<el-date-picker v-model="settlementcenterform.shippingDate" type="date" placeholder="请选择" disabled
							:style="{ width: '300px' }"></el-date-picker>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="出运状态">
						<el-input v-model="settlementcenterform.shippingStatus" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="销售合同">
						<el-input v-model="settlementcenterform.salesContract" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="客户合同">
						<el-input v-model="settlementcenterform.customerContract" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="客户编号">
						<el-input v-model="settlementcenterform.customerNumber" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="客户简称">
						<el-input v-model="settlementcenterform.customerAbbreviation" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="客户名称">
						<el-input v-model="settlementcenterform.customerName" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="我方公司">
						<el-select v-model="settlementcenterform.ourCompany" filterable placeholder="请选择" disabled
							:style="{ width: '300px' }">
							<el-option v-for="item in ourCompanyOptions" :key="item.value" :label="item.label"
								:value="item.value" />
						</el-select>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="结算类别">
						<el-input v-model="settlementcenterform.settlementType" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="外销币种">
						<el-input v-model="settlementcenterform.foreignCurrency" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="汇率">
						<el-input v-model="settlementcenterform.exchangeRate" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="价格条款">
						<el-input v-model="settlementcenterform.priceTerms" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="结汇方式">
						<el-input v-model="settlementcenterform.settlementMethod" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应收汇日">
						<el-date-picker v-model="settlementcenterform.receivableDate" type="date" placeholder="请选择" disabled
							:style="{ width: '300px' }"></el-date-picker>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="销售员">
						<el-input v-model="settlementcenterform.salesperson" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="单证员">
						<el-input v-model="settlementcenterform.documentOfficer" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="是否结账">
						<el-checkbox v-model="settlementcenterform.isSettled" disabled
							:style="{ width: '300px' }"></el-checkbox>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;客户收汇</span>
		</div>
		<el-divider></el-divider>
		<el-form :model="settlementcenterform2" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="货值合计">
						<el-input v-model="settlementcenterform2.totalValue" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="费用合计">
						<el-input v-model="settlementcenterform2.totalCost" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="索赔金额">
						<el-input v-model="settlementcenterform2.claimAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="应收货款">
						<el-input v-model="settlementcenterform2.totalReceivable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已收货款">
						<el-input v-model="settlementcenterform2.totalReceived" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已收货款￥">
						<el-input v-model="settlementcenterform2.totalReceivedCNY" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未收货款">
						<el-input v-model="settlementcenterform2.totalUnreceived" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;退税情况</span>
		</div>
		<el-divider> </el-divider>

		<el-form :model="settlementcenterform3" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="应退税￥">
						<el-input v-model="settlementcenterform3.totalRefund" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已退税￥">
						<el-input v-model="settlementcenterform3.totalRefunded" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="未退税￥">
						<el-input v-model="settlementcenterform3.totalUnrefunded" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="是否已退">
						<el-checkbox v-model="settlementcenterform3.isRefunded" disabled
							:style="{ width: '300px' }"></el-checkbox>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;国内费用</span>
		</div>
		<el-divider></el-divider>

		<el-form :model="settlementcenterform4" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="前程运输">
						<el-input v-model="settlementcenterform4.transportation" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应付运杂费￥">
						<el-input v-model="settlementcenterform4.totalPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付运杂费">
						<el-input v-model="settlementcenterform4.totalPaid" disabled :style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未付运杂费">
						<el-input v-model="settlementcenterform4.totalUnpaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="快件费用￥">
						<el-input v-model="settlementcenterform4.expressFee" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="其它国内费用￥">
						<el-input v-model="settlementcenterform4.otherDomesticCost" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
		</el-form>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;国外费用</span>
		</div>
		<el-divider> </el-divider>
		<el-form :model="settlementcenterform5" label-width="120px">
			<el-row>
				<el-col :span="8">
					<el-form-item label="应付佣金">
						<el-input v-model="settlementcenterform5.commissionPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付佣金">
						<el-input v-model="settlementcenterform5.commissionPaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付佣金￥">
						<el-input v-model="settlementcenterform5.commissionPaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="未付佣金">
						<el-input v-model="settlementcenterform5.commissionUnpaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应付保险">
						<el-input v-model="settlementcenterform5.insurancePayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付保险">
						<el-input v-model="settlementcenterform5.insurancePaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="已付保险￥">
						<el-input v-model="settlementcenterform5.insurancePaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="未付保险">
						<el-input v-model="settlementcenterform5.insuranceUnpaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="船代公司">
						<el-input v-model="settlementcenterform5.shippingAgentCompany" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="海运费币种">
						<el-input v-model="settlementcenterform5.shippingCurrency" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="海运费汇率">
						<el-input v-model="settlementcenterform5.shippingExchangeRate" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="应付海运">
						<el-input v-model="settlementcenterform5.shippingPayable" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="已付海运">
						<el-input v-model="settlementcenterform5.shippingPaid" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="已付海运￥">
						<el-input v-model="settlementcenterform5.shippingPaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
				<el-col :span="8">
					<el-form-item label="未付海运￥">
						<el-input v-model="settlementcenterform5.shippingUnpaidAmount" disabled
							:style="{ width: '300px' }"></el-input>
					</el-form-item>
				</el-col>
			</el-row>
			<el-row>
				<el-col :span="8">
					<el-form-item label="其它国外费用￥">
						<el-input v-model="settlementcenterform5.otherForeignCost" disabled
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
				<el-table :data="tableData">
					<el-table-column prop="purchaseContract" label="采购合同" width="150"></el-table-column>
					<el-table-column prop="vendorNumber" label="厂商编号" width="150"></el-table-column>
					<el-table-column prop="vendorAbbreviation" label="厂商简称" width="150"></el-table-column>
					<el-table-column prop="purchaser" label="采购员" width="150"></el-table-column>
					<el-table-column prop="purchaseCurrency" label="采购币种" width="150"></el-table-column>
					<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
					<el-table-column prop="paymentDueDate" label="应付款日" width="150"></el-table-column>
					<el-table-column prop="depositPaid" label="已付定金" width="150"></el-table-column>
					<el-table-column prop="paymentPaid" label="已付货款" width="150"></el-table-column>
					<el-table-column prop="totalPaid" label="已付合计" width="150"></el-table-column>
					<el-table-column prop="totalPaidAmount" label="已付合计￥" width="150"></el-table-column>
					<el-table-column prop="claimAmount" label="索赔金额" width="150"></el-table-column>
					<el-table-column prop="paymentUnpaid" label="未付货款" width="150"></el-table-column>
					<el-table-column prop="paymentUnpaidAmount" label="未付货款￥" width="150"></el-table-column>
				</el-table>
			</el-tab-pane>
			<el-tab-pane label="采购其它费用" name="othercosts">采购其它费用详情</el-tab-pane>
			<el-tab-pane label="客户已收汇详情" name="customerreceipts">客户已收汇详情</el-tab-pane>
			<el-tab-pane label="已退税详情" name="taxrefund">已退税详情</el-tab-pane>
			<el-tab-pane label="国内已付费用详情" name="domesticpayments">国内已付费用详情</el-tab-pane>
			<el-tab-pane label="国外已付费用详情" name="internationalpayments">国外已付费用详情</el-tab-pane>
		</el-tabs>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElTabs, UploadUserFile, UploadProps } from 'element-plus'
const activeName = ref('productinfo')
const handleClick = (tab: any, event: any) => {
	console.log(tab, event)
}
const settlementcenterform = ref({
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
})
const ourCompanyOptions = ref([
	{
		value: '1',
		label: '我方公司1'
	},
	{
		value: '2',
		label: '我方公司2'
	},
	{
		value: '3',
		label: '我方公司3'
	}
])

const invoiceOptions = ref([
	{
		value: '1',
		label: '发票号码1'
	},
	{
		value: '2',
		label: '发票号码2'
	},
	{
		value: '3',
		label: '发票号码3'
	}
])

const settlementcenterform2 = ref({
	totalValue: '',
	totalCost: '',
	claimAmount: '',
	totalReceivable: '',
	totalReceived: '',
	totalReceivedCNY: '',
	totalUnreceived: ''
})
const settlementcenterform3 = ref({
	totalRefund: '',
	totalRefunded: '',
	totalUnrefunded: '',
	isRefunded: false
})
const settlementcenterform4 = ref({
	transportation: '',
	totalPayable: '',
	totalPaid: '',
	totalUnpaid: '',
	expressFee: '',
	otherDomesticCost: ''
})
const settlementcenterform5 = ref({
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
})

const tableData = ref([
	{
		purchaseContract: 'Contract 1',
		vendorNumber: 'Vendor 1',
		vendorAbbreviation: 'Vendor Abbreviation 1',
		purchaser: 'Purchaser 1',
		purchaseCurrency: 'Currency 1',
		exchangeRate: 'Rate 1',
		paymentDueDate: 'Due Date 1',
		depositPaid: 'Deposit Paid 1',
		paymentPaid: 'Payment Paid 1',
		totalPaid: 'Total Paid 1',
		totalPaidAmount: 'Total Paid Amount 1',
		claimAmount: 'Claim Amount 1',
		paymentUnpaid: 'Payment Unpaid 1',
		paymentUnpaidAmount: 'Payment Unpaid Amount 1',
	},
	// Add more data objects here
]);
</script>