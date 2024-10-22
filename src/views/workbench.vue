<template>
  <div class="home">


    <!-- 任务看板 -->
    <el-row :gutter="15" style="font-size: 25px;">
      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%; border: 0px;" shadow="never">
          <div class="text-warning mb10">代办流程</div>
          <el-divider></el-divider>
          <div class="work-wrap" style="font-size: 25px;">
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
            <span style="font-weight: bold; font-size: 30px;">&nbsp;&nbsp;&nbsp;10</span>
            <span style="color: red; font-weight: bold;font-size: 30px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;8</span>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="8" class="mb10">
        <el-card style="height: 100%; border: 0px;" shadow="never">
          <div class="text-warning mb10">业务示警</div>
          <el-divider></el-divider>
          <div class="work-wrap2">
            <!-- <el-statistic title="交货逾期" value="10" />
            <el-statistic title="货款逾期" value="10" />
            <el-statistic title="沟通逾期" value="10" /> -->
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


  </div>
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

  <el-row :gutter="15" style=" margin-top: 30px;">
    <el-col :lg="8" class="mb10">
      <el-card style="height: 100%">
        <div class="text-warning mb10">国际时区&nbsp;&nbsp;&nbsp;&nbsp;
          <el-select v-model="value" class="m-2" placeholder="请选择" size="small">
            <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
        </div>
        <el-divider />
        <div class="work-wrap" style="margin-top: 10px;">
          <el-text class="mx-1" type="warning" size="large" style="color: black;font-size: 15px;">11月29日 周三
            20:55:00</el-text>
        </div>
      </el-card>
    </el-col>
    <el-col :lg="8" class="mb10">
      <el-card style="height: 100%">
        <div class="text-warning mb10">实时汇率&nbsp;&nbsp;&nbsp;&nbsp;

          <el-select v-model="value2" class="m-2" placeholder="请选择" size="small">
            <el-option v-for="item in options2" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
          --
          <el-select v-model="value3" class="m-2" placeholder="请选择" size="small">
            <el-option v-for="item in options3" :key="item.value" :label="item.label" :value="item.value" />
          </el-select>
        </div>
        <el-divider />
        <div class="work-wrap" style="margin-top: 10px;">
          <el-text class="mx-1" type="warning" size="large"
            style="color: black; font-size: 15px;">实时汇率：100&nbsp;&nbsp;&nbsp;&nbsp;=&nbsp;&nbsp;&nbsp;&nbsp;732&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</el-text>
          <el-text class="mx-1" type="warning" size="large"
            style="color: black;font-size: 15px;">&nbsp;现汇买入价：&nbsp;732</el-text>
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
</template>

<script lang="ts" setup name="index">
import PanelGroup from './dashboard/PanelGroup'
import LineChart from './dashboard/LineChart'
import RaddarChart from './dashboard/RaddarChart'
import PieChart from './dashboard/PieChart'
import BarChart from './dashboard/BarChart'
// import WordCloudChat from './dashboard/WordCloud.vue'
import CommonMenu from './components/CommonMenu'
import dayjs from 'dayjs'
// 时间插件
import duration from 'dayjs/plugin/duration'
dayjs.extend(duration)

import useUserStore from '@/store/modules/user'
import useSocketStore from '@/store/modules/socket'
import { getWeek } from '@/utils/ruoyi'

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
const { proxy } = getCurrentInstance()
const userInfo = computed(() => {
  return useUserStore().userInfo
})
const onlineInfo = computed(() => {
  return useSocketStore().onlineInfo
})
// const currentTime = computed(() => {
//   return proxy.parseTime(new Date(), 'YYYY-MM-DD')
// })
// const weekName = getWeek()

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

import { ref, reactive, toRefs, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";

const state = reactive({
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
  return state.calendarData.filter((item) => {
    return date === item.day;
  });
};


// 工作任务列表
const handleClick = () => {
  alert('click')
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
// }
</style>
<style scoped >
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


