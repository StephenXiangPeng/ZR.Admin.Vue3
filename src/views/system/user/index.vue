<template>
  <div class="app-container">
    <el-row :gutter="20">
      <!--部门数据-->
      <el-col :span="4" :xs="24">
        <div class="head-container">
          <el-input v-model="deptName" placeholder="请输入部门名称" clearable prefix-icon="search" style="margin-bottom: 20px" />
        </div>
        <div class="head-container">
          <el-tree
            :data="deptOptions"
            :props="{ label: 'label', children: 'children' }"
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            ref="deptTreeRef"
            node-key="id"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick">
            <template #default="{ node, data }">
              <span class="custom-tree-node">
                <span>
                  <svg-icon name="index" v-if="data.children && data.children.length > 0"></svg-icon>
                  {{ node.label }}
                </span>
              </span>
            </template>
          </el-tree>
        </div>
      </el-col>
      <!--用户数据-->
      <el-col :lg="20" :xm="24">
        <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
          <el-form-item label="用户名称" prop="userName">
            <el-input v-model="queryParams.userName" placeholder="请输入用户名称" clearable style="width: 240px" @keyup.enter="handleQuery" />
          </el-form-item>
          <el-form-item label="手机号码" prop="phonenumber">
            <el-input v-model="queryParams.phonenumber" placeholder="请输入手机号码" clearable style="width: 240px" @keyup.enter="handleQuery" />
          </el-form-item>
          <el-form-item label="状态" prop="status">
            <el-select v-model="queryParams.status" placeholder="用户状态" clearable style="width: 240px">
              <el-option label="全部" :value="-1" />
              <el-option v-for="dict in statusOptions" :key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
            </el-select>
          </el-form-item>
          <el-form-item label="创建时间">
            <el-date-picker
              v-model="dateRange"
              style="width: 240px"
              type="daterange"
              range-separator="-"
              start-placeholder="开始日期"
              end-placeholder="结束日期"></el-date-picker>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" icon="search" @click="handleQuery">{{ $t('btn.search') }}</el-button>
            <el-button icon="refresh" @click="resetQuery">{{ $t('btn.reset') }}</el-button>
          </el-form-item>
        </el-form>

        <el-row :gutter="10" class="mb8">
          <el-col :span="1.5">
            <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['system:user:add']">
              {{ $t('btn.add') }}
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['system:user:edit']">
              {{ $t('btn.edit') }}
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['system:user:remove']">
              {{ $t('btn.delete') }}
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button type="info" plain icon="Upload" @click="handleImport" v-hasPermi="['system:user:import']">
              {{ $t('btn.import') }}
            </el-button>
          </el-col>
          <el-col :span="1.5">
            <el-button type="warning" plain icon="Download" @click="handleExport" v-hasPermi="['system:user:export']">
              {{ $t('btn.export') }}
            </el-button>
          </el-col>

          <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" :data="userList" @selection-change="handleSelectionChange">
          <el-table-column type="selection" width="50" align="center" :selectable="checkSelectable" />
          <el-table-column label="用户编号" align="center" key="userId" prop="userId" v-if="columns.showColumn('userId')" />
          <el-table-column
            label="登录名"
            align="center"
            key="userName"
            prop="userName"
            v-if="columns.showColumn('userName')"
            :show-overflow-tooltip="true" />
          <el-table-column
            label="用户昵称"
            align="center"
            key="nickName"
            prop="nickName"
            v-if="columns.showColumn('nickName')"
            :show-overflow-tooltip="true" />
          <el-table-column
            label="部门"
            align="center"
            key="deptName"
            prop="deptName"
            v-if="columns.showColumn('deptName')"
            :show-overflow-tooltip="true" />
          <el-table-column
            label="手机号码"
            align="center"
            key="phonenumber"
            prop="phonenumber"
            v-if="columns.showColumn('phonenumber')"
            width="120" />
          <el-table-column label="启用" align="center" key="status" v-if="columns.showColumn('status')">
            <template #default="scope">
              <el-switch
                v-model="scope.row.status"
                :active-value="0"
                :inactive-value="1"
                active-text="是"
                inactive-text="否"
                inline-prompt
                @change="handleStatusChange(scope.row)"></el-switch>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns.showColumn('createTime')" width="160"></el-table-column>
          <el-table-column prop="sex" label="性别" align="center" v-if="columns.showColumn('sex')">
            <template #default="scope">
              <dict-tag :options="sexOptions" :value="scope.row.sex" />
            </template>
          </el-table-column>
          <el-table-column prop="avatar" label="头像" align="center" v-if="columns.showColumn('avatar')">
            <template #default="scope">
              <el-avatar :src="scope.row.avatar" />
            </template>
          </el-table-column>
          <el-table-column prop="email" label="用户邮箱" align="center" v-if="columns.showColumn('email')" />
          <el-table-column prop="loginDate" label="最后登录时间" align="center" v-if="columns.showColumn('loginDate')" />
          <el-table-column label="操作" align="left" width="110" class-name="small-padding fixed-width">
            <template #default="scope">
              <el-button text icon="Edit" v-if="scope.row.userId !== 1" @click="handleUpdate(scope.row)" v-hasPermi="['system:user:edit']">
              </el-button>
              <el-button v-if="!scope.row.isAdmin" text icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['system:user:remove']">
              </el-button>
              <el-button
                v-if="scope.row.userId !== 1"
                text
                icon="Key"
                title="重置密码"
                @click="handleResetPwd(scope.row)"
                v-hasPermi="['system:user:resetPwd']"></el-button>
            </template>
          </el-table-column>
        </el-table>
        <pagination :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />
      </el-col>
    </el-row>

    <!-- 添加或修改用户配置对话框 -->
    <el-dialog :title="title" v-model="open" width="600px" append-to-body>
      <el-form :model="form" :rules="rules" ref="userRef" label-width="80px">
        <el-row :gutter="20">
          <el-col :lg="12">
            <el-form-item label="用户名" prop="userName">
              <el-input :disabled="form.userId != undefined" v-model="form.userName" placeholder="请输入用户名(用于登陆)" />
            </el-form-item>
          </el-col>
          <el-col :lg="12" v-if="form.userId == undefined">
            <el-form-item label="用户密码" prop="password">
              <el-input v-model="form.password" show-password placeholder="请输入用户密码" type="password" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="用户昵称" prop="nickName">
              <el-input v-model="form.nickName" placeholder="请输入用户昵称" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="归属部门" prop="deptId">
              <el-tree-select
                v-model="form.deptId"
                :data="deptOptions"
                :props="{ value: 'id', label: 'label', children: 'children' }"
                value-key="id"
                placeholder="请选择归属部门"
                check-strictly
                :render-after-expand="false" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="手机号码" prop="phonenumber">
              <el-input v-model="form.phonenumber" placeholder="请输入手机号码" maxlength="11" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="form.email" placeholder="请输入邮箱" maxlength="50" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="用户性别">
              <el-radio-group v-model="form.sex" placeholder="请选择用户性别">
                <el-radio-button v-for="dict in sexOptions" :key="dict.dictValue" :value="parseInt(dict.dictValue)">{{
                  dict.dictLabel
                }}</el-radio-button>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="用户状态">
              <el-radio-group v-model="form.status">
                <el-radio-button v-for="dict in statusOptions" :key="dict.dictValue" :value="parseInt(dict.dictValue)">{{
                  dict.dictLabel
                }}</el-radio-button>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="岗位">
              <el-select v-model="form.postIds" multiple placeholder="请选择岗位" style="width: 100%">
                <el-option v-for="item in postOptions" :key="item.postId" :label="item.postName" :value="item.postId" :disabled="item.status == 1">
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="角色">
              <el-select v-model="form.roleIds" multiple placeholder="请选择角色" style="width: 100%" @change="selectRole($event)">
                <el-option
                  v-for="item in roleOptions"
                  :key="item.roleId"
                  :label="item.roleName"
                  :value="item.roleId"
                  :disabled="item.status == 1 || form.userId == 1">
                  <span style="float: left">{{ item.roleName }}</span>
                  <span style="float: right">{{ item.roleKey }}</span>
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="备注">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button text @click="cancel">{{ $t('btn.cancel') }}</el-button>
        <el-button type="primary" @click="submitForm">{{ $t('btn.submit') }}</el-button>
      </template>
    </el-dialog>

    <!-- 用户导入对话框 -->
    <el-dialog :title="upload.title" v-model="upload.open" width="400px" append-to-body>
      <el-upload
        name="file"
        ref="uploadRef"
        :limit="1"
        accept=".xlsx,.xls"
        :headers="upload.headers"
        :action="upload.url + '?updateSupport=' + upload.updateSupport"
        :disabled="upload.isUploading"
        :on-progress="handleFileUploadProgress"
        :on-success="handleFileSuccess"
        :auto-upload="false"
        drag>
        <el-icon class="el-icon--upload">
          <upload-filled />
        </el-icon>
        <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
        <template #tip>
          <div class="el-upload__tip text-center">
            <!-- <div class="el-upload__tip">
              <el-checkbox v-model="upload.updateSupport" /> 是否更新已经存在的用户数据
            </div> -->
            <span>仅允许导入xls、xlsx格式文件。</span>
            <el-link type="primary" :underline="false" style="font-size: 12px; vertical-align: baseline" @click="importTemplate">下载模板</el-link>
          </div>
        </template>
      </el-upload>
      <template #footer>
        <el-button @click="upload.open = false">{{ $t('btn.cancel') }}</el-button>
        <el-button type="primary" @click="submitFileForm">{{ $t('btn.submit') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="user">
import { getToken } from '@/utils/auth'
import { treeselect } from '@/api/system/dept'
import { changeUserStatus, listUser, resetUserPwd, delUser, getUser, updateUser, addUser, exportUser } from '@/api/system/user'

const { proxy } = getCurrentInstance()

const statusOptions = ref([])
const sexOptions = ref([])
proxy.getDicts('sys_normal_disable').then((response) => {
  statusOptions.value = response.data
})
proxy.getDicts('sys_user_sex').then((response) => {
  sexOptions.value = response.data
})
const userList = ref([])
const open = ref(false)
const loading = ref(true)
const showSearch = ref(true)
const ids = ref([])
const single = ref(true)
const multiple = ref(true)
const total = ref(0)
const title = ref('')
const dateRange = ref([])
const deptName = ref('')
const deptOptions = ref([])
const initPassword = ref(undefined)
const postOptions = ref([])
const roleOptions = ref([])
/*** 用户导入参数 */
const upload = reactive({
  // 是否显示弹出层（用户导入）
  open: false,
  // 弹出层标题（用户导入）
  title: '',
  // 是否禁用上传
  isUploading: false,
  // 是否更新已经存在的用户数据
  updateSupport: 0,
  // 设置上传的请求头部
  headers: { Authorization: 'Bearer ' + getToken() },
  // 上传的地址
  url: import.meta.env.VITE_APP_BASE_API + '/system/user/importData'
})
// 列显隐信息
const columns = ref([
  { key: 0, label: `用户编号`, visible: true, prop: 'userId' },
  { key: 1, label: `用户名称`, visible: true, prop: 'userName' },
  { key: 2, label: `用户昵称`, visible: true, prop: 'nickName' },
  { key: 3, label: `部门`, visible: true, prop: 'deptName' },
  { key: 4, label: `手机号码`, visible: true, prop: 'phonenumber' },
  { key: 5, label: `状态`, visible: true, prop: 'status' },
  { key: 6, label: `创建时间`, visible: false, prop: 'createTime' },
  { key: 7, label: `性别`, visible: true, prop: 'sex' },
  { key: 8, label: `头像`, visible: true, prop: 'avatar' },
  { key: 9, label: `邮箱`, visible: false, prop: 'email' },
  { key: 10, label: `最后登录时间`, visible: false, prop: 'loginDate' }
])

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    userName: undefined,
    phonenumber: undefined,
    status: -1,
    deptId: undefined
  },
  rules: {
    userName: [
      { required: true, message: '用户名称不能为空', trigger: 'blur' },
      {
        min: 2,
        max: 20,
        message: '用户名称长度必须介于 2 和 20 之间',
        trigger: 'blur'
      }
    ],
    nickName: [{ required: true, message: '用户昵称不能为空', trigger: 'blur' }],
    password: [
      { required: true, message: '用户密码不能为空', trigger: 'blur' },
      {
        min: 5,
        max: 20,
        message: '用户密码长度必须介于 5 和 20 之间',
        trigger: 'blur'
      }
    ],
    email: [
      {
        required: true,
        type: 'email',
        message: '请输入正确的邮箱地址',
        trigger: ['blur', 'change']
      }
    ],
    phonenumber: [
      {
        pattern: /^1[3|4|5|6|7|8|9][0-9]\d{8}$/,
        message: '请输入正确的手机号码',
        trigger: 'blur'
      }
    ]
  }
})

const { queryParams, form, rules } = toRefs(data)

proxy.getConfigKey('sys.user.initPassword').then((response) => {
  initPassword.value = response.data
})

/** 通过条件过滤节点  */
const filterNode = (value, data) => {
  if (!value) return true
  return data.label.indexOf(value) !== -1
}
/** 根据名称筛选部门树 */
watch(deptName, (val) => {
  proxy.$refs['deptTreeRef'].filter(val)
})
/** 查询部门下拉树结构 */
function getTreeselect() {
  treeselect().then((response) => {
    deptOptions.value = [{ id: 0, label: '未知部门', children: [] }, ...response.data]
  })
}
/** 查询用户列表 */
function getList() {
  loading.value = true
  listUser(proxy.addDateRange(queryParams.value, dateRange.value)).then((res) => {
    loading.value = false
    userList.value = res.data.result
    total.value = res.data.totalNum
  })
}
/** 节点单击事件 */
function handleNodeClick(data) {
  queryParams.value.deptId = data.id
  handleQuery()
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1
  getList()
}
/** 重置按钮操作 */
function resetQuery() {
  dateRange.value = []
  proxy.resetForm('queryRef')
  queryParams.value.deptId = undefined
  proxy.$refs.deptTreeRef.setCurrentKey(null)
  handleQuery()
}
/** 删除按钮操作 */
function handleDelete(row) {
  const userIds = row.userId || ids.value
  proxy.$modal
    .confirm('是否确认删除用户编号为"' + userIds + '"的数据项？')
    .then(function () {
      return delUser(userIds)
    })
    .then(() => {
      getList()
      proxy.$modal.msgSuccess('删除成功')
    })
    .catch(() => {})
}
/** 导出按钮操作 */
function handleExport() {
  proxy.$modal
    .confirm('是否确认导出所有用户数据项?', '警告', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    .then(async () => {
      await exportUser(queryParams.value)
    })
}
/** 用户状态修改  */
function handleStatusChange(row) {
  let text = row.status === '0' ? '启用' : '停用'
  proxy.$modal
    .confirm('确认要"' + text + '""' + row.userName + '"用户吗?')
    .then(function () {
      return changeUserStatus(row.userId, row.status)
    })
    .then(() => {
      proxy.$modal.msgSuccess(text + '成功')
    })
    .catch(function () {
      row.status = row.status == 1 ? 0 : 1
    })
}
/** 重置密码按钮操作 */
function handleResetPwd(row) {
  proxy
    .$prompt('请输入"' + row.userName + '"的新密码', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      closeOnClickModal: false,
      inputPattern: /^.{5,20}$/,
      inputErrorMessage: '用户密码长度必须介于 5 和 20 之间'
    })
    .then(({ value }) => {
      resetUserPwd(row.userId, value).then((response) => {
        proxy.$modal.msgSuccess('修改成功，新密码是：' + value)
      })
    })
    .catch(() => {})
}
/** 选择条数  */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.userId)
  single.value = selection.length != 1
  multiple.value = !selection.length
}
/** 导入按钮操作 */
function handleImport() {
  upload.title = '用户导入'
  upload.open = true
}
/** 下载模板操作 */
function importTemplate() {
  proxy.download('/system/user/importTemplate', '用户数据导入模板')
}
/**文件上传中处理 */
const handleFileUploadProgress = (event, file, fileList) => {
  upload.isUploading = true
}
/** 文件上传成功处理 */
const handleFileSuccess = (response, file, fileList) => {
  const { code, msg, data } = response
  upload.open = false
  upload.isUploading = false
  proxy.$refs['uploadRef'].clearFiles()
  proxy.$alert("<div style='overflow: auto;overflow-x: hidden;max-height: 70vh;padding: 10px 20px 0;'>" + data.item1 + '</div>', '导入结果', {
    dangerouslyUseHTMLString: true
  })
  getList()
}
/** 提交上传文件 */
function submitFileForm() {
  proxy.$refs['uploadRef'].submit()
}
/** 初始化部门数据 */
function initTreeData() {
  // 判断部门的数据是否存在，存在不获取，不存在则获取
  if (deptOptions.value === undefined) {
    treeselect().then((response) => {
      deptOptions.value = response.data
    })
  }
}
/** 重置操作表单 */
function reset() {
  form.value = {
    userId: undefined,
    deptId: undefined,
    userName: undefined,
    nickName: undefined,
    password: undefined,
    phonenumber: undefined,
    email: undefined,
    sex: 2,
    status: 0,
    remark: undefined,
    postIds: [],
    roleIds: []
  }
  proxy.resetForm('userRef')
}
/** 取消按钮 */
function cancel() {
  open.value = false
  reset()
}
/** 新增按钮操作 */
function handleAdd() {
  reset()
  initTreeData()
  getUser().then((response) => {
    postOptions.value = response.data.posts
    roleOptions.value = response.data.roles
    open.value = true
    title.value = '添加用户'
    form.value.password = initPassword.value
  })
}
/** 修改按钮操作 */
function handleUpdate(row) {
  reset()
  initTreeData()
  const userId = row.userId || ids.value

  getUser(userId).then((response) => {
    var data = response.data
    form.value = {
      userId: data.user.userId,
      deptId: data.user.deptId,
      userName: data.user.userName,
      nickName: data.user.nickName,
      password: '',
      phonenumber: data.user.phonenumber,
      email: data.user.email,
      sex: data.user.sex,
      status: data.user.status,
      remark: data.user.remark,
      postIds: data.postIds,
      roleIds: data.roleIds
    }
    roleOptions.value = response.data.roles
    postOptions.value = response.data.posts
    open.value = true
    title.value = '修改用户'
    form.password = ''
  })
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs['userRef'].validate((valid) => {
    if (valid) {
      if (form.value.userId != undefined) {
        updateUser(form.value).then((response) => {
          proxy.$modal.msgSuccess('修改成功')
          open.value = false
          getList()
        })
      } else {
        addUser(form.value).then((response) => {
          proxy.$modal.msgSuccess('新增成功')
          open.value = false
          getList()
        })
      }
    }
  })
}
function checkSelectable(row) {
  return row.userId != 1 ? true : false
}
/**
 * 解决编辑时角色选中不了问题
 */
function selectRole(e) {
  proxy.$forceUpdate()
}
getTreeselect()
getList()
</script>
<style scoped>
.avatar {
  width: 40px;
}
</style>
