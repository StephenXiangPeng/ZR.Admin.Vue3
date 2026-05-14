<template>
  <div class="app-container glass-bottle-stock">
    <el-form ref="queryRef" :model="queryParams" class="search-form" @submit.prevent>
      <el-form-item label="瓶身名称" prop="name" class="query-form-item">
        <el-input
          v-model="queryParams.name"
          placeholder="输入名称即时筛选"
          clearable
          class="query-control"
          size="default"
          @input="handleInputQuery"
          @clear="handleQuery" />
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" size="default" @click="handleAdd">新增瓶身</el-button>
      </el-col>
    </el-row>

    <el-table
      v-loading="loading"
      :data="tableRows"
      border
      stripe
      class="bottle-table"
      :height="tableHeight"
      :row-class-name="tableRowClassName">
      <el-table-column label="瓶身编码" prop="code" width="130" align="center" :show-overflow-tooltip="true" />
      <el-table-column label="瓶身名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="规格型号" prop="specification" min-width="140" :show-overflow-tooltip="true">
        <template #default="{ row }">{{ row.specification || '-' }}</template>
      </el-table-column>
      <el-table-column label="单位" prop="unit" width="90" align="center">
        <template #default="{ row }">
          {{ formatDictLabel(unitOptions, row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="当前库存" prop="currentStock" width="110" align="center">
        <template #default="{ row }">
          <span :class="getCurrentStockClass(row)">{{ formatNum(row.currentStock) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="安全库存" prop="safetyStock" width="100" align="center">
        <template #default="{ row }">{{ formatNum(row.safetyStock) }}</template>
      </el-table-column>
      <el-table-column label="创建时间" prop="createTime" width="168" align="center" />
      <el-table-column label="操作" fixed="right" width="248" align="center">
        <template #default="{ row }">
          <div class="op-actions">
            <el-button text size="small" class="op-btn is-primary" @click="openStockOp(row)">库存</el-button>
            <el-button text size="small" class="op-btn" @click="openRecords(row)">记录</el-button>
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

    <!-- 新增 / 编辑瓶身档案 -->
    <el-dialog :title="archiveTitle" v-model="archiveOpen" width="640px" append-to-body @close="resetArchiveForm">
      <el-form ref="archiveFormRef" :model="archiveForm" :rules="archiveRules" label-width="100px" @submit.prevent>
        <el-form-item label="瓶身编码" prop="code">
          <el-input v-model="archiveForm.code" maxlength="40" show-word-limit placeholder="唯一编码，如 GB-500-A" />
        </el-form-item>
        <el-form-item label="瓶身名称" prop="name">
          <el-input v-model="archiveForm.name" maxlength="80" show-word-limit placeholder="请输入瓶身名称" />
        </el-form-item>
        <el-form-item label="规格型号" prop="specification">
          <el-input v-model="archiveForm.specification" maxlength="100" show-word-limit placeholder="请输入规格型号" />
        </el-form-item>
        <el-row :gutter="16" class="archive-form-row">
          <el-col :span="12">
            <el-form-item label="单位" prop="unit">
              <el-select
                v-model="archiveForm.unit"
                placeholder="请选择单位"
                clearable
                filterable
                class="form-control-full">
                <el-option v-for="item in unitOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="安全库存" prop="safetyStock">
              <el-input-number
                v-model="archiveForm.safetyStock"
                :min="0"
                :precision="0"
                controls-position="right"
                class="form-control-full" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item v-if="archiveEdit" label="当前库存">
          <span class="readonly-text">{{ formatNum(archiveForm.currentStock) }}（由库存操作维护）</span>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="archiveForm.remark" type="textarea" maxlength="200" show-word-limit :rows="2" placeholder="选填" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="archiveOpen = false">取消</el-button>
        <el-button type="primary" @click="submitArchive">保存</el-button>
      </template>
    </el-dialog>

    <!-- 库存增加 / 减少 / 调整（同一弹窗内选择类型） -->
    <el-dialog
      v-model="stockOpOpen"
      title="库存变更"
      width="520px"
      append-to-body
      @close="resetStockOpForm">
      <el-form ref="stockOpFormRef" :model="stockOpForm" :rules="stockOpRules" label-width="100px" @submit.prevent>
        <el-form-item label="瓶身名称">
          <span class="readonly-text">{{ currentBottle.name || '-' }}</span>
        </el-form-item>
        <el-form-item label="当前库存">
          <span class="readonly-text">{{ formatNum(currentBottle.currentStock) }}</span>
        </el-form-item>
        <el-form-item label="操作类型">
          <el-radio-group v-model="stockOpType" class="stock-op-type-group" @change="onStockOpTypeChange">
            <el-radio value="in">增加</el-radio>
            <el-radio value="out">减少</el-radio>
            <el-radio value="adjust">调整</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="stockOpType === 'in'" label="增加数量" prop="quantity">
          <el-input-number
            v-model="stockOpForm.quantity"
            :min="1"
            :precision="0"
            controls-position="right"
            class="form-control-full" />
        </el-form-item>
        <el-form-item v-if="stockOpType === 'out'" label="减少数量" prop="quantity">
          <el-input-number
            v-model="stockOpForm.quantity"
            :min="1"
            :precision="0"
            :max="Math.max(Number(currentBottle.currentStock) || 0, 1)"
            controls-position="right"
            class="form-control-full" />
        </el-form-item>
        <el-form-item v-if="stockOpType === 'adjust'" label="调整后库存" prop="targetStock">
          <el-input-number
            v-model="stockOpForm.targetStock"
            :min="0"
            :precision="0"
            controls-position="right"
            class="form-control-full" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="stockOpForm.remark" type="textarea" maxlength="200" show-word-limit :rows="3" placeholder="请输入备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="stockOpOpen = false">取消</el-button>
        <el-button type="primary" :loading="stockOpSaving" @click="submitStockOp">保存</el-button>
      </template>
    </el-dialog>

    <!-- 库存记录 -->
    <el-dialog v-model="recordOpen" :title="`库存记录 - ${currentBottle.name || ''}`" width="920px" append-to-body @open="syncRecordPage">
      <el-table v-loading="recordLoading" :data="recordTableRows" border class="record-table" max-height="420">
        <el-table-column label="操作类型" prop="opType" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="recordTagType(row.opType)" size="small">{{ formatOpType(row.opType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作前库存" prop="beforeStock" width="110" align="center">
          <template #default="{ row }">{{ formatNum(row.beforeStock) }}</template>
        </el-table-column>
        <el-table-column label="操作数量" prop="changeQty" width="110" align="center">
          <template #default="{ row }">{{ formatChangeQty(row) }}</template>
        </el-table-column>
        <el-table-column label="操作后库存" prop="afterStock" width="110" align="center">
          <template #default="{ row }">{{ formatNum(row.afterStock) }}</template>
        </el-table-column>
        <el-table-column label="操作时间" prop="opTime" width="170" align="center" />
        <el-table-column label="操作人" prop="operator" width="120" align="center" :show-overflow-tooltip="true">
          <template #default="{ row }">{{ formatLogOperator(row) }}</template>
        </el-table-column>
        <el-table-column label="备注" prop="remark" min-width="160" :show-overflow-tooltip="true">
          <template #default="{ row }">{{ row.remark || '-' }}</template>
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

<script setup name="GlassBottleStock">
import { computed, getCurrentInstance, nextTick, reactive, ref } from 'vue'
import {
  addGlassBottle,
  changeGlassBottleStock,
  deleteGlassBottles,
  editGlassBottle,
  listGlassBottleStockLogs,
  listGlassBottles
} from '@/api/dcbl/glassBottle'

const { proxy } = getCurrentInstance()

const loading = ref(false)
const total = ref(0)
const tableRows = ref([])

const queryParams = reactive({
  pageNum: 1,
  pageSize: 30,
  name: ''
})

const queryDebounceTimer = ref()

const tableHeight = computed(() => 'calc(100vh - 290px)')

const archiveOpen = ref(false)
const archiveEdit = ref(false)
const archiveTitle = computed(() => (archiveEdit.value ? '编辑瓶身' : '新增瓶身'))
const archiveFormRef = ref()
const unitOptions = ref([])
const userOptions = ref([])
const archiveForm = reactive({
  id: undefined,
  code: '',
  name: '',
  specification: '',
  unit: '',
  safetyStock: 0,
  remark: '',
  currentStock: 0
})

const archiveRules = {
  code: [{ required: true, message: '请输入瓶身编码', trigger: 'blur' }],
  name: [{ required: true, message: '瓶身名称不能为空', trigger: 'blur' }],
  unit: [{ required: true, message: '请选择单位', trigger: 'change' }],
  safetyStock: [{ required: true, message: '请填写安全库存', trigger: 'change' }]
}

const currentBottle = reactive({
  id: undefined,
  code: '',
  name: '',
  currentStock: 0,
  safetyStock: 0
})

const stockOpOpen = ref(false)
const stockOpType = ref('in')
const stockOpSaving = ref(false)
const stockOpFormRef = ref()
const stockOpForm = reactive({
  quantity: 1,
  targetStock: 0,
  remark: ''
})

const validateOutQty = (_rule, value, callback) => {
  const n = Number(value)
  const max = Number(currentBottle.currentStock) || 0
  if (!Number.isFinite(n) || n < 1) {
    callback(new Error('请输入大于 0 的数量'))
    return
  }
  if (n > max) {
    callback(new Error('减少数量不能大于当前库存'))
    return
  }
  callback()
}

const stockOpRules = computed(() => {
  if (stockOpType.value === 'in') {
    return {
      quantity: [{ required: true, message: '请输入增加数量', trigger: 'change' }],
      remark: [{ required: true, message: '请输入备注', trigger: 'blur' }]
    }
  }
  if (stockOpType.value === 'out') {
    return {
      quantity: [
        { required: true, message: '请输入减少数量', trigger: 'change' },
        { validator: validateOutQty, trigger: 'change' }
      ],
      remark: [{ required: true, message: '请输入备注', trigger: 'blur' }]
    }
  }
  return {
    targetStock: [{ required: true, message: '请输入调整后库存', trigger: 'change' }],
    remark: [{ required: true, message: '请输入备注', trigger: 'blur' }]
  }
})

const recordOpen = ref(false)
const recordLoading = ref(false)
const recordTotal = ref(0)
const recordTableRows = ref([])
const recordQuery = reactive({
  pageNum: 1,
  pageSize: 20
})

function formatNum(v) {
  const n = Number(v)
  return Number.isFinite(n) ? n : 0
}

function loadDicts() {
  return proxy.getDicts(['dc_material_unit', 'sql_all_user']).then((response) => {
    const source = response?.data?.data || response?.data?.result || response?.data || []
    const dicts = Array.isArray(source) ? source : []
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
    userOptions.value = getDictList(dicts, 'sql_all_user').map((item) => ({
      userId: item.dictValue,
      userName: item.dictLabel,
      nickName: item.dictLabel
    }))
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

/** 库存流水操作人：按 sql_all_user 字典（dictValue=用户标识）解析展示 */
function formatLogOperator(row) {
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

function getPageData(response) {
  const raw = response?.data ?? {}
  const data = raw?.data ?? raw?.result ?? raw
  const list = data?.rows ?? data?.result ?? data?.list ?? data?.items ?? []
  const totalNum = data?.total ?? data?.totalNum ?? data?.count ?? list.length
  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function formatDateTime(value) {
  if (value == null || value === '') return ''
  if (typeof value === 'string') return value.replace('T', ' ').slice(0, 19)
  return String(value)
}

function normalizeBottleRow(item) {
  return {
    id: item.id ?? item.Id,
    code: item.bottleCode ?? item.BottleCode ?? '',
    name: item.bottleName ?? item.BottleName ?? '',
    specification: item.specification ?? item.Specification ?? '',
    unit: item.unit ?? item.Unit ?? '',
    currentStock: formatNum(item.currentStock ?? item.CurrentStock),
    safetyStock: formatNum(item.safetyStock ?? item.SafetyStock),
    isWarning: item.isWarning === true || item.isWarning === 1,
    remark: item.remark ?? item.Remark ?? '',
    createTime: formatDateTime(item.createTime ?? item.Create_time ?? item.CreateTime)
  }
}

function normalizeStockLog(item) {
  return {
    id: item.id ?? item.Id,
    opType: item.opType ?? item.OpType ?? '',
    beforeStock: formatNum(item.beforeStock ?? item.BeforeStock),
    changeQty: formatNum(item.changeQty ?? item.ChangeQty),
    afterStock: formatNum(item.afterStock ?? item.AfterStock),
    opTime: formatDateTime(item.opTime ?? item.OpTime),
    operator: item.operatorName ?? item.OperatorName ?? item.operator ?? item.Operator ?? '',
    operatorId:
      item.operatorId ??
      item.OperatorId ??
      item.createBy ??
      item.CreateBy ??
      item.userId ??
      item.UserId ??
      '',
    remark: item.remark ?? item.Remark ?? ''
  }
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

function getList() {
  loading.value = true
  listGlassBottles({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    bottleName: queryParams.name.trim()
  })
    .then((response) => {
      const pageData = getPageData(response)
      tableRows.value = pageData.list.map(normalizeBottleRow)
      total.value = pageData.total
    })
    .finally(() => {
      loading.value = false
    })
}

function isBelowSafety(row) {
  if (row.isWarning === true || row.isWarning === 1) return true
  return formatNum(row.currentStock) < formatNum(row.safetyStock)
}

function getCurrentStockClass(row) {
  if (isBelowSafety(row)) return 'stock-below-safe'
  return 'stock-highlight'
}

function tableRowClassName({ row }) {
  return isBelowSafety(row) ? 'is-low-stock' : ''
}

function resetArchiveForm() {
  Object.assign(archiveForm, {
    id: undefined,
    code: '',
    name: '',
    specification: '',
    unit: '',
    safetyStock: 0,
    remark: '',
    currentStock: 0
  })
  archiveFormRef.value?.clearValidate()
}

function handleAdd() {
  archiveEdit.value = false
  resetArchiveForm()
  archiveOpen.value = true
}

function handleEdit(row) {
  archiveEdit.value = true
  Object.assign(archiveForm, {
    id: row.id,
    code: row.code,
    name: row.name,
    specification: row.specification || '',
    unit: row.unit ?? '',
    safetyStock: formatNum(row.safetyStock),
    remark: row.remark || '',
    currentStock: formatNum(row.currentStock)
  })
  archiveFormRef.value?.clearValidate()
  archiveOpen.value = true
}

function submitArchive() {
  archiveFormRef.value?.validate((valid) => {
    if (!valid) return
    const payload = {
      bottleCode: archiveForm.code.trim(),
      bottleName: archiveForm.name.trim(),
      specification: (archiveForm.specification || '').trim(),
      unit: String(archiveForm.unit ?? '').trim(),
      safetyStock: formatNum(archiveForm.safetyStock),
      remark: (archiveForm.remark || '').trim()
    }
    const req = archiveEdit.value ? editGlassBottle({ ...payload, id: archiveForm.id }) : addGlassBottle(payload)
    req.then(() => {
      proxy.$modal.msgSuccess(archiveEdit.value ? '保存成功' : '新增成功')
      archiveOpen.value = false
      getList()
    })
  })
}

function handleDelete(row) {
  const stock = formatNum(row.currentStock)
  const tip =
    stock > 0
      ? `「${row.name}」当前库存为 ${stock}，删除后不可恢复，是否确认删除？`
      : `是否确认删除「${row.name}」？`
  proxy.$modal
    .confirm(tip)
    .then(() => deleteGlassBottles([row.id]))
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

function onStockOpTypeChange() {
  nextTick(() => {
    stockOpFormRef.value?.clearValidate()
  })
}

function openStockOp(row) {
  stockOpType.value = 'in'
  Object.assign(currentBottle, {
    id: row.id,
    code: row.code,
    name: row.name,
    currentStock: formatNum(row.currentStock),
    safetyStock: formatNum(row.safetyStock)
  })
  stockOpForm.quantity = 1
  stockOpForm.targetStock = formatNum(row.currentStock)
  stockOpForm.remark = ''
  stockOpOpen.value = true
  stockOpFormRef.value?.clearValidate()
}

function resetStockOpForm() {
  stockOpType.value = 'in'
  stockOpForm.quantity = 1
  stockOpForm.targetStock = 0
  stockOpForm.remark = ''
  stockOpFormRef.value?.clearValidate()
}

function submitStockOp() {
  stockOpFormRef.value?.validate((valid) => {
    if (!valid) return
    let opType = 'increase'
    let changeQty = 0
    let afterStock = 0
    if (stockOpType.value === 'in') {
      opType = 'increase'
      changeQty = formatNum(stockOpForm.quantity)
    } else if (stockOpType.value === 'out') {
      opType = 'decrease'
      changeQty = formatNum(stockOpForm.quantity)
    } else {
      opType = 'adjust'
      afterStock = formatNum(stockOpForm.targetStock)
    }

    const payload = {
      bottleId: currentBottle.id,
      opType,
      remark: stockOpForm.remark.trim()
    }
    if (opType === 'adjust') {
      payload.afterStock = afterStock
      payload.changeQty = 0
    } else {
      payload.changeQty = changeQty
    }

    stockOpSaving.value = true
    changeGlassBottleStock(payload)
      .then(() => {
        proxy.$modal.msgSuccess('保存成功')
        stockOpOpen.value = false
        getList()
      })
      .finally(() => {
        stockOpSaving.value = false
      })
  })
}

function syncRecordPage() {
  if (!currentBottle.id) return
  recordLoading.value = true
  listGlassBottleStockLogs({
    pageNum: recordQuery.pageNum,
    pageSize: recordQuery.pageSize,
    bottleId: currentBottle.id
  })
    .then((response) => {
      const pageData = getPageData(response)
      recordTableRows.value = pageData.list.map(normalizeStockLog)
      recordTotal.value = pageData.total
    })
    .finally(() => {
      recordLoading.value = false
    })
}

function openRecords(row) {
  Object.assign(currentBottle, {
    id: row.id,
    code: row.code,
    name: row.name,
    currentStock: formatNum(row.currentStock),
    safetyStock: formatNum(row.safetyStock)
  })
  recordQuery.pageNum = 1
  recordOpen.value = true
}

function handleRecordPagination({ page, limit }) {
  recordQuery.pageNum = page
  recordQuery.pageSize = limit
  syncRecordPage()
}

function formatOpType(op) {
  if (op === 'increase') return '增加'
  if (op === 'decrease') return '减少'
  if (op === 'adjust') return '调整'
  return op || '-'
}

function recordTagType(op) {
  if (op === 'increase') return 'success'
  if (op === 'decrease') return 'warning'
  if (op === 'adjust') return 'info'
  return ''
}

function formatChangeQty(row) {
  if (row.opType === 'decrease') return `-${formatNum(row.changeQty)}`
  if (row.opType === 'increase') return `+${formatNum(row.changeQty)}`
  const delta = formatNum(row.afterStock) - formatNum(row.beforeStock)
  if (delta === 0) return '0'
  return delta > 0 ? `+${delta}` : `${delta}`
}

loadDicts()
  .catch(() => {})
  .finally(() => {
    getList()
  })
</script>

<style scoped>
.glass-bottle-stock {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
  width: 100%;
}

.query-form-item {
  width: 100%;
  max-width: 360px;
  margin-right: 0;
  margin-bottom: 0;
}

.query-control {
  width: 100%;
}

.stock-op-type-group {
  display: flex;
  flex-wrap: wrap;
  gap: 4px 12px;
}

.bottle-table,
.record-table {
  width: 100%;
}

.bottle-table :deep(.el-table__header-wrapper th),
.record-table :deep(.el-table__header-wrapper th) {
  background: #f2f6fc;
  color: #303133;
}

.bottle-table :deep(.el-table__header-wrapper th .cell),
.record-table :deep(.el-table__header-wrapper th .cell) {
  height: 20px;
  line-height: 20px;
  padding-top: 0;
  padding-bottom: 0;
  font-size: 12px;
}

.bottle-table :deep(.el-table__body-wrapper td),
.record-table :deep(.el-table__body-wrapper td) {
  padding-top: 0;
  padding-bottom: 0;
}

.bottle-table :deep(.el-table__body-wrapper td .cell),
.record-table :deep(.el-table__body-wrapper td .cell) {
  min-height: 26px;
  line-height: 26px;
  padding-top: 0;
  padding-bottom: 0;
}

.bottle-table :deep(tr.is-low-stock) > td {
  background-color: #fef0f0 !important;
}

.form-control-full {
  width: 100%;
}

.form-control-narrow {
  width: 200px;
}

.archive-form-row {
  width: 100%;
}

.archive-form-row :deep(.el-col .el-form-item) {
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
  padding: 0 3px;
  min-height: 20px;
}

.op-btn + .op-btn {
  margin-left: 1px;
}

.op-btn.is-primary {
  color: #409eff;
}

.op-btn.is-danger {
  color: #f56c6c;
}

.stock-highlight {
  color: #409eff;
  font-weight: 700;
  font-size: 14px;
}

.stock-below-safe {
  color: #f56c6c;
  font-weight: 700;
  font-size: 14px;
}
</style>
