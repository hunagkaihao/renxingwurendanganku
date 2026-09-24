<style lang="less">
    @import "../stocktask.less";
</style>
<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <div class="margin-top-1">
                        <div class="main-header">
                             <nav class="navbar-default" > <h1>无人档案库操作平台-借阅出库</h1> </nav>
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
                <div class="margin-top-10">
                        <Form ref="queryForm" :label-width="80" label-position="left" inline>
                            <Row :gutter="16">
                                <Col span="6">
                                    <FormItem :label="L('Keyword')+':'" style="width:100%">
                                        <Input v-model="pagerequest.keyword" :placeholder="L('ArchiveBoxRfid')"></Input>
                                    </FormItem>
                                </Col>
                                <Col span="6">
                                    <Button icon="ios-search" type="primary" size="large" @click="getpage" class="toolbar-btn">{{L('Find')}}</Button>
                                </Col>
                            </Row>

                        </Form>
                </div>
                <div class="margin-top-10">
                    <Table  :loading="loading" stripe highlight-row :columns="columns" :no-data-text="L('NoDatas')" border  ref="selection"  :data="list" @on-row-click="rowselect">
                    </Table>
                    <Page  show-sizer class-name="fengpage" :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
                </div>

                <div class="margin-top-10">
                    <Row>
                        <Button v-show="current<=2" icon="ios-search" type="primary" size="large" @click="taskAssign" class="toolbar-btn">{{L('下达任务')}}</Button>
                        <Button v-show="current<=1" icon="ios-search" type="primary" size="large" @click="taskCancel" class="toolbar-btn">{{L('取消任务')}}</Button>
                        <Button v-show="current>=1&current<3" icon="ios-search" type="primary" size="large" @click="taskComplete" class="toolbar-btn">{{L('强制完成')}}</Button>
                        <Button v-show="current>2" icon="ios-search" type="primary" size="large" @click="taskConfirm" class="toolbar-btn">{{L('确认完成')}}</Button>
                        <Button v-show="current>2" icon="ios-search" type="primary" size="large" @click="taskConfirmBack" class="toolbar-btn">{{L('确认完成[回库]')}}</Button>
                        
                    </Row>
                </div>
                 <div class="margin-top-10">
                    <Table  :loading="loading" :columns="columns2" :no-data-text="L('NoDatas')" border  ref="selection2" :data="list2" >
                    </Table>
                </div>
                <div v-show="current>0" class="margin-top-10">
                     <Steps :current="current">
                        <Step v-for="(item) of stepdata" :key="item.id"  :title="item.title" :icon="item.icon"  :content="item.id<(current+1)?'已完成':item.content" ></Step>
                    </Steps>
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
    import Cookies from 'js-cookie';
    class  PageStocktaskRequest extends PageRequest{
        keyword:string;
        manageTypeCode:string='NpFullStockOut';
        manageStatus:string;
        from:Date;
        to:Date;
    }
        class pickDto {      
            mainId: number;
            goodsIds: Array<number>;      
            constructor ( sid: number, gids: Array<number> ) {
                this.mainId =sid;
                this.goodsIds =gids;
            };
        
        }
    @Component({
        components:{}
    })
    export default class Stocktasks extends AbpBase{

        //filters
        pagerequest:PageStocktaskRequest=new PageStocktaskRequest();
        creationTime:Date[]=[];
        currentRowId:number=0;
        currentRowRfid:string="";
                stepdata=[{ id:'1',title:'身份验证',
            content:'已完成',icon:'md-finger-print'},{ id:'2',title:'打开密集柜',
            content:'',icon:'ios-albums'},{ id:'3',title:'机械手抓取',
            content:'',icon:'md-download'},{ id:'4',title:'打开柜门',
            content:'',icon:'ios-person'},{ id:'5',title:'确认任务',
            content:'',icon:'md-checkbox'},{ id:'6',title:'任务结束',
            content:'',icon:'ios-flag'}];
        enumdata=[{ value:'入库',
            key:'NPFullStockIn'},{ value:'等待执行',
            key:'WaitingExecute'}
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
            key:'RobotComplete'}]
        current:number=0;
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
            location.reload();

        }


        get list(){
            return this.$store.state.stocktask.list;
        };
        get list2(){
            return this.$store.state.stocktask.stocktasklists;
        };
        get loading(){
            return this.$store.state.stocktask.loading;
        }
        pageChange(page:number){
            this.$store.commit('stocktask/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('stocktask/setPageSize',pagesize);
            this.getpage();
        }
        rowselect(currentRow, index)
        {
                this.currentRowId=currentRow.id;
                this.currentRowRfid=currentRow.archiveBoxRfid;
                this.getpage2(currentRow.id);
                this.getStep(currentRow.manageStatus);
        }
                getStep(status:string)
        {
                if(status=="Executing")
                {
                    this.current=2;
                }
                if(status=="WaitingExecute")
                {
                    this.current=0;
                    this.stepdata[0].content="已完成";
                }
                if(status=="CabinetWait")
                {
                    this.current=1;
                    this.stepdata[1].content="已开始";
                }
                if(status=="CabinetComplete")
                {
                    this.current=1;
                    this.stepdata[1].content="已完成";
                }
                if(status=="RobotWait")
                {
                    this.current=2;
                    this.stepdata[2].content="已开始";
                }
                if(status=="RobotComplete")
                {
                    this.current=2;
                    this.stepdata[2].content="已完成";
                }
                if(status=="StationOpen")
                {
                    this.current=3;
                    this.stepdata[3].content="已开始";
                }
                if(status=="StationClose")
                {
                    this.current=3;
                    this.stepdata[3].content="已完成";
                }
                if(status=="WaitingConfirm")
                {
                    this.current=4;
                    this.stepdata[4].content="已开始";
                }
        }
        async getpage(){
          
            this.pagerequest.maxResultCount=this.pageSize;
            this.pagerequest.skipCount=(this.currentPage-1)*this.pageSize;
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
        }
        async getpage2(mainid){       
           await this.$store.dispatch({
                type:'stocktask/getLists',
                id:mainid
            });
             (this.$refs.selection2 as any).selectAll(true);
        }

          async taskAssign(){  
            if((this.$refs.selection as any).getSelection().length>0)
            {
                for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                        const element = (this.$refs.selection as any).getSelection()[index];
                        await this.$store.dispatch({
                                type:'stocktask/taskAssignOut',
                                mainId:element.id
                            });
                        this.$Message.success('任务下达成功。');
                }
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择任务。');
            }

        }
        async taskCancel(){  
        //    alert(this.currentRowRfid);
        //    await this.$store.dispatch({
        //         type:'stocktask/taskCancelIn',
        //         mainId:this.currentRowId
        //     });
        //    this.$Message.success('任务取消成功。');
        //    await this.getpage();
            if((this.$refs.selection as any).getSelection().length>0)
            {
                for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                        const element = (this.$refs.selection as any).getSelection()[index];
                        await this.$store.dispatch({
                                type:'stocktask/taskCancelIn',
                                mainId:element.id
                            });
                        this.$Message.success('任务取消成功。');
                }
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择任务。');
            }

        }
              async taskComplete()
      {  
            if((this.$refs.selection as any).getSelection().length==1)
            {
                    const element = (this.$refs.selection as any).getSelection()[0];
                     this.$Modal.confirm({
                                        title:this.L('Tips'),
                                        content:this.L('异常情况下使用，且必须任务已经执行完毕，才能使用此项功能。'),
                                        okText:this.L('Yes'),
                                        cancelText:this.L('No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'stocktask/taskComplete',
                                                 mainId:element.id
                                            })
                        this.$Message.success('任务完成。');
                        await this.getpage();
                        await  this.getpage2(0);
                 }
                })
            }
            else
            {
                alert('先选择任务。强制完成只能选取单条任务');
            }

        }
        async taskConfirm(){  
        //    alert(this.currentRowRfid);
        //    await this.$store.dispatch({
        //         type:'stocktask/taskConfirm',
        //         mainId:this.currentRowId
        //     });
          // await this.getpage();
            if((this.$refs.selection as any).getSelection().length==1)
            {
                var gids=new Array<number>();
                  for (let index = 0; index < (this.$refs.selection2 as any).getSelection().length; index++) {
                      const element = (this.$refs.selection2 as any).getSelection()[index];
                      gids.push(element.goodsId);
                                      //console.log(JSON.stringify(pd));
                  }
                 const element = (this.$refs.selection as any).getSelection()[0];
                 var plist=  new pickDto(element.id,gids);
                        await this.$store.dispatch({
                                type:'stocktask/taskConfirm',
                                data:plist
                            });
                        this.$Message.success('任务确认成功。');
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择任务。且单次只能选择一条记录');
            }

        }
        async taskConfirmBack(){  
        //    alert(this.currentRowRfid);
        //    await this.$store.dispatch({
        //         type:'stocktask/taskConfirm',
        //         mainId:this.currentRowId
        //     });
          // await this.getpage();
            if((this.$refs.selection as any).getSelection().length==1)
            {
                var gids=new Array<number>();
                  for (let index = 0; index < (this.$refs.selection2 as any).getSelection().length; index++) {
                      const element = (this.$refs.selection2 as any).getSelection()[index];
                      gids.push(element.goodsId);
                                      //console.log(JSON.stringify(pd));
                  }
                 const element = (this.$refs.selection as any).getSelection()[0];
                 var plist=  new pickDto(element.id,gids);
                        await this.$store.dispatch({
                                type:'stocktask/taskConfirmBack',
                                data:plist
                            });
                        this.$Message.success('任务确认成功。');
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择任务。且单次只能选择一条记录');
            }

        }   
        
            async  completeNotice () {
        //是否有返回值
        this.$store.dispatch({
            type:'stocktask/completeNotice',
            data: { 
                "timeInterval": 15,                     
                     "managetype": 'NpFullStockOut'
                        }
        }) 
        //console.log(this.$store.state.stocktask.completelist);
         if(this.$store.state.stocktask.completelist.length>0)
         {
             this.getpage();
             for (let index = 0; index < this.$store.state.stocktask.completelist.length; index++) {
                 const element = this.$store.state.stocktask.completelist[index];
                 
              this.$Notice.success({
                    title: '任务完成',
                    desc: element.archiveBoxRfid+'档案盒入库完成。'
                });
             }

         }
    }
        get pageSize(){
            return this.$store.state.stocktask.pageSize;
        }
        get totalCount(){
            return this.$store.state.stocktask.totalCount;
        }
        get currentPage(){
            return this.$store.state.stocktask.currentPage;
        }

        columns=[            {
                type: 'selection',
                width: 60,
                align: 'center'
            },{
            title:this.L('Id'),
            key:'id'
        },{
            title:this.L('ArchiveBoxRfid'),
            key:'archiveBoxRfid'
        },
        {
            title:this.L('StartCellName'),
            key:'startCellName'
        },       
         {
            title:this.L('EndCellName'),
            key:'endCellName'
        },{
            title:this.L('ManageStatus'),
            key:'manageStatus',
            render:(h:any,params:any)=>{
                return h('span',this.getenumv(params.row.manageStatus))
            }
        },{
            title:this.L('CreationTime'),
            key:'creationTime',
            render:(h:any,params:any)=>{
                return h('span',new Date(params.row.creationTime).toLocaleDateString())
            }
        }
        ]

                        columns2=[ {
                type: 'selection',
                width: 60,
                align: 'center'
            },{
            title:this.L('Id'),
            key:'id'
        },{
            title:this.L('GoodsId'),
            key:'goodsId'
        },{
            title:this.L('ManageId'),
            key:'manageId'
        },{
            title:this.L('GoodsCode'),
            key:'goodsCode'
        },{
            title:this.L('GoodsName'),
            key:'goodsName'
        },{
            title:this.L('StorageListQuantity'),
            key:'storageListQuantity'
        },       
         {
            title:this.L('BorrowerId'),
            key:'borrowerId'
        }]
        async created(){
            this.getpage();
            setInterval(this.completeNotice, 13000);
            this.$Notice.config({
                top: 80,
                duration: 4.5
            });
        }
    }
</script>
