<template>
  <div class="home">
    <!-- 顶部汇率 + 任务看板 -->
    <el-row :gutter="12" class="dashboard-header">
      <el-col :lg="6" :md="12" :sm="24" class="mb8 exchange-rate-container">
        <el-card class="dashboard-card exchange-rate-card" shadow="hover">
          <div class="card-header">
            <el-icon class="card-icon">
              <Money />
            </el-icon>
            <span class="card-title">当日汇率</span>
          </div>
          <div class="exchange-rate-content">
            <div class="exchange-rate-list" v-if="todayExchangeRates.length > 0">
              <div v-for="rate in todayExchangeRates" :key="rate.currency" class="metric-row">
                <span class="metric-label">{{ rate.currencyName }}</span>
                <span class="metric-value">{{ rate.exchangeRate }}</span>
              </div>
            </div>
            <div v-else class="no-exchange-rate">
              <span>暂无数据</span>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="6" :md="12" :sm="24" class="mb8">
        <el-card class="dashboard-card" shadow="hover">
          <div class="card-header">
            <el-icon class="card-icon">
              <Clock />
            </el-icon>
            <span class="card-title">待办流程</span>
          </div>
          <div class="card-content">
            <div class="metric-row">
              <span class="metric-label">待您处理</span>
              <el-button type="text" class="metric-value primary" @click="WaitingforyouProcessedClick">
                {{ pendingCount }}
              </el-button>
            </div>
            <div class="metric-row">
              <span class="metric-label">超时未处理</span>
              <el-button type="text" class="metric-value danger" @click="TimeoutNotProcessedClick">
                {{ TimeoutNotProcessedCount }}
              </el-button>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="6" :md="12" :sm="24" class="mb8">
        <el-card class="dashboard-card" shadow="hover">
          <div class="card-header">
            <el-icon class="card-icon">
              <Warning />
            </el-icon>
            <span class="card-title">业务示警</span>
          </div>
          <div class="card-content">
            <div class="metric-row">
              <span class="metric-label">交货逾期</span>
              <el-button type="text" class="metric-value danger" @click="OverdueDeliveryContractClick">
                {{ OverdueDeliveryContractCount }}
              </el-button>
            </div>
            <div class="metric-row">
              <span class="metric-label">货款逾期</span>
              <el-button type="text" class="metric-value danger" @click="PaymentoverdueClick">
                {{ PaymentoverdueCount }}
              </el-button>
            </div>
            <div class="metric-row">
              <span class="metric-label">沟通逾期</span>
              <el-button type="text" class="metric-value danger" @click="CommunicationOverdueClick">
                {{ CommunicationOverdueCount }}
              </el-button>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="6" :md="12" :sm="24" class="mb8">
        <el-card class="dashboard-card" shadow="hover">
          <div class="card-header">
            <el-icon class="card-icon">
              <Message />
            </el-icon>
            <span class="card-title">工作任务</span>
          </div>
          <div class="card-content">
            <div class="metric-row">
              <span class="metric-label">待您处理</span>
              <el-button type="text" class="metric-value primary" @click="showPendingEmails">
                {{ workPendingTotalCount }}
              </el-button>
            </div>
            <div class="metric-row">
              <span class="metric-label">超时未处理</span>
              <el-button type="text" class="metric-value danger" @click="showOverdueEmails">
                {{ overdueEmailCount }}
              </el-button>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-row v-if="isSalesDeptUser()" :gutter="12" class="dashboard-header dashboard-equal-height-row">
      <el-col :lg="12" :md="12" :sm="24" class="mb8">
        <el-card class="dashboard-card customer-statistics-card" shadow="hover" v-loading="customerStatisticsLoading">
          <div class="card-header customer-statistics-header">
            <div class="customer-statistics-title">
              <el-icon class="card-icon">
                <List />
              </el-icon>
              <span class="card-title">客户统计</span>
            </div>
            <el-select v-model="selectedCustomerScope" class="customer-scope-select" size="small"
              :disabled="customerScopeOptions.length === 0" @change="handleCustomerScopeChange">
              <el-option v-for="item in customerScopeOptions" :key="item.value" :label="item.label"
                :value="item.value" />
            </el-select>
          </div>
          <div class="card-content customer-statistics-content">
            <div class="metric-row">
              <span class="metric-label">客户数</span>
              <span class="metric-value primary">{{ customerStatisticsSummary.customerCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">成交客户数</span>
              <span class="metric-value primary">{{ customerStatisticsSummary.dealCustomerCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">复购客户数</span>
              <span class="metric-value primary">{{ customerStatisticsSummary.repurchaseCustomerCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">流失客户数</span>
              <span class="metric-value danger">{{ customerStatisticsSummary.lostCustomerCount }}</span>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :lg="12" :md="12" :sm="24" class="mb8">
        <el-card class="dashboard-card sales-conversion-card" shadow="hover" v-loading="salesConversionLoading">
          <div class="card-header sales-conversion-header">
            <div class="sales-conversion-title">
              <el-icon class="card-icon">
                <List />
              </el-icon>
              <span class="card-title">销售转化统计</span>
            </div>
            <div class="sales-conversion-filters">
              <el-select v-model="selectedSalesConversionScope" class="sales-conversion-filter" size="small"
                :disabled="salesConversionScopeOptions.length === 0">
                <el-option v-for="item in salesConversionScopeOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
              <el-select v-model="selectedSalesConversionStatType" class="sales-conversion-filter" size="small">
                <el-option v-for="item in salesConversionStatTypeOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
              <el-select v-model="selectedSalesConversionYear" class="sales-conversion-filter" size="small">
                <el-option v-for="item in salesConversionYearOptions" :key="item" :label="`${item}年`" :value="item" />
              </el-select>
              <el-select v-if="selectedSalesConversionStatType === 'Month'" v-model="selectedSalesConversionMonth"
                class="sales-conversion-filter" size="small">
                <el-option v-for="item in salesConversionMonthOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
              <el-select v-if="selectedSalesConversionStatType === 'Quarter'" v-model="selectedSalesConversionQuarter"
                class="sales-conversion-filter" size="small">
                <el-option v-for="item in salesConversionQuarterOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
            </div>
          </div>
          <div class="card-content sales-conversion-content">
            <div class="metric-row">
              <span class="metric-label">询盘数</span>
              <span class="metric-value primary">{{ salesConversionSummary.inquiryCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">报价数</span>
              <span class="metric-value primary">{{ salesConversionSummary.quoteCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">沟通中</span>
              <span class="metric-value primary">{{ salesConversionSummary.communicatingCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">再报价</span>
              <span class="metric-value primary">{{ salesConversionSummary.requoteCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">成交数</span>
              <span class="metric-value primary">{{ salesConversionSummary.dealCount }}</span>
            </div>
            <div class="metric-row">
              <span class="metric-label">流失数</span>
              <span class="metric-value danger">{{ salesConversionSummary.lostCount }}</span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-row v-if="isSalesDeptUser()" :gutter="12" class="dashboard-header">
      <el-col :lg="24" :md="24" :sm="24" class="mb8">
        <el-card class="dashboard-card business-summary-card" shadow="hover" v-loading="businessSummaryLoading">
          <div class="card-header business-summary-header">
            <div class="business-summary-title">
              <el-icon class="card-icon">
                <List />
              </el-icon>
              <span class="card-title">经营数据统计</span>
            </div>
            <div class="business-summary-filters">
              <el-select v-model="selectedBusinessSummaryScope" class="business-summary-filter" size="small"
                :disabled="businessSummaryScopeOptions.length === 0">
                <el-option v-for="item in businessSummaryScopeOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
              <el-select v-model="selectedBusinessSummaryYear" class="business-summary-filter" size="small">
                <el-option v-for="item in businessSummaryYearOptions" :key="item" :label="`${item}年`" :value="item" />
              </el-select>
            </div>
          </div>

          <div class="business-summary-totals">
            <div class="business-summary-total-item">
              <div class="business-summary-total-label">本年总成交</div>
              <div class="business-summary-total-value">{{
                formatBusinessAmountDisplay(businessSummarySummary.totalDealAmount) }}</div>
            </div>
            <div class="business-summary-total-item">
              <div class="business-summary-total-label">已出运</div>
              <div class="business-summary-total-value">{{
                formatBusinessAmountDisplay(businessSummarySummary.shippedAmount) }}</div>
            </div>
            <div class="business-summary-total-item">
              <div class="business-summary-total-label">预估毛利</div>
              <div class="business-summary-total-value">{{
                formatBusinessAmountDisplay(businessSummarySummary.estimatedGrossProfit) }}</div>
            </div>
          </div>

          <div class="business-summary-chart-wrap">
            <svg class="business-summary-chart" viewBox="0 0 1000 240" preserveAspectRatio="none">
              <polyline class="business-summary-line deal-line" :points="businessSummaryDealLinePoints" />
              <polyline class="business-summary-line shipped-line" :points="businessSummaryShippedLinePoints" />
            </svg>
            <div class="business-summary-month-labels">
              <span v-for="month in 12" :key="month" class="business-summary-month-item">{{ month }}</span>
            </div>
            <div class="business-summary-legend">
              <span class="business-summary-legend-item deal">
                绿色是 月度成交金额
              </span>
              <span class="business-summary-legend-item shipped">
                橙色是 月度出运金额
              </span>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 左侧：日历和任务 -->
      <div :class="['left-panel', { 'full-width': isPurchaseRole() }]">
        <!-- 紧凑型日历 -->
        <el-card class="calendar-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <el-icon class="card-icon">
                <Calendar />
              </el-icon>
              <span class="card-title">计划任务日历</span>
            </div>
          </template>
          <div class="compact-calendar">
            <div class="calendar-header">
              <el-button-group>
                <el-button size="small" @click="previousMonth">
                  <el-icon>
                    <ArrowLeft />
                  </el-icon>
                </el-button>
                <span class="current-month">{{ currentMonthYear }}</span>
                <el-button size="small" @click="nextMonth">
                  <el-icon>
                    <ArrowRight />
                  </el-icon>
                </el-button>
              </el-button-group>
            </div>
            <div class="calendar-grid">
              <div class="calendar-weekdays">
                <div v-for="day in weekdays" :key="day" class="weekday">{{ day }}</div>
              </div>
              <div class="calendar-days">
                <div v-for="date in calendarDays" :key="date.date" :class="['calendar-day', {
                  'other-month': !date.isCurrentMonth,
                  'today': date.isToday,
                  'has-tasks': date.taskCount > 0
                }]" @click="selectDate(date)">
                  <div class="day-number">{{ date.day }}</div>
                  <div v-if="date.taskCount > 0" class="task-indicator">
                    <el-tag size="small" :type="date.hasOverdue ? 'danger' : 'warning'">
                      {{ date.taskCount }}
                    </el-tag>
                  </div>
                </div>
              </div>
              <!-- 调试信息 -->
              <div v-if="calendarDays.length === 0" style="text-align: center; padding: 20px; color: #999;">
                日历数据加载中...
              </div>
            </div>
            <!-- 颜色含义提示 -->
            <div class="calendar-legend">
              <div class="legend-title">颜色说明：</div>
              <div class="legend-items">
                <div class="legend-item">
                  <div class="legend-color normal"></div>
                  <span class="legend-text">普通日期</span>
                </div>
                <div class="legend-item">
                  <div class="legend-color today"></div>
                  <span class="legend-text">今天</span>
                </div>
                <div class="legend-item">
                  <div class="legend-color has-tasks"></div>
                  <span class="legend-text">有任务</span>
                </div>
                <div class="legend-item">
                  <div class="legend-color other-month"></div>
                  <span class="legend-text">其他月份</span>
                </div>
              </div>
            </div>
          </div>
        </el-card>

        <!-- 工作任务 -->
        <!-- 提醒事项和财务任务合并表格 -->
        <el-card class="tasks-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <el-icon class="card-icon">
                <List />
              </el-icon>
              <span class="card-title">待办事项</span>
              <el-tabs v-model="tasksActiveTab" class="compact-tabs">
                <el-tab-pane label="提醒事项" name="reminders"></el-tab-pane>
                <el-tab-pane label="财务任务" name="financial"></el-tab-pane>
                <el-tab-pane v-if="isAdminRole()" label="待删除单据" name="documentDeleteRequest"></el-tab-pane>
              </el-tabs>
            </div>
          </template>

          <!-- 提醒事项 -->
          <div v-if="tasksActiveTab === 'reminders'" class="tasks-content">
            <el-table :data="TaskReminderTableData" size="small" :max-height="200">
              <el-table-column prop="taskName" label="项目分类" width="120" />
              <el-table-column prop="taskDescription" label="内容" />
              <el-table-column prop="reminderTime" label="时间节点" width="120">
                <template #default="scope">
                  {{ formatDateTime(scope.row.reminderTime) }}
                </template>
              </el-table-column>
            </el-table>
            <el-pagination v-model:current-page="TaskReminderTableCurrentPage"
              v-model:page-size="TaskReminderTablePageSize" :total="TaskReminderTableTotalItems"
              @current-change="TaskReminderTableshandlePageChange" layout="total, prev, pager, next" size="small" />
          </div>

          <!-- 财务任务 -->
          <div v-if="tasksActiveTab === 'financial'" class="tasks-content">
            <el-table :data="FinancialTasksTableData" size="small" :max-height="200">
              <el-table-column prop="receiptNumber" label="收款单号" width="120" />
              <el-table-column prop="receiptDate" label="收汇日期" width="100" />
              <el-table-column prop="ourCompany" label="我方公司" width="100" />
              <el-table-column prop="amount" label="金额" width="100">
                <template #default="{ row }">
                  {{ formatAmountWithCurrency(row.amount, row.foreignCurrencyValue) }}
                </template>
              </el-table-column>
              <el-table-column prop="bank" label="收汇银行" width="100" />
              <el-table-column fixed="right" label="操作" width="80">
                <template #default="{ row }">
                  <el-button link type="primary" size="small" @click="handleClaim(row)">领取</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-pagination v-model:current-page="FinancialTasksTableCurrentPage"
              v-model:page-size="FinancialTasksTablePageSize" :total="FinancialTasksTableTotalItems"
              @current-change="FinancialTasksTableshandlePageChange" layout="total, prev, pager, next" size="small" />
          </div>
          <!-- 待删除单据（管理员） -->
          <div v-if="tasksActiveTab === 'documentDeleteRequest' && isAdminRole()" class="tasks-content">
            <el-table :data="documentDeleteRequestList" size="small" :max-height="200">
              <el-table-column prop="documentType" label="单据类型" width="130">
                <template #default="{ row }">
                  <span>{{ getDeleteRequestDocumentTypeLabel(row.documentType) }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="documentNo" label="单据编号" width="140" />
              <el-table-column prop="applyReason" label="申请原因" min-width="160" show-overflow-tooltip />
              <el-table-column prop="approveRemark" label="备注" min-width="140" show-overflow-tooltip />
              <el-table-column prop="applyUser" label="申请人" width="100">
                <template #default="{ row }">
                  <span>{{ getDeleteRequestApplyUserLabel(row.applyUser) }}</span>
                </template>
              </el-table-column>
              <el-table-column prop="approveStatus" label="审批状态" width="100" v-if="false">
                <template #default="{ row }">
                  <el-tag :type="getDeleteRequestApproveStatusType(row.approveStatus)">
                    {{ getDeleteRequestApproveStatusLabel(row.approveStatus) }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column label="操作" width="100" align="center" fixed="right">
                <template #default="{ row }">
                  <el-button type="danger" link size="small" @click="handleDelDocumentRequest(row)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-pagination v-model:current-page="documentDeleteRequestCurrentPage"
              v-model:page-size="documentDeleteRequestPageSize" :total="documentDeleteRequestTotalItems"
              @current-change="documentDeleteRequestHandlePageChange" layout="total, prev, pager, next" size="small" />
          </div>
        </el-card>
      </div>


      <!-- 右侧：商机看板 -->
      <div v-if="isSalesDeptUser()" class="right-panel">
        <el-card class="opportunities-card" shadow="hover">
          <template #header>
            <div class="card-header">
              <el-icon class="card-icon">
                <Message />
              </el-icon>
              <span class="card-title">商机看板</span>
            </div>
          </template>
          <div class="opportunities-container">
            <div class="opportunities-grid">
              <div v-for="stage in sortedStages" :key="stage.salesStage" class="opportunity-column">
                <div class="column-header">
                  <span class="stage-name">{{ stage.salesStage }}</span>
                  <span class="stage-count">({{ stage.count }})</span>
                </div>
                <div class="column-amount">
                  <span class="amount-text">
                    CNY {{ shouldShowZeroAmount(stage.salesStage) ? '0.00' : formatAmount(stage.totalAmount) }}
                  </span>
                </div>
                <div class="opportunities-list">
                  <div v-for="item in stage.details" :key="item.id" class="opportunity-item"
                    @dblclick="handleOpportunityClick(item)">
                    <div class="item-header">
                      <span class="item-number">
                        {{ item.sourceType === 'api' && (stage.salesStage === '初次报价' || stage.salesStage === '再次报价')
                          ? `报价单号：${item.opportunityNumber}`
                          : item.sourceType === 'api' && stage.salesStage === '合同确定'
                            ? `合同编号：${item.opportunityNumber}`
                            : `商机编号：${item.opportunityNumber}` }}
                      </span>
                    </div>
                    <div class="item-content">
                      <div class="item-row">
                        <span class="item-label">客户：</span>
                        <el-tooltip :content="item.businessName" placement="top"
                          :disabled="item.businessName.length <= 8">
                          <span class="item-value truncate">{{ truncateText(item.businessName, 8) }}</span>
                        </el-tooltip>
                      </div>
                      <div class="item-row">
                        <span class="item-label">时间：</span>
                        <span class="item-value">{{ formatDate(item.create_time) }}</span>
                      </div>
                      <div v-if="shouldShowAmount(stage.salesStage)" class="item-row">
                        <span class="item-label">金额：</span>
                        <span class="item-value">{{ formatAmount(item.amount) }}</span>
                      </div>
                    </div>
                  </div>

                </div>
              </div>
            </div>
          </div>
        </el-card>
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
      <el-tabs v-model="AgencyProcessdialogTableActiveName" type="card" @tab-click="handleTabClick">
        <el-tab-pane label="待审批单据" name="first">
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
        </el-tab-pane>
        <el-tab-pane v-if="userStore.userInfo && userStore.userInfo.deptId === 210">
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <document />
              </el-icon>
              <span>询价需求</span>
            </span>
          </template>
          <el-table :data="inquiryList" :height="400" style="width: 100%" @row-dblclick="handleInquiryRowDblClick">
            <el-table-column prop="inquiryNumber" label="询价单号" width="120"></el-table-column>
            <el-table-column prop="productName" label="产品名称" width="180"></el-table-column>
            <!-- <el-table-column prop="quoteQuantity" label="报价数量" width="120"></el-table-column> -->
            <el-table-column prop="create_time" label="创建时间" width="180">
              <template #default="scope">
                {{ scope.row.create_time }}
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane name="inquiryReplied">
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <document />
              </el-icon>
              <el-badge :value="inquiryRepliedCount" :hidden="inquiryRepliedCount === 0">
                <span>询价已回复</span>
              </el-badge>
            </span>
          </template>
          <el-table :data="inquiryRepliedList" :height="400" style="width: 100%"
            @row-dblclick="handleInquiryRepliedRowDblClick">
            <el-table-column prop="inquiry_number" label="询价单号" width="150"></el-table-column>
            <el-table-column prop="subject" label="询价主题" width="200"></el-table-column>
            <el-table-column prop="createTime" label="创建时间" width="180">
              <template #default="scope">
                {{ scope.row.createTime }}
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane v-if="userStore.userInfo && userStore.userInfo.deptId === 210">
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <document />
              </el-icon>
              <span>采购需求</span>
            </span>
          </template>
          <el-table :data="shoppinglisttableData" style="width: 100%" :span-method="mergeCells"
            @row-dblclick="shoppinglisttableDatahandleRowDblClick">
            <el-table-column prop="contractNumber" label="销售合同号" width="150"></el-table-column>
            <el-table-column prop="contractId" label="销售合同ID" width="150" v-if="false"></el-table-column>
            <el-table-column prop="productId" label="产品ID" width="150" v-if="false"></el-table-column>
            <el-table-column prop="productName" label="产品名称" width="150"></el-table-column>
            <el-table-column prop="quantity" label="数量" width="150"></el-table-column>
            <el-table-column prop="unitPrice" label="采购单价" width="150" v-if="false"></el-table-column>
            <el-table-column prop="totalPrice" label="采购总价" width="150" v-if="false"></el-table-column>
            <el-table-column prop="salesperson" label="销售员" width="150" v-if="false"></el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane v-if="userStore.userInfo && userStore.userInfo.deptId === 213" name="paymentTask">
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <document />
              </el-icon>
              <span>付款任务</span>
            </span>
          </template>
          <el-table :data="paymentTaskList" :height="400" style="width: 100%"
            @row-dblclick="handlePaymentTaskRowDblClick">
            <el-table-column prop="applicationDate" label="申请日期" width="120"></el-table-column>
            <el-table-column prop="payeeUnit" label="收款单位" width="180"></el-table-column>
            <el-table-column prop="paymentItemName" label="款项名称" width="200"></el-table-column>
            <el-table-column prop="paymentAmount" label="付款金额" width="150">
              <template #default="scope">
                {{ scope.row.currency }} {{ scope.row.paymentAmount.toLocaleString() }}
              </template>
            </el-table-column>
            <el-table-column prop="applicant" label="申请人" width="120"></el-table-column>
            <el-table-column fixed="right" prop="operate" label="操作" width="120" align="center">
              <template v-slot:default="scope">
                <el-button link type="primary" size="small" @click="viewPaymentTask(scope.row)">查看详情</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="采购价格变更" name="purchasePriceChange">
          <el-table :data="purchasePriceChangeList" :height="400" style="width: 100%"
            @row-dblclick="handlePurchasePriceChangeRowDblClick">
            <el-table-column prop="contractNumber" label="销售合同号" width="160"></el-table-column>
            <el-table-column prop="create_time" label="创建时间" width="180">
              <template #default="{ row }">
                <span>{{ formatDate(row.create_time || row.createTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column fixed="right" prop="operate" label="操作" width="120" align="center">
              <template v-slot:default="scope">
                <el-button link type="primary" size="small"
                  @click="viewPurchasePriceChangeContract(scope.row)">查看详情</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane v-if="isSalesRole()" label="收款单领取" name="receiptClaim">
          <el-table :data="FinancialTasksTableData" :height="400" style="width: 100%">
            <el-table-column prop="receiptNumber" label="收款单号" width="120" />
            <el-table-column prop="ourCompany" label="我方公司" width="120" />
            <el-table-column prop="amount" label="金额" width="200">
              <template #default="{ row }">
                {{ formatAmountWithCurrency(row.amount, row.foreignCurrencyValue) }}
              </template>
            </el-table-column>
            <el-table-column prop="bank" label="收汇银行" width="120" />
            <el-table-column fixed="right" label="操作" width="100" align="center">
              <template #default="{ row }">
                <el-button link type="primary" size="small" @click="handleClaim(row)">领取</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination v-model:current-page="FinancialTasksTableCurrentPage"
            v-model:page-size="FinancialTasksTablePageSize" :total="FinancialTasksTableTotalItems"
            @current-change="FinancialTasksTableshandlePageChange" layout="total, prev, pager, next" size="small" />
        </el-tab-pane>
        <el-tab-pane v-if="isFinanceRole()" label="收款单确认" name="receiptConfirm">
          <el-table :data="receiptConfirmTableData" :height="400" style="width: 100%"
            @row-dblclick="handleReceiptConfirmRowDblClick">
            <el-table-column prop="receiptNumber" label="收款单号" width="120" />
            <el-table-column prop="ourCompany" label="我方公司" width="120" />
            <el-table-column prop="amount" label="金额" width="200">
              <template #default="{ row }">
                {{ formatAmountWithCurrency(row.amount, row.foreignCurrencyValue) }}
              </template>
            </el-table-column>
            <el-table-column prop="bank" label="收汇银行" width="120" />
          </el-table>
          <el-pagination v-model:current-page="receiptConfirmTableCurrentPage"
            v-model:page-size="receiptConfirmTablePageSize" :total="receiptConfirmTableTotalItems"
            @current-change="receiptConfirmTableHandlePageChange" layout="total, prev, pager, next" size="small" />
        </el-tab-pane>
        <el-tab-pane name="reject">
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <document />
              </el-icon>
              <span>审核驳回单据</span>
            </span>
          </template>
          <!-- 被驳回的采购合同表格 -->
          <div style="margin-bottom: 20px;" v-if="rejectPurchaseContractList && rejectPurchaseContractList.length > 0">
            <el-table :data="rejectPurchaseContractList" :height="200" style="width: 100%"
              @row-dblclick="handleRejectPurchaseContractRowDblClick">
              <el-table-column prop="purchaseContractNumber" label="采购合同号" width="150"></el-table-column>
              <el-table-column prop="createTime" label="创建时间" width="180"></el-table-column>
              <el-table-column fixed="right" prop="operate" label="操作" width="120" align="center">
                <template v-slot:default="scope">
                  <el-button link type="primary" size="small"
                    @click="viewRejectPurchaseContract(scope.row)">查看详情</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
          <!-- 被驳回的销售合同表格 -->
          <div v-if="rejectContractList && rejectContractList.length > 0">
            <el-table :data="rejectContractList" :height="200" style="width: 100%"
              @row-dblclick="handleRejectContractRowDblClick">
              <el-table-column prop="contractNumber" label="销售合同号" width="150"></el-table-column>
              <el-table-column prop="createTime" label="创建时间" width="180"></el-table-column>
              <el-table-column fixed="right" prop="operate" label="操作" width="120" align="center">
                <template v-slot:default="scope">
                  <el-button link type="primary" size="small" @click="viewRejectContract(scope.row)">查看详情</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>

          <!-- 当两个列表都为空时显示提示信息 -->
          <div
            v-if="(!rejectPurchaseContractList || rejectPurchaseContractList.length === 0) && (!rejectContractList || rejectContractList.length === 0)"
            style="text-align: center; padding: 40px; color: #909399;">
            <el-icon size="48" style="margin-bottom: 16px;">
              <document />
            </el-icon>
            <p style="font-size: 16px; margin: 0;">暂无被驳回的单据</p>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-dialog>
    <el-dialog v-model="contractDialog" title="销售合同审批" :close-on-click-modal=false style="width: 75%;">
      <el-collapse v-model="basicInfoCollapseActive" style="margin-bottom: 20px;">
        <el-collapse-item title="基本信息" name="basicInfo">
          <template #title>
            <span style="font-size: 20px; font-weight: bold;">
              基本信息<span v-if="contractReviewStatus">【{{ contractReviewStatus }}】</span>
            </span>
          </template>
          <el-form :model="contractform" label-width="120px">
            <el-row>
              <el-col :span="6">
                <el-form-item label="销售合同">
                  <el-input v-model="contractform.contractNumber" disabled style="width: 300px"
                    placeholder="自动生成"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="合同日期">
                  <el-date-picker v-model="contractform.contractDate" type="date" placeholder="请选择合同日期" disabled
                    style="width: 300px"></el-date-picker>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="合同状态">
                  <el-select filterable v-model="contractform.contractStatus" placeholder="请选择合同状态" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="客户编号" prop="customerNumber">
                  <el-select filterable v-model="contractform.customerNumber" placeholder="请选择客户编号" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="客户简称" prop="customerAbbreviation">
                  <el-select v-model="contractform.customerAbbreviation" filterable placeholder="请选择客户简称" disabled
                    clearable style="width: 300px;">
                    <el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="报价单号">
                  <el-input v-model="contractform.quotationNumber" disabled style="width: 300px"
                    placeholder="自动生成"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="联系人" prop="contactPerson">
                  <el-select filterable v-model="contractform.contactPerson" placeholder="请选择联系人" style="width: 300px"
                    clearable disabled>
                    <el-option v-for="item in contactpersonSelectOptions" :key="item.value" :label="item.label"
                      :value="item.value" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="生效日期">
                  <el-date-picker v-model="contractform.effectiveDate" type="date" placeholder="请选择生效日期" disabled
                    style="width: 300px"></el-date-picker>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="客户等级">
                  <el-select filterable v-model="contractform.customerLevel" placeholder="请选择客户等级" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="联系人Email">
                  <el-input v-model="contractform.contactEmail" disabled style="width: 300px"></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="客户合同">
                  <el-input v-model="contractform.customerContract" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="交货日期" prop="deliveryDate">
                  <el-date-picker v-model="contractform.deliveryDate" type="date" placeholder="请选择交货日期" disabled
                    style="width: 300px"></el-date-picker>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="我方公司" prop="ourCompany">
                  <el-select filterable v-model="contractform.ourCompany" placeholder="请选择我方公司" style="width: 300px"
                    clearable disabled>
                    <el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="外销币种" prop="foreignCurrency">
                  <el-select filterable v-model="contractform.foreignCurrency" placeholder="请选择外销币种" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="结算类别" prop="settlementType">
                  <el-select filterable v-model="contractform.settlementType" placeholder="请选择结算类别" disabled clearable
                    style="width: 300px">
                    <el-option label="类别1" value="1"></el-option>
                    <el-option label="类别2" value="2"></el-option>
                    <el-option label="类别3" value="3"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="汇率" prop="exchangeRate">
                  <el-input v-model="contractform.exchangeRate" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="结汇方式" prop="settlementMethod">
                  <el-select filterable v-model="contractform.settlementMethod" placeholder="请选择结汇方式" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="价格条款" prop="priceTerms">
                  <el-select filterable v-model="contractform.priceTerms" placeholder="请选择价格条款" style="width: 300px"
                    clearable disabled>
                    <el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="出运口岸" prop="shippingPort">
                  <el-select filterable v-model="contractform.shippingPort" placeholder="请选择出运口岸" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="美金汇率" prop="usdExchangeRate">
                  <el-input v-model="contractform.usdExchangeRate" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6">
                <el-form-item label="目的地/港" prop="destinationPort">
                  <el-input v-model="contractform.destinationPort" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="贸易国别" prop="tradeCountry">
                  <el-select filterable v-model="contractform.tradeCountry" placeholder="请选择贸易国别" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="运输方式" prop="transportation">
                  <el-select filterable v-model="contractform.transportation" placeholder="请选择运输方式" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
                      :label="dict.dictLabel" :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="销售员" prop="salesperson">
                  <el-select filterable v-model="contractform.salesperson" placeholder="请选择销售员" style="width: 300px"
                    clearable disabled>
                    <el-option v-for="dict in optionss.sql_hr_sale" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6">
                <el-form-item label="有无预付款" prop="hasDeposit">
                  <el-checkbox v-model="contractform.hasDeposit" disabled></el-checkbox>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="预付款金额" v-show=DepositShow prop="Depositratio">
                  <el-input v-model="contractform.Depositratio" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="hasReceivedDepositShow">
                <el-form-item label="已收预付款" v-show=DepositShow prop="receivedDeposit">
                  <el-input v-model="contractform.receivedDeposit" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="预付款日期" v-show=DepositShow prop="depositDate">
                  <el-date-picker v-model="contractform.depositDate" type="date" placeholder="请选择预付款日期" disabled
                    style="width: 300px"></el-date-picker>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-collapse-item>
      </el-collapse>
      <el-collapse v-model="auxiliaryInfoCollapseActive" style="margin-bottom: 20px;">
        <el-collapse-item title="辅助信息" name="auxiliaryInfo">
          <template #title>
            <span style="font-size: 20px; font-weight: bold;">辅助信息</span>
          </template>
          <el-form :model="contractform" label-width="120px">
            <el-row v-if="false">
              <el-col :span="6" v-if="false">
                <el-form-item label="签约地点">
                  <el-select filterable v-model="contractform.signingLocation" placeholder="请选择签约地点" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.hr_signing_place" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="可否分批">
                  <el-select filterable v-model="contractform.canPartial" placeholder="请选择可否分批" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="可否转运">
                  <el-select filterable v-model="contractform.canTransit" placeholder="请选择可否转运" disabled clearable
                    style="width: 300px">
                    <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue"></el-option>
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="佣金比率">
                  <el-input v-model="contractform.commissionRate" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="保险加成">
                  <el-input v-model="contractform.insuranceAddition" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="保险比率">
                  <el-input v-model="contractform.insuranceRate" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="海运费/m³">
                  <el-input v-model="contractform.oceanFreight" style="width: 300px;" disabled />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="海运费币种" prop="shippingCurrency">
                  <el-select v-model="contractform.shippingCurrency" filterable placeholder="选择运费币种" disabled clearable
                    style="width: 300px;">
                    <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="海运费汇率">
                  <el-input v-model="contractform.shippingrate" style="width: 300px;" disabled />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="内陆运杂费/m³">
                  <el-input v-model="contractform.portMiscellaneousFees" style="width: 300px;" disabled />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6">
                <el-form-item label="货代港杂费">
                  <el-input v-model="contractform.freightForwarderCustomsClearanceFees" style="width: 300px;"
                    disabled />
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="银行费用">
                  <el-input v-model="contractform.BankFee" style="width: 300px;" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="收汇银行">
                  <el-input v-model="contractform.receivingBank" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="快递费(人民币)">
                  <el-input v-model="contractform.DocumentationFees" style="width: 300px;" disabled />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="付款日期">
                  <el-date-picker v-model="contractform.paymentDate" type="date" placeholder="请选择付款日期" disabled
                    style="width: 300px"></el-date-picker>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-collapse-item>
      </el-collapse>
      <span style="font-size: 20px; font-weight: bold;">产品资料&客户相关费用</span>
      <el-divider></el-divider>
      <el-tabs v-model="activeTab" tab-position="top" class="demo-tabs">
        <el-tab-pane label="产品资料" name="productMaterialtab">
          <el-table :data="productData" style="width: 100%;margin-bottom: 15px; table-layout: fixed;"
            :header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
            :row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
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
            <el-table-column prop="unitofmeasurement" label="计量单位编号" width="100" v-if="false">
              <template #default="scope">
                <el-select v-model="scope.row.unitofmeasurement" filterable placeholder="单位" style="width: 100%;"
                  disabled clearable>
                  <el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue" />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column label="计量单位" width="100">
              <template #default="scope">
                {{scope.row.unitOfMeasurementLabel || state.optionss.hr_calculate_unit.find(x =>
                  x.dictValue == scope.row.unitofmeasurement)?.dictLabel || '-'}}
              </template>
            </el-table-column>
            <el-table-column prop="purchasecurrency" label="采购币种" width="120">
              <template #default="scope">
                <el-select v-model="scope.row.purchasecurrency" filterable placeholder="币种" style="width: 100%;"
                  disabled clearable>
                  <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue" />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column prop="purchaseunitprice" label="采购单价" width="110">
              <template #default="{ row }">
                <el-input v-model="row.purchaseunitprice" disabled :style="row.isPriceChanged === 1 ? {
                  '--el-input-text-color': 'red',
                  '--el-disabled-text-color': 'red',
                } : {}">
                </el-input>
              </template>
            </el-table-column>
            <el-table-column prop="purchasepriceterms" label="采购价格条款" width="180">
              <template #default="{ row }">
                <span>{{optionss.hr_purchase_pricing_term.find(item =>
                  item.dictValue === row.purchasepriceterms?.toString())?.dictLabel || '-'}}</span>
              </template>
            </el-table-column>
            <el-table-column prop=" inlandfreightprice" label="内陆运费(m³)" width="130">
              <template #default="{ row }">
                <el-input v-model="row.inlandfreightprice" disabled />
              </template>
            </el-table-column>
            <el-table-column prop="AdditionalPackagingCosts" label="单个产品额外包装费用" width="185">
              <template #default="{ row }">
                <el-input v-model="row.AdditionalPackagingCosts" disabled />
              </template>
            </el-table-column>
            <el-table-column prop="singleProductGrossProfit" label="单个产品毛利" width="130">
              <template #default="scope">
                <span :class="{ 'red-text': scope.row.isPriceChanged }">{{ scope.row.singleProductGrossProfit }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="singleProductGrossProfitTotal" label="单个产品毛利合计" width="160">
              <template #default="scope">
                <span :class="{ 'red-text': scope.row.isPriceChanged }">{{ scope.row.singleProductGrossProfitTotal
                }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="grossProfitRate" label="毛利率%" width="110">
              <template #default="scope">
                <span :class="{ 'red-text': scope.row.isPriceChanged }">{{ scope.row.grossProfitRate }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="isInvoicingc" label="是否开票" width="120">
              <template #default="scope">
                <el-select v-model="scope.row.isInvoicingc" filterable placeholder="请选择" style="width: 100%;" clearable
                  disabled>
                  <el-option v-for="dict in optionss.hr_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue" />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column prop="packaging" label="包装方式" width="150">
              <template #default="scope">
                <el-select v-model="scope.row.packaging" filterable disabled placeholder="请选择" style="width: 100%;"
                  clearable>
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
                <el-select v-model="scope.row.outerboxunit" filterable placeholder="外箱单位" style="width: 100%;" disabled
                  clearable>
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
            <el-table-column prop="totalNetWeight" label="总净重(KG)" width="130">
              <template #default="scope">
                <span>{{ scope.row.totalNetWeight }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="totalGrossWeight" label="总毛重(KG)" width="130">
              <template #default="scope">
                <span>{{ scope.row.totalGrossWeight }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="totalVolume" label="总体积(m³)" width="130">
              <template #default="scope">
                <span>{{ scope.row.totalVolume }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="OtherFees" label="单个产品其它费用" width="160">
              <template #default="{ row }">
                <el-input v-model="row.OtherFees" disabled />
              </template>
            </el-table-column>
            <el-table-column prop="SinglesalesrevenueA" label="单个销售收入A" width="160" v-if="true">
              <template #default="scope">
                <span :class="{ 'red-text': scope.row.isPriceChanged }">{{ scope.row.SinglesalesrevenueA }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="Singleproductvolume" label="单个产品体积(m³)" width="160" v-if="true">
              <template #default="scope">
                <span>{{ scope.row.Singleproductvolume }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="Portchargesforindividualproducts" label="单个产品的港杂费" width="160" v-if="true">
              <template #default="scope">
                <span>{{ scope.row.Portchargesforindividualproducts }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="Oceanfreightforasingleproduct" label="单个产品海运费" width="130" v-if="true">
              <template #default="scope">
                <span>{{ scope.row.Oceanfreightforasingleproduct }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="Inlandfreightforasingleproduct" label="单个产品内陆运费" width="160" v-if="true">
              <template #default="scope">
                <span>{{ scope.row.Inlandfreightforasingleproduct }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="isPriceChanged" label="是否价格变动" width="140" v-if="true">
              <template #default="scope">
                <span>{{ scope.row.isPriceChanged === 1 ? '是' : '否' }}</span>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="客户相关费用" name="CustomerRelaterExoensestab">
          <el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; table-layout: fixed;"
            :header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
            :row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
            <el-table-column prop=" expenseName" label="费用名称" width="150">
              <template #default="{ row }">
                <el-input v-model="row.expenseName" placeholder="输入费用名称" size="small" disabled></el-input>
              </template>
            </el-table-column>
            <el-table-column prop="currency" label="币种" width="150">
              <template #default="{ row }">
                <el-select filterable v-model="row.currency" placeholder="选择币种" size="small" disabled clearable>
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
        <el-tab-pane label="已收费用详情" name="ReceivedExpenseDetailsTab">
          <el-table :data="ReceivedExpenseDetailsTableData" style="width: 100%; table-layout: fixed;"
            :header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
            :row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }">
            <el-table-column prop="receiptNumber" label="收款单号" width="150"></el-table-column>
            <el-table-column prop="fundsType" label="收款类型" width="150"></el-table-column>
            <el-table-column prop="receiptDate" label="收汇日期" width="150"
              :formatter="(row, column, cellValue) => formatDate(cellValue)"></el-table-column>
            <el-table-column prop="ourCompany" label="我方公司" width="150"></el-table-column>
            <el-table-column prop="foreignCurrency" label="外销币种" width="150"></el-table-column>
            <el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
            <el-table-column prop="amount" label="金额" width="150"></el-table-column>
            <el-table-column prop="bank" label="收汇银行" width="150"></el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>
      <el-collapse v-model="profitCalculationCollapseActive" style="margin-bottom: 20px;">
        <el-collapse-item title="合计信息&利润预估" name="profitCalculation">
          <template #title>
            <span style="font-size: 20px; font-weight: bold;">合计信息&利润预估</span>
          </template>
          <el-form :model="contractform" label-width="120px">
            <el-row>
              <el-col :span="6">
                <el-form-item label="货值合计">
                  <el-input
                    :value="formatTotalValueOfGoods(contractform.TotalValueOfGoods, contractform.foreignCurrency)"
                    style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="数量合计">
                  <el-input v-model="contractform.TotalQuantity" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="箱数合计">
                  <el-input v-model="contractform.TotalNumberOfBoxes" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="毛重合计">
                  <el-input v-model="contractform.TotalGrossWeight" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="净重合计">
                  <el-input v-model="contractform.TotalNetWeight" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6">
                <el-form-item label="体积合计">
                  <el-input v-model="contractform.TotalVolume" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="采购合计">
                  <el-input :value="formatCNYAmount(contractform.TotalPurchases)" style="width: 300px" disabled
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}">
                  </el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="客户费用合计">
                  <el-input v-model="contractform.customerExpenseTotal" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="总毛利合计">
                  <el-input :value="formatCNYAmount(contractform.Totalgrossprofit)" disabled style="width: 300px;"
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="退税总额">
                  <el-input v-model="contractform.TotalTaxRefund" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6" v-if="false">
                <el-form-item label="美金/欧元换算">
                  <el-input v-model="contractform.usdConversion" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="其它费用合计">
                  <el-input :value="formatCNYAmount(contractform.TotalOtherFees)" disabled style="width: 300px;"
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}" />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="利润金额">
                  <el-input :value="formatCNYAmount(contractform.ProfitAmount)" disabled style="width: 300px;"
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}" />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="退税总额">
                  <el-input :value="formatCNYAmount(contractform.TotalTaxRefund)" disabled style="width: 300px;"
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}" />
                </el-form-item>
              </el-col>
              <el-col :span="6">
                <el-form-item label="总利润率%">
                  <el-input :value="formatPercentage(contractform.Totalprofitmargin)" disabled style="width: 300px;"
                    :style="hasChangedProducts ? { '--el-input-text-color': 'red', '--el-disabled-text-color': 'red' } : {}" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row>
              <el-col :span="6" v-if="false">
                <el-form-item label="金额合计">
                  <el-input v-model="contractform.amountTotal" style="width: 300px" disabled></el-input>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-collapse-item>
      </el-collapse>
      <el-form :model="contractform" label-width="120px">
        <el-form-item label="备注信息">
          <el-input v-model="contractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }" type="textarea"
            placeholder="请输入备注信息" disabled />
        </el-form-item>
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
    <el-dialog :modal="false" :modal-penetrable="true" v-model="PurchaseContractDialog" title="采购合同审批"
      :close-on-click-modal=false style="width: 70%;">
      <span style="font-size: 20px; font-weight: bold;">基本信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="供应商" v-if="false">
          {{ PurchaseContractDialogData.vendorCode }}
        </el-descriptions-item>
        <el-descriptions-item label="采购合同">
          {{ PurchaseContractDialogData.purchaseContract }}
        </el-descriptions-item>
        <el-descriptions-item label="合同状态">
          {{ PurchaseContractDialogData.contractStatus }}
        </el-descriptions-item>
        <el-descriptions-item label="交货日期">
          {{ formatDate(PurchaseContractDialogData.deliveryDate) }}
        </el-descriptions-item>
        <el-descriptions-item label="采购币种">
          {{ PurchaseContractDialogData.purchaseCurrency }}
        </el-descriptions-item>
        <el-descriptions-item label="价格条款" v-if="false">
          {{ PurchaseContractDialogData.priceTerms }}
        </el-descriptions-item>
        <el-descriptions-item label="付款天数">
          {{ PurchaseContractDialogData.paymentDays }}
        </el-descriptions-item>
        <el-descriptions-item label="销售合同">
          {{ PurchaseContractDialogData.salesContract }}
        </el-descriptions-item>
        <el-descriptions-item label="销售员">
          {{ PurchaseContractDialogData.salesperson }}
        </el-descriptions-item>
        <el-descriptions-item label="采购员">
          {{ PurchaseContractDialogData.purchaser }}
        </el-descriptions-item>
        <el-descriptions-item label="交货地点">
          {{ PurchaseContractDialogData.deliveryLocation }}
        </el-descriptions-item>
        <el-descriptions-item label="预付款金额" v-if="false">
          {{ PurchaseContractDialogData.deposit }}
        </el-descriptions-item>
        <el-descriptions-item label="有无预付款" v-if="false">
          {{ PurchaseContractDialogData.hasDeposit ? '是' : '否' }}
        </el-descriptions-item>
      </el-descriptions>

      <el-tabs v-model="PurchaseContractDialogData.activeName" class="demo-tabs">
        <el-tab-pane label="产品资料" name="productinfo">
          <el-table :data="PurchaseContractDialogData.productinfotableData">
            <el-table-column prop="productCode" label="产品编号" width="120"></el-table-column>
            <el-table-column prop="supplier" label="供应商" width="200"></el-table-column>
            <el-table-column prop="hasDeposit" label="有无预付款" width="120">
              <template #default="scope">
                <span>{{ scope.row.hasDeposit ? '有' : '无' }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="depositAmount" label="预付款金额" width="120">
              <template #default="scope">
                <span>{{ scope.row.hasDeposit ? scope.row.depositAmount : '无' }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="customerCode" label="客户货号" width="120"></el-table-column>
            <el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
            <el-table-column prop="englishName" label="英文品名" width="150" v-if="false"></el-table-column>
            <el-table-column prop="chineseSpecification" label="中文规格" width="150"></el-table-column>
            <el-table-column prop="unit" label="计量单位" width="90"></el-table-column>
            <el-table-column prop="contractQuantity" label="合同数量" width="120"></el-table-column>
            <el-table-column prop="purchaseUnitPrice" label="采购单价" width="120"></el-table-column>
            <el-table-column prop="purchasePriceTerms" label="采购价格条款" width="180"></el-table-column>
            <el-table-column prop="purchaseTotalPrice" label="采购总价" width="120"></el-table-column>
            <el-table-column prop="deliveryDate" label="交货日期" width="120">
              <template #default="scope">
                {{ formatDate(scope.row.deliveryDate) }}
              </template>
            </el-table-column>
            <el-table-column prop="productionLeadTime" label="生产交期" width="120">
              <template #default="scope">
                {{ formatDate(scope.row.productionLeadTime) }}
              </template>
            </el-table-column>
            <el-table-column prop="packaging" label="包装方式" width="120"></el-table-column>
            <el-table-column prop="specialRequirements" label="特殊要求" width="120"></el-table-column>
            <el-table-column prop="invoice" label="是否开票" width="90"></el-table-column>
            <el-table-column prop="innerBoxQuantity" label="内盒装量" width="120"></el-table-column>
            <el-table-column prop="outerBoxQuantity" label="外箱装量" width="120"></el-table-column>
            <el-table-column prop="remark" label="备注" width="150"></el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="厂家相关费用" name="relatedcosts">
          <el-table :data="PurchaseContractDialogData.CustomerRelaterExoensesTableData" style="width: 100%; ">
            <el-table-column prop="expenseName" label="费用名称" width="150"></el-table-column>
            <el-table-column prop="currency" label="币种" width="150"></el-table-column>
            <el-table-column prop="exchangeRate" label="汇率" width="150"></el-table-column>
            <el-table-column prop="expense" label="费用" width="150"></el-table-column>
            <el-table-column prop="amount" label="金额" width="150"></el-table-column>
            <el-table-column prop="remark" label="备注" width="150"></el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>

      <div style="margin-top: 30px;"></div>
      <span style="font-size: 20px; font-weight: bold;">合计信息</span>
      <el-divider></el-divider>
      <el-descriptions :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="采购费用合计">
          {{ PurchaseContractDialogData.totalValue }}
        </el-descriptions-item>
        <el-descriptions-item label="数量合计">
          {{ PurchaseContractDialogData.totalQuantity }}
        </el-descriptions-item>
        <el-descriptions-item label="箱数合计">
          {{ PurchaseContractDialogData.totalBoxCount }}
        </el-descriptions-item>
        <el-descriptions-item label="毛重合计">
          {{ PurchaseContractDialogData.totalGrossWeight }}
        </el-descriptions-item>
        <el-descriptions-item label="净重合计">
          {{ PurchaseContractDialogData.totalNetWeight }}
        </el-descriptions-item>
        <el-descriptions-item label="体积合计">
          {{ PurchaseContractDialogData.totalVolume }}
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

        <!-- <el-descriptions-item label="收款单位编号">
          {{ PaymentrequestForm.payeeCode }}
        </el-descriptions-item> -->

        <el-descriptions-item label="收款单位">
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
        <!-- <el-descriptions-item label="经手人">
          {{ PaymentrequestForm.handler }}
        </el-descriptions-item> -->
      </el-descriptions>

      <!-- 关联合同字段 - 仅在业务费用且款项名称为其它时显示 -->
      <el-descriptions v-if="shouldShowRelatedContract" :column="3" :border="true" label-width="120px">
        <el-descriptions-item label="关联合同">
          {{ PaymentrequestForm.relatedContract }}
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
            <el-table-column prop="expenseName" label="费用名称" />
            <el-table-column prop="relatedDocumentTypeName" label="单据类型" />
            <el-table-column prop="relatedDocumentsNo" label="单据号" />
            <el-table-column prop="amount" label="金额" />
            <el-table-column prop="appliedAmount" label="已申请金额" />
            <el-table-column prop="amountAlreadyApplied" label="本次申请金额" />
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
            <el-table-column prop="depositpaid" label="已付预付款" />
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

        <el-descriptions-item label="有无预付款">
          <el-checkbox v-model="ShippingDeliveryForm.isDeposit" disabled />
        </el-descriptions-item>
      </el-descriptions>

      <!-- 辅助信息 -->
      <br><span style="font-size: 20px; font-weight: bold;">辅助信息</span>
      <el-divider></el-divider>

      <el-descriptions :column="3" border>
        <el-descriptions-item label="快递公司">
          {{ ShippingDeliveryForm.courierCompaniesID }}
        </el-descriptions-item>
        <el-descriptions-item label="物流公司">
          {{ ShippingDeliveryForm.logisticsCompanyID }}
        </el-descriptions-item>
        <el-descriptions-item label="船代公司">
          {{ ShippingDeliveryForm.shippingAgent }}
        </el-descriptions-item>
      </el-descriptions>

      <!-- 销售合同 -->
      <br><span style="font-size: 20px; font-weight: bold;">销售合同</span>
      <el-divider></el-divider>
      <el-table :data="shippingDeliveryContrctProductTableData"
        style="width: 100%; margin-bottom: 15px; table-layout: fixed;"
        :header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }">
        <el-table-column prop="contractNumber" label="销售合同" width="150"></el-table-column>
        <el-table-column prop="productCode" label="产品编号" width="150"></el-table-column>
        <el-table-column prop="customerCode" label="客户货号" width="180"></el-table-column>
        <el-table-column prop="chineseName" label="中文品名" width="150"></el-table-column>
        <el-table-column prop="chineseSpec" label="中文规格" width="150"></el-table-column>
        <el-table-column prop="contractQuantity" label="合同数量" width="150"></el-table-column>
        <el-table-column prop="shipmentQuantity" label="出货数量" width="150"></el-table-column>
        <el-table-column prop="unit" label="计量单位" width="150"></el-table-column>
        <el-table-column prop="exportUnitPrice" label="外销单价" width="150"></el-table-column>
        <el-table-column prop="exportTotalPrice" label="外销总价" width="150"></el-table-column>
        <el-table-column prop="specialRequirements" label="特殊要求" width="150"></el-table-column>
        <el-table-column prop="outerBoxQuantity" label="外箱装量" width="150"></el-table-column>
        <el-table-column prop="innerBoxQuantity" label="中包装量(内盒装量)" width="150"></el-table-column>
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
    <el-dialog v-model="claimDialogVisible" title="领取收款单" width="70%" :close-on-click-modal="false">
      <el-form ref="claimFormRef" :model="claimForm" :rules="claimRules" label-width="100px">
        <span style="font-size: 20px; font-weight: bold;">基本信息</span>
        <el-divider></el-divider>
        <el-form :model="addcustomercollectionform" label-width="120px">
          <el-row>
            <el-col :span="8">
              <el-form-item label="收款单号">
                <el-input v-model="addcustomercollectionform.receiptNumber" style="width: 300px" disabled></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="收汇日期">
                <el-date-picker v-model="addcustomercollectionform.receiptDate" type="date" disabled
                  style="width: 300px"></el-date-picker>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="我方公司">
                <el-select v-model="addcustomercollectionform.ourCompany" style="width: 300px" disabled clearable>
                  <el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="8">
              <el-form-item label="外销币种">
                <el-select v-model="addcustomercollectionform.foreignCurrency" style="width: 300px" disabled clearable>
                  <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="汇率">
                <el-input :value="formatExchangeRate(addcustomercollectionform.exchangeRate)" style="width: 300px"
                  disabled></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="收汇金额">
                <el-input
                  :value="formatAmountWithCurrency(addcustomercollectionform.amount, addcustomercollectionform.foreignCurrency)"
                  style="width: 300px" disabled></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="8">
              <el-form-item label="结汇金额">
                <el-input :value="formatAmountWithCurrency(addcustomercollectionform.ExchangeSettlementAmount, '3')"
                  style="width: 300px" disabled></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="收汇银行">
                <el-select v-model="addcustomercollectionform.bank" style="width: 300px" disabled clearable>
                  <el-option v-for="dict in receivingBankOptions" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="客户">
                <el-select v-model="addcustomercollectionform.Customer" filterable clearable placeholder="请选择客户"
                  style="width: 300px" @change="handleCustomerChange">
                  <el-option v-for="dict in optionss.sql_user_customers" :key="dict.dictCode" :label="dict.dictLabel"
                    :value="dict.dictValue"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <span style="font-size: 20px; font-weight: bold;">收款明细</span>
            <el-divider></el-divider>
            <el-button class="mt-4" type="primary" @click="handleAddRowReceivingPaymentsDetails"
              :disabled="isAddButtonDisabled" :title="isAddButtonDisabled ? '收汇金额已全部分配完毕' : ''"
              style="margin-bottom: 10px;">
              添加收款明细
            </el-button>
            <el-table :data="ReceivingPaymentsDetailsTbaleData" style="width: 100%" show-summary
              :summary-method="getSummaries">
              <el-table-column prop="fundsClassification" label="款项类别">
                <template #default="{ row }">
                  <el-select v-model="row.fundsClassification" filterable clearable placeholder="请选择款项类别"
                    @change="() => handleFundsClassificationChange(row)">
                    <el-option v-for="dict in optionss.hr_funds_classification" :key="dict.dictCode"
                      :label="dict.dictLabel" :value="dict.dictValue"></el-option>
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="associatedModulesDocumentID" label="关联单号">
                <template #default="{ row }">
                  <el-select v-model="row.associatedModulesDocumentID" filterable clearable placeholder="请选择关联单号"
                    @visible-change="(visible) => handleAssociatedDocumentVisibleChange(visible, row)">
                    <el-option v-for="dict in getAssociatedDocumentOptions(row)" :key="dict.dictValue"
                      :label="dict.dictLabel" :value="dict.dictValue"></el-option>
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="applicationamount" label="金额">
                <template #default="{ row }">
                  <el-input v-model="row.applicationamount" placeholder="输入金额" size="large" @input="handleAmountInput"
                    :class="{ 'amount-exceeded': isAmountExceeded }"
                    :title="isAmountExceeded ? '收款明细总金额已超过收汇金额' : ''"></el-input>
                </template>
              </el-table-column>
              <el-table-column fixed="right" label="操作" width="100">
                <template #default="scope">
                  <el-button type="text" size="large" @click="handleDeleteRow(scope.$index)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
            <!-- 合计行 -->
            <div v-if="ReceivingPaymentsDetailsTbaleData.length > 0"
              style="margin-top: 10px; text-align: right; padding: 10px; background-color: #f5f7fa; border-radius: 4px;">
              <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                  <span style="font-weight: bold; font-size: 14px;">合计金额：</span>
                  <span :style="{
                    fontWeight: 'bold',
                    fontSize: '16px',
                    color: isAmountExceeded ? '#f56c6c' : isAmountEqual ? '#67c23a' : '#409eff'
                  }">
                    {{ formatAmountWithCurrency(totalAmount, addcustomercollectionform.foreignCurrency) }}
                  </span>
                  <div v-if="isAmountExceeded" style="margin-top: 5px;">
                    <el-tag type="danger" size="small">
                      <el-icon>
                        <Warning />
                      </el-icon>
                      收款明细总金额超过收汇金额
                    </el-tag>
                  </div>
                  <div v-else-if="!isAmountEqual && totalAmount > 0" style="margin-top: 5px;">
                    <el-tag type="warning" size="small">
                      <el-icon>
                        <Warning />
                      </el-icon>
                      收款明细总金额必须等于收汇金额
                    </el-tag>
                  </div>
                  <div v-else-if="isAmountEqual" style="margin-top: 5px;">
                    <el-tag type="success" size="small">
                      <el-icon>
                        <CircleCheck />
                      </el-icon>
                      金额匹配，可以提交
                    </el-tag>
                  </div>
                </div>
                <div style="text-align: right;">
                  <div style="font-size: 12px; color: #909399;">收汇金额：</div>
                  <div style="font-size: 14px; color: #606266;">
                    {{ formatAmountWithCurrency(addcustomercollectionform.amount,
                      addcustomercollectionform.foreignCurrency)
                    }}
                  </div>
                </div>
              </div>
            </div>
          </el-row>

          <!-- 收款单据图片和附件显示 -->
          <el-row v-if="addcustomercollectionform.receiptImageUrl || addcustomercollectionform.attachmentUrl">
            <el-col :span="24">
              <span style="font-size: 20px; font-weight: bold;">收款单据</span>
              <el-divider></el-divider>

              <!-- 收款单据图片显示 -->
              <div v-if="addcustomercollectionform.receiptImageUrl" style="margin-bottom: 20px;">
                <h4 style="margin-bottom: 10px; color: #606266;">收款单据图片</h4>
                <div class="image-gallery">
                  <div v-for="(imageUrl, index) in getImageUrls(addcustomercollectionform.receiptImageUrl)" :key="index"
                    class="image-item" @click="previewImage(imageUrl)">
                    <img :src="imageUrl" :alt="`收款单据图片 ${index + 1}`" class="receipt-image" />
                    <div class="image-overlay">
                      <el-icon>
                        <ZoomIn />
                      </el-icon>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 附件显示 -->
              <div v-if="addcustomercollectionform.attachmentUrl">
                <h4 style="margin-bottom: 10px; color: #606266;">相关附件</h4>
                <div class="attachment-list">
                  <div v-for="(attachmentUrl, index) in getAttachmentUrls(addcustomercollectionform.attachmentUrl)"
                    :key="index" class="attachment-item">
                    <div class="attachment-info">
                      <el-icon class="attachment-icon">
                        <Document />
                      </el-icon>
                      <span class="attachment-name">{{ getFileName(attachmentUrl) }}</span>
                    </div>
                    <div class="attachment-actions">
                      <el-button type="text" size="small" @click="previewAttachment(attachmentUrl)">
                        <el-icon>
                          <View />
                        </el-icon>
                        预览
                      </el-button>
                    </div>
                  </div>
                </div>
              </div>
            </el-col>
          </el-row>
        </el-form>
      </el-form>

      <!-- 图片预览对话框 -->
      <el-dialog v-model="imagePreviewVisible" title="图片预览" width="80%" center>
        <div style="text-align: center;">
          <img :src="previewImageUrl" style="max-width: 100%; max-height: 70vh;" alt="预览图片" />
        </div>
      </el-dialog>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="claimDialogVisible = false">取 消</el-button>
          <el-button type="primary" @click="submitClaim" :disabled="isSubmitDisabled"
            :title="isSubmitDisabled ? '收款明细总金额必须等于收汇金额才能确定' : ''">
            确 定
          </el-button>
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
        <el-table-column prop="id" label="合同id" width="100" v-if="false"></el-table-column>
        <el-table-column prop="contractNumber" label="合同号" width="150">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleContractNumberClick(row)">{{ row.contractNumber }}</el-button>
          </template>
        </el-table-column>
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
        <el-table-column prop="totalAmount" label="出运单金额" width="150">
          <template #default="{ row }">
            {{ formatAmountWithCurrency(row.totalAmount, '3') }}
          </template>
        </el-table-column>
        <el-table-column prop="totalReceivedAmount" label="已收金额" width="100">
          <template #default="{ row }">
            {{ formatAmountWithCurrency(row.totalReceivedAmount, '3') }}
          </template>
        </el-table-column>
        <el-table-column prop="overdueDays" label="逾期天数" width="100" />
        <el-table-column fixed="right" label="操作" style="width: 25%; ">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleModifyPaymentDate(row)">申请修改收款日期</el-button>
          </template>
        </el-table-column>
        <!-- 其他需要显示的列 -->
      </el-table>
    </el-dialog>
    <el-dialog v-model="pendingEmailDialogVisible" title="待处理工作任务" :close-on-click-modal="false" class="custom-dialog"
      width="900px">
      <el-tabs type="border-card" class="demo-tabs">
        <el-tab-pane>
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <calendar />
              </el-icon>
              <span>计划任务</span>
            </span>
          </template>
          <el-table :data="pendingTaskPlanItemList" :height="400" style="width: 100%" @row-dblclick="handleRowDblClick">
            <el-table-column prop="taskId" label="任务ID" width="200" v-if="false"></el-table-column>
            <el-table-column prop="taskName" label="任务名称" width="200"></el-table-column>
            <el-table-column prop="phaseName" label="阶段名称" width="200"></el-table-column>
            <el-table-column prop="itemId" label="事项ID" width="200" v-if="false"></el-table-column>
            <el-table-column prop="itemName" label="事项名称" width="200"></el-table-column>
            <el-table-column prop="timePoint" label="截止时间" width="200">
              <template #default="scope">
                {{ scope.row.timePoint }}
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane>
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <message />
              </el-icon>
              <span>邮件</span>
            </span>
          </template>
          <el-table :data="pendingEmailList" :height="400" style="width: 100%">
            <el-table-column prop="customerName" label="客户名称" width="200"></el-table-column>
            <el-table-column prop="emailsubject" label="邮件标题" width="200"></el-table-column>
            <el-table-column prop="create_time" label="时间" width="250">
              <template #default="scope">
                {{ scope.row.createTime }}
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

      </el-tabs>
    </el-dialog>
    <el-dialog v-model="overdueEmailDialogVisible" title="超时未处理" :close-on-click-modal="false" class="custom-dialog"
      width="900px">
      <el-tabs type="border-card" class="demo-tabs">
        <el-tab-pane>
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <calendar />
              </el-icon>
              <span>计划任务</span>
            </span>
          </template>
          <el-table :data="overduePendingTaskPlanItemList" :height="400" style="width: 100%"
            @row-dblclick="handleRowDblClick">
            <el-table-column prop="taskId" label="任务ID" width="200" v-if="false"></el-table-column>
            <el-table-column prop="taskName" label="任务名称" width="200"></el-table-column>
            <el-table-column prop="phaseName" label="阶段名称" width="200"></el-table-column>
            <el-table-column prop="itemId" label="事项ID" width="200" v-if="false"></el-table-column>
            <el-table-column prop="itemName" label="事项名称" width="200"></el-table-column>
            <el-table-column prop="timePoint" label="截止时间" width="200">
              <template #default="scope">
                {{ scope.row.timePoint }}
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane>
          <template #label>
            <span class="custom-tabs-label">
              <el-icon>
                <calendar />
              </el-icon>
              <span>邮件</span>
            </span>
          </template>
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
        </el-tab-pane>

      </el-tabs>

    </el-dialog>

    <!-- 汇率填写通知对话框 -->
    <el-dialog v-model="exchangeRateNotificationVisible" title="汇率填写提醒" width="600px" :close-on-click-modal="false"
      :close-on-press-escape="false">
      <div class="exchange-rate-notification">
        <div class="notification-header">
          <el-icon class="notification-icon" size="24">
            <Warning />
          </el-icon>
          <span class="notification-title">请填写今日汇率</span>
        </div>
        <div class="notification-content">
          <p>您需要填写今日所有币种的汇率信息，请为每个币种输入对应的汇率值。</p>
        </div>

        <!-- 多币种汇率填写表单 -->
        <el-form ref="exchangeRateFormRef" :model="multiCurrencyExchangeRateForm" label-width="120px">
          <!-- 调试信息 -->

          <div class="currency-rates-container">
            <div v-if="multiCurrencyExchangeRateForm.exchangeRates.length === 0"
              style="text-align: center; padding: 20px; color: #999;">
              暂无币种数据，请检查字典配置
            </div>
            <div v-for="(rate, index) in multiCurrencyExchangeRateForm.exchangeRates" :key="rate.currency"
              class="currency-rate-item">
              <el-row :gutter="15">
                <el-col :span="11">
                  <el-form-item :label="`${rate.currency}汇率`" :prop="`exchangeRates.${index}.exchangeRate`" :rules="[
                    { required: true, message: '请输入汇率', trigger: 'blur' },
                    { pattern: /^\d+(\.\d+)?$/, message: '请输入有效的数字', trigger: 'blur' }
                  ]">
                    <el-input v-model="rate.exchangeRate" placeholder="请输入汇率" style="width: 100%" />
                  </el-form-item>
                </el-col>
                <el-col :span="13">
                  <el-form-item label="备注" :prop="`exchangeRates.${index}.remark`">
                    <el-input v-model="rate.remark" placeholder="请输入备注（选填）" style="width: 100%" />
                  </el-form-item>
                </el-col>
              </el-row>
            </div>
          </div>

          <el-row>
            <el-col :span="24">
              <el-form-item label="全局备注">
                <el-input v-model="multiCurrencyExchangeRateForm.globalRemark" type="textarea" :rows="2"
                  placeholder="请输入全局备注（选填）" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="skipExchangeRateNotification">稍后提醒</el-button>
          <el-button type="primary" @click="showExchangeRateConfirmDialog">提交所有汇率</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 汇率确认对话框 -->
    <el-dialog v-model="exchangeRateConfirmVisible" title="汇率确认" width="700px" :close-on-click-modal="false"
      :close-on-press-escape="false">
      <div class="exchange-rate-confirm">
        <div class="confirm-header">
          <el-icon class="confirm-icon" size="24" color="#409EFF">
            <CircleCheck />
          </el-icon>
          <span class="confirm-title">请确认以下汇率信息</span>
        </div>
        <div class="confirm-content">
          <p>请仔细核对以下汇率信息，确认无误后点击"确认提交"。</p>

          <!-- 汇率确认表格 -->
          <el-table :data="confirmExchangeRates" border style="width: 100%; margin-top: 20px;">
            <el-table-column prop="currency" label="币种" width="120" align="center" />
            <el-table-column prop="exchangeRate" label="汇率" width="150" align="center">
              <template #default="scope">
                <span class="exchange-rate-value">{{ scope.row.exchangeRate }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="remark" label="备注" min-width="200">
              <template #default="scope">
                <span v-if="scope.row.remark">{{ scope.row.remark }}</span>
                <span v-else class="text-muted">无</span>
              </template>
            </el-table-column>
          </el-table>

          <!-- 全局备注 -->
          <div v-if="confirmGlobalRemark" class="global-remark-section">
            <h4>全局备注：</h4>
            <p class="global-remark-text">{{ confirmGlobalRemark }}</p>
          </div>
        </div>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="exchangeRateConfirmVisible = false">返回修改</el-button>
          <el-button type="primary" @click="confirmSubmitMultiCurrencyExchangeRate">确认提交</el-button>
        </span>
      </template>
    </el-dialog>
  </div>

</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref, onMounted, onUnmounted, h, watch, computed, nextTick } from 'vue'
import { ElMessage, ElMessageBox, ElDatePicker, ElLoading, ElNotification } from "element-plus";
import request from '@/utils/request';
import dayjs from 'dayjs';
import useUserStore from '@/store/modules/user'
import useSocketStore from '@/store/modules/socket'
// 时间插件
import duration from 'dayjs/plugin/duration'
import { Picture, Warning, CircleCheck, Clock, Message, Calendar, List, ArrowLeft, ArrowRight, Money } from '@element-plus/icons-vue'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import { useRouter } from 'vue-router'
import { eventBus } from '@/utils/eventBus'
import { useRoute } from 'vue-router'
import exchangeRateService from '@/utils/exchangeRateService'


const totalAmount = ref(0);

// 新增的响应式数据
const tasksActiveTab = ref('reminders');
const currentMonthYear = ref('');
const weekdays = ['日', '一', '二', '三', '四', '五', '六'];
const calendarDays = ref([]);

// 汇率填写通知相关变量
const exchangeRateNotificationVisible = ref(false);
const exchangeRateFormRef = ref();
const exchangeRateNotificationTimer = ref(null);
const exchangeRateCheckTimer = ref(null);

// 汇率确认对话框相关变量
const exchangeRateConfirmVisible = ref(false);
const confirmExchangeRates = ref([]);
const confirmGlobalRemark = ref('');

// 当日汇率显示相关变量
const todayExchangeRates = ref([]);

// 汇率填写表单
interface ExchangeRateForm {
  currency: string;
  exchangeRate: number;
  remark: string;
}

// 多币种汇率填写表单
interface MultiCurrencyExchangeRateForm {
  exchangeRates: Array<{
    currency: string;
    exchangeRate: string;
    remark: string;
  }>;
  globalRemark: string;
}

const exchangeRateForm = reactive<ExchangeRateForm>({
  currency: '',
  exchangeRate: null,
  remark: ''
});

const multiCurrencyExchangeRateForm = reactive<MultiCurrencyExchangeRateForm>({
  exchangeRates: [],
  globalRemark: ''
});

// 汇率填写表单验证规则
const exchangeRateRules = reactive({
  currency: [
    { required: true, message: '请选择币种', trigger: 'change' }
  ],
  exchangeRate: [
    { required: true, message: '请输入汇率', trigger: 'blur' }
  ]
});

// 确保calendarDays有初始值
// 移除重复的onMounted，保留原有的

const handleAddRowReceivingPaymentsDetails = () => {
  // 检查是否还有剩余金额可以分配
  const receiptAmount = parseFloat(addcustomercollectionform.value.amount) || 0;
  const remainingAmount = receiptAmount - totalAmount.value;

  if (remainingAmount <= 0) {
    // 静默处理，不显示提示消息
    return;
  }

  ReceivingPaymentsDetailsTbaleData.value.push({
    fundsClassification: '',
    associatedModules: '',
    associatedModulesDocumentID: '',
    applicationamount: ''
  });
};
const ReceivingPaymentsDetailsTbaleData = ref([])
//收款单据表单
const addcustomercollectionform = ref({
  receiptNumber: '',
  receiptDate: '',
  ourCompany: '',
  foreignCurrency: '',
  exchangeRate: '',
  amount: '',
  bank: '',
  attachment: '',
  receiptImageUrl: '',
  attachmentUrl: '',
  Customer: '',
  FundsClassification: '',
  AssociatedModules: '',
  AssociatedModulesDocumentID: '',
  ExchangeSettlementAmount: ''
})

// 计算总金额
const calculateTotal = () => {
  // 使用更精确的计算方法，避免浮点数精度问题
  totalAmount.value = ReceivingPaymentsDetailsTbaleData.value.reduce((sum, row) => {
    const amount = parseFloat(row.applicationamount) || 0;
    // 将每个金额转换为分进行计算，避免浮点数精度问题
    return sum + Math.round(amount * 100);
  }, 0) / 100;
};

// 验证收款明细总金额是否完全等于收汇金额
const validateTotalAmount = () => {
  // 使用更精确的解析方法，避免浮点数精度问题
  const receiptAmount = Math.round(parseFloat(addcustomercollectionform.value.amount) * 100) / 100 || 0;
  const currentTotal = totalAmount.value;

  if (currentTotal > receiptAmount) {
    return false;
  }

  if (currentTotal !== receiptAmount) {
    return false;
  }

  return true;
};

// 获取验证错误消息
const getValidationMessage = () => {
  // 使用更精确的解析方法，避免浮点数精度问题
  const receiptAmount = Math.round(parseFloat(addcustomercollectionform.value.amount) * 100) / 100 || 0;
  const currentTotal = totalAmount.value;

  if (currentTotal > receiptAmount) {
    return `收款明细总金额 (${formatAmountWithCurrency(currentTotal, addcustomercollectionform.value.foreignCurrency)}) 不能超过收汇金额 (${formatAmountWithCurrency(receiptAmount, addcustomercollectionform.value.foreignCurrency)})`;
  }

  if (currentTotal !== receiptAmount) {
    return `收款明细总金额 (${formatAmountWithCurrency(currentTotal, addcustomercollectionform.value.foreignCurrency)}) 必须完全等于收汇金额 (${formatAmountWithCurrency(receiptAmount, addcustomercollectionform.value.foreignCurrency)})`;
  }

  return '';
};

// 检查是否超过收汇金额
const isAmountExceeded = computed(() => {
  // 使用更精确的解析方法，避免浮点数精度问题
  const receiptAmount = Math.round(parseFloat(addcustomercollectionform.value.amount) * 100) / 100 || 0;
  // 必须分毫不差，不允许任何误差
  return totalAmount.value > receiptAmount;
});

// 检查金额是否完全相等
const isAmountEqual = computed(() => {
  // 使用更精确的解析方法，避免浮点数精度问题
  const receiptAmount = Math.round(parseFloat(addcustomercollectionform.value.amount) * 100) / 100 || 0;
  // 必须分毫不差，不允许任何误差
  return totalAmount.value === receiptAmount;
});

// 检查确定按钮是否应该禁用
const isSubmitDisabled = computed(() => {
  return !isAmountEqual.value || isAmountExceeded.value;
});

// 检查添加按钮是否应该禁用
const isAddButtonDisabled = computed(() => {
  // 使用更精确的解析方法，避免浮点数精度问题
  const receiptAmount = Math.round(parseFloat(addcustomercollectionform.value.amount) * 100) / 100 || 0;
  const remainingAmount = receiptAmount - totalAmount.value;
  return remainingAmount <= 0;
});

// 处理金额输入
const handleAmountInput = () => {
  calculateTotal();
  // 移除实时验证提示，只在视觉上显示状态
};

// 获取货币符号
const getCurrencySymbol = (currencyValue) => {
  if (!currencyValue) return '';
  if (currencyValue === '1') return 'USD';
  if (currencyValue === '2') return 'EUR';
  if (currencyValue === '3') return 'CNY';
  return '';
};

// 格式化货币显示
const formatCurrency = (amount) => {
  return new Intl.NumberFormat('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount);
};

// 格式化带货币符号的金额显示
const formatAmountWithCurrency = (amount, currencyValue) => {
  if (!amount) return '';
  const symbol = getCurrencySymbol(currencyValue);
  const formattedAmount = new Intl.NumberFormat('zh-CN', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount);
  return symbol ? `${symbol} ${formattedAmount}` : formattedAmount;
};

// 格式化货值合计显示
const formatTotalValueOfGoods = (value, currencyValue) => {
  if (!value || value === 0) return '0.00';
  const symbol = getCurrencySymbol(currencyValue);
  const formattedValue = Number(value).toFixed(2);
  return symbol ? `${symbol} ${formattedValue}` : formattedValue;
};

// 格式化人民币金额显示
const formatCNYAmount = (value) => {
  if (!value || value === 0) return 'CNY 0.00';
  const formattedValue = Number(value).toFixed(2);
  return `CNY ${formattedValue}`;
};

// 格式化百分比显示
const formatPercentage = (value) => {
  if (!value || value === 0) return '0.00%';
  const formattedValue = Number(value).toFixed(2);
  return `${formattedValue}%`;
};

// 格式化汇率显示（保留3位小数）
const formatExchangeRate = (rate) => {
  if (!rate && rate !== 0) return '';
  return new Intl.NumberFormat('zh-CN', {
    minimumFractionDigits: 3,
    maximumFractionDigits: 3
  }).format(rate);
};

// 表格合计行方法
const getSummaries = (param) => {
  const { columns, data } = param;
  const sums = [];
  columns.forEach((column, index) => {
    if (index === 0) {
      sums[index] = '合计';
      return;
    }
    if (index === 3) { // 金额列
      const values = data.map(item => Number(item.applicationamount) || 0);
      if (!values.every(value => isNaN(value))) {
        const total = values.reduce((prev, curr) => {
          const value = Number(curr);
          if (!isNaN(value)) {
            return prev + curr;
          } else {
            return prev;
          }
        }, 0);
        sums[index] = formatAmountWithCurrency(total, addcustomercollectionform.value.foreignCurrency);
      } else {
        sums[index] = formatAmountWithCurrency(0, addcustomercollectionform.value.foreignCurrency);
      }
    } else {
      sums[index] = '';
    }
  });
  return sums;
};

// 删除收款明细行
const handleDeleteRow = (index) => {
  ReceivingPaymentsDetailsTbaleData.value.splice(index, 1);
  calculateTotal(); // 重新计算总金额
};

const handleAssociatedModulesChange = (row) => {
  // 当关联模块变化时，可以在这里添加其他逻辑
  // 现在不再需要清空关联单号，因为所有数据都来自同一个接口
};

// 存储关联单号选项数据缓存，key格式：customerID_paymentCategory
const associatedDocumentOptionsCache = ref({} as any);

// 获取关联单号选项数据（针对单行）
const getAssociatedDocumentOptionsData = async (paymentCategory, customerID) => {
  // 如果没有款项类别或客户ID，返回空数组
  if (!paymentCategory || !customerID) {
    return [];
  }

  // 生成缓存key
  const cacheKey = `${customerID}_${paymentCategory}`;

  // 如果缓存中有数据，直接返回
  if (associatedDocumentOptionsCache.value[cacheKey]) {
    return associatedDocumentOptionsCache.value[cacheKey];
  }

  try {
    const params = {} as any;
    params.PaymentCategories = paymentCategory; // 单个值，不是数组
    params.CustomerID = customerID;

    const response = await request({
      url: 'Contracts/GetSalesNumberAndShippingDeliverNumberSelectList/GetSalesNumberAndShippingDeliverNumberSelectList',
      method: 'GET',
      params: params
    });

    if (response.code === 200 && response.data) {
      // 缓存结果
      associatedDocumentOptionsCache.value[cacheKey] = response.data;
      return response.data;
    } else {
      console.error('获取关联单号选项失败:', response.msg);
      return [];
    }
  } catch (error) {
    console.error('获取关联单号选项失败:', error);
    return [];
  }
};

// 根据行数据获取关联单号选项
const getAssociatedDocumentOptions = (row) => {
  if (!row) {
    return [];
  }

  const paymentCategory = row.fundsClassification;
  const customerID = addcustomercollectionform.value?.Customer;

  if (!paymentCategory || !customerID) {
    return [];
  }

  const cacheKey = `${customerID}_${paymentCategory}`;
  return associatedDocumentOptionsCache.value[cacheKey] || [];
};

// 处理客户下拉框改变事件
const handleCustomerChange = async () => {
  // 清空缓存，因为客户改变了，所有行的关联单号选项都需要重新获取
  associatedDocumentOptionsCache.value = {};

  // 为所有已有款项类别的行重新获取关联单号选项
  const customerID = addcustomercollectionform.value?.Customer;
  if (customerID && ReceivingPaymentsDetailsTbaleData.value) {
    for (const row of ReceivingPaymentsDetailsTbaleData.value) {
      if (row.fundsClassification) {
        await getAssociatedDocumentOptionsData(row.fundsClassification, customerID);
      }
    }
  }
};

// 处理款项类别下拉框改变事件
const handleFundsClassificationChange = async (row) => {
  // 清空该行的关联单号选择
  if (row) {
    row.associatedModulesDocumentID = '';
  }

  // 获取该行的关联单号选项
  const customerID = addcustomercollectionform.value?.Customer;
  if (row && row.fundsClassification && customerID) {
    await getAssociatedDocumentOptionsData(row.fundsClassification, customerID);
  }
};

// 处理关联单号下拉框显示/隐藏事件
const handleAssociatedDocumentVisibleChange = async (visible, row) => {
  // 当下拉框打开时，如果还没有数据，则获取
  if (visible && row) {
    const paymentCategory = row.fundsClassification;
    const customerID = addcustomercollectionform.value?.Customer;

    if (paymentCategory && customerID) {
      const cacheKey = `${customerID}_${paymentCategory}`;
      // 如果缓存中没有数据，则获取
      if (!associatedDocumentOptionsCache.value[cacheKey]) {
        await getAssociatedDocumentOptionsData(paymentCategory, customerID);
      }
    }
  }
};

// 日历相关方法
const previousMonth = () => {
  let currentDate;
  if (currentMonthYear.value) {
    const match = currentMonthYear.value.match(/(\d{4})年(\d{1,2})月/);
    if (match) {
      const year = parseInt(match[1]);
      const month = parseInt(match[2]) - 1;
      currentDate = new Date(year, month, 1);
    } else {
      currentDate = new Date();
    }
  } else {
    currentDate = new Date();
  }

  currentDate.setMonth(currentDate.getMonth() - 1);
  currentMonthYear.value = currentDate.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long' });
  generateCalendarDays();

  // 获取上个月的任务数据
  const startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
  const endDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
  getPlanTaskItems(formatDate(startDate), formatDate(endDate));
};

const nextMonth = () => {
  let currentDate;
  if (currentMonthYear.value) {
    const match = currentMonthYear.value.match(/(\d{4})年(\d{1,2})月/);
    if (match) {
      const year = parseInt(match[1]);
      const month = parseInt(match[2]) - 1;
      currentDate = new Date(year, month, 1);
    } else {
      currentDate = new Date();
    }
  } else {
    currentDate = new Date();
  }

  currentDate.setMonth(currentDate.getMonth() + 1);
  currentMonthYear.value = currentDate.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long' });
  generateCalendarDays();

  // 获取下个月的任务数据
  const startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
  const endDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
  getPlanTaskItems(formatDate(startDate), formatDate(endDate));
};

// 判断是否为采购角色的函数
const isPurchaseRole = () => {
  // 根据用户信息判断是否为采购角色
  // 可以通过部门ID、角色名称等方式判断
  if (userStore.userInfo && userStore.userInfo.deptId) {
    // 采购部门的ID为210
    const isPurchase = userStore.userInfo.deptId === 210;
    console.log('用户部门ID:', userStore.userInfo.deptId, '是否为采购角色:', isPurchase);
    return isPurchase;
  }
  // 也可以通过角色名称判断
  if (userStore.roles && userStore.roles.length > 0) {
    const isPurchase = userStore.roles.some(role =>
      role.includes('采购') ||
      role.includes('purchase') ||
      role.includes('PURCHASE')
    );
    console.log('用户角色:', userStore.roles, '是否为采购角色:', isPurchase);
    return isPurchase;
  }
  console.log('无法判断用户角色，默认显示商机看板');
  return false;
};

const isFinanceRole = () => {
  if (userStore.userInfo && userStore.userInfo.deptId) {
    return userStore.userInfo.deptId === 213;
  }
  if (userStore.roles && userStore.roles.length > 0) {
    return userStore.roles.some(role =>
      role.includes('财务') ||
      role.includes('finance') ||
      role.includes('FINANCE')
    );
  }
  return false;
};

const isAdminRole = () => {
  return Array.isArray(userStore.roles) && userStore.roles.includes('admin');
};

const salesDeptIds = new Set([206, 207, 208, 209])
const isSalesDeptUser = () => {
  const deptId = Number(userStore.userInfo?.deptId)
  return Number.isFinite(deptId) && salesDeptIds.has(deptId)
}

const isSalesRole = () => {
  if (userStore.roles && userStore.roles.length > 0) {
    return userStore.roles.some(role =>
      role.includes('销售') ||
      role.includes('业务') ||
      role.includes('sales') ||
      role.includes('SALE')
    );
  }
  // 非采购/非财务默认视为业务员
  return !isPurchaseRole() && !isFinanceRole();
};

// 获取商机当前阶段的辅助函数
const getCurrentStage = (item) => {
  // 遍历所有阶段，找到包含该商机的阶段
  for (const stage of businessStages.value) {
    if (stage.details.some(detail => detail.id === item.id)) {
      return stage.salesStage;
    }
  }
  return null;
};

const selectDate = (date) => {
  // 实现日期选择逻辑
  console.log('选择日期:', date);
};
const handleOpportunityClick = (item) => {
  // 实现商机双击跳转逻辑
  console.log('双击商机:', item);

  // 根据商机类型和阶段进行跳转
  if (item.sourceType === 'api') {
    // API数据来源的商机
    if (item.opportunityNumber && item.opportunityNumber !== '无编号') {
      // 根据不同的阶段跳转到对应的页面
      const currentStage = getCurrentStage(item);
      console.log('当前阶段:', currentStage);
      console.log('商机编号:', item.opportunityNumber);

      // 根据商机编号前缀判断类型
      if (item.opportunityNumber.startsWith('SC') || item.opportunityNumber.includes('合同')) {
        // 合同编号，跳转到销售合同页面
        console.log('识别为合同，跳转到销售合同页面');
        router.push({
          path: '/sale/sale/salecontract',
          query: {
            contractId: item.id,
            viewDetail: 'true'
          }
        });
      } else if (item.opportunityNumber.startsWith('Q') || item.opportunityNumber.includes('报价')) {
        // 报价单号，跳转到报价详情页面
        console.log('识别为报价单，跳转到报价详情页面');
        router.push({
          path: '/sale/customerquotation',
          query: {
            id: item.id,
            viewDetail: 'true'
          }
        });
      } else {
        // 根据阶段判断
        switch (currentStage) {
          case '初次报价':
          case '再次报价':
            // 跳转到报价详情页面
            console.log('根据阶段判断为报价，跳转到报价详情页面');
            router.push({
              path: '/sale/customerquotation',
              query: {
                id: item.id,
                viewDetail: 'true'
              }
            });
            break;
          case '合同确定':
            // 跳转到销售合同页面
            console.log('根据阶段判断为合同，跳转到销售合同页面');
            router.push({
              path: '/sale/salecontract',
              query: {
                contractNumber: item.opportunityNumber,
                contractId: item.id,
                viewDetail: 'true'
              }
            });
            break;
          default:
            // 其他阶段（询盘、沟通需求）跳转到客户资料页面
            console.log('其他阶段，跳转到客户资料页面');
            router.push({
              path: '/sale/customerinfomation',
              query: {
                customerId: item.customerId || item.id
              }
            });
            break;
        }
      }
    } else {
      ElMessage.warning('该商机暂无详细信息');
    }
  } else {
    // 非API数据来源的商机，跳转到客户资料页面
    router.push({
      path: '/sale/customerinfomation',
      query: {
        customerId: item.customerId || item.id
      }
    });
  }
};

// 生成日历数据
const generateCalendarDays = () => {
  const today = new Date();
  let currentDate;

  // 安全地解析当前月份年份
  if (currentMonthYear.value) {
    // 尝试解析中文日期格式 "2024年1月"
    const match = currentMonthYear.value.match(/(\d{4})年(\d{1,2})月/);
    if (match) {
      const year = parseInt(match[1]);
      const month = parseInt(match[2]) - 1; // 月份从0开始
      currentDate = new Date(year, month, 1);
    } else {
      currentDate = new Date();
    }
  } else {
    currentDate = new Date();
  }

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  // 获取当月第一天和最后一天
  const firstDay = new Date(year, month, 1);
  const lastDay = new Date(year, month + 1, 0);

  // 获取上个月的最后几天
  const firstDayOfWeek = firstDay.getDay();
  const prevMonthDays = [];
  for (let i = firstDayOfWeek - 1; i >= 0; i--) {
    const date = new Date(year, month, -i);
    prevMonthDays.push({
      date: date.toISOString().split('T')[0],
      day: date.getDate(),
      isCurrentMonth: false,
      isToday: date.toDateString() === today.toDateString(),
      taskCount: 0,
      hasOverdue: false
    });
  }

  // 获取当月的天数
  const currentMonthDays = [];
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const date = new Date(year, month, i);
    const dateStr = date.toISOString().split('T')[0];
    const tasks = getDateTasks(dateStr);
    currentMonthDays.push({
      date: dateStr,
      day: i,
      isCurrentMonth: true,
      isToday: date.toDateString() === today.toDateString(),
      taskCount: tasks.length,
      hasOverdue: tasks.some(task => isOverdue(task.timePoint))
    });
  }

  // 获取下个月的前几天
  const lastDayOfWeek = lastDay.getDay();
  const nextMonthDays = [];
  for (let i = 1; i <= 6 - lastDayOfWeek; i++) {
    const date = new Date(year, month + 1, i);
    nextMonthDays.push({
      date: date.toISOString().split('T')[0],
      day: date.getDate(),
      isCurrentMonth: false,
      isToday: date.toDateString() === today.toDateString(),
      taskCount: 0,
      hasOverdue: false
    });
  }

  calendarDays.value = [...prevMonthDays, ...currentMonthDays, ...nextMonthDays];
  console.log('生成的日历数据:', calendarDays.value);
};

// 初始化日历
const initCalendar = () => {
  const today = new Date();
  currentMonthYear.value = today.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long' });
  console.log('初始化日历，当前月份年份:', currentMonthYear.value);
  generateCalendarDays();
  console.log('生成的日历数据:', calendarDays.value);

  // 获取当前月份的任务数据
  const startDate = new Date(today.getFullYear(), today.getMonth(), 1);
  const endDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);
  getPlanTaskItems(formatDate(startDate), formatDate(endDate));
};


const AgencyProcessdialogTableActiveName = ref('first')
// 添加一个 Set 来记录已经显示过的消息 ID
const displayedMessages = new Set();

// 修改获取未读消息的函数
const getUnreadMessages = async () => {
  try {
    const response = await request({
      url: '/MessageRecord/GetList/GetMessageList',
      method: 'get'
    });

    if (response.code === 200 && response.data) {
      console.log('获取到的消息:', response.data);

      // 过滤未显示的消息
      const unreadMessages = response.data.filter(msg => !displayedMessages.has(msg.id));
      console.log('待显示的消息:', unreadMessages);

      let currentOffset = 0;
      const notifications = [];

      unreadMessages.forEach((msg) => {
        displayedMessages.add(msg.id);

        const notification = ElNotification({
          title: '系统通知',
          customClass: 'custom-notification',
          duration: 0,
          position: 'bottom-right',
          offset: currentOffset,
          message: h('div', {
            class: 'notification-content',
          }, [
            h('div', {
              class: 'notification-message',
              style: { cursor: 'pointer' },
              onClick: async () => {
                try {
                  await request({
                    url: '/MessageRecord/UpdateIsReadByMessageID/UpdateIsRead',
                    method: 'get',
                    params: {
                      MessagesID: msg.id
                    }
                  });
                } catch (error) {
                  console.error('调用更新已读接口失败:', error);
                }
                let targetPath = '';
                switch (msg.type) {
                  case 'TaskItem':
                    targetPath = `/plantask?taskId=${msg.targetId}`;
                    break;
                  case 'saleContact':
                    targetPath = `/sale/sale/salecontract?contractId=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'purchaseContract':
                    targetPath = `/purchase/procurementplansandcontracts?purchaseContractId=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'paymentApplication':
                    targetPath = `/paymentrequest?PaymentRequestID=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'Shipping':
                    targetPath = `/sale/shippingdelivery?contractId=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'Settlement':
                    targetPath = `/finance/settlementcenter?contractId=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'Inquiry':
                    targetPath = `/purchase/requestquote?inquiryId=${msg.targetId}&fromDashboard=true`;
                    break;
                  default:
                    targetPath = '/index';
                }
                // 用事件总线通知 App.vue 跳转
                eventBus.emit('notification-navigate', {
                  path: targetPath,
                  data: msg
                });
                notification.close();
              }
            }, msg.content),
            h('div', { class: 'notification-footer' }, [
              h('button', {
                class: 'custom-btn',
                onClick: async () => {
                  try {
                    const readResponse = await request({
                      url: '/MessageRecord/UpdateIsReadByMessageID/UpdateIsRead',
                      method: 'get',
                      params: {
                        MessagesID: msg.id
                      }
                    });

                    if (readResponse.code === 200) {
                      notification.close();
                      const index = notifications.indexOf(notification);
                      if (index > -1) {
                        notifications.splice(index, 1);
                        // 重新计算剩余通知的位置
                        notifications.forEach((n, i) => {
                          n.offset = i * 150; // 增加间距，避免重叠
                        });
                      }
                    }
                  } catch (error) {
                    console.error('调用更新已读接口失败:', error);
                    notification.close();
                  }
                }
              }, '我知道了')
            ])
          ])
        });

        notifications.push(notification);
        currentOffset += 150; // 增加间距，避免重叠
      });
    }
  } catch (error) {
    console.error('获取消息列表失败:', error);
  }
};


// #region 商机看板

// 文本截断方法
const truncateText = (text, length) => {
  if (!text) return ''
  if (text.length <= length) return text
  return text.slice(0, length) + '...'
}

// 获取商机看板数据
function getBusinessDashboard() {
  return request({
    url: 'BusinessOpportunity/GetBusinessOpportunityDashboardDataByUser/GetDashboardData',
    method: 'get'
  })
}

// 获取当日汇率数据
const getTodayExchangeRates = async () => {
  try {
    // 使用exchangeRateService获取所有币种的最新汇率
    const allRates = await exchangeRateService.getAllLatestExchangeRates();

    if (allRates && Object.keys(allRates).length > 0) {
      // 将汇率数据转换为显示格式
      const formattedRates = [];

      // 检查币种选项是否已加载
      const currencyOptions = optionss.value.hr_export_currency;
      if (!currencyOptions || currencyOptions.length === 0) {
        console.warn('币种选项数据未加载，等待数据加载完成...');
        // 延迟重试
        setTimeout(() => {
          getTodayExchangeRates();
        }, 1000);
        return;
      }

      // 遍历所有币种汇率
      for (const [currency, rate] of Object.entries(allRates)) {
        // 过滤掉人民币汇率（币种ID为'3'）
        if (currency === '3') {
          continue;
        }

        // 获取币种名称
        const currencyName = exchangeRateService.getCurrencyName(currency, currencyOptions);

        formattedRates.push({
          currency: currency,
          currencyName: currencyName,
          exchangeRate: exchangeRateService.formatExchangeRate(rate)
        });
      }

      todayExchangeRates.value = formattedRates;
      console.log('当日汇率数据加载完成:', formattedRates);
    } else {
      console.warn('未获取到任何汇率数据');
      todayExchangeRates.value = [];
    }
  } catch (error) {
    console.error('获取当日汇率出错:', error);
    todayExchangeRates.value = [];
  }
};

// 获取报价单看板数据
function getQuotationsKanBanData() {
  return request({
    url: 'Quotation/GetKanBanData/GetKanBanData',
    method: 'get'
  })
}

// 获取合同确定阶段数据
function getContractConfirmedKanBanData() {
  return request({
    url: 'Contracts/GetKanBanData/GetContactKanBanData',
    method: 'get'
  })
}

type CustomerStatisticsScope = 'Self' | 'Group' | 'Department' | 'Company'

interface CustomerStatisticsSummary {
  customerCount: number
  dealCustomerCount: number
  repurchaseCustomerCount: number
  lostCustomerCount: number
}

type SalesConversionStatType = 'Month' | 'Quarter' | 'Year'

interface SalesConversionSummary {
  inquiryCount: number
  quoteCount: number
  communicatingCount: number
  requoteCount: number
  dealCount: number
  lostCount: number
}

interface BusinessSummarySummary {
  totalDealAmount: number
  shippedAmount: number
  estimatedGrossProfit: number
}

const scopeLabelMap: Record<CustomerStatisticsScope, string> = {
  Self: '本人',
  Group: '本组',
  Department: '本部门',
  Company: '本公司'
}

const selectedCustomerScope = ref<CustomerStatisticsScope>('Self')
const customerScopeOptions = ref<{ value: CustomerStatisticsScope; label: string }[]>([])
const customerStatisticsLoading = ref(false)
const customerStatisticsSummary = ref<CustomerStatisticsSummary>({
  customerCount: 0,
  dealCustomerCount: 0,
  repurchaseCustomerCount: 0,
  lostCustomerCount: 0
})
const salesConversionStatTypeOptions: Array<{ value: SalesConversionStatType; label: string }> = [
  { value: 'Month', label: '按月' },
  { value: 'Quarter', label: '按季' },
  { value: 'Year', label: '按年' }
]
const salesConversionMonthOptions = Array.from({ length: 12 }, (_, index) => ({
  value: index + 1,
  label: `${index + 1}月`
}))
const salesConversionQuarterOptions = [
  { value: 1, label: '第一季度' },
  { value: 2, label: '第二季度' },
  { value: 3, label: '第三季度' },
  { value: 4, label: '第四季度' }
]
const currentYear = new Date().getFullYear()
const salesConversionYearOptions = Array.from({ length: 6 }, (_, index) => currentYear - index)
const selectedSalesConversionScope = ref<CustomerStatisticsScope>('Self')
const salesConversionScopeOptions = ref<{ value: CustomerStatisticsScope; label: string }[]>([])
const selectedSalesConversionStatType = ref<SalesConversionStatType>('Month')
const selectedSalesConversionYear = ref<number>(currentYear)
const selectedSalesConversionMonth = ref<number>(new Date().getMonth() + 1)
const selectedSalesConversionQuarter = ref<number>(Math.floor(new Date().getMonth() / 3) + 1)
const salesConversionLoading = ref(false)
const salesConversionReady = ref(false)
const salesConversionSummary = ref<SalesConversionSummary>({
  inquiryCount: 0,
  quoteCount: 0,
  communicatingCount: 0,
  requoteCount: 0,
  dealCount: 0,
  lostCount: 0
})
const businessSummaryYearOptions = Array.from({ length: 6 }, (_, index) => currentYear - index)
const selectedBusinessSummaryScope = ref<CustomerStatisticsScope>('Self')
const businessSummaryScopeOptions = ref<{ value: CustomerStatisticsScope; label: string }[]>([])
const selectedBusinessSummaryYear = ref<number>(currentYear)
const businessSummaryLoading = ref(false)
const businessSummaryReady = ref(false)
const businessSummarySummary = ref<BusinessSummarySummary>({
  totalDealAmount: 0,
  shippedAmount: 0,
  estimatedGrossProfit: 0
})
const businessSummaryMonthlyDealAmounts = ref<number[]>(Array.from({ length: 12 }, () => 0))
const businessSummaryMonthlyShippedAmounts = ref<number[]>(Array.from({ length: 12 }, () => 0))

const mapCustomerStatisticsSummary = (summary: any): CustomerStatisticsSummary => {
  const numberValue = (value: unknown) => {
    const parsed = Number(value)
    return Number.isFinite(parsed) ? parsed : 0
  }

  return {
    customerCount: numberValue(summary?.customerCount ?? summary?.totalCustomerCount ?? summary?.customerTotal),
    dealCustomerCount: numberValue(summary?.dealCustomerCount ?? summary?.transactionCustomerCount),
    repurchaseCustomerCount: numberValue(summary?.repurchaseCustomerCount ?? summary?.repeatPurchaseCustomerCount),
    lostCustomerCount: numberValue(summary?.lostCustomerCount ?? summary?.churnedCustomerCount)
  }
}

const mapCustomerScopeOptions = (scopes: any[]) => {
  const fallbackOptions: Array<{ value: CustomerStatisticsScope; label: string }> = [
    { value: 'Self', label: scopeLabelMap.Self },
    { value: 'Group', label: scopeLabelMap.Group },
    { value: 'Department', label: scopeLabelMap.Department },
    { value: 'Company', label: scopeLabelMap.Company }
  ]

  if (!Array.isArray(scopes) || scopes.length === 0) {
    return fallbackOptions
  }

  return scopes
    .map((item) => {
      const scopeType = item?.scopeType as CustomerStatisticsScope
      if (!scopeType || !scopeLabelMap[scopeType]) return null
      return {
        value: scopeType,
        label: item?.scopeName || scopeLabelMap[scopeType]
      }
    })
    .filter(Boolean) as Array<{ value: CustomerStatisticsScope; label: string }>
}

const mapSalesConversionSummary = (summary: any): SalesConversionSummary => {
  const numberValue = (value: unknown) => {
    const parsed = Number(value)
    return Number.isFinite(parsed) ? parsed : 0
  }

  return {
    inquiryCount: numberValue(summary?.inquiryCount ?? summary?.inquiryNum ?? summary?.inquiryTotal),
    quoteCount: numberValue(summary?.quoteCount ?? summary?.quotationCount ?? summary?.quotedCount),
    communicatingCount: numberValue(summary?.communicatingCount ?? summary?.communicationCount ?? summary?.communicateCount),
    requoteCount: numberValue(summary?.requoteCount ?? summary?.requoteNum ?? summary?.reQuotationCount),
    dealCount: numberValue(summary?.dealCount ?? summary?.transactionCount ?? summary?.contractCount),
    lostCount: numberValue(summary?.lostCount ?? summary?.lossCount ?? summary?.lostNum)
  }
}

const mapBusinessSummary = (data: any) => {
  const numberValue = (value: unknown) => {
    const parsed = Number(value)
    return Number.isFinite(parsed) ? parsed : 0
  }

  const summaryData = data?.summary || data || {}
  const monthlyDataRaw = Array.isArray(data?.monthlyData)
    ? data.monthlyData
    : (Array.isArray(data?.monthData) ? data.monthData : (Array.isArray(data?.monthlyDetails) ? data.monthlyDetails : []))

  const dealAmounts = Array.from({ length: 12 }, (_, index) => {
    const monthData = monthlyDataRaw.find((item: any) => Number(item?.month) === index + 1) || {}
    return numberValue(monthData?.dealAmount ?? monthData?.monthlyDealAmount ?? monthData?.成交金额)
  })

  const shippedAmounts = Array.from({ length: 12 }, (_, index) => {
    const monthData = monthlyDataRaw.find((item: any) => Number(item?.month) === index + 1) || {}
    return numberValue(monthData?.shippedAmount ?? monthData?.monthlyShippedAmount ?? monthData?.出运金额)
  })

  return {
    summary: {
      totalDealAmount: numberValue(summaryData?.totalDealAmount ?? summaryData?.yearDealAmount ?? summaryData?.totalTransactionAmount),
      shippedAmount: numberValue(summaryData?.shippedAmount ?? summaryData?.totalShippedAmount ?? summaryData?.yearShipmentAmount),
      estimatedGrossProfit: numberValue(summaryData?.estimatedGrossProfit ?? summaryData?.grossProfitAmount ?? summaryData?.totalEstimatedGrossProfit)
    },
    dealAmounts,
    shippedAmounts
  }
}

const formatBusinessAmountDisplay = (amount: number) => {
  return `￥ ${formatAmount(amount)}`
}

const buildBusinessSummaryLinePoints = (values: number[]) => {
  const width = 1000
  const height = 240
  const paddingX = 18
  const paddingY = 18
  const innerWidth = width - paddingX * 2
  const innerHeight = height - paddingY * 2
  const safeValues = values.length === 12 ? values : Array.from({ length: 12 }, (_, index) => Number(values[index] || 0))
  const maxValue = Math.max(...safeValues, 1)
  return safeValues
    .map((value, index) => {
      const x = paddingX + (index * innerWidth) / 11
      const y = paddingY + innerHeight - (Number(value || 0) / maxValue) * innerHeight
      return `${x},${y}`
    })
    .join(' ')
}

const businessSummaryDealLinePoints = computed(() => buildBusinessSummaryLinePoints(businessSummaryMonthlyDealAmounts.value))
const businessSummaryShippedLinePoints = computed(() => buildBusinessSummaryLinePoints(businessSummaryMonthlyShippedAmounts.value))

const getCustomerStatisticsHomeData = async () => {
  return request({
    url: 'CustomerInfoMation/GetCustomerStatisticsHomeData/GetCustomerStatisticsHomeData',
    method: 'get'
  })
}

const getCustomerStatistics = async (scopeType: CustomerStatisticsScope) => {
  return request({
    url: 'CustomerInfoMation/GetCustomerStatistics/GetCustomerStatistics',
    method: 'get',
    params: { scopeType }
  })
}

const getSalesConversionStatisticsHomeData = async () => {
  return request({
    url: 'BusinessOpportunity/GetSalesConversionStatisticsHomeData/GetSalesConversionStatisticsHomeData',
    method: 'get'
  })
}

const getSalesConversionStatistics = async (params: {
  scopeType: CustomerStatisticsScope
  statType: SalesConversionStatType
  year?: number
  month?: number
  quarter?: number
}) => {
  return request({
    url: 'BusinessOpportunity/GetSalesConversionStatistics/GetSalesConversionStatistics',
    method: 'get',
    params
  })
}

const getBusinessSummaryStatistics = async (params: {
  scopeType: CustomerStatisticsScope
  year?: number
}) => {
  return request({
    url: 'BusinessOpportunity/GetBusinessSummaryStatistics/GetBusinessSummaryStatistics',
    method: 'get',
    params
  })
}

const loadCustomerStatisticsHomeData = async () => {
  customerStatisticsLoading.value = true
  try {
    const response = await getCustomerStatisticsHomeData()
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取客户统计首页数据失败')
      return
    }

    const homeData = response.data || {}
    const options = mapCustomerScopeOptions(homeData.availableScopes)
    customerScopeOptions.value = options

    const defaultScope = (homeData.defaultScope || 'Self') as CustomerStatisticsScope
    selectedCustomerScope.value = options.some(x => x.value === defaultScope) ? defaultScope : options[0]?.value || 'Self'
    customerStatisticsSummary.value = mapCustomerStatisticsSummary(homeData.summary || {})
  } catch (error) {
    console.error('获取客户统计首页数据失败:', error)
    ElMessage.error('获取客户统计首页数据失败')
  } finally {
    customerStatisticsLoading.value = false
  }
}

const handleCustomerScopeChange = async (scopeType: CustomerStatisticsScope) => {
  customerStatisticsLoading.value = true
  try {
    const response = await getCustomerStatistics(scopeType)
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取客户统计失败')
      return
    }

    customerStatisticsSummary.value = mapCustomerStatisticsSummary(response?.data?.summary || {})
  } catch (error) {
    console.error('获取客户统计失败:', error)
    ElMessage.error('获取客户统计失败')
  } finally {
    customerStatisticsLoading.value = false
  }
}

const loadSalesConversionStatisticsHomeData = async () => {
  salesConversionLoading.value = true
  try {
    const response = await getSalesConversionStatisticsHomeData()
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取销售转化统计首页数据失败')
      return
    }

    const homeData = response.data || {}
    const options = mapCustomerScopeOptions(homeData.availableScopes)
    salesConversionScopeOptions.value = options

    const defaultScope = (homeData.defaultScope || 'Self') as CustomerStatisticsScope
    selectedSalesConversionScope.value = options.some(x => x.value === defaultScope) ? defaultScope : options[0]?.value || 'Self'

    const defaultStatType = (homeData.defaultStatType || 'Month') as SalesConversionStatType
    selectedSalesConversionStatType.value = salesConversionStatTypeOptions.some(x => x.value === defaultStatType)
      ? defaultStatType
      : 'Month'

    selectedSalesConversionYear.value = currentYear
    selectedSalesConversionMonth.value = new Date().getMonth() + 1
    selectedSalesConversionQuarter.value = Math.floor(new Date().getMonth() / 3) + 1
    salesConversionSummary.value = mapSalesConversionSummary(homeData.summary || {})
    salesConversionReady.value = true
  } catch (error) {
    console.error('获取销售转化统计首页数据失败:', error)
    ElMessage.error('获取销售转化统计首页数据失败')
  } finally {
    salesConversionLoading.value = false
  }
}

const loadSalesConversionStatistics = async () => {
  salesConversionLoading.value = true
  try {
    const params: {
      scopeType: CustomerStatisticsScope
      statType: SalesConversionStatType
      year?: number
      month?: number
      quarter?: number
    } = {
      scopeType: selectedSalesConversionScope.value,
      statType: selectedSalesConversionStatType.value,
      year: selectedSalesConversionYear.value
    }

    if (selectedSalesConversionStatType.value === 'Month') {
      params.month = selectedSalesConversionMonth.value
    } else if (selectedSalesConversionStatType.value === 'Quarter') {
      params.quarter = selectedSalesConversionQuarter.value
    }

    const response = await getSalesConversionStatistics(params)
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取销售转化统计失败')
      return
    }

    salesConversionSummary.value = mapSalesConversionSummary(response?.data?.summary || response?.data || {})
  } catch (error) {
    console.error('获取销售转化统计失败:', error)
    ElMessage.error('获取销售转化统计失败')
  } finally {
    salesConversionLoading.value = false
  }
}

const loadBusinessSummaryStatistics = async () => {
  businessSummaryLoading.value = true
  try {
    const params = {
      scopeType: selectedBusinessSummaryScope.value,
      year: selectedBusinessSummaryYear.value
    }
    const response = await getBusinessSummaryStatistics(params)
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取经营数据统计失败')
      return
    }

    const mapped = mapBusinessSummary(response?.data || {})
    businessSummarySummary.value = mapped.summary
    businessSummaryMonthlyDealAmounts.value = mapped.dealAmounts
    businessSummaryMonthlyShippedAmounts.value = mapped.shippedAmounts
  } catch (error) {
    console.error('获取经营数据统计失败:', error)
    ElMessage.error('获取经营数据统计失败')
  } finally {
    businessSummaryLoading.value = false
  }
}

const loadBusinessSummaryHomeData = async () => {
  businessSummaryLoading.value = true
  try {
    const response = await getBusinessSummaryStatistics({
      scopeType: 'Self',
      year: currentYear
    })
    if (response?.code !== 200) {
      ElMessage.error(response?.msg || '获取经营数据统计失败')
      return
    }

    businessSummaryScopeOptions.value = customerScopeOptions.value.length > 0
      ? customerScopeOptions.value
      : mapCustomerScopeOptions([])
    selectedBusinessSummaryScope.value = businessSummaryScopeOptions.value.some(x => x.value === 'Self')
      ? 'Self'
      : (businessSummaryScopeOptions.value[0]?.value || 'Self')
    selectedBusinessSummaryYear.value = currentYear

    const mapped = mapBusinessSummary(response?.data || {})
    businessSummarySummary.value = mapped.summary
    businessSummaryMonthlyDealAmounts.value = mapped.dealAmounts
    businessSummaryMonthlyShippedAmounts.value = mapped.shippedAmounts
    businessSummaryReady.value = true
  } catch (error) {
    console.error('获取经营数据统计失败:', error)
    ElMessage.error('获取经营数据统计失败')
  } finally {
    businessSummaryLoading.value = false
  }
}

watch(
  customerScopeOptions,
  (options) => {
    if (!Array.isArray(options) || options.length === 0) return
    businessSummaryScopeOptions.value = options
    if (!options.some(x => x.value === selectedBusinessSummaryScope.value)) {
      selectedBusinessSummaryScope.value = options[0].value
    }
  },
  { deep: true }
)

watch(
  [
    selectedSalesConversionScope,
    selectedSalesConversionStatType,
    selectedSalesConversionYear,
    selectedSalesConversionMonth,
    selectedSalesConversionQuarter
  ],
  () => {
    if (!isSalesDeptUser() || !salesConversionReady.value) return
    loadSalesConversionStatistics()
  }
)

watch(
  [selectedBusinessSummaryScope, selectedBusinessSummaryYear],
  () => {
    if (!isSalesDeptUser() || !businessSummaryReady.value) return
    loadBusinessSummaryStatistics()
  }
)

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
    // 获取商机看板数据
    const response = await getBusinessDashboard()
    console.log('商机看板数据:', response)
    if (response.code === 200) {
      // 用stageOrder补全所有阶段
      const stages = stageOrder.map(stageName => {
        const existingStage = response.data.find(s => s.salesStage === stageName)
        if (existingStage) {
          // 处理每个商机的businessSource字段
          existingStage.details = existingStage.details.map(item => {
            let customerLabel = '客户邮箱'
            if (item.businessSource === '联系日志') {
              customerLabel = '客户名称'
            }
            return {
              ...item,
              customerLabel: customerLabel
            }
          })
        }
        return existingStage || {
          salesStage: stageName,
          count: 0,
          details: [],
          totalAmount: 0
        }
      })
      businessStages.value = stages
      console.log('处理后的商机看板数据:', businessStages.value)
    }
    // 获取报价单数据
    const quotationResponse = await getQuotationsKanBanData()
    console.log('报价单看板数据:', quotationResponse)
    if (quotationResponse.code === 200) {
      // 处理初次报价数据
      const initialQuotes = quotationResponse.data.initialQuotes || []
      // 处理最终报价数据
      const finalQuotes = quotationResponse.data.finalQuotes || []
      console.log('初次报价数据:', initialQuotes)
      console.log('最终报价数据:', finalQuotes)

      // 找到初次报价和再次报价的索引
      const initialQuoteIndex = businessStages.value.findIndex(stage => stage.salesStage === '初次报价')
      const finalQuoteIndex = businessStages.value.findIndex(stage => stage.salesStage === '再次报价')
      console.log('初次报价索引:', initialQuoteIndex)
      console.log('最终报价索引:', finalQuoteIndex)

      // 更新初次报价数据
      if (initialQuoteIndex !== -1) {
        // 添加报价数据到初次报价部分
        const initialQuoteDetails = initialQuotes.map(quote => ({
          id: quote.id,
          opportunityNumber: quote.quotationNum || '无编号',
          businessName: quote.customerAbbreviation || quote.customerName || '无客户简称',
          customer: quote.contactPersonEmail || quote.customerName || '无客户信息',
          amount: quote.totalValueOfGoods * quote.exchangeRate || 0,
          currency: quote.currency || '3', // 默认CNY
          projectDescription: quote.projectDescription || '无项目简介',
          create_time: quote.createTime || new Date().toISOString(),
          sourceType: 'api',
          customerLabel: '客户简称'
        }))
        console.log('处理后的初次报价数据:', initialQuoteDetails)

        // 直接合并数据
        businessStages.value[initialQuoteIndex].details = [
          ...businessStages.value[initialQuoteIndex].details,
          ...initialQuoteDetails
        ]
        businessStages.value[initialQuoteIndex].count = businessStages.value[initialQuoteIndex].details.length
        businessStages.value[initialQuoteIndex].totalAmount = businessStages.value[initialQuoteIndex].details.reduce((sum, item) => sum + (item.amount || 0), 0)
        console.log('更新后的初次报价阶段:', businessStages.value[initialQuoteIndex])
      }

      // 更新再次报价数据
      if (finalQuoteIndex !== -1) {
        // 添加报价数据到再次报价部分
        const finalQuoteDetails = finalQuotes.map(quote => ({
          id: quote.id,
          opportunityNumber: quote.quotationNum || '无编号',
          businessName: quote.customerAbbreviation || quote.customerName || '无客户简称',
          customer: quote.contactPersonEmail || quote.customerName || '无客户信息',
          amount: quote.totalValueOfGoods * quote.exchangeRate || 0,
          currency: quote.currency || '3', // 默认CNY
          projectDescription: quote.projectDescription || '无项目简介',
          create_time: quote.createTime || new Date().toISOString(),
          sourceType: 'api',
          customerLabel: '客户简称'
        }))
        console.log('处理后的最终报价数据:', finalQuoteDetails)

        // 直接合并数据
        businessStages.value[finalQuoteIndex].details = [
          ...businessStages.value[finalQuoteIndex].details,
          ...finalQuoteDetails
        ]
        businessStages.value[finalQuoteIndex].count = businessStages.value[finalQuoteIndex].details.length
        businessStages.value[finalQuoteIndex].totalAmount = businessStages.value[finalQuoteIndex].details.reduce((sum, item) => sum + (item.amount || 0), 0)
        console.log('更新后的最终报价阶段:', businessStages.value[finalQuoteIndex])
      }
    }
    // 获取合同确定阶段数据
    const contractConfirmedResponse = await getContractConfirmedKanBanData()
    console.log('合同确定看板数据:', contractConfirmedResponse)
    if (contractConfirmedResponse.code === 200) {
      const contractConfirmedQuotes = contractConfirmedResponse.data || []
      const contractConfirmedIndex = businessStages.value.findIndex(stage => stage.salesStage === '合同确定')
      if (contractConfirmedIndex !== -1) {
        const contractConfirmedDetails = contractConfirmedQuotes.map(quote => ({
          id: quote.id,
          opportunityNumber: quote.contractNumber || '无编号',
          businessName: quote.customerAbbreviation || quote.customerName || '无客户简称',
          customer: quote.contactEmail || '无客户邮箱',
          amount: (quote.amountTotal || 0) * (quote.exchangeRate || 1),
          currency: quote.currency || '3', // 默认CNY
          projectDescription: quote.projectDescription || '无项目简介',
          create_time: quote.createTime || new Date().toISOString(),
          sourceType: 'api',
          customerLabel: '客户邮箱'
        }))
        businessStages.value[contractConfirmedIndex].details = [
          ...businessStages.value[contractConfirmedIndex].details,
          ...contractConfirmedDetails
        ]
        businessStages.value[contractConfirmedIndex].count = businessStages.value[contractConfirmedIndex].details.length
        businessStages.value[contractConfirmedIndex].totalAmount = businessStages.value[contractConfirmedIndex].details.reduce((sum, item) => sum + (item.amount || 0), 0)
      }
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

// 图片和附件预览相关
const imagePreviewVisible = ref(false)
const previewImageUrl = ref('')

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
const handleClaim = async (row) => {
  claimForm.id = row.id
  claimDialogVisible.value = true

  try {
    // 清空缓存
    associatedDocumentOptionsCache.value = {};

    // 调用接口获取客户收款单详情
    const response = await request({
      url: 'CustomerCollections/GetCustomerCollectionDetails/GetDetails',
      method: 'GET',
      params: {
        id: row.id
      }
    })

    if (response.code === 200 && response.data) {
      const details = response.data

      // 绑定数据到表单
      addcustomercollectionform.value = {
        receiptNumber: details.receiptNumber || '',
        receiptDate: details.receiptDate || '',
        ourCompany: details.ourCompany || '',
        foreignCurrency: details.foreignCurrency || '',
        exchangeRate: details.exchangeRate ? String(parseFloat(details.exchangeRate)) : '',
        amount: details.amount || '',
        bank: details.bank || '',
        attachment: details.attachment || '',
        receiptImageUrl: details.receiptImageUrl || '',
        attachmentUrl: details.attachmentUrl || '',
        Customer: details.customer || '',
        FundsClassification: details.fundsClassification || '',
        AssociatedModules: details.associatedModules || '',
        AssociatedModulesDocumentID: details.associatedModulesDocumentID || '',
        ExchangeSettlementAmount: details.exchangeSettlementAmount || ''
      }

      // 如果有收款明细数据，也绑定到表格
      if (details.receivingPaymentsDetails && Array.isArray(details.receivingPaymentsDetails)) {
        ReceivingPaymentsDetailsTbaleData.value = details.receivingPaymentsDetails.map(item => ({
          fundsClassification: item.fundsClassification || '',
          associatedModules: item.associatedModules || '',
          associatedModulesDocumentID: item.associatedModulesDocumentID || '',
          applicationamount: item.applicationamount || ''
        }))
        // 重新计算总金额
        calculateTotal()
        // 为每一行单独获取关联单号选项
        const customerID = addcustomercollectionform.value?.Customer;
        if (customerID) {
          for (const row of ReceivingPaymentsDetailsTbaleData.value) {
            if (row.fundsClassification) {
              await getAssociatedDocumentOptionsData(row.fundsClassification, customerID);
            }
          }
        }
      } else {
        ReceivingPaymentsDetailsTbaleData.value = []
        totalAmount.value = 0
      }
    } else {
      ElMessage.error('获取收款单详情失败')
    }
  } catch (error) {
    console.error('获取收款单详情失败:', error)
    ElMessage.error('获取收款单详情失败')
  }
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

// 处理图片和附件显示的方法
// 获取图片URL数组
const getImageUrls = (imageUrlString) => {
  if (!imageUrlString) return [];
  return imageUrlString.split(',').filter(url => url.trim());
};

// 获取附件URL数组
const getAttachmentUrls = (attachmentUrlString) => {
  if (!attachmentUrlString) return [];
  return attachmentUrlString.split(',').filter(url => url.trim());
};

// 从URL中提取文件名
const getFileName = (url) => {
  if (!url) return '未知文件';
  const parts = url.split('/');
  return parts[parts.length - 1] || '未知文件';
};

// 预览图片
const previewImage = (imageUrl) => {
  previewImageUrl.value = imageUrl;
  imagePreviewVisible.value = true;
};

// 预览附件
const previewAttachment = (attachmentUrl) => {
  window.open(attachmentUrl, '_blank');
};

// 下载附件
const downloadAttachment = (attachmentUrl, fileName) => {
  const link = document.createElement('a');
  link.href = attachmentUrl;
  link.download = fileName;
  link.target = '_blank';
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

// 提交领取
const submitClaim = () => {
  // 验证收款明细总金额
  if (!validateTotalAmount()) {
    const errorMessage = getValidationMessage();
    if (errorMessage) {
      ElMessage.warning(errorMessage);
    }
    return;
  }

  // 验证客户是否已选择
  if (!addcustomercollectionform.value.Customer) {
    ElMessage.warning('请选择客户');
    return;
  }

  // 验证收款明细是否为空
  if (ReceivingPaymentsDetailsTbaleData.value.length === 0) {
    ElMessage.warning('请至少添加一条收款明细');
    return;
  }

  // 验证收款明细数据完整性
  for (let i = 0; i < ReceivingPaymentsDetailsTbaleData.value.length; i++) {
    const item = ReceivingPaymentsDetailsTbaleData.value[i];
    if (!item.fundsClassification) {
      ElMessage.warning(`第${i + 1}行请选择款项类别`);
      return;
    }
    if (!item.associatedModulesDocumentID) {
      ElMessage.warning(`第${i + 1}行请选择关联单号`);
      return;
    }
    if (!item.applicationamount || parseFloat(item.applicationamount) <= 0) {
      ElMessage.warning(`第${i + 1}行请输入有效金额`);
      return;
    }
  }

  try {
    // 构建请求数据，匹配后端 AssigningCustomerCollectionsRequest 结构
    const requestData = {
      id: claimForm.id,
      CustomerID: parseInt(addcustomercollectionform.value.Customer),
      customerCollectionsItens: ReceivingPaymentsDetailsTbaleData.value.map(item => ({
        CustomerCollectionsID: claimForm.id,
        FundsClassification: parseInt(item.fundsClassification),
        AssociatedModulesDocument: item.associatedModulesDocumentID,
        ApplicationAmount: parseFloat(item.applicationamount)
      }))
    };

    request({
      url: 'CustomerCollections/AssigningCustomerCollections/Assigning',
      method: 'post',
      data: requestData
    }).then((response) => {
      if (response && response.code === 200) {
        ElMessage.success(response.data || '客户收款单领取成功！')
        claimDialogVisible.value = false
        // 重新加载列表
        getFinancialTasksList(
          FinancialTasksTableCurrentPage.value,
          FinancialTasksTablePageSize.value
        )
      } else {
        ElMessage.error(response?.msg || '客户收款单领取失败！')
      }
    }).catch(error => {
      console.error('领取失败:', error)
      ElMessage.error('客户收款单领取失败！')
    })
  } catch (error) {
    console.error('数据验证失败:', error)
    ElMessage.error('数据验证失败')
  }
}

// 对话框关闭时重置表单
const resetClaimForm = () => {
  claimForm.contractsID = ''
  claimForm.fundsType = ''
  claimForm.remark = ''
  claimForm.contractType = ''
  claimFormRef.value?.resetFields()

  // 重置收款单表单数据
  addcustomercollectionform.value = {
    receiptNumber: '',
    receiptDate: '',
    ourCompany: '',
    foreignCurrency: '',
    exchangeRate: '',
    amount: '',
    bank: '',
    attachment: '',
    receiptImageUrl: '',
    attachmentUrl: '',
    Customer: '',
    FundsClassification: '',
    AssociatedModules: '',
    AssociatedModulesDocumentID: '',
    ExchangeSettlementAmount: ''
  }

  // 清空收款明细表格数据
  ReceivingPaymentsDetailsTbaleData.value = []
  totalAmount.value = 0
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
// 收款单确认表格数据
const receiptConfirmTableData = ref([])
const receiptConfirmTableFullData = ref([])
const receiptConfirmTableTotalItems = ref(0)
const receiptConfirmTableCurrentPage = ref(1)
const receiptConfirmTablePageSize = ref(10)
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
  FinancialTasksTableData.value = [];
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
        // 保存原始货币值用于显示货币符号
        item.foreignCurrencyValue = item.foreignCurrency;
        item.foreignCurrency = state.optionss.hr_export_currency.find(dict => dict.dictValue === item.foreignCurrency)?.dictLabel;

        // 根据我方公司选择对应的收汇银行数据源
        let bankOptions = [];
        if (item.ourCompany === '荣发塑料') {
          bankOptions = state.optionss.hr_rf_receiving_bank || [];
        } else if (item.ourCompany === '惠荣进出口') {
          bankOptions = state.optionss.hr_receiving_bank || [];
        }
        item.bank = bankOptions.find(dict => dict.dictValue === item.bank)?.dictLabel || item.bank;
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

const normalizeCustomerCollectionItem = (item) => {
  const normalized = { ...item };
  normalized.customerIDRaw = item.customerID;
  normalized.receivingUserRaw = item.receivingUser;

  normalized.ourCompany = state.optionss.hr_ourcompany.find(dict => dict.dictValue === item.ourCompany)?.dictLabel;
  normalized.foreignCurrencyValue = item.foreignCurrency;
  normalized.foreignCurrency = state.optionss.hr_export_currency.find(dict => dict.dictValue === item.foreignCurrency)?.dictLabel;

  let bankOptions = [];
  if (normalized.ourCompany === '荣发塑料') {
    bankOptions = state.optionss.hr_rf_receiving_bank || [];
  } else if (normalized.ourCompany === '惠荣进出口') {
    bankOptions = state.optionss.hr_receiving_bank || [];
  }
  normalized.bank = bankOptions.find(dict => dict.dictValue === item.bank)?.dictLabel || item.bank;

  normalized.isCollected = !!(item.customerID && item.receivingUser);
  normalized.isConfirm = typeof item.isConfirm === 'string' ? Number(item.isConfirm) : item.isConfirm;
  return normalized;
};

const filterReceiptConfirmList = (list) => list.filter(item => item.isCollected && Number(item.isConfirm) === 0);

const setReceiptConfirmPage = (pageNum) => {
  receiptConfirmTableCurrentPage.value = pageNum;
  const startIndex = (pageNum - 1) * receiptConfirmTablePageSize.value;
  const endIndex = startIndex + receiptConfirmTablePageSize.value;
  receiptConfirmTableData.value = receiptConfirmTableFullData.value.slice(startIndex, endIndex);
};

const receiptConfirmTableHandlePageChange = (newPage) => {
  setReceiptConfirmPage(newPage);
};

const loadReceiptConfirmList = async () => {
  try {
    const pageSize = 200;
    let pageNum = 1;
    let totalNum = 0;
    let allItems = [];

    const firstResponse = await request({
      url: 'CustomerCollections/GetCustomerCollectionsList/GetList',
      method: 'GET',
      params: {
        PageNum: pageNum,
        PageSize: pageSize
      }
    });

    const firstResult = firstResponse?.data?.result || [];
    totalNum = firstResponse?.data?.totalNum || firstResult.length;
    allItems = allItems.concat(firstResult);

    const totalPages = Math.ceil(totalNum / pageSize);
    for (let i = 2; i <= totalPages; i++) {
      const res = await request({
        url: 'CustomerCollections/GetCustomerCollectionsList/GetList',
        method: 'GET',
        params: {
          PageNum: i,
          PageSize: pageSize
        }
      });
      const result = res?.data?.result || [];
      allItems = allItems.concat(result);
    }

    const normalized = allItems.map(normalizeCustomerCollectionItem);
    receiptConfirmTableFullData.value = filterReceiptConfirmList(normalized);
    receiptConfirmTableTotalItems.value = receiptConfirmTableFullData.value.length;
    setReceiptConfirmPage(receiptConfirmTableCurrentPage.value);
  } catch (error) {
    console.error('获取收款单确认列表失败:', error);
    receiptConfirmTableFullData.value = [];
    receiptConfirmTableData.value = [];
    receiptConfirmTableTotalItems.value = 0;
  }
};

const getReceiptConfirmTotalCount = async () => {
  await loadReceiptConfirmList();
  return receiptConfirmTableTotalItems.value;
};

// #endregion

//销售合同产品表格
const productData = ref([]);
const hasChangedProducts = computed(() => productData.value.some(product => product.isPriceChanged === 1));
const hasReceivedDepositShow = computed(() => DepositShow.value && Number(contractform.receivedDeposit) > 0);

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
      // 触发业务示警更新事件
      eventBus.emit('updateBusinessAlerts');
      // 触发工作任务更新事件
      eventBus.emit('updateWorkTasks');
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
      // 触发业务示警更新事件
      eventBus.emit('updateBusinessAlerts');
      // 触发工作任务更新事件
      eventBus.emit('updateWorkTasks');
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
const basicInfoCollapseActive = ref(['basicInfo']);
const auxiliaryInfoCollapseActive = ref(['auxiliaryInfo']);
const profitCalculationCollapseActive = ref(['profitCalculation']);
const contractReviewStatus = ref('');
const ReceivedExpenseDetailsTableData = ref([]);
const CustomerRelaterExoensesTableData = ref([]);
const contractDialog = ref(false);
const contractform = reactive({
  id: 0,                     // 合同ID
  contractNumber: '',        // 合同编号
  contractDate: '',         // 合同日期
  contractStatus: null,     // 合同状态
  quotationNumber: '',      // 报价单号
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
  hasDeposit: false,        // 是否有预付款
  receivedDeposit: null,    // 已收预付款
  depositDate: '',          // 预付款日期
  Depositratio: null,       // 预付款比例
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
  salesContract: '',
  deliveryLocation: '',
  hasDeposit: false,
  activeName: 'productinfo',
  productinfotableData: [],
  CustomerRelaterExoensesTableData: [],
  totalValue: '',
  totalQuantity: '',
  totalBoxCount: '',
  totalGrossWeight: '',
  totalNetWeight: '',
  totalVolume: '',
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

// 获取已收费用明细
const getReceivedExpenseDetails = (contractId) => {
  ReceivedExpenseDetailsTableData.value = [];
  if (!contractId) return;
  request({
    url: 'CustomerCollections/GetCustomerCollectionsByContractID/GetList',
    method: 'GET',
    params: {
      ContractID: contractId
    }
  }).then(response => {
    if (response.data) {
      ReceivedExpenseDetailsTableData.value = response.data.map(item => ({
        receiptNumber: item.receiptNumber,
        receiptDate: item.receiptDate,
        ourCompany: state.optionss.hr_ourcompany.find(x => x.dictValue === item.ourCompany?.toString())?.dictLabel || '',
        foreignCurrency: state.optionss.hr_export_currency.find(x => x.dictValue === item.foreignCurrency?.toString())?.dictLabel || '',
        exchangeRate: item.exchangeRate,
        amount: item.amount,
        bank: state.optionss.hr_bank.find(x => x.dictValue === item.bank?.toString())?.dictLabel || '',
        fundsType: state.optionss.funds_type.find(x => x.dictValue === item.fundsType?.toString())?.dictLabel || ''
      }));
    }
  }).catch(error => {
    console.error('获取已收费用明细失败:', error);
    ReceivedExpenseDetailsTableData.value = [];
  });
}
const openSaleContractDialog = (row) => {
  ApproveDocumentRequest.ApprovalRecordID = row.recordID;
  ApproveDocumentRequest.DocumentID = row.documentID;
  ApproveDocumentRequest.StageID = row.stageID;
  ApproveDocumentRequest.ApproverID = row.approverID;
  ApproveDocumentRequest.DocumentType = row.documentType;
  //获取销售合同详情（1=销售合同审批 7=修改交货日期审批 9=销售合同手动完结审批）
  if (row.documentType == "1" || row.documentType == "7" || row.documentType == "9") {
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
        contractReviewStatus.value = response.data.contract?.reviewStatus != null
          ? getReviewStatusText(response.data.contract.reviewStatus)
          : (row.contractReviewStatusStr || '');
        contractform.customerid = response.data.contract.customerId;
        const customerOption = state.optionss['sql_user_customers'].find(item => item.dictValue === response.data.contract.customerId.toString())
          || state.optionss['sql_hr_customer'].find(item => item.dictValue === response.data.contract.customerId.toString());
        if (customerOption) {
          contractform.customerNumber = customerOption.dictValue;
          contractform.customerAbbreviation = customerOption.dictValue;
        } else {
          contractform.customerNumber = response.data.contract.customerId?.toString() || '';
          contractform.customerAbbreviation = response.data.contract.customerId?.toString() || '';
        }
        contractform.quotationNumber = response.data.contract.relatedQuotation || response.data.contract.quotationNumber || '';
        GetCustomerContactPerson(response.data.contract.customerId);
        contractform.contactPerson = response.data.contract.contactPerson;
        contractform.contactEmail = response.data.contract.contactEmail;
        contractform.effectiveDate = response.data.contract.effectiveDate;
        if (response.data.contract.customerLevel != null) {
          contractform.customerLevel = state.optionss['hr_customer_level'].find(item => item.dictValue === response.data.contract.customerLevel.toString()).dictValue;
        }
        contractform.customerContract = response.data.contract.customerContract;
        contractform.deliveryDate = response.data.contract.deliveryDate;
        contractform.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictValue === response.data.contract.ourCompany.toString()).dictValue;
        if (response.data.contract.settlementType) {
          contractform.settlementType = response.data.contract.settlementType.toString();
        }
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
        if (response.data.contract.shippingCurrency) {
          contractform.shippingCurrency = state.optionss['hr_export_currency'].find(item => item.dictValue === response.data.contract.shippingCurrency.toString()).dictValue;
        }
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
        getReceivedExpenseDetails(contractform.id);
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
                const unitOption = state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.unit?.toString());
                const purchaseCurrencyOption = state.optionss['hr_export_currency'].find(item => item.dictValue === element.purchasecurrency?.toString());
                const invoiceOption = state.optionss['hr_yes_no'].find(item => item.dictValue === element.invoice?.toString());
                const packagingOption = state.optionss['hr_packing'].find(item => item.dictValue === element.packaging?.toString());
                const outerBoxUnitOption = state.optionss['hr_calculate_unit'].find(item => item.dictValue === element.outerboxunit?.toString());
                productData.value.push({
                  id: element.id,
                  productID: element.productID,
                  contractId: element.contractId,
                  productNum: element.productCode,
                  customerNum: element.customerNum,
                  cproductname: element.chineseName,
                  cspecification: element.chineseSpec,
                  contractQuantity: element.contractQuantity,
                  exportunitprice: element.exportUnitPrice,
                  exporttotalprice: element.exportTotalPrice,
                  unitofmeasurement: unitOption?.dictValue || element.unit,
                  unitOfMeasurementLabel: unitOption?.dictLabel || '',
                  purchasecurrency: purchaseCurrencyOption?.dictValue || element.purchasecurrency,
                  purchaseunitprice: element.purchaseUnitPrice,
                  purchasepriceterms: element.purchasingPriceTerms ?? element.purchasePriceTerms ?? element.purchasepriceterms ?? null,
                  inlandfreightprice: element.inlandfreightprice,
                  AdditionalPackagingCosts: element.additionalPackagingCosts,
                  singleProductGrossProfit: element.singleProductGrossProfit,
                  singleProductGrossProfitTotal: element.singleProductGrossProfitTotal,
                  grossProfitRate: element.grossProfitRate,
                  isInvoicingc: element.invoice == 0 ? "2" : (invoiceOption?.dictValue || element.invoice),
                  packaging: packagingOption?.dictValue || element.packaging,
                  specialrequirements: element.specialRequirements,
                  rebaterate: element.taxRefundRate,
                  innerBoxLoading: element.innerBoxQuantity,
                  outerboxloading: element.outerBoxQuantity,
                  outerboxunit: outerBoxUnitOption?.dictValue || element.outerboxunit,
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
                  Inlandfreightforasingleproduct: element.inlandfreightforasingleproduct,
                  isPriceChanged: element.isPriceChanged
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
  } else if (row.documentType == "2" || row.documentType == "10") {//获取采购合同详情（2=采购合同审批 10=采购合同手动完结审批）
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
        PurchaseContractDialogData.value.contractStatus = purchaseContracts.contractStatus ? state.optionss['hr_contract_status'].find(item => item.dictValue === purchaseContracts.contractStatus.toString())?.dictLabel || '未知状态' : '未知状态';
        PurchaseContractDialogData.value.deliveryDate = purchaseContracts.deliveryDate;
        PurchaseContractDialogData.value.vendorCode = purchaseContracts.vendorCode ? state.optionss['sql_supplier_info'].find(item => item.dictValue === purchaseContracts.vendorCode.toString())?.dictLabel || '无' : '无';
        PurchaseContractDialogData.value.purchaseCurrency = purchaseContracts.purchaseCurrency ? state.optionss['hr_export_currency'].find(item => item.dictValue === purchaseContracts.purchaseCurrency.toString())?.dictLabel || '未知货币' : '未知货币';
        PurchaseContractDialogData.value.deposit = purchaseContracts.deposit || '0';
        PurchaseContractDialogData.value.salesperson = purchaseContracts.salesperson ? (state.optionss['sql_hr_sale'].find(item => item.dictValue === purchaseContracts.salesperson.toString()) || { dictLabel: '未知销售员' }).dictLabel : '未知销售员';
        PurchaseContractDialogData.value.purchaser = purchaseContracts.purchaser ? (state.optionss['sql_all_user'].find(item => item.dictValue === purchaseContracts.purchaser.toString()) || { dictLabel: '未知采购员' }).dictLabel : '未知采购员';
        PurchaseContractDialogData.value.priceTerms = purchaseContracts.priceTerms ? (state.optionss['hr_purchase_pricing_term'].find(item => item.dictValue === purchaseContracts.priceTerms.toString()) || { dictLabel: '未知价格条款' }).dictLabel : '未知价格条款';
        PurchaseContractDialogData.value.paymentDays = purchaseContracts.paymentDays ? (state.optionss['hr_purchase_payment_days'].find(item => item.dictValue === purchaseContracts.paymentDays.toString()) || { dictLabel: '未知付款天数' }).dictLabel : '未知付款天数';
        PurchaseContractDialogData.value.salesContract = purchaseContracts.salesContract ? state.optionss['sql_sale_contracts'].find(item => item.dictValue === purchaseContracts.salesContract.toString())?.dictLabel || '未知合同' : '未知合同';
        PurchaseContractDialogData.value.deliveryLocation = purchaseContracts.deliveryLocation || '';
        PurchaseContractDialogData.value.hasDeposit = parseFloat(purchaseContracts.deposit || '0') > 0;
        PurchaseContractDialogData.value.totalValue = purchaseContracts.totalGoodsValue;
        PurchaseContractDialogData.value.totalQuantity = purchaseContracts.totalQuantity;
        PurchaseContractDialogData.value.totalBoxCount = purchaseContracts.totalBoxes;
        PurchaseContractDialogData.value.totalGrossWeight = purchaseContracts.totalGrossWeight;
        PurchaseContractDialogData.value.totalNetWeight = purchaseContracts.totalNetWeight;
        PurchaseContractDialogData.value.totalVolume = purchaseContracts.totalVolume;
        PurchaseContractDialogData.value.TotalPurchases = purchaseContracts.totalPurchases;
        PurchaseContractDialogData.value.TotalTaxRefund = purchaseContracts.totalTaxRefund;
        PurchaseContractDialogData.value.customerExpenseTotal = purchaseContracts.customerExpenseTotal;
        PurchaseContractDialogData.value.appliedPayment = purchaseContracts.appliedPayment;
        PurchaseContractDialogData.value.availablePayment = purchaseContracts.availablePayment;
        PurchaseContractDialogData.value.unpaidAmount = purchaseContracts.unpaidAmount;
        PurchaseContractDialogData.value.paidAmount = purchaseContracts.paidAmount;
        response.data.purchaseContractProducts.forEach(productData => {
          productData.unit = productData.unit ? state.optionss['hr_calculate_unit'].find(item => item.dictValue === productData.unit.toString())?.dictLabel || '无' : '无';
          productData.packaging = productData.packaging ? state.optionss['hr_packing'].find(item => item.dictValue === productData.packaging.toString())?.dictLabel || '无' : '无';
          productData.invoice = productData.invoice == 0 ? "否" : "是";
          if (!productData.supplierID || productData.supplierID.toString() == '0') {
            productData.supplier = '无';
          } else {
            productData.supplier = state.optionss['sql_supplier_info'].find(item => item.dictValue === productData.supplierID.toString())?.dictLabel || '无';
          }
          productData.hasDeposit = productData.hasdeposit == 1 || productData.hasdeposit === true;
          productData.customerCode = productData.customerNumber;
          // 确保字段名与表格显示一致
          if (productData.chineseSpec) {
            productData.chineseSpecification = productData.chineseSpecification;
          }
          if (productData.purchasePrice) {
            productData.purchaseUnitPrice = productData.purchasePrice;
          }
          const rawPurchasePriceTerms = productData.purchasingPriceTerms ?? productData.purchasePriceTerms ?? productData.purchasepriceterms;
          productData.purchasePriceTerms = rawPurchasePriceTerms
            ? (state.optionss['hr_purchase_pricing_term'].find(item =>
              item.dictValue === rawPurchasePriceTerms.toString())?.dictLabel || '无')
            : '无';
        });
        PurchaseContractDialogData.value.productinfotableData = response.data.purchaseContractProducts;
        PurchaseContractDialogData.value.CustomerRelaterExoensesTableData = response.data.purchaseContractVendorExpenses;
        PurchaseContractDialogData.value.CustomerRelaterExoensesTableData.forEach(item => {
          item.amount = item.expense * item.exchangeRate;
          item.currency = item.currency ? state.optionss['hr_export_currency'].find(currency => currency.dictValue == item.currency.toString())?.dictLabel || '无' : '无';
        });
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
        // 基本信息绑定
        PaymentrequestForm.value.applicant = state.optionss['sql_all_user'].find(item => item.dictValue === response.data.paymentRequest.applicant.toString())?.dictLabel || '未知';
        PaymentrequestForm.value.applicationDepartment = state.optionss['sql_hr_dept'].find(item => item.dictValue === response.data.paymentRequest.applicationDepartment.toString())?.dictLabel || '未知';
        PaymentrequestForm.value.applicationDate = formatDate(response.data.paymentRequest.applicationDate);
        PaymentrequestForm.value.applicationNumber = response.data.paymentRequest.applicationNumber;
        PaymentrequestForm.value.currencyCode = state.optionss['hr_currency_code'].find(item => item.dictValue === response.data.paymentRequest.currencyCode.toString())?.dictLabel || '未知';
        PaymentrequestForm.value.totalAmount = response.data.paymentRequest.totalAmount;
        PaymentrequestForm.value.paymentCategory = state.optionss['hr_payment_category'].find(item => item.dictValue === response.data.paymentRequest.paymentCategory.toString())?.dictLabel || '未知';

        // 款项名称处理 - 根据付款类别选择对应的字典
        const paymentCategory = response.data.paymentRequest.paymentCategory.toString();
        const paymentName = response.data.paymentRequest.paymentName.toString();

        switch (paymentCategory) {
          case '1':
            PaymentrequestForm.value.paymentName = state.optionss['hr_factory_payment'].find(item => item.dictValue === paymentName)?.dictLabel || '未知';
            break;
          case '2':
            PaymentrequestForm.value.paymentName = state.optionss['hr_domestic_charges'].find(item => item.dictValue === paymentName)?.dictLabel || '未知';
            break;
          case '3':
            PaymentrequestForm.value.paymentName = state.optionss['hr_foreign_charges'].find(item => item.dictValue === paymentName)?.dictLabel || '未知';
            break;
          case '4':
            PaymentrequestForm.value.paymentName = state.optionss['hr_daily_expenses'].find(item => item.dictValue === paymentName)?.dictLabel || '未知';
            break;
          case '5':
            PaymentrequestForm.value.paymentName = state.optionss['hr_business_expenses'].find(item => item.dictValue === paymentName)?.dictLabel || '未知';
            break;
          default:
            PaymentrequestForm.value.paymentName = '未知';
            break;
        }

        // 收款单位处理 - 根据付款类别和款项名称确定收款单位类型
        const payeeCode = response.data.paymentRequest.payeeCode.toString();
        let payeeLabel = '无';

        // 先尝试从供应商信息中查找
        const supplierInfo = state.optionss['sql_supplier_info'].find(item => item.dictValue === payeeCode);
        if (supplierInfo) {
          payeeLabel = supplierInfo.dictLabel;
        } else {
          // 如果是业务费用，可能需要从物流公司等特殊类型中查找
          if (paymentCategory === '5') {
            // 业务费用可能需要从物流公司等特殊类型中查找
            // 这里可以根据实际需要扩展其他类型的查找逻辑
            payeeLabel = '特殊收款单位';
          }
        }

        PaymentrequestForm.value.payeeCode = payeeLabel;
        PaymentrequestForm.value.payeeName = response.data.paymentRequest.payeeName || '未知';
        PaymentrequestForm.value.bankName = response.data.paymentRequest.bankName || '未知';
        PaymentrequestForm.value.bankAccount = response.data.paymentRequest.bankAccount || '未知';
        PaymentrequestForm.value.ourCompany = state.optionss['hr_ourcompany'].find(item => item.dictValue === response.data.paymentRequest.ourCompany.toString())?.dictLabel || '未知';
        PaymentrequestForm.value.paidAmount = response.data.paymentRequest.paidAmount || '0';
        PaymentrequestForm.value.unpaidAmount = response.data.paymentRequest.unpaidAmount || '0';
        PaymentrequestForm.value.handler = state.optionss['sql_all_user'].find(item => item.dictValue === response.data.paymentRequest.handler.toString())?.dictLabel || '未知';
        PaymentrequestForm.value.remarks = response.data.paymentRequest.remarks || '';
        PaymentrequestForm.value.relatedContract = response.data.paymentRequest.relatedContracts || '';

        // 费用明细处理
        CostDetailsTbaleData.value = [];
        if (response.data.paymentRequestDetails && response.data.paymentRequestDetails.length > 0) {
          response.data.paymentRequestDetails.forEach(item => {
            CostDetailsTbaleData.value.push(item);
          });

          // 处理费用明细的显示标签
          var OrderType = [];
          CostDetailsTbaleData.value.forEach(costDetail => {
            // 关联模块处理
            switch (costDetail.relatedModules?.toString()) {
              case '1':
                OrderType = state.optionss['sql_purchase_contract'];
                break;
              case '2':
                OrderType = state.optionss['sql_sale_contracts'];
                break;
              default:
                costDetail.associatedOrderNumber = '无';
                break;
            }

            if (OrderType.length > 0 && costDetail.associatedOrderNumber) {
              const orderInfo = OrderType.find(item => item.dictValue === costDetail.associatedOrderNumber.toString());
              costDetail.associatedOrderNumber = orderInfo?.dictLabel || '无';
            }

            // 具体付款项目处理
            switch (paymentCategory) {
              case '1':
                costDetail.specificPaymentItems = state.optionss['hr_factory_payment'].find(item => item.dictValue === costDetail.specificPaymentItems?.toString())?.dictLabel || '未知';
                break;
              case '2':
                costDetail.specificPaymentItems = state.optionss['hr_domestic_charges'].find(item => item.dictValue === costDetail.specificPaymentItems?.toString())?.dictLabel || '未知';
                break;
              case '3':
                costDetail.specificPaymentItems = state.optionss['hr_foreign_charges'].find(item => item.dictValue === costDetail.specificPaymentItems?.toString())?.dictLabel || '未知';
                break;
              case '4':
                costDetail.specificPaymentItems = state.optionss['hr_daily_expenses'].find(item => item.dictValue === costDetail.specificPaymentItems?.toString())?.dictLabel || '未知';
                break;
              case '5':
                costDetail.specificPaymentItems = state.optionss['hr_business_expenses'].find(item => item.dictValue === costDetail.specificPaymentItems?.toString())?.dictLabel || '未知';
                break;
            }

            // 关联模块标签
            costDetail.relatedModules = state.optionss['hr_associated_modules'].find(item => item.dictValue === costDetail.relatedModules?.toString())?.dictLabel || '未知';
          });
        }

        PaymentrequestDialog.value = true;
      }
    }).catch(error => {
      console.error('获取付款申请详情失败:', error);
      ElMessage.error('获取付款申请详情失败，请重试');
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
      const shipping = response.data.shippingDeliveries;
      ShippingDeliveryForm.value.invoiceNumber = shipping.invoiceNumber;
      ShippingDeliveryForm.value.OrderMakingDate = shipping.orderMakingDate || shipping.createTime;
      ShippingDeliveryForm.value.shippingStatus = state.optionss['hr_shipping_status']
        .find(item => item.dictValue === shipping.shippingStatus?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.shippingDate = shipping.shippingDate;
      ShippingDeliveryForm.value.invoiceDate = shipping.invoiceDate;
      ShippingDeliveryForm.value.customerNumber = state.optionss['sql_hr_customer']
        .find(item => item.dictValue === shipping.customerNumber?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.customerAbbreviation = shipping.customerAbbreviation || '';
      ShippingDeliveryForm.value.referenceContractNumber = shipping.referenceContractNumber
        ? (state.optionss['sql_sale_contracts'].find(item => item.dictValue === shipping.referenceContractNumber.toString())?.dictLabel || '')
        : '';
      ShippingDeliveryForm.value.salesContractNumber = shipping.salesContractNumber?.toString() || '';
      ShippingDeliveryForm.value.customerContractNumber = shipping.customerContractNumber?.toString() || '';
      ShippingDeliveryForm.value.ourCompany = state.optionss['hr_ourcompany']
        .find(item => item.dictValue === shipping.ourCompany?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.bankOfReceipt = state.optionss['hr_bank']
        .find(item => item.dictValue === shipping.bankOfReceipt?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.exportCurrency = state.optionss['hr_export_currency']
        .find(item => item.dictValue === shipping.exportCurrency?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.exchangeRate = shipping.exchangeRate;
      ShippingDeliveryForm.value.priceTerms = state.optionss['hr_pricing_term']
        .find(item => item.dictValue === shipping.priceTerms?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.departurePort = state.optionss['hr_transport_port']
        .find(item => item.dictValue === shipping.departurePort?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.destinationPort = shipping.destinationPort;
      ShippingDeliveryForm.value.transportationMethod = state.optionss['hr_transportation_method']
        .find(item => item.dictValue === shipping.transportationMethod?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.tradeCountry = state.optionss['hr_nation']
        .find(item => item.dictValue === shipping.tradeCountry?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.settlementMethod = state.optionss['hr_settlement_way']
        .find(item => item.dictValue === shipping.settlementMethod?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.receivableDate = shipping.receivableDate;
      ShippingDeliveryForm.value.documentClerk = state.optionss['sql_all_user']
        .find(item => item.dictValue === shipping.documentClerk?.toString())?.dictLabel || '';
      ShippingDeliveryForm.value.isDeposit = shipping.isDeposit?.toString();
      // 运输相关字典映射（显示为名称）
      ShippingDeliveryForm.value.preCarriageTransport = state.optionss['hr_domestic_transport']
        .find(item => item.dictValue === (shipping.preCarriageTransport ?? '').toString())?.dictLabel || '';
      ShippingDeliveryForm.value.shippingAgent = state.optionss['hr_freight_forwarders']
        .find(item => item.dictValue === (shipping.shippingAgent ?? '').toString())?.dictLabel || '';
      ShippingDeliveryForm.value.courierCompaniesID = state.optionss['hr_courier_companies']
        .find(item => item.dictValue === (shipping.courierCompaniesID ?? '').toString())?.dictLabel || '';
      ShippingDeliveryForm.value.logisticsCompanyID = state.optionss['hr_logistics_companies']
        .find(item => item.dictValue === (shipping.logisticsCompanyID ?? '').toString())?.dictLabel || '';
      ShippingDeliveryForm.value.remark = shipping.remark;

      if (response.data.shippingDeliveryProducts.length > 0) {
        shippingDeliveryContrctProductTableData.value = response.data.shippingDeliveryProducts;
        shippingDeliveryContrctProductTableData.value.forEach(item => {
          request({
            url: 'Contracts/GetCustomerContractProductByCPID/GetCustomerContractProduct',
            method: 'GET',
            params: {
              CPID: item.contractProductId
            }
          }).then(resp => {
            if (resp.data != null && resp.data.length > 0) {
              const index = shippingDeliveryContrctProductTableData.value.findIndex(
                x => x.contractProductId === item.contractProductId
              );
              if (index !== -1) {
                const productData = resp.data[0];
                const existingRow = shippingDeliveryContrctProductTableData.value[index];
                // 与 shippingdelivery.vue 一致：箱规从出运单详情保留，仅用合同产品补全品名/单位等
                shippingDeliveryContrctProductTableData.value[index] = {
                  ...existingRow,
                  contractNumber: productData.contractNumber,
                  productCode: productData.productCode,
                  chineseName: productData.chineseName,
                  chineseSpec: productData.chineseSpec ?? '',
                  contractQuantity: productData.contractQuantity,
                  unit: state.optionss.hr_calculate_unit.find(u => u.dictValue === productData.unit?.toString())?.dictLabel || '无',
                  exportUnitPrice: productData.exportUnitPrice,
                  exportTotalPrice: productData.exportTotalPrice,
                  specialRequirements: productData.specialRequirements || ''
                  // 箱规(outerBoxQuantity/innerBoxQuantity/boxCount/outerBoxLength 等)保留 existingRow 即出运单详情返回
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
              CustomerReceiptsData.value = GetCustomerControllerDataresponse.data.map(item => {
                // 根据我方公司选择对应的收汇银行数据源
                let bankOptions = [];
                const ourCompanyLabel = state.optionss["hr_ourcompany"].find(oc => oc.dictValue == item.ourCompany.toString())?.dictLabel || '';
                if (ourCompanyLabel === '荣发塑料') {
                  bankOptions = state.optionss.hr_rf_receiving_bank || [];
                } else if (ourCompanyLabel === '惠荣进出口') {
                  bankOptions = state.optionss.hr_receiving_bank || [];
                }

                return {
                  ReceiptNumber: item.receiptNumber,
                  ReceiptDate: item.receiptDate ? item.receiptDate.split(' ')[0] : '',
                  ReceiptAmount: item.amount,
                  ReceiptCurrency: state.optionss["hr_export_currency"].find(cc => cc.dictValue == item.foreignCurrency.toString())?.dictLabel || '',
                  ExchangeRate: item.exchangeRate,
                  ReceiptBank: bankOptions.find(bank => bank.dictValue == item.bank.toString())?.dictLabel || '',
                  OurCompany: ourCompanyLabel,
                  FundsType: state.optionss["funds_type"].find(ft => ft.dictValue == item.fundsType.toString())?.dictLabel || '',
                }
              })
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
    hr_business_expenses: [],
    hr_associated_modules: [],
    sql_shippingdeliveries: [],
    hr_shipping_status: [],
    hr_domestic_transport: [],
    hr_freight_forwarding_company: [],
    hr_freight_forwarders: [],
    hr_courier_companies: [],
    hr_logistics_companies: [],
    sql_product: [],
    sql_settlement: [],
    hr_payment_contract_type: [],
    hr_funds_classification: [],
    sql_user_customers: [], // 用户特定的客户数据
    hr_receiving_bank: [],//惠荣收汇银行
    hr_rf_receiving_bank: [],//荣发塑料收汇银行
    hr_currency_code: [],//付款申请单币种字典
  }
})
const { optionss } = toRefs(state)
// 独立加载 货代/快递/物流 公司下拉（首页）
const loadLogisticsCompanySelectsForIndex = async () => {
  try {
    const [freightRes, courierRes, logisticsRes] = await Promise.all([
      request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 1 } }),
      request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 2 } }),
      request({ url: 'LogisticsCompany/GetSelectList/GetLogisticsCompanySelect', method: 'get', params: { companyType: 3 } })
    ])

    state.optionss.hr_freight_forwarders = (freightRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
    state.optionss.hr_courier_companies = (courierRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
    state.optionss.hr_logistics_companies = (logisticsRes.data || []).map(x => ({ dictValue: String(x.dictValue), dictLabel: x.dictLabel }))
  } catch (e) {
    console.error('加载物流公司下拉失败(index):', e)
  }
}
//从后台读取多个字典数据
var dictParams = [
  { dictType: 'sql_all_user' },
  { dictType: 'hr_approval_document_type' },
  { dictType: 'hr_business_documents' },
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
  { dictType: 'hr_purchase_pricing_term' },
  { dictType: 'hr_purchase_payment_days' },
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
  { dictType: 'hr_business_expenses' },
  { dictType: 'hr_associated_modules' },
  { dictType: 'sql_shippingdeliveries' },
  { dictType: 'hr_shipping_status' },
  { dictType: 'hr_domestic_transport' },
  { dictType: 'hr_freight_forwarding_company' },
  { dictType: 'sql_product' },
  { dictType: 'sql_settlement' },
  { dictType: 'hr_payment_contract_type' },
  { dictType: 'hr_funds_classification' },
  { dictType: 'hr_collection_associated_modules' },
  { dictType: 'hr_receiving_bank' },
  { dictType: 'hr_rf_receiving_bank' },
  { dictType: 'hr_currency_code' }
]
// 字典数据加载将在 onMounted 中处理

// 获取用户相关的客户数据
const getUserCustomerData = async () => {
  try {
    const response = await request({
      url: 'CustomerInfoMation/GetCustomerDataByUserID/GetSelectCustomerDataByUserID',
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
    console.error('获取客户数据失败:', error)
    ElMessage.error('获取客户数据失败')
  }
}


// 统一的pendingCount计算函数
const calculatePendingCount = async () => {
  let totalCount = 0;

  try {
    // 1. 获取待审批单据数量
    const approvalResponse = await request({
      url: 'ApprovalFlow/GetApprovalRecord/GetApprovalRecordCount',
      method: 'GET'
    });
    if (approvalResponse.code == 200) {
      totalCount += approvalResponse.data.length;
    }

    // 2. 获取询价已回复数量
    const inquiryRepliedResponse = await request({
      url: 'Inquiry/GetInquiryListByInquirer/GetInquiryList',
      method: 'get'
    });
    if (inquiryRepliedResponse.code === 200) {
      const repliedList = Array.isArray(inquiryRepliedResponse.data)
        ? inquiryRepliedResponse.data
        : (inquiryRepliedResponse.data?.result || []);
      totalCount += repliedList.length;
    }

    // 3. 获取询价需求数量
    if (userStore.userInfo && userStore.userInfo.deptId === 210) {
      const inquiryResponse = await request({
        url: 'Inquiry/GetInquiryProductListByBuyer/GetInquiryProductList',
        method: 'get',
        params: {
          PageNum: 1,
          PageSize: 10,
          Status: 0
        }
      });
      if (inquiryResponse.code === 200 && inquiryResponse.data != 0) {
        totalCount += inquiryResponse.data.result.length;
      }

      // 4. 获取采购需求数量（同一个合同号只算一条记录）
      const procurementResponse = await request({
        url: 'PurchaseContracts/GetProcurementRequirements/GetList',
        method: 'get',
        params: {
          PageNum: 1,
          PageSize: 10,
          Status: 0
        }
      });
      if (procurementResponse.code === 200 && procurementResponse.data.result) {
        // 对合同号进行去重，同一个合同号只算一条记录
        const uniqueContractNumbers = new Set(
          procurementResponse.data.result.map(item => item.contractNumber).filter(Boolean)
        );
        totalCount += uniqueContractNumbers.size;
      }

      // 5. 获取被驳回的采购合同数量
      const rejectPurchaseResponse = await request({
        url: 'PurchaseContracts/GetRejectPurchaseContractList/GetRejectList',
        method: 'get'
      });
      if (rejectPurchaseResponse.code == 200) {
        totalCount += rejectPurchaseResponse.data.length;
      }
    }

    // 6. 获取被驳回的销售合同数量
    const rejectContractResponse = await request({
      url: 'Contracts/GetRejectContractsList/GetRejectList',
      method: 'get'
    });
    if (rejectContractResponse.code == 200) {
      totalCount += rejectContractResponse.data.length;
    }

    // 7. 获取采购价格变更合同数量
    const priceChangeList = await getPurchasePriceChangeContracts()
    if (priceChangeList && priceChangeList.length > 0) {
      totalCount += priceChangeList.length
    }

    // 8. 获取付款任务数量（仅部门ID为213时）
    if (userStore.userInfo && userStore.userInfo.deptId === 213) {
      const paymentTaskResponse = await getPaymentTaskList();
      if (paymentTaskResponse && paymentTaskResponse.length > 0) {
        // 接口返回的是ReviewStatus == 2的待付款申请，都算作待处理任务
        totalCount += paymentTaskResponse.length;
      }
    }

    // 9. 获取收款单领取数量（业务员可见）
    if (isSalesRole()) {
      const receiptClaimResponse = await request({
        url: 'CustomerCollections/GetUnassignedCustomerCollectionsList/GetList',
        method: 'GET',
        params: {
          PageNum: 1,
          PageSize: 1
        }
      });
      if (receiptClaimResponse?.data?.totalNum) {
        totalCount += receiptClaimResponse.data.totalNum;
      }
    }

    // 10. 获取收款单确认数量（财务可见）
    if (isFinanceRole()) {
      const receiptConfirmCount = await getReceiptConfirmTotalCount();
      if (receiptConfirmCount > 0) {
        totalCount += receiptConfirmCount;
      }
    }

    // 更新pendingCount
    pendingCount.value = totalCount;

  } catch (error) {
    console.error('计算pendingCount失败:', error);
  }
};

//获取待办流程数量
const getPendingCount = () => {
  AgencyProcesstableData.value = [];
  request({
    url: 'ApprovalFlow/GetApprovalRecord/GetApprovalRecordCount',
    method: 'GET'
  }).then(response => {
    if (response.code == 200) {
      if (response.data.length > 0) {
        AgencyProcesstableData.value = response.data;
        AgencyProcesstableData.value.forEach(item => {
          item.createBy = state.optionss['sql_all_user'].filter(user => user.dictValue == item.createBy).map(user => user.dictLabel).values().next().value;
          item.documentTypeName = state.optionss['hr_approval_document_type'].filter(user => user.dictValue == item.documentType).map(user => user.dictLabel).values().next().value;
          if (item.documentType == "1" || item.documentType == "7" || item.documentType == "9") {
            item.documentNumber = state.optionss['sql_sale_contracts'].filter(Salecontract => Salecontract.dictValue == item.documentID.toString()).map(Salecontract => Salecontract.dictLabel).values().next().value;
          } else if (item.documentType == "2" || item.documentType == "10") {
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
    // 处理完待审批单据后，重新计算总的pendingCount
    calculatePendingCount();
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

// 计算属性：根据我方公司动态获取收汇银行选项
const receivingBankOptions = computed(() => {
  const selectedCompany = addcustomercollectionform.value.ourCompany;
  // 如果我方公司为空或未选择，返回空数组
  if (!selectedCompany || selectedCompany === '') {
    return [];
  }
  // 根据我方公司的值返回对应的收汇银行数据源
  if (selectedCompany === '1') {
    // 荣发塑料，使用 hr_rf_receiving_bank
    return optionss.value.hr_rf_receiving_bank || [];
  } else if (selectedCompany === '2') {
    // 惠荣进出口，使用 hr_receiving_bank
    return optionss.value.hr_receiving_bank || [];
  }
  return [];
});

// 计算属性：判断是否显示关联合同字段
const shouldShowRelatedContract = computed(() => {
  // 检查是否为业务费用类别
  const paymentCategory = PaymentrequestForm.value.paymentCategory;
  if (!paymentCategory || paymentCategory !== '业务费用') {
    return false;
  }

  // 检查款项名称是否为"其它"
  const paymentName = PaymentrequestForm.value.paymentName;
  if (!paymentName) {
    return false;
  }

  // 判断是否为其它款项名称
  return paymentName.includes('其它') || paymentName.includes('其他');
});


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
  remarks: '',
  relatedContract: '' // 关联合同字段
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
  courierCompaniesID: '',
  logisticsCompanyID: '',
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
      // 处理新的数据结构：data是包含date和items的数组
      if (Array.isArray(response.data)) {
        // 将嵌套结构扁平化为任务数组
        planTaskItems.value = response.data.reduce((acc, dateGroup) => {
          if (dateGroup.items && Array.isArray(dateGroup.items)) {
            return acc.concat(dateGroup.items)
          }
          return acc
        }, [])
      } else {
        planTaskItems.value = []
      }
      console.log('处理后的计划任务数据:', planTaskItems.value)
      // 重新生成日历数据以显示任务
      generateCalendarDays()
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
  // 从真实的计划任务数据中筛选指定日期的任务
  const tasks = planTaskItems.value.filter(task => {
    if (!task.timePoint) return false;

    // 直接解析时间字符串，避免时区问题
    const taskDate = task.timePoint.split(' ')[0]; // 只取日期部分 "2025-08-24"
    const matches = taskDate === dateString;

    // 调试信息
    if (taskDate === '2025-08-24' || dateString === '2025-08-24') {
      console.log('任务日期匹配:', {
        taskDate,
        dateString,
        matches,
        taskName: task.itemName
      });
    }

    return matches;
  });

  return tasks;
}

// 格式化日期时间
const formatDateTime = (dateTimeString) => {
  const date = new Date(dateTimeString)
  if (isNaN(date.getTime())) {
    return ''
  }
  return `${formatDate(date)} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
}

// 判断任务是否过期
const isOverdue = (timePoint) => {
  if (!timePoint) return false;

  // 解析时间字符串，避免时区问题
  const [dateStr, timeStr] = timePoint.split(' ');
  const [year, month, day] = dateStr.split('-').map(Number);
  const [hour, minute, second] = timeStr ? timeStr.split(':').map(Number) : [0, 0, 0];

  const taskDate = new Date(year, month - 1, day, hour, minute, second);
  const now = new Date();

  return taskDate < now;
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
        // 触发工作任务更新事件
        eventBus.emit('updateWorkTasks')
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
          // 触发业务示警更新事件
          eventBus.emit('updateBusinessAlerts')
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

// 处理合同号点击事件
const handleContractNumberClick = (row) => {
  // 跳转到销售合同页面，传递合同ID参数
  router.push({
    path: '/sale/sale/salecontract',
    query: {
      contractId: row.id,
      contractNumber: row.contractNumber,
      viewDetail: 'true'
    }
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
          // 触发业务示警更新事件
          eventBus.emit('updateBusinessAlerts')
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
const pendingTaskPlanItemList = ref([])
const workPendingTotalCount = computed(() => {
  const workCount = Number(pendingEmailCount.value) || 0
  const deleteCount = isAdminRole() ? (Number(documentDeleteRequestTotalItems.value) || 0) : 0
  return workCount + deleteCount
})
console.log(userStore.userInfo);
// 获取代处理的计划任务列表
const GetPlantTaskItemList = async () => {
  const res = await request({
    url: 'PlanTasks/GetPlanTask_Items/GetPlanTask_Items',
    method: 'GET'
  })
  if (res.code === 200) {
    pendingEmailCount.value += res.data.length;
    res.data.forEach(item => {
      pendingTaskPlanItemList.value.push({
        taskId: item.taskId,
        itemId: item.itemId,
        taskName: item.taskName,
        phaseName: item.phaseName,
        itemName: item.itemName,
        timePoint: item.timePoint
      })
    })
  }
}
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
      pendingEmailCount.value += response.data.length
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
      overdueEmailCount.value += response.data.length
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
  // 监听日历更新事件
  eventBus.on('updatePlanTaskItems', () => {
    console.log('收到日历更新事件');
    // 刷新当前日历数据
    const startDate = formatDate(calendarDates.value[0].date)
    const endDate = formatDate(calendarDates.value[calendarDates.value.length - 1].date)
    getPlanTaskItems(startDate, endDate);
  });
  // 监听业务示警更新事件
  eventBus.on('updateBusinessAlerts', () => {
    console.log('收到业务示警更新事件');
    GetOverdueDeliveryContractCount();
    GetCommunicationOverdueCount();
    getOverdueContracts();
  });
  // 监听工作任务更新事件
  eventBus.on('updateWorkTasks', () => {
    console.log('收到工作任务更新事件');
    getWithin24hoursEmailCount();
    getOutside24hoursEmailCount();
    getOverduePendingTaskPlanItemList();
    if (isAdminRole()) {
      getDocumentDeleteRequestList();
    }
  });
  try {
    // 首先加载字典数据
    const dictResponse = await proxy.getDicts(dictParams);
    dictResponse.data.forEach((element) => {
      state.optionss[element.dictType] = element.list
    })
    console.log('字典数据加载完成:', state.optionss.hr_export_currency)

    const dataPromises = [
      GetPlantTaskItemList(),
      getWithin24hoursEmailCount(),
      getOutside24hoursEmailCount(),
      getOverduePendingTaskPlanItemList(),
      fetchTaskReminderData(),
      getUnreadMessages(),
      // 关联单号选项数据会在打开领取对话框时按需获取
      getPendingCount(),
      getFinancialTasksList(1, 10),
      loadReceiptConfirmList(),
      getUserCustomerData()
    ];

    if (isSalesDeptUser()) {
      dataPromises.push(
        loadCustomerStatisticsHomeData(),
        loadSalesConversionStatisticsHomeData(),
        loadBusinessSummaryHomeData(),
        fetchDashboardData()
      );
    }

    await Promise.all(dataPromises);
    // 加载货代/快递/物流公司下拉
    await loadLogisticsCompanySelectsForIndex();

    // 字典数据加载完成后，初始化汇率填写通知
    initExchangeRateNotification();
  } catch (error) {
    console.error('数据加载失败:', error)
    ElMessage.error('数据加载失败，请刷新页面重试')
  }
})
// 组件卸载时清理事件监听
onUnmounted(() => {
  eventBus.off('updatePendingCount');
  eventBus.off('updatePlanTaskItems');
  eventBus.off('updateBusinessAlerts');
  eventBus.off('updateWorkTasks');
  displayedMessages.clear();
  // 关闭所有通知
  ElNotification.closeAll();
});

const showPendingEmails = async () => {
  if (workPendingTotalCount.value === 0) {
    ElMessage.warning('没有待处理工作任务')
    return;
  }
  if (pendingEmailCount.value > 0) {
    await getWithin24hoursEmailCount();
    pendingEmailDialogVisible.value = true
    return
  }
  if (isAdminRole() && documentDeleteRequestTotalItems.value > 0) {
    tasksActiveTab.value = 'documentDeleteRequest'
    ElMessage.info('已切换到待删除单据')
  }
}

const showOverdueEmails = async () => {
  if (overdueEmailCount.value === 0) {
    ElMessage.warning('没有超时未处理工作任务')
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

// 处理行双击事件
const handleRowDblClick = (row) => {
  // 打开标签页到 Plantask.vue
  router.push({
    path: '/plantask',
    query: {
      taskId: row.taskId
    }
  })
}

// 询价列表数据
const inquiryList = ref([])
// 询价已回复列表数据
const inquiryRepliedList = ref([])
const inquiryRepliedCount = computed(() => inquiryRepliedList.value?.length || 0)

// 被驳回的采购合同列表数据
const rejectPurchaseContractList = ref([])

// 被驳回的销售合同列表数据
const rejectContractList = ref([])

// 付款任务列表数据
const paymentTaskList = ref([])

// 采购价格变更合同列表数据
const purchasePriceChangeList = ref([])
// 待删除单据列表数据（管理员）
const documentDeleteRequestList = ref([])
const documentDeleteRequestTotalItems = ref(0)
const documentDeleteRequestCurrentPage = ref(1)
const documentDeleteRequestPageSize = ref(10)

const getDeleteRequestDocumentTypeLabel = (documentType) => {
  if (documentType === null || documentType === undefined || documentType === '') {
    return '—'
  }
  const label = state.optionss['hr_business_documents']
    ?.find(item => item.dictValue == documentType)?.dictLabel
  return label || String(documentType)
}

const getDeleteRequestApplyUserLabel = (applyUser) => {
  if (applyUser === null || applyUser === undefined || applyUser === '') {
    return '—'
  }
  const label = state.optionss['sql_all_user']
    ?.find(item => item.dictValue == applyUser)?.dictLabel
  return label || String(applyUser)
}

const getDeleteRequestApproveStatusLabel = (approveStatus) => {
  const statusMap = {
    0: '待审批',
    1: '通过',
    2: '驳回'
  }
  return statusMap[approveStatus] ?? String(approveStatus ?? '—')
}

const getDeleteRequestApproveStatusType = (approveStatus) => {
  const typeMap = {
    0: 'warning',
    1: 'success',
    2: 'danger'
  }
  return typeMap[approveStatus] ?? 'info'
}

const getDocumentDeleteRequestList = async (showError = false) => {
  try {
    const response = await request({
      url: 'ApplyDeleteDocument/GetDocumentDeleteRequestList/GetList',
      method: 'GET',
      params: {
        PageNum: documentDeleteRequestCurrentPage.value,
        PageSize: documentDeleteRequestPageSize.value
      }
    })
    if (response.code === 200) {
      const list = response.data?.result ?? response.data ?? []
      documentDeleteRequestList.value = Array.isArray(list) ? list : []
      documentDeleteRequestTotalItems.value = Number(
        response.data?.total ?? response.data?.totalNum ?? response.data?.totalCount ?? 0
      )
      documentDeleteRequestCurrentPage.value = Number(
        response.data?.pageIndex ?? documentDeleteRequestCurrentPage.value
      )
      documentDeleteRequestPageSize.value = Number(
        response.data?.pageSize ?? documentDeleteRequestPageSize.value
      )
      return documentDeleteRequestList.value
    }
    documentDeleteRequestList.value = []
    documentDeleteRequestTotalItems.value = 0
    if (showError) {
      ElMessage.error('获取待删除单据失败')
    }
  } catch (error) {
    console.error('获取待删除单据失败:', error)
    documentDeleteRequestList.value = []
    documentDeleteRequestTotalItems.value = 0
    if (showError) {
      ElMessage.error('获取待删除单据失败，请稍后重试')
    }
  }
  return []
}

const documentDeleteRequestHandlePageChange = (newPage) => {
  documentDeleteRequestCurrentPage.value = newPage
  getDocumentDeleteRequestList(true)
}

// 带读秒的二次确认（15秒后可点击确认删除）
const confirmDeleteWithCountdown = (message) => {
  return new Promise((resolve, reject) => {
    let countdown = 15
    let timer = 0
    ElMessageBox.confirm(message, '二次确认', {
      confirmButtonText: `确认删除(${countdown}s)`,
      cancelButtonText: '取消',
      type: 'warning',
      distinguishCancelAndClose: true,
      closeOnClickModal: false,
      closeOnPressEscape: false
    }).then(() => {
      window.clearInterval(timer)
      resolve(true)
    }).catch((action) => {
      window.clearInterval(timer)
      reject(action)
    })

    setTimeout(() => {
      const boxes = document.querySelectorAll('.el-message-box')
      const currentBox = boxes[boxes.length - 1]
      const confirmBtn = currentBox?.querySelector('.el-message-box__btns .el-button--primary')
      if (!confirmBtn) return

      confirmBtn.disabled = true
      confirmBtn.setAttribute('disabled', 'disabled')
      confirmBtn.classList.add('is-disabled')
      timer = window.setInterval(() => {
        if (!confirmBtn.isConnected) {
          window.clearInterval(timer)
          return
        }
        countdown -= 1
        if (countdown > 0) {
          confirmBtn.textContent = `确认删除(${countdown}s)`
          return
        }
        window.clearInterval(timer)
        confirmBtn.disabled = false
        confirmBtn.removeAttribute('disabled')
        confirmBtn.classList.remove('is-disabled')
        confirmBtn.textContent = '确认删除'
      }, 1000)
    }, 0)
  })
}

// 删除单据删除申请（逻辑删除）
const handleDelDocumentRequest = (row) => {
  confirmDeleteWithCountdown('确定要删除该单据删除申请吗？删除后将逻辑删除对应单据。').then(() => {
    request.post('ApplyDeleteDocument/DelDocumentRequest/DelDocument', {
      DocumentID: row.sourceId,
      DocumentType: row.documentType
    }).then(response => {
      if (response != null) {
        ElMessage({
          message: response.msg || '单据删除成功',
          type: response.code === 200 ? 'success' : 'error'
        });
        if (response.code === 200) {
          getDocumentDeleteRequestList(true)
        }
      } else {
        ElMessage({ message: '单据删除失败', type: 'error' });
      }
    }).catch(error => {
      console.error('删除单据删除申请出错', error);
      ElMessage({ message: '删除失败', type: 'error' });
    });
  }).catch(() => {
    ElMessage.info('已取消删除');
  });
}

// 获取询价列表
const getInquiryList = async () => {
  try {
    const res = await request({
      url: 'Inquiry/GetInquiryProductListByBuyer/GetInquiryProductList',
      method: 'get',
      params: {
        PageNum: 1,
        PageSize: 10,
        Status: 0
      }
    })
    if (res.code === 200) {
      inquiryList.value = res.data.result
      // 不再累加pendingCount，由统一计算函数处理
    }
  } catch (error) {
    console.error('获取询价列表失败', error)
  }
}

// 获取询价已回复列表
const getInquiryRepliedList = async () => {
  try {
    const res = await request({
      url: 'Inquiry/GetInquiryListByInquirer/GetInquiryList',
      method: 'get',
    })
    if (res.code === 200) {
      inquiryRepliedList.value = Array.isArray(res.data)
        ? res.data
        : (res.data?.result || [])
    }
  } catch (error) {
    console.error('获取询价已回复列表失败', error)
  }
}

// 获取采购价格变更合同列表
const getPurchasePriceChangeContracts = async (showError = false) => {
  try {
    const response = await request({
      url: 'Contracts/GetContractListByPurchaseUnitPrice/GetContractList',
      method: 'GET'
    })
    if (response.code === 200) {
      const list = Array.isArray(response.data)
        ? response.data
        : (response.data?.result || [])
      purchasePriceChangeList.value = list
      return list
    }
    purchasePriceChangeList.value = []
    if (showError) {
      ElMessage.error('获取采购价格变更合同失败')
    }
  } catch (error) {
    console.error('获取采购价格变更合同失败:', error)
    purchasePriceChangeList.value = []
    if (showError) {
      ElMessage.error('获取采购价格变更合同失败，请稍后重试')
    }
  }
  return []
}

// 处理询价需求表格的双击事件
const handleInquiryRowDblClick = (row) => {
  // 使用路由导航到requestquote页面，并通过query参数传递数据
  const inquiryId = row?.inquiryID ?? row?.inquiryId ?? row?.id ?? row?.Id
  if (!inquiryId) {
    ElMessage.warning('无法获取询价单ID')
    return
  }
  router.push({
    path: '/purchase/requestquote',
    query: {
      inquiryId: inquiryId,
      fromDashboard: 'true'
    }
  })
}

// 处理询价已回复表格的双击事件
const handleInquiryRepliedRowDblClick = (row) => {
  const inquiryId = row?.id ?? row?.Id ?? row?.inquiryID ?? row?.inquiryId
  if (!inquiryId) {
    ElMessage.warning('无法获取询价单ID')
    return
  }
  router.push({
    path: '/sale/productinquiry',
    query: {
      inquiryId: inquiryId,
      fromDashboard: 'true'
    }
  })
}

const shoppinglisttableData = ref([])

const GetProcurementequirements = async () => {
  try {
    const response = await request({
      url: 'PurchaseContracts/GetProcurementRequirements/GetList',
      method: 'GET',
      params: {
        PageNum: 1,
        PageSize: 10
      }
    })
    // 检查响应数据
    if (!response.data) {
      throw new Error('返回数据格式错误')
    }
    // 清空现有数据
    shoppinglisttableData.value = []
    // 如果有数据则处理
    if (response.data.result && response.data.result.length > 0) {
      shoppinglisttableData.value = response.data.result.map(element => ({
        contractId: element.contractId,
        productId: element.productId,
        procurementId: element.procurementId,
        contractNumber: element.contractNumber,
        productName: element.productName,
        quantity: element.quantity,
        unitPrice: element.unitPrice,
        totalPrice: element.totalPrice,
        status: element.status,
        salesperson: state.optionss.sql_hr_sale.find(item => item.dictValue === element.salesperson.toString())?.dictLabel || '未知',
        createTime: formatDate(element.createTime),
        remark: element.remark
      }))
    }
  } catch (error) {
    console.error('获取采购需求列表失败:', error)
    ElMessage.error('获取数据失败，请重试')
    throw error
  }
  // 不再累加pendingCount，由统一计算函数处理
}

// 动态合并单元格
const mergeCells = ({ row, columnIndex, rowIndex }) => {
  // 合并销售合同号列（第1列）
  if (columnIndex === 0) {
    if (rowIndex === getFirstRowIndex(row.contractNumber)) {
      const rowspan = getRowSpan(row.contractNumber);
      return [rowspan, 1];
    } else {
      return [0, 0];
    }
  }
};

// 获取当前合同号的第一行索引
const getFirstRowIndex = (contractNumber) => {
  return shoppinglisttableData.value.findIndex(
    (item) => item.contractNumber === contractNumber
  );
};

// 获取当前合同号的合并行数
const getRowSpan = (contractNumber) => {
  return shoppinglisttableData.value.filter(
    (item) => item.contractNumber === contractNumber
  ).length;
};

// 处理采购需求表格的双击事件
const shoppinglisttableDatahandleRowDblClick = (row) => {
  // 跳转到采购合同页面
  router.push({
    path: '/purchase/procurementplansandcontracts'
  })
}

onMounted(async () => {
  // 初始化日历
  initCalendar();

  // 使用统一的计算函数来获取pendingCount
  await calculatePendingCount();

  // 获取各种数据
  GetRejectContractList();
  // 获取询价已回复列表
  getInquiryRepliedList();
  if (userStore.userInfo && userStore.userInfo.deptId === 210) {
    getInquiryList();
    GetRejectPurchaseContractList();
    GetProcurementequirements();
  }
  if (isAdminRole()) {
    getDocumentDeleteRequestList();
  }

  // 监听更新待办数量事件
  eventBus.on('updatePendingCount', () => {
    calculatePendingCount();
  });
})

watch(tasksActiveTab, (tabName) => {
  if (tabName === 'documentDeleteRequest' && isAdminRole()) {
    getDocumentDeleteRequestList(true)
  }
})

const overduePendingTaskPlanItemList = ref([])

const getOverduePendingTaskPlanItemList = async () => {
  const res = await request({
    url: 'PlanTasks/GetPlanTask_ItemsOverTimeData/GetPlanTask_Items',
    method: 'GET'
  })
  if (res.code === 200) {
    overdueEmailCount.value += res.data.length;
    res.data.forEach(item => {
      overduePendingTaskPlanItemList.value.push({
        taskId: item.taskId,
        itemId: item.itemId,
        taskName: item.taskName,
        phaseName: item.phaseName,
        itemName: item.itemName,
        timePoint: item.timePoint
      })
    })
  }
}

const route = useRoute();

// onMounted(() => {
//   if (route.query.contactId) {
//     openAgencyProcessdialogTableVisible();
//     const row = AgencyProcesstableData.value.find(item => item.documentID === route.query.contactId && item.documentType === 1)
//     openSaleContractDialog(row);
//   }
// });

// eventBus.on('open-sale-contact-approval', ({ contactId }) => {
//   openAgencyProcessdialogTableVisible();
//   const row = AgencyProcesstableData.value.find(item => item.documentID === route.query.contactId && item.documentType === 1)
//   openSaleContractDialog(row);
// });

// 添加一个标记
const needOpenSaleContract = ref(null);

onMounted(() => {
  if (route.query.contactId) {
    needOpenSaleContract.value = route.query.contactId;
  }

  // 获取当日汇率数据
  getTodayExchangeRates();
});

// 监听数据变化
watch(AgencyProcesstableData, (newData) => {
  if (newData.length > 0 && needOpenSaleContract.value) {
    const row = newData.find(
      item => item.documentID == needOpenSaleContract.value && item.documentType == "1"
    );
    if (row) {
      openAgencyProcessdialogTableVisible();
      openSaleContractDialog(row);
      needOpenSaleContract.value = null; // 清除标记
    }
  }
});

eventBus.on('open-sale-contact-approval', ({ contactId }) => {
  openAgencyProcessdialogTableVisible();

  if (AgencyProcesstableData.value.length > 0) {
    const row = AgencyProcesstableData.value.find(
      item => item.documentID == contactId && item.documentType == "1"
    );
    if (row) {
      openSaleContractDialog(row);
    }
  } else {
    // 数据还没加载，设置标记
    needOpenSaleContract.value = contactId;
  }
});

// 汇率填写通知相关函数
const initExchangeRateNotification = async () => {
  try {
    // 检查当前用户是否需要填写汇率
    const res = await request.get('ExchangeRateTask/CheckUserNeedFill/CheckUserNeedFill') as unknown as { data: ApiResponse }
    if (res.code === 200 && res.data.needFill) {
      // 显示汇率填写通知
      showExchangeRateNotification()
    }
  } catch (error) {
    console.error('检查汇率填写需求失败:', error)
  }
}

const showExchangeRateNotification = () => {
  exchangeRateNotificationVisible.value = true

  // 使用 nextTick 确保DOM更新后再初始化
  nextTick(() => {
    // 初始化多币种汇率表单
    initMultiCurrencyExchangeRateForm()

    // 如果字典数据还没加载，延迟重试
    if (!optionss.hr_export_currency || optionss.hr_export_currency.length === 0) {
      setTimeout(() => {
        initMultiCurrencyExchangeRateForm()
      }, 1000)
    }
  })

  // 设置定时器，每隔指定时间检查一次
  startExchangeRateNotificationTimer()
}
// 强制初始化表单
const forceInitForm = () => {
  // 清空现有数据
  multiCurrencyExchangeRateForm.exchangeRates = []
  multiCurrencyExchangeRateForm.globalRemark = ''

  // 直接从调试信息中看到的数据进行初始化
  const testCurrencies = [
    { dictValue: '1', dictLabel: '美元' },
    { dictValue: '2', dictLabel: '欧元' },
    { dictValue: '3', dictLabel: '人民币' }
  ]

  testCurrencies.forEach(currency => {
    multiCurrencyExchangeRateForm.exchangeRates.push({
      currency: currency.dictLabel,
      exchangeRate: '',
      remark: ''
    })
  })
}

// 初始化多币种汇率表单
const initMultiCurrencyExchangeRateForm = () => {
  multiCurrencyExchangeRateForm.exchangeRates = []
  multiCurrencyExchangeRateForm.globalRemark = ''

  // 尝试从多个地方获取币种数据
  let currencyData = null

  // 方法1：从 optionss 获取
  if (optionss.hr_export_currency && optionss.hr_export_currency.length > 0) {
    currencyData = optionss.hr_export_currency
  }
  // 方法2：从 state.optionss 获取
  else if (state.optionss.hr_export_currency && state.optionss.hr_export_currency.length > 0) {
    currencyData = state.optionss.hr_export_currency
  }
  // 方法3：从全局 state 获取
  else if (state.optionss && state.optionss.hr_export_currency && state.optionss.hr_export_currency.length > 0) {
    currencyData = state.optionss.hr_export_currency
  }

  if (currencyData && currencyData.length > 0) {
    currencyData.forEach(currency => {
      // 过滤掉人民币，人民币汇率默认为1，不需要用户填写
      if (currency.dictLabel !== '人民币' && currency.dictLabel !== 'CNY') {
        multiCurrencyExchangeRateForm.exchangeRates.push({
          currency: currency.dictLabel,
          exchangeRate: '',
          remark: ''
        })
      }
    })
  }
}

const startExchangeRateNotificationTimer = () => {
  // 清除之前的定时器
  if (exchangeRateNotificationTimer.value) {
    clearInterval(exchangeRateNotificationTimer.value)
  }

  // 获取通知间隔配置
  getNotificationInterval().then(interval => {
    exchangeRateNotificationTimer.value = setInterval(() => {
      checkExchangeRateStatus()
    }, interval * 60 * 1000) // 转换为毫秒
  })
}

const getNotificationInterval = async () => {
  try {
    const res = await request.get('ExchangeRateTask/GetConfig/GetConfig') as unknown as { data: ApiResponse }
    if (res.code === 200 && res.data) {
      return res.data.notificationInterval || 5
    }
  } catch (error) {
    console.error('获取通知间隔配置失败:', error)
  }
  return 5 // 默认5分钟
}

const checkExchangeRateStatus = async () => {
  try {
    const res = await request.get('ExchangeRateTask/CheckUserNeedFill/CheckUserNeedFill') as unknown as { data: ApiResponse }
    if (res.code === 200 && res.data.needFill) {
      // 如果用户还没有填写，继续显示通知
      if (!exchangeRateNotificationVisible.value) {
        showExchangeRateNotification()
      }
    } else {
      // 用户已经填写，清除定时器
      if (exchangeRateNotificationTimer.value) {
        clearInterval(exchangeRateNotificationTimer.value)
        exchangeRateNotificationTimer.value = null
      }
    }
  } catch (error) {
    console.error('检查汇率填写状态失败:', error)
  }
}

const submitExchangeRate = async () => {
  if (!exchangeRateFormRef.value) return

  await exchangeRateFormRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.warning('请填写完整的汇率信息')
      return
    }

    try {
      const requestData = {
        currency: exchangeRateForm.currency,
        exchangeRate: exchangeRateForm.exchangeRate,
        remark: exchangeRateForm.remark,
        date: new Date().toISOString().split('T')[0] // 今天的日期
      }

      const res = await request.post('ExchangeRateTask/SubmitExchangeRate/SubmitExchangeRate', requestData) as unknown as { data: ApiResponse }
      if (res.code === 200) {
        ElMessage.success('汇率填写成功')
        exchangeRateNotificationVisible.value = false

        // 清除定时器
        if (exchangeRateNotificationTimer.value) {
          clearInterval(exchangeRateNotificationTimer.value)
          exchangeRateNotificationTimer.value = null
        }

        // 重置表单
        resetExchangeRateForm()
      } else {
        ElMessage.error(res.msg || '提交失败')
      }
    } catch (error) {
      console.error('提交汇率失败:', error)
      ElMessage.error('提交失败，请稍后重试')
    }
  })
}

const skipExchangeRateNotification = () => {
  exchangeRateNotificationVisible.value = false
  // 5分钟后再次提醒
  setTimeout(() => {
    checkExchangeRateStatus()
  }, 5 * 60 * 1000)
}

const resetExchangeRateForm = () => {
  exchangeRateForm.currency = ''
  exchangeRateForm.exchangeRate = null
  exchangeRateForm.remark = ''
}

// 重置多币种汇率表单
const resetMultiCurrencyExchangeRateForm = () => {
  multiCurrencyExchangeRateForm.exchangeRates = []
  multiCurrencyExchangeRateForm.globalRemark = ''
}

// 显示汇率确认对话框
const showExchangeRateConfirmDialog = async () => {
  if (!exchangeRateFormRef.value) return

  await exchangeRateFormRef.value.validate(async (valid) => {
    if (!valid) {
      ElMessage.warning('请填写完整的汇率信息')
      return
    }

    // 检查是否所有币种都已填写汇率
    const unfilledCurrencies = multiCurrencyExchangeRateForm.exchangeRates.filter(rate => !rate.exchangeRate || rate.exchangeRate.trim() === '')
    if (unfilledCurrencies.length > 0) {
      ElMessage.warning('请为所有币种填写汇率')
      return
    }

    // 准备确认数据，包括用户填写的汇率和人民币汇率
    const userRates = multiCurrencyExchangeRateForm.exchangeRates.map(rate => ({
      currency: rate.currency,
      exchangeRate: rate.exchangeRate,
      remark: rate.remark || ''
    }))

    // 添加人民币汇率到确认数据中
    userRates.push({
      currency: '人民币',
      exchangeRate: '1',
      remark: '人民币汇率默认为1'
    })

    confirmExchangeRates.value = userRates
    confirmGlobalRemark.value = multiCurrencyExchangeRateForm.globalRemark

    // 显示确认对话框
    exchangeRateConfirmVisible.value = true
  })
}

// 确认提交多币种汇率
const confirmSubmitMultiCurrencyExchangeRate = async () => {
  try {
    // 准备汇率数据，包括用户填写的汇率和自动添加的人民币汇率
    const exchangeRates = [...multiCurrencyExchangeRateForm.exchangeRates.map(rate => {
      // 将 dictLabel 转换回 dictValue
      const currencyData = optionss.hr_export_currency?.find(item => item.dictLabel === rate.currency) ||
        state.optionss.hr_export_currency?.find(item => item.dictLabel === rate.currency)
      return {
        currency: currencyData ? currencyData.dictValue : rate.currency,
        exchangeRate: parseFloat(rate.exchangeRate) || 0,
        remark: rate.remark || multiCurrencyExchangeRateForm.globalRemark
      }
    })]

    // 自动添加人民币汇率（默认为1）
    const cnyCurrencyData = optionss.hr_export_currency?.find(item => item.dictLabel === '人民币') ||
      state.optionss.hr_export_currency?.find(item => item.dictLabel === '人民币')

    if (cnyCurrencyData) {
      exchangeRates.push({
        currency: cnyCurrencyData.dictValue,
        exchangeRate: 1,
        remark: '人民币汇率默认为1'
      })
    }

    const requestData = {
      exchangeRates: exchangeRates,
      date: new Date().toISOString().split('T')[0] // 今天的日期
    }

    const res = await request.post('ExchangeRateTask/SubmitMultiCurrencyExchangeRate/SubmitMultiCurrencyExchangeRate', requestData) as unknown as { data: ApiResponse }
    if (res.code === 200) {
      ElMessage.success('所有汇率填写成功')
      exchangeRateNotificationVisible.value = false
      exchangeRateConfirmVisible.value = false
      // 清除定时器
      if (exchangeRateNotificationTimer.value) {
        clearInterval(exchangeRateNotificationTimer.value)
        exchangeRateNotificationTimer.value = null
      }
      // 重置表单
      resetMultiCurrencyExchangeRateForm()
    } else {
      ElMessage.error(res.msg || '汇率填写失败')
    }
  } catch (error) {
    console.error('提交汇率失败:', error)
    ElMessage.error('汇率填写失败，请重试')
  }
}

// 提交多币种汇率（保留原函数名以兼容其他调用）
const submitMultiCurrencyExchangeRate = async () => {
  await showExchangeRateConfirmDialog()
}

// 组件卸载时清理定时器
onUnmounted(() => {
  if (exchangeRateNotificationTimer.value) {
    clearInterval(exchangeRateNotificationTimer.value)
  }
  if (exchangeRateCheckTimer.value) {
    clearInterval(exchangeRateCheckTimer.value)
  }
})

//#region 获取驳回的采购合同列表
const GetRejectPurchaseContractList = async () => {
  try {
    //获取驳回的采购合同列表
    const RejectPurchaseContractListResponse = await request({
      url: 'PurchaseContracts/GetRejectPurchaseContractList/GetRejectList',
      method: 'get'
    })
    if (RejectPurchaseContractListResponse.code == 200) {
      rejectPurchaseContractList.value = RejectPurchaseContractListResponse.data || []
      // 处理数据格式化
      rejectPurchaseContractList.value.forEach(item => {
        // 格式化日期
        if (item.createTime) {
          item.createTime = formatDate(item.createTime)
        }
        // 格式化金额
        if (item.totalAmount) {
          item.totalAmount = parseFloat(item.totalAmount).toFixed(2)
        }
      })
    }
  } catch (error) {
    console.error('获取驳回采购合同列表失败', error)
  }
}
//#endregion

//#region 获取驳回的合同列表
const GetRejectContractList = async () => {
  try {
    const RejectContractListResponse = await request({
      url: 'Contracts/GetRejectContractsList/GetRejectList',
      method: 'get'
    })
    if (RejectContractListResponse.code == 200) {
      rejectContractList.value = RejectContractListResponse.data || []
      // 处理数据格式化
      rejectContractList.value.forEach(item => {
        // 格式化日期
        if (item.createTime) {
          item.createTime = formatDate(item.createTime)
        }
        // 格式化金额
        if (item.totalAmount) {
          item.totalAmount = parseFloat(item.totalAmount).toFixed(2)
        }
      })
    }
  } catch (error) {
    console.error('获取驳回合同列表失败', error)
  }
}
//#endregion

//#region 付款任务相关处理函数
// 获取付款任务列表
const getPaymentTaskList = async () => {
  try {
    const response = await request({
      url: 'PaymentRequest/GetPendingPaymentList/GetPendingPaymentList',
      method: 'GET',
      params: {
        PageNum: 1,
        PageSize: 100
      }
    });

    if (response.code === 200 && response.data && response.data.result) {
      // 映射数据到表格需要的格式
      paymentTaskList.value = response.data.result.map(item => {
        // 获取申请人名称
        const applicantName = state.optionss['sql_all_user']?.find(
          user => user.dictValue === item.applicant?.toString()
        )?.dictLabel || '未知';

        // 获取币种
        const currency = state.optionss['hr_currency_code']?.find(
          code => code.dictValue === item.currencyCode?.toString()
        )?.dictLabel || item.currencyCode || '';

        // 获取款项名称 - 根据付款类别选择对应的字典
        const paymentCategory = item.paymentCategory?.toString();
        const paymentName = item.paymentName?.toString();
        let paymentItemName = '未知';

        if (paymentCategory && paymentName) {
          switch (paymentCategory) {
            case '1':
              paymentItemName = state.optionss['hr_factory_payment']?.find(
                item => item.dictValue === paymentName
              )?.dictLabel || paymentName;
              break;
            case '2':
              paymentItemName = state.optionss['hr_domestic_charges']?.find(
                item => item.dictValue === paymentName
              )?.dictLabel || paymentName;
              break;
            case '3':
              paymentItemName = state.optionss['hr_foreign_charges']?.find(
                item => item.dictValue === paymentName
              )?.dictLabel || paymentName;
              break;
            case '4':
              paymentItemName = state.optionss['hr_daily_expenses']?.find(
                item => item.dictValue === paymentName
              )?.dictLabel || paymentName;
              break;
            case '5':
              paymentItemName = state.optionss['hr_business_expenses']?.find(
                item => item.dictValue === paymentName
              )?.dictLabel || paymentName;
              break;
            default:
              paymentItemName = paymentName;
              break;
          }
        }

        return {
          id: item.id,
          applicationDate: formatDate(item.applicationDate || item.create_time),
          payeeUnit: item.payeeName || '未知',
          paymentItemName: paymentItemName,
          paymentAmount: item.totalAmount || 0,
          currency: currency,
          applicant: applicantName,
          paymentCategory: paymentCategory, // 保存付款类别用于路由跳转
          // 保留原始数据用于详情查看
          originalData: item
        };
      });

      return paymentTaskList.value;
    } else {
      paymentTaskList.value = [];
      return [];
    }
  } catch (error) {
    console.error('获取付款任务列表失败', error);
    ElMessage.error('获取付款任务列表失败');
    paymentTaskList.value = [];
    return [];
  }
}

// 处理付款任务表格行双击事件
const handlePaymentTaskRowDblClick = (row) => {
  console.log('双击付款任务:', row);
  viewPaymentTask(row);
}

const handleReceiptConfirmRowDblClick = (row) => {
  const customerId = row.customerIDRaw || row.customerID || row.customerId;
  if (!customerId) {
    ElMessage.warning('无法获取客户信息');
    return;
  }
  router.push({
    path: '/finance/customercollection',
    query: {
      customerId,
      receiptId: row.id,
      receiptNumber: row.receiptNumber
    }
  });
}

// 查看付款任务详情
const viewPaymentTask = (row) => {
  console.log('查看付款任务详情:', row);
  // 获取付款申请ID和付款类别
  const paymentRequestId = row.id || row.originalData?.id;
  const paymentCategory = row.originalData?.paymentCategory?.toString() || row.paymentCategory?.toString();

  if (!paymentRequestId) {
    ElMessage.warning('无法获取付款申请ID');
    return;
  }

  // 根据付款类别跳转到不同的页面
  let targetPath = '';

  if (paymentCategory === '1') {
    // 工厂付款
    targetPath = '/finance/factorypayment';
  } else if (paymentCategory === '5') {
    // 业务费用付款
    targetPath = '/finance/businessExpensespayment';
  } else if (paymentCategory === '4') {
    // 日常费用付款
    targetPath = '/finance/dailyExpensespayment';
  } else {
    // 其他情况，跳转到付款申请详情页面
    targetPath = '/paymentrequest';
  }

  router.push({
    path: targetPath,
    query: {
      PaymentRequestID: paymentRequestId,
      viewDetail: 'true'
    }
  });
}
//#endregion

//#region 被驳回单据相关处理函数
// 处理tab切换事件
const handleTabClick = (tab) => {
  if (tab.props.name === 'reject') {
    // 当切换到被驳回单据tab时，重新加载数据
    GetRejectPurchaseContractList()
    GetRejectContractList()
    // 重新计算pendingCount
    calculatePendingCount()
  } else if (tab.props.name === 'purchasePriceChange') {
    // 当切换到采购价格变更tab时，重新加载数据
    getPurchasePriceChangeContracts(true)
  } else if (tab.props.name === 'paymentTask') {
    // 当切换到付款任务tab时，重新加载数据
    getPaymentTaskList()
  } else if (tab.props.name === 'inquiryReplied') {
    // 当切换到询价已回复tab时，重新加载数据
    getInquiryRepliedList()
  } else if (tab.props.name === 'receiptConfirm') {
    // 当切换到收款单确认tab时，重新加载数据
    loadReceiptConfirmList()
  }
}

// 处理采购价格变更表格行双击事件
const handlePurchasePriceChangeRowDblClick = (row) => {
  console.log('双击采购价格变更合同:', row)
  viewPurchasePriceChangeContract(row)
}

// 查看采购价格变更合同详情
const viewPurchasePriceChangeContract = (row) => {
  const contractId = row?.id || row?.contractId || row?.ID || row?.Id
  if (!contractId) {
    ElMessage.warning('无法获取销售合同ID')
    return
  }
  router.push({
    path: '/sale/sale/salecontract',
    query: {
      contractId: contractId,
      contractNumber: row.contractNumber,
      viewDetail: 'true'
    }
  })
}

// 处理被驳回采购合同表格行双击事件
const handleRejectPurchaseContractRowDblClick = (row) => {
  console.log('双击被驳回采购合同:', row)
  // 双击时也跳转到详情页面
  viewRejectPurchaseContract(row)
}

// 处理被驳回销售合同表格行双击事件
const handleRejectContractRowDblClick = (row) => {
  console.log('双击被驳回销售合同:', row)
  // 双击时也跳转到详情页面
  viewRejectContract(row)
}

// 查看被驳回采购合同详情
const viewRejectPurchaseContract = (row) => {
  console.log('查看被驳回采购合同详情:', row)
  if (row.id || row.purchaseContractId) {
    const contractId = row.id || row.purchaseContractId
    router.push({
      path: '/purchase/procurementplansandcontracts',
      query: {
        purchaseContractId: contractId,
        viewDetail: 'true'
      }
    })
  } else {
    ElMessage.warning('无法获取采购合同ID')
  }
}

// 查看被驳回销售合同详情
const viewRejectContract = (row) => {
  console.log('查看被驳回销售合同详情:', row)
  if (row.id || row.contractId) {
    const contractId = row.id || row.contractId
    router.push({
      path: '/sale/sale/salecontract',
      query: {
        contractId: contractId,
        contractNumber: row.contractNumber,
        viewDetail: 'true'
      }
    })
  } else {
    ElMessage.warning('无法获取销售合同ID')
  }
}
//#endregion
</script>


<style lang="scss" scoped>
.red-text {
  color: #f56c6c;
}

// 汇率填写通知样式
.exchange-rate-notification {
  .notification-header {
    display: flex;
    align-items: center;
    margin-bottom: 20px;
    padding: 15px;
    background-color: #fff7e6;
    border: 1px solid #ffd591;
    border-radius: 6px;

    .notification-icon {
      color: #fa8c16;
      margin-right: 10px;
    }

    .notification-title {
      font-size: 16px;
      font-weight: 600;
      color: #d46b08;
    }
  }

  .notification-content {
    margin-bottom: 20px;

    p {
      margin: 0;
      color: #666;
      line-height: 1.6;
    }
  }

  .currency-rates-container {
    max-height: 400px;
    overflow-y: auto;
    border: 1px solid #e4e7ed;
    border-radius: 6px;
    padding: 15px;
    background-color: #fafafa;

    .currency-rate-item {
      margin-bottom: 10px;
      padding: 8px;
      background-color: #fff;
      border-radius: 4px;
      border: 1px solid #e4e7ed;

      &:last-child {
        margin-bottom: 0;
      }

      .el-form-item {
        margin-bottom: 0;
      }
    }
  }
}

// 汇率确认对话框样式
.exchange-rate-confirm {
  .confirm-header {
    display: flex;
    align-items: center;
    margin-bottom: 20px;
    padding: 15px;
    background-color: #f0f9ff;
    border: 1px solid #b3d8ff;
    border-radius: 6px;

    .confirm-icon {
      color: #409EFF;
      margin-right: 10px;
    }

    .confirm-title {
      font-size: 16px;
      font-weight: 600;
      color: #1890ff;
    }
  }

  .confirm-content {
    p {
      margin: 0 0 15px 0;
      color: #666;
      line-height: 1.6;
    }

    .exchange-rate-value {
      font-weight: 600;
      color: #409EFF;
      font-size: 16px;
    }

    .text-muted {
      color: #999;
      font-style: italic;
    }

    .global-remark-section {
      margin-top: 20px;
      padding: 15px;
      background-color: #f8f9fa;
      border-radius: 6px;
      border: 1px solid #e9ecef;

      h4 {
        margin: 0 0 10px 0;
        color: #495057;
        font-size: 14px;
        font-weight: 600;
      }

      .global-remark-text {
        margin: 0;
        color: #666;
        line-height: 1.6;
        white-space: pre-wrap;
      }
    }
  }
}

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
  display: inline-block;
  margin-left: 5px;
  min-width: 18px;
  height: 18px;
  line-height: 18px;
  background-color: #E6A23C;
  color: white;
  font-size: 12px;
  text-align: center;
  border-radius: 9px;
  padding: 0 4px;
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

.custom-notification {
  margin-bottom: 16px !important;

  .notification-content {
    margin-top: 10px;
  }

  .notification-message {
    margin-bottom: 10px;
    word-break: break-all;
    line-height: 1.5;
  }

  .notification-footer {
    text-align: right;
    margin-top: 10px;
  }

  .custom-btn {
    padding: 5px 15px;
    background-color: #409eff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;

    &:hover {
      background-color: #66b1ff;
    }
  }
}

/* 金额超出时的输入框样式 */
.amount-exceeded {
  :deep(.el-input__wrapper) {
    box-shadow: 0 0 0 1px #f56c6c inset !important;
  }

  :deep(.el-input__inner) {
    color: #f56c6c;
  }
}

/* 新增的紧凑布局样式 */
.home {
  padding: 8px;
  background-color: #f5f7fa;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* 当日汇率显示样式 - 紧凑型 */
.exchange-rate-container {
  width: 100%;
}

.exchange-rate-card {
  height: 120px;
}

.exchange-rate-content {
  padding: 0;
  display: flex;
  align-items: center;
  min-height: 64px;
  width: 100%;
}

.exchange-rate-list {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
}

.no-exchange-rate {
  color: #909399;
  font-size: 12px;
  line-height: 1;
}

/* 仪表板头部样式 */
.dashboard-header {
  margin-bottom: 8px;
  flex-shrink: 0;
}

.mb8 {
  margin-bottom: 8px;
}

.dashboard-card {
  height: 120px;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.dashboard-equal-height-row .el-col {
  display: flex;
}

.dashboard-equal-height-row .dashboard-card {
  width: 100%;
  height: 100%;
}

.customer-statistics-card {
  min-height: 170px;
}

.customer-statistics-header {
  justify-content: space-between;
}

.customer-statistics-title {
  display: flex;
  align-items: center;
  gap: 8px;
}

.customer-scope-select {
  width: 140px;
}

.customer-statistics-content {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 12px;
}

.customer-statistics-content .metric-row {
  margin-bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;
  gap: 6px;
}

.sales-conversion-card {
  min-height: 170px;
}

.sales-conversion-header {
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.sales-conversion-title {
  display: flex;
  align-items: center;
  gap: 8px;
}

.sales-conversion-filters {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.sales-conversion-filter {
  width: 120px;
}

.sales-conversion-content {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 12px;
}

.sales-conversion-content .metric-row {
  margin-bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;
  gap: 6px;
}

.business-summary-card {
  height: auto;
  min-height: 280px;
}

.business-summary-header {
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.business-summary-title {
  display: flex;
  align-items: center;
  gap: 8px;
}

.business-summary-filters {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.business-summary-filter {
  width: 120px;
}

.business-summary-totals {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  border-top: 1px solid #ebeef5;
  border-left: 1px solid #ebeef5;
  margin-top: 6px;
}

.business-summary-total-item {
  border-right: 1px solid #ebeef5;
  border-bottom: 1px solid #ebeef5;
  padding: 10px 12px;
}

.business-summary-total-label {
  font-size: 14px;
  color: #303133;
  margin-bottom: 8px;
  font-weight: 600;
}

.business-summary-total-value {
  font-size: 32px;
  color: #111;
  font-weight: 700;
}

.business-summary-chart-wrap {
  border-left: 1px solid #ebeef5;
  border-right: 1px solid #ebeef5;
  border-bottom: 1px solid #ebeef5;
  padding: 6px 10px 10px;
}

.business-summary-chart {
  width: 100%;
  height: 150px;
  display: block;
}

.business-summary-line {
  fill: none;
  stroke-width: 2;
  stroke-linecap: round;
  stroke-linejoin: round;
}

.business-summary-line.deal-line {
  stroke: #67c23a;
}

.business-summary-line.shipped-line {
  stroke: #e6a23c;
}

.business-summary-month-labels {
  display: grid;
  grid-template-columns: repeat(12, minmax(0, 1fr));
  margin-top: 4px;
}

.business-summary-month-item {
  text-align: center;
  font-size: 12px;
  color: #606266;
}

.business-summary-legend {
  margin-top: 8px;
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.business-summary-legend-item {
  font-size: 13px;
  font-weight: 500;
}

.business-summary-legend-item.deal {
  color: #67c23a;
}

.business-summary-legend-item.shipped {
  color: #e6a23c;
}

.dashboard-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
}

.card-icon {
  font-size: 18px;
  color: #409eff;
}

.card-title {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}

.card-content {
  padding: 0;
}

.metric-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2px;
}

.metric-label {
  font-size: 12px;
  color: #606266;
}

.metric-value {
  font-size: 18px;
  font-weight: 600;
  padding: 0;
  height: auto;
}

.metric-value.primary {
  color: #409eff;
}

.metric-value.danger {
  color: #f56c6c;
}

/* 主要内容区域 */
.main-content {
  flex: 1;
  display: flex;
  gap: 8px;
  align-items: flex-start;
  overflow: hidden;
}

.left-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 0;
  min-height: 0;
}

.left-panel.full-width {
  flex: 2;
  max-width: 100%;
}

.right-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  margin-bottom: 0;
  height: 100%;
  min-height: 0;
  max-height: 100%;
}

/* 日历卡片样式 */
.calendar-card {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 430px;
  max-height: 430px;
  margin-bottom: 0;
  overflow: hidden;
}

.compact-calendar {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.calendar-header {
  display: flex;
  justify-content: center;
  margin-bottom: 8px;
}

.current-month {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin: 0 12px;
  line-height: 32px;
}

.calendar-grid {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 200px;
}

.calendar-weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0;
  margin-bottom: 4px;
}

.weekday {
  text-align: center;
  font-size: 12px;
  color: #909399;
  font-weight: 500;
  padding: 4px 0;
}

.calendar-days {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  grid-template-rows: repeat(6, 1fr);
  gap: 0;
  flex: 1;
  min-height: 160px;
}

.calendar-day {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2px;
  border-radius: 0;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  min-height: 32px;
  border: 1px solid transparent;
  background-color: white;
}

.calendar-day:hover {
  background-color: #f0f9ff;
  border-color: #409eff;
}

.calendar-day.other-month {
  color: #c0c4cc;
  background-color: #fafafa;
}

.calendar-day.today {
  background-color: #409eff;
  color: white;
  font-weight: bold;
}

.calendar-day.has-tasks {
  background-color: #fff7e6;
  border: 1px solid #ffd666;
}

/* 颜色含义提示样式 */
.calendar-legend {
  margin-top: 8px;
  padding: 8px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #e4e7ed;
}

.legend-title {
  font-size: 12px;
  font-weight: 500;
  color: #606266;
  margin-bottom: 6px;
}

.legend-items {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 4px;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 2px;
  border: 1px solid #dcdfe6;
}

.legend-color.normal {
  background-color: white;
}

.legend-color.today {
  background-color: #409eff;
}

.legend-color.has-tasks {
  background-color: #fff7e6;
  border-color: #ffd666;
}

.legend-color.other-month {
  background-color: #fafafa;
}

.legend-text {
  font-size: 11px;
  color: #606266;
}

.calendar-day.other-month {
  color: #c0c4cc;
}

.day-number {
  font-size: 12px;
  font-weight: 500;
}

.task-indicator {
  position: absolute;
  top: 2px;
  right: 2px;
}

/* 任务卡片样式 */
.tasks-card {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 340px;
  max-height: 340px;
  margin-bottom: 0;
  overflow: hidden;
}

.compact-tabs {
  margin-left: auto;
}

.compact-tabs :deep(.el-tabs__header) {
  margin: 0;
}

.compact-tabs :deep(.el-tabs__nav-wrap) {
  padding: 0;
}

.compact-tabs :deep(.el-tabs__item) {
  padding: 8px 16px;
  font-size: 12px;
}

.tasks-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  margin-bottom: 0;
}

.tasks-content .el-table {
  flex: 1;
  overflow: hidden;
}

.tasks-content .el-table__body-wrapper {
  overflow-y: auto;
  max-height: none;
}

/* 商机看板样式 */
.opportunities-card {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  max-height: 100%;
  margin-bottom: 0;
  overflow: hidden;
}

.opportunities-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  margin-bottom: 0;
  height: calc(100% - 70px);
  max-height: calc(100% - 70px);
  /* 减去头部和金额区域的高度 */
}

.opportunities-list {
  flex: 1;
  overflow-y: auto;
  padding: 0;
  display: flex;
  flex-direction: column;
}

.opportunities-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 4px;
  height: 100%;
  max-height: 100%;
  overflow: hidden;
}

.opportunity-column {
  display: flex;
  flex-direction: column;
  background-color: #f8f9fa;
  border-radius: 6px;
  overflow: hidden;
  height: 100%;
  max-height: 100%;
  border: 1px solid #e4e7ed;
  gap: 4px;
}

.column-header {
  background-color: #41c16e;
  color: white;
  padding: 4px;
  text-align: center;
  font-size: 12px;
  font-weight: 500;
}

.stage-name {
  display: block;
}

.stage-count {
  display: block;
  font-size: 10px;
  opacity: 0.8;
}

.column-amount {
  padding: 4px;
  text-align: center;
  background-color: white;
  border-bottom: 1px solid #e4e7ed;
}

.amount-text {
  font-size: 12px;
  font-weight: 600;
  color: #303133;
}

.opportunities-list {
  flex: 1;
  overflow-y: auto;
  padding: 4px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  max-height: calc(100% - 8px);
  /* 减去padding */
  overflow-x: hidden;
}

.opportunity-item {
  background-color: white;
  border-radius: 0;
  padding: 4px;
  margin-bottom: 0;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid #e4e7ed;
  border-bottom: none;
  min-height: 60px;
  height: auto;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.opportunity-item:last-child {
  border-bottom: 1px solid #e4e7ed;
}

.opportunity-item:hover {
  border-color: #409eff;
  box-shadow: 0 2px 8px rgba(64, 158, 255, 0.1);
  z-index: 10;
  position: relative;
}

.opportunity-item:active {
  transform: scale(0.98);
  transition: transform 0.1s ease;
}

/* 添加双击提示样式 */
.opportunity-item::after {
  content: "双击查看详情";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: rgba(0, 0, 0, 0.8);
  color: white;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.3s ease;
  z-index: 1000;
}

.opportunity-item:hover::after {
  opacity: 1;
}

.item-header {
  margin-bottom: 2px;
}

.item-number {
  font-size: 11px;
  color: #409eff;
  font-weight: 500;
}

.item-content {
  font-size: 11px;
}

.item-row {
  display: flex;
  margin-bottom: 1px;
}

.item-label {
  color: #909399;
  min-width: 40px;
}

.item-value {
  color: #303133;
  flex: 1;
}

.truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}



/* 自定义滚动条样式 */
.opportunities-list::-webkit-scrollbar {
  width: 6px;
}

.opportunities-list::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.opportunities-list::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.opportunities-list::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* 确保滚动条在内容溢出时显示 */
.opportunities-list {
  overflow-y: auto;
  overflow-x: hidden;
}

/* 响应式设计 */
@media (max-width: 1200px) {
  .opportunities-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 768px) {
  .opportunities-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .left-panel,
  .right-panel {
    height: auto;
  }
}

// 图片和附件显示样式
.image-gallery {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 15px;
  margin-top: 10px;
}

.image-item {
  position: relative;
  cursor: pointer;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.image-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.receipt-image {
  width: 100%;
  height: 120px;
  object-fit: cover;
  display: block;
}

.image-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
  color: white;
  font-size: 24px;
}

.image-item:hover .image-overlay {
  opacity: 1;
}

.attachment-list {
  margin-top: 10px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 10px;
  background-color: #f9fafb;
}

.attachment-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  margin-bottom: 8px;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.attachment-item:hover {
  background-color: #f8f9fa;
  border-color: #d1d5db;
}

.attachment-item:last-child {
  margin-bottom: 0;
}

.attachment-info {
  display: flex;
  align-items: center;
  flex: 1;
}

.attachment-icon {
  margin-right: 8px;
  color: #6b7280;
  font-size: 16px;
}

.attachment-name {
  font-weight: 500;
  color: #374151;
  word-break: break-all;
}

.attachment-actions {
  display: flex;
  gap: 4px;
}

.attachment-actions .el-button {
  padding: 4px 8px;
  font-size: 12px;
}
</style>