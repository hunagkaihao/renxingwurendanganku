import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const stockTask: AppRouteModule = {
  path: '/stockTask',
  name: 'StockTask',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 40,
    icon: 'ant-design:swap-outlined',
    title: t('routes.stockTask.stockTaskManagement'),
  },
  children: [
    // {
    //   path: 'bindBoxStock',
    //   name: 'BindBoxStock',
    //   component: () => import('/@/views/warehouse/stockTasks/BindBoxStock.vue'),
    //   meta: {
    //     title: t('档案入库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:select-outlined',
    //   },
    // },
    // {
    //   path: 'billBindBox',
    //   name: 'BillBindBox',
    //   component: () => import('/@/views/warehouse/stockTasks/BillBindBox.vue'),
    //   meta: {
    //     title: t('入库单据入库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:file-done-outlined',
    //   },
    // },
    // {
    //   path: 'incellList',
    //   name: 'IncellList',
    //   component: () => import('/@/views/warehouse/stockTasks/IncellList.vue'),
    //   meta: {
    //     title: t('入库单据管理'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:insert-row-below-outlined',
    //   },
    // },
    // {
    //   path: 'pickOut',
    //   name: 'PickOut',
    //   component: () => import('/@/views/warehouse/stockTasks/PickOut.vue'),
    //   meta: {
    //     title: t('档案出库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:export-outlined',
    //   },
    // },
    // {
    //   path: 'OrderOut',
    //   name: 'OrderOut',
    //   component: () => import('/@/views/warehouse/stockTasks/OrderOut.vue'),
    //   meta: {
    //     title: t('批次号出库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:file-done-outlined',
    //   },
    // },
    // {
    //   path: 'billOut',
    //   name: 'BillOut',
    //   component: () => import('/@/views/warehouse/stockTasks/BillOut.vue'),
    //   meta: {
    //     title: t('出库单据出库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:file-done-outlined',
    //   },
    // },
    // {
    //   path: 'outCellList',
    //   name: 'OutCellList',
    //   component: () => import('/@/views/warehouse/stockTasks/OutCellList.vue'),
    //   meta: {
    //     title: t('出库单据管理'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:insert-row-below-outlined',
    //   },
    // },
    // {
    //   path: 'emptyBoxOut',
    //   name: 'EmptyBoxOut',
    //   component: () => import('/@/views/warehouse/stockTasks/EmptyBoxOut.vue'),
    //   meta: {
    //     title: t('容器出库'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:skin-outlined',
    //   },
    // },
    // {
    //   path: 'outCellStock',
    //   name: 'OutCellStock',
    //   component: () => import('/@/views/warehouse/stockTasks/OutCellStock.vue'),
    //   meta: {
    //     title: t('出库确认'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:copyright-circle-outlined',
    //     ignoreKeepAlive: true, //忽略页面缓存，每次强制刷新
    //   },
    // },
    {
      path: 'stockTask',
      name: 'StockTask',
      component: () => import('/@/views/archivehouse/stock/StockTask.vue'),
      meta: {
        title: t('档案出入库管理'),
        policy: 'WarehouseManagement.StockTaskManagement',
        icon: 'ant-design:menu-outlined',
        ignoreKeepAlive: true, //忽略页面缓存，每次强制刷新
      },
    },
    {
      path: 'taskHis',
      name: 'TaskHis',
      component: () => import('/@/views/archivehouse/stock/StockTaskHis.vue'),
      meta: {
        title: t('出入库记录查询'),
        policy: 'WarehouseManagement.StockTaskManagement',
        icon: 'ant-design:file-search-outlined',
      },
    },
    // {
    //   path: 'stockHis',
    //   name: 'StockHis',
    //   component: () => import('/@/views/warehouse/taskHiss/StockHis.vue'),
    //   meta: {
    //     title: t('产品出入库记录'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:file-search-outlined',
    //   },
    // },
  ],
};

export default stockTask;
