<template>
	<div>
		<el-header style="display: flex; justify-content: space-between; align-items: center;">
			<h1>产品内容管理</h1>
			<div class="header-operations">
				<el-input v-model="searchQuery" placeholder="搜索产品名称" style="width: 300px; margin-right: 20px;" />
				<el-button type="primary" @click="handleAddProduct">添加产品</el-button>
			</div>
		</el-header>
		
		<el-row :gutter="20" class="product-list">
			<el-col :span="6" v-for="product in filteredProducts" :key="product.id">
				<el-card :body-style="{ padding: '0px' }" class="product-card">
					<div class="product-image-container">
						<img :src="product.image" alt="product image" class="product-image" />
					</div>
					<div class="product-content">
						<div class="product-title">
							<h2>{{ product.name }}</h2>
							<p>{{ product.nameEn }}</p>
						</div>
						<div class="product-description">{{ product.description }}</div>
						<div class="product-link" v-if="product.link">链接: {{ product.link }}</div>
						<div class="product-actions">
							<el-button type="primary" @click="handleEditProduct(product)">编辑</el-button>
							<el-button type="danger" @click="handleDeleteConfirm(product)">删除</el-button>
						</div>
					</div>
				</el-card>
			</el-col>
		</el-row>

		<!-- 添加/编辑产品对话框 -->
		<el-dialog
			:title="isEdit ? '编辑产品' : '添加产品'"
			v-model="dialogVisible"
			width="50%"
			:before-close="handleClose"
		>
			<el-form :model="productForm" :rules="rules" ref="productFormRef" label-width="100px">
				<el-form-item label="产品名称" prop="name">
					<el-input v-model="productForm.name" />
				</el-form-item>
				<el-form-item label="英文名称" prop="nameEn">
					<el-input v-model="productForm.nameEn" />
				</el-form-item>
				<el-form-item label="产品描述" prop="description">
					<el-input v-model="productForm.description" type="textarea" rows="4" />
				</el-form-item>
				<el-form-item label="详情页链接" prop="link">
					<el-input v-model="productForm.link" placeholder="例如: /product/detail/1">
						<template #prepend>
							<el-select v-model="linkType" style="width: 115px;">
								<el-option label="内部链接" value="internal" />
								<el-option label="外部链接" value="external" />
							</el-select>
						</template>
					</el-input>
				</el-form-item>
				<el-form-item label="产品分类" prop="category">
					<el-select v-model="productForm.category" placeholder="选择产品分类">
						<el-option label="苹果酒" value="cider" />
						<el-option label="果汁" value="juice" />
						<el-option label="水果" value="fruit" />
						<el-option label="其他" value="other" />
					</el-select>
				</el-form-item>
				<el-form-item label="产品图片" prop="image">
					<div class="image-upload-container">
						<el-upload
							class="avatar-uploader"
							action="#"
							:http-request="uploadImage"
							:show-file-list="false"
							:before-upload="beforeAvatarUpload"
							:on-success="handleAvatarSuccess"
						>
							<img v-if="productForm.image" :src="productForm.image" class="avatar" />
							<el-icon v-else class="avatar-uploader-icon"><Plus /></el-icon>
						</el-upload>
						<div class="image-info">
							<el-alert
								title="图片尺寸要求：不超过398×557像素"
								type="info"
								:closable="false"
								show-icon
								style="margin-top: 10px;"
							/>
						</div>
					</div>
				</el-form-item>
				<el-form-item label="显示顺序" prop="order">
					<el-input-number v-model="productForm.order" :min="1" :max="99" />
				</el-form-item>
				<el-form-item label="是否显示" prop="isShow">
					<el-switch v-model="productForm.isShow" />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="dialogVisible = false">取消</el-button>
					<el-button type="primary" @click="submitForm">确定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>	
</template>

<script>
import { Plus } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

export default {
	components: {
		Plus
	},
	data() {
		return {
			products: [
				// 示例数据
				{ 
					id: 1, 
					name: '苹尝', 
					nameEn: 'APPLE CIDER',
					description: '精选优质苹果，采用传统工艺酿造的苹果酒，口感清新独特。',
					image: 'https://via.placeholder.com/398x557?text=AppleCider', 
					link: '/product/detail/1',
					category: 'cider',
					order: 1,
					isShow: true
				},
				{ 
					id: 2, 
					name: '苹果宝贝宴会版', 
					nameEn: '100%NFC苹果汁',
					description: '100%纯天然浓缩苹果汁，无添加，保留水果原有营养成分。',
					image: 'https://via.placeholder.com/398x557?text=AppleJuice', 
					link: '/product/detail/2',
					category: 'juice',
					order: 2,
					isShow: true
				},
				{ 
					id: 3, 
					name: '苹果宝贝', 
					nameEn: 'Apple Baby',
					description: '创新设计的苹果形状果汁饮品，深受儿童喜爱，营养健康。',
					image: 'https://via.placeholder.com/398x557?text=AppleBaby', 
					link: '/product/detail/3',
					category: 'juice',
					order: 3,
					isShow: true
				},
				{ 
					id: 4, 
					name: '林示果', 
					nameEn: 'JIN',
					description: '采用精选林檎苹果酿造的高端苹果酒，口感醇厚，香气独特。',
					image: 'https://via.placeholder.com/398x557?text=Jin', 
					link: '/product/detail/4',
					category: 'cider',
					order: 4,
					isShow: true
				}
			],
			searchQuery: '',
			dialogVisible: false,
			isEdit: false,
			currentId: null,
			linkType: 'internal',
			productForm: {
				name: '',
				nameEn: '',
				description: '',
				image: '',
				link: '',
				category: '',
				order: 1,
				isShow: true
			},
			rules: {
				name: [{ required: true, message: '请输入产品名称', trigger: 'blur' }],
				nameEn: [{ required: true, message: '请输入英文名称', trigger: 'blur' }],
				image: [{ required: true, message: '请上传产品图片', trigger: 'change' }],
				category: [{ required: true, message: '请选择产品分类', trigger: 'change' }]
			}
		};
	},
	computed: {
		filteredProducts() {
			return this.products.filter(product =>
				product.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
				product.nameEn.toLowerCase().includes(this.searchQuery.toLowerCase())
			);
		}
	},
	methods: {
		handleAddProduct() {
			this.isEdit = false;
			this.resetForm();
			this.dialogVisible = true;
			this.linkType = 'internal';
		},
		handleEditProduct(product) {
			this.isEdit = true;
			this.currentId = product.id;
			
			// 判断链接类型
			if (product.link && (product.link.startsWith('http://') || product.link.startsWith('https://'))) {
				this.linkType = 'external';
			} else {
				this.linkType = 'internal';
			}
			
			// 复制产品数据到表单
			this.productForm = {
				name: product.name,
				nameEn: product.nameEn,
				description: product.description,
				image: product.image,
				link: product.link,
				category: product.category,
				order: product.order,
				isShow: product.isShow
			};
			
			this.dialogVisible = true;
		},
		handleDeleteConfirm(product) {
			this.$confirm('确定要删除该产品吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			})
				.then(() => {
					this.deleteProduct(product.id);
				})
				.catch(() => {
					this.$message({
						type: 'info',
						message: '已取消删除'
					});
				});
		},
		deleteProduct(id) {
			const index = this.products.findIndex(item => item.id === id);
			if (index > -1) {
				this.products.splice(index, 1);
				this.$message({
					type: 'success',
					message: '删除成功'
				});
			}
		},
		resetForm() {
			this.productForm = {
				name: '',
				nameEn: '',
				description: '',
				image: '',
				link: '',
				category: '',
				order: 1,
				isShow: true
			};
			if (this.$refs.productFormRef) {
				this.$refs.productFormRef.resetFields();
			}
		},
		uploadImage(options) {
			// 模拟上传过程
			const file = options.file;
			const reader = new FileReader();
			reader.readAsDataURL(file);
			reader.onload = () => {
				this.productForm.image = reader.result;
				options.onSuccess(); // 通知上传组件上传成功
			};
		},
		beforeAvatarUpload(file) {
			// 文件类型验证
			const isImage = file.type.startsWith('image/');
			if (!isImage) {
				ElMessage.error('只能上传图片文件!');
				return false;
			}
			
			// 文件大小验证（限制为2MB）
			const isLt2M = file.size / 1024 / 1024 < 2;
			if (!isLt2M) {
				ElMessage.error('图片大小不能超过2MB!');
				return false;
			}
			
			// 图片尺寸验证
			return new Promise((resolve, reject) => {
				const reader = new FileReader();
				reader.onload = (e) => {
					const img = new Image();
					img.onload = () => {
						if (img.width > 398 || img.height > 557) {
							ElMessage.error(`图片尺寸超出限制(398×557)，当前尺寸：${img.width}×${img.height}`);
							reject();
						} else {
							resolve();
						}
					};
					img.src = e.target.result;
				};
				reader.readAsDataURL(file);
			});
		},
		handleAvatarSuccess() {
			// 通知表单图片已更新
			this.$refs.productFormRef.validateField('image');
		},
		submitForm() {
			this.$refs.productFormRef.validate(valid => {
				if (valid) {
					// 处理链接格式
					let formattedLink = this.productForm.link;
					if (this.linkType === 'external' && formattedLink && !formattedLink.startsWith('http')) {
						formattedLink = 'https://' + formattedLink;
					}
					
					if (this.isEdit) {
						// 更新现有产品
						const index = this.products.findIndex(item => item.id === this.currentId);
						if (index > -1) {
							this.products[index] = {
								...this.products[index],
								name: this.productForm.name,
								nameEn: this.productForm.nameEn,
								description: this.productForm.description,
								image: this.productForm.image,
								link: formattedLink,
								category: this.productForm.category,
								order: this.productForm.order,
								isShow: this.productForm.isShow
							};
							this.$message({
								type: 'success',
								message: '产品更新成功'
							});
						}
					} else {
						// 添加新产品
						const newId = this.getMaxId() + 1;
						this.products.push({
							id: newId,
							name: this.productForm.name,
							nameEn: this.productForm.nameEn,
							description: this.productForm.description,
							image: this.productForm.image,
							link: formattedLink,
							category: this.productForm.category,
							order: this.productForm.order,
							isShow: this.productForm.isShow
						});
						this.$message({
							type: 'success',
							message: '产品添加成功'
						});
					}
					this.dialogVisible = false;
				} else {
					return false;
				}
			});
		},
		getMaxId() {
			return Math.max(...this.products.map(item => item.id), 0);
		},
		handleClose() {
			this.resetForm();
			this.dialogVisible = false;
		}
	}
}
</script>

<style>
.product-list {
	margin-top: 20px;
}
.header-operations {
	display: flex;
	align-items: center;
}
.product-card {
	margin-bottom: 20px;
	transition: all 0.3s;
}
.product-card:hover {
	transform: translateY(-5px);
	box-shadow: 0 10px 15px rgba(0, 0, 0, 0.1);
}
.product-image-container {
	height: 220px;
	overflow: hidden;
	display: flex;
	align-items: center;
	justify-content: center;
}
.product-image {
	max-width: 100%;
	max-height: 100%;
	object-fit: contain;
	transition: transform 0.3s;
}
.product-card:hover .product-image {
	transform: scale(1.05);
}
.product-content {
	padding: 15px;
}
.product-title {
	margin-bottom: 10px;
}
.product-title h2 {
	margin: 0;
	font-size: 18px;
}
.product-title p {
	margin: 5px 0 0;
	color: #909399;
	font-size: 14px;
}
.product-description {
	margin: 10px 0;
	color: #666;
	font-size: 14px;
	display: -webkit-box;
	-webkit-line-clamp: 2;
	-webkit-box-orient: vertical;
	overflow: hidden;
	text-overflow: ellipsis;
}
.product-link {
	font-size: 12px;
	color: #909399;
	margin-bottom: 10px;
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
}
.product-actions {
	display: flex;
	justify-content: flex-end;
	margin-top: 15px;
}
.image-upload-container {
	display: flex;
	flex-direction: column;
}
.avatar-uploader .avatar {
	width: 178px;
	height: 178px;
	display: block;
}
.avatar-uploader .el-upload {
	border: 1px dashed var(--el-border-color);
	border-radius: 6px;
	cursor: pointer;
	position: relative;
	overflow: hidden;
	transition: var(--el-transition-duration-fast);
}
.avatar-uploader .el-upload:hover {
	border-color: var(--el-color-primary);
}
.avatar-uploader-icon {
	font-size: 28px;
	color: #8c939d;
	width: 178px;
	height: 178px;
	text-align: center;
	line-height: 178px;
}
</style>
