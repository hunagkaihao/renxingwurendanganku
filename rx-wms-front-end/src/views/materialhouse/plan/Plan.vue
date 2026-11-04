<template>
    <div>
      <BasicTable @register="registerTable"
        @selection-change="onSelectChange"
        :clickToRowSelect="true"
        size="small">
        <template #toolbar>
          <a-button
            type="primary"
            @click="openBatchStockOutTask"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('创建批量出库任务') }}
          </a-button>
          <a-button
            type="primary"
            @click="executePlan"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('执行计划') }}
          </a-button>
          <a-button
            type="primary"
            @click="cancelPlan"
            v-auth="'WarehouseManagement.GoodsManagement.Create'"
          >
            {{ t('取消计划') }}
          </a-button>
        </template>
        <template #isActive="{ record }">
          <Tag :color="record.isActive ? 'green' : 'red'">
            {{ record.isActive ? t('common.enabled') : t('common.disEnabled') }}
          </Tag>
        </template>

      </BasicTable>
      <a-row style="height: 50%; margin-left: 15px; margin-right: 15px">
      <BasicTable @register="registerDetailTable"
       size="small">

      </BasicTable>
      </a-row>
      <BatchStockOutTask
      @register="registerBatchStockOutModal"
      @reload="reload"
      :bodyStyle="{ 'padding-top': '0' }"/>
    </div>
  </template>
  
  <script lang="ts">
    import { defineComponent,ref } from 'vue';
    import { useMessage } from '/@/hooks/web/useMessage';
    import { BasicTable, useTable, TableAction } from '/@/components/Table';
    import { planColumns, searchFormSchema, getTableListAsync, planDetailColumns, executePlanAsync, getPlanTaskListAsync,Delete } from './Plan';
    import BatchStockOutTask from './BatInTask.vue';
    import { useI18n } from '/@/hooks/web/useI18n';
    import { Tag } from 'ant-design-vue';
    import { message } from 'ant-design-vue';
    import { useModal } from '/@/components/Modal';
    export default defineComponent({
      name: 'Plan',
      components: {
        BasicTable,
        TableAction,
        BatchStockOutTask,
        Tag,
      },
      setup() {
        const { createConfirm } = useMessage();
        const [registerBatchStockOutModal, { openModal: openBatchStockOutTask }] = useModal();
        const { t } = useI18n();
        let selectedBoxIdRef = ref(0);
        // table配置
        const [registerTable, { reload }] = useTable({
          columns: planColumns,
          formConfig: {
            labelWidth: 70,
            schemas: searchFormSchema,
          },
          api: getTableListAsync,
          showTableSetting: true,
          useSearchForm: true,
          bordered: true,
          canResize: true,
          showIndexColumn: true,
          rowSelection: { type: 'radio' },
          rowKey: 'id',
          clearSelectOnPageChange: true,
          maxHeight: 300,
        });

        const [registerDetailTable, { reload:reloadDetail}] = useTable({
          columns: planDetailColumns,
          api: getPageDetaiTableListAsync,
          showTableSetting: false,
          showIndexColumn: true,
          bordered: true,
          canResize: false,
          maxHeight: 300,
        });

        async function getPageDetaiTableListAsync(params) {
        if (selectedBoxIdRef.value == 0) {
          return { items: [], totalCount: 0 };
        }
        return await getPlanTaskListAsync(params, selectedBoxIdRef.value);
      }
        //勾选事件
      const onSelectChange = async ({ rows }) => {
        if (rows.length > 0) {
          selectedBoxIdRef.value = rows[0].id;
        } else {
          selectedBoxIdRef.value = 0;
        }
        reloadDetail();
      };
  
  
        //下达盘点计划
        async function executePlan(){
          if(selectedBoxIdRef.value == 0){
            message.error("请先选择计划")
            return
          }
          let msg = t('确认执行计划？');
          let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await executePlanAsync(id);
                reload();
                reloadDetail();
              },
          })
        }
        //取消盘点计划
        async function cancelPlan(){
          if(selectedBoxIdRef.value == 0){
            message.error("请先选择盘点计划")
            return
          }
          let msg = t('确认取消盘点计划？');
          let id = selectedBoxIdRef.value
            createConfirm({
              iconType: 'warning',
              title: t('common.tip'),
              content: msg,
              onOk: async () => {
                await Delete(id);
                reload();
              },
          })
        }
        return {
          registerTable,
          registerDetailTable,
          executePlan,
          cancelPlan,
          t,
          reload,
          openBatchStockOutTask,
          onSelectChange,
          registerBatchStockOutModal,
        };
      },
    });
  </script>
