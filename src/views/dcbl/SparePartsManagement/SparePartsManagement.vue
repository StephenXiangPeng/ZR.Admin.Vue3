<template>
  <div class="app-container spare-parts-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form" @submit.prevent>
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
      <el-form-item label="所属分类" prop="categoryId" class="query-form-item">
        <el-tree-select
          v-model="queryParams.categoryId"
          :data="categoryTreeOptions"
          :props="categoryTreeProps"
          value-key="id"
          placeholder="全部分类"
          clearable
          filterable
          check-strictly
          class="query-control"
          @change="handleQuery" 
          size="default"/>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" size="default">新增零配件</el-button>
      </el-col>
    </el-row>

    <el-table v-loading="loading" :data="sparePartList" border stripe class="parts-table" :height="tableHeight">
      <el-table-column type="index" label="序号" width="70" align="center" :index="getTableIndex" v-if="false"/>
      <el-table-column label="名称" prop="name" min-width="160" :show-overflow-tooltip="true" />
      <el-table-column label="所属分类" prop="categoryId" min-width="140" :show-overflow-tooltip="true">
        <template #default="{ row }">
          {{ getCategoryLabel(row.categoryId) }}
        </template>
      </el-table-column>
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
          <el-input v-model="form.name" maxlength="50" show-word-limit placeholder="请输入零配件名称" />
        </el-form-item>
        <el-form-item label="所属分类" prop="categoryId">
          <div class="category-field-row">
            <el-tree-select
              v-model="form.categoryId"
              :data="categoryTreeOptions"
              :props="categoryTreeProps"
              value-key="id"
              placeholder="请选择所属分类"
              clearable
              filterable
              check-strictly
              class="form-control category-select" />
            <el-button type="primary" link @click="openCategoryManage">维护分类</el-button>
          </div>
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

    <el-dialog v-model="categoryManageOpen" title="分类维护" width="720px" append-to-body @open="loadCategoryManageTree">
      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="Plus" size="default" @click="handleCategoryAdd()">新增一级分类</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="info" plain icon="Sort" size="default" @click="toggleCategoryExpandAll">展开/折叠</el-button>
        </el-col>
      </el-row>
      <el-table
        v-if="categoryTableRefresh"
        v-loading="categoryLoading"
        :data="categoryManageTree"
        row-key="id"
        border
        :default-expand-all="categoryExpandAll"
        :tree-props="{ children: 'children' }"
        max-height="420">
        <el-table-column prop="categoryName" label="分类名称" min-width="200" :show-overflow-tooltip="true" />
        <el-table-column prop="sort" label="排序" width="80" align="center" />
        <el-table-column prop="status" label="状态" width="90" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'info'" effect="light">{{ row.status === 1 ? '启用' : '停用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" align="center" fixed="right">
          <template #default="{ row }">
            <el-button text size="small" class="op-btn" @click="handleCategoryAdd(row)">添加子级</el-button>
            <el-button text size="small" class="op-btn" @click="handleCategoryEdit(row)">编辑</el-button>
            <el-button text size="small" class="op-btn is-danger" @click="handleCategoryDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <template #footer>
        <el-button type="primary" @click="categoryManageOpen = false">关闭</el-button>
      </template>
    </el-dialog>

    <el-dialog
      v-model="categoryFormOpen"
      :title="categoryFormTitle"
      width="520px"
      append-to-body
      @close="resetCategoryForm">
      <el-form ref="categoryFormRef" :model="categoryForm" :rules="categoryRules" label-width="90px" @submit.prevent>
        <el-form-item v-if="categoryForm.id" label="上级分类" prop="parentId">
          <el-tree-select
            v-model="categoryForm.parentId"
            :data="categoryParentTreeOptions"
            :props="categoryTreeProps"
            value-key="id"
            placeholder="不选则为一级分类"
            clearable
            filterable
            check-strictly
            class="form-control" />
        </el-form-item>
        <el-form-item v-else label="上级分类">
          <span class="readonly-text">{{ categoryParentLabel }}</span>
        </el-form-item>
        <el-form-item label="分类名称" prop="categoryName">
          <el-input v-model="categoryForm.categoryName" maxlength="50" show-word-limit placeholder="请输入分类名称" />
        </el-form-item>
        <el-form-item label="排序" prop="sort">
          <el-input-number v-model="categoryForm.sort" :min="0" :precision="0" controls-position="right" class="form-control" />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="categoryForm.status">
            <el-radio :value="1">启用</el-radio>
            <el-radio :value="0">停用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="categoryForm.remark" type="textarea" maxlength="200" show-word-limit :rows="2" placeholder="选填" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button text @click="categoryFormOpen = false">取消</el-button>
        <el-button type="primary" @click="submitCategoryForm">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="SparePartsManagement">
import { computed, getCurrentInstance, nextTick, reactive, ref } from 'vue'
import {
  SPARE_PART_CATEGORY_TYPE,
  addCategory,
  deleteCategory,
  editCategory,
  listCategory
} from '@/api/dcbl/category'
import { addInboundRecord, listInboundRecords } from '@/api/dcbl/inboundRecords'
import { addItem, deleteItems, editItem, listItems } from '@/api/dcbl/items'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const stockInFormRef = ref()
const categoryFormRef = ref()
const queryDebounceTimer = ref()

const categoryTreeProps = { value: 'id', label: 'categoryName', children: 'children' }

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

const categoryManageOpen = ref(false)
const categoryFormOpen = ref(false)
const categoryLoading = ref(false)
const categoryExpandAll = ref(true)
const categoryTableRefresh = ref(true)
const categoryFormTitle = ref('')
const categoryIsEdit = ref(false)
const categoryParentLabel = ref('一级分类（无上级）')

const tableHeight = computed(() => 'calc(100vh - 290px)')

const queryParams = reactive({
  pageNum: 1,
  pageSize: 30,
  name: '',
  type: '',
  categoryId: undefined
})

const form = reactive({
  id: undefined,
  name: '',
  categoryId: undefined,
  unit: '',
  specification: '',
  remark: ''
})

const categoryForm = reactive({
  id: undefined,
  categoryName: '',
  parentId: 0,
  categoryType: SPARE_PART_CATEGORY_TYPE,
  sort: 0,
  status: 1,
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
const categoryTreeOptions = ref([])
const categoryManageTree = ref([])
const categoryParentTreeOptions = ref([])
const categoryLabelMap = ref({})
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

const categoryRules = {
  categoryName: [{ required: true, message: '分类名称不能为空', trigger: 'blur' }]
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
  const categoryId = item.categoryId ?? item.CategoryId
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name ?? '',
    type: item.type ?? item.Type ?? '',
    categoryId: categoryId === 0 || categoryId === '0' ? undefined : categoryId,
    unit: item.unit ?? item.Unit ?? '',
    stockQuantity: item.stockQuantity ?? item.StockQuantity ?? 0,
    specification: item.specification ?? item.Specification ?? item.spec ?? item.Spec ?? '',
    remark: item.remark ?? item.Remark ?? ''
  }
}

function normalizeCategory(item) {
  const parentId = item.parentId ?? item.ParentId ?? 0
  return {
    id: item.id ?? item.Id ?? item.value,
    categoryName: item.categoryName ?? item.CategoryName ?? item.label ?? '',
    parentId: parentId === null || parentId === undefined ? 0 : parentId,
    categoryType: item.categoryType ?? item.CategoryType ?? SPARE_PART_CATEGORY_TYPE,
    sort: item.sort ?? item.Sort ?? 0,
    status: item.status ?? item.Status ?? 1,
    remark: item.remark ?? item.Remark ?? ''
  }
}

function buildCategoryMaps(list) {
  const map = {}
  list.forEach((item) => {
    map[item.id] = item.categoryName
  })
  categoryLabelMap.value = map
}

function buildCategoryTree(list) {
  const normalized = list.map(normalizeCategory)
  buildCategoryMaps(normalized)
  const tree = proxy.handleTree(normalized, 'id', 'parentId')
  categoryTreeOptions.value = tree
  return tree
}

function getCategoryLabel(categoryId) {
  if (!categoryId) return '-'
  return categoryLabelMap.value[categoryId] || '-'
}

function loadCategoryTree() {
  return listCategory({
    pageNum: 1,
    pageSize: 9999,
    categoryType: SPARE_PART_CATEGORY_TYPE
  }).then((response) => {
    const pageData = getPageData(response)
    return buildCategoryTree(pageData.list)
  })
}

function loadCategoryManageTree() {
  categoryLoading.value = true
  return loadCategoryTree()
    .then((tree) => {
      categoryManageTree.value = tree
    })
    .finally(() => {
      categoryLoading.value = false
    })
}

function filterCategoryTreeForParent(tree, excludeId) {
  if (!excludeId) return tree
  const clone = JSON.parse(JSON.stringify(tree))
  const prune = (nodes) =>
    nodes.filter((node) => {
      if (node.id === excludeId) return false
      if (node.children?.length) {
        node.children = prune(node.children)
      }
      return true
    })
  return prune(clone)
}

function openCategoryManage() {
  categoryManageOpen.value = true
}

function toggleCategoryExpandAll() {
  categoryTableRefresh.value = false
  categoryExpandAll.value = !categoryExpandAll.value
  nextTick(() => {
    categoryTableRefresh.value = true
  })
}

function resetCategoryForm() {
  Object.assign(categoryForm, {
    id: undefined,
    categoryName: '',
    parentId: 0,
    categoryType: SPARE_PART_CATEGORY_TYPE,
    sort: 0,
    status: 1,
    remark: ''
  })
  categoryParentLabel.value = '一级分类（无上级）'
  categoryFormRef.value?.clearValidate()
}

function handleCategoryAdd(parentRow) {
  categoryIsEdit.value = false
  categoryFormTitle.value = parentRow ? '新增子级分类' : '新增一级分类'
  resetCategoryForm()
  if (parentRow) {
    categoryForm.parentId = parentRow.id
    categoryParentLabel.value = parentRow.categoryName
  }
  categoryFormOpen.value = true
}

function handleCategoryEdit(row) {
  categoryIsEdit.value = true
  categoryFormTitle.value = '编辑分类'
  Object.assign(categoryForm, normalizeCategory(row))
  categoryParentTreeOptions.value = filterCategoryTreeForParent(categoryTreeOptions.value, row.id)
  categoryFormOpen.value = true
}

function handleCategoryDelete(row) {
  proxy.$modal
    .confirm(`是否确认删除分类"${row.categoryName}"？`)
    .then(() => deleteCategory([row.id]))
    .then(() => {
      proxy.$modal.msgSuccess('删除成功')
      loadCategoryManageTree()
    })
    .catch(() => {})
}

function getCategorySubmitData() {
  const parentId = Number(categoryForm.parentId) || 0
  return {
    id: categoryForm.id,
    categoryName: categoryForm.categoryName.trim(),
    parentId,
    categoryType: SPARE_PART_CATEGORY_TYPE,
    sort: Number(categoryForm.sort) || 0,
    status: categoryForm.status,
    remark: categoryForm.remark?.trim() || ''
  }
}

function submitCategoryForm() {
  categoryFormRef.value?.validate((valid) => {
    if (!valid) return
    const payload = getCategorySubmitData()
    const submitApi = categoryIsEdit.value ? editCategory : addCategory
    submitApi(payload).then(() => {
      proxy.$modal.msgSuccess(categoryIsEdit.value ? '修改成功' : '新增成功')
      categoryFormOpen.value = false
      loadCategoryManageTree()
    })
  })
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
  const params = {
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    name: queryParams.name.trim(),
    type: queryParams.type || componentTypeValue.value
  }
  if (queryParams.categoryId) {
    params.CategoryId = Number(queryParams.categoryId) || queryParams.categoryId
  }
  listItems(params)
    .then((response) => {
      const pageData = getPageData(response)
      sparePartList.value = pageData.list.map(normalizeItem)
      total.value = pageData.total
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
    categoryId: undefined,
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
    categoryId: form.categoryId || 0,
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

loadDicts()
  .then(() => loadCategoryTree())
  .then(() => {
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
}

.query-form-item {
  margin-right: 16px;
  margin-bottom: 0;
}

.query-control {
  width: 220px;
}

.category-field-row {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
}

.category-select {
  flex: 1;
  min-width: 0;
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