<template>
	<div>
		<!-- 采购需求表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden; margin-bottom: 20px;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<span style="font-size: 20px; font-weight: bold;">采购需求</span>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 表格区域 -->
			<el-table :data="shoppinglisttableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }" :span-method="mergeCells">
				<el-table-column prop="procurementId" label="ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="contractNumber" label="销售合同号" width="110"></el-table-column>
				<el-table-column prop="contractId" label="销售合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="productId" label="产品ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="productName" label="产品名称" width="150"></el-table-column>
				<el-table-column prop="quantity" label="数量" width="90"></el-table-column>
				<el-table-column prop="unitPrice" label="采购单价" width="120"></el-table-column>
				<el-table-column prop="totalPrice" label="采购总价" width="120"></el-table-column>
				<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
				<!-- 操作 -->
				<el-table-column label="操作" width="150">
					<template #default="scope">
						<!-- 合并的"生成采购合同"按钮 -->
						<div v-if="isFirstRow(scope.row)">
							<el-button type="primary" size="small" @click="ViewDetails(scope.row)">
								需求详情
							</el-button>
						</div>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="contractsTableDatahandlePageChange"
				:current-page="contractsTableDatacurrentPage" :page-size="contractsTableDatapageSize"
				:total="contractsTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		</div>

		<!-- 采购合同表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<span style="font-size: 20px; font-weight: bold;">采购合同</span>
						</div>
					</el-col>
				</el-row>
			</div>
			<!-- 表格区域 -->
			<el-table :data="contractofpurchasetableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="id" label="ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="purchaseContractNumber" label="采购合同号" width="120">
					<template #default="scope">
						<span>{{ scope.row.purchaseContractNumber }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="contractStatus" label="合同状态" width="90"></el-table-column>
				<el-table-column prop="reviewStatus" label="审核状态编号" width="150" v-if="false"></el-table-column>
				<el-table-column prop="reviewStatusStr" label="审核状态" width="110" align="center">
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
				<el-table-column prop="salesContract" label="销售合同" width="100"></el-table-column>
				<el-table-column prop="customerContract" label="客户订单号" width="120"></el-table-column>
				<el-table-column prop="deliveryDate" label="交货日期" width="110"></el-table-column>
				<el-table-column prop="purchaseCurrency" label="采购货币" width="100"></el-table-column>
				<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
				<el-table-column prop="purchaser" label="采购员" width="150"></el-table-column>
				<el-table-column prop="createTime" label="创建日期" width="110"></el-table-column>
				<el-table-column prop="deliveryLocation" label="交货地点" width="150" v-if="false"></el-table-column>
				<el-table-column prop="paymentDays" label="付款天数" width="150" v-if="false"></el-table-column>
				<el-table-column prop="deposit" label="预付款金额" width="150" v-if="false"></el-table-column>
				<el-table-column prop="hasDeposit" label="有无预付款" width="150" v-if="false"></el-table-column>
				<el-table-column fixed="right" label="操作" width="280">
					<template #default="scope">
						<el-button type="text" size="small" @click="CheckDetails(scope.row)">查看详情</el-button>
						<el-button v-if="isContractStatusApproved(scope.row)" type="text" size="small"
							@click="GeneratePurchaseContract(scope.row)">生成采购合同PDF</el-button>
						<el-button type="primary" size="small" link
							v-if="!scope.row.isDraft && scope.row.contractStatus !== '已完结' && scope.row.originalContractStatus >= 3 && scope.row.originalContractStatus < 10 && scope.row.reviewStatusStr !== '审核中' && scope.row.purchaserId === useUserStore().userId.toString()"
							@click="completePurchaseContractManually(scope.row)">申请完结</el-button>
						<el-button v-if="scope.row.createBy === useUserStore().userId.toString() && scope.row.isDraft"
							link type="danger" size="small" @click="DeletePurchaseContract(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="purchasecontractsTableDatahandlePageChange"
				:current-page="purchasecontractsTableDatacurrentPage" :page-size="purchasecontractsTableDatapageSize"
				:total="purchasecontractsTableDatatotalItems" background layout="prev, pager, next"
				style="margin-top: 5px;" />
		</div>

		<el-dialog :modal="false" modal-penetrable v-model="Addcontractofpurchasedialog" title="新增采购合同"
			:close-on-click-modal=false style="width: 75%;" @close="handleAddcontractofpurchasedialogclose">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-form ref="AddcontractofpurchaseformRef" :rules="rules" :model="Addcontractofpurchaseform"
						label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="采购合同">
									<el-input ref="AddcontractofpurchaseformRef" :rules="rules"
										v-model="Addcontractofpurchaseform.purchaseContract" disabled
										placeholder="请输入采购合同" style="width: 300px" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="合同状态">
									<el-select v-model="Addcontractofpurchaseform.contractStatus" placeholder="请选择合同状态"
										style="width: 300px" disabled size="default" clearable>
										<el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="交货日期">
									<el-date-picker v-model="Addcontractofpurchaseform.deliveryDate" type="date"
										disabled style="width: 300px" size="default"></el-date-picker>
								</el-form-item>
							</el-col>
							<el-col :span="6" v-if="false">
								<el-form-item label="采购币种">
									<el-select v-model="Addcontractofpurchaseform.purchaseCurrency" style="width: 300px"
										:disabled="isFormDisabled" size="default" clearable>
										<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="采购员">
									<el-select disabled v-model="Addcontractofpurchaseform.purchaser"
										placeholder="请选择采购员" style="width: 300px" size="default" clearable>
										<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6" v-if="false">
								<el-form-item label="价格条款">
									<el-select v-model="Addcontractofpurchaseform.priceTerms" style="width: 300px"
										:disabled="isFormDisabled" size="default" @change="handlePriceTermsChange"
										clearable>
										<el-option v-for="dict in filteredPriceTermsOptions" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>

							<el-col :span="6">
								<el-form-item label="销售合同">
									<el-select v-model="Addcontractofpurchaseform.salesContract" placeholder="请选择销售合同"
										style="width: 300px" :disabled="isFormDisabled || isSalesContractDisabled"
										size="default" clearable>
										<el-option v-for="dict in optionss.sql_sale_contracts" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="销售员">
									<el-select v-model="Addcontractofpurchaseform.salesperson" disabled
										style="width: 300px" size="default" clearable>
										<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="付款天数" prop="paymentDays">
									<el-select v-model="Addcontractofpurchaseform.paymentDays" style="width: 300px"
										:disabled="isFormDisabled" size="default" clearable>
										<el-option v-for="dict in optionss.hr_purchase_payment_days"
											:key="dict.dictCode" :label="dict.dictLabel"
											:value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="交货地点">
									<el-input v-model="Addcontractofpurchaseform.deliveryLocation" style="width: 300px"
										:disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6" v-if="false">
								<el-form-item label="预付款金额">
									<el-input v-model="Addcontractofpurchaseform.deposit"
										:disabled="!Addcontractofpurchaseform.hasDeposit || isFormDisabled"
										style="width: 300px" placeholder="请输入预付款金额" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6" v-if="false">
								<el-form-item label="有无预付款" prop="hasDeposit">
									<el-checkbox v-model="Addcontractofpurchaseform.hasDeposit"
										@change="handleDepositChange" :disabled="isFormDisabled"></el-checkbox>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<el-tabs v-model="activeName" class="demo-tabs" @tab-click="handleClick">
				<el-tab-pane label="产品资料" name="productinfo">
					<el-table :data="productinfotableData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="productCode" label="产品编号" width="110"></el-table-column>
						<!-- 供应商列 -->
						<el-table-column prop="supplier" label="供应商" width="210">
							<template #default="scope">
								<el-select v-model="scope.row.supplier" placeholder="选择供应商" :disabled="isFormDisabled"
									@focus="loadProductSuppliers(scope.row)" clearable>
									<el-option v-for="supplier in (productSuppliersMap.get(scope.row.productId) || [])"
										:key="supplier.dictCode" :label="supplier.dictLabel"
										:value="supplier.dictValue">
									</el-option>
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="hasDeposit" label="有无预付款" width="120">
							<template #default="scope">
								<el-checkbox v-model="scope.row.hasDeposit" :disabled="isFormDisabled"
									@change="handleProductDepositChange(scope.row)">
								</el-checkbox>
							</template>
						</el-table-column>
						<el-table-column prop="depositAmount" label="预付款金额" width="150">
							<template #default="scope">
								<el-input v-model="scope.row.depositAmount" type="number" placeholder="请输入预付款金额"
									:disabled="!scope.row.hasDeposit || isFormDisabled" style="width: 110px"
									@change="handleDepositAmountChange(scope.row)">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
						<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
						<el-table-column prop="englishName" label="英文品名" width="150" v-if="false"></el-table-column>
						<el-table-column prop="chineseSpecification" label="中文规格" width="150"></el-table-column>
						<el-table-column prop="unit" label="计量单位" width="90"></el-table-column>
						<!-- 其他列保持不变 -->
						<el-table-column prop="contractQuantity" label="合同数量" width="150">
							<template #default="scope">
								<el-input v-model="scope.row.contractQuantity" type="number"
									@change="handleQuantityChange(scope.row)" placeholder="请输入数量"
									:disabled="isFormDisabled">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="purchasecurrency" label="采购币种" width="120">
							<template #default="scope">
								<el-select v-model="scope.row.purchasecurrency" placeholder="选择币种" size="default"
									:disabled="isFormDisabled" clearable>
									<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseUnitPrice" label="采购单价" width="150">
							<template #default="scope">
								<el-input :disabled="isFormDisabled" v-model="scope.row.purchaseUnitPrice" type="number"
									@change="handlePriceChange(scope.row)" placeholder="请输入单价">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="purchasePriceTerms" label="采购价格条款" width="180">
							<template #default="scope">
								<el-select v-model="scope.row.purchasePriceTerms" filterable placeholder="请选择采购价格条款"
									:disabled="true" clearable>
									<el-option v-for="dict in optionss.hr_purchase_pricing_term" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseTotalPrice" label="采购总价" width="90">
							<template #default="scope">
								<span>{{ scope.row.purchaseTotalPrice }}</span>
							</template>
						</el-table-column>
						<!-- 其他列保持不变 -->
						<el-table-column prop="deliveryDate" label="交货日期" width="200">
							<template #default="scope">
								<el-date-picker v-model="scope.row.deliveryDate" type="date" :disabled="isFormDisabled"
									style="width: 180px"></el-date-picker>
							</template>
						</el-table-column>
						<el-table-column prop="productionLeadTime" label="生产交期" width="200">
							<template #default="scope">
								<el-date-picker v-model="scope.row.productionLeadTime" type="date"
									:disabled="isFormDisabled" style="width: 180px"></el-date-picker>
							</template>
						</el-table-column>
						<el-table-column prop="packaging" label="包装方式" width="90"></el-table-column>
						<el-table-column prop="specialRequirements" label="特殊要求" width="90"></el-table-column>
						<el-table-column prop="invoice" label="是否开票" width="90">
							<template #default="scope">
								<span>{{ (scope.row.invoice === 1 || scope.row.invoice === '1' || scope.row.invoice ===
									true || scope.row.invoice === '是') ? '是' : '否' }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="innerBoxQuantity" label="内盒装量" width="90"></el-table-column>
						<el-table-column prop="outerBoxQuantity" label="外箱装量" width="90"></el-table-column>

						<el-table-column prop="remark" label="备注" width="200">
							<template #default="scope">
								<el-input v-model="scope.row.remark" placeholder="请输入备注" :disabled="isFormDisabled"
									style="width: 140px"></el-input>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="厂家相关费用" name="relatedcosts">
					<div style="margin-bottom: 10px;">
						<el-button type="primary" size="default" @click="addNewExpense" :disabled="isFormDisabled">
							新增费用
						</el-button>
					</div>
					<el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="expenseName" label="费用名称" width="150">
							<template #default="{ row }">
								<el-input v-model="row.expenseName" placeholder="请输入费用名称" size="default"
									:disabled="isFormDisabled" style="width: 100%"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="supplier" label="供应商" width="200">
							<template #default="{ row }">
								<el-select v-model="row.supplier" placeholder="选择供应商" size="default"
									:disabled="isFormDisabled" clearable>
									<el-option v-for="supplier in selectedSuppliers" :key="supplier.dictCode"
										:label="supplier.dictLabel" :value="supplier.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="currency" label="币种" width="150">
							<template #default="{ row }">
								<el-select v-model="row.currency" placeholder="选择币种" size="default"
									@change="currencyChange(row)" :disabled="isFormDisabled" clearable>
									<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="exchangeRate" label="汇率" width="150">
							<template #default="{ row }">
								<el-input v-model="row.exchangeRate" placeholder="输入汇率" size="default"
									:disabled="isFormDisabled" @input="expenseChange(row)"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="expense" label="费用" width="150">
							<template #default="{ row }">
								<el-input v-model="row.expense" placeholder="输入费用" size="default"
									@input="expenseChange(row)" :disabled="isFormDisabled"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="amount" label="金额" width="150">
							<template #default="{ row }">
								<el-input v-model="row.amount" disabled size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" width="150">
							<template #default="{ row }">
								<el-input v-model="row.remark" placeholder="输入备注" size="default"
									:disabled="isFormDisabled"></el-input>
							</template>
						</el-table-column>
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="default" @click="handleDelete(scope.$index)"
									:disabled="isFormDisabled">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
			</el-tabs>
			<el-collapse v-model="totalInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="合计信息" name="totalInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">合计信息</span>
					</template>
					<el-form :model="Totalvalueofgoodsform" label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="采购金额合计">
									<el-input v-model="Totalvalueofgoodsform.totalValue" placeholder="自动计算"
										style="width: 300px" disabled size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="数量合计">
									<el-input v-model="Totalvalueofgoodsform.totalQuantity" placeholder="请输入数量合计"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="箱数合计">
									<el-input v-model="Totalvalueofgoodsform.totalBoxCount" placeholder="请输入箱数合计"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="毛重合计">
									<el-input v-model="Totalvalueofgoodsform.totalGrossWeight" placeholder="请输入毛重合计"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="净重合计">
									<el-input v-model="Totalvalueofgoodsform.totalNetWeight" placeholder="请输入净重合计"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="体积合计">
									<el-input v-model="Totalvalueofgoodsform.totalVolume" placeholder="请输入体积合计"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="已申请付款">
									<el-input v-model="Totalvalueofgoodsform.appliedPayment" placeholder="请输入已申请付款"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="可申请付款">
									<el-input v-model="Totalvalueofgoodsform.availablePayment" placeholder="请输入可申请付款"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="已付货款">
									<el-input v-model="Totalvalueofgoodsform.paidAmount" placeholder="请输入已付货款"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="未付货款">
									<el-input v-model="Totalvalueofgoodsform.unpaidAmount" placeholder="请输入未付货款"
										style="width: 300px" :disabled="isFormDisabled" size="default"></el-input>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<template #footer>
				<span class="dialog-footer">
					<!-- 新增时的保存按钮 -->
					<el-button type="warning" v-show="isSaveBtnShow && userId.toString() === CheckUser"
						@click="SavePurchaseContract">
						保存草稿
					</el-button>
					<!-- 编辑时的保存按钮 -->
					<el-button type="warning" v-show="showEditSaveBtn && userId.toString() === CheckUser"
						@click="saveEditContract">
						保存草稿
					</el-button>
					<!-- 查看详情时的编辑按钮 -->
					<el-button type="primary" v-show="showEditBtn && userId.toString() === CheckUser"
						@click="editContract">
						编辑
					</el-button>
					<el-button type="success" v-show="showSubmitReviewBtn && userId.toString() === CheckUser"
						@click="submitForReview">
						提交
					</el-button>
					<el-button type="danger" v-show="showApproveRejectBtn" @click="ApproveReject">
						驳回
					</el-button>
					<el-button type="success" v-show="showApprovePassBtn" @click="Approvepass">
						通过
					</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 添加新的查看详情对话框 -->
		<el-dialog v-model="viewDetailsDialog" title="采购需求详情" :close-on-click-modal=false style="width: 75%;">
			<el-table :data="detailsTableData" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="SaleContractID" label="采购合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="id" label="id" width="150" v-if="false"></el-table-column>
				<el-table-column prop="productId" label="产品ID" width="150" v-if="false"></el-table-column>
				<el-table-column label="操作" width="150" v-if="showOperationColumn">
					<template #default="scope">
						<el-button v-if="scope.row.productId === 0 || scope.row.productId === null" type="primary"
							size="small" @click="handleAddNewProduct(scope.row)">
							发送添加产品申请
						</el-button>
					</template>
				</el-table-column>
				<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
				<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
				<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
				<el-table-column prop="englishName" label="英文品名" width="150"></el-table-column>
				<el-table-column prop="chineseSpec" label="中文规格" width="150"></el-table-column>
				<el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
				<el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
				<el-table-column prop="purchaseUnitPrice" label="采购单价" width="150">
					<template #default="scope">
						<el-input v-model="scope.row.purchaseUnitPrice" type="number" size="small"
							@input="handleDetailsPriceChange(scope.row)" placeholder="请输入单价">
						</el-input>
					</template>
				</el-table-column>
				<el-table-column prop="purchaseTotalPrice" label="采购总价" width="150">
					<template #default="scope">
						<span>{{ scope.row.purchaseTotalPrice }}</span>
					</template>
				</el-table-column>
				<!-- 添加其他字段 -->
				<el-table-column prop="packaging" label="包装方式" width="150"></el-table-column>
				<el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
				<el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
				<el-table-column prop="innerBoxQuantity" label="内盒装量" width="150"></el-table-column>
				<el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
				<el-table-column prop="remark" label="备注" width="150"></el-table-column>
			</el-table>

			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" v-if="hasPriceChanges" @click="notifySales">
						通知销售
					</el-button>
					<el-button type="primary" v-if="!hasPriceChanges && GeneratePurchaseContractBtnShow"
						@click="GeneratePurchaseContract(currentDetailRow)">
						生成采购合同
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref, computed } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { get } from 'sortablejs';
import { el, fa } from 'element-plus/es/locale';
import useUserStore from "@/store/modules/user";
import { FormInstance } from 'element-plus'
import { invoke } from '@vueuse/core';
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer } from 'element-plus'
import { useRouter } from 'vue-router'
import exchangeRateService from '@/utils/exchangeRateService'

const router = useRouter()
// 添加onMounted钩子
onMounted(async () => {
	console.log('采购合同页面挂载，检查路由参数')
	// 等待字典数据加载完成后再执行
	await fetchDataAndExecute()
	autoLoadpurchaseContractDetail()
})

// 添加自动加载合同详情的函数
const autoLoadpurchaseContractDetail = () => {
	// 检查URL参数
	const purchaseContractId = router.currentRoute.value.query.purchaseContractId
	const viewDetail = router.currentRoute.value.query.viewDetail
	if (purchaseContractId && viewDetail === 'true') {
		console.log('自动加载合同详情, ID:', purchaseContractId)

		// 查找匹配的合同
		GetpurchaseContractList(1, 100).then(() => {
			const contract = contractofpurchasetableData.value.find(item =>
				item.id.toString() === purchaseContractId.toString()
			)

			if (contract) {
				// 调用查看详情的函数
				CheckDetails(contract)
			} else {
				console.error('未找到匹配的合同:', purchaseContractId)
			}
		})
	}
}

const showOperationColumn = computed(() => {
	return detailsTableData.value.some(row => row.productId === 0 || row.productId === null);
});

// 产品供应商选择功能保留，每个产品可以独立选择供应商

const GeneratePurchaseContract = (row) => {
	ElMessageBox.confirm('确定生成采购合同吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		try {
			isGeneratedFromRequirement.value = true; // 设置标志
			isSaveBtnShow.value = true;           // 显示确定保存按钮
			showEditBtn.value = false;
			showEditSaveBtn.value = false;
			showSubmitReviewBtn.value = true;
			isFormDisabled.value = false;
			// 先获取新的采购合同号
			await GetNewPurchaseContractNumber(row.contractId);

			// 获取销售合同详情
			const response = await request({
				url: 'Contracts/GetContractDetailsById/GetContractDetails',
				method: 'GET',
				params: {
					contractId: row.contractId
				}
			});

			if (response.code === 200 && response.data) {
				// 绑定基本信息
				Addcontractofpurchaseform.value.purchaser = state.optionss.sql_all_user.find(item => item.dictValue === userId.toString())?.dictValue;
				Addcontractofpurchaseform.value.contractStatus = "1";
				Addcontractofpurchaseform.value.salesContract = state.optionss.sql_sale_contracts.find(item => item.dictValue === response.data.contract.id.toString())?.dictValue;
				// 销售合同绑定成功后，禁用销售合同选择框
				isSalesContractDisabled.value = true;
				Addcontractofpurchaseform.value.customerContract = response.data.contract.customerContract;
				Addcontractofpurchaseform.value.customerid = response.data.contract.customerId;
				Addcontractofpurchaseform.value.customerAbbreviation = response.data.contract.customerAbbreviation;
				Addcontractofpurchaseform.value.deliveryDate = response.data.contract.deliveryDate;
				// 设置采购币种默认为人民币
				Addcontractofpurchaseform.value.purchaseCurrency = "3";
				Addcontractofpurchaseform.value.deposit = "0";
				Addcontractofpurchaseform.value.hasDeposit = false;
				Addcontractofpurchaseform.value.salesperson = response.data.contract.salesperson.toString();
				// 判断价格条款是否在可选范围内
				const priceTermsValue = response.data.contract.priceTerms.toString();
				const isValidPriceTerm = state.optionss.hr_purchase_pricing_term.some(item => item.dictValue === priceTermsValue);
				Addcontractofpurchaseform.value.priceTerms = isValidPriceTerm ? priceTermsValue : '';
				Addcontractofpurchaseform.value.deliveryLocation = response.data.contract.deliveryLocation || '';

				// 绑定商品信息
				if (response.data.contractProducts && response.data.contractProducts.length > 0) {
					productinfotableData.value = response.data.contractProducts.map(product => ({
						id: product.id,
						productId: product.productID, // 确保这里设置了productId
						productCode: product.productCode,
						customerCode: product.customerCode || '', // 添加客户货号
						chineseName: product.chineseName,
						chineseSpecification: product.chineseSpec,
						unit: state.optionss.hr_calculate_unit.find(item => item.dictValue === product.unit.toString())?.dictLabel || '无',
						contractQuantity: product.contractQuantity,
						purchasecurrency: (product.purchasecurrency ?? product.purchaseCurrency)?.toString() || '3',
						purchaseUnitPrice: product.purchaseUnitPrice,
						purchasePriceTerms: (product.purchasingPriceTerms ?? product.purchasePriceTerms ?? product.purchasepriceterms)?.toString() || '',
						purchaseTotalPrice: product.purchaseTotalPrice,
						// 如果产品没有交货日期，则使用基本信息中的交货日期
						deliveryDate: product.deliveryDate || response.data.contract.deliveryDate,
						productionLeadTime: product.productionLeadTime,
						packaging: state.optionss.hr_packing.find(item => item.dictValue === product.packaging.toString())?.dictLabel || '无',
						specialRequirements: product.specialRequirements,
						innerBoxQuantity: product.innerBoxQuantity,
						outerBoxQuantity: product.outerBoxQuantity,
						invoice: product.invoice == 1 ? 1 : 0, // 统一使用数字格式
						hasDeposit: product.hasDeposit == 1 || product.hasDeposit === true,
						depositAmount: product.depositAmount || 0,
						remark: product.remark,
						supplier: product.supplierId || null // 添加供应商信息
					}));

					// 记录产品的初始开票状态
					initialInvoiceStates.value.clear();
					productinfotableData.value.forEach(product => {
						const isInvoice = product.invoice === 1 || product.invoice === '是' || product.invoice === '1';
						initialInvoiceStates.value.add(isInvoice);
					});

					// 根据产品的是否开票值设置默认价格条款
					const hasInvoiceProducts = initialInvoiceStates.value.has(true);
					const hasNoInvoiceProducts = initialInvoiceStates.value.has(false);

					// 如果所有产品都是开票的，设置默认价格条款为含税含运费（7）
					if (hasInvoiceProducts && !hasNoInvoiceProducts) {
						Addcontractofpurchaseform.value.priceTerms = '7';
					}
					// 如果所有产品都是不开票的，设置默认价格条款为不含税含运费（9）
					else if (hasNoInvoiceProducts && !hasInvoiceProducts) {
						Addcontractofpurchaseform.value.priceTerms = '9';
					}
					// 如果产品混合了开票和不开票，不设置默认价格条款，让用户选择

					// 为每个产品预加载供应商信息
					for (const product of productinfotableData.value) {
						if (product.productId) {
							await loadProductSuppliers(product);
						}
					}

					// 设置默认交货日期
					setDefaultDeliveryDateForProducts(Addcontractofpurchaseform.value.deliveryDate);
					// 自动计算合计信息
					updateTotalValues();
				}

				// 合计信息将通过updateTotalValues()自动计算，不再使用后端数据
				Totalvalueofgoodsform.value.totalBoxCount = response.data.contract.boxCount;
				Totalvalueofgoodsform.value.totalGrossWeight = response.data.contract.grossWeight;
				Totalvalueofgoodsform.value.totalNetWeight = response.data.contract.netWeight;
				Totalvalueofgoodsform.value.totalVolume = response.data.contract.volume;
				Totalvalueofgoodsform.value.appliedPayment = "0";
				Totalvalueofgoodsform.value.availablePayment = "0";
				Totalvalueofgoodsform.value.paidAmount = "0";
				Totalvalueofgoodsform.value.unpaidAmount = "0";

				// 关闭采购需求对话框
				viewDetailsDialog.value = false;  // 假设这是采购需求对话框的控制变量
				SaleContractID.value = row.contractId;
				// 打开对话框
				Addcontractofpurchasedialog.value = true;
			} else {
				ElMessage.error('获取销售合同详情失败');
			}
		} catch (error) {
			console.error('生成采购合同失败:', error);
			ElMessage.error('生成采购合同失败，请重试');
		}
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消生成采购合同'
		});
	});
}

// 设置产品表格中所有产品的默认交货日期
const setDefaultDeliveryDateForProducts = (defaultDeliveryDate) => {
	if (productinfotableData.value && productinfotableData.value.length > 0) {
		productinfotableData.value.forEach(product => {
			if (!product.deliveryDate) {
				product.deliveryDate = defaultDeliveryDate;
			}
		});
	}
}

const OpenAddcontractofpurchasedialog = async () => {
	isGeneratedFromRequirement.value = false; // 重置标志
	clearAll();
	await GetNewPurchaseContractNumber();
	Addcontractofpurchaseform.value.purchaser = userId.toString();
	Addcontractofpurchaseform.value.contractStatus = '1';
	// 设置采购币种默认为人民币
	Addcontractofpurchaseform.value.purchaseCurrency = "3";
	// 设置预付款相关字段
	Addcontractofpurchaseform.value.deposit = "0";
	Addcontractofpurchaseform.value.hasDeposit = false;
	Addcontractofpurchasedialog.value = true;
	isSaveBtnShow.value = true;
	showApproveRejectBtn.value = false;
	showApprovePassBtn.value = false;
	// 新增：如果有产品数据，设置默认交货日期
	if (productinfotableData.value.length > 0) {
		// 设置默认交货日期
		setDefaultDeliveryDateForProducts(Addcontractofpurchaseform.value.deliveryDate);
	}
}

// 添加新的响应式变量
const viewDetailsDialog = ref(false)
const detailsTableData = ref([])
const hasPriceChanges = ref(false)
const currentDetailRow = ref(null)
const ProcurementRequirementID = ref(0);
const GeneratePurchaseContractBtnShow = ref(false)
// 查看采购需求详情方法
const ViewDetails = (row) => {
	ProcurementRequirementID.value = row.procurementId;
	currentDetailRow.value = row
	hasPriceChanges.value = false
	// 获取该合同的详细信息
	request({
		url: 'Contracts/GetContractDetailsById/GetContractDetails',
		method: 'GET',
		params: {
			contractId: row.contractId
		}
	}).then(response => {
		if (response.code === 200) {
			// 映射产品数据到表格
			detailsTableData.value = response.data.contractProducts.map(item => ({
				SaleContractID: row.contractId,
				id: item.id,
				productId: item.productID,
				productCode: item.productCode,
				customerCode: item.customerCode,
				chineseName: item.chineseName,
				englishName: item.englishName,
				chineseSpec: item.chineseSpec,
				unit: state.optionss.hr_calculate_unit.find(p => p.dictValue === item.unit.toString())?.dictLabel || '',
				contractQuantity: item.contractQuantity,
				purchaseUnitPrice: item.purchaseUnitPrice,
				purchaseTotalPrice: item.purchaseTotalPrice,
				packaging: state.optionss.hr_packing.find(p => p.dictValue === item.packaging.toString())?.dictLabel || '',
				specialRequirements: item.specialRequirements,
				invoice: item.invoice == 1 ? '是' : '否',
				innerBoxQuantity: item.innerBoxQuantity,
				outerBoxQuantity: item.outerBoxQuantity,
				remark: item.remark,
				originalPrice: item.purchaseUnitPrice// 保存原始价格用于比较
			}))
			GeneratePurchaseContractBtnShow.value = detailsTableData.value.every(
				item => item.productId !== 0 && item.productId !== null
			)
			if (GeneratePurchaseContractBtnShow.value == false) {
				ElMessage.warning('当前采购需求列表中存在新产品，请先点击“添加新产品”按钮添加产品信息,再生成采购合同');
			}
			viewDetailsDialog.value = true
		} else {
			ElMessage.error('获取详情失败，请重试')
		}
	}).catch(error => {
		console.error('获取详情失败:', error)
		ElMessage.error('获取详情失败，请重试')
	})
}

// 处理详情页面的价格变化
const handleDetailsPriceChange = (row) => {
	// 确保输入的是数字
	const currentPrice = parseFloat(row.purchaseUnitPrice) || 0
	const originalPrice = parseFloat(row.originalPrice) || 0
	// 计算新的总价
	row.purchaseTotalPrice = (row.contractQuantity * currentPrice).toFixed(2)

	// 检查是否有价格变动（使用精确比较）
	hasPriceChanges.value = Math.abs(currentPrice - originalPrice) > 0.000001
}

// 通知销售方法
const notifySales = () => {
	ElMessageBox.confirm('确认通知销售价格变动？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 构建更新请求
		const editRequest = {
			contractId: parseInt(currentDetailRow.value.contractId),
			contractProductItems: detailsTableData.value.map(item => ({
				id: item.id,
				productId: item.productId,
				purchaseUnitPrice: parseFloat(item.purchaseUnitPrice),
				purchaseTotalPrice: parseFloat(item.purchaseTotalPrice),
				isPriceChanged: 1
			}))
		}
		// 调用更新价格接口
		request.post("contracts/ContractPurchasePriceChanges/UpdatePrice", editRequest)
			.then(response => {
				if (response.code == 200) {
					ElMessage.success('已通知销售价格变动！')
					viewDetailsDialog.value = false
					//更新生成采购合同状态
					updateGenerateStatusByContractId(editRequest.contractId);
				} else {
					ElMessage.error('通知失败，请重试')
				}
				// 刷新列表
				ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value)
			})
			.catch(error => {
				console.error('通知销售失败:', error)
				ElMessage.error('通知失败，请重试')
			})
	}).catch(() => {
		ElMessage.info('已取消通知')
	})
}

// 更新生成状态
const updateGenerateStatusByContractId = (contractId) => {
	return new Promise((resolve, reject) => {
		request({
			url: 'PurchaseContracts/UpdateIsGenerateByContractID/EditGenerate',
			method: 'GET',
			params: {
				ContractID: contractId
			}
		}).then(response => {
			if (response.code == 200) {
				// 供应商简称
				resolve(response);  // Resolve the promise with the response data
			} else {
				console.error('获取供应商信息失败');
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
};

// 获取产品信息（包括供应商信息）
const getProductInfo = async (productId) => {
	try {
		console.log('获取产品信息，产品ID:', productId);
		const response = await request({
			url: 'ProductInformation/GetProductInfo/GetProductInfo',
			method: 'GET',
			params: { ID: productId }
		});
		console.log('产品信息响应:', response);
		return response.data;
	} catch (error) {
		console.error('获取产品信息失败:', error);
		return null;
	}
};

// 获取产品对应的供应商列表
const getProductSuppliers = (productId) => {
	return new Promise(async (resolve, reject) => {
		try {
			console.log('获取产品供应商，产品ID:', productId);
			const productInfo = await getProductInfo(productId);
			console.log('产品信息:', productInfo);
			console.log('产品信息的所有字段:', Object.keys(productInfo || {}));

			// 检查可能的供应商字段名
			const possibleSupplierFields = ['supplierID', 'supplierId', 'supplier_id', 'supplierIds', 'supplier_ids', 'suppliers'];
			possibleSupplierFields.forEach(field => {
				if (productInfo && productInfo[field]) {
					console.log(`找到供应商字段 ${field}:`, productInfo[field]);
				}
			});

			if (productInfo && productInfo.supplierID) {
				console.log('产品供应商ID:', productInfo.supplierID);
				// 将供应商ID字符串转换为数组
				const supplierIds = productInfo.supplierID.split(',').map(id => id.trim());
				console.log('供应商ID数组:', supplierIds);
				console.log('可用供应商选项:', state.optionss.sql_supplier_info);

				// 根据供应商ID获取对应的供应商信息
				const suppliers = supplierIds.map(id =>
					state.optionss.sql_supplier_info.find(item => item.dictValue === id)
				).filter(Boolean);
				console.log('匹配的供应商:', suppliers);
				resolve(suppliers);
			} else {
				console.log('产品没有供应商信息，显示所有可用供应商');
				// 如果产品没有供应商信息，显示所有可用的供应商
				resolve(state.optionss.sql_supplier_info || []);
			}
		} catch (error) {
			console.error('获取产品供应商失败:', error);
			reject(error);
		}
	});
};

// 加载产品对应的供应商列表
const loadProductSuppliers = async (row) => {
	console.log('加载产品供应商，行数据:', row);
	console.log('产品ID:', row.productId);
	console.log('是否已加载:', productSuppliersMap.value.has(row.productId));

	if (!row.productId || productSuppliersMap.value.has(row.productId)) {
		console.log('跳过加载，产品ID为空或已加载');
		return; // 如果已经加载过，直接返回
	}

	try {
		const suppliers = await getProductSuppliers(row.productId);
		console.log('获取到的供应商列表:', suppliers);
		productSuppliersMap.value.set(row.productId, suppliers);
		console.log('供应商映射已更新:', productSuppliersMap.value);
	} catch (error) {
		console.error('加载产品供应商失败:', error);
		ElMessage.error('加载供应商列表失败');
	}
};

// 定义表单验证规则
const rules = ref({
	paymentDays: [
		{ required: true, message: '请输入付款天数', trigger: 'blur' },
		{ type: 'number', message: '付款天数必须为数字', trigger: 'blur' },
		{ min: 0, message: '付款天数不能小于0', trigger: 'blur' }
	]
	// ... 其他已有的验证规则 ...
});

// 处理数量变化
const handleQuantityChange = (row) => {
	// 确保输入的是数字
	row.contractQuantity = parseFloat(row.contractQuantity) || 0;
	// 计算总价
	calculateTotalPrice(row);
}

// 处理单价变化
const handlePriceChange = (row) => {
	// 确保输入的是数字
	row.purchaseUnitPrice = parseFloat(row.purchaseUnitPrice) || 0;
	// 计算总价
	calculateTotalPrice(row);
}

// 计算总价
const calculateTotalPrice = (row) => {
	row.purchaseTotalPrice = (row.contractQuantity * row.purchaseUnitPrice).toFixed(2);
	// 更新合计信息
	updateTotalValues();
}

// 更新合计信息
const updateTotalValues = () => {
	let totalValue = 0;
	let totalQuantity = 0;

	productinfotableData.value.forEach(item => {
		totalValue += parseFloat(item.purchaseTotalPrice) || 0;
		totalQuantity += parseFloat(item.contractQuantity) || 0;
	});

	// 更新合计表单 - 采购金额合计自动计算
	Totalvalueofgoodsform.value.totalValue = totalValue.toFixed(2);
	Totalvalueofgoodsform.value.totalQuantity = totalQuantity.toString();
}

var userId = useUserStore().userId;
var CheckUser = ref(userId.toString()); // 初始化为当前用户ID

// 自定义参数序列化函数：将数组序列化为重复参数名格式（.NET Core List<int> 期望的格式）
// 例如：ContractIDList=1&ContractIDList=2
const dotNetArrayParamsSerializer = (params) => {
	const parts = [];
	for (const key in params) {
		if (Array.isArray(params[key])) {
			params[key].forEach(value => {
				parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
			});
		} else {
			parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`);
		}
	}
	return parts.join('&');
};

// 按钮显示控制
const isSaveBtnShow = ref(false);        // 确定保存按钮(新增时使用)
const showEditBtn = ref(false);          // 编辑按钮
const showEditSaveBtn = ref(false);      // 编辑保存按钮
const showSubmitReviewBtn = ref(false);  // 提交审核按钮
const isFormDisabled = ref(true);        // 表单是否禁用
const showApproveRejectBtn = ref(false);      // 驳回按钮
const showApprovePassBtn = ref(false);        // 审核按钮
const isSalesContractDisabled = ref(false);   // 销售合同是否禁用

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
		hr_quotation_basis: [],
		hr_outerbox_unit: [],
		hr_calculate_unit: [],
		hr_supplier_level: [],
		hr_business_scope: [],
		hr_china_provinces: [],
		hr_china_city: [],
		sql_sale_contracts: [],
		sql_supplier_info: [],
		sql_hr_purchase: [],
		hr_packing: [],
		sql_product: [],
		hr_purchase_pricing_term: [],
		hr_purchase_payment_days: [],
		hr_factory_payment: [],
		sql_all_user: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_supplier_level' }, { dictType: 'hr_business_scope' }, { dictType: 'hr_china_provinces' }, { dictType: 'hr_china_city' }, { dictType: 'sql_sale_contracts' },
{ dictType: 'sql_supplier_info' }, { dictType: 'sql_hr_purchase' }, { dictType: 'hr_packing' }, { dictType: 'sql_product' }, { dictType: 'hr_purchase_pricing_term' }, { dictType: 'hr_purchase_payment_days' }, { dictType: 'hr_factory_payment' }, { dictType: 'sql_all_user' }]


async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		console.log('字典数据加载完成:', state.optionss);
		console.log('供应商信息:', state.optionss.sql_supplier_info);
		await ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);  // 现在可以安全执行
		await GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
/*动态下拉框end*/

const activeName = ref('productinfo')
const basicInfoCollapseActive = ref(['basicInfo'])
const totalInfoCollapseActive = ref(['totalInfo'])
const CustomerRelaterExoensesTableData = ref([]);

const Addcontractofpurchasedialog = ref(false)
const Addcontractofpurchaseform = ref({
	purchaseContract: '',
	contractStatus: '',
	salesContract: '',
	customerContract: '',
	customerAbbreviation: '',
	deliveryDate: '',
	purchaseCurrency: '',
	deposit: '0',
	salesperson: '',
	purchaser: '',
	paymentDays: '',
	priceTerms: '',
	customerid: '',
	hasDeposit: false,
	deliveryLocation: ''
})



const currencyChange = async (row) => {
	if (state.optionss['hr_export_currency'].filter(hr_export_currency => hr_export_currency.dictValue == row.currency).map(item => item.dictValue).values().next().value == 3) {
		row.exchangeRate = Number(1);
	} else {
		// 获取最新汇率
		try {
			const latestRate = await exchangeRateService.getLatestExchangeRate(row.currency);
			if (latestRate !== null) {
				row.exchangeRate = Number(latestRate);
			} else {
				// 如果获取不到最新汇率，使用默认汇率
				const defaultRate = exchangeRateService.getDefaultExchangeRate(row.currency);
				row.exchangeRate = Number(defaultRate);
				ElMessage.warning(`未找到${exchangeRateService.getCurrencyName(row.currency, state.optionss.hr_export_currency)}的最新汇率，已使用默认汇率`);
			}
		} catch (error) {
			console.error('获取汇率失败:', error);
			// 使用默认汇率
			const defaultRate = exchangeRateService.getDefaultExchangeRate(row.currency);
			row.exchangeRate = Number(defaultRate);
			ElMessage.warning(`获取汇率失败，已使用默认汇率`);
		}
	}
	// 币种变化后重新计算金额
	expenseChange(row);
}

const expenseChange = (row) => {
	// 确保输入的是数字
	const expense = parseFloat(row.expense) || 0;
	const exchangeRate = parseFloat(row.exchangeRate) || 0;
	// 计算金额
	row.amount = (expense * exchangeRate).toFixed(2);
}

const handleDelete = (index) => {
	CustomerRelaterExoensesTableData.value.splice(index, 1);
}

// 新增厂家相关费用
const addNewExpense = () => {
	const newExpense = {
		id: 0,
		expenseName: '',
		supplier: '', // 供应商
		currency: '3', // 默认人民币
		exchangeRate: 1, // 默认汇率1
		expense: 0,
		amount: 0,
		remark: ''
	};
	CustomerRelaterExoensesTableData.value.push(newExpense);
}

// 处理预付款变化
const handleDepositChange = (checked) => {
	if (!checked) {
		// 如果取消选中，将预付款金额设置为0
		Addcontractofpurchaseform.value.deposit = '0';
	}
}

// 处理产品预付款变化
const handleProductDepositChange = (row) => {
	if (!row.hasDeposit) {
		// 如果取消选中，将预付款金额设置为0
		row.depositAmount = 0;
	}
}

// 处理产品预付款金额变化
const handleDepositAmountChange = (row) => {
	// 确保输入的是数字
	row.depositAmount = parseFloat(row.depositAmount) || 0;
}

// 存储产品的初始开票状态，用于确定价格条款选项
const initialInvoiceStates = ref(new Set());

// 根据产品初始开票状态过滤价格条款选项
const filteredPriceTermsOptions = computed(() => {
	if (!productinfotableData.value || productinfotableData.value.length === 0) {
		return optionss.value.hr_purchase_pricing_term;
	}

	// 使用初始开票状态来判断，而不是当前的开票状态
	const hasInvoiceProducts = initialInvoiceStates.value.has(true);
	const hasNoInvoiceProducts = initialInvoiceStates.value.has(false);

	// 如果所有产品都是开票的，只显示含税选项（7和8）
	if (hasInvoiceProducts && !hasNoInvoiceProducts) {
		return optionss.value.hr_purchase_pricing_term.filter(option =>
			option.dictValue === '7' || option.dictValue === '8'
		);
	}
	// 如果所有产品都是不开票的，只显示不含税选项（9和10）
	else if (hasNoInvoiceProducts && !hasInvoiceProducts) {
		return optionss.value.hr_purchase_pricing_term.filter(option =>
			option.dictValue === '9' || option.dictValue === '10'
		);
	}
	// 如果产品混合了开票和不开票，显示所有选项
	else {
		return optionss.value.hr_purchase_pricing_term;
	}
});

// 获取产品资料列表中已选择的供应商（去重）
const selectedSuppliers = computed(() => {
	if (!productinfotableData.value || productinfotableData.value.length === 0) {
		return [];
	}

	// 收集所有已选择的供应商ID（去重）
	const supplierIds = new Set();
	productinfotableData.value.forEach(product => {
		if (product.supplier && product.supplier !== null && product.supplier !== '') {
			supplierIds.add(product.supplier.toString());
		}
	});

	// 根据供应商ID从字典中获取供应商信息
	const suppliers = Array.from(supplierIds)
		.map(id => state.optionss.sql_supplier_info.find(item => item.dictValue === id))
		.filter(Boolean); // 过滤掉未找到的供应商

	return suppliers;
});

// 处理价格条款变化
const handlePriceTermsChange = (value) => {
	// 价格条款变化时不再自动更新产品的开票状态
	// 产品的开票状态保持用户手动设置的值
	console.log('价格条款已更改为:', value);
}

const contractofpurchaseRequest = reactive({
	id: 0,
	PurchaseContractNumber: '',
	ContractStatus: null,
	VendorCode: '',
	VendorAbbreviation: '',
	SalesContract: '',
	CustomerContract: '',
	CustomerAbbreviation: '',
	DeliveryDate: null,
	PurchaseCurrency: null,
	Deposit: null,
	Salesperson: '',
	Purchaser: '',
	PaymentDays: null,
	PriceTerms: '',
	TotalGoodsValue: null,
	TotalQuantity: null,
	TotalBoxes: null,
	TotalGrossWeight: null,
	TotalNetWeight: null,
	TotalVolume: null,
	AppliedPayment: null,
	AvailablePayment: null,
	PaidAmount: null,
	UnpaidAmount: null,
	Remark: '',
	IsDelete: 0,
	PurchaseContractProducts: [],
	PurchaseContractVendorExpenses: []
});

const SavePurchaseContract = () => {
	ElMessageBox.confirm("确认保存采购合同信息吗？", "提示", {
		confirmButtonText: "确定",
		cancelButtonText: "取消",
		type: "warning",
	}).then(() => {
		submitPurchaseContract();
	}).catch(() => {
		ElMessage({
			type: "info",
			message: "已取消保存采购合同",
		});
	});
};

const NewPurchaseContractID = ref(0);//采购合同ID
// 提交采购合同的具体逻辑
const submitPurchaseContract = () => {
	// 映射产品数据以匹配后端模型
	const mappedProducts = productinfotableData.value.map(product => ({
		id: product.id || 0,
		productId: product.productId || 0,
		SupplierID: product.supplier || 0, // 使用产品表格中的供应商
		purchaseContractId: 0, // 新增时为0
		productCode: product.productCode || '',
		customerCode: product.customerCode || '',
		chineseName: product.chineseName || '',
		englishName: product.englishName || '',
		chineseSpec: product.chineseSpecification || '',
		contractQuantity: parseFloat(product.contractQuantity),
		unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
		purchaseCurrency: parseInt(product.purchasecurrency || product.purchaseCurrency || '3'),
		purchasePrice: parseFloat(product.purchaseUnitPrice),
		PurchasingPriceTerms: parseInt(product.purchasePriceTerms) || 0,
		purchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
		deliveryDate: product.deliveryDate,
		productionLeadTime: product.productionLeadTime,
		packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
		specialRequirements: product.specialRequirements || '',
		invoice: product.invoice === 1 || product.invoice === '是' || product.invoice === '1' ? 1 : 0,
		innerBoxQuantity: parseInt(product.innerBoxQuantity),
		outerBoxQuantity: parseInt(product.outerBoxQuantity),
		hasDeposit: product.hasDeposit ? 1 : 0,
		depositAmount: parseFloat(product.depositAmount) || 0,
		remark: product.remark || '',
		isDelete: 0,
		createTime: new Date(),
		updateTime: new Date(),
		createBy: userId,
		updateBy: userId
	}));

	// 映射厂商费用数据
	const mappedExpenses = CustomerRelaterExoensesTableData.value.map(expense => ({
		id: expense.id || 0,
		purchaseContractID: 0, // 新增时为0
		expenseName: expense.expenseName,
		supplierID: expense.supplier ? parseInt(expense.supplier) : 0, // 供应商ID
		currency: parseInt(expense.currency),
		exchangeRate: parseFloat(expense.exchangeRate),
		expense: parseFloat(expense.expense),
		remark: expense.remark || '',
		isDelete: 0,
		createTime: new Date(),
		updateTime: new Date(),
		createBy: userId,
		updateBy: userId
	}));

	// 构建合同请求数据
	const contractRequest = {
		id: 0, // 新增时为0
		customerId: Addcontractofpurchaseform.value.customerid,
		purchaseContractNumber: Addcontractofpurchaseform.value.purchaseContract,
		contractStatus: parseInt(Addcontractofpurchaseform.value.contractStatus),
		vendorCode: '',
		vendorAbbreviation: '',
		salesContract: Addcontractofpurchaseform.value.salesContract,
		customerContract: Addcontractofpurchaseform.value.customerContract,
		customerAbbreviation: Addcontractofpurchaseform.value.customerAbbreviation,
		deliveryDate: Addcontractofpurchaseform.value.deliveryDate,
		purchaseCurrency: parseInt(Addcontractofpurchaseform.value.purchaseCurrency),
		deposit: parseFloat(Addcontractofpurchaseform.value.deposit || '0'),
		salesperson: Addcontractofpurchaseform.value.salesperson,
		purchaser: userId.toString(), // 确保设置采购员为当前用户ID
		paymentDays: parseInt(Addcontractofpurchaseform.value.paymentDays),
		priceTerms: Addcontractofpurchaseform.value.priceTerms,
		totalGoodsValue: parseFloat(Totalvalueofgoodsform.value.totalValue),
		totalQuantity: parseFloat(Totalvalueofgoodsform.value.totalQuantity),
		totalBoxes: parseInt(Totalvalueofgoodsform.value.totalBoxCount),
		totalGrossWeight: parseFloat(Totalvalueofgoodsform.value.totalGrossWeight),
		totalNetWeight: parseFloat(Totalvalueofgoodsform.value.totalNetWeight),
		totalVolume: parseFloat(Totalvalueofgoodsform.value.totalVolume),
		appliedPayment: parseFloat(Totalvalueofgoodsform.value.appliedPayment || '0'),
		availablePayment: parseFloat(Totalvalueofgoodsform.value.availablePayment || '0'),
		paidAmount: parseFloat(Totalvalueofgoodsform.value.paidAmount || '0'),
		unpaidAmount: parseFloat(Totalvalueofgoodsform.value.unpaidAmount || '0'),
		remark: '',
		isDelete: 0,
		createTime: new Date(),
		updateTime: new Date(),
		createBy: userId,
		updateBy: userId,
		purchaseContractProducts: mappedProducts,
		purchaseContractVendorExpenses: mappedExpenses,
		isDraft: 1,
		ProcurementRequirementID: ProcurementRequirementID.value,
		hasdeposit: Addcontractofpurchaseform.value.hasDeposit ? 1 : 0, // 转换为数字：0否，1是
		SupplierID: 0, // 供应商ID设为0，因为不再使用基本信息中的供应商
		deliveryLocation: Addcontractofpurchaseform.value.deliveryLocation || '', // 交货地点
		salesContractID: parseInt(Addcontractofpurchaseform.value.salesContract) || 0 // 销售合同ID
	};
	// 提交采购合同
	request.post("PurchaseContracts/AddPurchaseContracts/Add", contractRequest)
		.then(response => {
			if (response.code === 200) {
				// 处理返回数据：可能是单个对象或数组
				const result = response.data;
				let contractIds = [];
				let contractNumbers = [];

				if (Array.isArray(result)) {
					// 多个供应商，返回数组
					contractIds = result.map(item => item.purchaseContractID);
					contractNumbers = result.map(item => item.purchaseContractNumber);
					ElMessage({
						message: `采购合同添加成功！共生成 ${result.length} 个合同：${contractNumbers.join(', ')}`,
						type: "success",
						duration: 5000
					});
				} else {
					// 单个供应商，返回对象
					contractIds = [result.purchaseContractID];
					contractNumbers = [result.purchaseContractNumber];
					ElMessage({
						message: `采购合同添加成功！合同号：${result.purchaseContractNumber}`,
						type: "success",
					});
				}

				Addcontractofpurchasedialog.value = false;
				GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
				if (isGeneratedFromRequirement.value) {
					if (contractRequest.isDraft == 0) {
						updateGenerateStatusByContractId(SaleContractID.value);
					}
					ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
				}
			} else {
				ElMessage.error(response.msg || '添加采购合同失败');
			}
		})
		.catch(error => {
			if (error.response?.data?.errors) {
				const errorMessages = Object.values(error.response.data.errors)
					.flat()
					.join(', ');
				ElMessage.error(`验证失败: ${errorMessages}`);
			} else {
				ElMessage.error('添加采购合同失败');
			}
			console.error("添加采购合同失败:", error);
		});
};

const Totalvalueofgoodsform = ref({
	totalValue: '',
	totalQuantity: '',
	totalBoxCount: '',
	totalGrossWeight: '',
	totalNetWeight: '',
	totalVolume: '',
	appliedPayment: '',
	availablePayment: '',
	paidAmount: '',
	unpaidAmount: ''
})

const editContract = () => {
	setPurchaseContractEditLock(currentContractId.value);
	showEditBtn.value = false;
	showEditSaveBtn.value = true;
	showSubmitReviewBtn.value = true;
	isFormDisabled.value = false;         // 启用表单编辑
}
// 保存编辑数据（不显示确认框，用于内部调用）
const saveEditContractData = async () => {
	// 构建产品数据
	const mappedProducts = productinfotableData.value.map(product => ({
		Id: product.id || 0,
		ProductId: product.productId || 0,
		PurchaseContractId: currentContractId.value,
		ProductNumber: product.productNumber || '',
		SupplierID: parseInt(product.supplier) || 0, // 使用产品表格中的供应商
		ProductCode: product.productCode || '',
		CustomerCode: product.customerCode || '',
		ChineseName: product.chineseName || '',
		EnglishName: product.englishName || '',
		chineseSpec: product.chineseSpecification || '',
		ContractQuantity: parseFloat(product.contractQuantity) || 0,
		Unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
		PurchaseCurrency: parseInt(product.purchasecurrency || product.purchaseCurrency || '3'),
		PurchasePrice: parseFloat(product.purchaseUnitPrice),
		PurchasingPriceTerms: parseInt(product.purchasePriceTerms) || 0,
		PurchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
		DeliveryDate: product.deliveryDate,
		ProductionLeadTime: product.productionLeadTime,
		Packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
		SpecialRequirements: product.specialRequirements || '',
		Invoice: product.invoice === 1 || product.invoice === '是' || product.invoice === '1' ? 1 : 0,
		InnerBoxQuantity: parseInt(product.innerBoxQuantity),
		OuterBoxQuantity: parseInt(product.outerBoxQuantity),
		HasDeposit: product.hasDeposit ? 1 : 0,
		DepositAmount: parseFloat(product.depositAmount) || 0,
		Remark: product.remark || '',
		IsDelete: 0
	}));

	// 构建厂商费用数据
	const mappedExpenses = CustomerRelaterExoensesTableData.value.map(expense => ({
		id: expense.id || 0,
		PurchaseContractID: currentContractId.value,
		expenseName: expense.expenseName,
		SupplierID: expense.supplier ? parseInt(expense.supplier) : 0, // 供应商ID
		currency: parseInt(expense.currency),
		exchangeRate: parseFloat(expense.exchangeRate),
		expense: parseFloat(expense.expense),
		Remark: expense.remark || '',
		IsDelete: 0
	}));

	// 解析必填项 ID：表单可能为 dictValue 或 dictLabel，确保提交时不为空
	const resolveDictValue = (options, formVal, fallback) => {
		if (formVal == null || formVal === '') return fallback;
		const str = formVal.toString();
		if (/^\d+$/.test(str)) return str;
		const found = options?.find(item => item.dictLabel === str || item.dictValue === str);
		return found?.dictValue ?? fallback;
	};
	const salesContractVal = resolveDictValue(state.optionss.sql_sale_contracts, Addcontractofpurchaseform.value.salesContract, '');
	const salespersonVal = resolveDictValue(state.optionss.sql_hr_sale, Addcontractofpurchaseform.value.salesperson, '');
	const purchaserVal = resolveDictValue(state.optionss.sql_all_user, Addcontractofpurchaseform.value.purchaser, (userId != null ? userId.toString() : useUserStore().userId?.toString()) || '');

	// 构建请求数据
	const purchaseContractsRequest = {
		id: currentContractId.value,
		PurchaseContractNumber: Addcontractofpurchaseform.value.purchaseContract,
		ContractStatus: 1,
		VendorCode: '',
		VendorAbbreviation: '',
		SalesContract: salesContractVal,
		CustomerContract: Addcontractofpurchaseform.value.customerContract,
		CustomerAbbreviation: Addcontractofpurchaseform.value.customerAbbreviation,
		DeliveryDate: Addcontractofpurchaseform.value.deliveryDate,
		PurchaseCurrency: state.optionss.hr_export_currency.find(item => item.dictLabel === Addcontractofpurchaseform.value.purchaseCurrency.toString())?.dictValue,
		Deposit: parseFloat(Addcontractofpurchaseform.value.deposit || '0'),
		Salesperson: salespersonVal,
		Purchaser: purchaserVal,
		PaymentDays: parseInt(Addcontractofpurchaseform.value.paymentDays || '0'),
		PriceTerms: Addcontractofpurchaseform.value.priceTerms,
		TotalGoodsValue: parseFloat(Totalvalueofgoodsform.value.totalValue || '0'),
		TotalQuantity: parseFloat(Totalvalueofgoodsform.value.totalQuantity || '0'),
		TotalBoxes: parseInt(Totalvalueofgoodsform.value.totalBoxCount || '0'),
		TotalGrossWeight: parseFloat(Totalvalueofgoodsform.value.totalGrossWeight || '0'),
		TotalNetWeight: parseFloat(Totalvalueofgoodsform.value.totalNetWeight || '0'),
		TotalVolume: parseFloat(Totalvalueofgoodsform.value.totalVolume || '0'),
		AppliedPayment: parseFloat(Totalvalueofgoodsform.value.appliedPayment || '0'),
		AvailablePayment: parseFloat(Totalvalueofgoodsform.value.availablePayment || '0'),
		PaidAmount: parseFloat(Totalvalueofgoodsform.value.paidAmount || '0'),
		UnpaidAmount: parseFloat(Totalvalueofgoodsform.value.unpaidAmount || '0'),
		Remark: '',
		IsDelete: 0,
		PurchaseContractProducts: mappedProducts,
		PurchaseContractVendorExpenses: mappedExpenses,
		isDraft: 1,
		ProcurementRequirementID: ProcurementRequirementID.value,
		hasdeposit: Addcontractofpurchaseform.value.hasDeposit ? 1 : 0, // 转换为数字：0否，1是
		SupplierID: 0, // 供应商ID设为0，因为不再使用基本信息中的供应商
		deliveryLocation: Addcontractofpurchaseform.value.deliveryLocation || '', // 交货地点
		salesContractID: parseInt(Addcontractofpurchaseform.value.salesContract) || 0 // 销售合同ID
	};

	// 发送请求
	const response = await request.post("PurchaseContracts/EditPurchaseContracts/Edit", purchaseContractsRequest);

	if (response.code === 200) {
		console.log('编辑保存成功');
		return true;
	} else {
		throw new Error(response.msg || '编辑采购合同失败');
	}
};

// 保存编辑
const saveEditContract = () => {
	ElMessageBox.confirm('确认保存编辑的采购合同信息吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		try {
			await saveEditContractData();
			ElMessage({
				message: "采购合同编辑成功！",
				type: "success",
			});
			showEditBtn.value = true;
			showEditSaveBtn.value = false;
			showSubmitReviewBtn.value = true;
			isFormDisabled.value = true;
			Addcontractofpurchasedialog.value = false;
			GetpurchaseContractList(
				purchasecontractsTableDatacurrentPage.value,
				purchasecontractsTableDatapageSize.value
			);
		} catch (error) {
			console.error('编辑采购合同失败:', error);
			ElMessage.error('编辑采购合同失败，请重试');
		}
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消编辑'
		});
	});
};



// 添加一个变量来存储当前操作的合同ID
const currentContractId = ref(null);
// 提交审核
const submitForReview = () => {
	ElMessageBox.confirm('确认要提交审核吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		if (currentContractId.value == null) {
			try {
				// 映射产品数据以匹配后端模型
				const mappedProducts = productinfotableData.value.map(product => ({
					id: product.id || 0,
					ProductID: product.productId || 0,
					SupplierID: product.supplier || 0, // 使用产品表格中的供应商
					purchaseContractId: 0, // 新增时为0
					productCode: product.productCode,
					customerCode: product.customerCode || '',
					chineseName: product.chineseName || '',
					englishName: product.englishName || '',
					chineseSpec: product.chineseSpec || '',
					contractQuantity: parseFloat(product.contractQuantity),
					unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
					purchaseCurrency: parseInt(product.purchasecurrency || product.purchaseCurrency || '3'),
					purchasePrice: parseFloat(product.purchaseUnitPrice),
					PurchasingPriceTerms: parseInt(product.purchasePriceTerms) || 0,
					purchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
					deliveryDate: product.deliveryDate,
					productionLeadTime: product.productionLeadTime,
					packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
					specialRequirements: product.specialRequirements || '',
					invoice: product.invoice === 1 || product.invoice === '是' || product.invoice === '1' ? 1 : 0,
					innerBoxQuantity: parseInt(product.innerBoxQuantity),
					outerBoxQuantity: parseInt(product.outerBoxQuantity),
					hasDeposit: product.hasDeposit ? 1 : 0,
					depositAmount: parseFloat(product.depositAmount) || 0,
					remark: product.remark || '',
					isDelete: 0,
					createTime: new Date(),
					updateTime: new Date(),
					createBy: userId,
					updateBy: userId
				}));

				// 映射厂商费用数据
				const mappedExpenses = CustomerRelaterExoensesTableData.value.map(expense => ({
					id: expense.id || 0,
					purchaseContractID: 0, // 新增时为0
					expenseName: expense.expenseName,
					supplierID: expense.supplier ? parseInt(expense.supplier) : 0, // 供应商ID
					currency: parseInt(expense.currency),
					exchangeRate: parseFloat(expense.exchangeRate),
					expense: parseFloat(expense.expense),
					remark: expense.remark || '',
					isDelete: 0,
					createTime: new Date(),
					updateTime: new Date(),
					createBy: userId,
					updateBy: userId
				}));

				// 构建合同请求数据
				const contractRequest = {
					id: 0, // 新增时为0
					customerId: Addcontractofpurchaseform.value.customerid,
					purchaseContractNumber: Addcontractofpurchaseform.value.purchaseContract,
					contractStatus: parseInt(Addcontractofpurchaseform.value.contractStatus),
					vendorCode: '',
					vendorAbbreviation: '',
					salesContract: Addcontractofpurchaseform.value.salesContract,
					customerContract: Addcontractofpurchaseform.value.customerContract,
					customerAbbreviation: Addcontractofpurchaseform.value.customerAbbreviation,
					deliveryDate: Addcontractofpurchaseform.value.deliveryDate,
					purchaseCurrency: parseInt(Addcontractofpurchaseform.value.purchaseCurrency),
					deposit: parseFloat(Addcontractofpurchaseform.value.deposit || '0'),
					salesperson: Addcontractofpurchaseform.value.salesperson,
					purchaser: userId.toString(), // 确保设置采购员为当前用户ID
					paymentDays: parseInt(Addcontractofpurchaseform.value.paymentDays),
					priceTerms: Addcontractofpurchaseform.value.priceTerms,
					totalGoodsValue: parseFloat(Totalvalueofgoodsform.value.totalValue),
					totalQuantity: parseFloat(Totalvalueofgoodsform.value.totalQuantity),
					totalBoxes: parseInt(Totalvalueofgoodsform.value.totalBoxCount),
					totalGrossWeight: parseFloat(Totalvalueofgoodsform.value.totalGrossWeight),
					totalNetWeight: parseFloat(Totalvalueofgoodsform.value.totalNetWeight),
					totalVolume: parseFloat(Totalvalueofgoodsform.value.totalVolume),
					appliedPayment: parseFloat(Totalvalueofgoodsform.value.appliedPayment || '0'),
					availablePayment: parseFloat(Totalvalueofgoodsform.value.availablePayment || '0'),
					paidAmount: parseFloat(Totalvalueofgoodsform.value.paidAmount || '0'),
					unpaidAmount: parseFloat(Totalvalueofgoodsform.value.unpaidAmount || '0'),
					remark: '',
					isDelete: 0,
					createTime: new Date(),
					updateTime: new Date(),
					createBy: userId,
					updateBy: userId,
					purchaseContractProducts: mappedProducts,
					purchaseContractVendorExpenses: mappedExpenses,
					isDraft: 1,
					ProcurementRequirementID: ProcurementRequirementID.value,
					hasdeposit: Addcontractofpurchaseform.value.hasDeposit ? 1 : 0, // 转换为数字：0否，1是
					SupplierID: 0, // 供应商ID设为0，因为不再使用基本信息中的供应商
					deliveryLocation: Addcontractofpurchaseform.value.deliveryLocation || '', // 交货地点
					salesContractID: parseInt(Addcontractofpurchaseform.value.salesContract) || 0 // 销售合同ID
				};
				// 提交采购合同
				request.post("PurchaseContracts/AddPurchaseContracts/Add", contractRequest)
					.then(async response => {
						if (response.code === 200) {
							// 处理返回数据：可能是单个对象或数组
							const result = response.data;
							let contractIds = [];

							if (Array.isArray(result)) {
								// 多个供应商，返回数组
								contractIds = result.map(item => item.purchaseContractID);
							} else {
								// 单个供应商，返回对象
								contractIds = [result.purchaseContractID];
							}

							// 批量提交审核
							// 直接在 URL 中构建查询字符串，避免 Content-Type 问题
							// 格式：ContractIDList=1&ContractIDList=2（.NET Core List<int> 期望的格式）
							const queryString = contractIds.map(id => `ContractIDList=${id}`).join('&');
							const Reviewresponse = await request({
								url: `PurchaseContracts/SubmitForReview/SubmitPurchseContractReview?${queryString}`,
								method: 'GET'
							});
							if (Reviewresponse.code === 200) {
								ElMessage.success(Reviewresponse.msg || '提交审核成功');
								Addcontractofpurchasedialog.value = false;  // 关闭对话框
								// 刷新采购合同列表
								await GetpurchaseContractList(
									purchasecontractsTableDatacurrentPage.value,
									purchasecontractsTableDatapageSize.value
								);
								await updateGenerateStatusByContractId(SaleContractID.value);
								await ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);

							} else {
								ElMessage.error(Reviewresponse.msg || '提交审核失败');
							}
						} else {
							ElMessage.error(response.msg || '提交审核失败,请重试');
						}
					})
					.catch(error => {
						if (error.response?.data?.errors) {
							const errorMessages = Object.values(error.response.data.errors)
								.flat()
								.join(', ');
							ElMessage.error(`验证失败: ${errorMessages}`);
						} else {
							ElMessage.error('添加采购合同失败');
						}
						console.error("添加采购合同失败:", error);
					});
			} catch (error) {
				console.error('提交审核失败:', error);
				ElMessage.error('提交审核失败，请重试');
			}
		} else {
			// 编辑合同时，先保存数据再提交审核
			try {
				// 先调用保存编辑函数
				await saveEditContractData();

				// 保存成功后，再提交审核（使用数组格式以支持批量）
				// 直接在 URL 中构建查询字符串，避免 Content-Type 问题
				// 格式：ContractIDList=1（.NET Core List<int> 期望的格式）
				const queryString = `ContractIDList=${currentContractId.value}`;
				const response = await request({
					url: `PurchaseContracts/SubmitForReview/SubmitPurchseContractReview?${queryString}`,
					method: 'GET'
				});

				if (response.code === 200) {
					ElMessage.success('提交审核成功');
					Addcontractofpurchasedialog.value = false;  // 关闭对话框
					// 刷新采购合同列表
					await GetpurchaseContractList(
						purchasecontractsTableDatacurrentPage.value,
						purchasecontractsTableDatapageSize.value
					);
					await ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
				} else {
					ElMessage.error('提交审核失败');
				}
			} catch (error) {
				console.error('提交审核失败:', error);
				ElMessage.error('提交审核失败，请重试');
			}
		}
	}).catch(() => {
		ElMessage.info('已取消提交审核');
	});
};


const handleClick = (tab, event) => {

}

const productinfotableData = ref([])
// 存储每个产品的供应商列表
const productSuppliersMap = ref(new Map())

// 采购计划表格数据
const shoppinglisttableData = ref([])
const contractsTableDatatotalItems = ref(0)
const contractsTableDatacurrentPage = ref(1)
const contractsTableDatapageSize = ref(10)

// 处理页码变化
const contractsTableDatahandlePageChange = async (newPage) => {
	try {
		contractsTableDatacurrentPage.value = newPage
		await ProcurementRequirements(newPage, contractsTableDatapageSize.value)
	} catch (error) {
		console.error('页码切换失败:', error)
		ElMessage.error('获取数据失败，请重试')
	}
}
// 获取采购需求列表数据
const ProcurementRequirements = async (pageNum, pageSize) => {
	try {
		const response = await request({
			url: 'PurchaseContracts/GetProcurementRequirements/GetList',
			method: 'GET',
			params: {
				PageNum: pageNum,
				PageSize: pageSize
			}
		})
		// 检查响应数据
		if (!response.data) {
			throw new Error('返回数据格式错误')
		}
		// 清空现有数据
		shoppinglisttableData.value = []
		// 如果有数据则处理
		if (response.data.result && response.data.result.length > 0) {
			shoppinglisttableData.value = response.data.result.map(element => ({
				contractId: element.contractId,
				productId: element.productId,
				procurementId: element.procurementId,
				contractNumber: element.contractNumber,
				productName: element.productName,
				quantity: element.quantity,
				unitPrice: element.unitPrice,
				totalPrice: element.totalPrice,
				status: element.status,
				salesperson: element.salesperson ? state.optionss.sql_hr_sale.find(item => item.dictValue === element.salesperson.toString())?.dictLabel || '未知' : '未知',
				createTime: formatDate(element.createTime),
				remark: element.remark
			}))
			// 更新总条数
			contractsTableDatatotalItems.value = response.data.totalNum
		} else {
			// 如果当前页无数据且不是第一页，则加载上一页
			if (response.data.totalNum > 0 && pageNum > 1) {
				return ProcurementRequirements(pageNum - 1, pageSize)
			}
			// 如果是第一页无数据，显示空数据
			contractsTableDatatotalItems.value = 0
		}
		return response.data
	} catch (error) {
		console.error('获取采购需求列表失败:', error)
		ElMessage.error('获取数据失败，请重试')
		throw error
	}
}
// 动态合并单元格
const mergeCells = ({ row, columnIndex, rowIndex }) => {
	// 合并销售合同号列（第1列）
	if (columnIndex === 0) {
		if (rowIndex === getFirstRowIndex(row.contractNumber)) {
			const rowspan = getRowSpan(row.contractNumber);
			return [rowspan, 1];
		} else {
			return [0, 0];
		}
	}

	// 合并操作列（最后一列）
	if (columnIndex === 6) {
		if (rowIndex === getFirstRowIndex(row.contractNumber)) {
			const rowspan = getRowSpan(row.contractNumber);
			return [rowspan, 1];
		} else {
			return [0, 0];
		}
	}
};

// 获取当前合同号的第一行索引
const getFirstRowIndex = (contractNumber) => {
	return shoppinglisttableData.value.findIndex(
		(item) => item.contractNumber === contractNumber
	);
};

// 获取当前合同号的合并行数
const getRowSpan = (contractNumber) => {
	return shoppinglisttableData.value.filter(
		(item) => item.contractNumber === contractNumber
	).length;
};

// 判断当前行是否为该合同号的第一行
const isFirstRow = (row) => {
	return row.id === shoppinglisttableData.value[getFirstRowIndex(row.contractNumber)].id;
};

// 添加日期格式化函数
const formatDate = (dateString) => {
	if (!dateString) return '';
	const date = new Date(dateString);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	return `${year}-${month}-${day}`;
}

const SaleContractID = ref(0);
const isGeneratedFromRequirement = ref(false); // 是否通过生成采购合同按钮触发

///采购合同表格数据
const contractofpurchasetableData = ref([])
const purchasecontractsTableDatatotalItems = ref(0);
const purchasecontractsTableDatacurrentPage = ref(1);
const purchasecontractsTableDatapageSize = ref(10);
const purchasecontractsTableDatahandlePageChange = async (newPage) => {
	purchasecontractsTableDatacurrentPage.value = newPage;
	const start = newPage;
	const end = purchasecontractsTableDatapageSize.value;
	const newData = await GetpurchaseContractList(start, end);
};
const reviewStatusMap = {
	'0': '待提审',
	'1': '审核中',
	'2': '已批准',
	'3': '已拒绝',
	'10': '申请完结'
}
function GetpurchaseContractList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'PurchaseContracts/GetpurchaseContractList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				contractofpurchasetableData.value = response.data.result;
				// 绑定数据
				contractofpurchasetableData.value.forEach(element => {
					// 保存原始合同状态值用于判断（如申请完结按钮显示条件）
					element.originalContractStatus = element.contractStatus != null && element.contractStatus !== '' ? parseInt(element.contractStatus) : 0;
					// 添加空值检查和默认值
					element.contractStatus = element.contractStatus ? optionss.value.hr_contract_status.find(item =>
						item.dictValue == element.contractStatus.toString())?.dictLabel || '未知状态' : '未知状态';

					// 移除供应商显示，因为不再使用基本信息中的供应商

					// 保存销售合同、销售员、采购员原始ID，供编辑提交时使用（避免提交时缺失必填项）
					element.salesContractId = element.salesContract != null && element.salesContract !== '' ? element.salesContract.toString() : '';
					element.salespersonId = element.salesperson != null && element.salesperson !== '' ? element.salesperson.toString() : '';
					element.purchaserId = element.purchaser != null && element.purchaser !== '' ? element.purchaser.toString() : '';

					element.salesContract = element.salesContract ? optionss.value.sql_sale_contracts.find(item =>
						item.dictValue == element.salesContract.toString())?.dictLabel || '未知合同' : '未知合同';

					element.purchaseCurrency = element.purchaseCurrency ? optionss.value.hr_export_currency.find(item =>
						item.dictValue == element.purchaseCurrency.toString())?.dictLabel || '未知货币' : '未知货币';

					element.salesperson = element.salesperson ? optionss.value.sql_hr_sale.find(item =>
						item.dictValue == element.salesperson.toString())?.dictLabel || '未知销售员' : '未知销售员';

					element.purchaser = element.purchaser ? optionss.value.sql_all_user.find(item =>
						item.dictValue == element.purchaser.toString())?.dictLabel || '未知采购员' : '未知采购员';

					// 格式化日期字段
					element.deliveryDate = formatDate(element.deliveryDate);
					element.createTime = formatDate(element.createTime);

					element.reviewStatus = element.reviewStatus ? element.reviewStatus.toString() : '0';
					element.reviewStatusStr = reviewStatusMap[element.reviewStatus] || '未知状态';
				});
				purchasecontractsTableDatatotalItems.value = response.data.totalNum;
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetpurchaseContractList(start - 1, end);
				} else {
					contractofpurchasetableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}


// 获取合同编辑锁状态
const getPurchaseContractEditLock = async (contractId) => {
	try {
		const res = await request({
			url: 'PurchaseContracts/GetPurchaseContractEditLock/GetPurchaseContractEditLock',
			method: 'get',
			params: { PurchaseContractID: contractId }
		});
		return res; // 返回锁定用户名，如果未锁定则为null
	} catch (error) {
		console.error('获取采购合同编辑锁失败:', error);
		return null;
	}
};

// 设置采购合同编辑锁
const setPurchaseContractEditLock = async (contractId) => {
	try {
		const res = await request({
			url: 'PurchaseContracts/SettingsPurchaseContractEditLock/SettingsPurchaseContractEditLock',
			method: 'get',
			params: { PurchaseContractID: contractId }
		});
		return res.code === 200;
	} catch (error) {
		console.error('设置采购合同编辑锁失败:', error);
		return false;
	}
};

// 移除采购合同编辑锁
const removePurchaseContractEditLock = async (contractId) => {
	try {
		await request({
			url: 'PurchaseContracts/RemovePurchaseContractEditLock/RemovePurchaseContractEditLock',
			method: 'get',
			params: { PurchaseContractID: contractId }
		});
	} catch (error) {
		console.error('移除采购合同编辑锁失败:', error);
	}
};

// 采购合同窗体关闭
const handleAddcontractofpurchasedialogclose = async () => {
	showApproveRejectBtn.value = false;
	showApprovePassBtn.value = false;
	if (CheckUser.toString() == userId.toString()) {
		await removePurchaseContractEditLock(currentContractId.value);
	}
	clearAll();
}

// 查看详情
const CheckDetails = async (row) => {
	// 先检查编辑锁
	const lockStatus = await getPurchaseContractEditLock(row.id);
	if (lockStatus.data.isEditLock == true) {
		ElMessageBox.alert(`当前采购合同正在被${lockStatus.data.editUser}编辑中，请稍后再试！`, '提示', {
			confirmButtonText: '确定',
			showClose: false
		});
		return;
	}
	isFormDisabled.value = true;
	currentContractId.value = row.id;  // 存储当前合同ID
	CheckUser = state.optionss.sql_all_user.find(p => p.dictLabel === (row.purchaser ? row.purchaser.toString() : ''))?.dictValue;
	// 重置所有按钮状态
	isSaveBtnShow.value = false;
	// 根据合同状态设置按钮显示
	if (row.contractStatus === "待确认" || (row.reviewStatus === "0" || row.reviewStatus === "3")) { // 草稿或驳回状态
		showEditBtn.value = true;
		showSubmitReviewBtn.value = true;
	} else {
		showEditBtn.value = false;
		showSubmitReviewBtn.value = false;
	}
	showEditSaveBtn.value = false;
	isFormDisabled.value = true;          // 禁用表单编辑
	isSaveBtnShow.value = false;

	Addcontractofpurchaseform.value.purchaseContract = row.purchaseContractNumber;
	Addcontractofpurchaseform.value.contractStatus = row.contractStatus ? row.contractStatus.toString() : '';
	// 使用保存的原始ID填充，确保提交时 Purchaser/SalesContract/Salesperson 必填项有值
	Addcontractofpurchaseform.value.salesContract = row.salesContractId ?? row.salesContract ?? '';
	Addcontractofpurchaseform.value.customerContract = row.customerContract;
	Addcontractofpurchaseform.value.customerAbbreviation = row.customerAbbreviation;
	Addcontractofpurchaseform.value.deliveryDate = row.deliveryDate;
	Addcontractofpurchaseform.value.purchaseCurrency = row.purchaseCurrency;
	Addcontractofpurchaseform.value.deposit = row.deposit || '0';
	Addcontractofpurchaseform.value.deliveryLocation = row.deliveryLocation || '';
	Addcontractofpurchaseform.value.paymentDays = row.paymentDays ? state.optionss.hr_purchase_payment_days.find(item =>
		item.dictValue == row.paymentDays.toString())?.dictValue || '' : '';
	// 根据预付款金额判断是否有预付款
	Addcontractofpurchaseform.value.hasDeposit = parseFloat(row.deposit || '0') > 0;
	Addcontractofpurchaseform.value.salesperson = row.salespersonId ?? row.salesperson ?? '';
	Addcontractofpurchaseform.value.purchaser = row.purchaserId ?? row.purchaser ?? '';
	Addcontractofpurchaseform.value.priceTerms = row.priceTerms;
	Totalvalueofgoodsform.value.totalValue = row.totalGoodsValue;
	Totalvalueofgoodsform.value.totalQuantity = row.totalQuantity;
	Totalvalueofgoodsform.value.totalBoxCount = row.totalBoxes;
	Totalvalueofgoodsform.value.totalGrossWeight = row.totalGrossWeight;
	Totalvalueofgoodsform.value.totalNetWeight = row.totalNetWeight;
	Totalvalueofgoodsform.value.totalVolume = row.totalVolume;
	Totalvalueofgoodsform.value.appliedPayment = row.appliedPayment;
	Totalvalueofgoodsform.value.availablePayment = row.availablePayment;
	Totalvalueofgoodsform.value.paidAmount = row.paidAmount;
	Totalvalueofgoodsform.value.unpaidAmount = row.unpaidAmount;


	/*采购合同产品信息与相关费用*/
	return new Promise((resolve, reject) => {
		request({
			url: 'PurchaseContracts/GetPurchaseContractDetailsById/GetPurchaseContractDetails',
			method: 'GET',
			params: {
				PurchaseContracID: row.id
			}
		}).then(async response => {
			console.log('CheckDetails - 产品数据响应:', response.data.purchaseContractProducts);
			if (response.data.purchaseContractProducts.length > 0) {
				const products = response.data.purchaseContractProducts.map(element => {
					console.log('产品元素:', element);
					return {
						id: element.id,
						productId: element.productNumber, // 确保这里设置了productId
						productCode: element.productCode || '',
						customerCode: element.customerNumber || '', // 添加客户货号
						chineseName: element.chineseName,
						chineseSpecification: element.chineseSpecification,
						unit: element.unit ? state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无' : '无',
						contractQuantity: element.contractQuantity,
						purchasecurrency: (element.purchasecurrency ?? element.purchaseCurrency)?.toString() || '3',
						purchaseUnitPrice: element.purchasePrice,
						purchasePriceTerms: (element.purchasingPriceTerms ?? element.purchasePriceTerms ?? element.purchasepriceterms)?.toString() || '',
						purchaseTotalPrice: element.purchaseTotalPrice,
						deliveryDate: element.deliveryDate,
						productionLeadTime: element.productionLeadTime,
						packaging: element.packaging ? state.optionss.hr_packing.find(item => item.dictValue === element.packaging.toString())?.dictLabel || '无' : '无',
						specialRequirements: element.specialRequirements,
						innerBoxQuantity: element.innerBoxQuantity,
						outerBoxQuantity: element.outerBoxQuantity,
						invoice: element.invoice == 1 ? 1 : 0,
						hasDeposit: element.hasdeposit == 1 || element.hasdeposit === true,
						depositAmount: element.depositAmount || 0,
						remark: element.remark,
						supplier: element.supplierID ? state.optionss.sql_supplier_info.find(item => item.dictValue === element.supplierID.toString())?.dictValue || null : null
					};
				});
				console.log('映射后的产品数据:', products);
				// 设置产品数据
				productinfotableData.value = products;

				// 为每个产品预加载供应商信息
				for (const product of productinfotableData.value) {
					if (product.productId) {
						await loadProductSuppliers(product);
					}
				}
				// 自动计算合计信息
				updateTotalValues();
			}
			if (response.data.purchaseContractVendorExpenses.length > 0) {
				CustomerRelaterExoensesTableData.value = [];

				CustomerRelaterExoensesTableData.value = response.data.purchaseContractVendorExpenses;
				CustomerRelaterExoensesTableData.value.forEach(element => {
					element.currency = element.currency ? optionss.value.hr_export_currency.find(item => item.dictValue === element.currency.toString())?.dictValue : '';
					element.amount = element.expense * element.exchangeRate;
					// 加载供应商字段
					element.supplier = element.supplierID ? element.supplierID.toString() : '';
					// 费用名称现在是手动输入的文本，不需要从字典中查找
				});
			}
			Addcontractofpurchasedialog.value = true;
			// 异步获取审批流程并设置审核按钮状态
			getApprovalFlow(row.id).then(() => {
				const isCurrentUserApprover = checkIfCurrentUserIsApprover();
				// 当前用户是审批人且合同在审核中或申请完结时显示审核按钮
				if (isCurrentUserApprover && (row.reviewStatusStr === '审核中' || row.reviewStatusStr === '申请完结')) {
					showApproveRejectBtn.value = true;
					showApprovePassBtn.value = true;
					// 设置文档类型（采购合同）
					ApproveDocumentRequest.DocumentType = 2;
				}
			});
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});
}

//获取供应商编号
const GetNewPurchaseContractNumber = (contractId) => {
	return new Promise((resolve, reject) => {
		request({
			url: 'PurchaseContracts/GetNewPurchaseContractNumber/GetPurchaseContractNumber',
			method: 'GET',
			params: {
				contractId: contractId
			}
		}).then(response => {
			if (response && response.data) {  // 确保响应中包含数据
				Addcontractofpurchaseform.value.purchaseContract = response.data;
				resolve(response.data);
			} else {
				reject(new Error('获取采购合同号失败'));
			}
		}).catch(error => {
			console.error('获取采购合同号失败:', error);
			reject(error);
		});
	});
}

const clearAll = () => {
	Addcontractofpurchaseform.value.purchaseContract = '';
	Addcontractofpurchaseform.value.contractStatus = '';
	Addcontractofpurchaseform.value.salesContract = '';
	Addcontractofpurchaseform.value.customerContract = '';
	Addcontractofpurchaseform.value.customerAbbreviation = '';
	Addcontractofpurchaseform.value.deliveryDate = '';
	Addcontractofpurchaseform.value.purchaseCurrency = '';
	Addcontractofpurchaseform.value.deposit = '0';
	Addcontractofpurchaseform.value.hasDeposit = false;
	Addcontractofpurchaseform.value.salesperson = '';
	Addcontractofpurchaseform.value.purchaser = '';
	Addcontractofpurchaseform.value.paymentDays = '';
	Addcontractofpurchaseform.value.priceTerms = '';
	Addcontractofpurchaseform.value.deliveryLocation = '';
	Totalvalueofgoodsform.value.totalValue = '';
	Totalvalueofgoodsform.value.totalQuantity = '';
	Totalvalueofgoodsform.value.totalBoxCount = '';
	Totalvalueofgoodsform.value.totalGrossWeight = '';
	Totalvalueofgoodsform.value.totalNetWeight = '';
	Totalvalueofgoodsform.value.totalVolume = '';
	Totalvalueofgoodsform.value.appliedPayment = '';
	Totalvalueofgoodsform.value.availablePayment = '';
	Totalvalueofgoodsform.value.paidAmount = '';
	Totalvalueofgoodsform.value.unpaidAmount = '';
	productinfotableData.value = [];
	CustomerRelaterExoensesTableData.value = [];
	// 重置销售合同禁用状态
	isSalesContractDisabled.value = false;
}
// 存储审批步骤数据
const approvalSteps = ref([])

// 获取审批流程
const getApprovalFlow = async (documentId: number) => {
	try {
		const res = await request({
			url: 'PurchaseContracts/GetPurchaseContractApprovalFlowByPCID/GetApprovalFlow',
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
		case '申请完结': return 'info'
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

// 审核通过
const Approvepass = async () => {
	try {
		await ElMessageBox.confirm('确定通过该采购合同的审批吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 设置审批参数
		ApproveDocumentRequest.ApproveStatus = true;
		ApproveDocumentRequest.DocumentID = currentContractId.value;
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
				Addcontractofpurchasedialog.value = false;
				// 刷新合同列表
				GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
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
		await ElMessageBox.confirm('确定驳回该采购合同的审批吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 设置审批参数
		ApproveDocumentRequest.ApproveStatus = false;
		ApproveDocumentRequest.DocumentID = currentContractId.value;
		ApproveDocumentRequest.ApproverID = userId;

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
				Addcontractofpurchasedialog.value = false;
				// 刷新合同列表
				GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
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

// 删除采购合同
const DeletePurchaseContract = (row) => {
	ElMessageBox.confirm('确定要删除该采购合同吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'PurchaseContracts/DeletePurchaseContracts/Delete',
			method: 'post',
			data: { PurchaseContractID: row.id }
		}).then(response => {
			if (response.code === 200) {
				ElMessage.success(response.msg || '删除成功');
				GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
			} else {
				ElMessage.error(response.msg || '删除失败');
			}
		}).catch(() => {
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage.info('已取消删除');
	});
};

const handleAddNewProduct = async (row) => {
	viewDetailsDialog.value = false;

	try {
		// 调用发送添加新产品消息接口
		const response = await request({
			url: 'PurchaseContracts/SendAddProductMessage/SendAddProductMessage',
			method: 'GET'
		});

		if (response.code === 200) {
			ElMessage.success('已发送添加新产品申请消息');
		} else {
			ElMessage.warning('发送消息失败，但可以继续添加产品');
		}
	} catch (error) {
		console.error('发送添加新产品消息失败:', error);
		ElMessage.warning('发送消息失败，但可以继续添加产品');
	}
	// 跳转到产品信息页面，可以带参数
	//router.push({ path: '/product/productinfomation', query: { from: 'purchase', contractProductsId: row.id } })
}

// 判断合同状态是否为已批准状态（3、4、5、6、7）
const isContractStatusApproved = (row) => {
	// 获取合同状态的数值
	const contractStatusValue = getContractStatusValue(row);
	// 已批准的状态：3-生产中、4-已完成、5-部分交货、6-已交货、7-已完结
	return [3, 4, 5, 6, 7].includes(contractStatusValue);
}

// 获取合同状态的数值
const getContractStatusValue = (row) => {
	// 如果contractStatus是字符串，需要从字典中查找对应的数值
	if (typeof row.contractStatus === 'string') {
		const statusOption = state.optionss.hr_contract_status.find(item =>
			item.dictLabel === row.contractStatus
		);
		return statusOption ? parseInt(statusOption.dictValue) : 0;
	}
	// 如果已经是数值，直接返回
	return parseInt(row.contractStatus) || 0;
}

// 手动完结采购合同（申请完结）
const completePurchaseContractManually = (row) => {
	ElMessageBox.confirm('确定要申请完结该采购合同吗？提交后将进入完结审批流程。', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'PurchaseContracts/CompleteContractManually/CompleteContractManually',
			method: 'post',
			params: { purchaseContractId: row.id }
		}).then(response => {
			if (response.code === 200) {
				ElMessage.success(response.msg || '采购合同已成功提交申请完结审批流程！');
				const start = purchasecontractsTableDatacurrentPage.value;
				const end = purchasecontractsTableDatapageSize.value;
				GetpurchaseContractList(start, end);
			} else {
				ElMessage.error(response.msg || '申请完结失败');
			}
		}).catch(() => {
			ElMessage.error('申请完结失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage.info('已取消操作');
	});
};
</script>

<style scoped>
/* 创建合同和查看合同详情dialog中的表单组件间距减少一半 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

.el-dialog .el-row {
	margin-bottom: 2.5px !important;
}
</style>
