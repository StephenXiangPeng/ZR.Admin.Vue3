<template>
  <div class="app-container">
    <!-- 标签页导航 -->
    <el-tabs 
      v-model="activeTab" 
      :type="isMobile ? 'border-card' : 'card'" 
      class="management-tabs" 
      @tab-change="handleTabChange"
      :class="{ 'mobile-tabs': isMobile }">
      <el-tab-pane label="仓库管理" name="warehouse">
        <!-- 仓库管理内容 -->
        <div class="tab-content">
          <!-- 搜索/筛选区域 -->
          <el-form 
            :model="warehouseQueryParams" 
            :inline="!isMobile" 
            ref="warehouseQueryRef" 
            v-show="showSearch" 
            class="compact-form"
            :class="{ 'mobile-form': isMobile }">
            <el-form-item prop="warehouseCode">
              <el-input 
                v-model="warehouseQueryParams.warehouseCode" 
                placeholder="请输入编号/名称" 
                clearable 
                style="width: 180px"
                @keyup.enter="handleWarehouseQuery"  size="small"/>
            </el-form-item>
            <el-form-item prop="status">
              <el-select 
                v-model="warehouseQueryParams.status" 
                placeholder="请选择仓库状态" 
                clearable 
                style="width: 180px"
                size="small">
                <el-option label="启用" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-form-item>
            <el-form-item prop="warehouseType">
              <el-select 
                v-model="warehouseQueryParams.warehouseType" 
                placeholder="请选择仓库类型" 
                clearable 
                style="width: 180px"
                size="small">
                <el-option label="成品仓" value="1" />
                <el-option label="物料仓" value="2" />
              </el-select>
            </el-form-item>
            <el-form-item prop="binManagement">
              <el-select 
                v-model="warehouseQueryParams.binManagement" 
                placeholder="请选择库位管理状态" 
                clearable 
                style="width: 180px"
                size="small">
                <el-option label="启用" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" icon="Search" @click="handleWarehouseQuery" size="small">搜索</el-button>
              <el-button icon="Refresh" @click="resetWarehouseQuery" size="small">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 操作按钮区域 -->
          <el-row :gutter="8" class="compact-toolbar">
            <el-col :span="1.5">
              <el-button type="primary" icon="Plus" @click="handleAddWarehouse" size="small">新增</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" icon="Upload" @click="handleImportWarehouse" size="small">导入</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getWarehouseList" />
          </el-row>

          <!-- 数据表格 -->
          <el-table 
            v-loading="warehouseLoading" 
            :data="warehouseList" 
            border 
            highlight-current-row
            :size="isMobile ? 'default' : 'small'"
            :height="isMobile ? '300' : '400'"
            :class="{ 'mobile-table': isMobile }"
            @selection-change="handleWarehouseSelectionChange">
            <el-table-column type="selection" width="45" align="center" />
            <el-table-column label="编号" align="center" prop="warehouseCode" width="100" />
            <el-table-column label="名称" align="center" prop="warehouseName" width="120" />
            <el-table-column label="状态" align="center" prop="status" width="70">
              <template #default="scope">
                <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'" size="small">
                  {{ scope.row.status === '1' ? '启用' : '禁用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="仓库类型" align="center" prop="warehouseType" width="80">
              <template #default="scope">
                <el-tag :type="scope.row.warehouseType === '1' ? 'primary' : 'warning'" size="small">
                  {{ scope.row.warehouseType === '1' ? '成品仓' : '物料仓' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="启用库位管理" align="center" prop="binManagement" width="100">
              <template #default="scope">
                <el-icon v-if="scope.row.binManagement === '1'" color="#67C23A" size="16">
                  <Check />
                </el-icon>
                <span v-else>-</span>
              </template>
            </el-table-column>
            <el-table-column label="联系人" align="center" prop="contactPerson" width="80">
              <template #default="scope">
                {{ scope.row.contactPerson || '-' }}
              </template>
            </el-table-column>
            <el-table-column label="联系电话" align="center" prop="contactPhone" width="100" />
            <el-table-column label="创建人" align="center" prop="createBy" width="100" />
            <el-table-column label="创建时间" align="center" prop="createTime" width="140" />
            <el-table-column label="操作" align="center" width="180" fixed="right">
              <template #default="scope">
                <el-button type="primary" link icon="Edit" @click="handleUpdateWarehouse(scope.row)" size="small">编辑</el-button>
                <el-button 
                  type="warning" 
                  link 
                  icon="Lock" 
                  @click="handleWarehouseStatusChange(scope.row)"
                  v-if="scope.row.status === '1'"
                  size="small">
                  禁用
                </el-button>
                <el-button 
                  type="success" 
                  link 
                  icon="Unlock" 
                  @click="handleWarehouseStatusChange(scope.row)"
                  v-else
                  size="small">
                  启用
                </el-button>
                <el-button type="danger" link icon="Delete" @click="handleDeleteWarehouse(scope.row)" size="small">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分页组件 -->
          <pagination 
            v-show="warehouseTotal > 0" 
            :total="warehouseTotal" 
            v-model:page="warehouseQueryParams.pageNum" 
            v-model:limit="warehouseQueryParams.pageSize" 
            @pagination="getWarehouseList" />
        </div>
      </el-tab-pane>

      <el-tab-pane label="库区管理" name="zone">
        <!-- 库区管理内容 -->
        <div class="tab-content">
          <!-- 搜索/筛选区域 -->
          <el-form 
            :model="zoneQueryParams" 
            :inline="!isMobile" 
            ref="zoneQueryRef" 
            v-show="showSearch" 
            class="compact-form"
            :class="{ 'mobile-form': isMobile }">
            <el-form-item prop="zoneCode">
              <el-input 
                v-model="zoneQueryParams.zoneCode" 
                placeholder="请输入库区名称/编号" 
                clearable 
                style="width: 180px"
                @keyup.enter="handleZoneQuery"  size="small"/>
            </el-form-item>
            <el-form-item prop="warehouseName">
              <el-input 
                v-model="zoneQueryParams.warehouseName" 
                placeholder="请输入仓库名称" 
                clearable 
                style="width: 180px"
                size="small"/>
            </el-form-item>
            <el-form-item prop="status">
              <el-select 
                v-model="zoneQueryParams.status" 
                placeholder="请选择状态" 
                clearable 
                style="width: 180px"
                size="small">
                <el-option label="正常" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" icon="Search" @click="handleZoneQuery" size="small">搜索</el-button>
              <el-button icon="Refresh" @click="resetZoneQuery" size="small">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 操作按钮区域 -->
          <el-row :gutter="8" class="compact-toolbar">
            <el-col :span="1.5">
              <el-button type="primary" icon="Plus" @click="handleAddZone" size="small">新增</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" icon="Upload" @click="handleImportZone" size="small">导入</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getZoneList" />
          </el-row>

          <!-- 数据表格 -->
          <el-table 
            v-loading="zoneLoading" 
            :data="zoneList" 
            border 
            highlight-current-row
            :size="isMobile ? 'default' : 'small'"
            :height="isMobile ? '300' : '400'"
            :class="{ 'mobile-table': isMobile }"
            @selection-change="handleZoneSelectionChange">
            <el-table-column type="selection" width="45" align="center" />
            <el-table-column label="编号" align="center" prop="zoneCode" width="120" />
            <el-table-column label="库区名称" align="center" prop="zoneName" width="100" />
            <el-table-column label="所属仓库" align="center" prop="warehouseName" width="100" />
            <el-table-column label="所属类型" align="center" prop="warehouseType" width="80">
              <template #default="scope">
                <el-tag :type="scope.row.warehouseType === '1' ? 'primary' : 'warning'" size="small">
                  {{ scope.row.warehouseType === '1' ? '成品仓' : '物料仓' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="备注" align="center" prop="remark" width="80">
              <template #default="scope">
                {{ scope.row.remark || '-' }}
              </template>
            </el-table-column>
            <el-table-column label="使用状态" align="center" prop="status" width="80">
              <template #default="scope">
                <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'" size="small">
                  {{ scope.row.status === '1' ? '正常' : '禁用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="创建人" align="center" prop="createBy" width="100" />
            <el-table-column label="创建时间" align="center" prop="createTime" width="140" />
            <el-table-column label="最后修改人" align="center" prop="updateBy" width="100" />
            <el-table-column label="最后修改时间" align="center" prop="updateTime" width="140" />
            <el-table-column label="操作" align="center" width="180" fixed="right">
              <template #default="scope">
                <el-button type="primary" link icon="Edit" @click="handleUpdateZone(scope.row)" size="small">编辑</el-button>
                <el-button 
                  type="warning" 
                  link 
                  icon="Lock" 
                  @click="handleZoneStatusChange(scope.row)"
                  v-if="scope.row.status === '1'"
                  size="small">
                  禁用
                </el-button>
                <el-button 
                  type="success" 
                  link 
                  icon="Unlock" 
                  @click="handleZoneStatusChange(scope.row)"
                  v-else
                  size="small">
                  启用
                </el-button>
                <el-button type="danger" link icon="Delete" @click="handleDeleteZone(scope.row)" size="small">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分页组件 -->
          <pagination 
            v-show="zoneTotal > 0" 
            :total="zoneTotal" 
            v-model:page="zoneQueryParams.pageNum" 
            v-model:limit="zoneQueryParams.pageSize" 
            @pagination="getZoneList" />
        </div>
      </el-tab-pane>

      <el-tab-pane label="库位管理" name="location">
        <!-- 库位管理内容 -->
        <div class="tab-content">
          <!-- 搜索/筛选区域 -->
          <el-form 
            :model="locationQueryParams" 
            :inline="!isMobile" 
            ref="locationQueryRef" 
            v-show="showSearch" 
            class="compact-form"
            :class="{ 'mobile-form': isMobile }">
            <el-form-item prop="locationCode">
              <el-input 
                v-model="locationQueryParams.locationCode" 
                placeholder="请输入库位编号/名称" 
                clearable 
                style="width: 180px"
                @keyup.enter="handleLocationQuery"  size="small"/>
            </el-form-item>
            <el-form-item prop="warehouseName">
              <el-input 
                v-model="locationQueryParams.warehouseName" 
                placeholder="请输入仓库名称" 
                clearable 
                style="width: 180px"
                size="small"/>
            </el-form-item>
            <el-form-item prop="status">
              <el-select 
                v-model="locationQueryParams.status" 
                placeholder="请选择状态" 
                clearable 
                style="width: 180px"
                size="small">
                <el-option label="正常" value="1" />
                <el-option label="禁用" value="0" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" icon="Search" @click="handleLocationQuery" size="small">搜索</el-button>
              <el-button icon="Refresh" @click="resetLocationQuery" size="small">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 操作按钮区域 -->
          <el-row :gutter="8" class="compact-toolbar">
            <el-col :span="1.5">
              <el-button type="primary" icon="Plus" @click="handleAddLocation" size="small">新增</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" icon="Upload" @click="handleImportLocation" size="small">导入</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getLocationList" />
          </el-row>

          <!-- 数据表格 -->
          <el-table 
            v-loading="locationLoading" 
            :data="locationList" 
            border 
            highlight-current-row
            :size="isMobile ? 'default' : 'small'"
            :height="isMobile ? '300' : '400'"
            :class="{ 'mobile-table': isMobile }"
            @selection-change="handleLocationSelectionChange">
            <el-table-column type="selection" width="45" align="center" />
            <el-table-column label="编号" align="center" prop="locationCode" width="120" />
            <el-table-column label="库位名称" align="center" prop="locationName" width="100" />
            <el-table-column label="所属库区" align="center" prop="zoneName" width="100">
              <template #default="scope">
                {{ scope.row.zoneName || '-' }}
              </template>
            </el-table-column>
            <el-table-column label="所属仓库" align="center" prop="warehouseName" width="100" />
            <el-table-column label="所属类型" align="center" prop="warehouseType" width="80">
              <template #default="scope">
                <el-tag :type="scope.row.warehouseType === '1' ? 'primary' : 'warning'" size="small">
                  {{ scope.row.warehouseType === '1' ? '成品仓' : '物料仓' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="备注" align="center" prop="remark" width="80">
              <template #default="scope">
                {{ scope.row.remark || '-' }}
              </template>
            </el-table-column>
            <el-table-column label="使用状态" align="center" prop="status" width="80">
              <template #default="scope">
                <el-tag :type="scope.row.status === '1' ? 'success' : 'danger'" size="small">
                  {{ scope.row.status === '1' ? '正常' : '禁用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="创建人" align="center" prop="createBy" width="100" />
            <el-table-column label="创建时间" align="center" prop="createTime" width="140" />
            <el-table-column label="最后修改人" align="center" prop="updateBy" width="100" />
            <el-table-column label="最后修改时间" align="center" prop="updateTime" width="140" />
            <el-table-column label="操作" align="center" width="180" fixed="right">
              <template #default="scope">
                <el-button type="primary" link icon="Edit" @click="handleUpdateLocation(scope.row)" size="small">编辑</el-button>
                <el-button 
                  type="warning" 
                  link 
                  icon="Lock" 
                  @click="handleLocationStatusChange(scope.row)"
                  v-if="scope.row.status === '1'"
                  size="small">
                  禁用
                </el-button>
                <el-button 
                  type="success" 
                  link 
                  icon="Unlock" 
                  @click="handleLocationStatusChange(scope.row)"
                  v-else
                  size="small">
                  启用
                </el-button>
                <el-button type="danger" link icon="Delete" @click="handleDeleteLocation(scope.row)" size="small">删除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分页组件 -->
          <pagination 
            v-show="locationTotal > 0" 
            :total="locationTotal" 
            v-model:page="locationQueryParams.pageNum" 
            v-model:limit="locationQueryParams.pageSize" 
            @pagination="getLocationList" />
        </div>
      </el-tab-pane>
    </el-tabs>

    <!-- 仓库对话框 -->
    <el-dialog :title="warehouseTitle" v-model="warehouseOpen" width="600px" append-to-body>
      <el-form ref="warehouseRef" :model="warehouseForm" :rules="warehouseRules" label-width="120px">
        <el-row>
          <el-col :span="12">
            <el-form-item label="仓库编号" prop="warehouseCode">
              <el-input v-model="warehouseForm.warehouseCode" placeholder="请输入仓库编号" size="default"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="仓库名称" prop="warehouseName">
              <el-input v-model="warehouseForm.warehouseName" placeholder="请输入仓库名称" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="仓库类型" prop="warehouseType">
              <el-select v-model="warehouseForm.warehouseType" placeholder="请选择仓库类型" style="width: 100%" size="default">
                <el-option label="成品仓" value="1" />
                <el-option label="物料仓" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="warehouseForm.status" size="default">
                <el-radio value="1">启用</el-radio>
                <el-radio value="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="启用库位管理" prop="binManagement">
              <el-radio-group v-model="warehouseForm.binManagement" size="default">
                <el-radio value="1">启用</el-radio>
                <el-radio value="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系人" prop="contactPerson">
              <el-input v-model="warehouseForm.contactPerson" placeholder="请输入联系人" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="contactPhone">
              <el-input v-model="warehouseForm.contactPhone" placeholder="请输入联系电话" size="default"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="备注" prop="remark">
              <el-input v-model="warehouseForm.remark" placeholder="请输入备注" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitWarehouseForm" size="default">确 定</el-button>
          <el-button @click="cancelWarehouse" size="default">取 消</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 库区对话框 -->
    <el-dialog :title="zoneTitle" v-model="zoneOpen" width="600px" append-to-body>
      <el-form ref="zoneRef" :model="zoneForm" :rules="zoneRules" label-width="120px">
        <el-row>
          <el-col :span="12">
            <el-form-item label="库区编号" prop="zoneCode">
              <el-input v-model="zoneForm.zoneCode" placeholder="请输入库区编号" size="default"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="库区名称" prop="zoneName">
              <el-input v-model="zoneForm.zoneName" placeholder="请输入库区名称" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="所属仓库" prop="warehouseId">
              <el-select v-model="zoneForm.warehouseId" placeholder="请选择所属仓库" style="width: 100%" size="default">
                <el-option 
                  v-for="warehouse in warehouseOptions" 
                  :key="warehouse.warehouseId" 
                  :label="warehouse.warehouseName" 
                  :value="warehouse.warehouseId" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="zoneForm.status" size="default">
                <el-radio value="1">启用</el-radio>
                <el-radio value="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注" prop="remark">
              <el-input v-model="zoneForm.remark" placeholder="请输入备注" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitZoneForm" size="default">确 定</el-button>
          <el-button @click="cancelZone" size="default">取 消</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 库位对话框 -->
    <el-dialog :title="locationTitle" v-model="locationOpen" width="600px" append-to-body>
      <el-form ref="locationRef" :model="locationForm" :rules="locationRules" label-width="120px">
        <el-row>
          <el-col :span="12">
            <el-form-item label="库位编号" prop="locationCode">
              <el-input v-model="locationForm.locationCode" placeholder="请输入库位编号" size="default"/>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="库位名称" prop="locationName">
              <el-input v-model="locationForm.locationName" placeholder="请输入库位名称" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="12">
            <el-form-item label="所属库区" prop="zoneId">
              <el-select v-model="locationForm.zoneId" placeholder="请选择所属库区" style="width: 100%" size="default">
                <el-option 
                  v-for="zone in zoneOptions" 
                  :key="zone.zoneId" 
                  :label="zone.zoneName" 
                  :value="zone.zoneId" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="locationForm.status" size="default">
                <el-radio value="1">启用</el-radio>
                <el-radio value="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注" prop="remark">
              <el-input v-model="locationForm.remark" placeholder="请输入备注" size="default"/>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitLocationForm" size="default">确 定</el-button>
          <el-button @click="cancelLocation" size="default">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, getCurrentInstance, onBeforeUnmount } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Check, ArrowDown, ArrowUp } from '@element-plus/icons-vue'

// 标签页状态控制
const activeTab = ref('warehouse')

// 移动端检测
const isMobile = ref(false)

// 检测设备类型
function checkDevice() {
  const userAgent = navigator.userAgent.toLowerCase()
  const isMobileDevice = /mobile|android|iphone|ipad|phone|blackberry|opera mini|windows phone/i.test(userAgent)
  const isSmallScreen = window.innerWidth <= 768
  
  isMobile.value = isMobileDevice || isSmallScreen
}

// 监听窗口大小变化
function handleResize() {
  checkDevice()
}

// 显示搜索条件
const showSearch = ref(true)

// 仓库管理相关
const warehouseLoading = ref(true)
const warehouseIds = ref([])
const warehouseSingle = ref(true)
const warehouseMultiple = ref(true)
const warehouseTotal = ref(0)
const warehouseList = ref([])
const warehouseTitle = ref("")
const warehouseOpen = ref(false)

// 库区管理相关
const zoneLoading = ref(true)
const zoneIds = ref([])
const zoneSingle = ref(true)
const zoneMultiple = ref(true)
const zoneTotal = ref(0)
const zoneList = ref([])
const zoneTitle = ref("")
const zoneOpen = ref(false)

// 库位管理相关
const locationLoading = ref(true)
const locationIds = ref([])
const locationSingle = ref(true)
const locationMultiple = ref(true)
const locationTotal = ref(0)
const locationList = ref([])
const locationTitle = ref("")
const locationOpen = ref(false)

// 选项数据
const warehouseOptions = ref([])
const zoneOptions = ref([])

// 仓库查询参数
const warehouseQueryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  warehouseCode: undefined,
  status: undefined,
  warehouseType: undefined,
  binManagement: undefined
})

// 库区查询参数
const zoneQueryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  zoneCode: undefined,
  warehouseName: undefined,
  status: undefined
})

// 库位查询参数
const locationQueryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  locationCode: undefined,
  warehouseName: undefined,
  status: undefined
})

// 仓库表单参数
const warehouseForm = reactive({
  warehouseId: undefined,
  warehouseCode: undefined,
  warehouseName: undefined,
  warehouseType: undefined,
  status: "1",
  binManagement: "0",
  contactPerson: undefined,
  contactPhone: undefined,
  remark: undefined
})

// 库区表单参数
const zoneForm = reactive({
  zoneId: undefined,
  zoneCode: undefined,
  zoneName: undefined,
  warehouseId: undefined,
  status: "1",
  remark: undefined
})

// 库位表单参数
const locationForm = reactive({
  locationId: undefined,
  locationCode: undefined,
  locationName: undefined,
  zoneId: undefined,
  status: "1",
  remark: undefined
})

// 仓库表单校验规则
const warehouseRules = reactive({
  warehouseCode: [
    { required: true, message: "仓库编号不能为空", trigger: "blur" }
  ],
  warehouseName: [
    { required: true, message: "仓库名称不能为空", trigger: "blur" }
  ],
  warehouseType: [
    { required: true, message: "仓库类型不能为空", trigger: "change" }
  ]
})

// 库区表单校验规则
const zoneRules = reactive({
  zoneCode: [
    { required: true, message: "库区编号不能为空", trigger: "blur" }
  ],
  zoneName: [
    { required: true, message: "库区名称不能为空", trigger: "blur" }
  ],
  warehouseId: [
    { required: true, message: "所属仓库不能为空", trigger: "change" }
  ]
})

// 库位表单校验规则
const locationRules = reactive({
  locationCode: [
    { required: true, message: "库位编号不能为空", trigger: "blur" }
  ],
  locationName: [
    { required: true, message: "库位名称不能为空", trigger: "blur" }
  ],
  zoneId: [
    { required: true, message: "所属库区不能为空", trigger: "change" }
  ]
})

const { proxy } = getCurrentInstance()

// 标签页切换函数
function handleTabChange(tabName) {
  activeTab.value = tabName
}

/** 查询仓库列表 */
function getWarehouseList() {
  warehouseLoading.value = true
  // 这里应该调用实际的API接口
  // listWarehouse(warehouseQueryParams).then(response => {
  //   warehouseLoading.value = false
  //   warehouseList.value = response.data.result
  //   warehouseTotal.value = response.data.totalNum
  // })
  
  // 模拟数据
  setTimeout(() => {
    warehouseLoading.value = false
    warehouseList.value = [
      {
        warehouseId: 1,
        warehouseCode: "CK2025001",
        warehouseName: "test仓库",
        status: "1",
        warehouseType: "1",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2025-06-24 10:30:00",
        updateBy: "镇江奔驰马...",
        updateTime: "2025-06-24 10:30:00"
      },
      {
        warehouseId: 2,
        warehouseCode: "CK20241001",
        warehouseName: "物料仓",
        status: "1",
        warehouseType: "2",
        binManagement: "1",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-12-02 15:20:00",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-12-02 15:20:00"
      },
      {
        warehouseId: 3,
        warehouseCode: "DYX",
        warehouseName: "DYX",
        status: "1",
        warehouseType: "1",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      },
      {
        warehouseId: 4,
        warehouseCode: "DYD",
        warehouseName: "DYD",
        status: "1",
        warehouseType: "2",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      },
      {
        warehouseId: 5,
        warehouseCode: "NE",
        warehouseName: "NE",
        status: "1",
        warehouseType: "1",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      },
      {
        warehouseId: 6,
        warehouseCode: "XY",
        warehouseName: "XY",
        status: "1",
        warehouseType: "2",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      },
      {
        warehouseId: 7,
        warehouseCode: "NY",
        warehouseName: "NY",
        status: "1",
        warehouseType: "1",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      },
      {
        warehouseId: 8,
        warehouseCode: "XYD",
        warehouseName: "XYD",
        status: "1",
        warehouseType: "2",
        binManagement: "0",
        contactPerson: null,
        contactPhone: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-11-25 11:26:50",
        updateBy: "镇江奔驰马...",
        updateTime: "2024-11-25 11:26:50"
      }
    ]
    warehouseTotal.value = warehouseList.value.length
    warehouseOptions.value = warehouseList.value
  }, 500)
}

/** 查询库区列表 */
function getZoneList() {
  zoneLoading.value = true
  // 模拟数据
  setTimeout(() => {
    zoneLoading.value = false
    zoneList.value = [
      {
        zoneId: 1,
        zoneCode: "CKKQ20241202001",
        zoneName: "NY",
        warehouseName: "物料仓",
        warehouseType: "2",
        status: "1",
        remark: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-12-02 13:07:29",
        updateBy: "镇江奔驰马具有限...",
        updateTime: "2024-12-02 13:07:29"
      },
      {
        zoneId: 2,
        zoneCode: "CKKQ20241202002",
        zoneName: "NE",
        warehouseName: "物料仓",
        warehouseType: "2",
        status: "1",
        remark: null,
        createBy: "镇江奔驰马...",
        createTime: "2024-12-02 13:07:13",
        updateBy: "镇江奔驰马具有限...",
        updateTime: "2024-12-02 13:07:13"
      }
    ]
    zoneTotal.value = zoneList.value.length
    zoneOptions.value = zoneList.value
  }, 500)
}

/** 查询库位列表 */
function getLocationList() {
  locationLoading.value = true
  // 模拟数据
  setTimeout(() => {
    locationLoading.value = false
    locationList.value = [
      {
        locationId: 1,
        locationCode: "CKKW20241202001",
        locationName: "NY",
        zoneName: null,
        warehouseName: "物料仓",
        warehouseType: "2",
        status: "1",
        remark: null,
        createBy: "镇江奔驰...",
        createTime: "2024-12-02 13:07:29",
        updateBy: "镇江奔驰马具有...",
        updateTime: "2024-12-02 13:07:29"
      }
    ]
    locationTotal.value = locationList.value.length
  }, 500)
}

// 仓库管理相关函数
function handleWarehouseQuery() {
  warehouseQueryParams.pageNum = 1
  getWarehouseList()
}

function resetWarehouseQuery() {
  proxy.resetForm("warehouseQueryRef")
  handleWarehouseQuery()
}

function handleWarehouseSelectionChange(selection) {
  warehouseIds.value = selection.map(item => item.warehouseId)
  warehouseSingle.value = selection.length !== 1
  warehouseMultiple.value = !selection.length
}

function handleAddWarehouse() {
  resetWarehouseForm()
  warehouseOpen.value = true
  warehouseTitle.value = "添加仓库"
}

function handleUpdateWarehouse(row) {
  resetWarehouseForm()
  Object.assign(warehouseForm, row)
  warehouseOpen.value = true
  warehouseTitle.value = "修改仓库"
}

function submitWarehouseForm() {
  proxy.$refs["warehouseRef"].validate(valid => {
    if (valid) {
      if (warehouseForm.warehouseId != null) {
        ElMessage.success("修改成功")
        warehouseOpen.value = false
        getWarehouseList()
      } else {
        ElMessage.success("新增成功")
        warehouseOpen.value = false
        getWarehouseList()
      }
    }
  })
}

function cancelWarehouse() {
  warehouseOpen.value = false
  resetWarehouseForm()
}

function resetWarehouseForm() {
  warehouseForm.warehouseId = undefined
  warehouseForm.warehouseCode = undefined
  warehouseForm.warehouseName = undefined
  warehouseForm.warehouseType = undefined
  warehouseForm.status = "1"
  warehouseForm.binManagement = "0"
  warehouseForm.contactPerson = undefined
  warehouseForm.contactPhone = undefined
  warehouseForm.remark = undefined
  proxy.resetForm("warehouseRef")
}

function handleDeleteWarehouse(row) {
  const warehouseIds = row.warehouseId || warehouseIds.value
  ElMessageBox.confirm('是否确认删除仓库编号为"' + warehouseIds + '"的数据项?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    getWarehouseList()
    ElMessage.success("删除成功")
  }).catch(() => {})
}

function handleWarehouseStatusChange(row) {
  let text = row.status === "1" ? "启用" : "停用"
  ElMessageBox.confirm('确认要"' + text + '""' + row.warehouseName + '"仓库吗?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    ElMessage.success(text + "成功")
  }).catch(function() {
    row.status = row.status === "1" ? "0" : "1"
  })
}

function handleImportWarehouse() {
  ElMessage.info("导入功能待实现")
}

// 库区管理相关函数
function handleZoneQuery() {
  zoneQueryParams.pageNum = 1
  getZoneList()
}

function resetZoneQuery() {
  proxy.resetForm("zoneQueryRef")
  handleZoneQuery()
}

function handleZoneSelectionChange(selection) {
  zoneIds.value = selection.map(item => item.zoneId)
  zoneSingle.value = selection.length !== 1
  zoneMultiple.value = !selection.length
}

function handleAddZone() {
  resetZoneForm()
  zoneOpen.value = true
  zoneTitle.value = "添加库区"
}

function handleUpdateZone(row) {
  resetZoneForm()
  Object.assign(zoneForm, row)
  zoneOpen.value = true
  zoneTitle.value = "修改库区"
}

function submitZoneForm() {
  proxy.$refs["zoneRef"].validate(valid => {
    if (valid) {
      if (zoneForm.zoneId != null) {
        ElMessage.success("修改成功")
        zoneOpen.value = false
        getZoneList()
      } else {
        ElMessage.success("新增成功")
        zoneOpen.value = false
        getZoneList()
      }
    }
  })
}

function cancelZone() {
  zoneOpen.value = false
  resetZoneForm()
}

function resetZoneForm() {
  zoneForm.zoneId = undefined
  zoneForm.zoneCode = undefined
  zoneForm.zoneName = undefined
  zoneForm.warehouseId = undefined
  zoneForm.status = "1"
  zoneForm.remark = undefined
  proxy.resetForm("zoneRef")
}

function handleDeleteZone(row) {
  const zoneIds = row.zoneId || zoneIds.value
  ElMessageBox.confirm('是否确认删除库区编号为"' + zoneIds + '"的数据项?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    getZoneList()
    ElMessage.success("删除成功")
  }).catch(() => {})
}

function handleZoneStatusChange(row) {
  let text = row.status === "1" ? "启用" : "停用"
  ElMessageBox.confirm('确认要"' + text + '""' + row.zoneName + '"库区吗?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    ElMessage.success(text + "成功")
  }).catch(function() {
    row.status = row.status === "1" ? "0" : "1"
  })
}

function handleImportZone() {
  ElMessage.info("导入功能待实现")
}

// 库位管理相关函数
function handleLocationQuery() {
  locationQueryParams.pageNum = 1
  getLocationList()
}

function resetLocationQuery() {
  proxy.resetForm("locationQueryRef")
  handleLocationQuery()
}

function handleLocationSelectionChange(selection) {
  locationIds.value = selection.map(item => item.locationId)
  locationSingle.value = selection.length !== 1
  locationMultiple.value = !selection.length
}

function handleAddLocation() {
  resetLocationForm()
  locationOpen.value = true
  locationTitle.value = "添加库位"
}

function handleUpdateLocation(row) {
  resetLocationForm()
  Object.assign(locationForm, row)
  locationOpen.value = true
  locationTitle.value = "修改库位"
}

function submitLocationForm() {
  proxy.$refs["locationRef"].validate(valid => {
    if (valid) {
      if (locationForm.locationId != null) {
        ElMessage.success("修改成功")
        locationOpen.value = false
        getLocationList()
      } else {
        ElMessage.success("新增成功")
        locationOpen.value = false
        getLocationList()
      }
    }
  })
}

function cancelLocation() {
  locationOpen.value = false
  resetLocationForm()
}

function resetLocationForm() {
  locationForm.locationId = undefined
  locationForm.locationCode = undefined
  locationForm.locationName = undefined
  locationForm.zoneId = undefined
  locationForm.status = "1"
  locationForm.remark = undefined
  proxy.resetForm("locationRef")
}

function handleDeleteLocation(row) {
  const locationIds = row.locationId || locationIds.value
  ElMessageBox.confirm('是否确认删除库位编号为"' + locationIds + '"的数据项?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    getLocationList()
    ElMessage.success("删除成功")
  }).catch(() => {})
}

function handleLocationStatusChange(row) {
  let text = row.status === "1" ? "启用" : "停用"
  ElMessageBox.confirm('确认要"' + text + '""' + row.locationName + '"库位吗?', "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(function() {
    return Promise.resolve()
  }).then(() => {
    ElMessage.success(text + "成功")
  }).catch(function() {
    row.status = row.status === "1" ? "0" : "1"
  })
}

function handleImportLocation() {
  ElMessage.info("导入功能待实现")
}

onMounted(() => {
  checkDevice()
  window.addEventListener('resize', handleResize)
  
  getWarehouseList()
  getZoneList()
  getLocationList()
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
/* 标签页样式 */
.management-tabs {
  height: calc(100vh - 120px);
  overflow: hidden;
}

.tab-content {
  padding: 16px;
  height: calc(100vh - 200px);
  overflow-y: auto;
}

/* 移动端标签页样式 */
.mobile-tabs {
  height: calc(100vh - 80px);
}

.mobile-tabs .tab-content {
  height: calc(100vh - 140px);
  padding: 12px 8px;
}

/* 紧凑表单样式 */
.compact-form {
  margin-bottom: 12px;
}

.compact-form .el-form-item {
  margin-bottom: 8px;
  margin-right: 16px;
}

.compact-form .el-form-item:last-child {
  margin-right: 0;
}

/* 移动端表单样式 */
.mobile-form {
  margin-bottom: 16px;
}

.mobile-form .el-form-item {
  margin-bottom: 12px;
  margin-right: 0;
  width: 100%;
}

.mobile-form .el-form-item .el-input,
.mobile-form .el-form-item .el-select {
  width: 100% !important;
}

.mobile-form .el-form-item .el-button {
  width: 100%;
  margin-bottom: 8px;
}

.mobile-form .el-form-item:last-child .el-button {
  margin-bottom: 0;
}

/* 紧凑工具栏样式 */
.compact-toolbar {
  margin-bottom: 12px;
}

.compact-toolbar .el-col {
  margin-bottom: 4px;
}

/* 表格紧凑样式 */
:deep(.el-table) {
  font-size: 12px;
}

:deep(.el-table .el-table__cell) {
  padding: 4px 0;
}

:deep(.el-table .el-table__header .el-table__cell) {
  padding: 8px 0;
  background-color: #f5f7fa;
}

/* 移动端表格样式 */
.mobile-table {
  font-size: 14px;
}

.mobile-table :deep(.el-table__body-wrapper) {
  overflow-x: auto;
}

.mobile-table :deep(.el-table__cell) {
  padding: 8px 4px;
  min-width: 80px;
}

.mobile-table :deep(.el-table__header .el-table__cell) {
  padding: 12px 4px;
  font-weight: 600;
}

/* 移动端表格横向滚动提示 */
.mobile-table::after {
  content: "← 左右滑动查看更多 →";
  display: block;
  text-align: center;
  color: #909399;
  font-size: 12px;
  padding: 8px 0;
  background: #f5f7fa;
  border-radius: 4px;
  margin-top: 8px;
}

/* 按钮链接样式 */
:deep(.el-button--link) {
  padding: 0;
  margin: 0 4px;
  font-size: 12px;
}

:deep(.el-button--link:first-child) {
  margin-left: 0;
}

:deep(.el-button--link:last-child) {
  margin-right: 0;
}

/* 标签页头部样式 */
:deep(.el-tabs__header) {
  margin-bottom: 0;
  background-color: #f5f7fa;
  border-bottom: 1px solid #e4e7ed;
}

:deep(.el-tabs__nav-wrap) {
  padding: 0 16px;
}

:deep(.el-tabs__item) {
  height: 40px;
  line-height: 40px;
  font-size: 14px;
  font-weight: 500;
}

:deep(.el-tabs__item.is-active) {
  background-color: #fff;
  border-bottom: 2px solid #409eff;
}

/* 分页组件样式 */
:deep(.el-pagination) {
  margin-top: 16px;
  text-align: right;
}

/* 对话框样式 */
.dialog-footer {
  text-align: right;
}

/* 响应式设计 */
@media (max-width: 1200px) {
  .tab-content {
    padding: 12px;
  }
  
  .compact-form .el-form-item .el-input,
  .compact-form .el-form-item .el-select {
    width: 160px !important;
  }
}

@media (max-width: 768px) {
  .management-tabs {
    height: calc(100vh - 80px);
  }
  
  .tab-content {
    height: calc(100vh - 140px);
    padding: 8px;
  }
  
  .compact-form .el-form-item .el-input,
  .compact-form .el-form-item .el-select {
    width: 100% !important;
  }
  
  .compact-form .el-form-item {
    margin-right: 0;
    margin-bottom: 12px;
  }
  
  /* 移动端标签页优化 */
  :deep(.el-tabs__header) {
    margin-bottom: 8px;
  }
  
  :deep(.el-tabs__nav-wrap) {
    padding: 0 8px;
  }
  
  :deep(.el-tabs__item) {
    height: 36px;
    line-height: 36px;
    font-size: 13px;
    padding: 0 12px;
  }
  
  /* 移动端工具栏优化 */
  .compact-toolbar .el-col {
    margin-bottom: 8px;
  }
  
  .compact-toolbar .el-button {
    width: 100%;
    margin-bottom: 4px;
  }
  
  /* 移动端分页优化 */
  :deep(.el-pagination) {
    text-align: center;
    margin-top: 12px;
  }
  
  :deep(.el-pagination .el-pagination__total) {
    display: none;
  }
  
  :deep(.el-pagination .el-pagination__sizes) {
    display: none;
  }
}

/* 滚动条样式 */
.tab-content::-webkit-scrollbar {
  width: 6px;
}

.tab-content::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.tab-content::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.tab-content::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* 触摸友好样式 */
@media (max-width: 768px) {
  /* 增加触摸目标大小 */
  .el-button {
    min-height: 44px;
    min-width: 44px;
  }
  
  .el-input__wrapper,
  .el-select .el-input__wrapper {
    min-height: 44px;
  }
  
  /* 触摸滚动优化 */
  .tab-content {
    -webkit-overflow-scrolling: touch;
    scroll-behavior: smooth;
  }
  
  /* 移动端表格触摸优化 */
  .mobile-table :deep(.el-table__body-wrapper) {
    -webkit-overflow-scrolling: touch;
  }
  
  /* 移动端标签页触摸优化 */
  :deep(.el-tabs__item) {
    min-height: 44px;
    display: flex;
    align-items: center;
    justify-content: center;
  }
}
</style>