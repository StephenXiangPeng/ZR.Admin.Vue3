#!/bin/bash

# 生产环境构建脚本
echo "开始生产环境构建..."

# 设置环境变量
export VITE_APP_TITLE="奔驰马具生产管理系统"
export VITE_APP_ROUTER_PREFIX="/"
export VITE_APP_BASE_API="/prod-api"
export VITE_APP_API_HOST="http://localhost:8888"
export VITE_APP_SOCKET_API="ws://localhost:8888"
export VITE_APP_UPLOAD_URL="/common/upload"

# 构建项目
echo "构建项目..."
yarn build:prod

if [ $? -eq 0 ]; then
    echo "构建成功！"
    echo "构建输出目录: dist/"
else
    echo "构建失败！"
    exit 1
fi
