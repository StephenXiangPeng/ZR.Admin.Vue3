<template>
	<div>
		<div class="package-spec-toolbar">
			<span class="package-spec-title">包装计价规格</span>
			<el-button v-if="!disabled" type="primary" size="small" native-type="button" @click="addRow">
				新增规格
			</el-button>
		</div>
		<el-table v-loading="listLoading" :data="tableData" border stripe
			class="customer-info-table product-pack-spec-table" style="width: 100%; table-layout: fixed;"
			:header-cell-style="{ background: '#d1d5db', color: '#333', fontWeight: 'bold' }"
			:row-style="{ height: '20px' }" :cell-style="{ padding: '2px 0' }" :row-key="getRowKey">
			<el-table-column label="计量单位" min-width="110" align="center">
				<template #default="scope">
					<el-select v-model="scope.row.unit" placeholder="请选择" clearable filterable size="small"
						:disabled="disabled" style="width: 100%">
						<el-option v-for="dict in dicts.hr_calculate_unit" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</template>
			</el-table-column>
			<el-table-column label="包装方式" min-width="110" align="center">
				<template #default="scope">
					<el-select v-model="scope.row.packageType" placeholder="请选择" clearable filterable size="small"
						:disabled="disabled" style="width: 100%">
						<el-option v-for="dict in dicts.hr_packing" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</template>
			</el-table-column>
			<el-table-column label="MOQ" min-width="90" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.moq" placeholder="MOQ" size="small" :disabled="disabled"
						@input="(val) => scope.row.moq = filterInteger(val)" />
				</template>
			</el-table-column>
			<el-table-column label="议价数" min-width="80" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.negotiatedQty" placeholder="议价数" size="small" :disabled="disabled"
						@input="(val) => scope.row.negotiatedQty = filterInteger(val)" />
				</template>
			</el-table-column>
			<el-table-column label="定制数" min-width="80" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.customQty" placeholder="定制数" size="small" :disabled="disabled"
						@input="(val) => scope.row.customQty = filterInteger(val)" />
				</template>
			</el-table-column>
			<el-table-column label="价格条款" min-width="110" align="center">
				<template #default="scope">
					<el-select v-model="scope.row.priceTerm" placeholder="请选择" clearable filterable size="small"
						:disabled="disabled" style="width: 100%">
						<el-option v-for="dict in dicts.hr_pricing_term" :key="dict.dictCode" :label="dict.dictLabel"
							:value="dict.dictValue" />
					</el-select>
				</template>
			</el-table-column>
			<el-table-column label="销售价" min-width="88" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.salePrice" placeholder="销售价" size="small" :disabled="disabled"
						@input="(val) => scope.row.salePrice = filterDecimal(val, 4)" />
				</template>
			</el-table-column>
			<el-table-column label="含税+/-(%)" min-width="110" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.taxIncluded" placeholder="百分比" size="small" :disabled="disabled"
						@input="(val) => scope.row.taxIncluded = filterPercentage(val)">
						<template #suffix>%</template>
					</el-input>
				</template>
			</el-table-column>
			<el-table-column label="中包装量" min-width="85" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.innerPackageQty" placeholder="中包装" size="small" :disabled="disabled"
						@input="(val) => scope.row.innerPackageQty = filterInteger(val)" />
				</template>
			</el-table-column>
			<el-table-column label="外箱装量" min-width="85" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.outerPackageQty" placeholder="外箱" size="small" :disabled="disabled"
						@input="(val) => scope.row.outerPackageQty = filterInteger(val)" />
				</template>
			</el-table-column>
			<el-table-column label="长" min-width="72" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.length" placeholder="长" size="small" :disabled="disabled"
						@input="(val) => onSizeInput(scope.row, 'length', val)" />
				</template>
			</el-table-column>
			<el-table-column label="宽" min-width="72" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.width" placeholder="宽" size="small" :disabled="disabled"
						@input="(val) => onSizeInput(scope.row, 'width', val)" />
				</template>
			</el-table-column>
			<el-table-column label="高" min-width="72" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.height" placeholder="高" size="small" :disabled="disabled"
						@input="(val) => onSizeInput(scope.row, 'height', val)" />
				</template>
			</el-table-column>
			<el-table-column label="外箱体积" min-width="88" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.outerBoxVolume" placeholder="体积" size="small" :disabled="disabled"
						@input="(val) => scope.row.outerBoxVolume = filterDecimal(val, 6)" />
				</template>
			</el-table-column>
			<el-table-column label="净重" min-width="72" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.netWeight" placeholder="净重" size="small" :disabled="disabled"
						@input="(val) => scope.row.netWeight = filterDecimal(val, 4)" />
				</template>
			</el-table-column>
			<el-table-column label="毛重" min-width="72" align="center">
				<template #default="scope">
					<el-input v-model="scope.row.grossWeight" placeholder="毛重" size="small" :disabled="disabled"
						@input="(val) => scope.row.grossWeight = filterDecimal(val, 4)" />
				</template>
			</el-table-column>
			<el-table-column label="备注" min-width="120" align="center" v-if="false">
				<template #default="scope">
					<el-input v-model="scope.row.remark" placeholder="备注" size="small" maxlength="500"
						:disabled="disabled" />
				</template>
			</el-table-column>
			<el-table-column v-if="!disabled" label="操作" width="120" align="center" fixed="right">
				<template #default="scope">
					<el-button v-if="hasProductId" link type="primary" size="small" native-type="button"
						:loading="isRowSaving(scope.row)" @click="saveRow(scope.row)">保存</el-button>
					<el-button link type="danger" size="small" native-type="button"
						:disabled="isRowSaving(scope.row) || (!hasProductId && !isNewRow(scope.row))"
						@click="handleDelete(scope.row, scope.$index)">删除</el-button>
				</template>
			</el-table-column>
		</el-table>
	</div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
	addProductPackageSpec,
	buildProductPackageSpecSavePayload,
	delProductPackageSpec,
	listProductPackageSpec,
	normalizeProductPackageSpec,
	updateProductPackageSpec,
	unwrapProductPackageSpecList,
	type ProductPackageSpec,
	type ProductPackageSpecSavePayload
} from '@/api/product/packageSpec'

type PackageSpecRow = ProductPackageSpec & { _localKey?: string }

const props = defineProps({
	productId: { type: [Number, String], default: 0 },
	disabled: { type: Boolean, default: false },
	dicts: {
		type: Object,
		default: () => ({
			hr_calculate_unit: [],
			hr_packing: [],
			hr_pricing_term: []
		})
	}
})

const tableData = ref<PackageSpecRow[]>([])
const listLoading = ref(false)
const savingKeys = ref<Record<string, boolean>>({})
let localKeySeq = 0

const hasProductId = computed(() => {
	const id = props.productId
	if (id === null || id === undefined || id === '') return false
	return String(id) !== '0'
})

const isNewRow = (row: PackageSpecRow) => {
	return row.id === null || row.id === undefined || row.id === ''
}

const getRowKey = (row: PackageSpecRow) => {
	if (!isNewRow(row)) return String(row.id)
	return row._localKey || ''
}

const isRowSaving = (row: PackageSpecRow) => !!savingKeys.value[getRowKey(row)]

const createEmptyRow = (): PackageSpecRow => ({
	_localKey: `new-${++localKeySeq}`,
	id: null,
	productID: props.productId,
	unit: null,
	packageType: null,
	moq: null,
	negotiatedQty: null,
	customQty: null,
	priceTerm: null,
	salePrice: null,
	taxIncluded: null,
	innerPackageQty: null,
	outerPackageQty: null,
	length: null,
	width: null,
	height: null,
	outerBoxVolume: null,
	netWeight: null,
	grossWeight: null,
	remark: null
})

const filterInteger = (val: string) => String(val ?? '').replace(/[^\d]/g, '')

const filterDecimal = (val: string, digits: number) => {
	let s = String(val ?? '').replace(/[^\d.]/g, '')
	const firstDot = s.indexOf('.')
	if (firstDot !== -1) {
		s = s.slice(0, firstDot + 1) + s.slice(firstDot + 1).replace(/\./g, '')
	}
	const parts = s.split('.')
	if (parts[1] && parts[1].length > digits) {
		s = parts[0] + '.' + parts[1].slice(0, digits)
	}
	return s
}

const filterPercentage = (val: string) => {
	const text = String(val ?? '')
	return (text.startsWith('-') ? '-' : '') + filterDecimal(text, 4)
}

const calcVolume = (row: PackageSpecRow) => {
	const length = Number(row.length)
	const width = Number(row.width)
	const height = Number(row.height)
	if (!length || !width || !height) {
		return
	}
	row.outerBoxVolume = ((length / 100) * (width / 100) * (height / 100)).toFixed(6)
}

const onSizeInput = (row: PackageSpecRow, field: 'length' | 'width' | 'height', val: string) => {
	row[field] = filterDecimal(val, 4)
	calcVolume(row)
}

const toNullableInt = (val: any) => {
	if (val === '' || val === null || val === undefined) return null
	const n = Number(val)
	if (!Number.isFinite(n)) return null
	return Math.trunc(n)
}

const toNullableNumber = (val: any) => {
	if (val === '' || val === null || val === undefined) return null
	const n = Number(val)
	if (!Number.isFinite(n)) return null
	return n
}

const toNullableString = (val: any) => {
	if (val === '' || val === null || val === undefined) return null
	return String(val)
}

const isValidInteger = (val: any) => {
	if (val === '' || val === null || val === undefined) return true
	return /^(0|[1-9]\d*)$/.test(String(val))
}

const isValidDecimal = (val: any, digits: number) => {
	if (val === '' || val === null || val === undefined) return true
	const pattern = new RegExp(`^(0|[1-9]\\d*)(\\.\\d{1,${digits}})?$`)
	return pattern.test(String(val))
}

const validateRow = (row: PackageSpecRow) => {
	if (toNullableString(row.unit) && String(row.unit).length > 50) return '计量单位最多50个字符'
	if (toNullableString(row.packageType) && String(row.packageType).length > 50) return '包装方式最多50个字符'
	if (toNullableString(row.priceTerm) && String(row.priceTerm).length > 50) return '价格条款最多50个字符'
	if (!isValidInteger(row.moq) || !isValidInteger(row.negotiatedQty) || !isValidInteger(row.customQty)
		|| !isValidInteger(row.innerPackageQty) || !isValidInteger(row.outerPackageQty)) {
		return '数量字段只能输入非负整数'
	}
	if (!isValidDecimal(row.salePrice, 4) || !isValidDecimal(row.length, 4) || !isValidDecimal(row.width, 4)
		|| !isValidDecimal(row.height, 4) || !isValidDecimal(row.netWeight, 4) || !isValidDecimal(row.grossWeight, 4)) {
		return '销售价、长、宽、高、净重、毛重最多4位小数'
	}
	if (row.taxIncluded !== null && row.taxIncluded !== undefined && row.taxIncluded !== ''
		&& !/^-?\d+(\.\d{0,4})?$/.test(String(row.taxIncluded))) {
		return '含税百分比请输入有效数字，最多4位小数，可为负数'
	}
	if (!isValidDecimal(row.outerBoxVolume, 6)) return '外箱体积最多6位小数'
	if (toNullableString(row.remark) && String(row.remark).length > 500) return '备注最多500个字符'
	return ''
}

const buildPayload = (row: PackageSpecRow, productId: string | number = props.productId): ProductPackageSpecSavePayload => {
	return buildProductPackageSpecSavePayload(row, productId, !isNewRow(row))
}

const mapListItem = (item: any): PackageSpecRow => {
	const row = normalizeProductPackageSpec(item) as PackageSpecRow
	if (row.taxIncluded !== null && row.taxIncluded !== undefined && row.taxIncluded !== '') {
		row.taxIncluded = Number(row.taxIncluded)
	}
	return row
}

const isRowEmpty = (row: PackageSpecRow) => {
	const fields = [
		'unit', 'packageType', 'moq', 'negotiatedQty', 'customQty', 'priceTerm', 'salePrice',
		'taxIncluded', 'innerPackageQty', 'outerPackageQty', 'length', 'width', 'height',
		'outerBoxVolume', 'netWeight', 'grossWeight', 'remark'
	]
	return fields.every((key) => {
		const val = (row as any)[key]
		return val === null || val === undefined || val === ''
	})
}

const loadList = async (excludeLocalKey?: string) => {
	if (!hasProductId.value) {
		return
	}
	const unsaved = tableData.value.filter((row) => isNewRow(row) && row._localKey !== excludeLocalKey)
	listLoading.value = true
	try {
		const response: any = await listProductPackageSpec(props.productId)
		if (response?.code == 200) {
			tableData.value = unwrapProductPackageSpecList(response.data).map(mapListItem).concat(unsaved)
		} else {
			tableData.value = unsaved
		}
	} catch (error) {
		tableData.value = unsaved
	} finally {
		listLoading.value = false
	}
}

const addRow = () => {
	tableData.value.push(createEmptyRow())
}

const reset = () => {
	tableData.value = []
	savingKeys.value = {}
}

const validatePending = () => {
	const rows = tableData.value.filter((row) => isNewRow(row) && !isRowEmpty(row))
	for (const row of rows) {
		const message = validateRow(row)
		if (message) {
			ElMessage.warning(message)
			return false
		}
	}
	return true
}

const getPendingRows = () => {
	return tableData.value
		.filter((row) => isNewRow(row) && !isRowEmpty(row))
		.map((row) => ({ ...row }))
}

const savePendingRows = async (productId: string | number, rows?: PackageSpecRow[]) => {
	if (productId === null || productId === undefined || productId === '' || String(productId) === '0') {
		return false
	}
	const pending = (rows && rows.length ? rows : getPendingRows())
	if (!pending.length) return true
	for (const row of pending) {
		const message = validateRow(row)
		if (message) {
			ElMessage.warning(message)
			return false
		}
	}
	for (const row of pending) {
		const response: any = await addProductPackageSpec(buildPayload(row, productId))
		if (response?.code != 200) return false
	}
	return true
}

const saveRow = async (row: PackageSpecRow) => {
	if (!hasProductId.value) {
		ElMessage.warning('请先保存产品后再新增包装计价规格')
		return
	}
	const message = validateRow(row)
	if (message) {
		ElMessage.warning(message)
		return
	}
	const key = getRowKey(row)
	if (savingKeys.value[key]) return
	savingKeys.value = { ...savingKeys.value, [key]: true }
	try {
		const isAdd = isNewRow(row)
		const response: any = isAdd
			? await addProductPackageSpec(buildPayload(row))
			: await updateProductPackageSpec(buildPayload(row))
		if (response?.code == 200) {
			ElMessage.success(response.msg || (isAdd ? '新增成功' : '修改成功'))
			const localKey = row._localKey
			const data = response.data
			if (data && typeof data === 'object' && !Array.isArray(data)) {
				Object.assign(row, mapListItem(data))
				delete row._localKey
			} else if (data !== null && data !== undefined && data !== '' && (typeof data === 'number' || typeof data === 'string')) {
				row.id = String(data)
				delete row._localKey
			} else {
				await loadList(localKey)
			}
		}
	} finally {
		const next = { ...savingKeys.value }
		delete next[key]
		savingKeys.value = next
	}
}

const handleDelete = (row: PackageSpecRow, index: number) => {
	if (isNewRow(row)) {
		tableData.value.splice(index, 1)
		return
	}
	ElMessageBox.confirm('确定要删除该包装计价规格吗？', '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning'
	}).then(async () => {
		listLoading.value = true
		try {
			const response: any = await delProductPackageSpec(String(row.id))
			if (response?.code == 200) {
				ElMessage.success(response.msg || '删除成功')
				await loadList()
			}
		} finally {
			listLoading.value = false
		}
	}).catch(() => { })
}

watch(
	() => props.productId,
	() => {
		tableData.value = []
		loadList()
	},
	{ immediate: true }
)

defineExpose({ loadList, reset, validatePending, getPendingRows, savePendingRows })
</script>

<style scoped>
.package-spec-toolbar {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 8px;
}

.package-spec-title {
	font-size: 14px;
	font-weight: bold;
	color: #333;
}

.product-pack-spec-table :deep(.el-table__header th.el-table__cell) {
	padding-top: 6px !important;
	padding-bottom: 6px !important;
	line-height: 1.2;
}

.product-pack-spec-table :deep(.el-table__body tr.el-table__row) {
	height: auto !important;
}

.product-pack-spec-table :deep(.el-table__body td.el-table__cell) {
	padding-top: 2px !important;
	padding-bottom: 2px !important;
	height: auto !important;
}

.product-pack-spec-table :deep(.el-input),
.product-pack-spec-table :deep(.el-select) {
	width: 100%;
}

.product-pack-spec-table :deep(.el-input__wrapper),
.product-pack-spec-table :deep(.el-select__wrapper) {
	min-height: 24px;
	padding: 0 6px;
}

.product-pack-spec-table :deep(.el-button + .el-button) {
	margin-left: 4px;
}
</style>
