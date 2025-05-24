// 官方文档：https://docs.microsoft.com/zh-cn/aspnet/core/signalr/javascript-client?view=aspnetcore-6.0&viewFallbackFrom=aspnetcore-2.2&tabs=visual-studio
import * as signalR from '@microsoft/signalr'
import { getToken } from '@/utils/auth'
import { ElMessage } from 'element-plus'
import cache from '@/plugins/cache'
import analysis from '@/signalr/analysis'

export default {
  // signalR对象
  SR: {},
  // 失败连接重试次数
  failNum: 4,
  // 连接状态
  connectionState: 'Disconnected',
  // 重连延迟时间（毫秒）
  reconnectDelay: 5000,
  // 最大重试次数
  maxRetries: 5,
  init(url) {
    var socketUrl = window.location.origin + url + '?clientId=' + cache.local.get('clientId')
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(socketUrl, { accessTokenFactory: () => getToken() })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 20000]) // 自定义重连间隔
      .configureLogging(signalR.LogLevel.Warning)
      .build()

    this.SR = connection

    // 监听连接状态变化
    connection.onclose(async (error) => {
      this.connectionState = 'Disconnected'
      console.error('连接断开:', error)
      ElMessage({
        message: '与服务器连接已断开，正在尝试重新连接...',
        type: 'warning',
        duration: 2000
      })
      await this.start()
    })

    connection.onreconnected((connectionId) => {
      this.connectionState = 'Connected'
      ElMessage({
        message: '与服务器通讯已连接成功',
        type: 'success',
        duration: 2000
      })
      console.log('断线重新连接成功:', connectionId)
    })

    connection.onreconnecting(async () => {
      this.connectionState = 'Reconnecting'
      console.log('正在重新连接...')
      ElMessage({
        message: '正在重新连接服务器...',
        type: 'info',
        duration: 2000
      })
    })

    analysis.onMessage(connection)
  },

  /**
   * 启动连接
   * @returns {Promise<boolean>}
   */
  async start() {
    try {
      if (this.SR.state === signalR.HubConnectionState.Connected) {
        return true
      }

      if (this.SR.state === signalR.HubConnectionState.Disconnected) {
        await this.SR.start()
        this.connectionState = 'Connected'
        return true
      }

      return false
    } catch (error) {
      console.error('SignalR 连接失败:', error)
      this.failNum--

      if (this.failNum > 0) {
        ElMessage({
          message: `连接失败，${this.reconnectDelay / 1000}秒后重试...`,
          type: 'warning',
          duration: 2000
        })

        await new Promise(resolve => setTimeout(resolve, this.reconnectDelay))
        return this.start()
      }

      ElMessage({
        message: '连接服务器失败，请刷新页面重试',
        type: 'error',
        duration: 3000
      })
      return false
    }
  },

  /**
   * 安全调用 SignalR 方法
   * @param {string} method - 方法名
   * @param {any[]} args - 参数
   * @returns {Promise<any>}
   */
  async safeInvoke(method, ...args) {
    let retries = 0

    while (retries < this.maxRetries) {
      try {
        if (this.SR.state !== signalR.HubConnectionState.Connected) {
          await this.start()
        }

        if (this.SR.state === signalR.HubConnectionState.Connected) {
          return await this.SR.invoke(method, ...args)
        }

        throw new Error('Connection not ready')
      } catch (error) {
        retries++
        console.error(`调用 ${method} 失败 (尝试 ${retries}/${this.maxRetries}):`, error)

        if (retries === this.maxRetries) {
          throw error
        }

        await new Promise(resolve => setTimeout(resolve, this.reconnectDelay))
      }
    }
  },

  // 接收消息处理
  receiveMsg(connection) { }
}
