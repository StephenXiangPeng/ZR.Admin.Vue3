<template>
  <div class="page-wrap">
    <el-page-header content="价格列表管理（品种主价 + 附加价）" @back="goBack" />
    <!-- ====== 对话框：新增 / 维护品种价格 ====== -->
    <el-dialog v-model="formDlg.visible" title="新增 / 维护品种价格" width="720px" :close-on-click-modal="false">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="110px" class="form-grid" :disabled="loading.form">
        <el-form-item label="折射率" prop="refractionId">
          <el-select 
            v-model="form.refractionId" 
            placeholder="请选择折射率" 
            filterable 
            clearable
            :loading="loading.refractions"
            @change="onRefractionChange" 
            style="width: 240px"
          >
            <el-option 
              v-for="o in options.refractions" 
              :key="o.id" 
              :label="o.optionName || o.option_name" 
              :value="o.id"
            >
              <span>{{ o.optionName || o.option_name }}</span>
              <span v-if="o.remark" class="option-remark">({{ o.remark }})</span>
            </el-option>
          </el-select>
          <el-link type="primary" class="ml8" @click="refreshRefractions" :underline="false">刷新</el-link>
        </el-form-item>

        <el-form-item label="材质" prop="materialId">
          <el-select 
            v-model="form.materialId" 
            placeholder="请先选择折射率" 
            :disabled="materialsDisabled" 
            filterable 
            clearable
            :loading="loading.materials"
            style="width: 260px"
          >
            <el-option 
              v-for="m in options.materials" 
              :key="m.id" 
              :label="(m.optionName || m.option_name) + (m.is_default ? '（默认）' : '')" 
              :value="m.id"
            >
              <span>{{ m.optionName || m.option_name }}</span>
              <span v-if="m.is_default" class="default-tag">（默认）</span>
            </el-option>
          </el-select>
          <!-- <el-link type="primary" class="ml8" @click="openMaterialConfig" :underline="false">配置材质</el-link> -->
        </el-form-item>

        <el-form-item label="设计" prop="designId">
          <el-select 
            v-model="form.designId" 
            placeholder="请选择设计" 
            filterable 
            clearable
            :loading="loading.designs"
            style="width: 240px"
          >
            <el-option 
              v-for="o in options.designs" 
              :key="o.id" 
              :label="o.optionName || o.option_name" 
              :value="o.id"
            >
              <span>{{ o.optionName || o.option_name }}</span>
              <span v-if="o.remark" class="option-remark">({{ o.remark }})</span>
            </el-option>
          </el-select>
          <el-link type="primary" class="ml8" @click="refreshDesigns" :underline="false">刷新</el-link>
        </el-form-item>

        <el-form-item label="基础价格" prop="basePrice">
          <el-input v-model="form.basePrice" placeholder="请输入价格，如 12.34" style="width: 200px">
            <template #prefix>￥</template>
          </el-input>
          <el-select v-model="form.currency" class="ml8" style="width: 110px">
            <el-option label="USD" value="USD" />
            <el-option label="CNY" value="CNY" />
          </el-select>
        </el-form-item>

        <el-form-item label="绑定客户">
          <el-select 
            v-model="form.customerId" 
            placeholder="请选择客户（可选）" 
            filterable 
            clearable
            :loading="loading.customers"
            style="width: 240px"
          >
            <el-option 
              v-for="customer in options.customers" 
              :key="customer.dictValue" 
              :label="customer.dictLabel" 
              :value="customer.dictValue"
            />
          </el-select>
          <el-link type="primary" class="ml8" @click="loadCustomers" :underline="false">刷新</el-link>
        </el-form-item>

        <el-form-item label="备注">
          <el-input v-model="form.remark" placeholder="可选" type="textarea" :rows="2" style="width: 520px" />
        </el-form-item>

        <!-- 组合预览条 -->
        <el-form-item v-if="comboPreview" label="当前组合">
          <el-alert :title="comboPreview" type="info" :closable="false" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="formDlg.visible=false">取 消</el-button>
        <el-button @click="resetForm">重 置</el-button>
        <el-button type="primary" :loading="loading.save" :disabled="!canSubmit" @click="handleSave">保 存</el-button>
      </template>
    </el-dialog>

    <!-- ====== 卡片2：已维护价格列表 ====== -->
    <el-card class="mt16" shadow="never">
      <template #header>
        <div class="card-header">
          <span>已维护价格</span>
          <div>
            <el-button type="success" @click="openExcelImport">Excel导入</el-button>
            <el-button type="primary" @click="openNewPrice">新增品种价格</el-button>
            <el-button :loading="loading.list" @click="loadPriceList">刷新</el-button>
          </div>
        </div>
      </template>

      <el-form :inline="true" class="mb12">
        <el-form-item label="折射率">
          <el-select v-model="query.refractionId" clearable placeholder="全部" style="width: 180px" @change="onQueryChange">
            <el-option v-for="o in options.refractions" :key="o.id" :label="o.optionName || o.option_name" :value="o.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="onQueryChange">查询</el-button>
          <el-button @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="table.rows" v-loading="loading.list" border>
        <el-table-column type="index" label="#" width="60" />
        <el-table-column prop="refractionName" label="折射率" width="120" />
        <el-table-column prop="materialName" label="材质" width="150" />
        <el-table-column prop="designName" label="设计" width="180" />
        <el-table-column prop="basePrice" label="基础价格" width="140">
          <template #default="{ row }">
            <span>{{ row.currency || 'USD' }} {{ formatMoney(row.basePrice) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="customerName" label="绑定客户" width="120">
          <template #default="{ row }">
            <span>{{ row.customerName || '无' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : 'info'">{{ row.status === 0 ? '正常' : '停用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="prefillForm(row)">编辑</el-button>
            <el-popconfirm title="确定删除该价格吗？" @confirm="removeRow(row.id)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pager">
        <el-pagination
          background
          layout="prev, pager, next, ->, total"
          :total="table.total"
          :current-page="table.page"
          :page-size="table.pageSize"
          @current-change="onPageChange"
        />
      </div>
    </el-card>

    <!-- ====== 卡片3：附加价维护（膜层 / 额外处理） ====== -->
    <el-card class="mt16" shadow="never">
      <template #header>
        <div class="card-header">
          <span>附加价维护（膜层 / 额外处理）</span>
          <div>
            <el-button type="primary" @click="openAddSurcharge">新增附加价</el-button>
            <el-button :loading="loading.surcharge" @click="loadSurchargeList">刷新</el-button>
          </div>
        </div>
      </template>

      <el-form :inline="true" class="mb12">
        <el-form-item label="类型">
          <el-select v-model="surcharge.type" style="width: 180px" @change="onSurchargeTypeChange">
            <el-option label="膜层" :value="OPTION_TYPE.Coating" />
            <el-option label="额外处理" :value="OPTION_TYPE.Extra" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadSurchargeList">查询</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="surcharge.rows" v-loading="loading.surcharge" border>
        <el-table-column type="index" label="#" width="60" />
        <el-table-column prop="optionName" label="选项名称" width="200">
          <template #default="{ row }">
            <span>{{ row.optionName || '加载中...' }}</span>
          </template>
        </el-table-column>
        <el-table-column label="默认包含" width="100">
          <template #default="{ row }">
            <span>{{ row.isIncluded === 1 ? '是' : '否' }}</span>
          </template>
        </el-table-column>
        <el-table-column label="附加价" width="120">
          <template #default="{ row }">
            <span>{{ row.isIncluded === 1 ? '已包含' : formatMoney(row.surcharge) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="customerName" label="绑定客户" width="120">
          <template #default="{ row }">
            <span>{{ row.customerName || '无' }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="editSurcharge(row)">编辑</el-button>
            <el-popconfirm title="确定删除该附加价吗？" @confirm="removeSurcharge(row.id)">
              <template #reference>
                <el-button link type="danger">删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pager">
        <el-pagination
          background
          layout="prev, pager, next, ->, total"
          :total="surcharge.total"
          :current-page="surcharge.page"
          :page-size="surcharge.pageSize"
          @current-change="onSurchargePageChange"
        />
      </div>

      <el-dialog v-model="addDlg.visible" title="新增附加项" width="520px">
        <el-form :model="addDlg.form" label-width="100px">
          <el-form-item label="类型">
            <el-segmented v-model="surcharge.type" :options="[{label:'膜层', value: OPTION_TYPE.Coating},{label:'额外处理', value: OPTION_TYPE.Extra}]" @change="onAddTypeChange" />
          </el-form-item>
          <el-form-item label="选项">
            <el-select v-model="addDlg.form.optionId" filterable placeholder="请选择选项" style="width: 320px">
              <el-option v-for="opt in addDlg.candidates" :key="opt.id" :label="opt.optionName || opt.option_name" :value="opt.id" />
            </el-select>
            <!-- <el-link class="ml8" type="primary" :underline="false" @click="gotoOptionsPage">去选项页新增</el-link> -->
          </el-form-item>
          <el-form-item label="默认包含">
            <el-switch v-model="addDlg.form.isIncluded" :active-value="1" :inactive-value="0" @change="val => { if(val===1) addDlg.form.surcharge = 0 }" />
          </el-form-item>
          <el-form-item label="附加价">
            <el-input-number v-model="addDlg.form.surcharge" :precision="2" :step="1" :disabled="addDlg.form.isIncluded===1" />
          </el-form-item>
          <el-form-item label="绑定客户">
            <el-select 
              v-model="addDlg.form.customerId" 
              placeholder="请选择客户（可选）" 
              filterable 
              clearable
              :loading="loading.customers"
              style="width: 320px"
            >
              <el-option 
                v-for="customer in options.customers" 
                :key="customer.dictValue" 
                :label="customer.dictLabel" 
                :value="customer.dictValue"
              />
            </el-select>
          </el-form-item>
        </el-form>
        <template #footer>
          <el-button @click="addDlg.visible=false">取 消</el-button>
          <el-button type="primary" :loading="addDlg.saving" @click="saveAddSurcharge">保 存</el-button>
        </template>
      </el-dialog>

    <!-- ====== Excel导入对话框 ====== -->
    <el-dialog v-model="excelImportDlg.visible" title="品种价格Excel导入" width="650px" :close-on-click-modal="false">
      <div class="import-content">
        <el-alert 
          title="导入说明" 
          type="info" 
          :closable="false" 
          class="mb16"
        >
          <template #default>
            <div>
              <p>1. 支持 .xlsx 和 .xls 格式文件</p>
              <p>2. 必填字段：INDEX(折射率)、DESIGN(设计)、MATERIALS(材质)、PRICE/PIECE(价格)</p>
              <p>3. 系统会自动去重，重复组合将被跳过</p>
            </div>
          </template>
        </el-alert>
        
        
        <div class="upload-area">
          <input 
            ref="fileInputRef"
            type="file" 
            accept=".xlsx,.xls" 
            @change="handleFileSelect"
            style="display: none"
          />
          <div 
            class="upload-dragger" 
            @click="triggerFileSelect"
            @dragover.prevent
            @drop.prevent="handleFileDrop"
          >
            <el-icon class="el-icon--upload"><upload-filled /></el-icon>
            <div class="upload-text">点击选择文件或拖拽文件到此处</div>
            <div class="upload-tip">仅允许导入xls、xlsx格式文件</div>
          </div>
          <div v-if="excelImportDlg.selectedFile" class="selected-file">
            <el-icon><document /></el-icon>
            <span>{{ excelImportDlg.selectedFile.name }}</span>
            <el-button type="text" @click="clearSelectedFile">移除</el-button>
          </div>
        </div>
      </div>
      
      <template #footer>
        <el-button @click="excelImportDlg.visible = false">取 消</el-button>
        <el-button type="primary" :loading="excelImportDlg.isUploading" @click="submitImport">开始导入</el-button>
      </template>
    </el-dialog>
    </el-card>
  </div>
</template>

<script lang="ts" setup>
import { reactive, ref, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { UploadFilled, Document } from '@element-plus/icons-vue'
import { getToken } from '@/utils/auth'

/** 约定的 option_type 值（与你后端枚举保持一致） */
const OPTION_TYPE = {
  Refraction: 5,
  Material: 3,
  LensType: 1,
  Design: 4,
  Coating: 2,
  Extra: 17,
}

type OptionItem = { 
  id: number; 
  option_name: string; 
  optionName?: string;
  is_default?: number;
  remark?: string;
}
type PriceRow = {
  id: number
  refractionId: number
  materialId: number
  lensTypeId: number
  designId: number
  basePrice: number
  currency: string
  status: number
  isDelete: number
  createBy: string
  createTime: string
  updateTime?: string
  remark?: string
  refractionName?: string
  materialName?: string
  designName?: string
}

type SurchargeRow = {
  id?: number
  optionId: number
  optionType: number
  optionName?: string
  isIncluded: number
  surcharge: number
  status: number
  is_Delete: number
  createBy: string
  createTime: string
  updateTime?: string
  remark?: string
}

const loading = reactive({ 
  form: false, 
  list: false, 
  save: false, 
  surcharge: false,
  refractions: false,
  materials: false,
  designs: false,
  customers: false
})
const options = reactive({
  refractions: [] as OptionItem[],
  materials: [] as OptionItem[],
  designs: [] as OptionItem[],
  customers: [] as { dictValue: number; dictLabel: string }[],
})

/** 新增/编辑表单 */
const formRef = ref()
const form = reactive({
  id: 0,
  refractionId: undefined as number | undefined,
  materialId: undefined as number | undefined,
  designId: undefined as number | undefined,
  basePrice: '' as string | number,
  currency: 'USD',
  remark: '',
  customerId: undefined as number | undefined,
})

// 对话框控制
const formDlg = reactive({ visible: false })

const rules = {
  refractionId: [{ required: true, message: '请选择折射率', trigger: 'change' }],
  materialId: [{ required: true, message: '请选择材质', trigger: 'change' }],
  designId: [{ required: true, message: '请选择设计', trigger: 'change' }],
  basePrice: [
    { required: true, message: '请输入基础价格', trigger: 'blur' },
    { validator: (_: any, v: any, cb: any) => (/^\d+(\.\d{1,2})?$/.test(String(v)) ? cb() : cb(new Error('价格格式不正确'))) },
  ],
}

/** 列表与查询 */
const table = reactive({
  rows: [] as PriceRow[],
  total: 0,
  page: 1,
  pageSize: 10,
})
const query = reactive({
  refractionId: undefined as number | undefined,
})

const materialsDisabled = computed(() => !form.refractionId)
const canSubmit = computed(() =>
  !!(form.refractionId && form.materialId && form.designId && String(form.basePrice).length > 0)
)

const comboPreview = computed(() => {
  const rn = options.refractions.find(x => x.id === form.refractionId)?.optionName || options.refractions.find(x => x.id === form.refractionId)?.option_name
  const mn = options.materials.find(x => x.id === form.materialId)?.optionName || options.materials.find(x => x.id === form.materialId)?.option_name
  const dn = options.designs.find(x => x.id === form.designId)?.optionName || options.designs.find(x => x.id === form.designId)?.option_name
  if (rn && mn && dn) {
    return `${rn} × ${mn} × ${dn} → 将保存为一条品种价格`
  }
  return ''
})

/** —— API 实现（使用项目内 request 封装） —— */
import { 
  getLensOptionsByType, 
  getLensOptionById,
  getMaterialsByRefraction,
  addVarietyPrice,
  updateVarietyPrice,
  getVarietyPriceList,
  deleteVarietyPrice,
  batchDeleteVarietyPrice,
  getVarietyPriceById,
  getSurchargeList,
  addSurcharge,
  updateSurcharge,
  deleteSurcharge,
  batchDeleteSurcharge,
  getSurchargeById,
  updateSurchargeStatus,
  getCustomerUserList,
  importVarietyPrice
} from '@/api/DFGX/priceManagement'

// 获取指定类型的选项列表
async function apiGetOptions(type: number) {
  try {
    const response = await getLensOptionsByType(type)
    return response.data || []
  } catch (error) {
    console.error('获取选项数据失败:', error)
    ElMessage.error('获取选项数据失败')
    return []
  }
}

// 根据折射率获取关联材质
async function apiGetMaterialsByRefraction(refractionId: number) {
  try {
    const response = await getMaterialsByRefraction(refractionId)
    console.log('材质API响应:', response)
    const materials = response.data || response || []
    console.log('材质数据:', materials)
    return materials
  } catch (error) {
    console.error('获取关联材质失败:', error)
    ElMessage.error('获取关联材质失败')
    return []
  }
}

// 新增品种价格
async function apiAddVarietyPrice(payload: any) {
  try {
    const response = await addVarietyPrice(payload)
    return response
  } catch (error) {
    console.error('新增价格失败:', error)
    throw error
  }
}

// 编辑品种价格
async function apiUpdateVarietyPrice(payload: any) {
  try {
    const response = await updateVarietyPrice(payload)
    return response
  } catch (error) {
    console.error('编辑价格失败:', error)
    throw error
  }
}

// 保存品种价格（统一接口）
async function apiSaveVarietyPrice(payload: any) {
  try {
    if (payload.id) {
      return await apiUpdateVarietyPrice(payload)
    } else {
      return await apiAddVarietyPrice(payload)
    }
  } catch (error) {
    console.error('保存价格失败:', error)
    throw error
  }
}

// 获取价格列表
async function apiListVarietyPrice(params: any) {
  try {
    const response = await getVarietyPriceList(params)
    const data = response.data || {}
    return {
      rows: data.result || [],
      total: data.totalNum || 0,
      pageSize: data.pageSize || 10,
      pageIndex: data.pageIndex || 1
    }
  } catch (error) {
    console.error('获取价格列表失败:', error)
    ElMessage.error('获取价格列表失败')
    return { rows: [], total: 0 }
  }
}

// 删除价格
async function apiRemoveVarietyPrice(id: number) {
  try {
    return await deleteVarietyPrice(id)
  } catch (error) {
    console.error('删除价格失败:', error)
    throw error
  }
}

// 获取附加价列表
async function apiListSurcharge(type: number, page: number = 1, pageSize: number = 10) {
  try {
    const response = await getSurchargeList(type, page, pageSize)
    const data = response.data || {}
    console.log('附加价列表原始数据:', data)
    
    // 处理分页数据结构
    if (data.result && Array.isArray(data.result)) {
      console.log('处理后的附加价列表:', data.result)
      return {
        rows: data.result,
        total: data.totalNum || 0,
        pageSize: data.pageSize || 10,
        pageIndex: data.pageIndex || 1
      }
    } else if (Array.isArray(data)) {
      return {
        rows: data,
        total: data.length,
        pageSize: pageSize,
        pageIndex: page
      }
    } else {
      return {
        rows: [],
        total: 0,
        pageSize: pageSize,
        pageIndex: page
      }
    }
  } catch (error) {
    console.error('获取附加价列表失败:', error)
    return {
      rows: [],
      total: 0,
      pageSize: pageSize,
      pageIndex: page
    }
  }
}

// 新增附加价
async function apiAddSurcharge(data: any) {
  try {
    return await addSurcharge(data)
  } catch (error) {
    console.error('新增附加价失败:', error)
    throw error
  }
}

// 编辑附加价
async function apiUpdateSurcharge(data: any) {
  try {
    return await updateSurcharge(data)
  } catch (error) {
    console.error('编辑附加价失败:', error)
    throw error
  }
}

// 删除附加价
async function apiDeleteSurcharge(id: number) {
  try {
    return await deleteSurcharge(id)
  } catch (error) {
    console.error('删除附加价失败:', error)
    throw error
  }
}

// 批量删除附加价
async function apiBatchDeleteSurcharge(ids: number[]) {
  try {
    return await batchDeleteSurcharge(ids)
  } catch (error) {
    console.error('批量删除附加价失败:', error)
    throw error
  }
}

// 更新附加价状态
async function apiUpdateSurchargeStatus(id: number, status: string) {
  try {
    return await updateSurchargeStatus(id, status)
  } catch (error) {
    console.error('更新附加价状态失败:', error)
    throw error
  }
}

// 获取客户用户列表
async function apiGetCustomerUserList() {
  try {
    const response = await getCustomerUserList()
    return response.data || []
  } catch (error) {
    console.error('获取客户用户列表失败:', error)
    ElMessage.error('获取客户用户列表失败')
    return []
  }
}

// 保存附加价（统一接口）
async function apiSaveSurcharge(row: SurchargeRow) {
  try {
    if (row.id) {
      return await apiUpdateSurcharge(row)
    } else {
      return await apiAddSurcharge(row)
    }
  } catch (error) {
    console.error('保存附加价失败:', error)
    throw error
  }
}

/** —— 事件与逻辑 —— */
function goBack() {
  history.back()
}

async function loadBaseOptions() {
  loading.form = true
  try {
    const [refs, designs, customers] = await Promise.all([
      loadRefractions(),
      loadDesigns(),
      loadCustomers(),
    ])
    
    // 加载完选项数据后，重新加载价格列表以显示正确的名称
    await loadPriceList()
  } finally {
    loading.form = false
  }
}

// 加载折射率选项
async function loadRefractions() {
  loading.refractions = true
  try {
    const refs = await apiGetOptions(5)
    options.refractions = refs
    return refs
  } catch (error) {
    console.error('加载折射率选项失败:', error)
    ElMessage.error('加载折射率选项失败')
    return []
  } finally {
    loading.refractions = false
  }
}

// 加载设计选项
async function loadDesigns() {
  loading.designs = true
  try {
    const designs = await apiGetOptions(4)
    options.designs = designs
    return designs
  } catch (error) {
    console.error('加载设计选项失败:', error)
    ElMessage.error('加载设计选项失败')
    return []
  } finally {
    loading.designs = false
  }
}

// 加载客户用户列表
async function loadCustomers() {
  loading.customers = true
  try {
    const customers = await apiGetCustomerUserList()
    options.customers = customers
    return customers
  } catch (error) {
    console.error('加载客户用户列表失败:', error)
    ElMessage.error('加载客户用户列表失败')
    return []
  } finally {
    loading.customers = false
  }
}

// 刷新折射率选项
async function refreshRefractions() {
  await loadRefractions()
  await loadPriceList() // 刷新价格列表以更新名称映射
  ElMessage.success('折射率选项已刷新')
}

// 刷新设计选项
async function refreshDesigns() {
  await loadDesigns()
  await loadPriceList() // 刷新价格列表以更新名称映射
  ElMessage.success('设计选项已刷新')
}

async function onRefractionChange() {
  form.materialId = undefined
  options.materials = []
  if (!form.refractionId) return
  
  loading.materials = true
  try {
    const materials = await apiGetMaterialsByRefraction(form.refractionId)
    options.materials = materials
    
    // 自动选择默认材质
    const def = materials.find(m => m.is_default === 1)
    if (def) {
      form.materialId = def.id
      ElMessage.success(`已自动选择默认材质: ${def.optionName || def.option_name}`)
    } else if (materials.length === 1) {
      // 如果只有一个材质选项，自动选择
      form.materialId = materials[0].id
      ElMessage.success(`已自动选择材质: ${materials[0].optionName || materials[0].option_name}`)
    }
  } catch (error) {
    console.error('获取关联材质失败:', error)
    ElMessage.error('获取关联材质失败')
  } finally {
    loading.materials = false
  }
}

function resetForm() {
  form.id = 0
  form.refractionId = undefined
  form.materialId = undefined
  form.designId = undefined
  form.basePrice = ''
  form.currency = 'USD'
  form.remark = ''
  form.customerId = undefined
}

async function handleSave() {
  // @ts-ignore
  await formRef.value?.validate()
  loading.save = true
  try {
    const payload = {
      id: form.id || undefined,
      refractionId: form.refractionId,
      materialId: form.materialId,
      lensTypeId: 0, // 忽略镜片类型
      designId: form.designId,
      basePrice: Number(form.basePrice),
      currency: form.currency,
      status: 0, // 默认正常状态
      remark: form.remark,
      customerId: form.customerId,
    }
    await apiSaveVarietyPrice(payload)
    ElMessage.success('保存成功')
    formDlg.visible = false
    resetForm()
    loadPriceList()
  } catch (e: any) {
    ElMessage.error(e.message || '保存失败')
  } finally {
    loading.save = false
  }
}

function prefillForm(row: PriceRow) {
  form.id = row.id
  form.refractionId = row.refractionId
  onRefractionChange().then(() => {
    form.materialId = row.materialId
  })
  form.designId = row.designId
  form.basePrice = row.basePrice
  form.currency = row.currency || 'USD'
  form.remark = row.remark || ''
  form.customerId = (row as any).customerId
  formDlg.visible = true
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function openNewPrice() {
  resetForm()
  formDlg.visible = true
}

async function removeRow(id: number) {
  try {
    await apiRemoveVarietyPrice(id)
    ElMessage.success('已删除')
    loadPriceList()
  } catch (error) {
    ElMessage.error('删除失败')
  }
}

async function loadPriceList() {
  loading.list = true
  try {
    // 确保材质选项已加载
    if (options.materials.length === 0) {
      console.log('材质选项未加载，正在加载所有材质选项...')
      const allMaterials = await apiGetOptions(3) // optionType=3 是材质
      options.materials = allMaterials
      console.log('已加载所有材质选项:', allMaterials)
    }
    
    const data = await apiListVarietyPrice({
      pageIndex: table.page,
      pageSize: table.pageSize,
      RefractiveIndex: query.refractionId ?? 0,
    })
    
    // 处理数据，添加名称映射
    const rows = data?.rows ?? []
    const processedRows = rows.map((row) => {
      // 获取折射率名称
      const refraction = options.refractions.find(r => r.id === row.refractionId)
      row.refractionName = refraction?.optionName || refraction?.option_name || `折射率ID: ${row.refractionId}`
      
      // 获取材质名称 - 直接从全局材质选项中查找
      const material = options.materials.find(m => m.id === row.materialId)
      if (material) {
        row.materialName = material.optionName || material.option_name || `材质ID: ${row.materialId}`
        console.log(`找到材质 ${row.materialId}: ${row.materialName}`)
      } else {
        row.materialName = `材质ID: ${row.materialId}`
        console.log(`未找到材质ID ${row.materialId} 对应的材质`)
      }
      
      // 获取设计名称
      const design = options.designs.find(d => d.id === row.designId)
      row.designName = design?.optionName || design?.option_name || `设计ID: ${row.designId}`
      
      // 获取客户名称
      const customer = options.customers.find(c => c.dictValue === row.customerId)
      row.customerName = customer?.dictLabel || null
      
      return row
    })
    
    table.rows = processedRows
    table.total = data?.total ?? 0
  } finally {
    loading.list = false
  }
}

function onPageChange(p: number) {
  table.page = p
  loadPriceList()
}
function onQueryChange() {
  table.page = 1
  loadPriceList()
}
function resetQuery() {
  query.refractionId = undefined
  onQueryChange()
}

function openMaterialConfig() {
  ElMessage.info('请在“折射率选项”页面使用【配置材质】功能进行维护')
}

/** —— 附加价维护 —— */
const surcharge = reactive({
  type: OPTION_TYPE.Coating as number, // 默认显示膜层
  rows: [] as SurchargeRow[],
  page: 1,
  pageSize: 10,
  total: 0,
})

async function loadSurchargeList() {
  loading.surcharge = true
  try {
    const result = await apiListSurcharge(surcharge.type, surcharge.page, surcharge.pageSize)
    console.log('加载附加价列表结果:', result)
    
    // 设置分页信息
    surcharge.total = result.total
    surcharge.pageSize = result.pageSize
    
    // 为每个附加价项获取真实的选项名称
    const processedRows = await Promise.all(result.rows.map(async (row) => {
      try {
        // 根据optionId获取选项详情
        const optionResponse = await getLensOptionById(row.optionId)
        const responseData = optionResponse.data
        console.log(`选项${row.optionId}详情:`, responseData)
        
        // 从嵌套的productConfiguration对象中获取选项名称
        const productConfig = responseData?.productConfiguration
        const optionName = productConfig?.optionName
        
        if (!optionName) {
          console.warn(`选项${row.optionId}没有找到名称，响应数据:`, responseData)
        }
        
        // 获取客户名称
        const customer = options.customers.find(c => c.dictValue === row.customerId)
        const customerName = customer?.dictLabel || null
        
        return {
          ...row,
          optionName: optionName || `未知选项(${row.optionId})`,
          customerName: customerName
        }
      } catch (error) {
        console.error(`获取选项${row.optionId}详情失败:`, error)
        return {
          ...row,
          optionName: `获取失败(${row.optionId})`,
          customerName: null
        }
      }
    }))
    
    surcharge.rows = processedRows
    console.log('surcharge.rows 设置后:', surcharge.rows)
  } finally {
    loading.surcharge = false
    // 同步候选项
    if (addDlg.visible) {
      loadSurchargeCandidates()
    }
  }
}
function onSurchargeTypeChange() {
  surcharge.page = 1 // 切换类型时重置到第一页
  loadSurchargeList()
}

function onSurchargePageChange(page: number) {
  surcharge.page = page
  loadSurchargeList()
}
function onIncludedChange(row: SurchargeRow) {
  if (row.isIncluded === 1) {
    row.surcharge = 0
  }
}
async function saveSurcharge(row: SurchargeRow) {
  try {
    await apiSaveSurcharge(row)
    ElMessage.success('已保存')
    loadSurchargeList()
  } catch (error) {
    ElMessage.error('保存失败')
  }
}

// 编辑附加价
function editSurcharge(row: SurchargeRow) {
  // 打开编辑对话框，预填充数据
  addDlg.visible = true
  addDlg.form.optionId = row.optionId
  addDlg.form.isIncluded = row.isIncluded
  addDlg.form.surcharge = row.surcharge
  addDlg.form.customerId = (row as any).customerId
  // 标记为编辑模式
  addDlg.form.id = row.id
  loadSurchargeCandidates()
}

// 删除附加价
async function removeSurcharge(id: number) {
  try {
    await apiDeleteSurcharge(id)
    ElMessage.success('删除成功')
    loadSurchargeList()
  } catch (e: any) {
    ElMessage.error(e.message || '删除失败')
  }
}

/** —— 附加价：新增 —— */
type OptionCandidate = { 
  id: number
  option_name: string
  optionName?: string
}

const addDlg = reactive({
  visible: false,
  saving: false,
  candidates: [] as OptionCandidate[],
  form: {
    id: undefined as number | undefined,
    optionId: undefined as number | undefined,
    isIncluded: 0,
    surcharge: 0,
    customerId: undefined as number | undefined,
  }
})

/** Excel导入相关 */
const excelImportDlg = reactive({
  visible: false,
  isUploading: false,
  selectedFile: null as File | null
})

// 文件输入引用
const fileInputRef = ref()

function openAddSurcharge() {
  console.log('打开新增附加价对话框')
  addDlg.visible = true
  addDlg.form.id = undefined
  addDlg.form.optionId = undefined
  addDlg.form.isIncluded = 0
  addDlg.form.surcharge = 0
  addDlg.form.customerId = undefined
  loadSurchargeCandidates()
}

async function loadSurchargeCandidates() {
  try {
    // 拉取当前类型下的所有启用选项，过滤掉已存在于 surcharge.rows 的
    const all: OptionCandidate[] = await apiGetOptions(surcharge.type)
    
    // 确保 surcharge.rows 是数组
    const existingRows = Array.isArray(surcharge.rows) ? surcharge.rows : []
    const existed = new Set<number>(existingRows.map(r => r.optionId))
    addDlg.candidates = all.filter(x => !existed.has(x.id))
    console.log('加载候选项:', addDlg.candidates)
  } catch (error) {
    console.error('加载候选项失败:', error)
    addDlg.candidates = []
  }
}

function onAddTypeChange() {
  // 清空当前选项
  addDlg.form.optionId = undefined
  addDlg.form.isIncluded = 0
  addDlg.form.surcharge = 0
  addDlg.form.customerId = undefined
  
  // 同步切换列表和候选项
  loadSurchargeList()
  loadSurchargeCandidates()
}

async function apiCreateSurcharge(payload: { optionId:number; isIncluded:number; surcharge:number; customerId?:number }) {
  try {
    return await addSurcharge({
      optionId: payload.optionId,
      optionType: surcharge.type,
      isIncluded: payload.isIncluded,
      surcharge: payload.surcharge,
      customerId: payload.customerId,
      status: 0
    })
  } catch (error) {
    console.error('新增附加价失败:', error)
    throw error
  }
}

async function saveAddSurcharge() {
  if (!addDlg.form.optionId) {
    ElMessage.warning('请选择一个选项')
    return
  }
  addDlg.saving = true
  try {
    const payload = {
      id: addDlg.form.id,
      optionId: addDlg.form.optionId!,
      optionType: surcharge.type,
      isIncluded: addDlg.form.isIncluded,
      surcharge: Number(addDlg.form.surcharge) || 0,
      customerId: addDlg.form.customerId,
      status: 0
    }
    
    if (addDlg.form.id) {
      // 编辑模式
      await apiUpdateSurcharge(payload)
      ElMessage.success('编辑成功')
    } else {
      // 新增模式
      await apiCreateSurcharge(payload)
      ElMessage.success('新增成功')
    }
    
    addDlg.visible = false
    await loadSurchargeList()
  } catch (e:any) {
    ElMessage.error(e.message || '保存失败')
  } finally {
    addDlg.saving = false
  }
}

function gotoOptionsPage() {
  // 这里保持简单：返回上一页或根据你的路由跳转到选项管理
  // 例如：router.push('/DFGX/Options')；当前项目未注入 router，这里用历史返回
  ElMessage.info('请到"镜片选项管理"页面新增选项后再回来设置附加价')
}

/** —— Excel导入功能 —— */
// 打开品种价格Excel导入
function openExcelImport() {
  excelImportDlg.visible = true
  excelImportDlg.selectedFile = null
}

// 触发文件选择
function triggerFileSelect() {
  fileInputRef.value?.click()
}

// 处理文件选择
function handleFileSelect(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    validateAndSetFile(file)
  }
}

// 处理文件拖拽
function handleFileDrop(event: DragEvent) {
  const files = event.dataTransfer?.files
  if (files && files.length > 0) {
    validateAndSetFile(files[0])
  }
}

// 验证并设置文件
function validateAndSetFile(file: File) {
  // 验证文件类型
  const allowedTypes = ['.xlsx', '.xls']
  const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase()
  
  if (!allowedTypes.includes(fileExtension)) {
    ElMessage.error('只支持 .xlsx 和 .xls 格式的文件')
    return
  }
  
  // 验证文件大小 (10MB)
  if (file.size > 10 * 1024 * 1024) {
    ElMessage.error('文件大小不能超过 10MB')
    return
  }
  
  excelImportDlg.selectedFile = file
}

// 清除选择的文件
function clearSelectedFile() {
  excelImportDlg.selectedFile = null
  if (fileInputRef.value) {
    fileInputRef.value.value = ''
  }
}

// 文件上传进度
function handleFileUploadProgress(event: any) {
  excelImportDlg.isUploading = true
}

// 品种价格导入成功
function handlePriceImportSuccess(response: any, file: any) {
  excelImportDlg.isUploading = false
  
  if (response.code === 200) {
    const data = response.data
    
    // 如果有错误信息，显示错误对话框
    if (data.errors && data.errors.length > 0) {
      showImportErrorDialog(data.errors, data.Total)
      return
    }
    if (data.inserted == 0) { 
      ElMessage.error('导入失败! ' + data.message)
      return;
    }
    // 显示成功信息
    if (data.Inserted !== undefined) {
      ElMessage.success(`导入完成！总计 ${data.Total} 条，成功导入 ${data.inserted} 条，跳过 ${data.skipped} 条重复数据`)
      getVarietyPriceById();
    } else {
      ElMessage.success(`导入完成！总计 ${data.Total} 条数据`)
    }
    
    if (data.Message) {
      ElMessage.info(data.Message)
    }
    
    loadPriceList()
  } else {
    ElMessage.error('导入失败：' + (response.msg || '未知错误'))
  }
}


// 文件上传错误
function handleFileUploadError(error: any) {
  excelImportDlg.isUploading = false
  ElMessage.error('文件上传失败：' + error)
}

// 提交导入
async function submitImport() {
  if (!excelImportDlg.selectedFile) {
    ElMessage.warning('请先选择要导入的文件')
    return
  }
  
  excelImportDlg.isUploading = true
  
  try {
    const response = await importVarietyPrice(excelImportDlg.selectedFile)
    
    if (response.code === 200) {
      const data = response.data
      
      // 如果有错误信息，显示错误对话框
      if (data.errors && data.errors.length > 0) {
        showImportErrorDialog(data.errors, data.total)
        return;
      }
      
      // 显示成功信息
      if (data.inserted !== 0) {
        ElMessage.success(`导入完成！总计 ${data.total} 条，成功导入 ${data.inserted} 条，跳过 ${data.skipped} 条重复数据`)
        await getVarietyPriceById();
      } else {
        ElMessage.error(`导入失败！` + data.message)
        return;
      }
      
    } else {

      ElMessage.error('导入失败：' + (response.data.msg || '未知错误'))
      return;
    }
  } catch (error) {
    console.error('导入失败:', error)
    ElMessage.error('导入失败：' + (error.message || '未知错误'))
    return;
  } finally {
    excelImportDlg.isUploading = false
  }
}



// 显示导入错误对话框
function showImportErrorDialog(errors: string[], total: number) {
  const errorCount = errors.length
  
  // 创建HTML内容，每行错误单独显示
  const errorListHTML = errors.map((error, index) => 
    `<div class="error-item">
      <span class="error-number">${index + 1}.</span>
      <span class="error-message">${error}</span>
    </div>`
  ).join('')
  
  const content = `
    <div class="import-error-content">
      <div class="error-summary">
        导入过程中发现 <strong>${errorCount}</strong> 个错误，请修正后重新导入：
      </div>
      <div class="error-list" style="height: 300px; overflow-y: scroll; border: 1px solid #e4e7ed; border-radius: 4px; background-color: #fafafa; padding: 8px;">
        ${errorListHTML}
      </div>
    </div>
  `
  
  // 使用 ElMessageBox.confirm 而不是 alert，这样可以更好地控制样式
  ElMessageBox.confirm(
    content,
    '导入错误详情',
    {
      confirmButtonText: '我知道了',
      cancelButtonText: '',
      showCancelButton: false,
      type: 'error',
      dangerouslyUseHTMLString: true,
      customClass: 'import-error-dialog',
      showClose: true,
      closeOnClickModal: false,
      closeOnPressEscape: true,
      customStyle: {
        width: '1000px',
        maxWidth: '1000px',
        height: '430px',
        maxHeight: '80vh'
      }
    }
  ).then(() => {
    // 对话框关闭后，移除动态添加的样式
    const styleElement = document.getElementById('import-error-scroll-style')
    if (styleElement) {
      styleElement.remove()
    }
  }).catch(() => {
    // 用户点击确认或关闭
    const styleElement = document.getElementById('import-error-scroll-style')
    if (styleElement) {
      styleElement.remove()
    }
  })
  
  // 动态添加强制滚动条样式
  setTimeout(() => {
    const existingStyle = document.getElementById('import-error-scroll-style')
    if (!existingStyle) {
      const style = document.createElement('style')
      style.id = 'import-error-scroll-style'
      style.textContent = `
        .import-error-dialog .error-list {
          height: 300px !important;
          max-height: 300px !important;
          overflow-y: scroll !important;
          overflow-x: hidden !important;
          border: 1px solid #e4e7ed !important;
          border-radius: 4px !important;
          background-color: #fafafa !important;
          padding: 8px !important;
        }
        .import-error-dialog .error-list::-webkit-scrollbar {
          width: 10px !important;
        }
        .import-error-dialog .error-list::-webkit-scrollbar-track {
          background: #f1f1f1 !important;
          border-radius: 5px !important;
        }
        .import-error-dialog .error-list::-webkit-scrollbar-thumb {
          background: #409eff !important;
          border-radius: 5px !important;
          min-height: 20px !important;
        }
        .import-error-dialog .error-list::-webkit-scrollbar-thumb:hover {
          background: #337ecc !important;
        }
      `
      document.head.appendChild(style)
    }
  }, 100)
}

/** —— 工具 —— */
function formatMoney(val: number | string): string {
  const num = Number(val || 0)
  return num.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

onMounted(async () => {
  await loadBaseOptions()  // 这里已经包含了loadPriceList()
  await loadSurchargeList()
})
</script>

<style scoped>
.page-wrap {
  padding: 12px;
}
.mt16 {
  margin-top: 16px;
}
.mb12 {
  margin-bottom: 12px;
}
.ml8 {
  margin-left: 8px;
}
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.form-grid :deep(.el-form-item) {
  margin-right: 24px;
  margin-bottom: 14px;
}
.pager {
  margin-top: 12px;
  display: flex;
  justify-content: flex-end;
}

/* 下拉框选项样式 */
.option-remark {
  color: #909399;
  font-size: 12px;
  margin-left: 8px;
}

.default-tag {
  color: #67c23a;
  font-weight: 500;
  font-size: 12px;
}

/* 下拉框加载状态 */
.el-select .el-loading-mask {
  border-radius: 4px;
}

/* 表单项间距优化 */
.form-grid :deep(.el-form-item) {
  margin-bottom: 18px;
}

/* 链接按钮样式 */
.ml8 {
  margin-left: 8px;
}

/* 下拉框宽度自适应 */
.el-select {
  min-width: 200px;
}

/* Excel导入相关样式 */
.import-content {
  padding: 16px 0;
}

.mb16 {
  margin-bottom: 16px;
}

.el-upload__tip {
  margin-top: 8px;
  font-size: 12px;
  color: #606266;
}

.el-upload__tip .el-link {
  margin-left: 8px;
}

/* 文件上传区域样式 */
.upload-area {
  .upload-dragger {
    border: 2px dashed #d9d9d9;
    border-radius: 6px;
    width: 100%;
    height: 180px;
    text-align: center;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    transition: border-color 0.3s;
    
    &:hover {
      border-color: #409eff;
    }
    
    .el-icon--upload {
      font-size: 67px;
      color: #c0c4cc;
      margin-bottom: 16px;
    }
    
    .upload-text {
      color: #606266;
      font-size: 14px;
      margin-bottom: 8px;
    }
    
    .upload-tip {
      color: #909399;
      font-size: 12px;
    }
  }
  
  .selected-file {
    margin-top: 16px;
    padding: 12px;
    background-color: #f5f7fa;
    border-radius: 4px;
    display: flex;
    align-items: center;
    gap: 8px;
    
    .el-icon {
      color: #409eff;
    }
    
    span {
      flex: 1;
      color: #606266;
    }
  }
}

/* 导入错误对话框样式 - 强制滚动方案 */
:deep(.import-error-dialog) {
  .el-message-box {
    width: 1000px !important;
    max-width: 1000px !important;
    height: 600px !important;
    max-height: 80vh !important;
    display: block !important;
    position: relative !important;
  }
  
  .el-message-box__header {
    position: absolute !important;
    top: 0 !important;
    left: 0 !important;
    right: 0 !important;
    height: 60px !important;
    padding: 20px 20px 10px 20px !important;
    border-bottom: 1px solid #e4e7ed !important;
    background: white !important;
    z-index: 10 !important;
  }
  
  .el-message-box__title {
    color: #f56c6c !important;
    font-weight: bold !important;
    font-size: 16px !important;
  }
  
  .el-message-box__content {
    position: absolute !important;
    top: 60px !important;
    left: 0 !important;
    right: 0 !important;
    bottom: 80px !important;
    padding: 20px !important;
    overflow: hidden !important;
  }
  
  .el-message-box__btns {
    position: absolute !important;
    bottom: 0 !important;
    left: 0 !important;
    right: 0 !important;
    height: 80px !important;
    padding: 10px 20px 20px 20px !important;
    border-top: 1px solid #e4e7ed !important;
    background: white !important;
    z-index: 10 !important;
  }
  
  .import-error-content {
    height: 100% !important;
    display: flex !important;
    flex-direction: column !important;
    
    .error-summary {
      flex-shrink: 0 !important;
      margin-bottom: 12px !important;
      padding: 12px !important;
      background-color: #fef0f0 !important;
      border: 1px solid #fbc4c4 !important;
      border-radius: 4px !important;
      color: #f56c6c !important;
      font-size: 14px !important;
    }
    
    .error-list {
      flex: 1 !important;
      height: 300px !important; /* 固定高度 */
      max-height: 300px !important;
      overflow-y: scroll !important;
      overflow-x: hidden !important;
      border: 1px solid #e4e7ed !important;
      border-radius: 4px !important;
      background-color: #fafafa !important;
      
      /* 强制显示滚动条 */
      scrollbar-width: thin !important;
      scrollbar-color: #409eff #f1f1f1 !important;
      
      /* 自定义滚动条样式 */
      &::-webkit-scrollbar {
        width: 10px !important;
        height: 10px !important;
      }
      
      &::-webkit-scrollbar-track {
        background: #f1f1f1 !important;
        border-radius: 5px !important;
      }
      
      &::-webkit-scrollbar-thumb {
        background: #409eff !important;
        border-radius: 5px !important;
        min-height: 20px !important;
        
        &:hover {
          background: #337ecc !important;
        }
      }
      
      &::-webkit-scrollbar-corner {
        background: #f1f1f1 !important;
      }
    }
    
    .error-item {
      display: flex !important;
      align-items: flex-start !important;
      padding: 8px 12px !important;
      border-bottom: 1px solid #e4e7ed !important;
      font-size: 12px !important;
      line-height: 1.5 !important;
      white-space: nowrap !important;
      overflow: hidden !important;
      text-overflow: ellipsis !important;
      
      &:last-child {
        border-bottom: none !important;
      }
      
      &:nth-child(even) {
        background-color: #f8f9fa !important;
      }
      
      .error-number {
        color: #909399 !important;
        font-weight: bold !important;
        margin-right: 8px !important;
        min-width: 20px !important;
        flex-shrink: 0 !important;
        font-size: 11px !important;
      }
      
      .error-message {
        color: #303133 !important;
        flex: 1 !important;
        word-break: break-word !important;
        font-size: 12px !important;
        white-space: normal !important;
      }
    }
  }
  
  /* 小屏幕适配 */
  @media (max-height: 700px) {
    .el-message-box {
      height: 500px !important;
      max-height: 90vh !important;
    }
    
    .error-list {
      height: 250px !important;
      max-height: 250px !important;
    }
  }
  
  @media (max-height: 600px) {
    .el-message-box {
      height: 450px !important;
      max-height: 95vh !important;
    }
    
    .error-list {
      height: 200px !important;
      max-height: 200px !important;
    }
  }
  
  @media (max-height: 500px) {
    .el-message-box {
      height: 400px !important;
      max-height: 98vh !important;
    }
    
    .error-list {
      height: 150px !important;
      max-height: 150px !important;
    }
  }
}

/* 全局强制滚动条样式 - 针对导入错误对话框 */
:global(.import-error-dialog .error-list) {
  overflow-y: scroll !important;
  scrollbar-width: thin !important;
  scrollbar-color: #409eff #f1f1f1 !important;
}

:global(.import-error-dialog .error-list::-webkit-scrollbar) {
  width: 10px !important;
  height: 10px !important;
}

:global(.import-error-dialog .error-list::-webkit-scrollbar-track) {
  background: #f1f1f1 !important;
  border-radius: 5px !important;
}

:global(.import-error-dialog .error-list::-webkit-scrollbar-thumb) {
  background: #409eff !important;
  border-radius: 5px !important;
  min-height: 20px !important;
}

:global(.import-error-dialog .error-list::-webkit-scrollbar-thumb:hover) {
  background: #337ecc !important;
}
</style>