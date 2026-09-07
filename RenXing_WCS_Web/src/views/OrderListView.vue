<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { cancelOrder, finishOrder, getOrders } from '../api/wcs';
import { useRouter } from 'vue-router';
import { useWcsStore } from '../stores/wcs';
import type { OrderInfo } from '../types/wcs';

const router = useRouter(); const wcs = useWcsStore(); const rows = ref<OrderInfo[]>(wcs.orders); const sortKey = ref<keyof OrderInfo>('orderCode'); const sortOrder = ref<'ascending'|'descending'>('ascending'); const loading = ref(false);
const sortedRows = computed(() => [...rows.value].sort((a,b) => String(a[sortKey.value] ?? '').localeCompare(String(b[sortKey.value] ?? ''), 'zh-CN', { numeric:true }) * (sortOrder.value === 'ascending' ? 1 : -1)));
async function load() { loading.value = true; try { rows.value = await getOrders(); wcs.orders = rows.value; } finally { loading.value = false; } }
function sortChange({ prop, order }: { prop: keyof OrderInfo; order: 'ascending'|'descending'|null }) { if (order) { sortKey.value = prop; sortOrder.value = order; } }
async function action(row: OrderInfo, kind: 'cancel'|'finish') { await ElMessageBox.confirm(kind === 'finish' ? '确定要结束该任务吗？' : '确定要取消该任务吗？'); const result = await (kind === 'finish' ? finishOrder(row.orderCode) : cancelOrder(row.orderCode)); if (result.success) { ElMessage.success('操作已提交'); await load(); } else ElMessage.error(result.message); }
onMounted(() => load().catch(() => ElMessage.error('订单数据加载失败')));
</script>
<template>
  <section class="page-view"><div class="page-heading"><div><h1>订单监控</h1></div><div class="heading-actions"><el-button type="primary" plain :loading="loading" @click="load">刷新订单</el-button></div></div>
    <div class="content-card mobile-scroll"><el-table :data="sortedRows" border stripe @sort-change="sortChange"><el-table-column prop="orderCode" label="订单号" min-width="150" sortable="custom"/><el-table-column prop="plateCode" label="档案盒" min-width="110" sortable="custom"/><el-table-column prop="orderType" label="类型" min-width="100" sortable="custom"/><el-table-column prop="startNode" label="起点" min-width="100" sortable="custom"/><el-table-column prop="endNode" label="终点" min-width="100" sortable="custom"/><el-table-column prop="orderState" label="状态" min-width="100" sortable="custom"/><el-table-column label="操作" width="240" fixed="right"><template #default="{ row }"><el-button size="small" @click="router.push(`/orders/${row.orderCode}`)">查看</el-button><el-button size="small" type="warning" @click="action(row, 'finish')">结束</el-button><el-button size="small" type="danger" @click="action(row, 'cancel')">取消</el-button></template></el-table-column></el-table></div>
  </section>
</template>
