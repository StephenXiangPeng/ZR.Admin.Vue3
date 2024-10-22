<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenCreateshippingdeliveryDialog()">创建出运单</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select filterable v-model="SearchSaleContractID" placeholder="选择销售合同（可输入查询）" style="width: 15%">
				<el-option v-for="dict in optionss.sql_sale_contracts" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="SearchCustomerID" placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchShippingDateStart" type="date" placeholder="请选择出运日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchShippingDateEnd" type="date" placeholder="请选择出运日期" size="Default"
				style="width: 15%" />
		</div>

		<div style="width: 100%; margin-top: 5px;">
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchClick()">查询</el-button>
				<el-button @click="ResetClick()">重置</el-button>
			</el-row>
		</div>

		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;出运发货信息表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="shippingDeliveryTableData">
			<el-table-column prop="salesContractNumber" label="销售合同号" width="150px"></el-table-column>
			<el-table-column prop="referenceContractNumber" label="参考合同号" width="150px"></el-table-column>
			<el-table-column prop="invoiceNumber" label="发票号码" width="150px"></el-table-column>
			<el-table-column prop="createTime" label="制单日期" width="150px"></el-table-column>
			<el-table-column prop="shippingStatus" label="出运状态" width="150px"></el-table-column>
			<el-table-column prop="shippingDate" label="出运日期" width="150px"></el-table-column>
			<el-table-column prop="invoiceDate" label="发票日期" width="150px"></el-table-column>
			<el-table-column prop="customerNumber" label="客户编号" width="150px"></el-table-column>
			<el-table-column prop="customerAbbreviation" label="客户简称" width="150px"></el-table-column>
			<el-table-column prop="customerContractNumber" label="客户合同号" width="150px"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150px"></el-table-column>
			<el-table-column prop="bankOfReceipt" label="收汇银行" width="150px"></el-table-column>
			<el-table-column prop="exportCurrency" label="外销币种" width="150px"></el-table-column>
			<el-table-column prop="exchangeRate" label="汇率" width="150px"></el-table-column>
			<el-table-column prop="priceTerms" label="价格条款" width="150px"></el-table-column>
			<el-table-column prop="departurePort" label="出运口岸" width="150px"></el-table-column>
			<el-table-column prop="destinationPort" label="目的口岸" width="150px"></el-table-column>
			<el-table-column prop="tradeCountry" label="贸易国别" width="150px"></el-table-column>
			<el-table-column prop="settlementMethod" label="结汇方式" width="150px"></el-table-column>
			<el-table-column prop="transportationMethod" label="运输方式" width="150px"></el-table-column>
			<el-table-column prop="receivableDate" label="应收汇日" width="150px"></el-table-column>
			<el-table-column fixed="right" label="操作" width="100">
				<template #default="scope">
					<el-button type="text" size="small" @click="CheckShipingDelivery(scope.row)">查看/编辑</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="paymentrequesttableDataHandlePageChange"
			:current-page="ShippingDeliveriesTableDataCurrentPage" :page-size="ShippingDeliveriesTableDataPageSize"
			:total="ShippingDeliveriesTableDataTotalItems" background layout="prev, pager, next"
			style="margin-top: 5px;" />
		<el-dialog v-model="CreateshippingdeliveryDialog" title="创建出运发货单" :close-on-click-modal=false
			style="width: 70%;" @close="CreateshippingdeliveryDialogClose()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="AddShippingDeliveryform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="发票号码">
							<el-input v-model="AddShippingDeliveryform.invoiceNumber" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="制单日期">
							<el-date-picker v-model="AddShippingDeliveryform.OrderMakingDate" type="date"
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="出运状态">
							<el-select filterable v-model="AddShippingDeliveryform.shippingStatus" style="width: 300px">
								<el-option v-for="dict in optionss.hr_shipping_status" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="出运日期">
							<el-date-picker v-model="AddShippingDeliveryform.shippingDate" type="date"
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="发票日期">
							<el-date-picker v-model="AddShippingDeliveryform.invoiceDate" type="date"
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户编号">
							<el-select filterable v-model="AddShippingDeliveryform.customerNumber"
								placeholder="选择客户（可输入查询）" style="width: 300px" @change="customerNumberChange()">
								<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="客户简称">
							<el-input v-model="AddShippingDeliveryform.customerAbbreviation" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="参考合同">
							<el-select filterable v-model="AddShippingDeliveryform.referenceContractNumber"
								placeholder="选择销售合同（可输入查询）" style="width: 300px"
								@change="referenceContractNumberChange()">
								<el-option v-for="dict in optionss.sql_sale_contracts" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="销售合同">
							<el-input v-model="AddShippingDeliveryform.salesContractNumber" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="客户合同">
							<el-input v-model="AddShippingDeliveryform.customerContractNumber" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select filterable v-model="AddShippingDeliveryform.ourCompany" style="width: 300px"
								disabled>
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="收汇银行">
							<el-select filterable v-model="AddShippingDeliveryform.bankOfReceipt" style="width: 300px">
								<el-option v-for="dict in optionss.hr_bank" :key="dict.dictCode" :label="dict.dictLabel"
									:value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="外销币种">
							<el-select filterable v-model="AddShippingDeliveryform.exportCurrency" placeholder="选择外销币种"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="汇率">
							<el-input v-model="AddShippingDeliveryform.exchangeRate" :disabled="true"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="价格条款">
							<el-select filterable v-model="AddShippingDeliveryform.priceTerms" placeholder="选择价格条款"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="出运口岸">
							<el-select filterable v-model="AddShippingDeliveryform.departurePort" placeholder="选择出运口岸"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="目的口岸">
							<el-select filterable v-model="AddShippingDeliveryform.destinationPort" placeholder="选择目的口岸"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="贸易国别">
							<el-select filterable v-model="AddShippingDeliveryform.tradeCountry" placeholder="选择贸易国别"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="结汇方式">
							<el-select filterable v-model="AddShippingDeliveryform.settlementMethod" disabled
								placeholder="选择结汇方式" style="width: 300px">
								<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="运输方式">
							<el-select filterable v-model="AddShippingDeliveryform.transportationMethod" disabled
								placeholder="选择运输方式" style="width: 300px">
								<el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="应收汇日">
							<el-date-picker v-model="AddShippingDeliveryform.receivableDate" type="date"
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="单证员">
							<el-select filterable v-model="AddShippingDeliveryform.documentClerk" placeholder="选择单证员"
								style="width: 300px">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="有无定金">
							<el-checkbox v-model="AddShippingDeliveryform.isDeposit"></el-checkbox>
						</el-form-item>

					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">辅助信息</span>
			<el-divider></el-divider>
			<el-form label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="前程运输">
							<el-select filterable v-model="AddShippingDeliveryform.preCarriageTransport"
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_domestic_transport" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="船代公司">
							<el-select filterable v-model="AddShippingDeliveryform.shippingAgent" style="width: 300px">
								<el-option v-for="dict in optionss.hr_freight_forwarding_company" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">产品资料</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryProductTableData">
				<el-table-column prop="contractId" label="销售合同" width="150"></el-table-column>
				<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="shipmentQuantity" label="出货数量" width="150"></el-table-column>
				<el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
				<el-table-column prop="exportUnitPrice" label="外销单价" width="150"></el-table-column>
				<el-table-column prop="exportTotalPrice" label="外销总价" width="150"></el-table-column>
				<el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
				<el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
				<el-table-column prop="boxCount" label="箱数" width="150"></el-table-column>
				<el-table-column prop="outerBoxUnit" label="外箱单位" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.outerBoxUnit
						}}</span>
						<el-input v-else v-model="scope.row.outerBoxUnit" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="outerBoxLength" label="外箱长度" width="150"></el-table-column>
				<el-table-column prop="outerBoxWidth" label="外箱宽度" width="150"></el-table-column>
				<el-table-column prop="outerBoxHeight" label="外箱高度" width="150"></el-table-column>
				<el-table-column prop="outerBoxVolume" label="外箱体积" width="150"></el-table-column>
				<el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
				<el-table-column prop="outerBoxNetWeight" label="外箱净重" width="150"></el-table-column>
				<el-table-column prop="outerBoxGrossWeight" label="外箱毛重" width="150"></el-table-column>
				<el-table-column prop="totalNetWeight" label="总净重" width="150"></el-table-column>
				<el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
			</el-table>
			<span style="font-size: 20px; font-weight: bold;">采购明细</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryPurchaseDetailsTableData">
				<el-table-column prop="purchaseContractNumber" label="采购合同" width="150"></el-table-column>
				<el-table-column prop="vendorAbbreviation" label="厂商简称" width="150"></el-table-column>
				<el-table-column prop="productNumber" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="chineseSpecification" label="中文规格" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="出货数量" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.contractQuantity
						}}</span>
						<el-input v-else v-model="scope.row.contractQuantity" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="CustomsDeclarationChineseProductName" label="报关中文品名" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.CustomsDeclarationChineseProductName
						}}</span>
						<el-input v-else v-model="scope.row.CustomsDeclarationChineseProductName" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="CustomsDeclarationEnglishProductName" label="报关英文品名" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.CustomsDeclarationEnglishProductName
						}}</span>
						<el-input v-else v-model="scope.row.CustomsDeclarationEnglishProductName" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="CustomsDeclarationUnitPrice" label="报关单价" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.CustomsDeclarationUnitPrice
						}}</span>
						<el-input v-else v-model="scope.row.CustomsDeclarationUnitPrice" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="valuationMethod" label="计价方式" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.valuationMethod
						}}</span>
						<el-input v-else v-model="scope.row.valuationMethod" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="TotalCustomsDeclarationPrice" label="报关总价" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.TotalCustomsDeclarationPrice
						}}</span>
						<el-input v-else v-model="scope.row.TotalCustomsDeclarationPrice" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="purchaseCurrency" label="采购币种" width="150"></el-table-column>
				<el-table-column prop="purchaseUnitPrice" label="采购单价" width="150"></el-table-column>
				<el-table-column prop="purchaseTotalPrice" label="采购总价" width="150"></el-table-column>
				<el-table-column prop="measurementUnit" label="计量单位" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.measurementUnit
						}}</span>
						<el-input v-else v-model="scope.row.measurementUnit" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="WhetherToDeclareCustoms" label="是否报关" width="150"></el-table-column>
				<el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
				<el-table-column prop="taxRefundRate" label="退税率" width="150"></el-table-column>
				<el-table-column prop="invoiceQuantity" label="开票数量" width="150"></el-table-column>
				<el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
				<el-table-column prop="BillingUnit" label="开票单位" width="150">
					<template #default="scope">
						<span v-if="!scope.row.editable" @dblclick="scope.row.editable = true">{{
							scope.row.BillingUnit
						}}</span>
						<el-input v-else v-model="scope.row.BillingUnit" size="small"
							@blur="scope.row.editable = false" />
					</template>
				</el-table-column>
				<el-table-column prop="invoiceAmount" label="开票金额" width="150"></el-table-column>
				<el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
			</el-table>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isSaveBtnShow" type="primary" @click="SaveClick()">
						确定保存
					</el-button>
					<el-button type="primary" v-show="isEditSaveBtnShow" @click="EditSaveClick()">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile } from 'element-plus'
import { FOCUSABLE_CHILDREN } from 'element-plus/es/directives/trap-focus';
import request from '@/utils/request';
import { el } from 'element-plus/es/locale';
import { get } from 'sortablejs';

const isSaveBtnShow = ref(true)
const isEditSaveBtnShow = ref(false)

//查询条件
const SearchCustomerID = ref('')
const SearchSaleContractID = ref('')
const SearchShippingDateStart = ref('')
const SearchShippingDateEnd = ref('')

//查询按钮点击事件
const SearchClick = () => {
	GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
}
//重置按钮点击事件
const ResetClick = () => {
	SearchCustomerID.value = ''
	SearchSaleContractID.value = ''
	SearchShippingDateStart.value = ''
	SearchShippingDateEnd.value = ''
	GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
}

//创建出运发货单弹窗
const CreateshippingdeliveryDialog = ref(false)
//创建出运表单
const AddShippingDeliveryform = ref({
	invoiceNumber: '',
	shippingStatus: '',
	shippingDate: null,
	OrderMakingDate: null,
	invoiceDate: null,
	customerNumber: '',
	customerAbbreviation: '',
	referenceContractNumber: '',
	salesContractNumber: '',
	customerContractNumber: '',
	ourCompany: '',
	bankOfReceipt: '',
	exportCurrency: '',
	exchangeRate: '',
	priceTerms: '',
	departurePort: '',
	destinationPort: '',
	tradeCountry: '',
	settlementMethod: '',
	transportationMethod: '',
	receivableDate: null,
	documentClerk: '',
	isDeposit: 0,
	preCarriageTransport: '',
	shippingAgent: ''
})

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		sql_sale_contracts: [],
		sql_hr_customer: [],
		hr_export_currency: [],
		hr_pricing_term: [],
		hr_transport_port: [],
		hr_nation: [],
		hr_settlement_way: [],
		hr_transportation_method: [],
		sql_all_user: [],
		hr_shipping_status: [],
		hr_ourcompany: [],
		hr_bank: [],
		hr_domestic_transport: [],
		hr_freight_forwarding_company: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_hr_customer' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_pricing_term' }, { dictType: 'hr_transport_port' }, { dictType: 'hr_nation' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_transportation_method' }, { dictType: 'sql_all_user' }, { dictType: 'hr_shipping_status' },
{ dictType: 'hr_ourcompany' }, { dictType: 'hr_bank' }, { dictType: 'hr_domestic_transport' }, { dictType: 'hr_freight_forwarding_company' }]
async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		/*获取当前页面列表函数放在下方*/
		await GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);  // 现在可以安全执行
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
fetchDataAndExecute();
/*动态下拉框end*/

//出运发货表格数据
const shippingDeliveryTableData = ref([])
//产品资料表格数据
const shippingDeliveryProductTableData = ref([])
//采购明细表格数据
const shippingDeliveryPurchaseDetailsTableData = ref([])

//客户编号改变
const customerNumberChange = () => {
	//根据客户编号查询客户简称
	request({
		url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
		method: 'GET',
		params: {
			ID: AddShippingDeliveryform.value.customerNumber
		}
	}).then(response => {
		if (response != null) {
			AddShippingDeliveryform.value.customerAbbreviation = response.customerAbbreviation
		}
	}).catch(error => {
		console.log(error)
	});
}

//参考合同号改变
const referenceContractNumberChange = () => {
	var SaleContractID = AddShippingDeliveryform.value.referenceContractNumber;
	/*合同产品信息与相关费用*/
	request({
		url: 'Contracts/GetContractDetailsById/GetContractDetails',
		method: 'GET',
		params: {
			contractId: SaleContractID
		}
	}).then(response => {
		if (response.data != null) {
			AddShippingDeliveryform.value.salesContractNumber = response.data.contract.contractNumber;
			AddShippingDeliveryform.value.customerContractNumber = response.data.contract.customerContract;
			AddShippingDeliveryform.value.ourCompany = response.data.contract.ourCompany.toString();
			AddShippingDeliveryform.value.exportCurrency = response.data.contract.foreignCurrency.toString();
			AddShippingDeliveryform.value.exchangeRate = response.data.contract.exchangeRate;
			AddShippingDeliveryform.value.priceTerms = response.data.contract.priceTerms.toString();
			AddShippingDeliveryform.value.departurePort = response.data.contract.shippingPort.toString();
			AddShippingDeliveryform.value.destinationPort = response.data.contract.destinationPort.toString();
			AddShippingDeliveryform.value.tradeCountry = response.data.contract.tradeCountry.toString();
			AddShippingDeliveryform.value.settlementMethod = response.data.contract.settlementMethod.toString();
			AddShippingDeliveryform.value.transportationMethod = response.data.contract.transportation.toString();
			shippingDeliveryProductTableData.value = response.data.contractProducts;
			shippingDeliveryProductTableData.value.forEach((element) => {
				element.outerBoxUnit = '无';
			});
		}
	}).catch(error => {
		console.error(error);
	});
	request({
		url: 'PurchaseContracts/GetPurchaseContractsDetailsBySaleContractID/GetList',
		method: 'GET',
		params: {
			SaleContractID: SaleContractID
		}
	}).then(response => {
		if (response.data != null && response.data.length > 0) {
			shippingDeliveryPurchaseDetailsTableData.value = response.data;
			shippingDeliveryPurchaseDetailsTableData.value.forEach((element) => {
				element.contractQuantity = 0;
				element.CustomsDeclarationChineseProductName = '无';
				element.CustomsDeclarationEnglishProductName = '无';
				element.CustomsDeclarationUnitPrice = 0;
				element.CustomsDeclarationUnitPrice = 0;
				element.valuationMethod = '无';
				element.TotalCustomsDeclarationPrice = 0;
				element.WhetherToDeclareCustoms = '无';
				element.measurementUnit = '无';
				element.BillingUnit = '无';
			});
		}
	}).catch(error => {
		console.error(error);
	});
}

const OpenCreateshippingdeliveryDialog = () => {
	AddShippingDeliveryform.value.OrderMakingDate = new Date();
	AddShippingDeliveryform.value.invoiceDate = new Date();
	AddShippingDeliveryform.value.shippingStatus = '2';
	CreateshippingdeliveryDialog.value = true
}

const shippingDeliveriesRequest = reactive({
	Id: 0,
	InvoiceNumber: '',
	OrderMakingDate: '',
	ShippingStatus: null,
	ShippingDate: '',
	InvoiceDate: '',
	CustomerNumber: null,
	CustomerAbbreviation: '',
	ReferenceContractNumber: null,
	SalesContractNumber: '',
	CustomerContractNumber: '',
	OurCompany: null,
	BankOfReceipt: null,
	ExportCurrency: null,
	ExchangeRate: null,
	PriceTerms: null,
	DeparturePort: null,
	DestinationPort: null,
	TradeCountry: null,
	SettlementMethod: null,
	TransportationMethod: null,
	ReceivableDate: '',
	DocumentClerk: null,
	IsDeposit: null,
	PreCarriageTransport: null,
	ShippingAgent: null,
	Remark: '',
	IsDelete: 0,
	ShippingDeliveryProductItems: [],
	ShippingDeliveryPurchaseDetailsItems: []
});

//保存出运发货单
const SaveClick = () => {
	ElMessageBox.confirm('确定保存出运发货单吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		shippingDeliveriesRequest.InvoiceNumber = AddShippingDeliveryform.value.invoiceNumber;
		shippingDeliveriesRequest.OrderMakingDate = AddShippingDeliveryform.value.OrderMakingDate;
		shippingDeliveriesRequest.ShippingStatus = AddShippingDeliveryform.value.shippingStatus;
		shippingDeliveriesRequest.ShippingDate = AddShippingDeliveryform.value.shippingDate;
		shippingDeliveriesRequest.InvoiceDate = AddShippingDeliveryform.value.invoiceDate;
		shippingDeliveriesRequest.CustomerNumber = AddShippingDeliveryform.value.customerNumber;
		shippingDeliveriesRequest.CustomerAbbreviation = AddShippingDeliveryform.value.customerAbbreviation;
		shippingDeliveriesRequest.ReferenceContractNumber = AddShippingDeliveryform.value.referenceContractNumber;
		shippingDeliveriesRequest.SalesContractNumber = AddShippingDeliveryform.value.salesContractNumber;
		shippingDeliveriesRequest.CustomerContractNumber = AddShippingDeliveryform.value.customerContractNumber;
		shippingDeliveriesRequest.OurCompany = AddShippingDeliveryform.value.ourCompany;
		shippingDeliveriesRequest.BankOfReceipt = Number(AddShippingDeliveryform.value.bankOfReceipt);
		shippingDeliveriesRequest.ExportCurrency = AddShippingDeliveryform.value.exportCurrency;
		shippingDeliveriesRequest.ExchangeRate = AddShippingDeliveryform.value.exchangeRate;
		shippingDeliveriesRequest.PriceTerms = AddShippingDeliveryform.value.priceTerms;
		shippingDeliveriesRequest.DeparturePort = AddShippingDeliveryform.value.departurePort;
		shippingDeliveriesRequest.DestinationPort = AddShippingDeliveryform.value.destinationPort;
		shippingDeliveriesRequest.TradeCountry = AddShippingDeliveryform.value.tradeCountry;
		shippingDeliveriesRequest.SettlementMethod = AddShippingDeliveryform.value.settlementMethod;
		shippingDeliveriesRequest.TransportationMethod = AddShippingDeliveryform.value.transportationMethod;
		shippingDeliveriesRequest.ReceivableDate = AddShippingDeliveryform.value.receivableDate;
		shippingDeliveriesRequest.DocumentClerk = AddShippingDeliveryform.value.documentClerk;
		shippingDeliveriesRequest.IsDeposit = AddShippingDeliveryform.value.isDeposit;
		shippingDeliveriesRequest.PreCarriageTransport = AddShippingDeliveryform.value.preCarriageTransport;
		shippingDeliveriesRequest.ShippingAgent = AddShippingDeliveryform.value.shippingAgent;
		shippingDeliveriesRequest.ShippingDeliveryProductItems = shippingDeliveryProductTableData.value;
		shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems = shippingDeliveryPurchaseDetailsTableData.value;

		request.post('ShippingDeliveries/AddShippingDeliveries/Add', shippingDeliveriesRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '出运发货单保存成功！',
					type: 'success'
				})
				CreateshippingdeliveryDialog.value = false;
			} else {
				console.error('出运发货单保存出错');
			}
		}).catch(error => {
			console.error('出运发货单保存出错！😔错误内容：', error);
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}

const IsEditShippingDeliveryID = ref(0)
//编辑保存出运发货单
const EditSaveClick = () => {
	ElMessageBox.confirm('确定保存编辑后的出运发货单吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		shippingDeliveriesRequest.Id = IsEditShippingDeliveryID.value;
		shippingDeliveriesRequest.InvoiceNumber = AddShippingDeliveryform.value.invoiceNumber;
		shippingDeliveriesRequest.OrderMakingDate = AddShippingDeliveryform.value.OrderMakingDate;
		shippingDeliveriesRequest.ShippingStatus = AddShippingDeliveryform.value.shippingStatus;
		shippingDeliveriesRequest.ShippingDate = AddShippingDeliveryform.value.shippingDate;
		shippingDeliveriesRequest.InvoiceDate = AddShippingDeliveryform.value.invoiceDate;
		shippingDeliveriesRequest.CustomerNumber = AddShippingDeliveryform.value.customerNumber;
		shippingDeliveriesRequest.CustomerAbbreviation = AddShippingDeliveryform.value.customerAbbreviation;
		shippingDeliveriesRequest.ReferenceContractNumber = AddShippingDeliveryform.value.referenceContractNumber;
		shippingDeliveriesRequest.SalesContractNumber = AddShippingDeliveryform.value.salesContractNumber;
		shippingDeliveriesRequest.CustomerContractNumber = AddShippingDeliveryform.value.customerContractNumber;
		shippingDeliveriesRequest.OurCompany = AddShippingDeliveryform.value.ourCompany;
		shippingDeliveriesRequest.BankOfReceipt = Number(AddShippingDeliveryform.value.bankOfReceipt);
		shippingDeliveriesRequest.ExportCurrency = AddShippingDeliveryform.value.exportCurrency;
		shippingDeliveriesRequest.ExchangeRate = AddShippingDeliveryform.value.exchangeRate;
		shippingDeliveriesRequest.PriceTerms = AddShippingDeliveryform.value.priceTerms;
		shippingDeliveriesRequest.DeparturePort = AddShippingDeliveryform.value.departurePort;
		shippingDeliveriesRequest.DestinationPort = AddShippingDeliveryform.value.destinationPort;
		shippingDeliveriesRequest.TradeCountry = AddShippingDeliveryform.value.tradeCountry;
		shippingDeliveriesRequest.SettlementMethod = AddShippingDeliveryform.value.settlementMethod;
		shippingDeliveriesRequest.TransportationMethod = AddShippingDeliveryform.value.transportationMethod;
		shippingDeliveriesRequest.ReceivableDate = AddShippingDeliveryform.value.receivableDate;
		shippingDeliveriesRequest.DocumentClerk = AddShippingDeliveryform.value.documentClerk;
		shippingDeliveriesRequest.IsDeposit = AddShippingDeliveryform.value.isDeposit;
		shippingDeliveriesRequest.PreCarriageTransport = AddShippingDeliveryform.value.preCarriageTransport;
		shippingDeliveriesRequest.ShippingAgent = AddShippingDeliveryform.value.shippingAgent;
		shippingDeliveriesRequest.ShippingDeliveryProductItems = shippingDeliveryProductTableData.value;
		shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems = shippingDeliveryPurchaseDetailsTableData.value;

		request.post('ShippingDeliveries/EditShippingDeliveries/Edit', shippingDeliveriesRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '出运发货单编辑成功！',
					type: 'success'
				})
				CreateshippingdeliveryDialog.value = false;
			} else {
				console.error('出运发货单编辑出错');
			}
		}).catch(error => {
			console.error('出运发货单编辑出错！😔错误内容：', error);
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});

}

///出运发货单表格数据
const ShippingDeliveriesTableDataTotalItems = ref(0);
const ShippingDeliveriesTableDataCurrentPage = ref(1);
const ShippingDeliveriesTableDataPageSize = ref(10);
const paymentrequesttableDataHandlePageChange = async (newPage) => {
	ShippingDeliveriesTableDataCurrentPage.value = newPage;
	const start = newPage;
	const end = ShippingDeliveriesTableDataPageSize.value;
	const newData = await GetShippingDeliveriesList(start, end);
};
function GetShippingDeliveriesList(start, end) {
	request({
		url: 'ShippingDeliveries/GetShippingDeliveriesList/GetList',
		method: 'GET',
		params: {
			CustomerID: SearchCustomerID.value,
			SaleContractID: SearchSaleContractID.value,
			ShippingDateStart: SearchShippingDateStart.value,
			ShippingDateEnd: SearchShippingDateEnd.value
		}
	}).then(response => {
		if (response.data != null) {
			shippingDeliveryTableData.value = response.data.result;
		}
	}).catch(error => {
		console.error(error);
	});
}

const CheckShipingDelivery = (row) => {
	isEditSaveBtnShow.value = true;
	isSaveBtnShow.value = false;
	request({
		url: 'ShippingDeliveries/GetShippingDeliveriesDetailsByid/GetShippingDeliveriesDetails',
		method: 'GET',
		params: {
			ShippingDeliveriesId: row.id
		}
	}).then(response => {
		if (response.data.shippingDeliveries != null) {
			IsEditShippingDeliveryID.value = response.data.shippingDeliveries.id;
			AddShippingDeliveryform.value.invoiceNumber = response.data.shippingDeliveries.invoiceNumber;
			AddShippingDeliveryform.value.OrderMakingDate = response.data.shippingDeliveries.createTime;
			AddShippingDeliveryform.value.shippingStatus = response.data.shippingDeliveries.shippingStatus.toString();
			AddShippingDeliveryform.value.shippingDate = response.data.shippingDeliveries.shippingDate;
			AddShippingDeliveryform.value.invoiceDate = response.data.shippingDeliveries.invoiceDate;
			AddShippingDeliveryform.value.customerNumber = response.data.shippingDeliveries.customerNumber.toString();
			AddShippingDeliveryform.value.customerAbbreviation = response.data.shippingDeliveries.customerAbbreviation;
			AddShippingDeliveryform.value.referenceContractNumber = response.data.shippingDeliveries.referenceContractNumber.toString();
			AddShippingDeliveryform.value.salesContractNumber = response.data.shippingDeliveries.salesContractNumber;
			AddShippingDeliveryform.value.customerContractNumber = response.data.shippingDeliveries.customerContractNumber;
			AddShippingDeliveryform.value.ourCompany = response.data.shippingDeliveries.ourCompany.toString();
			AddShippingDeliveryform.value.bankOfReceipt = response.data.shippingDeliveries.bankOfReceipt.toString();
			AddShippingDeliveryform.value.exportCurrency = response.data.shippingDeliveries.exportCurrency.toString();
			AddShippingDeliveryform.value.exchangeRate = response.data.shippingDeliveries.exchangeRate;
			AddShippingDeliveryform.value.priceTerms = response.data.shippingDeliveries.priceTerms.toString();
			AddShippingDeliveryform.value.departurePort = response.data.shippingDeliveries.departurePort.toString();
			AddShippingDeliveryform.value.destinationPort = response.data.shippingDeliveries.destinationPort.toString();
			AddShippingDeliveryform.value.tradeCountry = response.data.shippingDeliveries.tradeCountry.toString();
			AddShippingDeliveryform.value.settlementMethod = response.data.shippingDeliveries.settlementMethod.toString();
			AddShippingDeliveryform.value.transportationMethod = response.data.shippingDeliveries.transportationMethod.toString();
			AddShippingDeliveryform.value.receivableDate = response.data.shippingDeliveries.receivableDate;
			AddShippingDeliveryform.value.documentClerk = response.data.shippingDeliveries.documentClerk.toString();
			AddShippingDeliveryform.value.isDeposit = response.data.shippingDeliveries.isDeposit;
			AddShippingDeliveryform.value.preCarriageTransport = response.data.shippingDeliveries.preCarriageTransport.toString();
			AddShippingDeliveryform.value.shippingAgent = response.data.shippingDeliveries.shippingAgent.toString();
		}
		if (response.data.shippingDeliveryProducts.length > 0) {
			shippingDeliveryProductTableData.value = response.data.shippingDeliveryProducts;
		}
		if (response.data.shippingDeliveryPurchaseDetails != null) {
			shippingDeliveryPurchaseDetailsTableData.value = response.data.shippingDeliveryPurchaseDetails;
		}
		CreateshippingdeliveryDialog.value = true;
	}).catch(error => {
		console.error(error);
	});
};

const CreateshippingdeliveryDialogClose = () => {
	CreateshippingdeliveryDialog.value = false;
	isEditSaveBtnShow.value = false;
	isSaveBtnShow.value = true;
	IsEditShippingDeliveryID.value = 0;
	AddShippingDeliveryform.value.invoiceNumber = '';
	AddShippingDeliveryform.value.shippingStatus = '';
	AddShippingDeliveryform.value.shippingDate = null;
	AddShippingDeliveryform.value.OrderMakingDate = null;
	AddShippingDeliveryform.value.invoiceDate = null;
	AddShippingDeliveryform.value.customerNumber = '';
	AddShippingDeliveryform.value.customerAbbreviation = '';
	AddShippingDeliveryform.value.referenceContractNumber = '';
	AddShippingDeliveryform.value.salesContractNumber = '';
	AddShippingDeliveryform.value.customerContractNumber = '';
	AddShippingDeliveryform.value.ourCompany = '';
	AddShippingDeliveryform.value.bankOfReceipt = '';
	AddShippingDeliveryform.value.exportCurrency = '';
	AddShippingDeliveryform.value.exchangeRate = '';
	AddShippingDeliveryform.value.priceTerms = '';
	AddShippingDeliveryform.value.departurePort = '';
	AddShippingDeliveryform.value.destinationPort = '';
	AddShippingDeliveryform.value.tradeCountry = '';
	AddShippingDeliveryform.value.settlementMethod = '';
	AddShippingDeliveryform.value.transportationMethod = '';
	AddShippingDeliveryform.value.receivableDate = null;
	AddShippingDeliveryform.value.documentClerk = '';
	AddShippingDeliveryform.value.isDeposit = 0;
	AddShippingDeliveryform.value.preCarriageTransport = '';
	AddShippingDeliveryform.value.shippingAgent = '';
	shippingDeliveryProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
}
</script>