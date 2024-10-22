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
				<el-table-column fixed="right" label="操作" width="100">
					<template #default="scope">
						<el-button type="text" size="small" @click="checkContractsDetails(scope.row)">查看详情</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="contractsTableDatahandlePageChange"
				:current-page="contractsTableDatacurrentPage" :page-size="contractsTableDatapageSize"
				:total="contractsTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />

		</div>
		<el-dialog v-model="contractDialog" title="创建销售合同" :close-on-click-modal=false style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
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
								<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户简称" prop="customerAbbreviation">
							<el-select v-model="Newcontractform.customerAbbreviation" filterable placeholder="请选择客户简称"
								:disabled="isDisabled" style="width: 300px;" @change="handleCustomerSelection">
								<el-option v-for="dict in optionss.sql_hr_customer_abbreviation" :key="dict.dictCode"
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
					<el-col :span="8">
						<el-form-item label="联系人Email">
							<el-input v-model="Newcontractform.contactEmail" disabled style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="生效日期">
							<el-date-picker v-model="Newcontractform.effectiveDate" type="date" placeholder="请选择生效日期"
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户等级">
							<el-select filterable v-model="Newcontractform.customerLevel" placeholder="请选择客户等级"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="客户合同">
							<el-input v-model="Newcontractform.customerContract" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
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
				</el-row>
				<el-row>
					<el-col :span="8">
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
						<el-form-item label="外销币种" prop="foreignCurrency">
							<el-select filterable v-model="Newcontractform.foreignCurrency" placeholder="请选择外销币种"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="汇率" prop="exchangeRate">
							<el-input v-model="Newcontractform.exchangeRate" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="美金汇率" prop="usdExchangeRate">
							<el-input v-model="Newcontractform.usdExchangeRate" style="width: 300px"
								:disabled="isDisabled"></el-input>
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
							<el-select filterable v-model="Newcontractform.destinationPort" placeholder="请选择目的口岸"
								:disabled="isDisabled" style="width: 300px">
								<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
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
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
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
					<el-col :span="8">
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
						<el-form-item label="佣金比率">
							<el-input v-model="Newcontractform.commissionRate" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="收汇银行">
							<el-input v-model="Newcontractform.receivingBank" style="width: 300px"
								:disabled="isDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="付款日期">
							<el-date-picker v-model="Newcontractform.paymentDate" type="date" placeholder="请选择付款日期"
								:disabled="isDisabled" style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">产品资料&客户相关费用</span>
				<el-divider></el-divider>
				<el-button class="mt-4" type="primary" @click="OpenSearchProcutDialog"
					style="margin-bottom: 10px;">添加产品</el-button>
				<el-button class="mt-4" type="primary" @click="handleAddRow"
					style="margin-bottom: 10px;">添加相关费用</el-button>
				<el-tabs v-model="activeTab" tab-position="top" style="height: 350px; " class="demo-tabs">
					<el-tab-pane label="产品资料" name="productMaterialtab">
						<el-table :data="NewcontractProductTbaleData" style="width: 100%" height="280">
							<el-table-column prop="productCode" label="产品编号" width="120"></el-table-column>
							<el-table-column prop="customerCode" label="客户货号" width="120"></el-table-column>
							<el-table-column prop="chineseName" label="中文品名" width="120"></el-table-column>
							<el-table-column prop="chineseSpec" label="中文规格" width="120"></el-table-column>
							<el-table-column prop="englishName" label="英文品名" width="120"></el-table-column>
							<el-table-column prop="contractQuantity" label="合同数量" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.contractQuantity" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template>
							</el-table-column>
							<el-table-column prop="unit" label="计量单位" width="120">
								<template #default="scope">
									<el-select v-model="scope.row.unit" filterable placeholder="请选择"
										style="width: 100%;">
										<el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="exportUnitPrice" label="外销单价" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.exportUnitPrice" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template>
							</el-table-column>
							<el-table-column prop="exportTotalPrice" label="外销总价" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.exportTotalPrice" disabled />
								</template>
							</el-table-column>
							<el-table-column prop="purchaseUnitPrice" label="采购单价" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.purchaseUnitPrice" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template>
							</el-table-column>
							<el-table-column prop="purchaseTotalPrice" label="采购总价" width="120"></el-table-column>
							<el-table-column prop="packaging" label="包装方式" width="150">
								<template #default="scope">
									<el-select v-model="scope.row.packaging" filterable placeholder="请选择"
										style="width: 100%;">
										<el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="specialRequirements" label="特殊要求" width="200">
								<template #default="scope">
									<el-input v-model="scope.row.specialRequirements" size="small" />
								</template>
							</el-table-column>
							<el-table-column prop="invoice" label="是否开票" width="120">
								<template #default="scope">
									<el-select v-model="scope.row.invoice" filterable placeholder="请选择"
										style="width: 100%;">
										<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="taxRefundRate" label="退税率(%)" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.taxRefundRate" size="small" />
								</template>
							</el-table-column>
							<el-table-column prop="innerBoxQuantity" label="内盒装量" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.innerBoxQuantity" size="small" />
								</template>
							</el-table-column>
							<el-table-column prop="outerBoxQuantity" label="外箱装量" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.outerBoxQuantity" size="small" />
								</template></el-table-column>
							<el-table-column prop="boxCount" label="箱数" width="120"><template #default="scope">
									<el-input v-model="scope.row.boxCount" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="outerBoxUnit" label="外箱单位" width="150">
								<template #default="scope">
									<el-select v-model="scope.row.outerBoxUnit" filterable placeholder="外箱单位"
										style="width: 100%;" :disabled="isDisabled">
										<el-option v-for="dict in optionss.hr_outerbox_unit" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="outerBoxLength" label="外箱长度(CM)" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.outerBoxLength" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="outerBoxWidth" label="外箱宽度(CM)" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.outerBoxWidth" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="outerBoxHeight" label="外箱高度(CM)" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.outerBoxHeight" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="outerBoxVolume" label="外箱体积(m³)" width="120"></el-table-column>
							<el-table-column prop="outerBoxNetWeight" label="外箱净重" width="120">
								<template #default="scope">
									<el-input v-model="scope.row.outerBoxNetWeight" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="outerBoxGrossWeight" label="外箱毛重" width="120"><template
									#default="scope">
									<el-input v-model="scope.row.outerBoxGrossWeight" size="small"
										@input="SingleProductCalculation(scope.row)" />
								</template></el-table-column>
							<el-table-column prop="totalVolume" label="总体积" width="120"></el-table-column>
							<el-table-column prop="totalNetWeight" label="总净重" width="120"></el-table-column>
							<el-table-column prop="totalGrossWeight" label="总毛重" width="120"></el-table-column>
							<el-table-column fixed="right" label="操作" width="100">
								<template #default="scope">
									<el-button type="text" size="small"
										@click="ProductDatahandleDelete(scope.$index)">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</el-tab-pane>
					<el-tab-pane label="客户相关费用" name="CustomerRelaterExoensestab">
						<el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; " height="280">
							<el-table-column prop=" expenseName" label="费用名称" width="150">
								<template #default="{ row }">
									<el-input v-model="row.expenseName" placeholder="输入费用名称" size="small"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="currency" label="币种" width="150">
								<template #default="{ row }">
									<el-select filterable v-model="row.currency" placeholder="选择币种" size="small"
										@change="currencyChange(row)">
										<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</template>
							</el-table-column>
							<el-table-column prop="exchangeRate" label="汇率" width="150">
								<template #default="{ row }">
									<el-input v-model="row.exchangeRate" placeholder="输入汇率" size="small"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="expense" label="费用" width="150">
								<template #default="{ row }">
									<el-input v-model="row.expense" placeholder="输入费用" size="small"
										@input="expenseChange(row)"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="amount" label="金额" width="150">
								<template #default="{ row }">
									<el-input v-model="row.amount" disabled size="small"></el-input>
								</template>
							</el-table-column>
							<el-table-column prop="remark" label="备注" width="150">
								<template #default="{ row }">
									<el-input v-model="row.remark" placeholder="输入备注" size="small"></el-input>
								</template>
							</el-table-column>
							<el-table-column label="操作" width="100" fixed="right">
								<template #default="scope">
									<el-button type="text" size="small"
										@click="handleDelete(scope.$index)">删除</el-button>
								</template>
							</el-table-column>
						</el-table>
					</el-tab-pane>
				</el-tabs>
				<span style="font-size: 20px; font-weight: bold;">合计信息</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8">
						<el-form-item label="货值合计">
							<el-input v-model="Newcontractform.goodsValue" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="数量合计">
							<el-input v-model="Newcontractform.quantity" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="箱数合计">
							<el-input v-model="Newcontractform.boxCount" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="毛重合计">
							<el-input v-model="Newcontractform.grossWeight" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="净重合计">
							<el-input v-model="Newcontractform.netWeight" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="体积合计">
							<el-input v-model="Newcontractform.volume" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="采购合计">
							<el-input v-model="Newcontractform.purchaseTotal" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="退税总额">
							<el-input v-model="Newcontractform.taxRefundTotal" style="width: 300px" disabled></el-input>
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
							<el-input v-model="Newcontractform.amountTotal" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="美金/欧元换算">
							<el-input v-model="Newcontractform.usdConversion" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">利润预估</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8">
						<el-form-item label="佣金金额">
							<el-input v-model="Newcontractform.commissionAmount" style="width: 300px"
								disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="海运费币种">
							<el-input v-model="Newcontractform.shippingCurrency" style="width: 300px"
								:disabled="IsOceanFreightEdit"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="海运费汇率">
							<el-input v-model="Newcontractform.shippingExchangeRate" style="width: 300px"
								:disabled="IsOceanFreightEdit"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="海运费">
							<el-input v-model="Newcontractform.shippingCost" style="width: 300px"
								:disabled="IsOceanFreightEdit"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="保险费用">
							<el-input v-model="Newcontractform.insuranceCost" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="银行费用">
							<el-input v-model="Newcontractform.bankCost" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="其它费用">
							<el-input v-model="Newcontractform.otherCost" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="运杂费">
							<el-input v-model="Newcontractform.freightCost" style="width: 300px"
								:disabled="IsCarTransportationEdit"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="成本总额">
							<el-input v-model="Newcontractform.totalCost" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="换汇成本">
							<el-input v-model="Newcontractform.exchangeCost" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品毛利">
							<el-input v-model="Newcontractform.productProfit" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="利润金额">
							<el-input v-model="Newcontractform.profitAmount" style="width: 300px" disabled></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="利润率">
							<el-input v-model="Newcontractform.profitRate" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="运杂费币种">
							<el-input v-model="Newcontractform.FreightCurrency" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="运杂费报价">
							<el-input v-model="Newcontractform.FreightQuote" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="快递费">
							<el-input v-model="Newcontractform.Courierfee" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">备注信息</span>
				<el-divider></el-divider>
				<el-input v-model="Newcontractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }"
					type="textarea" placeholder="请输入备注信息" />
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" v-show="isSaveBtnShow" @click="SaveContract(NewcontractformRef)">
						确定保存
					</el-button>
					<el-button type="primary" v-show="showEditBtn" @click="EditContract">
						编辑
					</el-button>
					<el-button type="primary" v-show="showEditSaveBtn" @click="EditContractSave">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="SearchProcutDialog" title="选择产品" :close-on-click-modal=false :width="'50%'"
			@close="handleCloseSearchProcutDialog">
			<!-- <span style="font-size: 20px; font-weight: bold;">基本信息</span>
		<el-divider></el-divider> -->
			<el-input v-model="searchProductNameText" placeholder="请输入产品关键字进行搜索" style="margin-bottom: 10px;"
				@input="searchProductNameTextChange" />
			<el-table :data="productDatatwo" style="width: 100%"
				@selection-change="productDatatwohandleSelectionChange">
				<el-table-column type="selection" width="55" />
				<el-table-column prop="productCode" label="产品编号" width="120" />
				<el-table-column prop="chineseProductName" label="中文品名" width="120" />
				<el-table-column prop="englishProductName" label="英文品名" width="120" />
				<el-table-column prop="chineseSpecification" label="中文规格" width="120" />
				<el-table-column prop="englishSpecification" label="英文规格" width="120" />
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
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn,
	ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile,
	UploadFile, FormInstance, FormRules
} from 'element-plus'
import { FOCUSABLE_CHILDREN } from 'element-plus/es/directives/trap-focus';
import request from '@/utils/request';
import { number } from 'echarts';
import { Edit } from '@element-plus/icons-vue/dist/types';
import { dataScope } from '@/api/system/role';
import { JsonHubProtocol } from '@microsoft/signalr';
import { get } from 'sortablejs';
import useUserStore from "@/store/modules/user";

var userId = useUserStore().userId;

//允许编辑
const isSaveBtnShow = ref(true);
const showEditBtn = ref(false);
const showEditSaveBtn = ref(false);
const isDisabled = ref(false);

//产品资料tab&客户相关费用tab
const activeTab = ref('productMaterialtab');
const CustomerRelaterExoensesTableData = ref([]);

//产品资料表格
const NewcontractProductTbaleData = ref([]);

//查找产品窗体
const SearchProcutDialog = ref(false)
//查询产品信息表格
const productDatatwo = ref([])
//报价产品表格
const productData = ref([])
//分页组件
const SearchProducttotalItems = ref(0);
const SearchProductcurrentPage = ref(1);
const SearchProductpageSize = ref(10);
const searchProductNameText = ref('');
const SearchProducthandlePageChange = async (newPage) => {
	SearchProductpageSize.value = newPage;
	const start = newPage;
	const end = SearchProductpageSize.value;
	const newData = await GetProductInfoList(start, end);
};
GetProductInfoList(SearchProductcurrentPage.value, SearchProductpageSize.value);
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
	GetProductInfoList(SearchProductcurrentPage.value, SearchProductpageSize.value,
		searchProductNameText.value);
}

const productDatatwohandleSelectionChange = (selectedRows) => {
	selectedRows.forEach((item) => {
		const existingProduct = NewcontractProductTbaleData.value.find(p => p.productCode === item.productCode);
		if (!existingProduct) {
			NewcontractProductTbaleData.value.push({
				productCode: item.productCode,
				customerCode: item.customerCode ? item.customerCode : '',
				chineseName: item.chineseProductName,
				chineseSpec: item.chineseSpecification,
				englishName: item.englishProductName,
				contractQuantity: 0,
				unit: item.unitOfMeasurement,
				exportUnitPrice: 0,
				exportTotalPrice: 0,
				purchaseUnitPrice: 0,
				purchaseTotalPrice: 0,
				packaging: 0,
				specialRequirements: 0,
				invoice: 0,
				taxRefundRate: 0,
				innerBoxQuantity: 0,
				outerBoxQuantity: 0,
				boxCount: 0,
				outerBoxUnit: '',
				outerBoxLength: 0,
				outerBoxWidth: 0,
				outerBoxHeight: 0,
				outerBoxVolume: 0,
				outerBoxNetWeight: 0,
				outerBoxGrossWeight: 0,
				totalVolume: 0,
				totalNetWeight: 0,
				totalGrossWeight: 0
			});
		}
	});
}
const deleteRow = (index: number) => {
	productData.value.splice(index, 1)
}

const OpenSearchProcutDialog = () => {
	activeTab.value = 'productMaterialtab';
	GetProductInfoList(SearchProductcurrentPage.value, SearchProductpageSize.value);
	SearchProcutDialog.value = true;
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
	destinationPort: number,//目的口岸
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
	/*辅助信息End*/
	/*合计信息Start*/
	goodsValue: number,//货值合计
	quantity: number,//数量合计
	boxCount: number,//箱数合计
	grossWeight: number,//毛重合计
	netWeight: number,//净重合计
	volume: number,//体积合计
	purchaseTotal: number,//采购合计
	taxRefundTotal: number,//退税总额
	customerExpenseTotal: number,//客户费用合计
	amountTotal: number,//金额合计
	usdConversion: number//美金/欧元换算
	/*合计信息End*/
	/*利润预估Start*/
	commissionAmount: number,//佣金金额
	shippingCurrency: number,//海运费币种
	shippingExchangeRate: number,//海运费汇率
	shippingCost: number,//海运费
	insuranceCost: number,//保险费用
	bankCost: number,//银行费用
	otherCost: number,//其它费用
	freightCost: number,//运杂费
	totalCost: number,//成本总额
	exchangeCost: number,//换汇成本
	productProfit: number,//产品毛利
	profitAmount: number,//利润金额
	profitRate: number,//利润率
	FreightCurrency: number,//运杂费币种
	FreightQuote: number,//运杂费报价
	Courierfee: number//快递费
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
		// 如果勾选了“有定金”，则添加验证规则
		rules.receivedDeposit = [{ required: true, message: '请输入已收定金', trigger: 'change,blur' }];
		rules.depositDate = [{ required: true, message: '请选择定金日期', trigger: 'change,blur' }];
		rules.Depositratio = [{ required: true, message: '请输入定金比例', trigger: 'change,blur' }];
	} else if (val == false) {
		Newcontractform.receivedDeposit = null;
		// 如果取消勾选“有定金”，则移除验证规则
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
	{ dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' },
	{ dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
	{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' },
	{ dictType: 'hr_packing' }, { dictType: 'sql_hr_customer_abbreviation' },
	{ dictType: 'hr_outerbox_unit' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})
/*动态下拉框end*/
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
			Newcontractform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == response.customerLevel).map(item => item.dictValue).values().next().value;
			Newcontractform.priceTerms = state.optionss['hr_pricing_term'].filter(item => item.dictValue == response.pricingTerm).map(item => item.dictValue).values().next().value;
			Newcontractform.settlementType = state.optionss['hr_settlement_way'].filter(item => item.dictValue == response.settlementWay).map(item => item.dictValue).values().next().value;
			Newcontractform.tradeCountry = state.optionss['hr_nation'].filter(item => item.dictValue == response.tradingCountry).map(item => item.dictValue).values().next().value;
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
}
const ProductDatahandleDelete = (index) => {
	NewcontractProductTbaleData.value.splice(index, 1);
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
	row.amount = row.expense * row.exchangeRate;
}

const addContractsRequest = reactive({
	Id: null,
	ContractNumber: '',
	ContractDate: '',
	ContractStatus: null,
	CustomerID: null,
	CustomerNumber: null,
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
	HasDeposit: 0,
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
	contractProductItems: [],
	contractExpensesItems: []
});
const SaveContract = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			//#region  保存合同信息
			ElMessageBox.confirm('确定保存合同信息吗?', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
				//合同主体信息
				addContractsRequest.ContractNumber = Newcontractform.contractNumber;
				addContractsRequest.ContractDate = Newcontractform.contractDate;
				addContractsRequest.ContractStatus = Newcontractform.contractStatus;
				addContractsRequest.CustomerID = Newcontractform.customerid;
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
				addContractsRequest.HasDeposit = Newcontractform.hasDeposit ? 1 : 0;
				addContractsRequest.ReceivedDeposit = Newcontractform.receivedDeposit;
				addContractsRequest.DepositDate = Newcontractform.depositDate;
				addContractsRequest.CustomerOrder = Newcontractform.customerOrder;
				addContractsRequest.Remarks = '';
				addContractsRequest.SigningLocation = Newcontractform.signingLocation;
				addContractsRequest.CanPartial = Newcontractform.canPartial;
				addContractsRequest.CanTransit = Newcontractform.canTransit;
				addContractsRequest.InsuranceAddition = Newcontractform.insuranceAddition;
				addContractsRequest.InsuranceRate = Newcontractform.insuranceRate;
				addContractsRequest.CommissionRate = Newcontractform.commissionRate;
				addContractsRequest.ReceivingBank = Newcontractform.receivingBank;
				addContractsRequest.PaymentDate = null;
				addContractsRequest.GoodsValue = Newcontractform.goodsValue;
				addContractsRequest.Quantity = Newcontractform.quantity;
				addContractsRequest.BoxCount = Newcontractform.boxCount;
				addContractsRequest.GrossWeight = Newcontractform.grossWeight;
				addContractsRequest.NetWeight = Newcontractform.netWeight;
				addContractsRequest.Volume = Newcontractform.volume;
				addContractsRequest.PurchaseTotal = Newcontractform.purchaseTotal;
				addContractsRequest.TaxRefundTotal = Newcontractform.taxRefundTotal;
				addContractsRequest.CustomerExpenseTotal = Newcontractform.customerExpenseTotal;
				addContractsRequest.AmountTotal = Newcontractform.amountTotal;
				addContractsRequest.UsdConversion = Newcontractform.usdConversion;
				addContractsRequest.CommissionAmount = Newcontractform.commissionAmount;
				addContractsRequest.ShippingCurrency = Newcontractform.shippingCurrency;
				addContractsRequest.ShippingExchangeRate = Newcontractform.shippingExchangeRate;
				addContractsRequest.ShippingCost = Newcontractform.shippingCost;
				addContractsRequest.InsuranceCost = Newcontractform.insuranceCost;
				addContractsRequest.BankCost = Newcontractform.bankCost;
				addContractsRequest.OtherCost = Newcontractform.otherCost;
				addContractsRequest.FreightCost = Newcontractform.freightCost;
				addContractsRequest.TotalCost = Newcontractform.totalCost;
				addContractsRequest.ExchangeCost = Newcontractform.exchangeCost;
				addContractsRequest.ProductProfit = Newcontractform.productProfit;
				addContractsRequest.ProfitAmount = Newcontractform.profitAmount;
				addContractsRequest.ProfitRate = Newcontractform.profitRate;
				addContractsRequest.Remark = '';
				NewcontractProductTbaleData.value.forEach(productItem => {
					addContractsRequest.contractProductItems.push({
						ContractId: 0,
						ProductCode: productItem.productCode,
						CustomerCode: productItem.customerCode,
						ChineseName: productItem.chineseName,
						ChineseSpec: productItem.chineseSpec,
						EnglishName: productItem.englishName,
						ContractQuantity: productItem.contractQuantity,
						Unit: productItem.unit,
						ExportUnitPrice: productItem.exportUnitPrice,
						ExportTotalPrice: productItem.exportTotalPrice,
						PurchaseUnitPrice: productItem.purchaseUnitPrice,
						PurchaseTotalPrice: productItem.purchaseTotalPrice,
						Packaging: productItem.packaging,
						SpecialRequirements: productItem.specialRequirements,
						Invoice: productItem.invoice == 'Y' ? 1 : 0,
						TaxRefundRate: productItem.taxRefundRate,
						InnerBoxQuantity: productItem.innerBoxQuantity,
						OuterBoxQuantity: productItem.outerBoxQuantity,
						BoxCount: productItem.boxCount,
						OuterBoxLength: productItem.outerBoxLength,
						OuterBoxWidth: productItem.outerBoxWidth,
						OuterBoxHeight: productItem.outerBoxHeight,
						OuterBoxVolume: productItem.outerBoxVolume,
						OuterBoxNetWeight: productItem.outerBoxNetWeight,
						OuterBoxGrossWeight: productItem.outerBoxGrossWeight,
						TotalVolume: productItem.totalVolume,
						TotalNetWeight: productItem.totalNetWeight,
						TotalGrossWeight: productItem.totalGrossWeight,
						Remark: productItem.remark
					});
				});
				CustomerRelaterExoensesTableData.value.forEach(expenseItem => {
					addContractsRequest.contractExpensesItems.push({
						ContractId: expenseItem.contractId,
						ExpenseName: expenseItem.expenseName,
						Currency: expenseItem.currency,
						ExchangeRate: expenseItem.exchangeRate,
						Expense: expenseItem.expense,
						Remark: expenseItem.remark
					});
				});
				request.post('Contracts/AddContracts/Add', addContractsRequest).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})

					} else {
						console.error('新增销售合同出错');
					}
				}).catch(error => {
					console.error('新增销售合同出错！😔错误内容：', error);
				})
			}).catch(() => {
				ElMessage({
					type: 'info',
					message: '已取消保存销售合同'
				});
			});
			//#endregion  保存合同信息
		} else {
			ElMessage({
				type: 'error',
				message: '请检查销售合同基本信息是否填写完整'
			});
			return false;
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
	Newcontractform.purchaseTotal = null;
	Newcontractform.taxRefundTotal = null;
	Newcontractform.customerExpenseTotal = null;
	Newcontractform.amountTotal = null;
	Newcontractform.usdConversion = null;
	Newcontractform.commissionAmount = null;
	Newcontractform.shippingCurrency = null;
	Newcontractform.shippingExchangeRate = null;
	Newcontractform.shippingCost = null;
	Newcontractform.insuranceCost = null;
	Newcontractform.bankCost = null;
	Newcontractform.otherCost = null;
	Newcontractform.freightCost = null;
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
//计算外销总价
const SingleProductCalculation = (row) => {
	//外销总价 = 外销单价 * 合同数量
	row.exportTotalPrice = row.exportUnitPrice * row.contractQuantity;
	//采购总价 = 采购单价 * 合同数量
	row.purchaseTotalPrice = row.purchaseUnitPrice * row.contractQuantity;
	//外箱体积 = 外箱长 * 外箱宽 * 外箱高
	row.outerBoxVolume = row.outerBoxLength * row.outerBoxWidth * row.outerBoxHeight;
	//总体积 = 外箱体积 * 外箱数量
	row.totalVolume = row.outerBoxVolume * row.boxCount;
	//总净重 = 外箱净重 * 外箱数量
	row.totalNetWeight = row.outerBoxNetWeight * row.boxCount;
	//总毛重 = 外箱毛重 * 外箱数量
	row.totalGrossWeight = row.outerBoxGrossWeight * row.boxCount;
	calculate();
}

const calculate = () => {
	//货值合计 = 外销总价合计
	Newcontractform.goodsValue = productData.value.reduce((total, item) => total + item.exportTotalPrice, 0);
	Newcontractform.quantity = productData.value.reduce((total, item) => total + item.quantity, 0);
	Newcontractform.boxCount = productData.value.reduce((total, item) => total + item.boxCount, 0);
	Newcontractform.grossWeight = productData.value.reduce((total, item) => total + item.grossWeight, 0);
	Newcontractform.netWeight = productData.value.reduce((total, item) => total + item.netWeight, 0);
	Newcontractform.volume = productData.value.reduce((total, item) => total + item.volume, 0);
	Newcontractform.purchaseTotal = productData.value.reduce((total, item) => total + item.purchaseTotalPrice, 0);
	Newcontractform.taxRefundTotal = productData.value.reduce((total, item) => total + item.taxRefundTotal, 0);
	Newcontractform.customerExpenseTotal = CustomerRelaterExoensesTableData.value.reduce((total, item) => total + item.expense, 0);
	Newcontractform.amountTotal = Newcontractform.purchaseTotal + Newcontractform.taxRefundTotal + Newcontractform.customerExpenseTotal;
}
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
GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
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
					contractsTableData.value[i].contractStatus = state.optionss.hr_contract_status.find(item => item.dictValue === contractsTableData.value[i].contractStatus).dictLabel;
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
const SelctedContractId = ref(0);
const checkContractsDetails = (row) => {
	showEditSaveBtn.value = false;
	isSaveBtnShow.value = false;
	contractDialog.value = true;
	isDisabled.value = true;
	showEditBtn.value = true;
	NewcontractProductTbaleData.value = [];
	CustomerRelaterExoensesTableData.value = [];
	/*表单赋值*/
	SelctedContractId.value = row.id;
	Newcontractform.contractNumber = row.contractNumber;
	Newcontractform.contractDate = row.contractDate;
	Newcontractform.contractStatus = row.contractStatus;
	Newcontractform.customerid = row.customerid;
	Newcontractform.customerNumber = row.customerNumber;
	Newcontractform.customerAbbreviation = row.customerAbbreviation;
	Newcontractform.contactPerson = row.contactPerson;
	Newcontractform.contactEmail = row.contactEmail;
	Newcontractform.effectiveDate = row.effectiveDate;
	Newcontractform.customerLevel = row.customerLevel;
	Newcontractform.customerContract = row.customerContract;
	Newcontractform.deliveryDate = row.deliveryDate;
	Newcontractform.ourCompany = row.ourCompany;
	Newcontractform.settlementType = row.settlementType;
	Newcontractform.foreignCurrency = row.foreignCurrency;
	Newcontractform.exchangeRate = row.exchangeRate;
	Newcontractform.usdExchangeRate = row.usdExchangeRate;
	Newcontractform.settlementMethod = row.settlementMethod;
	Newcontractform.priceTerms = row.priceTerms;
	Newcontractform.shippingPort = row.shippingPort;
	Newcontractform.destinationPort = row.destinationPort;
	Newcontractform.tradeCountry = row.tradeCountry;
	Newcontractform.transportation = row.transportation;
	Newcontractform.salesperson = row.salesperson;
	Newcontractform.hasDeposit = row.hasDeposit;
	Newcontractform.receivedDeposit = row.receivedDeposit;
	Newcontractform.depositDate = row.depositDate;
	Newcontractform.stockProgress = row.stockProgress;
	Newcontractform.deliveryProgress = row.deliveryProgress;
	Newcontractform.profitCalculation = row.profitCalculation;
	Newcontractform.estimatedProfitMargin = row.estimatedProfitMargin;
	Newcontractform.customerOrder = row.customerOrder;
	Newcontractform.contractremark = row.remark;
	Newcontractform.signingLocation = row.signingLocation;
	Newcontractform.canPartial = row.canPartial;
	Newcontractform.canTransit = row.canTransit;
	Newcontractform.insuranceAddition = row.insuranceAddition;
	Newcontractform.insuranceRate = row.insuranceRate;
	Newcontractform.commissionRate = row.commissionRate;
	Newcontractform.receivingBank = row.receivingBank;
	Newcontractform.goodsValue = row.goodsValue;
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
				response.data.contractProducts.forEach(element => {
					NewcontractProductTbaleData.value.push(element);
				});
			}
			if (response.data.contractExpenses.length > 0) {
				CustomerRelaterExoensesTableData.value = response.data.contractExpenses;
			}
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
}

const EditContractsRequest = reactive({
	Id: null,
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
	contractProductItems: [],
	contractExpensesItems: []
});
const EditContractSave = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
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
				EditContractsRequest.ContractStatus = Newcontractform.contractStatus;
				EditContractsRequest.CustomerID = Newcontractform.customerid;
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
				EditContractsRequest.CustomerOrder = Newcontractform.customerOrder;
				EditContractsRequest.Remarks = '';
				EditContractsRequest.SigningLocation = Newcontractform.signingLocation;
				EditContractsRequest.CanPartial = Newcontractform.canPartial;
				EditContractsRequest.CanTransit = Newcontractform.canTransit;
				EditContractsRequest.InsuranceAddition = Newcontractform.insuranceAddition;
				EditContractsRequest.InsuranceRate = Newcontractform.insuranceRate;
				EditContractsRequest.CommissionRate = Newcontractform.commissionRate;
				EditContractsRequest.ReceivingBank = Newcontractform.receivingBank;
				EditContractsRequest.PaymentDate = null;
				EditContractsRequest.GoodsValue = Newcontractform.goodsValue;
				EditContractsRequest.Quantity = Newcontractform.quantity;
				EditContractsRequest.BoxCount = Newcontractform.boxCount;
				EditContractsRequest.GrossWeight = Newcontractform.grossWeight;
				EditContractsRequest.NetWeight = Newcontractform.netWeight;
				EditContractsRequest.Volume = Newcontractform.volume;
				EditContractsRequest.PurchaseTotal = Newcontractform.purchaseTotal;
				EditContractsRequest.TaxRefundTotal = Newcontractform.taxRefundTotal;
				EditContractsRequest.CustomerExpenseTotal = Newcontractform.customerExpenseTotal;
				EditContractsRequest.AmountTotal = Newcontractform.amountTotal;
				EditContractsRequest.UsdConversion = Newcontractform.usdConversion;
				EditContractsRequest.CommissionAmount = Newcontractform.commissionAmount;
				EditContractsRequest.ShippingCurrency = Newcontractform.shippingCurrency;
				EditContractsRequest.ShippingExchangeRate = Newcontractform.shippingExchangeRate;
				EditContractsRequest.ShippingCost = Newcontractform.shippingCost;
				EditContractsRequest.InsuranceCost = Newcontractform.insuranceCost;
				EditContractsRequest.BankCost = Newcontractform.bankCost;
				EditContractsRequest.OtherCost = Newcontractform.otherCost;
				EditContractsRequest.FreightCost = Newcontractform.freightCost;
				EditContractsRequest.TotalCost = Newcontractform.totalCost;
				EditContractsRequest.ExchangeCost = Newcontractform.exchangeCost;
				EditContractsRequest.ProductProfit = Newcontractform.productProfit;
				EditContractsRequest.ProfitAmount = Newcontractform.profitAmount;
				EditContractsRequest.ProfitRate = Newcontractform.profitRate;
				EditContractsRequest.Remark = '';
				NewcontractProductTbaleData.value.forEach(productItem => {
					EditContractsRequest.contractProductItems.push({
						ContractId: 0,
						ProductCode: productItem.productCode,
						CustomerCode: productItem.customerCode,
						ChineseName: productItem.chineseName,
						ChineseSpec: productItem.chineseSpec,
						EnglishName: productItem.englishName,
						ContractQuantity: productItem.contractQuantity,
						Unit: productItem.unit,
						ExportUnitPrice: productItem.exportUnitPrice,
						ExportTotalPrice: productItem.exportTotalPrice,
						PurchaseUnitPrice: productItem.purchaseUnitPrice,
						PurchaseTotalPrice: productItem.purchaseTotalPrice,
						Packaging: productItem.packaging,
						SpecialRequirements: productItem.specialRequirements,
						Invoice: productItem.invoice === 'Y' ? 1 : 0,
						TaxRefundRate: productItem.taxRefundRate,
						InnerBoxQuantity: productItem.innerBoxQuantity,
						OuterBoxQuantity: productItem.outerBoxQuantity,
						BoxCount: productItem.boxCount,
						OuterBoxLength: productItem.outerBoxLength,
						OuterBoxWidth: productItem.outerBoxWidth,
						OuterBoxHeight: productItem.outerBoxHeight,
						OuterBoxVolume: productItem.outerBoxVolume,
						OuterBoxNetWeight: productItem.outerBoxNetWeight,
						OuterBoxGrossWeight: productItem.outerBoxGrossWeight,
						TotalVolume: productItem.totalVolume,
						TotalNetWeight: productItem.totalNetWeight,
						TotalGrossWeight: productItem.totalGrossWeight,
						Remark: productItem.remark
					});
				});
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
				request.post('Contracts/EditContract/Edit', EditContractsRequest).then(response => {
					if (response != null) {
						ElMessage({
							message: response.msg,
							type: 'success'
						})
						contractDialog.value = false;
						SelctedContractId.value = 0;
						GetContractList(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
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
			});
			return false;
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
</script>
