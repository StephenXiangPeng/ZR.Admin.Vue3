#!/bin/bash

# 构建脚本 - 确保环境变量正确传递

echo "开始构建前端应用..."

# 设置环境变量
export VITE_APP_TITLE="Order System"
export VITE_APP_VERSION="v20240105"
export VITE_APP_BASE_API="/api"
export VITE_APP_ROUTER_PREFIX="/"
export VITE_APP_DEBUG="false"
export VITE_APP_MOCK="false"

# 显示环境变量
echo "环境变量设置："
echo "VITE_APP_TITLE=$VITE_APP_TITLE"
echo "VITE_APP_VERSION=$VITE_APP_VERSION"
echo "VITE_APP_BASE_API=$VITE_APP_BASE_API"

# 构建应用
echo "开始构建..."
yarn build:prod

echo "构建完成！"
