<template>
  <el-dialog v-model="visible" class="two-factor-guide-dialog" title="二次验证使用指南" width="min(1080px, 94vw)"
    top="4vh" append-to-body :close-on-click-modal="false" @closed="handleClosed">
    <div class="guide-layout">
      <el-steps class="guide-steps" :active="active" direction="vertical" finish-status="success">
        <el-step v-for="item in steps" :key="item.title" :title="item.title" />
      </el-steps>

      <section class="guide-content">
        <template v-if="active === 0">
          <h3>先在手机上安装验证器 App</h3>
          <p class="guide-intro">任选一种即可。验证器安装在手机上，不是安装在办公电脑上。</p>
          <div class="app-options">
            <article class="app-card">
              <div class="app-card__title">Microsoft Authenticator</div>
              <p>推荐使用。打开后点击“+” → “其他账户” → “扫描二维码”。</p>
              <el-link type="primary" href="https://www.microsoft.com/security/mobile-authenticator-app"
                target="_blank">前往微软官方下载页</el-link>
            </article>
            <article class="app-card">
              <div class="app-card__title">Google Authenticator</div>
              <p>打开后点击“+” → “扫描二维码”。请确认发布者为 Google LLC。</p>
              <div class="download-links">
                <el-link type="primary" href="https://apps.apple.com/app/google-authenticator/id388497605"
                  target="_blank">iPhone 下载</el-link>
                <el-link type="primary"
                  href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2"
                  target="_blank">Android 下载</el-link>
              </div>
            </article>
          </div>
          <el-alert title="不要从群聊、网盘或陌生网站下载验证器，也不要让他人代为扫描二维码。" type="warning" show-icon
            :closable="false" />
        </template>

        <template v-else-if="active === 1">
          <h3>进入个人中心</h3>
          <ol>
            <li>点击页面右上角的用户名。</li>
            <li>在下拉菜单中点击“个人中心”。</li>
          </ol>
          <el-image class="guide-image" :src="openProfileImage" fit="contain"
            :preview-src-list="[openProfileImage]" hide-on-click-modal />
        </template>

        <template v-else-if="active === 2">
          <h3>开启二次验证</h3>
          <ol>
            <li>在个人中心点击“账户安全”。</li>
            <li>确认状态为“未开启”，然后点击“开启二次验证”。</li>
          </ol>
          <el-image class="guide-image" :src="enableImage" fit="contain" :preview-src-list="[enableImage]"
            hide-on-click-modal />
        </template>

        <template v-else-if="active === 3">
          <h3>扫码并确认绑定</h3>
          <ol>
            <li>在验证器 App 中选择“扫描二维码”，扫描当前页面显示的二维码。</li>
            <li>输入 App 当前显示的 6 位动态码。</li>
            <li>点击“确认绑定”，看到“二次验证已开启”才表示绑定成功。</li>
          </ol>
          <el-image class="guide-image" :src="bindImage" fit="contain" :preview-src-list="[bindImage]"
            hide-on-click-modal />
          <el-alert title="示意图中的二维码已遮挡。请扫描您自己账号页面中当次显示的二维码。" type="warning" show-icon
            :closable="false" />
        </template>

        <template v-else>
          <h3>以后登录时如何使用</h3>
          <div class="usage-grid">
            <article>
              <h4>出现“身份验证”页面</h4>
              <ol>
                <li>打开手机验证器 App。</li>
                <li>找到“进出口办公系统”对应账号。</li>
                <li>输入当前 6 位动态码并点击“确认登录”。</li>
              </ol>
            </article>
            <article>
              <h4>可能触发验证的情况</h4>
              <ul>
                <li>新电脑或新浏览器登录；</li>
                <li>设备超过 30 天未验证；</li>
                <li>登录地区发生变化；</li>
                <li>近期密码多次输入错误。</li>
              </ul>
            </article>
          </div>
          <el-alert title="二维码和 6 位动态码都不能提供给他人。更换或丢失手机时，请联系管理员重置二次验证。" type="error"
            show-icon :closable="false" />
        </template>
      </section>
    </div>

    <template #footer>
      <div class="guide-footer">
        <span class="guide-progress">{{ active + 1 }} / {{ steps.length }}</span>
        <div>
          <el-button v-if="active > 0" @click="active--">上一步</el-button>
          <el-button v-if="active < steps.length - 1" type="primary" @click="active++">下一步</el-button>
          <el-button v-else type="primary" @click="finish">我知道了</el-button>
        </div>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import useUserStore from '@/store/modules/user'
import openProfileImage from '@/assets/images/two-factor-guide/open-profile.png'
import enableImage from '@/assets/images/two-factor-guide/enable-2fa.png'
import bindImage from '@/assets/images/two-factor-guide/bind-authenticator.png'

// 发布新的引导内容时修改此版本号，已登录用户会再次看到新版引导。
const GUIDE_VERSION = '2026-09-2fa-v1'
const userStore = useUserStore()
const visible = ref(false)
const active = ref(0)
const autoOpened = ref(false)

const steps = [
  { title: '下载验证器' },
  { title: '进入个人中心' },
  { title: '开启二次验证' },
  { title: '扫码确认绑定' },
  { title: '登录时使用' }
]

function storageKey() {
  const account = userStore.userName || userStore.userId || 'unknown'
  return `two-factor-guide:${GUIDE_VERSION}:${account}`
}

function markAsSeen() {
  localStorage.setItem(storageKey(), '1')
}

function open(force = false) {
  if (!force && localStorage.getItem(storageKey()) === '1') return
  active.value = 0
  autoOpened.value = !force
  visible.value = true
}

function finish() {
  markAsSeen()
  visible.value = false
}

function handleClosed() {
  // 用户通过右上角关闭时也视为已阅读，避免每次登录重复打扰。
  if (autoOpened.value) markAsSeen()
  autoOpened.value = false
}

defineExpose({ open })
</script>

<style lang="scss">
.two-factor-guide-dialog {
  .el-dialog__body {
    padding-top: 12px;
  }

  .guide-layout {
    display: grid;
    grid-template-columns: 180px minmax(0, 1fr);
    gap: 24px;
    min-height: 560px;
  }

  .guide-steps {
    padding: 12px 0 4px 8px;
  }

  .guide-content {
    min-width: 0;
    padding-right: 4px;

    h3 {
      margin: 0 0 12px;
      font-size: 20px;
    }

    h4 {
      margin: 0 0 10px;
      font-size: 16px;
    }

    li,
    p {
      line-height: 1.8;
    }
  }

  .guide-intro {
    color: #606266;
  }

  .guide-image {
    display: block;
    width: 100%;
    max-height: 430px;
    margin: 12px auto;
    border: 1px solid #dcdfe6;
    border-radius: 6px;
    background: #f5f7fa;
  }

  .app-options,
  .usage-grid {
    display: grid;
    grid-template-columns: repeat(2, minmax(0, 1fr));
    gap: 16px;
    margin: 20px 0;
  }

  .app-card,
  .usage-grid article {
    padding: 20px;
    border: 1px solid #dcdfe6;
    border-radius: 8px;
    background: #fafcff;
  }

  .app-card__title {
    font-size: 17px;
    font-weight: 600;
  }

  .download-links {
    display: flex;
    gap: 18px;
  }

  .guide-footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  .guide-progress {
    color: #909399;
  }
}

@media (max-width: 760px) {
  .two-factor-guide-dialog {
    width: 96vw !important;
    margin-top: 2vh !important;

    .guide-layout {
      display: block;
      min-height: 0;
    }

    .guide-steps {
      display: none;
    }

    .app-options,
    .usage-grid {
      grid-template-columns: 1fr;
    }

    .guide-image {
      max-height: 45vh;
    }
  }
}
</style>
