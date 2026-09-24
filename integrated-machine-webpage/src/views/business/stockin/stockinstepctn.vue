<style lang="less">
    @import "../stocktask.less";
</style>
<template>
    <div>
        <div dis-hover style="height:800px" id='building'>
            <div class="page-body2">
                <div class="margin-top-1">
                        <div class="main-header">
                            
                            <div class="navicon-con">
                                 <!-- <img class="xwblogo" src="../../../images/xwb1.png"  style="margin-top: 3px;"/> -->
                                   <!-- <img class="xwblogo" src="../../../images/qh.png"  style="margin-top: 3px;"/> -->
                                   <img class="xwblogo"   style="margin-top: 3px;" :src="smallLogoUrl"/>
                            <!-- <Icon type="md-archive" size="32" color="#57a3f3" style="margin-top: -8px;"/> -->
                            </div>
                             <div class="header-middle-con" style="margin-left: -25px;margin-top: 1px;"><h1>存档</h1> </div>
                            <div class="header-avator-con" style="margin-right: 10px;">     

                                <div class="user-dropdown-menu-con">
                                    <Row type="flex" justify="end" align="middle" class="user-dropdown-innercon" style="margin-left: -10px;">
                                        <Dropdown transfer trigger="click">
                                            <a href="javascript:void(0)">
                                                <span class="main-user-name">{{ userName }}</span>
                                                <Icon type="arrow-down-b"></Icon>
                                            </a>
                                        </Dropdown>
                                        <!-- <span class="avatar" style="background: #619fe7;margin-left: 10px;"><img src="../../../images/usericon.jpg" /></span> -->
                                            <Badge :count="alertCount">
                                                <Icon @click="openTaskExps" type="ios-notifications-outline" size="30"  style="margin-left: 10px;" color="LightSkyBlue"></Icon>
                                            </Badge>
                                        <Icon @click="openStatus" v-show="serviceConnect&&wcsStatusFlag&&mjgStatusFlag&&scadaStatusFlag&&plcStatusFlag" type="ios-wifi" size="30" style="margin-left: 10px;" color="LightSkyBlue" />
                                        <Icon @click="openStatus" v-show="!(serviceConnect&&wcsStatusFlag&&mjgStatusFlag&&scadaStatusFlag&&plcStatusFlag)" type="ios-wifi-outline" size="30" style="margin-left: 10px;" color="#ed4014" />

                                        <Icon @click="logout" type="md-return-left" size="28" style="margin-left: 10px;margin-top: 5px;" color="LightSkyBlue" />

                                    </Row>
                                </div>

                            </div>
                        </div>

                </div>
                <div v-show="current==1||continuousFlag" class="margin-top-cm">
                    <!-- <Form ref="queryForm" :label-width="80" label-position="left" inline> -->
                        <Row :gutter="24">
                            <Col span="6" offset="9">
                            <input id="inputEle"  ref="inputEle" v-model="findRfid"  style ="ime-mode:disabled;width:0px;height:0px;border-width:0px;" type="password" />
                                    <!-- <Input v-show="inputshow" element-id="inputEle"  ref="inputEle" v-model="findRfid"  style ="ime-mode:disabled;width:0px;height:0px" type="password" clearable required :placeholder="L('条码标签')" @keydown="scaninput1" > 
                                                                                                          <Icon type="md-barcode" slot="suffix"  @click="scan" />   
                                        </Input>                                    -->
                                     <Input    element-id="inputEle1" ref="inputEle1" v-model="findRfid"  style ="ime-mode:disabled;" clearable required :placeholder="L('条码标签')"  @keyup.enter.native="scan" @input="scaninput"  >
                                                                         <Icon type="md-barcode" slot="suffix"  @click="scan" />   
                                        </Input>

                                <!-- <Input v-model="findRfid"   element-id="inputEle1" ref="inputEle1"  clearable  @keyup.enter.native="scan"  placeholder="扫码条码" >
                            <Icon type="md-barcode"  id="inputEle2" ref="inputEle2" slot="suffix"  @click="scan" />
                        </Input> -->
                            </Col>
                             <Col span="3">

                                <!-- <Button  type="primary" size="large" @click="openDoor">{{L('打开柜门')}}</Button> -->
                                 <!-- <Button shape="circle" icon="ios-search" @click="getpage"></Button> -->
                            </Col>
                              <Col span="6">

                            </Col>
                        </Row>
                    <!-- </Form> -->
                </div>
                 <!-- <div v-show="false" class="margin-top-cm">
                    <Form ref="queryForm1" :label-width="40" label-position="right" inline>
                       <Row>
                        <Col  v-show=false  span="4">  
                          <FormItem :label="L('Id')+':'" > 
                       <Input v-model="currentID" disabled></Input>
                          </FormItem>
                        </Col>
                        <Col span="6">
                         <FormItem :label="L('条码')+':'" > 
                       <Input v-model="currentRfid" disabled></Input>
                         </FormItem>
                       </Col>
                       <Col span="6">
                            <FormItem :label="L('起点')+':'" > 
                       <Input v-model="currentScell" disabled></Input>
                         </FormItem>
                       </Col>
                       <Col span="6">
                         <FormItem :label="L('终点')+':'" > 
                       <Input v-model="currentEcell" disabled></Input>
                       </FormItem>
                       </Col>
                        <Col span="6">
                        <FormItem :label="L('状态')+':'" > 
                       <Input v-model="taskStatus" disabled></Input>
                       </FormItem>
                       </Col>
                       </Row>
                      </Form>
                  </div> -->
                <div v-show="current>1&&!continuousFlag" >
                        <Row>
                            <Col span="14" offset="8">
                            <!-- <Button v-show="current==1" shape="circle"  icon="ios-search" @click="getpage"></Button> -->
                            <ButtonGroup>
                                <!-- <Button v-show="current>1" type="primary" @click="goBack"  icon="ios-skip-backward" size="large"></Button> -->
                                 <!-- <Button v-show="current==2" icon="ios-apps-outline" type="primary" size="large" @click="editCell" class="toolbar-btn">{{L('指定库位')}}</Button> -->
                                <!-- <Button  v-show="current==2" icon="ios-expand" type="primary" size="large" @click="editStation" class="toolbar-btn">{{L('指定柜格')}}</Button> -->
                                <Button v-show="current==2" icon="ios-download" type="primary" size="large" @click="taskAssign">{{L('下达任务')}}</Button>
                                <Button  v-show="current>1&&current<4" icon="ios-remove" type="primary" size="large" @click="taskCancel"></Button>
                                <Button  v-show="current>2" icon="ios-refresh" type="primary" size="large" @click="getpage"></Button>
                                <Button  v-show="current>2" icon="ios-checkbox" type="primary" size="large" @click="taskComplete" ></Button>
                                <!-- <Button v-show="current>1" type="primary"  @click="goForward" icon="ios-skip-forward" size="large"></Button> -->
                            </ButtonGroup>
                            </Col>
                        </Row>
                </div>
                 <div v-show="current>=0"  class="margin-top-cm">
                     <Steps :current="current" class="taskstep" id="taskstep">
                        <Step v-for="(item) of stepdata" :key="item.id"  :title="item.title" :icon="item.icon"  :content="item.id<(current+1)?'已完成':item.content" ></Step>
                    </Steps>
                </div> 
                 <div   class="margin-top-cm">
                   <Row :gutter="24">
                        <Col span="6">
                            <div @click="doorSelect" @dblclick="doorDbClick"> 
                            <Card  :id="doorstatus[0].id" class="door DoorClose" >   
                                      <Row class="doortitle">
                                        <span class="cardtitle">1号柜</span>
                                    </Row>

                                    <Row>
                                     <Icon type="md-list-box" size="50" color="#e8eaec"/>
                                       </Row>
                                      <Row>
                                         <span class="cardtitle">{{getenumv(doorstatus[0].taskstatus)}}</span>
                                    </Row>
                                    <Row>
                                        <span class="cardtitle">{{doorstatus[0].rfid}}</span>
                                    </Row>
                            </Card>  
                            </div>
                        </Col>
                                                <Col span="6">
                                                <div @click="doorSelect" @dblclick="doorDbClick"> 
                            <Card :id="doorstatus[1].id" class="door DoorClose" >   
                                      <Row class="doortitle">
                                        <span class="cardtitle">2号柜</span>
                                    </Row>

                                    <Row>
                                     <Icon type="md-list-box" size="50" color="#e8eaec"/>
                                       </Row>
                                      <Row>
                                         <span class="cardtitle">{{getenumv(doorstatus[1].taskstatus)}}</span>
                                    </Row>
                                     <Row>
                                        <span class="cardtitle">{{doorstatus[1].rfid}}</span>
                                    </Row>
                            </Card>  
                                                </div>
                        </Col>
                                                <Col span="6">
                                                <div @click="doorSelect"  @dblclick="doorDbClick"> 
                            <Card  :id="doorstatus[2].id" class="door DoorClose" >   
                                      <Row class="doortitle">
                                        <span class="cardtitle">3号柜</span>
                                    </Row>

                                    <Row>
                                     <Icon type="md-list-box" size="50" color="#e8eaec"/>
                                       </Row>
                                      <Row>
                                         <span class="cardtitle">{{getenumv(doorstatus[2].taskstatus)}}</span>
                                    </Row>
                                                                         <Row>
                                        <span class="cardtitle">{{doorstatus[2].rfid}}</span>
                                    </Row>
                            </Card>  
                                                </div>
                        </Col>
                                                <Col span="6">
                                                <div @click="doorSelect($event)"  @dblclick="doorDbClick"> 
                            <Card :id="doorstatus[3].id" class="door DoorClose"  >   
                                      <Row class="doortitle">
                                        <span class="cardtitle">4号柜</span>
                                    </Row>

                                    <Row>
                                     <Icon type="md-list-box" size="50" color="#e8eaec"/>
                                       </Row>
                                      <Row>
                                         <span class="cardtitle">{{getenumv(doorstatus[3].taskstatus)}}</span>
                                    </Row>
                                    <Row>
                                        <span class="cardtitle">{{doorstatus[3].rfid}}</span>
                                    </Row>
                            </Card>  
                                                </div>
                        </Col>
                   </Row>

                </div>  
                <div v-show="true" class="margin-top-cm">
                                   <Row :gutter="24">
                             <Col span="8" offset="8">
                             <Alert  v-show="queueCount>0" show-icon>排队任务数：{{ queueCount }}</Alert>
                            </Col>
                              <Col span="8">
                            </Col>
            </Row>

                </div>              
            </div>
        </div>
         <achive-list v-model="createModalShow"  @save-success="getpage"></achive-list>
         <serviceStatus v-model="statusModalShow" :servicesFlag="serviceConnect" @save-success="GetServicesFlag" @never-tip="GetneverTip"></serviceStatus>
         <taskException v-model="taskExpModalShow" ></taskException>
         <putboxTip v-model="putTipModalShow" ></putboxTip>
     </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import Stocktask from '@/store/entities/stocktask'
    import Stocktasklist from '@/store/entities/stocktasklist'
    import AchiveList from './achive-list.vue'
    import ServiceStatus from './../../client/serviceStatus.vue'
    import TaskException from '../../client/taskException.vue'
    import PutboxTip from './putboxTip.vue'
    import Cookies from 'js-cookie';
    import axios from 'axios';
    class  PageStocktaskRequest extends PageRequest{
        keyword:string;
        manageTypeCode:string='NPFullStockIn';
        manageStatus:string;
        from:Date;
        to:Date;
    }
    class TaskAssignDto {      
    mainId: number=0;
    cellcode: string="";       
    }
class TaskFaultDto {
  controlTaskId: number;
  deviceTaskId: number;
  manageTaskId: number;
  barcode: string;
  taskFaultDesc: string;
    faultCode:number;
  resolveMethod:string;
}
    @Component({
        components:{AchiveList,ServiceStatus,TaskException,PutboxTip}
    })
    export default class Stocktasks extends AbpBase{
        taskAssignDto:TaskAssignDto=new TaskAssignDto();
               lockScreenSize:number=0;
        //filters
        pagerequest:PageStocktaskRequest=new PageStocktaskRequest();
        creationTime:Date[]=[];

       statusModalShow:boolean=false;
       serviceConnect:boolean=true;//服务器连接状态
       taskExpModalShow:boolean=false;//异常任务组件
       putTipModalShow:boolean=false;//放档案盒提示组件
        wcsStatusFlag:boolean=true;//WCS服务器连接状态
        mjgStatusFlag:boolean=true;//密集架连接状态
        scadaStatusFlag:boolean=true;//SCADA连接状态
        plcStatusFlag:boolean=true;//PLC及急停连接状态
        neverTip:boolean = false;
        //获取服务器状态
        getserviceStatus()
        {

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
        //logo图标地址
        get smallLogoUrl(){
            return abp.setting.values.ClientSmallLogoPath
        }
        openStatus()
        {
            this.statusModalShow=true;
        }
        openPutTip()
        {
            this.putTipModalShow=true;
        }
        closePutTip()
        {
            this.putTipModalShow=false;
        }

        createModalShow:boolean=false;
        editModalShow:boolean=false;
        updateCellModalShow:boolean=false;
        updateStationModalShow:boolean=false;
        inputshow:boolean=true;

        findRfid:string="";
        current:number=1;
        currentID:string="";
        currentRfid:string="";
        currentScell:string="";
        currentEcell:string="";
        taskStatus:string="";  
        intervalId:number=0;
         intervalId3:number=0;
         intervalIdwcs:number=0;
        currentpg:number=0;//当前页
        continuousFlag:boolean=true;//连续入库标识

                        stepdata=[{ id:'1',title:'档案扫码',
            content:'',icon:'md-barcode'},{ id:'2',title:'档案放入',
            content:'',icon:'md-log-in'},{ id:'3',title:'密集柜开启',
            content:'',icon:'ios-albums'},{ id:'4',title:'机械手抓取',
            content:'',icon:'md-download'},{ id:'5',title:'任务结束',
            content:'',icon:'ios-flag'}];
            //柜门任务状态
            doorstatus=[{ id:12001,rfid:'',cclass:'',status:'',taskstatus:'',taskid:'',taskEcell:''},
                        { id:12002,rfid:'',cclass:'',status:'',taskstatus:'',taskid:'',taskEcell:''},
                        { id:12003,rfid:'',cclass:'',status:'',taskstatus:'',taskid:'',taskEcell:''},
                        { id:12004,rfid:'',cclass:'',status:'',taskstatus:'',taskid:'',taskEcell:''}];
        enumdata=[{ value:'',
            key:''},{ value:'入库',
            key:'NPFullStockIn'},{ value:'等待执行',
            key:'WaitingExecute'}
            ,{ value:'指令已下达',
            key:'OrderCatched'}
            ,{ value:'执行中',
            key:'Executing'}
            ,{ value:'借阅出库',
            key:'NpFullStockOut'}
            ,{ value:'待完成确认',
            key:'WaitingConfirm'}
            ,{ value:'柜门打开',
            key:'StationOpen'}
            ,{ value:'柜门关闭',
            key:'StationClose'},
            { value:'密集柜打开中',
            key:'CabinetWait'}
            ,{ value:'密集柜已打开',
            key:'CabinetComplete'}
            ,{ value:'档案抓取中',
            key:'RobotWait'},
            { value:'档案抓取完成',
            key:'RobotComplete'},
            { value:'申请强制完成',
            key:'CompeleteRequest'},
            { value:'任务完成',
            key:'Complete'},
            { value:'申请强制取消',
            key:'TaskDeleteRequest'}]
        //     fireKeyEvent(el, evtType, keyCode) {
        //     var evtObj;
        //     if (document.createEvent) {
        //             //chrome 浏览器下模拟事件
        //             evtObj = document.createEvent('UIEvents');
        //             evtObj.initUIEvent(evtType, true, true, window, 1);
        //              console.log(evtObj);
        //             delete evtObj.keyCode;
        //             if (typeof evtObj.keyCode === "undefined") {//为了模拟keycode
        //                 Object.defineProperty(evtObj, "keyCode", { value: keyCode });   
        //                                      console.log("2"+keyCode);                    
        //             } else {
        //                 evtObj.key = String.fromCharCode(keyCode);
        //                      console.log("3"+evtObj.key);  
        //             }
        //                              evtObj.key = String.fromCharCode(keyCode);
        //         el.dispatchEvent(evtObj);

        //     } 
        // }

         fireKeyEvent(element, evtType, keyChar) {
            element.focus();
            var KeyboardEventInit = {key:keyChar, code:"", location:0, repeat:false, isComposing:false};
            var evtObj = new KeyboardEvent(evtType, KeyboardEventInit);
            element.dispatchEvent(evtObj);
        }
        // scanUniqueFlag:boolean=false;
        //20220907 增加防止重复下达管理任务的控制标记
        taskAssignFlag:boolean=false;
        //扫码查询
        async scan()
        {
            if(this.findRfid!="")
            {
                var flag=0;
                if(this.findRfid.length>8)
                {
                    this.findRfid=this.findRfid.substr(this.findRfid.length-8,this.findRfid.length);
                     console.log("scan:"+this.findRfid);
                }
                 for (let dd = 0; dd < this.doorstatus.length; dd++)  
                 {
                     if(this.doorstatus[dd].rfid==this.findRfid.trim())
                     {
                        let doorselectedOld = document.getElementById(this.currentScell);
                        doorselectedOld.classList.remove('DoorSelected');
                        let doorselected=document.getElementById(this.doorstatus[dd].id.toString());
                        doorselected.classList.add('DoorSelected');
                        this.currentID=this.doorstatus[dd].taskid;
                        this.currentRfid=this.doorstatus[dd].rfid;
                        this.currentScell=this.doorstatus[dd].id.toString();
                        this.currentEcell=this.doorstatus[dd].taskEcell;
                        this.taskStatus=this.getenumv(this.doorstatus[dd].taskstatus)
                        this.getStep(this.doorstatus[dd].taskstatus);
                        if(this.doorstatus[dd].taskstatus!="OrderCatched"&&this.doorstatus[dd].taskstatus!="Executing")
                        {
                                this.$Message.info('档案盒已完成扫码，无须重复扫码');
                                return;
                        }

                        await  this.$store.dispatch({
                                    type:'stocktask/tasklistCheckInt',
                                    mainId:this.currentID
                                });
                        setTimeout(async () =>{
                        await this.$store.dispatch({
                            type:'stocktask/getLists',
                            mainId:this.currentID
                        }).then( async (response) => {
                            //                        console.log("test");
                            //                         console.log(response);
                            // var slist= response.data.result.items as Stocktasklist[];
                            if(this.list2.length>0)
                            {
                                //20220718 暂时屏蔽档案文件RFID校验
                                // this.$store.commit('stocktask/edit',this.doorlist.filter(x=>x.id==this.currentID)[0]);
                                // //this.createModalShow=true;
                                // //设置演示，避免界面卡顿
                                //         setTimeout(() => {
                                // this.createModalShow=true;
                                //     }, 500);
                                // try {
                                //     // @ts-ignore：无法被执行的代码的错误
                                //     bound.check(this.currentID);
                                // } catch (error) {
                                //     console.log(error);
                                // }
                                // // // @ts-ignore：无法被执行的代码的错误
                                // // bound.check(this.currentID);
                                // //  this.findRfid="";

                                
                                //20220718 暂时屏蔽档案文件RFID校验
                                await   this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:this.currentID
                                }).then( async (response) => {
                                    this.$Message.info('开门命令已下达');   
                                })
                                .catch( (error) => {
                                    console.log(error);
                                }); 

                            }
                            else
                            {
                                // this.$Message.info('开门命令已下达');
                                //档案盒为空  直接执行开门程序
                              await   this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:this.currentID
                                }).then( async (response) => {
                                    this.$Message.info('开门命令已下达');   
                                })
                                .catch( (error) => {
                                    console.log(error);
                                }); 
                                //this.findRfid="";
                            }
                        })
                        .catch( (error) => {
                            console.log(error);
                        }); 
                        },200);
                        flag=1;
                     }

                 }  
                 if(flag==0)
                 {
                     console.log("扫码创建任务");
                      this.taskAssignFlag=true;//20220907
                       await  this.$store.dispatch({
                                type:'stocktask/taskAssignUseRfid',
                                rfid: this.findRfid
                            }).then( async (response) => {
                           this.$Message.info('已创建新的入库任务'); 
                           console.log(response);    
                           this.taskAssignFlag=false;   //20220907                  
                           return;
                        })
                        .catch( (error) => {
                            this.taskAssignFlag=false;  //20220907
                            console.log(error);
                        }); 
                    //  this.$Message.info('当前任务中条码信息不存在');
                      //this.findRfid="";
                 }
            }
            else
            {
                alert("条码信息不能为空。");
                //无效
                //this.$Message.info('请先扫码');
                //无效
                                //                 this.$Modal.confirm({
                                //         title:this.L('Tips'),
                                //         content:this.L('请先扫码'),
                                //         okText:this.L('Yes'),
                                //         cancelText:this.L('No'),
                                //         onOk:async()=>{

                                //         }
                                // })
                       this.changekeys();    //切换输入法     
            //     var inputEle1 = document.getElementById("inputEle1");
            //    inputEle1.blur();  
            //    this.inputshow=true;
            //     (this.$refs.inputEle as any).focus();
            //    // this.findRfid="aaa";
            //                      var inputEle = document.getElementById("inputEle");
            //         //  var cCode = "1".charCodeAt(0);
            //         //  console.log(cCode);
            //         //  this.fireKeyEvent(inputEle, "keydown", cCode);
            //          inputEle.addEventListener('keydown', function (e) {
            //              //var aa = document.getElementById("inputEle");
  
            //             (inputEle as HTMLInputElement).value = e.key;
            //         }, false);
                    
            //         this.fireKeyEvent(inputEle,"keydown","a");

            //         inputEle.focus();      
            //              setTimeout(this.changeinput, 800);                              
            //          //(this.$refs.inputEle as any).focus();
            //           //this.findRfid="";
            //         //(this.$refs.inputEle1 as any).focus();
            //          //this.inputshow=false;
            //         // this.fireKeyEvent(testss, "keypress", cCode);
            //         //this.fireKeyEvent(testss, "keyup", cCode);
            }
            if(flag>0)
            {
                //this.openDoor();  //扫描后自动下达指令
            }         
        }

        //切换输入法
        changekeys()
        {
            var inputEle1 = document.getElementById("inputEle1");
               inputEle1.blur();  
               this.inputshow=true;
                (this.$refs.inputEle as any).focus();
               // this.findRfid="aaa";
                                 var inputEle = document.getElementById("inputEle");
                    //  var cCode = "1".charCodeAt(0);
                    //  console.log(cCode);
                    //  this.fireKeyEvent(inputEle, "keydown", cCode);
                     inputEle.addEventListener('keydown', function (e) {
                         //var aa = document.getElementById("inputEle");
  
                        (inputEle as HTMLInputElement).value = e.key;
                    }, false);
                    
                    this.fireKeyEvent(inputEle,"keydown","a");

                    inputEle.focus();      
                         setTimeout(this.changeinput, 800);                              
                     //(this.$refs.inputEle as any).focus();
                      //this.findRfid="";
                    //(this.$refs.inputEle1 as any).focus();
                     //this.inputshow=false;
                    // this.fireKeyEvent(testss, "keypress", cCode);
                    //this.fireKeyEvent(testss, "keyup", cCode);
        }
        changeinput()
        {
                    //                          var inputEle = document.getElementById("inputEle");
                    //    (inputEle as HTMLInputElement).value ='a';
              var inputEle1 = document.getElementById("inputEle1");
               inputEle1.focus();  
               //this.inputshow=false;
        }
        scaninput(value)
        {
            //console.log(value);
            // 	this.value = this.value.replace(/[^\x00-\xff]/g,"");
            // alert(val);
            //value=value;
        }
      getpage2(mainid){       
            this.$store.dispatch({
                type:'stocktask/getLists',
                mainId:mainid
            })
        }
        doorSelect(event)
        {
            //console.log(event.currentTarget.firstChild)
            let doorselected=event.currentTarget.firstChild;
            console.log(doorselected.id)
            if(this.currentScell!=doorselected.id)
            {
                let selectTask=this.doorstatus.filter(x=>x.id==doorselected.id)[0];
                if(selectTask.rfid!="")
                {
                    let doorselectedOld = document.getElementById(this.currentScell);
                    doorselectedOld.classList.remove('DoorSelected');
                    doorselected.classList.add('DoorSelected');
                    this.currentID=selectTask.taskid;
                    this.currentRfid=selectTask.rfid;
                    this.currentScell=selectTask.id.toString();
                    this.currentEcell=selectTask.taskEcell;
                    this.taskStatus=this.getenumv(selectTask.taskstatus)
                    this.getStep(selectTask.taskstatus);
                    //this.currentpg=index;
                }
            }
            else
            {
                if(doorselected.classList.contains('DoorSelected'))
                {
                    doorselected.classList.remove('DoorSelected');
                }
                else
                {
                    doorselected.classList.add('DoorSelected');
                }
            }
            
        }
        doorDbClick(event)
        {
            let doorselected=event.currentTarget.firstChild;
                 console.log(doorselected.id);
                let selectTask=this.doorstatus.filter(x=>x.id==parseInt(doorselected.id))[0];
                // console.log(selectTask.id);
                if(selectTask.taskstatus=="OrderCatched"||selectTask.taskstatus=="Executing")
                {
                         //下达开门指令
                                     this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:selectTask.taskid
                                });
                     this.$Message.info('开门命令已下达');
                }
                else
                {
                    this.$Message.info('当前任务状态不能打开柜门');
                }
            
        }
        openDoor()
        {
            let doorselected = document.getElementsByClassName('DoorSelected');
            console.log(doorselected);
            if(doorselected.length>0)
            {
                //alert(this.currentID);
                 let selectTask=this.doorstatus.filter(x=>x.id==parseInt(doorselected[0].id))[0];
                if(selectTask.taskstatus=="OrderCatched"||selectTask.taskstatus=="Executing")
                {
                    this.$Message.info('开门命令已下达');
                    //执行开门程序
                                this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:this.currentID
                                });                                
                    // let lockScreenBack = document.getElementById('lock_screen_back') as HTMLElement;
                    // if(lockScreenBack){
                    // lockScreenBack.style.transition = 'all 1s';
                    // lockScreenBack.style.zIndex = "10000";
                    // lockScreenBack.style.boxShadow = '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset';
                    // Cookies.set('taskid', this.currentID);
                    // Cookies.set('last_page_name', 'stockinstepctn');
                    // //console.log(Cookies.get('facing'));
                    // setTimeout(() => {
                    //     lockScreenBack.style.transition = 'all 0s';
                    //     this.$router.push({
                    //         name: 'opendoor'
                    //     });
                    // }, 800);
                    // }
                }
                else
                {
                    this.$Message.info('当前任务状态不能打开柜门');
                }
            }
            // else
            // {
            //     this.$Message.warning('请先选择柜门。');
            // }


        }

        getenumv(k:string)
        {
            for (let index = 0; index < this.enumdata.length; index++) {
                const element = this.enumdata[index];
                if(element.key==k)
                {
                    return element.value;
                }
            }
            return k;
        }
                get userName(){
          return this.$store.state.session.user?this.$store.state.session.user.userName:''
        }
        logout()
        {   
            this.$store.commit('app/logout', this);
            Util.abp.auth.clearToken();
            Cookies.set('facing', '0');
            Cookies.set('userId', '0');
            //location.reload();
            //  this.$router.push({
            //     name: 'login'
            // });
            this.$router.push({
                name: '/'
            });

        }
        autologout()
        {
             console.log('autologout:');
            for (let dd = 0; dd < this.doorstatus.length; dd++)  
            {
                //console.log(this.doorstatus[dd].taskstatus);
                if(this.doorstatus[dd].taskstatus!="")
                {
                    // console.log(this.doorstatus[dd].taskstatus);
                    return;
                }
            }
            //20211025 增加还有任务时界面不退出
            if(this.doorlist.filter(n=>n.manageTypeCode=='NPFullStockIn').length>0)
            {
                // console.log(this.doorlist.length);
                return;
             }
            //  console.log('autologout:');
            //  console.log(this.doorstatus);
            //  console.log('autologout:'+this.doorlist.length);
            //没有任务时退出
            this.logout();
        }

        tempTaskExpId:number=0;//缓存任务ID
        //获取WCS任务异常
         async getWcstaskException()
        {
            // console.log(this.tempTaskExpId);
                 await this.$store.dispatch({
                        type:'stocktask/getWcsTaskException',                          
                  })
                  .then(async (response) => {
                  console.log(response.data.result);
                  var wcsTasks= response.data.result.items as TaskFaultDto[];
                  if(wcsTasks.length==0)
                  {
                    //   alert(11);
                  }
                  else
                  {
                      if(this.tempTaskExpId!=wcsTasks[0].controlTaskId)
                      {
                          this.tempTaskExpId=wcsTasks[0].controlTaskId;
                          this.taskExpModalShow=true;
                          //语音报警
                          try{
                            // @ts-ignore:无法被执行的代码的错误
                            bound.speakmsg("任务异常报警")
                          }catch(error){

                          }
                      }    
                  }
            //    {}
            })
            .catch(function (error) {
            //   alert("龙门回零接口调用失败");
              console.log(error);
            });
        }

        openTaskExps()
        {
            this.taskExpModalShow=true;
        }
        get alertCount()
        {
            return this.$store.state.stocktask.alertCount;
        }

         //更新门状态
        updateDoor()
        {

                 for (let dd = 0; dd < this.doorstatus.length; dd++)  
                 {
                            this.doorstatus[dd].rfid="";
                            this.doorstatus[dd].taskstatus="";
                            this.doorstatus[dd].taskid="";
                            this.doorstatus[dd].taskEcell="";
                            let doordiv = document.getElementById(this.doorstatus[dd].id.toString());
                            //console.log(this.doorstatus[dd].id.toString());
                            doordiv.classList.remove('DoorOHTask');
                            doordiv.classList.remove('DoorSelected');
                            doordiv.classList.add('DoorClose');
                 } 
                 //修改龙门提示
                 if(this.doorlist.length>0)
                 {
                     this.currentpg=0;
                 } 
                for (let index = 0; index < this.doorlist.length; index++) {
                for (let dd = 0; dd < this.doorstatus.length; dd++) 
                  {       
                    if(this.doorlist[index].startCellName==this.doorstatus[dd].id.toString())
                    {
                            this.doorstatus[dd].rfid=this.doorlist[index].archiveBoxRfid;
                            this.doorstatus[dd].taskstatus=this.doorlist[index].manageStatus;
                            this.doorstatus[dd].taskid=this.doorlist[index].id;
                            this.doorstatus[dd].taskEcell=this.doorlist[index].endCellName;
                            let doordiv = document.getElementById(this.doorstatus[dd].id.toString());
                            doordiv.classList.add('DoorOHTask');    
                            //this.currentpg=index;
                             //修改龙门提示
                             if(this.doorlist[index].manageStatus=='RobotWait')    
                             {
                                 this.currentpg=index;
                             }                       
                    }
                }


            }
        }


   
            
        get list(){
            return this.$store.state.stocktask.list;
        };
        get list2(){
            return this.$store.state.stocktask.stocktasklists;
        };
        get doorlist(){
            return this.$store.state.board.doorstocktasks;
        };
        get queueCount(){
            return this.$store.state.stocktask.queueCount;
        };
        get stocktask(){
            //return this.$store.state.stocktask.stocktask;
            //console.log(this.$store.state.stocktask.list);
            if(this.$store.state.stocktask.list.length>0)
            {
                return this.$store.state.stocktask.list[0];
            }
            else
            {
                return new Stocktask();
            }
        };
        get stationlist(){
            return this.$store.state.cell.stations;
        };
        get loading(){
            return this.$store.state.stocktask.loading;
        }
  

    //    async getTask()
    //     {
    //         this.getpage();
    //         this.current=1;
      
    //     }
        get currentPage(){
            return this.$store.state.stocktask.currentPage;
        }
        pageChange(page:number){
            //console.log("改变页面"+page);
            this.$store.commit('stocktask/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('stocktask/setPageSize',pagesize);
            this.getpage();
        }
        //临时rfid号
        temprfid:string="";
        temprfidDoor:string="";
        temprfidScan:string="";
        temprfidComplete:string="";
        getStep(status:string)
        {

                if(status=="Executing"||status=="OrderCatched")
                {
                    this.current=0;
                    this.stepdata[0].content="请扫描档案盒上的二维码、开柜门";
                    console.log("请扫描档案盒上的二维码。");
                    if(this.temprfidScan!=this.currentRfid)
                    {
                        this.temprfidScan=this.currentRfid;
                        //后续再启用
                        // this.openPutTip();
                        try {
                            // @ts-ignore：无法被执行的代码的错误
                            bound.speakmsg("请扫描档案盒上的二维码、开柜门");
                        } catch (error) {
                            
                        }
                    }
                }
                if(status=="StationOpen")
                {
                    this.current=1;
                    this.stepdata[0].content="已完成";
                    this.stepdata[1].content="请放入档案盒[二维码向内]，关柜门";
                    if(this.temprfidDoor!=this.currentRfid)
                    {
                        this.temprfidDoor=this.currentRfid;
                        //后续再启用
                        // this.openPutTip();
                        try {
                            // @ts-ignore：无法被执行的代码的错误
                            bound.speakmsg("请把档案盒贴有二维码的方向朝内放置，并关闭柜门。");
                        } catch (error) {
                            
                        }
                    }

                }
                if(status=="StationClose")
                {
                    this.current=2;
                    this.stepdata[0].content="已完成";
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="等待密集柜执行";
                    this.closePutTip();

                }
                if(status=="CabinetWait")
                {
                    this.current=2;
                    this.stepdata[0].content="已完成";
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="密集柜工作中";
                    this.closePutTip();

                }
                if(status=="CabinetComplete")
                {
                    this.current=3;
                    this.stepdata[0].content="已完成";
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="等待机械手取档案";
                    this.closePutTip();
                }
                if(status=="RobotWait")
                {
                    this.current=3;
                    this.stepdata[0].content="已完成";
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="机械手执行中";
                    //增加龙门运动开始语音
                    try {
                        console.log('龙门语音提示:'+this.temprfid+'--'+this.currentRfid);
                        if(this.temprfid!=this.currentRfid)
                        {
                                this.temprfid=this.currentRfid;
                            // @ts-ignore：无法被执行的代码的错误
                            bound.speak(this.currentRfid,"龙门机械手抓取中");
                        }
                    } catch (error) {
                        
                    }


                }
                if(status=="RobotComplete")
                {
                    this.current=4;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="已完成";
                    try {
                        if(this.temprfidComplete!=this.currentRfid)
                        {
                                this.temprfidComplete=this.currentRfid;
                            // @ts-ignore：无法被执行的代码的错误
                            bound.speakmsg("任务已完成");
                        }
                    } catch (error) {
                        
                    }
                }
        }


        async taskAssign(){  
           //alert(this.currentRowRfid);
           
        //    await this.$store.dispatch({
        //         type:'stocktask/taskAssign',
        //         mainId:this.currentRowId
        //     });
        //    this.$Message.success('任务下达成功。');
        //    await this.getpage();
            if(this.stocktask.id>0)
            {
                await this.$store.dispatch({
                                type:'stocktask/taskAssign',
                                mainId:this.stocktask.id
                            });
                        this.$Message.success('任务下达成功。');
                        this.getpage();
            }
            else
            {
                alert('空任务。');
            }
        }
        async taskCancel(){  
            if(this.stocktask.id>0)
            {
                        await this.$store.dispatch({
                                type:'stocktask/taskCancelIn',
                                mainId:this.stocktask.id
                            });
                        this.$Message.success('任务取消成功。');
                        this.getpage();
            }
            else
            {
                alert('空任务。');
            }

        }
      async taskComplete()
      {  
            if(this.stocktask.id>0)
            {
                     this.$Modal.confirm({
                                        title:this.L('Tips'),
                                        content:this.L('异常情况下使用，且必须任务已经执行完毕，才能使用此项功能。'),
                                        okText:this.L('Yes'),
                                        cancelText:this.L('No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'stocktask/taskComplete',
                                                 mainId:this.stocktask.id
                                            })
                        this.$Message.success('任务完成。');
                        this.getpage();
                 }
                })
            }
            else
            {
                alert('空任务。');
            }

        }
        async getpage(){

            await this.$store.dispatch({
                type:'board/getAllInOutTask'
            })
            // console.log(this.doorlist.filter(n=>n.manageTypeCode=='NPFullStockIn').length);
            if(this.doorlist.filter(n=>n.manageTypeCode=='NPFullStockIn').length>0)
            {
                // console.log(this.currentRfid);
                    this.updateDoor();
                    // console.log(this.doorlist[this.currentpg].manageTypeCode);
                        this.currentID=this.doorlist[this.currentpg].id;
                        this.currentRfid=this.doorlist[this.currentpg].archiveBoxRfid;
                        this.currentScell=this.doorlist[this.currentpg].startCellName;
                        this.currentEcell=this.doorlist[this.currentpg].endCellName;
                        this.taskStatus=this.getenumv(this.doorlist[this.currentpg].manageStatus)
                        this.getStep(this.doorlist[this.currentpg].manageStatus);

             }
             else
             {
                    this.updateDoor();
                    this.current=0;
             }
                         //this.inputshow=false;
            (this.$refs.inputEle1 as any).focus();


            
                          
    }
                //连续扫码，自动下达。
                async getpageAuto(){
                                console.log("getpageAuto:"+this.currentRfid);
                                if(this.currentRfid!="")
                                {
                                await  this.$store.dispatch({
                                        type:'stocktask/getTask',
                                        rfid:this.currentRfid
                                        })   
                                this.taskStatus=this.getenumv(this.stocktask.manageStatus)
                                this.getStep(this.stocktask.manageStatus);
                                //console.log(this.stocktask.manageStatus);
                                //如果任务未下达  进行判断 并执行自动下达
                                if(this.stocktask.manageStatus=="WaitingExecute")
                                {
                                        if(this.stocktask.id>0)
                                        {
                                            await this.$store.dispatch({
                                                            type:'stocktask/taskAssign',
                                                            mainId:this.stocktask.id
                                                        });
                                            this.$Message.success('任务下达成功。');
                                            this.getpage();
                                        }
                                }
                                else
                                {
                                    //alert("任务状态为已下达！")
                                     this.$Message.info("任务状态为已下达！");
                                    this.getpage();
                                }

                        } 
                 }
        //定时工作
          async  timedJob () {

               this.getpage();
               //20220915 防止任务重复下达去除自动下达
            //    if(this.taskAssignFlag)//20220907 防止任务重复下达
            //    {
            //        return;//20220907
            //    }
            // await this.$store.dispatch({
            //     type:'stocktask/taskAutoAssign'
            // }).then( async (response) => {
            //             //    this.$Message.info('已创建新的入库任务');                           
            //                return;
            //             })
            //             .catch( (error) => {
            //                 console.log(error);
            //                 this.logout();
            //             }); 

    }
    intervalIdstatus:number=0;
        async created(){
            console.log(abp.localization.currentLanguage.name);
                        let lockScreenBack = document.getElementById('lock_screen_back');
                        if(lockScreenBack!=null)
                        {
            lockScreenBack.style.zIndex = "-1";
            lockScreenBack.style.boxShadow = '0 0 0 0 #667aa6 inset';
                        }

               this.getpage();
                    //  await this.$store.dispatch({
                    //     type:'cell/getStations'
                    // });


                this.intervalId= setInterval(this.timedJob, 3000);
                                 //20211025 增加无任务退出得时间
                this.intervalId3= setInterval(this.autologout, 3600000);
                //220321 增加获取WCS异常任务
                this.intervalIdwcs= setInterval(this.getWcstaskException, 5000);

                this.intervalIdstatus= setInterval(this.checkStasus, 20000);
             //this.getRobotL();
        }
        async destroyed()
        {
            window.clearInterval(this.intervalId);
            window.clearInterval(this.intervalId3);
            window.clearInterval(this.intervalIdwcs);
            window.clearInterval(this.intervalIdstatus);
        }
        checkStasus () {
        if(!this.statusModalShow)
        {
            axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetWMSServerStatus')
            .then(async (response) => {
                if (response.data.result) {
                this.serviceConnect = true;
                } else {
                this.serviceConnect = false;
                this.openStatus2();
                }
            })
            .catch((error) => {
                this.serviceConnect = false;
                this.openStatus2();
            });
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
            }
          })
          .catch((error) => {
            // console.log(error);
            this.wcsStatusFlag=false;
              this.openStatus2();
              // alert("WCS后台服务器连接异常，请检查服务器");

          });
          //检查密集架服务状态
              axios.get('http://192.168.1.188:21021/api/services/app/DAClient/GetMJJServerStatus?mjjName=DAK')
          .then(async (response) => {
            if (response.data.result) {
              this.mjgStatusFlag = true;
            } else {
              this.mjgStatusFlag = false;
              this.openStatus2();
            }
          })
          .catch((error) => {
            this.mjgStatusFlag = false;
            this.openStatus2();
          }); 
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

              } else {
                if (nodes.find((f) => f.nodeName == "EmergencyStop").value == "0") {
                  this.plcStatusFlag = false;
                  this.openStatus2();
                  // alert("设备急停被拍下");
                }
                else
                {
                  this.plcStatusFlag = true;
                }
              }
            })
            .catch(async (error) => {
              // console.log(error);
              this.plcStatusFlag = false;
              this.openStatus2();

            });
        }


    }
    openStatus2 () {
      if(!this.neverTip){
        this.openStatus();
      }
    }
        async mounted () {
            // (this.$refs.inputEle as any).focus();
            //  (this.$refs.inputEle1 as any).focus();

                                    //  this.scan();
                  this.changekeys();//切换输入法

              //this.inputshow=false;
                // var e = document.createEvent('MouseEvent');
                // e.initEvent('click', false, false);
                // this.$refs.test.$el.dispatchEvent(e);
                    //  var inputEle = document.getElementById("inputEle");
                    // //  inputEle.dispatchEvent(e);

                    // //  inputEle.addEventListener('click', function (e) {
                    // // }, false);
                //       var inputEle1 = document.getElementById("12001");
                //     //  inputEle1.dispatchEvent(e);
                //      inputEle1.addEventListener('click', function (e) {
                //          //var aa = document.getElementById("inputEle");
                //         //  alert(e.);
                //         // inputEle1.focus();
  
                //         // (inputEle1 as HTMLInputElement).value = e.x.toString();
                //     }, false);
                //                     var e = document.createEvent('MouseEvent');
                // e.initEvent('click', true, true);
                // inputEle1.dispatchEvent(e);
                    //  inputEle.dispatchEvent(e);
                    // this.fireKeyEvent(inputEle1,"keydown","1");
                    // alert("1");
                    // this.$Message.info("请先扫码");

            //                                          var inputEle1 = document.getElementById("inputEle1");
            //          inputEle1.addEventListener('keydown', function (e) {
                        
            //             // alert(e.code);
            //             // (inputEle1 as HTMLInputElement).value = e.key;
            //         }, false);
            //                     inputEle1.focus();
            // var KeyboardEventInit = {key:"", code:"MetaLeft", location:0, repeat:false, isComposing:false};
            // var evtObj = new KeyboardEvent("keydown", KeyboardEventInit);
            // inputEle1.dispatchEvent(evtObj);
            // inputEle1.dispatchEvent(evtObj);
                    // this.fireKeyEvent(inputEle1,"keydown","MetaLeft");

        }
    }

    
</script>
<style scoped>
.page-body{
      height: 800px;  
}
.xwblogo{
    width: 32px;
    height: 32px;

}
.rect{
    width:30px;
    height: 90px;
    /* fill="#ebedf0" */

}
.rectC{
    width:70px;
    height: 138px;
    /* fill="#ebedf0" */

}
.rectsample{
    width:66px;
    height: 25px;
    /* fill="#ebedf0" */

}
.cabinet
{
    width:200px;
    height: 60px;
    stroke-width:1.5;
    stroke:#dcdee2;
    fill:none;
    /* fill="#ebedf0" */
}
.cabinet_box
{
       width:13px;
    height: 30px;
    stroke-width:1;
    stroke:#dcdee2;
    fill:none; 
}
.Full{
   fill:aqua;
       background: url(../../../images/box.svg) 12px 7px no-repeat; 
            background-size: 50px 50px; 
    /* fill="#ebedf0" */

}
.Nohave{
   fill:#f3f3f3;

}
.Selected{
   fill:#57a3f3;

}
.door
{
    text-align:center;
    height:248px;
    width:154px;
    margin-left: 40px;
    padding-top: 40px;
}
.DoorClose{
  background:#f3f3f3;

}
.DoorOHTask{

   background:#57a3f3;
}
.DoorOpen{
   background:#f02d37;

}
.DoorSelected{

   background:#2d8cf0;
}



.margin-top-20 {
    margin-top: 20px;
}

.margin-top-cm {
    margin-top: 50px;
}
     .cardtitle{
    font-size: 16px;
    /* color: rgba(0,0,0,.85); */
    /* color:#e8eaec; */
    /* font-family: "Myriad Pro","Helvetica Neue",Arial,Helvetica,sans-serif; */
    font-weight: 600;
    font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB", "Microsoft YaHei", "\5FAE\8F6F\96C5\9ED1", Arial, sans-serif;
    line-height: 1.5;
    color: #515a6e;
    position: relative;
    top: 2px;
  }
  .taskstep
  {
      margin-left: 60px;
  }
  .doortitle
  {
      margin-bottom: 10px;
  }

</style>
<style   lang="less">
//解决STEP 背景色显示BUG   注意去掉 上面<style >里的 scoped
#taskstep
{
    // background: #57a3f3;
    //      div{
    // background: #57a3f3;
    //      }
}
#building{
  background:url("../../../images/background.jpg");
  width:100%;
  height:100%;
  position:fixed;
  background-size:100% 100%;
}
h1{
  color: #fff;
}
</style>