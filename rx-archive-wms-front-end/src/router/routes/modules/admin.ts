import type { AppRouteModule } from '/@/router/types';
import { LAYOUT } from '/@/router/constant';
import { t } from '/@/hooks/web/useI18n';

const admin: AppRouteModule = {
  path: '/admin',
  name: 'Admin',
  component: LAYOUT,
  //redirect: '/admin/abpUser',
  meta: {
    orderNo: 90,
    icon: 'ion:grid-outline',
    title: t('routes.admin.systemManagement'),
  },
  children: [
    // {
    //   path: 'book',
    //   name: 'Book',
    //   component: () => import('/@/views/admin/books/Book.vue'),
    //   meta: {
    //     title: t('routes.admin.bookManagement'),
    //     policy: 'System.BookManagement',
    //     icon: 'ant-design:skin-outlined',
    //   },
    // },
    {
      path: 'abpUser',
      name: 'AbpUser',
      component: () => import('/@/views/admin/users/AbpUser.vue'),
      meta: {
        title: t('routes.admin.userManagement'),
        policy: 'AbpIdentity.Users',
        icon: 'material-symbols:person-outline',
      },
    },
    {
      path: 'abpRole',
      name: 'AbpRole',
      component: () => import('/@/views/admin/roles/AbpRole.vue'),
      meta: {
        title: t('routes.admin.roleManagement'),
        policy: 'AbpIdentity.Roles',
        icon: 'eos-icons:role-binding',
      },
    },
    {
      path: 'settings',
      name: 'Settings',
      component: () => import('/@/views/admin/settings/Setting.vue'),
      meta: {
        title: t('routes.admin.settingManagement'),
        policy: 'System.Setting',
        icon: 'ant-design:unordered-list-outlined',
      },
    },
    // {
    //   path: 'abpAuditLogs',
    //   name: 'AuditLogs',
    //   component: () => import('/@/views/admin/auditLog/AuditLog.vue'),
    //   meta: {
    //     title: t('routes.admin.auditLog'),
    //     policy: 'System.AuditLog',
    //     icon: 'ant-design:snippets-twotone',
    //   },
    // },
    // {
    //   path: 'esLogs',
    //   name: 'ESLogs',
    //   component: () => import('/@/views/admin/elasticSearch/ElasticSearch.vue'),
    //   meta: {
    //     title: t('routes.admin.esLogs'),
    //     policy: 'System.ES',
    //     icon: 'ant-design:snippets-twotone',
    //   },
    // },
    // {
    //   path: 'dataDictionary',
    //   name: 'dataDictionary',
    //   component: () => import('/@/views/admin/dictionary/AbpDictionary.vue'),
    //   meta: {
    //     title: t('routes.admin.dictionaryManagement'),
    //     icon: 'ant-design:table-outlined',
    //     policy: 'System.DataDictionaryManagement',
    //   },
    // },
    // {
    //   path: 'files',
    //   name: 'files',
    //   component: () => import('/@/views/admin/files/File.vue'),
    //   meta: {
    //     title: t('routes.admin.fileNameManagement'),
    //     icon: 'ant-design:snippets-outlined',
    //     policy: 'System.FileManagement',
    //   },
    // },
    // {
    //   path: 'DownLog',
    //   name: 'DownLog',
    //   component: () => import('/@/views/admin/auditLog/DownLog.vue'),
    //   meta: {
    //     title: t('日志下载'),
    //     policy: 'WarehouseManagement.StockTaskManagement',
    //     icon: 'ant-design:file-search-outlined',
    //   },
    // },
  ],
};

export default admin;
