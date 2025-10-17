<template>
  <div class="order-upload-file">
    <el-upload
      :action="uploadFileUrl"
      :before-upload="handleBeforeUpload"
      v-model:file-list="fileList"
      :limit="limit"
      :on-error="handleUploadError"
      :on-exceed="handleExceed"
      :on-success="handleUploadSuccess"
      :show-file-list="false"
      :data="uploadData"
      :drag="drag"
      :headers="headers"
      :auto-upload="autoUpload"
      :accept="acceptAttr"
      class="order-upload-file-uploader"
      ref="fileUpload"
    >
      <!-- 拖拽上传 -->
      <template v-if="drag">
        <el-icon class="el-icon--upload">
          <UploadFilled />
        </el-icon>
        <div class="el-upload__text">
          Drop file here or <em>click to upload</em>
        </div>
      </template>

      <!-- 按钮上传 -->
      <el-button type="primary" size="small" v-else>
        Select JPG
      </el-button>

      <!-- 上传提示 -->
      <template #tip>
        <div class="el-upload__tip" v-if="showTip">
          <slot name="tip">
            <template v-if="fileSize">
              Size limit: <b class="text-danger">{{ fileSize }}MB</b>
            </template>
            <template v-if="fileType">
              Format: <b class="text-danger">{{ allowedExt.join('/') }}</b>
            </template>
          </slot>
        </div>
      </template>
    </el-upload>

    <!-- 文件列表 -->
    <transition-group
      class="upload-file-list el-upload-list el-upload-list--text"
      name="el-fade-in-linear"
      tag="ul"
    >
      <li
        v-for="(file, index) in fileList"
        :key="file.uid"
        class="el-upload-list__item ele-upload-list__item-content"
      >
        <!-- 图片预览（JPG） -->
        <div v-if="isImage(file)" class="file-preview">
          <el-image
            :src="file.url"
            :preview-src-list="[file.url]"
            :hide-on-click-modal="true"
            fit="cover"
            class="preview-image"
          >
            <template #error>
              <el-icon><Document /></el-icon>
            </template>
          </el-image>
          <div class="file-info">
            <el-link :href="`${file.url}`" :underline="false" target="_blank">
              {{ file.name }}
            </el-link>
          </div>
        </div>

        <!-- 兜底：普通文件（理论上不会出现，因为只允 JPG） -->
        <div v-else class="file-item">
          <el-link :href="`${file.url}`" :underline="false" target="_blank">
            <el-icon class="doc-icon"><Document /></el-icon>
            {{ file.name }}
          </el-link>
        </div>

        <div class="ele-upload-list__item-content-action" v-if="!disabled">
          <el-link :underline="false" @click="handleDelete(index)" type="danger">
            Delete
          </el-link>
        </div>
      </li>
    </transition-group>
  </div>
</template>

<script setup>
import { ref, computed, watch, getCurrentInstance } from 'vue'
import { ElMessage } from 'element-plus'
import { UploadFilled, Document } from '@element-plus/icons-vue'
import { getToken } from '@/utils/auth'

const props = defineProps({
  modelValue: [String, Object, Array],
  // 数量限制
  limit: { type: Number, default: 5 },
  // 大小限制(MB)
  fileSize: { type: Number, default: 5 },
  // 文件类型（业务上只允 jpg；组件内部会自动扩展为 ['jpg','jpeg'] + image/jpeg）
  fileType: {
    type: Array,
    default: () => ['jpg']
  },
  // 是否显示提示
  isShowTip: { type: Boolean, default: true },
  // 是否拖拽
  drag: { type: Boolean, default: false },
  // 自动上传
  autoUpload: { type: Boolean, default: true },
  // 上传携带参数
  data: { type: Object },
  // 禁用组件（仅查看文件）
  disabled: { type: Boolean, default: false }
})

const { proxy } = getCurrentInstance()
const emit = defineEmits(['success', 'update:modelValue'])

const number = ref(0)
const uploadList = ref([])
const baseUrl = import.meta.env.VITE_APP_BASE_API
const uploadFileUrl = ref(baseUrl + import.meta.env.VITE_APP_UPLOAD_URL) // 上传服务器地址
const headers = ref({ Authorization: 'Bearer ' + getToken() })
const fileList = ref([])

const showTip = computed(() => props.isShowTip && (props.fileType || props.fileSize))
const uploadData = computed(() => props.data)

// ===== 类型限制（只允 JPG）=====
const allowedExt = computed(() => {
  // 把传入的 fileType 归一化，并自动扩展 jpg -> ['jpg','jpeg']
  const base = (props.fileType || []).map(t => String(t || '').toLowerCase())
  const set = new Set()
  base.forEach(t => {
    if (t === 'jpg' || t === 'jpeg') {
      set.add('jpg'); set.add('jpeg')
    } else {
      set.add(t)
    }
  })
  // 若业务只传 ['jpg']，最终为 ['jpg','jpeg']
  return Array.from(set)
})

const allowedMime = computed(() => {
  const m = []
  if (allowedExt.value.includes('jpg') || allowedExt.value.includes('jpeg')) {
    m.push('image/jpeg')
  }
  return m
})

// 提供给 <el-upload> 的 accept，便于选择框过滤
const acceptAttr = computed(() => {
  const dots = allowedExt.value.map(e => `.${e}`)
  return dots.join(',') // ".jpg,.jpeg"
})

// 图片判断（用于列表展示）
const isImage = (file) => {
  const ext = file.name?.split('.').pop()?.toLowerCase() || ''
  return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'].includes(ext)
}

// 同步外部 v-model 的文件列表
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      let temp = 1
      // 转为数组
      const list = Array.isArray(val) ? val : String(props.modelValue).split(',')
      // 将数组转为 {name,url,uid} 结构
      fileList.value = list.map((item) => {
        if (typeof item === 'string') {
          const fileName = item.slice(item.lastIndexOf('/') + 1)
          item = { name: fileName, url: item }
        }
        item.uid = item.uid || new Date().getTime() + temp++
        return item
      })
    } else {
      fileList.value = []
      return []
    }
  },
  { deep: true, immediate: true }
)

// 上传前校验：扩展名 + MIME 双重验证 + 大小限制
function handleBeforeUpload(file) {
  // 扩展名
  const ext = file.name?.split('.').pop()?.toLowerCase() || ''
  // MIME
  const mime = (file.type || '').toLowerCase()

  const okByExt = allowedExt.value.includes(ext)
  const okByMime = allowedMime.value.length === 0 ? true : allowedMime.value.includes(mime)

  if (!(okByExt || okByMime)) {
    proxy?.$modal?.msgError
      ? proxy.$modal.msgError(`File format incorrect, please upload ${allowedExt.value.join('/').toUpperCase()} files!`)
      : ElMessage.error(`File format incorrect, please upload ${allowedExt.value.join('/').toUpperCase()} files!`)
    return false
  }

  // 大小限制
  if (props.fileSize) {
    const isLt = file.size / 1024 / 1024 < props.fileSize
    if (!isLt) {
      proxy?.$modal?.msgError
        ? proxy.$modal.msgError(`Upload file size cannot exceed ${props.fileSize} MB!`)
        : ElMessage.error(`Upload file size cannot exceed ${props.fileSize} MB!`)
      return false
    }
  }

  proxy?.$modal?.loading && proxy.$modal.loading('Uploading file, please wait...')
  number.value++
  return true
}

// 文件数量超出
function handleExceed() {
  proxy?.$modal?.msgError
    ? proxy.$modal.msgError(`Upload file count cannot exceed ${props.limit} files!`)
    : ElMessage.error(`Upload file count cannot exceed ${props.limit} files!`)
}

// 上传失败
function handleUploadError() {
  proxy?.$modal?.msgError ? proxy.$modal.msgError('Upload failed') : ElMessage.error('Upload failed')
  proxy?.$modal?.closeLoading && proxy.$modal.closeLoading()
}

// 上传成功
function handleUploadSuccess(response, uploadFile) {
  // 和你原先保持一致：只当 code == 200 视为成功
  if (response.code != 200) {
    fileList.value = []
    proxy?.$modal?.msgError
      ? proxy.$modal.msgError(`Upload failed, reason: ${response.msg}!`)
      : ElMessage.error(`Upload failed, reason: ${response.msg}!`)
    proxy?.$modal?.closeLoading && proxy.$modal.closeLoading()
    return
  }

  const { fileName, url } = response.data || {}
  const tempFile = { name: fileName, url: url, uid: uploadFile.uid }
  uploadList.value.push(tempFile)

  // 简化：每次上传成功都合并并触发 success
  fileList.value = fileList.value.filter(f => f.url !== undefined).concat(uploadList.value)
  uploadList.value = []

  const fileUrls = listToString(fileList.value)
  emit('success', fileUrls)
  proxy?.$modal?.closeLoading && proxy.$modal.closeLoading()
}

// 删除文件
function handleDelete(index) {
  fileList.value.splice(index, 1)
  emit('update:modelValue', listToString(fileList.value))
}

// 对象数组转逗号分隔字符串
function listToString(list, separator = ',') {
  let strs = ''
  list.forEach((el) => {
    if (el && el.url) strs += el.url + separator
  })
  return strs !== '' ? strs.slice(0, -1) : ''
}

// 手动提交上传（当 :autoUpload="false" 时可用）
function submitUpload() {
  proxy.$refs.fileUpload.submit()
}

defineExpose({
  submitUpload,
  fileList
})
</script>

<style scoped lang="scss">
.order-upload-file-uploader {
  margin-bottom: 5px;
}

.upload-file-list .el-upload-list__item {
  border: 1px solid #e4e7ed;
  line-height: 2;
  margin-bottom: 10px;
  position: relative;
}

.upload-file-list .ele-upload-list__item-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: inherit;
}

.ele-upload-list__item-content-action .el-link {
  margin-right: 10px;
}

.doc-icon {
  margin: 0 10px;
}

.file-preview {
  display: flex;
  align-items: center;
  gap: 10px;
}

.preview-image {
  width: 40px;
  height: 40px;
  border-radius: 4px;
  border: 1px solid #e4e7ed;
}

.file-info {
  flex: 1;
}

.file-item {
  display: flex;
  align-items: center;
}
</style>