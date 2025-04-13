<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="addfatoryinvoicedialog = true">工厂开票</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="receiptNumber" filterable placeholder="选择发票号码" style="width: 15%">
				<el-option v-for="item in receiptNumberOptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="bank" filterable placeholder="选择客户简称" style="width: 15%">
				<el-option v-for="item in bankOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateStart" type="date" placeholder="请选择退税日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateEnd" type="date" placeholder="请选择退税日期止" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;工厂开票单据</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="fatoryinvoicetableData">
			<el-table-column prop="vendorCode" label="厂商编号" width="150"></el-table-column>
			<el-table-column prop="vendorName" label="厂商简称" width="150"></el-table-column>
			<el-table-column prop="contact" label="联系人" width="150"></el-table-column>
			<el-table-column prop="taxNumber" label="公司税号" width="150"></el-table-column>
			<el-table-column prop="invoiceNumber" label="开票编号" width="150"></el-table-column>
			<el-table-column prop="receiptNumber" label="发票号码" width="150"></el-table-column>
			<el-table-column prop="shipmentDate" label="出运日期" width="150"></el-table-column>
			<el-table-column prop="invoiceAmount" label="开票金额" width="150"></el-table-column>
			<el-table-column prop="vatInvoiceAmount" label="增值税发票金额" width="150"></el-table-column>
			<el-table-column prop="taxRate" label="税率" width="150"></el-table-column>
			<el-table-column prop="refundRate" label="退税率" width="150"></el-table-column>
			<el-table-column prop="refundableAmount" label="可退税额" width="150"></el-table-column>
		</el-table>
		<el-pagination background layout="prev, pager, next" :total="1000" style="margin-top: 5px;" />
		<el-dialog v-model="addfatoryinvoicedialog" title="新增工厂开票" :close-on-click-modal=false style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="addfatoryinvoiceform" label-width="100px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="厂商编号">
							<el-select v-model="addfatoryinvoiceform.vendorCode" filterable placeholder="选择厂商编号"
								style="width: 300px">
								<el-option v-for="item in vendorCodeOptions" :key="item.value" :label="item.label"
									:value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商简称">
							<el-input v-model="addfatoryinvoiceform.vendorName" placeholder="请输入厂商简称"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="联系人">
							<el-select v-model="addfatoryinvoiceform.contact" filterable placeholder="选择联系人"
								style="width: 300px">
								<el-option v-for="item in contactOptions" :key="item.value" :label="item.label"
									:value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="公司税号">
							<el-input v-model="addfatoryinvoiceform.taxNumber" placeholder="请输入公司税号"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="开票编号">
							<el-select v-model="addfatoryinvoiceform.invoiceNumber" filterable placeholder="选择开票编号"
								style="width: 300px">
								<el-option v-for="item in invoiceNumberOptions" :key="item.value" :label="item.label"
									:value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="发票号码">
							<el-input v-model="addfatoryinvoiceform.receiptNumber" placeholder="请输入发票号码"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="出运日期">
							<el-date-picker v-model="addfatoryinvoiceform.shipmentDate" type="date"
								placeholder="请选择出运日期" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">开票内容</span>
			<el-divider></el-divider>
			<el-table :data="invoicingcontenttableData">
				<el-table-column prop="customsCode" label="海关编码" width="150"></el-table-column>
				<el-table-column prop="productName" label="未开品名" width="150"></el-table-column>
				<el-table-column prop="invoiceQuantity" label="开票数量" width="150"></el-table-column>
				<el-table-column prop="invoiceUnitPrice" label="开票单价" width="150"></el-table-column>
				<el-table-column prop="measurementUnit" label="计量单位" width="150"></el-table-column>
				<el-table-column prop="displayMeasurementUnit" label="计量单位显示" width="150"></el-table-column>
				<el-table-column prop="invoiceAmount" label="开票金额" width="150"></el-table-column>
				<el-table-column prop="invoiceRemark" label="开票备注" width="150"></el-table-column>
				<el-table-column prop="vatInvoiceCode" label="增值税发票代码" width="150"></el-table-column>
				<el-table-column prop="vatInvoiceNumber" label="增值税发票号码" width="150"></el-table-column>
				<el-table-column prop="vatInvoiceDate" label="增值税发票日期" width="150"></el-table-column>
				<el-table-column prop="vatInvoiceAmount" label="增值税发票金额" width="150"></el-table-column>
				<el-table-column prop="uninvoicedAmount" label="未开票金额" width="150"></el-table-column>
				<el-table-column prop="taxRate" label="税率" width="150"></el-table-column>
				<el-table-column prop="taxAmount" label="税额" width="150"></el-table-column>
				<el-table-column prop="refundRate" label="退税率" width="150"></el-table-column>
				<el-table-column prop="refundableAmount" label="可退税额" width="150"></el-table-column>
			</el-table>
			<div style="margin-top: 30px;"></div>
			<span style="font-size: 20px; font-weight: bold;">合计信息</span>
			<el-divider></el-divider>
			<el-form :model="addfatoryinvoiceform2" label-width="100px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="应开票合计">
							<el-input v-model="addfatoryinvoiceform2.totalInvoiceAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已开票合计">
							<el-input v-model="addfatoryinvoiceform2.totalInvoicedAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="未开票合计">
							<el-input v-model="addfatoryinvoiceform2.totalUninvoicedAmount" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">备注信息</span>
			<el-divider></el-divider>
			<el-form :model="addfatoryinvoiceform2" label-width="100px">
				<el-row>
					<el-col :span="24">
						<el-form-item label="备注信息">
							<el-input type="textarea" :rows="5" placeholder="在此输入文字" v-model="textValue"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary">
						保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElTabs, UploadUserFile, UploadProps } from 'element-plus'
const fatoryinvoicetableData = ref([
	{
		vendorCode: '厂商编号',
		vendorName: '厂商简称',
		contact: '联系人',
		taxNumber: '公司税号',
		invoiceNumber: '开票编号',
		receiptNumber: '发票号码',
		shipmentDate: '出运日期',
		invoiceAmount: '开票金额',
		vatInvoiceAmount: '增值税发票金额',
		taxRate: '税率',
		refundRate: '退税率',
		refundableAmount: '可退税额'
	}
])
const addfatoryinvoicedialog = ref(false)
const addfatoryinvoiceform = ref({
	vendorCode: '',
	vendorName: '',
	contact: '',
	taxNumber: '',
	invoiceNumber: '',
	receiptNumber: '',
	shipmentDate: '',
	invoiceAmount: '',
	vatInvoiceAmount: '',
	taxRate: '',
	refundRate: '',
	refundableAmount: ''
})
const invoiceNumberOptions = ref([
	{
		value: '1',
		label: '开票编号1'
	},
	{
		value: '2',
		label: '开票编号2'
	},
	{
		value: '3',
		label: '开票编号3'
	}
])
const contactOptions = ref([
	{
		value: '1',
		label: '联系人1'
	},
	{
		value: '2',
		label: '联系人2'
	},
	{
		value: '3',
		label: '联系人3'
	}
])
const vendorCodeOptions = ref([
	{
		value: '1',
		label: '厂商编号1'
	},
	{
		value: '2',
		label: '厂商编号2'
	},
	{
		value: '3',
		label: '厂商编号3'
	}
])
const invoicingcontenttableData = ref([
	{
		customsCode: '海关编码',
		productName: '未开品名',
		invoiceQuantity: '开票数量',
		invoiceUnitPrice: '开票单价',
		measurementUnit: '计量单位',
		displayMeasurementUnit: '计量单位显示',
		invoiceAmount: '开票金额',
		invoiceRemark: '开票备注',
		vatInvoiceCode: '增值税发票代码',
		vatInvoiceNumber: '增值税发票号码',
		vatInvoiceDate: '增值税发票日期',
		vatInvoiceAmount: '增值税发票金额',
		uninvoicedAmount: '未开票金额',
		taxRate: '税率',
		taxAmount: '税额',
		refundRate: '退税率',
		refundableAmount: '可退税额'
	}
])
const addfatoryinvoiceform2 = ref({
	totalInvoiceAmount: '',
	totalInvoicedAmount: '',
	totalUninvoicedAmount: ''
})
</script>
