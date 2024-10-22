<template>
	<div>
		<div>
			<span style=" font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="receiptNumber" filterable placeholder="选择开票编号" style="width: 15%">
				<el-option v-for="item in receiptNumberOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="bank" filterable placeholder="选择厂家简称" style="width: 15%">
				<el-option v-for="item in bankOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateStart" type="date" placeholder="请选择发货日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateEnd" type="date" placeholder="请选择发货日期止" size="Default" style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;未开票清单</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="notinvoicedtableData">
			<el-table-column prop="invoiceNumber" label="开票编号" width="150"></el-table-column>
			<el-table-column prop="deliveryDate" label="发货日期" width="150"></el-table-column>
			<el-table-column prop="manufacturer" label="厂家简称" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="invoiceAmount" label="开票金额" width="150"></el-table-column>
			<el-table-column prop="receivedInvoice" label="是否收到发票" width="150"></el-table-column>
		</el-table>

		<div>
			<span style=" font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider> </el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="receiptNumber" filterable placeholder="选择开票编号" style="width: 15%">
				<el-option v-for="item in receiptNumberOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="bank" filterable placeholder="选择厂家简称" style="width: 15%">
				<el-option v-for="item in bankOptions" :key="item.value" :label="item.label" :value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateStart" type="date" placeholder="请选择发货日期起" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="receiptDateEnd" type="date" placeholder="请选择发货日期止" size="Default" style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;应收发票清单</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="ReceivableinvoicelisttableData">
			<el-table-column prop="invoiceNumber" label="开票编号" width="150"></el-table-column>
			<el-table-column prop="deliveryDate" label="发货日期" width="150"></el-table-column>
			<el-table-column prop="manufacturer" label="厂家简称" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="invoiceAmount" label="开票金额" width="150"></el-table-column>
			<el-table-column prop="receivedInvoice" label="是否收到发票" width="150"></el-table-column>
		</el-table>
	</div>
</template>
<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElTabs, UploadUserFile, UploadProps } from 'element-plus'
const notinvoicedtableData = ref([])
const ReceivableinvoicelisttableData = ref([])
</script>
