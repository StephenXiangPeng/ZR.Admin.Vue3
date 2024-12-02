<template>
  <div class="dashboard-container">
    <el-row :gutter="20">
      <!-- 系统概述卡片 -->
      <el-col :span="24">
        <el-card class="system-info mb20">
          <template #header>
            <div class="card-header">
              <span>惠扬特包综合管理系统(一期)</span>
            </div>
          </template>
          <div class="system-description">
            通过模块化设计，解决文件管理、供应商管理、人事管理及库存管理的核心需求，提升企业管理效率，确保流程规范化、数据可追溯性及信息的高效流转。
          </div>
        </el-card>
      </el-col>

      <!-- 四大模块卡片 -->
      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="module-card" shadow="hover" @click="navigateTo('/file')">
          <div class="module-header">
            <i class="el-icon-document"></i>
            <span>文件管理</span>
          </div>
          <div class="module-content">
            <ul>
              <li>文件上传与分类存储</li>
              <li>版本管理与历史记录</li>
              <li>文件审核提醒</li>
              <li>文件归档与导出</li>
            </ul>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="module-card" shadow="hover" @click="navigateTo('/supplier')">
          <div class="module-header">
            <i class="el-icon-office-building"></i>
            <span>供应商管理</span>
          </div>
          <div class="module-content">
            <ul>
              <li>供应商基本信息管理</li>
              <li>供应商分类管理</li>
              <li>供应商评分管理</li>
            </ul>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="module-card" shadow="hover" @click="navigateTo('/hr')">
          <div class="module-header">
            <i class="el-icon-user"></i>
            <span>人事管理</span>
          </div>
          <div class="module-content">
            <ul>
              <li>离职管理</li>
              <li>请假管理</li>
              <li>申请单据</li>
              <li>审批流程</li>
            </ul>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="12" :md="6">
        <el-card class="module-card" shadow="hover" @click="navigateTo('/inventory')">
          <div class="module-header">
            <i class="el-icon-box"></i>
            <span>库存管理</span>
          </div>
          <div class="module-content">
            <ul>
              <li>库存管理</li>
              <li>领用管理</li>
              <li>数据导入导出</li>
            </ul>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 待办提醒区域 -->
    <el-row :gutter="20" class="mt20">
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>待审核文件</span>
              <el-button class="button" text>查看全部</el-button>
            </div>
          </template>
          <el-table :data="pendingFiles" style="width: 100%">
            <el-table-column prop="fileName" label="文件名称" />
            <el-table-column prop="type" label="类型" />
            <el-table-column prop="deadline" label="审核期限" />
          </el-table>
        </el-card>
      </el-col>

      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>待办事项</span>
              <el-button class="button" text>查看全部</el-button>
            </div>
          </template>
          <el-table :data="todoItems" style="width: 100%">
            <el-table-column prop="title" label="事项" />
            <el-table-column prop="type" label="类型" />
            <el-table-column prop="status" label="状态" />
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// 模拟数据
const pendingFiles = ref([
  { fileName: '质量体系认证', type: '证书', deadline: '2024-04-01' },
  { fileName: '安全生产许可证', type: '许可证', deadline: '2024-04-15' },
  { fileName: '特种设备操作证', type: '人员证书', deadline: '2024-04-20' }
])

const todoItems = ref([
  { title: '供应商年度评估', type: '供应商管理', status: '待处理' },
  { title: '员工请假审批', type: '人事管理', status: '待审批' },
  { title: '物资采购申请', type: '库存管理', status: '待审批' }
])

// 页面跳转
const navigateTo = (path) => {
  router.push(path)
}
</script>

<style lang="scss" scoped>
.dashboard-container {
  padding: 20px;

  .mb20 {
    margin-bottom: 20px;
  }

  .mt20 {
    margin-top: 20px;
  }

  .system-info {
    .system-description {
      line-height: 1.8;
      color: #666;
    }
  }

  .module-card {
    height: 200px;
    cursor: pointer;
    transition: all 0.3s;

    &:hover {
      transform: translateY(-5px);
    }

    .module-header {
      font-size: 18px;
      font-weight: bold;
      margin-bottom: 15px;
      color: #303133;

      i {
        margin-right: 8px;
      }
    }

    .module-content {
      ul {
        list-style: none;
        padding: 0;
        margin: 0;

        li {
          line-height: 2;
          color: #606266;
          font-size: 14px;

          &:before {
            content: "•";
            color: #409EFF;
            margin-right: 8px;
          }
        }
      }
    }
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>
