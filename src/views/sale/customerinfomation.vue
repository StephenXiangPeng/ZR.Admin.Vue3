<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click=openCustomerProfileDialog()>客户建档</el-button>
		<el-button type="primary" @click="CustomerDuplicationCheckDialog = true">客户查重</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<el-divider></el-divider>
		<div style="width: 100%; margin-top: 30px;">
			<!-- <el-input v-model="Search_CustomerNo_input" clearable style="width: 15%"
				placeholder="输入客户编号" size="default" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; -->
			<el-input v-model="Search_CustomerEmail_input" clearable style="width: 15%" placeholder="输入邮箱地址"
				size="default" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Search_CustomerName_input" placeholder="选择客户名称" style="width: 15%;" filterable
				size="default">
				<el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="Search_CustomerContactPerson_input" clearable style="width: 15%" placeholder="输入联系人"
				size="default" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Search_BusinessScope_Select" placeholder="选择业务范围" style="width: 15%;" filterable
				size="default">
				<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		</div>
		<div style="width: 100%; margin-top: 5px;">
			<el-select v-model="Search_TradingCountry_Select" class="m-2" placeholder="选择国家" size="default"
				style="width: 15%;" filterable>
				<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" size="default"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="Search_CustomerLevel_Select" class="m-2" placeholder="选择客户等级" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" size="default"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="Search_CustomerSource_Select" class="m-2" placeholder="选择客户来源" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue" size="default"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="Search_StartDate_Select" type="date" placeholder="起始日期" size="default"
				style="width: 15%;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="Search_EndDate_Select" type="date" placeholder="结束日期" size="default"
				style="width: 15%;" />
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" @click="SearchSunmit" plain>查询</el-button>
				<el-button @click="resetSearch">重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;客户信息表</span>
		</div>
		<el-divider></el-divider>
		<el-table :data="CunstomeinfotableData" @row-dblclick="CunstomeinfotableDatahandleRowDblClick">
			<el-table-column prop="customerNo" label="客户编号" style="width: 8%;">
				<template #default="scope">
					<span>{{ scope.row.customerNo }}</span>
					<el-tag v-if="scope.row.isDraft" type="warning" style="margin-left: 5px;" size="small">草稿</el-tag>
				</template>
			</el-table-column>
			<el-table-column prop="customerStatus" label="客户状态" style="width: 8%;" />
			<el-table-column prop="customerLevel" label="客户等级" style="width: 8%;" />
			<el-table-column prop="customerAbbreviation" label="客户简称" style="width: 8%;" />
			<el-table-column prop="tradingCountry" label="贸易国别" style="width: 8%;" />
			<el-table-column prop="Lastcontactdate" label="最近联系时间" style="width: 8%;" />
			<el-table-column prop="create_time" label="建档时间" style="width: 8%;" />
			<el-table-column prop="salesPerson" label="所属销售员" style="width: 8%;" />
			<el-table-column prop="Originalfollower" label="原跟进人" style="width: 8%;" />
			<el-table-column prop="create_by" label="创建人" style="width: 8%;" />
			<el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;">
				<template v-slot:default="scope">
					<el-button link type="primary" size="small"
						@click=OpenCustomerProfileDetailDialog(scope.row)>查看详情</el-button>
				</template>
			</el-table-column>
		</el-table>
		<el-pagination @current-change="handlePageChange" :current-page="currentPage" :page-size="pageSize"
			:total="totalItems" background layout="prev, pager, next" style="margin-top: 5px;" />
		<el-dialog v-model="CustomerProfileDialog" title="客户建档" :close-on-click-modal=false style="width: 70%;"
			@close="resetCustomerProfileDialog()">
			<span style="font-size: 20px; font-weight: bold;">客户基本信息</span>
			<el-divider></el-divider>
			<el-button type="primary" :icon="Plus" size="default" @click="OpenLeadImportDialog"
				style="margin-bottom: 1.5%;">从线索导入</el-button>
			<el-form ref="CustomerProfileformRef" :rules="rules" :model="CustomerProfileform" :inline="true"
				label-width="auto">
				<el-form-item label="客户状态：">
					<el-select filterable v-model="CustomerProfileform.customerStatus" placeholder="选择客户状态"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_customer_status" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户等级：">
					<el-select filterable v-model="CustomerProfileform.customerLevel" placeholder="自动评级" disabled
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户编号：">
					<el-input v-model="CustomerProfileform.customerNo" disabled placeholder="自动编码"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户简称：" prop="customerAbbreviation">
					<el-input v-model="CustomerProfileform.customerAbbreviation" placeholder="输入客户简称"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户名称：" prop="customerName">
					<el-input v-model="CustomerProfileform.customerName" placeholder="输入客户名称" style="width: 300px;" />
				</el-form-item>
				<el-form-item label="贸易国别：" prop="tradingCountry">
					<el-select filterable v-model="CustomerProfileform.tradingCountry" placeholder="选择贸易国别"
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="公司主页：">
					<el-input v-model="CustomerProfileform.compantWebsite" placeholder="输入公司主页地址"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户来源：" prop="customerSource">
					<el-select filterable v-model="CustomerProfileform.customerSource" placeholder="选择客户来源"
						style="width: 300px;">
						<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="业务范围：" prop="businessScope">
					<el-select filterable v-model="CustomerProfileform.businessScope" placeholder="选择业务范围"
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="价格条款：">
					<el-select filterable v-model="CustomerProfileform.pricingTerm" placeholder="选择价格条款"
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="结汇方式：">
					<el-select filterable v-model="CustomerProfileform.settlementWay" placeholder="选择结汇方式"
						style="width: 300px;">
						<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="收汇期限：">
					<el-input v-model="CustomerProfileform.collectionPeriod" placeholder="输入收汇期限"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户税号：">
					<el-input v-model="CustomerProfileform.customerTaxNumber" placeholder="输入客户税号"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="销售人员：" prop="salesPerson">
					<el-select filterable v-model="CustomerProfileform.salesPerson" placeholder="选择销售员"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item><br>
				<el-form-item style=" width: 93.5%;" label="详细地址：" prop="address1">
					<el-input type="textarea" v-model="CustomerProfileform.address1" placeholder="输入详细地址" rows="5"
						style="width: 100%;" />
				</el-form-item>
				<el-form-item label="客户照片：">
					<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList" limit="3"
						:disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl" :data="formData">
						<el-icon>
							<Plus />
						</el-icon>

						<template #file="{ file }">
							<div>
								<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
								<span class="el-upload-list__item-actions">
									<span class="el-upload-list__item-preview" @click="handlePictureCardPreview(file)">
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
			</el-form>
			<span style="font-size: 20px; font-weight: bold;">联系人信息</span>
			<el-divider></el-divider>
			<el-button type="primary" :icon="Plus" size="default" @click="AddContactPersonTableRow">新增联系人</el-button>
			<el-table :data="CustomerContactPersonTableData" style="width: 100%; ">
				<el-table-column prop="name" label="联系人姓名" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.name" placeholder="输入联系人姓名" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="sex" label="性别" width="150" align="left">
					<template #default="{ row }">
						<el-select filterable v-model="row.sex" placeholder="请选择性别" size="default">
							<el-option v-for="dict in optionss.sys_user_sex" :key="dict.dictCode"
								:label="dict.dictLabel" :value="dict.dictValue" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="position" label="职务" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.position" placeholder="输入职务" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="telePhone" label="电话" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.telePhone" placeholder="输入电话号码" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="cellPhone" label="手机" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.cellPhone" placeholder="输入手机号码" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="fax" label="传真" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.fax" placeholder="输入传真" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="email" label="电子邮件" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.email" type="email" placeholder="输入电子邮件" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column label="操作" width="180" align="center" fixed="right">
					<template #default="scope">
						<el-button type="text" size="default"
							@click="RemoveContactPersonTableRow(scope.$index)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>

			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="SaveCustomerDraft(CustomerProfileformRef)">
						保存草稿
					</el-button>
					<el-button type="success" @click="submitForm(CustomerProfileformRef)">
						提交
					</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="LeadImportDialog" title="线索导入" :close-on-click-modal=false style="width: 50%;">
			<el-table :data="CustomerLeadsTableData" @row-dblclick="handleRowDblClick" align="center">
				<el-table-column fixed prop="id" label="线索编号" style="width: 12%;" />
				<el-table-column prop="contactEmail" label="邮箱" style="width: 12%;" />
				<el-table-column prop="customerName" label="客户名称" style="width: 12%;" />
				<el-table-column prop="customerLevel" label="客户等级" style="width: 12%;" />
				<el-table-column prop="customerNation" label="国家" style="width: 12%;" />
				<el-table-column prop="contactName" label="联系人" style="width: 12%;" />
				<el-table-column prop="customerSource" label="来源" style="width: 12%;" />
				<el-table-column prop="involvingBusiness" label="涉及业务" style="width: 12%;" />
				<el-table-column prop="create_by" label="创建人" style="width: 12%;" />
			</el-table>
			<el-pagination @current-change="CustomerLeadsTableDatahandlePageChange"
				:current-page="CustomerLeadsTableDatacurrentPage" :page-size="CustomerLeadsTableDatapageSize"
				:total="CustomerLeadsTableDatatotalItems" background layout="prev, pager, next"
				style="margin-top: 5px;" />
		</el-dialog>
		<el-dialog v-model="CustomerDuplicationCheckDialog" title="客户查重" :close-on-click-modal=false
			style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">查重字段</span>
			<el-divider></el-divider>
			<el-form :model="CustomerDuplicationCheckform" inline=true label-position="left" size="default"
				style="margin-top: 15px;">
				<el-form-item label="客户简称：" prop="Cabbreviation">
					<el-input v-model="CustomerDuplicationCheckform.Cabbreviation" placeholder="输入客户简称"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户名称：" prop="Cname">
					<el-input v-model="CustomerDuplicationCheckform.Cname" placeholder="输入客户名称" style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户地址：" prop="Caddress">
					<el-input v-model="CustomerDuplicationCheckform.Caddress" placeholder="输入客户地址"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户邮箱：" prop="Cemail">
					<el-input v-model="CustomerDuplicationCheckform.Cemail" placeholder="输入客户邮箱"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="电话号码：" prop="Ctellphone">
					<el-input v-model="CustomerDuplicationCheckform.Ctellphone" placeholder="输入电话号码"
						style="width: 300px;" />
				</el-form-item>
				<el-row class="mb-4">
					<el-button type="primary" plain @click="DuplicationCheckSearch">查询</el-button>
					<el-button @click="DuplicationCheckReset">重置</el-button>
				</el-row>
			</el-form>
			<div style="margin-top: 15px;">
				<span style="font-size: 20px; font-weight: bold; ">查重结果</span>
			</div>
			<el-divider></el-divider>
			<el-table :data="CustomerDuplicationCheckData">
				<el-table-column prop="customerNo" label="客户编号" style="width: 8%;" />
				<el-table-column prop="customerStatus" label="客户状态" style="width: 8%;" />
				<el-table-column prop="customerLevel" label="客户等级" style="width: 8%;" />
				<el-table-column prop="customerAbbreviation" label="客户简称" style="width: 8%;" />
				<el-table-column prop="tradingCountry" label="贸易国别" style="width: 8%;" />
				<el-table-column prop="Lastcontactdate" label="最近联系时间" style="width: 8%;" />
				<el-table-column prop="create_time" label="建档时间" style="width: 8%;" />
				<el-table-column prop="salesName" label="所属销售员" style="width: 8%;" />
				<el-table-column prop="Originalfollower" label="原跟进人" style="width: 8%;" />
				<el-table-column prop="create_by" label="创建人" style="width: 8%;" />
			</el-table>
		</el-dialog>
		<el-dialog v-model="CustomerProfileDetailDialog" title="客户详情" :close-on-click-modal=false style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">客户基本信息</span>
			<el-divider></el-divider>
			<el-form :model="CustomerProfileDetailDialogform" inline=true label-position="left" size="default">
				<el-form-item label="客户状态：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.customerStatus" placeholder="选择客户状态"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_customer_status" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户等级：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.customerLevel" placeholder="选择客户等级"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="客户编号：">
					<el-input v-model="CustomerProfileDetailDialogform.customerNo" disabled placeholder="自动编码"
						style="width: 300px;" />
				</el-form-item>
				<el-form-item label="客户简称：">
					<el-input v-model="CustomerProfileDetailDialogform.customerAbbreviation" placeholder="输入客户简称"
						style="width: 300px;" disabled />
				</el-form-item>
				<el-form-item label="客户名称：">
					<el-input v-model="CustomerProfileDetailDialogform.customerName" placeholder="输入客户名称"
						style="width: 300px;" disabled />
				</el-form-item>
				<el-form-item label="贸易国别：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.tradingCountry" placeholder="选择贸易国别"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="公司主页：">
					<el-input v-model="CustomerProfileDetailDialogform.compantWebsite" placeholder="输入公司主页地址"
						style="width: 300px;" disabled />
				</el-form-item>
				<el-form-item label="客户来源：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.customerSource" disabled class="m-2"
						placeholder="选择客户来源" size="default" style="width: 300px;">
						<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="业务范围：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.businessScope" disabled
						placeholder="选择业务范围" style="width: 300px;">
						<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="价格条款：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.pricingTerm" placeholder="选择价格条款"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="结汇方式：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.settlementWay" placeholder="选择结汇方式"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode"
							:label="dict.dictLabel" :value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item label="收汇期限：">
					<el-input v-model="CustomerProfileDetailDialogform.collectionPeriod" placeholder="输入收汇期限"
						style="width: 300px;" disabled />
				</el-form-item>
				<el-form-item label="客户税号：">
					<el-input v-model="CustomerProfileDetailDialogform.customerTaxNumber" placeholder="输入客户税号"
						style="width: 300px;" disabled />
				</el-form-item>
				<el-form-item label="销售人员：">
					<el-select filterable v-model="CustomerProfileDetailDialogform.salesPerson" placeholder="选择销售员"
						style="width: 300px;" disabled>
						<el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue"></el-option>
					</el-select>
				</el-form-item><br>
				<el-form-item label="详细地址：" style="width: 93.5%;">
					<el-input type="textarea" v-model="CustomerProfileDetailDialogform.address1" placeholder="输入详细地址"
						rows="5" style="width: 100%;" disabled />
				</el-form-item>
				<el-form-item label="客户照片：">
					<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="fileList" limit="3"
						:disabled="fileList.length >= 3" @change="handleChange" :action="UploadUrl" :data="formData">
						<el-icon>
							<Plus />
						</el-icon>
						<template #file="{ file }">
							<div>
								<img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
								<span class="el-upload-list__item-actions">
									<span class="el-upload-list__item-preview" @click="handlePictureCardPreview(file)">
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
			</el-form>
			<el-tabs v-model="activeTab" tab-position="top" style="height: 350px; " class="demo-tabs">
				<el-tab-pane label="联系人信息" name="ContactInfoTable">
					<el-table :data="ContactPersonData">
						<el-table-column prop="name" label="姓名" style="width: 8%;" />
						<el-table-column prop="sexText" label="性别" style="width: 8%;" />
						<el-table-column prop="position" label="职务" style="width: 8%;" />
						<el-table-column prop="telePhone" label="电话" style="width: 8%;" />
						<el-table-column prop="cellPhone" label="手机" style="width: 8%;" />
						<el-table-column prop="fax" label="传真" style="width: 8%;" />
						<el-table-column prop="email" label="邮件" style="width: 8%;" />
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="联系日志" name="ContactLogTable">
					<el-button type="primary" @click="AddContactLog">添加联系日志</el-button>
					<el-table :data="ContactLogData" height="200" style="width: 100%">
						<el-table-column prop="conactTag" label="日志标签" width="130" />
						<el-table-column prop="emailDate" label="联系日期" width="130" />
						<el-table-column prop="logSouce" label="来源" width="150" />
						<el-table-column prop="contact" label="联系人" width="180" />
						<el-table-column prop="ourPersonnel" label="我方人员" width="180" />
						<el-table-column prop="contactDetails" label="联系内容" show-overflow-tooltip />
						<el-table-column label="图片" width="100">
							<template #default="{ row }">
								<el-image v-if="row.images && row.images.length > 0" style="width: 50px; height: 50px;"
									:src="row.images.split(',')[0]" :preview-src-list="row.images.split(',')"
									fit="cover" :preview-teleported="true" :initial-index="0" />
								<span v-else>无图片</span>
							</template>
						</el-table-column>
						<el-table-column label="附件" width="100">
							<template #default="{ row }">
								<el-button v-if="row.attachments && row.attachments.length > 0" type="primary" link
									@click="downloadAttachment(row.attachments)">
									下载附件
								</el-button>
								<span v-else>无附件</span>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination @current-change="ContactLogTablehandlePageChange"
						:current-page="ContactLogTablecurrentPage" :page-size="ContactLogTablepageSize"
						:total="ContactLogTabletotalItems" background layout="prev, pager, next"
						style="margin-top: 5px;" />
				</el-tab-pane>
				<el-tab-pane label="报价记录" name="QuoteRecordTable">
					<el-table :data="QuotationRecordData">
						<el-table-column prop="quotationNum" label="报价单号" style="width: 8%;" />
						<el-table-column prop="realQuotationDate" label="报价日期" style="width: 8%;" />
						<el-table-column prop="validityPeriod" label="有效期" style="width: 8%;" />
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="销售记录" name="saleRecordTable">
					<el-table :data="SalesContractRecordData">
						<el-table-column prop="contractNumber" label="销售合同" style="width: 8%;" />
						<el-table-column prop="contractDate" label="合同日期" style="width: 8%;" />
						<el-table-column prop="effectiveDate" label="生效日期" style="width: 8%;" />
						<el-table-column prop="deliveryDate" label="交货日期" style="width: 8%;" />
						<el-table-column prop="goodsValue" label="货值合计" style="width: 8%;" />``
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="收寄样历史" name="SampleCollectionHistory">
					<el-table :data="CustomerSendSampleData" style="width: 100%">
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
					</el-table></el-tab-pane>
				<el-tab-pane label="出货记录" name="ShippingRecordTable">
					<el-table :data="ShipRecoreData">
						<el-table-column prop="Cnum" label="发票号码" style="width: 8%;" />
						<el-table-column prop="Cstate" label="出运日期" style="width: 8%;" />
						<el-table-column prop="Cgrade" label="销售合同" style="width: 8%;" />
						<el-table-column prop="Cabbreviation" label="产品编号" style="width: 8%;" />
						<el-table-column prop="Ctradingcountry" label="客户货号" style="width: 8%;" />
						<el-table-column prop="Lastcontactdate" label="中文品名" style="width: 8%;" />
						<el-table-column prop="Archivecreationdate" label="出货数量" style="width: 8%;" />
						<el-table-column prop="CSeller" label="计量单位" style="width: 8%;" />
						<el-table-column prop="Originalfollower" label="外销单价" style="width: 8%;" />
						<el-table-column prop="Founder" label="外销总价" style="width: 8%;" />
					</el-table>
				</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="warning" @click="EditCustomerInfoClick">编辑</el-button>
					<!-- <el-button type="primary">保存</el-button> -->
				</span>
			</template>
		</el-dialog>
		<!-- 添加联系日志对话框 -->
		<el-dialog v-model="contactLogDialogVisible" title="添加联系日志" width="600px" :close-on-click-modal="false">
			<el-form ref="contactLogFormRef" :model="contactLogForm" :rules="contactLogRules" label-width="100px">
				<el-form-item label="联系日期" prop="contactDate">
					<el-date-picker v-model="contactLogForm.contactDate" type="date" placeholder="选择联系日期"
						style="width: 100%;" />
				</el-form-item>
				<el-form-item label="日志标签" prop="ContactLogTag">
					<el-select v-model="contactLogForm.ContactLogTag" filterable placeholder="选择日志标签"
						style="width: 100%;">
						<el-option v-for="item in ContactLogTagData" :key="item.id" :label="item.emailTagName"
							:value="item.id" />
					</el-select>
				</el-form-item>
				<el-form-item label="联系人" prop="contactPerson">
					<el-select v-model="contactLogForm.contactPerson" filterable placeholder="选择联系人"
						style="width: 100%;">
						<el-option v-for="item in ContactPersonData" :key="item.id" :label="item.name"
							:value="item.id" />
					</el-select>
				</el-form-item>
				<el-form-item label="我方人员" prop="ourStaff">
					<el-select v-model="contactLogForm.ourStaff" filterable placeholder="选择我方人员" style="width: 100%;">
						<el-option v-for="item in state.optionss.sql_hr_sale" :key="item.dictValue"
							:label="item.dictLabel" :value="item.dictValue" />
					</el-select>
				</el-form-item>
				<el-form-item label="联系内容" prop="contactContent">
					<el-input v-model="contactLogForm.contactContent" type="textarea" :rows="4" placeholder="请输入联系内容" />
				</el-form-item>
				<el-form-item label="图片">
					<el-upload list-type="picture-card" :auto-upload="false" v-model:file-list="contactLogImages"
						:action="UploadUrl" accept="image/*" @preview="handleImagePreview">
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
						style="width: 100%">
						<el-button type="primary">选择文件</el-button>
						<template #tip>
							<div class="el-upload__tip">可上传任意类型文件</div>
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
	</div>
</template>

<script setup lang="ts">
import { getCurrentInstance, reactive, toRefs, ref, onMounted, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessageBox, UploadUserFile, ElMessage, UploadFile, FormInstance, FormRules } from 'element-plus'
import request from '@/utils/request';
import useUserStore from "@/store/modules/user";
import { number } from 'echarts';
import { fa } from 'element-plus/es/locale';
import { start } from 'nprogress';

const activeTab = ref('ContactInfoTable');
const isEditCustomerInfo = ref(false);
const proxy = getCurrentInstance().proxy
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
		sql_user_customers: []
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
	{ dictType: 'hr_ourcompany' }
]
proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((element) => {
		state.optionss[element.dictType] = element.list
	})
	//获取客户基本信息列表
	GetCustomeInfoList(currentPage.value, pageSize.value);
})

// 获取用户相关的客户数据
const getUserCustomerData = async () => {
	try {
		const response = await request({
			url: 'CustomerInfoMation/GetCustomerAllNameDataByUserId/GetSelectCustomerAllNameData',
			method: 'get'
		})
		if (response.code === 200) {
			state.optionss.sql_user_customers = response.data.map(item => ({
				dictValue: item.dictValue,
				dictLabel: item.dictLabel
			}))
		} else {
			ElMessage.error(response.msg || '获取客户数据失败')
		}
	} catch (error) {
		ElMessage.error('获取客户数据失败')
	}
}
getUserCustomerData();//获取用户相关的客户数据

//线索导入窗体
const LeadImportDialog = ref(false)
const OpenLeadImportDialog = () => {
	LeadImportDialog.value = true
	GetCustomeleadList(CustomerLeadsTableDatacurrentPage.value, CustomerLeadsTableDatapageSize.value);
}
//线索表格分页组件
const CustomerLeadsTableDatatotalItems = ref(0);
const CustomerLeadsTableDatacurrentPage = ref(1);
const CustomerLeadsTableDatapageSize = ref(10);
//线索信息表格
const CustomerLeadsTableData = ref([])
const CustomerLeadsTableDatahandlePageChange = async (newPage) => {
	// Fetch new data based on the new page and update currentTableData
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetCustomeleadList(start, end);  // Assume fetchData is a function that fetches data from the server
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
					url: url
				});
			}
		});
	}
	uploadedFiles.value = fileList.value;
	LeadImportDialog.value = false;
}

//客户建档窗体
const CustomerProfileDialog = ref(false)

const openCustomerProfileDialog = () => {
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
const fileList = ref<UploadUserFile[]>([])
const uploadedFiles = ref([]);  // 用于存储已上传的文件
// 删除客户图片
const handleRemove = (file: UploadFile) => {
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

// 检查上传客户图片数量
const handleChange = (file, fileList) => {
	// 先检查文件数量限制
	if (fileList.length > 3) {
		ElMessage({
			type: 'info',
			message: '最多上传3张图片！'
		});
		fileList.splice(3); // 保留前三个文件，移除其余文件
		return; // 不再继续执行后面的代码
	}

	// 检查是否是新文件
	if (file.raw) {
		const isDuplicate = fileList.value.some(f =>
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
			// 为新文件创建预览URL
			file.url = URL.createObjectURL(file.raw);
		}
	}
};

const handlePictureCardPreview = (file: UploadFile) => {
	dialogImageUrl.value = file.url!
	dialogVisible.value = true
}

const EditCustomerInfoClick = () => {
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
					isChanged: false
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
	await formEl.validate((valid, fields) => {
		if (valid) {
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
			//#region 保存客户资料
			ElMessageBox.confirm('确定保存该客户资料吗？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning'
			}).then(() => {
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
					if (isImport.value == false) {
						//上传客户图片
						const uploadPromises = fileList.value.map(file => {
							const formData = new FormData();
							formData.append('FileName', file.name);
							formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
							formData.append('FileNameType', '1');
							formData.append('File', file.raw);
							formData.append('storeType', '1');
							// 返回上传文件的 Promise
							return request.postForm(UploadUrl, formData);
						});
						Promise.all(uploadPromises).then(responses => {
							responses.forEach((response, index) => {
								if (response != null) {
									filelistUrlStr.value += (index > 0 ? ',' : '') + response.data.url;
								} else {
									ElMessage({
										message: "上传客户图片出错！😔",
										type: 'error'
									})
								}
							});
							addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
							addCustomerInfo.customerInfo = CustomerProfileform;
							//保存新建客户资料
							CustomerProfileform.customerPhoto = filelistUrlStr.value;
							CustomerProfileform.IsDraft = 0;
							request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then(response => {
								if (response != null) {
									ElMessage({
										message: response.msg,
										type: 'success'
									})
									//清空上传图片
									filelistUrlStr.value = '';
									CloseCustomerProfileDetailDialog();
								} else {
									console.error('保存客户资料出错');
								}
							}).catch(error => {
								console.error('保存客户资料出错！😔错误内容：', error);
							})
						}).catch(error => {
							console.error('上传公司图片出错！😔错误内容：', error);
						});
					} else {
						addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
						addCustomerInfo.customerInfo = CustomerProfileform;
						//保存导入客户资料
						request.post('CustomerInfoMation/AddCustomerInfo/Add', addCustomerInfo).then(response => {
							if (response != null) {
								ElMessage({
									message: response.msg,
									type: 'success'
								})
								//清空上传图片
								isImport.value = false;
								filelistUrlStr.value = '';
								CloseCustomerProfileDetailDialog();
							} else {
								console.error('保存客户资料出错');
							}
						}).catch(error => {
							console.error('保存客户资料出错！😔错误内容：', error);
						})
					}
				} else {
					//上传客户图片
					const uploadPromises = fileList.value.filter(file => file.isChanged).map(file => {
						const formData = new FormData();
						formData.append('FileName', file.name);
						formData.append('FileDir', 'CustomerInfo/CustomerInfoPhoto');
						formData.append('FileNameType', '1');
						formData.append('File', file.raw);
						formData.append('storeType', '1');
						// 返回上传文件的 Promise
						return request.postForm(UploadUrl, formData);
					});
					// 先将未更改的文件URL添加到filelistUrlStr
					fileList.value.filter(file => !file.isChanged).forEach(file => {
						if (filelistUrlStr.value) {
							filelistUrlStr.value += ',';
						}
						filelistUrlStr.value += file.url; // 假设未更改文件已有URL属性
					});
					Promise.all(uploadPromises).then(responses => {
						responses.forEach((response, index) => {
							if (response != null) {
								if (filelistUrlStr.value) {
									filelistUrlStr.value += ',';
								}
								filelistUrlStr.value += response.data.url;
							} else {
								ElMessage({
									message: "上传客户图片出错！😔",
									type: 'error'
								})
							}
						});
						addCustomerInfo.contactPeople = CustomerContactPersonTableData.value;
						CustomerProfileform.customerPhoto = filelistUrlStr.value;
						addCustomerInfo.customerInfo = CustomerProfileform;
						addCustomerInfo.customerInfo.id = CustomerProfileDetailDialogform.id;
						let collectionPeriodValue = CustomerProfileform.collectionPeriod;
						if (typeof collectionPeriodValue === 'string' && CustomerProfileform.collectionPeriod.toString().trim() === '') {
							collectionPeriodValue = 0; // 或者设置一个合理的默认值
						} else {
							collectionPeriodValue = Number(collectionPeriodValue); // 确保它被转换为数字
						}
						addCustomerInfo.customerInfo.collectionPeriod = collectionPeriodValue;
						addCustomerInfo.customerInfo.IsDraft = 0;
						//编辑保存客户资料
						request.post('CustomerInfoMation/EditCustomerInfo/Edit', addCustomerInfo).then(response => {
							if (response != null) {
								ElMessage({
									message: response.msg,
									type: 'success'
								})
								//清空上传图片
								filelistUrlStr.value = '';
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
						console.error('上传公司图片出错！😔错误内容：', error);
					});
				}
				//关闭建档窗体
			}).catch(() => {
				ElMessage({
					type: 'info',
					message: '已取消保存'
				})
			})
			//#endregion 
		} else {
			console.log('error submit!', fields)
		}
	})
}

const handleDownload = (file: UploadFile) => {
	console.log(file)
}

//分页组件
const totalItems = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
//客户基本信息表格
const CunstomeinfotableData = ref([])
const handlePageChange = async (newPage) => {
	// Fetch new data based on the new page and update currentTableData
	currentPage.value = newPage;
	const start = newPage;
	const end = pageSize.value;
	const newData = await GetCustomeInfoList(start, end);  // Assume fetchData is a function that fetches data from the server
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
					item.customerStatus = state.optionss['hr_customer_status'].filter(option => option.dictValue == item.customerStatus).map(option => option.dictLabel).values().next().value;
					item.customerLevel = state.optionss['hr_customer_level'].filter(option => option.dictValue == item.customerLevel).map(option => option.dictLabel).values().next().value;
					item.tradingCountry = state.optionss['hr_nation'].filter(option => option.dictValue == item.tradingCountry).map(option => option.dictLabel).values().next().value;
					item.customerSource = state.optionss['sys_customer_source'].filter(option => option.dictValue == item.customerSource).map(option => option.dictLabel).values().next().value;
					item.businessScope = state.optionss['hr_business_scope'].filter(option => option.dictValue == item.businessScope).map(option => option.dictLabel).values().next().value;
					item.salesPerson = state.optionss['sql_hr_sale'].filter(option => option.dictValue == item.salesPerson).map(option => option.dictLabel).values().next().value;
					item.create_by = state.optionss['sql_all_user'].filter(option => option.dictValue == item.create_by).map(option => option.dictLabel).values().next().value;
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
					isChanged: false
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
	//加载日志标签
	loadContactLogTagData();
	CustomerProfileDetailDialog.value = true;
}

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
				//加载联系日志
				var contactEmailStr = ContactPersonData.value.map(item => item.email).join(',');
				if (contactEmailStr != null && contactEmailStr != '') {
					//getContactLogList(contactEmailStr);
					loadCustomerContactLogs(selectCustomerID.value, contactEmailStr);
				}
			});
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
	isDelete: 0
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

// 获取报价历史记录
const loadQuotationHistory = async (customerId: number) => {
	try {
		const response = await request({
			url: 'Quotation/GetQuotaionHistoryByCustomerID/GetHistory',
			method: 'GET',
			params: { CustomerID: customerId }
		});
		if (response.code === 200) {
			QuotationRecordData.value = [];
			// 转换数据
			QuotationRecordData.value = response.data;
		} else {
			ElMessage.error(response.msg || '获取报价历史失败');
		}
	} catch (error) {
		console.error('获取报价历史失败:', error);
		ElMessage.error('获取报价历史失败，请稍后重试');
	}
};
//销售合同记录
const SalesContractRecordData = ref([]);
// 获取销售历史记录
const loadContractHistory = async (customerId: number) => {
	try {
		const response = await request({
			url: 'Contracts/GetContractHistoryByCustomerID/GetHistory',
			method: 'GET',
			params: { CustomerID: customerId }
		});

		if (response.code === 200) {
			SalesContractRecordData.value = [];
			// 转换数据
			SalesContractRecordData.value = response.data;
		} else {
			ElMessage.error(response.msg || '获取销售历史失败');
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
// 获取客户寄样历史
const loadCustomerSendSampleHistory = async (customerId) => {
	try {
		const response = await request({
			url: 'ProductSample/GetCustomerHistoryByCustomerID/GetHistory',
			method: 'GET',
			params: { CustomerID: customerId }
		});

		if (response.code === 200) {
			// 转换数据
			CustomerSendSampleData.value = response.data.map((item: SampleHistoryItem) => ({
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
				remark: item.remark || ''
			}));
		} else {
			ElMessage.error(response.msg || '获取收寄样历史失败');
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

interface ContactLogForm {
	contactDate: string | Date
	ContactLogTag: string | number
	contactPerson: string | number
	ourStaff: string | number
	contactContent: string
	remark: string
	attachmentURLs: string
	imageURLs: string
}

const contactLogForm = reactive<ContactLogForm>({
	contactDate: new Date(),
	contactPerson: '',
	ContactLogTag: '',
	ourStaff: '',
	contactContent: '',
	remark: '',
	attachmentURLs: '',
	imageURLs: ''
})

const contactLogRules = reactive<FormRules>({
	contactDate: [{ required: true, message: '请选择联系日期', trigger: 'change' }],
	ContactLogTag: [{ required: true, message: '请选择日志标签', trigger: 'change' }],
	contactPerson: [{ required: true, message: '请选择联系人', trigger: 'change' }],
	ourStaff: [{ required: true, message: '请选择我方人员', trigger: 'change' }],
	contactContent: [{ required: true, message: '请输入联系内容', trigger: 'blur' }]
})

// 打开添加联系日志对话框
const AddContactLog = async () => {
	// 重置表单
	contactLogForm.contactDate = new Date()
	contactLogForm.contactPerson = ''
	contactLogForm.ourStaff = ''
	contactLogForm.contactContent = ''
	contactLogForm.remark = ''
	contactLogForm.attachmentURLs = ''
	contactLogForm.imageURLs = ''
	contactLogAttachments.value = []
	contactLogImages.value = []
	previewImageUrl.value = ''

	// 显示对话框
	contactLogDialogVisible.value = true
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
					Remark: contactLogForm.remark
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
					ElMessage.error(response.msg || '添加联系日志失败')
				}
			} catch (error) {
				console.error('添加联系日志失败:', error)
				ElMessage.error('添加联系日志失败')
			}
		}
	})
}


const ContactLogTablecurrentPage = ref(1)
const ContactLogTablepageSize = ref(2)
const ContactLogTabletotalItems = ref(0)
const ContactLogTablehandlePageChange = async (newPage: number) => {
	// Get email addresses from contact persons
	const emailAddresses = ContactPersonData.value
		.filter(person => person.email && person.email.trim() !== '')
		.map(person => person.email)
		.join(',')
	ContactLogTablecurrentPage.value = newPage;
	const start = newPage;
	const end = ContactLogTablepageSize.value;
	loadCustomerContactLogs(selectCustomerID.value, emailAddresses, start, end)
}

// 加载客户联系日志
const loadCustomerContactLogs = async (customerId: number, emailaddress: string = '', pageNum: number = 1, pageSize: number = 2) => {
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
						const staff = state.optionss.sql_hr_sale.find(staff => staff.dictValue === item.ourPersonnel);
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
					isChanged: false
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
			console.log('error submit!', fields)
		}
	})
}

const ContactLogTagData = ref([]);

const loadContactLogTagData = async () => {
	try {
		const response = await request({
			url: 'Email/GetUserEmailTagList/GetUserEmailTag',
			method: 'get'
		})
		if (response && response.code === 200) {
			ContactLogTagData.value = response.data;
			console.log(ContactLogTagData.value)
		} else {
			ElMessage.error(response.msg || '获取联系日志失败')
		}
	} catch (error) {
		console.error('获取联系日志失败:', error)
	}
}
</script>
