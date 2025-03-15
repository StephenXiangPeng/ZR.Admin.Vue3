<template>
	<div>
		<el-container class="layout-container-demo">
			<el-aside width="300px">
				<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;产品分类</span>
				<el-divider></el-divider>
				<el-button size="small" plain @click="openAddproductCategoriesMessageBox()"
					v-if="userId.toString() === '1'">添加分类</el-button>
				<el-button size="small" plain @click="DelproductCategoriesMessageBox()"
					v-if="userId.toString() === '1'">删除分类</el-button>
				<el-tree-v2 :data="ProductCategoriesTreeData" style="font-size: 15px;" :height="700"
					@node-click="handleNodeClick" @node-contextmenu="handleRightClick" draggable :allow-drop="allowDrop"
					@node-drag-start="handleDragStart" @node-drag-enter="handleDragEnter"
					@node-drag-leave="handleDragLeave" @node-drag-end="handleDragEnd" @node-drop="handleDrop">
					<template #default="{ node }">
						<span class="prefix" :class="{ 'is-leaf': node.isLeaf }">
							<el-icon>
								<Folder />
							</el-icon>
						</span>
						<!-- <el-tooltip :content="node.label" placement="top" style="ellipsis"> -->
						<span>{{ node.label }}</span>
						<!-- </el-tooltip> -->
					</template>
				</el-tree-v2>

				<!-- 右键菜单 -->
				<ul v-show="contextMenuVisible" :style="{ left: contextMenuLeft + 'px', top: contextMenuTop + 'px' }"
					class="context-menu">
					<li @click="renameProductCategory" v-if="userId.toString() === '1'">
						<el-icon>
							<Edit />
						</el-icon>
						重命名
					</li>
					<li @click="openAddproductCategoriesMessageBox" v-if="userId.toString() === '1'">
						<el-icon>
							<Plus />
						</el-icon>
						添加子分类
					</li>
					<li @click="DelproductCategoriesMessageBox" v-if="userId.toString() === '1'">
						<el-icon>
							<Delete />
						</el-icon>
						删除分类
					</li>
				</ul>
			</el-aside>
			<el-container>
				<el-main>
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
					<el-divider></el-divider>
					<div style="width: 100%; margin-top: 30px;">
						<el-input v-model="Search_ProductCode" clearable style="width: 20%" size="large"
							placeholder="输入产品编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-date-picker v-model="Search_StartTransactionDate" type="date" placeholder="请选择最近成交日期"
							size="large"
							style="width: 20%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-date-picker v-model="Search_EndTransactionDate" type="date" placeholder="请选择最近成交日期"
							size="large"
							style="width: 20%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<el-button type="primary" @click="Search_ProductInfo()" plain>查询</el-button>
						<el-button @click="Search_Reset()">重置</el-button>
					</div>
					<div style="margin-top: 30px;"></div>
					<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;产品列表</span>
					<el-divider></el-divider>
					<el-button type="primary" @click="openAddProductDialog()"
						v-if="userId.toString() === '1'">添加产品</el-button>
					<el-table :data="ProductInfoTableData" row-key="id"
						:tree-props="{ children: 'children', hasChildren: 'hasChildren' }">
						<el-table-column prop="productCode" label="产品编号" width="200" sortable></el-table-column>
						<el-table-column prop="chineseProductName" label="中文品名" width="200"></el-table-column>
						<el-table-column prop="chineseSpecification" label="中文规格" width="200"></el-table-column>
						<el-table-column prop="englishProductName" label="英文品名" width="200"></el-table-column>
						<el-table-column prop="unitOfMeasurement" label="计量单位" width="200"></el-table-column>
						<el-table-column label="产品图片" width="200">
							<template #default="scope">
								<el-image v-if="scope.row.productPhotoPath"
									:src="getFirstImageUrl(scope.row.productPhotoPath)"
									style="width: 50px; height: 50px; object-fit: cover;"
									:preview-src-list="getImageUrlList(scope.row.productPhotoPath)" :initial-index="0"
									fit="cover" :preview-teleported="true">
								</el-image>
							</template>
						</el-table-column>
						<el-table-column label="最近成交" width="200">
							<template #default="scope">
								{{ formatDate(scope.row.recentTransactionDate) }}
							</template>
						</el-table-column>
						<el-table-column fixed="right" label="操作" width="200">
							<template #default="scope">
								<el-button link type="primary" size="small" v-if="!scope.row.isSubProduct"
									@click="OpenProductInfoDetailDialog(scope.row)">查看详情</el-button>
								<el-button link type="primary" size="small"
									v-if="isDelteBtnShow && userId.toString() === '1' && !scope.row.isSubProduct"
									@click="DeleteProduct(scope.row)">删除产品</el-button>
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
								style="width: 300px;" ref="productCodeInput" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="客户货号" prop="customerGoodsNumber">
							<el-input v-model="Productform.customerGoodsNumber" :disabled="isDisabled"
								placeholder="请输入客户货号" style="width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="产品条码">
							<el-input v-model="Productform.productBarcode" :disabled="isDisabled" placeholder="请输入产品条码"
								style="width: 300px;" />
						</el-form-item>
					</el-col>

				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="中文品名" prop="chineseProductName">
							<el-input v-model="Productform.chineseProductName" :disabled="isDisabled"
								placeholder="请输入中文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
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

				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="英文规格">
							<el-input v-model="Productform.englishSpecification" :disabled="isDisabled"
								placeholder="请输入英文规格" style="width: 300px;" />
						</el-form-item>
					</el-col>
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
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="报关中文品名" prop="chineseDeclarationProductName">
							<el-input v-model="Productform.chineseDeclarationProductName" :disabled="isDisabled"
								placeholder="请输入报关中文品名" style="width: 300px;" />
						</el-form-item>
					</el-col>
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
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="包装方式" prop="PackingMethod">
							<el-select v-model="Productform.PackingMethod" :disabled="isDisabled" placeholder="选择包装方式"
								style="width: 300px;">
								<el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="所属供应商" prop="Supplier">
							<el-select v-model="Productform.Supplier" multiple clearable filterable
								:disabled="isDisabled" placeholder="选择供应商" style="width: 300px;">
								<el-option v-for="dict in optionss.sql_supplier_info" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="库存数量">
							<el-input v-model="Productform.stockQuantity" disabled placeholder=""
								style="width: 300px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="开发时间日期">
							<el-date-picker v-model="Productform.developmentEventDate" type="date" disabled
								style=" width: 300px;" />
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="开发人员">
							<el-select v-model="Productform.developmentPersonnel" placeholder="" :disabled="isDisabled"
								style="width: 300px;">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
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
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="所属分类" prop="ProductCategories">
							<el-cascader v-model="Productform.ProductCategories" :disabled="isDisabled"
								:options="Productoptions" :props="props1" clearable style="width: 300px;"
								@change="handleCategoryChange" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="16">
						<el-form-item label="产品照片">
							<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList"
								:limit="3" :disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl"
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
					<el-col :span="24">
						<el-form-item label="产品描述">
							<el-input v-model="Productform.productDescription" :disabled="isDisabled"
								style="width: 1200px;" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-if="false">
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
				<el-button type="primary" @click="AddSubProduct()"
					v-if="showAddSubProductButton && userId.toString() === '1'" :disabled="isDisabled">添加子产品</el-button>
				<el-table :data="SubProductTableData" style="width: 100%; height: 550px;">
					<el-table-column prop="mainProductCode" label="主产品编号" width="150" align="center" v-if="false">
						<template #default="scope">
							<span>{{ scope.row.mainProductCode }}</span>
						</template>
					</el-table-column>
					<el-table-column prop="subProductCode" label="产品编号" width="300" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subProductCode" style="max-width:250px" placeholder="请输入产品编号"
								:disabled="isDisabled">
								<template #prepend>{{ Productform.productCode + "-" }}</template>
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subcustomerGoodsNumber" label="客户货号" width="300" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subcustomerGoodsNumber" style="max-width:200px"
								:disabled="isDisabled" placeholder="客户货号">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subProductFiles" label="子产品附件" width="200" align="center">
						<template #default="scope">
							<el-upload ref="uploadProductFileRef" class="upload-demo" :auto-upload="false" :limit="3"
								:show-file-list="true" :file-list="scope.row.productFiles || []"
								:on-change="(file, fileList) => handleSubProductFileChange(file.raw, fileList, scope.$index)"
								:on-remove="(file) => handleSubProductFileRemove(file, fileList, scope.$index)"
								:on-preview="handleFileDownload" :disabled="isDisabled">
								<template #trigger>
									<el-button type="primary" icon="Plus" size="default"
										:disabled="isDisabled || (scope.row.productFiles && scope.row.productFiles.length >= 3)"
										v-if="SelectFileView && userId.toString() === '1'">
										选择附件
									</el-button>
								</template>
								<template #tip>
									<div v-if="scope.row.productFiles && scope.row.productFiles.length >= 3"
										class="el-upload__tip">
										已达到最大附件数量
									</div>
								</template>
							</el-upload>
						</template>
					</el-table-column>
					<el-table-column prop="subproductImage" label="产品图片" width="200" align="center">
						<template #default="scope">
							<el-upload :id="`upload-${scope.$index}`" ref="uploadRefs" :auto-upload="false"
								:show-file-list="true" :on-change="(file) => handleImageSelect(file, scope.$index)"
								:on-remove="(file) => handleImageRemove(file, scope.$index)" :limit="3" accept="image/*"
								multiple list-type="text" :file-list="scope.row.subproductImages || []">
								<el-button
									v-if="!isViewMode && (!scope.row.subproductImages || scope.row.subproductImages.length < 3) && userId.toString() === '1'"
									type="primary" icon="Plus" size="default">
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
									@click="deleteCurrentImage(scope.$index)" size="small">删除</el-button>
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
					<el-table-column prop="subcustomsCode" label="海关编码" width="200" align="center" v-if="false">
						<template #default="scope">
							<el-input v-model="scope.row.subcustomsCode" style="max-width:200px" :disabled="isDisabled"
								placeholder="请输入子产品海关编码">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subchineseDeclarationProductName" label="报关中文品名" width="200" align="center"
						v-if="false">
						<template #default="scope">
							<el-input v-model="scope.row.subchineseDeclarationProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品报关中文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subenglishDeclarationProductName" label="报关英文品名" width="200" align="center"
						v-if="false">
						<template #default="scope">
							<el-input v-model="scope.row.subenglishDeclarationProductName" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品报关英文品名">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subinspectionMark" label="商检标志" width="120" align="center" v-if="false">
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
					<el-table-column prop="subrecentRecommendation" label="最近推荐" width="200" align="center"
						v-if="false">
						<template #default="scope">
							<el-input v-model="scope.row.subrecentRecommendation" style="max-width:200px" disabled
								placeholder="请输入子产品最近推荐">
							</el-input>
						</template>
					</el-table-column>
					<el-table-column prop="subrecentSampleShipment" label="最近寄样" width="150" align="center"
						v-if="false">
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
					<el-table-column prop="subProductRemark" label="备注" width="200" align="center">
						<template #default="scope">
							<el-input v-model="scope.row.subProductRemark" style="max-width:200px"
								:disabled="isDisabled" placeholder="请输入子产品备注">
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
							<el-table-column prop="" label="供应商编号" width="150" v-if="false"></el-table-column>
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
					<el-button type="primary" v-if="showSaveBtn && userId.toString() === '1'"
						@click="SaveProductinfomation(ProductformRef)">
						确定保存
					</el-button>
					<el-button type="primary" v-if="showEditBtn && userId.toString() === '1'"
						@click="EditProductinfomation()">
						编辑
					</el-button>
					<el-button type="primary" v-if="showEditSaveBtn && userId.toString() === '1'"
						@click="EditSaveProductinfomation()">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">

import { createApp, getCurrentInstance, reactive, toRefs, ref, callWithAsyncErrorHandling, nextTick, onMounted, onBeforeUnmount, computed, watch } from 'vue'
import { ElMessage, ElMessageBox, ElForm, ElTable, ElDialog, ElDivider, ElButton, ElInput, ElSelect, ElOption, ElDatePicker, ElCascader, ElRow, ElCol, ElFormItem, ElTableColumn, ElImage, ElPagination, ElTabs, ElTabPane, ElUpload, ElIcon, ElTree, ElTreeV2 } from 'element-plus'
import { Plus, Delete, Edit, Folder, ArrowLeft, ArrowRight } from '@element-plus/icons-vue'
import useUserStore from '@/store/modules/user'
import { useDict } from '@/utils/dict'
import request from '@/utils/request'
import { FormInstance, FormRules, UploadProps, UploadUserFile, UploadRawFile, UploadFile, UploadFiles } from 'element-plus'
import { ElNotification } from 'element-plus'
//初始运行钩子



// 获取第一张图片URL
const getFirstImageUrl = (imagePathString) => {
	if (!imagePathString) return '';
	const urls = imagePathString.split(',');
	return urls[0] || '';
};

// 获取所有图片URL列表
const getImageUrlList = (imagePathString) => {
	if (!imagePathString) return [];
	return imagePathString.split(',').filter(url => url.trim() !== '');
};

//获取当前登录用户信息
const userId = useUserStore().userId;
//#region 子产品附件
const SelectFileView = ref(true);
const handleSubProductFileChange = (file, fileList, index) => {
	// 检查文件大小
	const isLt100M = file.size / 1024 / 1024 < 100;
	if (!isLt100M) {
		ElMessage.error('文件大小不能超过 100MB!');
		return false;
	}

	// 确保 productFiles 数组存在
	if (!SubProductTableData.value[index].productFiles) {
		SubProductTableData.value[index].productFiles = [];
	}

	// 直接使用 fileList 更新整个文件列表
	SubProductTableData.value[index].productFiles = fileList.map(f => {
		// 检查是否为已存在的文件（有originalPath属性）
		const existingFile = SubProductTableData.value[index].productFiles.find(
			ef => ef.name === f.name && ef.originalPath
		);

		if (existingFile) {
			// 返回原有文件
			return existingFile;
		}

		// 处理新文件
		return {
			name: f.name,
			raw: f.raw || f,
			isChanged: true,
			url: URL.createObjectURL(f.raw || f)
		};
	});

	console.log('更新后的文件列表:', SubProductTableData.value[index].productFiles);
	return true;
};

// 在编辑保存时处理文件
const processSubProductFiles = async (subProduct) => {
	let fileUrls = [];

	// 处理文件
	if (Array.isArray(subProduct.productFiles)) {
		// 处理新文件和保留的原有文件
		fileUrls = await Promise.all(subProduct.productFiles.map(async (file) => {
			if (file.isChanged) {
				// 新文件，需要上传
				const response = await uploadSubProductFile(file.raw);
				return response.data.downloadurl;
			} else {
				// 原有文件，返回原始路径
				return file.originalPath;
			}
		}));
	}

	// 返回需要删除的文件和新的文件URL列表
	return {
		deletedFiles: subProduct.deletedFiles || [],
		fileUrls: fileUrls
	};
};

// 处理子产品文件删除
const handleSubProductFileRemove = (file, fileList, index) => {
	if (!SubProductTableData.value[index].productFiles) return;

	const fileIndex = SubProductTableData.value[index].productFiles.findIndex(
		item => item.name === file.name
	);

	if (fileIndex > -1) {
		SubProductTableData.value[index].productFiles.splice(fileIndex, 1);
	}
	if (SubProductTableData.value[index].productFiles.length < 3) {
		SelectFileView.value = true;
	}
};

// 检查文件是否在所有子产品中重复的辅助函数
const isFileExistInSubProducts = (fileName, excludeIndex = -1) => {
	return SubProductTableData.value.some((subProduct, index) => {
		if (index === excludeIndex) return false;
		return subProduct.productFiles?.some(file => file.name === fileName);
	});
};

//#endregion
onMounted(() => {
	// 这里编写进入页面时要运行的函数
	runOnPageLoad();

	// 添加点击事件监听，关闭右键菜单
	const closeContextMenuOnClick = () => {
		contextMenuVisible.value = false;
	};

	// 防止在页面其他区域显示默认右键菜单
	const preventDefaultContextMenu = (e) => {
		if (contextMenuVisible.value) {
			e.preventDefault();
		}
	};

	// 添加点击事件监听，关闭右键菜单
	document.addEventListener('click', closeContextMenuOnClick);

	// 添加右键点击事件监听，防止在页面其他区域显示默认右键菜单
	document.addEventListener('contextmenu', preventDefaultContextMenu);

	// 在组件卸载前移除事件监听
	onBeforeUnmount(() => {
		document.removeEventListener('click', closeContextMenuOnClick);
		document.removeEventListener('contextmenu', preventDefaultContextMenu);
	});
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
		subproductLength: Productform.productLength ? Productform.productLength : 0,
		subproductWidth: Productform.productwidth ? Productform.productwidth : 0,
		subproductHeight: Productform.productheight ? Productform.productheight : 0,
		subproductWeight: Productform.productweight ? Productform.productweight : 0,
		submediumPackagingVolume: Productform.mediumpackagingvolume ? Productform.mediumpackagingvolume : 0,
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
		subPackingMethod: state.optionss.hr_packing.find((dict) => dict.dictValue === Productform.PackingMethod)?.dictValue,
		subCustomerGoodsNumber: Productform.customerGoodsNumber
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


const activeTab = ref('FactoryQuotationTab');
const FactoryQuotationTableData = ref([]);
const SaleHistoryTableData = ref([]);
const PurchaseHistoryTableData = ref([]);
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
	clearProductform();
	Productform.ProductCategories = SelectNodeId.value;
	Productform.developmentEventDate = new Date().toISOString().split('T')[0];
	showSaveBtn.value = true;
	AddProductDialog.value = true;
}

const closeAddProductDialog = async () => {
	if (userId.toString() === '1') {
		await removeProductEditLock(EditProductID.value);
	}
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
	Productform.productDescription = '';
	Productform.customerGoodsNumber = '';
	Productform.Supplier = '';
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
		hr_inspectionmark: [],
		sql_supplier_info: [],
		sql_all_user: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_packing' }, { dictType: 'hr_calculate_unit' },
{ dictType: 'hr_inspectionmark' }, { dictType: 'sql_supplier_info' }, { dictType: 'sql_all_user' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	GetProductInfoList(currentPage.value, pageSize.value);
})

const handleCategoryChange = (value) => {
	// 如果是数组，取最后一个值（通常是叶子节点）
	if (Array.isArray(value)) {
		Productform.ProductCategories = value[value.length - 1];
		SelectNodeId.value = value[value.length - 1];
	} else {
		Productform.ProductCategories = value;
		SelectNodeId.value = value;
	}
}

// 定义级联选择器的数据和配置
const Productoptions = ref([])
const props1 = {
	value: 'value',
	label: 'label',
	children: 'children',
	checkStrictly: true,  // 可选，是否严格的遵守父子节点不互相关联
	emitPath: true       // 可选，是否返回选中节点的完整路径
}

// 获取产品分类树
const getProductCategories = async () => {
	try {
		const response = await request({
			url: 'ProductInformation/GetProductCategories/GetAllProductCategories',
			method: 'get'
		})

		if (response.code === 200) {
			Productoptions.value = response.data
		} else {
			ElMessage.error(response.msg || '获取产品分类失败')
		}
	} catch (error) {
		console.error('获取产品分类失败:', error)
		ElMessage.error('获取产品分类失败')
	}
}
getProductCategories();

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
	developmentPersonnel: number | null;
	recentRecommendation: string;
	recentSampleShipment: string;
	recentQuotation: string;
	recentTransactionDate: string;
	ProductPhoto: string;
	PackingMethod: string;
	Supplier: string;
	ProductCategories: number;
	customerGoodsNumber: string;
	//产品属性
	productDescription: string;
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
	subProductItems: typeof SubProductTableData.value;
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
	developmentPersonnel: null,
	recentRecommendation: '',
	recentSampleShipment: '',
	recentQuotation: '',
	recentTransactionDate: '',
	ProductPhoto: '',
	PackingMethod: '',
	Supplier: '',
	ProductCategories: 0,
	customerGoodsNumber: '',
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
	subProductItems: SubProductTableData.value,
	productDescription: ''
})

// 保存原始产品编号，用于编辑时比较
const originalProductCode = ref('');

// 添加产品编号验证器
const validateProductCode = (rule, value, callback) => {
	if (!value) {
		callback(new Error('请输入产品编号'));
		return;
	}

	// 如果是编辑状态且产品编号未修改，则直接通过验证
	if (showEditSaveBtn.value && value === originalProductCode.value) {
		callback();
		return;
	}

	// 返回一个Promise而不是使用async/await
	return new Promise<void>((resolve, reject) => {
		checkProductCode(value).then(isExist => {
			if (isExist) {
				callback(new Error('产品编号已存在，请修改'));
			} else {
				callback();
			}
			resolve();
		}).catch(error => {
			console.error('验证产品编号时出错:', error);
			callback(new Error('验证产品编号时出错'));
			resolve();
		});
	});
};

const ProductformRules = reactive<FormRules<Productform>>({
	productCode: [{ required: true, message: '请输入产品编号', trigger: ['blur', 'change'] },
	{ validator: validateProductCode, trigger: 'blur' }],
	chineseProductName: [{ required: true, message: '请输入中文品名', trigger: ['blur', 'change'] }],
	englishProductName: [{ required: true, message: '请输入英文品名', trigger: ['blur', 'change'] }],
	chineseSpecification: [{ required: true, message: '请输入中文规格', trigger: ['blur', 'change'] }],
	unit: [{ required: true, message: '请选择计量单位', trigger: ['blur', 'change'] }],
	customsCode: [{ required: true, message: '请输入海关编码', trigger: ['blur', 'change'] }],
	chineseDeclarationProductName: [{ required: true, message: '请输入中文申报品名', trigger: ['blur', 'change'] }],
	englishDeclarationProductName: [{ required: true, message: '请输入英文申报品名', trigger: ['blur', 'change'] }],
	inspectionMark: [{ required: true, message: '请选择检验标志', trigger: ['blur', 'change'] }],
	PackingMethod: [{ required: true, message: '请选择包装方式', trigger: ['blur', 'change'] }],
	Supplier: [{ required: true, message: '请选择供应商', trigger: ['blur', 'change'] }],
	ProductCategories: [{ required: true, message: '请选择产品分类', trigger: ['blur', 'change'] }]
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
	currentImageIndex: 0,
	subProductFiles: []
})

const filelistUrlStr = ref('');	// 产品图片
const AddProductDialog = ref(false)	// 添加产品对话框
const UploadUrl = '/api/File/UploadFile'	// 上传图片地址
const fileList = ref<UploadUserFile[]>([]);
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const formData = { filePath: 'product' }; // 上传文件的附加数据
const TableData = ref([])	// 产品列表
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)

// 添加检查产品编号的方法
const checkProductCode = async (productCode) => {
	try {
		const res = await request({
			url: 'ProductInformation/CheckProductCode/CheckProductCode',
			method: 'get',
			params: { ProductCode: productCode }
		});
		return res.data > 0; // 返回true表示编号已存在
	} catch (error) {
		console.error('检查产品编号失败:', error);
		return false;
	}
};



const productCodeInput = ref(null);

const checkProductCodeDuplicate = async () => {
	if (!Productform.productCode) return;
	try {
		const isExist = await checkProductCode(Productform.productCode);
		if (isExist) {
			// 聚焦输入框
			nextTick(() => {
				if (productCodeInput.value) {
					productCodeInput.value.focus();
				}
			});
		} else {

		}
	} catch (error) {
		console.error('验证产品编号时出错:', error);
	}
}

//新增保存产品信息
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
					SupplierID: Array.isArray(Productform.Supplier) ? Productform.Supplier.join(',') : Productform.Supplier,
					ProductDescription: Productform.productDescription,
					subProductItems: [],
					CustomerGoodsNumber: Productform.customerGoodsNumber == null || Productform.customerGoodsNumber == undefined ? '无' : Productform.customerGoodsNumber,
					developers: Productform.developmentPersonnel
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
						}
						//上传子产品附件
						let subProductFileUrls = [];
						if (Array.isArray(subProduct.productFiles) && subProduct.productFiles.length > 0) {
							subProductFileUrls = await Promise.all(subProduct.productFiles.map(async (file) => {
								if (file.isChanged) {
									const response = await uploadSubProductFile(file.raw || file);
									return response.data.downloadurl;
								} else {
									return file.url; // 如果文件没有改变，直接使用原有的URL
								}
							}));
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
							subpackingMethod: subProduct.subPackingMethod,
							subProductFiles: subProductFileUrls.join(','),
							subCustomerGoodsNumber: subProduct.subcustomerGoodsNumber == null || subProduct.subcustomerGoodsNumber == undefined ? '无' : subProduct.subcustomerGoodsNumber,
							developers: subProduct.developers,
							subProductRemark: subProduct.subProductRemark == null || subProduct.subProductRemark == undefined ? '无' : subProduct.subProductRemark
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

const uploadSubProductFile = async (file) => {
	const formData = new FormData();
	formData.append('FileName', file.name || '未命名文件');
	formData.append('FileDir', 'ProductInfo/ProductInfoFile');
	formData.append('FileNameType', '1');
	formData.append('File', file.raw || file);
	formData.append('storeType', '2');
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
	await GetProductInfoList(newPage, pageSize.value);

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

// 格式化日期
const formatDate = (dateString) => {
	if (!dateString || dateString.includes('1900-01-01')) {
		return '暂无记录';
	}
	return dateString.split(' ')[0]; // 只显示日期部分
}

// 处理API返回的数据，转换为树形结构
const processProductData = (data) => {
	return data.map(product => {
		const mainProduct = { ...product, isSubProduct: false };
		// 如果有子产品，将其添加为children
		if (product.subProductItems && product.subProductItems.length > 0) {
			mainProduct.children = product.subProductItems.map(subItem => {
				return {
					...subItem,
					isSubProduct: true,
					// 确保子产品有唯一ID
					id: subItem.id,
					productCode: subItem.subProductCode,
					chineseProductName: subItem.subchineseProductName,
					englishProductName: subItem.subenglishProductName,
					chineseSpecification: subItem.subchineseSpecification,
					unitOfMeasurement: state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === subItem.subunit)?.dictLabel,
					productPhotoPath: subItem.subProductImages,
					recentTransactionDate: subItem.subrecentTransactionDate
				};
			});
		}
		// 删除原始子产品数组，避免数据重复
		delete mainProduct.subProductItems;

		return mainProduct;
	});
};

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
				ProductInfoTableData.value = processProductData(response.data.data);
				ProductInfoTableData.value.forEach((item) => {
					item.unitOfMeasurement = state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === item.unitOfMeasurement)?.dictLabel;
				})
				totalItems.value = response.data.totalNum; // 更新总条数
				currentPage.value = response.data.pageIndex; // 更新当前页
				pageSize.value = response.data.pageSize; // 更新每页条数
				resolve(response.data.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetProductInfoList(start - 1, end);
				} else {
					ProductInfoTableData.value = [];
				}
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
			ElMessage({
				message: response.data,
				type: 'success'
			})
			GetProductInfoList(currentPage.value, pageSize.value);
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


// 获取产品编辑锁状态
const getProductEditLock = async (productId) => {
	try {
		const res = await request({
			url: 'ProductInformation/GetProductInfoEditLock/GetProductInfoEditLock',
			method: 'get',
			params: { ProductID: productId }
		});
		return res; // 返回锁定用户名，如果未锁定则为null
	} catch (error) {
		console.error('获取产品编辑锁失败:', error);
		return null;
	}
};

// 设置产品编辑锁
const setProductEditLock = async (productId) => {
	try {
		const res = await request({
			url: 'ProductInformation/SettingsProductInfoEditLock/SettingsProductInfoEditLock',
			method: 'get',
			params: { ProductID: productId }
		});
		return res.code === 200;
	} catch (error) {
		console.error('设置产品编辑锁失败:', error);
		return false;
	}
};

// 移除产品编辑锁
const removeProductEditLock = async (productId) => {
	try {
		await request({
			url: 'ProductInformation/RemoveProductInfoEditLock/RemoveProductInfoEditLock',
			method: 'get',
			params: { ProductID: productId }
		});
	} catch (error) {
		console.error('移除产品编辑锁失败:', error);
	}
};

const isViewMode = ref(false);
const showAddSubProductButton = ref(true);

//查看详情
const OpenProductInfoDetailDialog = async (row) => {
	// 确定是否为子产品，并获取正确的产品ID
	const isSubProduct = row.isSubProduct;
	const productId = isSubProduct ? row.mainProductId : row.id;
	// 先检查编辑锁
	const lockStatus = await getProductEditLock(productId);
	if (lockStatus.data.isEditLock == true) {
		ElMessageBox.alert(`当前产品正在被${lockStatus.data.editUser}编辑中，请稍后再试！`, '提示', {
			confirmButtonText: '确定',
			showClose: false
		});
		return;
	}
	// 如果是子产品，需要先获取主产品信息
	if (isSubProduct) {
		try {
			const response = await request({
				url: 'ProductInformation/GetProductInfo/GetProductInfo',
				method: 'get',
				params: { ID: productId }
			});

			if (response.code === 200) {
				// 使用主产品信息填充表单
				const mainProduct = response.data;
				fillProductForm(mainProduct);

				// 处理子产品列表
				processSubProducts(mainProduct);

				// 设置当前选中的子产品
				const currentSubIndex = mainProduct.subProductItems.findIndex(item => item.id === row.id);
				if (currentSubIndex !== -1) {
					// 可以在这里添加代码，滚动到或高亮显示当前子产品
					// 例如：currentSubProductIndex.value = currentSubIndex;
				}
			} else {
				ElMessage.error(response.msg || '获取产品详情失败');
				return;
			}
		} catch (error) {
			ElMessage.error('获取产品详情失败');
			return;
		}
	} else {
		// 直接使用当前行数据填充表单
		fillProductForm(row);
		processSubProducts(row);
	}

	// 设置编辑ID和显示对话框
	EditProductID.value = productId;
	AddProductDialog.value = true;
	runOnPageLoad();
}

// 辅助函数：填充产品表单
const fillProductForm = (product) => {
	// 保存原始产品编号
	originalProductCode.value = product.productCode;
	isViewMode.value = true;
	showSaveBtn.value = false;
	showEditBtn.value = true;
	isDisabled.value = true;
	showAddSubProductButton.value = false;  // 隐藏添加子产品按钮

	Productform.productDescription = product.productDescription;
	Productform.ProductCategories = product.productCategoriesID;
	Productform.Supplier = product.supplierId ? product.supplierId.split(',').map(id => id.trim()) : [];
	Productform.productCode = product.productCode;
	Productform.productBarcode = product.productBarcode;
	Productform.chineseProductName = product.chineseProductName;
	Productform.englishProductName = product.englishProductName;
	Productform.chineseSpecification = product.chineseSpecification;
	Productform.englishSpecification = product.englishSpecification;
	Productform.unit = product.unitOfMeasurement == 0 || product.unitOfMeasurement == null || product.unitOfMeasurement == undefined ?
		product.unitOfMeasurement :
		state.optionss.hr_calculate_unit.find((dict) => dict.dictLabel === product.unitOfMeasurement)?.dictValue;
	Productform.customsCode = product.customsCode;
	Productform.chineseDeclarationProductName = product.chineseDeclarationProductName;
	Productform.englishDeclarationProductName = product.englishDeclarationProductName;
	Productform.inspectionMark = product.inspectionMark;
	Productform.stockQuantity = product.stockQuantity;
	Productform.developmentEventDate = product.developmentEventDate;
	Productform.recentRecommendation = product.recentRecommendation;
	Productform.recentSampleShipment = product.recentSampleShipment;
	Productform.recentQuotation = product.recentQuotation;
	Productform.recentTransactionDate = product.recentTransactionDate;
	Productform.ProductPhoto = product.productPhotoPath;
	Productform.productLength = product.productLength;
	Productform.productwidth = product.productWidth;
	Productform.productheight = product.productHeight;
	Productform.productweight = product.productWeight;
	Productform.mediumpackagingvolume = product.mediumPackagingVolume;
	Productform.outerboxpackingquantity = product.outerBoxPackingQuantity;
	Productform.outerboxlength = product.outerBoxLength;
	Productform.outerboxwidth = product.outerBoxWidth;
	Productform.outerboxheight = product.outerBoxHeight;
	Productform.outerboxvolume = product.outerBoxVolume;
	Productform.outerboxnetweight = product.outerBoxNetWeight;
	Productform.outerboxgrossweight = product.outerBoxGrossWeight;
	Productform.PackingMethod = product.packingMethod == 0 || product.packingMethod == null || product.packingMethod == undefined ?
		product.packingMethod :
		state.optionss.hr_packing.find((dict) => dict.dictValue === product.packingMethod.toString())?.dictValue;
	Productform.customerGoodsNumber = product.customerGoodsNumber;
	Productform.developmentPersonnel = product.developers == null || product.developers == undefined ? 0 : state.optionss.sql_all_user.find((dict) => dict.dictValue === product.developers.toString())?.dictValue;

	// 处理产品图片
	fileList.value = [];
	filelistUrlStr.value = '';
	if (product.productPhotoPath != null && product.productPhotoPath != '') {
		product.productPhotoPath.split(',').forEach((url, index) => {
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
}

// 辅助函数：处理子产品
const processSubProducts = (product) => {
	SubProductTableData.value = [];

	// 确保使用正确的子产品数据源
	const subProducts = product.children || product.subProductItems || [];

	subProducts.forEach((item) => {
		const subProductImages = item.subProductImages ? item.subProductImages.split(',').map((url, index) => ({
			name: `Image ${index + 1}`,
			url: url,
			isChanged: false
		})) : [];

		// 处理子产品附件
		const subProductFiles = item.subproductFilepath ?
			item.subproductFilepath.map(fileItem => {
				// 去除路径中的换行符和多余的空白字符
				const cleanPath = fileItem.subproductOriginalFilePath.replace(/[\r\n\s]+/g, '');
				return {
					name: cleanPath.split('/').pop(), // 获取文件名
					url: fileItem.subproductFilepath, // 完整的带签名URL
					originalPath: cleanPath, // 清理后的原始路径
					isChanged: false
				};
			}) : [];

		if (subProductFiles.length >= 3) {
			SelectFileView.value = false;
		}

		// 安全地获取子产品编码
		const fullCode = item.subProductCode || '';
		const codeParts = fullCode.split('-');
		const mainCode = codeParts.length > 0 ? codeParts[0] : '';
		const subCode = codeParts.length > 1 ? codeParts[1] : '';

		SubProductTableData.value.push({
			ID: item.id,
			mainProductCode: mainCode,
			subProductCode: subCode,
			subproductImages: subProductImages,
			currentImageIndex: 0,
			subproductBarcode: item.subProductBarcode,
			subchineseProductName: item.subchineseProductName,
			subenglishProductName: item.subenglishProductName,
			subchineseSpecification: item.subchineseSpecification,
			subenglishSpecification: item.subenglishSpecification,
			subunit: item.subunit == 0 || item.subunit == null || item.subunit == undefined ?
				item.subunit :
				state.optionss.hr_calculate_unit.find((dict) => dict.dictValue === item.subunit.toString())?.dictValue,
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
			subPackingMethod: item.subpackingMethod == 0 || item.subpackingMethod == null || item.subpackingMethod == undefined ?
				item.subPackingMethod :
				state.optionss.hr_packing.find((dict) => dict.dictValue === item.subPackingMethod.toString())?.dictValue,
			productFiles: subProductFiles,
			subcustomerGoodsNumber: item.subCustomerGoodsNumber,
			developers: item.developers == null || item.developers == undefined ? 0 : state.optionss.sql_all_user.find((dict) => dict.dictValue === item.developers.toString())?.dictValue,
			subProductRemark: item.subProductRemark
		});
	});

	uploadedFiles.value = fileList.value;
}

const EditProductID = ref(0);
const EditProductinfomation = () => {
	setProductEditLock(EditProductID.value);
	isViewMode.value = false;
	showEditSaveBtn.value = true;
	showEditBtn.value = false;
	isDisabled.value = false;
	showAddSubProductButton.value = true;
}

//编辑保存产品信息
const EditSaveProductinfomation = async () => {
	try {
		// 添加表单验证
		if (!ProductformRef.value) return;

		// 验证主产品表单
		let isValid = false;
		let validationFields = null;

		await ProductformRef.value.validate((valid, fields) => {
			if (!valid) {
				console.error('表单验证失败:', fields);
				throw new Error('请完善表单信息');
			}
		});
		const editProductInfoRequest = {
			id: EditProductID.value,
			ProductCategoriesID: Productform.ProductCategories,
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
			SupplierID: Array.isArray(Productform.Supplier) ? Productform.Supplier.join(',') : Productform.Supplier,
			ProductDescription: Productform.productDescription,
			subProductItems: [],
			CustomerGoodsNumber: Productform.customerGoodsNumber == null || Productform.customerGoodsNumber == undefined ? '无' : Productform.customerGoodsNumber,
			developers: Productform.developmentPersonnel
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
				// 处理子产品附件
				let subProductFileUrls = [];
				if (Array.isArray(subProduct.productFiles) && subProduct.productFiles.length > 0) {
					subProductFileUrls = await Promise.all(
						subProduct.productFiles
							.filter(file => file && (file.originalPath || file.raw)) // 确保文件有效
							.map(async (file) => {
								if (file.isChanged) {
									// 新上传的文件
									const response = await uploadSubProductFile(file.raw || file);
									return response.data.downloadurl;
								} else {
									// 保留原有文件的原始路径
									return file.originalPath;
								}
							})
					);
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
					subPackingMethod: subProduct.subPackingMethod,
					subProductFiles: subProductFileUrls
						.filter(url => url && url.trim()) // 过滤掉空值和空白字符
						.join(','),
					subCustomerGoodsNumber: subProduct.subcustomerGoodsNumber == null || subProduct.subcustomerGoodsNumber == undefined ? '无' : subProduct.subcustomerGoodsNumber,
					developers: subProduct.developers,
					subProductRemark: subProduct.subProductRemark == null || subProduct.subProductRemark == undefined ? '无' : subProduct.subProductRemark
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
			getProductCategories();
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

const handleFileDownload = (file) => {
	// 创建一个隐藏的 a 标签
	const link = document.createElement('a');
	link.style.display = 'none';
	link.href = file.url;
	link.setAttribute('download', file.name); // 设置下载的文件名

	// 添加到页面并触发点击
	document.body.appendChild(link);
	link.click();

	// 清理 DOM
	document.body.removeChild(link);
};

// 右键菜单相关变量和方法
const contextMenuVisible = ref(false);
const contextMenuLeft = ref(0);
const contextMenuTop = ref(0);
const rightClickedNode = ref(null);

// 处理右键点击事件
const handleRightClick = (event, node) => {
	// 阻止默认右键菜单
	event.preventDefault();
	// 保存当前右键点击的节点
	rightClickedNode.value = node;
	// 设置选中的节点ID和名称
	SelectNodeId.value = node.id;
	SelectedProductCategoriesStr = '【' + node.label + '】';

	// 计算右键菜单的位置
	contextMenuLeft.value = event.clientX;
	contextMenuTop.value = event.clientY;
	// 显示右键菜单
	contextMenuVisible.value = true;

	// 阻止事件冒泡，防止立即触发document的click事件
	event.stopPropagation();
}

// 关闭右键菜单
const closeContextMenu = () => {
	contextMenuVisible.value = false;
}

// 重命名产品分类
const renameProductCategory = () => {
	if (SelectNodeId.value === 0) {
		ElMessage({
			type: 'warning',
			message: '请选择要重命名的产品分类'
		});
		return;
	}

	ElMessageBox.prompt(`请输入新的分类名称`, '重命名分类', {
		confirmButtonText: '保存',
		cancelButtonText: '取消',
		inputValue: rightClickedNode.value ? rightClickedNode.value.label : '',
		inputPattern: /^[a-zA-Z0-9\u4e00-\u9fa5]{1,10000000000000}$/,
		inputErrorMessage: '分类名称不能为空,可以是数字、汉字、字母组合,最多个10000000000000字符!😒',
	})
		.then(({ value }) => {
			// 构建更新请求
			const updateRequest = {
				"id": SelectNodeId.value,
				"name": value
			};

			// 发送更新请求
			request.put('ProductCategories/UpdateProductCategories/UpdateProductCategoriesName', updateRequest).then(response => {
				if (response != null) {
					ElMessage({
						message: response.msg || '重命名成功',
						type: 'success'
					});
					// 刷新产品分类列表
					GetAllParentProductCategoriesList();
				} else {
					console.error('重命名产品分类出错');
				}
			}).catch(error => {
				console.error('重命名产品分类出错！😔错误内容：', error);
			});
		})
		.catch(() => {
			ElMessage({
				type: 'info',
				message: '已取消重命名',
			});
		});
}

// 拖拽相关方法
// 判断是否允许放置
const allowDrop = (draggingNode, dropNode, type) => {
	// 不允许拖放到自己下面
	if (draggingNode.id === dropNode.id) {
		return false;
	}

	// 不允许拖放到自己的子节点下面
	const isChild = isChildOf(dropNode, draggingNode);
	if (isChild) {
		return false;
	}

	// 只允许拖放到其他节点内部，不允许拖放到前后
	return type === 'inner';
}

// 判断一个节点是否是另一个节点的子节点
const isChildOf = (child, parent) => {
	if (!child || !parent) return false;

	let currentNode = child;
	while (currentNode.parentId) {
		if (currentNode.parentId === parent.id) {
			return true;
		}
		// 查找父节点
		const parentNode = findNodeById(ProductCategoriesTreeData.value, currentNode.parentId);
		if (!parentNode) break;
		currentNode = parentNode;
	}
	return false;
}

// 根据ID查找节点
const findNodeById = (nodes, id) => {
	if (!nodes || nodes.length === 0) return null;

	for (const node of nodes) {
		if (node.id === id) {
			return node;
		}
		if (node.children && node.children.length > 0) {
			const found = findNodeById(node.children, id);
			if (found) return found;
		}
	}
	return null;
}

// 开始拖拽
const handleDragStart = (node) => {
	console.log('开始拖拽:', node);
}

// 拖拽进入目标节点
const handleDragEnter = (draggingNode, dropNode, ev) => {
	console.log('拖拽进入:', dropNode);
}

// 拖拽离开目标节点
const handleDragLeave = (draggingNode, dropNode, ev) => {
	console.log('拖拽离开:', dropNode);
}

// 拖拽结束
const handleDragEnd = (draggingNode, dropNode, dropType, ev) => {
	console.log('拖拽结束:', draggingNode, dropNode, dropType);
}

// 放置节点
const handleDrop = (draggingNode, dropNode, dropType, ev) => {
	console.log('放置节点:', draggingNode, dropNode, dropType);

	// 如果不是管理员，不允许拖拽
	if (userId.toString() !== '1') {
		ElMessage.warning('您没有权限移动分类');
		return;
	}

	// 确认是否要移动分类
	ElMessageBox.confirm(`确认将分类【${draggingNode.label}】移动到【${dropNode.label}】下吗？`, '移动分类', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		// 构建更新请求
		const updateRequest = {
			"id": draggingNode.id,
			"parentID": dropNode.id,
			"name": draggingNode.label,
			"isDelete": 0
		};

		// 发送更新请求
		request.put('ProductCategories/UpdateProductCategories/Update', updateRequest).then(response => {
			if (response != null) {
				ElMessage({
					message: response.msg || '移动分类成功',
					type: 'success'
				});
				// 刷新产品分类列表
				GetAllParentProductCategoriesList();
			} else {
				console.error('移动产品分类出错');
			}
		}).catch(error => {
			console.error('移动产品分类出错！😔错误内容：', error);
			// 刷新产品分类列表，恢复原状
			GetAllParentProductCategoriesList();
		});
	}).catch(() => {
		// 取消移动，刷新产品分类列表，恢复原状
		GetAllParentProductCategoriesList();
		ElMessage({
			type: 'info',
			message: '已取消移动'
		});
	});
}

</script>
<style lang="scss" scoped>
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

/* 添加以下样式 */
:deep(.el-image-viewer__wrapper) {
	z-index: 3000 !important;
	/* 使用更高的z-index值 */
}

:deep(.el-image-viewer__mask) {
	z-index: 2999 !important;
}

/* 确保图片在表格中正常显示 */
.el-table .el-image {
	position: relative;
	z-index: 1;
}

/* 当图片被点击预览时，提高其z-index */
.el-table .el-image:hover {
	z-index: 2;
}

/* 右键菜单样式 */
.context-menu {
	margin: 0;
	background: #fff;
	z-index: 3000;
	position: fixed;
	list-style-type: none;
	padding: 5px 0;
	border-radius: 4px;
	font-size: 12px;
	font-weight: 400;
	color: #333;
	box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.3);

	li {
		margin: 0;
		padding: 7px 16px;
		cursor: pointer;
		display: flex;
		align-items: center;

		.el-icon {
			margin-right: 5px;
		}

		&:hover {
			background: #f5f7fa;
		}
	}
}

/* 拖拽相关样式 */
:deep(.is-dragging) {
	opacity: 0.5;
}

:deep(.is-drop-inner) {
	background-color: #f0f9eb;
	border: 1px dashed #67c23a;
}
</style>