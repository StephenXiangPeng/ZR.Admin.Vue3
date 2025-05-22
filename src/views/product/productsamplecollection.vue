<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="handleCreate">创建收样/寄样</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<div style="width: 100%; margin-top: 30px;">
			<el-select v-model="SearchwaybillNumber" filterable clearable placeholder="选择运单号" style="width: 15%">
				<el-option v-for="item in optionss.sql_waybill_number" :key="item.dictCode" :label="item.dictLabel"
					:value="item.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SearchCustomer" filterable clearable placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="item in optionss.sql_hr_customer_abbreviation" :key="item.dictCode"
					:label="item.dictLabel" :value="item.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="SearchSalesperson" filterable clearable placeholder="选择业务员" style="width: 15%">
				<el-option v-for="item in optionss.sql_all_user" :key="item.dictCode" :label="item.dictLabel"
					:value="item.dictValue" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="SearchStartDate" type="date" placeholder="请选择开始日期"
				style="width: 15%" />&nbsp;至&nbsp;
			<el-date-picker v-model="SearchEndDate" type="date" placeholder="请选择结束日期" style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 5px;">
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchHandleSearch">查询</el-button>
				<el-button @click="SearchHandleReset">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;收样/寄样信息表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="ProductSampleTableData">

			<el-table-column prop="type" label="寄样/收样" width="150">
				<template #default="scope">
					<span>{{ scope.row.type }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="customer_or_Supplier" label="客户/供应商" width="150"></el-table-column>
			<el-table-column prop="customer_ID" label="寄样对象" width="150"></el-table-column>
			<el-table-column prop="waybill_Number" label="运单号" width="150"></el-table-column>
			<el-table-column prop="express_Company" label="快件公司" width="150"></el-table-column>
			<el-table-column prop="sample_Date" label="登记日期" width="150"></el-table-column>
			<el-table-column prop="payment_Method" label="付费方式" width="150"></el-table-column>
			<el-table-column prop="company_ID" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="paid_Express_Fee" label="已付快件费" width="150"></el-table-column>
			<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small" @click="handleView(scope.row.id)">查看/编辑</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />

		<el-dialog v-model="dialogVisible" :title="isCreateMode ? '创建收样/寄样' : '查看/编辑收样/寄样'"
			:close-on-click-modal="false" style="width: 70%;" @closed="handleDialogClosed">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="CreateDialogform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="请选择">
							<el-radio-group v-model="radioValue" :disabled="!isEditable">
								<el-radio label="1" size="large" border>寄样</el-radio>
								<el-radio label="2" size="large" border>收样</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户/供应商">
							<el-select v-model="CreateDialogform.recipienttypeexamples" placeholder="请选择供应商或客户"
								style="width: 300px;" @change="handleRecipientTypeChange" :disabled="!isEditable">
								<el-option v-for="item in optionss.hr_recipient_type_examples" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="运单号">
							<el-input v-model="CreateDialogform.waybillNumber" style="width: 300px;"
								:disabled="!isEditable"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="快递公司">
							<el-select v-model="CreateDialogform.expressCompany" placeholder="请选择快递公司"
								style="width: 300px;" :disabled="!isEditable">
								<el-option v-for="item in optionss.hr_express_delivery_company" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="寄样日期">
							<el-date-picker v-model="CreateDialogform.sampleDate" type="date" placeholder="请选择"
								style="width: 300px;" :disabled="!isEditable"></el-date-picker>
						</el-form-item>
						<el-form-item v-else-if="radioValue === '2'" label="收样日期">
							<el-date-picker v-model="CreateDialogform.sampleDate" type="date" placeholder="请选择"
								style="width: 300px;" :disabled="!isEditable"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item :label="sampleObjectLabel">
							<el-select v-model="CreateDialogform.sampleObject" placeholder="请选择" style="width: 300px;"
								:disabled="!isEditable">
								<el-option v-for="item in getObjectOptions" :key="item.dictCode" :label="item.dictLabel"
									:value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="对方简称">
							<el-input v-model="CreateDialogform.partnerAbbreviation" style="width: 300px;"
								:disabled="!isEditable"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="CreateDialogform.ourCompany" placeholder="请选择" style="width: 300px;"
								:disabled="!isEditable">
								<el-option v-for="item in optionss.hr_ourcompany" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="业务员">
							<el-select v-model="CreateDialogform.salesperson" placeholder="请选择" style="width: 300px;"
								:disabled="!isEditable">
								<el-option v-for="item in optionss.sql_all_user" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="付费方式">
							<el-select v-model="CreateDialogform.paymentMethod" placeholder="请选择快递付费方式"
								style="width: 300px;" :disabled="!isEditable">
								<el-option v-for="item in optionss.hr_express_payment_method" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付快递费">
							<el-input v-model="CreateDialogform.paidExpressCost" style="width: 300px;"
								:disabled="!isExpressFeeRequired || !isEditable"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">样品信息</span>
				<el-divider></el-divider>
				<el-button class="mt-4" type="primary" style="margin-bottom: 10px;" @click="addSampleRow"
					:disabled="!isEditable">添加样品</el-button>
				<el-table :data="SampleProductData" style="width: 100%;margin-bottom: 15px;" max-height="550">
					<el-table-column prop="productNumber" label="样品编号" width="250">
						<template #default="{ row, $index }">
							<el-select v-model="row.productNumber" filterable clearable allow-create
								:default-first-option="true" placeholder="请选择或输入样品编号" style="width: 100%"
								:disabled="!isEditable"
								@change="(value) => handleProductNumberChange(value, $index, $event)"
								@clear="handleClearProductNumber($index)">
								<el-option v-for="item in optionss.sql_product_name" :key="item.dictCode"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="productChineseName" label="中文品名" width="170">
						<template #default="{ row }">
							<el-input v-model="row.productChineseName" :disabled="!isEditable"></el-input>
						</template>
					</el-table-column>
					<el-table-column prop="SampleQuantity" label="寄样数量" width="100">
						<template #default="{ row }">
							<el-input v-model="row.SampleQuantity" :disabled="!isEditable"></el-input>
						</template>
					</el-table-column>
					<el-table-column prop="PricingAmount" label="计价金额" width="100">
						<template #default="{ row }">
							<el-input v-model="row.PricingAmount" :disabled="!isEditable"></el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subproductImage" label="产品图片" width="200" align="center">
						<template #default="scope">
							<el-upload :id="`upload-${scope.$index}`" ref="uploadRefs" :auto-upload="false"
								:show-file-list="true" :on-change="(file) => handleImageSelect(file, scope.$index)"
								:on-remove="(file) => handleImageRemove(file, scope.$index)" :limit="3" accept="image/*"
								multiple list-type="text" :file-list="scope.row.subproductImages || []">
								<el-button
									v-if="!isCreateMode && (!scope.row.subproductImages || scope.row.subproductImages.length < 3)"
									type="primary" icon="Plus" size="small" :disabled="!isEditable">
									选择图片
								</el-button>
								<template #tip>
									<div v-if="scope.row.subproductImages && scope.row.subproductImages.length >= 3"
										class="el-upload__tip">
										已达到最大图片数量
									</div>
								</template>
							</el-upload>
						</template>
					</el-table-column>
					<el-table-column prop="previewImages" label="图片预览" width="300" align="center">
						<template #default="scope">
							<div class="image-preview-container"
								v-if="scope.row.subproductImages && scope.row.subproductImages.length">
								<el-button type="text" :icon="ArrowLeft" @click="prevImage(scope.$index)"
									:disabled="scope.row.currentImageIndex === 0" />
								<el-image style="width: 150px; height: 150px;"
									:src="scope.row.subproductImages[scope.row.currentImageIndex || 0].url"
									:preview-src-list="scope.row.subproductImages.map(img => img.url)"
									:initial-index="scope.row.currentImageIndex || 0" fit="cover" preview-teleported
									@click="openPreview(scope.$index)" />
								<el-button type="text" :icon="ArrowRight" @click="nextImage(scope.$index)"
									:disabled="scope.row.currentImageIndex === scope.row.subproductImages.length - 1" />
								<el-button v-if="!isCreateMode" type="danger" icon="Delete"
									@click="deleteCurrentImage(scope.$index)" size="small"
									:disabled="!isEditable">删除</el-button>
							</div>
							<span v-else>暂无图片</span>
						</template>
					</el-table-column>
					<el-table-column label="操作" width="100" fixed="right">
						<template #default="{ $index }">
							<el-button type="danger" size="small" @click="removeSampleRow($index)"
								:disabled="!isEditable">删除</el-button>
						</template>
					</el-table-column>
				</el-table>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<!-- 查看模式：显示编辑按钮 -->
					<el-button v-if="isEditBtnShow" type="primary" @click="handleEdit">编辑</el-button>
					<!-- 新建模式：显示保存按钮 -->
					<el-button v-if="isSaveDraftBtnShow" type="warning" :loading="loading"
						@click="handleSaveDraft">保存草稿</el-button>
					<!-- 编辑模式：显示保存按钮 -->
					<el-button v-if="isEditSubmitBtnShow" type="success" :loading="loading"
						@click="handleEditSave">提交</el-button>
					<!-- 编辑模式：显示提交按钮 -->
					<el-button v-if="isSubmitBtnShow" type="success" :loading="loading"
						@click="handleSave">提交</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { createApp, ref, reactive, onMounted, getCurrentInstance, computed } from 'vue'
import { useRoute } from 'vue-router'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer,
	ElMessageBox, UploadUserFile, ElMessage, UploadFile, FormInstance, FormRules
} from 'element-plus'
import { ArrowLeft, ArrowRight } from '@element-plus/icons-vue'
import request from '@/utils/request';

// 获取路由实例
const route = useRoute()

const isEditBtnShow = ref(false);//编辑按钮
const isSaveDraftBtnShow = ref(false);//保存草稿按钮
const isEditSubmitBtnShow = ref(false);//编辑提交按钮
const isSubmitBtnShow = ref(false);//提交按钮

//查询条件
const SearchwaybillNumber = ref('');
const SearchCustomer = ref('');
const SearchSalesperson = ref('');
const SearchStartDate = ref('');
const SearchEndDate = ref('');
const ProductSampleTableData = ref([]);
//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetProductSampleList(start, end);
};
// 重置搜索条件
const SearchHandleReset = () => {
	SearchwaybillNumber.value = null;
	SearchCustomer.value = null;
	SearchSalesperson.value = null;
	SearchStartDate.value = null;
	SearchEndDate.value = null;
	GetProductSampleList(currentPage.value, pageSize.value);
};
//查询
const SearchHandleSearch = () => {
	GetProductSampleList(currentPage.value, pageSize.value);
}
//表格数据
const GetProductSampleList = (start, end) => {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'ProductSample/GetProductSampleList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				Waybill_Number: SearchwaybillNumber.value,
				Customer: SearchCustomer.value,
				Salesperson: SearchSalesperson.value,
				StartDate: SearchStartDate.value,
				EndDate: SearchEndDate.value,
			}
		}).then(response => {
			if (response.code === 200) {
				// 更新表格数据
				ProductSampleTableData.value = response.data.result;
				ProductSampleTableData.value.forEach(item => {
					item.type = item.type === 1 ? '寄样' : '收样';
					item.customer_or_Supplier = item.customer_or_Supplier === 1 ? '客户' : '供应商';
					if (item.customer_ID != 0 && item.customer_ID != null) {
						item.customer_ID = optionss.value.sql_hr_customer_abbreviation.find(customer => customer.dictValue === item.customer_ID.toString())?.dictLabel || '未知';
					}
					if (item.express_Company != 0 && item.express_Company != null) {
						item.express_Company = optionss.value.hr_express_delivery_company.find(company => company.dictValue === item.express_Company.toString())?.dictLabel || '未知';
					}
					item.paid_Express_Fee = item.paid_Express_Fee.toFixed(2);
					if (item.payment_Method != 0 && item.payment_Method != null) {
						item.payment_Method = optionss.value.hr_express_payment_method.find(method => method.dictValue === item.payment_Method.toString())?.dictLabel || '未知';
					}
					if (item.company_ID != 0 && item.company_ID != null) {
						item.company_ID = optionss.value.hr_ourcompany.find(company => company.dictValue === item.company_ID.toString())?.dictLabel || '未知';
					}
					item.sample_Date = formatDate(item.sample_Date);
				});
				// 更新分页信息
				totalItems.value = response.data.totalNum;
				// 解决 Promise
				resolve(response.data);
			} else {
				ElMessage.error(response.msg || '获取数据失败');
				reject(new Error(response.msg));
			}
		}).catch(error => {
			ElMessage.error('获取数据失败');
			reject(error);
		});
	})
}

const formatDate = (dateString) => {
	if (!dateString) return '';
	return dateString.split(' ')[0]; // Returns just the date part
};

// 1. 修改状态变量
const isCreateMode = ref(false);    // 是否是新建模式
const isEditable = ref(false);      // 是否可编辑
const dialogVisible = ref(false);   // 对话框显示状态（原 CreateDialog）

// 2. 修改处理函数
// 新建按钮点击事件
const handleCreate = () => {
	isCreateMode.value = true;      // 设置为新建模式
	isEditable.value = true;        // 允许编辑
	isEditBtnShow.value = false;
	isSaveDraftBtnShow.value = true;
	isEditSubmitBtnShow.value = false;
	isSubmitBtnShow.value = true;
	dialogVisible.value = true;     // 打开对话框
};

// 查看/编辑按钮点击事件
const handleView = async (id) => {
	currentEditId.value = id;
	isCreateMode.value = false;     // 设置为查看模式
	isEditable.value = false;       // 初始禁用编辑
	isEditBtnShow.value = true;
	isSaveDraftBtnShow.value = false;
	isEditSubmitBtnShow.value = false;
	isSubmitBtnShow.value = false;
	try {
		const response = await request({
			url: 'ProductSample/GetProductSampleDetails/GetDetails',
			method: 'GET',
			params: { ID: id }
		});

		if (response.code === 200) {
			const { sample, details } = response.data;
			// 填充主表数据
			CreateDialogform.value = {
				recipienttypeexamples: (sample?.customer_or_Supplier ?? 1).toString(),
				waybillNumber: sample?.waybill_Number ?? '',
				expressCompany: (sample?.express_Company ?? '').toString(),
				sampleDate: sample?.sample_Date ? sample.sample_Date.split(' ')[0] : '',
				sampleObject: (sample?.customer_ID ?? '').toString(),
				partnerAbbreviation: sample?.abbreviation ?? '',
				ourCompany: (sample?.company_ID ?? '').toString(),
				salesperson: (sample?.salesperson_ID ?? '').toString(),
				paymentMethod: (sample?.payment_Method ?? '').toString(),
				paidExpressCost: (sample?.paid_Express_Fee ?? 0).toString(),
				photos: sample?.photos ?? []
			};

			// 设置寄样/收样类型
			radioValue.value = (sample?.type ?? 1).toString();

			// 填充样品明细数据
			SampleProductData.value = (details || []).map(detail => {
				const images = detail?.product_Image_URL ?
					detail.product_Image_URL.split(',').map(url => ({
						url: url.trim(),
						name: url.split('/').pop()
					})) : [];

				return {
					id: detail?.ID ?? 0,
					productNumber: detail?.sample_Code ?? '',
					productChineseName: detail?.chinese_Name ?? '',
					SampleQuantity: detail?.sample_Quantity ?? '',
					PricingAmount: detail?.valuation_Amount ?? '',
					subproductImages: images,
					currentImageIndex: 0
				};
			});

			dialogVisible.value = true;
		} else {
			ElMessage.error(response.msg || '获取详情失败');
		}
	} catch (error) {
		console.error('获取详情失败:', error);
		ElMessage.error('获取详情失败');
	}
};

// 编辑按钮点击事件
const handleEdit = () => {
	isEditable.value = true;        // 切换到可编辑状态
	isEditBtnShow.value = false;
	isSaveDraftBtnShow.value = true;
	isEditSubmitBtnShow.value = true;
	isSubmitBtnShow.value = false;
};

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		hr_recipient_type_examples: [],//寄收样对象类型
		sql_all_user: [],//业务员
		sql_hr_customer_abbreviation: [],//客户简称
		sql_supplier_info: [],//供应商信息
		hr_ourcompany: [],//我方公司
		hr_express_delivery_company: [],//快递公司
		hr_express_payment_method: [],//快递付费方式
		sql_product_name: [],//产品名称
		sql_waybill_number: []//运单号
	}
})
const { optionss } = toRefs(state)
var dictParams = [
	{ dictType: 'hr_recipient_type_examples' },
	{ dictType: 'sql_all_user' },
	{ dictType: 'sql_hr_customer_abbreviation' },
	{ dictType: 'sql_supplier_info' },
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'hr_express_delivery_company' },
	{ dictType: 'hr_express_payment_method' },
	{ dictType: 'sql_product_name' },
	{ dictType: 'sql_waybill_number' }
]

async function fetchDataAndExecute() {
	try {
		const response = await proxy.getDicts(dictParams);
		response.data.forEach((element) => {
			state.optionss[element.dictType] = element.list;
		});
		GetProductSampleList(currentPage.value, pageSize.value);
	} catch (error) {
		console.error('Failed to fetch data:', error);
	}
}
fetchDataAndExecute();
/*动态下拉框end*/


// 处理寄收样对象类型变化
const handleRecipientTypeChange = () => {
	CreateDialogform.value.sampleObject = ''; // 清空选择的寄样对象
};
const sampleObjectLabel = computed(() => {
	// 根据recipienttypeexamples的值来决定使用哪个数据源
	// 假设2代表供应商，1代表客户
	if (CreateDialogform.value.recipienttypeexamples === '2') {
		return '供应商';
	} else if (CreateDialogform.value.recipienttypeexamples === '1') {
		return '客户';
	}
});
//计算属性来动态切换数据源
const getObjectOptions = computed(() => {
	// 根据recipienttypeexamples的值来决定使用哪个数据源
	// 假设2代表供应商，1代表客户
	if (CreateDialogform.value.recipienttypeexamples === '2') {
		return optionss.value.sql_supplier_info;
	} else if (CreateDialogform.value.recipienttypeexamples === '1') {
		return optionss.value.sql_hr_customer_abbreviation;
	}
	return []; // 默认返回空数组
});

//样品产品数据
const SampleProductData = ref([]);


// 删除行
const removeSampleRow = (index: number) => {
	SampleProductData.value.splice(index, 1);
};

// 修改图片选择处理函数
const handleImageSelect = (file, index) => {
	if (!file) {
		console.error('No file selected');
		ElMessage.error('请选择图片文件');
		return;
	}

	const reader = new FileReader();
	reader.onload = (e) => {
		if (!SampleProductData.value[index].subproductImages) {
			SampleProductData.value[index].subproductImages = [];
			SampleProductData.value[index].currentImageIndex = 0;
		}

		if (SampleProductData.value[index].subproductImages.length < 3) {
			const newImage = {
				name: file.name,
				url: e.target.result,
				raw: file.raw, // 保存原始文件对象
				isChanged: true // 标记为新添加的图片
			};
			SampleProductData.value[index].subproductImages.push(newImage);
			SampleProductData.value[index].currentImageIndex = SampleProductData.value[index].subproductImages.length - 1;
			// 强制更新视图
			SampleProductData.value = [...SampleProductData.value];
		} else {
			ElMessage.warning('最多只能上传3张图片');
		}
	};
	reader.readAsDataURL(file.raw);
};

const handleImageRemove = (file, index) => {
	const images = SampleProductData.value[index].subproductImages;
	const fileIndex = images.findIndex(img => img.name === file.name);
	if (fileIndex > -1) {
		images.splice(fileIndex, 1);
		if (SampleProductData.value[index].currentImageIndex >= images.length) {
			SampleProductData.value[index].currentImageIndex = Math.max(0, images.length - 1);
		}
		// 强制更新视图
		SampleProductData.value = [...SampleProductData.value];
	}
};

const prevImage = (index) => {
	const row = SampleProductData.value[index];
	if (row.currentImageIndex > 0) {
		row.currentImageIndex--;
		// 强制更新视图
		SampleProductData.value = [...SampleProductData.value];
	}
}

const nextImage = (index) => {
	const row = SampleProductData.value[index];
	if (row.currentImageIndex < row.subproductImages.length - 1) {
		row.currentImageIndex++;
		// 强制更新视图
		SampleProductData.value = [...SampleProductData.value];
	}
}

const openPreview = (index) => {
	const subProduct = SampleProductData.value[index];
	// 这里可以添加打开预览的逻辑，如果需要的话
	// 例如，可以使用 Element Plus 的 ElImageViewer 组件
};

const deleteCurrentImage = (index) => {
	const subProduct = SampleProductData.value[index];
	if (subProduct.subproductImages && subProduct.subproductImages.length > 0) {
		// 删除当前显示的图片
		const deletedImage = subProduct.subproductImages.splice(subProduct.currentImageIndex, 1)[0];

		// 更新 currentImageIndex
		if (subProduct.currentImageIndex >= subProduct.subproductImages.length) {
			subProduct.currentImageIndex = Math.max(0, subProduct.subproductImages.length - 1);
		}
		subProduct.subproductImages = [...subProduct.subproductImages];
		// 强制更新视图
		SampleProductData.value = [...SampleProductData.value];
	}
}

const CreateDialogform = ref({
	recipienttypeexamples: '1',
	waybillNumber: '',
	expressCompany: '',
	sampleDate: '',
	sampleObject: '',
	partnerAbbreviation: '',
	ourCompany: '',
	salesperson: '',
	paymentMethod: '',
	paidExpressCost: '',
	photos: [],
});

const radioValue = ref('1');
const customerinfoselect = ref('');
const loading = ref(false);
// 添加上传URL常量
const UploadUrl = 'Common/UploadFile'  // 上传图片地址

// 辅助函数：上传产品图片
const uploadProductPhoto = async (file) => {
	const formData = new FormData();
	formData.append('FileName', file.name || '未命名文件');
	formData.append('FileDir', 'ProductSample/SamplePhoto');
	formData.append('FileNameType', '1');
	formData.append('File', file.raw || file);
	formData.append('storeType', '1');
	const response = await request.postForm(UploadUrl, formData);
	return response;
};
//确定保存方法
const handleSave = async () => {
	if (loading.value) return;
	// 1. 构建基础请求数据用于验证
	const requestData = {
		type: parseInt(radioValue.value),
		customer_or_Supplier: parseInt(CreateDialogform.value.recipienttypeexamples),
		waybill_Number: CreateDialogform.value.waybillNumber,
		express_Company: parseInt(CreateDialogform.value.expressCompany),
		sample_Date: CreateDialogform.value.sampleDate,
		customer_ID: parseInt(CreateDialogform.value.sampleObject),
		abbreviation: CreateDialogform.value.partnerAbbreviation,
		company_ID: parseInt(CreateDialogform.value.ourCompany),
		salesperson_ID: parseInt(CreateDialogform.value.salesperson),
		payment_Method: parseInt(CreateDialogform.value.paymentMethod),
		paid_Express_Fee: parseFloat(CreateDialogform.value.paidExpressCost) || 0,
		isDelete: 0,
		isDraft: 0,
		details: SampleProductData.value.map(item => ({
			sample_Code: item.productNumber,
			chinese_Name: item.productChineseName,
			sample_Quantity: parseInt(item.SampleQuantity) || 0,
			valuation_Amount: parseFloat(item.PricingAmount) || 0,
			isDelete: 0,
			remark: ''
		}))
	};
	// 2. 表单验证
	if (!validateForm(requestData)) {
		return;
	}
	loading.value = true;
	try {
		// 3. 处理图片上传
		for (const row of SampleProductData.value) {
			if (row.subproductImages) {
				const newImageUrls = [];
				for (const img of row.subproductImages) {
					if (img.isChanged) { // 只上传新添加的图片
						try {
							const uploadResult = await uploadProductPhoto(img);
							if (uploadResult.code === 200) {
								newImageUrls.push(uploadResult.data.url);
							} else {
								throw new Error('图片上传失败');
							}
						} catch (error) {
							ElMessage.error(`图片 ${img.name} 上传失败`);
							return;
						}
					} else {
						newImageUrls.push(img.url); // 保留已有的图片URL
					}
				}
				row.finalImageUrls = newImageUrls.join(',');
			}
		}
		// 4. 更新请求数据中的图片URL
		requestData.details = requestData.details.map((detail, index) => ({
			...detail,
			product_Image_URL: SampleProductData.value[index].finalImageUrls || ''
		}));
		// 5. 发送保存请求
		const response = await request.post('ProductSample/AddProductSample/Add', requestData);
		if (response.code === 200) {
			ElMessage.success(response.msg || '保存成功');
			dialogVisible.value = false;
			resetForm();
			// 6. 刷新列表数据
			await GetProductSampleList(currentPage.value, pageSize.value);
		} else {
			ElMessage.error(response.msg || '保存失败');
		}
	} catch (error) {
		console.error('保存失败:', error);
		ElMessage.error('保存失败，请稍后重试');
	} finally {
		loading.value = false;
	}
};

// 计算属性：是否需要填写快递费
const isExpressFeeRequired = computed(() => {
	return (radioValue.value === '1' && CreateDialogform.value.paymentMethod === '1') || // 寄样且预付
		(radioValue.value === '2' && CreateDialogform.value.paymentMethod === '2');   // 收样且到付
});
// 表单验证
const validateForm = (data) => {
	if (!data.express_Company) {
		ElMessage.warning('请选择快递公司');
		return false;
	}
	if (!data.sample_Date) {
		ElMessage.warning('请选择日期');
		return false;
	}
	if (!data.customer_ID) {
		ElMessage.warning('请选择客户/供应商');
		return false;
	}
	if (!data.company_ID) {
		ElMessage.warning('请选择我方公司');
		return false;
	}
	if (!data.salesperson_ID) {
		ElMessage.warning('请选择业务员');
		return false;
	}

	// 寄样和收样的快递费验证
	if (data.type === 1 && data.payment_Method === 1 && !data.paid_Express_Fee) { // 寄样且预付
		ElMessage.warning('选择预付时，已付快递费为必填项');
		return false;
	}
	if (data.type === 2 && data.payment_Method === 2 && !data.paid_Express_Fee) { // 收样且到付
		ElMessage.warning('选择到付时，已付快递费为必填项');
		return false;
	}

	if (data.details.length === 0) {
		ElMessage.warning('请添加至少一个样品');
		return false;
	}

	// 验证样品信息
	for (let i = 0; i < data.details.length; i++) {
		const detail = data.details[i];
		if (!detail.chinese_Name) {
			ElMessage.warning(`第${i + 1}行中文品名不能为空`);
			return false;
		}
		if (!detail.sample_Quantity) {
			ElMessage.warning(`第${i + 1}行寄样数量不能为空`);
			return false;
		}
	}

	return true;
};
// 重置表单
const resetForm = () => {
	currentEditId.value = null;
	// 重置基本表单数据
	CreateDialogform.value = {
		recipienttypeexamples: '1',
		waybillNumber: '',
		expressCompany: '',
		sampleDate: '',
		sampleObject: '',
		partnerAbbreviation: '',
		ourCompany: '',
		salesperson: '',
		paymentMethod: '',
		paidExpressCost: '',
		photos: [],
	};
	// 重置寄样/收样选择
	radioValue.value = '1';
	// 清空样品列表数据
	SampleProductData.value = [];
	// 重置编辑状态
	isEditable.value = false;
};
// 对话框关闭时的处理函数
const handleDialogClosed = () => {
	resetForm();
	isCreateMode.value = false;
	isEditable.value = false;
};

// 编辑保存方法
// ... existing code ...
const handleEditSave = async () => {
	// 1. 构建请求数据
	const requestData = {
		ID: currentEditId.value,
		type: parseInt(radioValue.value),
		customer_or_Supplier: parseInt(CreateDialogform.value.recipienttypeexamples),
		waybill_Number: CreateDialogform.value.waybillNumber,
		express_Company: parseInt(CreateDialogform.value.expressCompany),
		sample_Date: CreateDialogform.value.sampleDate,
		customer_ID: parseInt(CreateDialogform.value.sampleObject),
		abbreviation: CreateDialogform.value.partnerAbbreviation,
		company_ID: parseInt(CreateDialogform.value.ourCompany),
		salesperson_ID: parseInt(CreateDialogform.value.salesperson),
		payment_Method: parseInt(CreateDialogform.value.paymentMethod),
		paid_Express_Fee: parseFloat(CreateDialogform.value.paidExpressCost) || 0,
		isDelete: 0,
		isDraft: 0,
		details: SampleProductData.value.map(item => ({
			ID: item.id || 0,
			sample_Code: item.productNumber,
			chinese_Name: item.productChineseName,
			sample_Quantity: parseInt(item.SampleQuantity) || 0,
			valuation_Amount: parseFloat(item.PricingAmount) || 0,
			isDelete: 0,
			remark: ''
		}))
	};
	// 2. 表单验证
	if (!validateForm(requestData)) {
		// 验证失败时保持编辑状态
		isEditable.value = true;
		return;
	}
	loading.value = true;
	try {
		// 3. 处理图片上传
		for (const row of SampleProductData.value) {
			if (row.subproductImages) {
				const newImageUrls = [];
				for (const img of row.subproductImages) {
					if (img.isChanged) {
						try {
							const uploadResult = await uploadProductPhoto(img);
							if (uploadResult.code === 200) {
								newImageUrls.push(uploadResult.data.url);
							} else {
								throw new Error('图片上传失败');
							}
						} catch (error) {
							ElMessage.error(`图片 ${img.name} 上传失败`);
							isEditable.value = false; // 保持编辑状态
							loading.value = false;
							return;
						}
					} else {
						newImageUrls.push(img.url);
					}
				}
				row.finalImageUrls = newImageUrls.join(',');
			}
		}
		// 4. 更新请求数据中的图片URL
		requestData.details = requestData.details.map((detail, index) => ({
			...detail,
			product_Image_URL: SampleProductData.value[index].finalImageUrls || ''
		}));
		// 5. 发送编辑请求
		const response = await request.post('ProductSample/EditProductSample/Edit', requestData);
		if (response.code === 200) {
			ElMessage.success('提交成功');
			dialogVisible.value = false;
			resetForm();
			// 6. 刷新列表数据
			await GetProductSampleList(currentPage.value, pageSize.value);
			isEditable.value = false; // 成功后禁用编辑
		} else {
			ElMessage.error('提交失败');
			isEditable.value = false; // 失败时保持编辑状态
		}
	} catch (error) {
		console.error('提交失败:', error);
		ElMessage.error('提交失败，请稍后重试');
		isEditable.value = false; // 错误时保持编辑状态
	} finally {
		loading.value = false;
	}
};

// 添加存储当前编辑记录ID的变量
const currentEditId = ref(null);

// 处理样品编号变化
// 处理样品编号变化
const handleProductNumberChange = async (value, rowIndex, event) => {
	if (rowIndex === undefined) {
		console.error('Row index is undefined!');
		return;
	}

	// 检查是否是从选项中选择的值
	const isSelectedFromOptions = optionss.value.sql_product_name.some(
		option => option.dictValue === value
	);

	// 如果不是从选项中选择的（即手动输入）或值为空，只更新样品编号
	if (!isSelectedFromOptions || !value) {
		SampleProductData.value.splice(rowIndex, 1, {
			...SampleProductData.value[rowIndex],
			productNumber: value || ''
		});
		return;
	}

	// 如果是从选择框选择的值，则获取产品信息
	try {
		const response = await request({
			url: 'ProductInformation/GetProductInfo/GetProductInfo',
			method: 'GET',
			params: { ID: value }
		});

		if (response.code === 200 && response.data) {
			// 保存当前行的数量和价格信息
			const currentQuantity = SampleProductData.value[rowIndex]?.SampleQuantity || '';
			const currentPrice = SampleProductData.value[rowIndex]?.PricingAmount || '';

			// 构建图片数组
			const images = response.data.productPhotoPath ? [{
				url: response.data.productPhotoPath,
				name: response.data.productPhotoPath.split('/').pop(),
				isChanged: false,
				raw: null
			}] : [];

			// 更新行数据
			SampleProductData.value.splice(rowIndex, 1, {
				...SampleProductData.value[rowIndex],
				productNumber: value,
				productChineseName: response.data.chineseProductName || '',
				SampleQuantity: currentQuantity,
				PricingAmount: currentPrice,
				subproductImages: images,
				currentImageIndex: 0
			});
		} else {
			ElMessage.warning('获取产品信息失败');
		}
	} catch (error) {
		console.error('获取产品信息失败:', error);
		ElMessage.error('获取产品信息失败，请稍后重试');
	}
};

// 处理清空样品编号
const handleClearProductNumber = (rowIndex) => {
	if (rowIndex === undefined) return;

	// 清空当前行的相关数据
	SampleProductData.value.splice(rowIndex, 1, {
		...SampleProductData.value[rowIndex],
		productNumber: '',
		productChineseName: '',
		subproductImages: [], // 清空图片
		currentImageIndex: 0
	});
};

// 添加样品行时的初始化
const addSampleRow = () => {
	SampleProductData.value.push({
		productNumber: '',           // 样品编号
		productChineseName: '',      // 中文品名
		SampleQuantity: '',          // 样品数量
		PricingAmount: '',          // 计价金额
		subproductImages: [],       // 图片数组
		currentImageIndex: 0,       // 当前图片索引
		// 添加其他可能需要的字段
	});
};

const handleSaveDraft = async () => {
	if (loading.value) return;
	// 1. 构建请求数据
	const requestData = {
		ID: currentEditId.value || 0,  // 如果是编辑模式，使用当前ID
		type: parseInt(radioValue.value),
		customer_or_Supplier: parseInt(CreateDialogform.value.recipienttypeexamples),
		waybill_Number: CreateDialogform.value.waybillNumber,
		express_Company: parseInt(CreateDialogform.value.expressCompany) || null,
		sample_Date: CreateDialogform.value.sampleDate,
		customer_ID: parseInt(CreateDialogform.value.sampleObject) || null,
		abbreviation: CreateDialogform.value.partnerAbbreviation,
		company_ID: parseInt(CreateDialogform.value.ourCompany) || null,
		salesperson_ID: parseInt(CreateDialogform.value.salesperson) || null,
		payment_Method: parseInt(CreateDialogform.value.paymentMethod) || null,
		paid_Express_Fee: parseFloat(CreateDialogform.value.paidExpressCost) || 0,
		isDelete: 0,
		isDraft: 1,  // 标记为草稿
		details: SampleProductData.value.map(item => ({
			ID: item.id || 0,
			sample_Code: item.productNumber,
			chinese_Name: item.productChineseName,
			sample_Quantity: parseInt(item.SampleQuantity) || 0,
			valuation_Amount: parseFloat(item.PricingAmount) || 0,
			isDelete: 0,
			remark: ''
		}))
	};

	loading.value = true;
	try {
		// 处理图片上传
		for (const row of SampleProductData.value) {
			if (row.subproductImages) {
				const newImageUrls = [];
				for (const img of row.subproductImages) {
					if (img.isChanged) { // 只上传新添加的图片
						try {
							const uploadResult = await uploadProductPhoto(img);
							if (uploadResult.code === 200) {
								newImageUrls.push(uploadResult.data.url);
							} else {
								throw new Error('图片上传失败');
							}
						} catch (error) {
							ElMessage.error(`图片 ${img.name} 上传失败`);
							return;
						}
					} else {
						newImageUrls.push(img.url); // 保留已有的图片URL
					}
				}
				row.finalImageUrls = newImageUrls.join(',');
			}
		}

		// 更新请求数据中的图片URL
		requestData.details = requestData.details.map((detail, index) => ({
			...detail,
			product_Image_URL: SampleProductData.value[index].finalImageUrls || ''
		}));

		// 发送请求
		const url = isCreateMode.value ?
			'ProductSample/AddProductSample/Add' :
			'ProductSample/EditProductSample/Edit';

		const response = await request.post(url, requestData);

		if (response.code === 200) {
			ElMessage.success('草稿保存成功');
			dialogVisible.value = false;
			resetForm();
			// 刷新列表数据
			await GetProductSampleList(currentPage.value, pageSize.value);
		} else {
			ElMessage.error(response.msg || '保存草稿失败');
		}
	} catch (error) {
		console.error('保存草稿失败:', error);
		ElMessage.error('保存草稿失败，请稍后重试');
	} finally {
		loading.value = false;
	}
};

// 监听付费方式变化
watch(() => CreateDialogform.value.paymentMethod, (newValue) => {
	// 如果选择到付，自动设置已付快递费为0
	if (newValue === '2') { // 假设'2'代表到付
		CreateDialogform.value.paidExpressCost = '0';
	}
});

// 在fetchDataAndExecute函数后添加
// 检查URL参数并自动加载样品详情
onMounted(async () => {
	// 检查URL中的参数
	const id = route.query.id
	const viewDetail = route.query.viewDetail

	// 如果有id和viewDetail参数，则自动打开详情
	if (id && viewDetail === 'true') {
		console.log('自动加载样品详情, ID:', id)
		// 确保数据已加载完成
		await fetchDataAndExecute();
		// 打开详情
		handleView(id);
	}
})
</script>

<style scoped></style>
