# 快速修复指南

## 问题描述
服务器上运行Docker镜像时出现Nginx配置错误：
```
invalid value "must-revalidate" in /etc/nginx/conf.d/default.conf:11
```

## 解决方案

### 方案1：使用简化版配置（推荐）

```bash
# 1. 停止现有容器
docker-compose down

# 2. 使用简化版启动
./docker-build.sh start-simple

# 3. 访问应用
# http://localhost:8080
```

### 方案2：修复原配置

```bash
# 1. 运行修复脚本
chmod +x fix-nginx-config.sh
./fix-nginx-config.sh
```

### 方案3：手动修复

```bash
# 1. 停止容器
docker-compose down

# 2. 重新构建镜像（已修复配置）
docker build -t zr-admin:latest .

# 3. 启动服务
docker-compose up -d

# 4. 检查状态
docker-compose ps
```

## 验证修复

```bash
# 检查容器状态
docker-compose ps

# 查看日志
docker-compose logs

# 测试健康检查
curl http://localhost/health

# 访问应用
curl http://localhost
```

## 如果仍有问题

1. **检查端口占用**：
   ```bash
   netstat -tlnp | grep :80
   ```

2. **查看详细日志**：
   ```bash
   docker-compose logs -f
   ```

3. **使用简化版**：
   ```bash
   ./docker-build.sh start-simple
   ```

## 推荐使用简化版

简化版配置避免了复杂的Nginx配置，更适合快速部署：

- ✅ 基本功能完整
- ✅ 配置简单可靠
- ✅ 启动速度快
- ✅ 问题排查容易

访问地址：http://localhost:8080
