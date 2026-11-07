<template>
  <div class="sample-portal">
    <header class="portal-header">
      <div class="portal-brand">
        <img v-if="logoUrl" :src="logoUrl" class="portal-logo" alt="系统标识">
        <span v-else class="portal-brand-icon"><Icon type="md-cube" size="28" /></span>
        <div><strong>智慧无人库</strong><span class="portal-brand-caption">样品自助服务终端</span></div>
      </div>
      <nav class="portal-tools" aria-label="终端工具">
        <button type="button" class="portal-status" @click="openStatus"
          :class="{ 'is-offline': !serviceConnect }">
          <span class="portal-status-dot"></span>
          {{ wmsStatusPending ? '正在检查 WMS' : (serviceConnect ? 'WMS 已连接' : 'WMS 待检查') }}
          <Icon type="ios-arrow-forward" size="16" />
        </button>
        <button type="button" class="portal-tool" @click="userRegister" title="人员注册">
          <Icon type="ios-person-outline" size="22" /><span>人员注册</span>
        </button>
        <button type="button" class="portal-tool" @click="logout" title="退出操作台">
          <Icon type="ios-power" size="21" /><span>退出</span>
        </button>
      </nav>
    </header>

    <main class="portal-workspace">
      <section class="portal-intro" aria-labelledby="portal-title">
        <span class="portal-eyebrow">便捷存取 · 有序管理</span>
        <h1 id="portal-title">欢迎使用自助操作台</h1>
        <p>请选择您需要办理的业务</p>
      </section>

      <section class="portal-actions" aria-label="样品存取">
        <button type="button" class="portal-action portal-action-in" @click="stockincnt"
          :aria-describedby="wmsOnly ? 'portal-device-notice' : undefined">
          <span class="portal-card-top">
            <span class="portal-card-symbol"><Icon type="md-archive" size="44" /></span>
            <span class="portal-card-number" aria-hidden="true">01</span>
          </span>
          <span class="portal-card-title">留样</span>
          <span class="portal-card-description">样品入库 · 有序留存</span>
          <span class="portal-card-bottom">
            <span>登录后进入留样流程</span>
            <span v-if="!wmsOnly && taskInCount > 0" class="portal-task-count">{{ taskInCount }} 项待处理</span>
            <span class="portal-card-arrow"><Icon type="ios-arrow-forward" size="24" /></span>
          </span>
        </button>
        <button type="button" class="portal-action portal-action-out" @click="stockoutcnt"
          :aria-describedby="wmsOnly ? 'portal-device-notice' : undefined">
          <span class="portal-card-top">
            <span class="portal-card-symbol"><Icon type="md-filing" size="44" /></span>
            <span class="portal-card-number" aria-hidden="true">02</span>
          </span>
          <span class="portal-card-title">取样</span>
          <span class="portal-card-description">样品出库 · 便捷取用</span>
          <span class="portal-card-bottom">
            <span>登录后进入取样流程</span>
            <span v-if="!wmsOnly && taskOutCount > 0" class="portal-task-count">{{ taskOutCount }} 项待处理</span>
            <span class="portal-card-arrow"><Icon type="ios-arrow-forward" size="24" /></span>
          </span>
        </button>
      </section>

      <div v-if="wmsOnly" id="portal-device-notice" class="portal-notice" role="status">
        <span class="portal-notice-icon"><Icon type="ios-information-circle-outline" size="24" /></span>
        <div><strong>设备连接待确认</strong><p>可先选择业务并登录，办理留样、取样前请确认设备服务已就绪。</p></div>
        <button type="button" @click="openStatus">查看状态 <Icon type="ios-arrow-forward" /></button>
      </div>
      <p v-else class="portal-guide"><Icon type="ios-hand-outline" size="20" /> 点击业务卡片，按照页面提示完成操作</p>
    </main>

    <footer class="portal-footer"><span>智慧无人库 · 自助操作台</span><span>请按页面指引操作</span></footer>
    <Modal v-model="statusModalShow" title="服务连接状态" :footer-hide="true">
      <p>{{ serviceConnect ? 'WMS 服务已连接。' : 'WMS 服务暂时无法访问，请检查服务连接。' }}</p>
      <p style="margin-top:12px">设备运行状态请以业务任务反馈及现场状态为准。</p>
      <Button style="margin-top:16px" :loading="wmsStatusPending" @click="validator">重新检查</Button>
    </Modal>
    <userlogin v-model="createModalShow"></userlogin>
  </div>
</template>
<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import iView from 'iview';
import Cookies from 'js-cookie';
import AbpBase from '../../lib/abpbase'
import userlogin from './userlogin.vue'
import AppConsts from '../../lib/appconst';

  @Component({
    components:{userlogin}
  })
export default class Login extends AbpBase {
  /** 仅 WMS 模式不调用旧任务统计及设备服务。 */
  wmsOnly:boolean = AppConsts.wmsOnly;
  /** 仅开发环境开放无设备的页面调试，仍通过 WMS 账号密码验证。 */
  wmsPageDebug:boolean = AppConsts.wmsOnly && process.env.NODE_ENV === 'development';
  /** 避免慢请求导致首页状态轮询重叠。 */
  wmsStatusPending:boolean = false;
  name:string= 'lockScreen';
  lockScreenSize:number=0;
  showUnlock:boolean=false;
  intervalId:number=0;
  statusModalShow:boolean=false;
  serviceConnect:boolean=false;//首次检查通过前不显示已连接
  wcsStatusFlag:boolean=true;//WCS服务器连接状态
  mjgStatusFlag:boolean=true;//密集架连接状态
  scadaStatusFlag:boolean=true;//SCADA连接状态
  createModalShow:boolean=false;//登录页面
  plcStatusFlag:boolean=true;//PLC及急停连接状态
  neverTip:boolean = false;
  @Prop({type:Boolean,default:false}) value:boolean;
 //获取服务器状态
 getserviceStatus()
 {

 }
 //获取logoUrl
 get logoUrl(){
    return abp.setting.values.ClientLogoPath
 }
 //获取取档口数量
 get CabinetQty(){
    return abp.setting.values.CabinetQty
 }
 //获取验证身份方式
 get ClientVerifyMethod(){
    return abp.setting.values.ClientVerifyMethod
 }

 //获取服务的状态标识
 GetServicesFlag(flag:boolean)
 {
   this.serviceConnect=flag;
 }
//不再提示状态标识
 GetneverTip(flag:boolean){
   this.neverTip = flag
 }

 openStatus()
 {
  

    this.statusModalShow=true;
   
 }
  /** 人员注册始终要求本轮管理员账号密码登录，设备是否可用由录入接口检查。 */
  userRegister()
  {
      Cookies.set('last_page_name', 'userregister');
      Cookies.set('facing', '0');
      Cookies.set('userId', '0');
      this.$router.push({ name: 'terminal-login', query: { next: 'userregister' } });
 }
 stockin() {
  //  if(this.taskInCount==0)
  //  {
  //    alert("当前无待处理任务");
  //    return;
  //  }

      let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      if(lockScreenBack){
      lockScreenBack.style.transition = 'all 1s';
      lockScreenBack.style.zIndex = "10000";
      lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      Cookies.set('last_page_name', 'stockin');
      //console.log('lock protal'+name);
      Cookies.set('facing', '1')
     Cookies.set('userId', '0')
      //console.log('protal'+Cookies.get('facing'));
      if(this.ClientVerifyMethod == "FaceAndVein" || this.ClientVerifyMethod == "FaceOnly"){
          setTimeout(() => {
              lockScreenBack.style.transition = 'all 0s';
              this.$router.push({
                  name: 'finger'
              });
          }, 800);
        }else{
          this.createModalShow=true;
        }
      //Cookies.set('facing', '1');
      }
  }
  /** 点击留样后进入统一登录页，认证后继续原柜门数量对应的页面。 */
  stockincnt() { this.openSampleLogin('in'); }

  /** 记录本次业务目标，并始终要求本轮人工登录。 */
  openSampleLogin(direction: string) {
    const destination = direction === 'in'
      ? (this.CabinetQty === '5' ? 'stockinstepctn2' : 'stockinstepctn')
      : (this.CabinetQty === '5' ? 'stockoutstepctn2' : 'stockoutstepctn');
    Cookies.set('last_page_name', destination);
    Cookies.set('facing', '0');
    Cookies.set('userId', '0');
    this.$router.push({ name: 'terminal-login', query: { next: destination } });
  }
     stockout() {
   if(this.taskOutCount==0)
   {
     alert("当前无待处理任务");
     return;
   }
      let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      if(lockScreenBack){
      lockScreenBack.style.transition = 'all 1s';
      lockScreenBack.style.zIndex = "10000";
      lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      Cookies.set('last_page_name', 'stockoutstep');
      //console.log('lock'+name);
      Cookies.set('facing', '1')
     Cookies.set('userId', '0')
      //console.log(Cookies.get('facing'));
      setTimeout(() => {
          lockScreenBack.style.transition = 'all 0s';
          this.$router.push({
              name: 'finger'
          });
      }, 800);
      //Cookies.set('facing', '1');
      }
  }
  /** 点击取样后进入统一登录页，认证后继续原取样流程。 */
  stockoutcnt() { this.openSampleLogin('out'); }
    batin() {
    if(this.batInCount!=0)
   {
     alert("当前无待处理任务");
     return;
   }
      let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      if(lockScreenBack){
      lockScreenBack.style.transition = 'all 1s';
      lockScreenBack.style.zIndex = "10000";
      lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      //Cookies.set('last_page_name', 'batin');
      Cookies.set('last_page_name', 'stockstep');
      //console.log('lock'+name);
      Cookies.set('facing', '1')
     Cookies.set('userId', '0')
      //console.log(Cookies.get('facing'));
      setTimeout(() => {
          lockScreenBack.style.transition = 'all 0s';
          this.$router.push({
              name: 'finger'
          });
      }, 500);
      //Cookies.set('facing', '1');
      }
  }
   checktask() {
    if(this.checkOutCount==0)
   {
     alert("当前无待处理任务");
     return;
   }
      let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      if(lockScreenBack){
      lockScreenBack.style.transition = 'all 1s';
      lockScreenBack.style.zIndex = "10000";
      lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      Cookies.set('last_page_name', 'checktask');
      //console.log('lock'+name);
      Cookies.set('facing', '1')
     Cookies.set('userId', '0')
      //console.log(Cookies.get('facing'));
      setTimeout(() => {
          lockScreenBack.style.transition = 'all 0s';
          this.$router.push({
              name: 'finger'
          });
      }, 800);
      //Cookies.set('facing', '1');
      }
  }
  mounted () {
      let lockdiv = document.createElement('div');
      lockdiv.setAttribute('id', 'lock_screen_back');
      lockdiv.setAttribute('class', 'lock-screen-back');
      document.body.appendChild(lockdiv);
      let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      let x = document.body.clientWidth;
      let y = document.body.clientHeight;
      let r = Math.sqrt(x * x + y * y).toString();
      let size = parseInt(r);
      this.lockScreenSize = size;
      window.addEventListener('resize', () => {
          let x = document.body.clientWidth;
          let y = document.body.clientHeight;
          let r = Math.sqrt(x * x + y * y).toString();
          let size = parseInt(r);
          this.lockScreenSize = size;
          lockScreenBack.style.transition = 'all 0s';
          lockScreenBack.style.width = lockScreenBack.style.height = size + 'px';
      });
      lockScreenBack.style.width = lockScreenBack.style.height = size + 'px';
  }
  logout()
  {   
          this.$router.push({
              name: 'windowsclose'
          });

 }
  usersync()
  {
    try {
       // @ts-ignore：无法被执行的代码的错误
        veinjs.usersync();
      
    } catch (error) {
      alert("同步异常");
    }
  }
get taskInCount(){
  // 首页尚未验证本轮身份，不展示旧接口缓存的业务数量。
  return null;
}
get taskOutCount(){
  return null;
}
  get batInCount(){
  return this.$store.state.app.batInCount;
}
  get checkOutCount(){
  return this.$store.state.app.checkOutCount;
}
/** 只检查当前配置的 WMS，首页不再探测旧地址或未验证的设备状态。 */
validator () {
  if (this.wmsStatusPending) return;
  this.wmsStatusPending = true;
  this.$store.dispatch('app/getWMSStatus')
    .then(() => { this.serviceConnect = true; })
    .catch(() => { this.serviceConnect = false; })
    .then(() => { this.wmsStatusPending = false; });
}
  openStatus2 () {
    if(!this.neverTip){
      this.openStatus();
    }
  }
stockinaa () {
  this.$Message.info('This is a info tip');
}

created(){
console.log(abp.localization.currentLanguage.name);
  this.validator();

   this.intervalId= setInterval(this.validator, 10000);
}
        async destroyed()
      {
          window.clearInterval(this.intervalId);
          // window.clearInterval(this.intervalId);
      }
}
</script>
<style scoped lang="less">
@import "./portal.less";
</style>
