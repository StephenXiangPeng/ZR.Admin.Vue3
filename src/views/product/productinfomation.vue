<template>
	<div>
		<el-container class="layout-container-demo">
			<el-aside width="300px">
				<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;产品分类</span>
				<el-divider></el-divider>
				<el-button size="small" plain @click="openAddproductCategoriesMessageBox()">添加分类</el-button>
				<el-button size="small" plain @click="DelproductCategoriesMessageBox()">删除分类</el-button>
				<el-tree-v2 :data="ProductCategoriesTreeData" style="font-size: 15px;" :height="700"
					@node-click="handleNodeClick">
					<template #default="{ node }">
						<span class="prefix" :class="{ 'is-leaf': node.isLeaf }">
							<el-icon>
								<Folder />
							</el-icon>
						</span>
						<el-tooltip :content="node.label" placement="top" style="ellipsis">
							<span>{{ node.label }}</span>
						</el-tooltip>
					</template>
				</el-tree-v2>
			</el-aside>
			<el-container>
				<el-main>
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
					<el-divider></el-divider>
					<div style="width: 100%; margin-top: 30px;">
						<el-input v-model="Search_ProductCode" clearable style="width: 15%"
							placeholder="输入产品编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-date-picker v-model="Search_StartTransactionDate" type="date" placeholder="请选择最近成交日期"
							size="Default"
							style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-date-picker v-model="Search_EndTransactionDate" type="date" placeholder="请选择最近成交日期"
							size="Default"
							style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-button type="primary" @click="Search_ProductInfo()" plain>查询</el-button>
						<el-button @click="Search_Reset()">重置</el-button>
					</div>
					<div style="margin-top: 30px;"></div>
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;产品列表</span>
					<el-divider></el-divider>
					<el-button type="primary" @click="openAddProductDialog()">添加产品</el-button>
					<el-table :data="ProductInfoTableData">
						<el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
						<el-table-column prop="chineseProductName" label="中文品名" width="150"></el-table-column>
						<el-table-column prop="chineseSpecification" label="中文规格" width="150"></el-table-column>
						<el-table-column prop="englishProductName" label="英文品名" width="150"></el-table-column>
						<el-table-column prop="unitOfMeasurement" label="计量单位" width="150"></el-table-column>
						<el-table-column prop="TBproductImage" label="产品图片" width="150"></el-table-column>
						<el-table-column prop="TBlastTransaction" label="最近成交" width="150"></el-table-column>
						<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
							<template v-slot:default="scope">
								<el-button link type="primary" size="small"
									@click=OpenProductInfoDetailDialog(scope.row)>查看详情</el-button>
								<el-button link type="primary" size="small" v-if="isDelteBtnShow"
									@click=DeleteProduct(scope.row)>删除产品</el-button>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
						:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
				</el-main>
			</el-container>
		</el-container>
		<el-dialog v-model="AddProductDialog" title="添加产品" :close-on-click-modal=false style="width: 70%;"
			@close="closeAddProductDialog()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form ref="ProductformRef" :rules="ProductformRules" :model="Productform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="产品编号" prop="productCode">
							<el-input v-model="Productform.productCode" :disabled="isDisabled" placeholder="请输入产品编号"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品条码">
							<el-input v-model="Productform.productBarcode" :disabled="isDisabled" placeholder="请输入产品条码"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="中文品名" prop="chineseProductName">
							<el-input v-model="Productform.chineseProductName" :disabled="isDisabled"
								placeholder="请输入中文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="英文品名" prop="englishProductName">
							<el-input v-model="Productform.englishProductName" :disabled="isDisabled"
								placeholder="请输入英文品名" style="width: 300px;" />
						</el-form-item>

					</el-col>
					<el-col :span="8">
						<el-form-item label="中文规格" prop="chineseSpecification">
							<el-input v-model="Productform.chineseSpecification" :disabled="isDisabled"
								placeholder="请输入中文规格" style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="英文规格">
							<el-input v-model="Productform.englishSpecification" :disabled="isDisabled"
								placeholder="请输入英文规格" style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="计量单位" prop="unit">
							<el-select v-model="Productform.unit" :disabled="isDisabled" placeholder="选择计量单位"
								style="width: 300px;">
								<el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="海关编码" prop="customsCode">
							<el-input v-model="Productform.customsCode" :disabled="isDisabled" placeholder="请输入海关编码"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="报关中文品名" prop="chineseDeclarationProductName">
							<el-input v-model="Productform.chineseDeclarationProductName" :disabled="isDisabled"
								placeholder="请输入报关中文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="报关英文品名" prop="englishDeclarationProductName">
							<el-input v-model="Productform.englishDeclarationProductName" :disabled="isDisabled"
								placeholder="请输入报关英文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="商检标志" prop="inspectionMark">
							<el-select v-model="Productform.inspectionMark" :disabled="isDisabled" placeholder="选择商检标志"
								style="width: 300px;">
								<el-option v-for="dict in optionss.hr_inspectionmark" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="包装方式" prop="PackingMethod">
							<el-select v-model="Productform.PackingMethod" :disabled="isDisabled" placeholder="选择包装方式"
								style="width: 300px;">
								<el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="库存数量">
							<el-input v-model="Productform.stockQuantity" disabled placeholder=""
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="开发时间日期">
							<el-date-picker v-model="Productform.developmentEventDate" type="date" disabled
								style=" width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="最近推荐">
							<el-select v-model="Productform.recentRecommendation" placeholder="" disabled
								style="width: 300px;">
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="最近寄样">
							<el-select v-model="Productform.recentSampleShipment" placeholder="" disabled
								style="width: 300px;">
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="最近报价">
							<el-select v-model="Productform.recentQuotation" placeholder="" disabled
								style="width: 300px;">
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="最近成交日期">
							<el-date-picker v-model="Productform.recentTransactionDate" type="date" disabled
								placeholder="" style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="16">
						<el-form-item label="产品照片：">
							<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList"
								limit="3" :disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl"
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
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">产品规格</span>
				<el-divider></el-divider>
				<el-row>
					<el-col :span="8">
						<el-form-item label="产品长度">
							<el-input v-model="Productform.productLength" :disabled="isDisabled"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品宽度">
							<el-input v-model="Productform.productwidth" :disabled="isDisabled" style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品高度">
							<el-input v-model="Productform.productheight" :disabled="isDisabled" placeholder="请输入中文品名"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="产品克重">
							<el-input v-model="Productform.productweight" :disabled="isDisabled"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="中包装量">
							<el-input v-model="Productform.mediumpackagingvolume" :disabled="isDisabled"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱装量">
							<el-input v-model="Productform.outerboxpackingquantity" :disabled="isDisabled"
								placeholder="请输入中文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="外箱长度(CM)">
							<el-input v-model="Productform.outerboxlength" :disabled="isDisabled" style="width: 300px;"
								@change="mainProductCalculatingOutBoxVolume()" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱宽度(CM)">
							<el-input v-model="Productform.outerboxwidth" :disabled="isDisabled" style="width: 300px;"
								@change="mainProductCalculatingOutBoxVolume()" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱高度(CM)">
							<el-input v-model="Productform.outerboxheight" :disabled="isDisabled" style="width: 300px;"
								@change="mainProductCalculatingOutBoxVolume()" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="外箱体积(m³)">
							<el-input v-model="Productform.outerboxvolume" disabled style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱净重(KG)">
							<el-input v-model="Productform.outerboxnetweight" :disabled="isDisabled"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="外箱毛重(KG)">
							<el-input v-model="Productform.outerboxgrossweight" :disabled="isDisabled"
								style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<span style="font-size: 20px; font-weight: bold;">子产品</span>
				<el-divider></el-divider>
				<el-button type="primary" @click="AddSubProduct()" v-if="showAddSubProductButton"
					:disabled="isDisabled">添加子产品</el-button>
				<el-table :data="SubProductTableData" style="width: 100%; height: 550px;">
					<el-table-column prop="mainProductCode" label="主产品编号" width="150" align="center">
						<template #default="scope">
							<span>{{ scope.row.mainProductCode }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="subProductCode" label="子产品编号" width="300" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subProductCode" style="max-width:250px" placeholder="请输入子产品编号"
								:disabled="isDisabled">
								<template #prepend>{{ Productform.productCode + "-" }}</template>
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subproductImage" label="产品图片" width="200" align="center">
						<template #default="scope">
							<el-upload :id="`upload-${scope.$index}`" ref="uploadRefs" :auto-upload="false"
								:show-file-list="true" :on-change="(file) => handleImageSelect(file, scope.$index)"
								:on-remove="(file) => handleImageRemove(file, scope.$index)" :limit="3" accept="image/*"
								multiple list-type="text" :file-list="scope.row.subproductImages || []">
								<el-button
									v-if="!isViewMode && (!scope.row.subproductImages || scope.row.subproductImages.length < 3)"
									type="primary" icon="Plus">
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
								<el-button v-if="!isViewMode" type="danger" icon="Delete"
									@click="deleteCurrentImage(scope.$index)">删除</el-button>
							</div>
							<span v-else>暂无图片</span>
						</template>
					</el-table-column>
					<el-table-column prop="subproductBarcode" label="产品条码" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subproductBarcode" style="max-width:250px"
								:disabled="isDisabled" placeholder="请输入子产品条码">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subchineseProductName" label="中文品名" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subchineseProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品中文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subenglishProductName" label="英文品名" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subenglishProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品英文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subchineseSpecification" label="中文规格" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subchineseSpecification" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品中文规格">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subenglishSpecification" label="英文规格" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subenglishSpecification" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品英文规格">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subunit" label="计量单位" width="150" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.subunit" :disabled="isDisabled" placeholder="请选择"
								style="width: 90px;">
								<el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode"
									:disabled="isDisabled" :label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="subcustomsCode" label="海关编码" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subcustomsCode" style="max-width:200px" :disabled="isDisabled"
								placeholder="请输入子产品海关编码">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subchineseDeclarationProductName" label="报关中文品名" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subchineseDeclarationProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品报关中文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subenglishDeclarationProductName" label="报关英文品名" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subenglishDeclarationProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品报关英文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subinspectionMark" label="商检标志" width="120" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.subinspectionMark" :disabled="isDisabled" placeholder="请选择"
								style="width: 90px;">
								<el-option v-for="dict in optionss.hr_inspectionmark" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="subPackingMethod" label="包装方式" width="130" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.subPackingMethod" :disabled="isDisabled" placeholder="请选择"
								style="width: 100px;">
								<el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="substockQuantity" label="库存数量" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.substockQuantity" style="max-width:200px" disabled
								placeholder="请输入子产品库存数量">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subdevelopmentEventDate" label="开发时间日期" width="200" align="center">
						<template #default="scope">
							<el-date-picker v-model="scope.row.subdevelopmentEventDate" type="date" disabled
								placeholder="请选择" style="width: 140px;" />
						</template>
					</el-table-column>
					<el-table-column prop="subrecentRecommendation" label="最近推荐" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subrecentRecommendation" style="max-width:200px" disabled
								placeholder="请输入子产品最近推荐">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subrecentSampleShipment" label="最近寄样" width="150" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.subrecentSampleShipment" disabled placeholder="请选择"
								style="width: 90px;">
								<el-option v-for="dict in optionss.hr_inspectionmark" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="subrecentQuotation" label="最近报价" width="150" align="center">
						<template #default="scope">
							<el-select v-model="scope.row.subrecentQuotation" disabled placeholder="请选择"
								style="width: 90px;">
								<el-option v-for="dict in optionss.hr_inspectionmark" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</template>
					</el-table-column>
					<el-table-column prop="subrecentTransactionDate" label="最近成交日期" width="200" align="center">
						<template #default="scope">
							<el-date-picker v-model="scope.row.subrecentTransactionDate" type="date" disabled
								placeholder="请选择" style="width: 140px;" />
						</template>
					</el-table-column>
					<el-table-column prop="subproductLength" label="产品长度" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subproductLength" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品长度">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subproductWidth" label="产品宽度" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subproductWidth" style="max-width:200px" :disabled="isDisabled"
								placeholder="请输入子产品宽度">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subproductHeight" label="产品高度" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subproductHeight" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品高度">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subproductWeight" label="产品克重" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subproductWeight" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品克重">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="submediumPackagingVolume" label="中包装量" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.submediumPackagingVolume" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品中包装量">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxPackingQuantity" label="外箱装量" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxPackingQuantity" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱装量">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxLength" label="外箱长度(CM)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxLength" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱长度"
								@change="subProductCalculatingOutBoxVolume(scope.$index)">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxWidth" label="外箱宽度(CM)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxWidth" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱宽度"
								@change="subProductCalculatingOutBoxVolume(scope.$index)">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxHeight" label="外箱高度(CM)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxHeight" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱高度"
								@change="subProductCalculatingOutBoxVolume(scope.$index)">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxVolume" label="外箱体积(m³)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxVolume" style="max-width:200px" disabled
								placeholder="请输入子产品外箱体积">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxNetWeight" label="外箱净重(KG)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxNetWeight" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱净重">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subouterBoxGrossWeight" label="外箱毛重(KG)" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subouterBoxGrossWeight" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品外箱毛重">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column label="操作" width="150" align="center" fixed="right">
						<template #default="scope">
							<el-button :disabled="isDisabled" link type="danger"
								@click="removeSubProduct(scope.$index)">删除</el-button>
						</template>
					</el-table-column>
				</el-table>

				<span style="font-size: 20px; font-weight: bold;">产品关联数据</span>
				<el-divider></el-divider>
				<el-tabs v-model="activeTab" class="demo-tabs">
					<el-tab-pane label="工厂报价" name="FactoryQuotationTab">
						<el-table :data="FactoryQuotationTableData" style="width: 100%">
							<el-table-column prop="" label="报价日期" width="150"></el-table-column>
							<el-table-column prop="" label="供应商编号" width="150"></el-table-column>
							<el-table-column prop="" label="供应商简称" width="150"></el-table-column>
							<el-table-column prop="" label="产品图片" width="150"></el-table-column>
							<el-table-column prop="" label="名称型号及规格" width="150"></el-table-column>
							<el-table-column prop="" label="包装方式" width="150"></el-table-column>
							<el-table-column prop="" label="起订量" width="150"></el-table-column>
							<el-table-column prop="" label="折扣价起订量" width="150"></el-table-column>
							<el-table-column prop="" label="定制起订量" width="150"></el-table-column>
							<el-table-column prop="" label="价格条款" width="150"></el-table-column>
							<el-table-column prop="" label="报价目的地" width="150"></el-table-column>
							<el-table-column prop="" label="单价" width="150"></el-table-column>
							<el-table-column prop="" label="计量单位" width="150"></el-table-column>
							<el-table-column prop="" label="含税+/-" width="150"></el-table-column>
						</el-table>
					</el-tab-pane>
					<el-tab-pane label="销售历史" name="SaleHistoryTab">
						<el-table :data="SaleHistoryTableData" style="width: 100%">
							<el-table-column prop="" label="交货日期" width="150"></el-table-column>
							<el-table-column prop="" label="客户简称" width="150"></el-table-column>
							<el-table-column prop="" label="客户货号" width="150"></el-table-column>
							<el-table-column prop="" label="中文品名" width="150"></el-table-column>
							<el-table-column prop="" label="合同数量" width="150"></el-table-column>
							<el-table-column prop="" label="包装方式" width="150"></el-table-column>
							<el-table-column prop="" label="价格条款" width="150"></el-table-column>
							<el-table-column prop="" label="销售单价" width="150"></el-table-column>
							<el-table-column prop="" label="计量单位" width="150"></el-table-column>
							<el-table-column prop="" label="总金额" width="150"></el-table-column>
							<el-table-column prop="" label="含税+/-" width="150"></el-table-column>
						</el-table>
					</el-tab-pane>
					<el-tab-pane label="采购历史" name="PurchaseHistoryTab">
						<el-table :data="PurchaseHistoryTableData" style="width: 100%">
							<el-table-column prop="" label="采购时间" width="150"></el-table-column>
							<el-table-column prop="" label="采购合同" width="150"></el-table-column>
							<el-table-column prop="" label="供应商编号" width="150"></el-table-column>
							<el-table-column prop="" label="供应商简称" width="150"></el-table-column>
							<el-table-column prop="" label="中文品名" width="150"></el-table-column>
							<el-table-column prop="" label="合同数量" width="150"></el-table-column>
							<el-table-column prop="" label="包装方式" width="150"></el-table-column>
							<el-table-column prop="" label="价格条款" width="150"></el-table-column>
							<el-table-column prop="" label="采购单价" width="150"></el-table-column>
							<el-table-column prop="" label="计量单位" width="150"></el-table-column>
							<el-table-column prop="" label="总金额" width="150"></el-table-column>
							<el-table-column prop="" label="含税+/-" width="150"></el-table-column>
						</el-table>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" v-if="showSaveBtn" @click="SaveProductinfomation(ProductformRef)">
						确定保存
					</el-button>
					<el-button type="primary" v-if="showEditBtn" @click="EditProductinfomation()">
						编辑
					</el-button>
					<el-button type="primary" v-if="showEditSaveBtn" @click="EditSaveProductinfomation()">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">

import { createApp, getCurrentInstance, reactive, toRefs, ref, callWithAsyncErrorHandling } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, FormInstance, UploadUserFile, UploadFile, subMenuProps, FormRules } from 'element-plus'
import { FOCUSABLE_CHILDREN } from 'element-plus/es/directives/trap-focus';
import request from '@/utils/request';
import { number } from 'echarts';
import { Edit } from '@element-plus/icons-vue/dist/types';
import useUserStore from '@/store/modules/user';
import { get } from 'sortablejs';
import { onMounted } from 'vue'; //初始运行钩子
import { ArrowLeft, ArrowRight } from '@element-plus/icons-vue'


onMounted(() => {
	// 这里编写进入页面时要运行的函数
	runOnPageLoad();
});

const SubProductTableData = ref([])

//计算主产品体积
const mainProductCalculatingOutBoxVolume = () => {
	Productform.outerboxvolume = (Productform.outerboxlength / 100) * (Productform.outerboxwidth / 100) * (Productform.outerboxheight / 100);
}
//计算子产品体积
const subProductCalculatingOutBoxVolume = (index) => {
	SubProductTableData.value[index].subouterBoxVolume = (SubProductTableData.value[index].subouterBoxLength / 100) * (SubProductTableData.value[index].subouterBoxWidth / 100) * (SubProductTableData.value[index].subouterBoxHeight / 100);
}

//添加子产品
const AddSubProduct = () => {
	if (!Productform.productCode) {
		ElMessage.warning('请先填写产品编号');
		return;
	}
	SubProductTableData.value.push({
		mainProductCode: Productform.productCode,
		subunit: state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === Productform.unit)?.dictValue,
		subcustomsCode: Productform.customsCode,
		subchineseDeclarationProductName: Productform.chineseDeclarationProductName,
		subenglishDeclarationProductName: Productform.englishDeclarationProductName,
		subinspectionMark: state.optionss.hr_inspectionmark.find((dict) => dict.dictValue === Productform.inspectionMark)?.dictValue,
		substockQuantity: Productform.stockQuantity,
		subdevelopmentEventDate: Productform.developmentEventDate,
		subrecentRecommendation: Productform.recentRecommendation,
		subrecentSampleShipment: Productform.recentSampleShipment,
		subrecentQuotation: Productform.recentQuotation,
		subrecentTransactionDate: Productform.recentTransactionDate,
		subproductLength: Productform.productLength,
		subproductWidth: Productform.productwidth,
		subproductHeight: Productform.productheight,
		subproductWeight: Productform.productweight,
		submediumPackagingVolume: Productform.mediumpackagingvolume,
		subouterBoxPackingQuantity: Productform.outerboxpackingquantity,
		subouterBoxLength: Productform.outerboxlength,
		subouterBoxWidth: Productform.outerboxwidth,
		subouterBoxHeight: Productform.outerboxheight,
		subouterBoxVolume: Productform.outerboxvolume,
		subouterBoxNetWeight: Productform.outerboxnetweight,
		subouterBoxGrossWeight: Productform.outerboxgrossweight,
		subproductBarcode: Productform.productBarcode,
		subchineseProductName: Productform.chineseProductName,
		subenglishProductName: Productform.englishProductName,
		subchineseSpecification: Productform.chineseSpecification,
		subenglishSpecification: Productform.englishSpecification,
		subPackingMethod: state.optionss.hr_packing.find((dict) => dict.dictValue === Productform.PackingMethod)?.dictValue
		// 添加其他需要的字段，可以设置默认值
	})
}
//编辑子产品
const editSubProduct = (index, row) => {
	// 实现编辑逻辑，可以打开一个编辑对话框
	console.log('编辑子产品', index, row)
}

//删除子产品
const removeSubProduct = (index) => {
	SubProductTableData.value.splice(index, 1)
}
const selectedImages = ref([]); // 存储用户选择的图片文件
const previewImage = ref(''); // 存储要预览的图片
const previewVisible = ref(false); // 控制预览对话框的显示

const prevImage = (index) => {
	if (SubProductTableData.value[index].currentImageIndex > 0) {
		SubProductTableData.value[index].currentImageIndex--;
	}
}

const nextImage = (index) => {
	if (SubProductTableData.value[index].currentImageIndex < SubProductTableData.value[index].subproductImages.length - 1) {
		SubProductTableData.value[index].currentImageIndex++;
	}
}

const openPreview = (index) => {
	const subProduct = SubProductTableData.value[index];
	// 这里可以添加打开预览的逻辑，如果需要的话
	// 例如，可以使用 Element Plus 的 ElImageViewer 组件
};

const deleteCurrentImage = (index) => {
	const subProduct = SubProductTableData.value[index];
	if (subProduct.subproductImages && subProduct.subproductImages.length > 0) {
		// 删除当前显示的图片
		const deletedImage = subProduct.subproductImages.splice(subProduct.currentImageIndex, 1)[0];

		// 更新 currentImageIndex
		if (subProduct.currentImageIndex >= subProduct.subproductImages.length) {
			subProduct.currentImageIndex = Math.max(0, subProduct.subproductImages.length - 1);
		}
		subProduct.subproductImages = [...subProduct.subproductImages];
		// 强制更新视图
		SubProductTableData.value = [...SubProductTableData.value];
	}
}

const uploadRefs = ref([]);
// 添加一个 watch 来监听 subproductImages 的变化
watch(() => SubProductTableData.value?.map(p => p.subproductImages), (newVal, oldVal) => {
	if (!SubProductTableData.value || SubProductTableData.value.length === 0) {
		return; // 如果 SubProductTableData 为空或不存在，直接返回
	}
	SubProductTableData.value.forEach((subProduct, index) => {
		if (subProduct && subProduct.subproductImages) {
			const uploadRef = uploadRefs.value?.[index];
			if (uploadRef && uploadRef.clearFiles && uploadRef.handleStart) {
				uploadRef.clearFiles();
				subProduct.subproductImages.forEach(img => {
					if (img && img.raw) {
						uploadRef.handleStart(img.raw);
					}
				});
			}
		}
	});
}, { deep: true });


const handleImageSelect = (file, index) => {
	if (!file) {
		console.error('No file selected');
		ElMessage.error('请选择图片文件');
		return;
	}
	const reader = new FileReader();
	reader.onload = (e) => {
		if (!SubProductTableData.value[index].subproductImages) {
			SubProductTableData.value[index].subproductImages = [];
			SubProductTableData.value[index].currentImageIndex = 0;
		}
		if (SubProductTableData.value[index].subproductImages.length < 3) {
			const newImage = {
				name: file.name,
				url: e.target.result,
				raw: file.raw,//保存原始文件对象
				isChanged: true
			};
			SubProductTableData.value[index].subproductImages.push(newImage);
			SubProductTableData.value[index].currentImageIndex = SubProductTableData.value[index].subproductImages.length - 1;
			// 强制更新视图
			SubProductTableData.value = [...SubProductTableData.value];
		} else {
			ElMessage.warning('最多只能上传3张图片');
		}
	};
	reader.readAsDataURL(file.raw);
};

const handleImageRemove = (file, index) => {
	const images = SubProductTableData.value[index].subproductImages;
	const fileIndex = images.findIndex(img => img.name === file.name);
	if (fileIndex > -1) {
		images.splice(fileIndex, 1);
		if (SubProductTableData.value[index].currentImageIndex >= images.length) {
			SubProductTableData.value[index].currentImageIndex = Math.max(0, images.length - 1);
		}
		// 强制更新视图
		SubProductTableData.value = [...SubProductTableData.value];
	}
};


const activeTab = ref('FactoryQuotationTab')
const isDelteBtnShow = ref(false);
const isDisabled = ref(false);
const showSaveBtn = ref(true);
const showEditBtn = ref(false);
const showEditSaveBtn = ref(false);
const runOnPageLoad = () => {
	//#region 是否显示删除按钮、编辑按钮；根据角色ID判断；
	var userInfo = useUserStore().userInfo;
	var roleIds = userInfo.roleIds;
	const contains1 = roleIds.includes(1);
	const contains2 = roleIds.includes(7);
	const contains3 = roleIds.includes(8);
	if (roleIds.includes(1)) {
		isDelteBtnShow.value = true;
		showEditBtn.value = true;
	} else if (roleIds.includes(7) || roleIds.includes(8)) {
		showEditBtn.value = false;
	} else {
		isDelteBtnShow.value = false;
		showEditBtn.value = false;
	}
	//#endregion
};

const openAddProductDialog = () => {
	if (SelectNodeId.value == 0) {
		ElMessage({
			type: 'warning',
			message: '请先在左侧选择产品分类！'
		})
		return;
	}
	clearProductform();
	showSaveBtn.value = true;
	AddProductDialog.value = true;
	showAddSubProductButton.value = true;
}

const closeAddProductDialog = () => {
	clearProductform();
	SelectNodeId.value = 0;
}

const clearProductform = () => {
	Productform.productCode = '';
	Productform.productBarcode = '';
	Productform.chineseProductName = '';
	Productform.englishProductName = '';
	Productform.chineseSpecification = '';
	Productform.englishSpecification = '';
	Productform.unit = '';
	Productform.customsCode = '';
	Productform.chineseDeclarationProductName = '';
	Productform.englishDeclarationProductName = '';
	Productform.inspectionMark = '';
	Productform.stockQuantity = 0;
	Productform.developmentEventDate = '';
	Productform.recentRecommendation = '';
	Productform.recentSampleShipment = '';
	Productform.recentQuotation = '';
	Productform.recentTransactionDate = '';
	Productform.ProductPhoto = '';
	Productform.PackingMethod = '';
	Productform.productLength = '';
	Productform.productwidth = '';
	Productform.productheight = '';
	Productform.productweight = 0;
	Productform.mediumpackagingvolume = 0;
	Productform.outerboxpackingquantity = 0;
	Productform.outerboxlength = 0;
	Productform.outerboxwidth = 0;
	Productform.outerboxheight = 0;
	Productform.outerboxvolume = 0;
	Productform.outerboxnetweight = 0;
	Productform.outerboxgrossweight = 0;
	fileList.value = []
	uploadedFiles.value = []
	SubProductTableData.value = []
	isDisabled.value = false;
	showEditSaveBtn.value = false;
	showEditBtn.value = false;
}

//  上传主产品图片
const handleChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 3) {
		ElMessage({
			type: 'info',
			message: '最多上传3张图片!'
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
// 删除产品图片
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

		// 从 CustomerProfileform.customerPhoto 中移除对应的 url
		if (Productform.ProductPhoto !== '' && Productform.ProductPhoto !== null) {
			const urls = Productform.ProductPhoto.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				Productform.ProductPhoto = urls.join(',');
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


const handlePictureCardPreview = (file: UploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}


const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_packing: [],
		hr_calculate_unit: [],
		hr_inspectionmark: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_packing' }, { dictType: 'hr_calculate_unit' }, { dictType: 'hr_inspectionmark' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	GetProductInfoList(currentPage.value, pageSize.value);
})


let SelectedProductCategoriesStr = '';	// 选中的产品分类
/*添加产品Request*/
const AddProductCategoriesRequest = {
	"create_by": "string",
	"create_time": "2024-03-14T06:34:07.762Z",
	"update_by": "string",
	"update_time": "2024-03-14T06:34:07.762Z",
	"remark": "string",
	"id": 0,
	"name": "string",
	"parentID": 0,
	"childID": 0,
	"isDelete": 0
}
/*添加产品分类*/
const openAddproductCategoriesMessageBox = () => {
	if (SelectedProductCategoriesStr != '') {
		SelectedProductCategoriesStr = '即将为' + SelectedProductCategoriesStr + '添加新分类,';
	}
	ElMessageBox.prompt(`${SelectedProductCategoriesStr}请输入新产品分类名称`, '添加分类', {
		confirmButtonText: '保存',
		cancelButtonText: '取消',
		inputPattern:
			/^[a-zA-Z0-9\u4e00-\u9fa5]{1,10000000000000}$/,
		inputErrorMessage: '分类名称不能为空,可以是数字、汉字、字母组合,最多个10000000000000字符!😒',
	})
		.then(({ value }) => {
			AddProductCategoriesRequest.name = value;
			if (SelectNodeId.value != 0) {
				AddProductCategoriesRequest.parentID = SelectNodeId.value;
			} else {
				AddProductCategoriesRequest.parentID = 0;
			}
			request.post('ProductCategories/AddProductCategories/Add', AddProductCategoriesRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: response.msg,
						type: 'success'
					})
					ResetSelectNode();
					GetAllParentProductCategoriesList();
				} else {
					console.error('添加产品分类出错');
				}
			}).catch(error => {
				ResetSelectNode();
				console.error('添加产品分类出错！😔错误内容：', error);
			})
		})
		.catch(() => {
			ResetSelectNode();
			ElMessage({
				type: 'info',
				message: '已取消',
			})
		})

}

const DelproductCategoriesMessageBox = () => {
	if (SelectNodeId.value == 0) {
		ElMessage({
			type: 'warning',
			message: '请选择要删除的产品分类'
		})
		return;
	} else {
		ElMessageBox.confirm(`确认删除${SelectedProductCategoriesStr}该产品分类？删除后当前分类下的所有子分类将自动归集到上一层分类中`, '删除分类', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			request.delete('ProductCategories/DelProductCategories/Del?ProductCategoriesID=' + SelectNodeId.value).then(response => {
				if (response != null) {
					ElMessage({
						message: response.msg,
						type: 'success'
					})
					ResetSelectNode();
					GetAllParentProductCategoriesList();
				} else {
					console.error('删除产品分类出错');
				}
			}).catch(error => {
				ResetSelectNode();
				console.error('删除产品分类出错！😔错误内容：', error);
			})
		}).catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消删除'
			});
		});
	}
}



const ProductCategoriesTreeData = ref([]);
/*获取产品分类*/
const GetAllParentProductCategoriesList = () => {
	request({
		url: 'ProductCategories/GetAllParentProductCategoriesList/GetList',
		method: 'GET'
	}).then(response => {
		if (response.length > 0) {
			ProductCategoriesTreeData.value = response;
		}
	}).catch(error => {
		console.error(error);
	});
}
const SelectNodeId = ref(0);
const handleNodeClick = (node) => {
	SelectNodeId.value = node.id;
	SelectedProductCategoriesStr = '【' + node.label + '】';
	GetProductInfoList(currentPage.value, pageSize.value);
}
const ResetSelectNode = () => {
	SelectNodeId.value = 0;
	SelectedProductCategoriesStr = '';
}
GetAllParentProductCategoriesList();

interface Productform {
	id: number;
	productCode: string;
	productBarcode: string;
	chineseProductName: string;
	englishProductName: string;
	chineseSpecification: string;
	englishSpecification: string;
	unit: string;
	customsCode: string;
	chineseDeclarationProductName: string;
	englishDeclarationProductName: string;
	inspectionMark: string;
	stockQuantity: number;
	developmentEventDate: string;
	recentRecommendation: string;
	recentSampleShipment: string;
	recentQuotation: string;
	recentTransactionDate: string;
	ProductPhoto: string;
	PackingMethod: string;
	//产品属性
	productLength: string;
	productwidth: string;
	productheight: string;
	productweight: number;
	mediumpackagingvolume: number;
	outerboxpackingquantity: number;
	outerboxlength: number;
	outerboxwidth: number;
	outerboxheight: number;
	outerboxvolume: number;
	outerboxnetweight: number;
	outerboxgrossweight: number;
	//子产品
	subProductItems: SubProductTableData[];
}

const ProductformRef = ref<FormInstance>()
const Productform = reactive<Productform>({
	id: null,
	productCode: '',
	productBarcode: '',
	chineseProductName: '',
	englishProductName: '',
	chineseSpecification: '',
	englishSpecification: '',
	unit: '',
	customsCode: '',
	chineseDeclarationProductName: '',
	englishDeclarationProductName: '',
	inspectionMark: '',
	stockQuantity: 0,
	developmentEventDate: '',
	recentRecommendation: '',
	recentSampleShipment: '',
	recentQuotation: '',
	recentTransactionDate: '',
	ProductPhoto: '',
	PackingMethod: '',
	//产品属性
	productLength: '',
	productwidth: '',
	productheight: '',
	productweight: 0,
	mediumpackagingvolume: 0,
	outerboxpackingquantity: 0,
	outerboxlength: 0,
	outerboxwidth: 0,
	outerboxheight: 0,
	outerboxvolume: 0,
	outerboxnetweight: 0,
	outerboxgrossweight: 0,
	// 新增 subProductItems 数组
	subProductItems: SubProductTableData.value
})

const ProductformRules = reactive<FormRules<Productform>>({
	productCode: [{ required: true, message: '请输入产品编号', trigger: ['blur', 'change'] }],
	chineseProductName: [{ required: true, message: '请输入中文品名', trigger: ['blur', 'change'] }],
	englishProductName: [{ required: true, message: '请输入英文品名', trigger: ['blur', 'change'] }],
	chineseSpecification: [{ required: true, message: '请输入中文规格', trigger: ['blur', 'change'] }],
	unit: [{ required: true, message: '请选择计量单位', trigger: ['blur', 'change'] }],
	customsCode: [{ required: true, message: '请输入海关编码', trigger: ['blur', 'change'] }],
	chineseDeclarationProductName: [{ required: true, message: '请输入中文申报品名', trigger: ['blur', 'change'] }],
	englishDeclarationProductName: [{ required: true, message: '请输入英文申报品名', trigger: ['blur', 'change'] }],
	inspectionMark: [{ required: true, message: '请选择检验标志', trigger: ['blur', 'change'] }],
	PackingMethod: [{ required: true, message: '请选择包装方式', trigger: ['blur', 'change'] }]
})

const createSubProductItem = () => ({
	mainProductCode: Productform.productCode,
	subProductCode: '',
	subproductImages: [],
	subproductBarcode: '',
	subchineseProductName: '',
	subenglishProductName: '',
	subchineseSpecification: '',
	subenglishSpecification: '',
	subunit: '',
	subcustomsCode: '',
	subchineseDeclarationProductName: '',
	subenglishDeclarationProductName: '',
	subinspectionMark: '',
	substockQuantity: 0,
	subdevelopmentEventDate: '',
	subrecentRecommendation: '',
	subrecentSampleShipment: '',
	subrecentQuotation: '',
	subrecentTransactionDate: '',
	subproductLength: '',
	subproductWidth: '',
	subproductHeight: '',
	subproductWeight: '',
	submediumPackagingVolume: '',
	subouterBoxPackingQuantity: '',
	subouterBoxLength: '',
	subouterBoxWidth: '',
	subouterBoxHeight: '',
	subouterBoxVolume: '',
	subouterBoxNetWeight: '',
	subouterBoxGrossWeight: '',
	currentImageIndex: 0
})

const filelistUrlStr = ref('');	// 产品图片
const AddProductDialog = ref(false)	// 添加产品对话框
const UploadUrl = 'Common/UploadFile'	// 上传图片地址
const fileList = ref<UploadUserFile[]>([]);
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const TableData = ref([])	// 产品列表
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)

const SaveProductinfomation = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate(async (valid, fields) => {
		if (valid) {
			try {
				const productInfoRequest = {
					ProductCategoriesID: SelectNodeId.value,
					ProductCode: Productform.productCode,
					ProductBarcode: Productform.productBarcode,
					ChineseProductName: Productform.chineseProductName,
					EnglishProductName: Productform.englishProductName,
					ChineseSpecification: Productform.chineseSpecification,
					EnglishSpecification: Productform.englishSpecification,
					UnitOfMeasurement: Productform.unit,
					CustomsCode: Productform.customsCode,
					ChineseDeclarationProductName: Productform.chineseDeclarationProductName,
					EnglishDeclarationProductName: Productform.englishDeclarationProductName,
					InspectionMark: Productform.inspectionMark,
					StockQuantity: Productform.stockQuantity,
					DevelopmentEventDate: Productform.developmentEventDate,
					RecentRecommendation: Productform.recentRecommendation,
					RecentSampleShipment: Productform.recentSampleShipment,
					RecentQuotation: Productform.recentQuotation,
					RecentTransactionDate: Productform.recentTransactionDate,
					ProductLength: Productform.productLength,
					ProductWidth: Productform.productwidth,
					ProductHeight: Productform.productheight,
					ProductWeight: Productform.productweight,
					MediumPackagingVolume: Productform.mediumpackagingvolume,
					OuterBoxPackingQuantity: Productform.outerboxpackingquantity,
					OuterBoxLength: Productform.outerboxlength,
					OuterBoxWidth: Productform.outerboxwidth,
					OuterBoxHeight: Productform.outerboxheight,
					OuterBoxVolume: Productform.outerboxvolume,
					OuterBoxNetWeight: Productform.outerboxnetweight,
					OuterBoxGrossWeight: Productform.outerboxgrossweight,
					ProductPhotoPath: '',
					PackingMethod: Productform.PackingMethod,
					Remark: '',
					subProductItems: []
				};
				// 上传主产品图片
				let mainProductImageUrls = [];
				if (Array.isArray(fileList.value) && fileList.value.length > 0) {
					mainProductImageUrls = await Promise.all(fileList.value.map(async (file) => {
						const response = await uploadProductPhoto(file);
						return response.data.url;
					}));
				}
				productInfoRequest.ProductPhotoPath = mainProductImageUrls.join(',');
				// 处理子产品
				if (Array.isArray(SubProductTableData.value) && SubProductTableData.value.length > 0) {
					// 先进行所有子产品的验证
					const validationErrors = SubProductTableData.value.flatMap((subProduct, index) => {
						const errors = [];
						if (subProduct.subProductCode == '' || subProduct.subProductCode == null || subProduct.subProductCode == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入子产品编号;`);
						}
						if (subProduct.subchineseProductName == '' || subProduct.subchineseProductName == null || subProduct.subchineseProductName == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入中文品名;`);
						}
						if (subProduct.subenglishProductName == '' || subProduct.subenglishProductName == null || subProduct.subenglishProductName == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入英文品名;`);
						}
						if (subProduct.subchineseSpecification == '' || subProduct.subchineseSpecification == null || subProduct.subchineseSpecification == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入中文规格;`);
						}
						if (subProduct.subunit == '' || subProduct.subunit == null || subProduct.subunit == undefined) {
							errors.push(`子产品 ${index + 1}: 请选择计量单位;`);
						}
						if (subProduct.subPackingMethod == '' || subProduct.subPackingMethod == null || subProduct.subPackingMethod == undefined) {
							errors.push(`子产品 ${index + 1}: 请选择包装方式;`);
						}
						if (subProduct.subcustomsCode == '' || subProduct.subcustomsCode == null || subProduct.subcustomsCode == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入海关编码;`);
						}
						if (subProduct.subchineseDeclarationProductName == '' || subProduct.subchineseDeclarationProductName == null || subProduct.subchineseDeclarationProductName == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入中文报关品名;`);
						}
						if (subProduct.subenglishDeclarationProductName == '' || subProduct.subenglishDeclarationProductName == null || subProduct.subenglishDeclarationProductName == undefined) {
							errors.push(`子产品 ${index + 1}: 请输入英文报关品名;`);
						}
						if (subProduct.subinspectionMark == '' || subProduct.subinspectionMark == null || subProduct.subinspectionMark == undefined) {
							errors.push(`子产品 ${index + 1}: 请选择商检标志;`);
						}
						return errors;
					});
					// 如果有验证错误，显示错误信息并中断保存过程
					if (validationErrors.length > 0) {
						ElMessage({
							message: validationErrors.join('\r\n'),
							type: 'warning',
							duration: 5000,
							showClose: true
						});
						return; // 中断保存过程
					}
					productInfoRequest.subProductItems = await Promise.all(SubProductTableData.value.map(async subProduct => {
						// 上传子产品图片
						let subProductImageUrls = [];
						if (Array.isArray(subProduct.subproductImages) && subProduct.subproductImages.length > 0) {
							subProductImageUrls = await Promise.all(subProduct.subproductImages.map(async (file) => {
								const response = await uploadProductPhoto(file);
								return response.data.url;
							}));
							console.log(subProduct.subproductImages.value);
						}
						return {
							mainProductCode: productInfoRequest.ProductCode,
							subProductCode: productInfoRequest.ProductCode + '-' + subProduct.subProductCode,
							subProductImages: subProductImageUrls.join(','),
							subProductBarcode: subProduct.subproductBarcode,
							subChineseProductName: subProduct.subchineseProductName,
							subEnglishProductName: subProduct.subenglishProductName,
							subChineseSpecification: subProduct.subchineseSpecification,
							subEnglishSpecification: subProduct.subenglishSpecification,
							subUnit: subProduct.subunit,
							subCustomsCode: subProduct.subcustomsCode,
							subChineseDeclarationProductName: subProduct.subchineseDeclarationProductName,
							subEnglishDeclarationProductName: subProduct.subenglishDeclarationProductName,
							subInspectionMark: subProduct.subinspectionMark,
							subStockQuantity: subProduct.substockQuantity,
							subDevelopmentEventDate: subProduct.subdevelopmentEventDate,
							subRecentRecommendation: subProduct.subrecentRecommendation,
							subRecentSampleShipment: subProduct.subrecentSampleShipment,
							subRecentQuotation: subProduct.subrecentQuotation,
							subRecentTransactionDate: subProduct.subrecentTransactionDate,
							subProductLength: subProduct.subproductLength,
							subProductWidth: subProduct.subproductWidth,
							subProductHeight: subProduct.subproductHeight,
							subProductWeight: subProduct.subproductWeight,
							subMediumPackagingVolume: subProduct.submediumPackagingVolume,
							subOuterBoxPackingQuantity: subProduct.subouterBoxPackingQuantity,
							subOuterBoxLength: subProduct.subouterBoxLength,
							subOuterBoxWidth: subProduct.subouterBoxWidth,
							subOuterBoxHeight: subProduct.subouterBoxHeight,
							subOuterBoxVolume: subProduct.subouterBoxVolume,
							subOuterBoxNetWeight: subProduct.subouterBoxNetWeight,
							subOuterBoxGrossWeight: subProduct.subouterBoxGrossWeight,
							subpackingMethod: subProduct.subPackingMethod
						};
					}));
				}
				console.log(productInfoRequest);
				const response = await request.post('ProductInformation/AddProductInformation/Add', productInfoRequest);
				if (response != null) {
					ElMessage({
						message: response.msg,
						type: 'success'
					})
					filelistUrlStr.value = '';
					GetProductInfoList(currentPage.value, pageSize.value);
					AddProductDialog.value = false
				} else {
					throw new Error(response.data.message || '保存失败');
				}
			} catch (error) {
				console.error('保存产品信息时出错：', error);
				ElMessage({
					message: '保存产品信息失败: ' + error.message,
					type: 'error'
				});
			}
		} else {
			console.log('error submit!', fields)
		}
	})
};
// 辅助函数：上传产品图片
const uploadProductPhoto = async (file) => {
	const formData = new FormData();
	formData.append('FileName', file.name || '未命名文件');
	formData.append('FileDir', 'ProductInfo/ProductInfoPhoto');
	formData.append('FileNameType', '1');
	formData.append('File', file.raw || file);
	formData.append('storeType', '1');
	const response = await request.postForm(UploadUrl, formData);
	return response;
};

//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
//产品信息表格
const ProductInfoTableData = ref([])
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetProductInfoList(start, end);
};
const Search_ProductCode = ref('');	// 查询产品编号
const Search_StartTransactionDate = ref('');	// 查询最近成交日期
const Search_EndTransactionDate = ref('');	// 查询最近成交日期
const Search_ProductInfo = () => {
	GetProductInfoList(currentPage.value, pageSize.value);
}
const Search_Reset = () => {
	Search_ProductCode.value = '';
	Search_StartTransactionDate.value = '';
	Search_EndTransactionDate.value = '';
	GetProductInfoList(currentPage.value, pageSize.value);
}

//获取产品信息列表
function GetProductInfoList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'ProductInformation/GetProductList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				ProductCode: Search_ProductCode.value,
				startDate: Search_StartTransactionDate.value,
				endDate: Search_EndTransactionDate.value,
				ProductCategoriesID: SelectNodeId.value
			}
		}).then(response => {
			if (response.data.data.length > 0) {
				ProductInfoTableData.value = response.data.data;
				ProductInfoTableData.value.forEach((item) => {
					item.unitOfMeasurement = state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === item.unitOfMeasurement)?.dictLabel;
				})
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					ProductInfoTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

//删除产品
const DeleteProduct = (row) => {
	ElMessageBox.confirm('确定删除【' + row.chineseProductName + '】吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		request({
			url: 'ProductInformation/DeleteProductInfo/Delete',
			method: 'DElETE',
			params: {
				productID: row.id
			}
		}).then(response => {
			alert(response.msg);
		}).catch(error => {
			console.error(error);
		}).finally(() => {
			GetProductInfoList(currentPage.value, pageSize.value);
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除',
		});
	});
}

const isViewMode = ref(false);
const showAddSubProductButton = ref(true);
//查看详情
const OpenProductInfoDetailDialog = (row) => {
	isViewMode.value = true;
	showSaveBtn.value = false;
	showEditBtn.value = true;
	isDisabled.value = true;
	showAddSubProductButton.value = false;  // 隐藏添加子产品按钮
	Productform.productCode = row.productCode;
	Productform.productBarcode = row.productBarcode;
	Productform.chineseProductName = row.chineseProductName;
	Productform.englishProductName = row.englishProductName;
	Productform.chineseSpecification = row.chineseSpecification;
	Productform.englishSpecification = row.englishSpecification;
	Productform.unit = state.optionss.hr_calculate_unit.find((dict) => dict.dictLabel === row.unitOfMeasurement)?.dictValue;
	Productform.customsCode = row.customsCode;
	Productform.chineseDeclarationProductName = row.chineseDeclarationProductName;
	Productform.englishDeclarationProductName = row.englishDeclarationProductName;
	Productform.inspectionMark = row.inspectionMark;
	Productform.stockQuantity = row.stockQuantity;
	Productform.developmentEventDate = row.developmentEventDate;
	Productform.recentRecommendation = row.recentRecommendation;
	Productform.recentSampleShipment = row.recentSampleShipment;
	Productform.recentQuotation = row.recentQuotation;
	Productform.recentTransactionDate = row.recentTransactionDate;
	Productform.ProductPhoto = row.productPhotoPath;
	Productform.productLength = row.productLength;
	Productform.productwidth = row.productWidth;
	Productform.productheight = row.productHeight;
	Productform.productweight = row.productWeight;
	Productform.mediumpackagingvolume = row.mediumPackagingVolume;
	Productform.outerboxpackingquantity = row.outerBoxPackingQuantity;
	Productform.outerboxlength = row.outerBoxLength;
	Productform.outerboxwidth = row.outerBoxWidth;
	Productform.outerboxheight = row.outerBoxHeight;
	Productform.outerboxvolume = row.outerBoxVolume;
	Productform.outerboxnetweight = row.outerBoxNetWeight;
	Productform.outerboxgrossweight = row.outerBoxGrossWeight;
	Productform.PackingMethod = state.optionss.hr_packing.find((dict) => dict.dictValue === row.packingMethod.toString())?.dictValue;
	if (row.productPhotoPath != null && row.productPhotoPath != '') {
		row.productPhotoPath.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false
				});
				filelistUrlStr.value += (index > 0 ? ',' : '') + url;
			}
		});
	}
	SubProductTableData.value = [];
	row.subProductItems.forEach((item) => {
		const subProductImages = item.subProductImages ? item.subProductImages.split(',').map((url, index) => ({
			name: `Image ${index + 1}`,
			url: url,
			isChanged: false
		})) : [];
		SubProductTableData.value.push({
			ID: item.id,
			mainProductCode: item.subProductCode.split('-')[0],
			subProductCode: item.subProductCode.split('-')[1], // 只保留 "-" 后面的部分
			subproductImages: subProductImages,
			currentImageIndex: 0,
			subproductBarcode: item.subProductBarcode,
			subchineseProductName: item.subchineseProductName,
			subenglishProductName: item.subenglishProductName,
			subchineseSpecification: item.subchineseSpecification,
			subenglishSpecification: item.subenglishSpecification,
			subunit: state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === item.subunit.toString())?.dictValue,
			subcustomsCode: item.subcustomsCode,
			subchineseDeclarationProductName: item.subchineseDeclarationProductName,
			subenglishDeclarationProductName: item.subenglishDeclarationProductName,
			subinspectionMark: item.subinspectionMark,
			substockQuantity: item.substockQuantity,
			subdevelopmentEventDate: item.subdevelopmentEventDate,
			subrecentRecommendation: item.subrecentRecommendation,
			subrecentSampleShipment: item.subrecentSampleShipment,
			subrecentQuotation: item.subrecentQuotation,
			subrecentTransactionDate: item.subrecentTransactionDate,
			subproductLength: item.subproductLength,
			subproductWidth: item.subproductWidth,
			subproductHeight: item.subproductHeight,
			subproductWeight: item.subproductWeight,
			submediumPackagingVolume: item.submediumPackagingVolume,
			subouterBoxPackingQuantity: item.subouterBoxPackingQuantity,
			subouterBoxLength: item.subouterBoxLength,
			subouterBoxWidth: item.subouterBoxWidth,
			subouterBoxHeight: item.subouterBoxHeight,
			subouterBoxVolume: item.subouterBoxVolume,
			subouterBoxNetWeight: item.subouterBoxNetWeight,
			subouterBoxGrossWeight: item.subouterBoxGrossWeight,
			subPackingMethod: state.optionss.hr_packing.find((dict) => dict.dictValue === item.subPackingMethod.toString())?.dictValue
		});
	});
	uploadedFiles.value = fileList.value;
	EditProductID.value = row.id;
	AddProductDialog.value = true;
	runOnPageLoad();
}
const EditProductID = ref(0);
const EditProductinfomation = () => {
	isViewMode.value = false;
	showEditSaveBtn.value = true;
	showEditBtn.value = false;
	isDisabled.value = false;
	showAddSubProductButton.value = true;
}


const EditSaveProductinfomation = async () => {
	try {
		const editProductInfoRequest = {
			id: EditProductID.value,
			productCategoriesID: SelectNodeId.value,
			productCode: Productform.productCode,
			productBarcode: Productform.productBarcode,
			chineseProductName: Productform.chineseProductName,
			englishProductName: Productform.englishProductName,
			chineseSpecification: Productform.chineseSpecification,
			englishSpecification: Productform.englishSpecification,
			unitOfMeasurement: Productform.unit,
			customsCode: Productform.customsCode,
			chineseDeclarationProductName: Productform.chineseDeclarationProductName,
			englishDeclarationProductName: Productform.englishDeclarationProductName,
			inspectionMark: Productform.inspectionMark,
			stockQuantity: Number(Productform.stockQuantity),
			developmentEventDate: Productform.developmentEventDate,
			RecentRecommendation: Productform.recentRecommendation || '',
			RecentSampleShipment: Productform.recentSampleShipment || '',
			RecentQuotation: Productform.recentQuotation || '',
			recentTransactionDate: Productform.recentTransactionDate,
			productPhotoPath: '',
			ProductLength: Productform.productLength,
			ProductWidth: Productform.productwidth,
			ProductHeight: Productform.productheight,
			ProductWeight: Productform.productweight,
			MediumPackagingVolume: Productform.mediumpackagingvolume,
			OuterBoxPackingQuantity: Productform.outerboxpackingquantity,
			OuterBoxLength: Productform.outerboxlength,
			OuterBoxWidth: Productform.outerboxwidth,
			OuterBoxHeight: Productform.outerboxheight,
			OuterBoxVolume: Productform.outerboxvolume,
			OuterBoxNetWeight: Productform.outerboxnetweight,
			OuterBoxGrossWeight: Productform.outerboxgrossweight,
			PackingMethod: Productform.PackingMethod,
			Remark: '',
			subProductItems: []
		};

		// 处理主产品图片
		let mainProductImageUrls = [];
		if (Array.isArray(fileList.value) && fileList.value.length > 0) {
			mainProductImageUrls = await Promise.all(fileList.value.map(async (file) => {
				if (file.isChanged) {
					const response = await uploadProductPhoto(file);
					return response.data.url;
				} else {
					return file.url; // 如果图片没有变动，直接使用原有的 URL
				}
			}));
		}
		editProductInfoRequest.productPhotoPath = mainProductImageUrls.join(',');

		// 处理子产品
		if (Array.isArray(SubProductTableData.value) && SubProductTableData.value.length > 0) {
			// 先进行所有子产品的验证
			const validationErrors = SubProductTableData.value.flatMap((subProduct, index) => {
				const errors = [];
				if (subProduct.subProductCode == '' || subProduct.subProductCode == null || subProduct.subProductCode == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入子产品编号;`);
				}
				if (subProduct.subchineseProductName == '' || subProduct.subchineseProductName == null || subProduct.subchineseProductName == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入中文品名;`);
				}
				if (subProduct.subenglishProductName == '' || subProduct.subenglishProductName == null || subProduct.subenglishProductName == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入英文品名;`);
				}
				if (subProduct.subchineseSpecification == '' || subProduct.subchineseSpecification == null || subProduct.subchineseSpecification == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入中文规格;`);
				}
				if (subProduct.subunit == '' || subProduct.subunit == null || subProduct.subunit == undefined) {
					errors.push(`子产品 ${index + 1}: 请选择计量单位;`);
				}
				if (subProduct.subPackingMethod == '' || subProduct.subPackingMethod == null || subProduct.subPackingMethod == undefined) {
					errors.push(`子产品 ${index + 1}: 请选择包装方式;`);
				}
				if (subProduct.subcustomsCode == '' || subProduct.subcustomsCode == null || subProduct.subcustomsCode == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入海关编码;`);
				}
				if (subProduct.subchineseDeclarationProductName == '' || subProduct.subchineseDeclarationProductName == null || subProduct.subchineseDeclarationProductName == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入中文报关品名;`);
				}
				if (subProduct.subenglishDeclarationProductName == '' || subProduct.subenglishDeclarationProductName == null || subProduct.subenglishDeclarationProductName == undefined) {
					errors.push(`子产品 ${index + 1}: 请输入英文报关品名;`);
				}
				if (subProduct.subinspectionMark == '' || subProduct.subinspectionMark == null || subProduct.subinspectionMark == undefined) {
					errors.push(`子产品 ${index + 1}: 请选择商检标志;`);
				}
				return errors;
			});
			// 如果有验证错误，显示错误信息并中断保存过程
			if (validationErrors.length > 0) {
				ElMessage({
					message: validationErrors.join('\r\n'),
					type: 'warning',
					duration: 5000,
					showClose: true
				});
				return; // 中断保存过程
			}
			editProductInfoRequest.subProductItems = await Promise.all(SubProductTableData.value.map(async subProduct => {
				// 上传子产品图片
				let subProductImageUrls = [];
				if (Array.isArray(subProduct.subproductImages) && subProduct.subproductImages.length > 0) {
					subProductImageUrls = await Promise.all(subProduct.subproductImages.map(async (file) => {
						if (file.isChanged) {
							const response = await uploadProductPhoto(file);
							return response.data.url;
						} else {
							return file.url; // 如果图片没有变动，直接使用原有的 URL
						}
					}));
				}

				return {
					ID: subProduct.ID,
					mainProductCode: editProductInfoRequest.productCode,
					subProductCode: editProductInfoRequest.productCode + '-' + subProduct.subProductCode,
					subProductImages: subProductImageUrls.join(','),
					subProductBarcode: subProduct.subproductBarcode,
					subChineseProductName: subProduct.subchineseProductName,
					subEnglishProductName: subProduct.subenglishProductName,
					subChineseSpecification: subProduct.subchineseSpecification,
					subEnglishSpecification: subProduct.subenglishSpecification,
					subUnit: subProduct.subunit,
					subCustomsCode: subProduct.subcustomsCode,
					subChineseDeclarationProductName: subProduct.subchineseDeclarationProductName,
					subEnglishDeclarationProductName: subProduct.subenglishDeclarationProductName,
					subInspectionMark: subProduct.subinspectionMark,
					subStockQuantity: subProduct.substockQuantity,
					subDevelopmentEventDate: subProduct.subdevelopmentEventDate,
					subRecentRecommendation: subProduct.subrecentRecommendation,
					subRecentSampleShipment: subProduct.subrecentSampleShipment,
					subRecentQuotation: subProduct.subrecentQuotation,
					subRecentTransactionDate: subProduct.subrecentTransactionDate,
					subProductLength: subProduct.subproductLength,
					subProductWidth: subProduct.subproductWidth,
					subProductHeight: subProduct.subproductHeight,
					subProductWeight: subProduct.subproductWeight,
					subMediumPackagingVolume: subProduct.submediumPackagingVolume,
					subOuterBoxPackingQuantity: subProduct.subouterBoxPackingQuantity,
					subOuterBoxLength: subProduct.subouterBoxLength,
					subOuterBoxWidth: subProduct.subouterBoxWidth,
					subOuterBoxHeight: subProduct.subouterBoxHeight,
					subOuterBoxVolume: subProduct.subouterBoxVolume,
					subOuterBoxNetWeight: subProduct.subouterBoxNetWeight,
					subOuterBoxGrossWeight: subProduct.subouterBoxGrossWeight,
					subPackingMethod: subProduct.subPackingMethod
				};
			}));
		}

		const response = await request.post('ProductInformation/UpdateProductInfo/Edit', editProductInfoRequest);
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			});
			filelistUrlStr.value = '';
			AddProductDialog.value = false;
			GetProductInfoList(currentPage.value, pageSize.value);
		} else {
			throw new Error(response.data.message || '保存失败');
		}
	} catch (error) {
		console.error('编辑保存产品信息时出错：', error);
		ElMessage({
			message: '编辑保存产品信息失败: ' + error.message,
			type: 'error'
		});
	}
};

</script>
<style>
.image-preview-container {
	display: flex;
	align-items: center;
	justify-content: center;
}

.image-preview-container .el-button {
	margin: 0 10px;
}

.ellipsis {
	display: inline-block;
	max-width: 100px;
	/* 根據需要調整寬度 */
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
	vertical-align: middle;
}

.container {
	display: flex;
	border: 1px solid #ca1818;
}

.tree-container {
	width: 25%;
}

.table-container {
	width: 70%;
}

.layout-container-demo .el-header {
	position: relative;
	background-color: var(--el-color-primary-light-7);
	color: var(--el-text-color-primary);

}

.layout-container-demo .el-aside {
	color: var(--el-text-color-primary);
	/*background: var(--el-color-primary-light-8);*/
}

.layout-container-demo .el-menu {
	border-right: none;
}

.layout-container-demo .el-main {
	padding: 0;
}

.layout-container-demo .toolbar {
	display: inline-flex;
	align-items: center;
	justify-content: center;
	height: 100%;
	right: 20px;
}
</style>