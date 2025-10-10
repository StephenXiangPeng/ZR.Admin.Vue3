<template>
  <div class="material-add-container">
    <!-- 页面头部 -->
    <div class="page-header">
      <div class="header-left">
        <h1 class="page-title">新增物料</h1>
      </div>
      <div class="header-right">
        <el-button class="cancel-btn" @click="handleCancel">取消</el-button>
        <el-button type="primary" class="save-btn" @click="handleSave">保存</el-button>
      </div>
    </div>

    <!-- 基础资料和价格信息 -->
    <div class="form-section">
      <!-- 卡片头部 -->
      <div class="section-header" data-collapsible @click="toggleBasicInfo">
        <div class="section-title-left">
          <div class="section-title-bar"></div>
          <h3 class="section-title">基础资料</h3>
        </div>
        <div class="section-header-right">
          <el-checkbox v-model="formData.syncToPriceList" class="sync-checkbox" @click.stop>
            同步到物料动态价格表中
          </el-checkbox>
          <el-icon class="collapse-icon" :class="{ 'collapsed': !basicInfoExpanded }">
            <ArrowDown />
          </el-icon>
        </div>
      </div>
      
      <!-- 卡片内容 -->
      <div v-show="basicInfoExpanded" class="card-content">
        <!-- 基础资料部分 -->
        <div class="subsection">
          <div class="form-content">
          <!-- 第一行：物料编号、物料名称、物料分类、物料来源、物料规格 -->
          <el-row :gutter="20">
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label required">物料编号</label>
                <div class="input-with-checkbox">
                  <el-input v-model="formData.materialCode" placeholder="请输入" size="default" />
                  <el-checkbox v-model="formData.autoGenerateCode" class="inline-checkbox">自动生成</el-checkbox>
                </div>
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label required">物料名称</label>
                <el-input v-model="formData.materialName" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label required">物料分类</label>
                <el-select v-model="formData.materialCategory" placeholder="请选择" style="width: 100%" size="default">
                  <el-option label="电子元件" value="electronic" />
                  <el-option label="机械零件" value="mechanical" />
                  <el-option label="化工原料" value="chemical" />
                </el-select>
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label required">物料来源</label>
                <el-select v-model="formData.materialSource" placeholder="请选择" style="width: 100%" size="default">
                  <el-option label="采购" value="purchase" />
                  <el-option label="自制" value="self" />
                  <el-option label="外协" value="outsource" />
                </el-select>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="form-item">
                <label class="form-label">物料规格</label>
                <el-input v-model="formData.specification" type="textarea" placeholder="请输入" :rows="1" size="default" />
              </div>
            </el-col>
          </el-row>

          <!-- 第二行：型号、类别、颜色、物料等级、备注 -->
          <el-row :gutter="20">
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">型号</label>
                <el-input v-model="formData.model" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">类别</label>
                <el-select v-model="formData.category" placeholder="请选择" style="width: 100%" size="default">
                  <el-option label="原材料" value="raw" />
                  <el-option label="半成品" value="semi" />
                  <el-option label="成品" value="finished" />
                </el-select>
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">颜色</label>
                <el-input v-model="formData.color" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">物料等级</label>
                <el-select v-model="formData.materialLevel" placeholder="请选择" style="width: 100%" size="default">
                  <el-option label="A级" value="A" />
                  <el-option label="B级" value="B" />
                  <el-option label="C级" value="C" />
                </el-select>
              </div>
            </el-col>
            <el-col :span="4">
              <div class="form-item">
                <label class="form-label">备注</label>
                <el-input v-model="formData.remarks" type="textarea" placeholder="请输入" maxlength="100" show-word-limit :rows="1" size="default" />
              </div>
            </el-col>
          </el-row>

          <!-- 第三行：物料条形码、附件、领料车间 -->
          <el-row :gutter="20">
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">
                  物料条形码
                  <el-tooltip content="请输入物料条形码" placement="top">
                    <el-icon><QuestionFilled /></el-icon>
                  </el-tooltip>
                </label>
                <el-input v-model="formData.barcode" placeholder="请输入物料条形码" maxlength="44" show-word-limit size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">
                  附件
                  <el-tooltip content="上传相关附件" placement="top">
                    <el-icon><QuestionFilled /></el-icon>
                  </el-tooltip>
                </label>
                <el-button type="primary" plain size="default">上传文件</el-button>
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">领料车间</label>
                <el-select v-model="formData.workshop" placeholder="请选择" style="width: 100%" size="default">
                  <el-option label="车间A" value="workshopA" />
                  <el-option label="车间B" value="workshopB" />
                </el-select>
              </div>
            </el-col>
          </el-row>
        </div>
      </div>
        <!-- 价格信息部分 -->
        <div class="subsection">
          <div class="subsection-header">
            <div class="section-title-bar"></div>
            <h3 class="section-title">价格信息</h3>
          </div>
          <div class="form-content">
          <el-row :gutter="20">
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label required">参考成本价 - 基本单位</label>
                <el-input v-model="formData.referenceCost" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label">销售单价 (含税) - 基本单位</label>
                <el-input v-model="formData.salesPrice" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label" :class="{ 'required': formData.enablePriceValidation }">最低销售单价 (含税) - 基本单位</label>
                <el-input v-model="formData.minSalesPrice" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="5">
              <div class="form-item">
                <label class="form-label" :class="{ 'required': formData.enablePriceValidation }">最高销售单价 (含税) - 基本单位</label>
                <el-input v-model="formData.maxSalesPrice" placeholder="请输入" size="default" />
              </div>
            </el-col>
            <el-col :span="4">
              <div class="form-item">
                <label class="form-label">是否启用价格区间校验</label>
                <div class="price-validation-container">
                  <el-checkbox v-model="formData.defaultPriceValidation" class="default-checkbox">默认</el-checkbox>
                </div>
              </div>
            </el-col>
          </el-row>
          </div>
        </div>
      </div>
    </div>

    <!-- 标签页导航 -->
    <div class="tab-navigation">
      <el-tabs v-model="activeTab" class="material-tabs">
        <el-tab-pane label="详细信息" name="details">
          <!-- 详细信息内容 -->
          <div class="tab-content">
            <!-- 单位信息 -->
            <div class="form-section">
              <div class="section-header">
                <div class="section-title-left">
                  <div class="section-title-bar"></div>
                  <h3 class="section-title">单位信息</h3>
                </div>
              </div>
              <div class="form-content">
                <el-row :gutter="20">
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label required">基本单位</label>
                      <el-select v-model="formData.baseUnit" placeholder="请选择" style="width: 100%" size="default">
                        <el-option label="个" value="piece" />
                        <el-option label="千克" value="kg" />
                        <el-option label="米" value="meter" />
                      </el-select>
                    </div>
                  </el-col>
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">启用副单位</label>
                      <el-switch v-model="formData.enableSecondaryUnit" size="default" />
                    </div>
                  </el-col>
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">启用多单位</label>
                      <el-switch v-model="formData.enableMultipleUnits" size="default" />
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>

            <!-- 库存信息 -->
            <div class="form-section">
              <div class="section-header">
                <div class="section-title-left">
                  <div class="section-title-bar"></div>
                  <h3 class="section-title">库存信息</h3>
                </div>
              </div>
              <div class="form-content">
                <el-row :gutter="20">
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">默认仓库</label>
                      <el-select v-model="formData.defaultWarehouse" placeholder="请选择" style="width: 100%" size="default">
                        <el-option label="仓库A" value="warehouseA" />
                        <el-option label="仓库B" value="warehouseB" />
                      </el-select>
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">采购最小起订量</label>
                      <el-input v-model="formData.minOrderQuantity" placeholder="请输入" size="default" />
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">物料批号</label>
                      <el-select v-model="formData.batchNumber" placeholder="请选择" style="width: 100%" size="default">
                        <el-option label="启用" value="enabled" />
                        <el-option label="禁用" value="disabled" />
                      </el-select>
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">采购最小包装量</label>
                      <el-input v-model="formData.minPackageQuantity" placeholder="请输入" size="default" />
                    </div>
                  </el-col>
                </el-row>
                <el-row :gutter="20">
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">序列号</label>
                      <el-select v-model="formData.serialNumber" placeholder="请选择" style="width: 100%" size="default">
                        <el-option label="启用" value="enabled" />
                        <el-option label="禁用" value="disabled" />
                      </el-select>
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">领料最小包装量</label>
                      <el-input v-model="formData.minPickingQuantity" placeholder="请输入" size="default" 	/>
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label">物料有限期</label>
                      <el-input v-model="formData.expirationDays" placeholder="请输入" size="default">
                        <template #append>天</template>
                      </el-input>
                    </div>
                  </el-col>
                  <el-col :span="6">
                    <div class="form-item">
                      <label class="form-label required">存货计价方法</label>
                      <el-select v-model="formData.valuationMethod" placeholder="请选择" style="width: 100%" size="default">
                        <el-option label="加权平均法" value="weighted" />
                        <el-option label="先进先出法" value="fifo" />
                        <el-option label="后进先出法" value="lifo" />
                      </el-select>
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>

            <!-- 库存管理方式 -->
            <div class="form-section">
              <div class="section-header">
                <div class="section-title-left">
                  <div class="section-title-bar"></div>
                  <h3 class="section-title">库存管理方式</h3>
                </div>
              </div>
              <div class="form-content">
                <el-radio-group v-model="formData.inventoryManagementMethod">
                  <el-radio value="unified">统一管理仓库</el-radio>
                  <el-radio value="branch">分仓管理库存</el-radio>
                </el-radio-group>
                <el-row :gutter="20" style="margin-top: 20px;">
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">最低库存数 (基本单位)</label>
                      <el-input v-model="formData.minInventory" placeholder="请输入" size="default" />
                    </div>
                  </el-col>
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">最高库存数 (基本单位)</label>
                      <el-input v-model="formData.maxInventory" placeholder="请输入" size="default" />
                    </div>
                  </el-col>
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">安全库存数 (基本单位)</label>
                      <el-input v-model="formData.safetyStock" placeholder="请输入" size="default" />
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>

            <!-- 质检信息 -->
            <div class="form-section">
              <div class="section-header">
                <div class="section-title-left">
                  <div class="section-title-bar"></div>
                  <h3 class="section-title">质检信息</h3>
                </div>
              </div>
              <div class="form-content">
                <el-row>
                  <el-col :span="8">
                    <div class="form-item">
                      <label class="form-label">质检配置</label>
                      <el-switch v-model="formData.qualityInspection" size="default" />
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>

            <!-- 图片信息 -->
            <div class="form-section">
              <div class="section-header">
                <div class="section-title-left">
                  <div class="section-title-bar"></div>
                  <h3 class="section-title">图片信息</h3>
                </div>
              </div>
              <div class="form-content">
                <el-row :gutter="20">
                  <el-col :span="12">
                    <div class="form-item">
                      <label class="form-label">物料图片</label>
                      <div class="upload-description">(最多上传6张图片,支持JPG,PNG,BMP格式,最大不能超过2MB)</div>
                      <div class="upload-area">
                        <el-upload
                          class="upload-demo"
                          action="#"
                          :auto-upload="false"
                          :limit="6"
                          :on-exceed="handleExceed"
                        >
                          <el-button type="primary">点击上传</el-button>
                        </el-upload>
                      </div>
                    </div>
                  </el-col>
                  <el-col :span="12">
                    <div class="form-item">
                      <label class="form-label">物料图纸</label>
                      <div class="upload-description">(支持上传1张图片,支持JPG,PNG,BMP格式,最大不能超过5MB)</div>
                      <div class="upload-area">
                        <el-upload
                          class="upload-demo"
                          action="#"
                          :auto-upload="false"
                          :limit="1"
                          :on-exceed="handleExceed"
                        >
                          <el-button type="primary">点击上传</el-button>
                        </el-upload>
                      </div>
                    </div>
                  </el-col>
                </el-row>
              </div>
            </div>
          </div>
        </el-tab-pane>

        <el-tab-pane label="物料单价" name="materialPrice">
          <!-- 物料单价内容 -->
          <div class="tab-content">
            <div class="price-settings">
              <div class="setting-item">
                <label>设定采购价格方案:</label>
                <el-select v-model="formData.purchasePriceScheme" placeholder="请选择" size="default">
                  <el-option label="询价单价格优先" value="inquiry" />
                  <el-option label="合同价格优先" value="contract" />
                </el-select>
              </div>
              <div class="setting-item">
                <label>多询价单匹配逻辑:</label>
                <el-radio-group v-model="formData.matchingLogic">
                  <el-radio value="latest">按最新询价匹配</el-radio>
                  <el-radio value="lowest">按最低询价匹配</el-radio>
                </el-radio-group>
              </div>
            </div>

            <div class="price-table">
              <el-table :data="materialPriceData" style="width: 100%">
                <el-table-column prop="supplierName" label="供应商名称" />
                <el-table-column prop="supplierCode" label="供应商编号" />
                <el-table-column prop="priority" label="采购优先级" />
                <el-table-column prop="lowerLimit" label="采购下限" />
                <el-table-column prop="upperLimit" label="采购上限" />
                <el-table-column prop="priceWithTax" label="* 采购含税单价(基本单位)" />
                <el-table-column prop="taxRate" label="税率" />
                <el-table-column prop="priceWithoutTax" label="* 采购不含税单价(基本单位)" />
                <el-table-column label="操作">
                  <template #default>
                    <el-button type="text" size="small">编辑</el-button>
                    <el-button type="text" size="small" style="color: #f56c6c;">删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
              <div v-if="materialPriceData.length === 0" class="empty-data">
                暂无数据
              </div>
              <div class="add-row-btn">
                <el-button type="text" @click="addMaterialPriceRow">新增一行</el-button>
              </div>
            </div>
          </div>
        </el-tab-pane>

        <el-tab-pane label="外协单价" name="outsourcePrice">
          <!-- 外协单价内容 -->
          <div class="tab-content">
            <div class="price-table">
              <el-table :data="outsourcePriceData" style="width: 100%">
                <el-table-column prop="supplierName" label="供应商名称" />
                <el-table-column prop="supplierCode" label="供应商编号" />
                <el-table-column prop="priceWithTax" label="* 外协含税单价(基本单位)" />
                <el-table-column prop="taxRate" label="税率" />
                <el-table-column prop="priceWithoutTax" label="* 外协不含税单价(基本单位)" />
                <el-table-column label="操作">
                  <template #default>
                    <el-button type="text" size="small">编辑</el-button>
                    <el-button type="text" size="small" style="color: #f56c6c;">删除</el-button>
                  </template>
                </el-table-column>
              </el-table>
              <div v-if="outsourcePriceData.length === 0" class="empty-data">
                暂无数据
              </div>
              <div class="add-row-btn">
                <el-button type="text" @click="addOutsourcePriceRow">新增一行</el-button>
              </div>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { QuestionFilled, ArrowUp } from '@element-plus/icons-vue'

// 表单数据
const formData = reactive({
  // 基础资料
  materialCode: '',
  autoGenerateCode: false,
  model: '',
  barcode: '',
  materialName: '',
  category: '',
  attachment: '',
  materialCategory: '',
  color: '',
  workshop: '',
  materialSource: '',
  materialLevel: '',
  specification: '',
  remarks: '',
  syncToPriceList: false,
  
  // 价格信息
  referenceCost: '',
  salesPrice: '',
  minSalesPrice: '',
  maxSalesPrice: '',
  enablePriceValidation: false,
  defaultPriceValidation: false,
  
  // 详细信息
  baseUnit: '',
  enableSecondaryUnit: false,
  enableMultipleUnits: false,
  
  // 库存信息
  defaultWarehouse: '',
  minOrderQuantity: '',
  batchNumber: '',
  minPackageQuantity: '',
  serialNumber: '',
  minPickingQuantity: '',
  expirationDays: '',
  valuationMethod: 'weighted',
  
  // 库存管理
  inventoryManagementMethod: 'unified',
  minInventory: '',
  maxInventory: '',
  safetyStock: '',
  
  // 质检信息
  qualityInspection: false,
  
  // 价格设置
  purchasePriceScheme: 'inquiry',
  matchingLogic: 'latest'
})

// 标签页
const activeTab = ref('details')

// 折叠状态
const basicInfoExpanded = ref(true)

// 表格数据
const materialPriceData = ref([])
const outsourcePriceData = ref([])

// 方法
const toggleBasicInfo = () => {
  basicInfoExpanded.value = !basicInfoExpanded.value
}

// 价格区间校验
const validatePriceRange = () => {
  if (formData.enablePriceValidation) {
    if (!formData.minSalesPrice || !formData.maxSalesPrice) {
      ElMessage.warning('启用价格区间校验时，最低销售单价和最高销售单价为必填项')
      return false
    }
    if (parseFloat(formData.minSalesPrice) >= parseFloat(formData.maxSalesPrice)) {
      ElMessage.warning('最低销售单价必须小于最高销售单价')
      return false
    }
  }
  return true
}

const handleExceed = (files: any) => {
  ElMessage.warning('文件数量超出限制')
}

const addMaterialPriceRow = () => {
  materialPriceData.value.push({
    supplierName: '',
    supplierCode: '',
    priority: '',
    lowerLimit: '',
    upperLimit: '',
    priceWithTax: '',
    taxRate: '',
    priceWithoutTax: ''
  })
}

const addOutsourcePriceRow = () => {
  outsourcePriceData.value.push({
    supplierName: '',
    supplierCode: '',
    priceWithTax: '',
    taxRate: '',
    priceWithoutTax: ''
  })
}

// 保存处理
const handleSave = () => {
  // 验证价格区间
  if (!validatePriceRange()) {
    return
  }
  
  // 这里可以添加其他验证逻辑
  ElMessage.success('保存成功')
  console.log('保存数据:', formData)
}

// 取消处理
const handleCancel = () => {
  ElMessageBox.confirm('确定要取消吗？未保存的数据将丢失。', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '继续编辑',
    type: 'warning'
  }).then(() => {
    // 这里可以添加返回上一页的逻辑
    ElMessage.info('已取消')
  })
}
</script>

<style scoped>
.material-add-container {
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
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.page-title {
  color: #e74c3c;
  font-size: 24px;
  font-weight: bold;
  margin: 0;
}

.header-right {
  display: flex;
  gap: 10px;
}

.cancel-btn {
  background-color: #f5f5f5;
  border-color: #d9d9d9;
  color: #666;
}

.save-next-btn {
  background-color: #f5f5f5;
  border-color: #d9d9d9;
  color: #666;
}

.save-btn {
  background-color: #e74c3c;
  border-color: #e74c3c;
}

/* 表单部分 */
.form-section {
  background: white;
  margin-bottom: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

/* 卡片内容 */
.card-content {
  padding: 0;
}

/* 子部分标题 */
.subsection-header {
  display: flex;
  align-items: center;
  padding: 20px 20px 0 20px;
  border-bottom: 1px solid #f0f0f0;
}

.subsection:last-child .subsection-header {
  border-bottom: none;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 20px 0 20px;
  /* 移除标题下方的横线 */
}

.section-header[data-collapsible] {
  cursor: pointer;
  transition: background-color 0.3s;
}

.section-header[data-collapsible]:hover {
  background-color: #f5f5f5;
}

.section-title-left {
  display: flex;
  align-items: center;
}

.section-title-bar {
  width: 4px;
  height: 20px;
  background-color: #e74c3c;
  margin-right: 10px;
}

.section-title {
  color: #e74c3c;
  font-size: 16px;
  font-weight: bold;
  margin: 0;
}

.section-header-right {
  display: flex;
  align-items: center;
}

.input-with-checkbox {
  display: flex;
  align-items: center;
  gap: 10px;
}

.inline-checkbox {
  margin: 0;
  white-space: nowrap;
}

.collapse-icon {
  margin-left: 10px;
  transition: transform 0.3s;
  color: #666;
}

.collapse-icon.collapsed {
  transform: rotate(-90deg);
}

.form-content {
  padding: 20px;
}

.form-item {
  margin-bottom: 20px;
}

.form-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #333;
}

.form-label.required::after {
  content: '*';
  color: #e74c3c;
  margin-left: 4px;
}

.checkbox-inline {
  margin-left: 10px;
}

.sync-checkbox {
  display: flex;
  align-items: center;
  gap: 5px;
}

.arrow-up {
  color: #e74c3c;
}

.price-validation-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.price-validation-checkbox {
  margin: 0;
}

.default-checkbox {
  margin: 0;
}

.text-right {
  text-align: right;
}

/* 标签页 */
.tab-navigation {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.material-tabs {
  padding: 0 20px;
}

.material-tabs :deep(.el-tabs__header) {
  margin: 0;
}

.material-tabs :deep(.el-tabs__nav-wrap::after) {
  display: none;
}

.material-tabs :deep(.el-tabs__item) {
  font-size: 16px;
  padding: 0 20px;
  height: 50px;
  line-height: 50px;
}

.material-tabs :deep(.el-tabs__item.is-active) {
  color: #e74c3c;
}

.material-tabs :deep(.el-tabs__active-bar) {
  background-color: #e74c3c;
}

.tab-content {
  padding: 20px;
}

/* 价格设置 */
.price-settings {
  margin-bottom: 20px;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 6px;
}

.setting-item {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  gap: 10px;
}

.setting-item:last-child {
  margin-bottom: 0;
}

.setting-item label {
  font-weight: 500;
  color: #333;
  min-width: 150px;
}

/* 表格 */
.price-table {
  margin-top: 20px;
}

.empty-data {
  text-align: center;
  color: #999;
  padding: 40px 0;
  font-size: 14px;
}

.add-row-btn {
  text-align: center;
  margin-top: 20px;
}

.add-row-btn .el-button {
  color: #409eff;
}

/* 上传区域 */
.upload-description {
  font-size: 12px;
  color: #999;
  margin-bottom: 10px;
}

.upload-area {
  border: 2px dashed #d9d9d9;
  border-radius: 6px;
  padding: 20px;
  text-align: center;
  background-color: #fafafa;
}

.upload-area:hover {
  border-color: #409eff;
}

/* 响应式布局 */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 15px;
  }
  
  .header-right {
    width: 100%;
    justify-content: center;
  }
  
  .form-content .el-row {
    margin: 0;
  }
  
  .form-content .el-col {
    padding: 0 10px;
  }
}
</style>