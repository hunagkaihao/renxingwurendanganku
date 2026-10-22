<template>
  <section class="sample-cabinets" :aria-label="operation + '柜门布局'">
    <div class="sample-cabinet-intro">
      <div><h2>请选择{{ operation }}柜门</h2><p>上层膜料盒 · 下层样品瓶</p></div>
      <span>共 22 个柜门</span>
    </div>
    <section v-for="group in groups" :key="group.key" class="sample-cabinet-group">
      <header><h3>{{ group.title }}</h3><span>{{ group.count }} 个柜门</span></header>
      <div class="sample-door-grid" :class="'sample-door-grid-' + group.key">
        <button v-for="number in group.count" :key="number" type="button"
          class="sample-door" :class="{ 'is-selected': selectedDoor === group.key + '-' + number }"
          :aria-pressed="selectedDoor === group.key + '-' + number"
          @click="selectDoor(group.key, group.title, number)">
          <span class="sample-door-number">{{ number < 10 ? '0' + number : number }}</span>
          <Icon :type="group.key === 'film' ? 'md-albums' : 'ios-flask-outline'" size="30" />
          <strong>{{ number }} 号门</strong><small>待接入</small>
        </button>
      </div>
    </section>
    <div class="sample-selection" role="status">
      <Icon type="ios-information-circle-outline" size="22" />
      <div><strong>{{ selectedLabel || '点击柜门可选择查看' }}</strong><p>当前为柜门布局调试，设备状态与柜门编号尚未对接，选择柜门不会发送开门指令。</p></div>
    </div>
  </section>
</template>
<script lang="ts">
import { Component, Prop } from 'vue-property-decorator';
import AbpBase from '../../../lib/abpbase';

/** 仅 WMS 环境的留样、取样柜布局；界面编号不作为后端设备编号使用。 */
@Component
export default class SampleCabinets extends AbpBase {
  /** 操作名称用于标题和无障碍提示，默认保持留样入口文案。 */
  @Prop({ type: String, default: '留样' }) operation!: string;
  /** 柜门按物料分层，各层从 1 开始编号。 */
  groups = [
    { key: 'film', title: '上层 · 膜料盒柜门', count: 10 },
    { key: 'bottle', title: '下层 · 样品瓶柜门', count: 12 }
  ];
  /** 仅用于高亮的界面标识，不对应 PLC 地址。 */
  selectedDoor:string = '';
  /** 当前选中柜门的可读名称。 */
  selectedLabel:string = '';
  /** 选择布局中的柜门，不触发任务或设备控制请求。 */
  selectDoor(group:string, title:string, number:number) {
    this.selectedDoor = group + '-' + number;
    this.selectedLabel = title + ' · ' + number + ' 号门';
  }
}
</script>
<style scoped lang="less">
.sample-cabinets { min-width: 0; padding: 20px 0; padding: ~"clamp(14px, 2vh, 28px) 0"; color: #2b405e; }
.sample-cabinet-intro { display: flex; align-items: center; justify-content: space-between; gap: 16px; margin-bottom: 24px; }
.sample-cabinet-intro h2 { font-size: 25px; font-weight: 600; }
.sample-cabinet-intro p { margin-top: 8px; color: #52657d; font-size: 15px; }
.sample-cabinet-intro > span { padding: 9px 14px; border-radius: 20px; background: #cbd9ec; color: #324f7a; white-space: nowrap; }
.sample-cabinet-group { margin-bottom: 22px; padding: 22px; border: 1px solid #9fadc1; border-radius: 18px; background: #f7f9fc; box-shadow: 0 2px 5px rgba(35, 53, 77, .06); }
.sample-cabinet-group header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; gap: 12px; }
.sample-cabinet-group h3 { font-size: 19px; font-weight: 600; }
.sample-cabinet-group header span { color: #52657d; white-space: nowrap; }
.sample-door-grid { display: grid; gap: 10px; }
.sample-door-grid-film { grid-template-columns: repeat(10, minmax(0, 1fr)); }
.sample-door-grid-bottle { grid-template-columns: repeat(12, minmax(0, 1fr)); }
.sample-door { min-width: 0; min-height: 120px; min-height: ~"clamp(100px, 15vh, 190px)"; display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 8px; padding: 12px 4px; border: 1px solid #8da5c6; border-radius: 11px; background: #e0e9f5; color: #315889; font-family: inherit; cursor: pointer; touch-action: manipulation; transition: background .18s, transform .18s; }
.sample-door-grid-bottle .sample-door { color: #28675d; background: #deece7; border-color: #8bad9f; }
.sample-door-number { align-self: flex-start; padding-left: 5px; font-size: 12px; color: #51647b; }
.sample-door strong { font-size: 15px; white-space: nowrap; }
.sample-door small { font-size: 12px; color: #51647b; }
.sample-door.is-selected { border-color: #315fa5; box-shadow: 0 0 0 2px #315fa5; background: #c5d9f5; }
.sample-door:focus { outline: 3px solid #95b6eb; outline-offset: 3px; }
.sample-door:active { transform: scale(.97); }
.sample-selection { display: flex; gap: 12px; padding: 18px; border: 1px solid #aab9cc; border-radius: 14px; background: #e8edf4; color: #455d7b; }
.sample-selection > div { min-width: 0; overflow-wrap: break-word; }
.sample-selection > i { flex-shrink: 0; }
.sample-selection p { margin-top: 5px; line-height: 1.7; font-size: 13px; }
@media (max-width: 900px) { .sample-door-grid-film { grid-template-columns: repeat(5, minmax(0, 1fr)); } .sample-door-grid-bottle { grid-template-columns: repeat(6, minmax(0, 1fr)); } }
@media (max-width: 600px) { .sample-cabinet-group { padding: 16px 12px; } .sample-door-grid { grid-template-columns: repeat(3, minmax(0, 1fr)); } .sample-cabinet-intro { flex-wrap: wrap; } .sample-cabinet-intro h2 { font-size: 22px; } }
@media (max-height: 800px) and (min-width: 901px) {
  .sample-cabinet-intro { margin-bottom: 14px; }
  .sample-cabinet-intro h2 { font-size: 22px; }
  .sample-cabinet-intro p { margin-top: 4px; font-size: 13px; }
  .sample-cabinet-group { padding: 14px; margin-bottom: 14px; }
  .sample-cabinet-group header { margin-bottom: 12px; }
  .sample-door-grid { gap: 8px; }
  .sample-door { min-height: 100px; gap: 4px; padding: 8px 3px; }
  .sample-door strong { font-size: 14px; }
  .sample-selection { padding: 12px 16px; }
}
@media (min-width: 1600px) and (min-height: 900px) {
  .sample-cabinet-intro h2 { font-size: 29px; }
  .sample-cabinet-group h3 { font-size: 22px; }
  .sample-door-grid { gap: 14px; }
  .sample-door strong { font-size: 19px; }
  .sample-door small, .sample-door-number { font-size: 15px; }
}
/* 与外层整屏网格配合，两层柜门均分剩余高度；不通过隐藏溢出来裁切柜门。 */
@media (min-width: 901px) and (min-height: 600px) {
  .sample-cabinets { min-height: 0; padding: 0; display: grid; grid-template-rows: auto minmax(0, 1fr) minmax(0, 1fr) auto; gap: 10px; }
  .sample-cabinet-intro { margin: 0; }
  .sample-cabinet-intro h2 { font-size: 21px; }
  .sample-cabinet-intro p { margin-top: 3px; font-size: 13px; }
  .sample-cabinet-group { min-height: 0; margin: 0; padding: 10px 14px; display: grid; grid-template-rows: auto minmax(0, 1fr); gap: 8px; }
  .sample-cabinet-group header { margin: 0; }
  .sample-cabinet-group h3 { font-size: 17px; }
  .sample-door-grid { min-height: 0; }
  .sample-door { min-height: 0; padding: 5px 3px; gap: 3px; }
  .sample-selection { padding: 10px 14px; align-items: center; }
  .sample-selection p { margin-top: 2px; }
}
@media (min-width: 901px) and (min-height: 600px) and (max-height: 800px) {
  .sample-cabinet-intro h2 { font-size: 18px; }
  .sample-cabinet-intro p { display: none; }
  .sample-cabinet-intro > span { padding: 4px 12px; }
  /* 门牌已包含编号，紧凑屏幕省去重复编号以保留点击区域。 */
  .sample-door-number { display: none; }
  .sample-door > i { font-size: 24px !important; }
  .sample-door strong { font-size: 14px; }
  .sample-door small { font-size: 12px; }
  .sample-selection { padding-top: 7px; padding-bottom: 7px; }
}
@media (prefers-reduced-motion: reduce) { .sample-door { transition: none; } .sample-door:active { transform: none; } }
</style>
