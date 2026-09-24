<template>
    <div>
        <Modal
         :title="L('EditArchiveBoxlist')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="archiveBoxlistForm"  label-position="top" :rules="archiveBoxlistRule" :model="archiveboxlist">
                <FormItem :label="L('Id')" prop="Id">
                    <Input  disabled="disabled"  v-model="archiveboxlist.id" :maxlength="32" :minlength="2"></Input>
                </FormItem>
                <FormItem :label="L('StorageListRemark')" prop="StorageListRemark">
                    <Input v-model="archiveboxlist.storageListRemark" :maxlength="256"></Input>
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
    import Archiveboxlist from '@/store/entities/archiveboxlist';
    @Component
    export default class EditArchiveBoxlist extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        archiveboxlist:Archiveboxlist=new Archiveboxlist();
        save(){
            (this.$refs.archiveBoxlistForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'archivebox/update',
                        data:this.archiveboxlist
                    });
                    (this.$refs.archiveBoxlistForm as any).resetFields();
                    this.$emit('save-success');
                    this.$emit('input',false);
                }
            })
        }
        cancel(){
            (this.$refs.archiveBoxlistForm as any).resetFields();
            this.$emit('input',false);
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                this.archiveboxlist=Util.extend(true,{},this.$store.state.archiveboxlist.editArchiveBoxlist);
            }
        }
        archiveBoxlistRule={
            archiveBoxName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('ArchiveBoxName')),trigger: 'blur'}]
        }
    }
</script>

