<template>
  <div class="dcbl-home">
    <section class="hero-card">
      <div>
        <p class="eyebrow">镇江市大成玻璃制造有限公司</p>
        <h1>镇江市大成玻璃制造有限公司库存管理操作流程（V1.0）</h1>
        <p class="hero-desc">按“先建资料、再做出入库、最后查台账”的顺序使用，库存变化一眼可追溯。</p>
      </div>
      <div class="hero-badge">
        <span>库存变化来源</span>
        <strong>入库 / 出库 / 库存调整</strong>
      </div>
    </section>

    <section class="flow-card">
      <div class="section-title">
        <span>一眼看懂主流程</span>
        <small>库存台账只负责查看，不直接改库存</small>
      </div>
      <div class="flow-list">
        <div v-for="(item, index) in flowSteps" :key="item.title" class="flow-step">
          <div class="step-index">{{ index + 1 }}</div>
          <h3>{{ item.title }}</h3>
          <p>{{ item.desc }}</p>
          <span class="step-page">{{ item.page }}</span>
          <div v-if="index < flowSteps.length - 1" class="flow-arrow">→</div>
        </div>
      </div>
    </section>

    <section class="content-grid">
      <div class="manual-card">
        <div class="section-title">
          <span>日常怎么用</span>
          <small>按业务场景选择入口</small>
        </div>
        <div class="usage-list">
          <div v-for="item in usageGuides" :key="item.title" class="usage-item">
            <div class="usage-icon">{{ item.icon }}</div>
            <div>
              <h3>{{ item.title }}</h3>
              <p>{{ item.desc }}</p>
            </div>
          </div>
        </div>
      </div>

      <div class="manual-card">
        <div class="section-title">
          <span>各页面负责什么</span>
          <small>操作前先找对页面</small>
        </div>
        <div class="page-list">
          <div v-for="item in pageGuides" :key="item.page" class="page-item">
            <strong>{{ item.page }}</strong>
            <span>{{ item.desc }}</span>
          </div>
        </div>
      </div>
    </section>

    <section class="tips-card">
      <div class="section-title">
        <span>操作提醒</span>
        <small>减少库存差异</small>
      </div>
      <div class="tips-list">
        <div v-for="tip in tips" :key="tip" class="tip-item">{{ tip }}</div>
      </div>
    </section>
  </div>
</template>

<script setup>
const flowSteps = [
  {
    title: '维护基础资料',
    desc: '先录入配件、产品，产品可维护配件组成和用量。',
    page: '配件/产品管理'
  },
  {
    title: '登记入库',
    desc: '采购、生产或其他入库场景，填写数量和入库时间。',
    page: '入库管理'
  },
  {
    title: '登记出库',
    desc: '领用、销售或消耗时登记出库，并填写用途便于追踪。',
    page: '出库管理'
  },
  {
    title: '盘点调整',
    desc: '盘盈、盘亏等差异，通过增加或减少进行修正。',
    page: '库存调整'
  },
  {
    title: '查看台账',
    desc: '查看当前库存、最后更新时间和每一次库存变动流水。',
    page: '库存台账'
  }
]

const usageGuides = [
  {
    icon: '建',
    title: '第一次使用',
    desc: '先到“配件/产品管理”录入配件和产品；如果产品由配件组成，继续维护产品配件组成。'
  },
  {
    icon: '入',
    title: '库存增加',
    desc: '正常到货或生产入库，使用“入库管理”；盘盈或修正库存，使用“库存调整-增加”。'
  },
  {
    icon: '出',
    title: '库存减少',
    desc: '正常领用或销售出库，使用“出库管理”；盘亏或修正库存，使用“库存调整-减少”。'
  },
  {
    icon: '查',
    title: '追溯库存',
    desc: '到“库存台账”查看当前库存，点击“查看台账”可看入库、出库、修改和删除产生的流水。'
  }
]

const pageGuides = [
  { page: '配件/产品管理', desc: '维护名称、类型、单位、规格、备注和产品配件组成。' },
  { page: '入库管理', desc: '新增、查看、编辑、删除入库记录，保存后增加库存。' },
  { page: '出库管理', desc: '新增、查看、编辑、删除出库记录，保存后减少库存。' },
  { page: '库存调整', desc: '处理盘点差异，只提供新增调整、查看详情和删除记录。' },
  { page: '库存台账', desc: '只读查看当前库存和库存流水，不直接修改库存。' }
]

const tips = [
  '入库、出库、库存调整会影响库存，提交前请核对配件/产品、数量和时间。',
  '库存台账只用于查看；发现差异时，应通过库存调整处理。',
  '删除入库、出库或调整记录可能反向影响库存，请谨慎操作。',
  '下拉框缺少类型、单位、用途或调整原因时，请检查系统字典配置。'
]
</script>

<style scoped>
.dcbl-home {
  min-height: calc(100vh - 84px);
  padding: 24px;
  background: #f5f7fb;
  color: #1f2d3d;
}

.hero-card,
.flow-card,
.manual-card,
.tips-card {
  border-radius: 18px;
  background: #fff;
  box-shadow: 0 12px 32px rgba(31, 45, 61, 0.08);
}

.hero-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  padding: 30px 34px;
  background: linear-gradient(135deg, #1746a2 0%, #409eff 100%);
  color: #fff;
}

.eyebrow {
  margin: 0 0 8px;
  font-size: 14px;
  letter-spacing: 2px;
  opacity: 0.86;
}

.hero-card h1 {
  margin: 0;
  font-size: 34px;
  font-weight: 700;
}

.hero-desc {
  margin: 14px 0 0;
  font-size: 16px;
  opacity: 0.9;
}

.hero-badge {
  min-width: 230px;
  padding: 18px 22px;
  border: 1px solid rgba(255, 255, 255, 0.35);
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.16);
  text-align: center;
}

.hero-badge span {
  display: block;
  margin-bottom: 8px;
  font-size: 13px;
  opacity: 0.85;
}

.hero-badge strong {
  font-size: 18px;
}

.flow-card,
.tips-card {
  margin-top: 18px;
  padding: 24px;
}

.section-title {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 18px;
}

.section-title span {
  font-size: 20px;
  font-weight: 700;
}

.section-title small {
  color: #7a8799;
}

.flow-list {
  display: grid;
  grid-template-columns: repeat(5, minmax(0, 1fr));
  gap: 14px;
}

.flow-step {
  position: relative;
  min-height: 178px;
  padding: 18px;
  border: 1px solid #e6ebf2;
  border-radius: 16px;
  background: #fbfdff;
}

.step-index {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: #ecf5ff;
  color: #409eff;
  font-weight: 700;
}

.flow-step h3 {
  margin: 14px 0 8px;
  font-size: 18px;
}

.flow-step p {
  min-height: 44px;
  margin: 0;
  color: #5f6f86;
  line-height: 1.6;
}

.step-page {
  display: inline-block;
  margin-top: 14px;
  padding: 6px 10px;
  border-radius: 999px;
  background: #f0f9eb;
  color: #529b2e;
  font-size: 13px;
  font-weight: 600;
}

.flow-arrow {
  position: absolute;
  top: 50%;
  right: -18px;
  z-index: 1;
  width: 28px;
  height: 28px;
  margin-top: -14px;
  border-radius: 50%;
  background: #409eff;
  color: #fff;
  line-height: 28px;
  text-align: center;
  font-weight: 700;
}

.content-grid {
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(360px, 0.8fr);
  gap: 18px;
  margin-top: 18px;
}

.manual-card {
  padding: 24px;
}

.usage-list {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 14px;
}

.usage-item {
  display: flex;
  gap: 14px;
  padding: 16px;
  border-radius: 14px;
  background: #f8fbff;
}

.usage-icon {
  flex: 0 0 auto;
  width: 42px;
  height: 42px;
  border-radius: 12px;
  background: #409eff;
  color: #fff;
  line-height: 42px;
  text-align: center;
  font-weight: 700;
}

.usage-item h3 {
  margin: 0 0 6px;
  font-size: 16px;
}

.usage-item p,
.page-item span {
  margin: 0;
  color: #5f6f86;
  line-height: 1.6;
}

.page-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.page-item {
  display: grid;
  grid-template-columns: 112px 1fr;
  gap: 12px;
  padding: 13px 14px;
  border-radius: 12px;
  background: #f8fbff;
}

.page-item strong {
  color: #1746a2;
}

.tips-list {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 12px;
}

.tip-item {
  padding: 14px 16px;
  border-left: 4px solid #e6a23c;
  border-radius: 12px;
  background: #fdf6ec;
  color: #7a4f12;
  line-height: 1.6;
}

@media (max-width: 1200px) {
  .flow-list,
  .tips-list {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .flow-arrow {
    display: none;
  }

  .content-grid,
  .usage-list {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .dcbl-home {
    padding: 14px;
  }

  .hero-card,
  .section-title {
    align-items: flex-start;
    flex-direction: column;
  }

  .hero-card h1 {
    font-size: 26px;
  }

  .hero-badge {
    width: 100%;
  }

  .flow-list,
  .tips-list {
    grid-template-columns: 1fr;
  }

  .page-item {
    grid-template-columns: 1fr;
  }
}
</style>