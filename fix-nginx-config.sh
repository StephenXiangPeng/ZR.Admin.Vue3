#!/bin/bash

# Nginx配置修复脚本
# 解决 "must-revalidate" 配置错误

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_message() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_message "开始修复Nginx配置问题..."

# 1. 停止现有容器
print_message "停止现有容器..."
docker-compose down 2>/dev/null || true

# 2. 重新构建镜像
print_message "重新构建镜像..."
docker build -t zr-admin:latest .

# 3. 启动服务
print_message "启动服务..."
docker-compose up -d

# 4. 等待服务启动
print_message "等待服务启动..."
sleep 10

# 5. 检查服务状态
print_message "检查服务状态..."
docker-compose ps

# 6. 检查健康状态
print_message "检查健康状态..."
if curl -f http://localhost/health 2>/dev/null; then
    print_message "✅ 服务启动成功！"
    print_message "访问地址: http://localhost"
else
    print_warning "服务可能还在启动中，请稍等片刻..."
    print_message "查看日志: docker-compose logs -f"
fi

print_message "修复完成！"
