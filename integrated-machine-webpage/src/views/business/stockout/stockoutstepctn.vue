<template>
  <sample-workflow direction="out" />
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import SampleWorkflow from '../sample-workflow.vue';

/** 保留现有业务路由，复用当前 WMS 取样流程。 */
@Component({ components: { SampleWorkflow } })
export default class StockOutWorkflow extends Vue {}
</script>
