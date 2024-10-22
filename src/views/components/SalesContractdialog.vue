<template>
	<div>
		<div v-if="loading">加载中...</div>
		<div v-if="error">加载失败: {{ error.message }}</div>
		<div v-if="data">
			<el-dialog v-model="contractDialog" title="销售合同" :close-on-click-modal=false style="width: 70%;">
				<span style="font-size: 20px; font-weight: bold;">基本信息</span>
				<el-divider></el-divider>
				<el-form :model="contractform" label-width="120px">
					<el-row>
						<el-col :span="8">
							<el-form-item label="销售合同">
								<el-input v-model="contractform.contractNumber" disabled style="width: 300px"
									placeholder="自动生成"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="合同日期">
								<el-date-picker v-model="contractform.contractDate" type="date" placeholder="请选择合同日期"
									disabled style="width: 300px"></el-date-picker>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="合同状态">
								<el-select filterable v-model="contractform.contractStatus" placeholder="请选择合同状态"
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
								<el-select filterable v-model="contractform.customerNumber" placeholder="请选择客户编号"
									disabled style="width: 300px" @change="handleCustomerSelection">
									<el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="客户简称" prop="customerAbbreviation">
								<el-select v-model="contractform.customerAbbreviation" filterable placeholder="请选择客户简称"
									disabled style="width: 300px;" @change="handleCustomerSelection">
									<el-option v-for="dict in optionss.sql_hr_customer_abbreviation"
										:key="dict.dictCode" :label="dict.dictLabel"
										:value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="联系人" prop="contactPerson">
								<el-select filterable v-model="contractform.contactPerson" placeholder="请选择联系人"
									style="width: 300px" disabled @change="handleContactpersonSelection">
									<el-option v-for="item in contactpersonSelectOptions" :key="item.value"
										:label="item.label" :value="item.value" />
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="联系人Email">
								<el-input v-model="contractform.contactEmail" disabled style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="生效日期">
								<el-date-picker v-model="contractform.effectiveDate" type="date" placeholder="请选择生效日期"
									disabled style="width: 300px"></el-date-picker>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="客户等级">
								<el-select filterable v-model="contractform.customerLevel" placeholder="请选择客户等级"
									disabled style="width: 300px">
									<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="客户合同">
								<el-input v-model="contractform.customerContract" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="交货日期" prop="deliveryDate">
								<el-date-picker v-model="contractform.deliveryDate" type="date" placeholder="请选择交货日期"
									disabled style="width: 300px"></el-date-picker>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="我方公司" prop="ourCompany">
								<el-select filterable v-model="contractform.ourCompany" placeholder="请选择我方公司"
									style="width: 300px" disabled>
									<el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="结算类别" prop="settlementType">
								<el-select filterable v-model="contractform.settlementType" placeholder="请选择结算类别"
									disabled style="width: 300px">
									<el-option label="类别1" value="1"></el-option>
									<el-option label="类别2" value="2"></el-option>
									<el-option label="类别3" value="3"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="外销币种" prop="foreignCurrency">
								<el-select filterable v-model="contractform.foreignCurrency" placeholder="请选择外销币种"
									disabled style="width: 300px">
									<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="汇率" prop="exchangeRate">
								<el-input v-model="contractform.exchangeRate" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="美金汇率" prop="usdExchangeRate">
								<el-input v-model="contractform.usdExchangeRate" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="结汇方式" prop="settlementMethod">
								<el-select filterable v-model="contractform.settlementMethod" placeholder="请选择结汇方式"
									disabled style="width: 300px">
									<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="价格条款" prop="priceTerms">
								<el-select filterable v-model="contractform.priceTerms" placeholder="请选择价格条款"
									style="width: 300px" disabled @change="FreightChange()">
									<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="出运口岸" prop="shippingPort">
								<el-select filterable v-model="contractform.shippingPort" placeholder="请选择出运口岸" disabled
									style="width: 300px">
									<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="目的口岸" prop="destinationPort">
								<el-select filterable v-model="contractform.destinationPort" placeholder="请选择目的口岸"
									disabled style="width: 300px">
									<el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="贸易国别" prop="tradeCountry">
								<el-select filterable v-model="contractform.tradeCountry" placeholder="请选择贸易国别" disabled
									style="width: 300px">
									<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="运输方式" prop="transportation">
								<el-select filterable v-model="contractform.transportation" placeholder="请选择运输方式"
									disabled style="width: 300px" @change="FreightChange()">
									<el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="销售员" prop="salesperson">
								<el-select filterable v-model="contractform.salesperson" placeholder="请选择销售员"
									style="width: 300px" disabled>
									<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="有无定金" prop="hasDeposit">
								<el-checkbox v-model="contractform.hasDeposit" disabled
									@change="hasDeposithandleCheckboxChange"></el-checkbox>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="已收定金" v-show=DepositShow prop="receivedDeposit">
								<el-input v-model="contractform.receivedDeposit" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="定金日期" v-show=DepositShow prop="depositDate">
								<el-date-picker v-model="contractform.depositDate" type="date" placeholder="请选择定金日期"
									disabled style="width: 300px"></el-date-picker>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="定金比例" v-show=DepositShow prop="Depositratio">
								<el-input v-model="contractform.Depositratio" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>

					<span style="font-size: 20px; font-weight: bold;">辅助信息</span>
					<el-divider></el-divider>

					<el-row>
						<el-col :span="8">
							<el-form-item label="签约地点">
								<el-select filterable v-model="contractform.signingLocation" placeholder="请选择签约地点"
									disabled style="width: 300px">
									<el-option v-for="dict in optionss.hr_signing_place" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="可否分批">
								<el-select filterable v-model="contractform.canPartial" placeholder="请选择可否分批" disabled
									style="width: 300px">
									<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="可否转运">
								<el-select filterable v-model="contractform.canTransit" placeholder="请选择可否转运" disabled
									style="width: 300px">
									<el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="保险加成">
								<el-input v-model="contractform.insuranceAddition" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="保险比率">
								<el-input v-model="contractform.insuranceRate" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="佣金比率">
								<el-input v-model="contractform.commissionRate" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="收汇银行">
								<el-input v-model="contractform.receivingBank" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="付款日期">
								<el-date-picker v-model="contractform.paymentDate" type="date" placeholder="请选择付款日期"
									disabled style="width: 300px"></el-date-picker>
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
											style="width: 100%;" disabled>
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
										<el-input v-model="row.expenseName" placeholder="输入费用名称"
											size="small"></el-input>
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
								<el-input v-model="contractform.goodsValue" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="数量合计">
								<el-input v-model="contractform.quantity" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="箱数合计">
								<el-input v-model="contractform.boxCount" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="毛重合计">
								<el-input v-model="contractform.grossWeight" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="净重合计">
								<el-input v-model="contractform.netWeight" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="体积合计">
								<el-input v-model="contractform.volume" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="采购合计">
								<el-input v-model="contractform.purchaseTotal" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="退税总额">
								<el-input v-model="contractform.taxRefundTotal" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="客户费用合计">
								<el-input v-model="contractform.customerExpenseTotal" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="金额合计">
								<el-input v-model="contractform.amountTotal" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="美金/欧元换算">
								<el-input v-model="contractform.usdConversion" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<span style="font-size: 20px; font-weight: bold;">利润预估</span>
					<el-divider></el-divider>
					<el-row>
						<el-col :span="8">
							<el-form-item label="佣金金额">
								<el-input v-model="contractform.commissionAmount" style="width: 300px"
									disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="海运费币种">
								<el-input v-model="contractform.shippingCurrency" style="width: 300px"
									:disabled="IsOceanFreightEdit"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="海运费汇率">
								<el-input v-model="contractform.shippingExchangeRate" style="width: 300px"
									:disabled="IsOceanFreightEdit"></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="海运费">
								<el-input v-model="contractform.shippingCost" style="width: 300px"
									:disabled="IsOceanFreightEdit"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="保险费用">
								<el-input v-model="contractform.insuranceCost" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="银行费用">
								<el-input v-model="contractform.bankCost" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="其它费用">
								<el-input v-model="contractform.otherCost" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="运杂费">
								<el-input v-model="contractform.freightCost" style="width: 300px"
									:disabled="IsCarTransportationEdit"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="成本总额">
								<el-input v-model="contractform.totalCost" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="换汇成本">
								<el-input v-model="contractform.exchangeCost" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="产品毛利">
								<el-input v-model="contractform.productProfit" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="利润金额">
								<el-input v-model="contractform.profitAmount" style="width: 300px" disabled></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="利润率">
								<el-input v-model="contractform.profitRate" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="运杂费币种">
								<el-input v-model="contractform.FreightCurrency" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
						<el-col :span="8">
							<el-form-item label="运杂费报价">
								<el-input v-model="contractform.FreightQuote" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<el-row>
						<el-col :span="8">
							<el-form-item label="快递费">
								<el-input v-model="contractform.Courierfee" style="width: 300px"></el-input>
							</el-form-item>
						</el-col>
					</el-row>
					<span style="font-size: 20px; font-weight: bold;">备注信息</span>
					<el-divider></el-divider>
					<el-input v-model="contractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }"
						type="textarea" placeholder="请输入备注信息" />
				</el-form>
				<template #footer>
					<span class="dialog-footer">
						<el-button type="primary">
							确定保存
						</el-button>
						<el-button type="primary">
							编辑
						</el-button>
						<el-button type="primary">
							编辑保存
						</el-button>
					</span>
				</template>
			</el-dialog>
		</div>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessage, ElRow, ElDialog, ElCol } from 'element-plus'
import request from '@/utils/request';
import { useFetchData } from '@/views/fetchcontractdata'

defineOptions({
	name: 'SalesContractDialog'
})
// 定义 props 的类型
interface Props {
	id: number;
}

// 接收 props
const props = defineProps<Props>();

// 调用组合式函数获取数据
const { data, loading, error } = useFetchData(props.id);

const contractDialog = ref(false)
const contractform = reactive({
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
	contractform.customerid = value;
	contractform.contactPerson = null;
	contractform.contactEmail = '';
	request({
		url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
		method: 'GET',
		params: {
			ID: value
		}
	}).then(response => {
		if (response != null) {
			contractform.customerNumber = value.toString();
			contractform.customerAbbreviation = value.toString();
			contractform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == response.customerLevel).map(item => item.dictValue).values().next().value;
			contractform.priceTerms = state.optionss['hr_pricing_term'].filter(item => item.dictValue == response.pricingTerm).map(item => item.dictValue).values().next().value;
			contractform.settlementType = state.optionss['hr_settlement_way'].filter(item => item.dictValue == response.settlementWay).map(item => item.dictValue).values().next().value;
			contractform.tradeCountry = state.optionss['hr_nation'].filter(item => item.dictValue == response.tradingCountry).map(item => item.dictValue).values().next().value;
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
</script>