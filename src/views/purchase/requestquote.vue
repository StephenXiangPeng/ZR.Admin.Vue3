<template>
	<div class="request-quote-container">
		<!-- 筛选区域 -->
		<el-card class="filter-card">
			<el-form :inline="true" class="filter-form">
				<el-form-item label="状态">
					<el-radio-group v-model="searchParams.status" @change="searchInquiries">
						<el-radio-button :label="0">待处理</el-radio-button>
						<el-radio-button :label="1">已处理</el-radio-button>
					</el-radio-group>
				</el-form-item>
				<el-form-item label="询价单号">
					<el-input v-model="searchParams.inquiryNumber" placeholder="请输入询价单号" clearable
						@keyup.enter="searchInquiries" />
				</el-form-item>
				<el-form-item label="销售员">
					<el-select v-model="searchParams.salesId" placeholder="请选择销售员" style="width: 150px" clearable
						@change="searchInquiries">
						<el-option v-for="item in optionss.sql_hr_sale" :key="item.dictCode" :label="item.dictLabel"
							:value="item.dictValue" />
					</el-select>
				</el-form-item>
			</el-form>
		</el-card>

		<!-- 列表区域 -->
		<div style="display: flex; min-height: 600px; height: 70vh;">
			<!-- 左侧表格 -->
			<el-card class="list-card"
				style="flex: 1 1 0; min-width: 420px; max-width: 600px; border-right: 1px solid #f0f0f0; padding-right: 12px; display: flex; flex-direction: column; height: 100%;">
				<div style="flex: 1 1 0; min-height: 0; overflow: auto; position: relative;">
					<el-table :data="inquiryList" style="width: 100%;" v-loading="loading" @row-click="handleRowClick"
						highlight-current-row :row-class-name="rowClassName" :max-height="'calc(70vh - 120px)'">
						<el-table-column prop="isDraft" label="草稿" width="80" v-if="false">
							<template #default="scope">
								<el-tag :type="scope.row.isDraft == 1 ? 'info' : 'success'">
									{{ scope.row.isDraft == 1 ? '是' : '否' }}
								</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="status" label="状态" width="100">
							<template #default="scope">
								<el-tag
									:type="Number(scope.row.productIsDraft) == 1 ? 'info' : (scope.row.status === 0 ? 'warning' : 'success')">
									{{ Number(scope.row.productIsDraft) == 1 ? '草稿' : (scope.row.status === 0 ? '待处理' :
										'已处理')
									}}
								</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="inquiryNumber" label="询价单号" width="120" />
						<el-table-column prop="productName" label="产品名称" width="180" v-if="false" />
						<el-table-column prop="salePerson" label="销售员" width="120" />
						<el-table-column label="操作" width="100">
							<template #default="scope">
								<el-button v-if="scope.row.status === 0" type="primary" size="small"
									@click.stop="editDetail(scope.row)">处理</el-button>
								<el-button v-else type="info" size="small"
									@click.stop="viewDetail(scope.row)">查看</el-button>
							</template>
						</el-table-column>
					</el-table>
				</div>
				<div class="pagination-container" style="margin-top: 0;">
					<el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
						:page-sizes="[5, 10, 15, 20]" layout="total, sizes, prev, pager, next, jumper" :total="total"
						@size-change="handleSizeChange" @current-change="handleCurrentChange" />
				</div>
			</el-card>
			<!-- 间隔 -->
			<div style="width:10px;"></div>
			<!-- 右侧详情区 -->
			<el-card
				style="flex: 2 1 0; min-width: 480px; min-height: 560px; height: 100%; display: flex; flex-direction: column;">
				<template #header>
					<span v-if="currentDetail">
						{{ detailEditMode ? '询价单处理' : '询价单详情' }} - 单号: {{ currentDetail.inquiryNumber }}
						<span v-if="Number(currentDetail.productIsDraft) == 1" style="color:#909399;">（草稿）</span>
					</span>
				</template>
				<template v-if="currentDetail">
					<div class="simple-detail-table-wrapper">
						<el-form ref="formRef" :model="currentDetail" :rules="rules">
							<table class="simple-detail-table">
								<tbody>
									<tr>
										<th>询价单号</th>
										<td>{{ currentDetail.inquiryNumber || '无' }}</td>
										<th>产品名称</th>
										<td>{{ currentDetail.productName || '无' }}</td>
										<th>销售员</th>
										<td>{{ currentDetail.salePerson || '无' }}</td>
									</tr>
									<tr>
										<th>采购员</th>
										<td>{{ currentDetail.buyer || '无' }}</td>
										<th>创建人</th>
										<td>{{ currentDetail.create_by || '无' }}</td>
										<th>日期</th>
										<td>{{ currentDetail.date || '无' }}</td>
									</tr>
									<tr>
										<th>状态</th>
										<td>{{ currentDetail.status === 0 ? '待报价' : '已报价' }}</td>
										<th>编号</th>
										<td>{{ currentDetail.productnumber || '无' }}</td>
										<th>规格</th>
										<td>{{ currentDetail.productspecifications || '无' }}</td>
									</tr>
									<tr>
										<th>主要材料</th>
										<td>{{ currentDetail.mainmaterials || '无' }}</td>
										<th>产品长度(CM)</th>
										<td>{{ currentDetail.productlength ?? '无' }}</td>
										<th>产品宽度(CM)</th>
										<td>{{ currentDetail.productwidth ?? '无' }}</td>
									</tr>
									<tr>
										<th>产品高度(CM)</th>
										<td>{{ currentDetail.productheight ?? '无' }}</td>
										<th>产品克重</th>
										<td>{{ currentDetail.productweight ?? '无' }}</td>
										<th>小包装方式</th>
										<td>{{ currentDetail.smallpackagingmethod || '无' }}</td>
									</tr>
									<tr>
										<th>MOQ</th>
										<td>
											<template v-if="detailEditMode">
												<el-form-item prop="moq">
													<el-input v-model="currentDetail.moq" size="default"
														style="width:90px;" />
												</el-form-item>
											</template>
											<template v-else>{{ currentDetail.moq ?? '无' }}</template>
										</td>
										<th>报价数量</th>
										<td>
											<template v-if="detailEditMode">
												<el-form-item prop="QuoteQuantity">
													<el-input v-model="currentDetail.QuoteQuantity" size="default"
														style="width:90px;" />
												</el-form-item>
											</template>
											<template v-else>{{ currentDetail.QuoteQuantity ?? '无' }}</template>
										</td>
										<th>价格</th>
										<td>
											<template v-if="detailEditMode">
												<el-form-item prop="price">
													<el-input v-model="currentDetail.price" size="default"
														style="width:90px;" />
												</el-form-item>
											</template>
											<template v-else>{{ currentDetail.price ?? '无' }}</template>
										</td>
									</tr>
									<tr>
										<th>议价量</th>
										<td>
											<template v-if="detailEditMode">
												<el-input v-model="currentDetail.negotiateprice" size="default"
													style="width:90px;" />
											</template>
											<template v-else>{{ currentDetail.negotiateprice ?? '无' }}</template>
										</td>
										<th>定制</th>
										<td>
											<template v-if="detailEditMode">
												<el-input v-model="currentDetail.custommade" size="default"
													style="width:90px;" />
											</template>
											<template v-else>{{ currentDetail.custommade ?? '无' }}</template>
										</td>
										<th>含税+/-</th>
										<td>
											<template v-if="detailEditMode">
												<el-input v-model="currentDetail.taxincluded" size="default"
													style="width:90px;" />
											</template>
											<template v-else>{{ currentDetail.taxincluded ?? '无' }}</template>
										</td>
									</tr>
									<tr>
										<th>价格条款</th>
										<td>
											<template v-if="detailEditMode">
												<el-select v-model="currentDetail.priceterms" filterable
													style="width:90px;">
													<el-option v-for="opt in optionss.hr_purchase_pricing_term"
														:key="opt.dictValue" :label="opt.dictLabel"
														:value="opt.dictValue" />
												</el-select>
											</template>
											<template v-else>{{ getPriceTermsLabel(currentDetail.priceterms) || '无'
											}}</template>
										</td>
										<th>供应商</th>
										<td colspan="2">
											<template v-if="detailEditMode">
												<el-form-item prop="SupplierID">
													<el-select v-model="currentDetail.SupplierID" filterable
														style="width: 180px;">
														<el-option v-for="item in supplierOptions" :key="item.value"
															:label="item.label" :value="item.value" />
													</el-select>
												</el-form-item>
											</template>
											<template v-else>{{ getSupplierLabel(currentDetail.SupplierID) || '无'
											}}</template>
										</td>
									</tr>
									<tr>
										<th>中包装</th>
										<td>{{ currentDetail.mediumpackaging ?? '无' }}</td>
										<th>外箱</th>
										<td>{{ currentDetail.outerbox ?? '无' }}</td>
										<th>中包/外箱</th>
										<td>{{ currentDetail.middlebagorouterbox ?? '无' }}</td>
									</tr>
									<tr>
										<th>外箱长度(CM)</th>
										<td>{{ currentDetail.outerboxlength ?? '无' }}</td>
										<th>外箱宽度(CM)</th>
										<td>{{ currentDetail.outerboxwidth ?? '无' }}</td>
										<th>外箱高度(CM)</th>
										<td>{{ currentDetail.outerboxheight ?? '无' }}</td>
									</tr>
									<tr>
										<th>外箱体积(m³)</th>
										<td>{{ currentDetail.outerboxvolume ?? '无' }}</td>
										<th>外箱毛重(KGS)</th>
										<td>{{ currentDetail.outerboxgrossweight ?? '无' }}</td>
										<th>是否新产品</th>
										<td>{{ currentDetail.IsNewProduct === 1 ? '是' : '否' }}</td>
									</tr>
									<tr>
										<th>产品图片</th>
										<td>
											<div v-if="currentDetail.productimage">
												<el-image :src="currentDetail.productimage"
													:preview-src-list="[currentDetail.productimage]"
													style="width: 60px; height: 60px; object-fit: contain; border: 1px solid #eee; border-radius: 4px;" />
											</div>
											<span v-else>无图片</span>
										</td>
										<th>备注</th>
										<td colspan="5">
											<el-input v-if="detailEditMode" v-model="currentDetail.Remark"
												type="textarea" :rows="3" style="width: 100%; min-height: 48px;" />
											<span v-else>{{ currentDetail.Remark || '无' }}</span>
										</td>
									</tr>
								</tbody>
							</table>
						</el-form>
						<div style="display: flex; justify-content: flex-end; margin-top: 8px; padding-bottom: 4px;">
							<el-button v-if="detailEditMode" type="warning" size="default"
								@click="saveDetailAsDraft">保存草稿</el-button>
							<el-button v-if="detailEditMode" type="success" size="default"
								@click="saveDetail">提交</el-button>
							<el-button v-if="detailEditMode" size="default" @click="cancelEdit">取消</el-button>
						</div>
					</div>
				</template>
				<div v-else style="color:#aaa;text-align:center;padding:100px 0;">请选择左侧列表中的一条数据</div>
			</el-card>
		</div>

		<!-- 弹窗详情+编辑 -->
		<!-- <el-dialog v-model="dialogVisible" :title="dialogEditMode ? '询价单处理' : '询价单详情'" width="900px"
			@close="closeDialog">
			<el-form :model="dialogForm" label-width="140px" :disabled="!dialogEditMode">
				<el-divider content-position="left">基本信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="询价单号">
							<el-input v-model="dialogForm.inquiryNumber" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="产品名称">
							<el-input v-model="dialogForm.productName" disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="销售员">
							<el-input v-model="dialogForm.salePerson" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="创建时间">
							<el-input v-model="dialogForm.create_time" disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-divider content-position="left">产品信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="产品编码">
							<el-input v-model="dialogForm.productCode" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="产品规格">
							<el-input v-model="dialogForm.chineseSpecification" disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="主要材料">
							<el-input v-model="dialogForm.mainmaterials" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="产品尺寸(CM)">
							<el-input
								:value="`${dialogForm.productlength} × ${dialogForm.productwidth} × ${dialogForm.productheight}`"
								disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="产品克重">
							<el-input v-model="dialogForm.productweight" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="小包装方式">
							<el-input v-model="dialogForm.smallpackagingmethod" disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-divider content-position="left">价格信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="供应商">
							<el-select v-model="dialogForm.SupplierID" filterable clearable :disabled="!dialogEditMode">
								<el-option v-for="item in supplierOptions" :key="item.value" :label="item.label"
									:value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="MOQ">
							<el-input-number v-model="dialogForm.moq" :precision="4" :step="1"
								:disabled="!dialogEditMode" size="small" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="报价数量">
							<el-input-number v-model="dialogForm.QuoteQuantity" :precision="0" :step="1"
								:disabled="!dialogEditMode" size="small" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="价格">
							<el-input-number v-model="dialogForm.price" :precision="4" :step="0.1"
								:disabled="!dialogEditMode" size="large" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="议价量">
							<el-input-number v-model="dialogForm.negotiateprice" :precision="2" :step="0.1"
								:disabled="!dialogEditMode" size="large" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="定制">
							<el-input-number v-model="dialogForm.custommade" :precision="4" :step="0.1"
								:disabled="!dialogEditMode" size="default" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="含税+/-">
							<el-input-number v-model="dialogForm.taxincluded" :precision="4" :step="0.1"
								:disabled="!dialogEditMode" size="default" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="价格条款">
							<el-select v-model="dialogForm.priceterms" filterable :disabled="!dialogEditMode">
								<el-option v-for="dict in optionss.hr_purchase_pricing_term" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-divider content-position="left">包装信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="中包装">
							<el-input-number v-model="dialogForm.mediumpackaging" :precision="4" :step="1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱">
							<el-input-number v-model="dialogForm.outerbox" :precision="4" :step="1" disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="中包/外箱">
							<el-input-number v-model="dialogForm.middlebagorouterbox" :precision="4" :step="0.1"
								disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row :gutter="20">
					<el-col :span="8">
						<el-form-item label="外箱尺寸(CM)">
							<el-input
								:value="`${dialogForm.outerboxlength} × ${dialogForm.outerboxwidth} × ${dialogForm.outerboxheight}`"
								disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱体积(m³)">
							<el-input-number v-model="dialogForm.outerboxvolume" :precision="4" :step="0.001"
								disabled />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱毛重(KGS)">
							<el-input-number v-model="dialogForm.outerboxgrossweight" :precision="4" :step="0.1"
								disabled />
						</el-form-item>
					</el-col>
				</el-row>
				<el-divider content-position="left">其他信息</el-divider>
				<el-row :gutter="20">
					<el-col :span="12">
						<el-form-item label="是否新产品">
							<el-radio-group v-model="dialogForm.IsNewProduct" disabled>
								<el-radio :label="0">否</el-radio>
								<el-radio :label="1">是</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="产品图片">
							<div v-if="dialogForm.productimage">
								<el-image :src="dialogForm.productimage" :preview-src-list="[dialogForm.productimage]"
									style="width: 120px; height: 120px; object-fit: contain; border: 1px solid #eee; border-radius: 4px;" />
							</div>
							<div v-else class="no-image">
								<el-icon>
									<Picture />
								</el-icon>
								<span>暂无图片</span>
							</div>
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item label="备注">
					<el-input v-model="dialogForm.Remark" type="textarea" :rows="3" :disabled="!dialogEditMode" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="closeDialog">取消</el-button>
				<el-button v-if="dialogEditMode" type="primary" @click="saveDialog">保存</el-button>
			</template>
		</el-dialog> -->
	</div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, toRefs, computed, h, nextTick } from 'vue';
import { ElMessage, ElMessageBox, FormInstance } from 'element-plus';
import { Plus, Picture } from '@element-plus/icons-vue';
import request from '@/utils/request'; // 导入请求工具
import { useRoute } from 'vue-router';


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
	// SupplierID 不再必填
};

// 表格引用
const tableRef = ref(null);

// 加载状态
const loading = ref(false);

// 弹窗相关
const dialogVisible = ref(false);
const dialogEditMode = ref(false);
const dialogForm = reactive({});

// 详情区相关
const currentDetail = ref(null);
const detailEditMode = ref(false);

// 字段配置（去掉产品ID）
const allFields = [
	{ label: '询价单号', key: 'inquiryNumber' },
	{ label: '产品名称', key: 'productName' },
	{ label: '销售员', key: 'salePerson' },
	{ label: '采购员', key: 'buyer' },
	{ label: '创建人', key: 'create_by' },
	{ label: '日期', key: 'date' },
	{ label: '状态', key: 'status', formatter: v => v === 0 ? '待报价' : '已报价' },
	{ label: '编号', key: 'productnumber' },
	{ label: '规格', key: 'productspecifications' },
	{ label: '主要材料', key: 'mainmaterials' },
	{ label: '产品长度(CM)', key: 'productlength' },
	{ label: '产品宽度(CM)', key: 'productwidth' },
	{ label: '产品高度(CM)', key: 'productheight' },
	{ label: '产品克重', key: 'productweight' },
	{ label: '小包装方式', key: 'smallpackagingmethod' },
	{ label: 'MOQ', key: 'moq', editable: true, type: 'number' },
	{ label: '报价数量', key: 'QuoteQuantity', editable: true, type: 'number' },
	{ label: '价格', key: 'price', editable: true, type: 'number' },
	{ label: '议价量', key: 'negotiateprice', editable: true, type: 'number' },
	{ label: '定制', key: 'custommade', editable: true, type: 'number' },
	{ label: '含税+/-', key: 'taxincluded', editable: true, type: 'number' },
	{ label: '价格条款', key: 'priceterms', editable: true, type: 'select', options: (optionss as any).value.hr_purchase_pricing_term },
	{ label: '中包装', key: 'mediumpackaging' },
	{ label: '外箱', key: 'outerbox' },
	{ label: '中包/外箱', key: 'middlebagorouterbox' },
	{ label: '外箱长度(CM)', key: 'outerboxlength' },
	{ label: '外箱宽度(CM)', key: 'outerboxwidth' },
	{ label: '外箱高度(CM)', key: 'outerboxheight' },
	{ label: '外箱体积(m³)', key: 'outerboxvolume' },
	{ label: '外箱毛重(KGS)', key: 'outerboxgrossweight' },
	{ label: '是否新产品', key: 'IsNewProduct', formatter: v => v === 1 ? '是' : '否' },
	{ label: '产品图片', key: 'productimage', type: 'image' },
	{ label: '备注', key: 'Remark', type: 'textarea', editable: true }
];

// 详情表格行数据（每行4组）
const detailTableRows = computed(() => {
	const rows = [];
	for (let i = 0; i < allFields.length; i += 4) {
		const row = {};
		for (let j = 0; j < 4; j++) {
			const field = allFields[i + j];
			if (field) {
				row[`label${j + 1}`] = field.label;
				row[`key${j + 1}`] = field.key;
				row[`field${j + 1}`] = field;
			}
		}
		rows.push(row);
	}
	return rows;
});

// 获取供应商标签
const getSupplierLabel = (supplierId) => {
	const supplier = supplierOptions.value.find(item => item.value === supplierId);
	return supplier ? supplier.label : '';
};

// 获取价格条款标签
const getPriceTermsLabel = (priceTerms) => {
	const priceTerm = (optionss as any).value.hr_purchase_pricing_term.find(item => item.dictValue === priceTerms);
	return priceTerm ? priceTerm.dictLabel : '';
};

// 处理编辑
const handleEdit = async (row) => {
	// 获取供应商列表
	if (row.productID) {
		await getSupplierListByProduct(row.productID);
	}
	// 设置编辑状态
	row.editing = true;
	// 保存原始数据
	row.originalData = { ...row };
};

// 处理保存
const handleSave = async (row) => {
	try {
		// 构建请求体
		const processingQuotesRequest = {
			ID: row.id,
			MOQ: row.moq,
			Price: row.price,
			QuoteQuantity: row.QuoteQuantity,
			negotiateprice: row.negotiateprice,
			CustomMade: row.custommade,
			TaxIncluded: row.taxincluded,
			PriceTerms: row.priceterms,
			SupplierID: row.SupplierID,
			Remark: row.Remark,
		};

		const res = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingQuotesRequest
		});

		if (res.code === 200) {
			ElMessage.success(res.msg || '报价提交成功');
			row.editing = false;
			row.status = 1; // 更新状态为已处理
			delete row.originalData;
			getInquiryList();
		} else {
			ElMessage.error(res.msg || '报价提交失败');
		}
	} catch (error) {
		console.error('提交失败', error);
		ElMessage.error('提交失败');
	}
};

// 处理取消
const handleCancel = (row) => {
	// 恢复原始数据
	Object.assign(row, row.originalData);
	row.editing = false;
	delete row.originalData;
};

// 处理查看
const handleView = (row) => {
	// 可以打开一个弹窗显示详细信息
	ElMessageBox.alert(JSON.stringify(row, null, 2), '询价单详情', {
		confirmButtonText: '确定',
		customClass: 'message-box-custom'
	});
};

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
		// 构造详情对象
		const detail = { ...formData };
		return detail;
	} catch (error) {
		console.error('获取询价单详情失败', error);
		ElMessage.error('获取询价单详情失败');
		return null;
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
					handleRowClick(targetInquiry);
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

const openDialog = (row, isEdit) => {
	Object.assign(dialogForm, row);
	dialogVisible.value = true;
	dialogEditMode.value = isEdit;
};
const closeDialog = () => {
	dialogVisible.value = false;
	dialogEditMode.value = false;
};
const saveDialog = async () => {
	// 构建请求体，调用保存接口
	try {
		const processingQuotesRequest = {
			ID: dialogForm.id,
			MOQ: dialogForm.moq,
			Price: dialogForm.price,
			QuoteQuantity: dialogForm.QuoteQuantity,
			negotiateprice: dialogForm.negotiateprice,
			CustomMade: dialogForm.custommade,
			TaxIncluded: dialogForm.taxincluded,
			PriceTerms: dialogForm.priceterms,
			SupplierID: dialogForm.SupplierID,
			Remark: dialogForm.Remark,
		};
		const res = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingQuotesRequest
		});
		if (res.code === 200) {
			ElMessage.success(res.msg || '报价提交成功');
			dialogVisible.value = false;
			getInquiryList();
		} else {
			ElMessage.error(res.msg || '报价提交失败');
		}
	} catch (error) {
		ElMessage.error('提交失败');
	}
};

const handleRowClick = (row) => {
	console.log(row);
	currentDetail.value = {
		...row,
		productIsDraft: row.productIsDraft,
		priceterms: row.priceterms ? row.priceterms.toString() : '' // 确保价格条款是字符串类型
	};
	console.log(currentDetail.value);
	detailEditMode.value = false;
};
const editDetail = async (row) => {
	supplierOptions.value = [];
	currentDetail.value = { ...row, productIsDraft: row.productIsDraft };
	detailEditMode.value = true;
	// 确保价格条款正确绑定
	if (currentDetail.value.priceterms) {
		currentDetail.value.priceterms = currentDetail.value.priceterms.toString();
	}

	// 获取供应商列表
	if (row.productID) {
		try {
			const res = await request({
				url: 'ProductInformation/GetSupplierListByProductID/GetProductSupplierList',
				method: 'get',
				params: {
					productID: row.productID
				}
			});
			if (res.code === 200) {
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
	}
};
const viewDetail = (row) => {
	currentDetail.value = { ...row, productIsDraft: row.productIsDraft };
	detailEditMode.value = false;
};
const saveDetail = async () => {
	if (!formRef.value) return;

	try {
		await formRef.value.validate();
		const detail = currentDetail.value as any;

		// 确保数值类型正确
		const processingQuotesRequest = {
			ID: detail.id,
			MOQ: Number(detail.moq),
			Price: Number(detail.price),
			QuoteQuantity: Number(detail.QuoteQuantity),
			negotiateprice: Number(detail.negotiateprice || 0),
			CustomMade: Number(detail.custommade || 0),
			TaxIncluded: Number(detail.taxincluded || 0),
			PriceTerms: detail.priceterms ? detail.priceterms.toString() : '',
			SupplierID: detail.SupplierID,
			Remark: detail.Remark || '',
			IsDraft: 0,
			Status: 1
		};

		const res = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingQuotesRequest
		});

		if (res.code === 200) {
			ElMessage.success(res.msg || '报价提交成功');
			detailEditMode.value = false;
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
const saveDetailAsDraft = async () => {
	if (!formRef.value) return;

	try {
		const detail = currentDetail.value as any;
		// 确保数值类型正确
		const processingQuotesRequest = {
			ID: detail.id,
			MOQ: Number(detail.moq || 0),
			Price: Number(detail.price || 0),
			QuoteQuantity: Number(detail.QuoteQuantity || 0),
			negotiateprice: Number(detail.negotiateprice || 0),
			CustomMade: Number(detail.custommade || 0),
			TaxIncluded: Number(detail.taxincluded || 0),
			PriceTerms: detail.priceterms ? detail.priceterms.toString() : '',
			SupplierID: detail.SupplierID ? Number(detail.SupplierID) : 0,
			Remark: detail.Remark || '无',
			IsDraft: 1,
			Status: 0
		};

		// 打印请求数据，用于调试
		console.log('保存草稿请求数据:', processingQuotesRequest);

		const res = await request({
			url: 'Inquiry/ProcessingQuotesByBuyer/ProcessingQuotes',
			method: 'post',
			data: processingQuotesRequest
		});

		if (res.code === 200) {
			ElMessage.success(res.msg || '草稿保存成功');
			detailEditMode.value = false;
			// 刷新列表
			await getInquiryList();
			// 重新获取当前详情
			if (currentDetail.value && currentDetail.value.id) {
				const updatedRow = inquiryList.value.find(item => item.id === currentDetail.value.id);
				if (updatedRow) {
					// 保留原有的价格条款等字段值
					currentDetail.value = {
						...updatedRow,
						productIsDraft: updatedRow.productIsDraft,
						priceterms: detail.priceterms, // 保留原有的价格条款值
						moq: detail.moq, // 保留原有的MOQ值
						price: detail.price, // 保留原有的价格值
						QuoteQuantity: detail.QuoteQuantity, // 保留原有的报价数量值
						negotiateprice: detail.negotiateprice, // 保留原有的议价量值
						custommade: detail.custommade, // 保留原有的定制值
						taxincluded: detail.taxincluded, // 保留原有的含税值
						SupplierID: detail.SupplierID, // 保留原有的供应商ID
						Remark: detail.Remark // 保留原有的备注值
					};
				}
			}
		} else {
			ElMessage.error(res.msg || '草稿保存失败');
		}
	} catch (error) {
		console.error('草稿保存失败', error);
		ElMessage.error('草稿保存失败');
	}
};
const cancelEdit = () => {
	detailEditMode.value = false;
	supplierOptions.value = []; // 清空供应商数据
};
const rowClassName = ({ row }) => {
	return currentDetail.value && row.id === currentDetail.value.id ? 'current-row' : '';
};
</script>

<style scoped>
.request-quote-container {
	padding: 20px;
}

.filter-card {
	margin-bottom: 20px;
}

.filter-form {
	display: flex;
	align-items: center;
}

.list-card {
	margin-bottom: 20px;
	display: flex;
	flex-direction: column;
	height: 100%;
	overflow: hidden;
}

.pagination-container {
	margin: 0;
	padding: 6px 0 2px 0;
	background: #fff;
	height: 44px;
	display: flex;
	align-items: center;
	justify-content: flex-end;
	border-top: 1px solid #f0f0f0;
	min-width: none;
}

.list-card>div:first-child {
	flex: 1 1 0;
	min-height: 0;
	overflow: auto;
}

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

.el-table .current-row {
	background: #f5f7fa !important;
}

.form-section-title {
	font-size: 13px;
	color: #888;
	font-weight: bold;
	margin: 8px 0 2px 0;
}

.detail-form .el-form-item {
	margin-bottom: 6px;
}

.detail-field :deep(.el-form-item__label) {
	color: #444;
	font-weight: bold;
	font-size: 14px;
	letter-spacing: 0.5px;
}

.detail-field .field-value {
	color: #888;
	font-size: 13px;
}

.simple-detail-table-wrapper {
	padding: 0 8px 8px 8px;
}

.simple-detail-table {
	width: 100%;
	border-collapse: separate;
	border-spacing: 0;
	background: #fff;
}

.simple-detail-table th {
	background: #f7fafc;
	font-weight: bold;
	color: #444;
	font-size: 14px;
	text-align: left;
	padding: 8px 10px 8px 12px;
	border-bottom: 1px solid #f0f0f0;
	min-width: 90px;
}

.simple-detail-table td {
	background: #fff;
	color: #333;
	font-size: 14px;
	padding: 8px 10px;
	border-bottom: 1px solid #f0f0f0;
}

.simple-detail-table tr:last-child th,
.simple-detail-table tr:last-child td {
	border-bottom: none;
}
</style>