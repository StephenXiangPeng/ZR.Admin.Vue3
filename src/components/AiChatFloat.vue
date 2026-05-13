<template>
  <div class="ai-chat">
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

    <button class="ai-chat__float-button" type="button" @click="showAiPendingMessage">
      <span>AI</span>
    </button>
  </div>
</template>

<script setup>
import { nextTick, ref } from 'vue'
import { ElMessage } from 'element-plus'
// import { chatWithAi } from '@/api/ai'

const visible = ref(false)
const loading = ref(false)
const question = ref('')
const messageListRef = ref(null)
const messages = ref([
  {
    id: Date.now(),
    role: 'assistant',
    content: 'AI 助手待接入，敬请期待。'
  }
])

function showAiPendingMessage() {
  ElMessage.info('AI 功能即将接入')
}

// function getAnswerContent(response) {
//   return response?.message?.content || response?.data?.message?.content || response?.content || response?.answer || 'AI 暂未返回内容。'
// }

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

  // 原 AI 接口调用逻辑暂时保留，后续接入时恢复。
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
    // messages.value.push({
    //   id: Date.now() + 1,
    //   role: 'assistant',
    //   content: getAnswerContent(response)
    // })
  } catch (error) {
    ElMessage.error('AI 对话请求失败，请稍后重试')
  } finally {
    loading.value = false
    await scrollToBottom()
  }
}
</script>

<style scoped lang="scss">
.ai-chat {
  position: fixed;
  right: 24px;
  bottom: 24px;
  z-index: 3000;
}

.ai-chat__float-button {
  width: 56px;
  height: 56px;
  border: none;
  border-radius: 50%;
  color: #fff;
  font-size: 18px;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 10px 24px rgba(64, 158, 255, 0.36);
  background: linear-gradient(135deg, #409eff, #7c4dff);
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
