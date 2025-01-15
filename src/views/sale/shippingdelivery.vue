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
			<el-date-picker v-model="SearchShippingDateStart" type="date" placeholder="请选择出运日期"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchShippingDateEnd" type="date" placeholder="请选择出运日期" style="width: 15%" />
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
			<el-table-column prop="id" label="出运发货单ID" width="150px" v-if="false"></el-table-column>
			<el-table-column prop="invoiceNumber" label="出运单号" width="150px"></el-table-column>
			<el-table-column prop="salesContractNumber" label="销售合同号" width="150px"></el-table-column>
			<el-table-column prop="createTime" label="制单日期" width="150px"></el-table-column>
			<el-table-column prop="shippingStatus" label="出运状态" width="150px"></el-table-column>
			<el-table-column prop="reviewStatus" label="审核状态编号" width="150" v-if="false"></el-table-column>
			<el-table-column prop="ReviewStatusStr" label="审核状态" width="150"></el-table-column>
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
							<el-input v-model="AddShippingDeliveryform.invoiceNumber" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="制单日期">
							<el-date-picker v-model="AddShippingDeliveryform.OrderMakingDate" type="date"
								style="width: 300px" :disabled="IsEditable"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="出运状态">
							<el-select filterable v-model="AddShippingDeliveryform.shippingStatus" style="width: 300px"
								:disabled="IsEditable">
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
								style="width: 300px" :disabled="IsEditable"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="发票日期">
							<el-date-picker v-model="AddShippingDeliveryform.invoiceDate" type="date"
								style="width: 300px" :disabled="IsEditable"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户编号">
							<el-select filterable v-model="AddShippingDeliveryform.customerNumber"
								placeholder="选择客户（可输入查询）" style="width: 300px" @change="customerNumberChange()"
								clearable :disabled="IsEditable">
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
								placeholder="选择销售合同（可输入查询）" style="width: 300px" clearable
								@change="referenceContractNumberChange()" :disabled="IsEditable">
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
							<el-select filterable v-model="AddShippingDeliveryform.bankOfReceipt" style="width: 300px"
								:disabled="IsEditable">
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
							<el-input v-model="AddShippingDeliveryform.destinationPort" style="width: 300px"
								disabled></el-input>
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
								style="width: 300px" :disabled="IsEditable"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="单证员">
							<el-select filterable v-model="AddShippingDeliveryform.documentClerk" placeholder="选择单证员"
								style="width: 300px" :disabled="IsEditable">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="有无定金">
							<el-checkbox v-model="AddShippingDeliveryform.isDeposit"
								:disabled="IsEditable"></el-checkbox>
						</el-form-item>

					</el-col>
				</el-row>
			</el-form>
			<br><span style="font-size: 20px; font-weight: bold;">辅助信息</span>
			<el-divider></el-divider>
			<el-form label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="前程运输">
							<el-select filterable v-model="AddShippingDeliveryform.preCarriageTransport"
								style="width: 300px" :disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_domestic_transport" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="船代公司">
							<el-select filterable v-model="AddShippingDeliveryform.shippingAgent" style="width: 300px"
								:disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_freight_forwarding_company" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<br><span style="font-size: 20px; font-weight: bold;">销售合同</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryContrctProductTableData">
				<el-table-column prop="contractId" label="销售合同" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractProductId" label="销售合同明细ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractNumber" label="销售合同" width="150"></el-table-column>
				<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
				<el-table-column prop="RemainingQuantityToBeShipped" label="剩余待出货数量" width="150"
					v-if="false"></el-table-column>
				<el-table-column prop="shipmentQuantity" label="出货数量" width="150">
					<template #default="scope">
						<el-input v-model="scope.row.shipmentQuantity" :disabled="IsEditable" style="width: 100%"
							@change="shipmentQuantityChange(scope.row)"></el-input>
					</template>
				</el-table-column>
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
				<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
					<template v-slot:default="scope">
						<el-button link type="primary" size="small"
							@click="DeleteShippingDeliveryContrctProduct(scope.$index)"
							:disabled="IsEditable">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<br><span style="font-size: 20px; font-weight: bold;">采购合同</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryPurchaseDetailsTableData">
				<el-table-column prop="purchaseContractID" label="采购合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="purchaseContractProductID" label="采购合同明细ID" width="150"
					v-if="false"></el-table-column>
				<el-table-column prop="purchaseContractNumber" label="采购合同" width="150"></el-table-column>
				<el-table-column prop="vendorAbbreviation" label="厂商简称" width="150"></el-table-column>
				<el-table-column prop="productNumber" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
				<el-table-column prop="shipmentQuantity" label="出货数量" width="150">
					<template #default="scope">
						<el-input v-model="scope.row.shipmentQuantity" disabled style="width: 100%"></el-input>
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
				<el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
				<el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
				<el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
				<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
					<template v-slot:default="scope">
						<el-button link type="primary" size="small"
							@click="DeleteShippingDeliveryPurchaseDetails(scope.$index)"
							:disabled="IsEditable">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<br><span style="font-size: 20px; font-weight: bold;">备注信息</span>
			<el-divider></el-divider>
			<el-form-item label="备注：" style="width: 100%;">
				<el-input v-model="AddShippingDeliveryform.remark" :autosize="{ minRows: 5, maxRows: 10 }"
					type="textarea" placeholder="输入备注内容" :disabled="IsEditable" />
			</el-form-item>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isSaveBtnShow" type="primary" @click="SaveClick()">
						确定保存
					</el-button>
					<el-button type="primary" v-show="isEditBtnShow" @click="EditClick()">
						编辑
					</el-button>
					<el-button type="primary" v-show="isEditSaveBtnShow" @click="EditSaveClick()">
						编辑保存
					</el-button>
					<el-button type="warning" v-show="isReviewBtnShow" @click="SubmitReview">
						提交审核
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
import Supperinfomation from '../purchase/supperinfomation.vue';
import dayjs from 'dayjs';

//是否可编辑
const IsEditable = ref(false) //是否可编辑
const isReviewBtnShow = ref(false) //提交审核按钮是否显示
const isEditBtnShow = ref(false) //编辑按钮是否显示
const isSaveBtnShow = ref(true) //保存按钮是否显示
const isEditSaveBtnShow = ref(false) //编辑保存按钮是否显示

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
	destinationPort: null,
	tradeCountry: '',
	settlementMethod: '',
	transportationMethod: '',
	receivableDate: null,
	documentClerk: '',
	isDeposit: 0,
	preCarriageTransport: '',
	shippingAgent: '',
	remark: ''
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
		hr_freight_forwarding_company: [],
		hr_calculate_unit: [],
		hr_outerbox_unit: [],
		sql_supplier_info: [],
		sql_product_name: [],
		sql_product: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_hr_customer' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_pricing_term' }, { dictType: 'hr_transport_port' }, { dictType: 'hr_nation' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_transportation_method' }, { dictType: 'sql_all_user' }, { dictType: 'hr_shipping_status' },
{ dictType: 'hr_ourcompany' }, { dictType: 'hr_bank' }, { dictType: 'hr_domestic_transport' },
{ dictType: 'hr_freight_forwarding_company' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'sql_supplier_info' }, { dictType: 'sql_product_name' }, { dictType: 'sql_product' }]
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
const shippingDeliveryContrctProductTableData = ref([])
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
	//根据客户ID获取所有销售合同
	request({
		url: 'Contracts/GetCustomerContractByCustomerID/GetAllCotracts',
		method: 'GET',
		params: {
			CustomerID: AddShippingDeliveryform.value.customerNumber
		}
	}).then(response => {
		shippingDeliveryContrctProductTableData.value = [];
		response.data.forEach((element) => {
			var ShippingQuantity = 0;
			request({
				url: 'ShippingDeliveries/GetShippingQuantityByContractIDAndProductID/GetShippingQuantity',
				method: 'GET',
				params: {
					ContractID: element.contractId,
					ProductID: element.id
				}
			}).then(ShippingQuantityResponse => {
				ShippingQuantity = ShippingQuantityResponse.data;
				shippingDeliveryContrctProductTableData.value.push({
					contractProductID: element.id,
					contractId: element.contractId,
					contractNumber: element.contractNumber,
					productCode: element.productCode,
					chineseName: element.chineseName,
					contractQuantity: element.contractQuantity,
					RemainingQuantityToBeShipped: element.contractQuantity - ShippingQuantity,
					shipmentQuantity: element.contractQuantity - ShippingQuantity,
					unit: element.unit,
					exportUnitPrice: element.exportUnitPrice,
					exportTotalPrice: element.exportTotalPrice,
					specialRequirements: element.specialRequirements,
					outerBoxQuantity: element.outerBoxQuantity,
					boxCount: element.boxCount,
					outerBoxUnit: element.outerboxunit,
					outerBoxLength: element.outerBoxLength,
					outerBoxWidth: element.outerBoxWidth,
					outerBoxHeight: element.outerBoxHeight,
					outerBoxVolume: element.outerBoxVolume,
					totalVolume: element.totalVolume,
					outerBoxNetWeight: element.outerBoxNetWeight,
					outerBoxGrossWeight: element.outerBoxGrossWeight,
					totalNetWeight: element.totalNetWeight,
					totalGrossWeight: element.totalGrossWeight
				});
			});
		}).catch(error => {
			console.log(error)
		})
		shippingDeliveryContrctProductTableData.value.forEach((element) => {
			element.unit = state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无';
			element.outerBoxUnit = state.optionss.hr_outerbox_unit.find(item => item.dictValue === element.outerBoxUnit.toString())?.dictLabel || '无';
		});
	}).catch(error => {
		console.log(error)
	});
	//根据客户ID获取所有采购合同
	request({
		url: 'PurchaseContracts/GetCustomerPurchasesByCustomerID/GetAllPurchasess',
		method: 'GET',
		params: {
			CustomerID: AddShippingDeliveryform.value.customerNumber
		}
	}).then(response => {
		shippingDeliveryPurchaseDetailsTableData.value = [];
		response.data.forEach((element) => {
			shippingDeliveryPurchaseDetailsTableData.value.push({
				purchaseContractID: element.purchaseContractID,
				purchaseContractProductID: element.purchaseContractProductID,
				purchaseContractNumber: element.purchaseContractNumber,
				shipmentQuantity: element.contractQuantity,
				vendorAbbreviation: state.optionss.sql_supplier_info.find(item => item.dictValue === element.supplierID.toString())?.dictLabel || '无',
				productNumber: state.optionss.sql_product.find(item => item.dictValue === element.productNumber.toString())?.dictLabel,
				chineseName: element.chineseName,
				purchaseCurrency: state.optionss.hr_export_currency.find(item => item.dictValue === element.purchaseCurrency.toString())?.dictLabel || '无',
				purchaseUnitPrice: element.purchasePrice,
				purchaseTotalPrice: element.purchaseTotalPrice,
				measurementUnit: state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无',
				invoice: element.invoice,
				totalVolume: element.TotalVolume,
				totalGrossWeight: element.TotalGrossWeight,
				contractQuantity: element.contractQuantity
			});
		});

	}).catch(error => {
		console.log(error)
	});
}

//参考合同号改变
const referenceContractNumberChange = () => {
	var SaleContractID = AddShippingDeliveryform.value.referenceContractNumber;
	if (SaleContractID == '' || SaleContractID == null || SaleContractID == undefined) {
		shippingDeliveryContrctProductTableData.value = [];
		shippingDeliveryPurchaseDetailsTableData.value = [];
		return;
	}
	//根据参考合同号获取销售合同信息
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
			shippingDeliveryContrctProductTableData.value = [];
			response.data.contractProducts.forEach((element) => {
				var ShippingQuantity = 0;
				request({
					url: 'ShippingDeliveries/GetShippingQuantityByContractIDAndProductID/GetShippingQuantity',
					method: 'GET',
					params: {
						ContractID: element.contractId,
						ProductID: element.id
					}
				}).then(ShippingQuantityResponse => {
					ShippingQuantity = ShippingQuantityResponse.data;
					shippingDeliveryContrctProductTableData.value.push({
						contractId: element.contractId,
						contractProductId: element.id,
						contractNumber: response.data.contract.contractNumber,
						productCode: element.productCode,
						chineseName: element.chineseName,
						contractQuantity: element.contractQuantity,
						RemainingQuantityToBeShipped: element.contractQuantity - ShippingQuantity,
						shipmentQuantity: element.contractQuantity - ShippingQuantity,
						unit: state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无',
						exportUnitPrice: element.exportUnitPrice,
						exportTotalPrice: element.exportTotalPrice,
						specialRequirements: element.specialRequirements,
						outerBoxQuantity: element.outerBoxQuantity,
						boxCount: element.boxCount,
						outerBoxUnit: state.optionss.hr_outerbox_unit.find(item => item.dictValue === element.outerboxunit.toString())?.dictLabel || '无',
						outerBoxLength: element.outerBoxLength,
						outerBoxWidth: element.outerBoxWidth,
						outerBoxHeight: element.outerBoxHeight,
						outerBoxVolume: element.outerBoxVolume,
						totalVolume: element.totalVolume,
						outerBoxNetWeight: element.outerBoxNetWeight,
						outerBoxGrossWeight: element.outerBoxGrossWeight,
						totalNetWeight: element.totalNetWeight,
						totalGrossWeight: element.totalGrossWeight
					});
				});
			}).catch(error => {
				console.log(error)
			})

		}
	}).catch(error => {
		console.error(error);
	});
	//根据参考合同号获取采购合同信息
	request({
		url: 'PurchaseContracts/GetCustomerPurchasesByContractID/GetAllPurchasess',
		method: 'GET',
		params: {
			ContractID: SaleContractID
		}
	}).then(response => {
		if (response.data != null && response.data.length > 0) {
			shippingDeliveryPurchaseDetailsTableData.value = [];
			response.data.forEach((element) => {
				shippingDeliveryPurchaseDetailsTableData.value.push({
					purchaseContractID: element.purchaseContractID,
					purchaseContractProductID: element.purchaseContractProductID,
					purchaseContractNumber: element.purchaseContractNumber,
					purchaseShippingNumber: element.purchaseContractNumber,
					shipmentQuantity: element.contractQuantity,
					vendorAbbreviation: state.optionss.sql_supplier_info.find(item => item.dictValue === element.supplierID.toString())?.dictLabel || '无',
					productNumber: state.optionss.sql_product.find(item => item.dictValue === element.productNumber.toString())?.dictLabel,
					chineseName: element.chineseName,
					purchaseCurrency: state.optionss.hr_export_currency.find(item => item.dictValue === element.purchaseCurrency.toString())?.dictLabel || '无',
					purchaseUnitPrice: element.purchasePrice,
					purchaseTotalPrice: element.purchaseTotalPrice,
					measurementUnit: state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无',
					invoice: element.invoice,
					totalVolume: element.TotalVolume,
					totalGrossWeight: element.TotalGrossWeight,
					contractQuantity: element.contractQuantity
				});
			});
		}
	}).catch(error => {
		console.error(error);
	});
}

const isCreateMode = ref(true) //是否是创建模式
const isEditable = ref(true) //是否可编辑
const dialogVisible = ref(false) //弹窗是否显示

//打开弹窗
const OpenCreateshippingdeliveryDialog = () => {
	IsEditable.value = false;
	isCreateMode.value = true;
	dialogVisible.value = true;

	// 重置表单
	AddShippingDeliveryform.value = {
		invoiceNumber: '',          // 发票号码会被自动填充
		shippingStatus: '2',
		shippingDate: null,
		OrderMakingDate: new Date(),  // 默认当前日期
		invoiceDate: new Date(),
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
	};

	// 清空产品数据
	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];

	// 获取新的发票号码
	getNextShippingNumber();
	CreateshippingdeliveryDialog.value = true;
}

//出运发货单请求数据
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
const SaveClick = async () => {
	// 1. 数据验证
	if (!validateForm()) {
		return;
	}

	try {
		await ElMessageBox.confirm('确定保存出运发货单吗?', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 2. 构建请求数据
		const requestData = {
			// 基本信息
			InvoiceNumber: AddShippingDeliveryform.value.invoiceNumber,
			OrderMakingDate: AddShippingDeliveryform.value.OrderMakingDate,
			ShippingStatus: Number(AddShippingDeliveryform.value.shippingStatus),
			ShippingDate: AddShippingDeliveryform.value.shippingDate,
			InvoiceDate: AddShippingDeliveryform.value.invoiceDate,
			CustomerNumber: Number(AddShippingDeliveryform.value.customerNumber),
			CustomerAbbreviation: AddShippingDeliveryform.value.customerAbbreviation,
			ReferenceContractNumber: AddShippingDeliveryform.value.referenceContractNumber ?
				Number(AddShippingDeliveryform.value.referenceContractNumber) : null,
			SalesContractNumber: AddShippingDeliveryform.value.salesContractNumber,
			CustomerContractNumber: AddShippingDeliveryform.value.customerContractNumber,
			OurCompany: Number(AddShippingDeliveryform.value.ourCompany),
			BankOfReceipt: AddShippingDeliveryform.value.bankOfReceipt ?
				Number(AddShippingDeliveryform.value.bankOfReceipt) : null,
			ExportCurrency: AddShippingDeliveryform.value.exportCurrency ?
				Number(AddShippingDeliveryform.value.exportCurrency) : null,
			ExchangeRate: AddShippingDeliveryform.value.exchangeRate ?
				Number(AddShippingDeliveryform.value.exchangeRate) : null,
			PriceTerms: AddShippingDeliveryform.value.priceTerms ?
				Number(AddShippingDeliveryform.value.priceTerms) : null,
			DeparturePort: AddShippingDeliveryform.value.departurePort ?
				Number(AddShippingDeliveryform.value.departurePort) : null,
			DestinationPort: AddShippingDeliveryform.value.destinationPort ?
				AddShippingDeliveryform.value.destinationPort : null,
			TradeCountry: AddShippingDeliveryform.value.tradeCountry ?
				Number(AddShippingDeliveryform.value.tradeCountry) : null,
			SettlementMethod: AddShippingDeliveryform.value.settlementMethod ?
				Number(AddShippingDeliveryform.value.settlementMethod) : null,
			TransportationMethod: AddShippingDeliveryform.value.transportationMethod ?
				Number(AddShippingDeliveryform.value.transportationMethod) : null,
			ReceivableDate: AddShippingDeliveryform.value.receivableDate,
			DocumentClerk: Number(AddShippingDeliveryform.value.documentClerk),
			IsDeposit: AddShippingDeliveryform.value.isDeposit ? false : true,
			PreCarriageTransport: AddShippingDeliveryform.value.preCarriageTransport ?
				Number(AddShippingDeliveryform.value.preCarriageTransport) : null,
			ShippingAgent: AddShippingDeliveryform.value.shippingAgent ?
				Number(AddShippingDeliveryform.value.shippingAgent) : null,
			Remark: AddShippingDeliveryform.value.remark,
			IsDelete: 0,

			// 产品明细
			ShippingDeliveryProductItems: shippingDeliveryContrctProductTableData.value.map(item => ({
				ContractId: item.contractId,
				ContractProductId: item.contractProductId,
				contractQuantity: item.contractQuantity,
				ShipmentQuantity: Number(item.shipmentQuantity),
				RemainingQuantity: Number(item.contractQuantity) - Number(item.shipmentQuantity),
				IsDelete: 0,
				Remark: item.remark || ''
			})),

			// 采购明细
			ShippingDeliveryPurchaseDetailsItems: shippingDeliveryPurchaseDetailsTableData.value.map(item => ({
				PurchaseContractID: item.purchaseContractID,
				PurchaseContractProductID: item.purchaseContractProductID,
				ContractQuantity: Number(item.contractQuantity),
				ShipmentQuantity: Number(item.shipmentQuantity),
				RemainingQuantity: Number(item.contractQuantity) - Number(item.shipmentQuantity),
				IsDelete: 0,
				Remark: item.remark || ''
			}))
		};

		// 3. 发送保存请求
		const response = await request.post('ShippingDeliveries/AddShippingDeliveries/Add', requestData);

		// 4. 处理响应
		if (response.code === 200) {
			ElMessage({
				message: response.msg || '出运发货单保存成功！',
				type: 'success'
			});

			// 关闭弹窗
			CreateshippingdeliveryDialog.value = false;

			// 重置表单
			resetForm();

			// 刷新列表
			await GetShippingDeliveriesList(
				ShippingDeliveriesTableDataCurrentPage.value,
				ShippingDeliveriesTableDataPageSize.value
			);
		} else {
			throw new Error(response.msg || '保存失败');
		}

	} catch (error) {
		if (error === 'cancel') {
			ElMessage({
				type: 'info',
				message: '已取消保存'
			});
		} else {
			console.error('保存出运发货单出错：', error);
			ElMessage.error(error.message || '系统错误，请稍后重试');
		}
	}
};

// 表单验证
const validateForm = () => {
	// 必填字段验证
	const requiredFields = {
		invoiceNumber: '发票号码',
		OrderMakingDate: '制单日期',
		shippingStatus: '出运状态',
		customerNumber: '客户编号',
		documentClerk: '单证员'
	};

	for (const [field, label] of Object.entries(requiredFields)) {
		if (!AddShippingDeliveryform.value[field]) {
			ElMessage.warning(`请填写${label}`);
			return false;
		}
	}

	// 验证是否有产品明细
	if (!shippingDeliveryContrctProductTableData.value.length) {
		ElMessage.warning('请添加产品明细');
		return false;
	}

	// 验证产品数量
	for (const product of shippingDeliveryContrctProductTableData.value) {
		if (!product.shipmentQuantity || product.shipmentQuantity <= 0) {
			ElMessage.warning(`产品 ${product.productCode} 的出货数量必须大于0`);
			return false;
		}
	}

	return true;
};

// 重置表单
const resetForm = () => {
	AddShippingDeliveryform.value = {
		invoiceNumber: '',
		shippingStatus: '2',
		shippingDate: null,
		OrderMakingDate: new Date(),
		invoiceDate: new Date(),
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
	};

	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
};

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
		shippingDeliveriesRequest.ShippingDeliveryProductItems = shippingDeliveryContrctProductTableData.value;
		shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems = shippingDeliveryPurchaseDetailsTableData.value;

		request.post('ShippingDeliveries/EditShippingDeliveries/Edit', shippingDeliveriesRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '出运发货单编辑成功！',
					type: 'success'
				});
				IsEditable.value = true;
				isEditBtnShow.value = true;
				isReviewBtnShow.value = true;
				isEditSaveBtnShow.value = false;
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
			shippingDeliveryTableData.value.forEach(item => {
				item.shippingStatus = state.optionss.hr_shipping_status.find(s => s.dictValue === item.shippingStatus.toString())?.dictLabel || '无';
				item.customerNumber = state.optionss.sql_hr_customer.find(c => c.dictValue === item.customerNumber.toString())?.dictLabel || '无';
				item.ourCompany = state.optionss.hr_ourcompany.find(c => c.dictValue === item.ourCompany.toString())?.dictLabel || '无';
				item.bankOfReceipt = !item.bankOfReceipt ? '无' : state.optionss.hr_bank.find(c => c.dictValue === item.bankOfReceipt.toString())?.dictLabel || '无';
				item.exportCurrency = state.optionss.hr_export_currency.find(c => c.dictValue === item.exportCurrency.toString())?.dictLabel || '无';
				item.departurePort = state.optionss.hr_transport_port.find(c => c.dictValue === item.departurePort.toString())?.dictLabel || '无';
				item.destinationPort = item.destinationPort.toString() || '无';
				item.priceTerms = state.optionss.hr_pricing_term.find(c => c.dictValue === item.priceTerms.toString())?.dictLabel || '无';
				item.settlementMethod = state.optionss.hr_settlement_way.find(c => c.dictValue === item.settlementMethod.toString())?.dictLabel || '无';
				item.transportationMethod = state.optionss.hr_transportation_method.find(c => c.dictValue === item.transportationMethod.toString())?.dictLabel || '无';
				item.tradeCountry = state.optionss.hr_nation.find(c => c.dictValue === item.tradeCountry.toString())?.dictLabel || '无';
				item.shippingDate = item.shippingDate ? dayjs(item.shippingDate).format('YYYY-MM-DD') : '';
				item.invoiceDate = item.invoiceDate ? dayjs(item.invoiceDate).format('YYYY-MM-DD') : '';
				item.OrderMakingDate = item.OrderMakingDate ? dayjs(item.OrderMakingDate).format('YYYY-MM-DD') : '';
				item.receivableDate = item.receivableDate ? dayjs(item.receivableDate).format('YYYY-MM-DD') : '';
				item.createTime = item.createTime ? dayjs(item.createTime).format('YYYY-MM-DD') : '';
				switch (item.reviewStatus) {
					case 0:
						item.ReviewStatusStr = '待提审';
						break;
					case 1:
						item.ReviewStatusStr = '审核中';
						break;
					case 2:
						item.ReviewStatusStr = '已批准';
						break;
					case 3:
						item.ReviewStatusStr = '已拒绝';
						break;
				}
			});
		}
	}).catch(error => {
		console.error(error);
	});
}

//检查出运发货单
const CheckShipingDelivery = (row) => {
	// 根据审核状态设置按钮显示
	const reviewStatus = row.reviewStatus;
	// 如果是审核中(1)或已批准(2),所有按钮都不显示
	if (reviewStatus === 1 || reviewStatus === 2) {
		IsEditable.value = true; // 设为不可编辑
		isReviewBtnShow.value = false; // 隐藏提交审核按钮
		isEditBtnShow.value = false; // 隐藏编辑按钮 
		isEditSaveBtnShow.value = false; // 隐藏编辑保存按钮
		isSaveBtnShow.value = false; // 隐藏保存按钮
	} else {
		// 其他状态(0:待提审 3:已拒绝)显示正常按钮
		IsEditable.value = true;
		isReviewBtnShow.value = true;
		isEditBtnShow.value = true;
		isEditSaveBtnShow.value = false;
		isSaveBtnShow.value = false;
	}
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
			AddShippingDeliveryform.value.bankOfReceipt = response.data.shippingDeliveries.bankOfReceipt ? response.data.shippingDeliveries.bankOfReceipt.toString() : '';
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
			shippingDeliveryContrctProductTableData.value = response.data.shippingDeliveryProducts;
			shippingDeliveryContrctProductTableData.value.forEach(item => {
				request({
					url: 'Contracts/GetCustomerContractProductByCPID/GetCustomerContractProduct',
					method: 'GET',
					params: {
						CPID: item.contractProductId
					}
				}).then(response => {
					if (response.data != null && response.data.length > 0) {
						// 找到当前产品在表格数据中的索引
						const index = shippingDeliveryContrctProductTableData.value.findIndex(
							x => x.contractProductId === item.contractProductId
						);
						if (index !== -1) {
							// 更新产品信息
							const productData = response.data[0];
							shippingDeliveryContrctProductTableData.value[index] = {
								...shippingDeliveryContrctProductTableData.value[index], // 保留原有数据
								contractNumber: productData.contractNumber,
								productCode: productData.productCode,
								chineseName: productData.chineseName,
								contractQuantity: productData.contractQuantity,
								unit: state.optionss.hr_calculate_unit.find(u => u.dictValue === productData.unit.toString())?.dictLabel || '无',
								exportUnitPrice: productData.exportUnitPrice,
								exportTotalPrice: productData.exportTotalPrice,
								specialRequirements: productData.specialRequirements,
								outerBoxQuantity: productData.outerBoxQuantity,
								boxCount: productData.boxCount,
								outerBoxUnit: state.optionss.hr_outerbox_unit.find(u => u.dictValue === productData.outerboxunit.toString())?.dictLabel || '无',
								outerBoxLength: productData.outerBoxLength,
								outerBoxWidth: productData.outerBoxWidth,
								outerBoxHeight: productData.outerBoxHeight,
								outerBoxVolume: productData.outerBoxVolume,
								totalVolume: productData.totalVolume,
								outerBoxNetWeight: productData.outerBoxNetWeight,
								outerBoxGrossWeight: productData.outerBoxGrossWeight,
								totalNetWeight: productData.totalNetWeight,
								totalGrossWeight: productData.totalGrossWeight
							};
						}
					}
				}).catch(error => {
					console.error('获取产品详情失败:', error);
					ElMessage.error('获取产品详情失败，请稍后重试');
				});
			});
		}
		if (response.data.shippingDeliveryPurchaseDetails != null) {
			shippingDeliveryPurchaseDetailsTableData.value = response.data.shippingDeliveryPurchaseDetails;
			shippingDeliveryPurchaseDetailsTableData.value.forEach(item => {
				request({
					url: 'PurchaseContracts/GetPurchaseProductByPPID/GetPurchaseProduct',
					method: 'GET',
					params: {
						PurchaseProductID: item.purchaseContractProductID
					}
				}).then(response => {
					if (response.data != null && response.data.length > 0) {
						// 找到当前采购明细在表格数据中的索引
						const index = shippingDeliveryPurchaseDetailsTableData.value.findIndex(
							x => x.purchaseContractProductID === item.purchaseContractProductID
						);

						if (index !== -1) {
							// 更新采购明细信息
							const purchaseData = response.data[0];
							shippingDeliveryPurchaseDetailsTableData.value[index] = {
								...shippingDeliveryPurchaseDetailsTableData.value[index], // 保留原有数据
								purchaseContractNumber: purchaseData.purchaseContractNumber,
								vendorAbbreviation: state.optionss.sql_supplier_info.find(s => s.dictValue === purchaseData.supplierID.toString())?.dictLabel || '无',
								productNumber: state.optionss.sql_product.find(p => p.dictValue === purchaseData.productNumber.toString())?.dictLabel || '无',
								chineseName: purchaseData.chineseName,
								purchaseCurrency: state.optionss.hr_export_currency.find(c => c.dictValue === purchaseData.purchaseCurrency.toString())?.dictLabel || '无',
								purchaseUnitPrice: purchaseData.purchasePrice,
								purchaseTotalPrice: purchaseData.purchaseTotalPrice,
								measurementUnit: state.optionss.hr_calculate_unit.find(u => u.dictValue === purchaseData.unit.toString())?.dictLabel || '无',
								invoice: purchaseData.invoice === 1 ? '是' : '否',
								totalVolume: purchaseData.totalVolume,
								totalGrossWeight: purchaseData.totalGrossWeight,
								contractQuantity: purchaseData.contractQuantity
							};
						}
					}
				}).catch(error => {
					console.error('获取采购明细失败:', error);
					ElMessage.error('获取采购明细失败，请稍后重试');
				});
			});
		}
		GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
		CreateshippingdeliveryDialog.value = true;
	}).catch(error => {
		console.error(error);
	});
};

//关闭弹窗
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
	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
}

// 获取下一个出运发货单编号
const getNextShippingNumber = async () => {
	try {
		const response = await request({
			url: 'ShippingDeliveries/GetNextShippingNumber/GetNextNumber',
			method: 'GET'
		});
		if (response.code === 200) {
			// 更新表单中的发票号码
			AddShippingDeliveryform.value.invoiceNumber = response.data;
		} else {
			ElMessage.warning(response.msg || '获取出运发货单编号失败');
		}
	} catch (error) {
		console.error('获取出运发货单编号失败:', error);
		ElMessage.error('获取出运发货单编号失败，请稍后重试');
	}
};

// 删除销售合同产品
const DeleteShippingDeliveryContrctProduct = (row) => {
	ElMessageBox.confirm(
		'确认要删除该产品吗？',
		'警告',
		{
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}
	).then(() => {
		shippingDeliveryContrctProductTableData.value.splice(row, 1);
		ElMessage.success('删除成功');
	}).catch(() => {
		// 用户取消删除操作
		ElMessage.info('已取消删除');
	});
};
// 删除采购明细
const DeleteShippingDeliveryPurchaseDetails = (row) => {
	ElMessageBox.confirm(
		'确认要删除该采购明细吗？',
		'警告',
		{
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}
	).then(() => {
		// 从数组中删除该采购明细
		shippingDeliveryPurchaseDetailsTableData.value.splice(row, 1);
		ElMessage.success('删除成功');

	}).catch(() => {
		// 用户取消删除操作
		ElMessage.info('已取消删除');
	});
};

// 发货数量变化处理
const shipmentQuantityChange = (row) => {
	// // 转换为数字类型
	// const newQuantity = parseFloat(row.shipmentQuantity);
	// const contractQuantity = parseFloat(row.contractQuantity);

	// // 数据验证
	// if (isNaN(newQuantity)) {
	// 	ElMessage.warning('请输入有效的数字');
	// 	row.shipmentQuantity = row.contractQuantity;
	// 	return;
	// }

	// // 检查是否超过合同数量
	// if (newQuantity > contractQuantity) {
	// 	ElMessage.warning('发货数量不能超过合同数量');
	// 	row.shipmentQuantity = row.contractQuantity;
	// 	return;
	// }

	// // 更新对应的采购合同出运数量
	// const purchaseDetail = shippingDeliveryPurchaseDetailsTableData.value.find(
	// 	item => item.productNumber === row.productCode
	// );

	// if (purchaseDetail) {
	// 	purchaseDetail.shipmentQuantity = row.shipmentQuantity;
	// } else {
	// 	ElMessage.warning(`产品 ${row.productCode} 没有关联的采购明细`);
	// }
	try {
		// 转换为数字进行比较
		const shipmentQty = Number(row.shipmentQuantity);
		const remainingQty = Number(row.RemainingQuantityToBeShipped);

		// 验证输入是否为有效数字
		if (isNaN(shipmentQty)) {
			ElMessage.warning('请输入有效的数字');
			row.shipmentQuantity = 0;
			return;
		}

		// 验证是否为正数
		if (shipmentQty <= 0) {
			ElMessage.warning('出货数量必须大于0');
			row.shipmentQuantity = 0;
			return;
		}

		// 验证是否超过剩余待出货数量
		if (shipmentQty > remainingQty) {
			ElMessage.warning(`出货数量不能大于剩余待出货数量(${remainingQty})`);
			row.shipmentQuantity = remainingQty;
			return;
		}

		// 更新相关计算
		row.exportTotalPrice = (shipmentQty * Number(row.exportUnitPrice)).toFixed(2);
		row.boxCount = Math.ceil(shipmentQty / Number(row.outerBoxQuantity));
		row.totalVolume = (row.boxCount * Number(row.outerBoxVolume)).toFixed(3);
		row.totalNetWeight = (shipmentQty * Number(row.outerBoxNetWeight)).toFixed(2);
		row.totalGrossWeight = (row.boxCount * Number(row.outerBoxGrossWeight)).toFixed(2);

		// 更新对应的采购合同出运数量
		const purchaseDetail = shippingDeliveryPurchaseDetailsTableData.value.find(
			item => item.productNumber === row.productCode
		);

		if (purchaseDetail) {
			purchaseDetail.shipmentQuantity = row.shipmentQuantity;
		} else {
			ElMessage.warning(`产品 ${row.productCode} 没有关联的采购明细`);
		}

	} catch (error) {
		console.error('出货数量变更处理错误：', error);
		ElMessage.error('出货数量计算出错，请重试');
	}
};

//编辑
const EditClick = () => {
	isReviewBtnShow.value = false;
	IsEditable.value = false;
	isEditBtnShow.value = false;
	isSaveBtnShow.value = false;
	isEditSaveBtnShow.value = true;
}

// 提交审核
const SubmitReview = () => {
	ElMessageBox.confirm(
		'确定要提交此出运发货单进行审核吗?',
		'提示',
		{
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning',
		}
	).then(() => {
		// 调用提交审核接口
		request({
			url: 'ShippingDeliveries/SubmitForReview/SubmitShippingDeliveriesReview',
			method: 'GET',
			params: {
				ShippingDeliveriesID: IsEditShippingDeliveryID.value
			}
		}).then(response => {
			if (response.code === 200) {
				ElMessage({
					type: 'success',
					message: response.msg
				});

				// 关闭弹窗
				CreateshippingdeliveryDialog.value = false;

				// 刷新列表数据
				GetShippingDeliveriesList(
					ShippingDeliveriesTableDataCurrentPage.value,
					ShippingDeliveriesTableDataPageSize.value
				);
			} else {
				ElMessage.error(response.msg || '提交审核失败');
			}
		}).catch(error => {
			console.error('提交审核失败:', error);
			ElMessage.error('提交审核失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消提交审核'
		});
	});
};
</script>
