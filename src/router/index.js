import { createWebHistory, createRouter } from 'vue-router'
import Layout from '@/layout'
import { ElLoading } from 'element-plus'

/**
 * Note: 路由配置项
 *
 * hidden: true                     // 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
 * alwaysShow: true                 // 当你一个路由下面的 children 声明的路由大于1个时，自动会变成嵌套的模式--如组件页面
 *                                  // 只有一个时，会将那个子路由当做根路由显示在侧边栏--如引导页面
 *                                  // 若你想不管路由下面的 children 声明的个数都显示你的根路由
 *                                  // 你可以设置 alwaysShow: true，这样它就会忽略之前定义的规则，一直显示根路由
 * redirect: noRedirect             // 当设置 noRedirect 的时候该路由在面包屑导航中不可被点击
 * name:'router-name'               // 设定路由的名字，一定要填写不然使用<keep-alive>时会出现各种问题
 * query: '{"id": 1, "name": "ry"}' // 访问路由的默认传递参数
 * meta : {
    noCache: true                   // 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
    title: 'title'                  // 设置该路由在侧边栏和面包屑中展示的名字
    icon: 'svg-name'                // 设置该路由的图标，对应路径src/assets/icons/svg
    breadcrumb: false               // 如果设置为false，则不会在breadcrumb面包屑中显示
    activeMenu: '/system/user'      // 当路由设置了该属性，则会高亮相对应的侧边栏。
  }
 */

// 公共路由
export const constantRoutes = [
  {
    path: '/redirect',
    component: Layout,
    hidden: true,
    children: [
      {
        path: '/redirect/:path(.*)',
        component: () => import('@/views/redirect/index.vue')
      }
    ]
  },
  {
    path: '/login',
    component: () => import('@/views/login'),
    hidden: true
  },
  {
    path: '/sociallogin',
    component: () => import('@/views/socialLogin'),
    hidden: true
  },
  {
    path: '/register',
    component: () => import('@/views/register'),
    hidden: true
  },
  {
    path: '/:pathMatch(.*)*',
    component: () => import('@/views/error/404'),
    hidden: true
  },
  {
    path: '/401',
    component: () => import('@/views/error/401'),
    hidden: true
  },
  {
    path: '',
    component: Layout,
    redirect: '/index',
    children: [
      {
        path: '/index',
        component: () => import('@/views/huirong/index/index'),
        name: 'Index',
        meta: { title: '首页', icon: 'index', affix: true, titleKey: 'menu.home' }
      }
    ]
  },
  {
    path: '/user',
    component: Layout,
    hidden: true,
    redirect: 'noredirect',
    children: [
      {
        path: 'profile',
        component: () => import('@/views/system/user/profile/index'),
        name: 'Profile',
        meta: { title: '个人中心', icon: 'user', titleKey: 'menu.personalCenter' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_APP_ROUTER_PREFIX),
  routes: constantRoutes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

router.onError((error) => {
  console.error('路由错误:', error);
  // 可以在这里添加重试逻辑或显示错误提示
  const failedChunk = /Loading chunk (\d)+ failed/gi.test(error.message);
  const failedToResolve = /Failed to resolve/gi.test(error.message);
  const componentError = /Failed to resolve component/gi.test(error.message);

  if (failedChunk || failedToResolve || componentError) {
    console.log('检测到资源加载失败，正在尝试刷新页面...');
    window.location.reload(); // 自动刷新页面
  }
});

// 添加全局导航守卫
router.beforeEach((to, from, next) => {
  // 显示加载指示器
  const loading = ElLoading.service({
    lock: true,
    text: '页面加载中...',
    background: 'rgba(0, 0, 0, 0.7)'
  });

  // 继续导航
  next();

  // 页面渲染后关闭加载指示器
  router.afterEach(() => {
    loading.close();
  });
});

export default router
