<template>
	<div>
		<el-container style="height: 100%;">
			<el-aside width="10%" style="height: 100%;">
				<el-menu style="height: 100%;" default-active="1" class="el-menu-vertical-demo" v-if="EmailMenuShow"
					@select="handleSelect">
					<div style="text-align: center;">
						<el-button color="#626aef" size="large" icon="Edit" :round="true"
							style="width: calc(75%);font-size: 20px;" @click="opensendEmaildialog(0)">写邮件</el-button>
					</div>
					<el-menu-item index="1">
						<template #title>
							<el-icon>
								<Message />
							</el-icon>
							<span>收件箱</span>
						</template>
					</el-menu-item>
					<el-menu-item index="2">
						<el-icon>
							<Position />
						</el-icon>
						<span>已发送</span>
					</el-menu-item>
					<el-menu-item index="3">
						<el-icon>
							<document />
						</el-icon>
						<span>草稿箱</span>
					</el-menu-item>
					<el-menu-item index="4">
						<el-icon>
							<Delete />
						</el-icon>
						<span>垃圾箱</span>
					</el-menu-item>
					<el-sub-menu index="5">
						<template #title>
							<el-icon>
								<CollectionTag />
							</el-icon>
							<span>标签分类邮件</span>
						</template>
						<el-menu-item v-for="(item, index) in UserEmailTagList" :key="index" :index="`5-${item.id}`">
							{{ item.emailTagName }}
						</el-menu-item>
					</el-sub-menu>
					<el-menu-item index="6">
						<el-icon>
							<Tools />
						</el-icon>
						<span @click="openUserEmailConfigDialog">邮箱配置</span>
					</el-menu-item>
				</el-menu>
			</el-aside>
			<el-main style=" height: 100%;">
				<el-row style="height: 100%;">
					<el-col :span="10" style="width: 40%; border-right: 1px solid rgb(219, 208, 208);">
						<template v-if="selectedMenu === '1'">
							<el-scrollbar v-if="EmailInboxArray != null" height="750px">
								<div v-for="(item, index) in EmailInboxArray     " :key="index">
									<el-card :class="{ 'selected-card': selectedIndex === index }" shadow="hover"
										style="max-width: 100%; height: 100px; max-height: 100px; border: 1 ;margin: 5px; "
										@click="EmailCardClick(index)">
										<div>
											<div
												style="width: 60%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;float: left;font-size:15px;">
												来自：{{ item.fromEmailAddress }}</div>
											<div style="float: right;font-size:15px;">时间：{{ item.emaildate }}</div>
										</div>
										<div>
											<div
												style="width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  float: left; font-size: 15px;">
												主题：{{ item.emailsubject }}
											</div>
										</div>
										<div v-if="item.emailTagNames !== '' && item.emailTagNames !== null"
											v-for="(     Tagitem, index     ) in item.emailTagNames.split(',')     ">
											<div
												style="text-align: center; width: 60px; height: 20px;  float:left;font-size:13px; background-color: rgb(89, 177, 158);border-radius: 3px; color: white; margin-top: 3px; margin-bottom: 3px; margin-right: 3px;">
												{{
													Tagitem }}</div>
										</div>
									</el-card>
								</div>
								<el-pagination @current-change="handlePageChange" :current-page="currentPage"
									:page-size="pageSize" :total="totalItems" background layout="prev, pager, next"
									style="margin-top: 5px;" />
							</el-scrollbar>
							<el-scrollbar v-else height="750px">
								<el-divider>
									无数据
								</el-divider>
							</el-scrollbar>
						</template>
						<template v-else-if="selectedMenu === '2'">
							<el-scrollbar v-if="EmailInboxArray != null" height="750px">
								<div v-for="(     item, index     ) in EmailInboxArray     " :key="index">
									<el-card :class="{ 'selected-card': selectedIndex === index }" shadow="hover"
										style="max-width: 100%; height: 100px; max-height: 100px; border: 1;margin: 5px; "
										@click="EmailCardClick(index)">
										<div>
											<div
												style="width: 60%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;float: left;font-size:15px;">
												来自：{{ item.fromEmailAddress }}</div>
											<div style="float: right;font-size:15px;">时间：{{ item.emaildate }}</div>
										</div>
										<div>
											<div
												style="width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  float: left; font-size: 15px;">
												主题：{{ item.emailsubject }}
											</div>
										</div>
										<div v-if="item.emailTagNames !== '' && item.emailTagNames !== null"
											v-for="(     Tagitem, index     ) in item.emailTagNames.split(',')     ">
											<div
												style="text-align: center; width: 60px; height: 20px;  float:left;font-size:13px; background-color: rgb(89, 177, 158);border-radius: 3px; color: white; margin-top: 3px; margin-bottom: 3px; margin-right: 3px;">
												{{
													Tagitem }}</div>
										</div>
									</el-card>
								</div>
								<el-pagination @current-change="handlePageChange" :current-page="currentPage"
									:page-size="pageSize" :total="totalItems" background layout="prev, pager, next"
									style="margin-top: 5px;" />
							</el-scrollbar>
							<el-scrollbar v-else height="750px">
								<el-divider>
									无数据
								</el-divider>
							</el-scrollbar>
						</template>
						<template v-else-if="selectedMenu === '3'">
							<el-scrollbar v-if="EmailInboxArray != null" height="750px">
								<div v-for="(     item, index     ) in EmailInboxArray     " :key="index">
									<el-card :class="{ 'selected-card': selectedIndex === index }" shadow="hover"
										style="max-width: 100%; height: 100px; max-height: 100px; border: 1;margin: 5px;"
										@click="EmailCardClick(index)">
										<div>
											<div
												style="width: 60%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;float: left;font-size:15px;">
												来自：{{ item.fromEmailAddress }}</div>
											<div style="float: right;font-size:15px;">时间：{{ item.emaildate }}</div>
										</div>
										<div>
											<div
												style="width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  float: left; font-size: 15px;">
												主题：{{ item.emailsubject }}
											</div>
										</div>
										<div v-if="item.emailTagNames !== '' && item.emailTagNames !== null"
											v-for="(     Tagitem, index     ) in item.emailTagNames.split(',')     ">
											<div
												style="text-align: center; width: 60px; height: 20px;  float:left;font-size:13px; background-color: rgb(89, 177, 158);border-radius: 3px; color: white; margin-top: 3px; margin-bottom: 3px; margin-right: 3px;">
												{{
													Tagitem }}</div>
										</div>
									</el-card>
								</div>
								<el-pagination @current-change="handlePageChange" :current-page="currentPage"
									:page-size="pageSize" :total="totalItems" background layout="prev, pager, next"
									style="margin-top: 5px;" />
							</el-scrollbar>
							<el-scrollbar v-else height="750px">
								<el-divider>
									无数据
								</el-divider>
							</el-scrollbar>
						</template>
						<template v-else-if="selectedMenu === '4'">
							<el-scrollbar v-if="EmailInboxArray != null" height="750px">
								<div v-for="(     item, index     ) in EmailInboxArray     " :key="index">
									<el-card :class="{ 'selected-card': selectedIndex === index }" shadow="hover"
										style="max-width: 100%; height: 100px; max-height: 100px; border: 1;margin: 5px;"
										@click="EmailCardClick(index)">
										<div>
											<div
												style="width: 60%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;float: left;font-size:15px;">
												来自：{{ item.fromEmailAddress }}</div>
											<div style="float: right;font-size:15px;">时间：{{ item.emaildate }}</div>
										</div>
										<div>
											<div
												style="width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  float: left; font-size: 15px;">
												主题：{{ item.emailsubject }}
											</div>
										</div>
										<div v-if="item.emailTagNames !== '' && item.emailTagNames !== null"
											v-for="(     Tagitem, index     ) in item.emailTagNames.split(',')     ">
											<div
												style="text-align: center; width: 60px; height: 20px;  float:left;font-size:13px; background-color: rgb(89, 177, 158);border-radius: 3px; color: white; margin-top: 3px; margin-bottom: 3px; margin-right: 3px;">
												{{
													Tagitem }}</div>
										</div>
									</el-card>
								</div>
								<el-pagination @current-change="handlePageChange" :current-page="currentPage"
									:page-size="pageSize" :total="totalItems" background layout="prev, pager, next"
									style="margin-top: 5px;" />
							</el-scrollbar>
							<el-scrollbar v-else height="750px">
								<el-divider>
									无数据
								</el-divider>
							</el-scrollbar>
						</template>
						<template v-else-if="selectedMenu === '5'">
							<el-scrollbar v-if="EmailInboxArray != null" height="750px">
								<div v-for="(     item, index     ) in EmailInboxArray     " :key="index">
									<el-card :class="{ 'selected-card': selectedIndex === index }" shadow="hover"
										style="max-width: 100%; height: 100px; max-height: 100px; border: 1;margin: 5px;"
										@click="EmailCardClick(index)">
										<div>
											<div
												style="width: 60%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;float: left;font-size:15px;">
												来自：{{ item.fromEmailAddress }}</div>
											<div style="float: right;font-size:15px;">时间：{{ item.emaildate }}</div>
										</div>
										<div>
											<div
												style="width: 100%; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;  float: left; font-size: 15px;">
												主题：{{ item.emailsubject }}
											</div>
										</div>
										<div v-if="item.emailTagNames !== '' && item.emailTagNames !== null"
											v-for="(Tagitem, index) in item.emailTagNames.split(',')">
											<div
												style="text-align: center; width: 60px; height: 20px;  float:left;font-size:13px; background-color: rgb(89, 177, 158);border-radius: 3px; color: white; margin-top: 3px; margin-bottom: 3px; margin-right: 3px;">
												{{ Tagitem }}</div>
										</div>
									</el-card>
								</div>
								<el-pagination @current-change="handlePageChange" :current-page="currentPage"
									:page-size="pageSize" :total="totalItems" background layout="prev, pager, next"
									style="margin-top: 5px;" />
							</el-scrollbar>
							<el-scrollbar v-else height="750px">
								<el-divider>
									无数据
								</el-divider>
							</el-scrollbar>
						</template>
					</el-col>
					<el-col :span="14" style="width: 60%;">
						<div style=" margin-left: 30px; " v-if="showEmailContent">
							<el-card style="max-width: 100%;min-height: 200px; max-height: 200px; ">
								<div
									style="overflow: hidden; text-overflow: ellipsis; font-size: 25px;font-weight: bold; min-height: 65px;max-height: 65px;border: 0px solid red;">
									<span>{{ selectRowEmail.subject }}</span>
								</div>
								<div style="margin-top: 5px;min-height: 25px;max-height: 25px;border: 0px solid red;">
									<span>发件人:
									</span>{{
										selectRowEmail.from }}
								</div>
								<div style="min-height: 25px;max-height: 25px;border: 0px solid red;">
									<span>收件人: </span>{{ selectRowEmail.to }}
								</div>
								<div class="outer">
									<div>
										<el-button-group>
											<el-tooltip content="标记未读" placement="bottom" effect="light">
												<el-button type="primary" icon="Message" @click="MarkUnread"
													v-if="showUnReadBtn"></el-button>
											</el-tooltip>
											<el-tooltip content="回复邮件" placement="bottom" effect="light">
												<el-button type="primary" icon="edit" @click="opensendEmaildialog(1)"
													v-if="showReplyBtn"></el-button>
											</el-tooltip>
											<el-tooltip content="转发邮件" placement="bottom" effect="light">
												<el-button type="primary" icon="promotion"
													@click="opensendEmaildialog(2)" v-if="showForWardBtn" />
											</el-tooltip>
											<el-popover placement="bottom" title="选择标签" :width="400" trigger="click">
												<template #reference>
													<el-button type="primary" icon="CollectionTag"
														v-if="showEmailTagBtn" @click="TagClick">
													</el-button>
												</template>
												<div class="mt-4">
													<el-checkbox-group v-model="EmailTagcheckboxGroup" size="large">
														<el-checkbox @change="handleCheckChange"
															v-for="     option in EmailTagcheckboxoptions     "
															:label="option.value" :key="option.value">
															{{ option.label }}
														</el-checkbox>
													</el-checkbox-group>
												</div>
												<el-divider />
												<div slot="footer">
													<el-input style="width:55%" v-model="newEmailTagName"
														placeholder="标签名称,最多10个汉字" size="default" :maxlength="10"
														@input="validateInput"></el-input>&nbsp;
													<el-button style="width:35%" type="primary" @click="addNewEmailTag"
														size="default">添加新标签</el-button>
												</div>
											</el-popover>
											<el-tooltip content="移至垃圾箱" placement="bottom" effect="light">
												<el-button type="primary" icon="Delete" @click="TrashEmail"
													v-if="showDleteBtn" />
											</el-tooltip>
											<el-tooltip content="移至收件箱" placement="bottom" effect="light">
												<el-button type="primary" icon="MessageBox" @click="MoveEmailInbox"
													v-if="showTrashBtn" />
											</el-tooltip>
										</el-button-group>
									</div>
								</div>
							</el-card>
							<el-card style="max-height: 580px;">
								<el-scrollbar height="520px">
									<div style="display: flex !important; width: 100% !important;">
										<div style="width: 100% !important;padding-bottom:10px; !important"
											v-html=selectRowEmail.body>
										</div>
									</div>
								</el-scrollbar>
							</el-card>
						</div>
					</el-col>
				</el-row>
			</el-main>
			<el-dialog v-model="sendEmaildialog" title="发送邮件" :close-on-click-modal=false style="width: 70%;">
				<el-form class="mt10" :model="sendEmailForm" label-width="110px">
					<el-form-item label="收件人" prop="toEmail"
						:rules="[{ required: true, message: '请输入收件人', trigger: 'blur' }]">
						<el-input v-model="sendEmailForm.toEmail"></el-input>
					</el-form-item>
					<el-form-item label="邮件主题" prop="subject"
						:rules="[{ required: true, message: '请输入邮件主题', trigger: 'blur' }]">
						<el-input v-model="sendEmailForm.subject"></el-input>
					</el-form-item>
					<el-form-item label="邮件内容" prop="htmlContent"
						:rules="[{ required: true, message: '请输入邮件内容', trigger: 'blur' }]">
						<editor v-model="sendEmailForm.htmlContent" />
					</el-form-item>
					<el-form-item label="发送自己" prop="sendMe">
						<el-switch v-model="sendEmailForm.sendMe" active-text="是" inactive-text="否"></el-switch>
					</el-form-item>
					<!-- <el-form-item label="附件">
					<UploadFile v-model="sendEmailForm.fileUrl" :limit="5" :fileSize="15"
						:data="{ fileDir: 'email', uploadType: 1 }" />
				</el-form-item> -->
				</el-form>
				<template #footer>
					<span class="dialog-footer">
						<el-button @click="sendEmaildialog = false">取消</el-button>
						<el-button type="primary" @click="SaveEmailDraft">暂存草稿</el-button>
						<el-button type="primary" @click="formSubmit">发送邮件</el-button>
					</span>
				</template>
			</el-dialog>
			<el-dialog v-model="ConfigEmaildialog" title="配置邮箱" :close-on-click-modal=false style="width: 35%;"
				@close="handleConfigEmailDialogClose">
				<el-form class="mt10" :model="ConfigEmailForm" label-width="250px">
					<el-form-item label="邮箱地址">
						<el-input v-model="ConfigEmailForm.email" style="width: 300px"
							@input="handleEmailInput"></el-input>
					</el-form-item>
					<el-form-item label="邮件授权码/密码">
						<el-input v-model="ConfigEmailForm.password" type="password" style="width: 300px"></el-input>
					</el-form-item>
					<el-form-item label="邮件发送服务器地址(SMTP协议)">
						<el-input v-model="ConfigEmailForm.emailSendServer" style="width: 300px"></el-input>
					</el-form-item>
					<el-form-item label="发信端口(默认465)">
						<el-input-number v-model="ConfigEmailForm.smtpPort" style="width: 300px"></el-input-number>
					</el-form-item>
					<el-form-item label="邮件接受服务器地址(IMAP协议)">
						<el-input v-model="ConfigEmailForm.emailReceiveServer" style="width: 300px"></el-input>
					</el-form-item>
					<el-form-item label="收信端口(默认993)">
						<el-input-number v-model="ConfigEmailForm.imapPort" style="width: 300px"></el-input-number>
					</el-form-item>
				</el-form>
				<template #footer>
					<span class="dialog-footer">
						<el-button @click="ConfigEmaildialog = false">取消</el-button>
						<el-button type="primary" @click="ConfigUserEmail">确定保存</el-button>
					</span>
				</template>
			</el-dialog>
		</el-container>
	</div>
</template>
<script setup lang="ts" name="Email">
import { callWithAsyncErrorHandling, createApp, reactive, ref, toRefs, onMounted } from 'vue'
import { ElMessage, ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer, ElTabs, UploadUserFile, UploadProps, ElLoading } from 'element-plus'
import Editor from '@/components/Editor/index.vue'
import request from '@/utils/request'
import { number } from 'echarts';
import { send } from 'vite';
import { useRouter } from 'vue-router';
import { closePage } from '@/plugins/tab';
import { get } from 'sortablejs';

const EmailMenuShow = ref(false);

const VerifyUserEmailConfigurationExists = () => {
	return request({
		url: 'Email/VerifyUserEmailConfigurationExists/VerifyUserEmailConfigurationExists',
		method: 'GET'
	}).then(response => {
		if (response.data == 0) {
			ElMessage({
				message: '您还没有配置邮箱，请先进行邮箱配置！',
				type: 'warning'
			})
			ConfigEmaildialog.value = true;
		} else {
			EmailMenuShow.value = true;
			ConfigEmaildialog.value = false;
			getInboxEmail(currentPage.value, pageSize.value, emailType);
		}
	}).catch(error => {
		console.error(error);
		return null;
	});
}
VerifyUserEmailConfigurationExists();

const handleEmailInput = () => {
	const email = ConfigEmailForm.email;
	const atIndex = email.indexOf('@');
	if (atIndex !== -1) {
		const domain = email.substring(atIndex + 1);
		ConfigEmailForm.password = '';
		ConfigEmailForm.emailSendServer = 'smtp.' + domain;
		ConfigEmailForm.emailReceiveServer = 'imap.' + domain;
		ConfigEmailForm.smtpPort = 465;
		ConfigEmailForm.imapPort = 993;
	}
};
const router = useRouter();
const handleConfigEmailDialogClose = () => {
	if (ConfigEmailForm.email == '' || ConfigEmailForm.password == '' || ConfigEmailForm.emailSendServer == '' || ConfigEmailForm.emailReceiveServer == '' || ConfigEmailForm.smtpPort == 0 || ConfigEmailForm.imapPort == 0) {
		closePage().then(() => {
			console.log('关闭邮箱标签页');
		});
	};
}


//card样式
const dynamicWidth = ref('100%');
const dynamicHeight = ref('100px');
const dynamicMaxHeight = ref('100px');
const dynamicBorder = ref('1');
const dynamicMargin = ref('5px');
const dynamicbackgroundcolor = ref('');//#626aef
const dynamiccolor = ref('');//#f7f2f2

const IsEditUserEmailConfig = ref(false);

//初始化收件箱
//getInboxEmail();
const selectedMenu = ref(''); // 默认选中收件箱
const sendEmaildialog = ref(false);
const inboxEmailTable = ref([]);
//移至垃圾箱
const showDleteBtn = ref(false);
//移至收件箱
const showTrashBtn = ref(false);
//转发邮件
const showForWardBtn = ref(false);
//回复邮件
const showReplyBtn = ref(false);
//标记未读
const showUnReadBtn = ref(false);
//选择标签
const showEmailTagBtn = ref(false);
//显示邮件内容
const showEmailContent = ref(false);
//配置邮箱弹窗
const ConfigEmaildialog = ref(false);
const ConfigEmailForm = reactive({
	email: '',
	password: '',
	smtpPort: 0,
	imapPort: 0,
	emailSendServer: '',
	emailReceiveServer: ''
})
const sendEmailForm = reactive({
	toEmail: '',
	subject: '',
	htmlContent: '',
	sendMe: false
})
function formSubmit() {
	var requestData = {
		toEmail: sendEmailForm.toEmail,
		subject: sendEmailForm.subject,
		EmailContent: sendEmailForm.htmlContent
	}
	sendEmail(requestData)
		.then(response => {
			// Handle success response
			console.log(response)
			ElMessage({
				message: '邮件已发送！😊',
				type: 'success'
			})
			sendEmaildialog.value = false;
		})
		.catch(error => {
			// Handle error response
			console.error(error)
			// Show error message to the user
			// proxy.$message.error('邮件发送失败')
		})
}
function sendEmail(data) {
	return request({
		url: 'Email/SendEmail/send',
		method: 'POST',
		data: data
	})
}

function ConfigUserEmail() {
	const loading = ElLoading.service({
		lock: true,
		text: '正在配置邮箱并获取邮件，请稍等.....',
		background: 'rgba(0, 0, 0, 0.7)',
	})
	try {
		var requestData = {
			UserEmail: ConfigEmailForm.email,
			UserEmailSendServer: ConfigEmailForm.emailSendServer,
			UserEmailReceiveServer: ConfigEmailForm.emailReceiveServer,
			UserEmailAuth: ConfigEmailForm.password,
			UserEmailSendPort: ConfigEmailForm.smtpPort,
			UserEmailReceivePort: ConfigEmailForm.imapPort
		}
		if (IsEditUserEmailConfig.value == false) {
			request({
				url: 'Email/AddUserEmailConfig/AddUserEmailConfig',
				method: 'POST',
				data: requestData
			}).then(response => {
				if (response != null) {
					ConfigEmaildialog.value = false;
					ElMessage({
						message: response.data.msg,
						type: 'success'
					})

				} else {
					console.error('No data in response');
				}
			}).catch(error => {
				console.error('邮箱配置失败:', error);
			}).finally(() => {
				// 在请求完成之后，无论成功或失败，关闭加载动画
				loading.close();
				handleSelect(1);
			});
		} else {
			request({
				url: 'Email/EditUserEmailConfig/EditUserEmailConfig',
				method: 'POST',
				data: requestData
			}).then(response => {
				if (response != null) {
					ConfigEmaildialog.value = false;
					ElMessage({
						message: response.data.msg,
						type: 'success'
					})
					getInboxEmail(currentPage.value, pageSize.value, emailType);
				} else {
					console.error('No data in response');
				}
			}).catch(error => {
				console.error('邮箱配置失败:', error);
			}).finally(() => {
				// 在请求完成之后，无论成功或失败，关闭加载动画
				loading.close();
				handleSelect(1);
			});
		}
	} catch (error) {
		loading.close();
		console.error('邮箱配置失败:', error);
	}
}

const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const emailType = ref(1);
const EmailInboxArray = ref([]);
const selectedIndex = ref(null);
const EmailTagIndex = ref(0);


const handlePageChange = async (newPage) => {
	// Fetch new data based on the new page and update currentTableData
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await getInboxEmail(start, end, emailType);  // Assume fetchData is a function that fetches data from the server
};

function getInboxEmail(start, end, emailType) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'Email/GetEmailInboxList/GetInbox',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				EmailType: emailType.value,
				EmailTagIndex: EmailTagIndex.value
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				inboxEmailTable.value = response.data.result;
				EmailInboxArray.value = response.data.result;
				totalItems.value = response.data.totalNum;
				selectedIndex.value = ref(null);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					getInboxEmail(start - 1, end, emailType);
				}
				EmailInboxArray.value = null;
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}
const selectRowEmail = ref({
	subject: '',
	body: '',
	from: '',
	fromaddress: '',
	date: '',
	to: '',
});

const EmailModel = reactive({
	"id": '',
	"userID": '',
	"emailID": '',
	"emailType": 0,
	"fromEmail": '',
	"fromEmailAddress": '',
	"toEmail": '',
	"toEmailAddress": '',
	"emailsubject": '',
	"emailContent": '',
	"emaildate": '',
	"isRead": 0,
	"EmailTags": '',
	"EmailTagNames": ''
});

const EmailCardClick = (index) => {
	selectedIndex.value = index;
	localStorage.setItem('selectedIndex', index);
	showEmailContent.value = true;
	selectRowEmail.value.subject = EmailInboxArray.value[index].emailsubject
	selectRowEmail.value.body = EmailInboxArray.value[index].emailContent
	selectRowEmail.value.from = EmailInboxArray.value[index].fromEmail
	selectRowEmail.value.date = EmailInboxArray.value[index].date
	selectRowEmail.value.to = EmailInboxArray.value[index].toEmail
	selectRowEmail.value.fromaddress = EmailInboxArray.value[index].fromEmailAddress

	EmailModel.id = EmailInboxArray.value[index].id
	EmailModel.userID = EmailInboxArray.value[index].userID
	EmailModel.emailID = EmailInboxArray.value[index].emailID
	EmailModel.emailType = EmailInboxArray.value[index].emailType
	EmailModel.fromEmail = EmailInboxArray.value[index].fromEmail
	EmailModel.fromEmailAddress = EmailInboxArray.value[index].fromEmailAddress
	EmailModel.toEmail = EmailInboxArray.value[index].toEmail
	EmailModel.toEmailAddress = EmailInboxArray.value[index].toEmailAddress
	EmailModel.emailsubject = EmailInboxArray.value[index].emailsubject
	EmailModel.emailContent = EmailInboxArray.value[index].emailContent
	EmailModel.emaildate = EmailInboxArray.value[index].emaildate
	EmailModel.isRead = EmailInboxArray.value[index].isRead
	EmailModel.EmailTags = EmailInboxArray.value[index].emailTags
}


const handleSelect = (key) => {
	if (key == 6) {
		openUserEmailConfigDialog();
	} else {
		VerifyUserEmailConfigurationExists();
		showEmailContent.value = false;
		if (key.includes('-')) {
			EmailTagIndex.value = key.split('-')[1];
			emailType.value = 5;
			key = key.split('-')[0];
			showUnReadBtn.value = false;
			showReplyBtn.value = true;
			showForWardBtn.value = true;
			showEmailTagBtn.value = false;
			showDleteBtn.value = false;
			showTrashBtn.value = false;
		} else {
			if (key == 2) {
				//已发送
				emailType.value = 2;
				showUnReadBtn.value = false;
				showReplyBtn.value = false;
				showForWardBtn.value = true;
				showEmailTagBtn.value = true;
				showDleteBtn.value = true;
				showTrashBtn.value = false;
			} else if (key == 3) {
				//草稿箱
				emailType.value = 3;
				showUnReadBtn.value = false;
				showReplyBtn.value = false;
				showForWardBtn.value = true;
				showEmailTagBtn.value = true;
				showDleteBtn.value = true;
				showTrashBtn.value = false;
			} else if (key == 4) {
				//垃圾箱
				emailType.value = 4;
				showUnReadBtn.value = false;
				showReplyBtn.value = false;
				showForWardBtn.value = false;
				showEmailTagBtn.value = true;
				showDleteBtn.value = false;
				showTrashBtn.value = true;
			} else if (key == 1) {
				//收件箱
				emailType.value = 1;
				showUnReadBtn.value = false;
				showReplyBtn.value = true;
				showForWardBtn.value = true;
				showEmailTagBtn.value = true;
				showDleteBtn.value = true;
				showTrashBtn.value = false;
			}
		}
		selectedMenu.value = key
		currentPage.value = 1;//只要切换了菜单，就重置页码
		getInboxEmail(currentPage.value, pageSize.value, emailType);
	}
}



function checkUserEmailConfig() {
	return request({
		url: 'Email/GetEmailConfigByUser/GetEmailConfigByUser',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			return response;
		} else {
			return null;
		}
	}).catch(error => {
		console.error(error);
		return null;
	});
}

const openUserEmailConfigDialog = async () => {
	try {
		const response = await checkUserEmailConfig();
		if (response && response.data) {
			ConfigEmailForm.email = response.data.userEmail;
			ConfigEmailForm.password = response.data.userEmailAuth;
			ConfigEmailForm.emailSendServer = response.data.userEmailSendServer;
			ConfigEmailForm.smtpPort = response.data.userEmailSendPort;
			ConfigEmailForm.emailReceiveServer = response.data.userEmailReceiveServer;
			ConfigEmailForm.imapPort = response.data.userEmailReceivePort;
			IsEditUserEmailConfig.value = true;
		} else {
			IsEditUserEmailConfig.value = false;
			console.error('No data in response');
		}
		ConfigEmaildialog.value = true;
	} catch (error) {
		console.error('Error fetching email config:', error);
	}
}


const opensendEmaildialog = async (type) => {
	try {
		const response = await checkUserEmailConfig();
		if (response && response.data) {
			if (type == 0) {
				sendEmailForm.toEmail = ''
				sendEmailForm.htmlContent = ''
				sendEmailForm.subject = ''
				sendEmaildialog.value = true
			} else if (type == 1) {
				sendEmailForm.toEmail = selectRowEmail.value.fromaddress;
				sendEmaildialog.value = true
			} else if (type == 2) {
				sendEmailForm.toEmail = ''
				sendEmailForm.subject = selectRowEmail.value.subject;
				sendEmailForm.htmlContent = selectRowEmail.value.body;
				sendEmaildialog.value = true
			}
		} else {
			ElMessage({
				message: '还没有配置邮箱，请先进行邮箱配置！😒',
				type: 'warning'
			})
			ConfigEmaildialog.value = true;
		}
	} catch (error) {
		console.error('😒出错啦！！！！这是错误信息：', error);
	}

}

//标记未读邮件
const MarkUnread = () => {
	EmailModel.isRead = 0;
	request({
		url: 'Email/EditEmailIsRead/EditEmailIsRead',
		method: 'POST',
		data: EmailModel
	}).then(response => {
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			getInboxEmail(currentPage.value, pageSize.value, emailType);
			showEmailContent.value = false;
		} else {
			console.error('更新邮件为未读状态失败');
		}
	}).catch(error => {
		console.error('将该邮件标记为未读状态出错！😔错误内容：', error);
	})
}

//标记垃圾邮件
const TrashEmail = () => {
	EmailModel.emailType = 4;
	request({
		url: 'Email/EditTrashEmail/TrashEmail',
		method: 'POST',
		data: EmailModel
	}).then(response => {
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			getInboxEmail(currentPage.value, pageSize.value, emailType);
			showEmailContent.value = false;
		} else {
			console.error('更新邮件为垃圾邮件失败');
		}
	}).catch(error => {
		console.error('将该邮件标记为垃圾邮件出错！😔错误内容：', error);
	})
}

//移至收件箱
const MoveEmailInbox = () => {
	EmailModel.emailType = 1;
	request({
		url: 'Email/EditInBoxEmail/MoveInBoxEmail',
		method: 'POST',
		data: EmailModel
	}).then(response => {
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			getInboxEmail(currentPage.value, pageSize.value, emailType);
			showEmailContent.value = false;
		} else {
			console.error('更新邮件为垃圾邮件失败');
		}
	}).catch(error => {
		console.error('将该邮件标记为垃圾邮件出错！😔错误内容：', error);
	})
}

//将该邮件存入草稿箱
const SaveEmailDraft = () => {
	if (sendEmailForm.toEmail == '' || sendEmailForm.subject == '' || sendEmailForm.htmlContent == '') {
		ElMessage({
			message: '收件人、主题、邮件内容不能为空！😒',
			type: 'warning'
		})
		return;
	} else {
		request({
			url: 'Email/SaveDraftEmail/SaveDraftEmail',
			method: 'POST',
			data: {
				toEmail: sendEmailForm.toEmail,
				subject: sendEmailForm.subject,
				EmailContent: sendEmailForm.htmlContent
			}
		}).then(response => {
			if (response != null) {
				ElMessage({
					message: response.data.msg,
					type: 'success'
				})
				sendEmaildialog.value = false;
				//getInboxEmail(currentPage.value, pageSize.value, emailType);
			} else {
				console.error('邮件存草稿箱失败');
			}
		}).catch(error => {
			console.error('将该邮件存入草稿箱出错！😔错误内容：', error);
		})
	}
}

const EmailTagcheckboxGroup = ref([]);
const EmailTagcheckboxoptions = ref([]);
const UserEmailTagList = ref([]);
const GetUserEmailTagList = () => {
	return request({
		url: 'Email/GetUserEmailTagList/GetUserEmailTag',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			if (EmailModel.EmailTags != null && EmailModel.EmailTags != '') {
				var EmailTagArray = EmailModel.EmailTags.split(',');
				response.data.forEach(item => {
					EmailTagcheckboxoptions.value.push({ label: item.emailTagName, value: item.id });
					if (EmailTagArray.length > 0) {
						EmailTagArray.forEach(element => {
							if (element == item.id) {
								EmailTagcheckboxGroup.value.push(item.id);
							}
						});
					}
				});
			} else {
				response.data.forEach(item => {
					EmailTagcheckboxoptions.value.push({ label: item.emailTagName, value: item.id });
				});
			}

			UserEmailTagList.value = response.data;
			return response.data.result;
		} else {
			return null;
		}
	}).catch(error => {
		console.error(error);
		return null;
	});
}
GetUserEmailTagList();

const TagClick = async () => {
	initEmailTag();
}


const newEmailTagName = ref('');
const addNewEmailTag = () => {
	if (newEmailTagName.value == '') {
		ElMessage({
			message: '标签名称不能为空！😒',
			type: 'warning'
		})
		return;
	} else {
		request({
			url: 'Email/AddUserEmailTag/AddUserEmailTag',
			method: 'POST',
			data: {
				EmailTagName: newEmailTagName.value
			}
		}).then(response => {
			if (response != null) {
				ElMessage({
					message: response.data.msg,
					type: 'success'
				})
				initEmailTag();
			} else {
				console.error('添加新标签失败');
			}
		}).catch(error => {
			console.error('添加新标签出错！😔错误内容：', error);
		})
	}
}

const initEmailTag = () => {
	EmailTagcheckboxGroup.value = [];
	EmailTagcheckboxoptions.value = [];
	newEmailTagName.value = '';
	GetUserEmailTagList();
}
onMounted(() => {
	const savedIndex = localStorage.getItem('selectedIndex');
	if (savedIndex !== null) {
		selectedIndex.value = Number(savedIndex);
	}
});

const handleCheckChange = (value) => {
	EditEmailTags();
};

const EditEmailTags = () => {
	EmailModel.EmailTags = EmailTagcheckboxGroup.value.toString();
	request({
		url: 'Email/EditEmailTags/EditEmailTags',
		method: 'POST',
		data: EmailModel
	}).then(response => {
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			getInboxEmail(currentPage.value, pageSize.value, emailType);
		} else {
			console.error('更新邮件标签失败');
		}
	}).catch(error => {
		console.error('更新邮件标签失败！😔错误内容：', error);
	})
}

const validateInput = (value) => {
	if (value.length > 10) {
		ElMessage({
			message: '标签名称不能超过10个字符！😒',
			type: 'warning'
		})
	}
}

</script>

<style lang="scss">
.selected-card {
	background-color: #626aef;
	color: #f7f2f2;

}

.outer {
	display: flex;
	justify-content: space-between;
	margin-top: 20px;
	/* 让子元素分布在两端 */
}

/* 可选：为了更好的视觉效果，添加一些内边距和边框 */
.outer,
.left,
.right {
	padding: 0px;
	border: 0px solid #ccc;
}

.el-table .el-table__row {
	height: 30px !important;
}

.el-table .el-table__row .cell {
	height: 29px !important;
	padding: 0 !important;
	line-height: 29px !important;
	width: 200px !important;
	/* 设置单元格宽度 */
	font-size: 18px !important;
	/* 设置字体大小 */
	white-space: nowrap;
	/* 防止文本换行 */
	overflow: hidden;
	/* 隐藏超出单元格的文本 */
	text-overflow: ellipsis;
	/* 使用省略号表示超出部分 */
}
</style>
