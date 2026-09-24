import Vue from 'vue';
import App from './app.vue';
import iView from 'iview';
import {router} from './router/index';
import 'famfamfam-flags/dist/sprite/famfamfam-flags.css';
import './theme.less';
import Util from './lib/util';
import SignalRAspNetCoreHelper from './lib/SignalRAspNetCoreHelper';
Vue.use(iView);
import store from './store/index';
Vue.config.productionTip = false;
import { appRouters,otherRouters} from './router/router';
if(!abp.utils.getCookieValue('Abp.Localization.CultureName')){
  let language=navigator.language;
  abp.utils.setCookieValue('Abp.Localization.CultureName',language,new Date(new Date().getTime() + 5 * 365 * 86400000),abp.appPath);
}

// 在路由和页面创建前完成配置及会话初始化，避免旧接口 404 和会话读取竞态。
store.dispatch('session/init').then(()=>{
  new Vue({
    render: h => h(App),
    router:router,
    store:store,
    data: {
      currentPageName: ''
    },
    async mounted () {
      this.currentPageName = this.$route.name as string;
      // 恢复此前启动失败留下的空路由；仅在启动时处理，不影响运行中的关闭操作。
      this.$router.onReady(() => {
        if (this.$route.name === 'windowsclose') {
          this.$router.replace({ name: 'client' });
        }
      });
      if(!!this.$store.state.session.user&&this.$store.state.session.application.features['SignalR']){
        if (this.$store.state.session.application.features['SignalR.AspNetCore']) {
            SignalRAspNetCoreHelper.initSignalR();
        }
      }
      this.$store.commit('app/initCachepage');
      this.$store.commit('app/updateMenulist');
    },
    created () {
      let tagsList:Array<any> = [];
      appRouters.map((item) => {
          if (item.children.length <= 1) {
              tagsList.push(item.children[0]);
          } else {
              tagsList.push(...item.children);
          }
      });
      this.$store.commit('app/setTagsList', tagsList);
    }
  }).$mount('#app')
}) .catch( (error) => {
  console.log(error);
  alert("WMS后台服务器连接异常，请检查服务器");
  // 保留当前地址，服务恢复后刷新即可重新初始化，避免跳入没有组件的关闭路由。
})

