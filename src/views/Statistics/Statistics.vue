<template>
	<div class="app-container">
		<el-card shadow="never">
			<template #header>
				<div class="card-header">
					<span>客户来源分布</span>
				</div>
			</template>
			<div class="toolbar">
				<span class="toolbar-label">统计维度</span>
				<el-select v-model="selectedDimension" placeholder="请选择统计维度" clearable filterable style="width: 260px">
					<el-option v-for="dict in optionss.hr_customer_source_statistics_dimension" :key="dict.dictCode"
						:label="dict.dictLabel" :value="dict.dictValue" />
				</el-select>
			</div>
			<div v-if="isTradingCountryDimension" class="dual-charts dual-charts-row">
				<div class="chart-panel">
					<div class="chart-block-head">
						<div class="chart-block-title">大洲分布</div>
						<div class="chart-hint">点击某一洲可筛选右侧国家</div>
					</div>
					<div ref="continentChartRef" class="chart-wrap chart-wrap-half" />
				</div>
				<div class="chart-panel">
					<div class="chart-block-head">
						<div class="chart-block-title">
							<span>国家分布</span>
							<el-button v-if="selectedContinentFilter" type="primary" link class="chart-reset-btn"
								@click="clearContinentFilter">
								显示全部国家
							</el-button>
						</div>
						<div class="chart-hint chart-hint--spacer" aria-hidden="true">&nbsp;</div>
					</div>
					<div ref="countryChartRef" class="chart-wrap chart-wrap-half" />
				</div>
			</div>
			<div v-else ref="chartRef" class="chart-wrap" />
		</el-card>

		<el-card shadow="never" class="section-card">
			<template #header>
				<div class="card-header">
					<span>产品销售统计（地区 · 金额：人民币）</span>
				</div>
			</template>
			<div class="toolbar sales-toolbar">
				<el-select v-model="salesTimeType" class="sales-toolbar-item" style="width: 120px">
					<el-option label="按月" value="month" />
					<el-option label="按季" value="quarter" />
					<el-option label="按年" value="year" />
				</el-select>
				<span class="toolbar-label">年份</span>
				<el-input-number v-model="salesYear" :min="2000" :max="2099" controls-position="right"
					class="sales-toolbar-item sales-year-input" />
				<template v-if="salesTimeType === 'month'">
					<span class="toolbar-label">月份</span>
					<el-select v-model="salesMonth" class="sales-toolbar-item" style="width: 100px">
						<el-option v-for="m in 12" :key="m" :label="`${m} 月`" :value="m" />
					</el-select>
				</template>
				<template v-if="salesTimeType === 'quarter'">
					<span class="toolbar-label">季度</span>
					<el-select v-model="salesQuarter" class="sales-toolbar-item" style="width: 120px">
						<el-option v-for="q in 4" :key="q" :label="`第 ${q} 季度`" :value="q" />
					</el-select>
				</template>
				<el-button type="primary" class="sales-toolbar-item" :loading="salesRegionLoading"
					@click="loadRegionSalesPieData">
					查询
				</el-button>
			</div>
			<div v-if="regionSalesMetaText" class="sales-range-hint">{{ regionSalesMetaText }}</div>
			<div class="dual-charts dual-charts-row">
				<div class="chart-panel">
					<div class="chart-block-head">
						<div class="chart-block-title">销售额 — 大洲</div>
						<div class="chart-hint">点击某一洲可筛选右侧国家</div>
					</div>
					<div ref="salesContinentChartRef" class="chart-wrap chart-wrap-half" />
				</div>
				<div class="chart-panel">
					<div class="chart-block-head">
						<div class="chart-block-title">
							<span>销售额 — 国家</span>
							<el-button v-if="selectedSalesContinentFilter" type="primary" link class="chart-reset-btn"
								@click="clearSalesContinentFilter">
								显示全部国家
							</el-button>
						</div>
						<div class="chart-hint chart-hint--spacer" aria-hidden="true">&nbsp;</div>
					</div>
					<div ref="salesCountryChartRef" class="chart-wrap chart-wrap-half" />
					<div v-if="salesCountryCategoryTableRows.length" class="sales-cat-table-wrap">
						<div class="sales-cat-table-title">各国家 · 产品分类明细（数量 / 金额 ¥）</div>
						<el-table :data="salesCountryCategoryTableRows" border stripe size="small" max-height="260"
							class="sales-cat-table">
							<el-table-column prop="country" label="国家" min-width="100" show-overflow-tooltip />
							<el-table-column prop="category" label="产品分类" min-width="120" show-overflow-tooltip />
							<el-table-column prop="quantity" label="数量" width="100" align="right" />
							<el-table-column prop="amountDisplay" label="金额（¥）" width="128" align="right" />
						</el-table>
					</div>
				</div>
			</div>
		</el-card>

		<el-card shadow="never" class="section-card">
			<template #header>
				<div class="card-header">
					<span>产品销售趋势统计</span>
				</div>
			</template>
			<div class="toolbar sales-toolbar">
				<el-select v-model="trendTimeType" class="sales-toolbar-item" style="width: 120px">
					<el-option label="按季度" value="quarter" />
					<el-option label="按年" value="year" />
				</el-select>
				<span class="toolbar-label">年份</span>
				<el-input-number v-model="trendYear" :min="2000" :max="2099" controls-position="right"
					class="sales-toolbar-item sales-year-input" />
				<template v-if="trendTimeType === 'quarter'">
					<span class="toolbar-label">季度</span>
					<el-select v-model="trendQuarter" class="sales-toolbar-item" style="width: 120px">
						<el-option v-for="q in 4" :key="q" :label="`第 ${q} 季度`" :value="q" />
					</el-select>
				</template>
				<span class="toolbar-label">维度</span>
				<el-radio-group v-model="trendDimension" class="sales-toolbar-item trend-radio-group">
					<el-radio-button label="continent">大洲</el-radio-button>
					<el-radio-button label="country">国家</el-radio-button>
					<el-radio-button label="category">产品分类</el-radio-button>
				</el-radio-group>
				<span class="toolbar-label">指标</span>
				<el-radio-group v-model="trendMetric" class="sales-toolbar-item trend-radio-group">
					<el-radio-button label="amount">金额（¥）</el-radio-button>
					<el-radio-button label="quantity">数量</el-radio-button>
				</el-radio-group>
				<el-button type="primary" class="sales-toolbar-item" :loading="salesTrendLoading"
					@click="loadSalesTrendData">
					查询
				</el-button>
			</div>
			<div v-if="salesTrendMetaText" class="sales-range-hint">{{ salesTrendMetaText }}</div>
			<div ref="salesTrendChartRef" class="chart-wrap trend-chart-wrap" />
		</el-card>
	</div>
</template>

<script setup name="Statistics">
import { getCurrentInstance, reactive, toRefs, ref, watch, nextTick, onMounted, onBeforeUnmount, computed } from 'vue'
import * as echarts from 'echarts'
import request from '@/utils/request'

const proxy = getCurrentInstance().proxy
const chartRef = ref(null)
const continentChartRef = ref(null)
const countryChartRef = ref(null)
const salesContinentChartRef = ref(null)
const salesCountryChartRef = ref(null)
const salesTrendChartRef = ref(null)
let chartInstance = null
let continentChartInstance = null
let countryChartInstance = null
let salesContinentChartInstance = null
let salesCountryChartInstance = null
let salesTrendChartInstance = null

const state = reactive({
	optionss: {
		hr_customer_source_statistics_dimension: [],
		hr_business_scope: [],
		sys_customer_source: [],
		hr_nation: [],
		sql_continent: [],
	},
})
const { optionss } = toRefs(state)

const selectedDimension = ref('')
const statisticsPayload = ref(null)
/** 点击洲后筛选国家饼图：{ continentId, continentName } */
const selectedContinentFilter = ref(null)

const salesTimeType = ref('month')
const salesYear = ref(new Date().getFullYear())
const salesMonth = ref(new Date().getMonth() + 1)
const salesQuarter = ref(Math.floor(new Date().getMonth() / 3) + 1)
const regionSalesPayload = ref(null)
const selectedSalesContinentFilter = ref(null)
const salesRegionLoading = ref(false)
/** 产品分类 id → 名称（来自 GetAllProductCategories） */
const productCategoryLabelMap = ref(new Map())

const trendTimeType = ref('quarter')
const trendYear = ref(new Date().getFullYear())
const trendQuarter = ref(Math.floor(new Date().getMonth() / 3) + 1)
const trendDimension = ref('continent')
const trendMetric = ref('amount')
const salesTrendPayload = ref(null)
const salesTrendLoading = ref(false)

const dictParams = [
	{ dictType: 'hr_customer_source_statistics_dimension' },
	{ dictType: 'hr_business_scope' },
	{ dictType: 'sys_customer_source' },
	{ dictType: 'hr_nation' },
	{ dictType: 'sql_continent' },
]

/** 字典 dictValue 与饼图数据字段对应；1=业务范围 2=国家地区 3=来源 */
const DIMENSION_TO_STATS_KEY = {
	customerSourceStats: 'customerSourceStats',
	businessScopeStats: 'businessScopeStats',
	tradingCountryStats: 'tradingCountryStats',
	CustomerSourceStats: 'customerSourceStats',
	BusinessScopeStats: 'businessScopeStats',
	TradingCountryStats: 'tradingCountryStats',
	'1': 'businessScopeStats',
	'2': 'tradingCountryStats',
	'3': 'customerSourceStats',
}

const STATS_LIST_KEYS = {
	customerSourceStats: ['customerSourceStats', 'CustomerSourceStats'],
	businessScopeStats: ['businessScopeStats', 'BusinessScopeStats'],
	tradingCountryStats: ['tradingCountryStats', 'TradingCountryStats'],
}

function pickArray(obj, keys) {
	if (!obj || typeof obj !== 'object') return []
	for (const k of keys) {
		if (Array.isArray(obj[k])) return obj[k]
	}
	return []
}

function normalizeStatisticsPayload(raw) {
	if (raw == null) return null
	let d = raw
	if (d.data && typeof d.data === 'object' && !Array.isArray(d.data)) {
		d = d.data
	}
	return {
		customerSourceStats: pickArray(d, STATS_LIST_KEYS.customerSourceStats),
		businessScopeStats: pickArray(d, STATS_LIST_KEYS.businessScopeStats),
		tradingCountryStats: pickArray(d, STATS_LIST_KEYS.tradingCountryStats),
	}
}

function formatRangeDate(v) {
	if (v == null || v === '') return ''
	const str = typeof v === 'string' ? v : String(v)
	if (str.includes('T')) return str.slice(0, 10)
	return str.length >= 10 ? str.slice(0, 10) : str
}

function formatMoneyAmount(val) {
	const num = typeof val === 'number' ? val : Number(val)
	if (!Number.isFinite(num)) return String(val ?? '—')
	return num.toLocaleString('zh-CN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function productCategoryLabel(id) {
	if (id == null || id === '' || Number(id) === 0) return '未分类'
	const key = String(id)
	return productCategoryLabelMap.value.get(key) ?? `分类（${key}）`
}

function flattenProductCategoriesToMap(nodes, map) {
	if (!Array.isArray(nodes)) return
	for (const n of nodes) {
		const id = n.value ?? n.id
		const label = n.label ?? n.name
		if (id != null && id !== '') map.set(String(id), String(label ?? id))
		if (n.children?.length) flattenProductCategoriesToMap(n.children, map)
	}
}

async function loadProductCategoryLabels() {
	try {
		const res = await request({
			url: 'ProductInformation/GetProductCategories/GetAllProductCategories',
			method: 'get',
		})
		if (res.code != null && res.code != 200) return
		const map = new Map()
		const root = res.data ?? res
		flattenProductCategoriesToMap(Array.isArray(root) ? root : [], map)
		productCategoryLabelMap.value = map
	} catch {
		/* 无映射时仍展示分类 ID 占位 */
	}
}

const chartTitle = computed(() => {
	const list = state.optionss.hr_customer_source_statistics_dimension || []
	const hit = list.find((d) => String(d.dictValue) === String(selectedDimension.value))
	return hit?.dictLabel || '统计分布'
})

const regionSalesMetaText = computed(() => {
	const p = regionSalesPayload.value
	if (!p) return ''
	const s = p.startTime ?? p.StartTime
	const e = p.endTime ?? p.EndTime
	if (s == null || e == null) return ''
	return `统计区间：${formatRangeDate(s)} ～ ${formatRangeDate(e)}`
})

const salesTrendMetaText = computed(() => {
	const p = salesTrendPayload.value
	if (!p) return ''
	const s = p.startTime ?? p.StartTime
	const e = p.endTime ?? p.EndTime
	if (s == null || e == null) return ''
	return `统计区间：${formatRangeDate(s)} ～ ${formatRangeDate(e)}`
})

function resolveStatsKey(dimension) {
	const raw = dimension
	const d = String(raw ?? '').trim()
	if (d === '' && raw !== 0 && raw !== '0') return null
	const keyFromMap = DIMENSION_TO_STATS_KEY[d]
	if (keyFromMap) return keyFromMap
	if (['customerSourceStats', 'businessScopeStats', 'tradingCountryStats'].includes(d)) return d
	const list = state.optionss.hr_customer_source_statistics_dimension || []
	const hit = list.find((x) => String(x.dictValue) === d)
	if (hit?.dictLabel) {
		const lab = String(hit.dictLabel)
		if (/来源|source/i.test(lab)) return 'customerSourceStats'
		if (/业务|范围|scope/i.test(lab)) return 'businessScopeStats'
		if (/国|国家|贸易|country|地区/i.test(lab)) return 'tradingCountryStats'
	}
	return null
}

const isTradingCountryDimension = computed(() => resolveStatsKey(selectedDimension.value) === 'tradingCountryStats')

const salesCountryCategoryTableRows = computed(() => {
	if (!regionSalesPayload.value?.countryData?.length) return []
	let list = [...regionSalesPayload.value.countryData]
	const f = selectedSalesContinentFilter.value
	if (f && f.continentId != null && f.continentId !== '') {
		list = list.filter((item) => String(item.continentId ?? item.ContinentId ?? '') === String(f.continentId))
	}
	const rows = []
	for (const item of list) {
		const tid = item.tradeCountry ?? item.TradeCountry
		const country = labelForSlice('tradingCountryStats', tid)
		const cats = item.categories ?? item.Categories ?? []
		if (!cats.length) {
			rows.push({
				country,
				category: '—',
				quantity: item.quantity ?? item.Quantity ?? '—',
				amountDisplay: formatMoneyAmount(item.amount ?? item.Amount ?? 0),
			})
			continue
		}
		for (const c of cats) {
			const pid = c.productCategoryId ?? c.ProductCategoryId
			rows.push({
				country,
				category: productCategoryLabel(pid),
				quantity: c.quantity ?? c.Quantity ?? '—',
				amountDisplay: formatMoneyAmount(c.amount ?? c.Amount ?? 0),
			})
		}
	}
	return rows
})

function getStatsList(payload, statsKey) {
	if (!payload || !statsKey) return []
	const arr = payload[statsKey]
	return Array.isArray(arr) ? arr : []
}

function getNationDictRow(countryKey) {
	const key = String(countryKey)
	return (state.optionss.hr_nation || []).find((o) => String(o.dictValue) === key)
}

function continentLabelForNation(countryKey) {
	const row = getNationDictRow(countryKey)
	const r = row?.remark != null ? String(row.remark).trim() : ''
	return r || '其他'
}

/** 客户来源-国家地区：大洲名称优先 sql_continent（continentId），其次接口 continentName，再国家字典 remark */
function continentLabelForCustomerStatsRow(item, countryKey) {
	const cid = item.continentId ?? item.ContinentId
	if (cid != null && cid !== '' && String(cid) !== '0') {
		const hit = (state.optionss.sql_continent || []).find((o) => String(o.dictValue) === String(cid))
		if (hit?.dictLabel) return hit.dictLabel
	}
	const raw = item.continentName ?? item.ContinentName
	if (raw != null && String(raw).trim() !== '') return String(raw).trim()
	return continentLabelForNation(countryKey)
}

function labelForSlice(statsKey, rawKey) {
	if (rawKey === null || rawKey === undefined || rawKey === '') {
		return '未分类'
	}
	const key = String(rawKey)
	if (statsKey === 'customerSourceStats') {
		const hit = (state.optionss.sys_customer_source || []).find((o) => String(o.dictValue) === key)
		return hit?.dictLabel ?? key
	}
	if (statsKey === 'businessScopeStats') {
		const hit = (state.optionss.hr_business_scope || []).find((o) => String(o.dictValue) === key)
		return hit?.dictLabel ?? key
	}
	if (statsKey === 'tradingCountryStats') {
		const hit = getNationDictRow(key)
		return hit?.dictLabel ?? key
	}
	return key
}

function rowsToPieData(statsKey, list) {
	if (!Array.isArray(list)) return []
	return list
		.map((item) => {
			let groupKey
			if (statsKey === 'customerSourceStats') {
				groupKey = item.customerSource ?? item.CustomerSource
			} else if (statsKey === 'businessScopeStats') {
				groupKey = item.businessScope ?? item.BusinessScope
			} else {
				groupKey = item.tradingCountry ?? item.TradingCountry
			}
			const value = Number(item.customerCount ?? item.CustomerCount ?? 0)
			return {
				name: labelForSlice(statsKey, groupKey),
				value,
			}
		})
		.filter((x) => x.name && !Number.isNaN(x.value))
}

/** 按洲汇总；扇区 data 带 continentId 供点击筛选 */
function tradingStatsToContinentPieData(list) {
	if (!Array.isArray(list)) return []
	const map = new Map()
	for (const item of list) {
		const value = Number(item.customerCount ?? item.CustomerCount ?? 0)
		if (Number.isNaN(value) || value <= 0) continue
		const countryKey = item.tradingCountry ?? item.TradingCountry
		const cid = item.continentId ?? item.ContinentId
		const cname = continentLabelForCustomerStatsRow(item, countryKey)
		const key =
			cid != null && cid !== '' && String(cid) !== '0' ? `id:${cid}` : `n:${cname}`
		const prev = map.get(key)
		if (prev) {
			prev.value += value
		} else {
			map.set(key, {
				name: cname,
				value,
				continentId: cid,
			})
		}
	}
	return [...map.values()].sort((a, b) => b.value - a.value)
}

/** filter 有值时只保留该洲下的国家；无 filter 显示全部国家 */
function tradingStatsToCountryPieData(list, filter) {
	if (!Array.isArray(list)) return []
	let rows = list
	if (filter) {
		const fid = filter.continentId
		if (fid !== undefined && fid !== null && fid !== '') {
			rows = list.filter((item) => String(item.continentId ?? item.ContinentId ?? '') === String(fid))
		} else if (filter.continentName) {
			const fn = String(filter.continentName).trim()
			rows = list.filter((item) => {
				const cn = String(item.continentName ?? item.ContinentName ?? '').trim()
				return cn === fn
			})
		}
	}
	const out = []
	for (const item of rows) {
		const countryKey = item.tradingCountry ?? item.TradingCountry
		const value = Number(item.customerCount ?? item.CustomerCount ?? 0)
		if (Number.isNaN(value) || value <= 0) continue
		const name = labelForSlice('tradingCountryStats', countryKey)
		if (!name) continue
		out.push({ name, value })
	}
	out.sort((a, b) => b.value - a.value)
	return out
}

function normalizeRegionSalesPayload(raw) {
	if (raw == null || typeof raw !== 'object') return null
	const d = raw.data && typeof raw.data === 'object' && !Array.isArray(raw.data) ? raw.data : raw
	return {
		...d,
		continentData: pickArray(d, ['continentData', 'ContinentData']),
		countryData: pickArray(d, ['countryData', 'CountryData']),
		startTime: d.startTime ?? d.StartTime,
		endTime: d.endTime ?? d.EndTime,
	}
}

function continentLabelByDictId(id) {
	if (id === null || id === undefined || id === '') return '未归类'
	const key = String(id)
	if (key === '0') return '未归类'
	const hit = (state.optionss.sql_continent || []).find((o) => String(o.dictValue) === key)
	return hit?.dictLabel ?? `大洲（${key}）`
}

function buildRegionSalesContinentPieData() {
	const list = regionSalesPayload.value?.continentData || []
	return list
		.map((item) => {
			const cid = item.continentId ?? item.ContinentId ?? 0
			const amount = Number(item.amount ?? item.Amount ?? 0)
			return {
				name: continentLabelByDictId(cid),
				value: amount,
				continentId: cid,
			}
		})
		.filter((x) => !Number.isNaN(x.value) && x.value > 0)
		.sort((a, b) => b.value - a.value)
}

function buildRegionSalesCountryPieData() {
	let list = regionSalesPayload.value?.countryData || []
	const f = selectedSalesContinentFilter.value
	if (f && f.continentId != null && f.continentId !== '') {
		list = list.filter((item) => String(item.continentId ?? item.ContinentId ?? '') === String(f.continentId))
	}
	return list
		.map((item) => {
			const tid = item.tradeCountry ?? item.TradeCountry
			const amount = Number(item.amount ?? item.Amount ?? 0)
			const name = labelForSlice('tradingCountryStats', tid)
			const catsRaw = item.categories ?? item.Categories ?? []
			const categoryRows = catsRaw.map((c) => ({
				productCategoryId: c.productCategoryId ?? c.ProductCategoryId,
				quantity: Number(c.quantity ?? c.Quantity ?? 0),
				amount: Number(c.amount ?? c.Amount ?? 0),
			}))
			const totalQty = Number(item.quantity ?? item.Quantity ?? NaN)
			return {
				name,
				value: amount,
				tradeCountry: tid,
				countryTotalQuantity: Number.isFinite(totalQty) ? totalQty : null,
				categoryRows,
			}
		})
		.filter((x) => !Number.isNaN(x.value) && x.value > 0 && x.name)
		.sort((a, b) => b.value - a.value)
}

function salesPieTooltipFormatter(params) {
	const v = params.value
	const txt = formatMoneyAmount(v)
	return `${params.marker}${params.seriesName}<br/>${params.name}：¥${txt}（${params.percent}%）`
}

function salesCountryPieTooltipFormatter(params) {
	const d = params.data || {}
	const pct = params.percent != null ? params.percent : '—'
	const money = formatMoneyAmount(params.value)
	const lines = [
		`${params.marker}<span style="font-weight:600">${params.name}</span>`,
		`销售合计：¥${money}（${pct}%）`,
	]
	if (d.countryTotalQuantity != null && Number.isFinite(d.countryTotalQuantity)) {
		lines.push(`总数量：${d.countryTotalQuantity}`)
	}
	const rows = d.categoryRows || []
	if (rows.length) {
		lines.push('')
		lines.push('<span style="font-weight:600">按产品分类</span>')
		for (const r of rows) {
			const cname = productCategoryLabel(r.productCategoryId)
			const q = Number.isFinite(r.quantity) ? r.quantity : r.quantity ?? '—'
			const a = formatMoneyAmount(r.amount)
			lines.push(`· ${cname} — 数量 ${q}，金额 ¥${a}`)
		}
	} else {
		lines.push('<span style="color:#999;font-size:12px">无分类明细</span>')
	}
	return lines.join('<br/>')
}

function ensureSalesChartInstances() {
	if (!salesContinentChartRef.value || !salesCountryChartRef.value) return false
	if (!salesContinentChartInstance) {
		salesContinentChartInstance = echarts.init(salesContinentChartRef.value)
		salesCountryChartInstance = echarts.init(salesCountryChartRef.value)
		bindSalesContinentChartClick()
	}
	return true
}

function renderSalesContinentChart() {
	if (!salesContinentChartInstance) return
	const data = buildRegionSalesContinentPieData()
	salesContinentChartInstance.clear()
	salesContinentChartInstance.setOption(
		{
			title: {
				text: '销售额 — 大洲',
				left: 'center',
				top: 4,
				textStyle: titleTextStyle,
			},
			tooltip: {
				trigger: 'item',
				formatter: salesPieTooltipFormatter,
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
				type: 'scroll',
				padding: [4, 16, 0, 16],
			},
			series: [
				{
					name: '大洲',
					type: 'pie',
					radius: ['36%', '60%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: true, formatter: (p) => p.name },
					data,
				},
			],
		},
		{ notMerge: true }
	)
}

function renderSalesCountryChart() {
	if (!salesCountryChartInstance) return
	const data = buildRegionSalesCountryPieData()
	const sub = selectedSalesContinentFilter.value?.continentName
		? ` — ${selectedSalesContinentFilter.value.continentName}`
		: ''
	salesCountryChartInstance.clear()
	salesCountryChartInstance.setOption(
		{
			title: {
				text: `销售额 — 国家${sub}`,
				left: 'center',
				top: 4,
				textStyle: titleTextStyle,
			},
			tooltip: {
				trigger: 'item',
				formatter: salesCountryPieTooltipFormatter,
				confine: true,
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
				type: 'scroll',
				padding: [4, 16, 0, 16],
			},
			series: [
				{
					name: '国家',
					type: 'pie',
					radius: ['36%', '60%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: {
						show: data.length <= 16,
						fontSize: 11,
						formatter: (p) => p.name,
					},
					labelLine: { show: data.length <= 16 },
					data,
				},
			],
		},
		{ notMerge: true }
	)
}

function bindSalesContinentChartClick() {
	if (!salesContinentChartInstance) return
	salesContinentChartInstance.off('click')
	salesContinentChartInstance.on('click', (params) => {
		const d = params.data
		if (!d) return
		selectedSalesContinentFilter.value = {
			continentId: d.continentId ?? d.ContinentId ?? null,
			continentName: d.name ?? '',
		}
		renderSalesCountryChart()
	})
}

function clearSalesContinentFilter() {
	selectedSalesContinentFilter.value = null
	renderSalesCountryChart()
}

async function loadRegionSalesPieData() {
	salesRegionLoading.value = true
	selectedSalesContinentFilter.value = null
	try {
		const params = {
			TimeType: salesTimeType.value,
			Year: Number(salesYear.value),
		}
		if (salesTimeType.value === 'month') params.Month = Number(salesMonth.value)
		if (salesTimeType.value === 'quarter') params.Quarter = Number(salesQuarter.value)

		const res = await request({
			url: 'Statistics/GetRegionSalesPieData/GetRegionSalesPieData',
			method: 'get',
			params,
		})
		if (res.code != null && res.code != 200) {
			regionSalesPayload.value = null
		} else {
			regionSalesPayload.value = normalizeRegionSalesPayload(res.data ?? res)
		}
	} catch {
		regionSalesPayload.value = null
	} finally {
		salesRegionLoading.value = false
		nextTick(() => {
			let attempts = 0
			const tryRender = () => {
				attempts += 1
				if (ensureSalesChartInstances()) {
					renderSalesContinentChart()
					renderSalesCountryChart()
				} else if (attempts < 6) {
					nextTick(tryRender)
				}
			}
			tryRender()
		})
	}
}

function normalizeSalesTrendPayload(raw) {
	if (raw == null || typeof raw !== 'object') return null
	const d = raw.data && typeof raw.data === 'object' && !Array.isArray(raw.data) ? raw.data : raw
	const xa = d.xAxis ?? d.XAxis
	return {
		...d,
		xAxis: Array.isArray(xa) ? xa : [],
		continentSeries: pickArray(d, ['continentSeries', 'ContinentSeries']),
		countrySeries: pickArray(d, ['countrySeries', 'CountrySeries']),
		categorySeries: pickArray(d, ['categorySeries', 'CategorySeries']),
		startTime: d.startTime ?? d.StartTime,
		endTime: d.endTime ?? d.EndTime,
	}
}

function normalizeTrendSeriesRow(s) {
	if (!s || typeof s !== 'object') return null
	const qty = s.quantityData ?? s.QuantityData
	const amt = s.amountData ?? s.AmountData
	return {
		key: s.key ?? s.Key ?? '',
		name: s.name ?? s.Name ?? '',
		quantityData: Array.isArray(qty) ? qty.map((n) => Number(n) || 0) : [],
		amountData: Array.isArray(amt) ? amt.map((n) => Number(n) || 0) : [],
	}
}

function getTrendSeriesRawList(payload, dimension) {
	if (!payload) return []
	let arr = []
	if (dimension === 'continent') arr = payload.continentSeries
	else if (dimension === 'country') arr = payload.countrySeries
	else arr = payload.categorySeries
	if (!Array.isArray(arr)) return []
	return arr.map(normalizeTrendSeriesRow).filter(Boolean)
}

function trendSeriesDisplayName(s, dimension) {
	const k = s.key
	if (dimension === 'continent') return continentLabelByDictId(Number(k) || k)
	if (dimension === 'country') return labelForSlice('tradingCountryStats', k)
	const nm = s.name != null && String(s.name).trim() !== '' ? String(s.name).trim() : ''
	return nm || productCategoryLabel(k)
}

function sumNumberArray(arr) {
	if (!Array.isArray(arr)) return 0
	return arr.reduce((acc, b) => acc + (Number(b) || 0), 0)
}

function mergeTrendSeriesData(seriesRows, dataKey) {
	const len = seriesRows[0]?.[dataKey]?.length ?? 0
	const out = Array.from({ length: len }, () => 0)
	for (const row of seriesRows) {
		const d = row[dataKey] || []
		for (let i = 0; i < len; i++) out[i] += Number(d[i]) || 0
	}
	return out
}

const TREND_MAX_LINES = 14

function buildTrimmedTrendSeriesForChart(payload, dimension, metric) {
	const raw = getTrendSeriesRawList(payload, dimension)
	if (!raw.length) return []
	const dataKey = metric === 'amount' ? 'amountData' : 'quantityData'
	const withSum = raw.map((s) => ({
		...s,
		displayName: trendSeriesDisplayName(s, dimension),
		_sum: sumNumberArray(s[dataKey]),
	}))
	const nonZero = withSum.filter((x) => x._sum > 0)
	nonZero.sort((a, b) => b._sum - a._sum)
	const top = nonZero.slice(0, TREND_MAX_LINES)
	const rest = nonZero.slice(TREND_MAX_LINES)
	const result = top.map(({ _sum, ...r }) => r)
	if (rest.length) {
		result.push({
			key: '__other__',
			name: '其他',
			displayName: '其他',
			quantityData: mergeTrendSeriesData(rest, 'quantityData'),
			amountData: mergeTrendSeriesData(rest, 'amountData'),
		})
	}
	return result
}

function renderSalesTrendChart() {
	if (!salesTrendChartRef.value) return
	if (!salesTrendChartInstance) {
		salesTrendChartInstance = echarts.init(salesTrendChartRef.value)
	}
	const p = salesTrendPayload.value
	const xLabels = Array.isArray(p?.xAxis) ? p.xAxis : []
	const trimmed = p ? buildTrimmedTrendSeriesForChart(p, trendDimension.value, trendMetric.value) : []
	const dataKey = trendMetric.value === 'amount' ? 'amountData' : 'quantityData'
	const isAmount = trendMetric.value === 'amount'
	const yAxisName = isAmount ? '金额（¥）' : '数量'

	const series = trimmed.map((s) => ({
		name: s.displayName || s.name || String(s.key),
		type: 'line',
		smooth: true,
		symbol: 'circle',
		symbolSize: 6,
		showSymbol: xLabels.length <= 18,
		data: (s[dataKey] || []).map((v) => (Number.isFinite(Number(v)) ? Number(v) : 0)),
	}))

	salesTrendChartInstance.clear()
	if (!xLabels.length && !series.length) {
		salesTrendChartInstance.setOption(
			{
				title: {
					text: '暂无趋势数据',
					left: 'center',
					top: 'center',
					textStyle: { fontSize: 14, color: 'var(--el-text-color-secondary)' },
				},
				xAxis: { type: 'category', data: [] },
				yAxis: { type: 'value' },
				series: [],
			},
			{ notMerge: true }
		)
		return
	}

	salesTrendChartInstance.setOption(
		{
			title: {
				text: '销售趋势',
				left: 'center',
				top: 6,
				textStyle: { fontSize: 14, fontWeight: 600, color: 'var(--el-text-color-primary)' },
			},
			grid: { left: 56, right: 24, top: 48, bottom: 80 },
			tooltip: {
				trigger: 'axis',
				confine: true,
				axisPointer: { type: 'cross' },
				formatter: (params) => {
					if (!params?.length) return ''
					const lines = [String(params[0].axisValue ?? '')]
					for (const it of params) {
						const v = it.value
						const txt = isAmount ? `¥${formatMoneyAmount(v)}` : String(v)
						lines.push(`${it.marker}${it.seriesName}：${txt}`)
					}
					return lines.join('<br/>')
				},
			},
			legend: {
				type: 'scroll',
				bottom: 4,
				data: series.map((x) => x.name),
			},
			xAxis: {
				type: 'category',
				boundaryGap: false,
				data: xLabels,
				axisLabel: { rotate: xLabels.length > 10 ? 35 : 0 },
			},
			yAxis: {
				type: 'value',
				name: yAxisName,
				axisLabel: {
					formatter: (v) => (isAmount ? formatMoneyAmount(v) : String(v)),
				},
			},
			series,
		},
		{ notMerge: true }
	)
}

async function loadSalesTrendData() {
	salesTrendLoading.value = true
	try {
		const params = {
			TimeType: trendTimeType.value,
			Year: Number(trendYear.value),
		}
		if (trendTimeType.value === 'quarter') {
			params.Quarter = Number(trendQuarter.value)
		}
		const res = await request({
			url: 'Statistics/GetSalesTrendData/GetSalesTrendData',
			method: 'get',
			params,
		})
		if (res.code != null && res.code != 200) {
			salesTrendPayload.value = null
		} else {
			salesTrendPayload.value = normalizeSalesTrendPayload(res.data ?? res)
		}
	} catch {
		salesTrendPayload.value = null
	} finally {
		salesTrendLoading.value = false
		nextTick(() => {
			let attempts = 0
			const tryRender = () => {
				attempts += 1
				if (salesTrendChartRef.value) {
					renderSalesTrendChart()
				} else if (attempts < 6) {
					nextTick(tryRender)
				}
			}
			tryRender()
		})
	}
}

watch([trendDimension, trendMetric], () => {
	if (salesTrendPayload.value) {
		nextTick(() => renderSalesTrendChart())
	}
})

function disposeSalesTrendChart() {
	salesTrendChartInstance?.dispose()
	salesTrendChartInstance = null
}

function disposeSalesCharts() {
	salesContinentChartInstance?.off('click')
	salesContinentChartInstance?.dispose()
	salesContinentChartInstance = null
	salesCountryChartInstance?.dispose()
	salesCountryChartInstance = null
}

function disposeAllCharts() {
	chartInstance?.dispose()
	chartInstance = null
	continentChartInstance?.off('click')
	continentChartInstance?.dispose()
	continentChartInstance = null
	countryChartInstance?.dispose()
	countryChartInstance = null
}

const pieItemStyle = {
	borderRadius: 4,
	borderColor: '#fff',
	borderWidth: 2,
}

const titleTextStyle = {
	fontSize: 14,
	fontWeight: 600,
	color: 'var(--el-text-color-primary)',
}

function renderPie(pieData, seriesName) {
	if (!chartInstance) return
	chartInstance.clear()
	chartInstance.setOption(
		{
			title: {
				text: seriesName || '统计分布',
				left: 'center',
				top: 8,
				textStyle: titleTextStyle,
			},
			tooltip: {
				trigger: 'item',
				formatter: '{b} : {c} ({d}%)',
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
			},
			series: [
				{
					name: seriesName || '统计分布',
					type: 'pie',
					radius: ['40%', '65%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: true },
					data: pieData,
				},
			],
		},
		{ notMerge: true }
	)
}

function renderContinentChart(list) {
	if (!continentChartInstance) return
	const data = tradingStatsToContinentPieData(list)
	continentChartInstance.clear()
	continentChartInstance.setOption(
		{
			title: {
				text: '大洲分布',
				left: 'center',
				top: 4,
				textStyle: titleTextStyle,
			},
			tooltip: {
				trigger: 'item',
				formatter: '{b}：{c}（{d}%）',
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
				type: 'scroll',
				padding: [4, 16, 0, 16],
			},
			series: [
				{
					name: '大洲',
					type: 'pie',
					radius: ['36%', '60%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: true },
					data,
				},
			],
		},
		{ notMerge: true }
	)
}

function renderCountryChart(list) {
	if (!countryChartInstance) return
	const data = tradingStatsToCountryPieData(list, selectedContinentFilter.value)
	const sub = selectedContinentFilter.value?.continentName
		? ` — ${selectedContinentFilter.value.continentName}`
		: ''
	countryChartInstance.clear()
	countryChartInstance.setOption(
		{
			title: {
				text: `国家分布${sub}`,
				left: 'center',
				top: 4,
				textStyle: titleTextStyle,
			},
			tooltip: {
				trigger: 'item',
				formatter: '{b}：{c}（{d}%）',
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
				type: 'scroll',
				padding: [4, 16, 0, 16],
			},
			series: [
				{
					name: '国家',
					type: 'pie',
					radius: ['36%', '60%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: data.length <= 16, fontSize: 11 },
					labelLine: { show: data.length <= 16 },
					data,
				},
			],
		},
		{ notMerge: true }
	)
}

function bindContinentChartClick() {
	if (!continentChartInstance) return
	continentChartInstance.off('click')
	continentChartInstance.on('click', (params) => {
		const d = params.data
		if (!d) return
		selectedContinentFilter.value = {
			continentId: d.continentId ?? d.ContinentId ?? null,
			continentName: d.name ?? '',
		}
		const list = getStatsList(statisticsPayload.value, 'tradingCountryStats')
		renderCountryChart(list)
	})
}

function clearContinentFilter() {
	selectedContinentFilter.value = null
	const list = getStatsList(statisticsPayload.value, 'tradingCountryStats')
	renderCountryChart(list)
}

function initSingleChartAndRender(pieData) {
	if (!chartRef.value) return
	chartInstance = echarts.init(chartRef.value)
	renderPie(pieData, chartTitle.value)
}

function initDualChartsAndRender(list) {
	if (!continentChartRef.value || !countryChartRef.value) return
	continentChartInstance = echarts.init(continentChartRef.value)
	countryChartInstance = echarts.init(countryChartRef.value)
	bindContinentChartClick()
	renderContinentChart(list)
	renderCountryChart(list)
}

function applyChartFromSelection() {
	disposeAllCharts()

	if (selectedDimension.value === '' || selectedDimension.value == null) {
		nextTick(() => initSingleChartAndRender([]))
		return
	}
	const statsKey = resolveStatsKey(selectedDimension.value)
	if (!statsKey || !statisticsPayload.value) {
		nextTick(() => initSingleChartAndRender([]))
		return
	}
	const list = getStatsList(statisticsPayload.value, statsKey)

	if (statsKey === 'tradingCountryStats') {
		nextTick(() => {
			if (continentChartRef.value && countryChartRef.value) {
				initDualChartsAndRender(list)
			} else {
				nextTick(() => initDualChartsAndRender(list))
			}
		})
		return
	}

	nextTick(() => {
		initSingleChartAndRender(rowsToPieData(statsKey, list))
	})
}

async function loadCustomerSourceStatistics() {
	try {
		const res = await request({
			url: 'Statistics/GetCustomerSourceStatistics/GetPieData',
			method: 'get',
		})
		if (res.code != null && res.code != 200) {
			statisticsPayload.value = null
			applyChartFromSelection()
			return
		}
		statisticsPayload.value = normalizeStatisticsPayload(res.data ?? res)
		applyChartFromSelection()
	} catch {
		statisticsPayload.value = null
		applyChartFromSelection()
	}
}

function onResize() {
	chartInstance?.resize()
	continentChartInstance?.resize()
	countryChartInstance?.resize()
	salesContinentChartInstance?.resize()
	salesCountryChartInstance?.resize()
	salesTrendChartInstance?.resize()
}

watch(selectedDimension, () => {
	selectedContinentFilter.value = null
	applyChartFromSelection()
})

onMounted(() => {
	window.addEventListener('resize', onResize)
})

proxy.getDicts(dictParams).then((response) => {
	response.data.forEach((el) => {
		state.optionss[el.dictType] = el.list
	})
	nextTick(async () => {
		const list = state.optionss.hr_customer_source_statistics_dimension
		if (list?.length) {
			selectedDimension.value = list[0].dictValue
		}
		await loadProductCategoryLabels()
		await loadCustomerSourceStatistics()
		await loadRegionSalesPieData()
		await loadSalesTrendData()
	})
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
	disposeAllCharts()
	disposeSalesCharts()
	disposeSalesTrendChart()
})
</script>

<style scoped>
.toolbar {
	margin-bottom: 16px;
	display: flex;
	align-items: center;
}

.toolbar-label {
	margin-right: 8px;
	color: var(--el-text-color-regular);
	font-size: 14px;
}

.chart-wrap {
	height: 420px;
	width: 100%;
}

.dual-charts {
	margin-top: 4px;
}

.dual-charts-row {
	display: flex;
	flex-flow: row nowrap;
	align-items: stretch;
	gap: 16px;
	width: 100%;
}

.chart-panel {
	flex: 1 1 0;
	min-width: 0;
	display: flex;
	flex-direction: column;
}

.chart-block-head {
	flex-shrink: 0;
	min-height: 48px;
	display: flex;
	flex-direction: column;
	justify-content: flex-start;
}

.chart-block-title {
	font-weight: 600;
	font-size: 14px;
	margin-bottom: 2px;
	display: flex;
	align-items: center;
	gap: 8px;
	flex-wrap: wrap;
}

.chart-hint {
	font-size: 12px;
	color: var(--el-text-color-secondary);
	line-height: 1.4;
	min-height: 1.4em;
}

.chart-hint--spacer {
	visibility: hidden;
	user-select: none;
}

.chart-reset-btn {
	font-size: 13px;
}

.chart-wrap-half {
	flex: 1 1 auto;
	width: 100%;
	min-height: 360px;
	height: 380px;
}

.card-header {
	font-weight: 600;
}

.section-card {
	margin-top: 20px;
}

.sales-toolbar {
	display: flex;
	align-items: center;
	flex-wrap: wrap;
	gap: 8px 12px;
	margin-bottom: 4px;
}

.sales-toolbar .toolbar-label {
	margin-right: 0;
}

.sales-year-input {
	width: 120px;
}

.sales-range-hint {
	font-size: 12px;
	color: var(--el-text-color-secondary);
	margin: 0 0 10px;
}

.sales-cat-table-wrap {
	margin-top: 8px;
	width: 100%;
}

.sales-cat-table-title {
	font-size: 13px;
	font-weight: 600;
	margin-bottom: 8px;
	color: var(--el-text-color-regular);
}

.sales-cat-table {
	width: 100%;
}

.trend-chart-wrap {
	height: 440px;
	margin-top: 4px;
}

.trend-radio-group {
	flex-wrap: wrap;
}
</style>
