import VueRouter from 'vue-router'
import LogPage from './pages/LogPage.vue'
import TagMonitorPage from './pages/TagMonitorPage.vue'
import OrderListMonitorPage from './pages/OrderListMonitorPage.vue'
import OneOrderMoniterPage from './pages/OneOrderMoniterPage.vue'

export default new VueRouter({
    routes:[
        {
            path:'/orderListMonitor',
            component:OrderListMonitorPage,
        },
        {
            path:'/oneOrderMonitor',
            component:OneOrderMoniterPage,
        },
        {
            path:'/log',
            component:LogPage
        },
        {
            path:'/tagMonitor',
            component:TagMonitorPage
        }
    ]
});