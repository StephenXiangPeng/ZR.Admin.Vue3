FROM node:18
COPY ./ /app
WORKDIR /app
RUN yarn install
# 更新SVG插件到新版本并修复配置
RUN yarn add vite-plugin-svg-icons@latest --dev
RUN sed -i 's/import svgIcon from/import { createSvgIconsPlugin } from/' /app/vite/plugins/svg-icon.js \
    && sed -i 's/return svgIcon(/return createSvgIconsPlugin(/' /app/vite/plugins/svg-icon.js
RUN yarn build 

FROM nginx
RUN mkdir /app
COPY --from=0 /app/dist /app
COPY nginx.conf /etc/nginx/nginx.conf