<template>
  <div ref="chatRootRef" class="ai-chat" :class="{ 'is-dragging': isDragging }" :style="containerStyle">
    <transition name="ai-chat-panel">
      <div v-show="visible" class="ai-chat__panel">
        <div class="ai-chat__header">
          <div>
            <div class="ai-chat__title">AI 助手</div>
            <div class="ai-chat__subtitle">进出口办公系统助手</div>
          </div>
          <el-button text circle class="ai-chat__close" @click="visible = false">×</el-button>
        </div>

        <div ref="messageListRef" class="ai-chat__messages">
          <div v-for="message in messages" :key="message.id" :class="['ai-chat__message', `is-${message.role}`]">
            <div class="ai-chat__bubble">{{ message.content }}</div>
          </div>
          <div v-if="loading" class="ai-chat__message is-assistant">
            <div class="ai-chat__bubble ai-chat__loading">AI 正在思考...</div>
          </div>
        </div>

        <div class="ai-chat__footer">
          <el-input v-model="question" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }" maxlength="1000"
            show-word-limit resize="none" placeholder="请输入问题，按 Ctrl + Enter 发送"
            @keydown.ctrl.enter.prevent="sendMessage" />
          <el-button type="primary" :loading="loading" :disabled="!question.trim()" @click="sendMessage">发送</el-button>
        </div>
      </div>
    </transition>

    <button class="ai-chat__float-button" type="button" @mousedown.prevent="onFloatMouseDown">
      <span>AI</span>
    </button>
  </div>
</template>

<script setup>
import { computed, nextTick, onMounted, onUnmounted, ref } from 'vue'
import { ElMessage } from 'element-plus'
// import { chatWithAi } from '@/api/ai'

const POSITION_STORAGE_KEY = 'huirong-ai-chat-float-position'
const FLOAT_BUTTON_SIZE = 56
const FLOAT_MARGIN = 8
const DRAG_THRESHOLD = 4

const visible = ref(false)
const loading = ref(false)
const question = ref('')
const messageListRef = ref(null)
const chatRootRef = ref(null)
const isDragging = ref(false)
const floatPos = ref(null)
const dragState = {
  startX: 0,
  startY: 0,
  originX: 0,
  originY: 0,
  moved: false
}

const messages = ref([
  {
    id: Date.now(),
    role: 'assistant',
    content: 'AI 助手待接入，敬请期待。'
  }
])

const containerStyle = computed(() => {
  if (!floatPos.value) {
    return {}
  }
  return {
    left: `${floatPos.value.x}px`,
    top: `${floatPos.value.y}px`,
    right: 'auto',
    bottom: 'auto'
  }
})

function clampPosition(x, y) {
  const maxX = Math.max(FLOAT_MARGIN, window.innerWidth - FLOAT_BUTTON_SIZE - FLOAT_MARGIN)
  const maxY = Math.max(FLOAT_MARGIN, window.innerHeight - FLOAT_BUTTON_SIZE - FLOAT_MARGIN)
  return {
    x: Math.min(Math.max(FLOAT_MARGIN, x), maxX),
    y: Math.min(Math.max(FLOAT_MARGIN, y), maxY)
  }
}

function loadSavedPosition() {
  try {
    const raw = localStorage.getItem(POSITION_STORAGE_KEY)
    if (!raw) {
      return
    }
    const parsed = JSON.parse(raw)
    if (typeof parsed?.x === 'number' && typeof parsed?.y === 'number') {
      floatPos.value = clampPosition(parsed.x, parsed.y)
    }
  } catch (error) {
    console.warn('读取 AI 悬浮位置失败:', error)
  }
}

function savePosition() {
  if (!floatPos.value) {
    return
  }
  localStorage.setItem(POSITION_STORAGE_KEY, JSON.stringify(floatPos.value))
}

function ensurePixelPosition() {
  if (floatPos.value || !chatRootRef.value) {
    return
  }
  const rect = chatRootRef.value.getBoundingClientRect()
  floatPos.value = clampPosition(rect.left, rect.top)
}

function onFloatMouseMove(event) {
  if (!isDragging.value) {
    return
  }
  const dx = event.clientX - dragState.startX
  const dy = event.clientY - dragState.startY
  if (Math.abs(dx) > DRAG_THRESHOLD || Math.abs(dy) > DRAG_THRESHOLD) {
    dragState.moved = true
  }
  floatPos.value = clampPosition(dragState.originX + dx, dragState.originY + dy)
}

function stopDragging() {
  if (!isDragging.value) {
    return
  }
  isDragging.value = false
  document.removeEventListener('mousemove', onFloatMouseMove)
  document.removeEventListener('mouseup', stopDragging)
  if (floatPos.value) {
    savePosition()
  }
  if (!dragState.moved) {
    showAiPendingMessage()
  }
}

function onFloatMouseDown(event) {
  if (event.button !== 0) {
    return
  }
  ensurePixelPosition()
  isDragging.value = true
  dragState.moved = false
  dragState.startX = event.clientX
  dragState.startY = event.clientY
  dragState.originX = floatPos.value.x
  dragState.originY = floatPos.value.y
  document.addEventListener('mousemove', onFloatMouseMove)
  document.addEventListener('mouseup', stopDragging)
}

function handleWindowResize() {
  if (!floatPos.value) {
    return
  }
  floatPos.value = clampPosition(floatPos.value.x, floatPos.value.y)
  savePosition()
}

function showAiPendingMessage() {
  ElMessage.info('AI 功能即将接入')
}

async function scrollToBottom() {
  await nextTick()
  if (messageListRef.value) {
    messageListRef.value.scrollTop = messageListRef.value.scrollHeight
  }
}

async function sendMessage() {
  const content = question.value.trim()
  if (!content || loading.value) {
    return
  }

  ElMessage.info('AI 功能即将接入')
  return

  messages.value.push({
    id: Date.now(),
    role: 'user',
    content
  })
  question.value = ''
  loading.value = true
  await scrollToBottom()

  try {
    // const response = await chatWithAi(content)
  } catch (error) {
    ElMessage.error('AI 对话请求失败，请稍后重试')
  } finally {
    loading.value = false
    await scrollToBottom()
  }
}

onMounted(() => {
  loadSavedPosition()
  window.addEventListener('resize', handleWindowResize)
})

onUnmounted(() => {
  document.removeEventListener('mousemove', onFloatMouseMove)
  document.removeEventListener('mouseup', stopDragging)
  window.removeEventListener('resize', handleWindowResize)
})
</script>

<style scoped lang="scss">
.ai-chat {
  position: fixed;
  right: 24px;
  bottom: 24px;
  z-index: 3000;
}

.ai-chat.is-dragging {
  user-select: none;
}

.ai-chat__float-button {
  width: 56px;
  height: 56px;
  border: none;
  border-radius: 50%;
  color: #fff;
  font-size: 18px;
  font-weight: 700;
  cursor: grab;
  touch-action: none;
  box-shadow: 0 10px 24px rgba(64, 158, 255, 0.36);
  background: linear-gradient(135deg, #409eff, #7c4dff);
}

.ai-chat.is-dragging .ai-chat__float-button {
  cursor: grabbing;
}

.ai-chat__panel {
  position: absolute;
  right: 0;
  bottom: 72px;
  width: 380px;
  max-width: calc(100vw - 32px);
  height: 520px;
  max-height: calc(100vh - 120px);
  overflow: hidden;
  border-radius: 14px;
  background: #fff;
  box-shadow: 0 16px 42px rgba(31, 45, 61, 0.2);
  display: flex;
  flex-direction: column;
}

.ai-chat__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 18px;
  color: #fff;
  background: linear-gradient(135deg, #409eff, #7c4dff);
}

.ai-chat__title {
  font-size: 17px;
  font-weight: 700;
}

.ai-chat__subtitle {
  margin-top: 4px;
  font-size: 12px;
  opacity: 0.86;
}

.ai-chat__close {
  color: #fff;
  font-size: 24px;
}

.ai-chat__messages {
  flex: 1;
  padding: 16px;
  overflow-y: auto;
  background: #f5f7fb;
}

.ai-chat__message {
  display: flex;
  margin-bottom: 12px;
}

.ai-chat__message.is-user {
  justify-content: flex-end;
}

.ai-chat__bubble {
  max-width: 82%;
  padding: 10px 12px;
  border-radius: 12px;
  line-height: 1.6;
  font-size: 14px;
  white-space: pre-wrap;
  word-break: break-word;
  color: #303133;
  background: #fff;
  box-shadow: 0 2px 8px rgba(31, 45, 61, 0.06);
}

.ai-chat__message.is-user .ai-chat__bubble {
  color: #fff;
  background: #409eff;
}

.ai-chat__loading {
  color: #909399;
}

.ai-chat__footer {
  padding: 12px;
  border-top: 1px solid #ebeef5;
  background: #fff;
  display: flex;
  align-items: flex-end;
  gap: 10px;
}

.ai-chat-panel-enter-active,
.ai-chat-panel-leave-active {
  transition: all 0.2s ease;
}

.ai-chat-panel-enter-from,
.ai-chat-panel-leave-to {
  opacity: 0;
  transform: translateY(8px);
}

@media (max-width: 480px) {
  .ai-chat {
    right: 16px;
    bottom: 16px;
  }

  .ai-chat__panel {
    right: -8px;
    width: calc(100vw - 32px);
  }
}
</style>
