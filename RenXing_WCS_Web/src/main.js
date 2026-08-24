import Vue from 'vue'
// import ElementUI from 'element-ui';
// import 'element-ui/lib/theme-chalk/index.css';
import App from './App.vue'
import Axios from './axios'
import HubConn from './hubConnection'
import VueRouter from 'vue-router'
import router from './router'

// Vue.use(ElementUI);
Vue.config.productionTip = false;
Vue.prototype.$axios = Axios;
Vue.prototype.$HubConn = HubConn;
Vue.use(VueRouter);
const originalPush = VueRouter.prototype.push;
//修改原型对象中的push方法
VueRouter.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err);
};
//修改原型对象中的replace方法
const originalReplace = VueRouter.prototype.replace;
VueRouter.prototype.replace = function replace(location) {
  return originalReplace.call(this, location).catch(err => err);
};

new Vue({
  render: h => h(App),
  router: router
}).$mount('#app')
