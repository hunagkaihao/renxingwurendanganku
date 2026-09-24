<template>
    <div>
        <Modal
         :title="L('查询档案盒')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <!-- <Form ref="archiveBoxForm" :label-width="80" label-position="left" inline >             -->
               <Row :gutter="24">
                       <Col span="12">
                            <!-- <FormItem :label="L('Keyword')+':'" style="width:100%"> -->
                                <Input v-model="pagerequest.keyword" :placeholder="L('案卷题名')+'/'+L('案卷标签')"  clearable required autofocus  @keyup.enter.native="getpage"><Icon type="ios-keypad" slot="suffix"  @click="openKeyboard" /></Input>
                            <!-- </FormItem> -->
                        </Col>
                         <Col span="12">
                            <Button icon="ios-search"  @click="getpage" class="toolbar-btn"></Button>
                        </Col>
                </Row>
                    <!-- <Row>
                        <Button icon="ios-search" type="primary" @click="getpage" class="toolbar-btn"></Button>
                    </Row> -->
            <!-- </Form> -->
             <div class="margin-top-10">
                <Table :columns="columns2"  stripe highlight-row  :no-data-text="L('NoDatas')" border  ref="selection" :data="list2" :height="230" >
                    </Table>
             </div>
            <div slot="footer">
                <Button @click="cancel">{{L('取消')}}</Button>
                <Button @click="save">{{L('下达出库')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import Stocktask from '@/store/entities/stocktask';
        class  PageArchiveBoxRequest extends PageRequest{
        keyword:string;
        cellCode:string;
        cellId:number=88888;
        from:Date;
        to:Date;
    }
    @Component
    export default class SearchArchive extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
                //filters
        pagerequest:PageArchiveBoxRequest=new PageArchiveBoxRequest();
        creationTime:Date[]=[];
        stocktask:Stocktask=new Stocktask();
         columns2=[
               {
                type: 'selection',
                width: 60,
                align: 'center',
            },    
                {
            title:this.L('案卷题名'),
            width: 140,
            align: 'center',
            key:'archiveBoxName'
        },{
            title:this.L('档案标签'),
            width: 140,
            align: 'center',
            key:'archiveBoxRfid'
        },
        {
            title:this.L('CellName'),
            width: 140,
            align: 'center',
            key:'cellName'
        }]
        // async save(){
        //     try {
        //     if((this.$refs.selection as any).getSelection().length==0)
        //     {
        //          this.$Message.success('请先选择档案盒');
        //          return;
        //     }
        //     if((this.$refs.selection as any).getSelection().length>1)
        //     {
        //          this.$Message.success('请选择单个档案');
        //          return;
        //     }

        //                         await this.$store.dispatch({
        //                             type:'archivebox/boxEmptyOutTask',
        //                             stgId: (this.$refs.selection as any).getSelection()[0].id
        //                         }).then( async (response) => {
        //                                             this.$Message.success('任务下达成功。');    
        //                                             // await this.getpage();
        //                                             //  await  this.getpage2(0);
        //                                     })
        //                                     .catch( (error) => {
        //                                         console.log(error);
        //                                     }); 
        //     } catch (error) {
        //          console.log(error);
        //     }
        // }
        async save(){
            try {
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

                                
            } catch (error) {
                 console.log(error);
            }
        }
        cancel(){
            // (this.$refs.archiveBoxForm as any).resetFields();
            this.$emit('input',false);
        }
        get list2(){
            return this.$store.state.archivebox.list;
        };
        get pageSize(){
            return this.$store.state.archivebox.pageSize;
        }
        get totalCount(){
            return this.$store.state.archivebox.totalCount;
        }
        get currentPage(){
            return this.$store.state.archivebox.currentPage;
        }
        async getpage(){
            // this.pagerequest.maxResultCount=this.pageSize;
            this.pagerequest.maxResultCount=20;
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
            this.pagerequest.cellId=999;
            this.pagerequest.cellCode="Enable";

            await this.$store.dispatch({
                type:'archivebox/getAll',
                data:this.pagerequest
            })
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                // this.stocktask=Util.extend(true,{},this.$store.state.stocktask.editStocktask);
                this.pagerequest.keyword='';
                this.getpage();
            }
        }
        
        openKeyboard()
        {
                try {
                    console.log("openkeyboard");
                    //打开键盘
                // @ts-ignore：无法被执行的代码的错误
                veinjs.openosk();
                } catch (error) {}
        }
    }
</script>
<style scoped>
.mylabel{
    margin-left: 10px;
    font-size: 13px;
    /* font-family: "Myriad Pro","Helvetica Neue",Arial,Helvetica,sans-serif; */
    /* font-weight: 600; */
    font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB", "Microsoft YaHei", "\5FAE\8F6F\96C5\9ED1", Arial, sans-serif;
}

  </style>  