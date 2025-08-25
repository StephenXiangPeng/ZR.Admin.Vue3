FROM node:18
COPY ./ /app
WORKDIR /app

# 设置环境变量
ENV VITE_APP_TITLE=奔驰马具生产管理系统
ENV VITE_APP_ROUTER_PREFIX=/
ENV VITE_APP_BASE_API=/prod-api
ENV VITE_APP_API_HOST=http://localhost:8888
ENV VITE_APP_SOCKET_API=ws://localhost:8888
ENV VITE_APP_UPLOAD_URL=/common/upload

RUN yarn install
# 更新SVG插件到新版本
RUN yarn add vite-plugin-svg-icons@latest --dev
RUN yarn build:prod 

FROM nginx
RUN mkdir /app
COPY --from=0 /app/dist /app
COPY nginx.conf /etc/nginx/nginx.conf

# 暴露端口
EXPOSE 80

# 启动nginx
CMD ["nginx", "-g", "daemon off;"]