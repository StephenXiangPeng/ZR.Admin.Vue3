<template>
  <div>
    <h1>员工基本信息</h1>
    <el-table :data="employees" style="width: 100%">
      <el-table-column prop="name" label="姓名" width="180"></el-table-column>
      <el-table-column prop="gender_id" label="性别" width="100"></el-table-column>
      <el-table-column prop="department_center_id" label="科室" width="180"></el-table-column>
      <el-table-column prop="position_id" label="岗位" width="180"></el-table-column>
      <el-table-column label="操作" width="180">
        <template v-slot="scope">
          <el-button @click="editEmployee(scope.row)" type="primary" size="small">编辑</el-button>
          <el-button @click="deleteEmployee(scope.row.id)" type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-button @click="addEmployee" type="primary">新增员工</el-button>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { ElMessage } from 'element-plus';

const employees = ref([
  // 示例数据，替换为 API 调用
  { id: 1, name: '张三', gender_id: 1, department_center_id: 101, position_id: 201 },
  { id: 2, name: '李四', gender_id: 2, department_center_id: 102, position_id: 202 },
]);

function addEmployee() {
  // 添加新员工的逻辑
  ElMessage.success('新增员工功能待实现');
}

function editEmployee(employee: any) {
  // 编辑现有员工的逻辑
  ElMessage.success(`编辑员工 ${employee.name} 功能待实现`);
}

function deleteEmployee(id: number) {
  // 删除员工的逻辑
  ElMessage.success(`删除员工 ID: ${id} 功能待实现`);
}
</script>
