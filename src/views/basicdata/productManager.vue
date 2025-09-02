<template>
  <div class="product-manager">
    <!-- 搜索/筛选区域 -->
    <el-card class="search-card" shadow="never">
      <div class="search-form" :class="{ collapsed: isCollapsed }">
        <!-- 收起状态：一行6个输入框，第7个位置放搜索重置按钮 -->
        <div v-if="isCollapsed">
          <el-row :gutter="20">
            <el-col :span="4">
              <el-input v-model="searchForm.productNumber" placeholder="请输入产品编号" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.productName" placeholder="请输入产品名称" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.category" placeholder="请输入产品分类" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.specification" placeholder="请输入规格" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.attribute" placeholder="请输入产品属性" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.remark" placeholder="请输入备注" clearable />
            </el-col>
            <el-col :span="4">
              <div class="button-group">
                <el-button type="primary" @click="handleSearch" class="search-btn">搜索</el-button>
                <el-button @click="handleReset" class="reset-btn">重置</el-button>
              </div>
            </el-col>
          </el-row>
        </div>

        <!-- 展开状态：一行7个输入框，搜索重置按钮在第7个位置 -->
        <div v-if="!isCollapsed">
          <el-row :gutter="20">
            <el-col :span="4">
              <el-input v-model="searchForm.productNumber" placeholder="请输入产品编号" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.productName" placeholder="请输入产品名称" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.category" placeholder="请输入产品分类" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.specification" placeholder="请输入规格" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.attribute" placeholder="请输入产品属性" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.remark" placeholder="请输入备注" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.updater" placeholder="请输入更新人" clearable />
            </el-col>
            <el-col :span="4">
              <el-input v-model="searchForm.barcode" placeholder="请输入产品条形码" clearable />
            </el-col>
            <el-col :span="4">
              <div class="button-group">
                <el-button type="primary" @click="handleSearch" class="search-btn">搜索</el-button>
                <el-button @click="handleReset" class="reset-btn">重置</el-button>
              </div>
            </el-col>
          </el-row>
        </div>

        <!-- 展开收起按钮 - 另起一行 -->
        <div class="collapse-actions">
          <el-button type="text" @click="toggleCollapse">
            {{ isCollapsed ? '展开' : '收起' }}
            <el-icon><ArrowUp v-if="!isCollapsed" /><ArrowDown v-else /></el-icon>
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 主要内容区域 -->
    <el-card class="main-card" shadow="never">
      <!-- 顶部操作按钮区域 -->
      <div class="top-actions">
        <div class="category-actions">
          <el-input
            v-model="keyword"
            placeholder="输入关键字"
            style="width: 150px; margin-right: 8px"
            clearable size="large"
          />
          <el-button type="primary" @click="handleAddCategory">新增分类</el-button>
        </div>
        <div class="product-actions">
          <el-button type="primary" @click="handleAddProduct">新增产品</el-button>
        </div>
      </div>

      <!-- 分类和产品列表区域 -->
      <div class="content-area">
        <!-- 左侧分类列表 -->
        <div class="category-panel" :class="{ collapsed: categoryCollapsed }">
          <div class="category-header">
            <span v-if="!categoryCollapsed">分类名称</span>
            <el-button type="text" @click="toggleCategory" class="collapse-btn">
              <el-icon><ArrowLeft v-if="!categoryCollapsed" /><ArrowRight v-else /></el-icon>
            </el-button>
          </div>
          <div class="category-list" v-if="!categoryCollapsed">
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
        </div>

        <!-- 分割线 -->
        <div class="divider"></div>

        <!-- 右侧产品表格 -->
        <div class="product-panel">
          <el-table
            :data="productList"
            @selection-change="handleSelectionChange"
            style="width: 100%"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column prop="productNumber" label="产品编号" sortable>
              <template #header>
                <span>产品编号</span>
                <el-icon><ArrowUp /><ArrowDown /></el-icon>
              </template>
            </el-table-column>
            <el-table-column prop="productImage" label="产品图" width="80">
              <template #default="scope">
                <el-image
                  :src="scope.row.productImage"
                  style="width: 50px; height: 50px"
                  fit="cover"
                >
                  <template #error>
                    <div class="image-placeholder">
                      <el-icon><Picture /></el-icon>
                    </div>
                  </template>
                </el-image>
              </template>
            </el-table-column>
            <el-table-column prop="productName" label="产品名称" />
            <el-table-column prop="specification" label="产品规格" />
            <el-table-column prop="inventoryUnit" label="库存单位" />
            <el-table-column prop="subUnit" label="副单位" />
            <el-table-column prop="barcode" label="条形码" />
            <el-table-column label="操作" width="400">
              <template #default="scope">
                <el-button type="text" @click="handleEdit(scope.row)">编辑</el-button>
                <el-button type="text" @click="handleCopy(scope.row)">复制</el-button>
                <el-button type="text" @click="handleDetail(scope.row)">详情</el-button>
                <el-button type="text" @click="handleDeleteItem(scope.row)">删除</el-button>
                <el-button type="text" @click="handlePrintQR(scope.row)">打印二维码</el-button>
                <el-button type="text" @click="handleDownloadSerial(scope.row)">下载序列号</el-button>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分页 -->
          <div class="pagination-wrapper">
            <el-pagination
              v-model:current-page="pagination.currentPage"
              v-model:page-size="pagination.pageSize"
              :page-sizes="[10, 20, 50, 100]"
              :total="pagination.total"
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="handleSizeChange"
              @current-change="handleCurrentChange"
            />
          </div>
        </div>
      </div>
    </el-card>
	</div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { ArrowUp, ArrowDown, ArrowLeft, ArrowRight, Picture } from '@element-plus/icons-vue'

// 搜索表单数据
const searchForm = reactive({
  productNumber: '',
  productName: '',
  category: '',
  specification: '',
  attribute: '',
  remark: '',
  updater: '',
  barcode: ''
})

// 页面状态
const isCollapsed = ref(false)
const categoryCollapsed = ref(false)
const keyword = ref('')
const selectedCategory = ref('')
const selectedProducts = ref([])

// 分类数据
const categories = ref([
  { id: '1', name: '马鞭成品' },
  { id: '2', name: '马鞭配件' },
  { id: '3', name: '0623' }
])

// 产品数据
const productList = ref([
  {
    id: '1',
    productNumber: 'MBCP0001',
    productImage: '',
    productName: 'BC20103691...',
    specification: '杆长50cm',
    inventoryUnit: '支',
    subUnit: '-',
    barcode: '-'
  },
  {
    id: '2',
    productNumber: 'MBCP0002',
    productImage: '',
    productName: 'BC20103691...',
    specification: '杆长50cm',
    inventoryUnit: '支',
    subUnit: '-',
    barcode: '-'
  },
  {
    id: '3',
    productNumber: 'MBCP0003',
    productImage: '',
    productName: 'BC20103691...',
    specification: '杆长60cm',
    inventoryUnit: '支',
    subUnit: '-',
    barcode: '-'
  },
  {
    id: '4',
    productNumber: 'MBCP0004',
    productImage: '',
    productName: 'BC20103691...',
    specification: '杆长60cm',
    inventoryUnit: '支',
    subUnit: '-',
    barcode: '-'
  }
])

// 分页数据
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 4
})

// 方法定义
const handleSearch = () => {
  console.log('搜索', searchForm)
}

const handleReset = () => {
  Object.keys(searchForm).forEach(key => {
    searchForm[key] = ''
  })
}

const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value
}

const toggleCategory = () => {
  categoryCollapsed.value = !categoryCollapsed.value
}

const selectCategory = (categoryId: string) => {
  selectedCategory.value = categoryId
}

const handleSelectionChange = (selection: any[]) => {
  selectedProducts.value = selection
}

const handleSizeChange = (size: number) => {
  pagination.pageSize = size
}

const handleCurrentChange = (page: number) => {
  pagination.currentPage = page
}

// 操作按钮方法
const handleAddCategory = () => console.log('新增分类')
const handleAddProduct = () => console.log('新增产品')

// 表格操作方法
const handleEdit = (row: any) => console.log('编辑', row)
const handleCopy = (row: any) => console.log('复制', row)
const handleDetail = (row: any) => console.log('详情', row)
const handleDeleteItem = (row: any) => console.log('删除', row)
const handlePrintQR = (row: any) => console.log('打印二维码', row)
const handleDownloadSerial = (row: any) => console.log('下载序列号', row)
</script>

<style scoped>
.product-manager {
  padding: 20px;
  background-color: #f5f5f5;
  min-height: 100vh;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  padding: 5px 0;
  transition: all 0.3s ease;
}

.search-form.collapsed {
  padding: 3px 0;
}

.search-form .el-row {
  margin-bottom: 5px;
}

.search-form .el-col {
  margin-bottom: 3px;
}

.search-form .el-input,
.search-form .el-select {
  height: 28px;
}

.search-form .el-input__wrapper {
  height: 28px;
  line-height: 28px;
}

.search-form .el-select .el-input__wrapper {
  height: 28px;
}

.search-form .el-select .el-input__inner {
  height: 28px;
  line-height: 28px;
}

.search-form .el-select .el-input {
  height: 28px;
}



.search-form .search-btn,
.search-form .reset-btn {
  height: 28px;
}

.button-group {
  display: flex;
  gap: 8px;
}

.button-group .el-button {
  flex: 1;
}

.collapse-actions {
  text-align: left;
  margin-top: 5px;
  margin-bottom: 5px;
}

.collapse-actions .el-button {
  height: 28px;
  padding: 6px 12px;
}





.main-card {
  background-color: #fff;
}



.content-area {
  display: flex;
  gap: 0;
  min-height: 600px;
}

.divider {
  width: 1px;
  background-color: #e4e7ed;
  margin: 0 20px;
  flex-shrink: 0;
}

.category-panel {
  width: 250px;
  background-color: #f8f9fa;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  transition: width 0.3s ease;
  flex-shrink: 0;
}

.category-panel.collapsed {
  width: 40px !important;
}

.category-header {
  padding: 15px;
  border-bottom: 1px solid #e4e7ed;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
  background-color: #f5f7fa;
}

.category-panel.collapsed .category-header {
  padding: 10px 5px;
  justify-content: center;
}

.collapse-btn {
  padding: 0;
  font-size: 12px;
}

.category-list {
  padding: 10px 0;
}

.category-item {
  padding: 10px 15px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.category-item:hover {
  background-color: #e6f7ff;
}

.category-item.active {
  background-color: #1890ff;
  color: white;
}

.top-actions {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  gap: 20px;
  padding: 15px 0;
  border-bottom: 1px solid #ebeef5;
  margin-bottom: 20px;
}

.category-actions {
  display: flex;
  align-items: center;
  max-width: 250px;
}

.product-actions {
  display: flex;
  align-items: center;
}

.product-panel {
  flex: 1;
  background-color: #fff;
}

.image-placeholder {
  width: 50px;
  height: 50px;
  background-color: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #999;
}

.pagination-wrapper {
  margin-top: 20px;
  text-align: right;
}

/* 响应式设计 */
@media (max-width: 1200px) {
  .content-area {
    flex-direction: column;
  }
  
  .category-panel {
    width: 250px !important;
    margin-bottom: 20px;
  }
  
  .category-panel.collapsed {
    width: 40px !important;
  }
}

@media (max-width: 768px) {
  .toolbar {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
  
  .left-tools {
    flex-direction: column;
    align-items: flex-start;
    width: 100%;
  }
  
  .right-tools {
    align-self: flex-end;
  }
}
</style>