<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <Form ref="queryForm" :label-width="80" label-position="left" inline>
                    <Row :gutter="16">
                         <Col span="6">
                            <FormItem :label="L('Keyword')+':'" style="width:100%">
                                <Input v-model="pagerequest.keyword" :placeholder="L('GoodsCode')+'/'+L('GoodsName')"></Input>
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
                        <Button icon="ios-search" type="primary" size="large" @click="pick" class="toolbar-btn">{{L('借阅出库')}}</Button>
                    </Row>
                </Form>
                <div class="margin-top-10">
                    <Table  :loading="loading" :columns="columns" :no-data-text="L('NoDatas')" border ref="selection" :data="list">
                    </Table>
                    <Page  show-sizer class-name="fengpage" :total="totalCount" class="margin-top-10" @on-change="pageChange" @on-page-size-change="pagesizeChange" :page-size="pageSize" :current="currentPage"></Page>
                </div>
            </div>
        </Card>
        <edit-archiveBoxlist v-model="editModalShow" @save-success="getpage"></edit-archiveBoxlist>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import EditArchiveBoxlist from './edit-archiveboxlist.vue'
    class  PageArchiveBoxlistRequest extends PageRequest{
        keyword:string;
        from:Date;
        to:Date;
    }
        class pickDto {      
            storageId: number;
            goodsId: number;    
            storageListQuantity: number;  
            manageListQuantity: number;     
            constructor ( sid: number, gid: number ) {
                this.storageId =sid;
                this.goodsId =gid;
            };
        
        }
    @Component({
        components:{EditArchiveBoxlist}
    })
    export default class ArchiveBoxlists extends AbpBase{
        edit(){
            this.editModalShow=true;
        }
        //filters
        pagerequest:PageArchiveBoxlistRequest=new PageArchiveBoxlistRequest();
        creationTime:Date[]=[];

        createModalShow:boolean=false;
        editModalShow:boolean=false;
        get list(){
            return this.$store.state.archiveboxlist.list;
        };
        get loading(){
            return this.$store.state.archiveboxlist.loading;
        }
        create(){
            this.createModalShow=true;
        }
        pageChange(page:number){
            this.$store.commit('archiveboxlist/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('archiveboxlist/setPageSize',pagesize);
            this.getpage();
        }
        async getpage(){
          
            this.pagerequest.maxResultCount=this.pageSize;
            this.pagerequest.skipCount=(this.currentPage-1)*this.pageSize;
            //filters
            
            if (this.creationTime.length>0) {
                this.pagerequest.from=this.creationTime[0];
            }
            if (this.creationTime.length>1) {
                this.pagerequest.to=this.creationTime[1];
            }

            await this.$store.dispatch({
                type:'archiveboxlist/getAll',
                data:this.pagerequest
            })
        }
       async pick(){
               var plist=  new Array<pickDto>();
                  console.log((this.$refs.selection as any).getSelection());
                  for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                      const element = (this.$refs.selection as any).getSelection()[index];
                      var pd=new pickDto(element.storageId,element.goodsId);
                      plist.push(pd);
                                      console.log(JSON.stringify(pd));
                  }
                   console.log(JSON.stringify(plist));
                await this.$store.dispatch({
                type:'archiveboxlist/pick',
                data:plist
                     })
                }
        get pageSize(){
            return this.$store.state.archiveboxlist.pageSize;
        }
        get totalCount(){
            return this.$store.state.archiveboxlist.totalCount;
        }
        get currentPage(){
            return this.$store.state.archiveboxlist.currentPage;
        }

        columns=[
            {
                type: 'selection',
                width: 60,
                align: 'center'
            },{
            title:this.L('Id'),
            key:'id'
        },{
            title:this.L('StorageId'),
            key:'storageId'
        },{
            title:this.L('GoodsId'),
            key:'goodsId'
        },{
            title:this.L('GoodsCode'),
            key:'goodsCode'
        },{
            title:this.L('GoodsName'),
            key:'goodsName'
        },{
            title:this.L('CellName'),
            key:'cellName'
        },{
            title:this.L('CreationTime'),
            key:'creationTime',
            render:(h:any,params:any)=>{
                return h('span',new Date(params.row.creationTime).toLocaleDateString())
            }
        },{
            title:this.L('BoxBarcode'),
            key:'boxBarcode'
        },
        {
            title:this.L('StorageListRemark'),
            key:'storageListRemark'
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
                                this.$store.commit('archiveboxlist/edit',params.row);
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
                                                type:'archiveboxlist/delete',
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
        async created(){
            this.getpage();
        }
    }
</script>