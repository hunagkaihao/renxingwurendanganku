<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <Form ref="queryForm" :label-width="80" label-position="left" inline>
                    <Row :gutter="16">
                         <Col span="6">
                            <FormItem :label="L('Keyword')+':'" style="width:100%">
                                <Input v-model="pagerequest.keyword" :placeholder="L('CheckCode')"></Input>
                            </FormItem>
                        </Col>
                         <Col span="6">
                            <FormItem :label="L('ManageStatus')+':'" style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')" @on-change="isSelectChange">
                                    <Option value="All">{{L('All')}}</Option>
                                    <Option value="WaitingExecute">{{L('等待执行')}}</Option>
                                    <Option value="Executing">{{L('执行中')}}</Option>
                                    <Option value="Finish">{{L('结束')}}</Option>
                                </Select>
                            </FormItem>
                        </Col>
                        <Col span="6">
                            <FormItem :label="L('CreationTime')+':'" style="width:100%">
                                <DatePicker  v-model="creationTime" type="datetimerange" format="yyyy-MM-dd" style="width:100%" placement="bottom-end" :placeholder="L('SelectDate')"></DatePicker>
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Button icon="ios-search" type="primary" size="large" @click="getpage" class="toolbar-btn">{{L('Find')}}</Button>
                        <Button icon="ios-search" type="primary" size="large" @click="plancomplete" class="toolbar-btn">{{L('完成计划')}}</Button>
                    </Row>
                </Form>
                <div class="margin-top-10">
                    <Table  :loading="loading" stripe highlight-row :columns="columns" :no-data-text="L('NoDatas')" border  ref="selection" :data="list" @on-row-click="rowselect">
                    </Table>
                    <Page  show-sizer class-name="fengpage" :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
                </div>
                <div class="margin-top-10">
                        <Button icon="ios-search" type="primary" size="large" @click="checkconfirm" class="toolbar-btn">{{L('账实一致确认')}}</Button>
                        <Button icon="ios-search" type="primary" size="large" @click="lossconfirm" class="toolbar-btn">{{L('盘亏确认')}}</Button>
                </div>
                <div class="margin-top-10">
                    <Table  :loading="loading" :columns="columns2" :no-data-text="L('NoDatas')" border  ref="selection2" :data="list2" >
                    </Table>
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
    class  PageCheckhisRequest extends PageRequest{
        keyword:string;
        checkStatus:string;
        from:Date;
        to:Date;
    }

    @Component({
        components:{}
    })
    export default class Checkhiss extends AbpBase{

        //filters
        pagerequest:PageCheckhisRequest=new PageCheckhisRequest();
        creationTime:Date[]=[];
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
            key:'Complete'},{ value:'取消',
            key:'Cancel'},{ value:'批量入库',
            key:'HPBatchStockIn'},{ value:'自动盘点',
            key:'AnnualCheck'},{ value:'结束',
            key:'Finish'}] 
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
        isSelectChange(val:string){
            //console.log(val);
            if(val==='WaitingExecute'){
                this.pagerequest.checkStatus='WaitingExecute';
            }else if(val==='Executing'){
                this.pagerequest.checkStatus='Executing';
            }else if(val==='Finish'){
                this.pagerequest.checkStatus='Finish';
            }else{
                this.pagerequest.checkStatus=null;
            }
        }
        get list(){
            return this.$store.state.checkhis.list;
        };
        get list2(){
            return this.$store.state.checkhis.checklisthiss;
        };
        get loading(){
            return this.$store.state.checkhis.loading;
        }
        pageChange(page:number){
            this.$store.commit('checkhis/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('checkhis/setPageSize',pagesize);
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
            //alert(this.creationTime);

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
                type:'checkhis/getAll',
                data:this.pagerequest
            })
        }
        async getpage2(mainid){       
           await this.$store.dispatch({
                type:'checkhis/getLists',
                id:mainid
            })
        }
        async checkconfirm(){  
            if((this.$refs.selection2 as any).getSelection().length>0)
            {
                for (let index = 0; index < (this.$refs.selection2 as any).getSelection().length; index++) {
                        const element = (this.$refs.selection2 as any).getSelection()[index];
                        await this.$store.dispatch({
                                    type:'checkhis/checkconfirm',
                                    id:element.id
                                })
                        this.$Message.success('确认成功。');
                }
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择盘点记录表。');
            }     

        }

        async lossconfirm(){  
            if((this.$refs.selection2 as any).getSelection().length>0)
            {
                for (let index = 0; index < (this.$refs.selection2 as any).getSelection().length; index++) {
                        const element = (this.$refs.selection2 as any).getSelection()[index];
                        await this.$store.dispatch({
                                    type:'checkhis/lossconfirm',
                                    id:element.id
                                })
                        this.$Message.success('盘亏确认成功。');
                }
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择盘点记录表。');
            }     

        }
        async plancomplete(){  
            if((this.$refs.selection as any).getSelection().length==1)
            {
                        const element = (this.$refs.selection as any).getSelection()[0];
                        await this.$store.dispatch({
                                    type:'checkhis/plancomplete',
                                    id:element.checkCode
                                })
                        this.$Message.success('盘亏确认成功。');
            await this.getpage();
            await  this.getpage2(0);
            }
            else
            {
                alert('先选择盘点计划，且单选。');
            }     

        }
        get pageSize(){
            return this.$store.state.checkhis.pageSize;
        }
        get totalCount(){
            return this.$store.state.checkhis.totalCount;
        }
        get currentPage(){
            return this.$store.state.checkhis.currentPage;
        }

        columns=[            {
                type: 'selection',
                width: 60,
                align: 'center'
            },{
            title:this.L('Id'),
            width: 60,
            key:'id'
        },{
            title:this.L('CheckCode'),
            key:'checkCode'
        },{
            title:this.L('CheckType'),
            key:'checkType',
            render:(h:any,params:any)=>{
                return h('span',this.getenumv(params.row.checkType))
            }
        },{
            title:this.L('CreationTime'),
            key:'creationTime',
            render:(h:any,params:any)=>{
                return h('span',new Date(params.row.creationTime).toLocaleDateString())
            }
        },{
            title:this.L('CheckStatus'),
            key:'checkStatus',
            render:(h:any,params:any)=>{
                return h('span',this.getenumv(params.row.checkStatus))
            }
        },
        {
            title:this.L('FinishTime'),
            key:'finishTime'
        },       
         {
            title:this.L('VerifyFinishTime'),
            key:'verifyFinishTime'
        }
        ]


                columns2=[{
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
            title:this.L('StockBarcode'),
            key:'stockBarcode'
        },{
            title:this.L('BoxBarcode'),
            key:'boxBarcode'
        },{
            title:this.L('Account'),
            key:'account'
        },{
            title:this.L('RealAmount_1'),
            key:'realAmount_1'
        },{
            title:this.L('VerifyAmount'),
            key:'verifyAmount'
        },{
            title:this.L('VerifyUser'),
            key:'verifyUser'
        },{
            title:this.L('VerifyFinishTime'),
            key:'verifyFinishTime'
        }]
        async created(){
            this.getpage();
        }
    }
</script>