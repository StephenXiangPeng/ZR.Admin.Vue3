<template>
	<div>
		<!-- 出运发货单表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click="OpenCreateshippingdeliveryDialog()"
								size="default">创建出运单</el-button>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 过滤条件区域 -->
			<div class="customer-search-area">
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-select filterable v-model="SearchSaleContractID" placeholder="选择销售合同（可输入查询）"
							style="width: 100%" size="default" clearable>
							<el-option v-for="dict in optionss.sql_sale_contracts" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select filterable v-model="SearchCustomerID" placeholder="选择客户（可输入查询）" style="width: 100%"
							size="default" clearable>
							<el-option v-for="dict in optionss.customer_data" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchShippingDateStart" type="date" placeholder="请选择出运日期起"
							style="width: 100%" size="default" />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="SearchShippingDateEnd" type="date" placeholder="请选择出运日期止"
							style="width: 100%" size="default" />
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchClick()" size="default">查询</el-button>
							<el-button @click="ResetClick()" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
				<!-- <el-row :gutter="15">
					<el-col :span="6">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchClick()" size="default">查询</el-button>
							<el-button @click="ResetClick()" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row> -->
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" :data="shippingDeliveryTableData"
				style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="id" label="出运发货单ID" width="150px" v-if="false"></el-table-column>
				<el-table-column prop="invoiceNumber" label="出运单号" width="100">
					<template #default="scope">
						<span>{{ scope.row.invoiceNumber }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="salesContractNumber" label="销售合同号" width="110"></el-table-column>
				<el-table-column prop="createTime" label="制单日期" width="110"></el-table-column>
				<el-table-column prop="shippingStatus" label="出运状态" width="90"></el-table-column>
				<el-table-column prop="reviewStatus" label="审核状态编号" width="150" v-if="false"></el-table-column>
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
				<el-table-column prop="shippingDate" label="出运日期" width="110"></el-table-column>
				<el-table-column prop="invoiceDate" label="发票日期" width="110"></el-table-column>
				<el-table-column prop="customerNumber" label="客户编号" width="150" v-if="false"></el-table-column>
				<el-table-column prop="customerAbbreviation" label="客户简称" width="150"></el-table-column>
				<el-table-column prop="customerContractNumber" label="客户合同号" width="150"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="110"></el-table-column>
				<el-table-column prop="bankOfReceipt" label="收汇银行" width="90"></el-table-column>
				<el-table-column prop="exportCurrency" label="外销币种" width="90"></el-table-column>
				<el-table-column prop="exchangeRate" label="汇率" width="70"></el-table-column>
				<el-table-column prop="priceTerms" label="价格条款" width="90"></el-table-column>
				<el-table-column prop="departurePort" label="出运口岸" width="90"></el-table-column>
				<el-table-column prop="destinationPort" label="目的口岸" width="150px"></el-table-column>
				<el-table-column prop="tradeCountry" label="贸易国别" width="90"></el-table-column>
				<el-table-column prop="settlementMethod" label="结汇方式" width="90"></el-table-column>
				<el-table-column prop="transportationMethod" label="运输方式" width="90"></el-table-column>
				<el-table-column prop="receivableDate" label="应收汇日" width="150"></el-table-column>
				<el-table-column fixed="right" label="操作" width="280px">
					<template #default="scope">
						<el-button type="text" size="small" @click="CheckShipingDelivery(scope.row)">查看/编辑</el-button>
						<el-button type="primary" link size="small" @click="handleDownloadInvoicePdf(scope.row)">下载发票PDF</el-button>
						<el-button type="warning" size="small" icon="Back" link
							v-if="scope.row.reviewStatusStr === '审核中' && (scope.row.createBy != null && scope.row.createBy.toString() === useUserStore().userId.toString())"
							@click="withdrawalApproval(scope.row)">撤回审批</el-button>
						<el-button v-if="useUserStore().roles.includes('admin')" link type="danger" size="small"
							@click="DeleteShipingDelivery(scope.row)">删除</el-button>
						<el-button
							v-if="!useUserStore().roles.includes('admin') && (scope.row.createBy != null && scope.row.createBy.toString() === useUserStore().userId.toString())"
							type="text" size="small" @click="applyDeleteDocument(scope.row)">申请删除单据</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="paymentrequesttableDataHandlePageChange"
				@size-change="ShippingDeliveriesTableDataHandleSizeChange"
				:current-page="ShippingDeliveriesTableDataCurrentPage" :page-size="ShippingDeliveriesTableDataPageSize"
				:total="ShippingDeliveriesTableDataTotalItems" :page-sizes="[10, 20, 30, 50]" background
				layout="total, sizes, prev, pager, next, jumper" style="margin-top: 10px; text-align: right;" />
		</div>
		<el-dialog :modal="false" modal-penetrable v-model="CreateshippingdeliveryDialog" title="创建出运发货单"
			:close-on-click-modal=false style="width: 75%;" @close="CreateshippingdeliveryDialogClose()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="AddShippingDeliveryform" label-width="120px">
				<el-row>
					<el-col :span="6">
						<el-form-item label="发票号码">
							<el-input v-model="AddShippingDeliveryform.invoiceNumber" style="width: 300px" disabled
								size="default"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="制单日期">
							<el-date-picker v-model="AddShippingDeliveryform.OrderMakingDate" type="date"
								style="width: 300px" :disabled="IsEditable" size="default"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="出运状态">
							<el-select filterable v-model="AddShippingDeliveryform.shippingStatus" style="width: 300px"
								:disabled="IsEditable" size="default" clearable>
								<el-option v-for="dict in optionss.hr_shipping_status" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="出运日期">
							<el-date-picker v-model="AddShippingDeliveryform.shippingDate" type="date"
								style="width: 300px" :disabled="IsEditable" size="default"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6">
						<el-form-item label="发票日期">
							<el-date-picker v-model="AddShippingDeliveryform.invoiceDate" type="date"
								style="width: 300px" :disabled="IsEditable" size="default"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="客户编号">
							<el-select filterable v-model="AddShippingDeliveryform.customerNumber"
								placeholder="选择客户（可输入查询）" style="width: 300px" @change="customerNumberChange()"
								clearable :disabled="IsEditable" size="default">
								<el-option v-for="dict in optionss.customer_data" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="客户简称">
							<el-input v-model="AddShippingDeliveryform.customerAbbreviation" disabled
								style="width: 300px" size="default"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="参考合同">
							<el-select filterable v-model="AddShippingDeliveryform.referenceContractNumber"
								placeholder="选择销售合同（可输入查询）" style="width: 300px" clearable
								@change="referenceContractNumberChange()" :disabled="IsEditable" size="default">
								<el-option v-for="dict in optionss.customer_contract_data" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6">
						<el-form-item label="销售合同">
							<el-input v-model="AddShippingDeliveryform.salesContractNumber" disabled
								style="width: 300px" size="default"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="客户合同">
							<el-input v-model="AddShippingDeliveryform.customerContractNumber" disabled
								style="width: 300px" size="default"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="我方公司">
							<el-select filterable v-model="AddShippingDeliveryform.ourCompany" style="width: 300px"
								clearable size="default" :disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="收汇银行">
							<el-select filterable v-model="AddShippingDeliveryform.bankOfReceipt" style="width: 300px"
								:disabled="IsEditable" size="default" clearable>
								<el-option v-for="dict in optionss.hr_bank" :key="dict.dictCode" :label="dict.dictLabel"
									:value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6">
						<el-form-item label="外销币种">
							<el-select filterable v-model="AddShippingDeliveryform.exportCurrency" placeholder="选择外销币种"
								style="width: 300px" size="default" clearable @change="exportCurrencyChange"
								:disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="汇率">
							<el-input v-model="AddShippingDeliveryform.exchangeRate" style="width: 300px" size="default"
								:disabled="IsEditable"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="价格条款">
							<el-select filterable v-model="AddShippingDeliveryform.priceTerms" placeholder="选择价格条款"
								style="width: 300px" size="default" clearable :disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="出运口岸">
							<el-select filterable v-model="AddShippingDeliveryform.departurePort" placeholder="选择出运口岸"
								style="width: 300px" size="default" clearable :disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6">
						<el-form-item label="目的口岸">
							<el-input v-model="AddShippingDeliveryform.destinationPort" style="width: 300px"
								size="default" :disabled="IsEditable"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="贸易国别">
							<el-select filterable v-model="AddShippingDeliveryform.tradeCountry" placeholder="选择贸易国别"
								disabled style="width: 300px" size="default" clearable>
								<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="结汇方式">
							<el-select filterable v-model="AddShippingDeliveryform.settlementMethod"
								placeholder="选择结汇方式" style="width: 300px" size="default" clearable
								:disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="运输方式">
							<el-select filterable v-model="AddShippingDeliveryform.transportationMethod"
								placeholder="选择运输方式" style="width: 300px" size="default" clearable
								:disabled="IsEditable">
								<el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6">
						<el-form-item label="应收汇日">
							<el-date-picker v-model="AddShippingDeliveryform.receivableDate" type="date"
								style="width: 300px" :disabled="IsEditable" size="default"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="单证员" v-if="false">
							<el-select filterable v-model="AddShippingDeliveryform.documentClerk" placeholder="选择单证员"
								style="width: 300px" size="default" disabled clearable>
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
						<el-form-item label="有无预付款">
							<el-checkbox v-model="AddShippingDeliveryform.isDeposit" :disabled="IsEditable"
								size="default"></el-checkbox>
						</el-form-item>
					</el-col>
					<el-col :span="6" v-if="false">
						<el-form-item label="前程运输">
							<el-select filterable v-model="AddShippingDeliveryform.preCarriageTransport"
								style="width: 300px" :disabled="IsEditable" clearable size="default">
								<el-option v-for="dict in optionss.hr_domestic_transport" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="6" v-if="false">
						<el-form-item label="船代公司">
							<el-select filterable v-model="AddShippingDeliveryform.shippingAgent" style="width: 300px"
								:disabled="IsEditable" clearable size="default">
								<el-option v-for="dict in optionss.hr_freight_forwarding_company" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<br><span style="font-size: 20px; font-weight: bold;">辅助信息</span>
			<el-divider></el-divider>
			<el-form label-width="120px">
				<el-row>
					<el-col :span="6">
						<el-form-item label="快递公司">
							<el-select filterable v-model="AddShippingDeliveryform.courierCompaniesID"
								style="width: 300px" :disabled="IsEditable" clearable size="default">
								<el-option v-for="dict in optionss.hr_courier_companies" :key="dict.dictValue"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="物流公司">
							<el-select filterable v-model="AddShippingDeliveryform.logisticsCompanyID"
								style="width: 300px" :disabled="IsEditable" clearable size="default">
								<el-option v-for="dict in optionss.hr_logistics_companies" :key="dict.dictValue"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="船代公司">
							<el-select filterable v-model="AddShippingDeliveryform.shippingAgent" style="width: 300px"
								:disabled="IsEditable" clearable size="default">
								<el-option v-for="dict in optionss.hr_freight_forwarders" :key="dict.dictValue"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<br><span style="font-size: 20px; font-weight: bold;">销售合同</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryContrctProductTableData"
				style="width: 100%;margin-bottom: 15px; table-layout: fixed;"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="id" label="ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractId" label="销售合同" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractProductId" label="销售合同明细ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractNumber" label="销售合同" width="150" sortable></el-table-column>
				<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="customerCode" label="客户货号" width="180"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="chineseSpec" label="中文规格" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
				<el-table-column prop="RemainingQuantityToBeShipped" label="剩余待出货数量" width="150"
					vif="false"></el-table-column>
				<el-table-column prop="shipmentQuantity" label="出货数量" width="150">
					<template #default="scope">
						<el-input v-model="scope.row.shipmentQuantity" :disabled="IsEditable" style="width: 100%"
							@change="shipmentQuantityChange(scope.row)" size="small"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
				<el-table-column prop="exportUnitPrice" label="外销单价" width="150"></el-table-column>
				<el-table-column prop="exportTotalPrice" label="外销总价" width="150"></el-table-column>
				<el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
				<el-table-column prop="outerBoxQuantity" label="外箱装量" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxQuantity" type="number"
							size="small" @change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxQuantity }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="innerBoxQuantity" label="中包装量" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.innerBoxQuantity" type="number"
							size="small" />
						<span v-else>{{ scope.row.innerBoxQuantity }}</span>
					</template>
				</el-table-column>
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
				<el-table-column prop="outerBoxLength" label="外箱长度" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxLength" type="number"
							size="small" @change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxLength }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="outerBoxWidth" label="外箱宽度" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxWidth" type="number" size="small"
							@change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxWidth }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="outerBoxHeight" label="外箱高度" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxHeight" type="number"
							size="small" @change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxHeight }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="outerBoxVolume" label="外箱体积" width="150"></el-table-column>
				<el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
				<el-table-column prop="outerBoxNetWeight" label="外箱净重" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxNetWeight" type="number"
							size="small" @change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxNetWeight }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="outerBoxGrossWeight" label="外箱毛重" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable" v-model.number="scope.row.outerBoxGrossWeight" type="number"
							size="small" @change="recalcSalesRowPackaging(scope.row)" />
						<span v-else>{{ scope.row.outerBoxGrossWeight }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="totalNetWeight" label="总净重" width="150"></el-table-column>
				<el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
				<el-table-column prop="singlesalesrevenue" label="单个销售收入" width="150" vif="false"></el-table-column>
				<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
					<template v-slot:default="scope">
						<el-button link type="primary" size="small"
							@click="DeleteShippingDeliveryContrctProduct(scope.$index)"
							:disabled="IsEditable">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<br><span style="font-size: 20px; font-weight: bold;">客户其他费用</span>
			<el-divider></el-divider>
			<div style="margin-bottom: 10px;">
				<el-button v-if="!IsEditable" type="primary" size="default"
					@click="AddCustomerExpenseRow">添加费用</el-button>
			</div>
			<el-table :data="shippingDeliveryCustomerExpensesTableData"
				style="width: 100%;margin-bottom: 15px; table-layout: fixed;"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="expenseName" label="费用名称" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable && !scope.row.isFromApi" v-model="scope.row.expenseName"
							placeholder="费用名称" size="default" />
						<span v-else>{{ scope.row.expenseName || '-' }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="currency" label="币种" width="150">
					<template #default="scope">
						<el-select v-if="!IsEditable && !scope.row.isFromApi" v-model="scope.row.currency"
							placeholder="币种" size="default" clearable style="width: 100%;"
							@change="customerExpenseCurrencyChange(scope.row)">
							<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
						<span v-else>{{ getCurrencyLabel(scope.row.currency) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="exchangeRate" label="汇率" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable && !scope.row.isFromApi" v-model.number="scope.row.exchangeRate"
							type="number" placeholder="汇率" size="default"
							@input="recalcCustomerExpenseAmount(scope.row)" />
						<span v-else>{{ scope.row.exchangeRate }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="expense" label="费用" width="150">
					<template #default="scope">
						<el-input v-if="!IsEditable && !scope.row.isFromApi" v-model.number="scope.row.expense"
							type="number" placeholder="费用" size="default"
							@input="recalcCustomerExpenseAmount(scope.row)" />
						<span v-else>{{ scope.row.expense }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="amount" label="金额" width="150">
					<template #default="scope">
						<span>{{ getCustomerExpenseAmount(scope.row) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="备注" width="200">
					<template #default="scope">
						<el-input v-if="!IsEditable && !scope.row.isFromApi" v-model="scope.row.remark" placeholder="备注"
							size="default" />
						<span v-else>{{ scope.row.remark || '-' }}</span>
					</template>
				</el-table-column>
				<el-table-column v-if="!IsEditable" fixed="right" label="操作" width="80">
					<template #default="scope">
						<el-button v-if="!scope.row.isFromApi" link type="primary" size="default"
							@click="DeleteCustomerExpenseRow(scope.$index)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<br><span style="font-size: 20px; font-weight: bold;">采购合同</span>
			<el-divider></el-divider>
			<el-table :data="getPurchaseTableData()" style="width: 100%;margin-bottom: 15px; table-layout: fixed;"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="purchaseContractId" label="采购合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="salesContract" label="销售合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="id" label="采购合同明细ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="purchaseContractNumber" label="采购合同" width="150" sortable></el-table-column>
				<el-table-column prop="vendorAbbreviation" label="厂商简称" width="150"></el-table-column>
				<el-table-column prop="productNumber" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
				<el-table-column prop="shipmentQuantity" label="出货数量" width="150">
					<template #default="scope">
						<el-input v-model="scope.row.shipmentQuantity" disabled style="width: 100%"
							size="small"></el-input>
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
			<br><span style="font-size: 20px; font-weight: bold;">采购其它费用</span>
			<el-divider></el-divider>
			<el-table :data="shippingDeliveryPurchaseExpensesTableData"
				style="width: 100%;margin-bottom: 15px; table-layout: fixed;"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="expenseName" label="费用名称" width="150"></el-table-column>
				<el-table-column prop="currency" label="币种" width="150">
					<template #default="scope">
						<span>{{ getCurrencyLabel(scope.row.currency) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
				<el-table-column prop="expense" label="费用" width="150"></el-table-column>
				<el-table-column prop="amount" label="金额" width="150"></el-table-column>
				<el-table-column prop="remark" label="备注" width="200"></el-table-column>
			</el-table>
			<br><span style="font-size: 20px; font-weight: bold;">备注信息</span>
			<el-divider></el-divider>
			<el-form-item label="备注：" style="width: 100%;">
				<el-input v-model="AddShippingDeliveryform.remark" :autosize="{ minRows: 5, maxRows: 10 }"
					type="textarea" placeholder="输入备注内容" :disabled="IsEditable" size="default" />
			</el-form-item>
			<template #footer>
				<el-text class="mx-1" size="large" type="success">出运单总金额：{{ AddShippingDeliveryform.shipmentTotalAmount
				}}</el-text>&nbsp;&nbsp;&nbsp;&nbsp;
				<span class="dialog-footer">
					<el-button v-show="isSaveBtnShow && userId.toString() === CreateByUser" type="warning"
						@click="SaveClick(true)" size="default">
						保存草稿
					</el-button>
					<el-button v-show="isSaveBtnShow && userId.toString() === CreateByUser" type="success"
						@click="SaveClick(false)" size="default">
						提交
					</el-button>
					<el-button type="primary" v-show="isEditBtnShow && userId.toString() === CreateByUser"
						@click="EditClick()" size="default">
						编辑
					</el-button>
					<el-button type="warning" v-show="isEditSaveBtnShow && userId.toString() === CreateByUser"
						@click="EditSaveClick(true)" size="default">
						保存草稿
					</el-button>
					<el-button type="success" v-show="isEditSaveBtnShow && userId.toString() === CreateByUser"
						@click="EditSaveClick(false)" size="default">
						提交
					</el-button>
					<el-button type="danger" v-show="showApproveRejectBtn" @click="ApproveReject" size="default">
						驳回
					</el-button>
					<el-button type="success" v-show="showApprovePassBtn" @click="Approvepass" size="default">
						通过
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref, nextTick } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, ElLoading, UploadUserFile, UploadFile } from 'element-plus'
import type { Action } from 'element-plus'
import request from '@/utils/request';
import { blobValidate } from '@/utils/ruoyi';
import { saveAs } from 'file-saver';
import { get } from 'sortablejs';
import Supperinfomation from '../purchase/supperinfomation.vue';
import dayjs from 'dayjs';
import useUserStore from "@/store/modules/user";
import { useRoute } from 'vue-router'
import exchangeRateService from '@/utils/exchangeRateService'

const route = useRoute()
// 添加onMounted钩子
onMounted(() => {
	console.log('出运发货单页面挂载，检查路由参数')
	autoLoadShippingDeliveryRequestDetail()
})

// 添加自动加载出运发货单详情的函数
const autoLoadShippingDeliveryRequestDetail = async () => {
	// 检查URL参数
	const contractId = route.query.contractId
	const viewDetail = route.query.viewDetail
	if (contractId && viewDetail === 'true') {
		console.log('自动加载出运发货单详情, ID:', contractId)

		try {
			// 等待获取出运发货单列表
			await GetShippingDeliveriesList(1, 100)

			// 查找匹配的出运发货单
			const shippingdelivery = shippingDeliveryTableData.value.find(item =>
				item.id.toString() === contractId.toString()
			)

			if (shippingdelivery) {
				// 调用查看详情的函数
				await CheckShipingDelivery(shippingdelivery)
				// 确保对话框显示
				CreateshippingdeliveryDialog.value = true
			} else {
				console.error('未找到匹配的出运发货单:', contractId)
				ElMessage.error('未找到匹配的出运发货单')
			}
		} catch (error) {
			console.error('加载出运发货单详情失败:', error)
			ElMessage.error('加载出运发货单详情失败')
		}
	}
}

//获取当前登录用户ID
var userId = useUserStore().userId;
//是否可编辑
const IsEditable = ref(false) //是否可编辑
const isReviewBtnShow = ref(false) //提交审核按钮是否显示
const isEditBtnShow = ref(false) //编辑按钮是否显示
const isSaveBtnShow = ref(true) //保存按钮是否显示
const isEditSaveBtnShow = ref(false) //编辑保存按钮是否显示
const showApproveRejectBtn = ref(false) //驳回按钮是否显示
const showApprovePassBtn = ref(false) //通过按钮是否显示

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
	courierCompaniesID: '',
	logisticsCompanyID: '',
	shippingAgent: '',
	remark: '',
	shipmentTotalAmount: 0
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
		hr_freight_forwarders: [],
		hr_courier_companies: [],
		hr_logistics_companies: [],
		hr_calculate_unit: [],
		hr_outerbox_unit: [],
		hr_yes_no: [],  // 是否开票等是/否字典
		sql_supplier_info: [],
		sql_product_name: [],
		sql_product: [],
		customer_data: [],  // 用户特定的客户数据
		customer_contract_data: [],  // 用户特定的客户合同数据
		customer_unfinished_contracts: []  // 客户未完结合同列表
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_sale_contracts' }, { dictType: 'sql_hr_customer' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_pricing_term' }, { dictType: 'hr_transport_port' }, { dictType: 'hr_nation' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_transportation_method' }, { dictType: 'sql_all_user' }, { dictType: 'hr_shipping_status' },
{ dictType: 'hr_ourcompany' }, { dictType: 'hr_bank' }, { dictType: 'hr_domestic_transport' },
{ dictType: 'hr_freight_forwarding_company' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_yes_no' }, { dictType: 'sql_supplier_info' }, { dictType: 'sql_product_name' }, { dictType: 'sql_product' }]
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

// 加载货代/快递/物流公司下拉
const loadLogisticsCompanySelects = async () => {
	try {
		// companyType: 1=货代公司, 2=快递公司, 3=物流公司
		const [freightRes, courierRes, logisticsRes] = await Promise.all([
			request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 1 } }),
			request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 2 } }),
			request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 3 } })
		])

		state.optionss.hr_freight_forwarders = (freightRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
		state.optionss.hr_courier_companies = (courierRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
		state.optionss.hr_logistics_companies = (logisticsRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
	} catch (e) {
		console.error('加载物流公司下拉失败:', e)
	}
}
loadLogisticsCompanySelects();

//获取特定的客户数据
const getCustomerData = async () => {
	state.optionss.customer_data = [];
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerDataByUserID/GetSelectCustomerDataByUserID',
			method: 'get'
		})
		if (response.code === 200) {
			// 更新客户数据选项
			state.optionss.customer_data = response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel,
				// ... 其他需要的字段映射
			}))
		} else {
			ElMessage.error(response.msg || '获取客户数据失败')
		}
	} catch (error) {
		console.error('获取客户数据失败:', error)
		ElMessage.error('获取客户数据失败')
	}
}
getCustomerData();

//获取特定的客户合同数据
const getContractData = async (customerId = 0) => {
	state.optionss.customer_contract_data = [];
	try {
		const response = await request({
			url: 'Contracts/GetContractDataByUserID/GetSelectContractData',
			method: 'get',
			params: {
				CustomerID: customerId
			}
		})
		if (response.code === 200) {
			state.optionss.customer_contract_data = response.data;
			// 选择客户时：根据该客户下合同ID集合获取客户相关费用
			const contractIds = (response.data || []).map(c => Number(c.dictValue ?? c.contractId ?? c.id)).filter(Boolean);
			if (contractIds.length > 0) {
				await loadCustomerExpensesData(contractIds);
			}
		} else {
			ElMessage.error(response.msg || '获取销售合同数据失败')
		}
	} catch (error) {
		console.error('获取销售合同数据失败:', error)
		ElMessage.error('获取销售合同数据失败')
	}
}
getContractData();

// 获取客户未完结合同列表
const getUnfinishedContractsList = async (customerId = 0) => {
	state.optionss.customer_unfinished_contracts = [];
	if (!customerId) {
		shippingDeliveryContrctProductTableData.value = [];
		shippingDeliveryPurchaseDetailsTableData.value = [];
		return;
	}
	try {
		const response = await request({
			url: 'Contracts/GetUnfinishedContractsList/GetList',
			method: 'get',
			params: {
				CustomerID: customerId
			}
		});
		if (response.code === 200) {
			const unfinishedData = response.data || {};
			const salesContracts = unfinishedData.salesContracts || [];
			const purchaseContracts = unfinishedData.purchaseContracts || [];

			state.optionss.customer_unfinished_contracts = salesContracts;
			shippingDeliveryContrctProductTableData.value = salesContracts.map(item => ({
				id: item.id,
				contractId: item.contractId ?? item.contractID ?? item.salesContractId ?? item.salesContractID ?? item.id,
				contractProductId: item.contractProductId ?? item.productId ?? item.id,
				contractNumber: item.contractNumber,
				productCode: item.productCode,
				customerCode: item.customerCode,
				chineseName: item.chineseName,
				chineseSpec: item.chineseSpec,
				contractQuantity: item.contractQuantity,
				RemainingQuantityToBeShipped: item.contractQuantity,
				shipmentQuantity: item.contractQuantity,
				unit: item.unit ? state.optionss.hr_calculate_unit.find(unitItem => unitItem.dictValue === item.unit.toString())?.dictLabel || '无' : '无',
				exportUnitPrice: item.exportUnitPrice,
				exportTotalPrice: item.exportTotalPrice,
				specialRequirements: item.specialRequirements,
				outerBoxQuantity: item.outerBoxQuantity,
				innerBoxQuantity: item.innerBoxQuantity,
				boxCount: item.boxCount,
				outerBoxUnit: item.outerboxunit ? state.optionss.hr_outerbox_unit.find(unitItem => unitItem.dictValue === item.outerboxunit.toString())?.dictLabel || '无' : '无',
				outerBoxLength: item.outerBoxLength,
				outerBoxWidth: item.outerBoxWidth,
				outerBoxHeight: item.outerBoxHeight,
				outerBoxVolume: item.outerBoxVolume,
				totalVolume: item.totalVolume,
				outerBoxNetWeight: item.outerBoxNetWeight,
				outerBoxGrossWeight: item.outerBoxGrossWeight,
				totalNetWeight: item.totalNetWeight,
				totalGrossWeight: item.totalGrossWeight,
				singlesalesrevenue: item.singlesalesrevenue
			}));
			// 无参考合同：批量根据销售合同ID+产品ID获取已出货数量（新接口）
			const contractIds = [...new Set(salesContracts.map(item => Number(item.contractId ?? item.contractID ?? item.salesContractId ?? item.salesContractID ?? item.id)).filter(Boolean))];
			const productIds = [...new Set(salesContracts.map(item => Number(item.contractProductId ?? item.productId ?? item.id)).filter(Boolean))];
			if (contractIds.length > 0 && productIds.length > 0) {
				try {
					const queryString = [...contractIds.map(c => 'ContractIDs=' + c), ...productIds.map(p => 'ProductIDs=' + p)].join('&');
					const shippingResponse = await request({
						url: 'ShippingDeliveries/GetShippingQuantity/GetShippingQuantity?' + queryString,
						method: 'GET'
					});
					const list = shippingResponse.data || [];
					shippingDeliveryContrctProductTableData.value.forEach((row, index) => {
						const contractId = row.contractId ?? row.id;
						const productId = row.contractProductId ?? row.id;
						const item = list.find(x => Number(x.contractId ?? x.ContractId) === Number(contractId) && Number(x.productId ?? x.ProductId) === Number(productId));
						const shippedQuantity = item ? Number(item.shippingQuantity ?? item.ShippingQuantity ?? 0) : 0;
						const contractQuantity = Number(row.contractQuantity || 0);
						const remainingQuantity = contractQuantity - shippedQuantity;
						row.RemainingQuantityToBeShipped = remainingQuantity;
						row.shipmentQuantity = remainingQuantity;
					});
					shippingDeliveryContrctProductTableData.value.forEach(row => recalcSalesRowPackaging(row));
				} catch (error) {
					console.error('获取出货数量失败:', error);
				}
			}
			const salesList = shippingDeliveryContrctProductTableData.value;
			shippingDeliveryPurchaseDetailsTableData.value = purchaseContracts.map(item => {
				const salesContract = item.salesContract;
				const salesRow = salesContract != null ? salesList.find(
					row => String(row.contractId ?? row.id) === String(salesContract) &&
						(row.productCode === item.productCode || row.productCode === item.productNumber)
				) : null;
				const shipmentQty = salesRow != null ? salesRow.shipmentQuantity : item.contractQuantity;
				return {
					id: item.id,
					purchaseContractId: item.purchaseContractId,
					salesContract: item.salesContract,
					purchaseContractProductID: item.id,
					purchaseContractNumber: item.purchaseContractNumber,
					vendorAbbreviation: item.supplierShortName,
					productCode: item.productCode,
					productNumber: item.productCode,
					chineseName: item.chineseName,
					contractQuantity: item.contractQuantity,
					shipmentQuantity: shipmentQty,
					purchaseCurrency: item.purchaseCurrency != null ? (state.optionss.hr_export_currency.find(c => c.dictValue === item.purchaseCurrency.toString())?.dictLabel || '无') : '无',
					purchaseUnitPrice: item.purchasePrice,
					purchaseTotalPrice: item.purchaseTotalPrice,
					measurementUnit: item.unit ? state.optionss.hr_calculate_unit.find(unitItem => unitItem.dictValue === item.unit.toString())?.dictLabel || '无' : '无',
					invoice: item.invoice != null ? (state.optionss.hr_yes_no.find(d => d.dictValue === item.invoice.toString())?.dictLabel ?? (Number(item.invoice) === 1 ? '是' : '否')) : '无',
					totalVolume: item.totalVolume,
					totalGrossWeight: item.totalGrossWeight
				};
			});
		} else {
			shippingDeliveryContrctProductTableData.value = [];
			shippingDeliveryPurchaseDetailsTableData.value = [];
			ElMessage.error(response.msg || '获取未完结合同列表失败');
		}
	} catch (error) {
		console.error('获取未完结合同列表失败:', error);
		shippingDeliveryContrctProductTableData.value = [];
		shippingDeliveryPurchaseDetailsTableData.value = [];
		ElMessage.error('获取未完结合同列表失败');
	}
};
/*动态下拉框end*/

//出运发货表格数据
const shippingDeliveryTableData = ref([])
//产品资料表格数据
const shippingDeliveryContrctProductTableData = ref([])
//采购明细表格数据
const shippingDeliveryPurchaseDetailsTableData = ref([])
// 客户其他费用
const shippingDeliveryCustomerExpensesTableData = ref([]);
// 采购其它费用
const shippingDeliveryPurchaseExpensesTableData = ref([]);

// 获取采购表格数据的函数
const getPurchaseTableData = () => {
	return shippingDeliveryPurchaseDetailsTableData.value;
};

// 获取币种标签
const getCurrencyLabel = (currencyValue) => {
	if (!currencyValue) return '无';
	const currency = state.optionss.hr_export_currency.find(c => c.dictValue === currencyValue.toString());
	return currency ? currency.dictLabel : '无';
};

// 外销币种变化，自动带出汇率
const exportCurrencyChange = async (value) => {
	if (!value) {
		AddShippingDeliveryform.value.exchangeRate = '';
		return;
	}
	if (Number(value) === 3) {
		AddShippingDeliveryform.value.exchangeRate = '1';
		return;
	}
	try {
		const latestRate = await exchangeRateService.getLatestExchangeRate(value);
		if (latestRate !== null) {
			AddShippingDeliveryform.value.exchangeRate = String(latestRate);
		} else {
			const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
			AddShippingDeliveryform.value.exchangeRate = String(defaultRate);
			ElMessage.warning(`未找到${exchangeRateService.getCurrencyName(value, state.optionss.hr_export_currency)}的最新汇率，已使用默认汇率`);
		}
	} catch (error) {
		console.error('获取汇率失败:', error);
		const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
		AddShippingDeliveryform.value.exchangeRate = String(defaultRate);
		ElMessage.warning('获取汇率失败，已使用默认汇率');
	}
};

// 加载客户其他费用数据（新接口：根据合同ID集合获取）
const loadCustomerExpensesData = async (contractIds) => {
	const ids = Array.isArray(contractIds) ? contractIds : (contractIds != null && contractIds !== '' ? [Number(contractIds)] : []);
	shippingDeliveryCustomerExpensesTableData.value = [];
	if (ids.length === 0) return;
	try {
		const queryString = ids.map(id => 'ContractIDS=' + id).join('&');
		const response = await request({
			url: 'Contracts/GetContractExpensesListByIDs/GetContractExpensesList?' + queryString,
			method: 'GET'
		});
		const list = response.data || [];
		if (list.length > 0) {
			shippingDeliveryCustomerExpensesTableData.value = list.map(item => {
				const rawCurrency = item.currency ?? item.Currency;
				// 币种绑定为与字典 hr_export_currency 的 dictValue 一致（字符串），以便下拉正确选中并显示
				const currencyValue = rawCurrency != null && rawCurrency !== '' ? String(rawCurrency) : '';
				return {
					isFromApi: true,
					expenseName: item.expenseName ?? item.ExpenseName ?? '',
					currency: currencyValue,
					exchangeRate: Number(item.exchangeRate ?? item.ExchangeRate ?? 0),
					expense: Number(item.expense ?? item.Expense ?? 0),
					amount: Number((item.expense ?? item.Expense ?? 0) * (item.exchangeRate ?? item.ExchangeRate ?? 0)),
					remark: item.remark ?? item.Remark ?? ''
				};
			});
		}
	} catch (error) {
		console.error('获取客户其他费用数据失败:', error);
	}
};

// 根据币种从系统获取汇率并写入行（用于客户其他费用）
const fetchExchangeRateForCustomerExpenseRow = async (row) => {
	const value = row.currency;
	if (!value) {
		row.exchangeRate = 0;
		recalcCustomerExpenseAmount(row);
		return;
	}
	if (Number(value) === 3) {
		row.exchangeRate = 1;
		recalcCustomerExpenseAmount(row);
		return;
	}
	try {
		const latestRate = await exchangeRateService.getLatestExchangeRate(value);
		if (latestRate !== null) {
			row.exchangeRate = Number(latestRate);
		} else {
			const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
			row.exchangeRate = Number(defaultRate);
			ElMessage.warning(`未找到${exchangeRateService.getCurrencyName(value, state.optionss.hr_export_currency)}的最新汇率，已使用默认汇率`);
		}
		recalcCustomerExpenseAmount(row);
	} catch (error) {
		console.error('获取汇率失败:', error);
		const defaultRate = exchangeRateService.getDefaultExchangeRate(value);
		row.exchangeRate = Number(defaultRate);
		ElMessage.warning('获取汇率失败，已使用默认汇率');
		recalcCustomerExpenseAmount(row);
	}
};

// 客户费用行币种变更时，根据币种获取系统汇率
const customerExpenseCurrencyChange = (row) => {
	fetchExchangeRateForCustomerExpenseRow(row);
};

// 添加客户其他费用行（根据默认币种从系统获取汇率）
const AddCustomerExpenseRow = async () => {
	const form = AddShippingDeliveryform.value;
	const newRow = {
		isFromApi: false,
		expenseName: '',
		currency: form.exportCurrency || '',
		exchangeRate: form.exchangeRate ? Number(form.exchangeRate) : 0,
		expense: 0,
		amount: 0,
		remark: ''
	};
	shippingDeliveryCustomerExpensesTableData.value.push(newRow);
	// 若有币种则从系统获取最新汇率
	if (newRow.currency) {
		await fetchExchangeRateForCustomerExpenseRow(newRow);
	}
};

// 删除客户其他费用行
const DeleteCustomerExpenseRow = (index) => {
	shippingDeliveryCustomerExpensesTableData.value.splice(index, 1);
};

// 客户费用金额（费用 * 汇率）
const getCustomerExpenseAmount = (row) => {
	const expense = Number(row.expense) || 0;
	const rate = Number(row.exchangeRate) || 0;
	return (expense * rate).toFixed(2);
};

// 输入时更新客户费用行的金额
const recalcCustomerExpenseAmount = (row) => {
	row.amount = (Number(row.expense) || 0) * (Number(row.exchangeRate) || 0);
};

// 加载采购其它费用数据
const loadPurchaseExpensesData = async (purchaseContractIds) => {
	try {
		// 清空现有数据
		shippingDeliveryPurchaseExpensesTableData.value = [];

		// 如果没有采购合同ID，直接返回
		if (!purchaseContractIds || purchaseContractIds.length === 0) {
			return;
		}

		// 获取所有采购合同的其它费用（去重合并）
		const allExpenses = [];
		const processedExpenses = new Set(); // 用于去重

		for (const purchaseContractId of purchaseContractIds) {
			try {
				const response = await request({
					url: 'PurchaseContracts/GetPurchaseContractDetailsById/GetPurchaseContractDetails',
					method: 'GET',
					params: {
						PurchaseContracID: purchaseContractId
					}
				});

				if (response.data && response.data.purchaseContractVendorExpenses && response.data.purchaseContractVendorExpenses.length > 0) {
					response.data.purchaseContractVendorExpenses.forEach(item => {
						// 使用费用名称+币种+费用金额作为唯一标识去重
						const uniqueKey = `${item.expenseName}_${item.currency}_${item.expense}`;
						if (!processedExpenses.has(uniqueKey)) {
							processedExpenses.add(uniqueKey);
							allExpenses.push({
								expenseName: item.expenseName || '',
								currency: item.currency || '',
								exchangeRate: item.exchangeRate || 0,
								expense: item.expense || 0,
								amount: (item.expense || 0) * (item.exchangeRate || 0),
								remark: item.remark || ''
							});
						}
					});
				}
			} catch (error) {
				console.error(`获取采购合同 ${purchaseContractId} 的其它费用失败:`, error);
				// 继续处理其他采购合同
			}
		}

		// 设置到表格中
		shippingDeliveryPurchaseExpensesTableData.value = allExpenses;
	} catch (error) {
		console.error('获取采购其它费用数据失败:', error);
		// 不显示错误消息，因为可能没有采购其它费用数据
	}
};

//客户编号改变
const customerNumberChange = () => {
	if (AddShippingDeliveryform.value.customerNumber != null && AddShippingDeliveryform.value.customerNumber != undefined && AddShippingDeliveryform.value.customerNumber != '') {
		getContractData(Number(AddShippingDeliveryform.value.customerNumber));
		getUnfinishedContractsList(Number(AddShippingDeliveryform.value.customerNumber));
	} else {
		getContractData();
		getUnfinishedContractsList();
		shippingDeliveryContrctProductTableData.value = [];
		shippingDeliveryPurchaseDetailsTableData.value = [];
	}
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
			AddShippingDeliveryform.value.tradeCountry = response.tradingCountry ? response.tradingCountry.toString() : '';
			AddShippingDeliveryform.value.settlementMethod = response.settlementWay ? response.settlementWay.toString() : '';
		}
	}).catch(error => {
		console.log(error)
	});

	// 清空相关数据，因为客户改变时需要重新选择参考合同
	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
	shippingDeliveryCustomerExpensesTableData.value = [];
	shippingDeliveryPurchaseExpensesTableData.value = [];
	AddShippingDeliveryform.value.referenceContractNumber = '';
	AddShippingDeliveryform.value.salesContractNumber = '';
	AddShippingDeliveryform.value.customerContractNumber = '';
}

//参考合同号改变
const referenceContractNumberChange = async () => {
	var SaleContractID = AddShippingDeliveryform.value.referenceContractNumber;
	if (SaleContractID == '' || SaleContractID == null || SaleContractID == undefined) {
		shippingDeliveryContrctProductTableData.value = [];
		shippingDeliveryPurchaseDetailsTableData.value = [];
		shippingDeliveryCustomerExpensesTableData.value = [];
		shippingDeliveryPurchaseExpensesTableData.value = [];
		return;
	}

	// 先获取采购合同信息，检查是否有采购合同数据

	request({
		url: 'PurchaseContracts/GetCustomerPurchasesByContractID/GetAllPurchasess',
		method: 'GET',
		params: {
			ContractID: SaleContractID
		}
	}).then(async (purchaseResponse) => {
		// 检查API响应状态
		if (purchaseResponse.code !== 200) {
			ElMessage.error('获取采购合同信息失败：' + purchaseResponse.msg);
			AddShippingDeliveryform.value.referenceContractNumber = '';
			shippingDeliveryContrctProductTableData.value = [];
			shippingDeliveryPurchaseDetailsTableData.value = [];
			return;
		}

		// 检查是否有采购合同数据
		if (!purchaseResponse.data || purchaseResponse.data.length === 0) {
			ElMessage.error('该销售合同没有关联的采购合同，无法创建出运单！');
			// 清空参考合同选择
			AddShippingDeliveryform.value.referenceContractNumber = '';
			// 清空相关数据
			shippingDeliveryContrctProductTableData.value = [];
			shippingDeliveryPurchaseDetailsTableData.value = [];
			shippingDeliveryCustomerExpensesTableData.value = [];
			shippingDeliveryPurchaseExpensesTableData.value = [];
			return;
		}

		// 有采购合同数据，继续获取销售合同信息
		request({
			url: 'Contracts/GetContractDetailsById/GetContractDetails',
			method: 'GET',
			params: {
				contractId: SaleContractID
			}
		}).then(response => {
			if (response.data != null) {
				AddShippingDeliveryform.value.customerNumber = response.data.contract.customerId ? state.optionss.customer_data.find(item => item.dictValue === response.data.contract.customerId.toString())?.dictValue || '' : '';

				// 手动获取客户简称，而不调用 customerNumberChange()
				request({
					url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
					method: 'GET',
					params: {
						ID: response.data.contract.customerId
					}
				}).then(customerResponse => {
					if (customerResponse != null) {
						AddShippingDeliveryform.value.customerAbbreviation = customerResponse.customerAbbreviation
						AddShippingDeliveryform.value.tradeCountry = customerResponse.tradingCountry ? customerResponse.tradingCountry.toString() : '';
						AddShippingDeliveryform.value.settlementMethod = customerResponse.settlementWay ? customerResponse.settlementWay.toString() : '';
					}
				}).catch(error => {
					console.error('获取客户简称失败:', error)
				});

				AddShippingDeliveryform.value.salesContractNumber = response.data.contract.contractNumber;
				AddShippingDeliveryform.value.customerContractNumber = response.data.contract.customerContract;
				AddShippingDeliveryform.value.ourCompany = response.data.contract.ourCompany ? response.data.contract.ourCompany.toString() : '';
				AddShippingDeliveryform.value.exportCurrency = response.data.contract.foreignCurrency ? response.data.contract.foreignCurrency.toString() : '';
				AddShippingDeliveryform.value.exchangeRate = response.data.contract.exchangeRate;
				AddShippingDeliveryform.value.priceTerms = response.data.contract.priceTerms ? response.data.contract.priceTerms.toString() : '';
				AddShippingDeliveryform.value.departurePort = response.data.contract.shippingPort ? response.data.contract.shippingPort.toString() : '';
				AddShippingDeliveryform.value.destinationPort = response.data.contract.destinationPort ? response.data.contract.destinationPort.toString() : '';
				AddShippingDeliveryform.value.tradeCountry = response.data.contract.tradeCountry ? response.data.contract.tradeCountry.toString() : '';
				AddShippingDeliveryform.value.settlementMethod = response.data.contract.settlementMethod ? response.data.contract.settlementMethod.toString() : '';
				AddShippingDeliveryform.value.transportationMethod = response.data.contract.transportation ? response.data.contract.transportation.toString() : '';
				shippingDeliveryContrctProductTableData.value = [];
				// 有参考合同：批量根据销售合同ID+产品ID获取已出货数量（新接口），再填充销售行并同步采购
				const contractProducts = response.data.contractProducts || [];
				const productIds = contractProducts.map(e => Number(e.id)).filter(Boolean);
				if (productIds.length > 0) {
					const queryString = ['ContractIDs=' + SaleContractID, ...productIds.map(p => 'ProductIDs=' + p)].join('&');
					request({
						url: 'ShippingDeliveries/GetShippingQuantity/GetShippingQuantity?' + queryString,
						method: 'GET'
					}).then((ShippingQuantityResponse) => {
						const list = ShippingQuantityResponse.data || [];
						const salesRows = contractProducts.map((element) => {
							const item = list.find(x => Number(x.contractId ?? x.ContractId) === Number(SaleContractID) && Number(x.productId ?? x.ProductId) === Number(element.id));
							const ShippingQuantity = item ? Number(item.shippingQuantity ?? item.ShippingQuantity ?? 0) : 0;
							const qty = element.contractQuantity - ShippingQuantity;
							return {
								contractId: element.contractId,
								contractProductId: element.id,
								contractNumber: response.data.contract.contractNumber,
								productCode: element.productCode,
								chineseName: element.chineseName,
								contractQuantity: element.contractQuantity,
								RemainingQuantityToBeShipped: qty,
								shipmentQuantity: qty,
								unit: element.unit ? state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无' : '无',
								exportUnitPrice: element.exportUnitPrice,
								exportTotalPrice: element.exportTotalPrice,
								specialRequirements: element.specialRequirements,
								outerBoxQuantity: element.outerBoxQuantity,
								innerBoxQuantity: element.innerBoxQuantity ?? element.innerBoxLoading ?? '',
								boxCount: element.boxCount,
								outerBoxUnit: element.outerboxunit ? state.optionss.hr_outerbox_unit.find(item => item.dictValue === element.outerboxunit.toString())?.dictLabel || '无' : '无',
								outerBoxLength: element.outerBoxLength,
								outerBoxWidth: element.outerBoxWidth,
								outerBoxHeight: element.outerBoxHeight,
								outerBoxVolume: element.outerBoxVolume,
								totalVolume: element.totalVolume,
								outerBoxNetWeight: element.outerBoxNetWeight,
								outerBoxGrossWeight: element.outerBoxGrossWeight,
								totalNetWeight: element.totalNetWeight,
								totalGrossWeight: element.totalGrossWeight,
								singlesalesrevenue: element.singlesalesrevenue
							};
						});
						shippingDeliveryContrctProductTableData.value = salesRows;
						shippingDeliveryContrctProductTableData.value.forEach(row => recalcSalesRowPackaging(row));
						// 获取时同步：采购出货数量与销售表一致（按销售合同ID+产品编号匹配）
						shippingDeliveryPurchaseDetailsTableData.value.forEach(purchaseRow => {
							const salesRow = shippingDeliveryContrctProductTableData.value.find(
								row => (String(row.contractId ?? row.id) === String(purchaseRow.salesContractID ?? purchaseRow.salesContractId)) &&
									(row.productCode === purchaseRow.productCode || row.productCode === purchaseRow.productNumber)
							);
							if (salesRow != null) purchaseRow.shipmentQuantity = salesRow.shipmentQuantity;
						});
					}).catch((error) => {
						console.error('获取出货数量失败:', error);
					});
				}

			}
		}).catch(error => {
			console.error(error);
		});

		// 填充采购合同数据
		// 创建新的数组来存储处理后的数据
		const newPurchaseData = [];

		purchaseResponse.data.forEach((element, index) => {
			const purchaseItem = {
				id: element.purchaseContractProductID ?? element.id,
				salesContractID: SaleContractID,
				salesContractId: SaleContractID,
				purchaseContractID: element.purchaseContractID,
				purchaseContractId: element.purchaseContractID,
				purchaseContractProductID: element.purchaseContractProductID,
				purchaseContractNumber: element.purchaseContractNumber,
				purchaseShippingNumber: element.purchaseContractNumber,
				shipmentQuantity: element.contractQuantity,
				vendorAbbreviation: element.supplierID ? state.optionss.sql_supplier_info.find(item => item.dictValue === element.supplierID.toString())?.dictLabel || '无' : '无',
				productCode: element.productNumber != null ? element.productNumber.toString() : '',
				productNumber: element.productNumber ? state.optionss.sql_product.find(item => item.dictValue === element.productNumber.toString())?.dictLabel : '无',
				chineseName: element.chineseName,
				purchaseCurrency: element.purchaseCurrency ? state.optionss.hr_export_currency.find(item => item.dictValue === element.purchaseCurrency.toString())?.dictLabel || '无' : '无',
				purchaseUnitPrice: element.purchasePrice,
				purchaseTotalPrice: element.purchaseTotalPrice,
				measurementUnit: element.unit ? state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无' : '无',
				invoice: element.invoice != null ? (state.optionss.hr_yes_no.find(d => d.dictValue === element.invoice.toString())?.dictLabel ?? (Number(element.invoice) === 1 ? '是' : '否')) : '无',
				totalVolume: element.totalVolume,
				totalGrossWeight: element.totalGrossWeight,
				contractQuantity: element.contractQuantity
			};

			newPurchaseData.push(purchaseItem);
		});

		// 使用 nextTick 确保 DOM 更新
		await nextTick();

		// 一次性更新响应式数据
		shippingDeliveryPurchaseDetailsTableData.value = newPurchaseData;

		// 强制触发响应式更新
		await nextTick();
		shippingDeliveryPurchaseDetailsTableData.value = [...newPurchaseData];

		// 加载客户其他费用数据
		await loadCustomerExpensesData(SaleContractID);

		// 加载采购其它费用数据
		const purchaseContractIds = Array.from(new Set(purchaseResponse.data.map(item => item.purchaseContractID)));
		await loadPurchaseExpensesData(purchaseContractIds);
	}).catch(error => {
		console.error('获取采购合同信息失败:', error);
		ElMessage.error('获取采购合同信息失败，请稍后重试');
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

	// 设置创建人为当前用户
	CreateByUser = userId.toString();

	// 设置按钮显示状态
	isSaveBtnShow.value = true;
	isEditBtnShow.value = false;
	isEditSaveBtnShow.value = false;
	isReviewBtnShow.value = false;

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
		documentClerk: userId.toString(),  // 默认当前用户
		isDeposit: 0,
		preCarriageTransport: '',
		courierCompaniesID: '',
		logisticsCompanyID: '',
		shippingAgent: '',
		remark: '',
		shipmentTotalAmount: 0
	};

	// 清空产品数据
	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
	shippingDeliveryCustomerExpensesTableData.value = [];
	shippingDeliveryPurchaseExpensesTableData.value = [];
	OriginalShipmentQuantity.value = [];

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
	IsDraft: 0, // 是否草稿：1是，0否
	shipmentTotalAmount: 0,
	ShippingDeliveryProductItems: [],
	ShippingDeliveryPurchaseDetailsItems: [],
	ShippingDeliveriesExpensesItems: []
});

//保存出运发货单
const SaveClick = async (isDraft) => {
	// 1. 数据验证 - 只在非草稿模式下验证
	if (!isDraft && !validateForm()) {
		return;
	}
	try {
		await ElMessageBox.confirm(`确定${isDraft ? '保存草稿' : '提交'}出运发货单吗?`, '提示', {
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
			CourierCompaniesID: AddShippingDeliveryform.value.courierCompaniesID ?
				Number(AddShippingDeliveryform.value.courierCompaniesID) : null,
			LogisticsCompanyID: AddShippingDeliveryform.value.logisticsCompanyID ?
				Number(AddShippingDeliveryform.value.logisticsCompanyID) : null,
			ShippingAgent: AddShippingDeliveryform.value.shippingAgent ?
				Number(AddShippingDeliveryform.value.shippingAgent) : null,
			Remark: AddShippingDeliveryform.value.remark,
			IsDelete: 0,
			IsDraft: isDraft ? 1 : 0, // 是否草稿：1是，0否
			shipmentTotalAmount: calculateShipmentTotalAmount(),

			// 产品明细（含箱规数据）
			ShippingDeliveryProductItems: shippingDeliveryContrctProductTableData.value.map(item => ({
				ContractId: item.contractId,
				ContractProductId: item.contractProductId,
				contractQuantity: item.contractQuantity,
				ShipmentQuantity: Number(item.shipmentQuantity),
				RemainingQuantity: Number(item.contractQuantity) - Number(item.shipmentQuantity),
				IsDelete: 0,
				Remark: item.remark || '',
				Singlesalesrevenue: Number(item.singlesalesrevenue || 0),
				// 箱规数据
				OuterBoxQuantity: Number(item.outerBoxQuantity) || 0,
				InnerBoxQuantity: Number(item.innerBoxQuantity) || 0,
				BoxCount: Number(item.boxCount) || 0,
				OuterBoxLength: Number(item.outerBoxLength) || 0,
				OuterBoxWidth: Number(item.outerBoxWidth) || 0,
				OuterBoxHeight: Number(item.outerBoxHeight) || 0,
				OuterBoxVolume: Number(item.outerBoxVolume) || 0,
				outerboxunit: item.outerBoxUnit || '',
				OuterBoxNetWeight: Number(item.outerBoxNetWeight) || 0,
				OuterBoxGrossWeight: Number(item.outerBoxGrossWeight) || 0,
				TotalVolume: Number(item.totalVolume) || 0,
				TotalNetWeight: Number(item.totalNetWeight) || 0,
				TotalGrossWeight: Number(item.totalGrossWeight) || 0
			})),

			// 采购明细（兼容 purchaseContractID/purchaseContractId、id/purchaseContractProductID 两种字段名）
			ShippingDeliveryPurchaseDetailsItems: shippingDeliveryPurchaseDetailsTableData.value.map(item => ({
				PurchaseContractID: Number(item.purchaseContractID ?? item.purchaseContractId ?? 0),
				PurchaseContractProductID: Number(item.id ?? item.purchaseContractProductID ?? 0),
				ContractQuantity: Number(item.contractQuantity || 0),
				ShipmentQuantity: Number(item.shipmentQuantity || 0),
				RemainingQuantity: Number(item.contractQuantity || 0) - Number(item.shipmentQuantity || 0),
				IsDelete: 0,
				Remark: item.remark || ''
			})),

			// 客户相关费用（其它费用）
			ShippingDeliveriesExpensesItems: shippingDeliveryCustomerExpensesTableData.value.map(item => ({
				ExpenseName: item.expenseName || '',
				Currency: item.currency ? Number(item.currency) : null,
				ExchangeRate: Number(item.exchangeRate) || 0,
				Expense: Number(item.expense) || 0,
				Amount: (Number(item.expense) || 0) * (Number(item.exchangeRate) || 0),
				Remark: item.remark || ''
			}))
		};
		// 3. 发送保存请求
		const response = await request.post('ShippingDeliveries/AddShippingDeliveries/Add', requestData);
		// 4. 处理响应
		if (response.code === 200) {
			ElMessage({
				message: response.msg || (isDraft ? '出运发货单保存草稿成功！' : '出运发货单提交成功！'),
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
			throw new Error(response.msg || '操作失败');
		}
	} catch (error) {
		if (error === 'cancel') {
			ElMessage({
				type: 'info',
				message: '已取消操作'
			});
		} else {
			console.error('出运发货单操作出错：', error);
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

	// 验证是否有采购合同数据
	if (!shippingDeliveryPurchaseDetailsTableData.value.length) {
		ElMessage.warning('没有关联的采购合同数据，无法创建出运单');
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
		documentClerk: userId.toString(),  // 默认当前用户
		isDeposit: 0,
		preCarriageTransport: '',
		shippingAgent: '',
		remark: '',
		shipmentTotalAmount: 0
	};

	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
	shippingDeliveryCustomerExpensesTableData.value = [];
	shippingDeliveryPurchaseExpensesTableData.value = [];
};

const IsEditShippingDeliveryID = ref(0)
//编辑保存出运发货单
const EditSaveClick = (isDraft) => {
	// 只在非草稿模式下验证
	if (!isDraft && !validateForm()) {
		ElMessage.warning('表单验证未通过，请检查填写的信息');
		return;
	}

	ElMessageBox.confirm(`确定${isDraft ? '保存' : '提交'}编辑后的出运发货单吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 如果必填字段为空，使用默认值
		if (!AddShippingDeliveryform.value.remark) {
			AddShippingDeliveryform.value.remark = '无备注';
		}
		if (!AddShippingDeliveryform.value.invoiceNumber) {
			AddShippingDeliveryform.value.invoiceNumber = '系统生成';
		}
		if (!AddShippingDeliveryform.value.salesContractNumber) {
			AddShippingDeliveryform.value.salesContractNumber = '无合同号';
		}
		if (!AddShippingDeliveryform.value.customerAbbreviation) {
			AddShippingDeliveryform.value.customerAbbreviation = '无简称';
		}
		if (!AddShippingDeliveryform.value.customerContractNumber) {
			AddShippingDeliveryform.value.customerContractNumber = '无合同号';
		}
		// 创建与后端模型匹配的请求体
		const shippingDeliveriesRequest = {
			Id: IsEditShippingDeliveryID.value,
			InvoiceNumber: AddShippingDeliveryform.value.invoiceNumber || '系统生成',
			OrderMakingDate: AddShippingDeliveryform.value.OrderMakingDate || new Date(),
			ShippingStatus: Number(AddShippingDeliveryform.value.shippingStatus),
			ShippingDate: AddShippingDeliveryform.value.shippingDate,
			InvoiceDate: AddShippingDeliveryform.value.invoiceDate,
			CustomerNumber: AddShippingDeliveryform.value.customerNumber ? Number(AddShippingDeliveryform.value.customerNumber) : 0,
			CustomerAbbreviation: AddShippingDeliveryform.value.customerAbbreviation || '无简称',
			ReferenceContractNumber: AddShippingDeliveryform.value.referenceContractNumber ?
				Number(AddShippingDeliveryform.value.referenceContractNumber) : null,
			SalesContractNumber: AddShippingDeliveryform.value.salesContractNumber || '无合同号',
			CustomerContractNumber: AddShippingDeliveryform.value.customerContractNumber || '无合同号',
			OurCompany: AddShippingDeliveryform.value.ourCompany ? Number(AddShippingDeliveryform.value.ourCompany) : 0,
			BankOfReceipt: AddShippingDeliveryform.value.bankOfReceipt ?
				Number(AddShippingDeliveryform.value.bankOfReceipt) : null,
			ExportCurrency: AddShippingDeliveryform.value.exportCurrency ?
				Number(AddShippingDeliveryform.value.exportCurrency) : null,
			ExchangeRate: AddShippingDeliveryform.value.exchangeRate ?
				Number(AddShippingDeliveryform.value.exchangeRate) : 0,
			PriceTerms: AddShippingDeliveryform.value.priceTerms ?
				Number(AddShippingDeliveryform.value.priceTerms) : null,
			DeparturePort: AddShippingDeliveryform.value.departurePort ?
				Number(AddShippingDeliveryform.value.departurePort) : null,
			DestinationPort: AddShippingDeliveryform.value.destinationPort || '',
			TradeCountry: AddShippingDeliveryform.value.tradeCountry ?
				Number(AddShippingDeliveryform.value.tradeCountry) : null,
			SettlementMethod: AddShippingDeliveryform.value.settlementMethod ?
				Number(AddShippingDeliveryform.value.settlementMethod) : null,
			TransportationMethod: AddShippingDeliveryform.value.transportationMethod ?
				Number(AddShippingDeliveryform.value.transportationMethod) : null,
			ReceivableDate: AddShippingDeliveryform.value.receivableDate,
			DocumentClerk: AddShippingDeliveryform.value.documentClerk ?
				Number(AddShippingDeliveryform.value.documentClerk) : null,
			IsDeposit: AddShippingDeliveryform.value.isDeposit ? true : false,
			PreCarriageTransport: AddShippingDeliveryform.value.preCarriageTransport ?
				Number(AddShippingDeliveryform.value.preCarriageTransport) : null,
			CourierCompaniesID: AddShippingDeliveryform.value.courierCompaniesID ?
				Number(AddShippingDeliveryform.value.courierCompaniesID) : null,
			LogisticsCompanyID: AddShippingDeliveryform.value.logisticsCompanyID ?
				Number(AddShippingDeliveryform.value.logisticsCompanyID) : null,
			ShippingAgent: AddShippingDeliveryform.value.shippingAgent ?
				Number(AddShippingDeliveryform.value.shippingAgent) : null,
			Remark: AddShippingDeliveryform.value.remark || '无备注',
			IsDelete: 0,
			IsDraft: isDraft ? 1 : 0,
			ShipmentTotalAmount: Number(calculateShipmentTotalAmount()),
			// 产品明细 - 确保至少有一个空项目
			ShippingDeliveryProductItems: shippingDeliveryContrctProductTableData.value.length > 0 ?
				shippingDeliveryContrctProductTableData.value.map(item => ({
					id: item.id || 0,
					ContractId: Number(item.contractId || 0),
					ContractProductId: Number(item.contractProductId || 0),
					contractQuantity: Number(item.contractQuantity || 0),
					ShipmentQuantity: Number(item.shipmentQuantity || 0),
					RemainingQuantity: Number(item.contractQuantity || 0) - Number(item.shipmentQuantity || 0),
					IsDelete: 0,
					Remark: item.remark || '无备注',
					Singlesalesrevenue: Number(item.singlesalesrevenue || 0),
					// 箱规数据
					OuterBoxQuantity: Number(item.outerBoxQuantity) || 0,
					InnerBoxQuantity: Number(item.innerBoxQuantity) || 0,
					BoxCount: Number(item.boxCount) || 0,
					OuterBoxLength: Number(item.outerBoxLength) || 0,
					OuterBoxWidth: Number(item.outerBoxWidth) || 0,
					OuterBoxHeight: Number(item.outerBoxHeight) || 0,
					OuterBoxVolume: Number(item.outerBoxVolume) || 0,
					outerboxunit: item.outerBoxUnit || '',
					OuterBoxNetWeight: Number(item.outerBoxNetWeight) || 0,
					OuterBoxGrossWeight: Number(item.outerBoxGrossWeight) || 0,
					TotalVolume: Number(item.totalVolume) || 0,
					TotalNetWeight: Number(item.totalNetWeight) || 0,
					TotalGrossWeight: Number(item.totalGrossWeight) || 0
				})) : [{
					id: 0,
					ContractId: 0,
					ContractProductId: 0,
					contractQuantity: 0,
					ShipmentQuantity: 0,
					RemainingQuantity: 0,
					IsDelete: 0,
					Remark: '无备注'
				}],
			// 采购明细 - 确保至少有一个空项目
			ShippingDeliveryPurchaseDetailsItems: shippingDeliveryPurchaseDetailsTableData.value.length > 0 ?
				shippingDeliveryPurchaseDetailsTableData.value.map(item => ({
					id: item.id || 0,
					PurchaseContractID: Number(item.purchaseContractID || 0),
					PurchaseContractProductID: Number(item.id || 0),
					ContractQuantity: Number(item.contractQuantity || 0),
					ShipmentQuantity: Number(item.shipmentQuantity || 0),
					RemainingQuantity: Number(item.contractQuantity || 0) - Number(item.shipmentQuantity || 0),
					IsDelete: 0,
					Remark: item.remark || '无备注'
				})) : [{
					id: 0,
					PurchaseContractID: 0,
					PurchaseContractProductID: 0,
					ContractQuantity: 0,
					ShipmentQuantity: 0,
					RemainingQuantity: 0,
					IsDelete: 0,
					Remark: '无备注'
				}],
			// 客户相关费用（其它费用）
			ShippingDeliveriesExpensesItems: shippingDeliveryCustomerExpensesTableData.value.map(item => ({
				ExpenseName: item.expenseName || '',
				Currency: item.currency ? Number(item.currency) : null,
				ExchangeRate: Number(item.exchangeRate) || 0,
				Expense: Number(item.expense) || 0,
				Amount: (Number(item.expense) || 0) * (Number(item.exchangeRate) || 0),
				Remark: item.remark || ''
			}))
		};
		// 直接发送请求，不再嵌套在shippingDeliveriesRequest中
		request.post('ShippingDeliveries/EditShippingDeliveries/Edit', shippingDeliveriesRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: isDraft ? '出运发货单编辑保存成功！' : '出运发货单编辑提交成功！',
					type: 'success'
				});
				IsEditable.value = true;
				isEditBtnShow.value = true;
				isReviewBtnShow.value = true;
				isEditSaveBtnShow.value = false;
				// 刷新列表数据
				GetShippingDeliveriesList(
					ShippingDeliveriesTableDataCurrentPage.value,
					ShippingDeliveriesTableDataPageSize.value
				);
				// 关闭弹窗
				CreateshippingdeliveryDialog.value = false;
			} else {
				console.error('出运发货单编辑出错');
			}
		}).catch(error => {
			console.error('出运发货单编辑出错！😔错误内容：', error);
			if (error.response && error.response.data) {
				ElMessage.error(JSON.stringify(error.response.data));
			} else {
				ElMessage.error('提交失败，请检查数据格式');
			}
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消操作'
		});
	});
}

///出运发货单表格数据
const ShippingDeliveriesTableDataTotalItems = ref(0);
const ShippingDeliveriesTableDataCurrentPage = ref(1);
const ShippingDeliveriesTableDataPageSize = ref(30);
const paymentrequesttableDataHandlePageChange = async (newPage) => {
	ShippingDeliveriesTableDataCurrentPage.value = newPage;
	await GetShippingDeliveriesList(newPage, ShippingDeliveriesTableDataPageSize.value);
};
const ShippingDeliveriesTableDataHandleSizeChange = async (size) => {
	ShippingDeliveriesTableDataPageSize.value = size;
	ShippingDeliveriesTableDataCurrentPage.value = 1;
	await GetShippingDeliveriesList(1, size);
};
async function GetShippingDeliveriesList(start, end) {
	try {
		const response = await request({
			url: 'ShippingDeliveries/GetShippingDeliveriesList/GetList',
			method: 'GET',
			params: {
				CustomerID: SearchCustomerID.value,
				SaleContractID: SearchSaleContractID.value,
				ShippingDateStart: SearchShippingDateStart.value,
				ShippingDateEnd: SearchShippingDateEnd.value
			}
		});
		if (response.data != null) {
			shippingDeliveryTableData.value = response.data.result;
			shippingDeliveryTableData.value.forEach(item => {
				item.shippingStatus = item.shippingStatus ? state.optionss.hr_shipping_status.find(s => s.dictValue === item.shippingStatus.toString())?.dictLabel || '无' : '无';
				item.customerNumber = item.customerNumber ? state.optionss.sql_hr_customer.find(c => c.dictValue === item.customerNumber.toString())?.dictLabel || '无' : '无';
				item.ourCompany = item.ourCompany ? state.optionss.hr_ourcompany.find(c => c.dictValue === item.ourCompany.toString())?.dictLabel || '无' : '无';
				item.bankOfReceipt = !item.bankOfReceipt ? '无' : state.optionss.hr_bank.find(c => c.dictValue === item.bankOfReceipt.toString())?.dictLabel || '无';
				item.exportCurrency = item.exportCurrency ? state.optionss.hr_export_currency.find(c => c.dictValue === item.exportCurrency.toString())?.dictLabel || '无' : '无';
				item.departurePort = item.departurePort ? state.optionss.hr_transport_port.find(c => c.dictValue === item.departurePort.toString())?.dictLabel || '无' : '无';
				item.destinationPort = item.destinationPort ? item.destinationPort.toString() : '无';
				item.priceTerms = item.priceTerms ? state.optionss.hr_pricing_term.find(c => c.dictValue === item.priceTerms.toString())?.dictLabel || '无' : '无';
				item.settlementMethod = item.settlementMethod ? state.optionss.hr_settlement_way.find(c => c.dictValue === item.settlementMethod.toString())?.dictLabel || '无' : '无';
				item.transportationMethod = item.transportationMethod ? state.optionss.hr_transportation_method.find(c => c.dictValue === item.transportationMethod.toString())?.dictLabel || '无' : '无';
				item.tradeCountry = item.tradeCountry ? state.optionss.hr_nation.find(c => c.dictValue === item.tradeCountry.toString())?.dictLabel || '无' : '无';
				item.shippingDate = item.shippingDate ? dayjs(item.shippingDate).format('YYYY-MM-DD') : '';
				item.invoiceDate = item.invoiceDate ? dayjs(item.invoiceDate).format('YYYY-MM-DD') : '';
				item.OrderMakingDate = item.OrderMakingDate ? dayjs(item.OrderMakingDate).format('YYYY-MM-DD') : '';
				item.receivableDate = item.receivableDate ? dayjs(item.receivableDate).format('YYYY-MM-DD') : '';
				item.createTime = item.createTime ? dayjs(item.createTime).format('YYYY-MM-DD') : '';
				switch (item.reviewStatus) {
					case 0:
						item.reviewStatusStr = '待提审';
						break;
					case 1:
						item.reviewStatusStr = '审核中';
						break;
					case 2:
						item.reviewStatusStr = '已批准';
						break;
					case 3:
						item.reviewStatusStr = '已拒绝';
						break;
				}
			});
		}
		return shippingDeliveryTableData.value;
	} catch (error) {
		console.error(error);
		return [];
	}
}

// 撤回审批（制单人出运合同撤回审批）
const withdrawalApproval = async (row: any) => {
	try {
		await ElMessageBox.confirm(
			'确定要撤回该出运合同的审批吗？',
			'提示',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}
		)

		const res = await request({
			url: 'ShippingDeliveries/WithdrawalApprovalShippingDeliverContract/WithdrawalApproval',
			method: 'get',
			params: {
				documentID: row.id
			}
		})

		if (res.code === 200) {
			ElMessage.success(res.msg)
			GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value)
		} else {
			ElMessage.error(res.msg)
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('撤回审批失败:', error)
		}
	}
}

// 	获取出运发货单编辑锁状态
const GetShippingDeliveriesContractEditLock = async (contractId) => {
	try {
		const res = await request({
			url: 'ShippingDeliveries/GetShippingDeliveriesContractEditLock/GetShippingDeliveriesContractEditLock',
			method: 'get',
			params: { ShippingDeliveriesContractID: contractId }
		});
		return res; // 返回锁定用户名，如果未锁定则为null
	} catch (error) {
		console.error('获取出运发货单编辑锁失败:', error);
		return null;
	}
};

// 设置出运发货单编辑锁
const SettingsShippingDeliveriesContractEditLock = async (contractId) => {
	try {
		const res = await request({
			url: 'ShippingDeliveries/SettingsShippingDeliveriesContractEditLock/SettingsShippingDeliveriesContractEditLock',
			method: 'get',
			params: { ShippingDeliveriesContractID: contractId }
		});
		return res.code === 200;
	} catch (error) {
		console.error('设置出运发货单编辑锁失败:', error);
		return false;
	}
};

// 移除出运发货单编辑锁
const RemoveShippingDeliveriesContractEditLock = async (contractId) => {
	try {
		await request({
			url: 'ShippingDeliveries/RemoveShippingDeliveriesContractEditLock/RemoveShippingDeliveriesContractEditLock',
			method: 'get',
			params: { ShippingDeliveriesContractID: contractId }
		});
	} catch (error) {
		console.error('移除出运发货单编辑锁失败:', error);
	}
};

// 单据创建人
var CreateByUser;
//出运发货单选中数据ID
var ShippingDeliveriesID = ref(0);

const handleDownloadInvoicePdf = async (row) => {
	if (!row?.id) {
		ElMessage.error('出运发货单 ID 无效');
		return;
	}
	const loading = ElLoading.service({ text: '正在下载发票，请稍候', background: 'rgba(0, 0, 0, 0.7)' });
	try {
		const resp = await request({
			url: 'CommercialInvoice/DownloadPdf/pdf',
			method: 'post',
			data: row.id,
			responseType: 'blob'
		});
		const isBlob = await blobValidate(resp.data);
		if (isBlob) {
			const contentDisposition = decodeURI(resp.headers['content-disposition'] || '');
			const match = /filename=([^;]+\.[^.;]+);*/.exec(contentDisposition);
			const fileName = match ? match[1].replace(/"/g, '') : `invoice-${row.invoiceNumber || row.id}.pdf`;
			saveAs(new Blob([resp.data], { type: 'application/pdf' }), fileName);
		} else {
			const resText = await resp.data.text();
			const rspObj = JSON.parse(resText);
			ElMessage.error(rspObj.msg || rspObj.title || '下载发票失败');
		}
	} catch (err) {
		const errData = err?.response?.data;
		if (errData instanceof Blob) {
			const resText = await errData.text();
			try {
				const rspObj = JSON.parse(resText);
				ElMessage.error(rspObj.msg || rspObj.title || '下载发票失败');
			} catch {
				ElMessage.error(resText || '下载发票失败');
			}
		} else {
			ElMessage.error(err?.message || '下载发票失败，请联系管理员');
		}
	} finally {
		loading.close();
	}
};

//检查出运发货单
const CheckShipingDelivery = async (row) => {
	ShippingDeliveriesID.value = row.id;
	CreateByUser = row.createBy;
	// 先检查编辑锁
	const lockStatus = await GetShippingDeliveriesContractEditLock(row.id);
	if (lockStatus && lockStatus.data && lockStatus.data.isEditLock === true) {
		ElMessageBox.alert(`当前出运发货单正在被${lockStatus.data.editUser}编辑中，请稍后再试！`, '提示', {
			confirmButtonText: '确定',
			showClose: false
		});
		return;
	}
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
	}).then(async response => {
		if (response.data && response.data.shippingDeliveries) {
			IsEditShippingDeliveryID.value = response.data.shippingDeliveries.id;
			AddShippingDeliveryform.value.invoiceNumber = response.data.shippingDeliveries.invoiceNumber || '';
			AddShippingDeliveryform.value.OrderMakingDate = response.data.shippingDeliveries.createTime;
			AddShippingDeliveryform.value.shippingStatus = response.data.shippingDeliveries.shippingStatus ? response.data.shippingDeliveries.shippingStatus.toString() : '';
			AddShippingDeliveryform.value.shippingDate = response.data.shippingDeliveries.shippingDate;
			AddShippingDeliveryform.value.invoiceDate = response.data.shippingDeliveries.invoiceDate;
			AddShippingDeliveryform.value.customerNumber = response.data.shippingDeliveries.customerNumber ? response.data.shippingDeliveries.customerNumber.toString() : '';
			AddShippingDeliveryform.value.customerAbbreviation = response.data.shippingDeliveries.customerAbbreviation || '';
			AddShippingDeliveryform.value.referenceContractNumber = state.optionss.customer_contract_data.find(c => c.dictValue === response.data.shippingDeliveries.referenceContractNumber)?.dictValue || '';
			AddShippingDeliveryform.value.salesContractNumber = response.data.shippingDeliveries.salesContractNumber || '';
			AddShippingDeliveryform.value.customerContractNumber = response.data.shippingDeliveries.customerContractNumber || '';
			AddShippingDeliveryform.value.ourCompany = response.data.shippingDeliveries.ourCompany ? response.data.shippingDeliveries.ourCompany.toString() : '';
			AddShippingDeliveryform.value.bankOfReceipt = response.data.shippingDeliveries.bankOfReceipt ? response.data.shippingDeliveries.bankOfReceipt.toString() : '';
			AddShippingDeliveryform.value.exportCurrency = response.data.shippingDeliveries.exportCurrency ? response.data.shippingDeliveries.exportCurrency.toString() : '';
			AddShippingDeliveryform.value.exchangeRate = response.data.shippingDeliveries.exchangeRate;
			AddShippingDeliveryform.value.priceTerms = response.data.shippingDeliveries.priceTerms ? response.data.shippingDeliveries.priceTerms.toString() : '';
			AddShippingDeliveryform.value.departurePort = response.data.shippingDeliveries.departurePort ? response.data.shippingDeliveries.departurePort.toString() : '';
			AddShippingDeliveryform.value.destinationPort = response.data.shippingDeliveries.destinationPort ? response.data.shippingDeliveries.destinationPort.toString() : '';
			AddShippingDeliveryform.value.tradeCountry = response.data.shippingDeliveries.tradeCountry ? response.data.shippingDeliveries.tradeCountry.toString() : '';
			AddShippingDeliveryform.value.settlementMethod = response.data.shippingDeliveries.settlementMethod ? response.data.shippingDeliveries.settlementMethod.toString() : '';
			AddShippingDeliveryform.value.transportationMethod = response.data.shippingDeliveries.transportationMethod ? response.data.shippingDeliveries.transportationMethod.toString() : '';
			AddShippingDeliveryform.value.receivableDate = response.data.shippingDeliveries.receivableDate;
			AddShippingDeliveryform.value.documentClerk = response.data.shippingDeliveries.documentClerk ? response.data.shippingDeliveries.documentClerk.toString() : userId.toString();
			AddShippingDeliveryform.value.isDeposit = response.data.shippingDeliveries.isDeposit;
			AddShippingDeliveryform.value.preCarriageTransport = response.data.shippingDeliveries.preCarriageTransport ? response.data.shippingDeliveries.preCarriageTransport.toString() : '';
			AddShippingDeliveryform.value.shippingAgent = response.data.shippingDeliveries.shippingAgent ? response.data.shippingDeliveries.shippingAgent.toString() : '';
			AddShippingDeliveryform.value.courierCompaniesID = response.data.shippingDeliveries.courierCompaniesID ? response.data.shippingDeliveries.courierCompaniesID.toString() : '';
			AddShippingDeliveryform.value.logisticsCompanyID = response.data.shippingDeliveries.logisticsCompanyID ? response.data.shippingDeliveries.logisticsCompanyID.toString() : '';
			AddShippingDeliveryform.value.remark = response.data.shippingDeliveries.remark || '';
			AddShippingDeliveryform.value.shipmentTotalAmount = response.data.shippingDeliveries.shipmentTotalAmount || 0;

		}
		if (response.data.shippingDeliveryProducts.length > 0) {
			shippingDeliveryContrctProductTableData.value = response.data.shippingDeliveryProducts;
			const products = shippingDeliveryContrctProductTableData.value;
			const contractIds = [...new Set(products.map(x => Number(x.contractId)).filter(Boolean))];
			const productIds = [...new Set(products.map(x => Number(x.contractProductId)).filter(Boolean))];
			let shippingQuantityList = [];
			if (contractIds.length > 0 && productIds.length > 0) {
				const queryString = [...contractIds.map(c => 'ContractIDs=' + c), ...productIds.map(p => 'ProductIDs=' + p)].join('&');
				try {
					const shippingResponse = await request({
						url: 'ShippingDeliveries/GetShippingQuantity/GetShippingQuantity?' + queryString,
						method: 'GET'
					});
					shippingQuantityList = shippingResponse.data || [];
				} catch (e) {
					console.error('获取出货数量失败:', e);
				}
			}
			const finalShippingList = shippingQuantityList;
			shippingDeliveryContrctProductTableData.value.forEach(item => {
				request({
					url: 'Contracts/GetCustomerContractProductByCPID/GetCustomerContractProduct',
					method: 'GET',
					params: {
						CPID: item.contractProductId
					}
				}).then(resp => {
					if (resp.data != null && resp.data.length > 0) {
						const index = shippingDeliveryContrctProductTableData.value.findIndex(
							x => x.contractProductId === item.contractProductId
						);
						if (index !== -1) {
							const entry = finalShippingList.find(x => Number(x.contractId ?? x.ContractId) === Number(item.contractId) && Number(x.productId ?? x.ProductId) === Number(item.contractProductId));
							const ShippingQuantity = entry ? Number(entry.shippingQuantity ?? entry.ShippingQuantity ?? 0) : 0;
							const productData = resp.data[0];
							const existingRow = shippingDeliveryContrctProductTableData.value[index];
							// 已保存草稿或提交后查看详情：箱规从出运单详情(shippingDeliveryProducts)保留，仅用合同产品补全品名/单位等
							shippingDeliveryContrctProductTableData.value[index] = {
								...existingRow,
								id: productData.id,
								contractNumber: productData.contractNumber,
								productCode: productData.productCode,
								chineseName: productData.chineseName,
								contractQuantity: productData.contractQuantity,
								unit: productData.unit ? state.optionss.hr_calculate_unit.find(u => u.dictValue === productData.unit.toString())?.dictLabel || '无' : '无',
								exportUnitPrice: productData.exportUnitPrice,
								exportTotalPrice: productData.exportTotalPrice,
								specialRequirements: productData.specialRequirements,
								singlesalesrevenue: item.singlesalesrevenue,
								RemainingQuantityToBeShipped: productData.contractQuantity - ShippingQuantity,
								outerBoxUnit: productData.outerboxunit ? state.optionss.hr_outerbox_unit.find(u => u.dictValue === productData.outerboxunit.toString())?.dictLabel || '无' : '无',
								// 箱规(outerBoxQuantity/InnerBoxQuantity/boxCount/outerBoxLength 等)保留 existingRow 即出运单详情返回的数据，不覆盖为合同产品
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
								vendorAbbreviation: purchaseData.supplierID ? state.optionss.sql_supplier_info.find(s => s.dictValue === purchaseData.supplierID.toString())?.dictLabel || '无' : '无',
								productNumber: purchaseData.productNumber ? state.optionss.sql_product.find(p => p.dictValue === purchaseData.productNumber.toString())?.dictLabel || '无' : '无',
								chineseName: purchaseData.chineseName,
								purchaseCurrency: purchaseData.purchaseCurrency ? state.optionss.hr_export_currency.find(c => c.dictValue === purchaseData.purchaseCurrency.toString())?.dictLabel || '无' : '无',
								purchaseUnitPrice: purchaseData.purchasePrice,
								purchaseTotalPrice: purchaseData.purchaseTotalPrice,
								measurementUnit: purchaseData.unit ? state.optionss.hr_calculate_unit.find(u => u.dictValue === purchaseData.unit.toString())?.dictLabel || '无' : '无',
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
		// 加载采购其它费用数据
		if (response.data.shippingDeliveryPurchaseDetails && response.data.shippingDeliveryPurchaseDetails.length > 0) {
			const purchaseContractIds = Array.from(new Set(response.data.shippingDeliveryPurchaseDetails.map(item => item.purchaseContractID)));
			await loadPurchaseExpensesData(purchaseContractIds);
		}
		// 加载客户其他费用数据（从出运单详情接口返回的 shippingDeliveriesExpenses）
		if (response.data.shippingDeliveriesExpenses && response.data.shippingDeliveriesExpenses.length > 0) {
			shippingDeliveryCustomerExpensesTableData.value = response.data.shippingDeliveriesExpenses.map(item => {
				const rawCurrency = item.currency;
				// 币种绑定为与字典 hr_export_currency 的 dictValue 一致（字符串），以便下拉正确选中并显示
				const currencyValue = rawCurrency != null && rawCurrency !== '' ? String(rawCurrency) : '';
				return {
					isFromApi: true,
					expenseName: item.expenseName || '',
					currency: currencyValue,
					exchangeRate: Number(item.exchangeRate || 0),
					expense: Number(item.expense || 0),
					amount: Number((item.expense || 0) * (item.exchangeRate || 0)),
					remark: item.remark || ''
				};
			});
		} else {
			shippingDeliveryCustomerExpensesTableData.value = [];
		}
		GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);

		getApprovalFlow(row.id).then(() => {
			const isCurrentUserApprover = checkIfCurrentUserIsApprover();
			// 只有当前用户是审批人且出运发货单在审核中时才显示审核按钮
			if (isCurrentUserApprover && row.reviewStatusStr === '审核中') {
				showApproveRejectBtn.value = true;
				showApprovePassBtn.value = true;
				// 设置文档类型（出运发货单）
				ApproveDocumentRequest.DocumentType = 3;//3出运发货单
			} else {
				// 如果不是审核中状态，隐藏审批按钮
				showApproveRejectBtn.value = false;
				showApprovePassBtn.value = false;
			}
		});
		// 确保弹窗显示
		CreateshippingdeliveryDialog.value = true;
	}).catch(error => {
		console.error('获取出运单详情失败：', error);
		ElMessage.error('获取出运单详情失败，请稍后重试');
	});
};

//关闭弹窗
const CreateshippingdeliveryDialogClose = async () => {
	if (CreateByUser.toString() == userId.toString()) {
		// 移除编辑锁
		await RemoveShippingDeliveriesContractEditLock(IsEditShippingDeliveryID.value);
	}
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
	AddShippingDeliveryform.value.courierCompaniesID = '';
	AddShippingDeliveryform.value.logisticsCompanyID = '';
	AddShippingDeliveryform.value.shippingAgent = '';
	shippingDeliveryContrctProductTableData.value = [];
	shippingDeliveryPurchaseDetailsTableData.value = [];
	shippingDeliveryCustomerExpensesTableData.value = [];
	shippingDeliveryPurchaseExpensesTableData.value = [];
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
		const salesRow = shippingDeliveryContrctProductTableData.value[row];
		const salesContractId = salesRow?.contractId ?? salesRow?.id;
		const productCode = salesRow?.productCode;
		shippingDeliveryContrctProductTableData.value.splice(row, 1);
		const purchaseBeforeLen = shippingDeliveryPurchaseDetailsTableData.value.length;
		if (salesContractId != null && productCode != null) {
			// 按销售合同ID + 产品编号确认同一条数据后再删除对应采购行
			shippingDeliveryPurchaseDetailsTableData.value = shippingDeliveryPurchaseDetailsTableData.value.filter(
				item => {
					const sameContract = String(item.salesContractID ?? item.salesContractId ?? item.salesContract ?? '') === String(salesContractId);
					const sameProduct = (item.productCode ?? item.productNumber) === productCode;
					return !(sameContract && sameProduct);
				}
			);
		}
		// 参考合同下采购与销售常按同序排列，若按合同+产品未删到则按同索引删除
		if (shippingDeliveryPurchaseDetailsTableData.value.length === purchaseBeforeLen && row < purchaseBeforeLen) {
			shippingDeliveryPurchaseDetailsTableData.value.splice(row, 1);
		}
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
		const purchaseRow = shippingDeliveryPurchaseDetailsTableData.value[row];
		const salesContractId = purchaseRow?.salesContractID ?? purchaseRow?.salesContractId ?? purchaseRow?.salesContract;
		const productCode = purchaseRow?.productCode ?? purchaseRow?.productNumber;
		// 从数组中删除该采购明细
		shippingDeliveryPurchaseDetailsTableData.value.splice(row, 1);
		const salesBeforeLen = shippingDeliveryContrctProductTableData.value.length;
		if (salesContractId != null && productCode != null) {
			// 按销售合同ID + 产品编号确认同一条数据后再删除对应销售行
			shippingDeliveryContrctProductTableData.value = shippingDeliveryContrctProductTableData.value.filter(
				item => {
					const sameContract = String(item.contractId ?? item.id) === String(salesContractId);
					const sameProduct = item.productCode === productCode || item.productCode === purchaseRow?.productNumber;
					return !(sameContract && sameProduct);
				}
			);
		}
		// 参考合同下销售与采购常按同序排列，若按合同+产品未删到则按同索引删除
		if (shippingDeliveryContrctProductTableData.value.length === salesBeforeLen && row < salesBeforeLen) {
			shippingDeliveryContrctProductTableData.value.splice(row, 1);
		}
		ElMessage.success('删除成功');

	}).catch(() => {
		// 用户取消删除操作
		ElMessage.info('已取消删除');
	});
};

// 销售行包装与总重/总体积计算（与销售合同页面公式一致）
const recalcSalesRowPackaging = (row) => {
	const L = Number(row.outerBoxLength) || 0;
	const W = Number(row.outerBoxWidth) || 0;
	const H = Number(row.outerBoxHeight) || 0;
	const shipmentQty = Number(row.shipmentQuantity) || 0;
	const outerQty = Number(row.outerBoxQuantity) || 1;
	const outerNet = Number(row.outerBoxNetWeight) || 0;
	const outerGross = Number(row.outerBoxGrossWeight) || 0;
	// 外箱体积 = (外箱长度*外箱宽度*外箱高度)/1000000，保留4位小数
	if (L && W && H) {
		row.outerBoxVolume = (L * W * H / 1000000).toFixed(4);
	}
	const boxCount = outerQty > 0 ? Math.ceil(shipmentQty / outerQty) : 0;
	row.boxCount = boxCount;
	const outerVol = Number(row.outerBoxVolume) || 0;
	// 总净重 = 外箱净重 x 箱数，保留1位小数
	row.totalNetWeight = (outerNet * boxCount).toFixed(1);
	// 总毛重 = 外箱毛重 x 箱数，保留1位小数
	row.totalGrossWeight = (outerGross * boxCount).toFixed(1);
	// 总体积 = 外箱体积 x 箱数，保留1位小数
	row.totalVolume = (outerVol * boxCount).toFixed(1);
	AddShippingDeliveryform.value.shipmentTotalAmount = Number(calculateShipmentTotalAmount());
};

// 发货数量变化处理
const shipmentQuantityChange = (row) => {
	try {
		const shipmentQty = Number(row.shipmentQuantity)
		const remainingQty = Number(row.RemainingQuantityToBeShipped)
		let originalQty = 0
		if (isEditable.value) {
			// 查找对应的原始数据
			originalQty = OriginalShipmentQuantity.value.find(item =>
				item.contractProductId.toString() === row.contractProductId.toString() &&
				item.contractId.toString() === row.contractId.toString()
			)?.OriginalShipmentQuantity || 0
		}
		const maxAllowedQty = remainingQty + originalQty
		// 验证输入
		if (isNaN(shipmentQty)) {
			ElMessage.warning('请输入有效的数字')
			row.shipmentQuantity = remainingQty
			row.exportTotalPrice = (remainingQty * Number(row.exportUnitPrice)).toFixed(2);
			recalcSalesRowPackaging(row);
			const salesContractIdRestore = row.contractId ?? row.id;
			shippingDeliveryPurchaseDetailsTableData.value.filter(
				item => (item.salesContractID == salesContractIdRestore || item.salesContractId == salesContractIdRestore) &&
					(item.productCode === row.productCode || item.productNumber === row.productCode)
			).forEach(p => { p.shipmentQuantity = remainingQty; });
			return
		}

		// 出货数量必须大于0，等于0时自动恢复为剩余待出货数量
		if (shipmentQty <= 0) {
			ElMessage.warning('出货数量必须大于0，已恢复为剩余待出货数量')
			row.shipmentQuantity = remainingQty
			row.exportTotalPrice = (remainingQty * Number(row.exportUnitPrice)).toFixed(2);
			recalcSalesRowPackaging(row);
			const salesContractId = row.contractId ?? row.id;
			shippingDeliveryPurchaseDetailsTableData.value.filter(
				item => (item.salesContractID == salesContractId || item.salesContractId == salesContractId) &&
					(item.productCode === row.productCode || item.productNumber === row.productCode)
			).forEach(p => { p.shipmentQuantity = remainingQty; });
			return
		}

		// 验证是否超过最大允许数量
		if (shipmentQty > maxAllowedQty) {
			ElMessageBox.alert(`合同数量为${maxAllowedQty},当前出运数量为${shipmentQty},大于合同数量,请知悉！`, '提示', {
				confirmButtonText: '确定',
				showClose: false
			})
		}
		// 更新相关计算（总毛重/总体积/总净重与销售合同公式一致）
		row.exportTotalPrice = (shipmentQty * Number(row.exportUnitPrice)).toFixed(2);
		recalcSalesRowPackaging(row);

		// 采购合同出货数量与销售合同实时同步：按销售合同ID+产品编号匹配并更新
		const salesContractId = row.contractId ?? row.id;
		const purchaseDetails = shippingDeliveryPurchaseDetailsTableData.value.filter(
			item => String(item.salesContractID ?? item.salesContractId ?? item.salesContract ?? '') === String(salesContractId) &&
				(item.productCode === row.productCode || item.productNumber === row.productCode)
		);
		if (purchaseDetails.length > 0) {
			purchaseDetails.forEach(p => { p.shipmentQuantity = row.shipmentQuantity; });
		} else {
			// 兜底：未完结合同等场景下销售与采购常按同序排列，按同索引同步
			const salesIndex = shippingDeliveryContrctProductTableData.value.indexOf(row);
			if (salesIndex >= 0 && salesIndex < shippingDeliveryPurchaseDetailsTableData.value.length) {
				shippingDeliveryPurchaseDetailsTableData.value[salesIndex].shipmentQuantity = row.shipmentQuantity;
			}
		}

	} catch (error) {
		console.error('出货数量变更处理错误：', error);
		ElMessage.error('出货数量计算出错，请重试');
	}
};

// 定义接口
interface OriginalQuantity {
	OriginalShipmentQuantity: number;
	contractProductId: number;
	contractId: number;
}

// 数组类型
const OriginalShipmentQuantity = ref<OriginalQuantity[]>([])
//编辑
const EditClick = () => {
	SettingsShippingDeliveriesContractEditLock(IsEditShippingDeliveryID.value);
	// 清空之前的数据
	OriginalShipmentQuantity.value = []

	// 保存所有产品的原始出货数量
	shippingDeliveryContrctProductTableData.value.forEach(item => {
		OriginalShipmentQuantity.value.push({
			OriginalShipmentQuantity: Number(item.shipmentQuantity) || 0,
			contractProductId: item.contractProductId,
			contractId: item.contractId
		})
	})
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


// 计算出运单总金额
const calculateShipmentTotalAmount = () => {
	let productTotal = 0

	// 计算产品金额总和
	shippingDeliveryContrctProductTableData.value.forEach(item => {
		productTotal += Number(item.shipmentQuantity) * Number(item.singlesalesrevenue)
	})

	// 确保返回有效数字
	console.log("总金额", productTotal);
	return isNaN(productTotal) ? '0.00' : productTotal.toFixed(3)
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

// 存储审批步骤数据
const approvalSteps = ref([])
// 获取审批流程
const getApprovalFlow = async (documentId: number) => {
	try {
		const res = await request({
			url: 'ShippingDeliveries/GetShippingDeliveriesApprovalFlowByPaymentRequestID/GetApprovalFlow',
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
		ApproveDocumentRequest.DocumentID = ShippingDeliveriesID.value;
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
				CreateshippingdeliveryDialog.value = false;
				// 刷新合同列表
				GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
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
				CreateshippingdeliveryDialog.value = false;
				// 刷新合同列表
				GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
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

const confirmDeleteWithCountdown = (message) => {
	return new Promise((resolve, reject) => {
		let countdown = 15;
		let timer = 0;
		ElMessageBox.confirm(message, '二次确认', {
			confirmButtonText: `确认删除(${countdown}s)`,
			cancelButtonText: '取消',
			type: 'warning',
			distinguishCancelAndClose: true,
			closeOnClickModal: false,
			closeOnPressEscape: false
		}).then(() => {
			window.clearInterval(timer);
			resolve(true);
		}).catch((action) => {
			window.clearInterval(timer);
			reject(action);
		});

		setTimeout(() => {
			const boxes = document.querySelectorAll('.el-message-box');
			const currentBox = boxes[boxes.length - 1] as HTMLElement | undefined;
			const confirmBtn = currentBox?.querySelector('.el-message-box__btns .el-button--primary') as HTMLButtonElement | null;
			if (!confirmBtn) return;

			confirmBtn.disabled = true;
			confirmBtn.setAttribute('disabled', 'disabled');
			confirmBtn.classList.add('is-disabled');
			timer = window.setInterval(() => {
				if (!confirmBtn.isConnected) {
					window.clearInterval(timer);
					return;
				}
				countdown -= 1;
				if (countdown > 0) {
					confirmBtn.textContent = `确认删除(${countdown}s)`;
					return;
				}
				window.clearInterval(timer);
				confirmBtn.disabled = false;
				confirmBtn.removeAttribute('disabled');
				confirmBtn.classList.remove('is-disabled');
				confirmBtn.textContent = '确认删除';
			}, 1000);
		}, 0);
	});
};

// 删除出运发货单（与待删除单据删除接口一致）
const DeleteShipingDelivery = (row) => {
	confirmDeleteWithCountdown('确定要删除该出运发货单吗？此操作不可恢复。').then(() => {
		request.post('ApplyDeleteDocument/DelDocumentRequest/DelDocument', {
			DocumentID: row.id,
			DocumentType: DOCUMENT_TYPE_SHIPPING_DELIVERY
		}).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg || '单据删除成功',
					type: response.code === 200 ? 'success' : 'error'
				});
				if (response.code === 200) {
					GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
				}
			} else {
				ElMessage({ message: '单据删除失败', type: 'error' });
			}
		}).catch(error => {
			console.error('删除出运发货单出错', error);
			ElMessage({ message: '删除失败', type: 'error' });
		});
	}).catch(() => {
		ElMessage.info('已取消删除');
	});
};

const DOCUMENT_TYPE_SHIPPING_DELIVERY = 4

const applyDeleteDocument = (row) => {
	ElMessageBox.prompt('请输入申请删除备注：', '申请删除单据', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		inputType: 'textarea',
		inputPlaceholder: '请输入备注（选填）',
		inputValue: '',
		type: 'warning'
	}).then(({ value }) => {
		request.post('ApplyDeleteDocument/AddSysDocumentDeleteRequest/AddApplyDeleteDocument', {
			DocumentID: row.id,
			DocumentType: DOCUMENT_TYPE_SHIPPING_DELIVERY,
			Remark: value?.trim() || ''
		}).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg || '单据申请删除提交成功，已进入审批阶段',
					type: response.code === 200 ? 'success' : 'error'
				});
				if (response.code === 200) {
					GetShippingDeliveriesList(ShippingDeliveriesTableDataCurrentPage.value, ShippingDeliveriesTableDataPageSize.value);
				}
			} else {
				ElMessage({ message: '单据申请删除提交失败', type: 'error' });
			}
		}).catch(error => {
			console.error('申请删除单据出错', error);
			ElMessage({ message: '申请删除单据失败', type: 'error' });
		});
	}).catch(() => {
		ElMessage({ type: 'info', message: '已取消申请' });
	});
};
</script>
