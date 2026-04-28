<template>
  <el-dialog :title="dialogTitle" v-model="visible" width="980px" append-to-body @open="handleOpen">
    <el-table v-loading="loading" :data="logList" border class="stock-log-table">
      <el-table-column label="时间" prop="createTime" width="180" align="center">
        <template #default="{ row }">
          {{ row.createTime || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="变动类型" prop="changeType" width="130" align="center">
        <template #default="{ row }">
          <el-tag :type="getChangeTypeTag(row.changeType)">{{ formatChangeType(row.changeType) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="变动数量" prop="changeQuantity" width="120" align="center">
        <template #default="{ row }">
          <span :class="getQuantityClass(row.changeQuantity)">{{ formatChangeQuantity(row.changeQuantity) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="变动前库存" prop="beforeQuantity" width="120" align="center" />
      <el-table-column label="变动后库存" prop="afterQuantity" width="120" align="center" />
      <el-table-column label="来源类型" prop="sourceType" width="120" align="center">
        <template #default="{ row }">
          {{ row.sourceType || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="handlePagination" />

    <template #footer>
      <el-button type="primary" @click="visible = false">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup name="StockLogDialog">
import { computed, reactive, ref, watch } from 'vue'
import { listStockLogs } from '@/api/dcbl/stocks'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  item: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:modelValue'])

const visible = computed({
  get() {
    return props.modelValue
  },
  set(value) {
    emit('update:modelValue', value)
  }
})

const loading = ref(false)
const total = ref(0)
const logList = ref([])

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10
})

const dialogTitle = computed(() => `库存台账 - ${props.item?.name || ''}`)

const changeTypeMap = {
  inbound: { label: '入库', tag: 'success' },
  outbound: { label: '出库', tag: 'danger' },
  inbound_update: { label: '入库修改', tag: 'warning' },
  inbound_delete: { label: '删除入库', tag: 'info' },
  outbound_update: { label: '出库修改', tag: 'warning' },
  outbound_delete: { label: '删除出库', tag: 'info' }
}

watch(
  () => props.item?.id,
  () => {
    queryParams.pageNum = 1
    logList.value = []
    total.value = 0
  }
)

function handleOpen() {
  queryParams.pageNum = 1
  getList()
}

function handlePagination({ page, limit }) {
  queryParams.pageNum = page
  queryParams.pageSize = limit
  getList()
}

function getList() {
  if (!props.item?.id) return

  loading.value = true
  listStockLogs({
    itemId: props.item.id,
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize
  })
    .then((response) => {
      const pageData = getPageData(response)
      logList.value = pageData.list.map(normalizeLog)
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

function normalizeLog(item) {
  return {
    createTime: item.createTime ?? item.CreateTime ?? '',
    changeType: item.changeType ?? item.ChangeType ?? '',
    changeQuantity: item.changeQuantity ?? item.ChangeQuantity ?? 0,
    beforeQuantity: item.beforeQuantity ?? item.BeforeQuantity ?? 0,
    afterQuantity: item.afterQuantity ?? item.AfterQuantity ?? 0,
    sourceType: item.sourceType ?? item.SourceType ?? '',
    remark: item.remark ?? item.Remark ?? ''
  }
}

function formatChangeType(value) {
  return changeTypeMap[value]?.label || value || '-'
}

function getChangeTypeTag(value) {
  return changeTypeMap[value]?.tag || 'info'
}

function formatChangeQuantity(value) {
  const quantity = Number(value || 0)
  return quantity > 0 ? `+${quantity}` : String(quantity)
}

function getQuantityClass(value) {
  const quantity = Number(value || 0)
  return {
    'quantity-positive': quantity > 0,
    'quantity-negative': quantity < 0
  }
}
</script>

<style scoped>
.stock-log-table {
  width: 100%;
}

.quantity-positive {
  color: #67c23a;
  font-weight: 600;
}

.quantity-negative {
  color: #f56c6c;
  font-weight: 600;
}
</style>
