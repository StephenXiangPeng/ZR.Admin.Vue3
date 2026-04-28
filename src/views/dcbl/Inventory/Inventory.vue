<template>
  <div class="app-container inventory-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="名称" prop="name">
        <el-input v-model="queryParams.name" placeholder="请输入物料/产品名称" clearable class="query-control" @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="类型" prop="type">
        <el-select v-model="queryParams.type" placeholder="请选择类型" clearable class="query-control" @change="handleQuery">
          <el-option v-for="item in typeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
        </el-select>
      </el-form-item>
      <el-form-item label="库存状态" prop="stockStatus">
        <el-select v-model="queryParams.stockStatus" placeholder="请选择库存状态" class="query-control" @change="handleQuery">
          <el-option v-for="item in stockStatusOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <el-alert
      title="库存台账仅用于查看当前库存，库存变化请通过入库、出库或库存调整产生。"
      type="info"
      show-icon
      :closable="false"
      class="inventory-tip" />

    <el-table v-loading="loading" :data="inventoryList" border class="inventory-table" :row-class-name="getRowClassName">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" />
      <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="type" width="120" align="center">
        <template #default="{ row }">
          <el-tag :type="getTypeTagType(row.type)">{{ formatDictLabel(typeOptions, row.type) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="规格" prop="specification" min-width="160" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.specification || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="单位" prop="unit" width="100" align="center">
        <template #default="{ row }">
          {{ formatDictLabel(unitOptions, row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="当前库存" prop="stockQuantity" width="120" align="center">
        <template #default="{ row }">
          <span :class="{ 'zero-stock': Number(row.stockQuantity || 0) === 0 }">{{ formatStock(row.stockQuantity) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="最后更新时间" prop="updateTime" width="180" align="center">
        <template #default="{ row }">
          {{ row.updateTime || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="130" align="center" fixed="right">
        <template #default="{ row }">
          <el-button text size="small" icon="View" @click="handleViewLedger(row)">查看台账</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="handlePagination" />

    <stock-log-dialog v-model="ledgerOpen" :item="currentItem" />
  </div>
</template>

<script setup name="Inventory">
import { getCurrentInstance, reactive, ref } from 'vue'
import { listStocks } from '@/api/dcbl/stocks'
import StockLogDialog from './components/StockLogDialog.vue'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const typeOptions = ref([])
const unitOptions = ref([])
const loading = ref(false)
const total = ref(0)
const inventoryList = ref([])
const ledgerOpen = ref(false)
const currentItem = ref({})

const stockStatusOptions = [
  { label: '全部', value: '' },
  { label: '有库存（>0）', value: 'has' },
  { label: '无库存（=0）', value: 'empty' }
]

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: '',
  type: '',
  stockStatus: ''
})

function loadDicts() {
  proxy.getDicts(['dc_material_unit', 'dc_material_classification']).then((response) => {
    const dicts = Array.isArray(response.data) ? response.data : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
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

function getTypeTagType(value) {
  const label = formatDictLabel(typeOptions.value, value)
  return label === '成品' ? 'success' : 'info'
}

function formatStock(value) {
  return Number(value || 0)
}

function getTableIndex(index) {
  return (queryParams.pageNum - 1) * queryParams.pageSize + index + 1
}

function getList() {
  loading.value = true
  listStocks({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    name: queryParams.name,
    type: queryParams.type,
    stockStatus: queryParams.stockStatus
  })
    .then((response) => {
      const pageData = getPageData(response)
      inventoryList.value = pageData.list.map(normalizeInventory)
      total.value = pageData.total
    })
    .finally(() => {
      loading.value = false
    })
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

function normalizeInventory(item) {
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name ?? '',
    type: item.type ?? item.Type ?? '',
    specification: item.specification ?? item.Specification ?? item.spec ?? item.Spec ?? '',
    unit: item.unit ?? item.Unit ?? '',
    stockQuantity: item.stockQuantity ?? item.StockQuantity ?? 0,
    updateTime: item.updateTime ?? item.UpdateTime ?? item.update_time ?? item.Update_time ?? ''
  }
}

function getRowClassName({ row }) {
  return Number(row.stockQuantity || 0) === 0 ? 'zero-stock-row' : ''
}

function handleQuery() {
  queryParams.name = queryParams.name.trim()
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

function handleViewLedger(row) {
  currentItem.value = { ...row }
  ledgerOpen.value = true
}

loadDicts()
getList()
</script>

<style scoped>
.inventory-management {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-control {
  width: 220px;
}

.inventory-tip {
  margin-bottom: 12px;
}

.inventory-table {
  width: 100%;
}

.zero-stock {
  color: #909399;
}

:deep(.zero-stock-row) {
  color: #909399;
  background-color: #fafafa;
}

:deep(.el-table__body tr:hover > td.el-table__cell) {
  background-color: #f5f7fa;
}
</style>