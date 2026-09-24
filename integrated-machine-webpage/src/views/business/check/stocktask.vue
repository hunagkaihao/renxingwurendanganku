<style lang="less">
    @import "../stocktask.less";
</style>
<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <div class="margin-top-1">
                        <div class="main-header">
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
                        <Button v-show="current==1" icon="ios-search" type="primary" size="large" @click="taskAssign" class="toolbar-btn">{{L('下达任务')}}</Button>
                        <Button v-show="current<=1" icon="ios-search" type="primary" size="large" @click="taskCancel" class="toolbar-btn">{{L('取消任务')}}</Button>
                        <Button v-show="current>=1" icon="ios-search" type="primary" size="large" @click="taskComplete" class="toolbar-btn">{{L('强制完成')}}</Button>
                        <Button v-show="current>1" icon="ios-search" type="primary" size="large" @click="taskConfirm" class="toolbar-btn">{{L('确认完成')}}</Button>
                    </Row>
                </div>
                <div class="margin-top-10">
                    <Table  :loading="loading" :columns="columns2" :no-data-text="L('NoDatas')" border :data="list2" >
                    </Table>
                </div>
                <div v-show="current>0" class="margin-top-10">
                     <Steps :current="current">
                        <Step title="人脸验证" icon="ios-person"></Step>
                        <Step title="下达任务" icon="ios-camera"></Step>
                        <Step title="执行任务" icon="ios-mail"></Step>
                        <Step title="任务完成" icon="ios-mail"></Step>
                    </Steps>
                </div>
            </div>
        </Card>
        <create-stocktask v-model="createModalShow" @save-success="getpage"></create-stocktask>
        <edit-stocktask v-model="editModalShow" @save-success="getpage"></edit-stocktask>
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
        manageTypeCode:string='HpAnnualCheckDown';
        manageStatus:string;
        from:Date;
        to:Date;
    }

    @Component({
        components:{}
    })
    export default class Stocktasks extends AbpBase{
        edit(){
            this.editModalShow=true;
        }

        //filters
        pagerequest:PageStocktaskRequest=new PageStocktaskRequest();
        creationTime:Date[]=[];

        createModalShow:boolean=false;
        editModalShow:boolean=false;
        currentRowId:number=0;
        currentRowRfid:string="";
        enumdata=[{ value:'入库',
            key:'NPFullStockIn'},{ value:'等待执行',
            key:'WaitingExecute'}
            ,{ value:'执行中',
            key:'Executing'}
            ,{ value:'借阅出库',
            key:'NpFullStockOut'}
            ,{ value:'待完成确认',
            key:'WaitingConfirm'},{ value:'盘点出库',
            key:'HpAnnualCheckDown'},{ value:'完成',
            key:'Complete'}]
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
        isSelectChange(val:string){
            console.log(val);
            if(val==='WaitingExecute'){
                this.pagerequest.manageStatus='WaitingExecute';
            }else if(val==='Executing'){
                this.pagerequest.manageStatus='Executing';
            }else{
                this.pagerequest.manageStatus=null;
            }
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
        create(){
            this.createModalShow=true;
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
                if(currentRow.manageStatus=="Executing")
                {
                    this.current=2;
                }
                if(currentRow.manageStatus=="WaitingExecute")
                {
                    this.current=1;
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
            })
        }

        async taskAssign(){  
           //alert(this.currentRowRfid);
           
        //    await this.$store.dispatch({
        //         type:'stocktask/taskAssign',
        //         mainId:this.currentRowId
        //     });
        //    this.$Message.success('任务下达成功。');
        //    await this.getpage();
            if((this.$refs.selection as any).getSelection().length>0)
            {
                for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                        const element = (this.$refs.selection as any).getSelection()[index];
                        await this.$store.dispatch({
                                type:'stocktask/taskAssign',
                                mainId:element.id
                            });
                        this.$Message.success('任务下达成功。');
                }
            await this.getpage();
            await  this.getpage2((this.$refs.selection as any).getSelection()[0].id);
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
            title:this.L('ManageTypeCode'),
            key:'manageTypeCode',
            render:(h:any,params:any)=>{
                return h('span',this.getenumv(params.row.manageTypeCode))
            }
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
        },{
            title:this.L('Actions'),
            key:'Actions',
            width:150,
            render:(h:any,params:any)=>{
                return h('div',[
                    h('Button',{
                        props:{
                            type:'primary',
                            size:'small'
                        },
                        style:{
                            marginRight:'5px'
                        },
                        on:{
                            click:()=>{
                                this.$store.commit('stocktask/edit',params.row);
                                this.edit();
                            }
                        }
                    },this.L('Edit')),
                    h('Button',{
                        props:{
                            type:'error',
                            size:'small'
                        },
                        on:{
                            click:async ()=>{
                                this.$Modal.confirm({
                                        title:this.L('Tips'),
                                        content:this.L('DeleteArchiveConfirm'),
                                        okText:this.L('Yes'),
                                        cancelText:this.L('No'),
                                        onOk:async()=>{
                                            await this.$store.dispatch({
                                                type:'stocktask/delete',
                                                data:params.row
                                            })
                                            await this.getpage();
                                        }
                                })
                            }
                        }
                    },this.L('Delete'))
                ])
            }
        }]
                columns2=[{
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
        }]
        async created(){
            this.getpage();
            this.getpage2(0);
        }
    }
</script>