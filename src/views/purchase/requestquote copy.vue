<template>
	<div class="request-quote-container">
		<!-- 询价单选择区域 -->
		<el-card class="inquiry-select-card">
			<template #header>
				<div class="card-header">
					<span>选择待处理询价单</span>
				</div>
			</template>
			<el-form :inline="true" class="inquiry-search-form">
				<el-form-item label="询价单号">
					<el-input v-model="searchParams.inquiryNumber" placeholder="请输入询价单号" clearable />
				</el-form-item>
				<el-form-item label="销售员">
					<el-select v-model="searchParams.salesId" placeholder="请选择销售员" style="width: 150px" clearable>
						<el-option v-for="item in optionss.sql_hr_sale" :key="item.dictCode" :label="item.dictLabel"
							:value="item.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="searchParams.status" placeholder="请选择状态" style="width: 150px" clearable>
						<el-option label="待处理" :value="0" />
						<el-option label="已处理" :value="1" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="searchInquiries">查询</el-button>
					<el-button @click="resetSearch">重置</el-button>
				</el-form-item>
			</el-form>

			<el-table ref="tableRef" :data="inquiryList" style="width: 100%" @row-click="handleSelectInquiry"
				highlight-current-row>
				<el-table-column prop="inquiryNumber" label="询价单号" width="120" />
				<el-table-column prop="productID" label="产品ID" width="180" v-if="false" />
				<el-table-column prop="productCode" label="产品编码" width="180" v-if="false" />
				<el-table-column prop="productName" label="产品名称" width="180" />
				<el-table-column prop="chineseSpecification" label="产品规格" width="180" v-if="false" />
				<el-table-column prop="mainmaterials" label="主要材料" width="180" v-if="false" />
				<el-table-column prop="productlength" label="产品长度" width="180" v-if="false" />
				<el-table-column prop="productwidth" label="产品宽度" width="180" v-if="false" />
				<el-table-column prop="productheight" label="产品高度" width="180" v-if="false" />
				<el-table-column prop="productweight" label="产品重量" width="180" v-if="false" />
				<el-table-column prop="smallpackagingmethod" label="小包装方式" width="180" v-if="false" />
				<el-table-column prop="moq" label="MOQ" width="180" v-if="false" />
				<el-table-column prop="quoteQuantity" label="报价数量" width="180" v-if="false" />
				<el-table-column prop="price" label="价格" width="180" v-if="false" />
				<el-table-column prop="negotiateprice" label="议价量" width="180" v-if="false" />
				<el-table-column prop="custommade" label="定制" width="180" v-if="false" />
				<el-table-column prop="priceterms" label="价格条款" width="180" v-if="false" />
				<el-table-column prop="taxincluded" label="含税+/-" width="180" v-if="false" />
				<el-table-column prop="mediumpackaging" label="中包装" width="180" v-if="false" />
				<el-table-column prop="outerbox" label="外箱" width="180" v-if="false" />
				<el-table-column prop="middlebagorouterbox" label="中包/外箱" width="180" v-if="false" />
				<el-table-column prop="outerboxlength" label="外箱长度" width="180" v-if="false" />
				<el-table-column prop="outerboxwidth" label="外箱宽度" width="180" v-if="false" />
				<el-table-column prop="outerboxheight" label="外箱高度" width="180" v-if="false" />
				<el-table-column prop="outerboxvolume" label="外箱体积" width="180" v-if="false" />
				<el-table-column prop="outerboxgrossweight" label="外箱毛重" width="180" v-if="false" />
				<el-table-column prop="IsNewProduct" label="是否新产品" width="180" v-if="false" />
				<el-table-column prop="productimage" label="产品图片" width="180" v-if="false" />
				<el-table-column prop="remark" label="备注" width="180" v-if="false" />
				<el-table-column prop="create_by" label="创建人" width="180" v-if="false" />
				<el-table-column prop="salePerson" label="销售员" width="120" />
				<el-table-column prop="create_time" label="创建时间" width="180" />
				<el-table-column prop="supplierID" label="供应商" width="180" v-if="false" />
				<el-table-column prop="status" label="状态" width="100">
					<template #default="scope">
						<el-tag :type="scope.row.status === 0 ? 'warning' : 'success'">
							{{ scope.row.status === 0 ? '待处理' : '已处理' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="150">
					<template #default="scope">
						<el-button :type="scope.row.status === 0 ? 'primary' : 'info'" size="small"
							@click.stop="handleRowButtonClick(scope.row, scope.$index)">
							{{ scope.row.status === 0 ? '处理' : '查看' }}
						</el-button>
					</template>
				</el-table-column>
			</el-table>

			<div class="pagination-container">
				<el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
					:page-sizes="[5, 10, 15, 20]" layout="total, sizes, prev, pager, next, jumper" :total="total"
					@size-change="handleSizeChange" @current-change="handleCurrentChange" />
			</div>
		</el-card>

		<!-- 询价处理表单 -->
		<el-card v-if="selectedInquiry" class="inquiry-form-card">
			<template #header>
				<div class="card-header">
					<span>询价单处理 - 单号: {{ selectedInquiry.inquiryNumber }}</span>
					<span class="inquiry-info">产品: {{ selectedInquiry.productName }} | 销售员: {{
						selectedInquiry.salePerson
					}}</span>
				</div>
			</template>

			<el-form :model="formData" label-width="180px" :rules="rules" ref="formRef">
				<!-- 基本信息 -->
				<el-divider content-position="left">基本信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="报价供应商" prop="SupplierID">
							<el-select v-model="formData.SupplierID" placeholder="请选择供应商" filterable clearable
								:disabled="selectedInquiry && selectedInquiry.status !== 0">
								<el-option v-for="item in supplierOptions" :key="item.value" :label="item.label"
									:value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="询价日期" prop="date">
							<el-date-picker v-model="formData.date" type="datetime" placeholder="选择日期时间" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="产品编号">
							<el-input v-model="formData.productnumber" placeholder="请输入产品编号" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<!-- 产品规格信息 -->
				<el-divider content-position="left">产品规格信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="产品规格">
							<el-input v-model="formData.productspecifications" placeholder="请输入产品规格" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="主要材料">
							<el-input v-model="formData.mainmaterials" placeholder="请输入主要材料" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="产品长度(CM)">
							<el-input-number v-model="formData.productlength" :precision="2" :step="0.1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品宽度(CM)">
							<el-input-number v-model="formData.productwidth" :precision="2" :step="0.1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品高度(CM)">
							<el-input-number v-model="formData.productheight" :precision="2" :step="0.1" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="产品克重">
							<el-input-number v-model="formData.productweight" :precision="4" :step="0.1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="小包装方式">
							<el-input v-model="formData.smallpackagingmethod" placeholder="请输入小包装方式" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<!-- 价格信息 -->
				<el-divider content-position="left">价格信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="MOQ" prop="moq">
							<el-input-number v-model="formData.moq" :precision="4" :step="1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="报价数量" prop="QuoteQuantity">
							<el-input-number v-model="formData.QuoteQuantity" :precision="0" :step="1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="价格" prop="price">
							<el-input-number v-model="formData.price" :precision="4" :step="0.1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="议价量">
							<el-input-number v-model="formData.negotiateprice" :precision="2" :step="0.1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="定制">
							<el-input-number v-model="formData.custommade" :precision="4" :step="0.1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="含税+/-">
							<el-input-number v-model="formData.taxincluded" :precision="4" :step="0.1"
								:disabled="selectedInquiry && selectedInquiry.status !== 0" />
						</el-form-item>
					</el-col>
				</el-row>

				<el-form-item label="价格条款">
					<el-select v-model="formData.priceterms" filterable placeholder="请选择价格条款"
						:disabled="selectedInquiry && selectedInquiry.status !== 0">
						<el-option v-for="dict in optionss.hr_purchase_pricing_term" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue" />
					</el-select>
				</el-form-item>

				<!-- 包装信息 -->
				<el-divider content-position="left">包装信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="中包装">
							<el-input-number v-model="formData.mediumpackaging" :precision="4" :step="1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱">
							<el-input-number v-model="formData.outerbox" :precision="4" :step="1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="中包/外箱">
							<el-input-number v-model="formData.middlebagorouterbox" :precision="4" :step="0.1"
								disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="外箱长度(CM)">
							<el-input-number v-model="formData.outerboxlength" :precision="4" :step="0.1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱宽度(CM)">
							<el-input-number v-model="formData.outerboxwidth" :precision="4" :step="0.1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱高度(CM)">
							<el-input-number v-model="formData.outerboxheight" :precision="4" :step="0.1" disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="外箱体积(m³)">
							<el-input-number v-model="formData.outerboxvolume" :precision="4" :step="0.001" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="外箱毛重(KGS)">
							<el-input-number v-model="formData.outerboxgrossweight" :precision="4" :step="0.1"
								disabled />
						</el-form-item>
					</el-col>
				</el-row>

				<!-- 其他信息 -->
				<el-divider content-position="left">其他信息</el-divider>
				<el-form-item label="是否新产品">
					<el-radio-group v-model="formData.IsNewProduct" disabled>
						<el-radio :label="0">否</el-radio>
						<el-radio :label="1">是</el-radio>
					</el-radio-group>
				</el-form-item>

				<el-form-item label="产品图片">
					<div v-if="formData.productimage">
						<el-image :src="formData.productimage" :preview-src-list="[formData.productimage]"
							style="width: 150px; height: 150px; object-fit: contain; border: 1px solid #eee; border-radius: 4px;" />
					</div>
					<div v-else class="no-image">
						<el-icon>
							<Picture />
						</el-icon>
						<span>暂无图片</span>
					</div>
				</el-form-item>

				<el-form-item label="备注">
					<el-input v-model="formData.Remark" type="textarea" :rows="3" placeholder="请输入备注信息"
						:disabled="selectedInquiry && selectedInquiry.status !== 0" />
				</el-form-item>

				<el-form-item>
					<template v-if="selectedInquiry && selectedInquiry.status === 0">
						<el-button type="primary" @click="submitForm">提交报价</el-button>
						<el-button @click="cancelEdit">取消</el-button>
					</template>
					<template v-else>
						<el-tag type="success">该询价单已处理</el-tag>
					</template>
				</el-form-item>
			</el-form>
		</el-card>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import request from '@/utils/request'; // 导入请求工具


/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_purchase_pricing_term: [], // 价格条款
		sql_hr_sale: [] // 销售员
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_purchase_pricing_term' }, { dictType: 'sql_hr_sale' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})
/*动态下拉框end*/

// 表单引用
const formRef = ref<FormInstance>();

// 分页相关
const currentPage = ref(1);
const pageSize = ref(5);
const total = ref(0);

// 搜索参数
const searchParams = reactive({
	inquiryNumber: '',
	salesId: null,
	status: 0, // 默认查询待处理的询价单
});

// 询价单列表
const inquiryList = ref([]);
// 选中的询价单
const selectedInquiry = ref(null);

// 供应商选项
const supplierOptions = ref([]);

// 表单数据
const formData = reactive({
	InquiryID: null,
	ProductID: null,
	date: new Date(),
	productimage: '',
	productnumber: '',
	productspecifications: '',
	mainmaterials: '',
	smallpackagingmethod: '',
	moq: null,
	negotiateprice: null,
	custommade: null,
	priceterms: null,
	taxincluded: null,
	QuoteQuantity: null,
	price: null,
	productlength: null,
	productwidth: null,
	productheight: null,
	productweight: null,
	mediumpackaging: null,
	outerbox: null,
	middlebagorouterbox: null,
	outerboxlength: null,
	outerboxwidth: null,
	outerboxheight: null,
	outerboxvolume: null,
	outerboxgrossweight: null,
	IsNewProduct: 0,
	Remark: '',
	SupplierID: null,
	Buyer: null
});

// 表单验证规则
const rules = {
	SupplierID: [{ required: true, message: '请选择供应商', trigger: 'change' }],
	date: [{ required: true, message: '请选择日期', trigger: 'change' }],
	moq: [{ required: true, message: '请输入MOQ', trigger: 'blur' }],
	QuoteQuantity: [{ required: true, message: '请输入报价数量', trigger: 'blur' }],
	price: [{ required: true, message: '请输入价格', trigger: 'blur' }]
};

// 表格引用
const tableRef = ref(null);

// 获取询价单列表
const getInquiryList = async () => {
	// 调用后端API获取询价单列表
	try {
		// 使用正确导入的request方法
		const res = await request({
			url: 'Inquiry/GetInquiryProductListByBuyer/GetInquiryProductList',
			method: 'get',
			params: {
				PageNum: currentPage.value,
				PageSize: pageSize.value,
				InquiryNumber: searchParams.inquiryNumber || '',
				salesPerson: searchParams.salesId || 0,
				Status: searchParams.status || 0
			}
		});
		if (res.code === 200) {
			inquiryList.value = res.data.result;
			total.value = res.data.totalNum;
		} else {
			ElMessage.error(res.msg || '获取询价单列表失败');
		}
		// ... existing code ...
	} catch (error) {
		console.error('获取询价单列表失败', error);
		ElMessage.error('获取询价单列表失败');
	}
};


// 查询询价单
const searchInquiries = () => {
	currentPage.value = 1;
	getInquiryList();
};

// 重置搜索条件
const resetSearch = () => {
	// 重置搜索参数
	Object.keys(searchParams).forEach(key => {
		searchParams[key] = key === 'status' ? 0 : '';
	});

	// 清空选中的询价单，这会导致询价单处理卡片隐藏
	selectedInquiry.value = null;

	// 重置表单数据
	if (formRef.value) {
		formRef.value.resetFields();
	}

	// 清空表单中的其他数据
	Object.keys(formData).forEach(key => {
		if (typeof formData[key] === 'number') {
			formData[key] = null;
		} else if (typeof formData[key] === 'string') {
			formData[key] = '';
		} else if (key === 'date') {
			formData[key] = new Date();
		} else if (key === 'IsNewProduct') {
			formData[key] = 0;
		}
	});

	// 重新获取询价单列表
	searchInquiries();

	// 清除表格选中行
	if (tableRef.value) {
		tableRef.value.setCurrentRow(null);
	}
};

// 处理分页大小变化
const handleSizeChange = (val: number) => {
	pageSize.value = val;
	getInquiryList();
};

// 处理页码变化
const handleCurrentChange = (val: number) => {
	currentPage.value = val;
	getInquiryList();
};

// 选择询价单
const handleSelectInquiry = (row: any) => {
	selectedInquiry.value = row;
	// 根据选中的询价单ID获取详细信息
	getInquiryDetail(row.id);
	if (row.productID) {
		getSupplierListByProduct(row.productID);
	}
};
// 根据产品ID获取供应商列表
const getSupplierListByProduct = async (productID: number) => {
	try {
		const res = await request({
			url: 'ProductInformation/GetSupplierListByProductID/GetProductSupplierList',
			method: 'get',
			params: {
				productID: productID
			}
		});
		if (res.code === 200) {
			// 使用后端返回的供应商数据
			supplierOptions.value = res.data.map(item => ({
				value: item.dictValue,
				label: item.dictLabel
			}));

			// 如果没有供应商，显示提示信息
			if (supplierOptions.value.length === 0) {
				ElMessage.warning('该产品没有关联的供应商');
			}
		} else {
			ElMessage.error(res.msg || '获取供应商列表失败');
		}
	} catch (error) {
		console.error('获取供应商列表失败', error);
		ElMessage.error('获取供应商列表失败');
	}
};

// 获取询价单详情
const getInquiryDetail = async (inquiryId: string) => {
	try {
		// 获取询价单详情
		formData.InquiryID = inquiryId;
		formData.ProductID = selectedInquiry.value.productID;
		formData.productnumber = selectedInquiry.value.productCode;
		formData.productspecifications = selectedInquiry.value.chineseSpecification;
		formData.mainmaterials = selectedInquiry.value.mainmaterials;
		formData.productlength = Number(selectedInquiry.value.productlength);
		formData.productwidth = Number(selectedInquiry.value.productwidth);
		formData.productheight = Number(selectedInquiry.value.productheight);
		formData.productweight = Number(selectedInquiry.value.productweight);
		formData.smallpackagingmethod = selectedInquiry.value.smallpackagingmethod;
		formData.moq = Number(selectedInquiry.value.moq);
		formData.negotiateprice = Number(selectedInquiry.value.negotiateprice);
		formData.custommade = Number(selectedInquiry.value.custommade);
		formData.priceterms = state.optionss.hr_purchase_pricing_term.find(item => item.dictValue === selectedInquiry.value.priceterms.toString())?.dictValue;
		formData.taxincluded = Number(selectedInquiry.value.taxincluded);
		formData.QuoteQuantity = Number(selectedInquiry.value.quoteQuantity);
		formData.price = Number(selectedInquiry.value.price);
		formData.outerboxlength = Number(selectedInquiry.value.outerboxlength);
		formData.outerboxwidth = Number(selectedInquiry.value.outerboxwidth);
		formData.outerboxheight = Number(selectedInquiry.value.outerboxheight);
		formData.outerboxvolume = Number(selectedInquiry.value.outerboxvolume);
		formData.outerboxgrossweight = Number(selectedInquiry.value.outerboxgrossweight);
		formData.productimage = selectedInquiry.value.productimage;
		formData.mediumpackaging = selectedInquiry.value.mediumpackaging;
		formData.outerbox = selectedInquiry.value.outerbox;
		formData.middlebagorouterbox = selectedInquiry.value.middlebagorouterbox;
		formData.IsNewProduct = Number(selectedInquiry.value.isNewProduct);
		formData.Remark = selectedInquiry.value.remark == null ? '无' : selectedInquiry.value.remark;
		formData.date = selectedInquiry.value.create_time;
		if (selectedInquiry.value.supplierID != 0) {
			formData.SupplierID = selectedInquiry.value.supplierID;
		}
		// 其他字段保持默认值或为空
	} catch (error) {
		console.error('获取询价单详情失败', error);
		ElMessage.error('获取询价单详情失败');
	}
};

// 提交表单
const submitForm = async () => {
	if (!formRef.value) return;

	await formRef.value.validate(async (valid) => {
		if (valid) {
			try {
				// 构建请求体
				const processingQuotesRequest = {
					ID: selectedInquiry.value.id,
					MOQ: formData.moq,
					Price: formData.price,
					QuoteQuantity: formData.QuoteQuantity,
					negotiateprice: formData.negotiateprice,
					CustomMade: formData.custommade,
					TaxIncluded: formData.taxincluded,
					PriceTerms: formData.priceterms,
					SupplierID: formData.SupplierID,
					Remark: formData.Remark,
				};
				const res = await request({
					url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
					method: 'post',
					data: processingQuotesRequest
				});

				if (res.code === 200) {
					ElMessage.success(res.msg || '报价提交成功');
					selectedInquiry.value = null;
					getInquiryList();
				} else {
					ElMessage.error(res.msg || '报价提交失败');
				}
			} catch (error) {
				console.error('提交失败', error);
				ElMessage.error('提交失败');
			}
		} else {
			ElMessage.error('请填写必填项');
		}
	});
};

// 取消编辑
const cancelEdit = () => {
	selectedInquiry.value = null;
	if (formRef.value) {
		formRef.value.resetFields();
	}
};

// 处理行按钮点击
const handleRowButtonClick = (row, index) => {
	// 设置当前行为选中状态
	if (tableRef.value) {
		tableRef.value.setCurrentRow(row);
	}
	// 调用原有的处理方法
	handleSelectInquiry(row);
};

// 页面加载时获取数据
onMounted(() => {
	getInquiryList();
});
</script>

<style scoped>
.request-quote-container {
	padding: 20px;
}

.inquiry-select-card,
.inquiry-form-card {
	margin-bottom: 20px;
}

.card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}

.inquiry-info {
	font-size: 14px;
	color: #606266;
}

.inquiry-search-form {
	margin-bottom: 15px;
}

.pagination-container {
	margin-top: 15px;
	display: flex;
	justify-content: flex-end;
}

.el-divider {
	margin: 24px 0;
}
</style>