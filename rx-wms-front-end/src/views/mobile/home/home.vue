<template>
  <div class="components-input-demo-presuffix">
    <a-page-header style="border: 1px solid rgb(235, 237, 240)" title="主页">
      <template #extra>
        <a-space>
          <!-- <close-circle-outlined @click="handleLoginOut" style="font-size: 24px" /> -->
        </a-space>
      </template>
    </a-page-header>
    <a-row justify="center" style="margin-top: 20px; margin-bottom: 20px">
      <h1>热处理:</h1>
      <a-col :span="8">
       
        <a-row justify="center">
          
          <a-space>
            <DatabaseTwoTone @click="putShelfStockIn" style="font-size: 72px" />
          </a-space>
        </a-row>
        <a-row justify="center">
          <span>入库</span>
        </a-row>
      </a-col>
      <a-col :span="8">
        <a-row justify="center">
          <a-space>
            <CodeTwoTone @click="EmptyoutCell" style="font-size: 72px" />
          </a-space>
        </a-row>
        <a-row justify="center">
          <span>补充空容器</span>
        </a-row>
      </a-col>
      
    </a-row>

    <a-row justify="center" style="margin-top: 20px; margin-bottom: 20px">
      <h1>基准面:</h1>
      <a-col :span="8">
        <a-row justify="center">
          <a-space>
            <DatabaseTwoTone @click="EmptyinCell" style="font-size: 72px" />
          </a-space>
        </a-row>
        <a-row justify="center">
          <span>空容器入库</span>
        </a-row>
      </a-col>
      <a-col :span="8">
        <a-row justify="center">
          <a-space>
            <CodeTwoTone @click="outCell" style="font-size: 72px" />
          </a-space>
        </a-row>
        <a-row justify="center">
          <span>出库</span>
        </a-row>
      </a-col>
    </a-row>

    <a-row justify="center" style="margin-top: 20px; margin-bottom: 20px">
      <!-- <a-col :span="8">
        <a-row justify="center">
          <a-space>
            <DatabaseTwoTone @click="stock" style="font-size: 72px" />
          </a-space>
        </a-row>
        <a-row justify="center">
          <span>任务管理</span>
        </a-row>
      </a-col> -->
      <a-col :span="8"></a-col>

    </a-row>
    
  </div>
</template>
<script lang="ts">
  import { computed } from 'vue';
  import { defineComponent, ref } from 'vue';
  import {
    UpSquareTwoTone,
    CloseCircleOutlined,
    DatabaseTwoTone,
    ContainerTwoTone,
    CodeTwoTone,
    CarryOutTwoTone,
    MinusSquareTwoTone,
  } from '@ant-design/icons-vue';
  import { router } from '/@/router';
  import { AppLocalePicker, AppDarkModeToggle } from '/@/components/Application';
  import { useGlobSetting } from '/@/hooks/setting';
  import { useI18n } from '/@/hooks/web/useI18n';
  import { useDesign } from '/@/hooks/web/useDesign';
  import { useLocaleStore } from '/@/store/modules/locale';
  import { useUserStore } from '/@/store/modules/user';
  export default defineComponent({
    name: 'MobileHome',
    components: {
      UpSquareTwoTone,
      CloseCircleOutlined,
      DatabaseTwoTone,
      ContainerTwoTone,
      CodeTwoTone,
      CarryOutTwoTone,
      MinusSquareTwoTone,
    },
    setup() {
      const { t } = useI18n();
      const userStore = useUserStore();
      const test = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/putshelfbefore');
      };

      const putshelfbefore = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/putshelfbefore');
      };

      const offshelfbefore = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/offshelfbefore');
      };

      const putshelfafter = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/putshelfafter');
      };
      const offshelfafter = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/offshelfafter');
      };
      const putShelfStockIn = async () => {
        // alert(1);
        // $router.go('/putshelfbefore');
        await router.replace('/putShelfStockIn');
      };
      const offShelfStockOut = async () => {
        await router.replace('/offShelfStockOut');
      };

      const outCell = async () => {
        await router.replace('/outCell');
      };
      const stock = async () => {
        await router.replace('/stockM');
      };

      const EmptyoutCell = async () => {
        await router.replace('/EmptyOutcell');
      };

      const EmptyinCell = async () => {
        await router.replace('/EmptyIncell');
      };
      

      function handleLoginOut() {
        userStore.mobilelogout(true);
      }
      return {
        test,
        stock,
        putshelfbefore,
        offshelfbefore,
        putshelfafter,
        offshelfafter,
        putShelfStockIn,
        offShelfStockOut,
        outCell,
        EmptyinCell,
        EmptyoutCell,
        t,
        handleLoginOut,
      };
    },
  });
</script>
