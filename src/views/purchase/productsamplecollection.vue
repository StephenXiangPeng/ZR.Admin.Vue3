<template>
	<div>
		<div style="margin-top: 0px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;功能区</span>
		</div>
		<el-divider></el-divider>
		<el-button type="primary" @click="CreateDialog = true">创建收样/寄样</el-button>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;过滤条件</span>
		</div>
		<div style="width: 100%; margin-top: 30px;">
			<el-input v-model="quotationNum" clearable style="width: 15%"
				placeholder="输入销售合同编号" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="customerinfoselect" filterable placeholder="选择客户（可输入查询）" style="width: 15%">
				<el-option v-for="item in customerinfoselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-select v-model="productselect" filterable placeholder="选择产品（可输入查询）" style="width: 15%">
				<el-option v-for="item in productselectoptions" :key="item.value" :label="item.label"
					:value="item.value" />
			</el-select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="inquiryDate" type="date" placeholder="请选择出运日期" size="Default"
				style="width: 15%" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
			<el-date-picker v-model="quotationDate" type="date" placeholder="请选择出运日期" size="Default"
				style="width: 15%" />
		</div>
		<div style="width: 100%; margin-top: 5px;">
		</div>
		<div style="width: 100%; margin-top: 20px; text-align: right;">
			<el-row class="mb-4">
				<el-button type="primary" plain>查询</el-button>
				<el-button>重置</el-button>
			</el-row>
		</div>
		<div style="margin-top: 30px;">
			<span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;收样/寄样信息表</span>
		</div>
		<el-divider> </el-divider>
		<el-table :data="tableData">
			<el-table-column prop="sampleType" label="寄样/收样" width="150"></el-table-column>
			<el-table-column prop="waybillNumber" label="运单号" width="150"></el-table-column>
			<el-table-column prop="expressCompany" label="快件公司" width="150"></el-table-column>
			<el-table-column prop="registrationDate" label="登记日期" width="150"></el-table-column>
			<el-table-column prop="sampleObject" label="寄样对象" width="150"></el-table-column>
			<el-table-column prop="partnerNumber" label="伙伴编号" width="150"></el-table-column>
			<el-table-column prop="partnerAbbreviation" label="伙伴简称" width="150"></el-table-column>
			<el-table-column prop="customerLevel" label="客户等级" width="150"></el-table-column>
			<el-table-column prop="sampleStatus" label="寄样状态" width="150"></el-table-column>
			<el-table-column prop="sampleDate" label="寄样日期" width="150"></el-table-column>
			<el-table-column prop="receivedDate" label="收到日期" width="150"></el-table-column>
			<el-table-column prop="tradeCountry" label="贸易国别" width="150"></el-table-column>
			<el-table-column prop="countryRegion" label="国家区域" width="150"></el-table-column>
			<el-table-column prop="partnerContact" label="伙伴联系人" width="150"></el-table-column>
			<el-table-column prop="paymentMethod" label="付费方式" width="150"></el-table-column>
			<el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
			<el-table-column prop="sender" label="寄件人" width="150"></el-table-column>
			<el-table-column prop="expressCost" label="快件费用" width="150"></el-table-column>
			<el-table-column prop="paidExpressCost" label="已付快件费" width="150"></el-table-column>
			<el-table-column prop="totalBoxes" label="箱数合计" width="150"></el-table-column>
		</el-table>
		<el-pagination background layout="prev, pager, next" :total="1000" style="margin-top: 5px;" />

		<el-dialog v-model="CreateDialog" title="创建收样/寄样" :close-on-click-modal=false style="width: 70%;">
			<span style="font-size: 20px; font-weight: bold;">基本信息</span>
			<el-divider></el-divider>
			<el-form :model="CreateDialogform" label-width="120px">
				<el-row>
					<el-col :span="8">
						<el-form-item label="请选择">
							<el-radio-group v-model="radioValue">
								<el-radio label="1" size="large" border>寄样1111</el-radio>
								<el-radio label="2" size="large" border>收样</el-radio>
							</el-radio-group>

						</el-form-item>

					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="运单号">
							<el-input v-model="CreateDialogform.waybillNumber" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="快递公司">
							<el-select v-model="CreateDialogform.expressCompany" placeholder="请选择"
								style="width: 300px;">
								<el-option label="Company A" value="Company A"></el-option>
								<el-option label="Company B" value="Company B"></el-option>
								<el-option label="Company C" value="Company C"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="寄样日期">
							<el-date-picker v-model="CreateDialogform.sampleDate" type="date" placeholder="请选择"
								style="width: 300px;"></el-date-picker>
						</el-form-item>
						<el-form-item v-else-if="radioValue === '2'" label="收样日期">
							<el-date-picker v-model="CreateDialogform.sampleDate" type="date" placeholder="请选择"
								style="width: 300px;"></el-date-picker>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="寄样对象">
							<el-select v-model="CreateDialogform.sampleObject" placeholder="请选择" style="width: 300px;">
								<el-option label="Object A" value="Object A"></el-option>
								<el-option label="Object B" value="Object B"></el-option>
								<el-option label="Object C" value="Object C"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="对方简称">
							<el-input v-model="CreateDialogform.partnerAbbreviation" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="我方公司">
							<el-select v-model="CreateDialogform.ourCompany" placeholder="请选择" style="width: 300px;">
								<el-option label="Company X" value="Company X"></el-option>
								<el-option label="Company Y" value="Company Y"></el-option>
								<el-option label="Company Z" value="Company Z"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item label="业务员">
							<el-select v-model="CreateDialogform.salesperson" placeholder="请选择" style="width: 300px;">
								<el-option label="Salesperson A" value="Salesperson A"></el-option>
								<el-option label="Salesperson B" value="Salesperson B"></el-option>
								<el-option label="Salesperson C" value="Salesperson C"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="付费方式">
							<el-select v-model="CreateDialogform.paymentMethod" placeholder="请选择" style="width: 300px;">
								<el-option label="Method A" value="Method A"></el-option>
								<el-option label="Method B" value="Method B"></el-option>
								<el-option label="Method C" value="Method C"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item label="已付快递费">
							<el-input v-model="CreateDialogform.paidExpressCost" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="产品编号">
							<el-select v-model="CreateDialogform.sampleObject" placeholder="请选择" style="width: 300px;">
								<el-option label="Object A" value="Object A"></el-option>
								<el-option label="Object B" value="Object B"></el-option>
								<el-option label="Object C" value="Object C"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="中文品名">
							<el-input v-model="CreateDialogform.paidExpressCost" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="寄样数量">
							<el-input v-model="CreateDialogform.paidExpressCost" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :span="8">
						<el-form-item v-if="radioValue === '1'" label="计价金额">
							<el-input v-model="CreateDialogform.paidExpressCost" style="width: 300px;"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="24">
						<el-form-item label="样品照片">
							<el-upload class="upload-demo" action="/upload" :on-success="handleUploadSuccess"
								:on-remove="handleUploadRemove" :file-list="CreateDialogform.photos"
								list-type="picture-card" :auto-upload="false" :limit="3">
								<i class="el-icon-plus"></i>
							</el-upload>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary">
						确定保存
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { createApp, ref } from 'vue'
import { ElButton, ElDivider, ElDialog, ElForm, ElTable, ElTableColumn, ElTreeV2, ElIcon, ElContainer } from 'element-plus'


const CreateDialogform = ref({
	recipienttypeexamples: '',
	waybillNumber: '',
	expressCompany: '',
	sampleDate: '',
	sampleObject: '',
	partnerAbbreviation: '',
	ourCompany: '',
	salesperson: '',
	paymentMethod: '',
	paidExpressCost: '',
	photos: [],
});

const handleUploadSuccess = (response, file, fileList) => {
	// Handle upload success
};

const handleUploadRemove = (file, fileList) => {
	// Handle file removal
};

const radioValue = ref('1');

const CreateDialog = ref(false);

const tableData = ref([
	// Mock data entries
	{
		sampleType: '收样',
		waybillNumber: '1234567890',
		expressCompany: 'Company A',
		registrationDate: '2022-01-01',
		sampleObject: 'Object A',
		partnerNumber: '001',
		partnerAbbreviation: 'Partner A',
		customerLevel: 'Level A',
		sampleStatus: 'Pending',
		sampleDate: '2022-01-02',
		receivedDate: '2022-01-03',
		tradeCountry: 'Country A',
		countryRegion: 'Region A',
		partnerContact: 'Contact A',
		paymentMethod: 'Method A',
		ourCompany: 'Our Company',
		sender: 'Sender A',
		expressCost: '100',
		paidExpressCost: '50',
		totalBoxes: '5',
	},
	// Add more mock data entries here...
]);

const customerinfoselect = ref('');

</script>

<style scoped></style>
