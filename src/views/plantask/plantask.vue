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
				<!-- 添加主任务附件上传 -->
				<el-row>
					<el-col :span="24">
						<el-form-item label="附件上传" prop="attachments">
							<el-upload action="#" :auto-upload="false" :on-change="handleMainTaskFileChange"
								:on-remove="handleMainTaskFileRemove" :file-list="mainTaskFileList" multiple
								style="width: 785px">
								<el-button type="primary">选择文件</el-button>
								<template #tip>
									<div class="el-upload__tip">
										支持任意类型文件
									</div>
								</template>
							</el-upload>
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
										style="width: 300px" @change="getsalePersonCustomerData(item.executor)">
										<el-option v-for="dict in optionss.sql_all_user" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue">
										</el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="12">
								<el-form-item :label="'关联客户'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.customer'">
									<el-select v-model="item.customer" filterable placeholder="选择关联客户"
										style="width: 300px" clearable>
										<el-option v-for="dict in optionss.salePersonCustomer" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue">
										</el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item :label="'时间节点'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.deadline'">
									<el-date-picker v-model="item.deadline" type="date" placeholder="选择时间节点"
										style="width: 300px" />
								</el-form-item>
							</el-col>
						</el-row>
						<!-- 添加事项附件上传 -->
						<el-row>
							<el-col :span="12">
								<el-form-item :label="'附件上传'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.attachments'">
									<el-upload action="#" :auto-upload="false"
										:on-change="(file, fileList) => handleItemFileChange(stageIndex, itemIndex, file, fileList)"
										:on-remove="(file) => handleItemFileRemove(stageIndex, itemIndex, file)"
										:file-list="item.fileList || []" multiple style="width: 300px">
										<el-button type="primary">选择文件</el-button>
										<template #tip>
											<div class="el-upload__tip">
												支持任意类型文件
											</div>
										</template>
									</el-upload>
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
				<el-card v-loading="loading" :body-style="{ padding: '15px 15px 0', height: '360px' }" class="mb-20">
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
							<el-tooltip :content="item.taskDescription" placement="top" :show-after="200">
								<div class="truncated-text">{{ item.taskDescription }}</div>
							</el-tooltip>
						</el-descriptions-item>
						<el-descriptions-item label="创建日期">
							{{ formatDate(item.create_time) }}
						</el-descriptions-item>
						<el-descriptions-item label="当前进度">
							<el-progress :text-inside="true" :stroke-width="24" :percentage="item.completionRate"
								:status="item.completionRate >= 1 ? 'success' : ''" />
						</el-descriptions-item>
						<!-- 添加附件列表 -->
						<el-descriptions-item label="附件">
							<div class="task-attachments">
								<el-button v-if="item.attachmentUrls" type="primary" link
									@click="showAttachmentsDialog(item)">
									<el-icon>
										<Document />
									</el-icon>
									查看附件
								</el-button>
								<span v-else>无</span>
							</div>
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
								<p>关联客户: {{ getCustomerName(item?.relatedCustomers) }}</p>
								<p>计划完成时间: {{ formatDate(item?.timePoint) }}</p>
								<p>{{ item?.realTimePoint ? `实际完成时间: ${formatDate(item?.realTimePoint)}` : '状态: 未完成' }}
								</p>
								<p>{{ item?.remark ? `备注: ${item?.remark}` : '' }}</p>
								<!-- 显示附件列表 -->
								<div class="item-attachments">
									<p class="attachments-title">附件:</p>
									<ul v-if="item?.attachmentUrls" class="attachments-list">
										<li v-for="(url, index) in item.attachmentUrls.split(',')" :key="index"
											class="attachment-item">
											<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
											<el-button type="primary" link
												@click="downloadFile(url, getFileNameFromUrl(url))">
												<el-icon>
													<Download />
												</el-icon>
												下载
											</el-button>
										</li>
									</ul>
									<span v-else>无</span>
								</div>
								<!-- 显示完成附件列表 -->
								<div class="item-attachments">
									<p class="attachments-title">完成附件:</p>
									<ul v-if="item?.finishattachmentUrls" class="attachments-list">
										<li v-for="(url, index) in item.finishattachmentUrls.split(',')" :key="index"
											class="attachment-item">
											<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
											<el-button type="primary" link
												@click="downloadFile(url, getFileNameFromUrl(url))">
												<el-icon>
													<Download />
												</el-icon>
												下载
											</el-button>
										</li>
									</ul>
									<span v-else>无</span>
								</div>
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
				<!-- 添加附件上传组件 -->
				<div class="completion-attachments">
					<p class="upload-label">上传附件（选填）：</p>
					<el-upload action="#" :auto-upload="false" :on-change="handleCompletionFileChange"
						:on-remove="handleCompletionFileRemove" :file-list="completionFileList" multiple
						style="width: 100%">
						<el-button type="primary">选择文件</el-button>
						<template #tip>
							<div class="el-upload__tip">
								支持任意类型文件
							</div>
						</template>
					</el-upload>
				</div>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancelTaskCompletion">取消</el-button>
					<el-button type="primary" @click="confirmTaskCompletion">
						确认完成
					</el-button>
				</span>
			</template>
		</el-dialog>

		<!-- 附件查看对话框 -->
		<el-dialog v-model="attachmentsDialogVisible" title="附件列表" width="40%">
			<div class="attachments-dialog">
				<el-empty v-if="!currentAttachments || currentAttachments.length === 0" description="无附件" />
				<ul v-else class="attachments-list">
					<li v-for="(url, index) in currentAttachments" :key="index" class="attachment-item">
						<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
						<el-button type="primary" link @click="downloadFile(url, getFileNameFromUrl(url))">
							<el-icon>
								<Download />
							</el-icon>
							下载
						</el-button>
					</li>
				</ul>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="attachmentsDialogVisible = false">关闭</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { reactive, ref, getCurrentInstance } from 'vue'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElMessage, DrawerProps, ElDrawer,
	ElMessageBox, ElTimeline, ElTimelineItem, FormInstance, FormRules, ElLoading
} from 'element-plus'
import { Document, Download } from '@element-plus/icons-vue'
import request from '@/utils/request';
import useUserStore from '@/store/modules/user'


// 定义API响应类型
interface ApiResponse<T = any> {
	code: number;
	msg: string;
	data: T;
}

const userStore = useUserStore()
const currentUser = ref(userStore.userId)
// 添加确认对话框相关的响应式变量
const confirmDialogVisible = ref(false)
const currentTask = ref(null)
const completionNote = ref('')
// 添加任务完成附件相关变量
const completionFileList = ref([])

// 附件对话框相关变量
const attachmentsDialogVisible = ref(false)
const currentAttachments = ref<string[]>([])

// 文件上传相关
const mainTaskFileList = ref([])
const handleMainTaskFileChange = (file, fileList) => {
	mainTaskFileList.value = fileList
}
const handleMainTaskFileRemove = (file) => {
	const index = mainTaskFileList.value.indexOf(file)
	if (index !== -1) {
		mainTaskFileList.value.splice(index, 1)
	}
}

// 处理事项文件变更
const handleItemFileChange = (stageIndex, itemIndex, file, fileList) => {
	if (!stages.value[stageIndex].items[itemIndex].fileList) {
		stages.value[stageIndex].items[itemIndex].fileList = []
	}
	stages.value[stageIndex].items[itemIndex].fileList = fileList
}

// 处理事项文件移除
const handleItemFileRemove = (stageIndex, itemIndex, file) => {
	const fileList = stages.value[stageIndex].items[itemIndex].fileList
	if (fileList) {
		const index = fileList.indexOf(file)
		if (index !== -1) {
			fileList.splice(index, 1)
		}
	}
}

// 将文件转换为FormData附件格式
const prepareFileForUpload = (file) => {
	return {
		fileName: file.name,
		fileType: file.type || getFileExtension(file.name),
		fileSize: file.size,
		fileUrl: file.url || '' // 添加URL字段
	}
}

// 获取文件扩展名
const getFileExtension = (filename) => {
	return filename.slice((filename.lastIndexOf(".") - 1 >>> 0) + 2) || ''
}

// 定义上传URL
const UploadUrl = 'Common/UploadFile' // 根据实际API调整

// 上传单个文件并返回URL
const uploadSingleFile = async (file) => {
	try {
		const formData = new FormData()
		formData.append('FileName', file.name)
		formData.append('FileDir', 'PlanTask/Attachments')
		formData.append('FileNameType', '1')
		formData.append('File', file.raw || file)
		formData.append('storeType', '2')

		const response = await request.postForm(UploadUrl, formData) as unknown as { data: ApiResponse };

		if (response.code === 200) {
			return response.data.downloadurl
		} else {
			console.error('文件上传失败:', file.name)
			ElMessage.warning(`文件 ${file.name} 上传失败`)
			return null
		}
	} catch (error) {
		console.error('文件上传错误:', error)
		ElMessage.error(`文件 ${file.name} 上传出错: ${error.message || '未知错误'}`)
		return null
	}
}

// 批量上传文件并返回URL字符串（逗号分隔）
const uploadFilesAndGetUrlString = async (files) => {
	if (!files || files.length === 0) return ''

	const uploadPromises = files.map(file => uploadSingleFile(file))
	const results = await Promise.all(uploadPromises)

	// 过滤掉上传失败的文件，并用逗号连接URL
	return results.filter(downloadurl => downloadurl !== null).join(',')
}

// 获取用户名的辅助函数
const getUserName = (executorId) => {
	const user = optionss.value.sql_all_user.find(u => u.dictValue === executorId.toString());
	return user ? user.dictLabel : executorId;
}

const getCustomerName = (customerId) => {
	const customer = optionss.value.sql_hr_customer.find(c => c.dictValue === customerId.toString());
	return customer == null ? '无' : customer.dictLabel;
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

// 处理任务完成勾选
const handleTaskComplete = (task) => {
	currentTask.value = task
	confirmDialogVisible.value = true
}

// 确认完成任务
const confirmTaskCompletion = async () => {
	try {
		// 显示加载状态
		const loadingInstance = ElLoading.service({
			lock: true,
			text: '正在上传附件并保存...',
			background: 'rgba(0, 0, 0, 0.7)'
		})

		try {
			// 1. 先上传附件并获取URL字符串
			const attachmentUrlsStr = await uploadFilesAndGetUrlString(completionFileList.value)

			// 2. 构建请求数据
			const requestData = {
				ID: currentTask.value.id,
				remark: completionNote.value || '',
				finishattachmentUrls: attachmentUrlsStr // 使用字符串格式的URL
			}

			// 3. 发送请求
			const res = await request.get(`PlanTasks/ConfirmationOfCompletion/ConfirmItem`, {
				params: requestData
			}) as unknown as { data: ApiResponse };

			if (res.code === 200) {
				ElMessage.success('任务完成确认成功');
				// 刷新任务详情
				await showTaskDetail(selectedTask.value);
				await getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
			} else {
				ElMessage.error(res.data.msg || '确认失败');
			}
		} finally {
			// 关闭加载状态
			loadingInstance.close()

			// 重置表单
			confirmDialogVisible.value = false;
			completionNote.value = '';
			completionFileList.value = [];
			currentTask.value = null;
		}
	} catch (error) {
		console.error('确认失败:', error);
		ElMessage.error('确认失败：' + (error.message || '未知错误'));
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
		const res = await request.get(`PlanTasks/GetPlanTaskDetails/GetDetails?ID=${task.id}`) as unknown as { data: ApiResponse };
		console.log('任务详情原始响应:', res); // 添加调试日志

		if (res.data.code === 200) {
			// 处理嵌套的响应结构
			const responseData = res.data.data;
			console.log('任务详情数据结构:', responseData); // 添加调试日志

			// 确保数据存在且是数组
			selectedTaskPhases.value = Array.isArray(responseData) ? responseData :
				(responseData && responseData.data && Array.isArray(responseData.data)) ? responseData.data : [];

			console.log('处理后的任务详情数据:', selectedTaskPhases.value); // 用于调试
		}
	} catch (error) {
		console.error('获取任务详情失败:', error);
		ElMessage.error('获取任务详情失败');
		selectedTaskPhases.value = []; // 出错时清空数据
	}
	drawer.value = true;
}

// 从URL中提取文件名并清理敏感信息
const getFileNameFromUrl = (url) => {
	if (!url) return '';

	// 从URL中提取文件名
	const parts = url.split('/');
	let fileName = parts[parts.length - 1];

	// 处理文件名中的查询参数
	if (fileName.includes('?')) {
		// 只保留问号前面的部分
		fileName = fileName.split('?')[0];
	}

	// 处理URL编码
	fileName = decodeURIComponent(fileName);

	// 处理特殊字符，如括号等
	fileName = fileName.replace(/%28/g, '(').replace(/%29/g, ')').replace(/%20/g, ' ');

	return fileName;
}

// 处理任务完成附件变更
const handleCompletionFileChange = (file, fileList) => {
	completionFileList.value = fileList
}

// 处理任务完成附件移除
const handleCompletionFileRemove = (file) => {
	const index = completionFileList.value.indexOf(file)
	if (index !== -1) {
		completionFileList.value.splice(index, 1)
	}
}

// 取消任务完成
const cancelTaskCompletion = () => {
	confirmDialogVisible.value = false
	completionNote.value = ''
	completionFileList.value = []
	currentTask.value = null
}

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

// 提交任务计划表单
const SubmitPlanTaskForm = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate(async (valid, fields) => {
		if (valid) {
			try {
				// 显示加载状态
				const loadingInstance = ElLoading.service({
					lock: true,
					text: '正在上传附件并保存...',
					background: 'rgba(0, 0, 0, 0.7)'
				})

				try {
					// 1. 先上传主任务附件并获取URL字符串
					const mainTaskUrlsStr = await uploadFilesAndGetUrlString(mainTaskFileList.value)

					// 2. 构建请求数据，使用与后端 JsonPropertyName 匹配的命名
					const requestData = {
						taskName: PlanTaskForm.plantaskname.trim(),
						taskDescription: PlanTaskForm.plantaskdescription?.trim() || '',
						startTime: PlanTaskForm.starttime ? new Date(PlanTaskForm.starttime).toISOString() : new Date().toISOString(),
						endTime: PlanTaskForm.endtime ? new Date(PlanTaskForm.endtime).toISOString() : new Date().toISOString(),
						totalLeaderIds: Array.isArray(PlanTaskForm.participants)
							? PlanTaskForm.participants.join(',')
							: PlanTaskForm.participants || '',
						attachmentUrls: mainTaskUrlsStr, // 使用字符串格式的URL
						planTask_Phases: []
					};
					// 3. 处理各阶段和事项
					for (let stageIndex = 0; stageIndex < stages.value.length; stageIndex++) {
						const stage = stages.value[stageIndex]
						const phaseData = {
							phaseName: stage.name?.trim() || '',
							planTask_Items: []
						}
						for (let itemIndex = 0; itemIndex < stage.items.length; itemIndex++) {
							const item = stage.items[itemIndex]
							// 上传该事项的附件
							const itemUrlsStr = await uploadFilesAndGetUrlString(item.fileList || [])

							phaseData.planTask_Items.push({
								itemName: item.name?.trim() || '',
								executorId: Number(item.executor) || 0,
								timePoint: item.deadline ? new Date(item.deadline).toISOString() : null,
								attachmentUrls: itemUrlsStr, // 使用字符串格式的URL
								finishattachmentUrls: '', // 初始为空，完成时才会有值
								relatedCustomers: item.customer || null//关联客户
							})
						}
						requestData.planTask_Phases.push(phaseData)
					}
					// 验证关键字段
					if (!requestData.taskName) {
						ElMessage.error('任务名称不能为空');
						return;
					}
					console.log('发送的数据:', JSON.stringify(requestData, null, 2));
					const res = await request.post('PlanTasks/AddPlanTasks/Add', requestData) as unknown as { data: ApiResponse };
					if (res.code === 200) {
						ElMessage.success(res.data);
						PlanTaskDialogVisible.value = false;
						ResetPlanTaskForm(formEl);
						getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
					} else {
						ElMessage.error(res.data.msg || '保存失败');
					}
				} finally {
					// 关闭加载状态
					loadingInstance.close()
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
	// 添加检查，确保 PlanTaskFormRef.value 存在
	if (PlanTaskFormRef.value) {
		PlanTaskFormRef.value.resetFields()
	}
	PlanTaskDialogVisible.value = true;
	// 确保打开对话框时有一个默认阶段
	if (stages.value.length === 0) {
		stages.value = [{
			name: '',
			items: [{
				name: '',
				executor: '',
				customer: '',
				deadline: ''
			}]
		}];
	}
}

interface StageItem {
	name: string;
	executor: string;
	customer: string;
	deadline: string;
	fileList?: any[]; // 添加文件列表属性
	attachments?: { fileName: string; fileUrl: string }[]; // 添加附件属性
	finishAttachments?: { fileName: string; fileUrl: string }[]; // 添加完成附件属性
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
		customer: '',
		deadline: '',
		fileList: [] // 初始化文件列表
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
					customer: '',
					deadline: '',
					fileList: [] // 初始化文件列表
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
		customer: '',
		deadline: '',
		fileList: [] // 初始化文件列表
	});
}

const removeItem = (stageIndex: number, itemIndex: number) => {
	stages.value[stageIndex].items.splice(itemIndex, 1);
}

//任务计划表单
interface PlanTaskForm {
	plantaskname: string,
	participants: string,
	TotalStageNumber: number,
	starttime: string,
	endtime: string,
	plantaskdescription: string
}

const PlanTaskFormRef = ref<FormInstance>()
const PlanTaskForm = reactive<PlanTaskForm>({
	plantaskname: '',
	participants: '',
	TotalStageNumber: 1,
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
	]
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
	// 重置文件列表
	mainTaskFileList.value = []
	// 重置阶段为一个默认阶段
	stages.value = [{
		name: '',
		items: [{
			name: '',
			executor: '',
			customer: '',
			deadline: '',
			fileList: [] // 重置文件列表
		}]
	}];
}

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy as any
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		sql_all_user: [],
		salePersonCustomer: [],
		sql_hr_customer: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'sql_all_user' }, { dictType: 'sql_hr_customer' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	});
	getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
})
/*动态下拉框end*/

// 获取用户相关的客户数据
const getsalePersonCustomerData = async (salesPersonID: string) => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerDataBysalesPersonID/GetSelectCustomerDataBysalesPersonID',
			method: 'get',
			params: {
				salesPersonID: salesPersonID
			}
		})

		if (response.code === 200) {
			state.optionss.salePersonCustomer = response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel
			}))
		} else {
			ElMessage.error(response.msg || '获取客户数据失败')
		}
	} catch (error) {
		console.error('获取客户数据失败:', error)
		ElMessage.error('获取客户数据失败')
	}
}

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
	attachmentUrls?: string; // 添加附件URL字段
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
		}) as unknown as { data: ApiResponse<PlanTask[]> };

		if (res.code === 200) {
			// 直接使用 res.data.data 作为数组
			dataPlanTasks.value = res.data.result || [];
			// 更新总数为数组长度
			total.value = res.data.totalNum;

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

// 显示附件对话框
const showAttachmentsDialog = (item) => {
	if (item.attachmentUrls) {
		// 将逗号分隔的URL字符串转换为数组
		currentAttachments.value = item.attachmentUrls.split(',').filter(url => url.trim() !== '')
		attachmentsDialogVisible.value = true
	} else {
		ElMessage.info('该任务没有附件')
	}
}

// 下载文件
const downloadFile = (url: string, fileName: string) => {
	try {
		// 创建一个隐藏的a标签
		const link = document.createElement('a')
		link.href = url
		link.download = fileName
		link.target = '_blank'

		// 添加到文档中并触发点击
		document.body.appendChild(link)
		link.click()

		// 清理
		document.body.removeChild(link)
	} catch (error) {
		console.error('下载文件失败:', error)
		ElMessage.error('下载文件失败，请稍后重试')
	}
}

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

.completion-attachments {
	margin-top: 10px;
}

.upload-label {
	margin-bottom: 8px;
	font-weight: 500;
}

.item-attachments {
	margin-top: 10px;
	border-top: 1px dashed #e0e0e0;
	padding-top: 8px;
}

.attachments-title {
	font-weight: 500;
	margin-bottom: 5px;
}

.attachments-list {
	list-style: none;
	padding: 0;
	margin: 0;
}

.attachment-item {
	margin-bottom: 5px;
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 5px 10px;
	border-radius: 4px;
	background-color: #f5f7fa;
}

.attachment-item:hover {
	background-color: #ecf5ff;
}

.file-name {
	font-size: 14px;
	color: #606266;
	word-break: break-all;
	max-width: 70%;
}

.attachment-item .el-icon {
	margin-right: 5px;
}

.task-attachments {
	display: flex;
	align-items: center;
}

.attachments-dialog {
	min-height: 100px;
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

.truncated-text {
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
	max-width: 200px;
	/* Adjust as needed */
}
</style>