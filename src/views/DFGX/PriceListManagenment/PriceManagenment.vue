<template>
  <div class="page-wrap">
    <el-page-header content="价格列表管理（品种主价 + 附加价）" @back="goBack" />
    <!-- ====== 对话框：新增 / 维护品种价格 ====== -->
    <el-dialog v-model="formDlg.visible" title="新增 / 维护品种价格" width="720px" :close-on-click-modal="false">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="110px" class="form-grid" :disabled="loading.form">
        <el-form-item label="折射率" prop="refractionId">
          <el-select v-model="form.refractionId" placeholder="请选择折射率" filterable @change="onRefractionChange" style="width: 240px">
            <el-option v-for="o in options.refractions" :key="o.id" :label="o.option_name" :value="o.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="材质" prop="materialId">
          <el-select v-model="form.materialId" placeholder="请先选择折射率" :disabled="materialsDisabled" filterable style="width: 260px">
            <el-option v-for="m in options.materials" :key="m.id" :label="m.option_name + (m.is_default ? '（默认）' : '')" :value="m.id" />
          </el-select>
          <el-link type="primary" class="ml8" @click="openMaterialConfig" :underline="false">配置材质</el-link>
        </el-form-item>

        <el-form-item label="镜片类型" prop="lensTypeId">
          <el-select v-model="form.lensTypeId" placeholder="请选择镜片类型" filterable style="width: 240px">
            <el-option v-for="o in options.lensTypes" :key="o.id" :label="o.option_name" :value="o.id" />
          </el-select>
        </el-form-item>

        <el-form-item label="设计" prop="designId">
          <el-select v-model="form.designId" placeholder="请选择设计" filterable style="width: 240px">
            <el-option v-for="o in options.designs" :key="o.id" :label="o.option_name" :value="o.id" />
          </el-select>
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
        <el-form-item label="镜片类型">
          <el-select v-model="query.lensTypeId" clearable placeholder="全部" style="width: 180px" @change="onQueryChange">
            <el-option v-for="o in options.lensTypes" :key="o.id" :label="o.option_name" :value="o.id" />
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
        <el-table-column prop="lensTypeName" label="镜片类型" width="150" />
        <el-table-column prop="designName" label="设计" width="180" />
        <el-table-column prop="base_price" label="基础价格" width="140">
          <template #default="{ row }">
            <span>{{ row.currency || 'USD' }} {{ formatMoney(row.base_price) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === '0' ? 'success' : 'info'">{{ row.status === '0' ? '正常' : '停用' }}</el-tag>
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
        <el-table-column prop="option_name" label="选项名称" />
        <el-table-column label="默认包含" width="120">
          <template #default="{ row }">
            <el-switch v-model="row.is_included" :active-value="1" :inactive-value="0" @change="onIncludedChange(row)" />
          </template>
        </el-table-column>
        <el-table-column label="附加价" width="180">
          <template #default="{ row }">
            <el-input-number v-model="row.surcharge" :precision="2" :step="1" :min="0" :disabled="row.is_included===1" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="saveSurcharge(row)">保存</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-dialog v-model="addDlg.visible" title="新增附加项" width="520px">
        <el-form :model="addDlg.form" label-width="100px">
          <el-form-item label="类型">
            <el-segmented v-model="surcharge.type" :options="[{label:'膜层', value: OPTION_TYPE.Coating},{label:'额外处理', value: OPTION_TYPE.Extra}]" @change="onAddTypeChange" />
          </el-form-item>
          <el-form-item label="选项">
            <el-select v-model="addDlg.form.option_id" filterable placeholder="请选择选项" style="width: 320px">
              <el-option v-for="opt in addDlg.candidates" :key="opt.id" :label="opt.option_name" :value="opt.id" />
            </el-select>
            <el-link class="ml8" type="primary" :underline="false" @click="gotoOptionsPage">去选项页新增</el-link>
          </el-form-item>
          <el-form-item label="默认包含">
            <el-switch v-model="addDlg.form.is_included" :active-value="1" :inactive-value="0" @change="val => { if(val===1) addDlg.form.surcharge = 0 }" />
          </el-form-item>
          <el-form-item label="附加价">
            <el-input-number v-model="addDlg.form.surcharge" :precision="2" :step="1" :min="0" :disabled="addDlg.form.is_included===1" />
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
  Refraction: 1,
  Material: 2,
  LensType: 3,
  Design: 4,
  Coating: 5,
  Extra: 6,
}

type OptionItem = { id: number; option_name: string; is_default?: number }
type PriceRow = {
  id: number
  refraction_id: number
  material_id: number
  lens_type_id: number
  design_id: number
  base_price: number
  currency: string
  status: string
  remark?: string
  refractionName?: string
  materialName?: string
  lensTypeName?: string
  designName?: string
}

type SurchargeRow = {
  option_id: number
  option_name: string
  is_included: number
  surcharge: number
}

const loading = reactive({ form: false, list: false, save: false, surcharge: false })
const options = reactive({
  refractions: [] as OptionItem[],
  materials: [] as OptionItem[],
  lensTypes: [] as OptionItem[],
  designs: [] as OptionItem[],
})

/** 新增/编辑表单 */
const formRef = ref()
const form = reactive({
  id: 0,
  refractionId: undefined as number | undefined,
  materialId: undefined as number | undefined,
  lensTypeId: undefined as number | undefined,
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
  lensTypeId: [{ required: true, message: '请选择镜片类型', trigger: 'change' }],
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
  lensTypeId: undefined as number | undefined,
})

const materialsDisabled = computed(() => !form.refractionId)
const canSubmit = computed(() =>
  !!(form.refractionId && form.materialId && form.lensTypeId && form.designId && String(form.basePrice).length > 0)
)

const comboPreview = computed(() => {
  const rn = options.refractions.find(x => x.id === form.refractionId)?.option_name
  const mn = options.materials.find(x => x.id === form.materialId)?.option_name
  const lt = options.lensTypes.find(x => x.id === form.lensTypeId)?.option_name
  const dn = options.designs.find(x => x.id === form.designId)?.option_name
  if (rn && mn && lt && dn) {
    return `${rn} × ${mn} × ${lt} × ${dn} → 将保存为一条品种价格`
  }
  return ''
})

/** —— API 占位（按需替换为项目内 request 封装） —— */
async function apiGetOptions(type: number) {
  const resp = await fetch(`/api/options?type=${type}&status=0`)
  const data = await resp.json()
  return data?.data ?? []
}
async function apiGetMaterialsByRefraction(refractionId: number) {
  const resp = await fetch(`/api/refractions/${refractionId}/materials`)
  const data = await resp.json()
  if (data?.list) return data.list
  const mats: OptionItem[] = await apiGetOptions(OPTION_TYPE.Material)
  const set = new Set<number>(data?.materialIds ?? [])
  const def = data?.defaultMaterialId
  return mats.filter(m => set.has(m.id)).map(m => ({ ...m, is_default: Number(m.id === def) }))
}
async function apiSaveVarietyPrice(payload: any) {
  return fetch('/api/variety-price', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) })
}
async function apiListVarietyPrice(params: any) {
  const usp = new URLSearchParams(params)
  const resp = await fetch(`/api/variety-price?${usp.toString()}`)
  const data = await resp.json()
  return data
}
async function apiRemoveVarietyPrice(id: number) {
  return fetch(`/api/variety-price/${id}`, { method: 'DELETE' })
}
async function apiListSurcharge(type: number) {
  const resp = await fetch(`/api/surcharge?type=${type}`)
  const data = await resp.json()
  return data?.rows ?? []
}
async function apiSaveSurcharge(row: SurchargeRow) {
  return fetch(`/api/surcharge/${row.option_id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ is_included: row.is_included, surcharge: Number(row.surcharge) }),
  })
}

/** —— 事件与逻辑 —— */
function goBack() {
  history.back()
}

async function loadBaseOptions() {
  loading.form = true
  try {
    const [refs, lensTypes, designs] = await Promise.all([
      apiGetOptions(OPTION_TYPE.Refraction),
      apiGetOptions(OPTION_TYPE.LensType),
      apiGetOptions(OPTION_TYPE.Design),
    ])
    options.refractions = refs
    options.lensTypes = lensTypes
    options.designs = designs
  } finally {
    loading.form = false
  }
}

async function onRefractionChange() {
  form.materialId = undefined
  options.materials = []
  if (!form.refractionId) return
  options.materials = await apiGetMaterialsByRefraction(form.refractionId)
  const def = options.materials.find(m => m.is_default === 1)
  if (def) form.materialId = def.id
}

function resetForm() {
  form.id = 0
  form.refractionId = undefined
  form.materialId = undefined
  form.lensTypeId = undefined
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
      refraction_id: form.refractionId,
      material_id: form.materialId,
      lens_type_id: form.lensTypeId,
      design_id: form.designId,
      base_price: Number(form.basePrice),
      currency: form.currency,
      remark: form.remark,
    }
    const resp = await apiSaveVarietyPrice(payload)
    if (!resp.ok) throw new Error('保存失败')
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
  form.refractionId = row.refraction_id
  onRefractionChange().then(() => {
    form.materialId = row.material_id
  })
  form.lensTypeId = row.lens_type_id
  form.designId = row.design_id
  form.basePrice = row.base_price
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
  const resp = await apiRemoveVarietyPrice(id)
  if (!resp.ok) return ElMessage.error('删除失败')
  ElMessage.success('已删除')
  loadPriceList()
}

async function loadPriceList() {
  loading.list = true
  try {
    const data = await apiListVarietyPrice({
      page: table.page,
      pageSize: table.pageSize,
      refractionId: query.refractionId ?? '',
      lensTypeId: query.lensTypeId ?? '',
    })
    table.rows = data?.rows ?? []
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
  query.lensTypeId = undefined
  onQueryChange()
}

function openMaterialConfig() {
  ElMessage.info('请在“折射率选项”页面使用【配置材质】功能进行维护')
}

/** —— 附加价维护 —— */
const surcharge = reactive({
  type: OPTION_TYPE.Coating as number, // 默认显示膜层
  rows: [] as SurchargeRow[],
})

async function loadSurchargeList() {
  loading.surcharge = true
  try {
    surcharge.rows = await apiListSurcharge(surcharge.type)
  } finally {
    loading.surcharge = false
    // 同步候选项
    if (addDlg.visible) {
      loadSurchargeCandidates()
    }
  }
}
function onSurchargeTypeChange() {
  loadSurchargeList()
}
function onIncludedChange(row: SurchargeRow) {
  if (row.is_included === 1) {
    row.surcharge = 0
  }
}
async function saveSurcharge(row: SurchargeRow) {
  const resp = await apiSaveSurcharge(row)
  if (!resp.ok) {
    ElMessage.error('保存失败')
    return
  }
  ElMessage.success('已保存')
  loadSurchargeList()
}

/** —— 附加价：新增 —— */
type OptionCandidate = { id:number; option_name: string }

const addDlg = reactive({
  visible: false,
  saving: false,
  candidates: [] as OptionCandidate[],
  form: {
    option_id: undefined as number | undefined,
    is_included: 0,
    surcharge: 0,
  }
})

function openAddSurcharge() {
  addDlg.visible = true
  addDlg.form.option_id = undefined
  addDlg.form.is_included = 0
  addDlg.form.surcharge = 0
  loadSurchargeCandidates()
}

async function loadSurchargeCandidates() {
  // 拉取当前类型下的所有启用选项，过滤掉已存在于 surcharge.rows 的
  const all: OptionCandidate[] = await apiGetOptions(surcharge.type)
  const existed = new Set<number>(surcharge.rows.map(r => r.option_id))
  addDlg.candidates = all.filter(x => !existed.has(x.id))
}

function onAddTypeChange() {
  // 同步切换列表和候选项
  loadSurchargeList()
  loadSurchargeCandidates()
}

async function apiCreateSurcharge(payload: { option_id:number; is_included:number; surcharge:number }) {
  return fetch('/api/surcharge', {
    method: 'POST',
    headers: { 'Content-Type':'application/json' },
    body: JSON.stringify(payload)
  })
}

async function saveAddSurcharge() {
  if (!addDlg.form.option_id) {
    ElMessage.warning('请选择一个选项')
    return
  }
  addDlg.saving = true
  try {
    const resp = await apiCreateSurcharge({
      option_id: addDlg.form.option_id!,
      is_included: addDlg.form.is_included,
      surcharge: Number(addDlg.form.surcharge) || 0
    })
    if (!resp.ok) throw new Error('保存失败')
    ElMessage.success('已新增')
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
  await loadBaseOptions()
  await loadPriceList()
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
</style>