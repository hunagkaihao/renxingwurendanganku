import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const basedata: AppRouteModule = {
  path: '/warehousedata',
  name: 'Warehousedata',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 50,
    icon: 'material-symbols:database-outline',
    title: t('库房基础数据'),
  },
  children: [
    {
      path: 'cell',
      name: 'Cell',
      component: () => import('/@/views/archivehouse/cell/Cell.vue'),
      meta: {
        title: t('库位管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'fluent:layout-cell-four-focus-top-left-16-filled',
      },
    },
    // {
    //   path: 'station',
    //   name: 'Station',
    //   component: () => import('/@/views/archivehouse/cell/Station.vue'),
    //   meta: {
    //     title: t('柜格状态'),
    //     policy: 'WarehouseManagement.StorageBoxManagement',
    //     icon: 'ant-design:file-search-outlined',
    //   },
    // },
    {
      path: 'archiveType',
      name: 'ArchiveType',
      component: () => import('/@/views/archivehouse/archivetype/ArchiveType.vue'),
      meta: {
        title: t('档案类型'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'fluent:archive-16-regular',
      },
    },
    {
      path: 'rfid',
      name: 'Rfid',
      component: () => import('/@/views/archivehouse/rfid/Rfid.vue'),
      meta: {
        title: t('标签管理'),
        policy: 'WarehouseManagement.StorageBoxManagement',
        icon: 'material-symbols:barcode',
      },
    },
    // {
    //   path: 'batch',
    //   name: 'Batch',
    //   component: () => import('/@/views/archivehouse/Batch/Batch.vue'),
    //   meta: {
    //     title: t('批量管理'),
    //     policy: 'WarehouseManagement.StorageBoxManagement',
    //     icon: 'ant-design:file-search-outlined',
    //   },
    // },
    
  ],
};

export default basedata;
