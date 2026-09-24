import Vue from 'vue';
import VueRouter from 'vue-router';
import {routers} from './router';
import iView from 'iview';
import Util from '../lib/util';
import Cookies from 'js-cookie'
import { appRouters,otherRouters} from './router'

Vue.use(VueRouter);

const RouterConfig = {
    // mode: 'history',
    routes: routers
};

export const router = new VueRouter(RouterConfig);

router.beforeEach((to, from, next) => {
    iView.LoadingBar.start();
    Util.title(to.meta.title);
    console.log('route'+Cookies.get('facing')+Cookies.get('locking')+to.name+from.name);
    if (Cookies.get('locking') === '1' && to.name !== 'locking') {
        next({
            replace: true,
            name: 'locking'
        });
    }else if (Cookies.get('locking') === '0' && to.name === 'locking') {
        next(false);
    }
    else if (Cookies.get('facing') === '1' && to.name !== 'face') {
        next({
            replace: true,
            name: 'face'
        });
    }else if (Cookies.get('facing') === '0' && to.name === 'face') {
        console.log(Cookies.get('facing'));
        console.log(Util.abp.session.userId);
        if (Cookies.get('userId') === '0') {
            Cookies.set('facing','1'); 
            Util.title(to.meta.title);
            next({
                replace: true,
                name: 'login'
            });
        }
        else
        {
            next(false);
            //  next({
            // name: 'stockin'
            //  });
        }
        // next({
        //     name: 'stockin'
        // });
        //next(false);
    }
    else if (to.name === 'client') {
        next();
    }
    else if (to.name === 'autol') {
        next();
    }
    else if (to.name === 'faceverification') {
        next();
    }
    else if (to.name === 'finger') {
        next();
    }
    else if (to.name === 'windowsclose') {
        next();
    }
    else if (to.name === 'opendoor') {
        next();
    }
    else if (to.name === 'userregister') {
        next();
    }
    else if (to.name === 'stockin') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount stockin 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount stockin 222');
            next({
                name: 'login'
            });
        }
    }
    else if (to.name === 'stockstep') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount stockstep 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount stockstep 222');
            next({
                name: 'login'
            });
        }
    }
    else if (to.name === 'stockinstepctn') {
        next();
    }else if (to.name === 'stockinstepctn2') {
        next();
    }
    else if (to.name === 'stockoutstep') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount stockoutstep 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount stockoutstep 222');
            next({
                name: 'login'
            });
        }
    }
    else if (to.name === 'stockoutstepctn') {
        next();
    }
    else if (to.name === 'stockoutstepctn2') {
        next();
    }
    else if (to.name === 'stockout') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount stockout 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount stockout 222');
            next({
                name: 'login'
            });
        }
    }
    else if (to.name === 'batin') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount batin 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount batin 222');
            next({
                name: 'login'
            });
        }
    }
    else if (to.name === 'checktask') {
        if(Cookies.get('userId') !== '0')
        {
            console.log('rount checktask 111');
            next();
        }
        else
        {
            console.log(Cookies.get('userId'));
            console.log('rount checktask 222');
            next({
                name: 'login'
            });
        }
    }
    else if ( to.name === 'face') {
        console.log(111);
        console.log(Cookies.get('facing'));
        console.log(Util.abp.session.userId);
        next();
    }
    else {
        if (!Util.abp.session.userId&& to.name !== 'login') {
            next({
                name: 'login'
            });
        } else if (!!Util.abp.session.userId && to.name === 'login') {
            Util.title(to.meta.title);
            next({
                name: 'home'
            });
        } else {
            const curRouterObj = Util.getRouterObjByName([otherRouters, ...appRouters], to.name);
            if (curRouterObj && curRouterObj.permission) {
                if (window.abp.auth.hasPermission(curRouterObj.permission)) {
                    Util.toDefaultPage([otherRouters, ...appRouters], to.name, router, next);
                } else {
                    next({
                        replace: true,
                        name: 'error-403'
                    });
                }
            } else {
                Util.toDefaultPage([...routers], to.name, router, next);
            }
        }
    }
});
router.afterEach((to) => {
    const app = router.app;
    if (app) {
        // 首次导航发生在 beforeCreate 中，等待 Vuex 注入及 created 中的页签初始化完成。
        app.$nextTick(() => {
            // 启动失败或连续重定向时，不更新尚未就绪或已经离开的页面。
            if (app.$store && router.currentRoute === to) {
                Util.openNewPage(app, to.name, to.params, to.query);
            }
        });
    }
    iView.LoadingBar.finish();
    window.scrollTo(0, 0);
});
