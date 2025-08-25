#!/bin/bash

echo "=== Docker构建测试 ==="

# 检查Docker是否运行
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker未运行，请启动Docker Desktop"
    exit 1
fi

echo "✅ Docker正在运行"

# 构建镜像
echo "🔨 开始构建镜像..."
docker build -t stephenxiang777/bcmjfront:1.0 .

if [ $? -eq 0 ]; then
    echo "✅ 镜像构建成功！"
    
    # 检查镜像是否存在
    if docker images | grep -q "stephenxiang777/bcmjfront"; then
        echo "✅ 镜像已创建"
        
        # 显示镜像信息
        echo "📦 镜像信息："
        docker images stephenxiang777/bcmjfront:1.0
        
        echo ""
        echo "🎉 构建成功！现在你可以运行以下命令启动容器："
        echo "   docker run -d -p 8080:80 --name zr-admin-vue3 stephenxiang777/bcmjfront:1.0"
        echo ""
        echo "   或者使用快速脚本："
        echo "   ./build-and-run.sh"
        
    else
        echo "❌ 镜像创建失败"
        exit 1
    fi
else
    echo "❌ 镜像构建失败！"
    exit 1
fi
