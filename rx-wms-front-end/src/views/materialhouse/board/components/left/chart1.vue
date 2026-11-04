<!-- left1 -->
<template>
  <div ref="refChart" :style="{ height: YHOne + 'px'}"></div>
</template>

<script setup lang="ts">
import { ref, onBeforeMount, onMounted, onUnmounted } from 'vue';
// import { getModuleData } from "../../../api/ems/index";
import * as echarts from 'echarts';
// 导入echarts皮肤
import skin from '../../../../../assets/theme/skin'; 
  // 获取ref
  let refChart = ref<any>()
  // 获取浏览器可视区域高度（包含滚动条）、 window.innerHeight
  // 获取浏览器可视区域高度（不包含工具栏高度）、document.documentElement.clientHeight
  // 获取body的实际高度  (三个都是相同，兼容性不同的浏览器而设置的) document.body.clientHeight
  let screenHeight = ref(window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight);
  let screenWidth = ref(window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth);
  // 初始化图表对象
  let chartInstance = ref();
  // 接口数据
  const allData = ref([
      { value: 5, name: 'AABB故障' },
      { value: 6, name: 'CCDD故障' },
      { value: 7, name: 'TTZZ故障' },
      { value: 8, name: 'GGHH故障' },
      { value: 9, name: 'YYXX故障' }
  ]);
  // echarts图表高度
  let YHOne = ref();
  // 接口数据定时器（需要定时刷新接口是打开）
  const dataTimer = ref();
  // 局部屏幕自适应定时器
  const screenTimer = ref();
  // tootip定时器ID-用于清除定时器
  const tootipTimer = ref();

  onBeforeMount(() => {
    YHOne.value = Math.round(screenHeight.value * 0.43)
  })
  
  onMounted(() => {
    // 页面大小改变时触发
    window.addEventListener('resize', getScreenHeight, false);
    // 页面大小改变时触发
    window.addEventListener('resize', getScreenWidth, false);
    // 鼠标移动时触发
    //window.addEventListener('mousemove',getHeight, false);
    resizeScreen();
    // 初始化统计报表对象
    if (chartInstance.value) {
      chartInstance.value.dispose();
    }
    // 图表初始化
    initChart();
    // 获取接口数据
    getData();
    // 开启图表自适应
    window.addEventListener("resize", screenAdapter);
    screenAdapter();
    // 局部刷新定时器
    // getDataTimer();
    // Tootip刷新定时器
    getTootipTimer();
  })

  onUnmounted(() => {
    // 清除自适应屏幕定时器
    clearInterval(dataTimer.value);
    dataTimer.value = null;
    // 清除局部刷新定时器
    clearInterval(screenTimer.value);
    screenTimer.value = null;
    // 页面大小改变时触发
    window.removeEventListener('resize', getScreenHeight);
    // 页面大小改变时触发
    window.removeEventListener('resize', getScreenWidth);
    window.removeEventListener("resize", screenAdapter);
    // 清除tootip刷新定时器
    clearInterval(tootipTimer.value);
    tootipTimer.value = null;
    // 销毁Echarts图表
    chartInstance.value.dispose();
    chartInstance.value = null;
  })

  const resizeScreen = () => {
      dataTimer.value = setInterval(() => {
        getScreenHeight();
        getScreenWidth();
      }, 200)
  }

  // 获取浏览器高度进行自适应
  const getScreenHeight = () => {
      screenHeight.value = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
      // 四舍五入取整数
      YHOne.value = Math.round(screenHeight.value * 0.43);
      //console.log("高度->"+screenHeight +"-"+ YHOne);
  }

    // 字体大小根据宽度自适应
  const getScreenWidth = () => {
      screenWidth.value = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
      //console.log("宽度->"+screenWidth);
  }

  const initChart = () => {
      // 覆盖默认主题
      echarts.registerTheme('myTheme', skin);
      // 定义实例
      chartInstance.value = echarts.init(refChart.value, 'myTheme');
      const initOption = {
        title: {
          text: "🍀近7天模块故障",
          left: 'center'
        },
        tooltip: {
          trigger: 'item'
        },
        legend: {
          orient: 'vertical',
          left: 'left',
        },
        series: [
          {
            name: '模块故障',
            type: 'pie',
            // 环形图大小
            radius: ['45%', '70%'],
            // 环形图位置
            center: ["56%", "60%"],
            avoidLabelOverlap: false,
            itemStyle: {
              borderRadius: 10,
              borderColor: '#fff',
              borderWidth: 2
            },
            label: {
              show: false,
              position: 'center',
              formatter: '{d}%' // 当前百分比
            },
            emphasis: {
              label: {
                show: true,
                fontSize: '16',
                fontWeight: 'bold'
              }
            },
            labelLine: {
              show: false
            },
          }
        ]
      };
      // 图表初始化配置
      chartInstance.value.setOption(initOption);

      // 鼠标移入停止定时器
      chartInstance.value.on("mouseover", () => {
        clearInterval(tootipTimer.value);
      });

      // 鼠标移入启动定时器
      chartInstance.value.on("mouseout", () => {
        getTootipTimer();
      });
    }


    const getData = () => {
      // getModuleData().then(res => {
      //     allData = res.data;
      //     updateChart(); 
      //     //console.log("数据left1->"+JSON.stringify(allData.pcsSum));
      //     // echarts查不到数据，将初始化echarts的方法全部放置到接口方法中即可。  
      // })         
      // 获取服务器的数据, 对allData进行赋值之后, 调用updateChart方法更新图表
      //console.log("ALLDATA->",JSON.stringify(res.data))
      //console.log("ALLDATA->",JSON.stringify(res.allData))
      updateChart();
    }

    const updateChart = () => {
      // 处理图表需要的数据
      const dataOption = {
        series: [
          {
            data: allData.value
          }
        ]
      };
      // 图表数据变化配置
      chartInstance.value.setOption(dataOption);
    }

    const screenAdapter = () => {
      let titleFontSize = (refChart.value.offsetWidth / 100) * 2;
      const adapterOption = {
        title: {
          textStyle: {
            fontSize: Math.round(titleFontSize * 2),
          },
        },
        // 圆点分类标题
        legend: {
          textStyle: {
            fontSize: Math.round(titleFontSize * 1.2),
          },
        }
      };
      // 图标自适应变化配置
      chartInstance.value.setOption(adapterOption);
      chartInstance.value.resize();
    }

    // 数据刷新定时器
    const getDataTimer = () => {
      screenTimer.value = setInterval(() => {
        // 执行刷新数据的方法
        getData();
        //console.log("Hello World")
      }, 3000)
    }

    // 定时器
    const getTootipTimer = () => {
      let index = 0; // 播放所在下标
      tootipTimer.value = setInterval(() => {
        // echarts实现定时播放tooltip
        chartInstance.value.dispatchAction({
          type: 'showTip',
          seriesIndex: 0,
          dataIndex: index
        });
        index++;
        if (index > allData.value.length) {
          index = 0;
        }
        //console.log("Hello World")
      }, 2000)
    }
</script>

<style lang='scss' scoped>

</style>