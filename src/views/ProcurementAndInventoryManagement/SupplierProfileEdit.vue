<template>
  <div class="app-container">
    <!-- 页面头部 -->
    <div class="page-header">
      <h1 class="page-title">新增供应商</h1>
      <div class="header-actions">
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSave" v-if="mode !== 'view'" :loading="uploading">保存</el-button>
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
            <el-col :span="6">
              <el-form-item label="*供应商编号" required>
                <el-input v-model="form.supplierNumber" placeholder="请输入" :disabled="form.useSystemNumber">
                  <template #append>
                    <el-checkbox v-model="form.useSystemNumber">使用系统编号</el-checkbox>
                  </template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="*供应商名称" required>
                <el-input v-model="form.supplierName" placeholder="请输入" />
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="产品分类">
                <el-select v-model="form.productCategory" placeholder="请选择" style="width: 100%">
                  <el-option label="分类1" value="1" />
                  <el-option label="分类2" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="物料分类">
                <el-select v-model="form.materialCategory" placeholder="请选择" style="width: 100%">
                  <el-option label="物料分类1" value="1" />
                  <el-option label="物料分类2" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          
          <el-row :gutter="20">
            
            <el-col :span="6">
              <el-form-item label="默认采购税率">
                <el-input v-model="form.taxRate" placeholder="请输入">
                  <template #append>%</template>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="结算期限">
                <el-date-picker v-model="form.settlementPeriod" type="date" placeholder="请选择" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="部门">
                <el-select v-model="form.department" placeholder="请选择" style="width: 100%">
                  <el-option label="采购部" value="1" />
                  <el-option label="财务部" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="采购员">
                <el-select v-model="form.purchaser" placeholder="请选择" style="width: 100%">
                  <el-option label="张三" value="1" />
                  <el-option label="李四" value="2" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="6">
              <el-form-item label="微信">
                <el-input v-model="form.wechat" placeholder="请输入" />
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="备注">
                <el-input 
                  v-model="form.remarks" 
                  type="textarea" 
                  :rows="2" 
                  placeholder="请输入"
                  maxlength="100"
                  show-word-limit
                />
              </el-form-item>
            </el-col>
            <el-col :span="6">
              <el-form-item label="附件">
                <el-upload
                  class="attachment-upload"
                  action="#"
                  :file-list="attachmentList"
                  :on-change="handleAttachmentChange"
                  :on-remove="handleAttachmentRemove"
                  :before-upload="beforeUpload"
                  multiple
                  :limit="5"
                >
                  <el-button type="primary" size="small">上传文件</el-button>
                </el-upload>
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
            <el-button type="primary" size="default" @click.stop="addBankInfo">新增银行信息</el-button>
            <el-button size="default" @click.stop="deleteBankInfo">删除银行信息</el-button>
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
                      <el-radio v-model="defaultBankIndex" :value="index" @change="setDefaultBank(index)">设为默认</el-radio>
                    </template>
                  </el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
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
            <el-button type="primary" size="default" @click.stop="addContactInfo">新增联系信息</el-button>
            <el-button size="default" @click.stop="deleteContactInfo">删除联系信息</el-button>
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
              <el-col :span="6">
                <el-form-item label="联系人">
                  <el-input v-model="contactInfo.contactPerson" placeholder="请输入" maxlength="50" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="联系方式">
                  <el-input v-model="contactInfo.contactMethod" placeholder="请输入" maxlength="30" show-word-limit />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="联系地址">
                  <el-input v-model="contactInfo.contactAddress" placeholder="请输入" maxlength="100" show-word-limit>
                    <template #append>
                      <el-radio v-model="defaultContactIndex" :value="index" @change="setDefaultContact(index)">设为默认</el-radio>
                    </template>
                  </el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
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
          action="#"
          multiple
          :limit="6"
          :file-list="imageList"
          :on-change="handleImageChange"
          :on-remove="handleImageRemove"
          :before-upload="beforeUpload"
          :show-file-list="true"
          list-type="picture-card"
        >
          <el-icon class="avatar-uploader-icon"><Plus /></el-icon>
        </el-upload>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowUp, QuestionFilled, UploadFilled, Plus } from '@element-plus/icons-vue'
import { addSupplierInfo } from '@/api/ProcurementAndInventoryManagement/supplier'
import { upload } from '@/api/common'

const route = useRoute()
const router = useRouter()

// 获取路由参数
const supplierId = ref(route.query.id)
const mode = ref(route.query.mode || 'add')

// 文件列表
const fileList = ref([])
const attachmentList = ref([]) // 附件列表
const imageList = ref([]) // 图片列表
const uploading = ref(false) // 上传状态

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

// 默认银行信息索引
const defaultBankIndex = ref(0)

// 默认联系信息索引
const defaultContactIndex = ref(0)

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
async function handleSave() {
  try {
    // 表单验证
    if (!validateForm()) {
      return
    }

    uploading.value = true
    
    // 上传附件
    let attachmentSet = ''
    if (attachmentList.value.length > 0) {
      const attachmentUrls = await uploadFiles(attachmentList.value)
      attachmentSet = attachmentUrls.join(',')
    }

    // 上传图片
    let imageSet = ''
    if (imageList.value.length > 0) {
      const imageUrls = await uploadFiles(imageList.value)
      imageSet = imageUrls.join(',')
    }

    // 构建联系信息数组
    const contactItems = contactInfoList.value.map(contact => ({
      contactName: contact.contactPerson,
      contactPhone: contact.contactMethod,
      contactAddress: contact.contactAddress,
      isDefaultContact: contact.isDefault ? 1 : 0,
      remark: ''
    }))

    // 构建财务信息数组
    const financialItems = bankInfoList.value.map(bank => ({
      taxIDNumber: bank.taxId,
      accountName: bank.accountName,
      openAccountBank: bank.bankName,
      bankAccount: bank.bankAccount,
      isDefault: bank.isDefault ? 1 : 0,
      remark: ''
    }))

    // 构建请求数据 - 根据后端接口要求
    const requestData = {
      supplierCode: form.supplierNumber || '',
      supplierName: form.supplierName || '',
      productClassificationSet: form.productCategory || '',
      materialsClassificationSet: form.materialCategory || '',
      defaultPurchaseTaxRate: parseFloat(form.taxRate) || 0,
      settlementPeriodID: form.settlementPeriod ? new Date(form.settlementPeriod).getTime() : 0,
      departmentID: parseInt(form.department) || 0,
      buyerID: parseInt(form.purchaser) || 0,
      attachmentSet: attachmentSet,
      imageSet: imageSet,
      weChatNumber: form.wechat || '',
      remark: form.remarks || '',
      contactItems: contactItems,
      financialItems: financialItems
    }

    // 调用新增接口
    const response = await addSupplierInfo(requestData)
    
    // 根据后端返回结果处理
    if (response && response.data && response.data.code === 200) {
      ElMessage.success('供应商信息添加成功')
      handleCancel()
    } else {
      ElMessage.error(response?.data?.msg || '供应商信息添加失败')
    }
  } catch (error) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败，请重试')
  } finally {
    uploading.value = false
  }
}

// 表单验证
function validateForm() {
  if (!form.supplierName.trim()) {
    ElMessage.error('请输入供应商名称')
    return false
  }
  
  if (!form.useSystemNumber && !form.supplierNumber.trim()) {
    ElMessage.error('请输入供应商编号或选择使用系统编号')
    return false
  }

  // 验证财务信息（银行信息）
  for (let i = 0; i < bankInfoList.value.length; i++) {
    const bankInfo = bankInfoList.value[i]
    if (!bankInfo.taxId.trim() || !bankInfo.accountName.trim() || 
        !bankInfo.bankName.trim() || !bankInfo.bankAccount.trim()) {
      ElMessage.error(`请完善第${i + 1}条财务信息（税号、开户名称、开户银行、银行账号）`)
      return false
    }
  }

  // 验证联系信息
  for (let i = 0; i < contactInfoList.value.length; i++) {
    const contactInfo = contactInfoList.value[i]
    if (!contactInfo.contactPerson.trim() || !contactInfo.contactMethod.trim() || 
        !contactInfo.contactAddress.trim()) {
      ElMessage.error(`请完善第${i + 1}条联系信息（联系人、联系方式、联系地址）`)
      return false
    }
  }

  return true
}

// 上传文件
async function uploadFiles(files) {
  const uploadPromises = files.map(async (file) => {
    const formData = new FormData()
    formData.append('file', file.raw || file)
    
    try {
      const response = await upload(formData)
      return response.data.url || response.data.fileName
    } catch (error) {
      console.error('文件上传失败:', error)
      throw error
    }
  })

  return Promise.all(uploadPromises)
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

function setDefaultBank(index: number) {
  // 重置所有银行信息的默认状态
  bankInfoList.value.forEach((item, i) => {
    item.isDefault = i === index
  })
  defaultBankIndex.value = index
}

function deleteBankInfo() {
  if (bankInfoList.value.length > 1) {
    bankInfoList.value.pop()
    // 如果删除的是默认项，重置默认索引
    if (defaultBankIndex.value >= bankInfoList.value.length) {
      defaultBankIndex.value = 0
      bankInfoList.value[0].isDefault = true
    }
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

function setDefaultContact(index: number) {
  // 重置所有联系信息的默认状态
  contactInfoList.value.forEach((item, i) => {
    item.isDefault = i === index
  })
  defaultContactIndex.value = index
}

function deleteContactInfo() {
  if (contactInfoList.value.length > 1) {
    contactInfoList.value.pop()
    // 如果删除的是默认项，重置默认索引
    if (defaultContactIndex.value >= contactInfoList.value.length) {
      defaultContactIndex.value = 0
      contactInfoList.value[0].isDefault = true
    }
    ElMessage.success('已删除最后一条联系信息')
  } else {
    ElMessage.warning('至少保留一条联系信息')
  }
}

// 文件上传处理
function handleFileChange(file: any, fileList: any[]) {
  console.log('文件变化:', file, fileList)
}

// 附件上传处理
function handleAttachmentChange(file: any, fileList: any[]) {
  attachmentList.value = fileList
}

function handleAttachmentRemove(file: any, fileList: any[]) {
  attachmentList.value = fileList
}

// 图片上传处理
function handleImageChange(file: any, fileList: any[]) {
  imageList.value = fileList
}

function handleImageRemove(file: any, fileList: any[]) {
  imageList.value = fileList
}

// 上传前验证
function beforeUpload(file: any) {
  const isImage = file.type.startsWith('image/')
  const isLt10M = file.size / 1024 / 1024 < 10

  if (!isLt10M) {
    ElMessage.error('文件大小不能超过 10MB!')
    return false
  }

  if (isImage) {
    const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/bmp'
    if (!isJpgOrPng) {
      ElMessage.error('图片只能是 JPG/PNG/BMP 格式!')
      return false
    }
  }

  return true
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
  padding: 6px;
  background-color: #f5f5f5;
  min-height: 100vh;
}

/* 页面头部 */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
  padding: 6px 12px;
  background: white;
  border-radius: 4px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.page-title {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #303133;
}

.header-actions {
  display: flex;
  gap: 4px;
}

/* 信息卡片 */
.info-card {
  margin-bottom: 6px;
  border-radius: 4px;
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
  padding: 1px 8px;
  min-height: 8px;
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
  gap: 3px;
}

/* 表单样式 */
.supplier-form {
  padding: 6px 0;
}

.supplier-form .el-form-item {
  margin-bottom: 6px;
}

.supplier-form .el-form-item__label {
  font-weight: 500;
  color: #606266;
  font-size: 12px;
}

/* 输入框高度压缩 */
.supplier-form .el-input__wrapper {
  min-height: 24px;
}

.supplier-form .el-input__inner {
  height: 24px;
  line-height: 24px;
}

.supplier-form .el-select .el-input__wrapper {
  min-height: 24px;
}

.supplier-form .el-select .el-input__inner {
  height: 24px;
  line-height: 24px;
}

.supplier-form .el-date-editor {
  height: 24px;
}

.supplier-form .el-date-editor .el-input__wrapper {
  min-height: 24px;
}

.supplier-form .el-date-editor .el-input__inner {
  height: 24px;
  line-height: 24px;
}

.supplier-form .el-textarea__inner {
  min-height: 48px;
}

.supplier-form .el-button {
  height: 24px;
  padding: 4px 8px;
  font-size: 12px;
}

.help-icon {
  margin-left: 3px;
  color: #909399;
  cursor: pointer;
}

/* 图片上传区域 */
.image-upload-section {
  padding: 4px 0;
}

.upload-tip {
  margin: 0 0 6px 0;
  color: #909399;
  font-size: 10px;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }
  
  .header-actions {
    width: 100%;
    justify-content: flex-end;
  }
  
  .supplier-form .el-col {
    margin-bottom: 4px;
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
  padding: 6px;
  margin-bottom: 4px;
  background: #fafafa;
  border: 1px solid #e4e7ed;
  border-radius: 3px;
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
  top: 6px;
  right: 6px;
  display: flex;
  gap: 3px;
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

/* 附件上传样式 */
.attachment-upload {
  width: 100%;
}

.attachment-upload .el-upload-list {
  margin-top: 10px;
}

</style>