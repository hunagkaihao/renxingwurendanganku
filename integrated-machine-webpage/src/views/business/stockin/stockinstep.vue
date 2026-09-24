<style lang="less">
    @import "../stocktask.less";
</style>
<template>
    <div>
        <Card dis-hover style="height:720px">
            <div class="page-body">
                <div class="margin-top-1">
                        <div class="main-header">
                             <nav class="navbar-default" > <h1>存档</h1> </nav>
                            <div class="header-avator-con">     

                                <div class="user-dropdown-menu-con">
                                    <Row type="flex" justify="end" align="middle" class="user-dropdown-innercon">
                                        <Dropdown transfer trigger="click">
                                            <a href="javascript:void(0)">
                                                <span class="main-user-name">{{ userName }}</span>
                                                <Icon type="arrow-down-b"></Icon>
                                            </a>
                                        </Dropdown>
                                        <span class="avatar" style="background: #619fe7;margin-left: 40px;"><img src="../../../images/usericon.jpg" /></span>
                                           <Icon @click="logout" type="ios-power" size="30" style="margin-left: 10px;" color="LightSkyBlue" />

                                    </Row>
                                </div>

                            </div>
                        </div>

                </div>
                <div v-show="current==1||continuousFlag" class="margin-top-20">
                    <Form ref="queryForm" :label-width="80" label-position="left" inline>
                        <Row :gutter="24">
                            <Col span="6" offset="8">
                                    <Input  v-model="currentRfid" required :placeholder="L('条码标签')" @keyup.enter.native="scan">
                                    <Icon type="md-barcode" slot="suffix"  @click="scan" />
                                    </Input>
                            </Col>
                             <Col span="3">
                                 <!-- <Button shape="circle" icon="ios-search" @click="getpage"></Button> -->
                            </Col>
                              <Col span="6">

                            </Col>
                        </Row>
                    </Form>
                </div>
                 <div v-show="current>1" class="margin-top-20">
                    <Form ref="queryForm1" :label-width="40" label-position="right" inline>
                       <Row>
                        <Col  v-show=false  span="4">  
                          <FormItem :label="L('Id')+':'" > 
                       <Input v-model="stocktask.id" disabled></Input>
                          </FormItem>
                        </Col>
                        <Col span="6">
                         <FormItem :label="L('条码')+':'" > 
                       <Input v-model="stocktask.archiveBoxRfid" disabled></Input>
                         </FormItem>
                       </Col>
                       <Col span="6">
                            <FormItem :label="L('起点')+':'" > 
                       <Input v-model="stocktask.startCellName" disabled></Input>
                         </FormItem>
                       </Col>
                       <Col span="6">
                         <FormItem :label="L('终点')+':'" > 
                       <Input v-model="stocktask.endCellName" disabled></Input>
                       </FormItem>
                       </Col>
                        <Col span="6">
                        <FormItem :label="L('状态')+':'" > 
                       <Input v-model="taskStatus" disabled></Input>
                       </FormItem>
                       </Col>
                       </Row>
                      </Form>
                  </div>
                <div v-show="current>1&&!continuousFlag" >
                        <Row>
                            <Col span="14" offset="8">
                            <!-- <Button v-show="current==1" shape="circle"  icon="ios-search" @click="getpage"></Button> -->
                            <ButtonGroup>
                                <Button v-show="current>1" type="primary" @click="goBack"  icon="ios-skip-backward" size="large"></Button>
                                 <Button v-show="current==2" icon="ios-apps-outline" type="primary" size="large" @click="editCell" class="toolbar-btn">{{L('指定库位')}}</Button>
                                <Button  v-show="current==2" icon="ios-expand" type="primary" size="large" @click="editStation" class="toolbar-btn">{{L('指定柜格')}}</Button>
                                <Button v-show="current==2" icon="ios-download" type="primary" size="large" @click="taskAssign">{{L('下达任务')}}</Button>
                                <Button  v-show="current>1&&current<4" icon="ios-remove" type="primary" size="large" @click="taskCancel"></Button>
                                <Button  v-show="current>2" icon="ios-refresh" type="primary" size="large" @click="getpage"></Button>
                                <Button  v-show="current>2" icon="ios-checkbox" type="primary" size="large" @click="taskComplete" ></Button>
                                <Button v-show="current>1" type="primary"  @click="goForward" icon="ios-skip-forward" size="large"></Button>
                            </ButtonGroup>
                            </Col>
                            <!-- <Button v-show="current==2" icon="ios-download" type="primary" size="large" @click="taskAssign" class="toolbar-btn">{{L('下达任务')}}</Button>
                            <Button  v-show="current>1" icon="ios-search" type="primary" size="large" @click="taskCancel" class="toolbar-btn">{{L('取消任务')}}</Button>
                            <Button  v-show="current>2" icon="ios-search" type="primary" size="large" @click="taskComplete" class="toolbar-btn">{{L('强制完成')}}</Button>
                            <Button v-show="current==3" icon="ios-search" type="primary" size="large" @click="editCell" class="toolbar-btn">{{L('更新库位')}}</Button>
                            <Button  v-show="current==2" icon="ios-search" type="primary" size="large" @click="editStation" class="toolbar-btn">{{L('更新柜格')}}</Button> -->
                        </Row>
                </div>
                 <div v-show="current>=1&&!continuousFlag"  class="margin-top-20">
                     <Steps :current="current">
                        <Step v-for="(item) of stepdata" :key="item.id"  :title="item.title" :icon="item.icon"  :content="item.id<(current+1)?'已完成':item.content" ></Step>
                    </Steps>
                </div> 
                <!-- <div v-show="current==1||continuousFlag"  class="margin-top-10">
                     <Row>
                        <Col span="16" offset="4">
                            <Steps :current="current">
                                <Step v-for="(item) of stepdata1" :key="item.id"  :title="item.title" :icon="item.icon"  :content="item.id<(current+1)?'已完成':item.content" ></Step>
                            </Steps>
                         </Col>
                           <Col v-show="current>1&&continuousFlag" span="4">
                            <Steps :current="current">
                                <Step v-for="(item) of stepdata2" :key="item.id"  :title="item.title" :icon="item.icon"  :content="item.id<(current+1)?'已完成':item.content" ></Step>
                            </Steps>
                         </Col>
                      </Row>
                </div> -->
                 <div class="margin-top-20">
                   <Row :gutter="24">
                    <Col v-show="current>1&&current<6&&!continuousFlag" span="8">
                        <h4>入库柜门</h4>
                         <div class="margin-top-10">
                                <svg width="223" height="150"  viewBox="0 0 146 100" class="js-station-graph-svg">
                            
                                    <g >
                                        <rect id="rect_1" height="100" width="146" y="0" x="0" stroke-width="1.5" stroke="#dcdee2"  fill="#fff"></rect>
                                        <rect  stroke-width="1" stroke="#dcdee2" @click="showBTT(item.cellCode,item.id)"  v-for="(item,i) in stationlist"  :key="item.id" :class="'rect '+item.cellStatus+' '+item.runStatus" :x=i*35+5 :y=5  :data-id="item.id" :id="item.cellCode" ></rect>
                                    </g>                    
                            
                                <text    v-for="(item,i) in stationlist" :key="item.id" :x=i*35+15 :y="50" fill="#515a6e"  class="month">{{i+1}}</text>

                        </svg>
                                                            </div>
                    </Col>
                    <Col v-show="current==2&&!continuousFlag" span="12">
                                    <div>
                                        <h4>目标库位</h4>
                    <RadioGroup v-model="cellZYDto.cell_z" type="button" button-style="solid" @on-change="isSelectChange">
                         <Radio v-for="n in 7" :key="n" :label=n>{{n}}排</Radio>
                     </RadioGroup>
                                    </div>
                                    <div class="margin-top-10">
                     <RadioGroup v-model="cellZYDto.cell_y" type="button" button-style="solid" @on-change="isSelectChange1">
                         <Radio v-for="n in 5" :key="n" :label=n>{{n}}层</Radio>
                     </RadioGroup>
                                    </div>
                                    <div class="margin-top-10">
                     <RadioGroup  v-model="jie"  type="button"  @on-change="isSelectChange2">
                         <Radio v-for="n in 3" :key="n" :label=n>{{n}}节</Radio>
                     </RadioGroup>
                                    </div>
                                    <div class="margin-top-10">

                               <svg width="460" height="100"  viewBox="0 0 460 100" class="js-calendar-graph-svg">
                         
                                <g >
                                    <rect id="rect_1" height="100" width="460" y="0" x="0" stroke-width="1.5" stroke="#dcdee2"  fill="#fff"></rect>
                                    <rect stroke-width="1" stroke="#dcdee2"  @click="showBTTcell(item.cellCode,item.id)" v-for="item in listcells.filter(a=>a.runStatus=='Enable'&a.cellStatus=='Nohave'&a.cell_x<=13*jie&a.cell_x>13*(jie-1))" :key="item.id" :class="'rect '+item.cellStatus+' '+item.runStatus" :x=(item.cell_x-1-13*(jie-1))*35+5 :y=5  :data-id="item.id" :id="item.cellCode"  ></rect>
                                </g>                    
                        
                               <text   v-for="item in listcells.filter(a=>a.runStatus=='Enable'&a.cellStatus=='Nohave'&a.cell_x<=13*jie&a.cell_x>13*(jie-1))" :key="item.id" :x=(item.cell_x-1-13*(jie-1))*35+15 :y="50" fill="#515a6e"  class="month">{{item.cell_x}}</text>

                    </svg>
                                                        </div>
                       </Col>
                        <Col v-show="current==4&&!continuousFlag" span="12" >
                     <h4>密集柜</h4>
                    <div  id="achiveBox1" class="achiveBox" :style="{'left': (0*boxLocationL+100)+'px'}">
                    <svg  width="60px" height="160px" viewBox="0 0 120 320" xmlns="http://www.w3.org/2000/svg">
                    <!-- Created with Method Draw - http://github.com/duopixel/Method-Draw/ -->
                    <g>
                    <title>Layer 1</title>
                    <rect id="svg_1" height="313.00001" width="111" y="0" x="0" stroke-width="2" stroke="#00BFFF" fill="none"/>
                    <rect id="svg_2" height="78" width="109" y="68" x="1.5" stroke-width="2" fill="#00BFFF"/>
                    <ellipse ry="10.5" rx="10.5" id="svg_4" cy="175" cx="55" stroke-width="5" stroke="#00BFFF" fill="none"/>
                    </g>
                    </svg></div>
                    <div id="achiveBox2" class="achiveBox" :style="{'left': (boxLocationL+100)+'px'}">
                        <svg  width="60px" height="160px" viewBox="0 0 120 320" xmlns="http://www.w3.org/2000/svg">
                        <!-- Created with Method Draw - http://github.com/duopixel/Method-Draw/ -->
                        <g>
                        <title>Layer 1</title>
                        <rect id="svg_1" height="313.00001" width="111" y="0" x="0" stroke-width="2" stroke="#00BFFF" fill="none"/>
                        <rect id="svg_2" height="78" width="109" y="68" x="1.5" stroke-width="2" fill="#00BFFF"/>
                        <ellipse ry="10.5" rx="10.5" id="svg_4" cy="175" cx="55" stroke-width="5" stroke="#00BFFF" fill="none"/>
                        </g>
                    </svg></div>
                    <div id="achiveBox3" class="achiveBox" :style="{'left': (2*boxLocationL+100)+'px'}"  @dblclick="moveBox3">
                        <svg  width="60px" height="160px" viewBox="0 0 120 320" xmlns="http://www.w3.org/2000/svg">
                        <!-- Created with Method Draw - http://github.com/duopixel/Method-Draw/ -->
                        <g>
                        <title>Layer 1</title>
                        <rect id="svg_1" height="313.00001" width="111" y="0" x="0" stroke-width="2" stroke="#00BFFF" fill="none"/>
                        <rect id="svg_2" height="78" width="109" y="68" x="1.5" stroke-width="2" fill="#00BFFF"/>
                        <ellipse ry="10.5" rx="10.5" id="svg_4" cy="175" cx="55" stroke-width="5" stroke="#00BFFF" fill="none"/>
                        </g>
                    </svg></div>
                    <div id="achiveBox4" class="achiveBox" :style="{'left': (3*boxLocationL+100)+'px'}" @dblclick="moveBox('1100')">
                        <svg  width="60px" height="160px" viewBox="0 0 120 320" xmlns="http://www.w3.org/2000/svg">
                        <!-- Created with Method Draw - http://github.com/duopixel/Method-Draw/ -->
                        <g>
                        <title>Layer 1</title>
                        <rect id="svg_1" height="313.00001" width="111" y="0" x="0" stroke-width="2" stroke="#00BFFF" fill="none"/>
                        <rect id="svg_2" height="78" width="109" y="68" x="1.5" stroke-width="2" fill="#00BFFF"/>
                        <ellipse ry="10.5" rx="10.5" id="svg_4" cy="175" cx="55" stroke-width="5" stroke="#00BFFF" fill="none"/>
                        </g>
                    </svg></div>

                       </Col>
                       <Col  v-show="current==5&&!continuousFlag"  span="12" >
                          <h4>机械手</h4>
                          <svg width="720" height="540"  viewBox="0 0 800 600" xmlns="http://www.w3.org/2000/svg">
                                <g >
                                    <rect class="cabinet" v-for="n in 4" :key="'index1_' + n"  :y=65*(n-1)+getRobotLL(n-1) x="0"  ></rect>
                                    <rect class="cabinet" v-for="n in 4" :key="'index2_' + n"   :y=65*(n-1)+getRobotLL(n-1)  x="202"  ></rect>
                                    <rect class="cabinet" v-for="n in 4" :key="'index3_' + n"  :y=65*(n-1)+getRobotLL(n-1) x="404"  ></rect>
                                    <rect :id="'achive'+ n"  class="cabinet_box" v-for="n in 13" :key=n :y=65*(Math.floor(cellZ/2))+2+cellZ%2*25+getRobotC(cellZ) :x=(jie-1)*202+3+(n-1)*15  ></rect>
                                    <text xml:space="preserve" text-anchor="start" font-family="Helvetica, Arial, sans-serif" font-size="24" id="svg_1" y="380" x="260" stroke-width="0" stroke="#dcdee2" fill="#dcdee2">操作台</text>
                                    <rect id="cabinetsvg_2" height="49" width="135" y="350" x="230" stroke-width="1.5" stroke="#dcdee2" fill="none"/>
                                </g>  
                        </svg>
                     </Col>
                     </Row>
                </div>
            </div>
        </Card>
     </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import Stocktask from '@/store/entities/stocktask'
    import Cookies from 'js-cookie';
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
    class CellZYDto {      
    cell_z: number=4;
    cell_y: number=1;      
    }
    
    @Component({
        components:{}
    })
    export default class Stocktasks extends AbpBase{
                taskAssignDto:TaskAssignDto=new TaskAssignDto();
        cellZYDto:CellZYDto=new CellZYDto();
                scellId:number=0;
                scellcode:string="";
                dcellId:number=0;
                dcellcode:string="";
                jie:number=1;
        //filters
        pagerequest:PageStocktaskRequest=new PageStocktaskRequest();
        creationTime:Date[]=[];

        createModalShow:boolean=false;
        editModalShow:boolean=false;
        updateCellModalShow:boolean=false;
        updateStationModalShow:boolean=false;
        currentRowId:number=0;
        currentRfid:string="";
        currentpg:number=1;//当前页
        //密集柜的初始位置
        boxLocationL:number=60;
        box2:boolean=true;
        box3:boolean=true;
        box1:boolean=true;
        cabinetA:boolean=true;//密集柜动画执行标识
        continuousFlag:boolean=false;//连续入库标识
        currentRowRfid:string="";
                stepdata=[{ id:'1',title:'身份验证',
            content:'已完成',icon:'md-finger-print'},{ id:'2',title:'档案扫码',
            content:'等待扫码',icon:'md-barcode'},{ id:'3',title:'柜门指定',
            content:'',icon:'ios-list-box'},{ id:'4',title:'档案放入',
            content:'',icon:'md-log-in'},{ id:'5',title:'密集柜开启',
            content:'',icon:'ios-albums'},{ id:'6',title:'机械手抓取',
            content:'',icon:'md-download'},{ id:'7',title:'任务结束',
            content:'',icon:'ios-flag'}];
            //连续入库步骤
            stepdata1=[{ id:'1',title:'身份验证',
            content:'已完成',icon:'md-finger-print'},{ id:'2',title:'档案扫码',
            content:'等待扫码',icon:'md-barcode'},{ id:'3',title:'柜门指定',
            content:'',icon:'ios-list-box'},{ id:'4',title:'档案放入',
            content:'',icon:'md-log-in'}];
             //当前任务状态
            stepdata2=[{ id:'1',title:'机械手抓取',
            content:'',icon:'md-download'}];
                        cabinetdata=[{ id:1,value:'0-50-50-50'},{ id:2,value:'0-50-50-50'}
            ,{ id:3,value:'0-0-50-50'},{ id:4,value:'0-0-50-50'},
            { id:5,value:'0-0-0-50'},{ id:6,value:'0-0-0-50'}
            ,{ id:7,value:'0-0-0-0'}];
            //柜门任务状态
            doorstatus=[{ id:12001,rfid:'80001',cclass:'',status:'关闭',taskstatus:'CabinetWait'},
            { id:12002,rfid:'80002',cclass:'',status:'关闭',taskstatus:'aa'}
            ,{ id:12003,rfid:'80003',cclass:'',status:'关闭',taskstatus:'aa'},
            { id:12002,rfid:'80004',cclass:'',status:'关闭',taskstatus:'aa'}];
        enumdata=[{ value:'All',
            key:''},{ value:'入库',
            key:'NPFullStockIn'},{ value:'等待执行',
            key:'WaitingExecute'}
            ,{ value:'执行中',
            key:'Executing'}
            ,{ value:'指令已下达',
            key:'OrderCatched'}
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
            key:'RobotComplete'}]
        current:number=1;
        taskStatus:string="";
        robotLocation=[0,0,0,0];        
        getRobotL()
        {
            for (let index = 0; index < this.robotLocation.length; index++) {
                 if(index+1>Math.floor((this.cellZYDto.cell_z+1)/2))
                 {
                    this.robotLocation[index]=50;
                     //console.log(this.cellZYDto.cell_z);
                     //console.log( Math.sign((index+1)-Math.floor((this.cellZYDto.cell_z+1)/2)));
                 }
                 else
                 {
                     //console.log(this.cellZYDto.cell_z);
                     //console.log( Math.sign((index+1)-Math.floor((this.cellZYDto.cell_z+1)/2)));
                     this.robotLocation[index]=0;
                 }
                
            }

            console.log(this.robotLocation);
        }
        getRobotLL(k:number)
        {
            let l=this.cabinetdata.filter(x=>x.id==this.cellZ)[0].value.split('-');
            // let a=(k+1)-Math.floor((this.cellZ+1)/2);
            // let b=(a+Math.abs(a))/2;
            // let c=Math.sign(b)*50;
            //console.log(this.cellZ+":"+l);
            //console.log(k+":"+l[k]);

            return parseInt(l[k]);                
        }
       getRobotC(k:number)
        {
            let a1 = 0;
            if( k%2 == 0 )
            {
                a1=50;
            }
            return a1;             
        }
        //扫码查询
        scan()
        {
            this.getpage();//单件入库
            // //多件模式暂时屏蔽
            // if(this.continuousFlag)
            // {
            //     this.getpageAuto();//连续入库
            // }
            // else
            // {
            //     this.$Modal.confirm({
            //                             title:this.L('Tips'),
            //                             content:this.L('是否连续入库'),
            //                             okText:this.L('Yes'),
            //                             cancelText:this.L('No'),
            //                             onOk:async()=>{
            //                                 this.continuousFlag=true;
            //                                 this.getpageAuto();//连续入库

            //                             },
            //                             onCancel:async()=>{
            //                                this.getpage();//单件入库
            //                             }
            //                     })
            // }
            
        }
        goBack()
        {

                // this.$Modal.confirm({
                //                         title:this.L('Tips'),
                //                         content:this.L('是否返回扫码页面'),
                //                         okText:this.L('Yes'),
                //                         cancelText:this.L('No'),
                //                         onOk:async()=>{
                //                          location.reload();//页面重新刷新
                //                         }
                //                 })
                 if(this.currentpg>1)
                 {
                     this.currentpg--;
                  this.pageChange(this.currentpg);
                // //  //this.currentpg=this.currentpg+1;
                }
                 else
                 {

                     this.pageChange(this.$store.state.stocktask.list.length);
                      this.currentpg=this.$store.state.stocktask.list.length;
                 }
            
        }

        goForward()
        {
            // if(this.current==2)
            // {
            //     this.$Modal.confirm({
            //                             title:this.L('Tips'),
            //                             content:this.L('是否自动下达任务'),
            //                             okText:this.L('Yes'),
            //                             cancelText:this.L('No'),
            //                             onOk:async()=>{
            //                                  this.taskAssign();
            //                             }
            //                     })

            // }
            // if(this.current>2)
            // {
            //     if(this.current>5)
            //     {
            //             this.$Modal.confirm({
            //                             title:this.L('Tips'),
            //                             content:this.L('是否返回扫码页面'),
            //                             okText:this.L('Yes'),
            //                             cancelText:this.L('No'),
            //                             onOk:async()=>{
            //                              location.reload();//页面重新刷新
            //                             }
            //                     })
            //         return;
            //     }
            //     this.getpage();
            // }
            if(this.current>0)
            {

                if(this.currentpg<=this.$store.state.stocktask.list.length)
                 {
                     if(this.$store.state.stocktask.list.length==1&&this.currentpg==1)
                     {
                     return;
                     }
                     this.currentpg++;
                  this.pageChange(this.currentpg);
                // //  //this.currentpg=this.currentpg+1;
                }
                 else
                 {

                     this.pageChange(1);
                      this.currentpg=1;
                 }

            }

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
                async isSelectChange(val:number){
            console.log(val);
            if(this.cellZYDto.cell_y>0&&val>0){
                  this.$store.dispatch({
                        type:'stocktask/getCells',
                        data:this.cellZYDto
                    });
            }
            this.taskAssignDto.cellcode="";
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
            location.reload();

        }
        async isSelectChange1(val:number){
            console.log(val);
            if(this.cellZYDto.cell_z>0&&val>0){
                  this.$store.dispatch({
                        type:'stocktask/getCells',
                        data:this.cellZYDto
                    });
            }
                       this.taskAssignDto.cellcode="";
        }
        async isSelectChange2(val:number){
            console.log(val);
            this.jie=val;
        }
        //选择目标库位
        showBTTcell(val:string,vid:number)
        {
            let stationDiv = document.getElementById(val);
            if(this.dcellId!=0 && this.dcellId==vid)
            {
                this.dcellId=0;
                this.dcellcode="";

                    //console.log(stationDiv);
                stationDiv.classList.remove('Selected');

            }
            else
            {
                //alert(val);
                if(this.dcellcode!="")
                {
                    let stationDivOld = document.getElementById(this.dcellcode);
                    if(stationDivOld!=null)
                    {
                       stationDivOld.classList.remove('Selected');
                    }
                }
                this.dcellId=vid;
                this.dcellcode=val;
                stationDiv.classList.add('Selected');
            }
        }
        //选择入库柜门
        showBTT(val:string,vid:number)
        {
            if(this.current>2)
            {
                return;
            }
            let cellDiv = document.getElementById(val);
            if(this.scellId!=0 && this.scellId==vid)
            {
                this.scellId=0;
                this.scellcode="";
                if(cellDiv!=null)
                {
                cellDiv.classList.remove('Selected');
                }
            }
            else
            {
                if(this.scellcode!="")
                {
                    let cellDivOld = document.getElementById(this.scellcode);
                    if(cellDivOld!=null)
                    {
                       cellDivOld.classList.remove('Selected');
                    }
                }
                //alert(val);
                this.scellId=vid;
                this.scellcode=val;
                if(cellDiv!=null)
                {
                cellDiv.classList.add('Selected');
                }
            }
        }
       async  selectedCell()
        {
            //if(this.stocktask.ma)
            if(this.stocktask.manageStatus!="RobotWait")
            {
                return;
            }
            console.log(this.stocktask.startCellName);
            if(this.stocktask.startCellName!="")
            {
                let stationDiv = document.getElementById(this.stocktask.startCellName);
                    console.log(stationDiv);
                    stationDiv.classList.add('Selected');
                    //this.box2=false;
            }
            if(this.stocktask.endCellName!=""&&this.current>=2)
            {

                let cell=this.stocktask.endCellName.split('-');
                console.log(cell);
                this.cellZYDto.cell_z=parseInt(cell[0]);
                this.cellZYDto.cell_y=parseInt(cell[2]);
                let aa=parseInt(cell[1])/13;
                let bb=parseInt(cell[1])%13;
                console.log(parseInt(cell[1])/13);
                this.jie=parseInt(aa.toString())+1;
                await  this.$store.dispatch({
                        type:'stocktask/getCells',
                        data:this.cellZYDto
                    });

                // let cellDiv = document.getElementById(this.stocktask.endCellName);
                // cellDiv.classList.add('Selected');
                if(this.stocktask.manageStatus=="RobotWait")
                {

                
                 console.log("转换:"+bb);
                let achiveDiv = document.getElementById("achive"+bb);
                achiveDiv.classList.add('Selected');
                 console.log("转换:"+achiveDiv);
                console.log("X:"+achiveDiv.getAttribute('x'));
//moveBox1.offsetLeft
                    var xx=parseInt(achiveDiv.getAttribute('x'));
                    var xparam = 650-xx;
                    var xparam1=340-xx;
                    var yparam = 40;
                    var yyy=0;
                    if(parseInt(cell[0])%2==0)
                    {
                     yparam = -40;
                    }
                    else
                    {
                       yparam = 40; 
                       yyy=80;
                    }

                    var yy = 320-parseInt(achiveDiv.getAttribute('y'))-yparam+yyy;
                    var style = document.styleSheets[0];
                    style.insertRule("@keyframes achive-move{0%{ transform: translate("+xparam1+"px,"+yy+"px);}30%{ transform: translate("+xparam+"px,"+yy+"px);}50%{ transform: translate("+xparam+"px,"+(yparam)+"px);}80%{ transform: translate(0px,"+yparam+"px); }100%{ transform: translate(0px,0px); }}",1);//写入样式
                    //style.insertRule("@keyframes achive-move{30%{top:30px;left:50px;} 100%{ top: 30px;left:250px;}}",1);//写入样式
                    achiveDiv.classList.add('aMovetoD');
                    //增加动画结束事件
                    achiveDiv.addEventListener("webkitAnimationEnd", this.myStartFunction);
                }

                //achiveDiv.classList.add('aMovetoR');
            }
             console.log("当前排："+this.stocktask.endCellName);
            console.log("当前排："+parseInt(this.stocktask.endCellName.substring(1,2)));
        }

        //更新门状态
        updateDoor()
        {

                 for (let dd = 0; dd < this.doorstatus.length; dd++)  
                 {
                            this.doorstatus[dd].rfid="";
                            this.doorstatus[dd].taskstatus="";
                 }  
                for (let index = 0; index < this.$store.state.stocktask.list.length; index++) {
                for (let dd = 0; dd < this.doorstatus.length; dd++) 
                  {       
                    if(this.$store.state.stocktask.list[index].startCellName==this.doorstatus[dd].id.toString())
                    {
                            this.doorstatus[dd].rfid=this.$store.state.stocktask.list[index].archiveBoxRfid;
                            this.doorstatus[dd].taskstatus=this.$store.state.stocktask.list[index].manageStatus;
                    }
                }

                    if(this.$store.state.stocktask.list[index].archiveBoxRfid==this.currentRfid&&this.currentRfid!="")
                    {

                            this.taskStatus=this.getenumv(this.$store.state.stocktask.list[index].manageStatus)
                            this.getStep(this.$store.state.stocktask.list[index].manageStatus);
                    }


            }
           //待补充
        }
        //动画执行标识
        myStartFunction()
        {
            //alert("动画执行完毕！");
        }
        async editCell(){
              this.taskAssignDto.cellcode=this.dcellcode;
              this.taskAssignDto.mainId=this.stocktask.id;
                await this.$store.dispatch({
                        type:'stocktask/taskUpdateOutCell',
                        data:this.taskAssignDto
                    });
              this.currentRfid=this.stocktask.archiveBoxRfid;
              this.getpage();
        }
        async editStation(){
              this.taskAssignDto.cellcode=this.scellcode;
              this.taskAssignDto.mainId=this.stocktask.id;
                await this.$store.dispatch({
                        type:'stocktask/taskUpdateInCell',
                        data:this.taskAssignDto
                    });
             this.currentRfid=this.stocktask.archiveBoxRfid;
             this.getpage();
        }
        moveBox(moverule:string)
        {
             console.log(moverule+this.box1);
            // let strbox=moverule.split('-');
            // console.log(strbox);
            // console.log(strbox[3]);
            if(moverule[0]=="1"&&this.box1)
            {
                let moveBox1 = document.getElementById('achiveBox1');
                    let box4left = moveBox1.offsetLeft;
                        console.log(box4left);
                    moveBox1.classList.remove('movetoR');
                    moveBox1.classList.remove('movetoR0');
                    moveBox1.classList.add('movetoL');
                    this.box1=false;
            }else if(moverule[3]=="2"&&!this.box1)
            {
            //console.log("moveBox4");
                let moveBox1 = document.getElementById('achiveBox1');
                    moveBox1.classList.remove('movetoL');
                    moveBox1.classList.remove('movetoL0');
                    moveBox1.classList.add('movetoR');
                    this.box1=true;
            }
            if(moverule[3]=="1"&&this.box3)
            {
                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoR');
                    moveBox3.classList.remove('movetoR0');
                    moveBox3.classList.add('movetoL');
                    this.box3=false;
            }else if(moverule[3]=="2"&&!this.box3)
            {
                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoL');
                    moveBox3.classList.remove('movetoL0');
                    moveBox3.classList.add('movetoR');
                    this.box3=true;
            }
            if(moverule[1]=="1"&&this.box2)
            {
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoR');
                                        moveBox2.classList.remove('movetoR0');
                    moveBox2.classList.add('movetoL');
                    this.box2=false;
            }else if(moverule[1]=="2"&&!this.box2)
            {
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoL');
                                        moveBox2.classList.remove('movetoL0');
                    moveBox2.classList.add('movetoR');
                    this.box2=true;
            }
        }
        updateCabinet()
        {
            console.log(this.deviceStatusDto.columnStatus);
             if(this.deviceStatusDto.columnStatus.indexOf("0")>=0)
            {
                return;
            }
            if(this.deviceStatusDto.columnStatus[3]=="2"&&this.deviceStatusDto.columnStatus[2]=="1")
            {

                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoR0');
                    moveBox3.classList.add('movetoL0');
                    this.box3=false;
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoR0');
                    moveBox2.classList.add('movetoL0');
                    this.box2=false;
                let moveBox1 = document.getElementById('achiveBox1');
                    moveBox1.classList.remove('movetoR0');
                    moveBox1.classList.add('movetoL0');
                    this.box1=false;
            }
            if(this.deviceStatusDto.columnStatus[2]=="2"&&this.deviceStatusDto.columnStatus[1]=="1")
            {
                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoL0');
                    moveBox3.classList.add('movetoR0');
                    this.box3=true;
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoR0');
                    moveBox2.classList.add('movetoL0');
                    this.box2=false;
                let moveBox1 = document.getElementById('achiveBox1');
                    moveBox1.classList.remove('movetoR0');
                    moveBox1.classList.add('movetoL0');
                    this.box1=false;
            }
            if(this.deviceStatusDto.columnStatus[1]=="2"&&this.deviceStatusDto.columnStatus[0]=="1")
            {
                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoL0');
                    moveBox3.classList.add('movetoR0');
                    this.box3=true;
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoL0');
                    moveBox2.classList.add('movetoR0');
                    this.box2=true;
                let moveBox1 = document.getElementById('achiveBox1');
                    moveBox1.classList.remove('movetoR0');
                    moveBox1.classList.add('movetoL0');
                    this.box1=false;
            }
            if(this.deviceStatusDto.columnStatus[0]=="2")
            {
                let moveBox3 = document.getElementById('achiveBox3');
                    moveBox3.classList.remove('movetoL0');
                    moveBox3.classList.add('movetoR0');
                    this.box3=true;
                let moveBox2 = document.getElementById('achiveBox2');
                    moveBox2.classList.remove('movetoL0');
                    moveBox2.classList.add('movetoR0');
                    this.box2=true;
                let moveBox1 = document.getElementById('achiveBox1');
                    moveBox1.classList.remove('movetoL0');
                    moveBox1.classList.add('movetoR0');
                    this.box1=true;
            }
        }
        moveBox3()
        {
            //console.log("moveBox4");
            let moveBox3 = document.getElementById('achiveBox3');
                let box3left = moveBox3.offsetLeft;
                let box3left2 = moveBox3.style.left;
                console.log(box3left);
                console.log(box3left2);
            if(this.box3)
            {
                moveBox3.classList.remove('movetoL');
                moveBox3.classList.add('movetoR');
                let moveBox4 = document.getElementById('achiveBox4');
                       //console.log(moveBox4);
                moveBox4.classList.remove('movetoL');
                moveBox4.classList.add('movetoR');
                this.box3=false;
            }
            else
            {
                moveBox3.classList.remove('movetoR');
                moveBox3.classList.add('movetoL');
                let moveBox4 = document.getElementById('achiveBox4');
                       //console.log(moveBox4);
                moveBox4.classList.remove('movetoR');
                moveBox4.classList.add('movetoL');
            }
        }
        get list(){
            return this.$store.state.stocktask.list;
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
        get listcells(){
            //console.log(this.$store.state.stocktask.cells);
            return this.$store.state.stocktask.cells;
        };
        get stationlist(){
            return this.$store.state.cell.stations;
        };
         get deviceStatusDto()
      {
          return this.$store.state.board.deviceStatusDto;
      }
        get loading(){
            return this.$store.state.stocktask.loading;
        }
        get cellZ()
        {
            let cell="01-01-01".split('-');
              //console.log(Object.keys(this.stocktask).length);
            if(Object.keys(this.stocktask).length>0)
            {
                if(this.stocktask.endCellName!="")
                {
                 cell=this.stocktask.endCellName.split('-');
                }
            }

                // console.log(cell);
                // this.cellZYDto.cell_z=parseInt(cell[0]);
                // this.cellZYDto.cell_y=parseInt(cell[2]);
                let aa=parseInt(cell[1])/13;
                console.log(parseInt(cell[1])/13);
                this.jie=parseInt(aa.toString())+1;
           return parseInt(cell[0].substring(1,2));
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
        getStep(status:string)
        {

                if(status=="WaitingExecute")
                {
                    this.current=2;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="等待柜门指定";
                }
                if(status=="Executing"||status=="OrderCatched")
                {
                    this.current=3;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="等待开门";
                }
                if(status=="StationOpen")
                {
                    this.current=3;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="等待放入档案，关门";
                }
                if(status=="StationClose")
                {
                    this.current=4;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="等待密集柜执行";
                }
                if(status=="CabinetWait")
                {
                    this.current=4;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="密集柜工作中";
                }
                if(status=="CabinetComplete")
                {
                    this.current=5;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="已完成";
                    this.stepdata[5].content="等待机械手取档案";
                }
                if(status=="RobotWait")
                {
                    this.current=5;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="已完成";
                    this.stepdata[5].content="机械手执行中";
                }
                if(status=="RobotComplete")
                {
                    this.current=6;
                    this.stepdata[1].content="已完成";
                    this.stepdata[2].content="已完成";
                    this.stepdata[3].content="已完成";
                    this.stepdata[4].content="已完成";
                    this.stepdata[5].content="已完成";
                    this.stepdata[6].content="已完成";
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

            this.pagerequest.maxResultCount=100;
            this.pagerequest.skipCount=(this.currentPage-1)*1;

            //filters
            if (this.creationTime.length>0) {
                if(this.creationTime[0].toString()=='')
                {
                    this.pagerequest.from=new Date(1111,1,1,1,1,1,1);
                }
                else
                {
                this.pagerequest.from=this.creationTime[0];
                }
            }
            if (this.creationTime.length>1) {
                if(this.creationTime[0].toString()=='')
                {
                    this.pagerequest.to=new Date(1111,1,1,1,1,1,1);
                }
                else
                {
                this.pagerequest.to=this.creationTime[1];
                }
            }

            await this.$store.dispatch({
                type:'stocktask/getAll',
                data:this.pagerequest
            })
            if(this.$store.state.stocktask.list.length>0)
            {
                // console.log(this.currentRfid);
                if(this.continuousFlag)
                {
                    this.updateDoor();
                }
                else
                {

                    if(this.currentRfid!="")
                    {
                        //alert(this.currentRfid);
                        var flag=0;
                        for (let index = 0; index < this.$store.state.stocktask.list.length; index++) {
                            if(this.$store.state.stocktask.list[index].archiveBoxRfid==this.currentRfid)
                            {
                                //this.$store.state.stocktask.stocktask= this.$store.state.stocktask.list[index]; 
                                this.currentpg=index+1;    
                                this.pageChange(this.currentpg);  
                                flag=1;
                                //alert(this.currentpg);
                            }

                        }
                        if(flag==0)
                        {
                            alert(this.currentRfid+"任务不存在1。");
                            //console.log(this.currentRfid+"任务不存在1。");
                            location.reload();//页面重新刷新
                        }
                        // console.log(this.stocktask);
                        //  if(Object.keys(this.stocktask).length<=0)
                        //  {                                
                        //     console.log(this.currentRfid+"任务不存在1。");
                        // }
                        this.currentRfid="";
                    //return this.$store.state.stocktask.list[0];
                    }
                                // var rfid=this.currentRfid;
                                    // console.log("当前条码："+this.stocktask.archiveBoxRfid);
                    else
                    {

                                        // if(Object.keys(this.stocktask).length>0)
                                        // {
                                        //                         var flag=0;
                                        //         for (let index = 0; index < this.$store.state.stocktask.list.length; index++) 
                                        //         {
                                        //             if(this.$store.state.stocktask.list[index].archiveBoxRfid==this.currentRfid)
                                        //             {
                                        //                  this.currentpg=index+1;    
                                        //                 this.pageChange(this.currentpg);  
                                        //                 flag=1;     
                                        //             }                        
                                        //         }
                                        //         if(flag==0)
                                        //         {
                                        //              alert(this.currentRfid+"任务不存在2。");
                                        //             //console.log(this.currentRfid+"任务不存在2。");
                                        //             location.reload();//页面重新刷新
                                        //         }

                                        // }
                                    }
                }

                                // await  this.$store.dispatch({
                                //     type:'stocktask/getTask',
                                //     rfid:rfid
                                //     })    
                            this.taskStatus=this.getenumv(this.stocktask.manageStatus)
                            this.getStep(this.stocktask.manageStatus);
                            console.log(this.stocktask.manageStatus);
                            //如果任务未下达  进行判断 并执行自动下达
                            if(this.stocktask.manageStatus=="WaitingExecute")
                            {
                                //起始库位不为空
                                if(this.stocktask.startCellName!="")
                                {
                                    if(this.stocktask.id>0)
                                    {
                                        await this.$store.dispatch({
                                                        type:'stocktask/taskAssign',
                                                        mainId:this.stocktask.id
                                                    });
                                        this.$Message.success('任务下达成功。');
                                    }
                                    this.getStep("Executing");
                                }
                                else
                                {

                                }


                            }


                            this.selectedCell();  
                            this.getRobotL();
                


                
            }
            else
            {
                this.$Message.success('所有任务已执行结束。');
                this.logout();//所有任务结束 退出取档
            }
            
                          
    }
                //连续扫码，自动下达。
                async getpageAuto(){
                                console.log(this.currentRfid);
                                if(this.currentRfid!="")
                                {
                                await  this.$store.dispatch({
                                        type:'stocktask/getTask',
                                        rfid:this.currentRfid
                                        })   
                                this.taskStatus=this.getenumv(this.stocktask.manageStatus)
                                this.getStep(this.stocktask.manageStatus);
                                console.log(this.stocktask.manageStatus);
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
                                    alert("任务状态为已下达！")
                                    this.getpage();
                                }

                        } 
                 }
        //定时工作
          async  timedJob () {
           if(this.current>2)
           {
               this.getpage();
                if(this.stocktask.manageStatus=="CabinetWait")
                {
                    await this.$store.dispatch({
                        type:'board/getCabinetStatus',
                        no: '1'
                    });   
                    if(this.cabinetA)
                    {
                        //this.updateCabinet();
                        this.moveBox(this.deviceStatusDto.columnAnimation);
                        this.cabinetA=false;
                    }
                }
                if(this.stocktask.manageStatus=="CabinetClose")
                {
                    await this.$store.dispatch({
                        type:'board/getCabinetStatus',
                        no: '1'
                    });   
                    this.updateCabinet();
                }
               //alert(1);
           }

    }
        async created(){
            //this.getpage();

                     await this.$store.dispatch({
                         type:'stocktask/getCells',
                         data:this.cellZYDto
                     });
                     await this.$store.dispatch({
                        type:'cell/getStations'
                    });
             await this.$store.dispatch({
                type:'board/getCabinetStatus',
                no: '1'
            });
                 setInterval(this.timedJob, 10000);
             //this.getRobotL();
        }
        async mounted () {

            //          await this.$store.dispatch({
            //             type:'cell/getStations'
            //         });
            //  await this.$store.dispatch({
            //     type:'board/getCabinetStatus',
            //     no: '1'
            // });
            //  this.getRobotL();

        }
    }

</script>
<style scoped>
.page-body{
      height: 450px;  
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

.js-calendar-graph-svg{
margin-left: 0px;
}
.js-station-graph-svg{
margin-left: 60px;
}

/* 档案柜动画 */
 .achiveBox
{
position: absolute;
top: 50px;
/* margin-left: 0px; */
}


.movetoL
{
    transform: translateX(-100px);
    transition: transform 15s;

animation-fill-mode: forwards;/*forwards属性可以让动画定格到最后一帧*/
}
.movetoR
{
    transform: translateX(0px);
    transition: transform 15s;
    animation-fill-mode: forwards;/*forwards属性可以让动画定格到最后一帧*/
}

.movetoL0
{
    transform: translateX(-100px);
    transition: transform 0s;

animation-fill-mode: forwards;/*forwards属性可以让动画定格到最后一帧*/
}
.movetoR0
{
    transform: translateX(0px);
    transition: transform 0s;
}
/*档案向下移动*/
.aMovetoD
{
 position:relative; 
animation:achive-move 60s;
animation-fill-mode: forwards;/*forwards属性可以让动画定格到最后一帧*/
}
/* @keyframes achive-move{
    30% {
        transform: translateY(30px);
        left: 50px;
    }
    100% {
        left:100px;
    }
} */

.margin-top-20 {
    margin-top: 20px;
}
     .cardtitle{
    font-size: 16px;
    /* color: rgba(0,0,0,.85); */
    /* color:#e8eaec; */
    font-family: "Myriad Pro","Helvetica Neue",Arial,Helvetica,sans-serif;
    font-weight: 600;
    position: relative;
    top: 2px;
  }
</style>