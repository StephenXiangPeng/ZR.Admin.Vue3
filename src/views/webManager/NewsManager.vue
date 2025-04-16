<template>
	<div>
		<el-header style="display: flex; justify-content: space-between; align-items: center;">
			<h1>新闻内容管理</h1>
			<div class="header-operations">
				<el-input v-model="searchQuery" placeholder="搜索新闻标题" style="width: 300px; margin-right: 20px;" />
				<el-button type="primary" @click="handleAddNews">添加新闻</el-button>
			</div>
		</el-header>
		
		<el-row :gutter="20" class="news-list">
			<el-col :span="8" v-for="news in filteredNews" :key="news.id">
				<el-card :body-style="{ padding: '20px' }">
					<img :src="news.image" alt="news image" style="width: 100%; height: 200px; object-fit: cover;" />
					<div class="news-content">
						<h2>{{ news.title }}</h2>
						<p>{{ news.date }}</p>
						<div class="news-content-preview" v-html="truncateHTML(news.content)"></div>
						<div class="news-actions">
							<el-button type="primary" @click="handleEditNews(news)">编辑</el-button>
							<el-button type="danger" @click="handleDeleteConfirm(news)">删除</el-button>
						</div>
					</div>
				</el-card>
			</el-col>
		</el-row>

		<!-- 添加/编辑新闻对话框 -->
		<el-dialog
			:title="isEdit ? '编辑新闻' : '添加新闻'"
			v-model="dialogVisible"
			width="80%"
			:before-close="handleClose"
		>
			<el-form :model="newsForm" :rules="rules" ref="newsFormRef" label-width="80px">
				<el-form-item label="标题" prop="title">
					<el-input v-model="newsForm.title" />
				</el-form-item>
				<el-form-item label="日期" prop="date">
					<el-date-picker v-model="newsForm.date" type="date" placeholder="选择日期" style="width: 100%;" />
				</el-form-item>
				<el-form-item label="内容" prop="content">
					<MdEditor v-model="newsForm.content" :showToolbarName="true" :theme="codeMode" :onUploadImg="onUploadImg" style="height: 500px" />
				</el-form-item>
				<el-form-item label="图片" prop="image">
					<el-upload
						class="image-upload"
						action="#"
						list-type="picture-card"
						:auto-upload="false"
						:on-change="handleImageChange"
						:on-remove="handleRemove"
						:limit="1"
						:file-list="fileList"
					>
						<i class="el-icon-plus"></i>
					</el-upload>
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
import { MdEditor } from 'md-editor-v3'
import 'md-editor-v3/lib/style.css'
import { upload } from '@/api/common.js'

export default {
	components: {
		MdEditor
	},
	data() {
		return {
			codeMode: 'light', // 编辑器主题
			newsItems: [
				// 示例数据
				{ 
					id: 1, 
					title: '王健林来我司洽谈合作', 
					date: '2025-04-05', 
					content: '<h3>合作交流，共创未来</h3><p>今日，万达集团董事长王健林来访我司，与公司高层进行了深入交流，探讨未来合作可能。双方就市场拓展、品牌推广等多个方面达成共识。</p><p>此次会谈为我司未来发展奠定了良好基础，期待双方合作成果丰硕。</p>',
					image: 'https://via.placeholder.com/400x300?text=News1' 
				},
				{ 
					id: 2, 
					title: '斯味特果业抗疫携手同行', 
					date: '2025-02-11', 
					content: '<h3>共克时艰，守望相助</h3><p>在全国抗击疫情的关键时刻，我公司积极响应号召，捐赠物资支援一线。公司全体员工团结一心，共同度过难关。</p><p>我们将继续发挥企业责任，为抗疫工作贡献力量。</p>',
					image: 'https://via.placeholder.com/400x300?text=News2' 
				},
				{ 
					id: 3, 
					title: '校外实践教学基地揭牌仪式', 
					date: '2024-12-17', 
					content: '<h3>产学结合，培养人才</h3><p>我公司与本地高校合作，建立校外实践教学基地，为学生提供实习机会。通过此次合作，学生们将有机会接触实际工作环境，提升专业技能。</p><p>公司将为学生提供全方位的指导和支持，帮助他们成长为行业优秀人才。</p>',
					image: 'https://via.placeholder.com/400x300?text=News3' 
				}
			],
			searchQuery: '',
			dialogVisible: false,
			isEdit: false,
			currentId: null,
			newsForm: {
				title: '',
				date: '',
				content: '',
				image: ''
			},
			fileList: [],
			rules: {
				title: [{ required: true, message: '请输入新闻标题', trigger: 'blur' }],
				date: [{ required: true, message: '请选择日期', trigger: 'change' }],
				content: [{ required: true, message: '请输入新闻内容', trigger: 'blur' }],
				image: [{ required: true, message: '请上传图片', trigger: 'change' }]
			}
		};
	},
	computed: {
		filteredNews() {
			return this.newsItems.filter(news =>
				news.title.toLowerCase().includes(this.searchQuery.toLowerCase())
			);
		}
	},
	methods: {
		// 截断HTML内容，保留一部分预览
		truncateHTML(html) {
			const div = document.createElement('div');
			div.innerHTML = html;
			const text = div.textContent || div.innerText || '';
			return text.length > 100 ? text.substring(0, 100) + '...' : text;
		},
		handleAddNews() {
			this.isEdit = false;
			this.resetForm();
			this.dialogVisible = true;
			this.fileList = [];
		},
		handleEditNews(news) {
			this.isEdit = true;
			this.currentId = news.id;
			// 复制新闻数据到表单
			this.newsForm = {
				title: news.title,
				date: news.date,
				content: news.content,
				image: news.image
			};
			// 设置文件列表以在上传组件中显示当前图片
			this.fileList = [
				{
					name: 'current-image.jpg',
					url: news.image
				}
			];
			this.dialogVisible = true;
		},
		handleDeleteConfirm(news) {
			this.$confirm('确定要删除该新闻吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			})
				.then(() => {
					this.deleteNews(news.id);
				})
				.catch(() => {
					this.$message({
						type: 'info',
						message: '已取消删除'
					});
				});
		},
		deleteNews(id) {
			const index = this.newsItems.findIndex(item => item.id === id);
			if (index > -1) {
				this.newsItems.splice(index, 1);
				this.$message({
					type: 'success',
					message: '删除成功'
				});
			}
		},
		resetForm() {
			this.newsForm = {
				title: '',
				date: '',
				content: '',
				image: ''
			};
			if (this.$refs.newsFormRef) {
				this.$refs.newsFormRef.resetFields();
			}
		},
		submitForm() {
			this.$refs.newsFormRef.validate(valid => {
				if (valid) {
					if (this.isEdit) {
						// 更新现有新闻
						const index = this.newsItems.findIndex(item => item.id === this.currentId);
						if (index > -1) {
							this.newsItems[index] = {
								...this.newsItems[index],
								title: this.newsForm.title,
								date: this.formatDate(this.newsForm.date),
								content: this.newsForm.content,
								image: this.newsForm.image
							};
							this.$message({
								type: 'success',
								message: '新闻更新成功'
							});
						}
					} else {
						// 添加新新闻
						const newId = this.getMaxId() + 1;
						this.newsItems.push({
							id: newId,
							title: this.newsForm.title,
							date: this.formatDate(this.newsForm.date),
							content: this.newsForm.content,
							image: this.newsForm.image
						});
						this.$message({
							type: 'success',
							message: '新闻添加成功'
						});
					}
					this.dialogVisible = false;
				} else {
					return false;
				}
			});
		},
		// 将图片上传到服务器，返回地址替换到md中
		async onUploadImg(files, callback) {
			try {
				const res = await Promise.all(
					Array.from(files).map((file) => {
						return new Promise((rev, rej) => {
							const form = new FormData()
							form.append('file', file)

							upload(form)
								.then((res) => rev(res))
								.catch((error) => rej(error))
						})
					})
				)

				callback(res.map((item) => item.data.url))
			} catch (error) {
				console.error('图片上传失败', error)
			}
		},
		getMaxId() {
			return Math.max(...this.newsItems.map(item => item.id), 0);
		},
		handleImageChange(file) {
			// 在实际应用中，这里应该是上传图片到服务器，然后获取URL
			// 这里我们只是模拟这个过程
			const reader = new FileReader();
			reader.readAsDataURL(file.raw);
			reader.onload = () => {
				this.newsForm.image = reader.result;
			};
		},
		handleRemove() {
			this.newsForm.image = '';
		},
		formatDate(date) {
			if (!date) return '';
			if (typeof date === 'string') return date;
			
			const d = new Date(date);
			const year = d.getFullYear();
			const month = String(d.getMonth() + 1).padStart(2, '0');
			const day = String(d.getDate()).padStart(2, '0');
			return `${year}-${month}-${day}`;
		},
		handleClose() {
			this.resetForm();
			this.dialogVisible = false;
		}
	}
}
</script>

<style>
.news-list {
	margin-top: 20px;
}
.header-operations {
	display: flex;
	align-items: center;
}
.news-content {
	margin-top: 15px;
}
.news-content-preview {
	margin: 10px 0;
	color: #666;
	display: -webkit-box;
	-webkit-line-clamp: 3;
	-webkit-box-orient: vertical;
	overflow: hidden;
	text-overflow: ellipsis;
	max-height: 80px;
}
.news-actions {
	margin-top: 15px;
	display: flex;
	justify-content: flex-end;
}
.image-upload .el-upload {
	width: 100%;
}
</style>
