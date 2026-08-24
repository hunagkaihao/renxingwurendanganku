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
      path: 'archive',
      name: 'Archive',
      component: () => import('/@/views/archivehouse/archive/Archive.vue'),
      meta: {
        title: t('routes.basedata.archiveManagement'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'ph:file-archive-fill',
      },
    },
    {
        path: 'archivebox',
        name: 'Archivebox',
        component: () => import('/@/views/archivehouse/archivebox/ArchiveBox.vue'),
        meta: {
          title: t('routes.basedata.archiveboxManagement'),
          policy: 'WarehouseManagement.StorageBoxManagement',
          icon: 'ph:archive-bold',
        },
      },
  ],
};

export default basedata;
