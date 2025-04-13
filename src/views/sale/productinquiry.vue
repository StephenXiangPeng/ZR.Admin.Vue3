<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="OpenCreateInquiryDialog" :close-on-click-modal=false>创建询价单</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<el-input v-model="inquirynumber" clearable style="width: 15%"
				placeholder="输入询价单号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="inquiryStartDate" type="date" placeholder="请选择起始日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="inquiryEndDate" type="date" placeholder="请选择结束日期" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain @click="SearchInquiry">查询</el-button>
				<el-button @click="ResetSearch">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;客户询价表</span>
			<el-divider></el-divider>
			<el-table :data="InquityTableData" style="width: 100%" stripe :size="small">
				<el-table-column prop="isDraft" label="是否草稿" width="100">
					<template #default="scope">
						<el-tag :type="scope.row.isDraft === 0 ? 'warning' : 'success'">
							{{ scope.row.isDraft === 0 ? '否' : '是' }}
						</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="inquiry_number" label="询价单号"></el-table-column>
				<el-table-column prop="date" label="询价日期"></el-table-column>
				<el-table-column prop="subject" label="询价主题"></el-table-column>
				<el-table-column prop="inquirer" label="询价人"></el-table-column>
				<el-table-column prop="shippingDestination" label="送货目的地" v-if="false"></el-table-column>
				<el-table-column fixed="right" prop="operate" label="操作" :width="200">
					<template v-slot:default="scope">
						<el-button link type="primary" size="small" @click="ChcekDetails(scope.row)">查看详情</el-button>
						<el-button link type="danger" size="small" @click="handleDelete(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="SearchInquiryhandlePageChange" :current-page="SearchInquirycurrentPage"
				:page-size="SearchInquirypageSize" :total="SearchInquirytotalItems" background
				layout="prev, pager, next" style="margin-top: 5px;" />
		</div>
		<el-dialog v-model="CreateInquiryDialog" title="创建询价单" :close-on-click-modal=false style="width: 80%;"
			@close="CloseInquiryDialog">
			<el-form label-position="right">
				<el-row>
					<el-col :span="6">
						<el-form-item label="询价单号">
							<el-input v-model="NewprudctInquityDetailsform.inquiry_number" disabled
								style="width: 290px"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="询价主题">
							<el-input v-model="NewprudctInquityDetailsform.Subject" style="width: 290px"
								:disabled="isEditable"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="询价日期">
							<el-date-picker v-model="NewprudctInquityDetailsform.Date" type="date" style="width: 290px"
								:disabled="isEditable"></el-date-picker>
						</el-form-item>
					</el-col>
					<el-col :span="6">
						<el-form-item label="询价人员">
							<el-select filterable v-model="NewprudctInquityDetailsform.Inquirer" placeholder="请选择询价人员"
								:disabled="true" style="width: 290px">
								<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<div style="margin-top: 30px;">
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;产品列表</span>
				</div>
				<el-divider></el-divider>
				<el-button class="mt-4" type="primary" style="margin-bottom: 10px;" @click="OpenSearchProcutDialog"
					:disabled="isEditable">导入产品</el-button>
				<el-button class="mt-4" type="primary" @click="onAddInquiryProductItem" style="margin-bottom: 10px;"
					:disabled="isEditable">空白新增</el-button>
				<el-table :data="inquryProductTableData" height="450" stripe>
					<el-table-column prop="productId" label="产品ID" width="150" align="center" v-if="false">
						<template #default="{ row }">
							<el-input v-model="row.productId" :disabled="isEditable" />
						</template>
					</el-table-column>
					<el-table-column prop="status" label="报价状态" width="100">
						<template #default="scope">
							<el-tag :type="scope.row.status === 0 ? 'warning' : 'success'">
								{{ scope.row.status === 0 ? '待报价' : '已报价' }}
							</el-tag>
						</template>
					</el-table-column>
					<el-table-column prop="date" label="日期" width="150" align="center" />
					<el-table-column prop="productimage" label="询价产品图片" width="150" align="center">
						<template #default="scope">
							<!-- 如果没有图片且可编辑，显示上传按钮 -->
							<template v-if="!scope.row.productimage && !isEditable && scope.row.status !== 1">
								<el-upload :auto-upload="false" :show-file-list="false"
									:on-change="(file) => handleImageSelect(file, scope.$index)" accept="image/*">
									<el-button type="primary" icon="UploadFilled">选择图片</el-button>
								</el-upload>
							</template>
							<!-- 如果没有图片且不可编辑，显示无图片文本 -->
							<template v-else-if="!scope.row.productimage">
								<span>无图片</span>
							</template>
							<!-- 如果有图片，显示预览和删除按钮 -->
							<template v-else>
								<div>
									<el-image style="width: 100px; height: 100px" :src="scope.row.productimage"
										:preview-src-list="[scope.row.productimage]" :zoom-rate="1.2" :max-scale="7"
										:min-scale="0.2" fit="cover" preview-teleported="true">
										<template #error>
											<span>加载失败</span>
										</template>
									</el-image>
									<div v-if="!isEditable && scope.row.status !== 1">
										<el-button type="danger" @click="handleImageDelete(scope.$index)">删除</el-button>
									</div>
								</div>
							</template>
						</template>
					</el-table-column>
					<el-table-column prop="productnumber" label="编号" width="150" align="center">
						<template #default="{ row }">
							<el-input v-model="row.productnumber" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="productspecifications" label="规格" width="150" align="center">
						<template #default="{ row }">
							<el-input v-model="row.productspecifications" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="mainmaterials" label="主要材料" width="150" align="center">
						<template #default="{ row }">
							<el-input v-model="row.mainmaterials" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="smallpackagingmethod" label="小包装方式" width="150" align="center">
						<template #default="{ row }">
							<el-input v-model="row.smallpackagingmethod" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="variousminimumorderquantities" label="各种起订量" width="120" align="center">
						<el-table-column prop="moq" label="MOQ" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.moq" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="negotiateprice" label="议价" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.negotiateprice" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="custommade" label="定制" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.custommade" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
					</el-table-column>
					<el-table-column prop="priceterms" label="价格条款" width="170" align="center">
						<template #default="{ row }">
							<el-select v-model="row.priceterms" filterable placeholder="选择价格条款"
								:disabled="isEditable || row.status == 1">
								<el-option v-for="dict in optionss.
									hr_purchase_pricing_term" :key="dict.dictCode" :label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="taxincluded" label="含税+/-(%)" width="120" align="center">
						<template #default="{ row }">
							<el-input v-model="row.taxincluded" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="QuoteQuantity" label="报价数量" width="120" align="center">
						<template #default="{ row }">
							<el-input v-model="row.QuoteQuantity" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="price" label="价格" width="120" align="center">
						<template #default="{ row }">
							<el-input v-model="row.price" :disabled="isEditable || row.status == 1" />
						</template>
					</el-table-column>
					<el-table-column prop="singleproductsalessize" label="单个产品销售尺寸(CM)" width="120" align="center">
						<el-table-column prop="productlength" label="长" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.productlength" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="productwidth" label="宽" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.productwidth" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="productheight" label="高" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.productheight" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="productweight" label="克重" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.productweight" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
					</el-table-column>
					<el-table-column prop="boxing" label="装箱" width="120" align="center">
						<el-table-column prop="mediumpackaging" label="中包装" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.mediumpackaging" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="outerbox" label="外箱" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerbox" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="middlebagorouterbox" label="中包/外箱" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.middlebagorouterbox" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
					</el-table-column>
					<el-table-column prop="outerboxdata" label="外箱数据(CM)" width="120" align="center">
						<el-table-column prop="outerboxlength" label="长" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerboxlength" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="outerboxwidth" label="宽" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerboxwidth" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="outerboxheight" label="高" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerboxheight" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="outerboxvolume" label="体积m³" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerboxvolume" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
						<el-table-column prop="outerboxgrossweight" label="毛重KGS" width="120" align="center">
							<template #default="{ row }">
								<el-input v-model="row.outerboxgrossweight" :disabled="isEditable || row.status == 1" />
							</template>
						</el-table-column>
					</el-table-column>
					<el-table-column fixed="right" label="操作" min-width="120">
						<template #default="scope">
							<el-button v-if="scope.row.status !== 1" type="danger" size="small"
								@click.prevent="deleteProduct(scope.row, scope.$index)" :disabled="isEditable">
								删除
							</el-button>
						</template>
					</el-table-column>
				</el-table>
				<!-- 图片预览的对话框 -->
				<el-dialog v-model="previewVisible" width="30%" :before-close="() => previewVisible = false">
					<img :src="previewImage" alt="preview" style="width: 100%;" />
				</el-dialog>
				<div style="margin-top: 30px;">
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;询价单附件</span>
				</div>
				<el-divider></el-divider>
				<!-- 附件部分 -->
				<div>
					<!-- 显示现有附件列表 -->
					<div>
						<h3>附件列表:</h3>
						<el-table :data="inquiryDocumentList" stripe>
							<el-table-column prop="fileName" label="文件名"></el-table-column>
							<el-table-column label="操作">
								<template #default="scope">
									<el-button link type="primary" size="small" @click="handleDownload(scope.row)"
										:disabled="isEditable">
										下载
									</el-button>
									<el-button link type="danger" size="small"
										@click="deleteDocument(scope.row, scope.$index)" :disabled="isEditable">
										<!-- 注意这里改为在查看模式下才能删除 -->
										删除
									</el-button>
								</template>
							</el-table-column>
						</el-table>
					</div>

					<!-- 上传新附件的组件 -->
					<div v-if="!isEditable">
						<h3>上传新附件:</h3>
						<el-upload :auto-upload="false" :on-change="handleFileChange" :on-remove="handleFileRemove"
							:file-list="uploadfileList" multiple>
							<template #trigger>
								<el-button type="primary">选取文件</el-button>
							</template>
						</el-upload>
					</div>

					<!-- 显示新上传的附件列表 -->
					<div v-if="!isEditable && inquryProductDocumentTableData.length > 0">
						<h3>新上传的附件:</h3>
						<el-table :data="inquryProductDocumentTableData">
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
				<el-row>
					<el-col :span="30" style="width: 100%;">
						<el-form-item label="询价详情">
							<el-input type="textarea" v-model="NewprudctInquityDetailsform.Description"
								style="width: 100%" :disabled="isEditable"
								:autosize="{ minRows: 5, maxRows: 10 }"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-show="isEditBtnVisible" type="primary" @click="IsEditBtnClick">
						编辑
					</el-button>
					<el-button v-show="isEditSaveBtnVisible" type="primary" @click="EditSaveInquiry">
						编辑保存
					</el-button>
					<el-button v-show="isSavebtnVisible" type="primary" @click="uploadFilesAndSaveInquiry">
						保存草稿
					</el-button>
					<el-button v-show="isSubmitbtnVisible" type="success" @click="SubmitInquiry">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="SearchProcutDialog" title="选择产品" :close-on-click-modal=false :width="'50%'">
			<el-input v-model="searchProductNameText" placeholder="请输入产品关键字进行搜索" style="margin-bottom: 10px;"
				@input="searchProductNameTextChange" />
			<el-table :data="productDatatwo" style="width: 100%"
				:default-sort="{ prop: 'productCode', order: 'descending' }" @row-dblclick="handleRowDblClick" stripe>
				<el-table-column prop="productCode" label="产品编号" sortable width="120" />
				<el-table-column prop="chineseProductName" label="中文品名" width="150" />
				<el-table-column prop="englishProductName" label="英文品名" width="180" />
				<el-table-column prop="chineseSpecification" label="中文规格" width="150" />
				<el-table-column prop="englishSpecification" label="英文规格" width="180" />
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
import { UploadFilled } from '@element-plus/icons-vue'
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessageBox, UploadProps, UploadUserFile, ElMessage, UploadFile } from 'element-plus'
import request from '@/utils/request';
import { create, get } from 'sortablejs';
import dayjs from 'dayjs';
import useUserStore from "@/store/modules/user";

//获取当前登录用户ID
var userId = useUserStore().userId;

//查找产品窗体
const SearchProcutDialog = ref(false)
const inquryProductDocumentTableData = ref([]); //询价单附件列表

//查询产品信息表格
const productDatatwo = ref([])
//报价产品表格
const productData = ref([])
//分页组件
const SearchProducttotalItems = ref(0);
const SearchProductCurrentPage = ref(1);
const SearchProductpageSize = ref(10);
const searchProductNameText = ref('');
const SearchProducthandlePageChange = async (newPage) => {
	SearchProductCurrentPage.value = newPage; // 修改这里，直接使用newPage作为当前页
	await GetProductInfoList(newPage, SearchProductpageSize.value);
};
GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
//获取产品信息列表
function GetProductInfoList(start, end) {
	return new Promise((resolve, reject) => {
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
				SearchProducttotalItems.value = response.data.totalNum;
				SearchProductCurrentPage.value = response.data.pageIndex;
				productDatatwo.value.forEach(item => {
					item.unitOfMeasurement = state.optionss['hr_calculate_unit'].filter(hr_calculate_unit => hr_calculate_unit.dictValue == item.unitOfMeasurement).map(item => item.dictLabel).values().next().value;
				});
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					productDatatwo.value = [];
				}
				reject(new Error('无数据'));
			}
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});
}

const searchProductNameTextChange = () => {
	GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
}

const handleRowDblClick = (row) => {
	const existingProduct = inquryProductTableData.value.find(p => p.productnumber === row.productCode);
	if (existingProduct) {
		ElMessage.error("产品【" + row.chineseProductName + "】已存在报价单的产品列表中，请重新选择");
		return;
	} else {
		inquryProductTableData.value.push({
			productId: row.id,
			date: dayjs().format('YYYY-MM-DD'),
			productimage: '',
			productsupplementarydocuments: '',
			productnumber: row.productCode,
			productspecifications: row.chineseSpecification,
			mainmaterials: '',
			smallpackagingmethod: '',
			moq: 0,
			negotiateprice: 0,
			custommade: 0,
			priceterms: '',
			taxincluded: '',
			QuoteQuantity: 0,
			price: 0,
			productlength: row.productLength ?? 0,
			productwidth: row.productWidth ?? 0,
			productheight: row.productHeight ?? 0,
			productweight: row.productWeight ?? 0,
			mediumpackaging: row.mediumPackagingVolume ?? 0,
			outerbox: row.outerBoxPackingQuantity ?? 0,
			middlebagorouterbox: row.middlebagorouterbox ?? 0,
			outerboxlength: row.outerBoxLength ?? 0,
			outerboxwidth: row.outerBoxWidth ?? 0,
			outerboxheight: row.outerBoxHeight ?? 0,
			outerboxvolume: row.outerBoxVolume ?? 0,
			outerboxgrossweight: row.outerBoxGrossWeight ?? 0,
			status: 0
		});
		SearchProcutDialog.value = false;
	}
};

const isImportProduct = ref(false);
const OpenSearchProcutDialog = () => {
	isImportProduct.value = true;
	GetProductInfoList(SearchProductCurrentPage.value, SearchProductpageSize.value);
	SearchProcutDialog.value = true;
}

const OpenCreateInquiryDialog = () => {
	isEditable.value = false;
	isShowUpload.value = true;
	isEditBtnVisible.value = false;
	isSavebtnVisible.value = true;
	isEditSaveBtnVisible.value = false;

	NewprudctInquityDetailsform.Id = 0;
	NewprudctInquityDetailsform.inquiry_number = '';
	NewprudctInquityDetailsform.Subject = '';
	NewprudctInquityDetailsform.Date = dayjs().format('YYYY-MM-DD');
	NewprudctInquityDetailsform.Inquirer = userId.toString();
	NewprudctInquityDetailsform.Description = '';
	NewprudctInquityDetailsform.InquiryProducts = [];
	NewprudctInquityDetailsform.InquirySupplementaryDocuments = [];

	inquryProductTableData.value = [];
	selectedImages.value = [];
	uploadfileList.value = [];

	CreateInquiryDialog.value = true;
	GetNextInquiryNumber();
}

//获取询价单最新编号
const GetNextInquiryNumber = () => {
	request({
		url: 'Inquiry/GetNextInquiryNumber/GetInquiryNumber',
		method: 'GET'
	}).then((response: any) => {
		if (response.code == "200") {
			NewprudctInquityDetailsform.inquiry_number = response.data;
		} else {
			ElMessage.error("获取客户编号失败，请重新打开客户建档窗体");
		}
	}).catch(error => {
		console.error(error);
	});
}


//询价产品列表
const inquryProductTableData = ref([]);
const now = new Date()
const onAddInquiryProductItem = () => {
	now.setDate(now.getDate())
	inquryProductTableData.value.push({
		date: dayjs(now).format('YYYY-MM-DD'),
		productimage: '',
		productsupplementarydocuments: '',
		productnumber: '',
		productspecifications: '',
		mainmaterials: '',
		smallpackagingmethod: '',
		moq: 0,
		negotiateprice: 0,
		custommade: 0,
		priceterms: '',
		taxincluded: '',
		QuoteQuantity: 0,
		price: 0,
		productlength: 0,
		productwidth: 0,
		productheight: 0,
		productweight: 0,
		mediumpackaging: 0,
		outerbox: 0,
		middlebagorouterbox: 0,
		outerboxlength: 0,
		outerboxwidth: 0,
		outerboxheight: 0,
		outerboxvolume: 0,
		outerboxgrossweight: 0,
		status: 0
	})
}
const selectedImages = ref([]); // 存储用户选择的图片文件
const previewImage = ref(''); // 存储要预览的图片
const previewVisible = ref(false); // 控制预览对话框的显示

const handleImageSelect = (event, index) => {
	if (isEditable.value) return; // 如果不可编辑，直接返回
	const file = event.raw || event; // 兼容不同的事件对象格式
	if (!file) {
		console.error('No file selected');
		ElMessage.error('请选择图片文件');
		return;
	}
	const reader = new FileReader();
	reader.onload = (e) => {
		inquryProductTableData.value[index].productimage = e.target.result;
		selectedImages.value[index] = file;
	};
	reader.readAsDataURL(file);
};

// 删除图片的处理函数
const handleImageDelete = (index) => {
	if (isEditable.value) return; // 如果不可编辑，直接返回
	inquryProductTableData.value[index].productimage = ''; // 清空图片
	selectedImages.value[index] = null; // 清空已选择的文件
};

const isEditable = ref(true);
/*创建询价单Dialog中的Button*/
const isEditBtnVisible = ref(false);
const isEditSaveBtnVisible = ref(false);
const isSavebtnVisible = ref(true);
const isSubmitbtnVisible = ref(false);
/*创建询价单Dialog中的Button*/

const inquirynumber = ref('')
const inquiryStartDate = ref('')
const inquiryEndDate = ref('')

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
		sql_product: [],
		hr_packing: [],
		hr_purchase_pricing_term: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_hr_customer' }, { dictType: 'hr_ourcompany' }, { dictType: 'hr_quotation_status' }, { dictType: 'hr_export_currency' },
{ dictType: 'hr_settlement_way' }, { dictType: 'hr_pricing_term' }, { dictType: 'hr_nation' }, { dictType: 'sql_hr_sale' }, { dictType: 'hr_transport_port' },
{ dictType: 'hr_transportation_method' }, { dictType: 'sys_yes_no' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_contract_status' },
{ dictType: 'hr_customer_level' }, { dictType: 'hr_signing_place' }, { dictType: 'hr_quotation_basis' }, { dictType: 'hr_outerbox_unit' }, { dictType: 'sql_product' },
{ dictType: 'hr_packing' }, { dictType: 'hr_purchase_pricing_term' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
})
/*动态下拉框end*/
const CreateInquiryDialog = ref(false)
const NewprudctInquityDetailsform = reactive({
	Id: 0,
	inquiry_number: '',
	Subject: '',
	Date: '',
	Inquirer: '',
	Description: '',
	InquiryProducts: [],
	InquirySupplementaryDocuments: [],
	isDraft: 0
})


const uploadRef = ref(null);
const uploadData = ref({}); // 这里定义上传时需要附带的数据
const uploadfileList = ref([]);//询价单附件列表
// 处理文件改变事件
const handleFileChange = (file, fileList) => {
	if (!file.isExisting) {
		// 这是新上传的文件
		const newFile = {
			fileName: file.name,
			file: file.raw,
			isNew: true,
			remark: '默认备注' // 添加默认备注
		};
		inquiryDocumentList.value.push(newFile);
	}
	// 更新 uploadfileList
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

// 上传图片到本地服务器
const uploadImageToLocal = async (file) => {
	if (!file) {
		console.error('No file to upload');
		ElMessage.error('没有可上传的图片文件');
		return null;
	}
	const formData = new FormData()
	formData.append('File', file)
	formData.append('FileDir', 'Inquiry/ProductImages')
	formData.append('FileNameType', '1')
	formData.append('FileName', file.name)

	try {
		const response = await request.post('Common/UploadFile', formData, {
			params: { storeType: 1 }, // 1 表示保存到本地
			headers: { 'Content-Type': 'multipart/form-data' }
		})
		if (response.code === 200) {
			return response.data.url // 返回上传后的图片地址
		} else {
			throw new Error(response.msg || '上传失败')
		}
	} catch (error) {
		console.error('图片上传失败:', error)
		ElMessage.error('图片上传失败: ' + (error.message || '未知错误'))
		return null
	}
}

// 上传附件到阿里云
const uploadFileToAliyun = async (file) => {
	if (!file) {
		console.error('No file to upload');
		ElMessage.error('没有可上传的附件文件');
		return null;
	}
	const formData = new FormData()
	formData.append('File', file)
	formData.append('FileDir', 'Inquiry/InquirySupplementaryDocuments')
	formData.append('FileNameType', '1')
	formData.append('FileName', file.name)

	try {
		const response = await request.post('Common/UploadFile', formData, {
			params: { storeType: 2 }, // 2 表示保存到阿里云
			headers: { 'Content-Type': 'multipart/form-data' }
		})
		if (response.code === 200) {
			return {
				fileName: response.data.fileName,
				documentUrl: response.data.downloadurl // 返回上传后的附件地址
			}
		} else {
			throw new Error(response.msg || '上传失败')
		}
	} catch (error) {
		console.error('附件上传失败:', error)
		ElMessage.error('附件上传失败: ' + (error.message || '未知错误'))
		return null
	}
}
var UploadResponse = ref(null);
// 保存询价单
const uploadFilesAndSaveInquiry = async () => {
	// 添加询价主题和询价人员的验证
	if (!NewprudctInquityDetailsform.Subject?.trim()) {
		ElMessage.error('请填写询价主题');
		return;
	}
	if (!NewprudctInquityDetailsform.Inquirer) {
		ElMessage.error('请选择询价人员');
		return;
	}
	// 添加价格条款和含税校验
	const invalidProduct = inquryProductTableData.value.find(product =>
		!product.priceterms || !product.taxincluded
	);
	ElMessageBox.confirm('确定保存该询价单吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		// 上传产品图片（每个产品只有一个图片）
		for (let i = 0; i < inquryProductTableData.value.length; i++) {
			const product = inquryProductTableData.value[i]
			if (selectedImages.value[i]) {
				const imageUrl = await uploadImageToLocal(selectedImages.value[i])
				if (imageUrl) {
					product.productimage = imageUrl
				} else {
					throw new Error(`产品 ${i + 1} 图片上传失败`)
				}
			}
		}

		// 上传询价单附件
		NewprudctInquityDetailsform.InquirySupplementaryDocuments = [];
		for (const fileItem of uploadfileList.value) {
			const result = await uploadFileToAliyun(fileItem.raw);
			if (result) {
				NewprudctInquityDetailsform.InquirySupplementaryDocuments.push({
					FileName: result.fileName,
					DocumentUrl: result.documentUrl,
					Remark: fileItem.remark || ''
				});
			} else {
				throw new Error(`附件 ${fileItem.name} 上传失败`);
			}
		}

		// 保存询价单
		NewprudctInquityDetailsform.InquiryProducts = inquryProductTableData.value
		NewprudctInquityDetailsform.InquiryProducts.forEach(item => {
			item.priceterms = item.priceterms ? item.priceterms : 0
			item.taxincluded = item.taxincluded ? item.taxincluded : 0
		})
		NewprudctInquityDetailsform.isDraft = 1;
		try {
			const response = await request.post('Inquiry/AddInquiry/Add', NewprudctInquityDetailsform)
			if (response.code === 200) {
				ElMessage({
					message: response.msg,
					type: 'success'
				})
				CreateInquiryDialog.value = false
				GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value)
			} else {
				throw new Error(response.msg || '新增询价单失败')
			}
		} catch (error) {
			console.error('新增询价单出错:', error)
			ElMessage.error(error.message || '新增询价单失败')
		}
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		})
	})
};

//询价单列表
const InquityTableData = ref([])
//分页组件
const SearchInquirytotalItems = ref(0);
const SearchInquirycurrentPage = ref(1);
const SearchInquirypageSize = ref(10);
const searchInquiryNameText = ref('');
const SearchInquiryhandlePageChange = async (newPage) => {
	SearchInquirypageSize.value = newPage;
	const start = newPage;
	const end = SearchInquirypageSize.value;
	const newData = await GetInquiryList(start, end);
};
//GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
function GetInquiryList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'Inquiry/GetInquiryList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				InquiryCode: inquirynumber.value,
				startDate: inquiryStartDate.value,
				endDate: inquiryEndDate.value
			}
		}).then(response => {
			if (response.data.result.length) {
				InquityTableData.value = response.data.result;
				InquityTableData.value.forEach(item => {
					item.inquirer = state.optionss.sql_hr_sale.find(option => option.dictValue === item.inquirer.toString()).dictLabel;
				});
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetInquiryList(start - 1, end);
				} else {
					InquityTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const EditInquiryID = ref(0)
const isShowUpload = ref(true) // 是否显示上传组件
const inquiryDocumentList = ref([]) // 询价单附件列表

// 查看询价单详情
const ChcekDetails = (row) => {
	isEditable.value = true;
	if (row.isDraft === 1) {
		isEditBtnVisible.value = true;
		isSavebtnVisible.value = false;
		isEditSaveBtnVisible.value = false;
		isSubmitbtnVisible.value = true;
	} else {
		isEditBtnVisible.value = false;
		isEditSaveBtnVisible.value = false;
		isSavebtnVisible.value = false;
		isSubmitbtnVisible.value = false;
	}
	EditInquiryID.value = row.id;
	NewprudctInquityDetailsform.Id = row.id;
	NewprudctInquityDetailsform.inquiry_number = row.inquiry_number;
	NewprudctInquityDetailsform.Subject = row.subject;
	NewprudctInquityDetailsform.Date = row.date;
	NewprudctInquityDetailsform.Inquirer = row.inquirer;
	NewprudctInquityDetailsform.Description = row.description;
	CreateInquiryDialog.value = true;
	isShowUpload.value = false

	request({
		url: 'Inquiry/GetInquiryProductsAndDocuments/GetInquiryProductsAndDocuments',
		method: 'GET',
		params: {
			InqueryID: row.id
		}
	}).then(response => {
		if (response.code === 200 && response.data) {
			if (response.data.products && response.data.products.length > 0) {
				inquryProductTableData.value = response.data.products.map(item => ({
					id: item.id,
					date: item.date,
					productId: item.productID,
					productimage: item.productImage,
					productnumber: item.productNumber,
					productspecifications: item.productSpecifications,
					mainmaterials: item.mainMaterials,
					smallpackagingmethod: item.smallPackagingMethod,
					moq: item.moq,
					negotiateprice: item.negotiateprice,
					custommade: item.customMade,
					priceterms: item.priceTerms == 0 ? 0 : state.optionss.hr_purchase_pricing_term.find(option => option.dictValue === item.priceTerms.toString()).dictValue,
					taxincluded: item.taxIncluded,
					price: item.price,
					QuoteQuantity: item.quoteQuantity,
					productlength: item.productLength,
					productwidth: item.productWidth,
					productheight: item.productHeight,
					productweight: item.productWeight,
					mediumpackaging: item.mediumPackaging,
					outerbox: item.outerBox,
					middlebagorouterbox: item.middleBagOrOuterBox,
					outerboxlength: item.outerBoxLength,
					outerboxwidth: item.outerBoxWidth,
					outerboxheight: item.outerBoxHeight,
					outerboxvolume: item.outerBoxVolume,
					outerboxgrossweight: item.outerBoxGrossWeight,
					status: item.status
				}))
			} else {
				inquryProductTableData.value = [];
				originalProductData.value = [];
			}

			if (response.data.documents && response.data.documents.length > 0) {
				inquiryDocumentList.value = response.data.documents.map(item => ({
					id: item.id,
					fileName: item.fileName,
					documentUrl: item.documentUrl
				}))
			} else {
				inquiryDocumentList.value = []
				originalDocumentData.value = [];
			}
		} else {
			ElMessage.error(response.msg || '获取询价单详情失败');
		}
	}).catch(error => {
		console.error(error);
		ElMessage.error('获取询价单详情失败');
	});
}

// 下载询价单附件
const handleDownload = (file) => {
	// 这里需要根据你的实际情况来处理下载逻辑
	// 如果 documentUrl 是完整的 URL，可以直接使用
	// 如果是相对路径，需要拼接基础 URL
	const baseUrl = 'http://your-api-base-url/'; // 替换为你的 API 基础 URL
	const fullUrl = file.documentUrl.startsWith('http') ? file.documentUrl : baseUrl + file.documentUrl;
	window.open(fullUrl, '_blank');
}

const existingDocuments = ref([]) // 存储已有的附件

const IsEditBtnClick = () => {
	isEditable.value = false
	isEditBtnVisible.value = false
	isEditSaveBtnVisible.value = true
	isSavebtnVisible.value = false
	isShowUpload.value = true // 显示上传组件
	// 将现有附件信息复制到 existingDocuments
	existingDocuments.value = [...inquiryDocumentList.value]
	// 初始化 uploadfileList 以显示现有附件
	uploadfileList.value = existingDocuments.value.map(doc => ({
		name: doc.fileName,
		url: doc.documentUrl,
		// 添加一个标志来表示这是现有文件
		isExisting: true
	}))
	// 清空 inquryProductDocumentTableData，为新上传做准备
	inquryProductDocumentTableData.value = [];
}

const CloseInquiryDialog = () => {
	isEditable.value = false;
	isEditBtnVisible.value = false;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = true;
	NewprudctInquityDetailsform.inquiry_number = '';
	NewprudctInquityDetailsform.Subject = '';
	NewprudctInquityDetailsform.Date = '';
	NewprudctInquityDetailsform.Inquirer = '';
	NewprudctInquityDetailsform.Description = '';
	inquryProductTableData.value = [];
	CreateInquiryDialog.value = false;
	isShowUpload.value = true
}

const handleDeleteNewFile = (index: number) => {
	inquryProductDocumentTableData.value.splice(index, 1)
	uploadfileList.value.splice(index, 1)
}

const originalProductData = ref([]);
const originalDocumentData = ref([]);
const EditSaveInquiry = async () => {
	try {
		// 添加询价主题和询价人员的验证
		if (!NewprudctInquityDetailsform.Subject?.trim()) {
			ElMessage.error('请填写询价主题');
			return;
		}
		if (!NewprudctInquityDetailsform.Inquirer) {
			ElMessage.error('请选择询价人员');
			return;
		}
		// 添加价格条款和含税校验
		// const invalidProduct = inquryProductTableData.value.find(product =>
		// 	!product.priceterms || !product.taxincluded
		// );
		// if (invalidProduct) {
		// 	if (!invalidProduct.priceterms) {
		// 		ElMessage.error('请为所有产品选择价格条款');
		// 	} else if (!invalidProduct.taxincluded) {
		// 		ElMessage.error('请为所有产品填写含税信息');
		// 	}
		// 	return;
		// }
		try {
			await ElMessageBox.confirm('确定编辑该询价单吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			});
		} catch (err) {
			// 用户点击取消按钮
			ElMessage({
				type: 'info',
				message: '已取消保存'
			});
			return; // 直接返回，不执行后续操作
		}

		// 准备要发送到服务器的数据
		const dataToSend = {
			...NewprudctInquityDetailsform,
			InquiryProducts: [],
			InquirySupplementaryDocuments: []
		};

		// 处理产品数据
		for (let i = 0; i < inquryProductTableData.value.length; i++) {
			const product = inquryProductTableData.value[i];
			// 如果有新的图片，先上传
			if (selectedImages.value[i]) {
				const imageUrl = await uploadImageToLocal(selectedImages.value[i]);
				if (imageUrl) {
					product.productimage = imageUrl;
				}
			}
			// 添加到要发送的数据中
			dataToSend.InquiryProducts.push({
				ID: product.id || 0, // 如果是新产品，ID 为 0
				ProductID: product.productId,
				Date: product.date,
				ProductImage: product.productimage,
				ProductNumber: product.productnumber,
				ProductSpecifications: product.productspecifications,
				MainMaterials: product.mainmaterials,
				SmallPackagingMethod: product.smallpackagingmethod,
				MOQ: product.moq,
				negotiateprice: product.negotiateprice,
				CustomMade: product.custommade,
				PriceTerms: product.priceterms ? product.priceterms : 0,
				TaxIncluded: product.taxincluded ? product.taxincluded : 0,
				QuoteQuantity: product.QuoteQuantity,
				Price: product.price,
				ProductLength: product.productlength,
				ProductWidth: product.productwidth,
				ProductHeight: product.productheight,
				ProductWeight: product.productweight,
				MediumPackaging: product.mediumpackaging,
				OuterBox: product.outerbox,
				MiddleBagOrOuterBox: product.middlebagorouterbox,
				OuterBoxLength: product.outerboxlength,
				OuterBoxWidth: product.outerboxwidth,
				OuterBoxHeight: product.outerboxheight,
				OuterBoxVolume: product.outerboxvolume,
				OuterBoxGrossWeight: product.outerboxgrossweight,
				IsNewProduct: product.id ? 0 : 1 // 如果有 id 则不是新产品
			});
		}

		// 处理附件数据
		for (const document of inquiryDocumentList.value) {
			if (document.isNew) {
				// 新上传的文件
				const result = await uploadFileToAliyun(document.file);
				if (result) {
					dataToSend.InquirySupplementaryDocuments.push({
						ID: 0,
						FileName: result.fileName,
						DocumentUrl: result.documentUrl,
						Remark: document.remark || ''
					});
				}
			} else {
				// 已存在的文件
				dataToSend.InquirySupplementaryDocuments.push({
					ID: document.id,
					FileName: document.fileName,
					DocumentUrl: document.documentUrl,
					Remark: document.remark || ''
				});
			}
		}

		// 发送编辑请求
		const response = await request.post('Inquiry/EditInquiry/Edit', dataToSend);

		if (response.code === 200) {
			ElMessage({
				message: response.msg,
				type: 'success'
			});
			// 更新状态
			isEditable.value = true;
			isEditBtnVisible.value = true;
			isEditSaveBtnVisible.value = false;
			isSavebtnVisible.value = false;
			isShowUpload.value = false;

			// 恢复到待编辑状态
			restoreToViewMode();

			// 刷新询价单列表
			GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);

			// 更新原始数据
			originalProductData.value = JSON.parse(JSON.stringify(inquryProductTableData.value));
			originalDocumentData.value = JSON.parse(JSON.stringify(inquiryDocumentList.value));

			// 关闭编辑对话框
			//CreateInquiryDialog.value = false;
		} else {
			throw new Error(response.msg || '编辑询价单失败');
		}
	} catch (error) {
		console.error('保存已编辑的询价单出错！😔错误内容：', error);
		ElMessage.error(error.message || '保存失败,请稍后重试');
	}
}

// 新增一个函数来恢复到待编辑状态
const restoreToViewMode = () => {
	isEditable.value = true;
	isEditBtnVisible.value = true;
	isEditSaveBtnVisible.value = false;
	isSavebtnVisible.value = false;
	isShowUpload.value = false;

	// 恢复附件列表显示
	uploadfileList.value = inquiryDocumentList.value.map(doc => ({
		name: doc.fileName,
		url: doc.documentUrl,
		isExisting: true
	}));

	// 禁用所有输入字段
	// 注意：这里假设您的模板中使用了 v-bind:disabled="isEditable" 来控制输入字段的禁用状态
	// 如果没有，您需要在这里添加代码来禁用所有相关的输入字段

	// 隐藏编辑相关的按钮或组件
	// 例如：隐藏文件上传组件，显示文件列表等
	// 这部分可能需要根据您的具体UI设计来调整
}

const SearchInquiry = () => {
	GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
}

const ResetSearch = () => {
	inquirynumber.value = '';
	inquiryStartDate.value = '';
	inquiryEndDate.value = '';
	GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
}

const handleDelete = (row) => {
	ElMessageBox.confirm('确定要删除该询价单吗？此操作不可恢复！', '警告', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'Inquiry/DeleteInquiry/DeleteInquiry',
			method: 'GET',
			params: {
				Id: row.id
			}
		}).then(response => {
			if (response.code === 200) {
				ElMessage({
					type: 'success',
					message: response.msg
				});
				// 刷新列表
				GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
			} else {
				ElMessage.error(response.msg || '删除失败');
			}
		}).catch(error => {
			console.error('删除询价单失败:', error);
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除'
		});
	});
};

const deleteProduct = async (row, index) => {
	try {
		await ElMessageBox.confirm('确定要删除该产品吗？此操作不可恢复！', '警告', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		// 如果产品没有 ID，说明是新添加的，直接从本地数组中删除
		if (!row.id) {
			inquryProductTableData.value.splice(index, 1);
			ElMessage({
				type: 'success',
				message: '删除成功'
			});
			return;
		}

		// 如果有 ID，则调用后端接口删除
		const response = await request({
			url: 'Inquiry/DeleteInquiryProducts/DeleteInquiryProducts',
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
			// 只重新加载产品列表
			await loadProductList(EditInquiryID.value);
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
			console.error('删除产品失败:', error);
			ElMessage.error(error.message || '删除失败，请稍后重试');
		}
	}
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
		const response = await request({
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
			// 只重新加载附件列表
			await loadDocumentList(EditInquiryID.value);
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

// 添加两个新的方法来分别加载产品列表和附件列表
const loadProductList = async (inquiryId) => {
	try {
		const response = await request({
			url: 'Inquiry/GetInquiryProductsAndDocuments/GetInquiryProductsAndDocuments',
			method: 'GET',
			params: {
				InqueryID: inquiryId
			}
		});

		if (response.code === 200 && response.data) {
			if (response.data.products && response.data.products.length > 0) {
				inquryProductTableData.value = response.data.products.map(item => ({
					id: item.id,
					date: item.date,
					productid: item.productID,
					productimage: item.productImage,
					productnumber: item.productNumber,
					productspecifications: item.productSpecifications,
					mainmaterials: item.mainMaterials,
					smallpackagingmethod: item.smallPackagingMethod,
					moq: item.moq,
					negotiateprice: item.negotiateprice,
					custommade: item.customMade,
					priceterms: item.priceTerms == 0 ? 0 : state.optionss.hr_purchase_pricing_term.find(option => option.dictValue === item.priceTerms.toString())?.dictValue,
					taxincluded: item.taxIncluded,
					price: item.price,
					QuoteQuantity: item.quoteQuantity,
					productlength: item.productLength,
					productwidth: item.productWidth,
					productheight: item.productHeight,
					productweight: item.productWeight,
					mediumpackaging: item.mediumPackaging,
					outerbox: item.outerBox,
					middlebagorouterbox: item.middleBagOrOuterBox,
					outerboxlength: item.outerBoxLength,
					outerboxwidth: item.outerBoxWidth,
					outerboxheight: item.outerBoxHeight,
					outerboxvolume: item.outerBoxVolume,
					outerboxgrossweight: item.outerBoxGrossWeight,
					status: item.status
				}));
			} else {
				inquryProductTableData.value = [];
			}
		}
	} catch (error) {
		console.error('加载产品列表失败:', error);
		ElMessage.error('加载产品列表失败');
	}
};

const loadDocumentList = async (inquiryId) => {
	try {
		const response = await request({
			url: 'Inquiry/GetInquiryProductsAndDocuments/GetInquiryProductsAndDocuments',
			method: 'GET',
			params: {
				InqueryID: inquiryId
			}
		});

		if (response.code === 200 && response.data) {
			if (response.data.documents && response.data.documents.length > 0) {
				inquiryDocumentList.value = response.data.documents.map(item => ({
					id: item.id,
					fileName: item.fileName,
					documentUrl: item.documentUrl
				}));
			} else {
				inquiryDocumentList.value = [];
			}
		}
	} catch (error) {
		console.error('加载附件列表失败:', error);
		ElMessage.error('加载附件列表失败');
	}
};

const SubmitInquiry = () => {
	if (isEditable.value == false) {
		ElMessage.warning('请先保存询价单');
		return;
	}
	ElMessageBox.confirm('确定要提交该询价单吗？', '提醒', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		const response = await request({
			url: 'Inquiry/UpdateInquiryIsDraft/UpdateInquiryIsDraft',
			method: 'GET',
			params: {
				IsDraft: 0,
				ID: EditInquiryID.value
			}
		});
		if (response.code === 200) {
			ElMessage({
				type: 'success',
				message: response.msg
			});
			CreateInquiryDialog.value = false;
			GetInquiryList(SearchInquirycurrentPage.value, SearchInquirypageSize.value);
		} else {
			ElMessage.error(response.msg || '提交失败');
		}
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消提交'
		});
	});
}
</script>
