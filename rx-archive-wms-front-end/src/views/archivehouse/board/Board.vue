<template>
  <!-- style="width:2560px;height:1440px" -->
  <div class="boardpage" >
    <!-- 顶部 -->
    <nav class="navbar-default" > 
      <h2>智慧无人档案系统</h2>
      <div class="full_screen">
      <full-screen v-model="isFullScreen" @on-change="fullscreenChange" ></full-screen>
      </div>
      </nav>

    <!-- 边框 -->
<div  h22rem flex="~ col" p3 justify-center items-center bg-dark>
  <dv-border-box1 ref="borderRef">
    <div  h18rem color-white flex justify-center items-center>

      <Row>
        <Col :span="8">
          <div demo-bg>
            <dv-border-box12>
              <div class="container">
                <!-- 柱状图 -->
                <SalesProductPie class="enter-y" :loading="loading"/>
              </div>
            </dv-border-box12>
          </div>
          
        </Col>
   

      </Row>

    </div>
  </dv-border-box1>
</div>


  </div>
</template>
<script lang="ts">
import { defineComponent, ref, $ref , computed, onMounted, reactive, onDeactivated } from 'vue';
import { Row , Col ,Button } from 'ant-design-vue'; 
import { useECharts } from '/@/hooks/web/useECharts';
import fullScreen from './fullscreen.vue';
import * as echarts from "echarts";
import { FullScreen, } from '/@/layouts/default/header/components';
import { useDesign } from '/@/hooks/web/useDesign';
import SalesProductPie from './components/zhuzhuang.vue';




export default defineComponent({
  components: {
      Row,
      Col,
      Button,
      FullScreen,
      fullScreen,
      SalesProductPie,
    },
  
  setup() {
    let isFullScreen:boolean=false;
    const { prefixCls } = useDesign('layout-header');

    const loading = ref(true);

    function fullscreenChange (isFullScreen:boolean) {
    if(isFullScreen){
    try {
            // @ts-ignore:无法被执行的代码的错误
            veinjs.exitfull();
          } catch (error) {

          } 
        }else{
          try {
            // @ts-ignore:无法被执行的代码的错误
            veinjs.openfull();
          } catch (error) {
            
          } 
        }
    }

    

    return {
      isFullScreen,
      fullscreenChange,
      prefixCls,
      loading,
    };

    


  }
})

</script>
<style scoped  lang="less">

:global(.dv-scroll-board .rows .row-item) {
    font-size: 24px;
}
:global(.dv-scroll-board .header) {
    font-size: 20px;
}
.boardpage
{
  width: 100%;
  height: 100%;
  /* background: #667aa6; */
  background: url(../../../assets/images/pageBg1.jpg)  no-repeat;
  background-size:100% 100%;
}

.navbar-default
{
border: none;
background-color: #293c55;
z-index: 10000;
-webkit-transition: background-color 0.5s linear;
-o-transition: background-color 0.5s linear;
transition: background-color 0.5s cubic-bezier(.65,.32,1,1);
height: 50px;
text-align: center;

}
.navbar-default h2 {
color: #ffffff;
font-size: 23px;
margin: 0 0 0 60px;
font-family: "Myriad Pro", "Helvetica Neue", Arial, Helvetica, sans-serif;

display: inline-block;
vertical-align: middle;
padding-top: 10px;
}
.navbar-default
{
  background: url(../../../assets/images/nav.png)  no-repeat;
  background-position:center;
  background-size:auto 100%;
        height: 70px;
}
#digital-flop {
  position: relative;
  height: 15%;
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: rgba(6, 30, 93, 0.5);
  margin-left: 0px;
  margin-right: 0px;
  margin-top: 5px;
    color: #ffffff;
    font-family:"Myriad Pro", "Helvetica Neue", Arial, Helvetica, sans-serif;

.dv-decoration-10 {
position: absolute;
width: 100%;
left: 0%;
height: 5px;
bottom: 0px;
}

.digital-flop-item {
width: 17%;
height: 80%;
display: flex;
flex-direction: column;
justify-content: center;
align-items: center;
border-left: 3px solid rgb(6, 30, 93);
border-right: 3px solid rgb(6, 30, 93);
}

.digital-flop-title {
font-size: 20px;
margin-bottom: 5px;
margin-top: 10px;
}

.unit{
  font-size: 18px;
}

.digital-flop {
margin-left: 10px;
display: flex;
}

h3{
  color: #ffffff;
  font-size: 23px;
}

.unit {
margin-left: 10px;
display: flex;
align-items: flex-end;
box-sizing: border-box;
padding-bottom: 13px;
}
}

.full_screen{
float: right;
margin-right: 30px;
padding-top: 10px;
}
.container{
padding: 10px;
font-size: 20;
}
.dv-scroll-board .header  {
  display: flex;
  flex-direction: row;
  font-size: 20px !important ;
}
/* 配置滚动条 */
::-webkit-scrollbar {
          width: 0px;    
          /*height: 4px;*/
      }


</style>
  
 