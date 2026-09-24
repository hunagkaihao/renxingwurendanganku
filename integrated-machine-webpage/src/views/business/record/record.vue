<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <Form ref="queryForm" :label-width="80" label-position="left" inline>
                    <Row :gutter="16">
                         <Col span="6">
                            <FormItem :label="L('Keyword')+':'" style="width:100%">
                                <Input v-model="pagerequest.keyword" :placeholder="L('ArchiveBoxRfid')"></Input>
                            </FormItem>
                        </Col>
                         <Col span="6">
                            <FormItem :label="L('ManageStatus')+':'" style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')" @on-change="isSelectChange">
                                    <Option value="All">{{L('All')}}</Option>
                                    <Option value="Complete">{{L('完成')}}</Option>
                                    <Option value="Cancel">{{L('取消')}}</Option>
                                </Select>
                            </FormItem>
                        </Col>
                         <Col span="6">
                            <FormItem :label="L('ManageTypeCode')+':'" style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')" @on-change="isSelectChange2">
                                    <Option value="All">{{L('All')}}</Option>
                                    <Option value="NPFullStockIn">{{L('入库')}}</Option>
                                    <Option value="NpFullStockOut">{{L('借阅出库')}}</Option>
                                    <Option value="HpAnnualCheckDown">{{L('盘点出库')}}</Option>
                                    <Option value="HPBatchStockIn">{{L('批量入库')}}</Option>
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
                    </Row>
                </Form>
                <div class="margin-top-10">
                    <Table  :loading="loading" stripe highlight-row :columns="columns" :no-data-text="L('NoDatas')" border :data="list" @on-row-click="rowselect">
                    </Table>
                    <Page  show-sizer class-name="fengpage" :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
                </div>

                <div class="margin-top-10">
                    <Table  :loading="loading" :columns="columns2" :no-data-text="L('NoDatas')" border :data="list2" >
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
    class  PageRecordRequest extends PageRequest{
        keyword:string;
        manageTypeCode:string;
        manageStatus:string;
        from:Date;
        to:Date;
    }

    @Component({
        components:{}
    })
    export default class Records extends AbpBase{

        //filters
        pagerequest:PageRecordRequest=new PageRecordRequest();
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
            key:'AnnualCheck'}] 
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
            if(val==='Complete'){
                this.pagerequest.manageStatus='Complete';
            }else if(val==='Cancel'){
                this.pagerequest.manageStatus='Cancel';
            }else{
                this.pagerequest.manageStatus=null;
            }
        }
                isSelectChange2(val:string){
            //console.log(val);
            if(val==='NPFullStockIn'){
                this.pagerequest.manageTypeCode='NPFullStockIn';
            }else if(val==='NpFullStockOut'){
                this.pagerequest.manageTypeCode='NpFullStockOut';
            }else if(val==='HpAnnualCheckDown'){
                this.pagerequest.manageTypeCode='HpAnnualCheckDown';
            }else if(val==='HPBatchStockIn'){
                this.pagerequest.manageTypeCode='HPBatchStockIn';
            }else{
                this.pagerequest.manageTypeCode=null;
            }
        }
        get list(){
            return this.$store.state.record.list;
        };
        get list2(){
            return this.$store.state.record.recordlists;
        };
        get loading(){
            return this.$store.state.record.loading;
        }
        pageChange(page:number){
            this.$store.commit('record/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('record/setPageSize',pagesize);
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
                type:'record/getAll',
                data:this.pagerequest
            })
        }
        async getpage2(mainid){       
           await this.$store.dispatch({
                type:'record/getLists',
                id:mainid
            })
        }

       
        get pageSize(){
            return this.$store.state.record.pageSize;
        }
        get totalCount(){
            return this.$store.state.record.totalCount;
        }
        get currentPage(){
            return this.$store.state.record.currentPage;
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
            title:this.L('ManageTypeCode'),
            key:'manageTypeCode',
            render:(h:any,params:any)=>{
                return h('span',this.getenumv(params.row.manageTypeCode))
            }
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
            title:this.L('ArchiveBoxRfid'),
            key:'archiveBoxRfid'
        },
        {
            title:this.L('StartCellName'),
            key:'startCellPosition'
        },       
         {
            title:this.L('EndCellName'),
            key:'endCellPosition'
        }
        ]


                columns2=[{
            title:this.L('Id'),
            key:'id'
        },{
            title:this.L('GoodsId'),
            key:'goodsId'
        },
        {
            title:this.L('GoodsCode'),
            key:'goodsCode'
        },{
            title:this.L('GoodsName'),
            key:'goodsName'
        },{
            title:this.L('BoxBarcode'),
            key:'boxBarcode'
        },{
            title:this.L('RecordListQuantity'),
            key:'recordListQuantity'
        },       
         {
            title:this.L('BorrowerId'),
            key:'borrowerId'
        }]
        async created(){
            this.getpage();
        }
    }
</script>