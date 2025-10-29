<template>
  <div class="order-view-container">
    <!-- <div class="order-header">
    </div> -->
    
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
          <el-button type="primary" @click="goToPlaceOrder">
            <el-icon><Plus /></el-icon>
            New Order
          </el-button>
        </div>
      </template>
      
      <el-table :data="orderList" v-loading="loading" stripe>
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
        
        <el-table-column prop="totalAmount" label="Total Amount" width="150" align="right" v-if="false">
          <template #default="{ row }">
            <span class="price">¥{{ row.totalAmount }}</span>
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
        
        <el-table-column label="Actions" width="230" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="viewOrderDetail(row)">
              View Details
            </el-button>
            <el-button 
              v-if="row.status === '1'" 
              type="success" 
              size="small" 
              @click="completeOrder(row)">
              Complete Order
            </el-button>
            <el-button 
              v-if="['0', '1'].includes(row.status)" 
              type="danger" 
              size="small" 
              @click="cancelOrder(row)">
              Cancel Order
            </el-button>
          </template>
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
        <!-- 订单基本信息 -->
        <el-descriptions :column="2" border>
          <el-descriptions-item label="Order No.">{{ selectedOrder.orderNo }}</el-descriptions-item>
          <el-descriptions-item label="Customer Name">{{ selectedOrder.customerName }}</el-descriptions-item>
          <el-descriptions-item label="Order Date">{{ formatDate(selectedOrder.orderDate) }}</el-descriptions-item>
          <el-descriptions-item label="Total Amount" v-if="false">${{ selectedOrder.totalAmount }}</el-descriptions-item>
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
        
        <!-- Order Status Progress -->
        <div class="order-status-section">
          <h4>Order Status Progress</h4>
          <el-steps :active="getOrderStatusStep(selectedOrder.orderStatus)" finish-status="success" align-center>
            <el-step 
              v-for="(status, index) in orderStatusOptions" 
              :key="index"
              :title="status.label"
              :description="getStatusDescription(index, selectedOrder.orderStatus)">
            </el-step>
          </el-steps>
        </div>
        
        <!-- Frame Information -->
        <div v-if="selectedOrder.frameType || selectedOrder.frameModel || selectedOrder.edEda || selectedOrder.sizeA || selectedOrder.sizeB" class="frame-info-section">
          <h4>Frame Information</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="Frame Type">{{ selectedOrder.frameType || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Model">{{ selectedOrder.frameModel || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="ED/EDA">{{ selectedOrder.edEda || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="sizeA-DBL-sizeA">{{ selectedOrder.sizeA || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="sizeB">{{ selectedOrder.sizeB || 'None' }}</el-descriptions-item>
          </el-descriptions>
        </div>
        
        <!-- Lens Information -->
        <div v-if="selectedOrder.dia || selectedOrder.refractiveIndex || selectedOrder.designName || selectedOrder.material || selectedOrder.channel || selectedOrder.coating || selectedOrder.color || selectedOrder.tintPercentageTop || selectedOrder.tintPercentageBottom" class="lens-info-section">
          <h4>Lens Information</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="Dia">{{ selectedOrder.dia || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Index">{{ selectedOrder.refractiveIndexValue || selectedOrder.refractiveIndex || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Design">{{ selectedOrder.designNameText || selectedOrder.designName || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Material">{{ selectedOrder.materialName || selectedOrder.material || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Corridor">{{ selectedOrder.channelName || selectedOrder.channel || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Coating">{{ selectedOrder.coatingName || selectedOrder.coating || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Tinting">{{ selectedOrder.tintingName || selectedOrder.tinting || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Color">{{ selectedOrder.colorName || selectedOrder.color || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Tint Percentage TOP">{{ selectedOrder.tintPercentageTop || 'None' }}</el-descriptions-item>
            <el-descriptions-item label="Tint Percentage BOTTOM">{{ selectedOrder.tintPercentageBottom || 'None' }}</el-descriptions-item>
          </el-descriptions>
        </div>
        
        <!-- Extra Processing Information -->
        <div v-if="selectedOrder.lenticularName || selectedOrder.lenticular" class="extra-processing-section">
          <h4>Extra Processing</h4>
          <el-descriptions :column="2" border>
            <el-descriptions-item label="Lenticular">{{ selectedOrder.lenticularName || selectedOrder.lenticular || 'None' }}</el-descriptions-item>
          </el-descriptions>
        </div>
        
        <!-- Eye Parameters -->
        <div v-if="selectedOrder.rightEyeSph || selectedOrder.rightEyeCyl || selectedOrder.leftEyeSph || selectedOrder.leftEyeCyl" class="eye-params-section">
          <h4>Eye Parameters</h4>
          <div class="eye-params">
            <h5>Right Eye</h5>
            <el-descriptions :column="3" border>
              <el-descriptions-item label="SPH">{{ selectedOrder.rightEyeSph || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="CYL">{{ selectedOrder.rightEyeCyl || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="AXIS">{{ selectedOrder.rightEyeAxis || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="ADD">{{ selectedOrder.rightEyeAdd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="PRISM">{{ selectedOrder.rightEyePrism || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="DIRECTION">{{ selectedOrder.rightEyePrismDirection || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="BASE">{{ selectedOrder.rightEyeBase || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="PH">{{ selectedOrder.rightEyePh || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="NEAR PD">{{ selectedOrder.rightEyeNearPd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="DISTANCE PD">{{ selectedOrder.rightEyeDistancePd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="QUANTITY">{{ selectedOrder.rightEyeQuantity || '1' }}</el-descriptions-item>
            </el-descriptions>
          </div>
          <div class="eye-params">
            <h5>Left Eye</h5>
            <el-descriptions :column="3" border>
              <el-descriptions-item label="SPH">{{ selectedOrder.leftEyeSph || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="CYL">{{ selectedOrder.leftEyeCyl || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="AXIS">{{ selectedOrder.leftEyeAxis || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="ADD">{{ selectedOrder.leftEyeAdd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="PRISM">{{ selectedOrder.leftEyePrism || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="DIRECTION">{{ selectedOrder.leftEyePrismDirection || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="BASE">{{ selectedOrder.leftEyeBase || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="PH">{{ selectedOrder.leftEyePh || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="NEAR PD">{{ selectedOrder.leftEyeNearPd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="DISTANCE PD">{{ selectedOrder.leftEyeDistancePd || 'None' }}</el-descriptions-item>
              <el-descriptions-item label="QUANTITY">{{ selectedOrder.leftEyeQuantity || '1' }}</el-descriptions-item>
            </el-descriptions>
          </div>
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
            <el-table-column prop="unitPrice" label="Unit Price" width="100" align="right" v-if="false">
              <template #default="{ row }">${{ row.unitPrice }}</template>
            </el-table-column>
            <el-table-column prop="totalPrice" label="Total Price" width="100" align="right" v-if="false" >
              <template #default="{ row }">${{ row.totalPrice }}</template>
            </el-table-column>
          </el-table>
        </div>
        
        <!-- 文件附件（已集成下载） -->
        <div v-if="selectedOrder.colorAttachment || selectedOrder.remarksAttachment" class="file-attachments-section">
          <h4>File Attachments</h4>
          <el-descriptions :column="1" border>
            <el-descriptions-item label="Color Attachment" v-if="selectedOrder.colorAttachment">
              <div class="file-list">
                <div
                  v-for="(file, index) in getFileList(selectedOrder.colorAttachment)"
                  :key="'color-'+index"
                  class="file-item">
                  <!-- 预览：新窗口打开 -->
                  <el-link :href="file" target="_blank" type="primary" style="margin-right:12px">
                    <el-icon><Document /></el-icon>
                    Color Attachment {{ index + 1 }}
                  </el-link>
                  <!-- 下载：blob方式（带Token/CORS也能下） -->
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

          <!-- 批量下载（可选） -->
          <div style="margin-top:10px">
            <el-button type="primary" @click="downloadAllAttachments(selectedOrder)">Download All</el-button>
          </div>
        </div>
        
        <!-- 状态历史 -->
        <div v-if="statusHistory.length > 0" class="status-history-section">
          <h4>Status History</h4>
          <el-timeline>
            <el-timeline-item
              v-for="(history, index) in statusHistory"
              :key="index"
              :timestamp="formatDate(history.createTime)"
              :type="getStatusType(history.status)">
              {{ history.statusName }} - {{ history.operator }}
            </el-timeline-item>
          </el-timeline>
        </div>
      </div>
      
      <template #footer>
        <el-button @click="detailDialogVisible = false">Close</el-button>
        <el-button type="primary" @click="goToPlaceOrder">New Order</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Plus, Document } from '@element-plus/icons-vue'
import request from '@/utils/request' // 使用统一封装的 request
import { listCustomerOrders, getCustomerOrder, updateOrderStatus, delCustomerOrder } from '@/api/DFGX/customerOrders'
import { getOrderDetailsByOrderId } from '@/api/DFGX/orderDetails'
import { getOrderStatusHistoryByOrderId, recordOrderStatusChange } from '@/api/DFGX/orderStatusHistory'
import { getCompleteOrder } from '@/api/DFGX/orderManagement'

const router = useRouter()

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

// 详情对话框
const detailDialogVisible = ref(false)
const selectedOrder = ref<any>(null)
const orderDetails = ref<any[]>([])
const statusHistory = ref<any[]>([])

// 订单列表数据
const orderList = ref<any[]>([])

// 订单状态选项 (optionType = 16, value = 1-6)
const orderStatusOptions = ref([
  { value: 1, label: 'ORDER CREATED' },
  { value: 2, label: 'CALCULATION' },
  { value: 3, label: 'IN PRODUCTION' },
  { value: 4, label: 'QUALITY INSPECTION' },
  { value: 5, label: 'FINISHED' },
  { value: 6, label: 'SHIPPED' }
])

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

// 获取染色名称
const getTintingName = (tinting: string) => {
  const tintings: Record<string, string> = {
    'none': 'No Tinting',
    'light': 'Light Tinting',
    'medium': 'Medium Tinting',
    'dark': 'Dark Tinting',
    'photochromic': 'Photochromic'
  }
  return tintings[tinting] || tinting
}

// 获取膜层名称
const getCoatingName = (coating: string) => {
  const coatings: Record<string, string> = {
    'none': 'No Coating',
    'ar': 'Anti-Reflective',
    'backside-ar': 'Backside Waterproof Green',
    'uv': 'UV Protection',
    'scratch-resistant': 'Scratch Resistant'
  }
  return coatings[coating] || coating
}

// 获取镜框类型名称
const getFrameTypeName = (type: string) => {
  const types: Record<string, string> = {
    'full': 'Full Frame',
    'half': 'Half Frame',
    'none': 'Rimless'
  }
  return types[type] || type
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

// 查看订单详情
const viewOrderDetail = async (order: any) => {
  try {
    loading.value = true
    selectedOrder.value = order
    const response: any = await getCompleteOrder(order.id)
    if (response.data && response.code == 200) {
      selectedOrder.value = response.data.order
      orderDetails.value = response.data.orderDetails || []
      statusHistory.value = response.data.statusHistory || []
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

// 获取订单状态步骤
const getOrderStatusStep = (orderStatus: string | number) => {
  if (!orderStatus) return 0
  const statusValue = parseInt(orderStatus.toString())
  return Math.max(0, statusValue - 1)
}

// 获取状态描述
const getStatusDescription = (index: number, orderStatus: string | number) => {
  if (!orderStatus) return ''
  const statusValue = parseInt(orderStatus.toString())
  const currentStep = Math.max(0, statusValue - 1)
  if (index < currentStep) return 'Completed'
  if (index === currentStep) return 'Current Step'
  return 'Pending'
}

// 处理文件列表（支持多个文件，逗号分隔）
const getFileList = (fileString: string) => {
  if (!fileString || fileString.trim() === '') return []
  return fileString.split(',').map(url => url.trim()).filter(url => url !== '')
}

// === 附件 URL 规范化（支持文件名/相对路径） ===
const FILE_BASE = import.meta.env.VITE_FILE_BASE_URL || '/uploads/'

const normalizeUrl = (u: string) => {
  if (!u) return ''
  return /^https?:\/\//i.test(u)
    ? u
    : FILE_BASE.replace(/\/+$/, '/') + u.replace(/^\/+/, '')
}

// === ↓↓↓ 下载相关：可直接复用 ↓↓↓ ===

// 从 Content-Disposition 中提取文件名（若无则从 URL 猜）
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

// 单个文件下载（使用统一 request；拦截器会自动带 token）
const downloadFile = async (url: string) => {
  try {
    const res = await request({
      url: normalizeUrl(url),
      method: 'get',
      responseType: 'blob'
    })

    // 防止后端错误被代理成 200 + JSON
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

// 批量下载（简单串行，需并发可改 Promise.all）
const downloadAllAttachments = async (order: any) => {
  const all = [
  ...(getFileList(order?.colorAttachment) || []).map(normalizeUrl),
  ...(getFileList(order?.remarksAttachment) || []).map(normalizeUrl)
]
  if (!all.length) return ElMessage.info('No attachments')
  for (const url of all) {
    await downloadFile(url)
  }
}

// === ↑↑↑ 下载相关结束 ↑↑↑ ===

// 完成订单
const completeOrder = async (order: any) => {
  try {
    await ElMessageBox.confirm(
      `Are you sure to mark order ${order.orderNo} as completed?`,
      'Complete Order',
      {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'success'
      }
    )
    
    const response: any = await updateOrderStatus(order.id, '2', '系统管理员')
    if (response.data && response.code == 200) {
      ElMessage.success('Order completed')
      await recordOrderStatusChange(order.id, '2', '已完成', '系统管理员')
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
      {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning'
      }
    )
    
    const response: any = await updateOrderStatus(order.id, '3', '系统管理员')
    if (response.data && response.code === 200) {
      ElMessage.success('Order cancelled')
      await recordOrderStatusChange(order.id, '3', '已取消', '系统管理员')
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

// 跳转到下单页面
const goToPlaceOrder = () => {
  router.push('/PlaceAnOrder')
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

.order-header {
  text-align: center;
  margin-bottom: 30px;
}

.order-header h1 {
  color: #333;
  font-size: 28px;
  font-weight: bold;
  margin-bottom: 10px;
}

.order-header p {
  color: #666;
  font-size: 16px;
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

.lens-info {
  font-size: 12px;
  line-height: 1.4;
}

.lens-info div {
  margin-bottom: 2px;
}

.price {
  font-weight: bold;
  color: #e6a23c;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}

.order-detail {
  max-height: 600px;
  overflow-y: auto;
}

.frame-info-section {
  margin: 20px 0;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.frame-info-section h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 16px;
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

.file-attachments-section h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 16px;
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

.file-item:hover {
  background-color: #e9ecef;
}

.lens-info-section {
  margin: 20px 0;
  padding: 15px;
  background-color: #f0f9ff;
  border-radius: 6px;
  border-left: 4px solid #3b82f6;
}

.lens-info-section h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 16px;
}

.eye-params-section {
  margin: 20px 0;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.eye-params-section h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 16px;
}

.eye-params {
  background-color: #fff;
  padding: 15px;
  border-radius: 6px;
  border: 1px solid #e4e7ed;
}

.eye-params h5 {
  margin: 0 0 10px 0;
  color: #333;
  font-size: 14px;
  font-weight: bold;
}

.remarks-section {
  margin: 20px 0;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.remarks-section h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 16px;
}

.remarks-content p {
  margin: 10px 0;
  line-height: 1.6;
}

.auto-remarks {
  margin-top: 10px;
}

.remark-tag {
  margin-right: 8px;
  margin-bottom: 8px;
}

:deep(.el-table__row:hover) {
  background-color: #f5f7fa;
}

:deep(.el-descriptions__label) {
  font-weight: bold;
  color: #333;
}

:deep(.el-card__header) {
  background-color: #f8f9fa;
  border-bottom: 1px solid #e4e7ed;
}

.quantity-info {
  font-size: 12px;
  line-height: 1.4;
}

.quantity-info div {
  margin-bottom: 2px;
}
</style>