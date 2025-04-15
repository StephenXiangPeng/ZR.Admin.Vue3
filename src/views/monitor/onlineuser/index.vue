<template>
  <div class="app-container">
    <el-form :model="queryParams" ref="queryRef" :inline="true">
      <el-form-item>
        <el-button plain type="primary" @click="onLockAll()" icon="lock"
          v-hasPermi="['monitor:online:forceLogout']">全部强退</el-button>
      </el-form-item>
      <el-form-item>
        <el-radio-group v-model="viewSwitch">
          <el-radio-button label="1">表格</el-radio-button>
          <el-radio-button label="2">卡片</el-radio-button>
        </el-radio-group>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">刷新</el-button>
      </el-form-item>
    </el-form>
    <!-- <el-row :gutter="10" class="mb8">
      <el-col :span="2"> </el-col>
    </el-row> -->
    <el-table :data="onlineUsers" v-loading="loading" ref="tableRef" border highlight-current-row
      v-if="viewSwitch == 1">
      <el-table-column label="No" type="index" width="50" align="center">
        <template #default="scope">
          <span>{{ (queryParams.pageNum - 1) * queryParams.pageSize + scope.$index + 1 }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="name" label="用户名" align="center" />
      <el-table-column label="登录地点" prop="location" align="center"> </el-table-column>
      <el-table-column label="登录IP" prop="userIP" align="center"></el-table-column>
      <el-table-column prop="browser" label="登录浏览器" width="210"></el-table-column>
      <el-table-column prop="platform" label="登录平台" align="center"></el-table-column>
      <el-table-column prop="loginTime" label="登录时间" witdh="280px">
        <template #default="scope">
          {{ dayjs(scope.row.loginTime).format('MM/DD日HH:mm:ss') }}
          <div>在线时长：{{ scope.row.onlineTime }}分钟</div>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="160">
        <template #default="scope">
          <el-button text @click="onChat(scope.row)" icon="bell" v-hasRole="['admin']">通知</el-button>
          <el-button text @click="onLock(scope.row)" icon="lock"
            v-hasPermi="['monitor:online:forceLogout']">强退</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-row :gutter="20" v-if="viewSwitch == 2">
      <el-col v-for="item in onlineUsers" :lg="4" :span="24">
        <el-card :body-style="{ padding: '15px 15px 0' }">
          <el-descriptions :column="1" :title="item.name">
            <el-descriptions-item label="登录平台">{{ item.platform }}</el-descriptions-item>
            <el-descriptions-item label="登录地点">{{ item.location }}</el-descriptions-item>
            <el-descriptions-item label="在线时长" :span="2">
              <el-tag type="success">{{ item.onlineTime }}分钟</el-tag>
            </el-descriptions-item>
          </el-descriptions>
          <el-text truncated>{{ item.browser }}</el-text>
          <div>
            <el-button text @click="onChat(item)" icon="bell" title="通知" v-hasRole="['admin']"></el-button>
            <el-button text @click="onLock(item)" icon="lock" title="强退"
              v-hasPermi="['monitor:online:forceLogout']"></el-button>
          </div>
        </el-card>
      </el-col>

      <el-empty v-show="total == 0" description="no data" />
    </el-row>
    <pagination :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize"
      @pagination="getList" />
  </div>
</template>

<script setup name="onlineuser">
import { listOnline, forceLogout, forceLogoutAll } from '@/api/monitor/online'
import dayjs from 'dayjs'
import useSocketStore from '@/store/modules/socket'
import request from '@/utils/request'

const { proxy } = getCurrentInstance()
const queryRef = ref(null)
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10
})

const onlineNum = computed(() => {
  return useSocketStore().onlineNum
})
const viewSwitch = ref(1)
const loading = ref(false)
const onlineUsers = ref([])
const total = ref(0)
function handleQuery() {
  queryParams.pageNum = 1
  getList()
}
function getList() {
  loading.value = true
  listOnline(queryParams).then((res) => {
    if (res.code == 200) {
      total.value = res.data.totalNum
      onlineUsers.value = res.data.result
      setTimeout(() => {
        loading.value = false
      }, 200)
    }
  })
}
getList()


// 添加发送通知的 API 函数
const sendNotification = (toUserId, message) => {
  return request({
    url: 'SendMessage/SendNotificationToUsers/SendNotification',
    method: 'GET',
    params: {
      ToUserID: toUserId,
      message: message
    }
  }).then(response => {
    return response;
  })
}
const onChat = (item) => {
  proxy.$prompt('请输入通知内容', '发送通知', {
    confirmButtonText: '发送',
    cancelButtonText: '取消',
    inputPattern: /\S/,
    inputErrorMessage: '消息内容不能为空'
  })
    .then(async ({ value }) => {
      try {
        // 使用 API 接口发送通知
        const res = await sendNotification(item.userid, value);

        if (res.code === 200) {
          proxy.$modal.msgSuccess('通知发送成功');
        } else {
          proxy.$modal.msgError(res.msg || '发送失败');
        }
      } catch (err) {
        console.error('发送通知失败:', err);
        proxy.$modal.msgError('发送失败：' + (err.message || '服务器错误'));
      }
    })
    .catch(() => {
      // 用户取消输入，不做处理
    });
};

function onLock(row) {
  proxy
    .$prompt('请输入强退原因', '', {
      confirmButtonText: '发送',
      cancelButtonText: '取消'
    })
    .then((val) => {
      forceLogout({ ...row, time: 10, reason: val.value, clientId: row.clientId }).then(() => {
        proxy.$modal.msgSuccess('强退成功')
      })
    })
}

// 批量强退
function onLockAll() {
  proxy
    .$prompt('请输入强退原因', '', {
      confirmButtonText: '发送',
      cancelButtonText: '取消'
    })
    .then((val) => {
      forceLogoutAll({ time: 10, reason: val.value }).then((res) => {
        proxy.$modal.msgSuccess('强退成功')
      })
    })
}
watch(
  onlineNum,
  () => {
    handleQuery()
  },
  {
    immediate: true
  }
)
</script>
<style>
.el-col {
  margin-bottom: 10px;
}
</style>
