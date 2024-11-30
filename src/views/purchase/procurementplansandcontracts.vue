<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenAddcontractofpurchasedialog">新增采购合同</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;采购需求</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="shoppinglisttableData" style="width: 100%" :span-method="mergeCells">
			<el-table-column prop="contractNumber" label="销售合同号" width="150"></el-table-column>
			<el-table-column prop="contractId" label="销售合同ID" width="150" v-if="false"></el-table-column>
			<el-table-column prop="productId" label="产品ID" width="150" v-if="false"></el-table-column>
			<el-table-column prop="productName" label="产品名称" width="150"></el-table-column>
			<el-table-column prop="quantity" label="数量" width="150"></el-table-column>
			<el-table-column prop="unitPrice" label="采购单价" width="150"></el-table-column>
			<el-table-column prop="totalPrice" label="采购总价" width="150"></el-table-column>
			<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
			<!-- 操作 -->
			<el-table-column label="操作" width="150">
				<template #default="scope">
					<!-- 合并的“生成采购合同”按钮 -->
					<div v-if="isFirstRow(scope.row)">
						<el-button type="primary" size="small" @click="GenerateContractPurchase(scope.row)">
							生成采购合同
						</el-button>
					</div>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="contractsTableDatahandlePageChange"
			:current-page="contractsTableDatacurrentPage" :page-size="contractsTableDatapageSize"
			:total="contractsTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;采购合同</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="contractofpurchasetableData" style="width: 100%">
			<el-table-column prop="purchaseContractNumber" label="采购合同号" width="150"></el-table-column>
			<el-table-column prop="contractStatus" label="合同状态" width="150"></el-table-column>
			<el-table-column prop="vendorCode" label="厂商编号" width="150"></el-table-column>
			<el-table-column prop="vendorAbbreviation" label="厂家简称" width="150"></el-table-column>
			<el-table-column prop="salesContract" label="销售合同" width="150"></el-table-column>
			<el-table-column prop="customerContract" label="客户订单号" width="150"></el-table-column>
			<el-table-column prop="deliveryDate" label="交货日期" width="150"></el-table-column>
			<el-table-column prop="purchaseCurrency" label="采购货币" width="150"></el-table-column>
			<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
			<el-table-column prop="purchaser" label="采购员" width="150"></el-table-column>
			<el-table-column prop="createTime" label="创建日期" width="150"></el-table-column>
			<el-table-column fixed="right" label="操作" width="100">
				<template #default="scope">
					<el-button type="text" size="small" @click="CheckDetails(scope.row)">查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="purchasecontractsTableDatahandlePageChange"
			:current-page="purchasecontractsTableDatacurrentPage" :page-size="purchasecontractsTableDatapageSize"
			:total="purchasecontractsTableDatatotalItems" background layout="prev, pager, next"
			style="margin-top: 5px;" />

		<el-dialog v-model="Addcontractofpurchasedialog" title="新增采购合同" :close-on-click-modal=false style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="Addcontractofpurchaseform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="采购合同">
							<el-input v-model="Addcontractofpurchaseform.purchaseContract" disabled
								placeholder="请输入采购合同" style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="合同状态">
							<el-select v-model="Addcontractofpurchaseform.contractStatus" placeholder="请选择合同状态"
								style="width: 300px" disabled>
								<el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="交货日期">
							<el-date-picker v-model="Addcontractofpurchaseform.deliveryDate" type="date" disabled
								style="width: 300px"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="厂商简称">
							<el-select v-model="Addcontractofpurchaseform.vendorCode" placeholder="请选择供应商" filterable
								style="width: 300px" @change="GetSupplierInfo">
								<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="采购币种">
							<el-select v-model="Addcontractofpurchaseform.purchaseCurrency" disabled
								style="width: 300px">
								<el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付定金">
							<el-input v-model="Addcontractofpurchaseform.deposit" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="销售员">
							<el-select v-model="Addcontractofpurchaseform.salesperson" disabled style="width: 300px">
								<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="采购员">
							<el-select disabled v-model="Addcontractofpurchaseform.purchaser" placeholder="请选择采购员"
								style="width: 300px">
								<el-option v-for="dict in optionss.sql_hr_purchase" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="价格条款">
							<el-select v-model="Addcontractofpurchaseform.priceTerms" disabled style="width: 300px">
								<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="付款天数">
							<el-input v-model="Addcontractofpurchaseform.paymentDays" placeholder="请输入付款天数"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="false">
					<el-col :span="8">
						<el-form-item label="客户简称">
							<el-input v-model="Addcontractofpurchaseform.customerAbbreviation" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商编号">
							<el-select v-model="Addcontractofpurchaseform.vendorCode" placeholder="请选择厂商编号"
								style="width: 300px" @change="GetSupplierInfo">
								<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="销售合同">
							<el-select v-model="Addcontractofpurchaseform.salesContract" placeholder="请选择销售合同"
								style="width: 300px">
								<el-option v-for="dict in optionss.sql_sale_contracts" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户合同">
							<el-input v-model="Addcontractofpurchaseform.customerContract" disabled
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<el-tabs v-model="activeName" class="demo-tabs" @tab-click="handleClick">
				<el-tab-pane label="产品资料" name="productinfo">
					<el-table :data="productinfotableData">
						<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
						<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
						<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
						<el-table-column prop="englishName" label="英文品名" width="150"></el-table-column>
						<el-table-column prop="chineseSpec" label="中文规格" width="150"></el-table-column>
						<el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
						<!-- 其他列保持不变 -->
						<el-table-column prop="contractQuantity" label="合同数量" width="150">
							<template #default="scope">
								<el-input v-model="scope.row.contractQuantity" type="number" size="small"
									@change="handleQuantityChange(scope.row)" placeholder="请输入数量">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseUnitPrice" label="采购单价" width="150">
							<template #default="scope">
								<el-input v-model="scope.row.purchaseUnitPrice" type="number" size="small"
									@change="handlePriceChange(scope.row)" placeholder="请输入单价">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseTotalPrice" label="采购总价" width="150">
							<template #default="scope">
								<span>{{ scope.row.purchaseTotalPrice }}</span>
							</template>
						</el-table-column>
						<!-- 其他列保持不变 -->
						<el-table-column prop="deliveryDate" label="交货日期" width="150"></el-table-column>
						<el-table-column prop="productionLeadTime" label="生产交期" width="150"></el-table-column>
						<el-table-column prop="packaging" label="包装方式" width="150"></el-table-column>
						<el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
						<el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
						<el-table-column prop="innerBoxQuantity" label="内盒装量" width="150"></el-table-column>
						<el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
						<el-table-column prop="remark" label="备注" width="150"></el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="厂家相关费用" name="relatedcosts">
					<el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; " height="280">
						<el-table-column prop=" expenseName" label="费用名称" width="150">
							<template #default="{ row }">
								<el-input v-model="row.expenseName" placeholder="输入费用名称" size="small"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="currency" label="币种" width="150">
							<template #default="{ row }">
								<el-select v-model="row.currency" placeholder="选择币种" size="small"
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
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="small" @click="handleDelete(scope.$index)">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
			</el-tabs>
			<div style="margin-top: 30px;"></div>
			<span style="font-size: 20px; font-weight: bold; ">合计信息</span>
			<el-divider></el-divider>
			<el-form :model="Totalvalueofgoodsform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="货值合计">
							<el-input v-model="Totalvalueofgoodsform.totalValue" placeholder="请输入货值合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="数量合计">
							<el-input v-model="Totalvalueofgoodsform.totalQuantity" placeholder="请输入数量合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="箱数合计">
							<el-input v-model="Totalvalueofgoodsform.totalBoxCount" placeholder="请输入箱数合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="毛重合计">
							<el-input v-model="Totalvalueofgoodsform.totalGrossWeight" placeholder="请输入毛重合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="净重合计">
							<el-input v-model="Totalvalueofgoodsform.totalNetWeight" placeholder="请输入净重合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="体积合计">
							<el-input v-model="Totalvalueofgoodsform.totalVolume" placeholder="请输入体积合计"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="已申请付款">
							<el-input v-model="Totalvalueofgoodsform.appliedPayment" placeholder="请输入已申请付款"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="可申请付款">
							<el-input v-model="Totalvalueofgoodsform.availablePayment" placeholder="请输入可申请付款"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付货款">
							<el-input v-model="Totalvalueofgoodsform.paidAmount" placeholder="请输入已付货款"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="未付货款">
							<el-input v-model="Totalvalueofgoodsform.unpaidAmount" placeholder="请输入未付货款"
								style="width: 300px"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" v-show="isSaveBtnShow" @click="SavePurchaseContract">
						确定保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { get } from 'sortablejs';
import { el } from 'element-plus/es/locale';
import useUserStore from "@/store/modules/user";


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

	// 更新合计表单
	Totalvalueofgoodsform.value.totalValue = totalValue.toFixed(2);
	Totalvalueofgoodsform.value.totalQuantity = totalQuantity.toString();
}

var userId = useUserStore().userId;

const isSaveBtnShow = ref(true);

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
		sql_hr_purchase: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_supplier_level' }, { dictType: 'hr_business_scope' }, { dictType: 'hr_china_provinces' }, { dictType: 'hr_china_city' }, { dictType: 'sql_sale_contracts' },
{ dictType: 'sql_supplier_info' }, { dictType: 'sql_hr_purchase' }]

// proxy.getDicts(dictParams).then((response) => {
// 	response.data.forEach((element) => {
// 		state.optionss[element.dictType] = element.list
// 	})
// })

async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		await ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);  // 现在可以安全执行
		await GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
fetchDataAndExecute();
/*动态下拉框end*/

const activeName = ref('productinfo')
const CustomerRelaterExoensesTableData = ref([]);

const Addcontractofpurchasedialog = ref(false)
const Addcontractofpurchaseform = ref({
	purchaseContract: '',
	contractStatus: '',
	vendorCode: '',
	vendorAbbreviation: '',
	salesContract: '',
	customerContract: '',
	customerAbbreviation: '',
	deliveryDate: '',
	purchaseCurrency: '',
	deposit: '',
	salesperson: '',
	purchaser: '',
	paymentDays: '',
	priceTerms: ''
})

const GetSupplierInfo = () => {
	return new Promise((resolve, reject) => {
		request({
			url: 'Supplierinfo/GetSupplierInfoByID/GetSupplierInfo',
			method: 'GET',
			params: {
				SupplierID: Addcontractofpurchaseform.value.vendorCode
			}
		}).then(response => {
			if (response.code == 200) {
				// 供应商简称
				Addcontractofpurchaseform.value.vendorAbbreviation = response.data.shortName;
				resolve(response);  // Resolve the promise with the response data
			} else {
				ElMessage({
					message: '未找到对应的厂商信息',
					type: 'error'
				});	// 提示错误信息
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const GetSaleContactInfo = () => {
	return new Promise((resolve, reject) => {
		request({
			url: 'Contracts/GetContractDetailsById/GetContractDetails',
			method: 'GET',
			params: {
				contractId: Addcontractofpurchaseform.value.salesContract
			}
		}).then(response => {
			if (response.code == 200) {
				Addcontractofpurchaseform.value.customerContract = response.data.contract.customerContract;
				Addcontractofpurchaseform.value.customerAbbreviation = response.data.contract.customerAbbreviation;
				Addcontractofpurchaseform.value.deliveryDate = response.data.contract.deliveryDate;
				Addcontractofpurchaseform.value.purchaseCurrency = response.data.contract.foreignCurrency.toString();
				Addcontractofpurchaseform.value.salesperson = response.data.contract.salesperson.toString();
				Addcontractofpurchaseform.value.priceTerms = response.data.contract.priceTerms.toString();
				productinfotableData.value = response.data.contractProducts;//产品资料
				CustomerRelaterExoensesTableData.value = response.data.contractExpenses;//厂家相关费用
				//合计信息
				Totalvalueofgoodsform.value.totalValue = response.data.contract.goodsValue;
				Totalvalueofgoodsform.value.totalQuantity = response.data.contract.quantity;
				Totalvalueofgoodsform.value.totalBoxCount = response.data.contract.boxCount;
				Totalvalueofgoodsform.value.totalGrossWeight = response.data.contract.grossWeight;
				Totalvalueofgoodsform.value.totalNetWeight = response.data.contract.netWeight;
				Totalvalueofgoodsform.value.totalVolume = response.data.contract.volume;
				resolve(response);  // Resolve the promise with the response data

			} else {
				ElMessage({
					message: '未找到对应的销售合同信息',
					type: 'error'
				});	// 提示错误信息
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
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

const handleDelete = (index) => {
	CustomerRelaterExoensesTableData.value.splice(index, 1);
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

//验证采购合同明细是否有变动信息
const VerifyPurchaseContractDetails = (contractId, productinfotableData) => {
	return new Promise((resolve, reject) => {
		request({
			url: 'Contracts/GetContractDetailsById/GetContractDetails',
			method: 'GET',
			params: {
				contractId: contractId
			}
		}).then(response => {
			if (response.code === 200) {
				const contractProducts = response.data.contractProducts;
				// 找出单价不一致的产品
				const discrepancies = productinfotableData.filter(product => {
					const matchingContractProduct = contractProducts.find(
						contractProduct => contractProduct.productCode === product.productCode
					);
					return (
						matchingContractProduct &&
						matchingContractProduct.purchaseUnitPrice !== product.purchaseUnitPrice
					);
				});
				if (discrepancies.length > 0) {
					// 存在不一致条目，返回不一致的数据
					resolve({
						hasDiscrepancies: true,
						discrepancies
					});
				} else {
					// 所有单价一致
					resolve({
						hasDiscrepancies: false,
						discrepancies: []
					});
				}
			} else {
				ElMessage({
					message: '校验销售合同出错,未找到对应的销售合同信息',
					type: 'error'
				});
				reject(new Error('未找到对应的销售合同信息'));
			}
		})
			.catch(error => {
				console.error(error);
				ElMessage({
					message: '校验销售合同出错，请稍后重试',
					type: 'error'
				});
				reject(error);
			});
	});
};

const SavePurchaseContract = () => {
	// 验证采购合同明细是否有变动信息
	VerifyPurchaseContractDetails(Addcontractofpurchaseform.value.salesContract, productinfotableData.value)
		.then(result => {
			const message = result.hasDiscrepancies
				? "部分产品的采购单价已发生变动，是否提交变动并通知销售人员进行确认？"
				: "确认保存采购合同信息吗？";

			ElMessageBox.confirm(message, "提示", {
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				type: "warning",
			}).then(() => {
				if (result.hasDiscrepancies) {
					// 如果有价格变动，调用更新价格接口
					const editRequest = {
						contractId: parseInt(Addcontractofpurchaseform.value.salesContract),
						contractProductItems: productinfotableData.value.map(item => ({
							id: item.id,
							productId: item.productId,
							purchaseUnitPrice: parseFloat(item.purchaseUnitPrice),
							purchaseTotalPrice: parseFloat(item.purchaseTotalPrice),
							isPriceChanged: 1
						}))
					};
					// 先更新价格
					request.post("contracts/ContractPurchasePriceChanges/UpdatePrice", editRequest)
						.then(response => {
							if (response.code === 200) {
								// 价格更新成功后，继续保存采购合同
								submitPurchaseContract();
							} else {
								ElMessage.error('更新采购单价失败');
							}
						})
						.catch(error => {
							console.error('更新采购单价失败:', error);
							ElMessage.error('更新采购单价失败，请重试');
						});
				} else {
					// 如果没有价格变动，直接保存采购合同
					submitPurchaseContract();
				}
			}).catch(() => {
				ElMessage({
					type: "info",
					message: "已取消保存采购合同",
				});
			});
		})
		.catch(error => {
			console.error('验证采购合同失败:', error);
			ElMessage.error('验证采购合同失败，请重试');
		});
};

// 提交采购合同的具体逻辑
const submitPurchaseContract = () => {
	// 构建采购合同请求数据
	contractofpurchaseRequest.PurchaseContractNumber = Addcontractofpurchaseform.value.purchaseContract;
	contractofpurchaseRequest.ContractStatus = Addcontractofpurchaseform.value.contractStatus;
	contractofpurchaseRequest.VendorCode = Addcontractofpurchaseform.value.vendorCode;
	contractofpurchaseRequest.VendorAbbreviation = Addcontractofpurchaseform.value.vendorAbbreviation;
	contractofpurchaseRequest.SalesContract = Addcontractofpurchaseform.value.salesContract;
	contractofpurchaseRequest.CustomerContract = Addcontractofpurchaseform.value.customerContract;
	contractofpurchaseRequest.CustomerAbbreviation = Addcontractofpurchaseform.value.customerAbbreviation;
	contractofpurchaseRequest.DeliveryDate = Addcontractofpurchaseform.value.deliveryDate;
	contractofpurchaseRequest.PurchaseCurrency = Addcontractofpurchaseform.value.purchaseCurrency;
	contractofpurchaseRequest.Deposit = Addcontractofpurchaseform.value.deposit;
	contractofpurchaseRequest.Salesperson = Addcontractofpurchaseform.value.salesperson;
	contractofpurchaseRequest.Purchaser = Addcontractofpurchaseform.value.purchaser;
	contractofpurchaseRequest.PaymentDays = Addcontractofpurchaseform.value.paymentDays;
	contractofpurchaseRequest.PriceTerms = Addcontractofpurchaseform.value.priceTerms;
	contractofpurchaseRequest.TotalGoodsValue = Totalvalueofgoodsform.value.totalValue;
	contractofpurchaseRequest.TotalQuantity = Totalvalueofgoodsform.value.totalQuantity;
	contractofpurchaseRequest.TotalBoxes = Totalvalueofgoodsform.value.totalBoxCount;
	contractofpurchaseRequest.TotalGrossWeight = Totalvalueofgoodsform.value.totalGrossWeight;
	contractofpurchaseRequest.TotalNetWeight = Totalvalueofgoodsform.value.totalNetWeight;
	contractofpurchaseRequest.TotalVolume = Totalvalueofgoodsform.value.totalVolume;
	contractofpurchaseRequest.AppliedPayment = Totalvalueofgoodsform.value.appliedPayment;
	contractofpurchaseRequest.AvailablePayment = Totalvalueofgoodsform.value.availablePayment;
	contractofpurchaseRequest.PaidAmount = Totalvalueofgoodsform.value.paidAmount;
	contractofpurchaseRequest.UnpaidAmount = Totalvalueofgoodsform.value.unpaidAmount;
	contractofpurchaseRequest.PurchaseContractProducts = productinfotableData.value;
	contractofpurchaseRequest.PurchaseContractVendorExpenses = CustomerRelaterExoensesTableData.value;
	// 提交采购合同
	request.post("PurchaseContracts/AddPurchaseContracts/Add", contractofpurchaseRequest)
		.then(response => {
			if (response.code === 200) {
				ElMessage({
					message: "采购合同添加成功！",
					type: "success",
				});
				Addcontractofpurchasedialog.value = false;
				GetpurchaseContractList(purchasecontractsTableDatacurrentPage.value, purchasecontractsTableDatapageSize.value);
			} else {
				ElMessage.error('添加采购合同失败');
			}
		})
		.catch(error => {
			console.error("添加采购合同失败:", error);
			ElMessage.error('添加采购合同失败，请重试');
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


const handleClick = (tab: TabsPaneContext, event: Event) => {
	console.log(tab, event)
}

const productinfotableData = ref([])

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
				salesperson: optionss.value.sql_hr_sale.find(item => item.dictValue == element.salesperson.toString()).dictLabel,
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
watch(() => Addcontractofpurchaseform.value.salesContract, (newValue, oldValue) => {
	// 这里的代码会在 salesContract 的值改变时执行
	if (Addcontractofpurchaseform.value.salesContract != '') {
		GetSaleContactInfo();
	}
});
const GenerateContractPurchase = (row) => {
	ElMessageBox.confirm('确定生成采购合同吗?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		//alert(JSON.stringify(row));
		Addcontractofpurchaseform.value.salesperson = row.salesperson;
		Addcontractofpurchaseform.value.salesContract = row.contractId.toString();
		Addcontractofpurchaseform.value.purchaser = userId.toString();
		// 设置默认合同状态为1
		Addcontractofpurchaseform.value.contractStatus = '1';
		Addcontractofpurchasedialog.value = true;
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消生成采购合同'
		});
	});
}

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
				contractofpurchasetableData.value.forEach(element => {
					element.contractStatus = optionss.value.hr_contract_status.find(item => item.dictValue == element.contractStatus.toString()).dictLabel;
					element.vendorCode = optionss.value.sql_supplier_info.find(item => item.dictValue == element.vendorCode).dictLabel;
					element.salesContract = optionss.value.sql_sale_contracts.find(item => item.dictValue == element.salesContract).dictLabel;
					element.purchaseCurrency = optionss.value.hr_export_currency.find(item => item.dictValue == element.purchaseCurrency).dictLabel;
					element.salesperson = optionss.value.sql_hr_sale.find(item => item.dictValue == element.salesperson).dictLabel;
					element.purchaser = optionss.value.sql_hr_purchase.find(item => item.dictValue == element.purchaser).dictLabel;
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

const CheckDetails = (row) => {
	isSaveBtnShow.value = false;
	Addcontractofpurchaseform.value.purchaseContract = row.purchaseContractNumber;
	Addcontractofpurchaseform.value.contractStatus = row.contractStatus.toString();
	Addcontractofpurchaseform.value.vendorCode = row.vendorCode;
	Addcontractofpurchaseform.value.vendorAbbreviation = row.vendorAbbreviation;
	Addcontractofpurchaseform.value.salesContract = row.salesContract;
	Addcontractofpurchaseform.value.customerContract = row.customerContract;
	Addcontractofpurchaseform.value.customerAbbreviation = row.customerAbbreviation;
	Addcontractofpurchaseform.value.deliveryDate = row.deliveryDate;
	Addcontractofpurchaseform.value.purchaseCurrency = row.purchaseCurrency;
	Addcontractofpurchaseform.value.deposit = row.deposit;
	Addcontractofpurchaseform.value.salesperson = row.salesperson;
	Addcontractofpurchaseform.value.purchaser = row.purchaser;
	Addcontractofpurchaseform.value.paymentDays = row.paymentDays;
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
		}).then(response => {
			if (response.data.purchaseContractProducts.length > 0) {
				productinfotableData.value = [];
				response.data.purchaseContractProducts.forEach(element => {
					productinfotableData.value.push(element);
				});
			}
			if (response.data.purchaseContractVendorExpenses.length > 0) {
				CustomerRelaterExoensesTableData.value = [];
				CustomerRelaterExoensesTableData.value = response.data.purchaseContractVendorExpenses;
			}
			Addcontractofpurchasedialog.value = true;
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});
}

//获取供应商编号
const GetNewPurchaseContractNumber = () => {
	request({
		url: 'PurchaseContracts/GetNewPurchaseContractNumber/GetPurchaseContractNumber',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			Addcontractofpurchaseform.value.purchaseContract = response;
		} else {
			console.error('获取下一个供应商编号出错');
		}
	}).catch(error => {
		console.error('获取下一个供应商编号出错！😔错误内容：', error);
	})
}

const OpenAddcontractofpurchasedialog = () => {
	clearAll();
	GetNewPurchaseContractNumber();
	Addcontractofpurchaseform.value.purchaser = userId.toString();
	Addcontractofpurchaseform.value.contractStatus = '1';
	Addcontractofpurchasedialog.value = true;
	isSaveBtnShow.value = true;
}

const clearAll = () => {
	Addcontractofpurchaseform.value.purchaseContract = '';
	Addcontractofpurchaseform.value.contractStatus = '';
	Addcontractofpurchaseform.value.vendorCode = '';
	Addcontractofpurchaseform.value.vendorAbbreviation = '';
	Addcontractofpurchaseform.value.salesContract = '';
	Addcontractofpurchaseform.value.customerContract = '';
	Addcontractofpurchaseform.value.customerAbbreviation = '';
	Addcontractofpurchaseform.value.deliveryDate = '';
	Addcontractofpurchaseform.value.purchaseCurrency = '';
	Addcontractofpurchaseform.value.deposit = '';
	Addcontractofpurchaseform.value.salesperson = '';
	Addcontractofpurchaseform.value.purchaser = '';
	Addcontractofpurchaseform.value.paymentDays = '';
	Addcontractofpurchaseform.value.priceTerms = '';
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
}

</script>
