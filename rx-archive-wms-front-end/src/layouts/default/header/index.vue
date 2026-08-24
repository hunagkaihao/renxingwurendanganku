<template>
  <Header :class="getHeaderClass">
    <!-- left start -->
    <div :class="`${prefixCls}-left`">
      <!-- logo -->
      <AppLogo
        v-if="getShowHeaderLogo || getIsMobile"
        :class="`${prefixCls}-logo`"
        :theme="getHeaderTheme"
        :style="getLogoWidth"
      />
      <LayoutTrigger
        v-if="
          (getShowContent && getShowHeaderTrigger && !getSplit && !getIsMixSidebar) || getIsMobile
        "
        :theme="getHeaderTheme"
        :sider="false"
      />
      <LayoutBreadcrumb v-if="getShowContent && getShowBread" :theme="getHeaderTheme" />
    </div>
    <!-- left end -->

    <!-- menu start -->
    <div :class="`${prefixCls}-menu`" v-if="getShowTopMenu && !getIsMobile">
      <LayoutMenu
        :isHorizontal="true"
        :theme="getHeaderTheme"
        :splitType="getSplitType"
        :menuMode="getMenuMode"
      />
    </div>
    <!-- menu-end -->

    <!-- action  -->
    <div :class="`${prefixCls}-action`">
      <ErrorAction v-if="getUseErrorHandle" :class="`${prefixCls}-action__item error-action`" />

      <home-outlined />
      <a-select
      ref="select"
      v-model:value="value1"
      style="width: 120px"
      @focus="focus"
      @change="handleChange"
      >
      <!-- 没有前部添加icon -->
      <!-- <template #itemIcon><home-outlined class="ant-select-suffix" /></template> -->
      <a-select-option v-for="n in options" :value="n.value" :key="n.label">{{n.value}}</a-select-option>
      <!-- <a-select-option v-for="n in options" :value="n.label + n.value" :key="n.label">{{n.value}}</a-select-option> -->
    </a-select>

      <Notify
        :class="`${prefixCls}-action__item notify-item`"
        :textMessage="textMessage"
        :broadCastMessage="broadCastMessage"
        @click="clickNotify"
      />

      <FullScreen v-if="getShowFullScreen" :class="`${prefixCls}-action__item fullscreen-item`" />

      <AppLocalePicker
        v-if="getShowLocalePicker"
        :reload="true"
        :showText="false"
        :class="`${prefixCls}-action__item`"
      />

      <UserDropDown :theme="getHeaderTheme" />

      <SettingDrawer v-if="getShowSetting" :class="`${prefixCls}-action__item`" />
    </div>
  </Header>
</template>
<script lang="ts">
  import { defineComponent, unref, computed, onMounted, reactive, toRefs } from 'vue';

  import { propTypes } from '/@/utils/propTypes';

  import { Layout } from 'ant-design-vue';
  import { AppLogo } from '/@/components/Application';
  import LayoutMenu from '../menu/index.vue';
  import LayoutTrigger from '../trigger/index.vue';

  import { AppSearch } from '/@/components/Application';

  import { useHeaderSetting } from '/@/hooks/setting/useHeaderSetting';
  import { useMenuSetting } from '/@/hooks/setting/useMenuSetting';
  import { useRootSetting } from '/@/hooks/setting/useRootSetting';

  import { MenuModeEnum, MenuSplitTyeEnum } from '/@/enums/menuEnum';
  import { SettingButtonPositionEnum } from '/@/enums/appEnum';
  import { AppLocalePicker } from '/@/components/Application';

  import { UserDropDown, LayoutBreadcrumb, FullScreen, Notify, ErrorAction } from './components';
  import { useAppInject } from '/@/hooks/web/useAppInject';
  import { useDesign } from '/@/hooks/web/useDesign';

  import { createAsyncComponent } from '/@/utils/factory/createAsyncComponent';
  import { useLocale } from '/@/locales/useLocale';
  import { useSignalR } from '/@/hooks/web/useSignalR';
  import { string } from '/@/services/ServiceProxies';

  import {
  HomeOutlined,
} from '@ant-design/icons-vue';
  import{ useUserStore } from '/@/store/modules/user'
  import { router } from '/@/router';
  import { useTabs } from '/@/hooks/web/useTabs';
  import { getWareListAsync} from './cell';
  import {WarehouseDto ,PagingWarehouseListInput} from '/@/services/ServiceProxies';
import { WareInfo } from '/#/store';

  export default defineComponent({
    name: 'LayoutHeader',
    components: {
      HomeOutlined,
      Header: Layout.Header,
      AppLogo,
      LayoutTrigger,
      LayoutBreadcrumb,
      LayoutMenu,
      UserDropDown,
      AppLocalePicker,
      FullScreen,
      Notify,
      AppSearch,
      ErrorAction,
      SettingDrawer: createAsyncComponent(() => import('/@/layouts/default/setting/index.vue'), {
        loading: true,
      }),
    },
    props: {
      fixed: propTypes.bool,
    },
    setup(props) {
      const { prefixCls } = useDesign('layout-header');
      const {
        getShowTopMenu,
        getShowHeaderTrigger,
        getSplit,
        getIsMixMode,
        getMenuWidth,
        getIsMixSidebar,
      } = useMenuSetting();
      const { getUseErrorHandle, getShowSettingButton, getSettingButtonPosition } =
        useRootSetting();

      const {
        getHeaderTheme,
        getShowFullScreen,
        getShowNotice,
        getShowContent,
        getShowBread,
        getShowHeaderLogo,
        getShowHeader,
        getShowSearch,
      } = useHeaderSetting();

      const { getShowLocalePicker } = useLocale();

      const { getIsMobile } = useAppInject();

      const getHeaderClass = computed(() => {
        const theme = unref(getHeaderTheme);
        return [
          prefixCls,
          {
            [`${prefixCls}--fixed`]: props.fixed,
            [`${prefixCls}--mobile`]: unref(getIsMobile),
            [`${prefixCls}--${theme}`]: theme,
          },
        ];
      });

      const getShowSetting = computed(() => {
        if (!unref(getShowSettingButton)) {
          return false;
        }
        const settingButtonPosition = unref(getSettingButtonPosition);

        if (settingButtonPosition === SettingButtonPositionEnum.AUTO) {
          return unref(getShowHeader);
        }
        return settingButtonPosition === SettingButtonPositionEnum.HEADER;
      });

      const getLogoWidth = computed(() => {
        if (!unref(getIsMixMode) || unref(getIsMobile)) {
          return {};
        }
        const width = unref(getMenuWidth) < 180 ? 180 : unref(getMenuWidth);
        return { width: `${width}px` };
      });

      const getSplitType = computed(() => {
        return unref(getSplit) ? MenuSplitTyeEnum.TOP : MenuSplitTyeEnum.NONE;
      });

      const getMenuMode = computed(() => {
        return unref(getSplit) ? MenuModeEnum.HORIZONTAL : null;
      });
      const { startConnect } = useSignalR();
      
      // onMounted(() => {
      //   startConnect();
      //   GetWare();
      // });
      let textMessage: string[] = [];
      let broadCastMessage: string[] = [];
      const notifiData = reactive({
        textMessage,
        broadCastMessage,
      });
      const clickNotify = async () => {

        console.log(notifiData);
      };
      const { closeAll } = useTabs();

      const handleChange = (value: string) => {
        cellStore.setCell(value);
        //移除tab
        
        closeAll();
        router.replace('/dashboard/analysis');
    };
    const focus = () => {
      //console.log(cellStore.cell);
    };

    //获取仓库清单
      let list : Array<WarehouseDto>
      let parms = new PagingWarehouseListInput()

      async function GetWare() {
        await getWareListAsync(parms).then((result)=>{
          list = result.items as WarehouseDto[]
          //console.log(list.length)
        })
        options.length = 0
        for (let index = 0; index < list.length; index++) {
          const a = ({value:'',label:''}) ;
          const c:WareInfo = ({wareid:0,warename:''});
          a.value = list[index].warehouseName as string
          a.label = list[index].id 
          c.wareid = list[index].id
          c.warename = list[index].warehouseName as string
          options.push(a)
          b.push(c)
        }
        cellStore.setWare(b);
        //console.log(cellStore.getWare)
        //console.log(options)
        //options.push(a)
      }
      const b :WareInfo[] = []
      const options =  reactive([
      {
        value: '自动化叉车库',
        label: '自动化叉车库',
      },
    ]);

      const cellStore = useUserStore()
      const getCell = computed(() => {
        const  cell  = cellStore.getCell ;
        return { cell };
      });
    

      return {
        focus,
        prefixCls,
        getHeaderClass,
        getShowHeaderLogo,
        getHeaderTheme,
        getShowHeaderTrigger,
        getIsMobile,
        getShowBread,
        getShowContent,
        getSplitType,
        getSplit,
        getMenuMode,
        getShowTopMenu,
        getShowLocalePicker,
        getShowFullScreen,
        getShowNotice,
        getUseErrorHandle,
        getLogoWidth,
        getIsMixSidebar,
        getShowSettingButton,
        getShowSetting,
        getShowSearch,
        options,
        value1: (getCell.value.cell),
        handleChange,
        clickNotify,
        
        ...toRefs(notifiData),
      };
    },
  });
</script>
<style lang="less">
  @import './index.less';
</style>
