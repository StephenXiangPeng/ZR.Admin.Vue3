# Docker 部署指南 - 服务器部署版本

本项目提供了完整的Docker化部署方案，专门针对服务器部署进行了优化，支持开发和生产环境。

## 文件说明

- `Dockerfile` - 生产环境镜像构建文件（Nginx + Vue3）
- `Dockerfile.dev` - 开发环境镜像构建文件
- `docker-compose.yml` - Docker Compose配置文件（支持资源限制、日志管理）
- `.dockerignore` - Docker构建忽略文件
- `docker-build.sh` - 增强的构建和部署脚本
- `env.production` - 生产环境配置文件
- `nginx/` - Nginx反向代理配置文件
  - `nginx.conf` - 主配置文件
  - `conf.d/default.conf` - 默认站点配置
  - `conf.d/ssl.conf` - HTTPS配置示例

## 快速开始

### 1. 构建生产镜像

```bash
# 使用脚本构建
./docker-build.sh build-prod

# 或直接使用Docker命令
docker build -t zr-admin:latest .
```

### 2. 启动生产环境

```bash
# 使用脚本启动
./docker-build.sh start-prod

# 或使用Docker Compose
docker-compose up -d zr-admin-frontend
```

访问地址: http://localhost

### 3. 开发环境

```bash
# 构建开发镜像
./docker-build.sh build-dev

# 启动开发环境
./docker-build.sh start-dev
```

访问地址: http://localhost:8887

## 脚本使用说明

`docker-build.sh` 脚本提供了完整的服务器部署管理功能：

```bash
# 构建相关
./docker-build.sh build-prod    # 构建生产镜像
./docker-build.sh build-dev     # 构建开发镜像

# 启动相关
./docker-build.sh start-prod    # 启动生产环境
./docker-build.sh start-dev     # 启动开发环境
./docker-build.sh start-proxy   # 启动带反向代理的生产环境

# 管理相关
./docker-build.sh stop          # 停止所有服务
./docker-build.sh restart-prod  # 重启生产环境
./docker-build.sh restart-dev   # 重启开发环境

# 监控相关
./docker-build.sh logs          # 查看日志
./docker-build.sh status        # 查看服务状态和资源使用情况

# 维护相关
./docker-build.sh backup        # 备份数据和配置
./docker-build.sh update        # 更新应用
./docker-build.sh cleanup       # 清理未使用的镜像
./docker-build.sh help          # 显示帮助信息
```

## 服务器部署流程

### 1. 首次部署

```bash
# 1. 克隆项目到服务器
git clone <your-repo-url>
cd ZR.Admin.Vue3

# 2. 配置生产环境变量
cp env.production .env.production
# 编辑 .env.production 文件，配置API地址等

# 3. 构建镜像
./docker-build.sh build-prod

# 4. 启动服务
./docker-build.sh start-prod

# 5. 检查状态
./docker-build.sh status
```

### 2. 使用反向代理部署

```bash
# 启动带反向代理的环境
./docker-build.sh start-proxy

# 访问地址: http://localhost:8080
```

### 3. 配置HTTPS

1. 将SSL证书文件放到 `nginx/ssl/` 目录
2. 编辑 `nginx/conf.d/ssl.conf` 文件
3. 取消注释并配置证书路径
4. 重启服务

### 4. 日常维护

```bash
# 查看服务状态
./docker-build.sh status

# 查看日志
./docker-build.sh logs

# 备份数据
./docker-build.sh backup

# 更新应用
./docker-build.sh update

# 清理旧镜像
./docker-build.sh cleanup
```

## 环境变量配置

### 生产环境

生产环境使用Nginx作为Web服务器，支持以下配置：

- 端口: 80
- 静态文件缓存: 1年
- Gzip压缩: 启用
- 安全头: 已配置

### 开发环境

开发环境使用Vite开发服务器：

- 端口: 8887
- 热重载: 启用
- 源码映射: 启用

## 自定义配置

### 修改Nginx配置

如果需要修改Nginx配置，可以编辑`Dockerfile`中的nginx配置部分，或者创建自定义的nginx配置文件。

### 环境变量

可以通过环境变量文件或Docker Compose的环境变量来配置应用：

```yaml
environment:
  - NODE_ENV=production
  - VITE_APP_API_HOST=http://your-api-host
```

## 健康检查

容器包含健康检查功能，会定期检查应用状态：

```bash
# 查看健康状态
docker ps
```

## 日志查看

```bash
# 查看实时日志
./docker-build.sh logs

# 或使用Docker命令
docker-compose logs -f
```

## 故障排除

### 1. 端口冲突

如果80端口被占用，可以修改`docker-compose.yml`中的端口映射：

```yaml
ports:
  - "8080:80"  # 改为8080端口
```

### 2. 构建失败

检查以下项目：
- Node.js版本兼容性
- 依赖包安装是否成功
- 网络连接是否正常

### 3. 权限问题

确保脚本有执行权限：

```bash
chmod +x docker-build.sh
```

## 生产部署建议

### 1. 安全配置
- **使用HTTPS**: 配置SSL证书，启用HTTPS
- **防火墙**: 只开放必要端口（80, 443）
- **定期更新**: 定期更新基础镜像和依赖包
- **安全头**: 已配置完整的安全头

### 2. 性能优化
- **资源限制**: 已配置CPU和内存限制
- **Gzip压缩**: 启用静态资源压缩
- **缓存策略**: 配置长期缓存静态资源
- **多阶段构建**: 优化镜像大小

### 3. 监控和日志
- **健康检查**: 自动监控应用状态
- **日志管理**: 配置日志轮转和大小限制
- **资源监控**: 使用 `./docker-build.sh status` 查看资源使用

### 4. 高可用部署
- **负载均衡**: 可以部署多个实例
- **反向代理**: 使用Nginx反向代理
- **数据备份**: 定期备份配置和数据

## 性能优化特性

- ✅ **多阶段构建**: 优化镜像大小，减少攻击面
- ✅ **Gzip压缩**: 减少传输数据量
- ✅ **静态资源缓存**: 1年缓存期，提高加载速度
- ✅ **安全头配置**: 完整的Web安全头
- ✅ **资源限制**: 防止资源滥用
- ✅ **健康检查**: 自动故障检测
- ✅ **日志管理**: 自动日志轮转

## 支持

如有问题，请检查：
1. Docker和Docker Compose版本
2. 系统资源是否充足
3. 网络连接是否正常
4. 日志文件中的错误信息
