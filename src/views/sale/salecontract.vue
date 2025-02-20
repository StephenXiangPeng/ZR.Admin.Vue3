<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="openContractDialog">销售合同</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-input v-model="quotationNum" clearable style="width: 15%"
				placeholder="输入合同编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="customerinfoselect" placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="item in customerinfoselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="productselect" placeholder="选择产品（可输入查询）" style="width: 15%">
				<el-option v-for="item in productselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="customerinfoselect" filterable placeholder="选择销售员（可输入查询）" style="width: 15%">
				<el-option v-for="item in customerinfoselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="customerinfoselect" filterable placeholder="选择合同状态" style="width: 15%">
				<el-option v-for="item in customerinfoselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>
		</div>
		<div style="width: 100%; margin-top: 30px;">
			<el-date-picker v-model="inquiryDate" type="date" placeholder="请选择合同日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="quotationDate" type="date" placeholder="请选择合同日期" size="Default"
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
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;销售合同表</span>
			<el-divider></el-divider>
			<el-table :data="contractsTableData" style="width: 100%">
				<el-table-column prop="reviewStatus" label="审核状态编号" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractReviewStatusStr" label="审核状态" width="150"></el-table-column>
				<el-table-column prop="contractNumber" label="合同编号" width="150"></el-table-column>
				<el-table-column prop="contractDate" label="合同日期" width="150"></el-table-column>
				<el-table-column prop="contractStatus" label="合同状态" width="150"></el-table-column>
				<el-table-column prop="customerNumber" label="客户编号" width="150"></el-table-column>
				<el-table-column prop="customerAbbreviation" label="客户简称" width="150"></el-table-column>
				<el-table-column prop="effectiveDate" label="生效日期" width="150"></el-table-column>
				<el-table-column prop="deliveryDate" label="交货日期" width="150"></el-table-column>
				<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
				<el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
				<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
				<el-table-column prop="settlementMethod" label="结汇方式" width="150"></el-table-column>
				<el-table-column prop="priceTerms" label="价格条款" width="150"></el-table-column>
				<el-table-column prop="shippingPort" label="出运口岸" width="150"></el-table-column>
				<el-table-column prop="destinationPort" label="目的口岸" width="150"></el-table-column>
				<el-table-column prop="tradeCountry" label="贸易国别" width="150"></el-table-column>
				<el-table-column prop="deposit" label="有无定金" width="150"></el-table-column>
				<el-table-column prop="receivedDeposit" label="已收定金" width="150"></el-table-column>
				<el-table-column prop="depositDate" label="定金日期" width="150"></el-table-column>
				<el-table-column prop="stockProgress" label="备货进度" width="150"></el-table-column>
				<el-table-column prop="deliveryProgress" label="交货进度" width="150"></el-table-column>
				<el-table-column prop="estimatedProfitMargin" label="预估利润率" width="150"></el-table-column>
				<el-table-column fixed="right" label="操作" width="300">
					<template #default="scope">
						<el-button type="text" size="small" icon="Bell"
							@click="openReminderDialog(scope.row)">设置提醒</el-button>
						<el-button type="text" size="small" @click="checkContractsDetails(scope.row)">查看详情</el-button>
						<el-button type="text" size="small" @click="GeneratePDF(scope.row)">生成PDF</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="contractsTableDatahandlePageChange"
				:current-page="contractsTableDatacurrentPage" :page-size="contractsTableDatapageSize"
				:total="contractsTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />

		</div>
		<el-dialog v-model="contractDialog" title="创建销售合同" :close-on-click-modal=false style="width: 70%;"
			@close="handlecontractDialogclose">
			<span style="font-size: 20px; font-weight: bold;">基本信息【{{ contractReviewStatus }}】</span>
			<el-divider></el-divider>
			<el-form ref="NewcontractformRef" :rules="rules" :model="Newcontractform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="销售合同">
							<el-input v-model="Newcontractform.contractNumber" disabled style="width: 300px"
								placeholder="自动生成"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="合同日期">
							<el-date-picker v-model="Newcontractform.contractDate" type="date" placeholder="请选择合同日期"
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="合同状态">
							<el-select filterable v-model="Newcontractform.contractStatus" placeholder="请选择合同状态"
								disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="客户编号" prop="customerNumber">
							<el-select filterable v-model="Newcontractform.customerNumber" placeholder="请选择客户编号"
								:disabled="isDisabled" style="width: 300px" @change="handleCustomerSelection">
								<el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户简称" prop="customerAbbreviation">
							<el-select v-model="Newcontractform.customerAbbreviation" filterable placeholder="请选择客户简称"
								:disabled="isDisabled" style="width: 300px;" @change="handleCustomerSelection">
								<el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="联系人" prop="contactPerson">
							<el-select filterable v-model="Newcontractform.contactPerson" placeholder="请选择联系人"
								style="width: 300px" :disabled="isDisabled" @change="handleContactpersonSelection">
								<el-option v-for="item in contactpersonSelectOptions" :key="item.value"
									:label="item.label" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="联系人Email">
							<el-input v-model="Newcontractform.contactEmail" disabled style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="生效日期">
							<el-date-picker v-model="Newcontractform.effectiveDate" type="date" placeholder="请选择生效日期"
								disabled style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户等级">
							<el-select filterable v-model="Newcontractform.customerLevel" placeholder="请选择客户等级" disabled
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户合同">
							<el-input v-model="Newcontractform.customerContract" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="交货日期" prop="deliveryDate">
							<el-date-picker v-model="Newcontractform.deliveryDate" type="date" placeholder="请选择交货日期"
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司" prop="ourCompany">
							<el-select filterable v-model="Newcontractform.ourCompany" placeholder="请选择我方公司"
								style="width: 300px" :disabled="isDisabled">
								<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外销币种" prop="foreignCurrency">
							<el-select filterable v-model="Newcontractform.foreignCurrency" placeholder="请选择外销币种"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="结算类别" prop="settlementType">
							<el-select filterable v-model="Newcontractform.settlementType" placeholder="请选择结算类别"
								:disabled="isDisabled" style="width: 300px">
								<el-option label="类别1" value="1"></el-option>
								<el-option label="类别2" value="2"></el-option>
								<el-option label="类别3" value="3"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="汇率" prop="exchangeRate">
							<el-input v-model="Newcontractform.exchangeRate" style="width: 300px" :disabled="isDisabled"
								@blur="formatExchangeRate"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="结汇方式" prop="settlementMethod">
							<el-select filterable v-model="Newcontractform.settlementMethod" placeholder="请选择结汇方式"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="价格条款" prop="priceTerms">
							<el-select filterable v-model="Newcontractform.priceTerms" placeholder="请选择价格条款"
								style="width: 300px" :disabled="isDisabled" @change="FreightChange()">
								<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="美金汇率" prop="usdExchangeRate">
							<el-input v-model="Newcontractform.usdExchangeRate" style="width: 300px"
								:disabled="isDisabled" @change="calculateTotal"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="出运口岸" prop="shippingPort">
							<el-select filterable v-model="Newcontractform.shippingPort" placeholder="请选择出运口岸"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="目的口岸" prop="destinationPort">
							<el-input v-model="Newcontractform.destinationPort" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="贸易国别" prop="tradeCountry">
							<el-select filterable v-model="Newcontractform.tradeCountry" placeholder="请选择贸易国别"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="运输方式" prop="transportation">
							<el-select filterable v-model="Newcontractform.transportation" placeholder="请选择运输方式"
								:disabled="isDisabled" style="width: 300px" @change="FreightChange()">
								<el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="销售员" prop="salesperson">
							<el-select filterable v-model="Newcontractform.salesperson" placeholder="请选择销售员"
								style="width: 300px" disabled>
								<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="有无定金" prop="hasDeposit">
							<el-checkbox v-model="Newcontractform.hasDeposit" :disabled="isDisabled"
								@change="hasDeposithandleCheckboxChange"></el-checkbox>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="已收定金" v-show=DepositShow prop="receivedDeposit">
							<el-input v-model="Newcontractform.receivedDeposit" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="定金日期" v-show=DepositShow prop="depositDate">
							<el-date-picker v-model="Newcontractform.depositDate" type="date" placeholder="请选择定金日期"
								:disabled="isDisabled" style="width: 300px"
								@change="handleDepositDateChange"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="定金比例" v-show=DepositShow prop="Depositratio">
							<el-input v-model="Newcontractform.Depositratio" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">辅助信息</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="签约地点">
							<el-select filterable v-model="Newcontractform.signingLocation" placeholder="请选择签约地点"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_signing_place" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="可否分批">
							<el-select filterable v-model="Newcontractform.canPartial" placeholder="请选择可否分批"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="可否转运">
							<el-select filterable v-model="Newcontractform.canTransit" placeholder="请选择可否转运"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="佣金比率">
							<el-input v-model="Newcontractform.commissionRate" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="保险加成">
							<el-input v-model="Newcontractform.insuranceAddition" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="保险比率">
							<el-input v-model="Newcontractform.insuranceRate" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="海运费/m³">
							<el-input v-model="Newcontractform.oceanFreight" style="width: 300px;"
								@change="calculateTotal" :disabled="isDisabled" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="海运费币种" prop="shippingCurrency">
							<el-select v-model="Newcontractform.shippingCurrency" filterable placeholder="选择运费币种"
								:disabled="isDisabled" style="width: 300px;" @change="shippingcurrencyChange">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="海运费汇率">
							<el-input v-model="Newcontractform.shippingrate" style="width: 300px;"
								:disabled="isDisabled" @change="calculateTotal" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="港杂费/m³">
							<el-input v-model="Newcontractform.portMiscellaneousFees" style="width: 300px;"
								@change="calculateTotal" :disabled="isDisabled" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="货代报关杂费">
							<el-input v-model="Newcontractform.freightForwarderCustomsClearanceFees"
								style="width: 300px;" :disabled="isDisabled" />
						</el-form-item>
					</el-col>
					<el-col :span="8" v-if="false">
						<el-form-item label="收汇银行">
							<el-input v-model="Newcontractform.receivingBank" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="银行费用">
							<el-input v-model="Newcontractform.BankFee" style="width: 300px;" @change="calculateTotal"
								:disabled="isDisabled" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="文件杂费">
							<el-input v-model="Newcontractform.DocumentationFees" style="width: 300px;"
								@change="calculateTotal" :disabled="isDisabled" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="付款日期">
							<el-date-picker v-model="Newcontractform.paymentDate" type="date" placeholder="请选择付款日期"
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">产品资料&客户相关费用</span>
				<el-divider></el-divider>
				<el-button class="mt-4" type="primary" @click="OpenSearchProcutDialog" style="margin-bottom: 10px;"
					:disabled="isDisabled">导入产品</el-button>
				<el-button class="mt-4" type="primary" @click="onAddquotationProductItem" style="margin-bottom: 10px;"
					:disabled="isDisabled">添加新产品</el-button>
				<el-button class="mt-4" type="primary" @click="handleAddRow" style="margin-bottom: 10px;"
					:disabled="isDisabled">添加相关费用</el-button>
				<el-tabs v-model="activeTab" tab-position="top" style="height: 350px;" class="demo-tabs">
					<el-tab-pane label="产品资料" name="productMaterialtab">
						<el-table :data="productData" style="width: 100%;margin-bottom: 15px;" max-height="550">
							<el-table-column prop="productID" label="产品ID" width="120" v-if="false" />
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
							<el-table-column prop="contractQuantity" label="合同数量" width="110">
								<template #default="{ row }">
									<el-input v-model="row.contractQuantity" @change="calculateTotal"
										:disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="exportunitprice" label="外销单价" width="110">
								<template #default="{ row }">
									<el-input v-model="row.exportunitprice" @change="calculateTotal"
										:disabled="isDisabled" />
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
										style="width: 100%;" :disabled="isDisabled">
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
									<el-input v-model="row.purchaseunitprice" @change="calculateTotal"
										:disabled="isDisabled" :style="row.isPriceChanged === 1 ? {
											'--el-input-text-color': 'red',
											'--el-disabled-text-color': 'red',
										} : {}">
									</el-input>
								</template>
							</el-table-column>
							<el-table-column prop=" inlandfreightprice" label="内陆运费(m³)" width="110">
								<template #default="{ row }">
									<el-input v-model="row.inlandfreightprice" @change="calculateTotal"
										:disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="AdditionalPackagingCosts" label="单个产品额外包装费用" width="165">
								<template #default="{ row }">
									<el-input v-model="row.AdditionalPackagingCosts" @change="calculateTotal"
										:disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="singleProductGrossProfit" label="单个产品毛利" width="110">
								<template #default="scope">
									<span :class="{ 'red-text': scope.row.isPriceChanged }">{{
										scope.row.singleProductGrossProfit }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="singleProductGrossProfitTotal" label="单个产品毛利合计" width="140">
								<template #default="scope">
									<span :class="{ 'red-text': scope.row.isPriceChanged }">{{
										scope.row.singleProductGrossProfitTotal }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="grossProfitRate" label="毛利率%" width="110">
								<template #default="scope">
									<span :class="{ 'red-text': scope.row.isPriceChanged }">{{
										scope.row.grossProfitRate
									}}</span>
								</template>
							</el-table-column>
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
									<el-select v-model="scope.row.packaging" filterable :disabled="isDisabled"
										placeholder="请选择" style="width: 100%;">
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
									<el-input v-model="row.rebaterate" @change="calculateTotal"
										:disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="innerBoxLoading" label="内盒装量" width="100">
								<template #default="{ row }">
									<el-input v-model="row.innerBoxLoading" @change="calculateTotal"
										:disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="outerboxloading" label="外箱装量" width="100">
								<template #default="{ row }">
									<el-input v-model="row.outerboxloading" @change="calculateTotal"
										:disabled="isDisabled" />
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
							<el-table-column prop="totalNetWeight" label="总净重(KG)" width="100">
								<template #default="scope">
									<span>{{ scope.row.totalNetWeight }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="totalGrossWeight" label="总毛重(KG)" width="100">
								<template #default="scope">
									<span>{{ scope.row.totalGrossWeight }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="totalVolume" label="总体积(m³)" width="100">
								<template #default="scope">
									<span>{{ scope.row.totalVolume }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="OtherFees" label="单个产品其它费用" width="140">
								<template #default="{ row }">
									<el-input v-model="row.OtherFees" @change="calculateTotal" :disabled="isDisabled" />
								</template>
							</el-table-column>
							<el-table-column prop="SinglesalesrevenueA" label="单个销售收入A" width="120" v-if="true">
								<template #default="scope">
									<span :class="{ 'red-text': scope.row.isPriceChanged }">{{
										scope.row.SinglesalesrevenueA }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="Singleproductvolume" label="单个产品体积(m³)" width="135" v-if="true">
								<template #default="scope">
									<span>{{ scope.row.Singleproductvolume }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="Portchargesforindividualproducts" label="单个产品的港杂费" width="140"
								v-if="true">
								<template #default="scope">
									<span>{{ scope.row.Portchargesforindividualproducts }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="Oceanfreightforasingleproduct" label="单个产品海运费" width="130"
								v-if="true">
								<template #default="scope">
									<span>{{ scope.row.Oceanfreightforasingleproduct }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="Inlandfreightforasingleproduct" label="单个产品内陆运费" width="140"
								v-if="true">
								<template #default="scope">
									<span>{{ scope.row.Inlandfreightforasingleproduct }}</span>
								</template>
							</el-table-column>
							<el-table-column prop="isPriceChanged" label="是否价格变动" width="140" v-if="true">
								<template #default="scope">
									<span>{{ scope.row.isPriceChanged }}</span>
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
					</el-tab-pane>
					<el-tab-pane label="客户相关费用" name="CustomerRelaterExoensestab">
						<el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; " height="280">
							<el-table-column prop=" expenseName" label="费用名称" width="150">
								<template #default="{ row }">
									<el-input v-model="row.expenseName" placeholder="输入费用名称" size="small"
										:disabled="isDisabled"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="currency" label="币种" width="150">
								<template #default="{ row }">
									<el-select filterable v-model="row.currency" placeholder="选择币种" size="small"
										@change="currencyChange(row)" :disabled="isDisabled">
										<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="exchangeRate" label="汇率" width="150">
								<template #default="{ row }">
									<el-input v-model="row.exchangeRate" placeholder="输入汇率" size="small"
										:disabled="isDisabled"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="expense" label="费用" width="150">
								<template #default="{ row }">
									<el-input v-model="row.expense" placeholder="输入费用" size="small"
										@input="expenseChange(row)" :disabled="isDisabled"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="amount" label="金额" width="150">
								<template #default="{ row }">
									<el-input v-model="row.amount" disabled size="small"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="remark" label="备注" width="150">
								<template #default="{ row }">
									<el-input v-model="row.remark" placeholder="输入备注" size="small"
										:disabled="isDisabled"></el-input>
								</template>
							</el-table-column>
							<el-table-column label="操作" width="100" fixed="right">
								<template #default="scope">
									<el-button type="text" size="small" @click="handleDelete(scope.$index)"
										:disabled="isDisabled">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</el-tab-pane>
					<el-tab-pane label="已收费用详情" name="ReceivedExpenseDetailsTab">
						<el-table :data="ReceivedExpenseDetailsTableData" style="width: 100%; " height="280">
							<el-table-column prop="receiptNumber" label="收款单号" width="150"></el-table-column>
							<el-table-column prop="fundsType" label="收款类型" width="150"></el-table-column>
							<el-table-column prop="receiptDate" label="收汇日期" width="150"
								:formatter="(row, column, cellValue) => formatDate(cellValue)"></el-table-column>
							<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
							<el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
							<el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
							<el-table-column prop="amount" label="金额" width="150"></el-table-column>
							<el-table-column prop="bank" label="收汇银行" width="150"></el-table-column>
						</el-table>
					</el-tab-pane>
				</el-tabs>
				<span style="font-size: 20px; font-weight: bold;">合计信息</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8">
						<el-form-item label="货值合计">
							<el-input v-model="Newcontractform.TotalValueOfGoods" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="数量合计">
							<el-input v-model="Newcontractform.TotalQuantity" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="箱数合计">
							<el-input v-model="Newcontractform.TotalNumberOfBoxes" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="毛重合计">
							<el-input v-model="Newcontractform.TotalGrossWeight" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="净重合计">
							<el-input v-model="Newcontractform.TotalNetWeight" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="体积合计">
							<el-input v-model="Newcontractform.TotalVolume" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="采购合计">
							<el-input v-model="Newcontractform.TotalPurchases" style="width: 300px"
								:disabled="isDisabled" :style="hasChangedProducts ? {
									'--el-input-text-color': 'red',
									'--el-disabled-text-color': 'red',
								} : {}"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="退税总额">
							<el-input v-model="Newcontractform.TotalTaxRefund" style="width: 300px" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户费用合计">
							<el-input v-model="Newcontractform.customerExpenseTotal" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="金额合计">
							<el-input v-model="Newcontractform.amountTotal" style="width: 300px" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="美金/欧元换算">
							<el-input v-model="Newcontractform.usdConversion" style="width: 300px" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="总毛利合计">
							<el-input v-model="Newcontractform.Totalgrossprofit" disabled style="width: 300px;" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="其它费用合计">
							<el-input v-model="Newcontractform.TotalOtherFees" disabled style="width: 300px;" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" />
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">利润预估</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8">
						<el-form-item label="利润金额">
							<el-input v-model="Newcontractform.ProfitAmount" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="退税总额">
							<el-input v-model="Newcontractform.TotalTaxRefund" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="总利润率%">
							<el-input v-model="Newcontractform.Totalprofitmargin" :style="hasChangedProducts ? {
								'--el-input-text-color': 'red',
								'--el-disabled-text-color': 'red',
							} : {}" disabled style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">备注信息</span>
				<el-divider></el-divider>
				<el-input v-model="Newcontractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }"
					type="textarea" placeholder="请输入备注信息" :disabled="isDisabled" />
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" v-show="isSaveBtnShow" @click="SaveContract(NewcontractformRef)">
						确定保存
					</el-button>
					<el-button type="primary" v-show="showEditBtn" @click="EditContract">
						编辑
					</el-button>
					<el-button type="primary" v-show="showEditSaveBtn" @click="EditContractSave(NewcontractformRef)">
						编辑保存
					</el-button>
					<el-button type="success" v-show="isAuditBtnShow" @click="SubmitForReview">
						提交审核
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="SearchProcutDialog" title="选择产品" :close-on-click-modal=false :width="'50%'"
			@close="handleCloseSearchProcutDialog">
			<el-input v-model="searchProductNameText" placeholder="请输入产品关键字进行搜索" style="margin-bottom: 10px;"
				@input="searchProductNameTextChange" />
			<el-table :data="productDatatwo" :default-sort="{ prop: 'productCode', order: 'descending' }"
				style="width: 100%" @row-dblclick="handleRowDblClick" stripe>
				<el-table-column prop="productCode" label="产品编号" sortable width="120" />
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
		<el-dialog v-model="reminderDialogVisible" title="设置提醒" width="30%" :close-on-click-modal="false">
			<div class="reminder-dialog">
				<el-form :model="reminderForm" label-width="100px">
					<el-form-item label="项目分类">
						<el-input v-model="reminderForm.title" type="text" placeholder="请输入项目分类" disabled></el-input>
					</el-form-item>
					<el-form-item label="提醒内容">
						<el-input v-model="reminderForm.content" type="text" placeholder="请输入提醒内容"></el-input>
					</el-form-item>
					<el-form-item label="提醒时间">
						<el-date-picker v-model="reminderForm.reminderTime" type="datetime" placeholder="选择提醒时间"
							format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DD HH:mm:ss"></el-date-picker>
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="reminderDialogVisible = false">取消</el-button>
					<el-button type="primary" @click="setReminder">确定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn,
	ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile,
	UploadFile, FormInstance, FormRules,
	stepProps
} from 'element-plus'
import { FOCUSABLE_CHILDREN } from 'element-plus/es/directives/trap-focus';
import request from '@/utils/request';
import { color, number } from 'echarts';
import { Edit } from '@element-plus/icons-vue/dist/types';
import { dataScope } from '@/api/system/role';
import { JsonHubProtocol } from '@microsoft/signalr';
import { get } from 'sortablejs';
import useUserStore from "@/store/modules/user";
import { Row } from 'element-plus/es/components/table-v2/src/components';

const GeneratePDF = (row) => {
	// 在这里添加生成PDF的逻辑
	console.log('正在为以下数据生成PDF:', row);
	request({
		url: 'Contracts/CreatePDF/CreatePDF',
		method: 'GET'
	}).then(response => {
		console.log(response);
	}).catch(error => {
		console.error('生成PDF失败:', error);
	});
}

const hasChangedProducts = computed(() => {
	return productData.value.some(product => product.isPriceChanged === 1);
});

// 添加此函数来获取已收费用明细
const getReceivedExpenseDetails = (contractId) => {
	request({
		url: 'CustomerCollections/GetCustomerCollectionsByContractID/GetList',
		method: 'GET',
		params: {
			ContractID: contractId
		}
	}).then(response => {
		if (response.data) {
			ReceivedExpenseDetailsTableData.value = response.data.map(item => ({
				receiptNumber: item.receiptNumber,    // 收据编号
				receiptDate: item.receiptDate,        // 收据日期
				ourCompany: state.optionss.hr_ourcompany.find(x => x.dictValue === item.ourCompany?.toString())?.dictLabel || '',  // 我方公司
				foreignCurrency: state.optionss.hr_export_currency.find(x => x.dictValue === item.foreignCurrency?.toString())?.dictLabel || '',  // 外币
				exchangeRate: item.exchangeRate,       // 汇率
				amount: item.amount,                   // 金额
				bank: state.optionss.hr_bank.find(x => x.dictValue === item.bank?.toString())?.dictLabel || '',  // 开户行
				fundsType: state.optionss.funds_type.find(x => x.dictValue === item.fundsType?.toString())?.dictLabel || ''  // 收款类型
			}));
		} else {
			ReceivedExpenseDetailsTableData.value = [];
		}
	}).catch(error => {
		console.error('获取已收费用明细失败:', error);
		ReceivedExpenseDetailsTableData.value = [];
	});
}
const ReceivedExpenseDetailsTableData = ref([]);
// 添加格式化日期函数
const formatDate = (dateString) => {
	if (!dateString) return '';
	const date = new Date(dateString);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	return `${year}-${month}-${day}`;
};

//提交审核
const SubmitForReview = () => {
	request({
		url: 'Contracts/SubmitForReview/SubmitContractReview',
		method: 'GET',
		params: {
			ContractID: SelctedContractId.value
		}
	}).then(response => {
		if (response != null) {
			ElMessage.success(response.data);
			GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
			contractDialog.value = false;
		} else {
			ElMessage.error(response.data);
		}
	}).catch(error => {
		console.error(error);
	});
}

//关闭合同窗体
const handlecontractDialogclose = () => {
	hasChangedProducts.value = false;
	SelctedContractId.value = 0;
	isDisabled.value = false;
	showEditBtn.value = false;
	showEditSaveBtn.value = false;
	isSaveBtnShow.value = true;
	GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
	clearAll();
}

//定金日期触发函数，将定金日期赋值给有效日期
const handleDepositDateChange = (date) => {
	if (date) {
		Newcontractform.effectiveDate = date;
	}
}
//美金汇率触发函数，将美金汇率保留4位小数
const formatExchangeRate = () => {
	if (Newcontractform.exchangeRate) {
		Newcontractform.exchangeRate = Number(Number(Newcontractform.exchangeRate).toFixed(4));
	}
}

var userId = useUserStore().userId;

//允许编辑
const isSaveBtnShow = ref(true);
const showEditBtn = ref(false);
const showEditSaveBtn = ref(false);
const isDisabled = ref(false);
//审核按钮
const isAuditBtnShow = ref(false);

//产品资料tab&客户相关费用tab
const activeTab = ref('productMaterialtab');
const CustomerRelaterExoensesTableData = ref([]);

//查找产品窗体
const SearchProcutDialog = ref(false)
//查询产品信息表格
const productDatatwo = ref([])
const myTable = ref(null)
//销售合同产品表格
const productData = ref([])
//分页组件
const SearchProducttotalItems = ref(0);
const SearchProductCurrentPage = ref(1);
const SearchProductpageSize = ref(10);
const searchProductNameText = ref('');
const SearchProducthandlePageChange = async (newPage) => {
	SearchProductpageSize.value = newPage;
	const start = newPage;
	const end = SearchProductpageSize.value;
	const newData = await GetProductInfoList(start, end);
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
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					productDatatwo.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
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
			productID: row.id,
			productNum: row.productCode,
			customerNum: '',
			cproductname: row.chineseProductName,
			cspecification: row.chineseSpecification,
			contractQuantity: 0,
			exportunitprice: 0,
			exporttotalprice: 0,
			unitofmeasurement: state.optionss['hr_calculate_unit'].filter(hr_calculate_unit => hr_calculate_unit.dictLabel == row.unitOfMeasurement).map(item => item.dictValue).values().next().value,
			purchaseinquiry: 0,
			purchaseunitprice: 0,
			onepacking: 0,
			invoice: '',
			packaging: '',
			specialrequirements: '',
			rebaterate: 0,
			outerboxunit: '',
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

const shippingcurrencyChange = (value) => {
	if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == value).map(item => item.dictValue).values().next().value == 3) {
		Newcontractform.shippingrate = 1;
	} else {
		Newcontractform.shippingrate = null;
	}
}

function formatNumber(row, key) {
	if (row[key] !== null && row[key] !== undefined) {
		row[key] = parseFloat(row[key]).toFixed(1);
	}
}

///计算外销总价、计算外箱体积、计算总货值
const calculateTotal = () => {
	Newcontractform.TotalValueOfGoods = 0;
	Newcontractform.TotalQuantity = 0;
	Newcontractform.TotalNumberOfBoxes = 0;
	Newcontractform.TotalGrossWeight = 0;
	Newcontractform.TotalNetWeight = 0;
	Newcontractform.TotalVolume = 0;
	Newcontractform.TotalPurchases = 0;
	Newcontractform.TotalOtherFees = 0;
	Newcontractform.TotalTaxRefund = 0;
	Newcontractform.ProfitAmount = 0;
	Newcontractform.Totalgrossprofit = 0;
	Newcontractform.Totalprofitmargin = 0;

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
	let TotalSinglesalesrevenueA = 0;//总和单个销售收入A
	let Totalgrossprofit = 0;//总毛利合计
	let Totalprofitmargin = 0;//总利润率
	let TotalOtherFees = 0;//其它费用合计
	productData.value.forEach((item) => {
		//箱数 = 合同数量/外箱装量
		item.NumberOfBoxes = (item.contractQuantity / item.outerboxloading);
		item.NumberOfBoxes = Number(item.NumberOfBoxes.toFixed(1));//箱数保留一位小数
		//外箱体积 =（外箱长度 x 外箱宽度 x 外箱高度）/1000000
		item.outerboxvolume = (item.outerboxlength * item.outerboxwidth * item.outerboxheight / 1000000);
		item.outerboxvolume = item.outerboxvolume.toFixed(4);//外箱体积保留4位小数
		//单个销售收入A =	（采购单价 / (1+退税率）x 13% + 销售单价 x 汇率）
		item.SinglesalesrevenueA = (item.purchaseunitprice / (1 + item.rebaterate / 100) * (item.rebaterate / 100) + item.exportunitprice * Number(Newcontractform.exchangeRate));
		item.SinglesalesrevenueA = item.SinglesalesrevenueA.toFixed(3);//单个销售收入A保留3位小数
		//单个产品体积=外箱体积/外箱装量
		item.Singleproductvolume = (item.outerboxvolume / item.outerboxloading).toFixed(4).toString().replace(/(\.\d*?[1-9])0+$/, '$1');
		//单个产品的港杂费=港杂费 x 单个产品体积
		item.Portchargesforindividualproducts = (Number(Newcontractform.portMiscellaneousFees) * Number(item.Singleproductvolume)).toFixed(3);
		console.log('单个产品的港杂费：' + item.Portchargesforindividualproducts);
		//单个产品海运费=海运费 x 单个产品体积(如果非人民币，还需要乘以汇率)
		if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == Newcontractform.shippingCurrency).map(item => item.dictLabel).values().next().value == '人民币') {
			item.Oceanfreightforasingleproduct = (Number(Newcontractform.oceanFreight) * item.Singleproductvolume).toFixed(3);
		} else {
			item.Oceanfreightforasingleproduct = (Number(Newcontractform.oceanFreight) * item.Singleproductvolume * Number(Newcontractform.shippingrate)).toFixed(3);
		}
		//item.Oceanfreightforasingleproduct = (Number(quotationDialogform.oceanFreight) * item.Singleproductvolume).toFixed(3);
		//单个产品内陆运费=内陆运费 x 单个产品体积
		item.Inlandfreightforasingleproduct = (Number(item.inlandfreightprice) * item.Singleproductvolume).toFixed(3);
		if (isNaN(item.SinglesalesrevenueA - item.Portchargesforindividualproducts - item.Inlandfreightforasingleproduct - item.Oceanfreightforasingleproduct - item.purchaseunitprice - item.additionalpackagingcosts - item.OtherFees)) {
			item.singleProductGrossProfit = 0.000;
		} else {
			//单个产品毛利=单个销售收入A - 单个港杂费 -单个产品内陆运费 - 单个海运费 - 采购价 - 额外包装费用 - 其他费用
			item.singleProductGrossProfit = (item.SinglesalesrevenueA - item.Portchargesforindividualproducts - item.Inlandfreightforasingleproduct - item.Oceanfreightforasingleproduct - item.purchaseunitprice - item.additionalpackagingcosts - item.OtherFees).toFixed(3);
			//单个产品毛利合计=单个产品毛利 x 合同数量
			item.singleProductGrossProfitTotal = (item.singleProductGrossProfit * item.contractQuantity).toFixed(3);
		}
		if (isNaN(item.singleProductGrossProfit / item.SinglesalesrevenueA)) {
			item.grossProfitRate = 0.000;
		} else {
			//毛利率=单个产品毛利/单个销售收入A
			item.grossProfitRate = (item.singleProductGrossProfit / item.SinglesalesrevenueA * 100).toFixed(3);
		}
		if (isNaN(item.outerboxnetweight * item.NumberOfBoxes)) {
			item.totalNetWeight = 0.000;
		} else {
			//总净 = 外箱净重 x 箱数
			item.totalNetWeight = (item.outerboxnetweight * item.NumberOfBoxes).toFixed(1);
		}
		if (isNaN(item.outerboxgrossweight * item.NumberOfBoxes)) {
			item.totalGrossWeight = 0.000;
		} else {
			//总毛重 = 外箱毛重 x 箱数
			item.totalGrossWeight = (item.outerboxgrossweight * item.NumberOfBoxes).toFixed(1);
		}
		if (isNaN(item.outerboxvolume * item.NumberOfBoxes)) {
			item.totalVolume = 0.000;
		} else {
			//总体积 = 外箱体积 x 箱数
			item.totalVolume = (item.outerboxvolume * item.NumberOfBoxes).toFixed(1);
		}
		//外销总价=合同数量 x 报价单价
		item.exporttotalprice = Number(item.contractQuantity * item.exportunitprice);
		item.exporttotalprice = item.exporttotalprice.toFixed(2);//外销总价保留2位小数
		TotalSinglesalesrevenueA += Number(item.SinglesalesrevenueA * item.contractQuantity);
		TotalvalueOfGoods += Number(item.exporttotalprice);
		TotalQuantity += Number(item.contractQuantity);
		TotalNumberOfBoxes += Number(item.NumberOfBoxes.toFixed(1));
		TotalGrossWeight += Number(item.totalGrossWeight);
		TotalNetWeight += Number(item.totalNetWeight);
		TotalVolume += Number(item.totalVolume);
		TotalPurchases += Number(item.purchaseunitprice) * Number(item.contractQuantity);
		OtherFees += Number(item.OtherFees);
		TotalTaxRefund += Number((item.purchaseunitprice / (1 + item.rebaterate / 100) * 0.13 * item.contractQuantity).toFixed(3));//采购单价*13%*合同数量
		ProfitAmount += Number(item.ProfitAmount);
		//总毛利合计=单个产品毛利 x 合同数量 - 其他费用
		Totalgrossprofit += Number(item.singleProductGrossProfit * item.contractQuantity - item.OtherFees);
		console.log('总毛利合计' + item.singleProductGrossProfit + '/' + item.contractQuantity + '=' + Totalgrossprofit);
		//总利润率=总毛利合计/单个销售收入A总和
		Totalprofitmargin = Number((Totalgrossprofit / TotalSinglesalesrevenueA * 100).toFixed(3));
		console.log('总利润率' + Totalgrossprofit + '/' + TotalSinglesalesrevenueA + '=' + Totalprofitmargin);
		//#region 其它费用合计计算
		//海运费合计=单个产品海运费*运费汇率*合同数量
		let oceanFreightTotal = item.Oceanfreightforasingleproduct * Newcontractform.shippingrate * item.contractQuantity;
		console.log('海运费合计：' + oceanFreightTotal)
		//内陆运费合计=单个产品内陆运费*合同数量
		let inlandAndPortChargesTotal = Number(item.Inlandfreightforasingleproduct * item.contractQuantity);
		console.log('内陆运费合计：' + inlandAndPortChargesTotal);
		let PortchargesforindividualproductsTotal = Number(item.Portchargesforindividualproducts * item.contractQuantity);
		console.log('港杂费合计：' + PortchargesforindividualproductsTotal);
		//银行费用合计=银行费用*汇率
		let bankFeeTotal = (Newcontractform.BankFee * Newcontractform.exchangeRate);
		console.log('银行费用合计：' + bankFeeTotal)
		//文件费用
		let documentationFees = Newcontractform.DocumentationFees;
		console.log('文件费用：' + documentationFees)
		//其它费用合计=海运费合计+内陆运费+港杂费合计+银行费用合计+文件费用+其它费用
		console.log('其它费用：' + OtherFees)
		TotalOtherFees = Number(oceanFreightTotal) + Number(inlandAndPortChargesTotal) + Number(PortchargesforindividualproductsTotal) + Number(bankFeeTotal) + Number(documentationFees) + Number(OtherFees);
		console.log('其它费用合计：' + TotalOtherFees)
		//#endregion 其它费用合���计算
	});
	Newcontractform.TotalValueOfGoods = TotalvalueOfGoods || 0;
	Newcontractform.TotalQuantity = TotalQuantity || 0;
	Newcontractform.TotalNumberOfBoxes = TotalNumberOfBoxes || 0;
	Newcontractform.TotalGrossWeight = TotalGrossWeight || 0;
	Newcontractform.TotalNetWeight = TotalNetWeight || 0;
	Newcontractform.TotalVolume = TotalVolume || 0;
	Newcontractform.TotalPurchases = TotalPurchases || 0;
	Newcontractform.TotalOtherFees = TotalOtherFees || 0;
	Newcontractform.TotalTaxRefund = TotalTaxRefund || 0;
	//利润金额=货值合计+退税总额-采购合计-其它费用
	Newcontractform.ProfitAmount = Number(Number((TotalvalueOfGoods * Newcontractform.exchangeRate + TotalTaxRefund) - Number(TotalPurchases) - Number(TotalOtherFees)).toFixed(2)) || 0;
	Newcontractform.Totalgrossprofit = Totalgrossprofit || 0;
	Newcontractform.Totalprofitmargin = Totalprofitmargin || 0;
	//金额合计
	Newcontractform.amountTotal = TotalvalueOfGoods;
	//美金换算
	Newcontractform.usdConversion = Number(Newcontractform.amountTotal / Newcontractform.exchangeRate);
}

const deleteRow = (index: number) => {
	productData.value.splice(index, 1)
}

const isImportProduct = ref(false);
const OpenSearchProcutDialog = () => {
	activeTab.value = 'productMaterialtab';
	isImportProduct.value = true;
	GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
	SearchProcutDialog.value = true;
}
const onAddquotationProductItem = () => {
	activeTab.value = 'productMaterialtab';
	isImportProduct.value = false;
	productData.value.push({
		productNum: '',
		customerNum: '',
		cproductname: '',
		cspecification: '',
		contractQuantity: 0,
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

// 销售合同窗体		
const contractDialog = ref(false)

interface Newcontractform {
	/*合同主体信息Start*/
	id: number,//id 自增列
	contractNumber: string,//销售合同编号
	contractDate: string,//合同日期
	contractStatus: number,//合同状态
	customerid: number,//客户ID
	customerNumber: string,//客户编号
	customerAbbreviation: string,//客户简称
	contactPerson: number,//联系人
	contactEmail: string,//联系人邮箱
	effectiveDate: string,//生效日期
	customerLevel: number,//客户级别
	customerContract: string,//客户合同
	deliveryDate: string,//交货日期
	ourCompany: number,//我方公司
	settlementType: number,//结算类别
	foreignCurrency: number,//外销币种
	exchangeRate: number,//汇率
	usdExchangeRate: number,//美金汇率
	settlementMethod: number,//结汇方式
	priceTerms: number,//价格条款
	shippingPort: number,//出运口岸
	destinationPort: string,//目的口岸
	tradeCountry: number,//贸易国别
	transportation: number,//运输方式
	salesperson: number,//销售员
	hasDeposit: boolean,//是否有定金
	receivedDeposit: number,//已收定金
	depositDate: string,//定金日期
	Depositratio: number,//定金比例
	/*合同主体信息End*/
	/*列表字段Start*/
	stockProgress: string,//备货进度
	deliveryProgress: string,//交货进度
	profitCalculation: string,//利润计算
	estimatedProfitMargin: string,//预估利润率
	customerOrder: string,//客户订单
	/*列表字段End*/
	/*辅助信息Start*/
	contractremark: string,//合同备注
	signingLocation: number,//签约地点
	canPartial: number,//是否可分批
	canTransit: number,//是否可转运
	insuranceAddition: number,//保险附加
	insuranceRate: number,//保险比率
	commissionRate: number,//佣金比率
	receivingBank: string//收汇银行
	paymentDate: string,//付款日期
	oceanFreight: number,//海运费
	portMiscellaneousFees: number,//港杂费
	inlandFreight: number,//内陆运费
	singleCubicCost: number,//个立方费用
	freightForwarderCustomsClearanceFees: number,//货代报关杂费
	/*辅助信息End*/
	/*合计信息Start*/
	TotalValueOfGoods: number,//货值合计
	TotalQuantity: number,//数量合计
	TotalNumberOfBoxes: number,//箱数合计
	TotalGrossWeight: number,//毛重合计
	TotalNetWeight: number,//净重合计
	TotalVolume: number,//体积合计
	TotalPurchases: number,//采购合计
	TotalTaxRefund: number,//退税总额
	customerExpenseTotal: number,//客户费用合计
	amountTotal: number,//金额合计
	usdConversion: number//美金/欧元换算
	/*合计信息End*/
	/*利润预估Start*/
	commissionAmount: number,//佣金金额
	shippingCurrency: number,//海运费币种
	shippingrate: number,//海运费汇率
	shippingCost: number,//海运费
	insuranceCost: number,//保险费用
	BankFee: number,//银行费用
	TotalOtherFees: number,//其它费用合计
	freightCost: number,//运杂费
	totalCost: number,//成本总额
	exchangeCost: number,//换汇成本
	productProfit: number,//产品毛利
	ProfitAmount: number,//利润金额
	profitRate: number,//利润率
	FreightCurrency: number,//运杂费币种
	FreightQuote: number,//运杂费报价
	Courierfee: number,//快递费
	Totalgrossprofit: number,//总毛利合计
	Totalprofitmargin: number,//总利润率
	DocumentationFees: number//文件杂费
	goodsValue: number | null,
	quantity: number | null,
	boxCount: number | null,
	grossWeight: number | null,
	netWeight: number | null,
	volume: number | null,
	/*利润预估End*/
}

const NewcontractformRef = ref<FormInstance>()
const Newcontractform = reactive<Newcontractform>({
	id: 0,
	contractNumber: '',
	contractDate: '',
	contractStatus: null,
	customerid: null,
	customerNumber: '',
	customerAbbreviation: '',
	contactPerson: null,
	contactEmail: '',
	effectiveDate: '',
	customerLevel: null,
	customerContract: '',
	deliveryDate: '',
	ourCompany: null,
	settlementType: null,
	foreignCurrency: null,
	exchangeRate: null,
	usdExchangeRate: null,
	settlementMethod: null,
	priceTerms: null,
	shippingPort: null,
	destinationPort: null,
	tradeCountry: null,
	transportation: null,
	salesperson: null,
	hasDeposit: false,
	receivedDeposit: null,
	depositDate: '',
	Depositratio: null,
	stockProgress: '',
	deliveryProgress: '',
	profitCalculation: '',
	estimatedProfitMargin: '',
	customerOrder: '',
	contractremark: '',
	signingLocation: null,
	canPartial: null,
	canTransit: null,
	insuranceAddition: null,
	insuranceRate: null,
	commissionRate: null,
	receivingBank: '',
	paymentDate: '',
	goodsValue: null,
	quantity: null,
	boxCount: null,
	grossWeight: null,
	netWeight: null,
	volume: null,
	purchaseTotal: null,
	taxRefundTotal: null,
	customerExpenseTotal: null,
	amountTotal: null,
	usdConversion: null,
	commissionAmount: null,
	shippingCurrency: null,
	shippingExchangeRate: null,
	shippingCost: null,
	insuranceCost: null,
	bankCost: null,
	otherCost: null,
	freightCost: null,
	totalCost: null,
	exchangeCost: null,
	productProfit: null,
	profitAmount: null,
	profitRate: null,
	FreightCurrency: null,
	FreightQuote: null,
	Courierfee: null
})

//合同表单验证规则
const rules = reactive<FormRules<Newcontractform>>({
	customerNumber: [{ required: true, message: '请选择客户', trigger: 'change,blur' }],
	customerAbbreviation: [{ required: true, message: '请输入客户简称', trigger: 'change,blur' }],
	contactPerson: [{ required: true, message: '请选择联系人', trigger: 'change,blur' }],
	deliveryDate: [{ required: true, message: '请选择交货日期', trigger: 'change,blur' }],
	ourCompany: [{ required: true, message: '请选择我方公司', trigger: 'change,blur' }],
	settlementType: [{ required: true, message: '请选择结算类别', trigger: 'change,blur' }],
	foreignCurrency: [{ required: true, message: '请选择外销币种', trigger: 'change,blur' }],
	exchangeRate: [{ required: true, message: '请输入汇率', trigger: 'change,blur' }],
	usdExchangeRate: [{ required: true, message: '请输入美金汇率', trigger: 'change,blur' }],
	settlementMethod: [{ required: true, message: '请选择结汇方式', trigger: 'change,blur' }],
	priceTerms: [{ required: true, message: '请选择价格条款', trigger: 'change,blur' }],
	shippingPort: [{ required: true, message: '请选择出运口岸', trigger: 'change,blur' }],
	destinationPort: [{ required: true, message: '请选择目的口岸', trigger: 'change,blur' }],
	tradeCountry: [{ required: true, message: '请选择贸易国别', trigger: 'change,blur' }],
	transportation: [{ required: true, message: '请选择运输方式', trigger: 'change,blur' }],
	salesperson: [{ required: true, message: '请选择销售员', trigger: 'change,blur' }],
	hasDeposit: [{ required: true, message: '请选择是否有定金', trigger: 'change' }],
	receivedDeposit: [],
	depositDate: [],
	Depositratio: []
});

const DepositShow = ref(false);
const hasDeposithandleCheckboxChange = (val) => {
	if (val == true) {
		Newcontractform.receivedDeposit = null;
		Newcontractform.depositDate = null;
		Newcontractform.Depositratio = null;
		// 如果勾选了"有定金"，则添加验证规则
		rules.receivedDeposit = [{ required: true, message: '请输入已收定金', trigger: 'change,blur' }];
		rules.depositDate = [{ required: true, message: '请选择定金日期', trigger: 'change,blur' }];
		rules.Depositratio = [{ required: true, message: '请输入定金比例', trigger: 'change,blur' }];
	} else if (val == false) {
		Newcontractform.receivedDeposit = null;
		// 如果取消勾选"有定金"，则移除验证规则
		rules.receivedDeposit = [];
		rules.depositDate = [];
		rules.Depositratio = [];
	}
	DepositShow.value = val;
}



/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_contract_status: [],
		sql_hr_customer: [],
		hr_customer_level: [],
		hr_settlement_way: [],
		hr_pricing_term: [],
		hr_nation: [],
		sql_hr_sale: [],
		hr_transport_port: [],
		hr_transportation_method: [],
		hr_ourcompany: [],
		hr_export_currency: [],
		hr_signing_place: [],
		sys_yes_no: [],
		hr_packing: [],
		sql_hr_customer_abbreviation: [],
		hr_outerbox_unit: [],
		hr_quotation_status: [],
		hr_yes_no: [],
		hr_calculate_unit: [],
		sql_hr_customer_name: [],
		hr_settlementcategory: [],
		sql_all_user: [],
		sql_hr_customer_contactperson: [],
		sql_hr_all_quotationnum: [],
		sql_product: [],
		sql_product_name: [],
		hr_bank: [],
		funds_type: [],
		sql_user_customers: []  // 用户特定的客户数据
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' },
	{ dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
	{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' },
	{ dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' },
	{ dictType: 'hr_transport_port' }, { dictType: 'hr_transportation_method' },
	{ dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' },
	{ dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
	{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' },
	{ dictType: 'hr_packing' }, { dictType: 'sql_hr_customer_abbreviation' },
	{ dictType: 'hr_outerbox_unit' }, { dictType: 'sql_product_name' },
	{ dictType: 'sql_hr_customer_contactperson' }, { dictType: 'sql_hr_all_quotationnum' },
	{ dictType: 'sql_product' }, { dictType: 'sql_product_name' },
	{ dictType: 'sql_hr_customer_abbreviation' }, { dictType: 'hr_yes_no' },
	{ dictType: 'hr_bank' }, { dictType: 'funds_type' }
]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
})

// 获取用户相关的客户数据
const getUserCustomerData = async () => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerDataByUserID/GetSelectCustomerDataByUserID',
			method: 'get'
		})

		if (response.code === 200) {
			state.optionss.sql_user_customers = response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel
			}))
		} else {
			ElMessage.error(response.msg || '获取客户数据失败')
		}
	} catch (error) {
		console.error('获取客户数据失败:', error)
		ElMessage.error('获取客户数据失败')
	}
}
getUserCustomerData();
/*动态下拉框end*/

///销售合同列表
const contractsTableData = ref([])
//销售合同列表分页组件
const contractsTableDatatotalItems = ref(0);
const contractsTableDatacurrentPage = ref(1);
const contractsTableDatapageSize = ref(10);
const contractsTableDatahandlePageChange = async (newPage) => {
	contractsTableDatacurrentPage.value = newPage;
	const start = newPage;
	const end = contractsTableDatapageSize.value;
	const newData = await GetContractList(start, end);
};

function GetContractList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'Contracts/GetContractsList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				contractsTableData.value = response.data.result;
				for (var i = 0; i < contractsTableData.value.length; i++) {
					contractsTableData.value[i].reviewStatus;
					switch (contractsTableData.value[i].reviewStatus) {
						case 0:
							contractsTableData.value[i].contractReviewStatusStr = '待提审';
							break;
						case 1:
							contractsTableData.value[i].contractReviewStatusStr = '审核中';
							break;
						case 2:
							contractsTableData.value[i].contractReviewStatusStr = '已批准';
							break;
						case 3:
							contractsTableData.value[i].contractReviewStatusStr = '已拒绝';
							break;
					}
					contractsTableData.value[i].contractStatus = state.optionss.hr_contract_status.find(item => item.dictValue === contractsTableData.value[i].contractStatus.toString()).dictLabel;
					contractsTableData.value[i].customerNumber = state.optionss.sql_hr_customer.find(item => item.dictValue === contractsTableData.value[i].customerId.toString()).dictLabel;
					contractsTableData.value[i].customerAbbreviation = state.optionss.sql_hr_customer_abbreviation.find(item => item.dictValue === contractsTableData.value[i].customerId.toString()).dictLabel;
					contractsTableData.value[i].ourCompany = state.optionss.hr_ourcompany.find(item => item.dictValue === contractsTableData.value[i].ourCompany.toString()).dictLabel;
					contractsTableData.value[i].foreignCurrency = state.optionss.hr_export_currency.find(item => item.dictValue === contractsTableData.value[i].foreignCurrency.toString()).dictLabel;
					contractsTableData.value[i].settlementMethod = state.optionss.hr_settlement_way.find(item => item.dictValue === contractsTableData.value[i].settlementMethod.toString()).dictLabel;
					contractsTableData.value[i].priceTerms = state.optionss.hr_pricing_term.find(item => item.dictValue === contractsTableData.value[i].priceTerms.toString()).dictLabel;
					contractsTableData.value[i].shippingPort = state.optionss.hr_transport_port.find(item => item.dictValue === contractsTableData.value[i].shippingPort.toString()).dictLabel;
					contractsTableData.value[i].tradeCountry = state.optionss.hr_nation.find(item => item.dictValue === contractsTableData.value[i].tradeCountry.toString()).dictLabel;
					contractsTableData.value[i].transportation = state.optionss.hr_transportation_method.find(item => item.dictValue === contractsTableData.value[i].transportation.toString()).dictLabel;
					contractsTableData.value[i].contractDate = contractsTableData.value[i].contractDate ? contractsTableData.value[i].contractDate.substring(0, 10) : '';
					contractsTableData.value[i].deliveryDate = contractsTableData.value[i].deliveryDate ? contractsTableData.value[i].deliveryDate.substring(0, 10) : '';
					contractsTableData.value[i].effectiveDate = contractsTableData.value[i].effectiveDate ? contractsTableData.value[i].effectiveDate.substring(0, 10) : '';
					contractsTableData.value[i].depositDate = contractsTableData.value[i].depositDate ? contractsTableData.value[i].depositDate.substring(0, 10) : '';
					contractsTableData.value[i].paymentDate = contractsTableData.value[i].paymentDate ? contractsTableData.value[i].paymentDate.substring(0, 10) : '';
				}
				contractsTableDatatotalItems.value = response.data.totalNum;
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetContractList(start - 1, end);
				} else {
					contractsTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}


//联系人
const contactpersonSelectOptions = ref([]);
const handleCustomerSelection = (value) => {
	//清空联系人
	Newcontractform.customerid = value;
	Newcontractform.contactPerson = null;
	Newcontractform.contactEmail = '';
	request({
		url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
		method: 'GET',
		params: {
			ID: value
		}
	}).then(response => {
		if (response != null) {
			Newcontractform.customerNumber = value.toString();
			Newcontractform.customerAbbreviation = value.toString();
			Newcontractform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == response.customerLevel.toString()).map(item => item.dictValue).values().next().value;
			Newcontractform.priceTerms = state.optionss['hr_pricing_term'].filter(item => item.dictValue == response.pricingTerm.toString()).map(item => item.dictValue).values().next().value;
			Newcontractform.settlementType = state.optionss['hr_settlement_way'].filter(item => item.dictValue == response.settlementWay.toString()).map(item => item.dictValue).values().next().value;
			Newcontractform.tradeCountry = state.optionss['hr_nation'].filter(item => item.dictValue == response.tradingCountry.toString()).map(item => item.dictValue).values().next().value;
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

//获取客户联系人
const GetCustomerContactPerson = (CustomerId) => {
	request({
		url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
		method: 'GET',
		params: {
			ID: CustomerId
		}
	}).then(response => {
		if (response != null) {
			if (response.contactPerson != null) {
				contactpersonSelectOptions.value = response.contactPerson.map(item => ({
					value: item.id,
					label: item.name,
					email: item.email
				}));
			}
		} else {
			ElMessage.error("获取客户联系人失败!");
		}
	}).catch(error => {
		console.log(error)
	});
}

const handleContactpersonSelection = (value) => {
	Newcontractform.contactEmail = contactpersonSelectOptions.value.find(item => item.value === value).email;
}

const handleAddRow = () => {
	activeTab.value = 'CustomerRelaterExoensestab';
	const newRow = {
		expenseName: '',
		currency: '',
		exchangeRate: '',
		expense: '',
		amount: '',
		remark: ''
	};
	CustomerRelaterExoensesTableData.value.push(newRow);
}

const handleDelete = (index) => {
	CustomerRelaterExoensesTableData.value.splice(index, 1);
	CalculateOtherCustomerFees();
}
const currencyChange = (row) => {
	switch (row.currency) {
		case '1':
			row.exchangeRate = 7.23;
			break;
		case '2':
			row.exchangeRate = 7.69;
			break;
		case '3':
			row.exchangeRate = 1;
			break;
	}
}

const expenseChange = (row) => {
	// 确保expense和exchangeRate是数字
	const expense = Number(row.expense) || 0;
	const rate = Number(row.exchangeRate) || 0;
	// 计算amount并保留2位小数
	row.amount = (expense * rate).toFixed(2);
	// 重置总费用
	Newcontractform.customerExpenseTotal = 0;
	CalculateOtherCustomerFees();
}

const CalculateOtherCustomerFees = () => {
	Newcontractform.customerExpenseTotal = 0;
	CustomerRelaterExoensesTableData.value.forEach(item => {
		Newcontractform.customerExpenseTotal += Number(item.amount) || 0;
	});
}

const addContractsRequest = reactive({
	// AddContractsRequest 主要属性
	Id: 0,
	ContractNumber: '',
	ContractDate: null,
	ContractStatus: null,
	CustomerId: null,
	CustomerNumber: '',
	CustomerAbbreviation: '',
	ContactPerson: null,
	ContactEmail: '',
	EffectiveDate: null,
	CustomerLevel: null,
	CustomerContract: '',
	DeliveryDate: null,
	OurCompany: null,
	SettlementType: null,
	ForeignCurrency: null,
	ExchangeRate: null,
	UsdExchangeRate: null,
	SettlementMethod: null,
	PriceTerms: null,
	ShippingPort: null,
	DestinationPort: null,
	TradeCountry: null,
	Transportation: null,
	Salesperson: null,
	HasDeposit: false,
	ReceivedDeposit: null,
	DepositDate: null,
	Depositratio: null,
	CustomerOrder: '',
	Remarks: '',
	SigningLocation: null,
	CanPartial: 0,
	CanTransit: 0,
	InsuranceAddition: null,
	InsuranceRate: 0,
	CommissionRate: null,
	ShippingCurrency: null,
	ShippingExchangeRate: null,
	ShippingCost: null,
	freightForwarderCustomsClearanceFees: null,
	ReceivingBank: '',
	BankCost: null,
	DocumentationFees: null,
	PaymentDate: null,
	GoodsValue: null,
	Quantity: null,
	BoxCount: 0,
	GrossWeight: null,
	NetWeight: null,
	Volume: null,
	CustomerExpenseTotal: null,
	PurchaseTotal: null,
	AmountTotal: null,
	UsdConversion: null,
	CommissionAmount: null,
	InsuranceCost: null,
	OtherCost: null,
	FreightCost: null,
	TotalCost: null,
	ExchangeCost: null,
	ProductProfit: null,
	ProfitRate: null,
	Remark: '',
	IsDelete: 0,
	FreightCurrency: null,
	FreightQuote: 0,
	Courierfee: 0,
	TaxRefundTotal: null,
	Totalgrossprofit: null,
	totalOtherFees: null,
	Totalprofitmargin: null,
	ProfitAmount: null,
	portMiscellaneousFees: null,
	// 合同产品项数组
	contractProductItems: [] as Array<{
		Id: number,
		ProductID: number,
		ContractId: number,
		ProductCode: string,
		CustomerCode: string,
		ChineseName: string,
		ChineseSpec: string,
		EnglishName: string,
		ContractQuantity: number,
		Unit: number,
		ExportUnitPrice: number,
		ExportTotalPrice: number,
		PurchaseUnitPrice: number,
		PurchaseTotalPrice: number,
		Packaging: number,
		SpecialRequirements: string,
		Invoice: number,
		TaxRefundRate: number,
		InnerBoxQuantity: number,
		OuterBoxQuantity: number,
		BoxCount: number,
		OuterBoxLength: number,
		OuterBoxWidth: number,
		OuterBoxHeight: number,
		OuterBoxVolume: number,
		OuterBoxNetWeight: number,
		OuterBoxGrossWeight: number,
		TotalVolume: number,
		TotalNetWeight: number,
		TotalGrossWeight: number,
		additionalPackagingCosts: number,
		Inlandfreightforasingleproduct: number,
		Inlandfreightprice: number,
		IsNewProduct: number,
		Oceanfreightforasingleproduct: number,
		Onepacking: string,
		OtherFees: number,
		outerboxunit: string,
		Portchargesforindividualproducts: number,
		Purchasecurrency: number,
		Remark: string,
		singleProductGrossProfit: number,
		singleProductGrossProfitTotal: number,
		Singleproductvolume: number,
		Singlesalesrevenue: number
	}>,

	// 合同费用项数组
	contractExpensesItems: [] as Array<{
		Id: number,
		ContractId: number,
		ExpenseName: string,
		Currency: number,
		ExchangeRate: number,
		Expense: number,
		Remark: string
	}>
});
const SaveContract = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid) => {
		if (valid) {
			ElMessageBox.confirm('确定保存合同信息吗?', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				// 映射基本信息
				addContractsRequest.Id = 0; // 新增时ID为0
				addContractsRequest.ContractNumber = Newcontractform.contractNumber;
				addContractsRequest.ContractDate = Newcontractform.contractDate;
				addContractsRequest.ContractStatus = Newcontractform.contractStatus;
				addContractsRequest.CustomerId = Newcontractform.customerid;
				addContractsRequest.CustomerNumber = Newcontractform.customerNumber;
				addContractsRequest.CustomerAbbreviation = Newcontractform.customerAbbreviation;
				addContractsRequest.ContactPerson = Newcontractform.contactPerson;
				addContractsRequest.ContactEmail = Newcontractform.contactEmail;
				addContractsRequest.EffectiveDate = Newcontractform.effectiveDate;
				addContractsRequest.CustomerLevel = Newcontractform.customerLevel;
				addContractsRequest.CustomerContract = Newcontractform.customerContract;
				addContractsRequest.DeliveryDate = Newcontractform.deliveryDate;
				addContractsRequest.OurCompany = Newcontractform.ourCompany;
				addContractsRequest.SettlementType = Newcontractform.settlementType;
				addContractsRequest.ForeignCurrency = Newcontractform.foreignCurrency;
				addContractsRequest.ExchangeRate = Newcontractform.exchangeRate;
				addContractsRequest.UsdExchangeRate = Newcontractform.usdExchangeRate;
				addContractsRequest.SettlementMethod = Newcontractform.settlementMethod;
				addContractsRequest.PriceTerms = Newcontractform.priceTerms;
				addContractsRequest.ShippingPort = Newcontractform.shippingPort;
				addContractsRequest.DestinationPort = Newcontractform.destinationPort;
				addContractsRequest.TradeCountry = Newcontractform.tradeCountry;
				addContractsRequest.Transportation = Newcontractform.transportation;
				addContractsRequest.Salesperson = Newcontractform.salesperson;
				addContractsRequest.HasDeposit = Newcontractform.hasDeposit;
				addContractsRequest.ReceivedDeposit = Newcontractform.receivedDeposit;
				addContractsRequest.DepositDate = Newcontractform.depositDate;
				addContractsRequest.Depositratio = Newcontractform.Depositratio;
				addContractsRequest.CustomerOrder = Newcontractform.customerOrder;
				addContractsRequest.SigningLocation = Newcontractform.signingLocation;
				addContractsRequest.InsuranceAddition = Newcontractform.insuranceAddition;
				addContractsRequest.InsuranceRate = Newcontractform.insuranceRate;
				addContractsRequest.CommissionRate = Newcontractform.commissionRate;
				addContractsRequest.ShippingCurrency = Newcontractform.shippingCurrency;
				addContractsRequest.ShippingExchangeRate = Newcontractform.shippingrate;
				addContractsRequest.ShippingCost = Newcontractform.oceanFreight;
				addContractsRequest.freightForwarderCustomsClearanceFees = Newcontractform.freightForwarderCustomsClearanceFees;
				addContractsRequest.ReceivingBank = Newcontractform.receivingBank;
				addContractsRequest.BankCost = Newcontractform.BankFee;
				addContractsRequest.DocumentationFees = Newcontractform.DocumentationFees;
				addContractsRequest.PaymentDate = Newcontractform.paymentDate;
				addContractsRequest.GoodsValue = Newcontractform.TotalValueOfGoods;
				addContractsRequest.Quantity = Newcontractform.TotalQuantity;
				addContractsRequest.BoxCount = Newcontractform.TotalNumberOfBoxes;
				addContractsRequest.GrossWeight = Newcontractform.TotalGrossWeight;
				addContractsRequest.NetWeight = Newcontractform.TotalNetWeight;
				addContractsRequest.Volume = Newcontractform.TotalVolume;
				addContractsRequest.PurchaseTotal = Newcontractform.TotalPurchases;
				addContractsRequest.TaxRefundTotal = Newcontractform.TotalTaxRefund;
				addContractsRequest.CustomerExpenseTotal = Newcontractform.customerExpenseTotal;
				addContractsRequest.AmountTotal = Newcontractform.amountTotal;
				addContractsRequest.UsdConversion = Newcontractform.usdConversion;
				addContractsRequest.Totalgrossprofit = Newcontractform.Totalgrossprofit;
				addContractsRequest.totalOtherFees = Newcontractform.TotalOtherFees;
				addContractsRequest.ProfitAmount = Newcontractform.ProfitAmount;
				addContractsRequest.Totalprofitmargin = Newcontractform.Totalprofitmargin;
				addContractsRequest.CommissionAmount = Newcontractform.commissionAmount;
				addContractsRequest.InsuranceCost = Newcontractform.insuranceCost;
				addContractsRequest.FreightCost = Newcontractform.freightCost;
				addContractsRequest.TotalCost = Newcontractform.totalCost;
				addContractsRequest.ExchangeCost = Newcontractform.exchangeCost;
				addContractsRequest.ProductProfit = Newcontractform.productProfit;
				addContractsRequest.ProfitRate = Newcontractform.profitRate;
				addContractsRequest.FreightCurrency = Newcontractform.FreightCurrency;
				addContractsRequest.FreightQuote = Newcontractform.FreightQuote;
				addContractsRequest.Courierfee = Newcontractform.Courierfee;
				addContractsRequest.IsDelete = 0;
				addContractsRequest.Remark = Newcontractform.contractremark;
				addContractsRequest.portMiscellaneousFees = Newcontractform.portMiscellaneousFees;
				addContractsRequest.CanPartial = Number(Newcontractform.canPartial) === 1 ? 1 : 0;
				addContractsRequest.CanTransit = Number(Newcontractform.canTransit) === 1 ? 1 : 0;
				// 映射产品信息
				addContractsRequest.contractProductItems = productData.value.map(item => ({
					Id: 0,
					ProductID: item.productID,
					ContractId: 0,
					ProductCode: item.productNum,
					CustomerCode: item.customerNum,
					ChineseName: item.cproductname,
					ChineseSpec: item.cspecification,
					EnglishName: '',
					ContractQuantity: item.contractQuantity,
					Unit: item.unitofmeasurement,
					ExportUnitPrice: item.exportunitprice,
					ExportTotalPrice: item.exporttotalprice,
					PurchaseUnitPrice: item.purchaseunitprice,
					PurchaseTotalPrice: item.purchaseunitprice * item.contractQuantity,
					Packaging: item.packaging,
					SpecialRequirements: item.specialrequirements,
					Invoice: item.isInvoicingc === 'Y' ? 1 : 0,
					TaxRefundRate: item.rebaterate,
					InnerBoxQuantity: item.innerBoxLoading,
					OuterBoxQuantity: item.outerboxloading,
					BoxCount: item.NumberOfBoxes,
					OuterBoxLength: item.outerboxlength,
					OuterBoxWidth: item.outerboxwidth,
					OuterBoxHeight: item.outerboxheight,
					OuterBoxVolume: item.outerboxvolume,
					OuterBoxNetWeight: item.outerboxnetweight,
					OuterBoxGrossWeight: item.outerboxgrossweight,
					TotalVolume: item.totalVolume,
					TotalNetWeight: item.totalNetWeight,
					TotalGrossWeight: item.totalGrossWeight,
					Remark: item.remark,
					additionalPackagingCosts: item.AdditionalPackagingCosts,
					grossProfitRate: item.grossProfitRate,
					Inlandfreightforasingleproduct: item.Inlandfreightforasingleproduct,
					Inlandfreightprice: item.inlandfreightprice,
					IsNewProduct: item.isImported ? 1 : 0,
					Oceanfreightforasingleproduct: item.Oceanfreightforasingleproduct,
					Onepacking: item.AdditionalPackagingCosts,
					OtherFees: item.OtherFees,
					outerboxunit: item.outerboxunit,
					Portchargesforindividualproducts: item.Portchargesforindividualproducts,
					Purchasecurrency: item.purchasecurrency,
					singleProductGrossProfit: item.singleProductGrossProfit,
					singleProductGrossProfitTotal: item.singleProductGrossProfitTotal,
					Singleproductvolume: item.Singleproductvolume,
					Singlesalesrevenue: item.SinglesalesrevenueA
				}));

				// 映射费用信息
				addContractsRequest.contractExpensesItems = CustomerRelaterExoensesTableData.value.map(item => ({
					Id: 0,
					ContractId: 0,
					ExpenseName: item.expenseName,
					Currency: item.currency,
					ExchangeRate: item.exchangeRate,
					Expense: item.expense,
					Remark: item.remark
				}));

				// 发送请求
				request.post('Contracts/AddContracts/Add', addContractsRequest)
					.then(response => {
						if (response != null) {
							ElMessage({
								message: response.msg,
								type: 'success'
							});
							contractDialog.value = false;
							GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
							isAuditBtnShow.value = true;
						}
					})
					.catch(error => {
						console.error('新增销售合同出错！😔错误内容：', error);
						ElMessage.error('保存失败：' + error.message);
					});
			});
		} else {
			ElMessage.error('请检查销售合同基本信息是否填写完整')
		}
	})
}

const clearAll = () => {
	Newcontractform.contractNumber = '';
	Newcontractform.contractDate = '';
	Newcontractform.contractStatus = null;
	Newcontractform.customerid = null;
	Newcontractform.customerNumber = null;
	Newcontractform.customerAbbreviation = '';
	Newcontractform.contactPerson = null;
	Newcontractform.contactEmail = '';
	Newcontractform.effectiveDate = '';
	Newcontractform.customerLevel = null;
	Newcontractform.customerContract = '';
	Newcontractform.deliveryDate = '';
	Newcontractform.ourCompany = null;
	Newcontractform.settlementType = null;
	Newcontractform.foreignCurrency = null;
	Newcontractform.exchangeRate = null;
	Newcontractform.usdExchangeRate = null;
	Newcontractform.settlementMethod = null;
	Newcontractform.priceTerms = null;
	Newcontractform.shippingPort = null;
	Newcontractform.destinationPort = null;
	Newcontractform.tradeCountry = null;
	Newcontractform.transportation = null;
	Newcontractform.salesperson = null;
	Newcontractform.hasDeposit = false;
	Newcontractform.receivedDeposit = null;
	Newcontractform.depositDate = '';
	Newcontractform.stockProgress = '';
	Newcontractform.deliveryProgress = '';
	Newcontractform.profitCalculation = '';
	Newcontractform.estimatedProfitMargin = '';
	Newcontractform.customerOrder = '';
	Newcontractform.contractremark = '';
	Newcontractform.signingLocation = null;
	Newcontractform.canPartial = null;
	Newcontractform.canTransit = null;
	Newcontractform.insuranceAddition = null;
	Newcontractform.insuranceRate = null;
	Newcontractform.commissionRate = null;
	Newcontractform.receivingBank = '';
	Newcontractform.goodsValue = null;
	Newcontractform.quantity = null;
	Newcontractform.boxCount = null;
	Newcontractform.grossWeight = null;
	Newcontractform.netWeight = null;
	Newcontractform.volume = null;
	Newcontractform.TotalPurchases = null;
	Newcontractform.TotalTaxRefund = null;
	Newcontractform.customerExpenseTotal = null;
	Newcontractform.amountTotal = null;
	Newcontractform.usdConversion = null;
	Newcontractform.commissionAmount = null;
	Newcontractform.shippingCurrency = null;
	Newcontractform.shippingCurrency = null;
	Newcontractform.shippingCost = null;
	Newcontractform.insuranceCost = null;
	Newcontractform.BankFee = null;
	Newcontractform.TotalOtherFees = null;
	Newcontractform.freightCost = null;
	Newcontractform.freightForwarderCustomsClearanceFees = null;
	Newcontractform.DocumentationFees = null;
	Newcontractform.ProfitAmount = null;
	Newcontractform.Totalprofitmargin = null;
	Newcontractform.oceanFreight = null;
	Newcontractform.shippingrate = null;
	Newcontractform.portMiscellaneousFees = null;
	Newcontractform.TotalValueOfGoods = null;
	Newcontractform.TotalQuantity = null;
	Newcontractform.TotalNumberOfBoxes = null;
	Newcontractform.TotalGrossWeight = null;
	Newcontractform.TotalNetWeight = null;
	Newcontractform.TotalVolume = null;
	Newcontractform.Totalgrossprofit = null;
	productData.value = [];
	CustomerRelaterExoensesTableData.value = [];

}

//获取销售合同编号
const GetContractNumber = () => {
	request({
		url: 'Contracts/GetNextContractNumber/GetContractNumber',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			Newcontractform.contractNumber = response.data;
		} else {
			console.error('获取销售合同编号出错');
		}
	}).catch(error => {
		console.error('获取销售合同编号出错！😔错误内容：', error);
	})
}

const openContractDialog = () => {
	clearAll();
	contractDialog.value = true;
	isDisabled.value = false;
	showEditBtn.value = false;
	showEditSaveBtn.value = false;
	isSaveBtnShow.value = true;
	const currentDate = new Date();
	const formattedDate = currentDate.toISOString().split('T')[0];
	Newcontractform.contractDate = formattedDate;
	Newcontractform.contractStatus = state.optionss['hr_contract_status'].filter(item => item.dictValue == 1).map(item => item.dictValue).values().next().value;
	Newcontractform.hasDeposit = false;
	Newcontractform.salesperson = state.optionss['sql_hr_sale'].filter(item => item.dictValue == userId.toString()).map(item => item.dictValue).values().next().value;
	GetContractNumber();

}

const SelctedContractId = ref(0);
const contractReviewStatus = ref(null);
const checkContractsDetails = (row) => {
	contractReviewStatus.value = row.contractReviewStatusStr;
	isDisabled.value = true;
	contractDialog.value = true;
	if (row.reviewStatus == 0 || row.reviewStatus == 3) {
		isSaveBtnShow.value = false;
		showEditSaveBtn.value = false;
		showEditBtn.value = true;
		isAuditBtnShow.value = true;
	} else {
		showEditSaveBtn.value = false;
		isSaveBtnShow.value = false;
		showEditBtn.value = false;
		isAuditBtnShow.value = false;
	}
	productData.value = [];
	CustomerRelaterExoensesTableData.value = [];
	/*表单赋值*/
	SelctedContractId.value = row.id;
	Newcontractform.contractNumber = row.contractNumber;
	Newcontractform.contractDate = row.contractDate;
	Newcontractform.contractStatus = row.contractStatus;
	Newcontractform.customerid = row.customerId;
	Newcontractform.customerNumber = state.optionss['sql_hr_customer'].find(item => item.dictValue === row.customerId.toString()).dictValue;
	Newcontractform.customerAbbreviation = state.optionss['sql_hr_customer'].find(item => item.dictValue === row.customerId.toString()).dictValue;
	GetCustomerContactPerson(row.customerId);
	Newcontractform.contactPerson = row.contactPerson;
	Newcontractform.contactEmail = row.contactEmail;
	Newcontractform.effectiveDate = row.effectiveDate;
	Newcontractform.customerLevel = state.optionss['hr_customer_level'].find(item => item.dictValue === row.customerLevel.toString()).dictValue;
	Newcontractform.customerContract = row.customerContract;
	Newcontractform.deliveryDate = row.deliveryDate;
	Newcontractform.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictLabel === row.ourCompany.toString()).dictValue;
	Newcontractform.settlementType = row.settlementType.toString();
	Newcontractform.foreignCurrency = state.optionss['hr_export_currency'].find(item => item.dictLabel === row.foreignCurrency.toString()).dictValue;
	Newcontractform.exchangeRate = row.exchangeRate;
	Newcontractform.usdExchangeRate = row.usdExchangeRate;
	Newcontractform.settlementMethod = state.optionss['hr_settlement_way'].find(item => item.dictLabel === row.settlementMethod.toString()).dictValue;
	Newcontractform.priceTerms = state.optionss['hr_pricing_term'].find(item => item.dictLabel === row.priceTerms.toString()).dictValue;
	Newcontractform.shippingPort = state.optionss['hr_transport_port'].find(item => item.dictLabel === row.shippingPort.toString()).dictValue;
	Newcontractform.destinationPort = row.destinationPort;
	Newcontractform.tradeCountry = state.optionss['hr_nation'].find(item => item.dictLabel === row.tradeCountry.toString()).dictValue;
	Newcontractform.transportation = state.optionss['hr_transportation_method'].find(item => item.dictLabel === row.transportation.toString()).dictValue;
	Newcontractform.salesperson = state.optionss['sql_hr_sale'].find(item => item.dictValue === row.salesperson.toString()).dictValue;
	Newcontractform.hasDeposit = row.hasDeposit;
	if (row.hasDeposit) {
		DepositShow.value = true;
		Newcontractform.receivedDeposit = row.receivedDeposit;
		Newcontractform.depositDate = row.depositDate;
		Newcontractform.Depositratio = row.depositratio;
	} else {
		DepositShow.value = false;
	}
	Newcontractform.stockProgress = row.stockProgress;
	Newcontractform.deliveryProgress = row.deliveryProgress;
	Newcontractform.profitCalculation = row.profitCalculation;
	Newcontractform.estimatedProfitMargin = row.estimatedProfitMargin;
	Newcontractform.customerOrder = row.customerOrder;
	Newcontractform.contractremark = row.remark;
	//Newcontractform.signingLocation = state.optionss['hr_signing_place'].find(item => item.dictValue === row.signingLocation.toString()).dictValue;
	Newcontractform.canPartial = state.optionss['sys_yes_no'].find(item => item.dictValue === (row.canPartial.toString() == "true" ? "Y" : "N")).dictValue;
	Newcontractform.canTransit = state.optionss['sys_yes_no'].find(item => item.dictValue === (row.canTransit.toString() == "true" ? "Y" : "N")).dictValue;
	Newcontractform.insuranceAddition = row.insuranceAddition;
	Newcontractform.insuranceRate = row.insuranceRate;
	Newcontractform.commissionRate = row.commissionRate;
	Newcontractform.receivingBank = row.receivingBank;
	Newcontractform.goodsValue = row.goodsValue;
	Newcontractform.oceanFreight = row.shippingCost;
	Newcontractform.shippingCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === row.shippingCurrency.toString()).dictValue;
	Newcontractform.shippingrate = row.shippingExchangeRate;
	Newcontractform.portMiscellaneousFees = row.portMiscellaneousFees;
	Newcontractform.freightForwarderCustomsClearanceFees = row.freightForwarderCustomsClearanceFees;
	Newcontractform.BankFee = row.bankCost;
	Newcontractform.DocumentationFees = row.documentationFees;
	Newcontractform.paymentDate = row.paymentDate;
	Newcontractform.TotalValueOfGoods = row.goodsValue;
	Newcontractform.TotalQuantity = row.quantity;
	Newcontractform.TotalNumberOfBoxes = row.boxCount;
	Newcontractform.TotalGrossWeight = row.grossWeight;
	Newcontractform.TotalNetWeight = row.netWeight;
	Newcontractform.TotalVolume = row.volume;
	Newcontractform.TotalPurchases = row.purchaseTotal;
	Newcontractform.TotalTaxRefund = row.taxRefundTotal;
	Newcontractform.customerExpenseTotal = row.customerExpenseTotal;
	Newcontractform.amountTotal = row.amountTotal;
	Newcontractform.usdConversion = row.usdConversion;
	Newcontractform.commissionAmount = row.commissionAmount;
	Newcontractform.Totalgrossprofit = row.totalgrossprofit;
	Newcontractform.TotalOtherFees = row.totalOtherFees;
	Newcontractform.ProfitAmount = row.profitAmount;
	Newcontractform.Totalprofitmargin = row.totalprofitmargin;
	/*合同产品信息与相关费用*/
	return new Promise((resolve, reject) => {
		request({
			url: 'Contracts/GetContractDetailsById/GetContractDetails',
			method: 'GET',
			params: {
				contractId: row.id
			}
		}).then(response => {
			if (response.data.contractProducts.length > 0) {
				productData.value = [];
				response.data.contractProducts.forEach(element => {
					productData.value.push({
						Id: element.id,
						ProductID: element.productID,
						ContractId: element.contractId,
						productNum: element.productCode,
						CustomerCode: element.customerNum,
						cproductname: element.chineseName,
						cspecification: element.chineseSpec,
						contractQuantity: element.contractQuantity,
						exportunitprice: element.exportUnitPrice,
						exporttotalprice: element.exportTotalPrice,
						unitofmeasurement: state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.unit.toString()).dictValue,
						purchasecurrency: state.optionss['hr_export_currency'].find(item => item.dictValue === element.purchasecurrency.toString()).dictValue,
						purchaseunitprice: element.purchaseUnitPrice,
						inlandfreightprice: element.inlandfreightprice,
						AdditionalPackagingCosts: element.additionalPackagingCosts,
						singleProductGrossProfit: element.singleProductGrossProfit,
						singleProductGrossProfitTotal: element.singleProductGrossProfitTotal,
						grossProfitRate: element.grossProfitRate,
						isInvoicingc: element.invoice == 0 ? "2" : state.optionss['hr_yes_no'].find(item => item.dictValue === element.invoice.toString()).dictValue,
						packaging: state.optionss['hr_packing'].find(item => item.dictValue === element.packaging.toString()).dictValue,
						specialrequirements: element.specialRequirements,
						rebaterate: element.taxRefundRate,
						innerBoxLoading: element.innerBoxQuantity,
						outerboxloading: element.outerBoxQuantity,
						outerboxunit: state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.outerboxunit.toString()).dictValue,
						outerboxlength: element.outerBoxLength,
						outerboxwidth: element.outerBoxWidth,
						outerboxheight: element.outerBoxHeight,
						outerboxnetweight: element.outerBoxNetWeight,
						outerboxgrossweight: element.outerBoxGrossWeight,
						outerboxvolume: element.outerBoxVolume,
						NumberOfBoxes: element.boxCount,
						totalNetWeight: element.totalNetWeight,
						totalGrossWeight: element.totalGrossWeight,
						totalVolume: element.totalVolume,
						OtherFees: element.otherFees,
						SinglesalesrevenueA: element.singlesalesrevenue,
						Singleproductvolume: element.singleproductvolume,
						Portchargesforindividualproducts: element.portchargesforindividualproducts,
						Oceanfreightforasingleproduct: element.oceanfreightforasingleproduct,
						Inlandfreightforasingleproduct: element.inlandfreightforasingleproduct,
						isPriceChanged: element.isPriceChanged
					});
				});
			}
			if (response.data.contractExpenses.length > 0) {
				CustomerRelaterExoensesTableData.value = [];
				CustomerRelaterExoensesTableData.value = response.data.contractExpenses;
				CustomerRelaterExoensesTableData.value.forEach(item => {
					item.currency = state.optionss['hr_export_currency'].find(x => x.dictValue === item.currency?.toString())?.dictValue || '';
					item.amount = item.expense * item.exchangeRate;
				});
			}
			getReceivedExpenseDetails(row.id);
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});

}

const EditContract = () => {
	isSaveBtnShow.value = false;
	showEditBtn.value = false;
	showEditSaveBtn.value = true;
	isDisabled.value = false;
	isAuditBtnShow.value = false;
}

const EditContractsRequest = reactive({
	Id: null,
	CustomerId: null,
	ContractNumber: '',
	ContractDate: '',
	ContractStatus: null,
	CustomerID: null,
	CustomerNumber: '',
	CustomerAbbreviation: '',
	ContactPerson: null,
	ContactEmail: '',
	EffectiveDate: '',
	CustomerLevel: null,
	CustomerContract: null,
	DeliveryDate: '',
	OurCompany: null,
	SettlementType: null,
	ForeignCurrency: null,
	ExchangeRate: null,
	UsdExchangeRate: null,
	SettlementMethod: null,
	PriceTerms: null,
	ShippingPort: null,
	DestinationPort: null,
	TradeCountry: null,
	Transportation: null,
	Salesperson: null,
	HasDeposit: null,
	ReceivedDeposit: null,
	DepositDate: '',
	CustomerOrder: '',
	Remarks: '',
	SigningLocation: null,
	CanPartial: null,
	CanTransit: null,
	InsuranceAddition: null,
	InsuranceRate: null,
	CommissionRate: null,
	ReceivingBank: '',
	PaymentDate: '',
	GoodsValue: null,
	Quantity: null,
	BoxCount: null,
	GrossWeight: null,
	NetWeight: null,
	Volume: null,
	PurchaseTotal: null,
	TaxRefundTotal: null,
	CustomerExpenseTotal: null,
	AmountTotal: null,
	UsdConversion: null,
	CommissionAmount: null,
	ShippingCurrency: null,
	ShippingExchangeRate: null,
	ShippingCost: null,
	InsuranceCost: null,
	BankCost: null,
	OtherCost: null,
	FreightCost: null,
	TotalCost: null,
	ExchangeCost: null,
	ProductProfit: null,
	ProfitAmount: null,
	ProfitRate: null,
	Remark: '',
	Depositratio: null,
	freightForwarderCustomsClearanceFees: null,
	DocumentationFees: null,
	Totalgrossprofit: null,
	totalOtherFees: null,
	Totalprofitmargin: null,
	FreightCurrency: null,
	FreightQuote: null,
	Courierfee: null,
	IsDelete: null,
	portMiscellaneousFees: null,
	contractProductItems: [],
	contractExpensesItems: []
});
const EditContractSave = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid) => {
		if (valid) {
			//#region 编辑保存合同信息
			ElMessageBox.confirm('确定保存已编辑的合同信息吗?', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				EditContractsRequest.Id = SelctedContractId.value;
				EditContractsRequest.ContractNumber = Newcontractform.contractNumber;
				EditContractsRequest.ContractDate = Newcontractform.contractDate;
				EditContractsRequest.ContractStatus = state.optionss['hr_contract_status'].filter(item => item.dictLabel == Newcontractform.contractStatus).map(item => item.dictValue).values().next().value;
				EditContractsRequest.CustomerId = Newcontractform.customerid;
				EditContractsRequest.CustomerNumber = Newcontractform.customerNumber;
				EditContractsRequest.CustomerAbbreviation = Newcontractform.customerAbbreviation;
				EditContractsRequest.ContactPerson = Newcontractform.contactPerson;
				EditContractsRequest.ContactEmail = Newcontractform.contactEmail;
				EditContractsRequest.EffectiveDate = Newcontractform.effectiveDate;
				EditContractsRequest.CustomerLevel = Newcontractform.customerLevel;
				EditContractsRequest.CustomerContract = Newcontractform.customerContract;
				EditContractsRequest.DeliveryDate = Newcontractform.deliveryDate;
				EditContractsRequest.OurCompany = Newcontractform.ourCompany;
				EditContractsRequest.SettlementType = Newcontractform.settlementType;
				EditContractsRequest.ForeignCurrency = Newcontractform.foreignCurrency;
				EditContractsRequest.ExchangeRate = Newcontractform.exchangeRate;
				EditContractsRequest.UsdExchangeRate = Newcontractform.usdExchangeRate;
				EditContractsRequest.SettlementMethod = Newcontractform.settlementMethod;
				EditContractsRequest.PriceTerms = Newcontractform.priceTerms;
				EditContractsRequest.ShippingPort = Newcontractform.shippingPort;
				EditContractsRequest.DestinationPort = Newcontractform.destinationPort;
				EditContractsRequest.TradeCountry = Newcontractform.tradeCountry;
				EditContractsRequest.Transportation = Newcontractform.transportation;
				EditContractsRequest.Salesperson = Newcontractform.salesperson;
				EditContractsRequest.HasDeposit = Newcontractform.hasDeposit;
				EditContractsRequest.ReceivedDeposit = Newcontractform.receivedDeposit;
				EditContractsRequest.DepositDate = Newcontractform.depositDate;
				EditContractsRequest.Depositratio = Newcontractform.Depositratio;
				EditContractsRequest.CustomerOrder = Newcontractform.customerOrder;
				EditContractsRequest.SigningLocation = Newcontractform.signingLocation;
				EditContractsRequest.InsuranceAddition = Newcontractform.insuranceAddition;
				EditContractsRequest.InsuranceRate = Newcontractform.insuranceRate;
				EditContractsRequest.CommissionRate = Newcontractform.commissionRate;
				EditContractsRequest.ShippingCurrency = Newcontractform.shippingCurrency;
				EditContractsRequest.ShippingExchangeRate = Newcontractform.shippingrate;
				EditContractsRequest.ShippingCost = Newcontractform.oceanFreight;
				EditContractsRequest.freightForwarderCustomsClearanceFees = Newcontractform.freightForwarderCustomsClearanceFees;
				EditContractsRequest.ReceivingBank = Newcontractform.receivingBank;
				EditContractsRequest.BankCost = Newcontractform.BankFee;
				EditContractsRequest.DocumentationFees = Newcontractform.DocumentationFees;
				EditContractsRequest.PaymentDate = Newcontractform.paymentDate;
				EditContractsRequest.GoodsValue = Newcontractform.TotalValueOfGoods;
				EditContractsRequest.Quantity = Newcontractform.TotalQuantity;
				EditContractsRequest.BoxCount = Newcontractform.TotalNumberOfBoxes;
				EditContractsRequest.GrossWeight = Newcontractform.TotalGrossWeight;
				EditContractsRequest.NetWeight = Newcontractform.TotalNetWeight;
				EditContractsRequest.Volume = Newcontractform.TotalVolume;
				EditContractsRequest.PurchaseTotal = Newcontractform.TotalPurchases;
				EditContractsRequest.TaxRefundTotal = Newcontractform.TotalTaxRefund;
				EditContractsRequest.CustomerExpenseTotal = Newcontractform.customerExpenseTotal;
				EditContractsRequest.AmountTotal = Newcontractform.amountTotal;
				EditContractsRequest.UsdConversion = Newcontractform.usdConversion;
				EditContractsRequest.Totalgrossprofit = Newcontractform.Totalgrossprofit;
				EditContractsRequest.totalOtherFees = Newcontractform.TotalOtherFees;
				EditContractsRequest.ProfitAmount = Newcontractform.ProfitAmount;
				EditContractsRequest.Totalprofitmargin = Newcontractform.Totalprofitmargin;
				EditContractsRequest.CommissionAmount = Newcontractform.commissionAmount;
				EditContractsRequest.InsuranceCost = Newcontractform.insuranceCost;
				EditContractsRequest.FreightCost = Newcontractform.freightCost;
				EditContractsRequest.TotalCost = Newcontractform.totalCost;
				EditContractsRequest.ExchangeCost = Newcontractform.exchangeCost;
				EditContractsRequest.ProductProfit = Newcontractform.productProfit;
				EditContractsRequest.ProfitRate = Newcontractform.profitRate;
				EditContractsRequest.FreightCurrency = Newcontractform.FreightCurrency;
				EditContractsRequest.FreightQuote = Newcontractform.FreightQuote;
				EditContractsRequest.Courierfee = Newcontractform.Courierfee;
				EditContractsRequest.IsDelete = 0;
				EditContractsRequest.Remark = Newcontractform.contractremark;
				EditContractsRequest.CanPartial = Number(Newcontractform.canPartial) === 1 ? 1 : 0;
				EditContractsRequest.CanTransit = Number(Newcontractform.canTransit) === 1 ? 1 : 0;
				EditContractsRequest.portMiscellaneousFees = Newcontractform.portMiscellaneousFees;
				EditContractsRequest.contractProductItems = [];
				productData.value.forEach(productItem => {
					EditContractsRequest.contractProductItems.push({
						Id: productItem.Id == 0 ? null : productItem.Id,
						ContractId: SelctedContractId.value,
						ProductID: productItem.ProductID,
						ProductCode: productItem.productNum,
						CustomerCode: productItem.customerNum,
						ChineseName: productItem.cproductname,
						ChineseSpec: productItem.cspecification,
						EnglishName: '',
						ContractQuantity: productItem.contractQuantity,
						Unit: productItem.unitofmeasurement,
						ExportUnitPrice: productItem.exportunitprice,
						ExportTotalPrice: productItem.exporttotalprice,
						PurchaseUnitPrice: productItem.purchaseunitprice,
						PurchaseTotalPrice: productItem.purchaseunitprice * productItem.contractQuantity,
						Packaging: productItem.packaging,
						SpecialRequirements: productItem.specialrequirements,
						invoice: productItem.isInvoicingc,
						TaxRefundRate: productItem.rebaterate,
						InnerBoxQuantity: productItem.innerBoxLoading,
						OuterBoxQuantity: productItem.outerboxloading,
						BoxCount: productItem.NumberOfBoxes,
						OuterBoxLength: productItem.outerboxlength,
						OuterBoxWidth: productItem.outerboxwidth,
						OuterBoxHeight: productItem.outerboxheight,
						OuterBoxVolume: productItem.outerboxvolume,
						OuterBoxNetWeight: productItem.outerboxnetweight,
						OuterBoxGrossWeight: productItem.outerboxgrossweight,
						TotalVolume: productItem.totalVolume,
						TotalNetWeight: productItem.totalNetWeight,
						TotalGrossWeight: productItem.totalGrossWeight,
						Remark: productItem.remark,
						additionalPackagingCosts: productItem.AdditionalPackagingCosts,
						Inlandfreightforasingleproduct: productItem.Inlandfreightforasingleproduct,
						Inlandfreightprice: productItem.inlandfreightprice,
						IsNewProduct: productItem.isImported ? 1 : 0,
						Oceanfreightforasingleproduct: productItem.Oceanfreightforasingleproduct,
						Onepacking: productItem.AdditionalPackagingCosts,
						OtherFees: productItem.OtherFees,
						outerboxunit: productItem.outerboxunit,
						Portchargesforindividualproducts: productItem.Portchargesforindividualproducts,
						Purchasecurrency: productItem.purchasecurrency,
						singleProductGrossProfit: productItem.singleProductGrossProfit,
						singleProductGrossProfitTotal: productItem.singleProductGrossProfitTotal,
						Singleproductvolume: productItem.Singleproductvolume,
						Singlesalesrevenue: productItem.SinglesalesrevenueA
					});
				});
				EditContractsRequest.contractExpensesItems = [];
				CustomerRelaterExoensesTableData.value.forEach(expenseItem => {
					EditContractsRequest.contractExpensesItems.push({
						ContractId: expenseItem.contractId,
						ExpenseName: expenseItem.expenseName,
						Currency: expenseItem.currency,
						ExchangeRate: expenseItem.exchangeRate,
						Expense: expenseItem.expense,
						Remark: expenseItem.remark
					});
				});
				console.log(EditContractsRequest);
				request.post('Contracts/EditContract/Edit', EditContractsRequest).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})
						//contractDialog.value = false;
						//SelctedContractId.value = 0;
						GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
						isSaveBtnShow.value = false;
						showEditBtn.value = true;
						showEditSaveBtn.value = false;
						isDisabled.value = true;
						isAuditBtnShow.value = true;
					} else {
						console.error('编辑销售合同出错');
					}
				}).catch(error => {
					console.error('编辑销售合同出错！😔错误内容：', error);
				})

			}).catch(() => {
				ElMessage({
					type: 'info',
					message: '已取消编辑保存销售合同'
				});
			});
			//#endregion 编辑保存合同信息
		} else {
			ElMessage({
				type: 'error',
				message: '请检查销售合同基本信息是否填写完整'
			})
			// Remove the return false
		}
	})
}
const IsOceanFreightEdit = ref(true);
const IsCarTransportationEdit = ref(false);
const IsFastTransferEdit = ref(false);
const FreightChange = () => {
	var priceTerms1 = [1, 2, 3, 4, 5];
	var transportation1 = [1, 2];
	var priceTerms2 = [3, 6];
	var transportation2 = [3];
	if (priceTerms1.includes(Number(Newcontractform.priceTerms)) && transportation1.includes(Number(Newcontractform.transportation))) {
		IsOceanFreightEdit.value = false;
	}
}
// region 设置提醒
// 添加提醒对话框相关的响应式变量
const reminderDialogVisible = ref(false)
const reminderForm = ref({
	title: '',
	content: '',
	reminderTime: null
})

// 打开提醒设置对话框
const openReminderDialog = (row) => {
	reminderDialogVisible.value = true;
	reminderForm.value.title = '销售合同';
	reminderForm.value.content = '合同号：' + row.contractNumber;
}
// 设置提醒
const setReminder = async () => {
	try {
		if (!reminderForm.value.content || !reminderForm.value.reminderTime) {
			ElMessage.warning('请填写完整信息')
			return
		}

		const res = await request({
			url: 'TaskReminder/AddTaskReminder/Add',
			method: 'GET',
			params: {
				userId: '', // 这个参数服务端会自动获取，可以传空
				title: reminderForm.value.title,
				content: reminderForm.value.content,
				reminderTime: reminderForm.value.reminderTime
			}
		})
		if (res.code === 200) {
			ElMessage.success('提醒设置成功')
			reminderDialogVisible.value = false
			// 重置表单
			reminderForm.value = {
				title: '',
				content: '',
				reminderTime: null
			}
		} else {
			ElMessage.error(res.msg || '设置失败')
		}
	} catch (error) {
		console.error('设置提醒失败:', error)
		ElMessage.error('设置提醒失败：' + (error.message || '未知错误'))
	}
}
// endregion 设置提醒
</script>
<style scoped>
/* 基础红色文本 */
.red-text {
	color: red !important;
}

/* el-input 组件的输入框 */
.red-text .el-input__inner {
	color: red !important;
}

/* 禁用状态的 el-input */
.red-text.el-input.is-disabled .el-input__inner {
	color: red !important;
}

/* span 元素 */
.red-text span {
	color: red !important;
}

/* 确保禁用状态下也显示红色 */
.el-input.is-disabled.red-text .el-input__inner {
	-webkit-text-fill-color: red !important;
	color: red !important;
}
</style>
