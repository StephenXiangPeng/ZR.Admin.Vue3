<template>
  <div class="order-container">
    <div class="order-header">
    </div>
    
    <div class="order-form" v-loading="loadingOptions" element-loading-text="正在加载选项数据...">
      <el-form :model="orderForm" :rules="rules" ref="orderFormRef" label-width="140px">
        <!-- Customer Information -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Customer Information</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="Order ID" prop="orderNo">
                <el-input v-model="orderForm.orderNo" placeholder="Please enter Order ID" @blur="checkOrderNoUnique">
                  <template #append>
                    <el-button @click="generateOrderNo" :loading="generatingOrderNo">Generate</el-button>
                  </template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="Customer" prop="customerName">
                <el-input v-model="orderForm.customerName" placeholder="Please enter Customer Name" readonly></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="Date" prop="orderDate">
                <el-input v-model="orderForm.orderDate" readonly></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- Frame Information -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Frame</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="Frame Type" prop="frameType">
                <el-select v-model="orderForm.frameType" placeholder="Please select Frame Type" style="width: 100%">
                  <el-option 
                    v-for="option in frameTypeOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="Model" prop="frameModel">
                <el-select v-model="orderForm.frameModel" placeholder="Please select Model" style="width: 100%">
                  <el-option 
                    v-for="option in frameModelOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="ED/EDA" prop="edEda">
                <el-select v-model="orderForm.edEda" placeholder="Please select ED/EDA" style="width: 100%">
                  <el-option 
                    v-for="option in edEdaOptions" 
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
              <el-form-item label="sizeA-DBL-sizeA" prop="sizeA">
                <el-select v-model="orderForm.sizeA" placeholder="Please select sizeA-DBL-sizeA" style="width: 100%">
                  <el-option 
                    v-for="option in sizeAOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="sizeB" prop="sizeB">
                <el-select v-model="orderForm.sizeB" placeholder="Please select sizeB" style="width: 100%">
                  <el-option 
                    v-for="option in sizeBOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- Lens Information -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Lens Information</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="Coating" prop="coating">
                <el-select v-model="orderForm.coating" placeholder="Please select Coating" style="width: 100%">
                  <el-option 
                    v-for="option in coatingOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="Tinting" prop="tinting">
                <el-select v-model="orderForm.tinting" placeholder="Please select Tinting" style="width: 100%">
                  <el-option 
                    v-for="option in tintingOptions" 
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
              <el-form-item label="Design" prop="designName">
                <el-select v-model="orderForm.designName" placeholder="Please select Design" style="width: 100%">
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
              <el-form-item label="Material" prop="material">
                <el-select v-model="orderForm.material" placeholder="Please select Material" style="width: 100%">
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
              <el-form-item label="Index" prop="refractiveIndex">
                <el-select v-model="orderForm.refractiveIndex" placeholder="Please select Index" style="width: 100%">
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
              <el-form-item label="Corridor" prop="channel">
                <el-select v-model="orderForm.channel" placeholder="Please select Corridor" style="width: 100%">
                  <el-option 
                    v-for="option in channelOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
        </el-card>

        <!-- Eye Parameters -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Eye Parameters</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <div class="eye-params">
                <h4>OD/Right Eye</h4>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="SPH" prop="rightEye.sph">
                      <el-input v-model="orderForm.rightEye.sph" placeholder="Sphere"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="CYL" prop="rightEye.cyl">
                      <el-input v-model="orderForm.rightEye.cyl" placeholder="Cylinder"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Axis" prop="rightEye.axis">
                      <el-input v-model="orderForm.rightEye.axis" placeholder="Axis"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="ADD" prop="rightEye.add">
                      <el-input v-model="orderForm.rightEye.add" placeholder="Addition"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Prism" prop="rightEye.prism">
                      <el-input v-model="orderForm.rightEye.prism" placeholder="Prism"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Direction" prop="rightEye.prismDirection">
                      <el-input v-model="orderForm.rightEye.prismDirection" placeholder="Direction"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Base" prop="rightEye.base">
                      <el-input v-model="orderForm.rightEye.base" placeholder="Base"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="PH" prop="rightEye.ph">
                      <el-input v-model="orderForm.rightEye.ph" placeholder="PH"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Near PD" prop="rightEye.nearPd">
                      <el-input v-model="orderForm.rightEye.nearPd" placeholder="Near PD"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Distance PD" prop="rightEye.distancePd">
                      <el-input v-model="orderForm.rightEye.distancePd" placeholder="Distance PD"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Quantity" prop="rightEye.quantity">
                      <el-input-number v-model="orderForm.rightEye.quantity" :min="1" :max="100" style="width: 100%"></el-input-number>
                    </el-form-item>
                  </el-col>
                </el-row>
              </div>
            </el-col>
            <el-col :span="12">
              <div class="eye-params">
                <h4>OS/Left Eye</h4>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="SPH" prop="leftEye.sph">
                      <el-input v-model="orderForm.leftEye.sph" placeholder="Sphere"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="CYL" prop="leftEye.cyl">
                      <el-input v-model="orderForm.leftEye.cyl" placeholder="Cylinder"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Axis" prop="leftEye.axis">
                      <el-input v-model="orderForm.leftEye.axis" placeholder="Axis"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="ADD" prop="leftEye.add">
                      <el-input v-model="orderForm.leftEye.add" placeholder="Addition"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Prism" prop="leftEye.prism">
                      <el-input v-model="orderForm.leftEye.prism" placeholder="Prism"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Direction" prop="leftEye.prismDirection">
                      <el-input v-model="orderForm.leftEye.prismDirection" placeholder="Direction"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Base" prop="leftEye.base">
                      <el-input v-model="orderForm.leftEye.base" placeholder="Base"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="PH" prop="leftEye.ph">
                      <el-input v-model="orderForm.leftEye.ph" placeholder="PH"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Near PD" prop="leftEye.nearPd">
                      <el-input v-model="orderForm.leftEye.nearPd" placeholder="Near PD"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Distance PD" prop="leftEye.distancePd">
                      <el-input v-model="orderForm.leftEye.distancePd" placeholder="Distance PD"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="15">
                  <el-col :span="12">
                    <el-form-item label="Quantity" prop="leftEye.quantity">
                      <el-input-number v-model="orderForm.leftEye.quantity" :min="1" :max="100" style="width: 100%"></el-input-number>
                    </el-form-item>
                  </el-col>
                </el-row>
              </div>
            </el-col>
          </el-row>
        </el-card>

        <!-- Remarks Information -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Remarks</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="Remarks Type" prop="remarksType">
                <el-select 
                  v-model="orderForm.remarksType" 
                  placeholder="Please select Remarks Type" 
                  style="width: 100%"
                  multiple
                  collapse-tags
                  collapse-tags-tooltip>
                  <el-option 
                    v-for="option in remarksTypeOptions" 
                    :key="option.value" 
                    :label="option.label" 
                    :value="option.value">
                  </el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="Remarks" prop="remarks">
                <el-input 
                  v-model="orderForm.remarks" 
                  placeholder="Please enter Remarks"
                  show-word-limit
                  maxlength="200">
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>
          
          <div class="auto-remarks">
            <h4>Remarks Summary:</h4>
            <div class="remarks-list">
              <!-- 选择的备注类型 -->
              <el-tag v-if="orderForm.remarksType && orderForm.remarksType.length > 0" type="primary">
                备注类型: {{ getRemarksTypeLabel(orderForm.remarksType) }}
              </el-tag>
              <!-- 用户填写的备注内容 -->
              <el-tag v-if="orderForm.remarks" type="info">
                备注内容: {{ orderForm.remarks }}
              </el-tag>
              <!-- 自动生成的备注 -->
              <el-tag v-for="remark in autoRemarks" :key="remark" type="success">
                {{ remark }}
              </el-tag>
            </div>
          </div>
        </el-card>

        <!-- Price Information -->
        <el-card class="form-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <span>Price Information</span>
            </div>
          </template>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="Price/pcs">
                <el-input v-model="calculatedPrice" readonly>
                  <template #append>USD$</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="Total Price">
                <el-input v-model="totalPrice" readonly>
                  <template #append>USD$</template>
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </el-card>

        <!-- 操作按钮 -->
        <div class="form-actions">
          <el-button type="primary" @click="submitOrder" :loading="submitting">
            Generate Order
          </el-button>
          <el-button @click="resetForm">Reset</el-button>
          <el-button @click="previewOrder">Preview Order</el-button>
          <el-button type="success" @click="goToViewOrder">
            <el-icon><View /></el-icon>
            View History Orders
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
import { getAllLensOptions } from '@/api/DFGX/lensOptions'
import { addCustomerOrder, checkOrderNoUnique as checkOrderNoUniqueAPI } from '@/api/DFGX/customerOrders'
import { addOrderDetails } from '@/api/DFGX/orderDetails'
import { recordOrderStatusChange } from '@/api/DFGX/orderStatusHistory'
import { createCompleteOrder } from '@/api/DFGX/orderManagement'
import useUserStore from '@/store/modules/user'

const router = useRouter()
const { proxy } = getCurrentInstance()
const userStore = useUserStore()

// 字典数据
const coatingOptions = ref([])
const tintingOptions = ref([])
const designNameOptions = ref([])
const materialOptions = ref([])
const refractiveIndexOptions = ref([])
const frameTypeOptions = ref([])
const frameModelOptions = ref([])
const channelOptions = ref([])
const edEdaOptions = ref([])
const sizeAOptions = ref([])
const sizeBOptions = ref([])
const remarksTypeOptions = ref([])

// 表单数据
const orderForm = reactive({
  // 客户信息
  orderNo: '',
  customerName: '',
  orderDate: new Date().toISOString().slice(0, 10), // 自动设置为当前日期
  status: '0', // 默认待处理状态
  
  // 镜框信息
  frameType: '',
  frameModel: '',
  edEda: '',
  sizeA: '',
  sizeB: '',
  
  // 镜片信息
  coating: '',
  tinting: '',
  designName: '',
  material: '',
  refractiveIndex: '',
  channel: '',
  
  // 左右眼参数
  leftEye: {
    sph: '',
    cyl: '',
    axis: '',
    add: '',
    prism: '',
    prismDirection: '',
    base: '',
    ph: '',
    nearPd: '',
    distancePd: '',
    quantity: 1
  },
  rightEye: {
    sph: '',
    cyl: '',
    axis: '',
    add: '',
    prism: '',
    prismDirection: '',
    base: '',
    ph: '',
    nearPd: '',
    distancePd: '',
    quantity: 1
  },
  
  // 其他信息
  remarksType: [],
  remarks: '',
  totalAmount: 0
})

// 自动备注
const autoRemarks = ref<string[]>([])

// Order ID validator
const validateOrderNo = (rule, value, callback) => {
  if (!value) {
    callback(new Error('Please enter Order ID'))
  } else {
    // Add more validation logic here
    callback()
  }
}

// 表单验证规则
const rules = {
  // Customer information validation
  orderNo: [
    { required: true, message: 'Please enter Order ID', trigger: 'blur' },
    { validator: validateOrderNo, trigger: 'blur' }
  ],
  customerName: [{ required: true, message: 'Customer Name is required', trigger: 'blur' }],
  orderDate: [{ required: true, message: 'Please generate Date', trigger: 'blur' }],
  
  // Lens information validation
  coating: [{ required: true, message: 'Please select Coating', trigger: 'change' }],
  tinting: [{ required: true, message: 'Please select Tinting', trigger: 'change' }],
  designName: [{ required: true, message: 'Please select Design', trigger: 'change' }],
  material: [{ required: true, message: 'Please select Material', trigger: 'change' }],
  refractiveIndex: [{ required: true, message: 'Please select Index', trigger: 'change' }],
  frameType: [{ required: true, message: 'Please select Frame Type', trigger: 'change' }]
}

// 计算价格
const calculatedPrice = ref(0)
const totalPrice = computed(() => {
  return calculatedPrice.value * (orderForm.leftEye.quantity + orderForm.rightEye.quantity)
})

// 提交状态
const submitting = ref(false)
const generatingOrderNo = ref(false)
const orderNoValidating = ref(false)
const loadingOptions = ref(false)

// 获取字典数据
const getDictData = async () => {
  try {
    loadingOptions.value = true
    console.log('开始获取所有选项数据...')
    // 获取所有选项数据
    const response = await getAllLensOptions()
    console.log('API响应:', response)
    
    const allOptions = response.data.result || response.data
    console.log('所有选项数据:', allOptions)
    
    if (!allOptions || !Array.isArray(allOptions)) {
      console.error('获取到的数据格式不正确:', allOptions)
      ElMessage.error('获取选项数据失败，数据格式不正确')
      return
    }
    
    // 根据optionType查找对应的选项数据
    // optionType映射: 2:膜层, 3:材质, 4:设计名称, 5:折射率, 6:通道, 7:frame type, 8:model, 13:染色
    coatingOptions.value = allOptions
      .filter(item => item.optionType === 2)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    tintingOptions.value = allOptions
      .filter(item => item.optionType === 13)
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
    
    // 折射率选项从字典获取
    refractiveIndexOptions.value = allOptions
      .filter(item => item.optionType === 5)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    channelOptions.value = allOptions
      .filter(item => item.optionType === 6)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    frameTypeOptions.value = allOptions
      .filter(item => item.optionType === 7)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    frameModelOptions.value = allOptions
      .filter(item => item.optionType === 8)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    // 根据数据结构，ED/EDA、sizeA-DBL-sizeA、sizeB 需要根据实际数据确定optionType
    // 暂时设置为空，需要根据实际数据调整
    edEdaOptions.value = []
    sizeAOptions.value = []
    sizeBOptions.value = []
    
    // 备注类型选项 - optionType为12
    console.log('所有选项数据:', allOptions)
    console.log('optionType为12的数据:', allOptions.filter(item => item.optionType === 12))
    
    remarksTypeOptions.value = allOptions
      .filter(item => item.optionType === 12)
      .map(item => ({
        label: item.optionName,
        value: item.optionValue,
        price: item.remark ? parseFloat(item.remark) : 0
      }))
    
    console.log('备注类型选项映射结果:', remarksTypeOptions.value)
    
    // 输出各选项的映射结果用于调试
    console.log('膜层选项:', coatingOptions.value)
    console.log('染色选项:', tintingOptions.value)
    console.log('材质选项:', materialOptions.value)
    console.log('设计名称选项:', designNameOptions.value)
    console.log('折射率选项:', refractiveIndexOptions.value)
    console.log('通道选项:', channelOptions.value)
    console.log('镜框类型选项:', frameTypeOptions.value)
    console.log('镜框型号选项:', frameModelOptions.value)
    console.log('ED/EDA选项:', edEdaOptions.value)
    console.log('sizeA选项:', sizeAOptions.value)
    console.log('sizeB选项:', sizeBOptions.value)
    console.log('备注类型选项:', remarksTypeOptions.value)
    
    ElMessage.success('选项数据加载成功')
  } catch (error) {
    console.error('获取选项数据失败:', error)
    ElMessage.error('Failed to get configuration data')
  } finally {
    loadingOptions.value = false
  }
}

// 组件挂载时获取字典数据和设置客户名称
onMounted(() => {
  getDictData()
  // 设置客户名称为当前登录用户的名称
  if (userStore.name) {
    orderForm.customerName = userStore.name
  } else if (userStore.userName) {
    orderForm.customerName = userStore.userName
  } else {
    // 如果无法获取用户信息，设置默认值
    orderForm.customerName = 'Current User'
  }
})

// 监听表单变化，自动计算价格和添加备注
watch([() => orderForm.coating, () => orderForm.tinting, () => orderForm.designName, () => orderForm.material, () => orderForm.refractiveIndex], () => {
  calculatePrice()
  generateAutoRemarks()
}, { deep: true })

// 计算价格
const calculatePrice = () => {
  let basePrice = 0
  
  // 从字典数据中获取价格
  const coatingOption = coatingOptions.value.find(option => option.value === orderForm.coating)
  const tintingOption = tintingOptions.value.find(option => option.value === orderForm.tinting)
  const designNameOption = designNameOptions.value.find(option => option.value === orderForm.designName)
  const materialOption = materialOptions.value.find(option => option.value === orderForm.material)
  const refractiveIndexOption = refractiveIndexOptions.value.find(option => option.value === orderForm.refractiveIndex)
  
  basePrice = (coatingOption?.price || 0) + 
              (tintingOption?.price || 0) + 
              (designNameOption?.price || 0) + 
              (materialOption?.price || 0) + 
              (refractiveIndexOption?.price || 0)
  
  calculatedPrice.value = basePrice
}

// 获取备注类型标签
const getRemarksTypeLabel = (values) => {
  if (!values || values.length === 0) return ''
  if (Array.isArray(values)) {
    return values.map(value => {
      const option = remarksTypeOptions.value.find(option => option.value === value)
      return option ? option.label : value
    }).join(', ')
  }
  const option = remarksTypeOptions.value.find(option => option.value === values)
  return option ? option.label : values
}

// 生成自动备注
const generateAutoRemarks = () => {
  const remarks = []
  
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
  
  // 染色选项相关备注
  if (orderForm.tinting) {
    const tintingOption = tintingOptions.value.find(option => option.value === orderForm.tinting)
    if (tintingOption && tintingOption.label) {
      remarks.push(`染色选项: ${tintingOption.label}`)
    }
  }
  
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
      ElMessage.success('Order ID generated successfully')
    } else {
      ElMessage.warning('Generated Order ID already exists, please regenerate')
    }
  } catch (error) {
    ElMessage.error('Failed to generate Order ID')
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
      ElMessage.warning('Order ID already exists, please re-enter')
    }
  } catch (error) {
    ElMessage.error('Failed to validate Order ID')
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
    const orderDateValue = orderForm.orderDate ? 
      new Date(orderForm.orderDate).toISOString() : 
      new Date().toISOString()
    
    const orderData = {
      orderNo: orderForm.orderNo,
      customerName: orderForm.customerName,
      // 使用后端期望的字段名
      Order_date: orderDateValue,
      status: orderForm.status,
      totalAmount: totalPrice.value,
      remarks: orderForm.remarks,
      // 镜框信息
      frameType: orderForm.frameType,
      frameModel: orderForm.frameModel,
      edEda: orderForm.edEda,
      sizeA: orderForm.sizeA,
      sizeB: orderForm.sizeB,
      // 新增：左右眼数量
      rightEyeQuantity: orderForm.rightEye.quantity,
      leftEyeQuantity: orderForm.leftEye.quantity,
      // 右眼参数
      rightEyeSph: orderForm.rightEye.sph,
      rightEyeCyl: orderForm.rightEye.cyl,
      rightEyeAxis: orderForm.rightEye.axis,
      rightEyeAdd: orderForm.rightEye.add,
      rightEyePrism: orderForm.rightEye.prism,
      rightEyePrismDirection: orderForm.rightEye.prismDirection,
      rightEyeBase: orderForm.rightEye.base,
      rightEyePh: orderForm.rightEye.ph,
      rightEyeNearPd: orderForm.rightEye.nearPd,
      rightEyeDistancePd: orderForm.rightEye.distancePd,
      // 左眼参数
      leftEyeSph: orderForm.leftEye.sph,
      leftEyeCyl: orderForm.leftEye.cyl,
      leftEyeAxis: orderForm.leftEye.axis,
      leftEyeAdd: orderForm.leftEye.add,
      leftEyePrism: orderForm.leftEye.prism,
      leftEyePrismDirection: orderForm.leftEye.prismDirection,
      leftEyeBase: orderForm.leftEye.base,
      leftEyePh: orderForm.leftEye.ph,
      leftEyeNearPd: orderForm.leftEye.nearPd,
      leftEyeDistancePd: orderForm.leftEye.distancePd,
      // 备注信息
      remarksType: Array.isArray(orderForm.remarksType) ? orderForm.remarksType.join(',') : orderForm.remarksType,
      autoRemarks: autoRemarks.value.join(', ')
    }
    
    // 构建订单详情数据
    const orderDetailsData = [{
      coating: orderForm.coating,
      coatingName: coatingOptions.value.find(opt => opt.value === orderForm.coating)?.label || '',
      tinting: orderForm.tinting,
      tintingName: tintingOptions.value.find(opt => opt.value === orderForm.tinting)?.label || '',
      designName: orderForm.designName,
      designNameText: designNameOptions.value.find(opt => opt.value === orderForm.designName)?.label || '',
      material: orderForm.material,
      materialName: materialOptions.value.find(opt => opt.value === orderForm.material)?.label || '',
      refractiveIndex: orderForm.refractiveIndex,
      refractiveIndexValue: refractiveIndexOptions.value.find(opt => opt.value === orderForm.refractiveIndex)?.label || '',
      frameType: orderForm.frameType,
      frameModel: orderForm.frameModel,
      edEda: orderForm.edEda,
      sizeA: orderForm.sizeA,
      sizeB: orderForm.sizeB,
      // 修改：数量字段调整
      leftEyeQuantity: orderForm.leftEye.quantity,
      rightEyeQuantity: orderForm.rightEye.quantity,
      quantity: orderForm.leftEye.quantity + orderForm.rightEye.quantity, // 总数量
      unitPrice: calculatedPrice.value,
      totalPrice: totalPrice.value,
      // 右眼参数
      rightEyeSph: orderForm.rightEye.sph,
      rightEyeCyl: orderForm.rightEye.cyl,
      rightEyeAxis: orderForm.rightEye.axis,
      rightEyeAdd: orderForm.rightEye.add,
      rightEyePrism: orderForm.rightEye.prism,
      rightEyePrismDirection: orderForm.rightEye.prismDirection,
      rightEyeBase: orderForm.rightEye.base,
      rightEyePh: orderForm.rightEye.ph,
      rightEyeNearPd: orderForm.rightEye.nearPd,
      rightEyeDistancePd: orderForm.rightEye.distancePd,
      // 左眼参数
      leftEyeSph: orderForm.leftEye.sph,
      leftEyeCyl: orderForm.leftEye.cyl,
      leftEyeAxis: orderForm.leftEye.axis,
      leftEyeAdd: orderForm.leftEye.add,
      leftEyePrism: orderForm.leftEye.prism,
      leftEyePrismDirection: orderForm.leftEye.prismDirection,
      leftEyeBase: orderForm.leftEye.base,
      leftEyePh: orderForm.leftEye.ph,
      leftEyeNearPd: orderForm.leftEye.nearPd,
      leftEyeDistancePd: orderForm.leftEye.distancePd,
      // 备注信息
      remarksType: Array.isArray(orderForm.remarksType) ? orderForm.remarksType.join(',') : orderForm.remarksType,
      autoRemarks: autoRemarks.value.join(', ')
    }]
    
    // 使用综合管理接口创建完整订单
    const completeOrderData = {
      Order: orderData,
      OrderDetails: orderDetailsData
    }
    
    const response = await createCompleteOrder(completeOrderData)
    
    if (response.data && response.code == 200) {
      const payload = response.data.data || {}
      const orderId = payload.OrderId || response.data.orderId
      
      ElMessage.success(payload.Message || 'Order created successfully!')
      
      // 记录状态变更
      try {
        if (orderId) {
          await recordOrderStatusChange(orderId, '0', '待处理', orderForm.customerName)
        }
      } catch (error) {
        console.warn('记录状态变更失败:', error)
      }
      
      // 跳转到查看订单页面
      router.push('/CheckOrder')
    } else {
      const errMsg = (response.data && (response.data.message || response.data.msg)) || 'Order creation failed'
      ElMessage.error(errMsg)
    }
    
  } catch (error) {
    console.error('提交订单失败:', error)
    ElMessage.error('Order creation failed, please check form information')
  } finally {
    submitting.value = false
  }
}

// 重置表单
const resetForm = () => {
  orderFormRef.value.resetFields()
  calculatedPrice.value = 0
  autoRemarks.value = []
  // 重置订单日期为当前日期
  orderForm.orderDate = new Date().toISOString().slice(0, 10)
}

// 预览订单
const previewOrder = () => {
  const coatingLabel = coatingOptions.value.find(option => option.value === orderForm.coating)?.label || orderForm.coating
  const tintingLabel = tintingOptions.value.find(option => option.value === orderForm.tinting)?.label || orderForm.tinting
  const designNameLabel = designNameOptions.value.find(option => option.value === orderForm.designName)?.label || orderForm.designName
  const materialLabel = materialOptions.value.find(option => option.value === orderForm.material)?.label || orderForm.material
  const refractiveIndexLabel = refractiveIndexOptions.value.find(option => option.value === orderForm.refractiveIndex)?.label || orderForm.refractiveIndex
  
  ElMessageBox.alert(
    `Order Preview:
Order ID: ${orderForm.orderNo}
Customer Name: ${orderForm.customerName}
Date: ${orderForm.orderDate}

Lens Information:
Coating: ${coatingLabel}
Tinting: ${tintingLabel}
Design: ${designNameLabel}
Material: ${materialLabel}
Index: ${refractiveIndexLabel}
Frame Type: ${orderForm.frameType}
Left Eye Qty: ${orderForm.leftEye.quantity}
Right Eye Qty: ${orderForm.rightEye.quantity}
Price/pcs: ${calculatedPrice.value}USD$
Total Price: ${totalPrice.value}USD$

Remarks:
User Remarks: ${orderForm.remarks || 'None'}
Auto Remarks: ${autoRemarks.value.join(', ')}`,
    'Order Preview',
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