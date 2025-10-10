<template>
  <div class="app-container">
    <el-card class="box-card">
      <!-- 搜索栏 -->
      <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="88px">
        <el-form-item label="供应商编号" prop="supplierNumber">
          <el-input
            v-model="queryParams.supplierNumber"
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
        <el-table-column label="供应商编号" align="center" prop="supplierNumber" />
        <el-table-column label="供应商名称" align="center" prop="supplierName" />
        <el-table-column label="联系人" align="center" prop="contactPerson" />
        <el-table-column label="联系方式" align="center" prop="contactMethod" />
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
          <el-input v-model="editForm.supplierNumber" placeholder="请输入供应商编号" />
        </el-form-item>
        <el-form-item label="供应商名称">
          <el-input v-model="editForm.supplierName" placeholder="请输入供应商名称" />
        </el-form-item>
        <el-form-item label="联系人">
          <el-input v-model="editForm.contactPerson" placeholder="请输入联系人" />
        </el-form-item>
        <el-form-item label="联系方式">
          <el-input v-model="editForm.contactMethod" placeholder="请输入联系方式" />
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
  supplierNumber: undefined,
  supplierName: undefined
})

// 表单参数
const form = ref({})

// 弹窗相关
const editDialogVisible = ref(false)
const editDialogTitle = ref('新增供应商')
const editForm = reactive({
  id: null,
  supplierNumber: '',
  supplierName: '',
  contactPerson: '',
  contactMethod: '',
  taxRate: '',
  wechat: ''
})

/** 查询供应商列表 */
function getList() {
  loading.value = true
  // 模拟数据，实际项目中应该调用API
  setTimeout(() => {
    supplierList.value = [
      {
        id: 1,
        supplierNumber: 'YWJK2',
        supplierName: '义乌健坤2',
        contactPerson: '陈',
        contactMethod: '19884933801',
        taxRate: '0%',
        wechat: '-'
      },
      {
        id: 2,
        supplierNumber: 'YWJK',
        supplierName: '义乌健坤',
        contactPerson: '陈',
        contactMethod: '19884933801',
        taxRate: '0%',
        wechat: '-'
      },
      {
        id: 3,
        supplierNumber: 'CQQG',
        supplierName: '重庆乔国皮革',
        contactPerson: '唐小姐',
        contactMethod: '13436030929',
        taxRate: '13%',
        wechat: '-'
      },
      {
        id: 4,
        supplierNumber: 'CZJC',
        supplierName: '常州骏驰',
        contactPerson: '施展宏',
        contactMethod: '13906127510',
        taxRate: '13%',
        wechat: '-'
      },
      {
        id: 5,
        supplierNumber: 'CZSD',
        supplierName: '常州三鼎织带',
        contactPerson: '喻静',
        contactMethod: '13913637618',
        taxRate: '0%',
        wechat: '-'
      },
      {
        id: 6,
        supplierNumber: 'CZTS',
        supplierName: '常州天晟切片',
        contactPerson: '朱泽成',
        contactMethod: '13861144133',
        taxRate: '13%',
        wechat: '-'
      },
      {
        id: 7,
        supplierNumber: 'CZYD',
        supplierName: '常州亿达化纤',
        contactPerson: '臧争光',
        contactMethod: '13327893258',
        taxRate: '13%',
        wechat: '0519-83761075'
      },
      {
        id: 8,
        supplierNumber: 'CZZG',
        supplierName: '常州筑高',
        contactPerson: '庄伟国',
        contactMethod: '13885335326',
        taxRate: '13%',
        wechat: '-'
      },
      {
        id: 9,
        supplierNumber: 'DGEZ',
        supplierName: '东莞恩泽皮革',
        contactPerson: '高海兰',
        contactMethod: '13829117724',
        taxRate: '0%',
        wechat: 'LMXHZ8866123456'
      },
      {
        id: 10,
        supplierNumber: 'DGJC',
        supplierName: '东莞晶彩',
        contactPerson: '王阳保',
        contactMethod: '15077431686',
        taxRate: '0%',
        wechat: '-'
      }
    ]
    total.value = 72
    loading.value = false
  }, 500)
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.pageNum = 1
  getList()
}

/** 重置按钮操作 */
function resetQuery() {
  queryParams.supplierNumber = undefined
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
function handleUpdate(row) {
  if (row && row.id) {
    // 编辑指定行
    router.push({ 
      name: '/SupplierProfileAdd', 
      query: { id: row.id, mode: 'edit' } 
    })
  } else {
    // 批量编辑选中的行
    if (ids.value.length === 1) {
      router.push({ 
        name: '/SupplierProfileAdd', 
        query: { id: ids.value[0], mode: 'edit' } 
      })
    } else {
      ElMessage.warning('请选择一条记录进行编辑')
    }
  }
}

/** 详情按钮操作 */
function handleDetail(row) {
  router.push({ 
    name: '/SupplierProfileAdd', 
    query: { id: row.id, mode: 'view' } 
  })
}

/** 删除按钮操作 */
function handleDelete(row) {
  const supplierIds = row.id || ids.value
  ElMessageBox.confirm('是否确认删除供应商编号为"' + supplierIds + '"的数据项？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(function() {
    ElMessage.success('删除成功')
    getList()
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
  editForm.supplierNumber = ''
  editForm.supplierName = ''
  editForm.contactPerson = ''
  editForm.contactMethod = ''
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