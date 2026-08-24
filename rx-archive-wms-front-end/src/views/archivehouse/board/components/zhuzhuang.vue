<template>
  <div ref="chartRef" :style="{ height, width }"></div>
</template>

<script lang="ts" setup>
import { onMounted, ref, Ref } from 'vue';
import { useECharts } from '/@/hooks/web/useECharts';
import { basicProps } from './props';
import {
  getSevenDayTasks
} from './Board';
import {
  SevenDayTasksDto
} from '/@/services/ServiceProxies';
defineProps({
  ...basicProps,
});
const chartRef = ref<HTMLDivElement | null>(null);
const { setOptions } = useECharts(chartRef as Ref<HTMLDivElement>);
const timer = ref(0)
let daysSum: SevenDayTasksDto
onMounted(() => {
  setOptions({
    color: ['#0b20e5', '#00DDFF'],
    title: {
      text: '七日出入库统计',
      left: 'center',
      textStyle: { //主标题文本样式{"fontSize": 18,"fontWeight": "bolder","color": "#333"}
        color: '#ffffff',
        fontSize: 20,
        fontWeight: 'bolder',
      }
    },
    legend: {
      orient: 'vertical',
      left: 'right',
      data: ['入库数', '出库数'],
      textStyle: {
        color: '#ffffff',
        fontSize: 20,
        fontWeight: 'bolder',
      }
    },
    xAxis: {
      type: 'category',
      //设置坐标轴字体颜色和宽度
      axisLine: {  //这是x轴文字颜色
        lineStyle: {
          color: "#ffffff",
        },
      },
      data: ['6', '6', '5', '4', '3', '2', '1'],
      axisLabel: { fontSize: 20 }
    },
    yAxis: {
      type: 'value',
      axisLabel: { fontSize: 20 },
      //设置坐标轴字体颜色和宽度
      axisLine: {  //这是x轴文字颜色
        lineStyle: {
          color: "#ffffff",
        }
      }
    },
    series: [{
      name: '入库数',
      data: [3, 4, 3, 4, 3, 4, 2],
      type: 'bar'
    }, {
      name: '出库数',
      data: [3, 4, 4, 5, 3, 4, 3],
      type: 'bar',
    }]
  });

  timer.value = window.setTimeout(() => {
    updataPage();
  }, 3000);


  async function updataPage() {
    await getSevenDayTasks().then((result) => {
      console.log(result)
      daysSum = result
      setOptions({
        color: ['#0b20e5', '#00DDFF'],
        title: {
          text: '七日出入库统计',
          left: 'center',
          textStyle: { //主标题文本样式{"fontSize": 18,"fontWeight": "bolder","color": "#333"}
            color: '#ffffff',
            fontSize: 20,
            fontWeight: 'bolder',
          }
        },
        legend: {
          orient: 'vertical',
          left: 'right',
          data: ['入库数', '出库数'],
          textStyle: {
            color: '#ffffff',
            fontSize: 20,
            fontWeight: 'bolder',
          }
        },
        xAxis: {
          type: 'category',
          //设置坐标轴字体颜色和宽度
          axisLine: {  //这是x轴文字颜色
            lineStyle: {
              color: "#ffffff",
            },
          },
          data: daysSum.keys,
          axisLabel: { fontSize: 20 }
        },
        yAxis: {
          type: 'value',
          axisLabel: { fontSize: 20 },
          //设置坐标轴字体颜色和宽度
          axisLine: {  //这是x轴文字颜色
            lineStyle: {
              color: "#ffffff",
            }
          }
        },
        series: [{
          name: '入库数',
          data: daysSum.invalue,
          type: 'bar'
        }, {
          name: '出库数',
          data: daysSum.outvalue,
          type: 'bar',
        }]
      });
    })
  }
});

</script>