import { createApp } from 'vue'
// import Cookies from 'js-cookie'

import ElementPlus from 'element-plus'
import 'dayjs/locale/zh-cn'
import '@/assets/styles/index.scss' // global css

import App from './App'
import router from './router'
import directive from './directive' // directive
// 注册指令
import plugins from './plugins' // plugins
import { downFile } from '@/utils/request'
import signalR from '@/signalr/signalr'
import vueI18n from './i18n/index'
import pinia from '@/store/index'

// svg图标
import '@/assets/iconfont/iconfont.js' //iconfont
import 'virtual:svg-icons-register'
import SvgIcon from '@/components/SvgIcon/index.vue'
import elementIcons from '@/components/SvgIcon/svgicon'

import './permission' // permission control

import { getConfigKey } from '@/api/system/config'
import { getDicts } from '@/api/system/dict/data'
import { parseTime, resetForm, addDateRange, handleTree, selectDictLabel, download } from '@/utils/ruoyi'

// 分页组件
import Pagination from '@/components/Pagination'
// 自定义表格工具组件
import RightToolbar from '@/components/RightToolbar'
// 文件上传组件
import FileUpload from '@/components/FileUpload'
// 图片上传组件
import ImageUpload from '@/components/ImageUpload'
// 图片预览组件
import ImagePreview from '@/components/ImagePreview'
// 字典标签组件
import DictTag from '@/components/DictTag'
// el-date-picker 快捷选项
import dateOptions from '@/utils/dateOptions'
// Dialog组件
import Dialog from '@/components/Dialog'

// 创建全局错误处理函数
window.onerror = function (message, source, lineno, colno, error) {
	console.error('全局错误:', message, source, error);

	// 如果是模块加载错误，自动尝试刷新
	if (message.includes('Loading chunk') || message.includes('failed')) {
		console.log('检测到资源加载失败，正在尝试刷新...');
		localStorage.setItem('app_reload_attempt', (parseInt(localStorage.getItem('app_reload_attempt') || '0') + 1).toString());
		if (parseInt(localStorage.getItem('app_reload_attempt')) < 3) {
			window.location.reload();
		}
	}

	return false;
};

// 检查是否为刷新后的加载
if (localStorage.getItem('app_reload_attempt')) {
	console.log(`应用刷新尝试次数: ${localStorage.getItem('app_reload_attempt')}`);
}

// 确保CSS和其他资源加载完成
document.addEventListener('DOMContentLoaded', () => {
	console.log('DOM已加载完成，准备初始化应用...');
	startApp();
});

// 如果DOMContentLoaded事件已经触发，直接初始化
if (document.readyState === 'interactive' || document.readyState === 'complete') {
	console.log('DOM已就绪，直接初始化应用...');
	startApp();
}

// 定义应用启动函数
function startApp() {
	// 防止重复初始化
	if (window.appInitialized) return;
	window.appInitialized = true;

	console.log('开始初始化应用...');

	// 创建并挂载Vue应用
	const app = createApp(App);

	// 添加全局错误处理
	app.config.errorHandler = (err, vm, info) => {
		console.error('Vue错误:', err, info);
	};

	// 使用路由和其他插件
	app.use(router);
	// 其他插件...

	// 标记成功加载
	localStorage.setItem('app_reload_attempt', '0');

	// 挂载应用
	app.mount('#app');
	console.log('应用挂载完成');
}

const app = createApp(App)
signalR.init(import.meta.env.VITE_APP_SOCKET_API)
app.config.globalProperties.signalr = signalR
// 全局方法挂载
app.config.globalProperties.getConfigKey = getConfigKey
app.config.globalProperties.getDicts = getDicts
app.config.globalProperties.download = download
app.config.globalProperties.downFile = downFile
app.config.globalProperties.parseTime = parseTime
app.config.globalProperties.resetForm = resetForm
app.config.globalProperties.handleTree = handleTree
app.config.globalProperties.addDateRange = addDateRange
app.config.globalProperties.selectDictLabel = selectDictLabel
app.config.globalProperties.dateOptions = dateOptions

// 全局组件挂载
app.component('DictTag', DictTag)
app.component('Pagination', Pagination)
app.component('UploadFile', FileUpload)
app.component('UploadImage', ImageUpload)
app.component('ImagePreview', ImagePreview)
app.component('RightToolbar', RightToolbar)
app.component('svg-icon', SvgIcon)
app.component('ZrDialog', Dialog)

directive(app)

app.use(pinia).use(router).use(plugins).use(ElementPlus, {}).use(elementIcons).use(vueI18n).mount('#app')
