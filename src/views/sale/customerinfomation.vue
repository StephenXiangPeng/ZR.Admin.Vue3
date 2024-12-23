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
			<el-input v-model="Search_CustomerNo_input" clearable style="width: 15%"
				placeholder="输入客户编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="Search_CustomerEmail_input" clearable style="width: 15%"
				placeholder="输入邮箱地址" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="Search_CustomerName_input" clearable style="width: 15%"
				placeholder="输入客户名称" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-input v-model="Search_CustomerContactPerson_input" clearable style="width: 15%"
				placeholder="输入联系人" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="Search_BusinessScope_Select" placeholder="选择业务范围" style="width: 15%;" filterable>
				<el-option v-for="dict in optionss.hr_business_scope" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		</div>
		<div style="width: 100%; margin-top: 5px;">
			<el-select v-model="Search_TradingCountry_Select" class="m-2" placeholder="选择国家" size="default"
				style="width: 15%;" filterable>
				<el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="Search_CustomerLevel_Select" class="m-2" placeholder="选择客户等级" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select filterable v-model="Search_CustomerSource_Select" class="m-2" placeholder="选择客户来源" size="default"
				style="width: 15%;">
				<el-option v-for="dict in optionss.sys_customer_source" :key="dict.dictCode" :label="dict.dictLabel"
					:value="dict.dictValue"></el-option>
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="Search_StartDate_Select" type="date" placeholder="起始日期" size="default"
				style="width: 15%;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;------&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
			<el-table-column prop="customerNo" label="客户编号" style="width: 8%;" />
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
						style="width: 300px;">
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
						style="width: 300px;">
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
						<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
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
				<el-table-column prop="telephone" label="电话" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.telephone" placeholder="输入电话号码" size="default"></el-input>
					</template>
				</el-table-column>
				<el-table-column prop="cellphone" label="手机" width="180" align="left">
					<template #default="{ row }">
						<el-input v-model="row.cellphone" placeholder="输入手机号码" size="default"></el-input>
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
					<el-button type="primary" @click="submitForm(CustomerProfileformRef)">
						确定保存
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
				<el-table-column prop="Create_time" label="建档时间" style="width: 8%;" />
				<el-table-column prop="SalesPerson" label="所属销售员" style="width: 8%;" />
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
					<el-dialog v-model="dialogVisible">
						<img style="max-width: 100%; max-height: 100%; width: auto; height: auto;" w-full
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
						<el-table-column prop="telephone" label="电话" style="width: 8%;" />
						<el-table-column prop="cellphone" label="手机" style="width: 8%;" />
						<el-table-column prop="fax" label="传真" style="width: 8%;" />
						<el-table-column prop="email" label="邮件" style="width: 8%;" />
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="联系日志" name="ContactLogTable">
					<el-table :data="ContactLogData" height="300" style="width: 100%">
						<el-table-column prop="EmailDate" label="联系日期" />
						<el-table-column prop="Contact" label="联系人" />
						<el-table-column prop="OurPersonnel" label="我方人员" />
						<el-table-column prop="ContactDetails" label="联系内容" show-overflow-tooltip />
					</el-table>
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
	</div>
</template>

<script setup lang="ts">
import { getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessageBox, UploadUserFile, ElMessage, UploadFile, FormInstance, FormRules } from 'element-plus'
import request from '@/utils/request';
import { number } from 'echarts';
import { fa } from 'element-plus/es/locale';

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
		hr_ourcompany: []
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
const Search_CustomerName_input = ref('')
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
	Search_CustomerName_input.value = ''
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
	isDelete: number
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
	isDelete: 0
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
	const duplicate = uploadedFiles.value.findIndex(fileItem => fileItem.name === file.name);
	if (duplicate !== -1) {
		ElMessage({
			type: 'info',
			message: '不要上传重复的文件哦！😔'
		});
		const duplicatesInFileList = fileList.filter(fileItem => fileItem.name === file.name);
		if (duplicatesInFileList.length > 1) {
			for (let i = 1; i < duplicatesInFileList.length; i++) {
				const index = fileList.findIndex(fileItem => fileItem.uid === duplicatesInFileList[i].uid);
				if (index !== -1) {
					fileList.splice(index, 1); // 从fileList中移除重复文件，保留一个
				}
			}
		}
	} else {
		// 添加文件到uploadedFiles，确保不重复
		const newFiles = fileList.filter(file => !uploadedFiles.value.some(fileItem => fileItem.name === file.name));
		newFiles.forEach(file => {
			if (!file.isChanged) {
				file.isChanged = true;
				uploadedFiles.value.push(file);
			}
		});
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
	return new Promise((resolve, reject) => { // Adjust the Promise constructor usage
		request({
			url: 'CustomerInfoMation/GetCustomerInfoList/GetList',
			method: 'GET',
			params: {
				PageNum: start,
				PageSize: end,
				customerNo: Search_CustomerNo_input.value || '',
				emailAddress: Search_CustomerEmail_input.value || '',
				customerName: Search_CustomerName_input.value || '',
				contactPerson: Search_CustomerContactPerson_input.value || '',
				BusinessScope: Search_BusinessScope_Select.value || 0,
				TradingCountry: Search_TradingCountry_Select.value || 0,
				CustomerLevel: Search_CustomerLevel_Select.value || 0,
				CustomerSource: Search_CustomerSource_Select.value || 0,
				StartDate: Search_StartDate_Select.value || '',
				EndDate: Search_EndDate_Select.value || ''
			}
		}).then(response => {
			if (response.data.length > 0) {
				CunstomeinfotableData.value = response.data;
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
				}
				reject(new Error('无数据'));  // Reject the promise if the response is null
			}
		}).catch(error => {
			console.error(error);
			reject(error);  // Reject the promise if an error occurs
		});
	});
}
//详情联系人列表
const ContactPersonData = ref([]);
//联系日志
const ContactLogData = ref([]);
//报价记录
const QuotationRecordData = ref([]);
//双击查看详情
const CunstomeinfotableDatahandleRowDblClick = (row) => {
	OpenCustomerProfileDetailDialog(row);
}

//打开客户详情窗体并加载数据
const OpenCustomerProfileDetailDialog = (row) => {
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
	//加载联系人信息
	row.contactPerson.forEach(person => {
		if (person.sex == null || person.sex == '' || person.sex == undefined) {
			person.sexText = state.optionss['sys_user_sex'].find(option => "0" && option.dictValue.toString() === "0").dictLabel;
		} else {
			person.sexText = state.optionss['sys_user_sex'].find(option => person.sex && option.dictValue.toString() === person.sex.toString()).dictLabel;
		}
	});
	ContactPersonData.value = row.contactPerson;
	//加载联系日志
	var contactEmailStr = row.contactPerson.map(item => item.email).join(',');
	getContactLogList(contactEmailStr);
	//加载报价记录
	loadQuotationHistory(row.id);
	//加载销售合同记录
	loadContractHistory(row.id);
	//加载收寄样历史
	loadCustomerSendSampleHistory(row.id);
	CustomerProfileDetailDialog.value = true;
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
</script>
