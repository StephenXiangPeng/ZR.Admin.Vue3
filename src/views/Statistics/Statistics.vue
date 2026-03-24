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
let chartInstance = null
let continentChartInstance = null
let countryChartInstance = null

const state = reactive({
	optionss: {
		hr_customer_source_statistics_dimension: [],
		hr_business_scope: [],
		sys_customer_source: [],
		hr_nation: [],
	},
})
const { optionss } = toRefs(state)

const selectedDimension = ref('')
const statisticsPayload = ref(null)
/** 点击洲后筛选国家饼图：{ continentId, continentName } */
const selectedContinentFilter = ref(null)

const dictParams = [
	{ dictType: 'hr_customer_source_statistics_dimension' },
	{ dictType: 'hr_business_scope' },
	{ dictType: 'sys_customer_source' },
	{ dictType: 'hr_nation' },
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

const chartTitle = computed(() => {
	const list = state.optionss.hr_customer_source_statistics_dimension || []
	const hit = list.find((d) => String(d.dictValue) === String(selectedDimension.value))
	return hit?.dictLabel || '统计分布'
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

function continentNameFromTradingRow(item, countryKey) {
	const raw = item.continentName ?? item.ContinentName
	if (raw != null && String(raw).trim() !== '') {
		return String(raw).trim()
	}
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
		const cnameRaw = item.continentName ?? item.ContinentName
		const cname =
			cnameRaw != null && String(cnameRaw).trim() !== ''
				? String(cnameRaw).trim()
				: continentNameFromTradingRow(item, countryKey)
		const key = cid != null && cid !== '' ? `id:${cid}` : `n:${cname}`
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
		await loadCustomerSourceStatistics()
	})
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
	disposeAllCharts()
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
</style>
