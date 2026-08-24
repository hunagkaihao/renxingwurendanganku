import type { AppRouteRecordRaw, AppRouteModule } from '/@/router/types';

import { PAGE_NOT_FOUND_ROUTE, REDIRECT_ROUTE } from '/@/router/routes/basic';

import { mainOutRoutes } from './mainOut';
import { PageEnum } from '/@/enums/pageEnum';
import { t } from '/@/hooks/web/useI18n';

const modules = import.meta.globEager('./modules/**/*.ts');

const routeModuleList: AppRouteModule[] = [];

Object.keys(modules).forEach((key) => {
  const mod = modules[key].default || {};
  const modList = Array.isArray(mod) ? [...mod] : [mod];
  routeModuleList.push(...modList);
});

export const asyncRoutes = [PAGE_NOT_FOUND_ROUTE, ...routeModuleList];

export const RootRoute: AppRouteRecordRaw = {
  path: '/',
  name: 'Root',
  redirect: PageEnum.BASE_HOME,
  meta: {
    title: 'Root',
  },
};

export const LoginRoute: AppRouteRecordRaw = {
  path: '/login',
  name: 'Login',
  component: () => import('/@/views/sys/login/Login.vue'),
  meta: {
    title: t('routes.basic.login'),
  },
};
//新增移动端登录
export const MobileLoginRoute: AppRouteRecordRaw = {
  path: '/mobilelogin',
  name: 'MobileLogin',
  component: () => import('/@/views/mobile/login/Login.vue'),
  meta: {
    title: t('routes.basic.login'),
    ignoreAuth: true,
  },
};
//新增移动端主页
export const MobileHomeRoute: AppRouteRecordRaw = {
  path: '/mobilehome',
  name: 'MobileHome',
  component: () => import('/@/views/mobile/home/home.vue'),
  meta: {
    title: t('主页'),
  },
};
//看板
export const Board: AppRouteRecordRaw = {
  path: '/board',
  name: 'Board',
  component: () => import('/@/views/archivehouse/board/Board.vue'),
  meta: {
    title: t('看板'),
  },
};
//看板
export const Screen: AppRouteRecordRaw = {
  path: '/screen',
  name: 'Screen',
  component: () => import('/@/views/archivehouse/board/Index.vue'),
  meta: {
    title: t('看板1'),
  },
};







// Basic routing without permission
export const basicRoutes = [
  LoginRoute,
  MobileLoginRoute,
  MobileHomeRoute,
  RootRoute,
  Board,
  Screen,
  ...mainOutRoutes,
  REDIRECT_ROUTE,
  PAGE_NOT_FOUND_ROUTE,
];
