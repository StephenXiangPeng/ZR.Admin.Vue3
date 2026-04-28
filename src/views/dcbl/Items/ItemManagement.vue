<template>
  <div class="app-container item-management">
    <el-form ref="queryRef" :model="queryParams" :inline="true" class="search-form">
      <el-form-item label="名称" prop="name">
        <el-input v-model="queryParams.name" placeholder="请输入物料/产品名称" clearable class="query-control" @keyup.enter="handleQuery" />
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
          <el-tag :type="getTypeTagType(row.type)">{{ formatDictLabel(typeOptions, row.type) }}</el-tag>
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

    <el-dialog :title="dialogTitle" v-model="open" width="520px" append-to-body @close="cancel">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入物料/产品名称" maxlength="50" show-word-limit />
        </el-form-item>
        <el-form-item label="类型" prop="type">
          <el-select v-model="form.type" placeholder="请选择类型" clearable class="form-select">
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
      </el-form>
      <template #footer>
        <el-button text @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="ItemManagement">
import { getCurrentInstance, reactive, ref } from 'vue'
import { addItem, deleteItems, editItem, listItems } from '@/api/dcbl/items'

const { proxy } = getCurrentInstance()

const queryRef = ref()
const formRef = ref()
const open = ref(false)
const dialogTitle = ref('')
const isEdit = ref(false)
const loading = ref(false)
const total = ref(0)

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: '',
  type: ''
})

const typeOptions = ref([])
const unitOptions = ref([])
const itemList = ref([])

const form = reactive({
  id: undefined,
  name: '',
  type: '',
  unit: '',
  specification: '',
  remark: ''
})

const rules = {
  name: [{ required: true, message: '名称不能为空', trigger: 'blur' }],
  type: [{ required: true, message: '请选择类型', trigger: 'change' }],
  unit: [{ required: true, message: '请选择单位', trigger: 'change' }]
}

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

function getSubmitData() {
  return {
    Id: form.id,
    Name: form.name.trim(),
    Type: form.type,
    Unit: form.unit,
    Spec: form.specification.trim(),
    Remark: form.remark.trim()
  }
}

function resetForm() {
  Object.assign(form, {
    id: undefined,
    name: '',
    type: '',
    unit: '',
    specification: '',
    remark: ''
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
  dialogTitle.value = '新增物料/产品'
  resetForm()
  open.value = true
}

function handleEdit(row) {
  isEdit.value = true
  dialogTitle.value = '编辑物料/产品'
  Object.assign(form, { ...row })
  formRef.value?.clearValidate()
  open.value = true
}

function cancel() {
  open.value = false
  resetForm()
}

function submitForm() {
  formRef.value.validate((valid) => {
    if (!valid) return

    const submitData = getSubmitData()
    if (isEdit.value) {
      editItem(submitData).then(() => {
        proxy.$modal.msgSuccess('修改成功')
        open.value = false
        resetForm()
        getList()
      })
    } else {
      addItem(submitData).then(() => {
        proxy.$modal.msgSuccess('新增成功')
        open.value = false
        resetForm()
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
      getList()
    })
    .catch(() => {})
}

loadDicts()
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
</style>