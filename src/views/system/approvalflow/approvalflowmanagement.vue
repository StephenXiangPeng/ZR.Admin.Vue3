<template>
	<div>
		<div>
			<span style=" font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider> </el-divider>
		<el-button type="primary" @click="OpenApprovalProcessDialogClick">新增审批流</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;审批流程列表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="ApprovalProcessTableData">
			<el-table-column prop="processName" label="流程名称" width="250" align="center"></el-table-column>
			<el-table-column prop="documentType" label="单据类型" width="250" align="center"></el-table-column>
			<el-table-column prop="triggerCondition" label="触发条件" width="250" align="center"></el-table-column>
			<el-table-column prop="remark" label="备注信息" width="300" align="center"></el-table-column>
			<el-table-column prop="createTime" label="创建时间" width="200" align="center"></el-table-column>
			<el-table-column label="操作" align="center">
				<template v-slot="scope">
					<el-button link type="primary" size="small" @click="editProcess(scope.row)">编辑</el-button>
					<el-button link type="primary" size="small" @click="deleteProcess(scope.row)">删除</el-button>
					<el-button link type="primary" size="small" @click="viewDetails(scope.row)">查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="ApprovalProcessDialog" title="新增/编辑审批流" :close-on-click-modal=false style="width: 50%;"
			@close="CloseApprovalProcessDialog()">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="ApprovalProcessForm" label-width="120px">
				<el-row>
					<el-col :span="12">
						<el-form-item label="流程名称">
							<el-input v-model="ApprovalProcessForm.ProcessName" style="width: 250px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="单据类型">
							<el-select v-model="ApprovalProcessForm.DocumentType" filterable placeholder="选择单据类型"
								style="width: 250px;" @change="DocumentTypeChange()">
								<el-option v-for="dict in optionss.hr_approval_document_type" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
						</el-form-item>

					</el-col>
				</el-row>
				<el-row>
					<el-col :span="12">
						<el-form-item label="触发条件">
							<el-input v-model="ApprovalProcessForm.TriggerCondition" style="width: 250px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="备注信息">
							<el-input v-model="ApprovalProcessForm.Remark" style="width: 250px;"> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row v-show="IsSaleOrderShow">
					<el-col :span="12">
						<el-form-item label="美元/欧元">
							<el-select v-model="ApprovalProcessForm.CustomFields1operators" filterable
								placeholder="选择运算符" style="width: 60px;">
								<el-option v-for="dict in optionss.hr_operators" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
							<el-input v-model="ApprovalProcessForm.CustomFields1" style="width: 250px;"
								placeholder="请输入美元/欧元金额"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="人民币">
							<el-select v-model="ApprovalProcessForm.CustomFields2operators" filterable
								placeholder="选择运算符" style="width: 60px;">
								<el-option v-for="dict in optionss.hr_operators" :key="dict.dictCode"
									:label="dict.dictLabel" :value="dict.dictValue" />
							</el-select>
							<el-input v-model="ApprovalProcessForm.CustomFields2" style="width: 250px;"
								placeholder="请输入人民币金额">
							</el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">审批阶段</span>
			<el-divider></el-divider>
			<el-button class="mt-4" type="primary" @click="handleAddApprovalStageRow"
				style="margin-bottom: 10px;">添加审批阶段</el-button>
			<el-table :data="ApprovalStageTableData" style="width: 100%; " height="280">
				<el-table-column prop=" StageName" label="阶段名称" width="150">
					<template #default="{ row }">
						<el-input v-model="row.StageName" placeholder="输入阶段名称" size="small" disabled></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="ApproverRoleID" label="审批角色" width="150">
					<template #default="{ row }">
						<el-select v-model="row.ApproverRoleID" placeholder="选择审批角色" size="small">
							<el-option v-for="dict in optionss.sql_hr_allrole" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="Profitmargin" label="低于利润率(%)" width="150">
					<template #default="{ row }">
						<el-input v-model="row.Profitmargin" placeholder="输入利润率" size="small"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="备注" width="150">
					<template #default="{ row }">
						<el-input v-model="row.remark" placeholder="输入备注" size="small"></el-input>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="100">
					<template #default="scope">
						<el-button type="text" size="small" @click="handleDelete(scope.$index)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" v-if="isSaveBtnShow" @click="SaveApprovalProcess()">
						确定保存
					</el-button>
					<el-button type="primary" v-if="isEditSaveBtnShow" @click="EditApprovalProcess()">
						编辑保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<script setup lang="ts">
import { createApp, getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElMessageBox, ElMessage, UploadUserFile, UploadFile } from 'element-plus'
import { FOCUSABLE_CHILDREN } from 'element-plus/es/directives/trap-focus';
import request from '@/utils/request';

const IsSaleOrderShow = ref(false);

/*动态下拉框start*/
const proxy = getCurrentInstance().proxy
const state = reactive({
	optionss: {
		// 选项列表(动态字典将会从后台获取数据)
		hr_approval_document_type: [],
		sql_hr_allrole: [],
		hr_operators: []
	}
})
const { optionss } = toRefs(state)
var dictParams = [{ dictType: 'hr_approval_document_type' }, { dictType: 'sql_hr_allrole' }, { dictType: 'hr_operators' }]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
})
/*动态下拉框end*/


//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const NameText = ref('');
const handlePageChange = async (newPage) => {
	pageSize.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetApprovalProcessList(start, end);
};

GetApprovalProcessList(currentPage.value, pageSize.value);

function GetApprovalProcessList(start, end) {
	request({
		url: 'ApprovalFlow/GetApprovalProcessList/GetList',
		method: 'GET',
		params: {
			PageNum: start,
			PageSize: end
		}
	}).then(response => {
		if (response.data.result.length > 0) {
			ApprovalProcessTableData.value = response.data.result;
			ApprovalProcessTableData.value.forEach(element => {
				element.documentType = state.optionss.hr_approval_document_type.find(item => item.dictValue == element.documentType).dictLabel;
			});
		} else {
			ElMessage({
				message: '暂无数据',
				type: 'warning'
			});
		}
	}).catch(error => {
		console.error(error);
		ElMessage({
			message: '获取数据失败',
			type: 'error'
		});
	});
	// ApprovalProcessTableData.values = res.data.page.list;
	// totalItems.value = res.data.page.totalCount;
	// return res.data.page.list;
}


const ApprovalProcessTableData = ref([]);
const isSaveBtnShow = ref(true);
const isEditSaveBtnShow = ref(false);
const ApprovalProcessForm = reactive({
	ProcessID: 0,
	ProcessName: '',
	DocumentType: '',
	TriggerCondition: '',
	Remark: '',
	CustomFields1operators: null,
	CustomFields2operators: null,
	CustomFields3operators: null,
	CustomFields1: '',
	CustomFields2: '',
	CustomFields3: '',
	ApprovalStageItems: []
});


const ApprovalProcessDialog = ref(false);
const OpenApprovalProcessDialogClick = () => {
	ApprovalProcessDialog.value = true;
}
const CloseApprovalProcessDialog = () => {
	clearApprovalDialog();
	ApprovalProcessDialog.value = false;
}

const DocumentTypeChange = () => {
	if (ApprovalProcessForm.DocumentType == '1') {
		IsSaleOrderShow.value = true;
	} else {
		IsSaleOrderShow.value = false;
	}
}

const SaveApprovalProcess = () => {
	ApprovalProcessForm.ApprovalStageItems = ApprovalStageTableData.value;
	//新增
	request.post('ApprovalFlow/AddApprovalProcess/Add', ApprovalProcessForm).then(response => {
		if (response.msg == 'success') {
			ElMessage({
				message: response.data,
				type: 'success'
			})
			ApprovalProcessDialog.value = false;
			GetApprovalProcessList(currentPage.value, pageSize.value);
		} else {
			console.error('新增审批流程出错');
		}
	}).catch(error => {
		console.error('新增审批流程出错！😔错误内容：', error);
	})
}

const EditApprovalProcess = () => {
	ApprovalProcessForm.ApprovalStageItems = ApprovalStageTableData.value;
	request.post('ApprovalFlow/EditApprovalProcess/Edit', ApprovalProcessForm).then(response => {
		if (response.msg == 'success') {
			ElMessage({
				message: response.data,
				type: 'success'
			})
		} else {
			console.error('编辑审批流程出错');
		}
	}).catch(error => {
		console.error('编辑审批流程出错！😔错误内容：', error);
	})
}


const editProcess = (row) => {
	clearApprovalDialog();
	request({
		url: 'ApprovalFlow/GetApprovalProcessDetails/GetDetails',
		method: 'GET',
		params: {
			ProcessID: row.processID
		}
	}).then(response => {
		if (response.data != null) {
			ApprovalProcessForm.ProcessID = response.data.process.processID;
			ApprovalProcessForm.ProcessName = response.data.process.processName;
			ApprovalProcessForm.DocumentType = state.optionss.hr_approval_document_type.find(item => item.dictValue == response.data.process.documentType).dictValue;
			ApprovalProcessForm.TriggerCondition = response.data.process.triggerCondition;
			ApprovalProcessForm.Remark = response.data.process.remark;
			ApprovalProcessForm.CustomFields1 = response.data.process.customFields1;
			ApprovalProcessForm.CustomFields2 = response.data.process.customFields2;
			ApprovalProcessForm.CustomFields3 = response.data.process.customFields3;
			ApprovalProcessForm.CustomFields1operators = state.optionss.hr_operators.find(item => item.dictValue == response.data.process.customFields1Operators).dictValue;
			ApprovalProcessForm.CustomFields2operators = state.optionss.hr_operators.find(item => item.dictValue == response.data.process.customFields2Operators).dictValue;
			ApprovalProcessForm.CustomFields3operators = state.optionss.hr_operators.find(item => item.dictValue == response.data.process.customFields3Operators).dictValue;
			if (response.data.process.documentType == '1') {
				IsSaleOrderShow.value = true;
			} else {
				IsSaleOrderShow.value = false;
			}
			response.data.stageList.forEach(element => {
				const newRow = {
					processID: response.data.process.processID,
					StageID: element.stageID,
					StageName: element.stageName,
					ApproverRoleID: state.optionss.sql_hr_allrole.find(item => item.dictValue == element.approverRoleID).dictValue,
					Profitmargin: element.profitmargin,
					remark: element.remark
				};
				ApprovalStageTableData.value.push(newRow);
			});
		} else {
			ElMessage({
				message: '暂无数据',
				type: 'warning'
			});
		}
	}).catch(error => {
		console.error(error);
		ElMessage({
			message: '获取数据失败',
			type: 'error'
		});
	});
	ApprovalProcessDialog.value = true;
	isSaveBtnShow.value = false;
	isEditSaveBtnShow.value = true;
}
const deleteProcess = (row) => {
	alert(JSON.stringify(row.processID));
	ElMessageBox.confirm('此操作将永久删除该审批流程, 是否继续?', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request.get('ApprovalFlow/DeleteApprovalProcess/Delete', {
			params: {
				ProcessID: row.processID
			}
		}).then(response => {
			if (response.msg == 'success') {
				ElMessage({
					message: response.data,
					type: 'success'
				})
				GetApprovalProcessList(currentPage.value, pageSize.value);
			} else {
				console.error('删除审批流程出错');
			}
		}).catch(error => {
			console.error('删除审批流程出错！😔错误内容：', error);
		})
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除'
		});
	});
}

const viewDetails = (row) => {
	ElMessageBox.alert('流程名称：' + row.processName + '<br>' + '单据类型：' + row.documentType + '<br>' + '触发条件：' +
		row.triggerCondition + '<br>' + '备注信息：' + row.remark + '<br>' + '创建时间：' + row.createTime, '审批流程详情', {
		dangerouslyUseHTMLString: true
	});
}
//////////////////////////////审批阶段/////////////////////////////////////
const ApprovalStageTableData = ref([]);
const handleAddApprovalStageRow = () => {
	const newRow = {
		StageName: '阶段' + Number(ApprovalStageTableData.value.length + 1),
		ApproverRoleID: '',
		remark: ''
	};
	ApprovalStageTableData.value.push(newRow);
}
const handleDelete = (index) => {
	ApprovalStageTableData.value.splice(index, 1);
}

const clearApprovalDialog = () => {
	ApprovalProcessForm.ProcessID = 0;
	ApprovalProcessForm.ProcessName = '';
	ApprovalProcessForm.DocumentType = '';
	ApprovalProcessForm.TriggerCondition = '';
	ApprovalProcessForm.Remark = '';
	ApprovalProcessForm.CustomFields1 = '';
	ApprovalProcessForm.CustomFields2 = '';
	ApprovalProcessForm.CustomFields3 = '';
	ApprovalStageTableData.value = [];
	IsSaleOrderShow.value = false;
}
</script>