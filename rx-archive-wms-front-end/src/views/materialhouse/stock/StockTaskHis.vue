<template>
  <div class="task-history-page">
    <div>
      <BasicTable
        @register="registerTable"
        @selection-change="onSelectChange"
        :clickToRowSelect="true"
        size="small"
      >
        <!-- <template #toolbar> </template> -->
      </BasicTable>
    </div>
    <div class="task-history-detail">
      <BasicTable @register="registerDetailTable" size="small" />
    </div>
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
        // 主从表按内容撑开，避免按视口剩余空间计算时挤占明细区域。
        canResize: false,
        scroll: { y: 300 },
        showIndexColumn: true,
        rowKey: 'id', //设置选择项的key
        rowSelection: { type: 'radio' }, // 可尝试其它设置
        clearSelectOnPageChange: true, //换页时清空行选择

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
        // 明细表始终保留内容高度，多条记录在表内滚动。
        canResize: false,
        scroll: { y: 300 },

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
<style lang="less" scoped>
  .task-history-detail {
    margin: 0 15px;
  }

  // 仅约束本页，少量数据不预留固定空白，明细不会被压缩为零高度。
  .task-history-page :deep(.ant-table-body) {
    height: auto;
  }
</style>
