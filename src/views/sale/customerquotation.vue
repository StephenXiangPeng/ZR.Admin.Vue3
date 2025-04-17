<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenQuotationDialog">创建报价</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="SearchQuotationNum" filterable placeholder="选择报价单号（可输入查询）" style="width: 15%">
				<el-option v-for="dict in optionss.sql_hr_all_quotationnum" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SechaerCustomerSelect" filterable placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="dict in optionss.sql_hr_customer_name" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchInquiryDate" type="date" placeholder="请选择询价日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchRealQuotationDate" type="date" placeholder="请选择报价日期" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 5px;">
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="search">查询</el-button>
				<el-button @click="reset">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;报价单</span>
		</div>
		<el-divider></el-divider>
		<el-table :data="quotationData" stripe>
			<el-table-column prop="quotationNum" label="报价单号" :width="150">
				<template #default="scope">
					<span>{{ scope.row.quotationNum }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="inquiryDate" label="询价日期" :width="180" :formatter="formatDate" />
			<el-table-column prop="realQuotationDate" label="实际报价日期" :width="180" :formatter="formatDate" />
			<el-table-column prop="quotationStatus" label="报价状态" :width="150" v-if="false" />
			<el-table-column prop="validityPeriod" label="有效期限" :width="150" />
			<el-table-column prop="customerNum" label="客户编号" :width="150" />
			<el-table-column prop="customerName" label="客户简称" :width="150" />
			<el-table-column prop="totalValueOfGoods" label="货值合计" :width="150" />
			<el-table-column prop="totalQuantity" label="数量合计" :width="150" v-if="false" />
			<el-table-column prop="totalNumberOfBoxes" label="箱数合计" :width="150" v-if="false" />
			<el-table-column prop="totalGrossWeight" label="毛重合计" :width="150" v-if="false" />
			<el-table-column prop="totalNetWeight" label="净重合计" :width="150" v-if="false" />
			<el-table-column prop="totalVolume" label="体积合计" :width="150" v-if="false" />
			<el-table-column prop="totalPurchases" label="采购合计" :width="150" v-if="false" />
			<el-table-column prop="totalOtherFees" label="其它费用合计" :width="150" v-if="false" />
			<el-table-column prop="totalTaxRefund" label="退税总额" :width="150" v-if="false" />
			<el-table-column prop="profitAmount" label="利润金额" :width="150" />
			<el-table-column prop="createBy" label="创建人" :width="150" />
			<el-table-column fixed="right" prop="operate" label="操作" :width="150">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small" @click="ChcekDetails(scope.row)">查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />

		<el-dialog v-model="quotationDialog" title="创建报价单" :close-on-click-modal=false style="width: 70%;"
			@close="quotationDialogHandClose">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form ref="quotationDialogformRef" :rules="quotationDialogformRules" :model="quotationDialogform"
				label-width="auto" :inline="true">
				<el-form-item label="报价单号">
					<el-input v-model="quotationDialogform.quotationnum" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="询价日期" prop="inquirydate" data-field="inquirydate">
					<el-date-picker v-model="quotationDialogform.inquirydate" type="date" placeholder="选择日期"
						style="width: 250px;" :disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="实际报价日期" prop="realquotationdate" data-field="realquotationdate">
					<el-date-picker v-model="quotationDialogform.realquotationdate" type="date" placeholder="选择日期"
						style="width: 250px;" :disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="有效期限" prop="validityperiod" data-field="validityperiod">
					<el-input v-model="quotationDialogform.validityperiod" style="width: 250px;"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="报价状态">
					<el-select v-model="quotationDialogform.quorationstatus" filterable placeholder="选择报价状态" disabled
						style="width: 250px;">
						<el-option v-for="dict in optionss.hr_quotation_status" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="客户编号" prop="customernum" v-if="false">
					<el-select v-model="quotationDialogform.customernum" filterable placeholder="选择客户编号"
						:disabled="isDisabled" style="width: 250px;" @change="handleCustomerSelection">
						<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户简称" prop="customername" data-field="customername">
					<el-select v-model="quotationDialogform.customername" filterable placeholder="选择客户名称"
						:disabled="isDisabled" style="width: 250px;" @change="handleCustomerSelection">
						<el-option v-for="dict in optionss.sql_hr_customer_abbreviation" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="联系人" prop="contactperson" data-field="contactperson">
					<el-select v-model="quotationDialogform.contactperson" filterable placeholder="选择联系人"
						:disabled="isDisabled" style="width: 250px;" @change="handleContactpersonSelection">
						<el-option v-for="item in contactpersonSelectOptions" :key="item.value" :label="item.label"
							:value="item.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="联系人Email">
					<el-input v-model="quotationDialogform.contactpersonEmail" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="客户等级">
					<el-select v-model="quotationDialogform.customerlevel" filterable placeholder="自动评级" disabled
						style="width: 250px;">
						<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="我方公司" prop="ourcompany" data-field="ourcompany">
					<el-select v-model="quotationDialogform.ourcompany" filterable placeholder="请选择我方公司"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="结算类别" v-show="false">
					<el-select v-model="quotationDialogform.settlementcategory" filterable placeholder="选择结算类别"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_settlementcategory" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="外销币种" prop="exportcurrency" data-field="exportcurrency">
					<el-select v-model="quotationDialogform.exportcurrency" filterable placeholder="选择外销币种"
						:disabled="isDisabled" style="width: 250px;" @change="exportcurrencyChange">
						<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="汇率" prop="exchangerate" data-field="exchangerate">
					<el-input v-model="quotationDialogform.exchangerate" style="width: 250px;" :disabled="isDisabled"
						@change="calculateTotal" />
				</el-form-item>
				<el-form-item label="统一利润率">
					<el-input v-model="quotationDialogform.uniformprofitmargin" style="width: 250px;"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="结汇方式" prop="settlementway" data-field="settlementway">
					<el-select v-model="quotationDialogform.settlementway" filterable placeholder="选择结汇方式"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="价格条款" prop="pricingterm" data-field="pricingterm">
					<el-select v-model="quotationDialogform.pricingterm" filterable placeholder="选择价格条款"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="出运口岸" prop="shippingport" data-field="shippingport">
					<el-select v-model="quotationDialogform.shippingport" filterable placeholder="选择出运口岸"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="目的口岸" prop="destinationport" data-field="destinationport">
					<el-input v-model="quotationDialogform.destinationport" :disabled="isDisabled"
						style="width: 250px;" />
				</el-form-item>
				<el-form-item label="贸易国别" prop="tradingcountry" data-field="tradingcountry">
					<el-select v-model="quotationDialogform.tradingcountry" filterable placeholder="选择贸易国别"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="运输方式" prop="transportationmethod" data-field="transportationmethod">
					<el-select v-model="quotationDialogform.transportationmethod" filterable placeholder="选择运输方式"
						:disabled="isDisabled" style="width: 250px;">
						<el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>

				<el-form-item label="单位运费" v-show="false">
					<el-input v-model="quotationDialogform.unitfreight" style="width: 250px;" :disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="佣金比率">
					<el-input v-model="quotationDialogform.commissionrate" style="width: 250px;"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="海运费/m³">
					<el-input v-model="quotationDialogform.oceanFreight" style="width: 250px;" @change="calculateTotal"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="海运费币种" prop="shippingcurrency">
					<el-select v-model="quotationDialogform.shippingcurrency" filterable placeholder="选择运费币种"
						:disabled="isDisabled" style="width: 250px;" @change="shippingcurrencyChange">
						<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="海运费汇率">
					<el-input v-model="quotationDialogform.shippingrate" style="width: 250px;" :disabled="isDisabled"
						@change="calculateTotal" />
				</el-form-item>
				<el-form-item label="港杂费/m³">
					<el-input v-model="quotationDialogform.portMiscellaneousFees" style="width: 250px;"
						@change="calculateTotal" :disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="内陆运费/m³" v-show="false">
					<el-input v-model="quotationDialogform.inlandFreight" style="width: 250px;" @change="calculateTotal"
						:disabled="isDisabled" />
				</el-form-item>

				<el-form-item label="单个立方费用" v-show="false">
					<el-input v-model="quotationDialogform.singleCubicCost" style="width: 250px;"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="货代报关杂费">
					<el-input v-model="quotationDialogform.freightForwarderCustomsClearanceFees" style="width: 250px;"
						:disabled="isDisabled" />
				</el-form-item>
				<el-form-item label="销售员">
					<el-select v-model="quotationDialogform.seller" filterable placeholder="选择销售员" disabled
						style="width: 250px;">
						<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</el-form-item>
				<br><span style="font-size: 20px; font-weight: bold;">产品信息</span>
				<el-divider></el-divider>
				<el-button class="mt-4" type="primary" @click="OpenSearchProcutDialog" style="margin-bottom: 10px;"
					:disabled="isDisabled">导入产品</el-button>
				<el-button class="mt-4" type="primary" @click="onAddquotationProductItem" style="margin-bottom: 10px;"
					:disabled="isDisabled">添加新产品</el-button>
				<el-table :data="productData" style="width: 100%;margin-bottom: 15px;" max-height="550">
					<el-table-column prop="productNum" label="产品编号" width="120" />
					<el-table-column prop="customerNum" label="客户货号" width="120" />
					<el-table-column prop="cproductname" label="中文品名" width="120">
						<template #default="{ row }">
							<span v-if="row.isImported">{{ row.cproductname }}</span>
							<el-input v-else v-model="row.cproductname" :disabled="isDisabled"></el-input>
						</template>
					</el-table-column>
					<el-table-column prop="cspecification" label="中文规格" width="120">
						<template #default="{ row }">
							<span v-if="row.isImported">{{ row.cspecification }}</span>
							<el-input v-else v-model="row.cspecification" :disabled="isDisabled"></el-input>
						</template>
					</el-table-column>
					<el-table-column prop="ProfitMargin" label="利润率%" width="100">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'ProfitMargin')" v-model="row.ProfitMargin"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="quotationnum" label="报价数量" width="110">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'quotationnum')" v-model="row.quotationnum"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="exportunitprice" label="外销单价" width="110">
						<template #default="{ row }">
							<el-input v-model="row.exportunitprice" @change="calculateTotal" :disabled="true" />
						</template>
					</el-table-column>
					<el-table-column prop="exporttotalprice" label="外销总价" width="110">
						<template #default="scope">
							<span>{{ scope.row.exporttotalprice }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="unitofmeasurement" label="计量单位" width="100">
						<template #default="scope">
							<el-select v-model="scope.row.unitofmeasurement" filterable placeholder="单位"
								style="width: 100%;" :disabled="true">
								<el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="purchasecurrency" label="采购币种" width="110">
						<template #default="scope">
							<el-select v-model="scope.row.purchasecurrency" filterable placeholder="币种"
								style="width: 100%;" :disabled="isDisabled">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="purchaseunitprice" label="采购单价" width="110">
						<template #default="{ row }">
							<el-input @blur="formatNumber2(row, 'purchaseunitprice')" v-model="row.purchaseunitprice"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="inlandfreightprice" label="内陆运费(m³)" width="130">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'inlandfreightprice')" v-model="row.inlandfreightprice"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="additionalpackagingcosts" label="单个产品额外包装费用" width="180">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'additionalpackagingcosts')"
								v-model="row.additionalpackagingcosts" @change="calculateTotal"
								:disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="singleProductGrossProfit" label="单个产品毛利" width="130">
						<template #default="scope">
							<span>{{ scope.row.singleProductGrossProfit }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="singleProductGrossProfitTotal" label="单个产品毛利合计" width="160">
						<template #default="scope">
							<span>{{ scope.row.singleProductGrossProfitTotal }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="grossProfitRate" label="毛利率%" width="110" v-if="false">
						<template #default="scope">
							<span>{{ scope.row.grossProfitRate }}</span>
						</template>
					</el-table-column>
					<!-- <el-table-column prop="onepacking" label="单个产品包装物" width="140" v-show="false">
						<template #default="{ row }">
							<el-input v-model="row.onepacking" size="small" @change="calculateTotal"
								:disabled="isDisabled" />
						</template>
					</el-table-column> -->
					<el-table-column prop="isInvoicingc" label="是否开票" width="120">
						<template #default="scope">
							<el-select v-model="scope.row.isInvoicingc" filterable placeholder="请选择"
								style="width: 100%;" :disabled="isDisabled">
								<el-option v-for="dict in optionss.hr_yes_no" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="packaging" label="包装方式" width="150">
						<template #default="scope">
							<el-select v-model="scope.row.packaging" filterable :disabled="isDisabled" placeholder="请选择"
								style="width: 100%;">
								<el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="specialrequirements" label="特殊要求" width="200">
						<template #default="{ row }">
							<el-input v-model="row.specialrequirements" @change="calculateTotal"
								:disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="rebaterate" label="退税率%" width="100">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'rebaterate')" v-model="row.rebaterate"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>

					<el-table-column prop="innerBoxLoading" label="内盒装量" width="100">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'innerBoxLoading')" v-model="row.innerBoxLoading"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxloading" label="外箱装量" width="100">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'outerboxloading')" v-model="row.outerboxloading"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxunit" label="外箱单位" width="150">
						<template #default="scope">
							<el-select v-model="scope.row.outerboxunit" filterable placeholder="外箱单位"
								style="width: 100%;" :disabled="isDisabled">
								<el-option v-for="dict in optionss.hr_outerbox_unit" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="outerboxlength" label="外箱长度(CM)" width="120">
						<template #default="{ row }">
							<el-input v-model="row.outerboxlength" @change="calculateTotal"
								@blur="formatNumber(row, 'outerboxlength')" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxwidth" label="外箱宽度(CM)" width="120">
						<template #default="{ row }">
							<el-input v-model="row.outerboxwidth" @change="calculateTotal"
								@blur="formatNumber(row, 'outerboxwidth')" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxheight" label="外箱高度(CM)" width="120">
						<template #default="{ row }">
							<el-input v-model="row.outerboxheight" @change="calculateTotal"
								@blur="formatNumber(row, 'outerboxheight')" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxnetweight" label="外箱净重(KG)" width="120">
						<template #default="{ row }">
							<el-input v-model="row.outerboxnetweight" @change="calculateTotal"
								@blur="formatNumber(row, 'outerboxnetweight')" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxgrossweight" label="外箱毛重(KG)" width="120">
						<template #default="{ row }">
							<el-input v-model="row.outerboxgrossweight" @change="calculateTotal"
								@blur="formatNumber(row, 'outerboxgrossweight')" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="outerboxvolume" label="外箱体积(m³)" width="120">
						<template #default="scope">
							<span>{{ scope.row.outerboxvolume }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="NumberOfBoxes" label="箱数" width="100">
						<template #default="scope">
							<span>{{ scope.row.NumberOfBoxes }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="totalNetWeight" label="总净重(KG)" width="130">
						<template #default="scope">
							<span>{{ scope.row.totalNetWeight }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="totalGrossWeight" label="总毛重(KG)" width="130">
						<template #default="scope">
							<span>{{ scope.row.totalGrossWeight }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="totalVolume" label="总体积(m³)" width="130">
						<template #default="scope">
							<span>{{ scope.row.totalVolume }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="OtherFees" label="单个产品其它费用" width="170">
						<template #default="{ row }">
							<el-input @blur="formatNumber(row, 'OtherFees')" v-model="row.OtherFees"
								@change="calculateTotal" :disabled="isDisabled" />
						</template>
					</el-table-column>
					<el-table-column prop="SinglesalesrevenueA" label="单个销售收入A" width="170" v-if="true">
						<template #default="scope">
							<span>{{ scope.row.SinglesalesrevenueA }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="Singleproductvolume" label="单个产品体积(m³)" width="170" v-if="true">
						<template #default="scope">
							<span>{{ scope.row.Singleproductvolume }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="Portchargesforindividualproducts" label="单个产品的港杂费" width="170" v-if="true">
						<template #default="scope">
							<span>{{ scope.row.Portchargesforindividualproducts }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="Oceanfreightforasingleproduct" label="单个产品海运费" width="130" v-if="true">
						<template #default="scope">
							<span>{{ scope.row.Oceanfreightforasingleproduct }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="Inlandfreightforasingleproduct" label="单个产品内陆运费" width="170" v-if="true">
						<template #default="scope">
							<span>{{ scope.row.Inlandfreightforasingleproduct }}</span>
						</template>
					</el-table-column>
					<el-table-column fixed="right" label="操作" width="120">
						<template #default="scope">
							<el-button :disabled="isDisabled" link type="primary"
								@click.prevent="deleteRow(scope.$index)">
								删除
							</el-button>
						</template>
					</el-table-column>
				</el-table>
				<span style="font-size: 20px; font-weight: bold;">利润预估</span>
				<el-divider></el-divider>
				<el-form-item label="货值合计">
					<el-input v-model="quotationDialogform.TotalValueOfGoods" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="数量合计">
					<el-input v-model="quotationDialogform.TotalQuantity" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="箱数合计">
					<el-input v-model="quotationDialogform.TotalNumberOfBoxes" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="毛重合计">
					<el-input v-model="quotationDialogform.TotalGrossWeight" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="净重合计">
					<el-input v-model="quotationDialogform.TotalNetWeight" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="体积合计">
					<el-input v-model="quotationDialogform.TotalVolume" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="采购合计">
					<el-input v-model="quotationDialogform.TotalPurchases" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="其它费用合计">
					<el-input v-model="quotationDialogform.TotalOtherFees" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="退税总额">
					<el-input v-model="quotationDialogform.TotalTaxRefund" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="利润金额">
					<el-input v-model="quotationDialogform.ProfitAmount" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="总毛利合计">
					<el-input v-model="quotationDialogform.Totalgrossprofit" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="总利润率%">
					<el-input v-model="quotationDialogform.Totalprofitmargin" disabled style="width: 250px;" />
				</el-form-item>
				<el-form-item label="银行费用">
					<el-input v-model="quotationDialogform.BankFee" :disabled="isDisabled" style="width: 250px;"
						@change="calculateTotal" />
				</el-form-item>
				<el-form-item label="文件杂费">
					<el-input v-model="quotationDialogform.DocumentationFees" :disabled="isDisabled"
						style="width: 250px;" @change="calculateTotal" />
				</el-form-item>
				<br><span style="font-size: 20px; font-weight: bold;">备注信息</span>
				<el-divider></el-divider>
				<el-form-item label="报价备注：" style="width: 100%;">
					<el-input v-model="quotationDialogform.Remark" :autosize="{ minRows: 5, maxRows: 10 }"
						type="textarea" :disabled="isDisabled" placeholder="输入报价备注内容" />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" v-show="showEditBtn" @click="EditQuotation">
						编辑
					</el-button>
					<el-button type="primary" @click="SaveDraft" v-if="showSaveDraftBtn">
						保存草稿
					</el-button>
					<el-button v-show="isSaveBtnShow" type="success" @click="AddQuotation(quotationDialogformRef)">
						提交
					</el-button>
					<el-button type="success" v-show="showEditSaveBtn"
						@click="EditSaveQuotation(quotationDialogformRef)">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="SearchProcutDialog" title="选择产品" :close-on-click-modal=false :width="'50%'"
			@close="handleCloseSearchProcutDialog">
			<el-input v-model="searchProductNameText" placeholder="请输入产品关键字进行搜索" style="margin-bottom: 10px;"
				@input="searchProductNameTextChange" />
			<el-table :data="productDatatwo" style="width: 100%"
				:default-sort="{ prop: 'productCode', order: 'descending' }" @row-dblclick="handleRowDblClick" stripe>
				<el-table-column prop="productCode" label="产品编号" sortable width="120" />
				<el-table-column prop="customerGoodsNumber" label="客户货号" width="120" />
				<el-table-column prop="chineseProductName" label="中文品名" width="150" />
				<el-table-column prop="englishProductName" label="英文品名" width="180" />
				<el-table-column prop="chineseSpecification" label="中文规格" width="150" />
				<el-table-column prop="englishSpecification" label="英文规格" width="180" />
				<el-table-column prop="unitOfMeasurement" label="计量单位" width="120" />
			</el-table>
			<el-pagination @current-change="SearchProducthandlePageChange" :current-page="SearchProductCurrentPage"
				:page-size="SearchProductpageSize" :total="SearchProducttotalItems" background
				layout="prev, pager, next" style="margin-top: 5px;" />
			<template #footer>
				<span class="dialog-footer">
					<el-button type="danger" @click="SearchProcutDialog = false">
						关闭
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<style scoped>
.highlight-error {
	animation: highlight 3s ease-in-out;
	border-color: #f56c6c !important;
	box-shadow: 0 0 0 2px rgba(245, 108, 108, 0.2) !important;
}

@keyframes highlight {
	0% {
		background-color: rgba(245, 108, 108, 0.1);
	}

	100% {
		background-color: transparent;
	}
}
</style>

<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElMessageBox, ElMessage, FormInstance, FormRules } from 'element-plus'
import request from '@/utils/request';
import { Search } from '@element-plus/icons-vue';
import DictData from '../components/dictData.vue';
import { isNumber } from '@vueuse/core';
import useUserStore from "@/store/modules/user";
import { number } from 'echarts';

const isSaveBtnShow = ref(false);
const showEditBtn = ref(false);
const showEditSaveBtn = ref(false);
const isDisabled = ref(false);
const isReviewBtnShow = ref(false);
const showSaveDraftBtn = ref(false);
const isViewDetails = ref(false); // 新增变量，用于标记是否是通过查看详情打开的对话框
const QuotationRemarksTextarea = ref('');
var userId = useUserStore().userId;
//查询条件
const SearchQuotationNum = ref('');
const SechaerCustomerSelect = ref('');
const SearchInquiryDate = ref('');
const SearchRealQuotationDate = ref('');

const reset = () => {
	SearchQuotationNum.value = ''
	SechaerCustomerSelect.value = ''
	SearchInquiryDate.value = ''
	SearchRealQuotationDate.value = ''
	GetQuotationList(currentPage.value, pageSize.value);
}

const search = () => {
	GetQuotationList(currentPage.value, pageSize.value);
}

//创建报价窗体
const quotationDialog = ref(false)

interface quotationDialogform {
	id: number;
	//报价单号
	quotationnum: string
	//询价日期
	inquirydate: string,
	//实际报价日期
	realquotationdate: string,
	//有效期限
	validityperiod: number,
	//报价状态
	quorationstatus: number,
	//客户ID
	customerid: number,
	//客户编号
	customernum: string,
	//客户简称
	customername: string,
	//联系人
	contactperson: number,
	//联系人Email
	contactpersonEmail: string,
	//客户等级
	customerlevel: number,
	//我方公司
	ourcompany: number
	//结算类别
	settlementcategory: number,
	//外销币种
	exportcurrency: number,
	//汇率
	exchangerate: number,
	//统一利润率
	uniformprofitmargin: number,
	//结汇方式
	settlementway: number,
	//价格条款
	pricingterm: number,
	//出运口岸
	shippingport: number,
	//目的口岸
	destinationport: string,
	//贸易国别
	tradingcountry: number,
	//运输方式
	transportationmethod: number,
	//运费币种
	shippingcurrency: number,
	//运费汇率
	shippingrate: number,
	//单位运费
	unitfreight: number,
	//佣金比率
	commissionrate: number,
	//销售员
	seller: number,
	//港杂费
	portMiscellaneousFees: number,
	//内陆运费
	inlandFreight: number,
	//海运费
	oceanFreight: number,
	//单个立方费用
	singleCubicCost: number,
	//货代报关杂费
	freightForwarderCustomsClearanceFees: number,
	//货值合计
	TotalValueOfGoods: number,
	//数量合计
	TotalQuantity: number,
	//箱数合计
	TotalNumberOfBoxes: number,
	//毛重合计
	TotalGrossWeight: number,
	//净重合计
	TotalNetWeight: number,
	//体积合计
	TotalVolume: number,
	//采购合计
	TotalPurchases: number,
	//其它费用合计
	TotalOtherFees: number,
	//退税总额
	TotalTaxRefund: number,
	//利润金额
	ProfitAmount: number,
	//总毛利合计
	Totalgrossprofit: number,
	//总利润率%
	Totalprofitmargin: number,
	//备注
	Remark: string,
	//银行费用
	BankFee: number,
	//文件杂费
	DocumentationFees: number,
	//是否草稿
	isDraft: number

}
const quotationDialogformRef = ref<FormInstance>()
//报价单from参数
const quotationDialogform = reactive<quotationDialogform>({
	//报价单ID
	id: null,
	//报价单号
	quotationnum: '',
	//询价日期
	inquirydate: '',
	//实际报价日期
	realquotationdate: '',
	//有效期限
	validityperiod: null,
	//报价状态
	quorationstatus: null,
	//客户ID
	customerid: null,
	//客户编号
	customernum: '',
	//客户简称
	customername: '',
	//联系人
	contactperson: null,
	//联系人Email
	contactpersonEmail: '',
	//客户等级
	customerlevel: null,
	//我方公司
	ourcompany: null,
	//结算类别
	settlementcategory: null,
	//外销币种
	exportcurrency: null,
	//汇率
	exchangerate: null,
	//统一利润率
	uniformprofitmargin: null,
	//结汇方式
	settlementway: null,
	//价格条款
	pricingterm: null,
	//出运口岸
	shippingport: null,
	//目的口岸
	destinationport: null,
	//贸易国别
	tradingcountry: null,
	//运输方式
	transportationmethod: null,
	//运费币种
	shippingcurrency: null,
	//运费汇率
	shippingrate: null,
	//单位运费
	unitfreight: null,
	//佣金比率
	commissionrate: null,
	//销售员
	seller: null,
	//港杂费
	portMiscellaneousFees: null,
	//内陆运费
	inlandFreight: null,
	//海运费
	oceanFreight: null,
	//单个立方费用
	singleCubicCost: null,
	//货代报关杂费
	freightForwarderCustomsClearanceFees: null,
	//货值合计
	TotalValueOfGoods: null,
	//数量合计
	TotalQuantity: null,
	//箱数合计
	TotalNumberOfBoxes: null,
	//毛重合计
	TotalGrossWeight: null,
	//净重合计
	TotalNetWeight: null,
	//体积合计
	TotalVolume: null,
	//采购合计
	TotalPurchases: null,
	//其它费用合计
	TotalOtherFees: null,
	//退税总额
	TotalTaxRefund: null,
	//利润金额
	ProfitAmount: null,
	//总毛利合计
	Totalgrossprofit: null,
	//总利润率%
	Totalprofitmargin: null,
	//备注
	Remark: '',
	//银行费用
	BankFee: null,
	//文件杂费
	DocumentationFees: null,
	//是否草稿
	isDraft: null
})

const quotationDialogformRules = reactive<FormRules<quotationDialogform>>({
	inquirydate: [{ required: true, message: '请选择询价日期', trigger: ['blur', 'change'] }],
	realquotationdate: [{ required: true, message: '请选择实际报价日期', trigger: ['blur', 'change'] }],
	validityperiod: [{ required: true, message: '请输入有效期限', trigger: ['blur', 'change'] }],
	customerid: [{ required: true, message: '请选择客户', trigger: ['blur', 'change'] }],
	customernum: [{ required: true, message: '请选择客户编号', trigger: ['blur', 'change'] }],
	customername: [{ required: true, message: '请选择客户简称', trigger: ['blur', 'change'] }],
	contactperson: [{ required: true, message: '请选择联系人', trigger: ['blur', 'change'] }],
	ourcompany: [{ required: true, message: '请选择我方公司', trigger: ['blur', 'change'] }],
	//settlementcategory: [{ required: true, message: '请选择结算类别', trigger: ['blur', 'change'] }],
	exportcurrency: [{ required: true, message: '请选择外销币种', trigger: ['blur', 'change'] }],
	exchangerate: [{ required: true, message: '请输入汇率', trigger: ['blur', 'change'] }],
	settlementway: [{ required: true, message: '请选择结汇方式', trigger: ['blur', 'change'] }],
	pricingterm: [{ required: true, message: '请选择价格条款', trigger: ['blur', 'change'] }],
	shippingport: [{ required: true, message: '请选择出运口岸', trigger: ['blur', 'change'] }],
	destinationport: [{ required: true, message: '请选择目的口岸', trigger: ['blur', 'change'] }],
	tradingcountry: [{ required: true, message: '请选择贸易国别', trigger: ['blur', 'change'] }],
	transportationmethod: [{ required: true, message: '请选择运输方式', trigger: ['blur', 'change'] }],
	shippingcurrency: [{ required: true, message: '请选择运费币种', trigger: ['blur', 'change'] }]
})

const OpenQuotationDialog = () => {
	GetNextQuotationNo();
	const currentDate = new Date();
	const formattedDate = currentDate.toISOString().split('T')[0];
	isSaveBtnShow.value = true;
	showEditBtn.value = false;
	showEditSaveBtn.value = false;
	showSaveDraftBtn.value = true;
	isViewDetails.value = false; // 新建时设置为false
	quotationDialog.value = true;
	for (let key in quotationDialogform) {
		quotationDialogform[key] = null;
	}
	quotationDialogform.inquirydate = formattedDate;
	quotationDialogform.realquotationdate = formattedDate;
	quotationDialogform.quorationstatus = state.optionss.hr_quotation_status[0].dictValue;
	quotationDialogform.shippingcurrency = state.optionss.hr_export_currency[2].dictValue;//默认人民币
	quotationDialogform.shippingrate = 1;//默认1
	quotationDialogform.seller = state.optionss.sql_all_user.find(item => item.dictValue == userId).dictValue;
};

//获取报价单最新编号
const GetNextQuotationNo = () => {
	request({
		url: 'Quotation/GetNextQuotaionNum/GetNextQuotaionNum',
		method: 'GET'
	}).then(response => {
		if (response.code == "200") {
			quotationDialogform.quotationnum = response.data;
		} else {
			ElMessage.error("获取客户编号失败，请重新打开客户建档窗体");
		}
	}).catch(error => {
		console.error(error);
	});
}

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		sql_hr_customer: [],
		hr_ourcompany: [],
		hr_quotation_status: [],
		hr_export_currency: [],
		hr_settlement_way: [],
		hr_pricing_term: [],
		hr_nation: [],
		sql_hr_sale: [],
		hr_transport_port: [],
		hr_transportation_method: [],
		hr_yes_no: [],
		hr_calculate_unit: [],
		sql_hr_customer_name: [],
		hr_settlementcategory: [],
		hr_customer_level: [],
		sql_all_user: [],
		sql_hr_customer_contactperson: [],
		sql_hr_all_quotationnum: [],
		sql_product: [],
		sql_product_name: [],
		sql_hr_customer_abbreviation: [],
		hr_packing: [],
		hr_outerbox_unit: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' },
	{ dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
	{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' },
	{ dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' },
	{ dictType: 'hr_transport_port' }, { dictType: 'hr_transportation_method' },
	{ dictType: 'hr_yes_no' }, { dictType: 'hr_calculate_unit' },
	{ dictType: 'hr_settlementcategory' }, { dicttype: 'sql_hr_customer_name' },
	{ dictType: 'hr_customer_level' }, { dictType: 'sql_all_user' },
	{ dictType: 'sql_hr_customer_contactperson' }, { dictType: 'sql_hr_all_quotationnum' },
	{ dictType: 'sql_product' }, { dictType: 'sql_product_name' },
	{ dictType: 'sql_hr_customer_abbreviation' }, { dicttype: 'hr_packing' },
	{ dictType: 'hr_outerbox_unit' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	GetQuotationList(currentPage.value, pageSize.value);
})
/*动态下拉框end*/
//联系人
const contactpersonSelectOptions = ref([]);
const handleCustomerSelection = (value) => {
	//清空联系人
	quotationDialogform.customerid = value;
	quotationDialogform.contactperson = null;
	quotationDialogform.contactpersonEmail = '';
	request({
		url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
		method: 'GET',
		params: {
			ID: value
		}
	}).then(response => {
		if (response != null) {
			quotationDialogform.customernum = value.toString();
			quotationDialogform.customername = value.toString();
			quotationDialogform.customerlevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == response.customerLevel).map(item => item.dictValue).values().next().value;
			quotationDialogform.tradingcountry = state.optionss['hr_nation'].filter(item => item.dictValue == response.tradingCountry).map(item => item.dictValue).values().next().value;
			if (response.contactPerson != null) {
				contactpersonSelectOptions.value = response.contactPerson.map(item => ({
					value: item.id,
					label: item.name,
					email: item.email
				}));
			}
		} else {
			ElMessage.error("获取客户信息失败，请重新打开客户建档窗体");
		}
	}).catch(error => {
		console.log(error)
	});
}

const SelectCustomerInfoEvent = (value) => {
	handleCustomerSelection(value);
}

const handleContactpersonSelection = (value) => {
	quotationDialogform.contactpersonEmail = contactpersonSelectOptions.value.find(item => item.value === value).email;
}

//查找产品窗体
const SearchProcutDialog = ref(false)
//查询产品信息表格
const productDatatwo = ref([])
const myTable = ref(null)
//报价产品表格
const productData = ref([])
//分页组件
const SearchProducttotalItems = ref(0);
const SearchProductCurrentPage = ref(1);
const SearchProductpageSize = ref(10);
const searchProductNameText = ref('');
const SearchProducthandlePageChange = async (newPage) => {
	SearchProductCurrentPage.value = newPage; // 修改这里，直接使用newPage作为当前页
	await GetProductInfoList(newPage, SearchProductpageSize.value);
};
GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
//获取产品信息列表
function GetProductInfoList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'ProductInformation/GetAllProductList/AllProduct',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				chineseProductName: searchProductNameText.value
			}
		}).then(response => {
			if (response.data.data.length > 0) {
				productDatatwo.value = response.data.data;
				SearchProducttotalItems.value = response.data.totalNum;
				SearchProductCurrentPage.value = response.data.pageIndex;
				productDatatwo.value.forEach(item => {
					item.unitOfMeasurement = state.optionss['hr_calculate_unit'].filter(hr_calculate_unit => hr_calculate_unit.dictValue == item.unitOfMeasurement).map(item => item.dictLabel).values().next().value;
				});
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					productDatatwo.value = [];
				}
			}
		}).catch(error => {
			console.error(error);
		});
	});
}

const searchProductNameTextChange = () => {
	GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
}

const handleRowDblClick = (row) => {
	const existingProduct = productData.value.find(p => p.productNum === row.productCode);
	if (existingProduct) {
		ElMessage.error("产品【" + row.chineseProductName + "】已存在报价单的产品列表中，请重新选择");
		return;
	} else {
		productData.value.push({
			productNum: row.productCode,
			customerNum: row.customerGoodsNumber,
			cproductname: row.chineseProductName,
			cspecification: row.chineseSpecification,
			quotationnum: 0,
			exportunitprice: 0,
			exporttotalprice: 0,
			unitofmeasurement: state.optionss['hr_calculate_unit'].filter(hr_calculate_unit => hr_calculate_unit.dictLabel == row.unitOfMeasurement).map(item => item.dictValue).values().next().value,
			purchaseinquiry: 0,
			purchaseunitprice: 0,
			purchasecurrency: state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictLabel == '人民币').map(item => item.dictValue).values().next().value,
			onepacking: 0,
			invoice: '',
			packaging: '',
			specialrequirements: '',
			rebaterate: 0,
			outerboxunit: state.optionss['hr_outerbox_unit'].filter(hr_outerbox_unit => hr_outerbox_unit.dictLabel == '箱').map(item => item.dictValue).values().next().value,
			outerboxlength: row.outerBoxLength,
			outerboxwidth: row.outerBoxWidth,
			outerboxheight: row.outerBoxHeight,
			outerboxnetweight: row.outerBoxNetWeight,
			outerboxgrossweight: row.outerBoxGrossWeight,
			outerboxvolume: 0,
			NumberOfBoxes: 0,
			OtherFees: 0,
			additionalpackagingcosts: 0,
			singleProductGrossProfit: 0,
			grossProfitRate: 0,
			totalNetWeight: 0,
			totalGrossWeight: 0,
			totalVolume: 0,
			innerBoxLoading: row.mediumPackagingVolume,
			outerboxloading: row.outerBoxPackingQuantity,
			inlandfreightprice: 0,
			isImported: true
		});
	}
};

const deleteRow = (index: number) => {
	productData.value.splice(index, 1);
	calculateTotal();
}
const isImportProduct = ref(false);
const OpenSearchProcutDialog = () => {
	isImportProduct.value = true;
	GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
	SearchProcutDialog.value = true;
}

const onAddquotationProductItem = () => {
	isImportProduct.value = false;
	productData.value.push({
		productNum: '',
		customerNum: '',
		cproductname: '',
		cspecification: '',
		quotationnum: 0,
		exportunitprice: 0,
		exporttotalprice: 0,
		unitofmeasurement: '',
		purchaseinquiry: 0,
		purchaseunitprice: 0,
		onepacking: 0,
		invoice: '',
		packaging: '',
		specialrequirements: '',
		rebaterate: 0,
		outerboxunit: '',
		outerboxlength: 0,
		outerboxwidth: 0,
		outerboxheight: 0,
		outerboxnetweight: 0,
		outerboxgrossweight: 0,
		outerboxvolume: 0,
		NumberOfBoxes: 0,
		OtherFees: 0,
		additionalpackagingcosts: 0,
		singleProductGrossProfit: 0,
		grossProfitRate: 0,
		totalNetWeight: 0,
		totalGrossWeight: 0,
		totalVolume: 0,
		innerBoxLoading: 0,
		outerboxloading: 0,
		inlandfreightprice: 0,
		isImported: false
	});
}

const handleCloseSearchProcutDialog = () => {
	searchProductNameText.value = '';
}

function formatNumber(row, key) {
	if (row[key] !== null && row[key] !== undefined) {
		row[key] = parseFloat(row[key]).toFixed(1);
	}
}


function formatNumber2(row, key) {
	if (row[key] !== null && row[key] !== undefined) {
		row[key] = parseFloat(row[key]).toFixed(2);
	}
}
/// 外销币种Change事件
///如果为人民币，汇率默认为1
const currencySymbol = ref('$'); // 默認貨幣符號
const exportcurrencyChange = (value) => {
	if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == value).map(item => item.dictValue).values().next().value == 3) {
		quotationDialogform.exchangerate = 1;
		currencySymbol.value = '￥';
	} else if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == value).map(item => item.dictValue).values().next().value == 2) {
		currencySymbol.value = '€';
	} else if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == value).map(item => item.dictValue).values().next().value == 1) {
		currencySymbol.value = '＄';
	} else {
		quotationDialogform.exchangerate = null;
	}
}
const shippingcurrencyChange = (value) => {
	if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == value).map(item => item.dictValue).values().next().value == 3) {
		quotationDialogform.shippingrate = 1;
	} else {
		quotationDialogform.shippingrate = null;
	}
}
///计算外销总价、计算外箱体积、计算总货值
const calculateTotal = () => {
	quotationDialogform.TotalValueOfGoods = 0;
	quotationDialogform.TotalQuantity = 0;
	quotationDialogform.TotalNumberOfBoxes = 0;
	quotationDialogform.TotalGrossWeight = 0;
	quotationDialogform.TotalNetWeight = 0;
	quotationDialogform.TotalVolume = 0;
	quotationDialogform.TotalPurchases = 0;
	quotationDialogform.TotalOtherFees = 0;
	quotationDialogform.TotalTaxRefund = 0;
	quotationDialogform.ProfitAmount = 0;
	quotationDialogform.Totalgrossprofit = 0;
	quotationDialogform.Totalprofitmargin = 0;

	let TotalvalueOfGoods = 0;
	let TotalQuantity = 0;
	let TotalNumberOfBoxes = 0;
	let TotalGrossWeight = 0;
	let TotalNetWeight = 0;
	let TotalVolume = 0;
	let TotalPurchases = 0;
	let OtherFees = 0;
	let TotalTaxRefund = 0;
	let ProfitAmount = 0;
	let TotalSinglesalesrevenueA = 0; // 总和单个销售收入A
	let Totalgrossprofit = 0; // 总毛利合计
	let Totalprofitmargin = 0; // 总利润率
	let TotalOtherFees = 0; // 其它费用合计

	productData.value.forEach((item) => {
		// 计算退税率相关的变量，避免退税率为0导致除以零
		const effectiveRebate = Number(item.rebaterate) || 0; // 退税率百分比，如13代表13%
		// 当退税率为0时，分母直接取1，否则取 (1+rebate/100)
		const rebateDivisor = effectiveRebate === 0 ? 1 : (1 + effectiveRebate / 100);

		// 箱数 = 报价数量 / 外箱装量
		item.NumberOfBoxes = (item.quotationnum / item.outerboxloading);
		item.NumberOfBoxes = Number(item.NumberOfBoxes.toFixed(1)); // 保留一位小数

		// 外箱体积 = (外箱长度 x 外箱宽度 x 外箱高度) / 1000000
		item.outerboxvolume = (item.outerboxlength * item.outerboxwidth * item.outerboxheight / 1000000);
		item.outerboxvolume = item.outerboxvolume.toFixed(4); // 保留4位小数

		// 单个产品体积 = 外箱体积 / 外箱装量
		item.Singleproductvolume = (item.outerboxvolume / item.outerboxloading).toFixed(6).toString().replace(/(\.\d*?[1-9])0+$/, '$1');

		// 单个产品的港杂费 = 港杂费 x 单个产品体积
		item.Portchargesforindividualproducts = (Number(quotationDialogform.portMiscellaneousFees) * Number(item.Singleproductvolume)).toFixed(3);
		console.log('单个产品的港杂费：' + item.Portchargesforindividualproducts);

		// 单个产品海运费 = 海运费 x 单个产品体积 (如果非人民币，还需乘以汇率)
		if (state.optionss['hr_export_currency']
			.filter(hr_export_currency => hr_export_currency.dictValue == quotationDialogform.shippingcurrency)
			.map(i => i.dictLabel).values().next().value == '人民币') {
			item.Oceanfreightforasingleproduct = (Number(quotationDialogform.oceanFreight) * item.Singleproductvolume).toFixed(3);
		} else {
			item.Oceanfreightforasingleproduct = (Number(quotationDialogform.oceanFreight) * item.Singleproductvolume * Number(quotationDialogform.shippingrate)).toFixed(3);
		}

		// 单个产品内陆运费 = 内陆运费 x 单个产品体积
		item.Inlandfreightforasingleproduct = (Number(item.inlandfreightprice) * item.Singleproductvolume).toFixed(3);

		// 总净重 = 外箱净重 x 箱数
		item.totalNetWeight = isNaN(item.outerboxnetweight * item.NumberOfBoxes) ? 0.000 : (item.outerboxnetweight * item.NumberOfBoxes).toFixed(1);
		// 总毛重 = 外箱毛重 x 箱数
		item.totalGrossWeight = isNaN(item.outerboxgrossweight * item.NumberOfBoxes) ? 0.000 : (item.outerboxgrossweight * item.NumberOfBoxes).toFixed(1);
		// 总体积 = 外箱体积 x 箱数
		item.totalVolume = isNaN(item.outerboxvolume * item.NumberOfBoxes) ? 0.000 : (item.outerboxvolume * item.NumberOfBoxes).toFixed(3);


		TotalSinglesalesrevenueA += Number(item.SinglesalesrevenueA * item.quotationnum);
		TotalQuantity += Number(item.quotationnum);
		TotalNumberOfBoxes += Number(item.NumberOfBoxes.toFixed(1));
		TotalGrossWeight += Number(item.totalGrossWeight);
		TotalNetWeight += Number(item.totalNetWeight);
		TotalVolume += Number(item.totalVolume);
		TotalPurchases += Number(item.purchaseunitprice) * Number(item.quotationnum);
		OtherFees += Number(item.OtherFees);
		// 退税总额 = (采购单价 / rebateDivisor * 0.13 * 报价数量)
		TotalTaxRefund += Number((item.purchaseunitprice / rebateDivisor * 0.13 * item.quotationnum).toFixed(3));
		ProfitAmount += Number(item.ProfitAmount);
		// 总毛利合计 = 单个产品毛利 x 报价数量 - 其他费用
		Totalgrossprofit += Number(item.singleProductGrossProfit * item.quotationnum - item.OtherFees);
		console.log('总毛利合计' + item.singleProductGrossProfit + '/' + item.quotationnum + '=' + Totalgrossprofit);
		// 总利润率 = 总毛利合计 / 单个销售收入A总和 x 100
		Totalprofitmargin = Number((Totalgrossprofit / TotalSinglesalesrevenueA * 100).toFixed(3));
		console.log('总利润率' + Totalgrossprofit + '/' + TotalSinglesalesrevenueA + '=' + Totalprofitmargin);

		//#region 其它费用合计计算
		// 海运费合计 = 单个产品海运费 * 运费汇率 * 报价数量
		let oceanFreightTotal = item.Oceanfreightforasingleproduct * quotationDialogform.shippingrate * item.quotationnum;
		console.log('海运费合计：' + oceanFreightTotal);
		// 内陆运费合计 = 单个产品内陆运费 * 报价数量
		let inlandAndPortChargesTotal = Number(item.Inlandfreightforasingleproduct * item.quotationnum);
		console.log('内陆运费合计：' + inlandAndPortChargesTotal);
		let PortchargesforindividualproductsTotal = Number(item.Portchargesforindividualproducts * item.quotationnum);
		console.log('港杂费合计：' + PortchargesforindividualproductsTotal);
		// 银行费用合计 = 银行费用 * 汇率
		let bankFeeTotal = (quotationDialogform.BankFee * quotationDialogform.exchangerate);
		console.log('银行费用合计：' + bankFeeTotal);
		// 文件费用
		let documentationFees = quotationDialogform.DocumentationFees;
		console.log('文件费用：' + documentationFees);
		// 其它费用合计 = 海运费合计 + 内陆运费 + 港杂费合计 + 银行费用合计 + 文件费用 + 其它费用
		console.log('其它费用：' + OtherFees);
		TotalOtherFees = Number(oceanFreightTotal) + Number(inlandAndPortChargesTotal) + Number(PortchargesforindividualproductsTotal) + Number(bankFeeTotal) + Number(documentationFees) + Number(OtherFees);
		console.log('其它费用合计：' + TotalOtherFees);

		//计算外销单价
		// 计算成本价
		//成本价=采购单价+单个产品额外包装费用+单个产品其它费用+单个产品内陆运费+单个产品港杂费+（每立方米海运费*（外箱体积/外箱装量）*海运费汇率）
		const costPrice = Number(item.purchaseunitprice) +
			Number(item.additionalpackagingcosts) +
			Number(item.OtherFees) +
			Number(item.Inlandfreightforasingleproduct) +
			Number(item.Portchargesforindividualproducts) +
			(Number(quotationDialogform.oceanFreight) *
				(Number(item.outerboxvolume) / Number(item.outerboxloading)) *
				Number(quotationDialogform.shippingrate));
		console.log('成本价：' + costPrice.toFixed(3));
		// 数据验证
		if (!item.purchaseunitprice || !item.outerboxloading || !quotationDialogform.exchangerate) {
			return;
		}
		if (item.outerboxloading <= 0) {
			return;
		}
		if (item.ProfitMargin < 0) {
			return;
		} else if (item.ProfitMargin >= 100) {
			ElMessage.warning('利润率不能大于100%');
			item.ProfitMargin = 99.9;
			return;
		}
		let exportPrice;
		const profitRateDecimal = item.ProfitMargin / 100;
		if (profitRateDecimal < 1) {
			// 正常情况的计算公式
			exportPrice = (costPrice - (item.purchaseunitprice / 1.13 * ((item.rebaterate / 100) * (1 - profitRateDecimal)))) /
				(quotationDialogform.exchangerate * (1 - profitRateDecimal));
			// 保留三位小数
			item.exportunitprice = Number(exportPrice.toFixed(3));
			console.log('外销单价：' + item.exportunitprice);
		} else {
			// 当利润率 >= 100% 时，可以使用一个较大的基数来计算
			item.exportunitprice = 0;
			return;
		}
		// 外销总价 = 报价数量 x 报价单价
		item.exporttotalprice = Number(item.quotationnum * item.exportunitprice).toFixed(2);
		// 单个销售收入A = (采购单价 / rebateDivisor * (rebate/100) + 销售单价 x 汇率)
		item.SinglesalesrevenueA = (item.purchaseunitprice / rebateDivisor * (effectiveRebate / 100) + item.exportunitprice * Number(quotationDialogform.exchangerate));
		item.SinglesalesrevenueA = item.SinglesalesrevenueA.toFixed(3);

		// 计算单个产品毛利
		if (isNaN(item.SinglesalesrevenueA - item.Portchargesforindividualproducts - item.Inlandfreightforasingleproduct - item.Oceanfreightforasingleproduct - item.purchaseunitprice - item.additionalpackagingcosts - item.OtherFees)) {
			item.singleProductGrossProfit = 0.000;
		} else {
			// 单个产品毛利 = 单个销售收入A - 港杂费 - 内陆运费 - 海运费 - 采购价 - 额外包装费用 - 其他费用
			item.singleProductGrossProfit = (item.SinglesalesrevenueA - item.Portchargesforindividualproducts - item.Inlandfreightforasingleproduct - item.Oceanfreightforasingleproduct - item.purchaseunitprice - item.additionalpackagingcosts - item.OtherFees).toFixed(3);
			// 单个产品毛利合计 = 单个产品毛利 x 报价数量
			item.singleProductGrossProfitTotal = (item.singleProductGrossProfit * item.quotationnum).toFixed(3);
		}

		if (isNaN(item.singleProductGrossProfit / item.SinglesalesrevenueA)) {
			item.grossProfitRate = 0.000;
		} else {
			// 毛利率 = 单个产品毛利 / 单个销售收入A x 100
			item.grossProfitRate = (item.singleProductGrossProfit / item.SinglesalesrevenueA * 100).toFixed(3);
		}

		TotalvalueOfGoods += Number(item.exporttotalprice);
	});

	quotationDialogform.TotalValueOfGoods = TotalvalueOfGoods || 0;
	quotationDialogform.TotalQuantity = TotalQuantity || 0;
	quotationDialogform.TotalNumberOfBoxes = TotalNumberOfBoxes || 0;
	quotationDialogform.TotalGrossWeight = TotalGrossWeight || 0;
	quotationDialogform.TotalNetWeight = TotalNetWeight || 0;
	quotationDialogform.TotalVolume = Number(TotalVolume.toFixed(2)) || 0;
	quotationDialogform.TotalPurchases = TotalPurchases || 0;
	quotationDialogform.TotalOtherFees = TotalOtherFees || 0;
	quotationDialogform.TotalTaxRefund = TotalTaxRefund || 0;
	// 利润金额 = 货值合计 + 退税总额 - 采购合计 - 其它费用
	quotationDialogform.ProfitAmount = Number(Number((TotalvalueOfGoods * quotationDialogform.exchangerate + TotalTaxRefund) - Number(TotalPurchases) - Number(TotalOtherFees)).toFixed(2)) || 0;
	quotationDialogform.Totalgrossprofit = Totalgrossprofit || 0;
	quotationDialogform.Totalprofitmargin = Totalprofitmargin || 0;
};

const addQuotationRequest = reactive({
	id: 0,
	quotationNum: '',
	inquiryDate: '',
	realQuotationDate: '',
	validityPeriod: null,
	quotationStatus: null,
	customerid: null,
	customerNum: '',
	customerName: '',
	contactPerson: null,
	contactPersonEmail: '',
	customerLevel: null,
	ourCompany: null,
	settlementCategory: null,
	exportCurrency: null,
	exchangeRate: null,
	settlementWay: null,
	uniformProfitMargin: null,
	pricingTerm: null,
	shippingPort: null,
	destinationPort: null,
	tradingCountry: null,
	transportationMethod: null,
	shippingCurrency: null,
	shippingRate: null,
	unitFreight: null,
	commissionRate: null,
	seller: null,
	createBy: '',
	createTime: '',
	updateBy: '',
	updateTime: '',
	isDelete: null,
	remark: '',
	totalValueOfGoods: null,
	totalQuantity: null,
	totalNumberOfBoxes: null,
	totalGrossWeight: null,
	totalNetWeight: null,
	totalVolume: null,
	totalPurchases: null,
	totalOtherFees: null,
	totalTaxRefund: null,
	profitAmount: null,
	portMiscellaneousFees: null,
	inlandFreight: null,
	oceanFreight: null,
	singleCubicCost: null,
	freightForwarderCustomsClearanceFees: null,
	totalgrossprofit: null,
	totalprofitmargin: null,
	quotationProductDetailsList: [],
	bankfee: null,
	documentationfees: null,
	isDraft: null
});
const AddQuotation = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			addQuotationRequest.customerid = parseInt(quotationDialogform.customernum);
			addQuotationRequest.quotationProductDetailsList = [];
			addQuotationRequest.quotationNum = quotationDialogform.quotationnum;
			addQuotationRequest.inquiryDate = quotationDialogform.inquirydate;
			addQuotationRequest.realQuotationDate = quotationDialogform.realquotationdate;
			addQuotationRequest.validityPeriod = quotationDialogform.validityperiod;
			addQuotationRequest.quotationStatus = quotationDialogform.quorationstatus;
			addQuotationRequest.customerNum = state.optionss['sql_hr_customer'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value;
			addQuotationRequest.customerName = state.optionss['sql_hr_customer_name'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value;
			addQuotationRequest.settlementWay = quotationDialogform.settlementway;
			addQuotationRequest.contactPerson = quotationDialogform.contactperson;
			addQuotationRequest.contactPersonEmail = quotationDialogform.contactpersonEmail;
			addQuotationRequest.customerLevel = quotationDialogform.customerlevel;
			addQuotationRequest.ourCompany = quotationDialogform.ourcompany;
			addQuotationRequest.settlementCategory = quotationDialogform.settlementcategory;
			addQuotationRequest.exportCurrency = quotationDialogform.exportcurrency;
			addQuotationRequest.exchangeRate = quotationDialogform.exchangerate;
			addQuotationRequest.uniformProfitMargin = quotationDialogform.uniformprofitmargin;
			addQuotationRequest.pricingTerm = quotationDialogform.pricingterm;
			addQuotationRequest.shippingPort = quotationDialogform.shippingport;
			addQuotationRequest.destinationPort = quotationDialogform.destinationport;
			addQuotationRequest.tradingCountry = quotationDialogform.tradingcountry;
			addQuotationRequest.transportationMethod = quotationDialogform.transportationmethod;
			addQuotationRequest.shippingCurrency = quotationDialogform.shippingcurrency;
			addQuotationRequest.shippingRate = quotationDialogform.shippingrate;
			addQuotationRequest.unitFreight = quotationDialogform.unitfreight;
			addQuotationRequest.commissionRate = quotationDialogform.commissionrate;
			addQuotationRequest.seller = quotationDialogform.seller;
			addQuotationRequest.remark = quotationDialogform.Remark ?? '';
			addQuotationRequest.totalValueOfGoods = quotationDialogform.TotalValueOfGoods;
			addQuotationRequest.totalQuantity = quotationDialogform.TotalQuantity;
			addQuotationRequest.totalNumberOfBoxes = quotationDialogform.TotalNumberOfBoxes.toFixed(1);
			addQuotationRequest.totalGrossWeight = quotationDialogform.TotalGrossWeight;
			addQuotationRequest.totalNetWeight = quotationDialogform.TotalNetWeight;
			addQuotationRequest.totalVolume = quotationDialogform.TotalVolume;
			addQuotationRequest.totalPurchases = quotationDialogform.TotalPurchases;
			addQuotationRequest.totalOtherFees = quotationDialogform.TotalOtherFees;
			addQuotationRequest.totalTaxRefund = quotationDialogform.TotalTaxRefund;
			addQuotationRequest.profitAmount = quotationDialogform.ProfitAmount;
			addQuotationRequest.portMiscellaneousFees = quotationDialogform.portMiscellaneousFees;
			addQuotationRequest.inlandFreight = quotationDialogform.inlandFreight;
			addQuotationRequest.oceanFreight = quotationDialogform.oceanFreight;
			addQuotationRequest.singleCubicCost = quotationDialogform.singleCubicCost;
			addQuotationRequest.freightForwarderCustomsClearanceFees = quotationDialogform.freightForwarderCustomsClearanceFees;
			addQuotationRequest.totalgrossprofit = quotationDialogform.Totalgrossprofit;
			addQuotationRequest.totalprofitmargin = quotationDialogform.Totalprofitmargin;
			addQuotationRequest.bankfee = quotationDialogform.BankFee;
			addQuotationRequest.documentationfees = quotationDialogform.DocumentationFees;
			addQuotationRequest.quotationProductDetailsList = [];
			productData.value.forEach(item => {
				addQuotationRequest.quotationProductDetailsList.push({
					productNum: item.productNum,
					customerNum: item.customerNum,
					cproductname: item.cproductname,
					cspecification: item.cspecification,
					quotationnum: item.quotationnum,
					exportunitprice: item.exportunitprice,
					exporttotalprice: item.exporttotalprice,
					unitofmeasurement: item.unitofmeasurement,
					purchasecurrency: item.purchasecurrency,
					purchaseunitprice: item.purchaseunitprice,
					onepacking: item.additionalPackagingCosts,
					Invoice: item.isInvoicingc,
					packaging: item.packaging,
					specialrequirements: item.specialrequirements,
					rebaterate: item.rebaterate,
					innerBoxLoading: item.innerBoxLoading,
					outerboxloading: item.outerboxloading,
					outerboxunit: item.outerboxunit,
					outerboxlength: item.outerboxlength,
					outerboxwidth: item.outerboxwidth,
					outerboxheight: item.outerboxheight,
					outerboxnetweight: item.outerboxnetweight,
					outerboxgrossweight: item.outerboxgrossweight,
					outerboxvolume: item.outerboxvolume,
					NumberOfBoxes: item.NumberOfBoxes,
					OtherFees: item.OtherFees,
					additionalpackagingcosts: item.additionalpackagingcosts,
					singleProductGrossProfit: item.singleProductGrossProfit,
					singleProductGrossProfitTotal: item.singleProductGrossProfitTotal,
					grossProfitRate: item.grossProfitRate,
					totalNetWeight: item.totalNetWeight,
					totalGrossWeight: item.totalGrossWeight,
					totalVolume: item.totalVolume,
					Singlesalesrevenue: item.SinglesalesrevenueA,
					Singleproductvolume: item.Singleproductvolume,
					Portchargesforindividualproducts: item.Portchargesforindividualproducts,
					Oceanfreightforasingleproduct: item.Oceanfreightforasingleproduct,
					Inlandfreightforasingleproduct: item.Inlandfreightforasingleproduct,
					inlandfreightprice: item.inlandfreightprice,
					IsNewProduct: item.isImported == true ? 0 : 1
				});
			});
			request.post('Quotation/AddQuotation/Add', addQuotationRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: '报价单提交成功',
						type: 'success'
					})
					if (response.data > 0) {
						GetQuotationDetailsList(response.data);
					}
					EditQuotationId.value = response.data;
					isDisabled.value = true;
					isSaveBtnShow.value = false;
					isReviewBtnShow.value = false;
					showEditBtn.value = false;
					GetQuotationList(currentPage.value, pageSize.value);
				} else {
					console.error('新增报价单出错');
				}
			}).catch(error => {
				console.error('新增报价单出错！😔错误内容：', error);
			});
		} else {
			// 获取第一个验证失败的字段
			const errorFields = Object.keys(fields);
			if (errorFields.length > 0) {
				const firstErrorField = errorFields[0];
				const errorElement = document.querySelector(`[data-field="${firstErrorField}"]`);
				if (errorElement) {
					// 获取元素的位置信息
					const elementRect = errorElement.getBoundingClientRect();
					const isInViewport = (
						elementRect.top >= 0 &&
						elementRect.left >= 0 &&
						elementRect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
						elementRect.right <= (window.innerWidth || document.documentElement.clientWidth)
					);

					// 如果元素不在视口中，则滚动到该元素
					if (!isInViewport) {
						errorElement.scrollIntoView({
							behavior: 'smooth',
							block: 'center',
							inline: 'nearest'
						});
					}

					// 添加高亮效果
					errorElement.classList.add('highlight-error');

					// 2秒后移除高亮效果
					setTimeout(() => {
						errorElement.classList.remove('highlight-error');
					}, 2000);
				}
			}
		}
	});
}

const SaveDraft = async () => {
	// 设置默认值
	addQuotationRequest.customerid = parseInt(quotationDialogform.customernum) || 0;
	addQuotationRequest.quotationProductDetailsList = [];
	addQuotationRequest.quotationNum = quotationDialogform.quotationnum || '';
	addQuotationRequest.inquiryDate = quotationDialogform.inquirydate || new Date().toISOString().split('T')[0];
	addQuotationRequest.realQuotationDate = quotationDialogform.realquotationdate || new Date().toISOString().split('T')[0];
	addQuotationRequest.validityPeriod = quotationDialogform.validityperiod || 30;
	addQuotationRequest.quotationStatus = quotationDialogform.quorationstatus || 0;
	addQuotationRequest.customerNum = quotationDialogform.customernum ? state.optionss['sql_hr_customer'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value : '';
	addQuotationRequest.customerName = quotationDialogform.customername ? state.optionss['sql_hr_customer_name'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value : '';
	addQuotationRequest.settlementWay = quotationDialogform.settlementway || 0;
	addQuotationRequest.contactPerson = quotationDialogform.contactperson || 0;
	addQuotationRequest.contactPersonEmail = quotationDialogform.contactpersonEmail || '';
	addQuotationRequest.customerLevel = quotationDialogform.customerlevel || 0;
	addQuotationRequest.ourCompany = quotationDialogform.ourcompany || 0;
	addQuotationRequest.settlementCategory = quotationDialogform.settlementcategory || 0;
	addQuotationRequest.exportCurrency = quotationDialogform.exportcurrency || 0;
	addQuotationRequest.exchangeRate = quotationDialogform.exchangerate || 1;
	addQuotationRequest.uniformProfitMargin = quotationDialogform.uniformprofitmargin || 0;
	addQuotationRequest.pricingTerm = quotationDialogform.pricingterm || 0;
	addQuotationRequest.shippingPort = quotationDialogform.shippingport || 0;
	addQuotationRequest.destinationPort = quotationDialogform.destinationport || '';
	addQuotationRequest.tradingCountry = quotationDialogform.tradingcountry || 0;
	addQuotationRequest.transportationMethod = quotationDialogform.transportationmethod || 0;
	addQuotationRequest.shippingCurrency = quotationDialogform.shippingcurrency || 0;
	addQuotationRequest.shippingRate = quotationDialogform.shippingrate || 1;
	addQuotationRequest.unitFreight = quotationDialogform.unitfreight || 0;
	addQuotationRequest.commissionRate = quotationDialogform.commissionrate || 0;
	addQuotationRequest.seller = quotationDialogform.seller || 0;
	addQuotationRequest.remark = quotationDialogform.Remark || '';
	addQuotationRequest.totalValueOfGoods = quotationDialogform.TotalValueOfGoods || 0;
	addQuotationRequest.totalQuantity = quotationDialogform.TotalQuantity || 0;
	addQuotationRequest.totalNumberOfBoxes = quotationDialogform.TotalNumberOfBoxes ? Number(quotationDialogform.TotalNumberOfBoxes).toFixed(1) : '0.0';
	addQuotationRequest.totalGrossWeight = quotationDialogform.TotalGrossWeight || 0;
	addQuotationRequest.totalNetWeight = quotationDialogform.TotalNetWeight || 0;
	addQuotationRequest.totalVolume = quotationDialogform.TotalVolume || 0;
	addQuotationRequest.totalPurchases = quotationDialogform.TotalPurchases || 0;
	addQuotationRequest.totalOtherFees = quotationDialogform.TotalOtherFees || 0;
	addQuotationRequest.totalTaxRefund = quotationDialogform.TotalTaxRefund || 0;
	addQuotationRequest.profitAmount = quotationDialogform.ProfitAmount || 0;
	addQuotationRequest.portMiscellaneousFees = quotationDialogform.portMiscellaneousFees || 0;
	addQuotationRequest.inlandFreight = quotationDialogform.inlandFreight || 0;
	addQuotationRequest.oceanFreight = quotationDialogform.oceanFreight || 0;
	addQuotationRequest.singleCubicCost = quotationDialogform.singleCubicCost || 0;
	addQuotationRequest.freightForwarderCustomsClearanceFees = quotationDialogform.freightForwarderCustomsClearanceFees || 0;
	addQuotationRequest.totalgrossprofit = quotationDialogform.Totalgrossprofit || 0;
	addQuotationRequest.totalprofitmargin = quotationDialogform.Totalprofitmargin || 0;
	addQuotationRequest.bankfee = quotationDialogform.BankFee || 0;
	addQuotationRequest.documentationfees = quotationDialogform.DocumentationFees || 0;
	addQuotationRequest.isDraft = 1; // 设置为草稿状态
	productData.value.forEach((item) => {
		addQuotationRequest.quotationProductDetailsList.push({
			id: item.id || 0,
			productNum: item.productNum || '',
			customerNum: item.customerNum || '',
			cproductname: item.cproductname || '',
			cspecification: item.cspecification || '',
			quotationnum: item.quotationnum || 0,
			exportunitprice: item.exportunitprice || 0,
			exporttotalprice: item.exporttotalprice || 0,
			unitofmeasurement: item.unitofmeasurement || 0,
			purchasecurrency: item.purchasecurrency || 0,
			purchaseunitprice: item.purchaseunitprice || 0,
			onepacking: item.additionalPackagingCosts || 0,
			Invoice: item.isInvoicingc || 0,
			packaging: item.packaging || '',
			specialrequirements: item.specialrequirements || '',
			rebaterate: item.rebaterate || 0,
			innerBoxLoading: item.innerBoxLoading || 0,
			outerboxloading: item.outerboxloading || 0,
			outerboxunit: item.outerboxunit || 0,
			outerboxlength: item.outerboxlength || 0,
			outerboxwidth: item.outerboxwidth || 0,
			outerboxheight: item.outerboxheight || 0,
			outerboxnetweight: item.outerboxnetweight || 0,
			outerboxgrossweight: item.outerboxgrossweight || 0,
			outerboxvolume: item.outerboxvolume || 0,
			NumberOfBoxes: item.NumberOfBoxes || 0,
			OtherFees: item.OtherFees || 0,
			additionalpackagingcosts: item.additionalpackagingcosts || 0,
			singleProductGrossProfit: item.singleProductGrossProfit || 0,
			singleProductGrossProfitTotal: item.singleProductGrossProfitTotal || 0,
			grossProfitRate: item.grossProfitRate || 0,
			totalNetWeight: item.totalNetWeight || 0,
			totalGrossWeight: item.totalGrossWeight || 0,
			totalVolume: item.totalVolume || 0,
			Singlesalesrevenue: item.SinglesalesrevenueA || 0,
			Singleproductvolume: item.Singleproductvolume || 0,
			Portchargesforindividualproducts: item.Portchargesforindividualproducts || 0,
			Oceanfreightforasingleproduct: item.Oceanfreightforasingleproduct || 0,
			Inlandfreightforasingleproduct: item.Inlandfreightforasingleproduct || 0,
			inlandfreightprice: item.inlandfreightprice || 0,
			IsNewProduct: item.isImported == true ? 0 : 1,
			ProfitMargin: item.ProfitMargin || 0
		});
	});

	// 如果是查看详情打开的对话框，使用修改接口
	if (isViewDetails.value) {
		addQuotationRequest.id = EditQuotationId.value;
		request.post('Quotation/EditQuotation/Edit', addQuotationRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '修改草稿成功！',
					type: 'success'
				})
				if (response.data > 0) {
					showSaveDraftBtn.value = false;
					addQuotationRequest.quotationProductDetailsList = [];
					GetQuotationDetailsList(response.data);
				}
				isDisabled.value = true;
				isSaveBtnShow.value = false;
				isReviewBtnShow.value = true;
				showEditBtn.value = true;
				GetQuotationList(currentPage.value, pageSize.value);
			} else {
				ElMessage.error('修改草稿失败');
			}
		}).catch(error => {
			console.error('修改草稿出错！😔错误内容：', error);
			ElMessage.error('修改草稿失败，请稍后重试');
		});
	} else {
		// 新建时使用新增接口
		request.post('Quotation/AddQuotation/Add', addQuotationRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: '保存草稿成功！',
					type: 'success'
				})
				if (response.data > 0) {
					addQuotationRequest.quotationProductDetailsList = [];
					showSaveDraftBtn.value = false;
					EditQuotationId.value = response.data;
					isViewDetails.value = true;
					GetQuotationDetailsList(response.data);
				}
				EditQuotationId.value = response.data;
				isDisabled.value = true;
				isSaveBtnShow.value = false;
				isReviewBtnShow.value = true;
				showEditBtn.value = true;
				GetQuotationList(currentPage.value, pageSize.value);
			} else {
				ElMessage.error('保存草稿失败');
			}
		}).catch(error => {
			console.error('保存草稿出错！😔错误内容：', error);
			ElMessage.error('保存草稿失败，请稍后重试');
		});
	}
}

///报价单列表
const quotationData = ref([])
//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
//产品信息表格
const ProductInfoTableData = ref([])
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetQuotationList(start, end);
};
function GetQuotationList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'Quotation/GetQuotaionList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				quotationnum: SearchQuotationNum.value,
				customerid: SechaerCustomerSelect.value,
				inquiryDate: SearchInquiryDate.value,
				realQuotationDate: SearchRealQuotationDate.value
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				quotationData.value = response.data.result.map(item => ({
					...item,
					inquiryDate: item.inquiryDate?.split('T')[0] || '',
					realQuotationDate: item.realQuotationDate?.split('T')[0] || ''
				}));
				quotationData.value.forEach((item) => {
					item.quotationStatus = state.optionss['hr_quotation_status'].filter(hr_quotation_status => hr_quotation_status.dictValue == item.quotationStatus).map(item => item.dictLabel).values().next().value;
					item.createBy = optionss.value.sql_all_user.find(sql_all_user => sql_all_user.dictValue === item.createBy)?.dictLabel;
				});
				resolve(response.data.result);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					quotationData.value = [];
				}
			}
		}).catch(error => {
			console.error(error);
		});
	});
}

//#region 查看报价单详情
const EditQuotationId = ref(0);
const ChcekDetails = async (row) => {
	// if (row.quotationStatus == '待审核') {
	// 	isReviewBtnShow.value = true;
	// } else {
	// 	isReviewBtnShow.value = false;
	// }
	isDisabled.value = true;
	isViewDetails.value = true; // 查看详情时设置为true
	if (row.isDraft == 0) {
		showEditBtn.value = false;
		showEditSaveBtn.value = false;
		isSaveBtnShow.value = false;
		showSaveDraftBtn.value = false;
		handleCustomerSelection(row.customerid);
	} else {
		showEditBtn.value = true;
		showEditSaveBtn.value = true;
		isSaveBtnShow.value = false;
		showSaveDraftBtn.value = false;
	}
	EditQuotationId.value = row.id;
	if (row.customerid != 0 && row.customerid != null) {
		quotationDialogform.customername = state.optionss['sql_hr_customer_abbreviation'].find(item => item.dictValue == row.customerid)?.dictValue;
		handleCustomerSelection(row.customerid);
	}
	quotationDialogform.contactperson = row.contactPerson;
	quotationDialogform.quotationnum = row.quotationNum;
	quotationDialogform.inquirydate = row.inquiryDate;
	quotationDialogform.realquotationdate = row.realQuotationDate;
	quotationDialogform.validityperiod = row.validityPeriod;
	quotationDialogform.quorationstatus = optionss.value.hr_quotation_status.find(item => item.dictLabel === row.quotationStatus)?.dictValue || '0';
	quotationDialogform.contactpersonEmail = row.contactPersonEmail;
	quotationDialogform.customerlevel = state.optionss.hr_customer_level.find(item => item.dictValue == row.customerLevel)?.dictValue;
	quotationDialogform.ourcompany = state.optionss.hr_ourcompany.find(item => item.dictValue == row.ourCompany)?.dictValue;
	quotationDialogform.settlementcategory = state.optionss.hr_settlementcategory.find(item => item.dictValue == row.settlementCategory)?.dictValue;
	quotationDialogform.exportcurrency = state.optionss.hr_export_currency.find(item => item.dictValue == row.exportCurrency)?.dictValue;
	quotationDialogform.exchangerate = row.exchangeRate;
	quotationDialogform.uniformprofitmargin = row.uniformProfitMargin;
	quotationDialogform.settlementway = state.optionss.hr_settlement_way.find(item => item.dictValue == row.settlementWay)?.dictValue;
	quotationDialogform.pricingterm = state.optionss.hr_pricing_term.find(item => item.dictValue == row.pricingTerm)?.dictValue;
	quotationDialogform.shippingport = state.optionss.hr_transport_port.find(item => item.dictValue == row.shippingPort)?.dictValue;
	quotationDialogform.destinationport = row.destinationPort;
	quotationDialogform.tradingcountry = state.optionss.hr_nation.find(item => item.dictValue == row.tradingCountry)?.dictValue;
	quotationDialogform.transportationmethod = state.optionss.hr_transportation_method.find(item => item.dictValue == row.transportationMethod)?.dictValue;
	quotationDialogform.shippingcurrency = state.optionss.hr_export_currency.find(item => item.dictValue == row.shippingCurrency)?.dictValue;
	quotationDialogform.shippingrate = row.shippingRate;
	quotationDialogform.unitfreight = row.unitFreight;
	quotationDialogform.commissionrate = row.commissionRate;
	quotationDialogform.seller = state.optionss.sql_all_user.find(item => item.dictValue == row.seller)?.dictValue;
	quotationDialogform.TotalValueOfGoods = row.totalValueOfGoods;
	quotationDialogform.TotalQuantity = row.totalQuantity;
	quotationDialogform.TotalNumberOfBoxes = row.totalNumberOfBoxes.toFixed(1);
	quotationDialogform.TotalGrossWeight = row.totalGrossWeight;
	quotationDialogform.TotalNetWeight = row.totalNetWeight;
	quotationDialogform.TotalVolume = row.totalVolume;
	quotationDialogform.TotalPurchases = row.totalPurchases;
	quotationDialogform.TotalOtherFees = row.totalOtherFees;
	quotationDialogform.TotalTaxRefund = row.totalTaxRefund;
	quotationDialogform.ProfitAmount = row.profitAmount;
	quotationDialogform.Remark = row.remark ?? '';
	quotationDialogform.portMiscellaneousFees = row.portMiscellaneousFees;
	quotationDialogform.inlandFreight = row.inlandFreight;
	quotationDialogform.oceanFreight = row.oceanFreight;
	quotationDialogform.singleCubicCost = row.singleCubicCost;
	quotationDialogform.freightForwarderCustomsClearanceFees = row.freightForwarderCustomsClearanceFees;
	quotationDialogform.Totalgrossprofit = row.totalgrossprofit;
	quotationDialogform.Totalprofitmargin = row.totalprofitmargin;
	quotationDialogform.BankFee = row.bankFee;
	quotationDialogform.DocumentationFees = row.documentationFees;
	GetQuotationDetailsList(row.id);
	quotationDialog.value = true;
}
//获取报价单产品列表
const GetQuotationDetailsList = (ID) => {
	request({
		url: 'Quotation/GetQuotationDetailsListByQuotationID/GetQuotationDetailsList',
		method: 'GET',
		params: {
			ID: ID
		}
	}).then(response => {
		if (response.data.length > 0) {
			productData.value = [];
			response.data.forEach(element => {
				productData.value.push
					({
						id: element.id,
						productNum: element.productNum,
						customerNum: element.customerNum,
						cproductname: element.cProductName,
						cspecification: element.cSpecification,
						quotationnum: element.quotationNum,
						exportunitprice: element.exportUnitPrice,
						exporttotalprice: element.exportTotalPrice,
						unitofmeasurement: state.optionss["hr_calculate_unit"].filter(hr_calculate_unit => hr_calculate_unit.dictValue == element.unitOfMeasurement).map(item => item.dictValue).values().next().value,
						purchasecurrency: state.optionss["hr_export_currency"].filter(hr_export_currency => hr_export_currency.dictValue == element.purchaseCurrency).map(item => item.dictValue).values().next().value,
						purchaseunitprice: element.purchaseUnitPrice,
						onepacking: element.onePacking,
						isInvoicingc: state.optionss['hr_yes_no'].filter(hr_yes_no => hr_yes_no.dictValue == element.invoice).map(item => item.dictValue).values().next().value,
						packaging: element.packaging,
						specialrequirements: element.specialRequirements,
						rebaterate: element.rebateRate,
						outerboxloading: element.outerBoxLoading,
						outerboxunit: element.outerBoxUnit,
						outerboxlength: element.outerBoxLength.toFixed(1),
						outerboxwidth: element.outerBoxWidth.toFixed(1),
						outerboxheight: element.outerBoxHeight.toFixed(1),
						outerboxnetweight: element.outerBoxNetWeight.toFixed(1),
						outerboxgrossweight: element.outerBoxGrossWeight.toFixed(1),
						NumberOfBoxes: element.numberOfBoxes.toFixed(1),
						OtherFees: element.otherFees,
						additionalpackagingcosts: element.additionalPackagingCosts,
						innerBoxLoading: element.innerBoxLoading,
						singleProductGrossProfit: element.singleProductGrossProfit,
						singleProductGrossProfitTotal: element.singleProductGrossProfitTotal,
						grossProfitRate: element.grossProfitRate,
						totalNetWeight: element.totalNetWeight.toFixed(1),
						totalGrossWeight: element.totalGrossWeight.toFixed(1),
						totalVolume: element.totalVolume,
						SinglesalesrevenueA: element.singlesalesrevenue,
						Singleproductvolume: element.singleproductvolume,
						Oceanfreightforasingleproduct: element.oceanfreightforasingleproduct,
						Portchargesforindividualproducts: element.portchargesforindividualproducts,
						Inlandfreightforasingleproduct: element.inlandfreightforasingleproduct,
						outerboxvolume: element.outerBoxVolume,
						inlandfreightprice: element.inlandfreightprice,
						IsNewProduct: element.IsNewProduct,
						ProfitMargin: element.profitMargin
					});
			});
		}
	}).catch(error => {
		console.error(error);
	});
}
//#endregion

//#region 编辑功能开启
const EditQuotation = () => {
	isSaveBtnShow.value = false;
	showEditBtn.value = false;
	showEditSaveBtn.value = true;
	isDisabled.value = false;
	showSaveDraftBtn.value = true;
}
//#endregion
//编辑保存报价单
const EditSaveQuotation = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			for (let key in addQuotationRequest) {
				addQuotationRequest[key] = null;
			}
			addQuotationRequest.quotationProductDetailsList = [];
			addQuotationRequest.id = EditQuotationId.value;
			addQuotationRequest.customerid = parseInt(quotationDialogform.customernum);
			addQuotationRequest.quotationNum = quotationDialogform.quotationnum;
			addQuotationRequest.inquiryDate = quotationDialogform.inquirydate;
			addQuotationRequest.realQuotationDate = quotationDialogform.realquotationdate;
			addQuotationRequest.validityPeriod = quotationDialogform.validityperiod;
			addQuotationRequest.quotationStatus = quotationDialogform.quorationstatus;
			addQuotationRequest.customerNum = state.optionss['sql_hr_customer'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value;
			addQuotationRequest.customerName = state.optionss['sql_hr_customer_abbreviation'].filter(item => item.dictValue == quotationDialogform.customerid).map(item => item.dictLabel).values().next().value;
			addQuotationRequest.contactPerson = quotationDialogform.contactperson;
			addQuotationRequest.contactPersonEmail = quotationDialogform.contactpersonEmail;
			addQuotationRequest.customerLevel = quotationDialogform.customerlevel;
			addQuotationRequest.ourCompany = quotationDialogform.ourcompany;
			addQuotationRequest.settlementWay = quotationDialogform.settlementway;
			addQuotationRequest.settlementCategory = quotationDialogform.settlementcategory;
			addQuotationRequest.exportCurrency = quotationDialogform.exportcurrency;
			addQuotationRequest.exchangeRate = quotationDialogform.exchangerate;
			addQuotationRequest.uniformProfitMargin = quotationDialogform.uniformprofitmargin;
			addQuotationRequest.pricingTerm = quotationDialogform.pricingterm;
			addQuotationRequest.shippingPort = quotationDialogform.shippingport;
			addQuotationRequest.destinationPort = quotationDialogform.destinationport;
			addQuotationRequest.tradingCountry = quotationDialogform.tradingcountry;
			addQuotationRequest.transportationMethod = quotationDialogform.transportationmethod;
			addQuotationRequest.shippingCurrency = quotationDialogform.shippingcurrency;
			addQuotationRequest.shippingRate = quotationDialogform.shippingrate;
			addQuotationRequest.unitFreight = quotationDialogform.unitfreight;
			addQuotationRequest.commissionRate = quotationDialogform.commissionrate;
			addQuotationRequest.seller = quotationDialogform.seller;
			addQuotationRequest.remark = quotationDialogform.Remark ?? '';
			addQuotationRequest.totalValueOfGoods = quotationDialogform.TotalValueOfGoods;
			addQuotationRequest.totalQuantity = quotationDialogform.TotalQuantity;
			addQuotationRequest.totalNumberOfBoxes = Number(quotationDialogform.TotalNumberOfBoxes).toFixed(1);
			addQuotationRequest.totalGrossWeight = quotationDialogform.TotalGrossWeight;
			addQuotationRequest.totalNetWeight = quotationDialogform.TotalNetWeight;
			addQuotationRequest.totalVolume = quotationDialogform.TotalVolume;
			addQuotationRequest.totalPurchases = quotationDialogform.TotalPurchases;
			addQuotationRequest.totalOtherFees = quotationDialogform.TotalOtherFees;
			addQuotationRequest.totalTaxRefund = quotationDialogform.TotalTaxRefund;
			addQuotationRequest.profitAmount = quotationDialogform.ProfitAmount;
			addQuotationRequest.portMiscellaneousFees = quotationDialogform.portMiscellaneousFees;
			addQuotationRequest.inlandFreight = quotationDialogform.inlandFreight;
			addQuotationRequest.oceanFreight = quotationDialogform.oceanFreight;
			addQuotationRequest.singleCubicCost = quotationDialogform.singleCubicCost;
			addQuotationRequest.freightForwarderCustomsClearanceFees = quotationDialogform.freightForwarderCustomsClearanceFees;
			addQuotationRequest.totalgrossprofit = quotationDialogform.Totalgrossprofit;
			addQuotationRequest.totalprofitmargin = quotationDialogform.Totalprofitmargin;
			addQuotationRequest.bankfee = quotationDialogform.BankFee;
			addQuotationRequest.documentationfees = quotationDialogform.DocumentationFees;
			productData.value.forEach((item) => {
				addQuotationRequest.quotationProductDetailsList.push({
					id: item.id,
					productNum: item.productNum,
					customerNum: item.customerNum,
					cproductname: item.cproductname,
					cspecification: item.cspecification,
					quotationnum: item.quotationnum,
					exportunitprice: item.exportunitprice,
					exporttotalprice: item.exporttotalprice,
					unitofmeasurement: item.unitofmeasurement,
					purchasecurrency: item.purchasecurrency,
					purchaseunitprice: item.purchaseunitprice,
					onepacking: item.additionalpackagingcosts,
					Invoice: item.isInvoicingc,
					packaging: item.packaging,
					specialrequirements: item.specialrequirements,
					rebaterate: item.rebaterate,
					outerboxloading: item.outerboxloading,
					outerboxunit: item.outerboxunit,
					outerboxlength: item.outerboxlength,
					outerboxwidth: item.outerboxwidth,
					outerboxheight: item.outerboxheight,
					outerboxnetweight: item.outerboxnetweight,
					outerboxgrossweight: item.outerboxgrossweight,
					outerboxvolume: item.outerboxvolume,
					NumberOfBoxes: item.NumberOfBoxes,
					OtherFees: item.OtherFees,
					additionalpackagingcosts: item.additionalpackagingcosts,
					innerBoxLoading: item.innerBoxLoading,
					singleProductGrossProfit: item.singleProductGrossProfit,
					singleProductGrossProfitTotal: item.singleProductGrossProfitTotal,
					grossProfitRate: item.grossProfitRate,
					totalNetWeight: item.totalNetWeight,
					totalGrossWeight: item.totalGrossWeight,
					totalVolume: item.totalVolume,
					Singlesalesrevenue: item.SinglesalesrevenueA,
					Singleproductvolume: item.Singleproductvolume,
					Portchargesforindividualproducts: item.Portchargesforindividualproducts,
					Oceanfreightforasingleproduct: item.Oceanfreightforasingleproduct,
					Inlandfreightforasingleproduct: item.Inlandfreightforasingleproduct,
					inlandfreightprice: item.inlandfreightprice,
					IsNewProduct: item.isImported == true ? 0 : 1
				});
			});
			request.post('Quotation/EditQuotation/Edit', addQuotationRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: "报价单提交成功",
						type: 'success'
					})
					isDisabled.value = false;
					showEditSaveBtn.value = false;
					showEditBtn.value = false;
					isReviewBtnShow.value = false;
					GetQuotationList(currentPage.value, pageSize.value);
				} else {
					console.error('编辑报价单出错');
				}
			}).catch(error => {
				console.error('编辑报价单出错！😔错误内容：', error);
			})
		} else {
			// 获取第一个验证失败的字段
			const errorFields = Object.keys(fields);
			if (errorFields.length > 0) {
				const firstErrorField = errorFields[0];
				const errorElement = document.querySelector(`[data-field="${firstErrorField}"]`);
				if (errorElement) {
					// 获取元素的位置信息
					const elementRect = errorElement.getBoundingClientRect();
					const isInViewport = (
						elementRect.top >= 0 &&
						elementRect.left >= 0 &&
						elementRect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
						elementRect.right <= (window.innerWidth || document.documentElement.clientWidth)
					);

					// 如果元素不在视口中，则滚动到该元素
					if (!isInViewport) {
						errorElement.scrollIntoView({
							behavior: 'smooth',
							block: 'center',
							inline: 'nearest'
						});
					}

					// 添加高亮效果
					errorElement.classList.add('highlight-error');

					// 2秒后移除高亮效果
					setTimeout(() => {
						errorElement.classList.remove('highlight-error');
					}, 2000);
				}
			}
		}
	});
}
const quotationDialogHandClose = () => {
	// 清空表单验证提示
	quotationDialogformRef.value?.clearValidate();
	quotationDialog.value = false;
	productData.value = [];
	QuotationRemarksTextarea.value = '';
	isSaveBtnShow.value = true;
	showEditBtn.value = false;
	showEditSaveBtn.value = false;
	isDisabled.value = false;
}
//提交审核报价单
const SubmitReview = (row) => {
	ElMessageBox.confirm('确定将当前单据提交审核么?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		ElMessage({
			message: '提交审核成功',
			type: 'success'
		})
	}).catch(() => {

	});
}

// 添加日期格式化函数
const formatDate = (row, column) => {
	const date = row[column.property];
	if (date) {
		// 使用正则表达式只保留年月日
		return date.replace(/^(\d{4}-\d{2}-\d{2}).*$/, '$1');
	}
	return '';
};
</script>