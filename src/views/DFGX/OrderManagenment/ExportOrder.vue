<template>
  <div class="order-view-container">
    <!-- 搜索和筛选 -->
    <el-card class="search-card" shadow="hover">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-input
            v-model="searchForm.orderNo"
            placeholder="Order No."
            clearable
            @clear="handleSearch">
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-input
            v-model="searchForm.customerName"
            placeholder="Customer Name"
            clearable
            @clear="handleSearch">
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-select v-model="searchForm.status" placeholder="Order Status" clearable @change="handleSearch">
            <el-option label="All" value=""></el-option>
            <el-option label="Pending" value="0"></el-option>
            <el-option label="In Production" value="1"></el-option>
            <el-option label="Completed" value="2"></el-option>
            <el-option label="Cancelled" value="3"></el-option>
          </el-select>
        </el-col>
      </el-row>

      <el-row :gutter="20" style="margin-top: 15px;">
        <el-col :span="6">
          <el-date-picker
            v-model="searchForm.beginTime"
            type="datetime"
            placeholder="Start Date"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            @change="handleSearch">
          </el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="searchForm.endTime"
            type="datetime"
            placeholder="End Date"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            @change="handleSearch">
          </el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-input-number
            v-model="searchForm.minAmount"
            placeholder="Min Amount"
            :min="0"
            :precision="2"
            @change="handleSearch">
          </el-input-number>
        </el-col>
        <el-col :span="6">
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            Search
          </el-button>
          <el-button @click="resetSearch">
            <el-icon><Refresh /></el-icon>
            Reset
          </el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- 订单列表 -->
    <el-card class="order-list-card" shadow="hover">
      <template #header>
        <div class="card-header">
          <span>Order List</span>
          <el-button type="primary" :disabled="!selectedRows.length" @click="exportSelected">
            <el-icon><Document /></el-icon>
            导出订单
          </el-button>
        </div>
      </template>

      <el-table
        :data="orderList"
        v-loading="loading"
        stripe
        @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" fixed="left" />

        <el-table-column prop="orderNo" label="Order No." width="150" fixed="left">
          <template #default="{ row }">
            <el-link type="primary" @click="viewOrderDetail(row)">
              {{ row.orderNo }}
            </el-link>
          </template>
        </el-table-column>

        <el-table-column prop="customerName" label="Customer Name" width="150">
        </el-table-column>

        <el-table-column prop="orderDate" label="Order Date" width="120">
          <template #default="{ row }">
            {{ formatDate(row.order_date) }}
          </template>
        </el-table-column>

        <el-table-column prop="status" label="Status" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusName(row.status) }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column prop="remarks" label="Remarks" min-width="200" show-overflow-tooltip>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.currentPage"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange">
        </el-pagination>
      </div>
    </el-card>

    <!-- 订单详情对话框 -->
    <el-dialog
      v-model="detailDialogVisible"
      title="Order Details"
      width="800px"
      :before-close="handleCloseDetail">
      <div v-if="selectedOrder" class="order-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="Order No.">{{ selectedOrder.orderNo }}</el-descriptions-item>
          <el-descriptions-item label="Customer Name">{{ selectedOrder.customerName }}</el-descriptions-item>
          <el-descriptions-item label="Order Date">{{ formatDate(selectedOrder.orderDate) }}</el-descriptions-item>
          <el-descriptions-item label="Order Status">
            <el-tag :type="getStatusType(selectedOrder.status)">
              {{ getStatusName(selectedOrder.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="Remarks">{{ selectedOrder.remarks || 'None' }}</el-descriptions-item>
          <el-descriptions-item label="Auto Remarks" v-if="selectedOrder.autoRemarks">
            <el-tag type="success" size="small">{{ selectedOrder.autoRemarks }}</el-tag>
          </el-descriptions-item>
        </el-descriptions>

        <!-- Order Status Progress (仅显示 1,5,6) -->
        <div class="order-status-section">
          <h4>Order Status Progress</h4>
          <el-steps :active="getFilteredOrderStatusStep(selectedOrder.orderStatus)" finish-status="success" align-center>
            <el-step 
              v-for="(status, index) in filteredOrderStatusOptions" 
              :key="index"
              :title="status.label"
              :description="getFilteredStatusDescription(index, selectedOrder.orderStatus)">
            </el-step>
          </el-steps>
        </div>

        <!-- 订单详情列表 -->
        <div v-if="orderDetails.length > 0" class="order-details-section">
          <h4>Order Details</h4>
          <el-table :data="orderDetails" size="small" border>
            <el-table-column prop="dia" label="Dia" width="80"></el-table-column>
            <el-table-column prop="refractiveIndexValue" label="Index" width="100"></el-table-column>
            <el-table-column prop="designNameText" label="Design" width="120"></el-table-column>
            <el-table-column prop="materialName" label="Material" width="120"></el-table-column>
            <el-table-column prop="channelName" label="Corridor" width="100"></el-table-column>
            <el-table-column prop="coatingName" label="Coating" width="120"></el-table-column>
            <el-table-column prop="tintingName" label="Tinting" width="120"></el-table-column>
            <el-table-column prop="lenticularName" label="Lenticular" width="120"></el-table-column>
            <el-table-column prop="frameType" label="Frame Type" width="100"></el-table-column>
            <el-table-column label="Quantity" width="120" align="center">
              <template #default="{ row }">
                <div class="quantity-info">
                  <div>Left: {{ row.leftEyeQuantity || 1 }}</div>
                  <div>Right: {{ row.rightEyeQuantity || 1 }}</div>
                  <div>Total: {{ row.quantity || (row.leftEyeQuantity || 1) + (row.rightEyeQuantity || 1) }}</div>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 文件附件（下载） -->
        <div v-if="selectedOrder.colorAttachment || selectedOrder.remarksAttachment" class="file-attachments-section">
          <h4>File Attachments</h4>
          <el-descriptions :column="1" border>
            <el-descriptions-item label="Color Attachment" v-if="selectedOrder.colorAttachment">
              <div class="file-list">
                <div
                  v-for="(file, index) in getFileList(selectedOrder.colorAttachment)"
                  :key="'color-'+index"
                  class="file-item">
                  <el-link :href="file" target="_blank" type="primary" style="margin-right:12px">
                    <el-icon><Document /></el-icon>
                    Color Attachment {{ index + 1 }}
                  </el-link>
                  <el-button type="primary" link @click="downloadFile(normalizeUrl(file))">Download</el-button>
                </div>
              </div>
            </el-descriptions-item>

            <el-descriptions-item label="Remarks Attachment" v-if="selectedOrder.remarksAttachment">
              <div class="file-list">
                <div
                  v-for="(file, index) in getFileList(selectedOrder.remarksAttachment)"
                  :key="'remark-'+index"
                  class="file-item">
                  <el-link :href="file" target="_blank" type="primary" style="margin-right:12px">
                    <el-icon><Document /></el-icon>
                    Remarks Attachment {{ index + 1 }}
                  </el-link>
                  <el-button type="primary" link @click="downloadFile(normalizeUrl(file))">Download</el-button>
                </div>
              </div>
            </el-descriptions-item>
          </el-descriptions>
        </div>
      </div>

      <template #footer>
        <el-button @click="detailDialogVisible = false">Close</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Document } from '@element-plus/icons-vue'
import request from '@/utils/request'
import { listCustomerOrders, updateOrderStatus } from '@/api/DFGX/customerOrders'
import { getCompleteOrder } from '@/api/DFGX/orderManagement'

// 搜索表单
const searchForm = reactive({
  orderNo: '',
  customerName: '',
  status: '',
  beginTime: '',
  endTime: '',
  minAmount: '',
  maxAmount: ''
})

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 0
})

// 加载状态
const loading = ref(false)

// 选择的行
const selectedRows = ref<any[]>([])

// 详情对话框
const detailDialogVisible = ref(false)
const selectedOrder = ref<any>(null)
const orderDetails = ref<any[]>([])

// 订单列表数据
const orderList = ref<any[]>([])

// 订单状态选项 (optionType = 16, value = 1-6)
const orderStatusOptions = ref([
  { value: 1, label: 'ORDER CREATED' },
  { value: 5, label: 'FINISHED' },
  { value: 6, label: 'SHIPPED' }
])

// 仅显示 1、5、6 阶段
const filteredOrderStatusOptions = computed(() => orderStatusOptions.value)

// 获取订单列表
const getOrderList = async () => {
  try {
    loading.value = true
    const params = {
      pageNum: pagination.currentPage,
      pageSize: pagination.pageSize,
      ...searchForm
    }
    const response: any = await listCustomerOrders(params)
    if (response.data && response.code == 200) {
      orderList.value = response.data.result || []
      pagination.total = response.data.totalNum || 0
    } else {
      ElMessage.error(response.data?.msg || 'Failed to get order list')
    }
  } catch (error) {
    console.error('获取订单列表失败:', error)
    ElMessage.error('Failed to get order list')
  } finally {
    loading.value = false
  }
}

// 获取状态名称
const getStatusName = (status: string) => {
  const statuses: Record<string, string> = {
    '0': 'Pending',
    '1': 'In Production',
    '2': 'Completed',
    '3': 'Cancelled'
  }
  return statuses[status] || status
}

// 获取状态类型
const getStatusType = (status: string) => {
  const types: Record<string, 'primary'|'success'|'warning'|'danger'|'info'> = {
    '0': 'warning',
    '1': 'primary',
    '2': 'success',
    '3': 'danger'
  }
  return types[status] || 'info'
}

// 格式化日期
const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  try {
    const date = new Date(dateStr)
    if (isNaN(date.getTime())) return dateStr
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  } catch {
    return dateStr
  }
}

// 搜索
const handleSearch = () => {
  pagination.currentPage = 1
  getOrderList()
}

// 重置搜索
const resetSearch = () => {
  searchForm.orderNo = ''
  searchForm.customerName = ''
  searchForm.status = ''
  searchForm.beginTime = ''
  searchForm.endTime = ''
  searchForm.minAmount = ''
  searchForm.maxAmount = ''
  pagination.currentPage = 1
  getOrderList()
}

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.currentPage = 1
  getOrderList()
}

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.currentPage = page
  getOrderList()
}

// 行选择
const handleSelectionChange = (rows: any[]) => {
  selectedRows.value = rows || []
}

// 查看订单详情
const viewOrderDetail = async (order: any) => {
  try {
    loading.value = true
    selectedOrder.value = order
    const response: any = await getCompleteOrder(order.id)
    if (response.data && response.code == 200) {
      selectedOrder.value = response.data.order
      orderDetails.value = response.data.orderDetails || []
    } else {
      ElMessage.error(response.data?.msg || 'Failed to get order details')
    }
    detailDialogVisible.value = true
  } catch (error) {
    console.error('获取订单详情失败:', error)
    ElMessage.error('Failed to get order details')
  } finally {
    loading.value = false
  }
}

// 关闭详情对话框
const handleCloseDetail = () => {
  detailDialogVisible.value = false
  selectedOrder.value = null
}

// 获取订单状态步骤（仅 1,5,6）
const getFilteredOrderStatusStep = (orderStatus: string | number) => {
  if (!orderStatus) return 0
  const statusValue = parseInt(orderStatus.toString())
  if (statusValue >= 6) return 2 // SHIPPED
  if (statusValue >= 5) return 1 // FINISHED
  return 0 // ORDER CREATED
}

// 获取状态描述
const getFilteredStatusDescription = (index: number, orderStatus: string | number) => {
  if (!orderStatus) return ''
  const active = getFilteredOrderStatusStep(orderStatus)
  if (index < active) return 'Completed'
  if (index === active) return 'Current Step'
  return 'Pending'
}

// === 附件 URL 规范化（支持文件名/相对路径） ===
const FILE_BASE = '/uploads/'

const normalizeUrl = (u: string) => {
  if (!u) return ''
  return /^https?:\/\//i.test(u)
    ? u
    : FILE_BASE.replace(/\/+$/, '/') + u.replace(/^\/+/, '')
}

// 处理文件列表（支持多个文件，逗号分隔）
const getFileList = (fileString: string) => {
  if (!fileString || fileString.trim() === '') return []
  return fileString.split(',').map(url => url.trim()).filter(url => url !== '')
}

// 从 Content-Disposition 中提取文件名
const extractFilename = (res: any, url: string) => {
  const headerVal = res?.headers?.['content-disposition'] || res?.headers?.get?.('content-disposition')
  if (headerVal) {
    const utf8 = /filename\*\s*=\s*UTF-8''([^;]+)/i.exec(headerVal)
    const ascii = /filename\s*=\s*"([^"]+)"/i.exec(headerVal) || /filename\s*=\s*([^;]+)/i.exec(headerVal)
    const raw = utf8?.[1] || ascii?.[1]
    if (raw) {
      try { return decodeURIComponent(raw.replace(/["']/g, '')) } catch { return raw.replace(/["']/g, '') }
    }
  }
  try {
    const u = new URL(url, window.location.origin)
    const last = u.pathname.split('/').pop() || 'download'
    return last.split('?')[0] || 'download'
  } catch {
    const last = url.split('/').pop() || 'download'
    return last.split('?')[0] || 'download'
  }
}

// 触发浏览器下载
const triggerBrowserDownload = (blob: Blob, fileName: string) => {
  const link = document.createElement('a')
  const objectUrl = URL.createObjectURL(blob)
  link.href = objectUrl
  link.download = fileName || 'download'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  URL.revokeObjectURL(objectUrl)
}

// 单个文件下载（用于附件）
const downloadFile = async (url: string) => {
  try {
    const res = await request({ url: normalizeUrl(url), method: 'get', responseType: 'blob' })
    if ((res.data as any)?.type === 'application/json') {
      const text = await (res.data as any).text?.()
      throw new Error(text || 'Server error')
    }
    const fileName = extractFilename(res, url)
    triggerBrowserDownload(res.data as Blob, fileName)
  } catch (err: any) {
    console.error('下载失败:', err)
    ElMessage.error('Download failed')
  }
}

// 导出所选订单（后端：POST /dfgx/customer-orders/export-selected，Body: long[]）
const exportSelected = async () => {
  if (!selectedRows.value.length) return
  try {
    const ids = selectedRows.value.map((r: any) => r.id).filter((v: any) => v != null)
    if (!ids.length) return ElMessage.info('No valid orders selected')
    loading.value = true
    const url = '/dfgx/customer-orders/export-selected'
    const res = await request({
      url,
      method: 'post',
      data: ids,
      responseType: 'blob'
    })

    if ((res.data as any)?.type === 'application/json') {
      const text = await (res.data as any).text?.()
      throw new Error(text || 'Server error')
    }
    const fileName = extractFilename(res, url) || `客户订单_${new Date().toISOString().slice(0,19).replace(/[-:T]/g,'')}.xlsx`
    triggerBrowserDownload(res.data as Blob, fileName)
    ElMessage.success('Export started')
  } catch (e) {
    console.error('导出失败', e)
    ElMessage.error('Export failed')
  } finally {
    loading.value = false
  }
}

// 完成订单
const completeOrder = async (order: any) => {
  try {
    await ElMessageBox.confirm(
      `Are you sure to mark order ${order.orderNo} as completed?`,
      'Complete Order',
      { confirmButtonText: 'Confirm', cancelButtonText: 'Cancel', type: 'success' }
    )
    const response: any = await updateOrderStatus(order.id, '2', '系统管理员')
    if (response.data && response.code == 200) {
      ElMessage.success('Order completed')
      getOrderList()
    } else {
      ElMessage.error(response.data?.msg || 'Failed to complete order')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('完成订单失败:', error)
      ElMessage.error('Failed to complete order')
    }
  }
}

// 取消订单
const cancelOrder = async (order: any) => {
  try {
    await ElMessageBox.confirm(
      `Are you sure to cancel order ${order.orderNo}?`,
      'Cancel Order',
      { confirmButtonText: 'Confirm', cancelButtonText: 'Cancel', type: 'warning' }
    )
    const response: any = await updateOrderStatus(order.id, '3', '系统管理员')
    if (response.data && response.code === 200) {
      ElMessage.success('Order cancelled')
      getOrderList()
    } else {
      ElMessage.error(response.data?.msg || 'Failed to cancel order')
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('取消订单失败:', error)
      ElMessage.error('Failed to cancel order')
    }
  }
}

// 初始化
onMounted(() => {
  getOrderList()
})
</script>

<style scoped>
.order-view-container {
  padding: 20px;
  background-color: #f5f5f5;
  min-height: 100vh;
}

.search-card {
  margin-bottom: 20px;
}

.order-list-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 16px;
  font-weight: bold;
  color: #333;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}

.order-detail {
  max-height: 600px;
  overflow-y: auto;
}

.order-status-section {
  margin: 20px 0;
  padding: 20px;
  background-color: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #e4e7ed;
}

.order-status-section h4 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 16px;
  font-weight: bold;
  text-align: center;
}

.file-attachments-section {
  margin: 20px 0;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.file-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.file-item {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #e4e7ed;
}

.quantity-info {
  font-size: 12px;
  line-height: 1.4;
}

.quantity-info div {
  margin-bottom: 2px;
}

::deep(.el-card__header) {
  background-color: #f8f9fa;
  border-bottom: 1px solid #e4e7ed;
}

::deep(.el-descriptions__label) {
  font-weight: bold;
  color: #333;
}
</style>