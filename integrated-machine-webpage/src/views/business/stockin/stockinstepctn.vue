<template>
  <sample-workflow direction="in" />
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import SampleWorkflow from '../sample-workflow.vue';

/** 保留现有业务路由，复用当前 WMS 留样流程。 */
@Component({ components: { SampleWorkflow } })
export default class StockInWorkflow extends Vue {}
</script>
