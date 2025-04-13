import useTagsViewStore from '@/store/modules/tagsView'
import router from '@/router'

export default {
  // 刷新当前tab页签
  refreshPage(obj) {
    const { path, query, matched } = router.currentRoute.value;
    if (obj === undefined) {
      matched.forEach((m) => {
        if (m.components && m.components.default && m.components.default.name) {
          if (!['Layout', 'ParentView'].includes(m.components.default.name)) {
            obj = { name: m.components.default.name, path: path, query: query };
          }
        }
      });
    }

    // 使用更可靠的刷新方法 - 先删除缓存，然后重定向刷新
    return Promise.resolve().then(() => {
      // 确保先删除缓存
      if (obj && obj.name) {
        useTagsViewStore().delCachedView(obj);
      }

      // 然后进行路由重定向
      const { path, query } = obj || router.currentRoute.value;
      router.replace({
        path: '/redirect' + path,
        query: query
      });
    });
  },
  // 关闭当前tab页签，打开新页签
  closeOpenPage(obj) {
    useTagsViewStore().delView(router.currentRoute.value);
    if (obj !== undefined) {
      return router.push(obj);
    }
  },
  // 关闭指定tab页签
  closePage(obj) {
    if (obj === undefined) {
      return useTagsViewStore().delView(router.currentRoute.value).then(({ lastPath }) => {
        return router.push(lastPath || '/index');
      });
    }
    return useTagsViewStore().delView(obj);
  },
  // 关闭所有tab页签
  closeAllPage() {
    return useTagsViewStore().delAllViews();
  },
  // 关闭左侧tab页签
  closeLeftPage(obj) {
    return useTagsViewStore().delLeftTags(obj || router.currentRoute.value);
  },
  // 关闭右侧tab页签
  closeRightPage(obj) {
    return useTagsViewStore().delRightTags(obj || router.currentRoute.value);
  },
  // 关闭其他tab页签
  closeOtherPage(obj) {
    return useTagsViewStore().delOthersViews(obj || router.currentRoute.value);
  },
  // 打开tab页签
  openPage(url) {
    return router.push(url);
  },
  // 修改tab页签
  updatePage(obj) {
    return useTagsViewStore().updateVisitedView(obj);
  }
}

// 导出关闭指定tab页签
export function closePage(obj) {
  if (obj === undefined) {
    return useTagsViewStore().delView(router.currentRoute.value).then(({ lastPath }) => {
      return router.push(lastPath || '/index');
    });
  }
  return useTagsViewStore().delView(obj);
}