<template>
  <div></div>
</template>
<!-- ！！！这里有坑 template里面要有div标签，不然页面会出现各种问题-->
<script setup>
import { useRoute, useRouter } from 'vue-router'
import useTagsViewStore from '@/store/modules/tagsView'

const route = useRoute()
const router = useRouter()
const { params, query } = route
const { path } = params

// 直接重定向到目标路径
if (path) {
  router.replace({
    path: '/' + path,
    query
  });
}
</script>
