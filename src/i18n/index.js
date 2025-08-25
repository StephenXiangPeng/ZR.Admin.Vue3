import { createI18n } from 'vue-i18n'
// import useAppStore from '@/store/modules/app'
import { listLangByLocale } from '@/api/system/commonlang.js'
import defaultSettings from '@/settings'
import cache from '@/plugins/cache'
const language = computed(() => {
  // return useAppStore().lang
  return cache.local.get('lang') || defaultSettings.defaultLang
})

import zhCn from './lang/zh-cn.json'
import en from './lang/en.json'
import zhTw from './lang/zh-tw.json'

import pageLoginCn from './pages/login/zh-cn.json'
import pageLoginEn from './pages/login/en.json'
import pageLoginTw from './pages/login/zh-tw.json'

// 菜单页面
import pagemenuCn from './pages/menu/zh-cn.json'
import pagemenuEn from './pages/menu/en.json'
import pagemenuTw from './pages/menu/zh-tw.json'

const i18n = createI18n({
  // 全局注入 $t 函数
  globalInjection: true,
  fallbackLocale: 'zh-cn',
  locale: language.value, //默认选择的语言
  legacy: false, // 使用 Composition API 模式，则需要将其设置为false
  messages: {
    'zh-cn': {
      ...zhCn,
      ...pageLoginCn,
      ...pagemenuCn
    },
    'zh-tw': {
      ...zhTw,
      ...pageLoginTw,
      ...pagemenuTw
    },
    en: {
      ...en,
      ...pageLoginEn,
      ...pagemenuEn
    }
    //... 在这里添加其他语言支持
  }
})

const loadLocale = () => {
  // 添加错误处理，确保即使API调用失败也能正常工作
  try {
    listLangByLocale(language.value).then((res) => {
      const { code, data } = res
      if (code == 200) {
        console.log('动态加载的国际化数据:', data)
        i18n.global.mergeLocaleMessage(language.value, data)
      }
    }).catch((error) => {
      console.error('加载国际化数据失败:', error)
      // 如果动态加载失败，使用静态文件作为备用
      console.log('使用静态国际化文件作为备用')
    })
  } catch (error) {
    console.error('国际化初始化失败:', error)
  }
}

// 确保在DOM加载完成后执行
if (typeof window !== 'undefined') {
  loadLocale()
}

export default i18n
