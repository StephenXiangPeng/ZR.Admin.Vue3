# 标题显示问题修复指南

## 问题描述

服务器上运行的应用没有显示标题"车房网下单系统"，而本地运行的应用正常显示。

## 问题原因

1. **环境变量未正确传递**：Docker构建时，`VITE_APP_TITLE` 环境变量没有正确传递到构建过程中
2. **构建配置差异**：本地和服务器使用了不同的构建配置

## 解决方案

### 方案1：使用修改后的Dockerfile（推荐）

我已经修改了 `Dockerfile`，在构建前设置了环境变量：

```dockerfile
# 设置环境变量
ENV VITE_APP_TITLE=车房网下单系统
ENV VITE_APP_VERSION=v20240105
ENV VITE_APP_BASE_API=/api
ENV VITE_APP_ROUTER_PREFIX=/
ENV VITE_APP_DEBUG=false
ENV VITE_APP_MOCK=false

# 构建生产版本
RUN yarn build:prod
```

**重新构建和部署：**

```bash
# 重新构建镜像
docker build -t frontend-container .

# 停止旧容器
docker stop frontend-container
docker rm frontend-container

# 启动新容器
docker run -d \
  --name frontend-container \
  --network your-network-name \
  -p 80:80 \
  frontend-container
```

### 方案2：使用docker-compose部署

```bash
# 使用修改后的docker-compose.yml
docker-compose --profile backend up -d --build
```

### 方案3：手动设置环境变量构建

```bash
# 设置环境变量
export VITE_APP_TITLE="车房网下单系统"
export VITE_APP_VERSION="v20240105"
export VITE_APP_BASE_API="/api"

# 构建应用
yarn build:prod

# 然后构建Docker镜像
docker build -t frontend-container .
```

### 方案4：使用构建脚本

```bash
# 使用提供的构建脚本
chmod +x build-with-env.sh
./build-with-env.sh

# 然后构建Docker镜像
docker build -t frontend-container .
```

## 验证修复

### 1. 检查构建日志

```bash
# 查看构建日志，确认环境变量被正确设置
docker build -t frontend-container . 2>&1 | grep VITE_APP_TITLE
```

### 2. 检查容器内环境变量

```bash
# 进入容器检查环境变量
docker exec -it frontend-container env | grep VITE_APP
```

### 3. 检查应用标题

访问应用，确认登录页面显示标题"车房网下单系统"。

## 标题显示位置

应用标题在以下位置显示：

1. **登录页面**：`src/views/login.vue` 第5行
   ```vue
   <h3 class="title">{{ defaultSettings.title }}</h3>
   ```

2. **侧边栏Logo**：`src/layout/components/Sidebar/Logo.vue` 第27行
   ```vue
   const title = ref(import.meta.env.VITE_APP_TITLE)
   ```

3. **页面标题**：`src/settings.js` 第9行
   ```javascript
   title: import.meta.env.VITE_APP_TITLE,
   ```

## 环境变量说明

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `VITE_APP_TITLE` | 应用标题 | 车房网下单系统 |
| `VITE_APP_VERSION` | 应用版本 | v20240105 |
| `VITE_APP_BASE_API` | API基础路径 | /api |
| `VITE_APP_ROUTER_PREFIX` | 路由前缀 | / |
| `VITE_APP_DEBUG` | 调试模式 | false |
| `VITE_APP_MOCK` | 模拟数据 | false |

## 常见问题

### Q1: 为什么本地有标题，服务器没有？

**A**: 本地可能使用了 `index.html` 中的默认标题，而服务器使用环境变量构建，环境变量未正确设置。

### Q2: 如何确认环境变量是否正确传递？

**A**: 检查构建日志或容器内的环境变量：
```bash
docker exec -it frontend-container env | grep VITE_APP
```

### Q3: 修改后还是不显示标题怎么办？

**A**: 检查以下几点：
1. 确认重新构建了镜像
2. 确认重启了容器
3. 检查浏览器缓存，尝试强制刷新
4. 检查控制台是否有JavaScript错误

## 部署命令总结

```bash
# 完整重新部署流程
docker-compose down
docker-compose --profile backend up -d --build

# 或者单独重新构建前端
docker build -t frontend-container .
docker stop frontend-container
docker rm frontend-container
docker run -d --name frontend-container --network your-network -p 80:80 frontend-container
```
