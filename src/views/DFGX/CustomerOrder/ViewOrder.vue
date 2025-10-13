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
            placeholder="订单号"
            clearable
            @clear="handleSearch">
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-select v-model="searchForm.status" placeholder="订单状态" clearable @change="handleSearch">
            <el-option label="全部" value=""></el-option>
            <el-option label="待处理" value="pending"></el-option>
            <el-option label="处理中" value="processing"></el-option>
            <el-option label="已完成" value="completed"></el-option>
            <el-option label="已取消" value="cancelled"></el-option>
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="searchForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleSearch">
          </el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="resetSearch">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- 订单列表 -->
    <el-card class="order-list-card" shadow="hover">
      <template #header>
        <div class="card-header">
          <span>订单列表</span>
          <el-button type="primary" @click="goToPlaceOrder">
            <el-icon><Plus /></el-icon>
            新建订单
          </el-button>
        </div>
      </template>
      
      <el-table :data="filteredOrders" v-loading="loading" stripe>
        <el-table-column prop="orderNo" label="订单号" width="150" fixed="left">
          <template #default="{ row }">
            <el-link type="primary" @click="viewOrderDetail(row)">
              {{ row.orderNo }}
            </el-link>
          </template>
        </el-table-column>
        
        <el-table-column prop="createTime" label="下单时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.createTime) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="lensInfo" label="镜片信息" width="200">
          <template #default="{ row }">
            <div class="lens-info">
              <div><strong>类型：</strong>{{ getLensTypeName(row.lensType) }}</div>
              <div><strong>材质：</strong>{{ row.material }}</div>
              <div><strong>膜层：</strong>{{ getCoatingName(row.coating) }}</div>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="quantity" label="数量" width="80" align="center">
        </el-table-column>
        
        <el-table-column prop="totalPrice" label="总价" width="120" align="right">
          <template #default="{ row }">
            <span class="price">¥{{ row.totalPrice }}</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="status" label="状态" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusName(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="remarks" label="备注" min-width="200" show-overflow-tooltip>
        </el-table-column>
        
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="viewOrderDetail(row)">
              查看详情
            </el-button>
            <el-button 
              v-if="row.status === 'pending'" 
              type="danger" 
              size="small" 
              @click="cancelOrder(row)">
              取消订单
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
      title="订单详情"
      width="800px"
      :before-close="handleCloseDetail">
      <div v-if="selectedOrder" class="order-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="订单号">{{ selectedOrder.orderNo }}</el-descriptions-item>
          <el-descriptions-item label="下单时间">{{ formatDate(selectedOrder.createTime) }}</el-descriptions-item>
          <el-descriptions-item label="镜片类型">{{ getLensTypeName(selectedOrder.lensType) }}</el-descriptions-item>
          <el-descriptions-item label="膜层">{{ getCoatingName(selectedOrder.coating) }}</el-descriptions-item>
          <el-descriptions-item label="途径">{{ selectedOrder.path }}</el-descriptions-item>
          <el-descriptions-item label="材质">{{ selectedOrder.material }}</el-descriptions-item>
          <el-descriptions-item label="镜框类型">{{ getFrameTypeName(selectedOrder.frameType) }}</el-descriptions-item>
          <el-descriptions-item label="数量">{{ selectedOrder.quantity }}</el-descriptions-item>
          <el-descriptions-item label="单价">¥{{ selectedOrder.unitPrice }}</el-descriptions-item>
          <el-descriptions-item label="总价">¥{{ selectedOrder.totalPrice }}</el-descriptions-item>
          <el-descriptions-item label="订单状态">
            <el-tag :type="getStatusType(selectedOrder.status)">
              {{ getStatusName(selectedOrder.status) }}
            </el-tag>
          </el-descriptions-item>
        </el-descriptions>
        
        <!-- 左右眼参数 -->
        <div class="eye-params-section">
          <h4>左右眼参数</h4>
          <el-row :gutter="20">
            <el-col :span="12">
              <div class="eye-params">
                <h5>左眼参数</h5>
                <el-descriptions :column="2" size="small">
                  <el-descriptions-item label="SPH">{{ selectedOrder.leftEye.sph || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="CYL">{{ selectedOrder.leftEye.cyl || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="AXIS">{{ selectedOrder.leftEye.axis || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="ADD">{{ selectedOrder.leftEye.add || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="PRISM">{{ selectedOrder.leftEye.prism || '-' }}</el-descriptions-item>
                </el-descriptions>
              </div>
            </el-col>
            <el-col :span="12">
              <div class="eye-params">
                <h5>右眼参数</h5>
                <el-descriptions :column="2" size="small">
                  <el-descriptions-item label="SPH">{{ selectedOrder.rightEye.sph || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="CYL">{{ selectedOrder.rightEye.cyl || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="AXIS">{{ selectedOrder.rightEye.axis || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="ADD">{{ selectedOrder.rightEye.add || '-' }}</el-descriptions-item>
                  <el-descriptions-item label="PRISM">{{ selectedOrder.rightEye.prism || '-' }}</el-descriptions-item>
                </el-descriptions>
              </div>
            </el-col>
          </el-row>
        </div>
        
        <!-- 备注信息 -->
        <div class="remarks-section">
          <h4>备注信息</h4>
          <div class="remarks-content">
            <p><strong>用户备注：</strong>{{ selectedOrder.remarks || '无' }}</p>
            <div v-if="selectedOrder.autoRemarks && selectedOrder.autoRemarks.length > 0">
              <p><strong>自动备注：</strong></p>
              <div class="auto-remarks">
                <el-tag v-for="remark in selectedOrder.autoRemarks" :key="remark" type="info" class="remark-tag">
                  {{ remark }}
                </el-tag>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <template #footer>
        <el-button @click="detailDialogVisible = false">关闭</el-button>
        <el-button type="primary" @click="goToPlaceOrder">新建订单</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'

const router = useRouter()

// 搜索表单
const searchForm = reactive({
  orderNo: '',
  status: '',
  dateRange: []
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
const selectedOrder = ref(null)

// Mock数据 - 订单历史记录
const mockOrders = [
  {
    id: 1,
    orderNo: 'ORD20241201001',
    createTime: '2024-12-01 10:30:00',
    lensType: 'single',
    coating: 'backside-ar',
    path: '9mm',
    material: 'MR8',
    frameType: 'full',
    quantity: 2,
    unitPrice: 180,
    totalPrice: 360,
    status: 'completed',
    leftEye: {
      sph: '-2.50',
      cyl: '-0.75',
      axis: '180',
      add: '',
      prism: ''
    },
    rightEye: {
      sph: '-2.25',
      cyl: '-0.50',
      axis: '175',
      add: '',
      prism: ''
    },
    remarks: '请尽快处理',
    autoRemarks: ['ST自动改成FT', '途径9mm是ACOMODA', '膜层Backside AR改成背面超防水绿膜', '需要不干胶标贴']
  },
  {
    id: 2,
    orderNo: 'ORD20241130002',
    createTime: '2024-11-30 14:20:00',
    lensType: 'progressive',
    coating: 'ar',
    path: '8mm',
    material: 'MR174',
    frameType: 'half',
    quantity: 1,
    unitPrice: 380,
    totalPrice: 380,
    status: 'processing',
    leftEye: {
      sph: '+1.50',
      cyl: '',
      axis: '',
      add: '+2.00',
      prism: ''
    },
    rightEye: {
      sph: '+1.25',
      cyl: '',
      axis: '',
      add: '+2.00',
      prism: ''
    },
    remarks: '',
    autoRemarks: ['需要不干胶标贴']
  },
  {
    id: 3,
    orderNo: 'ORD20241129003',
    createTime: '2024-11-29 09:15:00',
    lensType: 'blue-light',
    coating: 'uv',
    path: '7mm',
    material: 'CR39',
    frameType: 'none',
    quantity: 1,
    unitPrice: 190,
    totalPrice: 190,
    status: 'pending',
    leftEye: {
      sph: '-1.00',
      cyl: '-0.25',
      axis: '90',
      add: '',
      prism: ''
    },
    rightEye: {
      sph: '-0.75',
      cyl: '-0.25',
      axis: '85',
      add: '',
      prism: ''
    },
    remarks: '防蓝光镜片',
    autoRemarks: ['需要不干胶标贴']
  },
  {
    id: 4,
    orderNo: 'ORD20241128004',
    createTime: '2024-11-28 16:45:00',
    lensType: 'bifocal',
    coating: 'scratch-resistant',
    path: '10mm',
    material: 'PC',
    frameType: 'full',
    quantity: 1,
    unitPrice: 270,
    totalPrice: 270,
    status: 'cancelled',
    leftEye: {
      sph: '+2.00',
      cyl: '',
      axis: '',
      add: '+1.50',
      prism: ''
    },
    rightEye: {
      sph: '+1.75',
      cyl: '',
      axis: '',
      add: '+1.50',
      prism: ''
    },
    remarks: '客户取消',
    autoRemarks: ['需要不干胶标贴']
  },
  {
    id: 5,
    orderNo: 'ORD20241127005',
    createTime: '2024-11-27 11:30:00',
    lensType: 'single',
    coating: 'none',
    path: '6mm',
    material: 'MR7',
    frameType: 'half',
    quantity: 2,
    unitPrice: 120,
    totalPrice: 240,
    status: 'completed',
    leftEye: {
      sph: '-3.00',
      cyl: '-1.00',
      axis: '170',
      add: '',
      prism: ''
    },
    rightEye: {
      sph: '-2.75',
      cyl: '-0.75',
      axis: '175',
      add: '',
      prism: ''
    },
    remarks: '',
    autoRemarks: ['ST自动改成FT', '需要不干胶标贴']
  }
]

// 过滤后的订单列表
const filteredOrders = computed(() => {
  let orders = [...mockOrders]
  
  // 按订单号搜索
  if (searchForm.orderNo) {
    orders = orders.filter(order => 
      order.orderNo.toLowerCase().includes(searchForm.orderNo.toLowerCase())
    )
  }
  
  // 按状态筛选
  if (searchForm.status) {
    orders = orders.filter(order => order.status === searchForm.status)
  }
  
  // 按日期范围筛选
  if (searchForm.dateRange && searchForm.dateRange.length === 2) {
    const [startDate, endDate] = searchForm.dateRange
    orders = orders.filter(order => {
      const orderDate = order.createTime.split(' ')[0]
      return orderDate >= startDate && orderDate <= endDate
    })
  }
  
  // 更新总数
  pagination.total = orders.length
  
  // 分页
  const start = (pagination.currentPage - 1) * pagination.pageSize
  const end = start + pagination.pageSize
  return orders.slice(start, end)
})

// 获取镜片类型名称
const getLensTypeName = (type: string) => {
  const types = {
    'single': '单焦点',
    'bifocal': '双焦点',
    'progressive': '渐进多焦点',
    'blue-light': '防蓝光'
  }
  return types[type] || type
}

// 获取膜层名称
const getCoatingName = (coating: string) => {
  const coatings = {
    'none': '无膜层',
    'ar': '防反射膜',
    'backside-ar': '背面超防水绿膜',
    'uv': '防紫外线膜',
    'scratch-resistant': '防刮膜'
  }
  return coatings[coating] || coating
}

// 获取镜框类型名称
const getFrameTypeName = (type: string) => {
  const types = {
    'full': '全框',
    'half': '半框',
    'none': '无框'
  }
  return types[type] || type
}

// 获取状态名称
const getStatusName = (status: string) => {
  const statuses = {
    'pending': '待处理',
    'processing': '处理中',
    'completed': '已完成',
    'cancelled': '已取消'
  }
  return statuses[status] || status
}

// 获取状态类型
const getStatusType = (status: string) => {
  const types = {
    'pending': 'warning',
    'processing': 'primary',
    'completed': 'success',
    'cancelled': 'danger'
  }
  return types[status] || 'info'
}

// 格式化日期
const formatDate = (dateStr: string) => {
  return dateStr
}

// 搜索
const handleSearch = () => {
  pagination.currentPage = 1
}

// 重置搜索
const resetSearch = () => {
  searchForm.orderNo = ''
  searchForm.status = ''
  searchForm.dateRange = []
  pagination.currentPage = 1
}

// 分页大小改变
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.currentPage = 1
}

// 当前页改变
const handleCurrentChange = (page: number) => {
  pagination.currentPage = page
}

// 查看订单详情
const viewOrderDetail = (order: any) => {
  selectedOrder.value = order
  detailDialogVisible.value = true
}

// 关闭详情对话框
const handleCloseDetail = () => {
  detailDialogVisible.value = false
  selectedOrder.value = null
}

// 取消订单
const cancelOrder = async (order: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要取消订单 ${order.orderNo} 吗？`,
      '取消订单',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 更新订单状态
    order.status = 'cancelled'
    ElMessage.success('订单已取消')
  } catch {
    // 用户取消操作
  }
}

// 跳转到下单页面
const goToPlaceOrder = () => {
  router.push('/PlaceAnOrder')
}

// 初始化
onMounted(() => {
  pagination.total = mockOrders.length
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
</style>