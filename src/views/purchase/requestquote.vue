<template>
	<div>
		<!-- 询价单表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 过滤条件区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="16" style="margin-bottom: 10px;">
					<el-col :span="3">
						<el-radio-group v-model="searchParams.status" @change="searchInquiries" size="default">
							<el-radio-button :label="0">待处理</el-radio-button>
							<el-radio-button :label="1">已处理</el-radio-button>
						</el-radio-group>
					</el-col>
					<el-col :span="3">
						<el-input v-model="searchParams.inquiryNumber" clearable placeholder="输入询价单号"
							@keyup.enter="searchInquiries" size="default" />
					</el-col>
					<el-col :span="3">
						<el-select v-model="searchParams.salesId" placeholder="请选择销售员" clearable
							@change="searchInquiries" size="default" style="width: 100%">
							<el-option v-for="item in optionss.sql_hr_sale" :key="item.dictCode" :label="item.dictLabel"
								:value="item.dictValue" />
						</el-select>
					</el-col>
					<el-col :span="3">
						<div style="text-align: left;">
							<el-button type="primary" @click="searchInquiries" size="default">搜索</el-button>
							<el-button @click="resetSearch" size="default">重置</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table :data="inquiryList" style="width: 100%; table-layout: fixed;" stripe v-loading="loading"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="inquiry_number" label="询价单号" width="120">
					<template #default="scope">
						<span>{{ scope.row.inquiry_number }}</span>
						<el-tag v-if="Number(scope.row.isDraft) == 1" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="subject" label="询价主题" width="200"></el-table-column>
				<el-table-column prop="salePerson" label="询价人" width="130"></el-table-column>
				<el-table-column prop="status" label="状态" width="100">
					<template #default="scope">
						<el-tag
							:type="Number(scope.row.isDraft) == 1 ? 'info' : (scope.row.status === 0 ? 'warning' : 'success')">
							{{ Number(scope.row.isDraft) == 1 ? '草稿' : (scope.row.status === 0 ? '待处理' :
								'已处理')
							}}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column fixed="right" prop="operate" label="操作" :width="200">
					<template v-slot:default="scope">
						<el-button v-if="scope.row.status === 0" link type="primary" size="small"
							@click="editDetail(scope.row)">去处理</el-button>
						<el-button v-else link type="primary" size="small"
							@click="viewDetail(scope.row)">查看详情</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handleCurrentChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :page-sizes="[5, 10, 15, 20]" :total="total"
				background layout="total, sizes, prev, pager, next, jumper" style="margin-top: 5px;" />
		</div>

		<!-- 处理询价单弹窗 -->
		<el-dialog :modal="false" :modal-penetrable="true" v-model="dialogVisible"
			:title="dialogEditMode ? '询价单处理' : '询价单详情'" width="75%" @close="closeDialog" :close-on-click-modal="false">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">基本信息</span>
					</template>
					<el-form ref="formRef" :model="dialogForm" :rules="rules" label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="询价单号">
									<el-input v-model="dialogForm.inquiry_number" disabled style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="询价主题">
									<el-input v-model="dialogForm.subject" disabled style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="询价人">
									<el-input v-model="dialogForm.salePerson" disabled style="width: 300px"
										size="default" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="状态">
									<el-tag
										:type="Number(dialogForm.isDraft) == 1 ? 'info' : (dialogForm.status === 0 ? 'warning' : 'success')">
										{{ Number(dialogForm.isDraft) == 1 ? '草稿' : (dialogForm.status === 0 ? '待处理' :
											'已处理')
										}}
									</el-tag>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>
			<span style="font-size: 20px; font-weight: bold;">产品信息</span>
			<el-divider></el-divider>
			<el-table :data="productList" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="status" label="报价状态" width="100">
					<template #default="scope">
						<el-tag :type="scope.row.status === 0 ? 'warning' : 'success'">
							{{ scope.row.status === 0 ? '待报价' : '已报价' }}
						</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="productimage" label="询价产品图片" width="150" align="center">
					<template #default="scope">
						<div v-if="scope.row.productimage">
							<el-image style="width: 37.8px; height: 37.8px" :src="scope.row.productimage"
								:preview-src-list="[scope.row.productimage]" :zoom-rate="1.2" :max-scale="7"
								:min-scale="0.2" fit="cover" preview-teleported="true" class="product-image-small"
								@mouseenter="showHoverImage($event, scope.row.productimage)"
								@mouseleave="hideHoverImage">
								<template #error>
									<span>加载失败</span>
								</template>
							</el-image>
						</div>
						<div v-else class="no-image">
							<el-icon>
								<Picture />
							</el-icon>
							<span>暂无图片</span>
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="productCode" label="编号" width="150" align="center">
					<template #default="{ row }">
						<span>{{ row.productCode }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="productName" label="产品名称" width="200" align="center">
					<template #default="{ row }">
						<span>{{ row.productName }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="productspecifications" label="规格" width="150" align="center">
					<template #default="{ row }">
						<span>{{ row.productspecifications }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="mainmaterials" label="主要材料" width="150" align="center" v-if="false">
					<template #default="{ row }">
						<span>{{ row.mainmaterials }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="smallpackagingmethod" label="小包装方式" width="150" align="center">
					<template #default="{ row }">
						<span>{{ getPackingLabel(row.smallpackagingmethod) }}</span>
					</template>
				</el-table-column>
				<!-- <el-table-column prop="IsNewProduct" label="是否新产品" width="120" align="center">
						<template #default="{ row }">
							<el-tag :type="row.IsNewProduct === 1 ? 'success' : 'info'">
								{{ row.IsNewProduct === 1 ? '是' : '否' }}
							</el-tag>
						</template>
					</el-table-column> -->
				<el-table-column prop="supplierID" label="供应商" width="200" align="center">
					<template #default="{ row }">
						<el-select v-if="dialogEditMode" v-model="row.supplierID" filterable clearable
							placeholder="请选择供应商" style="width: 100%" size="default">
							<el-option v-for="item in row.supplierOptions.filter(option => option.value !== 0)"
								:key="item.value" :label="item.label" :value="item.value" />
						</el-select>
						<span v-else>{{ !row.supplierID ? '无供应商' : getSupplierLabelForProduct(row.supplierID,
							row.supplierOptions) }}</span>
					</template>
				</el-table-column>

				<el-table-column prop="variousminimumorderquantities" label="各种起订量" width="120" align="center">
					<el-table-column prop="moq" label="MOQ" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.moq" size="default" />
							<span v-else>{{ row.moq }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="negotiateprice" label="议价" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.negotiateprice" size="default" />
							<span v-else>{{ row.negotiateprice }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="custommade" label="定制" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.custommade" size="default" />
							<span v-else>{{ row.custommade }}</span>
						</template>
					</el-table-column>
				</el-table-column>
				<el-table-column prop="priceTerms" label="价格条款" width="200" align="center">
					<template #default="{ row }">
						<el-select v-if="dialogEditMode" v-model="row.priceTerms" filterable placeholder="请选择价格条款"
							style="width: 100%" size="default" clearable>
							<el-option
								v-for="dict in optionss.hr_purchase_pricing_term.filter(option => option.dictValue !== 0)"
								:key="dict.dictCode" :label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
						<span v-else>{{ !row.priceTerms ? '无价格条款' : getPriceTermsLabel(row.priceTerms)
						}}</span>
					</template>
				</el-table-column>
				<el-table-column prop="taxincluded" label="含税+/-(%)" width="120" align="center">
					<template #default="{ row }">
						<el-input v-if="dialogEditMode" v-model="row.taxincluded" size="default" />
						<span v-else>{{ formatTaxIncluded(row.taxincluded) }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="quoteQuantity" label="报价数量" width="120" align="center">
					<template #default="{ row }">
						<el-input v-if="dialogEditMode" v-model="row.quoteQuantity" size="default" />
						<span v-else>{{ row.quoteQuantity }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="price" label="价格" width="120" align="center">
					<template #default="{ row }">
						<el-input v-if="dialogEditMode" v-model="row.price" size="default" />
						<span v-else>{{ row.price }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="singleproductsalessize" label="单个产品销售尺寸(CM)" width="120" align="center"
					v-if="false">
					<el-table-column prop="productlength" label="长" width="120" align="center" v-if="false">
						<template #default="{ row }">
							<span>{{ row.productlength }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="productwidth" label="宽" width="120" align="center" v-if="false">
						<template #default="{ row }">
							<span>{{ row.productwidth }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="productheight" label="高" width="120" align="center" v-if="false">
						<template #default="{ row }">
							<span>{{ row.productheight }}</span>
						</template>
					</el-table-column>
					<!-- <el-table-column prop="productweight" label="克重" width="120" align="center">
							<template #default="{ row }">
								<span>{{ row.productweight }}</span>
							</template>
						</el-table-column> -->
				</el-table-column>
				<el-table-column prop="productweight" label="克重" width="120" align="center">
					<template #default="{ row }">
						<span>{{ row.productweight }}</span>
					</template>
				</el-table-column>
				<el-table-column prop="boxing" label="装箱" width="120" align="center">
					<el-table-column prop="mediumpackaging" label="中包装" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.mediumpackaging" @input="calculateVolume(row)"
								size="default" />
							<span v-else>{{ row.mediumpackaging }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="outerbox" label="外箱" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.outerbox" @input="calculateVolume(row)"
								size="default" />
							<span v-else>{{ row.outerbox }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="middlebagorouterbox" label="中包/外箱" width="120" align="center" v-if="false">
						<template #default="{ row }">
							<span>{{ row.middlebagorouterbox }}</span>
						</template>
					</el-table-column>
				</el-table-column>
				<el-table-column prop="outerboxdata" label="外箱数据(CM)" width="120" align="center">
					<el-table-column prop="outerboxlength" label="长" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.outerboxlength" @input="calculateVolume(row)"
								size="default" />
							<span v-else>{{ row.outerboxlength }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="outerboxwidth" label="宽" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.outerboxwidth" @input="calculateVolume(row)"
								size="default" />
							<span v-else>{{ row.outerboxwidth }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="outerboxheight" label="高" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.outerboxheight" @input="calculateVolume(row)"
								size="default" />
							<span v-else>{{ row.outerboxheight }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="outerboxvolume" label="体积m³" width="120" align="center">
						<template #default="{ row }">
							<span>{{ getDisplayVolume(row) }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="outerboxgrossweight" label="毛重KGS" width="120" align="center">
						<template #default="{ row }">
							<el-input v-if="dialogEditMode" v-model="row.outerboxgrossweight" size="default" />
							<span v-else>{{ row.outerboxgrossweight }}</span>
						</template>
					</el-table-column>
				</el-table-column>
				<el-table-column prop="quoteNotes" label="备注" width="200" align="center">
					<template #default="{ row }">
						<el-input v-if="dialogEditMode" v-model="row.quoteNotes" style="width: 100%" size="default" />
						<span v-else>{{ row.quoteNotes }}</span>
					</template>
				</el-table-column>
			</el-table>
			<span style="font-size: 20px; font-weight: bold;">询价单附件</span>
			<el-divider></el-divider>
			<!-- 附件部分 -->
			<div>
				<!-- 显示现有附件列表 -->
				<div>
					<h3>附件列表:</h3>
					<el-table :data="inquiryDocumentList" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="fileName" label="文件名"></el-table-column>
						<el-table-column label="操作">
							<template #default="scope">
								<el-button link type="primary" size="small" @click="handleDownload(scope.row)"
									:disabled="!dialogEditMode">
									下载
								</el-button>
								<!-- <el-button link type="danger" size="small"
										@click="deleteDocument(scope.row, scope.$index)" :disabled="!dialogEditMode">
										删除
									</el-button> -->
							</template>
						</el-table-column>
					</el-table>
				</div>

				<!-- 上传新附件的组件 -->
				<div v-if="dialogEditMode">
					<h3>上传新附件:</h3>
					<el-upload :auto-upload="false" :on-change="handleFileChange" :on-remove="handleFileRemove"
						:file-list="uploadfileList" multiple>
						<template #trigger>
							<el-button type="primary">选取文件</el-button>
						</template>
					</el-upload>
				</div>

				<!-- 显示新上传的附件列表 -->
				<div v-if="dialogEditMode && inquryProductDocumentTableData.length > 0">
					<h3>新上传的附件:</h3>
					<el-table :data="inquryProductDocumentTableData" style="width: 100%; table-layout: fixed;"
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="FileName" label="文件名"></el-table-column>
						<el-table-column label="操作">
							<template #default="scope">
								<el-button @click="handleDeleteNewFile(scope.$index)" type="text"
									size="small">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</div>
			</div>
			<el-divider></el-divider>
			<span style="font-size: 20px; font-weight: bold;">其他信息</span>
			<el-divider></el-divider>
			<el-form :model="dialogForm" label-width="120px">
				<el-row>
					<el-col :span="24">
						<el-form-item label="询价备注">
							<el-input type="textarea" v-model="dialogForm.description" style="width: 100%" disabled
								:autosize="{ minRows: 3, maxRows: 6 }" size="default" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="closeDialog">取消</el-button>
					<el-button v-if="dialogEditMode" type="warning" @click="saveDetailAsDraft">保存草稿</el-button>
					<el-button v-if="dialogEditMode" type="success" @click="saveDetail">提交</el-button>
				</span>
			</template>
		</el-dialog>

		<!-- 悬停图片容器 -->
		<div v-if="hoverImageVisible" class="hover-image-container"
			:style="{ left: hoverImagePosition.x + 'px', top: hoverImagePosition.y + 'px' }">
			<img :src="hoverImageSrc" alt="产品图片" />
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, toRefs, computed, h, nextTick, getCurrentInstance } from 'vue';
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus';
import { Plus, Picture } from '@element-plus/icons-vue';
import request from '@/utils/request'; // 导入请求工具
import { useRoute } from 'vue-router';
import { getDicts } from '@/api/system/dict/data';

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_purchase_pricing_term: [], // 价格条款
		sql_hr_sale: [], // 销售员
		hr_packing: [] // 小包装方式
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_purchase_pricing_term' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_packing' }]
getDicts(dictParams).then((response) => {
	console.log('字典数据加载结果:', response);
	response.data.forEach((element) => {
		// 对于价格条款字典，过滤掉值为0的选项
		if (element.dictType === 'hr_purchase_pricing_term') {
			state.optionss[element.dictType] = element.list.filter(item => item.dictValue !== 0);
		} else {
			state.optionss[element.dictType] = element.list;
		}
		console.log(`加载字典 ${element.dictType}:`, state.optionss[element.dictType]);
	})
})
/*动态下拉框end*/

// 表单引用
const formRef = ref<FormInstance>();

// 分页相关
const currentPage = ref(1);
const pageSize = ref(10);
const total = ref(0);

// 搜索参数
const searchParams = reactive({
	inquiryNumber: '',
	salesId: null,
	status: 0, // 默认查询待处理的询价单
});

// 询价单列表
const inquiryList = ref([]);

// 供应商选项（全局，用于兼容）
const supplierOptions = ref([]);

// 附件相关
const inquiryDocumentList = ref([]); // 询价单附件列表
const uploadfileList = ref([]); // 上传文件列表
const inquryProductDocumentTableData = ref([]); // 新上传的附件列表

// 表单验证规则
const rules = {
	moq: [
		{ required: true, message: '请输入MOQ', trigger: 'blur' },
		{ type: 'number', message: 'MOQ必须为数字', trigger: 'blur', transform: (value) => Number(value) }
	],
	QuoteQuantity: [
		{ required: true, message: '请输入报价数量', trigger: 'blur' },
		{ type: 'number', message: '报价数量必须为数字', trigger: 'blur', transform: (value) => Number(value) }
	],
	price: [
		{ required: true, message: '请输入价格', trigger: 'blur' },
		{ type: 'number', message: '价格必须为数字', trigger: 'blur', transform: (value) => Number(value) }
	]
};

// 加载状态
const loading = ref(false);

// 弹窗相关
const dialogVisible = ref(false);
const dialogEditMode = ref(false);
const dialogForm = reactive<any>({});
const productList = ref([]); // 产品列表
const basicInfoCollapseActive = ref(['basicInfo']); // 折叠面板激活状态

// 获取供应商标签
const getSupplierLabel = (supplierId) => {
	const supplier = supplierOptions.value.find(item => item.value === supplierId);
	return supplier ? supplier.label : '';
};

// 获取产品特定的供应商标签
const getSupplierLabelForProduct = (supplierId, productSupplierOptions) => {
	console.log('getSupplierLabelForProduct 调用:', { supplierId, productSupplierOptions });
	if (!supplierId || supplierId === 0) return '';
	// 确保类型匹配
	const supplier = productSupplierOptions?.find(item => Number(item.value) === Number(supplierId));
	console.log('找到的供应商:', supplier);
	return supplier ? supplier.label : '';
};

// 获取价格条款标签
const getPriceTermsLabel = (priceTerms) => {
	console.log('getPriceTermsLabel 调用:', { priceTerms, availableOptions: optionss.value.hr_purchase_pricing_term });
	if (!priceTerms || priceTerms === 0) return '';
	// 确保类型匹配
	const priceTerm = optionss.value.hr_purchase_pricing_term.find(item => Number(item.dictValue) === Number(priceTerms));
	console.log('找到的价格条款:', priceTerm);
	return priceTerm ? priceTerm.dictLabel : '';
};

// 获取小包装方式标签
const getPackingLabel = (packingValue) => {
	if (!packingValue || packingValue === 0) return '';
	const packing = optionss.value.hr_packing?.find(item => Number(item.dictValue) === Number(packingValue));
	return packing ? packing.dictLabel : packingValue;
};

// 含税显示时加上%号
const formatTaxIncluded = (val) => {
	if (val == null || val === '') return '';
	const s = String(val).trim();
	return s.endsWith('%') ? s : s + '%';
};

// 计算体积
const calculateVolume = (row) => {
	const length = parseFloat(row.outerboxlength) || 0;
	const width = parseFloat(row.outerboxwidth) || 0;
	const height = parseFloat(row.outerboxheight) || 0;

	// 计算体积 (长 * 宽 * 高) / 1000000 转换为立方米
	const volume = (length * width * height) / 1000000;
	row.outerboxvolume = volume.toFixed(4);
};

// 体积列显示计算结果（长*宽*高/1000000）
const getDisplayVolume = (row) => {
	const length = parseFloat(row.outerboxlength) || 0;
	const width = parseFloat(row.outerboxwidth) || 0;
	const height = parseFloat(row.outerboxheight) || 0;
	const volume = (length * width * height) / 1000000;
	return volume > 0 ? volume.toFixed(4) : (row.outerboxvolume ?? '');
};

// 悬停图片相关
const hoverImageVisible = ref(false);
const hoverImageSrc = ref('');
const hoverImagePosition = ref({ x: 0, y: 0 });

// 显示悬停图片
const showHoverImage = (event, imageSrc) => {
	hoverImageSrc.value = imageSrc;
	hoverImagePosition.value = {
		x: event.clientX + 10,
		y: event.clientY - 100
	};
	hoverImageVisible.value = true;
};

// 隐藏悬停图片
const hideHoverImage = () => {
	hoverImageVisible.value = false;
};

// 获取询价单列表
// const getInquiryList = async () => {
// 	// 调用后端API获取询价单列表
// 	try {
// 		loading.value = true;
// 		// 使用采购员专用的API接口
// 		const requestParams = {
// 			PageNum: currentPage.value,
// 			PageSize: pageSize.value,
// 			InquiryCode: searchParams.inquiryNumber || '',
// 			startDate: '',
// 			endDate: '',
// 			Status: searchParams.status === 1 ? [1, 2] : [0]
// 		};
// 		console.log('API请求参数:', requestParams); // 添加调试日志

// 		const res: any = await request({
// 			url: 'Inquiry/GetInquiryListByPurchase/GetList',
// 			method: 'GET',
// 			params: requestParams
// 		});
// 		if (res.code === 200) {
// 			const data = res.data;
// 			inquiryList.value = data.result || [];
// 			total.value = data.totalNum || 0;
// 			console.log('API返回数据:', data); // 添加调试日志
// 			console.log('当前状态过滤:', searchParams.status); // 添加调试日志

// 			// 处理询价人字段，转换为销售员显示
// 			if (inquiryList.value.length > 0) {
// 				inquiryList.value.forEach(item => {
// 					item.salePerson = state.optionss.sql_hr_sale.find(option => option.dictValue === item.inquirer?.toString())?.dictLabel || '';
// 				});
// 			}
// 		} else {
// 			ElMessage.error(res.msg || '获取询价单列表失败');
// 		}
// 	} catch (error) {
// 		console.error('获取询价单列表失败', error);
// 		ElMessage.error('获取询价单列表失败');
// 	} finally {
// 		loading.value = false;
// 	}
// };

const getInquiryList = async () => {
	try {
		loading.value = true;

		const params = new URLSearchParams();
		params.append('PageNum', String(currentPage.value));
		params.append('PageSize', String(pageSize.value));

		// 这些空的别传，避免后端 DateTime? 绑定异常
		if (searchParams.inquiryNumber) params.append('InquiryCode', searchParams.inquiryNumber);
		if (searchParams.startDate) params.append('startDate', searchParams.startDate);
		if (searchParams.endDate) params.append('endDate', searchParams.endDate);

		const statusArr = (searchParams.status === 1) ? [1, 2] : [0];
		statusArr.forEach(s => params.append('Status', String(s))); // ✅ repeat 格式

		const res: any = await request({
			url: `Inquiry/GetInquiryListByPurchase/GetList?${params.toString()}`,
			method: 'GET'
		});

		if (res.code === 200) {
			const data = res.data;
			inquiryList.value = data.result || [];
			total.value = data.totalNum || 0;

			inquiryList.value.forEach(item => {
				item.salePerson =
					state.optionss.sql_hr_sale.find(option => option.dictValue === item.inquirer?.toString())
						?.dictLabel || '';
			});
		} else {
			ElMessage.error(res.msg || '获取询价单列表失败');
		}
	} catch (error) {
		console.error('获取询价单列表失败', error);
		ElMessage.error('获取询价单列表失败');
	} finally {
		loading.value = false;
	}
};


// 查询询价单
const searchInquiries = () => {
	currentPage.value = 1;
	console.log('搜索参数:', searchParams); // 添加调试日志
	getInquiryList();
};

// 重置搜索条件
const resetSearch = () => {
	// 重置搜索参数
	searchParams.inquiryNumber = '';
	searchParams.salesId = null;
	searchParams.status = 0; // 重置为待处理状态
	console.log('重置后的搜索参数:', searchParams); // 添加调试日志

	// 重新获取询价单列表
	searchInquiries();
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

// 根据产品ID获取供应商列表
const getSupplierListByProduct = async (productID: number) => {
	try {
		const res: any = await request({
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

// 打开弹窗
const openDialog = (row, isEdit) => {
	Object.assign(dialogForm, row);
	dialogVisible.value = true;
	dialogEditMode.value = isEdit;
};

// 关闭弹窗
const closeDialog = () => {
	dialogVisible.value = false;
	dialogEditMode.value = false;
	// 清空表单数据
	Object.keys(dialogForm).forEach(key => {
		delete dialogForm[key];
	});
	// 清空产品列表
	productList.value = [];
	// 清空附件相关数据
	inquiryDocumentList.value = [];
	uploadfileList.value = [];
	inquryProductDocumentTableData.value = [];
};

// 编辑详情
const editDetail = async (row) => {
	console.log('编辑详情，传入的row:', row); // 添加调试日志
	Object.assign(dialogForm, row);
	dialogEditMode.value = true;
	// 先加载产品详细信息（包括每个产品的供应商列表）
	await loadInquiryDocuments(row.id);

	dialogVisible.value = true;
};

// 查看详情
const viewDetail = async (row) => {
	console.log('查看详情，传入的row:', row); // 添加调试日志
	Object.assign(dialogForm, row);
	dialogEditMode.value = false;

	// 先加载产品详细信息（包括每个产品的供应商列表）
	await loadInquiryDocuments(row.id);
	dialogVisible.value = true;
};

// 保存详情
const saveDetail = async () => {
	if (!formRef.value) return;

	try {
		await formRef.value.validate();

		// 处理所有产品的数据
		const processingRequests = productList.value.map(product => {
			return {
				InquiryID: dialogForm.id,
				ID: product.id,
				MOQ: Number(product.moq),
				Price: Number(product.price),
				QuoteQuantity: Number(product.quoteQuantity),
				negotiateprice: Number(product.negotiateprice || 0),
				CustomMade: Number(product.custommade || 0),
				TaxIncluded: Number(product.taxincluded || 0),
				PriceTerms: product.priceTerms ? Number(product.priceTerms) : null,
				SupplierID: product.supplierID ? Number(product.supplierID) : 0,
				QuoteNotes: product.quoteNotes || '',
				Remark: product.quoteNotes || '无',
				BuyerID: 0, // 后端会自动设置
				IsDraft: 0,
				Status: 1,
				// 新增的包装和尺寸字段
				MediumPackaging: Number(product.mediumpackaging || 0),
				OuterBox: Number(product.outerbox || 0),
				OuterBoxLength: Number(product.outerboxlength || 0),
				OuterBoxWidth: Number(product.outerboxwidth || 0),
				OuterBoxHeight: Number(product.outerboxheight || 0),
				OuterBoxVolume: Number(product.outerboxvolume || 0),
				OuterBoxGrossWeight: Number(product.outerboxgrossweight || 0)
			};
		});

		console.log('提交的产品数据:', processingRequests);

		// 直接提交产品数组
		const res: any = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingRequests
		});

		if (res.code === 200) {
			ElMessage.success(res.msg || '报价提交成功');
			dialogVisible.value = false;
			getInquiryList();
		} else {
			ElMessage.error(res.msg || '报价提交失败');
		}
	} catch (error) {
		if (error instanceof Error) {
			ElMessage.error(error.message || '请填写必填项');
		} else {
			ElMessage.error('提交失败');
		}
	}
};

// 保存草稿
const saveDetailAsDraft = async () => {
	if (!formRef.value) return;

	try {
		// 处理所有产品的数据
		const processingRequests = productList.value.map(product => {
			return {
				InquiryID: dialogForm.id,
				ID: product.id,
				MOQ: Number(product.moq || 0),
				Price: Number(product.price || 0),
				QuoteQuantity: Number(product.quoteQuantity || 0),
				negotiateprice: Number(product.negotiateprice || 0),
				CustomMade: Number(product.custommade || 0),
				TaxIncluded: Number(product.taxincluded || 0),
				PriceTerms: product.priceTerms ? Number(product.priceTerms) : null,
				SupplierID: product.supplierID ? Number(product.supplierID) : 0,
				QuoteNotes: product.quoteNotes || '无',
				Remark: product.quoteNotes || '无',
				BuyerID: 0, // 后端会自动设置
				IsDraft: 1,
				Status: 0,
				// 新增的包装和尺寸字段
				MediumPackaging: Number(product.mediumpackaging || 0),
				OuterBox: Number(product.outerbox || 0),
				OuterBoxLength: Number(product.outerboxlength || 0),
				OuterBoxWidth: Number(product.outerboxwidth || 0),
				OuterBoxHeight: Number(product.outerboxheight || 0),
				OuterBoxVolume: Number(product.outerboxvolume || 0),
				OuterBoxGrossWeight: Number(product.outerboxgrossweight || 0)
			};
		});

		// 打印请求数据，用于调试
		console.log('保存草稿请求数据:', processingRequests);

		// 直接提交产品数组
		const res: any = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingRequests
		});

		if (res.code === 200) {
			ElMessage.success(res.msg || '草稿保存成功');
			dialogVisible.value = false;
			// 刷新列表
			await getInquiryList();
		} else {
			ElMessage.error(res.msg || '草稿保存失败');
		}
	} catch (error) {
		console.error('草稿保存失败', error);
		ElMessage.error('草稿保存失败');
	}
};

// 加载询价单附件和基本信息
const loadInquiryDocuments = async (inquiryId) => {
	try {
		const res: any = await request({
			url: 'Inquiry/GetInquiryProductsAndDocumentsByPurchase/GetInquiryProductsAndDocuments',
			method: 'GET',
			params: {
				InqueryID: inquiryId
			}
		});

		if (res.code === 200 && res.data) {
			// 加载附件列表
			if (res.data.documents && res.data.documents.length > 0) {
				inquiryDocumentList.value = res.data.documents.map(item => ({
					id: item.id,
					fileName: item.fileName,
					documentUrl: item.documentUrl
				}));
			} else {
				inquiryDocumentList.value = [];
			}

			// 加载询价单基本信息，包括 description
			if (res.data.inquiry && res.data.inquiry.length > 0) {
				dialogForm.description = res.data.inquiry[0].description || '';
				dialogForm.subject = res.data.inquiry[0].subject || '';
			}

			// 加载产品详细信息，包括所有产品字段
			if (res.data.products && res.data.products.length > 0) {
				console.log('产品数据:', res.data.products); // 添加调试日志

				// 处理所有产品数据，并为每个产品获取供应商列表
				const processedProducts = [];
				for (const product of res.data.products) {
					// 获取该产品的供应商列表
					let productSupplierOptions = [];
					if (product.productID) {
						try {
							const supplierRes: any = await request({
								url: 'ProductInformation/GetSupplierListByProductID/GetProductSupplierList',
								method: 'get',
								params: {
									productID: product.productID
								}
							});
							if (supplierRes.code === 200) {
								// 过滤掉值为0的供应商选项
								productSupplierOptions = supplierRes.data
									.filter(item => item.dictValue !== 0)
									.map(item => ({
										value: item.dictValue,
										label: item.dictLabel
									}));
							}
						} catch (error) {
							console.error(`获取产品${product.productID}的供应商列表失败:`, error);
						}
					}

					// 添加调试日志
					console.log(`产品 ${product.productNumber} 的原始数据:`, {
						supplierID: product.supplierID,
						priceTerms: product.priceTerms,
						productID: product.productID
					});

					const processedProduct = {
						// 产品基本信息
						id: product.id,
						productID: product.productID,
						status: product.status,
						productimage: product.productImage || '',
						productName: product.productName || '',
						productCode: product.productNumber || '',
						productspecifications: product.productSpecifications || '',
						mainmaterials: product.mainMaterials || '',
						smallpackagingmethod: product.smallPackagingMethod || '',
						IsNewProduct: product.isNewProduct || 0,

						// 可编辑字段
						supplierID: product.supplierID !== undefined && product.supplierID !== null && product.supplierID !== 0 ? product.supplierID : null,
						quoteNotes: product.quoteNotes || '',
						moq: product.moq !== undefined && product.moq !== null ? product.moq : 0,
						negotiateprice: product.negotiateprice !== undefined && product.negotiateprice !== null ? product.negotiateprice : 0,
						custommade: product.customMade !== undefined && product.customMade !== null ? product.customMade : 0,
						priceTerms: product.priceTerms !== undefined && product.priceTerms !== null && product.priceTerms !== 0 ? product.priceTerms : null,
						taxincluded: product.taxIncluded !== undefined && product.taxIncluded !== null ? product.taxIncluded : 0,
						quoteQuantity: product.quoteQuantity !== undefined && product.quoteQuantity !== null ? product.quoteQuantity : 0,
						price: product.price !== undefined && product.price !== null ? product.price : 0,

						// 产品尺寸信息
						productlength: product.productLength || '',
						productwidth: product.productWidth || '',
						productheight: product.productHeight || '',
						productweight: product.productWeight || 0,

						// 包装信息
						mediumpackaging: product.mediumPackaging || 0,
						outerbox: product.outerBox || 0,
						middlebagorouterbox: product.middleBagOrOuterBox || 0,

						// 外箱数据
						outerboxlength: product.outerBoxLength || 0,
						outerboxwidth: product.outerBoxWidth || 0,
						outerboxheight: product.outerBoxHeight || 0,
						outerboxvolume: product.outerBoxVolume || 0,
						outerboxgrossweight: product.outerBoxGrossWeight || 0,

						// 该产品的供应商选项
						supplierOptions: productSupplierOptions
					};

					processedProducts.push(processedProduct);
				}

				productList.value = processedProducts;
				console.log('处理后的产品列表:', productList.value); // 添加调试日志

				// 添加详细的调试信息
				productList.value.forEach((product, index) => {
					console.log(`产品 ${index + 1} 详细信息:`, {
						productName: product.productName,
						supplierID: product.supplierID,
						supplierOptions: product.supplierOptions,
						priceTerms: product.priceTerms,
						availablePriceTerms: optionss.value.hr_purchase_pricing_term
					});
				});
			} else {
				productList.value = [];
			}
		} else {
			inquiryDocumentList.value = [];
		}
	} catch (error) {
		console.error('加载附件列表失败:', error);
		ElMessage.error('加载附件列表失败');
		inquiryDocumentList.value = [];
	}
};

// 附件相关方法
const handleDownload = (file) => {
	// 这里需要根据你的实际情况来处理下载逻辑
	// 如果 documentUrl 是完整的 URL，可以直接使用
	// 如果是相对路径，需要拼接基础 URL
	const baseUrl = 'http://your-api-base-url/'; // 替换为你的 API 基础 URL
	const fullUrl = file.documentUrl.startsWith('http') ? file.documentUrl : baseUrl + file.documentUrl;
	window.open(fullUrl, '_blank');
};

// 附件总量不超过 50M
const ATTACHMENT_TOTAL_MAX = 50 * 1024 * 1024;
const getAttachmentTotalSize = (list) => {
	return (list || []).reduce((sum, f) => sum + (f.raw ? f.raw.size : 0), 0);
};
const handleFileChange = (file, fileList) => {
	const totalSize = getAttachmentTotalSize(fileList);
	if (totalSize > ATTACHMENT_TOTAL_MAX) {
		ElMessage.error('附件总大小不能超过 50M');
		uploadfileList.value = fileList.filter(f => f.uid !== file.uid);
		return;
	}
	if (!file.isExisting) {
		const newFile = {
			fileName: file.name,
			file: file.raw,
			isNew: true,
			remark: '默认备注'
		};
		inquiryDocumentList.value.push(newFile);
	}
	uploadfileList.value = fileList;
};

const handleFileRemove = (file) => {
	if (file.isExisting) {
		// 这是现有文件，从 inquiryDocumentList 中移除
		const index = inquiryDocumentList.value.findIndex(doc => doc.fileName === file.name);
		if (index > -1) {
			inquiryDocumentList.value.splice(index, 1);
		}
	} else {
		// 这是新上传的文件，从 inquiryDocumentList 中移除
		const index = inquiryDocumentList.value.findIndex(doc => doc.fileName === file.name && doc.isNew);
		if (index > -1) {
			inquiryDocumentList.value.splice(index, 1);
		}
	}
	// 更新 uploadfileList
	uploadfileList.value = uploadfileList.value.filter(f => f !== file);
};

const handleDeleteNewFile = (index) => {
	inquryProductDocumentTableData.value.splice(index, 1);
	uploadfileList.value.splice(index, 1);
};

const deleteDocument = async (row, index) => {
	try {
		await ElMessageBox.confirm('确定要删除该附件吗？此操作不可恢复！', '警告', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 如果附件没有 ID，说明是新上传的，直接从本地数组中删除
		if (!row.id) {
			inquiryDocumentList.value.splice(index, 1);
			// 同时从上传文件列表中删除
			uploadfileList.value.splice(index, 1);
			ElMessage({
				type: 'success',
				message: '删除成功'
			});
			return;
		}

		// 如果有 ID，则调用后端接口删除
		const response: any = await request({
			url: 'Inquiry/DeleteInquirySupplementaryDocuments/DeleteInquirySupplementaryDocuments',
			method: 'GET',
			params: {
				Id: row.id
			}
		});

		if (response.code === 200) {
			ElMessage({
				type: 'success',
				message: response.msg
			});
		} else {
			throw new Error(response.msg || '删除失败');
		}
	} catch (error) {
		if (error === 'cancel' || error.message === 'cancel') {
			// 用户取消删除时的提示
			ElMessage({
				type: 'info',
				message: '已取消删除'
			});
		} else {
			// 其他错误的提示
			console.error('删除附件失败:', error);
			ElMessage.error(error.message || '删除失败，请稍后重试');
		}
	}
};

// 页面加载时获取数据
onMounted(() => {
	getInquiryList();

	// 检查是否有从首页传递过来的询价ID参数
	const route = useRoute();
	if (route.query.inquiryId && route.query.fromDashboard) {
		// 在获取列表数据后，需要找到并选中对应ID的行
		nextTick(() => {
			setTimeout(() => {
				const inquiryId = route.query.inquiryId as string;
				const targetInquiry = inquiryList.value.find(item => item.id.toString() == inquiryId.toString());
				if (targetInquiry) {
					// 如果状态是待处理，则自动打开处理窗口
					if (targetInquiry.status === 0) {
						editDetail(targetInquiry);
					} else {
						viewDetail(targetInquiry);
					}
				}
			}, 500); // 给予一定延迟确保数据已加载
		});
	}
});
</script>

<style scoped>
.no-image {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	width: 100px;
	height: 100px;
	border: 1px dashed #dcdfe6;
	border-radius: 4px;
	color: #909399;
}

:deep(.message-box-custom) {
	width: 80%;
	max-width: 800px;
}

/* 产品图片悬停效果 */
.product-image-small {
	cursor: pointer;
}

/* 悬停图片容器 */
.hover-image-container {
	position: fixed;
	z-index: 99999;
	background: white;
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	box-shadow: 0 4px 20px 0 rgba(0, 0, 0, 0.15);
	padding: 6px;
	pointer-events: none;
}

.hover-image-container img {
	width: 189px;
	/* 5cm = 189px (37.8px * 5) */
	height: 189px;
	object-fit: cover;
	border-radius: 2px;
}
</style>