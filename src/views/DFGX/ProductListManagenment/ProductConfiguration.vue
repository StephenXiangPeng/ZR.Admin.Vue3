<template>
  <div class="app-container">
    <!-- 搜索区域 -->
    <el-form :model="queryParams" ref="queryForm" :inline="true" v-show="showSearch">
      <el-form-item label="选项类型" prop="optionType">
        <el-select v-model="queryParams.optionType" placeholder="请选择选项类型" clearable>
          <el-option 
            v-for="dict in optionTypeOptions" 
            :key="dict.dictValue" 
            :label="dict.dictLabel" 
            :value="parseInt(dict.dictValue)" 
          />
        </el-select>
      </el-form-item>
      <el-form-item label="选项名称" prop="optionName">
        <el-input
          v-model="queryParams.optionName"
          placeholder="请输入选项名称"
          clearable
          @keyup.enter="handleQuery"
        />
      </el-form-item>
      <el-form-item label="状态" prop="status">
        <el-select v-model="queryParams.status" placeholder="请选择状态" clearable>
          <el-option label="正常" value="0" />
          <el-option label="停用" value="1" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <!-- 操作按钮区域 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button
          type="primary"
          plain
          icon="Plus"
          @click="handleAdd"
        >新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="success"
          plain
          icon="Edit"
          :disabled="single"
          @click="handleUpdate"
        >修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button
          type="danger"
          plain
          icon="Delete"
          :disabled="multiple"
          @click="handleBatchDelete"
        >删除</el-button>
      </el-col>
      <!-- <el-col :span="1.5">
        <el-button
          type="warning"
          plain
          icon="Download"
          @click="handleExport"
          v-hasPermi="['DFGX:lensOptions:export']"
        >导出</el-button>
      </el-col> -->
      <el-col :span="1.5">
        <el-button
          type="info"
          plain
          icon="Setting"
          @click="handleSpecificationConfig"
        >规格配置</el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <!-- 数据表格 -->
    <el-table v-loading="loading" :data="lensOptionsList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="序号" type="index" width="60" align="center" />
      <el-table-column label="选项类型" align="center" prop="optionType" width="110">
        <template #default="scope">
          <el-tag :type="getTagType(scope.row.optionType)">
            {{ getOptionTypeLabel(scope.row.optionType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="选项名称" align="center" prop="optionName" width="200" :show-overflow-tooltip="true" />
      <el-table-column label="状态" align="center" prop="status" width="120">
        <template #default="scope">
          <el-switch
            v-model="scope.row.status"
            active-value="0"
            inactive-value="1"
            @change="handleStatusChange(scope.row)"
          />
        </template>
      </el-table-column>
      <el-table-column label="备注" align="center" prop="remark" :show-overflow-tooltip="true" width="500">
        <template #default="scope">
          <span>{{ scope.row.remark && scope.row.remark.trim() ? scope.row.remark : '无' }}</span>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" width="180">
        <template #default="scope">
          <span>{{ scope.row.createTime }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" class-name="small-padding fixed-width" width="200">
        <template #default="scope">
          <el-button
            link
            type="primary"
            icon="Edit"
            @click="handleUpdate(scope.row)"
          >修改</el-button>
          <el-button
            link
            type="danger"
            icon="Delete"
            @click="handleDelete(scope.row)"
          >删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="getList"
    />

    <!-- 添加或修改镜片选项对话框 -->
    <el-dialog 
      :title="title" 
      v-model="open" 
      width="480px" 
      append-to-body
      :close-on-click-modal="false"
      :close-on-press-escape="false"
      @close="cancel"
    >
      <el-form 
        ref="lensOptionRef" 
        :model="form" 
        :rules="rules" 
        label-width="80px"
        class="lens-option-form"
      >
        <el-form-item label="选项类型" prop="optionType">
          <el-select 
            v-model="form.optionType" 
            placeholder="请选择选项类型"
            style="width: 100%"
            clearable
            @change="handleOptionTypeChange"
          >
            <el-option 
              v-for="dict in optionTypeOptions" 
              :key="dict.dictValue" 
              :label="dict.dictLabel" 
              :value="parseInt(dict.dictValue)"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="选项名称" prop="optionName">
          <el-input 
            v-model="form.optionName" 
            placeholder="请输入选项名称"
            maxlength="100"
            show-word-limit
            clearable
          />
        </el-form-item>
        
        <!-- 模型图片上传 - 仅当选项类型为model(optionType=8)时显示 -->
        <el-form-item 
          label="模型图片" 
          prop="imageUrl" 
          v-if="form.optionType === 8"
        >
          <UploadImage
            v-model="form.imageUrl"
            ref="modelImageRef"
            :limit="1"
            :file-size="5"
            :file-type="['png','jpg','jpeg','webp']"
            :is-show-tip="true"
            :style="{ width: '120px' }"
            list-type="picture-card"
            :auto-upload="false"
            @success="onModelImageUploaded"
          />
          <div class="form-tip">
            <el-text type="info" size="small">
              上传一张模型图片（支持 jpg、png、jpeg、webp，最大 5MB）
            </el-text>
          </div>
        </el-form-item>

        <el-row :gutter="12">
          <el-col :span="12">
            <el-form-item label="选项值" prop="optionValue">
              <el-input-number 
                v-model="form.optionValue" 
                placeholder="自动" 
                :min="1" 
                :max="999" 
                :disabled="!form.id"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="form.status">
                <el-radio label="0">正常</el-radio>
                <el-radio label="1">停用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        
        <!-- 关联材质下拉框 - 仅当选项类型为折射率时显示 -->
        <el-form-item 
          label="关联材质" 
          prop="relatedMaterial" 
          v-if="form.optionType === 5"
        >
          <el-select 
            v-model="form.relatedMaterial" 
            placeholder="请选择关联材质（支持多选）"
            style="width: 100%"
            clearable
            filterable
            multiple
            collapse-tags
            collapse-tags-tooltip
            :max-collapse-tags="2"
          >
            <el-option 
              v-for="material in materialOptions" 
              :key="material.value" 
              :label="material.label" 
              :value="material.value"
            />
          </el-select>
          <div class="form-tip">
            <el-text type="info" size="small">
              提示：可以选择多个材质，系统会以数组格式保存
            </el-text>
          </div>
        </el-form-item>
        
        <!-- 关联设计下拉框 - 仅当选项类型为材质时显示 -->
        <el-form-item 
          label="关联设计" 
          prop="relatedDesign" 
          v-if="form.optionType === 3"
        >
          <el-select 
            v-model="form.relatedDesign" 
            placeholder="请选择关联设计（支持多选）"
            style="width: 100%"
            clearable
            filterable
            multiple
            collapse-tags
            collapse-tags-tooltip
            :max-collapse-tags="2"
          >
            <el-option 
              v-for="design in designOptions" 
              :key="design.value" 
              :label="design.label" 
              :value="design.value"
            />
          </el-select>
          <div class="form-tip">
            <el-text type="info" size="small">
              提示：可以选择多个设计，系统会以数组格式保存
            </el-text>
          </div>
        </el-form-item>
        
        <!-- 车房类型复选框 - 仅当选项类型为材质、膜层或设计时显示 -->
        <el-form-item 
          label="车房类型" 
          v-if="form.optionType === 2 || form.optionType === 3 || form.optionType === 4"
        >
          <el-checkbox-group v-model="form.workshopTypes">
            <el-checkbox label="newWorkshop">新车房</el-checkbox>
            <el-checkbox label="oldWorkshop">老车房</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        
        <el-form-item label="备注" prop="remark">
          <el-input 
            v-model="form.remark" 
            type="textarea" 
            placeholder="请输入备注信息（可选）"
            :rows="2"
            maxlength="200"
            show-word-limit
            resize="none"
          />
        </el-form-item>
        
        <!-- 预览区域 -->
        <el-form-item label="预览" v-if="form.optionType && form.optionName">
          <div class="option-preview">
            <el-tag :type="getTagType(form.optionType)" size="small">
              {{ getOptionTypeLabel(form.optionType) }}
            </el-tag>
            <span class="preview-separator">-</span>
            <span class="preview-name">{{ form.optionName }}</span>
            <span class="preview-value" v-if="form.optionValue">(值: {{ form.optionValue }})</span>
            <span class="preview-material" v-if="form.optionType === 5 && form.relatedMaterial && form.relatedMaterial.length > 0">
              (关联材质: {{ getMaterialNames(form.relatedMaterial) }})
            </span>
            <span class="preview-design" v-if="form.optionType === 3 && form.relatedDesign && form.relatedDesign.length > 0">
              (关联设计: {{ getDesignNames(form.relatedDesign) }})
            </span>
          </div>
        </el-form-item>
      </el-form>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="cancel">
            取 消
          </el-button>
          <el-button 
            type="primary" 
            @click="submitForm" 
            :loading="submitLoading"
          >
            {{ form.id ? '保存修改' : '添加选项' }}
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 规格配置对话框 -->
    <el-dialog 
      :title="'规格配置 - 折射率·材质·设计·膜层'" 
      v-model="specConfigOpen" 
      width="1200px" 
      append-to-body
      :close-on-click-modal="false"
      :close-on-press-escape="false"
      @close="cancelSpecConfig"
    >
      <div class="spec-config-container">
        <!-- 顶部：折射率选择 -->
        <div class="spec-config-header">
          <el-form :inline="true">
            <el-form-item label="选择折射率">
              <el-select 
                v-model="specConfig.currentIndexId" 
                placeholder="请选择折射率"
                style="width: 300px"
                filterable
                clearable
                @change="handleIndexChange"
                :loading="specConfig.loading.index"
              >
                <el-option 
                  v-for="index in specConfig.indexOptions" 
                  :key="index.id" 
                  :label="index.optionName" 
                  :value="index.id"
                />
              </el-select>
            </el-form-item>
          </el-form>
        </div>

        <!-- 三列布局：材质 | 设计 | 膜层 -->
        <div class="spec-config-content">
          <!-- 左列：材质列表 -->
          <div class="spec-config-column">
            <div class="column-header">
              <h3>材质列表</h3>
              <el-text type="info" size="small">根据折射率显示关联材质</el-text>
            </div>
            <div class="column-body" v-loading="specConfig.loading.materials">
              <div 
                v-for="material in specConfig.materialOptions" 
                :key="material.id"
                class="config-item"
                :class="{ 'active': specConfig.currentMaterialId === material.id }"
                @click="handleMaterialClick(material.id)"
              >
                <div class="item-content">
                  <el-switch
                    v-model="material.enabled"
                    @change="handleMaterialToggle(material)"
                    @click.stop
                  />
                  <span class="item-name">{{ material.optionName }}</span>
                </div>
                <div class="item-status" v-if="material.status === '1'">
                  <el-tag type="danger" size="small">停用</el-tag>
                </div>
              </div>
              <el-empty v-if="specConfig.materialOptions.length === 0" description="暂无材质数据" />
            </div>
          </div>

          <!-- 中列：设计列表 -->
          <div class="spec-config-column">
            <div class="column-header">
              <h3>设计列表</h3>
              <el-text type="info" size="small" v-if="specConfig.currentMaterialId">
                根据材质显示关联设计
              </el-text>
              <el-text type="warning" size="small" v-else>
                请先选择材质
              </el-text>
            </div>
            <div class="column-body" v-loading="specConfig.loading.designs">
              <div 
                v-for="design in specConfig.designOptions" 
                :key="design.id"
                class="config-item"
                :class="{ 'active': specConfig.currentDesignId === design.id }"
                @click="handleDesignClick(design.id)"
              >
                <div class="item-content">
                  <el-switch
                    v-model="design.enabled"
                    @change="handleDesignToggle(design)"
                    @click.stop
                  />
                  <span class="item-name">{{ design.optionName }}</span>
                </div>
                <div class="item-status" v-if="design.status === '1'">
                  <el-tag type="danger" size="small">停用</el-tag>
                </div>
              </div>
              <el-empty v-if="specConfig.designOptions.length === 0 && !specConfig.currentMaterialId" description="请先选择材质" />
              <el-empty v-else-if="specConfig.designOptions.length === 0" description="暂无设计数据" />
            </div>
          </div>

          <!-- 右列：膜层配置 -->
          <div class="spec-config-column">
            <div class="column-header">
              <h3>膜层配置</h3>
              <el-text type="info" size="small" v-if="specConfig.currentDesignId">
                根据设计显示膜层配置
              </el-text>
              <el-text type="warning" size="small" v-else>
                请先选择设计
              </el-text>
            </div>
            <div class="column-body" v-loading="specConfig.loading.coatings">
              <div 
                v-for="coating in specConfig.coatingOptions" 
                :key="coating.id"
                class="config-item"
              >
                <div class="item-content">
                  <el-switch
                    v-model="coating.enabled"
                    @change="handleCoatingToggle(coating)"
                  />
                  <span class="item-name">{{ coating.optionName }}</span>
                </div>
                <div class="item-status" v-if="coating.status === '1'">
                  <el-tag type="danger" size="small">停用</el-tag>
                </div>
              </div>
              <el-empty v-if="specConfig.coatingOptions.length === 0 && !specConfig.currentDesignId" description="请先选择设计" />
              <el-empty v-else-if="specConfig.coatingOptions.length === 0" description="暂无膜层数据" />
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="cancelSpecConfig">
            取 消
          </el-button>
          <el-button 
            type="primary" 
            @click="saveSpecConfig" 
            :loading="specConfig.saving"
            :disabled="!specConfig.currentIndexId"
          >
            保存配置
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="LensOptions">
import { listLensOptions, getLensOption, delLensOption, delLensOptions, addLensOption, updateLensOption, changeLensOptionStatus, getNextOptionValue, getLensOptionsByType } from '@/api/DFGX/lensOptions'
import { parseTime } from '@/utils/ruoyi'

const { proxy } = getCurrentInstance()

const lensOptionsList = ref([])
const open = ref(false)
const loading = ref(true)
const showSearch = ref(true)
const ids = ref([])
const single = ref(true)
const multiple = ref(true)
const total = ref(0)
const title = ref('')
const optionTypeOptions = ref([])
const materialOptions = ref([])
const designOptions = ref([])
const submitLoading = ref(false)
const modelImageRef = ref(null)

// 规格配置相关
const specConfigOpen = ref(false)
const specConfig = reactive({
  currentIndexId: null,
  currentMaterialId: null,
  currentDesignId: null,
  indexOptions: [],
  materialOptions: [],
  designOptions: [],
  coatingOptions: [],
  loading: {
    index: false,
    materials: false,
    designs: false,
    coatings: false
  },
  saving: false
})

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    optionType: null,
    optionName: null,
    status: null
  },
  rules: {
    optionType: [
      { required: true, message: '请选择选项类型', trigger: 'change' }
    ],
    optionName: [
      { required: true, message: '请输入选项名称', trigger: 'blur' },
      { min: 1, max: 100, message: '选项名称长度在 1 到 100 个字符', trigger: 'blur' }
    ],
    optionValue: [
      { required: true, message: '选项值不能为空', trigger: 'blur' },
      { type: 'number', min: 1, max: 999, message: '选项值必须在 1 到 999 之间', trigger: 'blur' }
    ],
    status: [
      { required: true, message: '请选择状态', trigger: 'change' }
    ],
    relatedMaterial: [
      { 
        validator: (rule, value, callback) => {
          if (form.value.optionType === 5 && (!value || value.length === 0)) {
            callback(new Error('折射率选项必须选择关联材质'))
          } else {
            callback()
          }
        }, 
        trigger: 'change' 
      }
    ],
    relatedDesign: [
      { 
        validator: (rule, value, callback) => {
          if (form.value.optionType === 3 && (!value || value.length === 0)) {
            callback(new Error('材质选项必须选择关联设计'))
          } else {
            callback()
          }
        }, 
        trigger: 'change' 
      }
    ]
  }
})

const { queryParams, form, rules } = toRefs(data)

/** 查询镜片选项列表 */
function getList() {
  loading.value = true
  listLensOptions(queryParams.value).then(response => {
    lensOptionsList.value = response.data.result
    total.value = response.data.totalNum
    loading.value = false
  })
}

// 取消按钮
function cancel() {
  open.value = false
  reset()
}

// 表单重置
function reset() {
  form.value = {
    id: null,
    optionType: null,
    optionName: null,
    optionValue: null,
    status: '0',
    remark: '',
    relatedMaterial: [],
    relatedDesign: [],
    imageUrl: '',
    workshopTypes: []
  }
  proxy.resetForm('lensOptionRef')
}

/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1
  getList()
}

/** 重置按钮操作 */
function resetQuery() {
  proxy.resetForm('queryForm')
  handleQuery()
}

// 多选框选中数据
function handleSelectionChange(selection) {
  ids.value = selection.map(item => item.id)
  single.value = selection.length !== 1
  multiple.value = !selection.length
}

/** 新增按钮操作 */
function handleAdd() {
  reset()
  open.value = true
  title.value = '添加镜片选项'
  // 新增时不需要获取选项值，由服务端自动生成
}

/** 修改按钮操作 */
function handleUpdate(row) {
  reset()
  const id = row.id || ids.value[0] // 如果是批量选择，取第一个ID
  if (!id) {
    proxy.$modal.msgWarning('请选择要修改的数据')
    return
  }
  getLensOption(id).then(response => {
    console.log('获取到的数据:', response.data) // 调试信息
    
    // 处理新的接口结构
    const productConfig = response.data.productConfiguration || response.data
    const relatedMaterials = response.data.relatedMaterials || []
    const relatedDesigns = response.data.relatedDesigns || []
    
    // 确保数据格式正确，处理备注字段
    const remark = productConfig.remark || productConfig.Remarks || ''
    
    // 从relatedMaterials数组中提取material_id
    let relatedMaterial = []
    if (Array.isArray(relatedMaterials) && relatedMaterials.length > 0) {
      relatedMaterial = relatedMaterials.map(item => item.material_id).filter(id => id != null)
    }
    
    // 从relatedDesigns数组中提取design_id
    let relatedDesign = []
    if (Array.isArray(relatedDesigns) && relatedDesigns.length > 0) {
      relatedDesign = relatedDesigns.map(item => item.design_id || item.id).filter(id => id != null)
    }
    
    // 处理车房类型（后端返回的是整数：0=否，1=是）
    let workshopTypes = []
    const newWorkshopValue = productConfig.newWorkshop ?? productConfig.NewWorkshop
    const oldWorkshopValue = productConfig.oldWorkshop ?? productConfig.OldWorkshop
    if (newWorkshopValue === 1 || newWorkshopValue === true) {
      workshopTypes.push('newWorkshop')
    }
    if (oldWorkshopValue === 1 || oldWorkshopValue === true) {
      workshopTypes.push('oldWorkshop')
    }
    
    form.value = {
      id: productConfig.id || productConfig.ID,
      optionType: productConfig.optionType || productConfig.OptionType,
      optionName: productConfig.optionName || productConfig.OptionName,
      optionValue: productConfig.optionValue || productConfig.OptionValue,
      status: productConfig.status || productConfig.Status || '0',
      remark: remark === '无' ? '' : remark, // 如果备注是"无"，则显示为空，让用户可以重新输入
      relatedMaterial: relatedMaterial,
      relatedDesign: relatedDesign,
      imageUrl: productConfig.imageUrl || productConfig.ImageUrl || '',
      workshopTypes: workshopTypes
    }
    console.log('格式化后的表单数据:', form.value) // 调试信息
    open.value = true
    title.value = '修改镜片选项'
  }).catch(error => {
    console.error('获取数据失败:', error)
    proxy.$modal.msgError('获取数据失败')
  })
}

/** 提交按钮 */
function submitForm() {
  proxy.$refs['lensOptionRef'].validate(valid => {
    if (valid) {
      submitLoading.value = true

      // 处理备注字段：如果为空或null，设置为"无"
      const remark = form.value.remark && form.value.remark.trim() ? form.value.remark.trim() : '无'

      // 判断是否需要先上传模型图片：
      // 情况1：imageUrl 是本地 blob
      // 情况2：imageUrl 为空，但上传组件里存在未上传的本地文件（ready/raw/blob 预览）
      const hasPendingUploadFile = (() => {
        const comp = modelImageRef?.value
        const files = comp && comp.fileList ? comp.fileList : []
        if (!Array.isArray(files) || files.length === 0) return false
        return files.some(f => {
          const url = f && f.url ? String(f.url) : ''
          return (url.startsWith('blob:')) || (f.status === 'ready') || !!f.raw
        })
      })()

      const needUploadModelImage = form.value.optionType === 8 && (
        (typeof form.value.imageUrl === 'string' && form.value.imageUrl.startsWith('blob:')) ||
        (!form.value.imageUrl && hasPendingUploadFile)
      )

      const proceed = () => {
        // 处理车房类型（在材质、膜层或设计时）
        // 将布尔值转换为整数：true -> 1, false -> 0
        const newWorkshopBool = (form.value.optionType === 2 || form.value.optionType === 3 || form.value.optionType === 4) && 
                                form.value.workshopTypes && 
                                form.value.workshopTypes.includes('newWorkshop')
        const oldWorkshopBool = (form.value.optionType === 2 || form.value.optionType === 3 || form.value.optionType === 4) && 
                               form.value.workshopTypes && 
                               form.value.workshopTypes.includes('oldWorkshop')
        
        // 转换为整数：true -> 1, false -> 0, null -> null
        const newWorkshop = (form.value.optionType === 2 || form.value.optionType === 3 || form.value.optionType === 4) 
                          ? (newWorkshopBool ? 1 : 0) 
                          : null
        const oldWorkshop = (form.value.optionType === 2 || form.value.optionType === 3 || form.value.optionType === 4) 
                          ? (oldWorkshopBool ? 1 : 0) 
                          : null
        
        if (form.value.id != null) {
          // 编辑时构建符合后端接口的数据格式
          const requestData = {
            ID: form.value.id,
            OptionType: form.value.optionType,
            OptionName: form.value.optionName,
            OptionValue: form.value.optionValue,
            Status: form.value.status,
            Remarks: remark,
            RelatedMaterial: form.value.relatedMaterial && form.value.relatedMaterial.length > 0 ? form.value.relatedMaterial : null,
            RelatedDesign: form.value.relatedDesign && form.value.relatedDesign.length > 0 ? form.value.relatedDesign : null,
            ImageUrl: form.value.optionType === 8 ? (form.value.imageUrl || '') : null,
            NewWorkshop: newWorkshop,
            OldWorkshop: oldWorkshop
          }
          console.log('修改请求数据:', requestData) // 调试信息
          updateLensOption(requestData).then(response => {
            console.log('修改响应:', response) // 调试信息
            proxy.$modal.msgSuccess('修改成功')
            open.value = false
            getList()
          }).catch(error => {
            console.error('修改失败:', error) // 调试信息
            proxy.$modal.msgError('修改失败: ' + (error.message || '未知错误'))
          }).finally(() => {
            submitLoading.value = false
          })
        } else {
          // 新增时构建符合后端接口的数据格式
          const requestData = {
            OptionType: form.value.optionType,
            OptionName: form.value.optionName,
            OptionValue: form.value.optionValue,
            Status: form.value.status,
            Remarks: remark,
            RelatedMaterial: form.value.relatedMaterial && form.value.relatedMaterial.length > 0 ? form.value.relatedMaterial : null,
            RelatedDesign: form.value.relatedDesign && form.value.relatedDesign.length > 0 ? form.value.relatedDesign : null,
            ImageUrl: form.value.optionType === 8 ? (form.value.imageUrl || '') : null,
            NewWorkshop: newWorkshop,
            OldWorkshop: oldWorkshop
          }
          console.log('新增请求数据:', requestData) // 调试信息
          addLensOption(requestData).then(response => {
            console.log('新增响应:', response) // 调试信息
            proxy.$modal.msgSuccess('新增成功')
            open.value = false
            getList()
          }).catch(error => {
            console.error('新增失败:', error) // 调试信息
            proxy.$modal.msgError('新增失败: ' + (error.message || '未知错误'))
          }).finally(() => {
            submitLoading.value = false
          })
        }
      }

      if (needUploadModelImage && modelImageRef.value && typeof modelImageRef.value.submitUpload === 'function') {
        // 触发上传，等回调成功后继续提交
        pendingSubmitCallback = proceed
        modelImageRef.value.submitUpload()
      } else {
        proceed()
      }
    } else {
      console.log('表单验证失败')
      proxy.$modal.msgError('请检查表单填写是否正确')
    }
  })
}

// 上传成功后的回调：设置表单 imageUrl 并继续提交
let pendingSubmitCallback = null
function onModelImageUploaded(urls) {
  if (typeof urls === 'string') {
    form.value.imageUrl = urls.split(',')[0] || ''
  }
  if (pendingSubmitCallback) {
    const cb = pendingSubmitCallback
    pendingSubmitCallback = null
    cb()
  }
}

/** 删除按钮操作 */
function handleDelete(row) {
  const _ids = row.id || ids.value
  proxy.$modal.confirm('是否确认删除镜片选项编号为"' + _ids + '"的数据项？').then(function() {
    // 单个删除直接传递ID数组
    const idsArray = Array.isArray(_ids) ? _ids : [_ids]
    return delLensOption(idsArray)
  }).then(() => {
    getList()
    proxy.$modal.msgSuccess('删除成功')
  }).catch(() => {})
}

/** 批量删除按钮操作 */
function handleBatchDelete() {
  if (ids.value.length === 0) {
    proxy.$modal.msgWarning('请选择要删除的数据')
    return
  }
  console.log('批量删除的IDs:', ids.value) // 调试信息
  proxy.$modal.confirm('是否确认删除选中的"' + ids.value.length + '"条数据项？').then(function() {
    // ids.value 已经是数组，直接传递
    return delLensOptions(ids.value)
  }).then(() => {
    getList()
    proxy.$modal.msgSuccess('删除成功')
  }).catch((error) => {
    console.error('删除失败:', error) // 错误信息
    proxy.$modal.msgError('删除失败')
  })
}

/** 状态修改 */
function handleStatusChange(row) {
  // 保存原始状态
  const originalStatus = row.status
  const text = originalStatus === '0' ? '启用' : '停用'
  
  proxy.$modal.confirm('确认要"' + text + '""' + row.optionName + '"选项吗？').then(function() {
    // 传递新的状态值给后端
    return changeLensOptionStatus(row.id, originalStatus)
  }).then(() => {
    // 更新前端显示状态
    row.status = originalStatus
    proxy.$modal.msgSuccess('状态修改成功')
  }).catch(function(error) {
    // 检查是否是用户取消操作
    if (error === 'cancel') {
      // 用户取消操作，恢复原状态
      row.status = originalStatus
      return
    }
    // 其他错误，恢复原状态并显示错误信息
    row.status = originalStatus
    proxy.$modal.msgError('状态修改失败')
  })
}

/** 导出按钮操作 */
function handleExport() {
  proxy.download('DFGX/lensOptions/export', {
    ...queryParams.value
  }, `镜片选项_${new Date().getTime()}.xlsx`)
}

// 获取选项类型字典数据
function getOptionTypeOptions() {
  proxy.getDicts('dfgx_product_configuration').then(response => {
    optionTypeOptions.value = response.data
  })
}

// 获取材质选项数据
function getMaterialOptions() {
  // 使用新接口获取材质选项（optionType = 3）
  getLensOptionsByType(3).then(response => {
    const result = response.data || []
    materialOptions.value = result.map(item => ({
      label: item.optionName,
      value: item.id  // 使用id作为value，对应material_id
    }))
  }).catch(error => {
    console.error('获取材质选项失败:', error)
    materialOptions.value = []
  })
}

// 获取设计选项数据
function getDesignOptions() {
  // 使用新接口获取设计名称选项（optionType = 4）
  getLensOptionsByType(4).then(response => {
    const result = response.data || []
    designOptions.value = result.map(item => ({
      label: item.optionName,
      value: item.id  // 使用id作为value，对应design_id
    }))
  }).catch(error => {
    console.error('获取设计选项失败:', error)
    designOptions.value = []
  })
}

// 获取标签类型
function getTagType(optionType) {
  const typeMap = {
    1: 'primary',    // 镜片类型
    2: 'success',    // 膜层
    3: 'warning',    // 材质
    4: 'info'        // 设计名称
  }
  return typeMap[optionType] || 'default'
}

// 获取选项类型标签
function getOptionTypeLabel(optionType) {
  const dict = optionTypeOptions.value.find(item => parseInt(item.dictValue) === optionType)
  return dict ? dict.dictLabel : optionType
}

// 获取选项类型描述
function getOptionTypeDescription(optionType) {
  const descriptions = {
    1: '镜片类型选项',
    2: '膜层选项',
    3: '材质选项',
    4: '设计名称选项',
    5: '折射率选项'
  }
  return descriptions[optionType] || '未知类型'
}

// 获取材质名称
function getMaterialName(materialValue) {
  const material = materialOptions.value.find(item => item.value === materialValue)
  return material ? material.label : materialValue
}

// 获取多个材质名称
function getMaterialNames(materialValues) {
  if (!Array.isArray(materialValues) || materialValues.length === 0) {
    return ''
  }
  return materialValues.map(value => getMaterialName(value)).filter(name => name).join(', ')
}

// 获取设计名称
function getDesignName(designValue) {
  const design = designOptions.value.find(item => item.value === designValue)
  return design ? design.label : designValue
}

// 获取多个设计名称
function getDesignNames(designValues) {
  if (!Array.isArray(designValues) || designValues.length === 0) {
    return ''
  }
  return designValues.map(value => getDesignName(value)).filter(name => name).join(', ')
}

// 处理选项类型变化
function handleOptionTypeChange(value) {
  // 清空关联材质和关联设计字段
  form.value.relatedMaterial = []
  form.value.relatedDesign = []
  // 清空车房类型（如果切换到的类型不是材质、膜层或设计）
  if (value !== 2 && value !== 3 && value !== 4) {
    form.value.workshopTypes = []
  }
  
  if (value && !form.value.id) { // 只在新增时获取
    getNextOptionValue(value).then(response => {
      form.value.optionValue = response.data
    }).catch(() => {
      // 如果获取失败，设置为1
      form.value.optionValue = 1
    })
  }
}

// ========== 规格配置相关函数 ==========

/** 打开规格配置Dialog */
function handleSpecificationConfig() {
  specConfigOpen.value = true
  resetSpecConfig()
  loadSpecConfigDictionaries()
}

/** 重置规格配置数据 */
function resetSpecConfig() {
  specConfig.currentIndexId = null
  specConfig.currentMaterialId = null
  specConfig.currentDesignId = null
  specConfig.materialOptions = []
  specConfig.designOptions = []
  specConfig.coatingOptions = []
}

/** 加载规格配置所需的字典数据 */
function loadSpecConfigDictionaries() {
  // 加载折射率字典（optionType = 5）
  specConfig.loading.index = true
  getLensOptionsByType(5).then(response => {
    specConfig.indexOptions = response.data || []
    specConfig.loading.index = false
  }).catch(error => {
    console.error('加载折射率字典失败:', error)
    proxy.$modal.msgError('加载折射率字典失败')
    specConfig.loading.index = false
  })
}

/** 处理折射率选择变化 */
function handleIndexChange(indexId) {
  if (!indexId) {
    // 清空折射率时，重置所有数据
    resetSpecConfig()
    return
  }

  // 重置材质和设计的选择状态
  specConfig.currentMaterialId = null
  specConfig.currentDesignId = null
  specConfig.designOptions = []
  specConfig.coatingOptions = []

  // 调用 GetLensOptionByID 获取该折射率关联的材质
  specConfig.loading.materials = true
  getLensOption(indexId).then(response => {
    console.log('获取折射率关联材质 - 响应数据:', response.data) // 调试信息
    const productConfig = response.data.productConfiguration || response.data
    const relatedMaterials = response.data.relatedMaterials || []

    // 获取全量材质字典
    getLensOptionsByType(3).then(materialResponse => {
      const allMaterials = materialResponse.data || []
      
      // 从relatedMaterials数组中提取material_id（与handleUpdate中的逻辑保持一致）
      let relatedMaterialIds = []
      if (Array.isArray(relatedMaterials) && relatedMaterials.length > 0) {
        relatedMaterialIds = relatedMaterials.map(item => item.material_id || item.id).filter(id => id != null)
      }
      
      console.log('关联的材质ID列表:', relatedMaterialIds) // 调试信息
      console.log('全量材质列表:', allMaterials.map(m => ({ id: m.id, name: m.optionName }))) // 调试信息
      
      // 构建材质选项列表，标记哪些已关联
      specConfig.materialOptions = allMaterials.map(material => {
        const isEnabled = relatedMaterialIds.includes(material.id)
        console.log(`材质 ${material.optionName} (ID: ${material.id}) - 关联状态: ${isEnabled}`) // 调试信息
        return {
          id: material.id,
          optionName: material.optionName,
          status: material.status,
          enabled: isEnabled
        }
      })
      
      specConfig.loading.materials = false
    }).catch(error => {
      console.error('加载材质字典失败:', error)
      proxy.$modal.msgError('加载材质字典失败')
      specConfig.loading.materials = false
    })
  }).catch(error => {
    console.error('获取折射率关联材质失败:', error)
    proxy.$modal.msgError('获取折射率关联材质失败')
    specConfig.loading.materials = false
  })
}

/** 处理材质点击 */
function handleMaterialClick(materialId) {
  if (specConfig.currentMaterialId === materialId) {
    // 如果点击的是已选中的材质，取消选择
    specConfig.currentMaterialId = null
    specConfig.currentDesignId = null
    specConfig.designOptions = []
    specConfig.coatingOptions = []
    return
  }

  specConfig.currentMaterialId = materialId
  specConfig.currentDesignId = null
  specConfig.coatingOptions = []

  // 调用 GetLensOptionByID 获取该材质关联的设计
  specConfig.loading.designs = true
  getLensOption(materialId).then(response => {
    console.log('获取材质关联设计 - 响应数据:', response.data) // 调试信息
    const productConfig = response.data.productConfiguration || response.data
    const relatedDesigns = response.data.relatedDesigns || []

    // 获取全量设计字典
    getLensOptionsByType(4).then(designResponse => {
      const allDesigns = designResponse.data || []
      
      // 从relatedDesigns数组中提取design_id（与handleUpdate中的逻辑保持一致）
      let relatedDesignIds = []
      if (Array.isArray(relatedDesigns) && relatedDesigns.length > 0) {
        relatedDesignIds = relatedDesigns.map(item => item.design_id || item.id).filter(id => id != null)
      }
      
      console.log('关联的设计ID列表:', relatedDesignIds) // 调试信息
      console.log('全量设计列表:', allDesigns.map(d => ({ id: d.id, name: d.optionName }))) // 调试信息
      
      // 构建设计选项列表，标记哪些已关联
      specConfig.designOptions = allDesigns.map(design => {
        const isEnabled = relatedDesignIds.includes(design.id)
        console.log(`设计 ${design.optionName} (ID: ${design.id}) - 关联状态: ${isEnabled}`) // 调试信息
        return {
          id: design.id,
          optionName: design.optionName,
          status: design.status,
          enabled: isEnabled
        }
      })
      
      specConfig.loading.designs = false
    }).catch(error => {
      console.error('加载设计字典失败:', error)
      proxy.$modal.msgError('加载设计字典失败')
      specConfig.loading.designs = false
    })
  }).catch(error => {
    console.error('获取材质关联设计失败:', error)
    proxy.$modal.msgError('获取材质关联设计失败')
    specConfig.loading.designs = false
  })
}

/** 处理设计点击 */
function handleDesignClick(designId) {
  if (specConfig.currentDesignId === designId) {
    // 如果点击的是已选中的设计，取消选择
    specConfig.currentDesignId = null
    specConfig.coatingOptions = []
    return
  }

  specConfig.currentDesignId = designId

  // 加载膜层字典（optionType = 2）
  specConfig.loading.coatings = true
  getLensOptionsByType(2).then(response => {
    const allCoatings = response.data || []
    
    // 构建膜层选项列表（这里暂时都设为未启用，后续可以根据设计ID获取已关联的膜层）
    specConfig.coatingOptions = allCoatings.map(coating => ({
      id: coating.id,
      optionName: coating.optionName,
      status: coating.status,
      enabled: false // 默认未启用，后续可以根据实际接口返回的数据设置
    }))
    
    specConfig.loading.coatings = false
  }).catch(error => {
    console.error('加载膜层字典失败:', error)
    proxy.$modal.msgError('加载膜层字典失败')
    specConfig.loading.coatings = false
  })
}

/** 处理材质开关切换 */
function handleMaterialToggle(material) {
  // 这里只是更新本地状态，实际保存需要在保存按钮中处理
  console.log('材质开关切换:', material.id, material.enabled)
}

/** 处理设计开关切换 */
function handleDesignToggle(design) {
  // 这里只是更新本地状态，实际保存需要在保存按钮中处理
  console.log('设计开关切换:', design.id, design.enabled)
}

/** 处理膜层开关切换 */
function handleCoatingToggle(coating) {
  // 这里只是更新本地状态，实际保存需要在保存按钮中处理
  console.log('膜层开关切换:', coating.id, coating.enabled)
}

/** 保存规格配置 */
function saveSpecConfig() {
  if (!specConfig.currentIndexId) {
    proxy.$modal.msgWarning('请先选择折射率')
    return
  }

  // 收集需要保存的数据
  const enabledMaterials = specConfig.materialOptions
    .filter(m => m.enabled)
    .map(m => m.id)
  
  const enabledDesigns = specConfig.designOptions
    .filter(d => d.enabled)
    .map(d => d.id)
  
  const enabledCoatings = specConfig.coatingOptions
    .filter(c => c.enabled)
    .map(c => c.id)

  console.log('保存配置数据:', {
    indexId: specConfig.currentIndexId,
    materials: enabledMaterials,
    designs: enabledDesigns,
    coatings: enabledCoatings
  })

  // TODO: 调用后端保存接口
  // 这里需要根据实际的后端接口来实现
  proxy.$modal.msgInfo('保存功能待实现，请根据实际后端接口完成')
  
  // 示例代码（需要根据实际接口调整）：
  // specConfig.saving = true
  // saveSpecificationConfig({
  //   indexId: specConfig.currentIndexId,
  //   materials: enabledMaterials,
  //   designs: enabledDesigns,
  //   coatings: enabledCoatings
  // }).then(() => {
  //   proxy.$modal.msgSuccess('保存成功')
  //   cancelSpecConfig()
  // }).catch(error => {
  //   console.error('保存失败:', error)
  //   proxy.$modal.msgError('保存失败')
  // }).finally(() => {
  //   specConfig.saving = false
  // })
}

/** 取消规格配置 */
function cancelSpecConfig() {
  specConfigOpen.value = false
  resetSpecConfig()
}

onMounted(() => {
  getList()
  getOptionTypeOptions()
  getMaterialOptions()
  getDesignOptions()
})
</script>

<style scoped>
.app-container {
  padding: 20px;
}

/* 对话框表单样式 */
.lens-option-form {
  padding: 10px 0;
}

.lens-option-form .el-form-item {
  margin-bottom: 16px;
}

.lens-option-form .el-form-item__label {
  font-weight: 500;
  color: #303133;
  font-size: 14px;
}

/* 预览区域样式 */
.option-preview {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  background: #f5f7fa;
  border-radius: 4px;
  border: 1px solid #e4e7ed;
  font-size: 13px;
}

.preview-separator {
  margin: 0 6px;
  color: #909399;
  font-weight: 500;
}

.preview-name {
  font-weight: 500;
  color: #303133;
}

.preview-value {
  margin-left: 6px;
  font-size: 11px;
  color: #909399;
}

.preview-material {
  margin-left: 6px;
  font-size: 11px;
  color: #67c23a;
  font-weight: 500;
}

.preview-design {
  margin-left: 6px;
  font-size: 11px;
  color: #409eff;
  font-weight: 500;
}

/* 表单提示样式 */
.form-tip {
  margin-top: 4px;
  line-height: 1.4;
}

/* 对话框底部按钮样式 */
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  padding: 16px 0 0 0;
  border-top: 1px solid #e4e7ed;
}

.dialog-footer .el-button {
  min-width: 80px;
}

/* 单选框样式优化 */
.el-radio {
  margin-right: 16px;
}

/* 紧凑布局 */
.el-row {
  margin-bottom: 0;
}

.el-col {
  padding: 0 6px;
}

/* ========== 规格配置Dialog样式 ========== */
.spec-config-container {
  padding: 10px 0;
}

.spec-config-header {
  padding: 16px;
  background: #f5f7fa;
  border-radius: 4px;
  margin-bottom: 20px;
}

.spec-config-header .el-form-item {
  margin-bottom: 0;
}

.spec-config-content {
  display: flex;
  gap: 16px;
  min-height: 500px;
}

.spec-config-column {
  flex: 1;
  display: flex;
  flex-direction: column;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  overflow: hidden;
  background: #fff;
}

.column-header {
  padding: 12px 16px;
  background: #f5f7fa;
  border-bottom: 1px solid #e4e7ed;
}

.column-header h3 {
  margin: 0 0 4px 0;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}

.column-body {
  flex: 1;
  padding: 8px;
  overflow-y: auto;
  min-height: 400px;
}

.config-item {
  padding: 10px 12px;
  margin-bottom: 8px;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  background: #fff;
}

.config-item:hover {
  border-color: #409eff;
  background: #ecf5ff;
}

.config-item.active {
  border-color: #409eff;
  background: #ecf5ff;
}

.item-content {
  display: flex;
  align-items: center;
  gap: 12px;
}

.item-name {
  flex: 1;
  font-size: 14px;
  color: #303133;
  user-select: none;
}

.item-status {
  margin-top: 4px;
}

.config-item .el-switch {
  flex-shrink: 0;
}
</style>