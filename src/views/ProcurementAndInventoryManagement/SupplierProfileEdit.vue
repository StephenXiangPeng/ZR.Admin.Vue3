<template>
  <div class="app-container">
    <!-- 页面头部 -->
    <div class="page-header">
      <h1 class="page-title">新增供应商</h1>
      <div class="header-actions">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSave" v-if="mode !== 'view'">保存</el-button>
      </div>
    </div>

    <!-- 基础信息 -->
    <el-card class="info-card" shadow="never">
      <template #header>
        <div class="card-header" @click="toggleCollapse('basicInfo')">
          <span class="card-title">基础信息</span>
          <el-icon class="collapse-icon" :class="{ 'collapsed': collapseState.basicInfo }">
            <ArrowUp />
          </el-icon>
        </div>
      </template>
      
      <div v-show="!collapseState.basicInfo" class="card-content">
        <el-form :model="form" label-width="120px" class="supplier-form">
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="*供应商编号" required>
                <el-input v-model="form.supplierNumber" placeholder="请输入" :disabled="form.useSystemNumber">
                  <template #append>
                    <el-checkbox v-model="form.useSystemNumber">使用系统编号</el-checkbox>
                  </template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="*供应商名称" required>
                <el-input v-model="form.supplierName" placeholder="请输入" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="产品分类">
                <el-select v-model="form.productCategory" placeholder="请选择" style="width: 100%">
                  <el-option label="分类1" value="1" />
                  <el-option label="分类2" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="物料分类">
                <el-select v-model="form.materialCategory" placeholder="请选择" style="width: 100%">
                  <el-option label="物料分类1" value="1" />
                  <el-option label="物料分类2" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="默认采购税率">
                <el-input v-model="form.taxRate" placeholder="请输入">
                  <template #append>%</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="结算期限">
                <el-date-picker v-model="form.settlementPeriod" type="date" placeholder="请选择" style="width: 100%" />
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="8">
              <el-form-item label="部门">
                <el-select v-model="form.department" placeholder="请选择" style="width: 100%">
                  <el-option label="采购部" value="1" />
                  <el-option label="财务部" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="采购员">
                <el-select v-model="form.purchaser" placeholder="请选择" style="width: 100%">
                  <el-option label="张三" value="1" />
                  <el-option label="李四" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="附件">
                <el-button type="text">上传文件</el-button>
                <el-icon class="help-icon"><QuestionFilled /></el-icon>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="备注">
                <el-input 
                  v-model="form.remarks" 
                  type="textarea" 
                  :rows="3" 
                  placeholder="请输入"
                  maxlength="100"
                  show-word-limit
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="微信">
                <el-input v-model="form.wechat" placeholder="请输入" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
      </div>
    </el-card>

    <!-- 财务信息 -->
    <el-card class="info-card" shadow="never">
      <template #header>
        <div class="card-header" @click="toggleCollapse('financialInfo')">
          <span class="card-title">财务信息</span>
          <div class="card-actions">
            <el-button type="primary" size="small" @click.stop="addBankInfo">新增银行信息</el-button>
            <el-button size="small" @click.stop="deleteBankInfo">删除银行信息</el-button>
            <el-icon class="collapse-icon" :class="{ 'collapsed': collapseState.financialInfo }">
              <ArrowUp />
            </el-icon>
          </div>
        </div>
      </template>
      
      <div v-show="!collapseState.financialInfo" class="card-content">
        <div class="bank-info-list">
          <div v-for="(bankInfo, index) in bankInfoList" :key="bankInfo.id" class="bank-info-item">
          <el-form :model="bankInfo" label-width="120px" class="supplier-form">
            <el-row :gutter="20">
              <el-col :span="6">
                <el-form-item label="税号">
                  <el-input v-model="bankInfo.taxId" placeholder="请输入" maxlength="100" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="开户名称">
                  <el-input v-model="bankInfo.accountName" placeholder="请输入" />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="开户银行">
                  <el-input v-model="bankInfo.bankName" placeholder="请输入" maxlength="100" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="银行账号">
                  <el-input v-model="bankInfo.bankAccount" placeholder="请输入" maxlength="100" show-word-limit>
                    <template #append>
                      <el-checkbox v-model="bankInfo.isDefault">设为默认</el-checkbox>
                    </template>
                  </el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <div class="item-actions" v-if="bankInfoList.length > 1">
            <el-button type="danger" size="small" @click="removeBankInfo(index)">删除</el-button>
          </div>
        </div>
        </div>
      </div>
    </el-card>

    <!-- 联系信息 -->
    <el-card class="info-card" shadow="never">
      <template #header>
        <div class="card-header" @click="toggleCollapse('contactInfo')">
          <span class="card-title">联系信息</span>
          <div class="card-actions">
            <el-button type="primary" size="small" @click.stop="addContactInfo">新增联系信息</el-button>
            <el-button size="small" @click.stop="deleteContactInfo">删除联系信息</el-button>
            <el-icon class="collapse-icon" :class="{ 'collapsed': collapseState.contactInfo }">
              <ArrowUp />
            </el-icon>
          </div>
        </div>
      </template>
      
      <div v-show="!collapseState.contactInfo" class="card-content">
        <div class="contact-info-list">
          <div v-for="(contactInfo, index) in contactInfoList" :key="contactInfo.id" class="contact-info-item">
          <el-form :model="contactInfo" label-width="120px" class="supplier-form">
            <el-row :gutter="20">
              <el-col :span="8">
                <el-form-item label="联系人">
                  <el-input v-model="contactInfo.contactPerson" placeholder="请输入" maxlength="50" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="联系方式">
                  <el-input v-model="contactInfo.contactMethod" placeholder="请输入" maxlength="30" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="联系地址">
                  <el-input v-model="contactInfo.contactAddress" placeholder="请输入" maxlength="100" show-word-limit>
                    <template #append>
                      <el-checkbox v-model="contactInfo.isDefault">设为默认</el-checkbox>
                    </template>
                  </el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <div class="item-actions" v-if="contactInfoList.length > 1">
            <el-button type="danger" size="small" @click="removeContactInfo(index)">删除</el-button>
          </div>
        </div>
        </div>
      </div>
    </el-card>

    <!-- 图片 -->
    <el-card class="info-card" shadow="never">
      <template #header>
        <div class="card-header" @click="toggleCollapse('images')">
          <span class="card-title">图片</span>
          <el-icon class="collapse-icon" :class="{ 'collapsed': collapseState.images }">
            <ArrowUp />
          </el-icon>
        </div>
      </template>
      
      <div v-show="!collapseState.images" class="card-content">
        <div class="image-upload-section">
          <p class="upload-tip">最多上传6张图片,支持JPG,PNG,BMP格式</p>
        <el-upload
          class="simple-upload"
          action="#"
          multiple
          :limit="6"
          :file-list="fileList"
          :on-change="handleFileChange"
          :show-file-list="false"
        >
          <div class="upload-area">
            <div class="upload-icon">+</div>
          </div>
        </el-upload>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { ArrowUp, QuestionFilled, UploadFilled } from '@element-plus/icons-vue'

const route = useRoute()
const router = useRouter()

// 获取路由参数
const supplierId = ref(route.query.id)
const mode = ref(route.query.mode || 'add')

// 文件列表
const fileList = ref([])

// 折叠状态
const collapseState = ref({
  basicInfo: false,    // 基础信息
  financialInfo: false, // 财务信息
  contactInfo: false,  // 联系信息
  images: false       // 图片
})

// 银行信息列表
const bankInfoList = ref([
  {
    id: 1,
    taxId: '',
    accountName: '',
    bankName: '',
    bankAccount: '',
    isDefault: false
  }
])

// 联系信息列表
const contactInfoList = ref([
  {
    id: 1,
    contactPerson: '',
    contactMethod: '',
    contactAddress: '',
    isDefault: false
  }
])

// 表单数据
const form = reactive({
  // 基础信息
  supplierNumber: '',
  supplierName: '',
  useSystemNumber: false,
  productCategory: '',
  materialCategory: '',
  taxRate: '',
  settlementPeriod: '',
  department: '',
  purchaser: '',
  remarks: '',
  wechat: ''
})

// 页面操作
function handleSave() {
  console.log('保存表单:', form)
  ElMessage.success('保存成功')
  handleCancel()
}


function handleCancel() {
  router.back()
}

// 重置表单
function resetForm() {
  Object.keys(form).forEach(key => {
    if (typeof form[key] === 'boolean') {
      form[key] = false
    } else {
      form[key] = ''
    }
  })
}

// 切换折叠状态
function toggleCollapse(section: string) {
  collapseState.value[section] = !collapseState.value[section]
}

// 银行信息操作
function addBankInfo() {
  const newId = Math.max(...bankInfoList.value.map(item => item.id)) + 1
  bankInfoList.value.push({
    id: newId,
    taxId: '',
    accountName: '',
    bankName: '',
    bankAccount: '',
    isDefault: false
  })
  ElMessage.success('已添加新的银行信息')
}

function removeBankInfo(index: number) {
  if (bankInfoList.value.length > 1) {
    bankInfoList.value.splice(index, 1)
    ElMessage.success('已删除银行信息')
  } else {
    ElMessage.warning('至少保留一条银行信息')
  }
}

function deleteBankInfo() {
  if (bankInfoList.value.length > 1) {
    bankInfoList.value.pop()
    ElMessage.success('已删除最后一条银行信息')
  } else {
    ElMessage.warning('至少保留一条银行信息')
  }
}

// 联系信息操作
function addContactInfo() {
  const newId = Math.max(...contactInfoList.value.map(item => item.id)) + 1
  contactInfoList.value.push({
    id: newId,
    contactPerson: '',
    contactMethod: '',
    contactAddress: '',
    isDefault: false
  })
  ElMessage.success('已添加新的联系信息')
}

function removeContactInfo(index: number) {
  if (contactInfoList.value.length > 1) {
    contactInfoList.value.splice(index, 1)
    ElMessage.success('已删除联系信息')
  } else {
    ElMessage.warning('至少保留一条联系信息')
  }
}

function deleteContactInfo() {
  if (contactInfoList.value.length > 1) {
    contactInfoList.value.pop()
    ElMessage.success('已删除最后一条联系信息')
  } else {
    ElMessage.warning('至少保留一条联系信息')
  }
}

// 文件上传处理
function handleFileChange(file: any, fileList: any[]) {
  console.log('文件变化:', file, fileList)
}

// 页面加载时获取数据
onMounted(() => {
  if (supplierId.value && mode.value === 'edit') {
    // 这里应该调用API获取供应商详情
    console.log('获取供应商详情:', supplierId.value)
    // 模拟数据
    form.supplierNumber = 'SUP001'
    form.supplierName = '示例供应商'
    // 设置联系信息
    contactInfoList.value[0].contactPerson = '张三'
    contactInfoList.value[0].contactMethod = '13800138000'
  }
})
</script>

<style scoped>
.app-container {
  padding: 20px;
  background-color: #f5f5f5;
  min-height: 100vh;
}

/* 页面头部 */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.page-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #303133;
}

.header-actions {
  display: flex;
  gap: 12px;
}

/* 信息卡片 */
.info-card {
  margin-bottom: 20px;
  border-radius: 8px;
  border: 1px solid #e4e7ed;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 600;
  color: #303133;
  cursor: pointer;
  user-select: none;
  transition: all 0.3s ease;
}

.card-header:hover {
  background-color: #f5f7fa;
}

.card-title {
  font-size: 16px;
}

.collapse-icon {
  cursor: pointer;
  color: #909399;
  transition: transform 0.3s ease;
}

.collapse-icon.collapsed {
  transform: rotate(180deg);
}

.card-content {
  transition: all 0.3s ease;
  overflow: hidden;
}

.card-actions {
  display: flex;
  gap: 8px;
}

/* 表单样式 */
.supplier-form {
  padding: 20px 0;
}

.supplier-form .el-form-item {
  margin-bottom: 20px;
}

.supplier-form .el-form-item__label {
  font-weight: 500;
  color: #606266;
}

.help-icon {
  margin-left: 8px;
  color: #909399;
  cursor: pointer;
}

/* 图片上传区域 */
.image-upload-section {
  padding: 20px 0;
}

.upload-tip {
  margin: 0 0 20px 0;
  color: #909399;
  font-size: 14px;
}

/* 简洁上传样式 */
.simple-upload {
  width: 100%;
}

.upload-area {
  width: 120px;
  height: 120px;
  border: 2px dashed #d9d9d9;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background-color: #fafafa;
}

.upload-area:hover {
  border-color: #409eff;
  background-color: #f0f9ff;
}

.upload-icon {
  font-size: 48px;
  color: #c0c4cc;
  font-weight: 300;
  line-height: 1;
  transition: color 0.3s ease;
}

.upload-area:hover .upload-icon {
  color: #409eff;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  
  .header-actions {
    width: 100%;
    justify-content: flex-end;
  }
  
  .supplier-form .el-col {
    margin-bottom: 16px;
  }
  
  .upload-area {
    width: 100px;
    height: 100px;
  }
  
  .upload-icon {
    font-size: 40px;
  }
}

/* 表单验证样式 */
.el-form-item.is-required .el-form-item__label::before {
  content: '*';
  color: #f56c6c;
  margin-right: 4px;
}

/* 卡片阴影效果 */
.info-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.3s ease;
}

/* 按钮样式优化 */
.el-button--primary {
  background-color: #409eff;
  border-color: #409eff;
}

.el-button--primary:hover {
  background-color: #66b1ff;
  border-color: #66b1ff;
}

/* 输入框样式 */
.el-input__inner:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.2);
}

/* 选择器样式 */
.el-select .el-input__inner:focus {
  border-color: #409eff;
}

/* 日期选择器样式 */
.el-date-editor.el-input {
  width: 100%;
}

/* 银行信息和联系信息列表样式 */
.bank-info-list,
.contact-info-list {
  padding: 0;
}

.bank-info-item,
.contact-info-item {
  position: relative;
  padding: 20px;
  margin-bottom: 16px;
  background: #fafafa;
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  transition: all 0.3s ease;
}

.bank-info-item:hover,
.contact-info-item:hover {
  background: #f0f9ff;
  border-color: #409eff;
}

.bank-info-item:last-child,
.contact-info-item:last-child {
  margin-bottom: 0;
}

.item-actions {
  position: absolute;
  top: 20px;
  right: 20px;
  display: flex;
  gap: 8px;
}

/* 第一行不显示删除按钮 */
.bank-info-item:first-child .item-actions,
.contact-info-item:first-child .item-actions {
  display: none;
}

/* 当只有一行时隐藏删除按钮 */
.bank-info-list:has(.bank-info-item:only-child) .item-actions,
.contact-info-list:has(.contact-info-item:only-child) .item-actions {
  display: none;
}

/* 添加新行时的动画效果 */
.bank-info-item,
.contact-info-item {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 删除行时的动画效果 */
.bank-info-item.removing,
.contact-info-item.removing {
  animation: slideOut 0.3s ease-in forwards;
}

@keyframes slideOut {
  from {
    opacity: 1;
    transform: translateY(0);
    max-height: 200px;
  }
  to {
    opacity: 0;
    transform: translateY(-10px);
    max-height: 0;
    padding: 0;
    margin: 0;
  }
}
</style>