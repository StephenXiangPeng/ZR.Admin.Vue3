# 后端服务部署配置说明

## 概述

本文档说明如何在Docker环境中配置前端应用访问后端服务。

## 部署方案

### 方案1：外部后端服务（推荐）

如果您的后端服务运行在Docker容器外部（如直接运行在服务器上），使用此方案。

#### 1. 修改环境变量

编辑 `env.production` 文件：

```bash
# 直接访问后端服务
VITE_APP_BASE_API=http://your-backend-host:8888
```

#### 2. 修改Dockerfile中的nginx配置

在 `Dockerfile` 中，将 `your-backend-host:8888` 替换为实际的后端服务地址：

```nginx
location /api/ {
    proxy_pass http://your-actual-backend-host:8888/;
    # ... 其他配置
}
```

#### 3. 构建和部署

```bash
# 构建镜像
docker build -t zr-admin-frontend .

# 运行容器
docker run -d -p 80:80 --name zr-admin-frontend zr-admin-frontend
```

### 方案2：Docker容器化后端服务

如果您的后端服务也使用Docker部署，使用此方案。

#### 1. 准备后端服务

确保您有后端服务的Docker镜像，例如：
- `your-backend-image:latest`

#### 2. 使用docker-compose部署

```bash
# 启动前端和后端服务
docker-compose --profile backend up -d

# 或者使用nginx代理模式
docker-compose --profile proxy --profile backend up -d
```

#### 3. 访问地址

- 前端应用：http://localhost:80
- 后端API：http://localhost:8888
- 通过nginx代理：http://localhost:8080

### 方案3：使用nginx反向代理

#### 1. 启动所有服务

```bash
# 启动前端、后端和nginx代理
docker-compose --profile proxy --profile backend up -d
```

#### 2. 访问地址

- 通过nginx代理访问：http://localhost:8080
- 前端和后端都通过nginx代理访问

## 配置说明

### 环境变量

| 变量名 | 说明 | 示例值 |
|--------|------|--------|
| `VITE_APP_BASE_API` | API基础路径 | `/api` 或 `http://backend:8888` |
| `VITE_APP_API_HOST` | 后端服务地址 | `http://backend:8888` |

### nginx代理配置

nginx会将以下路径的请求代理到后端服务：

- `/api/*` → 后端服务
- `/prod-api/*` → 后端服务

### 网络配置

所有服务都在 `zr-admin-network` 网络中，可以通过容器名互相访问：

- 前端容器：`zr-admin-frontend`
- 后端容器：`zr-admin-backend`
- nginx代理：`zr-admin-proxy`

## 故障排除

### 1. 检查服务状态

```bash
# 查看所有容器状态
docker ps

# 查看特定容器日志
docker logs zr-admin-frontend
docker logs zr-admin-backend
```

### 2. 检查网络连接

```bash
# 进入前端容器测试后端连接
docker exec -it zr-admin-frontend sh
curl http://zr-admin-backend:8888/health
```

### 3. 检查nginx配置

```bash
# 测试nginx配置
docker exec -it zr-admin-frontend nginx -t

# 重新加载nginx配置
docker exec -it zr-admin-frontend nginx -s reload
```

## 安全注意事项

1. **生产环境**：请使用HTTPS和有效的SSL证书
2. **防火墙**：确保只开放必要的端口
3. **环境变量**：不要在代码中硬编码敏感信息
4. **网络安全**：使用Docker网络隔离服务

## 示例命令

```bash
# 完整部署流程
git clone <your-repo>
cd ZR.Admin.Vue3

# 配置环境变量
cp env.production .env.production
# 编辑 .env.production 文件

# 构建和启动
docker-compose --profile backend up -d

# 查看状态
docker-compose ps

# 查看日志
docker-compose logs -f
```
