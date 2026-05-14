<template>
  <div class="app-container spare-parts-management">
    <el-form ref="queryRef" :model="queryParams" class="search-form" @submit.prevent>
      <el-form-item label="零配件名称" prop="name" class="query-form-item">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入零配件名称"
          clearable
          class="query-control"
          @input="handleInputQuery"
          @clear="handleQuery"
          @keyup.enter="handleQuery"
		  size="default" />
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" size="default">新增零配件</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="sparePartList" border stripe class="parts-table" :height="tableHeight">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" v-if="false"/>
      <el-table-column label="名称" prop="name" min-width="180" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="type" width="110" align="center" v-if="false">
        <template #default="{ row }">
          <el-tag :type="getTypeTagType(row.type)" effect="light">{{ formatTypeLabel(row.type) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="单位" prop="unit" width="90" align="center" v-if="false">
          <template #default="{ row }">
            {{ formatDictLabel(unitOptions, row.unit) }}
          </template>
        </el-table-column>
      <el-table-column label="规格" prop="specification" min-width="160" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.specification || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="当前库存" prop="stockQuantity" width="120" align="center">
        <template #default="{ row }">
          <span :class="getStockClass(row.stockQuantity)">{{ formatStock(row.stockQuantity) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="220" align="center">
        <template #default="{ row }">
          <div class="op-actions">
            <el-button text size="small" class="op-btn is-primary" @click="handleStockIn(row)">入库</el-button>
            <el-button text size="small" class="op-btn" @click="handleViewInboundRecords(row)">记录</el-button>
            <el-button text size="small" class="op-btn" @click="handleEdit(row)">编辑</el-button>
            <el-button text size="small" class="op-btn is-danger" @click="handleDelete(row)">删除</el-button>
          </div>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="handlePagination" />

    <el-dialog :title="dialogTitle" v-model="open" width="680px" append-to-body @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="90px" @submit.prevent>
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" maxlength="50"  show-word-limit placeholder="请输入零配件名称" />
        </el-form-item>
        <el-form-item label="单位" prop="unit">
          <el-select v-model="form.unit" placeholder="请选择单位" clearable class="form-control">
            <el-option v-for="item in unitOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="规格" prop="specification">
          <el-input v-model="form.specification" maxlength="100" show-word-limit placeholder="请输入规格型号" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" maxlength="200" show-word-limit :rows="3" placeholder="请输入备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="stockInOpen" title="零配件入库" width="460px" append-to-body @close="resetStockInForm">
      <el-form ref="stockInFormRef" :model="stockInForm" :rules="stockInRules" label-width="100px" @submit.prevent>
        <el-form-item label="零配件名称">
          <span class="readonly-text">{{ currentPart.name || '-' }}</span>
        </el-form-item>
        <el-form-item label="当前库存">
          <span class="readonly-text">{{ formatStock(currentPart.stockQuantity) }}</span>
        </el-form-item>
        <el-form-item label="入库数量" prop="quantity">
          <el-input-number v-model="stockInForm.quantity" :min="1" :precision="0" controls-position="right" class="form-control" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="stockInForm.remark" type="textarea" maxlength="200" show-word-limit :rows="3" placeholder="请输入入库备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="stockInOpen = false">取消</el-button>
        <el-button type="primary" :loading="stockInLoading" @click="submitStockIn">保存</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="recordOpen" :title="`入库记录 - ${currentPart.name || ''}`" width="860px" append-to-body @open="getRecordList">
      <el-table v-loading="recordLoading" :data="recordList" border class="record-table">
        <el-table-column label="入库时间" prop="inboundTime" width="180" align="center">
          <template #default="{ row }">
            {{ row.inboundTime || '-' }}
          </template>
        </el-table-column>
        <el-table-column label="入库数量" prop="quantity" width="120" align="center" />
        <el-table-column label="操作人" prop="operator" width="140" align="center">
          <template #default="{ row }">
            {{ formatOperator(row) }}
          </template>
        </el-table-column>
        <el-table-column label="备注" prop="remark" min-width="220" :show-overflow-tooltip="true">
          <template #default="{ row }">
            {{ row.remark || '-' }}
          </template>
        </el-table-column>
      </el-table>

      <pagination
        v-show="recordTotal > 0"
        :total="recordTotal"
        v-model:page="recordQuery.pageNum"
        v-model:limit="recordQuery.pageSize"
        @pagination="handleRecordPagination" />
      <template #footer>
        <el-button type="primary" @click="recordOpen = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="SparePartsManagement">
import { computed, getCurrentInstance, reactive, ref } from 'vue'
import { addInboundRecord, listInboundRecords } from '@/api/dcbl/inboundRecords'
import { addItem, deleteItems, editItem, listItems } from '@/api/dcbl/items'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const stockInFormRef = ref()
const queryDebounceTimer = ref()

const loading = ref(false)
const stockInLoading = ref(false)
const total = ref(0)
const open = ref(false)
const isEdit = ref(false)
const dialogTitle = ref('')

const stockInOpen = ref(false)
const recordOpen = ref(false)
const recordLoading = ref(false)
const recordTotal = ref(0)

const tableHeight = computed(() => 'calc(100vh - 290px)')

const queryParams = reactive({
  pageNum: 1,
  pageSize: 30,
  name: '',
  type: ''
})

const form = reactive({
  id: undefined,
  name: '',
  unit: '',
  specification: '',
  remark: ''
})

const stockInForm = reactive({
  quantity: 1,
  remark: ''
})

const recordQuery = reactive({
  pageNum: 1,
  pageSize: 30
})

const typeOptions = ref([])
const unitOptions = ref([])
const componentTypeValue = ref('')
const sparePartList = ref([])
const recordList = ref([])
const userOptions = ref([])
const currentPart = reactive({
  id: undefined,
  name: '',
  type: '',
  unit: '',
  specification: '',
  remark: '',
  stockQuantity: 0
})

const rules = {
  name: [{ required: true, message: '名称不能为空', trigger: 'blur' }],
  unit: [{ required: true, message: '请选择单位', trigger: 'change' }]
}

const stockInRules = {
  quantity: [{ required: true, message: '请输入入库数量', trigger: 'change' }]
}

function loadDicts() {
  return proxy.getDicts(['dc_material_unit', 'dc_material_classification', 'sql_all_user']).then((response) => {
    const source = response?.data?.data || response?.data?.result || response?.data || []
    const dicts = Array.isArray(source) ? source : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
    userOptions.value = getDictList(dicts, 'sql_all_user').map((item) => ({
      userId: item.dictValue,
      userName: item.dictLabel,
      nickName: item.dictLabel
    }))
    componentTypeValue.value = resolveComponentTypeValue()
    queryParams.type = componentTypeValue.value
  })
}

function getDictList(dicts, dictType) {
  const safeDicts = Array.isArray(dicts) ? dicts : []
  const matched = safeDicts.find((item) => item.dictType === dictType)
  return Array.isArray(matched?.list) ? matched.list : []
}

function formatDictLabel(optionsRef, value) {
  const options = Array.isArray(optionsRef?.value) ? optionsRef.value : Array.isArray(optionsRef) ? optionsRef : []
  const matched = options.find((item) => String(item.dictValue) === String(value))
  return matched?.dictLabel || value || '-'
}

function formatTypeLabel(value) {
  const label = formatDictLabel(typeOptions, value)
  if (label !== value) return label

  const typeMap = {
    accessory: '配件',
    component: '配件',
    part: '配件',
    product: '产品',
    finished: '产品'
  }
  return typeMap[value] || value || '-'
}

function getTypeTagType(value) {
  const label = formatTypeLabel(value)
  return label === '产品' ? 'success' : 'info'
}

function isComponentType(value) {
  const label = formatTypeLabel(value)
  return label === '配件' || String(value) === '1' || value === 'accessory' || value === 'component' || value === 'part'
}

function resolveComponentTypeValue() {
  const preferred = typeOptions.value.find((item) => isComponentType(item.dictValue))
  return preferred?.dictValue ?? ''
}

function formatStock(value) {
  return Number(value || 0)
}

function getStockClass(value) {
  const stock = Number(value || 0)
  return {
    'stock-danger': stock <= 0,
    'stock-warning': stock > 0 && stock < 10,
    'stock-normal': stock >= 10
  }
}

function getTableIndex(index) {
  return (queryParams.pageNum - 1) * queryParams.pageSize + index + 1
}

function getPageData(response) {
  const raw = response?.data || {}
  const data = raw?.data || raw?.result || raw
  const list = data?.result || data?.rows || data?.list || data?.items || []
  const totalNum = data?.totalNum ?? data?.total ?? data?.count ?? list.length

  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function getListData(response) {
  const raw = response?.data || {}
  const data = raw?.data || raw?.result || raw
  const list = data?.result || data?.rows || data?.list || data?.items || data
  return Array.isArray(list) ? list : []
}

function normalizeItem(item) {
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name ?? '',
    type: item.type ?? item.Type ?? '',
    unit: item.unit ?? item.Unit ?? '',
    stockQuantity: item.stockQuantity ?? item.StockQuantity ?? 0,
    specification: item.specification ?? item.Specification ?? item.spec ?? item.Spec ?? '',
    remark: item.remark ?? item.Remark ?? ''
  }
}

function normalizeInboundRecord(item) {
  return {
    id: item.id ?? item.Id,
    inboundTime: formatTime(item.inboundTime ?? item.InboundTime ?? item.createTime ?? item.CreateTime),
    quantity: Number(item.quantity ?? item.Quantity ?? 0),
    operator: item.operator ?? item.Operator ?? item.createBy ?? item.CreateBy ?? item.userId ?? item.UserId ?? '',
    operatorId: item.userId ?? item.UserId ?? item.createById ?? item.CreateById ?? '',
    remark: item.remark ?? item.Remark ?? ''
  }
}

function formatOperator(row) {
  const rawOperator = row?.operator
  const rawOperatorId = row?.operatorId
  const matched = userOptions.value.find(
    (item) =>
      String(item.userId) === String(rawOperatorId) ||
      String(item.userId) === String(rawOperator) ||
      String(item.userName) === String(rawOperator)
  )
  return matched?.nickName || matched?.userName || rawOperator || '-'
}

function formatTime(value) {
  if (!value) return ''
  if (typeof value !== 'string') return value
  return value.replace('T', ' ').slice(0, 19)
}

function getCurrentTime() {
  const now = new Date()
  const pad = (val) => String(val).padStart(2, '0')
  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())} ${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`
}

function getList() {
  loading.value = true
  listItems({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    name: queryParams.name.trim(),
    type: queryParams.type
  })
    .then((response) => {
      const pageData = getPageData(response)
      const normalizedList = pageData.list.map(normalizeItem)
      sparePartList.value = normalizedList.filter((item) => isComponentType(item.type))
      total.value = queryParams.type ? pageData.total : sparePartList.value.length
    })
    .finally(() => {
      loading.value = false
    })
}

function getRecordList() {
  if (!currentPart.id) return
  recordLoading.value = true
  const itemId = Number(currentPart.id) || currentPart.id
  listInboundRecords({
    pageNum: recordQuery.pageNum,
    pageSize: recordQuery.pageSize,
    itemId,
    ItemId: itemId
  })
    .then((response) => {
      const pageData = getPageData(response)
      let rows = pageData.list
      let totalCount = pageData.total

      // Fallback for non-standard pagination payloads.
      if (!rows.length) {
        const allRows = getListData(response)
        const filtered = allRows.filter((item) => {
          const rowItemId = item.itemId ?? item.ItemId
          return String(rowItemId) === String(itemId)
        })
        rows = filtered
        totalCount = filtered.length
      }

      recordList.value = rows.map(normalizeInboundRecord)
      recordTotal.value = totalCount
    })
    .finally(() => {
      recordLoading.value = false
    })
}

function handleQuery() {
  queryParams.pageNum = 1
  getList()
}

function handleInputQuery() {
  if (queryDebounceTimer.value) {
    clearTimeout(queryDebounceTimer.value)
  }
  queryDebounceTimer.value = setTimeout(() => {
    handleQuery()
  }, 250)
}

function handlePagination({ page, limit }) {
  queryParams.pageNum = page
  queryParams.pageSize = limit
  getList()
}

function handleRecordPagination({ page, limit }) {
  recordQuery.pageNum = page
  recordQuery.pageSize = limit
  getRecordList()
}

function resetForm() {
  Object.assign(form, {
    id: undefined,
    name: '',
    unit: '',
    specification: '',
    remark: ''
  })
  formRef.value?.clearValidate()
}

function handleAdd() {
  isEdit.value = false
  dialogTitle.value = '新增零配件'
  resetForm()
  open.value = true
}

function handleEdit(row) {
  isEdit.value = true
  dialogTitle.value = '编辑零配件'
  Object.assign(form, { ...row })
  formRef.value?.clearValidate()
  open.value = true
}

function cancel() {
  open.value = false
  resetForm()
}

function getSubmitData() {
  return {
    id: form.id,
    name: form.name.trim(),
    type: componentTypeValue.value,
    unit: form.unit,
    spec: form.specification.trim(),
    remark: form.remark.trim()
  }
}

function submitForm() {
  formRef.value?.validate((valid) => {
    if (!valid) return
    const submitApi = isEdit.value ? editItem : addItem
    submitApi(getSubmitData()).then(() => {
      proxy.$modal.msgSuccess(isEdit.value ? '修改成功' : '新增成功')
      open.value = false
      getList()
    })
  })
}

function handleDelete(row) {
  proxy.$modal
    .confirm(`是否确认删除"${row.name}"？`)
    .then(() => deleteItems([row.id]))
    .then(() => {
      const maxPage = Math.max(Math.ceil((total.value - 1) / queryParams.pageSize), 1)
      if (queryParams.pageNum > maxPage) {
        queryParams.pageNum = maxPage
      }
      proxy.$modal.msgSuccess('删除成功')
      getList()
    })
    .catch(() => {})
}

function handleStockIn(row) {
  Object.assign(currentPart, {
    id: row.id,
    name: row.name,
    type: row.type,
    unit: row.unit,
    specification: row.specification,
    remark: row.remark,
    stockQuantity: row.stockQuantity
  })
  resetStockInForm()
  stockInOpen.value = true
}

function resetStockInForm() {
  Object.assign(stockInForm, {
    quantity: 1,
    remark: ''
  })
  stockInFormRef.value?.clearValidate()
}

function submitStockIn() {
  stockInFormRef.value?.validate((valid) => {
    if (!valid) return
    if (Number(stockInForm.quantity) <= 0) {
      proxy.$modal.msgError('入库数量必须大于 0')
      return
    }

    const quantity = Number(stockInForm.quantity)
    const payload = {
      ItemId: currentPart.id,
      Quantity: quantity,
      InboundTime: getCurrentTime(),
      Remark: stockInForm.remark.trim()
    }
    stockInLoading.value = true
    addInboundRecord(payload)
      .then(() => {
        proxy.$modal.msgSuccess('入库成功')
        stockInOpen.value = false
        getList()
      })
      .finally(() => {
        stockInLoading.value = false
      })
  })
}

function handleViewInboundRecords(row) {
  Object.assign(currentPart, {
    id: row.id,
    name: row.name,
    type: row.type,
    unit: row.unit,
    specification: row.specification,
    remark: row.remark,
    stockQuantity: row.stockQuantity
  })
  recordQuery.pageNum = 1
  recordOpen.value = true
}

loadDicts().then(() => {
  resetForm()
  getList()
})
</script>

<style scoped>
.spare-parts-management {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
  width: 100%;
}

.query-form-item {
  width: 100%;
  margin-right: 0;
}

.query-control {
  width: 100%;
}

.parts-table,
.record-table {
  width: 100%;
}

.parts-table :deep(.el-table__header-wrapper th),
.record-table :deep(.el-table__header-wrapper th) {
  background: #f2f6fc;
  color: #303133;
}

.parts-table :deep(.el-table__header-wrapper th .cell),
.record-table :deep(.el-table__header-wrapper th .cell) {
  height: 20px;
  line-height: 20px;
  padding-top: 0;
  padding-bottom: 0;
  font-size: 11px;
}

.parts-table :deep(.el-table__body-wrapper td),
.record-table :deep(.el-table__body-wrapper td) {
  padding-top: 0;
  padding-bottom: 0;
}

.parts-table :deep(.el-table__body-wrapper td .cell),
.record-table :deep(.el-table__body-wrapper td .cell) {
  height: 24px;
  line-height: 24px;
  padding-top: 0;
  padding-bottom: 0;
}

.form-control {
  width: 100%;
}

.readonly-text {
  display: inline-block;
  width: 100%;
  color: #303133;
  line-height: 28px;
}

.op-actions {
  display: flex;
  flex-wrap: nowrap;
  justify-content: center;
  align-items: center;
  white-space: nowrap;
  gap: 0;
  line-height: 20px;
}

.op-btn {
  margin: 0;
  padding: 0 4px;
  min-height: 20px;
}

.op-btn + .op-btn {
  margin-left: 2px;
}

.op-btn.is-primary {
  color: #409eff;
}

.op-btn.is-danger {
  color: #f56c6c;
}

.stock-danger {
  color: #f56c6c;
  font-weight: 700;
}

.stock-warning {
  color: #e6a23c;
  font-weight: 700;
}

.stock-normal {
  color: #67c23a;
  font-weight: 700;
}
</style>