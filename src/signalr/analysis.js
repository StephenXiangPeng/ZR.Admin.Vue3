// import signalr from './signalr'
import { ElNotification, ElMessage, ElMessageBox } from 'element-plus'
import useSocketStore from '@/store/modules/socket'
import useUserStore from '@/store/modules/user'
import { webNotify } from '@/utils/index'
import { eventBus } from '@/utils/eventBus'
import request from '@/utils/request'  // 添加这行，导入 request
import { h } from 'vue'  // 添加这行，因为使用了 h 函数
import { useRouter } from 'vue-router'


export default {
  onMessage(connection) {
    connection.on(MsgType.M001, (data) => {
      useSocketStore().setOnlineUsers(data)
    })

    connection.on(MsgType.M002, (data) => {
      // useUserStore().saveConnId(data)
    })
    // 接收后台手动推送消息
    connection.on('receiveNotice', (title, data) => {
      ElNotification({
        type: 'info',
        title: title,
        message: data,
        dangerouslyUseHTMLString: true,
        duration: 0
      })
      webNotify({ title: title, body: data })
    })
    // 接收系统通知/公告
    connection.on('moreNotice', (data) => {
      if (data.code == 200) {
        useSocketStore().setNoticeList(data.data)
      }
    })

    // 接收在线用户
    // connection.on('onlineUser', (data) => {
    //   useSocketStore().setOnlineUsers(data)
    // })

    // 接收强退通知
    connection.on('forceUser', (data) => {
      ElMessageBox.alert(`你的账号已被强退，原因：${data.reason || '无'}`, '提示', {
        confirmButtonText: '确定',
        callback: () => {
          useUserStore()
            .logOut()
            .then(() => {
              location.href = import.meta.env.VITE_APP_ROUTER_PREFIX + 'index'
            })
        }
      })
    })
    // 接收聊天数据
    connection.on('receiveChat', (data) => {
      const title = `来自${data.userName}的消息通知`
      useSocketStore().setChat(data)

      if (data.userid != useUserStore().userId) {
        ElNotification({
          title: title,
          message: data.message,
          type: 'success',
          duration: 3000
        })
      }
      webNotify({ title: title, body: data.message })
    })

    connection.on('onlineInfo', (data) => {
      useSocketStore().getOnlineInfo(data)
    })

    connection.on('logOut', () => {
      useUserStore()
        .logOut()
        .then(() => {
          ElMessageBox.alert(`你的账号已在其他设备登录，如果不是你的操作请尽快修改密码`, '提示', {
            confirmButtonText: '确定',
            callback: () => {
              location.href = import.meta.env.VITE_APP_ROUTER_PREFIX + 'index'
            }
          })
        })
    })

    connection.on('TaskReminder', (data) => {
      console.log(data);
      ElMessageBox.alert(`${data.message}`, '自定义提醒', {
        confirmButtonText: '确定'
      })
    })

    // 添加任务状态更新监听
    connection.on('taskStatsUpdate', (data) => {
      console.log('接收到任务状态更新', data)
      useSocketStore().setTaskStats(data)
    })

    // 添加待办流程更新监听
    connection.on('TodoProcessUpdate', (data) => {
      console.log('接收到待办流程更新', data)
      useSocketStore().setTodoProcessCount(data)
    })

    // 添加消息发送监听
    connection.on('SendMessage', (data) => {
      console.log('接收到消息', data);
      // 检查消息接收者是否是当前用户
      const currentUser = useUserStore().userId; // 获取当前用户ID
      if (data.toUserId.toString() === currentUser.toString()) {  // 只有消息接收者才显示通知
        const title = `来自${data.userName}的${data.title}`;
        useSocketStore().setMessage(data);
        // 触发事件
        eventBus.emit('updatePendingCount');
        ElNotification({
          title: title,
          message: data.message,
          type: 'success',
          duration: 30000
        });
      }
    })

    // 添加管理员通知监听
    connection.on('SendNotification', (data) => {
      console.log('接收到管理员消息', data);
      const currentUser = useUserStore().userId;
      if (data.toUserId.toString() === currentUser.toString()) {
        useSocketStore().setSystemNotice(data);

        ElNotification({
          title: data.title,
          customClass: 'custom-notification', // 添加自定义样式类
          duration: 0,
          position: 'bottom-right',
          message: h('div', { class: 'notification-content' }, [
            h('div', { class: 'notification-message' }, data.message),
            h('div', { class: 'notification-footer' }, [
              h('button', {
                class: 'custom-btn',
                onClick: () => {
                  console.log('按钮被点击了')
                  ElNotification.closeAll()
                }
              }, '我知道了')
            ])
          ])
        })
      }
    })

    // 添加系统通知监听
    connection.on('SystemNotification', (data) => {
      console.log('接收到系统通知', data);
      const currentUser = useUserStore().userId;
      if (data.toUserId.toString() === currentUser.toString()) {
        useSocketStore().setSystemNotice(data);
        eventBus.emit('updatePlanTaskItems');
        eventBus.emit('updatePendingCount');

        // 创建通知并获取关闭函数
        const notification = ElNotification({
          title: data.title,
          customClass: 'custom-notification',
          duration: 0,
          position: 'bottom-right',
          message: h('div', { class: 'notification-content' }, [
            h('div', {
              class: 'notification-message',
              style: { cursor: 'pointer' },
              onClick: async () => {
                // 1. 先调用已读接口
                try {
                  await request({
                    url: 'MessageRecord/UpdateIsReadByMessageID/UpdateIsRead',
                    method: 'get',
                    params: {
                      MessagesID: data.messageID
                    }
                  });
                } catch (error) {
                  console.error('调用更新已读接口失败:', error);
                }
                // 2. 再跳转
                let targetPath = '';
                switch (data.type) {
                  case 'TaskItem':
                    targetPath = `/plantask?taskId=${data.targetId}`;
                    break;
                  case 'saleContact':
                    targetPath = `/sale/sale/salecontract?contractId=${data.targetId}&viewDetail=true`;
                    break;
                  case 'purchaseContract':
                    targetPath = `/purchase/procurementplansandcontracts?purchaseContractId=${data.targetId}&viewDetail=true`;
                    break;
                  case 'paymentApplication':
                    targetPath = `/paymentrequest?PaymentRequestID=${data.targetId}&viewDetail=true`;
                    break;
                  case 'Shipping':
                    targetPath = `/sale/shippingdelivery?contractId=${msg.targetId}&viewDetail=true`;
                    break;
                  case 'Settlement':
                    targetPath = `/finance/settlementcenter?contractId=${data.targetId}&viewDetail=true`;
                    break;
                  case 'Inquiry':
                    targetPath = `/purchase/requestquote?inquiryId=${data.targetId}`;
                    break;
                  default:
                    targetPath = '/index';
                }
                eventBus.emit('notification-navigate', {
                  path: targetPath,
                  data: data
                });
                notification.close();
              }
            }, data.message),
            h('div', { class: 'notification-footer' }, [
              h('button', {
                class: 'custom-btn',
                onClick: async () => {
                  try {
                    // 调用更新已读状态的接口
                    const response = await request({
                      url: 'MessageRecord/UpdateIsReadByMessageID/UpdateIsRead',
                      method: 'get',
                      params: {
                        MessagesID: data.messageID
                      }
                    });
                  } catch (error) {
                    console.error('调用更新已读接口失败:', error);
                  } finally {
                    notification.close();
                  }
                }
              }, '我知道了')
            ])
          ])
        });
      }
    });
  }
}
const MsgType = {
  M001: 'onlineNum',
  M002: 'connId',
  M003: 'SendMessage',
  M004: 'SendNotification'
}

