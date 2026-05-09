<template>
  <el-config-provider :locale="locale" :size="size">
    <router-view />
    <AiChatFloat />
  </el-config-provider>
</template>
<script setup>
import useUserStore from './store/modules/user'
import useAppStore from './store/modules/app'
import AiChatFloat from '@/components/AiChatFloat.vue'
import { ElConfigProvider, ElMessage } from 'element-plus'
import zh from 'element-plus/dist/locale/zh-cn.mjs' // 中文语言
import en from 'element-plus/dist/locale/en.mjs' // 英文语言
import tw from 'element-plus/dist/locale/zh-tw.mjs' //繁体
import defaultSettings from '@/settings'
import { eventBus } from '@/utils/eventBus'
import { useRouter, useRoute } from 'vue-router'
const { proxy } = getCurrentInstance()

const token = computed(() => {
  return useUserStore().userId
})

const lang = computed(() => {
  return useAppStore().lang
})
const locale = ref(zh)
const size = ref(defaultSettings.defaultSize)

size.value = useAppStore().size
watch(
  token,
  (val) => {
    if (val) {
      proxy.signalr.start().then(async (res) => {
        if (res) {
          try {
            await proxy.signalr.safeInvoke('logOut')
          } catch (error) {
            console.error('调用 logOut 失败:', error)
            ElMessage({
              message: '操作失败，请稍后重试',
              type: 'error',
              duration: 3000
            })
          }
        }
      })
    }
  },
  {
    immediate: true,
    deep: true
  }
)
watch(
  lang,
  (val) => {
    if (val == 'zh-cn') {
      locale.value = zh
    } else if (val == 'en') {
      locale.value = en
    } else if (val == 'zh-tw') {
      locale.value = tw
    } else {
      locale.value = zh
    }
  },
  {
    immediate: true
  }
)

const route = useRoute();
const router = useRouter();

// 监听通知导航事件
eventBus.on('notification-navigate', ({ path, data, action, contactId }) => {
  if (action === 'openSaleContactApproval' && route.path === '/index') {
    eventBus.emit('open-sale-contact-approval', { contactId });
  } else {
    router.push(path);
  }
});


</script>
