<template>
  <div>
    <a-row style="height: 50%">
      <BasicTable
        @register="registerTable"
        @selection-change="onSelectChange"
        :clickToRowSelect="true"
        size="small"
      >
        <!-- <template #toolbar> </template> -->
      </BasicTable>
    </a-row>
    <a-row style="height: 50%; margin-left: 15px; margin-right: 15px">
      <BasicTable @register="registerDetailTable" size="small" />
    </a-row>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from 'vue';
  // import { useMessage } from '/@/hooks/web/useMessage';
  import { BasicTable, useTable } from '/@/components/Table';
  import {
    tableColumns,
    tableDetailColumns,
    searchFormSchema,
    getTableListAsync,
    getDetaiTableListAsync,
  } from './TaskHis';
  import { useI18n } from '/@/hooks/web/useI18n';
  // import { Tag } from 'ant-design-vue';
  export default defineComponent({
    name: 'TaskHis',
    components: {
      BasicTable,
      // Tag,
    },
    setup() {
      const { t } = useI18n();
      let selectedBoxIdRef = '';
      // table配置
      const [registerTable, { reload }] = useTable({
        columns: tableColumns,
        formConfig: {
          labelWidth: 70,
          schemas: searchFormSchema,
          fieldMapToTime: [['time', ['startCreationTime', 'endCreationTime']]],
        },
        api: getTableListAsync,
        showTableSetting: true,
        useSearchForm: true,
        bordered: true,
        canResize: true,
        showIndexColumn: true,
        rowKey: 'id', //设置选择项的key
        rowSelection: { type: 'radio' }, // 可尝试其它设置
        clearSelectOnPageChange: true, //换页时清空行选择
        maxHeight: 300,

      });

      //勾选事件
      const onSelectChange = async ({ rows }) => {
        // console.log(rows);
        if (rows.length > 0) {
          selectedBoxIdRef = rows[0].id;
          // selectRows = rows;
          console.log(rows[0].id);
        } else {
          // selectRows = [];
          selectedBoxIdRef = '';
        }

        reloadDetail();
      };
      const [registerDetailTable, { reload: reloadDetail }] = useTable({
        columns: tableDetailColumns,
        api: getPageDetaiTableListAsync,

        showTableSetting: true,
        showIndexColumn: true,
        bordered: true,
        maxHeight: 300,
        canResize: true,

      });
      async function getPageDetaiTableListAsync(params) {
        if (selectedBoxIdRef == '') {
          return [];
        }
        params.taskHisId = selectedBoxIdRef;
        return await getDetaiTableListAsync(params);
      }

      return {
        registerTable,
        onSelectChange,
        registerDetailTable,
        t,
        reload,
        reloadDetail,
      };
    },
  });
</script>
<style lang="less">
  @border-color: #cecece4d;

  @prefix-cls: ~'@{namespace}-basic-table';

  .@{prefix-cls} {
    .ant-table-body {
      height: 400px;
    }
  }
</style>
