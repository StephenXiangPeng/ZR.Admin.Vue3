<template>
  <div class="material-management">
    <!-- 搜索筛选区域 -->
    <div class="search-section">
      <el-card class="search-card">
        <div v-show="!searchFormCollapsed" class="search-form">
          <el-row :gutter="20">
            <el-col :span="6">
              <el-input
                v-model="searchForm.materialNumber"
                placeholder="请输入物料编号"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-input
                v-model="searchForm.materialName"
                placeholder="请输入物料名称"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-input
                v-model="searchForm.specification"
                placeholder="请输入物料规格/型号"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-input
                v-model="searchForm.supplierName"
                placeholder="请输入供应商名称"
                clearable
              />
            </el-col>
          </el-row>
          <el-row :gutter="20" style="margin-top: 15px;">
            <el-col :span="6">
              <el-input
                v-model="searchForm.category"
                placeholder="请输入物料分类"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-input
                v-model="searchForm.remarks"
                placeholder="请输入备注"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-input
                v-model="searchForm.barcode"
                placeholder="请输入物料条形码"
                clearable
              />
            </el-col>
            <el-col :span="6">
              <el-select
                v-model="searchForm.serialStatus"
                placeholder="请选择序列号状态"
                clearable
                style="width: 100%"
              >
                <el-option label="启用" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-col>
          </el-row>
          <el-row :gutter="20" style="margin-top: 15px;">
            <el-col :span="6">
              <el-select
                v-model="searchForm.batchStatus"
                placeholder="请选择批次状态"
                clearable
                style="width: 100%"
              >
                <el-option label="启用" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-col>
            <el-col :span="6">
              <el-select
                v-model="searchForm.usageStatus"
                placeholder="请选择使用状态"
                clearable
                style="width: 100%"
              >
                <el-option label="正常" value="1" />
                <el-option label="停用" value="0" />
              </el-select>
            </el-col>
            <el-col :span="6">
              <el-select
                v-model="searchForm.multiUnitStatus"
                placeholder="请选择多单位启用状态"
                clearable
                style="width: 100%"
              >
                <el-option label="启用" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-col>
            <el-col :span="6">
              <div class="search-buttons">
                <el-button type="danger" @click="handleSearch">搜索</el-button>
                <el-button @click="handleReset">重置</el-button>
              </div>
            </el-col>
          </el-row>
        </div>
        <div class="collapse-link" @click="toggleSearchForm">
          <span>{{ searchFormCollapsed ? '展开' : '收起' }}</span>
          <el-icon>
            <ArrowUp v-if="!searchFormCollapsed" />
            <ArrowDown v-else />
          </el-icon>
        </div>
      </el-card>
    </div>
    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 顶部操作栏 -->
      <div class="action-bar">
        <!-- 左侧操作栏 - 与分类区域对齐 -->
        <div class="action-left-section">
          <div class="action-left">
            <el-input
              v-model="keyword"
              placeholder="输入关键字"
              style="width: 125px; margin-right: 2px;"
            />
            <el-button type="danger" @click="handleAddCategory">新增分类</el-button>
          </div>
        </div>
        
        <!-- 右侧操作栏 - 与物料列表卡片对齐 -->
        <div class="action-right-section">
          <div class="action-buttons">
            <el-button type="danger" @click="handleAddMaterial">新增物料</el-button>
            <el-button @click="handleImport">导入</el-button>
            <el-button @click="handleExport">导出</el-button>
            <el-button @click="handleDelete">删除</el-button>
            <el-button @click="handleBatchModify">批量修改</el-button>
            <el-button @click="handleBatchPrintQR">批量打印二维码</el-button>
            <el-button @click="handleBatchPrintBarcode">批量打印条形码</el-button>
            <el-button @click="handleUpdate">更新</el-button>
            <el-button @click="handleImportCategory">导入分类</el-button>
            <el-button @click="handleImportSubCategory">导入子分类</el-button>
          </div>
          <div class="action-right">
            <el-button @click="handleListFields">
              <el-icon><List /></el-icon>
              列表字段
            </el-button>
          </div>
        </div>
      </div>

      <!-- 主体内容区域 -->
      <div class="content-body">
        <!-- 左侧分类区域 -->
        <div class="category-section">
          <el-card class="category-card">
            <div class="category-header">
              <h3>分类名称</h3>
            </div>
            <div class="category-list">
              <div
                v-for="category in categories"
                :key="category.id"
                class="category-item"
                :class="{ active: selectedCategory === category.id }"
                @click="selectCategory(category.id)"
              >
                {{ category.name }}
              </div>
            </div>
          </el-card>
        </div>

        <!-- 右侧物料区域 -->
        <div class="material-section">
          <el-card class="material-card">
            <div class="material-content">
              <el-table
                :data="materialList"
                style="width: 100%"
                @selection-change="handleSelectionChange"
              >
                <el-table-column type="selection" width="55" />
                <el-table-column prop="image" label="物料图片" width="100">
                  <template #default="scope">
                    <el-image
                      v-if="scope.row.image"
                      :src="scope.row.image"
                      style="width: 50px; height: 50px;"
                      fit="cover"
                    />
                    <span v-else>-</span>
                  </template>
                </el-table-column>
                <el-table-column prop="name" label="物料名称" />
                <el-table-column prop="code" label="物料编码" sortable>
                  <template #header>
                    <span>物料编码</span>
                    <el-icon class="sort-icon"><ArrowUp /></el-icon>
                  </template>
                </el-table-column>
                <el-table-column prop="salesPrice" label="销售单价(含税)-..." />
                <el-table-column prop="lowestPrice" label="最低销售单价(含." />
                <el-table-column label="操作" width="300">
                  <template #default="scope">
                    <el-button type="primary" link @click="handleCopy(scope.row)">复制</el-button>
                    <el-button type="primary" link @click="handleEdit(scope.row)">编辑</el-button>
                    <el-button type="primary" link @click="handleDetail(scope.row)">详情</el-button>
                    <el-button type="primary" link @click="handleDeleteItem(scope.row)">删除</el-button>
                    <el-button type="primary" link @click="handlePrintQR(scope.row)">打印二维码</el-button>
                    <el-button type="primary" link @click="handleDownloadSerial(scope.row)">下载序列号</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>
            <!-- 分页组件在物料卡片内 -->
            <div class="pagination">
              <el-pagination
                v-model:current-page="currentPage"
                v-model:page-size="pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="handleSizeChange"
                @current-change="handleCurrentChange"
              />
            </div>
          </el-card>
        </div>
      </div>
    </div>

    <!-- 新增分类对话框 -->
    <el-dialog
      v-model="addCategoryDialogVisible"
      title="新增分类"
      width="500px"
      :before-close="handleCloseAddCategoryDialog"
    >
      <el-form
        ref="addCategoryFormRef"
        :model="addCategoryForm"
        :rules="addCategoryRules"
        label-width="100px"
      >
        <el-form-item label="分类编号" prop="categoryNumber" required>
          <el-input
            v-model="addCategoryForm.categoryNumber"
            placeholder="使用系统编号"
            readonly
          >
            <template #suffix>
              <el-icon class="auto-generate-icon">
                <Check />
              </el-icon>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="分类名称" prop="categoryName" required>
          <el-input
            v-model="addCategoryForm.categoryName"
            placeholder="请输入分类名称"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="handleCloseAddCategoryDialog">取消</el-button>
          <el-button type="danger" @click="handleSubmitAddCategory">提交</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowUp, ArrowDown, List, Check } from '@element-plus/icons-vue'

// 路由
const router = useRouter()

// 搜索表单
const searchForm = reactive({
  materialNumber: '',
  materialName: '',
  specification: '',
  supplierName: '',
  category: '',
  remarks: '',
  barcode: '',
  serialStatus: '',
  batchStatus: '',
  usageStatus: '',
  multiUnitStatus: ''
})

// 搜索表单折叠状态
const searchFormCollapsed = ref(false)

// 关键字搜索
const keyword = ref('')

// 分类数据
const categories = ref([
  { id: 1, name: '尾盖' },
  { id: 2, name: '尾盖加工' },
  { id: 3, name: '项圈' },
  { id: 4, name: '项圈加工' },
  { id: 5, name: '手柄' },
  { id: 6, name: '裹条' },
  { id: 7, name: '皮革' },
  { id: 8, name: '皮革加工' },
  { id: 9, name: '鞭杆' },
  { id: 10, name: '鞭杆加工' },
  { id: 11, name: '鞭头' }
])

// 选中的分类
const selectedCategory = ref(1)

// 物料列表数据
const materialList = ref([
  {
    id: 1,
    name: '230TPR圈115078黑色',
    code: 'XQ0009',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 2,
    name: '鞭头916玻纤真皮黑色',
    code: 'BTJG0001',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 3,
    name: '牛头层棕色1.8mm',
    code: 'PG0017',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 4,
    name: '牛头层黑色1.8mm',
    code: 'PG0016',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 5,
    name: '热熔胶',
    code: 'FL0016',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 6,
    name: '101极速胶',
    code: 'FL0015',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 7,
    name: 'PET特制双面胶带(固化)',
    code: 'FL0014',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 8,
    name: '玻纤杆φ7.8-0500/200/3.5...',
    code: 'BG0236',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 9,
    name: '压变革黄棕P1-1049#',
    code: 'PG0015',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  },
  {
    id: 10,
    name: '压变革深棕P4-1054#',
    code: 'PG0014',
    image: '',
    salesPrice: '',
    lowestPrice: ''
  }
])

// 选中的物料
const selectedMaterials = ref([])

// 分页数据
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(21)

// 新增分类对话框
const addCategoryDialogVisible = ref(false)
const addCategoryFormRef = ref()

// 新增分类表单
const addCategoryForm = reactive({
  categoryNumber: '',
  categoryName: ''
})

// 新增分类表单验证规则
const addCategoryRules = {
  categoryName: [
    { required: true, message: '请输入分类名称', trigger: 'blur' }
  ]
}

// 搜索表单折叠切换
const toggleSearchForm = () => {
  searchFormCollapsed.value = !searchFormCollapsed.value
}

// 搜索
const handleSearch = () => {
  ElMessage.success('搜索功能待实现')
}

// 重置
const handleReset = () => {
  Object.keys(searchForm).forEach(key => {
    searchForm[key] = ''
  })
}

// 选择分类
const selectCategory = (categoryId: number) => {
  selectedCategory.value = categoryId
}

// 表格选择变化
const handleSelectionChange = (selection: any[]) => {
  selectedMaterials.value = selection
}

// 操作按钮处理函数
const handleAddCategory = () => {
  // 生成系统编号
  const newCategoryNumber = `FL${String(categories.value.length + 1).padStart(4, '0')}`
  addCategoryForm.categoryNumber = newCategoryNumber
  addCategoryForm.categoryName = ''
  addCategoryDialogVisible.value = true
}

const handleAddMaterial = () => {
  router.push('/materialAdd')
}

const handleImport = () => {
  ElMessage.info('导入功能待实现')
}

const handleExport = () => {
  ElMessage.info('导出功能待实现')
}

const handleDelete = () => {
  if (selectedMaterials.value.length === 0) {
    ElMessage.warning('请选择要删除的物料')
    return
  }
  ElMessageBox.confirm('确定要删除选中的物料吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    ElMessage.success('删除成功')
  })
}

const handleBatchModify = () => {
  ElMessage.info('批量修改功能待实现')
}

const handleBatchPrintQR = () => {
  ElMessage.info('批量打印二维码功能待实现')
}

const handleBatchPrintBarcode = () => {
  ElMessage.info('批量打印条形码功能待实现')
}

const handleUpdate = () => {
  ElMessage.info('更新功能待实现')
}

const handleImportCategory = () => {
  ElMessage.info('导入分类功能待实现')
}

const handleImportSubCategory = () => {
  ElMessage.info('导入子分类功能待实现')
}

const handleListFields = () => {
  ElMessage.info('列表字段功能待实现')
}

// 物料操作
const handleCopy = (row: any) => {
  ElMessage.info(`复制物料: ${row.name}`)
}

const handleEdit = (row: any) => {
  ElMessage.info(`编辑物料: ${row.name}`)
}

const handleDetail = (row: any) => {
  ElMessage.info(`查看详情: ${row.name}`)
}

const handleDeleteItem = (row: any) => {
  ElMessageBox.confirm(`确定要删除物料 ${row.name} 吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    ElMessage.success('删除成功')
  })
}

const handlePrintQR = (row: any) => {
  ElMessage.info(`打印二维码: ${row.name}`)
}

const handleDownloadSerial = (row: any) => {
  ElMessage.info(`下载序列号: ${row.name}`)
}

// 分页处理
const handleSizeChange = (val: number) => {
  pageSize.value = val
  ElMessage.info(`每页显示 ${val} 条`)
}

const handleCurrentChange = (val: number) => {
  currentPage.value = val
  ElMessage.info(`跳转到第 ${val} 页`)
}

// 新增分类对话框处理函数
const handleCloseAddCategoryDialog = () => {
  addCategoryDialogVisible.value = false
  addCategoryFormRef.value?.resetFields()
}

const handleSubmitAddCategory = async () => {
  if (!addCategoryFormRef.value) return
  
  try {
    await addCategoryFormRef.value.validate()
    
    // 添加新分类到列表
    const newCategory = {
      id: categories.value.length + 1,
      name: addCategoryForm.categoryName
    }
    categories.value.push(newCategory)
    
    ElMessage.success('分类添加成功')
    addCategoryDialogVisible.value = false
    addCategoryFormRef.value.resetFields()
  } catch (error) {
    console.log('表单验证失败', error)
  }
}

onMounted(() => {
  // 初始化数据
})
</script>

<style scoped>
.material-management {
  padding: 20px;
  background-color: #f5f5f5;
  min-height: 100vh;
}

/* 搜索区域样式 */
.search-section {
  margin-bottom: 20px;
}

.search-card {
  background-color: #f9f9f9;
}

.search-form {
  padding: 20px;
}

.search-buttons {
  display: flex;
  gap: 10px;
}

.collapse-link {
  text-align: center;
  padding: 10px;
  cursor: pointer;
  color: #666;
  border-top: 1px solid #eee;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.collapse-link:hover {
  color: #333;
}

/* 主要内容区域 */
.main-content {
  background: white;
  border-radius: 4px;
  overflow: hidden;
}

/* 操作栏样式 */
.action-bar {
  display: flex;
  align-items: center;
  padding: 15px 20px;
  border-bottom: 1px solid #eee;
  background: #fafafa;
  gap: 20px;
}

/* 左侧操作栏 - 与分类区域对齐 */
.action-left-section {
  width: 250px;
  flex-shrink: 0;
}

.action-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* 右侧操作栏 - 与物料列表卡片对齐 */
.action-right-section {
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.action-buttons {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.action-right {
  display: flex;
  align-items: center;
}

/* 内容主体 */
.content-body {
  display: flex;
  gap: 20px;
  min-height: 600px;
}

/* 左侧分类区域 */
.category-section {
  width: 250px;
  flex-shrink: 0;
}

.category-card {
  height: 100%;
  background: #fafafa;
}

.category-header {
  padding: 15px;
  border-bottom: 1px solid #eee;
  background: #f0f0f0;
}

.category-header h3 {
  margin: 0;
  font-size: 14px;
  color: #333;
  font-weight: 600;
}


.category-list {
  max-height: 500px;
  overflow-y: auto;
  padding: 10px 0;
}

.category-item {
  padding: 12px 20px;
  cursor: pointer;
  border-bottom: 1px solid #f0f0f0;
  transition: all 0.3s;
  font-size: 14px;
}

.category-item:hover {
  background-color: #e6f7ff;
}

.category-item.active {
  background-color: #1890ff;
  color: white;
}

/* 右侧物料区域 */
.material-section {
  flex: 1;
  min-width: 0;
}

.material-card {
  height: 100%;
  background: white;
}

.material-content {
  padding: 0;
}

.sort-icon {
  margin-left: 5px;
  color: #1890ff;
}

/* 分页样式 */
.pagination {
  padding: 15px 20px;
  text-align: center;
  border-top: 1px solid #eee;
  background: #fafafa;
}

/* 新增分类对话框样式 */
.auto-generate-icon {
  color: #f56c6c;
  font-size: 16px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .action-bar {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .action-left-section {
    width: 100%;
  }
  
  .action-right-section {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .action-buttons {
    flex-direction: column;
    gap: 5px;
  }
  
  .content-body {
    flex-direction: column;
    gap: 15px;
  }
  
  .category-section {
    width: 100%;
  }
  
  .category-list {
    max-height: 200px;
  }
}
</style>