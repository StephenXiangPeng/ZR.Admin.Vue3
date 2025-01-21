<template>
	<div>
		<el-form :model="queryParams" inline @submit.prevent ref="queryRef">
			<el-form-item prop="queryText">
				<el-input v-model="queryParams.queryText" placeholder="请输入计划任务名称" clearable @clear="Search_PlanTaskInfo"
					style="width: 600px" />
			</el-form-item>
			<el-form-item>
				<el-button type="primary" icon="search" @click="Search_PlanTaskInfo">{{ $t('btn.search') }}</el-button>
				<el-button type="primary" @click="OpenPlanTaskDialog">新建计划/任务</el-button>
			</el-form-item>
		</el-form>
		<el-dialog v-model="PlanTaskDialogVisible" title="新建计划/任务" width="1000">
			<el-form ref="PlanTaskFormRef" :model="PlanTaskForm" :rules="rules" label-width="120px"
				class="demo-ruleForm" status-icon :inline="true">
				<el-row>
					<el-col :span="12">
						<el-form-item label="计划/任务名称" prop="plantaskname">
							<el-input v-model="PlanTaskForm.plantaskname" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="总负责人" prop="participants">
							<el-select v-model="PlanTaskForm.participants" filterable placeholder="选择总负责人" multiple
								clearable style="width: 300px">
								<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="12">
						<el-form-item label="开始时间" prop="starttime">
							<el-date-picker v-model="PlanTaskForm.starttime" type="date" aria-label="选择日期"
								placeholder="请选择开始日期" style="width: 300px" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="结束时间" prop="endtime">
							<el-date-picker v-model="PlanTaskForm.endtime" type="date" aria-label="选择日期"
								placeholder="请选择结束日期" style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="12">
						<el-form-item label="计划/任务描述" prop="plantaskdescription">
							<el-input v-model="PlanTaskForm.plantaskdescription" type="textarea" :rows="5"
								style="width: 785px" />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="24">
						<el-form-item label="总阶段数" prop="TotalStageNumber">
							<el-input-number v-model="PlanTaskForm.TotalStageNumber" :min="1" :max="10"
								@change="handleStageNumberChange" style="width: 300px" />
						</el-form-item>
					</el-col>
				</el-row>
				<!-- 动态阶段表单 -->
				<div v-for="(stage, stageIndex) in stages" :key="stageIndex" class="stage-container">
					<el-divider>第{{ stageIndex + 1 }}阶段</el-divider>
					<el-row>
						<el-col :span="24">
							<el-form-item :label="'阶段名称'" :prop="'stages.' + stageIndex + '.name'" style="width: 50%;">
								<div style="display: flex; gap: 10px; align-items: center;">
									<el-input v-model="stage.name" placeholder="请输入阶段名称" style="width: 350px;" />
									<el-button type="primary" @click="addItem(stageIndex)">添加事项</el-button>
								</div>
							</el-form-item>
						</el-col>
					</el-row>

					<!-- 具体事项列表 -->
					<div v-for="(item, itemIndex) in stage.items" :key="itemIndex" class="item-container">
						<el-row>
							<el-col :span="12">
								<el-form-item :label="'事项名称'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.name'">
									<el-input v-model="item.name" placeholder="请输入事项名称" style="width: 300px" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item :label="'执行人'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.executor'">
									<el-select v-model="item.executor" filterable placeholder="选择执行人"
										style="width: 300px">
										<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue">
										</el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="12">
								<el-form-item :label="'时间节点'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.deadline'">
									<el-date-picker v-model="item.deadline" type="date" placeholder="选择时间节点"
										style="width: 300px" />
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-button type="danger" @click="removeItem(stageIndex, itemIndex)">删除事项</el-button>
							</el-col>
						</el-row>
					</div>
				</div>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="ResetPlanTaskForm(PlanTaskFormRef)">重置</el-button>
					<el-button type="primary" @click="SubmitPlanTaskForm(PlanTaskFormRef)">
						保存
					</el-button>
				</span>
			</template>
		</el-dialog>
		<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;计划/任务</span>
		<el-divider></el-divider>

		<!-- 任务列表 -->
		<el-row :gutter="20">
			<el-col v-for="item in dataPlanTasks" :key="item.id" :lg="8" :span="24">
				<el-card v-loading="loading" :body-style="{ padding: '15px 15px 0', height: '330px' }" class="mb-20">
					<el-descriptions :column="1" :title="item.taskName" size="small" border>
						<el-descriptions-item label="计划任务名称">
							{{ item.taskName }}
						</el-descriptions-item>
						<el-descriptions-item label="总负责人">
							{{ item.totalLeaderIds }}
						</el-descriptions-item>
						<el-descriptions-item label="开始日期">
							{{ formatDate(item.startTime) }}
						</el-descriptions-item>
						<el-descriptions-item label="结束日期">
							{{ formatDate(item.endTime) }}
						</el-descriptions-item>
						<el-descriptions-item label="任务描述">
							{{ item.taskDescription }}
						</el-descriptions-item>
						<el-descriptions-item label="创建日期">
							{{ formatDate(item.create_time) }}
						</el-descriptions-item>
						<el-descriptions-item label="当前进度">
							<el-progress :text-inside="true" :stroke-width="24" :percentage="item.completionRate"
								:status="item.completionRate >= 1 ? 'success' : ''" />
						</el-descriptions-item>
					</el-descriptions>
					<div style="text-align: right; margin-top: 15px;">
						<el-button type="primary" size="default" @click="showTaskDetail(item)">
							查看详情
						</el-button>
					</div>
				</el-card>
			</el-col>
		</el-row>
		<!-- 分页 -->
		<el-pagination v-model:current-page="queryParams.pageNum" v-model:page-size="queryParams.pageSize"
			:total="total" :page-sizes="[3, 6, 9, 12]" layout="total, sizes, prev, pager, next, jumper"
			@size-change="handleSizeChange" @current-change="handlePageChange" />
		<el-drawer v-model="drawer" :title="selectedTask?.taskName || '任务详情'" :direction="direction"
			:before-close="handleClose" size="40%">
			<template #default>
				<el-timeline v-if="selectedTaskPhases && selectedTaskPhases.length">
					<el-timeline-item v-for="phaseData in selectedTaskPhases"
						:key="phaseData?.phase?.id || Math.random()" :timestamp="phaseData?.phase?.phaseName"
						:type="getPhaseStatus(phaseData?.items)" placement="top">
						<el-card>
							<el-timeline-item v-for="item in phaseData?.items || []" :key="item?.id || Math.random()"
								:timestamp="formatDate(item?.timePoint)"
								:type="item?.realTimePoint ? 'success' : 'primary'" size="large">
								<!-- 现有的事项内容保持不变 -->
								<div class="task-item-title">
									{{ item?.itemName }}
									<el-link type="success"
										v-if="currentUser === item?.executorId && !item?.realTimePoint"
										@click="handleTaskComplete(item)">确认完成</el-link>
								</div>
								<p>执行人: {{ getUserName(item?.executorId) }}</p>
								<p>计划完成时间: {{ formatDate(item?.timePoint) }}</p>
								<p>{{ item?.realTimePoint ? `实际完成时间: ${formatDate(item?.realTimePoint)}` : '状态: 未完成' }}
								</p>
								<p>{{ item?.remark ? `备注: ${item?.remark}` : '' }}</p>
							</el-timeline-item>
						</el-card>
					</el-timeline-item>
				</el-timeline>
				<div v-else>
					<el-empty description="暂无数据" />
				</div>
			</template>
		</el-drawer>

		<!-- 添加确认对话框 -->
		<el-dialog v-model="confirmDialogVisible" title="确认完成任务" width="30%">
			<div class="completion-dialog">
				<p>确认完成任务：{{ currentTask?.itemName }}？</p>
				<el-input v-model="completionNote" type="textarea" rows="3" placeholder="请输入备注（选填）"></el-input>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="confirmDialogVisible = false">取消</el-button>
					<el-button type="primary" @click="confirmTaskCompletion">
						确认完成
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { reactive, ref } from 'vue'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElMessage, DrawerProps, ElDrawer,
	ElMessageBox, ElTimeline, ElTimelineItem, FormInstance
} from 'element-plus'
import request from '@/utils/request';
import useUserStore from '@/store/modules/user'
const userStore = useUserStore()
const currentUser = ref(userStore.userId)
// 添加确认对话框相关的响应式变量
const confirmDialogVisible = ref(false)
const currentTask = ref(null)
const completionNote = ref('')

// 添加获取阶段状态的函数
const getPhaseStatus = (items: any[]) => {
	if (!items || items.length === 0) return 'info';

	const completedCount = items.filter(item => item.realTimePoint).length;
	const totalCount = items.length;

	if (completedCount === totalCount) {
		return 'success';  // 全部完成 - 绿色
	} else if (completedCount === 0) {
		return 'danger';   // 全部未完成 - 红色
	} else {
		return 'warning';  // 部分完成 - 黄色
	}
};

// 处理任务完成勾选
const handleTaskComplete = (task) => {
	currentTask.value = task
	confirmDialogVisible.value = true
}

// 确认完成任务
const confirmTaskCompletion = async () => {
	try {
		const res = await request.get(`PlanTasks/ConfirmationOfCompletion/ConfirmItem`, {
			params: {
				ID: currentTask.value.id,
				remark: completionNote.value
			}
		});
		if (res.code === 200) {
			ElMessage.success('任务完成确认成功');
			// 刷新任务详情
			await showTaskDetail(selectedTask.value);
		} else {
			ElMessage.error(res.msg || '确认失败');
		}
	} catch (error) {
		console.error('确认失败:', error);
		ElMessage.error('确认失败：' + (error.message || '未知错误'));
	} finally {
		confirmDialogVisible.value = false;
		completionNote.value = '';
		currentTask.value = null;
	}
}

const selectedTask = ref(null);
const selectedTaskPhases = ref([]);
const drawer = ref(false);
const direction = ref<DrawerProps['direction']>('rtl')
const handleClose = () => {
	drawer.value = false;
}

// 修改查看详情按钮的点击事件
const showTaskDetail = async (task) => {
	selectedTask.value = task;
	try {
		const res = await request.get(`PlanTasks/GetPlanTaskDetails/GetDetails?ID=${task.id}`);
		if (res.code === 200 && res.data.code === 200) {
			// 确保数据存在且是数组
			selectedTaskPhases.value = Array.isArray(res.data.data) ? res.data.data : [];
			console.log('任务详情数据:', selectedTaskPhases.value); // 用于调试
		}
	} catch (error) {
		console.error('获取任务详情失败:', error);
		ElMessage.error('获取任务详情失败');
		selectedTaskPhases.value = []; // 出错时清空数据
	}
	drawer.value = true;
}

// 获取用户名的辅助函数
const getUserName = (executorId) => {
	const user = optionss.value.sql_all_user.find(u => u.dictValue === executorId.toString());
	return user ? user.dictLabel : executorId;
}

// 日期格式化函数
const formatDate = (dateStr: string) => {
	if (!dateStr) return '';
	const date = new Date(dateStr);
	const year = date.getFullYear().toString(); // 获取后两位年份
	const month = (date.getMonth() + 1).toString().padStart(2, '0'); // 月份补零
	const day = date.getDate().toString().padStart(2, '0'); // 日期补零
	return `${year}-${month}-${day}`;
};

//提交任务计划表单
const SubmitPlanTaskForm = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate(async (valid, fields) => {
		if (valid) {
			try {
				// 构建请求数据，使用与后端 JsonPropertyName 匹配的命名
				const requestData = {
					taskName: PlanTaskForm.plantaskname.trim(),
					taskDescription: PlanTaskForm.plantaskdescription?.trim() || '',
					startTime: PlanTaskForm.starttime ? new Date(PlanTaskForm.starttime).toISOString() : new Date().toISOString(),
					endTime: PlanTaskForm.endtime ? new Date(PlanTaskForm.endtime).toISOString() : new Date().toISOString(),
					totalLeaderIds: Array.isArray(PlanTaskForm.participants)
						? PlanTaskForm.participants.join(',')
						: PlanTaskForm.participants || '',
					planTask_Phases: stages.value.map(stage => ({
						phaseName: stage.name?.trim() || '',
						planTask_Items: stage.items.map(item => ({
							itemName: item.name?.trim() || '',
							executorId: Number(item.executor) || 0,
							timePoint: item.deadline ? new Date(item.deadline).toISOString() : null
						}))
					}))
				};
				// 验证关键字段
				if (!requestData.taskName) {
					ElMessage.error('任务名称不能为空');
					return;
				}
				console.log('发送的数据:', JSON.stringify(requestData, null, 2));
				const res = await request.post('PlanTasks/AddPlanTasks/Add', requestData);
				if (res.code === 200) {
					ElMessage.success('保存成功');
					PlanTaskDialogVisible.value = false;
					ResetPlanTaskForm(formEl);
					getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
				} else {
					ElMessage.error(res.msg || '保存失败');
				}
			} catch (error) {
				console.error('保存失败:', error);
				ElMessage.error('保存失败，请稍后重试');
			}
		} else {
			console.log('表单验证失败:', fields);
			ElMessage.warning('请填写完整的表单信息');
		}
	})
}

const PlanTaskDialogVisible = ref(false);

const OpenPlanTaskDialog = () => {
	PlanTaskDialogVisible.value = true;
	// 确保打开对话框时有一个默认阶段
	if (stages.value.length === 0) {
		stages.value = [{
			name: '',
			items: [{
				name: '',
				executor: '',
				deadline: ''
			}]
		}];
	}
}

interface StageItem {
	name: string;
	executor: string;
	deadline: string;
}

interface Stage {
	name: string;
	items: StageItem[];
}

// Add to your reactive data
const stages = ref<Stage[]>([{
	name: '',
	items: [{
		name: '',
		executor: '',
		deadline: ''
	}]
}]);

// Add these methods
const handleStageNumberChange = (value: number) => {
	const currentLength = stages.value.length;
	if (value > currentLength) {
		// Add new stages
		for (let i = currentLength; i < value; i++) {
			stages.value.push({
				name: '',
				items: [{
					name: '',
					executor: '',
					deadline: ''
				}]
			});
		}
	} else if (value < currentLength) {
		// Remove excess stages
		stages.value.splice(value);
	}
}

const addItem = (stageIndex: number) => {
	stages.value[stageIndex].items.push({
		name: '',
		executor: '',
		deadline: ''
	});
}

const removeItem = (stageIndex: number, itemIndex: number) => {
	stages.value[stageIndex].items.splice(itemIndex, 1);
}

//任务计划表单
interface PlanTaskForm {
	plantaskname: string,
	participants: string,
	TotalStageNumber: 1,
	starttime: string,
	endtime: string,
	plantaskdescription: string
}

const PlanTaskFormRef = ref<FormInstance>()
const PlanTaskForm = reactive<PlanTaskForm>({
	plantaskname: '',
	participants: '',
	TotalStageNumber: 0,
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
	TotalStageNumber: [
		{ required: true, message: '请输入总阶段数', trigger: 'change,blur' }
	],
	starttime: [
		{ required: true, message: '请选择开始时间', trigger: 'change,blur' }
	],
	endtime: [
		{ required: true, message: '请选择结束时间', trigger: 'change,blur' }
	],
	plantaskdescription: [
		{ required: true, message: '请输入计划/任务描述', trigger: 'change,blur' }
	],
	// ... existing rules ...
	// 'stages.*.name': [
	// 	{ required: true, message: '请输入阶段名称', trigger: 'blur' }
	// ],
	// 'stages.*.items.*.name': [
	// 	{ required: true, message: '请输入事项名称', trigger: 'blur' }
	// ],
	// 'stages.*.items.*.executor': [
	// 	{ required: true, message: '请选择执行人', trigger: 'blur' }
	// ],
	// 'stages.*.items.*.deadline': [
	// 	{ required: true, message: '请选择时间节点', trigger: 'blur' }
	// ]
})

//重置任务计划表单
const ResetPlanTaskForm = (formEl: FormInstance | undefined) => {
	if (!formEl) return
	formEl.resetFields()
	PlanTaskForm.plantaskname = ''
	PlanTaskForm.participants = ''
	PlanTaskForm.TotalStageNumber = 1
	PlanTaskForm.starttime = ''
	PlanTaskForm.endtime = ''
	PlanTaskForm.plantaskdescription = ''
	// 重置阶段为一个默认阶段
	stages.value = [{
		name: '',
		items: [{
			name: '',
			executor: '',
			deadline: ''
		}]
	}];
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
	getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
})
/*动态下拉框end*/

// 定义返回数据的接口
interface PlanTask {
	id: number;
	taskName: string;
	taskDescription: string;
	startTime: string;
	endTime: string;
	totalLeaderIds: string;
	create_by: string;
	create_time: string;
	completionRate: number;
}

interface PaginationData {
	pageSize: number;
	pageIndex: number;
	totalNum: number;
	totalPage: number;
	result: PlanTask[];
	extra: Record<string, any>;
}

//分页组件
const loading = ref(false);
const total = ref(0);
const queryParams = reactive({
	pageNum: 1,
	pageSize: 6,
	queryText: ''
});
const dataPlanTasks = ref<PlanTask[]>([]);
const handlePageChange = async (newPage: number) => {
	queryParams.pageNum = newPage;
	await getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
};

// 处理每页显示数量变化
const handleSizeChange = async (size: number) => {
	try {
		queryParams.pageSize = size;
		queryParams.pageNum = 1;  // 切换每页数量时重置为第一页
		await getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
	} catch (error) {
		console.error('切换每页显示数量失败:', error);
		ElMessage.error('操作失败，请稍后重试');
	}
};

const Search_PlanTaskName = ref('');
const Search_PlanTaskInfo = () => {
	queryParams.pageNum = 1; // 搜索时重置到第一页
	getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
}

// 获取计划任务列表
const getPlanTasksList = async (pageNum: number, pageSize: number) => {
	loading.value = true;
	try {
		const res = await request({
			url: 'PlanTasks/GetPlanTasksList/GetList',
			method: 'get',
			params: {
				pageNum: pageNum,
				pageSize: pageSize,
				TaskName: queryParams.queryText?.trim() || ''
			}
		});

		if (res.code === 200) {
			// 直接使用 res.data 作为数组
			dataPlanTasks.value = res.data || [];
			// 更新总数为数组长度
			total.value = res.data.length;

			// 转换用户ID为用户名
			dataPlanTasks.value.forEach(item => {
				const userIds = item.totalLeaderIds.split(',');
				const userNames = userIds.map(id => {
					const user = optionss.value.sql_all_user.find(u => u.dictValue === id);
					return user ? user.dictLabel : id;
				});
				item.totalLeaderIds = userNames.join(', ');
			});
		}
	} catch (error) {
		console.error('获取列表失败:', error);
		ElMessage.error('获取列表失败，请稍后重试');
	} finally {
		loading.value = false;
	}
};

</script>

<style scoped>
.mb-20 {
	margin-bottom: 20px;
}

.task-item-title {
	display: flex;
	align-items: center;
	gap: 10px;
}

.completion-dialog {
	display: flex;
	flex-direction: column;
	gap: 15px;
}

.el-timeline-item {
	margin-bottom: 20px;
}

.el-timeline-item__node--success {
	background-color: var(--el-color-success) !important;
}

.el-timeline-item__node--warning {
	background-color: var(--el-color-warning) !important;
}

.el-timeline-item__node--danger {
	background-color: var(--el-color-danger) !important;
}
</style>