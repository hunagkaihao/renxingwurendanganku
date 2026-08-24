import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const plan: AppRouteModule = {
  path: '/plan',
  name: 'Plan',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 70,
    icon: 'material-symbols:batch-prediction-sharp',
    title: t('批量管理'),
  },
  children: [
    {
      path: 'plan',
      name: 'plan',
      component: () => import('/@/views/archivehouse/plan/plan.vue'),
      meta: {
        title: t('批量计划管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'material-symbols:inventory-rounded',
      },
    },
    // {
    //   path: 'planHis',
    //   name: 'planHis',
    //   component: () => import('/@/views/archivehouse/plan/planHis.vue'),
    //   meta: {
    //     title: t('盘点结果管理'),
    //     policy: 'WarehouseManagement.StorageBoxManagement',
    //     icon: 'ic:twotone-inventory',
    //   },
    // },
  ],
};

export default plan;
