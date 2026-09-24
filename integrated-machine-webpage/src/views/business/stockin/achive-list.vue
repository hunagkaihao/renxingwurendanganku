<template>
    <div>
        <Modal
         :title="L('档案文件校验')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <!-- <Form ref="archiveBoxForm">
            </Form> -->
               <Row :gutter="24">
                            <Col span="16">
                    <label class="mylabel">档案盒：{{stocktask.archiveBoxRfid}}--{{stocktask.archiveBoxName}}</label>
                            </Col>
                             <Col span="8">
                    <label  class="mylabel">校对进度：{{this.list2.filter(x=>x.goodsProperty8=='1').length}}/{{list2.length}}</label>
                            </Col>
            </Row>
             <div class="margin-top-10">
                <Table :columns="columns2" :no-data-text="L('NoDatas')" border :data="list2" :height="230" >
                    </Table>
             </div>
            <div slot="footer">
                <Button @click="cancel">{{L('取消')}}</Button>
                <Button @click="save">{{L('再次校验')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import Stocktask from '@/store/entities/stocktask';
    @Component
    export default class EditArchiveBox extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        stocktask:Stocktask=new Stocktask();
         columns2=[{
            title:this.L('BoxBarcode'),
            key:'boxBarcode',
            width: 100
        },{
            title:this.L('GoodsCode'),
            key:'goodsCode',
            width: 100
        },{
            title:this.L('GoodsName'),
            key:'goodsName'
        },{
            title:this.L('校验结果'),
            key:'goodsProperty8',
            width: 100,
            render:(h:any,params:any)=>{
            return params.row.goodsProperty8=="1"?h('span','OK'): h('span','')
            }
        }]
        save(){
            try {
                // @ts-ignore：无法被执行的代码的错误
                bound.recheck();
                this.$Message.info('再次启动校验');
            } catch (error) {
                
            }
            //    // @ts-ignore：无法被执行的代码的错误
            //     bound.recheck();
            //   this.$Message.info('再次启动校验');
            // (this.$refs.archiveBoxForm as any).validate(async (valid:boolean)=>{
            //     if(valid){
            //        this.$Message.info('校验完成，下达开门指令');
            //         // await this.$store.dispatch({
            //         //     type:'archivebox/update',
            //         //     data:this.stocktask
            //         // });
            //         // (this.$refs.archiveBoxForm as any).resetFields();
            //         this.$emit('save-success');
            //         this.$emit('input',false);
            //     }
            // })
        }
        cancel(){
            // (this.$refs.archiveBoxForm as any).resetFields();
            this.$emit('input',false);
        }
        get list2(){
            return this.$store.state.stocktask.stocktasklists;
        };
        timeJob1()
        {
            this.$store.dispatch({
                            type:'stocktask/getLists',
                            mainId:this.stocktask.id
                        }).then( async (response) => {
                            if(this.list2.length>0)
                            {
                                if(this.list2.filter(x=>x.goodsProperty8!='1').length==0)
                                {
                                    //下达开门指令
                                     this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:this.stocktask.id
                                });
                                     this.$emit('input',false);
                                     this.$Message.info('开门命令已下达');
                                }
                            }
                            else
                            {
                                 //下达开门指令
                                 this.$store.dispatch({
                                    type:'stocktask/openDoor',
                                    mId:this.stocktask.id
                                });
                                     this.$emit('input',false);
                                      this.$Message.info('开门命令已下达');
                            }
                        })
                        .catch( (error) => {
                            console.log(error);
                        }); 
        }
                intervalId:number=0;
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
                window.clearInterval(this.intervalId);
            }else{
                this.stocktask=Util.extend(true,{},this.$store.state.stocktask.editStocktask);
                 this.intervalId= setInterval(this.timeJob1, 3000);
            }
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