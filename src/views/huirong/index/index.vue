<template>
  <div class="home">
    <!-- 任务看板 -->
    <el-row :gutter="15" style="font-size: 25px;">
      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%; border: 0px;" shadow="never">
          <div class="text-warning mb10">待办流程</div>
          <el-divider></el-divider>
          <div class="work-wrap" style="font-size: 25px;">
            <el-row>
              <cl-col>
                <span>待您处理</span>
              </cl-col>
            </el-row>
            <el-row>
              <cl-col>
                <span>超时未处理</span>
              </cl-col>
            </el-row>
            <span>&nbsp;&nbsp;&nbsp;<el-button type="text" style="font-weight: bold;font-size: 30px; color: black;"
                @click="WaitingforyouProcessedClick">{{
                  pendingCount }}</el-button></span>
            <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<el-button type="text"
                style="font-weight: bold;font-size: 30px; color: red;" @click="TimeoutNotProcessedClick">{{
                  TimeoutNotProcessedCount }}</el-button></span>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%; border: 0px;" shadow="never">
          <div class="text-warning mb10">业务示警</div>
          <el-divider></el-divider>
          <div class="work-wrap2">
            <el-row>
              <cl-col>
                <span>交货逾期</span>
              </cl-col>
            </el-row>
            <el-row>
              <cl-col>
                <span>货款逾期</span>
              </cl-col>
            </el-row>
            <el-row>
              <cl-col>
                <span>沟通逾期</span>
              </cl-col>
            </el-row>
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;
              <el-button type="text" style="font-weight: bold;font-size: 30px; color: red;"
                @click="OverdueDeliveryContractClick">{{ OverdueDeliveryContractCount }}
              </el-button>
            </span>
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;
              <el-button type="text" style="font-weight: bold;font-size: 30px; color: red;"
                @click="PaymentoverdueClick">{{ PaymentoverdueCount }}
              </el-button></span>
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp; <el-button type="text"
                style="font-weight: bold;font-size: 30px; color: red;" @click="CommunicationOverdueClick">{{
                  CommunicationOverdueCount }}
              </el-button></span>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%;border: 0px;" shadow="never">
          <div class="text-warning mb10">工作任务</div>
          <el-divider></el-divider>
          <div class="work-wrap">
            <el-row>
              <cl-col>
                <span>待您处理</span>
              </cl-col>
            </el-row>
            <el-row>
              <cl-col>
                <span>超时未处理</span>
              </cl-col>
            </el-row>
            <span style="font-weight: bold;font-size: 30px; cursor: pointer;"
              @click="showPendingEmails">&nbsp;&nbsp;&nbsp;&nbsp;{{
                pendingEmailCount }}</span>
            <span style="color: red; font-weight: bold;font-size: 30px; cursor: pointer;"
              @click="showOverdueEmails">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{{
                overdueEmailCount }}</span>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 计划任务日历 -->
    <el-calendar class="custom-calendar" v-model="currentDate">
      <template #date-cell="{ data }">
        <div class="calendar-cell">
          <!-- 日期显示部分 -->
          <div :class="[
            'date-text',
            isToday(data.day) ? 'is-today' : '',
            data.isSelected ? 'is-selected' : '',
            calendarDates.find(d => formatDate(d.date) === data.day)?.type
          ]">
            {{ data.day.split('-').slice(1).join('-') }}
            <span v-if="isToday(data.day)" class="today-icon">今日</span>
          </div>

          <!-- 任务列表部分 -->
          <div class="tasks-container">
            <template v-for="task in getDateTasks(data.day)" :key="task.id">
              <el-tooltip
                :content="`${task.itemName}<br/>时间：${formatDateTime(task.timePoint)}<br/>备注：${task.remark || '无'}`"
                raw-content placement="top">
                <div class="task-item" :class="{ 'is-overdue': isOverdue(task.timePoint) }"
                  @click="handleTaskClick(task)">
                  <el-tag size="small" :type="isOverdue(task.timePoint) ? 'danger' : 'warning'">
                    {{ task.itemName }}
                  </el-tag>
                </div>
              </el-tooltip>
            </template>
          </div>
        </div>
      </template>
    </el-calendar>

    <!-- 工作任务 -->
    <span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;提醒事项</span>
    <el-divider></el-divider>
    <el-table :data="TaskReminderTableData" style="width: 50%;">
      <el-table-column prop="taskName" label="项目分类" style="width: 25%;" />
      <el-table-column prop="taskDescription" label="内容" style="width: 25%;" />
      <el-table-column prop="reminderTime" label="原设定时间节点" style="width: 25%;">
        <template #default="scope">
          {{ formatDateTime(scope.row.reminderTime) }}
        </template>
      </el-table-column>
      <el-table-column prop="isCompleted" label="是否通知" style="width: 25%;" v-if="false">
        <template #default="scope">
          <span :class="scope.row.isCompleted ? 'text-success' : 'text-danger'">
            {{ scope.row.isCompleted ? '已通知' : '未通知' }}
          </span>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination v-model:current-page="TaskReminderTableCurrentPage" v-model:page-size="TaskReminderTablePageSize"
      :total="TaskReminderTableTotalItems" @current-change="TaskReminderTableshandlePageChange"
      layout="total, prev, pager, next" />

    <div style="margin-top: 30px;"></div>
    <!-- 财务任务 -->
    <span style="font-size: 20px; font-weight: bold; margin-top: 30px;">&nbsp;&nbsp;财务任务</span>
    <el-divider></el-divider>
    <!--财务任务列表 -->
    <el-table :data="FinancialTasksTableData" style="width: 100%">
      <!-- 其他列... -->
      <el-table-column prop="id" label="ID" v-if="false" />
      <el-table-column prop="receiptNumber" label="收款单号" />
      <el-table-column prop="receiptDate" label="收汇日期" />
      <el-table-column prop="ourCompany" label="我方公司" />
      <el-table-column prop="foreignCurrency" label="外销币种" />
      <el-table-column prop="exchangeRate" label="汇率" />
      <el-table-column prop="amount" label="金额" />
      <el-table-column prop="bank" label="收汇银行" />
      <el-table-column label="收款凭证" width="120">
        <template #default="{ row }">
          <el-image v-if="row.receiptImageUrl" style="width: 50px; height: 50px; cursor: pointer"
            :src="row.receiptImageUrl" :preview-src-list="[row.receiptImageUrl]" preview-teleported fit="cover"
            :initial-index="0" hide-on-click-modal>
            <template #error>
              <div class="image-error">
                <el-icon>
                  <Picture />
                </el-icon>
              </div>
            </template>
          </el-image>
          <span v-else>无图片</span>
        </template>
      </el-table-column>
      <el-table-column fixed="right" label="操作" style="width: 25%;">
        <template #default="{ row }">
          <el-button link type="primary" size="small" @click="handleClaim(row)">领取</el-button>
        </template>
      </el-table-column>
      <!-- 其他列... -->
    </el-table>
    <!-- 分页组件 -->
    <el-pagination v-model:current-page="FinancialTasksTableCurrentPage" v-model:page-size="FinancialTasksTablePageSize"
      :total="FinancialTasksTableTotalItems" @current-change="FinancialTasksTableshandlePageChange"
      layout="total, prev, pager, next" />


    <div style="margin-top: 30px;"></div>
    <span style="font-size: 20px; font-weight: bold; ">&nbsp;&nbsp;商机</span>
    <el-divider></el-divider>
    <div style="display: flex; margin-top: 5px;">
      <div v-for="stage in sortedStages" :key="stage.salesStage" style="width: 20%; height: 560px;">
        <!-- 标题栏 -->
        <div
          style="background-color: #41c16e; height: 8%; padding: 10px; text-align: center; color: white; font-weight:500;">
          {{ stage.salesStage }}（{{ stage.count }}）
        </div>

        <!-- 金额显示 - 询盘和沟通需求显示0，其他阶段显示实际金额 -->
        <div style="text-align: center;">
          <span style="color: black; font-weight: 600;">
            CNY {{ shouldShowZeroAmount(stage.salesStage) ? '0.00' : formatAmount(stage.totalAmount) }}
          </span>
        </div>

        <!-- 商机卡片列表 -->
        <div style="overflow: auto; height: 490px;">
          <el-card v-for="item in stage.details" :key="item.id" style="margin: 3px;" class="box-card">
            <template #header>
              <div class="card-header">
                <span>商机编号：{{ item.opportunityNumber }}</span>
              </div>
            </template>
            <div class="text item">商机名称：
              <el-tooltip :content="item.businessName" placement="top" :disabled="item.businessName.length <= 8">
                <span class="truncate-text">{{ truncateText(item.businessName, 8) }}</span>
              </el-tooltip>
            </div>
            <div class="text item">客户邮箱：
              <el-tooltip :content="item.customer" placement="top" :disabled="item.customer.length <= 16">
                <span class="truncate-text">{{ truncateText(item.customer, 16) }}</span>
              </el-tooltip>
            </div>
            <div class="text item">创建时间：{{ formatDateTime(item.create_time) }}</div>
            <!-- 根据销售阶段显示不同信息 -->
            <div class="text item" v-if="shouldShowAmount(stage.salesStage)">
              预估金额：{{ formatAmount(item.amount) }}
            </div>
            <div class="text item" v-if="stage.salesStage === '沟通需求' && item.emailcreatetime">
              最后沟通：{{ formatDateTime(item.emailcreatetime) }}
            </div>
          </el-card>
        </div>
      </div>
    </div>
    <!-- <el-row :gutter="15" style=" margin-top: 30px; font-size: 25px;">
      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%">
          <div class="text-warning mb10">国际时区&nbsp;&nbsp;&nbsp;&nbsp;
            <el-select v-model="value" class="m-2" placeholder="请选择" size="default">
              <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </div>
          <el-divider />
          <div class="work-wrap" style="margin-top: 10px;">
            <el-text class="mx-1" type="warning" size="large" style="color: black;font-size: 25px;">11月29日 周三
              20:55:00</el-text>
          </div>
        </el-card>
      </el-col>
      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%">
          <div class="text-warning mb10">实时汇率&nbsp;&nbsp;&nbsp;&nbsp;
            <el-select v-model="value2" class="m-2" placeholder="请选择" size="default" style="width: 33%;">
              <el-option v-for="item in options2" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
            --
            <el-select v-model="value3" class="m-2" placeholder="请选择" size="default" style="width: 33%;">
              <el-option v-for="item in options3" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </div>
          <el-divider />
          <div class="work-wrap" style="margin-top: 10px;">
            <el-text class="mx-1" type="warning" size="large"
              style="color: black; font-size: 25px;">实时汇率：100&nbsp;&nbsp;=&nbsp;&nbsp;732&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</el-text>
            <el-text class="mx-1" type="warning" size="large"
              style="color: black;font-size: 25px;">&nbsp;现汇买入价：&nbsp;732</el-text>
          </div>
        </el-card>
      </el-col>
    </el-row> -->
    <el-dialog v-model="AgencyProcessdialogTableVisible" title="待办流程" width="800">
      <el-table :data="AgencyProcesstableData">
        <el-table-column prop="documentType" label="单据类型ID" width="150" align="center" v-if="false" />
        <el-table-column prop="documentID" label="单据编号ID" width="150" align="center" v-if="false" />
        <el-table-column prop="documentTypeName" label="单据类型" width="150" align="center" />
        <el-table-column prop="documentNumber" label="单据编号" width="150" align="center" />
        <el-table-column prop="createBy" label="发起人" width="150" align="center" />
        <el-table-column prop="createTime" label="发起时间" width="180" align="center" />
        <el-table-column fixed="right" prop="operate" label="操作" style="width: 8%;" align="center">
          <template v-slot:default="scope">
            <el-button link type="primary" size="small" @click="openSaleContractDialog(scope.row)">查看详情</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
    <el-dialog v-model="contractDialog" title="销售合同审批" :close-on-click-modal=false style="width: 70%;">
      <span style="font-size: 20px; font-weight: bold;">基本信息</span>
      <el-divider></el-divider>
      <el-form :model="contractform" label-width="120px">
        <el-row>
          <el-col :span="8">
            <el-form-item label="销售合同">
              <el-input v-model="contractform.contractNumber" disabled style="width: 300px"
                placeholder="自动生成"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="合同日期">
              <el-date-picker v-model="contractform.contractDate" type="date" placeholder="请选择合同日期" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="合同状态">
              <el-select filterable v-model="contractform.contractStatus" placeholder="请选择合同状态" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="客户编号" prop="customerNumber">
              <el-select filterable v-model="contractform.customerNumber" placeholder="请选择客户编号" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户简称" prop="customerAbbreviation">
              <el-select v-model="contractform.customerAbbreviation" filterable placeholder="请选择客户简称" disabled
                style="width: 300px;">
                <el-option v-for="dict in optionss.sql_hr_customer_abbreviation" :key="dict.dictCode"
                  :label="dict.dictLabel" :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="联系人" prop="contactPerson">
              <el-select filterable v-model="contractform.contactPerson" placeholder="请选择联系人" style="width: 300px"
                disabled>
                <el-option v-for="item in contactpersonSelectOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8" v-if="false">
            <el-form-item label="联系人Email">
              <el-input v-model="contractform.contactEmail" disabled style="width: 300px"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="生效日期">
              <el-date-picker v-model="contractform.effectiveDate" type="date" placeholder="请选择生效日期" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户等级">
              <el-select filterable v-model="contractform.customerLevel" placeholder="请选择客户等级" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户合同">
              <el-input v-model="contractform.customerContract" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="交货日期" prop="deliveryDate">
              <el-date-picker v-model="contractform.deliveryDate" type="date" placeholder="请选择交货日期" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="我方公司" prop="ourCompany">
              <el-select filterable v-model="contractform.ourCompany" placeholder="请选择我方公司" style="width: 300px"
                disabled>
                <el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="外销币种" prop="foreignCurrency">
              <el-select filterable v-model="contractform.foreignCurrency" placeholder="请选择外销币种" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8" v-if="false">
            <el-form-item label="结算类别" prop="settlementType">
              <el-select filterable v-model="contractform.settlementType" placeholder="请选择结算类别" disabled
                style="width: 300px">
                <el-option label="类别1" value="1"></el-option>
                <el-option label="类别2" value="2"></el-option>
                <el-option label="类别3" value="3"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="汇率" prop="exchangeRate">
              <el-input v-model="contractform.exchangeRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="结汇方式" prop="settlementMethod">
              <el-select filterable v-model="contractform.settlementMethod" placeholder="请选择结汇方式" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="价格条款" prop="priceTerms">
              <el-select filterable v-model="contractform.priceTerms" placeholder="请选择价格条款" style="width: 300px"
                disabled>
                <el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8" v-if="false">
            <el-form-item label="美金汇率" prop="usdExchangeRate">
              <el-input v-model="contractform.usdExchangeRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="出运口岸" prop="shippingPort">
              <el-select filterable v-model="contractform.shippingPort" placeholder="请选择出运口岸" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="目的口岸" prop="destinationPort">
              <el-select filterable v-model="contractform.destinationPort" placeholder="请选择目的口岸" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="贸易国别" prop="tradeCountry">
              <el-select filterable v-model="contractform.tradeCountry" placeholder="请选择贸易国别" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="运输方式" prop="transportation">
              <el-select filterable v-model="contractform.transportation" placeholder="请选择运输方式" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
                  :label="dict.dictLabel" :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="销售员" prop="salesperson">
              <el-select filterable v-model="contractform.salesperson" placeholder="请选择销售员" style="width: 300px"
                disabled>
                <el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="有无定金" prop="hasDeposit">
              <el-checkbox v-model="contractform.hasDeposit" disabled></el-checkbox>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="已收定金" v-show=DepositShow prop="receivedDeposit">
              <el-input v-model="contractform.receivedDeposit" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="定金日期" v-show=DepositShow prop="depositDate">
              <el-date-picker v-model="contractform.depositDate" type="date" placeholder="请选择定金日期" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="定金比例" v-show=DepositShow prop="Depositratio">
              <el-input v-model="contractform.Depositratio" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">辅助信息</span>
        <el-divider></el-divider>
        <el-row>
          <el-col :span="8" v-if="false">
            <el-form-item label="签约地点">
              <el-select filterable v-model="contractform.signingLocation" placeholder="请选择签约地点" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.hr_signing_place" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="可否分批">
              <el-select filterable v-model="contractform.canPartial" placeholder="请选择可否分批" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="可否转运">
              <el-select filterable v-model="contractform.canTransit" placeholder="请选择可否转运" disabled
                style="width: 300px">
                <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="佣金比率">
              <el-input v-model="contractform.commissionRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="保险加成">
              <el-input v-model="contractform.insuranceAddition" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="保险比率">
              <el-input v-model="contractform.insuranceRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="海运费/m³">
              <el-input v-model="contractform.oceanFreight" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="海运费币种" prop="shippingCurrency">
              <el-select v-model="contractform.shippingCurrency" filterable placeholder="选择运费币种" disabled
                style="width: 300px;">
                <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="海运费汇率">
              <el-input v-model="contractform.shippingrate" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="港杂费/m³">
              <el-input v-model="contractform.portMiscellaneousFees" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="货代报关杂费">
              <el-input v-model="contractform.freightForwarderCustomsClearanceFees" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
          <el-col :span="8" v-if="false">
            <el-form-item label="收汇银行">
              <el-input v-model="contractform.receivingBank" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="银行费用">
              <el-input v-model="contractform.BankFee" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="文件杂费">
              <el-input v-model="contractform.DocumentationFees" style="width: 300px;" disabled />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8" v-if="false">
            <el-form-item label="付款日期">
              <el-date-picker v-model="contractform.paymentDate" type="date" placeholder="请选择付款日期" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">产品资料&客户相关费用</span>
        <el-divider></el-divider>
        <el-tabs v-model="activeTab" tab-position="top" style="height: 350px;" class="demo-tabs">
          <el-tab-pane label="产品资料" name="productMaterialtab">
            <el-table :data="productData" style="width: 100%;margin-bottom: 15px;" max-height="550">
              <el-table-column prop="productID" label="产品ID" width="120" v-if="false" />
              <el-table-column prop="productNum" label="产品编号" width="120" />
              <el-table-column prop="customerNum" label="客户货号" width="120" />
              <el-table-column prop="cproductname" label="中文品名" width="120">
                <template #default="{ row }">
                  <span v-if="row.isImported">{{ row.cproductname }}</span>
                  <el-input v-else v-model="row.cproductname" disabled></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="cspecification" label="中文规格" width="120">
                <template #default="{ row }">
                  <span v-if="row.isImported">{{ row.cspecification }}</span>
                  <el-input v-else v-model="row.cspecification" disabled></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="contractQuantity" label="合同数量" width="110">
                <template #default="{ row }">
                  <el-input v-model="row.contractQuantity" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="exportunitprice" label="外销单价" width="110">
                <template #default="{ row }">
                  <el-input v-model="row.exportunitprice" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="exporttotalprice" label="外销总价" width="110">
                <template #default="scope">
                  <span>{{ scope.row.exporttotalprice }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="unitofmeasurement" label="计量单位" width="100">
                <template #default="scope">
                  <el-select v-model="scope.row.unitofmeasurement" filterable placeholder="单位" style="width: 100%;"
                    disabled>
                    <el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="purchasecurrency" label="采购币种" width="110">
                <template #default="scope">
                  <el-select v-model="scope.row.purchasecurrency" filterable placeholder="币种" style="width: 100%;"
                    disabled>
                    <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="purchaseunitprice" label="采购单价" width="110">
                <template #default="{ row }">
                  <el-input v-model="row.purchaseunitprice" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="inlandfreightprice" label="内陆运费(m³)" width="110">
                <template #default="{ row }">
                  <el-input v-model="row.inlandfreightprice" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="AdditionalPackagingCosts" label="单个产品额外包装费用" width="165">
                <template #default="{ row }">
                  <el-input v-model="row.AdditionalPackagingCosts" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="singleProductGrossProfit" label="单个产品毛利" width="110">
                <template #default="scope">
                  <span>{{ scope.row.singleProductGrossProfit }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="singleProductGrossProfitTotal" label="单个产品毛利合计" width="140">
                <template #default="scope">
                  <span>{{ scope.row.singleProductGrossProfitTotal }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="grossProfitRate" label="毛利率%" width="110">
                <template #default="scope">
                  <span>{{ scope.row.grossProfitRate }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="isInvoicingc" label="是否开票" width="120">
                <template #default="scope">
                  <el-select v-model="scope.row.isInvoicingc" filterable placeholder="请选择" style="width: 100%;"
                    disabled>
                    <el-option v-for="dict in optionss.hr_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="packaging" label="包装方式" width="150">
                <template #default="scope">
                  <el-select v-model="scope.row.packaging" filterable disabled placeholder="请选择" style="width: 100%;">
                    <el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="specialrequirements" label="特殊要求" width="200">
                <template #default="{ row }">
                  <el-input v-model="row.specialrequirements" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="rebaterate" label="退税率%" width="100">
                <template #default="{ row }">
                  <el-input v-model="row.rebaterate" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="innerBoxLoading" label="内盒装量" width="100">
                <template #default="{ row }">
                  <el-input v-model="row.innerBoxLoading" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxloading" label="外箱装量" width="100">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxloading" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxunit" label="外箱单位" width="150">
                <template #default="scope">
                  <el-select v-model="scope.row.outerboxunit" filterable placeholder="外箱单位" style="width: 100%;"
                    disabled>
                    <el-option v-for="dict in optionss.hr_outerbox_unit" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="outerboxlength" label="外箱长度(CM)" width="120">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxlength" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxwidth" label="外箱宽度(CM)" width="120">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxwidth" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxheight" label="外箱高度(CM)" width="120">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxheight" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxnetweight" label="外箱净重(KG)" width="120">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxnetweight" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxgrossweight" label="外箱毛重(KG)" width="120">
                <template #default="{ row }">
                  <el-input v-model="row.outerboxgrossweight" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="outerboxvolume" label="外箱体积(m³)" width="120">
                <template #default="scope">
                  <span>{{ scope.row.outerboxvolume }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="NumberOfBoxes" label="箱数" width="100">
                <template #default="scope">
                  <span>{{ scope.row.NumberOfBoxes }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="totalNetWeight" label="总净重(KG)" width="100">
                <template #default="scope">
                  <span>{{ scope.row.totalNetWeight }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="totalGrossWeight" label="总毛重(KG)" width="100">
                <template #default="scope">
                  <span>{{ scope.row.totalGrossWeight }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="totalVolume" label="总体积(m³)" width="100">
                <template #default="scope">
                  <span>{{ scope.row.totalVolume }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="OtherFees" label="单个产品其它费用" width="140">
                <template #default="{ row }">
                  <el-input v-model="row.OtherFees" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="SinglesalesrevenueA" label="单个销售收入A" width="120" v-if="true">
                <template #default="scope">
                  <span>{{ scope.row.SinglesalesrevenueA }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="Singleproductvolume" label="单个产品体积(m³)" width="135" v-if="true">
                <template #default="scope">
                  <span>{{ scope.row.Singleproductvolume }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="Portchargesforindividualproducts" label="单个产品的港杂费" width="140" v-if="true">
                <template #default="scope">
                  <span>{{ scope.row.Portchargesforindividualproducts }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="Oceanfreightforasingleproduct" label="单个产品海运费" width="130" v-if="true">
                <template #default="scope">
                  <span>{{ scope.row.Oceanfreightforasingleproduct }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="Inlandfreightforasingleproduct" label="单个产品内陆运费" width="140" v-if="true">
                <template #default="scope">
                  <span>{{ scope.row.Inlandfreightforasingleproduct }}</span>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
          <el-tab-pane label="客户相关费用" name="CustomerRelaterExoensestab">
            <el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; " height="280">
              <el-table-column prop=" expenseName" label="费用名称" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.expenseName" placeholder="输入费用名称" size="small" disabled></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="currency" label="币种" width="150">
                <template #default="{ row }">
                  <el-select filterable v-model="row.currency" placeholder="选择币种" size="small" disabled>
                    <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="exchangeRate" label="汇率" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.exchangeRate" placeholder="输入汇率" size="small" disabled></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="expense" label="费用" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.expense" placeholder="输入费用" size="small" disabled></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="amount" label="金额" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.amount" disabled size="small"></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="remark" label="备注" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.remark" placeholder="输入备注" size="small" disabled></el-input>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
        <span style="font-size: 20px; font-weight: bold;">合计信息</span>
        <el-divider></el-divider>
        <el-row>
          <el-col :span="8">
            <el-form-item label="货值合计">
              <el-input v-model="contractform.TotalValueOfGoods" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="数量合计">
              <el-input v-model="contractform.TotalQuantity" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="箱数合计">
              <el-input v-model="contractform.TotalNumberOfBoxes" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="毛重合计">
              <el-input v-model="contractform.TotalGrossWeight" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="净重合计">
              <el-input v-model="contractform.TotalNetWeight" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="体积合计">
              <el-input v-model="contractform.TotalVolume" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="采购合计">
              <el-input v-model="contractform.TotalPurchases" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="退税总额">
              <el-input v-model="contractform.TotalTaxRefund" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户费用合计">
              <el-input v-model="contractform.customerExpenseTotal" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="金额合计">
              <el-input v-model="contractform.amountTotal" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="美金/欧元换算">
              <el-input v-model="contractform.usdConversion" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="总毛利合计">
              <el-input v-model="contractform.Totalgrossprofit" disabled style="width: 300px;" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="其它费用合计">
              <el-input v-model="contractform.TotalOtherFees" disabled style="width: 300px;" />
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">利润预估</span>
        <el-divider></el-divider>
        <el-row>
          <el-col :span="8">
            <el-form-item label="利润金额">
              <el-input v-model="contractform.ProfitAmount" disabled style="width: 300px;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="退税总额">
              <el-input v-model="contractform.TotalTaxRefund" disabled style="width: 300px;" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="总利润率%">
              <el-input v-model="contractform.Totalprofitmargin" disabled style="width: 300px;" />
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">备注信息</span>
        <el-divider></el-divider>
        <el-input v-model="contractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }" type="textarea"
          placeholder="请输入备注信息" disabled />
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="ApproveReject">
            驳回
          </el-button>
          <el-button type="success" @click="Approvepass">
            通过
          </el-button>
        </span>
      </template>
    </el-dialog>
    <el-dialog v-model="PurchaseContractDialog" title="采购合同审批" :close-on-click-modal=false style="width: 70%;">
      <span style="font-size: 20px; font-weight: bold;">基本信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="采购合同">
          {{ PurchaseContractDialogData.purchaseContract }}
        </el-descriptions-item>
        <el-descriptions-item label="合同状态">
          {{ PurchaseContractDialogData.contractStatus }}
        </el-descriptions-item>
        <el-descriptions-item label="交货日期">
          {{ PurchaseContractDialogData.deliveryDate }}
        </el-descriptions-item>
        <el-descriptions-item label="采购币种">
          {{ PurchaseContractDialogData.purchaseCurrency }}
        </el-descriptions-item>
        <el-descriptions-item label="已付定金">
          {{ PurchaseContractDialogData.deposit }}
        </el-descriptions-item>
        <el-descriptions-item label="销售员">
          {{ PurchaseContractDialogData.salesperson }}
        </el-descriptions-item>
        <el-descriptions-item label="采购员">
          {{ PurchaseContractDialogData.purchaser }}
        </el-descriptions-item>
        <el-descriptions-item label="价格条款">
          {{ PurchaseContractDialogData.priceTerms }}
        </el-descriptions-item>
        <el-descriptions-item label="付款天数">
          {{ PurchaseContractDialogData.paymentDays }}
        </el-descriptions-item>
      </el-descriptions>

      <el-tabs v-model="PurchaseContractDialogData.activeName" class="demo-tabs">
        <el-tab-pane label="产品资料" name="productinfo">
          <el-table :data="PurchaseContractDialogData.productinfotableData">
            <el-table-column prop="productSupplier" label="产品供应商" width="150"></el-table-column>
            <el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
            <el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
            <el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
            <el-table-column prop="englishName" label="英文品名" width="150"></el-table-column>
            <el-table-column prop="chineseSpec" label="中文规格" width="150"></el-table-column>
            <el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
            <el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
            <el-table-column prop="purchasePrice" label="采购单价" width="150"></el-table-column>
            <el-table-column prop="purchaseTotalPrice" label="采购总价" width="150"></el-table-column>
            <el-table-column prop="deliveryDate" label="交货日期" width="150"></el-table-column>
            <el-table-column prop="productionLeadTime" label="生产交期" width="150"></el-table-column>
            <el-table-column prop="packaging" label="包装方式" width="150"></el-table-column>
            <el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
            <el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
            <el-table-column prop="innerBoxQuantity" label="内盒装量" width="150"></el-table-column>
            <el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
            <el-table-column prop="remark" label="备注" width="150"></el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="厂家相关费用" name="relatedcosts">
          <el-table :data="PurchaseContractDialogData.CustomerRelaterExoensesTableData" style="width: 100%; "
            height="280">
            <el-table-column prop="expenseName" label="费用名称" width="150"></el-table-column>
            <el-table-column prop="currency" label="币种" width="150"></el-table-column>
            <el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
            <el-table-column prop="expense" label="费用" width="150"></el-table-column>
            <el-table-column prop="amount" label="金额" width="150"></el-table-column>
            <el-table-column prop="remark" label="备注" width="150"></el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>

      <span style="font-size: 20px; font-weight: bold;">合计信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="货值合计">
          {{ PurchaseContractDialogData.TotalValueOfGoods }}
        </el-descriptions-item>
        <el-descriptions-item label="数量合计">
          {{ PurchaseContractDialogData.TotalQuantity }}
        </el-descriptions-item>
        <el-descriptions-item label="箱数合计">
          {{ PurchaseContractDialogData.TotalNumberOfBoxes }}
        </el-descriptions-item>
        <el-descriptions-item label="毛重合计">
          {{ PurchaseContractDialogData.TotalGrossWeight }}
        </el-descriptions-item>
        <el-descriptions-item label="净重合计">
          {{ PurchaseContractDialogData.TotalNetWeight }}
        </el-descriptions-item>
        <el-descriptions-item label="体积合计">
          {{ PurchaseContractDialogData.TotalVolume }}
        </el-descriptions-item>
        <el-descriptions-item label="已申请付款">
          {{ PurchaseContractDialogData.appliedPayment }}
        </el-descriptions-item>
        <el-descriptions-item label="可申请付款">
          {{ PurchaseContractDialogData.availablePayment }}
        </el-descriptions-item>
        <el-descriptions-item label="已付货款">
          {{ PurchaseContractDialogData.paidAmount }}
        </el-descriptions-item>
        <el-descriptions-item label="未付货款">
          {{ PurchaseContractDialogData.unpaidAmount }}
        </el-descriptions-item>
      </el-descriptions>

      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="ApproveReject">
            驳回
          </el-button>
          <el-button type="success" @click="Approvepass">
            通过
          </el-button>
        </span>
      </template>
    </el-dialog>
    <el-dialog v-model="PaymentrequestDialog" title="付款申请审批" :close-on-click-modal="false" style="width: 70%;">
      <!-- 基本信息 -->
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="申请单号">
          {{ PaymentrequestForm.applicationNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="申请日期">
          {{ PaymentrequestForm.applicationDate }}
        </el-descriptions-item>

        <el-descriptions-item label="付款类别">
          {{ PaymentrequestForm.paymentCategory }}
        </el-descriptions-item>

        <el-descriptions-item label="款项名称">
          {{ PaymentrequestForm.paymentName }}
        </el-descriptions-item>

        <el-descriptions-item label="收款单位编号">
          {{ PaymentrequestForm.payeeCode }}
        </el-descriptions-item>

        <el-descriptions-item label="收款单位名称">
          {{ PaymentrequestForm.payeeName }}
        </el-descriptions-item>

        <el-descriptions-item label="开户银行">
          {{ PaymentrequestForm.bankName }}
        </el-descriptions-item>

        <el-descriptions-item label="银行账号">
          {{ PaymentrequestForm.bankAccount }}
        </el-descriptions-item>

        <el-descriptions-item label="我方公司">
          {{ PaymentrequestForm.ourCompany }}
        </el-descriptions-item>

        <el-descriptions-item label="货币代码">
          {{ PaymentrequestForm.currencyCode }}
        </el-descriptions-item>

        <el-descriptions-item label="申请总额">
          {{ PaymentrequestForm.totalAmount }}
        </el-descriptions-item>

        <el-descriptions-item label="已付金额">
          {{ PaymentrequestForm.paidAmount }}
        </el-descriptions-item>

        <el-descriptions-item label="未付金额">
          {{ PaymentrequestForm.unpaidAmount }}
        </el-descriptions-item>

        <el-descriptions-item label="申请人">
          {{ PaymentrequestForm.applicant }}
        </el-descriptions-item>

        <el-descriptions-item label="申请部门">
          {{ PaymentrequestForm.applicationDepartment }}
        </el-descriptions-item>

        <el-descriptions-item label="经手人">
          {{ PaymentrequestForm.handler }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 备注说明 -->
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="备注说明">
          {{ PaymentrequestForm.remarks }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 费用信息 -->
      <el-tabs v-model="PaymentactiveTab" class="demo-tabs">
        <!-- 费用明细 -->
        <el-tab-pane label="费用明细" name="CostDetailsTab">
          <el-table :data="CostDetailsTbaleData" border>
            <el-table-column prop="relatedModules" label="关联模块" />
            <el-table-column prop="associatedOrderNumber" label="关联单号" />
            <el-table-column prop="applicationAmount" label="申请金额" />
            <el-table-column prop="relevantDates" label="关联日期" />
            <el-table-column prop="specificPaymentItems" label="具体款项" />
            <el-table-column prop="remark" label="备注" />
          </el-table>
        </el-tab-pane>

        <!-- 未支付款项详情 -->
        <el-tab-pane label="未支付款项详情" name="UnpaidDetailsTab">
          <el-table :data="UnpaidDetailsTbaleData" border>
            <el-table-column prop="contractofpurchaseNo" label="采购合同" />
            <el-table-column prop="contractdate" label="合同日期" />
            <el-table-column prop="relatedmodules" label="关联模块" />
            <el-table-column prop="exportcurrency" label="外销币种" />
            <el-table-column prop="exchangerate" label="汇率" />
            <el-table-column prop="amountspayable" label="应支付金额" />
            <el-table-column prop="depositpaid" label="已付定金" />
            <el-table-column prop="paymentrequested" label="已申请付款" />
            <el-table-column prop="nopaymentrequested" label="未申请付款" />
            <el-table-column prop="paymentpaid" label="已付货款" />
            <el-table-column prop="unpaiditems" label="未付货款" />
          </el-table>
        </el-tab-pane>

        <!-- 客诉索赔 -->
        <el-tab-pane label="客诉索赔" name="CustomerComplaintsTab">
        </el-tab-pane>
      </el-tabs>

      <!-- 底部按钮 -->
      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="ApproveReject">
            驳回
          </el-button>
          <el-button type="success" @click="Approvepass">
            通过
          </el-button>
        </span>
      </template>
    </el-dialog>
    <el-dialog v-model="ShippingDeliveryDialog" title="出运发货单审批" :close-on-click-modal=false style="width: 70%;">
      <!-- 基本信息 -->
      <span style="font-size: 20px; font-weight: bold;">基本信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="发票号码">
          {{ ShippingDeliveryForm.invoiceNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="制单日期">
          {{ ShippingDeliveryForm.OrderMakingDate }}
        </el-descriptions-item>

        <el-descriptions-item label="出运状态">
          {{ ShippingDeliveryForm.shippingStatus }}
        </el-descriptions-item>

        <el-descriptions-item label="出运日期">
          {{ ShippingDeliveryForm.shippingDate }}
        </el-descriptions-item>

        <el-descriptions-item label="发票日期">
          {{ ShippingDeliveryForm.invoiceDate }}
        </el-descriptions-item>

        <el-descriptions-item label="客户编号">
          {{ ShippingDeliveryForm.customerNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="客户简称">
          {{ ShippingDeliveryForm.customerAbbreviation }}
        </el-descriptions-item>

        <el-descriptions-item label="参考合同">
          {{ ShippingDeliveryForm.referenceContractNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="销售合同">
          {{ ShippingDeliveryForm.salesContractNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="客户合同">
          {{ ShippingDeliveryForm.customerContractNumber }}
        </el-descriptions-item>

        <el-descriptions-item label="我方公司">
          {{ ShippingDeliveryForm.ourCompany }}
        </el-descriptions-item>

        <el-descriptions-item label="收汇银行">
          {{ ShippingDeliveryForm.bankOfReceipt }}
        </el-descriptions-item>

        <el-descriptions-item label="外销币种">
          {{ ShippingDeliveryForm.exportCurrency }}
        </el-descriptions-item>

        <el-descriptions-item label="汇率">
          {{ ShippingDeliveryForm.exchangeRate }}
        </el-descriptions-item>

        <el-descriptions-item label="价格条款">
          {{ ShippingDeliveryForm.priceTerms }}
        </el-descriptions-item>

        <el-descriptions-item label="出运口岸">
          {{ ShippingDeliveryForm.departurePort }}
        </el-descriptions-item>

        <el-descriptions-item label="目的口岸">
          {{ ShippingDeliveryForm.destinationPort }}
        </el-descriptions-item>

        <el-descriptions-item label="贸易国别">
          {{ ShippingDeliveryForm.tradeCountry }}
        </el-descriptions-item>

        <el-descriptions-item label="结汇方式">
          {{ ShippingDeliveryForm.settlementMethod }}
        </el-descriptions-item>

        <el-descriptions-item label="运输方式">
          {{ ShippingDeliveryForm.transportationMethod }}
        </el-descriptions-item>

        <el-descriptions-item label="应收汇日">
          {{ ShippingDeliveryForm.receivableDate }}
        </el-descriptions-item>

        <el-descriptions-item label="单证员">
          {{ ShippingDeliveryForm.documentClerk }}
        </el-descriptions-item>

        <el-descriptions-item label="有无定金">
          <el-checkbox v-model="ShippingDeliveryForm.isDeposit" disabled />
        </el-descriptions-item>
      </el-descriptions>

      <!-- 辅助信息 -->
      <br><span style="font-size: 20px; font-weight: bold;">辅助信息</span>
      <el-divider></el-divider>

      <el-descriptions :column="3" border>
        <el-descriptions-item label="前程运输">
          {{ ShippingDeliveryForm.preCarriageTransport }}
        </el-descriptions-item>

        <el-descriptions-item label="船代公司">
          {{ ShippingDeliveryForm.shippingAgent }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 销售合同 -->
      <br><span style="font-size: 20px; font-weight: bold;">销售合同</span>
      <el-divider></el-divider>
      <el-table :data="shippingDeliveryContrctProductTableData">
        <el-table-column prop="contractNumber" label="销售合同" width="150"></el-table-column>
        <el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
        <el-table-column prop="customerCode" label="客户货号" width="150"></el-table-column>
        <el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
        <el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
        <el-table-column prop="shipmentQuantity" label="出货数量" width="150"></el-table-column>
        <el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
        <el-table-column prop="exportUnitPrice" label="外销单价" width="150"></el-table-column>
        <el-table-column prop="exportTotalPrice" label="外销总价" width="150"></el-table-column>
        <el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
        <el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
        <el-table-column prop="boxCount" label="箱数" width="150"></el-table-column>
        <el-table-column prop="outerBoxUnit" label="外箱单位" width="150"></el-table-column>
        <el-table-column prop="outerBoxLength" label="外箱长度" width="150"></el-table-column>
        <el-table-column prop="outerBoxWidth" label="外箱宽度" width="150"></el-table-column>
        <el-table-column prop="outerBoxHeight" label="外箱高度" width="150"></el-table-column>
        <el-table-column prop="outerBoxVolume" label="外箱体积" width="150"></el-table-column>
        <el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
        <el-table-column prop="outerBoxNetWeight" label="外箱净重" width="150"></el-table-column>
        <el-table-column prop="outerBoxGrossWeight" label="外箱毛重" width="150"></el-table-column>
        <el-table-column prop="totalNetWeight" label="总净重" width="150"></el-table-column>
        <el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
      </el-table>

      <!-- 采购合同 -->
      <br><span style="font-size: 20px; font-weight: bold;">采购合同</span>
      <el-divider></el-divider>
      <el-table :data="shippingDeliveryPurchaseDetailsTableData">
        <el-table-column prop="purchaseContractNumber" label="采购合同" width="150"></el-table-column>
        <el-table-column prop="vendorAbbreviation" label="厂商简称" width="150"></el-table-column>
        <el-table-column prop="productNumber" label="产品编号" width="150"></el-table-column>
        <el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
        <el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
        <el-table-column prop="shipmentQuantity" label="出货数量" width="150"></el-table-column>
        <el-table-column prop="purchaseCurrency" label="采购币种" width="150"></el-table-column>
        <el-table-column prop="purchaseUnitPrice" label="采购单价" width="150"></el-table-column>
        <el-table-column prop="purchaseTotalPrice" label="采购总价" width="150"></el-table-column>
        <el-table-column prop="measurementUnit" label="计量单位" width="150"></el-table-column>
        <el-table-column prop="invoice" label="是否开票" width="150"></el-table-column>
        <el-table-column prop="totalVolume" label="总体积" width="150"></el-table-column>
        <el-table-column prop="totalGrossWeight" label="总毛重" width="150"></el-table-column>
      </el-table>

      <!-- 备注信息 -->
      <br><span style="font-size: 20px; font-weight: bold;">备注信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="1" border>
        <el-descriptions-item label="备注">
          {{ ShippingDeliveryForm.remark }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 底部按钮 -->
      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="ApproveReject">
            驳回
          </el-button>
          <el-button type="success" @click="Approvepass">
            通过
          </el-button>
        </span>
      </template>
    </el-dialog>
    <el-dialog v-model="SettlementApprovalDialog" title="结算单审批" :close-on-click-modal="false" style="width: 70%;">
      <!-- 主要信息 -->
      <div style="margin-top: 30px;">
        <span style="font-size: 20px; font-weight: bold;">主要信息</span>
      </div>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="发票号码">
          {{ SettlementForm.invoiceNumber }}
        </el-descriptions-item>
        <el-descriptions-item label="出运日期">
          {{ SettlementForm.shippingDate }}
        </el-descriptions-item>
        <el-descriptions-item label="出运状态">
          {{ SettlementForm.shippingStatus }}
        </el-descriptions-item>
        <el-descriptions-item label="销售合同">
          {{ SettlementForm.salesContractNumber }}
        </el-descriptions-item>
        <el-descriptions-item label="客户合同">
          {{ SettlementForm.customerContract }}
        </el-descriptions-item>
        <el-descriptions-item label="客户名称">
          {{ SettlementForm.customerName }}
        </el-descriptions-item>
        <el-descriptions-item label="我方公司">
          {{ SettlementForm.ourCompany }}
        </el-descriptions-item>
        <el-descriptions-item label="外销币种">
          {{ SettlementForm.foreignCurrency }}
        </el-descriptions-item>
        <el-descriptions-item label="汇率">
          {{ SettlementForm.exchangeRate }}
        </el-descriptions-item>
        <el-descriptions-item label="价格条款">
          {{ SettlementForm.priceTerms }}
        </el-descriptions-item>
        <el-descriptions-item label="结汇方式">
          {{ SettlementForm.settlementMethod }}
        </el-descriptions-item>
        <el-descriptions-item label="应收汇日">
          {{ SettlementForm.receivableDate }}
        </el-descriptions-item>
        <el-descriptions-item label="销售员">
          {{ SettlementForm.salesClerk }}
        </el-descriptions-item>
        <el-descriptions-item label="单证员">
          {{ SettlementForm.documentClerk }}
        </el-descriptions-item>
        <el-descriptions-item label="是否结账">
          {{ SettlementForm.isSettlement }}
        </el-descriptions-item>
      </el-descriptions>


      <!-- 客户收汇 -->
      <div style="margin-top: 30px;">
        <span style="font-size: 20px; font-weight: bold;">客户收汇</span>
      </div>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="货值合计">
          {{ CustomerReceiptForm.totalValue }}
        </el-descriptions-item>
        <el-descriptions-item label="费用合计">
          {{ CustomerReceiptForm.totalCost }}
        </el-descriptions-item>
        <el-descriptions-item label="索赔金额">
          {{ CustomerReceiptForm.claimAmount }}
        </el-descriptions-item>
        <el-descriptions-item label="应收货款">
          {{ CustomerReceiptForm.totalReceivable }}
        </el-descriptions-item>
        <el-descriptions-item label="已收货款">
          {{ CustomerReceiptForm.totalReceived }}
        </el-descriptions-item>
        <el-descriptions-item label="未收货款">
          {{ CustomerReceiptForm.totalUnreceived }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 退税情况 -->
      <div style="margin-top: 30px;">
        <span style="font-size: 20px; font-weight: bold;">退税情况</span>
      </div>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="应退税金额">
          {{ TaxRefundForm.totalRefund }}
        </el-descriptions-item>
        <el-descriptions-item label="已退税金额">
          {{ TaxRefundForm.totalRefunded }}
        </el-descriptions-item>
        <el-descriptions-item label="未退税金额">
          {{ TaxRefundForm.totalUnrefunded }}
        </el-descriptions-item>
        <el-descriptions-item label="是否已退税">
          <el-checkbox v-model="TaxRefundForm.isRefunded" disabled />
        </el-descriptions-item>
      </el-descriptions>

      <!-- 国内费用 -->
      <div style="margin-top: 30px;">
        <span style="font-size: 20px; font-weight: bold;">国内费用</span>
      </div>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="前程运输">
          {{ DomesticFeesForm.transportation }}
        </el-descriptions-item>
        <el-descriptions-item label="应付运杂费￥">
          {{ DomesticFeesForm.totalPayable }}
        </el-descriptions-item>
        <el-descriptions-item label="已付运杂费">
          {{ DomesticFeesForm.totalPaid }}
        </el-descriptions-item>
        <el-descriptions-item label="未付运杂费">
          {{ DomesticFeesForm.totalUnpaid }}
        </el-descriptions-item>
        <el-descriptions-item label="快件费用￥">
          {{ DomesticFeesForm.expressFee }}
        </el-descriptions-item>
        <el-descriptions-item label="其它国内费用￥">
          {{ DomesticFeesForm.otherDomesticCost }}
        </el-descriptions-item>
      </el-descriptions>


      <!-- 国外费用 -->
      <div style="margin-top: 30px;">
        <span style="font-size: 20px; font-weight: bold;">国外费用</span>
      </div>
      <el-divider></el-divider>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="应付佣金">
          {{ ForeignExpensesForm.commissionPayable }}
        </el-descriptions-item>
        <el-descriptions-item label="已付佣金">
          {{ ForeignExpensesForm.commissionPaid }}
        </el-descriptions-item>
        <el-descriptions-item label="已付佣金￥">
          {{ ForeignExpensesForm.commissionPaidAmount }}
        </el-descriptions-item>
        <el-descriptions-item label="未付佣金">
          {{ ForeignExpensesForm.commissionUnpaid }}
        </el-descriptions-item>
        <el-descriptions-item label="船代公司">
          {{ ForeignExpensesForm.shippingAgentCompany }}
        </el-descriptions-item>
        <el-descriptions-item label="海运费币种">
          {{ ForeignExpensesForm.shippingCurrency }}
        </el-descriptions-item>
        <el-descriptions-item label="海运费汇率">
          {{ ForeignExpensesForm.shippingExchangeRate }}
        </el-descriptions-item>
        <el-descriptions-item label="应付海运">
          {{ ForeignExpensesForm.shippingPayable }}
        </el-descriptions-item>
        <el-descriptions-item label="已付海运">
          {{ ForeignExpensesForm.shippingPaid }}
        </el-descriptions-item>
        <el-descriptions-item label="已付海运￥">
          {{ ForeignExpensesForm.shippingPaidAmount }}
        </el-descriptions-item>
        <el-descriptions-item label="未付海运￥">
          {{ ForeignExpensesForm.shippingUnpaidAmount }}
        </el-descriptions-item>
        <el-descriptions-item label="其它国外费用￥">
          {{ ForeignExpensesForm.otherForeignCost }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 列表部分 -->
      <div style="margin-top: 30px;">
        <el-tabs v-model="settlementactiveTab">
          <!-- 工厂付款 -->
          <el-tab-pane label="工厂付款" name="factoryPayment">
            <el-table :data="FactoryPaymentData" border>
              <el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
              <el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
              <el-table-column prop="reviewStatusStr" label="审核状态" width="150">
                <template #default="scope">
                  <el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
                    {{ scope.row.reviewStatusStr }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="paymentCategory" label="付款类别" width="150"></el-table-column>
              <el-table-column prop="paymentName" label="款项名称" width="150"></el-table-column>
              <el-table-column prop="payeeName" label="收款单位名称" width="150"></el-table-column>
              <el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
              <el-table-column prop="bankAccount" label="银行账号" width="150"></el-table-column>
              <el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
              <el-table-column prop="currencyCode" label="货币代码" width="150"></el-table-column>
              <el-table-column prop="totalAmount" label="申请总额" width="150"></el-table-column>
              <el-table-column prop="paidAmount" label="已付金额" width="150"></el-table-column>
              <el-table-column prop="unpaidAmount" label="未付金额" width="150"></el-table-column>
              <el-table-column prop="applicant" label="申请人" width="150"></el-table-column>
              <el-table-column prop="applicationDepartment" label="申请部门" width="150"></el-table-column>
              <el-table-column prop="handler" label="经手人" width="150"></el-table-column>
              <el-table-column prop="applicationDate" label="申请日期" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>

          <!-- 采购其它费用 -->
          <el-tab-pane label="采购其它费用" name="purchaseOtherCosts">
            <el-table :data="PurchaseOtherCostsData" border>
              <el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
              <el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
              <el-table-column prop="reviewStatusStr" label="审核状态" width="150">
                <template #default="scope">
                  <el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
                    {{ scope.row.reviewStatusStr }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="paymentCategory" label="付款类别" width="150"></el-table-column>
              <el-table-column prop="paymentName" label="款项名称" width="150"></el-table-column>
              <el-table-column prop="payeeName" label="收款单位名称" width="150"></el-table-column>
              <el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
              <el-table-column prop="bankAccount" label="银行账号" width="150"></el-table-column>
              <el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
              <el-table-column prop="currencyCode" label="货币代码" width="150"></el-table-column>
              <el-table-column prop="totalAmount" label="申请总额" width="150"></el-table-column>
              <el-table-column prop="paidAmount" label="已付金额" width="150"></el-table-column>
              <el-table-column prop="unpaidAmount" label="未付金额" width="150"></el-table-column>
              <el-table-column prop="applicant" label="申请人" width="150"></el-table-column>
              <el-table-column prop="applicationDepartment" label="申请部门" width="150"></el-table-column>
              <el-table-column prop="handler" label="经手人" width="150"></el-table-column>
              <el-table-column prop="applicationDate" label="申请日期" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>

          <!-- 客户已收汇详情 -->
          <el-tab-pane label="客户已收汇详情" name="customerReceipts">
            <el-table :data="CustomerReceiptsData" border>
              <el-table-column prop="ReceiptNumber" label="收汇单号" width="150"></el-table-column>
              <el-table-column prop="ReceiptBank" label="收汇银行" width="150"></el-table-column>
              <el-table-column prop="ReceiptDate" label="收汇日期" width="150"></el-table-column>
              <el-table-column prop="OurCompany" label="我方公司" width="150"></el-table-column>
              <el-table-column prop="FundsType" label="款项类型" width="150"></el-table-column>
              <el-table-column prop="ReceiptAmount" label="收汇金额" width="150"></el-table-column>
              <el-table-column prop="ReceiptCurrency" label="收汇币种" width="150"></el-table-column>
              <el-table-column prop="ExchangeRate" label="汇率" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>

          <!-- 退税详情 -->
          <el-tab-pane label="退税详情" name="taxRefundDetails">
            <el-table :data="TaxRefundDetailsData" border>
              <el-table-column prop="ReceiptNumber" label="退税单编号" width="150"></el-table-column>
              <el-table-column prop="InvoiceNumber" label="发票号" width="150"></el-table-column>
              <el-table-column prop="RefundAmount" label="应退税额" width="150"></el-table-column>
              <el-table-column prop="ActualRefundAmount" label="实际退税额" width="150"></el-table-column>
              <el-table-column prop="IsRefunded" label="是否已退" width="150"></el-table-column>
              <el-table-column prop="RefundDate" label="退税日期" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>

          <!-- 国内已付费用详情 -->
          <el-tab-pane label="国内已付费用详情" name="domesticPayments">
            <el-table :data="DomesticPaymentsData" border>
              <el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
              <el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
              <el-table-column prop="reviewStatusStr" label="审核状态" width="150">
                <template #default="scope">
                  <el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
                    {{ scope.row.reviewStatusStr }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="paymentCategory" label="付款类别" width="150"></el-table-column>
              <el-table-column prop="paymentName" label="款项名称" width="150"></el-table-column>
              <el-table-column prop="payeeName" label="收款单位名称" width="150"></el-table-column>
              <el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
              <el-table-column prop="bankAccount" label="银行账号" width="150"></el-table-column>
              <el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
              <el-table-column prop="currencyCode" label="货币代码" width="150"></el-table-column>
              <el-table-column prop="totalAmount" label="申请总额" width="150"></el-table-column>
              <el-table-column prop="paidAmount" label="已付金额" width="150"></el-table-column>
              <el-table-column prop="unpaidAmount" label="未付金额" width="150"></el-table-column>
              <el-table-column prop="applicant" label="申请人" width="150"></el-table-column>
              <el-table-column prop="applicationDepartment" label="申请部门" width="150"></el-table-column>
              <el-table-column prop="handler" label="经手人" width="150"></el-table-column>
              <el-table-column prop="applicationDate" label="申请日期" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>

          <!-- 国外已付费用详情 -->
          <el-tab-pane label="国外已付费用详情" name="internationalPayments">
            <el-table :data="InternationalPaymentsData" border>
              <el-table-column prop="applicationNumber" label="申请单号" width="150"></el-table-column>
              <el-table-column prop="reviewStatus" label="审核状态Index" width="150" v-if="false"></el-table-column>
              <el-table-column prop="reviewStatusStr" label="审核状态" width="150">
                <template #default="scope">
                  <el-tag :type="getStatusType(scope.row.reviewStatus)" effect="plain">
                    {{ scope.row.reviewStatusStr }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="paymentCategory" label="付款类别" width="150"></el-table-column>
              <el-table-column prop="paymentName" label="款项名称" width="150"></el-table-column>
              <el-table-column prop="payeeName" label="收款单位名称" width="150"></el-table-column>
              <el-table-column prop="bankName" label="开户银行" width="150"></el-table-column>
              <el-table-column prop="bankAccount" label="银行账号" width="150"></el-table-column>
              <el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
              <el-table-column prop="currencyCode" label="货币代码" width="150"></el-table-column>
              <el-table-column prop="totalAmount" label="申请总额" width="150"></el-table-column>
              <el-table-column prop="paidAmount" label="已付金额" width="150"></el-table-column>
              <el-table-column prop="unpaidAmount" label="未付金额" width="150"></el-table-column>
              <el-table-column prop="applicant" label="申请人" width="150"></el-table-column>
              <el-table-column prop="applicationDepartment" label="申请部门" width="150"></el-table-column>
              <el-table-column prop="handler" label="经手人" width="150"></el-table-column>
              <el-table-column prop="applicationDate" label="申请日期" width="150"></el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </div>

      <!-- 底部操作按钮 -->
      <template #footer>
        <span class="dialog-footer">
          <el-button type="danger" @click="ApproveReject">驳回</el-button>
          <el-button type="success" @click="Approvepass">通过</el-button>
        </span>
      </template>
    </el-dialog>
    <!-- 领取收款单对话框 -->
    <el-dialog v-model="claimDialogVisible" title="领取收款单" width="500px" :close-on-click-modal="false">
      <el-form ref="claimFormRef" :model="claimForm" :rules="claimRules" label-width="100px">
        <el-form-item label="合同类型" prop="contractType">
          <el-select v-model="claimForm.contractType" placeholder="请选择合同类型" style="width: 100%" filterable
            @change="relatedmoduleshandleChange(claimForm.contractType)">
            <el-option v-for="item in state.optionss.hr_payment_contract_type" :key="item.dictValue"
              :label="item.dictLabel" :value="item.dictValue" />
          </el-select>
        </el-form-item>

        <el-form-item label="合同号" prop="contractsID">
          <el-select v-model="claimForm.contractsID" placeholder="请选择合同号" style="width: 100%" filterable>
            <el-option v-for="item in AssociatedOrderNumberOptions" :key="item.dictValue" :label="item.dictLabel"
              :value="item.dictValue" />
          </el-select>
        </el-form-item>

        <el-form-item label="款项类型" prop="fundsType">
          <el-select v-model="claimForm.fundsType" placeholder="请选择款项类型" style="width: 100%">
            <el-option v-for="dict in state.optionss.funds_type" :key="dict.dictValue" :label="dict.dictLabel"
              :value="dict.dictValue" />
          </el-select>
        </el-form-item>

        <el-form-item label="备注" prop="remark">
          <el-input v-model="claimForm.remark" type="textarea" :rows="3" placeholder="请输入备注信息" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="claimDialogVisible = false">取 消</el-button>
          <el-button type="primary" @click="submitClaim">确 定</el-button>
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
            :on-remove="handleCompletionFileRemove" :file-list="completionFileList" multiple style="width: 100%">
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
          <el-button @click="confirmDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="confirmTaskCompletion">
            确认完成
          </el-button>
        </span>
      </template>
    </el-dialog>
    <!-- 逾期交货合同 -->
    <el-dialog v-model="overdueDeliveryContractDialogVisible" title="逾期交货合同" width="600px"
      :close-on-click-modal="false">
      <el-table :data="overdueDeliveryContractData">
        <el-table-column prop="contractNumber" label="合同号" width="150"></el-table-column>
        <el-table-column prop="originalDeliveryDate" label="原交货日期" width="150"></el-table-column>
        <el-table-column prop="overdueDays" label="已超期天数" width="150"></el-table-column>
        <el-table-column fixed="right" label="操作" style="width: 25%;">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleModifyDeliveryDate(row)">申请修改交货日期</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
    <!-- 沟通逾期 -->
    <el-dialog v-model="communicationOverdueDialogVisible" title="沟通逾期" width="500px" :close-on-click-modal="false">
      <el-table :data="communicationOverdueData" @row-dblclick="handleCommunicationRowDblClick">
        <el-table-column prop="ID" label="客户编号" width="150" v-if="false"></el-table-column>
        <el-table-column prop="customerNo" label="客户编号" width="150"></el-table-column>
        <el-table-column prop="customerName" label="客户名称" width="150"></el-table-column>
        <el-table-column prop="daysSinceLastContact" label="未沟通天数" width="150"></el-table-column>
      </el-table>
    </el-dialog>
    <!-- 货款逾期列表 -->
    <el-dialog v-model="overdueContractsDialogVisible" title="货款逾期列表" width="800px">
      <el-table :data="overdueContractsData">
        <el-table-column prop="id" label="出运单id" width="100" v-if="false" />
        <el-table-column prop="contractName" label="出运单号" width="100" />
        <el-table-column prop="receivableDate" label="应收汇日" width="150" />
        <el-table-column prop="totalAmount" label="出运单金额" width="150" />
        <el-table-column prop="totalReceivedAmount" label="已收金额" width="100" />
        <el-table-column prop="overdueDays" label="逾期天数" width="100" />
        <el-table-column fixed="right" label="操作" style="width: 25%; ">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleModifyPaymentDate(row)">申请修改收款日期</el-button>
          </template>
        </el-table-column>
        <!-- 其他需要显示的列 -->
      </el-table>
    </el-dialog>
    <el-dialog v-model="pendingEmailDialogVisible" title="待处理邮件" :close-on-click-modal="false" class="custom-dialog"
      width="700px">
      <el-table :data="pendingEmailList" :height="400" style="width: 100%">
        <el-table-column prop="customerName" label="客户名称" width="200"></el-table-column>
        <el-table-column prop="emailsubject" label="邮件标题" width="200"></el-table-column>
        <el-table-column prop="create_time" label="时间" width="250">
          <template #default="scope">
            {{ scope.row.createTime }}
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
    <el-dialog v-model="overdueEmailDialogVisible" title="超时未处理邮件" :close-on-click-modal="false" class="custom-dialog"
      width="700px">
      <el-table :data="overdueEmailList" :height="400" style="width: 100%"
        @row-dblclick="handleOverdueEmailRowDblClick">
        <el-table-column prop="customerName" label="客户名称" width="200"></el-table-column>
        <el-table-column prop="emailsubject" label="邮件标题" width="200"></el-table-column>
        <el-table-column prop="create_time" label="时间" width="250">
          <template #default="scope">
            {{ scope.row.createTime }}
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
  </div>

</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref, onMounted, h } from 'vue'
import { ElMessage, ElMessageBox, ElDatePicker, ElLoading } from "element-plus";
import request from '@/utils/request';
import dayjs from 'dayjs';
import useUserStore from '@/store/modules/user'
import useSocketStore from '@/store/modules/socket'
// 时间插件
import duration from 'dayjs/plugin/duration'
import { Picture } from '@element-plus/icons-vue'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import { useRouter } from 'vue-router'
import { eventBus } from '@/utils/eventBus'


// #region 商机看板

// 文本截断方法
const truncateText = (text, length) => {
  if (!text) return ''
  if (text.length <= length) return text
  return text.slice(0, length) + '...'
}

function getBusinessDashboard() {
  return request({
    url: 'BusinessOpportunity/GetBusinessOpportunityDashboardDataByUser/GetDashboardData',
    method: 'get'
  })
}
// 定义固定的阶段顺序
const stageOrder = ['询盘', '初次报价', '沟通需求', '再次报价', '合同确定']

// 根据固定顺序排序并补充缺失的阶段
const sortedStages = computed(() => {
  return stageOrder.map(stageName => {
    const existingStage = businessStages.value.find(s => s.salesStage === stageName)
    return existingStage || {
      salesStage: stageName,
      count: 0,
      details: [],
      totalAmount: 0
    }
  })
})
const businessStages = ref([])

// 判断是否显示金额为0
const shouldShowZeroAmount = (stage) => {
  return ['询盘', '沟通需求'].includes(stage)
}

// 判断是否显示金额
const shouldShowAmount = (stage) => {
  return ['初次报价', '再次报价', '合同确定'].includes(stage)
}

// 格式化金额显示
const formatAmount = (amount) => {
  if (amount === null || amount === undefined) {
    return '0.00'
  }
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// 获取看板数据
const fetchDashboardData = async () => {
  try {
    const response = await getBusinessDashboard()
    if (response.code === 200) {
      const stages = response.data.map(stageData => ({
        salesStage: stageData.salesStage,
        count: stageData.count,
        details: stageData.details,
        totalAmount: shouldShowAmount(stageData.salesStage)
          ? stageData.details.reduce((sum, item) => sum + (item.amount || 0), 0)
          : 0
      }))

      businessStages.value = stages
    }
  } catch (error) {
    console.error('获取看板数据失败:', error)
  }
}

// #endregion

// 添加当前日期的响应式引用
const currentDate = ref(new Date())

// 添加判断是否为今天的方法
const isToday = (dateString) => {
  const today = new Date()
  const date = new Date(dateString)
  return date.getDate() === today.getDate() &&
    date.getMonth() === today.getMonth() &&
    date.getFullYear() === today.getFullYear()
}

// #region 财务收款单相关
// 对话框显示控制
const claimDialogVisible = ref(false)
const claimFormRef = ref(null)

// 表单数据
const claimForm = reactive({
  id: '', // 收款单ID
  contractsID: '', // 合同号
  fundsType: '', // 款项类型
  remark: '', // 备注
  contractType: '' // 合同类型
})

// 表单验证规则
const claimRules = {
  contractType: [
    { required: true, message: '请选择合同类型', trigger: 'change' }
  ],
  contractsID: [
    { required: true, message: '请选择合同号', trigger: 'change' }
  ],
  fundsType: [
    { required: true, message: '请选择款项类型', trigger: 'change' }
  ]
}

// 打开领取对话框
const handleClaim = (row) => {
  claimForm.id = row.id
  claimDialogVisible.value = true
}

const AssociatedOrderNumberOptions = ref([])
const relatedmoduleshandleChange = (type) => {
  claimForm.contractsID = '';
  switch (type) {
    case '1':
      AssociatedOrderNumberOptions.value = state.optionss.sql_purchase_contract;
      break;
    case '2':
      AssociatedOrderNumberOptions.value = state.optionss.sql_sale_contracts;
      break;
    case '3':
      AssociatedOrderNumberOptions.value = state.optionss.sql_shippingdeliveries;
      break;
    default:
      AssociatedOrderNumberOptions.value = [];
      break;
  }
}

// 提交领取
const submitClaim = () => {
  claimFormRef.value?.validate(async (valid) => {
    if (valid) {
      try {
        const response = await request({
          url: 'CustomerCollections/AssigningCustomerCollections/Assigning',
          method: 'post',
          data: {
            id: claimForm.id,
            ContractsID: claimForm.contractsID,
            FundsType: claimForm.fundsType,
            Remark: claimForm.remark,
            ContractType: claimForm.contractType
          }
        })

        if (response.code === 200) {
          ElMessage.success('领取成功')
          claimDialogVisible.value = false
          // 重新加载列表
          getFinancialTasksList(
            FinancialTasksTableCurrentPage.value,
            FinancialTasksTablePageSize.value
          )
        } else {
          ElMessage.error(response.msg || '领取失败')
        }
      } catch (error) {
        console.error('领取失败:', error)
        ElMessage.error('领取失败')
      }
    }
  })
}

// 对话框关闭时重置表单
const resetClaimForm = () => {
  claimForm.contractsID = ''
  claimForm.fundsType = ''
  claimForm.remark = ''
  claimForm.contractType = ''
  claimFormRef.value?.resetFields()
}

// 监听对话框关闭
watch(claimDialogVisible, (val) => {
  if (!val) {
    resetClaimForm()
  }
})


//分页组件
const FinancialTasksTableTotalItems = ref(0);
const FinancialTasksTableCurrentPage = ref(1);
const FinancialTasksTablePageSize = ref(10);
//收款单据表格
const FinancialTasksTableData = ref([])
// 分页变化处理函数
const FinancialTasksTableshandlePageChange = (newPage) => {
  getFinancialTasksList(newPage, FinancialTasksTablePageSize.value);
};
// 日期格式化函数
const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

// 获取财务任务列表
const getFinancialTasksList = (start, end) => {
  request({
    url: 'CustomerCollections/GetUnassignedCustomerCollectionsList/GetList',
    method: 'GET',
    params: {
      PageNum: start,
      PageSize: end
    }
  }).then(response => {
    if (response.data.result.length > 0) {
      FinancialTasksTableData.value = response.data.result;
      FinancialTasksTableData.value.forEach(item => {
        item.receiptDate = formatDate(item.receiptDate);
        item.ourCompany = state.optionss.hr_ourcompany.find(dict => dict.dictValue === item.ourCompany)?.dictLabel;
        item.foreignCurrency = state.optionss.hr_export_currency.find(dict => dict.dictValue === item.foreignCurrency)?.dictLabel;
        item.bank = state.optionss.hr_bank.find(dict => dict.dictValue === item.bank)?.dictLabel;
      });
      // 更新分页信息
      FinancialTasksTableTotalItems.value = response.data.totalNum;
      FinancialTasksTableCurrentPage.value = response.data.pageIndex;
      FinancialTasksTablePageSize.value = response.data.pageSize;
    }
  }).catch(error => {
    console.error('获取列表数据失败:', error);
    ElMessage.error('获取列表数据失败');
  });
};

// #endregion

//销售合同产品表格
const productData = ref([]);

const ApproveDocumentRequest = reactive({
  ApprovalRecordID: 0,
  DocumentType: 0,
  DocumentID: 0,
  StageID: 0,
  ApproverID: 0,
  ApproveStatus: false
});
const Approvepass = () => {
  ApproveDocumentRequest.ApproveStatus = true;
  request.post('ApprovalFlow/ApprovalDocument/ApprovalDocument', ApproveDocumentRequest).then(response => {
    if (response != null) {
      ElMessage({
        message: response.data,
        type: 'success'
      })
      contractDialog.value = false;
      PurchaseContractDialog.value = false;
      PaymentrequestDialog.value = false;
      ShippingDeliveryDialog.value = false;
      SettlementApprovalDialog.value = false;
      getPendingCount();
    } else {
      console.error('审批失败');
    }
  }).catch(error => {
    console.error('审批失败', error);
  })
}

const ApproveReject = () => {
  ApproveDocumentRequest.ApproveStatus = false;
  request.post('ApprovalFlow/ApprovalDocument/ApprovalDocument', ApproveDocumentRequest).then(response => {
    if (response != null) {
      ElMessage({
        message: response.data,
        type: 'success'
      })
      contractDialog.value = false;
      PurchaseContractDialog.value = false;
      PaymentrequestDialog.value = false;
      ShippingDeliveryDialog.value = false;
      SettlementApprovalDialog.value = false;
      getPendingCount();
    } else {
      console.error('驳回失败');
    }
  }).catch(error => {
    console.error('审批失败', error);
  })
}

//联系人
const contactpersonSelectOptions = ref([]);

//产品资料表格
const NewcontractProductTbaleData = ref([]);
const DepositShow = ref(false);
//产品资料tab&客户相关费用tab
const activeTab = ref('productMaterialtab');
const CustomerRelaterExoensesTableData = ref([]);
const contractDialog = ref(false);
const contractform = reactive({
  id: 0,                     // 合同ID
  contractNumber: '',        // 合同编号
  contractDate: '',         // 合同日期
  contractStatus: null,     // 合同状态
  customerid: null,         // 客户ID
  customerNumber: '',       // 客户编号
  customerAbbreviation: '', // 客户简称
  contactPerson: null,      // 联系人
  contactEmail: '',         // 联系人邮箱
  effectiveDate: '',        // 生效日期
  customerLevel: null,      // 客户等级
  customerContract: '',     // 客户合同
  deliveryDate: '',         // 交货日期
  ourCompany: null,         // 我方公司
  settlementType: null,     // 结算类别
  foreignCurrency: null,    // 外销币种
  exchangeRate: null,       // 汇率
  usdExchangeRate: null,    // 美金汇率
  settlementMethod: null,   // 结汇方式
  priceTerms: null,         // 价格条款
  shippingPort: null,       // 出运口岸
  destinationPort: null,    // 目的口岸
  tradeCountry: null,       // 贸易国别
  transportation: null,     // 运输方式
  salesperson: null,        // 销售员
  hasDeposit: false,        // 是否有定金
  receivedDeposit: null,    // 已收定金
  depositDate: '',          // 定金日期
  Depositratio: null,       // 定金比例
  stockProgress: '',        // 备货进度
  deliveryProgress: '',     // 交货进度
  profitCalculation: '',    // 利润计算
  estimatedProfitMargin: '', // 预估利润率
  customerOrder: '',        // 客户订单
  contractremark: '',       // 合同备注
  signingLocation: null,    // 签约地点
  canPartial: null,         // 是否可分批
  canTransit: null,         // 是否可转运
  insuranceAddition: null,  // 保险附加
  insuranceRate: null,      // 保险比率
  commissionRate: null,     // 佣金比率
  receivingBank: '',        // 收汇银行
  paymentDate: '',          // 付款日期
  oceanFreight: null,       // 海运费
  portMiscellaneousFees: null, // 港杂费
  inlandFreight: null,      // 内陆运费
  singleCubicCost: null,    // 个立方费用
  freightForwarderCustomsClearanceFees: null, // 货代报关杂费
  TotalValueOfGoods: null,  // 货值合计
  TotalQuantity: null,      // 数量合计
  TotalNumberOfBoxes: null, // 箱数合计
  TotalGrossWeight: null,   // 毛重合计
  TotalNetWeight: null,     // 净重合计
  TotalVolume: null,        // 体积合计
  TotalPurchases: null,     // 采购合计
  TotalTaxRefund: null,     // 退税总额
  customerExpenseTotal: null, // 客户费用合计
  amountTotal: null,        // 金额合计
  usdConversion: null,      // 美金/欧元换算
  commissionAmount: null,    // 佣金金额
  shippingCurrency: null,    // 海运费币种
  shippingrate: null,       // 海运费汇率
  shippingCost: null,       // 海运费
  insuranceCost: null,      // 保险费用
  BankFee: null,            // 银行费用
  TotalOtherFees: null,     // 其它费用合计
  freightCost: null,        // 运杂费
  totalCost: null,          // 成本总额
  exchangeCost: null,       // 换汇成本
  productProfit: null,      // 产品毛利
  ProfitAmount: null,       // 利润金额
  profitRate: null,         // 利润率
  FreightCurrency: null,    // 运杂费币种
  FreightQuote: null,       // 运杂费报价
  Courierfee: null,         // 快递费
  Totalgrossprofit: null,   // 总毛利合计
  Totalprofitmargin: null,  // 总利润率
  DocumentationFees: null,  // 文件杂费
  goodsValue: null,         // 货值
  quantity: null,           // 数量
  boxCount: null,           // 箱数
  grossWeight: null,        // 毛重
  netWeight: null,          // 净重
  volume: null,              // 体积
  NewdeliveryDate: null      // 新交货日期
});
//采购合同审批对话框
const PurchaseContractDialog = ref(false);
const PurchaseContractDialogData = ref({
  purchaseContract: '',
  contractStatus: '',
  deliveryDate: '',
  vendorCode: '',
  purchaseCurrency: '',
  deposit: '',
  salesperson: '',
  purchaser: '',
  priceTerms: '',
  paymentDays: '',
  activeName: 'productinfo',
  productinfotableData: [],
  CustomerRelaterExoensesTableData: [],
  TotalValueOfGoods: '',
  TotalQuantity: '',
  TotalNumberOfBoxes: '',
  TotalGrossWeight: '',
  TotalNetWeight: '',
  TotalVolume: '',
  TotalPurchases: '',
  TotalTaxRefund: '',
  customerExpenseTotal: '',
  appliedPayment: '',
  availablePayment: '',
  unpaidAmount: '',
  paidAmount: ''
});

dayjs.extend(duration)
dayjs.locale('zh-cn')

//获取客户联系人
const GetCustomerContactPerson = (CustomerId) => {
  request({
    url: 'CustomerInfoMation/getCustomerInfoByID/GetCustomerInfo',
    method: 'GET',
    params: {
      ID: CustomerId
    }
  }).then(response => {
    if (response != null) {
      if (response.contactPerson != null) {
        contactpersonSelectOptions.value = response.contactPerson.map(item => ({
          value: item.id,
          label: item.name,
          email: item.email
        }));
      }
    } else {
      ElMessage.error("获取客户联系人失败!");
    }
  }).catch(error => {
    console.log(error)
  });
}
const PaymentactiveTab = ref('CostDetailsTab')//费用信息Tab
const settlementactiveTab = ref('factoryPayment')//结算单Tab
// 审核状态转换函数
const getReviewStatusText = (status) => {
  const statusMap = {
    0: '待提审',
    1: '审核中',
    2: '已批准',
    3: '已拒绝'
  }
  return statusMap[status] || '未知状态'
}
// 获取状态对应的标签类型
const getStatusType = (status) => {
  const typeMap = {
    0: 'info',    // 待提审 - 灰色
    1: 'warning', // 审核中 - 黄色
    2: 'success', // 已批准 - 绿色
    3: 'danger'   // 已拒绝 - 红色
  }
  return typeMap[status] || 'info'
}
const openSaleContractDialog = (row) => {
  ApproveDocumentRequest.ApprovalRecordID = row.recordID;
  ApproveDocumentRequest.DocumentID = row.documentID;
  ApproveDocumentRequest.StageID = row.stageID;
  ApproveDocumentRequest.ApproverID = row.approverID;
  ApproveDocumentRequest.DocumentType = row.documentType;
  //获取销售合同详情
  if (row.documentType == "1" || row.documentType == "7") {
    request({
      url: 'Contracts/GetContractDetailsById/GetContractDetails',
      method: 'GET',
      params: {
        contractId: row.documentID
      }
    }).then(response => {
      if (response.data != null) {
        if (row.documentType == "7") {
          ElMessageBox.alert('该合同申请将交货日期修改为：' + response.data.contract.newDeliveryDate + '，请确认！', '交货日期修改申请', {
            confirmButtonText: 'OK'
          })
        }
        /*表单赋值*/
        contractform.id = row.documentID;
        contractform.contractNumber = response.data.contract.contractNumber;
        contractform.contractDate = response.data.contract.contractDate;
        contractform.contractStatus = state.optionss['hr_contract_status'].find(item => item.dictValue === response.data.contract.contractStatus.toString()).dictValue;
        contractform.customerid = response.data.contract.customerId;
        contractform.customerNumber = state.optionss['sql_hr_customer'].find(item => item.dictValue === response.data.contract.customerId.toString()).dictValue;
        contractform.customerAbbreviation = state.optionss['sql_hr_customer'].find(item => item.dictValue === response.data.contract.customerId.toString()).dictValue;
        GetCustomerContactPerson(response.data.contract.customerId);
        contractform.contactPerson = response.data.contract.contactPerson;
        contractform.contactEmail = response.data.contract.contactEmail;
        contractform.effectiveDate = response.data.contract.effectiveDate;
        contractform.customerLevel = state.optionss['hr_customer_level'].find(item => item.dictValue === response.data.contract.customerLevel.toString()).dictValue;
        contractform.customerContract = response.data.contract.customerContract;
        contractform.deliveryDate = response.data.contract.deliveryDate;
        contractform.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictValue === response.data.contract.ourCompany.toString()).dictValue;
        contractform.settlementType = response.data.contract.settlementType.toString();
        contractform.foreignCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === response.data.contract.foreignCurrency.toString()).dictValue;
        contractform.exchangeRate = response.data.contract.exchangeRate;
        contractform.usdExchangeRate = response.data.contract.usdExchangeRate;
        contractform.settlementMethod = state.optionss['hr_settlement_way'].find(item => item.dictValue === response.data.contract.settlementMethod.toString()).dictValue;
        contractform.priceTerms = state.optionss['hr_pricing_term'].find(item => item.dictValue === response.data.contract.priceTerms.toString()).dictValue;
        contractform.shippingPort = state.optionss['hr_transport_port'].find(item => item.dictValue === response.data.contract.shippingPort.toString()).dictValue;
        contractform.destinationPort = response.data.contract.destinationPort;
        contractform.tradeCountry = state.optionss['hr_nation'].find(item => item.dictValue === response.data.contract.tradeCountry.toString()).dictValue;
        contractform.transportation = state.optionss['hr_transportation_method'].find(item => item.dictValue === response.data.contract.transportation.toString()).dictValue;
        contractform.salesperson = state.optionss['sql_hr_sale'].find(item => item.dictValue === response.data.contract.salesperson.toString()).dictValue;
        contractform.hasDeposit = response.data.contract.hasDeposit;
        if (response.data.contract.hasDeposit) {
          DepositShow.value = true;
          contractform.receivedDeposit = response.data.contract.receivedDeposit;
          contractform.depositDate = response.data.contract.depositDate;
          contractform.Depositratio = response.data.contract.depositratio;
        } else {
          DepositShow.value = false;
        }
        contractform.stockProgress = response.data.contract.stockProgress;
        contractform.deliveryProgress = response.data.contract.deliveryProgress;
        contractform.profitCalculation = response.data.contract.profitCalculation;
        contractform.estimatedProfitMargin = response.data.contract.estimatedProfitMargin;
        contractform.customerOrder = response.data.contract.customerOrder;
        contractform.contractremark = response.data.contract.remark;
        //contractform.signingLocation = state.optionss['hr_signing_place'].find(item => item.dictValue === response.data.signingLocation.toString()).dictValue;
        contractform.canPartial = state.optionss['sys_yes_no'].find(item => item.dictValue === (response.data.contract.canPartial.toString() == "true" ? "Y" : "N")).dictValue;
        contractform.canTransit = state.optionss['sys_yes_no'].find(item => item.dictValue === (response.data.contract.canTransit.toString() == "true" ? "Y" : "N")).dictValue;
        contractform.insuranceAddition = response.data.contract.insuranceAddition;
        contractform.insuranceRate = response.data.contract.insuranceRate;
        contractform.commissionRate = response.data.contract.commissionRate;
        contractform.receivingBank = response.data.contract.receivingBank;
        contractform.goodsValue = response.data.contract.goodsValue;
        contractform.oceanFreight = response.data.contract.shippingCost;
        contractform.shippingCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === response.data.contract.shippingCurrency.toString()).dictValue;
        contractform.shippingrate = response.data.contract.shippingExchangeRate;
        contractform.portMiscellaneousFees = response.data.contract.portMiscellaneousFees;
        contractform.freightForwarderCustomsClearanceFees = response.data.contract.freightForwarderCustomsClearanceFees;
        contractform.BankFee = response.data.contract.bankCost;
        contractform.DocumentationFees = response.data.contract.documentationFees;
        contractform.paymentDate = response.data.contract.paymentDate;
        contractform.TotalValueOfGoods = response.data.contract.goodsValue;
        contractform.TotalQuantity = response.data.contract.quantity;
        contractform.TotalNumberOfBoxes = response.data.contract.boxCount;
        contractform.TotalGrossWeight = response.data.contract.grossWeight;
        contractform.TotalNetWeight = response.data.contract.netWeight;
        contractform.TotalVolume = response.data.contract.volume;
        contractform.TotalPurchases = response.data.contract.purchaseTotal;
        contractform.TotalTaxRefund = response.data.contract.taxRefundTotal;
        contractform.customerExpenseTotal = response.data.contract.customerExpenseTotal;
        contractform.amountTotal = response.data.contract.amountTotal;
        contractform.usdConversion = response.data.contract.usdConversion;
        contractform.commissionAmount = response.data.contract.commissionAmount;
        contractform.Totalgrossprofit = response.data.contract.totalgrossprofit;
        contractform.TotalOtherFees = response.data.contract.totalOtherFees;
        contractform.ProfitAmount = response.data.contract.profitAmount;
        contractform.Totalprofitmargin = response.data.contract.totalprofitmargin;
        /*合同产品信息与相关费用*/
        return new Promise((resolve, reject) => {
          request({
            url: 'Contracts/GetContractDetailsById/GetContractDetails',
            method: 'GET',
            params: {
              contractId: row.documentID
            }
          }).then(response => {
            if (response.data.contractProducts.length > 0) {
              productData.value = [];
              response.data.contractProducts.forEach(element => {
                productData.value.push({
                  Id: element.id,
                  ProductID: element.productID,
                  ContractId: element.contractId,
                  productNum: element.productCode,
                  CustomerCode: element.customerNum,
                  cproductname: element.chineseName,
                  cspecification: element.chineseSpec,
                  contractQuantity: element.contractQuantity,
                  exportunitprice: element.exportUnitPrice,
                  exporttotalprice: element.exportTotalPrice,
                  unitofmeasurement: state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.unit.toString()).dictValue,
                  purchasecurrency: state.optionss['hr_export_currency'].find(item => item.dictValue === element.purchasecurrency.toString()).dictValue,
                  purchaseunitprice: element.purchaseUnitPrice,
                  inlandfreightprice: element.inlandfreightprice,
                  AdditionalPackagingCosts: element.additionalPackagingCosts,
                  singleProductGrossProfit: element.singleProductGrossProfit,
                  singleProductGrossProfitTotal: element.singleProductGrossProfitTotal,
                  grossProfitRate: element.grossProfitRate,
                  isInvoicingc: element.invoice == 0 ? "2" : state.optionss['hr_yes_no'].find(item => item.dictValue === element.invoice.toString()).dictValue,
                  packaging: state.optionss['hr_packing'].find(item => item.dictValue === element.packaging.toString()).dictValue,
                  specialrequirements: element.specialRequirements,
                  rebaterate: element.taxRefundRate,
                  innerBoxLoading: element.innerBoxQuantity,
                  outerboxloading: element.outerBoxQuantity,
                  outerboxunit: state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.outerboxunit.toString()).dictValue,
                  outerboxlength: element.outerBoxLength,
                  outerboxwidth: element.outerBoxWidth,
                  outerboxheight: element.outerBoxHeight,
                  outerboxnetweight: element.outerBoxNetWeight,
                  outerboxgrossweight: element.outerBoxGrossWeight,
                  outerboxvolume: element.outerBoxVolume,
                  NumberOfBoxes: element.boxCount,
                  totalNetWeight: element.totalNetWeight,
                  totalGrossWeight: element.totalGrossWeight,
                  totalVolume: element.totalVolume,
                  OtherFees: element.otherFees,
                  SinglesalesrevenueA: element.singlesalesrevenue,
                  Singleproductvolume: element.singleproductvolume,
                  Portchargesforindividualproducts: element.portchargesforindividualproducts,
                  Oceanfreightforasingleproduct: element.oceanfreightforasingleproduct,
                  Inlandfreightforasingleproduct: element.inlandfreightforasingleproduct
                });
              });
            }
            if (response.data.contractExpenses.length > 0) {
              CustomerRelaterExoensesTableData.value = [];
              CustomerRelaterExoensesTableData.value = response.data.contractExpenses;
              CustomerRelaterExoensesTableData.value.forEach(item => {
                item.amount = item.expense * item.exchangeRate;
              });
            }
          }).catch(error => {
            console.error(error);
            reject(error);
          });
        });
      }
    }).catch(error => {
      console.error(error);
    });
    contractDialog.value = true;
    contractform.id = row.documentID;
  } else if (row.documentType == "2") {//获取采购合同详情
    request({
      url: 'PurchaseContracts/GetPurchaseContractDetailsById/GetPurchaseContractDetails',
      method: 'GET',
      params: {
        PurchaseContracID: row.documentID
      }
    }).then(response => {
      if (response.data != null) {
        const purchaseContracts = response.data.purchaseContracts;
        PurchaseContractDialogData.value.purchaseContract = purchaseContracts.purchaseContractNumber;
        PurchaseContractDialogData.value.contractStatus = state.optionss['hr_contract_status'].find(item => item.dictValue === purchaseContracts.contractStatus.toString()).dictLabel;
        PurchaseContractDialogData.value.deliveryDate = purchaseContracts.deliveryDate;
        //PurchaseContractDialogData.value.vendorCode = state.optionss['sql_supplier_info'].find(item => item.dictValue === purchaseContracts.vendorCode.toString()).dictLabel;
        PurchaseContractDialogData.value.purchaseCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === purchaseContracts.purchaseCurrency.toString()).dictLabel;
        PurchaseContractDialogData.value.deposit = purchaseContracts.deposit;
        PurchaseContractDialogData.value.salesperson = (state.optionss['sql_hr_sale'].find(item => item.dictValue === purchaseContracts.salesperson.toString()) || { dictLabel: '未知销售员' }).dictLabel;
        PurchaseContractDialogData.value.purchaser = (state.optionss['sql_hr_purchase'].find(item => item.dictValue === purchaseContracts.purchaser.toString()) || { dictLabel: '未知采购员' }).dictLabel;
        PurchaseContractDialogData.value.priceTerms = (state.optionss['hr_pricing_term'].find(item => item.dictValue === purchaseContracts.priceTerms.toString()) || { dictLabel: '未知价格条款' }).dictLabel;
        PurchaseContractDialogData.value.paymentDays = purchaseContracts.paymentDays;
        PurchaseContractDialogData.value.TotalValueOfGoods = purchaseContracts.totalGoodsValue;
        PurchaseContractDialogData.value.TotalQuantity = purchaseContracts.totalQuantity;
        PurchaseContractDialogData.value.TotalNumberOfBoxes = purchaseContracts.totalBoxCount;
        PurchaseContractDialogData.value.TotalGrossWeight = purchaseContracts.totalGrossWeight;
        PurchaseContractDialogData.value.TotalNetWeight = purchaseContracts.totalNetWeight;
        PurchaseContractDialogData.value.TotalVolume = purchaseContracts.totalVolume;
        PurchaseContractDialogData.value.TotalPurchases = purchaseContracts.totalPurchases;
        PurchaseContractDialogData.value.TotalTaxRefund = purchaseContracts.totalTaxRefund;
        PurchaseContractDialogData.value.customerExpenseTotal = purchaseContracts.customerExpenseTotal;
        PurchaseContractDialogData.value.appliedPayment = purchaseContracts.appliedPayment;
        PurchaseContractDialogData.value.availablePayment = purchaseContracts.availablePayment;
        PurchaseContractDialogData.value.unpaidAmount = purchaseContracts.unpaidAmount;
        PurchaseContractDialogData.value.paidAmount = purchaseContracts.paidAmount;
        response.data.purchaseContractProducts.forEach(productData => {
          productData.productSupplier = state.optionss['sql_supplier_info'].find(item => item.dictValue === productData.supplierID.toString()).dictLabel;
          //productData.unit = state.optionss['hr_calculate_unit'].find(item => item.dictValue === productData.unit.toString()).dictLabel;
          //productData.packaging = state.optionss['hr_packing'].find(item => item.dictValue === productData.packaging.toString()).dictLabel;
          productData.invoice = productData.invoice == 0 ? "否" : "是";
        });
        PurchaseContractDialogData.value.productinfotableData = response.data.purchaseContractProducts;
        PurchaseContractDialogData.value.CustomerRelaterExoensesTableData = response.data.purchaseContractVendorExpenses;
        PurchaseContractDialog.value = true;
      }
    }).catch(error => {
      console.error(error);
    });
  } else if (row.documentType == "5") {//获取付款申请详情
    request({
      url: 'PaymentRequest/GetPaymentRequestDetailsByID/GetDetails',
      method: 'GET',
      params: {
        id: row.documentID
      }
    }).then(response => {
      if (response.data != null) {
        PaymentrequestForm.value.applicant = state.optionss['sql_all_user'].find(item => item.dictValue === response.data.paymentRequest.applicant.toString()).dictLabel;
        PaymentrequestForm.value.applicationDepartment = state.optionss['sql_hr_dept'].find(item => item.dictValue === response.data.paymentRequest.applicationDepartment.toString()).dictLabel;
        PaymentrequestForm.value.applicationDate = formatDate(response.data.paymentRequest.applicationDate);
        PaymentrequestForm.value.applicationNumber = response.data.paymentRequest.applicationNumber;
        PaymentrequestForm.value.currencyCode = state.optionss['hr_export_currency'].find(item => item.dictValue === response.data.paymentRequest.currencyCode.toString()).dictLabel;
        PaymentrequestForm.value.totalAmount = response.data.paymentRequest.totalAmount;
        PaymentrequestForm.value.paymentCategory = state.optionss['hr_payment_category'].find(item => item.dictValue === response.data.paymentRequest.paymentCategory.toString()).dictLabel;
        PaymentrequestForm.value.paymentName = response.data.paymentRequest.paymentName;
        switch (response.data.paymentRequest.paymentCategory.toString()) {
          case '1':
            PaymentrequestForm.value.paymentName = state.optionss['hr_factory_payment'].find(item => item.dictValue === response.data.paymentRequest.paymentName.toString()).dictLabel;
            break;
          case '2':
            PaymentrequestForm.value.paymentName = state.optionss['hr_domestic_charges'].find(item => item.dictValue === response.data.paymentRequest.paymentName.toString()).dictLabel;
            break;
          case '3':
            PaymentrequestForm.value.paymentName = state.optionss['hr_foreign_charges'].find(item => item.dictValue === response.data.paymentRequest.paymentName.toString()).dictLabel;
            break;
          case '4':
            PaymentrequestForm.value.paymentName = state.optionss['hr_daily_expenses'].find(item => item.dictValue === response.data.paymentRequest.paymentName.toString()).dictLabel;
            break;
        }
        PaymentrequestForm.value.payeeCode = state.optionss['sql_supplier_info'].find(item => item.dictValue === response.data.paymentRequest.payeeCode.toString()).dictLabel;
        PaymentrequestForm.value.payeeName = response.data.paymentRequest.payeeName;
        PaymentrequestForm.value.bankName = response.data.paymentRequest.bankName;
        PaymentrequestForm.value.bankAccount = response.data.paymentRequest.bankAccount;
        PaymentrequestForm.value.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictValue === response.data.paymentRequest.ourCompany.toString()).dictLabel;
        PaymentrequestForm.value.paidAmount = response.data.paymentRequest.paidAmount;
        PaymentrequestForm.value.unpaidAmount = response.data.paymentRequest.unpaidAmount;
        PaymentrequestForm.value.handler = state.optionss['sql_all_user'].find(item => item.dictValue === response.data.paymentRequest.handler.toString()).dictLabel;
        PaymentrequestForm.value.remarks = response.data.paymentRequest.remarks;
        CostDetailsTbaleData.value = [];
        response.data.paymentRequestDetails.forEach(item => {
          CostDetailsTbaleData.value.push(item);
        });
        var OrderType = [];
        CostDetailsTbaleData.value.forEach(CostDetailsTbaleData => {
          switch (CostDetailsTbaleData.relatedModules.toString()) {
            case '1':
              OrderType = state.optionss['sql_purchase_contract'];
              break;
            case '2':
              OrderType = state.optionss['sql_sale_contracts'];
              break;
            default:
              CostDetailsTbaleData.associatedOrderNumber = '无';
              break;
          }
          CostDetailsTbaleData.associatedOrderNumber = OrderType.find(item => item.dictValue === CostDetailsTbaleData.associatedOrderNumber.toString()).dictLabel;
          switch (response.data.paymentRequest.paymentCategory.toString()) {
            case '1':
              CostDetailsTbaleData.specificPaymentItems = state.optionss['hr_factory_payment'].find(item => item.dictValue === CostDetailsTbaleData.specificPaymentItems.toString()).dictLabel;
              break;
            case '2':
              CostDetailsTbaleData.specificPaymentItems = state.optionss['hr_domestic_charges'].find(item => item.dictValue === CostDetailsTbaleData.specificPaymentItems.toString()).dictLabel;
              break;
            case '3':
              CostDetailsTbaleData.specificPaymentItems = state.optionss['hr_foreign_charges'].find(item => item.dictValue === CostDetailsTbaleData.specificPaymentItems.toString()).dictLabel;
              break;
            case '4':
              CostDetailsTbaleData.specificPaymentItems = state.optionss['hr_daily_expenses'].find(item => item.dictValue === CostDetailsTbaleData.specificPaymentItems.toString()).dictLabel;
              break;
          }
          CostDetailsTbaleData.relatedModules = state.optionss['hr_associated_modules'].find(item => item.dictValue === CostDetailsTbaleData.relatedModules.toString()).dictLabel;
        });
        PaymentrequestDialog.value = true;
      }
    }).catch(error => {
      console.error(error);
    });
  } else if (row.documentType == "3") {//获取出运发货单详情
    ShippingDeliveryDialog.value = true;
    request({
      url: 'ShippingDeliveries/GetShippingDeliveriesDetailsByid/GetShippingDeliveriesDetails',
      method: 'GET',
      params: {
        ShippingDeliveriesId: row.documentID
      }
    }).then(response => {
      ShippingDeliveryForm.value.invoiceNumber = response.data.shippingDeliveries.invoiceNumber;
      ShippingDeliveryForm.value.OrderMakingDate = response.data.shippingDeliveries.createTime;
      ShippingDeliveryForm.value.shippingStatus = state.optionss['hr_shipping_status'].find(item => item.dictValue === response.data.shippingDeliveries.shippingStatus.toString()).dictLabel;
      ShippingDeliveryForm.value.shippingDate = response.data.shippingDeliveries.shippingDate;
      ShippingDeliveryForm.value.invoiceDate = response.data.shippingDeliveries.invoiceDate;
      ShippingDeliveryForm.value.customerNumber = state.optionss['sql_hr_customer'].find(item => item.dictValue === response.data.shippingDeliveries.customerNumber.toString()).dictLabel;
      ShippingDeliveryForm.value.customerAbbreviation = response.data.shippingDeliveries.customerAbbreviation;
      ShippingDeliveryForm.value.referenceContractNumber = state.optionss['sql_sale_contracts'].find(item => item.dictValue === response.data.shippingDeliveries.referenceContractNumber.toString()).dictLabel;
      ShippingDeliveryForm.value.salesContractNumber = response.data.shippingDeliveries.salesContractNumber.toString();
      ShippingDeliveryForm.value.customerContractNumber = response.data.shippingDeliveries.customerContractNumber.toString();
      ShippingDeliveryForm.value.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictValue === response.data.shippingDeliveries.ourCompany.toString()).dictLabel;
      ShippingDeliveryForm.value.bankOfReceipt = state.optionss['hr_bank'].find(item => item.dictValue === response.data.shippingDeliveries.bankOfReceipt.toString()).dictLabel;
      ShippingDeliveryForm.value.exportCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === response.data.shippingDeliveries.exportCurrency.toString()).dictLabel;
      ShippingDeliveryForm.value.exchangeRate = response.data.shippingDeliveries.exchangeRate;
      ShippingDeliveryForm.value.priceTerms = state.optionss['hr_pricing_term'].find(item => item.dictValue === response.data.shippingDeliveries.priceTerms.toString()).dictLabel;
      ShippingDeliveryForm.value.departurePort = state.optionss['hr_transport_port'].find(item => item.dictValue === response.data.shippingDeliveries.departurePort.toString()).dictLabel;
      ShippingDeliveryForm.value.destinationPort = response.data.shippingDeliveries.destinationPort;
      ShippingDeliveryForm.value.transportationMethod = state.optionss['hr_transportation_method'].find(item => item.dictValue === response.data.shippingDeliveries.transportationMethod.toString()).dictLabel;
      ShippingDeliveryForm.value.tradeCountry = state.optionss['hr_nation'].find(item => item.dictValue === response.data.shippingDeliveries.tradeCountry.toString()).dictLabel;
      ShippingDeliveryForm.value.settlementMethod = state.optionss['hr_settlement_way'].find(item => item.dictValue === response.data.shippingDeliveries.settlementMethod.toString()).dictLabel;
      ShippingDeliveryForm.value.receivableDate = response.data.shippingDeliveries.receivableDate;
      ShippingDeliveryForm.value.documentClerk = state.optionss['sql_all_user'].find(item => item.dictValue === response.data.shippingDeliveries.documentClerk.toString()).dictLabel;
      ShippingDeliveryForm.value.isDeposit = response.data.shippingDeliveries.isDeposit.toString();
      ShippingDeliveryForm.value.preCarriageTransport = state.optionss['hr_domestic_transport'].find(item => item.dictValue === response.data.shippingDeliveries.shippingAgent.toString()).dictLabel;
      ShippingDeliveryForm.value.shippingAgent = state.optionss['hr_freight_forwarding_company'].find(item => item.dictValue === response.data.shippingDeliveries.preCarriageTransport.toString()).dictLabel;
      ShippingDeliveryForm.value.remark = response.data.shippingDeliveries.remark;

      if (response.data.shippingDeliveryProducts.length > 0) {
        shippingDeliveryContrctProductTableData.value = response.data.shippingDeliveryProducts;
        shippingDeliveryContrctProductTableData.value.forEach(item => {
          request({
            url: 'Contracts/GetCustomerContractProductByCPID/GetCustomerContractProduct',
            method: 'GET',
            params: {
              CPID: item.contractProductId
            }
          }).then(response => {
            if (response.data != null && response.data.length > 0) {
              // 找到当前产品在表格数据中的索引
              const index = shippingDeliveryContrctProductTableData.value.findIndex(
                x => x.contractProductId === item.contractProductId
              );
              if (index !== -1) {
                // 更新产品信息
                const productData = response.data[0];
                shippingDeliveryContrctProductTableData.value[index] = {
                  ...shippingDeliveryContrctProductTableData.value[index], // 保留原有数据
                  contractNumber: productData.contractNumber,
                  productCode: productData.productCode,
                  chineseName: productData.chineseName,
                  contractQuantity: productData.contractQuantity,
                  unit: state.optionss.hr_calculate_unit.find(u => u.dictValue === productData.unit.toString())?.dictLabel || '无',
                  exportUnitPrice: productData.exportUnitPrice,
                  exportTotalPrice: productData.exportTotalPrice,
                  specialRequirements: productData.specialRequirements,
                  outerBoxQuantity: productData.outerBoxQuantity,
                  boxCount: productData.boxCount,
                  outerBoxUnit: state.optionss.hr_outerbox_unit.find(u => u.dictValue === productData.outerboxunit.toString())?.dictLabel || '无',
                  outerBoxLength: productData.outerBoxLength,
                  outerBoxWidth: productData.outerBoxWidth,
                  outerBoxHeight: productData.outerBoxHeight,
                  outerBoxVolume: productData.outerBoxVolume,
                  totalVolume: productData.totalVolume,
                  outerBoxNetWeight: productData.outerBoxNetWeight,
                  outerBoxGrossWeight: productData.outerBoxGrossWeight,
                  totalNetWeight: productData.totalNetWeight,
                  totalGrossWeight: productData.totalGrossWeight
                };
              }
            }
          }).catch(error => {
            console.error('获取产品详情失败:', error);
            ElMessage.error('获取产品详情失败，请稍后重试');
          });
        });
      }
      if (response.data.shippingDeliveryPurchaseDetails != null) {
        shippingDeliveryPurchaseDetailsTableData.value = response.data.shippingDeliveryPurchaseDetails;
        shippingDeliveryPurchaseDetailsTableData.value.forEach(item => {
          request({
            url: 'PurchaseContracts/GetPurchaseProductByPPID/GetPurchaseProduct',
            method: 'GET',
            params: {
              PurchaseProductID: item.purchaseContractProductID
            }
          }).then(response => {
            if (response.data != null && response.data.length > 0) {
              // 找到当前采购明细在表格数据中的索引
              const index = shippingDeliveryPurchaseDetailsTableData.value.findIndex(
                x => x.purchaseContractProductID === item.purchaseContractProductID
              );

              if (index !== -1) {
                // 更新采购明细信息
                const purchaseData = response.data[0];
                shippingDeliveryPurchaseDetailsTableData.value[index] = {
                  ...shippingDeliveryPurchaseDetailsTableData.value[index], // 保留原有数据
                  purchaseContractNumber: purchaseData.purchaseContractNumber,
                  vendorAbbreviation: state.optionss.sql_supplier_info.find(s => s.dictValue === purchaseData.supplierID.toString())?.dictLabel || '无',
                  productNumber: state.optionss.sql_product.find(p => p.dictValue === purchaseData.productNumber.toString())?.dictLabel || '无',
                  chineseName: purchaseData.chineseName,
                  purchaseCurrency: state.optionss.hr_export_currency.find(c => c.dictValue === purchaseData.purchaseCurrency.toString())?.dictLabel || '无',
                  purchaseUnitPrice: purchaseData.purchasePrice,
                  purchaseTotalPrice: purchaseData.purchaseTotalPrice,
                  measurementUnit: state.optionss.hr_calculate_unit.find(u => u.dictValue === purchaseData.unit.toString())?.dictLabel || '无',
                  invoice: purchaseData.invoice === 1 ? '是' : '否',
                  totalVolume: purchaseData.totalVolume,
                  totalGrossWeight: purchaseData.totalGrossWeight,
                  contractQuantity: purchaseData.contractQuantity
                };
              }
            }
          }).catch(error => {
            console.error('获取采购明细失败:', error);
            ElMessage.error('获取采购明细失败，请稍后重试');
          });
        });
      }
    });
  } else if (row.documentType == "6") {//获取结算单详情
    var ShippingDeliveryID = null;
    request({
      url: 'ShippingDeliveries/GetSettlementByID/GetSettlement',
      method: 'GET',
      params: {
        SettlementID: row.documentID
      }
    }).then(response => {
      if (response.data != null) {
        //主要信息
        SettlementForm.value.invoiceNumber = response.data.invoiceNumber;
        SettlementForm.value.shippingDate = response.data.shippingDate;
        SettlementForm.value.shippingStatus = response.data.shippingStatus;
        SettlementForm.value.salesContractNumber = response.data.salesContract;
        SettlementForm.value.customerName = response.data.customerName;
        SettlementForm.value.customerContract = response.data.customerContract;
        SettlementForm.value.ourCompany = response.data.ourCompany;
        SettlementForm.value.foreignCurrency = state.optionss.hr_export_currency.find(item => item.dictValue === response.data.foreignCurrency.toString()).dictLabel;
        SettlementForm.value.exchangeRate = response.data.exchangeRate;
        SettlementForm.value.settlementMethod = response.data.settlementMethod;
        SettlementForm.value.isSettlement = response.data.isSettlement;
        SettlementForm.value.priceTerms = response.data.priceTerms;
        SettlementForm.value.receivableDate = response.data.receivableDate;
        SettlementForm.value.salesClerk = response.data.salesperson;
        SettlementForm.value.documentClerk = response.data.documentOfficer;
        SettlementForm.value.isSettlement = response.data.isSettled ? '是' : '否';

        //客户收汇
        CustomerReceiptForm.value.totalValue = response.data.totalValue;
        CustomerReceiptForm.value.totalCost = response.data.totalCost;
        CustomerReceiptForm.value.claimAmount = response.data.claimAmount;
        CustomerReceiptForm.value.totalReceivable = response.data.totalReceivable;
        CustomerReceiptForm.value.totalReceived = response.data.totalReceived;
        CustomerReceiptForm.value.totalUnreceived = response.data.totalUnreceived;

        //应退税金额
        TaxRefundForm.value.totalRefund = response.data.totalRefund;
        TaxRefundForm.value.totalRefunded = response.data.totalRefunded;
        TaxRefundForm.value.totalUnrefunded = response.data.totalUnrefunded;
        TaxRefundForm.value.isRefunded = response.data.isRefunded ? '是' : '否';

        //国内费用
        DomesticFeesForm.value.expressFee = response.data.expressFee;
        DomesticFeesForm.value.transportation = response.data.transportation;
        DomesticFeesForm.value.totalPayable = response.data.totalPayable;
        DomesticFeesForm.value.totalPaid = response.data.totalPaid;
        DomesticFeesForm.value.totalUnpaid = response.data.totalUnpaid;

        //国外费用
        ForeignExpensesForm.value.commissionPayable = response.data.commissionPayable;
        ForeignExpensesForm.value.commissionPaid = response.data.commissionPaid;
        ForeignExpensesForm.value.commissionPaidAmount = response.data.commissionPaidAmount || 0;
        ForeignExpensesForm.value.commissionUnpaid = response.data.commissionUnpaid;
        ForeignExpensesForm.value.shippingAgentCompany = response.data.shippingAgentCompany;
        ForeignExpensesForm.value.shippingCurrency = response.data.shippingCurrency || '无';
        ForeignExpensesForm.value.shippingExchangeRate = response.data.shippingExchangeRate || 0;
        ForeignExpensesForm.value.shippingPayable = response.data.shippingPayable || 0;
        ForeignExpensesForm.value.shippingPaid = response.data.shippingPaid || 0;
        ForeignExpensesForm.value.shippingPaidAmount = response.data.shippingPaidAmount || 0;
        ForeignExpensesForm.value.shippingUnpaidAmount = response.data.shippingUnpaidAmount || 0;
        ForeignExpensesForm.value.otherForeignCost = response.data.otherForeignCost || 0;
        ShippingDeliveryID = response.data.shippingDeliveriesID;
        if (ShippingDeliveryID != null) {
          //#region 获取工厂付款数据
          request({
            url: 'ShippingDeliveries/GetFactoryPaymentData/GetFactoryPaymentDataList',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetFactoryPaymentDataList => {
            if (GetFactoryPaymentDataList.code === 200) {
              FactoryPaymentData.value = GetFactoryPaymentDataList.data.map(item => ({
                applicationNumber: item.applicationNumber,
                reviewStatus: item.reviewStatus,
                reviewStatusStr: getReviewStatusText(item.reviewStatus),
                paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
                paymentName: state.optionss["hr_factory_payment"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
                payeeName: item.payeeName,
                bankName: item.bankName,
                bankAccount: item.bankAccount,
                ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
                currencyCode: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
                totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
                paidAmount: item.paidAmount?.toFixed(2) || '0.00',
                unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
                applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
                applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
                handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
                applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
              }))
            } else {
              ElMessage.error('获取工厂付款数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取工厂付款数据失败，请稍后重试');
          });
          //#endregion
          //#region 获取采购其它费用数据
          request({
            url: 'ShippingDeliveries/GetPurchaseOtherFeeDataByShippingDeliveriesID/GetPurchaseOtherFeeData',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetPurchaseOtherFeeDataresponse => {
            if (GetPurchaseOtherFeeDataresponse.code === 200) {
              PurchaseOtherCostsData.value = GetPurchaseOtherFeeDataresponse.data.map(item => ({
                applicationNumber: item.applicationNumber,
                reviewStatus: item.reviewStatus,
                reviewStatusStr: getReviewStatusText(item.reviewStatus),
                paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
                paymentName: state.optionss["hr_factory_payment"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
                payeeName: item.payeeName,
                bankName: item.bankName,
                bankAccount: item.bankAccount,
                ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
                currencyCode: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
                totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
                paidAmount: item.paidAmount?.toFixed(2) || '0.00',
                unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
                applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
                applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
                handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
                applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
              }))
            } else {
              ElMessage.error('获取采购其它费用数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取采购其它费用数据失败，请稍后重试');
          });
          //#endregion
          //#region 获取客户收汇数据
          request({
            url: 'ShippingDeliveries/GetCustomerControllerDataByShippingDeliveriesID/GetCustomerControllerData',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetCustomerControllerDataresponse => {
            if (GetCustomerControllerDataresponse.code === 200) {
              CustomerReceiptsData.value = GetCustomerControllerDataresponse.data.map(item => ({
                ReceiptNumber: item.receiptNumber,
                ReceiptDate: item.receiptDate ? item.receiptDate.split(' ')[0] : '',
                ReceiptAmount: item.amount,
                ReceiptCurrency: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.foreignCurrency.toString())?.dictLabel || '',
                ExchangeRate: item.exchangeRate,
                ReceiptBank: state.optionss["hr_bank"].find(bank => bank.dictValue == item.bank.toString())?.dictLabel || '',
                OurCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
                FundsType: state.optionss["funds_type"].find(ft => ft.dictValue == item.fundsType.toString())?.dictLabel || '',
              }))
            } else {
              ElMessage.error('获取客户收汇数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取客户收汇数据失败，请稍后重试');
          });
          //#endregion
          //#region 获取退税数据
          request({
            url: 'ShippingDeliveries/GetTaxRefundDataByShippingDeliveriesID/GetTaxRefundData',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetTaxRefundDataresponse => {
            if (GetTaxRefundDataresponse.code === 200) {
              TaxRefundDetailsData.value = GetTaxRefundDataresponse.data.map(item => ({
                ReceiptNumber: item.refundNumber,
                InvoiceNumber: item.invoiceNumber,
                RefundAmount: item.refundAmount,
                ActualRefundAmount: item.actualRefundAmount,
                IsRefunded: item.isRefunded ? '是' : '否',
                RefundDate: item.refundDate ? item.refundDate.split(' ')[0] : '',
              }))
            } else {
              ElMessage.error('获取退税数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取退税数据失败，请稍后重试');
          });
          //#endregion
          //#region 获取国内已付费用数据
          request({
            url: 'ShippingDeliveries/GetDomesticFeesDataByShippingDeliveriesID/GetDomesticFeesData',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetDomesticFeesDataresponse => {
            if (GetDomesticFeesDataresponse.code === 200) {
              DomesticPaymentsData.value = GetDomesticFeesDataresponse.data.map(item => ({
                applicationNumber: item.applicationNumber,
                reviewStatus: item.reviewStatus,
                reviewStatusStr: getReviewStatusText(item.reviewStatus),
                paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
                paymentName: state.optionss["hr_domestic_charges"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
                payeeName: item.payeeName,
                bankName: item.bankName,
                bankAccount: item.bankAccount,
                ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
                currencyCode: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
                totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
                paidAmount: item.paidAmount?.toFixed(2) || '0.00',
                unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
                applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
                applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
                handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
                applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
              }))
            } else {
              ElMessage.error('获取国内已付费用数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取国内已付费用数据失败，请稍后重试');
          });
          //#endregion
          //#region 获取国外已付费用数据
          request({
            url: 'ShippingDeliveries/GetForeignExpensesByShippingDeliveriesID/GetForeignExpensesData',
            method: 'GET',
            params: {
              ShippingDeliveriesId: ShippingDeliveryID
            }
          }).then(GetForeignExpensesDataresponse => {
            if (GetForeignExpensesDataresponse.code === 200) {
              InternationalPaymentsData.value = GetForeignExpensesDataresponse.data.map(item => ({
                applicationNumber: item.applicationNumber,
                reviewStatus: item.reviewStatus,
                reviewStatusStr: getReviewStatusText(item.reviewStatus),
                paymentCategory: state.optionss["hr_payment_category"].find(pc => pc.dictValue == item.paymentCategory.toString())?.dictLabel || '',
                paymentName: state.optionss["hr_foreign_charges"].find(hfp => hfp.dictValue == item.paymentName.toString())?.dictLabel || '', // 转换款项名称
                payeeName: item.payeeName,
                bankName: item.bankName,
                bankAccount: item.bankAccount,
                ourCompany: state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '',
                currencyCode: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.currencyCode.toString())?.dictLabel || '',
                totalAmount: item.applicationAmount?.toFixed(2) || '0.00',
                paidAmount: item.paidAmount?.toFixed(2) || '0.00',
                unpaidAmount: item.unpaidAmount?.toFixed(2) || '0.00',
                applicant: state.optionss["sql_all_user"].find(user => user.dictValue == item.applicant.toString())?.dictLabel || '',
                applicationDepartment: state.optionss["sql_hr_dept"].find(dept => dept.dictValue == item.applicationDepartment.toString())?.dictLabel || '',
                handler: state.optionss["sql_all_user"].find(user => user.dictValue == item.handler.toString())?.dictLabel || '',
                applicationDate: item.applicationDate ? item.applicationDate.split(' ')[0] : ''
              }))
            } else {
              ElMessage.error('获取国外已付费用数据失败：无效的响应数据');
            }
          }).catch(error => {
            ElMessage.error('获取国外已付费用数据失败，请稍后重试');
          });
          //#endregion
        }
      }
    }).catch(error => {
      ElMessage.error('获取结算单失败，请稍后重试');
    });
    SettlementApprovalDialog.value = true;
  }
}


const TimeoutNotProcessedClick = () => {
  if (TimeoutNotProcessedCount.value > 0) {
    // 执行点击事件的逻辑
    openAgencyProcessdialogTableVisible();
  } else {
    ElMessage.warning("没有超时未处理的流程");
  }
}
const WaitingforyouProcessedClick = () => {
  if (pendingCount.value > 0) {
    // 执行点击事件的逻辑
    openAgencyProcessdialogTableVisible();
  } else {
    ElMessage.warning("没有待您处理的流程");
  }
}

// 待办流程弹窗
const AgencyProcessdialogTableVisible = ref(false);

const openAgencyProcessdialogTableVisible = () => {
  AgencyProcessdialogTableVisible.value = true;
}

const AgencyProcesstableData = ref([]);

// 待办流程待处理数量
const pendingCount = ref(0);
// 超时未处理数量
const TimeoutNotProcessedCount = ref(0);
//逾期交货合同数量
const OverdueDeliveryContractCount = ref(0);
//逾期交货合同数据
const overdueDeliveryContractData = ref([]);
//逾期交货合同弹窗
const overdueDeliveryContractDialogVisible = ref(false);
//获取逾期交货合同数量
const GetOverdueDeliveryContractCount = () => {
  OverdueDeliveryContractCount.value = 0;
  overdueDeliveryContractData.value = [];
  request({
    url: 'Contracts/GetOverdueContractInfo/GetOverdueContract',
    method: 'GET'
  }).then(response => {
    if (response.code == "200") {
      OverdueDeliveryContractCount.value = response.data.length;
      overdueDeliveryContractData.value = response.data;
    } else {
      ElMessage.error('获取逾期交货合同数量失败');
    }
  })
}
GetOverdueDeliveryContractCount();

const OverdueDeliveryContractClick = () => {
  if (OverdueDeliveryContractCount.value > 0) {
    overdueDeliveryContractDialogVisible.value = true;
  } else {
    ElMessage.warning("没有逾期交货合同");
  }
}

//获取沟通逾期的数量
const CommunicationOverdueCount = ref(0);
//沟通逾期数据
const communicationOverdueData = ref([]);
//沟通逾期弹窗
const communicationOverdueDialogVisible = ref(false);
//获取沟通逾期数量
const GetCommunicationOverdueCount = () => {
  CommunicationOverdueCount.value = 0;
  communicationOverdueData.value = [];
  request({
    url: 'CustomerInfoMation/GetExpiringCustomersDataByUser/GetExpiringCustomers',
    method: 'GET'
  }).then(response => {
    if (response.code == "200") {
      CommunicationOverdueCount.value = response.data.length;
      communicationOverdueData.value = response.data;
    } else {
      ElMessage.error('获取沟通逾期数量失败');
    }
  })
}
GetCommunicationOverdueCount();

const CommunicationOverdueClick = () => {
  if (CommunicationOverdueCount.value > 0) {
    communicationOverdueDialogVisible.value = true;
  } else {
    ElMessage.warning("没有沟通逾期");
  }
}
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
    hr_export_currency: [],
    hr_outerbox_unit: [],
    sys_yes_no: [],
    hr_packing: [],
    hr_calculate_unit: [],
    hr_signing_place: [],
    hr_transportation_method: [],
    hr_transport_port: [],
    hr_ourcompany: [],
    hr_contract_status: [],
    sql_hr_customer: [],
    sql_hr_customer_abbreviation: [],
    sql_hr_customer_contactperson: [],
    hr_yes_no: [],
    hr_bank: [],
    funds_type: [],
    sql_sale_contracts: [],
    sql_purchase_contract: [],
    sql_supplier_info: [],
    sql_hr_purchase: [],
    sql_payment_requests: [],
    sql_hr_dept: [],
    hr_payment_category: [],
    hr_factory_payment: [],
    hr_domestic_charges: [],
    hr_foreign_charges: [],
    hr_daily_expenses: [],
    hr_associated_modules: [],
    sql_shippingdeliveries: [],
    hr_shipping_status: [],
    hr_domestic_transport: [],
    hr_freight_forwarding_company: [],
    sql_product: [],
    sql_settlement: [],
    hr_payment_contract_type: []
  }
})
const { optionss } = toRefs(state)
//从后台读取多个字典数据
var dictParams = [
  { dictType: 'sql_all_user' },
  { dictType: 'hr_approval_document_type' },
  { dictType: 'sql_sale_contracts' },
  { dictType: 'hr_export_currency' },
  { dictType: 'hr_outerbox_unit' },
  { dictType: 'sys_yes_no' },
  { dictType: 'hr_packing' },
  { dictType: 'hr_calculate_unit' },
  { dictType: 'hr_signing_place' },
  { dictType: 'hr_transportation_method' },
  { dictType: 'hr_transport_port' },
  { dictType: 'hr_ourcompany' },
  { dictType: 'hr_contract_status' },
  { dictType: 'sql_hr_customer' },
  { dictType: 'sql_hr_customer_abbreviation' },
  { dictType: 'sql_hr_customer_contactperson' },
  { dictType: 'hr_yes_no' },
  { dictType: 'hr_customer_level' },
  { dictType: 'hr_settlement_way' },
  { dictType: 'hr_pricing_term' },
  { dictType: 'hr_nation' },
  { dictType: 'sql_hr_sale' },
  { dictType: 'hr_bank' },
  { dictType: 'sql_sale_contracts' },
  { dictType: 'funds_type' },
  { dictType: 'sql_purchase_contract' },
  { dictType: 'sql_supplier_info' },
  { dictType: 'hr_customer_level' },
  { dictType: 'sql_hr_purchase' },
  { dictType: 'sql_payment_requests' },
  { dictType: 'sql_hr_dept' },
  { dictType: 'hr_payment_category' },
  { dictType: 'hr_factory_payment' },
  { dictType: 'hr_domestic_charges' },
  { dictType: 'hr_foreign_charges' },
  { dictType: 'hr_daily_expenses' },
  { dictType: 'hr_associated_modules' },
  { dictType: 'sql_shippingdeliveries' },
  { dictType: 'hr_shipping_status' },
  { dictType: 'hr_domestic_transport' },
  { dictType: 'hr_freight_forwarding_company' },
  { dictType: 'sql_product' },
  { dictType: 'sql_settlement' },
  { dictType: 'hr_payment_contract_type' }
]
proxy.getDicts(dictParams).then((response) => {
  response.data.forEach((element) => {
    state.optionss[element.dictType] = element.list
  })
  getPendingCount();
  getFinancialTasksList(1, 10);
})

const getPendingCount = () => {
  AgencyProcesstableData.value = [];
  request({
    url: 'ApprovalFlow/GetApprovalRecord/GetApprovalRecordCount',
    method: 'GET'
  }).then(response => {
    if (response.code == "200") {
      pendingCount.value = response.data.length;
      if (response.data.length > 0) {
        AgencyProcesstableData.value = response.data;
        AgencyProcesstableData.value.forEach(item => {
          item.createBy = state.optionss['sql_all_user'].filter(user => user.dictValue == item.createBy).map(user => user.dictLabel).values().next().value;
          item.documentTypeName = state.optionss['hr_approval_document_type'].filter(user => user.dictValue == item.documentType).map(user => user.dictLabel).values().next().value;
          if (item.documentType == "1" || item.documentType == "7") {
            item.documentNumber = state.optionss['sql_sale_contracts'].filter(Salecontract => Salecontract.dictValue == item.documentID.toString()).map(Salecontract => Salecontract.dictLabel).values().next().value;
          } else if (item.documentType == "2") {
            item.documentNumber = state.optionss['sql_purchase_contract'].filter(Purchasecontract => Purchasecontract.dictValue == item.documentID.toString()).map(Purchasecontract => Purchasecontract.dictLabel).values().next().value;
          } else if (item.documentType == "5") {
            item.documentNumber = state.optionss['sql_payment_requests'].filter(Paymentrequests => Paymentrequests.dictValue == item.documentID.toString()).map(Paymentrequests => Paymentrequests.dictLabel).values().next().value;
          } else if (item.documentType == "3") {
            item.documentNumber = state.optionss['sql_shippingdeliveries'].filter(Shippingdeliveries => Shippingdeliveries.dictValue == item.documentID.toString()).map(Shippingdeliveries => Shippingdeliveries.dictLabel).values().next().value;
          } else if (item.documentType == "6") {
            item.documentNumber = state.optionss['sql_settlement'].filter(Settlement => Settlement.dictValue == item.documentID.toString()).map(Settlement => Settlement.dictLabel).values().next().value;
          }
          const createTime = dayjs(item.createTime);
          const now = dayjs();
          const hoursDiff = now.diff(createTime, 'hour');
          item.createTime = createTime.format('YYYY-MM-DD HH:mm:ss');
          if (hoursDiff >= 6) {
            TimeoutNotProcessedCount.value++; // 增加超时未处理计数
          }
        });
      }
    } else {
      ElMessage.error("获取待办流程数量失败");
    }
  }).catch(error => {
    console.error(error);
  });
}

const value = ref('')

const options = [
  {
    value: '西九区：阿拉斯加',
    label: '西九区：阿拉斯加',
  },
  {
    value: '西八区：洛杉矶，旧金山',
    label: '西八区：洛杉矶，旧金山',
  },
  {
    value: '西七区：盐湖城、丹佛、凤凰城',
    label: '西七区：盐湖城、丹佛、凤凰城',
  }
]

const value2 = ref('')

const options2 = [
  {
    value: 'USD',
    label: 'USD',
  },
  {
    value: 'CNY',
    label: 'CNY',
  },
  {
    value: 'EUR',
    label: 'EUR',
  }
]

const value3 = ref('')

const options3 = [
  {
    value: 'USD',
    label: 'USD',
  },
  {
    value: 'CNY',
    label: 'CNY',
  },
  {
    value: 'EUR',
    label: 'EUR',
  }
]

const showEdit = ref(false)
const data = {
  newVisitis: {
    expectedData: [100, 120, 161, 134, 105, 160, 165],
    actualData: [120, 82, 91, 154, 162, 140, 145]
  },
  messages: {
    expectedData: [200, 192, 120, 144, 160, 130, 140],
    actualData: [180, 160, 151, 106, 145, 150, 130]
  },
  purchases: {
    expectedData: [80, 100, 121, 104, 105, 90, 100],
    actualData: [120, 90, 100, 138, 142, 130, 130]
  },
  shoppings: {
    expectedData: [130, 140, 141, 142, 145, 150, 160],
    actualData: [120, 82, 91, 154, 162, 140, 130]
  }
}

const userInfo = computed(() => {
  return useUserStore().userInfo
})
const onlineInfo = computed(() => {
  return useSocketStore().onlineInfo
})


let lineChartData = reactive([])
const dataType = ref(null)
function handleSetLineChartData(type) {
  dataType.value = type
  lineChartData = data[type]
}
handleSetLineChartData('newVisitis')

function handleAdd() {
  proxy.$modal.msg('请通过搜索添加')
}

const calendarState = reactive({
  tableData: [],
  //测试数据
  calendarData: [
    {
      day: "2025-01-04",
      xianyue: 100,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2025-01-05",
      xianyue: 100,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2025-01-06",
      xianyue: 100,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2025-01-07",
      xianyue: 100,
      yiyue: 5,
      sy: 1,
    },
  ],
});

//处理日期获取后台数据动态渲染上去
const textContent = (date) => {
  //当前date是拿到上面日历组件当前的日期值 根据该值去筛选测试数据找到对应各个日期下对应的数据return出去
  return calendarState.calendarData.filter((item) => {
    return date === item.day;
  });
};

// 工作任务列表
const handleClick = () => {
  var num = 24700000000000 / 510000;
}

const UnpaidDetailsTbaleData = ref([])
const CostDetailsTbaleData = ref([])
const PaymentrequestDialog = ref(false)
const PaymentrequestForm = ref({
  applicationNumber: '',
  applicationDate: '',
  paymentCategory: '',
  paymentName: '',
  payeeCode: '',
  payeeName: '',
  bankName: '',
  bankAccount: '',
  ourCompany: '',
  currencyCode: '',
  totalAmount: '',
  paidAmount: '',
  unpaidAmount: '',
  applicant: '',
  applicationDepartment: '',
  financialApproval: '',
  handler: '',
  remarks: ''
})

//出运发货单审批
const shippingDeliveryContrctProductTableData = ref([])
const shippingDeliveryPurchaseDetailsTableData = ref([])
const ShippingDeliveryDialog = ref(false)
const ShippingDeliveryForm = ref({
  invoiceNumber: '',
  shippingStatus: '',
  shippingDate: null,
  OrderMakingDate: null,
  invoiceDate: null,
  customerNumber: '',
  customerAbbreviation: '',
  referenceContractNumber: '',
  salesContractNumber: '',
  customerContractNumber: '',
  ourCompany: '',
  bankOfReceipt: '',
  exportCurrency: '',
  exchangeRate: '',
  priceTerms: '',
  departurePort: '',
  destinationPort: '',
  tradeCountry: '',
  settlementMethod: '',
  transportationMethod: '',
  receivableDate: null,
  documentClerk: '',
  isDeposit: 0,
  preCarriageTransport: '',
  shippingAgent: '',
  remark: ''
})

//结算单
const SettlementApprovalDialog = ref(false)
const SettlementForm = ref({
  invoiceNumber: '',
  shippingDate: '',
  shippingStatus: '',
  salesContractNumber: '',
  customerContract: '',
  customerName: '',
  ourCompany: '',
  foreignCurrency: '',
  exchangeRate: '',
  priceTerms: '',
  settlementMethod: '',
  receivableDate: '',
  salesClerk: '',
  documentClerk: '',
  isSettlement: ''
})

const CustomerReceiptForm = ref({
  totalValue: '',
  totalCost: '',
  claimAmount: '',
  totalReceivable: '',
  totalReceived: '',
  totalUnreceived: ''
})

//退税
const TaxRefundForm = ref({
  totalRefund: '',
  totalRefunded: '',
  totalUnrefunded: '',
  isRefunded: ''
})

//国内费用
const DomesticFeesForm = ref({
  transportation: '',
  totalPayable: '',
  totalPaid: '',
  totalUnpaid: '',
  expressFee: '',
  otherDomesticCost: ''
})

//国外费用
const ForeignExpensesForm = ref({
  commissionPayable: '',
  commissionPaid: '',
  commissionPaidAmount: '',
  commissionUnpaid: '',
  shippingAgentCompany: '',
  shippingCurrency: '',
  shippingPayable: '',
  shippingPaid: '',
  shippingPaidAmount: '',
  shippingUnpaidAmount: '',
  otherForeignCost: '',
  shippingExchangeRate: ''
})

const FactoryPaymentData = ref([])
const PurchaseOtherCostsData = ref([])
const CustomerReceiptsData = ref([])
const TaxRefundDetailsData = ref([])
const DomesticPaymentsData = ref([])
const InternationalPaymentsData = ref([])

//#region 计划任务日历
// 存储当前日历显示的所有日期
const calendarDates = ref([])

// 监听日历月份变化
const handleMonthChange = (date) => {
  const year = date.getFullYear()
  const month = date.getMonth()

  // 获取当月第一天和最后一天
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)

  // 获取当月第一天是星期几（0-6，0代表星期日）
  const firstDayWeek = firstDay.getDay()

  // 获取上个月需要显示的天数
  const prevMonthDays = firstDayWeek === 0 ? 6 : firstDayWeek - 1

  // 获取当月的总天数
  const currentMonthDays = lastDay.getDate()

  // 计算下个月需要显示的天数（确保总行数为6行）
  const totalDays = 42 // 6行 * 7天
  const nextMonthDays = totalDays - prevMonthDays - currentMonthDays

  const dates = []

  // 添加上个月的日期
  const prevMonth = new Date(year, month - 1)
  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = prevMonthDays - 1; i >= 0; i--) {
    const day = prevMonthLastDay - i
    dates.push({
      date: new Date(prevMonth.getFullYear(), prevMonth.getMonth(), day),
      type: 'prev-month'
    })
  }

  // 添加当月的日期
  for (let i = 1; i <= currentMonthDays; i++) {
    dates.push({
      date: new Date(year, month, i),
      type: 'current-month'
    })
  }

  // 添加下个月的日期
  const nextMonth = new Date(year, month + 1)
  for (let i = 1; i <= nextMonthDays; i++) {
    dates.push({
      date: new Date(nextMonth.getFullYear(), nextMonth.getMonth(), i),
      type: 'next-month'
    })
  }

  calendarDates.value = dates
}

// 监听当前日期变化
watch(currentDate, (newDate) => {
  handleMonthChange(newDate)
}, { immediate: true })


// 在模板中使用
const getDateInfo = (date) => {
  return {
    formattedDate: formatDate(date),
    isCurrentMonth: date.getMonth() === currentDate.value.getMonth(),
    isToday: isToday(formatDate(date))
  }
}

// 获取当前月份的所有日期
const currentMonthDates = calendarDates.value.filter(d => d.type === 'current-month')

// 获取所有显示的日期的格式化字符串
const allFormattedDates = calendarDates.value.map(d => formatDate(d.date))

// 获取当前显示的日期范围
const dateRange = {
  start: formatDate(calendarDates.value[0].date),
  end: formatDate(calendarDates.value[calendarDates.value.length - 1].date)
}

// 存储计划任务数据
const planTaskItems = ref([])

// 获取计划任务数据
const getPlanTaskItems = async (startDate, endDate) => {
  try {
    const response = await request({
      url: 'PlanTasks/GetPlanTaskItemsByUserIDAndDate/GetPlanTaskItems',
      method: 'GET',
      params: {
        startDate,
        endDate
      }
    })
    if (response.code === 200) {
      // 确保response.data是数组
      planTaskItems.value = Array.isArray(response.data) ? response.data : []
    } else {
      ElMessage.error('获取计划任务失败：' + response.msg)
      planTaskItems.value = []
    }
  } catch (error) {
    console.error('获取计划任务出错:', error)
    ElMessage.error('获取计划任务出错')
    planTaskItems.value = []
  }
}

// 获取指定日期的任务
const getDateTasks = (dateString) => {
  if (!Array.isArray(planTaskItems.value)) {
    return []
  }

  const dateData = planTaskItems.value.find(item => {
    // 将接口返回的日期格式化为 YYYY-MM-DD 格式进行比较
    const itemDate = formatDate(new Date(item.date))
    return itemDate === dateString
  })

  return dateData?.items || []
}

// 格式化日期时间
const formatDateTime = (dateTimeString) => {
  const date = new Date(dateTimeString)
  if (isNaN(date)) {
    return ''
  }
  return `${formatDate(date)} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
}

// 判断任务是否过期
const isOverdue = (timePoint) => {
  const date = new Date(timePoint)
  return !isNaN(date) && date < new Date()
}

// 监听日历日期范围变化，获取对应时间段的任务
watch(calendarDates, (newDates) => {
  if (Array.isArray(newDates) && newDates.length > 0) {
    const startDate = formatDate(newDates[0].date)
    const endDate = formatDate(newDates[newDates.length - 1].date)
    if (startDate && endDate) {
      getPlanTaskItems(startDate, endDate)
    }
  }
}, { immediate: true })

// 添加确认对话框相关的响应式变量
const confirmDialogVisible = ref(false)
const currentTask = ref(null)
const completionNote = ref('')

// 处理任务点击事件
const handleTaskClick = (task) => {
  currentTask.value = task
  confirmDialogVisible.value = true
}


// 定义API响应类型
interface ApiResponse<T = any> {
  code: number;
  msg: string;
  data: T;
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
        // 刷新当前日历数据
        const startDate = formatDate(calendarDates.value[0].date)
        const endDate = formatDate(calendarDates.value[calendarDates.value.length - 1].date)
        await getPlanTaskItems(startDate, endDate)
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
//#endregion


// 表格数据和分页
const TaskReminderTableData = ref([])
const TaskReminderTableCurrentPage = ref(1)
const TaskReminderTablePageSize = ref(5)
const TaskReminderTableTotalItems = ref(0)

// 获取任务提醒数据
const fetchTaskReminderData = async () => {
  try {
    const params = {
      pageNum: TaskReminderTableCurrentPage.value,
      pageSize: TaskReminderTablePageSize.value
    }
    const res = await getTaskReminderList(params)
    if (res.code === 200) {
      TaskReminderTableData.value = res.data.result
      TaskReminderTableTotalItems.value = res.data.totalNum
    }
  } catch (error) {
    console.error('获取任务提醒数据失败:', error)
  }
}
// 处理页码改变
const TaskReminderTableshandlePageChange = (page) => {
  TaskReminderTableCurrentPage.value = page
  fetchTaskReminderData()
}

// Load data when component mounts

const getTaskReminderList = (params) => {
  return request({
    url: 'TaskReminder/GetTaskReminderList/GetList',
    method: 'get',
    params
  })
}

const selectedDate = ref('')
const handleModifyDeliveryDate = (row) => {
  // 重置日期
  selectedDate.value = ''
  const DatePickerVNode = h(ElDatePicker, {
    modelValue: selectedDate.value,
    'onUpdate:modelValue': (val) => {
      selectedDate.value = val
      // 强制更新 VNode
      DatePickerVNode.component.props.modelValue = val
    },
    type: 'date',
    placeholder: '请选择日期',
    style: 'width: 100%',
    valueFormat: 'YYYY-MM-DD',
    locale: zhCn,
    disabledDate: (time) => time.getTime() < Date.now() - 8.64e7,
  })
  ElMessageBox({
    title: '申请修改交货日期',
    message: () => DatePickerVNode,
    showCancelButton: true,
    confirmButtonText: '提交审批',
    cancelButtonText: '取消',
    beforeClose: (action, instance, done) => {
      if (action === 'confirm' && !selectedDate.value) {
        ElMessage.warning('请选择日期')
        return
      }
      done()
    }
  }).then(async () => {
    try {
      const res = await request({
        url: 'Contracts/SubmitUpdateContractDeliveryDateApproval/UpdateDeliverryDateApproval',
        method: 'GET',
        params: {
          Id: row.id,  // 合同ID
          DeliveryDate: selectedDate.value  // 新的交货日期
        }
      })
      if (res.code === 200) {
        if (res.data == true) {
          // 获取逾期交货合同数量
          GetOverdueDeliveryContractCount()
          ElMessage.success('交货日期修改申请已提交！')
        } else {
          ElMessage.error('交货日期修改申请提交失败！')
        }
      } else {
        ElMessage.error(res.msg)
      }
    } catch (error) {
      ElMessage.error('提交审批失败')
    }
  }).catch(() => {
    ElMessage.info('已取消修改')
  })
}

//#region 获取逾期交货合同数量
// 定义接口返回的数据类型
interface OverdueContract {
  id: number;
  receivableDate: string;
  shipmentTotalAmount: number;
  // ... 其他需要的字段
}

// 定义响应式数据
const overdueContractsData = ref<OverdueContract[]>([])
const overdueContractsDialogVisible = ref(false)
const PaymentoverdueCount = ref(0)
// 获取逾期合同列表
const getOverdueContracts = async () => {
  try {
    const response = await request({
      url: 'ShippingDeliveries/GetPaymentoverdueShippingContractByUser/GetPaymentOberdueContract',
      method: 'get'
    })

    if (response.code === 200) {
      PaymentoverdueCount.value = response.data.length
      overdueContractsData.value = response.data
    } else {
      ElMessage.error(response.msg || '获取逾期合同列表失败')
    }
  } catch (error) {
    console.error('获取逾期合同列表失败:', error)
    ElMessage.error('获取逾期合同列表失败')
  }
}
getOverdueContracts();

const PaymentoverdueClick = () => {
  if (PaymentoverdueCount.value > 0) {
    overdueContractsDialogVisible.value = true
  } else {
    ElMessage.warning('没有逾期合同')
  }
}

const selectedNewPaymentDate = ref('')
const handleModifyPaymentDate = (row) => {
  // 重置日期
  selectedNewPaymentDate.value = ''
  const DatePickerVNode = h(ElDatePicker, {
    modelValue: selectedDate.value,
    'onUpdate:modelValue': (val) => {
      selectedNewPaymentDate.value = val
      // 强制更新 VNode
      DatePickerVNode.component.props.modelValue = val
    },
    type: 'date',
    placeholder: '请选择日期',
    style: 'width: 100%',
    valueFormat: 'YYYY-MM-DD',
    locale: zhCn,
    disabledDate: (time) => time.getTime() < Date.now() - 8.64e7,
  })
  ElMessageBox({
    title: '申请修改收款日期',
    message: () => DatePickerVNode,
    showCancelButton: true,
    confirmButtonText: '提交审批',
    cancelButtonText: '取消',
    beforeClose: (action, instance, done) => {
      if (action === 'confirm' && !selectedDate.value) {
        ElMessage.warning('请选择日期')
        return
      }
      done()
    }
  }).then(async () => {
    try {
      const res = await request({
        url: '',
        method: 'GET',
        params: {
          Id: row.id,  // 出运合同ID
          ReceivableDate: selectedNewPaymentDate.value  // 新的收款日期
        }
      })
      if (res.code === 200) {
        if (res.data == true) {
          // 获取逾期交货合同数量
          GetOverdueDeliveryContractCount()
          ElMessage.success('收款日期修改申请已提交！')
        } else {
          ElMessage.error('收款日期修改申请提交失败！')
        }
      } else {
        ElMessage.error(res.msg)
      }
    } catch (error) {
      ElMessage.error('提交审批失败')
    }
  }).catch(() => {
    ElMessage.info('已取消修改')
  })
}

// 添加任务完成附件相关变量
const completionFileList = ref([])
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
//#endregion

const pendingEmailList = ref([])
const overdueEmailList = ref([])
const pendingEmailCount = ref(0)
const overdueEmailCount = ref(0)
const pendingEmailDialogVisible = ref(false)
const overdueEmailDialogVisible = ref(false)
const userStore = useUserStore()

// 获取24小时内需要回复的邮件数量
const getWithin24hoursEmailCount = async () => {
  try {
    const response = await request({
      url: 'Email/GetWithin24hoursEmail/GetWithin24hoursEmail',
      method: 'GET',
      params: {
        UserID: userStore.userId
      }
    })
    if (response.code === 200) {
      pendingEmailCount.value = response.data.length
      pendingEmailList.value = response.data
    } else {
      console.error('获取待处理邮件数量失败:', response.msg)
    }
  } catch (error) {
    console.error('获取待处理邮件数量出错:', error)
  }
}
const getOutside24hoursEmailCount = async () => {
  try {
    const response = await request({
      url: 'Email/GetOutside24hoursEmail/GetOutside24hoursEmail',
      method: 'GET',
      params: {
        UserID: userStore.userId
      }
    })
    if (response.code === 200) {
      overdueEmailCount.value = response.data.length
      overdueEmailList.value = response.data
    }
  } catch (error) {
    console.error('获取超时未处理邮件数量出错:', error)
  }
}

onMounted(async () => {
  // 监听更新待办数量事件
  eventBus.on('updatePendingCount', () => {
    getPendingCount();
  });
  try {
    await Promise.all([
      getWithin24hoursEmailCount(),
      getOutside24hoursEmailCount(),
      fetchTaskReminderData(),
      fetchDashboardData()
    ])
  } catch (error) {
    console.error('数据加载失败:', error)
    ElMessage.error('数据加载失败，请刷新页面重试')
  }
})
// 组件卸载时清理事件监听
onUnmounted(() => {
  eventBus.off('updatePendingCount');
});

const showPendingEmails = async () => {
  if (pendingEmailCount.value === 0) {
    ElMessage.warning('没有待处理邮件')
    return;
  } else {
    await getWithin24hoursEmailCount();
    pendingEmailDialogVisible.value = true
  }
}

const showOverdueEmails = async () => {
  if (overdueEmailCount.value === 0) {
    ElMessage.warning('没有超时未处理邮件')
    return;
  } else {
    await getOutside24hoursEmailCount();
    overdueEmailDialogVisible.value = true
  }
}


const router = useRouter()
const handleCommunicationRowDblClick = (row) => {
  // 关闭当前对话框
  communicationOverdueDialogVisible.value = false
  // 跳转到客户资料页面
  router.push({
    path: '/sale/customerinfomation',
    query: {
      customerId: row.customerId
    }
  })
}
const handleOverdueEmailRowDblClick = (row) => {
  // 关闭当前对话框
  overdueEmailDialogVisible.value = false
  // 跳转到email页面
  router.push({
    path: '/email',
    query: {
      id: row.id
    }
  })
}

</script>


<style lang="scss" scoped>
.work-wrap span {
  cursor: pointer;
}

//财务任务 图片预览样式开始
.image-error {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 50px;
  height: 50px;
  background: #f5f7fa;
  color: #909399;
}

/* 自定义预览图片的样式 */
:deep(.el-image-viewer__wrapper) {
  .el-image-viewer__close {
    color: #fff;
  }

  .el-image-viewer__actions {
    opacity: 1;
    background: rgba(0, 0, 0, 0.7);
  }

  .el-image-viewer__prev,
  .el-image-viewer__next {
    color: #fff;
  }
}

/* 图片容器hover效果 */
.el-image {
  transition: transform 0.3s;

  &:hover {
    transform: scale(1.05);
  }
}

//财务任务 图片预览样式结束

.home {
  .home-card-more {
    float: right;
    padding: 3px 0;
    font-size: 13px;
  }

  .user-item {
    // height: 198px;
    display: flex;
    align-items: center;

    .user-item-left {
      width: 60px;
      height: 60px;
      overflow: hidden;
      margin-right: 10px;
    }

    .user-item-right {
      flex: 1;

      .right-title {
        font-size: 20px;
      }
    }
  }

  .info {
    height: 200px;
    // overflow-y: scroll;
  }

  .work-wrap {
    display: grid;
    grid-template-columns: repeat(2, 50%);

    .item {
      text-align: center;

      .name {
        color: #606666;
      }
    }
  }

  .work-wrap2 {
    display: grid;
    grid-template-columns: repeat(3, 40%);

    .item {
      text-align: center;

      .name {
        color: #606666;
      }
    }
  }
}

.chart-wrapper {
  background: var(--base-bg-main);
  padding: 16px 16px 0;
  margin-bottom: 32px;
}

@media (max-width: 1024px) {
  .chart-wrapper {
    padding: 8px;
  }
}

:deep .el-calendar__body {
  padding: 4px 20px 35px;
}

:deep .el-calendar-table thead th {
  color: #ffab11;
  font-weight: bold;
  font-size: 25px;
}

.tag {
  display: flex;
  align-items: center;
  height: 2.5vh;
  justify-content: center;
}

.tag2 {
  display: flex;
  align-items: center;
  height: 2.5vh;
  justify-content: center;
  font-size: 18px;
}

.aaa .is-selected .yuyue {
  color: #ffab11;
}

.aaa .datastyle {
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 3rem;
  color: #353636;
  font-weight: 600;
}

:deep .el-calendar-table .el-calendar-day:hover {
  background-color: #faecd8;
}

:deep .el-calendar {
  --el-calendar-selected-bg-color: #faecd8;
}

.aaa .is-selected .datastyle {
  color: #ffab11;
}

.aaa .prev .datastyle {
  color: #c4c5c8;
}

.aaa .next .datastyle {
  color: #c4c5c8;
}

:deep .el-calendar {
  --el-calendar-cell-width: 130px;
}

.center {
  display: flex;
  justify-content: center;
  align-items: center;
}

.center2 {
  display: flex;
  justify-content: center;
  align-items: center;
  color: #ffab11;
  margin-left: 5px;
  font-size: 20px;
}

:deep .el-calendar__header {
  justify-content: center;
}

/* 日历整体样式 */
.custom-calendar {
  margin: 20px 0;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}

/* 日历单元格样式 */
.calendar-cell {
  min-height: 80px;
  padding: 8px;
}

/* 日期文本样式 */
.date-text {
  font-size: 14px;
  margin-bottom: 8px;
  color: #606266;
}

/* 选中日期的样式 */
.date-text.is-selected {
  color: #409EFF;
  font-weight: bold;
}

/* 选中图标样式 */
.selected-icon {
  margin-left: 4px;
  color: #67C23A;
}

/* 任务标签容器样式 */
.task-tag-container {
  margin: 4px 0;
}

/* 任务标签样式 */
.task-tag {
  width: 100%;
  margin: 2px 0;
}

/* 任务标签文本样式 */
.task-label {
  font-size: 12px;
}

/* 任务数量样式 */
.task-count {
  text-align: right;
  font-weight: bold;
}

/* 有任务时的样式 */
.has-tasks {
  color: #E6A23C;
}

/* 无任务时的样式 */
.no-tasks {
  color: #909399;
}

/* 添加今天日期的样式 */
.date-text.is-today {
  color: #409EFF;
  font-weight: bold;
  background-color: #ecf5ff;
  border-radius: 4px;
  padding: 2px 4px;
}

/* 今天图标样式 */
.today-icon {
  display: inline-block;
  padding: 0 4px;
  height: 18px;
  line-height: 18px;
  background-color: #409EFF;
  color: white;
  font-size: 12px;
  text-align: center;
  border-radius: 9px;
  /* 改为圆角矩形 */
  margin-left: 4px;
}

/* 选中日期的样式调整 */
.date-text.is-selected {
  color: #409EFF;
  font-weight: bold;
  background-color: #ecf5ff;
  border-radius: 4px;
  padding: 2px 4px;
}

/* 同时选中且是今天的样式 */
.date-text.is-today.is-selected {
  background-color: #409EFF;
  color: white;
}

/* 添加过渡效果 */
.date-text {
  transition: all 0.3s ease;
}

/* 悬停效果 */
.calendar-cell:hover .date-text:not(.is-selected):not(.is-today) {
  background-color: #f5f7fa;
  border-radius: 4px;
  padding: 2px 4px;
}

.tasks-container {
  margin-top: 4px;
  max-height: 60px;
  overflow-y: auto;
}

.task-item {
  margin: 2px 0;
}

.task-item .el-tag {
  width: 100%;
  text-overflow: ellipsis;
  overflow: hidden;
  white-space: nowrap;
}

.task-item.is-overdue .el-tag {
  background-color: var(--el-color-danger-light-9);
  border-color: var(--el-color-danger);
  color: var(--el-color-danger);
}

/* 自定义滚动条样式 */
.tasks-container::-webkit-scrollbar {
  width: 4px;
}

.tasks-container::-webkit-scrollbar-thumb {
  background-color: #E4E7ED;
  border-radius: 2px;
}

.tasks-container::-webkit-scrollbar-track {
  background-color: transparent;
}

/* 自定义对话框样式 */
.custom-dialog :deep(.el-dialog) {
  margin-top: 8vh !important;
}

.custom-dialog :deep(.el-dialog__body) {
  padding: 10px 20px;
  height: 450px;
  overflow-y: auto;
}

/* 表格样式优化 */
.custom-dialog :deep(.el-table) {
  border-radius: 4px;
}
</style>
