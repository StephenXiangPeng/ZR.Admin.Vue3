<template>
	<div>
		<!-- <el-form :model="queryParams" inline @submit.prevent ref="queryRef">
			<el-form-item prop="queryText">
				<el-input v-model="queryParams.queryText" placeholder="请输入计划任务名称" clearable @clear="Search_PlanTaskInfo"
					style="width: 600px" />
			</el-form-item>
			<el-form-item>
				<el-button type="primary" icon="search" @click="Search_PlanTaskInfo">{{ $t('btn.search') }}</el-button>
				<el-button type="primary" @click="OpenPlanTaskDialog">新建计划/任务</el-button>
			</el-form-item>
		</el-form> -->
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
								placeholder="请选择结束日期" style="width: 300px" @change="(value) => {
									if (!validateEndTime(value)) {
										PlanTaskForm.endtime = '';
									}
								}" />
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
				<!-- 添加图片上传组件 -->
				<el-row>
					<el-col :span="24">
						<el-form-item label="计划/任务图片">
							<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="planTaskImages"
								:limit="3" :disabled="planTaskImages.length >= 3" @change="handleImageChange"
								:action="UploadUrl" :data="formData">
								<el-icon>
									<Plus />
								</el-icon>
								<template #file="{ file }">
									<div>
										<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
										<span class="el-upload-list__item-actions">
											<span class="el-upload-list__item-preview"
												@click="handlePictureCardPreview(file)">
												<el-icon>
													<ZoomIn />
												</el-icon>
											</span>
											<span v-if="!disabled" class="el-upload-list__item-delete"
												@click="handleImageRemove(file)">
												<el-icon>
													<Delete />
												</el-icon>
											</span>
										</span>
									</div>
								</template>
							</el-upload>
							<el-dialog v-model="dialogVisible">
								<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
									:src="dialogImageUrl" alt="Preview Image" />
							</el-dialog>
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
										style="width: 300px"
										@change="handleExecutorChange(stageIndex, itemIndex, item.executor)">
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
										style="width: 300px" clearable multiple>
										<el-option v-for="dict in item.customerOptions || []" :key="dict.dictValue"
											:label="dict.dictLabel" :value="dict.dictValue">
										</el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item :label="'时间节点'"
									:prop="'stages.' + stageIndex + '.items.' + itemIndex + '.deadline'">
									<el-date-picker v-model="item.deadline" type="date" placeholder="选择时间节点"
										style="width: 300px" @change="(value) => {
											if (!value) return;
											if (!PlanTaskForm.starttime || !PlanTaskForm.endtime) {
												ElMessage.warning('请先设置任务的开始时间和结束时间');
												item.deadline = '';
												return;
											}
											const deadline = new Date(value);
											const startTime = new Date(PlanTaskForm.starttime);
											const endTime = new Date(PlanTaskForm.endtime);

											if (deadline < startTime) {
												ElMessage.warning(`第${stageIndex + 1}阶段第${itemIndex + 1}个事项的时间节点不能早于任务开始时间`);
												item.deadline = '';
											} else if (deadline > endTime) {
												ElMessage.warning(`第${stageIndex + 1}阶段第${itemIndex + 1}个事项的时间节点不能超过任务结束时间`);
												item.deadline = '';
											}
										}" />
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
					<!-- <el-button type="primary" @click="SubmitPlanTaskForm(PlanTaskFormRef, false)">
						保存
					</el-button>
					<el-button type="primary" @click="SubmitPlanTaskForm(PlanTaskFormRef, true)">
						编辑
					</el-button> -->
					<el-button type="warning" @click="SubmitPlanTaskForm(PlanTaskFormRef, true)">
						保存草稿
					</el-button>
					<el-button type="success" @click="SubmitPlanTaskForm(PlanTaskFormRef, false)">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- <span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;计划/任务</span> -->
		<!-- <el-divider></el-divider> -->
		<div>
			<el-form :model="queryParams" inline @submit.prevent ref="queryRef">
				<el-form-item>
					<el-radio-group v-model="PlanTaskStatusRadio" size="large">
						<el-radio-button label="进行中" value="0" />
						<el-radio-button label="已完成" value="1" />
						<el-radio-button label="已作废" value="2" />
					</el-radio-group>
				</el-form-item>
				<el-form-item prop="queryText">
					<el-input v-model="queryParams.queryText" placeholder="请输入计划任务名称" clearable
						@clear="Search_PlanTaskInfo" style="width: 600px" />
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="search" @click="Search_PlanTaskInfo">{{ $t('btn.search')
					}}</el-button>
					<el-button type="primary" @click="OpenPlanTaskDialog">新建计划/任务</el-button>
				</el-form-item>
			</el-form>
		</div>
		<el-divider></el-divider>
		<!-- 任务列表 -->
		<el-table v-loading="loading" :data="dataPlanTasks" style="width: 100%" border>
			<el-table-column prop="id" label="任务ID" min-width="120" v-if="false">
			</el-table-column>
			<el-table-column prop="create_time" label="创建日期" min-width="120">
				<template #default="{ row }">
					<span>{{ formatDate(row.create_time) }}</span>
					<el-tag v-if="row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="taskName" label="计划任务名称" min-width="180">
				<template #default="{ row }">
					<el-tooltip :content="row.taskName" placement="top" :show-after="200">
						<div class="truncated-text">{{ row.taskName }}</div>
					</el-tooltip>
				</template>
			</el-table-column>
			<el-table-column prop="totalLeaderIds" label="总负责人" min-width="120" />
			<el-table-column prop="taskDescription" label="任务描述" min-width="200">
				<template #default="{ row }">
					<el-tooltip :content="row.taskDescription" placement="top" :show-after="200">
						<div class="truncated-text">{{ row.taskDescription }}</div>
					</el-tooltip>
				</template>
			</el-table-column>
			<el-table-column label="当前进度" min-width="150">
				<template #default="{ row }">
					<el-progress :text-inside="true" :stroke-width="24" :percentage="row.completionRate"
						:status="row.completionRate >= 1 ? 'success' : ''" />
				</template>
			</el-table-column>
			<el-table-column label="时间范围" min-width="200">
				<template #default="{ row }">
					{{ formatDate(row.startTime) }} 至 {{ formatDate(row.endTime) }}
				</template>
			</el-table-column>
			<!-- <el-table-column label="附件" min-width="100">
				<template #default="{ row }">
					<div class="task-attachments">
						<el-button v-if="row.attachmentUrls" type="primary" link @click="showAttachmentsDialog(row)">
							<el-icon>
								<Document />
							</el-icon>
							查看附件
						</el-button>
						<span v-else>无</span>
					</div>
				</template>
			</el-table-column> -->
			<el-table-column label="操作" width="180" fixed="right">
				<template #default="{ row }">
					<el-button type="primary" link @click="showTaskDetail(row)">
						查看详情
					</el-button>
					<el-button v-if="isCurrentUserTotalLeader(row.totalLeaderIds) && row.planTaskStatus !== 2"
						type="danger" link @click="InvalidTask(row)">
						作废
					</el-button>
				</template>
			</el-table-column>
		</el-table>

		<!-- 分页 -->
		<el-pagination v-model:current-page="queryParams.pageNum" v-model:page-size="queryParams.pageSize"
			:total="total" :page-sizes="[10, 20, 30, 40]" layout="total, sizes, prev, pager, next, jumper"
			@size-change="handleSizeChange" @current-change="handlePageChange" />

		<!-- 任务详情对话框 -->
		<el-dialog v-model="taskDetailDialogVisible" :title="selectedTask?.taskName || '任务详情'" width="70%">
			<template v-if="selectedTask">
				<!-- 添加水印 -->
				<div v-if="selectedTask.planTaskStatus === 2" class="watermark">
					<div class="watermark-text">已作废</div>
				</div>
				<el-descriptions :column="2" border>
					<el-descriptions-item label="任务名称">{{ selectedTask.taskName }}</el-descriptions-item>
					<el-descriptions-item label="总负责人">{{ selectedTask.totalLeaderIds }}</el-descriptions-item>
					<el-descriptions-item label="开始时间">{{ formatDate(selectedTask.startTime) }}</el-descriptions-item>
					<el-descriptions-item label="结束时间">{{ formatDate(selectedTask.endTime) }}</el-descriptions-item>
					<el-descriptions-item label="任务描述" :span="2">{{ selectedTask.taskDescription
					}}</el-descriptions-item>
					<el-descriptions-item label="创建时间">{{ formatDate(selectedTask.create_time) }}</el-descriptions-item>
					<el-descriptions-item label="完成进度">
						<el-progress :percentage="selectedTask.completionRate"
							:status="selectedTask.completionRate >= 100 ? 'success' : ''" :stroke-width="15"
							:text-inside="true" />
					</el-descriptions-item>
					<el-descriptions-item v-if="selectedTask.imageUrls" label="任务图片" :span="2">
						<div class="task-images">
							<div v-for="(url, index) in selectedTask.imageUrls.split(',')" :key="index"
								class="image-item">
								<img :src="url" @click="handleTaskImagePreview(url)" class="preview-image" />
							</div>
						</div>
					</el-descriptions-item>
					<el-descriptions-item v-if="selectedTask.attachmentUrls" label="附件" :span="2">
						<div class="attachments-list">
							<div v-for="(url, urlIndex) in selectedTask.attachmentUrls.split(',')" :key="urlIndex"
								class="attachment-item">
								<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
								<el-button type="primary" link @click="downloadFile(url, getFileNameFromUrl(url))">
									<el-icon>
										<Download />
									</el-icon>下载
								</el-button>
							</div>
						</div>
					</el-descriptions-item>
				</el-descriptions>

				<!-- 阶段和事项列表 -->
				<div v-if="selectedTaskPhases && selectedTaskPhases.length" class="phases-container">
					<div v-for="(phaseData, phaseIndex) in selectedTaskPhases" :key="phaseData.phase.id"
						class="phase-item">
						<el-divider>第{{ phaseIndex + 1 }}阶段：{{ phaseData.phase.phaseName }}</el-divider>
						<el-table :data="phaseData.items" border style="width: 100%">
							<el-table-column prop="itemName" label="事项名称" min-width="150" />
							<el-table-column prop="executorId" label="执行人" min-width="120">
								<template #default="{ row }">
									{{ getUserName(row.executorId) }}
								</template>
							</el-table-column>
							<el-table-column prop="relatedCustomers" label="关联客户" min-width="150">
								<template #default="{ row }">
									{{ getCustomerName(row.relatedCustomers) }}
								</template>
							</el-table-column>
							<el-table-column prop="timePoint" label="计划完成时间" min-width="150">
								<template #default="{ row }">
									{{ formatDate(row.timePoint) }}
								</template>
							</el-table-column>
							<el-table-column prop="realTimePoint" label="实际完成时间" min-width="150">
								<template #default="{ row }">
									{{ row.realTimePoint ? formatDate(row.realTimePoint) : '未完成' }}
								</template>
							</el-table-column>
							<el-table-column label="状态" width="100">
								<template #default="{ row }">
									<el-tag :type="row.realTimePoint ? 'success' : 'warning'">
										{{ row.realTimePoint ? '已完成' : '进行中' }}
									</el-tag>
								</template>
							</el-table-column>
							<el-table-column label="操作" width="130" fixed="right">
								<template #default="{ row }">
									<el-button
										v-if="isCurrentUserExecutor(row.executorId) && !row.realTimePoint && !selectedTask?.isDraft && selectedTask?.planTaskStatus !== 2"
										type="success" size="default" @click="handleTaskComplete(row)">
										确认完成
									</el-button>
									<el-button
										v-if="!isCurrentUserExecutor(row.executorId) && !row.realTimePoint && !selectedTask?.isDraft && selectedTask?.planTaskStatus !== 2"
										type="warning" size="default" @click="handleTaskExpediting(row)">
										催办
									</el-button>
									<el-button v-if="row.realTimePoint" type="primary" link
										@click="showCompletionDetails(row)">
										查看完成详情
									</el-button>
								</template>
							</el-table-column>
						</el-table>

						<!-- 事项附件列表 -->
						<template v-for="(item, itemIndex) in phaseData.items" :key="item.id">
							<div v-if="item.attachmentUrls" class="item-attachments">
								<el-collapse>
									<el-collapse-item :title="'事项附件：' + item.itemName">
										<div class="attachments-list">
											<div v-for="(url, urlIndex) in item.attachmentUrls.split(',')"
												:key="urlIndex" class="attachment-item">
												<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
												<el-button type="primary" link
													@click="downloadFile(url, getFileNameFromUrl(url))">
													<el-icon>
														<Download />
													</el-icon>下载
												</el-button>
											</div>
										</div>
									</el-collapse-item>
								</el-collapse>
							</div>
						</template>
					</div>
				</div>
				<el-empty v-else description="暂无阶段数据" />
			</template>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-if="selectedTask?.isDraft" type="primary" @click="handleEditTask">编辑</el-button>
					<el-button @click="taskDetailDialogVisible = false">关闭</el-button>
				</span>
			</template>
		</el-dialog>

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

		<!-- 添加完成详情对话框 -->
		<el-dialog v-model="completionDetailsDialogVisible"
			:title="`完成详情 - ${currentCompletionDetails?.phaseName || '未知阶段'} - ${currentCompletionDetails?.itemName || ''}`"
			width="50%">
			<template v-if="currentCompletionDetails">
				<el-descriptions :column="2" border>
					<!-- <el-descriptions-item label="所属阶段">
						{{ currentCompletionDetails.phaseName || '未知阶段' }}
					</el-descriptions-item>
					<el-descriptions-item label="事项名称">
						{{ currentCompletionDetails.itemName }}
					</el-descriptions-item> -->
					<el-descriptions-item label="执行人">
						{{ getUserName(currentCompletionDetails.executorId) }}
					</el-descriptions-item>
					<el-descriptions-item label="关联客户">
						{{ getCustomerName(currentCompletionDetails.relatedCustomers) }}
					</el-descriptions-item>
					<el-descriptions-item label="计划完成时间">
						{{ formatDate(currentCompletionDetails.timePoint) }}
					</el-descriptions-item>
					<el-descriptions-item label="实际完成时间">
						{{ formatDate(currentCompletionDetails.realTimePoint) }}
					</el-descriptions-item>
					<el-descriptions-item label="完成备注" :span="2">
						{{ currentCompletionDetails.remark || '无' }}
					</el-descriptions-item>
					<el-descriptions-item label="完成附件" :span="2">
						<div v-if="currentCompletionDetails.finishattachmentUrls && currentCompletionDetails.finishattachmentUrls.trim()"
							class="attachments-list">
							<div v-for="(url, index) in currentCompletionDetails.finishattachmentUrls.split(',')"
								:key="index" class="attachment-item">
								<span class="file-name">{{ getFileNameFromUrl(url) }}</span>
								<el-button type="primary" link @click="downloadFile(url, getFileNameFromUrl(url))">
									<el-icon>
										<Download />
									</el-icon>下载
								</el-button>
							</div>
						</div>
						<span v-else>无</span>
					</el-descriptions-item>
				</el-descriptions>
			</template>
		</el-dialog>

		<!-- 添加任务图片预览对话框 -->
		<el-dialog v-model="taskImagePreviewVisible" title="图片预览" width="50%">
			<img :src="previewImageUrl" style="max-width: 100%; max-height: 100%; width: auto; height: auto;" />
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { reactive, ref, getCurrentInstance, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import {
	ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElMessage, DrawerProps, ElDrawer,
	ElMessageBox, ElTimeline, ElTimelineItem, FormInstance, FormRules, ElLoading, ElRadioGroup, ElRadioButton
} from 'element-plus'
import { Document, Download, Plus, Delete, ZoomIn } from '@element-plus/icons-vue'
import request from '@/utils/request';
import useUserStore from '@/store/modules/user'

const PlanTaskStatusRadio = ref('0');
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
	// 如果是编辑模式，保留原有的文件列表
	if (isEditMode.value && selectedTask.value?.attachmentUrls) {
		const existingFiles = selectedTask.value.attachmentUrls.split(',').map(url => ({
			name: getFileNameFromUrl(url),
			url: getRelativePathFromUrl(url),
			isExisting: true
		}));
		mainTaskFileList.value = [...existingFiles, ...fileList.filter(f => !f.url && !f.isExisting)];
	} else {
		mainTaskFileList.value = fileList;
	}
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

	// 如果是编辑模式，保留原有的文件列表
	if (isEditMode.value && selectedTaskPhases.value) {
		const phase = selectedTaskPhases.value[stageIndex];
		if (phase?.items?.[itemIndex]?.attachmentUrls) {
			const existingFiles = phase.items[itemIndex].attachmentUrls.split(',').map(url => ({
				name: getFileNameFromUrl(url),
				url: getRelativePathFromUrl(url),
				isExisting: true
			}));
			stages.value[stageIndex].items[itemIndex].fileList = [
				...existingFiles,
				...fileList.filter(f => !f.url && !f.isExisting)
			];
		} else {
			stages.value[stageIndex].items[itemIndex].fileList = fileList;
		}
	} else {
		stages.value[stageIndex].items[itemIndex].fileList = fileList;
	}
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

	// 如果是编辑模式，更新 selectedTaskPhases 中的 attachmentUrls
	if (isEditMode.value && selectedTaskPhases.value) {
		const phase = selectedTaskPhases.value[stageIndex]
		if (phase && phase.items && phase.items[itemIndex]) {
			const remainingUrls = fileList
				.filter(f => f.url)
				.map(f => f.url)
				.join(',')
			phase.items[itemIndex].attachmentUrls = remainingUrls
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

	// 分离已有URL和需要上传的新文件
	const existingUrls = files
		.filter(file => file.isExisting)
		.map(file => file.url)

	const newFiles = files.filter(file => !file.isExisting && !file.url)

	// 只上传新文件
	const uploadPromises = newFiles.map(file => uploadSingleFile(file))
	const uploadResults = await Promise.all(uploadPromises)

	// 合并已有URL和新上传的URL
	const allUrls = [
		...existingUrls,
		...uploadResults.filter(url => url !== null)
	]

	return allUrls.join(',')
}

// 获取用户名的辅助函数
const getUserName = (executorId) => {
	const user = optionss.value.sql_all_user.find(u => u.dictValue === executorId.toString());
	return user ? user.dictLabel : executorId;
}

// 修改获取客户名称的辅助函数
const getCustomerName = (customerIds) => {
	if (!customerIds) return '无';
	const ids = customerIds.split(',');
	const names = ids.map(id => {
		const customer = optionss.value.sql_hr_customer.find(c => c.dictValue === id);
		return customer ? customer.dictLabel : id;
	});
	return names.join(', ') || '无';
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

const handleTaskExpediting = async (task) => {
	if (!task || !task.id) {
		ElMessage.error('无效的任务信息');
		return;
	}
	try {
		const loadingInstance = ElLoading.service({
			lock: true,
			text: '正在发送催办通知...',
			background: 'rgba(0, 0, 0, 0.7)'
		});

		try {
			const res = await request.get(`PlanTasks/ExpeditingTaskItem/Expediting`, {
				params: {
					ID: task.id
				}
			}) as unknown as { data: ApiResponse };

			if (res.code === 200) {
				ElMessage.success(res.msg || '催办通知发送成功');
			} else {
				ElMessage.error(res.msg || '催办失败');
			}
		} finally {
			loadingInstance.close();
		}
	} catch (error) {
		console.error('催办失败:', error);
		ElMessage.error('催办操作失败，请稍后重试');
	}
}

// 处理任务完成附件变更
const handleCompletionFileChange = (file, fileList) => {
	completionFileList.value = fileList;
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

			console.log('发送的完成确认数据:', requestData); // 添加日志

			// 3. 发送请求
			const res = await request.get(`PlanTasks/ConfirmationOfCompletion/ConfirmItem`, {
				params: requestData
			}) as unknown as { data: ApiResponse };

			if (res.code === 200) {
				ElMessage.success('任务完成确认成功');
				// 刷新任务详情
				await showTaskDetail(selectedTask.value);
				// 获取最新的任务信息并更新进度
				const taskRes = await request.get(`PlanTasks/GetPlanTaskDetails/GetDetails?ID=${selectedTask.value.id}`) as unknown as { data: ApiResponse };
				if (taskRes.data.code === 200) {
					selectedTaskPhases.value = taskRes.data.data;
					// 更新主任务的完成进度
					const completedItems = selectedTaskPhases.value.reduce((total, phase) => {
						return total + phase.items.filter(item => item.realTimePoint).length;
					}, 0);
					const totalItems = selectedTaskPhases.value.reduce((total, phase) => {
						return total + phase.items.length;
					}, 0);
					selectedTask.value.completionRate = totalItems > 0 ? (completedItems / totalItems) * 100 : 0;
				}
				// 刷新列表
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
	// 处理总负责人显示
	const leaderIds = task.totalLeaderIds.split(',');
	task.totalLeaderIds = leaderIds.map(id => {
		const user = optionss.value.sql_all_user.find(u => u.dictValue === id);
		return user ? user.dictLabel : id;
	}).join(', ');

	selectedTask.value = task;
	taskDetailDialogVisible.value = true;
	try {
		const res = await request.get(`PlanTasks/GetPlanTaskDetails/GetDetails?ID=${task.id}`) as unknown as { data: ApiResponse };
		console.log('任务详情原始响应:', res);

		if (res.data.code === 200) {
			// 直接使用 res.data.data 作为数组，因为数据结构已经正确
			selectedTaskPhases.value = res.data.data;
			console.log('处理后的任务详情数据:', selectedTaskPhases.value);
		}
	} catch (error) {
		console.error('获取任务详情失败:', error);
		ElMessage.error('获取任务详情失败');
		selectedTaskPhases.value = []; // 出错时清空数据
	}
}

// 从URL中提取文件名并清理敏感信息
const getFileNameFromUrl = (url) => {
	if (!url) return '';
	const parts = url.split('/');
	let fileName = parts[parts.length - 1];
	if (fileName.includes('?')) {
		fileName = fileName.split('?')[0];
	}
	// 解码URL编码的文件名
	try {
		return decodeURIComponent(fileName);
	} catch (error) {
		console.error('解码文件名失败:', error);
		return fileName;
	}
}

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

// 修改下载文件函数
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

const InvalidTask = (row) => {
	ElMessageBox.confirm('确定作废该任务吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		const res = await request({
			url: 'PlanTasks/InvalidPlanTaskItemByID/InvalidPlanTask',
			method: 'get',
			params: {
				ID: row.id
			}
		})
		if (res.code === 200) {
			ElMessage.success('作废成功')
			getPlanTasksList(queryParams.pageNum, queryParams.pageSize)
		} else {
			ElMessage.error(res.msg || '作废失败')
		}
	})
}

const taskDetailDialogVisible = ref(false);

// 处理编辑任务
const handleEditTask = async () => {
	// 关闭详情对话框
	taskDetailDialogVisible.value = false;

	// 打开新建对话框
	PlanTaskDialogVisible.value = true;
	isEditMode.value = true; // 设置为编辑模式

	// 填充表单数据
	PlanTaskForm.plantaskname = selectedTask.value.taskName;
	// 将显示的用户名转换回用户ID
	const userIds = selectedTask.value.totalLeaderIds.split(',').map(name => {
		const user = optionss.value.sql_all_user.find(u => u.dictLabel === name.trim());
		return user ? user.dictValue : '';
	}).filter(id => id); // 过滤掉空值
	PlanTaskForm.participants = userIds;

	PlanTaskForm.starttime = selectedTask.value.startTime;
	PlanTaskForm.endtime = selectedTask.value.endTime;
	PlanTaskForm.plantaskdescription = selectedTask.value.taskDescription;

	// 如果有图片，设置图片列表
	if (selectedTask.value.imageUrls) {
		planTaskImages.value = selectedTask.value.imageUrls.split(',').map(url => ({
			name: getFileNameFromUrl(url),
			url: url,
			isExisting: true
		}));
	}

	// 设置阶段数据
	stages.value = await Promise.all(selectedTaskPhases.value.map(async phaseData => {
		const items = await Promise.all(phaseData.items.map(async item => {
			// 如果执行人ID不为空且不为0，则加载该执行人的客户数据
			let customerOptions = [];
			if (item.executorId && item.executorId !== 0) {
				customerOptions = await getsalePersonCustomerData(item.executorId.toString());
			}

			return {
				name: item.itemName,
				executor: item.executorId == 0 ? '' : item.executorId.toString(),
				customer: item.relatedCustomers ? item.relatedCustomers.split(',') : [], // 将逗号分隔的字符串转换为数组
				deadline: item.timePoint,
				fileList: item.attachmentUrls ? item.attachmentUrls.split(',').map(url => ({
					name: getFileNameFromUrl(url),
					url: getRelativePathFromUrl(url),
					isExisting: true
				})) : [],
				customerOptions: customerOptions // 保存客户选项到事项中
			};
		}));

		return {
			name: phaseData.phase.phaseName,
			items: items
		};
	}));

	// 设置总阶段数
	PlanTaskForm.TotalStageNumber = stages.value.length;

	// 如果有主任务附件，设置主任务附件列表
	if (selectedTask.value.attachmentUrls) {
		mainTaskFileList.value = selectedTask.value.attachmentUrls.split(',').map(url => ({
			name: getFileNameFromUrl(url),
			url: getRelativePathFromUrl(url),
			isExisting: true
		}));
	}
}

// 检查当前用户是否是总负责人
const isCurrentUserTotalLeader = (totalLeaderIds: string) => {
	console.log(totalLeaderIds, 'totalLeaderIds', currentUser.value, 'currentUser.value')
	if (!totalLeaderIds) return false;
	const leaderIds = totalLeaderIds.split(',').map(id => id.trim());
	const user = optionss.value.sql_all_user.find(u => u.dictValue === currentUser.value.toString());
	return leaderIds.includes(user?.dictLabel || '');
}

// 修改获取相对路径的函数
const getRelativePathFromUrl = (url: string) => {
	if (!url) return '';
	try {
		const urlObj = new URL(url);
		// 获取路径部分，去掉查询参数
		const path = urlObj.pathname;
		// 去掉域名部分，只保留 PlanTask/Attachments/... 部分
		const relativePath = path.substring(path.indexOf('PlanTask/Attachments/'));
		// 解码相对路径
		return decodeURIComponent(relativePath);
	} catch (error) {
		// 如果URL解析失败，检查是否已经是相对路径
		if (url.includes('PlanTask/Attachments/')) {
			return decodeURIComponent(url);
		}
		console.error('解析URL失败:', error);
		return url;
	}
}

// 检查当前用户是否是事项执行人
const isCurrentUserExecutor = (executorId: string | number) => {
	return executorId?.toString() === currentUser.value?.toString();
}

// 添加新的响应式变量
const completionDetailsDialogVisible = ref(false);
const currentCompletionDetails = ref(null);

// 修改显示完成详情的方法
const showCompletionDetails = (row) => {
	console.log('完成详情数据:', row); // 添加日志
	// 从任务详情中找到对应的阶段和事项
	const phase = selectedTaskPhases.value.find(phase =>
		phase.items.some(item => item.id === row.id)
	);
	const item = phase?.items.find(item => item.id === row.id);

	currentCompletionDetails.value = {
		...row,
		phaseName: phase?.phase.phaseName || '未知阶段',
		itemName: item?.itemName || row.itemName,
		executorId: item?.executorId || row.executorId,
		relatedCustomers: item?.relatedCustomers || row.relatedCustomers,
		timePoint: item?.timePoint || row.timePoint,
		realTimePoint: row.realTimePoint,
		remark: row.remark || '',
		finishattachmentUrls: row.finishattachmentUrls || ''
	};
	completionDetailsDialogVisible.value = true;
};

// 修改获取用户相关的客户数据的函数
const getsalePersonCustomerData = async (salesPersonID: string) => {
	if (!salesPersonID || salesPersonID === '0') {
		return [];
	}

	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerDataBysalesPersonID/GetSelectCustomerDataBysalesPersonID',
			method: 'get',
			params: {
				salesPersonID: salesPersonID
			}
		})

		if (response.code === 200) {
			return response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel
			}));
		} else {
			ElMessage.error(response.msg || '获取客户数据失败')
			return [];
		}
	} catch (error) {
		console.error('获取客户数据失败:', error)
		ElMessage.error('获取客户数据失败')
		return [];
	}
}

// 修改事项执行人变更处理函数
const handleExecutorChange = async (stageIndex: number, itemIndex: number, executorId: string) => {
	// 清空当前事项的客户选择
	stages.value[stageIndex].items[itemIndex].customer = '';
	// 加载新的执行人的客户数据并保存到事项中
	stages.value[stageIndex].items[itemIndex].customerOptions = await getsalePersonCustomerData(executorId);
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
	pageSize: 10,
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
				TaskName: queryParams.queryText?.trim() || '',
				PlanTaskStatus: PlanTaskStatusRadio.value
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

// 添加图片上传相关的响应式变量
const planTaskImages = ref([])
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)
const formData = { filePath: 'planTask' }

// 处理图片变更
const handleImageChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 3) {
		ElMessage({
			type: 'info',
			message: '最多上传3张图片!'
		});
		fileList.splice(3); // 保留前三个文件，移除其余文件
		return;
	}
	// 如果是编辑模式，保留已存在的图片
	if (isEditMode.value) {
		const existingImages = planTaskImages.value.filter(img => img.isExisting);
		planTaskImages.value = [...existingImages, ...fileList.filter(f => !f.isExisting)];
	} else {
		planTaskImages.value = fileList;
	}
}

// 处理图片预览
const handlePictureCardPreview = (file) => {
	dialogImageUrl.value = file.url
	dialogVisible.value = true
}

// 处理图片删除
const handleImageRemove = (file) => {
	ElMessageBox.confirm('确定删除该图片吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		if (isEditMode.value && file.isExisting) {
			// 如果是编辑模式且是已存在的图片，标记为删除
			file.isDeleted = true;
			const index = planTaskImages.value.findIndex(f => f.uid === file.uid);
			if (index !== -1) {
				planTaskImages.value.splice(index, 1);
			}
		} else {
			// 如果是新上传的图片，直接删除
			const index = planTaskImages.value.findIndex(f => f.uid === file.uid);
			if (index !== -1) {
				planTaskImages.value.splice(index, 1);
			}
		}
		ElMessage({
			type: 'success',
			message: '删除成功!',
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除',
		});
	});
}

// 提交任务计划表单
const SubmitPlanTaskForm = async (formEl: FormInstance | undefined, isDraft: boolean = false) => {
	if (!formEl) return
	await formEl.validate(async (valid, fields) => {
		if (!isDraft && !valid) {
			console.log('表单验证失败:', fields);
			ElMessage.warning('请填写完整的表单信息');
			return;
		}

		try {
			const loadingInstance = ElLoading.service({
				lock: true,
				text: '正在上传附件并保存...',
				background: 'rgba(0, 0, 0, 0.7)'
			})

			try {
				// 1. 处理主任务图片
				let mainTaskImageUrls = [];
				if (isEditMode.value) {
					// 编辑模式：保留未删除的已存在图片，上传新图片
					const existingImages = planTaskImages.value
						.filter(img => img.isExisting && !img.isDeleted)
						.map(img => img.url);

					const newImages = planTaskImages.value
						.filter(img => !img.isExisting && !img.isDeleted)
						.map(file => file.raw || file);

					if (newImages.length > 0) {
						const newUrls = await Promise.all(newImages.map(async (file) => {
							const formData = new FormData();
							formData.append('FileName', file.name);
							formData.append('FileDir', 'PlanTask/Images');
							formData.append('FileNameType', '1');
							formData.append('File', file);
							formData.append('storeType', '1');
							const response = await request.postForm(UploadUrl, formData);
							return response.data.url;
						}));
						mainTaskImageUrls = [...existingImages, ...newUrls];
					} else {
						mainTaskImageUrls = existingImages;
					}
				} else {
					// 新建模式：上传所有图片
					if (planTaskImages.value.length > 0) {
						mainTaskImageUrls = await Promise.all(planTaskImages.value.map(async (file) => {
							const formData = new FormData();
							formData.append('FileName', file.name);
							formData.append('FileDir', 'PlanTask/Images');
							formData.append('FileNameType', '1');
							formData.append('File', file.raw || file);
							formData.append('storeType', '1');
							const response = await request.postForm(UploadUrl, formData);
							return response.data.url;
						}));
					}
				}

				// 2. 处理主任务附件
				let mainTaskUrlsStr = '';
				if (isEditMode.value) {
					// 获取现有附件（已经是相对路径）
					const existingUrls = mainTaskFileList.value
						.filter(f => f.isExisting)
						.map(f => f.url)
						.join(',');

					// 获取新上传的文件
					const newFiles = mainTaskFileList.value.filter(file => !file.isExisting && !file.url);
					if (newFiles.length > 0) {
						const newUrls = await uploadFilesAndGetUrlString(newFiles);
						// 确保新上传的文件也转换为相对路径
						const newRelativeUrls = newUrls.split(',').map(url => getRelativePathFromUrl(url)).join(',');
						mainTaskUrlsStr = existingUrls ? `${existingUrls},${newRelativeUrls}` : newRelativeUrls;
					} else {
						mainTaskUrlsStr = existingUrls;
					}
				} else {
					const uploadedUrls = await uploadFilesAndGetUrlString(mainTaskFileList.value);
					mainTaskUrlsStr = uploadedUrls.split(',').map(url => getRelativePathFromUrl(url)).join(',');
				}

				// 3. 构建请求数据
				const requestData = {
					id: isEditMode.value ? selectedTask.value.id : 0,
					taskName: PlanTaskForm.plantaskname?.trim() || '',
					taskDescription: PlanTaskForm.plantaskdescription?.trim() || '',
					startTime: PlanTaskForm.starttime ? new Date(PlanTaskForm.starttime).toISOString() : new Date().toISOString(),
					endTime: PlanTaskForm.endtime ? new Date(PlanTaskForm.endtime).toISOString() : new Date().toISOString(),
					totalLeaderIds: Array.isArray(PlanTaskForm.participants)
						? PlanTaskForm.participants.join(',')
						: PlanTaskForm.participants || '',
					attachmentUrls: mainTaskUrlsStr,
					imageUrls: mainTaskImageUrls.join(','), // 添加图片URL
					planTask_Phases: [],
					isDraft: isDraft ? 1 : 0
				};

				// 4. 处理各阶段和事项
				for (let stageIndex = 0; stageIndex < stages.value.length; stageIndex++) {
					const stage = stages.value[stageIndex]
					const phaseData = {
						id: isEditMode.value && selectedTaskPhases.value[stageIndex]?.phase?.id || 0,
						phaseName: stage.name?.trim() || '',
						planTask_Items: []
					}

					for (let itemIndex = 0; itemIndex < stage.items.length; itemIndex++) {
						const item = stage.items[itemIndex];
						let itemUrlsStr = '';

						if (isEditMode.value && selectedTaskPhases.value[stageIndex]?.items[itemIndex]) {
							// 获取现有附件（已经是相对路径）
							const existingUrls = (item.fileList || [])
								.filter(f => f.isExisting)
								.map(f => f.url)
								.join(',');

							// 获取新上传的文件
							const newFiles = (item.fileList || []).filter(file => !file.isExisting && !file.url);
							if (newFiles.length > 0) {
								const newUrls = await uploadFilesAndGetUrlString(newFiles);
								// 确保新上传的文件也转换为相对路径
								const newRelativeUrls = newUrls.split(',').map(url => getRelativePathFromUrl(url)).join(',');
								itemUrlsStr = existingUrls ? `${existingUrls},${newRelativeUrls}` : newRelativeUrls;
							} else {
								itemUrlsStr = existingUrls;
							}
						} else {
							const uploadedUrls = await uploadFilesAndGetUrlString(item.fileList || []);
							itemUrlsStr = uploadedUrls.split(',').map(url => getRelativePathFromUrl(url)).join(',');
						}

						phaseData.planTask_Items.push({
							id: isEditMode.value && selectedTaskPhases.value[stageIndex]?.items[itemIndex]?.id || 0,
							itemName: item.name?.trim() || '',
							executorId: Number(item.executor) || 0,
							timePoint: item.deadline ? new Date(item.deadline).toISOString() : null,
							attachmentUrls: itemUrlsStr,
							finishattachmentUrls: '',
							relatedCustomers: Array.isArray(item.customer) ? item.customer.join(',') : item.customer || null // 将数组转换为逗号分隔的字符串
						})
					}
					requestData.planTask_Phases.push(phaseData)
				}

				// 验证关键字段（仅在非草稿模式下）
				if (!isDraft && !requestData.taskName) {
					ElMessage.error('任务名称不能为空');
					return;
				}

				console.log('发送的数据:', JSON.stringify(requestData, null, 2));

				// 根据是否是编辑模式选择不同的API端点
				const apiEndpoint = isEditMode.value ? 'PlanTasks/EditPlanTasks/Edit' : 'PlanTasks/AddPlanTasks/Add';
				const res = await request.post(apiEndpoint, requestData) as unknown as { data: ApiResponse };

				if (res.code === 200) {
					ElMessage.success(isDraft ? '草稿保存成功' : '保存成功');
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
	})
}

const PlanTaskDialogVisible = ref(false);
const isEditMode = ref(false); // 添加编辑模式标识

const OpenPlanTaskDialog = () => {
	// 添加检查，确保 PlanTaskFormRef.value 存在
	if (PlanTaskFormRef.value) {
		PlanTaskFormRef.value.resetFields()
	}
	PlanTaskDialogVisible.value = true;
	isEditMode.value = false; // 重置为新建模式
	// 确保打开对话框时有一个默认阶段
	if (stages.value.length === 0) {
		stages.value = [{
			name: '',
			items: [{
				name: '',
				executor: '',
				customer: '',
				deadline: '',
				fileList: [], // 初始化文件列表
				customerOptions: [] // 初始化客户选项
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
	customerOptions?: any[]; // 添加客户选项属性
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
		fileList: [], // 初始化文件列表
		customerOptions: [] // 初始化客户选项
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
					fileList: [], // 初始化文件列表
					customerOptions: [] // 初始化客户选项
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
		fileList: [], // 初始化文件列表
		customerOptions: [] // 初始化客户选项
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
		{ required: true, message: '请选择结束时间', trigger: 'change,blur' },
		{
			validator: (rule, value, callback) => {
				if (value && PlanTaskForm.starttime && new Date(value) < new Date(PlanTaskForm.starttime)) {
					callback(new Error('结束时间不能小于开始时间'));
				} else {
					callback();
				}
			},
			trigger: 'change,blur'
		}
	],
	plantaskdescription: [
		{ required: true, message: '请输入计划/任务描述', trigger: 'change,blur' }
	],
	'stages.*.items.*.deadline': [
		{
			validator: (rule, value, callback) => {
				if (!value) {
					callback();
					return;
				}
				if (!PlanTaskForm.endtime) {
					callback();
					return;
				}

				const deadlineDate = new Date(value);
				const endTimeDate = new Date(PlanTaskForm.endtime);

				if (deadlineDate > endTimeDate) {
					callback(new Error('时间节点不能超过任务结束时间'));
				} else {
					callback();
				}
			},
			trigger: 'change,blur'
		}
	]
});

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
	// 重置图片列表
	planTaskImages.value = []
	// 重置阶段为一个默认阶段
	stages.value = [{
		name: '',
		items: [{
			name: '',
			executor: '',
			customer: '',
			deadline: '',
			fileList: [],
			customerOptions: []
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

// 在 script setup 部分添加新的响应式变量和函数
const taskImagePreviewVisible = ref(false)
const previewImageUrl = ref('')

// 添加任务图片预览处理函数
const handleTaskImagePreview = (url) => {
	previewImageUrl.value = url
	taskImagePreviewVisible.value = true
}

// 添加时间验证函数
const validateEndTime = (value) => {
	if (!value || !PlanTaskForm.starttime) return true;
	const endTime = new Date(value);
	const startTime = new Date(PlanTaskForm.starttime);
	if (endTime < startTime) {
		ElMessage.warning('结束时间不能小于开始时间');
		return false;
	}
	return true;
};

const validateItemDeadline = (value, stageIndex, itemIndex) => {
	if (!value || !PlanTaskForm.endtime) return true;
	const deadline = new Date(value);
	const endTime = new Date(PlanTaskForm.endtime);
	if (deadline > endTime) {
		ElMessage.warning(`第${stageIndex + 1}阶段第${itemIndex + 1}个事项的时间节点不能超过任务结束时间`);
		return false;
	}
	return true;
};

// 修改事项时间节点变更处理函数
const handleItemDeadlineChange = (value, stageIndex, itemIndex) => {
	if (!validateItemDeadline(value, stageIndex, itemIndex)) {
		// 如果验证失败，清空时间节点
		stages.value[stageIndex].items[itemIndex].deadline = '';
	}
};

// 监听状态切换
watch(PlanTaskStatusRadio, (newValue) => {
	getPlanTasksList(queryParams.pageNum, queryParams.pageSize);
});

const route = useRoute()

// 在组件挂载时检查是否有taskId参数
onMounted(async () => {
	const taskId = route.query.taskId
	if (taskId) {
		// 先获取任务列表
		await getPlanTasksList(queryParams.pageNum, queryParams.pageSize)
		// 查找对应的任务
		const task = dataPlanTasks.value.find(t => t.id.toString() === taskId.toString())
		if (task) {
			showTaskDetail(task)
		}
	}
})
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

.phases-container {
	margin-top: 20px;
}

.phase-item {
	margin-bottom: 30px;
}

.item-attachments {
	margin-top: 15px;
}

.attachments-list {
	margin-top: 10px;
}

.attachment-item {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 8px;
	background-color: #f5f7fa;
	margin-bottom: 5px;
	border-radius: 4px;
}

.file-name {
	flex: 1;
	margin-right: 10px;
	word-break: break-all;
}

.task-images {
	display: flex;
	flex-wrap: wrap;
	gap: 10px;
	margin-top: 10px;
}

.image-item {
	width: 120px;
	height: 120px;
	border-radius: 4px;
	overflow: hidden;
	cursor: pointer;
	transition: transform 0.3s;
}

.image-item:hover {
	transform: scale(1.05);
}

.preview-image {
	width: 100%;
	height: 100%;
	object-fit: cover;
}

.watermark {
	position: fixed;
	/* 改为 fixed 定位 */
	top: 50%;
	left: 50%;
	transform: translate(-50%, -50%);
	/* 居中定位 */
	width: 100%;
	height: 100%;
	pointer-events: none;
	z-index: 9999;
	/* 提高 z-index 确保在最上层 */
	display: flex;
	justify-content: center;
	align-items: center;
}

.watermark-text {
	font-size: 120px;
	color: rgba(0, 0, 0, 0.1);
	transform: rotate(-45deg);
	white-space: nowrap;
	user-select: none;
	position: relative;
	/* 添加相对定位 */
	z-index: 9999;
	/* 确保文字也在最上层 */
}

/* 添加一个相对定位的容器来确保水印的正确定位 */
.el-dialog {
	position: relative;
	overflow: hidden;
}
</style>