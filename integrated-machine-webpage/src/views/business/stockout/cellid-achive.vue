<template>
    <div>
        <Modal
         :title="L('选择出库')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
           <Card dis-hover>
            <div class="page-body">
                <Form ref="queryForm" :label-width="80" label-position="left" inline>
                    <Row :gutter="16">
                                    <div>

                    <RadioGroup v-model="cellZYDto.cell_z" type="button" button-style="solid" @on-change="isSelectChange">
                         <Radio v-for="n in maxZ" :key="n" :label=n>{{n}}排</Radio>
                     </RadioGroup>
                                    </div>
                                    <div class="margin-top-10">
                     <RadioGroup v-model="cellZYDto.cell_y" type="button" button-style="solid" @on-change="isSelectChange1">
                         <Radio v-for="n in maxY+1" :key="n-1" :label=n-1>{{n-1}}层</Radio>
                     </RadioGroup>
                                    </div>
                                    <div class="margin-top-10">
                     <RadioGroup  v-model="jie"  type="button"  @on-change="isSelectChange2">
                         <Radio v-for="n in maxJie" :key="n" :label=n>{{n}}节</Radio>
                     </RadioGroup>
                                    </div>

                            <div class="margin-top-10">

                               <svg :width="archiveCounts*30+5"  height="100"  :viewBox="'0 0 '+(archiveCounts*35+5)+' 100'" class="js-calendar-graph-svg">
                         
                                <g >
                                    <rect id="rect_1" height="100" :width="archiveCounts*35+5" y="0" x="0" stroke-width="1" stroke="#dcdee2"  fill="#fff"></rect>
                                    <rect stroke-width="1" stroke="#dcdee2"  @click="showBTTcell(item.cell_x,item.id)" v-for="item in listcells.filter(a=>a.cell_x<=archiveCounts*jie&a.cell_x>archiveCounts*(jie-1))" :key="item.id" :class="'rect '+item.cellStatus+' '+item.runStatus" :x=(item.cell_x-1-archiveCounts*(jie-1))*35+5 :y=5  :data-id="item.id" :id="item.cellCode" ></rect>
                                </g>                    
                        
                               <text   v-for="item in listcells.filter(a=>a.cell_x<=archiveCounts*jie&a.cell_x>archiveCounts*(jie-1))" :key="item.id" :x=(item.cell_x-1-archiveCounts*(jie-1))*35+15 :y="50" fill="#515a6e"  class="month">{{item.cell_x}}</text>

                                </svg>
                                <h4>选择区域：{{areaCode}}</h4>
                                <h4>档案盒名：{{storageMain.archiveBoxName}}</h4>
                                <h4>档案标签：{{storageMain.archiveBoxRfid}}</h4>
                      </div>
                    </Row>
                    <Row>
                        <!-- <Button type="primary"  @click="outByArea" class="toolbar-btn">{{L('创建批量出库')}}</Button> -->
                        <p></p>
                    </Row>
                </Form>
            </div>
        </Card>
    </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import PageRequest from '@/store/entities/page-request'
    import Stocktask from '@/store/entities/stocktask';
    import StorageMain from '@/store/entities/archivebox';
    class  PageCellRequest extends PageRequest{
        keyword:string;
        cellStatus:string;
        runStatus:string;
        from:Date;
        to:Date;
    }
        class batInDto {      
            cellId: number;    
            constructor ( cid: number) {
                this.cellId =cid;
            };
        
        }
     class CellZYDto {      
        cell_z: number=1;
        cell_y: number=0;      
        }

    @Component
    export default class CellsBatTask extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        creationTime:Date[]=[];
        stocktask:Stocktask=new Stocktask();

        //filters
        pagerequest:PageCellRequest=new PageCellRequest();
        rowcn:number=1;
        cellCode:string="";
        cellId:number=0;
        cell_x:number=0;
        cellZYDto:CellZYDto=new CellZYDto();
        jie:number=1;
        // archiveCounts:number=13;//定义一节所拥有的档案数量
        //areaCode:string="";
        //更新库位查询信息
        async getCells(){
            this.storageMain.archiveBoxName = undefined
            this.storageMain.archiveBoxRfid = undefined
            this.storageMain.id = undefined;
            this.$store.dispatch({
                        type:'stocktask/getCells',
                        data:this.cellZYDto
                    });
        }
        isSelectChange(val:number){

           // console.log(val);
            this.rowcn=val;
            this.getCells()
        }
        async isSelectChange1(val:number){
            //console.log(val);
            //alert(val);
            this.getCells()
                      // this.taskAssignDto.cellcode="";
        }
       isSelectChange2(val:number){
            this.jie=val;
            this.cell_x=0;
            this.getCells()
        }
        get areaCode()
        {
           return this.cellZYDto.cell_z+'-'+this.cell_x+'-'+this.cellZYDto.cell_y;
        }
                //选择目标库位
        async showBTTcell(cell_x:number,cid:number)
        {       this.cell_x = cell_x
             await this.$store.dispatch({
                type:'cell/getBoxByCellId',
                data:cid
                })
                
       }
       get storageMain(){
        return this.$store.state.cell.storageMain ? this.$store.state.cell.storageMain as StorageMain : new StorageMain();
        
       }
        get list(){
            console.log(this.$store.state.cell.list);
            return this.$store.state.cell.list;
        };
        get maxX(){
            return this.$store.state.cell.maxX;
        };
        get maxY(){
        //  console.log(this.$store.state.cell.cell_zMax);
            return this.$store.state.cell.maxY;
        };
        get maxZ(){
            return this.$store.state.cell.maxZ;
        };
        //获取最大节数
       get maxJie(){
            // return Math.ceil(this.maxX/this.archiveCounts);
            return this.$store.state.cell.maxJie;
        };

       get archiveCounts(){
            // return Math.ceil(this.maxX/this.archiveCounts);
            return this.$store.state.cell.jieCounts;
        };
                get listcells(){
            //console.log(this.$store.state.stocktask.cells);
            return this.$store.state.stocktask.cells;
        };
        async getpage(){
          
                     await this.$store.dispatch({
                         type:'stocktask/getCells',
                         data:this.cellZYDto
                     });
        }


        //根据选择库位编码，创建出库任务
        async save(){
            try {
            if(this.storageMain.id == undefined)
            {
                 this.$Message.error('请先选择有货库位');
                 return;
            }
            // if((this.$refs.selection as any).getSelection().length>1)
            // {
            //      this.$Message.success('请选择单个档案');
            //      return;
            // }

                                await this.$store.dispatch({
                                    type:'archivebox/boxEmptyOutTask',
                                    stgId: this.storageMain.id
                                }).then( async (response) => {
                                                    this.$Message.success('任务下达成功。');    
                                                    // await this.getpage();
                                                    //  await  this.getpage2(0);
                                            })
                                            .catch( (error) => {
                                                console.log(error);
                                            }); 
            } catch (error) {
                 console.log(error);
            }
        }

        

        // async createBatCheck(){          
        //    await this.$store.dispatch({
        //         type:'check/CreateCheckByArea',
        //         areaCode:this.areaCode
        //     })
        //     this.$Message.success('批量盘点任务创建成功。');
        // }

        // async createBatStockIn(){
        //    await this.$store.dispatch({
        //         type:'cell/batoutByArea',
        //         areaCode:this.areaCode
        //     })
        //     this.$Message.success('批量入库任务创建成功。');
        // }
        

        get pageSize(){
            return this.$store.state.cell.pageSize;
        }
        get totalCount(){
            return this.$store.state.cell.totalCount;
        }
        get currentPage(){
            return this.$store.state.cell.currentPage;
        }
        async created(){
            this.getpage();
            await this.$store.dispatch({
                type:'cell/getcellmax'
            })
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                // this.stocktask=Util.extend(true,{},this.$store.state.stocktask.editStocktask);
                this.pagerequest.keyword='';
                this.storageMain.archiveBoxName = undefined
                this.storageMain.archiveBoxRfid = undefined
                this.storageMain.id = undefined;
                this.cellZYDto.cell_z = 1;
                this.cell_x = 0;
                this.cellZYDto.cell_y = 0;
                this.getpage();
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
.rect{
    width:30px;
    height: 90px;
    /* fill="#ebedf0" */

}
.Full{
   fill:aqua;
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
svg{
margin-left: 10px;
}
</style> 