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
    <el-table :data="tableData">
      <el-table-column fixed prop="date" label="收款单号" style="width: 12%;" />
      <el-table-column prop="name" label="收款日期" style="width: 12%;" />
      <el-table-column prop="state" label="我方公司" style="width: 12%;" />
      <el-table-column prop="state" label="外销币种" style="width: 12%;" />
      <el-table-column prop="state" label="汇率" style="width: 12%;" />
      <el-table-column prop="state" label="金额" style="width: 12%;" />
      <el-table-column prop="state" label="收汇银行" style="width: 12%;" />
      <el-table-column prop="state" label="附件图片" style="width: 12%;" />
      <el-table-column fixed="right" label="Operations" style="width: 12%;">
        <template #default>
          <el-button link type="primary" size="small" @click="handleClick">Detail</el-button>
          <el-button link type="primary" size="small">Edit</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination background layout="prev, pager, next" :total="1000" style="margin-top: 5px;" />



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
              <el-input v-model="contractform.contractNumber" disabled style="width: 300px"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="合同日期">
              <el-date-picker v-model="contractform.contractDate" type="date" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="合同状态">
              <el-select filterable v-model="contractform.contractStatus" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_contract_status" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="客户编号" prop="customerNumber">
              <el-select filterable v-model="contractform.customerNumber" disabled style="width: 300px">
                <el-option v-for="dict in optionss.sql_hr_customer" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户简称" prop="customerAbbreviation">
              <el-select v-model="contractform.customerAbbreviation" filterable disabled style="width: 300px;">
                <el-option v-for="dict in optionss.sql_hr_customer_abbreviation" :key="dict.dictCode"
                  :label="dict.dictLabel" :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="联系人" prop="contactPerson">
              <el-select filterable v-model="contractform.contactPerson" style="width: 300px" disabled>
                <el-option v-for="item in contactpersonSelectOptions" :key="item.value" :label="item.label"
                  :value="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="联系人Email">
              <el-input v-model="contractform.contactEmail" disabled style="width: 300px"></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="生效日期">
              <el-date-picker v-model="contractform.effectiveDate" type="date" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="客户等级">
              <el-select filterable v-model="contractform.customerLevel" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_customer_level" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="客户合同">
              <el-input v-model="contractform.customerContract" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="交货日期" prop="deliveryDate">
              <el-date-picker v-model="contractform.deliveryDate" type="date" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="我方公司" prop="ourCompany">
              <el-select filterable v-model="contractform.ourCompany" style="width: 300px" disabled>
                <el-option v-for="dict in optionss.hr_ourcompany" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="结算类别" prop="settlementType">
              <el-select filterable v-model="contractform.settlementType" disabled style="width: 300px">
                <el-option label="类别1" value="1"></el-option>
                <el-option label="类别2" value="2"></el-option>
                <el-option label="类别3" value="3"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="外销币种" prop="foreignCurrency">
              <el-select filterable v-model="contractform.foreignCurrency" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="汇率" prop="exchangeRate">
              <el-input v-model="contractform.exchangeRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="美金汇率" prop="usdExchangeRate">
              <el-input v-model="contractform.usdExchangeRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="结汇方式" prop="settlementMethod">
              <el-select filterable v-model="contractform.settlementMethod" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_settlement_way" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="价格条款" prop="priceTerms">
              <el-select filterable v-model="contractform.priceTerms" style="width: 300px" disabled>
                <el-option v-for="dict in optionss.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="出运口岸" prop="shippingPort">
              <el-select filterable v-model="contractform.shippingPort" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="目的口岸" prop="destinationPort">
              <el-select filterable v-model="contractform.destinationPort" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_transport_port" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="贸易国别" prop="tradeCountry">
              <el-select filterable v-model="contractform.tradeCountry" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_nation" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="运输方式" prop="transportation">
              <el-select filterable v-model="contractform.transportation" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_transportation_method" :key="dict.dictCode"
                  :label="dict.dictLabel" :value="dict.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="销售员" prop="salesperson">
              <el-select filterable v-model="contractform.salesperson" style="width: 300px" disabled>
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
              <el-date-picker v-model="contractform.depositDate" type="date" disabled
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
          <el-col :span="8">
            <el-form-item label="签约地点">
              <el-select filterable v-model="contractform.signingLocation" disabled style="width: 300px">
                <el-option v-for="dict in optionss.hr_signing_place" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="可否分批">
              <el-select filterable v-model="contractform.canPartial" disabled style="width: 300px">
                <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="可否转运">
              <el-select filterable v-model="contractform.canTransit" disabled style="width: 300px">
                <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                  :value="dict.dictValue"></el-option>
              </el-select>
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
            <el-form-item label="佣金比率">
              <el-input v-model="contractform.commissionRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="收汇银行">
              <el-input v-model="contractform.receivingBank" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="付款日期">
              <el-date-picker v-model="contractform.paymentDate" type="date" disabled
                style="width: 300px"></el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">产品资料&客户相关费用</span>
        <el-divider></el-divider>
        <el-tabs v-model="activeTab" tab-position="top" style="height: 350px; " class="demo-tabs">
          <el-tab-pane label="产品资料" name="productMaterialtab">
            <el-table :data="NewcontractProductTbaleData" style="width: 100%" height="280">
              <el-table-column prop="productCode" label="产品编号" width="120"></el-table-column>
              <el-table-column prop="customerCode" label="客户货号" width="120"></el-table-column>
              <el-table-column prop="chineseName" label="中文品名" width="120"></el-table-column>
              <el-table-column prop="chineseSpec" label="中文规格" width="120"></el-table-column>
              <el-table-column prop="englishName" label="英文品名" width="120"></el-table-column>
              <el-table-column prop="contractQuantity" label="合同数量" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.contractQuantity" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="unit" label="计量单位" width="120">
                <template #default="scope">
                  <el-select v-model="scope.row.unit" filterable style="width: 100%;">
                    <el-option v-for="dict in optionss.hr_calculate_unit" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="exportUnitPrice" label="外销单价" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.exportUnitPrice" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="exportTotalPrice" label="外销总价" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.exportTotalPrice" disabled />
                </template>
              </el-table-column>
              <el-table-column prop="purchaseUnitPrice" label="采购单价" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.purchaseUnitPrice" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="purchaseTotalPrice" label="采购总价" width="120"></el-table-column>
              <el-table-column prop="packaging" label="包装方式" width="150">
                <template #default="scope">
                  <el-select v-model="scope.row.packaging" filterable style="width: 100%;">
                    <el-option v-for="dict in optionss.hr_packing" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="specialRequirements" label="特殊要求" width="200">
                <template #default="scope">
                  <el-input v-model="scope.row.specialRequirements" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="invoice" label="是否开票" width="120">
                <template #default="scope">
                  <el-select v-model="scope.row.invoice" filterable style="width: 100%;">
                    <el-option v-for="dict in optionss.sys_yes_no" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="taxRefundRate" label="退税率(%)" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.taxRefundRate" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="innerBoxQuantity" label="内盒装量" width="120"><template #default="scope">
                  <el-input v-model="scope.row.innerBoxQuantity" size="small" />
                </template>
              </el-table-column>
              <el-table-column prop="outerBoxQuantity" label="外箱装量" width="120"><template #default="scope">
                  <el-input v-model="scope.row.outerBoxQuantity" size="small" />
                </template></el-table-column>
              <el-table-column prop="boxCount" label="箱数" width="120"><template #default="scope">
                  <el-input v-model="scope.row.boxCount" size="small" />
                </template></el-table-column>
              <el-table-column prop="outerBoxUnit" label="外箱单位" width="150">
                <template #default="scope">
                  <el-select v-model="scope.row.outerBoxUnit" filterable style="width: 100%;" disabled>
                    <el-option v-for="dict in optionss.hr_outerbox_unit" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="outerBoxLength" label="外箱长度(CM)" width="120"><template #default="scope">
                  <el-input v-model="scope.row.outerBoxLength" size="small" />
                </template></el-table-column>
              <el-table-column prop="outerBoxWidth" label="外箱宽度(CM)" width="120"><template #default="scope">
                  <el-input v-model="scope.row.outerBoxWidth" size="small" />
                </template></el-table-column>
              <el-table-column prop="outerBoxHeight" label="外箱高度(CM)" width="120"><template #default="scope">
                  <el-input v-model="scope.row.outerBoxHeight" size="small" />
                </template></el-table-column>
              <el-table-column prop="outerBoxVolume" label="外箱体积(m³)" width="120"></el-table-column>
              <el-table-column prop="outerBoxNetWeight" label="外箱净重" width="120">
                <template #default="scope">
                  <el-input v-model="scope.row.outerBoxNetWeight" size="small" />
                </template></el-table-column>
              <el-table-column prop="outerBoxGrossWeight" label="外箱毛重" width="120"><template #default="scope">
                  <el-input v-model="scope.row.outerBoxGrossWeight" size="small" />
                </template></el-table-column>
              <el-table-column prop="totalVolume" label="总体积" width="120"></el-table-column>
              <el-table-column prop="totalNetWeight" label="总净重" width="120"></el-table-column>
              <el-table-column prop="totalGrossWeight" label="总毛重" width="120"></el-table-column>
            </el-table>
          </el-tab-pane>
          <el-tab-pane label="客户相关费用" name="CustomerRelaterExoensestab">
            <el-table :data="CustomerRelaterExoensesTableData" style="width: 100%; " height="280">
              <el-table-column prop=" expenseName" label="费用名称" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.expenseName" size="small"></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="currency" label="币种" width="150">
                <template #default="{ row }">
                  <el-select filterable v-model="row.currency" size="small">
                    <el-option v-for="dict in optionss.hr_export_currency" :key="dict.dictCode" :label="dict.dictLabel"
                      :value="dict.dictValue" />
                  </el-select>
                </template>
              </el-table-column>
              <el-table-column prop="exchangeRate" label="汇率" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.exchangeRate" size="small"></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="expense" label="费用" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.expense" size="small"></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="amount" label="金额" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.amount" disabled size="small"></el-input>
                </template>
              </el-table-column>
              <el-table-column prop="remark" label="备注" width="150">
                <template #default="{ row }">
                  <el-input v-model="row.remark" size="small"></el-input>
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
              <el-input v-model="contractform.goodsValue" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="数量合计">
              <el-input v-model="contractform.quantity" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="箱数合计">
              <el-input v-model="contractform.boxCount" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="毛重合计">
              <el-input v-model="contractform.grossWeight" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="净重合计">
              <el-input v-model="contractform.netWeight" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="体积合计">
              <el-input v-model="contractform.volume" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="采购合计">
              <el-input v-model="contractform.purchaseTotal" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="退税总额">
              <el-input v-model="contractform.taxRefundTotal" style="width: 300px" disabled></el-input>
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
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">利润预估</span>
        <el-divider></el-divider>
        <el-row>
          <el-col :span="8">
            <el-form-item label="佣金金额">
              <el-input v-model="contractform.commissionAmount" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="海运费币种">
              <el-input v-model="contractform.shippingCurrency" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="海运费汇率">
              <el-input v-model="contractform.shippingExchangeRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="海运费">
              <el-input v-model="contractform.shippingCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="保险费用">
              <el-input v-model="contractform.insuranceCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="银行费用">
              <el-input v-model="contractform.bankCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="其它费用">
              <el-input v-model="contractform.otherCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="运杂费">
              <el-input v-model="contractform.freightCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="成本总额">
              <el-input v-model="contractform.totalCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="换汇成本">
              <el-input v-model="contractform.exchangeCost" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="产品毛利">
              <el-input v-model="contractform.productProfit" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="利润金额">
              <el-input v-model="contractform.profitAmount" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="利润率">
              <el-input v-model="contractform.profitRate" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="运杂费币种">
              <el-input v-model="contractform.FreightCurrency" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="运杂费报价">
              <el-input v-model="contractform.FreightQuote" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="8">
            <el-form-item label="快递费">
              <el-input v-model="contractform.Courierfee" style="width: 300px" disabled></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <span style="font-size: 20px; font-weight: bold;">备注信息</span>
        <el-divider></el-divider>
        <el-input v-model="contractform.contractremark" :autosize="{ minRows: 5, maxRows: 10 }" type="textarea"
          disabled />
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
  </div>
</template>

<script lang="ts" setup>
import { getCurrentInstance, reactive, toRefs, ref } from 'vue'
import { ElMessage, ElMessageBox } from "element-plus";
import request from '@/utils/request';
import dayjs from 'dayjs';
import useUserStore from '@/store/modules/user'
import useSocketStore from '@/store/modules/socket'

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
  id: 0,
  contractNumber: '',
  contractDate: '',
  contractStatus: null,
  customerid: null,
  customerNumber: '',
  customerAbbreviation: '',
  contactPerson: null,
  contactEmail: '',
  effectiveDate: '',
  customerLevel: null,
  customerContract: '',
  deliveryDate: '',
  ourCompany: null,
  settlementType: null,
  foreignCurrency: null,
  exchangeRate: null,
  usdExchangeRate: null,
  settlementMethod: null,
  priceTerms: null,
  shippingPort: null,
  destinationPort: null,
  tradeCountry: null,
  transportation: null,
  salesperson: null,
  hasDeposit: false,
  receivedDeposit: null,
  depositDate: '',
  Depositratio: null,
  stockProgress: '',
  deliveryProgress: '',
  profitCalculation: '',
  estimatedProfitMargin: '',
  customerOrder: '',
  contractremark: '',
  signingLocation: null,
  canPartial: null,
  canTransit: null,
  insuranceAddition: null,
  insuranceRate: null,
  commissionRate: null,
  receivingBank: '',
  paymentDate: '',
  goodsValue: null,
  quantity: null,
  boxCount: null,
  grossWeight: null,
  netWeight: null,
  volume: null,
  purchaseTotal: null,
  taxRefundTotal: null,
  customerExpenseTotal: null,
  amountTotal: null,
  usdConversion: null,
  commissionAmount: null,
  shippingCurrency: null,
  shippingExchangeRate: null,
  shippingCost: null,
  insuranceCost: null,
  bankCost: null,
  otherCost: null,
  freightCost: null,
  totalCost: null,
  exchangeCost: null,
  productProfit: null,
  profitAmount: null,
  profitRate: null,
  FreightCurrency: null,
  FreightQuote: null,
  Courierfee: null
});

// 时间插件
import duration from 'dayjs/plugin/duration'
import { number } from 'echarts';
import SalesContractdialog from '@/views/components/SalesContractdialog.vue';
dayjs.extend(duration)
dayjs.locale('zh-cn')

const openSaleContractDialog = (row) => {
  ApproveDocumentRequest.ApprovalRecordID = row.recordID;
  ApproveDocumentRequest.DocumentID = row.documentID;
  ApproveDocumentRequest.StageID = row.stageID;
  ApproveDocumentRequest.ApproverID = row.approverID;
  ApproveDocumentRequest.DocumentType = row.documentType;
  request({
    url: 'Contracts/GetContractDetailsById/GetContractDetails',
    method: 'GET',
    params: {
      contractId: row.documentID
    }
  }).then(response => {
    if (response.data != null) {
      contractform.contractNumber = response.data.contract.contractNumber;
      contractform.contractDate = response.data.contract.contractDate;
      contractform.contractStatus = state.optionss['hr_contract_status'].filter(item => item.dictValue == response.data.contract.contractStatus).map(item => item.dictLabel).values().next().value;
      contractform.customerid = response.data.contract.customerid;
      contractform.customerNumber = response.data.contract.customerNumber;
      contractform.customerAbbreviation = response.data.contract.customerAbbreviation;
      contractform.contactPerson = state.optionss['sql_hr_customer_contactperson'].filter(item => item.dictValue == response.data.contract.contactPerson).map(item => item.dictLabel).values().next().value;
      contractform.contactEmail = response.data.contract.contactEmail;
      contractform.effectiveDate = response.data.contract.effectiveDate;
      contractform.customerLevel = state.optionss['hr_customer_level'].filter(item => item.dictValue == response.data.contract.customerLevel).map(item => item.dictLabel).values().next().value;
      contractform.customerContract = response.data.contract.customerContract;
      contractform.deliveryDate = response.data.contract.deliveryDate;
      contractform.ourCompany = state.optionss['hr_ourcompany'].filter(item => item.dictValue == response.data.contract.ourCompany).map(item => item.dictLabel).values().next().value;
      contractform.settlementType = response.data.contract.settlementType;
      contractform.foreignCurrency = response.data.contract.foreignCurrency;
      contractform.exchangeRate = response.data.contract.exchangeRate;
      contractform.usdExchangeRate = response.data.contract.usdExchangeRate;
      contractform.settlementMethod = state.optionss['hr_settlement_way'].filter(item => item.dictValue == response.data.contract.settlementMethod).map(item => item.dictLabel).values().next().value;
      contractform.priceTerms = state.optionss['hr_pricing_term'].filter(item => item.dictValue == response.data.contract.priceTerms).map(item => item.dictLabel).values().next().value;
      contractform.shippingPort = state.optionss['hr_transport_port'].filter(item => item.dictValue == response.data.contract.shippingPort).map(item => item.dictLabel).values().next().value;
      contractform.destinationPort = state.optionss['hr_transport_port'].filter(item => item.dictValue == response.data.contract.destinationPort).map(item => item.dictLabel).values().next().value;
      contractform.tradeCountry = state.optionss['hr_nation'].filter(item => item.dictValue == response.data.contract.tradeCountry).map(item => item.dictLabel).values().next().value;
      contractform.transportation = state.optionss['hr_transportation_method'].filter(item => item.dictValue == response.data.contract.transportation).map(item => item.dictLabel).values().next().value;
      contractform.salesperson = state.optionss['sql_all_user'].filter(item => item.dictValue == response.data.contract.salesperson).map(item => item.dictLabel).values().next().value;
      contractform.hasDeposit = state.optionss['sys_yes_no'].filter(item => item.dictValue == response.data.contract.hasDeposit).map(item => item.dictLabel).values().next().value;
      contractform.receivedDeposit = response.data.contract.receivedDeposit;
      contractform.depositDate = response.data.contract.depositDate;
      contractform.Depositratio = response.data.contract.Depositratio;
      contractform.stockProgress = response.data.contract.stockProgress;
      contractform.deliveryProgress = response.data.contract.deliveryProgress;
      contractform.profitCalculation = response.data.contract.profitCalculation;
      contractform.estimatedProfitMargin = response.data.contract.estimatedProfitMargin;
      contractform.customerOrder = response.data.contract.customerOrder;
      contractform.contractremark = response.data.contract.contractremark;
      contractform.signingLocation = response.data.contract.signingLocation;
      contractform.canPartial = response.data.contract.canPartial;
      contractform.canTransit = response.data.contract.canTransit;
      contractform.insuranceAddition = response.data.contract.insuranceAddition;
      contractform.insuranceRate = response.data.contract.insuranceRate;
      contractform.commissionRate = response.data.contract.commissionRate;
      contractform.receivingBank = response.data.contract.receivingBank;
      contractform.paymentDate = response.data.contract.paymentDate;
      contractform.goodsValue = response.data.contract.goodsValue;
      contractform.quantity = response.data.contract.quantity;
      contractform.boxCount = response.data.contract.boxCount;
      contractform.grossWeight = response.data.contract.grossWeight;
      contractform.netWeight = response.data.contract.netWeight;
      contractform.volume = response.data.contract.volume;
      contractform.purchaseTotal = response.data.contract.purchaseTotal;
      contractform.taxRefundTotal = response.data.contract.taxRefundTotal;
      contractform.customerExpenseTotal = response.data.contract.customerExpenseTotal;
      contractform.amountTotal = response.data.contract.amountTotal;
      contractform.usdConversion = response.data.contract.usdConversion;
      contractform.commissionAmount = response.data.contract.commissionAmount;
      contractform.shippingCurrency = response.data.contract.shippingCurrency;
      contractform.shippingExchangeRate = response.data.contract.shippingExchangeRate;
      contractform.shippingCost = response.data.contract.shippingCost;
      contractform.insuranceCost = response.data.contract.insuranceCost;
      contractform.bankCost = response.data.contract.bankCost;
      contractform.otherCost = response.data.contract.otherCost;
      contractform.freightCost = response.data.contract.freightCost;
      contractform.totalCost = response.data.contract.totalCost;
      contractform.exchangeCost = response.data.contract.exchangeCost;
      contractform.productProfit = response.data.contract.productProfit;
      contractform.profitAmount = response.data.contract.profitAmount;
      contractform.profitRate = response.data.contract.profitRate;
      contractform.FreightCurrency = response.data.contract.FreightCurrency;
      contractform.FreightQuote = response.data.contract.FreightQuote;
      contractform.Courierfee = response.data.contract.Courierfee;
    }
  }).catch(error => {
    console.error(error);
  });
  contractDialog.value = true;
  contractform.id = id;
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
    sql_hr_customer_contactperson: []
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
  { dictType: 'sql_hr_customer_contactperson' }
]
proxy.getDicts(dictParams).then((response) => {
  response.data.forEach((element) => {
    state.optionss[element.dictType] = element.list
  })
  getPendingCount();
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
          item.documentNumber = state.optionss['sql_sale_contracts'].filter(user => user.dictValue == item.documentType).map(user => user.dictLabel).values().next().value;
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
  alert(num)
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

</script>


<style lang="scss" scoped>
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
