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
            <el-button type="primary" @click="openNewPrice">新增品种价格</el-button>
            <el-button :loading="loading.list" @click="loadPriceList">刷新</el-button>
          </div>
        </div>
      </template>

      <el-form :inline="true" class="mb12">
        <el-form-item label="折射率">
          <el-select v-model="query.refractionId" clearable placeholder="全部" style="width: 180px" @change="onQueryChange">
            <el-option v-for="o in options.refractions" :key="o.id" :label="o.option_name" :value="o.id" />
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
        </el-form>
        <template #footer>
          <el-button @click="addDlg.visible=false">取 消</el-button>
          <el-button type="primary" :loading="addDlg.saving" @click="saveAddSurcharge">保 存</el-button>
        </template>
      </el-dialog>
    </el-card>
  </div>
</template>

<script lang="ts" setup>
import { reactive, ref, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'

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
  designs: false
})
const options = reactive({
  refractions: [] as OptionItem[],
  materials: [] as OptionItem[],
  designs: [] as OptionItem[],
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
  const rn = options.refractions.find(x => x.id === form.refractionId)?.option_name
  const mn = options.materials.find(x => x.id === form.materialId)?.option_name
  const dn = options.designs.find(x => x.id === form.designId)?.option_name
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
  updateSurchargeStatus
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
    const materials = await getMaterialsByRefraction(refractionId)
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
    const [refs, designs] = await Promise.all([
      loadRefractions(),
      loadDesigns(),
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
      ElMessage.success(`已自动选择默认材质: ${def.option_name || def.optionName}`)
    } else if (materials.length === 1) {
      // 如果只有一个材质选项，自动选择
      form.materialId = materials[0].id
      ElMessage.success(`已自动选择材质: ${materials[0].option_name || materials[0].optionName}`)
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
      
      // 获取材质名称
      const material = options.materials.find(m => m.id === row.materialId)
      row.materialName = material?.optionName || material?.option_name || `材质ID: ${row.materialId}`
      
      // 获取设计名称
      const design = options.designs.find(d => d.id === row.designId)
      row.designName = design?.optionName || design?.option_name || `设计ID: ${row.designId}`
      
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
        
        return {
          ...row,
          optionName: optionName || `未知选项(${row.optionId})`
        }
      } catch (error) {
        console.error(`获取选项${row.optionId}详情失败:`, error)
        return {
          ...row,
          optionName: `获取失败(${row.optionId})`
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
  }
})

function openAddSurcharge() {
  console.log('打开新增附加价对话框')
  addDlg.visible = true
  addDlg.form.id = undefined
  addDlg.form.optionId = undefined
  addDlg.form.isIncluded = 0
  addDlg.form.surcharge = 0
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
  
  // 同步切换列表和候选项
  loadSurchargeList()
  loadSurchargeCandidates()
}

async function apiCreateSurcharge(payload: { optionId:number; isIncluded:number; surcharge:number }) {
  try {
    return await addSurcharge({
      optionId: payload.optionId,
      optionType: surcharge.type,
      isIncluded: payload.isIncluded,
      surcharge: payload.surcharge,
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
  ElMessage.info('请到“镜片选项管理”页面新增选项后再回来设置附加价')
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
</style>