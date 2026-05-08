<template>
  <div class="app-container inbound-outbound-page">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="搜索" prop="name">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入配件/产品名称"
          clearable
          class="query-control"
          @input="handleInputSearch"
          @keyup.enter="handleQuery"
          @clear="handleQuery" />
      </el-form-item>
    </el-form>

    <el-table v-loading="loading" :data="itemList" border class="item-table">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" />
      <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="type" width="120" align="center">
        <template #default="{ row }">
          <el-tag :type="getTypeTagType(row.type)">{{ formatTypeLabel(row.type) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="规格" prop="specification" min-width="160" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.specification || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="单位" prop="unit" width="100" align="center">
        <template #default="{ row }">
          {{ formatUnitLabel(row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="当前库存" prop="stockQuantity" width="120" align="center">
        <template #default="{ row }">
          {{ formatStock(row.stockQuantity) }}
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="260" align="center" fixed="right">
        <template #default="{ row }">
          <el-button text size="small" type="primary" icon="Download" @click="handleInbound(row)">入库</el-button>
          <el-button text size="small" type="warning" icon="Upload" @click="handleOutbound(row)">出库</el-button>
          <el-button text size="small" icon="Document" @click="handleViewRecords(row)">查看记录</el-button>
        </template>
      </el-table-column>
    </el-table>

    <div v-loading="loading" class="mobile-card-list">
      <el-empty v-if="!loading && itemList.length === 0" description="暂无数据" />
      <div v-for="item in itemList" :key="item.id" class="mobile-item-card">
        <div class="mobile-card-header">
          <div class="mobile-card-title">
            <span class="mobile-item-name">{{ item.name }}</span>
            <el-tag :type="getTypeTagType(item.type)" size="small" round>{{ formatTypeLabel(item.type) }}</el-tag>
          </div>
          <div class="mobile-stock">
            <span class="mobile-stock-value">{{ formatStock(item.stockQuantity) }}</span>
            <span class="mobile-stock-unit">{{ formatUnitLabel(item.unit) }}</span>
          </div>
        </div>
        <div class="mobile-card-content">
          <div class="mobile-info-row">
            <span class="mobile-info-label">规格</span>
            <span class="mobile-info-value">{{ item.specification || '-' }}</span>
          </div>
          <div class="mobile-info-row">
            <span class="mobile-info-label">备注</span>
            <span class="mobile-info-value">{{ item.remark || '-' }}</span>
          </div>
        </div>
        <div class="mobile-card-actions">
          <el-button type="primary" round @click="handleInbound(item)">入库</el-button>
          <el-button type="warning" round @click="handleOutbound(item)">出库</el-button>
          <el-button round @click="handleViewRecords(item)">查看记录</el-button>
        </div>
      </div>
    </div>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="handlePagination" />

    <el-dialog :title="operationDialogTitle" v-model="operationOpen" width="560px" append-to-body @close="cancelOperation">
      <el-form ref="operationFormRef" :model="operationForm" :rules="operationRules" label-width="100px">
        <el-form-item label="配件/产品">
          <el-input :model-value="operationForm.itemName" disabled />
        </el-form-item>
        <el-form-item label="类型">
          <el-input :model-value="formatTypeLabel(operationForm.type)" disabled />
        </el-form-item>
        <el-form-item label="当前库存">
          <el-input :model-value="`${formatStock(operationForm.stockQuantity)} ${formatUnitLabel(operationForm.unit)}`" disabled />
        </el-form-item>
        <el-form-item label="数量" prop="quantity">
          <el-input-number v-model="operationForm.quantity" :min="1" :precision="0" controls-position="right" class="form-number" />
        </el-form-item>
        <el-form-item v-if="operationType === 'outbound'" label="用途" prop="usageType">
          <el-select
            v-model="operationForm.usageType"
            placeholder="请选择或输入用途"
            filterable
            allow-create
            default-first-option
            clearable
            class="form-select">
            <el-option v-for="item in usageTypeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item :label="operationType === 'inbound' ? '入库时间' : '出库时间'" prop="operateTime">
          <el-date-picker
            v-model="operationForm.operateTime"
            type="datetime"
            value-format="YYYY-MM-DD HH:mm:ss"
            placeholder="请选择操作时间"
            class="form-select" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="operationForm.remark" type="textarea" placeholder="请输入备注" maxlength="200" show-word-limit :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="cancelOperation">取消</el-button>
        <el-button type="primary" @click="submitOperation">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog :title="recordDialogTitle" v-model="recordOpen" width="900px" append-to-body>
      <el-tabs v-model="recordActiveTab">
        <el-tab-pane label="入库记录" name="inbound">
          <el-table v-loading="recordLoading" :data="inboundRecordList" border class="record-table" empty-text="暂无入库记录">
            <el-table-column type="index" label="序号" width="70" align="center" />
            <el-table-column label="入库数量" prop="quantity" width="120" align="center" />
            <el-table-column label="入库时间" prop="inboundTime" width="180" align="center" />
            <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
              <template #default="{ row }">
                {{ row.remark || '-' }}
              </template>
            </el-table-column>
          </el-table>
          <div v-loading="recordLoading" class="mobile-record-list">
            <el-empty v-if="!recordLoading && inboundRecordList.length === 0" description="暂无入库记录" />
            <div v-for="(record, index) in inboundRecordList" :key="record.id || index" class="mobile-record-card inbound-record-card">
              <div class="mobile-record-header">
                <span class="mobile-record-title">入库记录 #{{ index + 1 }}</span>
                <span class="mobile-record-quantity">+{{ record.quantity }}</span>
              </div>
              <div class="mobile-record-row">
                <span>入库时间</span>
                <strong>{{ record.inboundTime || '-' }}</strong>
              </div>
              <div class="mobile-record-row">
                <span>备注</span>
                <strong>{{ record.remark || '-' }}</strong>
              </div>
            </div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="出库记录" name="outbound">
          <el-table v-loading="recordLoading" :data="outboundRecordList" border class="record-table" empty-text="暂无出库记录">
            <el-table-column type="index" label="序号" width="70" align="center" />
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
          </el-table>
          <div v-loading="recordLoading" class="mobile-record-list">
            <el-empty v-if="!recordLoading && outboundRecordList.length === 0" description="暂无出库记录" />
            <div v-for="(record, index) in outboundRecordList" :key="record.id || index" class="mobile-record-card outbound-record-card">
              <div class="mobile-record-header">
                <span class="mobile-record-title">出库记录 #{{ index + 1 }}</span>
                <span class="mobile-record-quantity">-{{ record.quantity }}</span>
              </div>
              <div class="mobile-record-row">
                <span>用途</span>
                <strong>{{ formatDictLabel(usageTypeOptions, record.usageType) }}</strong>
              </div>
              <div class="mobile-record-row">
                <span>出库时间</span>
                <strong>{{ record.outboundTime || '-' }}</strong>
              </div>
              <div class="mobile-record-row">
                <span>备注</span>
                <strong>{{ record.remark || '-' }}</strong>
              </div>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <el-button type="primary" @click="recordOpen = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="InboundAndOutbound">
import { computed, getCurrentInstance, onBeforeUnmount, reactive, ref } from 'vue'
import { listItems } from '@/api/dcbl/items'
import { addInboundRecord, listInboundRecords } from '@/api/dcbl/inboundRecords'
import { addOutboundRecord, listOutboundRecords } from '@/api/dcbl/outboundRecords'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const operationFormRef = ref()
const loading = ref(false)
const total = ref(0)
const itemList = ref([])
const operationOpen = ref(false)
const operationType = ref('inbound')
const recordOpen = ref(false)
const recordLoading = ref(false)
const recordActiveTab = ref('inbound')
const currentRecordItem = ref({})
const inboundRecordList = ref([])
const outboundRecordList = ref([])
const typeOptions = ref([])
const unitOptions = ref([])
const usageTypeOptions = ref([])
let searchTimer = null

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: ''
})

const operationForm = reactive({
  itemId: undefined,
  itemName: '',
  type: '',
  unit: '',
  stockQuantity: 0,
  quantity: 1,
  operateTime: '',
  usageType: '',
  remark: ''
})

const operationDialogTitle = computed(() => (operationType.value === 'inbound' ? '入库操作' : '出库操作'))
const recordDialogTitle = computed(() => `出入库记录 - ${currentRecordItem.value?.name || ''}`)

const operationRules = {
  quantity: [
    { required: true, message: '数量不能为空', trigger: 'blur' },
    { validator: validatePositiveQuantity, trigger: 'change' }
  ],
  operateTime: [{ required: true, message: '请选择操作时间', trigger: 'change' }]
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
    component: '配件',
    part: '配件',
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
  return label === '成品' || label === '产品' || value === 'finished' || value === 'product' ? 'success' : 'info'
}

function formatStock(value) {
  return Number(value || 0)
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

function getPageData(response) {
  const data = response?.data || {}
  const list = data.result || data.rows || data.list || data.items || []
  const totalNum = data.totalNum ?? data.total ?? data.count ?? list.length

  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function normalizeItem(item) {
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name ?? '',
    type: item.type ?? item.Type ?? '',
    unit: item.unit ?? item.Unit ?? '',
    specification: item.specification ?? item.Specification ?? item.spec ?? item.Spec ?? '',
    stockQuantity: item.stockQuantity ?? item.StockQuantity ?? item.quantity ?? item.Quantity ?? 0,
    remark: item.remark ?? item.Remark ?? ''
  }
}

function formatTime(value) {
  if (!value) return ''
  if (typeof value !== 'string') return value
  return value.replace('T', ' ').slice(0, 19)
}

function normalizeInboundRecord(record) {
  return {
    id: record.id ?? record.Id,
    itemId: record.itemId ?? record.ItemId,
    quantity: Number(record.quantity ?? record.Quantity ?? 0),
    inboundTime: formatTime(record.inboundTime ?? record.InboundTime),
    remark: record.remark ?? record.Remark ?? ''
  }
}

function normalizeOutboundRecord(record) {
  return {
    id: record.id ?? record.Id,
    itemId: record.itemId ?? record.ItemId,
    quantity: Number(record.quantity ?? record.Quantity ?? 0),
    usageType: record.usageType ?? record.UsageType ?? '',
    outboundTime: formatTime(record.outboundTime ?? record.OutboundTime),
    remark: record.remark ?? record.Remark ?? ''
  }
}

function getList() {
  loading.value = true
  listItems({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    name: queryParams.name.trim()
  })
    .then((response) => {
      const pageData = getPageData(response)
      itemList.value = pageData.list.map(normalizeItem)
      total.value = pageData.total
    })
    .finally(() => {
      loading.value = false
    })
}

function handleInputSearch() {
  window.clearTimeout(searchTimer)
  searchTimer = window.setTimeout(() => {
    handleQuery()
  }, 300)
}

function handleQuery() {
  window.clearTimeout(searchTimer)
  queryParams.name = queryParams.name.trim()
  queryParams.pageNum = 1
  getList()
}

function handlePagination({ page, limit }) {
  queryParams.pageNum = page
  queryParams.pageSize = limit
  getList()
}

function resetOperationForm() {
  Object.assign(operationForm, {
    itemId: undefined,
    itemName: '',
    type: '',
    unit: '',
    stockQuantity: 0,
    quantity: 1,
    operateTime: getCurrentTime(),
    usageType: '',
    remark: ''
  })
  operationFormRef.value?.clearValidate()
}

function openOperation(row, type) {
  operationType.value = type
  resetOperationForm()
  Object.assign(operationForm, {
    itemId: row.id,
    itemName: row.name,
    type: row.type,
    unit: row.unit,
    stockQuantity: row.stockQuantity,
    operateTime: getCurrentTime()
  })
  operationOpen.value = true
}

function handleInbound(row) {
  openOperation(row, 'inbound')
}

function handleOutbound(row) {
  openOperation(row, 'outbound')
}

function handleViewRecords(row) {
  currentRecordItem.value = { ...row }
  recordActiveTab.value = 'inbound'
  recordOpen.value = true
  getRecordLists(row.id)
}

function getRecordLists(itemId) {
  recordLoading.value = true
  Promise.all([
    listInboundRecords({ pageNum: 1, pageSize: 999, itemId }),
    listOutboundRecords({ pageNum: 1, pageSize: 999, itemId })
  ])
    .then(([inboundResponse, outboundResponse]) => {
      inboundRecordList.value = getPageData(inboundResponse).list.map(normalizeInboundRecord)
      outboundRecordList.value = getPageData(outboundResponse).list.map(normalizeOutboundRecord)
    })
    .finally(() => {
      recordLoading.value = false
    })
}

function cancelOperation() {
  operationOpen.value = false
  resetOperationForm()
}

function getSubmitData() {
  const baseData = {
    ItemId: operationForm.itemId,
    Quantity: Number(operationForm.quantity),
    Remark: operationForm.remark.trim()
  }

  if (operationType.value === 'inbound') {
    return {
      ...baseData,
      InboundTime: operationForm.operateTime
    }
  }

  return {
    ...baseData,
    UsageType: String(operationForm.usageType || '').trim(),
    OutboundTime: operationForm.operateTime
  }
}

function submitOperation() {
  operationFormRef.value.validate((valid) => {
    if (!valid) return

    const isInbound = operationType.value === 'inbound'
    const request = isInbound ? addInboundRecord : addOutboundRecord
    request(getSubmitData()).then(() => {
      proxy.$modal.msgSuccess(isInbound ? '入库成功' : '出库成功')
      operationOpen.value = false
      resetOperationForm()
      getList()
    })
  })
}

onBeforeUnmount(() => {
  window.clearTimeout(searchTimer)
})

loadDicts()
resetOperationForm()
getList()
</script>

<style scoped>
.inbound-outbound-page {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-control {
  width: 320px;
}

.item-table {
  width: 100%;
}

.mobile-card-list {
  display: none;
}

.record-table {
  width: 100%;
}

.mobile-record-list {
  display: none;
}

.form-select,
.form-number {
  width: 100%;
}

@media screen and (max-width: 768px) {
  .inbound-outbound-page {
    min-height: 100vh;
    padding: 12px;
    background: #f5f7fb;
  }

  .search-form {
    position: sticky;
    top: 0;
    z-index: 5;
    margin: -12px -12px 12px;
    padding: 12px;
    background: #f5f7fb;
  }

  .search-form :deep(.el-form-item) {
    display: block;
    margin-right: 0;
    margin-bottom: 0;
  }

  .search-form :deep(.el-form-item__label) {
    display: none;
  }

  .search-form :deep(.el-form-item__content) {
    display: block;
  }

  .query-control {
    width: 100%;
  }

  .query-control :deep(.el-input__wrapper) {
    border-radius: 999px;
    box-shadow: 0 4px 14px rgb(31 45 61 / 8%);
  }

  .item-table {
    display: none;
  }

  .mobile-card-list {
    display: block;
  }

  .mobile-item-card {
    margin-bottom: 12px;
    padding: 14px;
    border-radius: 16px;
    background: #fff;
    box-shadow: 0 8px 24px rgb(31 45 61 / 8%);
  }

  .mobile-card-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 12px;
  }

  .mobile-card-title {
    min-width: 0;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .mobile-item-name {
    overflow: hidden;
    color: #1f2d3d;
    font-size: 16px;
    font-weight: 600;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .mobile-stock {
    flex-shrink: 0;
    text-align: right;
  }

  .mobile-stock-value {
    color: #1677ff;
    font-size: 22px;
    font-weight: 700;
    line-height: 1;
  }

  .mobile-stock-unit {
    margin-left: 2px;
    color: #909399;
    font-size: 12px;
  }

  .mobile-card-content {
    margin-top: 12px;
    padding: 10px 12px;
    border-radius: 12px;
    background: #f8fafc;
  }

  .mobile-info-row {
    display: flex;
    gap: 12px;
    color: #606266;
    font-size: 13px;
    line-height: 22px;
  }

  .mobile-info-row + .mobile-info-row {
    margin-top: 4px;
  }

  .mobile-info-label {
    flex-shrink: 0;
    color: #909399;
  }

  .mobile-info-value {
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .mobile-card-actions {
    display: grid;
    grid-template-columns: repeat(3, minmax(0, 1fr));
    gap: 8px;
    margin-top: 12px;
  }

  .mobile-card-actions :deep(.el-button) {
    width: 100%;
    margin-left: 0;
  }

  :deep(.pagination-container) {
    margin-top: 8px;
    padding: 8px 0 0;
    background: transparent;
  }

  :deep(.el-dialog) {
    width: calc(100vw - 24px) !important;
    margin-top: 5vh !important;
    border-radius: 16px;
  }

  :deep(.el-dialog__body) {
    max-height: 70vh;
    overflow-y: auto;
    padding: 12px 16px;
  }

  :deep(.el-dialog__footer) {
    padding: 10px 16px 16px;
  }

  :deep(.el-tabs__nav-wrap::after) {
    height: 1px;
  }

  :deep(.el-tabs__item) {
    font-size: 15px;
  }

  .record-table {
    display: none;
  }

  .mobile-record-list {
    display: block;
  }

  .mobile-record-card {
    margin-bottom: 10px;
    padding: 12px;
    border: 1px solid #eef1f6;
    border-radius: 14px;
    background: #fff;
    box-shadow: 0 6px 18px rgb(31 45 61 / 6%);
  }

  .mobile-record-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 10px;
  }

  .mobile-record-title {
    color: #303133;
    font-size: 15px;
    font-weight: 600;
  }

  .mobile-record-quantity {
    font-size: 20px;
    font-weight: 700;
    line-height: 1;
  }

  .inbound-record-card .mobile-record-quantity {
    color: #67c23a;
  }

  .outbound-record-card .mobile-record-quantity {
    color: #e6a23c;
  }

  .mobile-record-row {
    display: flex;
    justify-content: space-between;
    gap: 12px;
    color: #909399;
    font-size: 13px;
    line-height: 24px;
  }

  .mobile-record-row strong {
    min-width: 0;
    overflow: hidden;
    color: #606266;
    font-weight: 500;
    text-align: right;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}
</style>