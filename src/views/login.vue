<template>
  <starBackground></starBackground>
  <div class="login-wrap">
    <div class="login">
      <h3 class="title">{{ defaultSettings.title }}</h3>
      <el-form ref="loginRef" :model="loginForm" :rules="loginRules" class="login-form">
        <el-form-item prop="username">
          <el-input v-model="loginForm.username" type="text" auto-complete="off" :placeholder="$t('login.account')">
            <template #prefix>
              <svg-icon name="user" class="input-icon" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            show-password
            type="password"
            auto-complete="off"
            :placeholder="$t('login.password')"
            @keyup.enter="handleLogin">
            <template #prefix>
              <svg-icon name="password" class="input-icon" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="code" v-if="captchaOnOff != 'off'">
          <el-input v-model="loginForm.code" auto-complete="off" :placeholder="$t('login.captcha')" style="width: 63%" @keyup.enter="handleLogin">
            <template #prefix>
              <svg-icon name="validCode" class="input-icon" />
            </template>
          </el-input>
          <div class="login-code">
            <el-image :src="codeUrl" @click="getCode" class="login-code-img" />
          </div>
        </el-form-item>

        <el-form-item style="width: 100%" :style="{ 'margin-top': captchaOnOff == 'off' ? '40px' : '' }">
          <el-button :loading="loading" size="default" round type="primary" style="width: 100%" @click.prevent="handleLogin">
            <span v-if="!loading">{{ $t('login.btnLogin') }}</span>
            <span v-else>登 录 中...</span>
          </el-button>
        </el-form-item>

        <div class="login-options">
          <el-checkbox v-model="loginForm.rememberMe">{{ $t('login.rememberMe') }}</el-checkbox>
          <span style="font-size: 12px">
            <!-- <router-link class="link-type" :to="'/register'">{{ $t('login.register') }}</router-link> -->
            <span @click="handleForgetPwd()" class="forget-pwd">{{ $t('login.forgotPwd') }}</span>
          </span>
        </div>
      </el-form>
      <oauthLogin v-if="defaultSettings.showOtherLogin"></oauthLogin>
    </div>

    <div class="el-login-footer">
      <div v-html="defaultSettings.copyright"></div>
    </div>
  </div>
</template>

<script setup name="login">
import { getCodeImg } from '@/api/system/login'
import Cookies from 'js-cookie'
import { encrypt, decrypt } from '@/utils/jsencrypt'
import defaultSettings from '@/settings'
import starBackground from '@/views/components/starBackground.vue'
import useUserStore from '@/store/modules/user'
import oauthLogin from './components/Login/oauthLogin.vue'

var visitorId = ''
const fpPromise = import('https://openfpcdn.io/fingerprintjs/v3').then((FingerprintJS) => FingerprintJS.load())

const userStore = useUserStore()
const router = useRouter()
const route = useRoute()
const { proxy } = getCurrentInstance()

const loginForm = ref({
  username: '',
  password: '',
  rememberMe: false,
  code: '',
  uuid: ''
})
const loginRules = {
  username: [{ required: true, trigger: 'blur', message: '请输入您的账号' }],
  password: [{ required: true, trigger: 'blur', message: '请输入您的密码' }],
  code: [{ required: true, trigger: 'change', message: '请输入验证码' }]
}
const codeUrl = ref('')
const loading = ref(false)
// 验证码开关
const captchaOnOff = ref('')
// 注册开关
const register = ref(false)
const redirect = ref()
redirect.value = route.query.redirect
// Get the visitor identifier when you need it.
fpPromise
  .then((fp) => fp.get())
  .then((result) => {
    // This is the visitor identifier:
    visitorId = result.visitorId
    userStore.setClientId(visitorId)
  })
watch(
  route,
  (newRoute) => {
    redirect.value = newRoute.query && newRoute.query.redirect
  },
  { immediate: true }
)
function handleLogin() {
  proxy.$refs.loginRef.validate((valid) => {
    if (valid) {
      loading.value = true
      // 勾选了需要记住密码设置在cookie中设置记住用户名和密码
      if (loginForm.value.rememberMe) {
        Cookies.set('username', loginForm.value.username, { expires: 7 })
        Cookies.set('password', encrypt(loginForm.value.password), { expires: 7 })
        Cookies.set('rememberMe', loginForm.value.rememberMe, { expires: 7 })
      } else {
        // 否则移除
        Cookies.remove('username')
        Cookies.remove('password')
        Cookies.remove('rememberMe')
      }
      // 调用action的登录方法
      userStore
        .login(loginForm.value)
        .then(() => {
          proxy.$modal.msgSuccess(proxy.$t('login.loginSuccess'))
          const query = route.query
          const otherQueryParams = Object.keys(query).reduce((acc, cur) => {
            if (cur !== 'redirect') {
              acc[cur] = query[cur]
            }
            return acc
          }, {})
          router.push({ path: redirect.value || '/', query: otherQueryParams })
        })
        .catch((error) => {
          console.error(error)
          proxy.$modal.msgError(error.msg)
          loading.value = false
          // 重新获取验证码
          if (captchaOnOff.value) {
            getCode()
          }
        })
    }
  })
}

function getCode() {
  getCodeImg().then((res) => {
    codeUrl.value = 'data:image/gif;base64,' + res.data.img
    loginForm.value.uuid = res.data.uuid
    captchaOnOff.value = res.data.captchaOff
  })
}

function getCookie() {
  const username = Cookies.get('username')
  const password = Cookies.get('password')
  const rememberMe = Cookies.get('rememberMe')
  loginForm.value = {
    username: username === undefined ? loginForm.value.username : username,
    password: password === undefined ? loginForm.value.password : decrypt(password),
    rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
  }
}
function handleForgetPwd() {
  proxy.$modal.msg('请联系管理员')
}

// 仅保留账号密码登录模式
userStore.loginType = 1
getCode()
getCookie()
</script>

<style lang="scss" scoped>
@use '@/assets/styles/login.scss';
.login-form {
  height: auto;
  padding-bottom: 5px;
}
.forget-pwd {
  color: #ccc;
  margin-left: 10px;
  cursor: pointer;
  border-left: 1px solid;
  padding-left: 10px;
}
.login-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 6px;
}
.qrCode {
  width: 160px;
  height: 160px;
  line-height: 160px;
}
</style>
