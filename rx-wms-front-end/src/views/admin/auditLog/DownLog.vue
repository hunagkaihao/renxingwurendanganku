<template>
    <div>
      <!-- <a href="http://www.baidu.com">跳转</a> -->
      <BasicTable @register="registerTable" size="small">
        <template #logFileUrl="{ record, column }">
          <a :href="record[column.dataIndex]" target="_blank">{{ record[column.dataIndex] }}</a>
        </template>
      </BasicTable>
    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent } from 'vue';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { tableColumns, searchFormSchema, getTableListAsync } from './DownLog';
    import { Tag } from 'ant-design-vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    export default defineComponent({
      name: 'DownLog',
      components: {
        BasicTable,
        TableAction,
        Tag,
      },
      setup() {
        const { t } = useI18n();
        // table配置
        const [registerTable, { reload }] = useTable({
          columns: tableColumns,
          
          api: getTableListAsync,
          showTableSetting: true,
          //useSearchForm: true,
          bordered: true,
          canResize: true,
          showIndexColumn: true,
        });

    


  
        return {
          registerTable,
          reload,
          t,
        };
      },
    });
  </script>
  