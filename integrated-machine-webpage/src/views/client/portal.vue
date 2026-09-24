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
          :class="{ 'is-offline': !serviceConnect || (!wmsOnly && !(wcsStatusFlag && mjgStatusFlag && scadaStatusFlag && plcStatusFlag)) }">
          <span class="portal-status-dot"></span>
          {{ serviceConnect && (wmsOnly || (wcsStatusFlag && mjgStatusFlag && scadaStatusFlag && plcStatusFlag)) ? (wmsOnly ? 'WMS 已连接' : '服务正常') : '服务待检查' }}
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
            <span>{{ wmsOnly ? '设备待接入' : '进入留样流程' }}</span>
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
            <span>{{ wmsOnly ? '设备待接入' : '进入取样流程' }}</span>
            <span v-if="!wmsOnly && taskOutCount > 0" class="portal-task-count">{{ taskOutCount }} 项待处理</span>
            <span class="portal-card-arrow"><Icon type="ios-arrow-forward" size="24" /></span>
          </span>
        </button>
      </section>

      <div v-if="wmsOnly" id="portal-device-notice" class="portal-notice" role="status">
        <span class="portal-notice-icon"><Icon type="ios-information-circle-outline" size="24" /></span>
        <div><strong>设备尚未接入</strong><p>当前可查看 WMS 连接状态，设备接入后可办理留样、取样业务。</p></div>
        <button type="button" @click="openStatus">查看状态 <Icon type="ios-arrow-forward" /></button>
      </div>
      <p v-else class="portal-guide"><Icon type="ios-hand-outline" size="20" /> 点击业务卡片，按照页面提示完成操作</p>
    </main>

    <footer class="portal-footer"><span>智慧无人库 · 自助操作台</span><span>请按页面指引操作</span></footer>
    <serviceStatus v-model="statusModalShow" :servicesFlag="serviceConnect" @save-success="GetServicesFlag" @never-tip="GetneverTip"></serviceStatus>
    <userlogin v-model="createModalShow"></userlogin>
  </div>
</template>
<script lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import iView from 'iview';
import Cookies from 'js-cookie';
import ServiceStatus from './serviceStatus.vue'
import axios from 'axios';
import AbpBase from '../../lib/abpbase'
import userlogin from './userlogin.vue'
import AppConsts from '../../lib/appconst';

  @Component({
    components:{ServiceStatus,userlogin}
  })
export default class Login extends AbpBase {
  /** 仅 WMS 模式不调用旧任务统计及设备服务。 */
  wmsOnly:boolean = AppConsts.wmsOnly;
  /** 避免慢请求导致首页状态轮询重叠。 */
  wmsStatusPending:boolean = false;
  name:string= 'lockScreen';
  lockScreenSize:number=0;
  showUnlock:boolean=false;
  intervalId:number=0;
  statusModalShow:boolean=false;
  serviceConnect:boolean=true;//服务器连接状态
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
  userRegister()
  {
    if (this.wmsOnly) {
      this.$Message.info('设备服务未接入，暂不支持特征注册');
      return;
    }
    //   let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
    //   if(lockScreenBack){
    //   lockScreenBack.style.transition = 'all 1s';
    //   lockScreenBack.style.zIndex = "10000";
    //   lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
    //   this.showUnlock = true;
    //   let name=this.$route.name?this.$route.name:'';
    //   Cookies.set('last_page_name', 'userregister');
    //   //console.log('lock'+name);
    //   Cookies.set('facing', '0')
    //  Cookies.set('userId', '0')
    //   //console.log(Cookies.get('facing'));
    //   setTimeout(() => {
    //       lockScreenBack.style.transition = 'all 0s';
    //       this.$router.push({
    //           name: 'opendoor'
    //       });
    //   }, 500);
    //   //Cookies.set('facing', '1');
    //   }
          // this.$router.push({
          //     name: 'userregister'
          // });
      Cookies.set('last_page_name', 'userregister');
      this.createModalShow=true;

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
     stockincnt() {
      if (this.wmsOnly) {
        this.$Message.info('设备服务未接入，暂不支持存档操作');
        return;
      }
                   // bound.aa();//用于和我inform后台进行交互
  //  if(this.taskInCount==0)
  //  {
  //    //alert("当前无待处理任务");
  //    this.$Message.info('当前无待处理任务');
  //    return;
  //  }
      //let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      //if(lockScreenBack){
      // lockScreenBack.style.transition = 'all 1s';
      // lockScreenBack.style.zIndex = "10000";
      // lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      //this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      //选择展示柜门数
      if(this.CabinetQty == "5"){
      Cookies.set('last_page_name', 'stockinstepctn2');
      }
      else{
      Cookies.set('last_page_name', 'stockinstepctn');
      }
      if(this.ClientVerifyMethod == "FaceAndVein" || this.ClientVerifyMethod == "FaceOnly"){
      
          //console.log('lock'+name);
          Cookies.set('facing', '0')
          Cookies.set('userId', '0')
          //console.log(Cookies.get('facing'));
          setTimeout(() => {
              //lockScreenBack.style.transition = 'all 0s';
              this.$router.push({
                  name: 'faceverification'
              });
          }, 500);
        }else{
          this.createModalShow=true;
        }
      //Cookies.set('facing', '1');
      //}
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
      stockoutcnt() {
      if (this.wmsOnly) {
        this.$Message.info('设备服务未接入，暂不支持取档操作');
        return;
      }
  //            if(this.taskOutCount==0)
  //  {
  //    //alert("当前无待处理任务");
  //   this.$Message.info('当前无待处理任务');
  //    return;
  //  }
      //let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
      //if(lockScreenBack){
      // lockScreenBack.style.transition = 'all 1s';
      // lockScreenBack.style.zIndex = "10000";
      // lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
      this.showUnlock = true;
      let name=this.$route.name?this.$route.name:'';
      //选择展示柜门数
      if(this.CabinetQty == "5"){
      Cookies.set('last_page_name', 'stockoutstepctn2');
      }
      else{
        Cookies.set('last_page_name', 'stockoutstepctn');
      }
      //console.log('lock'+name);
      Cookies.set('facing', '0')
      Cookies.set('userId', '0')

      if(this.ClientVerifyMethod == "FaceAndVein" || this.ClientVerifyMethod == "FaceOnly"){
          setTimeout(() => {

              this.$router.push({
                  name: 'faceverification'
              });
          }, 500);
        }else{
          this.createModalShow=true;
        }

  }
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
  if (this.wmsOnly) return null;
  return this.$store.state.app.taskInCount;
}
get taskOutCount(){
  if (this.wmsOnly) return null;
  //console.log(this.$store.state.app.taskOutCount)
  return this.$store.state.app.taskOutCount;
}
  get batInCount(){
  return this.$store.state.app.batInCount;
}
  get checkOutCount(){
  return this.$store.state.app.checkOutCount;
}
validator () {
    if (this.wmsOnly) {
      if (this.wmsStatusPending) return;
      this.wmsStatusPending = true;
      this.$store.dispatch('app/getWMSStatus')
        .then(() => { this.serviceConnect = true; })
        .catch(() => { this.serviceConnect = false; })
        .then(() => { this.wmsStatusPending = false; });
      return;
    }
      // await this.$store.dispatch({
      //     type:'app/faceinfo',
      //      data: {
      //           "RequestValue":  2818,
      //           "pass":  "123456"
      //           }
      // }) 

        this.$store.dispatch({
          type:'app/getTaskCt'
        }).then( async (response) => {
          this.serviceConnect=true;
                      })
                      .catch( (error) => {
                          // console.log(error);
                          this.serviceConnect=false;
                          this.openStatus2();
                          alert("WMS后台服务器连接异常，请检查服务器");
                          //语音报警
                          try{
                            // @ts-ignore:无法被执行的代码的错误
                            bound.speakmsg("后台服务器异常")
                          }catch(error){

                          }
                      }); 
             //alert(1);
      if(!this.statusModalShow)
      {
                //检查WCS服务状态
        axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetWCSServerStatus')
        .then(async (response) => {
          // this.serviceConnect=false;
          // console.log(response);
          if (response.data.result) {
            this.wcsStatusFlag=true;
          } else {
            this.wcsStatusFlag=false;
            this.openStatus2();
            // alert("WCS后台服务器连接异常，请检查服务器");
            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }
          }
        })
        .catch((error) => {
          // console.log(error);
          this.wcsStatusFlag=false;
            this.openStatus2();
            // alert("WCS后台服务器连接异常，请检查服务器");
            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("WCS控制系统异常")
             }catch(error){

             }

        });
        //检查密集架服务状态
            axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetMJJServerStatus?mjjName=DAK')
        .then(async (response) => {
          if (response.data.result) {
            this.mjgStatusFlag = true;
          } else {
            this.mjgStatusFlag = false;
            this.openStatus2();
            try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("密集架接口异常")
             }catch(error){

             }
          }
        })
        .catch((error) => {
          this.mjgStatusFlag = false;
          this.openStatus2();
          try{
              // @ts-ignore:无法被执行的代码的错误
              bound.speakmsg("密集架接口异常")
             }catch(error){

             }
        }); 
        //龙门机械手运行状态
            axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetScadaServerStatus')
        .then(async (response) => {
          if (response.data.result) {
            this.scadaStatusFlag = true;
          } else {
            this.scadaStatusFlag = false;
            this.openStatus2();
          }
        })
        .catch((error) => {
          this.scadaStatusFlag = false;
          this.openStatus2();
        }); 
        
                //PLC状态       
            axios.get("http://192.168.1.188:21021/api/services/app/DAClient/GetPLCServerStatus")
          .then(async (response) => {
            var nodes = response.data.result.items;
            //  console.log(nodes);
            if (nodes.find((f) => f.commQuality == "Bad"&&f.nodeName == "EmergencyStop")) {
              this.plcStatusFlag = false;
              try{
                  // @ts-ignore:无法被执行的代码的错误
                  bound.speakmsg("PLC通讯异常")
                }catch(error){

               }
            } else {
              if (nodes.find((f) => f.nodeName == "EmergencyStop").value == "0") {
                this.plcStatusFlag = false;
                this.openStatus2();
                // alert("设备急停被拍下");
                try{
                  // @ts-ignore:无法被执行的代码的错误
                  bound.speakmsg("急停按钮被按下")
                }catch(error){

               }
              }
              else
              {
                this.plcStatusFlag = true;
              }
              //龙门原点状态
              if (nodes.find((f) => f.nodeName == "LM_Zero").value == "0") {
                if(this.taskInCount+this.taskOutCount==0)
                {
                    this.plcStatusFlag = false;
                }
              }
              else
              {
                // this.plcStatusFlag = true;
              }
            }
          })
          .catch(async (error) => {
            // console.log(error);
            this.plcStatusFlag = false;
            this.openStatus2();
            try{
                  // @ts-ignore:无法被执行的代码的错误
                  bound.speakmsg("PLC通讯异常")
                }catch(error){

               }

          });
      }


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
