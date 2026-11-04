<template>
  <div>
    <BasicTable
      @register="registerTable"
      @selection-change="onSelectChange"
      :clickToRowSelect="true"
      size="small"
    />
    <BasicTable @register="registerdetaildetailTable" size="small" />
  </div>
</template>

<script lang="ts">
  import { defineComponent, ref, onActivated } from 'vue';
  import { BasicTable, useTable } from '/@/components/Table';
  import {
    hisColumns,
    checkhissearchFormSchema,
    hisDetailColumns,
    GetTableDetailHis,
    GetTableHis,
  } from './Check';

  export default defineComponent({
    name: 'CheckHis',
    components: { BasicTable },
    setup() {
      // 主表选择仅用于查询该计划的盘点结果，不再对结果进行审核或库存调整。
      const selectedCheckCode = ref('');
      const [registerTable, { reload }] = useTable({
        columns: hisColumns,
        formConfig: {
          labelWidth: 70,
          schemas: checkhissearchFormSchema,
        },
        api: GetTableHis,
        showTableSetting: true,
        useSearchForm: true,
        bordered: true,
        canResize: true,
        maxHeight: 250,
        showIndexColumn: true,
        rowSelection: { type: 'radio' },
      });

      const [registerdetaildetailTable, { reload: reloadDetail }] = useTable({
        columns: hisDetailColumns,
        api: GetDetailHis,
        bordered: true,
        canResize: false,
        maxHeight: 250,
        showIndexColumn: true,
      });

      /**
       * 查询当前选中计划的执行结果；未选择计划时返回空页。
       * @param params 结果表格的分页和筛选参数。
       * @returns 当前计划的盘点结果分页。
       */
      async function GetDetailHis(params) {
        if (!selectedCheckCode.value) {
          return { items: [], totalCount: 0 };
        }
        params.checkCode = selectedCheckCode.value;
        return await GetTableDetailHis(params);
      }

      // 切换计划时同步加载对应的执行结果。
      const onSelectChange = async ({ rows }) => {
        selectedCheckCode.value = rows.length > 0 ? rows[0].checkCode || '' : '';
        await reloadDetail();
      };

      onActivated(async () => {
        selectedCheckCode.value = '';
        await reload();
        await reloadDetail();
      });

      return {
        registerTable,
        registerdetaildetailTable,
        onSelectChange,
      };
    },
  });
</script>
  
