<template>
    <div style="width: 100%;height: 100%;background: #667aa6">
          <Button type="primary" @click="getMedia2" long size="large" >开启摄像头</Button>
       <div>
    <Steps :current="1">
        <Step title="已完成" content="人脸验证"></Step>
        <Step title="进行中" content="放置档案盒"></Step>
        <Step title="待进行" content="执行入库"></Step>
        <Step title="待进行" content="入库完成"></Step>
    </Steps>
    </div>
    </div>
</template>

<script  lang="ts">
import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
import AbpBase from '../../lib/abpbase'
import Cookies from 'js-cookie';
import lockScreen from './lockscreen.vue';
    @Component({
      components:{lockScreen}
    })


export default class Face extends AbpBase {
    getMedia2() {

    }
}
</script>