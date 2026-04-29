<template>
  <div class="app-container outbound-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="配件/产品" prop="itemId">
        <el-select v-model="queryParams.itemId" placeholder="请选择配件/产品" clearable filterable class="query-control" @change="handleQuery">
          <el-option v-for="item in itemOptions" :key="item.value" :label="formatItemOptionLabel(item)" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd">新增出库</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="outboundList" border class="outbound-table">
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
      <el-table-column label="出库数量" prop="quantity" width="120" align="center" />
      <el-table-column label="用途" prop="usageType" min-width="140" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ formatDictLabel(usageTypeOptions, row.usageType) }}
        </template>
      </el-table-column>
      <el-table-column label="出库时间" prop="outboundTime" width="180" align="center" />
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="180" align="center" fixed="right">
        <template #default="{ row }">
          <el-button text size="small" icon="View" @click="handleView(row)">查看</el-button>
          <el-button text size="small" icon="Edit" @click="handleEdit(row)">编辑</el-button>
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

    <el-dialog :title="dialogTitle" v-model="open" width="560px" append-to-body @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="配件/产品" prop="itemId">
          <el-select v-model="form.itemId" placeholder="请选择配件/产品" filterable class="form-select">
            <el-option v-for="item in itemOptions" :key="item.value" :label="formatItemOptionLabel(item)" :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="数量" prop="quantity">
          <el-input-number v-model="form.quantity" :min="1" :precision="0" :controls-position="'right'" class="form-number" />
        </el-form-item>
        <el-form-item label="用途" prop="usageType">
          <el-select
            v-model="form.usageType"
            placeholder="请选择或输入用途"
            filterable
            allow-create
            default-first-option
            clearable
            class="form-select">
            <el-option v-for="item in usageTypeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="出库时间" prop="outboundTime">
          <el-date-picker
            v-model="form.outboundTime"
            type="datetime"
            value-format="YYYY-MM-DD HH:mm:ss"
            placeholder="请选择出库时间"
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

    <el-dialog title="出库详情" v-model="detailOpen" width="520px" append-to-body>
      <el-descriptions :column="1" border>
        <el-descriptions-item label="名称">{{ detailRecord.name }}</el-descriptions-item>
        <el-descriptions-item label="类型">{{ formatTypeLabel(detailRecord.type) }}</el-descriptions-item>
        <el-descriptions-item label="单位">{{ formatUnitLabel(detailRecord.unit) }}</el-descriptions-item>
        <el-descriptions-item label="出库数量">{{ detailRecord.quantity }}</el-descriptions-item>
        <el-descriptions-item label="用途">{{ formatDictLabel(usageTypeOptions, detailRecord.usageType) }}</el-descriptions-item>
        <el-descriptions-item label="出库时间">{{ detailRecord.outboundTime }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ detailRecord.remark || '-' }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button type="primary" @click="detailOpen = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="OutboundManagement">
import { getCurrentInstance, reactive, ref } from 'vue'
import {
  addOutboundRecord,
  deleteOutboundRecords,
  editOutboundRecord,
  getOutboundRecord,
  listOutboundItemOptions,
  listOutboundRecords
} from '@/api/dcbl/outboundRecords'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const open = ref(false)
const detailOpen = ref(false)
const loading = ref(false)
const dialogTitle = ref('')
const isEdit = ref(false)
const total = ref(0)
const typeOptions = ref([])
const unitOptions = ref([])
const usageTypeOptions = ref([])
const itemOptions = ref([])
const outboundList = ref([])

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  itemId: undefined
})

const form = reactive({
  id: undefined,
  itemId: undefined,
  quantity: 1,
  usageType: '',
  outboundTime: '',
  remark: ''
})

const detailRecord = reactive({
  id: undefined,
  itemId: undefined,
  name: '',
  type: '',
  unit: '',
  quantity: 0,
  usageType: '',
  outboundTime: '',
  remark: ''
})

const rules = {
  itemId: [{ required: true, message: '请选择配件/产品', trigger: 'change' }],
  quantity: [
    { required: true, message: '数量不能为空', trigger: 'blur' },
    { validator: validatePositiveQuantity, trigger: 'change' }
  ],
  outboundTime: [{ required: true, message: '请选择出库时间', trigger: 'change' }]
}

function loadDicts() {
  proxy.getDicts(['dc_material_unit', 'dc_material_classification', 'dc_usagetype']).then((response) => {
    const dicts = Array.isArray(response.data) ? response.data : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
    usageTypeOptions.value = getDictList(dicts, 'dc_usagetype')
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

  callback(new Error('数量必须为正数'))
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

function normalizeOutboundRecord(record) {
  const itemId = record.itemId ?? record.ItemId
  const matchedItem = getItemOption(itemId) || {}

  return {
    id: record.id ?? record.Id,
    itemId,
    name: record.name ?? record.Name ?? matchedItem.name ?? '-',
    type: record.type ?? record.Type ?? matchedItem.type ?? '',
    unit: record.unit ?? record.Unit ?? matchedItem.unit ?? '',
    quantity: Number(record.quantity ?? record.Quantity ?? 0),
    usageType: record.usageType ?? record.UsageType ?? '',
    outboundTime: formatTime(record.outboundTime ?? record.OutboundTime),
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
  listOutboundRecords({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    itemId: queryParams.itemId
  })
    .then((response) => {
      const pageData = getPageData(response)
      outboundList.value = pageData.list.map(normalizeOutboundRecord)
      total.value = pageData.total
    })
    .finally(() => {
      loading.value = false
    })
}

function getItemOptions() {
  return listOutboundItemOptions().then((response) => {
    itemOptions.value = getResponseList(response).map(normalizeItemOption)
  })
}

function getSubmitData() {
  return {
    Id: form.id,
    ItemId: form.itemId,
    Quantity: Number(form.quantity),
    UsageType: String(form.usageType || '').trim(),
    OutboundTime: form.outboundTime,
    Remark: form.remark.trim()
  }
}

function resetForm() {
  Object.assign(form, {
    id: undefined,
    itemId: undefined,
    quantity: 1,
    usageType: '',
    outboundTime: getCurrentTime(),
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
  isEdit.value = false
  dialogTitle.value = '新增出库记录'
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

    const request = isEdit.value ? editOutboundRecord : addOutboundRecord
    request(getSubmitData()).then(() => {
      proxy.$modal.msgSuccess(isEdit.value ? '编辑出库成功' : '新增出库成功')
      open.value = false
      resetForm()
      getItemOptions().then(getList)
    })
  })
}

function handleView(row) {
  getOutboundRecord(row.id).then((response) => {
    Object.assign(detailRecord, normalizeOutboundRecord(getResponseData(response) || row))
    detailOpen.value = true
  })
}

function handleEdit(row) {
  getOutboundRecord(row.id).then((response) => {
    const record = normalizeOutboundRecord(getResponseData(response) || row)
    isEdit.value = true
    dialogTitle.value = '编辑出库记录'
    Object.assign(form, {
      id: record.id,
      itemId: record.itemId,
      quantity: record.quantity,
      usageType: record.usageType || '',
      outboundTime: record.outboundTime || getCurrentTime(),
      remark: record.remark || ''
    })
    formRef.value?.clearValidate()
    getItemOptions()
    open.value = true
  })
}

function handleDelete(row) {
  proxy.$modal
    .confirm(`是否确认删除"${row.name}"的出库记录？`)
    .then(() => {
      return deleteOutboundRecords([row.id])
    })
    .then(() => {
      const maxPage = Math.max(Math.ceil((total.value - 1) / queryParams.pageSize), 1)
      if (queryParams.pageNum > maxPage) {
        queryParams.pageNum = maxPage
      }
      proxy.$modal.msgSuccess('删除出库成功')
      getItemOptions().then(getList)
    })
    .catch(() => {})
}

loadDicts()
resetForm()
getItemOptions().then(getList)
</script>

<style scoped>
.outbound-management {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-control {
  width: 220px;
}

.outbound-table {
  width: 100%;
}

.form-select,
.form-number {
  width: 100%;
}
</style>