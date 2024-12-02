<template>
  <div class="app-container">
    <el-card>
      <!-- 搜索区域 -->
      <el-form :inline="true" :model="queryParams" class="demo-form-inline">
        <el-form-item label="文件名称">
          <el-input
            v-model="queryParams.fileName"
            placeholder="请输入文件名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="文件分类">
          <el-select
            v-model="queryParams.category"
            placeholder="请选择分类"
            clearable
          >
            <el-option
              v-for="item in categoryOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="上传时间">
          <el-date-picker
            v-model="queryParams.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查询</el-button>
          <el-button @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 操作按钮区域 -->
      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" @click="handleUpload">
            上传文件
          </el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" @click="handleExport">
            导出
          </el-button>
        </el-col>
      </el-row>

      <!-- 文件列表 -->
      <el-table
        v-loading="loading"
        :data="fileList"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="文件名称" prop="fileName" :show-overflow-tooltip="true" />
        <el-table-column label="文件分类" prop="category" align="center">
          <template #default="scope">
            <dict-tag :options="categoryOptions" :value="scope.row.category" />
          </template>
        </el-table-column>
        <el-table-column label="文件大小" prop="fileSize" align="center">
          <template #default="scope">
            {{ formatFileSize(scope.row.fileSize) }}
          </template>
        </el-table-column>
        <el-table-column label="版本" prop="version" width="80" align="center" />
        <el-table-column label="上传时间" prop="createTime" width="180" align="center" />
        <el-table-column label="操作" align="center" width="200">
          <template #default="scope">
            <el-button link type="primary" @click="handleDownload(scope.row)">
              下载
            </el-button>
            <el-button link type="primary" @click="handlePreview(scope.row)">
              预览
            </el-button>
            <el-button link type="primary" @click="handleVersion(scope.row)">
              版本
            </el-button>
            <el-button link type="danger" @click="handleDelete(scope.row)">
              删除
            </el-button>
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

    <!-- 上传文件弹窗 -->
    <el-dialog
      v-model="uploadOpen"
      title="上传文件"
      width="500px"
      append-to-body
    >
      <file-upload @upload-success="handleUploadSuccess" />
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="uploadOpen = false">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { getFileList, deleteFile } from '@/api/file'
import FileUpload from '@/components/FileUpload'
import { ElMessage, ElMessageBox } from 'element-plus'

// 文件列表数据
const fileList = ref([])
const total = ref(0)
const loading = ref(false)
const uploadOpen = ref(false)

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  fileName: '',
  category: '',
  dateRange: []
})

// 获取文件列表
const getList = async () => {
  loading.value = true
  try {
    const response = await getFileList(queryParams)
    fileList.value = response.data.rows
    total.value = response.data.total
  } catch (error) {
    console.error('获取文件列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 文件大小格式化
const formatFileSize = (size) => {
  if (size < 1024) {
    return size + 'B'
  } else if (size < 1024 * 1024) {
    return (size / 1024).toFixed(2) + 'KB'
  } else {
    return (size / 1024 / 1024).toFixed(2) + 'MB'
  }
}

// 查询操作
const handleQuery = () => {
  queryParams.pageNum = 1
  getList()
}

// 重置操作
const resetQuery = () => {
  queryParams.fileName = ''
  queryParams.category = ''
  queryParams.dateRange = []
  handleQuery()
}

// 上传成功回调
const handleUploadSuccess = (fileInfo) => {
  uploadOpen.value = false
  getList()
  ElMessage.success('上传成功')
}

// 删除文件
const handleDelete = (row) => {
  ElMessageBox.confirm('确认删除该文件吗？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteFile(row.id)
      getList()
      ElMessage.success('删除成功')
    } catch (error) {
      console.error('删除文件失败:', error)
    }
  })
}

// 打开上传弹窗
const handleUpload = () => {
  uploadOpen.value = true
}

onMounted(() => {
  getList()
})
</script>