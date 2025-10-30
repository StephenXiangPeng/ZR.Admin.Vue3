<template>
  <div class="component-upload-image">
    <el-upload
      multiple
      v-bind="$attrs"
      :action="uploadImgUrl"
      :list-type="listType"
      :on-success="handleUploadSuccess"
      :before-upload="handleBeforeUpload"
      :limit="limit"
      :on-error="handleUploadError"
      :on-exceed="handleExceed"
      name="file"
      :data="uploadData"
      :on-remove="handleRemove"
      :show-file-list="true"
      :headers="headers"
      v-model:file-list="fileList"
      :on-preview="handlePictureCardPreview"
      :style="cssVars"
      :auto-upload="autoUpload"
      ref="uploadRef"
      :class="{ hide: fileList.length >= limit }">
      <slot name="icon">
        <el-icon class="avatar-uploader-icon"><uploadFilled /></el-icon>
      </slot>

      <template v-slot:tip>
        <div class="el-upload__tip" v-if="showTip">
          <slot name="tip">
            <template v-if="fileSize">
              大小不超过 <b class="text-danger">{{ fileSize }}MB</b>
            </template>
            <template v-if="fileType">
              格式为 <b class="text-danger">{{ fileType.join('/') }}</b>
            </template>
          </slot>
        </div>
      </template>
    </el-upload>

    <el-dialog v-model="dialogVisible" append-to-body>
      <el-form label-width="100px">
        <el-form-item label="预览">
          <el-image style="display: block; max-width: 50%" :src="dialogImageUrl">
            <template #error>
              <div class="image-slot">加载失败</div>
            </template>
          </el-image>
        </el-form-item>

        <el-form-item label="访问路径">
          <el-link type="warning" :href="dialogImageUrl" target="_blank">{{ dialogImageUrl }}</el-link>
          <el-button type="danger" text icon="document-copy" plain class="ml10" v-clipboard:success="copySuccess" v-clipboard:copy="dialogImageUrl"
            >复制</el-button
          >
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>

<script setup>
import { getToken } from '@/utils/auth'

const props = defineProps({
  modelValue: [String, Object, Array],
  // 图片数量限制
  limit: {
    type: Number,
    default: 1
  },
  // 大小限制(MB)
  fileSize: {
    type: Number,
    default: 5
  },
  // 文件类型, 例如['png', 'jpg', 'jpeg']
  fileType: {
    type: Array,
    default: () => ['png', 'jpg', 'jpeg', 'webp']
  },
  // 是否显示提示
  isShowTip: {
    type: Boolean,
    default: true
  },
  // 上传携带的参数
  data: {
    type: Object
  },
  style: {
    type: Object,
    default: {
      width: '120px'
    }
  },
  listType: {
    type: String,
    default: 'picture-card'
  },
  // 是否自动上传
  autoUpload: {
    type: Boolean,
    default: true
  }
})

const { proxy } = getCurrentInstance()
const emit = defineEmits(['success','update:modelValue'])
const number = ref(0)
const uploadList = ref([])
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const baseUrl = import.meta.env.VITE_APP_BASE_API
const apiHost = import.meta.env.VITE_APP_API_HOST
const isDev = import.meta.env.DEV
const apiPrefix = computed(() => {
  // 与业务接口同前缀：开发环境固定 /dev-api；生产环境用 VITE_APP_BASE_API
  if (isDev) return '/dev-api'
  return typeof baseUrl === 'string' ? baseUrl : ''
})
const uploadBase = computed(() => {
  // 本地开发强制使用 /dev-api 以命中 vite 代理
  if (isDev) return '/dev-api'
  return baseUrl
})
const uploadImgUrl = computed(() => {
  const prefix = String(uploadBase.value || '')
  const path = String(import.meta.env.VITE_APP_UPLOAD_URL || '/common/upload')
  if (!prefix) return path
  if (prefix.endsWith('/') && path.startsWith('/')) return prefix + path.slice(1)
  if (!prefix.endsWith('/') && !path.startsWith('/')) return prefix + '/' + path
  return prefix + path
}) // 上传的图片服务器地址
const headers = ref({ Authorization: 'Bearer ' + getToken() })
const fileList = ref([])
const uploadRef = ref()
const showTip = computed(() => props.isShowTip && (props.fileType || props.fileSize))
const uploadData = computed(() => props.data)
const cssVars = computed(() => {
  return {
    '--el-upload-picture-card-size': props.style.width,
    '--el-upload-list-picture-card-size': props.style.width
  }
})
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      // 首先将值转为数组
      const list = Array.isArray(val) ? val : props.modelValue.split(',')
      // 然后将数组转为对象数组
      fileList.value = list.map((item) => {
        if (typeof item === 'string') {
          item = { name: item, url: item }
        }
        return item
      })
    } else {
      fileList.value = []
      return []
    }
  },
  { deep: true, immediate: true }
)

// 删除图片
function handleRemove(file, files) {
  // 优先使用回调传入的最新文件列表，避免依赖可能滞后的内部 fileList
  const current = Array.isArray(files) ? files : fileList.value
  const value = listToString(current)
  emit('update:modelValue', value)
}

// 上传成功回调
function handleUploadSuccess(res) {
  if (res.code != 200) {
    proxy.$modal.msgError(`上传失败，原因:${res.msg}!`)
    proxy.$modal.closeLoading()
    fileList.value = fileList.value.slice(0, fileList.value.length - 1)
    return
  }
  // 兼容后端返回的相对路径，自动补齐与接口一致的前缀（开发：/dev-api；生产：VITE_APP_BASE_API）
  const rawUrl = (res.data && (res.data.url || res.data.fileUrl || res.data.path)) || ''
  const name = (res.data && (res.data.fileName || res.data.name)) || (rawUrl ? rawUrl.split('/').pop() : '')
  let finalUrl = rawUrl
  if (isDev) {
    // 开发环境：全部映射到 /dev-api 与接口一致
    if (/^https?:\/\//i.test(rawUrl)) {
      try {
        const u = new URL(rawUrl)
        finalUrl = apiPrefix.value.replace(/\/$/, '') + u.pathname
      } catch (e) {
        finalUrl = apiPrefix.value.replace(/\/$/, '') + (rawUrl.startsWith('/') ? '' : '/') + rawUrl
      }
    } else {
      finalUrl = apiPrefix.value.replace(/\/$/, '') + (rawUrl.startsWith('/') ? '' : '/') + rawUrl
    }
  } else {
    // 生产环境：
    // 1) 相对路径 -> 拼接 BASE_API 或 API_HOST
    // 2) 绝对路径：若为 localhost/127.0.0.1 则重写为当前站点主机 + :8887
    if (finalUrl) {
      if (!/^https?:\/\//i.test(finalUrl)) {
        // 相对路径
        if (typeof apiHost === 'string' && /^https?:\/\//i.test(apiHost)) {
          finalUrl = finalUrl.startsWith('/') ? apiHost + finalUrl : apiHost + '/' + finalUrl
        } else if (typeof baseUrl === 'string' && /^https?:\/\//i.test(baseUrl)) {
          finalUrl = finalUrl.startsWith('/') ? baseUrl + finalUrl : baseUrl + '/' + finalUrl
        } else if (typeof baseUrl === 'string' && baseUrl.startsWith('/')) {
          finalUrl = baseUrl.replace(/\/$/, '') + (finalUrl.startsWith('/') ? '' : '/') + finalUrl
        }
      } else {
        // 绝对路径：当后端返回 localhost/127.0.0.1 时，替换为当前站点 + 配置的 API 前缀（生产为 /api），不加端口
        try {
          const u = new URL(finalUrl)
          const isLocal = u.hostname === 'localhost' || u.hostname === '127.0.0.1'
          if (isLocal) {
            const fileHost = import.meta.env.VITE_APP_FILE_HOST
            const base = import.meta.env.VITE_APP_BASE_API
            // 优先使用显式文件主机；否则用 BASE_API（若为相对路径，以当前 origin 拼接）
            let targetPrefix = ''
            if (typeof fileHost === 'string' && /^https?:\/\//i.test(fileHost)) {
              targetPrefix = fileHost.replace(/\/$/, '')
            } else if (typeof base === 'string' && /^https?:\/\//i.test(base)) {
              targetPrefix = base.replace(/\/$/, '')
            } else {
              // 生产默认 BASE_API=/api
              const apiPrefix = typeof base === 'string' ? base : '/api'
              const origin = window.location.origin.replace(/\/$/, '')
              targetPrefix = origin + (apiPrefix.startsWith('/') ? apiPrefix : '/' + apiPrefix)
              targetPrefix = targetPrefix.replace(/\/$/, '')
            }
            finalUrl = targetPrefix + u.pathname
          }
        } catch (e) {
          // 忽略 URL 解析错误，沿用原始 absolute 链接
        }
      }
    }
  }
  uploadList.value.push({ name, url: finalUrl })
  if (uploadList.value.length === number.value) {
    fileList.value = fileList.value.filter((f) => f.url !== undefined).concat(uploadList.value)
    uploadList.value = []
    number.value = 0
    emit('update:modelValue', listToString(fileList.value))
    emit('success', listToString(fileList.value))
  }
  proxy.$modal.closeLoading()
}

// 上传前loading加载
function handleBeforeUpload(file) {
  let isImg = false
  if (props.fileType.length) {
    let fileExtension = ''
    if (file.name.lastIndexOf('.') > -1) {
      fileExtension = file.name.slice(file.name.lastIndexOf('.') + 1)
    }
    isImg = props.fileType.some((type) => {
      if (file.type.indexOf(type) > -1) return true
      if (fileExtension && fileExtension.indexOf(type) > -1) return true
      return false
    })
  } else {
    isImg = file.type.indexOf('image') > -1
  }
  if (!isImg) {
    proxy.$modal.msgError(`文件格式不正确, 请上传${props.fileType.join('/')}图片格式文件!`)
    return false
  }
  if (props.fileSize) {
    const isLt = file.size / 1024 / 1024 < props.fileSize
    if (!isLt) {
      proxy.$modal.msgError(`上传头像图片大小不能超过 ${props.fileSize} MB!`)
      return false
    }
  }
  proxy.$modal.loading('正在上传图片，请稍候...')
  number.value++
}

// 文件个数超出
function handleExceed() {
  proxy.$modal.msgError(`上传文件数量不能超过 ${props.limit} 个!`)
}

// 上传失败
function handleUploadError() {
  proxy.$modal.msgError('上传图片失败')
  proxy.$modal.closeLoading()
}

// 预览
function handlePictureCardPreview(file) {
  dialogImageUrl.value = file.url
  dialogVisible.value = true
}

// 对象转成指定字符串分隔
function listToString(list, separator) {
  let strs = ''
  separator = separator || ','
  for (let i in list) {
    if (undefined !== list[i].url && list[i].url.indexOf('blob:') !== 0) {
      strs += list[i].url + separator
    }
  }
  return strs != '' ? strs.substr(0, strs.length - 1) : ''
}
function copySuccess() {
  proxy.$modal.msgSuccess('复制成功')
}

// 手动提交上传（当 :auto-upload="false" 时可用）
function submitUpload() {
  if (uploadRef.value && typeof uploadRef.value.submit === 'function') {
    uploadRef.value.submit()
  }
}

defineExpose({
  submitUpload,
  fileList
})
</script>
<style>
.el-upload--picture-card {
  --el-upload-picture-card-size: unset;
}
.el-upload-list--picture-card {
  --el-upload-list-picture-card-size: unset;
}
.el-upload__tip {
  font-size: 10px;
}
</style>
