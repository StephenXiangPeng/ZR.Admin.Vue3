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
          <el-input
            v-model="searchForm.customerName"
            placeholder="客户姓名"
            clearable
            @clear="handleSearch">
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-input
            v-model="searchForm.customerPhone"
            placeholder="客户电话"
            clearable
            @clear="handleSearch">
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-select v-model="searchForm.status" placeholder="订单状态" clearable @change="handleSearch">
            <el-option label="全部" value=""></el-option>
            <el-option label="待处理" value="0"></el-option>
            <el-option label="生产中" value="1"></el-option>
            <el-option label="已完成" value="2"></el-option>
            <el-option label="已取消" value="3"></el-option>
          </el-select>
        </el-col>
      </el-row>
      
      <el-row :gutter="20" style="margin-top: 15px;">
        <el-col :span="6">
          <el-date-picker
            v-model="searchForm.beginTime"
            type="datetime"
            placeholder="开始日期"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            @change="handleSearch">
          </el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="searchForm.endTime"
            type="datetime"
            placeholder="结束日期"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            @change="handleSearch">
          </el-date-picker>
        </el-col>
        <el-col :span="6">
          <el-input-number
            v-model="searchForm.minAmount"
            placeholder="最小金额"
            :min="0"
            :precision="2"
            @change="handleSearch">
          </el-input-number>
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
      
      <el-table :data="orderList" v-loading="loading" stripe>
        <el-table-column prop="orderNo" label="订单号" width="150" fixed="left">
          <template #default="{ row }">
            <el-link type="primary" @click="viewOrderDetail(row)">
              {{ row.orderNo }}
            </el-link>
          </template>
        </el-table-column>
        
        <el-table-column prop="customerName" label="客户姓名" width="120">
        </el-table-column>
        
        <el-table-column prop="customerPhone" label="客户电话" width="130">
        </el-table-column>
        
        <el-table-column prop="orderDate" label="下单时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.orderDate) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="totalAmount" label="总金额" width="120" align="right">
          <template #default="{ row }">
            <span class="price">¥{{ row.totalAmount }}</span>
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
        
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="viewOrderDetail(row)">
              查看详情
            </el-button>
            <el-button 
              v-if="row.status === '1'" 
              type="success" 
              size="small" 
              @click="completeOrder(row)">
              完成订单
            </el-button>
            <el-button 
              v-if="['0', '1'].includes(row.status)" 
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
        <!-- 订单基本信息 -->
        <el-descriptions :column="2" border>
          <el-descriptions-item label="订单号">{{ selectedOrder.orderNo }}</el-descriptions-item>
          <el-descriptions-item label="客户姓名">{{ selectedOrder.customerName }}</el-descriptions-item>
          <el-descriptions-item label="客户电话">{{ selectedOrder.customerPhone }}</el-descriptions-item>
          <el-descriptions-item label="客户邮箱">{{ selectedOrder.customerEmail }}</el-descriptions-item>
          <el-descriptions-item label="下单时间">{{ formatDate(selectedOrder.orderDate) }}</el-descriptions-item>
          <el-descriptions-item label="总金额">¥{{ selectedOrder.totalAmount }}</el-descriptions-item>
          <el-descriptions-item label="订单状态">
            <el-tag :type="getStatusType(selectedOrder.status)">
              {{ getStatusName(selectedOrder.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="备注">{{ selectedOrder.remarks || '无' }}</el-descriptions-item>
        </el-descriptions>
        
        <!-- 订单详情列表 -->
        <div v-if="orderDetails.length > 0" class="order-details-section">
          <h4>订单详情</h4>
          <el-table :data="orderDetails" size="small" border>
            <el-table-column prop="lensTypeName" label="镜片类型" width="120"></el-table-column>
            <el-table-column prop="coatingName" label="膜层" width="120"></el-table-column>
            <el-table-column prop="materialName" label="材质" width="120"></el-table-column>
            <el-table-column prop="frameType" label="镜框类型" width="100"></el-table-column>
            <el-table-column prop="quantity" label="数量" width="80" align="center"></el-table-column>
            <el-table-column prop="unitPrice" label="单价" width="100" align="right">
              <template #default="{ row }">¥{{ row.unitPrice }}</template>
            </el-table-column>
            <el-table-column prop="totalPrice" label="总价" width="100" align="right">
              <template #default="{ row }">¥{{ row.totalPrice }}</template>
            </el-table-column>
          </el-table>
        </div>
        
        <!-- 状态历史 -->
        <div v-if="statusHistory.length > 0" class="status-history-section">
          <h4>状态历史</h4>
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
import { listCustomerOrders, getCustomerOrder, updateOrderStatus, delCustomerOrder } from '@/api/DFGX/customerOrders'
import { getOrderDetailsByOrderId } from '@/api/DFGX/orderDetails'
import { getOrderStatusHistoryByOrderId, recordOrderStatusChange } from '@/api/DFGX/orderStatusHistory'
import { getCompleteOrder } from '@/api/DFGX/orderManagement'

const router = useRouter()

// 搜索表单
const searchForm = reactive({
  orderNo: '',
  customerName: '',
  customerPhone: '',
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
const selectedOrder = ref(null)
const orderDetails = ref([])
const statusHistory = ref([])

// 订单列表数据
const orderList = ref([])

// 获取订单列表
const getOrderList = async () => {
  try {
    loading.value = true
    const params = {
      pageNum: pagination.currentPage,
      pageSize: pagination.pageSize,
      ...searchForm
    }
    
    const response = await listCustomerOrders(params)
    if (response.code == 200) {
      orderList.value = response.data.result || []
      pagination.total = response.data.totalNum || 0
    } else {
      ElMessage.error(response.data.msg || '获取订单列表失败')
    }
  } catch (error) {
    console.error('获取订单列表失败:', error)
    ElMessage.error('获取订单列表失败')
  } finally {
    loading.value = false
  }
}

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
    '0': '待处理',
    '1': '生产中',
    '2': '已完成',
    '3': '已取消'
  }
  return statuses[status] || status
}

// 获取状态类型
const getStatusType = (status: string) => {
  const types = {
    '0': 'warning',
    '1': 'primary',
    '2': 'success',
    '3': 'danger'
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
  getOrderList()
}

// 重置搜索
const resetSearch = () => {
  searchForm.orderNo = ''
  searchForm.customerName = ''
  searchForm.customerPhone = ''
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
    
    // 获取完整订单信息
    const response = await getCompleteOrder(order.id)
    if (response.code == 200) {
      selectedOrder.value = response.data.order
      orderDetails.value = response.data.orderDetails || []
      statusHistory.value = response.data.statusHistory || []
    } else {
      ElMessage.error(response.data.msg || '获取订单详情失败')
    }
    
    detailDialogVisible.value = true
  } catch (error) {
    console.error('获取订单详情失败:', error)
    ElMessage.error('获取订单详情失败')
  } finally {
    loading.value = false
  }
}

// 关闭详情对话框
const handleCloseDetail = () => {
  detailDialogVisible.value = false
  selectedOrder.value = null
}

// 完成订单
const completeOrder = async (order: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要将订单 ${order.orderNo} 标记为已完成吗？`,
      '完成订单',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'success'
      }
    )
    
    const response = await updateOrderStatus(order.id, '2', '系统管理员')
    if (response.code == 200) {
      ElMessage.success('订单已完成')
      // 记录状态变更
      await recordOrderStatusChange(order.id, '2', '已完成', '系统管理员')
      // 刷新订单列表
      getOrderList()
    } else {
      ElMessage.error(response.data.msg || '完成订单失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('完成订单失败:', error)
      ElMessage.error('完成订单失败')
    }
  }
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
    
    const response = await updateOrderStatus(order.id, '3', '系统管理员')
    if (response.code === 200) {
      ElMessage.success('订单已取消')
      // 记录状态变更
      await recordOrderStatusChange(order.id, '3', '已取消', '系统管理员')
      // 刷新订单列表
      getOrderList()
    } else {
      ElMessage.error(response.data.msg || '取消订单失败')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('取消订单失败:', error)
      ElMessage.error('取消订单失败')
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