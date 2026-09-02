<template>
  <div class="two-factor-settings">
    <div class="status-row">
      <span>二次验证：</span>
      <el-tag :type="enabled ? 'success' : 'info'">{{ enabled ? '已开启' : '未开启' }}</el-tag>
    </div>
    <el-button v-if="!enabled" type="primary" :loading="initLoading" @click="startBind">
      开启二次验证
    </el-button>

    <el-dialog v-model="dialogVisible" title="开启二次验证" width="460px" append-to-body
      :before-close="handleClose" @closed="clearBindState">
      <p class="bind-tip">
        请使用 Google Authenticator、Microsoft Authenticator<br />
        或其他兼容 TOTP 的验证器 App 扫描二维码。
      </p>
      <div ref="qrContainer" class="qr-container"></div>
      <p class="bind-tip">请输入验证器中的6位动态验证码完成绑定。</p>
      <el-input ref="codeInputRef" v-model="code" maxlength="6" inputmode="numeric"
        autocomplete="one-time-code" placeholder="请输入6位动态验证码" @input="handleCodeInput"
        @keyup.enter="confirmBind" />
      <template #footer>
        <el-button :disabled="confirmLoading" @click="closeDialog">取消</el-button>
        <el-button type="primary" :loading="confirmLoading" :disabled="confirmLoading" @click="confirmBind">
          确认绑定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import QRCode from 'qrcodejs2-fixes'
import { initTwoFactorBind, confirmTwoFactorBind } from '@/api/system/user'

const props = defineProps({
  user: {
    type: Object,
    default: () => ({})
  }
})
const emit = defineEmits(['refresh'])
const { proxy } = getCurrentInstance()

const enabled = computed(() => {
  const value = props.user.twoFactorEnabled
  return value === true || value === 1 || value === '1'
})
const dialogVisible = ref(false)
const initLoading = ref(false)
const confirmLoading = ref(false)
const bindToken = ref('')
const otpauthUri = ref('')
const expiresAt = ref(0)
const code = ref('')
const qrContainer = ref()
const codeInputRef = ref()

function renderQrCode() {
  nextTick(() => {
    if (!qrContainer.value || !otpauthUri.value) return
    qrContainer.value.innerHTML = ''
    new QRCode(qrContainer.value, {
      text: otpauthUri.value,
      width: 180,
      height: 180
    })
    codeInputRef.value?.focus()
  })
}

function startBind() {
  initLoading.value = true
  initTwoFactorBind()
    .then((response) => {
      bindToken.value = response.data.bindToken
      otpauthUri.value = response.data.otpauthUri
      expiresAt.value = Date.now() + Number(response.data.expiresIn || 0) * 1000
      dialogVisible.value = true
      renderQrCode()
    })
    .finally(() => {
      initLoading.value = false
    })
}

function handleCodeInput(value) {
  code.value = String(value || '').replace(/\D/g, '').slice(0, 6)
}

function focusCode() {
  nextTick(() => codeInputRef.value?.focus())
}

function confirmBind() {
  if (confirmLoading.value) return
  if (!/^\d{6}$/.test(code.value)) {
    proxy.$modal.msgError('请输入6位动态验证码。')
    focusCode()
    return
  }

  confirmLoading.value = true
  confirmTwoFactorBind({ bindToken: bindToken.value, code: code.value })
    .then(() => {
      proxy.$modal.msgSuccess('二次验证已开启')
      dialogVisible.value = false
      emit('refresh')
    })
    .catch((error) => {
      const message = error?.msg || String(error || '')
      if (message.includes('动态验证码错误')) {
        proxy.$modal.msgError('验证码不正确，请重新输入。')
        code.value = ''
      } else {
        proxy.$modal.msgError(message || '绑定失败，请重试。')
      }
      focusCode()
    })
    .finally(() => {
      confirmLoading.value = false
    })
}

function clearBindState() {
  bindToken.value = ''
  otpauthUri.value = ''
  expiresAt.value = 0
  code.value = ''
  if (qrContainer.value) qrContainer.value.innerHTML = ''
}

function closeDialog() {
  dialogVisible.value = false
}

function handleClose(done) {
  if (!confirmLoading.value) done()
}

onBeforeUnmount(clearBindState)
</script>

<style scoped>
.status-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 20px;
}

.bind-tip {
  color: #606266;
  line-height: 1.7;
  text-align: center;
}

.qr-container {
  display: flex;
  width: 180px;
  height: 180px;
  margin: 18px auto;
  align-items: center;
  justify-content: center;
}
</style>
