<template>
    <div>
        <Modal
         :title="L('放入档案提示')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
         :mask="false"
         :styles="{top: '250px'}"
         :footer-hide="true"
        >
            <!-- <Form ref="archiveBoxForm">
            </Form> -->
               <Row :gutter="24">
                            <Col span="10">
                            <div  id="boxsvg">
                    <svg width="100" height="300" xmlns="http://www.w3.org/2000/svg">
                            <g id="Layer_1">
                            <title>Layer 1</title>
                            <g transform="matrix(1.28846, 0, 0, 1.31731, -62.6923, -114.019)" id="svg_22">
                            <svg y="240.48176" x="57.31342" t="1648558518440" class="iconbar" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2379" width="64" height="64" id="svg_4">
                                <path d="m597.33333,597.33333l85.33334,0l0,-85.33333l85.33333,0l0,128l-85.33333,0l0,42.66667l-85.33334,0l0,-42.66667l-85.33333,0l0,-128l85.33333,0l0,85.33333zm-384,-85.33333l256,0l0,256l-256,0l0,-256zm85.33334,85.33333l0,85.33334l85.33333,0l0,-85.33334l-85.33333,0zm-85.33334,-384l256,0l0,256l-256,0l0,-256zm85.33334,85.33334l0,85.33333l85.33333,0l0,-85.33333l-85.33333,0zm213.33333,-85.33334l256,0l0,256l-256,0l0,-256zm85.33333,85.33334l0,85.33333l85.33334,0l0,-85.33333l-85.33334,0zm85.33334,384l85.33333,0l0,85.33333l-85.33333,0l0,-85.33333zm-170.66667,0l85.33333,0l0,85.33333l-85.33333,0l0,-85.33333z" fill="#444444" p-id="2380" id="svg_5"/>
                            </svg>
                            </g>
                            <rect stroke-width="2" id="svg_16" height="280" width="81" y="10" x="10" stroke="#000" fill="none"/>
                            </g>

                            </svg>
                            </div>
                            </Col>
                             <Col span="14">
                             <div style="margin-top: 60px;"></div>
                    <label  class="mylabel">放置档案盒时，请注意二维码标签一侧面向无人库。</label>
                     <label  class="mylabel">无二维码标签一侧面向用户。</label>
                            </Col>
            </Row>
            <!-- <div slot="footer">
                <Button @click="cancel">{{L('取消')}}</Button>
                <Button @click="save">{{L('再次校验')}}</Button>
            </div> -->
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    @Component
    export default class PutboxTip extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        save(){
            // try {
            //     // @ts-ignore：无法被执行的代码的错误
            //     bound.recheck();
            //     this.$Message.info('再次启动校验');
            // } catch (error) {
                
            // }
        }
        cancel(){
            // (this.$refs.archiveBoxForm as any).resetFields();
            this.$emit('input',false);
        }

        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                try {
                    // @ts-ignore：无法被执行的代码的错误
                    bound.speakmsg("请放入档案盒，注意标签面向库房");
                } catch (error) {
                    
                }
            }
        }
    }
</script>
<style scoped>
.mylabel{
    margin-left: 10px;
    font-size: 20px;
    /* font-family: "Myriad Pro","Helvetica Neue",Arial,Helvetica,sans-serif; */
    /* font-weight: 600; */
    font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB", "Microsoft YaHei", "\5FAE\8F6F\96C5\9ED1", Arial, sans-serif;
}
#boxsvg
{
    animation: re-rotate 4s infinite linear;
    -webkit-animation: re-rotate 4s infinite linear;
    -moz-animation: re-rotate 4s infinite linear;
    transform-origin:top center;
    -webkit-transform-origin:top center;
    -moz-transform-origin:top center;

}
.iconbar
{ 
        animation: re-lost 4s infinite linear;
    -webkit-animation: re-lost 4s infinite linear;
    -moz-animation: re-lost 4s infinite linear;
    /* transform-origin:top center;
    -webkit-transform-origin:top center;
    -moz-transform-origin:top center; */
 }
@keyframes re-rotate{
    0%{
        transform: rotateY(0deg);
    }
     70%{
        transform: rotateY(-80deg);
    }
    100%{
        transform: rotateY(-180deg);
    }
}
@keyframes re-lost{
    0%{
        opacity: 1;
    }
         70%{
         opacity: 0.1;
    }
    100%{
        opacity: 0;
    }
}



  </style>  