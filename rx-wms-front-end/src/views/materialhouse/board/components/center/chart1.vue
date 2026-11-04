<template>
  <div id="center">
    <!-- 方块显示图 -->
    <div class="square">
      <!-- 正常 -->
      <div
        class="bg-color-black item"
        :style="{ height: YHOne + 'px'}"
        v-for="item in row1"
        :key="item.name"
      >
        <p
          class="colorDarkturquoise font-bold"
          :style="{ 'font-size': YFOne + 'px','text-align': 'left','margin-left': YFOne + 'px'}"
          v-text="item.name"
        ></p>
        <p :style="{ height: Math.round(YFOne/3) + 'px'}"></p>
        <p
          :style="{ height: YHOne + 'px','font-size': Math.round(YFOne*1.6) + 'px','text-align': 'left','margin-left': YFOne + 'px','margin-top': Math.round(YFOne/2) + 'px', 'color': item.color}"
          v-text="item.value"
        ></p>
      </div>

      <!-- 异常 -->
      <div
        class="bg-color-black item"
        :style="{ height: YHOne + 'px'}"
        v-for="item in row2"
        :key="item.name"
      >
        <p
          class="colorLightsteelblue font-bold"
          :style="{ 'font-size': YFOne + 'px','text-align': 'left','margin-left': YFOne + 'px'}"
          v-text="item.name"
        ></p>
        <p :style="{ height: Math.round(YFOne/1.5) + 'px'}"></p>
        <p
          :style="{ height: YHOne + 'px','font-size': Math.round(YFOne*1.6) + 'px','text-align': 'left','margin-left': YFOne + 'px','margin-top': Math.round(YFOne/2) + 'px', 'color': item.color}"
          v-text="item.value"
        ></p>
      </div>
    </div>
  </div>
</template>rts

<script setup lang="ts">
// import { getCurrentData } from "@/api/test/index";
import { ref, onBeforeMount, onMounted, onUnmounted } from 'vue';
    // 获取浏览器可视区域高度（包含滚动条）、 window.innerHeight
    // 获取浏览器可视区域高度（不包含工具栏高度）、document.documentElement.clientHeight
    // 获取body的实际高度  (三个都是相同，兼容性不同的浏览器而设置的) document.body.clientHeight
    let screenHeight = ref(window.innerHeight || document.documentElement.clientHeight ||document.body.clientHeight);
    let screenWidth = ref(window.innerWidth || document.documentElement.clientWidth ||document.body.clientWidth);
    const screenTimer = ref();
    const dataTimer = ref();
    let row1 = ref();
    let row2 = ref();
    let YHOne = ref();
    let YFOne = ref();

  onBeforeMount( () => {
    YHOne.value = Math.round(screenHeight.value * 0.07);
  })  

  onMounted(() => {
    // 页面大小改变时触发
    window.addEventListener("resize", getScreenHeight, false);
    // 页面大小改变时触发
    window.addEventListener("resize", getScreenWidth, false);
    // 鼠标移动时触发
    // window.addEventListener('mousemove',getHeight, false);
    resizeScreen();
    getData();
    // 定时器
    getDataTimer();
  })

  onUnmounted( () => {
    // 清除多次执行定时器
    clearInterval(screenTimer.value);
    screenTimer.value = null;
    // 清除多次执行定时器
    clearInterval(dataTimer.value);
    dataTimer.value = null;
    // 页面大小改变时触发
    window.removeEventListener("resize", getScreenHeight, false);
    // 页面大小改变时触发
    window.removeEventListener("resize", getScreenWidth, false);
  }) 
  
  // 自适应监控定时器
  const resizeScreen = () => {
      screenTimer.value = setInterval(() => {
        getScreenHeight();
        getScreenWidth();
      }, 200);
    }

  // 获取浏览器高度进行自适应
  const getScreenHeight = () => {
      screenHeight.value = window.innerHeight || document.documentElement.clientHeight ||document.body.clientHeight
      // 四舍五入取整数
      YHOne.value = Math.round(screenHeight.value * 0.07);
      //console.log("高度->"+screenHeight +"-"+ kHOne);
    }
    // 字体大小根据宽度自适应
  const getScreenWidth = () => {
      screenWidth.value = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
      YFOne.value = Math.round(screenWidth.value / 150);
      //console.log("宽度->"+screenWidth);
    }
    function randomNum(m: number,n: number){
      var num = Math.floor(Math.random()*(m - n) + n);
      return num;
    }

  const getData = () => {
      row1.value = [];
      row1.value.push({ name: 'column1', value: "正常", color: '#33cea0'})
      row1.value.push({ name: 'column2', value: "正常", color: '#33cea0'})
      row1.value.push({ name: 'column3', value: "故障", color: 'tomato' })
      row1.value.push({ name: 'column4', value: "故障", color: 'tomato' })

      let num1 = randomNum(10000,99999);
      let num2 = randomNum(10000,99999);
      let num3 = randomNum(10000,99999);
      let num4 = randomNum(10000,99999);
      row2.value = [];
      row2.value .push({ name: 'column5', value: num1, color: '#33cea0'})
      row2.value .push({ name: 'column6', value: num2, color: '#33cea0'})
      row2.value .push({ name: 'column7', value: num3, color: '#33cea0'})
      row2.value .push({ name: 'column8', value: num4, color: '#33cea0'})         
      // getCurrentData().then((res) => {
      //   //console.log("ALLDATA->",JSON.stringify(res.data))
      //   row1 = res.data.row1;
      //   row2 = res.data.row2;      
      //   });
      // 获取服务器的数据, 对allData进行赋值之后, 调用updateChart方法更新图表
      //console.log("ALLDATA->",JSON.stringify(res.data))
      //console.log("ALLDATA->",JSON.stringify(res.allData))
    }
    // 定时器
    const getDataTimer = () => {
      dataTimer.value = setInterval(() => {
        getData();
        //console.log("Hello World")
      }, 3000);
    }
</script>

<style lang="less" scoped>
p{
  margin: 0;
}
#center {
  display: flex;
  flex-direction: column;
  .square {
    width: 100%;
    display: flex;
    flex-wrap: wrap;
    justify-content: space-around;
    .item {
      // 控制方块宽度比例
      width: 24.5%;
      border-radius: 6px;
      margin-top: 0.5%;
      margin-bottom: 0.5%;
    }
  }
}
</style>
