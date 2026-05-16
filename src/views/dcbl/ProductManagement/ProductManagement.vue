<template>
  <div class="app-container product-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form" @submit.prevent>
      <el-form-item label="产品名称" prop="name" class="query-form-item">
        <el-input
          v-model="queryParams.name"
          placeholder="请输入产品名称"
          clearable
          class="query-control"
          size="default"
          @input="handleInputQuery"
          @clear="handleQuery"
          @keyup.enter="handleQuery" />
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" size="default" @click="handleAdd">新增产品</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="productList" border stripe class="product-table" :height="tableHeight">
      <el-table-column label="名称" prop="name" min-width="180" :show-overflow-tooltip="true" />
      <el-table-column label="单位" prop="unit" width="90" align="center" >
        <template #default="{ row }">
          {{ formatDictLabel(unitOptions, row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="规格" prop="specification" min-width="160" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.specification || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="配件组成" width="120" align="center">
        <template #default="{ row }">
          <el-button text size="small" type="primary" @click="handleViewComponents(row)">查看配件</el-button>
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="240" align="center">
        <template #default="{ row }">
          <div class="op-actions">
            <el-button text size="small" class="op-btn is-warning" @click="handleStockOut(row)">出库</el-button>
            <el-button text size="small" class="op-btn" @click="handleViewOutboundRecords(row)">出库记录</el-button>
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

    <el-dialog :title="dialogTitle" v-model="open" width="900px" append-to-body class="product-edit-dialog" @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="90px" @submit.prevent>
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" maxlength="50" show-word-limit placeholder="请输入产品名称" />
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
        <el-form-item label="配件组成" class="component-form-item">
          <div class="component-section">
            <el-table :data="form.components" border class="component-edit-table" empty-text="暂无配件组成">
              <el-table-column label="分类" width="180">
                <template #default="{ row }">
                  <el-tree-select
                    v-model="row.filterCategoryId"
                    :data="categoryTreeOptions"
                    :props="categoryTreeProps"
                    value-key="id"
                    placeholder="全部分类"
                    clearable
                    filterable
                    check-strictly
                    class="form-control"
                    @change="handleComponentCategoryChange(row)" />
                </template>
              </el-table-column>
              <el-table-column label="配件" min-width="200">
                <template #default="{ row }">
                  <el-select v-model="row.componentId" placeholder="请选择配件" filterable clearable class="form-control">
                    <el-option
                      v-for="item in getFilteredComponentOptions(row)"
                      :key="item.value"
                      :label="formatComponentOptionLabel(item)"
                      :value="item.value"
                      :disabled="isComponentSelected(item.value, row)" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column label="用量" width="160" align="center">
                <template #default="{ row }">
                  <el-input-number v-model="row.quantity" :min="0.000001" :precision="3" controls-position="right" class="component-quantity" />
                </template>
              </el-table-column>
              <el-table-column label="操作" width="90" align="center">
                <template #default="{ $index }">
                  <el-button text type="danger" icon="Delete" @click="removeComponentRow($index)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-button type="primary" plain icon="Plus" class="component-add-button" @click="addComponentRow">添加配件</el-button>
          </div>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog :title="componentDialogTitle" v-model="componentOpen" width="680px" append-to-body>
      <el-table v-loading="componentLoading" :data="productComponentList" border class="component-view-table" empty-text="暂无配件组成">
        <el-table-column label="配件名称" prop="componentName" min-width="160" :show-overflow-tooltip="true" />
        <el-table-column label="规格" prop="spec" min-width="160" :show-overflow-tooltip="true">
          <template #default="{ row }">
            {{ row.spec || '-' }}
          </template>
        </el-table-column>
        <el-table-column label="单位" prop="unit" width="100" align="center">
          <template #default="{ row }">
            {{ formatDictLabel(unitOptions, row.unit) }}
          </template>
        </el-table-column>
        <el-table-column label="用量" prop="quantity" width="120" align="center" />
      </el-table>
      <template #footer>
        <el-button type="primary" @click="componentOpen = false">关闭</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="stockOutOpen" title="产品出库" width="460px" append-to-body @close="resetStockOutForm">
      <el-form ref="stockOutFormRef" :model="stockOutForm" :rules="stockOutRules" label-width="100px" @submit.prevent>
        <el-form-item label="产品名称">
          <span class="readonly-text">{{ currentStockItem.name || '-' }}</span>
        </el-form-item>
        <el-form-item label="出库数量" prop="quantity">
          <el-input-number v-model="stockOutForm.quantity" :min="1" :precision="0" controls-position="right" class="form-control" />
        </el-form-item>
        <el-form-item label="用途" prop="usageType">
          <el-select
            v-model="stockOutForm.usageType"
            placeholder="请选择或输入用途"
            filterable
            allow-create
            default-first-option
            clearable
            class="form-control">
            <el-option v-for="item in usageTypeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="出库时间" prop="outboundTime">
          <el-date-picker
            v-model="stockOutForm.outboundTime"
            type="datetime"
            value-format="YYYY-MM-DD HH:mm:ss"
            placeholder="请选择出库时间"
            class="form-control" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="stockOutForm.remark" type="textarea" maxlength="200" show-word-limit :rows="3" placeholder="请输入出库备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="stockOutOpen = false">取消</el-button>
        <el-button type="primary" :loading="stockOutLoading" @click="submitStockOut">保存</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="outboundRecordOpen" :title="`出库记录 - ${currentStockItem.name || ''}`" width="860px" append-to-body @open="getOutboundRecordList">
      <el-table v-loading="outboundRecordLoading" :data="outboundRecordList" border class="component-view-table">
        <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
        <el-table-column label="类型" prop="type" width="120" align="center">
          <template #default="{ row }">
            {{ formatTypeLabel(row.type) }}
          </template>
        </el-table-column>
        <el-table-column label="单位" prop="unit" width="100" align="center" v-if="false">
          <template #default="{ row }">
            {{ formatDictLabel(unitOptions, row.unit) }}
          </template>
        </el-table-column>
        <el-table-column label="出库时间" prop="outboundTime" width="180" align="center">
          <template #default="{ row }">
            {{ row.outboundTime || '-' }}
          </template>
        </el-table-column>
        <el-table-column label="出库数量" prop="quantity" width="120" align="center" />
        <el-table-column label="用途" prop="usageType" min-width="140" :show-overflow-tooltip="true">
          <template #default="{ row }">
            {{ formatDictLabel(usageTypeOptions, row.usageType) }}
          </template>
        </el-table-column>
        <el-table-column label="备注" prop="remark" min-width="220" :show-overflow-tooltip="true">
          <template #default="{ row }">
            {{ row.remark || '-' }}
          </template>
        </el-table-column>
      </el-table>

      <pagination
        v-show="outboundRecordTotal > 0"
        :total="outboundRecordTotal"
        v-model:page="outboundRecordQuery.pageNum"
        v-model:limit="outboundRecordQuery.pageSize"
        @pagination="handleOutboundRecordPagination" />
      <template #footer>
        <el-button type="primary" @click="outboundRecordOpen = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="ProductManagement">
import { computed, getCurrentInstance, reactive, ref } from 'vue'
import { SPARE_PART_CATEGORY_TYPE, listCategory } from '@/api/dcbl/category'
import { addItem, deleteItems, editItem, listItems, listProductComponents } from '@/api/dcbl/items'
import { addOutboundRecord, listOutboundRecords } from '@/api/dcbl/outboundRecords'

const categoryTreeProps = { value: 'id', label: 'categoryName', children: 'children' }

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const queryDebounceTimer = ref()

const loading = ref(false)
const total = ref(0)
const open = ref(false)
const isEdit = ref(false)
const dialogTitle = ref('')
const componentOpen = ref(false)
const componentLoading = ref(false)
const currentProduct = ref({})
const stockOutOpen = ref(false)
const stockOutLoading = ref(false)
const outboundRecordOpen = ref(false)
const outboundRecordLoading = ref(false)
const outboundRecordTotal = ref(0)

const tableHeight = computed(() => 'calc(100vh - 290px)')
const componentDialogTitle = computed(() => `配件组成 - ${currentProduct.value?.name || ''}`)

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
  remark: '',
  components: []
})
const stockOutForm = reactive({
  quantity: 1,
  usageType: '',
  outboundTime: '',
  remark: ''
})
const outboundRecordQuery = reactive({
  pageNum: 1,
  pageSize: 30
})

const rules = {
  name: [{ required: true, message: '名称不能为空', trigger: 'blur' }],
  unit: [{ required: true, message: '请选择单位', trigger: 'change' }]
}
const stockOutRules = {
  quantity: [{ required: true, message: '请输入出库数量', trigger: 'change' }],
  outboundTime: [{ required: true, message: '请选择出库时间', trigger: 'change' }]
}

const typeOptions = ref([])
const unitOptions = ref([])
const usageTypeOptions = ref([])
const productTypeValue = ref('')
const componentTypeValue = ref('')
const productList = ref([])
const componentOptions = ref([])
const categoryTreeOptions = ref([])
const productComponentList = ref([])
const outboundRecordList = ref([])
const stockOutFormRef = ref()
const currentStockItem = reactive({
  id: undefined,
  name: ''
})

function loadDicts() {
  return proxy.getDicts(['dc_material_unit', 'dc_material_classification', 'dc_usagetype']).then((response) => {
    const source = response?.data?.data || response?.data?.result || response?.data || []
    const dicts = Array.isArray(source) ? source : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
    usageTypeOptions.value = getDictList(dicts, 'dc_usagetype')
    productTypeValue.value = resolveProductTypeValue()
    componentTypeValue.value = resolveComponentTypeValue()
    queryParams.type = productTypeValue.value
  })
}

function resolveComponentTypeValue() {
  const preferred = typeOptions.value.find((item) => isComponentType(item.dictValue))
  return preferred?.dictValue ?? '1'
}

function loadCategoryTree() {
  return listCategory({
    pageNum: 1,
    pageSize: 9999,
    categoryType: SPARE_PART_CATEGORY_TYPE
  }).then((response) => {
    const pageData = getPageData(response)
    const normalized = pageData.list.map(normalizeCategory)
    categoryTreeOptions.value = proxy.handleTree(normalized, 'id', 'parentId')
  })
}

function normalizeCategory(item) {
  const parentId = item.parentId ?? item.ParentId ?? 0
  return {
    id: item.id ?? item.Id ?? item.value,
    categoryName: item.categoryName ?? item.CategoryName ?? item.label ?? '',
    parentId: parentId === null || parentId === undefined ? 0 : parentId
  }
}

function collectCategoryDescendantIds(categoryId) {
  const ids = new Set()
  const collectFromTree = (nodes, parentMatched) => {
    nodes.forEach((node) => {
      const matched = parentMatched || String(node.id) === String(categoryId)
      if (matched) ids.add(String(node.id))
      if (node.children?.length) collectFromTree(node.children, matched)
    })
  }
  collectFromTree(categoryTreeOptions.value, false)
  return ids
}

function loadComponentOptions() {
  return listItems({
    pageNum: 1,
    pageSize: 9999,
    type: componentTypeValue.value
  }).then((response) => {
    const pageData = getPageData(response)
    componentOptions.value = pageData.list
      .map(normalizeComponentOption)
      .filter((item) => !item.type || isComponentType(item.type))
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

function isProductType(value) {
  const label = formatTypeLabel(value)
  return label === '产品' || String(value) === '2' || value === 'product' || value === 'finished'
}

function isComponentType(value) {
  const label = formatTypeLabel(value)
  return label === '配件' || String(value) === '1' || value === 'accessory' || value === 'component' || value === 'part'
}

function resolveProductTypeValue() {
  const preferred = typeOptions.value.find((item) => isProductType(item.dictValue))
  return preferred?.dictValue ?? ''
}

function getCurrentTime() {
  const now = new Date()
  const pad = (val) => String(val).padStart(2, '0')
  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())} ${pad(now.getHours())}:${pad(now.getMinutes())}:${pad(now.getSeconds())}`
}

function getPageData(response) {
  const raw = response?.data || {}
  // `raw` is often `{ result: Row[], totalNum, pageIndex, ... }`. Using `raw.result` as the
  // "data" node breaks extraction because `result` is the list array, not a wrapper object.
  const pageNode =
    raw?.data != null && !Array.isArray(raw.data)
      ? raw.data
      : raw?.result != null && !Array.isArray(raw.result)
        ? raw.result
        : raw
  const list = pageNode?.result || pageNode?.rows || pageNode?.list || pageNode?.items || []
  const totalNum = pageNode?.totalNum ?? pageNode?.total ?? pageNode?.count ?? list.length

  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function getListData(response) {
  const raw = response?.data || {}
  const pageNode =
    raw?.data != null && !Array.isArray(raw.data)
      ? raw.data
      : raw?.result != null && !Array.isArray(raw.result)
        ? raw.result
        : raw
  const list = pageNode?.result || pageNode?.rows || pageNode?.list || pageNode?.items || pageNode
  return Array.isArray(list) ? list : []
}

function normalizeItem(item) {
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name ?? '',
    type: item.type ?? item.Type ?? '',
    unit: item.unit ?? item.Unit ?? '',
    specification: item.specification ?? item.Specification ?? item.spec ?? item.Spec ?? '',
    remark: item.remark ?? item.Remark ?? ''
  }
}

function normalizeComponents(list) {
  if (!Array.isArray(list)) return []
  return list.map((item) => ({
    componentId: item.componentId ?? item.ComponentId ?? item.id ?? item.Id,
    componentName: item.componentName ?? item.ComponentName ?? item.name ?? item.Name ?? '',
    spec: item.spec ?? item.Spec ?? item.specification ?? item.Specification ?? '',
    unit: item.unit ?? item.Unit ?? '',
    quantity: Number(item.quantity ?? item.Quantity ?? 1)
  }))
}

function normalizeComponentOption(item) {
  const categoryId = item.categoryId ?? item.CategoryId
  return {
    value: item.value ?? item.id ?? item.Id ?? item.componentId ?? item.ComponentId,
    label: item.label ?? item.name ?? item.Name ?? item.componentName ?? item.ComponentName ?? '',
    name: item.name ?? item.Name ?? item.label ?? item.componentName ?? item.ComponentName ?? '',
    type: item.type ?? item.Type ?? '',
    categoryId: categoryId === 0 || categoryId === '0' ? undefined : categoryId,
    unit: item.unit ?? item.Unit ?? '',
    spec: item.spec ?? item.Spec ?? item.specification ?? item.Specification ?? ''
  }
}

function getFilteredComponentOptions(row) {
  if (!row.filterCategoryId) return componentOptions.value
  const matchIds = collectCategoryDescendantIds(row.filterCategoryId)
  return componentOptions.value.filter((item) => item.categoryId && matchIds.has(String(item.categoryId)))
}

function handleComponentCategoryChange(row) {
  if (!row.componentId) return
  const stillValid = getFilteredComponentOptions(row).some((item) => String(item.value) === String(row.componentId))
  if (!stillValid) row.componentId = undefined
}

function resolveComponentFilterCategoryId(componentId) {
  if (!componentId) return undefined
  const matched = componentOptions.value.find((item) => String(item.value) === String(componentId))
  return matched?.categoryId
}

function normalizeOutboundRecord(item) {
  return {
    id: item.id ?? item.Id,
    itemId: item.itemId ?? item.ItemId,
    name: item.name ?? item.Name ?? currentStockItem.name ?? '',
    type: item.type ?? item.Type ?? productTypeValue.value ?? '',
    unit: item.unit ?? item.Unit ?? '',
    quantity: Number(item.quantity ?? item.Quantity ?? 0),
    usageType: item.usageType ?? item.UsageType ?? '',
    outboundTime: formatTime(item.outboundTime ?? item.OutboundTime),
    remark: item.remark ?? item.Remark ?? ''
  }
}

function formatTime(value) {
  if (!value) return ''
  if (typeof value !== 'string') return value
  return value.replace('T', ' ').slice(0, 19)
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
      productList.value = normalizedList.filter((item) => isProductType(item.type))
      total.value = queryParams.type ? pageData.total : productList.value.length
    })
    .finally(() => {
      loading.value = false
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

function handleOutboundRecordPagination({ page, limit }) {
  outboundRecordQuery.pageNum = page
  outboundRecordQuery.pageSize = limit
  getOutboundRecordList()
}

function resetForm() {
  Object.assign(form, {
    id: undefined,
    name: '',
    unit: '',
    specification: '',
    remark: '',
    components: []
  })
  formRef.value?.clearValidate()
}

function handleAdd() {
  isEdit.value = false
  dialogTitle.value = '新增产品'
  resetForm()
  open.value = true
}

function handleEdit(row) {
  isEdit.value = true
  dialogTitle.value = '编辑产品'
  Object.assign(form, { ...row, components: [] })
  formRef.value?.clearValidate()
  open.value = true
  loadProductComponents(row.id, (components) => {
    form.components = components.map((item) => ({
      filterCategoryId: resolveComponentFilterCategoryId(item.componentId),
      componentId: item.componentId,
      quantity: item.quantity
    }))
  })
}

function cancel() {
  open.value = false
  resetForm()
}

function addComponentRow() {
  form.components.push({
    filterCategoryId: undefined,
    componentId: undefined,
    quantity: 1
  })
}

function removeComponentRow(index) {
  form.components.splice(index, 1)
}

function isComponentSelected(componentId, currentRow) {
  if (!componentId) return false
  return form.components.some((item) => item !== currentRow && String(item.componentId) === String(componentId))
}

function formatComponentOptionLabel(item) {
  const spec = item.spec ? ` / ${item.spec}` : ''
  return `${item.name || item.label}${spec} / ${formatDictLabel(unitOptions.value, item.unit)}`
}

function validateComponents() {
  const usedComponentIds = new Set()
  for (const item of form.components) {
    const hasComponent = Boolean(item.componentId)
    const hasQuantity = item.quantity !== undefined && item.quantity !== null && item.quantity !== ''
    if (!hasComponent && !hasQuantity) continue

    if (!hasComponent || !hasQuantity || Number(item.quantity) <= 0) {
      proxy.$modal.msgError('请完整填写配件和大于 0 的用量')
      return false
    }

    const key = String(item.componentId)
    if (usedComponentIds.has(key)) {
      proxy.$modal.msgError('同一个产品下不能重复选择相同配件')
      return false
    }
    usedComponentIds.add(key)
  }
  return true
}

function getSubmitData() {
  return {
    id: form.id,
    name: form.name.trim(),
    type: productTypeValue.value,
    unit: form.unit,
    spec: form.specification.trim(),
    remark: form.remark.trim(),
    components: form.components
      .filter((item) => item.componentId || item.quantity)
      .map((item) => ({
        componentId: item.componentId,
        quantity: Number(item.quantity)
      }))
  }
}

function submitForm() {
  formRef.value?.validate((valid) => {
    if (!valid) return
    if (!validateComponents()) return
    const submitApi = isEdit.value ? editItem : addItem
    submitApi(getSubmitData()).then(() => {
      proxy.$modal.msgSuccess(isEdit.value ? '修改成功' : '新增成功')
      open.value = false
      resetForm()
      loadComponentOptions()
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
      loadComponentOptions()
      getList()
    })
    .catch(() => {})
}

function handleViewComponents(row) {
  currentProduct.value = { ...row }
  componentOpen.value = true
  loadProductComponents(row.id, (components) => {
    productComponentList.value = components
  })
}

function handleViewOutboundRecords(row) {
  Object.assign(currentStockItem, {
    id: row.id,
    name: row.name
  })
  outboundRecordQuery.pageNum = 1
  outboundRecordOpen.value = true
}

function getOutboundRecordList() {
  if (!currentStockItem.id) return
  outboundRecordLoading.value = true
  listOutboundRecords({
    pageNum: outboundRecordQuery.pageNum,
    pageSize: outboundRecordQuery.pageSize,
    itemId: currentStockItem.id
  })
    .then((response) => {
      const pageData = getPageData(response)
      outboundRecordList.value = pageData.list.map(normalizeOutboundRecord)
      outboundRecordTotal.value = pageData.total
    })
    .finally(() => {
      outboundRecordLoading.value = false
    })
}

function handleStockOut(row) {
  Object.assign(currentStockItem, {
    id: row.id,
    name: row.name
  })
  resetStockOutForm()
  stockOutOpen.value = true
}

function resetStockOutForm() {
  Object.assign(stockOutForm, {
    quantity: 1,
    usageType: '',
    outboundTime: getCurrentTime(),
    remark: ''
  })
  stockOutFormRef.value?.clearValidate()
}

function submitStockOut() {
  stockOutFormRef.value?.validate((valid) => {
    if (!valid) return
    const quantity = Number(stockOutForm.quantity)
    if (quantity <= 0) {
      proxy.$modal.msgError('出库数量必须大于 0')
      return
    }
    stockOutLoading.value = true
    addOutboundRecord({
      ItemId: currentStockItem.id,
      Quantity: quantity,
      UsageType: String(stockOutForm.usageType || '').trim(),
      OutboundTime: stockOutForm.outboundTime,
      Remark: stockOutForm.remark.trim()
    })
      .then(() => {
        proxy.$modal.msgSuccess('出库成功')
        stockOutOpen.value = false
        getList()
      })
      .finally(() => {
        stockOutLoading.value = false
      })
  })
}

function loadProductComponents(productId, callback) {
  if (!productId) {
    callback([])
    return
  }
  componentLoading.value = true
  listProductComponents(productId)
    .then((response) => {
      callback(normalizeComponents(getListData(response)))
    })
    .finally(() => {
      componentLoading.value = false
    })
}

loadDicts()
  .then(() => Promise.all([loadCategoryTree(), loadComponentOptions()]))
  .then(() => {
    resetForm()
    getList()
  })
</script>

<style scoped>
.product-management {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-form-item {
  margin-right: 16px;
  margin-bottom: 0;
}

.query-control {
  width: 220px;
}

.product-table,
.component-view-table {
  width: 100%;
}

.product-table :deep(.el-table__header-wrapper th),
.component-view-table :deep(.el-table__header-wrapper th) {
  background: #f2f6fc;
  color: #303133;
}

.product-table :deep(.el-table__header-wrapper th .cell),
.component-view-table :deep(.el-table__header-wrapper th .cell) {
  height: 20px;
  line-height: 20px;
  padding-top: 0;
  padding-bottom: 0;
  font-size: 11px;
}

.product-table :deep(.el-table__body-wrapper td),
.component-view-table :deep(.el-table__body-wrapper td) {
  padding-top: 0;
  padding-bottom: 0;
}

.product-table :deep(.el-table__body-wrapper td .cell),
.component-view-table :deep(.el-table__body-wrapper td .cell) {
  height: 24px;
  line-height: 24px;
  padding-top: 0;
  padding-bottom: 0;
}

.form-control {
  width: 100%;
}

.component-section {
  width: 100%;
}

.component-edit-table {
  width: 100%;
}

.component-edit-table :deep(.el-table__header-wrapper th) {
  background: #f2f6fc;
  color: #303133;
}

.component-edit-table :deep(.el-table__body-wrapper td .cell) {
  min-height: 42px;
  line-height: normal;
  padding-top: 6px;
  padding-bottom: 6px;
}

.component-form-item :deep(.el-form-item__content) {
  display: block;
}

.product-edit-dialog :deep(.el-dialog__body) {
  max-height: 70vh;
  overflow-y: auto;
  padding-right: 20px;
}

.component-quantity {
  width: 120px;
}

.component-add-button {
  margin-top: 10px;
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

.op-btn.is-danger {
  color: #f56c6c;
}

.op-btn.is-warning {
  color: #e6a23c;
}

.readonly-text {
  display: inline-block;
  width: 100%;
  color: #303133;
  line-height: 28px;
}

</style>