<template>
  <div class="test-i18n">
    <h1>国际化测试页面</h1>
    <div class="test-content">
      <h2>静态国际化测试</h2>
      <p>首页: {{ $t('menu.home') }}</p>
      <p>控制台: {{ $t('menu.dashboard') }}</p>
      <p>系统管理: {{ $t('menu.system') }}</p>
      
      <h2>动态国际化测试</h2>
      <p>当前语言: {{ currentLang }}</p>
      <p>可用语言: {{ availableLocales }}</p>
      
      <h2>环境变量测试</h2>
      <p>应用标题: {{ appTitle }}</p>
      <p>API地址: {{ apiHost }}</p>
      
      <h2>菜单数据测试</h2>
      <div v-if="menuData.length > 0">
        <div v-for="menu in menuData" :key="menu.path">
          <p>{{ menu.meta?.titleKey }}: {{ $t(menu.meta?.titleKey) || menu.meta?.title || '未定义' }}</p>
        </div>
      </div>
      <p v-else>暂无菜单数据</p>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import usePermissionStore from '@/store/modules/permission'

const { t, locale, availableLocales } = useI18n()
const permissionStore = usePermissionStore()

const currentLang = computed(() => locale.value)
const appTitle = computed(() => import.meta.env.VITE_APP_TITLE || '未设置')
const apiHost = computed(() => import.meta.env.VITE_APP_API_HOST || '未设置')
const menuData = computed(() => permissionStore.sidebarRouters || [])
</script>

<style scoped>
.test-i18n {
  padding: 20px;
}

.test-content {
  margin-top: 20px;
}

.test-content h2 {
  color: #409EFF;
  margin-top: 20px;
}

.test-content p {
  margin: 10px 0;
  padding: 5px;
  background: #f5f5f5;
  border-radius: 4px;
}
</style>
