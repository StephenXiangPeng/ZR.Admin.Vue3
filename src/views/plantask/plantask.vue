<template>
	<div>
		<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;添加计划/任务</span>
		<el-divider></el-divider>
		<el-form ref="PlanTaskFormRef" style="max-width: 100%" :model="PlanTaskForm" :rules="rules" label-width="auto"
			class="demo-ruleForm" status-icon :inline="true">
			<el-form-item label="计划/任务名称" prop="plantaskname" style="width: 50%;">
				<el-input v-model="PlanTaskForm.plantaskname" />
			</el-form-item>
			<el-form-item label="参与人员" prop="participants" style="width: 50%;">
				<el-select v-model="PlanTaskForm.participants" filterable placeholder="选择参与人员" multiple
					style="width: 100%;">
					<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode" :label="dict.dictLabel"
						:value="dict.dictValue"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="开始时间" prop="starttime" style="width: 50%;">
				<el-date-picker v-model="PlanTaskForm.starttime" type="date" aria-label="选择日期" placeholder="请选择开始日期"
					style="width: 50%;" />
			</el-form-item>
			<el-form-item label="结束时间" prop="endtime" style=" width: 50%;">
				<el-date-picker v-model="PlanTaskForm.endtime" type="date" aria-label="选择日期" placeholder="请选择结束日期"
					style="width: 50%;" />
			</el-form-item>
			<el-form-item label="计划/任务描述" prop="plantaskdescription" style=" width: 50%;">
				<el-input v-model="PlanTaskForm.plantaskdescription" type="textarea" :rows="5" style="width: 100%;" />
			</el-form-item>
			<el-form-item style=" width: 50%;">
				<el-button type="primary" @click="SubmitPlanTaskForm(PlanTaskFormRef)">
					保存
				</el-button>
				<el-button @click="ResetPlanTaskForm(PlanTaskFormRef)">重置</el-button>
			</el-form-item>
		</el-form>
		<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;计划/任务列表</span>
		<el-divider></el-divider>
		<el-table ref="PlanTaskDataTable" :data="PlanTaskDataTable" style="width: 100%">
			<el-table-column prop="productCode" label="计划任务名称" width="200px" />
			<el-table-column prop="chineseProductName" label="参与人员" width="400px" />
			<el-table-column prop="englishProductName" label="详细描述" width="400px" />
			<el-table-column prop="chineseSpecification" label="开始时间" width="200px" />
			<el-table-column prop="englishSpecification" label="结束时间" width="200px" />
			<el-table-column prop="unitOfMeasurement" label="发起人" width="200px" />
		</el-table>
		<el-pagination @current-change="PlanTaskDataTablePageChange" :current-page="PlanTaskDataTableCurrentPage"
			:page-size="PlanTaskDataTablePageSize" :total="PlanTaskDataTableTotalItems" background
			layout="prev, pager, next" style="margin-top: 5px;" />
	</div>
</template>
<script setup lang="ts">
import { reactive, ref } from 'vue'
import type { ComponentSize, FormInstance, FormRules } from 'element-plus'
import request from '@/utils/request';

//任务计划表单
interface PlanTaskForm {
	plantaskname: string,
	participants: string,
	starttime: string,
	endtime: string,
	plantaskdescription: string
}

const PlanTaskFormRef = ref<FormInstance>()
const PlanTaskForm = reactive<PlanTaskForm>({
	plantaskname: '',
	participants: '',
	starttime: '',
	endtime: '',
	plantaskdescription: ''
})

const rules = reactive<FormRules<PlanTaskForm>>({
	plantaskname: [
		{ required: true, message: '请输入计划/任务名称', trigger: 'change,blur' },
	],
	participants: [
		{ required: true, message: '请选择参与人员', trigger: 'change,blur' }
	],
	starttime: [
		{ required: true, message: '请选择开始时间', trigger: 'change,blur' }
	],
	endtime: [
		{ required: true, message: '请选择结束时间', trigger: 'change,blur' }
	],
	plantaskdescription: [
		{ required: true, message: '请输入计划/任务描述', trigger: 'change,blur' }
	]
})

//重置任务计划表单
const ResetPlanTaskForm = (formEl: FormInstance | undefined) => {
	if (!formEl) return
	formEl.resetFields()
	PlanTaskForm.plantaskname = ''
	PlanTaskForm.participants = ''
	PlanTaskForm.starttime = ''
	PlanTaskForm.endtime = ''
	PlanTaskForm.plantaskdescription = ''
}

//提交任务计划表单
const SubmitPlanTaskForm = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate((valid, fields) => {
		if (valid) {
			console.log('submit!')
		} else {
			console.log('error submit!', fields)
		}
	})
}

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		sql_all_user: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_all_user' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	//GetQuotationList(currentPage.value, pageSize.value);
})
/*动态下拉框end*/

const PlanTaskDataTable = reactive([]); //任务计划列表
//分页组件
const PlanTaskDataTableTotalItems = ref(0);
const PlanTaskDataTableCurrentPage = ref(1);
const PlanTaskDataTablePageSize = ref(10);
const PlanTaskDataTablePageChange = async (newPage) => {
	PlanTaskDataTablePageSize.value = newPage;
	const start = newPage;
	const end = PlanTaskDataTablePageSize.value;
	//const newData = await GetProductInfoList(start, end);
};
</script>