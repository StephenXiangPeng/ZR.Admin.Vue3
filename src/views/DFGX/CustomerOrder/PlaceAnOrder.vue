<template>
  <div class="order-container">
    <div class="order-header">
    </div>
    
    <div class="order-form">
      <el-form :model="orderForm" :rules="rules" ref="orderFormRef" label-width="120px">
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
                  <el-option label="单焦点" value="single"></el-option>
                  <el-option label="双焦点" value="bifocal"></el-option>
                  <el-option label="渐进多焦点" value="progressive"></el-option>
                  <el-option label="防蓝光" value="blue-light"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="膜层" prop="coating">
                <el-select v-model="orderForm.coating" placeholder="请选择膜层" style="width: 100%">
                  <el-option label="无膜层" value="none"></el-option>
                  <el-option label="防反射膜" value="ar"></el-option>
                  <el-option label="背面超防水绿膜" value="backside-ar"></el-option>
                  <el-option label="防紫外线膜" value="uv"></el-option>
                  <el-option label="防刮膜" value="scratch-resistant"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="途径" prop="path">
                <el-select v-model="orderForm.path" placeholder="请选择途径" style="width: 100%">
                  <el-option label="6mm" value="6mm"></el-option>
                  <el-option label="7mm" value="7mm"></el-option>
                  <el-option label="8mm" value="8mm"></el-option>
                  <el-option label="9mm (ACOMODA)" value="9mm"></el-option>
                  <el-option label="10mm" value="10mm"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="材质" prop="material">
                <el-select v-model="orderForm.material" placeholder="请选择材质" style="width: 100%">
                  <el-option label="MR7" value="MR7"></el-option>
                  <el-option label="MR8" value="MR8"></el-option>
                  <el-option label="MR174" value="MR174"></el-option>
                  <el-option label="CR39" value="CR39"></el-option>
                  <el-option label="PC" value="PC"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="镜框类型" prop="frameType">
                <el-radio-group v-model="orderForm.frameType">
                  <el-radio label="full">全框</el-radio>
                  <el-radio label="half">半框</el-radio>
                  <el-radio label="none">无框</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
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
import { ref, reactive, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { View } from '@element-plus/icons-vue'

const router = useRouter()

// 表单数据
const orderForm = reactive({
  lensType: '',
  coating: '',
  path: '',
  material: '',
  frameType: 'full',
  quantity: 1,
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
  remarks: ''
})

// 自动备注
const autoRemarks = ref<string[]>([])

// 表单验证规则
const rules = {
  lensType: [{ required: true, message: '请选择镜片类型', trigger: 'change' }],
  coating: [{ required: true, message: '请选择膜层', trigger: 'change' }],
  path: [{ required: true, message: '请选择途径', trigger: 'change' }],
  material: [{ required: true, message: '请选择材质', trigger: 'change' }],
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

// 监听表单变化，自动计算价格和添加备注
watch([() => orderForm.lensType, () => orderForm.coating, () => orderForm.path, () => orderForm.material], () => {
  calculatePrice()
  generateAutoRemarks()
}, { deep: true })

// 计算价格
const calculatePrice = () => {
  let basePrice = 0
  
  // 根据镜片类型计算基础价格
  const lensTypePrices = {
    'single': 100,
    'bifocal': 200,
    'progressive': 300,
    'blue-light': 150
  }
  
  // 根据膜层计算价格
  const coatingPrices = {
    'none': 0,
    'ar': 50,
    'backside-ar': 80,
    'uv': 30,
    'scratch-resistant': 40
  }
  
  // 根据材质计算价格
  const materialPrices = {
    'MR7': 20,
    'MR8': 30,
    'MR174': 40,
    'CR39': 10,
    'PC': 15
  }
  
  basePrice = (lensTypePrices[orderForm.lensType] || 0) + 
              (coatingPrices[orderForm.coating] || 0) + 
              (materialPrices[orderForm.material] || 0)
  
  calculatedPrice.value = basePrice
}

// 生成自动备注
const generateAutoRemarks = () => {
  const remarks = []
  
  // ST自动改成FT
  if (orderForm.lensType === 'single') {
    remarks.push('ST自动改成FT')
  }
  
  // 途径9mm是ACOMODA
  if (orderForm.path === '9mm') {
    remarks.push('途径9mm是ACOMODA')
  }
  
  // 膜层Backside AR改成背面超防水绿膜
  if (orderForm.coating === 'backside-ar') {
    remarks.push('膜层Backside AR改成背面超防水绿膜')
  }
  
  // 所有订单加"需要不干胶标贴"
  remarks.push('需要不干胶标贴')
  
  autoRemarks.value = remarks
}

// 提交订单
const submitOrder = async () => {
  try {
    await orderFormRef.value.validate()
    submitting.value = true
    
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    ElMessage.success('订单生成成功！')
    
    // 这里可以调用实际的API
    console.log('订单数据:', {
      ...orderForm,
      calculatedPrice: calculatedPrice.value,
      totalPrice: totalPrice.value,
      autoRemarks: autoRemarks.value
    })
    
  } catch (error) {
    ElMessage.error('请检查表单信息')
  } finally {
    submitting.value = false
  }
}

// 重置表单
const resetForm = () => {
  orderFormRef.value.resetFields()
  calculatedPrice.value = 0
  autoRemarks.value = []
}

// 预览订单
const previewOrder = () => {
  ElMessageBox.alert(
    `订单预览：
镜片类型：${orderForm.lensType}
膜层：${orderForm.coating}
途径：${orderForm.path}
材质：${orderForm.material}
镜框类型：${orderForm.frameType}
数量：${orderForm.quantity}
单价：${calculatedPrice.value}元
总价：${totalPrice.value}元
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