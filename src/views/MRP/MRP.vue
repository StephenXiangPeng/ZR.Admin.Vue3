<template>
  <div>
    <el-button type="primary" :loading="loading" @click="runMrp">运行 MRP</el-button>
    <el-table :data="rows" style="width:100%;margin-top:12px">
      <el-table-column prop="materialCode" label="物料"/>
      <el-table-column prop="qty" label="数量"/>
      <el-table-column prop="actionType" label="动作"/>
      <el-table-column prop="planReleaseDate" label="下达日"/>
      <el-table-column prop="requirementDate" label="需求日"/>
      <el-table-column prop="parentDemandNo" label="来源单号"/>
      <el-table-column prop="parentCode" label="父件"/>
      <el-table-column prop="levelNo" label="层级" width="70"/>
    </el-table>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { runMrp as runMrpApi, getMrpResult } from '@/api/MRP/mrp'

const loading = ref(false)
const rows = ref([])

/**
 * 运行MRP
 */
async function runMrp() {
  try {
    loading.value = true
    const response = await runMrpApi({})
    const data = response.data || response
    
    if (data.code === 200) {
      ElMessage.success('MRP已执行')
      await load()
    } else {
      ElMessage.error(data.msg || 'MRP执行失败')
    }
  } catch (error) {
    console.error('运行MRP失败:', error)
    ElMessage.error('运行MRP失败，请重试')
  } finally {
    loading.value = false
  }
}

/**
 * 加载MRP结果数据
 */
async function load() {
  try {
    const response = await getMrpResult({})
    if (response.code === 200) {
      rows.value = response.data
    } else {
      ElMessage.error(response.msg || '获取MRP结果失败')
    }
  } catch (error) {
    console.error('获取MRP结果失败:', error)
    ElMessage.error('获取MRP结果失败，请重试')
  }
}

// 页面加载时获取数据
onMounted(() => {
  load()
})
</script>