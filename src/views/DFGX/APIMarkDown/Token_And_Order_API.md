# Authentication and Order Creation API Documentation

## Overview

This document contains two core APIs:
1. **Get User Token** - For JWT authentication
2. **Create Complete Order** - For generating order data

**Base URL**: `http://170.106.146.87/api`

**Authentication Method**: Bearer Token (use after obtaining Token)

---

## 1. Get User Token

### API Information

**Endpoint**: `POST /GetToken`

**Authorization Required**: No (anonymous access)

**Description**: Get JWT Token by username and password for subsequent API authentication

### Request Parameters

**Request Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
  "username": "string",
  "password": "string",
  "tenantId": "string (optional)"
}
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| username | string | Yes | Username |
| password | string | Yes | Password |
| tenantId | string | No | Tenant ID (optional) |

### Response Examples

**Success Response** (200):
```json
{
  "code": 200,
  "msg": "Operation successful",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
}
```

**Failure Response** (400):
```json
{
  "code": 400,
  "msg": "Invalid request parameters",
  "data": null
}
```

### Password Encryption Method

**Important**: Password must be **MD5 encrypted** before transmission. Encryption method:

- **Encryption Library**: `crypto-js/md5`
- **Encryption Method**: Convert password to MD5 hash string
- **Frontend Implementation**:
  ```javascript
  import md5 from 'crypto-js/md5'
  const encryptedPassword = md5(password).toString()
  ```

**Note**: If password is transmitted without encryption, the API will return authentication failure. Backend does not accept plaintext passwords.

### Notes

1. **Password Encryption**: **Must** encrypt password with MD5 on the frontend before transmission, refer to encryption method above
2. **Token Storage**: Token should be stored securely after obtaining, and included in request headers for subsequent APIs
3. **Token Validity**: Token has an expiration time, need to re-obtain after expiration
4. **Account Lockout**: Consecutive login failures will cause account lockout, need to wait for unlock
5. **IP Recording**: System automatically records login IP and geographic location information

### Request Examples

#### JavaScript (fetch + MD5 Encryption)
```javascript
import md5 from 'crypto-js/md5'

// Encrypt password
const username = 'admin'
const plainPassword = '123456'
const encryptedPassword = md5(plainPassword).toString()

const response = await fetch('/api/GetToken', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: username,
    password: encryptedPassword  // Use encrypted password
  })
});

const result = await response.json();
if (result.code === 200) {
  const token = result.data;
  // Save token
  localStorage.setItem('token', token);
}
```

#### Node.js (using crypto-js)
```javascript
const crypto = require('crypto-js');

const username = 'admin';
const password = '123456';
const encryptedPassword = crypto.MD5(password).toString();

const response = await fetch('/api/GetToken', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: username,
    password: encryptedPassword
  })
});
```

#### cURL
```bash
curl -X POST http://170.106.146.87/api/GetToken \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "123456"
  }'
```

---

## 2. Create Complete Order

### API Information

**Endpoint**: `POST /dfgx/order-management/create-complete`

**Authorization Required**: `dfgx:orderManagement:add` + Bearer Token authentication

**Description**: Create a complete order including order basic information and order details. System automatically calculates total amount and records order status history

### Request Parameters

**Request Headers**:
```
Content-Type: application/json
Authorization: Bearer {token}
```

**Request Body**:
```json
{
  "Order": {
    "orderNo": "ORD202401010001",
    "customerName": "Zhang San",
    "customerPhone": "13800138000",
    "customerEmail": "zhangsan@example.com",
    "Order_date": "2024-01-01 10:00:00",
    "status": "0",
    "totalAmount": 598.00,
    "remarks": "Remarks",
    "frameType": "full",
    "frameModel": "Model-001",
    "edEda": "65/20",
    "sizeA": "52",
    "sizeB": "18",
    "dia": "70",
    "refractiveIndex": 1,
    "refractiveIndexValue": "1.56",
    "designName": 1,
    "designNameText": "Standard Design",
    "material": "1,2",
    "materialName": "Resin, PC",
    "channel": 1,
    "channelName": "Standard Channel",
    "coating": 1,
    "coatingName": "Blue Light Protection",
    "tinting": 1,
    "tintingName": "No Tinting",
    "color": 1,
    "colorName": "Transparent",
    "tintPercentageTop": 0,
    "tintPercentageBottom": 0,
    "lenticular": 1,
    "lenticularName": "None",
    "orderStatus": "0",
    "orderStatusName": "Pending",
    "colorAttachment": "",
    "remarksAttachment": "",
    "rightEyeQuantity": 1,
    "leftEyeQuantity": 1,
    "rightEyeSph": "-2.00",
    "rightEyeCyl": "0.00",
    "rightEyeAxis": "0",
    "rightEyeAdd": "0.00",
    "rightEyePrism": "0.00",
    "rightEyePrismDirection": "",
    "rightEyeBase": "",
    "rightEyePh": "",
    "rightEyeNearPd": "32",
    "rightEyeDistancePd": "32",
    "leftEyeSph": "-2.00",
    "leftEyeCyl": "0.00",
    "leftEyeAxis": "0",
    "leftEyeAdd": "0.00",
    "leftEyePrism": "0.00",
    "leftEyePrismDirection": "",
    "leftEyeBase": "",
    "leftEyePh": "",
    "leftEyeNearPd": "32",
    "leftEyeDistancePd": "32",
    "remarksType": "Special Requirements, Rush Processing",
    "autoRemarks": "Auto remarks"
  },
  "OrderDetails": [
    {
      "dia": "70",
      "refractiveIndex": 1,
      "refractiveIndexValue": "1.56",
      "designName": 1,
      "designNameText": "Standard Design",
      "material": "1,2",
      "materialName": "Resin, PC",
      "channel": 1,
      "channelName": "Standard Channel",
      "coating": 1,
      "coatingName": "Blue Light Protection",
      "tinting": 1,
      "tintingName": "No Tinting",
      "color": 1,
      "colorName": "Transparent",
      "tintPercentageTop": 0,
      "tintPercentageBottom": 0,
      "lenticular": 1,
      "lenticularName": "None",
      "orderStatus": "0",
      "orderStatusName": "Pending",
      "frameType": "full",
      "frameModel": "Model-001",
      "edEda": "65/20",
      "sizeA": "52",
      "sizeB": "18",
      "leftEyeQuantity": 1,
      "rightEyeQuantity": 1,
      "quantity": 2,
      "unitPrice": 299.00,
      "totalPrice": 598.00,
      "colorAttachment": "",
      "remarksAttachment": "",
      "rightEyeSph": "-2.00",
      "rightEyeCyl": "0.00",
      "rightEyeAxis": "0",
      "rightEyeAdd": "0.00",
      "rightEyePrism": "0.00",
      "rightEyePrismDirection": "",
      "rightEyeBase": "",
      "rightEyePh": "",
      "rightEyeNearPd": "32",
      "rightEyeDistancePd": "32",
      "leftEyeSph": "-2.00",
      "leftEyeCyl": "0.00",
      "leftEyeAxis": "0",
      "leftEyeAdd": "0.00",
      "leftEyePrism": "0.00",
      "leftEyePrismDirection": "",
      "leftEyeBase": "",
      "leftEyePh": "",
      "leftEyeNearPd": "32",
      "leftEyeDistancePd": "32",
      "remarksType": "Special Requirements, Rush Processing",
      "autoRemarks": "Auto remarks"
    }
  ]
}
```

**Important Notes**: 
- Top-level field names in request body must be `Order` and `OrderDetails` (capitalized), not `order` and `orderDetails`
- `Order_date` field name contains underscore, format is `yyyy-MM-dd HH:mm:ss`
- If `material` field is an array, it needs to be converted to comma-separated string (e.g., "1,2")

### Request Body Field Descriptions

#### Order Object Fields (Capitalized)

**Basic Information Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| orderNo | string | Yes | Order number (unique) |
| customerName | string | Yes | Customer name |
| customerPhone | string | No | Customer phone |
| customerEmail | string | No | Customer email |
| Order_date | datetime | No | Order date, format: `yyyy-MM-dd HH:mm:ss` (note: field name contains underscore) |
| status | string | No | Order status (default "0" pending) |
| totalAmount | decimal | No | Order total amount |
| remarks | string | No | Order remarks |

**Frame Information Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| frameType | string | No | Frame type (full/half/none) |
| frameModel | string | No | Frame model |
| edEda | string | No | Frame size (ED/DA) |
| sizeA | string | No | Frame size A |
| sizeB | string | No | Frame size B |

**Lens Information Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| dia | string | No | Diameter |
| refractiveIndex | int | No | Refractive index type |
| refractiveIndexValue | string | No | Refractive index value |
| designName | int | No | Design type |
| designNameText | string | No | Design type text |
| material | string | No | Material (multiple values separated by commas, e.g., "1,2") |
| materialName | string | No | Material name (multiple values separated by commas) |
| channel | int | No | Channel type |
| channelName | string | No | Channel name |
| coating | int | No | Coating type |
| coatingName | string | No | Coating name |
| tinting | int | No | Tinting type |
| tintingName | string | No | Tinting name |
| color | int | No | Color type |
| colorName | string | No | Color name |
| tintPercentageTop | int | No | Top tint percentage |
| tintPercentageBottom | int | No | Bottom tint percentage |
| lenticular | int | No | Lenticular type |
| lenticularName | string | No | Lenticular name |
| orderStatus | string | No | Order status |
| orderStatusName | string | No | Order status name |
| colorAttachment | string | No | Color attachment |
| remarksAttachment | string | No | Remarks attachment |

**Quantity Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| rightEyeQuantity | int | No | Right eye quantity |
| leftEyeQuantity | int | No | Left eye quantity |

**Right Eye Parameter Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| rightEyeSph | string | No | Right eye sphere power |
| rightEyeCyl | string | No | Right eye cylinder power |
| rightEyeAxis | string | No | Right eye axis |
| rightEyeAdd | string | No | Right eye addition power |
| rightEyePrism | string | No | Right eye prism power |
| rightEyePrismDirection | string | No | Right eye prism direction |
| rightEyeBase | string | No | Right eye base |
| rightEyePh | string | No | Right eye pupillary height |
| rightEyeNearPd | string | No | Right eye near PD |
| rightEyeDistancePd | string | No | Right eye distance PD |

**Left Eye Parameter Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| leftEyeSph | string | No | Left eye sphere power |
| leftEyeCyl | string | No | Left eye cylinder power |
| leftEyeAxis | string | No | Left eye axis |
| leftEyeAdd | string | No | Left eye addition power |
| leftEyePrism | string | No | Left eye prism power |
| leftEyePrismDirection | string | No | Left eye prism direction |
| leftEyeBase | string | No | Left eye base |
| leftEyePh | string | No | Left eye pupillary height |
| leftEyeNearPd | string | No | Left eye near PD |
| leftEyeDistancePd | string | No | Left eye distance PD |

**Remarks Fields**:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| remarksType | string | No | Remarks type (multiple values separated by commas) |
| autoRemarks | string | No | Auto remarks |

#### OrderDetails Array Fields (Capitalized)

Fields in the order details array are basically the same as the `Order` object, but include the following additional fields:

| Field Name | Type | Required | Description |
|------------|------|----------|-------------|
| quantity | int | Yes | Total quantity (left eye + right eye) |
| unitPrice | decimal | Yes | Unit price |
| totalPrice | decimal | No | Total price (quantity × unit price) |

**Note**: Apart from the above three fields, each element in the `OrderDetails` array can include all fields from the `Order` object (such as lens information, frame information, left and right eye parameters, etc.). Please refer to the `Order` object field descriptions above.

### Response Examples

**Success Response** (200):
```json
{
  "code": 200,
  "msg": "Operation successful",
  "data": {
    "orderId": 123,
    "orderNo": "ORD202401010001",
    "totalAmount": 598.00
  }
}
```

**Failure Response** (400):
```json
{
  "code": 400,
  "msg": "Order number already exists",
  "data": null
}
```

**Unauthorized Response** (401):
```json
{
  "code": 401,
  "msg": "Invalid session, or session has expired, please login again.",
  "data": null
}
```

**Insufficient Permissions Response** (403):
```json
{
  "code": 403,
  "msg": "Insufficient permissions",
  "data": null
}
```

### Order Status Descriptions

| Status Code | Status Name | Description |
|-------------|-------------|-------------|
| 0 | Pending | Order just created, awaiting processing |
| 1 | In Production | Order being produced |
| 2 | Completed | Order production completed |
| 3 | Cancelled | Order cancelled |

### Notes

1. **Order Number Uniqueness**: Order number must be unique, system automatically validates
2. **Total Amount Calculation**: If `totalAmount` is 0, system will automatically calculate total amount based on order details
3. **Status History**: Order creation automatically records initial status history
4. **Required Fields**: `orderNo` and `customerName` are required fields
5. **Token Authentication**: Must include valid Bearer Token in request headers

### Request Examples

#### JavaScript (fetch + MD5 Encryption)
```javascript
import md5 from 'crypto-js/md5'

// 1. Encrypt password with MD5 and get Token
const username = 'admin'
const plainPassword = '123456'
const encryptedPassword = md5(plainPassword).toString()

const tokenResponse = await fetch('/api/GetToken', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    username: username,
    password: encryptedPassword  // Use encrypted password
  })
});

const tokenResult = await tokenResponse.json();
if (tokenResult.code !== 200) {
  throw new Error('Failed to get Token: ' + tokenResult.msg);
}
const token = tokenResult.data;

// 2. Use Token to create order
// Note: Use Order and OrderDetails (capitalized), Order_date field name contains underscore
const orderData = {
  Order: {
    orderNo: 'ORD202401010001',
    customerName: 'Zhang San',
    customerPhone: '13800138000',
    customerEmail: 'zhangsan@example.com',
    Order_date: '2024-01-01 10:00:00',  // Note field name
    status: '0',
    totalAmount: 598.00,
    remarks: 'Remarks',
    frameType: 'full',
    rightEyeQuantity: 1,
    leftEyeQuantity: 1,
    rightEyeSph: '-2.00',
    leftEyeSph: '-2.00'
    // ... other order fields
  },
  OrderDetails: [  // Note capitalization
    {
      quantity: 2,
      unitPrice: 299.00,
      totalPrice: 598.00,
      rightEyeQuantity: 1,
      leftEyeQuantity: 1,
      rightEyeSph: '-2.00',
      leftEyeSph: '-2.00'
      // ... other detail fields
    }
  ]
};

const orderResponse = await fetch('/api/dfgx/order-management/create-complete', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify(orderData)
});

const orderResult = await orderResponse.json();
if (orderResult.code === 200) {
  console.log('Order created successfully, Order ID:', orderResult.data.orderId || orderResult.data.OrderId);
} else {
  console.error('Order creation failed:', orderResult.msg);
}
```

#### cURL (Need to MD5 encrypt password first)

**Note**: In cURL example, password needs to be MD5 encrypted first. The following example uses `md5sum` command (Linux/Mac):

```bash
# 1. MD5 encrypt password (Linux/Mac)
PASSWORD_MD5=$(echo -n "123456" | md5sum | cut -d' ' -f1)

# 2. Get Token
TOKEN=$(curl -X POST http://170.106.146.87/api/GetToken \
  -H "Content-Type: application/json" \
  -d "{
    \"username\": \"admin\",
    \"password\": \"$PASSWORD_MD5\"
  }" | jq -r '.data')

# 3. Create order (note: use Order and OrderDetails, capitalized)
curl -X POST http://170.106.146.87/api/dfgx/order-management/create-complete \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "Order": {
      "orderNo": "ORD202401010001",
      "customerName": "Zhang San",
      "customerPhone": "13800138000",
      "Order_date": "2024-01-01 10:00:00",
      "status": "0",
      "totalAmount": 598.00,
      "frameType": "full",
      "rightEyeQuantity": 1,
      "leftEyeQuantity": 1
    },
    "OrderDetails": [
      {
        "quantity": 2,
        "unitPrice": 299.00,
        "totalPrice": 598.00
      }
    ]
  }'
```

---

## 3. Complete Call Flow

### Typical Use Case

```javascript
// Complete order creation flow (including MD5 encryption)
import md5 from 'crypto-js/md5'

async function createOrderFlow(orderInfo) {
  try {
    // Step 1: MD5 encrypt password and get Token
    const encryptedPassword = md5('123456').toString() // Use actual password
    
    const tokenResponse = await fetch('/api/GetToken', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: 'admin',
        password: encryptedPassword  // Use encrypted password
      })
    });
    
    const tokenData = await tokenResponse.json();
    if (tokenData.code !== 200) {
      throw new Error('Failed to get Token: ' + tokenData.msg);
    }
    
    const token = tokenData.data;
    
    // Step 2: Use Token to create order
    // Note: Use Order and OrderDetails (capitalized)
    const orderDate = new Date().toISOString().slice(0, 19).replace('T', ' ')
    
    const orderResponse = await fetch('/api/dfgx/order-management/create-complete', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({
        Order: {
          orderNo: orderInfo.orderNo,
          customerName: orderInfo.customerName,
          customerPhone: orderInfo.customerPhone,
          Order_date: orderDate,  // Note field name contains underscore
          status: '0',
          totalAmount: orderInfo.totalAmount || 0,
          ...orderInfo.orderFields  // Other order fields
        },
        OrderDetails: orderInfo.orderDetails  // Note capitalization
      })
    });
    
    const orderData = await orderResponse.json();
    if (orderData.code !== 200) {
      throw new Error('Order creation failed: ' + orderData.msg);
    }
    
    return {
      success: true,
      orderId: orderData.data.orderId || orderData.data.OrderId,
      orderNo: orderData.data.orderNo
    };
    
  } catch (error) {
    console.error('Order creation flow failed:', error);
    return {
      success: false,
      error: error.message
    };
  }
}

// Usage example
const orderInfo = {
  orderNo: 'ORD202401010001',
  customerName: 'Zhang San',
  customerPhone: '13800138000',
  totalAmount: 598.00,
  orderFields: {
    frameType: 'full',
    rightEyeQuantity: 1,
    leftEyeQuantity: 1,
    rightEyeSph: '-2.00',
    leftEyeSph: '-2.00'
    // ... other order fields
  },
  orderDetails: [
    {
      quantity: 2,
      unitPrice: 299.00,
      totalPrice: 598.00,
      rightEyeQuantity: 1,
      leftEyeQuantity: 1
      // ... other detail fields
    }
  ]
};

createOrderFlow(orderInfo).then(result => {
  if (result.success) {
    console.log('Order created successfully, Order ID:', result.orderId);
  } else {
    console.error('Order creation failed:', result.error);
  }
});
```

---

## 4. Data Enumeration Definitions

This section contains enumeration value definitions for all option fields in the order creation API. These enumeration values correspond to data from the `merged_lens_table_filtered.csv` file.

### 4.1 Lens Type

**Field Name**: `lensType` (Note: This field may not be used in Order object, only in OrderDetails)

| Value | Label | Description |
|-------|-------|-------------|
| 1 | SINGLE VISION | Single vision lens |
| 2 | BIFOCAL | Bifocal lens |
| 3 | PROGRESSIVE | Progressive lens |

### 4.2 Coating

**Field Name**: `coating`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | UC | Uncoated |
| 2 | HC | Hard coating |
| 3 | HMC | Hard multilayer coating |
| 4 | BACK HMC | Back hard multilayer coating |
| 5 | SHMC | Super hard multilayer coating |
| 6 | BACK SHMC | Back super hard multilayer coating |
| 7 | A1 | A1 coating |
| 8 | A3 | A3 coating |
| 9 | A4 | A4 coating |
| 10 | X6 | X6 coating |
| 11 | X9 | X9 coating |

### 4.3 Material

**Field Name**: `material` (supports multiple selection, multiple values separated by commas, e.g., "1,2")

| Value | Label | Description |
|-------|-------|-------------|
| 1 | HARD RESIN | Hard resin |
| 2 | Glass / BLUE CUT | Glass / Blue light protection |
| 3 | PHOTOGREY | Photochromic grey |
| 5 | PHOTOBROWN | Photochromic brown |
| 6 | PHOTOBLUE | Photochromic blue |
| 26 | PHOTOGREEN / TRANSITION 8 BROWN / TRANSITION XTR BROWN / SILVER MIRROR COATING(only photo and tint) | Photochromic green / Transition 8 brown / Transition XTR brown / Silver mirror coating |
| 27 | PHOTOGREEN | Photochromic green |
| 28 | PHOTOPURPLE | Photochromic purple |
| 18 | PHOTOPINK | Photochromic pink |
| 29 | POLAR-GREY | Polarized grey |
| 30 | POLAR-BROWN | Polarized brown |
| 31 | POLAR-GREEN | Polarized green |
| 32 | TRANSITION 8 GREY | Transition 8 grey |
| 33 | TRANSITION 8 BLUE | Transition 8 blue |
| 34 | TRANSITION 8 RED | Transition 8 red |
| 35 | TRANSITION XTR GREY | Transition XTR grey |
| 36 | TRANSITION XTP GREY | Transition XTP grey |
| 37 | GOLD MIRROR COATING(only photo and tint) | Gold mirror coating |
| 38 | ORANGE MIRROR COATING(only photo and tint) | Orange mirror coating |
| 39 | BLUE MIRROR COATING(only photo and tint) | Blue mirror coating |
| 40 | GREEN MIRROR COATING(only photo and tint) | Green mirror coating |
| 4 | PINK MIRROR COATING(only photo and tint) | Pink mirror coating |
| 41 | WHITE MIRROR COATING(only polar) | White mirror coating (polarized only) |
| 42 | PINK MIRROR COATING(only polar) | Pink mirror coating (polarized only) |
| 43 | ICE BLUE MIRROR COATING(only polar) | Ice blue mirror coating (polarized only) |
| 44 | BLUE MIRROR COATING(only polar) | Blue mirror coating (polarized only) |
| 45 | LENTICULAR | Lenticular |
| 46 | TAC POLAR GREY | TAC polarized grey |
| 47 | TAC POLAR BROWN | TAC polarized brown |
| 48 | NuPOLARIZED GREY | Nu polarized grey |
| 49 | NuPOLARIZED BROWN | Nu polarized brown |
| 50 | NuPOLARIZED GREEN | Nu polarized green |
| 51 | TRANSITION XTP BROWN | Transition XTP brown |
| 52 | Drive Wear | Drive wear lens |
| 53 | TRIVEX | TRIVEX material |
| 54 | SPIN-COATING PHOTOGREY | Spin-coating photochromic grey |
| 55 | SPIN-COATING PHOTOBROWN | Spin-coating photochromic brown |
| 56 | SPIN-COATING PHOTOBLUE | Spin-coating photochromic blue |
| 57 | SPIN-COATING PHOTOPINK | Spin-coating photochromic pink |
| 58 | SPIN-COATING PHOTOGREEN | Spin-coating photochromic green |
| 59 | SPIN-COATING PHOTOPURPLE | Spin-coating photochromic purple |
| 60 | SPIN-COATING PHOTO BLUE CUT | Spin-coating blue light protection |
| 61 | PC lens | PC lens |
| 62 | ARCRYLIC | Acrylic |
| 63 | MR8 | MR8 material |
| 64 | Tribrid | Tribrid material |
| 65 | BLUE CUT(ACR) | Blue light protection (ACR) |
| 66 | BLUE CUT(MR8) | Blue light protection (MR8) |
| 67 | MR8 TRANSITION 8 GREY | MR8 Transition 8 grey |
| 68 | MR8 TRANSITION 8 BROWN | MR8 Transition 8 brown |
| 69 | MR8 TRANSITION 8 ORANGE | MR8 Transition 8 orange |
| 70 | MR8 TRANSITION 8 RED | MR8 Transition 8 red |
| 71 | MR8 TRANSITION 8 BLUE | MR8 Transition 8 blue |
| 72 | MR8 TRANSITION 8 GREEN | MR8 Transition 8 green |
| 73 | MR8 TRANSITION 8 PURPLE | MR8 Transition 8 purple |
| 74 | MR8 TRANSITION XTR GREY | MR8 Transition XTR grey |
| 75 | MR8 TRANSITION XTR BROWN | MR8 Transition XTR brown |
| 76 | MR8 TRANSITION XTP GREY | MR8 Transition XTP grey |
| 77 | MR8 TRANSITION XTP BROWN | MR8 Transition XTP brown |
| 78 | MR7 | MR7 material |
| 79 | TRANSITION S GREY | Transition S grey |
| 80 | TRANSITION S BROWN | Transition S brown |
| 81 | TRANSITION S ORANGE | Transition S orange |
| 82 | TRANSITION S RED | Transition S red |
| 83 | TRANSITION S BLUE | Transition S blue |
| 84 | TRANSITION S GREEN | Transition S green |
| 12 | TRANSITION S PURPLE | Transition S purple |

**Note**: Material field supports multiple selection. Multiple values need to be comma-separated when transmitting (e.g., "1,2,63").

### 4.4 Design Name

**Field Name**: `designName`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | SV / IOT Basic S35 / IOT Basic S40 | Single vision |
| 2 | FT-28 | FT-28 design |
| 3 | Sports Design / FT-35(only 1.6) | Sports design |
| 4 | RT-28(only 1.5) | RT-28 design |
| 5 | INVISIBLE ROUND TOP | Invisible round top |
| 6 | OPTO PROGRESSIVE | OPTO progressive |
| 7 | IOT AH45 PROGRESSIVE | IOT AH45 progressive |
| 8 | IOT BH40 PROGRESSIVE | IOT BH40 progressive |
| 9 | NATURAL ACCURACY | Natural accuracy |
| 10 | PROFESSIONAL ACCURACY | Professional accuracy |
| 11 | ENDLESS STEADY PRGRESSIVE | ENDLESS steady progressive |
| 12 | ENDLESS SV | ENDLESS single vision |
| 13 | ENDLESS OFFICE 1.3M | ENDLESS office 1.3M |
| 14 | ENDLESS OFFICE 2M | ENDLESS office 2M |
| 15 | ENDLESS OFFICE 4M | ENDLESS office 4M |
| 16 | ENDLESS OFFICE 6M | ENDLESS office 6M |
| 17 | ANTI-FATIGUE corridor 9 | Anti-fatigue corridor 9 |
| 18 | ANTI-FATIGUE corridor13 | Anti-fatigue corridor 13 |
| 19 | IOT OFFICE 1.3M | IOT office 1.3M |
| 20 | IOT OFFICE 2M | IOT office 2M |
| 21 | IOT OFFICE 4M | IOT office 4M |
| 22 | IOT OFFICE 6M | IOT office 6M |
| 23 | MYOPIA CONTROL CONCENTRIC CIRCLES | Myopia control concentric circles |
| 24 | MYOPIA CONTROL HONEYCOMB | Myopia control honeycomb |
| 25 | MYOPIA CONTROL SHELL | Myopia control shell |
| 26 | OPTO SV | OPTO single vision |
| 27 | IOT SV | IOT single vision |
| 28 | IOT I-Venure SV | IOT I-Venure single vision |
| 29 | IOT Sporthin SV | IOT Sporthin single vision |
| 30 | IOT SV Toric | IOT single vision toric |
| 31 | RT-40 | RT-40 design |
| 32 | B-Free Bifocal | B-Free bifocal |
| 33 | IOT Alpha mobile | IOT Alpha mobile |
| 34 | IOT Alpha H25 | IOT Alpha H25 |
| 35 | IOT Alpha H65 | IOT Alpha H65 |
| 36 | IOT Alpha S35 | IOT Alpha S35 |
| 37 | IOT Alpha S45 | IOT Alpha S45 |
| 38 | IOT Ultra short | IOT ultra short |
| 39 | IOT Basic H20 | IOT basic H20 |
| 40 | IOT Basic H60 | IOT basic H60 |
| 41 | SCOPUS | SCOPUS design |
| 42 | IOT Office Reader | IOT office reader |
| 43 | IOT Pure Office II | IOT pure office II |
| 44 | IOT Sport Progressive | IOT sport progressive |
| 45 | IOT Sporthin PAL | IOT Sporthin PAL |
| 46 | IOT Drive Progressive | IOT drive progressive |
| 47 | IOT YSIAN | IOT YSIAN |
| 48 | Camber Steady | Camber steady |
| 49 | IOT ENDLESS drive progressive | IOT ENDLESS drive progressive |
| 50 | IOT Endless Steady EasyFit Progressive | IOT Endless Steady EasyFit Progressive |
| 51 | IOT e-Endless Steady | IOT e-Endless Steady |
| 52 | IOT Endless Anti-fatigue | IOT Endless Anti-fatigue |
| 53 | IOT Camber Steady Plus | IOT Camber Steady Plus |
| 54 | IOT Essential Steady | IOT Essential Steady |
| 55 | IOT Endless Sport | IOT Endless Sport |
| 56 | IOT Endless Pilot | IOT Endless Pilot |
| 57 | IOT Endless Bifocal | IOT Endless Bifocal |
| 58 | IOT Endless Anti-fatigue SV | IOT Endless Anti-fatigue SV |
| 59 | IOT Endless Drive SV | IOT Endless Drive SV |
| 60 | OPTO Office 0.8M | OPTO office 0.8M |
| 61 | OPTO Office 1M | OPTO office 1M |
| 62 | OPTO Office 1.3M | OPTO office 1.3M |
| 63 | OPTO Office 2M | OPTO office 2M |
| 64 | OPTO Office 4M | OPTO office 4M |
| 65 | OPTO Office 6M | OPTO office 6M |
| 66 | OPTO 4K | OPTO 4K |
| 67 | OPTO 8K | OPTO 8K |
| 68 | OPTO HD | OPTO HD |
| 69 | OPTO HD2 | OPTO HD2 |
| 70 | OPTO MD | OPTO MD |
| 71 | OPTO MD2 | OPTO MD2 |
| 72 | OPTO SD | OPTO SD |
| 73 | OPTO SD2 | OPTO SD2 |
| 74 | OPTO Extended IXL | OPTO Extended IXL |
| 75 | OPTO Extended IXL 4K | OPTO Extended IXL 4K |
| 76 | OPTO VARIOfocal | OPTO VARIOfocal |
| 77 | OPTO Office 14 | OPTO office 14 |
| 78 | OPTO Mild ADD | OPTO Mild ADD |
| 79 | OPTO VARIO III | OPTO VARIO III |
| 80 | OPTO VARIO III | OPTO VARIO III (duplicate) |
| 81 | OPTO Atorical Optimization | OPTO Atorical Optimization |
| 82 | OPTO Blended Design | OPTO Blended Design |
| 83 | SND SV Accuracy | SND SV Accuracy |
| 84 | SND SV Perfection | SND SV Perfection |
| 85 | SND SV Sport Perfection | SND SV Sport Perfection |
| 86 | SND SV Perfection | SND SV Perfection (duplicate) |
| 87 | SND Natural Perfection | SND Natural Perfection |
| 88 | SND Professional Perfection | SND Professional Perfection |
| 89 | SND Office Accuracy | SND Office Accuracy |
| 90 | SND Tablet Perfection | SND Tablet Perfection |
| 91 | IOT ENDLESS endless office | IOT ENDLESS endless office |
| 92 | OPTO Sport Optics | OPTO Sport Optics |

### 4.5 Refractive Index

**Field Name**: `refractiveIndex` (value), `refractiveIndexValue` (text value)

| Value | Label | Description |
|-------|-------|-------------|
| 1 | 1.50 | Refractive index 1.50 |
| 2 | 1.56 | Refractive index 1.56 |
| 3 | 1.60 | Refractive index 1.60 |
| 4 | 1.67 | Refractive index 1.67 |
| 5 | 1.74 | Refractive index 1.74 |
| 6 | 1.50 | Refractive index 1.50 (duplicate) |
| 7 | 1.53 | Refractive index 1.53 |
| 8 | 1.56 | Refractive index 1.56 (duplicate) |
| 9 | 1.59 | Refractive index 1.59 |
| 10 | 1.60 | Refractive index 1.60 (duplicate) |
| 11 | 1.67 | Refractive index 1.67 (duplicate) |
| 12 | 1.74 | Refractive index 1.74 (duplicate) |

### 4.6 Channel / Corridor

**Field Name**: `channel`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | SV | Single vision |
| 2 | FT-28 / FT-28 | FT-28 channel |
| 3 | FT-35(only 1.6) / FT-35 | FT-35 channel |
| 4 | RT-28 | RT-28 channel |
| 5 | INV-28 | INV-28 channel |
| 6 | 5 | Channel 5 |
| 7 | 7 | Channel 7 |
| 8 | 9 | Channel 9 |
| 11 | 9 | Channel 9 (duplicate) |
| 12 | 11 | Channel 11 |
| 13 | 13 | Channel 13 |
| 14 | 14 | Channel 14 |
| 15 | 15 | Channel 15 |
| 16 | 16 | Channel 16 |
| 17 | 18 | Channel 18 |

### 4.7 Frame Type

**Field Name**: `frameType`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | Metal | Metal frame |
| 2 | Plastic | Plastic frame |
| 3 | Nylon | Nylon frame |
| 4 | Semi-rim | Semi-rim frame |
| 5 | Rimless | Rimless frame |
| 6 | Drill | Drill frame |

### 4.8 Frame Model

**Field Name**: `frameModel`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | 001 | Model 001 |
| 2 | 002 | Model 002 |
| 3 | 003 | Model 003 |
| 7 | 004 | Model 004 |
| 8 | 005 | Model 005 |
| 9 | 006 | Model 006 |
| 10 | 007 | Model 007 |
| 11 | 008 | Model 008 |

### 4.9 Remarks Type

**Field Name**: `remarksType` (supports multiple selection, multiple values separated by commas)

| Value | Label | Description |
|-------|-------|-------------|
| 1 | Need adhesive label | Need adhesive label |
| 2 | RUSH | Rush |
| 3 | KNIFE EDGE | Knife edge |
| 4 | AS THIN AS POSSIBLE | As thin as possible |
| 5 | SPECIFY EDGE THICKNESS | Specify edge thickness |
| 6 | SPECIFY CENTER THICKNESS | Specify center thickness |
| 7 | Need to be cut | Need to be cut |

**Note**: Remarks type field supports multiple selection. Multiple values need to be comma-separated when transmitting (e.g., "1,2,7").

### 4.10 Tinting

**Field Name**: `tinting`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | RED | Red tinting |
| 2 | SOLID TINT | Solid tint |
| 3 | GRADIENT TINT | Gradient tint |

### 4.11 Color

**Field Name**: `color`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | GREY | Grey |
| 2 | BROWN | Brown |
| 4 | Attach tint sample image(please use white background) | Attach tint sample image (please use white background) |

**Note**: When selecting color value 4, need to upload color attachment (`colorAttachment` field).

### 4.12 Order Status

**Field Name**: `orderStatus`

| Value | Label | Description |
|-------|-------|-------------|
| 1 | ORDER CREATED | Order created |
| 2 | CALCULATION | Calculation |
| 3 | CALCULATION | Calculation (duplicate) |
| 4 | QUALITY INSPECTION | Quality inspection |
| 5 | FINISHED | Finished |
| 6 | FINISHED | Finished (duplicate) |

**Note**: When creating an order, if `orderStatus` is not specified, system will default to "ORDER CREATED" (value 1).

---

## 5. Error Handling

### Common Error Codes

| Error Code | Description | Solution |
|------------|-------------|----------|
| 200 | Operation successful | - |
| 400 | Invalid request parameters | Check if request parameters are complete and correct |
| 401 | Unauthorized | Token invalid or expired, re-obtain Token |
| 403 | Insufficient permissions | Check if user has required permissions |
| 404 | Resource not found | Check if endpoint address is correct |
| 500 | Internal server error | Contact system administrator |

### Error Response Format

All error responses follow a unified format:

```json
{
  "code": error_code,
  "msg": "error_message",
  "data": null
}
```

---

## 6. Changelog

- **v1.1.0** (2024-01-XX): Added data enumeration definitions
  - Added enumeration value definitions for all option fields
  - Included 12 enumerations: lens type, coating, material, design name, refractive index, channel, frame type, frame model, remarks type, tinting, color, order status
  - Clarified fields that support multiple selection (material, remarksType) and their format requirements
  - Added field usage instructions and notes

- **v1.0.0** (2024-01-01): Initial version
  - Supported Get User Token API
  - Supported Create Complete Order API
  - Supported JWT Token authentication
  - Supported automatic order status management
