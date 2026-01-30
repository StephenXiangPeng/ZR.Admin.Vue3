<template>
  <el-aside :data-theme="sideTheme" class="sidebar" :class="{ 'has-logo': showLogo }">
    <logo v-if="showLogo" :collapse="isCollapse" />

    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu :key="menuKey" :default-active="activeMenu" :default-openeds="defaultOpeneds" :collapse="isCollapse"
        :unique-opened="false" :active-text-color="theme" :collapse-transition="false" background-color="transparent"
        mode="vertical">
        <sidebar-item v-for="(route, index) in sidebarRouters" :key="route.path + index" :item="route"
          :base-path="route.path" :isCollapse="isCollapse" />
      </el-menu>
    </el-scrollbar>
  </el-aside>
</template>

<script setup>
import Logo from './Logo'
import SidebarItem from './SidebarItem'
import useAppStore from '@/store/modules/app'
import useSettingsStore from '@/store/modules/settings'
import usePermissionStore from '@/store/modules/permission'
import { isExternal } from '@/utils/validate'
import { getNormalPath } from '@/utils/ruoyi'

const route = useRoute()
const appStore = useAppStore()
const settingsStore = useSettingsStore()
const permissionStore = usePermissionStore()

const sidebarRouters = computed(() => permissionStore.sidebarRouters)
const showLogo = computed(() => settingsStore.sidebarLogo)
const sideTheme = computed(() => settingsStore.sideTheme)
const theme = computed(() => settingsStore.theme)
const isCollapse = computed(() => !appStore.sidebar.opened)
const device = computed(() => appStore.device)
const defaultOpeneds = computed(() => {
  const openeds = []
  sidebarRouters.value.forEach((route) => {
    openeds.push(...collectOpeneds([route], route.path))
  })
  return openeds
})
const menuKey = ref(0)
const activeMenu = computed(() => {
  const { meta, path } = route
  // if set path, the sidebar will highlight the path you set
  if (meta.activeMenu) {
    return meta.activeMenu
  }
  return path
})
watch(route, (val) => {
  if (device.value === 'mobile') {
    appStore.closeSideBar()
  }
})

watch(
  sidebarRouters,
  () => {
    menuKey.value += 1
  },
  { deep: true }
)

function resolvePath(basePath, routePath) {
  if (isExternal(routePath)) {
    return routePath
  }
  if (isExternal(basePath)) {
    return basePath
  }
  return getNormalPath(basePath + '/' + routePath)
}

function collectOpeneds(routes, basePath = '') {
  const openeds = []
  routes.forEach((item) => {
    if (item.hidden) {
      return
    }
    const currentPath = resolvePath(basePath, item.path)
    if (item.children && item.children.length > 0) {
      openeds.push(currentPath)
      openeds.push(...collectOpeneds(item.children, currentPath))
    }
  })
  return openeds
}
</script>
