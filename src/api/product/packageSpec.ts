import request from '@/utils/request'

/** 产品包装计价规格 */
export interface ProductPackageSpec {
	id?: string | number | null
	productID?: string | number | null
	unit?: string | null
	packageType?: string | null
	moq?: number | string | null
	negotiatedQty?: number | string | null
	customQty?: number | string | null
	priceTerm?: string | null
	salePrice?: number | string | null
	taxIncluded?: number | string | null
	innerPackageQty?: number | string | null
	outerPackageQty?: number | string | null
	length?: number | string | null
	width?: number | string | null
	height?: number | string | null
	outerBoxVolume?: number | string | null
	netWeight?: number | string | null
	grossWeight?: number | string | null
	remark?: string | null
}

/** 新增/修改提交体（后端 PascalCase） */
export interface ProductPackageSpecSavePayload {
	Id?: string | number
	id?: string | number
	ProductID: string | number
	productID?: string | number
	Unit?: string | null
	PackageType?: string | null
	MOQ?: number | null
	NegotiatedQty?: number | null
	CustomQty?: number | null
	PriceTerm?: string | null
	SalePrice?: number | null
	TaxIncluded?: number | null
	InnerPackageQty?: number | null
	OuterPackageQty?: number | null
	Length?: number | null
	Width?: number | null
	Height?: number | null
	OuterBoxVolume?: number | null
	NetWeight?: number | null
	GrossWeight?: number | null
	Remark?: string | null
}

/** long/snowflake 超过 JS 安全整数时，先转成字符串再 JSON.parse，避免 Id 精度丢失 */
function parseApiJson(data: any) {
	if (typeof data !== 'string') return data
	const text = data.replace(/(^|[:\[,]\s*)(-?\d{16,})(?=\s*[,\]}])/g, '$1"$2"')
	return JSON.parse(text)
}

function packageSpecRequest(config: Record<string, any>) {
	return request({
		...config,
		transformResponse: [parseApiJson]
	})
}

function toIdString(id: string | number | null | undefined) {
	if (id === null || id === undefined || id === '') return ''
	return String(id)
}

export function listProductPackageSpec(productID: string | number) {
	return packageSpecRequest({
		url: 'ProductPackageSpec/GetProductPackageSpecList/GetList',
		method: 'get',
		params: { productID: toIdString(productID) }
	})
}

export function getProductPackageSpec(id: string | number) {
	return packageSpecRequest({
		url: 'ProductPackageSpec/GetProductPackageSpec/Get',
		method: 'get',
		params: { id: toIdString(id) }
	})
}

export function addProductPackageSpec(data: ProductPackageSpecSavePayload) {
	return packageSpecRequest({
		url: 'ProductPackageSpec/AddProductPackageSpec/Add',
		method: 'post',
		data
	})
}

export function updateProductPackageSpec(data: ProductPackageSpecSavePayload) {
	return packageSpecRequest({
		url: 'ProductPackageSpec/EditProductPackageSpec/Edit',
		method: 'put',
		data
	})
}

export function delProductPackageSpec(id: string | number) {
	return packageSpecRequest({
		url: 'ProductPackageSpec/DeleteProductPackageSpec/Del',
		method: 'delete',
		params: { id: toIdString(id) }
	})
}

export function unwrapProductPackageSpecList(data: any): any[] {
	if (Array.isArray(data)) return data
	if (Array.isArray(data?.result)) return data.result
	if (Array.isArray(data?.Result)) return data.Result
	if (Array.isArray(data?.list)) return data.list
	if (Array.isArray(data?.List)) return data.List
	return []
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

export function buildProductPackageSpecSavePayload(row: ProductPackageSpec, productId: string | number, includeId = false): ProductPackageSpecSavePayload {
	const payload: ProductPackageSpecSavePayload = {
		ProductID: toIdString(productId),
		productID: toIdString(productId),
		Unit: toNullableString(row.unit),
		PackageType: toNullableString(row.packageType),
		MOQ: toNullableInt(row.moq),
		NegotiatedQty: toNullableInt(row.negotiatedQty),
		CustomQty: toNullableInt(row.customQty),
		PriceTerm: toNullableString(row.priceTerm),
		SalePrice: toNullableNumber(row.salePrice),
		TaxIncluded: toNullableNumber(row.taxIncluded),
		InnerPackageQty: toNullableInt(row.innerPackageQty),
		OuterPackageQty: toNullableInt(row.outerPackageQty),
		Length: toNullableNumber(row.length),
		Width: toNullableNumber(row.width),
		Height: toNullableNumber(row.height),
		OuterBoxVolume: toNullableNumber(row.outerBoxVolume),
		NetWeight: toNullableNumber(row.netWeight),
		GrossWeight: toNullableNumber(row.grossWeight),
		Remark: toNullableString(row.remark)
	}
	if (includeId && row.id !== null && row.id !== undefined && row.id !== '') {
		const specId = toIdString(row.id)
		payload.Id = specId
		payload.id = specId
	}
	return payload
}

export function normalizeProductPackageSpec(item: any): ProductPackageSpec {
	if (!item) return {}
	const id = item.id ?? item.Id ?? null
	const productID = item.productID ?? item.ProductID ?? item.productId ?? null
	return {
		id: id === null || id === undefined || id === '' ? null : String(id),
		productID: productID === null || productID === undefined || productID === '' ? null : String(productID),
		unit: item.unit ?? item.Unit ?? null,
		packageType: item.packageType ?? item.PackageType ?? null,
		moq: item.moq ?? item.MOQ ?? null,
		negotiatedQty: item.negotiatedQty ?? item.NegotiatedQty ?? null,
		customQty: item.customQty ?? item.CustomQty ?? null,
		priceTerm: item.priceTerm ?? item.PriceTerm ?? null,
		salePrice: item.salePrice ?? item.SalePrice ?? null,
		taxIncluded: item.taxIncluded ?? item.TaxIncluded ?? null,
		innerPackageQty: item.innerPackageQty ?? item.InnerPackageQty ?? null,
		outerPackageQty: item.outerPackageQty ?? item.OuterPackageQty ?? null,
		length: item.length ?? item.Length ?? null,
		width: item.width ?? item.Width ?? null,
		height: item.height ?? item.Height ?? null,
		outerBoxVolume: item.outerBoxVolume ?? item.OuterBoxVolume ?? null,
		netWeight: item.netWeight ?? item.NetWeight ?? null,
		grossWeight: item.grossWeight ?? item.GrossWeight ?? null,
		remark: item.remark ?? item.Remark ?? null
	}
}
