<template>
    <div>
        <Modal
         :title="L('CreateNewArchive')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="archiveForm"  label-position="top" :rules="archiveRule" :model="archive">
                <FormItem :label="L('GoodsCode')" prop="goodsCode">
                    <Input v-model="archive.goodsCode" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('GoodsName')" prop="goodsName">
                    <Input v-model="archive.goodsName" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('GoodsUnits')" prop="goodsUnits">
                    <Input v-model="archive.goodsUnits" :maxlength="32"></Input>
                </FormItem>
                <FormItem :label="L('GoodsRemark')" prop="goodsRemark">
                    <Input v-model="archive.goodsRemark" :maxlength="256"></Input>
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
    export default class CreateArchive extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        archive:Archive=new Archive();
        save(){
            (this.$refs.archiveForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'archive/create',
                        data:this.archive
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
            }
        }
        archiveRule={
            goodsCode:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('GoodsCode')),trigger: 'blur'}],
            goodsName:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('GoodsName')),trigger: 'blur'}]
        }
    }
</script>

