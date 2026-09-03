import { createRouter, createWebHistory } from 'vue-router';

export default createRouter({ history: createWebHistory(), routes: [
  { path: '/', redirect: '/orders' },
  { path: '/orders', component: () => import('../views/OrderListView.vue') },
  { path: '/orders/:orderCode', component: () => import('../views/OrderDetailView.vue') },
  { path: '/tags', component: () => import('../views/TagMonitorView.vue') },
  { path: '/logs', component: () => import('../views/LogView.vue') }
] });
