<template>
  <div>
    <Modal
      :title="L('任务异常报警')"
      :value="value"
      :width="700"
      @on-ok="save"
      @on-visible-change="visibleChange"      
    >
       <div class="margin-top-10">
          <Table  stripe highlight-row size="large" :columns="taskExpcolumns" :no-data-text="L('NoDatas')" border  ref="wcstaskselection"  :data="wcsTasklist">
           </Table>
       </div>
      <div slot="footer">
        <!-- <Button @click="redoTask" type="primary">{{ L("重发设备任务") }}</Button> -->
        <Button @click="forceFinishTask" type="primary">{{ L("强制完成任务") }}</Button>
        <Button @click="cancel" type="primary">{{ L("关闭") }}</Button>
      </div>
    </Modal>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "../../lib/util";
import AbpBase from "../../lib/abpbase";
import axios from "axios";
class TaskFaultDto {
  controlTaskId: number;
  deviceTaskId: number;
  manageTaskId: number;
  barcode: string;
  taskFaultDesc: string;
  faultCode:number;
  resolveMethod:string;
}
@Component
export default class TaskException extends AbpBase {
  @Prop({ type: Boolean, default: false }) value: boolean;
  wmsStatusInfo: string = "正在检测";
  checkFlag:boolean=false;

      taskExpcolumns=[{
                type: 'selection',
                width: 60,
                align: 'center'
            },
            {
            title:this.L('任务ID'),
            key:'controlTaskId',
            align: 'center',
            width: 85,
        },
        // {
        //     title:this.L('子任务ID'),
        //     key:'deviceTaskId'
        // },
        // {
        //     title:this.L('管理任务ID'),
        //     key:'manageTaskId'
            
        // },
        {
            title:this.L('档案盒条码'),
            align: 'center',
            key:'barcode',
            width: 110
        },       
         {
            title:this.L('错误代码'),
            align: 'center',
            key:'faultCode',
            width: 95
        }, 
         {
            title:this.L('任务异常描述'),
            align: 'center',
            key:'taskFaultDesc'
        }, 
         {
            title:this.L('异常处理建议'),
            align: 'center',
            key:'resolveMethod'
        }
        ]

  get wcsTasklist(){
      // return [{controlTaskId:1,deviceTaskId:123,manageTaskId:98555000,barcode:'8000555',taskFaultDesc:'错误asjdkasdjasdjkasdjajsdkadskjkadsjkajsdjkasdjkjabhjhjhhjhhhhjhjhjjhhj'}];
     return this.$store.state.stocktask.taskFaultlists;
  };

  async redoTask()
  {
       if((this.$refs.wcstaskselection as any).getSelection().length>0)
      {
                for (let index = 0; index < (this.$refs.wcstaskselection as any).getSelection().length; index++) {
                        const element = (this.$refs.wcstaskselection as any).getSelection()[index];
                        // alert(element.manageTaskId);
                        await this.$store.dispatch({
                                type:'stocktask/redoWcsTask',
                                data:{controlTaskId:element.controlTaskId,
                                deviceTaskId:element.deviceTaskId}
                            });
                        this.$Message.success('任务重发成功。'+element.controlTaskId);
                }
            }
            else
            {
                alert('先选择任务。');
            }
  }
  async forceFinishTask()
  {
      if((this.$refs.wcstaskselection as any).getSelection().length>0)
      {
                for (let index = 0; index < (this.$refs.wcstaskselection as any).getSelection().length; index++) {
                        const element = (this.$refs.wcstaskselection as any).getSelection()[index];
                                       await this.$store.dispatch({
                                                type:'stocktask/taskComplete',
                                                 mainId:element.manageTaskId
                                       })
                }
                          this.$Message.success({
                            content: '任务强制完成。',
                            duration: 5
                        });
            }
            else
            {
                alert('先选择任务。');
            }
  }
  save() {
    // (this.$refs.archiveboxForm as any).validate(async (valid:boolean)=>{
    //     if(valid){
    //         await this.$store.dispatch({
    //             type:'archivebox/create',
    //             data:this.archivebox
    //         });
    //         (this.$refs.archiveboxForm as any).resetFields();
    //         this.$emit('save-success');
    //         this.$emit('input',false);
    //     }
    // })
    this.$emit("save-success", false);
    this.$emit("input", false);
  }
  cancel() {
    // (this.$refs.archiveboxForm as any).resetFields();
    this.$emit("input", false);
  }


  async visibleChange(value: boolean) {
    if (!value) {
      this.$emit("input", value);
    } else {
      // await this.getAllStatus();
    }
  }
}
</script>
<style scoped>
.noselect {
  -moz-user-select: none; /*火狐*/

  -webkit-user-select: none; /*webkit浏览器*/

  -ms-user-select: none; /*IE10*/

  -khtml-user-select: none; /*早期浏览器*/

  user-select: none;
}
</style>
