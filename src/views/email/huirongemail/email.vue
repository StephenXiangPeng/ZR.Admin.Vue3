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
							<el-button class="add-button" link size="small" @click="showAddFolderDialog = true">
								<el-icon>
									<Plus />
								</el-icon>
							</el-button>
						</div>
						<el-tree :data="emailFolders" :props="defaultProps" @node-click="handleNodeClick"
							:default-expanded-keys="['1']" node-key="id" :expand-on-click-node="false"
							class="email-folder-tree">
							<template #default="{ node, data }">
								<div class="custom-tree-node">
									<el-icon>
										<Folder />
									</el-icon>
									<span>{{ node.label }}</span>
									<div class="folder-actions" v-if="!data.isSystem">
										<el-button link type="danger" size="small"
											@click.stop="handleDeleteFolder(node, data)">
											<el-icon>
												<Delete />
											</el-icon>
										</el-button>
									</div>
								</div>
							</template>
						</el-tree>

					</el-menu>
				</el-scrollbar>
			</el-aside>

			<!-- 添加文件夹对话框 -->
			<el-dialog v-model="showAddFolderDialog" title="新建文件夹" width="30%" :close-on-click-modal="false">
				<el-form :model="newFolderForm" label-width="80px">
					<el-form-item label="名称">
						<el-input v-model="newFolderForm.name" placeholder="请输入文件夹名称" />
					</el-form-item>
					<el-form-item label="父文件夹">
						<el-cascader v-model="newFolderForm.parentId" :options="cascaderOptions"
							:props="{ checkStrictly: true, emitPath: true }" placeholder="请选择父文件夹" clearable
							style="width: 100%" />
					</el-form-item>
				</el-form>
				<template #footer>
					<span class="dialog-footer">
						<el-button @click="showAddFolderDialog = false">取消</el-button>
						<el-button type="primary" @click="handleAddFolder">确定</el-button>
					</span>
				</template>
			</el-dialog>

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

							<!-- 批量操作按钮区域 -->
							<div v-if="selectedRows.length > 0" class="batch-actions">
								<div class="batch-info">
									<el-icon>
										<InfoFilled />
									</el-icon>
									<span>已选择 {{ selectedRows.length }} 封邮件</span>
								</div>
								<div class="batch-buttons">
									<el-button-group>
										<el-button type="primary" @click="handleBatchArchive"
											:loading="isBatchProcessing">
											<el-icon>
												<Box />
											</el-icon>
											归档
										</el-button>
										<!-- 新增：移动至按钮 -->
										<el-button type="warning" @click="showBatchMoveDialog"
											:loading="isBatchProcessing">
											<el-icon>
												<Folder />
											</el-icon>
											移动至
										</el-button>
										<el-button type="danger" @click="handleBatchDelete"
											:loading="isBatchProcessing">
											<el-icon>
												<Delete />
											</el-icon>
											删除
										</el-button>
									</el-button-group>
									<el-button @click="clearSelection" size="small">
										取消选择
									</el-button>
								</div>
							</div>

							<el-table ref="emailTable" :data="EmailTableData" @selection-change="handleSelectionChange"
								@row-click="handleRowClick" :row-class-name="tableRowClassName" style="width: 100%;">
								<!-- 选择列 -->
								<el-table-column type="selection" fixed min-width="50" align="center" />
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
					<span>{{ dialogTitle }}</span>
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
				<!-- 富文本编辑器（保持 contentType="delta"） -->
				<QuillEditor ref="quillEditor" v-model:content="emailForm.delta" contentType="delta"
					:options="editorOptions" :style="{ minHeight: '300px' }" />

				<!-- 原文只读区 -->
				<div v-if="quotedHtml" class="quoted-wrapper">
					<div class="quoted-toggle" @click="showQuoted = !showQuoted">
						{{ showQuoted ? '收起原文' : '展开原文' }}
					</div>
					<div v-show="showQuoted" class="quoted-container" v-html="quotedHtml"></div>
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
					<el-form-item label="商机名称">
						<el-select v-model="BusinessOpportunityForm.opportunityId" clearable filterable
							placeholder="请选择商机名称" @change="handleOpportunityChange">
							<el-option v-for="item in BusinessOpportunityList" :key="item.id" :label="item.businessName"
								:value="item.id">
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
		<!-- 询盘商机名称输入对话框 -->
		<el-dialog v-model="inquiryOpportunityDialog" title="创建商机" width="30%">
			<div class="inquiry-opportunity-dialog">
				<el-form :model="inquiryOpportunityForm" label-width="100px">
					<el-form-item label="商机名称" required>
						<el-input v-model="inquiryOpportunityForm.opportunityName" placeholder="请输入商机名称" maxlength="100"
							show-word-limit />
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="inquiryOpportunityDialog = false">取消</el-button>
					<el-button type="primary" @click="handleInquiryOpportunityConfirm">确定</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 批量移动至文件夹对话框 -->
		<el-dialog v-model="showBatchMoveToFolderDialog" title="移动邮件至文件夹" width="50%" :close-on-click-modal="false">
			<div class="move-to-folder-dialog">
				<div class="selected-emails-info">
					<el-alert title="提示" type="info" :closable="false" style="margin-bottom: 20px;">
						<template #default>
							将移动 <strong>{{ selectedRows.length }}</strong> 封邮件到选定的文件夹
						</template>
					</el-alert>
				</div>
				<el-form label-width="100px">
					<el-form-item label="目标文件夹">
						<el-tree :data="flattenedFolderOptions" :props="folderTreeProps"
							@node-click="handleFolderSelect" node-key="value" highlight-current
							:default-expanded-keys="['1']"
							style="max-height: 300px; overflow-y: auto; border: 1px solid #dcdfe6; border-radius: 4px; padding: 10px;">
							<template #default="{ node, data }">
								<div class="folder-tree-node">
									<el-icon>
										<Folder />
									</el-icon>
									<span style="margin-left: 8px;">{{ node.label }}</span>
									<el-tag v-if="data.isSystem" size="small" type="info" style="margin-left: 8px;">
										系统
									</el-tag>
								</div>
							</template>
						</el-tree>
					</el-form-item>

					<el-form-item label="选中文件夹">
						<el-input v-model="selectedFolderName" readonly placeholder="请选择目标文件夹" />
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<div class="dialog-footer">
					<el-button @click="showBatchMoveToFolderDialog = false">取消</el-button>
					<el-button type="primary" @click="handleBatchMoveToFolder" :loading="isBatchMovingToFolder"
						:disabled="!selectedFolderData">
						移动邮件
					</el-button>
				</div>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { ref, watch, reactive, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
import { useRoute } from 'vue-router'
import {
	Menu as IconMenu, Message, Setting, EditPen, Delete, Position, Search, FullScreen, Rank, Close,
	ArrowDown, ArrowLeft, Right, Bell, ChatRound, Share, DocumentAdd, InfoFilled, User, SwitchButton,
	Operation, Plus, Box, Edit, Paperclip, Document, Folder, CollectionTag
} from '@element-plus/icons-vue'
import { closePage } from '@/plugins/tab'
import { QuillEditor } from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css'
import request from '@/utils/request'
import DOMPurify from 'dompurify'
import { useRouter } from 'vue-router'
import Delta from 'quill-delta'                // ✅ v2 用这个类


type DeltaType = InstanceType<typeof Delta>

// 编辑器仍用 delta（你已有）
const quotedHtml = ref('')        // 原文块（只读 v-html）
const showQuoted = ref(true)      // 展开/收起


function decodeIfEscaped(html: string) {
	if (/&lt;|&gt;|&amp;/.test(html)) {
		const ta = document.createElement('textarea')
		ta.innerHTML = html
		return ta.value
	}
	return html
}

function htmlToDelta(quill, html) {
	const decoded = decodeIfEscaped(html)
	const safe = DOMPurify.sanitize(decoded, { USE_PROFILES: { html: true } })
	return quill.clipboard.convert(safe) // 交给 Quill 转 Delta（会按白名单裁剪）
}



function extractBodyHtml(rawHtml: string) {
	const decoded = decodeIfEscaped(rawHtml)
	const doc = new DOMParser().parseFromString(decoded, 'text/html')
	let bodyHtml = doc.body ? doc.body.innerHTML : decoded
	bodyHtml = bodyHtml.replace(/<!--\[if[\s\S]*?endif\]-->/gi, '') // 清 MSO 条件注释
	// 禁止把 <html>/<head>/<style>/<script> 等带进来
	return DOMPurify.sanitize(bodyHtml, {
		FORBID_TAGS: ['html', 'head', 'meta', 'link', 'style', 'script', 'title', 'base']
	})
}

// 简单转义（用于头信息里）
function esc(s?: string) {
	return (s ?? '').replace(/[&<>"']/g, m =>
		({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]!)
	)
}

function buildQuotedBlock(mail: {
	from: string; to: string; cc?: string; date: string; subject: string; content: string
}) {
	const body = extractBodyHtml(mail.content)
	return `
    <div class="quoted-mail">
      <div class="quoted-title">------------------ 原始邮件 ------------------</div>
      <div class="quoted-meta"><b>发件人:</b> ${mail.from}</div>
      <div class="quoted-meta"><b>发送时间:</b> ${mail.date}</div>
      <div class="quoted-meta"><b>主题:</b> ${mail.subject}</div>
      <div class="quoted-meta"><b>收件人:</b> ${mail.to}</div>
      ${mail.cc ? `<div class="quoted-meta"><b>抄送:</b> ${mail.cc}</div>` : ''}
      <div class="quoted-body">${body}</div>
    </div>
  `
}
// 路由实例
const router = useRouter()
const route = useRoute()

// 防止重复执行自动打开邮件的标志
const isAutoOpeningEmail = ref(false)

// #region 系统标签定义
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

// 定义标签类型映射
const tagTypeMap = {
	'询盘': 'info',
	'初次报价': 'success',
	'沟通需求': 'warning',
	'再次报价': 'danger',
	'合同确定': 'primary',
	'开发信': ''
}

// 获取标签类型的方法
const getTagType = (tagName) => {
	return tagTypeMap[tagName] || ''
}
// #endregion

// #region 商机选择相关
const BusinessOpportunitySelectionDialog = ref(false)
const BusinessOpportunityList = ref([])
const quotationList = ref([])
const contractList = ref([])
const BusinessOpportunityForm = ref({
	opportunityId: null,
	quotationId: null,
	contractId: null,
	type: '',
	tagNames: []
})

// 询盘商机名称输入对话框相关
const inquiryOpportunityDialog = ref(false)
const inquiryOpportunityForm = ref({
	opportunityName: ''
})

// 打开询盘商机名称输入对话框
const openInquiryOpportunityDialog = () => {
	inquiryOpportunityForm.value.opportunityName = ''
	inquiryOpportunityDialog.value = true
}

// 打开商机选择对话框
const openBusinessOpportunitySelectionDialog = async (type, tagNames) => {
	BusinessOpportunityForm.value = {
		opportunityId: null,
		quotationId: null,
		contractId: null,
		type,
		tagNames
	}

	try {
		const listResponse = await request({
			url: '/BusinessOpportunity/GetBusinessOpportunityListByUser/GetBusinessOpportunityList',
			method: 'GET'
		})
		if (listResponse?.data?.code === 200 && listResponse.data?.data?.length > 0) {
			await handleOpportunityChange(listResponse.data.data[0].id)
			BusinessOpportunityList.value = listResponse.data.data
			BusinessOpportunitySelectionDialog.value = true
		} else {
			ElMessage.warning('未找到相关商机')
		}
	} catch (error) {
		console.error('获取商机列表失败:', error)
		ElMessage.error('获取商机列表失败')
	}
}

// 商机选择变更时的处理
const handleOpportunityChange = async (opportunityId) => {
	if (!opportunityId) return
	try {
		if (BusinessOpportunityForm.value.type === 'quotation') {
			const response = await request({
				url: '/Quotation/GetQuotaionListByUser/GetQuotaionList',
				method: 'GET'
			})
			if (response?.data?.code === 200) {
				quotationList.value = response.data.data
			}
		} else if (BusinessOpportunityForm.value.type === 'contract') {
			const response = await request({
				url: '/Contracts/GetContractListByUser/GetContractList',
				method: 'GET'
			})
			if (response.code === 200) {
				contractList.value = response.data
			}
		}
	} catch (error) {
		console.error('获取关联数据失败:', error)
		ElMessage.error('获取关联数据失败')
	}
}

// 增强的确认选择方法 - 添加本地数据更新
const handleConfirmSelection = async () => {
	if (!BusinessOpportunityForm.value.opportunityId) {
		ElMessage.warning('请选择商机编号')
		return
	}

	if (BusinessOpportunityForm.value.type === 'quotation' && !BusinessOpportunityForm.value.quotationId) {
		ElMessage.warning('请选择报价单号')
		return
	}

	if (BusinessOpportunityForm.value.type === 'contract' && !BusinessOpportunityForm.value.contractId) {
		ElMessage.warning('请选择合同编号')
		return
	}

	try {
		let salesStage = ''
		let docuementID = 0
		if (BusinessOpportunityForm.value.type === 'quotation') {
			salesStage = BusinessOpportunityForm.value.tagNames.includes('初次报价') ? '初次报价' : '再次报价'
			docuementID = BusinessOpportunityForm.value.quotationId
		} else if (BusinessOpportunityForm.value.type === 'contract') {
			salesStage = '合同确定'
			docuementID = BusinessOpportunityForm.value.contractId
		} else if (BusinessOpportunityForm.value.type === 'communication') {
			salesStage = '沟通需求'
			docuementID = 0
		}

		const response = await request({
			url: '/BusinessOpportunity/UpdateBusinessOpportunitySalesStage/UpdateSalesStage',
			method: 'GET',
			params: {
				BusinessOpportunityID: BusinessOpportunityForm.value.opportunityId,
				SalesStage: salesStage,
				DocuementID: docuementID
			}
		})

		if (response.code === 200) {
			await EditEmailTags()
			BusinessOpportunitySelectionDialog.value = false
			ElMessage.success('设置成功')

			// 成功后立即更新本地标签数据，避免需要刷新才能看到
			await updateLocalEmailTags(EmailModel.id, EmailTagcheckboxGroup.value)

		} else {
			ElMessage.error(response.msg || '更新销售阶段失败')
		}
	} catch (error) {
		console.error('设置失败:', error)
		ElMessage.error('设置失败')
	}
}

// 处理询盘商机名称确认
const handleInquiryOpportunityConfirm = async () => {
	if (!inquiryOpportunityForm.value.opportunityName.trim()) {
		ElMessage.warning('请输入商机名称')
		return
	}
	try {
		// 更新邮件标签
		await EditEmailTags()
		// 成功后立即更新本地标签数据
		await updateLocalEmailTags(EmailModel.id, EmailTagcheckboxGroup.value)
		inquiryOpportunityDialog.value = false

	} catch (error) {
		console.error('创建商机失败:', error)
		ElMessage.error('创建商机失败')
	}
}
// #endregion

// #region 邮件相关状态管理
const EmailTableData = ref([])
const originalEmailData = ref([])
const activeMenu = ref('1')
const showEmailDetail = ref(false)
const currentEmail = ref({
	id: '',
	subject: '',
	from: '',
	to: '',
	cc: '',
	date: '',
	content: '',
	attachments: [],
	tags: []
})

// 邮件列表相关
const currentPage = ref(1)
const pageSize = ref(20)
const totalItems = ref(0)
const selectedRows = ref([])
const isBatchProcessing = ref(false)

// 搜索相关
const input1 = ref('')
const isSearchMode = ref(false)
const lastSearchParams = ref(null)
const showAdvancedSearch = ref(false)
const searchForm = reactive({
	sender: '',
	receiver: '',
	subject: '',
	includeWords: '',
	excludeWords: '',
	dateRange: '1',
	customDateRange: [],
	searchScope: '0',
	hasAttachment: false
})

// 其他状态
const EmailTagIndex = ref(0)
const folderName = ref(null)
const SelectEmailID = ref('')
// #endregion

// #region 邮件配置相关
const ConfigEmaildialog = ref(false)
const IsEditUserEmailConfig = ref(false)
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
	const response = await checkUserEmailConfig()
	if (response && response.data) {
		Object.assign(ConfigEmailForm, {
			id: response.data.id,
			userID: response.data.userID,
			email: response.data.userEmail,
			password: response.data.userEmailAuth,
			emailSendServer: response.data.userEmailSendServer,
			smtpPort: response.data.userEmailSendPort,
			emailReceiveServer: response.data.userEmailReceiveServer,
			imapPort: response.data.userEmailReceivePort
		})
	}
}

// 检查用户邮箱配置
function checkUserEmailConfig() {
	return request({
		url: 'Email/GetEmailConfigByUser/GetEmailConfigByUser',
		method: 'GET'
	}).then(response => {
		return response || null
	}).catch(error => {
		console.error(error)
		return null
	})
}

// 验证邮箱配置是否存在
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
			ConfigEmaildialog.value = true
		} else {
			ConfigEmaildialog.value = false
			// 确保设置为收件箱
			if (activeMenu.value !== '1') {
				activeMenu.value = '1'
			}
			refreshCurrentView()
			initEmailTag()
			fetchCustomFolders()
		}
	}).catch(error => {
		console.error(error)
		return null
	})
}

// 配置邮箱
function ConfigUserEmail() {
	const loading = ElLoading.service({
		lock: true,
		text: '正在配置邮箱并获取邮件，请稍等.....',
		background: 'rgba(0, 0, 0, 0.7)',
	})

	try {
		const requestData = {
			id: ConfigEmailForm.id,
			UserID: ConfigEmailForm.userID,
			UserEmail: ConfigEmailForm.email,
			UserEmailSendServer: ConfigEmailForm.emailSendServer,
			UserEmailReceiveServer: ConfigEmailForm.emailReceiveServer,
			UserEmailAuth: ConfigEmailForm.password,
			UserEmailSendPort: ConfigEmailForm.smtpPort,
			UserEmailReceivePort: ConfigEmailForm.imapPort
		}

		const url = IsEditUserEmailConfig.value ?
			'Email/EditUserEmailConfig/EditUserEmailConfig' :
			'Email/AddUserEmailConfig/AddUserEmailConfig'

		request({
			url: url,
			method: 'POST',
			data: requestData
		}).then(response => {
			if (response != null) {
				ConfigEmaildialog.value = false
				ElMessage({
					message: response.msg,
					type: 'success'
				})
				refreshCurrentView()
				initEmailTag()
				fetchCustomFolders()
			}
		}).catch(error => {
			console.error('邮箱配置失败:', error)
		}).finally(() => {
			loading.close()
		})
	} catch (error) {
		loading.close()
		console.error('邮箱配置失败:', error)
	}
}

// 解绑邮箱
function UnbindUserEmail() {
	ElMessageBox.confirm('确定要解绑当前邮箱吗？解绑后将无法收发邮件。', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		try {
			const loading = ElLoading.service({
				lock: true,
				text: '正在解绑邮箱，请稍等...',
				background: 'rgba(0, 0, 0, 0.7)',
			})

			const requestData = {
				id: ConfigEmailForm.id,
				UserEmail: ConfigEmailForm.email,
				UserEmailSendServer: ConfigEmailForm.emailSendServer,
				UserEmailReceiveServer: ConfigEmailForm.emailReceiveServer,
				UserEmailAuth: ConfigEmailForm.password,
				UserEmailSendPort: ConfigEmailForm.smtpPort,
				UserEmailReceivePort: ConfigEmailForm.imapPort,
				UserID: 0
			}

			const response = await request({
				url: 'Email/UnbindEmail/UnbindEmail',
				method: 'POST',
				data: requestData
			})

			loading.close()

			if (response.code === 200) {
				ElMessage.success(response.msg || '解绑邮箱成功')
				// 清空所有数据
				resetAllData()
				// 关闭页面
				closePage().then(() => {
					console.log('关闭邮箱页面')
				})
			} else {
				ElMessage.error(response.msg || '解绑邮箱失败')
			}
		} catch (error) {
			console.error('解绑邮箱失败:', error)
			ElMessage.error('解绑邮箱失败，请重试')
		}
	}).catch(() => {
		// 用户取消操作
	})
}

// 重置所有数据
const resetAllData = () => {
	EmailTableData.value = []
	originalEmailData.value = []
	totalItems.value = 0
	UserEmailTagList.value = []
	EmailTagcheckboxoptions.value = []
	EmailTagcheckboxGroup.value = []
	customFolders.value = []
	Object.assign(ConfigEmailForm, {
		id: 0,
		userID: 0,
		email: '',
		password: '',
		emailSendServer: '',
		emailReceiveServer: '',
		smtpPort: 0,
		imapPort: 0
	})
	showEmailDetail.value = false
	currentEmail.value = {
		id: '',
		subject: '',
		from: '',
		to: '',
		cc: '',
		date: '',
		content: '',
		attachments: [],
		tags: []
	}
	ConfigEmaildialog.value = false
	IsEditUserEmailConfig.value = false
}

const openUserEmailConfigDialog = async () => {
	try {
		const loading = ElLoading.service({
			lock: true,
			text: '正在获取邮箱配置...',
			background: 'rgba(0, 0, 0, 0.7)',
		})

		const response = await request({
			url: 'Email/GetEmailConfigByUser/GetEmailConfigByUser',
			method: 'GET',
			params: { timestamp: new Date().getTime() }
		})

		loading.close()

		if (response && response.data) {
			Object.assign(ConfigEmailForm, {
				id: response.data.id || 0,
				userID: response.data.userID || 0,
				email: response.data.userEmail || '',
				password: response.data.userEmailAuth || '',
				emailSendServer: response.data.userEmailSendServer || '',
				smtpPort: response.data.userEmailSendPort || 0,
				emailReceiveServer: response.data.userEmailReceiveServer || '',
				imapPort: response.data.userEmailReceivePort || 0
			})
			IsEditUserEmailConfig.value = true
		} else {
			IsEditUserEmailConfig.value = false
			ElMessage.warning('未能获取邮箱配置数据')
		}
		ConfigEmaildialog.value = true
	} catch (error) {
		console.error('获取邮箱配置失败:', error)
		ElMessage.error('获取邮箱配置失败')
		ConfigEmaildialog.value = true
	}
}

const handleEmailInput = () => {
	const email = ConfigEmailForm.email
	const atIndex = email.indexOf('@')
	if (atIndex !== -1) {
		const domain = email.substring(atIndex + 1)
		ConfigEmailForm.password = ''
		ConfigEmailForm.emailSendServer = 'smtp.' + domain
		ConfigEmailForm.emailReceiveServer = 'imap.' + domain
		ConfigEmailForm.smtpPort = 465
		ConfigEmailForm.imapPort = 993
	}
}

const handleConfigEmailDialogClose = () => {
	if (!ConfigEmailForm.email || !ConfigEmailForm.password || !ConfigEmailForm.emailSendServer ||
		!ConfigEmailForm.emailReceiveServer || !ConfigEmailForm.smtpPort || !ConfigEmailForm.imapPort) {
		closePage().then(() => {
			console.log('关闭邮箱标签页')
		})
	}
}
// #endregion

// #region 邮件数据管理
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
	"businessopportunityid": 0,
	"businessopportunityname": ''
})

// 统一的数据刷新方法
const refreshCurrentView = async () => {
	try {
		console.log('刷新当前视图, activeMenu:', activeMenu.value)

		// 清除搜索状态
		isSearchMode.value = false
		lastSearchParams.value = null

		if (isSearchMode.value && lastSearchParams.value) {
			// 搜索模式
			await handleAdvancedSearchRequest()
		} else if (activeMenu.value.startsWith('tag-')) {
			// 标签模式
			const tagId = activeMenu.value.replace('tag-', '')
			EmailTagIndex.value = Number(tagId)
			await getInboxEmail(currentPage.value, pageSize.value, 1)
		} else if (activeMenu.value.startsWith('folder-')) {
			// 文件夹模式
			const folderId = activeMenu.value.replace('folder-', '')
			const folderData = findFolderDataById(emailFolders.value, folderId)
			if (folderData && folderData.type) {
				await getEmailArchiveList(currentPage.value, pageSize.value, folderData.type, folderId)
			}
		} else {
			// 普通菜单模式
			await getInboxEmail(currentPage.value, pageSize.value, activeMenu.value)
		}
	} catch (error) {
		console.error('刷新视图失败:', error)
		ElMessage.error('刷新数据失败')
	}
}

// 获取邮件列表的统一方法
function getInboxEmail(start, end, emailType) {
	console.log('getInboxEmail 调用 - start:', start, 'end:', end, 'emailType:', emailType)

	EmailTableData.value = []
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
				originalEmailData.value = [...processedEmails]
				totalItems.value = response.data.totalNum
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					getInboxEmail(start - 1, end, emailType)
					return
				}
				EmailTableData.value = []
				originalEmailData.value = []
				totalItems.value = 0
			}
			resolve(response.data)
		}).catch(error => {
			console.error('获取邮件列表失败:', error)
			reject(error)
		})
	})
}

// 增强的获取归档邮件列表方法 - 添加参数验证
const getEmailArchiveList = async (pageNum, pageSize, type, dataId) => {
	try {
		console.log('获取归档邮件列表 - 参数:', { pageNum, pageSize, type, dataId })

		// 参数验证
		if (!type) {
			throw new Error('文件夹类型(type)不能为空')
		}
		if (!dataId) {
			throw new Error('文件夹ID(dataId)不能为空')
		}
		if (!pageNum || pageNum < 1) {
			throw new Error('页码必须大于0')
		}
		if (!pageSize || pageSize < 1) {
			throw new Error('每页数量必须大于0')
		}

		const response = await request({
			url: 'Email/GetEmailArchiveList/GetArchive',
			method: 'GET',
			params: {
				PageNum: pageNum,
				PageSize: pageSize,
				type: type,
				dataId: dataId
			}
		})

		console.log('归档邮件API响应:', response)

		if (response.data && response.data.result) {
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

			console.log('处理后的邮件数据数量:', processedEmails.length)
			EmailTableData.value = processedEmails
			originalEmailData.value = [...processedEmails]
			totalItems.value = response.data.totalNum || 0
		} else {
			console.log('API返回空数据')
			EmailTableData.value = []
			originalEmailData.value = []
			totalItems.value = 0
		}
	} catch (error) {
		console.error('获取归档邮件失败:', error)
		ElMessage.error(`获取邮件失败: ${error.message || '未知错误'}`)

		// 发生错误时清空数据
		EmailTableData.value = []
		originalEmailData.value = []
		totalItems.value = 0

		throw error
	}
}

// 获取发件人姓名
const GetFromEmailName = (fromEmail) => {
	if (!fromEmail) return '未知'

	const nameMatch = fromEmail.match(/"([^"]+)"/)
	if (nameMatch && nameMatch[1]) {
		return nameMatch[1]
	}

	const emailOnlyMatch = fromEmail.match(/^([^@]+)@/)
	if (emailOnlyMatch && emailOnlyMatch[1]) {
		return emailOnlyMatch[1]
	}

	return fromEmail
}

// 标记邮件为已读
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
				row.isRead = 0
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
			await refreshCurrentView()
		} else {
			ElMessage.error(response.msg || '标记未读失败')
		}
	} catch (error) {
		console.error('标记邮件为未读失败:', error)
		ElMessage.error('操作失败，请重试')
	}
}
// #endregion

// #region 菜单操作
// 5. 优化的 MenuClick 方法
const MenuClick = async (menuIndex) => {
	if (await CheckShowEmailDetail() == false) {
		return
	}

	console.log('📧 MenuClick - 切换菜单，menuIndex:', menuIndex)

	// 重置所有状态
	showEmailDetail.value = false
	isSearchMode.value = false
	lastSearchParams.value = null
	currentPage.value = 1

	// 设置系统文件夹状态
	const systemFolderNames = {
		'1': '收件箱',
		'2': '已发邮件',
		'3': '草稿箱',
		'4': '垃圾箱',
		'6': '归档邮件'
	}

	setCurrentFolderState('system', menuIndex, systemFolderNames[menuIndex.toString()])

	// 清除选择
	clearSelection()

	// 获取邮件列表
	await getInboxEmail(currentPage.value, pageSize.value, menuIndex)
}

// 4. 优化的 filterByTag 方法
const filterByTag = async (tagId) => {
	if (await CheckShowEmailDetail() == false) {
		return
	}

	try {
		showEmailDetail.value = false
		isSearchMode.value = false
		lastSearchParams.value = null
		currentPage.value = 1

		// 查找标签名称
		const tagData = UserEmailTagList.value.find(tag => tag.id == tagId)
		const tagName = tagData?.emailTagName || `标签${tagId}`

		// 设置统一的标签状态
		setCurrentFolderState('tag', tagId, tagName, tagData)

		console.log('🏷️ 标签状态设置完成:', currentFolderState.value)

		// 清除选择
		clearSelection()

		// 获取带有该标签的邮件列表
		await getInboxEmail(currentPage.value, pageSize.value, 1)
	} catch (error) {
		console.error('❌ 按标签过滤邮件失败:', error)
		ElMessage.error('获取标签邮件失败')
	}
}

/// 3. 优化的 filterByFolder 方法
const filterByFolder = async (folderId, type) => {
	if (await CheckShowEmailDetail() == false) {
		return
	}

	try {
		console.log('📁 filterByFolder - 开始过滤文件夹，folderId:', folderId, 'type:', type)

		showEmailDetail.value = false
		isSearchMode.value = false
		lastSearchParams.value = null
		currentPage.value = 1

		// 查找文件夹数据
		const folderData = findFolderDataById(emailFolders.value, folderId)
		const folderName = folderData?.label || `文件夹${folderId}`

		// 设置统一的文件夹状态
		setCurrentFolderState('folder', folderId, folderName, folderData, { type })

		console.log('📁 文件夹状态设置完成:', currentFolderState.value)

		// 清除选择
		clearSelection()

		// 获取归档邮件列表
		await getEmailArchiveList(currentPage.value, pageSize.value, type, folderId)
	} catch (error) {
		console.error('❌ 过滤分类邮件失败:', error)
		ElMessage.error('获取分类邮件失败')
	}
}
// #endregion

// #region 邮件标签管理
const EmailTagcheckboxGroup = ref([])
const EmailTagcheckboxoptions = ref([])
const UserEmailTagList = ref([])
const newEmailTagName = ref('')
const isLoadingTags = ref(false)

// 获取用户邮件标签列表
const GetUserEmailTagList = () => {
	return request({
		url: 'Email/GetUserEmailTagList/GetUserEmailTag',
		method: 'GET'
	}).then(response => {
		if (response != null) {
			EmailTagcheckboxoptions.value = []
			if (EmailModel.EmailTags != null && EmailModel.EmailTags != '') {
				const EmailTagArray = EmailModel.EmailTags.split(',')
				response.data.forEach(item => {
					EmailTagcheckboxoptions.value.push({ label: item.emailTagName, value: item.id })
					if (EmailTagArray.length > 0) {
						EmailTagArray.forEach(element => {
							if (element == item.id) {
								EmailTagcheckboxGroup.value.push(item.id)
							}
						})
					}
				})
			} else {
				response.data.forEach(item => {
					EmailTagcheckboxoptions.value.push({ label: item.emailTagName, value: item.id })
				})
			}
			UserEmailTagList.value = response.data
			return response.data.result
		} else {
			return null
		}
	}).catch(error => {
		console.error(error)
		return null
	})
}

// 初始化邮件标签
const initEmailTag = async () => {
	EmailTagcheckboxGroup.value = []
	UserEmailTagList.value = []
	newEmailTagName.value = ''

	const response = await request({
		url: 'Email/GetUserEmailTagList/GetUserEmailTag',
		method: 'GET'
	})

	if (response?.data) {
		EmailTagcheckboxoptions.value = response.data.map(item => ({
			label: item.emailTagName,
			value: item.id
		}))
		UserEmailTagList.value = response.data
	}
}

// 添加新邮件标签
const addNewEmailTag = async (type) => {
	let nameStr = ''
	if (type == 1) {
		nameStr = newFolderName.value
		showNewFolderDialog.value = false
	} else {
		nameStr = newEmailTagName.value
	}

	if (!nameStr) {
		ElMessage({
			message: '标签名称不能为空！😒',
			type: 'warning'
		})
		return
	}

	try {
		const response = await request({
			url: 'Email/AddUserEmailTag/AddUserEmailTag',
			method: 'POST',
			data: {
				EmailTagName: nameStr,
				Type: 0
			}
		})

		if (response.code == 200) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
			await initEmailTag()
			if (type === 1) {
				newFolderName.value = ''
			} else {
				newEmailTagName.value = ''
			}
		} else {
			ElMessage.error('添加新标签失败')
		}
	} catch (error) {
		ElMessage.error('添加失败，请重试')
	}
}

// 删除邮件标签
const deleteEmailTag = async (tagId) => {
	try {
		await ElMessageBox.confirm('确定要删除此标签吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		})

		const response = await request({
			url: 'Email/DelUserEmailTag/DelUserEmailTag',
			method: 'Delete',
			data: { id: tagId }
		})

		if (response.code == 200) {
			ElMessage.success(response.msg)
			initEmailTag()
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('删除标签失败:', error)
			ElMessage.error('删除标签失败')
		}
	}
}

// 处理标签变化
const handleTagChange = async (value) => {
	if (isLoadingTags.value) return

	const lastChangedTagId = value[value.length - 1]
	const isChecked = EmailTagcheckboxGroup.value.includes(lastChangedTagId)

	if (!isChecked) {
		EditEmailTags()
	} else {
		SelectEmailTags()
	}
}

// 选择邮件标签
const SelectEmailTags = () => {
	EmailModel.EmailTags = EmailTagcheckboxGroup.value.toString()
	EmailModel.EmailTagNames = EmailTagcheckboxGroup.value.map(tagId => getTagName(tagId)).join(',')

	// 根据标签名称判断需要打开的对话框类型
	if (EmailModel.EmailTagNames.includes('询盘')) {
		openInquiryOpportunityDialog()
		return
	}
	if (EmailModel.EmailTagNames.includes('初次报价')) {
		openBusinessOpportunitySelectionDialog('quotation', ['初次报价'])
		return
	}
	if (EmailModel.EmailTagNames.includes('再次报价')) {
		openBusinessOpportunitySelectionDialog('quotation', ['再次报价'])
		return
	}
	if (EmailModel.EmailTagNames.includes('合同确定')) {
		openBusinessOpportunitySelectionDialog('contract', ['合同确定'])
		return
	}
	if (EmailModel.EmailTagNames.includes('沟通需求')) {
		openBusinessOpportunitySelectionDialog('communication', ['沟通需求'])
		return
	}

	EditEmailTags()
}

// 修复标签编辑后的数据同步
const EditEmailTags = async () => {
	EmailModel.EmailTags = EmailTagcheckboxGroup.value.toString()
	EmailModel.EmailTagNames = EmailTagcheckboxGroup.value.map(tagId => getTagName(tagId)).join(',')
	EmailModel.businessopportunityid = BusinessOpportunityForm.value.opportunityId
	EmailModel.businessopportunityname = inquiryOpportunityForm.value.opportunityName

	try {
		const response = await request({
			url: 'Email/EditEmailTags/EditEmailTags',
			method: 'POST',
			data: EmailModel
		})

		if (response != null && response.code === 200) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})

			// 标签更新成功后，同步更新前端数据
			await updateLocalEmailTags(EmailModel.id, EmailTagcheckboxGroup.value)
		} else {
			console.error('更新邮件标签失败')
			throw new Error(response?.msg || '更新邮件标签失败')
		}
	} catch (error) {
		console.error('更新邮件标签失败！😔错误内容：', error)
		throw error // 重新抛出异常，让调用者能够捕获
	}
}

// 新增：更新本地邮件数据中的标签信息
const updateLocalEmailTags = async (emailId, newTagIds) => {
	try {
		console.log('更新本地邮件标签数据 - 邮件ID:', emailId, '新标签:', newTagIds)

		// 更新 EmailTableData 中的标签
		const emailIndex = EmailTableData.value.findIndex(email => email.id === emailId)
		if (emailIndex !== -1) {
			const newTagsString = newTagIds.join(',')
			EmailTableData.value[emailIndex].emailTags = newTagsString
			console.log('已更新EmailTableData中的标签:', EmailTableData.value[emailIndex])
		}

		// 更新 originalEmailData 中的标签
		const originalIndex = originalEmailData.value.findIndex(email => email.id === emailId)
		if (originalIndex !== -1) {
			const newTagsString = newTagIds.join(',')
			originalEmailData.value[originalIndex].emailTags = newTagsString
			console.log('已更新originalEmailData中的标签:', originalEmailData.value[originalIndex])
		}

		// 更新当前邮件详情中的标签（如果还在详情页）
		if (currentEmail.value.id === emailId) {
			currentEmail.value.tags = newTagIds
			console.log('已更新currentEmail中的标签')
		}

	} catch (error) {
		console.error('更新本地邮件标签数据失败:', error)
	}
}

// 获取标签行数据
const getRowTags = (tagsString) => {
	if (!tagsString) return []
	return tagsString.split(',').filter(tag => tag && !isNaN(tag)).map(Number)
}

// 获取标签名称
const getTagName = (tagId) => {
	const tag = EmailTagcheckboxoptions.value.find(t => t.value === tagId)
	return tag ? tag.label : ''
}
// #endregion

// #region 邮件文件夹管理
const emailFolders = ref([])
const customFolders = ref([])
const showAddFolderDialog = ref(false)
const showNewFolderDialog = ref(false)
const newFolderName = ref('')
const newFolderForm = ref({
	name: '',
	parentId: '1'
})
const cascaderOptions = ref([])

const defaultProps = {
	children: 'children',
	label: 'label'
}

// 获取自定义文件夹列表
const fetchCustomFolders = async () => {
	try {
		const response = await request({
			url: 'Email/GetAllContinent/GetAllContinent',
			method: 'GET'
		})
		if (response.data) {
			emailFolders.value = response.data
		}
	} catch (error) {
		console.error('获取文件夹列表失败:', error)
	}
}

// 处理节点点击
const handleNodeClick = (data) => {
	console.log(data)
	if (data.dataId && data.type) {
		filterByFolder(data.dataId, data.type)
	}
}

// 删除文件夹
const handleDeleteFolder = async (node, data) => {
	try {
		await ElMessageBox.confirm(
			'确定要删除这个文件夹吗？',
			'警告',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}
		)

		const response = await request({
			url: '/CustomArchiveSet/DeleteCustomArchiveSet/Del',
			method: 'GET',
			params: { id: data.dataId }
		})

		if (response.code == 200) {
			ElMessage.success(response.data || '文件夹删除成功')
			await fetchCascaderOptions()
			await fetchCustomFolders()
		} else {
			ElMessage.error(response.data || '文件夹删除失败')
		}
	} catch (error) {
		if (error !== 'cancel') {
			ElMessage.error('删除文件夹失败')
		}
	}
}

// 获取级联选择器选项
const fetchCascaderOptions = async () => {
	try {
		const response = await request({
			url: '/CustomArchiveSet/GetLevelListByUser/GetLevelList',
			method: 'GET'
		})
		if (response.data) {
			cascaderOptions.value = response.data
		}
	} catch (error) {
		ElMessage.error('获取父文件夹数据失败')
	}
}

// 添加文件夹
const handleAddFolder = async () => {
	if (!newFolderForm.value.name) {
		ElMessage.warning('请输入文件夹名称')
		return
	}

	try {
		let parentType = 5
		let parentIdValue = 0

		if (Array.isArray(newFolderForm.value.parentId)) {
			const lastNode = findNodeByPath(cascaderOptions.value, newFolderForm.value.parentId)
			if (lastNode) {
				parentType = lastNode.type
				parentIdValue = Number(lastNode.value)
			}
		} else {
			const parentNode = findNodeById(cascaderOptions.value, newFolderForm.value.parentId)
			if (parentNode) {
				parentType = parentNode.type
				parentIdValue = Number(parentNode.value)
			}
		}

		if (parentIdValue === null) {
			parentIdValue = Array.isArray(newFolderForm.value.parentId) ?
				Number(newFolderForm.value.parentId[newFolderForm.value.parentId.length - 1]) :
				Number(newFolderForm.value.parentId)
		}

		const payload = {
			Name: newFolderForm.value.name,
			Description: '手动添加归档集',
			ParentType: parentType,
			ParentId: parentIdValue
		}

		const response = await request({
			url: '/CustomArchiveSet/AddCustomeArchiveSet/Add',
			method: 'POST',
			data: payload
		})

		if (response.code == 200) {
			ElMessage.success(response.data || '文件夹创建成功')
			showAddFolderDialog.value = false
			newFolderForm.value.name = ''
			await fetchCascaderOptions()
			await fetchCustomFolders()
		} else {
			ElMessage.error(response.data || '创建文件夹失败')
		}
	} catch (error) {
		ElMessage.error('创建文件夹失败')
	}
}

// 辅助函数
function findNodeById(nodes, id) {
	for (const node of nodes) {
		if (String(node.value) === String(id)) return node
		if (node.children) {
			const found = findNodeById(node.children, id)
			if (found) return found
		}
	}
	return null
}

function findNodeByPath(nodes, pathArr) {
	let current = nodes
	let node = null
	for (const id of pathArr) {
		node = (current || []).find(n => String(n.value) === String(id))
		if (!node) return null
		current = node.children
	}
	return node
}

// 增强的文件夹数据查找方法 - 添加更多调试信息和容错处理
const findFolderDataById = (folders, id) => {
	console.log('查找文件夹数据 - ID:', id, '文件夹列表长度:', folders?.length)

	if (!folders || !Array.isArray(folders) || folders.length === 0) {
		console.warn('文件夹列表为空或无效:', folders)
		return null
	}

	// 递归查找函数
	const searchInFolders = (folderList, searchId, depth = 0) => {
		const indent = '  '.repeat(depth)
		console.log(`${indent}在层级 ${depth} 中查找ID: ${searchId}`)

		for (let i = 0; i < folderList.length; i++) {
			const folder = folderList[i]
			console.log(`${indent}检查文件夹 ${i}:`, {
				id: folder.id,
				dataId: folder.dataId,
				label: folder.label,
				type: folder.type
			})

			// 支持多种ID匹配方式
			if (String(folder.dataId) === String(searchId) ||
				String(folder.id) === String(searchId)) {
				console.log(`${indent}✅ 找到匹配的文件夹:`, folder)
				return folder
			}

			// 递归查找子文件夹
			if (folder.children && Array.isArray(folder.children) && folder.children.length > 0) {
				console.log(`${indent}递归查找子文件夹，子文件夹数量:`, folder.children.length)
				const found = searchInFolders(folder.children, searchId, depth + 1)
				if (found) {
					return found
				}
			}
		}

		console.log(`${indent}在层级 ${depth} 中未找到匹配项`)
		return null
	}

	const result = searchInFolders(folders, id)

	if (!result) {
		console.warn('❌ 未找到文件夹数据，ID:', id)
		console.log('可用的文件夹列表:', folders.map(f => ({
			id: f.id,
			dataId: f.dataId,
			label: f.label,
			type: f.type
		})))
	} else {
		console.log('✅ 成功找到文件夹数据:', result)
	}

	return result
}
// #endregion

// #region 搜索功能
// 本地搜索处理
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

// 清除本地搜索
const clearLocalSearch = () => {
	EmailTableData.value = [...originalEmailData.value]
}

// 重置搜索表单
const resetSearchForm = () => {
	Object.assign(searchForm, {
		sender: '',
		receiver: '',
		subject: '',
		includeWords: '',
		excludeWords: '',
		dateRange: '1',
		customDateRange: [],
		searchScope: '0',
		hasAttachment: false
	})

	isSearchMode.value = false
	lastSearchParams.value = null
	currentPage.value = 1
	pageSize.value = 20

	refreshCurrentView()
}

// 高级搜索请求
const handleAdvancedSearchRequest = async () => {
	try {
		const searchParams = {
			...lastSearchParams.value,
			PageNum: currentPage.value,
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
			originalEmailData.value = [...EmailTableData.value]
			totalItems.value = response.data.totalNum
		}
	} catch (error) {
		console.error('搜索失败:', error)
		ElMessage.error('搜索失败，请重试')
	}
}

// 处理高级搜索
const handleAdvancedSearch = async () => {
	try {
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

		if (searchForm.dateRange === 'custom' && searchForm.customDateRange?.length === 2) {
			searchParams.startDate = searchForm.customDateRange[0]
			searchParams.endDate = searchForm.customDateRange[1]
		} else if (searchForm.dateRange !== 'custom' && searchForm.dateRange) {
			const days = parseInt(searchForm.dateRange)
			searchParams.endDate = new Date()
			searchParams.startDate = new Date(Date.now() - days * 24 * 60 * 60 * 1000)
		}

		// 设置搜索状态
		setCurrentFolderState('search', 'advanced', '高级搜索结果', null, lastSearchParams.value)
		isSearchMode.value = true
		currentPage.value = 1

		await handleAdvancedSearchRequest()

		showAdvancedSearch.value = false
		ElMessage.success('搜索完成')
	} catch (error) {
		console.error('搜索失败:', error)
		ElMessage.error('搜索失败，请重试')
	}
}
// #endregion

// #region 分页处理 - 修复文件夹分页问题
const handleCurrentChange = async (newPage) => {
	try {
		console.log('📄 分页切换 - 当前状态:', currentFolderState.value, '新页码:', newPage)
		currentPage.value = newPage

		const state = currentFolderState.value

		if (state.type === 'search') {
			// 搜索模式分页
			await handleAdvancedSearchRequest()
		} else if (state.type === 'folder') {
			// 文件夹模式分页
			const folderData = state.folderData || findFolderDataById(emailFolders.value, state.id)
			if (folderData && folderData.type) {
				await getEmailArchiveList(newPage, pageSize.value, folderData.type, state.id)
			} else {
				throw new Error('文件夹数据丢失')
			}
		} else if (state.type === 'tag') {
			// 标签模式分页
			EmailTagIndex.value = state.id
			await getInboxEmail(newPage, pageSize.value, 1)
		} else if (state.type === 'system') {
			// 系统文件夹分页
			await getInboxEmail(newPage, pageSize.value, state.id)
		} else {
			throw new Error('未知的分页模式')
		}
	} catch (error) {
		console.error('❌ 分页处理失败:', error)
		ElMessage.error('分页操作失败，请重试')

		// 错误降级
		try {
			await MenuClick(1)
			currentPage.value = 1
		} catch (fallbackError) {
			console.error('❌ 降级处理失败:', fallbackError)
		}
	}
}


// 处理每页显示数量改变 - 修复版本
const handleSizeChange = async (newSize) => {
	try {
		console.log('每页数量改变 - 当前模式:', activeMenu.value, '新数量:', newSize)
		pageSize.value = newSize
		currentPage.value = 1 // 重置到第一页

		if (isSearchMode.value && lastSearchParams.value) {
			// 搜索模式
			console.log('执行搜索模式每页数量变更')
			await handleAdvancedSearchRequest()
		} else if (activeMenu.value.startsWith('folder-')) {
			// 文件夹模式 - 重点修复区域
			const folderId = activeMenu.value.replace('folder-', '')
			console.log('文件夹模式每页数量变更 - 文件夹ID:', folderId)

			// 验证文件夹数据
			const folderData = findFolderDataById(emailFolders.value, folderId)
			console.log('查找到的文件夹数据:', folderData)

			if (!folderData) {
				console.error('每页数量变更时找不到文件夹数据:', folderId)
				ElMessage.warning('文件夹数据丢失，正在重新加载...')

				// 尝试重新获取文件夹数据
				await fetchCustomFolders()
				const refreshedFolderData = findFolderDataById(emailFolders.value, folderId)

				if (refreshedFolderData && refreshedFolderData.type) {
					console.log('重新获取文件夹数据成功:', refreshedFolderData)
					await getEmailArchiveList(1, newSize, refreshedFolderData.type, folderId)
				} else {
					console.error('重新获取文件夹数据仍然失败，切换到收件箱')
					ElMessage.error('文件夹数据异常，已切换到收件箱')
					activeMenu.value = '1'
					await getInboxEmail(1, newSize, 1)
				}
				return
			}

			if (!folderData.type) {
				console.error('文件夹缺少type属性:', folderData)
				ElMessage.error('文件夹配置异常，请联系管理员')
				return
			}

			console.log('执行文件夹邮件获取（每页数量变更） - type:', folderData.type, 'dataId:', folderId)
			await getEmailArchiveList(1, newSize, folderData.type, folderId)

		} else {
			// 普通模式
			let emailType = activeMenu.value
			if (activeMenu.value.startsWith('tag-')) {
				emailType = '1'
			}
			console.log('执行普通模式每页数量变更 - emailType:', emailType)
			await getInboxEmail(1, newSize, emailType)
		}
	} catch (error) {
		console.error('每页数量变更处理失败:', error)
		ElMessage.error('操作失败，请重试')

		// 错误降级处理
		try {
			console.log('执行错误降级处理')
			activeMenu.value = '1'
			await getInboxEmail(1, newSize, 1)
			currentPage.value = 1
		} catch (fallbackError) {
			console.error('降级处理也失败:', fallbackError)
			ElMessage.error('系统异常，请刷新页面')
		}
	}
}


// 10. 页面刷新时的状态恢复（可选）
const restoreStateFromActiveMenu = () => {
	if (activeMenu.value.startsWith('folder-')) {
		const folderId = activeMenu.value.replace('folder-', '')
		const folderData = findFolderDataById(emailFolders.value, folderId)
		if (folderData) {
			setCurrentFolderState('folder', folderId, folderData.label, folderData, { type: folderData.type })
		}
	} else if (activeMenu.value.startsWith('tag-')) {
		const tagId = activeMenu.value.replace('tag-', '')
		const tagData = UserEmailTagList.value.find(tag => tag.id == tagId)
		if (tagData) {
			setCurrentFolderState('tag', tagId, tagData.emailTagName, tagData)
		}
	} else {
		const systemFolderNames = {
			'1': '收件箱',
			'2': '已发邮件',
			'3': '草稿箱',
			'4': '垃圾箱',
			'6': '归档邮件'
		}
		setCurrentFolderState('system', activeMenu.value, systemFolderNames[activeMenu.value])
	}
}

// #endregion

// #region 邮件详情处理
// 获取邮件附件
const GetEmailAttachment = async (emailId) => {
	try {
		const response = await request({
			url: 'Email/GetEmailAttachments/GetEmailAttachments',
			method: 'GET',
			params: { emailId: emailId }
		})
		console.log(response.data)
		return response.data || []
	} catch (error) {
		console.error('获取邮件附件失败:', error)
		return []
	}
}

// 优化后的处理行点击方法 - 确保标签数据同步
const handleRowClick = async (row, column, event) => {
	if (column.type === 'selection') {
		return
	}

	SelectEmailID.value = row.id

	try {
		const attachmentsList = await GetEmailAttachment(row.EmailID)
		currentEmail.value = {
			id: row.id,
			subject: row.subject,
			from: row.fromEmailAddress,
			to: row.toEmail,
			cc: row.ccEmail || null,
			date: row.date,
			content: row.content,
			attachments: attachmentsList.map(attachment => ({
				id: attachment.id,
				name: attachment.attachmentsName,
				fileUrl: attachment.attachmentsDownLoadUrl
			})),
			tags: []
		}

		EmailModel.id = row.id
		EmailModel.emailsubject = row.subject
		EmailModel.fromEmail = row.fromEmailAddress

		isLoadingTags.value = true
		markAsRead(row)

		// 清空并重新获取标签
		EmailTagcheckboxGroup.value = []
		EmailTagcheckboxoptions.value = []

		const response = await request({
			url: 'Email/GetUserEmailTagList/GetUserEmailTag',
			method: 'GET'
		})

		if (response?.data) {
			EmailTagcheckboxoptions.value = response.data.map(item => ({
				label: item.emailTagName,
				value: item.id
			}))

			// 使用最新的标签数据（优先使用当前行数据）
			if (row.emailTags) {
				const tagArray = row.emailTags.split(',')
				tagArray.forEach(tagId => {
					if (tagId && !isNaN(tagId)) {
						EmailTagcheckboxGroup.value.push(Number(tagId))
					}
				})
			}
		}

		showEmailDetail.value = true
	} catch (error) {
		console.error('获取标签列表失败:', error)
		ElMessage.error('获取标签列表失败')
	} finally {
		isLoadingTags.value = false
	}
}

// 6. 大幅优化的 backToList 方法
const backToList = async () => {
	if (await CheckShowEmailDetail() == false) {
		return
	}

	console.log('🔙 返回列表 - 当前状态:', currentFolderState.value)
	showEmailDetail.value = false

	try {
		const state = currentFolderState.value

		if (state.type === 'folder') {
			// 文件夹模式
			console.log('📁 文件夹模式返回，刷新归档邮件列表')

			let folderData = state.folderData

			// 如果没有缓存的文件夹数据，重新查找
			if (!folderData || !folderData.type) {
				console.log('📁 重新查找文件夹数据')
				await fetchCustomFolders()
				folderData = findFolderDataById(emailFolders.value, state.id)

				// 更新缓存的文件夹数据
				if (folderData) {
					currentFolderState.value.folderData = folderData
				}
			}

			if (folderData && folderData.type) {
				await getEmailArchiveList(currentPage.value, pageSize.value, folderData.type, state.id)
			} else {
				console.error('❌ 无法找到文件夹数据，执行降级处理')
				ElMessage.warning('文件夹数据丢失，切换到收件箱')
				await MenuClick(1)
			}

		} else if (state.type === 'tag') {
			// 标签模式
			console.log('🏷️ 标签模式返回，刷新标签邮件列表')
			EmailTagIndex.value = state.id
			await getInboxEmail(currentPage.value, pageSize.value, 1)

		} else if (state.type === 'search') {
			// 搜索模式
			console.log('🔍 搜索模式返回，刷新搜索结果')
			await handleAdvancedSearchRequest()

		} else if (state.type === 'system') {
			// 系统文件夹模式
			console.log('📧 系统文件夹模式返回，刷新邮件列表')
			await getInboxEmail(currentPage.value, pageSize.value, state.id)

		} else {
			// 降级处理：如果状态异常，切换到收件箱
			console.warn('⚠️ 未知的文件夹状态，切换到收件箱')
			await MenuClick(1)
		}

	} catch (error) {
		console.error('❌ 返回列表时刷新数据失败:', error)
		ElMessage.warning('数据刷新失败')

		// 错误降级：切换到收件箱
		try {
			await MenuClick(1)
		} catch (fallbackError) {
			console.error('❌ 降级处理也失败:', fallbackError)
			ElMessage.error('系统异常，请刷新页面')
		}
	}
}

// 检查邮件详情是否可以关闭
const CheckShowEmailDetail = async () => {
	return new Promise(async (resolve) => {
		if (!showEmailDetail.value) {
			resolve(true)
			return
		}

		if (currentEmail.value) {
			// 先检查前端是否已经选择了标签
			if (EmailTagcheckboxGroup.value && EmailTagcheckboxGroup.value.length > 0) {
				// 如果前端有标签，说明用户已经选择了标签（即使还没保存），需要先保存标签到后端
				try {
					await EditEmailTags()
					// 保存成功，允许返回
					resolve(true)
					return
				} catch (error) {
					console.error('保存标签失败:', error)
					// 保存失败，不允许返回，提示用户重新保存
					ElMessageBox.confirm(
						'标签保存失败，请重新保存标签后再返回。是否现在保存标签？',
						'提示',
						{
							confirmButtonText: '重新保存',
							cancelButtonText: '取消',
							type: 'error',
							closeOnClickModal: false,
							closeOnPressEscape: false
						}
					).then(() => {
						// 用户选择重新保存，打开标签面板
						tagPopoverVisible.value = true
						resolve(false)
					}).catch(() => {
						// 用户取消，不允许返回
						resolve(false)
					})
					return
				}
			}

			// 如果前端没有标签，再检查后端是否有标签
			try {
				const res = await request({
					url: 'Email/CheckEmailTagsByEmailID/CheckEmailTags',
					method: 'GET',
					params: { id: currentEmail.value.id }
				})

				if (res.code === 200 && !res.data) {
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
					).then(() => {
						tagPopoverVisible.value = true
						resolve(false)
					}).catch(() => {
						resolve(false)
					})
					return
				}
				resolve(true)
			} catch (error) {
				console.error('检查邮件标签失败:', error)
				resolve(true)
			}
			return
		}
		resolve(true)
	})
}

// 下载附件
const downloadAttachment = (id, fileName, fileUrl) => {
	alert(fileUrl);
	if (fileUrl) {
		const link = document.createElement('a')
		link.href = fileUrl
		link.download = fileName
		document.body.appendChild(link)
		link.click()
		document.body.removeChild(link)
	} else {
		request({
			url: `Email/DownloadAttachment/DownloadAttachment`,
			method: 'GET',
			params: { id: id },
			responseType: 'blob'
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

// HTML 内容样式
const emailContentStyle = computed(() => ({
	fontSize: '14px',
	lineHeight: '1.6',
	color: '#333',
	fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif'
}))
// #endregion

// #region 邮件操作
// 移动邮件
const MoveEmail = async (emailType) => {
	try {
		EmailModel.emailType = emailType
		const response = await request({
			url: 'Email/MoveEmail/MoveEmail',
			method: 'POST',
			data: EmailModel
		})

		if (response != null) {
			ElMessage({
				message: response.msg,
				type: 'success'
			})
		} else {
			console.error('移动邮件失败')
		}
	} catch (error) {
		console.error('移动邮件失败！😔错误内容：', error)
	}
}

// 修复移动邮件后的处理
const handleMoveEmail = async (command) => {
	try {
		await MoveEmail(command)
		showEmailDetail.value = false

		// 移动邮件后需要刷新当前视图
		if (activeMenu.value.startsWith('folder-')) {
			// 文件夹模式特殊处理
			const folderId = activeMenu.value.replace('folder-', '')
			const folderData = findFolderDataById(emailFolders.value, folderId)

			if (folderData && folderData.type) {
				await getEmailArchiveList(currentPage.value, pageSize.value, folderData.type, folderId)
			} else {
				await refreshCurrentView()
			}
		} else {
			await refreshCurrentView()
		}
	} catch (error) {
		console.error('移动邮件失败:', error)
		ElMessage.error('移动邮件失败')
	}
}

// 批量操作相关
const handleSelectionChange = (selection) => {
	console.log('📝 选择状态变化 - 新选择数量:', selection.length)
	console.log('📝 选择的邮件 IDs:', selection.map(item => item.id))

	selectedRows.value = selection

	// 调试信息
	if (selection.length === 0) {
		console.log('📝 所有选择已清空')
	} else {
		console.log('📝 当前选择的邮件:', selection.map(item => ({
			id: item.id,
			subject: item.subject
		})))
	}
}

// 5. 带确认的清除选择方法
const clearSelectionWithConfirm = async () => {
	if (selectedRows.value.length > 0) {
		try {
			await ElMessageBox.confirm(
				`确定要取消选择这 ${selectedRows.value.length} 封邮件吗？`,
				'确认取消选择',
				{
					confirmButtonText: '确定',
					cancelButtonText: '取消',
					type: 'info'
				}
			)
			clearSelection()
			ElMessage.success('已取消选择')
		} catch (error) {
			if (error !== 'cancel') {
				console.error('❌ 确认取消选择失败:', error)
			}
		}
	} else {
		ElMessage.info('当前没有选择的邮件')
	}
}

const showBatchActions = computed(() => {
	return selectedRows.value.length > 0
})

// 6. 监听选择状态变化
watch(selectedRows, (newSelection, oldSelection) => {
	console.log('🔍 选择状态监听 - 从', oldSelection?.length || 0, '变为', newSelection.length)

	// 如果选择状态异常，尝试修复
	if (newSelection.length < 0) {
		console.warn('⚠️ 检测到异常的选择状态，执行修复')
		selectedRows.value = []
	}
}, { deep: true })

// 7. 页面数据变化时自动清除选择
watch(EmailTableData, () => {
	console.log('📊 邮件数据已更新，自动清除选择')
	clearSelection()
}, { deep: false })

// 8. 全选和取消全选
const selectAll = () => {
	if (emailTable.value) {
		emailTable.value.toggleAllSelection()
		console.log('🔄 切换全选状态')
	}
}

const selectAllEmails = () => {
	if (emailTable.value) {
		EmailTableData.value.forEach(row => {
			emailTable.value.toggleRowSelection(row, true)
		})
		console.log('✅ 已选择所有邮件')
	}
}

const unselectAllEmails = () => {
	if (emailTable.value) {
		EmailTableData.value.forEach(row => {
			emailTable.value.toggleRowSelection(row, false)
		})
		console.log('✅ 已取消选择所有邮件')
	}
}

// 9. 选择指定邮件
const selectEmailById = (emailId) => {
	const email = EmailTableData.value.find(item => item.id === emailId)
	if (email && emailTable.value) {
		emailTable.value.toggleRowSelection(email, true)
		console.log('✅ 已选择邮件:', emailId)
	}
}

const unselectEmailById = (emailId) => {
	const email = EmailTableData.value.find(item => item.id === emailId)
	if (email && emailTable.value) {
		emailTable.value.toggleRowSelection(email, false)
		console.log('✅ 已取消选择邮件:', emailId)
	}
}

// 10. 表格状态检查和修复
const checkTableState = () => {
	console.log('🔍 检查表格状态')
	console.log('📊 表格引用存在:', !!emailTable.value)
	console.log('📊 邮件数据数量:', EmailTableData.value.length)
	console.log('📊 选择数量:', selectedRows.value.length)

	if (emailTable.value) {
		// 获取表格内部选择状态
		const tableSelection = emailTable.value.selection || []
		console.log('📊 表格内部选择数量:', tableSelection.length)

		// 检查数据一致性
		if (tableSelection.length !== selectedRows.value.length) {
			console.warn('⚠️ 检测到选择状态不一致，执行同步')
			selectedRows.value = [...tableSelection]
		}
	}
}

// 11. 组件挂载后的初始化
onMounted(() => {
	nextTick(() => {
		console.log('🚀 组件挂载完成，初始化表格状态')

		if (emailTable.value) {
			console.log('✅ 邮件表格引用已准备就绪')

			// 设置表格的默认配置
			console.log('⚙️ 配置表格默认行为')
		} else {
			console.warn('⚠️ 邮件表格引用未找到，延迟重试')

			// 延迟重试
			setTimeout(() => {
				if (emailTable.value) {
					console.log('✅ 延迟获取到邮件表格引用')
				} else {
					console.error('❌ 仍然无法获取邮件表格引用')
				}
			}, 1000)
		}
	})
})

const emailTable = ref(null)

const clearSelection = () => {
	console.log('🔄 开始清除选择状态')
	console.log('🔍 清除前选择数量:', selectedRows.value.length)

	try {
		// 第一步：清除响应式数据
		selectedRows.value = []
		console.log('✅ 响应式数据已清除')

		// 第二步：清除表格组件的选择状态
		if (emailTable.value) {
			emailTable.value.clearSelection()
			console.log('✅ 表格选择状态已清除')
		} else {
			console.warn('⚠️ 表格引用不存在，尝试通过 DOM 查找')

			// 备用方案：通过 DOM 操作清除选择
			const tableEl = document.querySelector('.el-table')
			if (tableEl) {
				const checkboxes = tableEl.querySelectorAll('.el-checkbox__input')
				checkboxes.forEach(checkbox => {
					if (checkbox.classList.contains('is-checked')) {
						checkbox.click() // 模拟点击取消选择
					}
				})
				console.log('✅ 通过 DOM 操作清除选择')
			}
		}

		// 第三步：强制更新界面
		nextTick(() => {
			console.log('✅ 清除选择完成，最终选择数量:', selectedRows.value.length)
		})

	} catch (error) {
		console.error('❌ 清除选择状态失败:', error)

		// 最后的备用方案：强制重置
		selectedRows.value = []
		nextTick(() => {
			if (emailTable.value) {
				emailTable.value.clearSelection()
			}
		})
	}
}

const tableRowClassName = ({ row }) => {
	const classes = []

	if (row.isRead === 1) {
		classes.push('unread-row')
	}

	if (selectedRows.value.some(selected => selected.id === row.id)) {
		classes.push('selected-row')
	}

	return classes.join(' ')
}

// 批量归档
const handleBatchArchive = async () => {
	if (selectedRows.value.length === 0) {
		ElMessage.warning('请先选择要归档的邮件')
		return
	}

	try {
		await ElMessageBox.confirm(
			`确定要将选中的 ${selectedRows.value.length} 封邮件归档吗？`,
			'确认归档',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}
		)

		isBatchProcessing.value = true
		const emailIds = selectedRows.value.map(row => row.id)

		const response = await request({
			url: 'Email/BatchArchiveEmails/BatchArchiveEmails',
			method: 'POST',
			data: {
				emailIds: emailIds,
				emailType: 6
			}
		})

		if (response.code == 200) {
			ElMessage.success(response.data || '批量归档成功')
			clearSelection()
			await refreshCurrentView()
		} else {
			ElMessage.error('批量归档失败')
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('批量归档失败:', error)
			ElMessage.error('批量归档失败，请重试')
		}
	} finally {
		isBatchProcessing.value = false
	}
}

// 批量删除
const handleBatchDelete = async () => {
	if (selectedRows.value.length === 0) {
		ElMessage.warning('请先选择要删除的邮件')
		return
	}

	try {
		await ElMessageBox.confirm(
			`确定要将选中的 ${selectedRows.value.length} 封邮件移动到垃圾箱吗？`,
			'确认删除',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}
		)

		isBatchProcessing.value = true
		const emailIds = selectedRows.value.map(row => row.id.toString())

		// 构建垃圾箱移动请求参数
		const trashFolderData = {
			isSystem: true,
			type: 4,
			dataId: 4,
			label: '垃圾箱'
		}

		const moveRequest = buildBatchMoveRequest(trashFolderData, emailIds)

		console.log('批量删除到垃圾箱请求参数:', moveRequest)

		const response = await request({
			url: 'Email/BatchMoveEmailToFolder/BatchMoveToFolder',
			method: 'POST',
			data: moveRequest
		})

		if (response.code === 200) {
			ElMessage.success(response.msg || `成功删除 ${selectedRows.value.length} 封邮件`)
			clearSelection()
			await refreshCurrentView()
		} else {
			ElMessage.error(response.msg || '批量删除失败')
		}
	} catch (error) {
		if (error !== 'cancel') {
			console.error('批量删除失败:', error)
			ElMessage.error('批量删除失败，请重试')
		}
	} finally {
		isBatchProcessing.value = false
	}
}
// #endregion

// #region 邮件编写相关
const showEmailDialog = ref(false)
const dialogTitle = ref('新邮件')
const emailType = ref('new')
const isFullscreen = ref(false)
const showCc = ref(false)
const fileList = ref([])
const fileInput = ref(null)
const quillEditor = ref(null)
const recipientTreeData = ref([])
const isSavingDraft = ref(false)

// 邮件表单数据
const emailForm = reactive({
	draftId: null,
	ToEmail: [],
	cc: [],
	bcc: [],
	subject: '',
	content: '',
	emailTags: [],
	originalMessageId: null,
	delta: new Delta()
})

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
		],
		clipboard: {
			matchVisual: false
		}
	},
	placeholder: '撰写邮件...',
	formats: [
		'header', 'font', 'size',
		'bold', 'italic', 'underline', 'strike', 'blockquote',
		'list', 'bullet', 'indent',
		'link', 'image', 'video',
		'color', 'background',
		'align', 'direction',
		'code-block', 'script'
	]
}

// 获取邮件联系人
const GetEmailContract = async () => {
	try {
		const response = await request({
			url: 'Email/GetEmailContact/GetEmailContact',
			method: 'GET'
		})
		recipientTreeData.value = response.data || []
	} catch (error) {
		console.error('获取联系人失败:', error)
	}
}

// 邮箱验证函数
const validateEmail = (email: string) => {
	const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
	return emailRegex.test(email)
}

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
	return Array.from(new Set(emails))
}

// 处理抄送列表
const handleCcList = (ccList) => {
	const currentUserEmail = ConfigEmailForm.email
	return uniqueEmails(parseEmailList(ccList))
		.filter(email => email !== currentUserEmail)
}

// 切换全屏状态
const toggleFullScreen = () => {
	isFullscreen.value = !isFullscreen.value
}

// 写邮件处理函数
const handleWriteEmail = async () => {
	dialogTitle.value = '新邮件'
	emailType.value = 'new'
	resetEmailForm()
	await GetEmailContract()
	showEmailDialog.value = true
}

// 编辑器事件处理
const onEditorReady = (quill) => {
	console.log('QuillEditor 已准备就绪:', quill)
}

const onTextChange = (delta, oldDelta, source) => {
	if (source === 'user') {
		console.log('用户输入内容变化')
	}
}

// 监听emailForm.content的变化
// watch(() => emailForm.delta, (d) => {
// 	const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
// 	if (quill && d) quill.setContents(d, 'silent')
// }, { deep: true })



// 处理回复邮件
const handleReply = async (replyAll = false) => {
	emailForm.originalMessageId = SelectEmailID.value
	emailType.value = replyAll ? 'replyAll' : 'reply'
	dialogTitle.value = replyAll ? '回复全部' : '回复'
	await GetEmailContract()

	emailForm.ToEmail = [currentEmail.value.from]

	if (replyAll && currentEmail.value.cc) {
		const ccList = handleCcList(currentEmail.value.cc)
		if (ccList.length > 0) {
			showCc.value = true
			emailForm.cc = ccList
		}
	} else {
		showCc.value = false
		emailForm.cc = []
	}

	emailForm.subject = `回复: ${currentEmail.value.subject}`

	// 处理原始邮件内容，确保HTML内容能够正确显示
	//const originalContent = processOriginalEmailContent(currentEmail.value.content)
	const originalContent = extractBodyHtml(currentEmail.value.content)
	// 创建格式化的原始邮件内容
	const formattedOriginalContent = `
		<div style="border-left: 3px solid #ddd; padding-left: 15px; margin: 20px 0; color: #666; background-color: #f9f9f9; padding: 15px; border-radius: 4px;">
			<div style="font-weight: bold; margin-bottom: 15px; color: #333; border-bottom: 1px solid #eee; padding-bottom: 10px;">
				------------------ 原始邮件 ------------------
			</div>
			<div style="margin-bottom: 8px;">
				<strong>发件人:</strong> ${currentEmail.value.from}
			</div>
			<div style="margin-bottom: 8px;">
				<strong>发送时间:</strong> ${currentEmail.value.date}
			</div>
			<div style="margin-bottom: 8px;">
				<strong>主题:</strong> ${currentEmail.value.subject}
			</div>
			<div style="margin-bottom: 8px;">
				<strong>收件人:</strong> ${currentEmail.value.to}
			</div>
			${currentEmail.value.cc ? `<div style="margin-bottom: 8px;"><strong>抄送:</strong> ${currentEmail.value.cc}</div>` : ''}
			<div style="margin-top: 15px; padding-top: 15px; border-top: 1px solid #eee;">
				${originalContent}
			</div>
		</div>
	`
	// 生成原文只读块
	quotedHtml.value = buildQuotedBlock(currentEmail.value)

	// 显示对话框
	showEmailDialog.value = true

	await nextTick() // 确保编辑器挂载

	const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
	if (quill) {
		quill.setContents([], 'silent')   // 清空编辑区（只写你的回复）
		quill.setSelection(0)
		emailForm.delta = quill.getContents()
	}
}

// 转发邮件处理
const handleForward = async () => {
	try {
		emailType.value = 'forward'
		dialogTitle.value = '转发'
		await GetEmailContract()

		// 基本表单
		emailForm.draftId = null
		emailForm.ToEmail = []
		emailForm.cc = []
		emailForm.bcc = []
		emailForm.subject = `转发: ${currentEmail.value.subject || ''}`

		// 取原文正文（去掉整页<html>等 + 消毒）
		const raw = currentEmail.value.content || ''
		const safeBody = extractBodyHtml(stripQuotedBlock(raw))

		// 组装"转发邮件"块（你原来的样式保留）
		const forwardedBlock = `
      <div style="border-left: 3px solid #007bff; padding-left: 15px; margin: 20px 0; color: #666; background-color: #f0f8ff; padding: 15px; border-radius: 4px;">
        <div style="font-weight: bold; margin-bottom: 15px; color: #333; border-bottom: 1px solid #007bff; padding-bottom: 10px;">
          ------------------ 转发邮件 ------------------
        </div>
        <div style="margin-bottom: 8px;"><strong>发件人:</strong> ${currentEmail.value.from || ''}</div>
        <div style="margin-bottom: 8px;"><strong>发送时间:</strong> ${currentEmail.value.date || ''}</div>
        <div style="margin-bottom: 8px;"><strong>主题:</strong> ${currentEmail.value.subject || ''}</div>
        <div style="margin-bottom: 8px;"><strong>收件人:</strong> ${currentEmail.value.to || ''}</div>
        ${currentEmail.value.cc ? `<div style="margin-bottom: 8px;"><strong>抄送:</strong> ${currentEmail.value.cc}</div>` : ''}
        <div style="margin-top: 15px; padding-top: 15px; border-top: 1px solid #007bff;">
          ${safeBody}
        </div>
      </div>
    `

		// 打开弹窗并把内容塞进 Quill（关键：用 convert → setContents）
		showEmailDialog.value = true
		await nextTick()

		const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
		if (quill) {
			const html = `<p><br></p>${forwardedBlock}`   // 顶部空一行，便于先写内容
			const delta = quill.clipboard.convert(html)   // HTML → Delta
			quill.setContents(delta, 'silent')
			quill.setSelection(0)                         // 光标放顶部
			// 同步回 v-model（contentType="delta"）
			emailForm.delta = quill.getContents()
			emailForm.content = quill.root.innerHTML
		}

		// 处理附件
		fileList.value = []
		if (currentEmail.value.attachments?.length) {
			for (const attachment of currentEmail.value.attachments) {
				try {
					const response = await request({
						url: 'Email/DownloadAttachment/DownloadAttachment',
						method: 'GET',
						params: { id: attachment.id },
						responseType: 'blob'
					})
					const base64Content = await blobToBase64(response.data)
					fileList.value.push({
						name: attachment.name,
						size: attachment.size,
						uid: Date.now() + Math.random().toString(36).substr(2, 9),
						raw: new File([response.data], attachment.name),
						base64Content
					})
				} catch (err) {
					console.error('获取附件失败:', err)
					ElMessage.warning(`附件 "${attachment.name}" 获取失败`)
				}
			}
		}
	} catch (e) {
		console.error('转发失败:', e)
		ElMessage.error('打开转发窗口失败，请重试')
	}
}

// blob 转 base64 的辅助函数
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

// 编辑草稿
const editDraft = async () => {
	try {
		dialogTitle.value = '编辑草稿'
		emailForm.draftId = currentEmail.value.id
		emailForm.ToEmail = currentEmail.value.to ? parseEmailList(currentEmail.value.to) : []
		emailForm.cc = currentEmail.value.cc ? parseEmailList(currentEmail.value.cc) : []
		emailForm.subject = currentEmail.value.subject || ''
		emailForm.content = currentEmail.value.content || ''

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
		await nextTick()

		// ★ 从草稿 EmailContent 拆分
		const combined = currentEmail.value.content || ''
		const { replyHtml, quotedHtml: quotedFromDraft } = splitCombinedContent(combined)

		// 只读区：原文块
		quotedHtml.value = quotedFromDraft || ''   // 没有的话可保持空
		showQuoted.value = false                   // 默认折叠（按你习惯）

		// 编辑器：只放"回复部分"
		const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
		if (quill) {
			const delta = quill.clipboard.convert(replyHtml || '')
			quill.setContents(delta, 'silent')
			quill.setSelection(quill.getLength(), 0)
			emailForm.delta = quill.getContents()
			emailForm.content = quill.root.innerHTML // 如果你别处还用到
		}
	} catch (error) {
		console.error('编辑草稿失败:', error)
		ElMessage.error('编辑草稿失败，请重试')
	}
}

// 文件相关处理
const handleFileChange = (file) => {
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

const triggerFileInput = () => {
	fileInput.value.click()
}

const handleFileInputChange = (event) => {
	const files = Array.from(event.target.files)
	files.forEach(file => {
		if (handleFileChange(file)) {
			fileList.value.push({
				name: (file as File).name,
				size: (file as File).size,
				raw: file,
				uid: Date.now() + Math.random().toString(36).substr(2, 9)
			})
		}
	})
	event.target.value = ''
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

// 重置表单
function resetEmailForm() {
	emailForm.draftId = null
	emailForm.ToEmail = []
	emailForm.cc = []
	emailForm.bcc = []
	emailForm.subject = ''
	emailForm.content = ''           // 如果还在用
	emailForm.delta = new Delta()    // 你当前用的 delta

	fileList.value = []

	// 关键：清原文块 & 恢复默认展开状态
	quotedHtml.value = ''            // 原文只读块内容清空
	showQuoted.value = false         // 默认折叠（或 true，看你偏好）

	// 可选：清空编辑器
	const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
	if (quill) quill.setContents([], 'silent')
}

// 丢弃邮件
const discardEmail = () => {
	resetEmailForm()
	showEmailDialog.value = false
}

// 关闭对话框处理
const handleDialogClose = () => {
	resetEmailForm()
}

// ====== 统一标记（尽量独特，避免误伤）======
const QUOTED_START = '<!--EMAIL-QUOTED:START-->'
const QUOTED_END = '<!--EMAIL-QUOTED:END-->'

// 去掉已有的原文块（防止重复嵌套）
function stripQuotedBlock(html: string) {
	return (html || '').replace(
		/<!--EMAIL-QUOTED:START-->[\s\S]*?<!--EMAIL-QUOTED:END-->/i,
		''
	).trim()
}

// 合并：回复 + 原文块（存库/发信时用）
function combineReplyAndQuoted(replyHtml: string, quotedHtml: string) {
	const cleanReply = stripQuotedBlock(replyHtml)
	if (quotedHtml && quotedHtml.trim()) {
		return `${cleanReply}\n${QUOTED_START}\n${quotedHtml}\n${QUOTED_END}`
	}
	return cleanReply
}

// 拆分：从存库的 EmailContent 拆出 回复部分 / 原文块（编辑草稿时用）
function splitCombinedContent(combinedHtml: string) {
	const re = /<!--EMAIL-QUOTED:START-->[\s\S]*?<!--EMAIL-QUOTED:END-->/i
	const m = (combinedHtml || '').match(/<!--EMAIL-QUOTED:START-->([\s\S]*?)<!--EMAIL-QUOTED:END-->/i)
	const quoted = m ? m[1].trim() : ''
	const reply = (combinedHtml || '').replace(re, '').trim()
	return { replyHtml: reply, quotedHtml: quoted }
}

// 暂存草稿
const saveDraft = async () => {
	if (!emailForm.ToEmail?.length && !emailForm.subject && !emailForm.content) {
		ElMessage.warning('邮件内容为空，无需保存')
		return
	}

	try {
		isSavingDraft.value = true

		let attachments = []
		if (fileList.value && fileList.value.length > 0) {
			attachments = await Promise.all(fileList.value.map(async (file) => {
				if (file.fileUrl) {
					return {
						FileName: file.name,
						FileSize: file.size,
						FileUrl: file.fileUrl
					}
				}

				if (file.raw) {
					const base64Content = await convertFileToBase64(file.raw)
					return {
						FileName: file.name,
						FileSize: file.size,
						FileContent: base64Content,
						ContentType: file.raw.type
					}
				}

				return {
					FileName: file.name,
					FileSize: file.size
				}
			}))
		}
		const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill

		const replyHtml = quill ? quill.root.innerHTML : (emailForm.content || '')

		// ★ 关键：把回复区 + 原文块 合并成一个 HTML
		const combinedHtml = combineReplyAndQuoted(replyHtml, quotedHtml.value || '')

		const draftData = {
			DraftId: emailForm.draftId,
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc,
			BccEmail: emailForm.bcc,
			Subject: emailForm.subject,
			EmailContent: combinedHtml,
			QuotedHtml: quotedHtml.value,
			Attachments: attachments
		}

		const response = await request({
			url: 'Email/SaveDraftEmail/SaveDraftEmail',
			method: 'POST',
			data: draftData
		})

		if (response.code === 200) {
			ElMessage.success('草稿保存成功')
			if (activeMenu.value === '3') {
				await refreshCurrentView()
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

// 发送邮件
const sendEmail = async () => {
	try {
		const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
		const replyHtml = quill ? quill.root.innerHTML : ''
		// 组合：你的回复 + 两个空行 + 原文只读块
		emailForm.content = `${replyHtml}<p><br></p><p><br></p>${quotedHtml.value}`
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

// 发送新邮件
const sendNewEmail = async () => {
	try {
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

		await appendSignatureIfEnabled()

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

		const emailData = {
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc || [],
			BccEmail: emailForm.bcc || [],
			Subject: emailForm.subject,
			EmailContent: emailForm.content,
			Attachments: attachments,
			EmailTags: emailForm.emailTags,
			EmailTagNames: EmailTagcheckboxoptions.value.find(option => option.value === emailForm.emailTags)?.label,
			originalMessageId: Number(emailForm.originalMessageId)
		}
		const quill = quillEditor.value?.getQuill?.() || quillEditor.value?.quill
		if (quill) {
			emailForm.content = quill.root.innerHTML   // ✅ 用编辑器渲染出来的 HTML
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
				if (activeMenu.value === '2') {
					await refreshCurrentView()
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
	try {
		if (!emailForm.ToEmail.length) {
			ElMessage.warning('请填写收件人')
			return
		}
		if (!emailForm.subject) {
			ElMessage.warning('请填写主题')
			return
		}

		await appendSignatureIfEnabled()

		const attachments = await Promise.all(fileList.value.map(async file => {
			if (file.id && file.fileUrl) {
				return {
					Id: file.id,
					FileName: file.name,
					FileSize: file.size,
					FileUrl: file.fileUrl,
					AttachmentId: file.id
				}
			}

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

		const draftData = {
			DraftId: emailForm.draftId,
			ToEmail: emailForm.ToEmail,
			CcEmail: emailForm.cc || [],
			BccEmail: emailForm.bcc || [],
			Subject: emailForm.subject,
			EmailContent: emailForm.content,
			Attachments: attachments,
			EmailTags: emailForm.emailTags,
			EmailTagNames: EmailTagcheckboxoptions.value.find(option => option.value === emailForm.emailTags)?.label
		}

		const loading = ElLoading.service({
			lock: true,
			text: '正在发送草稿邮件，请稍候...',
			background: 'rgba(0, 0, 0, 0.7)'
		})

		try {
			const response = await request({
				url: 'Email/SendFromDraft/SendFromDraft',
				method: 'POST',
				data: draftData
			})

			if (response.code === 200) {
				ElMessage.success('草稿邮件发送成功')
				showEmailDetail.value = false
				resetEmailForm()
				await refreshCurrentView()
			} else {
				ElMessage.error(response.msg || '发送失败')
			}
		} finally {
			loading.close()
		}
	} catch (error) {
		console.error('发送草稿失败:', error)
		ElMessage.error('发送草稿失败，请重试')
	}
}
// #endregion

// #region 签名管理
const signatureDialog = ref(false)
const currentSignature = ref({
	id: 0,
	name: '默认签名',
	content: '',
	enabled: false
})

const signatureToolbar = [
	['bold', 'italic', 'underline', 'strike'],
	[{ 'font': [] }],
	[{ 'size': ['small', false, 'large', 'huge'] }],
	[{ 'color': [] }, { 'background': [] }],
	[{ 'align': [] }],
	['link'],
	['clean']
]

// 打开签名设置对话框
const openSignatureDialog = async () => {
	await getUserSignature()
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
			currentSignature.value.id = response.data[0].id
			currentSignature.value.content = response.data[0].signature_content
			currentSignature.value.enabled = response.data[0].isEnable == 1 ? true : false
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

// 添加签名
const appendSignatureIfEnabled = async () => {
	try {
		await getUserSignature()
		if (currentSignature.value.enabled && currentSignature.value.content) {
			const signatureContent = currentSignature.value.content

			if (emailType.value === 'reply' || emailType.value === 'replyAll' || emailType.value === 'forward') {
				const separatorIndex = emailForm.content.indexOf('------------------ 原始邮件 ------------------')
				const forwardSeparatorIndex = emailForm.content.indexOf('------------------ 转发邮件 ------------------')

				const actualSeparatorIndex = Math.min(
					separatorIndex === -1 ? Infinity : separatorIndex,
					forwardSeparatorIndex === -1 ? Infinity : forwardSeparatorIndex
				)

				if (actualSeparatorIndex !== Infinity) {
					const beforeSeparator = emailForm.content.substring(0, actualSeparatorIndex)
					const afterSeparator = emailForm.content.substring(actualSeparatorIndex)

					if (!beforeSeparator.includes(signatureContent)) {
						emailForm.content = beforeSeparator + `<br><br>${signatureContent}` + afterSeparator
					}
				}
			} else {
				if (!emailForm.content.includes(signatureContent)) {
					emailForm.content += `<br><br>${signatureContent}`
				}
			}
		}
	} catch (error) {
		console.error('获取签名失败:', error)
	}
}
// #endregion

// #region 提醒功能
const reminderDialogVisible = ref(false)
const reminderForm = ref({
	title: '',
	content: '',
	reminderTime: null
})

const openReminderDialog = (email) => {
	reminderForm.value.title = '邮件提醒'
	reminderForm.value.content = email.from + '发来的邮件'
	reminderDialogVisible.value = true
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
				userId: '',
				title: reminderForm.value.title,
				content: reminderForm.value.content,
				reminderTime: reminderForm.value.reminderTime
			}
		})

		if (res.code === 200) {
			ElMessage.success('提醒设置成功')
			reminderDialogVisible.value = false
		} else {
			ElMessage.error(res.msg || '设置失败')
		}
	} catch (error) {
		console.error('设置提醒失败:', error)
		ElMessage.error('设置提醒失败：' + (error.message || '未知错误'))
	}
}
// #endregion

// #region 其他状态和变量
const tagPopoverVisible = ref(false)
const tagPopover = ref(null)

// 自动保存相关
let autoSaveTimer = null
const AUTO_SAVE_INTERVAL = 900000 // 900秒

const startAutoSave = () => {
	stopAutoSave()
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

// 监听对话框状态
watch(showEmailDialog, (newVal) => {
	if (newVal) {
		startAutoSave()
	} else {
		stopAutoSave()
	}
})

// 监听文件夹对话框状态
watch(showAddFolderDialog, (val) => {
	if (val) fetchCascaderOptions()
})
// #endregion

// #region 生命周期和初始化
onMounted(async () => {
	try {
		// 初始化邮箱配置
		await initEmailConfig()

		// 验证邮箱配置并获取数据
		await VerifyUserEmailConfigurationExists()

		// 获取联系人数据
		await GetEmailContract()

		// 获取标签列表
		await GetUserEmailTagList()

		// 检查路由参数，如果有id参数则自动打开邮件详情
		await nextTick()
		console.log('路由对象:', route)
		console.log('路由查询参数:', route?.query)

		if (route && route.query && route.query.id) {
			const emailId = route.query.id
			console.log('检测到路由参数，自动打开邮件详情，ID:', emailId)
			await autoOpenEmailDetail(emailId)
		} else {
			console.log('未检测到路由参数或参数为空')
		}
	} catch (error) {
		console.error('初始化失败:', error)
	}
})

// 自动打开邮件详情的函数
const autoOpenEmailDetail = async (emailId) => {
	// 防止重复执行
	if (isAutoOpeningEmail.value) {
		console.log('正在执行自动打开邮件，跳过重复请求')
		return
	}

	isAutoOpeningEmail.value = true

	try {
		console.log('开始自动打开邮件详情，ID:', emailId)

		// 显示搜索状态提示
		const loading = ElLoading.service({
			lock: true,
			text: `正在搜索邮件 (ID: ${emailId})...`,
			background: 'rgba(0, 0, 0, 0.7)'
		})

		// 首先尝试通过API直接获取邮件详情（不依赖当前列表，即使页面上没有该条记录也能打开）
		try {
			const response = await request({
				url: 'Email/GetEmailById/GetEmailById',
				method: 'GET',
				params: { id: emailId }
			})

			// 接口明确返回不存在或无数据：直接提示并返回收件箱，不再查当前列表
			if (response.code === 404 || (response.code === 200 && !response.data)) {
				loading.close()
				ElMessage.warning('该邮件不存在或已被删除，已返回收件箱')
				await MenuClick(1)
				return
			}

			if (response.code === 200 && response.data) {
				const emailData = response.data
				console.log('通过API获取到邮件数据:', emailData)
				console.log('发件人信息:', {
					fromEmail: emailData.fromEmail,
					fromEmailAddress: emailData.fromEmailAddress
				})
				console.log('收件人信息:', {
					toEmail: emailData.toEmail,
					toEmailAddress: emailData.toEmailAddress
				})

				// 关闭加载提示
				loading.close()

				// 构造邮件对象
				const targetEmail = {
					id: emailData.id,
					subject: emailData.emailsubject,
					from: emailData.fromEmail, // 发件人完整信息（包含姓名）
					to: emailData.toEmail, // 收件人完整信息（包含姓名）
					cc: emailData.ccEmail || null,
					date: emailData.emaildate,
					content: emailData.emailContent,
					emailTags: emailData.emailTags,
					EmailID: emailData.emailID,
					hasAttachments: emailData.isAttachments === 1,
					isRead: emailData.isRead,
					fromEmailAddress: emailData.fromEmailAddress, // 发件人邮箱地址
					toEmailAddress: emailData.toEmailAddress, // 收件人邮箱地址
					// 添加其他可能需要的字段
					fromEmail: emailData.fromEmail,
					toEmail: emailData.toEmail,
					ccEmail: emailData.ccEmail
				}

				console.log('构造的邮件对象:', targetEmail)
				console.log('邮件对象中的发件人:', targetEmail.from)
				console.log('邮件对象中的收件人:', targetEmail.to)

				// 根据邮件类型设置正确的文件夹状态
				const emailType = emailData.emailType || '1'
				const menuNames = {
					'1': '收件箱',
					'2': '已发邮件',
					'3': '草稿箱',
					'4': '垃圾箱',
					'6': '归档邮件'
				}

				setCurrentFolderState('system', emailType, menuNames[emailType])

				// 模拟点击行来打开邮件详情
				await handleRowClick(targetEmail, {}, {})
				ElMessage.success('邮件详情已打开')
				return
			} else {
				console.log('API返回数据格式不正确:', response)
				ElMessage.warning('邮件数据格式异常，尝试其他方式获取')
			}
		} catch (apiError) {
			// 若为 404 等“不存在”类错误，直接提示并返回收件箱
			const status = apiError?.response?.status
			const code = apiError?.response?.data?.code
			if (status === 404 || code === 404) {
				loading.close()
				ElMessage.warning('该邮件不存在或已被删除，已返回收件箱')
				await MenuClick(1)
				return
			}
			console.log('API获取邮件失败，尝试在现有列表中搜索:', apiError)
			ElMessage.warning('API获取失败，尝试在现有列表中搜索')
		}

		// 如果API获取失败，尝试在现有邮件列表中搜索
		let targetEmail = null
		let foundInType = null
		let foundInFolder = null

		// 在现有邮件列表中搜索
		if (EmailTableData.value?.length > 0) {
			const foundEmail = EmailTableData.value.find(item =>
				item.id.toString() === emailId.toString()
			)

			if (foundEmail) {
				targetEmail = foundEmail
				console.log('在现有列表中找到目标邮件:', targetEmail)
			} else {
				console.log('在现有列表中未找到邮件，ID:', emailId)
			}
		} else {
			console.log('当前邮件列表为空，无法搜索')
		}

		// 关闭加载提示
		loading.close()

		// 如果找到了邮件，打开详情
		if (targetEmail) {
			console.log('找到目标邮件，自动打开详情:', targetEmail)

			if (foundInFolder) {
				// 在自定义文件夹中找到的邮件
				setCurrentFolderState('folder', foundInFolder.dataId, foundInFolder.label, foundInFolder, { type: foundInFolder.type })
			} else {
				// 在系统文件夹中找到的邮件
				const menuNames = {
					'1': '收件箱',
					'2': '已发邮件',
					'3': '草稿箱',
					'4': '垃圾箱',
					'6': '归档邮件'
				}
				setCurrentFolderState('system', foundInType || '1', menuNames[foundInType || '1'])
			}

			// 模拟点击行来打开邮件详情
			await handleRowClick(targetEmail, {}, {})
			ElMessage.success(`邮件详情已打开 (ID: ${emailId})`)
		} else {
			// API 未返回数据且当前列表也没有：邮件不存在或不在当前视图
			console.warn('在所有邮件文件夹中未找到对应的邮件，ID:', emailId)
			ElMessage.warning('该邮件不存在或已被删除，已返回收件箱')

			// 返回收件箱
			await MenuClick(1)
		}
	} catch (error) {
		console.error('自动打开邮件详情失败:', error)
		ElMessage.error('自动打开邮件详情失败')
	} finally {
		// 重置标志
		isAutoOpeningEmail.value = false
	}
}

onUnmounted(() => {
	stopAutoSave()
})

// 监听路由变化，处理URL参数
watch(() => route.query, (newQuery) => {
	console.log('路由查询参数变化:', newQuery)
	if (newQuery && newQuery.id) {
		console.log('检测到路由参数变化，自动打开邮件详情，ID:', newQuery.id)
		// 延迟执行，确保路由和组件完全加载
		setTimeout(() => {
			autoOpenEmailDetail(newQuery.id)
		}, 500)
	}
}, { immediate: true })

// 监听路由路径变化，处理URL参数
watch(() => route.path, (newPath) => {
	console.log('路由路径变化:', newPath)
	if (newPath === '/email' && route.query && route.query.id) {
		console.log('检测到邮件页面路由，自动打开邮件详情，ID:', route.query.id)
		// 延迟执行，确保路由和组件完全加载
		setTimeout(() => {
			autoOpenEmailDetail(route.query.id)
		}, 500)
	}
}, { immediate: true })

// #endregion

// #region 批量移动至文件夹功能
// 新增状态变量
const showBatchMoveToFolderDialog = ref(false)
const isBatchMovingToFolder = ref(false)
const selectedFolderData = ref(null)
const selectedFolderName = ref('')
const flattenedFolderOptions = ref([])

// 文件夹树的属性配置
const folderTreeProps = {
	children: 'children',
	label: 'label',
	value: 'value'
}

// 显示移动对话框时构建文件夹树
const showBatchMoveDialog = async () => {
	if (selectedRows.value.length === 0) {
		ElMessage.warning('请先选择要移动的邮件')
		return
	}

	try {
		// 确保有最新的文件夹数据
		await fetchCustomFolders()

		// 构建完整的文件夹选择树
		buildCompletefolderTreeOptions()

		// 重置选择状态
		selectedFolderData.value = null
		selectedFolderName.value = ''

		// 显示对话框
		showBatchMoveToFolderDialog.value = true

	} catch (error) {
		console.error('加载文件夹数据失败:', error)
		ElMessage.error('加载文件夹数据失败，请重试')
	}
}

// 优化构建文件夹树选项的方法
const buildFolderTreeOptions = () => {
	console.log('构建文件夹树选项，原始数据:', emailFolders.value)

	// 系统默认文件夹
	const systemFolders = [
		{
			value: 'inbox-1',
			label: '收件箱',
			type: 1,
			dataId: 1,
			isSystem: true,
			children: []
		},
		{
			value: 'sent-2',
			label: '已发送',
			type: 2,
			dataId: 2,
			isSystem: true,
			children: []
		},
		{
			value: 'draft-3',
			label: '草稿箱',
			type: 3,
			dataId: 3,
			isSystem: true,
			children: []
		},
		{
			value: 'trash-4',
			label: '垃圾箱',
			type: 4,
			dataId: 4,
			isSystem: true,
			children: []
		},
		{
			value: 'archive-6',
			label: '归档邮件',
			type: 6,
			dataId: 6,
			isSystem: true,
			children: []
		}
	]

	// 递归转换自定义文件夹数据
	const convertFolderData = (folders, parentPath = '') => {
		if (!folders || !Array.isArray(folders)) {
			return []
		}

		return folders.map(folder => {
			const folderValue = `folder-${folder.dataId || folder.id}`
			const folderPath = parentPath ? `${parentPath}/${folder.label}` : folder.label

			const converted = {
				value: folderValue,
				label: folder.label || folder.name || '未命名文件夹',
				type: folder.type,
				dataId: folder.dataId || folder.id,
				isSystem: folder.isSystem || false,
				children: []
			}

			// 递归处理子文件夹
			if (folder.children && folder.children.length > 0) {
				converted.children = convertFolderData(folder.children, folderPath)
			}

			return converted
		})
	}

	// 转换自定义文件夹
	const customFolders = convertFolderData(emailFolders.value)

	// 组合所有文件夹选项
	flattenedFolderOptions.value = [
		{
			value: 'system',
			label: '系统文件夹',
			isSystem: true,
			children: systemFolders
		},
		{
			value: 'custom',
			label: '自定义文件夹',
			isSystem: false,
			children: customFolders
		}
	]

	console.log('构建完成的文件夹树:', flattenedFolderOptions.value)
}

// 修改 handleFolderSelect 方法，增加调试信息和验证：
const handleFolderSelect = (data, node) => {
	console.log('=== 文件夹选择调试信息 ===')
	console.log('选择的节点数据:', data)
	console.log('节点标签:', node.label)

	// 详细分析文件夹类型
	let analysisResult = {
		isSystemFolder: false,
		isHierarchyFolder: false,
		actualType: null,
		conflictRisk: false
	}

	if (data.isSystem && [1, 2, 3, 4, 6].includes(data.type)) {
		analysisResult.isSystemFolder = true
		analysisResult.actualType = 'system'
	} else {
		analysisResult.isHierarchyFolder = true
		analysisResult.actualType = 'hierarchy'

		// 检查是否存在ID冲突风险
		if ([1, 2, 3, 4, 6].includes(data.dataId)) {
			analysisResult.conflictRisk = true
			console.warn('⚠️ 检测到潜在的ID冲突风险:', {
				dataId: data.dataId,
				type: data.type,
				label: data.label
			})
		}
	}

	console.log('文件夹类型分析:', analysisResult)
	console.log('节点详细信息:', {
		value: data.value,
		label: data.label,
		type: data.type,           // 原始层级类型
		dataId: data.dataId,       // 具体ID（可能冲突）
		isSystem: data.isSystem,
		folderCategory: data.folderCategory, // 新增字段
		determinedFolderType: determineFolderLevelType(data)  // 计算后的类型
	})

	// 不允许选择分组节点
	if (['system-folders', 'hierarchy-folders'].includes(data.value)) {
		ElMessage.warning('请选择具体的文件夹')
		return
	}

	// 显示详细的选择信息
	const folderType = getFolderTypeName(data)
	const message = `已选择${folderType}: ${data.label} (ID: ${data.dataId}, 计算类型: ${determineFolderLevelType(data)})`
	console.log(message)
	ElMessage.info(message)

	selectedFolderData.value = data
	selectedFolderName.value = node.label

	// 生成测试请求来验证参数
	const testRequest = buildBatchMoveRequest(data, ['test-email-id'])
	console.log('测试请求参数:', testRequest)
	console.log('=== 调试信息结束 ===')
}

// 确保文件夹数据结构包含足够的信息
const convertEmailFoldersToHierarchy = (folders) => {
	if (!folders || !Array.isArray(folders)) {
		return []
	}

	return folders.map(folder => {
		const converted = {
			value: `folder-${folder.dataId || folder.id}`,
			label: folder.label || folder.name,
			type: folder.type,                    // 原始层级类型（1=大洲,2=国家等）
			dataId: folder.dataId || folder.id,   // 具体的ID（可能与系统文件夹ID重复）
			isSystem: false,                      // 明确标记为非系统文件夹
			folderCategory: 'hierarchy',          // 新增：文件夹类别
			children: folder.children ? convertEmailFoldersToHierarchy(folder.children) : []
		}

		console.log('转换文件夹数据:', {
			原始: {
				label: folder.label,
				type: folder.type,
				dataId: folder.dataId,
				isSystem: folder.isSystem
			},
			转换后: {
				label: converted.label,
				type: converted.type,
				dataId: converted.dataId,
				isSystem: converted.isSystem,
				folderCategory: converted.folderCategory,
				determinedType: determineFolderLevelType(converted)
			}
		})

		return converted
	})
}

// 同时，需要修改 buildSystemFolders 方法，确保 folderCategory 字段正确：
const buildSystemFolders = () => {
	return [
		{
			value: 'inbox-1',
			label: '收件箱',
			type: 1,
			dataId: 1,
			isSystem: true,
			folderCategory: 'system'
		},
		{
			value: 'sent-2',
			label: '已发送',
			type: 2,
			dataId: 2,
			isSystem: true,
			folderCategory: 'system'
		},
		{
			value: 'draft-3',
			label: '草稿箱',
			type: 3,
			dataId: 3,
			isSystem: true,
			folderCategory: 'system'
		},
		{
			value: 'trash-4',
			label: '垃圾箱',
			type: 4,
			dataId: 4,
			isSystem: true,
			folderCategory: 'system'
		},
		{
			value: 'archive-6',
			label: '归档邮件',
			type: 6,
			dataId: 6,
			isSystem: true,
			folderCategory: 'system'
		}
	]
}

// 执行批量移动 - 使用正确的BatchMoveToFolder接口
const handleBatchMoveToFolder = async () => {
	if (!selectedFolderData.value) {
		ElMessage.warning('请选择目标位置')
		return
	}

	if (selectedRows.value.length === 0) {
		ElMessage.warning('没有选中的邮件')
		return
	}

	try {
		await ElMessageBox.confirm(
			`确定要将选中的 ${selectedRows.value.length} 封邮件移动到 "${selectedFolderName.value}" 吗？`,
			'确认移动',
			{
				confirmButtonText: '确定移动',
				cancelButtonText: '取消',
				type: 'warning'
			}
		)

		isBatchMovingToFolder.value = true
		const emailIds = selectedRows.value.map(row => row.id.toString())

		// 构建移动请求参数
		const moveRequest = buildBatchMoveRequest(selectedFolderData.value, emailIds)

		console.log('批量移动请求参数:', moveRequest)

		const response = await request({
			url: 'Email/BatchMoveEmailToFolder/BatchMoveToFolder',
			method: 'POST',
			data: moveRequest
		})

		if (response.code === 200) {
			ElMessage.success(response.msg || `成功移动 ${selectedRows.value.length} 封邮件`)
			showBatchMoveToFolderDialog.value = false
			clearSelection()
			await refreshCurrentView()
		} else {
			ElMessage.error(response.msg || '移动邮件失败')
		}

	} catch (error) {
		if (error !== 'cancel') {
			console.error('批量移动邮件失败:', error)
			ElMessage.error('移动邮件失败，请重试')
		}
	} finally {
		isBatchMovingToFolder.value = false
	}
}

// 重新设计的批量移动请求参数构建
const buildBatchMoveRequest = (folderData, emailIds) => {
	const request: any = {
		emailIds: emailIds
	}

	console.log('构建请求参数，文件夹数据:', folderData)

	// 明确区分不同类型的文件夹
	if (folderData.isSystem && [1, 2, 3, 4, 6].includes(folderData.type)) {
		// 系统文件夹（收件箱、发件箱、草稿箱、垃圾箱、归档箱）
		request.targetType = 'system'
		request.targetFolderId = folderData.dataId || folderData.id
		request.folderType = folderData.type  // 系统文件夹类型：1,2,3,4,6

		// 为系统文件夹添加 folderContext
		request.folderContext = {
			originalType: folderData.type,      // 原始类型
			isHierarchy: false,                 // 明确标记为非层级文件夹
			levelName: getSystemFolderLevelName(folderData.type),  // 系统文件夹级别名称
			isSystem: true                      // 明确标记为系统文件夹
		}

		console.log('系统文件夹请求:', request)
	} else {
		// 层级文件夹或自定义文件夹
		request.targetType = 'folder'
		request.targetFolderId = folderData.dataId || folderData.id  // 具体的ID

		// 关键修改：使用明确的层级类型区分
		request.folderType = determineFolderLevelType(folderData)

		// 添加额外的上下文信息来帮助后端识别
		request.folderContext = {
			originalType: folderData.type,      // 原始类型
			isHierarchy: !folderData.isSystem,  // 是否为层级文件夹
			levelName: getFolderLevelName(folderData.type),  // 层级名称
			isSystem: false                     // 明确标记为非系统文件夹
		}

		console.log('层级/自定义文件夹请求:', {
			targetType: request.targetType,
			targetFolderId: request.targetFolderId,
			folderType: request.folderType,
			folderContext: request.folderContext,
			folderData: folderData
		})
	}

	return request
}

// 新增：获取系统文件夹级别名称的方法
const getSystemFolderLevelName = (type) => {
	const systemLevelNames = {
		1: 'inbox',      // 收件箱
		2: 'sent',       // 发件箱
		3: 'draft',      // 草稿箱
		4: 'trash',      // 垃圾箱
		6: 'archive'     // 归档箱
	}
	return systemLevelNames[type] || 'unknown'
}

// 确定文件夹层级类型（使用不同的数值范围避免冲突）
const determineFolderLevelType = (folderData) => {
	if (folderData.isSystem) {
		// 系统文件夹：1-10范围
		return folderData.type
	} else {
		// 层级文件夹：使用100+的范围避免与系统文件夹冲突
		switch (folderData.type) {
			case 1: return 101  // 大洲层级
			case 2: return 102  // 国家层级  
			case 3: return 103  // 客户层级
			case 4: return 104  // 联系人层级
			case 5: return 105  // 自定义文件夹
			default: return 199 // 未知层级
		}
	}
}
// 修改现有的 getFolderLevelName 方法，确保层级文件夹的名称正确
const getFolderLevelName = (type) => {
	const levelNames = {
		1: 'continent',    // 大洲
		2: 'country',      // 国家
		3: 'customer',     // 客户
		4: 'contact',      // 联系人
		5: 'custom'        // 自定义
	}
	return levelNames[type] || 'unknown'
}



// 监听文件夹数据变化，自动更新文件夹选项
watch(emailFolders, () => {
	if (showBatchMoveToFolderDialog.value) {
		buildFolderTreeOptions()
	}
}, { deep: true })

// 获取系统目标层级
const getSystemTargetLevel = (systemType) => {
	const mapping = {
		1: 10, // 收件箱
		2: 11, // 发件箱  
		3: 12, // 草稿箱
		4: 13, // 垃圾箱
		6: 14  // 归档箱
	}
	return mapping[systemType] || 10
}

// 修改 buildCompletefolderTreeOptions 方法，确保系统文件夹正确标记：
const buildCompletefolderTreeOptions = () => {
	console.log('构建完整文件夹树')

	// 1. 系统文件夹（明确标记）
	const systemFolders = buildSystemFolders()

	// 2. 层级文件夹（明确标记为非系统）
	const hierarchyFolders = convertEmailFoldersToHierarchy(emailFolders.value)

	// 3. 组合所有选项
	flattenedFolderOptions.value = [
		{
			value: 'system-folders',
			label: '系统文件夹',
			isSystem: true,
			folderCategory: 'system-group', // 新增：分组标识
			children: systemFolders
		},
		{
			value: 'hierarchy-folders',
			label: '分类文件夹',
			isSystem: false,
			folderCategory: 'hierarchy-group', // 新增：分组标识
			children: hierarchyFolders
		}
	]

	console.log('完整文件夹树构建完成:', flattenedFolderOptions.value)

	// 检查潜在冲突
	const systemIds = systemFolders.map(f => f.dataId)
	const hierarchyIds = hierarchyFolders.map(f => f.dataId)
	const conflicts = systemIds.filter(id => hierarchyIds.includes(id))

	if (conflicts.length > 0) {
		console.warn('⚠️ 检测到ID冲突:', conflicts)
		console.log('系统文件夹IDs:', systemIds)
		console.log('层级文件夹IDs:', hierarchyIds)
	}
}


// 获取文件夹类型名称
const getFolderTypeName = (folderData) => {
	if (folderData.isSystem && [1, 2, 3, 4, 6].includes(folderData.type)) {
		const systemNames = {
			1: '系统收件箱',
			2: '系统发件箱',
			3: '系统草稿箱',
			4: '系统垃圾箱',
			6: '系统归档箱'
		}
		return systemNames[folderData.type] || '系统文件夹'
	}

	const hierarchyNames = {
		1: '大洲层级',
		2: '国家层级',
		3: '客户层级',
		4: '联系人层级',
		5: '自定义文件夹'
	}
	return hierarchyNames[folderData.type] || '未知类型'
}
// #endregion

// 1. 添加当前文件夹状态的响应式变量
const currentFolderState = ref({
	type: null,        // 'system' | 'folder' | 'tag' | 'search'
	id: null,          // 文件夹/标签ID
	name: null,        // 文件夹/标签名称
	folderData: null,  // 完整的文件夹数据对象
	params: null       // 额外参数
})

// 2. 统一的状态设置方法
const setCurrentFolderState = (type, id, name = null, folderData = null, params = null) => {
	console.log('🎯 设置当前文件夹状态:', { type, id, name, folderData, params })

	currentFolderState.value = {
		type,
		id,
		name,
		folderData,
		params
	}

	// 同时更新原有的状态变量以保持兼容性
	if (type === 'folder') {
		activeMenu.value = `folder-${id}`
		folderName.value = id.toString()
		EmailTagIndex.value = 0
	} else if (type === 'tag') {
		activeMenu.value = `tag-${id}`
		EmailTagIndex.value = id.toString()
		folderName.value = null
	} else if (type === 'system') {
		activeMenu.value = id.toString()
		folderName.value = null
		EmailTagIndex.value = 0
	}
}

// 监听文件夹数据变化
watch(emailFolders, () => {
	if (showBatchMoveToFolderDialog.value) {
		buildCompletefolderTreeOptions()
	}
}, { deep: true })
</script>

<style lang="scss" scoped>
.quoted-wrapper {
	margin-top: 16px;
}

.quoted-toggle {
	cursor: pointer;
	color: #409eff;
	font-size: 12px;
	margin-bottom: 8px;
}

.quoted-container .quoted-mail {
	border-left: 3px solid #ddd;
	background: #f9f9f9;
	padding: 12px 16px;
	border-radius: 4px;
	color: #666;
	font-size: 13px;
}

.quoted-container .quoted-title {
	font-weight: 600;
	color: #333;
	margin-bottom: 10px;
}

.quoted-container .quoted-meta {
	margin-bottom: 6px;
}

.quoted-container .quoted-body img {
	max-width: 100%;
	height: auto;
}

/* 防止图片撑爆 */

/* 放到本组件样式里 */
.ql-editor blockquote {
	border-left: 3px solid #ddd;
	padding: 12px 16px;
	background: #f9f9f9;
	color: #666;
	margin: 12px 0;
}

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
	margin-left: 70px;
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
		min-width: 60px;
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
		background-color: #ecf5ff !important;

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
		margin-top: 8px;

		.custom-tag {
			font-size: 14px;
			padding: 12px 24px;
			margin: 10px;
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
}

.layout-container-demo .el-header {
	position: relative;
	background: none;
	color: var(--el-text-color-primary);
}

.layout-container-demo .el-aside {
	color: var(--el-text-color-primary);
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

.email-folder-tree {
	margin-top: 10px;
	padding: 0 10px;
}

.custom-tree-node {
	flex: 1;
	display: flex;
	align-items: center;
	font-size: 14px;
	padding-right: 8px;
}

.custom-tree-node .el-icon {
	margin-right: 8px;
	font-size: 16px;
	color: #909399;
}

.folder-actions {
	margin-left: auto;
	display: none;
}

.custom-tree-node:hover .folder-actions {
	display: flex;
	align-items: center;
}

.menu-section-title {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 0 20px;
	margin: 10px 0;
	color: #909399;
	font-size: 14px;
}

.add-button {
	padding: 2px;
}

.add-button .el-icon {
	font-size: 14px;
}

/* 批量操作样式 */
.batch-actions {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 12px 16px;
	background-color: #f5f7fa;
	border: 1px solid #e4e7ed;
	border-radius: 4px;
	margin-bottom: 16px;
	box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.batch-info {
	display: flex;
	align-items: center;
	gap: 8px;
	color: #606266;
	font-size: 14px;
}

.batch-info .el-icon {
	color: #409eff;
}

.batch-buttons {
	display: flex;
	gap: 12px;
	align-items: center;
}

.batch-buttons .el-button-group {
	margin-right: 8px;
}

.batch-buttons .el-button {
	display: flex;
	align-items: center;
	gap: 4px;
}

/* 批量处理时的禁用状态 */
.batch-actions.is-batch-processing {
	opacity: 0.6;
	pointer-events: none;
}

/* 表格选择列的样式调整 */
:deep(.el-table .el-table-column--selection .cell) {
	padding: 0;
	text-align: center;
}

:deep(.el-table .el-checkbox) {
	margin: 0;
}

/* 确保复选框在行点击时不会触发选择 */
:deep(.el-table .el-table-column--selection .cell) {
	pointer-events: auto;
}

:deep(.el-table .el-table-column--selection .cell .el-checkbox) {
	pointer-events: auto;
}

/* 移动至文件夹对话框样式 */
.move-to-folder-dialog {
	padding: 10px 0;
}

.selected-emails-info {
	margin-bottom: 20px;
}

.folder-tree-node {
	display: flex;
	align-items: center;
	width: 100%;
	padding: 4px 0;
}

.folder-tree-node .el-icon {
	color: #909399;
	font-size: 16px;
}

/* 文件夹树样式 */
:deep(.el-tree) {
	.el-tree-node__content {
		height: 40px;

		&:hover {
			background-color: #f5f7fa;
		}
	}

	.el-tree-node.is-current>.el-tree-node__content {
		background-color: #ecf5ff;
		color: #409eff;

		.el-icon {
			color: #409eff;
		}
	}

	.el-tree-node__expand-icon {
		color: #c0c4cc;

		&.is-leaf {
			color: transparent;
			cursor: default;
		}
	}
}

/* 批量操作按钮组样式调整 */
.batch-buttons .el-button-group {
	.el-button {
		&:nth-child(1) {
			/* 归档按钮 */
			border-top-right-radius: 0;
			border-bottom-right-radius: 0;
		}

		&:nth-child(2) {
			/* 移动至按钮 */
			border-radius: 0;
			border-left: none;
			border-right: none;
		}

		&:nth-child(3) {
			/* 删除按钮 */
			border-top-left-radius: 0;
			border-bottom-left-radius: 0;
		}
	}
}

/* 移动至按钮特殊样式 */
.el-button--warning {
	&:hover {
		background-color: #ebb563;
		border-color: #ebb563;
	}

	&:active {
		background-color: #cf9236;
		border-color: #cf9236;
	}
}

/* 对话框内容区域样式 */
.move-to-folder-dialog {
	.el-form-item__label {
		font-weight: 500;
		color: #606266;
	}

	.el-input__wrapper {
		background-color: #f5f7fa;

		&.is-focus {
			background-color: #fff;
		}
	}
}

/* 文件夹选择树容器样式 */
.folder-tree-container {
	border: 1px solid #dcdfe6;
	border-radius: 4px;
	padding: 10px;
	max-height: 300px;
	overflow-y: auto;
	background-color: #fafafa;

	&:hover {
		border-color: #c0c4cc;
	}
}

/* 系统文件夹和自定义文件夹分组样式 */
:deep(.el-tree-node) {

	&[data-value="system"],
	&[data-value="custom"] {
		.el-tree-node__content {
			font-weight: 600;
			color: #303133;
			background-color: #f0f2f5;

			&:hover {
				background-color: #e6e8eb;
			}
		}
	}
}

/* 提示信息样式 */
.selected-emails-info {
	.el-alert {
		.el-alert__content {
			font-size: 14px;
		}

		strong {
			color: #409eff;
			font-weight: 600;
		}
	}
}

/* 对话框底部按钮样式 */
.move-to-folder-dialog .dialog-footer {
	display: flex;
	justify-content: flex-end;
	gap: 12px;
	padding-top: 20px;
	border-top: 1px solid #ebeef5;
	margin-top: 20px;
}

/* 禁用状态的按钮样式 */
.el-button:disabled {
	opacity: 0.6;
	cursor: not-allowed;
}

/* 加载状态样式 */
.batch-actions.is-moving {
	opacity: 0.8;
	pointer-events: none;
}
</style>