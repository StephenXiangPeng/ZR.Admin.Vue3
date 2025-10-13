<template>
  <div class="app-container">
    <el-card class="box-card">
      <!-- 搜索栏 -->
      <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="88px">
        <el-form-item label="供应商编号" prop="supplierCode">
          <el-input
            v-model="queryParams.supplierCode"
            placeholder="请输入供应商编号"
            clearable
            @keyup.enter="handleQuery"
          />
        </el-form-item>
        <el-form-item label="供应商名称" prop="supplierName">
          <el-input
            v-model="queryParams.supplierName"
            placeholder="请输入供应商名称"
            clearable
            @keyup.enter="handleQuery"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
          <el-button icon="Refresh" @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 操作按钮栏 -->
      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button
            type="primary"
            plain
            icon="Plus"
            @click="handleAdd"
          >新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="info"
            plain
            icon="Upload"
            @click="handleImport"
          >导入</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="warning"
            plain
            icon="Download"
            @click="handleExport"
          >导出</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="success"
            plain
            icon="Edit"
            :disabled="single"
            @click="handleUpdate"
          >更新</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button
            type="danger"
            plain
            icon="Delete"
            :disabled="multiple"
            @click="handleDelete"
          >删除</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <!-- 数据表格 -->
      <el-table v-loading="loading" :data="supplierList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="供应商编号" align="center" prop="supplierCode" />
        <el-table-column label="供应商名称" align="center" prop="supplierName" />
        <el-table-column label="联系人" align="center" prop="contactName" />
        <el-table-column label="联系方式" align="center" prop="contactPhone" />
        <el-table-column label="税率%" align="center" prop="taxRate" />
        <el-table-column label="微信" align="center" prop="wechat" />
        <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)">编辑</el-button>
            <el-button link type="info" icon="View" @click="handleDetail(scope.row)">详情</el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <pagination
        v-show="total > 0"
        :total="total"
        v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize"
        @pagination="getList"
      />
    </el-card>

    <!-- 编辑弹窗 -->
    <el-dialog v-model="editDialogVisible" :title="editDialogTitle" width="600px" :close-on-click-modal="false">
      <el-form :model="editForm" label-width="120px" style="max-width: 500px;">
        <el-form-item label="供应商编号">
          <el-input v-model="editForm.supplierCode" placeholder="请输入供应商编号" />
        </el-form-item>
        <el-form-item label="供应商名称">
          <el-input v-model="editForm.supplierName" placeholder="请输入供应商名称" />
        </el-form-item>
        <el-form-item label="联系人">
          <el-input v-model="editForm.contactName" placeholder="请输入联系人" />
        </el-form-item>
        <el-form-item label="联系方式">
          <el-input v-model="editForm.contactPhone" placeholder="请输入联系方式" />
        </el-form-item>
        <el-form-item label="税率">
          <el-input v-model="editForm.taxRate" placeholder="请输入税率" />
        </el-form-item>
        <el-form-item label="微信">
          <el-input v-model="editForm.wechat" placeholder="请输入微信" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleEditSubmit">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useRouter } from 'vue-router'
import { listSupplierInfo, delSupplierInfo, getSupplierDetail } from '@/api/ProcurementAndInventoryManagement/supplier'

const router = useRouter()

// 遮罩层
const loading = ref(true)
// 选中数组
const ids = ref([])
// 非单个禁用
const single = ref(true)
// 非多个禁用
const multiple = ref(true)
// 显示搜索条件
const showSearch = ref(true)
// 总条数
const total = ref(0)
// 供应商表格数据
const supplierList = ref([])
// 弹出层标题
const title = ref('')
// 是否显示弹出层
const open = ref(false)

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  supplierCode: undefined,
  supplierName: undefined
})

// 表单参数
const form = ref({})

// 弹窗相关
const editDialogVisible = ref(false)
const editDialogTitle = ref('新增供应商')
const editForm = reactive({
  id: null,
  supplierCode: '',
  supplierName: '',
  contactName: '',
  contactPhone: '',
  taxRate: '',
  wechat: ''
})

/** 查询供应商列表 */
function getList() {
  loading.value = true
  listSupplierInfo(queryParams).then(response => {
    const data = response.data || response
    supplierList.value = data.data || []
    total.value = data.total || 0
    loading.value = false
  }).catch(() => {
    loading.value = false
  })
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.pageNum = 1
  getList()
}

/** 重置按钮操作 */
function resetQuery() {
  queryParams.supplierCode = undefined
  queryParams.supplierName = undefined
  handleQuery()
}

// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map(item => item.id)
  single.value = selection.length !== 1
  multiple.value = !selection.length
}

/** 新增按钮操作 */
function handleAdd() {
  // 先尝试路由跳转
  try {
    router.push({ path: '/SupplierProfileAdd' })
    return
  } catch (error) {
    console.log('路由跳转失败，使用弹窗模式:', error)
  }
  
  // 如果路由跳转失败，使用弹窗模式
  editDialogTitle.value = '新增供应商'
  resetEditForm()
  editDialogVisible.value = true
}

/** 修改按钮操作 */
async function handleUpdate(row) {
  try {
    const supplierId = row?.id || (ids.value.length === 1 ? ids.value[0] : null)
    
    if (!supplierId) {
      ElMessage.warning('请选择一条记录进行编辑')
      return
    }
    
    if (ids.value.length > 1) {
      ElMessage.warning('请选择一条记录进行编辑')
      return
    }
    
    // 获取供应商详情数据
    loading.value = true
    console.log('开始获取供应商详情，ID:', supplierId)
    
    const response = await getSupplierDetail(supplierId)
    console.log('API响应:', response)
    
    const data = response.data || response
    console.log('解析后的数据:', data)
    
    // 检查是否有code字段，如果没有则直接使用数据
    if (data.code === 200 || (data.supplier && data.contacts && data.finances)) {
      console.log('获取详情成功，准备跳转')
      // 跳转到编辑页面，传递详情数据
      router.push({ 
        path: '/SupplierProfileAdd', 
        query: { 
          id: supplierId, 
          mode: 'edit',
          data: JSON.stringify(data) // 传递详情数据
        } 
      })
    } else {
      console.error('API返回错误:', data)
      ElMessage.error(data.msg || '获取供应商详情失败')
    }
  } catch (error) {
    console.error('获取供应商详情失败:', error)
    console.error('错误详情:', error.response || error.message)
    ElMessage.error('获取供应商详情失败，请重试')
  } finally {
    loading.value = false
  }
}

/** 详情按钮操作 */
async function handleDetail(row) {
  try {
    if (!row?.id) {
      ElMessage.warning('请选择一条记录查看详情')
      return
    }
    
    // 获取供应商详情数据
    loading.value = true
    console.log('开始获取供应商详情，ID:', row.id)
    
    const response = await getSupplierDetail(row.id)
    console.log('API响应:', response)
    
    const data = response.data || response
    console.log('解析后的数据:', data)
    
    // 检查是否有code字段，如果没有则直接使用数据
    if (data.code === 200 || (data.supplier && data.contacts && data.finances)) {
      console.log('获取详情成功，准备跳转')
      // 跳转到详情页面，传递详情数据
      router.push({ 
        path: '/SupplierProfileAdd', 
        query: { 
          id: row.id, 
          mode: 'view',
          data: JSON.stringify(data) // 传递详情数据
        } 
      })
    } else {
      console.error('API返回错误:', data)
      ElMessage.error(data.msg || '获取供应商详情失败')
    }
  } catch (error) {
    console.error('获取供应商详情失败:', error)
    console.error('错误详情:', error.response || error.message)
    ElMessage.error('获取供应商详情失败，请重试')
  } finally {
    loading.value = false
  }
}

/** 删除按钮操作 */
function handleDelete(row) {
  const supplierIds = row.id || ids.value
  const deleteIds = Array.isArray(supplierIds) ? supplierIds : [supplierIds]
  
  ElMessageBox.confirm(`是否确认删除选中的${deleteIds.length}个供应商？`, '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(function() {
    return delSupplierInfo(deleteIds)
  }).then((response) => {
    const data = response.data || response
    if (data.code === 200) {
      ElMessage.success('删除成功')
      getList()
    } else {
      ElMessage.error(data.msg || '删除失败')
    }
  }).catch((error) => {
    console.error('删除失败:', error)
    ElMessage.error('删除失败，请重试')
  })
}

/** 导入按钮操作 */
function handleImport() {
  ElMessage.info('导入功能')
}

/** 导出按钮操作 */
function handleExport() {
  ElMessage.info('导出功能')
}

// 弹窗相关方法
function resetEditForm() {
  editForm.id = null
  editForm.supplierCode = ''
  editForm.supplierName = ''
  editForm.contactName = ''
  editForm.contactPhone = ''
  editForm.taxRate = ''
  editForm.wechat = ''
}

function handleEditSubmit() {
  console.log('提交编辑表单:', editForm)
  ElMessage.success('保存成功')
  editDialogVisible.value = false
  getList() // 刷新列表
}

onMounted(() => {
  getList()
})

// 测试API调用
async function testApiCall() {
  try {
    console.log('测试API调用...')
    const testId = 1 // 使用测试ID
    const response = await getSupplierDetail(testId)
    console.log('测试API响应:', response)
  } catch (error) {
    console.error('测试API调用失败:', error)
  }
}

// 暴露测试函数到全局，方便调试
;(window as any).testSupplierApi = testApiCall
</script>

<style scoped>
.app-container {
  padding: 20px;
}

.box-card {
  margin-bottom: 20px;
}

.mb8 {
  margin-bottom: 8px;
}
</style>