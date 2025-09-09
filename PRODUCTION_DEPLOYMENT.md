# 生产环境部署指南

## 当前生产环境配置

基于您的Docker网络信息，当前生产环境包含以下容器：

### 🐳 容器配置
- **frontend-container** (172.18.0.4) - 前端应用
- **backend-container** (172.18.0.5) - 后端服务 (端口8888)
- **mysql57** (172.18.0.3) - 数据库服务
- **redis** (172.18.0.2) - 缓存服务

### 🌐 网络配置
- 网络段：172.18.0.0/16
- 容器间通信：通过容器名或IP地址

## 前端访问后端服务的方式

### 方式1：通过nginx代理（推荐）

前端容器内的nginx将API请求代理到后端容器：

```nginx
# 前端容器nginx配置
location /api/ {
    proxy_pass http://backend-container:8888/;
    # ... 其他配置
}
```

**访问流程：**
1. 用户访问前端 → `http://your-domain.com`
2. 前端发起API请求 → `/api/user/login`
3. nginx代理转发 → `http://backend-container:8888/user/login`
4. 后端处理请求并返回结果

### 方式2：直接访问后端服务

如果前端直接访问后端服务，需要配置：

```javascript
// 环境变量配置
VITE_APP_BASE_API=http://backend-container:8888
```

## 部署步骤

### 1. 构建前端镜像

```bash
# 在项目根目录执行
docker build -t frontend-container .
```

### 2. 启动服务

```bash
# 使用docker-compose启动所有服务
docker-compose --profile backend up -d

# 或者单独启动前端容器
docker run -d \
  --name frontend-container \
  --network your-network-name \
  -p 80:80 \
  frontend-container
```

### 3. 验证部署

```bash
# 检查容器状态
docker ps

# 检查网络连接
docker exec -it frontend-container ping backend-container

# 测试API代理
curl http://localhost/api/health
```

## 配置说明

### 环境变量

生产环境使用以下配置：

```bash
# env.production
VITE_APP_BASE_API=/api                    # 使用nginx代理
VITE_APP_API_HOST=http://backend-container:8888  # 后端服务地址
```

### nginx代理规则

前端容器内的nginx配置了以下代理规则：

- `/api/*` → `http://backend-container:8888/*`
- `/prod-api/*` → `http://backend-container:8888/*`

### 网络通信

容器间通信使用Docker网络：

```bash
# 前端容器访问后端
http://backend-container:8888

# 前端容器访问数据库
mysql57:3306

# 前端容器访问Redis
redis:6379
```

## 故障排除

### 1. 检查容器网络

```bash
# 查看网络详情
docker network inspect your-network-name

# 测试容器间连通性
docker exec -it frontend-container ping backend-container
```

### 2. 检查nginx配置

```bash
# 进入前端容器
docker exec -it frontend-container sh

# 测试nginx配置
nginx -t

# 重新加载配置
nginx -s reload
```

### 3. 检查API代理

```bash
# 测试API代理
curl -v http://localhost/api/health

# 查看nginx访问日志
docker exec -it frontend-container tail -f /var/log/nginx/access.log
```

### 4. 检查后端服务

```bash
# 检查后端容器状态
docker logs backend-container

# 测试后端服务
curl http://backend-container:8888/health
```

## 性能优化

### 1. nginx缓存配置

```nginx
# 静态资源缓存
location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg)$ {
    expires 1y;
    add_header Cache-Control "public, immutable";
}
```

### 2. gzip压缩

```nginx
# 启用gzip压缩
gzip on;
gzip_types text/plain text/css application/json application/javascript;
```

### 3. 连接池配置

```nginx
# 后端连接池
upstream backend {
    server backend-container:8888;
    keepalive 32;
}
```

## 监控和维护

### 1. 健康检查

```bash
# 前端健康检查
curl http://localhost/health

# 后端健康检查
curl http://backend-container:8888/health
```

### 2. 日志查看

```bash
# 查看前端日志
docker logs frontend-container

# 查看nginx访问日志
docker exec -it frontend-container tail -f /var/log/nginx/access.log
```

### 3. 性能监控

```bash
# 查看容器资源使用
docker stats

# 查看网络流量
docker exec -it frontend-container netstat -tulpn
```

## 安全建议

1. **使用HTTPS**：配置SSL证书
2. **防火墙**：只开放必要端口
3. **网络隔离**：使用Docker网络隔离服务
4. **定期更新**：保持镜像和依赖更新

## 常用命令

```bash
# 重新构建和部署
docker-compose down
docker-compose --profile backend up -d --build

# 查看服务状态
docker-compose ps

# 查看日志
docker-compose logs -f frontend-container

# 进入容器调试
docker exec -it frontend-container sh
```
