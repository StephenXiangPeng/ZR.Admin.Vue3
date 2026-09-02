<template>
	<div>
		<!-- 客户信息表 -->
		<div style="border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden;">
			<!-- 功能区区域 -->
			<div style="background: #f8f9fa; padding: 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15">
					<el-col :span="12">
						<div style="text-align: left;">
							<el-button type="primary" @click=openCustomerProfileDialog() size="default">客户建档</el-button>
							<el-button type="primary" @click="CustomerDuplicationCheckDialog = true"
								size="default">客户查重</el-button>
							<el-button type="primary" @click="openCustomerPublicSeaDialog"
								size="default">客户公海</el-button>
							<el-button type="primary" @click="openPendingCustomerRatingDialog"
								:disabled="PendingCustomerRatingTableDatatotalItems === 0" size="default">
								待评级客户({{ PendingCustomerRatingTableDatatotalItems }})
							</el-button>
						</div>
					</el-col>
				</el-row>
			</div>

			<!-- 过滤条件区域 -->
			<div class="customer-search-area"
				style="background: #f8f9fa; padding: 8px 15px; border-bottom: 1px solid #e5e7eb;">
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-input v-model="Search_CustomerEmail_input" clearable placeholder="请输入邮箱地址" size="default" />
					</el-col>
					<el-col :span="4">
						<el-select v-model="Search_CustomerName_input" placeholder="请选择客户名称" style="width: 100%;"
							filterable size="default" clearable>
							<el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-input v-model="Search_CustomerContactPerson_input" clearable placeholder="请输入联系人"
							size="default" />
					</el-col>
					<el-col :span="4">
						<el-select v-model="Search_BusinessScope_Select" placeholder="请选择业务范围" style="width: 100%;"
							filterable size="default" clearable>
							<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select v-model="Search_TradingCountry_Select" placeholder="请选择国家" size="default"
							style="width: 100%;" filterable clearable>
							<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
								:value="dict.dictValue" size="default"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-select filterable v-model="Search_CustomerLevel_Select" placeholder="请选择客户等级" size="default"
							style="width: 100%;" clearable>
							<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" size="default"></el-option>
						</el-select>
					</el-col>
				</el-row>
				<el-row :gutter="15" class="search-row">
					<el-col :span="4">
						<el-select filterable v-model="Search_CustomerSource_Select" placeholder="请选择客户来源"
							size="default" style="width: 100%;" clearable>
							<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" size="default"></el-option>
						</el-select>
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="Search_StartDate_Select" type="date" placeholder="请选择起始日期"
							size="default" style="width: 100%;" clearable />
					</el-col>
					<el-col :span="4">
						<el-date-picker v-model="Search_EndDate_Select" type="date" placeholder="请选择结束日期" size="default"
							style="width: 100%;" clearable />
					</el-col>
					<el-col :span="4">
						<div style="text-align: left;">
							<el-button type="primary" plain @click="SearchSunmit" size="default">查询</el-button>
							<el-button @click="resetSearch" size="default">重置</el-button>
						</div>
					</el-col>
					<el-col :span="8">
						<!-- 空白区域 -->
					</el-col>
				</el-row>
			</div>

			<!-- 表格区域 -->
			<el-table class="customer-info-table" :data="CunstomeinfotableData"
				@row-dblclick="CunstomeinfotableDatahandleRowDblClick" style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '22px' }" :cell-style="{ padding: '1px 0' }">
				<el-table-column prop="customerNo" label="客户编号" width="120" sortable>
					<template #default="scope">
						<span>{{ scope.row.customerNo }}</span>
						<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
							size="small">草稿</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="customerStatus" label="客户状态" width="120" sortable />
				<el-table-column prop="customerLevel" label="客户等级" width="120" sortable />
				<el-table-column prop="customerAbbreviation" label="客户简称" width="150" sortable />
				<el-table-column prop="tradingCountry" label="贸易国别" width="120" sortable />
				<el-table-column prop="lastContactTime" label="最近联系时间" width="140" sortable>
					<template #default="scope">
						{{ formatDate(scope.row.lastContactTime) }}
					</template>
				</el-table-column>
				<el-table-column prop="nextFollowUpTime" label="下次跟进时间" width="140" sortable>
					<template #default="scope">
						<span :style="{ color: getFollowUpTimeColor(scope.row.nextFollowUpTime) }">
							{{ formatDate(scope.row.nextFollowUpTime) }}
						</span>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="建档时间" width="120" sortable>
					<template #default="scope">
						{{ formatDate(scope.row.createTime) }}
					</template>
				</el-table-column>
				<el-table-column prop="salesPerson" label="所属销售员" width="130" sortable />
				<el-table-column prop="lastbindSalePerson" label="原跟进人" width="130" v-if="false" />
				<el-table-column prop="create_by" label="创建人" width="130" v-if="false" />
				<el-table-column fixed="right" prop="operate" label="操作" width="280">
					<template v-slot:default="scope">
						<el-button link type="primary" size="small"
							@click=OpenCustomerProfileDetailDialog(scope.row)>查看详情</el-button>
						<el-button v-if="isCurrentUserSalesPerson(scope.row._salesPersonValue)" link type="danger"
							size="small" @click.stop="releaseCustomerToPool(scope.row.id)">释放到公海</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="handlePageChange" @size-change="handleSizeChange"
				:current-page="currentPage" :page-size="pageSize" :total="totalItems" :page-sizes="[10, 20, 30, 50]"
				background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</div>
		<el-dialog :modal="false" :modal-penetrable="true" v-model="CustomerProfileDialog" title="客户建档"
			:close-on-click-modal=false style="width: 75%;" @close="resetCustomerProfileDialog()">
			<el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="客户基本信息" name="basicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">客户基本信息</span>
					</template>
					<el-button type="primary" :icon="Plus" size="default" @click="OpenLeadImportDialog"
						style="margin-bottom: 15px;">从线索导入</el-button>
					<el-form ref="CustomerProfileformRef" :rules="rules" :model="CustomerProfileform"
						label-width="120px" :show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户状态">
									<el-select filterable v-model="CustomerProfileform.customerStatus"
										placeholder="选择客户状态" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_customer_status" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户等级">
									<el-select filterable v-model="CustomerProfileform.customerLevel" placeholder="自动评级"
										disabled style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户编号">
									<el-input v-model="CustomerProfileform.customerNo" disabled placeholder="自动编码"
										style="width: 100%;" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户简称" prop="customerAbbreviation">
									<el-input v-model="CustomerProfileform.customerAbbreviation" placeholder="输入客户简称"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户名称" prop="customerName">
									<el-input v-model="CustomerProfileform.customerName" placeholder="输入客户名称"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="贸易国别" prop="tradingCountry">
									<el-select filterable v-model="CustomerProfileform.tradingCountry"
										placeholder="选择贸易国别" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="公司主页">
									<el-input v-model="CustomerProfileform.compantWebsite" placeholder="输入公司主页地址"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户来源" prop="customerSource">
									<el-select filterable v-model="CustomerProfileform.customerSource"
										placeholder="选择客户来源" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="业务范围" prop="businessScope">
									<el-select filterable v-model="CustomerProfileform.businessScope"
										placeholder="选择业务范围" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="价格条款">
									<el-select filterable v-model="CustomerProfileform.pricingTerm" placeholder="选择价格条款"
										style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="结汇方式">
									<el-select filterable v-model="CustomerProfileform.settlementWay"
										placeholder="选择结汇方式" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="收汇期限">
									<el-input v-model="CustomerProfileform.collectionPeriod" placeholder="输入收汇期限"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户税号">
									<el-input v-model="CustomerProfileform.customerTaxNumber" placeholder="输入客户税号"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="销售人员" prop="salesPerson">
									<el-select filterable v-model="CustomerProfileform.salesPerson" placeholder="选择销售员"
										style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="详细地址" prop="address1">
									<el-input type="textarea" v-model="CustomerProfileform.address1"
										placeholder="输入详细地址" rows="3" style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="客户照片">
									<el-upload list-type="picture-card" :auto-upload="false"
										v-model:file-list="fileList" limit="3" :disabled="fileList.length >= 3"
										@change="handleChange" :action="UploadUrl" :data="formData">
										<el-icon>
											<Plus />
										</el-icon>

										<template #file="{ file }">
											<div>
												<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
												<span class="el-upload-list__item-actions">
													<span class="el-upload-list__item-preview"
														@click="handlePictureCardPreview(file)">
														<el-icon><zoom-in /></el-icon>
													</span>
													<span v-if="!disabled" class="el-upload-list__item-delete"
														@click="handleRemove(file)">
														<el-icon>
															<Delete />
														</el-icon>
													</span>
												</span>
											</div>
										</template>
									</el-upload>
									<el-dialog v-model="dialogVisible">
										<img style="max-width: 100%; max-height: 80vh; object-fit: contain; display: block; margin: 0 auto;"
											:src="dialogImageUrl" alt="Preview Image" />
									</el-dialog>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="contactInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="联系人信息" name="contactInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">联系人信息</span>
					</template>
					<el-button type="primary" :icon="Plus" size="default" @click="AddContactPersonTableRow"
						style="margin-bottom: 15px;">新增联系人</el-button>
					<el-table :data="CustomerContactPersonTableData" style="width: 100%; table-layout: fixed;"
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="name" label="联系人姓名" width="150" align="left">
							<template #default="{ row }">
								<el-input v-model="row.name" placeholder="输入联系人姓名" size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="sex" label="性别" width="120" align="left">
							<template #default="{ row }">
								<el-select filterable v-model="row.sex" placeholder="请选择性别" size="default">
									<el-option v-for="dict in optionss.sys_user_sex" :key="dict.dictCode"
										:label="dict.dictLabel" :value="dict.dictValue" clearable />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column prop="position" label="职务" width="150" align="left">
							<template #default="{ row }">
								<el-input v-model="row.position" placeholder="输入职务" size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="telePhone" label="电话" width="150" align="left">
							<template #default="{ row }">
								<el-input v-model="row.telePhone" placeholder="输入电话号码" size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="cellPhone" label="手机" width="150" align="left">
							<template #default="{ row }">
								<el-input v-model="row.cellPhone" placeholder="输入手机号码" size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="fax" label="传真" width="150" align="left">
							<template #default="{ row }">
								<el-input v-model="row.fax" placeholder="输入传真" size="default"></el-input>
							</template>
						</el-table-column>
						<el-table-column prop="email" label="电子邮件" width="200" align="left">
							<template #default="{ row }">
								<el-input v-model="row.email" type="email" placeholder="输入电子邮件" size="default"
									clearable></el-input>
							</template>
						</el-table-column>
						<el-table-column label="操作" width="100" align="center" fixed="right">
							<template #default="scope">
								<el-button type="text" size="default"
									@click="RemoveContactPersonTableRow(scope.$index)">删除</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-collapse-item>
			</el-collapse>

			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="SaveCustomerDraft(CustomerProfileformRef)"
						:disabled="!canSubmitCustomerProfile">
						保存草稿
					</el-button>
					<el-button type="success" @click="submitForm(CustomerProfileformRef)"
						:disabled="!canSubmitCustomerProfile">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="LeadImportDialog" title="线索导入" :close-on-click-modal=false style="width: 70%;">
			<el-table :data="CustomerLeadsTableData" @row-dblclick="handleRowDblClick" align="center"
				style="width: 100%; table-layout: fixed;" stripe
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column fixed prop="id" label="线索编号" width="120" />
				<el-table-column prop="contactEmail" label="邮箱" width="200" />
				<el-table-column prop="customerName" label="客户名称" width="150" />
				<el-table-column prop="customerLevel" label="客户等级" width="120" />
				<el-table-column prop="customerNation" label="国家" width="120" />
				<el-table-column prop="contactName" label="联系人" width="120" />
				<el-table-column prop="customerSource" label="来源" width="120" />
				<el-table-column prop="involvingBusiness" label="涉及业务" width="150" />
				<el-table-column prop="create_by" label="创建人" width="120" />
			</el-table>
			<el-pagination @current-change="CustomerLeadsTableDatahandlePageChange"
				@size-change="CustomerLeadsTableDatahandleSizeChange" :current-page="CustomerLeadsTableDatacurrentPage"
				:page-size="CustomerLeadsTableDatapageSize" :total="CustomerLeadsTableDatatotalItems"
				:page-sizes="[10, 20, 30, 50]" background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</el-dialog>
		<el-dialog :modal="false" modal-penetrable v-model="CustomerDuplicationCheckDialog" title="客户查重"
			:close-on-click-modal=false style="width: 75%;">
			<el-collapse v-model="duplicationCheckCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="查重字段" name="duplicationCheck">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">查重字段</span>
					</template>
					<el-form :model="CustomerDuplicationCheckform" label-width="120px" size="default"
						:show-message="false">
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户简称" prop="Cabbreviation">
									<el-input v-model="CustomerDuplicationCheckform.Cabbreviation" placeholder="输入客户简称"
										style="width: 100%;" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户名称" prop="Cname">
									<el-input v-model="CustomerDuplicationCheckform.Cname" placeholder="输入客户名称"
										style="width: 100%;" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户地址" prop="Caddress">
									<el-input v-model="CustomerDuplicationCheckform.Caddress" placeholder="输入客户地址"
										style="width: 100%;" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户邮箱" prop="Cemail">
									<el-input v-model="CustomerDuplicationCheckform.Cemail" placeholder="输入客户邮箱"
										style="width: 100%;" />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="电话号码" prop="Ctellphone">
									<el-input v-model="CustomerDuplicationCheckform.Ctellphone" placeholder="输入电话号码"
										style="width: 100%;" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<!-- <div style="text-align: left;">
									<el-button type="primary" plain @click="DuplicationCheckSearch"
										size="default">查询</el-button>
									<el-button @click="DuplicationCheckReset" size="default">重置</el-button>
								</div> -->
								<el-form-item>
									<el-button type="primary" plain @click="DuplicationCheckSearch"
										size="default">查询</el-button>
									<el-button @click="DuplicationCheckReset" size="default">重置</el-button>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<!-- 空白区域 -->
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="duplicationResultCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="查重结果" name="duplicationResult">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">查重结果</span>
					</template>
					<el-table :data="CustomerDuplicationCheckData" style="width: 100%; table-layout: fixed;" stripe
						:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
						:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
						<el-table-column prop="customerNo" label="客户编号" width="120" />
						<el-table-column prop="customerStatus" label="客户状态" width="100" />
						<el-table-column prop="customerLevel" label="客户等级" width="100" />
						<el-table-column prop="customerAbbreviation" label="客户简称" width="150" />
						<el-table-column prop="tradingCountry" label="贸易国别" width="120" />
						<el-table-column prop="Lastcontactdate" label="最近联系时间" width="140" />
						<el-table-column prop="createTime" label="建档时间" width="140">
							<template #default="scope">
								{{ formatDate(scope.row.createTime) }}
							</template>
						</el-table-column>
						<el-table-column prop="salesName" label="所属销售员" width="120" />
						<el-table-column prop="Originalfollower" label="原跟进人" width="120" />
						<el-table-column prop="create_by" label="创建人" width="100" />
					</el-table>
				</el-collapse-item>
			</el-collapse>
		</el-dialog>
		<el-dialog v-model="CustomerPublicSeaDialog" title="客户公海" :close-on-click-modal="false" style="width: 75%;">
			<el-table :data="CustomerPublicSeaTableData" style="width: 100%; table-layout: fixed;" stripe
				@row-dblclick="OpenCustomerProfileDetailDialog"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column prop="customerNo" label="客户编号" width="120" />
				<el-table-column prop="customerStatus" label="客户状态" width="120" />
				<el-table-column prop="customerLevel" label="客户等级" width="100" />
				<el-table-column prop="customerAbbreviation" label="客户简称" width="150" />
				<el-table-column prop="tradingCountry" label="贸易国别" width="120" />
				<el-table-column prop="businessScope" label="市场" width="120" />
				<el-table-column prop="Lastcontactdate" label="最近联系时间" width="140" />
				<el-table-column prop="createTime" label="建档时间" width="140">
					<template #default="scope">
						{{ formatDate(scope.row.createTime) }}
					</template>
				</el-table-column>
				<el-table-column prop="lastbindSalePerson" label="原属业务员" width="120" />
				<el-table-column fixed="right" label="操作" width="180">
					<template #default="scope">
						<el-button link type="primary" size="small"
							@click.stop="OpenCustomerProfileDetailDialog(scope.row)">查看详情</el-button>
						<el-button link type="success" size="small" @click.stop="claimPublicSeaCustomer(scope.row)">
							领取
						</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="CustomerPublicSeaTableDatahandlePageChange"
				@size-change="CustomerPublicSeaTableDatahandleSizeChange"
				:current-page="CustomerPublicSeaTableDatacurrentPage" :page-size="CustomerPublicSeaTableDatapageSize"
				:total="CustomerPublicSeaTableDatatotalItems" :page-sizes="[10, 20, 30, 50]" background
				layout="total, sizes, prev, pager, next, jumper" style="margin-top: 10px; text-align: right;" />
		</el-dialog>
		<el-dialog v-model="PendingCustomerRatingDialog" title="待评级客户" :close-on-click-modal="false"
			style="width: 88%;">
			<div style="margin-bottom: 10px;">
				<el-button type="primary" @click="batchRecalculateCustomerLevel"
					:disabled="PendingCustomerRatingSelectedCustomerIds.length === 0">
					批量处理
				</el-button>
			</div>
			<el-table :data="PendingCustomerRatingTableData" style="width: 100%; table-layout: fixed;" stripe
				@selection-change="handlePendingCustomerRatingSelectionChange"
				:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
				:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
				<el-table-column type="selection" width="50" />
				<el-table-column prop="id" label="ID" width="90" v-if="false" />
				<el-table-column prop="customerId" label="客户ID" width="100" v-if="false" />
				<el-table-column prop="customerName" label="客户名称" width="180" />
				<el-table-column prop="salesPerson" label="当前所属业务员" width="130" />
				<el-table-column prop="targetSalesPerson" label="目标业务员" width="120" />
				<el-table-column prop="oldLevel" label="原客户等级" width="120" />
				<el-table-column prop="targetLevel" label="目标客户等级" width="120" />
				<el-table-column prop="failType" label="失败类型" width="140" v-if="false" />
				<el-table-column prop="failReason" label="失败原因" width="260" show-overflow-tooltip />
				<el-table-column prop="statusText" label="处理状态" width="100" />
				<el-table-column prop="remark" label="备注" width="200" show-overflow-tooltip />
				<el-table-column prop="create_by" label="创建人" width="120" v-if="false" />
				<el-table-column prop="create_time" label="创建时间" width="170">
					<template #default="scope">
						{{ formatDateTime(scope.row.create_time) }}
					</template>
				</el-table-column>
				<el-table-column prop="update_by" label="更新人" width="120" v-if="false" />
				<el-table-column prop="update_time" label="更新时间" width="170" v-if="false">
					<template #default="scope">
						{{ formatDateTime(scope.row.update_time) }}
					</template>
				</el-table-column>
				<el-table-column prop="isDelete" label="是否删除" width="90" v-if="false" />
				<el-table-column fixed="right" label="操作" width="120">
					<template #default="scope">
						<el-button link type="primary" size="small"
							@click="recalculateSingleCustomerLevel(scope.row.customerId)">手动评级</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination @current-change="PendingCustomerRatingTableDatahandlePageChange"
				@size-change="PendingCustomerRatingTableDatahandleSizeChange"
				:current-page="PendingCustomerRatingTableDatacurrentPage"
				:page-size="PendingCustomerRatingTableDatapageSize" :total="PendingCustomerRatingTableDatatotalItems"
				:page-sizes="[10, 20, 30, 50]" background layout="total, sizes, prev, pager, next, jumper"
				style="margin-top: 10px; text-align: right;" />
		</el-dialog>
		<el-dialog :modal="false" :modal-penetrable="true" v-model="CustomerProfileDetailDialog" title="客户详情"
			:close-on-click-modal=false style="width: 75%;">
			<el-collapse v-model="detailBasicInfoCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="客户基本信息" name="detailBasicInfo">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">客户基本信息</span>
					</template>
					<el-form :model="CustomerProfileDetailDialogform" label-width="120px" size="default">
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户状态">
									<el-select filterable v-model="CustomerProfileDetailDialogform.customerStatus"
										placeholder="选择客户状态" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_customer_status" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户等级">
									<el-select filterable v-model="CustomerProfileDetailDialogform.customerLevel"
										placeholder="选择客户等级" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户编号">
									<el-input v-model="CustomerProfileDetailDialogform.customerNo" disabled
										placeholder="自动编码" style="width: 100%;" />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户简称">
									<el-input v-model="CustomerProfileDetailDialogform.customerAbbreviation"
										placeholder="输入客户简称" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户名称">
									<el-input v-model="CustomerProfileDetailDialogform.customerName"
										placeholder="输入客户名称" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="贸易国别">
									<el-select filterable v-model="CustomerProfileDetailDialogform.tradingCountry"
										placeholder="选择贸易国别" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="公司主页">
									<el-input v-model="CustomerProfileDetailDialogform.compantWebsite"
										placeholder="输入公司主页地址" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="客户来源">
									<el-select filterable v-model="CustomerProfileDetailDialogform.customerSource"
										disabled placeholder="选择客户来源" size="default" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="业务范围">
									<el-select filterable v-model="CustomerProfileDetailDialogform.businessScope"
										disabled placeholder="选择业务范围" style="width: 100%;" clearable>
										<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="价格条款">
									<el-select filterable v-model="CustomerProfileDetailDialogform.pricingTerm"
										placeholder="选择价格条款" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="结汇方式">
									<el-select filterable v-model="CustomerProfileDetailDialogform.settlementWay"
										placeholder="选择结汇方式" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="收汇期限">
									<el-input v-model="CustomerProfileDetailDialogform.collectionPeriod"
										placeholder="输入收汇期限" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="6">
								<el-form-item label="客户税号">
									<el-input v-model="CustomerProfileDetailDialogform.customerTaxNumber"
										placeholder="输入客户税号" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="6">
								<el-form-item label="销售人员">
									<el-select filterable v-model="CustomerProfileDetailDialogform.salesPerson"
										placeholder="选择销售员" style="width: 100%;" disabled clearable>
										<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode"
											:label="dict.dictLabel" :value="dict.dictValue"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12">
								<el-form-item label="详细地址">
									<el-input type="textarea" v-model="CustomerProfileDetailDialogform.address1"
										placeholder="输入详细地址" rows="3" style="width: 100%;" disabled />
								</el-form-item>
							</el-col>
						</el-row>
						<el-row>
							<el-col :span="24">
								<el-form-item label="客户照片">
									<el-upload list-type="picture-card" :auto-upload="false"
										v-model:file-list="fileList" limit="3" :disabled="fileList.length >= 3"
										@change="handleChange" :action="UploadUrl" :data="formData">
										<el-icon>
											<Plus />
										</el-icon>
										<template #file="{ file }">
											<div>
												<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
												<span class="el-upload-list__item-actions">
													<span class="el-upload-list__item-preview"
														@click="handlePictureCardPreview(file)">
														<el-icon><zoom-in /></el-icon>
													</span>
													<span v-if="!disabled" class="el-upload-list__item-delete"
														@click="handleRemove(file)">
														<el-icon>
															<Delete />
														</el-icon>
													</span>
												</span>
											</div>
										</template>
									</el-upload>
									<el-dialog v-model="dialogVisible" append-to-body>
										<img style="max-width: 100%; max-height: 80vh; object-fit: contain; display: block; margin: 0 auto;"
											:src="dialogImageUrl" alt="Preview Image" />
									</el-dialog>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-collapse-item>
			</el-collapse>

			<el-collapse v-model="detailTabsCollapseActive" style="margin-bottom: 20px;">
				<el-collapse-item title="详细信息" name="detailTabs">
					<template #title>
						<span style="font-size: 20px; font-weight: bold;">详细信息</span>
					</template>
					<el-tabs v-model="activeTab" tab-position="top" style="height: 350px;" class="demo-tabs">
						<el-tab-pane label="联系人信息" name="ContactInfoTable">
							<el-table :data="ContactPersonData" style="width: 100%; table-layout: fixed;"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="name" label="姓名" width="120" />
								<el-table-column prop="sexText" label="性别" width="100" />
								<el-table-column prop="position" label="职务" width="150" />
								<el-table-column prop="telePhone" label="电话" width="150" />
								<el-table-column prop="cellPhone" label="手机" width="150" />
								<el-table-column prop="fax" label="传真" width="150" />
								<el-table-column prop="email" label="邮件" width="200" />
							</el-table>
						</el-tab-pane>
						<el-tab-pane label="联系日志" name="ContactLogTable">
							<el-button type="primary" @click="AddContactLog"
								style="margin-bottom: 15px;">添加联系日志</el-button>
							<el-table :data="ContactLogData" height="200" style="width: 100%; table-layout: fixed;"
								@row-dblclick="handleContactLogRowDblClick"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="emailDate" label="联系日期" width="130" />
								<el-table-column prop="conactTag" label="日志标签" width="130" />
								<el-table-column prop="logSouce" label="来源" width="150" />
								<el-table-column prop="contact" label="联系人" width="180" />
								<el-table-column prop="ourPersonnel" label="我方人员" width="180" />
								<el-table-column prop="relatedDocumentType" label="关联单据类型" width="180">
									<template #default="{ row }">
										{{ row.relatedDocumentType === 1 ? '销售合同' : row.relatedDocumentType === 2 ?
											'出运合同' : '无'
										}}
									</template>
								</el-table-column>
								<el-table-column prop="contactDetails" label="联系内容" show-overflow-tooltip
									min-width="120">
									<template #default="{ row }">
										<el-link v-if="row.logSouce === '邮件记录'" type="primary"
											:underline="false" @click.stop="handleContactContentClick(row)">
											查看邮件
										</el-link>
										<span v-else>{{ row.contactDetails }}</span>
									</template>
								</el-table-column>
								<el-table-column label="图片" width="100">
									<template #default="{ row }">
										<el-image v-if="row.images && row.images.length > 0"
											style="width: 50px; height: 50px;" :src="row.images.split(',')[0]"
											:preview-src-list="row.images.split(',')" fit="cover"
											:preview-teleported="true" :initial-index="0" />
										<span v-else>无图片</span>
									</template>
								</el-table-column>
								<el-table-column label="附件" width="100">
									<template #default="{ row }">
										<el-button v-if="row.attachments && row.attachments.length > 0" type="primary"
											link @click="downloadAttachment(row.attachments)">
											下载附件
										</el-button>
										<span v-else>无附件</span>
									</template>
								</el-table-column>
							</el-table>
							<el-pagination @current-change="ContactLogTablehandlePageChange"
								@size-change="ContactLogTablehandleSizeChange"
								:current-page="ContactLogTablecurrentPage" :page-size="ContactLogTablepageSize"
								:total="ContactLogTabletotalItems" :page-sizes="[10, 20, 30, 50]" background
								layout="total, sizes, prev, pager, next, jumper"
								style="margin-top: 10px; text-align: right;" />
						</el-tab-pane>
						<el-tab-pane label="报价记录" name="QuoteRecordTable">
							<el-table :data="QuotationRecordData" @row-dblclick="handleQuoteRowDblClick"
								style="width: 100%; table-layout: fixed;"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="id" label="ID" width="100" v-if="false"></el-table-column>
								<el-table-column prop="quotationNum" label="报价单号" width="200">
									<template #default="scope">
										<span>{{ scope.row.quotationNum }}</span>
										<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;"
											size="small">草稿</el-tag>
										<el-tag v-if="scope.row.version > 1" type="success" style="margin-left: 5px;"
											size="small">
											{{ getVersionText(scope.row.version) }}
										</el-tag>
									</template>
								</el-table-column>
								<el-table-column prop="realQuotationDate" label="报价日期" width="150" />
								<el-table-column prop="validityPeriod" label="有效期" width="150" />
							</el-table>
							<el-pagination @current-change="QuotationRecordHandlePageChange"
								@size-change="QuotationRecordHandleSizeChange"
								:current-page="QuotationRecordCurrentPage" :page-size="QuotationRecordPageSize"
								:total="QuotationRecordTotalItems" :page-sizes="[10, 20, 30, 50]" background
								layout="total, sizes, prev, pager, next, jumper"
								style="margin-top: 10px; text-align: right;" />
						</el-tab-pane>
						<el-tab-pane label="销售记录" name="saleRecordTable">
							<el-table :data="SalesContractRecordData" @row-dblclick="handleSalesContractRowDblClick"
								style="width: 100%; table-layout: fixed;"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="contractNumber" label="销售合同" width="150" />
								<el-table-column prop="contractDate" label="合同日期" width="150" />
								<el-table-column prop="effectiveDate" label="生效日期" width="150" />
								<el-table-column prop="deliveryDate" label="交货日期" width="150" />
								<el-table-column prop="goodsValue" label="货值合计" width="150" />
							</el-table>
							<el-pagination @current-change="SalesContractRecordHandlePageChange"
								@size-change="SalesContractRecordHandleSizeChange"
								:current-page="SalesContractRecordCurrentPage" :page-size="SalesContractRecordPageSize"
								:total="SalesContractRecordTotalItems" :page-sizes="[10, 20, 30, 50]" background
								layout="total, sizes, prev, pager, next, jumper"
								style="margin-top: 10px; text-align: right;" />
						</el-tab-pane>
						<el-tab-pane label="收寄样历史" name="SampleCollectionHistory">
							<el-table :data="CustomerSendSampleData" style="width: 100%; table-layout: fixed;"
								@row-dblclick="handleSampleRowDblClick"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="id" label="ID" width="100" v-if="false"></el-table-column>
								<el-table-column prop="type" label="寄样/收样" width="100"></el-table-column>
								<el-table-column prop="waybillNumber" label="运单号" width="150"></el-table-column>
								<el-table-column prop="expressCompany" label="快递公司" width="120"></el-table-column>
								<el-table-column prop="sampleDate" label="样品日期" width="120"></el-table-column>
								<el-table-column prop="abbreviation" label="供应商简称" width="150"></el-table-column>
								<el-table-column prop="companyName" label="我方公司" width="150"></el-table-column>
								<el-table-column prop="paymentMethod" label="付费方式" width="120"></el-table-column>
								<el-table-column prop="paidExpressFee" label="已付快递费" width="120">
									<template #default="scope">
										<span>￥{{ scope.row.paidExpressFee }}</span>
									</template>
								</el-table-column>
								<el-table-column prop="remark" label="备注"></el-table-column>
							</el-table>
							<el-pagination @current-change="CustomerSendSampleHandlePageChange"
								@size-change="CustomerSendSampleHandleSizeChange"
								:current-page="CustomerSendSampleCurrentPage" :page-size="CustomerSendSamplePageSize"
								:total="CustomerSendSampleTotalItems" :page-sizes="[10, 20, 30, 50]" background
								layout="total, sizes, prev, pager, next, jumper"
								style="margin-top: 10px; text-align: right;" />
						</el-tab-pane>
						<el-tab-pane label="财务" name="FinanceLedgerTable">
							<el-table v-loading="financeLedgerLoading" :data="financeLedgerPaginatedData"
								style="width: 100%; table-layout: fixed;" stripe
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="date" label="日期" width="130" align="center" />
								<el-table-column prop="summary" label="摘要/事项" width="150" align="center" />

								<el-table-column label="收支" align="center">
									<el-table-column prop="income.rmb" label="人民币" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.income.rmb > 0">{{
												formatFinanceAmount(scope.row.income.rmb)
											}}</span>
										</template>
									</el-table-column>
									<el-table-column prop="income.usd" label="美元" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.income.usd > 0">{{
												formatFinanceAmount(scope.row.income.usd)
											}}</span>
										</template>
									</el-table-column>
									<el-table-column prop="income.eur" label="欧元" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.income.eur > 0">{{
												formatFinanceAmount(scope.row.income.eur)
											}}</span>
										</template>
									</el-table-column>
								</el-table-column>

								<el-table-column label="支出" align="center">
									<el-table-column prop="expenditure.rmb" label="人民币" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.expenditure.rmb > 0">{{
												formatFinanceAmount(scope.row.expenditure.rmb)
											}}</span>
										</template>
									</el-table-column>
									<el-table-column prop="expenditure.usd" label="美元" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.expenditure.usd > 0">{{
												formatFinanceAmount(scope.row.expenditure.usd)
											}}</span>
										</template>
									</el-table-column>
									<el-table-column prop="expenditure.eur" label="欧元" width="120" align="right">
										<template #default="scope">
											<span v-if="scope.row.expenditure.eur > 0">{{
												formatFinanceAmount(scope.row.expenditure.eur)
											}}</span>
										</template>
									</el-table-column>
								</el-table-column>

								<el-table-column label="备注" align="center">
									<el-table-column label="收款客户/付款单位" width="180" align="center">
										<template #default="scope">
											<span v-if="scope.row.remarks && scope.row.remarks.customerOrSupplier">
												{{ scope.row.remarks.customerOrSupplier }}
											</span>
											<span v-else>-</span>
										</template>
									</el-table-column>
									<el-table-column label="合同号/备注" width="200" align="center">
										<template #default="scope">
											<span v-if="scope.row.remarks && scope.row.remarks.contractOrRemark">
												{{ scope.row.remarks.contractOrRemark }}
											</span>
											<span v-else>-</span>
										</template>
									</el-table-column>
								</el-table-column>
							</el-table>
							<el-pagination @current-change="handleFinanceLedgerPageChange"
								@size-change="handleFinanceLedgerSizeChange" :current-page="financeLedgerCurrentPage"
								:page-size="financeLedgerPageSize" :total="financeLedgerTotalRecords"
								:page-sizes="[10, 20, 30, 50]" background
								layout="total, sizes, prev, pager, next, jumper"
								style="margin-top: 10px; text-align: right;" />
						</el-tab-pane>
						<el-tab-pane label="出货记录" name="ShippingRecordTable">
							<el-table :data="ShipRecoreData" style="width: 100%; table-layout: fixed;"
								:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
								:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
								<el-table-column prop="Cnum" label="发票号码" width="120" />
								<el-table-column prop="Cstate" label="出运日期" width="120" />
								<el-table-column prop="Cgrade" label="销售合同" width="120" />
								<el-table-column prop="Cabbreviation" label="产品编号" width="120" />
								<el-table-column prop="Ctradingcountry" label="客户货号" width="120" />
								<el-table-column prop="Lastcontactdate" label="中文品名" width="150" />
								<el-table-column prop="Archivecreationdate" label="出货数量" width="120" />
								<el-table-column prop="CSeller" label="计量单位" width="120" />
								<el-table-column prop="Originalfollower" label="外销单价" width="120" />
								<el-table-column prop="Founder" label="外销总价" width="120" />
							</el-table>
						</el-tab-pane>
					</el-tabs>
				</el-collapse-item>
			</el-collapse>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" @click="EditCustomerInfoClick"
						:disabled="!canEditSelectedCustomer">编辑</el-button>
					<el-button
						v-if="CustomerProfileDetailDialogform.id && isCurrentUserSalesPerson(CustomerProfileDetailDialogform.salesPerson)"
						type="danger" plain
						@click="releaseCustomerToPool(CustomerProfileDetailDialogform.id)">释放到公海</el-button>
					<!-- <el-button type="primary">保存</el-button> -->
				</span>
			</template>
		</el-dialog>
		<!-- 添加联系日志对话框 -->
		<el-dialog v-model="contactLogDialogVisible" title="添加联系日志" width="70%" :close-on-click-modal="false">
			<el-form ref="contactLogFormRef" :model="contactLogForm" :rules="contactLogRules" label-width="120px">
				<el-row>
					<el-col :span="12">
						<el-form-item label="联系日期" prop="contactDate">
							<el-date-picker v-model="contactLogForm.contactDate" type="date" placeholder="选择联系日期"
								style="width: 100%;" />
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="日志标签" prop="ContactLogTag">
							<el-select v-model="contactLogForm.ContactLogTag" filterable clearable placeholder="选择日志标签"
								style="width: 100%;" @change="handleContactLogTagChange">
								<el-option v-for="item in ContactLogTagData" :key="item.id" :label="item.emailTagName"
									:value="item.id" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<!-- 商机名称字段 - 根据日志标签显示 -->
				<el-form-item v-if="showBusinessOpportunityField" label="商机名称"
					:required="contactLogForm.ContactLogTag && ContactLogTagData.find(tag => tag.id === contactLogForm.ContactLogTag)?.emailTagName === '询盘'"
					prop="businessOpportunityName">
					<!-- 询盘时显示文本框 -->
					<el-input
						v-if="contactLogForm.ContactLogTag && ContactLogTagData.find(tag => tag.id === contactLogForm.ContactLogTag)?.emailTagName === '询盘'"
						v-model="contactLogForm.businessOpportunityName" placeholder="请输入商机名称" style="width: 100%;" />
					<!-- 其他情况显示下拉框 -->
					<el-select v-else v-model="contactLogForm.businessOpportunityName" filterable placeholder="请选择商机名称"
						style="width: 100%;" clearable>
						<el-option v-for="item in businessOpportunityOptions" :key="item.dictValue"
							:label="item.dictLabel" :value="item.dictValue" />
					</el-select>
				</el-form-item>
				<el-row>
					<el-col :span="12">
						<el-form-item label="联系人" prop="contactPerson">
							<el-select v-model="contactLogForm.contactPerson" filterable allow-create
								placeholder="选择联系人" style="width: 100%;" clearable>
								<el-option v-for="item in ContactPersonData" :key="item.id" :label="item.name"
									:value="item.id" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item label="我方人员" prop="ourStaff">
							<el-select v-model="contactLogForm.ourStaff" filterable placeholder="选择我方人员"
								style="width: 100%;" clearable>
								<el-option v-for="item in state.optionss.sql_hr_sale" :key="item.dictValue"
									:label="item.dictLabel" :value="item.dictValue" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-form-item label="关联单据" v-if="relatedDocumentTypeOptions.length > 0">
					<el-radio-group v-model="contactLogForm.relatedDocumentType" @change="RelatedDocumentsChange"
						:disabled="isRelatedDocumentLocked">
						<el-radio v-for="item in relatedDocumentTypeOptions" :key="item.value" :value="item.value">
							{{ item.label }}
						</el-radio>
					</el-radio-group>
					<el-select v-if="getRelatedDocumentOptions().length > 0" v-model="contactLogForm.relatedDocumentID"
						filterable :placeholder="getRelatedDocumentPlaceholder()" style="width: 100%;" clearable>
						<el-option v-for="item in getRelatedDocumentOptions()" :key="item.dictValue"
							:label="item.dictLabel" :value="item.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="联系内容" prop="contactContent">
					<el-input v-model="contactLogForm.contactContent" type="textarea" :rows="4" placeholder="请输入联系内容" />
				</el-form-item>
				<el-form-item label="图片">
					<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="contactLogImages"
						:limit="3" :action="UploadUrl" accept="image/*" @change="handleContactLogImageChange"
						@preview="handleImagePreview">
						<el-icon>
							<Plus />
						</el-icon>
					</el-upload>
					<el-dialog v-model="dialogVisible" append-to-body>
						<img style="max-width: 100%; max-height: 80vh; object-fit: contain; display: block; margin: 0 auto;"
							:src="dialogImageUrl" alt="Preview Image" />
					</el-dialog>
				</el-form-item>
				<el-form-item label="附件">
					<el-upload :auto-upload="false" v-model:file-list="contactLogAttachments" :action="UploadUrl"
						@change="handleContactLogAttachmentChange" style="width: 100%">
						<el-button type="primary">选择文件</el-button>
						<template #tip>
							<div class="el-upload__tip">可上传任意类型文件，附件总大小不超过 50M</div>
						</template>
					</el-upload>
				</el-form-item>
				<el-form-item label="备注">
					<el-input v-model="contactLogForm.remark" type="textarea" :rows="2" placeholder="请输入备注信息" />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="contactLogDialogVisible = false">取 消</el-button>
					<el-button type="primary" @click="submitContactLog(contactLogFormRef)">确 定</el-button>
				</span>
			</template>
		</el-dialog>
		<!-- 添加联系日志详情对话框 -->
		<el-dialog v-model="contactLogDetailDialogVisible" title="联系日志详情" width="70%" :close-on-click-modal="false">
			<el-descriptions :column="1" border>
				<el-descriptions-item label="联系日期">{{ selectedContactLog.emailDate }}</el-descriptions-item>
				<el-descriptions-item label="日志标签">{{ selectedContactLog.conactTag }}</el-descriptions-item>
				<el-descriptions-item label="来源">{{ selectedContactLog.logSouce }}</el-descriptions-item>
				<el-descriptions-item label="联系人">{{ selectedContactLog.contact }}</el-descriptions-item>
				<el-descriptions-item label="我方人员">{{ selectedContactLog.ourPersonnel }}</el-descriptions-item>
				<el-descriptions-item label="关联单据类型">
					{{
						selectedContactLog.relatedDocumentType === 1
							? '销售合同'
							: selectedContactLog.relatedDocumentType === 2
								? '出运合同'
								: '无'
					}}
				</el-descriptions-item>
				<el-descriptions-item label="关联单据">
					{{
						selectedContactLog.relatedDocumentID && selectedContactLog.relatedDocumentID !== 0
							? (
								selectedContactLog.relatedDocumentType === 1
									? (state.optionss.sql_sale_contracts.find(
										item => item.dictValue.toString() === selectedContactLog.relatedDocumentID.toString()
									)?.dictLabel || selectedContactLog.relatedDocumentID)
									: selectedContactLog.relatedDocumentType === 2
										? (state.optionss.sql_shippingdeliveries.find(
											item => item.dictValue.toString() === selectedContactLog.relatedDocumentID.toString()
										)?.dictLabel || selectedContactLog.relatedDocumentID)
										: '无'
							)
							: '无'
					}}
				</el-descriptions-item>
				<el-descriptions-item label="联系内容">
					<el-link v-if="selectedContactLog.logSouce === '邮件记录'" type="primary"
						:underline="false" @click="handleContactContentClick(selectedContactLog)">查看邮件</el-link>
					<div v-else style="white-space: pre-wrap;">{{ selectedContactLog.contactDetails }}</div>
				</el-descriptions-item>
				<el-descriptions-item label="图片" v-if="selectedContactLog.images">
					<el-image style="width: 100px; height: 100px; margin-right: 10px;"
						v-for="(url, index) in selectedContactLog.images.split(',')" :key="index" :src="url"
						:preview-src-list="selectedContactLog.images.split(',')" fit="cover" />
				</el-descriptions-item>
				<el-descriptions-item label="附件" v-if="selectedContactLog.attachments">
					<div v-for="(url, index) in selectedContactLog.attachments.split(',')" :key="index">
						<el-button type="primary" link @click="downloadAttachment(url)">
							{{ url.split('/').pop() }}
						</el-button>
					</div>
				</el-descriptions-item>
			</el-descriptions>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { getCurrentInstance, reactive, toRefs, ref, onMounted, nextTick, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessageBox, UploadUserFile, ElMessage, UploadFile, FormInstance, FormRules } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import request from '@/utils/request';
import { getFinancialGeneralLedgerData } from '@/api/finance'
import useUserStore from "@/store/modules/user";
import { number } from 'echarts';
import { fa } from 'element-plus/es/locale';
import { start } from 'nprogress';

// 定义接口类型
interface ContactLog {
	emailDate: string;
	conactTag: string;
	logSouce: string;
	contact: string;
	ourPersonnel: string;
	contactDetails: string;
	relatedDocumentType: number;
	relatedDocumentID: number;
	/** 邮件记录时的邮件ID，部分接口可能用此字段 */
	emailID?: number;
	images?: string;
	attachments?: string;
}

interface ApiResponse<T> {
	code: number;
	msg: string;
	data: T;
}

// 扩展 UploadFile 类型
interface CustomUploadFile extends UploadFile {
	isChanged?: boolean;
}

interface FinanceAmount {
	rmb: number
	usd: number
	eur: number
}

interface FinanceRemarks {
	customerOrSupplier: string
	contractOrRemark: string
}

interface FinanceLedgerItem {
	date: string
	summary: string
	income: FinanceAmount
	expenditure: FinanceAmount
	remarks: FinanceRemarks
}

interface FinanceApiRow {
	date: string
	summary: string
	incomeCny: number
	incomeUsd: number
	incomeEur: number
	expenseCny: number
	expenseUsd: number
	expenseEur: number
	customerOrPayee: string
	contractOrRemark: string
}

const activeTab = ref('ContactInfoTable');
const userStore = useUserStore();
const isEditCustomerInfo = ref(false);
const basicInfoCollapseActive = ref(['basicInfo']);
const contactInfoCollapseActive = ref(['contactInfo']);
const detailBasicInfoCollapseActive = ref(['detailBasicInfo']);
const detailTabsCollapseActive = ref(['detailTabs']);
const duplicationCheckCollapseActive = ref(['duplicationCheck']);
const duplicationResultCollapseActive = ref(['duplicationResult']);
const proxy = getCurrentInstance().proxy as any
const financeLedgerLoading = ref(false)
const financeLedgerData = ref<FinanceLedgerItem[]>([])
const financeLedgerCurrentPage = ref(1)
const financeLedgerPageSize = ref(30)
const financeLedgerTotalRecords = computed(() => financeLedgerData.value.length)
const financeLedgerPaginatedData = computed(() => {
	const start = (financeLedgerCurrentPage.value - 1) * financeLedgerPageSize.value
	const end = start + financeLedgerPageSize.value
	return financeLedgerData.value.slice(start, end)
})
const state = reactive({
	optionss: {
		// 显示状态选项列表(动态字典将会从后台获取数据)
		sys_customer_source: [],
		hr_customer_level: [],
		hr_nation: [],
		hr_business_scope: [],
		hr_customer_status: [],
		hr_settlement_way: [],
		hr_pricing_term: [],
		sql_hr_sale: [],
		sys_user_sex: [],
		hr_express_delivery_company: [],
		hr_express_payment_method: [],
		hr_contract_status: [],
		hr_ourcompany: [],
		sql_user_customers: [],
		sql_sale_contracts: [],
		sql_shippingdeliveries: [],
		customer_contract_data: [],
		hr_quotationalidity: []
	}
})
const { optionss } = toRefs(state)
//从后台读取多个字典数据(客户来源、客户等级、贸易国别、业务范围、客户状态、结汇方式、价格条款、销售人员)
var dictParams = [
	{ dictType: 'sys_user_sex' },
	{ dictType: 'sql_hr_sale' },
	{ dictType: 'hr_pricing_term' },
	{ dictType: 'hr_customer_status' },
	{ dictType: 'hr_settlement_way' },
	{ dictType: 'sys_customer_source' },
	{ dictType: 'hr_customer_level' },
	{ dictType: 'hr_nation' },
	{ dictType: 'hr_business_scope' },
	{ dictType: 'sql_all_user' },
	{ dictType: 'hr_express_delivery_company' },
	{ dictType: 'hr_express_payment_method' },
	{ dictType: 'hr_contract_status' },
	{ dictType: 'hr_ourcompany' },
	{ dictType: 'sql_sale_contracts' },
	{ dictType: 'sql_shippingdeliveries' },
	{ dictType: 'customer_contract_data' },
	{ dictType: 'hr_quotationalidity' }
]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	//获取客户基本信息列表
	GetCustomeInfoList(currentPage.value, pageSize.value);
	refreshPendingCustomerRatingCount();
})

// 获取用户相关的客户数据
const getUserCustomerData = async () => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerAllNameDataByUserId/GetSelectCustomerAllNameData',
			method: 'get'
		}) as ApiResponse<any>

		if (response.code === 200) {
			state.optionss.sql_user_customers = response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel
			}))
		} else {
			ElMessage.error((response as any).msg || '获取客户数据失败')
		}
	} catch (error) {
		ElMessage.error('获取客户数据失败')
	}
}
getUserCustomerData();//获取用户相关的客户数据

//获取特定的客户合同数据
const GetVisibleContractSelectList = async (customerId = 0) => {
	state.optionss.customer_contract_data = [];
	try {
		const response = await request({
			url: 'Contracts/GetVisibleContractSelectList/GetVisibleContractSelectList',
			method: 'get',
			params: {
				CustomerID: customerId
			}
		})
		if (response.code === 200) {
			state.optionss.customer_contract_data = response.data
		} else {
			ElMessage.error((response as any).msg || '获取销售合同数据失败')
		}
	} catch (error) {
		console.error('获取销售合同数据失败:', error)
		ElMessage.error('获取销售合同数据失败')
	}
}
GetVisibleContractSelectList();

//线索导入窗体
const LeadImportDialog = ref(false)
const OpenLeadImportDialog = () => {
	LeadImportDialog.value = true
	GetCustomeleadList(CustomerLeadsTableDatacurrentPage.value, CustomerLeadsTableDatapageSize.value);
}
//线索表格分页组件
const CustomerLeadsTableDatatotalItems = ref(0);
const CustomerLeadsTableDatacurrentPage = ref(1);
const CustomerLeadsTableDatapageSize = ref(30);
//线索信息表格
const CustomerLeadsTableData = ref([])
const CustomerLeadsTableDatahandlePageChange = async (newPage) => {
	CustomerLeadsTableDatacurrentPage.value = newPage;
	await GetCustomeleadList(newPage, CustomerLeadsTableDatapageSize.value);
};
const CustomerLeadsTableDatahandleSizeChange = async (size) => {
	CustomerLeadsTableDatapageSize.value = size;
	CustomerLeadsTableDatacurrentPage.value = 1;
	await GetCustomeleadList(1, size);
};
//获取线索信息列表
GetCustomeleadList(CustomerLeadsTableDatacurrentPage.value, CustomerLeadsTableDatapageSize.value);
//获取线索列表
function GetCustomeleadList(start, end) {
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'CustomerLeads/GetCustomerLeadsListByUserID/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				clueNo: 0,
				clueEmail: null,
				clueCustomerName: null,
				contactName: null,
				business: 0,
				nation: 0,
				customerLevel: 0,
				customerSource: 0,
				startDate: null,
				endDate: null
			}
		}).then(response => {
			if (response.data.length > 0) {
				CustomerLeadsTableData.value = response.data;
				resolve(response.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetCustomeleadList(start - 1, end);
				} else {
					CustomerLeadsTableData.value = [];
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}

const GetNextCustomerNo = () => {
	request({
		url: 'CustomerInfoMation/GetNextCustomerNo/GetNextCustomerNo',
		method: 'GET'
	}).then(response => {
		if (response.code == "200") {
			CustomerProfileform.customerNo = response.data;
		} else {
			ElMessage.error("获取客户编号失败，请重新打开客户建档窗体");
		}
	}).catch(error => {
		console.error(error);
	});
}

const isImport = ref(false);
const handleRowDblClick = (row) => {
	isImport.value = true;
	clearUploadfile();
	CustomerProfileform.customerName = row.customerName;
	CustomerProfileform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictLabel == row.customerLevel).map(item => item.dictValue).values().next().value;
	CustomerProfileform.tradingCountry = state.optionss['hr_nation'].filter(item => item.dictLabel == row.customerNation).map(item => item.dictValue).values().next().value;
	CustomerProfileform.customerSource = state.optionss['sys_customer_source'].filter(item => item.dictLabel == row.customerSource).map(item => item.dictValue).values().next().value;
	CustomerProfileform.businessScope = state.optionss['hr_business_scope'].filter(item => item.dictLabel == row.involvingBusiness).map(item => item.dictValue).values().next().value;
	CustomerProfileform.customerStatus = state.optionss['hr_customer_status'][0].dictValue;
	CustomerProfileform.compantWebsite = row.compantWebsite;
	CustomerProfileform.customerPhoto = row.compantPhotoStr;
	const newRow = {
		name: row.contactName,
		sex: '2',
		position: row.contactPosition,
		telephone: row.contactPhone,
		cellphone: row.contactPhone,
		fax: row.contactPhone,
		email: row.contactEmail
	};
	CustomerContactPersonTableData.value.push(newRow);
	if (row.compantPhotoStr != null && row.compantPhotoStr != '') {
		row.compantPhotoStr.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					uid: Date.now() + Math.random(),
					status: 'success'
				});
			}
		});
	}
	uploadedFiles.value = fileList.value;
	LeadImportDialog.value = false;
}

//客户建档窗体
const CustomerProfileDialog = ref(false)

const openCustomerProfileDialog = async () => {
	if (!(await assertPotentialCanAddForNewCustomerArchive())) {
		return;
	}
	GetNextCustomerNo();
	clearUploadfile();
	// 设置默认销售人员为当前登录用户
	var userId = useUserStore().userId;
	CustomerProfileform.salesPerson = state.optionss.sql_hr_sale.filter(item => item.dictValue == userId).map(item => item.dictValue).values().next().value;
	CustomerProfileDialog.value = true;
}

//清空上传组件
const clearUploadfile = () => {
	fileList.value = []
	uploadedFiles.value = []
}
//客户详情窗体
const CustomerProfileDetailDialog = ref(false)
//客户查重窗体
const CustomerDuplicationCheckDialog = ref(false)
//客户公海窗体
const CustomerPublicSeaDialog = ref(false)
const CustomerPublicSeaTableData = ref([])
const CustomerPublicSeaTableDatatotalItems = ref(0)
const CustomerPublicSeaTableDatacurrentPage = ref(1)
const CustomerPublicSeaTableDatapageSize = ref(30)
//待处理客户窗体
const PendingCustomerRatingDialog = ref(false)
const PendingCustomerRatingTableData = ref([])
const PendingCustomerRatingTableDatatotalItems = ref(0)
const PendingCustomerRatingTableDatacurrentPage = ref(1)
const PendingCustomerRatingTableDatapageSize = ref(30)
const PendingCustomerRatingSelectedCustomerIds = ref<number[]>([])
//查询条件
const Search_CustomerBusiness_input = ref('')
const Search_CustomerContactPerson_input = ref('')
const Search_CustomerEmail_input = ref('')
const Search_CustomerName_input = ref(null)
const Search_CustomerNo_input = ref('')
const Search_BusinessScope_Select = ref('')
const Search_TradingCountry_Select = ref('')
const Search_CustomerLevel_Select = ref('')
const Search_CustomerSource_Select = ref('')
const Search_StartDate_Select = ref('')
const Search_EndDate_Select = ref('')

const loadCustomerPublicSeaData = async (pageNum = CustomerPublicSeaTableDatacurrentPage.value, pageSizeNum = CustomerPublicSeaTableDatapageSize.value) => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetOpenSeaCustomerInfoList/GetList',
			method: 'GET',
			params: {
				PageNum: pageNum,
				PageSize: pageSizeNum
			}
		})
		const result = response?.data?.result || []
		CustomerPublicSeaTableDatacurrentPage.value = response?.data?.pageIndex || pageNum
		CustomerPublicSeaTableDatapageSize.value = response?.data?.pageSize || pageSizeNum
		CustomerPublicSeaTableDatatotalItems.value = response?.data?.totalNum || 0
		CustomerPublicSeaTableData.value = result.map(item => ({
			...item,
			_customerLevelValue: item.customerLevel,
			customerStatus:
				item.customerStatus === 0 || item.customerStatus === '0'
					? '暂未更新状态'
					: state.optionss['hr_customer_status'].filter(option => option.dictValue == item.customerStatus).map(option => option.dictLabel).values().next().value || item.customerStatus,
			customerLevel: state.optionss['hr_customer_level'].filter(option => option.dictValue == item.customerLevel).map(option => option.dictLabel).values().next().value || item.customerLevel,
			tradingCountry: state.optionss['hr_nation'].filter(option => option.dictValue == item.tradingCountry).map(option => option.dictLabel).values().next().value || item.tradingCountry,
			businessScope: state.optionss['hr_business_scope'].filter(option => option.dictValue == item.businessScope).map(option => option.dictLabel).values().next().value || item.businessScope,
			salesPerson: state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.salesPerson).map(option => option.dictLabel).values().next().value || item.salesPerson,
			create_by: state.optionss['sql_all_user'].filter(option => option.dictValue == item.create_by).map(option => option.dictLabel).values().next().value || item.create_by,
			lastbindSalePerson: state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.lastbindSalePerson).map(option => option.dictLabel).values().next().value || item.lastbindSalePerson
		}))
	} catch (error) {
		console.error('获取公海客户列表失败:', error)
		ElMessage.error('获取公海客户列表失败')
		CustomerPublicSeaTableData.value = []
		CustomerPublicSeaTableDatatotalItems.value = 0
	}
}

const openCustomerPublicSeaDialog = async () => {
	CustomerPublicSeaTableDatacurrentPage.value = 1
	await loadCustomerPublicSeaData(1, CustomerPublicSeaTableDatapageSize.value)
	CustomerPublicSeaDialog.value = true
}

const loadPendingCustomerRatingData = async (
	pageNum = PendingCustomerRatingTableDatacurrentPage.value,
	pageSizeNum = PendingCustomerRatingTableDatapageSize.value
) => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerRatingFailedRecords/GetList',
			method: 'GET',
			params: {
				PageNum: pageNum,
				PageSize: pageSizeNum
			}
		})
		const data = response?.data || {}
		const result = data?.result || []
		PendingCustomerRatingTableDatacurrentPage.value = data?.pageIndex || pageNum
		PendingCustomerRatingTableDatapageSize.value = data?.pageSize || pageSizeNum
		PendingCustomerRatingTableDatatotalItems.value = data?.totalNum || 0
		PendingCustomerRatingTableData.value = result.map(item => ({
			...item,
			salesPerson: state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.salesPerson).map(option => option.dictLabel).values().next().value || item.salesPerson,
			targetSalesPerson: state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.targetSalesPerson).map(option => option.dictLabel).values().next().value || item.targetSalesPerson,
			oldLevel: state.optionss['hr_customer_level'].filter(option => option.dictValue == item.oldLevel).map(option => option.dictLabel).values().next().value || item.oldLevel,
			targetLevel: state.optionss['hr_customer_level'].filter(option => option.dictValue == item.targetLevel).map(option => option.dictLabel).values().next().value || item.targetLevel,
			statusText: item.status === 0 ? '未处理' : item.status === 1 ? '已处理' : item.status === 2 ? '已忽略' : item.status
		}))
	} catch (error) {
		console.error('获取待处理客户列表失败:', error)
		ElMessage.error('获取待处理客户列表失败')
		PendingCustomerRatingTableData.value = []
		PendingCustomerRatingTableDatatotalItems.value = 0
	}
}

const refreshPendingCustomerRatingCount = async () => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerRatingFailedRecords/GetList',
			method: 'GET',
			params: {
				PageNum: 1,
				PageSize: 1
			}
		})
		PendingCustomerRatingTableDatatotalItems.value = response?.data?.totalNum || 0
	} catch (error) {
		console.error('获取待处理客户数量失败:', error)
		PendingCustomerRatingTableDatatotalItems.value = 0
	}
}

const openPendingCustomerRatingDialog = async () => {
	if (PendingCustomerRatingTableDatatotalItems.value === 0) {
		return
	}
	PendingCustomerRatingSelectedCustomerIds.value = []
	PendingCustomerRatingTableDatacurrentPage.value = 1
	await loadPendingCustomerRatingData(1, PendingCustomerRatingTableDatapageSize.value)
	PendingCustomerRatingDialog.value = true
}

const handlePendingCustomerRatingSelectionChange = (rows: any[]) => {
	PendingCustomerRatingSelectedCustomerIds.value = (rows || [])
		.map(item => Number(item?.customerId))
		.filter(id => !Number.isNaN(id) && id > 0)
}

const recalculateSingleCustomerLevel = async (customerId: number | string) => {
	const id = Number(customerId)
	if (!id) {
		ElMessage.warning('客户ID无效，无法处理')
		return
	}
	try {
		const response: any = await request({
			url: 'CustomerInfoMation/RecalculateCustomerLevel/RecalculateCustomerLevel',
			method: 'POST',
			params: { customerId: id }
		})
		if (response?.code == 200 || response?.code === '200') {
			ElMessage.success((typeof response?.data === 'string' && response.data) || response?.msg || '处理成功')
			await loadPendingCustomerRatingData(PendingCustomerRatingTableDatacurrentPage.value, PendingCustomerRatingTableDatapageSize.value)
			await refreshPendingCustomerRatingCount()
			await GetCustomeInfoList(currentPage.value, pageSize.value)
		}
	} catch {
		// 失败提示由请求拦截器处理
	}
}

const batchRecalculateCustomerLevel = async () => {
	const ids = Array.from(new Set(PendingCustomerRatingSelectedCustomerIds.value))
	if (ids.length === 0) {
		ElMessage.warning('请先选择需要处理的客户')
		return
	}
	try {
		const response: any = await request({
			url: 'CustomerInfoMation/BatchRecalculateCustomerLevel/BatchRecalculateCustomerLevel',
			method: 'POST',
			data: ids
		})
		if (response?.code == 200 || response?.code === '200') {
			const summary = response?.data
			const msg = summary && typeof summary === 'object'
				? `批量处理完成：总数${summary.TotalCount ?? ids.length}，成功${summary.SuccessCount ?? 0}，失败${summary.FailCount ?? 0}`
				: (response?.msg || '批量处理成功')
			ElMessage.success(msg)
			PendingCustomerRatingSelectedCustomerIds.value = []
			await loadPendingCustomerRatingData(PendingCustomerRatingTableDatacurrentPage.value, PendingCustomerRatingTableDatapageSize.value)
			await refreshPendingCustomerRatingCount()
			await GetCustomeInfoList(currentPage.value, pageSize.value)
		}
	} catch {
		// 失败提示由请求拦截器处理
	}
}

const PendingCustomerRatingTableDatahandlePageChange = async (newPage) => {
	PendingCustomerRatingTableDatacurrentPage.value = newPage
	await loadPendingCustomerRatingData(newPage, PendingCustomerRatingTableDatapageSize.value)
}

const PendingCustomerRatingTableDatahandleSizeChange = async (size) => {
	PendingCustomerRatingTableDatapageSize.value = size
	PendingCustomerRatingTableDatacurrentPage.value = 1
	await loadPendingCustomerRatingData(1, size)
}

const CustomerPublicSeaTableDatahandlePageChange = async (newPage) => {
	CustomerPublicSeaTableDatacurrentPage.value = newPage
	await loadCustomerPublicSeaData(newPage, CustomerPublicSeaTableDatapageSize.value)
}

const CustomerPublicSeaTableDatahandleSizeChange = async (size) => {
	CustomerPublicSeaTableDatapageSize.value = size
	CustomerPublicSeaTableDatacurrentPage.value = 1
	await loadCustomerPublicSeaData(1, size)
}

const claimPublicSeaCustomer = async (row) => {
	if (!row || !row.id) return
	try {
		const lim = await fetchCustomerLimitStatus()
		const bucket = getPublicSeaClaimLimitBucket(row._customerLevelValue)
		const potential = lim?.Potential ?? lim?.potential
		const activeLost = lim?.ActiveAndLost ?? lim?.activeAndLost
		const seg = bucket === 'potential' ? potential : activeLost
		const canAdd = seg?.CanAdd ?? seg?.canAdd
		if (!canAdd) {
			const c = seg?.Count ?? seg?.count
			const l = seg?.Limit ?? seg?.limit
			const name = bucket === 'potential' ? '潜在客户' : '成交客户与流失风险客户'
			ElMessage.warning(`${name}数量已达上限（${c}/${l}），无法从公海领取`)
			return
		}
	} catch (e: any) {
		ElMessage.error(e?.message || '获取客户数量上限失败')
		return
	}
	try {
		const response: any = await request({
			url: 'CustomerInfoMation/ClaimCustomerFromPool/ClaimCustomerFromPool',
			method: 'POST',
			params: { customerID: row.id }
		})
		if (response?.code == 200 || response?.code === '200') {
			const okMsg =
				typeof response?.data === 'string' && response.data
					? response.data
					: response?.msg
			ElMessage.success(okMsg || '领取成功')
			await loadCustomerPublicSeaData(
				CustomerPublicSeaTableDatacurrentPage.value,
				CustomerPublicSeaTableDatapageSize.value
			)
			await GetCustomeInfoList(currentPage.value, pageSize.value)
		}
	} catch {
		// 业务失败等已由 request 响应拦截器提示
	}
}

//查询条件重置功能
const resetSearch = () => {
	Search_CustomerBusiness_input.value = ''
	Search_CustomerContactPerson_input.value = ''
	Search_CustomerEmail_input.value = ''
	Search_CustomerName_input.value = null
	Search_CustomerNo_input.value = ''
	Search_BusinessScope_Select.value = ''
	Search_TradingCountry_Select.value = ''
	Search_CustomerLevel_Select.value = ''
	Search_CustomerSource_Select.value = ''
	Search_StartDate_Select.value = ''
	Search_EndDate_Select.value = ''
	GetCustomeInfoList(currentPage.value, pageSize.value);
}
//提交查询
const SearchSunmit = () => {
	GetCustomeInfoList(currentPage.value, pageSize.value);
}

const CustomerContactPersonTableData = ref([])
const AddContactPersonTableRow = () => {
	const newRow = {
		name: '',
		sex: state.optionss['sys_user_sex'].filter(item => item.dictValue == 2).map(item => item.dictValue).values().next().value,
		position: '',
		telephone: '',
		cellphone: '',
		fax: '',
		email: ''
	};
	CustomerContactPersonTableData.value.push(newRow);
}
const RemoveContactPersonTableRow = (index) => {
	CustomerContactPersonTableData.value.splice(index, 1);
}

const contactPersonEmailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;
const normalizeContactEmail = (email) => (email || '').trim().toLowerCase();
const getContactPersonCountByEmail = async (emailAddress) => {
	const response = await request({
		url: 'CustomerInfoMation/GetContactPersonCountByEmailAddress/GetContactPersonCount',
		method: 'GET',
		params: { EmailAddress: emailAddress }
	});
	if (response?.code === 200) {
		return response.data;
	}
	throw new Error(response?.msg || '验证联系人邮箱失败');
};

/** 当前用户客户数量及上限（与后端 GetCustomerLimitStatus 一致，JSON 可能为 camelCase） */
const fetchCustomerLimitStatus = async () => {
	const response: any = await request({
		url: 'CustomerInfoMation/GetCustomerLimitStatus/GetCustomerLimitStatus',
		method: 'GET'
	});
	if (response?.code == 200) {
		return response.data;
	}
	throw new Error(response?.msg || '获取客户数量上限失败');
};

/** 公海领取：按字典标签判断走潜在客户上限还是成交+流失风险上限 */
const getPublicSeaClaimLimitBucket = (customerLevelValue: string | number | null | undefined): 'potential' | 'activeAndLost' => {
	const levels = state.optionss?.['hr_customer_level'] || [];
	const v = customerLevelValue != null && customerLevelValue !== '' ? String(customerLevelValue) : '';
	const opt = levels.find(o => String(o.dictValue) === v);
	const label = (opt?.dictLabel || '').toString();
	if (label.includes('潜在')) {
		return 'potential';
	}
	if (label.includes('成交') || label.includes('流失')) {
		return 'activeAndLost';
	}
	return 'potential';
};

/** 新增客户档案：潜在客户上限预校验（点击建档与正式保存共用） */
const assertPotentialCanAddForNewCustomerArchive = async (): Promise<boolean> => {
	try {
		const lim = await fetchCustomerLimitStatus();
		const potential = lim?.Potential ?? lim?.potential;
		const canAdd = potential?.CanAdd ?? potential?.canAdd;
		if (!canAdd) {
			const c = potential?.Count ?? potential?.count;
			const l = potential?.Limit ?? potential?.limit;
			ElMessage.warning(`潜在客户数量已达上限（${c}/${l}），无法新增客户档案`);
			return false;
		}
		return true;
	} catch (e: any) {
		ElMessage.error(e?.message || '获取客户数量上限失败');
		return false;
	}
};

const isCustomerContactPersonTableDataCount = ref(0);
//客户联系人表格
isCustomerContactPersonTableDataCount.value = CustomerContactPersonTableData.value.length;



const contactPeople = [];

//新建客户表单
interface CustomerProfileform {
	update_time: string,
	id: number,
	customerNo: string,
	customerStatus: number,
	customerLevel: number,
	customerAbbreviation: string,
	customerName: string,
	tradingCountry: number,
	compantWebsite: string,
	customerSource: number,
	businessScope: number,
	pricingTerm: number,
	settlementWay: number,
	collectionPeriod: number,
	customerTaxNumber: string,
	salesPerson: number,
	address1: string,
	customerPhoto: string,
	remark: string,
	create_by: string,
	create_time: string,
	update_by: string,
	isDelete: number,
	IsDraft: number
}
const CustomerProfileformRef = ref<FormInstance>()
//新建客户表单
const CustomerProfileform = reactive<CustomerProfileform>({
	update_time: "",
	id: 0,
	customerNo: "",
	customerStatus: null,
	customerLevel: null,
	customerAbbreviation: "",
	customerName: "",
	tradingCountry: null,
	compantWebsite: "",
	customerSource: null,
	businessScope: null,
	pricingTerm: null,
	settlementWay: null,
	collectionPeriod: null,
	customerTaxNumber: "",
	salesPerson: null,
	address1: "",
	customerPhoto: "",
	remark: "",
	create_by: "",
	create_time: "",
	update_by: "",
	isDelete: 0,
	IsDraft: 0
});

const addCustomerInfo = {
	customerInfo: CustomerProfileform,
	//联系人信息
	contactPeople: contactPeople
}

// 客户信息表单验证规则
const rules = reactive<FormRules<CustomerProfileform>>({
	customerAbbreviation: [
		{
			required: true, message: '请输入客户简称', trigger: ['blur', 'change']
		}
	],
	customerName: [
		{
			required: true, message: '请输入客户名称', trigger: ['blur', 'change']
		}
	],
	tradingCountry: [
		{
			required: true, message: '请选择贸易国别', trigger: ['blur', 'change']
		}
	],
	businessScope: [
		{
			required: true, message: '请选择业务范围', trigger: ['blur', 'change']
		}
	],
	customerSource: [
		{
			required: true, message: '请选择客户来源', trigger: ['blur', 'change']
		}
	],
	salesPerson: [
		{
			required: true, message: '请选择销售人员', trigger: ['blur', 'change']
		}
	],
	address1: [
		{
			required: true, message: '请填写详细地址', trigger: ['blur', 'change']
		}
	]
});



const resetCustomerProfileDialog = () => {
	// Reset the form values
	CustomerProfileform.update_time = ""
	CustomerProfileform.id = 0
	CustomerProfileform.customerNo = ""
	CustomerProfileform.customerStatus = null
	CustomerProfileform.customerLevel = null
	CustomerProfileform.customerAbbreviation = ""
	CustomerProfileform.customerName = ""
	CustomerProfileform.tradingCountry = null
	CustomerProfileform.compantWebsite = ""
	CustomerProfileform.customerSource = null
	CustomerProfileform.businessScope = null
	CustomerProfileform.pricingTerm = null
	CustomerProfileform.settlementWay = null
	CustomerProfileform.collectionPeriod = null
	CustomerProfileform.customerTaxNumber = ""
	CustomerProfileform.salesPerson = null
	CustomerProfileform.address1 = ""
	CustomerProfileform.customerPhoto = ""
	CustomerProfileform.remark = ""
	CustomerProfileform.create_by = ""
	CustomerProfileform.create_time = ""
	CustomerProfileform.update_by = ""
	CustomerProfileform.isDelete = 0
	// Reset the contact person form values
	CustomerContactPersonTableData.value = []
	// resetFields
	CustomerProfileformRef.value.resetFields();

}

const filelistUrlStr = ref('')
const UploadUrl = 'Common/UploadFile'
const dialogImageUrl = ref('')
const dialogVisible = ref(false)
const disabled = ref(false)
const fileList = ref<CustomUploadFile[]>([])
const uploadedFiles = ref([]);  // 用于存储已上传的文件
const formData = ref({});  // 添加缺失的formData变量
// 删除客户图片
const handleRemove = (file: CustomUploadFile) => {
	ElMessageBox.confirm('确定删除该图片吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(() => {
		// 从 fileList 中移除
		const index = fileList.value.findIndex(f => f.uid === file.uid);
		if (index !== -1) {
			fileList.value.splice(index, 1);
		}

		// 从 uploadedFiles 中移除
		const uploadedIndex = uploadedFiles.value.findIndex(f => f.uid === file.uid);
		if (uploadedIndex !== -1) {
			uploadedFiles.value.splice(uploadedIndex, 1);
		}

		// 从 CustomerProfileform.customerPhoto 中移除对应的 url
		if (CustomerProfileform.customerPhoto !== '' && CustomerProfileform.customerPhoto !== null) {
			const urls = CustomerProfileform.customerPhoto.split(',');
			const urlIndex = urls.indexOf(file.url);
			if (urlIndex !== -1) {
				urls.splice(urlIndex, 1);
				CustomerProfileform.customerPhoto = urls.join(',');
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
};

// 照片每张最大 1M
const IMAGE_MAX_SIZE = 1 * 1024 * 1024;
// 检查上传客户图片数量
const handleChange = (file: CustomUploadFile, fileList: CustomUploadFile[]) => {
	if (file.raw && file.raw.size > IMAGE_MAX_SIZE) {
		ElMessage.error('每张图片不能超过 1M');
		fileList.splice(fileList.findIndex(f => f.uid === file.uid), 1);
		return;
	}
	if (fileList.length > 3) {
		ElMessage({ type: 'info', message: '最多上传3张图片！' });
		fileList.splice(3);
		return;
	}

	if (file.raw) {
		const isDuplicate = fileList.some(f =>
			f !== file && f.name === file.name && !f.raw
		);
		if (isDuplicate) {
			ElMessage({
				type: 'info',
				message: '不要上传重复的文件！'
			});
			const index = fileList.findIndex(f => f.uid === file.uid);
			if (index !== -1) {
				fileList.splice(index, 1);
			}
		} else {
			file.url = URL.createObjectURL(file.raw);
		}
	}
}

const handlePictureCardPreview = (file: CustomUploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}

const EditCustomerInfoClick = () => {
	if (!canEditSelectedCustomer.value) {
		ElMessage({
			message: '当前登录用户不是该客户的销售人员，无法编辑。',
			type: 'warning'
		});
		return;
	}
	isEditCustomerInfo.value = true;
	CustomerProfileform.customerStatus = state.optionss['hr_customer_status'].filter(item => item.dictValue == CustomerProfileDetailDialogform.customerStatus).map(item => item.dictValue).values().next().value;
	CustomerProfileform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == CustomerProfileDetailDialogform.customerLevel).map(item => item.dictValue).values().next().value;
	CustomerProfileform.customerNo = CustomerProfileDetailDialogform.customerNo;
	CustomerProfileform.customerAbbreviation = CustomerProfileDetailDialogform.customerAbbreviation;
	CustomerProfileform.customerName = CustomerProfileDetailDialogform.customerName;
	CustomerProfileform.tradingCountry = state.optionss['hr_nation'].filter(item => item.dictValue == CustomerProfileDetailDialogform.tradingCountry).map(item => item.dictValue).values().next().value;
	CustomerProfileform.compantWebsite = CustomerProfileDetailDialogform.compantWebsite;
	CustomerProfileform.customerSource = state.optionss['sys_customer_source'].filter(item => item.dictValue == CustomerProfileDetailDialogform.customerSource).map(item => item.dictValue).values().next().value;
	CustomerProfileform.businessScope = state.optionss['hr_business_scope'].filter(item => item.dictValue == CustomerProfileDetailDialogform.businessScope).map(item => item.dictValue).values().next().value;
	CustomerProfileform.pricingTerm = state.optionss['hr_pricing_term'].filter(item => item.dictValue == CustomerProfileDetailDialogform.pricingTerm).map(item => item.dictValue).values().next().value;
	CustomerProfileform.settlementWay = state.optionss['hr_settlement_way'].filter(item => item.dictValue == CustomerProfileDetailDialogform.settlementWay).map(item => item.dictValue).values().next().value;
	CustomerProfileform.collectionPeriod = CustomerProfileDetailDialogform.collectionPeriod;
	CustomerProfileform.customerTaxNumber = CustomerProfileDetailDialogform.customerTaxNumber;
	CustomerProfileform.salesPerson = state.optionss['sql_hr_sale'].filter(item => item.dictValue == CustomerProfileDetailDialogform.salesPerson).map(item => item.dictValue).values().next().value;
	CustomerProfileform.address1 = CustomerProfileDetailDialogform.address1;
	//加载客户图片
	if (CustomerProfileDetailDialogform.customerPhoto != null && CustomerProfileDetailDialogform.customerPhoto != '') {
		CustomerProfileDetailDialogform.customerPhoto.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false,
					uid: Date.now() + Math.random(),
					status: 'success'
				});
			}
		});
	}
	//加载联系人信息
	CustomerContactPersonTableData.value = [];
	CustomerContactPersonTableData.value = ContactPersonData.value;
	CustomerContactPersonTableData.value.forEach(person => {
		if (person.sex != null || person.sex != '' || person.sex != undefined) {
			person.sex = person.sex.toString();
		}
	});
	CustomerProfileDetailDialog.value = false;
	CustomerProfileDialog.value = true;
}

//保存草稿
const SaveCustomerDraft = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	if (isEditCustomerInfo.value && !canSubmitCustomerProfile.value) {
		ElMessage({
			message: '当前登录用户不是该客户的销售人员，无法保存草稿。',
			type: 'warning'
		});
		return;
	}
	ElMessageBox.confirm('确定保存该客户资料的草稿吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		try {
			// 设置必要的默认值
			if (CustomerProfileform.customerStatus === null || CustomerProfileform.customerStatus === undefined) {
				CustomerProfileform.customerStatus = 0;
			}
			if (CustomerProfileform.customerLevel === null || CustomerProfileform.customerLevel === undefined) {
				CustomerProfileform.customerLevel = 0;
			}
			if (CustomerProfileform.pricingTerm === null || CustomerProfileform.pricingTerm === undefined) {
				CustomerProfileform.pricingTerm = 0;
			}
			if (CustomerProfileform.settlementWay === null || CustomerProfileform.settlementWay === undefined) {
				CustomerProfileform.settlementWay = 0;
			}
			if (CustomerProfileform.collectionPeriod === null || CustomerProfileform.collectionPeriod === undefined) {
				CustomerProfileform.collectionPeriod = 0;
			}
			if (CustomerProfileform.salesPerson === null || CustomerProfileform.salesPerson === undefined) {
				CustomerProfileform.salesPerson = 0;
			}
			if (CustomerProfileform.tradingCountry === null || CustomerProfileform.tradingCountry === undefined) {
				CustomerProfileform.tradingCountry = 0;
			}
			if (CustomerProfileform.customerSource === null || CustomerProfileform.customerSource === undefined) {
				CustomerProfileform.customerSource = 0;
			}
			if (CustomerProfileform.businessScope === null || CustomerProfileform.businessScope === undefined) {
				CustomerProfileform.businessScope = 0;
			}
			if (CustomerProfileform.customerAbbreviation === null || CustomerProfileform.customerAbbreviation === undefined) {
				CustomerProfileform.customerAbbreviation = '';
			}
			if (CustomerProfileform.customerName === null || CustomerProfileform.customerName === undefined) {
				CustomerProfileform.customerName = '';
			}
			if (CustomerProfileform.customerTaxNumber === null || CustomerProfileform.customerTaxNumber === undefined) {
				CustomerProfileform.customerTaxNumber = '';
			}
			if (CustomerProfileform.compantWebsite === null || CustomerProfileform.compantWebsite === undefined) {
				CustomerProfileform.compantWebsite = '';
			}
			if (CustomerProfileform.address1 === null || CustomerProfileform.address1 === undefined) {
				CustomerProfileform.address1 = '';
			}
			if (CustomerProfileform.remark === null || CustomerProfileform.remark === undefined) {
				CustomerProfileform.remark = '';
			}

			// 处理文件上传
			let filelistUrlStr = '';
			if (fileList.value && fileList.value.length > 0) {
				// 收集已存在的URL（排除blob URL）
				const existingUrls = fileList.value
					.filter(file => file.url && !file.url.startsWith('blob:'))
					.map(file => file.url);

				// 只上传新文件
				const newFiles = fileList.value.filter(file => file.raw && (!file.url || file.url.startsWith('blob:')));
				if (newFiles.length > 0) {
					const uploadPromises = newFiles.map(file => {
						const formData = new FormData();
						formData.append('FileName', file.name);
						formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
						formData.append('FileNameType', '1');
						formData.append('File', file.raw);
						formData.append('storeType', '1');
						return request.postForm(UploadUrl, formData);
					});

					const responses = await Promise.all(uploadPromises);
					responses.forEach(response => {
						if (response?.data?.url) {
							existingUrls.push(response.data.url);
						}
					});
				}

				// 合并所有URL
				filelistUrlStr = existingUrls.join(',');
			}

			// 准备保存数据
			const saveData = {
				customerInfo: {
					...CustomerProfileform,
					customerPhoto: filelistUrlStr || CustomerProfileform.customerPhoto,
					IsDraft: 1
				},
				contactPeople: CustomerContactPersonTableData.value || []
			};

			// 保存草稿
			const response = await request.post(
				isEditCustomerInfo.value
					? 'CustomerInfoMation/EditCustomerInfo/Edit'
					: 'CustomerInfoMation/AddCustomerInfo/Add',
				isEditCustomerInfo.value
					? { ...saveData, customerInfo: { ...saveData.customerInfo, id: CustomerProfileDetailDialogform.id } }
					: saveData
			);

			if (response?.data) {
				ElMessage({
					message: '保存草稿成功！',
					type: 'success'
				});
				CloseCustomerProfileDetailDialog();
			} else {
				throw new Error(response?.msg || '保存草稿失败');
			}
		} catch (error) {
			console.error('保存草稿出错！😔错误内容：', error);
			ElMessage({
				message: error?.message || '保存草稿失败',
				type: 'error'
			});
		}
	}).catch(() => {
		ElMessage({
			message: '已取消保存草稿',
			type: 'info'
		});
	});
}

//保存客户资料
const submitForm = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	if (isEditCustomerInfo.value && !canSubmitCustomerProfile.value) {
		ElMessage({
			message: '当前登录用户不是该客户的销售人员，无法提交。',
			type: 'warning'
		});
		return;
	}
	try {
		await formEl.validate();
	} catch (error) {
		return;
	}
	if (CustomerContactPersonTableData.value != null) {
		var personnamestr = '';
		var isEmail = true;
		CustomerContactPersonTableData.value.forEach(person => {
			if (person.email == null || person.email == '' || person.email == undefined) {
				personnamestr += '【' + person.name + '】'
				isEmail = false;
			}
		});
		if (isEmail == false) {
			ElMessage({
				message: '请在联系人列表里，为' + personnamestr + '补充完整的邮箱',
				type: 'warning',
			})
			return;
		}
	}

	const contactPeople = CustomerContactPersonTableData.value || [];
	const invalidEmailNames = [];
	const emailCheckMap = new Map();
	const existingEmailMap = new Map();
	if (ContactPersonData.value && ContactPersonData.value.length > 0) {
		ContactPersonData.value.forEach(item => {
			if (item?.id != null) {
				existingEmailMap.set(item.id, normalizeContactEmail(item.email));
			}
		});
	}

	contactPeople.forEach(person => {
		const normalizedEmail = normalizeContactEmail(person?.email);
		if (!normalizedEmail) {
			return;
		}
		const displayName = person?.name || normalizedEmail;
		if (!contactPersonEmailRegex.test(normalizedEmail)) {
			invalidEmailNames.push(displayName);
			return;
		}
		const originalEmail = person?.id != null ? existingEmailMap.get(person.id) : '';
		if (!person?.id || originalEmail !== normalizedEmail) {
			if (!emailCheckMap.has(normalizedEmail)) {
				emailCheckMap.set(normalizedEmail, []);
			}
			emailCheckMap.get(normalizedEmail).push(displayName);
		}
	});

	if (invalidEmailNames.length > 0) {
		ElMessage({
			message: `联系人邮箱格式不正确：${invalidEmailNames.join('、')}`,
			type: 'warning'
		});
		return;
	}

	if (emailCheckMap.size > 0) {
		const existedEmailNames = [];
		try {
			await Promise.all(
				Array.from(emailCheckMap.entries()).map(async ([email, names]) => {
					const count = await getContactPersonCountByEmail(email);
					if (Number(count) > 0) {
						existedEmailNames.push(...names);
					}
				})
			);
		} catch (error) {
			console.error('验证联系人邮箱失败：', error);
			ElMessage({
				message: error?.message || '验证联系人邮箱失败',
				type: 'error'
			});
			return;
		}
		if (existedEmailNames.length > 0) {
			ElMessage({
				message: `联系人邮箱已存在：${existedEmailNames.join('、')}`,
				type: 'warning'
			});
			return;
		}
	}

	//#region 保存客户资料
	ElMessageBox.confirm('确定保存该客户资料吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		if (CustomerProfileform.customerStatus == null) {
			CustomerProfileform.customerStatus = 0;
		}
		if (addCustomerInfo.customerInfo.customerLevel == null) {
			addCustomerInfo.customerInfo.customerLevel = 0;
		}
		if (addCustomerInfo.customerInfo.pricingTerm == null) {
			addCustomerInfo.customerInfo.pricingTerm = 0;
		}
		if (addCustomerInfo.customerInfo.settlementWay == null) {
			addCustomerInfo.customerInfo.settlementWay = 0;
		}
		if (addCustomerInfo.customerInfo.collectionPeriod == null) {
			addCustomerInfo.customerInfo.collectionPeriod = 0;
		}
		if (isEditCustomerInfo.value == false) {
			if (!(await assertPotentialCanAddForNewCustomerArchive())) {
				return;
			}
			if (isImport.value == false) {
				// 处理文件上传 - 使用与SaveCustomerDraft相同的逻辑
				let filelistUrlStr = '';
				if (fileList.value && fileList.value.length > 0) {
					// 收集已存在的URL（排除blob URL）
					const existingUrls = fileList.value
						.filter(file => file.url && !file.url.startsWith('blob:'))
						.map(file => file.url);

					// 只上传新文件
					const newFiles = fileList.value.filter(file => file.raw && (!file.url || file.url.startsWith('blob:')));
					if (newFiles.length > 0) {
						const uploadPromises = newFiles.map(file => {
							const formData = new FormData();
							formData.append('FileName', file.name);
							formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
							formData.append('FileNameType', '1');
							formData.append('File', file.raw);
							formData.append('storeType', '1');
							return request.postForm(UploadUrl, formData);
						});

						Promise.all(uploadPromises).then(responses => {
							responses.forEach(response => {
								if (response?.data?.url) {
									existingUrls.push(response.data.url);
								} else {
									ElMessage({
										message: "上传客户图片出错！😔",
										type: 'error'
									})
								}
							});

							// 合并所有URL
							filelistUrlStr = existingUrls.join(',');

							// 保存客户资料
							addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
							CustomerProfileform.customerPhoto = filelistUrlStr;
							CustomerProfileform.IsDraft = 0;
							request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then(response => {
								if (response != null) {
									ElMessage({
										message: response.msg,
										type: 'success'
									})
									//清空上传图片
									filelistUrlStr = '';
									CloseCustomerProfileDetailDialog();
								} else {
									console.error('保存客户资料出错');
								}
							}).catch(error => {
								console.error('保存客户资料出错！😔错误内容：', error);
							})
						}).catch(error => {
							console.error('上传客户图片出错！😔错误内容：', error);
						});
					} else {
						// 没有新文件需要上传，直接保存
						filelistUrlStr = existingUrls.join(',');
						addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
						CustomerProfileform.customerPhoto = filelistUrlStr;
						CustomerProfileform.IsDraft = 0;
						request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then(response => {
							if (response != null) {
								ElMessage({
									message: response.msg,
									type: 'success'
								})
								CloseCustomerProfileDetailDialog();
							} else {
								console.error('保存客户资料出错');
							}
						}).catch(error => {
							console.error('保存客户资料出错！😔错误内容：', error);
						})
					}
				} else {
					// 没有文件，直接保存
					addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
					CustomerProfileform.IsDraft = 0;
					request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then(response => {
						if (response != null) {
							ElMessage({
								message: response.msg,
								type: 'success'
							})
							CloseCustomerProfileDetailDialog();
						} else {
							console.error('保存客户资料出错');
						}
					}).catch(error => {
						console.error('保存客户资料出错！😔错误内容：', error);
					})
				}
			} else {
				addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
				addCustomerInfo.customerInfo = CustomerProfileform;
				//保存导入客户资料
				request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then((response: any) => {
					if (response != null) {
						ElMessage({
							message: (response as any).msg,
							type: 'success'
						})
						//清空上传图片
						isImport.value = false;
						CloseCustomerProfileDetailDialog();
					} else {
						console.error('保存客户资料出错');
					}
				}).catch(error => {
					console.error('保存客户资料出错！😔错误内容：', error);
				})
			}
		} else {
			// 编辑客户资料 - 使用与SaveCustomerDraft相同的逻辑
			let filelistUrlStr = '';
			if (fileList.value && fileList.value.length > 0) {
				// 收集已存在的URL（排除blob URL）
				const existingUrls = fileList.value
					.filter(file => file.url && !file.url.startsWith('blob:'))
					.map(file => file.url);

				// 只上传新文件或已更改的文件
				const newFiles = fileList.value.filter(file =>
					file.raw && (!file.url || file.url.startsWith('blob:')) || file.isChanged
				);

				if (newFiles.length > 0) {
					const uploadPromises = newFiles.map(file => {
						const formData = new FormData();
						formData.append('FileName', file.name);
						formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
						formData.append('FileNameType', '1');
						formData.append('File', file.raw);
						formData.append('storeType', '1');
						return request.postForm(UploadUrl, formData);
					});

					Promise.all(uploadPromises).then(responses => {
						responses.forEach(response => {
							if (response?.data?.url) {
								existingUrls.push(response.data.url);
							} else {
								ElMessage({
									message: "上传客户图片出错！😔",
									type: 'error'
								})
							}
						});

						// 合并所有URL
						filelistUrlStr = existingUrls.join(',');

						// 保存客户资料
						addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
						CustomerProfileform.customerPhoto = filelistUrlStr;
						addCustomerInfo.customerInfo = CustomerProfileform;
						addCustomerInfo.customerInfo.id = CustomerProfileDetailDialogform.id;
						let collectionPeriodValue = CustomerProfileform.collectionPeriod;
						if (typeof collectionPeriodValue === 'string' && CustomerProfileform.collectionPeriod.toString().trim() === '') {
							collectionPeriodValue = 0;
						} else {
							collectionPeriodValue = Number(collectionPeriodValue);
						}
						addCustomerInfo.customerInfo.collectionPeriod = collectionPeriodValue;
						addCustomerInfo.customerInfo.IsDraft = 0;

						request.post('CustomerInfoMation/EditCustomerInfo/Edit', addCustomerInfo).then((response: any) => {
							if (response != null) {
								ElMessage({
									message: (response as any).msg,
									type: 'success'
								})
								//清空上传图片
								filelistUrlStr = '';
								//关闭编辑状态
								isEditCustomerInfo.value = false;
								CloseCustomerProfileDetailDialog();
							} else {
								console.error('保存客户资料出错');
							}
						}).catch(error => {
							console.error('保存客户资料出错！😔错误内容：', error);
						})
					}).catch(error => {
						console.error('上传客户图片出错！😔错误内容：', error);
					});
				} else {
					// 没有新文件需要上传，直接保存
					filelistUrlStr = existingUrls.join(',');
					addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
					CustomerProfileform.customerPhoto = filelistUrlStr;
					addCustomerInfo.customerInfo = CustomerProfileform;
					addCustomerInfo.customerInfo.id = CustomerProfileDetailDialogform.id;
					let collectionPeriodValue = CustomerProfileform.collectionPeriod;
					if (typeof collectionPeriodValue === 'string' && CustomerProfileform.collectionPeriod.toString().trim() === '') {
						collectionPeriodValue = 0;
					} else {
						collectionPeriodValue = Number(collectionPeriodValue);
					}
					addCustomerInfo.customerInfo.collectionPeriod = collectionPeriodValue;
					addCustomerInfo.customerInfo.IsDraft = 0;

					request.post('CustomerInfoMation/EditCustomerInfo/Edit', addCustomerInfo).then(response => {
						if (response != null) {
							ElMessage({
								message: response.msg,
								type: 'success'
							})
							//关闭编辑状态
							isEditCustomerInfo.value = false;
							CloseCustomerProfileDetailDialog();
						} else {
							console.error('保存客户资料出错');
						}
					}).catch(error => {
						console.error('保存客户资料出错！😔错误内容：', error);
					})
				}
			} else {
				// 没有文件，直接保存
				addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
				addCustomerInfo.customerInfo = CustomerProfileform;
				addCustomerInfo.customerInfo.id = CustomerProfileDetailDialogform.id;
				let collectionPeriodValue = CustomerProfileform.collectionPeriod;
				if (typeof collectionPeriodValue === 'string' && CustomerProfileform.collectionPeriod.toString().trim() === '') {
					collectionPeriodValue = 0;
				} else {
					collectionPeriodValue = Number(collectionPeriodValue);
				}
				addCustomerInfo.customerInfo.collectionPeriod = collectionPeriodValue;
				addCustomerInfo.customerInfo.IsDraft = 0;

				request.post('CustomerInfoMation/EditCustomerInfo/Edit', addCustomerInfo).then((response: any) => {
					if (response != null) {
						ElMessage({
							message: (response as any).msg,
							type: 'success'
						})
						//关闭编辑状态
						isEditCustomerInfo.value = false;
						CloseCustomerProfileDetailDialog();
					} else {
						console.error('保存客户资料出错');
					}
				}).catch(error => {
					console.error('保存客户资料出错！😔错误内容：', error);
				})
			}
		}
		//关闭建档窗体
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消保存'
		})
	})
	//#endregion 
}

const handleDownload = (file: UploadFile) => {
	console.log(file)
}

//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(30);
//客户基本信息表格
const CunstomeinfotableData = ref([])
const handlePageChange = async (newPage) => {
	currentPage.value = newPage;
	await GetCustomeInfoList(newPage, pageSize.value);
};
const handleSizeChange = async (size) => {
	pageSize.value = size;
	currentPage.value = 1;
	await GetCustomeInfoList(1, size);
};

//获取客户信息列表
function GetCustomeInfoList(start, end) {
	return new Promise((resolve, reject) => {
		request({
			url: 'CustomerInfoMation/GetCustomerInfoList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				customerNo: Search_CustomerNo_input.value || '',
				emailAddress: Search_CustomerEmail_input.value || '',
				customer: Search_CustomerName_input.value || 0,
				contactPerson: Search_CustomerContactPerson_input.value || '',
				BusinessScope: Search_BusinessScope_Select.value || 0,
				TradingCountry: Search_TradingCountry_Select.value || 0,
				CustomerLevel: Search_CustomerLevel_Select.value || 0,
				CustomerSource: Search_CustomerSource_Select.value || 0,
				StartDate: Search_StartDate_Select.value || '',
				EndDate: Search_EndDate_Select.value || ''
			}
		}).then(response => {
			totalItems.value = response.data.totalNum;
			if (response.data.result.length > 0) {
				CunstomeinfotableData.value = response.data.result;
				CunstomeinfotableData.value.forEach(item => {
					item._salesPersonValue = item.salesPerson;
					item.customerStatus = state.optionss['hr_customer_status'].filter(option => option.dictValue == item.customerStatus).map(option => option.dictLabel).values().next().value;
					item.customerLevel = state.optionss['hr_customer_level'].filter(option => option.dictValue == item.customerLevel).map(option => option.dictLabel).values().next().value;
					item.tradingCountry = state.optionss['hr_nation'].filter(option => option.dictValue == item.tradingCountry).map(option => option.dictLabel).values().next().value;
					item.customerSource = state.optionss['sys_customer_source'].filter(option => option.dictValue == item.customerSource).map(option => option.dictLabel).values().next().value;
					item.businessScope = state.optionss['hr_business_scope'].filter(option => option.dictValue == item.businessScope).map(option => option.dictLabel).values().next().value;
					item.salesPerson = state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.salesPerson).map(option => option.dictLabel).values().next().value;
					item.create_by = state.optionss['sql_all_user'].filter(option => option.dictValue == item.create_by).map(option => option.dictLabel).values().next().value;
					item.lastbindSalePerson = state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.lastbindSalePerson).map(option => option.dictLabel).values().next().value;
				});
				resolve(response.data);
			} else {
				if (response.data.totalNum > 0 && start > 1) {
					GetCustomeInfoList(start - 1, end);
				} else {
					CunstomeinfotableData.value = [];
					resolve([]); // 确保在没有数据时也会 resolve
				}
			}
		}).catch(error => {
			console.error(error);
			reject(error);
		});
	});
}
//详情联系人列表
const ContactPersonData = ref([]);
//联系日志
const ContactLogData = ref([]);
//报价记录
const QuotationRecordData = ref([]);
//选择客户ID
const selectCustomerID = ref(0);
//双击查看详情
const CunstomeinfotableDatahandleRowDblClick = (row) => {
	selectCustomerID.value = row.id;
	OpenCustomerProfileDetailDialog(row);
}

//打开客户详情窗体并加载数据
const OpenCustomerProfileDetailDialog = (row) => {
	selectCustomerID.value = row.id;
	clearUploadfile();
	CustomerProfileDetailDialogform.id = row.id;
	CustomerProfileDetailDialogform.customerStatus = state.optionss['hr_customer_status'].filter(item => item.dictLabel == row.customerStatus).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictLabel == row.customerLevel).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.customerNo = row.customerNo;
	CustomerProfileDetailDialogform.customerAbbreviation = row.customerAbbreviation;
	CustomerProfileDetailDialogform.customerName = row.customerName;
	CustomerProfileDetailDialogform.tradingCountry = state.optionss['hr_nation'].filter(item => item.dictLabel == row.tradingCountry).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.compantWebsite = row.compantWebsite;
	CustomerProfileDetailDialogform.customerSource = state.optionss['sys_customer_source'].filter(item => item.dictLabel == row.customerSource).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.businessScope = state.optionss['hr_business_scope'].filter(item => item.dictLabel == row.businessScope).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.pricingTerm = state.optionss['hr_pricing_term'].filter(item => item.dictValue == row.pricingTerm).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.settlementWay = state.optionss['hr_settlement_way'].filter(item => item.dictValue == row.settlementWay).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.collectionPeriod = row.collectionPeriod;
	CustomerProfileDetailDialogform.customerTaxNumber = row.customerTaxNumber;
	CustomerProfileDetailDialogform.salesPerson = state.optionss['sql_hr_sale'].filter(item => item.dictLabel == row.salesPerson).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.address1 = row.address1;
	if (row.customerPhoto != null && row.customerPhoto != '') {
		//NewCustomerleadsform.compantPhotoStr = row.compantPhotoStr;
		row.customerPhoto.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false,
					uid: Date.now() + Math.random(),
					status: 'success'
				});
			}
		});
	}
	uploadedFiles.value = fileList.value;
	//加载客户联系人
	loadCustomerContractPerson(row.id);
	//加载报价记录
	loadQuotationHistory(row.id);
	//加载销售合同记录
	loadContractHistory(row.id);
	//加载收寄样历史
	loadCustomerSendSampleHistory(row.id);
	//加载财务总账
	loadFinanceLedger(row.id);
	//加载日志标签
	loadContactLogTagData();
	CustomerProfileDetailDialog.value = true;
}

/** 将客户释放到公海（仅后端校验所属销售员，前端仅控制按钮显示） */
const releaseCustomerToPool = async (customerId: number) => {
	if (!customerId) return;
	try {
		await ElMessageBox.confirm(
			'确定将该客户释放到客户公海吗？释放后将不再归属于当前销售人员。',
			'提示',
			{
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}
		);
	} catch {
		return;
	}
	try {
		const response: any = await request({
			url: 'CustomerInfoMation/ReleaseCustomerToPool/ReleaseCustomerToPool',
			method: 'GET',
			params: { customerId }
		});
		if (response?.code == 200 || response?.code === '200') {
			const okMsg =
				typeof response?.data === 'string' && response.data
					? response.data
					: response?.msg;
			ElMessage.success(okMsg || '操作成功');
			CustomerProfileDetailDialog.value = false;
			await GetCustomeInfoList(currentPage.value, pageSize.value);
		} else {
			ElMessage.error(response?.msg || '释放失败');
		}
	} catch (e: any) {
		ElMessage.error(e?.message || '释放失败');
	}
};

const loadCustomerContractPerson = (customerId) => {
	request({
		url: 'CustomerInfoMation/GetCustomerContractPersonListByCustomerID/GetContractPersonData',
		method: 'GET',
		params: { CustomerID: customerId }
	}).then(response => {
		ContactLogData.value = [];//清空联系日志
		if (response.data.length > 0) {
			ContactPersonData.value = response.data;
			ContactPersonData.value.forEach(item => {
				if (item.sex == null || item.sex == '' || item.sex == undefined) {
					item.sexText = state.optionss['sys_user_sex'].find(option => "0" && option.dictValue.toString() === "0").dictLabel;
				} else {
					item.sexText = state.optionss['sys_user_sex'].find(option => item.sex && option.dictValue.toString() === item.sex.toString()).dictLabel;
				}
			});
			const contactEmailStr = ContactPersonData.value
				.map(item => item.email)
				.filter(email => email && email.trim() !== '')
				.join(',');
			if (contactEmailStr) {
				ContactLogTablecurrentPage.value = 1;
				loadCustomerContactLogs(
					selectCustomerID.value,
					contactEmailStr,
					1,
					ContactLogTablepageSize.value
				);
			}
		} else {
			ContactPersonData.value = [];
		}
	}).catch(error => {
		console.error('获取客户联系人失败:', error);
	});
}

//日期格式化函数
const formatDateTime = (dateTimeStr) => {
	if (!dateTimeStr) return '';
	const date = new Date(dateTimeStr);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	const hours = String(date.getHours()).padStart(2, '0');
	const minutes = String(date.getMinutes()).padStart(2, '0');
	const seconds = String(date.getSeconds()).padStart(2, '0');
	return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

//日期格式化函数（只显示年月日）
const formatDate = (dateTimeStr) => {
	if (!dateTimeStr) return '';
	const date = new Date(dateTimeStr);
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	return `${year}-${month}-${day}`;
}

const getFollowUpTimeColor = (dateTimeStr) => {
	if (!dateTimeStr) return '';
	const followUpDate = new Date(dateTimeStr);
	if (Number.isNaN(followUpDate.getTime())) return '';

	const today = new Date();
	today.setHours(0, 0, 0, 0);
	followUpDate.setHours(0, 0, 0, 0);
	const days = Math.floor((followUpDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

	if (days < 0) return 'red';
	if (days <= 2) return 'red';
	if (days <= 7) return 'orange';
	if (days <= 15) return 'blue';
	return 'green';
}

const formatFinanceAmount = (amount: number): string => {
	if (!amount) return ''
	return Number(amount).toFixed(2)
}

const parseFinanceRemarks = (row: FinanceApiRow): FinanceRemarks => {
	return {
		customerOrSupplier: row.customerOrPayee || '-',
		contractOrRemark: row.contractOrRemark || '-'
	}
}

const transformFinanceLedgerData = (apiRows: FinanceApiRow[]): FinanceLedgerItem[] => {
	return apiRows.map(row => ({
		date: formatDate(row.date),
		summary: row.summary || '',
		income: {
			rmb: row.incomeCny || 0,
			usd: row.incomeUsd || 0,
			eur: row.incomeEur || 0
		},
		expenditure: {
			rmb: row.expenseCny || 0,
			usd: row.expenseUsd || 0,
			eur: row.expenseEur || 0
		},
		remarks: parseFinanceRemarks(row)
	}))
}

const handleFinanceLedgerPageChange = (page: number) => {
	financeLedgerCurrentPage.value = page
}
const handleFinanceLedgerSizeChange = (size: number) => {
	financeLedgerPageSize.value = size
	financeLedgerCurrentPage.value = 1
}

const loadFinanceLedger = async (customerId: number) => {
	financeLedgerLoading.value = true
	try {
		const response = await getFinancialGeneralLedgerData(customerId || null, null)
		if (response && response.code === 200 && response.data && response.data.rows) {
			const currentCustomerLabel = CustomerProfileDetailDialogform.customerAbbreviation
				|| CustomerProfileDetailDialogform.customerName
				|| ''
			const filteredRows = currentCustomerLabel
				? response.data.rows.filter((row: FinanceApiRow) =>
					(row.customerOrPayee || '').includes(currentCustomerLabel)
				)
				: response.data.rows
			financeLedgerData.value = transformFinanceLedgerData(filteredRows)
			financeLedgerCurrentPage.value = 1
		} else {
			financeLedgerData.value = []
		}
	} catch (error) {
		console.error('加载财务总账数据失败:', error)
		financeLedgerData.value = []
	} finally {
		financeLedgerLoading.value = false
	}
}

// 获取联系日志数据的方法
const getContactLogList = (emailAddress) => {
	ContactLogData.value = [];
	request({
		url: 'Email/GetEmailContactRecords/GetEmailList',
		method: 'GET',
		params: {
			EmailAddress: emailAddress
		}
	}).then(response => {
		if (response.data.length > 0) {
			response.data.forEach(item => {
				ContactLogData.value.push({
					EmailDate: formatDateTime(item.emaildate),
					Contact: item.toEmail,
					OurPersonnel: item.fromEmail,
					ContactDetails: item.emailsubject
				});
			});
		} else {
			ContactLogData.value = [];
		}
	}).catch(error => {
		console.error('获取联系日志失败:', error);
		ContactLogData.value = [];
	});
}


const CloseCustomerProfileDetailDialog = () => {
	CustomerProfileDialog.value = false;
	GetCustomeInfoList(currentPage.value, pageSize.value);
}


//客户查重表单
const CustomerDuplicationCheckform = reactive({
	//客户简称
	Cabbreviation: '',
	//客户名称
	Cname: '',
	//客户地址
	Caddress: '',
	//客户邮箱
	Cemail: '',
	//客户电话
	Ctellphone: ''
})

const CustomerDuplicationCheckData = ref([]);

//客户详情表单
const CustomerProfileDetailDialogform = reactive<CustomerProfileform>({
	update_time: "",
	id: 0,
	customerNo: "",
	customerStatus: null,
	customerLevel: null,
	customerAbbreviation: "",
	customerName: "",
	tradingCountry: null,
	compantWebsite: "",
	customerSource: null,
	businessScope: null,
	pricingTerm: null,
	settlementWay: null,
	collectionPeriod: null,
	customerTaxNumber: "",
	salesPerson: null,
	address1: "",
	customerPhoto: "",
	remark: "",
	create_by: "",
	create_time: "",
	update_by: "",
	isDelete: 0,
	IsDraft: 0
});

const isCurrentUserSalesPerson = (salesPersonId: unknown) => {
	if (salesPersonId === null || salesPersonId === undefined || salesPersonId === '') {
		return false;
	}
	return Number(salesPersonId) === Number(userStore.userId);
};

const canEditSelectedCustomer = computed(() => {
	if (!CustomerProfileDetailDialogform.id) {
		return true;
	}
	return isCurrentUserSalesPerson(CustomerProfileDetailDialogform.salesPerson);
});

const canSubmitCustomerProfile = computed(() => {
	if (!isEditCustomerInfo.value) {
		return true;
	}
	return isCurrentUserSalesPerson(CustomerProfileform.salesPerson);
});


const DuplicationCheckSearch = () => {
	if (CustomerDuplicationCheckform.Cabbreviation != '' || CustomerDuplicationCheckform.Cname != '' || CustomerDuplicationCheckform.Caddress != '' || CustomerDuplicationCheckform.Cemail != '' || CustomerDuplicationCheckform.Ctellphone != '') {
		return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
			request({
				url: '/CustomerInfoMation/GetCustomerQueryIsDuplicated/GetCustomerQueryIsDuplicatedList',
				method: 'GET',
				params: {
					customerCabbreviation: CustomerDuplicationCheckform.Cabbreviation,
					customerCname: CustomerDuplicationCheckform.Cname,
					customerCAddress: CustomerDuplicationCheckform.Caddress,
					customerCemail: CustomerDuplicationCheckform.Cemail,
					CustomerCtellphone: CustomerDuplicationCheckform.Ctellphone
				}
			}).then(response => {
				if (response.data.length > 0) {
					CustomerDuplicationCheckData.value = response.data;
					resolve(response.data);
				} else {
					ElMessage({
						message: '当前系统中不存在所查询的客户信息😊！',
						type: 'warning'
					})
					reject(new Error('无数据'));  // Reject the promise if the response is null
				}
			}).catch(error => {
				console.error(error);
				reject(error);  // Reject the promise if an error occurs
			});
		});
	} else {
		ElMessage({
			message: '请填写查重字段😒！',
			type: 'warning'
		})
	}

}
const DuplicationCheckReset = () => {
	CustomerDuplicationCheckform.Cabbreviation = '';
	CustomerDuplicationCheckform.Cname = '';
	CustomerDuplicationCheckform.Caddress = '';
	CustomerDuplicationCheckform.Cemail = '';
	CustomerDuplicationCheckform.Ctellphone = '';
	CustomerDuplicationCheckData.value = [];
}

// 获取版本文字显示
const getVersionText = (version) => {
	if (version === 1) return "一次报价";
	if (version === 2) return "二次报价";
	if (version === 3) return "三次报价";
	if (version === 4) return "四次报价";
	if (version === 5) return "五次报价";
	if (version === 6) return "六次报价";
	if (version === 7) return "七次报价";
	if (version === 8) return "八次报价";
	if (version === 9) return "九次报价";
	if (version === 10) return "十次报价";
	return `${version}次报价`;
};
// 获取报价历史记录
const loadQuotationHistory = async (customerId: number, pageNum: number = 1, pageSize: number = 10) => {
	try {
		const response = await request({
			url: 'Quotation/GetQuotaionList/GetList',
			method: 'GET',
			params: {
				PageNum: pageNum,
				PageSize: pageSize,
				quotationnum: '',
				customerid: customerId,
				inquiryDate: '',
				realQuotationDate: '',
				showAllVersions: false // 添加是否显示所有版本参数
			}
		});
		if (response.code === 200) {
			QuotationRecordData.value = [];
			// 设置分页信息
			QuotationRecordCurrentPage.value = response.data.pageIndex || pageNum
			QuotationRecordPageSize.value = response.data.pageSize || pageSize
			QuotationRecordTotalItems.value = response.data.totalNum || 0
			// 转换数据，有效期使用 hr_quotationalidity 字典标签
			const result = response.data.result || [];
			QuotationRecordData.value = result.map(item => ({
				...item,
				validityPeriod: state.optionss['hr_quotationalidity']?.find(d => d.dictValue == item.validityPeriod)?.dictLabel ?? item.validityPeriod
			}));
		} else {
			ElMessage.error((response as any).msg || '获取报价历史失败');
		}
	} catch (error) {
		console.error('获取报价历史失败:', error);
		ElMessage.error('获取报价历史失败，请稍后重试');
	}
};
//销售合同记录
const SalesContractRecordData = ref([]);
// 获取销售历史记录
const loadContractHistory = async (customerId: number, pageNum: number = 1, pageSize: number = 10) => {
	try {
		const response = await request({
			url: 'Contracts/GetContractHistoryByCustomerID/GetHistory',
			method: 'GET',
			params: {
				CustomerID: customerId,
				PageNum: pageNum,
				PageSize: pageSize
			}
		});

		if (response.code === 200) {
			SalesContractRecordData.value = [];
			// 设置分页信息
			SalesContractRecordCurrentPage.value = response.data.pageIndex || pageNum
			SalesContractRecordPageSize.value = response.data.pageSize || pageSize
			SalesContractRecordTotalItems.value = response.data.totalNum || 0
			// 转换数据
			SalesContractRecordData.value = response.data.result || response.data;
		} else {
			ElMessage.error((response as any).msg || '获取销售历史失败');
		}
	} catch (error) {
		console.error('获取销售历史失败:', error);
		ElMessage.error('获取销售历史失败，请稍后重试');
	}
};
// 定义接口类型
interface SampleHistoryItem {
	id: number;
	type: number;                    // 1:寄样 2:收样
	customer_or_Supplier: number;    // 1:客户 2:供应商
	waybill_Number: string;
	express_Company: number;
	sample_Date: string;
	customer_ID: number;
	abbreviation: string;
	company_ID: number;
	salesperson_ID: number;
	payment_Method: number;
	paid_Express_Fee: number;
	remark: string | null;
}
const CustomerSendSampleData = ref([]) //收寄样历史
const ShipRecoreData = ref([]) //出货记录
// 获取客户寄样历史
const loadCustomerSendSampleHistory = async (customerId, pageNum: number = 1, pageSize: number = 10) => {
	try {
		const response = await request({
			url: 'ProductSample/GetCustomerHistoryByCustomerID/GetHistory',
			method: 'GET',
			params: {
				CustomerID: customerId,
				PageNum: pageNum,
				PageSize: pageSize
			}
		});

		if (response.code === 200) {
			// 设置分页信息
			CustomerSendSampleCurrentPage.value = response.data.pageIndex || pageNum
			CustomerSendSamplePageSize.value = response.data.pageSize || pageSize
			CustomerSendSampleTotalItems.value = response.data.totalNum || 0

			// 转换数据
			const data = response.data.result || response.data;
			CustomerSendSampleData.value = data.map((item: SampleHistoryItem) => ({
				type: item.type === 1 ? '寄样' : '收样',
				waybillNumber: item.waybill_Number,
				expressCompany: state.optionss.hr_express_delivery_company.find(
					company => company.dictValue === item.express_Company.toString()
				)?.dictLabel || '',
				sampleDate: item.sample_Date ? item.sample_Date.split(' ')[0] : '',
				abbreviation: item.abbreviation,
				companyName: state.optionss.hr_ourcompany.find(
					company => company.dictValue === item.company_ID.toString()
				)?.dictLabel || '',
				paymentMethod: state.optionss.hr_express_payment_method.find(
					method => method.dictValue === item.payment_Method.toString()
				)?.dictLabel || '',
				paidExpressFee: item.paid_Express_Fee?.toFixed(2) || '0.00',
				remark: item.remark || '',
				id: item.id
			}));
		} else {
			ElMessage.error((response as any).msg || '获取收寄样历史失败');
		}
	} catch (error) {
		console.error('获取收寄样历史失败:', error);
		ElMessage.error('获取收寄样历史失败，请稍后重试');
	}
};

// 联系日志相关
const contactLogDialogVisible = ref(false)
const contactLogFormRef = ref<FormInstance>()
const contactLogAttachments = ref<any[]>([])
const contactLogImages = ref<any[]>([])
const previewImageVisible = ref(false)
const previewImageUrl = ref('')
const quotationNumberOptions = ref([])

interface ContactLogForm {
	contactDate: string | Date
	ContactLogTag: string | number
	contactPerson: string | number
	ourStaff: string | number
	contactContent: string
	remark: string
	attachmentURLs: string
	imageURLs: string
	relatedDocumentType: number
	relatedDocumentID: number
	businessOpportunityName: string
}

const contactLogForm = reactive<ContactLogForm>({
	contactDate: new Date(),
	contactPerson: '',
	ContactLogTag: '',
	ourStaff: '',
	contactContent: '',
	remark: '',
	attachmentURLs: '',
	imageURLs: '',
	relatedDocumentType: 1,
	relatedDocumentID: null,
	businessOpportunityName: ''
})

// 判断关联单据是否被锁定
const isRelatedDocumentLocked = computed(() => {
	if (!contactLogForm.ContactLogTag) return false
	const selectedTag = ContactLogTagData.value.find(tag => tag.id === contactLogForm.ContactLogTag)
	if (!selectedTag) return false

	const tagName = selectedTag.emailTagName
	return tagName === '初次报价' || tagName === '再次报价' || tagName === '合同确定'
})

// 获取关联单据占位符
const getRelatedDocumentPlaceholder = () => {
	switch (contactLogForm.relatedDocumentType) {
		case 1:
			return '请选择销售合同'
		case 2:
			return '请选择出运合同'
		case 3:
			return '请选择报价单'
		default:
			return '请选择关联单据'
	}
}

// 获取关联单据选项
const getRelatedDocumentOptions = () => {
	switch (contactLogForm.relatedDocumentType) {
		case 1:
			return state.optionss.customer_contract_data || []
		case 2:
			return state.optionss.sql_shippingdeliveries || []
		case 3:
			return quotationNumberOptions.value || []
		default:
			return []
	}
}

const relatedDocumentTypeOptions = computed(() => {
	const options = []
	if ((state.optionss.customer_contract_data || []).length > 0) {
		options.push({ value: 1, label: '销售合同' })
	}
	if ((state.optionss.sql_shippingdeliveries || []).length > 0) {
		options.push({ value: 2, label: '出运合同' })
	}
	if ((quotationNumberOptions.value || []).length > 0) {
		options.push({ value: 3, label: '报价单' })
	}
	return options
})

const normalizeRelatedDocumentType = () => {
	const options = relatedDocumentTypeOptions.value
	if (!options.length) return
	if (!options.some(option => option.value === contactLogForm.relatedDocumentType)) {
		contactLogForm.relatedDocumentType = options[0].value
		contactLogForm.relatedDocumentID = null
	}
}

watch(relatedDocumentTypeOptions, () => {
	normalizeRelatedDocumentType()
})

const RelatedDocumentsChange = (value) => {
	// 清空当前选择的值
	contactLogForm.relatedDocumentID = null;
	if (value === 1) {
		// 切换到销售合同选项
		state.optionss.sql_sale_contracts = state.optionss.sql_sale_contracts || [];
	} else if (value === 2) {
		// 切换到出运合同选项
		state.optionss.sql_shippingdeliveries = state.optionss.sql_shippingdeliveries || [];
	} else if (value === 3) {
		// 切换到报价单选项
		loadQuotationNumberOptions();
	}
}

const contactLogRules = reactive<FormRules>({
	contactDate: [{ required: true, message: '请选择联系日期', trigger: 'change' }],
	ContactLogTag: [{ required: true, message: '请选择日志标签', trigger: 'change' }],
	contactPerson: [{ required: true, message: '请选择联系人', trigger: 'change' }],
	ourStaff: [{ required: true, message: '请选择我方人员', trigger: 'change' }],
	contactContent: [{ required: true, message: '请输入联系内容', trigger: 'blur' }],
	businessOpportunityName: [{
		required: false,
		validator: (rule, value, callback) => {
			// 检查是否为询盘标签
			const isInquiryTag = contactLogForm.ContactLogTag &&
				ContactLogTagData.value.find(tag => tag.id === contactLogForm.ContactLogTag)?.emailTagName === '询盘'

			if (showBusinessOpportunityField.value && isInquiryTag && !value) {
				callback(new Error('日志标签为询盘时，必须填写商机名称'))
			} else {
				callback()
			}
		},
		trigger: 'blur'
	}],


})

// 打开添加联系日志对话框
const AddContactLog = async () => {
	// 重置表单
	contactLogForm.contactDate = new Date()
	contactLogForm.contactPerson = ''
	contactLogForm.ourStaff = state.optionss.sql_hr_sale.find(item => item.dictValue == useUserStore().userId).dictValue
	contactLogForm.contactContent = ''
	contactLogForm.remark = ''
	contactLogForm.attachmentURLs = ''
	contactLogForm.imageURLs = ''
	contactLogForm.ContactLogTag = ''
	contactLogForm.businessOpportunityName = ''
	contactLogForm.relatedDocumentType = 1 // 默认选择销售合同
	contactLogForm.relatedDocumentID = null
	normalizeRelatedDocumentType()
	contactLogAttachments.value = []
	contactLogImages.value = []
	previewImageUrl.value = ''

	// 重置字段显示状态
	showBusinessOpportunityField.value = false

	// 显示对话框
	contactLogDialogVisible.value = true
}

// 联系记录图片：最多3张，每张1M
const handleContactLogImageChange = (file, fileList) => {
	if (file.raw && file.raw.size > IMAGE_MAX_SIZE) {
		ElMessage.error('每张图片不能超过 1M')
		contactLogImages.value = fileList.filter(f => f.uid !== file.uid)
		return
	}
	if (fileList.length > 3) {
		ElMessage.warning('最多上传3张图片')
		contactLogImages.value = fileList.slice(0, 3)
	}
}
// 联系记录附件：总量不超过 50M
const ATTACHMENT_TOTAL_MAX = 50 * 1024 * 1024
const getAttachmentTotalSize = (list) => (list || []).reduce((sum, f) => sum + (f.raw ? f.raw.size : 0), 0)
const handleContactLogAttachmentChange = (file, fileList) => {
	if (getAttachmentTotalSize(fileList) > ATTACHMENT_TOTAL_MAX) {
		ElMessage.error('附件总大小不能超过 50M')
		contactLogAttachments.value = fileList.filter(f => f.uid !== file.uid)
	}
}
// 图片预览
const handleImagePreview = (file) => {
	previewImageUrl.value = file.url || URL.createObjectURL(file.raw)
	previewImageVisible.value = true
}

// 提交联系日志
const submitContactLog = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate(async (valid) => {
		if (valid) {
			try {
				// 处理附件上传
				let attachmentUrlStr = ''
				let imageUrlStr = ''

				// 上传附件
				if (contactLogAttachments.value.length > 0) {
					const uploadPromises = contactLogAttachments.value.map(file => {
						const formData = new FormData()
						formData.append('FileName', file.name)
						formData.append('FileDir', 'CustomerInfo/ContactLogAttachments')
						formData.append('FileNameType', '1')
						formData.append('File', file.raw)
						formData.append('storeType', '2');
						return request.postForm(UploadUrl, formData);
					})

					const uploadResults = await Promise.all(uploadPromises)

					// 收集上传后的URL
					uploadResults.forEach(response => {
						if (response && response.code === 200) {
							if (attachmentUrlStr) {
								attachmentUrlStr += ','
							}
							attachmentUrlStr += response.data.downloadurl
						}
					})
				}

				// 上传图片
				if (contactLogImages.value.length > 0) {
					const uploadPromises = contactLogImages.value.map(file => {
						const formData = new FormData()
						formData.append('FileName', file.name)
						formData.append('FileDir', 'CustomerInfo/ContactLogImages')
						formData.append('FileNameType', '1')
						formData.append('File', file.raw)
						formData.append('storeType', '1');
						return request.postForm(UploadUrl, formData);
					})

					const uploadResults = await Promise.all(uploadPromises)

					// 收集上传后的URL
					uploadResults.forEach(response => {
						if (response && response.code === 200) {
							if (imageUrlStr) {
								imageUrlStr += ','
							}
							imageUrlStr += response.data.url
						}
					})
				}

				// 准备提交的数据
				const contactLogData = {
					CustomerID: selectCustomerID.value,
					ContactDate: contactLogForm.contactDate instanceof Date
						? contactLogForm.contactDate.toISOString().split('T')[0]
						: contactLogForm.contactDate,
					ContactTag: contactLogForm.ContactLogTag,
					ContactPerson: contactLogForm.contactPerson,
					ContactContent: contactLogForm.contactContent,
					OurStaff: contactLogForm.ourStaff,
					AttachmentURLs: attachmentUrlStr,
					ImageURLs: imageUrlStr,
					Remark: contactLogForm.remark,
					relatedDocumentType: contactLogForm.relatedDocumentType,
					relatedDocumentID: contactLogForm.relatedDocumentID,
					businessOpportunityName: contactLogForm.businessOpportunityName,
					businessOpportunityID: (() => {
						// 如果日志标签是询盘，businessOpportunityID设置为0
						const selectedTag = ContactLogTagData.value.find(tag => tag.id === contactLogForm.ContactLogTag)
						if (selectedTag && selectedTag.emailTagName === '询盘') {
							return 0
						}
						// 确保返回整数类型
						const businessOpportunityID = contactLogForm.businessOpportunityName
						return businessOpportunityID ? parseInt(businessOpportunityID) : 0
					})(),
					// 添加必需的customerContactLog字段
					customerContactLog: {
						CustomerID: selectCustomerID.value,
						ContactDate: contactLogForm.contactDate instanceof Date
							? contactLogForm.contactDate.toISOString().split('T')[0]
							: contactLogForm.contactDate,
						ContactTag: contactLogForm.ContactLogTag,
						ContactPerson: contactLogForm.contactPerson,
						ContactContent: contactLogForm.contactContent,
						OurStaff: contactLogForm.ourStaff,
						AttachmentURLs: attachmentUrlStr,
						ImageURLs: imageUrlStr,
						Remark: contactLogForm.remark,
						relatedDocumentType: contactLogForm.relatedDocumentType,
						relatedDocumentID: contactLogForm.relatedDocumentID,
						businessOpportunityName: contactLogForm.businessOpportunityName,
						businessOpportunityID: (() => {
							const selectedTag = ContactLogTagData.value.find(tag => tag.id === contactLogForm.ContactLogTag)
							if (selectedTag && selectedTag.emailTagName === '询盘') {
								return 0
							}
							const businessOpportunityID = contactLogForm.businessOpportunityName
							return businessOpportunityID ? parseInt(businessOpportunityID) : 0
						})()
					}
				}

				// 发送请求保存联系日志
				const response = await request({
					url: 'CustomerInfoMation/AddCustomerContractLog/AddCustomerContractLog',
					method: 'post',
					data: contactLogData
				})

				if (response && response.code === 200) {
					ElMessage({
						message: '添加联系日志成功',
						type: 'success'
					})

					// 关闭对话框
					contactLogDialogVisible.value = false

					// 刷新联系日志列表
					const emailAddresses = ContactPersonData.value
						.filter(person => person.email && person.email.trim() !== '')
						.map(person => person.email)
						.join(',')
					loadCustomerContactLogs(selectCustomerID.value, emailAddresses)
				} else {
					ElMessage.error((response as any).msg || '添加联系日志失败')
				}
			} catch (error) {
				console.error('添加联系日志失败:', error)
				ElMessage.error('添加联系日志失败')
			}
		}
	})
}


const ContactLogTablecurrentPage = ref(1)
const ContactLogTablepageSize = ref(30)
const ContactLogTabletotalItems = ref(0)
const ContactLogTablehandlePageChange = async (newPage: number) => {
	const emailAddresses = ContactPersonData.value
		.filter(person => person.email && person.email.trim() !== '')
		.map(person => person.email)
		.join(',')
	ContactLogTablecurrentPage.value = newPage;
	loadCustomerContactLogs(selectCustomerID.value, emailAddresses, newPage, ContactLogTablepageSize.value)
}
const ContactLogTablehandleSizeChange = async (size: number) => {
	ContactLogTablepageSize.value = size;
	ContactLogTablecurrentPage.value = 1;
	const emailAddresses = ContactPersonData.value
		.filter(person => person.email && person.email.trim() !== '')
		.map(person => person.email)
		.join(',')
	loadCustomerContactLogs(selectCustomerID.value, emailAddresses, 1, size)
}

// 加载客户联系日志
const loadCustomerContactLogs = async (customerId: number, emailaddress: string = '', pageNum: number = 1, pageSize: number = 30) => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetContactLogsByCustomerID/GetContactLogs',
			method: 'get',
			params: {
				customerid: customerId,
				emailaddress: emailaddress,
				pageNum: pageNum,
				pageSize: pageSize
			}
		})
		if (response && response.code === 200) {
			// 设置分页信息
			ContactLogTablecurrentPage.value = response.data.pageIndex
			if (response.data.pageSize && response.data.pageSize !== ContactLogTablepageSize.value) {
				ContactLogTablepageSize.value = response.data.pageSize
			}
			ContactLogTabletotalItems.value = response.data.totalNum || 0
			// 使用返回的统一格式数据
			ContactLogData.value = response.data.result || []
			// 使用返回的统一格式数据，但需要转换联系人和我方人员的ID为名称
			ContactLogData.value = (response.data.result || []).map(item => {
				if (item.logSouce == "人工记录") {
					// 查找联系人名称 - 将ID转换为名称
					let contactName = item.contact;
					if (!isNaN(Number(item.contact))) {
						const contactPerson = ContactPersonData.value.find(contact => contact.id === Number(item.contact));
						if (contactPerson) {
							contactName = contactPerson.name;
						}
					}
					// 查找我方人员名称 - 将ID转换为名称
					let ourStaffName = item.ourPersonnel;
					if (!isNaN(Number(item.ourPersonnel))) {
						const staff = state.optionss.sql_hr_sale.find(staff => String(staff.dictValue) === String(item.ourPersonnel));
						if (staff) {
							ourStaffName = staff.dictLabel;
						}
					}
					// 查找日志标签名称 - 将ID转换为名称
					let contactTagName = item.contactTag;
					if (!isNaN(Number(item.conactTag))) {
						const contactTag = ContactLogTagData.value.find(tag => tag.id === Number(item.conactTag));
						if (contactTag) {
							contactTagName = contactTag.emailTagName;
						}
					}
					return {
						...item,
						contact: contactName,
						ourPersonnel: ourStaffName,
						conactTag: contactTagName
					};
				} else {
					return item;
				}
			})
		} else {
			ElMessage.error(response.msg || '获取联系日志失败')
		}
	} catch (error) {
		console.error('获取联系日志失败:', error)
	}
}

// 下载附件
const downloadAttachment = (attachmentUrls: string) => {
	if (!attachmentUrls) return

	const urls = attachmentUrls.split(',')
	if (urls.length === 1) {
		// 单个附件直接下载
		window.open(urls[0], '_blank')
	} else {
		// 多个附件，显示选择列表
		ElMessageBox.confirm(
			'<div style="max-height: 300px; overflow-y: auto;">' +
			urls.map((url, index) => {
				const fileName = url.split('/').pop()
				return `<div style="margin: 10px 0;"><a href="${url}" target="_blank">${index + 1}. ${fileName}</a></div>`
			}).join('') +
			'</div>',
			'选择要下载的附件',
			{
				dangerouslyUseHTMLString: true,
				confirmButtonText: '关闭',
				showCancelButton: false,
				type: 'info',
			}
		)
	}
}

// 在加载客户详情时加载联系日志
const loadCustomerDetail = async (customerId: number) => {
	try {
		// ... existing code ...

		// 加载联系人信息
		loadCustomerContractPerson(customerId)

		// 加载联系日志 - 先加载联系人信息，然后在回调中加载联系日志
		// 这里不传递邮箱参数，因为此时联系人数据可能还没加载完成
		// 联系人加载完成后会自动调用 loadCustomerContactLogs 并传递邮箱
		loadCustomerContactLogs(customerId, '')

		// 加载报价历史
		loadQuotationHistory(customerId)

		// ... existing code ...
	} catch (error) {
		// ... existing code ...
	}
}

const route = useRoute()
onMounted(async () => {
	// 获取路由参数
	const customerId = route.query.customerId
	if (customerId) {
		try {
			// 等待数据加载完成
			await GetCustomeInfoList(currentPage.value, pageSize.value);

			// 添加一个小延时确保数据已更新
			await nextTick();

			// 确保数据已加载
			let retryCount = 0;
			const maxRetries = 3;
			const checkAndOpenDialog = async () => {
				if (CunstomeinfotableData.value?.length > 0) {
					OpenDetailDialog(customerId);
				} else if (retryCount < maxRetries) {
					retryCount++;
					await new Promise(resolve => setTimeout(resolve, 500));
					await checkAndOpenDialog();
				} else {
					ElMessage.warning('加载客户数据超时，请刷新页面重试');
				}
			};

			await checkAndOpenDialog();
		} catch (error) {
			console.error('加载客户数据失败:', error);
			ElMessage.error('加载客户数据失败，请刷新页面重试');
		}
	}
});

//打开客户详情窗体并加载数据
const OpenDetailDialog = async (customerId) => {
	const customerInfo = CunstomeinfotableData.value.find(item => item.id == customerId);
	selectCustomerID.value = customerId;
	clearUploadfile();
	CustomerProfileDetailDialogform.id = customerId;
	CustomerProfileDetailDialogform.customerStatus = state.optionss['hr_customer_status'].filter(item => item.dictLabel == customerInfo.customerStatus).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictLabel == customerInfo.customerLevel).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.customerNo = customerInfo.customerNo;
	CustomerProfileDetailDialogform.customerAbbreviation = customerInfo.customerAbbreviation;
	CustomerProfileDetailDialogform.customerName = customerInfo.customerName;
	CustomerProfileDetailDialogform.tradingCountry = state.optionss['hr_nation'].filter(item => item.dictLabel == customerInfo.tradingCountry).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.compantWebsite = customerInfo.compantWebsite;
	CustomerProfileDetailDialogform.customerSource = state.optionss['sys_customer_source'].filter(item => item.dictLabel == customerInfo.customerSource).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.businessScope = state.optionss['hr_business_scope'].filter(item => item.dictLabel == customerInfo.businessScope).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.pricingTerm = state.optionss['hr_pricing_term'].filter(item => item.dictValue == customerInfo.pricingTerm).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.settlementWay = state.optionss['hr_settlement_way'].filter(item => item.dictValue == customerInfo.settlementWay).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.collectionPeriod = customerInfo.collectionPeriod;
	CustomerProfileDetailDialogform.customerTaxNumber = customerInfo.customerTaxNumber;
	CustomerProfileDetailDialogform.salesPerson = state.optionss['sql_hr_sale'].filter(item => item.dictLabel == customerInfo.salesPerson).map(item => item.dictValue).values().next().value;
	CustomerProfileDetailDialogform.address1 = customerInfo.address1;
	if (customerInfo.customerPhoto != null && customerInfo.customerPhoto != '') {
		//NewCustomerleadsform.compantPhotoStr = row.compantPhotoStr;
		customerInfo.customerPhoto.split(',').forEach((url, index) => {
			if (!fileList.value.some(item => item.url === url)) {
				let name = url.split('/').pop();
				fileList.value.push({
					name: name,
					url: url,
					isChanged: false,
					uid: Date.now() + Math.random(),
					status: 'success'
				});
			}
		});
	}
	uploadedFiles.value = fileList.value;
	//加载客户联系人
	loadCustomerContractPerson(customerInfo.id);
	//加载报价记录
	loadQuotationHistory(customerInfo.id);
	//加载销售合同记录
	loadContractHistory(customerInfo.id);
	//加载收寄样历史
	loadCustomerSendSampleHistory(customerInfo.id);
	CustomerProfileDetailDialog.value = true;
}

// 上传客户图片
const uploadFiles = async () => {
	try {
		// 重置文件URL字符串
		filelistUrlStr.value = '';

		// 1. 收集已存在的图片URL（从服务器加载的图片）
		const existingUrls = fileList.value
			.filter(file => file.url && file.url.includes('/CustomerInfo/CustomerInfoPhoto/'))
			.map(file => file.url);

		// 2. 只处理新上传的图片
		const newFiles = fileList.value.filter(file => file.raw);

		// 3. 上传新图片
		if (newFiles.length > 0) {
			const uploadPromises = newFiles.map(file => {
				const formData = new FormData();
				formData.append('FileName', file.name);
				formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
				formData.append('FileNameType', '1');
				formData.append('File', file.raw);
				formData.append('storeType', '1');
				return request.postForm(UploadUrl, formData);
			});

			const responses = await Promise.all(uploadPromises);
			const newUrls = responses
				.filter(response => response?.data?.url)
				.map(response => response.data.url);

			// 4. 合并所有URL（已存在的和新上传的）
			filelistUrlStr.value = [...existingUrls, ...newUrls].filter(Boolean).join(',');
		} else {
			// 如果没有新图片，只使用已存在的URL
			filelistUrlStr.value = existingUrls.join(',');
		}

		// 5. 更新表单中的图片字符串
		CustomerProfileform.customerPhoto = filelistUrlStr.value;

	} catch (error) {
		console.error('上传文件出错：', error);
		ElMessage({
			message: "上传文件出错！",
			type: 'error'
		});
		throw error;
	}
};

const SaveCustomerProfile = async (formEl: FormInstance | undefined) => {
	if (!formEl) return
	await formEl.validate(async (valid) => {
		if (valid) {
			try {
				// 检查是否有新上传的文件需要处理
				const hasNewFiles = fileList.value.some(file => file.raw && !file.url);

				if (hasNewFiles) {
					// 只处理新上传的文件
					const uploadPromises = fileList.value
						.filter(file => file.raw && !file.url)
						.map(file => {
							const formData = new FormData();
							formData.append('FileName', file.name);
							formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
							formData.append('FileNameType', '1');
							formData.append('File', file.raw);
							formData.append('storeType', '1');
							return request.postForm(UploadUrl, formData);
						});

					const responses = await Promise.all(uploadPromises);

					// 收集所有已存在的URL
					const existingUrls = fileList.value
						.filter(file => file.url)
						.map(file => file.url);

					// 添加新上传文件的URL
					responses.forEach(response => {
						if (response?.data?.url) {
							existingUrls.push(response.data.url);
						}
					});

					// 更新filelistUrlStr
					filelistUrlStr.value = existingUrls.join(',');
				}

				// 准备保存数据
				const saveData = {
					customerInfo: {
						...CustomerProfileform,
						customerPhoto: filelistUrlStr.value,
						IsDraft: 1
					},
					contactPeople: CustomerContactPersonTableData.value || []
				};

				// 保存草稿
				const response = await request.post(
					isEditCustomerInfo.value
						? 'CustomerInfoMation/EditCustomerInfo/Edit'
						: 'CustomerInfoMation/AddCustomerInfo/Add',
					isEditCustomerInfo.value
						? { ...saveData, customerInfo: { ...saveData.customerInfo, id: CustomerProfileDetailDialogform.id } }
						: saveData
				);

				if (response?.data) {
					ElMessage({
						message: '保存草稿成功！',
						type: 'success'
					});
					CloseCustomerProfileDetailDialog();
				} else {
					throw new Error(response?.msg || '保存草稿失败');
				}
			} catch (error) {
				console.error('保存草稿出错！😔错误内容：', error);
				ElMessage({
					message: error?.message || '保存草稿失败',
					type: 'error'
				});
			}
		} else {
		}
	})
}

const ContactLogTagData = ref([]);

// 控制字段显示的变量
const showBusinessOpportunityField = ref(false)
const businessOpportunityOptions = ref([])

const loadContactLogTagData = async () => {
	try {
		const response = await request({
			url: 'Email/GetUserEmailTagList/GetUserEmailTag',
			method: 'get'
		}) as ApiResponse<any>
		if (response && response.code === 200) {
			ContactLogTagData.value = response.data;
			console.log(ContactLogTagData.value)
		} else {
			ElMessage.error((response as any).msg || '获取联系日志失败')
		}
	} catch (error) {
		console.error('获取联系日志失败:', error)
	}
}

// 加载商机名称选项
const loadBusinessOpportunityOptions = async () => {
	try {
		const response = await request({
			url: 'BusinessOpportunity/GetBusinessOpportunityListByUser/GetBusinessOpportunityList',
			method: 'get'
		}) as ApiResponse<any>
		if (response && response.code === 200) {
			businessOpportunityOptions.value = response.data.map(item => ({
				dictValue: item.id,
				dictLabel: item.businessName
			}))
		} else {
			ElMessage.error((response as any).msg || '获取商机列表失败')
		}
	} catch (error) {
		console.error('获取商机列表失败:', error)
		ElMessage.error('获取商机列表失败')
	}
}

// 处理日志标签变化的方法
const handleContactLogTagChange = (tagId) => {
	// 重置所有字段显示状态
	showBusinessOpportunityField.value = false

	// 清空相关字段值
	contactLogForm.businessOpportunityName = ''

	// 如果表单引用存在，清除验证状态
	if (contactLogFormRef.value) {
		contactLogFormRef.value.clearValidate('businessOpportunityName')
	}

	// 根据选中的标签ID获取标签名称
	const selectedTag = ContactLogTagData.value.find(tag => tag.id === tagId)
	if (!selectedTag) return

	const tagName = selectedTag.emailTagName

	// 根据标签名称显示相应字段
	switch (tagName) {
		case '询盘':
			showBusinessOpportunityField.value = true
			// 询盘时商机名称是文本框
			break
		case '初次报价':
		case '再次报价':
			showBusinessOpportunityField.value = true
			// 加载商机名称选项（下拉框）
			loadBusinessOpportunityOptions()
			// 加载报价单号选项
			loadQuotationNumberOptions()
			// 关联单据锁定报价单
			contactLogForm.relatedDocumentType = 3 // 假设3代表报价单
			break
		case '沟通需求':
			showBusinessOpportunityField.value = true
			// 加载商机名称选项（下拉框）
			loadBusinessOpportunityOptions()
			break
		case '合同确定':
			showBusinessOpportunityField.value = true
			// 加载商机名称选项（下拉框）
			loadBusinessOpportunityOptions()
			// 关联单据锁定销售合同
			contactLogForm.relatedDocumentType = 1 // 1代表销售合同
			break
		case '默认标签':
		case '开发信':
			// 默认标签和开发信不显示商机字段
			showBusinessOpportunityField.value = false
			break
		default:
			// 其他标签类型默认不显示商机字段
			showBusinessOpportunityField.value = false
			break
	}
}

// 加载报价单号选项
const loadQuotationNumberOptions = async () => {
	try {
		const response = await request({
			url: 'Quotation/GetQuotaionListByUserAndCustomerID/GetQuotaionList',
			method: 'get',
			params: {
				customerId: selectCustomerID.value
			}
		}) as ApiResponse<any>
		if (response && response.code === 200) {
			quotationNumberOptions.value = response.data.map(item => ({
				dictValue: item.id,
				dictLabel: item.quotationNum
			}))
		} else {
			ElMessage.error((response as any).msg || '获取报价单号列表失败')
		}
	} catch (error) {
		console.error('获取报价单号列表失败:', error)
		ElMessage.error('获取报价单号列表失败')
	}
}

// 在 script setup 部分添加以下代码
const contactLogDetailDialogVisible = ref(false)
const selectedContactLog = ref<ContactLog>({} as ContactLog)

const handleContactLogRowDblClick = (row) => {
	selectedContactLog.value = row
	contactLogDetailDialogVisible.value = true
}

// 联系内容中的邮件链接：跳转邮件页面并打开该邮件
const handleContactContentClick = (row: ContactLog) => {
	if (row.logSouce === '邮件记录') {
		const emailID = row.emailID ?? row.relatedDocumentID
		if (emailID) {
			router.push({ path: '/email', query: { id: String(emailID) } })
		} else {
			ElMessage.warning('该邮件记录缺少邮件ID，无法打开')
		}
	} else {
		selectedContactLog.value = row
		contactLogDetailDialogVisible.value = true
	}
}

onMounted(() => {
	console.log('sql_sale_contracts:', state.optionss.sql_sale_contracts)
});

const router = useRouter()

const handleQuoteRowDblClick = (row) => {
	if (row.id) {
		router.push({
			name: 'customerquotation', // 请确保路由名称为QuoteDetail
			query: { id: row.id }
		})
	}
}

const handleSalesContractRowDblClick = (row) => {
	if (row.contractNumber) {
		try {
			router.push({
				path: 'sale/salecontract',
				query: {
					contractNumber: row.contractNumber,
					contractId: row.id,
					viewDetail: 'true'
				}
			}).catch(err => {
				console.error('Router navigation error:', err);
				window.location.href = `#/sale/salecontract?contractNumber=${row.contractNumber}&contractId=${row.id}&viewDetail=true`;
			});
		} catch (err) {
			console.error('Navigation error:', err);
			window.location.href = `#/sale/salecontract?contractNumber=${row.contractNumber}&contractId=${row.id}&viewDetail=true`;
		}
	}
}

const handleSampleRowDblClick = (row) => {
	if (row.id) {
		console.log('Navigating to productsamplecollection with id:', row.id);
		try {
			router.push({
				path: '/product/productsamplecollection',
				query: {
					id: row.id,
					viewDetail: 'true' // Add viewDetail parameter to trigger auto-opening of details
				}
			}).catch(err => {
				console.error('Router navigation error:', err);
				window.location.href = `#/product/productsamplecollection?id=${row.id}&viewDetail=true`;
			});
		} catch (err) {
			console.error('Navigation error:', err);
			window.location.href = `#/product/productsamplecollection?id=${row.id}&viewDetail=true`;
		}
	} else {
		ElMessage.warning('样品记录ID不存在，无法查看详情');
	}
}

// 删除客户资料
const DeleteCustomerProfile = (row) => {
	ElMessageBox.confirm('确定要删除该客户资料吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(() => {
		request({
			url: 'CustomerInfoMation/DeleteCustomerInfo/Delete',
			method: 'post',
			data: {
				CustomerID: row.id
			}
		}).then((response: any) => {
			if (response.code === 200) {
				ElMessage({
					message: '删除成功',
					type: 'success'
				});
				// 刷新列表
				GetCustomeInfoList(currentPage.value, pageSize.value);
			} else {
				ElMessage.error(response.msg || '删除失败');
			}
		}).catch(error => {
			console.error('删除客户资料失败:', error);
			ElMessage.error('删除失败，请稍后重试');
		});
	}).catch(() => {
		ElMessage({
			type: 'info',
			message: '已取消删除'
		});
	});
}

// 报价记录分页
const QuotationRecordCurrentPage = ref(1)
const QuotationRecordPageSize = ref(30)
const QuotationRecordTotalItems = ref(0)
const QuotationRecordHandlePageChange = async (newPage) => {
	QuotationRecordCurrentPage.value = newPage
	await loadQuotationHistory(selectCustomerID.value, newPage, QuotationRecordPageSize.value)
}
const QuotationRecordHandleSizeChange = async (size) => {
	QuotationRecordPageSize.value = size
	QuotationRecordCurrentPage.value = 1
	await loadQuotationHistory(selectCustomerID.value, 1, size)
}

// 销售合同记录分页
const SalesContractRecordCurrentPage = ref(1)
const SalesContractRecordPageSize = ref(30)
const SalesContractRecordTotalItems = ref(0)
const SalesContractRecordHandlePageChange = async (newPage) => {
	SalesContractRecordCurrentPage.value = newPage
	await loadContractHistory(selectCustomerID.value, newPage, SalesContractRecordPageSize.value)
}
const SalesContractRecordHandleSizeChange = async (size) => {
	SalesContractRecordPageSize.value = size
	SalesContractRecordCurrentPage.value = 1
	await loadContractHistory(selectCustomerID.value, 1, size)
}

// 收寄样历史分页
const CustomerSendSampleCurrentPage = ref(1)
const CustomerSendSamplePageSize = ref(30)
const CustomerSendSampleTotalItems = ref(0)
const CustomerSendSampleHandlePageChange = async (newPage) => {
	CustomerSendSampleCurrentPage.value = newPage
	await loadCustomerSendSampleHistory(selectCustomerID.value, newPage, CustomerSendSamplePageSize.value)
}
const CustomerSendSampleHandleSizeChange = async (size) => {
	CustomerSendSamplePageSize.value = size
	CustomerSendSampleCurrentPage.value = 1
	await loadCustomerSendSampleHistory(selectCustomerID.value, 1, size)
}
</script>

<style scoped>
/* 搜索区：缩小行间距 */
.customer-search-area .search-row {
	margin-bottom: 4px !important;
}

.customer-search-area .search-row:last-child {
	margin-bottom: 0 !important;
}

/* 列表表头高度减半（默认约 12px 上下内边距改为 6px） */
:deep(.el-table th.el-table__cell) {
	padding-top: 6px !important;
	padding-bottom: 6px !important;
	line-height: 1.2;
}

/* 列表表格：缩小行间距与单元格内行高（含客户简称等多行内容） */
.customer-info-table :deep(.el-table__body td.el-table__cell),
.customer-info-table :deep(.el-table__body .cell) {
	line-height: 1.25;
	padding-top: 1px !important;
	padding-bottom: 1px !important;
}

/* 客户信息页面dialog中的表单组件间距减少一半，与销售合同页面保持一致 */
.el-dialog .el-form-item {
	margin-bottom: 5px !important;
}

.el-dialog .el-row {
	margin-bottom: 2.5px !important;
}
</style>
