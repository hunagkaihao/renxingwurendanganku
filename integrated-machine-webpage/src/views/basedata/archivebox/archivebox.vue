<template>
    <div>
        <Card dis-hover>
            <div class="page-body">
                <Form ref="queryForm" :label-width="80" label-position="left" inline>
                    <Row :gutter="16">
                         <Col span="6">
                            <FormItem :label="L('Keyword')+':'" style="width:100%">
                                <Input v-model="pagerequest.keyword" :placeholder="L('ArchiveBoxName')+'/'+L('ArchiveBoxRfid')"></Input>
                            </FormItem>
                        </Col>
                         <Col span="6">
                            <FormItem :label="L('CellCode')+':'" style="width:100%">
                                <Input v-model="pagerequest.cellCode" :placeholder="L('CellCode')"></Input>
                            </FormItem>
                        </Col>
                        <Col span="6">
                            <FormItem :label="L('CreationTime')+':'" style="width:100%">
                                <DatePicker  v-model="creationTime" type="datetimerange" format="yyyy-MM-dd" style="width:100%" placement="bottom-end" :placeholder="L('SelectDate')"></DatePicker>
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Button @click="create" icon="android-add" type="primary" size="large">{{L('Add')}}</Button>
                        <Button icon="ios-search" type="primary" size="large" @click="getpage" class="toolbar-btn">{{L('Find')}}</Button>
                        <Button icon="ios-search" type="primary" size="large" @click="fullIn" class="toolbar-btn">{{L('创建入库')}}</Button>
                        <Button icon="ios-search" type="primary" size="large" @click="boxEmptyOutTask" class="toolbar-btn">{{L('档案盒出库')}}</Button>
                        
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
        <create-archiveBox v-model="createModalShow" @save-success="getpage"></create-archiveBox>
        <edit-archiveBox v-model="editModalShow" @save-success="getpage"></edit-archiveBox>
        <binding-archiveBox v-model="bindingModalShow" @save-success="getpage"></binding-archiveBox>
        <bindrfid-archiveBox v-model="bindrfidModalShow" @save-success="getpage2"></bindrfid-archiveBox>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import CreateArchiveBox from './create-archivebox.vue'
    import EditArchiveBox from './edit-archivebox.vue'
    import BindingArchiveBox from './binding-archivebox.vue'
    import BindrfidArchiveBox from './bindrfid-archivebox.vue'
    class  PageArchiveBoxRequest extends PageRequest{
        keyword:string;
        cellCode:string;
        from:Date;
        to:Date;
    }

    @Component({
        components:{CreateArchiveBox,EditArchiveBox,BindingArchiveBox,BindrfidArchiveBox}
    })
    export default class Archiveboxs extends AbpBase{
        edit(){
            this.editModalShow=true;
        }
        binding(){
               this.bindingModalShow=true;
 

        }
        bindingRfid(){
            this.bindrfidModalShow=true;
        }
        //filters
        pagerequest:PageArchiveBoxRequest=new PageArchiveBoxRequest();
        creationTime:Date[]=[];

        createModalShow:boolean=false;
        editModalShow:boolean=false;
        bindingModalShow:boolean=false;
        bindrfidModalShow:boolean=false;
        currentRowId:number=0;
        currentRowRfid:string="";
        get list(){
            return this.$store.state.archivebox.list;
        };
        get list2(){
            return this.$store.state.archivebox.archiveboxlists;
        };
        get loading(){
            return this.$store.state.archivebox.loading;
        }
        create(){
            this.createModalShow=true;
        }
        pageChange(page:number){
            this.$store.commit('archivebox/setCurrentPage',page);
            this.getpage();
        }
        pagesizeChange(pagesize:number){
            this.$store.commit('archivebox/setPageSize',pagesize);
            this.getpage();
        }
        rowselect(currentRow, index)
        {
                this.currentRowId=currentRow.id;
                this.currentRowRfid=currentRow.archiveBoxRfid;
                this.getpage2(currentRow.id);
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
                type:'archivebox/getAll',
                data:this.pagerequest
            })
        }
        async getpage2(mainid){       
           await this.$store.dispatch({
                type:'archivebox/getLists',
                id:mainid
            })
        }

        async fullIn(){  
           //alert(this.currentRowRfid);
           var aa=this.currentRowRfid;
           await this.$store.dispatch({
                type:'archivebox/fullIn',
                Rfid: this.currentRowRfid
            })
            this.$Message.success('任务创建成功。');
        }
        // async boxEmptyOutTask(){  
        //    //alert(this.currentRowRfid);
        //    //var aa=this.currentRowRfid;
        //    await this.$store.dispatch({
        //         type:'archivebox/boxEmptyOutTask',
        //         stgId: this.currentRowId
        //     })
        //     this.$Message.success('任务创建成功。');
        // }
        async boxEmptyOutTask(){  
             if((this.$refs.selection as any).getSelection().length==0)
            {
                 this.$Message.error('请先选择档案盒');
                 return;
            }

                for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                      const element = (this.$refs.selection as any).getSelection()[index];
                      console.log(element.cellName);
                      if(element.cellName=="")
                      {
                         this.$Message.error('请选择库内档案盒');
                         return;
                                // this.$Message.success('任务创建成功。');
                      }
                  }

                for (let index = 0; index < (this.$refs.selection as any).getSelection().length; index++) {
                      const element = (this.$refs.selection as any).getSelection()[index];
                      console.log(element.cellName);

                            await this.$store.dispatch({
                                    type:'archivebox/boxEmptyOutTask',
                                    stgId: element.id
                                })
                                // this.$Message.success('任务创建成功。');
                      
                  }
                this.$Message.success('任务创建成功。');
            // if((this.$refs.selection as any).getSelection().length==1)
            // {
            //         const element = (this.$refs.selection as any).getSelection()[0];
            //         await this.$store.dispatch({
            //                 type:'archivebox/boxEmptyOutTask',
            //                 stgId: element.id
            //             })
            //             this.$Message.success('任务创建成功。');
            // }
            // else
            // {
            //     alert('先选择单条任务。');
            // }

        }

        
        get pageSize(){
            return this.$store.state.archivebox.pageSize;
        }
        get totalCount(){
            return this.$store.state.archivebox.totalCount;
        }
        get currentPage(){
            return this.$store.state.archivebox.currentPage;
        }

        columns=[{
            title:this.L('Id'),
            width: 55,
            key:'id'
        },{
            title:this.L('ArchiveBoxName'),
            key:'archiveBoxName'
        },{
            title:this.L('ArchiveBoxRfid'),
            key:'archiveBoxRfid'
        },{
            title:this.L('立卷时间'),
            key:'creationTime',
            render:(h:any,params:any)=>{
            return h('span',new Date(params.row.creationTime).getFullYear().toString())
            }
        },{
            title:this.L('保管期限'),
            key:'fullFlag',
            render:(h:any,params:any)=>{
            return params.row.fullFlag==null?h('span','10年'): h('span',params.row.fullFlag)
            }
        },{
            title:this.L('CreationTime'),
            key:'creationTime',
            render:(h:any,params:any)=>{
                return h('span',new Date(params.row.creationTime).toLocaleDateString())
            }
        },{
            title:this.L('CellName'),
            key:'cellName'
        },
        {
            title:this.L('StorageRemark'),
            key:'storageRemark'
        },{
            title:this.L('Actions'),
            key:'Actions',
            width:275,
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
                                this.$store.commit('archivebox/edit',params.row);
                                this.edit();
                            }
                        }
                    },this.L('Edit')),
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
                                this.$store.commit('archivebox/bind',params.row);
                                this.binding();
                            }
                        }
                    },this.L('Binding')),
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
                                this.$store.commit('archivebox/bindrfid',params.row);
                                this.bindingRfid();
                                //alert(params.row.id)
                                this.getpage2(params.row.id);
                            }
                        }
                    },this.L('BindingArchives')),
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
                                                type:'archivebox/delete',
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
                        width: 55,
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
        },
        {
            title:this.L('Actions'),
            key:'Actions',
            width:75,
            render:(h:any,params:any)=>{
                return h('div',[
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
                                            alert(params.row.storageId);
                                            await this.getpage2(params.row.storageId);
                                        }
                                })
                            }
                        }
                    },this.L('Delete'))
                ])
            }
        }]
        mounted(){
                         const s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = 'http://127.0.0.1:8008/YOWOCloudRFIDReader.js';
                document.body.appendChild(s);
        
        }
        async created(){
            this.getpage();
        }
    }
</script>