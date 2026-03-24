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
			<div ref="chartRef" class="chart-wrap" />
		</el-card>
	</div>
</template>

<script setup name="Statistics">
import { getCurrentInstance, reactive, toRefs, ref, watch, nextTick, onMounted, onBeforeUnmount, computed } from 'vue'
import * as echarts from 'echarts'
import request from '@/utils/request'

const proxy = getCurrentInstance().proxy
const chartRef = ref(null)
let chartInstance = null

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

const dictParams = [
	{ dictType: 'hr_customer_source_statistics_dimension' },
	{ dictType: 'hr_business_scope' },
	{ dictType: 'sys_customer_source' },
	{ dictType: 'hr_nation' },
]

/** 字典 dictValue 与饼图数据字段对应；1=业务范围 2=国家 3=来源；也支持 dictLabel 关键词匹配 */
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

/** 兼容 camelCase / PascalCase、以及 data 多包一层 */
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
		if (/国|国家|贸易|country/i.test(lab)) return 'tradingCountryStats'
	}
	return null
}

function getStatsList(payload, statsKey) {
	if (!payload || !statsKey) return []
	const arr = payload[statsKey]
	return Array.isArray(arr) ? arr : []
}

function getNationDictRow(countryKey) {
	const key = String(countryKey)
	return (state.optionss.hr_nation || []).find((o) => String(o.dictValue) === key)
}

/** 接口未返回 continentName 时：用国家字典 remark，再无则「其他」 */
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

/** 国家维度：内层国家、外层按洲汇总 */
function tradingCountryRowsToContinentDoublePie(list) {
	if (!Array.isArray(list)) return { innerData: [], outerData: [] }
	const continentTotals = new Map()
	const innerData = []
	for (const item of list) {
		const countryKey = item.tradingCountry ?? item.TradingCountry
		const value = Number(item.customerCount ?? item.CustomerCount ?? 0)
		if (Number.isNaN(value) || value <= 0) continue
		const name = labelForSlice('tradingCountryStats', countryKey)
		if (!name) continue
		innerData.push({ name, value })
		const cont = continentNameFromTradingRow(item, countryKey)
		continentTotals.set(cont, (continentTotals.get(cont) || 0) + value)
	}
	const outerData = [...continentTotals.entries()]
		.map(([name, value]) => ({ name, value }))
		.sort((a, b) => b.value - a.value)
	innerData.sort((a, b) => b.value - a.value)
	return { innerData, outerData }
}

function initChart() {
	if (!chartRef.value || chartInstance) return
	chartInstance = echarts.init(chartRef.value)
}

function renderPie(pieData, seriesName) {
	if (!chartInstance) initChart()
	if (!chartInstance) return
	chartInstance.clear()
	chartInstance.setOption(
		{
			title: {
				text: seriesName || '统计分布',
				left: 'center',
				top: 8,
				textStyle: { fontSize: 14, fontWeight: 600, color: 'var(--el-text-color-primary)' },
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

const pieItemStyle = {
	borderRadius: 4,
	borderColor: '#fff',
	borderWidth: 2,
}

function renderContinentCountryDoublePie(innerData, outerData, cardTitle) {
	if (!chartInstance) initChart()
	if (!chartInstance) return
	chartInstance.clear()
	chartInstance.setOption(
		{
			title: {
				text: cardTitle || '统计分布',
				left: 'center',
				top: 8,
				textStyle: { fontSize: 14, fontWeight: 600, color: 'var(--el-text-color-primary)' },
			},
			tooltip: {
				trigger: 'item',
				formatter: '{a}<br/>{b}：{c}（{d}%）',
			},
			legend: {
				orient: 'horizontal',
				bottom: 0,
				type: 'scroll',
				padding: [8, 24, 0, 24],
			},
			series: [
				{
					name: '国家',
					type: 'pie',
					radius: [0, '32%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: innerData.length <= 12, fontSize: 11 },
					labelLine: { show: innerData.length <= 12 },
					data: innerData,
				},
				{
					name: '大洲',
					type: 'pie',
					radius: ['38%', '58%'],
					center: ['50%', '52%'],
					avoidLabelOverlap: true,
					itemStyle: pieItemStyle,
					label: { show: true },
					data: outerData,
				},
			],
		},
		{ notMerge: true }
	)
}

function applyChartFromSelection() {
	if (selectedDimension.value === '' || selectedDimension.value == null) {
		renderPie([], chartTitle.value)
		return
	}
	const statsKey = resolveStatsKey(selectedDimension.value)
	if (!statsKey || !statisticsPayload.value) {
		renderPie([], chartTitle.value)
		return
	}
	const list = getStatsList(statisticsPayload.value, statsKey)
	if (statsKey === 'tradingCountryStats') {
		const { innerData, outerData } = tradingCountryRowsToContinentDoublePie(list)
		renderContinentCountryDoublePie(innerData, outerData, chartTitle.value)
		return
	}
	const pieData = rowsToPieData(statsKey, list)
	renderPie(pieData, chartTitle.value)
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
}

watch(selectedDimension, () => {
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
		initChart()
		const list = state.optionss.hr_customer_source_statistics_dimension
		if (list?.length) {
			selectedDimension.value = list[0].dictValue
		}
		await loadCustomerSourceStatistics()
	})
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
	chartInstance?.dispose()
	chartInstance = null
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

.card-header {
	font-weight: 600;
}
</style>
