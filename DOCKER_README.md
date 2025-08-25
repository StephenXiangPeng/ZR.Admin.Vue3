# Docker 部署说明

本项目提供了完整的Docker化部署方案，包括Dockerfile、docker-compose.yml和相关配置文件。

## 文件说明

- `Dockerfile`: 多阶段构建的Docker镜像定义（已优化）
- `docker-compose.yml`: Docker Compose编排文件
- `nginx.conf`: Nginx配置文件，支持Vue Router history模式和API代理
- `.dockerignore`: Docker构建时忽略的文件列表
- `build-and-run.sh`: 快速构建和运行脚本

## 快速开始

### 使用Docker Compose（推荐）

1. 构建并启动服务：
```bash
docker-compose up -d
```

2. 查看服务状态：
```bash
docker-compose ps
```

3. 查看日志：
```bash
docker-compose logs -f zr-admin-vue3
```

4. 停止服务：
```bash
docker-compose down
```

### 使用Docker命令

1. 构建镜像：
```bash
docker build -t zr-admin-vue3 .
```

2. 运行容器：
```bash
docker run -d -p 8080:80 --name zr-admin-vue3 zr-admin-vue3
```

### 使用快速脚本

```bash
# 给脚本执行权限
chmod +x build-and-run.sh

# 一键构建和运行
./build-and-run.sh
```

3. 查看容器状态：
```bash
docker ps
```

4. 停止并删除容器：
```bash
docker stop zr-admin-vue3
docker rm zr-admin-vue3
```

## 访问应用

构建完成后，可以通过以下地址访问应用：
- http://localhost:8080

## 配置说明

### 环境变量

可以通过环境变量配置应用：

```bash
# 在docker-compose.yml中设置
environment:
  - NODE_ENV=production
  - VITE_APP_API_HOST=http://your-api-server
```

### Nginx配置

`nginx.conf`文件包含了以下特性：
- Vue Router history模式支持
- 静态资源缓存优化
- API代理配置（/prod-api/ 和 /msgHub）
- 静态文件MIME类型配置
- 错误页面处理
- 支持WebSocket连接（SignalR）

如需修改配置，请编辑`nginx.conf`文件后重新构建镜像。

### 端口映射

默认端口映射为`8080:80`，如需修改，请编辑`docker-compose.yml`中的ports配置：

```yaml
ports:
  - "3000:80"  # 修改为3000端口
```

## 生产环境部署

### 1. 构建生产镜像

```bash
docker build -t zr-admin-vue3:latest .
```

### 2. 推送到镜像仓库

```bash
docker tag zr-admin-vue3:latest your-registry/zr-admin-vue3:latest
docker push your-registry/zr-admin-vue3:latest
```

### 3. 在生产服务器上部署

```bash
# 拉取镜像
docker pull your-registry/zr-admin-vue3:latest

# 运行容器
docker run -d \
  --name zr-admin-vue3 \
  -p 80:80 \
  --restart unless-stopped \
  your-registry/zr-admin-vue3:latest
```

## 故障排除

### 1. 构建失败

检查以下几点：
- Node.js版本兼容性
- 网络连接是否正常
- 依赖包是否正确安装

### 2. 容器启动失败

查看容器日志：
```bash
docker logs zr-admin-vue3
```

### 3. 应用无法访问

检查：
- 端口映射是否正确
- 防火墙设置
- 容器是否正常运行

### 4. 路由问题

确保nginx.conf中的try_files配置正确：
```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

### 5. API连接问题

检查nginx.conf中的API代理配置：
- `/prod-api/` 代理到后端API服务
- `/msgHub` 代理到SignalR服务
- 确保后端服务地址正确（当前配置为 `172.18.0.3:8888`）

## 性能优化

### 1. 镜像大小优化

Dockerfile使用了多阶段构建，最终镜像只包含必要的文件。

### 2. 缓存优化

- 静态资源设置了长期缓存
- 使用Gzip压缩减少传输大小

### 3. 安全优化

- 使用非root用户运行nginx
- 设置了安全头
- 禁用了不必要的nginx模块

## 监控和日志

### 1. 健康检查

docker-compose.yml中配置了健康检查：
```yaml
healthcheck:
  test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost:80"]
  interval: 30s
  timeout: 10s
  retries: 3
```

### 2. 日志管理

日志文件映射到宿主机：
```yaml
volumes:
  - ./logs:/var/log/nginx
```

## 更新部署

### 1. 更新代码后重新构建

```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### 2. 滚动更新（生产环境）

```bash
# 构建新镜像
docker build -t zr-admin-vue3:new .

# 停止旧容器
docker stop zr-admin-vue3

# 启动新容器
docker run -d --name zr-admin-vue3-new -p 8080:80 zr-admin-vue3:new

# 验证新容器正常后，删除旧容器
docker rm zr-admin-vue3
docker tag zr-admin-vue3:new zr-admin-vue3:latest
```
