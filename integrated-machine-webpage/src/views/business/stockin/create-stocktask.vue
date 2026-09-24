<template>
    <div>
        <Modal
         :title="L('指定库位下达入库')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="archiveboxForm"  label-position="top" :rules="archiveboxRule" :model="taskAssignDto">
                <FormItem :label="L('任务ID')" prop="archiveBoxName">
                    <Input  disabled="disabled"  v-model="taskAssignDto.mainId" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                  <FormItem :label="L('选择货架：')" clearable style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')"  v-model="cellZYDto.cell_z" @on-change="isSelectChange">
                                    <Option v-for="n in 6" :key="n"  :value=n>第{{n}}排</Option>
                                </Select>
                 </FormItem>
                 <FormItem :label="L('选择层数：')" clearable style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')"  v-model="cellZYDto.cell_y"  @on-change="isSelectChange1">
                                    <Option v-for="n in 6" :key="n"  :value=n>第{{n}}层</Option>
                                </Select>
                 </FormItem>
                  <FormItem :label="L('选择库位：')" clearable style="width:100%">
                                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                                <Select :placeholder="L('Select')" v-model="taskAssignDto.cellcode"  >
                                    <Option v-for="item in list.filter(a=>a.runStatus=='Enable'&a.cellStatus=='Nohave')" :key="item.id"  :value="item.cellCode">{{item.cellCode}}</Option>
                                </Select>
                 </FormItem>
                <!-- <FormItem :label="L('库位编码')" prop="cellcode">
                    <Input v-model="taskAssignDto.cellcode" :maxlength="32"></Input>
                </FormItem> -->
            </Form>
            <div slot="footer">
                <Button @click="cancel">{{L('Cancel')}}</Button>
                <Button @click="save" type="primary">{{L('OK')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import Archivebox from '@/store/entities/archivebox';
    class TaskAssignDto {      
    mainId: number=0;
    cellcode: string="";       
    }
    class CellZYDto {      
    cell_z: number=1;
    cell_y: number=1;      
    }
    @Component
    export default class CreateArchiveBox extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        @Prop()mianId:number;
        taskAssignDto:TaskAssignDto=new TaskAssignDto();
        cellZYDto:CellZYDto=new CellZYDto();
         get list(){
            //console.log(this.$store.state.stocktask.cells);
            return this.$store.state.stocktask.cells;
        };
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
        save(){
            (this.$refs.archiveboxForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'stocktask/taskAssignCell',
                        data:this.taskAssignDto
                    });
                    (this.$refs.archiveboxForm as any).resetFields();
                    this.$emit('save-success');
                    this.$emit('input',false);
                }
            })
        }
        cancel(){
            (this.$refs.archiveboxForm as any).resetFields();
            this.$emit('input',false);
        }
        async visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }
             if(value){
                 if(this.mianId==0)
                 {
                    alert('请先选择任务。');
                    this.$emit('input',false);
                 }
                 this.taskAssignDto.mainId=this.mianId;
                 await this.$store.dispatch({
                        type:'stocktask/getCells',
                        data:this.cellZYDto
                    });
            }
        }
        archiveboxRule={
            cellcode:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('库位编码')),trigger: 'blur'}]
        }
    }
</script>

