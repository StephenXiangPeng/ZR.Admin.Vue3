<template>
  <div class="app-container item-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="名称" prop="name">
        <el-input v-model="queryParams.name" placeholder="请输入配件/产品名称" clearable class="query-control" @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="类型" prop="type">
        <el-select v-model="queryParams.type" placeholder="请选择类型" clearable class="query-control" @change="handleQuery">
          <el-option v-for="item in typeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="itemList" border class="item-table">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" />
      <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="type" width="120" align="center">
        <template #default="{ row }">
          <el-tag :type="getTypeTagType(row.type)">{{ formatTypeLabel(row.type) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="单位" prop="unit" width="100" align="center">
        <template #default="{ row }">
          {{ formatDictLabel(unitOptions, row.unit) }}
        </template>
      </el-table-column>
      <el-table-column label="规格" prop="specification" min-width="160" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.specification || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="当前库存" prop="stockQuantity" width="100" align="center">
        <template #default="{ row }">
          {{ formatStock(row.stockQuantity) }}
        </template>
      </el-table-column>
      <el-table-column label="备注" prop="remark" min-width="180" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ row.remark || '-' }}
        </template>
      </el-table-column>
      <el-table-column label="配件组成" width="120" align="center">
        <template #default="{ row }">
          <el-button v-if="hasComponentRelation(row)" text size="small" type="primary" @click="handleViewComponents(row)">查看配件</el-button>
          <span v-else-if="isProductType(row.type)">-</span>
          <span v-else>-</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="160" align="center" fixed="right">
        <template #default="{ row }">
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

    <el-dialog :title="dialogTitle" v-model="open" width="760px" append-to-body @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入配件/产品名称" maxlength="50" show-word-limit />
        </el-form-item>
        <el-form-item label="类型" prop="type">
          <el-select v-model="form.type" placeholder="请选择类型" clearable class="form-select" @change="handleFormTypeChange">
            <el-option v-for="item in typeOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="单位" prop="unit">
          <el-select v-model="form.unit" placeholder="请选择单位" clearable class="form-select">
            <el-option v-for="item in unitOptions" :key="item.dictValue" :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item label="规格" prop="specification">
          <el-input v-model="form.specification" placeholder="请输入规格，如型号、尺寸、颜色等" maxlength="100" show-word-limit />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="200" show-word-limit :rows="3" />
        </el-form-item>
        <el-form-item v-if="isProductType(form.type)" label="配件组成">
          <div class="component-section">
            <el-table :data="form.components" border class="component-edit-table" empty-text="暂无配件组成">
              <el-table-column label="配件" min-width="220">
                <template #default="{ row }">
                  <el-select v-model="row.componentId" placeholder="请选择配件" filterable clearable class="form-select">
                    <el-option
                      v-for="item in componentOptions"
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
  </div>
</template>

<script setup name="ItemManagement">
import { computed, getCurrentInstance, reactive, ref } from 'vue'
import { addItem, deleteItems, editItem, listComponentOptions, listItems, listProductComponents } from '@/api/dcbl/items'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const open = ref(false)
const dialogTitle = ref('')
const isEdit = ref(false)
const loading = ref(false)
const total = ref(0)
const componentOpen = ref(false)
const componentLoading = ref(false)
const currentProduct = ref({})

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: '',
  type: ''
})

const typeOptions = ref([])
const unitOptions = ref([])
const itemList = ref([])
const componentOptions = ref([])
const productComponentList = ref([])

const form = reactive({
  id: undefined,
  name: '',
  type: '',
  unit: '',
  specification: '',
  remark: '',
  components: []
})

const rules = {
  name: [{ required: true, message: '名称不能为空', trigger: 'blur' }],
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  unit: [{ required: true, message: '请选择单位', trigger: 'change' }]
}

const componentDialogTitle = computed(() => `配件组成 - ${currentProduct.value?.name || ''}`)

function loadDicts() {
  proxy.getDicts(['dc_material_unit', 'dc_material_classification']).then((response) => {
    const dicts = Array.isArray(response.data) ? response.data : []
    typeOptions.value = getDictList(dicts, 'dc_material_classification')
    unitOptions.value = getDictList(dicts, 'dc_material_unit')
  })
}

function loadComponentOptions() {
  listComponentOptions().then((response) => {
    componentOptions.value = getListData(response)
      .map(normalizeComponentOption)
      .filter((item) => !item.type || isComponentType(item.type))
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

function hasComponentRelation(row) {
  if (!isProductType(row.type)) return false
  if (Array.isArray(row.components) && row.components.length > 0) return true
  if (Number(row.componentCount || 0) > 0) return true
  return row.hasComponents !== false && row.componentCount === undefined
}

function getTypeTagType(value) {
  return isProductType(value) ? 'success' : 'info'
}

function formatStock(value) {
  return Number(value || 0)
}

function getTableIndex(index) {
  return (queryParams.pageNum - 1) * queryParams.pageSize + index + 1
}

function getList() {
  loading.value = true
  listItems({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    name: queryParams.name,
    type: queryParams.type
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

function getPageData(response) {
  const data = response?.data || {}
  const list = data.result || data.rows || data.list || data.items || []
  const totalNum = data.totalNum ?? data.total ?? data.count ?? list.length

  return {
    list: Array.isArray(list) ? list : [],
    total: Number(totalNum || 0)
  }
}

function getListData(response) {
  const data = response?.data || []
  const list = Array.isArray(data) ? data : data.result || data.rows || data.list || data.items || []
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
    remark: item.remark ?? item.Remark ?? '',
    components: normalizeComponents(item.components ?? item.Components ?? []),
    componentCount: item.componentCount ?? item.ComponentCount,
    hasComponents: item.hasComponents ?? item.HasComponents
  }
}

function normalizeComponentOption(item) {
  return {
    value: item.value ?? item.id ?? item.Id ?? item.componentId ?? item.ComponentId,
    label: item.label ?? item.name ?? item.Name ?? item.componentName ?? item.ComponentName ?? '',
    name: item.name ?? item.Name ?? item.label ?? item.componentName ?? item.ComponentName ?? '',
    type: item.type ?? item.Type ?? '',
    unit: item.unit ?? item.Unit ?? '',
    spec: item.spec ?? item.Spec ?? item.specification ?? item.Specification ?? ''
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

function getSubmitData() {
  return {
    id: form.id,
    name: form.name.trim(),
    type: form.type,
    unit: form.unit,
    spec: form.specification.trim(),
    remark: form.remark.trim(),
    components: isProductType(form.type)
      ? form.components
          .filter((item) => item.componentId || item.quantity)
          .map((item) => ({
            componentId: item.componentId,
            quantity: Number(item.quantity)
          }))
      : []
  }
}

function resetForm() {
  Object.assign(form, {
    id: undefined,
    name: '',
    type: '',
    unit: '',
    specification: '',
    remark: '',
    components: []
  })
  formRef.value?.clearValidate()
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

function handleAdd() {
  isEdit.value = false
  dialogTitle.value = '新增配件/产品'
  resetForm()
  open.value = true
}

function handleEdit(row) {
  isEdit.value = true
  dialogTitle.value = '编辑配件/产品'
  Object.assign(form, { ...row, components: [] })
  formRef.value?.clearValidate()
  open.value = true
  if (isProductType(row.type)) {
    loadProductComponents(row.id, (components) => {
      form.components = components.map((item) => ({
        componentId: item.componentId,
        quantity: item.quantity
      }))
    })
  }
}

function cancel() {
  open.value = false
  resetForm()
}

function handleFormTypeChange(value) {
  if (!isProductType(value)) {
    form.components = []
  }
}

function addComponentRow() {
  form.components.push({
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
  if (!isProductType(form.type)) return true

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

function submitForm() {
  formRef.value.validate((valid) => {
    if (!valid) return
    if (!validateComponents()) return

    const submitData = getSubmitData()
    if (isEdit.value) {
      editItem(submitData).then(() => {
        proxy.$modal.msgSuccess('修改成功')
        open.value = false
        resetForm()
        loadComponentOptions()
        getList()
      })
    } else {
      addItem(submitData).then(() => {
        proxy.$modal.msgSuccess('新增成功')
        open.value = false
        resetForm()
        loadComponentOptions()
        getList()
      })
    }
  })
}

function handleDelete(row) {
  proxy.$modal
    .confirm(`是否确认删除"${row.name}"？`)
    .then(() => {
      return deleteItems([row.id])
    })
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
loadComponentOptions()
getList()
</script>

<style scoped>
.item-management {
  background: #fff;
}

.search-form {
  padding-bottom: 4px;
}

.query-control {
  width: 220px;
}

.item-table {
  width: 100%;
}

.form-select {
  width: 100%;
}

.component-section {
  width: 100%;
}

.component-edit-table,
.component-view-table {
  width: 100%;
}

.component-quantity {
  width: 120px;
}

.component-add-button {
  margin-top: 10px;
}
</style>