declare global {
    interface RouterMeta {
        title: string;
    }
    interface Router {
        path: string;
        name: string;
        icon?: string;
        permission?: string;
        meta?: RouterMeta;
        component: any;
        children?: Array<Router>;
    }
    interface System {
        import(request: string): Promise<any>
    }
    var System: System
}
import login from '../views/login.vue'
import home from '../views/home/home.vue'
import main from '../views/main.vue'

export const locking = {
    path: '/locking',
    name: 'locking',
    component: () => import('../components/lockscreen/components/locking-page.vue')
};
export const client = {
    path: '/client',
    name: 'client',
    meta: {
        title: 'client'
    },
    component: () => import('../views/client/portal.vue')
};
export const autol = {
    path: '/autol',
    name: 'autol',
    meta: {
        title: 'autol'
    },
    component: () => import('../views/client/autol.vue')
};
export const face = {
    path: '/face',
    name: 'face',
    meta: {
        title: 'face'
    },
    component: () => import('../views/client/locking-page.vue')
};
export const windowsclose = {
    path: '/windowsclose',
    name: 'windowsclose',
    meta: {
        title: 'windowsclose'
    }
};
export const opendoor = {
    path: '/opendoor',
    name: 'opendoor',
    meta: {
        title: 'opendoor'
    },
    component: () => import('../views/client/opendoor.vue')
};
export const faceverification = {
    path: '/faceverification',
    name: 'faceverification',
    meta: {
        title: 'faceverification'
    },
    component: () => import('../views/client/face.vue')
};
export const finger = {
    path: '/finger',
    name: 'finger',
    meta: {
        title: 'finger'
    },
    component: () => import('../views/client/finger.vue')
};
export const stockin = {
    path: '/stockin',
    name: 'stockin',
    meta: {
        title: '样品入库管理'
    },
    component: () => import('../views/business/stockin/stocktask.vue')
};
export const stockstep = {
    path: '/stockstep',
    name: 'stockstep',
    meta: {
        title: '留样'
    },
    component: () => import('../views/business/stockin/stockinstep.vue')
};
export const stockinstepctn = {
    path: '/stockinstepctn',
    name: 'stockinstepctn',
    meta: {
        title: '连续留样'
    },
    component: () => import('../views/business/stockin/stockinstepctn.vue')
};
export const stockinstepctn2 = {
    path: '/stockinstepctn2',
    name: 'stockinstepctn2',
    meta: {
        title: '连续留样'
    },
    component: () => import('../views/business/stockin/stockinstepctn2.vue')
};
export const stockoutstep = {
    path: '/stockoutstep',
    name: 'stockoutstep',
    meta: {
        title: '取样'
    },
    component: () => import('../views/business/stockout/stockoutstep.vue')
};
export const stockoutstepctn = {
    path: '/stockoutstepctn',
    name: 'stockoutstepctn',
    meta: {
        title: '连续取样'
    },
    component: () => import('../views/business/stockout/stockoutstepctn.vue')
};
export const stockoutstepctn2 = {
    path: '/stockoutstepctn2',
    name: 'stockoutstepctn2',
    meta: {
        title: '连续取样'
    },
    component: () => import('../views/business/stockout/stockoutstepctn2.vue')
};
export const stockout = {
    path: '/stockout',
    name: 'stockout',
    meta: {
        title: '样品借用管理'
    },
    component: () => import('../views/business/stockout/stocktask.vue')
};
export const batin = {
    path: '/batin',
    name: 'batin',
    meta: {
        title: '批量入库管理'
    },
    component: () => import('../views/business/batin/stocktask.vue')
};
export const userregister = {
    path: '/userregister',
    name: 'userregister',
    meta: {
        title: '用户特征数据'
    },
    component: () => import('../views/setting/user/userregister.vue')
};
export const checktask = {
    path: '/checktask',
    name: 'checktask',
    meta: {
        title: '盘点任务管理'
    },
    component: () => import('../views/business/check/stocktask.vue')
};
export const loginRouter: Router = {
    path: '/',
    name: 'login',
    meta: {
        title: '操作台首页'
    },
    component: () => import('../views/client/portal.vue')
};
export const otherRouters: Router = {
    path: '/main',
    name: 'main',
    permission: '',
    meta: { title: 'ManageMenu' },
    component: main,
    children: [
        { path: 'home', meta: { title: 'HomePage' }, name: 'home', component: () => import('../views/home/home.vue') }
    ]
}
export const appRouters: Array<Router> = [{
    path: '/setting',
    name: 'setting',
    permission: '',
    meta: { title: 'ManageMenu' },
    icon: '&#xe68a;',
    component: main,
    children: [
        { path: 'user', permission: 'Pages.Users', meta: { title: 'Users' }, name: 'user', component: () => import('../views/setting/user/user.vue') },
        { path: 'role', permission: 'Pages.Roles', meta: { title: 'Roles' }, name: 'role', component: () => import('../views/setting/role/role.vue') },
        { path: 'tenant', permission: 'Pages.Tenants', meta: { title: 'Tenants' }, name: 'tenant', component: () => import('../views/setting/tenant/tenant.vue') }
    ]
},
{
    path: '/basedata',
    name: 'basedata',
    permission: '',
    meta: { title: '样品基础数据' },
    icon: '&#xe68a;',
    component: main,
    children: [
        { path: 'archivebox', permission: 'Pages.Users', meta: { title: '档案盒管理' }, name: 'archivebox', component: () => import('../views/basedata/archivebox/archivebox.vue') },
        { path: 'archive', permission: 'Pages.Users', meta: { title: '档案文件管理' }, name: 'archive', component: () => import('../views/basedata/archive/archive.vue') },
        { path: 'archiveboxlist', permission: 'Pages.Users', meta: { title: '入库文件管理' }, name: 'archiveboxlist', component: () => import('../views/basedata/archiveboxlist/archiveboxlist.vue') },
        { path: 'cell', permission: 'Pages.Users', meta: { title: '库位管理' }, name: 'cell', component: () => import('../views/basedata/cell/cell.vue') },
        { path: 'rfid', permission: 'Pages.Users', meta: { title: '标签管理' }, name: 'rfid', component: () => import('../views/basedata/rfid/rfid.vue') }
    ]
},
{
    path: '/business',
    name: 'business',
    permission: '',
    meta: { title: '出入库管理' },
    icon: '&#xe68a;',
    component: main,
    children: [
        { path: 'stocktask', permission: 'Pages.Users', meta: { title: '档案入库管理' }, name: 'stocktask', component: () => import('../views/business/stockin/stocktask.vue') },
        { path: 'stockout', permission: 'Pages.Users', meta: { title: '档案出库管理' }, name: 'stockout', component: () => import('../views/business/stockout/stocktask.vue') },
        { path: 'batin', permission: 'Pages.Users', meta: { title: '批量入库管理' }, name: 'batin', component: () => import('../views/business/batin/stocktask.vue') },
        { path: 'checktask', permission: 'Pages.Users', meta: { title: '盘点任务管理' }, name: 'checktask', component: () => import('../views/business/check/stocktask.vue') },
        { path: 'record', permission: 'Pages.Users', meta: { title: '出入库查询' }, name: 'record', component: () => import('../views/business/record/record.vue') },
        { path: 'check', permission: 'Pages.Users', meta: { title: '盘点管理' }, name: 'check', component: () => import('../views/business/check/check.vue') },
        { path: 'checkhis', permission: 'Pages.Users', meta: { title: '盘点记录管理' }, name: 'checkhis', component: () => import('../views/business/checkhis/checkhis.vue') },
    ]
}]
export const routers = [
    loginRouter,
    locking,
    client,
    autol,
    face,
    faceverification,
    finger,
    windowsclose,
    opendoor,
    stockin,
    stockstep,
    stockinstepctn,
    stockinstepctn2,
    stockoutstep,
    stockoutstepctn,
    stockoutstepctn2,
    stockout,
    batin,
    userregister,
    checktask,
    ...appRouters,
    otherRouters
];
