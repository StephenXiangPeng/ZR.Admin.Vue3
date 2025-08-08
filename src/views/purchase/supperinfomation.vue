<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenAddSupperDialog">添加供应商</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="Searchsupplierselect" filterable placeholder="选择供应商（可输入查询）" style="width: 15%">
				<el-option v-for="item in supplierselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Searchsupplierselect" filterable placeholder="选择供应商等级" style="width: 15%">
				<el-option v-for="item in supplierselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Searchproductselect" filterable placeholder="选择业务范围" style="width: 15%">
				<el-option v-for="item in productselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchinquiryDate" type="date" placeholder="请选择最近交易日期"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchquotationDate" type="date" placeholder="请选择最近交易日期" style="width: 15%" />
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
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;供应商信息表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="SupplierInfoTableData">
			<el-table-column prop="Id" label="供应商ID" width="150" v-if="false"></el-table-column>
			<el-table-column prop="supplierId" label="供应商编号" width="150">
				<template #default="scope">
					<span>{{ scope.row.supplierId }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="shortName" label="供应商简称" width="150"></el-table-column>
			<el-table-column prop="fullName" label="供应商全称" width="150"></el-table-column>
			<el-table-column prop="cooperationLevel" label="供应商等级" width="150"></el-table-column>
			<el-table-column prop="businessScope" label="业务范围" width="150"></el-table-column>
			<el-table-column prop="province" label="所在省份" width="150"></el-table-column>
			<el-table-column prop="address" label="详细地址" width="150"></el-table-column>
			<el-table-column prop="developmentDate" label="开发时间" width="150"></el-table-column>
			<el-table-column prop="lastTransaction" label="最近交易" width="150"></el-table-column>
			<el-table-column prop="quoteSuccessRate" label="报价成交率" width="150"></el-table-column>
			<el-table-column fixed="right" label="详情" width="150px">
				<template #default="scope">
					<el-button type="text" size="small" @click="checkSupplierDetails(scope.row)">查看详情</el-button>
					<el-button v-if="scope.row.createBy === useUserStore().userId.toString() && scope.row.isDraft" link
						type="danger" size="small" @click="DeleteSupplier(scope.row)">删除</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="SupplierInfoTableDatahandlePageChange"
			:current-page="SupplierInfoTableDatacurrentPage" :page-size="SupplierInfoTableDatapageSize"
			:total="SupplierInfoTableDatatotalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="AddSupperDialog" title="添加供应商" :close-on-click-modal=false style="width: 70%;"
			@close="Closeaddsupperdialog">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="Addsupperinfoform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="厂商编号">
							<el-input v-model="Addsupperinfoform.supplierId" placeholder="请输入厂商编号" disabled
								style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商简称">
							<el-input v-model="Addsupperinfoform.shortName" placeholder="请输入厂商简称" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="厂商名称">
							<el-input v-model="Addsupperinfoform.fullName" placeholder="请输入厂商名称" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="所在省份">
							<el-select v-model="Addsupperinfoform.province" placeholder="请选择所在省份" style="width: 300px"
								:disabled="isEditable">
								<el-option v-for="dict in optionss.hr_china_provinces" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>

							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="所在城市">
							<el-input v-model="Addsupperinfoform.city" placeholder="请输入所在城市" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="公司主页">
							<el-input v-model="Addsupperinfoform.website" placeholder="请输入公司主页" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="邮政编码">
							<el-input v-model="Addsupperinfoform.postalCode" placeholder="请输入邮政编码"
								:disabled="isEditable" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="详细地址">
							<el-input v-model="Addsupperinfoform.address" placeholder="请输入详细地址" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="业务范围">
							<el-input v-model="Addsupperinfoform.businessScope" placeholder="请输入业务范围"
								style="width: 300px" :disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="信用等级">
							<el-select v-model="Addsupperinfoform.creditLevel" filterable placeholder="自动计算信用等级"
								disabled style="width: 300px;">
								<el-option v-for="dict in optionss.hr_supplier_level" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="合作等级">
							<el-select v-model="Addsupperinfoform.cooperationLevel" filterable placeholder="自动计算合作等级"
								disabled style="width: 300px;">
								<el-option v-for="dict in optionss.hr_supplier_level" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="结算方式">
							<el-select v-model="Addsupperinfoform.paymentMethod" placeholder="请选择结算方式"
								:disabled="isEditable" style="width: 300px">
								<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="公司税号">
							<el-input v-model="Addsupperinfoform.taxNumber" placeholder="请输入公司税号" :disabled="isEditable"
								style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8" v-if="false">
						<el-form-item label="开户银行">
							<el-input v-model="Addsupperinfoform.bankName" placeholder="请输入开户银行" style="width: 300px"
								:disabled="isEditable" />
						</el-form-item>
					</el-col>
					<el-col :span="8" v-if="false">
						<el-form-item label="银行账号">
							<el-input v-model="Addsupperinfoform.bankAccount" placeholder="请输入银行账号"
								:disabled="isEditable" style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="开发时间">
							<el-date-picker v-model="Addsupperinfoform.developmentDate" type="date"
								:disabled="isEditable" placeholder="请选择开发时间" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="最近成交日期">
							<el-date-picker v-model="Addsupperinfoform.lastTransaction" disabled type="date"
								placeholder="自动获取" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="能否开票">
							<el-checkbox v-model="Addsupperinfoform.canInvoice" :disabled="isEditable" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-form-item label="供应商图片">
						<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList" limit="5"
							:disabled="fileList.length >= 5" @change="handleChange" :action="UploadUrl"
							:data="formData">
							<el-icon>
								<Plus />
							</el-icon>
							<template #file="{ file }">
								<div>
									<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
									<span class="el-upload-list__item-actions">
										<span class="el-upload-list__item-preview"
											@click="handlePictureCardPreview(file)">
											<el-icon><zoom-in /></el-icon>
										</span>
										<span v-if="!disabled" class="el-upload-list__item-delete"
											@click="handleRemove(file)">
											<el-icon>
												<Delete />
											</el-icon>
										</span>
									</span>
								</div>
							</template>
						</el-upload>
						<el-dialog v-model="dialogVisible">
							<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
								:src="dialogImageUrl" alt="Preview Image" />
						</el-dialog>
					</el-form-item>

				</el-row>
			</el-form>
			<el-tabs v-model="activeTab" class="demo-tabs">
				<el-tab-pane label="联系人" name="contacttabpane">
					<el-button class="mt-4" type="primary" @click="handleAddContactRow" :disabled="isEditable"
						style="margin-bottom: 10px;">添加联系人</el-button>
					<el-table :data="supperinfoContactsTableData" style="width: 100%">
						<el-table-column label="联系人姓名">
							<template #default="{ row }">
								<el-input v-model="row.name" placeholder="联系人姓名" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人部门">
							<template #default="{ row }">
								<el-input v-model="row.department" placeholder="联系人部门" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人性别">
							<template #default="{ row }">
								<el-select v-model="row.gender" placeholder="联系人性别" :disabled="isEditable">
									<el-option v-for="dict in optionss.sys_user_sex" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue"></el-option>
								</el-select>
							</template>
						</el-table-column>
						<el-table-column label="联系人职务">
							<template #default="{ row }">
								<el-input v-model="row.position" placeholder="联系人职务" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人电话号码">
							<template #default="{ row }">
								<el-input v-model="row.phoneNumber" placeholder="电话号码" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人手机号码">
							<template #default="{ row }">
								<el-input v-model="row.mobileNumber" placeholder="手机号码" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人电子邮件">
							<template #default="{ row }">
								<el-input v-model="row.email" placeholder="电子邮件" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="联系人备注">
							<template #default="{ row }">
								<el-input v-model="row.remark" placeholder="备注" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="small" :disabled="isEditable"
									@click="handleDeleteContactRow(scope.$index)">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="银行账号" name="second">
					<el-button class="mt-4" type="primary" @click="handleAddBankRow" :disabled="isEditable"
						style="margin-bottom: 10px;">添加银行账号</el-button>
					<el-table :data="supperinfoBankAccountInfoTableData" style="width: 100%">
						<el-table-column label="开户名称">
							<template #default="{ row }">
								<el-input v-model="row.bank_account_name" placeholder="输入开户名称" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="开户银行">
							<template #default="{ row }">
								<el-input v-model="row.bank" placeholder="输入开户银行" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="银行账号">
							<template #default="{ row }">
								<el-input v-model="row.bank_account_number" placeholder="输入银行账号"
									:disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="银行地址" v-if="false">
							<template #default="{ row }">
								<el-input v-model="row.bank_address" placeholder="输入银行地址" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="备注">
							<template #default="{ row }">
								<el-input v-model="row.remark" placeholder="输入备注" :disabled="isEditable" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button type="text" size="small" :disabled="isEditable"
									@click="handleDeleteBankAccountRow(scope.$index)">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="产品清单" name="ProductList">
					<el-table :data="supperinfoProductTableData" style="width: 100%">
						<el-table-column prop="ProductCode" label="产品编号" width="150"></el-table-column>
						<el-table-column prop="ChineseName" label="中文品名" width="300"></el-table-column>
						<el-table-column prop="ChineseSpecifications" label="中文规格" width="300"></el-table-column>
						<el-table-column prop="Unit" label="计量单位" width="150"></el-table-column>
						<el-table-column prop="lastTransaction" label="最近成交" width="200"></el-table-column>
						<el-table-column prop="packagingMethod" label="包装方式" width="150" v-if="false"></el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="报价历史" name="quotationHistory">
					<el-table :data="supperinfoQuotationHistoryData" style="width: 100%">
						<el-table-column prop="inquiryDate" label="询价日期" width="120"></el-table-column>
						<el-table-column prop="productCode" label="产品编号" width="120"></el-table-column>
						<el-table-column prop="chineseName" label="中文品名" width="200"></el-table-column>
						<el-table-column prop="chineseSpecifications" label="中文规格" width="300"></el-table-column>
						<el-table-column prop="packagingMethod" label="包装方式" width="120"></el-table-column>
						<el-table-column prop="purchasePrice" label="采购价格" width="100"></el-table-column>
						<el-table-column prop="unit" label="计量单位" width="100"></el-table-column>
						<el-table-column prop="quotationQuantity" label="报价数量" width="100"></el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="采购历史" name="purchaseHistory">
					<el-table :data="supperinfoPurchaseHistoryData" style="width: 100%">
						<el-table-column prop="purchaseContractNumber" label="采购合同号"></el-table-column>
						<el-table-column prop="contractStatus" label="合同状态"></el-table-column>
						<el-table-column prop="contractQuantity" label="合同数量"></el-table-column>
						<el-table-column prop="purchasePrice" label="采购单价"></el-table-column>
						<el-table-column prop="purchaseTotalPrice" label="采购总价"></el-table-column>
						<el-table-column prop="create_time" label="创建日期"></el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="收寄样历史" name="SampleCollectionHistory">
					<el-table :data="supperinfoSendSampleData" style="width: 100%">
						<el-table-column prop="type" label="寄样/收样" width="100"></el-table-column>
						<el-table-column prop="waybillNumber" label="运单号" width="150"></el-table-column>
						<el-table-column prop="expressCompany" label="快递公司" width="120"></el-table-column>
						<el-table-column prop="sampleDate" label="样品日期" width="120"></el-table-column>
						<el-table-column prop="abbreviation" label="供应商简称" width="150"></el-table-column>
						<el-table-column prop="companyName" label="我方公司" width="150"></el-table-column>
						<el-table-column prop="paymentMethod" label="付费方式" width="120"></el-table-column>
						<el-table-column prop="paidExpressFee" label="已付快递费" width="120">
							<template #default="scope">
								<span>￥{{ scope.row.paidExpressFee }}</span>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注"></el-table-column>
					</el-table></el-tab-pane>
				<el-tab-pane label="往来邮件" name="emailHistory">
					<el-table :data="EmailHistoryData" style="width: 100%; max-height: 300px; overflow-y: auto;">
						<el-table-column prop="EmailDate" label="联系日期" />
						<el-table-column prop="Contact" label="联系人" />
						<el-table-column prop="OurPersonnel" label="我方人员" />
						<el-table-column prop="ContactDetails" label="联系内容" show-overflow-tooltip />
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="客诉历史" name="complaintHistory">
					<el-table :data="supperinfoComplaintHistoryData" style="width: 100%">
						<el-table-column prop="purchaseContract" label="采购合同" width="120"></el-table-column>
						<el-table-column prop="productCode" label="产品编号" width="120"></el-table-column>
						<el-table-column prop="chineseName" label="中文品名" width="120"></el-table-column>
						<el-table-column prop="chineseSpecifications" label="中文规格" width="120"></el-table-column>
						<el-table-column prop="shippingQuantity" label="出货数量" width="100"></el-table-column>
						<el-table-column prop="unit" label="计量单位" width="100"></el-table-column>
						<el-table-column prop="claimAmount" label="索赔金额" width="100"></el-table-column>
						<el-table-column prop="actualCompensation" label="实赔金额" width="100"></el-table-column>
						<el-table-column prop="salesContract" label="销售合同" width="120"></el-table-column>
						<el-table-column prop="shippingDate" label="出运日期" width="120"></el-table-column>
						<el-table-column prop="customerShortName" label="客户简称" width="120"></el-table-column>
					</el-table></el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isEditBtnVisible" type="primary" @click="IsEditBtnClick">
						编辑
					</el-button>
					<el-button v-show="isSavebtnVisible" type="warning" @click="SaveSupperinfo">
						保存草稿
					</el-button>
					<el-button v-show="isEditSaveBtnVisible" type="warning" @click="EditSaveSupperinfo">
						保存草稿
					</el-button>
					<el-button v-show="isSubmitbtnVisible" type="success" @click="SubmitSupperinfo">
						提交
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
import qs from 'qs';
import useUserStore from "@/store/modules/user";

const supperinfoBankAccountInfoTableData = ref([]) //银行账号
const supperinfoProductTableData = ref([]) //产品清单
const supperinfoQuotationHistoryData = ref([]) //报价历史
const supperinfoPurchaseHistoryData = ref([]) //采购历史
const supperinfoSendSampleData = ref([]) //收寄样历史
const supperinfoReceiveSampleData = ref([]) //收样历史	
const supperinfoComplaintHistoryData = ref([]) //客诉历史
const EmailHistoryData = ref([]) //往来邮件
const currentSupplierProductIds = ref<number[]>([]);// 存储当前供应商所有产品ID的数组

// 获取供应商产品列表
const getSupplierProductList = (supplierId: number) => {
	return request({
		url: 'ProductInformation/GetProductListBySupplierID/GetProductList',
		method: 'get',
		params: { SupplierID: supplierId }
	})
}

// 获取报价历史的方法，支持多个产品ID
const loadProductInquiryHistory = async (productIds: number[]) => {
	try {
		const response = await request({
			url: 'Inquiry/GetInquiryProductHistoryListByProductIDs/GetInquiryProductHistoryList',
			method: 'GET',
			params: {
				productIDs: productIds
			},
			paramsSerializer: (params) => {
				return qs.stringify(params, { arrayFormat: 'repeat' });
			}
		});
		if (response.code === 200) {
			// 使用产品列表数据创建查找映射
			const productInfoMap = supperinfoProductTableData.value.reduce((map, product) => {
				map[product.ProductCode] = {
					chineseName: product.ChineseName,
					unit: product.Unit,
					specifications: product.ChineseSpecifications
				};
				return map;
			}, {});

			// 映射报价历史数据，并包含产品相关信息
			supperinfoQuotationHistoryData.value = response.data.map(item => {
				const productInfo = productInfoMap[item.productNumber] || {
					chineseName: '',
					unit: '',
					specifications: ''
				};
				const date = item.date ? new Date(item.date) : null;
				const formattedDate = date ?
					`${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}` :
					'';
				return {
					inquiryDate: formattedDate,
					productCode: item.productNumber,
					chineseName: productInfo.chineseName,
					chineseSpecifications: productInfo.specifications || item.productSpecifications, // 优先使用产品列表中的规格
					packagingMethod: item.smallPackagingMethod,
					purchasePrice: item.price,
					unit: productInfo.unit || item.unit, // 优先使用产品列表中的单位
					quotationQuantity: item.quoteQuantity
				};
			});
		} else {
			ElMessage.error(response.msg || '获取报价历史失败');
		}
	} catch (error) {
		console.error('获取报价历史失败:', error);
		ElMessage.error('获取报价历史失败，请稍后重试');
	}
};

// 获取供应商产品列表
const loadSupplierProductList = async (supplierId: number) => {
	try {
		const response = await getSupplierProductList(supplierId);
		if (response && response.code === 200) {
			// 清空产品ID数组
			currentSupplierProductIds.value = [];
			// 处理产品列表数据
			supperinfoProductTableData.value = response.data.map(item => {
				// 收集产品ID
				if (item.id) {
					currentSupplierProductIds.value.push(item.id);
				}
				return {
					id: item.id,
					ProductCode: item.productCode,
					ChineseName: item.chineseProductName,
					ChineseSpecifications: item.chineseSpecification,
					Unit: state.optionss.hr_calculate_unit.find(
						unit => unit.dictValue === item.unitOfMeasurement.toString()
					)?.dictLabel || '',
					lastTransaction: item.recentTransactionDate,
					packagingMethod: item.packingMethod
				};
			});
			// 如果有产品ID，则获取所有产品的报价历史
			if (currentSupplierProductIds.value.length > 0) {
				await loadProductInquiryHistory(currentSupplierProductIds.value);
			} else {
				// 清空报价历史数据
				supperinfoQuotationHistoryData.value = [];
			}

		} else {
			ElMessage.error('获取供应商产品列表失败');
		}
	} catch (error) {
		console.error('获取供应商产品列表出错：', error);
		ElMessage.error('获取供应商产品列表失败');
	}
};

//#region 银行账号相关的方法
// 添加银行账号
const handleAddBankRow = () => {
	const newRow = {
		supplier_id: SelctedSupplierId.value,
		bank_account_name: '',
		bank: '',
		bank_account_number: '',
		bank_address: '',
		remark: ''
	};
	supperinfoBankAccountInfoTableData.value.push(newRow);
}

// 删除银行账号
const handleDeleteBankAccountRow = (index) => {
	ElMessageBox.confirm('确定要删除该银行账号吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		const row = supperinfoBankAccountInfoTableData.value[index];
		if (row.id) {
			// 如果有ID，说明是已存在的记录，调用删除接口
			request({
				url: 'Supplierinfo/DeleteSupplierBankAccount/DelBankAccount',
				method: 'POST',
				data: row
			}).then(response => {
				if (response && response.code === 200) {
					ElMessage.success(response.msg);
					supperinfoBankAccountInfoTableData.value.splice(index, 1);
				} else {
					ElMessage.error(response.msg);
				}
			});
		} else {
			// 如果没有ID，说明是新添加的记录，直接从数组中删除
			supperinfoBankAccountInfoTableData.value.splice(index, 1);
		}
	});
}

// 保存银行账号信息
const saveBankAccountInfo = () => {
	const promises = supperinfoBankAccountInfoTableData.value.map(row => {
		if (row.id) {
			// 更新已存在的记录
			return request({
				url: 'Supplierinfo/EditBankAccount/EditBankAccount',
				method: 'POST',
				data: row
			});
		} else {
			// 添加新记录
			return request({
				url: 'Supplierinfo/AddSupplierBankAccount/AddBankAccount',
				method: 'POST',
				data: row
			});
		}
	});

	Promise.all(promises)
		.then(responses => {
			const hasError = responses.some(response => response && response.code !== 200);
			if (!hasError) {
				ElMessage.success('信息保存成功！');
				loadBankAccountList(); // 重新加载列表
			} else {
				ElMessage.error('部分银行账号信息保存失败，请重试！');
			}
		})
		.catch(error => {
			console.error('保存银行账号信息出错：', error);
			ElMessage.error('保存银行账号信息失败！');
		});
}

// 加载银行账号列表
const loadBankAccountList = () => {
	if (SelctedSupplierId.value == '') {
		return;
	} else {
		request({
			url: 'Supplierinfo/GetSupplierBankAccountList/GetBankAccountList',
			method: 'GET',
			params: {
				supplierId: SelctedSupplierId.value
			}
		}).then(response => {
			if (response && response.code === 200) {
				supperinfoBankAccountInfoTableData.value = response.data;
			} else {
				ElMessage.error('获取银行账号列表失败！');
			}
		}).catch(error => {
			console.error('获取银行账号列表出错：', error);
			ElMessage.error('获取银行账号列表失败！');
		});
	}
}
//#endregion	



const activeTab = ref('contacttabpane')
const isEditBtnVisible = ref(false)
const isEditSaveBtnVisible = ref(false)
const isSavebtnVisible = ref(true)
const isSubmitbtnVisible = ref(true)

/*供应商联系人列表*/
const supperinfoContactsTableData = ref([]);
const handleAddContactRow = () => {
	const newRow = {
		name: '',
		department: '',
		gender: '',
		position: '',
		phoneNumber: '',
		mobileNumber: '',
		email: '',
		remarks: ''
	};
	supperinfoContactsTableData.value.push(newRow);
}

//获取供应商编号
const GetNextSupplierID = () => {
	request({
		url: 'Supplierinfo/GetNewSupplierID/GetSupplierID',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			Addsupperinfoform.supplierId = response.data || response;
		} else {
			console.error('获取下一个供应商编号出错');
		}
	}).catch(error => {
		console.error('获取下一个供应商编号出错！😔错误内容：', error);
	})
}

const OpenAddSupperDialog = () => {
	GetNextSupplierID();
	AddSupperDialog.value = true;
}

const handleDeleteContactRow = (index) => {
	supperinfoContactsTableData.value.splice(index, 1);
}
/*供应商联系人列表*/
const Addsupperinfoform = reactive({
	Id: 0,
	supplierId: '',
	shortName: '',
	fullName: '',
	province: '',
	city: '',
	website: '',
	postalCode: '',
	address: '',
	businessScope: '',
	creditLevel: '',
	cooperationLevel: '',
	canInvoice: '',
	paymentMethod: '',
	bankName: '',
	bankAccount: '',
	taxNumber: '',
	developmentDate: '',
	lastTransaction: '',
	factoryImageURL: ''
})

const SupplierRequest = reactive({
	Id: 0,
	SupplierId: '',
	ShortName: '',
	FullName: '',
	Province: '',
	City: '',
	Website: '',
	PostalCode: '',
	Address: '',
	BusinessScope: '',
	CreditLevel: '',
	CooperationLevel: '',
	CanInvoice: false,
	PaymentMethod: '',
	BankName: '',
	BankAccount: '',
	TaxNumber: '',
	DevelopmentDate: '',
	LastTransaction: '',
	FactoryImageUrl: '',
	IsDelete: 0,
	IsDraft: 1,
	contactInfoItems: []
});

const AddSupperDialog = ref(false)

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
		hr_recipient_type_examples: [],
		hr_express_delivery_company: [],
		hr_express_payment_method: [],
		sys_user_sex: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' },
{ dictType: 'hr_supplier_level' }, { dictType: 'hr_business_scope' }, { dictType: 'hr_china_provinces' }, { dictType: 'hr_china_city' },
{ dictType: 'hr_recipient_type_examples' }, { dictType: 'hr_express_delivery_company' }, { dictType: 'hr_express_payment_method' }, { dictType: 'sys_user_sex' }]
// 获取字典数据
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})
/*动态下拉框end*/

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)
const fileList = ref<UploadUserFile[]>([])
const uploadedFiles = ref([]);  // 用于存储已上传的文件



const SaveSupperinfo = () => {
	ElMessageBox.confirm('确定保存供应商信息？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 保存供应商信息
		SupplierRequest.Id = 0
		SupplierRequest.SupplierId = Addsupperinfoform.supplierId
		SupplierRequest.ShortName = Addsupperinfoform.shortName
		SupplierRequest.FullName = Addsupperinfoform.fullName
		SupplierRequest.Province = Addsupperinfoform.province
		SupplierRequest.City = Addsupperinfoform.city
		SupplierRequest.Website = Addsupperinfoform.website
		SupplierRequest.PostalCode = Addsupperinfoform.postalCode
		SupplierRequest.Address = Addsupperinfoform.address
		SupplierRequest.BusinessScope = Addsupperinfoform.businessScope
		SupplierRequest.CreditLevel = Addsupperinfoform.creditLevel
		SupplierRequest.CooperationLevel = Addsupperinfoform.cooperationLevel
		SupplierRequest.CanInvoice = Addsupperinfoform.canInvoice ? true : false
		SupplierRequest.PaymentMethod = Addsupperinfoform.paymentMethod
		SupplierRequest.BankName = Addsupperinfoform.bankName
		SupplierRequest.BankAccount = Addsupperinfoform.bankAccount
		SupplierRequest.TaxNumber = Addsupperinfoform.taxNumber
		SupplierRequest.DevelopmentDate = Addsupperinfoform.developmentDate
		SupplierRequest.LastTransaction = Addsupperinfoform.lastTransaction
		SupplierRequest.IsDelete = 0
		SupplierRequest.contactInfoItems = supperinfoContactsTableData.value
		SupplierRequest.IsDraft = 1
		// 如果联系人为空，赋值空字符串
		SupplierRequest.contactInfoItems.forEach((element) => {
			element.name = element.name || '';
			element.department = element.department || '';
			element.gender = element.gender || '2';
			element.position = element.position || '';
			element.phoneNumber = element.phoneNumber || '';
			element.mobileNumber = element.mobileNumber || '';
			element.email = element.email || '';
			element.Remark = element.remarks || '无';
		});
		//上传供应商图片
		const uploadPromises = fileList.value.map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'Supplier/SupplierPhoto');
			formData.append('FileNameType', '1');
			formData.append('File', file.raw);
			formData.append('storeType', '1');
			// 返回上传文件的 Promise
			return request.postForm(UploadUrl, formData);
		});
		Promise.all(uploadPromises).then(responses => {
			responses.forEach((response, index) => {
				if (response != null) {
					filelistUrlStr.value += (index > 0 ? ',' : '') + response.data.url;
				} else {
					ElMessage({
						message: "上传供应商图片出错！😔",
						type: 'error'
					})
				}
			});
			// 保存供应商
			Addsupperinfoform.factoryImageURL = filelistUrlStr.value;
			SupplierRequest.FactoryImageUrl = Addsupperinfoform.factoryImageURL;
			request.post('Supplierinfo/AddSupplierinfo/Add', SupplierRequest).then(response => {
				if (response != null) {
					saveBankAccountInfo();// 保存银行账号信息
					loadBankAccountList();// 加载银行账号列表
					isEditable.value = true;
					ElMessage({
						message: response.msg || '保存成功',
						type: 'success'
					})
					GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
					AddSupperDialog.value = false;
					SelctedSupplierId.value = '';
				} else {
					console.error('新增供应商资料出错');
				}
			}).catch(error => {
				console.error('新增供应商资料出错！😔错误内容：', error);
			})
		}).catch(error => {
			console.error('上传供应商图片出错！😔错误内容：', error);
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}

// 上传文件
const handleChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 5) {
		ElMessage({
			type: 'info',
			message: '最多上传3张图片！'
		});
		fileList.splice(3); // 保留前三个文件，移除其余文件
		return; // 不再继续执行后面的代码
	}
	const duplicate = uploadedFiles.value.findIndex(fileItem => fileItem.name === file.name);
	if (duplicate !== -1) {
		ElMessage({
			type: 'info',
			message: '不要上传重复的文件哦！😔'
		});
		const duplicatesInFileList = fileList.filter(fileItem => fileItem.name === file.name);
		if (duplicatesInFileList.length > 1) {
			for (let i = 1; i < duplicatesInFileList.length; i++) {
				const index = fileList.findIndex(fileItem => fileItem.uid === duplicatesInFileList[i].uid);
				if (index !== -1) {
					fileList.splice(index, 1); // 从fileList中移除重复文件，保留一个
				}
			}
		}
	} else {
		// 添加文件到uploadedFiles，确保不重复
		const newFiles = fileList.filter(file => !uploadedFiles.value.some(fileItem => fileItem.name === file.name));
		newFiles.forEach(file => {
			if (!file.isChanged) {
				file.isChanged = true;
				uploadedFiles.value.push(file);
			}
		});
	}
};

// 预览图片
const handlePictureCardPreview = (file: UploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}
// 删除图片
const handleRemove = (file: UploadFile) => {
	ElMessageBox.confirm('确定删除该图片吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		// 从 fileList 中移除
		const index = fileList.value.findIndex(f => f.uid === file.uid);
		if (index !== -1) {
			fileList.value.splice(index, 1);
		}

		// 从 uploadedFiles 中移除
		const uploadedIndex = uploadedFiles.value.findIndex(f => f.uid === file.uid);
		if (uploadedIndex !== -1) {
			uploadedFiles.value.splice(uploadedIndex, 1);
		}

		// 从 NewCustomerleadsform.compantPhotoStr 中移除对应的 url
		if (Addsupperinfoform.factoryImageURL) {
			const urls = Addsupperinfoform.factoryImageURL.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				Addsupperinfoform.factoryImageURL = urls.join(',');
			}
		}
		ElMessage({
			type: 'success',
			message: '删除成功!',
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除',
		});
	});
};

//查询条件
const Searchsupplierselect = ref('')
const Searchproductselect = ref('')
const SearchinquiryDate = ref('')
const SearchquotationDate = ref('')

// 添加缺失的变量定义
const supplierselectoptions = ref([])
const productselectoptions = ref([])
const formData = ref({})

// 供应商信息表
const SupplierInfoTableData = ref([]);
//分页组件
const SupplierInfoTableDatatotalItems = ref(0);
const SupplierInfoTableDatacurrentPage = ref(1);
const SupplierInfoTableDatapageSize = ref(10);
const SupplierInfoTableDatahandlePageChange = async (newPage) => {
	SupplierInfoTableDatacurrentPage.value = newPage;
	const start = newPage;
	const end = SupplierInfoTableDatapageSize.value;
	const newData = await GetSupplierInfoList(start, end);
};
GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
function GetSupplierInfoList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'Supplierinfo/GetSupplierList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				// 转换所在省份为label
				const processedData = response.data.result.map(item => {
					const provinceOption = state.optionss.hr_china_provinces.find(
						option => option.dictValue === item.province?.toString()
					);
					return {
						...item,
						province: provinceOption?.dictLabel || item.province
					};
				});
				SupplierInfoTableData.value = processedData;
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetSupplierInfoList(start - 1, end);
				} else {
					SupplierInfoTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const isEditable = ref(false);
// 供应商详情
const SelctedSupplierId = ref('')
//查看供应商详情
const checkSupplierDetails = async (row) => {
	if (row.isDraft == 0) {
		isSubmitbtnVisible.value = false;
	} else {
		isSubmitbtnVisible.value = true;
	}
	SelctedSupplierId.value = row.id;
	loadSupplierProductList(row.id)// 加载供应商产品列表
	loadBankAccountList();// 加载银行账号列表
	isEditBtnVisible.value = true;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = false;
	await loadSupplierSendSampleHistory(row.id);
	// 获取供应商信息
	isEditable.value = true;
	Addsupperinfoform.Id = row.id;
	Addsupperinfoform.supplierId = row.supplierId;
	Addsupperinfoform.shortName = row.shortName;
	Addsupperinfoform.fullName = row.fullName;
	Addsupperinfoform.province = row.province;
	Addsupperinfoform.city = row.city;
	Addsupperinfoform.website = row.website;
	Addsupperinfoform.postalCode = row.postalCode;
	Addsupperinfoform.address = row.address;
	Addsupperinfoform.businessScope = row.businessScope;
	Addsupperinfoform.creditLevel = row.creditLevel;
	Addsupperinfoform.cooperationLevel = row.cooperationLevel;
	Addsupperinfoform.canInvoice = row.canInvoice;
	Addsupperinfoform.paymentMethod = row.paymentMethod;
	Addsupperinfoform.bankName = row.bankName;
	Addsupperinfoform.bankAccount = row.bankAccount;
	Addsupperinfoform.taxNumber = row.taxNumber;
	Addsupperinfoform.developmentDate = row.developmentDate;
	Addsupperinfoform.lastTransaction = row.lastTransaction;
	if (row.factoryImageUrl != null && row.factoryImageUrl != '') {
		Addsupperinfoform.factoryImageURL = row.factoryImageUrl;
		row.factoryImageUrl.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false
				} as any);
			}
		});
	}
	uploadedFiles.value = fileList.value;
	// 获取联系人信息
	request({
		url: 'Supplierinfo/GetSupplierContactBySUpplierID/GetSupplierContact',
		method: 'GET',
		params: {
			SupplierID: SelctedSupplierId.value
		}
	}).then(response => {
		if (response.data.length > 0) {
			supperinfoContactsTableData.value = response.data;
			supperinfoContactsTableData.value.forEach((element) => {
				element.gender = state.optionss.sys_user_sex.find(option => option.dictValue === element.gender.toString())?.dictValue || '';
			})
			const emailAddress = response.data.map(item => item.email).join(',');
			if (emailAddress != null && emailAddress != '') {
				getEmailHistoryList(emailAddress);
			}
		} else {
			supperinfoContactsTableData.value = [];
		}
	}).catch(error => {
		console.error(error);
	});
	//获取采购历史
	request({
		url: 'PurchaseContracts/GetPurchseHistoryListBySupplierID/GetPurchaseHistoryList',
		method: 'GET',
		params: {
			SupplierID: SelctedSupplierId.value
		}
	}).then(response => {
		if (response.data.length > 0) {
			supperinfoPurchaseHistoryData.value = response.data;
			supperinfoPurchaseHistoryData.value.forEach((element) => {
				element.contractStatus = state.optionss.hr_contract_status.find(option => option.dictValue === element.contractStatus.toString())?.dictLabel || '未知状态';
			})
		} else {
			supperinfoPurchaseHistoryData.value = [];
		}
	}).catch(error => {
		console.error(error);
	});
	AddSupperDialog.value = true;
}
const IsEditBtnClick = () => {
	isEditable.value = false;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = true;
}

const EditSaveSupperinfo = () => {
	ElMessageBox.confirm('确定保存已完成编辑供应商信息？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 保存供应商信息
		SupplierRequest.Id = Number(Addsupperinfoform.Id)
		SupplierRequest.SupplierId = Addsupperinfoform.supplierId
		SupplierRequest.ShortName = Addsupperinfoform.shortName
		SupplierRequest.FullName = Addsupperinfoform.fullName
		SupplierRequest.Province = Addsupperinfoform.province
		SupplierRequest.City = Addsupperinfoform.city
		SupplierRequest.Website = Addsupperinfoform.website
		SupplierRequest.PostalCode = Addsupperinfoform.postalCode
		SupplierRequest.Address = Addsupperinfoform.address
		SupplierRequest.BusinessScope = Addsupperinfoform.businessScope
		SupplierRequest.CreditLevel = Addsupperinfoform.creditLevel
		SupplierRequest.CooperationLevel = Addsupperinfoform.cooperationLevel
		SupplierRequest.CanInvoice = Addsupperinfoform.canInvoice ? false : true
		SupplierRequest.PaymentMethod = Addsupperinfoform.paymentMethod
		SupplierRequest.BankName = Addsupperinfoform.bankName
		SupplierRequest.BankAccount = Addsupperinfoform.bankAccount
		SupplierRequest.TaxNumber = Addsupperinfoform.taxNumber
		SupplierRequest.DevelopmentDate = Addsupperinfoform.developmentDate
		SupplierRequest.LastTransaction = Addsupperinfoform.lastTransaction
		SupplierRequest.IsDelete = 0
		SupplierRequest.contactInfoItems = supperinfoContactsTableData.value
		SupplierRequest.contactInfoItems.forEach((element) => {
			element.name = element.name || '';
			element.department = element.department || '';
			element.gender = element.gender || '2';
			element.position = element.position || '';
			element.phoneNumber = element.phoneNumber || '';
			element.mobileNumber = element.mobileNumber || '';
			element.email = element.email || '';
			element.Remark = element.remarks || '无';
		});
		// 上传供应商图片
		const uploadPromises = fileList.value.filter(file => (file as any).isChanged).map(file => {
			const formData = new FormData();
			formData.append('FileName', file.name);
			formData.append('FileDir', 'Supplier/SupplierPhoto');
			formData.append('FileNameType', '1');
			formData.append('File', file.raw);
			formData.append('storeType', '1');
			// 返回上传文件的 Promise
			return request.postForm(UploadUrl, formData);
		});
		Promise.all(uploadPromises).then(responses => {
			responses.forEach((response, index) => {
				if (response != null) {
					filelistUrlStr.value += (index > 0 ? ',' : '') + response.data.url;
				} else {
					ElMessage({
						message: "上传供应商图片出错！😔",
						type: 'error'
					})
				}
			});
			// 编辑保存线索
			SupplierRequest.FactoryImageUrl = filelistUrlStr.value;
			request.post('Supplierinfo/EditSupplierinfo/Edit', SupplierRequest).then(response => {
				if (response != null) {
					saveBankAccountInfo();// 保存银行账号信息
					loadBankAccountList();// 加载银行账号列表
					isEditable.value = true;
					// 更新按钮状态
					isEditBtnVisible.value = true;        // 显示编辑按钮
					isEditSaveBtnVisible.value = false;   // 隐藏编辑保存按钮
					GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
					isSubmitbtnVisible.value = true;
				} else {
					console.error('编辑供应商信息出错');
				}
			}).catch(error => {
				console.error('保存已编辑的供应商信息出错！😔错误内容：', error);
			})
		}).catch(error => {
			console.error('上传供应商图片出错！😔错误内容：', error);
		});
	}
	).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		});
	});
}

const Closeaddsupperdialog = () => {
	supperinfoProductTableData.value = [] // 关闭对话框时清空供应商产品数据
	supperinfoBankAccountInfoTableData.value = [];// 关闭对话框时清空银行账号数据
	Addsupperinfoform.supplierId = '';
	Addsupperinfoform.shortName = '';
	Addsupperinfoform.fullName = '';
	Addsupperinfoform.province = '';
	Addsupperinfoform.city = '';
	Addsupperinfoform.website = '';
	Addsupperinfoform.postalCode = '';
	Addsupperinfoform.address = '';
	Addsupperinfoform.businessScope = '';
	Addsupperinfoform.creditLevel = '';
	Addsupperinfoform.cooperationLevel = '';
	Addsupperinfoform.canInvoice = '';
	Addsupperinfoform.paymentMethod = '';
	Addsupperinfoform.bankName = '';
	Addsupperinfoform.bankAccount = '';
	Addsupperinfoform.taxNumber = '';
	Addsupperinfoform.developmentDate = '';
	Addsupperinfoform.lastTransaction = '';
	Addsupperinfoform.factoryImageURL = '';
	supperinfoContactsTableData.value = [];
	fileList.value = [];
	uploadedFiles.value = [];
	isEditable.value = false;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = true;
}

// 定义接口类型
interface SampleHistoryItem {
	id: number;
	type: number;                    // 1:寄样 2:收样
	customer_or_Supplier: number;    // 1:客户 2:供应商
	waybill_Number: string;
	express_Company: number;
	sample_Date: string;
	customer_ID: number;
	abbreviation: string;
	company_ID: number;
	salesperson_ID: number;
	payment_Method: number;
	paid_Express_Fee: number;
	isDelete: number;
	createBy: string;
	createTime: string;
	updateTime: string;
	remark: string | null;
}
// 获取供应商寄样历史
const loadSupplierSendSampleHistory = async (supplierId) => {
	try {
		const response = await request({
			url: 'ProductSample/GetSupplierHistoryBySupplierID/GetHistory',
			method: 'GET',
			params: { SupplierID: supplierId }
		});

		if (response && response.code === 200) {
			// 转换数据
			supperinfoSendSampleData.value = response.data.map((item: SampleHistoryItem) => ({
				type: item.type === 1 ? '寄样' : '收样',
				waybillNumber: item.waybill_Number,
				expressCompany: state.optionss.hr_express_delivery_company.find(
					company => company.dictValue === item.express_Company.toString()
				)?.dictLabel || '',
				sampleDate: item.sample_Date ? item.sample_Date.split(' ')[0] : '',
				abbreviation: item.abbreviation,
				companyName: state.optionss.hr_ourcompany.find(
					company => company.dictValue === item.company_ID.toString()
				)?.dictLabel || '',
				paymentMethod: state.optionss.hr_express_payment_method.find(
					method => method.dictValue === item.payment_Method.toString()
				)?.dictLabel || '',
				paidExpressFee: item.paid_Express_Fee?.toFixed(2) || '0.00',
				remark: item.remark || ''
			}));
		} else {
			ElMessage.error(response?.msg || '获取收寄样历史失败');
		}
	} catch (error) {
		console.error('获取收寄样历史失败:', error);
		ElMessage.error('获取收寄样历史失败，请稍后重试');
	}
};
// 获取往来邮件数据的方法
const getEmailHistoryList = (emailAddress) => {
	EmailHistoryData.value = [];
	request({
		url: 'Email/GetEmailContactRecords/GetEmailList',
		method: 'GET',
		params: {
			EmailAddress: emailAddress
		}
	}).then(response => {
		if (response.data.length > 0) {
			response.data.forEach(item => {
				EmailHistoryData.value.push({
					EmailDate: formatDateTime(item.emaildate),
					Contact: item.toEmail,
					OurPersonnel: item.fromEmail,
					ContactDetails: item.emailsubject
				});
			});
		} else {
			EmailHistoryData.value = [];
		}
	}).catch(error => {
		console.error('获取往来邮件失败:', error);
		EmailHistoryData.value = [];
	});
}
//日期格式化函数
const formatDateTime = (dateTimeStr) => {
	if (!dateTimeStr) return '';
	const date = new Date(dateTimeStr);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	const hours = String(date.getHours()).padStart(2, '0');
	const minutes = String(date.getMinutes()).padStart(2, '0');
	const seconds = String(date.getSeconds()).padStart(2, '0');
	return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

const isDraft = ref(0)
const SubmitSupperinfo = () => {
	if (SelctedSupplierId.value == '') {
		isDraft.value = 0;
		SaveSupperinfo();
	} else {
		ElMessageBox.confirm('确定提交供应商信息？提交后供应商信息将不再是草稿状态', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			request({
				url: 'Supplierinfo/UpdateSupplierInfoIsDraft/UpdateSupplierInfoIsDraft',
				method: 'GET',
				params: {
					supplierId: SelctedSupplierId.value
				}
			}).then(response => {
				if (response && response.code === 200) {
					ElMessage({
						message: response.msg || '供应商信息提交成功',
						type: 'success'
					})
					GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
					AddSupperDialog.value = false;
				} else {
					ElMessage.error(response?.msg || '供应商信息提交失败');
				}
			}).catch(error => {
				console.error('提交供应商信息出错！😔错误内容：', error);
				ElMessage.error('提交供应商信息失败，请稍后重试');
			})
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消提交'
			});
		});
	}
}
const DeleteSupplier = (row) => {
	ElMessageBox.confirm('确定要删除该供应商吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'Supplierinfo/DeleteSupplierinfo/Delete',
			method: 'post',
			data: { SupplierID: row.id }
		}).then(response => {
			const res = response;
			if (res && res.code == 200) {
				ElMessage({
					message: '删除成功',
					type: 'success'
				});
				GetSupplierInfoList(SupplierInfoTableDatacurrentPage.value, SupplierInfoTableDatapageSize.value);
			} else {
				ElMessage.error('删除失败');
			}
		}).catch(() => {
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage.info('已取消删除');
	});
};
</script>
