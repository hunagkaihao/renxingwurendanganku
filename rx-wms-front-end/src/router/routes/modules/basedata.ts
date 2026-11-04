import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const basedata: AppRouteModule = {
  path: '/basedata',
  name: 'Basedata',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 50,
    icon: 'ant-design:database-outlined',
    title: t('routes.basedata.archivedataManagement'),
  },
  children: [
    {
      path: 'materialtype',
      name: 'MaterialType',
      component: () => import('/@/views/materialhouse/materialtype/MaterialType.vue'),
      meta: {
        title: t('routes.basedata.archiveManagement'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'material-symbols:category-outline',
      },
    },
    {
        path: 'materialbox',
        name: 'Materialbox',
        component: () => import('/@/views/materialhouse/materialbox/MaterialBox.vue'),
        meta: {
          title: t('routes.basedata.archiveboxManagement'),
          policy: 'WarehouseManagement.StorageBoxManagement',
          icon: 'ph:archive-bold',
        },
      },
  ],
};

export default basedata;
