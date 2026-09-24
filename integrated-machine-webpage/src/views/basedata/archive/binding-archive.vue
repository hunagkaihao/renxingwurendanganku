<template>
    <div>
        <Modal
         :title="L('BindingArchive')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="archiveForm"  label-position="top" :rules="archiveRule" :model="archive">
                <FormItem :label="L('GoodsCode')" prop="goodsCode">
                    <Input disabled="disabled" v-model="archive.goodsCode" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                 <FormItem :label="L('RfidId')" prop="rfidId">
                    <Input v-model="archive.rfidId" :maxlength="32" :minlength="2"></Input>
                </FormItem>
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
    import Archive from '@/store/entities/archive';
    @Component
    export default class BindingArchive extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        archive:Archive=new Archive();
        save(){
            (this.$refs.archiveForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'archive/binding',
                        data:   {
                            "goodsMainId": this.archive.id,
                            "rfidId": this.archive.rfidId
                        }
                    });
                    (this.$refs.archiveForm as any).resetFields();
                    this.$emit('save-success');
                    this.$emit('input',false);
                }
            })
        }
        cancel(){
            (this.$refs.archiveForm as any).resetFields();
            this.$emit('input',false);
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                this.archive=Util.extend(true,{},this.$store.state.archive.bindingArchive);
            }
        }
        archiveRule={
            rfidId:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('RfidId')),trigger: 'blur'}]
        }
    }
</script>

