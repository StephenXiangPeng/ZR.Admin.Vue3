<template>
  <div class="app-container">
    <!-- 搜索和操作区域 -->
    <el-card class="search-card">
      <el-form :model="queryParams" ref="queryForm" :inline="true">
        <el-form-item label="姓名" prop="name">
          <el-input v-model="queryParams.name" placeholder="请输入姓名" clearable />
        </el-form-item>
        <el-form-item label="身份证号" prop="id_number">
          <el-input v-model="queryParams.id_number" placeholder="请输入身份证号" clearable />
        </el-form-item>
        <el-form-item label="科室" prop="department_center_id">
          <el-select v-model="queryParams.department_center_id" placeholder="请选择科室" clearable>
            <el-option label="科室1" value="1" />
            <el-option label="科室2" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="岗位" prop="position_id">
          <el-select v-model="queryParams.position_id" placeholder="请选择岗位" clearable>
            <el-option label="岗位1" value="1" />
            <el-option label="岗位2" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">搜索</el-button>
          <el-button @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>
      <div class="operation-buttons">
        <el-button type="primary" @click="handleAdd">新增员工</el-button>
      </div>
    </el-card>

    <!-- 表格区域 -->
    <el-card class="table-card">
      <el-table :data="employees" style="width: 100%" border :max-height="800">
        <el-table-column fixed label="关键信息" align="center">
          <el-table-column prop="name" label="姓名" width="120"></el-table-column>
          <el-table-column prop="id_number" label="身份证号" width="180"></el-table-column>
        </el-table-column>

        <el-table-column label="基本个人信息" align="center">
          <el-table-column prop="gender_id" label="性别" width="80"></el-table-column>
          <el-table-column prop="ethnicity_id" label="民族" width="100"></el-table-column>
          <el-table-column prop="birth_date" label="出生日期" width="120"></el-table-column>
          <el-table-column prop="age" label="年龄" width="80"></el-table-column>
          <el-table-column label="籍贯" align="center">
            <el-table-column prop="birthplace_province_id" label="省" width="120"></el-table-column>
            <el-table-column prop="birthplace_city_id" label="市" width="120"></el-table-column>
            <el-table-column prop="birthplace_district_id" label="区" width="120"></el-table-column>
          </el-table-column>
        </el-table-column>

        <el-table-column label="工作信息" align="center">
          <el-table-column prop="department_center_id" label="科室" width="150"></el-table-column>
          <el-table-column prop="internal_structure_id" label="内设机构" width="150"></el-table-column>
          <el-table-column prop="position_id" label="岗位" width="150"></el-table-column>
          <el-table-column prop="employee_type" label="人员性质" width="120"></el-table-column>
          <el-table-column prop="last_entry_date" label="最后一次进入该单位工作时间" width="180"></el-table-column>
          <el-table-column prop="overall_year_month" label="参加工作年月" width="120"></el-table-column>
        </el-table-column>

        <el-table-column label="岗位级别信息" align="center">
          <el-table-column prop="current_position_level" label="现任岗级" width="120"></el-table-column>
          <el-table-column prop="current_position_level_start_date" label="岗级起聘时间" width="120"></el-table-column>
          <el-table-column prop="current_position_start_date" label="岗位起聘时间" width="120"></el-table-column>
          <el-table-column prop="enterprise_staff_start_date" label="企编起聘时间" width="120"></el-table-column>
        </el-table-column>

        <el-table-column label="教育背景" align="center">
          <el-table-column prop="education_type_id" label="学历类型" width="120"></el-table-column>
          <el-table-column prop="highest_degree_id" label="最高学历" width="120"></el-table-column>
          <el-table-column prop="highest_graduation_school" label="毕业学校" width="180"></el-table-column>
          <el-table-column prop="highest_major" label="专业" width="150"></el-table-column>
        </el-table-column>

        <el-table-column label="技能职称信息" align="center">
          <el-table-column prop="department_skill_level" label="技能等级" width="120"></el-table-column>
          <el-table-column prop="department_skill_level_date" label="技能评定时间" width="120"></el-table-column>
          <el-table-column prop="technical_title_level" label="技术职称等级" width="120"></el-table-column>
          <el-table-column prop="promotion_date" label="晋升评定时间" width="120"></el-table-column>
        </el-table-column>

        <el-table-column label="联系方式与户籍信息" align="center">
          <el-table-column prop="phone_number" label="联系电话" width="130"></el-table-column>
          <el-table-column prop="current_address" label="现住地址" width="200"></el-table-column>
          <el-table-column prop="household_address" label="户口所在地" width="200"></el-table-column>
          <el-table-column prop="household_type" label="户口性质" width="120"></el-table-column>
        </el-table-column>

        <el-table-column label="保险与福利信息" align="center">
          <el-table-column prop="card_number" label="一卡通号" width="120"></el-table-column>
          <el-table-column prop="join_mutual_aid" label="参加大病互助" width="120">
            <template #default="scope">
              {{ scope.row.join_mutual_aid ? '是' : '否' }}
            </template>
          </el-table-column>
          <el-table-column prop="insurance_type" label="参保类型" width="120"></el-table-column>
          <el-table-column prop="logistics_insurance_date" label="参保时间" width="120"></el-table-column>
        </el-table-column>

        <el-table-column label="退休信息" align="center">
          <el-table-column prop="retired" label="是否退休" width="100">
            <template #default="scope">
              {{ scope.row.retired ? '是' : '否' }}
            </template>
          </el-table-column>
          <el-table-column prop="retirement_date" label="退休年月" width="120"></el-table-column>
        </el-table-column>

        <el-table-column fixed="right" label="操作" align="center" width="150">
          <template v-slot="scope">
            <el-button @click="editEmployee(scope.row)" type="primary" size="small">编辑</el-button>
            <el-button @click="deleteEmployee(scope.row.id)" type="danger" size="small">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分页组件 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="queryParams.pageNum"
          v-model:page-size="queryParams.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>

    <!-- 添加/编辑对话框 -->
    <el-dialog
      :title="dialog.title"
      v-model="dialog.visible"
      width="800px"
      append-to-body
    >
      <!-- 对话框的表单内容将在后续实现 -->
      <div>表单内容待实现</div>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialog.visible = false">取 消</el-button>
          <el-button type="primary" @click="submitForm">确 定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';

// 查询参数
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  name: '',
  id_number: '',
  department_center_id: '',
  position_id: ''
});

// 总条数
const total = ref(0);

// 对话框配置
const dialog = reactive({
  title: '',
  visible: false,
  type: 'add' // 'add' 或 'edit'
});

// 员工数据列表
const employees = ref([
  {
    id: 1,
    name: '张三',
    gender_id: 1,
    id_number: '330102199001011234',
    department_center_id: 101,
    internal_structure_id: 1,
    position_id: 201,
    phone_number: '13800138000',
    education_type_id: 1,
    highest_degree_id: 1,
    employee_type: 1,
    join_mutual_aid: 1,
    retired: 0
  },
]);

// 查询方法
const handleQuery = () => {
  // TODO: 实现查询逻辑
  console.log('查询参数：', queryParams);
};

// 重置查询
const resetQuery = () => {
  queryParams.name = '';
  queryParams.id_number = '';
  queryParams.department_center_id = '';
  queryParams.position_id = '';
  handleQuery();
};

// 新增员工
const handleAdd = () => {
  dialog.title = '新增员工';
  dialog.type = 'add';
  dialog.visible = true;
};

// 编辑员工
const editEmployee = (row: any) => {
  dialog.title = '编辑员工';
  dialog.type = 'edit';
  dialog.visible = true;
  // TODO: 加载员工数据到表单
};

// 删除员工
const deleteEmployee = (id: number) => {
  ElMessageBox.confirm('确认要删除该员工吗？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    // TODO: 实现删除逻辑
    ElMessage.success('删除成功');
  }).catch(() => {});
};

// 提交表单
const submitForm = () => {
  // TODO: 实现表单提交逻辑
  dialog.visible = false;
  ElMessage.success('操作成功');
};

// 分页方法
const handleSizeChange = (val: number) => {
  queryParams.pageSize = val;
  handleQuery();
};

const handleCurrentChange = (val: number) => {
  queryParams.pageNum = val;
  handleQuery();
};
</script>

<style scoped>
.app-container {
  padding: 20px;
}

.search-card {
  margin-bottom: 20px;
}

.operation-buttons {
  margin-top: 20px;
}

.table-card {
  margin-top: 20px;
}

.pagination-container {
  margin-top: 20px;
  text-align: right;
}

.dialog-footer {
  text-align: right;
}
</style>
