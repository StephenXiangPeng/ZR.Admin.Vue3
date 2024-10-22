<template>
  <el-container class="email-client">
    <el-header height="40px">
      <div class="header-left">
        <el-dropdown>
          <el-button type="primary" @click="createNewEmail">
            新邮件 <el-icon class="el-icon--right"><arrow-down /></el-icon>
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <!-- 下拉菜单项 -->
            </el-dropdown-menu>
          </template>
        </el-dropdown>
        <el-button-group>
          <el-button>删除</el-button>
          <el-button>存档</el-button>
          <el-button>报告</el-button>
          <el-button>移至</el-button>
          <el-button>全部标签</el-button>
        </el-button-group>
        <el-button>已读/未读</el-button>
        <el-button>标记/取消标记</el-button>
        <el-button>发现组</el-button>
        <el-button>刷新</el-button>
        <el-button>...</el-button>
      </div>
    </el-header>
    <el-container>
      <el-aside width="200px">
        <el-menu default-active="1" class="sidebar-menu">
          <el-menu-item-group title="收藏夹">
            <el-menu-item index="1" class="custom-menu-item">
              <el-icon>
                <message />
              </el-icon>
              <span>收件箱</span>
              <span class="item-count">4364</span>
            </el-menu-item>
            <el-menu-item index="2" class="custom-menu-item">
              <el-icon>
                <warning />
              </el-icon>
              <span>垃圾邮件</span>
              <span class="item-count">17</span>
            </el-menu-item>
            <el-menu-item index="3" class="custom-menu-item">
              <el-icon>
                <edit />
              </el-icon>
              <span>草稿</span>
              <span class="item-count">5</span>
            </el-menu-item>
            <!-- 其他菜单项 -->
          </el-menu-item-group>
        </el-menu>
      </el-aside>
      <el-main class="email-main">
        <el-tabs v-model="activeTab" type="card" closable @tab-remove="removeTab">
          <el-tab-pane v-for="item in tabs" :key="item.name" :label="item.title" :name="item.name">
            <component :is="item.content" :email="item.email"></component>
          </el-tab-pane>
        </el-tabs>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, shallowRef, defineComponent } from 'vue'
import { ArrowDown, Message, Warning, Edit } from '@element-plus/icons-vue'

const InboxView = defineComponent({
  template: `
    <div class="inbox-view">
      <el-tabs type="border-card" class="custom-tabs">
        <el-tab-pane label="重点">
          <el-table :data="emails" style="width: 100%" :show-header="false" @row-click="selectEmail">
            <el-table-column width="30">
              <template #default>
                <el-checkbox></el-checkbox>
              </template>
            </el-table-column>
            <el-table-column width="30">
              <template #default="scope">
                <el-avatar :size="20" :class="['custom-avatar', scope.row.avatarClass]">
                  {{ scope.row.icon }}
                </el-avatar>
              </template>
            </el-table-column>
            <el-table-column>
              <template #default="scope">
                <div class="email-content">
                  <span class="sender">{{ scope.row.sender }}</span>
                  <span class="subject">{{ scope.row.subject }}</span>
                  <span class="preview">{{ scope.row.preview }}</span>
                </div>
              </template>
            </el-table-column>
            <el-table-column width="40" align="right">
              <template #default="scope">
                <span class="time">{{ scope.row.time }}</span>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="其他"></el-tab-pane>
      </el-tabs>
    </div>
  `,
  setup(props, { emit }) {
    const emails = ref([
      { id: 1, icon: 'G', sender: 'Google', subject: '安全提醒', preview: '"Microsoft apps & services"已...', time: '0:47', avatarClass: 'google-avatar' },
      { id: 2, icon: 'G', sender: 'Google', subject: 'Peng, 请查看您的 Google...', preview: 'Peng, 您好！ 欢迎使用新的个人资料...', time: '0:45', avatarClass: 'google-avatar' },
      { id: 3, icon: 'G', sender: 'Google', subject: '安全提醒', preview: '"Windows"已获得对您的 Google...', time: '0:44', avatarClass: 'google-avatar' },
      { id: 4, icon: 'G', sender: 'Google', subject: '安全提醒', preview: '在 Windows 设备上有新的登录...', time: '0:44', avatarClass: 'google-avatar' },
      { id: 5, icon: 'SR', sender: 'Substack Reads', subject: 'Immunity as metap...', preview: 'Jessica DeFino shares her favo...', time: '昨天 21:43', avatarClass: 'substack-avatar' },
      { id: 6, icon: 'AN', sender: 'Apple News', subject: "Why America's you...", preview: 'Good morning from Apple News...', time: '昨天 20:45', avatarClass: 'apple-avatar' },
      { id: 7, icon: 'B', sender: 'Busuu', subject: '可能让你的语法学习...', preview: '今天的课程专题内容：将 teams@busuu.com 添加到您的通讯录中', time: '昨天 16:32', avatarClass: 'busuu-avatar' },
      { id: 8, icon: 'F', sender: 'Freelancer.com', subject: 'peng, these Python...', preview: 'Hi peng, Here are the latest projects that match your skills:', time: '昨天 11:37', avatarClass: 'freelancer-avatar' },
      { id: 9, icon: 'B', sender: 'Busuu', subject: 'My Busuu weekly st...', preview: '看看你这周取得的进展！将 team@busuu.com 添加到您的通讯录中', time: '昨天 7:29', avatarClass: 'busuu-avatar' },
    ])

    const selectEmail = (row) => {
      emit('select-email', row)
    }

    return { emails, selectEmail }
  }
})

const ComposeView = defineComponent({
  template: `
    <div class="compose-view">
      <el-form :model="emailForm" label-width="80px">
        <el-form-item label="发件人">
          <el-input v-model="emailForm.from" disabled></el-input>
        </el-form-item>
        <el-form-item label="收件人">
          <el-input v-model="emailForm.to" placeholder="输入收件人邮箱"></el-input>
        </el-form-item>
        <el-form-item label="主题">
          <el-input v-model="emailForm.subject" placeholder="输入邮件主题"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary">发送</el-button>
        </el-form-item>
      </el-form>
      <div class="email-content">
        <el-input type="textarea" v-model="emailForm.content" rows="15"></el-input>
      </div>
    </div>
  `,
  setup() {
    const emailForm = ref({
      from: 'stephen.xiangpeng@gmail.com',
      to: '',
      subject: '',
      content: ''
    })
    return { emailForm }
  }
})

const EmailDetailView = defineComponent({
  props: ['email'],
  template: `
    <div class="email-detail-view">
      <div class="email-header">
        <h2>{{ email.subject }}</h2>
        <div class="email-info">
          <el-avatar :size="40" :class="['custom-avatar', email.avatarClass]">{{ email.icon }}</el-avatar>
          <div class="sender-info">
            <div class="sender-name">{{ email.sender }}</div>
            <div class="sender-email">{{ email.sender.toLowerCase() }}@example.com</div>
          </div>
          <div class="email-actions">
            <el-button>回复</el-button>
            <el-button>转发</el-button>
          </div>
        </div>
      </div>
      <div class="email-body">
        {{ email.preview }}
      </div>
    </div>
  `
})

const tabs = ref([
  { name: 'inbox', title: '收件箱', content: InboxView }
])
const activeTab = ref('inbox')

const createNewEmail = () => {
  const newTab = {
    name: `compose-${Date.now()}`,
    title: '新邮件',
    content: ComposeView
  }
  tabs.value.push(newTab)
  activeTab.value = newTab.name
}

const removeTab = (targetName) => {
  const targetIndex = tabs.value.findIndex(tab => tab.name === targetName)
  if (targetIndex !== -1) {
    tabs.value.splice(targetIndex, 1)
  }
  if (activeTab.value === targetName) {
    activeTab.value = tabs.value[targetIndex - 1]?.name || tabs.value[0]?.name
  }
}

const showEmailDetail = (email) => {
  const newTab = {
    name: `email-${email.id}`,
    title: email.subject,
    content: EmailDetailView,
    email: email
  }
  tabs.value.push(newTab)
  activeTab.value = newTab.name
}
</script>

<style scoped>
.email-client {
  height: 100vh;
  font-family: Arial, sans-serif;
}

.el-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 16px;
  border-bottom: 1px solid #e0e0e0;
  background-color: #f2f6fc;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.el-aside {
  border-right: 1px solid #e0e0e0;
  background-color: #f2f6fc;
}

.sidebar-menu {
  border-right: none;
}

.custom-menu-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.item-count {
  font-size: 12px;
  color: #909399;
}

.email-main {
  padding: 0;
}

.custom-tabs :deep(.el-tabs__header) {
  margin: 0;
}

.custom-tabs :deep(.el-tabs__nav-wrap) {
  padding: 0 16px;
}

.custom-avatar {
  font-size: 12px;
}

.google-avatar {
  background-color: #fbbc05;
}

.substack-avatar {
  background-color: #ff6719;
}

.apple-avatar {
  background-color: #a2aaad;
}

.busuu-avatar {
  background-color: #5cacee;
}

.reddit-avatar {
  background-color: #ff4500;
}

.freelancer-avatar {
  background-color: #29b2fe;
}

.email-content {
  display: flex;
  flex-direction: column;
}

.sender {
  font-weight: bold;
}

.subject,
.preview {
  font-size: 13px;
  color: #606266;
}

.time {
  font-size: 12px;
  color: #909399;
}

.el-table :deep(td) {
  border-bottom: none;
}

.el-table :deep(.el-table__row) {
  cursor: pointer;
}

.el-table :deep(.el-table__row:hover) {
  background-color: #f5f7fa;
}

.email-detail-view {
  padding: 20px;
}

.email-header {
  margin-bottom: 20px;
}

.email-info {
  display: flex;
  align-items: center;
  margin-top: 10px;
}

.sender-info {
  margin-left: 10px;
}

.sender-name {
  font-weight: bold;
}

.sender-email {
  font-size: 12px;
  color: #909399;
}

.email-actions {
  margin-left: auto;
}

.email-body {
  margin-top: 20px;
  white-space: pre-wrap;
}

.compose-view {
  padding: 20px;
}

.compose-view .el-form-item {
  margin-bottom: 15px;
}

.compose-view .el-input {
  width: 100%;
}

.compose-view .el-button {
  margin-top: 10px;
}

.compose-view .email-content {
  margin-top: 20px;
}

.compose-view .el-textarea__inner {
  min-height: 300px;
}
</style>