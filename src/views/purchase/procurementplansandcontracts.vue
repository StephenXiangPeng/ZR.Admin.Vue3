<template>
	<div>
		<!-- <div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenAddcontractofpurchasedialog">新增采购合同</el-button> -->
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;采购需求</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="shoppinglisttableData" style="width: 100%" :span-method="mergeCells">
			<el-table-column prop="procurementId" label="ID" width="150" v-if="false"></el-table-column>
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
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;采购合同</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="contractofpurchasetableData" style="width: 100%">
			<el-table-column prop="id" label="ID" width="150" v-if="false"></el-table-column>
			<el-table-column prop="purchaseContractNumber" label="采购合同号" width="180">
				<template #default="scope">
					<span>{{ scope.row.purchaseContractNumber }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="contractStatus" label="合同状态" width="100"></el-table-column>
			<el-table-column prop="reviewStatus" label="审核状态编号" width="150" v-if="false"></el-table-column>
			<el-table-column prop="reviewStatusStr" label="审核状态" width="150" align="center">
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
			<el-table-column prop="salesContract" label="销售合同" width="150"></el-table-column>
			<el-table-column prop="customerContract" label="客户订单号" width="150"></el-table-column>
			<el-table-column prop="deliveryDate" label="交货日期" width="150"></el-table-column>
			<el-table-column prop="purchaseCurrency" label="采购货币" width="150"></el-table-column>
			<el-table-column prop="salesperson" label="销售员" width="150"></el-table-column>
			<el-table-column prop="purchaser" label="采购员" width="150"></el-table-column>
			<el-table-column prop="createTime" label="创建日期" width="150"></el-table-column>
			<el-table-column fixed="right" label="操作" width="280">
				<template #default="scope">
					<el-button type="text" size="small" @click="CheckDetails(scope.row)">查看详情</el-button>
					<el-button type="text" size="small"
						@click="GeneratePurchaseContract(scope.row)">生成采购合同PDF</el-button>
					<el-button v-if="scope.row.createBy === useUserStore().userId.toString() && scope.row.isDraft" link
						type="danger" size="small" @click="DeletePurchaseContract(scope.row)">删除</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="purchasecontractsTableDatahandlePageChange"
			:current-page="purchasecontractsTableDatacurrentPage" :page-size="purchasecontractsTableDatapageSize"
			:total="purchasecontractsTableDatatotalItems" background layout="prev, pager, next"
			style="margin-top: 5px;" />

		<el-dialog v-model="Addcontractofpurchasedialog" title="新增采购合同" :close-on-click-modal=false style="width: 70%;"
			@close="handleAddcontractofpurchasedialogclose">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="Addcontractofpurchaseform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="采购合同">
							<el-input ref="AddcontractofpurchaseformRef" :rules="rules"
								v-model="Addcontractofpurchaseform.purchaseContract" disabled placeholder="请输入采购合同"
								style="width: 300px"></el-input>
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
					<el-col :span="8">
						<el-form-item label="销售员">
							<el-select v-model="Addcontractofpurchaseform.salesperson" disabled style="width: 300px">
								<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
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
					<el-col :span="8">
						<el-form-item label="付款天数" prop="paymentDays">
							<el-input v-model="Addcontractofpurchaseform.paymentDays" placeholder="请输入付款天数"
								style="width: 300px" :disabled="isFormDisabled">
							</el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="false">
					<el-col :span="8">
						<el-form-item label="厂商简称" prop="vendorCode">
							<el-select v-model="Addcontractofpurchaseform.vendorCode" placeholder="请选择供应商" filterable
								style="width: 300px" @change="GetSupplierInfo">
								<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue">
								</el-option>
							</el-select>
						</el-form-item>
					</el-col>
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
						<el-table-column prop="vendorCode" label="供应商" width="200">
							<template #default="scope">
								<el-select :disabled="isFormDisabled" v-model="scope.row.vendorCode"
									placeholder="请选择供应商" filterable>
									<el-option v-for="dict in (supplierCache[scope.row.productId] || [])"
										:key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue">
									</el-option>
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
						<el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
						<el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
						<el-table-column prop="englishName" label="英文品名" width="150"></el-table-column>
						<el-table-column prop="chineseSpecification" label="中文规格" width="150"></el-table-column>
						<el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
						<!-- 其他列保持不变 -->
						<el-table-column prop="contractQuantity" label="合同数量" width="150">
							<template #default="scope">
								<el-input v-model="scope.row.contractQuantity" type="number"
									@change="handleQuantityChange(scope.row)" placeholder="请输入数量"
									:disabled="isFormDisabled">
								</el-input>
							</template>
						</el-table-column>
						<el-table-column prop="purchaseUnitPrice" label="采购单价" width="150">
							<template #default="scope">
								<el-input :disabled="isFormDisabled" v-model="scope.row.purchaseUnitPrice" type="number"
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
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="数量合计">
							<el-input v-model="Totalvalueofgoodsform.totalQuantity" placeholder="请输入数量合计"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="箱数合计">
							<el-input v-model="Totalvalueofgoodsform.totalBoxCount" placeholder="请输入箱数合计"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="毛重合计">
							<el-input v-model="Totalvalueofgoodsform.totalGrossWeight" placeholder="请输入毛重合计"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="净重合计">
							<el-input v-model="Totalvalueofgoodsform.totalNetWeight" placeholder="请输入净重合计"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="体积合计">
							<el-input v-model="Totalvalueofgoodsform.totalVolume" placeholder="请输入体积合计"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="已申请付款">
							<el-input v-model="Totalvalueofgoodsform.appliedPayment" placeholder="请输入已申请付款"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="可申请付款">
							<el-input v-model="Totalvalueofgoodsform.availablePayment" placeholder="请输入可申请付款"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付货款">
							<el-input v-model="Totalvalueofgoodsform.paidAmount" placeholder="请输入已付货款"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="未付货款">
							<el-input v-model="Totalvalueofgoodsform.unpaidAmount" placeholder="请输入未付货款"
								style="width: 300px" :disabled="isFormDisabled"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
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
		<el-dialog v-model="viewDetailsDialog" title="采购需求详情" :close-on-click-modal=false style="width: 70%;">
			<el-table :data="detailsTableData">
				<el-table-column prop="SaleContractID" label="采购合同ID" width="150" v-if="false"></el-table-column>
				<el-table-column prop="id" label="id" width="150" v-if="false"></el-table-column>
				<el-table-column prop="productId" label="产品ID" width="150" v-if="false"></el-table-column>
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
					<el-button type="primary" v-if="!hasPriceChanges"
						@click="GeneratePurchaseContract(currentDetailRow)">
						生成采购合同
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
import { FormInstance } from 'element-plus'
import { invoke } from '@vueuse/core';
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer } from 'element-plus'
import { useRoute } from 'vue-router'

const route = useRoute()
// 添加onMounted钩子
onMounted(() => {
	console.log('采购合同页面挂载，检查路由参数')
	autoLoadpurchaseContractDetail()
})

// 添加自动加载合同详情的函数
const autoLoadpurchaseContractDetail = () => {
	// 检查URL参数
	const purchaseContractId = route.query.purchaseContractId
	const viewDetail = route.query.viewDetail
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


// 产品供应商选项
const ProductSupplierOptions = ref([]);

// 添加供应商缓存
const supplierCache = reactive({});

// 根据产品ID获取供应商列表
const getSupplierListByProduct = async (productID: number) => {
	if (!productID) {
		return [];
	}

	// 如果缓存中有数据，直接返回
	if (supplierCache[productID]) {
		return supplierCache[productID];
	}

	try {
		const res = await request({
			url: 'ProductInformation/GetSupplierListByProductID/GetProductSupplierList',
			method: 'get',
			params: {
				productID: productID
			}
		});
		if (res.code == 200) {
			// 将数据存入缓存
			supplierCache[productID] = res.data;
			return res.data;
		} else {
			ElMessage.error(res.msg || '获取供应商列表失败');
			return [];
		}
	} catch (error) {
		console.error('获取供应商列表失败', error);
		ElMessage.error('获取供应商列表失败');
		return [];
	}
};

// 在表格数据加载时预加载供应商数据
const loadSupplierData = async (products) => {
	if (!products || !Array.isArray(products)) {
		return;
	}

	for (const product of products) {
		if (product.productId && !supplierCache[product.productId]) {
			await getSupplierListByProduct(product.productId);
		}
	}
};

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
			await GetNewPurchaseContractNumber();

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
				Addcontractofpurchaseform.value.purchaser = state.optionss.sql_hr_purchase.find(item => item.dictValue === userId.toString())?.dictValue;
				Addcontractofpurchaseform.value.contractStatus = "1";
				Addcontractofpurchaseform.value.salesContract = row.contractId;
				Addcontractofpurchaseform.value.customerContract = response.data.contract.customerContract;
				Addcontractofpurchaseform.value.customerid = response.data.contract.customerId;
				Addcontractofpurchaseform.value.customerAbbreviation = response.data.contract.customerAbbreviation;
				Addcontractofpurchaseform.value.deliveryDate = response.data.contract.deliveryDate;
				Addcontractofpurchaseform.value.purchaseCurrency = response.data.contract.foreignCurrency.toString();
				Addcontractofpurchaseform.value.deposit = "0";
				Addcontractofpurchaseform.value.salesperson = response.data.contract.salesperson.toString();
				Addcontractofpurchaseform.value.priceTerms = response.data.contract.priceTerms.toString();

				// 绑定商品信息
				if (response.data.contractProducts && response.data.contractProducts.length > 0) {
					productinfotableData.value = response.data.contractProducts.map(product => ({
						id: product.id,
						productId: product.productID, // 确保这里设置了productId
						vendorCode: product.vendorCode,
						productCode: product.productCode,
						chineseName: product.chineseName,
						chineseSpecification: product.chineseSpecification,
						unit: state.optionss.hr_calculate_unit.find(item => item.dictValue === product.unit.toString())?.dictLabel || '无',
						contractQuantity: product.contractQuantity,
						purchaseUnitPrice: product.purchaseUnitPrice,
						purchaseTotalPrice: product.purchaseTotalPrice,
						deliveryDate: product.deliveryDate,
						productionLeadTime: product.productionLeadTime,
						packaging: state.optionss.hr_packing.find(item => item.dictValue === product.packaging.toString())?.dictLabel || '无',
						specialRequirements: product.specialRequirements,
						innerBoxQuantity: product.innerBoxQuantity,
						outerBoxQuantity: product.outerBoxQuantity,
						invoice: product.invoice == 1 ? '是' : '否',
						remark: product.remark
					}));
					// 先加载供应商数据
					await loadSupplierData(productinfotableData.value);
				}

				// 绑定合计信息
				Totalvalueofgoodsform.value.totalValue = response.data.contract.goodsValue;
				Totalvalueofgoodsform.value.totalQuantity = response.data.contract.quantity;
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

const OpenAddcontractofpurchasedialog = async () => {
	isGeneratedFromRequirement.value = false; // 重置标志
	clearAll();
	await GetNewPurchaseContractNumber();
	Addcontractofpurchaseform.value.purchaser = userId.toString();
	Addcontractofpurchaseform.value.contractStatus = '1';
	Addcontractofpurchasedialog.value = true;
	isSaveBtnShow.value = true;
	showApproveRejectBtn.value = false;
	showApprovePassBtn.value = false;
	// 新增：如果有产品数据，加载供应商数据
	if (productinfotableData.value.length > 0) {
		await loadSupplierData(productinfotableData.value);
	}
}

// 添加新的响应式变量
const viewDetailsDialog = ref(false)
const detailsTableData = ref([])
const hasPriceChanges = ref(false)
const currentDetailRow = ref(null)
const ProcurementRequirementID = ref(0);
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
				if (response.code === 200) {
					ElMessage.success('已通知销售价格变动！')
					viewDetailsDialog.value = false
					//更新生成采购合同状态
					updateGenerateStatusByContractId(editRequest.contractId);
					// 刷新列表
					ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value)
				} else {
					ElMessage.error('通知失败，请重试')
				}
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
				console.error(error);
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
};

// 定义表单验证规则
const rules = ref({
	vendorCode: [
		{ required: true, message: '请选择厂商简称', trigger: 'change' }
	],
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

	// 更新合计表单
	Totalvalueofgoodsform.value.totalValue = totalValue.toFixed(2);
	Totalvalueofgoodsform.value.totalQuantity = totalQuantity.toString();
}

var userId = useUserStore().userId;
var CheckUser = ref(userId.toString()); // 初始化为当前用户ID

// 按钮显示控制
const isSaveBtnShow = ref(false);        // 确定保存按钮(新增时使用)
const showEditBtn = ref(false);          // 编辑按钮
const showEditSaveBtn = ref(false);      // 编辑保存按钮
const showSubmitReviewBtn = ref(false);  // 提交审核按钮
const isFormDisabled = ref(true);        // 表单是否禁用
const showApproveRejectBtn = ref(false);      // 驳回按钮
const showApprovePassBtn = ref(false);        // 审核按钮

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
		sql_product: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_supplier_level' }, { dictType: 'hr_business_scope' }, { dictType: 'hr_china_provinces' }, { dictType: 'hr_china_city' }, { dictType: 'sql_sale_contracts' },
{ dictType: 'sql_supplier_info' }, { dictType: 'sql_hr_purchase' }, { dictType: 'hr_packing' }, { dictType: 'sql_product' }]


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
	priceTerms: '',
	customerid: ''
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
		SupplierID: product.vendorCode || 0,
		purchaseContractId: 0, // 新增时为0
		productCode: state.optionss.sql_product.find(item => item.dictLabel === product.productCode.toString())?.dictValue,
		customerCode: product.customerCode || '',
		chineseName: product.chineseName || '',
		englishName: product.englishName || '',
		chineseSpec: product.chineseSpec || '',
		contractQuantity: parseFloat(product.contractQuantity),
		unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
		purchasePrice: parseFloat(product.purchaseUnitPrice),
		purchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
		deliveryDate: product.deliveryDate,
		productionLeadTime: product.productionLeadTime,
		packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
		specialRequirements: product.specialRequirements || '',
		invoice: product.invoice == '是' ? 1 : 0,
		innerBoxQuantity: parseInt(product.innerBoxQuantity),
		outerBoxQuantity: parseInt(product.outerBoxQuantity),
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
		vendorCode: Addcontractofpurchaseform.value.vendorCode,
		vendorAbbreviation: Addcontractofpurchaseform.value.vendorAbbreviation,
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
		ProcurementRequirementID: ProcurementRequirementID.value
	};
	// 提交采购合同
	request.post("PurchaseContracts/AddPurchaseContracts/Add", contractRequest)
		.then(response => {
			if (response.code === 200) {
				ElMessage({
					message: "采购合同添加成功！",
					type: "success",
				});
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
// 保存编辑
const saveEditContract = () => {
	ElMessageBox.confirm('确认保存编辑的采购合同信息吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 构建产品数据
		const mappedProducts = productinfotableData.value.map(product => ({
			Id: product.id || 0,
			ProductId: product.productId || 0,
			PurchaseContractId: currentContractId.value,
			SupplierID: parseInt(product.vendorCode) || 0,
			ProductCode: product.productCode || '',
			CustomerCode: product.customerCode || '',
			ChineseName: product.chineseName || '',
			EnglishName: product.englishName || '',
			chineseSpec: product.chineseSpec || '',
			ContractQuantity: parseFloat(product.contractQuantity) || 0,
			Unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
			PurchasePrice: parseFloat(product.purchaseUnitPrice),
			PurchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
			DeliveryDate: product.deliveryDate,
			ProductionLeadTime: product.productionLeadTime,
			Packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
			SpecialRequirements: product.specialRequirements || '',
			Invoice: product.invoice == '否' ? 0 : 1,
			InnerBoxQuantity: parseInt(product.innerBoxQuantity),
			OuterBoxQuantity: parseInt(product.outerBoxQuantity),
			Remark: product.remark || '',
			IsDelete: 0,
			SupplierID: parseInt(product.vendorCode)
		}));

		// 构建厂商费用数据
		const mappedExpenses = CustomerRelaterExoensesTableData.value.map(expense => ({
			id: expense.id || 0,
			PurchaseContractID: currentContractId.value,
			expenseName: expense.expenseName,
			currency: parseInt(expense.currency),
			exchangeRate: parseFloat(expense.exchangeRate),
			expense: parseFloat(expense.expense),
			Remark: expense.remark || '',
			IsDelete: 0
		}));
		// 构建请求数据
		const purchaseContractsRequest = {
			id: currentContractId.value,
			PurchaseContractNumber: Addcontractofpurchaseform.value.purchaseContract,
			ContractStatus: 1,
			VendorCode: '',
			VendorAbbreviation: '',
			SalesContract: state.optionss.sql_sale_contracts.find(item => item.dictLabel === Addcontractofpurchaseform.value.salesContract.toString())?.dictValue,
			CustomerContract: Addcontractofpurchaseform.value.customerContract,
			CustomerAbbreviation: Addcontractofpurchaseform.value.customerAbbreviation,
			DeliveryDate: Addcontractofpurchaseform.value.deliveryDate,
			PurchaseCurrency: state.optionss.hr_export_currency.find(item => item.dictLabel === Addcontractofpurchaseform.value.purchaseCurrency.toString())?.dictValue,
			Deposit: parseFloat(Addcontractofpurchaseform.value.deposit || '0'),
			Salesperson: state.optionss.sql_hr_sale.find(item => item.dictLabel === Addcontractofpurchaseform.value.salesperson.toString())?.dictValue,
			Purchaser: state.optionss.sql_hr_purchase.find(item => item.dictLabel === Addcontractofpurchaseform.value.purchaser.toString())?.dictValue,
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
			ProcurementRequirementID: ProcurementRequirementID.value
		};

		// 发送请求
		request.post("PurchaseContracts/EditPurchaseContracts/Edit", purchaseContractsRequest)
			.then(response => {
				if (response.code === 200) {
					ElMessage({
						message: response.msg || "采购合同编辑成功！",
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
				} else {
					ElMessage.error(response.msg || '编辑采购合同失败');
				}
			})
			.catch(error => {
				console.error('编辑采购合同失败:', error);
				if (error.response?.data?.errors) {
					const errorMessages = Object.values(error.response.data.errors)
						.flat()
						.join(', ');
					ElMessage.error(`验证失败: ${errorMessages}`);
				} else {
					ElMessage.error('编辑采购合同失败，请重试');
				}
			});
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
			let isCheckVendor = true;
			productinfotableData.value.forEach(item => {
				if (item.vendorCode == null || item.vendorCode == 0 || item.vendorCode == undefined || item.vendorCode == '') {
					isCheckVendor = false;
				}
			});
			if (!isCheckVendor) {
				ElMessage.error('请为产品列表中的产品选择供应商！');
				return;
			}
			try {
				// 映射产品数据以匹配后端模型
				const mappedProducts = productinfotableData.value.map(product => ({
					id: product.id || 0,
					ProductID: product.productId || 0,
					SupplierID: product.vendorCode || 0,
					purchaseContractId: 0, // 新增时为0
					productCode: state.optionss.sql_product.find(item => item.dictLabel === product.productCode.toString())?.dictValue,
					customerCode: product.customerCode || '',
					chineseName: product.chineseName || '',
					englishName: product.englishName || '',
					chineseSpec: product.chineseSpec || '',
					contractQuantity: parseFloat(product.contractQuantity),
					unit: state.optionss.hr_calculate_unit.find(item => item.dictLabel === product.unit.toString())?.dictValue,
					purchasePrice: parseFloat(product.purchaseUnitPrice),
					purchaseTotalPrice: parseFloat(product.purchaseTotalPrice),
					deliveryDate: product.deliveryDate,
					productionLeadTime: product.productionLeadTime,
					packaging: state.optionss.hr_packing.find(item => item.dictLabel === product.packaging.toString())?.dictValue,
					specialRequirements: product.specialRequirements || '',
					invoice: product.invoice == '否' ? 0 : 1,
					innerBoxQuantity: parseInt(product.innerBoxQuantity),
					outerBoxQuantity: parseInt(product.outerBoxQuantity),
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
					vendorCode: Addcontractofpurchaseform.value.vendorCode,
					vendorAbbreviation: Addcontractofpurchaseform.value.vendorAbbreviation,
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
					ProcurementRequirementID: ProcurementRequirementID.value
				};
				// 提交采购合同
				request.post("PurchaseContracts/AddPurchaseContracts/Add", contractRequest)
					.then(async response => {
						if (response.code === 200) {
							const Reviewresponse = await request({
								url: 'PurchaseContracts/SubmitForReview/SubmitPurchseContractReview',
								method: 'GET',
								params: {
									ContractID: response.data
								}
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
			request({
				url: 'PurchaseContracts/SubmitForReview/SubmitPurchseContractReview',
				method: 'GET',
				params: {
					ContractID: currentContractId.value // 需要添加这个响应式变量来存储当前合同ID
				}
			}).then(async response => {
				if (response.code === 200) {
					ElMessage.success('提交审核成功');
					Addcontractofpurchasedialog.value = false;  // 关闭对话框
					// 刷新采购合同列表
					GetpurchaseContractList(
						purchasecontractsTableDatacurrentPage.value,
						purchasecontractsTableDatapageSize.value
					);
					await ProcurementRequirements(contractsTableDatacurrentPage.value, contractsTableDatapageSize.value);
				} else {
					ElMessage.error('提交审核失败');
				}
			}).catch(error => {
				console.error('提交审核失败:', error);
				ElMessage.error('提交审核失败，请重试');
			});
		}
	}).catch(() => {
		ElMessage.info('已取消提交审核');
	});
};


const handleClick = (tab: TabsPaneContext, event: Event) => {

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
				salesperson: state.optionss.sql_hr_sale.find(item => item.dictValue === element.salesperson.toString())?.dictLabel || '未知',
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
	'3': '已拒绝'
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
					// 添加空值检查和默认值
					element.contractStatus = optionss.value.hr_contract_status.find(item =>
						item.dictValue == element.contractStatus.toString())?.dictLabel || '未知状态';

					element.vendorCode = optionss.value.sql_supplier_info.find(item =>
						item.dictValue == element.vendorCode.toString())?.dictLabel || '未知供应商';

					element.salesContract = optionss.value.sql_sale_contracts.find(item =>
						item.dictValue == element.salesContract.toString())?.dictLabel || '未知合同';

					element.purchaseCurrency = optionss.value.hr_export_currency.find(item =>
						item.dictValue == element.purchaseCurrency.toString())?.dictLabel || '未知货币';

					element.salesperson = optionss.value.sql_hr_sale.find(item =>
						item.dictValue == element.salesperson.toString())?.dictLabel || '未知销售员';

					element.purchaser = optionss.value.sql_hr_purchase.find(item =>
						item.dictValue == element.purchaser.toString())?.dictLabel || '未知采购员';

					element.reviewStatus = element.reviewStatus.toString();
					element.reviewStatusStr = reviewStatusMap[element.reviewStatus.toString()] || '未知状态';
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
	CheckUser = state.optionss.sql_hr_purchase.find(p => p.dictLabel === row.purchaser.toString())?.dictValue;
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
		}).then(async response => {
			if (response.data.purchaseContractProducts.length > 0) {
				const products = response.data.purchaseContractProducts.map(element => {
					return {
						id: element.id,
						productId: element.productNumber, // 确保这里设置了productId
						vendorCode: element.supplierID, // 存储供应商ID
						productCode: state.optionss.sql_product.find(item => item.dictValue === element.productNumber.toString())?.dictLabel,
						chineseName: element.chineseName,
						chineseSpecification: element.chineseSpecification,
						unit: state.optionss.hr_calculate_unit.find(item => item.dictValue === element.unit.toString())?.dictLabel || '无',
						contractQuantity: element.contractQuantity,
						purchaseUnitPrice: element.purchasePrice,
						purchaseTotalPrice: element.purchaseTotalPrice,
						deliveryDate: element.deliveryDate,
						productionLeadTime: element.productionLeadTime,
						packaging: state.optionss.hr_packing.find(item => item.dictValue === element.packaging.toString())?.dictLabel || '无',
						specialRequirements: element.specialRequirements,
						innerBoxQuantity: element.innerBoxQuantity,
						outerBoxQuantity: element.outerBoxQuantity,
						invoice: element.invoke == '是' ? 1 : 0,
						remark: element.remark
					};
				});
				// 先加载供应商数据
				await loadSupplierData(products);
				// 然后设置产品数据
				productinfotableData.value = products;
			}
			if (response.data.purchaseContractVendorExpenses.length > 0) {
				CustomerRelaterExoensesTableData.value = [];

				CustomerRelaterExoensesTableData.value = response.data.purchaseContractVendorExpenses;
			}
			Addcontractofpurchasedialog.value = true;
			// 异步获取审批流程并设置审核按钮状态
			getApprovalFlow(row.id).then(() => {
				const isCurrentUserApprover = checkIfCurrentUserIsApprover();
				// 只有当前用户是审批人且合同在审核中时才显示审核按钮
				if (isCurrentUserApprover && row.reviewStatusStr === '审核中') {
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
const GetNewPurchaseContractNumber = () => {
	return new Promise((resolve, reject) => {
		request({
			url: 'PurchaseContracts/GetNewPurchaseContractNumber/GetPurchaseContractNumber',
			method: 'GET'
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
				ElMessage.error(res.msg || '删除失败');
			}
		}).catch(() => {
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage.info('已取消删除');
	});
};
</script>
