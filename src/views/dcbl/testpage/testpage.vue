<template>
  <div class="app-container folder-page">
    <div class="folder-grid">
      <div
        v-for="item in folderList"
        :key="item.label"
        class="folder-item"
        @click="handleFolderClick(item)">
        <div class="folder-icon" :style="{ '--folder-color': item.color }">
          <svg viewBox="0 0 80 64" class="folder-svg" aria-hidden="true">
            <path
              class="folder-back"
              d="M4 14c0-3.3 2.7-6 6-6h18l6 6h36c3.3 0 6 2.7 6 6v34c0 3.3-2.7 6-6 6H10c-3.3 0-6-2.7-6-6V14z" />
            <path
              class="folder-front"
              d="M4 22c0-3.3 2.7-6 6-6h54c3.3 0 6 2.7 6 6v26c0 3.3-2.7 6-6 6H10c-3.3 0-6-2.7-6-6V22z" />
          </svg>
        </div>
        <span class="folder-label">{{ item.label }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
const folderList = [
  { label: '牙盖', color: '#f5a623' },
  { label: '奶嘴', color: '#4a90d9' },
  { label: '瓶子', color: '#50c878' },
  { label: '包装', color: '#9b59b6' },
  { label: '外采', color: '#e67e22' }
]

function handleFolderClick(item) {
  console.log('点击文件夹:', item.label)
}
</script>

<style scoped lang="scss">
.folder-page {
  padding: 24px;
}

.folder-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 32px 40px;
  align-items: flex-start;
}

.folder-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100px;
  cursor: pointer;
  user-select: none;
  transition: transform 0.2s ease;

  &:hover {
    transform: translateY(-4px);

    .folder-front {
      filter: brightness(1.05);
    }
  }

  &:active {
    transform: translateY(-1px);
  }
}

.folder-icon {
  width: 80px;
  height: 64px;
  margin-bottom: 10px;
}

.folder-svg {
  width: 100%;
  height: 100%;
  display: block;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.12));
}

.folder-back {
  fill: color-mix(in srgb, var(--folder-color) 75%, #000 25%);
}

.folder-front {
  fill: var(--folder-color);
  transition: filter 0.2s ease;
}

.folder-label {
  font-size: 14px;
  line-height: 1.4;
  color: var(--el-text-color-primary);
  text-align: center;
  word-break: keep-all;
}
</style>
