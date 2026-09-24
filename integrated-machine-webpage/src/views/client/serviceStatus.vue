<template>
  <div>
    <Modal
      :title="L('服务器工作状态')"
      :value="value"
      :width="630"
      @on-ok="save"
      @on-visible-change="visibleChange"
      :styles="{top: '40px'}"
    >
    <div v-show="!plcWarningModalShow">
        <div class="noselect" @dblclick="checkWMSStatus">
          <Alert :type="wmsStatusFlag ? 'success' : 'error'" show-icon>
            WMS 无人库管理系统后台
            <span slot="desc" v-html="wmsStatusInfo"> </span>
          </Alert>
        </div>
        <Alert v-if="wmsOnly" type="info" show-icon>
          设备服务未接入
          <span slot="desc">当前仅连接 WMS。WCS、密集柜、PLC、SCADA 及龙门状态未检测，设备控制暂不可用。</span>
        </Alert>
        <div v-if="!wmsOnly">
        <div class="noselect" @dblclick="checkWCSStatus">
          <Alert :type="wcsStatusFlag ? 'success' : 'error'" show-icon>
            WCS 无人库控制系统
            <span slot="desc" v-html="wcsStatusInfo"> </span>
          </Alert>
        </div>
        <div class="noselect" @dblclick="checkMjgStatus">
          <Alert :type="mjgStatusFlag ? 'success' : 'error'" show-icon>
            密集柜接口服务
            <span slot="desc" v-html="mjgStatusInfo"> </span>
          </Alert>
        </div>
         <div class="noselect" @dblclick="clickPlcStatus">
          <Alert :type="plcStatusFlag&&scadaStatusFlag ? 'success' : 'error'" show-icon>
            PLC通讯&PLC读写
            <span slot="desc" v-html="plcStatusInfo+scadaStatusInfo"> </span>
          </Alert>
        </div>
        <div class="noselect" @dblclick="clickPlcStatus">
          <Alert :type="lmStatusFlag ? 'success' : 'error'" show-icon>
            龙门机械手运行状态
            <span slot="desc" v-html="lmStatusInfo"> </span>
          </Alert>
        </div>
        <div class="noselect" @dblclick="clickPlcStatus">
          <Alert :type="lmydStatusFlag ? 'success' : 'error'" show-icon>
            龙门机械手零点状态
            <span slot="desc" v-html="lmydStatusInfo"> </span>
          </Alert>
        </div>
      </div>
      </div>
      <div v-show="plcWarningModalShow">
            <div class="noselect">
              <Alert type="warning" show-icon>
                <!-- 龙门回原点操作警示： -->
                <span slot="desc" v-html="plcWarningInfo"  style="font-size: 20px;"> </span>
              </Alert>
            </div>
            <div>
              <Checkbox v-model="checkFlag"><span style="font-size: 15px;">请对以上警示事项进行确认 </span></Checkbox>
            </div>
      </div>
      <!-- <Form ref="archiveboxForm"  label-position="top" :rules="archiveboxRule" :model="archivebox">
                <FormItem :label="L('ArchiveBoxName')" prop="archiveBoxName">
                    <Input v-model="archivebox.archiveBoxName" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('ArchiveBoxRfid')" prop="archiveBoxRfid">
                    <Input v-model="archivebox.archiveBoxRfid" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('CellId')" prop="cellId">
                    <Input v-model="archivebox.cellId" :maxlength="32"></Input>
                </FormItem>
                <FormItem :label="L('StorageRemark')" prop="storageRemark">
                    <Input v-model="archivebox.storageRemark" :maxlength="256"></Input>
                </FormItem>
            </Form> -->
      <div slot="footer">
        
          <Checkbox v-model="neverTip" ><span style="font-size: 15px;">不再提示 </span></Checkbox>
        <Button @click="backZero" :disabled="wmsOnly" type="primary">{{ L("龙门回原点") }}</Button>
        <Button  v-show="!plcWarningModalShow" @click="refresh" type="primary">{{ L("手动刷新") }}</Button>
        <Button @click="cancel" type="primary">{{ L("关闭") }}</Button>
      </div>
    </Modal>
  </div>
  <!-- <pLCInitWarning v-model="plcWarningModalShow" ></pLCInitWarning> -->
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "../../lib/util";
import AbpBase from "../../lib/abpbase";
// import PLCInitWarning from './PLCInitWarning.vue'
import axios from "axios";
import AppConsts from '../../lib/appconst';
class PlcNode {
  NodeName: string;
  NodeType: string;
  Address: string;
  Value: string;
  CommQuality: string;
  TimeStamp: string;
}
@Component
export default class ServiceStatus extends AbpBase {
  /** 仅 WMS 模式不检测或控制尚未接入的设备。 */
  wmsOnly:boolean = AppConsts.wmsOnly;
  /** 避免手动刷新与定时刷新重复发起状态请求。 */
  wmsStatusPending:boolean = false;
  @Prop({ type: Boolean, default: false }) value: boolean;
  @Prop() servicesFlag: boolean;
  wmsStatusInfo: string = "正在检测";
  wmsStatusFlag: boolean = true;
  wcsStatusInfo: string = "正在检测";
  wcsStatusFlag: boolean = true;
  mjgStatusInfo: string = "正在检测";
  mjgStatusFlag: boolean = true;
  plcStatusInfo: string = "正在检测";
  scadaStatusInfo: string = "";
  scadaStatusFlag: boolean = true;
  plcStatusFlag: boolean = true;
  lmStatusInfo: string = "正在检测";
  lmStatusFlag: boolean = true;
  lmydStatusInfo: string = "正在检测";
  lmydStatusFlag: boolean = true;
  plcWarningModalShow:boolean=false;
  plcWarningInfo:string="龙门回原点需确认以下条件：<br>1、请按下急停按钮后并完成恢复；<br>2、请确保库房内龙门区域无人；<br>3、点击龙门回原点按钮并等待直至恢复；"

  checkFlag:boolean=false;
  neverTip:boolean=false;
  //龙门回原点
   async backZero() {
     if (this.wmsOnly) return;
     if(!this.plcWarningModalShow)
     {
       this.plcWarningModalShow=true;
     }
     else
     {
       if(this.checkFlag)
       {
            this.$Modal.confirm({
                title:this.L('Tips'),
                content:this.L('龙门回原点需慎重操作，请确认是否下达回原点指令'),
                okText:this.L('Yes'),
                cancelText:this.L('No'),                
                onOk:async()=>{
              await axios
                .post("http://192.168.1.188:21021/api/services/app/DAClient/longmengohome", {})
                .then(async (response) => {
                  console.log(response); 
                  alert("指令已下发！");
                    this.checkFlag=false;
                    this.plcWarningModalShow=false;
                })
                .catch(function (error) {
                  alert("龙门回零接口调用失败,请检查服务是否正常");
                  console.log(error);
                });
          }
         })


       }
       else
       {
         alert("请先确认已阅读警示信息！");
       }
     }

  }
  
  save() {
    // (this.$refs.archiveboxForm as any).validate(async (valid:boolean)=>{
    //     if(valid){
    //         await this.$store.dispatch({
    //             type:'archivebox/create',
    //             data:this.archivebox
    //         });
    //         (this.$refs.archiveboxForm as any).resetFields();
    //         this.$emit('save-success');
    //         this.$emit('input',false);
    //     }
    // })
    this.$emit("save-success", false);
    this.$emit("input", false);
    this.$emit("never-tip",this.neverTip)
  }
  cancel() {
    // (this.$refs.archiveboxForm as any).resetFields();
    if(this.wmsStatusFlag&&this.wcsStatusFlag&&this.mjgStatusFlag&&this.plcStatusFlag&&this.scadaStatusFlag)
    {
    this.$emit("save-success", true);
    }
    else
    {
      this.$emit("save-success", false);
    }

    
    this.$emit("never-tip",this.neverTip)
    this.$emit("input", false);
  }
  //手动刷新
  refresh() {
    // this.wmsStatusInfo="正在检测";
    // this.wcsStatusInfo="正在检测";
    // this.mjgStatusInfo="正在检测";
    // this.plcStatusInfo="正在检测";
    // this.lmydStatusInfo="正在检测";
    this.getAllStatus();
  }
   getAllStatus() {
     this.checkWMSStatus();
     if (this.wmsOnly) return;
     this.checkWCSStatus();
     this.checkMjgStatus();
     this.checkPlcStatus();
     this.checkScadaStatus();
  }
  async checkWMSStatus() {
    if (this.wmsOnly) {
      if (this.wmsStatusPending) return;
      this.wmsStatusPending = true;
      this.wmsStatusInfo = '正在检测 WMS';
      try {
        await this.$store.dispatch('app/getWMSStatus');
        this.wmsStatusFlag = true;
        this.wmsStatusInfo = 'WMS 可访问（设备状态未检测）';
      } catch (error) {
        this.wmsStatusFlag = false;
        this.wmsStatusInfo = 'WMS 暂不可访问，请检查后端进程及地址配置';
      } finally {
        this.wmsStatusPending = false;
      }
      return;
    }
    this.wmsStatusInfo = "正在检测";
    //使用axios.get  不会有异常弹出报错。
    await axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetWMSServerStatus')
      .then(async (response) => {
        // this.serviceConnect=false;
        // console.log(response);
        if (response.data.result) {
          this.wmsStatusFlag = true;
          this.wmsStatusInfo = "连接正常";
        } else {
          this.wmsStatusFlag = false;
          this.wmsStatusInfo = "连接异常（请到服务器服务监控界面查看WMS服务是否启动，可尝试重启WMS服务）";
        }
      })
      .catch((error) => {
        console.log(error);
        this.wmsStatusFlag = false;
        this.wmsStatusInfo = "检测接口异常（请到服务器服务监控界面查看网络和WMS服务是否启动，可尝试重启WMS服务）";
      });
  }
  async checkWCSStatus() {
    if (this.wmsOnly) return;
    this.wcsStatusInfo = "正在检测";
    await axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetWCSServerStatus')
      .then(async (response) => {
        // this.serviceConnect=false;
        console.log(response);
        if (response.data.result) {
          this.wcsStatusFlag = true;
          this.wcsStatusInfo = "连接正常";
        } else {
          this.wcsStatusFlag = false;
          this.wcsStatusInfo = "服务异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WCS服务）";
        }
      })
      .catch((error) => {
        console.log(error);
        this.wcsStatusFlag = false;
        this.wcsStatusInfo = "检测接口异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS、WCS服务))";
      });
    // //   let rep = axios.get('http://192.168.1.188:7788/wcs/serviceStatus',  {
    //   let rep = axios.get('http://192.168.1.188:7788/clientaccesspolicy',  {
    //       headers:{'Cache-Control': 'no-cache',
    //       'Access-Control-Allow-Origin':'*'},
    //        params: {
    //             timestamp: new Date().getTime()
    //         },
    //       responseType:'json',
    //       withCredentials: false,
    //             })
    //         .then( async (response) => {
    //             console.log(response);
    //                if(response.data)
    //                {
    //                     this.wcsStatusFlag=true;
    //                     this.wcsStatusInfo='连接正常';
    //                }
    //                else
    //                {
    //                 this.wcsStatusFlag=false;
    //                 this.wcsStatusInfo='服务异常';
    //                }
    //         })
    //         .catch(async (error) =>  {
    //             console.log(error);
    //             this.wcsStatusFlag=false;
    //             this.wcsStatusInfo='连接异常';
    //         });
  }
  async checkMjgStatus() {
    if (this.wmsOnly) return;
    this.mjgStatusInfo = "正在检测";
    await axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetMJJServerStatus?mjjName=DAK')
      .then(async (response) => {
        // this.serviceConnect=false;
        console.log(response);
        if (response.data.result) {
          this.mjgStatusFlag = true;
          this.mjgStatusInfo = "连接正常";
        } else {
          this.mjgStatusFlag = false;
          this.mjgStatusInfo = "连接异常（请到服务器的服务监控界面查看密集架设备接口、ping状态是否正常，可尝试重启密集架设备和WCS服务）";
        }
      })
      .catch((error) => {
        console.log(error);
        this.mjgStatusFlag = false;
        this.mjgStatusInfo = "检测接口异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS、WCS服务))";
      }); 
  }
    async checkScadaStatus() {
    if (this.wmsOnly) return;
    this.scadaStatusInfo = "";
    await axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetScadaServerStatus')
      .then(async (response) => {
        // this.serviceConnect=false;
        console.log(response);
        if (response.data.result) {
          // this.mjgStatusFlag = true;
          // this.mjgStatusInfo = "连接正常";
          this.scadaStatusFlag = true;
          this.scadaStatusInfo = "&PLC读写正常";
        } else {
          // this.mjgStatusFlag = false;
          this.scadaStatusFlag = false;
          this.scadaStatusInfo = "&PLC读写服务异常（请到服务器服查看SCADA服务是否启动，可尝试重启SCADA服务）";
        }
      })
      .catch((error) => {
        console.log(error);
        this.scadaStatusFlag = false;
        // this.mjgStatusFlag = false;
        // this.scadaStatusInfo = "检测接口异常（请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS\WCS服务））";
      }); 
  }
  async clickPlcStatus()
  {
    await this.checkPlcStatus()
    await this.checkScadaStatus()
  }
  async checkPlcStatus() {
    if (this.wmsOnly) return;
//       var node={address: "DB500.B183",​​​​
// commQuality: "Good",​​​​
// nodeName: "LM_Zero",​​​​
// nodeType: "Byte",​​​​
// timeStamp: "2022-03-18T21:41:49.685+08:00",​​​​
// value: "0"}
    // var nodeaa= node as PlcNode;
    this.plcStatusInfo = "正在检测";
    this.lmStatusInfo = "正在检测";
    this.lmydStatusInfo = "正在检测";
    await axios
      .get("http://192.168.1.188:21021/api/services/app/DAClient/GetPLCServerStatus")
      .then(async (response) => {
        // console.log(response.data);
        // var nodes = response.data.result.items as PlcNode[];
         var nodes = response.data.result.items;
         console.log(nodes);
        if (nodes.find((f) => f.commQuality == "Bad"&&f.nodeName == "LM_State")) {
          this.plcStatusFlag = false;
          this.plcStatusInfo = "PLC通讯异常（请到服务器服务监控界面查看PLC通讯是否异常,可尝试重启PLC,SCADA服务）";
          this.lmStatusFlag = false;
          this.lmStatusInfo = "PLC通讯异常";
          this.lmydStatusFlag = false;
          this.lmydStatusInfo = "PLC通讯异常";
        } else {
           this.plcStatusFlag = true;
          this.plcStatusInfo = "PLC通讯正常";
          if (nodes.find((f) => f.nodeName == "LM_State").value == "1") {
            // this.plcStatusFlag = true;
            // this.plcStatusInfo = "连接正常&龙门就绪";
           this.lmStatusFlag = true;
          this.lmStatusInfo = "龙门停止";
          } else {
            // this.plcStatusFlag = true;
            // this.plcStatusInfo = "连接正常&龙门未就绪";
            this.lmStatusFlag = false;
            this.lmStatusInfo = "龙门运行中";
          }
          if (nodes.find((f) => f.nodeName == "EmergencyStop").value == "0") {
            this.lmStatusFlag = false;
            this.lmStatusInfo += "&急停按钮被按下(请尝试处理设备异常和恢复状态)";
          }
          else
          {
            this.lmStatusFlag = true;
          }
          if (nodes.find((f) => f.nodeName == "LM_Zero").value == "1") {
            this.lmydStatusFlag = true;
            this.lmydStatusInfo = "在零点";
          } else {
            this.lmydStatusFlag = false;
            this.lmydStatusInfo = "不在零点（若设备异常，可尝试龙门回零）";
          }
        }
      })
      .catch(async (error) => {
        console.log(error);
        this.plcStatusFlag = false;
        this.plcStatusInfo = "检测接口异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS、WCS服务))";
        this.lmStatusFlag = false;
        this.lmStatusInfo = "检测接口异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS、WCS服务))";
        this.lmydStatusFlag = false;
        this.lmydStatusInfo = "检测接口异常(请到服务器服务监控界面查看WCS服务是否启动，可尝试重启WMS、WCS服务))";
      });
  }
  servicestatusintervalId:number=0;
  /** 离开操作台后停止状态窗口的轮询。 */
  beforeDestroy() {
    window.clearInterval(this.servicestatusintervalId);
  }
  async visibleChange(value: boolean) {
    if (!value) {
       window.clearInterval(this.servicestatusintervalId);
       console.log('hide');
       console.log(this.servicestatusintervalId);
      this.$emit("input", value);
    } else {
                this.checkFlag=false;
          this.plcWarningModalShow=false;
      await this.getAllStatus();
      this.servicestatusintervalId= setInterval(this.refresh, 20000);

    }
  }
}
</script>
<style scoped>
.noselect {
  -moz-user-select: none; /*火狐*/

  -webkit-user-select: none; /*webkit浏览器*/

  -ms-user-select: none; /*IE10*/

  -khtml-user-select: none; /*早期浏览器*/

  user-select: none;
}
</style>
