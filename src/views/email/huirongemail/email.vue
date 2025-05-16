<template>
	<div>
		<el-container class="layout-container-demo" style="height: 100%">
			<el-aside width="300px">
				<el-scrollbar height="calc(100vh - 60px)">
					<el-menu :default-active="activeMenu">
						<!-- 修改写邮件图标为 Edit -->
						<el-menu-item @click="handleWriteEmail">
							<el-icon>
								<Edit />
							</el-icon>写邮件
						</el-menu-item>
						<el-menu-item index="1" @click="MenuClick(1)">
							<el-icon>
								<message />
							</el-icon>收件箱
						</el-menu-item>
						<el-menu-item index="3" @click="MenuClick(3)">
							<el-icon>
								<EditPen />
							</el-icon>草稿箱
						</el-menu-item>
						<el-menu-item index="4" @click="MenuClick(4)">
							<el-icon>
								<Delete />
							</el-icon>垃圾箱
						</el-menu-item>
						<el-menu-item index="2" @click="MenuClick(2)">
							<el-icon>
								<Position />
							</el-icon>已发邮件
						</el-menu-item>
						<el-menu-item index="6" @click="MenuClick(6)">
							<el-icon>
								<Box />
							</el-icon>归档邮件
						</el-menu-item>
						<!-- 添加分隔线 -->
						<el-divider />

						<!-- Labels 部分 -->
						<div class="menu-section-title">
							<span>标签</span>
							<el-button class="add-button" link size="small" @click="showNewFolderDialog = true">
								<el-icon>
									<Plus />
								</el-icon>
							</el-button>
						</div>
						<el-menu-item v-for="tag in UserEmailTagList" :key="tag.id" :index="`tag-${tag.id}`"
							@click="filterByTag(tag.id)" class="tag-menu-item">
							<div class="tag-content">
								<div class="tag-left">
									<el-icon>
										<CollectionTag />
									</el-icon>
									<span>{{ tag.emailTagName }}</span>
								</div>
								<el-button v-if="!isSystemTag(tag.emailTagName)" class="delete-button" link
									type="danger" size="small" @click.stop="deleteEmailTag(tag.id)">
									<el-icon>
										<Delete />
									</el-icon>
								</el-button>
							</div>
						</el-menu-item>

						<!-- 添加分隔线 -->
						<el-divider />

						<!-- Folders 部分 -->

						<div class="menu-section-title">
							<span>邮件分类</span>
						</div>
						<el-menu-item v-for="folder in customFolders" :key="folder.id" :index="`folder-${folder.id}`"
							@click="filterByFolder(folder.id)">
							<el-icon>
								<Folder />
							</el-icon>
							<span>{{ folder.fromEmail }}</span>
						</el-menu-item>

					</el-menu>
				</el-scrollbar>
			</el-aside>
			<el-container>
				<el-header style="text-align: left; font-size: 12px; height: 50px;">
					<div class="search-container" style="width: 70%;">
						<el-input v-model="input1" style="width: 100%" size="large" placeholder="搜索邮件"
							@input="handleLocalSearch" clearable @clear="clearLocalSearch">
							<template #suffix>
								<el-tooltip content="高级搜索" placement="bottom">
									<el-icon class="cursor-pointer" @click="showAdvancedSearch = true">
										<Operation />
									</el-icon>
								</el-tooltip>
							</template>
						</el-input>
					</div>
					<div class="toolbar">
						<el-dropdown>
							<div class="user-profile">
								<el-avatar size="small">{{ ConfigEmailForm.email ?
									ConfigEmailForm.email.split('@')[0][0].toUpperCase() : 'U' }}</el-avatar>
								<span class="username">{{ ConfigEmailForm.email ? ConfigEmailForm.email.split('@')[0] :
									'User' }}</span>
								<el-icon class="el-icon--right">
									<arrow-down />
								</el-icon>
							</div>
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item>
										<el-icon>
											<User />
										</el-icon>
										<span @click="openSignatureDialog">签名设置</span>
									</el-dropdown-item>
									<el-dropdown-item>
										<el-icon>
											<Setting />
										</el-icon>
										<span @click="openUserEmailConfigDialog">邮箱设置</span>
									</el-dropdown-item>
								</el-dropdown-menu>
							</template>
						</el-dropdown>
					</div>
				</el-header>
				<el-main>
					<!-- 邮件列表视图 -->
					<div v-show="!showEmailDetail">
						<!-- 修改表格容器样式 -->
						<div style="width: 100%; ">
							<!-- 添加搜索状态提示到这里 -->
							<div v-if="isSearchMode" class="search-status">
								<el-alert title="当前显示搜索结果" type="info" :closable="false" style="margin-bottom: 10px;">
									<template #default>
										<el-button link type="primary" @click="resetSearchForm">
											清除搜索
										</el-button>
									</template>
								</el-alert>
							</div>
							<el-table :data="EmailTableData" @selection-change="handleSelectionChange"
								@row-click="handleRowClick" :row-class-name="tableRowClassName" style="width: 100%;">
								<!-- 选择列 -->
								<!-- <el-table-column type="selection" fixed min-width="50" align="center" /> -->
								<!-- 邮件ID列 -->
								<el-table-column prop="id" label="邮件编号" min-width="200" v-if="false"
									show-overflow-tooltip />
								<!-- EmailID列 -->
								<el-table-column prop="EmailID" label="EmailID" min-width="200" v-if="false"
									show-overflow-tooltip />
								<!-- 邮件内容列 -->
								<el-table-column prop="emailContent" label="邮件内容" min-width="200" v-if="false"
									show-overflow-tooltip />
								<!--发件人列 -->
								<el-table-column prop="name" label="发件人" min-width="200" show-overflow-tooltip />
								<el-table-column prop="FromEmailAddress" label="发件人Email" min-width="200"
									show-overflow-tooltip v-if="false" />
								<!--收件人列 -->
								<el-table-column prop="toEmail" label="收件人" min-width="200" show-overflow-tooltip
									v-if="false" />
								<!-- 主题列 -->
								<!-- <el-table-column prop="subject" label="主题" min-width="400" show-overflow-tooltip /> -->
								<el-table-column prop="subject" min-width="400" label="主题" show-overflow-tooltip>
									<template #default="{ row }">
										<div style="display: flex; align-items: center;">
											<span>{{ row.subject }}</span>
											<el-icon v-if="row.hasAttachments"
												style="margin-left: 8px; color: #909399;">
												<Paperclip />
											</el-icon>
										</div>
									</template>
								</el-table-column>
								<!-- 日期列 -->
								<el-table-column prop="date" label="日期" min-width="150" show-overflow-tooltip />

								<!-- 标签列 -->
								<el-table-column prop="emailTags" label="标签ID" min-width="150" v-if="false"
									show-overflow-tooltip />
								<el-table-column prop="emailTagsShow" label="标签" min-width="150" show-overflow-tooltip>
									<template #default="{ row }">
										<div class="email-tags-container">
											<el-tag v-for="tagId in getRowTags(row.emailTags)" :key="tagId" size="small"
												:type="getTagType(getTagName(tagId))" class="mx-1">
												{{ getTagName(tagId) }}
											</el-tag>
										</div>
									</template>
								</el-table-column>

								<!-- 操作按钮列 -->
								<!-- <el-table-column fixed="right" min-width="120">
									<template #default="{ row }">
										<div class="operation-buttons"
											v-if="selectedRows.some(selected => selected.id === row.id)">
											<el-button-group>
												<el-button icon="Delete" circle />
												<el-button icon="MessageBox" circle />
												<el-button icon="Timer" circle />
											</el-button-group>
										</div>
									</template>
								</el-table-column> -->
							</el-table>
							<!-- 添加分页组件 -->
							<div class="pagination-container">
								<el-pagination v-model:current-page="currentPage" v-model:page-size="pageSize"
									:page-sizes="[20, 30, 50]" :total="totalItems" @size-change="handleSizeChange"
									@current-change="handleCurrentChange"
									layout="total, sizes, prev, pager, next, jumper" />
							</div>
						</div>
					</div>
					<!-- 邮件详情视图 -->
					<div v-show="showEmailDetail" class="email-detail">
						<!-- 返回按钮和工具栏 -->
						<div class="detail-header">
							<div class="left-actions">
								<el-button @click="backToList" icon="Back" style="margin-right: 5px;">返回</el-button>
								<!-- 只在非草稿箱时显示这些按钮 -->
								<template v-if="activeMenu != '3'">
									<el-button icon="Box" circle title="归档" style="margin-right: 5px;"
										@click="MoveEmail(6)" />
									<el-dropdown trigger="click" @command="handleMoveEmail" style="margin-right: 5px;">
										<el-button icon="Folder" circle title="移至" />
										<template #dropdown>
											<el-dropdown-menu>
												<!-- 系统默认文件夹 -->
												<el-dropdown-item command="1">收件箱</el-dropdown-item>
												<el-dropdown-item command="2">已发送</el-dropdown-item>
												<el-dropdown-item command="3">草稿箱</el-dropdown-item>
												<el-dropdown-item command="4">垃圾箱</el-dropdown-item>
											</el-dropdown-menu>
										</template>
									</el-dropdown>
									<el-button :icon="Message" circle title="标记为未读"
										@click="markAsUnread(currentEmail.id)" />
									<el-button icon="Bell" circle title="设置提醒"
										@click="openReminderDialog(currentEmail)" />
								</template>
							</div>
							<div class="right-actions" v-if="activeMenu != '3'">
								<el-button-group>
									<!-- 在非已发邮件时显示回复按钮 -->
									<template v-if="activeMenu != '2'">
										<el-button icon="ChatRound" circle title="回复" @click="handleReply(false)" />
										<el-button icon="Share" circle title="回复全部" @click="handleReply(true)" />
									</template>
									<!-- 转发按钮始终显示 -->
									<el-button icon="Right" circle title="转发" @click="handleForward" />
									<el-popover ref="tagPopover" v-model:visible="tagPopoverVisible" placement="bottom"
										:width="300" trigger="click" popper-class="tag-popover">
										<template #reference>
											<el-button icon="CollectionTag" circle
												:class="{ 'has-tags': currentEmail.tags?.length }" title="标签" />
										</template>
										<!-- 标签选择面板 -->
										<div class="tag-panel">
											<div class="tag-list">
												<el-checkbox-group v-model="EmailTagcheckboxGroup"
													@change="handleTagChange">
													<div v-for="tag in EmailTagcheckboxoptions" :key="tag.value"
														class="tag-item">
														<div class="tag-checkbox">
															<el-checkbox :label="tag.value">{{ tag.label
															}}</el-checkbox>
														</div>
														<div class="tag-actions">
															<el-button type="danger" link size="small"
																v-if="!isSystemTag(tag.label)"
																@click="deleteEmailTag(tag.value)">
																删除
															</el-button>
														</div>
													</div>
												</el-checkbox-group>
											</div>
											<!-- 添加新标签 -->
											<div class="add-tag-section">
												<el-input v-model="newEmailTagName" placeholder="新建标签" size="small"
													:maxlength="10" @keyup.enter="addNewEmailTag(0)">
													<template #append>
														<el-button @click="addNewEmailTag(0)">添加</el-button>
													</template>
												</el-input>
											</div>
										</div>
									</el-popover>
									<el-button icon="Delete" circle title="删除" @click="handleMoveEmail('4')" />
								</el-button-group>
							</div>
						</div>

						<!-- 邮件内容区域 -->
						<div class="detail-content">
							<div class="email-header">
								<h2 class="subject">{{ currentEmail.subject }}</h2>

								<!-- 标签区域 -->
								<div class="email-tags" v-if="EmailTagcheckboxGroup.length">
									<el-tag v-for="tagId in EmailTagcheckboxGroup" :key="tagId" size="small"
										effect="plain" class="custom-tag" :type="getTagType(getTagName(tagId))">
										{{ getTagName(tagId) }}
									</el-tag>
								</div>

								<!-- 邮件信息 -->
								<div class="meta-info">
									<div class="meta-item">
										<span class="label">发件人：</span>
										<span class="value">{{ currentEmail.from }}</span>
									</div>
									<div class="meta-item">
										<span class="label">收件人：</span>
										<span class="value">{{ currentEmail.to }}</span>
									</div>
									<div class="meta-item" v-if="currentEmail.cc">
										<span class="label">抄送：</span>
										<span class="value">{{ currentEmail.cc }}</span>
									</div>
									<div class="meta-item">
										<span class="label">日期：</span>
										<span class="value">{{ currentEmail.date }}</span>
									</div>
								</div>
							</div>

							<!-- 邮件正文 -->
							<div class="email-body">
								<div class="email-content-wrapper">
									<!-- 添加安全的 HTML 内容渲染 -->
									<div class="email-content" v-html="currentEmail.content" :style="emailContentStyle">
									</div>
								</div>
							</div>
							<!-- <div class="email-body">
								<el-skeleton :loading="!currentEmail.content" animated>
									<template #template>
										<el-skeleton-item variant="text" style="width: 100%; height: 400px;" />
									</template>
									<template #default>
										<div v-html="currentEmail.content"></div>
									</template>
								</el-skeleton>
							</div> -->


							<!-- 附件区域 -->
							<div v-if="currentEmail.attachments?.length" class="attachments">
								<div class="attachment-header">
									<el-icon>
										<Paperclip />
									</el-icon>
									<span>附件 ({{ currentEmail.attachments.length }})</span>
								</div>
								<div class="attachment-list">
									<div v-for="file in currentEmail.attachments" :key="file.id"
										class="attachment-item">
										<el-icon>
											<Document />
										</el-icon>
										<span class="filename">{{ file.name }}</span>
										<span class="filesize">{{ file.size }}</span>
										<el-button type="primary" link
											@click="downloadAttachment(file.id, file.name, file.fileUrl)">
											下载
										</el-button>
									</div>
								</div>
							</div>
							<!-- 添加回复和转发按钮 -->
							<div class="action-buttons">
								<!-- 在草稿箱显示编辑草稿按钮 -->
								<template v-if="activeMenu == '3'">
									<el-button @click="editDraft">
										<el-icon>
											<EditPen />
										</el-icon>
										编辑草稿
									</el-button>
								</template>
								<!-- 在非草稿箱显示回复等按钮 -->
								<template v-else-if="activeMenu != '2'">
									<el-button class="action-btn" plain @click="handleReply(false)">
										<el-icon>
											<ArrowLeft />
										</el-icon>
										回复
									</el-button>
									<el-button class="action-btn" plain @click="handleReply(true)">
										<el-icon>
											<ArrowLeft />
										</el-icon>
										回复全部
									</el-button>
									<el-button class="action-btn" plain @click="handleForward">
										<el-icon>
											<Right />
										</el-icon>
										转发
									</el-button>
								</template>
								<!-- 在已发邮件中只显示转发按钮 -->
								<template v-else>
									<el-button class="action-btn" plain @click="handleForward">
										<el-icon>
											<Right />
										</el-icon>
										转发
									</el-button>
								</template>
							</div>
						</div>
					</div>
				</el-main>
			</el-container>
		</el-container>
		<!-- 写邮件 Dialog -->
		<el-dialog v-model="showEmailDialog" :title="dialogTitle" :close-on-click-modal="false"
			@close="handleDialogClose" :fullscreen="isFullscreen" class="email-dialog" style="width: 70%;">
			<!-- 自定义标题栏，添加全屏按钮 -->
			<template #header>
				<div class="dialog-header">
					<span>新邮件</span>
					<div class="dialog-header-actions">
						<el-tooltip :content="isFullscreen ? '退出全屏' : '全屏'" placement="bottom">
							<el-button :icon="isFullscreen ? 'FullScreen' : 'Rank'" circle @click="toggleFullScreen" />
						</el-tooltip>
					</div>
				</div>
			</template>
			<div class="email-form">
				<!-- 收件人 -->
				<div class="form-item">
					<div class="form-label">收件人：</div>
					<el-select v-model="emailForm.ToEmail" multiple filterable allow-create default-first-option
						placeholder="请输入邮箱地址" class="full-width">
						<el-option v-for="item in recipientTreeData" :key="item.value" :label="item.label"
							:value="item.value">
						</el-option>
					</el-select>
				</div>
				<!-- 抄送人（当showCc为true时显示） -->
				<div class="form-item" v-if="showCc">
					<div class="form-label">抄送：</div>
					<el-select v-model="emailForm.cc" multiple filterable allow-create default-first-option
						placeholder="请输入抄送邮箱地址" class="full-width">
						<el-option v-for="item in recipientTreeData" :key="item.value" :label="item.label"
							:value="item.value">
						</el-option>
					</el-select>
				</div>

				<!-- 主题 -->
				<div class="form-item">
					<div class="form-label">主题：</div>
					<el-input v-model="emailForm.subject" placeholder="邮件主题" class="form-input" />
				</div>
				<!-- 选择标签 -->
				<div class="form-item">
					<div class="form-label">标签：</div>
					<el-select v-model="emailForm.emailTags" clearable filterable allow-create default-first-option
						placeholder="请选择标签" class="full-width">
						<el-option v-for="tag in EmailTagcheckboxoptions" :key="tag.value" :label="tag.label"
							:value="tag.value">
						</el-option>
					</el-select>
				</div>
				<!-- 富文本编辑器 -->
				<div class="email-content">
					<QuillEditor ref="quillEditor" v-model:content="emailForm.content" contentType="html" toolbar="full"
						theme="snow" :options="editorOptions" :style="{ minHeight: '300px' }" />
				</div>
				<!-- 在编辑器下方添加附件列表 -->
				<div class="attachment-section" v-show="fileList.length > 0">
					<el-upload v-model:file-list="fileList" :auto-upload="false" :on-change="handleFileChange"
						:on-remove="handleFileRemove" class="attachment-list">
						<template #trigger>
							<!-- 触发按钮隐藏，因为我们使用底部的附件按钮 -->
							<div style="display: none;"></div>
						</template>

						<template #file="{ file }">
							<div class="attachment-item">
								<el-icon>
									<Document />
								</el-icon>
								<span class="filename">{{ file.name }}</span>
								<!-- <span class="filesize">{{ formatFileSize(file.size) }}</span> -->
								<el-icon class="remove-icon" @click="handleFileRemove(file)">
									<Close />
								</el-icon>
							</div>
						</template>
					</el-upload>
				</div>
			</div>

			<!-- 底部工具栏 -->
			<template #footer>
				<div class="dialog-footer">
					<div class="left">
						<el-button @click="saveDraft" :loading="isSavingDraft">
							<el-icon>
								<DocumentAdd />
							</el-icon>
							暂存草稿
						</el-button>
						<el-button type="primary" class="mp4" @click="sendEmail">发送</el-button>
						<el-dropdown>
							<el-button>
								<span>
									抄送
								</span>
								<el-icon>
									<ArrowDown />
								</el-icon>
							</el-button>
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item @click="showCc = !showCc">
										<el-icon>
											<Share />
										</el-icon>
										<span style="margin-left: 8px">
											{{ showCc ? '隐藏抄送' : '显示抄送' }}
										</span>
										<el-tag size="small" :type="showCc ? 'success' : 'info'"
											style="margin-left: 8px">
											{{ showCc ? '已启用' : '未启用' }}
										</el-tag>
									</el-dropdown-item>
									<!-- 可以在这里添加其他邮件选项，比如：
                    <el-dropdown-item>
                        <el-icon><Timer /></el-icon>
                        <span style="margin-left: 8px">定时发送</span>
                    </el-dropdown-item>
                    -->
								</el-dropdown-menu>
							</template>
						</el-dropdown>
					</div>
					<div class="right">
						<el-button-group>
							<el-button @click="triggerFileInput" icon="Paperclip">附件</el-button>
							<input type="file" ref="fileInput" multiple style="display: none"
								@change="handleFileInputChange">
							<el-button icon="Delete" @click="discardEmail">丢弃</el-button>
						</el-button-group>
					</div>
				</div>
			</template>
		</el-dialog>
		<!-- 高级搜索对话框 -->
		<el-dialog v-model="showAdvancedSearch" title="高级搜索" width="600px">
			<el-form :model="searchForm" label-width="100px">
				<el-form-item label="发件人">
					<el-input v-model="searchForm.sender" placeholder="请输入发件人邮箱"></el-input>
				</el-form-item>

				<el-form-item label="收件人">
					<el-input v-model="searchForm.receiver" placeholder="请输入收件人邮箱"></el-input>
				</el-form-item>

				<el-form-item label="主题">
					<el-input v-model="searchForm.subject" placeholder="请输入邮件主题"></el-input>
				</el-form-item>

				<el-form-item label="包含字词">
					<el-input v-model="searchForm.includeWords" placeholder="请输入包含的关键词"></el-input>
				</el-form-item>

				<el-form-item label="不包含">
					<el-input v-model="searchForm.excludeWords" placeholder="请输入要排除的关键词"></el-input>
				</el-form-item>

				<!-- <el-form-item label="大小">
					<el-select v-model="searchForm.sizeOperator" style="width: 120px">
						<el-option label="大于" value="gt"></el-option>
						<el-option label="小于" value="lt"></el-option>
						<el-option label="等于" value="eq"></el-option>
					</el-select>
					<el-input-number v-model="searchForm.size" :min="0"
						style="width: 150px; margin-left: 10px"></el-input-number>
					<el-select v-model="searchForm.sizeUnit" style="width: 120px; margin-left: 10px">
						<el-option label="MB" value="MB"></el-option>
						<el-option label="KB" value="KB"></el-option>
					</el-select>
				</el-form-item> -->

				<el-form-item label="日期范围">
					<el-select v-model="searchForm.dateRange" style="width: 120px">
						<el-option label="1天" value="1"></el-option>
						<el-option label="3天" value="3"></el-option>
						<el-option label="7天" value="7"></el-option>
						<el-option label="30天" value="30"></el-option>
						<el-option label="自定义" value="custom"></el-option>
					</el-select>
					<el-date-picker v-if="searchForm.dateRange === 'custom'" v-model="searchForm.customDateRange"
						type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期"
						style="margin-left: 10px; width: 300px"></el-date-picker>
				</el-form-item>

				<el-form-item label="搜索范围">
					<el-select v-model="searchForm.searchScope" style="width: 100%">
						<el-option label="所有邮件" value="0"></el-option>
						<el-option label="收件箱" value="1"></el-option>
						<el-option label="已发送" value="2"></el-option>
						<el-option label="草稿箱" value="3"></el-option>
						<el-option label="垃圾箱" value="4"></el-option>
						<el-option label="已归档" value="6"></el-option>
					</el-select>
				</el-form-item>

				<el-form-item>
					<el-checkbox v-model="searchForm.hasAttachment">带有附件</el-checkbox>
				</el-form-item>
			</el-form>

			<template #footer>
				<div>
					<!-- <el-button @click="createSearchFilter">创建过滤器</el-button> -->
					<div style="display: flex; justify-content: flex-end; ">
						<el-button @click="resetSearchForm">重置</el-button>
						<el-button @click="showAdvancedSearch = false">取消</el-button>
						<el-button type="primary" @click="handleAdvancedSearch">搜索</el-button>
					</div>
				</div>
			</template>
		</el-dialog>
		<!-- 配置邮箱 -->
		<el-dialog v-model="ConfigEmaildialog" title="配置邮箱" :close-on-click-modal=false style="width: 35%;"
			@close="handleConfigEmailDialogClose">
			<el-form class="mt10" :model="ConfigEmailForm" label-width="250px">
				<el-form-item label="邮箱地址">
					<el-input v-model="ConfigEmailForm.email" style="width: 300px" @input="handleEmailInput"></el-input>
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
				<span>
					<el-button size="default" @click="ConfigEmaildialog = false">取消</el-button>
					<el-button size="default" type="primary" @click="ConfigUserEmail">保存</el-button>
					<el-button size="default" type="danger" @click="UnbindUserEmail"
						v-if="ConfigEmailForm.id != 0">解绑</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 新建标签 -->
		<el-dialog v-model="showNewFolderDialog" title="新建标签" width="30%">
			<el-form>
				<el-form-item label="标签名称">
					<el-input v-model="newFolderName" placeholder="请输入标签名称" />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="showNewFolderDialog = false">取消</el-button>
					<el-button type="primary" @click="addNewEmailTag(1)">确定</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 签名设置弹窗 -->
		<el-dialog title="签名设置" v-model="signatureDialog" width="700px">
			<div class="signature-container">
				<!-- 签名编辑区域 -->
				<div class="signature-content">
					<QuillEditor v-model:content="currentSignature.content" contentType="html"
						:toolbar="signatureToolbar" theme="snow" style="height: 200px" />
				</div>

				<!-- 签名设置 -->
				<div class="signature-settings">
					<el-checkbox v-model="currentSignature.enabled">
						启用签名
					</el-checkbox>
				</div>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button @click="signatureDialog = false">取 消</el-button>
					<el-button type="primary" @click="saveSignatureSettings">保 存</el-button>
				</div>
			</template>
		</el-dialog>
		<!-- 设置提醒 -->
		<el-dialog v-model="reminderDialogVisible" title="设置提醒" width="30%" :close-on-click-modal="false">
			<div class="reminder-dialog">
				<el-form :model="reminderForm" label-width="100px">
					<el-form-item label="项目分类">
						<el-input v-model="reminderForm.title" type="text" placeholder="请输入项目分类" disabled></el-input>
					</el-form-item>
					<el-form-item label="提醒内容">
						<el-input v-model="reminderForm.content" type="text" placeholder="请输入提醒内容"></el-input>
					</el-form-item>
					<el-form-item label="提醒时间">
						<el-date-picker v-model="reminderForm.reminderTime" type="datetime" placeholder="选择提醒时间"
							format="YYYY-MM-DD HH:mm" value-format="YYYY-MM-DD HH:mm:ss"></el-date-picker>
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="reminderDialogVisible = false">取消</el-button>
					<el-button type="primary" @click="setReminder">确定</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 商机选择 -->
		<el-dialog v-model="BusinessOpportunitySelectionDialog" title="商机选择" width="30%">
			<div class="reminder-dialog">
				<el-form :model="BusinessOpportunityForm" label-width="100px">
					<!-- 商机选择（所有情况都显示） -->
					<el-form-item label="商机编号">
						<el-select v-model="BusinessOpportunityForm.opportunityId" clearable filterable
							placeholder="请选择商机编号" @change="handleOpportunityChange">
							<el-option v-for="item in BusinessOpportunityList" :key="item.id"
								:label="item.opportunityNumber" :value="item.id">
							</el-option>
						</el-select>
					</el-form-item>

					<!-- 报价单选择（仅在初次报价和再次报价时显示） -->
					<el-form-item v-if="BusinessOpportunityForm.type === 'quotation'" label="报价单号">
						<el-select v-model="BusinessOpportunityForm.quotationId" clearable filterable
							placeholder="请选择报价单号">
							<el-option v-for="item in quotationList" :key="item.id" :label="item.quotationNum"
								:value="item.id">
							</el-option>
						</el-select>
					</el-form-item>

					<!-- 合同选择（仅在合同确定时显示） -->
					<el-form-item v-if="BusinessOpportunityForm.type === 'contract'" label="合同编号">
						<el-select v-model="BusinessOpportunityForm.contractId" clearable filterable
							placeholder="请选择合同编号">
							<el-option v-for="item in contractList" :key="item.id" :label="item.contractNumber"
								:value="item.id">
							</el-option>
						</el-select>
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="BusinessOpportunitySelectionDialog = false">取消</el-button>
					<el-button type="primary" @click="handleConfirmSelection">确定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue'
import { ElMessage, ElMessageBox, ElLoading, checkboxEmits } from 'element-plus'
import {
	Menu as IconMenu, Message, Setting, EditPen, Delete, Position, Search, FullScreen, Rank, Close,
	ArrowDown,
	User,
	SwitchButton, Operation, Plus,
	Box, Share, DocumentAdd
} from '@element-plus/icons-vue';
import { closePage } from '@/plugins/tab';
import { QuillEditor } from '@vueup/vue-quill';
import '@vueup/vue-quill/dist/vue-quill.snow.css';
import request from '@/utils/request'
import DOMPurify from 'dompurify'
import { h } from 'vue'
import { ElSelect, ElOption } from 'element-plus'
import { stringify } from 'qs';
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import { id } from 'element-plus/es/locale';

// #region 商机选择
const BusinessOpportunitySelectionDialog = ref(false);
const BusinessOpportunityList = ref([]);
const quotationList = ref([]);
const contractList = ref([]);
const BusinessOpportunityForm = ref({
	opportunityId: null,
	quotationId: null,
	contractId: null,
	type: '', // 'quotation', 'contract', 'communication'
	tagNames: []
});

// 打开商机选择对话框
const openBusinessOpportunitySelectionDialog = async (type, tagNames) => {
	BusinessOpportunityForm.value = {
		opportunityId: null,
		quotationId: null,
		contractId: null,
		type,
		tagNames
	};

	try {
		// 获取商机下拉框数据
		const listResponse = await request({
			url: '/BusinessOpportunity/GetBusinessOpportunityListByUser/GetBusinessOpportunityList',
			method: 'GET'
		});
		if (listResponse.code === 200 && listResponse.data?.length > 0) {
			// 如果是报价或合同类型，需要获取相关列表
			await handleOpportunityChange(listResponse.data[0].id);
			BusinessOpportunityList.value = listResponse.data;
			BusinessOpportunitySelectionDialog.value = true;
		} else {
			ElMessage.warning('未找到相关商机');
		}
	} catch (error) {
		console.error('获取商机列表失败:', error);
		ElMessage.error('获取商机列表失败');
	}
}

// 获取商机列表
const GetBusinessOpportunityList = async () => {
	try {
		const response = await request({
			url: '/BusinessOpportunity/GetBusinessOpportunityListByUser/GetBusinessOpportunityList',
			method: 'GET'
		});
		if (response.code === 200) {
			BusinessOpportunityList.value = response.data;
		}
	} catch (error) {
		console.error('获取商机列表失败:', error);
		ElMessage.error('获取商机列表失败');
	}
}

// 商机选择变更时的处理
const handleOpportunityChange = async (opportunityId) => {
	if (!opportunityId) return;
	try {
		if (BusinessOpportunityForm.value.type === 'quotation') {
			// 获取报价单列表
			const response = await request({
				url: '/Quotation/GetQuotaionListByUser/GetQuotaionList',
				method: 'GET'
			});
			if (response.code === 200) {
				quotationList.value = response.data;
			}
		} else if (BusinessOpportunityForm.value.type === 'contract') {
			// 获取合同列表
			const response = await request({
				url: '/Contracts/GetContractListByUser/GetContractList',
				method: 'GET'
			});
			if (response.code === 200) {
				contractList.value = response.data;
			}
		}
	} catch (error) {
		console.error('获取关联数据失败:', error);
		ElMessage.error('获取关联数据失败');
	}
}
// endregion 商机选择

// 定义系统标签列表
const systemTags = [
	'开发信',
	'询盘',
	'初次报价',
	'沟通需求',
	'再次报价',
	'合同确定'
]

// 检查是否为系统标签
const isSystemTag = (tagName) => {
	return systemTags.includes(tagName)
}

// 修改编辑草稿方法，确保设置草稿ID
const editDraft = async () => {
	try {
		dialogTitle.value = '编辑草稿'

		// 设置表单数据，包括草稿ID
		emailForm.draftId = currentEmail.value.id  // 保存草稿ID
		emailForm.ToEmail = currentEmail.value.to ? parseEmailList(currentEmail.value.to) : []
		emailForm.cc = currentEmail.value.cc ? parseEmailList(currentEmail.value.cc) : []
		emailForm.subject = currentEmail.value.subject || ''
		emailForm.content = currentEmail.value.content || ''

		// 处理附件
		if (currentEmail.value.attachments?.length) {
			fileList.value = currentEmail.value.attachments.map(attachment => ({
				name: attachment.name,
				size: attachment.size,
				uid: Date.now() + Math.random().toString(36).substr(2, 9),
				fileUrl: attachment.fileUrl,
				id: attachment.id
			}))
		} else {
			fileList.value = []
		}

		await GetEmailContract()
		showEmailDetail.value = false
		showEmailDialog.value = true
	} catch (error) {
		console.error('编辑草稿失败:', error)
		ElMessage.error('编辑草稿失败，请重试')
	}
}

// 控制 dialog 显示
const showEmailDialog = ref(false)

// 邮件表单数据
const emailForm = reactive({
	draftId: null,
	ToEmail: [],
	cc: [],
	bcc: [],
	subject: '',
	content: '',
	emailTags: [],
	originalMessageId: null
})

// 添加暂存相关的状态
const isSavingDraft = ref(false)

// 暂存草稿方法
const saveDraft = async () => {
	// 基本验证
	if (!emailForm.ToEmail?.length && !emailForm.subject && !emailForm.content) {
		ElMessage.warning('邮件内容为空，无需保存')
		return
	}
	try {
		isSavingDraft.value = true
		// 处理附件
		let attachments = []
		if (fileList.value && fileList.value.length > 0) {
			attachments = await Promise.all(fileList.value.map(async (file) => {
				// 如果文件已经上传过并有 fileUrl，直接使用
				if (file.fileUrl) {
					return {
						FileName: file.name,
						FileSize: file.size,
						FileUrl: file.fileUrl
					}
				}

				// 如果是新文件，需要先转换为 Base64
				if (file.raw) {
					const base64Content = await convertFileToBase64(file.raw)
					return {
						FileName: file.name,
						FileSize: file.size,
						FileContent: base64Content,
						ContentType: file.raw.type
					}
				}

				// 如果既没有 fileUrl 也不是新文件，返回基本信息
				return {
					FileName: file.name,
					FileSize: file.size
				}
			}))
		}
		// 构建要发送的数据
		const draftData = {
			DraftId: emailForm.draftId,
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc,
			BccEmail: emailForm.bcc,
			Subject: emailForm.subject,
			EmailContent: emailForm.content,
			Attachments: attachments
		}
		const response = await request({
			url: 'Email/SaveDraftEmail/SaveDraftEmail',
			method: 'POST',
			data: draftData
		})

		if (response.code === 200) {
			ElMessage.success('草稿保存成功')
			// 可选：刷新草稿箱
			if (activeMenu.value === '3') { // 假设 3 是草稿箱的 menu id
				await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
			}
		} else {
			ElMessage.error(response.msg || '保存草稿失败')
		}
	} catch (error) {
		console.error('保存草稿失败:', error)
		ElMessage.error('保存草稿失败，请重试')
	} finally {
		isSavingDraft.value = false
	}
}

// 添加自动保存功能（可选）
let autoSaveTimer = null
const AUTO_SAVE_INTERVAL = 900000 // 900秒

const startAutoSave = () => {
	stopAutoSave() // 先清除之前的定时器
	autoSaveTimer = setInterval(async () => {
		if (emailForm.ToEmail?.length || emailForm.subject || emailForm.content) {
			ElMessage.info('自动保存中...')
			await saveDraft()
		}
	}, AUTO_SAVE_INTERVAL)
}

const stopAutoSave = () => {
	if (autoSaveTimer) {
		clearInterval(autoSaveTimer)
		autoSaveTimer = null
	}
}

// 在对话框打开时启动自动保存，关闭时停止
watch(showEmailDialog, (newVal) => {
	if (newVal) {
		startAutoSave()
	} else {
		stopAutoSave()
	}
})

// 在组件卸载时清理定时器
onUnmounted(() => {
	stopAutoSave();
})

// 在组件挂载时获取联系人数据
onMounted(async () => {
	try {
		await fetchEmailList(currentPage.value, pageSize.value)
		await GetEmailContract() // 获取联系人数据
	} catch (error) {
		console.error('初始化数据失败:', error)
	}
})

const originalEmailData = ref([]) // 用于存储原始邮件列表

// 本地搜索处理函数
const handleLocalSearch = () => {
	if (!input1.value) {
		EmailTableData.value = [...originalEmailData.value]
		return
	}

	const searchTerm = input1.value.toLowerCase()
	EmailTableData.value = originalEmailData.value.filter(email => {
		return (
			email.subject?.toLowerCase().includes(searchTerm) ||
			email.name?.toLowerCase().includes(searchTerm) ||
			email.fromEmailAddress?.toLowerCase().includes(searchTerm) ||
			email.toEmail?.toLowerCase().includes(searchTerm)
		)
	})
}

// 清除搜索
const clearLocalSearch = () => {
	EmailTableData.value = [...originalEmailData.value]
}


// 签名相关的响应式变量
const currentSignature = ref({
	id: 0,
	name: '默认签名',
	content: '',
	enabled: false
})

// 打开签名设置对话框时获取签名
const openSignatureDialog = async () => {
	await getUserSignature();
	signatureDialog.value = true
}

// 获取用户签名
const getUserSignature = async () => {
	try {
		const response = await request({
			url: 'Email/GetUserEmailSignature/GetUserEmailSignature',
			method: 'GET'
		})
		if (response.data.length > 0) {
			currentSignature.value.id = response.data[0].id;
			currentSignature.value.content = response.data[0].signature_content;
			currentSignature.value.enabled = response.data[0].isEnable == 1 ? true : false;
		}
	} catch (error) {
		console.error('获取签名失败:', error)
		ElMessage.error('获取签名失败')
	}
}

// 保存签名设置
const saveSignatureSettings = async () => {
	try {
		const signatureData = {
			ID: currentSignature.value.id,
			signature_name: '默认签名',
			signature_content: currentSignature.value.content,
			isEnable: currentSignature.value.enabled ? 1 : 0
		}
		const response = await request({
			url: 'Email/UpdateUserEmailSignature/UpdateUserEmailSignature',
			method: 'POST',
			data: signatureData
		})
		if (response.code === 200) {
			ElMessage.success('保存成功')
			signatureDialog.value = false
		} else {
			ElMessage.error(response.msg || '保存失败')
		}
	} catch (error) {
		console.error('保存签名失败:', error)
	}
}

// 签名编辑器工具栏配置
const signatureToolbar = [
	['bold', 'italic', 'underline', 'strike'],
	[{ 'font': [] }],
	[{ 'size': ['small', false, 'large', 'huge'] }],
	[{ 'color': [] }, { 'background': [] }],
	[{ 'align': [] }],
	['link'],
	['clean']
]

const signatureDialog = ref(false);
const signatures = ref([]);
const defaultNewSignature = ref('none');
const defaultReplySignature = ref('none');
const insertBeforeQuoted = ref(false);

const createSignature = () => {

}

const editSignature = (signature) => {

}

// 菜单切换
const MenuClick = async (menuIndex) => {
	if (await CheckShowEmailDetail() == false) {
		return;
	}
	showEmailDetail.value = false
	EmailTagIndex.value = 0
	folderName.value = null
	activeMenu.value = menuIndex
	currentPage.value = 1

	// 清除搜索状态
	isSearchMode.value = false
	lastSearchParams.value = null

	// 获取邮件列表
	getInboxEmail(currentPage.value, pageSize.value, menuIndex)
}

// 添加以下方法到 setup 中
const getRowTags = (tagsString) => {
	if (!tagsString) return [];
	return tagsString.split(',').filter(tag => tag && !isNaN(tag)).map(Number);
}

// 添加这些方法到 setup 中
const getTagName = (tagId) => {
	const tag = EmailTagcheckboxoptions.value.find(t => t.value === tagId)
	return tag ? tag.label : ''
}

// 可以根据标签ID返回不同的类型，实现标签的多彩效果
// const getTagType = (tagId) => {
// 	//EditEmailTags();
// 	const types = ['', 'success', 'warning', 'info', 'danger']
// 	return types[tagId % types.length]
// }

// 标签过滤列表
const filterByTag = async (tagId) => {
	if (await CheckShowEmailDetail() == false) {
		return;
	}
	try {
		showEmailDetail.value = false
		// 设置当前选中的标签ID
		EmailTagIndex.value = tagId.toString();
		activeMenu.value = `tag-${tagId}`;
		// 重置页码
		currentPage.value = 1;
		// 获取带有该标签的邮件列表
		await getInboxEmail(currentPage.value, pageSize.value, 1);

	} catch (error) {
		console.error('按标签过滤邮件失败:', error);
		ElMessage.error('获取标签邮件失败');
	}
}

const isLoadingTags = ref(false)

// 在复选框变化时调用
const handleTagChange = async (value) => {
	if (isLoadingTags.value) return;

	// 获取最后一个改变的标签ID
	const lastChangedTagId = value[value.length - 1];
	// 判断是选中还是取消选中
	const isChecked = EmailTagcheckboxGroup.value.includes(lastChangedTagId);

	if (!isChecked) {
		// 取消选中，直接调用EditEmailTags
		EditEmailTags();
	} else {
		// 选中标签，调用SelectEmailTags
		SelectEmailTags();
	}
}

// 确认选择
const handleConfirmSelection = async () => {
	// 验证必填项
	if (!BusinessOpportunityForm.value.opportunityId) {
		ElMessage.warning('请选择商机编号');
		return;
	}

	if (BusinessOpportunityForm.value.type === 'quotation' && !BusinessOpportunityForm.value.quotationId) {
		ElMessage.warning('请选择报价单号');
		return;
	}

	if (BusinessOpportunityForm.value.type === 'contract' && !BusinessOpportunityForm.value.contractId) {
		ElMessage.warning('请选择合同编号');
		return;
	}

	try {
		// 根据不同类型更新销售阶段
		let salesStage = '';
		let docuementID = 0;
		if (BusinessOpportunityForm.value.type === 'quotation') {
			salesStage = BusinessOpportunityForm.value.tagNames.includes('初次报价') ? '初次报价' : '再次报价';
			docuementID = BusinessOpportunityForm.value.quotationId;
		} else if (BusinessOpportunityForm.value.type === 'contract') {
			salesStage = '合同确定';
			docuementID = BusinessOpportunityForm.value.contractId;
		} else if (BusinessOpportunityForm.value.type === 'communication') {
			salesStage = '沟通需求';
			docuementID = 0;
		}
		// 更新商机销售阶段
		const response = await request({
			url: '/BusinessOpportunity/UpdateBusinessOpportunitySalesStage/UpdateSalesStage',
			method: 'GET',
			params: {
				BusinessOpportunityID: BusinessOpportunityForm.value.opportunityId,
				SalesStage: salesStage,
				DocuementID: docuementID
			}
		});

		if (response.code === 200) {
			// 更新标签和关联信息
			await EditEmailTags();
			BusinessOpportunitySelectionDialog.value = false;
			ElMessage.success('设置成功');
		} else {
			ElMessage.error(response.msg || '更新销售阶段失败');
		}
	} catch (error) {
		console.error('设置失败:', error);
		ElMessage.error('设置失败');
	}
}

const SelectEmailTags = () => {
	EmailModel.EmailTags = EmailTagcheckboxGroup.value.toString();
	EmailModel.EmailTagNames = EmailTagcheckboxGroup.value.map(tagId => getTagName(tagId)).join(',');
	// 根据标签名称判断需要打开的对话框类型
	if (EmailModel.EmailTagNames.includes('初次报价')) {
		openBusinessOpportunitySelectionDialog('quotation', ['初次报价']);
		return;
	}
	if (EmailModel.EmailTagNames.includes('再次报价')) {
		openBusinessOpportunitySelectionDialog('quotation', ['再次报价']);
		return;
	}
	if (EmailModel.EmailTagNames.includes('合同确定')) {
		openBusinessOpportunitySelectionDialog('contract', ['合同确定']);
		return;
	}
	if (EmailModel.EmailTagNames.includes('沟通需求')) {
		openBusinessOpportunitySelectionDialog('communication', ['沟通需求']);
		return;
	}
	// 如果不是特殊标签，直接更新
	EditEmailTags();
}

const EditEmailTags = () => {
	EmailModel.EmailTags = EmailTagcheckboxGroup.value.toString();
	EmailModel.EmailTagNames = EmailTagcheckboxGroup.value.map(tagId => getTagName(tagId)).join(',');
	EmailModel.businessopportunityid = BusinessOpportunityForm.value.opportunityId;
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
		} else {
			console.error('更新邮件标签失败');
		}
	}).catch(error => {
		console.error('更新邮件标签失败！😔错误内容：', error);
	})
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

const initEmailTag = async () => {
	// 清空当前标签数据
	EmailTagcheckboxGroup.value = [];
	UserEmailTagList.value = [];
	newEmailTagName.value = '';

	// 重新获取标签列表
	const response = await request({
		url: 'Email/GetUserEmailTagList/GetUserEmailTag',
		method: 'GET'
	});

	if (response?.data) {
		// 更新标签选项
		EmailTagcheckboxoptions.value = response.data.map(item => ({
			label: item.emailTagName,
			value: item.id
		}));
		UserEmailTagList.value = response.data;
	}
}
const newEmailTagName = ref('');
const addNewEmailTag = async (type) => {
	var nameStr = '';
	if (type == 1) {
		nameStr = newFolderName.value;
		// 关闭新建文件夹对话框
		showNewFolderDialog.value = false;
	}
	else {
		nameStr = newEmailTagName.value;
	}
	if (nameStr == '') {
		ElMessage({
			message: '标签名称不能为空！😒',
			type: 'warning'
		})
		return;
	} try {
		const response = await request({
			url: 'Email/AddUserEmailTag/AddUserEmailTag',
			method: 'POST',
			data: {
				EmailTagName: nameStr,
				Type: 0
			}
		});
		if (response.code == 200) {
			ElMessage({
				message: response.msg,
				type: 'success'
			});
			// 重新初始化标签列表
			await initEmailTag();
			// 清空输入框
			if (type === 1) {
				newFolderName.value = '';
			} else {
				newEmailTagName.value = '';
			}
		} else {
			ElMessage.error('添加新标签失败');
		}
	} catch (error) {
		ElMessage.error('添加失败，请重试');
	}
}

// 删除标签的方法
const deleteEmailTag = async (tagId) => {
	try {
		await ElMessageBox.confirm('确定要删除此标签吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		});

		const response = await request({
			url: 'Email/DelUserEmailTag/DelUserEmailTag',
			method: 'Delete',
			data: { id: tagId }
		});
		if (response.code == 200) {
			ElMessage.success(response.msg);
			initEmailTag();
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('删除标签失败:', error);
			ElMessage.error('删除标签失败');
		}
	}
}


// 自定义文件夹列表
const customFolders = ref([])

// 新建文件夹对话框控制
const showNewFolderDialog = ref(false)
const newFolderName = ref('')


// 处理移动邮件命令
const handleMoveEmail = async (command) => {
	try {
		// 保存当前的菜单状态
		const currentMenuState = activeMenu.value;
		// 移动邮件
		await MoveEmail(command);

		// 返回列表，但不要让backToList改变菜单状态
		showEmailDetail.value = false;

		// 刷新当前列表数据
		await getInboxEmail(currentPage.value, pageSize.value, currentMenuState);

	} catch (error) {
		console.error('移动邮件失败:', error)
		ElMessage.error('移动邮件失败')
	}
}

//移动邮件
const MoveEmail = (emailType) => {
	EmailModel.emailType = emailType;
	request({
		url: 'Email/MoveEmail/MoveEmail',
		method: 'POST',
		data: EmailModel
	}).then(response => {
		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			//getInboxEmail(currentPage.value, pageSize.value, emailType);
		} else {
			console.error('移动邮件失败');
		}
	}).catch(error => {
		console.error('移动邮件失败！😔错误内容：', error);
	})
}

const folderName = ref(null);
// 过滤分类邮件
const filterByFolder = async (folderId) => {
	if (await CheckShowEmailDetail() == false) {
		return;
	}
	try {
		showEmailDetail.value = false
		// 设置当前选中的邮件分类ID
		activeMenu.value = `folder-${folderId}`;
		// 重置页码
		currentPage.value = 1;
		folderName.value = folderId;
		// 获取该文件夹的邮件
		await getInboxEmail(currentPage.value, pageSize.value, 1);

	} catch (error) {
		console.error('过滤分类邮件失败:', error);
		ElMessage.error('获取分类邮件失败');
	}
}

// 获取自定义文件夹列表
const fetchCustomFolders = async () => {
	try {
		const response = await request({
			url: 'Email/GetMailClassificationList/MailClassification',
			method: 'GET'
		})
		if (response.data) {
			if (response.data.length > 0) {
				customFolders.value = response.data;
				var index = 0;
				customFolders.value.forEach(item => {
					index++;
					item.id = item.fromEmail;
					item.fromEmail = GetFromEmailName(item.fromEmail);
				});
			}
		}
	} catch (error) {
		console.error('获取文件夹列表失败:', error)
	}
}

// 在组件挂载时获取文件夹列表
onMounted(async () => {
	await fetchCustomFolders()
})

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
	"EmailTagNames": '',
	"businessopportunityid": 0
});

const activeMenu = ref('1');

const GetFromEmailName = (fromEmail) => {
	if (!fromEmail) return '未知';

	// 匹配双引号中的内容
	const nameMatch = fromEmail.match(/"([^"]+)"/);
	if (nameMatch && nameMatch[1]) {
		return nameMatch[1];
	}

	// 如果没有引号（纯邮箱地址），则返回@前的部分
	const emailOnlyMatch = fromEmail.match(/^([^@]+)@/);
	if (emailOnlyMatch && emailOnlyMatch[1]) {
		return emailOnlyMatch[1];
	}

	// 如果都没匹配到，返回原始值
	return fromEmail;
}

// 添加 HTML 净化函数
const sanitizeHtml = (html) => {
	if (!html) return ''
	return DOMPurify.sanitize(html, {
		ALLOWED_TAGS: [
			'a', 'b', 'br', 'div', 'em', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
			'i', 'img', 'li', 'ol', 'p', 'span', 'strong', 'table', 'tbody',
			'td', 'th', 'thead', 'tr', 'ul', 'blockquote'
		],
		ALLOWED_ATTR: [
			'href', 'src', 'alt', 'title', 'style', 'target', 'class'
		]
	})
}

// 添加邮件内容样式控制
const emailContentStyle = computed(() => ({
	fontSize: '14px',
	lineHeight: '1.6',
	color: '#333',
	fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif'
}))

const EmailTableData = ref([]);
const IsEditUserEmailConfig = ref(false);
const selectedIndex = ref(null);
const inboxEmailTable = ref([]);
const EmailInboxArray = ref([]);
const EmailTagIndex = ref(0);
const ConfigEmaildialog = ref(false);
const ConfigEmailForm = reactive({
	id: 0,
	userID: 0,
	email: '',
	password: '',
	smtpPort: 0,
	imapPort: 0,
	emailSendServer: '',
	emailReceiveServer: ''
})

// 初始化邮箱配置
const initEmailConfig = async () => {
	const response = await checkUserEmailConfig();
	if (response && response.data) {
		ConfigEmailForm.id = response.data.id;
		ConfigEmailForm.userID = response.data.userID;
		ConfigEmailForm.email = response.data.userEmail;
		ConfigEmailForm.password = response.data.userEmailAuth;
		ConfigEmailForm.emailSendServer = response.data.userEmailSendServer;
		ConfigEmailForm.smtpPort = response.data.userEmailSendPort;
		ConfigEmailForm.emailReceiveServer = response.data.userEmailReceiveServer;
		ConfigEmailForm.imapPort = response.data.userEmailReceivePort;
	}
}
initEmailConfig();

const openUserEmailConfigDialog = async () => {
	try {
		// 显示加载指示器
		const loading = ElLoading.service({
			lock: true,
			text: '正在获取邮箱配置...',
			background: 'rgba(0, 0, 0, 0.7)',
		})

		// 强制重新获取最新配置
		const response = await request({
			url: 'Email/GetEmailConfigByUser/GetEmailConfigByUser',
			method: 'GET',
			params: { timestamp: new Date().getTime() } // 添加时间戳避免缓存
		})

		loading.close()

		if (response && response.data) {
			// 更新所有字段，确保完整加载
			ConfigEmailForm.id = response.data.id || 0
			ConfigEmailForm.userID = response.data.userID || 0
			ConfigEmailForm.email = response.data.userEmail || ''
			ConfigEmailForm.password = response.data.userEmailAuth || ''
			ConfigEmailForm.emailSendServer = response.data.userEmailSendServer || ''
			ConfigEmailForm.smtpPort = response.data.userEmailSendPort || 0
			ConfigEmailForm.emailReceiveServer = response.data.userEmailReceiveServer || ''
			ConfigEmailForm.imapPort = response.data.userEmailReceivePort || 0
			IsEditUserEmailConfig.value = true
			ConfigEmaildialog.value = true
		} else {
			IsEditUserEmailConfig.value = false
			ConfigEmaildialog.value = true
			ElMessage.warning('未能获取邮箱配置数据')
		}
	} catch (error) {
		console.error('获取邮箱配置失败:', error)
		ElMessage.error('获取邮箱配置失败')
		ConfigEmaildialog.value = true
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

function ConfigUserEmail() {
	const loading = ElLoading.service({
		lock: true,
		text: '正在配置邮箱并获取邮件，请稍等.....',
		background: 'rgba(0, 0, 0, 0.7)',
	})
	try {
		var requestData = {
			id: ConfigEmailForm.id,
			UserID: ConfigEmailForm.userID,
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
						message: response.msg,
						type: 'success'
					})
					getInboxEmail(currentPage.value, pageSize.value, 1);
					initEmailTag();
					fetchCustomFolders();
				} else {
					console.error('No data in response');
				}
			}).catch(error => {
				console.error('邮箱配置失败:', error);
			}).finally(() => {
				// 在请求完成之后，无论成功或失败，关闭加载动画
				loading.close();
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
						message: response.msg,
						type: 'success'
					})
					getInboxEmail(currentPage.value, pageSize.value, 1);
					initEmailTag();
					fetchCustomFolders();
				} else {
					console.error('No data in response');
				}
			}).catch(error => {
				console.error('邮箱配置失败:', error);
			}).finally(() => {
				// 在请求完成之后，无论成功或失败，关闭加载动画
				loading.close();
			});
		}
	} catch (error) {
		loading.close();
		console.error('邮箱配置失败:', error);
	}
}

function UnbindUserEmail() {
	// 弹出确认对话框，提示用户确认解绑操作
	ElMessageBox.confirm('确定要解绑当前邮箱吗？解绑后将无法收发邮件。', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		try {
			// 显示加载动画
			const loading = ElLoading.service({
				lock: true,
				text: '正在解绑邮箱，请稍等...',
				background: 'rgba(0, 0, 0, 0.7)',
			})

			var requestData = {
				id: ConfigEmailForm.id,
				UserEmail: ConfigEmailForm.email,
				UserEmailSendServer: ConfigEmailForm.emailSendServer,
				UserEmailReceiveServer: ConfigEmailForm.emailReceiveServer,
				UserEmailAuth: ConfigEmailForm.password,
				UserEmailSendPort: ConfigEmailForm.smtpPort,
				UserEmailReceivePort: ConfigEmailForm.imapPort,
				UserID: 0
			}

			// 调用解绑邮箱API
			const response = await request({
				url: 'Email/UnbindEmail/UnbindEmail',
				method: 'POST',
				data: requestData
			})

			loading.close()

			if (response.code === 200) {
				// 解绑成功，显示成功消息
				ElMessage.success(response.msg || '解绑邮箱成功')

				// 清空所有邮件相关数据
				EmailTableData.value = []
				originalEmailData.value = []
				totalItems.value = 0

				// 清空标签相关数据
				UserEmailTagList.value = []
				EmailTagcheckboxoptions.value = []
				EmailTagcheckboxGroup.value = []

				// 清空文件夹数据
				customFolders.value = []

				// 重置表单数据
				ConfigEmailForm.id = 0
				ConfigEmailForm.userID = 0
				ConfigEmailForm.email = ''
				ConfigEmailForm.password = ''
				ConfigEmailForm.emailSendServer = ''
				ConfigEmailForm.emailReceiveServer = ''
				ConfigEmailForm.smtpPort = 0
				ConfigEmailForm.imapPort = 0

				// 重置当前邮件详情
				showEmailDetail.value = false
				currentEmail.value = {
					id: '',
					subject: '',
					from: '',
					to: '',
					cc: '',
					date: '',
					content: '',
					attachments: []
				}

				// 关闭配置对话框
				ConfigEmaildialog.value = false

				// 重置邮箱配置状态
				IsEditUserEmailConfig.value = false

				// 关闭当前邮箱页面并返回首页
				closePage().then(() => {
					console.log('关闭邮箱页面')
				})
			} else {
				// 解绑失败，显示错误消息
				ElMessage.error(response.msg || '解绑邮箱失败')
			}
		} catch (error) {
			// 捕获并处理异常
			console.error('解绑邮箱失败:', error)
			ElMessage.error('解绑邮箱失败，请重试')
		}
	}).catch(() => {
		// 用户取消操作，不做任何处理
	})
}

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
			ConfigEmaildialog.value = false;
			getInboxEmail(currentPage.value, pageSize.value, 1);
			initEmailTag();
			fetchCustomFolders();
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
function getInboxEmail(start, end, emailType) {
	activeMenu.value = emailType;
	EmailTableData.value = [];
	return new Promise((resolve, reject) => {
		request({
			url: 'Email/GetEmailInboxList/GetInbox',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				EmailType: emailType,
				EmailTagIndex: EmailTagIndex.value,
				floderName: folderName.value
			}
		}).then(response => {
			if (response.data.result.length > 0) {
				// 清空数组
				EmailTableData.value = []
				const processedEmails = response.data.result.map(item => ({
					id: item.id,
					subject: item.emailsubject,
					date: item.emaildate,
					name: GetFromEmailName(item.fromEmail),
					tags: item.emailtags,
					content: item.emailContent,
					emailTags: item.emailTags,
					toEmail: item.toEmail,
					ccEmail: item.ccEmail,
					fromEmailAddress: item.fromEmailAddress,
					EmailID: item.emailID,
					hasAttachments: item.isAttachments === 1,
					isRead: item.isRead
				}))

				EmailTableData.value = processedEmails
				originalEmailData.value = [...processedEmails] // 保存原始数据
				totalItems.value = response.data.totalNum
				selectedIndex.value = ref(null)
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					getInboxEmail(start - 1, end, 1);
				}
				EmailInboxArray.value = null;
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}


// 控制高级搜索对话框的显示
const showAdvancedSearch = ref(false)

// 高级搜索表单数据
const searchForm = reactive({
	sender: '',
	receiver: '',
	subject: '',
	includeWords: '',
	excludeWords: '',
	dateRange: '1', // 默认1天
	customDateRange: [],
	searchScope: '0', // 默认搜索所有邮件
	hasAttachment: false
})

// 添加重置搜索表单的方法
const resetSearchForm = () => {
	// 重置搜索表单数据
	searchForm.sender = ''
	searchForm.receiver = ''
	searchForm.subject = ''
	searchForm.includeWords = ''
	searchForm.excludeWords = ''
	searchForm.dateRange = '1'
	searchForm.customDateRange = []
	searchForm.searchScope = '0'
	searchForm.hasAttachment = false

	// 清除搜索状态
	isSearchMode.value = false
	lastSearchParams.value = null

	// 重置页码和每页显示数量
	currentPage.value = 1
	pageSize.value = 20  // 或者你设定的默认值
	// 重新获取普通邮件列表
	getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
}

// 添加一个变量来跟踪是否处于搜索结果状态
const isSearchMode = ref(false)
const lastSearchParams = ref(null)

// 处理高级搜索
const handleAdvancedSearch = async () => {
	try {
		// 构建搜索参数
		const searchParams = {
			sender: searchForm.sender,
			receiver: searchForm.receiver,
			subject: searchForm.subject,
			includeWords: searchForm.includeWords,
			excludeWords: searchForm.excludeWords,
			hasAttachment: searchForm.hasAttachment ? 1 : 0,
			searchScope: searchForm.searchScope,
			startDate: null,
			endDate: null
		}

		// 处理日期范围...
		if (searchForm.dateRange === 'custom' && searchForm.customDateRange?.length === 2) {
			searchParams.startDate = searchForm.customDateRange[0]
			searchParams.endDate = searchForm.customDateRange[1]
		} else if (searchForm.dateRange !== 'custom' && searchForm.dateRange) {
			const days = parseInt(searchForm.dateRange)
			searchParams.endDate = new Date()
			searchParams.startDate = new Date(Date.now() - days * 24 * 60 * 60 * 1000)
		}

		// 保存搜索参数
		lastSearchParams.value = searchParams
		isSearchMode.value = true

		// 执行搜索...
		const response = await request({
			url: 'Email/SearchEmailInboxList/SearchEmailList',
			method: 'GET',
			params: searchParams
		})

		if (response.data) {
			// 更新邮件列表...
			EmailTableData.value = response.data.result.map(item => ({
				id: item.id,
				subject: item.emailsubject,
				date: item.emaildate,
				name: GetFromEmailName(item.fromEmail),
				tags: item.emailtags,
				content: item.emailContent,
				emailTags: item.emailTags,
				toEmail: item.toEmail,
				fromEmailAddress: item.fromEmailAddress,
				EmailID: item.emailID,
				hasAttachments: item.isAttachments === 1,
				isRead: item.isRead
			}))
			totalItems.value = response.data.totalNum
			currentPage.value = 1
			showAdvancedSearch.value = false
			ElMessage.success('搜索完成')
		} else {
			ElMessage.warning('未找到匹配的邮件')
		}
	} catch (error) {
		console.error('搜索失败:', error)
		ElMessage.error('搜索失败，请重试')
	}
}

const markAsRead = async (row) => {
	if (!row.id) {
		ElMessage.warning('无法获取邮件ID')
		return
	}
	try {
		if (row.isRead == 1) {
			const response = await request({
				url: 'Email/EditEmailIsRead/EditEmailIsRead',
				method: 'POST',
				data: {
					id: row.id,
					isRead: 0
				}
			})
			if (response.code === 200) {
				row.isRead = 0;
			} else {
				ElMessage.error(response.msg || '标记已读失败')
			}
		}
	} catch (error) {
		console.error('标记邮件为已读失败:', error)
		ElMessage.error('操作失败，请重试')
	}
}

// 标记邮件为未读
const markAsUnread = async (emailId) => {
	if (!emailId) {
		ElMessage.warning('无法获取邮件ID')
		return
	}
	try {
		const response = await request({
			url: 'Email/EditEmailIsRead/EditEmailIsRead',
			method: 'POST',
			data: {
				id: emailId,
				isRead: 1
			}
		})

		if (response.code === 200) {
			ElMessage.success(response.msg)
			// 刷新邮件列表
			await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
		} else {
			ElMessage.error(response.msg || '标记未读失败')
		}
	} catch (error) {
		console.error('标记邮件为未读失败:', error)
		ElMessage.error('操作失败，请重试')
	}
}

// 分页相关的响应式变量
const currentPage = ref(1)
const pageSize = ref(20)
const totalItems = ref(0)


// 处理页码改变
const handleCurrentChange = async (newPage) => {
	try {
		if (isSearchMode.value && lastSearchParams.value) {
			// 在搜索模式下，使用上次的搜索参数，但更新页码
			const searchParams = {
				...lastSearchParams.value,
				PageNum: newPage,
				PageSize: pageSize.value
			}

			const response = await request({
				url: 'Email/SearchEmailInboxList/SearchEmailList',
				method: 'GET',
				params: searchParams
			})

			if (response.data) {
				EmailTableData.value = response.data.result.map(item => ({
					id: item.id,
					subject: item.emailsubject,
					date: item.emaildate,
					name: GetFromEmailName(item.fromEmail),
					tags: item.emailtags,
					content: item.emailContent,
					emailTags: item.emailTags,
					toEmail: item.toEmail,
					fromEmailAddress: item.fromEmailAddress,
					EmailID: item.emailID,
					hasAttachments: item.isAttachments === 1,
					isRead: item.isRead
				}))
				totalItems.value = response.data.totalNum
				currentPage.value = newPage
			}
		} else {
			// 非搜索模式下的普通分页
			await fetchEmailList(newPage, pageSize.value)
		}
	} catch (error) {
		console.error('获取邮件列表失败:', error)
		ElMessage.error('获取邮件列表失败')
	}
}

// 处理每页显示数量改变
const handleSizeChange = async (newSize) => {
	try {
		pageSize.value = newSize
		if (isSearchMode.value && lastSearchParams.value) {
			// 在搜索模式下，使用上次的搜索参数，但更新页码和每页数量
			const searchParams = {
				...lastSearchParams.value,
				PageNum: 1, // 切换每页显示数量时重置为第一页
				PageSize: newSize
			}

			const response = await request({
				url: 'Email/SearchEmailInboxList/SearchEmailList',
				method: 'GET',
				params: searchParams
			})

			if (response.data) {
				EmailTableData.value = response.data.result.map(item => ({
					id: item.id,
					subject: item.emailsubject,
					date: item.emaildate,
					name: GetFromEmailName(item.fromEmail),
					tags: item.emailtags,
					content: item.emailContent,
					emailTags: item.emailTags,
					toEmail: item.toEmail,
					fromEmailAddress: item.fromEmailAddress,
					EmailID: item.emailID,
					hasAttachments: item.isAttachments === 1,
					isRead: item.isRead
				}))
				totalItems.value = response.data.totalNum
				currentPage.value = 1
			}
		} else {
			// 非搜索模式下的普通分页
			currentPage.value = 1
			await fetchEmailList(1, newSize)
		}
	} catch (error) {
		console.error('获取邮件列表失败:', error)
		ElMessage.error('获取邮件列表失败')
	}
}

// 获取邮件列表数据
const fetchEmailList = async (page, size) => {
	getInboxEmail(page, size, activeMenu.value);
}

// 在组件挂载时获取第一页数据
onMounted(async () => {
	try {
		await fetchEmailList(currentPage.value, pageSize.value)
	} catch (error) {
		console.error('初始化邮件列表失败:', error)
	}
})



// 标签相关的响应式变量
const selectedTags = ref([])

// 监听标签选择变化
watch(selectedTags, async (newTags) => {
	try {
		await updateEmailTags(currentEmail.value.id, newTags)
		ElMessage.success('标签更新成功')
		// 更新当前邮件的标签
		currentEmail.value.tags = newTags
	} catch (error) {
		console.error('更新标签失败:', error)
		ElMessage.error('标签更新失败')
	}
})

// 更新邮件标签的方法
const updateEmailTags = async (emailId, tagIds) => {
	// TODO: 实现调用后端 API 更新邮件标签
	return new Promise((resolve) => {
		setTimeout(resolve, 500) // 模拟 API 调用
	})
}

// 添加新的响应式变量
const dialogTitle = ref('新邮件')
const emailType = ref('new') // new, reply, replyAll, forward

// 辅助函数：将邮箱字符串转换为数组
const parseEmailList = (emailStr) => {
	if (!emailStr) return []
	if (Array.isArray(emailStr)) return emailStr
	return emailStr.split(/[,;]/)
		.map(email => email.trim())
		.filter(email => email && validateEmail(email))
}

// 辅助函数：过滤掉重复的邮箱地址
const uniqueEmails = (emails) => {
	return [...new Set(emails)]
}

// 修改后的处理抄送的逻辑
const handleCcList = (ccList) => {
	const currentUserEmail = ConfigEmailForm.email
	return uniqueEmails(parseEmailList(ccList))
		.filter(email => email !== currentUserEmail)
}
// 处理回复邮件
const handleReply = async (replyAll = false) => {
	emailForm.originalMessageId = SelectEmailID;
	emailType.value = replyAll ? 'replyAll' : 'reply'
	dialogTitle.value = replyAll ? '回复全部' : '回复'
	await GetEmailContract() // 获取联系人数据
	// 设置收件人 - 只设置原始发件人
	emailForm.ToEmail = [currentEmail.value.from]
	// 处理抄送
	if (replyAll && currentEmail.value.cc) {
		const ccList = handleCcList(currentEmail.value.cc)
		if (ccList.length > 0) {
			showCc.value = true // 自动显示抄送字段
			emailForm.cc = ccList
		}
	} else {
		showCc.value = false
		emailForm.cc = []
	}
	// 设置主题
	emailForm.subject = `回复: ${currentEmail.value.subject}`
	// 设置内容
	emailForm.content = `
        <br><br>
        <p>------------------ 原始邮件 ------------------</p>
        <p>发件人: ${currentEmail.value.from}</p>
        <p>发送时间: ${currentEmail.value.date}</p>
        <p>主题: ${currentEmail.value.subject}</p>
        <p>收件人: ${currentEmail.value.to}</p>
        ${currentEmail.value.cc ? `<p>抄送: ${currentEmail.value.cc}</p>` : ''}
        ${currentEmail.value.content}
    `
	showEmailDialog.value = true
}

// 修改转发邮件的处理函数
const handleForward = async () => {
	emailType.value = 'forward'
	dialogTitle.value = '转发'
	await GetEmailContract() // 获取联系人数据
	// 清空收件人和抄送
	emailForm.ToEmail = []
	emailForm.cc = []

	// 设置主题
	emailForm.subject = `转发: ${currentEmail.value.subject}`

	// 设置内容
	emailForm.content = `
        <br><br>
        <p>------------------ 转发邮件 ------------------</p>
        <p>发件人: ${currentEmail.value.from}</p>
        <p>发送时间: ${currentEmail.value.date}</p>
        <p>主题: ${currentEmail.value.subject}</p>
        <p>收件人: ${currentEmail.value.to}</p>
        ${currentEmail.value.content}
    `

	// 如果有附件，复制原邮件的附件
	if (currentEmail.value.attachments?.length) {
		// 清空当前附件列表
		fileList.value = []

		// 获取原邮件的附件
		for (const attachment of currentEmail.value.attachments) {
			try {
				// 获取附件内容
				const response = await request({
					url: 'Email/DownloadAttachment/DownloadAttachment',
					method: 'GET',
					params: { id: attachment.id },
					responseType: 'blob'
				})

				// 转换为 Base64
				const base64Content = await blobToBase64(response.data)

				// 添加到文件列表
				fileList.value.push({
					name: attachment.name,
					size: attachment.size,
					uid: Date.now() + Math.random().toString(36).substr(2, 9),
					raw: new File([response.data], attachment.name),
					// 保存 base64 内容以便发送
					base64Content: base64Content
				})
			} catch (error) {
				console.error('获取附件失败:', error)
				ElMessage.warning(`附件 "${attachment.name}" 获取失败`)
			}
		}
	}

	showEmailDialog.value = true
}

// 添加 blob 转 base64 的辅助函数
const blobToBase64 = (blob) => {
	return new Promise((resolve, reject) => {
		const reader = new FileReader()
		reader.onload = () => {
			const base64String = reader.result.toString().split(',')[1]
			resolve(base64String)
		}
		reader.onerror = reject
		reader.readAsDataURL(blob)
	})
}

// 控制详情页显示
const showEmailDetail = ref(false)

// 当前查看的邮件数据
const currentEmail = ref({
	id: '',
	subject: '',
	from: '',
	to: '',
	cc: '',
	date: '',
	content: '',
	attachments: []
})



const downloadAttachment = (id, fileName, fileUrl) => {
	//方法 1: 如果文件URL可以直接访问
	if (fileUrl) {
		const link = document.createElement('a')
		link.href = fileUrl
		link.download = fileName
		document.body.appendChild(link)
		link.click()
		document.body.removeChild(link)
	} else {
		// 方法 2: 如果需要通过API下载
		request({
			url: `Email/DownloadAttachment/DownloadAttachment`,
			method: 'GET',
			params: { id: id },
			responseType: 'blob'  // 重要：设置响应类型为blob
		}).then(response => {
			const blob = new Blob([response.data])
			const link = document.createElement('a')
			link.href = URL.createObjectURL(blob)
			link.download = fileName
			document.body.appendChild(link)
			link.click()
			document.body.removeChild(link)
			URL.revokeObjectURL(link.href)
		}).catch(error => {
			console.error('下载失败:', error)
			ElMessage.error('下载失败，请重试')
		})
	}
}

// 获取邮件附件
const GetEmailAttachment = async (emailId) => {
	const response = await request({
		url: 'Email/GetEmailAttachments/GetEmailAttachments',
		method: 'GET',
		params: {
			emailId: emailId
		}
	});
	console.log(response.data);
	return response.data;
}
const SelectEmailID = ref('');
// 处理行点击，显示邮件详情
const handleRowClick = async (row) => {
	SelectEmailID.value = row.id;
	// 保存当前状态
	const attachmentsList = await GetEmailAttachment(row.EmailID);
	currentEmail.value = {
		id: row.id,
		subject: row.subject,
		from: row.fromEmailAddress,
		to: row.toEmail,
		cc: row.ccEmail || null, // 如果有抄送则显示，没有则为null
		date: row.date,
		content: row.content,
		attachments: attachmentsList.map(attachment => ({
			id: attachment.id,
			name: attachment.attachmentsName,
			fileUrl: attachment.attachmentsDownLoadUrl
		}))
	};

	EmailModel.id = row.id;
	EmailModel.emailsubject = row.subject;
	EmailModel.fromEmail = row.fromEmailAddress;
	isLoadingTags.value = true;
	markAsRead(row); // 标记为已读

	try {
		// 清空当前选中的标签
		EmailTagcheckboxGroup.value = [];
		EmailTagcheckboxoptions.value = []; // 清空标签选项

		// 重新获取标签列表
		const response = await request({
			url: 'Email/GetUserEmailTagList/GetUserEmailTag',
			method: 'GET'
		});

		if (response?.data) {
			// 更新标签选项
			EmailTagcheckboxoptions.value = response.data.map(item => ({
				label: item.emailTagName,
				value: item.id
			}));

			// 如果当前邮件有标签，设置选中状态
			if (row.emailTags) {
				const tagArray = row.emailTags.split(',');
				tagArray.forEach(tagId => {
					if (tagId && !isNaN(tagId)) {
						EmailTagcheckboxGroup.value.push(Number(tagId));
					}
				});
			}
		}
	} catch (error) {
		console.error('获取标签列表失败:', error);
		ElMessage.error('获取标签列表失败');
	} finally {
		isLoadingTags.value = false;
		showEmailDetail.value = true;
	}
}

// 返回列表
const backToList = async (updateMenu = true) => {
	if (await CheckShowEmailDetail() == false) {
		return;
	}
	showEmailDetail.value = false
	if (isSearchMode.value && lastSearchParams.value) {
		// 如果是搜索模式，使用保存的搜索参数和当前页码
		const searchParams = {
			...lastSearchParams.value,
			PageNum: currentPage.value,  // 使用当前页码
			PageSize: pageSize.value
		}

		const response = await request({
			url: 'Email/SearchEmailInboxList/SearchEmailList',
			method: 'GET',
			params: searchParams
		})

		if (response.data) {
			EmailTableData.value = response.data.result.map(item => ({
				id: item.id,
				subject: item.emailsubject,
				date: item.emaildate,
				name: GetFromEmailName(item.fromEmail),
				tags: item.emailtags,
				content: item.emailContent,
				emailTags: item.emailTags,
				toEmail: item.toEmail,
				fromEmailAddress: item.fromEmailAddress,
				EmailID: item.emailID,
				hasAttachments: item.isAttachments === 1,
				isRead: item.isRead
			}))
			totalItems.value = response.data.totalNum
		}
	} else if (updateMenu) {
		// 普通模式，使用当前页码获取邮件列表
		await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
	}
}

const GetEmailContract = async () => {
	return request({
		url: 'Email/GetEmailContact/GetEmailContact',
		method: 'GET'
	}).then(response => {
		recipientTreeData.value = response.data;
	}).catch(error => {
		console.error(error);
	});
}

// 收件人树形数据
const recipientTreeData = ref([])

// 邮箱验证函数
const validateEmail = (email: string) => {
	const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
	return emailRegex.test(email)
}

// 过滤收件人的方法
const filterRecipients = (query: string) => {
	if (query) {
		// 这里可以实现搜索逻辑
		// 如果需要，可以调用后端 API 进行搜索
	}
}

// 文件列表
const fileList = ref([])
// 文件输入引用
const fileInput = ref(null)

// 格式化文件大小
const formatFileSize = (bytes) => {
	if (bytes === 0) return '0 B'
	const k = 1024
	const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
	const i = Math.floor(Math.log(bytes) / Math.log(k))
	return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}


// 控制全屏状态
const isFullscreen = ref(false)

// 切换全屏状态
const toggleFullScreen = () => {
	isFullscreen.value = !isFullscreen.value
}

// Quill 编辑器配置
const editorOptions = {
	modules: {
		toolbar: [
			['bold', 'italic', 'underline', 'strike'],
			['blockquote', 'code-block'],
			[{ 'header': 1 }, { 'header': 2 }],
			[{ 'list': 'ordered' }, { 'list': 'bullet' }],
			[{ 'script': 'sub' }, { 'script': 'super' }],
			[{ 'indent': '-1' }, { 'indent': '+1' }],
			[{ 'direction': 'rtl' }],
			[{ 'size': ['small', false, 'large', 'huge'] }],
			[{ 'header': [1, 2, 3, 4, 5, 6, false] }],
			[{ 'color': [] }, { 'background': [] }],
			[{ 'font': [] }],
			[{ 'align': [] }],
			['clean'],
			['link', 'image']
		]
	},
	placeholder: '撰写邮件...',
}

// 发送新邮件
const sendNewEmail = async () => {
	try {
		// 基本验证
		if (!emailForm.ToEmail.length) {
			ElMessage.warning('请填写收件人')
			return
		}
		if (!emailForm.subject) {
			ElMessage.warning('请填写主题')
			return
		}
		if (!emailForm.emailTags) {
			ElMessage.warning('请选择标签')
			return
		}
		// 验证所有邮箱格式
		const allEmails = [
			...emailForm.ToEmail,
			...(emailForm.cc || []),
			...(emailForm.bcc || [])
		]

		const invalidEmails = allEmails.filter(email => !validateEmail(email))
		if (invalidEmails.length > 0) {
			ElMessage.error(`以下邮箱格式不正确：${invalidEmails.join(', ')}`)
			return
		}
		// 检查并添加签名
		await appendSignatureIfEnabled()
		// 处理新附件
		const attachments = await Promise.all(
			fileList.value.map(async file => {
				if (file.raw) {
					const base64Content = await convertFileToBase64(file.raw)
					return {
						FileName: file.name,
						FileContent: base64Content,
						FileSize: file.size,
						ContentType: file.raw.type
					}
				}
				return null
			})
		).then(results => results.filter(Boolean))

		alert(emailForm.originalMessageId);
		// 构建发送数据
		const emailData = {
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc || [],
			BccEmail: emailForm.bcc || [],
			Subject: emailForm.subject,
			EmailContent: emailForm.content,
			Attachments: attachments,
			EmailTags: emailForm.emailTags,
			EmailTagNames: EmailTagcheckboxoptions.value.find(option => option.value === emailForm.emailTags).label,
			originalMessageId: Number(emailForm.originalMessageId)
		}

		const loading = ElLoading.service({
			lock: true,
			text: '正在发送邮件，请稍候...',
			background: 'rgba(0, 0, 0, 0.7)'
		})

		try {
			const response = await request({
				url: 'Email/SendEmail/send',
				method: 'POST',
				data: emailData
			})

			if (response.code === 200) {
				ElMessage.success('邮件发送成功')
				showEmailDialog.value = false
				resetEmailForm()
				// 如果在已发送文件夹，刷新列表
				if (activeMenu.value === '2') {
					await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
				}
			} else {
				ElMessage.error(response.msg || '发送失败')
			}
		} finally {
			loading.close()
		}
	} catch (error) {
		console.error('发送邮件失败:', error)
		ElMessage.error('发送失败，请重试')
	}
}

// 发送草稿邮件
const sendFromDraft = async () => {
	var loading = null;
	try {
		// 基本验证
		if (!emailForm.ToEmail.length) {
			ElMessage.warning('请填写收件人')
			return
		}
		if (!emailForm.subject) {
			ElMessage.warning('请填写主题')
			return
		}
		// 检查并添加签名
		await appendSignatureIfEnabled()
		// 处理附件信息 - 区分已有附件和新上传的附件
		const attachments = await Promise.all(fileList.value.map(async file => {
			// 如果是已有的附件（从草稿加载的）
			if (file.id && file.fileUrl) {
				return {
					Id: file.id,
					FileName: file.name,
					FileSize: file.size,
					FileUrl: file.fileUrl,
					AttachmentId: file.id
				}
			}

			// 如果是新上传的附件
			if (file.raw) {
				const base64Content = await convertFileToBase64(file.raw)
				return {
					FileName: file.name,
					FileSize: file.size,
					FileContent: base64Content,
					ContentType: file.raw.type
				}
			}

			return null
		})).then(results => results.filter(Boolean))

		// 构建草稿发送数据
		const draftData = {
			DraftId: emailForm.draftId,
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc || [],
			BccEmail: emailForm.bcc || [],
			Subject: emailForm.subject,
			EmailContent: emailForm.content,
			Attachments: attachments,
			EmailTags: emailForm.emailTags,
			EmailTagNames: EmailTagcheckboxoptions.value.find(option => option.value === emailForm.emailTags).label
		}

		loading = ElLoading.service({
			lock: true,
			text: '正在发送草稿邮件，请稍候...',
			background: 'rgba(0, 0, 0, 0.7)'
		})

		const response = await request({
			url: 'Email/SendFromDraft/SendFromDraft',
			method: 'POST',
			data: draftData
		})

		if (response.code === 200) {
			ElMessage.success('草稿邮件发送成功')
			loading.close()
			showEmailDetail.value = false // 触发 watch
			resetEmailForm()
			await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
		} else {
			ElMessage.error(response.msg || '发送失败')
		}

	} catch (error) {
		console.error('发送草稿失败:', error)
		ElMessage.error('发送草稿失败，请重试')
	}
}

// 修改签名添加逻辑，增加验证和位置控制
const appendSignatureIfEnabled = async () => {
	try {
		await getUserSignature()
		if (currentSignature.value.enabled && currentSignature.value.content) {
			const signatureContent = currentSignature.value.content

			// 如果是回复或转发的邮件
			if (emailType.value === 'reply' || emailType.value === 'replyAll' || emailType.value === 'forward') {
				// 查找原始邮件分隔线的位置
				const separatorIndex = emailForm.content.indexOf('------------------ 原始邮件 ------------------')
				const forwardSeparatorIndex = emailForm.content.indexOf('------------------ 转发邮件 ------------------')

				// 获取实际的分隔线位置
				const actualSeparatorIndex = Math.min(
					separatorIndex === -1 ? Infinity : separatorIndex,
					forwardSeparatorIndex === -1 ? Infinity : forwardSeparatorIndex
				)

				if (actualSeparatorIndex !== Infinity) {
					// 在分隔线之前插入签名
					const beforeSeparator = emailForm.content.substring(0, actualSeparatorIndex)
					const afterSeparator = emailForm.content.substring(actualSeparatorIndex)

					// 检查签名是否已存在
					if (!beforeSeparator.includes(signatureContent)) {
						emailForm.content = beforeSeparator + `<br><br>${signatureContent}` + afterSeparator
					}
				}
			} else {
				// 新邮件，直接在末尾添加签名（如果尚未添加）
				if (!emailForm.content.includes(signatureContent)) {
					emailForm.content += `<br><br>${signatureContent}`
				}
			}
		}
	} catch (error) {
		console.error('获取签名失败:', error)
		// 如果获取签名失败，继续发送邮件但不添加签名
	}
}

// 统一的发送邮件入口
const sendEmail = async () => {
	try {
		// 根据是否有草稿ID来决定使用哪个发送方法
		if (emailForm.draftId) {
			await sendFromDraft()
		} else {
			await sendNewEmail()
		}
	} catch (error) {
		console.error('发送邮件失败:', error)
		ElMessage.error('发送失败，请重试')
	}
}

// 文件转Base64函数
const convertFileToBase64 = (file) => {
	return new Promise((resolve, reject) => {
		const reader = new FileReader()
		reader.readAsDataURL(file)
		reader.onload = () => {
			const base64String = reader.result.toString().split(',')[1]
			resolve(base64String)
		}
		reader.onerror = reject
	})
}

// 修改文件上传相关函数
const handleFileChange = (file) => {
	// 可以在这里添加文件类型和大小的验证
	const isLt10M = file.size / 1024 / 1024 < 10
	if (!isLt10M) {
		ElMessage.error('文件大小不能超过 10MB!')
		return false
	}
	return true
}

const handleFileRemove = (file) => {
	const index = fileList.value.findIndex(item => item.uid === file.uid)
	if (index !== -1) {
		fileList.value.splice(index, 1)
	}
}

// 修改触发文件选择的函数
const triggerFileInput = () => {
	fileInput.value.click()
}

const handleFileInputChange = (event) => {
	const files = Array.from(event.target.files)
	files.forEach(file => {
		if (handleFileChange(file)) {
			fileList.value.push({
				name: file.name,
				size: file.size,
				raw: file,
				uid: Date.now() + Math.random().toString(36).substr(2, 9) // 生成唯一ID
			})
		}
	})
	// 清空input，以便可以重复选择同一文件
	event.target.value = ''
}

const showCc = ref(false)
// 修改重置表单函数，确保清空附件
const resetEmailForm = () => {
	emailForm.emailTags = null;
	emailForm.draftId = null
	emailForm.ToEmail = []
	emailForm.cc = []
	emailForm.bcc = []
	emailForm.subject = ''
	emailForm.content = ''
	fileList.value = []
	showCc.value = false
	if (quillEditor.value) {
		quillEditor.value.setContents([])
	}
	isFullscreen.value = false
}

// 丢弃邮件
const discardEmail = () => {
	emailForm.ToEmail = []
	emailForm.cc = []
	emailForm.bcc = []
	emailForm.subject = ''
	emailForm.content = ''
	emailForm.emailTags = null;
	showEmailDetail.value = false // 触发 watch
}

const quillEditor = ref(null) // 添加编辑器引用

// 在关闭对话框时也调用重置函数
const handleDialogClose = () => {
	resetEmailForm()
}

GetEmailContract();
//写邮件的处理函数
const handleWriteEmail = async () => {
	dialogTitle.value = '新邮件'
	emailType.value = 'new'
	resetEmailForm()
	await GetEmailContract() // 获取联系人数据
	showEmailDialog.value = true
}

const input1 = ref('')

// 选中的行数据
const selectedRows = ref([])
// 当前高亮的行
const currentRow = ref(null)

// 选择变化处理
const handleSelectionChange = (selection) => {
	selectedRows.value = selection
}

// 设置行的 class
const tableRowClassName = ({ row }) => {
	const classes = []

	// 添加未读样式
	if (row.isRead === 1) {
		classes.push('unread-row')
	}

	// 保留原有的高亮逻辑
	if (currentRow.value === row) {
		classes.push('highlight-row')
	}
	// 选中行样式
	if (selectedRows.value.some(selected => selected.id === row.id)) {
		classes.push('selected-row')
	}

	return classes.join(' ')
}

// 设置提醒
const reminderDialogVisible = ref(false);
const reminderForm = ref({
	title: '',
	content: '',
	reminderTime: null
})
const openReminderDialog = (email) => {
	reminderForm.value.title = '邮件提醒';
	reminderForm.value.content = email.from + '发来的邮件';
	reminderDialogVisible.value = true;
}
const setReminder = async () => {
	try {
		if (!reminderForm.value.content || !reminderForm.value.reminderTime) {
			ElMessage.warning('请填写完整信息')
			return
		}

		const res = await request({
			url: 'TaskReminder/AddTaskReminder/Add',
			method: 'GET',
			params: {
				userId: '', // 这个参数服务端会自动获取，可以传空
				title: reminderForm.value.title,
				content: reminderForm.value.content,
				reminderTime: reminderForm.value.reminderTime
			}
		})
		if (res.code === 200) {
			ElMessage.success('提醒设置成功')
			reminderDialogVisible.value = false
			// 重置表单

		} else {
			ElMessage.error(res.msg || '设置失败')
		}
	} catch (error) {
		console.error('设置提醒失败:', error)
		ElMessage.error('设置提醒失败：' + (error.message || '未知错误'))
	}
}

// 添加控制 popover 显示的变量
const tagPopoverVisible = ref(false)
const tagPopover = ref(null)
// 检查当前详情页是否显示，如果显示，说明正在浏览邮件，需要判断当前被浏览的邮件有没有标签
// 如果被浏览的邮件有标签，则允许关闭详情页，否则不允许关闭详情页
// 检查邮件方法
const CheckShowEmailDetail = async () => {
	return new Promise(async (resolve) => {
		// 如果详情页没有显示，直接返回true
		if (!showEmailDetail.value) {
			resolve(true)
			return
		}

		// 如果正在查看邮件详情
		if (currentEmail.value) {
			try {
				// 调用检查标签接口
				const res = await request({
					url: 'Email/CheckEmailTagsByEmailID/CheckEmailTags',
					method: 'GET',
					params: {
						id: currentEmail.value.id
					}
				})

				if (res.code === 200 && !res.data) {
					// 如果没有标签，显示提示框
					ElMessageBox.confirm(
						'当前邮件未添加标签，添加标签后才能关闭。是否现在添加标签？',
						'提示',
						{
							confirmButtonText: '去添加标签',
							type: 'warning',
							showCancelButton: false,
							closeOnClickModal: false,
							closeOnPressEscape: false,
							showClose: false
						}
					)
						.then(() => {
							tagPopoverVisible.value = true
							resolve(false)
						})
						.catch(() => {
							resolve(false)
						})
					return
				}
				resolve(true)
			} catch (error) {
				console.error('检查邮件标签失败:', error)
				resolve(true) // 出错时允许关闭
			}
			return
		}
		// 如果没有当前邮件数据，允许关闭
		resolve(true)
	})
}

// 定义标签类型映射
const tagTypeMap = {
	'询盘': 'info',        // 蓝色
	'初次报价': 'success', // 绿色
	'沟通需求': 'warning', // 橙色
	'再次报价': 'danger',  // 红色
	'合同确定': 'primary',        // 主色调
	'开发信': ''    // 主色调	
}
// 获取标签类型的方法
const getTagType = (tagName) => {
	return tagTypeMap[tagName] || '' // 默认返回空字符串（灰色）
}


</script>

<style lang="scss" scoped>
.dialog-footer {
	display: flex;
	justify-content: space-between;
	align-items: center;
}

.left {
	display: flex;
	gap: 8px;
}

.right {
	display: flex;
	gap: 8px;
}

/* 主发送按钮样式 */
.el-button--primary {
	min-width: 80px;
}

/* 下拉按钮组样式 */
.el-dropdown {
	margin-left: 4px;
}

.email-form {
	padding: 0 20px;
}

.form-item {
	margin-bottom: 16px;
	display: flex;
	align-items: flex-start;
}

.form-label {
	width: 70px;
	line-height: 32px;
	flex-shrink: 0;
	color: #606266;
}

.full-width {
	flex: 1;
}

.cc-checkbox {
	margin-left: 70px; // 与其他表单项对齐
	color: #606266;
}

:deep(.el-select) {
	width: 100%;
}

:deep(.el-checkbox) {
	height: 32px;
	line-height: 32px;
}

:deep(.el-select .el-input__wrapper),
:deep(.el-input .el-input__wrapper) {
	line-height: 32px;
}

/* 邮件详情头部样式Start */
.email-header {
	padding: 0px 0px;
	background-color: #fff;
}

.subject {
	font-size: 24px;
	color: #333;
	margin: 0 0 16px 0;
	font-weight: normal;
}

.email-tags {
	margin-bottom: 20px;
	display: flex;
	flex-wrap: wrap;
	gap: 8px;

	.custom-tag {
		padding: 4px 12px;
		border-radius: 4px;
		font-size: 12px;
		font-weight: normal;
		background-color: #f8f8f8;
		border: none;
		color: #666;

		// 不同类型标签的样式
		&.el-tag--success {
			background-color: #f0f9eb;
			color: #67c23a;
		}

		&.el-tag--warning {
			background-color: #fdf6ec;
			color: #e6a23c;
		}

		&.el-tag--info {
			background-color: #f4f4f5;
			color: #909399;
		}
	}
}

.meta-info {
	color: #666;
	font-size: 14px;
	border-top: 1px solid #eee;
	padding-top: 0px;
}

.meta-item {
	display: flex;
	align-items: flex-start;
	line-height: 20px;

	.label {
		width: 70px;
		color: #999;
		text-align: left;
	}

	.value {
		color: #666;
	}
}

/* 邮件详情头部样式End */
.meta-info {
	color: #666;
	font-size: 14px;

	>div {
		margin: 8px 0;
		display: flex;
		align-items: flex-start;
	}

	.label {
		color: #999;
		margin-right: 8px;
		min-width: 60px; // 确保标签对齐
		display: inline-block;
	}

	// 处理长邮件地址的换行
	span:not(.label) {
		word-break: break-all;
		flex: 1;
	}
}

// 如果抄送人较多，可以添加展开/收起功能的样式
.expandable-content {
	max-height: 100px;
	overflow: hidden;
	position: relative;

	&.expanded {
		max-height: none;
	}

	&:not(.expanded)::after {
		content: '';
		position: absolute;
		bottom: 0;
		left: 0;
		right: 0;
		height: 20px;
		background: linear-gradient(transparent, white);
	}
}

:deep(.el-table) {
	.unread-row {
		font-weight: bold;
		background-color: #f8f9fa;

		td {
			.cell {
				color: #303133;
			}
		}
	}

	// 选中行样式
	.selected-row {
		background-color: #ecf5ff !important; // 使用 !important 确保样式优先级

		td {
			background-color: #ecf5ff !important;
		}
	}

	// 鼠标悬停效果
	.el-table__row:hover {
		background-color: #f5f7fa;
	}

	// 已读邮件样式
	.el-table__row:not(.unread-row) {
		color: #606266;
		background-color: #ffffff;
	}

	// // 调整复选框列的样式
	// .el-table-column--selection .cell {
	// 	padding-top: -10px; // 根据实际需要调整padding值
	// 	vertical-align: middle;
	// }

	// // 确保所有单元格垂直居中
	// .el-table__cell {
	// 	vertical-align: middle;
	// }
}

// 签名设置起
.signature-container {
	padding: 20px;
}

.signature-content {
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	margin-bottom: 20px;
}

.signature-settings {
	margin-top: 20px;
}

:deep(.el-checkbox) {
	margin-top: 10px;
}

.dialog-footer {
	display: flex;
	justify-content: flex-end;
	gap: 12px;
}

.signature-editor {
	margin-bottom: 30px;
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	padding: 20px;
}

.signature-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin-bottom: 20px;
}

.setting-item {
	display: flex;
	align-items: center;
	margin-bottom: 15px;

	span {
		width: 120px;
		color: #606266;
	}
}

.signature-list {
	margin-bottom: 30px;
}

.signature-item {
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	padding: 10px;
	margin-bottom: 10px;
}

.signature-actions {
	display: flex;
	gap: 10px;
}

.el-select {
	width: 200px;
}

// 签名设置止


.email-tags-container {
	display: flex;
	flex-wrap: wrap;
	gap: 4px;

	:deep(.el-tag) {
		margin: 2px;
		max-width: 120px;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	}
}

.subject-container {
	display: flex;
	align-items: center;
	gap: 12px;
	flex-wrap: wrap;

	.subject {
		margin: 0;
	}

	.email-tags {
		display: flex;
		flex-wrap: wrap;
		gap: 5px;
		/* 使用 gap 属性设置统一的间距 */
		margin-top: 8px;
		/* 与标题保持一定距离 */

		.custom-tag {
			font-size: 14px;
			/* 增加字体大小 */
			padding: 12px 24px;
			/* 增加内边距使标签更大 */
			margin: 10px;
			/* 清除默认边距，使用 gap 控制间距 */

			/* 可选：让标签更突出 */
			font-weight: 500;
			border-radius: 4px;
		}
	}
}

.tag-menu-item {
	position: relative;

	.tag-content {
		display: flex;
		justify-content: space-between;
		align-items: center;
		width: 100%;
	}

	.tag-left {
		display: flex;
		align-items: center;
		gap: 8px;
	}

	.delete-button {
		display: none;
		position: absolute;
		right: 8px;
		top: 50%;
		transform: translateY(-50%);
		padding: 4px;

		&:hover {
			color: var(--el-color-danger) !important;
		}
	}

	&:hover {
		.delete-button {
			display: inline-flex;
		}
	}
}

/* 可以添加以下样式来调整图标的外观 */
.el-menu-item .el-icon {
	margin-right: 8px;
	color: #909399;
	/* 设置图标颜色 */
}

/* 当菜单项被选中时的图标样式 */
.el-menu-item.is-active .el-icon {
	color: var(--el-menu-active-color);
}

.layout-container-demo {
	height: 100vh;
	overflow: hidden;
}

:deep(.el-scrollbar) {
	height: calc(100vh - 60px);
	/* 减去头部高度 */
}

:deep(.el-menu) {
	border-right: none;
}

:deep(.el-scrollbar__wrap) {
	overflow-x: hidden;
}

.menu-section-title {
	display: flex;
	justify-content: space-between;
	align-items: center;
	width: 100%;
	color: #666;
	font-size: 14px;

	.add-button {
		opacity: 0.7;

		&:hover {
			opacity: 1;
		}
	}
}

:deep(.el-collapse) {
	border: none;

	.el-collapse-item {
		.el-collapse-item__header {
			border: none;
			height: 40px;
			padding: 0 20px;

			&:hover {
				background-color: #f5f5f5;
			}
		}

		.el-collapse-item__content {
			padding: 0;
		}

		.el-collapse-item__arrow {
			margin-right: 10px;
		}
	}
}

:deep(.el-menu-item) {
	padding-left: 35px !important;
	height: 36px;
	line-height: 36px;
}

.menu-section-title {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 0 20px;
	height: 40px;
	color: #666;
	font-size: 14px;

	.add-button {
		opacity: 0.7;

		&:hover {
			opacity: 1;
		}
	}
}

.email-content-wrapper {
	padding: 20px;
	background: #fff;
}

.email-content {
	width: 100%;
	max-width: 100%;
	overflow-x: auto;
	word-break: break-word;
}

/* 确保表格样式正确 */
.email-content :deep(table) {
	border-collapse: collapse;
	margin: 10px 0;
	max-width: 100%;
}

/* 确保图片不会溢出容器 */
.email-content :deep(img) {
	max-width: 100%;
	height: auto;
}

/* 保持引用样式 */
.email-content :deep(blockquote) {
	border-left: 3px solid #ddd;
	margin: 10px 0;
	padding-left: 10px;
	color: #666;
}

/* 保持链接样式 */
.email-content :deep(a) {
	color: #409eff;
	text-decoration: none;
}

.email-content :deep(a:hover) {
	text-decoration: underline;
}

.email-body {
	padding: 16px;
	background: #fff;
	border: 1px solid #eee;
	border-radius: 4px;
	margin: 16px 0;
	overflow-x: auto;
}

/* 确保表格不会溢出容器 */
.email-body :deep(table) {
	width: auto !important;
	max-width: 100%;
}

/* 确保图片响应式缩放 */
.email-body :deep(img) {
	max-width: 100%;
	height: auto;
}

/* 所有元素都不应该溢出容器 */
.email-body :deep(*) {
	max-width: 100%;
	word-wrap: break-word;
}

.search-container {
	position: relative;
	/* 确保有相对定位 */
	width: 70%;
}

:deep(.advanced-search-popover) {
	padding: 0;
	max-height: 80vh;
	overflow-y: auto;
}

/* 确保 popover 在其他元素之上 */
:deep(.el-popover) {
	z-index: 2000;
}

.cursor-pointer {
	cursor: pointer;
}

.search-container {
	position: relative;
	width: 70%;
}

:deep(.advanced-search-popover) {
	padding: 0;
	max-height: 80vh;
	overflow-y: auto;
}

.advanced-search-content {
	padding: 20px;
}

.popover-title {
	font-size: 16px;
	font-weight: bold;
	margin-bottom: 20px;
	padding-bottom: 10px;
	border-bottom: 1px solid var(--el-border-color-lighter);
}

.popover-footer {
	display: flex;
	justify-content: space-between;
	padding-top: 20px;
	margin-top: 20px;
	border-top: 1px solid var(--el-border-color-lighter);
}

/* 确保表单样式正确 */
:deep(.el-form-item__content) {
	display: flex;
	align-items: center;
}

:deep(.el-input-number) {
	width: 150px;
}

/* 添加一些动画效果 */
:deep(.el-popover) {
	transition: transform 0.3s, opacity 0.3s;
	transform-origin: top;
}

:deep(.el-popover)[data-popper-placement^='bottom'] {
	margin-top: 12px;
}

.email-list-container {
	display: flex;
	flex-direction: column;
	height: 100%;
}

.pagination-container {
	padding: 15px;
	display: flex;
	justify-content: flex-end;
	background-color: white;
	border-top: 1px solid var(--el-border-color-lighter);
}

/* 确保表格占据剩余空间 */
.el-table {
	flex: 1;
	overflow: auto;
}

/* 分页组件样式 */
:deep(.el-pagination) {
	justify-content: flex-end;
	padding: 0;
}

:deep(.el-pagination .el-select .el-input) {
	width: 100px;
}

.tag-item {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 4px 8px;
}

.tag-checkbox {
	flex: 1;
}

.tag-actions {
	margin-left: 8px;
}

/* 确保复选框文本不会被截断 */
:deep(.el-checkbox__label) {
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
}

/* 删除按钮样式 */
.tag-actions :deep(.el-button--link) {
	padding: 2px 4px;
	font-size: 12px;
}

.action-buttons {
	display: flex;
	gap: 12px;
	margin-top: 20px;
}

.action-btn {
	display: flex;
	align-items: center;
	gap: 4px;
}

/* 添加新的样式 */
.action-buttons {
	margin-top: 12px;
	display: flex;
	gap: 12px;
}

.action-btn {
	border-radius: 20px;
	padding: 8px 20px;
	display: flex;
	align-items: center;
	gap: 4px;
	font-size: 14px;
}

.action-btn :deep(.el-icon) {
	margin-right: 4px;
}

.toolbar {
	display: flex;
	align-items: center;
	position: absolute;
	right: 20px;
	top: 50%;
	transform: translateY(-50%);
}

.user-profile {
	display: flex;
	align-items: center;
	cursor: pointer;
	padding: 4px 8px;
	border-radius: 20px;
	transition: background-color 0.3s;
}

.user-profile:hover {
	background-color: var(--el-fill-color-light);
}

.username {
	margin: 0 8px;
	font-size: 14px;
	color: var(--el-text-color-primary);
}

:deep(.el-dropdown-menu__item) {
	display: flex;
	align-items: center;
	gap: 8px;
}

:deep(.el-avatar) {
	background-color: var(--el-color-primary);
	color: white;
}

.search-container {
	margin-left: 10px;
}

/* 确保 header 内容垂直居中 */
.layout-container-demo .el-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	background: none;
	color: var(--el-text-color-primary);
}

/* ... 其他样式 ... */
.email-detail {
	height: 100%;
	background: #fff;
}

.detail-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 16px;
	border-bottom: 1px solid #eee;
}

.detail-content {
	padding: 20px;
	height: calc(100% - 64px);
	overflow: auto;
}

.email-header {
	margin-bottom: 24px;
}

.subject {
	margin: 0 0 16px;
	font-size: 24px;
	font-weight: 500;
}

.meta-info {
	color: #666;
	font-size: 14px;
}

.meta-info>div {
	margin: 8px 0;
}

.label {
	color: #999;
	margin-right: 8px;
}

.email-body {
	padding: 16px 0;
	border-top: 1px solid #eee;
	min-height: 200px;
}

.attachments {
	margin-top: 24px;
	padding-top: 16px;
	border-top: 1px solid #eee;
}

.attachment-header {
	display: flex;
	align-items: center;
	gap: 8px;
	margin-bottom: 12px;
	color: #666;
}

.attachment-list {
	display: flex;
	flex-direction: column;
	gap: 8px;
}

.attachment-item {
	display: flex;
	align-items: center;
	gap: 8px;
	padding: 8px;
	background: #f5f7fa;
	border-radius: 4px;
}

.filename {
	flex: 1;
}

.filesize {
	color: #999;
	margin-right: 16px;
}


.dialog-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 0 20px;
}

.dialog-header-actions {
	display: flex;
	gap: 8px;
}

/* 修改全屏样式 */
:deep(.el-dialog.is-fullscreen) {
	display: flex;
	flex-direction: column;
	margin: 0 !important;
	width: 100% !important;
	height: 100% !important;

	.el-dialog__body {
		flex: 1;
		height: calc(100vh - 120px);
		overflow: auto;
	}

	.email-content {
		height: calc(100vh - 350px);
	}
}

/* 修改全屏按钮样式 */
.dialog-header-actions {
	display: flex;
	gap: 8px;

	.el-button {
		padding: 8px;

		&:hover {
			background-color: var(--el-color-primary-light-9);
		}
	}
}

/* 表单项的间距调整 */
.email-form-item {
	margin-bottom: 8px;
	/* 减小表单项之间的间距 */
}

.email-form {
	padding: 0 20px;
}

.email-form-item {
	display: flex;
	align-items: center;
	margin-bottom: 12px;

	.label {
		width: 60px;
		color: #606266;
	}
}

.email-content {
	margin-top: 20px;
}

.dialog-footer {
	display: flex;
	justify-content: space-between;
	align-items: center;

	.left {
		display: flex;
		gap: 8px;
	}

	.right {
		display: flex;
		gap: 8px;
	}
}

/* 高亮行样式 */
:deep(.highlight-row) {
	background-color: #f2f6fc !important;
}

/* 鼠标悬停样式 */
:deep(.el-table__row:hover) {
	background-color: #f5f7fa !important;
}

/* 选中行样式 */
:deep(.el-table__row.selected) {
	background-color: #ecf5ff !important;
}

/* 操作按钮容器样式 */
.operation-buttons {
	display: flex;
	justify-content: flex-end;
	gap: 8px;
}

/* 去除表格的默认边框 */
:deep(.el-table) {
	border: none;
}

/* 自定义表格行高 */
:deep(.el-table__row) {
	height: 48px;
}

.layout-container-demo .el-header {
	position: relative;
	background: none;
	color: var(--el-text-color-primary);
}

.search-container {
	margin-left: 10px;
	/* 可以根据需要调整左边距 */
}

.layout-container-demo .el-header {
	position: relative;
	/* background-color: var(--el-col</script>*/
}

.layout-container-demo .el-header {
	position: relative;
	/* background-color: var(--el-col</script>*/
}

.layout-container-demo .el-header {
	position: relative;
	/* background-color: var(--el-color-primary-light-7); */
	background: none;
	color: var(--el-text-color-primary);
}

.layout-container-demo .el-aside {
	color: var(--el-text-color-primary);
	/* background: var(--el-color-primary-light-8); */
	background: none;
}

.layout-container-demo .el-menu {
	border-right: none;
	background: none;
}

.layout-container-demo .el-main {
	padding: 0;
}

.layout-container-demo .toolbar {
	display: inline-flex;
	align-items: center;
	justify-content: center;
	height: 100%;
	right: 20px;
}
</style>