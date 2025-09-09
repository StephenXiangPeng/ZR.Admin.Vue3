#!/bin/bash

# Docker构建和部署脚本 - 服务器部署版本
# 使用方法: ./docker-build.sh [命令] [选项]

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 项目信息
PROJECT_NAME="zr-admin"
VERSION=$(date +%Y%m%d-%H%M%S)
REGISTRY=""  # Docker镜像仓库地址，如：registry.your-domain.com
IMAGE_TAG="${PROJECT_NAME}:${VERSION}"
LATEST_TAG="${PROJECT_NAME}:latest"

# 打印带颜色的消息
print_message() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# 检查Docker是否安装
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker未安装，请先安装Docker"
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose未安装，请先安装Docker Compose"
        exit 1
    fi
}

# 构建生产镜像
build_prod() {
    print_message "开始构建生产镜像..."
    
    # 构建镜像
    docker build -t ${LATEST_TAG} .
    docker tag ${LATEST_TAG} ${IMAGE_TAG}
    
    # 如果配置了镜像仓库，推送到仓库
    if [ -n "$REGISTRY" ]; then
        print_message "推送镜像到仓库..."
        docker tag ${LATEST_TAG} ${REGISTRY}/${LATEST_TAG}
        docker tag ${IMAGE_TAG} ${REGISTRY}/${IMAGE_TAG}
        docker push ${REGISTRY}/${LATEST_TAG}
        docker push ${REGISTRY}/${IMAGE_TAG}
        print_message "镜像已推送到仓库: ${REGISTRY}/${IMAGE_TAG}"
    fi
    
    print_message "生产镜像构建完成: ${IMAGE_TAG}"
}

# 构建开发镜像
build_dev() {
    print_message "开始构建开发镜像..."
    
    # 构建开发镜像
    docker build -f Dockerfile.dev -t ${PROJECT_NAME}-dev:latest .
    docker tag ${PROJECT_NAME}-dev:latest ${PROJECT_NAME}-dev:${VERSION}
    
    print_message "开发镜像构建完成: ${PROJECT_NAME}-dev:${VERSION}"
}

# 启动生产环境
start_prod() {
    print_message "启动生产环境..."
    docker-compose up -d zr-admin-frontend
    print_message "生产环境启动完成，访问地址: http://localhost"
}

# 启动开发环境
start_dev() {
    print_message "启动开发环境..."
    docker-compose --profile dev up -d zr-admin-dev
    print_message "开发环境启动完成，访问地址: http://localhost:8887"
}

# 停止所有服务
stop_all() {
    print_message "停止所有服务..."
    docker-compose down
    print_message "所有服务已停止"
}

# 清理镜像
cleanup() {
    print_warning "清理未使用的Docker镜像..."
    docker image prune -f
    print_message "清理完成"
}

# 显示帮助信息
show_help() {
    echo "使用方法: $0 [命令] [选项]"
    echo ""
    echo "命令:"
    echo "  build-prod    构建生产镜像"
    echo "  build-dev     构建开发镜像"
    echo "  start-prod    启动生产环境"
    echo "  start-dev     启动开发环境"
    echo "  start-proxy   启动带反向代理的生产环境"
    echo "  start-simple  启动简化版生产环境（推荐）"
    echo "  stop          停止所有服务"
    echo "  restart-prod  重启生产环境"
    echo "  restart-dev   重启开发环境"
    echo "  logs          查看日志"
    echo "  status        查看服务状态"
    echo "  cleanup       清理未使用的镜像"
    echo "  backup        备份数据"
    echo "  update        更新应用"
    echo "  help          显示此帮助信息"
    echo ""
    echo "服务器部署示例:"
    echo "  $0 build-prod    # 构建生产镜像"
    echo "  $0 start-prod    # 启动生产环境"
    echo "  $0 start-proxy   # 启动带反向代理的环境"
    echo "  $0 status        # 查看服务状态"
    echo "  $0 logs          # 查看日志"
    echo ""
    echo "环境变量:"
    echo "  REGISTRY        Docker镜像仓库地址"
    echo "  PROJECT_NAME    项目名称 (默认: zr-admin)"
    echo ""
    echo "配置文件:"
    echo "  env.production  生产环境配置"
    echo "  nginx/          Nginx配置文件"
}

# 查看日志
show_logs() {
    docker-compose logs -f
}

# 重启生产环境
restart_prod() {
    print_message "重启生产环境..."
    docker-compose restart zr-admin-frontend
    print_message "生产环境重启完成"
}

# 重启开发环境
restart_dev() {
    print_message "重启开发环境..."
    docker-compose --profile dev restart zr-admin-dev
    print_message "开发环境重启完成"
}

# 启动带反向代理的生产环境
start_proxy() {
    print_message "启动带反向代理的生产环境..."
    docker-compose --profile proxy up -d
    print_message "反向代理环境启动完成，访问地址: http://localhost:8080"
}

# 启动简化版生产环境
start_simple() {
    print_message "启动简化版生产环境..."
    docker-compose --profile simple up -d
    print_message "简化版环境启动完成，访问地址: http://localhost:8080"
}

# 查看服务状态
show_status() {
    print_message "服务状态:"
    docker-compose ps
    echo ""
    print_message "容器资源使用情况:"
    docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"
}

# 备份数据
backup_data() {
    print_message "备份数据..."
    BACKUP_DIR="./backups/$(date +%Y%m%d_%H%M%S)"
    mkdir -p "$BACKUP_DIR"
    
    # 备份配置文件
    cp -r nginx/ "$BACKUP_DIR/"
    cp docker-compose.yml "$BACKUP_DIR/"
    cp env.production "$BACKUP_DIR/" 2>/dev/null || true
    
    # 备份镜像
    docker save ${LATEST_TAG} | gzip > "$BACKUP_DIR/${PROJECT_NAME}_image.tar.gz"
    
    print_message "数据已备份到: $BACKUP_DIR"
}

# 更新应用
update_app() {
    print_message "更新应用..."
    
    # 拉取最新代码（如果使用git）
    if [ -d ".git" ]; then
        print_message "拉取最新代码..."
        git pull
    fi
    
    # 重新构建镜像
    build_prod
    
    # 重启服务
    restart_prod
    
    print_message "应用更新完成"
}

# 主函数
main() {
    check_docker
    
    case "${1:-help}" in
        "build-prod")
            build_prod
            ;;
        "build-dev")
            build_dev
            ;;
        "start-prod")
            start_prod
            ;;
        "start-dev")
            start_dev
            ;;
        "start-proxy")
            start_proxy
            ;;
        "start-simple")
            start_simple
            ;;
        "stop")
            stop_all
            ;;
        "restart-prod")
            restart_prod
            ;;
        "restart-dev")
            restart_dev
            ;;
        "logs")
            show_logs
            ;;
        "status")
            show_status
            ;;
        "backup")
            backup_data
            ;;
        "update")
            update_app
            ;;
        "cleanup")
            cleanup
            ;;
        "help"|*)
            show_help
            ;;
    esac
}

# 执行主函数
main "$@"
