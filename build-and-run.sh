#!/bin/bash

# 构建和运行脚本
echo "开始构建Docker镜像..."

# 构建镜像
docker build -t stephenxiang777/bcmjfront:1.0 .

if [ $? -eq 0 ]; then
    echo "镜像构建成功！"
    
    # 停止并删除旧容器（如果存在）
    docker stop zr-admin-vue3 2>/dev/null
    docker rm zr-admin-vue3 2>/dev/null
    
    # 运行新容器
    echo "启动容器..."
    docker run -d \
        --name zr-admin-vue3 \
        -p 8080:80 \
        --restart unless-stopped \
        stephenxiang777/bcmjfront:1.0
    
    if [ $? -eq 0 ]; then
        echo "容器启动成功！"
        echo "应用地址: http://localhost:8080"
        echo "查看日志: docker logs -f zr-admin-vue3"
        echo "停止容器: docker stop zr-admin-vue3"
    else
        echo "容器启动失败！"
        exit 1
    fi
else
    echo "镜像构建失败！"
    exit 1
fi
