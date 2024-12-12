<template>
  <div>
    <el-form
      ref="employeeFormRef"
      :model="employeeForm"
      :rules="rules"
      label-width="160px"
      class="employee-form"
    >
      <!-- 关键信息 -->
      <el-divider content-position="left">关键信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="姓名" prop="name">
            <el-input v-model="employeeForm.name" placeholder="请输入姓名" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="身份证号" prop="id_number">
            <el-input v-model="employeeForm.id_number" placeholder="请输入身份证号" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="政治面貌" prop="political_status_id">
            <el-select v-model="employeeForm.political_status_id" filterable placeholder="请选择政治面貌" clearable>
              <el-option v-for="dict in state.jdrs_zzmm" :key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 个人基本信息 -->
      <el-divider content-position="left">个人基本信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="性别" prop="gender_id">
            <el-radio-group v-model="employeeForm.gender_id">
              <el-radio :label="1">男</el-radio>
              <el-radio :label="2">女</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="民族" prop="ethnicity_id">
            <el-select v-model="employeeForm.ethnicity_id"  filterable placeholder="请选择民族" clearable>
              <el-option v-for="dict in state.jdrs_mz" :key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="出生年月" prop="birth_date">
            <el-date-picker
              v-model="employeeForm.birth_date"
              type="date"
              placeholder="选择出生年月"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
		 <el-col :span="6">
          <el-form-item label="年龄" prop="age">
            <el-input v-model="employeeForm.age" placeholder="请输入年龄" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
		<el-col :span="6">
          <el-form-item label="籍贯省" prop="birthplace_province_id">
            <el-select 
              v-model="employeeForm.birthplace_province_id" 
              placeholder="请选择省份"
              @change="handleProvinceChange"
            >
              <el-option 
                v-for="item in provinceOptions" 
                :key="item.value" 
                :label="item.label" 
                :value="item.value" 
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="籍贯市" prop="birthplace_city_id">
            <el-select 
              v-model="employeeForm.birthplace_city_id" 
              placeholder="请选择城市"
              @change="handleCityChange"
            >
              <el-option 
                v-for="item in cityOptions" 
                :key="item.value" 
                :label="item.label" 
                :value="item.value" 
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="籍贯区" prop="birthplace_district_id">
            <el-select 
              v-model="employeeForm.birthplace_district_id" 
              placeholder="请选择区县"
            >
              <el-option 
                v-for="item in districtOptions" 
                :key="item.value" 
                :label="item.label" 
                :value="item.value" 
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 工作信息 -->
      <el-divider content-position="left">工作信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="科室(中心)" prop="department_center_id">
            <el-select v-model="employeeForm.department_center_id" placeholder="请选择科室">
              <el-option label="科室1" :value="1" />
              <el-option label="科室2" :value="2" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="内设机构" prop="internal_structure_id">
            <el-input v-model="employeeForm.internal_structure_id" placeholder="请输入内设机构" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="岗位名称" prop="position_id">
            <el-select v-model="employeeForm.position_id" placeholder="请选择岗位">
              <el-option label="岗位1" :value="1" />
              <el-option label="岗位2" :value="2" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="人员性质" prop="employee_type">
            <el-select v-model="employeeForm.employee_type" placeholder="请选择人员性质">
              <el-option label="正式" :value="1" />
              <el-option label="临时" :value="2" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
		 <el-col :span="6">
          <el-form-item label="企编起聘时间" prop="enterprise_staff_start_date">
            <el-date-picker
              v-model="employeeForm.enterprise_staff_start_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
        
        <el-col :span="6">
          <el-form-item label="参加工作年月" prop="overall_year_month">
            <el-date-picker
              v-model="employeeForm.overall_year_month"
              type="month"
              placeholder="选择年月"
              format="YYYY-MM"
            />
          </el-form-item>
        </el-col>
		<el-col :span="8">
          <el-form-item label="最后一次进入后勤时间" prop="last_entry_date">
            <el-date-picker
              v-model="employeeForm.last_entry_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 岗位级别信息 -->
      <el-divider content-position="left">岗位级别信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="现任岗级" prop="current_position_level">
            <el-input v-model="employeeForm.current_position_level" placeholder="请输入现任岗级" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="现任岗级起聘时间" prop="current_position_level_start_date">
            <el-date-picker
              v-model="employeeForm.current_position_level_start_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="现任岗位起聘时间" prop="current_position_start_date">
            <el-date-picker
              v-model="employeeForm.current_position_start_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
       
      </el-row>

      <!-- 教育背景 -->
      <el-divider content-position="left">教育背景</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="学历类型" prop="education_type_id">
            <el-input v-model="employeeForm.education_type_id" placeholder="请输入学历类型" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="最高学历(全/非全)" prop="highest_degree_id">
            <el-input v-model="employeeForm.highest_degree_id" placeholder="请输入最高学历" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="最高学历毕业学校" prop="highest_graduation_school">
            <el-input v-model="employeeForm.highest_graduation_school" placeholder="请输入毕业学校" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="最高学历毕业专业" prop="highest_major">
            <el-input v-model="employeeForm.highest_major" placeholder="请输入专业" />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 技能职称信息 -->
      <el-divider content-position="left">技能职称信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="技能等级" prop="department_skill_level">
            <el-input v-model="employeeForm.department_skill_level" placeholder="请输入技能等级" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="技能评定时间" prop="department_skill_level_date">
            <el-date-picker
              v-model="employeeForm.department_skill_level_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="技术职称等级" prop="technical_title_level">
            <el-input v-model="employeeForm.technical_title_level" placeholder="请输入技术职称等级" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="晋升评定时间" prop="promotion_date">
            <el-date-picker
              v-model="employeeForm.promotion_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="技能证书名称" prop="skill_certificate_name">
            <el-input v-model="employeeForm.skill_certificate_name" placeholder="请输入技能证书名称" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="技能证书等级" prop="skill_certificate_level">
            <el-input v-model="employeeForm.skill_certificate_level" placeholder="请输入技能证书等级" />
          </el-form-item>
        </el-col>
        <el-col :span="10">
          <el-form-item label="附件" prop="attachment">
            <el-upload
              action="#"
              list-type="text"
              :on-preview="handlePreview"
              :on-remove="handleRemove"
            >
              <el-button size="small" type="primary">点击上传</el-button>
            </el-upload>
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 联系方式与户籍信息 -->
      <el-divider content-position="left">联系方式与户籍信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="联系电话" prop="phone_number">
            <el-input v-model="employeeForm.phone_number" placeholder="请输入联系电话" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="现住地址" prop="current_address">
            <el-input v-model="employeeForm.current_address" placeholder="请输入现住地址" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="户口所在地" prop="household_address">
            <el-input v-model="employeeForm.household_address" placeholder="请输入户口所在地" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="户口性质" prop="household_type">
            <el-input v-model="employeeForm.household_type" placeholder="请输入户口性质" />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 保险与福利信息 -->
      <el-divider content-position="left">保险与福利信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="一卡通号" prop="card_number">
            <el-input v-model="employeeForm.card_number" placeholder="请输入一卡通号" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="参加大病互助" prop="join_mutual_aid">
            <el-switch v-model="employeeForm.join_mutual_aid" active-value="1" inactive-value="0" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="参保类型" prop="insurance_type">
            <el-input v-model="employeeForm.insurance_type" placeholder="请输入参保类型" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="参保时间" prop="logistics_insurance_date">
            <el-date-picker
              v-model="employeeForm.logistics_insurance_date"
              type="date"
              placeholder="选择日期"
              format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 退休信息 -->
      <el-divider content-position="left">退休信息</el-divider>
      <el-row :gutter="20">
        <el-col :span="6">
          <el-form-item label="是否退休" prop="retired">
            <el-switch v-model="employeeForm.retired" active-value="1" inactive-value="0" />
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="退休年月" prop="retirement_date">
            <el-date-picker
              v-model="employeeForm.retirement_date"
              type="month"
              placeholder="选择年月"
              format="YYYY-MM"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getDicts } from '@/api/system/dict/data'
const { proxy } = getCurrentInstance()
const state = reactive({
  form: {},
  queryParams: {
    menuName: undefined,
    visible: undefined,
    menuTypeIds: 'M,C',
    parentId: undefined
  },
  rules: {},
  jdrs_zzmm: [],
  jdrs_mz: []
})

var dictParams = [{ dictType: 'jdrs_zzmm' }, { dictType: 'jdrs_mz' }]
proxy.getDicts(dictParams).then((response) => {
  response.data.forEach((element) => {
    state[element.dictType] = element.list
  })
})

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

// 省市区数据
const provinceOptions = ref([
  { value: '1', label: '北京市' },
  { value: '2', label: '上海市' },
  // ... 其他省份数据
]);

const cityOptions = ref([]);
const districtOptions = ref([]);

// 省份改变时的处理函数
const handleProvinceChange = (value: string) => {
  employeeForm.birthplace_city_id = '';
  employeeForm.birthplace_district_id = '';
  // 这里应该根据选择的省份加载对应的城市数据
  cityOptions.value = [
    { value: '1', label: '市辖区' },
    { value: '2', label: '县' },
  ];
};

// 城市改变时的处理函数
const handleCityChange = (value: string) => {
  employeeForm.birthplace_district_id = '';
  // 这里应该根据选择的城市加载对应的区县数据
  districtOptions.value = [
    { value: '1', label: '东城区' },
    { value: '2', label: '西城区' },
  ];
};

// 表单引用
const employeeFormRef = ref();

// 表单数据
const employeeForm = reactive({
  name: '',
  id_number: '',
  gender_id: '',
  ethnicity_id: '',
  birth_date: '',
  age: '',
  birthplace_province_id: '',
  birthplace_city_id: '',
  birthplace_district_id: '',
  department_center_id: '',
  internal_structure_id: '',
  position_id: '',
  employee_type: '',
  last_entry_date: '',
  overall_year_month: '',
  current_position_level: '',
  current_position_level_start_date: '',
  current_position_start_date: '',
  enterprise_staff_start_date: '',
  education_type_id: '',
  highest_degree_id: '',
  highest_graduation_school: '',
  highest_major: '',
  department_skill_level: '',
  department_skill_level_date: '',
  technical_title_level: '',
  promotion_date: '',
  phone_number: '',
  current_address: '',
  household_address: '',
  household_type: '',
  card_number: '',
  join_mutual_aid: 0,
  insurance_type: '',
  logistics_insurance_date: '',
  retired: 0,
  retirement_date: '',
  political_status_id: '',
  skill_certificate_name: '',
  skill_certificate_level: '',
  attachment: null
});

// 表单验证规则
const rules = {
  name: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  id_number: [
    { required: true, message: '请输入身份证号', trigger: 'blur' },
    { pattern: /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/, message: '请入正确的身份证号', trigger: 'blur' }
  ],
  gender_id: [
    { required: true, message: '请选择性别', trigger: 'change' }
  ],
  phone_number: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  political_status_id: [
    { required: true, message: '请选择政治面貌', trigger: 'change' }
  ]
};

// 上传处理函数
const handlePreview = (file) => {
  console.log('Preview:', file);
};

const handleRemove = (file, fileList) => {
  console.log('Remove:', file, fileList);
};
</script>

<style scoped>
.el-input {
  width: 180px !important;  /* 文本输入框统一宽度 */
}
.el-select {
  width: 180px !important;  /* 下拉框统一宽度 */
}
:deep(.el-date-editor.el-input) {
  width: 180px !important;  /* 日期选择器统一宽度 */
}
:deep(.el-date-editor--date) {
  width: 180px !important;  /* 日期选择器统一宽度 */
}
:deep(.el-date-editor--month) {
  width: 180px !important;  /* 月份选择器统一宽度 */
}
.el-form-item {
  margin-bottom: 18px;  /* 统一表单项间距 */
}

/* 为特定的长标签表单项添加特殊样式 */
:deep(.long-label) {
  .el-form-item__label {
    width: 200px !important;  /* 为长标签设置更宽的宽度 */
  }
  /* 调整内容区域位置 */
  .el-form-item__content {
    margin-left: 200px !important;
  }
}
</style>
