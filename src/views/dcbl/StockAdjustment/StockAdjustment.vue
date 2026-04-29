<template>
  <div class="app-container stock-adjustment">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="配件/产品" prop="itemId">
        <el-select v-model="queryParams.itemId" placeholder="请选择配件/产品" clearable filterable class="query-control" @change="handleQuery">
          <el-option v-for="item in itemOptions" :key="item.value" :label="formatItemOptionLabel(item)" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="调整类型" prop="adjustType">
        <el-select v-model="queryParams.adjustType" placeholder="请选择调整类型" clearable class="query-control" @change="handleQuery">
          <el-option v-for="item in adjustmentTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd">新增调整</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="adjustmentList" border class="adjustment-table">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" />
      <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="type" width="120" align="center">
        <template #default="{ row }">
          <el-tag :type="getTypeTagType(row.type)">{{ formatTypeLabel(row.type) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="单位" prop="unit" width="100" align="center">
        <template #default="{ row }">
          {{ formatUnitLabel(row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="调整类型" prop="adjustmentType" width="110" align="center">
        <template #default="{ row }">
          <el-tag :type="row.adjustmentType === 'increase' ? 'success' : 'danger'">
            {{ formatAdjustmentType(row.adjustmentType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="调整数量" prop="quantity" width="120" align="center" />
      <el-table-column label="调整原因" prop="reason" min-width="140" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ formatDictLabel(reasonOptions, row.reason) }}
        </template>
      </el-table-column>
      <el-table-column label="调整时间" prop="adjustmentTime" width="180" align="center" />
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="130" align="center" fixed="right">
        <template #default="{ row }">
          <el-button text size="small" icon="View" @click="handleView(row)">查看</el-button>
          <el-button text size="small" type="danger" icon="Delete" @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="handlePagination" />

    <el-dialog title="新增库存调整" v-model="open" width="560px" append-to-body @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="配件/产品" prop="itemId">
          <el-select v-model="form.itemId" placeholder="请选择配件/产品" filterable class="form-select">
            <el-option
              v-for="item in itemOptions"
              :key="item.value"
              :label="formatItemOptionLabel(item)"
              :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="调整类型" prop="adjustmentType">
          <el-radio-group v-model="form.adjustmentType">
            <el-radio-button label="increase">增加</el-radio-button>
            <el-radio-button label="decrease">减少</el-radio-button>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="调整数量" prop="quantity">
          <el-input-number v-model="form.quantity" :min="1" :precision="0" :controls-position="'right'" class="form-number" />
        </el-form-item>
        <el-form-item label="调整原因" prop="reason">
          <el-select v-model="form.reason" placeholder="请选择调整原因" clearable class="form-select">
            <el-option v-for="item in reasonOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="调整时间" prop="adjustmentTime">
          <el-date-picker
            v-model="form.adjustmentTime"
            type="datetime"
            value-format="YYYY-MM-DD HH:mm:ss"
            placeholder="请选择调整时间"
            class="form-select" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="200" show-word-limit :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog title="库存调整详情" v-model="detailOpen" width="520px" append-to-body>
      <el-descriptions :column="1" border>
        <el-descriptions-item label="名称">{{ detailRecord.name }}</el-descriptions-item>
        <el-descriptions-item label="类型">{{ formatTypeLabel(detailRecord.type) }}</el-descriptions-item>
        <el-descriptions-item label="单位">{{ formatUnitLabel(detailRecord.unit) }}</el-descriptions-item>
        <el-descriptions-item label="调整类型">{{ formatAdjustmentType(detailRecord.adjustmentType) }}</el-descriptions-item>
        <el-descriptions-item label="调整数量">{{ detailRecord.quantity }}</el-descriptions-item>
        <el-descriptions-item label="调整原因">{{ formatDictLabel(reasonOptions, detailRecord.reason) }}</el-descriptions-item>
        <el-descriptions-item label="调整时间">{{ detailRecord.adjustmentTime }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ detailRecord.remark || '-' }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button type="primary" @click="detailOpen = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="StockAdjustment">
import { getCurrentInstance, reactive, ref } from 'vue'
import {
  addStockAdjustment,
  deleteStockAdjustments,
  getStockAdjustment,
  listAdjustmentItemOptions,
  listStockAdjustments
} from '@/api/dcbl/stockAdjustments'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const open = ref(false)
const detailOpen = ref(false)
const loading = ref(false)
const total = ref(0)
const typeOptions = ref([])
const unitOptions = ref([])
const reasonOptions = ref([])
const itemOptions = ref([])
const adjustmentList = ref([])

const adjustmentTypeOptions = [
  { label: '增加', value: 'increase' },
  { label: '减少', value: 'decrease' }
]

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  itemId: undefined,
  adjustType: ''
})

const form = reactive({
  itemId: undefined,
  adjustmentType: 'increase',
  quantity: 1,
  reason: '',
  adjustmentTime: '',
  remark: ''
})

const detailRecord = reactive({
  id: undefined,
  itemId: undefined,
  name: '',
  type: '',
  unit: '',
  adjustmentType: '',
  quantity: 0,
  reason: '',
  adjustmentTime: '',
  remark: ''
})

const rules = {
  itemId: [{ required: true, message: '请选择物料/产品', trigger: 'change' }],
  adjustmentType: [{ required: true, message: '请选择调整类型', trigger: 'change' }],
  quantity: [
    { required: true, message: '调整数量不能为空', trigger: 'blur' },
    { validator: validatePositiveQuantity, trigger: 'change' }
  ],
  reason: [{ required: true, message: '请选择调整原因', trigger: 'change' }],
  adjustmentTime: [{ required: true, message: '请选择调整时间', trigger: 'change' }]
}

function loadDicts() {
  proxy.getDicts(['dc_material_unit', 'dc_material_classification', 'dc_inventory_adjustment']).then((response) => {
    const dicts = Array.isArray(response.data) ? response.data : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
    reasonOptions.value = getDictList(dicts, 'dc_inventory_adjustment')
  })
}

function getDictList(dicts, dictType) {
  const matched = dicts.find((item) => item.dictType === dictType)
  return matched?.list || []
}

function formatDictLabel(options, value) {
  const matched = options.find((item) => String(item.dictValue) === String(value))
  return matched?.dictLabel || value || '-'
}

function formatTypeLabel(value) {
  const label = formatDictLabel(typeOptions.value, value)
  if (label !== value) return label

  const typeMap = {
    accessory: '配件',
    finished: '成品',
    product: '成品'
  }
  return typeMap[value] || value || '-'
}

function formatUnitLabel(value) {
  return formatDictLabel(unitOptions.value, value)
}

function getTypeTagType(value) {
  const label = formatTypeLabel(value)
  return label === '成品' || value === 'finished' || value === 'product' ? 'success' : 'info'
}

function formatAdjustmentType(value) {
  const matched = adjustmentTypeOptions.find((item) => item.value === value)
  return matched?.label || value || '-'
}

function formatItemOptionLabel(item) {
  return `${item.name} / ${formatTypeLabel(item.type)} / ${formatUnitLabel(item.unit)} / 当前库存 ${item.stockQuantity}`
}

function getCurrentTime() {
  const now = new Date()
  const pad = (value) => String(value).padStart(2, '0')

  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())} ${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`
}

function validatePositiveQuantity(rule, value, callback) {
  if (Number(value) > 0) {
    callback()
    return
  }

  callback(new Error('调整数量必须为正数'))
}

function getTableIndex(index) {
  return (queryParams.pageNum - 1) * queryParams.pageSize + index + 1
}

function getResponseList(response) {
  const data = response?.data || {}
  const list = data.result || data.rows || data.list || data.items || data
  return Array.isArray(list) ? list : []
}

function getResponseData(response) {
  const data = response?.data
  return data?.result || data?.data || data
}

function getPageData(response) {
  const data = response?.data || {}
  const list = data.result || data.rows || data.list || data.items || []
  const totalNum = data.totalNum ?? data.total ?? data.count ?? list.length

  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function normalizeItemOption(item) {
  const value = item.value ?? item.id ?? item.Id
  const type = item.type ?? item.Type ?? ''
  const unit = item.unit ?? item.Unit ?? ''
  const stockQuantity = item.stockQuantity ?? item.StockQuantity ?? item.quantity ?? item.Quantity ?? 0
  const name = item.name ?? item.Name ?? ''

  return {
    value,
    name,
    type,
    unit,
    stockQuantity: Number(stockQuantity || 0)
  }
}

function getItemOption(itemId) {
  return itemOptions.value.find((item) => String(item.value) === String(itemId))
}

function normalizeAdjustmentRecord(record) {
  const itemId = record.itemId ?? record.ItemId
  const matchedItem = getItemOption(itemId) || {}

  return {
    id: record.id ?? record.Id,
    itemId,
    name: record.name ?? record.Name ?? matchedItem.name ?? '-',
    type: record.type ?? record.Type ?? matchedItem.type ?? '',
    unit: record.unit ?? record.Unit ?? matchedItem.unit ?? '',
    adjustmentType: record.adjustmentType ?? record.AdjustmentType ?? record.adjustType ?? record.AdjustType ?? '',
    quantity: Number(record.quantity ?? record.Quantity ?? 0),
    reason: record.reason ?? record.Reason ?? '',
    adjustmentTime: formatTime(record.adjustmentTime ?? record.AdjustmentTime ?? record.adjustTime ?? record.AdjustTime),
    remark: record.remark ?? record.Remark ?? ''
  }
}

function formatTime(value) {
  if (!value) return ''
  if (typeof value !== 'string') return value
  return value.replace('T', ' ').slice(0, 19)
}

function getList() {
  loading.value = true
  listStockAdjustments({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    itemId: queryParams.itemId,
    adjustType: queryParams.adjustType
  })
    .then((response) => {
      const pageData = getPageData(response)
      adjustmentList.value = pageData.list.map(normalizeAdjustmentRecord)
      total.value = pageData.total
    })
    .finally(() => {
      loading.value = false
    })
}

function getItemOptions() {
  return listAdjustmentItemOptions().then((response) => {
    itemOptions.value = getResponseList(response).map(normalizeItemOption)
  })
}

function getSubmitData() {
  return {
    ItemId: form.itemId,
    AdjustType: form.adjustmentType,
    Quantity: Number(form.quantity),
    Reason: String(form.reason || '').trim(),
    AdjustTime: form.adjustmentTime,
    Remark: form.remark.trim()
  }
}

function resetForm() {
  Object.assign(form, {
    itemId: undefined,
    adjustmentType: 'increase',
    quantity: 1,
    reason: '',
    adjustmentTime: getCurrentTime(),
    remark: ''
  })
  formRef.value?.clearValidate()
}

function handleQuery() {
  queryParams.pageNum = 1
  getList()
}

function resetQuery() {
  queryRef.value?.resetFields()
  handleQuery()
}

function handlePagination({ page, limit }) {
  queryParams.pageNum = page
  queryParams.pageSize = limit
  getList()
}

function handleAdd() {
  resetForm()
  getItemOptions()
  open.value = true
}

function cancel() {
  open.value = false
  resetForm()
}

function submitForm() {
  formRef.value.validate((valid) => {
    if (!valid) return

    addStockAdjustment(getSubmitData()).then(() => {
      proxy.$modal.msgSuccess('新增库存调整成功')
      open.value = false
      resetForm()
      getItemOptions().then(getList)
    })
  })
}

function handleView(row) {
  getStockAdjustment(row.id).then((response) => {
    Object.assign(detailRecord, normalizeAdjustmentRecord(getResponseData(response) || row))
    detailOpen.value = true
  })
}

function handleDelete(row) {
  proxy.$modal
    .confirm(`是否确认删除"${row.name}"的库存调整记录？`)
    .then(() => {
      return deleteStockAdjustments([row.id])
    })
    .then(() => {
      const maxPage = Math.max(Math.ceil((total.value - 1) / queryParams.pageSize), 1)
      if (queryParams.pageNum > maxPage) {
        queryParams.pageNum = maxPage
      }
      proxy.$modal.msgSuccess('删除库存调整成功')
      getItemOptions().then(getList)
    })
    .catch(() => {})
}

loadDicts()
resetForm()
getItemOptions().then(getList)
</script>

<style scoped>
.stock-adjustment {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-control {
  width: 220px;
}

.adjustment-table {
  width: 100%;
}

.form-select,
.form-number {
  width: 100%;
}
</style>