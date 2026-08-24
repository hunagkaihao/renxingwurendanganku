import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const check: AppRouteModule = {
  path: '/check',
  name: 'Check',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 60,
    icon: 'material-symbols:inventory-rounded',
    title: t('盘点管理'),
  },
  children: [
    {
      path: 'check',
      name: 'Check',
      component: () => import('/@/views/archivehouse/check/Check.vue'),
      meta: {
        title: t('盘点计划管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'material-symbols:inventory-rounded',
      },
    },
    {
      path: 'checktask',
      name: 'Checktask',
      component: () => import('/@/views/archivehouse/check/Checktask.vue'),
      meta: {
        title: t('盘点任务管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'ant-design:file-search-outlined',
      },
    },
    {
      path: 'checkHis',
      name: 'CheckHis',
      component: () => import('/@/views/archivehouse/check/CheckHis.vue'),
      meta: {
        title: t('盘点结果管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'ic:twotone-inventory',
      },
    },
  ],
};

export default check;
