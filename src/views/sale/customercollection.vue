<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="CreateCustomercollectionDialog = true">新建客户收汇单</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-input v-model="quotationNum" clearable style="width: 15%"
				placeholder="输入收款单号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="customerinfoselect" filterable placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="item in customerinfoselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="productselect" filterable placeholder="选择经手人（可输入查询）" style="width: 15%">
				<el-option v-for="item in productselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="inquiryDate" type="date" placeholder="请选择起始日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="quotationDate" type="date" placeholder="请选择结束日期" size="Default"
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
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;客户收汇表</span>
		</div>
		<el-divider></el-divider>
		<el-table :data="tableData">
			<el-table-column prop="receiptNo" label="收款单号" width="150"></el-table-column>
			<el-table-column prop="collectionDate" label="收汇日期" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="customerNo" label="客户编号" width="150"></el-table-column>
			<el-table-column prop="customerAbbreviation" label="客户简称" width="150"></el-table-column>
			<el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
			<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
			<el-table-column prop="collectionAmount" label="收汇金额" width="150"></el-table-column>
			<el-table-column prop="collectionBank" label="收汇银行" width="150"></el-table-column>
			<el-table-column prop="bankDeduction" label="银行扣款" width="150"></el-table-column>
			<el-table-column prop="bankFee" label="银行手续费" width="150"></el-table-column>
			<el-table-column prop="otherDeduction" label="其它扣款" width="150"></el-table-column>
			<el-table-column prop="deductionTotal" label="扣款合计" width="150"></el-table-column>
			<el-table-column prop="netCollectionAmount" label="收汇净额" width="150"></el-table-column>
			<el-table-column prop="usedAmount" label="已用金额" width="150"></el-table-column>
			<el-table-column prop="remainingAmount" label="剩余金额" width="150"></el-table-column>
			<el-table-column prop="handler" label="经手人" width="150"></el-table-column>
			<el-table-column prop="remarks" label="备注说明" width="150"></el-table-column>
		</el-table>
		<el-dialog v-model="CreateCustomercollectionDialog" title="新建客户收汇" :close-on-click-modal=false
			style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="CustomercollectionDialogform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="客户编号">
							<el-select v-model="CustomercollectionDialogform.customerNo" filterable placeholder="选择客户编号"
								disabled style="width: 300px">
								<!-- Options for customerNo select -->
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户简称">
							<el-input v-model="CustomercollectionDialogform.customerAbbreviation" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇单号">
							<el-input v-model="CustomercollectionDialogform.receiptNo" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="收汇日期">
							<el-date-picker v-model="CustomercollectionDialogform.collectionDate" type="date" disabled
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="CustomercollectionDialogform.ourCompany" filterable placeholder="选择我方公司"
								disabled style="width: 300px">
								<!-- Options for ourCompany select -->
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外销币种">
							<el-select v-model="CustomercollectionDialogform.foreignCurrency" filterable
								placeholder="选择外销币种" disabled style="width: 300px">
								<!-- Options for foreignCurrency select -->
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="汇率">
							<el-input v-model="CustomercollectionDialogform.exchangeRate" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇金额">
							<el-input v-model="CustomercollectionDialogform.collectionAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇银行">
							<el-input v-model="CustomercollectionDialogform.collectionBank" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="银行扣款">
							<el-input v-model="CustomercollectionDialogform.bankDeduction"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="银行手续费">
							<el-input v-model="CustomercollectionDialogform.bankFee" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="其它扣款">
							<el-input v-model="CustomercollectionDialogform.otherDeduction"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="扣款合计">
							<el-input v-model="CustomercollectionDialogform.deductionTotal" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇净额">
							<el-input v-model="CustomercollectionDialogform.netCollectionAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已用金额">
							<el-input v-model="CustomercollectionDialogform.usedAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="剩余金额">
							<el-input v-model="CustomercollectionDialogform.remainingAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="经手人">
							<el-select v-model="CustomercollectionDialogform.handler" filterable placeholder="选择经手人"
								disabled style="width: 300px">
								<!-- Options for handler select -->
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="备注说明">
							<el-input type="textarea" v-model="CustomercollectionDialogform.remarks"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">详细用途（结算中心已结账则不显示）</span>
			<el-divider></el-divider>
			<el-button class="mt-4" type="primary" @click="SearchProcutDialog = true"
				style="margin-bottom: 10px;">添加发票</el-button>
			<el-table :data="tableData">
				<el-table-column prop="invoiceNo" label="发票号码" width="150"></el-table-column>
				<el-table-column prop="paymentCategory" label="款项类别" width="150"></el-table-column>
				<el-table-column prop="relatedModule" label="关联模块" width="150"></el-table-column>
				<el-table-column prop="relatedNumber" label="关联单号" width="150"></el-table-column>
				<el-table-column prop="relatedDate" label="关联日期" width="150"></el-table-column>
				<el-table-column prop="usageDate" label="使用日期" width="150"></el-table-column>
				<el-table-column prop="usageAmount" label="使用金额" width="150"></el-table-column>
				<el-table-column prop="remarks" label="备注说明" width="150"></el-table-column>
				<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
			</el-table>
			<span style="font-size: 20px; font-weight: bold;">未收款项（结算中心已结账则不显示）</span>
			<el-divider></el-divider>
			<el-table :data="tableData2">
				<el-table-column prop="invoiceNo" label="发票号码" width="150"></el-table-column>
				<el-table-column prop="paymentCategory" label="款项类别" width="150"></el-table-column>
				<el-table-column prop="relatedModule" label="关联模块" width="150"></el-table-column>
				<el-table-column prop="relatedNumber" label="关联单号" width="150"></el-table-column>
				<el-table-column prop="relatedDate" label="关联日期" width="150"></el-table-column>
				<el-table-column prop="usageDate" label="使用日期" width="150"></el-table-column>
				<el-table-column prop="usageAmount" label="使用金额" width="150"></el-table-column>
				<el-table-column prop="handler" label="经手人" width="150"></el-table-column>
			</el-table>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="success" @click="">
						收款完结
					</el-button>
					<el-button type="primary" @click="">
						确定保存
					</el-button>

				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn } from 'element-plus'

const CreateCustomercollectionDialog = ref(false)
const CustomercollectionDialogform = ref({
	customerNo: '',
	customerAbbreviation: '',
	receiptNo: '',
	collectionDate: '',
	ourCompany: '',
	foreignCurrency: '',
	exchangeRate: '',
	collectionAmount: '',
	collectionBank: '',
	bankDeduction: '',
	bankFee: '',
	otherDeduction: '',
	deductionTotal: '',
	netCollectionAmount: '',
	usedAmount: '',
	remainingAmount: '',
	handler: '',
	remarks: ''
})
</script>