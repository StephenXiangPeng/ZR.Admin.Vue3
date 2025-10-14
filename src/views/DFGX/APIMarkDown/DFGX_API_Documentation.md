# DFGX 订单管理系统 API 接口文档

## 概述

DFGX订单管理系统提供完整的订单管理功能，包括客户订单、订单详情、订单状态历史等模块的CRUD操作。

**基础URL**: `http://your-domain.com/api`

**认证方式**: Bearer Token

**API分组**: `dfgx`

---

## 1. 客户订单管理 (Customer Orders)

### 1.1 分页查询客户订单列表

**接口地址**: `GET /dfgx/customer-orders/list`

**权限要求**: `dfgx:customerOrders:list`

**请求参数**:

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| orderNo | string | 否 | 订单编号（模糊查询） |
| customerName | string | 否 | 客户姓名（模糊查询） |
| customerPhone | string | 否 | 客户电话（模糊查询） |
| status | string | 否 | 订单状态（0待处理 1生产中 2已完成 3已取消） |
| beginTime | datetime | 否 | 开始日期 |
| endTime | datetime | 否 | 结束日期 |
| minAmount | decimal | 否 | 最小金额 |
| maxAmount | decimal | 否 | 最大金额 |
| pageNum | int | 是 | 页码（默认1） |
| pageSize | int | 是 | 每页数量（默认10） |

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "total": 100,
    "result": [
      {
        "id": 1,
        "orderNo": "ORD202401010001",
        "customerName": "张三",
        "customerPhone": "13800138000",
        "customerEmail": "zhangsan@example.com",
        "orderDate": "2024-01-01 10:00:00",
        "status": "0",
        "totalAmount": 299.00,
        "remarks": "备注信息",
        "createTime": "2024-01-01 10:00:00",
        "updateTime": "2024-01-01 10:00:00"
      }
    ]
  }
}
```

### 1.2 获取订单详情

**接口地址**: `GET /dfgx/customer-orders/{orderId}`

**权限要求**: `dfgx:customerOrders:query`

**路径参数**:
- `orderId`: 订单ID

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "id": 1,
    "orderNo": "ORD202401010001",
    "customerName": "张三",
    "customerPhone": "13800138000",
    "customerEmail": "zhangsan@example.com",
    "orderDate": "2024-01-01 10:00:00",
    "status": "0",
    "totalAmount": 299.00,
    "remarks": "备注信息"
  }
}
```

### 1.3 根据订单编号获取订单

**接口地址**: `GET /dfgx/customer-orders/by-order-no/{orderNo}`

**权限要求**: `dfgx:customerOrders:query`

**路径参数**:
- `orderNo`: 订单编号

### 1.4 新增客户订单

**接口地址**: `POST /dfgx/customer-orders`

**权限要求**: `dfgx:customerOrders:add`

**请求体**:
```json
{
  "orderNo": "ORD202401010001",
  "customerName": "张三",
  "customerPhone": "13800138000",
  "customerEmail": "zhangsan@example.com",
  "orderDate": "2024-01-01 10:00:00",
  "status": "0",
  "totalAmount": 299.00,
  "remarks": "备注信息"
}
```

### 1.5 更新客户订单

**接口地址**: `PUT /dfgx/customer-orders`

**权限要求**: `dfgx:customerOrders:edit`

**请求体**:
```json
{
  "id": 1,
  "orderNo": "ORD202401010001",
  "customerName": "张三",
  "customerPhone": "13800138000",
  "customerEmail": "zhangsan@example.com",
  "orderDate": "2024-01-01 10:00:00",
  "status": "1",
  "totalAmount": 299.00,
  "remarks": "更新后的备注信息"
}
```

### 1.6 删除客户订单

**接口地址**: `DELETE /dfgx/customer-orders/{orderId}`

**权限要求**: `dfgx:customerOrders:remove`

**路径参数**:
- `orderId`: 订单ID

### 1.7 批量删除客户订单

**接口地址**: `DELETE /dfgx/customer-orders/batch`

**权限要求**: `dfgx:customerOrders:remove`

**请求体**:
```json
[1, 2, 3, 4, 5]
```

### 1.8 更新订单状态

**接口地址**: `PUT /dfgx/customer-orders/{orderId}/status`

**权限要求**: `dfgx:customerOrders:edit`

**查询参数**:
- `status`: 新状态
- `operator`: 操作人（可选）

### 1.9 校验订单编号唯一性

**接口地址**: `GET /dfgx/customer-orders/check-order-no`

**权限要求**: `dfgx:customerOrders:query`

**查询参数**:
- `orderNo`: 订单编号
- `orderId`: 订单ID（更新时排除自己，可选）

### 1.10 导出客户订单

**接口地址**: `GET /dfgx/customer-orders/export`

**权限要求**: `dfgx:customerOrders:export`

**查询参数**: 同分页查询参数

---

## 2. 订单详情管理 (Order Details)

### 2.1 分页查询订单详情列表

**接口地址**: `GET /dfgx/order-details/list`

**权限要求**: `dfgx:orderDetails:list`

**请求参数**:

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| orderId | long | 否 | 订单ID |
| lensType | int | 否 | 镜片类型 |
| lensTypeName | string | 否 | 镜片类型名称 |
| coating | int | 否 | 膜层 |
| coatingName | string | 否 | 膜层名称 |
| material | int | 否 | 材质 |
| materialName | string | 否 | 材质名称 |
| frameType | string | 否 | 镜框类型 |
| minQuantity | int | 否 | 最小数量 |
| maxQuantity | int | 否 | 最大数量 |
| minUnitPrice | decimal | 否 | 最小单价 |
| maxUnitPrice | decimal | 否 | 最大单价 |

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "total": 50,
    "result": [
      {
        "id": 1,
        "orderId": 1,
        "lensType": 1,
        "lensTypeName": "单焦点",
        "coating": 1,
        "coatingName": "防蓝光",
        "designName": 1,
        "designNameText": "标准设计",
        "material": 1,
        "materialName": "树脂",
        "refractiveIndex": 1,
        "refractiveIndexValue": "1.56",
        "frameType": "full",
        "quantity": 1,
        "unitPrice": 299.00,
        "totalPrice": 299.00,
        "leftEyeSph": "-2.00",
        "leftEyeCyl": "0.00",
        "leftEyeAxis": "0",
        "leftEyeAdd": "0.00",
        "leftEyePrism": "0.00",
        "rightEyeSph": "-2.00",
        "rightEyeCyl": "0.00",
        "rightEyeAxis": "0",
        "rightEyeAdd": "0.00",
        "rightEyePrism": "0.00",
        "autoRemarks": "自动备注"
      }
    ]
  }
}
```

### 2.2 获取订单详情

**接口地址**: `GET /dfgx/order-details/{detailId}`

**权限要求**: `dfgx:orderDetails:query`

### 2.3 根据订单ID获取详情列表

**接口地址**: `GET /dfgx/order-details/by-order/{orderId}`

**权限要求**: `dfgx:orderDetails:query`

### 2.4 新增订单详情

**接口地址**: `POST /dfgx/order-details`

**权限要求**: `dfgx:orderDetails:add`

**请求体**:
```json
{
  "orderId": 1,
  "lensType": 1,
  "lensTypeName": "单焦点",
  "coating": 1,
  "coatingName": "防蓝光",
  "designName": 1,
  "designNameText": "标准设计",
  "material": 1,
  "materialName": "树脂",
  "refractiveIndex": 1,
  "refractiveIndexValue": "1.56",
  "frameType": "full",
  "quantity": 1,
  "unitPrice": 299.00,
  "leftEyeSph": "-2.00",
  "leftEyeCyl": "0.00",
  "leftEyeAxis": "0",
  "leftEyeAdd": "0.00",
  "leftEyePrism": "0.00",
  "rightEyeSph": "-2.00",
  "rightEyeCyl": "0.00",
  "rightEyeAxis": "0",
  "rightEyeAdd": "0.00",
  "rightEyePrism": "0.00",
  "autoRemarks": "自动备注"
}
```

### 2.5 批量新增订单详情

**接口地址**: `POST /dfgx/order-details/batch`

**权限要求**: `dfgx:orderDetails:add`

**请求体**:
```json
[
  {
    "orderId": 1,
    "lensType": 1,
    "lensTypeName": "单焦点",
    "quantity": 1,
    "unitPrice": 299.00
  },
  {
    "orderId": 1,
    "lensType": 2,
    "lensTypeName": "渐进",
    "quantity": 1,
    "unitPrice": 399.00
  }
]
```

### 2.6 更新订单详情

**接口地址**: `PUT /dfgx/order-details`

**权限要求**: `dfgx:orderDetails:edit`

### 2.7 删除订单详情

**接口地址**: `DELETE /dfgx/order-details/{detailId}`

**权限要求**: `dfgx:orderDetails:remove`

### 2.8 批量删除订单详情

**接口地址**: `DELETE /dfgx/order-details/batch`

**权限要求**: `dfgx:orderDetails:remove`

### 2.9 根据订单ID删除所有详情

**接口地址**: `DELETE /dfgx/order-details/by-order/{orderId}`

**权限要求**: `dfgx:orderDetails:remove`

### 2.10 计算订单详情总金额

**接口地址**: `GET /dfgx/order-details/calculate-total/{orderId}`

**权限要求**: `dfgx:orderDetails:query`

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "totalAmount": 598.00
  }
}
```

### 2.11 导出订单详情

**接口地址**: `GET /dfgx/order-details/export`

**权限要求**: `dfgx:orderDetails:export`

---

## 3. 订单状态历史管理 (Order Status History)

### 3.1 分页查询订单状态历史列表

**接口地址**: `GET /dfgx/order-status-history/list`

**权限要求**: `dfgx:orderStatusHistory:list`

**请求参数**:

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| orderId | long | 否 | 订单ID |
| status | string | 否 | 状态 |
| statusName | string | 否 | 状态名称 |
| operator | string | 否 | 操作人 |
| beginTime | datetime | 否 | 开始时间 |
| endTime | datetime | 否 | 结束时间 |

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "total": 20,
    "result": [
      {
        "id": 1,
        "orderId": 1,
        "status": "0",
        "statusName": "待处理",
        "operator": "张三",
        "createTime": "2024-01-01 10:00:00"
      }
    ]
  }
}
```

### 3.2 获取状态历史详情

**接口地址**: `GET /dfgx/order-status-history/{historyId}`

**权限要求**: `dfgx:orderStatusHistory:query`

### 3.3 根据订单ID获取状态历史列表

**接口地址**: `GET /dfgx/order-status-history/by-order/{orderId}`

**权限要求**: `dfgx:orderStatusHistory:query`

### 3.4 获取订单最新状态历史

**接口地址**: `GET /dfgx/order-status-history/latest/{orderId}`

**权限要求**: `dfgx:orderStatusHistory:query`

### 3.5 新增订单状态历史

**接口地址**: `POST /dfgx/order-status-history`

**权限要求**: `dfgx:orderStatusHistory:add`

**请求体**:
```json
{
  "orderId": 1,
  "status": "1",
  "statusName": "生产中",
  "operator": "张三"
}
```

### 3.6 批量新增订单状态历史

**接口地址**: `POST /dfgx/order-status-history/batch`

**权限要求**: `dfgx:orderStatusHistory:add`

### 3.7 更新订单状态历史

**接口地址**: `PUT /dfgx/order-status-history`

**权限要求**: `dfgx:orderStatusHistory:edit`

### 3.8 删除订单状态历史

**接口地址**: `DELETE /dfgx/order-status-history/{historyId}`

**权限要求**: `dfgx:orderStatusHistory:remove`

### 3.9 批量删除订单状态历史

**接口地址**: `DELETE /dfgx/order-status-history/batch`

**权限要求**: `dfgx:orderStatusHistory:remove`

### 3.10 根据订单ID删除所有状态历史

**接口地址**: `DELETE /dfgx/order-status-history/by-order/{orderId}`

**权限要求**: `dfgx:orderStatusHistory:remove`

### 3.11 记录订单状态变更

**接口地址**: `POST /dfgx/order-status-history/record-change`

**权限要求**: `dfgx:orderStatusHistory:add`

**查询参数**:
- `orderId`: 订单ID
- `status`: 新状态
- `statusName`: 状态名称
- `operator`: 操作人（可选）

### 3.12 导出订单状态历史

**接口地址**: `GET /dfgx/order-status-history/export`

**权限要求**: `dfgx:orderStatusHistory:export`

---

## 4. 订单综合管理 (Order Management)

### 4.1 获取订单完整信息

**接口地址**: `GET /dfgx/order-management/complete/{orderId}`

**权限要求**: `dfgx:orderManagement:query`

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "order": {
      "id": 1,
      "orderNo": "ORD202401010001",
      "customerName": "张三",
      "customerPhone": "13800138000",
      "customerEmail": "zhangsan@example.com",
      "orderDate": "2024-01-01 10:00:00",
      "status": "0",
      "totalAmount": 299.00,
      "remarks": "备注信息"
    },
    "orderDetails": [
      {
        "id": 1,
        "orderId": 1,
        "lensType": 1,
        "lensTypeName": "单焦点",
        "quantity": 1,
        "unitPrice": 299.00,
        "totalPrice": 299.00
      }
    ],
    "statusHistory": [
      {
        "id": 1,
        "orderId": 1,
        "status": "0",
        "statusName": "待处理",
        "operator": "系统",
        "createTime": "2024-01-01 10:00:00"
      }
    ],
    "calculatedTotalAmount": 299.00
  }
}
```

### 4.2 创建完整订单

**接口地址**: `POST /dfgx/order-management/create-complete`

**权限要求**: `dfgx:orderManagement:add`

**请求体**:
```json
{
  "order": {
    "orderNo": "ORD202401010001",
    "customerName": "张三",
    "customerPhone": "13800138000",
    "customerEmail": "zhangsan@example.com",
    "orderDate": "2024-01-01 10:00:00",
    "status": "0",
    "totalAmount": 0,
    "remarks": "备注信息"
  },
  "orderDetails": [
    {
      "lensType": 1,
      "lensTypeName": "单焦点",
      "coating": 1,
      "coatingName": "防蓝光",
      "quantity": 1,
      "unitPrice": 299.00
    }
  ]
}
```

### 4.3 更新订单状态并记录历史

**接口地址**: `PUT /dfgx/order-management/update-status/{orderId}`

**权限要求**: `dfgx:orderManagement:edit`

**查询参数**:
- `status`: 新状态
- `operator`: 操作人（可选）

### 4.4 删除完整订单

**接口地址**: `DELETE /dfgx/order-management/complete/{orderId}`

**权限要求**: `dfgx:orderManagement:remove`

### 4.5 获取订单统计信息

**接口地址**: `GET /dfgx/order-management/statistics`

**权限要求**: `dfgx:orderManagement:query`

**查询参数**:
- `beginTime`: 开始时间（可选）
- `endTime`: 结束时间（可选）

**响应示例**:
```json
{
  "code": 200,
  "msg": "操作成功",
  "data": {
    "totalOrders": 100,
    "totalAmount": 29900.00,
    "statusCounts": [
      { "status": "0", "count": 30 },
      { "status": "1", "count": 40 },
      { "status": "2", "count": 25 },
      { "status": "3", "count": 5 }
    ],
    "dateRange": {
      "beginTime": "2024-01-01 00:00:00",
      "endTime": "2024-01-31 23:59:59"
    }
  }
}
```

---

## 5. 数据字典

### 5.1 订单状态

| 状态码 | 状态名称 | 说明 |
|--------|----------|------|
| 0 | 待处理 | 订单刚创建，等待处理 |
| 1 | 生产中 | 订单正在生产制作 |
| 2 | 已完成 | 订单已完成制作 |
| 3 | 已取消 | 订单已取消 |

### 5.2 镜框类型

| 类型码 | 类型名称 | 说明 |
|--------|----------|------|
| full | 全框 | 完全包围镜片的镜框 |
| half | 半框 | 只包围镜片上半部分的镜框 |
| none | 无框 | 没有镜框，直接固定在镜片上 |

### 5.3 响应状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 操作成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |
| 403 | 权限不足 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

---

## 6. 错误处理

### 6.1 错误响应格式

```json
{
  "code": 400,
  "msg": "请求参数错误",
  "data": null
}
```

### 6.2 常见错误信息

- `"订单不存在"` - 指定的订单ID不存在
- `"订单编号已存在"` - 订单编号重复
- `"请求参数不能为空"` - 必填参数缺失
- `"状态不能为空"` - 状态参数缺失
- `"权限不足"` - 当前用户没有操作权限

---

## 7. 使用示例

### 7.1 创建订单流程

```javascript
// 1. 创建订单
const orderData = {
  orderNo: "ORD202401010001",
  customerName: "张三",
  customerPhone: "13800138000",
  customerEmail: "zhangsan@example.com",
  orderDate: "2024-01-01 10:00:00",
  status: "0",
  totalAmount: 0,
  remarks: "备注信息"
};

const orderResponse = await fetch('/api/dfgx/customer-orders', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer your-token'
  },
  body: JSON.stringify(orderData)
});

const order = await orderResponse.json();

// 2. 添加订单详情
const orderDetails = [
  {
    orderId: order.data.id,
    lensType: 1,
    lensTypeName: "单焦点",
    coating: 1,
    coatingName: "防蓝光",
    quantity: 1,
    unitPrice: 299.00
  }
];

await fetch('/api/dfgx/order-details/batch', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer your-token'
  },
  body: JSON.stringify(orderDetails)
});

// 3. 记录状态变更
await fetch(`/api/dfgx/order-status-history/record-change?orderId=${order.data.id}&status=0&statusName=待处理&operator=张三`, {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer your-token'
  }
});
```

### 7.2 更新订单状态

```javascript
// 更新订单状态
await fetch(`/api/dfgx/order-management/update-status/1?status=1&operator=李四`, {
  method: 'PUT',
  headers: {
    'Authorization': 'Bearer your-token'
  }
});
```

### 7.3 查询订单列表

```javascript
// 查询订单列表
const response = await fetch('/api/dfgx/customer-orders/list?pageNum=1&pageSize=10&status=0', {
  headers: {
    'Authorization': 'Bearer your-token'
  }
});

const result = await response.json();
console.log(result.data.result); // 订单列表
```

---

## 8. 注意事项

1. **认证**: 所有接口都需要在请求头中携带有效的Bearer Token
2. **权限**: 不同接口需要不同的权限，请确保用户具有相应权限
3. **分页**: 列表查询接口都支持分页，建议合理设置pageSize避免数据量过大
4. **时间格式**: 所有时间字段都使用 `yyyy-MM-dd HH:mm:ss` 格式
5. **删除操作**: 所有删除操作都是逻辑删除，不会真正删除数据
6. **状态管理**: 订单状态变更会自动记录历史，建议使用综合管理接口
7. **数据校验**: 前端应进行必要的数据校验，如订单编号唯一性等
8. **错误处理**: 请妥善处理各种错误情况，给用户友好的提示

---

## 9. 更新日志

- **v1.0.0** (2024-01-01): 初始版本，包含基础CRUD功能
- 支持客户订单管理
- 支持订单详情管理  
- 支持订单状态历史管理
- 支持订单综合管理
- 支持数据导出功能
- 支持权限控制
- 支持操作日志记录
