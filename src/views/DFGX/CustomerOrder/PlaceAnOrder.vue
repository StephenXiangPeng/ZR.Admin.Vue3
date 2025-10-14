<template>
  <div class="order-container">
    <div class="order-header">
    </div>
    
    <div class="order-form">
      <el-form :model="orderForm" :rules="rules" ref="orderFormRef" label-width="120px">
        <!-- 客户信息 -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>客户信息</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="订单编号" prop="orderNo">
                <el-input v-model="orderForm.orderNo" placeholder="请输入订单编号" @blur="checkOrderNoUnique">
                  <template #append>
                    <el-button @click="generateOrderNo" :loading="generatingOrderNo">生成</el-button>
                  </template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="客户姓名" prop="customerName">
                <el-input v-model="orderForm.customerName" placeholder="请输入客户姓名"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="客户电话" prop="customerPhone">
                <el-input v-model="orderForm.customerPhone" placeholder="请输入客户电话"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="客户邮箱" prop="customerEmail">
                <el-input v-model="orderForm.customerEmail" placeholder="请输入客户邮箱"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="订单日期" prop="orderDate">
                <el-date-picker
                  v-model="orderForm.orderDate"
                  type="datetime"
                  placeholder="选择订单日期"
                  format="YYYY-MM-DD HH:mm:ss"
                  value-format="YYYY-MM-DD HH:mm:ss"
                  style="width: 100%">
                </el-date-picker>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- 镜片信息 -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>镜片信息</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="镜片类型" prop="lensType">
                <el-select v-model="orderForm.lensType" placeholder="请选择镜片类型" style="width: 100%">
                  <el-option 
                    v-for="option in lensTypeOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="膜层" prop="coating">
                <el-select v-model="orderForm.coating" placeholder="请选择膜层" style="width: 100%">
                  <el-option 
                    v-for="option in coatingOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="设计名称" prop="designName">
                <el-select v-model="orderForm.designName" placeholder="请选择设计名称" style="width: 100%">
                  <el-option 
                    v-for="option in designNameOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="材质" prop="material">
                <el-select v-model="orderForm.material" placeholder="请选择材质" style="width: 100%">
                  <el-option 
                    v-for="option in materialOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="折射率" prop="refractiveIndex">
                <el-select v-model="orderForm.refractiveIndex" placeholder="请选择折射率" style="width: 100%">
                  <el-option 
                    v-for="option in refractiveIndexOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="镜框类型" prop="frameType">
                <el-radio-group v-model="orderForm.frameType">
                  <el-radio label="full">全框</el-radio>
                  <el-radio label="half">半框</el-radio>
                  <el-radio label="none">无框</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="数量" prop="quantity">
                <el-input-number v-model="orderForm.quantity" :min="1" :max="100" style="width: 100%"></el-input-number>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- 左右眼参数 -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>左右眼参数</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <div class="eye-params">
                <h4>左眼参数</h4>
                <el-row :gutter="10">
                  <el-col :span="8">
                    <el-form-item label="SPH" prop="leftEye.sph">
                      <el-input v-model="orderForm.leftEye.sph" placeholder="球镜"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="8">
                    <el-form-item label="CYL" prop="leftEye.cyl">
                      <el-input v-model="orderForm.leftEye.cyl" placeholder="柱镜"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="8">
                    <el-form-item label="AXIS" prop="leftEye.axis">
                      <el-input v-model="orderForm.leftEye.axis" placeholder="轴位"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="10">
                  <el-col :span="12">
                    <el-form-item label="ADD" prop="leftEye.add">
                      <el-input v-model="orderForm.leftEye.add" placeholder="下加光"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="PRISM" prop="leftEye.prism">
                      <el-input v-model="orderForm.leftEye.prism" placeholder="棱镜"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
              </div>
            </el-col>
            <el-col :span="12">
              <div class="eye-params">
                <h4>右眼参数</h4>
                <el-row :gutter="10">
                  <el-col :span="8">
                    <el-form-item label="SPH" prop="rightEye.sph">
                      <el-input v-model="orderForm.rightEye.sph" placeholder="球镜"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="8">
                    <el-form-item label="CYL" prop="rightEye.cyl">
                      <el-input v-model="orderForm.rightEye.cyl" placeholder="柱镜"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="8">
                    <el-form-item label="AXIS" prop="rightEye.axis">
                      <el-input v-model="orderForm.rightEye.axis" placeholder="轴位"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="10">
                  <el-col :span="12">
                    <el-form-item label="ADD" prop="rightEye.add">
                      <el-input v-model="orderForm.rightEye.add" placeholder="下加光"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="PRISM" prop="rightEye.prism">
                      <el-input v-model="orderForm.rightEye.prism" placeholder="棱镜"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
              </div>
            </el-col>
          </el-row>
        </el-card>

        <!-- 备注信息 -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>备注信息</span>
            </div>
          </template>
          
          <el-form-item label="备注" prop="remarks">
            <el-input 
              v-model="orderForm.remarks" 
              type="textarea" 
              :rows="4" 
              placeholder="请输入备注信息"
              show-word-limit
              maxlength="500">
            </el-input>
          </el-form-item>
          
          <div class="auto-remarks">
            <h4>自动添加的备注：</h4>
            <div class="remarks-list">
              <el-tag v-if="autoRemarks.includes('ST自动改成FT')" type="info">ST自动改成FT</el-tag>
              <el-tag v-if="autoRemarks.includes('途径9mm是ACOMODA')" type="info">途径9mm是ACOMODA</el-tag>
              <el-tag v-if="autoRemarks.includes('膜层Backside AR改成背面超防水绿膜')" type="info">膜层Backside AR改成背面超防水绿膜</el-tag>
              <el-tag type="success">需要不干胶标贴</el-tag>
            </div>
          </div>
        </el-card>

        <!-- 价格和订单信息 -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>价格信息</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="单价">
                <el-input v-model="calculatedPrice" readonly>
                  <template #append>元</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="总价">
                <el-input v-model="totalPrice" readonly>
                  <template #append>元</template>
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- 操作按钮 -->
        <div class="form-actions">
          <el-button type="primary" @click="submitOrder" :loading="submitting">
            生成订单
          </el-button>
          <el-button @click="resetForm">重置</el-button>
          <el-button @click="previewOrder">预览订单</el-button>
          <el-button type="success" @click="goToViewOrder">
            <el-icon><View /></el-icon>
            查看历史订单
          </el-button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { View } from '@element-plus/icons-vue'
import { getCurrentInstance } from 'vue'
import { listLensOptions } from '@/api/DFGX/lensOptions'
import { addCustomerOrder, checkOrderNoUnique as checkOrderNoUniqueAPI } from '@/api/DFGX/customerOrders'
import { addOrderDetails } from '@/api/DFGX/orderDetails'
import { recordOrderStatusChange } from '@/api/DFGX/orderStatusHistory'
import { createCompleteOrder } from '@/api/DFGX/orderManagement'

const router = useRouter()
const { proxy } = getCurrentInstance()

// 字典数据
const lensTypeOptions = ref([])
const coatingOptions = ref([])
const designNameOptions = ref([])
const materialOptions = ref([])
const refractiveIndexOptions = ref([])

// 表单数据
const orderForm = reactive({
  // 客户信息
  orderNo: '',
  customerName: '',
  customerPhone: '',
  customerEmail: '',
  orderDate: new Date().toISOString().slice(0, 19).replace('T', ' '),
  status: '0', // 默认待处理状态
  
  // 镜片信息
  lensType: '',
  coating: '',
  designName: '',
  material: '',
  refractiveIndex: '',
  frameType: 'full',
  quantity: 1,
  
  // 左右眼参数
  leftEye: {
    sph: '',
    cyl: '',
    axis: '',
    add: '',
    prism: ''
  },
  rightEye: {
    sph: '',
    cyl: '',
    axis: '',
    add: '',
    prism: ''
  },
  
  // 其他信息
  remarks: '',
  totalAmount: 0
})

// 自动备注
const autoRemarks = ref<string[]>([])

// 订单编号验证器
const validateOrderNo = (rule, value, callback) => {
  if (!value) {
    callback(new Error('请输入订单编号'))
  } else {
    // 这里可以添加更多验证逻辑
    callback()
  }
}

// 表单验证规则
const rules = {
  // 客户信息验证
  orderNo: [
    { required: true, message: '请输入订单编号', trigger: 'blur' },
    { validator: validateOrderNo, trigger: 'blur' }
  ],
  customerName: [{ required: true, message: '请输入客户姓名', trigger: 'blur' }],
  customerPhone: [
    { required: true, message: '请输入客户电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  customerEmail: [
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  orderDate: [{ required: true, message: '请选择订单日期', trigger: 'change' }],
  
  // 镜片信息验证
  lensType: [{ required: true, message: '请选择镜片类型', trigger: 'change' }],
  coating: [{ required: true, message: '请选择膜层', trigger: 'change' }],
  designName: [{ required: true, message: '请选择设计名称', trigger: 'change' }],
  material: [{ required: true, message: '请选择材质', trigger: 'change' }],
  refractiveIndex: [{ required: true, message: '请选择折射率', trigger: 'change' }],
  frameType: [{ required: true, message: '请选择镜框类型', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }]
}

// 计算价格
const calculatedPrice = ref(0)
const totalPrice = computed(() => {
  return calculatedPrice.value * orderForm.quantity
})

// 提交状态
const submitting = ref(false)
const generatingOrderNo = ref(false)
const orderNoValidating = ref(false)

// 获取字典数据
const getDictData = async () => {
  try {
    // 获取所有镜片选项数据
    const response = await listLensOptions({})
    const allOptions = response.data.result || response.data
    
    // 根据选项类型分类数据
    // 1: 镜片类型, 2: 膜层, 3: 材质, 4: 设计名称, 5: 折射率
    lensTypeOptions.value = allOptions
      .filter(item => item.optionType === 1)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    coatingOptions.value = allOptions
      .filter(item => item.optionType === 2)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    materialOptions.value = allOptions
      .filter(item => item.optionType === 3)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    designNameOptions.value = allOptions
      .filter(item => item.optionType === 4)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    refractiveIndexOptions.value = allOptions
      .filter(item => item.optionType === 5)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
  } catch (error) {
    console.error('获取选项数据失败:', error)
    ElMessage.error('获取配置数据失败')
  }
}

// 组件挂载时获取字典数据
onMounted(() => {
  getDictData()
})

// 监听表单变化，自动计算价格和添加备注
watch([() => orderForm.lensType, () => orderForm.coating, () => orderForm.designName, () => orderForm.material, () => orderForm.refractiveIndex], () => {
  calculatePrice()
  generateAutoRemarks()
}, { deep: true })

// 计算价格
const calculatePrice = () => {
  let basePrice = 0
  
  // 从字典数据中获取价格
  const lensTypeOption = lensTypeOptions.value.find(option => option.value === orderForm.lensType)
  const coatingOption = coatingOptions.value.find(option => option.value === orderForm.coating)
  const designNameOption = designNameOptions.value.find(option => option.value === orderForm.designName)
  const materialOption = materialOptions.value.find(option => option.value === orderForm.material)
  const refractiveIndexOption = refractiveIndexOptions.value.find(option => option.value === orderForm.refractiveIndex)
  
  basePrice = (lensTypeOption?.price || 0) + 
              (coatingOption?.price || 0) + 
              (designNameOption?.price || 0) + 
              (materialOption?.price || 0) + 
              (refractiveIndexOption?.price || 0)
  
  calculatedPrice.value = basePrice
}

// 生成自动备注
const generateAutoRemarks = () => {
  const remarks = []
  
  // ST自动改成FT
  if (orderForm.lensType === 'single') {
    remarks.push('ST自动改成FT')
  }
  
  // 设计名称相关备注
  if (orderForm.designName) {
    const designNameOption = designNameOptions.value.find(option => option.value === orderForm.designName)
    if (designNameOption && designNameOption.label.includes('ACOMODA')) {
      remarks.push('设计名称包含ACOMODA')
    }
  }
  
  // 膜层Backside AR改成背面超防水绿膜
  if (orderForm.coating === 'backside-ar') {
    remarks.push('膜层Backside AR改成背面超防水绿膜')
  }
  
  // 所有订单加"需要不干胶标贴"
  remarks.push('需要不干胶标贴')
  
  autoRemarks.value = remarks
}

// 生成订单编号
const generateOrderNo = async () => {
  try {
    generatingOrderNo.value = true
    const now = new Date()
    const timestamp = now.getTime()
    const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0')
    const orderNo = `ORD${now.getFullYear()}${(now.getMonth() + 1).toString().padStart(2, '0')}${now.getDate().toString().padStart(2, '0')}${random}`
    
    // 检查订单编号是否唯一
    const isUnique = await checkOrderNoUniqueAPI(orderNo)
    if (isUnique.data) {
      orderForm.orderNo = orderNo
      ElMessage.success('订单编号生成成功')
    } else {
      ElMessage.warning('生成的订单编号已存在，请重新生成')
    }
  } catch (error) {
    ElMessage.error('生成订单编号失败')
  } finally {
    generatingOrderNo.value = false
  }
}

// 校验订单编号唯一性
const checkOrderNoUnique = async () => {
  if (!orderForm.orderNo) return
  
  try {
    orderNoValidating.value = true
    const response = await checkOrderNoUniqueAPI(orderForm.orderNo)
    if (!response.data) {
      ElMessage.warning('订单编号已存在，请重新输入')
    }
  } catch (error) {
    ElMessage.error('校验订单编号失败')
  } finally {
    orderNoValidating.value = false
  }
}

// 提交订单
const submitOrder = async () => {
  try {
    await orderFormRef.value.validate()
    submitting.value = true
    
    // 构建订单数据
    const orderData = {
      orderNo: orderForm.orderNo,
      customerName: orderForm.customerName,
      customerPhone: orderForm.customerPhone,
      customerEmail: orderForm.customerEmail,
      orderDate: orderForm.orderDate,
      status: orderForm.status,
      totalAmount: totalPrice.value,
      remarks: orderForm.remarks
    }
    
    // 构建订单详情数据
    const orderDetailsData = [{
      lensType: orderForm.lensType,
      lensTypeName: lensTypeOptions.value.find(opt => opt.value === orderForm.lensType)?.label || '',
      coating: orderForm.coating,
      coatingName: coatingOptions.value.find(opt => opt.value === orderForm.coating)?.label || '',
      designName: orderForm.designName,
      designNameText: designNameOptions.value.find(opt => opt.value === orderForm.designName)?.label || '',
      material: orderForm.material,
      materialName: materialOptions.value.find(opt => opt.value === orderForm.material)?.label || '',
      refractiveIndex: orderForm.refractiveIndex,
      refractiveIndexValue: refractiveIndexOptions.value.find(opt => opt.value === orderForm.refractiveIndex)?.label || '',
      frameType: orderForm.frameType,
      quantity: orderForm.quantity,
      unitPrice: calculatedPrice.value,
      totalPrice: totalPrice.value,
      leftEyeSph: orderForm.leftEye.sph,
      leftEyeCyl: orderForm.leftEye.cyl,
      leftEyeAxis: orderForm.leftEye.axis,
      leftEyeAdd: orderForm.leftEye.add,
      leftEyePrism: orderForm.leftEye.prism,
      rightEyeSph: orderForm.rightEye.sph,
      rightEyeCyl: orderForm.rightEye.cyl,
      rightEyeAxis: orderForm.rightEye.axis,
      rightEyeAdd: orderForm.rightEye.add,
      rightEyePrism: orderForm.rightEye.prism,
      autoRemarks: autoRemarks.value.join(', ')
    }]
    
    // 使用综合管理接口创建完整订单
    const completeOrderData = {
      order: orderData,
      orderDetails: orderDetailsData
    }
    
    const response = await createCompleteOrder(completeOrderData)
    
    if (response.code == 200) {
      ElMessage.success('订单创建成功！')
      
      // 记录状态变更
      try {
        // 根据实际API响应结构获取订单ID
        const orderId = response.data.orderId 
        if (orderId) {
          await recordOrderStatusChange(orderId, '0', '待处理', orderForm.customerName)
        }
      } catch (error) {
        console.warn('记录状态变更失败:', error)
      }
      
      // 跳转到查看订单页面
      router.push('/CheckOrder')
    } else {
      ElMessage.error(response.data.message || '订单创建失败')
    }
    
  } catch (error) {
    console.error('提交订单失败:', error)
    ElMessage.error('订单创建失败，请检查表单信息')
  } finally {
    submitting.value = false
  }
}

// 重置表单
const resetForm = () => {
  orderFormRef.value.resetFields()
  calculatedPrice.value = 0
  autoRemarks.value = []
  // 重置订单日期为当前时间
  orderForm.orderDate = new Date().toISOString().slice(0, 19).replace('T', ' ')
}

// 预览订单
const previewOrder = () => {
  const lensTypeLabel = lensTypeOptions.value.find(option => option.value === orderForm.lensType)?.label || orderForm.lensType
  const coatingLabel = coatingOptions.value.find(option => option.value === orderForm.coating)?.label || orderForm.coating
  const designNameLabel = designNameOptions.value.find(option => option.value === orderForm.designName)?.label || orderForm.designName
  const materialLabel = materialOptions.value.find(option => option.value === orderForm.material)?.label || orderForm.material
  const refractiveIndexLabel = refractiveIndexOptions.value.find(option => option.value === orderForm.refractiveIndex)?.label || orderForm.refractiveIndex
  
  ElMessageBox.alert(
    `订单预览：
订单编号：${orderForm.orderNo}
客户姓名：${orderForm.customerName}
客户电话：${orderForm.customerPhone}
客户邮箱：${orderForm.customerEmail}
订单日期：${orderForm.orderDate}

镜片信息：
镜片类型：${lensTypeLabel}
膜层：${coatingLabel}
设计名称：${designNameLabel}
材质：${materialLabel}
折射率：${refractiveIndexLabel}
镜框类型：${orderForm.frameType}
数量：${orderForm.quantity}
单价：${calculatedPrice.value}元
总价：${totalPrice.value}元

备注信息：
用户备注：${orderForm.remarks || '无'}
自动备注：${autoRemarks.value.join(', ')}`,
    '订单预览',
    { type: 'info' }
  )
}

// 跳转到查看订单页面
const goToViewOrder = () => {
  router.push('/CheckOrder')
}

const orderFormRef = ref()
</script>

<style scoped>
.order-container {
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
}

.order-form {
  max-width: 1200px;
  margin: 0 auto;
}

.form-card {
  margin-bottom: 20px;
}

.card-header {
  font-size: 16px;
  font-weight: bold;
  color: #333;
}

.eye-params {
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  padding: 15px;
  background-color: #fafafa;
}

.eye-params h4 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 14px;
  font-weight: bold;
}

.auto-remarks {
  margin-top: 15px;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.auto-remarks h4 {
  margin: 0 0 10px 0;
  color: #666;
  font-size: 14px;
}

.remarks-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.form-actions {
  text-align: center;
  margin-top: 30px;
  padding: 20px;
}

.form-actions .el-button {
  margin: 0 10px;
  min-width: 100px;
}

:deep(.el-form-item__label) {
  font-weight: bold;
  color: #333;
}

:deep(.el-card__header) {
  background-color: #f8f9fa;
  border-bottom: 1px solid #e4e7ed;
}

:deep(.el-input.is-disabled .el-input__inner) {
  background-color: #f5f7fa;
  color: #606266;
}
</style>