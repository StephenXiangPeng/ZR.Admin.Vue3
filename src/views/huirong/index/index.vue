<template>
  <div class="home">
    <!-- 任务看板 -->
    <el-row :gutter="15" style="font-size: 25px;">
      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%; border: 0px;" shadow="never">
          <div class="text-warning mb10">代办流程</div>
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
            <span>&nbsp;&nbsp;&nbsp;&nbsp;<el-button type="text"
                style="font-weight: bold;font-size: 30px; color: black;" @click="WaitingforyouProcessedClick">{{
                  pendingCount }}</el-button></span>
            <span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<el-button type="text"
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
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;10</span>
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;10</span>
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;10</span>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%;border: 0px;" shadow="never">
          <div class="text-warning mb10">工作任务</div>
          <el-divider></el-divider>
          <div class="work-wrap">
            <!-- <el-statistic title="待您处理" value="10" />
            <el-statistic title="超时未处理" value="10" /> -->
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
            <span style="font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;10</span>
            <span style="color: red; font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;8</span>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 计划任务 -->
    <el-calendar>
      <template #date-cell="{ data }">
        <el-row :class="data.isSelected ? 'is-selected' : 'sds'">
          {{ data.day.split('-').slice(1).join('-') }}
          {{ data.isSelected ? '✔️' : '' }}
        </el-row>
        <div v-for="(item, index) in textContent(data.day)" :key="index">
          <e-row>
            <el-col class="center">
              <el-tag type="warning" class="tag">
                <el-row v-if="item.xianyue == 0">
                  <el-col :span="17" class="tag">
                    <span>逾期的任务</span>
                  </el-col>
                  <el-col :span="1"></el-col>
                  <el-col :span="6" class="tag2">
                    <span>0</span>
                  </el-col>
                </el-row>
                <el-row v-else>
                  <el-col :span="17" class="tag">
                    <span>逾期的任务</span>
                  </el-col>
                  <el-col :span="1"></el-col>
                  <el-col :span="6" class="tag2">
                    <span>{{ item.xianyue }}</span>
                  </el-col>
                </el-row>
              </el-tag>
            </el-col>
          </e-row>
          <!-- <el-row style="margin-top: 10px" class="yuyue" v-if="item.yiyue && item.sy == 0">
            <el-col :span="11" class="center">
              <span>已约</span><span class="center2" style="">0</span></el-col>
            <el-col :span="2" class="center">|</el-col>
            <el-col :span="11" class="center">
              <span>剩余</span><span class="center2">0</span></el-col>
          </el-row>
          <el-row style="margin-top: 10px" class="yuyue" v-else>
            <el-col :span="11" class="center">
              <span>已约</span><span class="center2" style="">{{
                item.yiyue
              }}</span></el-col>
            <el-col :span="2" class="center">|</el-col>
            <el-col :span="11" class="center">
              <span>剩余</span><span class="center2">{{ item.sy }}</span></el-col>
          </el-row> -->
        </div>

      </template>
    </el-calendar>

    <!-- 工作任务 -->
    <span style="font-size: 20px; font-weight: bold;">&nbsp;&nbsp;工作任务</span>
    <el-divider></el-divider>
    <el-table :data="tableData" style="width: 50%;">
      <el-table-column fixed prop="date" label="项目分类" style="width: 25%;" />
      <el-table-column prop="name" label="内容" style="width: 25%;" />
      <el-table-column prop="state" label="原设定时间节点" style="width: 25%;" />
      <el-table-column fixed="right" label="Operations" style="width: 25%;">
        <template #default>
          <el-button link type="primary" size="small" @click="handleClick">Detail</el-button>
          <el-button link type="primary" size="small">Edit</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination background layout="prev, pager, next" :total="1000" style="margin-top: 5px;" />

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
    <div style="width: 100%; height: 600px;">
      <div style="margin-top: 5px;margin-bottom: 5px;">
        <span style="color: #6d6d71;">总销售金额：</span><span style="color: black; font-weight: 600;">CNY 1,247.97万 </span>
        <el-divider direction="vertical" />
        <span style="color: #6d6d71;">进行中金额：</span><span style="color: #51bf5d; font-weight: 600;">CNY 1,247.97万 </span>
        <el-divider direction="vertical" />
        <span style="color: #6d6d71;">赢单金额：</span><span style="color: #338bff; font-weight: 600;">CNY 1,247.97万 </span>
      </div>

      <div style="display: flex; margin-top: 5px;">
        <div style="width: 20%;; height: 560px;">
          <div
            style="background-color: #41c16e;height: 8%; padding: 10px; text-align: center; color: white; font-weight:500; ">
            询盘（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 30.56 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
        <div style="width: 20%; height: 560px;overflow: auto;">
          <div
            style="background-color: #41c16e;height: 8%;padding: 10px; text-align: center; color: white; font-weight:500; ">
            第一次报价（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 50.88 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责��：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
        <div style="width: 20%; height: 560px;overflow: auto;">
          <div
            style="background-color: #41c16e;height: 8%;padding: 10px; text-align: center; color: white; font-weight:500; ">
            沟通需求（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 600.66 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
        <div style="width: 20%; height: 560px;overflow: auto;">
          <div
            style="background-color: #41c16e;height: 8%;padding: 10px; text-align: center; color: white; font-weight:500; ">
            再次报价（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 600.66 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
        <div style="width: 20%; height: 560px;overflow: auto;">
          <div
            style="background-color: #41c16e;height: 8%;padding: 10px; text-align: center; color: white; font-weight:500; ">
            合同确定（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 600.66 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
        <div style="width: 20%; height: 560px;overflow: auto;">
          <div
            style="background-color: #41c16e;height: 8%;padding: 10px; text-align: center; color: white; font-weight:500; ">
            赢单金额（1000）
          </div>
          <div style="text-align: center;">
            <span style="color: black; font-weight: 600; ">CNY 600.66 万 </span>
          </div>
          <div style="overflow: auto; height: 490px;">
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
            <el-card style="margin: 3px;" class="box-card">
              <template #header>
                <div class="card-header">
                  <span>商机编号：00000001</span>
                </div>
              </template>
              <div class="text item">
                商机名称：撒大苏打撒旦艰苦拉萨角度看
              </div>
              <div class="text item">预估金额：21万</div>
              <div class="text item">国家地区：美国</div>
              <div class="text item">负责人：张三</div>
              <div class="text item">创建日期：23年12月20日</div>
              <div class="text item">当前停留时间：50天</div>
              <div class="text item">备注：该客户已经联系多次，并未给出明确采购需求。</div>
            </el-card>
          </div>
        </div>
      </div>

    </div>

    <el-row :gutter="15" style=" margin-top: 30px; font-size: 25px;">
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
              <el-option v-for=" item in options3" :key="item.value" :label="item.label" :value="item.value" />
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


    </el-row>
    <!-- 常用功能 -->
    <!-- <el-row :gutter="15" style="margin-top: 30px;">
      <el-col :md="24" :lg="24" :xl="24" class="mb10">
        <el-card shadow="hover">
          <template #header>
            <span><svg-icon name="tool" /> {{ $t('layout.commonFuncs') }}</span>
            <div class="home-card-more">
              <el-button text @click="handleAdd()">{{ $t('btn.add') }}</el-button>
            </div>
          </template>
          <div>
            <el-scrollbar wrap-class="scrollbar-wrapper"> <CommonMenu v-model="showEdit"></CommonMenu></el-scrollbar>
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
    <!-- 领取收款单对话框 -->
    <el-dialog v-model="claimDialogVisible" title="领取收款单" width="500px" :close-on-click-modal="false">
      <el-form ref="claimFormRef" :model="claimForm" :rules="claimRules" label-width="100px">
        <el-form-item label="合同号" prop="contractsID">
          <el-select v-model="claimForm.contractsID" placeholder="请选择合同号" style="width: 100%" filterable>
            <el-option v-for="item in state.optionss.sql_sale_contracts" :key="item.dictValue" :label="item.dictLabel"
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
  </div>
</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessage, ElMessageBox } from "element-plus";
import request from '@/utils/request';
import dayjs from 'dayjs';
import useUserStore from '@/store/modules/user'
import useSocketStore from '@/store/modules/socket'
// 时间插件
import duration from 'dayjs/plugin/duration'
import { number } from 'echarts';
import SalesContractdialog from '@/views/components/SalesContractdialog.vue';
import { Picture } from '@element-plus/icons-vue'
import Salecontract from '@/views/sale/salecontract.vue';
import { id } from 'element-plus/es/locale';


// #region 财务收款单相关
// 对话框显示控制
const claimDialogVisible = ref(false)
const claimFormRef = ref(null)

// 表单数据
const claimForm = reactive({
  id: '', // 收款单ID
  contractsID: '', // 合同号
  fundsType: '', // 款项类型
  remark: '' // 备注
})

// 表单验证规则
const claimRules = {
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
            Remark: claimForm.remark
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
  claimForm.contractId = ''
  claimForm.fundsType = ''
  claimForm.remark = ''
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
      ShippingDeliveryDialog.value = false;
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
  volume: null              // 体积
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
const openSaleContractDialog = (row) => {
  ApproveDocumentRequest.ApprovalRecordID = row.recordID;
  ApproveDocumentRequest.DocumentID = row.documentID;
  ApproveDocumentRequest.StageID = row.stageID;
  ApproveDocumentRequest.ApproverID = row.approverID;
  ApproveDocumentRequest.DocumentType = row.documentType;
  //获取销售合同详情
  if (row.documentType == "1") {
    request({
      url: 'Contracts/GetContractDetailsById/GetContractDetails',
      method: 'GET',
      params: {
        contractId: row.documentID
      }
    }).then(response => {
      if (response.data != null) {
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
    sql_product: []
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
  { dictType: 'sql_product' }
]
proxy.getDicts(dictParams).then((response) => {
  response.data.forEach((element) => {
    state.optionss[element.dictType] = element.list
  })
  getPendingCount();
  getFinancialTasksList(1, 10);
})

const getPendingCount = () => {
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
          if (item.documentType == "1") {
            item.documentNumber = state.optionss['sql_sale_contracts'].filter(Salecontract => Salecontract.dictValue == item.documentID.toString()).map(Salecontract => Salecontract.dictLabel).values().next().value;
          } else if (item.documentType == "2") {
            item.documentNumber = state.optionss['sql_purchase_contract'].filter(Purchasecontract => Purchasecontract.dictValue == item.documentID.toString()).map(Purchasecontract => Purchasecontract.dictLabel).values().next().value;
          } else if (item.documentType == "5") {
            item.documentNumber = state.optionss['sql_payment_requests'].filter(Paymentrequests => Paymentrequests.dictValue == item.documentID.toString()).map(Paymentrequests => Paymentrequests.dictLabel).values().next().value;
          } else if (item.documentType == "3") {
            item.documentNumber = state.optionss['sql_shippingdeliveries'].filter(Shippingdeliveries => Shippingdeliveries.dictValue == item.documentID.toString()).map(Shippingdeliveries => Shippingdeliveries.dictLabel).values().next().value;
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
      ElMessage.error("获取代办流程数量失败");
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
      day: "2023-11-04",
      xianyue: 400,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2023-11-05",
      xianyue: 500,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2023-11-06",
      xianyue: 200,
      yiyue: 5,
      sy: 1,
    },
    {
      day: "2023-11-07",
      xianyue: 0,
      yiyue: 0,
      sy: 0,
    },
  ],
});

//处理日期获取后台数据动态渲染上去
const textContent = (date) => {
  //当前date是拿到上面日历组件当前的日期值 根据该值去筛选测试数据找到对应各个日期下对应的数据return出去
  console.log(date, 1111);
  return calendarState.calendarData.filter((item) => {
    return date === item.day;
  });
};

// 工作任务列表
const handleClick = () => {
  var num = 24700000000000 / 510000;
}

const tableData = [
  {
    date: '2016-05-03',
    name: 'Tom',
    state: 'California'

  },
  {
    date: '2016-05-02',
    name: 'Tom',
    state: 'California'
  },
  {
    date: '2016-05-04',
    name: 'Tom',
    state: 'California'
  },
  {
    date: '2016-05-01',
    name: 'Tom',
    state: 'California',
    city: 'Los Angeles'
  }, {
    date: '2016-05-03',
    name: 'Tom',
    state: 'California'

  }, {
    date: '2016-05-03',
    name: 'Tom',
    state: 'California'

  },
  {
    date: '2016-05-02',
    name: 'Tom',
    state: 'California'
  },
  {
    date: '2016-05-04',
    name: 'Tom',
    state: 'California'
  },
  {
    date: '2016-05-01',
    name: 'Tom',
    state: 'California',
    city: 'Los Angeles'
  }, {
    date: '2016-05-03',
    name: 'Tom',
    state: 'California'
  }
]

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


</script>


<style lang="scss" scoped>
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

// .is-selected {
//   color: #1989fa;
// }</style>
<style scoped>
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
</style>
