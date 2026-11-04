<template>
  <div id="index">
    <Loading v-if="loading" :style="{ 'font-weight': 500 }">Loading...</Loading>
    <div v-else class="koi-body">

      <!-- 第一部分-头部-start -->
      <div class="header">
        <!-- 首页 -->
        <span class="koiPage font-bold colorDeepskyblue" @click="handleSkip()"
          :style="{ 'font-size': Math.round(koiParams.screen.screenWidth / 100) + 'px' }">首页</span>
        <!-- 时间 -->
        <div class="localTime colorPink"
          :style="{ 'font-size': Math.round(koiParams.screen.screenWidth / 100) + 'px' }">{{
      koiParams.dateParams.dateYear }} {{ koiParams.dateParams.dateWeek }} {{ koiParams.dateParams.dateDay }}</div>
        <!-- 装饰10 -->
        <Decoration10 class="dv-dec-10-left" />
        <!-- 装饰8 -->
        <Decoration8 class="dv-dec-8-left" :color="decorationColor" />
        <!-- 标题 -->
        <span class="title font-bold colorText"
          :style="{ 'font-size': Math.round(koiParams.screen.screenWidth / 100) + 'px' }">智慧无人档案系统</span>
        <!-- 装饰8 -->
        <Decoration8 class="dv-dec-8-right" :reverse="true" :color="decorationColor" />
        <!-- 装饰10 -->
        <Decoration10 class="dv-dec-10-right" />
      </div>
      <!-- 第一部分-头部-end -->

      <div class="layoutHome">
        <Row>
          <Col :span="6">
          <div :style="{ height: koiParams.height.YHLeftOne + 'px' }">
            <div demo-bg>
              <BorderBox12 style="padding:12px">
                <leftchart1></leftchart1>
              </BorderBox12>
            </div>
          </div>

          <div :style="{ height: koiParams.height.YHLeftTwo + 'px' }">
            <BorderBox12 style="padding:12px">
              <leftchart2></leftchart2>
            </BorderBox12>
          </div>
          </Col>

          <Col :span="9">
          <div :style="{ height: koiParams.height.YHCenterOne + 'px' }">
            <BorderBox12 style="padding:12px">
              <centerchart1></centerchart1>
            </BorderBox12>
          </div>

          <div :style="{ height: koiParams.height.YHCenterTwo + 'px' }">
            <BorderBox12 style="padding:12px">
              <centerchart2></centerchart2>
            </BorderBox12>
          </div>


          <div :style="{ height: koiParams.height.YHCenterThree + 'px' }">
            <BorderBox12 style="padding:12px">
              <centerchart3></centerchart3>
            </BorderBox12>
          </div>
          </Col>




          <Col :span="9">
          <div :style="{ height: koiParams.height.YHRightOne + 'px' }">
            <BorderBox12 style="padding:12px">
              <rightchart1></rightchart1>
            </BorderBox12>
          </div>
          <div :style="{ height: koiParams.height.YHRightTwo + 'px' }">
            <BorderBox12 style="padding:12px">
              <rightchart2></rightchart2>
            </BorderBox12>
          </div>
          <div :style="{ height: koiParams.height.YHRightThree + 'px' }">
            <BorderBox12 style="padding:12px">
              <rightchart3></rightchart3>
            </BorderBox12>
          </div>

          </Col>
        </Row>
      </div>
    </div>
  </div>
</template>
<script lang="ts" setup>
import { defineComponent, ref, $ref, computed, onMounted, onBeforeUnmount, reactive, onDeactivated } from 'vue';
import { Row, Col, Button } from 'ant-design-vue';
import { useECharts } from '/@/hooks/web/useECharts';
import fullScreen from './fullscreen.vue';
import * as echarts from "echarts";
import { FullScreen, } from '/@/layouts/default/header/components';
import { useDesign } from '/@/hooks/web/useDesign';
import { BorderBox12, Decoration10, Decoration8, Loading } from '@kjgl77/datav-vue3'
import SalesProductPie from './components/zhuzhuang.vue';
import leftchart1 from "./components/left/chart1.vue";
import leftchart2 from "./components/left/chart2.vue";
import centerchart1 from "./components/center/chart1.vue";
import centerchart2 from "./components/center/chart2.vue";
import centerchart3 from "./components/center/chart3.vue";
import rightchart1 from "./components/right/chart1.vue";
import rightchart2 from "./components/right/chart2.vue";
import rightchart3 from "./components/right/chart3.vue";
import { formatTime } from './utils/index';
// 单个使用ref
const decorationColor = ref<string[]>(['#568aea', '#000000']); // 装饰8颜色
const loading = ref<boolean>(true);
const weekday = ref<string[]>(['周日', '周一', '周二', '周三', '周四', '周五', '周六']);

// 只用来放置对象和数组，不建议放置单个
const koiParams = reactive<any>({
  // 定时任务对象
  timer: {
    // 时间
    dateTimer: null,
    // 适应浏览器
    koiTimer: null,
    // Loading定时器
    loadingTimer: null
  },
  // 时间参数对象
  dateParams: {
    dateDay: null,
    dateYear: null,
    dateWeek: null
  },
  screen: {
    // 获取浏览器可视区域高度（包含滚动条）、
    // 获取浏览器可视区域高度（不包含工具栏高度）、
    // 获取body的实际高度  (三个都是相同，兼容性不同的浏览器而设置的)
    screenHeight: window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight,
    screenWidth: window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth
  },
  height: {
    YHLeftOne: null,
    YHLeftTwo: null,
    YHCenterOne: null,
    YHCenterTwo: null,
    YHCenterThree: null,
    YHRightOne: null,
    YHRightTwo: null,
    YHRightThree: null
  }
});



//  页面渲染结束
onMounted(() => {
  // 页面大小改变时触发
  window.addEventListener('resize', getScreenHeight, false);
  // 页面大小改变时触发
  window.addEventListener('resize', getScreenWidth, false);
  // 鼠标移动时触发
  // window.addEventListener('mousemove',getHeight, false);
  // 时间定时器
  timeInterval();
  // 取消Loading定时器
  cancelLoading();
  // 自适应屏幕宽高定时器
  resizeScreen();
})

// Vue实例销毁之前
onBeforeUnmount(() => {
  // 清除时间定时器
  clearInterval(koiParams.timer.dateTimer);
  koiParams.timer.dateTimer = null;
  // 清除loading定时器
  clearInterval(koiParams.timer.loadingTimer);
  koiParams.timer.loadingTimer = null;
  // 清除自适应屏幕宽高定时器
  clearInterval(koiParams.timer.koiTimer);
  koiParams.timer.koiTimer = null;
  // 移除页面大小改变时触发事件
  window.removeEventListener('resize', getScreenHeight);
  // 移除页面大小改变时触发事件
  window.removeEventListener('resize', getScreenWidth);
})


const timeInterval = () => {
  koiParams.timer.dateTimer = setInterval(() => {
    const date = new Date()
    koiParams.dateParams.dateDay = formatTime(date, 'HH:mm:ss')
    koiParams.dateParams.dateYear = formatTime(date, 'yyyy/MM/dd')
    koiParams.dateParams.dateWeek = weekday.value[date.getDay()]
  }, 1000)
}

const cancelLoading = () => {
  koiParams.timer.loadingTimer = setTimeout(() => {
    loading.value = false
  }, 500)
}

const resizeScreen = () => {
  koiParams.timer.koiTimer = setInterval(() => {
    getScreenHeight();
    getScreenWidth();
  }, 200)
}

// 获取浏览器高度进行自适应
const getScreenHeight = () => {
  let screenHeight = koiParams.screen.screenHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
  // 四舍五入取整数
  koiParams.height.YHLeftOne = Math.round(screenHeight * 0.46);
  koiParams.height.YHLeftTwo = Math.round(screenHeight * 0.46);
  koiParams.height.YHCenterOne = Math.round(screenHeight * 0.18);
  koiParams.height.YHCenterTwo = Math.round(screenHeight * 0.37);
  koiParams.height.YHCenterThree = Math.round(screenHeight * 0.37);
  koiParams.height.YHRightOne = Math.round(screenHeight * 0.3);
  koiParams.height.YHRightTwo = Math.round(screenHeight * 0.31);
  koiParams.height.YHRightThree = Math.round(screenHeight * 0.31);
  //console.log(screenHeight +"-"+ Math.round(percentHThirty) +"-"+ Math.round(percentHForty));
}
// 字体大小根据宽度自适应
const getScreenWidth = () => {
  const screenWidth = koiParams.screen.screenWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
  console.log("hh-" + screenWidth + "-" + koiParams.screen.screenHeight);
}

// 点击跳转Gitee页面
const handleSkip = () => {
  //window.location.href = 'https://gitee.com/BigCatHome/koi-screen';
}

function fullscreenChange(isFullScreen: boolean) {
  if (isFullScreen) {
    try {
      // @ts-ignore:无法被执行的代码的错误
      veinjs.exitfull();
    } catch (error) {

    }
  } else {
    try {
      // @ts-ignore:无法被执行的代码的错误
      veinjs.openfull();
    } catch (error) {

    }
  }
}





</script>
<style scoped lang="less">
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  list-style-type: none;
  outline: none;
}

html {
  margin: 0;
  padding: 0;
}

body {
  font-family: 'YouYuan';
  background-color: black;
  margin: 0;
  padding: 0;
}

// 让所有斜体不倾斜
em,
i {
  font-style: normal;
}

// 去掉列表前面的小点
li {
  list-style: none;
}

// 图片没有边框 去掉图片dice的空白缝隙
img {
  border: 0; //ie6
  vertical-align: middle;
}

// button 按钮变小手
buttom {
  cursor: pointer
}

// 取消a标签连接下划线
a {
  color: #343440;
  text-decoration: none;
}

a:hover {
  color: #343440;
}

//浮动
.float-r {
  float: right;
}

//浮动
.float-l {
  float: left;
}

// 字体加粗
.font-bold {
  font-weight: bold;
}

//文章一行显示，多余省略号显示
.title-item {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

// 方块背景颜色
.bg-color-black {
  background-color: rgba(19, 25, 47, 0.6);
}

// 常用颜色
.colorBlack {
  color: black !important;
}

.colorGrass {
  color: #33cea0 !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorRed {
  color: red !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorYellow {
  color: yellow !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorLightBlue {
  color: turquoise !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorPink {
  color: pink !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorText {
  color: white !important;

}

.colorBlue {
  color: blue !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorTomato {
  color: tomato !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorDeepskyblue {
  color: deepskyblue !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorDarkturquoise {
  color: darkturquoise !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorLightsalmon {
  color: lightsalmon !important;

  &:hover {
    color: deepskyblue !important;
  }
}

.colorGold {
  color: gold !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorLightsteelblue {
  color: lightsteelblue !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorSienna {
  color: sienna !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorDarkorchid {
  color: darkorchid !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorLightpink {
  color: lightpink !important;

  &:hover {
    color: lightsalmon !important;
  }
}

.colorHotpink {
  color: hotpink !important;

  &:hover {
    color: lightsalmon !important;
  }
}

// 模板样式结束

// 设置默认背景图片
#index {
  color: darkturquoise;
  width: 100%;
  height: 100%;
  // 图片缩放自适应
  background: url("../../../assets/images/pageBg1.jpg") center center no-repeat;

  background-size: 100% 100%;
  position: fixed;
  overflow: hidden;
}

// 最小只支持1000px * 600px，太小了margin-bottom和padding-bottom属性比例不够协调。

.koi-body-png {

  // 头部布局
  .header {
    position: relative;
    top: 0.5vw;
    width: 100%;
    background: url("../../../assets/images/pageBg1.jpg") no-repeat;
    background-size: 100%;
    text-align: center;
    margin: 0;
  }

  // 标题自定义修改
  .title {
    position: absolute;
    left: 46%;
    margin-top: 0.4%;
  }

  // 当前时间css
  .localTime {
    position: absolute;
    right: 1%;
    top: 35%;
  }

  // 子节点首页字体css
  .koiPage {
    position: absolute;
    left: 1%;
    top: 35%;
  }

  // 跳转大屏页面选择框
  .selectPage {
    position: absolute;
    left: 0.4%;
    top: 35%;
  }

  // 轮播图标
  .skipPage {
    position: absolute;
    right: 17%;
    top: 2.2%;
  }

  /* 本项目采用ElementPlus - Layout布局 */
  .layoutHome {
    position: absolute;
    min-width: 1000px;
    min-height: 600px;
    top: 8%;
    width: 100%;
    height: 100%;
  }
}

// 总体布局
.koi-body {

  // 头部布局
  .header {

    // DataV边框10宽度左侧和高度设置
    .dv-dec-10-left {
      width: 25%;
      margin-top: 0.5%;
      margin-left: 0.2%;
      height: 8px;
      float: left;
    }

    // DataV边框10宽度右侧和高度设置
    .dv-dec-10-right {
      width: 25%;
      margin-top: 0.5%;
      margin-left: 0.2%;
      height: 8px;
      float: right;
      // 边框翻转
      transform: rotateY(180deg);
    }

    // DataV边框8左侧宽度和高度设置
    .dv-dec-8-left {
      width: 18%;
      height: 6%;
      margin-top: 0.7%;
      position: absolute;
      left: 25%;
    }

    // DataV边框8宽度右侧和高度设置
    .dv-dec-8-right {
      width: 18%;
      height: 6%;
      margin-top: 0.7%;
      position: absolute;
      right: 25%;
    }

    // 标题自定义修改
    .title {
      position: absolute;
      left: 46%;
      margin-top: 0.8%;
    }

    // 当前时间css
    .localTime {
      position: absolute;
      right: 1%;
      top: 2%;
    }

    // 首页Gitee字体css
    .homePage {
      position: absolute;
      left: 18%;
      top: 2.4%;
    }

    // 子节点首页字体css
    .koiPage {
      position: absolute;
      left: 1%;
      top: 2.2%;
    }

    // 跳转大屏页面选择框
    .selectPage {
      position: absolute;
      left: 0.4%;
      top: 2.2%;
    }

    // 轮播图标
    .skipPage {
      position: absolute;
      right: 17%;
      top: 2.2%;
    }

  }

  /* 本项目采用ElementPlus - Layout布局 */
  .layoutHome {
    position: absolute;
    min-width: 1000px;
    min-height: 600px;
    top: 5.5%;
    width: 100%;
    height: 100%;
  }

}
</style>