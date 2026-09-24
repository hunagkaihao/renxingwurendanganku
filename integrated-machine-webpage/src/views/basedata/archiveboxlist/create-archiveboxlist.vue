<template>
    <div>
        <Modal
         :title="L('CreateNewArchiveBox')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="archiveboxForm"  label-position="top" :rules="archiveboxRule" :model="archivebox">
                <FormItem :label="L('ArchiveBoxName')" prop="archiveBoxName">
                    <Input v-model="archivebox.archiveBoxName" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('ArchiveBoxRfid')" prop="archiveBoxRfid">
                    <Input v-model="archivebox.archiveBoxRfid" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('CellId')" prop="cellId">
                    <Input v-model="archivebox.cellId" :maxlength="32"></Input>
                </FormItem>
                <FormItem :label="L('StorageRemark')" prop="storageRemark">
                    <Input v-model="archivebox.storageRemark" :maxlength="256"></Input>
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
    import Archivebox from '@/store/entities/archivebox';
    @Component
    export default class CreateArchiveBox extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        archivebox:Archivebox=new Archivebox();
        save(){
            (this.$refs.archiveboxForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'archivebox/create',
                        data:this.archivebox
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
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }
        }
        archiveboxRule={
            archiveBoxName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('ArchiveBoxName')),trigger: 'blur'}]
        }
    }
</script>

